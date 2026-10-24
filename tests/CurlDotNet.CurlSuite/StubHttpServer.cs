using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace CurlDotNet.CurlSuite;

/// <summary>
/// A from-scratch HTTP/1.x stub server with the behaviour of curl's tests/server/sws.c:
/// it records every request byte-for-byte, picks the reply part from the number after the
/// last slash in the request target (NNNN0002 means test NNNN part 2), honours
/// <c>swsclose</c>/<c>swsbounce</c>, answers <c>Expect: 100-continue</c>, and bumps the part
/// number for Digest/NTLM/Basic/Negotiate challenges exactly like sws does.
/// Built on TcpListener only so there is nothing between the test and the wire.
/// </summary>
public sealed class StubHttpServer : IDisposable
{
    private readonly TcpListener _listener;
    private Dictionary<int, string> _replies;               // part -> fully preprocessed reply text (Latin-1)
    private readonly CancellationTokenSource _cts = new();
    private readonly List<byte[]> _requests = new();
    private readonly List<byte[]> _bodies = new();
    private readonly object _lock = new();
    private int _previousPart = -1;
    private bool _bounceNext;
    private int _negotiateCount;
    private Task? _acceptLoop;

    public StubHttpServer(Dictionary<int, string> replies)
    {
        _replies = replies;
        _listener = new TcpListener(IPAddress.Loopback, 0);
    }

    public int Port => ((IPEndPoint)_listener.LocalEndpoint).Port;

    /// <summary>Installs the reply parts once the port is known and the test file has been preprocessed.</summary>
    public void SetReplies(Dictionary<int, string> replies)
    {
        lock (_lock) _replies = replies;
    }

    /// <summary>Raw requests in arrival order (request line, headers and body as received).</summary>
    public IReadOnlyList<byte[]> Requests { get { lock (_lock) return _requests.ToList(); } }

    /// <summary>Request bodies in arrival order, de-chunked, for &lt;upload&gt; verification.</summary>
    public IReadOnlyList<byte[]> Bodies { get { lock (_lock) return _bodies.ToList(); } }

    public void Start()
    {
        _listener.Start();
        _acceptLoop = Task.Run(AcceptLoopAsync);
    }

    private async Task AcceptLoopAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            TcpClient client;
            try
            {
                client = await _listener.AcceptTcpClientAsync(_cts.Token);
            }
            catch (OperationCanceledException) { return; }
            catch (ObjectDisposedException) { return; }
            catch (SocketException) { return; }

