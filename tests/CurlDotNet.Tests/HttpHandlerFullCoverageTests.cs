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
    /// Comprehensive tests for HttpHandler to achieve 100% code coverage.
    /// Tests all HTTP request building, response handling, redirects, and edge cases.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    public class HttpHandlerFullCoverageTests : IDisposable
    {
        private readonly Mock<HttpMessageHandler> _mockHandler;
        private readonly HttpClient _httpClient;
        private readonly HttpHandler _handler;

        public HttpHandlerFullCoverageTests()
        {
            _mockHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHandler.Object);
            _handler = new HttpHandler(_httpClient);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithDefaultHttpClient_CreatesInstance()
        {
            // Act
            var handler = new HttpHandler();

            // Assert
            handler.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithCustomHttpClient_UsesProvidedClient()
        {
            // Arrange
            var customClient = new HttpClient();

            // Act
            var handler = new HttpHandler(customClient);

            // Assert
            handler.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullHttpClient_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new HttpHandler(null));
        }

        #endregion

        #region Protocol Support Tests

        [Fact]
        public void SupportsProtocol_Http_ReturnsTrue()
        {
            // Act
            var result = _handler.SupportsProtocol("http");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void SupportsProtocol_Https_ReturnsTrue()
        {
            // Act
            var result = _handler.SupportsProtocol("https");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void SupportsProtocol_Ftp_ReturnsFalse()
        {
            // Act
            var result = _handler.SupportsProtocol("ftp");

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region Basic Request Tests

        [Fact]
        public async Task ExecuteAsync_SimpleGetRequest_ExecutesSuccessfully()
        {
            // Arrange
            var options = new CurlOptions { Url = "https://api.example.com" };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Success")
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Body.Should().Be("Success");
        }

        [Fact]
        public async Task ExecuteAsync_PostRequest_SendsCorrectMethod()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                Method = "POST",
                Data = "test=value"
            };
            HttpRequestMessage capturedRequest = null;

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest.Method.Should().Be(HttpMethod.Post);
        }

        [Fact]
        public async Task ExecuteAsync_CustomMethod_UsesCustomMethod()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                CustomMethod = "PATCH"
            };
            HttpRequestMessage capturedRequest = null;

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            capturedRequest.Method.Method.Should().Be("PATCH");
        }

        #endregion

        #region Header Tests

        [Fact]
        public async Task ExecuteAsync_WithCustomHeaders_AddsHeaders()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                Headers = new Dictionary<string, string>
                {
                    ["X-Custom-Header"] = "CustomValue",
                    ["Accept"] = "application/json"
                }
            };
            HttpRequestMessage capturedRequest = null;

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            capturedRequest.Headers.GetValues("X-Custom-Header").Should().Contain("CustomValue");
            capturedRequest.Headers.GetValues("Accept").Should().Contain("application/json");
        }

        [Fact]
        public async Task ExecuteAsync_WithUserAgent_SetsUserAgent()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                UserAgent = "MyApp/1.0"
            };
            HttpRequestMessage capturedRequest = null;

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            capturedRequest.Headers.UserAgent.ToString().Should().Contain("MyApp/1.0");
        }

        [Fact]
        public async Task ExecuteAsync_WithReferer_SetsReferer()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                Referer = "https://example.com"
            };
            HttpRequestMessage capturedRequest = null;

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            capturedRequest.Headers.Referrer.Should().NotBeNull();
            capturedRequest.Headers.Referrer.ToString().Should().Be("https://example.com/");
        }

        [Fact]
        public async Task ExecuteAsync_WithCookie_SetsCookieHeader()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                Cookie = "session=abc123; user=john"
            };
            HttpRequestMessage capturedRequest = null;

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            capturedRequest.Headers.GetValues("Cookie").Should().Contain("session=abc123; user=john");
        }

        [Fact]
        public async Task ExecuteAsync_WithBasicAuth_SetsAuthorizationHeader()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                Credentials = new NetworkCredential("user", "pass")
            };
            HttpRequestMessage capturedRequest = null;

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            capturedRequest.Headers.Authorization.Should().NotBeNull();
            capturedRequest.Headers.Authorization.Scheme.Should().Be("Basic");
            // user:pass in base64 = dXNlcjpwYXNz
            capturedRequest.Headers.Authorization.Parameter.Should().Be("dXNlcjpwYXNz");
        }

        [Fact]
        public async Task ExecuteAsync_WithRange_SetsRangeHeader()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                Range = "0-499"
            };
            HttpRequestMessage capturedRequest = null;

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.PartialContent));

            // Act
            await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            capturedRequest.Headers.Range.Should().NotBeNull();
            capturedRequest.Headers.Range.Ranges.Should().HaveCount(1);
        }

        [Fact]
        public async Task ExecuteAsync_WithCompressed_SetsAcceptEncodingHeader()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                Compressed = true
            };
            HttpRequestMessage capturedRequest = null;

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            capturedRequest.Headers.TryGetValues("Accept-Encoding", out var values).Should().BeTrue();
            values.Should().Contain(v => v.Contains("gzip") && v.Contains("deflate"));
        }

        #endregion

        #region Content Tests

        [Fact]
        public async Task ExecuteAsync_WithTextData_SendsStringContent()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                Method = "POST",
                Data = "key=value&foo=bar"
            };
            HttpRequestMessage capturedRequest = null;

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            capturedRequest.Content.Should().NotBeNull();
            var content = await capturedRequest.Content.ReadAsStringAsync();
            content.Should().Be("key=value&foo=bar");
            capturedRequest.Content.Headers.ContentType.MediaType.Should().Be("application/x-www-form-urlencoded");
        }

        [Fact]
        public async Task ExecuteAsync_WithJsonData_SendsJsonContent()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                Method = "POST",
                Data = "{\"name\":\"John\"}",
                Headers = new Dictionary<string, string> { ["Content-Type"] = "application/json" }
            };
            HttpRequestMessage capturedRequest = null;

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            capturedRequest.Content.Headers.ContentType.MediaType.Should().Be("application/json");
            var content = await capturedRequest.Content.ReadAsStringAsync();
            content.Should().Be("{\"name\":\"John\"}");
        }

        [Fact]
        public async Task ExecuteAsync_WithBinaryData_SendsByteArrayContent()
        {
            // Arrange
            var binaryData = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                Method = "POST",
                BinaryData = binaryData
            };
            HttpRequestMessage capturedRequest = null;

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            capturedRequest.Content.Should().NotBeNull();
            var content = await capturedRequest.Content.ReadAsByteArrayAsync();
            content.Should().BeEquivalentTo(binaryData);
        }

        [Fact]
        public async Task ExecuteAsync_WithFormData_SendsMultipartContent()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                Method = "POST",
                FormData = new Dictionary<string, string>
                {
                    ["field1"] = "value1",
                    ["field2"] = "value2"
                }
            };
            HttpRequestMessage capturedRequest = null;

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            capturedRequest.Content.Should().BeOfType<MultipartFormDataContent>();
            var content = await capturedRequest.Content.ReadAsStringAsync();
            content.Should().Contain("value1");
            content.Should().Contain("value2");
        }

        [Fact]
        public async Task ExecuteAsync_WithFiles_SendsMultipartWithFiles()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "test file content");

            try
            {
                var options = new CurlOptions
                {
                    Url = "https://api.example.com",
                    Method = "POST",
                    Files = new Dictionary<string, string>
                    {
                        ["file"] = tempFile
                    }
                };
                HttpRequestMessage capturedRequest = null;

                _mockHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .Callback<HttpRequestMessage, CancellationToken>((req, ct) => capturedRequest = req)
                    .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

                // Act
                await _handler.ExecuteAsync(options, CancellationToken.None);

                // Assert
                capturedRequest.Content.Should().BeOfType<MultipartFormDataContent>();
                var content = await capturedRequest.Content.ReadAsStringAsync();
                content.Should().Contain("test file content");
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        #endregion

        #region Response Handling Tests

        [Fact]
        public async Task ExecuteAsync_WithIncludeHeaders_IncludesHeadersInBody()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                IncludeHeaders = true
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Response body"),
                ReasonPhrase = "OK"
            };
            responseMessage.Headers.Add("X-Custom", "Value");

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            result.Body.Should().Contain("HTTP/");
            result.Body.Should().Contain("200 OK");
            result.Body.Should().Contain("X-Custom: Value");
            result.Body.Should().Contain("Response body");
        }

        [Fact]
        public async Task ExecuteAsync_WithVerbose_IncludesVerboseOutput()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                Verbose = true
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Response body")
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            result.Body.Should().Contain("*   Trying");
            result.Body.Should().Contain("* Connected to");
            result.Body.Should().Contain("> GET");
            result.Body.Should().Contain("< HTTP/");
        }

        [Fact]
        public async Task ExecuteAsync_WithWriteOut_FormatsWriteOutVariables()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                WriteOut = "Status: %{http_code}\\nSize: %{size_download}\\nURL: %{url_effective}\\nType: %{content_type}"
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Response body", Encoding.UTF8, "text/plain")
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            result.Body.Should().Contain("Status: 200");
            result.Body.Should().Contain($"Size: {Encoding.UTF8.GetByteCount("Response body")}");
            result.Body.Should().Contain("URL: https://api.example.com");
            result.Body.Should().Contain("Type: text/plain");
        }

        [Fact]
        public async Task ExecuteAsync_WithBinaryResponse_StoresBinaryData()
        {
            // Arrange
            var options = new CurlOptions { Url = "https://api.example.com" };
            var binaryContent = new byte[] { 0xFF, 0xFE, 0xFD, 0xFC };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(binaryContent)
            };
            responseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            result.BinaryData.Should().BeEquivalentTo(binaryContent);
            result.Body.Should().BeEmpty();
        }

        #endregion

        #region Output File Tests

        [Fact]
        public async Task ExecuteAsync_WithOutputFile_WritesResponseToFile()
        {
            // Arrange
            var outputFile = Path.GetTempFileName();
            try
            {
                var options = new CurlOptions
                {
                    Url = "https://api.example.com",
                    OutputFile = outputFile
                };
                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("File content")
                };

                _mockHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .ReturnsAsync(responseMessage);

                // Act
                var result = await _handler.ExecuteAsync(options, CancellationToken.None);

                // Assert
                result.OutputFiles.Should().Contain(outputFile);
                File.ReadAllText(outputFile).Should().Be("File content");
            }
            finally
            {
                File.Delete(outputFile);
            }
        }

        [Fact]
        public async Task ExecuteAsync_WithUseRemoteFileName_UsesFileNameFromUrl()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com/document.pdf",
                UseRemoteFileName = true
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(new byte[] { 0x01, 0x02, 0x03 })
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            result.OutputFiles.Should().HaveCount(1);
            result.OutputFiles.First().Should().EndWith("document.pdf");

            // Cleanup
            foreach (var file in result.OutputFiles)
            {
                if (File.Exists(file))
                    File.Delete(file);
            }
        }

        #endregion

        #region Redirect Tests

        [Fact]
        public async Task ExecuteAsync_WithRedirect_FollowsRedirect()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                FollowLocation = true
            };

            var redirectResponse = new HttpResponseMessage(HttpStatusCode.Redirect);
            redirectResponse.Headers.Location = new Uri("https://api.example.com/redirected");

            var finalResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Final content")
            };

            var callCount = 0;
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() =>
                {
                    callCount++;
                    return callCount == 1 ? redirectResponse : finalResponse;
                });

            // Act
            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            result.StatusCode.Should().Be(200);
            result.Body.Should().Be("Final content");
            callCount.Should().Be(2);
        }

        [Fact]
        public async Task ExecuteAsync_WithTooManyRedirects_ThrowsCurlTooManyRedirectsException()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                FollowLocation = true,
                MaxRedirects = 2
            };

            var redirectResponse = new HttpResponseMessage(HttpStatusCode.Redirect);
            redirectResponse.Headers.Location = new Uri("https://api.example.com/loop");

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(redirectResponse);

            // Act & Assert
            await Assert.ThrowsAsync<CurlTooManyRedirectsException>(
                () => _handler.ExecuteAsync(options, CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteAsync_WithRelativeRedirect_ResolvesUrl()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com/path",
                FollowLocation = true
            };

            var redirectResponse = new HttpResponseMessage(HttpStatusCode.Redirect);
            redirectResponse.Headers.Location = new Uri("/newpath", UriKind.Relative);

            var finalResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Final content")
            };

            var callCount = 0;
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() =>
                {
                    callCount++;
                    return callCount == 1 ? redirectResponse : finalResponse;
                });

            // Act
            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task ExecuteAsync_WithRedirectAndIncludeHeaders_IncludesAllHeaders()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                FollowLocation = true,
                IncludeHeaders = true
            };

            var redirectResponse = new HttpResponseMessage(HttpStatusCode.Redirect);
            redirectResponse.Headers.Location = new Uri("https://api.example.com/redirected");
            redirectResponse.Headers.Add("X-Redirect", "true");

            var finalResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Final content")
            };
            finalResponse.Headers.Add("X-Final", "true");

            var callCount = 0;
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() =>
                {
                    callCount++;
                    return callCount == 1 ? redirectResponse : finalResponse;
                });

            // Act
            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            result.Body.Should().Contain("HTTP/");
            result.Body.Should().Contain("302");
            result.Body.Should().Contain("X-Redirect: true");
            result.Body.Should().Contain("200");
            result.Body.Should().Contain("X-Final: true");
            result.Body.Should().Contain("Final content");
        }

        #endregion

        #region Timeout and Cancellation Tests

        [Fact]
        public async Task ExecuteAsync_WithTimeout_ThrowsTimeoutException()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                MaxTime = 1 // 1 second timeout
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new TaskCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<CurlOperationTimeoutException>(
                () => _handler.ExecuteAsync(options, CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteAsync_WithCancellation_ThrowsCurlAbortedByCallbackException()
        {
            // Arrange
            var options = new CurlOptions { Url = "https://api.example.com" };
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new TaskCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<CurlAbortedByCallbackException>(
                () => _handler.ExecuteAsync(options, cts.Token));
        }

        [Fact]
        public async Task ExecuteAsync_WithHttpRequestException_ThrowsCurlCouldntConnectException()
        {
            // Arrange
            var options = new CurlOptions { Url = "https://api.example.com:8080" };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Connection failed"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<CurlCouldntConnectException>(
                () => _handler.ExecuteAsync(options, CancellationToken.None));
            ex.Host.Should().Be("api.example.com");
            ex.Port.Should().Be(8080);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task ExecuteAsync_WithHeadRequest_DoesNotIncludeBody()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                Method = "HEAD"
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Should not be included")
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            result.Body.Should().BeEmpty();
        }

        [Fact]
        public async Task ExecuteAsync_WithHeadOnly_DoesNotReadBody()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                HeadOnly = true
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Should not be included")
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            result.Body.Should().BeEmpty();
        }

        [Fact]
        public async Task ExecuteAsync_WithNullMaxTime_UsesDefaultTimeout()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                MaxTime = null
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task ExecuteAsync_WithEmptyWriteOut_DoesNotAddToBody()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://api.example.com",
                WriteOut = ""
            };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Body content")
            };

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _handler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            result.Body.Should().Be("Body content");
        }

        #endregion
    }
}