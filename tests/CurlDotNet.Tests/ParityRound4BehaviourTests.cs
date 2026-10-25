using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Behaviour behind curl tests 164, 220, 223, 277, 669, 1148, 1293, 1455, 1473, 1489, 2305, 3201 and 3220,
    /// each checked against what real curl (built from the suite's commit) sends and prints: tracing never
    /// changes the request bytes, %{size_delivered} is the decoded size, --compressed fails like curl's
    /// streaming decoder (61 at the first corrupt block, 23 for a second gzip member), -F under a custom
    /// Content-Type, the HAProxy PROXY protocol line, -D % and the -# progress bar.
    /// All servers are in-process TcpListeners on ephemeral ports.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class ParityRound4BehaviourTests : IDisposable
    {
        private static readonly Encoding Latin1 = Encoding.GetEncoding("ISO-8859-1");
        private readonly string _dir;

        public ParityRound4BehaviourTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "curldotnet-round4-" + Guid.NewGuid().ToString("N"));
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

        private string PathOf(string name) => Path.Combine(_dir, name);

        private static string Q(string path) => "\"" + path + "\"";

        private static string Line(string request, string header) =>
            request.Split(new[] { "\r\n" }, StringSplitOptions.None)
                .FirstOrDefault(l => l.StartsWith(header + ":", StringComparison.OrdinalIgnoreCase));

        private static byte[] Gzip(string text)
        {
            using var output = new MemoryStream();
            using (var gz = new GZipStream(output, CompressionLevel.Optimal, leaveOpen: true))
            {
                var bytes = Encoding.ASCII.GetBytes(text);
                gz.Write(bytes, 0, bytes.Length);
            }
            return output.ToArray();
        }

        private static string Reply(string headers, byte[] body) =>
            headers + "Content-Length: " + body.Length + "\r\n\r\n" + Latin1.GetString(body);

        // ------------------------------------------------------------------ tracing keeps the wire bytes (164)

        [Theory]
        [InlineData("--trace-ascii")]
        [InlineData("--trace")]
        public async Task Trace_DoesNotReformatTheRangeHeader(string traceOption)
        {
            using var server = new RawReplyServer();
            var trace = PathOf("trace.txt");

            await Curl.ExecuteAsync($"curl {traceOption} {Q(trace)} -r 0-10,12-15 {server.Url("/164")}");

            Line(server.Requests.Single(), "Range").Should().Be("Range: bytes=0-10,12-15",
                "reading the headers for the trace must not make HttpClient re-serialize them as 'bytes=0-10, 12-15'");
            File.Exists(trace).Should().BeTrue();
        }

        [Fact]
        public async Task Verbose_DoesNotReformatTheRangeHeader()
        {
            using var server = new RawReplyServer();

            await Curl.ExecuteAsync($"curl -v -r 0-10,12-15 {server.Url("/164")}");

            Line(server.Requests.Single(), "Range").Should().Be("Range: bytes=0-10,12-15");
        }

        // ------------------------------------------------------------------ %{size_delivered} (220)

        [Fact]
        public async Task SizeDelivered_IsTheDecodedBodySize_WhileSizeDownloadStaysTheWireSize()
        {
            var body = Gzip("line 1\n line 2\n  line 3\n");
            using var server = new RawReplyServer(Reply("HTTP/1.1 200 OK\r\nContent-Encoding: gzip\r\n", body));

            var result = await Curl.ExecuteAsync($"curl --compressed -s -w \"[%{{size_delivered}} %{{size_download}}]\" {server.Url("/220")}");

            result.Body.Should().Be($"line 1\n line 2\n  line 3\n[24 {body.Length}]");
        }

        [Fact]
        public async Task SizeDelivered_DoesNotCountIncludedHeaders_AndJsonCarriesTheSameValue()
        {
            // curl counts only body bytes handed to the write callback (lib/cw-out.c), even with -i
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 5\r\n\r\nhello");

            var result = await Curl.ExecuteAsync($"curl -i -s -w \"\\n%{{size_delivered}}\\n%{{json}}\" {server.Url("/")}");

            var lines = result.Body.Split('\n');
            lines[lines.Length - 2].Should().Be("5");
            lines[lines.Length - 1].Should().Contain("\"size_delivered\":5");
        }

        [Fact]
        public async Task SizeDelivered_IsZero_WhenFailHidesTheErrorBody()
        {
            using var server = new RawReplyServer("HTTP/1.1 404 Not Found\r\nContent-Length: 9\r\n\r\nnot found");

            Func<Task> act = () => Curl.ExecuteAsync($"curl -f -s -w \"[%{{size_delivered}} %{{size_download}}]\" {server.Url("/")}");

            var thrown = (await act.Should().ThrowAsync<CurlHttpReturnedErrorException>()).Which;
            TransferStdout.Get(thrown).Should().Be("[0 9]");
        }

        // ------------------------------------------------------------------ corrupt deflate fails at once (223)

        [Fact]
        public async Task BrokenDeflate_FailsWith61_WithoutWaitingForTheRestOfTheBody()
        {
            // a raw deflate stream that is corrupt from its first block, announced as 1305 bytes but only 20
            // sent, and the server then stalls: curl's streaming decoder fails on the first bytes (exit 61)
            var corrupt = new byte[] { 0x58, 0xdb, 0x6e, 0xe3, 0x36, 0x10, 0x7d, 0x37, 0x90, 0x7f, 0x60, 0xfd, 0xd4, 0x02, 0xb6, 0x6e, 0xb6, 0x13, 0x39, 0x70 };
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Encoding: deflate\r\nContent-Length: 1305\r\n\r\n" + Latin1.GetString(corrupt))
            {
                StallMilliseconds = 20000,
            };
            var started = DateTime.UtcNow;

            Func<Task> act = () => Curl.ExecuteAsync($"curl --compressed -s {server.Url("/223")}");

            var thrown = (await act.Should().ThrowAsync<CurlBadContentEncodingException>()).Which;
            thrown.CurlErrorCode.Should().Be(61);
            (DateTime.UtcNow - started).Should().BeLessThan(TimeSpan.FromSeconds(10), "the stall must not be waited out");
        }

        [Fact]
        public async Task ValidDeflate_StillDecodes_ZlibAndRaw()
        {
            var text = "deflate works\n";
            byte[] raw;
            using (var ms = new MemoryStream())
            {
                using (var d = new DeflateStream(ms, CompressionLevel.Optimal, leaveOpen: true))
                {
                    var b = Encoding.ASCII.GetBytes(text);
                    d.Write(b, 0, b.Length);
                }
                raw = ms.ToArray();
            }
            var zlib = new byte[] { 0x78, 0x9c }.Concat(raw).Concat(new byte[] { 0, 0, 0, 0 }).ToArray();

            using var rawServer = new RawReplyServer(Reply("HTTP/1.1 200 OK\r\nContent-Encoding: deflate\r\n", raw));
            using var zlibServer = new RawReplyServer(Reply("HTTP/1.1 200 OK\r\nContent-Encoding: deflate\r\n", zlib));

            (await Curl.ExecuteAsync($"curl --compressed -s {rawServer.Url()}")).Body.Should().Be(text);
            (await Curl.ExecuteAsync($"curl --compressed -s {zlibServer.Url()}")).Body.Should().Be(text);
        }

        [Fact]
        public async Task LargeGzipBody_ArrivingInManyReads_DecodesExactly()
        {
            // incompressible data, so the compressed body spans many socket reads while the probe decodes it
            var random = new Random(1234);
            var sb = new StringBuilder();
            for (var i = 0; i < 300000; i++)
            {
                sb.Append((char)random.Next(33, 127));
            }
            var text = sb.ToString();
            using var server = new RawReplyServer(Reply("HTTP/1.1 200 OK\r\nContent-Encoding: gzip\r\n", Gzip(text)));

            var result = await Curl.ExecuteAsync($"curl --compressed -s {server.Url("/big")}");

            result.Body.Should().Be(text);
        }

        [Fact]
        public async Task GzipLabelledZlibBody_IsDecoded_LikeZlibsAutomaticHeaderDetection()
        {
            byte[] raw;
            using (var ms = new MemoryStream())
            {
                using (var d = new DeflateStream(ms, CompressionLevel.Optimal, leaveOpen: true))
                {
                    var b = Encoding.ASCII.GetBytes("zlib inside gzip\n");
                    d.Write(b, 0, b.Length);
                }
                raw = ms.ToArray();
            }
            var zlib = new byte[] { 0x78, 0x9c }.Concat(raw).ToArray();
            using var server = new RawReplyServer(Reply("HTTP/1.1 200 OK\r\nContent-Encoding: gzip\r\n", zlib));

            (await Curl.ExecuteAsync($"curl --compressed -s {server.Url()}")).Body.Should().Be("zlib inside gzip\n");
        }

        // ------------------------------------------------------------------ one gzip member only (2305)

        [Fact]
        public async Task TwoGzipMembers_FailWith23_MultiMemberGzipResponseNotSupported()
        {
            var body = Gzip("Hello, ").Concat(Gzip("World!\n")).ToArray();
            using var server = new RawReplyServer(Reply("HTTP/1.1 200 OK\r\nContent-Encoding: gzip\r\n", body));

            Func<Task> act = () => Curl.ExecuteAsync($"curl --compressed --silent --show-error {server.Url("/2305")}");

            var thrown = (await act.Should().ThrowAsync<CurlDecodedTrailingDataException>()).Which;
            thrown.CurlErrorCode.Should().Be(23);
            thrown.CurlErrorMessage.Should().Be("Multi-member gzip response not supported");
            thrown.AnotherGzipMember.Should().BeTrue();
            thrown.TrailingBytes.Should().Be(Gzip("World!\n").Length);
            thrown.Suggestions.Should().NotBeEmpty();
            thrown.DocumentationUrl.Should().StartWith("https://");
        }

        [Fact]
        public void Gunzip_OneMember_Decodes_AndOtherTrailingBytesAreCurlsWriteError()
        {
            ContentDecoding.Gunzip(Gzip("single member"), null).Should().Equal(Encoding.ASCII.GetBytes("single member"));

            var withGarbage = Gzip("x").Concat(new byte[] { (byte)'\n' }).ToArray();
            Action act = () => ContentDecoding.Gunzip(withGarbage, null);
            var thrown = act.Should().Throw<CurlDecodedTrailingDataException>().Which;
            thrown.AnotherGzipMember.Should().BeFalse();
            thrown.CurlErrorMessage.Should().Be("Failed writing received data to disk/application");
            thrown.TrailingBytes.Should().Be(1);
        }

        [Fact]
        public void Gunzip_TruncatedMember_IsAcceptedLikeZlibWaitingForMore()
        {
            var full = Gzip("a longer text that compresses into several bytes of deflate data");
            var truncated = full.Take(full.Length - 8).ToArray(); // the CRC-32/ISIZE trailer never arrived

            Action act = () => ContentDecoding.Gunzip(truncated, null);

            act.Should().NotThrow<CurlDecodedTrailingDataException>();
        }

        [Fact]
        public void Crc32_MatchesTheGzipCheckValue()
        {
            ContentDecoding.Crc32(Encoding.ASCII.GetBytes("123456789")).Should().Be(0xCBF43926u);
        }

        // ------------------------------------------------------------------ -F with a custom Content-Type (277, 669, 1293)

        [Fact]
        public async Task Form_WithTextInfoContentType_KeepsTheTypeAndSendsAttachmentParts()
        {
            using var server = new RawReplyServer();

            await Curl.ExecuteAsync($"curl {server.Url("/want/277")} -F name=daniel -H \"Content-Type: text/info\"");

            var request = server.Requests.Single();
            var type = Line(request, "Content-Type");
            type.Should().MatchRegex("^Content-Type: text/info; boundary=-{24}[0-9A-Za-z]{22}$");
            var boundary = type!.Substring(type.IndexOf("boundary=", StringComparison.Ordinal) + 9);
            var body = request.Substring(request.IndexOf("\r\n\r\n", StringComparison.Ordinal) + 4);
            body.Should().Be($"--{boundary}\r\nContent-Disposition: attachment; name=\"name\"\r\n\r\ndaniel\r\n--{boundary}--\r\n");
            Line(request, "Content-Length").Should().Be("Content-Length: 158");
            request.Split(new[] { "\r\n" }, StringSplitOptions.None).Count(l => l.StartsWith("Content-Type:", StringComparison.OrdinalIgnoreCase))
                .Should().Be(1, "the user's header is replaced, not sent twice");
        }

        [Fact]
        public async Task Form_WithMultipartContentTypeParameter_KeepsTheParameterBeforeTheBoundary()
        {
            using var server = new RawReplyServer();

            await Curl.ExecuteAsync($"curl {server.Url("/we/want/669")} -H \"Content-type: multipart/form-data; charset=utf-8\" -F name=daniel -F tool=curl");

            var request = server.Requests.Single();
            Line(request, "Content-Type").Should().MatchRegex("^Content-Type: multipart/form-data; charset=utf-8; boundary=-{24}[0-9A-Za-z]{22}$");
            request.Should().Contain("Content-Disposition: form-data; name=\"name\"\r\n").And.Contain("Content-Disposition: form-data; name=\"tool\"\r\n");
            Line(request, "Content-Length").Should().Be("Content-Length: 260");
        }

        [Fact]
        public async Task Form_EmptyName_SendsFormDataWithoutAName()
        {
            using var server = new RawReplyServer();

            await Curl.ExecuteAsync($"curl {server.Url("/1293")} -F=");

            var request = server.Requests.Single();
            request.Should().Contain("\r\nContent-Disposition: form-data\r\n\r\n\r\n");
            Line(request, "Content-Length").Should().Be("Content-Length: 138");
        }

        [Fact]
        public async Task Form_FileUnderCustomType_IsAnAttachmentWithNameAndFilename()
        {
            var file = PathOf("a.txt");
            File.WriteAllText(file, "hi");
            using var server = new RawReplyServer();

            await Curl.ExecuteAsync($"curl {server.Url("/")} -H \"Content-Type: multipart/related\" -F \"f=@{file}\"");

            var request = server.Requests.Single();
            Line(request, "Content-Type").Should().StartWith("Content-Type: multipart/related; boundary=");
            request.Should().Contain("Content-Disposition: attachment; name=\"f\"; filename=\"a.txt\"\r\nContent-Type: text/plain\r\n\r\nhi\r\n");
        }

        // ------------------------------------------------------------------ HAProxy PROXY protocol (1455, 3201, 3220)