            _ = Task.Run(() => ServeConnectionAsync(client));
        }
    }

    private async Task ServeConnectionAsync(TcpClient client)
    {
        using var _ = client;
        client.NoDelay = true;
        using var stream = client.GetStream();
        var buffer = new MemoryStream();

        try
        {
            while (!_cts.IsCancellationRequested)
            {
                var request = await ReadRequestAsync(stream, buffer);
                if (request == null)
                {
                    return; // client went away
                }

                lock (_lock)
                {
                    _requests.Add(request.Raw);
                    if (request.Body.Length > 0 || request.HasBody)
                    {
                        _bodies.Add(request.Body);
                    }
                }

                var part = SelectPart(request);
                string? reply;
                lock (_lock)
                {
                    if (_bounceNext && _previousPart >= 0)
                    {
                        part = _previousPart + 1;
                    }
                    _replies.TryGetValue(part, out reply);
                    _previousPart = part;
                    _bounceNext = reply != null && reply.Contains("swsbounce", StringComparison.Ordinal);
                }

                var close = request.ConnectionClose;
                byte[] bytes;
                if (reply == null)
                {
                    // sws has no page for an unknown part: it just drops the connection
                    try { client.Client.Shutdown(SocketShutdown.Both); } catch { /* ignore */ }
                    return;
                }
                else
                {
                    if (reply.Contains("swsclose", StringComparison.Ordinal))
                    {
                        close = true;
                    }
                    if (!HasFramedBody(reply))
                    {
                        close = true; // only EOF can delimit this body
                    }
                }

                bytes = Encoding.Latin1.GetBytes(reply);
                await stream.WriteAsync(bytes, 0, bytes.Length, _cts.Token);
                await stream.FlushAsync(_cts.Token);

                if (close)
                {
                    try { client.Client.Shutdown(SocketShutdown.Send); } catch { /* ignore */ }
                    return;
                }
            }
        }
        catch (IOException) { }
        catch (ObjectDisposedException) { }
        catch (OperationCanceledException) { }
        catch (SocketException) { }
    }

    private sealed class ParsedRequest
    {
        public byte[] Raw = Array.Empty<byte>();
        public byte[] Body = Array.Empty<byte>();
        public bool HasBody;
        public string RequestLine = string.Empty;
        public string Target = string.Empty;
        public List<KeyValuePair<string, string>> Headers = new();
        public bool ConnectionClose;

        public string? Header(string name) =>
            Headers.FirstOrDefault(h => string.Equals(h.Key, name, StringComparison.OrdinalIgnoreCase)).Value;
    }

    private static bool HasFramedBody(string reply)
    {
        var headerEnd = reply.IndexOf("\r\n\r\n", StringComparison.Ordinal);
        var head = headerEnd < 0 ? reply : reply.Substring(0, headerEnd);
        if (Regex.IsMatch(head, @"(?im)^Content-Length:\s*\d+")) return true;
        if (Regex.IsMatch(head, @"(?im)^Transfer-Encoding:.*chunked")) return true;
        // Replies without a body at all (1xx, 204, 304) are self-delimiting.
        if (Regex.IsMatch(head, @"^HTTP/\d\.\d (1\d\d|204|304)")) return true;
        return false;
    }

    private async Task<ParsedRequest?> ReadRequestAsync(NetworkStream stream, MemoryStream pending)
    {
        var chunk = new byte[16 * 1024];
        int headerEnd;
        int sepLen;

        while (true)
        {
            var data = pending.GetBuffer();
            var len = (int)pending.Length;
            (headerEnd, sepLen) = FindHeaderEnd(data, len);
            if (headerEnd >= 0) break;

            using var readCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
            readCts.CancelAfter(TimeSpan.FromSeconds(20));
            int read;
            try
            {
                read = await stream.ReadAsync(chunk, 0, chunk.Length, readCts.Token);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            if (read <= 0)
            {
                return null;
            }
            pending.Write(chunk, 0, read);
        }

        var all = pending.GetBuffer();
        var total = (int)pending.Length;
        var headText = Encoding.Latin1.GetString(all, 0, headerEnd);
        var request = new ParsedRequest();
        var headLines = headText.Split('\n').Select(l => l.TrimEnd('\r')).ToList();
        request.RequestLine = headLines.Count > 0 ? headLines[0] : string.Empty;
        var reqParts = request.RequestLine.Split(' ');
        request.Target = reqParts.Length > 1 ? reqParts[1] : string.Empty;
        foreach (var line in headLines.Skip(1))
        {
            var colon = line.IndexOf(':');
            if (colon > 0)
            {
                request.Headers.Add(new KeyValuePair<string, string>(line.Substring(0, colon).Trim(), line.Substring(colon + 1).Trim()));
            }
        }

        var bodyStart = headerEnd + sepLen;
        request.ConnectionClose = string.Equals(request.Header("Connection"), "close", StringComparison.OrdinalIgnoreCase)
                                  || request.RequestLine.EndsWith("HTTP/1.0", StringComparison.Ordinal);

        if (string.Equals(request.Header("Expect"), "100-continue", StringComparison.OrdinalIgnoreCase))
        {
            var cont = Encoding.ASCII.GetBytes("HTTP/1.1 100 Continue\r\n\r\n");
            await stream.WriteAsync(cont, 0, cont.Length, _cts.Token);
            await stream.FlushAsync(_cts.Token);
        }

        var contentLength = request.Header("Content-Length");
        var transferEncoding = request.Header("Transfer-Encoding");
        var rawEnd = bodyStart;

        if (contentLength != null && long.TryParse(contentLength, out var cl) && cl > 0)
        {
            request.HasBody = true;
            var needed = bodyStart + (int)cl;
            await EnsureBytesAsync(stream, pending, needed);
            all = pending.GetBuffer();
            request.Body = all.Skip(bodyStart).Take((int)cl).ToArray();
            rawEnd = needed;
        }
        else if (transferEncoding != null && transferEncoding.Contains("chunked", StringComparison.OrdinalIgnoreCase))
        {
            request.HasBody = true;
            var (body, end) = await ReadChunkedAsync(stream, pending, bodyStart);
            request.Body = body;
            rawEnd = end;
        }
        else if (contentLength != null)
        {
            request.HasBody = true; // Content-Length: 0
        }

        all = pending.GetBuffer();
        total = (int)pending.Length;
        request.Raw = all.Take(rawEnd).ToArray();

        // Keep any pipelined bytes for the next request on this connection.
        var leftover = all.Skip(rawEnd).Take(total - rawEnd).ToArray();
        pending.SetLength(0);
        if (leftover.Length > 0)
        {
            pending.Write(leftover, 0, leftover.Length);
        }

        return request;
    }

    private static (int end, int sepLen) FindHeaderEnd(byte[] data, int len)
    {
        for (var i = 0; i + 1 < len; i++)
        {
            if (data[i] == (byte)'\r' && i + 3 < len && data[i + 1] == (byte)'\n' && data[i + 2] == (byte)'\r' && data[i + 3] == (byte)'\n')
            {
                return (i, 4);
            }
            if (data[i] == (byte)'\n' && data[i + 1] == (byte)'\n')
            {
                return (i, 2);
            }
        }
        return (-1, 0);
    }

    private async Task EnsureBytesAsync(NetworkStream stream, MemoryStream pending, int needed)
    {
        var chunk = new byte[16 * 1024];
        while (pending.Length < needed)
        {
            using var readCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
            readCts.CancelAfter(TimeSpan.FromSeconds(20));
            var read = await stream.ReadAsync(chunk, 0, chunk.Length, readCts.Token);
            if (read <= 0) break;
            pending.Write(chunk, 0, read);
        }
    }

    private async Task<(byte[] body, int rawEnd)> ReadChunkedAsync(NetworkStream stream, MemoryStream pending, int start)
    {
        var body = new MemoryStream();
        var pos = start;
        while (true)
        {
            // chunk-size line
            int lineEnd;
            while ((lineEnd = IndexOf(pending, (byte)'\n', pos)) < 0)
            {
                await EnsureBytesAsync(stream, pending, (int)pending.Length + 1);
                if (pending.Length <= pos) return (body.ToArray(), (int)pending.Length);
            }
            var data = pending.GetBuffer();
            var sizeLine = Encoding.ASCII.GetString(data, pos, lineEnd - pos).Trim();
            var semi = sizeLine.IndexOf(';');
            if (semi >= 0) sizeLine = sizeLine.Substring(0, semi);
            if (!int.TryParse(sizeLine, System.Globalization.NumberStyles.HexNumber, null, out var size))
            {
                return (body.ToArray(), lineEnd + 1);
            }
            pos = lineEnd + 1;
            if (size == 0)
            {
                // trailers until blank line
                while (true)
                {
                    while ((lineEnd = IndexOf(pending, (byte)'\n', pos)) < 0)
                    {
                        await EnsureBytesAsync(stream, pending, (int)pending.Length + 1);
                        if (pending.Length <= pos) return (body.ToArray(), (int)pending.Length);
                    }
                    data = pending.GetBuffer();
                    var trailer = Encoding.ASCII.GetString(data, pos, lineEnd - pos).TrimEnd('\r');
                    pos = lineEnd + 1;
                    if (trailer.Length == 0) break;
                }
                return (body.ToArray(), pos);
            }

            await EnsureBytesAsync(stream, pending, pos + size + 2);
            data = pending.GetBuffer();
            body.Write(data, pos, Math.Min(size, (int)pending.Length - pos));
            pos += size;
            // skip CRLF after chunk data
            if (pos < pending.Length && data[pos] == (byte)'\r') pos++;
            if (pos < pending.Length && data[pos] == (byte)'\n') pos++;
        }
    }

    private static int IndexOf(MemoryStream ms, byte value, int from)
    {
        var data = ms.GetBuffer();
        for (var i = from; i < ms.Length; i++)
        {
            if (data[i] == value) return i;
        }
        return -1;
    }

    /// <summary>Mirrors sws.c: number after the last '/' selects test and part; auth headers bump the part.</summary>
    private int SelectPart(ParsedRequest request)
    {
        var target = request.Target;
        var part = 0;
        var slash = target.LastIndexOf('/');
        if (slash >= 0)
        {
            var digits = new string(target.Skip(slash + 1).TakeWhile(char.IsDigit).ToArray());
            if (digits.Length > 0 && long.TryParse(digits, out var num))
            {
                part = num > 10000 ? (int)(num % 10000) : 0;
            }
        }

        // sws.c tests with strstr(reqbuf, "Authorization: Digest") etc., so Proxy-Authorization counts too
        var auth = request.Header("Authorization") ?? request.Header("Proxy-Authorization");
        if (auth != null)
        {
            if (auth.StartsWith("Digest ", StringComparison.OrdinalIgnoreCase))
            {
                part += 1000;
            }
            else if (auth.StartsWith("NTLM ", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var blob = Convert.FromBase64String(auth.Substring(5).Trim());
                    if (blob.Length > 8)
                    {
                        if (blob[8] == 1) part += 1001;
                        else if (blob[8] == 3) part += 1002;
                    }
                }
                catch (FormatException) { }
            }
            else if (auth.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                if (part >= 1000) part += 1;
            }
            else if (auth.StartsWith("Negotiate ", StringComparison.OrdinalIgnoreCase))
            {
                part += Interlocked.Increment(ref _negotiateCount);
            }
        }

        return part;
    }

    public void Dispose()
    {
        _cts.Cancel();
        try { _listener.Stop(); } catch { /* ignore */ }
        _cts.Dispose();
    }
}
