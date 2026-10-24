using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet.Exceptions;
using CurlDotNet.Extensions;
using FluentAssertions;
using Xunit;
using CoreHttpException = CurlDotNet.Core.CurlHttpException;
using CurlResult = CurlDotNet.Core.CurlResult;
using CurlSettings = CurlDotNet.Core.CurlSettings;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Messages thrown by CurlResult, CurlSettings and the extension helpers: each says what happened (with the URL,
    /// status, path or value), how to fix it, and links the documentation - and never prints credentials.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class DocsResultSettingsTests : IDisposable
    {
        private const string Link = "See https://github.com/jacob-mellor/curl-dot-net/";
        private readonly string _dir;

        public DocsResultSettingsTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "curldotnet-docs-" + Guid.NewGuid().ToString("N"));
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

        private static CurlResult Result(int status, string body, string command = "curl -u admin:hunter2 https://api.example.com/v1/users?token=abc") =>
            new CurlResult { StatusCode = status, Body = body, Command = command };

        [Fact]
        public void SaveToFile_MissingFolder_IsWriteErrorNamingThePathAndTheFix()
        {
            var target = Path.Combine(_dir, "no-such-folder", "out.json");

            Action act = () => Result(200, "{}").SaveToFile(target);

            var ex = act.Should().Throw<CurlWriteErrorException>().Which;
            ex.CurlErrorCode.Should().Be(23);
            ex.FilePath.Should().Be(target);
            ex.Message.Should().Contain("no-such-folder").And.Contain("does not exist").And.Contain(Link);
            ex.Message.Should().NotContain("hunter2").And.NotContain("token=abc");
            ex.Suggestions.Should().NotBeEmpty();
        }

        [Fact]
        public async Task SaveToFileAsync_MissingFolder_IsWriteError()
        {
            var target = Path.Combine(_dir, "missing", "out.bin");

            Func<Task> act = () => new CurlResult { StatusCode = 200, BinaryData = new byte[] { 1, 2, 3 } }.SaveToFileAsync(target);

            (await act.Should().ThrowAsync<CurlWriteErrorException>()).Which.Message.Should().Contain("3 bytes").And.Contain(Link);
        }

        [Fact]
        public void SaveToFile_EmptyPath_IsArgumentExceptionWithFix()
        {
            Action act = () => Result(200, "x").SaveToFile("  ");

            act.Should().Throw<ArgumentException>().WithMessage("*needs a file path*" + Link + "*");
        }

        [Theory]
        [InlineData(401, "credentials")]
        [InlineData(403, "permissions")]
        [InlineData(404, "URL path")]
        [InlineData(503, "Retry later")]
        public void EnsureSuccess_GivesAStatusSpecificFix(int status, string fix)
        {
            Action act = () => Result(status, "{\"error\":\"nope\"}").EnsureSuccess();

            var ex = act.Should().Throw<CoreHttpException>().Which;
            ex.StatusCode.Should().Be(status);
            ex.Message.Should().Contain($"HTTP {status}").And.Contain("https://api.example.com/v1/users").And.Contain(fix).And.Contain(Link);
            ex.Message.Should().NotContain("hunter2").And.NotContain("token=abc");
        }

        [Fact]
        public void EnsureSuccess_Redirect_SuggestsDashL()
        {
            var result = Result(301, "");
            result.Headers["Location"] = "https://api.example.com/v2/users";

            Action act = () => result.EnsureSuccess();

            act.Should().Throw<CoreHttpException>().Which.Message.Should().Contain("-L").And.Contain("/v2/users");
        }

        [Fact]
        public void EnsureStatus_SuccessButDifferent_SaysSo()
        {
            Action act = () => Result(200, "ok").EnsureStatus(201);

            act.Should().Throw<CoreHttpException>().Which.Message.Should().Contain("EnsureStatus(201)").And.Contain("200").And.Contain("EnsureSuccess()");
        }

        [Fact]
        public void EnsureContains_DifferentCase_SaysTheCheckIsCaseSensitive()
        {
            Action act = () => Result(200, "Status: OK").EnsureContains("ok");

            act.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("case-sensitive").And.Contain(Link);
        }

        [Fact]
        public void ParseJson_ErrorPage_PointsAtTheStatus()
        {
            Action act = () => Result(404, "<html>Not Found</html>").ParseJson<Dictionary<string, string>>();

            var ex = act.Should().Throw<CurlParsingException>().Which;
            ex.InnerException.Should().NotBeNull();
            ex.Message.Should().Contain("HTTP 404").And.Contain("EnsureSuccess").And.Contain("<html>Not Found").And.Contain(Link);
        }

        [Fact]
        public void ParseJson_BodyWrittenToFile_SaysWhereItWent()
        {
            var result = Result(200, "", "curl -o data.json https://api.example.com/data");
            result.OutputFiles.Add("data.json");

            Action act = () => result.ParseJson<object>();

            act.Should().Throw<CurlParsingException>().Which.Message.Should().Contain("'data.json'").And.Contain("-o");
        }

        [Fact]
        public void ParseJsonWithOptions_NonSuccess_IsParsingExceptionWithStatusFix()
        {
            Action act = () => Result(401, "{\"error\":\"bad token\"}").ParseJson<Dictionary<string, string>>(new System.Text.Json.JsonSerializerOptions());

            act.Should().Throw<CurlParsingException>().Which.Message.Should().Contain("HTTP 401").And.Contain("credentials").And.Contain(Link);
        }

        [Fact]
        public void ParseJsonWithOptions_JsonNull_ExplainsNullLiteral()
        {
            Action act = () => Result(200, "null").ParseJson<Dictionary<string, string>>(new System.Text.Json.JsonSerializerOptions());

            act.Should().Throw<CurlParsingException>().Which.Message.Should().Contain("literal null").And.Contain("TryParseJson");
        }

        [Fact]
        public void EnsureSuccessStatusCode_CarriesHeadersAndFix()
        {
            var result = Result(429, "slow down");
            result.Headers["Retry-After"] = "30";

            Action act = () => result.EnsureSuccessStatusCode();

            var ex = act.Should().Throw<CurlDotNet.Exceptions.CurlHttpException>().Which;
            ex.ResponseHeaders["Retry-After"].Should().Be("30");
            ex.GetRetryAfter().Should().Be(TimeSpan.FromSeconds(30));
            ex.Message.Should().Contain("30 second(s)").And.Contain(Link);
            CountOf(ex.Message, "See https://").Should().Be(1);
        }

        [Fact]
        public async Task Retry_WithoutCommand_SaysHowToFix()
        {
            Func<Task> act = () => new CurlResult { StatusCode = 500 }.Retry();

            (await act.Should().ThrowAsync<InvalidOperationException>()).Which.Message.Should().Contain("Curl.ExecuteAsync(command)").And.Contain(Link);
        }

        [Fact]
        public async Task RetryWith_NullCallback_IsArgumentNullException()
        {
            Func<Task> act = () => Result(500, "").RetryWith(null);

            (await act.Should().ThrowAsync<ArgumentNullException>()).Which.ParamName.Should().Be("configure");
        }

        [Fact]
        public void HasHeader_IsCaseInsensitiveAndNullSafe()
        {
            var result = new CurlResult { Headers = new Dictionary<string, string> { ["X-Thing"] = "1" } };
            result.HasHeader("x-thing").Should().BeTrue();
            result.HasHeader(null).Should().BeFalse();
        }

        [Fact]
        public void CurlSettings_WithProxyInvalidUrl_IsMalformedUrlWithExample()
        {
            Action act = () => new CurlSettings().WithProxy("http://bad host:abc");

            var ex = act.Should().Throw<CurlMalformedUrlException>().Which;
            ex.CurlErrorCode.Should().Be(3);
            ex.Message.Should().Contain("http://proxy.local:3128").And.Contain(Link);
        }

        [Fact]
        public void CurlSettings_WithHeadersNull_IsArgumentNullExceptionWithFix()
        {
            Action act = () => new CurlSettings().WithHeaders(null);

            act.Should().Throw<ArgumentNullException>().WithMessage("*WithHeader(name, value)*");
        }

        [Fact]
        public void CurlApiClient_NullBaseUrl_IsArgumentNullException()
        {
            Action act = () => new CurlApiClient(null);

            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("baseUrl");
        }

        [Fact]
        public async Task CurlGetAsync_NullUrl_IsArgumentNullException()
        {
            Func<Task> act = () => ((string)null).CurlGetAsync();

            (await act.Should().ThrowAsync<ArgumentNullException>()).Which.Message.Should().Contain(Link);
        }

        private static int CountOf(string text, string part)
        {
            var count = 0;
            for (var i = text.IndexOf(part, StringComparison.Ordinal); i >= 0; i = text.IndexOf(part, i + part.Length, StringComparison.Ordinal))
            {
                count++;
            }
            return count;
        }
    }
}
