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

            var result = await Curl.ExecuteAsync($"curl --retry 2 --retry-delay 0.1 -w \"[%{{http_code}} %{{num_retries}}]\" {server.Url("/")}");

            server.Requests.Should().HaveCount(3);
            result.Body.Should().Be("busy\nbusy\nok\n[200 2]", "each attempt's body is kept, the -w text only once");
        }

        [Fact]
        public async Task Retry_WriteOutOnce_WhenTheLastAttemptStillFails()
        {
            using var server = new RawReplyServer(Busy);

            var result = await Curl.ExecuteAsync($"curl --retry 1 --retry-delay 0.1 -w \"<%{{http_code}}:%{{num_retries}}>\" {server.Url("/")}");

            server.Requests.Should().HaveCount(2);
            result.StatusCode.Should().Be(503);
            result.Body.Should().Be("busy\nbusy\n<503:1>");
        }

        [Fact]
        public async Task Retry_FailedAttempts_WriteOutOnceOnTheException()
        {
            using var server = new RawReplyServer(Busy);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -f --retry 2 --retry-delay 0.1 -w \"(%{{exitcode}} %{{num_retries}})\" {server.Url("/")}");

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

            Func<Task> act = () => Curl.ExecuteAsync($"curl -f --retry 5 --retry-delay 1 --retry-max-time 1 {server.Url("/")}");

            var ex = (await act.Should().ThrowAsync<CurlException>()).Which;
            ex.Should().BeOfType<CurlHttpReturnedErrorException>("the outcome of the last attempt stands, like curl");
            ex.CurlErrorCode.Should().Be(22);
            server.Requests.Should().HaveCount(2, "after the 1 s wait, --retry-max-time 1 has passed, so curl stops after the second attempt");
        }

        [Fact]
        public async Task RetryMaxTime_RunsOut_WithAResponse_ReturnsItWithItsWriteOut()
        {
            using var server = new RawReplyServer(Busy);

            var result = await Curl.ExecuteAsync($"curl --retry 5 --retry-delay 1 --retry-max-time 1 -w \"|%{{http_code}}\" {server.Url("/")}");

            server.Requests.Should().HaveCount(2);
            result.Body.Should().Be("busy\nbusy\n|503");
        }

        [Fact]
        public async Task RetryMaxTime_ARetryAfterBeyondTheLimit_StopsRightAway()
        {
            using var server = new RawReplyServer("HTTP/1.1 503 Busy\r\nRetry-After: 30\r\nContent-Length: 0\r\n\r\n", Ok);

            var clock = Stopwatch.StartNew();
            var result = await Curl.ExecuteAsync($"curl --retry 3 --retry-max-time 5 {server.Url("/")}");

            server.Requests.Should().HaveCount(1, "waiting 30 s would exceed --retry-max-time 5");
            result.StatusCode.Should().Be(503);
            clock.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task Retry_AHostThatDoesNotResolve_IsTransient_LikeCurl()
        {
            Func<Task> act = () => Curl.ExecuteAsync("curl --retry 1 --retry-delay 0.1 -w \"%{num_retries}\" http://no-such-host.invalid/");

            var ex = (await act.Should().ThrowAsync<CurlCouldntResolveHostException>()).Which;
            ex.Data[CurlException.WriteOutDataKey].Should().Be("1", "curl retries exit 6 without --retry-all-errors");
        }

        [Fact]
        public async Task RetryDelay_WinsOverRetryAfter()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 503 Busy\r\nRetry-After: 120\r\nContent-Length: 0\r\n\r\n",
                Ok);

            var clock = Stopwatch.StartNew();
            var result = await Curl.ExecuteAsync($"curl --retry 1 --retry-delay 1 {server.Url("/")}");

            clock.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(20), "an explicit --retry-delay is not overridden by the server's Retry-After");
            clock.Elapsed.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(900));
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task RetryAfter_LongerThanTheBackoff_IsHonouredWithoutRetryDelay()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 429 Slow down\r\nRetry-After: 2\r\nContent-Length: 0\r\n\r\n",
                Ok);

            var clock = Stopwatch.StartNew();
            var result = await Curl.ExecuteAsync($"curl --retry 1 {server.Url("/")}");

            clock.Elapsed.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(1900), "Retry-After: 2 is longer than the 1 s backoff");
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task RetryAfterZero_KeepsTheBackoff()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 503 Busy\r\nRetry-After: 0\r\nContent-Length: 0\r\n\r\n",
                Ok);

            var clock = Stopwatch.StartNew();
            await Curl.ExecuteAsync($"curl --retry 1 {server.Url("/")}");

            clock.Elapsed.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(900), "Retry-After: 0 is no hint, so curl's 1 s backoff applies");
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

        // ------------------------------------------------------------------ speed options

        [Fact]
        public async Task SpeedLimit_AbortsAStalledBody_WithExit28()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 1000\r\n\r\nstart") { StallMilliseconds = 8000 };

            var clock = Stopwatch.StartNew();
            Func<Task> act = () => Curl.ExecuteAsync($"curl --speed-limit 100 --speed-time 1 {server.Url("/slow")}");

            var ex = (await act.Should().ThrowAsync<CurlOperationTimeoutException>()).Which;
            clock.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(6));
            ex.CurlErrorCode.Should().Be(28);
            ex.WhatHappened.Should().StartWith("Operation too slow. Less than 100 bytes/sec transferred the last 1 seconds");
            ExceptionMessageTests.AssertWhatFixLink(ex);
        }

        [Fact]
        public async Task SpeedLimit_DoesNotAbortAFastTransfer()
        {
            using var server = new RawReplyServer(Ok);

            var result = await Curl.ExecuteAsync($"curl -Y 1 -y 5 {server.Url("/")}");

            result.Body.Should().Be("ok\n");
        }

        [Fact]
        public async Task LimitRate_SlowsTheBodyDownToTheCap()
        {
            var body = new string('x', 3000);
            using var server = new RawReplyServer($"HTTP/1.1 200 OK\r\nContent-Length: {body.Length}\r\n\r\n{body}");

            var clock = Stopwatch.StartNew();
            var result = await Curl.ExecuteAsync($"curl --limit-rate 2000 {server.Url("/")}");

            result.Body.Should().Be(body);
            clock.Elapsed.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(1300), "3000 bytes at 2000 bytes/s take about 1.5 s");
        }

        // ------------------------------------------------------------------ E. CurlSettings members are applied

        [Fact]
        public async Task Settings_Proxy_SendsThroughTheProxyWithBasicCredentials()
        {
            using var proxy = new RawReplyServer(Ok);
            var settings = new CurlSettings().WithProxy(proxy.Url(), "alice", "pr0xyPw");

            var result = await Curl.ExecuteAsync("curl http://origin.example/path", settings);

            result.Body.Should().Be("ok\n");
            var request = proxy.Requests.Single();
            request.Should().StartWith("GET http://origin.example/path HTTP/1.1");
            Header(request, "Proxy-Authorization").Should().Be("Proxy-Authorization: Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("alice:pr0xyPw")));
        }

        [Fact]
        public async Task Settings_Proxy_BypassedUrl_GoesDirect()
        {
            using var origin = new RawReplyServer(Ok);
            var bypassing = new System.Net.WebProxy("http://127.0.0.1:9") { BypassProxyOnLocal = false };
            bypassing.BypassList = new[] { "127\\.0\\.0\\.1" };

            var result = await Curl.ExecuteAsync($"curl {origin.Url("/direct")}", new CurlSettings { Proxy = bypassing });

            result.Body.Should().Be("ok\n");
            origin.Requests.Single().Should().StartWith("GET /direct HTTP/1.1");
        }

        [Fact]
        public async Task Settings_UserAgent_ReplacesTheCommandsUserAgent()
        {
            using var server = new RawReplyServer(Ok);

            await Curl.ExecuteAsync($"curl -A old/1 {server.Url("/")}", new CurlSettings().WithUserAgent("MyApp/2.0"));
            await Curl.ExecuteAsync($"curl -H \"User-Agent: old/1\" {server.Url("/")}", new CurlSettings().WithUserAgent("MyApp/2.0"));

            Header(server.Requests[0], "User-Agent").Should().Be("User-Agent: MyApp/2.0");
            Header(server.Requests[1], "User-Agent").Should().Be("User-Agent: MyApp/2.0");
        }

        [Fact]
        public async Task Settings_Cookies_AreStoredAndSentOnTheNextCall()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 200 OK\r\nSet-Cookie: sid=abc123; Path=/\r\nContent-Length: 3\r\n\r\nin\n",
                Ok);
            var jar = new System.Net.CookieContainer();
            var settings = new CurlSettings().WithCookies(jar);

            await Curl.ExecuteAsync($"curl {server.Url("/login")}", settings);
            await Curl.ExecuteAsync($"curl -b theme=dark {server.Url("/account")}", settings);

            jar.GetCookieHeader(new Uri(server.Url("/"))).Should().Be("sid=abc123");
            Header(server.Requests[0], "Cookie").Should().BeNull();
            Header(server.Requests[1], "Cookie").Should().Be("Cookie: theme=dark; sid=abc123");
        }

        [Fact]
        public async Task Settings_OnRedirect_IsCalledForEveryFollowedHop()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 302 Found\r\nLocation: /two\r\nContent-Length: 0\r\n\r\n",
                "HTTP/1.1 301 Moved\r\nLocation: three\r\nContent-Length: 0\r\n\r\n",
                Ok);
            var hops = new System.Collections.Generic.List<string>();

            var result = await Curl.ExecuteAsync($"curl -L {server.Url("/one")}", new CurlSettings().WithRedirectHandler(hops.Add));

            result.Body.Should().Be("ok\n");
            hops.Should().Equal(server.Url("/two"), server.Url("/three"));
        }

        [Fact]
        public async Task Settings_OnRedirect_NotCalledWithoutL_AndAThrowAbortsWith42()
        {
            using var server = new RawReplyServer("HTTP/1.1 302 Found\r\nLocation: /two\r\nContent-Length: 0\r\n\r\n", Ok);
            var hops = new System.Collections.Generic.List<string>();

            await Curl.ExecuteAsync($"curl {server.Url("/one")}", new CurlSettings().WithRedirectHandler(hops.Add));
            hops.Should().BeEmpty("without -L nothing is followed");

            using var again = new RawReplyServer("HTTP/1.1 302 Found\r\nLocation: /two\r\nContent-Length: 0\r\n\r\n", Ok);
            Func<Task> act = () => Curl.ExecuteAsync($"curl -L {again.Url("/one")}",
                new CurlSettings().WithRedirectHandler(_ => throw new InvalidOperationException("no thanks")));
            var ex = (await act.Should().ThrowAsync<CurlAbortedByCallbackException>()).Which;
            ex.CurlErrorCode.Should().Be(42);
            ex.InnerException.Should().BeOfType<InvalidOperationException>();
            ExceptionMessageTests.AssertWhatFixLink(ex);
        }

        [Fact]
        public async Task Settings_AutomaticDecompression_AppliesOnlyWhenSet()
        {
            using var server = new RawReplyServer(Ok);

            await Curl.ExecuteAsync($"curl {server.Url("/")}", new CurlSettings());
            await Curl.ExecuteAsync($"curl {server.Url("/")}", new CurlSettings().WithAutoDecompression());
            await Curl.ExecuteAsync($"curl --compressed {server.Url("/")}", new CurlSettings { AutomaticDecompression = false });

            Header(server.Requests[0], "Accept-Encoding").Should().BeNull("an untouched setting leaves the command in charge");
            Header(server.Requests[1], "Accept-Encoding").Should().StartWith("Accept-Encoding: gzip, deflate");
            Header(server.Requests[2], "Accept-Encoding").Should().BeNull("false turns --compressed off");
        }

        [Fact]
        public async Task Settings_BufferSize_SetsTheReadChunk()
        {
            var body = new string('b', 3000);
            using var server = new RawReplyServer($"HTTP/1.1 200 OK\r\nContent-Length: {body.Length}\r\n\r\n{body}");
            var reports = 0;

            var result = await Curl.ExecuteAsync($"curl {server.Url("/")}",
                new CurlSettings().WithBufferSize(1000).WithProgress((a, b, c) => reports++));

            result.Body.Should().Be(body);
            reports.Should().BeGreaterOrEqualTo(3, "each read is at most 1000 bytes");
        }

        [Fact]
        public async Task Builder_WithSettings_DoesNotChangeTheBuilder()
        {
            using var server = new RawReplyServer(Ok);
            var builder = CurlRequestBuilder.Get(server.Url("/"));

            await builder.ExecuteAsync(new CurlSettings().WithHeader("X-Once", "1").WithUserAgent("Once/1").WithTimeout(5));
            await builder.ExecuteAsync();

            Header(server.Requests[0], "X-Once").Should().Be("X-Once: 1");
            Header(server.Requests[1], "X-Once").Should().BeNull();
            builder.GetOptions().MaxTime.Should().BeNull();
            builder.GetOptions().UserAgent.Should().BeNull();
        }

        // ------------------------------------------------------------------ E. Clone copies every property

        [Fact]
        public void Clone_CopiesEveryPublicProperty_AndNeverSharesACollection()
        {
            var original = new CurlOptions();
            var properties = typeof(CurlOptions).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(p => p.GetIndexParameters().Length == 0)
                .ToList();
            var skipped = new System.Collections.Generic.List<string>();
            foreach (var property in properties)
            {
                if (!SetDistinctValue(original, property))
                {
                    skipped.Add(property.Name);
                }
            }
            skipped.Should().BeEmpty("the test must know how to give every property a non-default value; extend SetDistinctValue for new types");

            var clone = original.Clone();

            foreach (var property in properties)
            {
                var before = property.GetValue(original);
                var after = property.GetValue(clone);
                if (before is System.Collections.IEnumerable list && !(before is string))
                {
                    after.Should().NotBeNull(property.Name);
                    ReferenceEquals(before, after).Should().BeFalse($"{property.Name} must be a copy, not the same collection");
                    if (property.Name == nameof(CurlOptions.AdditionalTransfers))
                    {
                        ((System.Collections.Generic.List<CurlOptions>)after!).Select(o => o.Url)
                            .Should().Equal(((System.Collections.Generic.List<CurlOptions>)before!).Select(o => o.Url));
                    }
                    else
                    {
                        ((System.Collections.IEnumerable)after!).Cast<object>().Should().Equal(list.Cast<object>(), $"{property.Name} must keep its entries");
                    }
                }
                else
                {
                    after.Should().BeEquivalentTo(before, $"Clone() must copy {property.Name}");
                }
            }

            clone.Headers.ContainsKey("x-release").Should().BeTrue("the header dictionary keeps its case-insensitive comparer");
        }

        /// <summary>Gives <paramref name="property"/> a value that differs from a new CurlOptions' default.</summary>
        private static bool SetDistinctValue(CurlOptions options, System.Reflection.PropertyInfo property)
        {
            var type = property.PropertyType;
            var name = property.Name;
            var current = property.GetValue(options);
            switch (current)
            {
                case System.Collections.Generic.Dictionary<string, string> dictionary:
                    dictionary["X-Release"] = name;
                    return true;
                case System.Collections.Generic.List<string> strings:
                    strings.Add(name);
                    return true;
                case System.Collections.Generic.HashSet<string> set:
                    set.Add(name);
                    return true;
                case System.Collections.Generic.List<CurlFormPart> parts:
                    parts.Add(new CurlFormPart { Name = name, Value = "v" });
                    return true;
                case System.Collections.Generic.List<CurlOptions> transfers:
                    transfers.Add(new CurlOptions { Url = "https://next.example/" + name });
                    return true;
            }

            if (!property.CanWrite || property.SetMethod == null || !property.SetMethod.IsPublic)
            {
                return false;
            }

            object? value;
            var underlying = Nullable.GetUnderlyingType(type) ?? type;
            if (underlying == typeof(string)) value = "value-" + name;
            else if (underlying == typeof(bool)) value = !(current as bool? ?? false);
            else if (underlying == typeof(int)) value = 7;
            else if (underlying == typeof(long)) value = 7L;
            else if (underlying == typeof(double)) value = 7.5;
            else if (underlying == typeof(DateTimeOffset)) value = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero);
            else if (underlying.IsEnum) value = Enum.GetValues(underlying).Cast<object>().Last();
            else if (underlying == typeof(byte[])) value = new byte[] { 1, 2, 3 };
            else if (underlying == typeof(System.Net.NetworkCredential)) value = new System.Net.NetworkCredential("u-" + name, "p");
            else if (underlying == typeof(AwsSigV4Config)) value = AwsSigV4Config.Parse("aws:amz:eu-west-1:s3");
            else if (underlying == typeof(Action<double, long, long>)) value = new Action<double, long, long>((a, b, c) => { });
            else return false;

            property.SetValue(options, value);
            return true;
        }

        // ------------------------------------------------------------------ G. content types and generated code

        private static string? Header(string request, string name) =>
            request.Split(new[] { "\r\n" }, StringSplitOptions.None)
                .FirstOrDefault(l => l.StartsWith(name + ":", StringComparison.OrdinalIgnoreCase));

        [Fact]
        public async Task BinaryData_KeepsTheExplicitContentType()
        {
            using var server = new RawReplyServer(Ok);
            var options = new CurlOptions
            {
                Url = server.Url("/upload"),
                Method = "POST",
                BinaryData = new byte[] { 0x89, 0x50, 0x4E, 0x47 },
                Headers = new System.Collections.Generic.Dictionary<string, string> { ["content-type"] = "image/png" },
            };

            await Curl.ExecuteAsync(options);

            Header(server.Requests.Single(), "Content-Type").Should().Be("Content-Type: image/png");
        }

        [Fact]
        public async Task UploadFile_KeepsTheExplicitContentType_AndSendsNoneOtherwise()
        {
            File.WriteAllBytes(P("blob.bin"), new byte[] { 1, 2, 3 });
            using var server = new RawReplyServer(Ok);

            await Curl.ExecuteAsync($"curl -T {Q(P("blob.bin"))} -H \"Content-Type: application/octet-stream\" {server.Url("/a/")}");
            await Curl.ExecuteAsync($"curl -T {Q(P("blob.bin"))} {server.Url("/b/")}");

            Header(server.Requests[0], "Content-Type").Should().Be("Content-Type: application/octet-stream");
            Header(server.Requests[1], "Content-Type").Should().BeNull("curl sends no Content-Type for -T unless -H gives one");
        }

        [Fact]
        public async Task DataWithAnUnparsableContentType_IsSentVerbatim_NotARawFormatException()
        {
            using var server = new RawReplyServer(Ok);
            var options = new CurlOptions
            {
                Url = server.Url("/"),
                Method = "POST",
                Data = "a=1",
                Headers = new System.Collections.Generic.Dictionary<string, string> { ["Content-Type"] = "weird type;;" },
            };

            await Curl.ExecuteAsync(options);

            Header(server.Requests.Single(), "Content-Type").Should().Be("Content-Type: weird type;;");
        }

        [Fact]
        public void ToHttpClient_WithoutL_TurnsAutoRedirectOff()
        {
            var code = Curl.ToHttpClient("curl https://example.com/old");

            code.Should().Contain("AllowAutoRedirect = false").And.Contain("new HttpClient(handler)");
            code.Should().NotContain("AllowAutoRedirect = true");
        }

        [Fact]
        public void ToHttpClient_WithL_FollowsRedirects()
        {
            var code = Curl.ToHttpClient("curl -L --max-redirs 5 https://example.com/old");

            code.Should().Contain("AllowAutoRedirect = true").And.Contain("MaxAutomaticRedirections = 5");
            code.Should().NotContain("AllowAutoRedirect = false");
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

        // ------------------------------------------------------------------ F. secrets never reach logs or messages

        private static readonly string[] Secrets = { "s3cretPw", "tok-123-secret", "pr0xyPw", "urlPw99", "cook1eVal", "p12Pw" };

        private static void ShouldHoldNoSecret(string? text, string what)
        {
            foreach (var secret in Secrets)
            {
                (text ?? string.Empty).Should().NotContain(secret, $"{what} must not print credentials");
            }
        }

        [Theory]
        [InlineData("curl -u alice:s3cretPw https://example.com/", "-u ***")]
        [InlineData("curl -ualice:s3cretPw https://example.com/", "-u***")]
        [InlineData("curl --user 'alice:s3cretPw' https://example.com/", "--user ***")]
        [InlineData("curl -H 'Authorization: Bearer tok-123-secret' https://example.com/", "Authorization: ***")]
        [InlineData("curl -H \"Proxy-Authorization: Basic pr0xyPw\" https://example.com/", "Proxy-Authorization: ***")]
        [InlineData("curl --oauth2-bearer tok-123-secret https://example.com/", "--oauth2-bearer ***")]
        [InlineData("curl -x http://proxy:3128 -U bob:pr0xyPw https://example.com/", "-U ***")]
        [InlineData("curl https://alice:urlPw99@example.com/", "https://alice:***@example.com/")]
        [InlineData("curl -b 'session=cook1eVal' -H 'Cookie: a=cook1eVal' https://example.com/", "Cookie: ***")]
        [InlineData("curl --cert id.p12:p12Pw --cert-type P12 https://example.com/", "--cert id.p12:***")]
        [InlineData("curl -su alice:s3cretPw https://example.com/", "-su ***")]
        [InlineData("curl -fsSualice:s3cretPw https://example.com/", "-fsSu***")]
        [InlineData("curl -sU bob:pr0xyPw -x http://p:3128 https://example.com/", "-sU ***")]
        [InlineData("curl -Eid.pem:p12Pw https://example.com/", "-Eid.pem:***")]
        [InlineData("curl -x bob:pr0xyPw@proxy:8080 https://example.com/", "-x bob:***@proxy:8080")]
        [InlineData("curl -x http://bob:pr0xyPw@proxy:8080 https://example.com/", "http://bob:***@proxy:8080")]
        [InlineData("curl -bsession=cook1eVal https://example.com/", "-b***")]
        public void RedactCommand_MasksEveryCredential(string command, string expected)
        {
            var redacted = ErrorText.RedactCommand(command);

            redacted.Should().Contain(expected);
            ShouldHoldNoSecret(redacted, "RedactCommand");
        }

        [Fact]
        public async Task LoggingMiddleware_NeverLogsTheBearerTokenAddedByAuthenticationMiddleware()
        {
            using var server = new RawReplyServer(Ok);
            var lines = new System.Collections.Generic.List<string>();
            var pipeline = CurlDotNet.Middleware.CurlMiddlewarePipeline.CreateBuilder()
                .Use(new CurlDotNet.Middleware.AuthenticationMiddleware(ctx => Task.FromResult("tok-123-secret")))
                .Use(new CurlDotNet.Middleware.LoggingMiddleware(lines.Add))
                .WithHandler(ctx => Curl.ExecuteAsync(ctx.Command, ctx.CancellationToken))
                .Build();

            var result = await pipeline.ExecuteAsync(new CurlDotNet.Middleware.CurlContext { Command = $"curl -u alice:s3cretPw {server.Url("/me")}" });

            result.StatusCode.Should().Be(200);
            Header(server.Requests.Single(), "Authorization").Should().Be("Authorization: Bearer tok-123-secret", "the token is still sent");
            lines.Should().NotBeEmpty();
            lines.Should().Contain(l => l.Contains("Authorization: ***"));
            foreach (var line in lines)
            {
                ShouldHoldNoSecret(line, "LoggingMiddleware");
            }
        }

        [Theory]
        [InlineData("curl -f -u alice:s3cretPw -H 'Authorization: Bearer tok-123-secret' -b 'id=cook1eVal' URL")]
        [InlineData("curl -f -H 'Proxy-Authorization: Basic pr0xyPw' --oauth2-bearer tok-123-secret URL")]
        public async Task FailedTransfer_MessagesAndLogHelpers_HoldNoSecret(string template)
        {
            using var server = new RawReplyServer("HTTP/1.1 403 Forbidden\r\nContent-Length: 0\r\n\r\n");

            Func<Task> act = () => Curl.ExecuteAsync(template.Replace("URL", server.Url("/private")));

            var ex = (await act.Should().ThrowAsync<CurlException>()).Which;
            ShouldHoldNoSecret(ex.Message, "the message");
            ShouldHoldNoSecret(ex.ToString(), "ToString()");
            ShouldHoldNoSecret(ex.ToDetailedString(), "ToDetailedString()");
            ShouldHoldNoSecret(ex.ToJson(), "ToJson()");
            ShouldHoldNoSecret(string.Join(" ", ex.GetDiagnosticInfo().Values), "GetDiagnosticInfo()");
            ShouldHoldNoSecret(ex.ToUserFriendlyMessage(), "ToUserFriendlyMessage()");
        }

        [Fact]
        public async Task ConnectionFailure_WithUrlAndProxyPasswords_HoldsNoSecret()
        {
            Func<Task> direct = () => Curl.ExecuteAsync("curl https://alice:urlPw99@127.0.0.1:9/x --connect-timeout 5");
            var ex = (await direct.Should().ThrowAsync<CurlException>()).Which;
            ShouldHoldNoSecret(ex.Message, "a connect failure");
            ShouldHoldNoSecret(ex.ToDetailedString(), "ToDetailedString()");

            Func<Task> proxied = () => Curl.ExecuteAsync("curl -x http://bob:pr0xyPw@127.0.0.1:9 http://example.com/ --connect-timeout 5");
            var proxyEx = (await proxied.Should().ThrowAsync<CurlException>()).Which;
            ShouldHoldNoSecret(proxyEx.Message, "a proxy connect failure");
            ShouldHoldNoSecret(proxyEx.ToJson(), "ToJson()");
        }

        [Fact]
        public async Task TraceInfoLines_HoldNoUrlPassword()
        {
            var trace = P("trace.txt");
            File.WriteAllText(P("exists.txt"), "x");
            using var server = new RawReplyServer(Ok);

            await Curl.ExecuteAsync($"curl --trace-ascii {Q(trace)} --skip-existing -o {Q(P("exists.txt"))} http://alice:urlPw99@127.0.0.1:{server.Port}/f");

            ShouldHoldNoSecret(File.Exists(trace) ? File.ReadAllText(trace) : string.Empty, "the trace's own lines");
        }

        // ------------------------------------------------------------------ I. ConfigureAwait(false)

        /// <summary>A UI-like context: continuations posted to it run only when its own thread pumps, which a blocked thread never does.</summary>
        private sealed class NonPumpingContext : System.Threading.SynchronizationContext
        {
            public int Posts;

            public override void Post(System.Threading.SendOrPostCallback d, object? state) => System.Threading.Interlocked.Increment(ref Posts);

            public override void Send(System.Threading.SendOrPostCallback d, object? state) => throw new InvalidOperationException("Send is not expected");
        }

        [Fact]
        public void SyncExecute_OnASingleThreadedContext_DoesNotDeadlock()
        {
            using var server = new RawReplyServer(
                "HTTP/1.1 302 Found\r\nLocation: /done\r\nContent-Length: 0\r\n\r\n",
                Ok);
            CurlResult? result = null;
            Exception? failure = null;
            NonPumpingContext? context = null;
            var thread = new System.Threading.Thread(() =>
            {
                context = new NonPumpingContext();
                System.Threading.SynchronizationContext.SetSynchronizationContext(context);
                try
                {
                    result = Curl.Execute($"curl -L --retry 1 -w \"%{{http_code}}\" {server.Url("/start")}");
                }
                catch (Exception ex)
                {
                    failure = ex;
                }
            }) { IsBackground = true }; // a deadlock must not keep the test host alive
            thread.Start();

            thread.Join(TimeSpan.FromSeconds(30)).Should().BeTrue("a blocked UI thread must not deadlock the transfer");
            failure.Should().BeNull();
            result!.Body.Should().Be("ok\n200");
            context!.Posts.Should().Be(0, "no continuation is posted back to the caller's context");
        }

        [Fact]
        public void EveryAwaitInTheLibrary_UsesConfigureAwaitFalse()
        {
            var src = Path.Combine(ExceptionMessageTests.RepoRoot(), "src", "CurlDotNet");
            var missing = new System.Collections.Generic.List<string>();
            foreach (var file in Directory.GetFiles(src, "*.cs", SearchOption.AllDirectories)
                         .Where(f => !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar)))
            {
                var lines = File.ReadAllLines(file);
                for (var i = 0; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var code = line.TrimStart();
                    if (code.StartsWith("//", StringComparison.Ordinal))
                    {
                        continue;
                    }
                    var at = System.Text.RegularExpressions.Regex.Match(line, @"\bawait\s");
                    if (!at.Success || line.Substring(0, at.Index).Count(c => c == '"') % 2 == 1)
                    {
                        continue; // not an await, or an await inside a string (generated code samples)
                    }
                    // the awaited expression: up to the first ';' or unbalanced ')' outside parentheses and braces
                    var rest = line.Substring(at.Index) + "\n" + string.Join("\n", lines.Skip(i + 1).Take(30));
                    var depth = 0;
                    var end = rest.Length;
                    for (var k = 0; k < rest.Length; k++)
                    {
                        var c = rest[k];
                        if (c == '(' || c == '{' || c == '[') depth++;
                        else if (c == ')' || c == '}' || c == ']')
                        {
                            if (depth == 0) { end = k; break; }
                            depth--;
                        }
                        else if ((c == ';' || c == ',') && depth == 0) { end = k; break; }
                    }
                    if (rest.Substring(0, end).TrimEnd().EndsWith(".ConfigureAwait(false)", StringComparison.Ordinal) == false)
                    {
                        missing.Add($"{Path.GetFileName(file)}:{i + 1}: {code}");
                    }
                }
            }

            missing.Should().BeEmpty("library code must not resume on the caller's SynchronizationContext");
        }

        // ------------------------------------------------------------------ D. options that are parsed

        [Theory]
        [InlineData("--interface eth0", "--interface")]
        [InlineData("--dns-servers 1.1.1.1", "--dns-servers")]
        [InlineData("-Q 'DELE x'", "--quote")]
        [InlineData("--disable-epsv", "--disable-epsv")]
        [InlineData("--key-type DER --key k.der", "--key-type")]
        public void NotApplicableOptions_AreRecordedAsUnsupported(string option, string recorded)
        {
            var o = new CommandParser().Parse($"curl {option} https://example.com/");

            o.UnsupportedOptions.Should().Contain(recorded);
        }

        [Fact]
        public void ProxyHeader_IsAppliedSoNotUnsupported()
        {
            var o = new CommandParser().Parse("curl -x http://proxy:3128 --proxy-header 'X-Via: me' --no-proxytunnel http://example.com/");

            o.UnsupportedOptions.Should().NotContain("--proxy-header").And.NotContain("--no-proxytunnel");
        }

        [Fact]
        public async Task ProxyHeader_IsSentToThePlainProxy()
        {
            using var proxy = new RawReplyServer(Ok);

            await Curl.ExecuteAsync($"curl -x {proxy.Url()} --proxy-header \"X-Via: me\" http://origin.example/");

            Header(proxy.Requests.Single(), "X-Via").Should().Be("X-Via: me");
        }

        [Theory]
        [InlineData("client.p12:s3cret", "client.p12", "s3cret")]
        [InlineData("client.pem", "client.pem", null)]
        [InlineData("C:\\certs\\id.p12:pw", "C:\\certs\\id.p12", "pw")]
        [InlineData("odd\\:name.pem:pw", "odd:name.pem", "pw")]
        public void Cert_SplitsFileAndPassword_LikeCurl(string value, string file, string? password)
        {
            CommandParser.SplitCertArgument(value).Should().Be((file, password));
        }

        [Fact]
        public void TlsOptions_ParseIntoProperties()
        {
            var o = new CommandParser().Parse("curl --cert c.p12:pw --cert-type P12 --key k.pem --pass phrase --cacert ca.pem --capath cas " +
                                              "--pinnedpubkey sha256//AAAA --tlsv1.2 --tls-max 1.3 https://example.com/");

            o.CertFile.Should().Be("c.p12");
            o.CertPassword.Should().Be("pw");
            o.CertType.Should().Be("P12");
            o.KeyFile.Should().Be("k.pem");
            o.KeyPassword.Should().Be("phrase");
            o.CaCertFile.Should().Be("ca.pem");
            o.CaPath.Should().Be("cas");
            o.PinnedPublicKey.Should().Be("sha256//AAAA");
            o.TlsMinVersion.Should().Be("1.2");
            o.TlsMaxVersion.Should().Be("1.3");
            o.UnsupportedOptions.Should().BeEmpty("every one of these is applied on .NET 5 and later");
        }

        [Fact]
        public async Task TlsFiles_AreNotLoaded_ForAPlainHttpRequest()
        {
            using var server = new RawReplyServer(Ok);

            var result = await Curl.ExecuteAsync($"curl --cacert {Q(P("missing-ca.pem"))} --cert {Q(P("missing.pem"))} --tlsv1.3 --tls-max 1.2 {server.Url("/")}");

            result.Body.Should().Be("ok\n", "curl only loads TLS files when it makes a TLS connection");
        }

        [Fact]
        public async Task TlsMaxAlone_BelowTheDefaultMinimum_NamesTheMinimum()
        {
            Func<Task> act = () => Curl.ExecuteAsync("curl --tls-max 1.0 https://127.0.0.1:9/");

            var ex = (await act.Should().ThrowAsync<CurlSslConnectErrorException>()).Which;
            ex.WhatHappened.Should().Contain("--tls-max 1.0 is lower than the minimum TLS 1.2 (curl's default minimum)");
        }

        [Fact]
        public async Task PinnedPubKey_WithACallerSuppliedHttpClient_IsExit4_NotSilentlySkipped()
        {
            using var client = new System.Net.Http.HttpClient();
            using var engine = new CurlEngine(client);

            Func<Task> act = () => engine.ExecuteAsync("curl --pinnedpubkey sha256//AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA= https://127.0.0.1:9/");

            var ex = (await act.Should().ThrowAsync<CurlNotBuiltInException>()).Which;
            ex.CurlErrorCode.Should().Be(4);
            ExceptionMessageTests.AssertWhatFixLink(ex);
        }

        [Theory]
        [InlineData("-m 1e1", 10.0)]
        [InlineData("-m +5", 5.0)]
        public void SecondsOptions_AcceptStrtodForms(string option, double expected)
        {
            new CommandParser().Parse($"curl {option} https://example.com/").MaxTime.Should().Be(expected);
        }

        [Fact]
        public void SpeedLimit_AcceptsValuesAboveInt32()
        {
            new CommandParser().Parse("curl -Y 5000000000 https://example.com/").SpeedLimit.Should().Be(5000000000L);
        }

        [Fact]
        public async Task Settings_ApplyToEveryUrlOfTheCommand()
        {
            using var server = new RawReplyServer(Ok);

            await Curl.ExecuteAsync($"curl {server.Url("/a")} {server.Url("/b")}", new CurlSettings().WithUserAgent("Both/1"));

            server.Requests.Select(r => Header(r, "User-Agent")).Should().Equal("User-Agent: Both/1", "User-Agent: Both/1");
        }

        [Fact]
        public void TlsMax_RejectsAnUnknownVersion_WithExitTwo()
        {
            var ex = Parses<CurlException>("curl --tls-max 2.0 https://example.com/");

            ex.CurlErrorCode.Should().Be(2);
            ExceptionMessageTests.AssertWhatFixLink(ex);
        }

        [Fact]
        public void Ciphers_OpenSslNames_AreUnsupported_IanaNamesFollowThePlatform()
        {
            var openSsl = new CommandParser().Parse("curl --ciphers ECDHE-RSA-AES128-GCM-SHA256 https://example.com/");
            openSsl.UnsupportedOptions.Should().Contain("--ciphers");
            openSsl.Ciphers.Should().BeNull();

            var iana = new CommandParser().Parse("curl --tls13-ciphers TLS_AES_256_GCM_SHA384 https://example.com/");
            if (CurlTls.PlatformSupportsCipherPolicy())
            {
                iana.Ciphers.Should().Be("TLS_AES_256_GCM_SHA384");
                iana.UnsupportedOptions.Should().BeEmpty();
            }
            else
            {
                iana.Ciphers.Should().BeNull();
                iana.UnsupportedOptions.Should().Contain("--tls13-ciphers");
            }
        }

