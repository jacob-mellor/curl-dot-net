using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// A tiny HTTP/1.x server on a TcpListener: serves canned replies (one per connection, in order)
    /// byte-for-byte and records every request, so tests can check exactly what goes over the wire.
    /// </summary>
    internal sealed class RawReplyServer : IDisposable
    {
        private static readonly Encoding Latin1 = Encoding.GetEncoding("ISO-8859-1");
        private readonly TcpListener _listener = new TcpListener(IPAddress.Loopback, 0);
        private readonly Queue<string> _replies;
        private readonly List<string> _requests = new List<string>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public RawReplyServer(params string[] replies)
        {
            _replies = new Queue<string>(replies);
            _listener.Start();
            Port = ((IPEndPoint)_listener.LocalEndpoint).Port;
            _ = Task.Run(AcceptLoopAsync);
        }

        public int Port { get; }

        /// <summary>Keep the connection open for a while after the reply (like a keep-alive server).</summary>
        public bool KeepOpen { get; set; }

        public string Url(string path = "/") => $"http://127.0.0.1:{Port}{path}";

        public IReadOnlyList<string> Requests
        {
            get { lock (_requests) return _requests.ToList(); }
        }

        private async Task AcceptLoopAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                TcpClient client;
                try
                {
                    client = await _listener.AcceptTcpClientAsync();
                }
                catch (Exception)
                {
                    return;
                }
                _ = Task.Run(() => ServeAsync(client));
            }
        }

        private async Task ServeAsync(TcpClient client)
        {
            using (client)
            {
                try
                {
                    var stream = client.GetStream();
                    var buffer = new MemoryStream();
                    var chunk = new byte[8192];
                    while (true)
                    {
                        var text = Latin1.GetString(buffer.ToArray());
                        var end = text.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                        if (end >= 0)
                        {
                            var head = text.Substring(0, end);
                            var length = 0;
                            foreach (var line in head.Split(new[] { "\r\n" }, StringSplitOptions.None))
                            {
                                if (line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
                                {
                                    int.TryParse(line.Substring(15).Trim(), out length);
                                }
                            }
                            if (text.Length >= end + 4 + length)
                            {
                                break;
                            }
                        }
                        var n = await stream.ReadAsync(chunk, 0, chunk.Length);
                        if (n <= 0) return;
                        buffer.Write(chunk, 0, n);
                    }

                    string reply;
                    lock (_requests)
                    {
                        _requests.Add(Latin1.GetString(buffer.ToArray()));
                        reply = _replies.Count > 0 ? _replies.Dequeue() : "HTTP/1.1 404 Not Found\r\nContent-Length: 0\r\n\r\n";
                    }

                    var bytes = Latin1.GetBytes(reply);
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                    await stream.FlushAsync();
                    if (KeepOpen)
                    {
                        await Task.Delay(2000);
                    }
                    client.Client.Shutdown(SocketShutdown.Send);
                    await Task.Delay(50);
                }
                catch (IOException)
                {
                }
                catch (SocketException)
                {
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _listener.Stop();
        }
    }

    /// <summary>
    /// Transport-side curl parity: proxies, redirects, -f, output files, -w, cookies, multipart and
    /// exit codes, each checked against what curl 8 does.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class ParityTransportTests
    {
        private readonly ITestOutputHelper _output;

        public ParityTransportTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Debug_NulInTrailer()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nServer: test\r\nTransfer-Encoding: chunked\r\nTrailer: chunky-trailer\r\n\r\n6\r\n-foo-\r\n0\r\nchunky-trailer: he\0llo\r\n\r\n") { KeepOpen = true };
            try
            {
                await Curl.ExecuteAsync($"curl {server.Url("/x")}");
            }
            catch (Exception ex)
            {
                for (var e = ex; e != null; e = e.InnerException) _output.WriteLine(e.GetType().FullName + ": " + e.Message);
            }
        }
    }
}
