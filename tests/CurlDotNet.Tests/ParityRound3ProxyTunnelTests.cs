using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Core.Auth;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Proxy NTLM on CONNECT tunnels opened by CurlDotNet itself, checked against the bytes curl's tests
    /// 209, 213 and 265 pin: type-1 CONNECT, 407 with the type-2 challenge on the same connection,
    /// type-3 CONNECT, then the request through the tunnel; -i prints every CONNECT response head.
    /// In-process servers only.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class ParityRound3ProxyTunnelTests : IDisposable
    {
        // curl's canned NTLM messages (tests 209/213/265, user testuser:testpass, workstation WORKSTATION)
        private const string Type1 = "TlRMTVNTUAABAAAABoIIAAAAAAAAAAAAAAAAAAAAAAA=";
        private const string Type2 = "TlRMTVNTUAACAAAAAgACADAAAACGggEAc51AYVDgyNcAAAAAAAAAAG4AbgAyAAAAQ0MCAAQAQwBDAAEAEgBFAEwASQBTAEEAQgBFAFQASAAEABgAYwBjAC4AaQBjAGUAZABlAHYALgBuAHUAAwAsAGUAbABpAHMAYQBiAGUAdABoAC4AYwBjAC4AaQBjAGUAZABlAHYALgBuAHUAAAAAAA==";
        private const string Type3 = "TlRMTVNTUAADAAAAGAAYAEAAAAAYABgAWAAAAAAAAABwAAAACAAIAHAAAAALAAsAeAAAAAAAAAAAAAAAhoIBAFpkQwKRCZFMhjj0tw47wEjKHRHlvzfxQamFcheMuv8v+xeqphEO5V41xRd7R9deOXRlc3R1c2VyV09SS1NUQVRJT04=";

        private const string Challenge407 = "HTTP/1.1 407 Authorization Required to proxy me my dear\r\nProxy-Authenticate: NTLM " + Type2 + "\r\nContent-Length: 33\r\n\r\nAnd you should ignore this data.\n";
        private const string Connected = "HTTP/1.1 200 Things are fine in proxy land\r\nServer: Microsoft-IIS/5.0\r\nContent-Type: text/html; charset=iso-8859-1\r\n\r\n";
        private const string Origin = "HTTP/1.1 200 OK\r\nContent-Length: 7\r\nFunny-head: yesyes\r\n\r\ndaniel\n";

        private readonly string _dir;

        public ParityRound3ProxyTunnelTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "curldotnet-round3-tunnel-" + Guid.NewGuid().ToString("N"));
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

        private static string Connect(string authority, string version, string? authorization) =>
            $"CONNECT {authority} HTTP/{version}\r\nHost: {authority}\r\n" +
            (authorization == null ? string.Empty : $"Proxy-Authorization: {authorization}\r\n") +
            $"User-Agent: {Curl.DefaultUserAgent}\r\nProxy-Connection: Keep-Alive\r\n\r\n";

        private static string FirstLine(string request) => request.Substring(0, request.IndexOf("\r\n", StringComparison.Ordinal));

        [Fact]
        public void ForTunnel_NegotiatesOnlyNtlm()
        {
            var credentials = new NetworkCredential("u", "p");

            HttpAuth.ForTunnel("ntlm", credentials).Should().NotBeNull();
            HttpAuth.ForTunnel("ntlm", null).Should().BeNull("no credentials, nothing to answer");
            HttpAuth.ForTunnel("basic", credentials).Should().BeNull("Basic goes out preemptively");
            HttpAuth.ForTunnel("digest", credentials).Should().BeNull("Digest on CONNECT is not confirmed against curl yet");
            HttpAuth.ForTunnel("anyauth", credentials).Should().BeNull("--proxy-anyauth on CONNECT is not confirmed against curl yet");
            HttpAuth.ForTunnel("negotiate", credentials).Should().BeNull();
        }

        [Fact]
        public void TunnelAuthorization_SendsType1ThenType3ForTheChallenge()
        {
            var auth = HttpAuth.ForTunnel("ntlm", new NetworkCredential("testuser", "testpass"))!;

            auth.TunnelAuthorization("host:80").Should().Be("NTLM " + Type1);
            auth.TunnelShouldRetry(407, new[] { "NTLM " + Type2 }).Should().BeTrue();
            auth.TunnelAuthorization("host:80").Should().Be("NTLM " + Type3);
            auth.TunnelMethodName.Should().Be("NTLM");
        }

