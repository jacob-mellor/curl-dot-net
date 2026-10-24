/***************************************************************************
 * WireRecording - captures HTTP/1.x responses exactly as they arrive
 *
 * curl prints response headers byte-for-byte (-i, -D, -I), counts header bytes for -w,
 * and decides between "empty reply" (52), "recv failure" (56) and "partial file" (18)
 * from what actually came over the socket. HttpClient normalises headers (it splits
 * "Server: a b" into product tokens, trims Location values, merges duplicates) and only
 * reports "the response ended prematurely", so CurlDotNet records the plaintext side of
 * every connection and parses the byte stream itself.
 *
 * The recording is bound to a transfer through an AsyncLocal that is read when a request
 * is WRITTEN to a connection (writes always run in the flow of the request that owns them);
 * every byte read afterwards on that connection is credited to that request, even when the
 * read was started by HttpClient's idle-connection read-ahead.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CurlDotNet.Core
{
    /// <summary>
    /// One response header block exactly as the server sent it (status line, header lines and the
    /// terminating blank line), plus what the body framing turned out to be.
    /// </summary>
    internal sealed class RawResponseHead
    {
        /// <summary>The header block decoded as ISO-8859-1, including the final empty line.</summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>Status code from the status line, or 0 when the line is not <c>HTTP/x.y nnn</c>.</summary>
        public int StatusCode { get; set; }

        /// <summary>Protocol version from the status line, such as <c>1.1</c> or <c>1.0</c>.</summary>
        public string HttpVersion { get; set; } = string.Empty;

        /// <summary>Reason phrase after the status code (may be empty).</summary>
        public string ReasonPhrase { get; set; } = string.Empty;

        /// <summary>Header fields in arrival order; values have surrounding whitespace removed.</summary>
        public List<KeyValuePair<string, string>> Headers { get; } = new List<KeyValuePair<string, string>>();

        /// <summary>Raw trailer lines that followed a chunked body (CRLF terminated), if any.</summary>
        public string? Trailers { get; set; }

        /// <summary>Number of body bytes that arrived for this response.</summary>
        public long BodyBytes { get; set; }

        /// <summary>Content-Length the server announced, when it announced one.</summary>
        public long? DeclaredLength { get; set; }

        /// <summary>True when the body used chunked transfer-encoding.</summary>
        public bool Chunked { get; set; }

        /// <summary>True once the body framing (length, last chunk, or no body) was satisfied.</summary>
        public bool BodyComplete { get; set; }

        /// <summary>Stopwatch timestamp of when the header block completed (orders blocks across connections).</summary>
        public long Timestamp { get; set; }

        /// <summary>True for 1xx interim responses (other than 101 Switching Protocols).</summary>
        public bool IsInformational => StatusCode >= 100 && StatusCode < 200 && StatusCode != 101;

        /// <summary>Bytes the header block occupied on the wire (before folded lines were joined).</summary>
        public int ByteLength { get; set; }

        /// <summary>Returns the first value of the named header, or null.</summary>
        public string? GetHeader(string name)
        {
            foreach (var h in Headers)
            {
                if (string.Equals(h.Key, name, StringComparison.OrdinalIgnoreCase))
                {
                    return h.Value;
                }
            }
            return null;
        }

        /// <summary>Returns every value of the named header in arrival order.</summary>
        public IEnumerable<string> GetHeaders(string name)
        {
            foreach (var h in Headers)
            {
                if (string.Equals(h.Key, name, StringComparison.OrdinalIgnoreCase))
                {
                    yield return h.Value;
                }
            }
        }

        /// <summary>Parses the status line and header fields of <paramref name="text"/>.</summary>
        public static RawResponseHead Parse(string text, long timestamp)
        {
            var head = new RawResponseHead { Text = Unfold(text), Timestamp = timestamp, ByteLength = text.Length };
            var lines = text.Split('\n');
            var statusLine = lines.Length > 0 ? lines[0].TrimEnd('\r') : string.Empty;
            ParseStatusLine(statusLine, head);

            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i].TrimEnd('\r');
                if (line.Length == 0)
                {
                    continue;
                }

                if ((line[0] == ' ' || line[0] == '\t') && head.Headers.Count > 0)
                {
                    // obs-fold continuation: append to the previous field value
                    var last = head.Headers[head.Headers.Count - 1];
                    head.Headers[head.Headers.Count - 1] = new KeyValuePair<string, string>(last.Key, (last.Value + " " + line.Trim()).Trim());
                    continue;
                }

                var colon = line.IndexOf(':');
                if (colon <= 0)
                {
                    continue;
                }

                head.Headers.Add(new KeyValuePair<string, string>(line.Substring(0, colon).Trim(), line.Substring(colon + 1).Trim()));
            }

            return head;
        }

        /// <summary>
        /// Joins obs-fold continuation lines the way curl prints them: the folded line loses its
        /// trailing blanks and each continuation is appended after a single space.
        /// </summary>
        internal static string Unfold(string text)
        {
            if (text.IndexOf("\n ", StringComparison.Ordinal) < 0 && text.IndexOf("\n\t", StringComparison.Ordinal) < 0)
            {
                return text;
            }

            var lines = text.Split('\n');
            var sb = new StringBuilder(text.Length);
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var hasCr = line.EndsWith("\r", StringComparison.Ordinal);
                var content = hasCr ? line.Substring(0, line.Length - 1) : line;
                var isLast = i == lines.Length - 1;

                // swallow following continuation lines into this one
                while (i > 0 && content.Length > 0 && i + 1 < lines.Length && lines[i + 1].Length > 0
                       && (lines[i + 1][0] == ' ' || lines[i + 1][0] == '\t') && lines[i + 1].TrimEnd('\r').Trim(' ', '\t').Length > 0)
                {
                    var next = lines[++i].TrimEnd('\r');
                    content = content.TrimEnd(' ', '\t') + " " + next.Trim(' ', '\t');
                    isLast = i == lines.Length - 1;
                }

                sb.Append(content);
                if (hasCr) sb.Append('\r');
                if (!isLast) sb.Append('\n');
            }
            return sb.ToString();
        }

        private static void ParseStatusLine(string line, RawResponseHead head)
        {
            if (!line.StartsWith("HTTP/", StringComparison.Ordinal))
            {
                return;
            }

            var space = line.IndexOf(' ');
            if (space < 0)
            {
                head.HttpVersion = line.Substring(5);
                return;
            }

            head.HttpVersion = line.Substring(5, space - 5);
            var rest = line.Substring(space + 1).TrimStart(' ');
            var codeEnd = rest.IndexOf(' ');
            var codeText = codeEnd < 0 ? rest : rest.Substring(0, codeEnd);
            if (codeText.Length == 3 && int.TryParse(codeText, NumberStyles.None, CultureInfo.InvariantCulture, out var code))
            {
                head.StatusCode = code;
            }
            head.ReasonPhrase = codeEnd < 0 ? string.Empty : rest.Substring(codeEnd + 1);
        }
    }

    /// <summary>
    /// Incremental parser for the bytes a server sends on one connection: it splits header blocks
    /// off, follows Content-Length / chunked framing to skip bodies without storing them, and keeps
    /// enough state to explain an early close.
    /// </summary>
    internal sealed class ResponseStreamParser
    {
        private enum State { Head, Body, ChunkSize, ChunkData, ChunkDataEnd, Trailers, ToEof }

        /// <summary>How many leading bytes are kept for HTTP/0.9 and --raw (1 MB unless the exchange asks for more).</summary>
        public int RawCap { get; set; } = 1024 * 1024;
        private readonly bool _isHeadRequest;
        private readonly MemoryStream _raw = new MemoryStream();
        private State _state = State.Head;
        private readonly MemoryStream _head = new MemoryStream();
        private readonly StringBuilder _line = new StringBuilder();
        private readonly StringBuilder _trailers = new StringBuilder();
        private long _remaining;
        private RawResponseHead? _current;

        /// <summary>Creates a parser; <paramref name="isHeadRequest"/> means responses carry no body.</summary>
        public ResponseStreamParser(bool isHeadRequest)
        {
            _isHeadRequest = isHeadRequest;
        }

        /// <summary>Completed header blocks in arrival order.</summary>
        public List<RawResponseHead> Heads { get; } = new List<RawResponseHead>();

        /// <summary>Total bytes fed so far.</summary>
        public long TotalBytes { get; private set; }

        /// <summary>True when the peer closed the connection (a read returned 0 bytes).</summary>
        public bool Closed { get; set; }

        /// <summary>Bytes of a header block that started but never finished.</summary>
        public long PendingHeadBytes => _head.Length;

        /// <summary>The bytes of an unfinished header block, decoded as ISO-8859-1.</summary>
        public string PendingHeadText => WireText.Latin1.GetString(_head.GetBuffer(), 0, (int)_head.Length);

        /// <summary>Trailer text received so far for an unfinished chunked body.</summary>
        public string PendingTrailerText => _state == State.Trailers ? _trailers.ToString() + _line : string.Empty;

        /// <summary>The response whose body is currently being received, if any.</summary>
        public RawResponseHead? Current => _current;

        /// <summary>The first bytes received (up to 1 MB), kept for HTTP/0.9 replies that have no headers.</summary>
        public byte[] RawPrefix => _raw.ToArray();

        /// <summary>Feeds bytes read from the connection.</summary>
        public void Feed(byte[] buffer, int offset, int count)
        {
            if (_raw.Length < RawCap)
            {
                _raw.Write(buffer, offset, (int)Math.Min(count, RawCap - _raw.Length));
            }
            TotalBytes += count;
            var i = offset;
            var end = offset + count;
            while (i < end)
            {
                switch (_state)
                {
                    case State.Head:
                    {
                        var b = buffer[i++];
                        _head.WriteByte(b);
                        if (b == (byte)'\n')
                        {
                            var len = (int)_head.Length;
                            var buf = _head.GetBuffer();
                            if (len == 1 || (len == 2 && buf[0] == (byte)'\r'))
                            {
                                _head.SetLength(0); // stray CRLF between responses
                            }
                            else if ((len >= 2 && buf[len - 2] == (byte)'\n') ||
                                     (len >= 3 && buf[len - 2] == (byte)'\r' && buf[len - 3] == (byte)'\n'))
                            {
                                CompleteHead();
                            }
                        }
                        break;
                    }
                    case State.Body:
                    {
                        var take = (int)Math.Min(_remaining, end - i);
                        i += take;
                        _remaining -= take;
                        _current!.BodyBytes += take;
                        if (_remaining == 0)
                        {
                            _current.BodyComplete = true;
                            _state = State.Head;
                        }
                        break;
                    }
                    case State.ToEof:
                    {
                        _current!.BodyBytes += end - i;
                        i = end;
                        break;
                    }
                    case State.ChunkSize:
                    {
                        var b = buffer[i++];
                        if (b == (byte)'\n')
                        {
                            var sizeText = _line.ToString().Trim();
                            _line.Clear();
                            var semi = sizeText.IndexOf(';');
                            if (semi >= 0) sizeText = sizeText.Substring(0, semi).Trim();
                            if (!long.TryParse(sizeText, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var size) || size < 0)
                            {
                                _state = State.ToEof; // garbage: HttpClient will fail the read
                            }
                            else if (size == 0)
                            {
                                _state = State.Trailers;
                                _trailers.Clear();
                            }
                            else
                            {
                                _remaining = size;
                                _state = State.ChunkData;
                            }
                        }
                        else
                        {
                            _line.Append((char)b);
                        }
                        break;
                    }
                    case State.ChunkData:
                    {
                        var take = (int)Math.Min(_remaining, end - i);
                        i += take;
                        _remaining -= take;
                        _current!.BodyBytes += take;
                        if (_remaining == 0)
                        {
                            _state = State.ChunkDataEnd;
                        }
                        break;
                    }
                    case State.ChunkDataEnd:
                    {
                        if (buffer[i++] == (byte)'\n')
                        {
                            _state = State.ChunkSize;
                        }
                        break;
                    }
                    case State.Trailers:
                    {
                        var b = buffer[i++];
                        if (b == (byte)'\n')
                        {
                            var line = _line.ToString().TrimEnd('\r');
                            _line.Clear();
                            if (line.Length == 0)
                            {
                                _current!.BodyComplete = true;
                                _current.Trailers = _trailers.Length > 0 ? _trailers.ToString() : null;
                                _state = State.Head;
                            }
                            else
                            {
                                _trailers.Append(line).Append("\r\n");
                            }
                        }
                        else
                        {
                            _line.Append((char)b);
                        }
                        break;
                    }
                }
            }
        }

        private void CompleteHead()
        {
            var text = WireText.Latin1.GetString(_head.GetBuffer(), 0, (int)_head.Length);
            _head.SetLength(0);
            if (!text.StartsWith("HTTP/", StringComparison.Ordinal) && Heads.Count > 0
                && !Heads[Heads.Count - 1].IsInformational && Heads[Heads.Count - 1].BodyComplete)
            {
                // Junk after a complete response (for example a chunk terminator some servers send
                // after a HEAD reply). HttpClient ignores it; so do we.
                return;
            }
            var head = RawResponseHead.Parse(text, Stopwatch.GetTimestamp());
            Heads.Add(head);
            _current = head;

            if (head.IsInformational)
            {
                head.BodyComplete = true;
                _state = State.Head;
                return;
            }

            if (_isHeadRequest || head.StatusCode == 204 || head.StatusCode == 304)
            {
                head.BodyComplete = true;
                _state = State.Head;
                return;
            }

            var te = string.Join(",", head.GetHeaders("Transfer-Encoding"));
            if (te.Length > 0)
            {
                var last = te.Split(',').Select(t => t.Trim()).LastOrDefault(t => t.Length > 0);
                if (string.Equals(last, "chunked", StringComparison.OrdinalIgnoreCase))
                {
                    head.Chunked = true;
                    _state = State.ChunkSize;
                    return;
                }
            }

            var cl = head.GetHeader("Content-Length");
            if (cl != null && long.TryParse(cl.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var length))
            {
                head.DeclaredLength = length;
                if (length == 0)
                {
                    head.BodyComplete = true;
                    _state = State.Head;
                }
                else
                {
                    _remaining = length;
                    _state = State.Body;
                }
                return;
            }

            _state = State.ToEof;
        }
    }

    /// <summary>Shared text helpers for wire data.</summary>
    internal static class WireText
    {
        /// <summary>ISO-8859-1: maps every byte to the char with the same value and back.</summary>
        public static readonly Encoding Latin1 = Encoding.GetEncoding("ISO-8859-1");
    }

    /// <summary>
    /// Timing and endpoint facts about one TCP connection, filled in by the connect callback.
    /// </summary>
    internal sealed class WireConnectionInfo
    {
        /// <summary>Stopwatch timestamp when the connection attempt started.</summary>
        public long StartedTimestamp { get; set; }

        /// <summary>Stopwatch timestamp when name resolution finished.</summary>
        public long ResolvedTimestamp { get; set; }

        /// <summary>Stopwatch timestamp when the TCP handshake finished.</summary>
        public long ConnectedTimestamp { get; set; }

        /// <summary>Stopwatch timestamp when TLS (if any) finished and HTTP could start.</summary>
        public long ReadyTimestamp { get; set; }

        /// <summary>True when TLS was negotiated on this connection.</summary>
        public bool Secure { get; set; }

        /// <summary>Local socket address.</summary>
        public IPEndPoint? LocalEndPoint { get; set; }

        /// <summary>Remote socket address (the proxy when one is used).</summary>
        public IPEndPoint? RemoteEndPoint { get; set; }
    }

    /// <summary>
    /// The bytes received on one connection for one exchange.
    /// </summary>
    internal sealed class WireSegment
    {
        /// <summary>Creates a segment.</summary>
        public WireSegment(bool isHeadRequest, WireConnectionInfo? connection, bool newConnection)
        {
            Parser = new ResponseStreamParser(isHeadRequest);
            Connection = connection;
            NewConnection = newConnection;
        }

        /// <summary>Parser fed with every byte received.</summary>
        public ResponseStreamParser Parser { get; }

        /// <summary>The connection this segment was received on.</summary>
        public WireConnectionInfo? Connection { get; }

        /// <summary>True when this exchange opened the connection (rather than reusing it).</summary>
        public bool NewConnection { get; }

        /// <summary>Bytes written to the connection for this exchange.</summary>
        public long BytesSent { get; set; }
    }

    /// <summary>
    /// One <c>HttpClient.SendAsync</c> call: the request(s) HttpClient wrote (more than one when it
    /// answers an auth challenge itself) and every response byte that came back.
    /// </summary>
    internal sealed class WireExchange
    {
        private static readonly AsyncLocal<WireExchange?> CurrentSlot = new AsyncLocal<WireExchange?>();
        private readonly List<WireSegment> _segments = new List<WireSegment>();
        private readonly object _lock = new object();

        /// <summary>Creates an exchange for a request.</summary>
        public WireExchange(bool isHeadRequest)
        {
            IsHeadRequest = isHeadRequest;
            StartedTimestamp = Stopwatch.GetTimestamp();
        }

        /// <summary>The exchange of the request currently being sent in this async flow.</summary>
        public static WireExchange? Current
        {
            get => CurrentSlot.Value;
            set => CurrentSlot.Value = value;
        }

        /// <summary>True for HEAD requests (responses have no body).</summary>
        public bool IsHeadRequest { get; }

        /// <summary>How many raw response bytes each segment keeps (<see cref="ResponseStreamParser.RawCap"/>).</summary>
        public int RawLimit { get; set; } = 1024 * 1024;

        /// <summary>Changes to apply to this request's head on the wire (HTTP/1.x), or null.</summary>
        public RequestRewrite? Rewrite { get; set; }

        /// <summary>Stopwatch timestamp of when the exchange began.</summary>
        public long StartedTimestamp { get; }

        /// <summary>Stopwatch timestamp of the first request byte written.</summary>
        public long FirstWriteTimestamp { get; private set; }

        /// <summary>Stopwatch timestamp of the first response byte received.</summary>
        public long FirstByteTimestamp { get; private set; }

        /// <summary>Starts a segment for a connection that just carried a write for this exchange.</summary>
        public WireSegment Attach(WireConnectionInfo? connection, bool newConnection)
        {
            var segment = new WireSegment(IsHeadRequest, connection, newConnection);
            segment.Parser.RawCap = RawLimit;
            lock (_lock)
            {
                _segments.Add(segment);
                if (FirstWriteTimestamp == 0)
                {
                    FirstWriteTimestamp = Stopwatch.GetTimestamp();
                }
            }
            return segment;
        }

        /// <summary>Records received bytes for a segment.</summary>
        public void Received(WireSegment segment, byte[] buffer, int offset, int count)
        {
            lock (_lock)
            {
                if (FirstByteTimestamp == 0)
                {
                    FirstByteTimestamp = Stopwatch.GetTimestamp();
                }
                segment.Parser.Feed(buffer, offset, count);
            }
        }

        /// <summary>Snapshot of the segments in the order they were opened.</summary>
        public List<WireSegment> Segments
        {
            get { lock (_lock) return _segments.ToList(); }
        }

        /// <summary>Every header block received, in arrival order across connections.</summary>
        public List<RawResponseHead> Heads
        {
            get
            {
                lock (_lock)
                {
                    return _segments.SelectMany(s => s.Parser.Heads).OrderBy(h => h.Timestamp).ToList();
                }
            }
        }

        /// <summary>The final (non-1xx) header block, or null when none arrived.</summary>
        public RawResponseHead? FinalHead => Heads.LastOrDefault(h => !h.IsInformational);

        /// <summary>Total bytes received from the server.</summary>
        public long BytesReceived
        {
            get { lock (_lock) return _segments.Sum(s => s.Parser.TotalBytes); }
        }

        /// <summary>Total bytes written to the server.</summary>
        public long BytesSent
        {
            get { lock (_lock) return _segments.Sum(s => s.BytesSent); }
        }

        /// <summary>True when at least one byte of a reply arrived.</summary>
        public bool ReceivedAnything => BytesReceived > 0;

        /// <summary>Number of connections this exchange opened.</summary>
        public int NewConnections
        {
            get { lock (_lock) return _segments.Count(s => s.NewConnection); }
        }

        /// <summary>The connection the last segment used.</summary>
        public WireConnectionInfo? LastConnection
        {
            get { lock (_lock) return _segments.LastOrDefault(s => s.Connection != null)?.Connection; }
        }

        /// <summary>The first segment that opened a new connection, for connect timings.</summary>
        public WireConnectionInfo? FirstNewConnection
        {
            get { lock (_lock) return _segments.FirstOrDefault(s => s.NewConnection && s.Connection != null)?.Connection; }
        }
    }

    /// <summary>
    /// All exchanges of one curl transfer (the first request, auth retries and redirect hops),
    /// used to print headers like curl and to compute <c>-w</c> variables.
    /// </summary>
    internal sealed class WireTranscript
    {
        private readonly List<WireExchange> _exchanges = new List<WireExchange>();

        /// <summary>Stopwatch timestamp of the start of the transfer.</summary>
        public long StartedTimestamp { get; } = Stopwatch.GetTimestamp();

        /// <summary>Begins recording a new exchange.</summary>
        public WireExchange Begin(bool isHeadRequest)
        {
            var exchange = new WireExchange(isHeadRequest);
            lock (_exchanges) _exchanges.Add(exchange);
            return exchange;
        }

        /// <summary>Exchanges so far.</summary>
        public List<WireExchange> Exchanges
        {
            get { lock (_exchanges) return _exchanges.ToList(); }
        }

        /// <summary>True when raw bytes were captured (HTTP/1.x on a recording transport).</summary>
        public bool HasRecording => Exchanges.Any(e => e.Segments.Count > 0);

        /// <summary>Every header block of every exchange in order.</summary>
        public List<RawResponseHead> AllHeads => Exchanges.SelectMany(e => e.Heads).ToList();

        /// <summary>Number of new connections opened during the transfer.</summary>
        public int NewConnections => Exchanges.Sum(e => e.NewConnections);

        /// <summary>Total header bytes received.</summary>
        public long HeaderBytes => AllHeads.Sum(h => (long)h.ByteLength);

        /// <summary>Converts a stopwatch timestamp to seconds since the transfer started.</summary>
        public double SecondsSinceStart(long timestamp)
        {
            if (timestamp == 0)
            {
                return 0;
            }
            return Math.Max(0, (timestamp - StartedTimestamp) / (double)Stopwatch.Frequency);
        }
    }

#if !NETSTANDARD2_0 && !NET472 && !NET48
    /// <summary>
    /// A NetworkStream that remembers the timings and endpoints of its connection.
    /// </summary>
    internal sealed class InfoNetworkStream : System.Net.Sockets.NetworkStream
    {
        /// <summary>Wraps a connected socket (the stream owns it).</summary>
        public InfoNetworkStream(System.Net.Sockets.Socket socket, WireConnectionInfo info) : base(socket, ownsSocket: true)
        {
            Info = info;
        }

        /// <summary>Connection facts.</summary>
        public WireConnectionInfo Info { get; }
    }

    /// <summary>
    /// Wraps the plaintext side of a connection and feeds what it reads into the
    /// <see cref="WireExchange"/> whose request was last written to it.
    /// </summary>
    internal sealed class RecordingStream : Stream
    {
        private readonly Stream _inner;
        private readonly WireConnectionInfo? _info;
        private WireExchange? _bound;
        private WireSegment? _segment;
        private bool _used;
        private MemoryStream? _head;
        private RequestRewrite? _rewrite;
        private ChunkSizeLowercaser? _chunks;

        /// <summary>Wraps <paramref name="inner"/>.</summary>
        public RecordingStream(Stream inner, WireConnectionInfo? info)
        {
            _inner = inner;
            _info = info;
        }

        private void OnWrite(int count)
        {
            var current = WireExchange.Current;
            if (!ReferenceEquals(current, _bound))
            {
                _bound = current;
                _segment = current?.Attach(_info, newConnection: !_used);
                _rewrite = current?.Rewrite;
                _head = _rewrite != null ? new MemoryStream() : null;
                _chunks = null;
            }
            _used = true;
            if (_segment != null)
            {
                _segment.BytesSent += count;
            }
        }

        private void OnRead(byte[] buffer, int offset, int requested, int read)
        {
            var exchange = _bound;
            var segment = _segment;
            if (exchange == null || segment == null)
            {
                return;
            }

            if (read > 0)
            {
                exchange.Received(segment, buffer, offset, read);
            }
            else if (requested > 0)
            {
                segment.Parser.Closed = true;
            }
        }

        private void OnRead(ReadOnlySpan<byte> data, int requested)
        {
            var exchange = _bound;
            var segment = _segment;
            if (exchange == null || segment == null)
            {
                return;
            }

            if (data.Length > 0)
            {
                var copy = data.ToArray();
                exchange.Received(segment, copy, 0, copy.Length);
            }
            else if (requested > 0)
            {
                segment.Parser.Closed = true;
            }
        }

        /// <inheritdoc />
        public override bool CanRead => _inner.CanRead;

        /// <inheritdoc />
        public override bool CanSeek => false;

        private static readonly string NotSeekable = CurlDotNet.Exceptions.CurlException.Compose(
            "The recorded network stream of a CurlDotNet connection is forward-only: Length, Position, Seek and SetLength are not supported.",
            "Read the stream sequentially (CanSeek is false); copy it into a MemoryStream first if you need random access.",
            CurlDotNet.Exceptions.DocLinks.HowItWorks);

        /// <inheritdoc />
        public override bool CanWrite => _inner.CanWrite;

        /// <inheritdoc />
        public override long Length => throw new NotSupportedException(NotSeekable);

        /// <inheritdoc />
        public override long Position
        {
            get => throw new NotSupportedException(NotSeekable);
            set => throw new NotSupportedException(NotSeekable);
        }

        /// <inheritdoc />
        public override void Flush() => _inner.Flush();

        /// <inheritdoc />
        public override Task FlushAsync(CancellationToken cancellationToken) => _inner.FlushAsync(cancellationToken);

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            var n = _inner.Read(buffer, offset, count);
            OnRead(buffer, offset, count, n);
            return n;
        }

        /// <inheritdoc />
        public override int Read(Span<byte> buffer)
        {
            var n = _inner.Read(buffer);
            OnRead(buffer.Slice(0, n), buffer.Length);
            return n;
        }

        /// <inheritdoc />
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var n = await _inner.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
            OnRead(buffer, offset, count, n);
            return n;
        }

        /// <inheritdoc />
        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            var n = await _inner.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
            if (System.Runtime.InteropServices.MemoryMarshal.TryGetArray<byte>(buffer, out var segment) && segment.Array != null)
            {
                OnRead(segment.Array, segment.Offset, buffer.Length, n); // no copy for the usual array-backed buffer
            }
            else
            {
                OnRead(buffer.Span.Slice(0, n), buffer.Length);
            }
            return n;
        }

        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException(NotSeekable);

        /// <inheritdoc />
        public override void SetLength(long value) => throw new NotSupportedException(NotSeekable);

        /// <summary>
        /// While a request head that needs a <see cref="RequestRewrite"/> is being written, collects it;
        /// returns null to pass <paramref name="data"/> through unchanged, or the bytes to write now
        /// (empty until the head is complete, then the rewritten head plus whatever followed it).
        /// </summary>
        private byte[]? Intercept(ReadOnlySpan<byte> data)
        {
            if (_chunks != null)
            {
                var copy = data.ToArray();
                _chunks.Transform(copy, 0, copy.Length);
                return copy;
            }

            if (_head == null || _rewrite == null)
            {
                return null;
            }

            _head.Write(data.ToArray(), 0, data.Length);
            var all = _head.GetBuffer();
            var length = (int)_head.Length;
            var end = -1;
            for (var i = 3; i < length; i++)
            {
                if (all[i] == '\n' && all[i - 1] == '\r' && all[i - 2] == '\n' && all[i - 3] == '\r')
                {
                    end = i + 1;
                    break;
                }
            }
            if (end < 0)
            {
                return Array.Empty<byte>();
            }

            var head = _rewrite.Apply(all, end);
            var output = new byte[head.Length + (length - end)];
            Buffer.BlockCopy(head, 0, output, 0, head.Length);
            Buffer.BlockCopy(all, end, output, head.Length, length - end);
            if (_rewrite.LowercaseChunkSizes)
            {
                _chunks = new ChunkSizeLowercaser();
                _chunks.Transform(output, head.Length, length - end);
            }
            _head = null;
            _rewrite = null;
            return output;
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            OnWrite(count);
            var data = Intercept(new ReadOnlySpan<byte>(buffer, offset, count));
            if (data == null)
            {
                _inner.Write(buffer, offset, count);
            }
            else if (data.Length > 0)
            {
                _inner.Write(data, 0, data.Length);
            }
        }

        /// <inheritdoc />
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            OnWrite(buffer.Length);
            var data = Intercept(buffer);
            if (data == null)
            {
                _inner.Write(buffer);
            }
            else if (data.Length > 0)
            {
                _inner.Write(data, 0, data.Length);
            }
        }

        /// <inheritdoc />
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            OnWrite(count);
            var data = Intercept(new ReadOnlySpan<byte>(buffer, offset, count));
            if (data == null)
            {
                return _inner.WriteAsync(buffer, offset, count, cancellationToken);
            }
            return data.Length > 0 ? _inner.WriteAsync(data, 0, data.Length, cancellationToken) : Task.CompletedTask;
        }

        /// <inheritdoc />
        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            OnWrite(buffer.Length);
            var data = Intercept(buffer.Span);
            if (data == null)
            {
                return _inner.WriteAsync(buffer, cancellationToken);
            }
            return data.Length > 0 ? _inner.WriteAsync(data, cancellationToken) : default;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _inner.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <inheritdoc />
        public override async ValueTask DisposeAsync()
        {
            await _inner.DisposeAsync().ConfigureAwait(false);
            await base.DisposeAsync().ConfigureAwait(false);
        }
    }
#endif
}
