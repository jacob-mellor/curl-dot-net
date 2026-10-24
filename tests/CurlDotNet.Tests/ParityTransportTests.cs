using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// A tiny HTTP/1.x server on a TcpListener: answers request N with reply N (the last reply is
    /// repeated), byte-for-byte, and records every request, so tests can check exactly what goes over
    /// the wire. With <see cref="KeepAlive"/> several requests share one connection (needed for
    /// CONNECT tunnels); otherwise the server closes after each reply.
    /// </summary>
    internal sealed class RawReplyServer : IDisposable
    {
        private static readonly Encoding Latin1 = Encoding.GetEncoding("ISO-8859-1");
        private readonly TcpListener _listener = new TcpListener(IPAddress.Loopback, 0);
        private readonly string[] _replies;
        private readonly List<string> _requests = new List<string>();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        private int _connections;

        public RawReplyServer(params string[] replies)
        {
            _replies = replies.Length == 0 ? new[] { "HTTP/1.1 200 OK\r\nContent-Length: 2\r\n\r\nok" } : replies;
            _listener.Start();
            Port = ((IPEndPoint)_listener.LocalEndpoint).Port;
            _ = Task.Run(AcceptLoopAsync);
        }

        public int Port { get; }

        /// <summary>Serve further requests on the same connection instead of closing after a reply.</summary>
        public bool KeepAlive { get; set; }

        /// <summary>After writing a reply, wait this long before closing (to test timeouts).</summary>
        public int StallMilliseconds { get; set; }

        public int Connections => _connections;

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
                Interlocked.Increment(ref _connections);
                _ = Task.Run(() => ServeAsync(client));
            }
        }

        private async Task<string?> ReadRequestAsync(NetworkStream stream, MemoryStream pending)
        {
            var chunk = new byte[64 * 1024];
            while (true)
            {
                var text = Latin1.GetString(pending.ToArray());
                var end = text.IndexOf("\r\n\r\n", StringComparison.Ordinal);
                if (end >= 0)
                {
                    var head = text.Substring(0, end);
                    long length = 0;
                    foreach (var line in head.Split(new[] { "\r\n" }, StringSplitOptions.None))
                    {
                        if (line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
                        {
                            long.TryParse(line.Substring(15).Trim(), out length);
                        }
                    }
                    var expectContinue = head.IndexOf("Expect: 100-continue", StringComparison.OrdinalIgnoreCase) >= 0;
                    if (expectContinue && text.Length == end + 4 && length > 0)
                    {
                        var cont = Latin1.GetBytes("HTTP/1.1 100 Continue\r\n\r\n");
                        await stream.WriteAsync(cont, 0, cont.Length);
                    }
                    if (text.Length >= end + 4 + length)
                    {
                        var request = text.Substring(0, (int)(end + 4 + length));
                        var rest = pending.ToArray().Skip(request.Length).ToArray();
                        pending.SetLength(0);
                        pending.Write(rest, 0, rest.Length);
                        return request;
                    }
                }

                var n = await stream.ReadAsync(chunk, 0, chunk.Length);
                if (n <= 0)
                {
                    return null;
                }
                pending.Write(chunk, 0, n);
            }
        }

        private async Task ServeAsync(TcpClient client)
        {
            using (client)
            {
                try
                {
                    var stream = client.GetStream();
                    var pending = new MemoryStream();
                    while (true)
                    {
                        var request = await ReadRequestAsync(stream, pending);
                        if (request == null)
                        {
                            return;
                        }

                        string reply;
                        lock (_requests)
                        {
                            _requests.Add(request);
                            reply = _replies[Math.Min(_requests.Count - 1, _replies.Length - 1)];
                        }

                        var bytes = Latin1.GetBytes(reply);
                        await stream.WriteAsync(bytes, 0, bytes.Length);
                        await stream.FlushAsync();
                        if (StallMilliseconds > 0)
                        {
                            await Task.Delay(StallMilliseconds);
                        }
                        if (!KeepAlive)
                        {
                            client.Client.Shutdown(SocketShutdown.Send);
                            await Task.Delay(50);
                            return;
                        }
                    }
                }
                catch (IOException)
                {
                }
                catch (SocketException)
                {
                }
                catch (ObjectDisposedException)
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
    /// Transport-side curl parity: proxies, redirects, -f, output files, -w, cookies, multipart, timeouts
    /// and exit codes, each checked against what curl 8 sends and prints.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class ParityTransportTests : IDisposable
    {
        private const string Ok = "HTTP/1.1 200 OK\r\nContent-Length: 5\r\nContent-Type: text/plain\r\n\r\nhello";
        private readonly string _dir;

        public ParityTransportTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "curldotnet-parity-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(_dir, recursive: true);
            }
            catch (IOException)
            {
            }
        }

        private string P(string name) => Path.Combine(_dir, name);

        private static string Q(string path) => CommandParser.ShellQuote(path);

        private static int ClosedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private static string Line(string request, string header)
        {
            return request.Split(new[] { "\r\n" }, StringSplitOptions.None)
                .FirstOrDefault(l => l.StartsWith(header + ":", StringComparison.OrdinalIgnoreCase));
        }

        private static string FirstLine(string request) => request.Substring(0, request.IndexOf("\r\n", StringComparison.Ordinal));

        // ------------------------------------------------------------------ proxy

        [Fact]
        public async Task Proxy_HttpUrl_SendsAbsoluteUrlAndProxyConnection()
        {
            using var proxy = new RawReplyServer(Ok);

            var result = await Curl.ExecuteAsync($"curl -x 127.0.0.1:{proxy.Port} http://example.invalid/path?q=1");

            result.Body.Should().Be("hello");
            var request = proxy.Requests.Single();
            FirstLine(request).Should().Be("GET http://example.invalid/path?q=1 HTTP/1.1");
            Line(request, "Host").Should().Be("Host: example.invalid");
            Line(request, "Proxy-Connection").Should().Be("Proxy-Connection: Keep-Alive");
            Line(request, "Proxy-Authorization").Should().BeNull();
        }

        [Fact]
        public async Task Proxy_UserFlag_SendsBasicProxyAuthorizationPreemptively()
        {
            using var proxy = new RawReplyServer(Ok);

            await Curl.ExecuteAsync($"curl -x http://127.0.0.1:{proxy.Port} -U fake:user http://example.invalid/");

            Line(proxy.Requests.Single(), "Proxy-Authorization").Should().Be("Proxy-Authorization: Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("fake:user")));
        }

        [Fact]
        public async Task Proxy_UserInfoInProxyUrl_SendsBasicProxyAuthorization()
        {
            using var proxy = new RawReplyServer(Ok);

            await Curl.ExecuteAsync($"curl -x http://alice:s3cret@127.0.0.1:{proxy.Port} http://example.invalid/");

            Line(proxy.Requests.Single(), "Proxy-Authorization").Should().Be("Proxy-Authorization: Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("alice:s3cret")));
        }

        [Fact]
        public async Task Proxy_NoProxyList_ConnectsDirectly()
        {
            using var origin = new RawReplyServer(Ok);
            var deadProxy = ClosedPort();

            var result = await Curl.ExecuteAsync($"curl -x 127.0.0.1:{deadProxy} --noproxy 127.0.0.1,example.com {origin.Url("/direct")}");

            result.Body.Should().Be("hello");
            FirstLine(origin.Requests.Single()).Should().Be("GET /direct HTTP/1.1");
            Line(origin.Requests.Single(), "Proxy-Connection").Should().BeNull();
        }

        [Fact]
        public async Task Proxy_Unreachable_ErrorNamesTheProxy()
        {
            var deadProxy = ClosedPort();

            Func<Task> act = () => Curl.ExecuteAsync($"curl -x 127.0.0.1:{deadProxy} http://example.invalid/");

            var thrown = await act.Should().ThrowAsync<CurlCouldntConnectException>();
            thrown.Which.CurlErrorCode.Should().Be(7);
            thrown.Which.ViaProxy.Should().BeTrue();
            thrown.Which.Port.Should().Be(deadProxy);
            thrown.Which.Message.Should().Contain("proxy").And.Contain("dns-errors.md");
        }

        [Fact]
        public async Task Proxy_BadProxyName_IsCouldntResolveProxy()
        {
            Func<Task> act = () => Curl.ExecuteAsync("curl -x \"http://bad host\" http://example.invalid/");

            (await act.Should().ThrowAsync<CurlCouldntResolveProxyException>()).Which.CurlErrorCode.Should().Be(5);
        }

        [Fact]
        public void NoProxyList_MatchesCurlRules()
        {
            var list = new NoProxyList("example.com, .internal,10.0.0.0/8,::1");

            list.Matches("example.com").Should().BeTrue();
            list.Matches("www.example.com").Should().BeTrue();
            list.Matches("badexample.com").Should().BeFalse();
            list.Matches("api.internal").Should().BeTrue();
            list.Matches("10.20.30.40").Should().BeTrue();
            list.Matches("11.0.0.1").Should().BeFalse();
            list.Matches("[::1]").Should().BeTrue();
            new NoProxyList("*").Matches("anything").Should().BeTrue();
            new NoProxyList(null).Matches("anything").Should().BeFalse();
        }

        [Fact]
        public void ProxyUrl_DefaultsToPort1080LikeCurl()
        {
            CurlProxy.ParseProxyUrl("proxy.example", null).Port.Should().Be(1080);
            CurlProxy.ParseProxyUrl("proxy.example:3128", null).Port.Should().Be(3128);
            CurlProxy.ParseProxyUrl("https://proxy.example", null).Port.Should().Be(443);
        }

#if NET5_0_OR_GREATER
        [Fact]
        public async Task ProxyTunnel_HttpUrl_SendsConnectFirst()
        {
            using var proxy = new RawReplyServer("HTTP/1.1 200 Connection established\r\n\r\n", Ok) { KeepAlive = true };

            var result = await Curl.ExecuteAsync($"curl -p -x 127.0.0.1:{proxy.Port} -U me:pw http://tunnel.invalid:8080/inside");

            result.Body.Should().Be("hello");
            proxy.Requests.Should().HaveCount(2);
            FirstLine(proxy.Requests[0]).Should().Be("CONNECT tunnel.invalid:8080 HTTP/1.1");
            Line(proxy.Requests[0], "Proxy-Authorization").Should().StartWith("Proxy-Authorization: Basic ");
            FirstLine(proxy.Requests[1]).Should().Be("GET /inside HTTP/1.1");
        }

        [Fact]
        public async Task ProxyTunnel_Proxy10_UsesHttp10Connect()
        {
            using var proxy = new RawReplyServer("HTTP/1.0 200 Connection established\r\n\r\n", Ok) { KeepAlive = true };

            await Curl.ExecuteAsync($"curl -p --proxy1.0 127.0.0.1:{proxy.Port} http://tunnel.invalid/");

            FirstLine(proxy.Requests[0]).Should().Be("CONNECT tunnel.invalid:80 HTTP/1.0");
        }

        [Fact]
        public async Task ProxyTunnel_Refused_IsConnectFailureNamingTheProxy()
        {
            using var proxy = new RawReplyServer("HTTP/1.1 407 Proxy Authentication Required\r\nContent-Length: 0\r\n\r\n");

            Func<Task> act = () => Curl.ExecuteAsync($"curl -p -x 127.0.0.1:{proxy.Port} http://tunnel.invalid/");

            var thrown = await act.Should().ThrowAsync<CurlCouldntConnectException>();
            thrown.Which.ViaProxy.Should().BeTrue();
            thrown.Which.Message.Should().Contain("407");
        }
#endif

        // ------------------------------------------------------------------ redirects

        [Fact]
        public async Task Redirect_301_TurnsPostIntoGetWithoutBody()
        {
            using var server = new RawReplyServer("HTTP/1.1 301 Moved\r\nLocation: /new\r\nContent-Length: 0\r\n\r\n", Ok);

            await Curl.ExecuteAsync($"curl -L -d secret=1 {server.Url("/old")}");

            FirstLine(server.Requests[0]).Should().Be("POST /old HTTP/1.1");
            FirstLine(server.Requests[1]).Should().Be("GET /new HTTP/1.1");
            server.Requests[1].Should().NotContain("secret=1");
            Line(server.Requests[1], "Content-Length").Should().BeNull();
        }

        [Fact]
        public async Task Redirect_Post301_KeepsPostAndBody()
        {
            using var server = new RawReplyServer("HTTP/1.1 301 Moved\r\nLocation: /new\r\nContent-Length: 0\r\n\r\n", Ok);

            await Curl.ExecuteAsync($"curl -L --post301 -d secret=1 {server.Url("/old")}");

            FirstLine(server.Requests[1]).Should().Be("POST /new HTTP/1.1");
            server.Requests[1].Should().EndWith("secret=1");
        }

        [Fact]
        public async Task Redirect_307_KeepsMethodAndBody()
        {
            using var server = new RawReplyServer("HTTP/1.1 307 Temporary\r\nLocation: /new\r\nContent-Length: 0\r\n\r\n", Ok);

            await Curl.ExecuteAsync($"curl -L -d a=b {server.Url("/old")}");

            FirstLine(server.Requests[1]).Should().Be("POST /new HTTP/1.1");
            server.Requests[1].Should().EndWith("a=b");
        }

        [Fact]
        public async Task Redirect_303_TurnsAnyMethodIntoGet()
        {
            using var server = new RawReplyServer("HTTP/1.1 303 See Other\r\nLocation: /new\r\nContent-Length: 0\r\n\r\n", Ok);

            await Curl.ExecuteAsync($"curl -L -X DELETE {server.Url("/old")} --follow");

            FirstLine(server.Requests[0]).Should().Be("DELETE /old HTTP/1.1");
            FirstLine(server.Requests[1]).Should().Be("GET /new HTTP/1.1");
        }

        [Fact]
        public async Task Redirect_CustomMethodWithPlainL_IsKeptForEveryHop()
        {
            using var server = new RawReplyServer("HTTP/1.1 302 Found\r\nLocation: /new\r\nContent-Length: 0\r\n\r\n", Ok);

            await Curl.ExecuteAsync($"curl -L -X IGLOO {server.Url("/old")}");

            FirstLine(server.Requests[1]).Should().Be("IGLOO /new HTTP/1.1");
        }

        [Fact]
        public async Task Redirect_ToOtherOrigin_DropsAuthorization()
        {
            using var target = new RawReplyServer(Ok);
            using var origin = new RawReplyServer($"HTTP/1.1 302 Found\r\nLocation: {target.Url("/landing")}\r\nContent-Length: 0\r\n\r\n");

            await Curl.ExecuteAsync($"curl -L -u user:pass -H \"Authorization: Bearer abc\" {origin.Url("/start")}");

            Line(origin.Requests.Single(), "Authorization").Should().NotBeNull();
            Line(target.Requests.Single(), "Authorization").Should().BeNull();
        }

        [Fact]
        public async Task Redirect_LocationTrusted_KeepsCredentialsAndImpliesFollow()
        {
            using var target = new RawReplyServer(Ok);
            using var origin = new RawReplyServer($"HTTP/1.1 302 Found\r\nLocation: {target.Url("/landing")}\r\nContent-Length: 0\r\n\r\n");

            await Curl.ExecuteAsync($"curl --location-trusted -u user:pass {origin.Url("/start")}");

            Line(target.Requests.Single(), "Authorization").Should().Be("Authorization: Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("user:pass")));
        }

        [Fact]
        public async Task Redirect_MaxRedirs_AllowsExactlyN()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 302 Found\r\nLocation: /2\r\nContent-Length: 0\r\n\r\n",
                "HTTP/1.1 302 Found\r\nLocation: /3\r\nContent-Length: 0\r\n\r\n",
                Ok);

            var result = await Curl.ExecuteAsync($"curl -L --max-redirs 2 {server.Url("/1")}");
            result.Body.Should().Be("hello");

            using var looping = new RawReplyServer("HTTP/1.1 302 Found\r\nLocation: /again\r\nContent-Length: 0\r\n\r\n");
            Func<Task> act = () => Curl.ExecuteAsync($"curl -L --max-redirs 2 {looping.Url("/1")}");
            var thrown = await act.Should().ThrowAsync<CurlTooManyRedirectsException>();
            thrown.Which.CurlErrorCode.Should().Be(47);
            thrown.Which.Message.Should().Contain("Maximum (2) redirects followed");
            looping.Requests.Should().HaveCount(3);
        }

        [Fact]
        public async Task Redirect_MaxRedirsMinusOne_IsUnlimited()
        {
            var replies = Enumerable.Range(0, 60).Select(i => $"HTTP/1.1 302 Found\r\nLocation: /{i + 1}\r\nContent-Length: 0\r\n\r\n").Concat(new[] { Ok }).ToArray();
            using var server = new RawReplyServer(replies);

            var result = await Curl.ExecuteAsync($"curl -L --max-redirs=-1 {server.Url("/0")}");

            result.Body.Should().Be("hello");
            server.Requests.Should().HaveCount(61);
        }

        [Fact]
        public async Task Redirect_PaddedRelativeLocation_IsTrimmedAndEncoded()
        {
            using var server = new RawReplyServer("HTTP/1.1 302 Found\r\nLocation:   ../m o o.html?a b   \r\nContent-Length: 0\r\n\r\n", Ok);

            await Curl.ExecuteAsync($"curl -L {server.Url("/dir/sub/page")}");

            FirstLine(server.Requests[1]).Should().Be("GET /dir/m%20o%20o.html?a+b HTTP/1.1");
        }

        [Fact]
        public async Task Redirect_IncludeHeaders_PrintsEveryHopVerbatim()
        {
            using var server = new RawReplyServer("HTTP/1.1 302 Found\r\nServer: a b c\r\nLocation: /next\r\nContent-Length: 0\r\n\r\n", Ok);

            var result = await Curl.ExecuteAsync($"curl -i -L {server.Url("/")}");

#if NET5_0_OR_GREATER
            result.Body.Should().Be("HTTP/1.1 302 Found\r\nServer: a b c\r\nLocation: /next\r\nContent-Length: 0\r\n\r\n" + Ok);
#else
            result.Body.Should().Contain("302").And.EndWith("hello");
#endif
        }

        [Fact]
        public async Task Redirect_WithoutL_ReportsRedirectUrlInWriteOut()
        {
            using var server = new RawReplyServer("HTTP/1.1 301 Moved\r\nLocation: data/x.txt?y=1\r\nContent-Length: 0\r\n\r\n");

            var result = await Curl.ExecuteAsync($"curl -w \"%{{redirect_url}}|%{{num_redirects}}\" {server.Url("/we/want/it")}");

            result.Body.Should().Be($"{server.Url("/we/want/data/x.txt?y=1")}|0");
        }

        [Fact]
        public async Task Redirect_ToUnsupportedScheme_IsExitCode1()
        {
            using var server = new RawReplyServer("HTTP/1.1 302 Found\r\nLocation: gopher://example.invalid/\r\nContent-Length: 0\r\n\r\n");

            Func<Task> act = () => Curl.ExecuteAsync($"curl -L {server.Url("/")}");

            (await act.Should().ThrowAsync<CurlUnsupportedProtocolException>()).Which.CurlErrorCode.Should().Be(1);
        }

        // ------------------------------------------------------------------ -f / --fail-with-body

        [Fact]
        public async Task Fail_On404_ThrowsExit22AndWritesNoBody()
        {
            using var server = new RawReplyServer("HTTP/1.1 404 Not Found\r\nContent-Length: 4\r\n\r\ngone");
            var output = P("out.txt");

            Func<Task> act = () => Curl.ExecuteAsync($"curl -f -o {Q(output)} {server.Url("/missing")}");

            var thrown = await act.Should().ThrowAsync<CurlHttpReturnedErrorException>();
            thrown.Which.CurlErrorCode.Should().Be(22);
            thrown.Which.StatusCode.Should().Be(404);
            thrown.Which.Message.Should().Contain("The requested URL returned error: 404");
            File.Exists(output).Should().BeFalse();
        }

        [Fact]
        public async Task FailWithBody_On500_WritesBodyThenThrows()
        {
            using var server = new RawReplyServer("HTTP/1.1 500 Oops\r\nContent-Length: 6\r\nContent-Type: text/plain\r\n\r\nbroken");
            var output = P("out.txt");

            Func<Task> act = () => Curl.ExecuteAsync($"curl --fail-with-body --fail-early -o {Q(output)} {server.Url("/")}");

            var thrown = await act.Should().ThrowAsync<CurlHttpReturnedErrorException>();
            thrown.Which.CurlErrorCode.Should().Be(22);
            thrown.Which.ResponseBody.Should().Be("broken");
            File.ReadAllText(output).Should().Be("broken");
        }

        [Fact]
        public async Task Fail_Below400_Succeeds()
        {
            using var server = new RawReplyServer(Ok);

            var result = await Curl.ExecuteAsync($"curl -f {server.Url("/")}");

            result.Body.Should().Be("hello");
        }

        // ------------------------------------------------------------------ output files

        [Fact]
        public async Task DumpHeader_WritesHeaderBlockToFile()
        {
            using var server = new RawReplyServer(Ok);
            var heads = P("heads.txt");

            var result = await Curl.ExecuteAsync($"curl -D {Q(heads)} {server.Url("/")}");

            result.Body.Should().Be("hello");
            File.ReadAllText(heads).Should().StartWith("HTTP/1.1 200 OK\r\n").And.EndWith("\r\n\r\n");
        }

        [Fact]
        public async Task DumpHeader_Dash_WritesHeadersToBodyBeforeContent()
        {
            using var server = new RawReplyServer(Ok);

            var result = await Curl.ExecuteAsync($"curl -D - {server.Url("/")}");

            result.Body.Should().StartWith("HTTP/1.1 200 OK\r\n").And.EndWith("\r\n\r\nhello");
        }

        [Fact]
        public async Task DumpHeader_MissingDirectory_IsWriteError23()
        {
            using var server = new RawReplyServer(Ok);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -D {Q(P("no/such/dir/h.txt"))} {server.Url("/")}");

            var thrown = await act.Should().ThrowAsync<CurlWriteErrorException>();
            thrown.Which.CurlErrorCode.Should().Be(23);
            thrown.Which.Message.Should().Contain("--create-dirs").And.Contain("error-reference.md");
        }

        [Fact]
        public async Task Output_MissingDirectory_IsWriteError23NotDirectoryNotFound()
        {
            using var server = new RawReplyServer(Ok);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -o {Q(P("missing/out.txt"))} {server.Url("/")}");

            (await act.Should().ThrowAsync<CurlWriteErrorException>()).Which.CurlErrorCode.Should().Be(23);
        }

        [Fact]
        public async Task Output_CreateDirs_CreatesTheDirectory()
        {
            using var server = new RawReplyServer(Ok);
            var output = P("made/for/you/out.txt");

            await Curl.ExecuteAsync($"curl --create-dirs -o {Q(output)} {server.Url("/")}");

            File.ReadAllText(output).Should().Be("hello");
        }

        [Fact]
        public async Task RemoteName_UsesLastPathSegmentInOutputDir()
        {
            using var server = new RawReplyServer(Ok);

            var result = await Curl.ExecuteAsync($"curl -O --output-dir {Q(_dir)} {server.Url("/files/report.csv?x=1")}");

            result.OutputFiles.Should().ContainSingle().Which.Should().Be(Path.Combine(_dir, "report.csv"));
            File.ReadAllText(P("report.csv")).Should().Be("hello");
        }

        [Fact]
        public async Task RemoteName_UrlWithoutFileName_UsesCurlResponse()
        {
            using var server = new RawReplyServer(Ok);

            await Curl.ExecuteAsync($"curl -O --output-dir {Q(_dir)} {server.Url("/")}");

            File.ReadAllText(P("curl_response")).Should().Be("hello");
        }

        [Fact]
        public async Task RemoteHeaderName_UsesContentDispositionWithoutDirectories()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Disposition: attachment; filename=\"../../evil/name.txt\"\r\nContent-Length: 5\r\n\r\nhello");

            await Curl.ExecuteAsync($"curl -J -O --output-dir {Q(_dir)} {server.Url("/download")}");

            File.ReadAllText(P("name.txt")).Should().Be("hello");
        }

        [Fact]
        public async Task RemoteHeaderName_ExistingFile_IsRefusedWithExit23()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Disposition: attachment; filename=keep.txt\r\nContent-Length: 5\r\n\r\nhello");
            File.WriteAllText(P("keep.txt"), "original");

            Func<Task> act = () => Curl.ExecuteAsync($"curl -J -O --output-dir {Q(_dir)} {server.Url("/download")}");

            (await act.Should().ThrowAsync<CurlWriteErrorException>()).Which.CurlErrorCode.Should().Be(23);
            File.ReadAllText(P("keep.txt")).Should().Be("original");
        }

        [Fact]
        public void ContentDispositionFileName_FollowsCurlParsing()
        {
            TransferOutput.ContentDispositionFileName("filename=name1311; charset=funny").Should().Be("name1311");
            TransferOutput.ContentDispositionFileName("inline; filename=\"name;weird\"").Should().Be("name;weird");
            TransferOutput.ContentDispositionFileName("inline; filename='unclosed").Should().Be("unclosed");
            TransferOutput.ContentDispositionFileName("filename=log\\server\\name").Should().Be("name");
            TransferOutput.ContentDispositionFileName("inline").Should().BeNull();
        }

        [Fact]
        public async Task NoClobber_PicksNumberedName()
        {
            using var server = new RawReplyServer(Ok);
            File.WriteAllText(P("save.txt"), "old");
            File.WriteAllText(P("plain"), "old");

            await Curl.ExecuteAsync($"curl --no-clobber -o {Q(P("save.txt"))} {server.Url("/")}");
            await Curl.ExecuteAsync($"curl --no-clobber -o {Q(P("plain"))} {server.Url("/")}");

            File.ReadAllText(P("save.txt")).Should().Be("old");
            File.ReadAllText(P("save-1.txt")).Should().Be("hello");
            File.ReadAllText(P("plain.1")).Should().Be("hello");
        }

        [Fact]
        public async Task RemoteTime_SetsFileTimeFromLastModified()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nLast-Modified: Tue, 13 Jun 2000 12:10:00 GMT\r\nContent-Length: 5\r\n\r\nhello");
            var output = P("dated.txt");

            await Curl.ExecuteAsync($"curl -R -o {Q(output)} {server.Url("/")}");

            File.GetLastWriteTimeUtc(output).Should().Be(new DateTime(2000, 6, 13, 12, 10, 0, DateTimeKind.Utc));
        }

