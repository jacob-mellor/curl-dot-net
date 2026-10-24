using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// curl command-line parity for the parser and request shaping: every behaviour here was
    /// checked against curl 8.7.1 (and curl's own test suite) and is asserted on the parsed
    /// options or on the exact HttpRequestMessage HttpHandler builds.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class ParityParserTests : IDisposable
    {
        private readonly CommandParser _parser = new CommandParser();
        private readonly string _dir;

        public ParityParserTests()
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

        private string WriteFile(string name, string content)
        {
            var path = Path.Combine(_dir, name);
            File.WriteAllText(path, content);
            return path;
        }

        private string WriteBytes(string name, byte[] content)
        {
            var path = Path.Combine(_dir, name);
            File.WriteAllBytes(path, content);
            return path;
        }

        private static string Q(string value) => CommandParser.ShellQuote(value);

        /// <summary>Runs the options through HttpHandler with a mock transport and returns the request it sent.</summary>
        private static async Task<(HttpRequestMessage Request, byte[]? Body)> SendAsync(CurlOptions options)
        {
            HttpRequestMessage? captured = null;
            byte[]? body = null;
            var mock = new Mock<HttpMessageHandler>();
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns<HttpRequestMessage, CancellationToken>(async (req, _) =>
                {
                    captured = req;
                    body = req.Content == null ? null : await req.Content.ReadAsByteArrayAsync();
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") };
                });

            using var client = new HttpClient(mock.Object);
            var handler = new HttpHandler(client);
            await handler.ExecuteAsync(options, CancellationToken.None);
            return (captured!, body);
        }

        private static string? HeaderValue(HttpRequestMessage request, string name)
        {
#if NET6_0_OR_GREATER
            // The raw value as it goes on the wire (TryGetValues would re-format parsed headers such as Range)
            if (request.Headers.NonValidated.TryGetValues(name, out var raw))
            {
                return string.Join(", ", raw);
            }
#endif
            if (request.Headers.TryGetValues(name, out var values))
            {
                return string.Join(", ", values);
            }

            if (request.Content != null && request.Content.Headers.TryGetValues(name, out var contentValues))
            {
                return string.Join(", ", contentValues);
            }

            return null;
        }

        #region Option table

        [Fact]
        public void UploadFile_ConsumesItsArgument_SoTheUrlIsTheTarget()
        {
            var file = WriteFile("up.txt", "payload");

            var options = _parser.Parse($"curl -T {Q(file)} http://example.com/dir/x");

            options.Url.Should().Be("http://example.com/dir/x");
            options.Method.Should().Be("PUT");
            options.UploadFile.Should().Be(file);
            Encoding.UTF8.GetString(options.BinaryData).Should().Be("payload");
        }

        [Theory]
        [InlineData("--ciphers ALL", "--ciphers")]
        [InlineData("--tcp-nodelay", "--tcp-nodelay")]
        [InlineData("--no-sessionid", "--no-sessionid")]
        [InlineData("-q", "--disable")]
        [InlineData("-j", "--junk-session-cookies")]
        [InlineData("-n", "--netrc")]
        [InlineData("-N", "--no-buffer")]
        [InlineData("-l", "--list-only")]
        [InlineData("-R", "--remote-time")]
        [InlineData("-p", "--proxytunnel")]
        [InlineData("-a", "--append")]
        [InlineData("-B", "--use-ascii")]
        [InlineData("-Z", "--parallel")]
        [InlineData("-1", "--tlsv1")]
        [InlineData("-2", "--sslv2")]
        [InlineData("-3", "--sslv3")]
        [InlineData("-4", "--ipv4")]
        [InlineData("-6", "--ipv6")]
        [InlineData("-P -", "--ftp-port")]
        [InlineData("-t TTYPE=vt100", "--telnet-option")]
        public void RealButUnimplementedOptions_AreRecordedAsUnsupported_AndKeepTheUrl(string option, string expected)
        {
            var options = _parser.Parse($"curl {option} https://example.com/");

            options.Url.Should().Be("https://example.com/");
            options.UnsupportedOptions.Should().Contain(expected);
            options.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void UnknownOptions_StayInUnknownOptions_AndDoNotEatTheUrl()
        {
            var options = _parser.Parse("curl --not-a-curl-option https://example.com");

            options.Url.Should().Be("https://example.com");
            options.UnknownOptions.Should().Contain("--not-a-curl-option");
            options.UnsupportedOptions.Should().BeEmpty();
        }

        [Fact]
        public void ShortAliases_MapToTheirLongOptions()
        {
            var options = _parser.Parse("curl -U proxy:secret -E client.pem -0 -G -g -Y 100 -y 7 -Q NOOP -# -d q=1 https://example.com/");

            options.ProxyCredentials.UserName.Should().Be("proxy");
            options.ProxyCredentials.Password.Should().Be("secret");
            options.CertFile.Should().Be("client.pem");
            options.HttpVersion.Should().Be("1.0");
            options.UseGet.Should().BeTrue();
            options.GlobOff.Should().BeTrue();
            options.SpeedLimit.Should().Be(100);
            options.SpeedTime.Should().Be(7);
            options.Quote.Should().Contain("NOOP");
            options.ProgressBar.Should().BeTrue();
        }

        [Theory]
        [InlineData("-D heads.txt")]
        [InlineData("-K")]
        [InlineData("-J -O")]
        public void ValueAndFlagAliases_DoNotTurnArgumentsIntoUrls(string option)
        {
            var command = option == "-K" ? "curl https://example.com/" : $"curl {option} https://example.com/";

            var options = _parser.Parse(command);

            options.Url.Should().Be("https://example.com/");
        }

        #endregion

        #region Short-option syntax

        [Fact]
        public void CombinedFlags_AreAllApplied()
        {
            var options = _parser.Parse("curl -sSLk https://example.com");

            options.Silent.Should().BeTrue();
            options.ShowError.Should().BeTrue();
            options.FollowLocation.Should().BeTrue();
            options.Insecure.Should().BeTrue();
        }

        [Theory]
        [InlineData("curl -ofile.txt https://example.com", "file.txt")]
        [InlineData("curl -sofile.txt https://example.com", "file.txt")]
        [InlineData("curl --output=file.txt https://example.com", "file.txt")]
        public void AttachedValues_AreTheOptionArgument(string command, string output)
        {
            _parser.Parse(command).OutputFile.Should().Be(output);
        }

        [Fact]
        public void AttachedValues_WorkForNumbersHeadersAndData()
        {
            var options = _parser.Parse("curl -m10 -H\"X-One: 1\" -dfoo=bar -XPATCH https://example.com");

            options.MaxTime.Should().Be(10);
            options.Headers["X-One"].Should().Be("1");
            options.Data.Should().Be("foo=bar");
            options.CustomMethod.Should().Be("PATCH");
        }

        [Fact]
        public void LongOptionWithEquals_KeepsEqualsSignsInTheValue()
        {
            var options = _parser.Parse("curl --data=a=b --header=\"X-Two: 2\" --max-time=2.5 https://example.com");

            options.Data.Should().Be("a=b");
            options.Headers["X-Two"].Should().Be("2");
            options.MaxTime.Should().Be(2.5);
        }

        [Theory]
        [InlineData("curl -d -5 https://example.com", "-5")]
        [InlineData("curl -d '-x=1' https://example.com", "-x=1")]
        [InlineData("curl --data --verbose https://example.com", "--verbose")]
        public void ValueStartingWithDash_IsStillConsumed(string command, string data)
        {
            var options = _parser.Parse(command);

            options.Data.Should().Be(data);
            options.Url.Should().Be("https://example.com");
        }

        [Theory]
        [InlineData("curl --url https://example.com/a")]
        [InlineData("curl --url=https://example.com/a")]
        [InlineData("curl -s -- https://example.com/a")]
        public void UrlOption_AndDoubleDash_GiveTheUrl(string command)
        {
            _parser.Parse(command).Url.Should().Be("https://example.com/a");
        }

        [Fact]
        public void DoubleDash_EndsOptionProcessing()
        {
            var options = _parser.Parse("curl https://example.com/a -- -v");

            options.Verbose.Should().BeFalse();
            options.AdditionalTransfers.Should().ContainSingle();
        }

        [Fact]
        public void Next_StartsAFreshGroupOfOptions()
        {
            var options = _parser.Parse("curl -H \"X-A: 1\" https://a.example/ --next -d x=1 https://b.example/");

            options.Url.Should().Be("https://a.example/");
            options.Headers.Should().ContainKey("X-A");
            options.Method.Should().BeNull();
            options.AdditionalTransfers.Should().ContainSingle();
            var second = options.AdditionalTransfers[0];
            second.Url.Should().Be("https://b.example/");
            second.Method.Should().Be("POST");
            second.Headers.Should().NotContainKey("X-A");
        }

        [Fact]
        public void ShortNext_IsTheSameAsNext()
        {
            var options = _parser.Parse("curl https://a.example/ -: https://b.example/");

            options.AdditionalTransfers.Select(t => t.Url).Should().Equal("https://b.example/");
        }

        [Fact]
        public void ExtraUrls_GetTheOptionsOfTheirGroup()
        {
            var options = _parser.Parse("curl -H \"X-A: 1\" https://a.example/ https://b.example/");

            options.AdditionalTransfers.Should().ContainSingle();
            options.AdditionalTransfers[0].Headers["X-A"].Should().Be("1");
        }

        [Fact]
        public void OutputOptions_ArePairedWithUrlsInOrder()
        {
            var options = _parser.Parse("curl https://a.example/1 -o one.txt https://a.example/2 -o two.txt");

            options.OutputFile.Should().Be("one.txt");
            options.AdditionalTransfers[0].OutputFile.Should().Be("two.txt");
        }

        [Fact]
        public void SurplusOutputOptions_AreIgnoredLikeCurl()
        {
            _parser.Parse("curl -o first.txt -o - https://a.example/").OutputFile.Should().Be("first.txt");
        }

        [Fact]
        public void MissingOptionValue_IsExitCodeTwo()
        {
            var ex = Assert.Throws<CurlException>(() => _parser.Parse("curl https://example.com -H"));

            ex.CurlErrorCode.Should().Be(2);
            ex.Message.Should().Contain("-H").And.Contain("docs/exceptions");
        }

        #endregion

        #region Credentials

        [Fact]
        public void User_SplitsAtTheFirstColon()
        {
            var options = _parser.Parse("curl -u user:pa:ss https://example.com");

            options.Credentials.UserName.Should().Be("user");
            options.Credentials.Password.Should().Be("pa:ss");
        }

        [Fact]
        public void User_WithoutColon_HasEmptyPassword()
        {
            var options = _parser.Parse("curl -u user https://example.com");

            options.Credentials.UserName.Should().Be("user");
            options.Credentials.Password.Should().BeEmpty();
        }

        [Theory]
        [InlineData("--proxy-user p:q:r", "p", "q:r")]
        [InlineData("-U puser:", "puser", "")]
        [InlineData("-U onlyuser", "onlyuser", "")]
        public void ProxyUser_SplitsLikeUser(string option, string user, string password)
        {
            var options = _parser.Parse($"curl -x http://proxy:8080 {option} https://example.com");

            options.ProxyCredentials.UserName.Should().Be(user);
            options.ProxyCredentials.Password.Should().Be(password);
        }

        [Fact]
        public async Task UrlCredentials_AreSentAsBasicAuth_PercentDecoded()
        {
            var options = _parser.Parse("curl http://foo%40bar:secret@example.com/x");

            options.Credentials.UserName.Should().Be("foo@bar");
            var (request, _) = await SendAsync(options);

            request.Headers.Authorization.Scheme.Should().Be("Basic");
            request.Headers.Authorization.Parameter.Should().Be(Convert.ToBase64String(Encoding.UTF8.GetBytes("foo@bar:secret")));
        }

        [Fact]
        public async Task UserOption_WinsOverUrlCredentials()
        {
            var (request, _) = await SendAsync(_parser.Parse("curl -u a:b http://x:y@example.com/"));

            request.Headers.Authorization.Parameter.Should().Be("YTpi");
        }

        #endregion

        #region Request shaping

        [Fact]
        public async Task Get_MovesDataIntoTheQueryString()
        {
            var options = _parser.Parse("curl -G -d a=1 --data-urlencode \"b=x y\" https://example.com/p?q=1");

            options.Url.Should().Be("https://example.com/p?q=1&a=1&b=x+y");
            options.Data.Should().BeNull();
            options.Method.Should().BeNull();

            var (request, body) = await SendAsync(options);
            request.Method.Should().Be(HttpMethod.Get);
            body.Should().BeNull();
        }

        [Fact]
        public void Get_WithHead_StaysHead()
        {
            var options = _parser.Parse("curl -G -I -d a=1 https://example.com/");

            options.Method.Should().Be("HEAD");
            options.Url.Should().Be("https://example.com/?a=1");
        }

        [Theory]
        [InlineData("http://example.com/dir/", "http://example.com/dir/my%20file%5B1%5D.txt")]
        [InlineData("http://example.com", "http://example.com/my%20file%5B1%5D.txt")]
        [InlineData("http://example.com/dir/?query", "http://example.com/dir/?query")]
        [InlineData("http://example.com/dir/name", "http://example.com/dir/name")]
        public void UploadFile_AppendsTheFileNameToUrlsEndingInSlash(string url, string expected)
        {
            var file = WriteFile("my file[1].txt", "x");

            _parser.Parse($"curl -g -T {Q(file)} {url}").Url.Should().Be(expected);
        }

        [Fact]
        public async Task UploadFile_SendsPutWithTheBytes_AndNoContentType()
        {
            var bytes = new byte[] { 0, 1, 2, 0xFF, 13, 10 };
            var file = WriteBytes("blob.bin", bytes);

            var (request, body) = await SendAsync(_parser.Parse($"curl -T {Q(file)} http://example.com/up/"));

            request.Method.Should().Be(HttpMethod.Put);
            request.RequestUri.AbsolutePath.Should().Be("/up/blob.bin");
            body.Should().Equal(bytes);
            request.Content.Headers.ContentType.Should().BeNull();
        }

        [Fact]
        public async Task UploadFile_WithCustomMethod_KeepsTheMethodAndBody()
        {
            var file = WriteFile("b.txt", "test");

            var (request, body) = await SendAsync(_parser.Parse($"curl -X POST -T {Q(file)} http://example.com/x"));

            request.Method.Should().Be(HttpMethod.Post);
            Encoding.ASCII.GetString(body).Should().Be("test");
        }

        [Fact]
        public async Task UploadFile_WithRange_SendsContentRangeInsteadOfRange()
        {
            var file = WriteFile("part.txt", "testdata\n");

            var (request, body) = await SendAsync(_parser.Parse($"curl -T {Q(file)} -r 0-4 http://example.com/r"));

            HeaderValue(request, "Content-Range").Should().Be("bytes 0-4/9");
            request.Headers.Contains("Range").Should().BeFalse();
            body.Should().HaveCount(9);
        }

        [Fact]
        public void UploadFile_Missing_IsReadErrorTwentySix()
        {
            var ex = Assert.Throws<CurlReadErrorException>(() => _parser.Parse("curl -T /no/such/file.bin http://example.com/"));

            ex.CurlErrorCode.Should().Be(26);
            ex.Message.Should().Contain("does not exist").And.Contain("docs/exceptions");
        }

        [Theory]
        [InlineData("-T -")]
        [InlineData("-T .")]
        [InlineData("-d @-")]
        public void StandardInput_IsReportedAsUnsupported(string option)
        {
            var ex = Assert.Throws<CurlReadErrorException>(() => _parser.Parse($"curl {option} -d x=1 http://example.com/"));

            ex.CurlErrorCode.Should().Be(26);
            ex.Message.Should().Contain("standard input");
        }

        [Fact]
        public async Task DataAscii_IsTheSameAsData()
        {
            var (request, body) = await SendAsync(_parser.Parse("curl --data-ascii a=1 -d b=2 http://example.com/"));

            request.Method.Should().Be(HttpMethod.Post);
            Encoding.ASCII.GetString(body).Should().Be("a=1&b=2");
            request.Content.Headers.ContentType.MediaType.Should().Be("application/x-www-form-urlencoded");
        }

        [Fact]
        public async Task DataFile_StripsCarriageReturnsNewlinesAndNul_ButDataBinaryKeepsThem()
        {
            var file = WriteBytes("in.txt", Encoding.ASCII.GetBytes("one\rtwo\nthree\0four\r\n"));

            var (_, dataBody) = await SendAsync(_parser.Parse($"curl -d @{Q(file)} http://example.com/"));
            var (_, binaryBody) = await SendAsync(_parser.Parse($"curl --data-binary @{Q(file)} http://example.com/"));

            Encoding.ASCII.GetString(dataBody).Should().Be("onetwothreefour");
            Encoding.ASCII.GetString(binaryBody).Should().Be("one\rtwo\nthree\0four\r\n");
        }

        [Fact]
        public async Task MixedDataItems_ResolveEachFileInPlace()
        {
            var file = WriteFile("part.txt", "l1\r\nl2\n");

            var options = _parser.Parse($"curl -d x=1 -d @{Q(file)} --data-binary @{Q(file)} http://example.com/");
            var (_, body) = await SendAsync(options);

            Encoding.ASCII.GetString(body).Should().Be("x=1&l1l2&l1\r\nl2\n");
            options.Data.Should().Be("x=1&l1l2&l1\r\nl2\n");
        }

        [Fact]
        public void MixedDataItems_WithAMissingFile_AreReadErrorTwentySix()
        {
            var ex = Assert.Throws<CurlReadErrorException>(() => _parser.Parse("curl -d a=1 -d @/no/such/file http://example.com/"));

            ex.CurlErrorCode.Should().Be(26);
        }

        [Fact]
        public async Task CustomGet_WithData_SendsGetWithABody()
        {
            var options = _parser.Parse("curl -X GET -d body http://example.com/");

            options.CustomMethod.Should().Be("GET");
            var (request, body) = await SendAsync(options);
            request.Method.Should().Be(HttpMethod.Get);
            if (System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework"))
            {
                body.Should().BeNull("the .NET Framework handler cannot send a GET body");
                return;
            }

            Encoding.ASCII.GetString(body!).Should().Be("body");
        }

        [Fact]
        public async Task CustomMethod_KeepsItsCase()
        {
            var (request, _) = await SendAsync(_parser.Parse("curl -X igloo http://example.com/"));

            request.Method.Method.Should().Be("igloo");
        }

        [Fact]
        public async Task EmptyUserAgent_SendsNoUserAgent()
        {
            var options = _parser.Parse("curl -A \"\" http://example.com/");

            options.UserAgent.Should().Be(string.Empty);
            var (request, _) = await SendAsync(options);
            request.Headers.Contains("User-Agent").Should().BeFalse();
        }

        [Fact]
        public async Task HeaderFile_AddsEveryHeaderLine()
        {
            var file = WriteFile("heads.txt", "One: 1\r\nTwo: 2\nno-colon\n\nUser-Agent:\n");

            var options = _parser.Parse($"curl -H @{Q(file)} http://example.com/");
            var (request, _) = await SendAsync(options);

            HeaderValue(request, "One").Should().Be("1");
            HeaderValue(request, "Two").Should().Be("2");
            request.Headers.Contains("User-Agent").Should().BeFalse();
        }

        [Fact]
        public async Task SemicolonHeader_SendsAnEmptyValue_AndColonHeaderSuppresses()
        {
            var options = _parser.Parse("curl -H \"X-Empty;\" -H \"X-Ignored;junk\" -H \"Accept:\" http://example.com/");

            options.EmptyHeaders.Should().Contain("X-Empty");
            var (request, _) = await SendAsync(options);
            request.Headers.Contains("X-Empty").Should().BeTrue();
            HeaderValue(request, "X-Empty").Should().BeEmpty();
            request.Headers.Contains("X-Ignored").Should().BeFalse();
            request.Headers.Contains("Accept").Should().BeFalse();
        }

        [Fact]
        public void DuplicateHeaders_AreJoined_AndTheFirstHostWins()
        {
            var options = _parser.Parse("curl -H \"X-A: 1\" -H \"X-A: 2\" -H \"Host: h1\" -H \"Host: h2\" http://example.com/");

            options.Headers["X-A"].Should().Be("1, 2");
            options.Headers["Host"].Should().Be("h1");
        }

        [Fact]
        public void HeaderValue_KeepsTrailingWhitespace()
        {
            _parser.Parse("curl -H \"X-Test: foo; \" http://example.com/").Headers["X-Test"].Should().Be("foo; ");
        }

        [Theory]
        [InlineData(";auto", null)]
        [InlineData("https://from.example/;auto", "https://from.example/")]
        [InlineData("firstone.html", "firstone.html")]
        public async Task Referer_SupportsTheAutoSuffix(string argument, string expected)
        {
            var options = _parser.Parse($"curl -e {Q(argument)} http://example.com/");

            options.AutoReferer.Should().Be(argument.EndsWith(";auto"));
            options.Referer.Should().Be(expected);
            var (request, _) = await SendAsync(options);
            HeaderValue(request, "Referer").Should().Be(expected);
        }

        [Fact]
        public async Task OAuth2Bearer_SendsABearerToken()
        {
            var (request, _) = await SendAsync(_parser.Parse("curl --oauth2-bearer mF_9.B5f-4.1JqM http://example.com/"));

            HeaderValue(request, "Authorization").Should().Be("Bearer mF_9.B5f-4.1JqM");
        }

        [Theory]
        [InlineData("dec 12 12:00:00 1999 GMT", "Sun, 12 Dec 1999 12:00:00 GMT")]
        [InlineData("Sun, 12 Dec 1999 12:00:00 GMT", "Sun, 12 Dec 1999 12:00:00 GMT")]
        [InlineData("Sunday, 12-Dec-99 12:00:00 GMT", "Sun, 12 Dec 1999 12:00:00 GMT")]
        [InlineData("Sun Dec 12 12:00:00 1999", "Sun, 12 Dec 1999 12:00:00 GMT")]
        [InlineData("19991212", "Sun, 12 Dec 1999 00:00:00 GMT")]
        [InlineData("12 Dec 1999 13:00 +0100", "Sun, 12 Dec 1999 12:00:00 GMT")]
        public async Task TimeCondition_AcceptsCurlDateFormats(string date, string expected)
        {
            var (request, _) = await SendAsync(_parser.Parse($"curl -z {Q(date)} http://example.com/"));

            HeaderValue(request, "If-Modified-Since").Should().Be(expected);
        }

        [Fact]
        public async Task TimeCondition_WithDash_IsIfUnmodifiedSince()
        {
            var options = _parser.Parse("curl -z \"-dec 12 12:00:00 1999 GMT\" http://example.com/");

            options.TimeConditionType.Should().Be(CurlTimeCondition.IfUnmodifiedSince);
            var (request, _) = await SendAsync(options);
            HeaderValue(request, "If-Unmodified-Since").Should().Be("Sun, 12 Dec 1999 12:00:00 GMT");
            request.Headers.Contains("If-Modified-Since").Should().BeFalse();
        }

        [Fact]
        public void TimeCondition_FromAFile_UsesItsModificationTime()
        {
            var file = WriteFile("stamp.txt", "x");
            var stamp = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            File.SetLastWriteTimeUtc(file, stamp);

            var options = _parser.Parse($"curl -z {Q(file)} http://example.com/");

            options.TimeCondition.Should().Be(new DateTimeOffset(stamp));
        }

        [Fact]
        public void TimeCondition_NotADateOrFile_IsDropped()
        {
            _parser.Parse("curl -z not-a-date-or-file http://example.com/").TimeCondition.Should().BeNull();
        }

        [Fact]
        public async Task HttpVersionOptions_SetTheRequestVersion()
        {
            var (http10, _) = await SendAsync(_parser.Parse("curl --http1.0 http://example.com/"));
            var (http11, _) = await SendAsync(_parser.Parse("curl --http1.1 http://example.com/"));
            var (http2, _) = await SendAsync(_parser.Parse("curl --http2-prior-knowledge http://example.com/"));

            http10.Version.Should().Be(new Version(1, 0));
            http11.Version.Should().Be(new Version(1, 1));
            http2.Version.Should().Be(new Version(2, 0));
#if NET5_0_OR_GREATER
            http2.VersionPolicy.Should().Be(HttpVersionPolicy.RequestVersionExact);
#endif
        }

        [Theory]
        [InlineData("0-99,200-299", "bytes=0-99,200-299")]
        [InlineData("-500", "bytes=-500")]
        [InlineData("500-", "bytes=500-")]
        [InlineData("4", "bytes=4-")]
        public async Task Range_IsSentVerbatim(string range, string expected)
        {
            var (request, _) = await SendAsync(_parser.Parse($"curl -r {range} http://example.com/"));

            // .NET Framework re-formats a parsed Range as "0-99, 200-299"; the wire value is what counts on .NET 6+
            HeaderValue(request, "Range")!.Replace(", ", ",").Should().Be(expected);
        }

        [Theory]
        [InlineData("2", 2L)]
        [InlineData("10k", 10240L)]
        [InlineData("1M", 1048576L)]
        public void MaxFileSize_AcceptsSuffixes(string value, long expected)
        {
            _parser.Parse($"curl --max-filesize {value} http://example.com/").MaxFileSize.Should().Be(expected);
        }

        [Fact]
        public void MaxFileSize_Invalid_IsExitCodeTwo()
        {
            Assert.Throws<CurlException>(() => _parser.Parse("curl --max-filesize lots http://example.com/"))
                .CurlErrorCode.Should().Be(2);
        }

        [Theory]
        [InlineData("-I -d x")]
        [InlineData("-d x -T {file}")]
        [InlineData("-F a=b -d x")]
        public void ConflictingRequestMethods_AreExitCodeTwo(string options)
        {
            var file = WriteFile("c.txt", "x");

            var ex = Assert.Throws<CurlException>(() => _parser.Parse($"curl {options.Replace("{file}", Q(file))} http://example.com/"));

            ex.CurlErrorCode.Should().Be(2);
            ex.Message.Should().Contain("only select one HTTP request method");
        }

        [Theory]
        [InlineData("-d x -C 5")]
        [InlineData("-d x -C -")]
        [InlineData("-F a=b -C -")]
        public void ResumingAnUpload_IsExitCodeFortyThree(string options)
        {
            Assert.Throws<CurlException>(() => _parser.Parse($"curl {options} http://example.com/"))
                .CurlErrorCode.Should().Be(43);
        }

        [Fact]
        public void ResumeOffsetZero_IsAllowedWithData()
        {
            _parser.Parse("curl -d x -C 0 http://example.com/").ResumeFrom.Should().Be(0);
        }

        [Fact]
        public void UrlQuery_EncodesLikeCurl()
        {
            var file = WriteFile("q.txt", "a b\n");

            var options = _parser.Parse($"curl --url-query \"my name\" --url-query \"yes=s i r\" --url-query v@{Q(file)} --url-query +%3d http://example.com/p");

            options.Url.Should().Be("http://example.com/p?my+name&yes=s+i+r&v=a+b%0A&%3D");
        }

        [Fact]
        public void Json_ThenHeader_HeaderReplacesTheJsonContentType()
        {
            var options = _parser.Parse("curl --json {} -H \"Content-Type: application/xml\" http://example.com/");

            options.Headers["Content-Type"].Should().Be("application/xml");
        }

#if NET6_0_OR_GREATER
        [Fact]
        public async Task PathAsIs_KeepsDotSegments_AndDropsTheFragment()
        {
            var (request, _) = await SendAsync(_parser.Parse("curl --path-as-is \"http://example.com/../a/./b?x=../y#frag/../z\""));

            request.RequestUri!.OriginalString.Should().Be("http://example.com/../a/./b?x=../y");
        }
#endif

        [Fact]
        public void DataUrlencode_EncodesSpacesAsPlus_LikeCurl8()
        {
            _parser.Parse("curl --data-urlencode \"a=b c~*\" http://example.com/").Data.Should().Be("a=b+c~%2A");
        }

        [Theory]
        [InlineData("X-Spaced;  ")]
        [InlineData("X-Junk;junk")]
        public void SemicolonHeader_MustEndWithTheSemicolon(string header)
        {
            var options = _parser.Parse($"curl -H {Q(header)} http://example.com/");

            options.EmptyHeaders.Should().BeEmpty();
            options.Headers.Should().BeEmpty();
        }

        [Theory]
        [InlineData("http:/example.com/a", "http://example.com/a")]
        [InlineData("https:/example.com/a", "https://example.com/a")]
        public void SingleSlashScheme_IsRepaired(string url, string expected)
        {
            _parser.Parse($"curl {url}").Url.Should().Be(expected);
        }

        [Fact]
        public void ProtoDefault_IsUsedForSchemelessUrls()
        {
            _parser.Parse("curl --proto-default https ftp.example.com/x").Url.Should().Be("https://ftp.example.com/x");
            _parser.Parse("curl ftp.example.com/x").Url.Should().Be("ftp://ftp.example.com/x");
        }

        [Fact]
        public async Task EtagCompare_SendsIfNoneMatch()
        {
            var file = WriteFile("etag.txt", "\"21025-dc7-39462498\"\n");

            var (request, _) = await SendAsync(_parser.Parse($"curl --etag-compare {Q(file)} http://example.com/"));
            var (blank, _) = await SendAsync(_parser.Parse("curl --etag-compare /no/such/etag http://example.com/"));

            HeaderValue(request, "If-None-Match").Should().Be("\"21025-dc7-39462498\"");
            HeaderValue(blank, "If-None-Match").Should().Be("\"\"");
        }

        [Fact]
        public async Task ContinueAtDash_ResumesFromTheExistingOutputFile()
        {
            var output = WriteFile("partial.bin", new string('x', 40));

            var options = _parser.Parse($"curl -C - -o {Q(output)} http://example.com/file");
            var (request, _) = await SendAsync(new CurlOptions { Url = options.Url, Range = options.Range });

            options.ResumeFrom.Should().Be(40);
            HeaderValue(request, "Range").Should().Be("bytes=40-");
        }

        [Theory]
        [InlineData("--no-clobber")]
        [InlineData("--remove-on-error")]
        public void ContinueAt_IsExclusiveWithOutputProtection(string option)
        {
            Assert.Throws<CurlException>(() => _parser.Parse($"curl {option} -C 1 -o out.txt http://example.com/"))
                .CurlErrorCode.Should().Be(2);
        }

        [Fact]
        public async Task PlainHttpProxy_GetsProxyConnectionAndPreemptiveBasic()
        {
            var (request, _) = await SendAsync(_parser.Parse("curl -x 127.0.0.1:3128 -U fake@user:pass --proxy-header \"X-Proxy: 1\" http://example.com/"));

            HeaderValue(request, "Proxy-Connection").Should().Be("Keep-Alive");
            request.Headers.ProxyAuthorization!.Parameter.Should().Be(Convert.ToBase64String(Encoding.UTF8.GetBytes("fake@user:pass")));
            HeaderValue(request, "X-Proxy").Should().Be("1");
        }

        [Fact]
        public async Task ProxyUrlCredentials_AreSentPreemptively()
        {
            var (request, _) = await SendAsync(_parser.Parse("curl -x http://f%61ke:@127.0.0.1:3128 http://example.com/"));

            request.Headers.ProxyAuthorization!.Parameter.Should().Be("ZmFrZTo=");
        }

        [Theory]
        [InlineData("curl -x 127.0.0.1:3128 https://example.com/")]
        [InlineData("curl -x 127.0.0.1:3128 -p http://example.com/")]
        [InlineData("curl -x 127.0.0.1:3128 --noproxy example.com http://www.example.com/")]
        [InlineData("curl -x 127.0.0.1:3128 --noproxy \"*\" http://example.com/")]
        [InlineData("curl -x socks5://127.0.0.1:1080 http://example.com/")]
        public async Task TunnelledDirectOrSocksRequests_GetNoProxyHeaders(string command)
        {
            var (request, _) = await SendAsync(_parser.Parse(command + " -U u:p"));

            request.Headers.Contains("Proxy-Connection").Should().BeFalse();
            request.Headers.Contains("Proxy-Authorization").Should().BeFalse();
        }

        [Fact]
        public async Task ProxyDigest_DoesNotSendPreemptiveBasic()
        {
            var (request, _) = await SendAsync(_parser.Parse("curl -x 127.0.0.1:3128 -U u:p --proxy-digest http://example.com/"));

            request.Headers.Contains("Proxy-Authorization").Should().BeFalse();
            HeaderValue(request, "Proxy-Connection").Should().Be("Keep-Alive");
        }

        #endregion

        #region Shell emulation

        [Fact]
        public void EnvironmentVariables_AreNotExpandedInsideSingleQuotes()
        {
            Environment.SetEnvironmentVariable("CURLDOTNET_PARITY_VAR", "expanded");
            try
            {
                var options = _parser.Parse("curl -H 'X-A: $CURLDOTNET_PARITY_VAR' -H \"X-B: $CURLDOTNET_PARITY_VAR\" -H 'X-C: %CURLDOTNET_PARITY_VAR%' -H \"X-D: ${CURLDOTNET_PARITY_VAR}\" http://example.com/");

                options.Headers["X-A"].Should().Be("$CURLDOTNET_PARITY_VAR");
                options.Headers["X-B"].Should().Be("expanded");
                options.Headers["X-C"].Should().Be("%CURLDOTNET_PARITY_VAR%");
                options.Headers["X-D"].Should().Be("expanded");
            }
            finally
            {
                Environment.SetEnvironmentVariable("CURLDOTNET_PARITY_VAR", null);
            }
        }

        [Fact]
        public void EscapedDollar_StaysLiteral()
        {
            Environment.SetEnvironmentVariable("CURLDOTNET_PARITY_VAR", "expanded");
            try
            {
                _parser.Parse("curl -d \\$CURLDOTNET_PARITY_VAR http://example.com/").Data.Should().Be("$CURLDOTNET_PARITY_VAR");
            }
            finally
            {
                Environment.SetEnvironmentVariable("CURLDOTNET_PARITY_VAR", null);
            }
        }

        [Fact]
        public void PercentEncodedUrls_AreNotTreatedAsCmdVariables()
        {
            _parser.Parse("curl http://example.com/%5cuser%22").Url.Should().Be("http://example.com/%5cuser%22");
        }

        [Fact]
        public void BackslashSpace_OutsideQuotes_IsALiteralSpace()
        {
            _parser.Parse(@"curl -d a\ b https://x/").Data.Should().Be("a b");
        }

        [Theory]
        [InlineData("curl.exe -s https://example.com/")]
        [InlineData("/usr/bin/curl -s https://example.com/")]
        [InlineData("CURL.EXE -s https://example.com/")]
        [InlineData("\"C:\\Program Files\\curl\\curl.exe\" -s https://example.com/")]
        public void ProgramPrefixes_AreAccepted(string command)
        {
            var options = _parser.Parse(command);

            options.Url.Should().Be("https://example.com/");
            options.Silent.Should().BeTrue();
        }

        #endregion

        #region Config files and variables

        [Fact]
        public void ConfigFile_OptionsAreAppliedInPlace()
        {
            var config = WriteFile("curl.cfg",
                "# comment\n" +
                "url = \"http://example.com/from-config\"\n" +
                "header = \"X-Config: yes\"\n" +
                "--data \"a b\\tc\"\n" +
                "-A agent\n" +
                "silent\n" +
                "data-raw unquoted words\n");

            var options = _parser.Parse($"curl -K {Q(config)}");

            options.Url.Should().Be("http://example.com/from-config");
            options.Headers["X-Config"].Should().Be("yes");
            options.Data.Should().Be("a b\tc&unquoted");
            options.UserAgent.Should().Be("agent");
            options.Silent.Should().BeTrue();
        }

        [Fact]
        public void ConfigFile_Missing_IsReadErrorTwentySix()
        {
            Assert.Throws<CurlReadErrorException>(() => _parser.Parse("curl -K /no/such/config http://example.com/"))
                .CurlErrorCode.Should().Be(26);
        }

        [Fact]
        public void Variables_ExpandInExpandOptions()
        {
            var file = WriteFile("v.txt", "  space with space\n");

            var options = _parser.Parse($"curl --variable name=Ishmael --variable what@{Q(file)} --expand-data \"{{{{what:trim:url}}}}&n={{{{name:b64}}}}&{{{{missing}}}}x\" --expand-url \"http://example.com/{{{{name}}}}\"");

            options.Url.Should().Be("http://example.com/Ishmael");
            options.Data.Should().Be("space%20with%20space&n=SXNobWFlbA==&x");
        }

        [Fact]
        public void Variables_ByteRangesAndJson()
        {
            var options = _parser.Parse("curl --variable \"name[2-4]=0123456789\" --variable \"q=a\\\"b\" --expand-data \"{{name}}{{q:json}}\" http://example.com/");

            options.Data.Should().Be("234a\\\"b");
        }

        [Theory]
        [InlineData("--variable what=hello --expand-data \"{{what:super}}\"")]
        [InlineData("--variable what=hello --expand-data \"{{what:trim,url}}\"")]
        [InlineData("--variable %CURLDOTNET_SURELY_NOT_SET")]
        [InlineData("--variable \"name[5-2]=abc\"")]
        public void Variables_Errors_AreExitCodeTwo(string arguments)
        {
            Assert.Throws<CurlException>(() => _parser.Parse($"curl {arguments} http://example.com/"))
                .CurlErrorCode.Should().Be(2);
        }

        [Fact]
        public void Variables_WithoutExpand_AreLiteral()
        {
            _parser.Parse("curl --variable a=1 -d \"{{a}}\" http://example.com/").Data.Should().Be("{{a}}");
        }

        #endregion

        #region Globbing

        [Fact]
        public void Globs_ExpandIntoOneTransferPerUrl()
        {
            var options = _parser.Parse("curl \"http://example.com/{one,two}/[1-2].txt\" -o \"out-#1-#2.txt\"");

            var all = new[] { options }.Concat(options.AdditionalTransfers).ToList();
            all.Select(o => o.Url).Should().Equal(
                "http://example.com/one/1.txt", "http://example.com/one/2.txt",
                "http://example.com/two/1.txt", "http://example.com/two/2.txt");
            all.Select(o => o.OutputFile).Should().Equal("out-one-1.txt", "out-one-2.txt", "out-two-1.txt", "out-two-2.txt");
        }

        [Fact]
        public void Globs_SupportPaddingStepsLettersAndNames()
        {
            var options = _parser.Parse("curl \"http://example.com/[008-012:2]/[a-c:2]/{<n>x,y}\" -o \"#<n>\"");

            var all = new[] { options }.Concat(options.AdditionalTransfers).ToList();
            all.Should().HaveCount(12);
            all[0].Url.Should().Be("http://example.com/008/a/x");
            all[0].OutputFile.Should().Be("x");
            all.Last().Url.Should().Be("http://example.com/012/c/y");
        }

        [Fact]
        public void Globoff_SendsBracketsLiterally()
        {
            var options = _parser.Parse("curl -g \"http://example.com/{a,b}\"");

            options.AdditionalTransfers.Should().BeEmpty();
            options.Url.Should().Be("http://example.com/{a,b}");
        }

        [Fact]
        public void EmptyBrackets_AreNotGlobs()
        {
            var options = _parser.Parse("curl \"http://example.com/we/want/[]/page{}\"");

            options.AdditionalTransfers.Should().BeEmpty();
            options.Url.Should().Be("http://example.com/we/want/[]/page{}");
        }

        [Fact]
        public void Ipv6Literals_AreNotGlobs()
        {
            _parser.Parse("curl http://[::1]:8080/x").Url.Should().Be("http://[::1]:8080/x");
        }

        [Theory]
        [InlineData("http://example.com/{a,b")]
        [InlineData("http://example.com/[1-]")]
        [InlineData("http://example.com/[0-9223372036854775807]")]
        public void BadGlobs_AreExitCodeThree(string url)
        {
            Assert.Throws<CurlException>(() => _parser.Parse($"curl \"{url}\"")).CurlErrorCode.Should().Be(3);
        }

        [Fact]
        public void UnknownNamedGlobInOutput_IsExitCodeFortyThree()
        {
            Assert.Throws<CurlException>(() => _parser.Parse("curl http://example.com/x -o \"#<nope>\"")).CurlErrorCode.Should().Be(43);
        }

        [Fact]
        public void UploadGlobs_ProduceOneUploadPerFile()
        {
            WriteFile("g1.txt", "1");
            WriteFile("g2.txt", "2");
            var pattern = Path.Combine(_dir, "g{1,2}.txt");

            var options = _parser.Parse($"curl -T {Q(pattern)} http://example.com/up/");

            var all = new[] { options }.Concat(options.AdditionalTransfers).ToList();
            all.Select(o => o.Url).Should().Equal("http://example.com/up/g1.txt", "http://example.com/up/g2.txt");
            all.Select(o => Encoding.ASCII.GetString(o.BinaryData)).Should().Equal("1", "2");
        }

        #endregion
    }
}