#if NET5_0_OR_GREATER
        [Fact]
        public async Task ProxyNtlmTunnel_SendsCurlsConnectsOnOneConnection_Test209()
        {
            using var proxy = new RawReplyServer(Challenge407, Connected, Origin) { KeepAlive = true };
            var output = Path.Combine(_dir, "out.txt");

            await Curl.ExecuteAsync($"curl -i -o {CommandParser.ShellQuote(output)} http://test.remote.example.com.209:8990/path/2090002 " +
                                    $"--proxy http://127.0.0.1:{proxy.Port} --proxy-user testuser:testpass --proxy-ntlm --proxytunnel");

            var authority = "test.remote.example.com.209:8990";
            proxy.Requests.Should().HaveCount(3);
            proxy.Requests[0].Should().Be(Connect(authority, "1.1", "NTLM " + Type1));
            proxy.Requests[1].Should().Be(Connect(authority, "1.1", "NTLM " + Type3));
            FirstLine(proxy.Requests[2]).Should().Be("GET /path/2090002 HTTP/1.1");
            proxy.Connections.Should().Be(1, "NTLM authenticates the connection, so every round shares it");

            // -i: both CONNECT response heads (not the 407 body), then the tunnelled response
            File.ReadAllText(output, Encoding.Latin1).Should().Be(
                Challenge407.Substring(0, Challenge407.IndexOf("\r\n\r\n", StringComparison.Ordinal) + 4) + Connected + Origin);
        }

        [Fact]
        public async Task ProxyNtlmTunnel_Proxy10_SendsHttp10Connects_Test213()
        {
            using var proxy = new RawReplyServer(Challenge407, Connected, Origin) { KeepAlive = true };

            var result = await Curl.ExecuteAsync($"curl http://test.remote.example.com.213:8990/path/2130002 --proxy1.0 http://127.0.0.1:{proxy.Port} " +
                                                 "--proxy-user testuser:testpass --proxy-ntlm --proxytunnel -d postit");

            var authority = "test.remote.example.com.213:8990";
            result.Body.Should().Be("daniel\n");
            proxy.Requests[0].Should().Be(Connect(authority, "1.0", "NTLM " + Type1));
            proxy.Requests[1].Should().Be(Connect(authority, "1.0", "NTLM " + Type3));
            FirstLine(proxy.Requests[2]).Should().Be("POST /path/2130002 HTTP/1.1");
            proxy.Requests[2].Should().EndWith("\r\n\r\npostit", "the CONNECT handshake does not turn the POST into a probe");
        }

        [Fact]
        public async Task ProxyNtlmTunnel_ReadsAndIgnoresA407Body_Test265()
        {
            var body = "And you should ignore this data.\n" + new string('Q', 999) + "\n";
            var challenge = "HTTP/1.0 407 Authorization Required to proxy me my dear\r\nProxy-Authenticate: NTLM " + Type2 +
                            "\r\nConnection: Keep-Alive\r\nContent-Length: " + body.Length + "\r\n\r\n" + body;
            using var proxy = new RawReplyServer(challenge, Connected, Origin) { KeepAlive = true };

            var result = await Curl.ExecuteAsync($"curl http://test.remote.example.com.265:8990/path/2650002 --proxy http://127.0.0.1:{proxy.Port} " +
                                                 "--proxy-user testuser:testpass --proxy-ntlm --proxytunnel -d postit");

            result.Body.Should().Be("daniel\n");
            proxy.Requests.Should().HaveCount(3);
            proxy.Requests[1].Should().StartWith("CONNECT ").And.Contain("Proxy-Authorization: NTLM " + Type3,
                "the 1033-byte body was skipped, so the second CONNECT is read as a request");
            FirstLine(proxy.Requests[2]).Should().Be("POST /path/2650002 HTTP/1.1");
        }

        [Fact]
        public async Task ProxyNtlmTunnel_RejectedType3_IsTunnelFailureNamingTheUser()
        {
            var rejected = "HTTP/1.1 407 Denied\r\nProxy-Authenticate: NTLM\r\nContent-Length: 0\r\n\r\n";
            using var proxy = new RawReplyServer(Challenge407, rejected) { KeepAlive = true };

            Func<Task> act = () => Curl.ExecuteAsync($"curl -p -x 127.0.0.1:{proxy.Port} -U testuser:wrong --proxy-ntlm http://tunnel.invalid/");

            var thrown = await act.Should().ThrowAsync<CurlCouldntConnectException>();
            thrown.Which.CurlErrorCode.Should().Be(7);
            thrown.Which.ViaProxy.Should().BeTrue();
            thrown.Which.Message.Should().Contain("CONNECT tunnel failed, response 407")
                .And.Contain("rejected the NTLM credentials of proxy user \"testuser\"");
            proxy.Requests.Should().HaveCount(2, "a rejected type-3 ends the handshake");
        }

        [Fact]
        public async Task ProxyNtlmTunnel_Unframed407_FailsInsteadOfGuessing()
        {
            var unframed = "HTTP/1.1 407 Auth\r\nProxy-Authenticate: NTLM " + Type2 + "\r\n\r\n";
            using var proxy = new RawReplyServer(unframed) { KeepAlive = true };

            Func<Task> act = () => Curl.ExecuteAsync($"curl -p -x 127.0.0.1:{proxy.Port} -U testuser:testpass --proxy-ntlm http://tunnel.invalid/");

            var thrown = await act.Should().ThrowAsync<CurlCouldntConnectException>();
            thrown.Which.Message.Should().Contain("no Content-Length");
            proxy.Requests.Should().HaveCount(1);
        }

        [Fact]
        public async Task ProxyNtlmTunnel_HttpsUrl_IsNegotiatedByCurlDotNet()
        {
            // the "proxy" accepts the tunnel but then speaks plain HTTP, so the TLS handshake fails (35)
            using var proxy = new RawReplyServer(Challenge407, "HTTP/1.1 200 Connection established\r\n\r\nthis is not a TLS server hello at all\r\n\r\n") { KeepAlive = true };

            Func<Task> act = () => Curl.ExecuteAsync($"curl -k -x http://127.0.0.1:{proxy.Port} -U testuser:testpass --proxy-ntlm https://secure.invalid/");

            (await act.Should().ThrowAsync<CurlSslConnectErrorException>()).Which.CurlErrorCode.Should().Be(35);
            proxy.Requests[0].Should().Be(Connect("secure.invalid:443", "1.1", "NTLM " + Type1));
            proxy.Requests[1].Should().Be(Connect("secure.invalid:443", "1.1", "NTLM " + Type3));
        }

        [Fact]
        public async Task ProxyNtlmTunnel_ConnectTimeoutCoversTheHandshake()
        {
            // answers the type-1 CONNECT with the challenge, then never answers the type-3 one
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            using var stop = new CancellationTokenSource();
            var server = Task.Run(async () =>
            {
                using var client = await listener.AcceptTcpClientAsync();
                var stream = client.GetStream();
                var received = new StringBuilder();
                var buffer = new byte[8192];
                while (received.ToString().IndexOf("\r\n\r\n", StringComparison.Ordinal) < 0)
                {
                    var n = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (n <= 0)
                    {
                        return;
                    }
                    received.Append(Encoding.ASCII.GetString(buffer, 0, n));
                }
                var reply = Encoding.ASCII.GetBytes(Challenge407);
                await stream.WriteAsync(reply, 0, reply.Length);
                try
                {
                    await Task.Delay(Timeout.Infinite, stop.Token);
                }
                catch (OperationCanceledException)
                {
                }
            });

            try
            {
                Func<Task> act = () => Curl.ExecuteAsync($"curl --connect-timeout 1 -p -x 127.0.0.1:{port} -U testuser:testpass --proxy-ntlm http://tunnel.invalid/");

                (await act.Should().ThrowAsync<CurlOperationTimeoutException>()).Which.CurlErrorCode.Should().Be(28);
            }
            finally
            {
                stop.Cancel();
                listener.Stop();
                await server;
            }
        }

        [Fact]
        public async Task ProxyBasicTunnel_StillSendsPreemptiveBasic()
        {
            using var proxy = new RawReplyServer(Connected, Origin) { KeepAlive = true };

            var result = await Curl.ExecuteAsync($"curl -p -x 127.0.0.1:{proxy.Port} -U me:pw http://tunnel.invalid:8080/inside");

            result.Body.Should().Be("daniel\n");
            proxy.Requests[0].Should().Be(Connect("tunnel.invalid:8080", "1.1", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("me:pw"))));
        }
#endif
    }
}
