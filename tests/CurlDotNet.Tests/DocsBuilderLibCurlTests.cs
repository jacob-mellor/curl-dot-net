using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;
using CurlDotNet.Lib;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Behaviour promised by the XML docs of <see cref="CurlRequestBuilder"/> and <see cref="LibCurl"/>, and the
    /// what / how-to-fix / link messages of their argument checks.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class DocsBuilderLibCurlTests
    {
        private const string Ok = "HTTP/1.1 200 OK\r\nContent-Length: 2\r\n\r\nok";
        private const string DocsLink = "See https://github.com/jacob-mellor/curl-dot-net/";

        private static string Line(string request, string header) =>
            request.Split(new[] { "\r\n" }, StringSplitOptions.None)
                .FirstOrDefault(l => l.StartsWith(header + ":", StringComparison.OrdinalIgnoreCase));

        // ------------------------------------------------------------------ LibCurl transport is curl's

        [Fact]
        public async Task LibCurl_WithFollowRedirects_FollowsToTheTarget()
        {
            using var target = new RawReplyServer(Ok);
            using var redirect = new RawReplyServer($"HTTP/1.1 302 Found\r\nLocation: {target.Url("/t")}\r\nContent-Length: 0\r\n\r\n");
            using var curl = new LibCurl().WithFollowRedirects();

            var result = await curl.GetAsync(redirect.Url("/r"));

            result.StatusCode.Should().Be(200);
            result.Body.Should().Be("ok");
        }

        [Fact]
        public async Task LibCurl_TransferFailure_IsReturnedInResultExceptionNotThrown()
        {
            var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
            listener.Start();
            var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            using var curl = new LibCurl();

            var result = await curl.GetAsync($"http://127.0.0.1:{port}/");

            result.StatusCode.Should().Be(0);
            result.Exception.Should().BeAssignableTo<CurlException>();
            ((CurlException)result.Exception).CurlErrorCode.Should().Be(7);
            result.Exception.Message.Should().Contain(DocsLink);
        }

        [Fact]
        public async Task LibCurl_PerformAsyncNull_ExplainsWhatAndHow()
        {
            using var curl = new LibCurl();

            Func<Task> act = () => curl.PerformAsync(null!);

            var ex = (await act.Should().ThrowAsync<ArgumentNullException>()).Which;
            ex.ParamName.Should().Be("options");
            ex.Message.Should().Contain("PerformAsync").And.Contain("Url").And.Contain(DocsLink);
        }

        [Fact]
        public async Task LibCurl_Cancellation_ThrowsOperationCanceledWithExplanation()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 100\r\n\r\nshort") { StallMilliseconds = 5000 };
            using var curl = new LibCurl();
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));

            Func<Task> act = () => curl.GetAsync(server.Url(), null, cts.Token);

            var ex = (await act.Should().ThrowAsync<OperationCanceledException>()).Which;
            ex.CancellationToken.Should().Be(cts.Token);
        }

        // ------------------------------------------------------------------ builder behaviour and messages

        [Fact]
        public void Builder_WithFileMissing_IsReadError26NamingPathFieldAndFix()
        {
            Action act = () => CurlRequestBuilder.Post("https://api.example.com").WithFile("document", "nonexistent.txt");

            var ex = act.Should().Throw<CurlReadErrorException>().Which;
            ex.CurlErrorCode.Should().Be(26);
            ex.Message.Should().Contain("nonexistent.txt").And.Contain("\"document\"").And.Contain("absolute path").And.Contain(DocsLink);
            ex.Suggestions.Should().NotBeEmpty();
        }

        [Fact]
        public void Builder_NullUrl_ExplainsWhichFactoryAndHowToFix()
        {
            Action act = () => CurlRequestBuilder.Post(null!);

            var ex = act.Should().Throw<ArgumentNullException>().Which;
            ex.ParamName.Should().Be("url");
            ex.Message.Should().Contain("CurlRequestBuilder.Post(url)").And.Contain("https://").And.Contain(DocsLink);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("GET STUFF")]
        public void Builder_RequestWithBadMethod_ExplainsTheMethodRule(string method)
        {
            Action act = () => CurlRequestBuilder.Request(method!, "https://api.example.com/x?token=secret");

            var ex = act.Should().Throw<ArgumentException>().Which;
            ex.ParamName.Should().Be("method");
            ex.Message.Should().Contain("single non-empty word").And.Contain(DocsLink).And.NotContain("token=secret");
        }

        [Fact]
        public void Builder_WithHeadersNull_ExplainsHowToFix()
        {
            Action act = () => CurlRequestBuilder.Get("https://example.com").WithHeaders(null!);

            act.Should().Throw<ArgumentNullException>().Which.Message.Should().Contain("empty dictionary").And.Contain(DocsLink);
        }

        [Fact]
        public void Builder_HttpVersionOne_PrintsHttp10()
        {
            CurlRequestBuilder.Get("https://example.com").WithHttpVersion("1").ToCurlCommand().Should().Contain("--http1.0");
        }

        [Fact]
        public void Builder_GetOptions_CopiesEveryBuilderSetting()
        {
            var options = CurlRequestBuilder.Request("PURGE", "https://cdn.example.com/a")
                .WithTrace("t.log", includeTimestamps: true).WithTraceAscii("a.log").WithHttpVersion("2").WithRange("0-9")
                .AsBinary().WithBinaryContentType("application/x-a").WithTextContentType("application/x-b")
                .WithProxy("http://p:1", "u", "p").WithReferer("https://r/").GetOptions();

            options.Method.Should().Be("PURGE");
            options.TraceFile.Should().Be("t.log");
            options.TraceAsciiFile.Should().Be("a.log");
            options.TraceTime.Should().BeTrue();
            options.HttpVersion.Should().Be("2");
            options.Range.Should().Be("0-9");
            options.ForceBinary.Should().BeTrue();
            options.BinaryContentTypes.Should().Contain("application/x-a");
            options.TextContentTypes.Should().Contain("application/x-b");
            options.ProxyCredentials.UserName.Should().Be("u");
            options.Referer.Should().Be("https://r/");
        }
    }
}