#if NET8_0_OR_GREATER
        [Fact]
        public async Task CaCert_TrustsAPrivateCa_ThatTheSystemDoesNot()
        {
            using var pki = new TestPki();
            using var server = new TlsTestServer(pki.ServerCertificate);

            Func<Task> withoutCa = () => Curl.ExecuteAsync($"curl {server.Url}");
            var rejected = (await withoutCa.Should().ThrowAsync<CurlPeerFailedVerificationException>()).Which;
            rejected.CurlErrorCode.Should().Be(60);
            rejected.Message.Should().Contain("--cacert");
            ExceptionMessageTests.AssertWhatFixLink(rejected);

            var result = await Curl.ExecuteAsync($"curl --cacert {Q(pki.CaPemPath)} {server.Url}");
            result.Body.Should().Be("tls ok");
        }

        [Fact]
        public async Task CaCert_WithTheWrongCa_IsRejected60()
        {
            using var pki = new TestPki();
            using var other = new TestPki();
            using var server = new TlsTestServer(pki.ServerCertificate);

            Func<Task> act = () => Curl.ExecuteAsync($"curl --cacert {Q(other.CaPemPath)} {server.Url}");

            var ex = (await act.Should().ThrowAsync<CurlPeerFailedVerificationException>()).Which;
            ex.Message.Should().Contain("--cacert/--capath bundle");
            ExceptionMessageTests.AssertWhatFixLink(ex);
        }

        [Fact]
        public async Task CaCert_ACertificateNotIssuedForServers_IsRejected60()
        {
            using var pki = new TestPki();
            using var server = new TlsTestServer(pki.ClientAuthOnlyServerCertificate);

            Func<Task> act = () => Curl.ExecuteAsync($"curl --cacert {Q(pki.CaPemPath)} {server.Url}");

            (await act.Should().ThrowAsync<CurlPeerFailedVerificationException>()).Which.CurlErrorCode.Should().Be(60);
        }

        [Fact]
        public async Task CaPath_TrustsEveryCertificateInTheFolder()
        {
            using var pki = new TestPki();
            using var server = new TlsTestServer(pki.ServerCertificate);
            var dir = Directory.CreateDirectory(P("cas")).FullName;
            File.Copy(pki.CaPemPath, Path.Combine(dir, "5ed36f99.0"));
            File.WriteAllText(Path.Combine(dir, "README"), "not a certificate");

            var result = await Curl.ExecuteAsync($"curl --capath {Q(dir)} {server.Url}");

            result.Body.Should().Be("tls ok");
        }

        [Fact]
        public async Task CaCert_MissingFile_IsExit77()
        {
            using var pki = new TestPki();
            using var server = new TlsTestServer(pki.ServerCertificate);

            Func<Task> act = () => Curl.ExecuteAsync($"curl --cacert {Q(P("missing-ca.pem"))} {server.Url}");

            var ex = (await act.Should().ThrowAsync<CurlSslCaCertBadFileException>()).Which;
            ex.CurlErrorCode.Should().Be(77);
            ex.FilePath.Should().EndWith("missing-ca.pem");
            ExceptionMessageTests.AssertWhatFixLink(ex);
        }

        [Fact]
        public async Task PinnedPubKey_Match_Passes_Mismatch_Is90_EvenWithInsecure()
        {
            using var pki = new TestPki();
            using var server = new TlsTestServer(pki.ServerCertificate);

            var good = await Curl.ExecuteAsync($"curl --cacert {Q(pki.CaPemPath)} --pinnedpubkey \"sha256//AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=;{pki.ServerPin}\" {server.Url}");
            good.Body.Should().Be("tls ok");

            Func<Task> act = () => Curl.ExecuteAsync($"curl -k --pinnedpubkey sha256//AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA= {server.Url}");
            var ex = (await act.Should().ThrowAsync<CurlSslPinnedPublicKeyMismatchException>()).Which;
            ex.CurlErrorCode.Should().Be(90);
            ex.Message.Should().Contain(pki.ServerPin, "the message shows the key that was presented");
            ExceptionMessageTests.AssertWhatFixLink(ex);
        }

        [Fact]
        public async Task PinnedPubKey_FromAPublicKeyFile()
        {
            using var pki = new TestPki();
            using var server = new TlsTestServer(pki.ServerCertificate);

            var result = await Curl.ExecuteAsync($"curl -k --pinnedpubkey {Q(pki.ServerPublicKeyPemPath)} {server.Url}");

            result.Body.Should().Be("tls ok");
        }

        [Fact]
        public async Task ClientCertificate_Pem_IsPresented()
        {
            using var pki = new TestPki();
            using var server = new TlsTestServer(pki.ServerCertificate, requireClientCertificate: true);

            var result = await Curl.ExecuteAsync($"curl --cacert {Q(pki.CaPemPath)} --cert {Q(pki.ClientCertPemPath)} --key {Q(pki.ClientKeyPemPath)} {server.Url}");

            result.Body.Should().Be("tls ok");
            server.ClientCertificateThumbprint.Should().Be(pki.ClientThumbprint);
        }

        [Fact]
        public async Task ClientCertificate_P12WithPassword_IsPresented()
        {
            using var pki = new TestPki();
            using var server = new TlsTestServer(pki.ServerCertificate, requireClientCertificate: true);

            var result = await Curl.ExecuteAsync($"curl -k --cert {Q(pki.ClientP12Path + ":p12pass")} --cert-type P12 {server.Url}");

            result.Body.Should().Be("tls ok");
            server.ClientCertificateThumbprint.Should().Be(pki.ClientThumbprint);
        }

        [Fact]
        public async Task ClientCertificate_WrongPassword_IsExit58()
        {
            using var pki = new TestPki();
            using var server = new TlsTestServer(pki.ServerCertificate);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -k --cert {Q(pki.ClientP12Path + ":wrong")} --cert-type P12 {server.Url}");

            var ex = (await act.Should().ThrowAsync<CurlSslCertificateProblemException>()).Which;
            ex.CurlErrorCode.Should().Be(58);
            ex.Message.Should().NotContain("wrong", "the password is never printed");
            ExceptionMessageTests.AssertWhatFixLink(ex);
        }

        [Fact]
        public async Task TlsMax12_NegotiatesTls12()
        {
            using var pki = new TestPki();
            using var server = new TlsTestServer(pki.ServerCertificate);

            var result = await Curl.ExecuteAsync($"curl -k --tls-max 1.2 {server.Url}");

            result.Body.Should().Be("tls ok");
            server.NegotiatedProtocol.Should().Be(System.Security.Authentication.SslProtocols.Tls12);
        }

        [Fact]
        public async Task TlsV13_AgainstATls12OnlyServer_IsExit35()
        {
            using var pki = new TestPki();
            using var server = new TlsTestServer(pki.ServerCertificate, protocols: System.Security.Authentication.SslProtocols.Tls12);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -k --tlsv1.3 {server.Url}");

            var ex = (await act.Should().ThrowAsync<CurlSslException>()).Which;
            ex.CurlErrorCode.Should().Be(35);
        }

        [Fact]
        public async Task TlsMaxBelowMin_IsExit35_BeforeConnecting()
        {
            Func<Task> act = () => Curl.ExecuteAsync("curl --tlsv1.3 --tls-max 1.2 https://127.0.0.1:9/");

            var ex = (await act.Should().ThrowAsync<CurlSslConnectErrorException>()).Which;
            ex.CurlErrorCode.Should().Be(35);
            ExceptionMessageTests.AssertWhatFixLink(ex);
        }