#if NET5_0_OR_GREATER
        [Fact]
        public async Task RemoveOnError_DeletesNothingButNeverLeavesAPartialFile()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 100\r\n\r\nonly part");
            var output = P("partial.bin");

            Func<Task> act = () => Curl.ExecuteAsync($"curl --remove-on-error -o {Q(output)} {server.Url("/")}");

            (await act.Should().ThrowAsync<CurlPartialFileException>()).Which.CurlErrorCode.Should().Be(18);
            File.Exists(output).Should().BeFalse();
        }

        [Fact]
        public async Task PartialBody_WithoutRemoveOnError_KeepsWhatArrived()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 100\r\n\r\nonly part");
            var output = P("partial.bin");

            Func<Task> act = () => Curl.ExecuteAsync($"curl -o {Q(output)} {server.Url("/")}");

            var thrown = await act.Should().ThrowAsync<CurlPartialFileException>();
            thrown.Which.BytesExpected.Should().Be(100);
            thrown.Which.BytesReceived.Should().Be(9);
            File.ReadAllText(output).Should().Be("only part");
        }
#endif

        [Fact]
        public async Task HeadFlag_PrintsHeadersWithoutI()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 5\r\nX-Thing: yes\r\n\r\n");

            var result = await Curl.ExecuteAsync($"curl -I {server.Url("/")}");

            FirstLine(server.Requests.Single()).Should().Be("HEAD / HTTP/1.1");
            result.Body.Should().StartWith("HTTP/1.1 200 OK\r\n").And.Contain("X-Thing: yes");
        }

        // ------------------------------------------------------------------ -w / --write-out

        [Fact]
        public async Task WriteOut_ExpandsCommonVariablesAndEscapes()
        {
            using var server = new RawReplyServer("HTTP/1.1 201 Created\r\nContent-Type: text/plain; charset=us-ascii\r\nX-Id: 42\r\nContent-Length: 5\r\n\r\nhello");

            var result = await Curl.ExecuteAsync(
                $"curl -w \"\\n%{{http_code}} %{{response_code}} %{{size_download}} %{{content_type}} %header{{x-id}} %{{method}} %{{scheme}} %{{http_version}} %{{num_headers}} 100%%\\t.\" {server.Url("/w")}");

            result.Body.Should().Be("hello\n201 201 5 text/plain; charset=us-ascii 42 GET http 1.1 3 100%\t.");
        }

        [Fact]
        public async Task WriteOut_TimesUseSixDecimalsAndUrlsAreReported()
        {
            using var server = new RawReplyServer(Ok);

            var result = await Curl.ExecuteAsync($"curl -w \"%{{time_total}}|%{{time_connect}}|%{{url}}|%{{url_effective}}|%{{exitcode}}|%{{url.port}}\" -o {Q(P("o"))} {server.Url("/t")}");

            var parts = result.Body.Split('|');
            Regex.IsMatch(parts[0], @"^\d+\.\d{6}$").Should().BeTrue(parts[0]);
            Regex.IsMatch(parts[1], @"^\d+\.\d{6}$").Should().BeTrue(parts[1]);
            parts[2].Should().Be(server.Url("/t"));
            parts[3].Should().Be(server.Url("/t"));
            parts[4].Should().Be("0");
            parts[5].Should().Be(server.Port.ToString());
        }

        [Fact]
        public async Task WriteOut_StderrSwitch_KeepsThatPartOutOfTheBody()
        {
            using var server = new RawReplyServer(Ok);
            var stderr = P("stderr.txt");

            var result = await Curl.ExecuteAsync($"curl --stderr {Q(stderr)} -w \"a%{{stderr}}b%{{stdout}}c\" {server.Url("/")}");

            result.Body.Should().Be("helloac");
            File.ReadAllText(stderr).Should().Be("b");
        }

        [Fact]
        public async Task WriteOut_FromFile_AndJson()
        {
            using var server = new RawReplyServer(Ok);
            var format = P("format.txt");
            File.WriteAllText(format, "%{json}");

            var result = await Curl.ExecuteAsync($"curl -o {Q(P("o"))} -w @{Q(format)} {server.Url("/j")}");

            result.Body.Should().StartWith("{").And.EndWith("}");
            result.Body.Should().Contain("\"http_code\":\"200\"").And.Contain("\"size_download\":5");
        }

        [Fact]
        public async Task WriteOut_NumRedirectsAndFilenameEffective()
        {
            using var server = new RawReplyServer("HTTP/1.1 302 Found\r\nLocation: /final\r\nContent-Length: 0\r\n\r\n", Ok);
            var output = P("final.txt");

            var result = await Curl.ExecuteAsync($"curl -L -o {Q(output)} -w \"%{{num_redirects}} %{{filename_effective}} %{{url_effective}}\" {server.Url("/start")}");

            result.Body.Should().Be($"1 {output} {server.Url("/final")}");
        }

        [Fact]
        public void WriteOutFormatter_UnknownVariable_ExpandsToNothing()
        {
            var ctx = new WriteOutContext { ResponseCode = 200 };

            WriteOutFormatter.Format("[%{nope}][%{http_code}][\\x]", ctx).Stdout.Should().Be("[][200][\\x]");
        }

        [Fact]
        public void WriteOutFormatter_OnError_HidesTailForSuccess()
        {
            WriteOutFormatter.Format("ok%{onerror}failed", new WriteOutContext { ExitCode = 0 }).Stdout.Should().Be("ok");
            WriteOutFormatter.Format("ok%{onerror}failed", new WriteOutContext { ExitCode = 7 }).Stdout.Should().Be("okfailed");
        }

        // ------------------------------------------------------------------ cookies

        [Fact]
        public async Task CookieFile_NetscapeFormat_SendsMatchingCookiesOnly()
        {
            using var server = new RawReplyServer(Ok);
            var jar = P("cookies.txt");
            File.WriteAllText(jar,
                "# Netscape HTTP Cookie File\n" +
                "127.0.0.1\tFALSE\t/\tFALSE\t0\tsession\tone\n" +
                "127.0.0.1\tFALSE\t/other/\tFALSE\t0\twrongpath\tx\n" +
                "example.com\tFALSE\t/\tFALSE\t0\twronghost\tx\n" +
                "127.0.0.1\tFALSE\t/\tFALSE\t1\texpired\tx\n" +
                "#HttpOnly_127.0.0.1\tFALSE\t/we\tFALSE\t0\tdeep\ttwo\n");

            await Curl.ExecuteAsync($"curl -b {Q(jar)} {server.Url("/we/want")}");

            Line(server.Requests.Single(), "Cookie").Should().Be("Cookie: deep=two; session=one");
        }

        [Fact]
        public async Task CookieFile_JunkSessionCookies_DropsSessionCookies()
        {
            using var server = new RawReplyServer(Ok);
            var jar = P("cookies.txt");
            File.WriteAllText(jar, "127.0.0.1\tFALSE\t/\tFALSE\t0\tsession\tone\n127.0.0.1\tFALSE\t/\tFALSE\t4102444800\tlasting\ttwo\n");

            await Curl.ExecuteAsync($"curl -j -b {Q(jar)} {server.Url("/")}");

            Line(server.Requests.Single(), "Cookie").Should().Be("Cookie: lasting=two");
        }

        [Fact]
        public async Task CookieJar_WritesNetscapeFormatWithHttpOnlyPrefix()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nSet-Cookie: plain=1; path=/\r\nSet-Cookie: secret=2; HttpOnly; path=/\r\nContent-Length: 0\r\n\r\n");
            var jar = P("jar.txt");

            await Curl.ExecuteAsync($"curl -c {Q(jar)} {server.Url("/a/b")}");

            var text = File.ReadAllText(jar);
            text.Should().StartWith("# Netscape HTTP Cookie File\n");
            text.Should().Contain("#HttpOnly_127.0.0.1\tFALSE\t/\tFALSE\t0\tsecret\t2\n");
            text.Should().Contain("127.0.0.1\tFALSE\t/\tFALSE\t0\tplain\t1\n");
        }

        [Fact]
        public async Task CookieEngine_CookiesFromARedirectAreSentOnTheNextHop()
        {
            using var server = new RawReplyServer("HTTP/1.1 302 Found\r\nSet-Cookie: hop=yes\r\nLocation: /next\r\nContent-Length: 0\r\n\r\n", Ok);

            await Curl.ExecuteAsync($"curl -L -b none {server.Url("/first")}");

            Line(server.Requests[0], "Cookie").Should().BeNull();
            Line(server.Requests[1], "Cookie").Should().Be("Cookie: hop=yes");
        }

        [Fact]
        public async Task CookieString_WithoutEngine_IsSentVerbatim()
        {
            using var server = new RawReplyServer(Ok);

            await Curl.ExecuteAsync($"curl -b \"a=1; b=2\" {server.Url("/")}");

            Line(server.Requests.Single(), "Cookie").Should().Be("Cookie: a=1; b=2");
        }

        [Fact]
        public void SetCookieParsing_AppliesCurlDomainRules()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            CookieEngine.ParseSetCookie("a=1; domain=com", "www.example.com", "/", true, now).Should().BeNull("a bare TLD is refused");
            CookieEngine.ParseSetCookie("a=1; domain=other.org", "www.example.com", "/", true, now).Should().BeNull();
            CookieEngine.ParseSetCookie("a=1; secure", "example.com", "/", false, now).Should().BeNull("secure cookies need a secure origin");
            var ok = CookieEngine.ParseSetCookie("a=1; domain=.example.com; path=/dir/", "www.example.com", "/", true, now);
            ok!.TailMatch.Should().BeTrue();
            ok.Path.Should().Be("/dir");
            CookieEngine.ParseSetCookie("__Host-x=1; secure; domain=example.com", "example.com", "/", true, now).Should().BeNull();
        }

        [Fact]
        public void CurlDate_ParsesCookieExpiryFormats()
        {
            CurlDate.Parse("Fri Feb 13 11:56:27 GMT 2037").Should().Be(new DateTimeOffset(2037, 2, 13, 11, 56, 27, TimeSpan.Zero).ToUnixTimeSeconds());
            CurlDate.Parse("Tue, 13 Jun 2000 12:10:00 GMT").Should().Be(new DateTimeOffset(2000, 6, 13, 12, 10, 0, TimeSpan.Zero).ToUnixTimeSeconds());
            CurlDate.Parse("Sunday, 06-Nov-94 08:49:37 GMT").Should().Be(new DateTimeOffset(1994, 11, 6, 8, 49, 37, TimeSpan.Zero).ToUnixTimeSeconds());
            CurlDate.Parse("garbage").Should().BeNull();
        }

        // ------------------------------------------------------------------ multipart

        [Fact]
        public async Task Form_TextAndFileParts_LookLikeCurl()
        {
            using var server = new RawReplyServer(Ok);
            var file = P("notes.txt");
            File.WriteAllText(file, "file body");

            await Curl.ExecuteAsync($"curl -F name=daniel -F \"upload=@{file}\" -F \"pic=@{file};type=image/png;filename=me.png\" {server.Url("/f")}");

            var request = server.Requests.Single();
            var contentType = Line(request, "Content-Type");
            var match = Regex.Match(contentType, "^Content-Type: multipart/form-data; boundary=(-{24}[0-9A-Za-z]{22})$");
            match.Success.Should().BeTrue(contentType);
            var boundary = match.Groups[1].Value;
            var body = request.Substring(request.IndexOf("\r\n\r\n", StringComparison.Ordinal) + 4);

            body.Should().Be(
                $"--{boundary}\r\nContent-Disposition: form-data; name=\"name\"\r\n\r\ndaniel\r\n" +
                $"--{boundary}\r\nContent-Disposition: form-data; name=\"upload\"; filename=\"notes.txt\"\r\nContent-Type: text/plain\r\n\r\nfile body\r\n" +
                $"--{boundary}\r\nContent-Disposition: form-data; name=\"pic\"; filename=\"me.png\"\r\nContent-Type: image/png\r\n\r\nfile body\r\n" +
                $"--{boundary}--\r\n");
            Line(request, "Content-Length").Should().Be("Content-Length: " + Encoding.UTF8.GetByteCount(body));
        }

        [Fact]
        public async Task Form_FileContentAndFormString()
        {
            using var server = new RawReplyServer(Ok);
            var file = P("value.txt");
            File.WriteAllText(file, "from file");

            await Curl.ExecuteAsync($"curl -F \"v=<{file}\" --form-string \"lit=@not-a-file\" {server.Url("/f")}");

            var request = server.Requests.Single();
            request.Should().Contain("Content-Disposition: form-data; name=\"v\"\r\n\r\nfrom file\r\n");
            request.Should().Contain("Content-Disposition: form-data; name=\"lit\"\r\n\r\n@not-a-file\r\n");
            FirstLine(request).Should().StartWith("POST ");
        }

        [Fact]
        public async Task Form_MissingFile_IsReadError26()
        {
            using var server = new RawReplyServer(Ok);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -F \"f=@{P("does-not-exist.bin")}\" {server.Url("/f")}");

            var thrown = await act.Should().ThrowAsync<CurlReadErrorException>();
            thrown.Which.CurlErrorCode.Should().Be(26);
            thrown.Which.Message.Should().Contain("--form-string");
            server.Requests.Should().BeEmpty();
        }

        [Fact]
        public void FormParsing_HandlesQuotesTypesAndParameters()
        {
            var part = MultipartFormBuilder.Parse("html= <b>hi</b>;type=text/html;charset=x", literal: false)!;
            part.Value.Should().Be("<b>hi</b>");
            part.IsFileContent.Should().BeFalse();
            part.ContentType.Should().Be("text/html;charset=x");

            var file = MultipartFormBuilder.Parse("f=@\"my \\\"file\\\".txt\";filename=\"x;y\"", literal: false)!;
            file.IsFileUpload.Should().BeTrue();
            file.FilePath.Should().Be("my \"file\".txt");
            file.FileName.Should().Be("x;y");

            var mixed = MultipartFormBuilder.Parse("f=@a.txt,b.bin;type=x/y", literal: false)!;
            mixed.MixedFiles.Select(m => m.FilePath).Should().Equal("a.txt", "b.bin");
            mixed.MixedFiles[1].ContentType.Should().Be("x/y");

            MultipartFormBuilder.ContentTypeFor("photo.JPG").Should().Be("image/jpeg");
            MultipartFormBuilder.ContentTypeFor("data.json").Should().Be("application/octet-stream");
        }

        // ------------------------------------------------------------------ timeouts and Expect

        [Fact]
        public async Task MaxTime_BoundsTheBodyDownload()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 1000\r\n\r\nstart") { StallMilliseconds = 5000 };

            var started = DateTime.UtcNow;
            Func<Task> act = () => Curl.ExecuteAsync($"curl -m 0.5 {server.Url("/slow")}");

            (await act.Should().ThrowAsync<CurlOperationTimeoutException>()).Which.CurlErrorCode.Should().Be(28);
            (DateTime.UtcNow - started).Should().BeLessThan(TimeSpan.FromSeconds(4));
        }

        [Fact]
        public async Task Expect100_OnlyForBodiesOverOneMegabyte()
        {
            using var server = new RawReplyServer(Ok);
            var big = P("big.bin");
            File.WriteAllBytes(big, new byte[1024 * 1024 + 10]);

            await Curl.ExecuteAsync($"curl -d small=1 {server.Url("/small")}");
            await Curl.ExecuteAsync($"curl --data-binary @{Q(big)} --expect100-timeout 2 {server.Url("/big")}");

            Line(server.Requests[0], "Expect").Should().BeNull();
            Line(server.Requests[1], "Expect").Should().Be("Expect: 100-continue");
        }

        // ------------------------------------------------------------------ exit codes

        [Fact]
        public async Task EmptyReply_IsGotNothing52()
        {
            using var server = new RawReplyServer(string.Empty);

            Func<Task> act = () => Curl.ExecuteAsync($"curl {server.Url("/")}");

            var thrown = await act.Should().ThrowAsync<CurlException>();
            thrown.Which.CurlErrorCode.Should().Be(52);
        }

        [Fact]
        public async Task ConnectionRefused_IsCouldntConnect7()
        {
            Func<Task> act = () => Curl.ExecuteAsync($"curl http://127.0.0.1:{ClosedPort()}/");

            var thrown = await act.Should().ThrowAsync<CurlCouldntConnectException>();
            thrown.Which.CurlErrorCode.Should().Be(7);
            thrown.Which.ViaProxy.Should().BeFalse();
        }