#if NET5_0_OR_GREATER
        [Fact]
        public async Task HaproxyProtocol_SendsTheProxyLineFirst_WithTheConnectionsAddressesAndPorts()
        {
            using var server = new RawReplyServer();

            await Curl.ExecuteAsync($"curl {server.Url("/1455")} --haproxy-protocol -H \"Testno: 1455\"");

            var request = server.Requests.Single();
            request.Should().MatchRegex($"^PROXY TCP4 127\\.0\\.0\\.1 127\\.0\\.0\\.1 \\d+ {server.Port}\\r\\nGET /1455 HTTP/1\\.1\\r\\n");
        }

        [Theory]
        [InlineData("192.168.1.1", "TCP4")]
        [InlineData("2a04:4e42::347", "TCP6")]
        public async Task HaproxyClientIp_IsBothAddresses_AndImpliesTheProtocol(string clientIp, string family)
        {
            using var server = new RawReplyServer();

            await Curl.ExecuteAsync($"curl {server.Url("/3201")} --haproxy-clientip \"{clientIp}\"");

            server.Requests.Single().Should().MatchRegex($"^PROXY {family} {Regex.Escape(clientIp)} {Regex.Escape(clientIp)} \\d+ {server.Port}\\r\\nGET /3201 ");
        }

        [Fact]
        public async Task HaproxyProtocol_ThroughTheApi_SendsTheLineOnEveryNewConnection()
        {
            using var server = new RawReplyServer();

            await Curl.ExecuteAsync(new CurlOptions { Url = server.Url("/a"), HaproxyProtocol = true });
            await Curl.ExecuteAsync(new CurlOptions { Url = server.Url("/b"), HaproxyClientIp = "10.0.0.1" });

            server.Requests.Should().HaveCount(2);
            server.Requests[0].Should().StartWith("PROXY TCP4 127.0.0.1 127.0.0.1 ");
            server.Requests[1].Should().StartWith("PROXY TCP4 10.0.0.1 10.0.0.1 ");
        }

        [Fact]
        public async Task NoHaproxyProtocol_TurnsItOffAgain()
        {
            using var server = new RawReplyServer();

            await Curl.ExecuteAsync($"curl --haproxy-protocol --no-haproxy-protocol {server.Url("/")}");

            server.Requests.Single().Should().StartWith("GET / HTTP/1.1\r\n");
        }

        [Fact]
        public async Task HaproxyProtocol_WithACallerSuppliedHttpClient_IsRefusedInsteadOfSilentlyDropped()
        {
            using var server = new RawReplyServer();
            using var client = new System.Net.Http.HttpClient();
            var engine = new CurlEngine(client);

            Func<Task> act = () => engine.ExecuteAsync($"curl --haproxy-protocol {server.Url("/")}");

            var thrown = (await act.Should().ThrowAsync<CurlNotBuiltInException>()).Which;
            thrown.CurlErrorCode.Should().Be(4);
            thrown.Message.Should().Contain("--haproxy-protocol").And.Contain("HttpClient");
            server.Requests.Should().BeEmpty();
        }