#endif
    }

#if NET8_0_OR_GREATER
    /// <summary>A throw-away CA with a server certificate for 127.0.0.1 and a client certificate, written as PEM/P12 files.</summary>
    internal sealed class TestPki : IDisposable
    {
        private readonly string _dir = Path.Combine(Path.GetTempPath(), "curldotnet-pki-" + Guid.NewGuid().ToString("N"));

        public TestPki()
        {
            Directory.CreateDirectory(_dir);
            var now = DateTimeOffset.UtcNow;
            using var caKey = System.Security.Cryptography.RSA.Create(2048);
            var caRequest = new System.Security.Cryptography.X509Certificates.CertificateRequest("CN=CurlDotNet Test CA " + Guid.NewGuid().ToString("N").Substring(0, 8),
                caKey, System.Security.Cryptography.HashAlgorithmName.SHA256, System.Security.Cryptography.RSASignaturePadding.Pkcs1);
            caRequest.CertificateExtensions.Add(new System.Security.Cryptography.X509Certificates.X509BasicConstraintsExtension(true, false, 0, true));
            caRequest.CertificateExtensions.Add(new System.Security.Cryptography.X509Certificates.X509KeyUsageExtension(
                System.Security.Cryptography.X509Certificates.X509KeyUsageFlags.KeyCertSign | System.Security.Cryptography.X509Certificates.X509KeyUsageFlags.CrlSign, true));
            using var ca = caRequest.CreateSelfSigned(now.AddDays(-1), now.AddDays(30));
            CaPemPath = Write("ca.pem", ca.ExportCertificatePem());

            ServerCertificate = Issue(ca, "CN=127.0.0.1", serverAuth: true, out var serverKey, out var serverPublicKeyPem, out _);
            ServerPin = "sha256//" + Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(ServerCertificate.PublicKey.ExportSubjectPublicKeyInfo()));
            ServerPublicKeyPemPath = Write("server-pub.pem", serverPublicKeyPem);
            serverKey.Dispose();

            ClientAuthOnlyServerCertificate = Issue(ca, "CN=127.0.0.1", serverAuth: false, out var wrongKey, out _, out _, sanWithClientAuthOnly: true);
            wrongKey.Dispose();

            using var client = Issue(ca, "CN=curl client", serverAuth: false, out var clientKey, out _, out var clientP12);
            ClientThumbprint = client.Thumbprint;
            ClientCertPemPath = Write("client.pem", client.ExportCertificatePem());
            ClientKeyPemPath = Write("client.key", clientKey.ExportPkcs8PrivateKeyPem());
            ClientP12Path = Path.Combine(_dir, "client.p12");
            File.WriteAllBytes(ClientP12Path, clientP12);
            clientKey.Dispose();
        }

        public string CaPemPath { get; }
        public System.Security.Cryptography.X509Certificates.X509Certificate2 ServerCertificate { get; }
        public string ServerPin { get; }
        public System.Security.Cryptography.X509Certificates.X509Certificate2 ClientAuthOnlyServerCertificate { get; }
        public string ServerPublicKeyPemPath { get; }
        public string ClientThumbprint { get; }
        public string ClientCertPemPath { get; }
        public string ClientKeyPemPath { get; }
        public string ClientP12Path { get; }

        private string Write(string name, string text)
        {
            var path = Path.Combine(_dir, name);
            File.WriteAllText(path, text);
            return path;
        }

        private static System.Security.Cryptography.X509Certificates.X509Certificate2 Issue(
            System.Security.Cryptography.X509Certificates.X509Certificate2 ca, string subject, bool serverAuth,
            out System.Security.Cryptography.RSA key, out string publicKeyPem, out byte[] p12WithPassword, bool sanWithClientAuthOnly = false)
        {
            key = System.Security.Cryptography.RSA.Create(2048);
            var request = new System.Security.Cryptography.X509Certificates.CertificateRequest(subject, key,
                System.Security.Cryptography.HashAlgorithmName.SHA256, System.Security.Cryptography.RSASignaturePadding.Pkcs1);
            request.CertificateExtensions.Add(new System.Security.Cryptography.X509Certificates.X509BasicConstraintsExtension(false, false, 0, false));
            request.CertificateExtensions.Add(new System.Security.Cryptography.X509Certificates.X509EnhancedKeyUsageExtension(
                new System.Security.Cryptography.OidCollection { new System.Security.Cryptography.Oid(serverAuth ? "1.3.6.1.5.5.7.3.1" : "1.3.6.1.5.5.7.3.2") }, false));
            if (serverAuth || sanWithClientAuthOnly)
            {
                var san = new System.Security.Cryptography.X509Certificates.SubjectAlternativeNameBuilder();
                san.AddIpAddress(System.Net.IPAddress.Loopback);
                san.AddDnsName("localhost");
                request.CertificateExtensions.Add(san.Build());
            }
            var serial = new byte[8];
            System.Security.Cryptography.RandomNumberGenerator.Fill(serial);
            var now = DateTimeOffset.UtcNow;
            using var issued = request.Create(ca, now.AddHours(-1), now.AddDays(29), serial);
            using var withKey = System.Security.Cryptography.X509Certificates.RSACertificateExtensions.CopyWithPrivateKey(issued, key);
            publicKeyPem = key.ExportSubjectPublicKeyInfoPem();
            p12WithPassword = withKey.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Pkcs12, "p12pass");
            // a PKCS#12 round trip gives SslStream a key it can use on every platform
            return System.Security.Cryptography.X509Certificates.X509CertificateLoader.LoadPkcs12(
                withKey.Export(System.Security.Cryptography.X509Certificates.X509ContentType.Pkcs12), null);
        }

        public void Dispose()
        {
            ServerCertificate.Dispose();
            ClientAuthOnlyServerCertificate.Dispose();
            try
            {
                Directory.Delete(_dir, recursive: true);
            }
            catch (IOException)
            {
            }
        }
    }

    /// <summary>
    /// A one-request-per-connection HTTPS server on 127.0.0.1: TLS with the given certificate (optionally asking for a client
    /// certificate), answers every request with "tls ok", and records the negotiated protocol and client certificate.
    /// </summary>
    internal sealed class TlsTestServer : IDisposable
    {
        private readonly System.Net.Sockets.TcpListener _listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
        private readonly System.Security.Cryptography.X509Certificates.X509Certificate2 _certificate;
        private readonly bool _requireClientCertificate;
        private readonly System.Security.Authentication.SslProtocols _protocols;

        public TlsTestServer(System.Security.Cryptography.X509Certificates.X509Certificate2 certificate, bool requireClientCertificate = false,
            System.Security.Authentication.SslProtocols protocols = System.Security.Authentication.SslProtocols.None)
        {
            _certificate = certificate;
            _requireClientCertificate = requireClientCertificate;
            _protocols = protocols;
            _listener.Start();
            Url = $"https://127.0.0.1:{((System.Net.IPEndPoint)_listener.LocalEndpoint).Port}/";
            _ = Task.Run(AcceptLoopAsync);
        }

        public string Url { get; }

        public string? ClientCertificateThumbprint { get; private set; }

        public System.Security.Authentication.SslProtocols NegotiatedProtocol { get; private set; }

        private async Task AcceptLoopAsync()
        {
            while (true)
            {
                System.Net.Sockets.TcpClient client;
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

        private async Task ServeAsync(System.Net.Sockets.TcpClient client)
        {
            using (client)
            {
                try
                {
                    using var ssl = new System.Net.Security.SslStream(client.GetStream(), false, (s, c, ch, e) => true);
                    await ssl.AuthenticateAsServerAsync(new System.Net.Security.SslServerAuthenticationOptions
                    {
                        ServerCertificate = _certificate,
                        ClientCertificateRequired = _requireClientCertificate,
                        EnabledSslProtocols = _protocols,
                        CertificateRevocationCheckMode = System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck,
                    });
                    NegotiatedProtocol = ssl.SslProtocol;
                    if (ssl.RemoteCertificate != null)
                    {
                        ClientCertificateThumbprint = new System.Security.Cryptography.X509Certificates.X509Certificate2(ssl.RemoteCertificate).Thumbprint;
                    }

                    var buffer = new byte[8192];
                    var request = new StringBuilder();
                    while (request.ToString().IndexOf("\r\n\r\n", StringComparison.Ordinal) < 0)
                    {
                        var n = await ssl.ReadAsync(buffer, 0, buffer.Length);
                        if (n <= 0) return;
                        request.Append(Encoding.ASCII.GetString(buffer, 0, n));
                    }
                    var reply = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\nContent-Length: 6\r\nConnection: close\r\n\r\ntls ok");
                    await ssl.WriteAsync(reply, 0, reply.Length);
                    await ssl.FlushAsync();
                }
                catch (Exception)
                {
                    // handshake refused by the client (untrusted certificate, pin mismatch): nothing to answer
                }
            }
        }

        public void Dispose() => _listener.Stop();
    }
#endif
}