#if NET5_0_OR_GREATER
        [Theory]
        [InlineData("HTTP/1.2 200 OK\r\nContent-Length: 2\r\n\r\nok", 1)]
        [InlineData("HTTP/1.1 2 OK\r\nContent-Length: 2\r\n\r\nok", 1)]
        [InlineData("just some text\n", 1)]
        [InlineData("HTTP/1.1 200 OK\r\nServer test\r\nContent-Length: 2\r\n\r\nok", 8)]
        [InlineData("HTTP/1.1 200 OK\r\nContent-Length: crap\r\n\r\nok", 8)]
        [InlineData("HTTP/1.1 200 OK\r\nLocation: a\r\nLocation: b\r\nContent-Length: 0\r\n\r\n", 8)]
        [InlineData("HTTP/1.1 200 OK\r\nTransfer-Encoding: chunked, another\r\n\r\n0\r\n\r\n", 61)]
        [InlineData("HTTP/1.1 200 OK\r\nTransfer-Encoding: chunked\r\n\r\n5\r\nab", 18)]
        [InlineData("HTTP/1.1 200 OK\r\nTransfer-Encoding: chunked\r\n\r\nZZ\r\nab\r\n", 56)]
        [InlineData("HTTP/1.1 100 Continue\r\n\r\n", 52)]
        public async Task MalformedReplies_MapToCurlExitCodes(string reply, int expected)
        {
            using var server = new RawReplyServer(reply);

            Func<Task> act = () => Curl.ExecuteAsync($"curl {server.Url("/")}");

            var thrown = await act.Should().ThrowAsync<CurlException>();
            thrown.Which.CurlErrorCode.Should().Be(expected, thrown.Which.Message);
            thrown.Which.Message.Should().Contain("docs/");
        }

        [Fact]
        public async Task Http09_WithFlag_ReturnsTheRawReply()
        {
            using var server = new RawReplyServer("no headers at all\n");

            var result = await Curl.ExecuteAsync($"curl --http0.9 {server.Url("/")}");

            result.Body.Should().Be("no headers at all\n");
        }

        [Fact]
        public async Task NulInChunkedTrailer_IsWeirdReply8()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nTransfer-Encoding: chunked\r\n\r\n6\r\n-foo-\r\n0\r\nx-trailer: he\0llo\r\n\r\n") { KeepAlive = true };

            Func<Task> act = () => Curl.ExecuteAsync($"curl {server.Url("/")}");

            (await act.Should().ThrowAsync<CurlWeirdServerReplyException>()).Which.CurlErrorCode.Should().Be(8);
        }

        [Fact]
        public async Task IncludeHeaders_PrintsHeaderLinesExactlyAsSent()
        {
            const string reply = "HTTP/1.1 200 Fine thanks\r\nServer: test-server/fake swsclose\r\nConnection: something-close, close\r\nX-Spaced:   padded   \r\nContent-Length: 2\r\n\r\nok";
            using var server = new RawReplyServer(reply);

            var result = await Curl.ExecuteAsync($"curl -i {server.Url("/")}");

            result.Body.Should().Be(reply);
            result.Headers["Server"].Should().Be("test-server/fake swsclose");
        }

        [Fact]
        public async Task ResumeFromEndOfFile_416IsAlreadyComplete()
        {
            using var server = new RawReplyServer("HTTP/1.1 416 Range Not Satisfiable\r\nContent-Length: 3\r\n\r\nbad");
            var output = P("done.bin");
            File.WriteAllText(output, "0123456789");

            await Curl.ExecuteAsync($"curl -C - -o {Q(output)} {server.Url("/")}");

            Line(server.Requests.Single(), "Range").Should().Be("Range: bytes=10-");
            File.ReadAllText(output).Should().Be("0123456789");
        }

        [Fact]
        public async Task ResumeIgnoredByServer_IsRangeError33()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 4\r\n\r\nfull");

            Func<Task> act = () => Curl.ExecuteAsync($"curl -C 2 {server.Url("/")}");

            (await act.Should().ThrowAsync<CurlRangeErrorException>()).Which.CurlErrorCode.Should().Be(33);
        }

        [Fact]
        public async Task ResumedUpload_IsBadArgument43()
        {
            using var server = new RawReplyServer(Ok);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -C 3 -d foobar {server.Url("/")}");

            (await act.Should().ThrowAsync<CurlBadFunctionArgumentException>()).Which.CurlErrorCode.Should().Be(43);
            server.Requests.Should().BeEmpty();
        }