#endif

        [Fact]
        public void HaproxyLine_FollowsCurlsFormat()
        {
            var info = new WireConnectionInfo
            {
                LocalEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 50123),
                RemoteEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.2"), 8080),
            };
            HaproxyHeader.Line(info, null).Should().Be("PROXY TCP4 127.0.0.1 127.0.0.2 50123 8080\r\n");
            HaproxyHeader.Line(info, "192.168.1.1").Should().Be("PROXY TCP4 192.168.1.1 192.168.1.1 50123 8080\r\n");
            HaproxyHeader.Line(info, "2a04:4e42::347").Should().Be("PROXY TCP6 2a04:4e42::347 2a04:4e42::347 50123 8080\r\n");
            HaproxyHeader.Line(info, "not-an-ip").Should().StartWith("PROXY TCP6 not-an-ip not-an-ip ", "curl only tests for a dotted IPv4 address");

            var v6 = new WireConnectionInfo
            {
                LocalEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.IPv6Loopback, 1),
                RemoteEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.IPv6Loopback, 2),
            };
            HaproxyHeader.Line(v6, null).Should().Be("PROXY TCP6 ::1 ::1 1 2\r\n");
        }

        [Theory]
        [InlineData("192.168.1.1", true)]
        [InlineData("0.0.0.0", true)]
        [InlineData("255.255.255.255", true)]
        [InlineData("256.1.1.1", false)]
        [InlineData("1.2.3", false)]
        [InlineData("1.2.3.4.5", false)]
        [InlineData("::1", false)]
        [InlineData("a.b.c.d", false)]
        public void IsIPv4Address_IsCurlsDottedQuadTest(string text, bool expected)
        {
            HaproxyHeader.IsIPv4Address(text).Should().Be(expected);
        }

        [Fact]
        public void Parser_ReadsTheHaproxyOptions()
        {
            var parser = new CommandParser();
            var on = parser.Parse("curl --haproxy-protocol http://example.com/");
            on.HaproxyProtocol.Should().BeTrue();
            on.HaproxyClientIp.Should().BeNull();

            var ip = parser.Parse("curl --haproxy-clientip 10.1.2.3 http://example.com/");
            ip.HaproxyClientIp.Should().Be("10.1.2.3");
            ip.Url.Should().Be("http://example.com/", "the address is the option's value, not the URL");
        }

        // ------------------------------------------------------------------ -D % (1489)

        [Fact]
        public async Task DumpHeaderPercent_WritesTheHeadersToStderr_NotToAFileNamedPercent()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nServer: test-server/fake\r\nContent-Length: 2\r\n\r\nok");
            var stderr = PathOf("stderr.txt");
            var cwdPercent = Path.Combine(Directory.GetCurrentDirectory(), "%");
            var existedBefore = File.Exists(cwdPercent);

            var result = await Curl.ExecuteAsync($"curl -s -D % --stderr {Q(stderr)} {server.Url("/1489")}");

            result.Body.Should().Be("ok");
            File.ReadAllText(stderr).Should().Be("HTTP/1.1 200 OK\r\nServer: test-server/fake\r\nContent-Length: 2\r\n\r\n");
            if (!existedBefore)
            {
                File.Exists(cwdPercent).Should().BeFalse();
            }
        }

        // ------------------------------------------------------------------ -# progress bar (1148)

        [Fact]
        public async Task ProgressBar_IsCurlsHashBar_EndingAt100PercentAndANewline()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 10\r\n\r\n0123456789");
            var stderr = PathOf("bar.txt");

            await Curl.ExecuteAsync($"curl -# --stderr {Q(stderr)} {server.Url("/1148")}");

            // curl's tool_cb_prg.c: "\r", width-7 columns of '#', " 100.0%", then a newline after the transfer
            var text = File.ReadAllText(stderr);
            var match = Regex.Match(text, "^[^\n]*\r(#+) 100\\.0%\n$");
            match.Success.Should().BeTrue($"'{text.Replace("\r", "\\r").Replace("\n", "\\n")}' is curl's bar");
            var width = match.Groups[1].Length + 7;
            width.Should().BeInRange(20, 400);
        }

        [Fact]
        public async Task ProgressBar_IsOffWithSilent()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 2\r\n\r\nok");
            var stderr = PathOf("bar.txt");

            await Curl.ExecuteAsync($"curl -# -s --stderr {Q(stderr)} {server.Url("/")}");

            (File.Exists(stderr) ? File.ReadAllText(stderr) : string.Empty).Should().BeEmpty();
        }

        // ------------------------------------------------------------------ first header line with a blank and a bare LF (1473)

#if NET5_0_OR_GREATER
        [Fact]
        public async Task FirstHeaderLineWithLeadingBlankAndBareLf_IsIncludedExactlyAsReceived()
        {
            // real curl's log/curl1473.out: the sws server sends " Date: ...\n" (runtests' crlf="headers" only
            // converts lines that start with a header name) and curl writes the bytes unchanged
            const string reply = "HTTP/1.1 200 OK\r\n Date: Tue, 09 Nov 2010 14:49:00 GMT\nServer: test-server/\r\nLast-Modified: Tue, 13 Jun 2000 12:10:00 GMT\r\nContent-Length: 6\r\n\r\n-foo-\n";
            using var server = new RawReplyServer(reply);

            var result = await Curl.ExecuteAsync($"curl -i {server.Url("/1473")}");

            result.Body.Should().Be(reply);
        }
#endif
    }
}
