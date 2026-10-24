using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Library defects found in the developer-experience review: progress callbacks that never fired,
    /// ToCurlCommand() dropping builder options, AsJsonDynamic() returning a JsonDocument, -D to an
    /// uncreatable path losing to the transfer error, and timings for plain HTTP.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class DeveloperExperienceTests : IDisposable
    {
        private readonly string _dir;

        public DeveloperExperienceTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "curldotnet-dx-" + Guid.NewGuid().ToString("N"));
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

        private static string Body(int length) => new string('x', length);

        private static string FixedLengthReply(int length) =>
            $"HTTP/1.1 200 OK\r\nContent-Length: {length}\r\nContent-Type: application/octet-stream\r\n\r\n{Body(length)}";

        private static string ChunkedReply(int length) =>
            $"HTTP/1.1 200 OK\r\nTransfer-Encoding: chunked\r\nContent-Type: text/plain\r\n\r\n{length:x}\r\n{Body(length)}\r\n0\r\n\r\n";

        // ------------------------------------------------------------------ 1. progress

        [Fact]
        public async Task Progress_WithSettings_ReportsEveryChunkWithPercentReceivedAndTotal()
        {
            const int size = 300_000;
            using var server = new RawReplyServer(FixedLengthReply(size));
            var calls = new List<(double Percent, long Received, long Total)>();
            var settings = new CurlSettings().WithProgress((percent, received, total) => calls.Add((percent, received, total)));

            var result = await Curl.ExecuteAsync($"curl {server.Url("/big")}", settings);

            result.StatusCode.Should().Be(200);
            calls.Should().NotBeEmpty();
            calls.Select(c => c.Received).Should().BeInAscendingOrder();
            calls.Should().OnlyContain(c => c.Total == size);
            calls.Last().Received.Should().Be(size);
            calls.Last().Percent.Should().Be(100.0);
            calls.Should().OnlyContain(c => c.Percent >= 0 && c.Percent <= 100);
        }

        [Fact]
        public async Task Progress_UnknownLength_ReportsMinusOneForPercentAndTotal()
        {
            using var server = new RawReplyServer(ChunkedReply(5000));
            var calls = new List<(double Percent, long Received, long Total)>();
            var options = new CommandParser().Parse($"curl {server.Url()}");
            options.ProgressHandler = (percent, received, total) => calls.Add((percent, received, total));

            await Curl.ExecuteAsync(options);

            calls.Should().NotBeEmpty();
            calls.Should().OnlyContain(c => c.Percent == -1 && c.Total == -1);
            calls.Last().Received.Should().Be(5000);
        }

        [Fact]
        public async Task Progress_BuilderExecuteAsyncWithSettings_AppliesTheSettings()
        {
            using var server = new RawReplyServer(FixedLengthReply(1000));
            long lastReceived = 0;
            var builder = CurlRequestBuilder.Get(server.Url("/file"));
            var settings = new CurlSettings().WithProgress((p, received, t) => lastReceived = received);

            var result = await builder.ExecuteAsync(settings);

            result.StatusCode.Should().Be(200);
            lastReceived.Should().Be(1000);
            builder.GetOptions().ProgressHandler.Should().BeNull("settings apply to that one call, not to the builder");
        }

        [Fact]
        public async Task Progress_HandlerThatThrows_AbortsWithExitCode42AndKeepsTheCause()
        {
            using var server = new RawReplyServer(FixedLengthReply(1000));
            var settings = new CurlSettings().WithProgress((p, r, t) => throw new InvalidOperationException("stop here"));

            Func<Task> act = () => Curl.ExecuteAsync($"curl {server.Url()}", settings);

            var ex = (await act.Should().ThrowAsync<CurlAbortedByCallbackException>()).Which;
            ex.CurlErrorCode.Should().Be(42);
            ex.InnerException.Should().BeOfType<InvalidOperationException>();
            ex.Message.Should().Contain("stop here").And.Contain("WithProgress").And.Contain("See https://github.com/jacob-mellor/curl-dot-net/");
        }

        // ------------------------------------------------------------------ 2. ToCurlCommand

        [Fact]
        public void ToCurlCommand_WithProxyCredentials_IncludesProxyUser()
        {
            var command = CurlRequestBuilder.Get("https://example.com/")
                .WithProxy("http://proxy.local:3128", "alice", "s3cret")
                .ToCurlCommand();

            command.Should().Contain("-x http://proxy.local:3128");
            command.Should().Contain("--proxy-user 'alice:s3cret'");
        }

        [Fact]
        public void ToCurlCommand_EveryBuilderOptionRoundTripsThroughTheParser()
        {
            var trace = Path.Combine(_dir, "trace file.txt");
            var builder = CurlRequestBuilder.Put("https://api.example.com/items/1?x=1&y=2")
                .WithHeader("X-Api-Key", "it's secret")
                .WithUserAgent("MyApp/1.0")
                .WithReferer("https://ref.example/")
                .WithBasicAuth("bob", "pa ss")
                .WithTimeout(TimeSpan.FromSeconds(12.5))
                .WithConnectTimeout(TimeSpan.FromSeconds(3))
                .FollowRedirects()
                .WithMaxRedirects(7)
                .Insecure()
                .WithProxy("http://proxy.local:3128", "alice", "s3cret")
                .WithCookie("a=1; b=2")
                .WithCookieJar(Path.Combine(_dir, "jar.txt"))
                .WithOutput(Path.Combine(_dir, "out.bin"))
                .IncludeHeaders()
                .Silent()
                .FailOnError()
                .WithHttpVersion("2.0")
                .Compressed()
                .WithRange("0-99")
                .WithTrace(trace, includeTimestamps: true)
                .WithBinaryData(Encoding.UTF8.GetBytes("{\"name\":\"x\"}"));

            var command = builder.ToCurlCommand();
            var parsed = new CommandParser().Parse(command);

            parsed.Method.Should().Be("PUT");
            parsed.Url.Should().Be("https://api.example.com/items/1?x=1&y=2");
            parsed.Headers["X-Api-Key"].Should().Be("it's secret");
            parsed.UserAgent.Should().Be("MyApp/1.0");
            parsed.Referer.Should().Be("https://ref.example/");
            parsed.Credentials.UserName.Should().Be("bob");
            parsed.Credentials.Password.Should().Be("pa ss");
            parsed.MaxTime.Should().Be(12.5);
            parsed.ConnectTimeout.Should().Be(3);
            parsed.FollowLocation.Should().BeTrue();
            parsed.MaxRedirects.Should().Be(7);
            parsed.Insecure.Should().BeTrue();
            parsed.Proxy.Should().Be("http://proxy.local:3128");
            parsed.ProxyCredentials.UserName.Should().Be("alice");
            parsed.ProxyCredentials.Password.Should().Be("s3cret");
            parsed.Cookie.Should().Be("a=1; b=2");
            parsed.CookieJar.Should().Be(Path.Combine(_dir, "jar.txt"));
            parsed.OutputFile.Should().Be(Path.Combine(_dir, "out.bin"));
            parsed.IncludeHeaders.Should().BeTrue();
            parsed.Silent.Should().BeTrue();
            parsed.FailOnError.Should().BeTrue();
            parsed.HttpVersion.Should().Be("2.0");
            parsed.Compressed.Should().BeTrue();
            parsed.Range.Should().Be("0-99");
            parsed.TraceFile.Should().Be(trace);
            parsed.TraceTime.Should().BeTrue();
            command.Should().Contain("--data-binary '{\"name\":\"x\"}'");
        }

        [Theory]
        [InlineData("1.0", "--http1.0")]
        [InlineData("1.1", "--http1.1")]
        [InlineData("2", "--http2")]
        [InlineData("3.0", "--http3")]
        public void ToCurlCommand_HttpVersion_UsesCurlsFlag(string version, string flag)
        {
            CurlRequestBuilder.Get("https://example.com").WithHttpVersion(version).ToCurlCommand().Should().Contain(flag);
        }

        [Fact]
        public void ToCurlCommand_TraceAscii_IsIncluded()
        {
            CurlRequestBuilder.Get("https://example.com").WithTraceAscii("t.log").ToCurlCommand().Should().Contain("--trace-ascii t.log");
        }

        [Fact]
        public void ToCurlCommand_RealBinaryData_IsLeftOut()
        {
            var command = CurlRequestBuilder.Post("https://example.com").WithBinaryData(new byte[] { 0, 1, 2, 255 }).ToCurlCommand();
            command.Should().NotContain("--data-binary");
        }

        // ------------------------------------------------------------------ 3. AsJsonDynamic

        [Fact]
        public void AsJsonDynamic_NestedMemberAccess_Works()
        {
            var result = new CurlResult { StatusCode = 200, Body = "{\"rate\":{\"limit\":60,\"remaining\":59,\"reset\":1700000000123},\"ok\":true,\"name\":\"octo\",\"ratio\":0.5,\"none\":null}" };

            dynamic json = result.AsJsonDynamic();

            int remaining = json.rate.remaining;
            remaining.Should().Be(59);
            ((long)json.rate.reset).Should().Be(1700000000123L);
            ((bool)json.ok).Should().BeTrue();
            ((string)json.name).Should().Be("octo");
            ((string)json["name"]).Should().Be("octo");
            ((string)json.Name).Should().Be("octo", "a single case-insensitive match is used when the exact name is missing");
            ((double)json.ratio).Should().Be(0.5);
            ((object)json.none).Should().BeNull();
            ((object)json.missing).Should().BeNull();
        }

        [Fact]
        public void AsJsonDynamic_ArraysAreListsAndObjectsEnumerate()
        {
            var result = new CurlResult { StatusCode = 200, Body = "[{\"full_name\":\"a/b\"},{\"full_name\":\"c/d\"}]" };

            dynamic json = result.AsJsonDynamic();

            ((int)json.Count).Should().Be(2);
            ((string)json[1].full_name).Should().Be("c/d");
            var names = new List<string>();
            foreach (var repo in json)
            {
                names.Add((string)repo.full_name);
            }
            names.Should().Equal("a/b", "c/d");

            dynamic first = json[0];
            var keys = new List<string>();
            foreach (KeyValuePair<string, object> property in first)
            {
                keys.Add(property.Key);
            }
            keys.Should().Equal("full_name");
            ((string)first.ToString()).Should().Be("{\"full_name\":\"a/b\"}");
        }

        [Fact]
        public void AsJsonDynamic_EmptyBody_ThrowsParsingExceptionThatSaysWhatAndHowToFix()
        {
            var result = new CurlResult { StatusCode = 204, Body = "", Command = "curl -u user:pw https://api.example.com/v1/thing?token=abc" };

            Action act = () => result.AsJsonDynamic();

            var ex = act.Should().Throw<CurlParsingException>().Which;
            ex.Message.Should().Contain("https://api.example.com/v1/thing").And.Contain("204").And.Contain("See https://github.com/jacob-mellor/curl-dot-net/");
            ex.Message.Should().NotContain("token=abc").And.NotContain("user:pw");
            ex.Suggestions.Should().NotBeEmpty();
        }

        [Fact]
        public void AsJsonDynamic_HtmlBody_ThrowsParsingExceptionWithTheParserAsInner()
        {
            var result = new CurlResult { StatusCode = 200, Body = "<html>Login</html>", Headers = { ["Content-Type"] = "text/html" } };

            Action act = () => result.AsJsonDynamic();

            var ex = act.Should().Throw<CurlParsingException>().Which;
            ex.InnerException.Should().NotBeNull();
            ex.Message.Should().Contain("text/html").And.Contain("<html>Login");
        }

        // ------------------------------------------------------------------ 4. -D to an uncreatable path

        [Fact]
        public async Task DumpHeader_ToMissingDirectory_IsWriteError23EvenWhenTheServerSendsNothing()
        {
            using var server = new RawReplyServer(string.Empty);
            var target = Path.Combine(_dir, "no-such-dir", "deeper", "headers.txt");

            Func<Task> act = () => Curl.ExecuteAsync($"curl {server.Url()} -D {CommandParser.ShellQuote(target)}");

            var ex = (await act.Should().ThrowAsync<CurlWriteErrorException>()).Which;
            ex.CurlErrorCode.Should().Be(23);
            ex.Message.Should().Contain("-D/--dump-header").And.Contain("--create-dirs");
        }

        [Fact]
        public async Task DumpHeader_ToMissingDirectoryWithCreateDirs_Works()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 2\r\nX-Test: yes\r\n\r\nok");
            var target = Path.Combine(_dir, "made", "headers.txt");

            await Curl.ExecuteAsync($"curl --create-dirs {server.Url()} -D {CommandParser.ShellQuote(target)}");

            File.ReadAllText(target).Should().Contain("X-Test: yes");
        }

        // ------------------------------------------------------------------ 6. timings

        [Fact]
        public async Task Timings_PlainHttp_HaveNameLookupConnectAndNoTlsHandshake()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 2\r\n\r\nok");

            var result = await Curl.ExecuteAsync($"curl http://localhost:{server.Port}/");

            var t = result.Timings;
            t.Should().NotBeNull();
            t.NameLookup.Should().BeGreaterThan(0);
            t.Connect.Should().BeGreaterThan(0).And.BeGreaterOrEqualTo(t.NameLookup);
            t.AppConnect.Should().Be(0, "a plain http:// transfer has no TLS handshake, like curl's %{time_appconnect}");
            t.StartTransfer.Should().BeGreaterOrEqualTo(t.Connect);
            t.Total.Should().BeGreaterOrEqualTo(t.StartTransfer);
        }
    }
}