#endif

        [Fact]
        public void ShallowCopy_DoesNotAffectTheOriginal()
        {
            var options = new CurlOptions { Url = "http://a/", Method = "POST", Data = "x" };

            var copy = options.ShallowCopy();
            copy.Url = "http://b/";
            copy.Data = null;

            options.Url.Should().Be("http://a/");
            options.Data.Should().Be("x");
        }

        [Fact]
        public void TransportOptions_AreParsed()
        {
            var parser = new CommandParser();
            var options = parser.Parse("curl -p --noproxy a,b --proxy-digest -U u:p:w --post301 --post302 --post303 --fail-with-body -D h.txt -J -R " +
                                       "--output-dir out --no-clobber --remove-on-error -j --expect100-timeout 1.5 --max-filesize 2k " +
                                       "--etag-save e --etag-compare c --stderr err.txt --form-escape --raw --tr-encoding --http0.9 http://x/");

            options.ProxyTunnel.Should().BeTrue();
            options.NoProxy.Should().Be("a,b");
            options.ProxyAuthScheme.Should().Be("digest");
            options.ProxyCredentials!.UserName.Should().Be("u");
            options.ProxyCredentials.Password.Should().Be("p:w");
            (options.Post301 && options.Post302 && options.Post303 && options.FailWithBody).Should().BeTrue();
            options.DumpHeaderFile.Should().Be("h.txt");
            (options.RemoteHeaderName && options.RemoteTime && options.NoClobber && options.RemoveOnError && options.JunkSessionCookies).Should().BeTrue();
            options.OutputDir.Should().Be("out");
            options.Expect100TimeoutSeconds.Should().Be(1.5);
            options.MaxFileSize.Should().Be(2048);
            options.EtagSave.Should().Be("e");
            options.EtagCompare.Should().Be("c");
            options.StderrFile.Should().Be("err.txt");
            (options.FormEscape && options.Raw && options.TransferEncoding && options.AllowHttp09).Should().BeTrue();
        }
    }
}
