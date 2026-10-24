using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Transfer-side failures: each one says what happened (with host, port, file or limit), how to fix
    /// it and where to read more, keeps curl's wording on the first line, and is a CurlException with
    /// curl's exit code rather than a raw .NET exception.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class TransportMessageTests : IDisposable
    {
        private readonly string _dir;

        public TransportMessageTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "curldotnet-transport-msg-" + Guid.NewGuid().ToString("N"));
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

        private static string Q(string path) => CommandParser.ShellQuote(path);

        [Fact]
        public async Task MaxTime_FirstLineIsCurlsWordingAndTheFixNamesTheOption()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 100\r\n\r\nshort") { StallMilliseconds = 5000 };

            Func<Task> act = () => Curl.ExecuteAsync($"curl -m 0.3 {server.Url("/slow")}");

            var ex = (await act.Should().ThrowAsync<CurlOperationTimeoutException>()).Which;
            ex.CurlErrorCode.Should().Be(28);
            ex.WhatHappened.Should().StartWith("Operation timed out after 300 milliseconds with ").And.Contain("127.0.0.1");
            ExceptionMessageTests.AssertWhatFixLink(ex).Should().Contain("--max-time").And.Contain("--connect-timeout");
            ex.DocumentationUrl.Should().EndWith("timeout-errors.md");
        }

        [Fact]
        public async Task WeirdServerReply_NamesTheHostAndTheBrokenRule()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nThisLineHasNoColon\r\nContent-Length: 2\r\n\r\nok");

            Func<Task> act = () => Curl.ExecuteAsync($"curl {server.Url()}");

            var ex = (await act.Should().ThrowAsync<CurlWeirdServerReplyException>()).Which;
            ex.CurlErrorCode.Should().Be(8);
            ex.WhatHappened.Should().StartWith("Weird server reply from 127.0.0.1").And.Contain("ThisLineHasNoColon");
            ExceptionMessageTests.AssertWhatFixLink(ex).Should().Contain("--trace-ascii");
        }

        [Fact]
        public async Task Http09Reply_SaysHowToAllowIt()
        {
            using var server = new RawReplyServer("hello there, no status line");

            Func<Task> act = () => Curl.ExecuteAsync($"curl {server.Url()}");

            var ex = (await act.Should().ThrowAsync<CurlUnsupportedProtocolException>()).Which;
            ex.CurlErrorCode.Should().Be(1);
            ex.WhatHappened.Should().Contain("Received HTTP/0.9 when not allowed");
            ExceptionMessageTests.AssertWhatFixLink(ex).Should().Contain("--http0.9");
        }

        [Fact]
        public async Task EmptyReply_IsGotNothingWithAFix()
        {
            using var server = new RawReplyServer(string.Empty);

            Func<Task> act = () => Curl.ExecuteAsync($"curl {server.Url()}");

            var ex = (await act.Should().ThrowAsync<CurlGotNothingException>()).Which;
            ex.CurlErrorCode.Should().Be(52);
            ExceptionMessageTests.AssertWhatFixLink(ex).Should().Contain("127.0.0.1");
        }

        [Fact]
        public async Task AwsSigV4_WithoutSecretKey_IsBadArgument43NotArgumentNullException()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 2\r\n\r\nok");

            Func<Task> act = () => Curl.ExecuteAsync($"curl --aws-sigv4 aws:amz:us-east-1:s3 -u AKIDONLY: {server.Url("/bucket/key")}");

            var ex = (await act.Should().ThrowAsync<CurlBadFunctionArgumentException>()).Which;
            ex.CurlErrorCode.Should().Be(43);
            ExceptionMessageTests.AssertWhatFixLink(ex).Should().Contain("no secret key").And.Contain("-u ACCESS_KEY_ID:SECRET_ACCESS_KEY");
        }

        [Fact]
        public async Task UnsupportedScheme_FromOptions_KeepsTheProtocolAndDoesNotRepeatItself()
        {
            Func<Task> act = () => Curl.ExecuteAsync(new CurlOptions { Url = "gopher://example.com/1" });

            var ex = (await act.Should().ThrowAsync<CurlUnsupportedProtocolException>()).Which;
            ex.Protocol.Should().Be("gopher");
            ex.CurlErrorCode.Should().Be(1);
            var message = ExceptionMessageTests.AssertWhatFixLink(ex);
            message.Should().NotContain("Unsupported protocol: Unsupported protocol");
            message.Should().Contain("http://, https://, ftp://, ftps:// or file://");
        }

        [Fact]
        public async Task EmptyUrl_FromOptions_IsNoUrlSpecifiedWithExitCode2()
        {
            Func<Task> act = () => Curl.ExecuteAsync(new CurlOptions { Url = string.Empty });

            var ex = (await act.Should().ThrowAsync<CurlException>()).Which;
            ex.CurlErrorCode.Should().Be(2);
            ex.WhatHappened.Should().StartWith("No URL specified");
            ExceptionMessageTests.AssertWhatFixLink(ex);
        }

        [Fact]
        public async Task FileUrl_OutputToAFolder_IsWriteError23NamingTheOutputNotTheSource()
        {
            var source = Path.Combine(_dir, "source.txt");
            File.WriteAllText(source, "data");
            var folder = Path.Combine(_dir, "a-folder");
            Directory.CreateDirectory(folder);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -o {Q(folder)} {new Uri(source).AbsoluteUri}");

            var ex = (await act.Should().ThrowAsync<CurlWriteErrorException>()).Which;
            ex.CurlErrorCode.Should().Be(23);
            ex.FilePath.Should().Be(folder);
            ExceptionMessageTests.AssertWhatFixLink(ex).Should().Contain("-o/--output");
        }

        [Fact]
        public async Task HttpOutputToAFolder_IsWriteError23()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 2\r\n\r\nok");
            var folder = Path.Combine(_dir, "out-folder");
            Directory.CreateDirectory(folder);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -o {Q(folder)} {server.Url()}");

            var ex = (await act.Should().ThrowAsync<CurlWriteErrorException>()).Which;
            ex.CurlErrorCode.Should().Be(23);
            ExceptionMessageTests.AssertWhatFixLink(ex).Should().Contain("Failed writing").And.Contain(folder);
        }

        [Fact]
        public async Task CallerCancellation_IsOperationCanceledExceptionThatExplainsItself()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 100\r\n\r\nshort") { StallMilliseconds = 5000 };
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(300));

            Func<Task> act = () => Curl.ExecuteAsync($"curl {server.Url("/slow")}", cts.Token);

            var ex = (await act.Should().ThrowAsync<OperationCanceledException>()).Which;
            ex.Message.Should().Contain("cancelled by the CancellationToken").And.Contain("See https://github.com/jacob-mellor/curl-dot-net/");
        }

        [Fact]
        public async Task InvalidProxy_NeverPrintsTheProxyPassword()
        {
            Func<Task> act = () => Curl.ExecuteAsync("curl -x \"http://alice:hunter2@bad host:8080\" http://example.invalid/");

            var ex = (await act.Should().ThrowAsync<CurlCouldntResolveProxyException>()).Which;
            ex.CurlErrorCode.Should().Be(5);
            ex.Message.Should().NotContain("hunter2");
            ExceptionMessageTests.AssertWhatFixLink(ex).Should().Contain("host:port");
        }

        [Fact]
        public void FtpException_WithCommand_KeepsCommandCodeAndAddsFixAndLink()
        {
            var ex = new CurlDotNet.Core.CurlFtpException("The FTP server answered 550 (No such file) for ftp://example.com/x.csv", 550, "curl ftp://example.com/x.csv");

            ex.Command.Should().Be("curl ftp://example.com/x.csv");
            ex.FtpStatusCode.Should().Be(550);
            ex.CurlErrorCode.Should().Be(9);
            ExceptionMessageTests.AssertWhatFixLink(ex);
        }

        [Fact]
        public void AwsSigV4ConfigParse_Empty_ExplainsTheFormat()
        {
            Action act = () => AwsSigV4Config.Parse(" ");

            act.Should().Throw<CurlInvalidCommandException>().Which.Message
                .Should().Contain("aws:amz:us-east-1:s3").And.Contain("See https://github.com/jacob-mellor/curl-dot-net/");
        }
    }
}
