using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using CurlDotNet;
using CurlDotNet.Core;
using CurlDotNet.Tests.TestServers;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Tests for the detailed trace logging feature (--trace / --trace-ascii / --trace-time)
    /// and the fluent WithTrace()/WithTraceAscii() builder methods.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    [Trait("Category", TestCategories.Http)]
    public class TraceLoggingTests : CurlTestBase
    {
        private readonly CommandParser _parser = new CommandParser();

        public TraceLoggingTests(ITestOutputHelper output) : base(output)
        {
        }

        #region Parsing

        [Theory]
        [Trait("Category", TestCategories.Parser)]
        [Trait("Category", TestCategories.Unit)]
        [InlineData("--trace")]
        public void Parse_Trace_ShouldSetTraceFile(string flag)
        {
            var options = _parser.Parse($"curl {flag} trace.log https://example.com");

            options.TraceFile.Should().Be("trace.log");
            options.TraceAsciiFile.Should().BeNull();
        }

        [Fact]
        [Trait("Category", TestCategories.Parser)]
        [Trait("Category", TestCategories.Unit)]
        public void Parse_TraceAscii_ShouldSetTraceAsciiFile()
        {
            var options = _parser.Parse("curl --trace-ascii trace.txt https://example.com");

            options.TraceAsciiFile.Should().Be("trace.txt");
            options.TraceFile.Should().BeNull();
        }

        [Fact]
        [Trait("Category", TestCategories.Parser)]
        [Trait("Category", TestCategories.Unit)]
        public void Parse_TraceTime_ShouldSetTraceTimeFlag()
        {
            var options = _parser.Parse("curl --trace trace.log --trace-time https://example.com");

            options.TraceFile.Should().Be("trace.log");
            options.TraceTime.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", TestCategories.Parser)]
        [Trait("Category", TestCategories.Unit)]
        public void Parse_TraceDash_ShouldTargetStandardOutput()
        {
            var options = _parser.Parse("curl --trace - https://example.com");

            options.TraceFile.Should().Be("-");
        }

        #endregion

        #region Builder

        [Fact]
        [Trait("Category", TestCategories.Unit)]
        public void Builder_WithTrace_ShouldSetTraceFile()
        {
            var builder = CurlRequestBuilder.Get("https://example.com")
                .WithTrace("out.log", includeTimestamps: true);

            var options = builder.GetOptions();

            options.TraceFile.Should().Be("out.log");
            options.TraceTime.Should().BeTrue();
        }

        [Fact]
        [Trait("Category", TestCategories.Unit)]
        public void Builder_WithTraceAscii_ShouldSetTraceAsciiFile()
        {
            var builder = CurlRequestBuilder.Get("https://example.com")
                .WithTraceAscii("out.txt");

            var options = builder.GetOptions();

            options.TraceAsciiFile.Should().Be("out.txt");
            options.TraceTime.Should().BeFalse();
        }

        #endregion

        #region End-to-end trace file output

        [Fact]
        [Trait("Category", TestCategories.Integration)]
        [Trait("OnlineRequired", "false")]
        public async Task Trace_GetRequest_ShouldWriteDetailedLogFile()
        {
            using var server = new LocalTestHttpServer();
            server.Start();
            await Task.Delay(50);

            var traceFile = Path.Combine(Path.GetTempPath(), $"curldotnet-trace-{Guid.NewGuid():N}.log");
            try
            {
                var command = $"curl --trace {traceFile} {server.BaseUrl}/get";

                var result = await Curl.ExecuteAsync(command);

                result.Should().NotBeNull();
                File.Exists(traceFile).Should().BeTrue("the trace file should be created");

                var trace = File.ReadAllText(traceFile);
                Output.WriteLine(trace);

                trace.Should().Contain("CurlDotNet trace started");
                trace.Should().Contain("Trying");
                trace.Should().Contain("=> Send header");
                trace.Should().Contain("<= Recv header");
                trace.Should().Contain("Transfer complete");
                // Confirm the request line and response status are captured.
                trace.Should().Contain("GET");
                trace.Should().Contain("HTTP/1.1 200");
            }
            finally
            {
                if (File.Exists(traceFile))
                {
                    File.Delete(traceFile);
                }
            }
        }

        [Fact]
        [Trait("Category", TestCategories.Integration)]
        [Trait("OnlineRequired", "false")]
        public async Task TraceAscii_ShouldOmitHexColumn()
        {
            using var server = new LocalTestHttpServer();
            server.Start();
            await Task.Delay(50);

            var traceFile = Path.Combine(Path.GetTempPath(), $"curldotnet-trace-ascii-{Guid.NewGuid():N}.log");
            try
            {
                var command = $"curl --trace-ascii {traceFile} {server.BaseUrl}/get";

                await Curl.ExecuteAsync(command);

                File.Exists(traceFile).Should().BeTrue();
                var trace = File.ReadAllText(traceFile);

                trace.Should().Contain("ascii (--trace-ascii)");
                trace.Should().Contain("=> Send header");
                // The "GET ..." dump line should start at offset 0000 with no hex bytes before the text.
                trace.Should().Contain("0000: GET");
            }
            finally
            {
                if (File.Exists(traceFile))
                {
                    File.Delete(traceFile);
                }
            }
        }

        [Fact]
        [Trait("Category", TestCategories.Integration)]
        [Trait("OnlineRequired", "false")]
        public async Task TraceTime_ShouldPrefixLinesWithTimestamps()
        {
            using var server = new LocalTestHttpServer();
            server.Start();
            await Task.Delay(50);

            var traceFile = Path.Combine(Path.GetTempPath(), $"curldotnet-trace-time-{Guid.NewGuid():N}.log");
            try
            {
                var command = $"curl --trace {traceFile} --trace-time {server.BaseUrl}/get";

                await Curl.ExecuteAsync(command);

                var firstLine = File.ReadLines(traceFile).First();
                // Timestamp format HH:mm:ss.ffffff (e.g., 14:03:27.123456)
                System.Text.RegularExpressions.Regex.IsMatch(firstLine, @"^\d{2}:\d{2}:\d{2}\.\d{6} ")
                    .Should().BeTrue("each line should be prefixed with a high-resolution timestamp");
            }
            finally
            {
                if (File.Exists(traceFile))
                {
                    File.Delete(traceFile);
                }
            }
        }

        #endregion
    }
}
