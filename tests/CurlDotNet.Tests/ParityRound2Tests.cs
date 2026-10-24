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
        public async Task LastTransferFails_ThrowsIt_WithEveryTransfersStandardOutput()
        {
            using var server = new RawReplyServer(Hello, World, NotFound);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -f {server.Url("/1")} {server.Url("/2")} {server.Url("/3")}");

            var thrown = await act.Should().ThrowAsync<CurlHttpReturnedErrorException>();
            thrown.Which.CurlErrorCode.Should().Be(22);
            thrown.Which.StandardOutput.Should().Be("hello\nworld\n");
            thrown.Which.Data[CurlException.StandardOutputDataKey].Should().Be("hello\nworld\n");
        }

        [Fact]
        public async Task FailureThenSuccess_LikeCurl_TheLastTransferDecides()
        {
            // curl test 655 / 1328: the exit code is the one of the last transfer
            using var server = new RawReplyServer(Hello, NotFound, World);

            var result = await Curl.ExecuteAsync($"curl -f {server.Url("/1")} {server.Url("/2")} {server.Url("/3")}");

            result.Body.Should().Be("hello\nworld\n");
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

        [Fact]
        public async Task CookieDeletedByServer_IsNotReloadedFromTheFileForTheNextUrl()
        {
            var jar = P("jar.txt");
            var jar2 = P("jar2.txt");
            File.WriteAllText(jar, ".host.foo.com\tTRUE\t/we/want/\tFALSE\t22147483647\ttest\tno\n");
            File.WriteAllText(jar2, ".host.foo.com\tTRUE\t/we/want/\tFALSE\t22147483647\ttester\tyes\n");
            using var server = new RawReplyServer(
                "HTTP/1.1 200 OK\r\nSet-Cookie: test=yes; path=/we/want/; domain=.host.foo.com; Max-Age=0;\r\nContent-Length: 0\r\n\r\n",
                World);

            await Curl.ExecuteAsync($"curl {server.Url("/we/want/1")} -b {Q(jar)} -b {Q(jar2)} -H \"Host: host.foo.com\" {server.Url("/we/want/2")}");

            Line(server.Requests[0], "Cookie").Should().Be("Cookie: tester=yes; test=no");
            Line(server.Requests[1], "Cookie").Should().Be("Cookie: tester=yes");
        }

        [Fact]
        public async Task OutNull_DiscardsThatUrlsResponse()
        {
            using var server = new RawReplyServer(Hello, World);

            var result = await Curl.ExecuteAsync($"curl {server.Url("/1")} {server.Url("/2")} --out-null -o -");

            server.Requests.Should().HaveCount(2);
            result.Body.Should().Be("world\n");
        }

        [Fact]
        public async Task NoRemoteName_SendsThatUrlToStandardOutput_DespiteRemoteNameAll()
        {
            using var server = new RawReplyServer(Hello);

            var result = await Curl.ExecuteAsync($"curl --remote-name-all --no-remote-name --output-dir {Q(_dir)} {server.Url("/file.txt")}");

            result.Body.Should().Be("hello\n");
            File.Exists(P("file.txt")).Should().BeFalse();
        }

        // ------------------------------------------------------------------ -z time condition

        [Fact]
        public async Task TimeCondition_OlderDocument_IsNotDownloaded_AndReportsA304()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 200 OK\r\nLast-Modified: Tue, 13 Jun 1990 12:10:00 GMT\r\nContent-Length: 6\r\n\r\n-foo-\n");
            var output = P("out.txt");
            File.WriteAllText(output, "original contents\n");

            var result = await Curl.ExecuteAsync($"curl -z \"dec 12 11:00:00 1999 GMT\" -w \"%{{response_code}}\" -o {Q(output)} {server.Url("/")}");

            Line(server.Requests.Single(), "If-Modified-Since").Should().Be("If-Modified-Since: Sun, 12 Dec 1999 11:00:00 GMT");
            File.ReadAllText(output).Should().Be("original contents\n", "nothing was downloaded, so the file is untouched");
            result.Body.Should().Be("304");
            result.StatusCode.Should().Be(304);
        }

        [Fact]
        public async Task TimeCondition_Include_KeepsTheHeaders_DropsTheBody()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 200 OK\r\nLast-Modified: Tue, 13 Jun 2010 12:10:00 GMT\r\nContent-Length: 6\r\n\r\n-foo-\n");

            var result = await Curl.ExecuteAsync($"curl -i -z \"-dec 12 12:00:00 1999 GMT\" {server.Url("/")}");

            Line(server.Requests.Single(), "If-Unmodified-Since").Should().NotBeNull();
            result.Body.Should().Be("HTTP/1.1 200 OK\r\nLast-Modified: Tue, 13 Jun 2010 12:10:00 GMT\r\nContent-Length: 6\r\n\r\n");
        }

        [Fact]
        public async Task TimeCondition_NewerDocument_IsDownloaded()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 200 OK\r\nLast-Modified: Tue, 13 Jun 2002 12:10:00 GMT\r\nContent-Length: 6\r\n\r\nfresh\n");

            var result = await Curl.ExecuteAsync($"curl -z \"dec 12 12:00:00 1999 GMT\" {server.Url("/")}");

            result.Body.Should().Be("fresh\n");
            result.StatusCode.Should().Be(200);
        }

        // ------------------------------------------------------------------ .netrc

        private static string Basic(string userPass) => "Authorization: Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(userPass));

        [Fact]
        public async Task Netrc_MachineEntry_SuppliesBasicCredentials()
        {
            var netrc = P("netrc");
            File.WriteAllText(netrc, "machine 127.0.0.1 login alice password s3cret\n");
            using var server = new RawReplyServer(Hello);

            await Curl.ExecuteAsync($"curl --netrc-file {Q(netrc)} {server.Url("/")}");

            Line(server.Requests.Single(), "Authorization").Should().Be(Basic("alice:s3cret"));
        }

        [Fact]
        public async Task Netrc_UrlUser_PicksThatLogin()
        {
            var netrc = P("netrc");
            File.WriteAllText(netrc, "machine 127.0.0.1 login user1 password one\nmachine 127.0.0.1 login user2 password two\n");
            using var server = new RawReplyServer(Hello);

            await Curl.ExecuteAsync($"curl --netrc-optional --netrc-file {Q(netrc)} http://user2@127.0.0.1:{server.Port}/");

            Line(server.Requests.Single(), "Authorization").Should().Be(Basic("user2:two"));
        }

        [Fact]
        public async Task Netrc_QuotedValues_EscapesAndMacdefAreHandled()
        {
            var netrc = P("netrc");
            File.WriteAllText(netrc,
                "macdef init\n  machine 127.0.0.1 login wrong password \"broken\n\n" +
                "# a comment\nmachine 127.0.0.1\n  login \"user one\"\n  password \"with\\tTAB\"\n");
            using var server = new RawReplyServer(Hello);

            await Curl.ExecuteAsync($"curl -n --netrc-file {Q(netrc)} {server.Url("/")}");

            Line(server.Requests.Single(), "Authorization").Should().Be(Basic("user one:with\tTAB"));
        }

        [Fact]
        public async Task Netrc_UserOption_WinsOverTheFile()
        {
            var netrc = P("netrc");
            File.WriteAllText(netrc, "default login fromfile password filepass\n");
            using var server = new RawReplyServer(Hello);

            await Curl.ExecuteAsync($"curl -n --netrc-file {Q(netrc)} -u me:mine {server.Url("/")}");

            Line(server.Requests.Single(), "Authorization").Should().Be(Basic("me:mine"));
        }

        [Fact]
        public async Task Netrc_RedirectToOtherHost_UsesThatHostsEntry()
        {
            var netrc = P("netrc");
            File.WriteAllText(netrc, "machine first.example login a password 1\nmachine second.example login b password 2\n");
            using var proxy = new RawReplyServer(
                "HTTP/1.1 302 Found\r\nLocation: http://second.example/next\r\nContent-Length: 0\r\n\r\n",
                "HTTP/1.1 302 Found\r\nLocation: http://third.example/last\r\nContent-Length: 0\r\n\r\n",
                Hello);

            await Curl.ExecuteAsync($"curl -L -x 127.0.0.1:{proxy.Port} --netrc-optional --netrc-file {Q(netrc)} http://first.example/");

            var requests = proxy.Requests;
            Line(requests[0], "Authorization").Should().Be(Basic("a:1"));
            Line(requests[1], "Authorization").Should().Be(Basic("b:2"));
            Line(requests[2], "Authorization").Should().BeNull();
        }

        [Fact]
        public async Task Netrc_UnterminatedQuote_IsReadError26()
        {
            var netrc = P("netrc");
            File.WriteAllText(netrc, "machine 127.0.0.1 login x password \"oops\n");
            using var server = new RawReplyServer(Hello);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -n --netrc-file {Q(netrc)} {server.Url("/")}");

            var thrown = await act.Should().ThrowAsync<CurlReadErrorException>();
            thrown.Which.CurlErrorCode.Should().Be(26);
            thrown.Which.Message.Should().Contain(".netrc").And.Contain("closing quote");
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

        [Fact]
        public async Task EmptyReply_IsGotNothing52_AfterOneRequest()
        {
            using var server = new RawReplyServer(string.Empty);

            Func<Task> act = () => Curl.ExecuteAsync($"curl {server.Url("/want/30")}");

            var thrown = await act.Should().ThrowAsync<CurlException>();
            thrown.Which.CurlErrorCode.Should().Be(52);
            server.Requests.Should().ContainSingle("curl sends the request once; nothing is retried behind the caller's back");
        }

        [Fact]
        public async Task WriteOut_UrlNumAndUrlParts_ForEveryTransfer_EvenOnesThatFailEarly()
        {
            using var server = new RawReplyServer(Hello, World);

            var result = await Curl.ExecuteAsync(
                $"curl -o {Q(P("o1"))} -o {Q(P("o2"))} -o {Q(P("o3"))} -w \"%{{urlnum}}:%{{url.scheme}}:%{{url.port}}:%{{exitcode}}\\n\" " +
                $"{server.Url("/1")} h55p://example.invalid:1/x {server.Url("/3")}");

            result.Body.Should().Be($"0:http:{server.Port}:0\n1:h55p:1:1\n2:http:{server.Port}:0\n");
        }

        [Fact]
        public async Task Http10Reply_MakesTheNextRequestOfTheTransferHttp10()
        {
            using var server = new RawReplyServer(
                "HTTP/1.0 401 Auth\r\nWWW-Authenticate: Digest realm=\"r\", nonce=\"n\"\r\nContent-Length: 0\r\nConnection: close\r\n\r\n",
                "HTTP/1.0 200 OK\r\nContent-Length: 6\r\n\r\nhello\n");

            await Curl.ExecuteAsync($"curl -u u:p --digest {server.Url("/")}");

            FirstLine(server.Requests[0]).Should().Be("GET / HTTP/1.1");
            FirstLine(server.Requests[1]).Should().Be("GET / HTTP/1.0");
        }

        [Fact]
        public async Task TrEncoding_WithBlankedConnection_SendsJustTe()
        {
            using var server = new RawReplyServer(Hello);

            await Curl.ExecuteAsync($"curl --tr-encoding -H \"Connection;\" {server.Url("/")}");

            Line(server.Requests.Single(), "TE").Should().Be("TE: gzip");
            Line(server.Requests.Single(), "Connection").Should().Be("Connection: TE");
        }

        [Fact]
        public async Task AutoReferer_FollowsRedirects_WithThePreviousUrl()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 302 Found\r\nLocation: /next\r\nContent-Length: 0\r\n\r\n",
                Hello);

            await Curl.ExecuteAsync($"curl -L -e \"firstone.html;auto\" {server.Url("/start")}");

            Line(server.Requests[0], "Referer").Should().Be("Referer: firstone.html");
            Line(server.Requests[1], "Referer").Should().Be("Referer: " + server.Url("/start"));
        }

        [Fact]
        public async Task CookieCap_KeepsTheFirst150CookiesOfTheJar_ThenSortsThem()
        {
            var jar = P("jar.txt");
            var lines = Enumerable.Range(1, 151).Select(i => $"127.0.0.1\tFALSE\t/\tFALSE\t0\tname{i}\tv{i}");
            File.WriteAllText(jar, string.Join("\n", lines) + "\n");
            using var server = new RawReplyServer(Hello);

            await Curl.ExecuteAsync($"curl -b {Q(jar)} {server.Url("/")}");

            var cookie = Line(server.Requests.Single(), "Cookie")!;
            cookie.Should().StartWith("Cookie: name150=v150; name149=v149;");
            cookie.Should().NotContain("name151");
            cookie.Should().EndWith("name1=v1");
        }

        // ------------------------------------------------------------------ connection options

        [Fact]
        public async Task ConnectTo_SendsTheRequestToAnotherHostAndPort_KeepingTheHostHeader()
        {
            using var server = new RawReplyServer(Hello);

            var result = await Curl.ExecuteAsync($"curl --connect-to www.example.com:8081:127.0.0.1:{server.Port} http://www.example.com:8081/x");

            result.Body.Should().Be("hello\n");
            Line(server.Requests.Single(), "Host").Should().Be("Host: www.example.com:8081");
        }

        [Fact]
        public async Task ConnectTo_FirstMatchingEntryWins_EmptyPartsMatchAnything()
        {
            using var server = new RawReplyServer(Hello);
            var dead = ClosedPortNumber();

            await Curl.ExecuteAsync($"curl --connect-to ::127.0.0.1:{server.Port} --connect-to ::127.0.0.1:{dead} http://anything.example/");

            server.Requests.Should().ContainSingle();
        }

        [Theory]
        [InlineData("--resolve 8080:example.com:127.0.0.1", "--resolve")]
        [InlineData("--resolve example.com:http:127.0.0.1", "--resolve")]
        [InlineData("--connect-to ::example.com:example.com", "--connect-to")]
        [InlineData("--connect-to a:b", "--connect-to")]
        public async Task MalformedResolveOrConnectTo_IsExit49(string option, string name)
        {
            Func<Task> act = () => Curl.ExecuteAsync($"curl {option} http://example.com/");

            var thrown = await act.Should().ThrowAsync<CurlOptionSyntaxException>();
            thrown.Which.CurlErrorCode.Should().Be(49);
            thrown.Which.InvalidPart.Should().Be(name);
            thrown.Which.Message.Should().Contain("dns-errors.md");
        }

        [Fact]
        public async Task HostHeader_KeepsTheCaseOfTheUrl()
        {
            using var server = new RawReplyServer(Hello);

            await Curl.ExecuteAsync($"curl --resolve MiXeD.example:{server.Port}:127.0.0.1 http://MiXeD.example:{server.Port}/");

            Line(server.Requests.Single(), "Host").Should().Be($"Host: MiXeD.example:{server.Port}");
        }

        [Fact]
        public async Task LaterTransferThatCannotBeSetUp_EndsTheCommandAfterTheEarlierOnesRan()
        {
            var good = P("in.txt");
            File.WriteAllText(good, "surprise!\n");
            using var server = new RawReplyServer(Hello);

            Func<Task> act = () => Curl.ExecuteAsync($"curl {server.Url("/u")} -T \"{{{good},{P("missing.txt")}}}\"");

            var thrown = await act.Should().ThrowAsync<CurlReadErrorException>();
            thrown.Which.CurlErrorCode.Should().Be(26);
            server.Requests.Should().ContainSingle().Which.Should().EndWith("surprise!\n");
        }

        [Fact]
        public async Task DumpHeader_SharedByTwoTransfers_KeepsBothBlocks()
        {
            using var server = new RawReplyServer(Hello, World);
            var heads = P("heads.txt");

            await Curl.ExecuteAsync($"curl -D {Q(heads)} {server.Url("/1")} {server.Url("/2")}");

            File.ReadAllText(heads).Should().Be("HTTP/1.1 200 OK\r\nContent-Length: 6\r\n\r\nHTTP/1.1 200 OK\r\nContent-Length: 6\r\n\r\n");
        }

        [Fact]
        public async Task DumpHeader_RepeatedAfterNext_StartsTheFileOver()
        {
            using var server = new RawReplyServer(Hello, World, "HTTP/1.1 200 OK\r\nContent-Length: 2\r\n\r\nok");
            var heads = P("heads.txt");

            await Curl.ExecuteAsync($"curl -D {Q(heads)} {server.Url("/1")} {server.Url("/2")} --next -D {Q(heads)} {server.Url("/3")}");

            File.ReadAllText(heads).Should().Be("HTTP/1.1 200 OK\r\nContent-Length: 2\r\n\r\n");
        }

        [Fact]
        public async Task RemoteHeaderName_NeverOverwritesTheUrlFallbackName_Either()
        {
            var existing = P("name1718");
            File.WriteAllText(existing, "initial content\n");
            using var server = new RawReplyServer(
                "HTTP/1.1 200 OK\r\nContent-Disposition: inline\r\nContent-Length: 8\r\n\r\nPAYLOAD\n");

            Func<Task> act = () => Curl.ExecuteAsync($"curl -O -J --output-dir {Q(_dir)} {server.Url("/name1718")}");

            (await act.Should().ThrowAsync<CurlWriteErrorException>()).Which.CurlErrorCode.Should().Be(23);
            File.ReadAllText(existing).Should().Be("initial content\n");
        }

        [Fact]
        public async Task NulByteInChunkedTrailer_IsWeirdServerReply8()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 200 OK\r\nTransfer-Encoding: chunked\r\nTrailer: t\r\n\r\n6\r\n-foo-\n\r\n0\r\nt: he\0llo\r\n\r\n");

            Func<Task> act = () => Curl.ExecuteAsync($"curl {server.Url("/")}");

            (await act.Should().ThrowAsync<CurlWeirdServerReplyException>()).Which.CurlErrorCode.Should().Be(8);
        }

        private static int ClosedPortNumber()
        {
            var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
            listener.Start();
            var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        // ------------------------------------------------------------------ FTP upload

        [Fact]
        public async Task FtpUpload_SendsTheUploadFilesExactBytes()
        {
            var file = P("upload.bin");
            var bytes = new byte[] { 0, 1, 2, 0xFF, (byte)'\n', 0x80, 0x7F };
            File.WriteAllBytes(file, bytes);
            using var ftp = new MiniFtpServer();

            await Curl.ExecuteAsync($"curl -T {Q(file)} ftp://127.0.0.1:{ftp.Port}/incoming/");

            ftp.StoredPath.Should().Contain("upload.bin");
            ftp.Stored.Should().Equal(bytes);
        }

        [Fact]
        public void FtpUploadPayload_PrefersTheUploadFile_ThenData_ThenNothing()
        {
            FtpHandler.UploadPayload(new CurlOptions { BinaryData = new byte[] { 9 }, Data = "text" }).Should().Equal(new byte[] { 9 });
            FtpHandler.UploadPayload(new CurlOptions { Data = "text" }).Should().Equal(Encoding.UTF8.GetBytes("text"));
            FtpHandler.UploadPayload(new CurlOptions()).Should().BeEmpty();
        }

        /// <summary>Just enough of an FTP server for FtpWebRequest to STOR one file in passive mode.</summary>
        private sealed class MiniFtpServer : IDisposable
        {
            private readonly System.Net.Sockets.TcpListener _control = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);

            public MiniFtpServer()
            {
                _control.Start();
                Port = ((System.Net.IPEndPoint)_control.LocalEndpoint).Port;
                _ = Task.Run(ServeAsync);
            }

            public int Port { get; }

            public string StoredPath { get; private set; } = string.Empty;

            public byte[] Stored { get; private set; } = Array.Empty<byte>();

            private async Task ServeAsync()
            {
                try
                {
                    using var client = await _control.AcceptTcpClientAsync();
                    using var stream = client.GetStream();
                    using var reader = new StreamReader(stream, Encoding.ASCII);
                    using var writer = new StreamWriter(stream, Encoding.ASCII) { NewLine = "\r\n", AutoFlush = true };
                    System.Net.Sockets.TcpListener? data = null;
                    await writer.WriteLineAsync("220 mini ftp");
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var verb = line.Split(' ')[0].ToUpperInvariant();
                        switch (verb)
                        {
                            case "USER": await writer.WriteLineAsync("331 password please"); break;
                            case "PASS": await writer.WriteLineAsync("230 logged in"); break;
                            case "PWD": await writer.WriteLineAsync("257 \"/\" is current"); break;
                            case "CWD": await writer.WriteLineAsync("250 ok"); break;
                            case "TYPE": await writer.WriteLineAsync("200 type set"); break;
                            case "PASV":
                                data = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
                                data.Start();
                                var port = ((System.Net.IPEndPoint)data.LocalEndpoint).Port;
                                await writer.WriteLineAsync($"227 Entering Passive Mode (127,0,0,1,{port / 256},{port % 256})");
                                break;
                            case "STOR":
                                StoredPath = line.Substring(5);
                                await writer.WriteLineAsync("150 send it");
                                using (var dataClient = await data!.AcceptTcpClientAsync())
                                using (var ms = new MemoryStream())
                                {
                                    await dataClient.GetStream().CopyToAsync(ms);
                                    Stored = ms.ToArray();
                                }
                                data.Stop();
                                await writer.WriteLineAsync("226 stored");
                                break;
                            case "QUIT": await writer.WriteLineAsync("221 bye"); return;
                            default: await writer.WriteLineAsync("200 ok"); break;
                        }
                    }
                }
                catch (Exception ex) when (ex is IOException || ex is ObjectDisposedException || ex is System.Net.Sockets.SocketException)
                {
                }
            }

            public void Dispose() => _control.Stop();
        }

        // ------------------------------------------------------------------ crypto primitives

        private static string Hex(byte[] bytes) => string.Concat(bytes.Select(b => b.ToString("x2")));

        private static byte[] FromHex(string hex) =>
            Enumerable.Range(0, hex.Length / 2).Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16)).ToArray();

        [Fact]
        public void Des_MatchesTheClassicTestVector()
        {
            var output = new byte[8];
            CurlDotNet.Core.Auth.Des.Encrypt(FromHex("133457799bbcdff1"), FromHex("0123456789abcdef"), output, 0);

            Hex(output).Should().Be("85e813540f0ab405");
        }

        [Fact]
        public void Md4_MatchesRfc1320Vectors()
        {
            Hex(CurlDotNet.Core.Auth.Md4.Hash(Array.Empty<byte>())).Should().Be("31d6cfe0d16ae931b73c59d7e0c089c0");
            Hex(CurlDotNet.Core.Auth.Md4.Hash(Encoding.ASCII.GetBytes("abc"))).Should().Be("a448017aaf21d8525fc10ae87aa6729d");
            Hex(CurlDotNet.Core.Auth.Md4.Hash(Encoding.ASCII.GetBytes("12345678901234567890123456789012345678901234567890123456789012345678901234567890")))
                .Should().Be("e33b4ddc9c38f2199c3e7b164fcc0536");
        }

        // ------------------------------------------------------------------ challenge authentication

        // curl's test 67: the server's type-2 challenge and the exact type-3 answer curl sends for testuser:testpass
        private const string Type2 = "TlRMTVNTUAACAAAAAgACADAAAACGggEAc51AYVDgyNcAAAAAAAAAAG4AbgAyAAAAQ0MCAAQAQwBDAAEAEgBFAEwASQBTAEEAQgBFAFQASAAEABgAYwBjAC4AaQBjAGUAZABlAHYALgBuAHUAAwAsAGUAbABpAHMAYQBiAGUAdABoAC4AYwBjAC4AaQBjAGUAZABlAHYALgBuAHUAAAAAAA==";
        private const string Type1 = "TlRMTVNTUAABAAAABoIIAAAAAAAAAAAAAAAAAAAAAAA=";
        private const string Type3 = "TlRMTVNTUAADAAAAGAAYAEAAAAAYABgAWAAAAAAAAABwAAAACAAIAHAAAAALAAsAeAAAAAAAAAAAAAAAhoIBAFpkQwKRCZFMhjj0tw47wEjKHRHlvzfxQamFcheMuv8v+xeqphEO5V41xRd7R9deOXRlc3R1c2VyV09SS1NUQVRJT04=";

        [Fact]
        public async Task Ntlm_SendsCurlsType1AndType3_ByteForByte()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 401 Auth\r\nWWW-Authenticate: NTLM " + Type2 + "\r\nContent-Length: 4\r\n\r\nnope",
                Hello);

            var result = await Curl.ExecuteAsync($"curl -u testuser:testpass --ntlm {server.Url("/67")}");

            result.Body.Should().Be("hello\n", "the 401 body is not part of the output");
            Line(server.Requests[0], "Authorization").Should().Be("Authorization: NTLM " + Type1);
            Line(server.Requests[1], "Authorization").Should().Be("Authorization: NTLM " + Type3);
        }

        [Fact]
        public async Task Ntlm_PostIsProbedWithContentLengthZero_ThenSentWithType3()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 401 Auth\r\nWWW-Authenticate: NTLM " + Type2 + "\r\nContent-Length: 0\r\n\r\n",
                Hello);

            await Curl.ExecuteAsync($"curl -u testuser:testpass --ntlm -d payload {server.Url("/")}");

            Line(server.Requests[0], "Content-Length").Should().Be("Content-Length: 0");
            server.Requests[0].Should().EndWith("\r\n\r\n");
            Line(server.Requests[1], "Content-Length").Should().Be("Content-Length: 7");
            server.Requests[1].Should().EndWith("payload");
        }

        [Fact]
        public async Task Digest_ProbeThenAnswer_LikeCurlTest245()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 401 authentication please\r\nWWW-Authenticate: Digest realm=\"testrealm\", nonce=\"1053604144\"\r\nContent-Length: 0\r\n\r\n",
                "HTTP/1.1 200 A OK\r\nContent-Length: 3\r\n\r\nok\n");

            var result = await Curl.ExecuteAsync($"curl -u auser:apasswd --digest -d junkelijunk {server.Url("/245")}");

            result.Body.Should().Be("ok\n");
            var first = server.Requests[0];
            Line(first, "Authorization").Should().BeNull();
            Line(first, "Content-Length").Should().Be("Content-Length: 0");
            Line(first, "Content-Type").Should().Be("Content-Type: application/x-www-form-urlencoded");
            Line(server.Requests[1], "Authorization").Should().Be(
                "Authorization: Digest username=\"auser\", realm=\"testrealm\", nonce=\"1053604144\", uri=\"/245\", response=\"379a439b1737ba257c1d2f103914b18b\"");
            server.Requests[1].Should().EndWith("junkelijunk");
        }

        [Fact]
        public async Task Digest_WithQop_UsesCnonceAndNonceCount()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 401 Auth\r\nWWW-Authenticate: Digest realm=\"r\", nonce=\"n1\", qop=\"auth-int, auth\", opaque=\"o\", algorithm=MD5\r\nContent-Length: 0\r\n\r\n",
                Hello);

            await Curl.ExecuteAsync($"curl -u u:p --digest {server.Url("/q?x=1")}");

            var header = Line(server.Requests[1], "Authorization")!;
            header.Should().StartWith("Authorization: Digest username=\"u\", realm=\"r\", nonce=\"n1\", uri=\"/q?x=1\", cnonce=\"");
            header.Should().Contain(", nc=00000001, qop=auth, response=\"").And.EndWith("\", opaque=\"o\", algorithm=MD5");

            var cnonce = System.Text.RegularExpressions.Regex.Match(header, "cnonce=\"([^\"]+)\"").Groups[1].Value;
            string Md5(string s)
            {
                using var md5 = System.Security.Cryptography.MD5.Create();
                return Hex(md5.ComputeHash(Encoding.UTF8.GetBytes(s)));
            }
            var expected = Md5(Md5("u:r:p") + ":n1:00000001:" + cnonce + ":auth:" + Md5("GET:/q?x=1"));
            header.Should().Contain("response=\"" + expected + "\"");
        }

        [Fact]
        public async Task Digest_RejectedTwice_StopsAndReturnsThe401()
        {
            const string challenge = "HTTP/1.1 401 Auth\r\nWWW-Authenticate: Digest realm=\"r\", nonce=\"n\"\r\nContent-Length: 5\r\n\r\ndenied";
            using var server = new RawReplyServer(challenge, challenge, Hello);

            var result = await Curl.ExecuteAsync($"curl -u u:wrong --digest {server.Url("/")}");

            server.Requests.Should().HaveCount(2, "a second nonce without stale=true means the password was wrong");
            result.StatusCode.Should().Be(401);
            result.Body.Should().Be("denie");
        }

        [Fact]
        public async Task AnyAuth_PicksBasicWhenThatIsAllTheServerOffers()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 401 Auth\r\nWWW-Authenticate: Basic realm=\"x\"\r\nContent-Length: 0\r\n\r\n",
                Hello);

            await Curl.ExecuteAsync($"curl -u iam:myself --anyauth {server.Url("/")}");

            Line(server.Requests[0], "Authorization").Should().BeNull("--anyauth asks first");
            Line(server.Requests[1], "Authorization").Should().Be(Basic("iam:myself"));
        }

        [Fact]
        public async Task AnyAuth_PrefersDigestOverNtlm()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 401 Auth\r\nWWW-Authenticate: NTLM\r\nWWW-Authenticate: Digest realm=\"r\", nonce=\"n\"\r\nWWW-Authenticate: Basic\r\nContent-Length: 0\r\n\r\n",
                Hello);

            await Curl.ExecuteAsync($"curl -u u:p --anyauth {server.Url("/")}");

            Line(server.Requests[1], "Authorization").Should().StartWith("Authorization: Digest username=\"u\"");
        }

        [Fact]
        public async Task Digest_WithNetrcCredentials_NeverSendsBasic()
        {
            var netrc = P("netrc");
            File.WriteAllText(netrc, "machine 127.0.0.1 login nu password np\n");
            using var server = new RawReplyServer(
                "HTTP/1.1 401 Auth\r\nWWW-Authenticate: Digest realm=\"r\", nonce=\"n\"\r\nContent-Length: 0\r\n\r\n",
                Hello);

            await Curl.ExecuteAsync($"curl --netrc-file {Q(netrc)} --digest {server.Url("/")}");

            Line(server.Requests[0], "Authorization").Should().BeNull();
            Line(server.Requests[1], "Authorization").Should().StartWith("Authorization: Digest username=\"nu\"");
        }

        [Fact]
        public async Task ProxyDigest_Answers407WithProxyAuthorization()
        {
            using var proxy = new RawReplyServer(
                "HTTP/1.1 407 Proxy auth\r\nProxy-Authenticate: Digest realm=\"p\", nonce=\"pn\"\r\nContent-Length: 0\r\n\r\n",
                Hello);

            var result = await Curl.ExecuteAsync($"curl -x 127.0.0.1:{proxy.Port} -U pu:pp --proxy-digest http://example.invalid/path");

            result.Body.Should().Be("hello\n");
            Line(proxy.Requests[0], "Proxy-Authorization").Should().BeNull("Digest needs the proxy's challenge first; no Basic may leak");
            Line(proxy.Requests[1], "Proxy-Authorization").Should().StartWith(
                "Proxy-Authorization: Digest username=\"pu\", realm=\"p\", nonce=\"pn\", uri=\"/path\", response=\"");
        }

        [Fact]
        public async Task ProxyNtlm_ProbesAPostAndAnswersTheChallenge()
        {
            using var proxy = new RawReplyServer(
                "HTTP/1.1 407 Proxy auth\r\nProxy-Authenticate: NTLM " + Type2 + "\r\nContent-Length: 0\r\n\r\n",
                Hello);

            await Curl.ExecuteAsync($"curl -x 127.0.0.1:{proxy.Port} -U testuser:testpass --proxy-ntlm -d postit http://example.invalid/");

            Line(proxy.Requests[0], "Proxy-Authorization").Should().Be("Proxy-Authorization: NTLM " + Type1);
            Line(proxy.Requests[0], "Content-Length").Should().Be("Content-Length: 0");
            Line(proxy.Requests[1], "Proxy-Authorization").Should().Be("Proxy-Authorization: NTLM " + Type3);
            proxy.Requests[1].Should().EndWith("postit");
        }

        [Fact]
        public async Task Ntlm_OverlongUser_FailsWithCode100()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 401 Auth\r\nWWW-Authenticate: NTLM " + Type2 + "\r\nContent-Length: 0\r\n\r\n",
                Hello);
            var user = new string('u', 1100);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -u {user}:p --ntlm {server.Url("/")}");

            var thrown = await act.Should().ThrowAsync<CurlException>();
            thrown.Which.CurlErrorCode.Should().Be(100);
            thrown.Which.Message.Should().Contain("too long");
        }

        [Fact]
        public async Task OAuth2Bearer_IsNotSentToAnotherHost_AndNoBasicReplacesIt()
        {
            using var proxy = new RawReplyServer(
                "HTTP/1.1 302 Found\r\nLocation: http://second.example/\r\nContent-Length: 0\r\n\r\n",
                Hello);

            await Curl.ExecuteAsync($"curl -L -x 127.0.0.1:{proxy.Port} -u a:b --oauth2-bearer tok http://first.example/");

            Line(proxy.Requests[0], "Authorization").Should().Be("Authorization: Bearer tok");
            Line(proxy.Requests[1], "Authorization").Should().BeNull();
        }
    }
}
