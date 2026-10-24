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
    }
}
