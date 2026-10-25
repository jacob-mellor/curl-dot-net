using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Transport behaviour behind curl tests 46, 1473, 1617, 2051 and 5029: a first response header
    /// line that starts with a blank, two user Connection headers with --tr-encoding, connection
    /// reuse across --next when --connect-to changes nothing, the TLS handshake inside
    /// --connect-timeout, and URL host names System.Uri refuses (aliased while --resolve maps them).
    /// All servers are in-process TcpListeners on ephemeral ports.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class ParityRound3TransportTests : IDisposable
    {
        private const string Ok = "HTTP/1.1 200 OK\r\nContent-Length: 2\r\nContent-Type: text/plain\r\n\r\nok";
        private static readonly Encoding Latin1 = Encoding.GetEncoding("ISO-8859-1");
        private readonly string _dir;

        public ParityRound3TransportTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "curldotnet-round3-" + Guid.NewGuid().ToString("N"));
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

        private static string Line(string request, string header)
        {
            return request.Split(new[] { "\r\n" }, StringSplitOptions.None)
                .FirstOrDefault(l => l.StartsWith(header + ":", StringComparison.OrdinalIgnoreCase));
        }

        private static List<string> Lines(string request, string header)
        {
            return request.Split(new[] { "\r\n" }, StringSplitOptions.None)
                .Where(l => l.StartsWith(header + ":", StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // ------------------------------------------------------------------ first header line with a leading blank (1473)

        [Fact]
        public void Parser_FirstHeaderLineLeadingBlanks_AreReportedHiddenAndKeptInTheRecordedHead()
        {
            var bytes = Latin1.GetBytes("HTTP/1.1 200 OK\r\n \tDate: x\r\nContent-Length: 2\r\n\r\nok");
            var parser = new ResponseStreamParser(isHeadRequest: false);
            var hidden = new List<int>();

            // one byte at a time, as the worst case of split reads
            for (var i = 0; i < bytes.Length; i++)
            {
                parser.Feed(bytes, i, 1, hidden);
            }

            hidden.Should().Equal(17, 18); // the blank and the tab after the status line's CRLF
            parser.Heads.Should().HaveCount(1);
            parser.Heads[0].Text.Should().Be("HTTP/1.1 200 OK\r\n \tDate: x\r\nContent-Length: 2\r\n\r\n");
            parser.Heads[0].GetHeader("Date").Should().Be("x");
            parser.Heads[0].BodyComplete.Should().BeTrue();
        }

        [Fact]
        public void Parser_LaterFoldedLines_AreNotHidden()
        {
            var bytes = Latin1.GetBytes("HTTP/1.1 200 OK\r\nX-A: 1\r\n  folded\r\nContent-Length: 0\r\n\r\n");
            var parser = new ResponseStreamParser(isHeadRequest: false);
            var hidden = new List<int>();

            parser.Feed(bytes, 0, bytes.Length, hidden);

            hidden.Should().BeEmpty("only the first header line can start with blanks without being a fold");
            parser.Heads.Should().HaveCount(1);
        }

        [Fact]
        public void Parser_BlankOnlyFirstHeaderLine_EndsTheHeadLikeHttpClientSeesIt()
        {
            // HttpClient never sees the hidden blanks, so for it "   \r\n" is the empty line ending the head
            var bytes = Latin1.GetBytes("HTTP/1.1 200 OK\r\n   \r\nok");
            var parser = new ResponseStreamParser(isHeadRequest: false);
            var hidden = new List<int>();

            parser.Feed(bytes, 0, bytes.Length, hidden);

            hidden.Should().Equal(17, 18, 19);
            parser.Heads.Should().HaveCount(1);
            parser.Heads[0].Text.Should().Be("HTTP/1.1 200 OK\r\n   \r\n");
        }

#if NET5_0_OR_GREATER
        [Fact]
        public async Task FirstHeaderLineStartingWithBlank_IsAccepted_AndIncludedAsReceived()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\n Date: Tue, 09 Nov 2010 14:49:00 GMT\r\nServer: test/\r\nContent-Length: 6\r\n\r\n-foo-\n");

            var result = await Curl.ExecuteAsync($"curl -i {server.Url("/1473")}");

            result.Body.Should().Be("HTTP/1.1 200 OK\r\n Date: Tue, 09 Nov 2010 14:49:00 GMT\r\nServer: test/\r\nContent-Length: 6\r\n\r\n-foo-\n");
        }

        // ------------------------------------------------------------------ --tr-encoding with two Connection headers (1617)

        [Fact]
        public async Task TrEncoding_TwoUserConnectionHeaders_TeJoinsTheFirstAndEachStaysOnItsOwnLine()
        {
            using var server = new RawReplyServer(Ok);

            await Curl.ExecuteAsync($"curl {server.Url("/1617")} --tr-encoding -H \"Connection: this\" -H \"Connection: that\"");

            var request = server.Requests.Single();
            Line(request, "TE").Should().Be("TE: gzip");
            Lines(request, "Connection").Should().Equal("Connection: this, TE", "Connection: that");
        }

        [Fact]
        public async Task TrEncoding_OneUserConnectionHeader_GetsTeAppended()
        {
            using var server = new RawReplyServer(Ok);

            await Curl.ExecuteAsync($"curl {server.Url("/")} --tr-encoding -H \"Connection: this\"");

            Lines(server.Requests.Single(), "Connection").Should().Equal("Connection: this, TE");
        }

        // ------------------------------------------------------------------ connection reuse across --next (2051)

        [Fact]
        public async Task ConnectToTheUrlsOwnHostAndPort_ReusesTheEarlierTransfersConnection()
        {
            using var server = new RawReplyServer(Ok) { KeepAlive = true };
            var url = server.Url("/2051");

            var result = await Curl.ExecuteAsync(
                $"curl {url} -w \"%{{num_connects}}\\n\" --next --connect-to ::127.0.0.1:{server.Port} {url} -w \"%{{num_connects}}\\n\" --next {url} -w \"%{{num_connects}}\\n\"");

            result.Body.Should().Be("ok1\nok0\nok0\n");
            server.Connections.Should().Be(1);
            server.Requests.Should().HaveCount(3);
        }

        [Fact]
        public void ConnectToChangesNothing_OnlyWhenTheFirstMatchingEntryKeepsHostAndPort()
        {
            bool Check(string url, bool follow, params string[] entries)
            {
                var options = new CurlOptions { Url = url, FollowLocation = follow };
                options.ConnectTo.AddRange(entries);
                return CurlTransport.ConnectToChangesNothing(options);
            }

            Check("http://127.0.0.1:8080/", false).Should().BeTrue("no --connect-to at all");
            Check("http://127.0.0.1:8080/", false, "::127.0.0.1:8080").Should().BeTrue();
            Check("http://Example.COM:80/", false, "example.com:80:EXAMPLE.com:80").Should().BeTrue("host names compare case-insensitively");
            Check("http://127.0.0.1:8080/", false, "::127.0.0.1:9090").Should().BeFalse("the port changes");
            Check("http://127.0.0.1:8080/", false, "::localhost:").Should().BeFalse("the host changes");
            Check("http://127.0.0.1:8080/", false, "other.example:80:x:1", "::127.0.0.1:8080").Should().BeTrue("entries for other hosts do not apply");
            Check("http://127.0.0.1:8080/", true, "::127.0.0.1:8080").Should().BeFalse("with -L a redirect may reach a host the entry does move");
        }

        // ------------------------------------------------------------------ TLS handshake inside --connect-timeout (5029)

        [Fact]
        public async Task TlsHandshakeThatNeverFinishes_FailsWithConnectTimeout28_AndWriteOutKeepsTheConnection()
        {
            // the server accepts TCP and reads the ClientHello, but never answers it
            using var server = new RawReplyServer(Ok) { KeepAlive = true };
            var started = DateTime.UtcNow;

            Func<Task> act = () => Curl.ExecuteAsync(
                $"curl -k https://127.0.0.1:{server.Port}/5029 --connect-timeout 1 -w \"remote_ip == %{{remote_ip}}\\nremote_port == %{{remote_port}}\\nlocal ip == %{{local_ip}}\\ntime_connect == %{{time_connect}}\\n\"");

            var thrown = (await act.Should().ThrowAsync<CurlOperationTimeoutException>()).Which;
            thrown.CurlErrorCode.Should().Be(28);
            thrown.Message.Should().Contain("SSL connection timeout").And.Contain("--connect-timeout 1").And.Contain("http");
            (DateTime.UtcNow - started).Should().BeLessThan(TimeSpan.FromSeconds(15));

            var stdout = TransferStdout.Get(thrown);
            stdout.Should().Contain("remote_ip == 127.0.0.1\n")
                .And.Contain($"remote_port == {server.Port}\n")
                .And.Contain("local ip == 127.0.0.1\n");
            var timeConnect = stdout.Split('\n').Single(l => l.StartsWith("time_connect == ", StringComparison.Ordinal)).Substring(16);
            double.Parse(timeConnect, System.Globalization.CultureInfo.InvariantCulture).Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task TlsHandshakeTimer_IsStoppedForPlainHttp_AndAFastServerIsNotCutOff()
        {
            using var server = new RawReplyServer(Ok);

            var result = await Curl.ExecuteAsync($"curl {server.Url("/")} --connect-timeout 1");

            result.Body.Should().Be("ok");
        }

        // ------------------------------------------------------------------ bad URL host names (46)

        [Fact]
        public async Task HostSystemUriRefuses_WithResolve_IsSentAsWritten_AndCookiesUseIt()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nSet-Cookie: sid=1; path=/\r\nContent-Length: 2\r\n\r\nok");
            var jar = Path.Combine(_dir, "jar.txt");

            var result = await Curl.ExecuteAsync(
                $"curl domain..tld:{server.Port}/want -w \"%{{url_effective}}\" --resolve domain..tld:{server.Port}:127.0.0.1 -c {CommandParser.ShellQuote(jar)}");

            var request = server.Requests.Single();
            request.Should().StartWith("GET /want HTTP/1.1\r\n");
            Line(request, "Host").Should().Be($"Host: domain..tld:{server.Port}");
            request.Should().NotContain(HostAlias.Suffix);
            result.Body.Should().Be($"okhttp://domain..tld:{server.Port}/want");
            File.ReadAllText(jar).Should().Contain("domain..tld\tFALSE\t/\tFALSE\t0\tsid\t1");
        }

        [Fact]
        public async Task HostSystemUriRefuses_WithoutResolveOrConnectTo_StillFailsAsMalformedUrl()
        {
            Func<Task> act = () => Curl.ExecuteAsync("curl http://domain..tld:1/x");

            (await act.Should().ThrowAsync<CurlMalformedUrlException>()).Which.CurlErrorCode.Should().Be(3);
        }
#endif

        [Fact]
        public void HostAlias_RoundTripsAnyName_AndLeavesOtherHostsAlone()
        {
            foreach (var name in new[] { "domain..tld", "-lead.x", "a_b.", new string('z', 150) })
            {
                var alias = HostAlias.Encode(name);
                Uri.TryCreate("http://" + alias + ":81/p", UriKind.Absolute, out var uri).Should().BeTrue(alias);
                HostAlias.IsAlias(uri!.Host).Should().BeTrue();
                HostAlias.Restore(uri.Host).Should().Be(name);
                HostAlias.RestoreUrl(uri.AbsoluteUri).Should().Be("http://" + name + ":81/p");
            }

            HostAlias.Restore("example.com").Should().Be("example.com");
            HostAlias.RestoreUrl("http://example.com/").Should().Be("http://example.com/");
        }

        [Fact]
        public void HostAlias_Apply_OnlyAliasesARefusedHostThatResolveOrConnectToMaps()
        {
            var mapped = new CurlOptions { Url = "http://domain..tld:8080/want" };
            mapped.Resolve["domain..tld:8080"] = "127.0.0.1";
            HostAlias.Apply(mapped);

            var unmapped = new CurlOptions { Url = "http://domain..tld:8080/want" };
            HostAlias.Apply(unmapped);

            var valid = new CurlOptions { Url = "http://example.com/" };
            valid.Resolve["example.com:80"] = "127.0.0.1";
            HostAlias.Apply(valid);

            if (CurlTransport.SupportsRecording)
            {
                mapped.Url.Should().Be("http://" + HostAlias.Encode("domain..tld") + ":8080/want");
            }
            else
            {
                mapped.Url.Should().Be("http://domain..tld:8080/want", "without a connect callback the original name could not be used");
            }
            unmapped.Url.Should().Be("http://domain..tld:8080/want");
            valid.Url.Should().Be("http://example.com/");
        }
    }
}
