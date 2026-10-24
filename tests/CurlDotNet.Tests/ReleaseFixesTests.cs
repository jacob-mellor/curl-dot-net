using System;
using System.Diagnostics;
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
    /// The 9.8.0 release fixes: --retry output and limits, exit codes for bad command lines, -w JSON types,
    /// TLS and speed options, CurlSettings members that are now applied, Clone(), secret redaction,
    /// HttpHandler content types, generated HttpClient code and ConfigureAwait.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class ReleaseFixesTests : IDisposable
    {
        private const string Busy = "HTTP/1.1 503 Busy\r\nRetry-After: 0\r\nContent-Length: 5\r\n\r\nbusy\n";
        private const string Ok = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: 3\r\n\r\nok\n";
        private readonly string _dir;

        public ReleaseFixesTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "curldotnet-release-" + Guid.NewGuid().ToString("N"));
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

        private static T Parses<T>(string command) where T : Exception
        {
            Action act = () => new CommandParser().Parse(command);
            return act.Should().Throw<T>().Which;
        }

        // ------------------------------------------------------------------ A. retries

        [Fact]
        public async Task Retry_WriteOutIsWrittenOnce_ForTheFinalAttempt_WithNumRetries()
        {
            using var server = new RawReplyServer(Busy, Busy, Ok);

            var result = await Curl.ExecuteAsync($"curl --retry 2 -w \"[%{{http_code}} %{{num_retries}}]\" {server.Url("/")}");

            server.Requests.Should().HaveCount(3);
            result.Body.Should().Be("busy\nbusy\nok\n[200 2]", "each attempt's body is kept, the -w text only once");
        }

        [Fact]
        public async Task Retry_WriteOutOnce_WhenTheLastAttemptStillFails()
        {
            using var server = new RawReplyServer(Busy);

            var result = await Curl.ExecuteAsync($"curl --retry 1 -w \"<%{{http_code}}:%{{num_retries}}>\" {server.Url("/")}");

            server.Requests.Should().HaveCount(2);
            result.StatusCode.Should().Be(503);
            result.Body.Should().Be("busy\nbusy\n<503:1>");
        }

        [Fact]
        public async Task Retry_FailedAttempts_WriteOutOnceOnTheException()
        {
            using var server = new RawReplyServer(Busy);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -f --retry 2 -w \"(%{{exitcode}} %{{num_retries}})\" {server.Url("/")}");

            var ex = (await act.Should().ThrowAsync<CurlHttpReturnedErrorException>()).Which;
            server.Requests.Should().HaveCount(3);
            ex.StandardOutput.Should().Be("(22 2)");
            ex.Data[CurlException.WriteOutDataKey].Should().Be("(22 2)");
        }

        [Fact]
        public async Task NumRetries_IsZeroWithoutRetry()
        {
            using var server = new RawReplyServer(Ok);

            var result = await Curl.ExecuteAsync($"curl -w \"%{{num_retries}}\" {server.Url("/")}");

            result.Body.Should().Be("ok\n0");
        }

        [Fact]
        public async Task RetryMaxTime_RunsOut_TheLastAttemptsErrorIsThrown_NotATimeout()
        {
            using var server = new RawReplyServer(Busy);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -f --retry 5 --retry-delay 3 --retry-max-time 1 {server.Url("/")}");

            var ex = (await act.Should().ThrowAsync<CurlException>()).Which;
            ex.Should().BeOfType<CurlHttpReturnedErrorException>("the outcome of the last attempt stands, like curl");
            ex.CurlErrorCode.Should().Be(22);
            server.Requests.Should().HaveCount(1, "the next wait (3 s) would pass --retry-max-time 1");
        }

        [Fact]
        public async Task RetryMaxTime_RunsOut_WithAResponse_ReturnsItWithItsWriteOut()
        {
            using var server = new RawReplyServer(Busy);

            var result = await Curl.ExecuteAsync($"curl --retry 5 --retry-delay 3 --retry-max-time 1 -w \"|%{{http_code}}\" {server.Url("/")}");

            server.Requests.Should().HaveCount(1);
            result.Body.Should().Be("busy\n|503");
        }

        [Fact]
        public async Task RetryDelay_WinsOverRetryAfter()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 503 Busy\r\nRetry-After: 120\r\nContent-Length: 0\r\n\r\n",
                Ok);

            var clock = Stopwatch.StartNew();
            var result = await Curl.ExecuteAsync($"curl --retry 1 --retry-delay 1 {server.Url("/")}");

            clock.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(20), "curl ignores Retry-After when --retry-delay is given");
            clock.Elapsed.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(900));
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task RetryAfter_IsHonouredWithoutRetryDelay()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 429 Slow down\r\nRetry-After: 0\r\nContent-Length: 0\r\n\r\n",
                Ok);

            var clock = Stopwatch.StartNew();
            var result = await Curl.ExecuteAsync($"curl --retry 1 {server.Url("/")}");

            clock.Elapsed.Should().BeLessThan(TimeSpan.FromMilliseconds(900), "Retry-After: 0 replaces the 1 s backoff");
            result.StatusCode.Should().Be(200);
        }

        // ------------------------------------------------------------------ B. exit codes

        [Theory]
        [InlineData("curl -s")]
        [InlineData("curl -s -H 'Accept: text/plain'")]
        [InlineData("curl")]
        public void NoUrl_IsExitCodeTwo(string command)
        {
            var ex = Parses<CurlException>(command);
            ex.CurlErrorCode.Should().Be(2, "curl exits with 2 for \"no URL specified\"");
            ex.WhatHappened.Should().StartWith("No URL specified");
            ExceptionMessageTests.AssertWhatFixLink(ex);
        }

        [Fact]
        public async Task NoUrl_FromOptions_IsExitCodeTwo()
        {
            Func<Task> act = () => Curl.ExecuteAsync(new CurlOptions { Url = string.Empty });

            (await act.Should().ThrowAsync<CurlException>()).Which.CurlErrorCode.Should().Be(2);
        }

        [Theory]
        [InlineData("-m abc", "-m/--max-time")]
        [InlineData("--max-time 1,5", "-m/--max-time")]
        [InlineData("-m -3", "-m/--max-time")]
        [InlineData("--connect-timeout soon", "--connect-timeout")]
        [InlineData("--max-redirs many", "--max-redirs")]
        [InlineData("--max-redirs -2", "--max-redirs")]
        [InlineData("--retry three", "--retry")]
        [InlineData("--retry -1", "--retry")]
        [InlineData("--retry-delay x", "--retry-delay")]
        [InlineData("--retry-max-time 1m", "--retry-max-time")]
        [InlineData("--speed-limit fast", "-Y/--speed-limit")]
        [InlineData("--speed-time 1.5", "-y/--speed-time")]
        [InlineData("--limit-rate quick", "--limit-rate")]
        public void InvalidNumericValues_AreExitCodeTwo_NamingTheOption(string option, string named)
        {
            var ex = Parses<CurlException>($"curl {option} https://example.com/");

            ex.CurlErrorCode.Should().Be(2);
            ex.WhatHappened.Should().Contain("option " + named);
            ex.Suggestions.Should().Contain(s => s.Contains(named) && s.Contains("for example"));
            ExceptionMessageTests.AssertWhatFixLink(ex);
        }

        [Fact]
        public void ValidNumericValues_StillParse()
        {
            var o = new CommandParser().Parse("curl -m 2.5 --connect-timeout 3 --max-redirs -1 --retry 4 --retry-delay 0.5 --retry-max-time 1.2 " +
                                              "--limit-rate 100k -Y 500 -y 7 https://example.com/");

            o.MaxTime.Should().Be(2.5);
            o.ConnectTimeout.Should().Be(3);
            o.MaxRedirects.Should().Be(-1);
            o.Retry.Should().Be(4);
            o.RetryDelay.Should().Be(0.5);
            o.RetryMaxTime.Should().Be(2, "a fraction rounds up");
            o.LimitRate.Should().Be(100 * 1024);
            o.SpeedLimit.Should().Be(500);
            o.SpeedTime.Should().Be(7);
        }

        [Theory]
        [InlineData(":amz:us-east-1:s3")]
        [InlineData("aws:amz:us-east-1:s3:extra")]
        [InlineData("aws amz")]
        public void MalformedAwsSigV4_IsCurlExceptionTwo(string value)
        {
            var ex = Parses<CurlException>($"curl --aws-sigv4 '{value}' -u key:secret https://s3.amazonaws.com/");

            ex.Should().BeOfType<CurlInvalidCommandException>();
            ex.CurlErrorCode.Should().Be(2);
            ((CurlInvalidCommandException)ex).InvalidPart.Should().Be("--aws-sigv4");
            ex.Message.Should().Contain("aws:amz:us-east-1:s3").And.NotContain("secret");
            ExceptionMessageTests.AssertWhatFixLink(ex);
        }

        [Fact]
        public void AwsSigV4ConfigParse_Empty_IsCurlExceptionTwo()
        {
            Action act = () => AwsSigV4Config.Parse(" ");

            act.Should().Throw<CurlInvalidCommandException>().Which.CurlErrorCode.Should().Be(2);
        }

        [Fact]
        public void HeaderFromMissingFile_IsReadError26()
        {
            var ex = Parses<CurlReadErrorException>($"curl -H @{Q(P("no-such-headers.txt"))} https://example.com/");

            ex.CurlErrorCode.Should().Be(26);
            ex.FilePath.Should().EndWith("no-such-headers.txt");
            ExceptionMessageTests.AssertWhatFixLink(ex);
        }

        [Fact]
        public void HeaderFromExistingFile_StillReadsEveryLine()
        {
            File.WriteAllText(P("h.txt"), "X-One: 1\nX-Two: 2\n");

            var o = new CommandParser().Parse($"curl -H @{Q(P("h.txt"))} https://example.com/");

            o.Headers["X-One"].Should().Be("1");
            o.Headers["X-Two"].Should().Be("2");
        }

        [Fact]
        public async Task UnsupportedProtocol_ProtocolIsTheScheme()
        {
            Func<Task> act = () => Curl.ExecuteAsync("curl gopher://example.com/");

            (await act.Should().ThrowAsync<CurlUnsupportedProtocolException>()).Which.Protocol.Should().Be("gopher");
        }

        [Fact]
        public async Task UnsupportedProtocol_FromABadStatusLine_ProtocolIsTheVersionToken()
        {
            using var server = new RawReplyServer("HTTP/7.3 200 OK\r\nContent-Length: 2\r\n\r\nok");

            Func<Task> act = () => Curl.ExecuteAsync($"curl {server.Url("/")}");

            var thrown = await act.Should().ThrowAsync<CurlUnsupportedProtocolException>();
            thrown.Which.Protocol.Should().Be("HTTP/7.3", "the property holds the protocol, not the status line or message text");
            thrown.Which.CurlErrorCode.Should().Be(1);
        }

        [Fact]
        public async Task MalformedUrl_MalformedUrlIsTheUrl()
        {
            Func<Task> act = () => Curl.ExecuteAsync(new CurlOptions { Url = "http://exa mple.com:99999/" });

            var ex = (await act.Should().ThrowAsync<CurlMalformedUrlException>()).Which;
            ex.MalformedUrl.Should().Be("http://exa mple.com:99999/");
            ex.CurlErrorCode.Should().Be(3);
        }

        // ------------------------------------------------------------------ C. -w %{json}

        [Fact]
        public async Task WriteOutJson_NumbersAreJsonNumbers_TextIsStringsOrNull()
        {
            using var server = new RawReplyServer(Ok);

            var result = await Curl.ExecuteAsync($"curl -s -o {Q(P("out.txt"))} -w \"%{{json}}\" {server.Url("/j")}");

            using var doc = System.Text.Json.JsonDocument.Parse(result.Body);
            var root = doc.RootElement;
            foreach (var number in new[] { "http_code", "response_code", "http_connect", "exitcode", "num_connects", "num_headers",
                         "num_redirects", "num_retries", "size_download", "size_header", "size_request", "size_upload",
                         "speed_download", "speed_upload", "time_total", "time_connect", "time_namelookup", "urlnum",
                         "local_port", "remote_port", "proxy_used", "ssl_verify_result", "num_certs" })
            {
                root.GetProperty(number).ValueKind.Should().Be(System.Text.Json.JsonValueKind.Number, $"curl writes {number} as a JSON number");
            }
            root.GetProperty("http_code").GetInt32().Should().Be(200);
            root.GetProperty("response_code").GetInt32().Should().Be(200);
            root.GetProperty("size_download").GetInt64().Should().Be(3);
            root.GetProperty("http_version").GetString().Should().Be("1.1");
            root.GetProperty("method").GetString().Should().Be("GET");
            root.GetProperty("content_type").GetString().Should().Be("text/plain");
            root.GetProperty("errormsg").ValueKind.Should().Be(System.Text.Json.JsonValueKind.Null, "a transfer that worked has no error message");
            root.GetProperty("redirect_url").ValueKind.Should().Be(System.Text.Json.JsonValueKind.Null);
            root.GetProperty("url.path").GetString().Should().Be("/j");
            root.GetProperty("certs").GetString().Should().BeEmpty();
            result.Body.Should().Contain("\"http_code\":200,\"http_connect\":0,\"http_version\":\"1.1\"");
        }

        [Fact]
        public async Task WriteOutJson_ForAFailedTransfer_HasExitCodeAndMessage()
        {
            using var server = new RawReplyServer("HTTP/1.1 404 Not Found\r\nContent-Length: 0\r\n\r\n");

            Func<Task> act = () => Curl.ExecuteAsync($"curl -f -w \"%{{json}}\" {server.Url("/missing")}");

            var ex = (await act.Should().ThrowAsync<CurlHttpReturnedErrorException>()).Which;
            using var doc = System.Text.Json.JsonDocument.Parse((string)ex.Data[CurlException.WriteOutDataKey]!);
            doc.RootElement.GetProperty("http_code").GetInt32().Should().Be(404);
            doc.RootElement.GetProperty("exitcode").GetInt32().Should().Be(22);
            doc.RootElement.GetProperty("errormsg").GetString().Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task WriteOutHeaderJson_IsValidJson_WithLowerCaseNamesAndStringArrays()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nX-Count: 2\r\nSet-Cookie: a=1\r\nSet-Cookie: b=2\r\nContent-Length: 0\r\n\r\n");

            var result = await Curl.ExecuteAsync($"curl -s -w \"%{{header_json}}\" {server.Url("/")}");

            using var doc = System.Text.Json.JsonDocument.Parse(result.Body);
            doc.RootElement.GetProperty("x-count")[0].GetString().Should().Be("2", "header values stay strings, as in curl");
            doc.RootElement.GetProperty("set-cookie").GetArrayLength().Should().Be(2);
        }
    }
}
