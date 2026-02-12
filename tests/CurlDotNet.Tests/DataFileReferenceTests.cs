using System;
using System.IO;
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
using Xunit.Abstractions;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Tests for @file reference resolution in -d, --json, --data-binary, and --data-raw.
    /// Verifies that the execution layer reads file contents when data starts with @,
    /// matching real curl behavior. Resolves GitHub Issue #32.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    [Trait("Category", TestCategories.Unit)]
    public class DataFileReferenceTests : CurlTestBase
    {
        public DataFileReferenceTests(ITestOutputHelper output) : base(output)
        {
        }

        #region Helper methods

        /// <summary>
        /// Creates a mock HTTP handler that captures the request body and returns 200 OK.
        /// </summary>
        private (HttpHandler handler, Mock<HttpMessageHandler> mock) CreateMockHandler()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("OK", Encoding.UTF8, "text/plain")
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var httpHandler = new HttpHandler(httpClient);
            return (httpHandler, handlerMock);
        }

        /// <summary>
        /// Extracts the request body string from a captured HttpRequestMessage.
        /// </summary>
        private async Task<string> CaptureRequestBodyAsync(Mock<HttpMessageHandler> mock)
        {
            HttpRequestMessage capturedRequest = null;
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("OK", Encoding.UTF8, "text/plain")
                });

            return capturedRequest?.Content != null
                ? await capturedRequest.Content.ReadAsStringAsync()
                : null;
        }

        #endregion

        #region -d @file resolution

        [Fact]
        public async Task Data_WithFileReference_ReadsFileContents()
        {
            // Arrange
            var fileContent = "{\"query\":\"DeviceProcessEvents | take 10\"}";
            var filePath = CreateTempFile(fileContent, ".json");

            var (handler, mock) = CreateMockHandler();
            HttpRequestMessage capturedRequest = null;
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("OK", Encoding.UTF8, "text/plain")
                });

            var options = new CurlOptions
            {
                Url = "https://api.example.com/data",
                Method = "POST",
                Data = $"@{filePath}"
            };

            // Act
            await handler.ExecuteAsync(options, CancellationToken.None);

            // Assert - the request body should contain the file contents, not the @path
            var body = await capturedRequest.Content.ReadAsStringAsync();
            body.Should().Contain(fileContent);
            body.Should().NotContain("@");
        }

        [Fact]
        public async Task Data_WithFileReference_ViaParser_ReadsFileContents()
        {
            // Arrange - full end-to-end: parse command then execute
            var fileContent = "name=test&value=123";
            var filePath = CreateTempFile(fileContent);

            var (handler, mock) = CreateMockHandler();
            HttpRequestMessage capturedRequest = null;
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("OK", Encoding.UTF8, "text/plain")
                });

            var parser = new CommandParser();
            var options = parser.Parse($"curl -d @\"{filePath}\" https://api.example.com");

            // Act
            await handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            var body = await capturedRequest.Content.ReadAsStringAsync();
            body.Should().Contain(fileContent);
        }

        #endregion

        #region --json @file resolution

        [Fact]
        public async Task Json_WithFileReference_ReadsFileContentsAndSetsHeaders()
        {
            // Arrange - this is the core fix for GitHub Issue #32
            var fileContent = "{\"query\":\"DeviceProcessEvents | take 10\"}";
            var filePath = CreateTempFile(fileContent, ".json");

            var (handler, mock) = CreateMockHandler();
            HttpRequestMessage capturedRequest = null;
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("OK", Encoding.UTF8, "text/plain")
                });

            var parser = new CommandParser();
            var options = parser.Parse($"curl --json @\"{filePath}\" https://api.example.com");

            // Act
            await handler.ExecuteAsync(options, CancellationToken.None);

            // Assert - file contents sent as body
            var body = await capturedRequest.Content.ReadAsStringAsync();
            body.Should().Contain(fileContent);
            body.Should().NotStartWith("@");

            // Assert - JSON headers set correctly by parser
            options.Headers["Content-Type"].Should().Be("application/json");
            options.Headers["Accept"].Should().Be("application/json");
        }

        [Fact]
        public async Task Json_WithFileReference_MassimosScenario_ReadsFileContents()
        {
            // Arrange - Massimo's exact scenario from Issue #32
            var queryContent = "{\"Query\":\"DeviceProcessEvents | take 10\"}";
            var filePath = CreateTempFile(queryContent, ".txt");

            var (handler, mock) = CreateMockHandler();
            HttpRequestMessage capturedRequest = null;
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"results\":[]}", Encoding.UTF8, "application/json")
                });

            var parser = new CommandParser();
            var options = parser.Parse(
                $"curl -X POST -H \"Authorization: Bearer xxx\" --json @\"{filePath}\" -L -k https://graph.microsoft.com/beta/security/runHuntingQuery");

            // Act
            await handler.ExecuteAsync(options, CancellationToken.None);

            // Assert - the query file content should be sent, not the @path
            var body = await capturedRequest.Content.ReadAsStringAsync();
            body.Should().Contain("DeviceProcessEvents");
            body.Should().NotContain("@");
        }

        #endregion

        #region --data-raw @file does NOT resolve

        [Fact]
        public async Task DataRaw_WithAtSign_DoesNotResolveFile()
        {
            // Arrange - --data-raw should send @ literally
            var (handler, mock) = CreateMockHandler();
            HttpRequestMessage capturedRequest = null;
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("OK", Encoding.UTF8, "text/plain")
                });

            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                Method = "POST",
                Data = "@somefile.txt",
                DataRaw = true
            };

            // Act
            await handler.ExecuteAsync(options, CancellationToken.None);

            // Assert - the @ should be sent literally
            var body = await capturedRequest.Content.ReadAsStringAsync();
            body.Should().Contain("@somefile.txt");
        }

        [Fact]
        public async Task DataRaw_ViaParser_DoesNotResolveFile()
        {
            // Arrange
            var (handler, mock) = CreateMockHandler();
            HttpRequestMessage capturedRequest = null;
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("OK", Encoding.UTF8, "text/plain")
                });

            var parser = new CommandParser();
            var options = parser.Parse("curl --data-raw @somefile.txt https://api.example.com");

            // Verify parser sets DataRaw
            options.DataRaw.Should().BeTrue();
            options.Data.Should().Be("@somefile.txt");

            // Act
            await handler.ExecuteAsync(options, CancellationToken.None);

            // Assert - @ treated literally
            var body = await capturedRequest.Content.ReadAsStringAsync();
            body.Should().Contain("@somefile.txt");
        }

        #endregion

        #region --data-binary @file resolution

        [Fact]
        public async Task DataBinary_WithFileReference_ReadsFileContents()
        {
            // Arrange
            var fileContent = "binary-like-content-here";
            var filePath = CreateTempFile(fileContent);

            var (handler, mock) = CreateMockHandler();
            HttpRequestMessage capturedRequest = null;
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("OK", Encoding.UTF8, "text/plain")
                });

            var parser = new CommandParser();
            var options = parser.Parse($"curl --data-binary @\"{filePath}\" https://api.example.com");

            // Verify parser does NOT set DataRaw for --data-binary
            options.DataRaw.Should().BeFalse();

            // Act
            await handler.ExecuteAsync(options, CancellationToken.None);

            // Assert - file contents sent
            var body = await capturedRequest.Content.ReadAsStringAsync();
            body.Should().Contain(fileContent);
        }

        #endregion

        #region Non-existent file throws exception

        [Fact]
        public async Task Data_WithNonExistentFile_ThrowsCurlFileCouldntReadException()
        {
            // Arrange
            var (handler, _) = CreateMockHandler();
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                Method = "POST",
                Data = "@/nonexistent/path/file.json"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CurlFileCouldntReadException>(
                () => handler.ExecuteAsync(options, CancellationToken.None));

            exception.FilePath.Should().Be("/nonexistent/path/file.json");
        }

        [Fact]
        public async Task Json_WithNonExistentFile_ThrowsCurlFileCouldntReadException()
        {
            // Arrange
            var (handler, _) = CreateMockHandler();
            var parser = new CommandParser();
            var options = parser.Parse("curl --json @missing.json https://api.example.com");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CurlFileCouldntReadException>(
                () => handler.ExecuteAsync(options, CancellationToken.None));

            exception.FilePath.Should().Be("missing.json");
        }

        #endregion

        #region Parser-level DataRaw flag tests

        [Fact]
        public void Parser_DataRaw_SetsDataRawFlag()
        {
            // Arrange
            var parser = new CommandParser();

            // Act
            var options = parser.Parse("curl --data-raw @file.txt https://api.example.com");

            // Assert
            options.DataRaw.Should().BeTrue();
            options.Data.Should().Be("@file.txt");
        }

        [Fact]
        public void Parser_Data_DoesNotSetDataRawFlag()
        {
            // Arrange
            var parser = new CommandParser();

            // Act
            var options = parser.Parse("curl -d @file.txt https://api.example.com");

            // Assert
            options.DataRaw.Should().BeFalse();
            options.Data.Should().Be("@file.txt");
        }

        [Fact]
        public void Parser_DataBinary_DoesNotSetDataRawFlag()
        {
            // Arrange
            var parser = new CommandParser();

            // Act
            var options = parser.Parse("curl --data-binary @file.txt https://api.example.com");

            // Assert
            options.DataRaw.Should().BeFalse();
            options.Data.Should().Be("@file.txt");
        }

        [Fact]
        public void Parser_Json_DoesNotSetDataRawFlag()
        {
            // Arrange
            var parser = new CommandParser();

            // Act
            var options = parser.Parse("curl --json @file.json https://api.example.com");

            // Assert
            options.DataRaw.Should().BeFalse();
            options.Data.Should().Be("@file.json");
        }

        #endregion

        #region Data without @ is unchanged

        [Fact]
        public async Task Data_WithoutAtSign_SendsDataDirectly()
        {
            // Arrange - plain data without @ should be sent as-is (no change in behavior)
            var (handler, mock) = CreateMockHandler();
            HttpRequestMessage capturedRequest = null;
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("OK", Encoding.UTF8, "text/plain")
                });

            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                Method = "POST",
                Data = "{\"key\":\"value\"}"
            };

            // Act
            await handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            var body = await capturedRequest.Content.ReadAsStringAsync();
            body.Should().Contain("{\"key\":\"value\"}");
        }

        #endregion
    }
}
