using System;
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
    /// Second round of curl parity: every transfer of a command line runs, partial output travels on
    /// failures, empty bodies, auth on redirects, cookie edge cases and FTP uploads.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class ParityRound2Tests : IDisposable
    {
        private const string Hello = "HTTP/1.1 200 OK\r\nContent-Length: 6\r\n\r\nhello\n";
        private const string World = "HTTP/1.1 200 OK\r\nContent-Length: 6\r\n\r\nworld\n";
        private const string NotFound = "HTTP/1.1 404 Not Found\r\nContent-Length: 5\r\n\r\ngone\n";
        private readonly string _dir;

        public ParityRound2Tests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "curldotnet-round2-" + Guid.NewGuid().ToString("N"));
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

        private static string FirstLine(string request) => request.Substring(0, request.IndexOf("\r\n", StringComparison.Ordinal));

        private static string? Line(string request, string header)
        {
            return request.Split(new[] { "\r\n" }, StringSplitOptions.None)
                .FirstOrDefault(l => l.StartsWith(header + ":", StringComparison.OrdinalIgnoreCase));
        }

        // ------------------------------------------------------------------ every transfer runs

        [Fact]
        public async Task TwoUrls_BothRequested_BodiesConcatenated()
        {
            using var server = new RawReplyServer(Hello, World);

            var result = await Curl.ExecuteAsync($"curl {server.Url("/one")} {server.Url("/two")}");

            server.Requests.Select(FirstLine).Should().Equal("GET /one HTTP/1.1", "GET /two HTTP/1.1");
            result.Body.Should().Be("hello\nworld\n");
        }

        [Fact]
        public async Task Glob_RunsEveryMatch_InOrder()
        {
            using var server = new RawReplyServer(Hello);

            await Curl.ExecuteAsync($"curl {server.Url("/{a,b}/[1-2]")}");

            server.Requests.Select(FirstLine).Should().Equal(
                "GET /a/1 HTTP/1.1", "GET /a/2 HTTP/1.1", "GET /b/1 HTTP/1.1", "GET /b/2 HTTP/1.1");
        }

        [Fact]
        public async Task Next_SecondGroupHasItsOwnOptions()
        {
            using var server = new RawReplyServer(Hello, World);

            await Curl.ExecuteAsync($"curl -d first {server.Url("/post")} --next {server.Url("/get")}");

            var requests = server.Requests;
            FirstLine(requests[0]).Should().Be("POST /post HTTP/1.1");
            requests[0].Should().EndWith("first");
            FirstLine(requests[1]).Should().Be("GET /get HTTP/1.1");
            Line(requests[1], "Content-Length").Should().BeNull();
        }

        [Fact]
        public async Task PerUrlOutputFiles_AreAllWrittenAndListed()
        {
            using var server = new RawReplyServer(Hello, World);
            var one = P("one.txt");
            var two = P("two.txt");

            var result = await Curl.ExecuteAsync($"curl -o {Q(one)} {server.Url("/1")} -o {Q(two)} {server.Url("/2")}");

            File.ReadAllText(one).Should().Be("hello\n");
            File.ReadAllText(two).Should().Be("world\n");
            result.OutputFiles.Should().Equal(one, two);
            result.Body.Should().BeEmpty();
        }

        [Fact]
        public async Task FailureThenSuccess_ThrowsTheFailure_WithAllStandardOutput()
        {
            using var server = new RawReplyServer(Hello, NotFound, World);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -f {server.Url("/1")} {server.Url("/2")} {server.Url("/3")}");

            var thrown = await act.Should().ThrowAsync<CurlHttpReturnedErrorException>();
            thrown.Which.CurlErrorCode.Should().Be(22);
            thrown.Which.StandardOutput.Should().Be("hello\nworld\n");
            thrown.Which.Data[CurlException.StandardOutputDataKey].Should().Be("hello\nworld\n");
            server.Requests.Should().HaveCount(3, "a failure does not stop the remaining transfers");
        }

        [Fact]
        public async Task FailEarly_StopsAfterTheFirstFailure()
        {
            using var server = new RawReplyServer(NotFound, World);

            Func<Task> act = () => Curl.ExecuteAsync($"curl --fail-early -f {server.Url("/1")} {server.Url("/2")}");

            await act.Should().ThrowAsync<CurlHttpReturnedErrorException>();
            server.Requests.Should().ContainSingle();
        }

        [Fact]
        public async Task CookiesSetByFirstUrl_AreSentToTheSecond_WhenTheEngineIsOn()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 200 OK\r\nSet-Cookie: session=abc; Path=/\r\nContent-Length: 0\r\n\r\n",
                World);

            await Curl.ExecuteAsync($"curl -b {Q(P("none"))} {server.Url("/login")} {server.Url("/page")}");

            Line(server.Requests[0], "Cookie").Should().BeNull();
            Line(server.Requests[1], "Cookie").Should().Be("Cookie: session=abc");
        }

        [Fact]
        public async Task CookiesSetByFirstUrl_AreNotSent_WithoutTheCookieEngine()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 200 OK\r\nSet-Cookie: session=abc; Path=/\r\nContent-Length: 0\r\n\r\n",
                World);

            await Curl.ExecuteAsync($"curl {server.Url("/login")} {server.Url("/page")}");

            Line(server.Requests[1], "Cookie").Should().BeNull();
        }

        // ------------------------------------------------------------------ -b repeated

        [Fact]
        public void RepeatedCookieStrings_AreJoinedWithSemicolonSpace()
        {
            var options = new CommandParser().Parse("curl -b \"a=1;b=2\" -b c=3 http://example.invalid/");

            options.Cookie.Should().Be("a=1;b=2; c=3");
        }

        [Fact]
        public async Task CookieFileAndCookieString_BothSent_FileCookiesFirst()
        {
            var jar = P("jar.txt");
            File.WriteAllText(jar, "127.0.0.1\tFALSE\t/\tFALSE\t0\tfromfile\tyes\n");
            using var server = new RawReplyServer(Hello);

            await Curl.ExecuteAsync($"curl -b {Q(jar)} -b \"tool=curl\" {server.Url("/")}");

            Line(server.Requests.Single(), "Cookie").Should().Be("Cookie: fromfile=yes; tool=curl");
        }

        // ------------------------------------------------------------------ partial output on failure

        [Fact]
        public async Task FailWithInclude_HeadersAreTheStandardOutput()
        {
            using var server = new RawReplyServer(NotFound);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -f -i {server.Url("/")}");

            var thrown = await act.Should().ThrowAsync<CurlHttpReturnedErrorException>();
            thrown.Which.StandardOutput.Should().Be("HTTP/1.1 404 Not Found\r\nContent-Length: 5\r\n\r\n");
        }

        [Fact]
        public async Task FailWithBody_ErrorPageIsTheStandardOutput_AndWriteOutFollows()
        {
            using var server = new RawReplyServer(NotFound);

            Func<Task> act = () => Curl.ExecuteAsync($"curl --fail-with-body -w \"[%{{http_code}} %{{exitcode}}]\" {server.Url("/")}");

            var thrown = await act.Should().ThrowAsync<CurlHttpReturnedErrorException>();
            thrown.Which.StandardOutput.Should().Be("gone\n[404 22]");
            thrown.Which.Data[CurlException.WriteOutDataKey].Should().Be("[404 22]");
        }

        [Fact]
        public async Task TooManyRedirects_IncludedHeadersAndRedirectUrlAreTheStandardOutput()
        {
            using var server = new RawReplyServer("HTTP/1.1 301 Moved\r\nLocation: /next\r\nContent-Length: 0\r\n\r\n");

            Func<Task> act = () => Curl.ExecuteAsync($"curl -i -L --max-redirs 0 -w \"%{{redirect_url}}\" {server.Url("/start")}");

            var thrown = await act.Should().ThrowAsync<CurlTooManyRedirectsException>();
            thrown.Which.CurlErrorCode.Should().Be(47);
            thrown.Which.StandardOutput.Should().Be("HTTP/1.1 301 Moved\r\nLocation: /next\r\nContent-Length: 0\r\n\r\n" + server.Url("/next"));
        }

        [Fact]
        public async Task RetriedFailWithBody_KeepsTheFirstAttemptsPage()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 429 Too Many\r\nRetry-After: 0\r\nContent-Length: 4\r\n\r\nmoo\n",
                "HTTP/1.1 200 OK\r\nContent-Length: 4\r\n\r\nhey\n");

            var result = await Curl.ExecuteAsync($"curl --retry 1 --fail-with-body {server.Url("/")}");

            result.Body.Should().Be("moo\nhey\n");
        }
    }
}
