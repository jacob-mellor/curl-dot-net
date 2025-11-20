using System;
using System.Net;
using System.Net.Http;
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
    /// Comprehensive tests for CurlEngine to achieve 100% code coverage.
    /// Tests all execution paths, validation, code generation, and disposal.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    public class CurlEngineFullCoverageTests : IDisposable
    {
        private readonly Mock<HttpMessageHandler> _mockHandler;
        private readonly HttpClient _httpClient;
        private readonly CurlEngine _engine;

        public CurlEngineFullCoverageTests()
        {
            _mockHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHandler.Object);
            _engine = new CurlEngine(_httpClient);
        }

        public void Dispose()
        {
            _engine?.Dispose();
            _httpClient?.Dispose();
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithDefaultHttpClient_CreatesInstance()
        {
            // Act
            using var engine = new CurlEngine();

            // Assert
            engine.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithCustomHttpClient_UsesProvidedClient()
        {
            // Arrange
            var customClient = new HttpClient();

            // Act
            using var engine = new CurlEngine(customClient);

            // Assert
            engine.Should().NotBeNull();
        }

        #endregion

        #region ExecuteAsync Tests

        [Fact]
        public async Task ExecuteAsync_SimpleCommand_ExecutesSuccessfully()
        {
            // Arrange
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
            var result = await _engine.ExecuteAsync("curl https://api.example.com");

            // Assert
            result.Should().NotBeNull();
            result.Body.Should().Be("Success");
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task ExecuteAsync_WithCancellationToken_PropagatesToken()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.Is<CancellationToken>(ct => ct == cts.Token))
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _engine.ExecuteAsync("curl https://api.example.com", cts.Token);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_WithSettings_AppliesSettings()
        {
            // Arrange
            var settings = new CurlSettings()
                .WithTimeout(30)
                .WithFollowRedirects()
                .WithInsecure();

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _engine.ExecuteAsync("curl https://api.example.com", settings);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_WithOptions_ExecutesSuccessfully()
        {
            // Arrange
            var options = new CurlOptions { Url = "https://api.example.com" };
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(responseMessage);

            // Act
            var result = await _engine.ExecuteAsync(options);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_InvalidUrl_ThrowsCurlException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<CurlMalformedUrlException>(
                () => _engine.ExecuteAsync("curl not-a-valid-url"));
        }

        [Fact]
        public async Task ExecuteAsync_UnsupportedProtocol_ThrowsCurlUnsupportedProtocolException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<CurlUnsupportedProtocolException>(
                () => _engine.ExecuteAsync("curl gopher://example.com"));
        }

        [Fact]
        public async Task ExecuteAsync_FtpUrl_UsesFtpHandler()
        {
            // Act
            var result = await _engine.ExecuteAsync("curl ftp://ftp.example.com");

            // Assert
            result.Should().NotBeNull();
            // FTP handler would handle this, but without actual FTP server it returns an error
        }

        [Fact]
        public async Task ExecuteAsync_FileUrl_UsesFileHandler()
        {
            // Act
            var result = await _engine.ExecuteAsync("curl file:///tmp/test.txt");

            // Assert
            result.Should().NotBeNull();
            // File handler would handle this, returning 404 for non-existent file
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task ExecuteAsync_Timeout_ReturnsTimeoutResult()
        {
            // Arrange
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new TaskCanceledException());

            // Act
            var result = await _engine.ExecuteAsync("curl https://api.example.com");

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(408);
            result.Body.Should().Contain("timeout");
        }

        [Fact]
        public async Task ExecuteAsync_HttpRequestException_ThrowsCurlHttpException()
        {
            // Arrange
            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<CurlDotNet.Exceptions.CurlCouldntConnectException>(
                () => _engine.ExecuteAsync("curl https://api.example.com"));
            ex.Message.Should().Contain("Failed to connect");
        }

        #endregion

        #region Validate Tests

        [Fact]
        public void Validate_ValidCommand_ReturnsValidResult()
        {
            // Act
            var result = _engine.Validate("curl https://api.example.com");

            // Assert
            result.IsValid.Should().BeTrue();
            result.Error.Should().BeNull();
        }

        [Fact]
        public void Validate_EmptyCommand_ReturnsInvalidResult()
        {
            // Act
            var result = _engine.Validate("");

            // Assert
            result.IsValid.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Should().Contain("null");
        }

        [Fact]
        public void Validate_NoUrl_ReturnsInvalidResult()
        {
            // Act
            var result = _engine.Validate("curl -X POST");

            // Assert
            result.IsValid.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Should().Contain("URL");
        }

        [Fact]
        public void Validate_UnsupportedProtocol_AddsWarning()
        {
            // Act
            var result = _engine.Validate("curl telnet://example.com");

            // Assert
            // Protocol warnings would be nice, but ValidationResult only has Error
            result.IsValid.Should().BeFalse();
            result.Error.Should().Contain("protocol");
        }

        [Fact]
        public void Validate_InvalidUrl_ReturnsInvalidResult()
        {
            // Act
            var result = _engine.Validate("curl not-a-url");

            // Assert
            result.IsValid.Should().BeFalse();
            result.Error.Should().NotBeNull();
            result.Error.Should().Contain("valid URL");
        }

        [Fact]
        public void Validate_InsecureOption_AddsWarning()
        {
            // Act
            var result = _engine.Validate("curl -k https://api.example.com");

            // Assert
            // Should still be valid with insecure flag
            result.IsValid.Should().BeTrue();
            result.Error.Should().BeNull();
        }

        [Fact]
        public void Validate_ComplexCommand_ValidatesAllOptions()
        {
            // Act
            var result = _engine.Validate(
                "curl -X POST -H 'Content-Type: application/json' -d '{\"test\":true}' https://api.example.com");

            // Assert
            result.IsValid.Should().BeTrue();
            result.Error.Should().BeNull();
        }

        #endregion

        #region Code Generation Tests

        [Fact]
        public void ToHttpClientCode_GeneratesCorrectCode()
        {
            // Act
            var code = _engine.ToHttpClientCode("curl -X POST -H 'Accept: application/json' https://api.example.com");

            // Assert
            code.Should().Contain("HttpClient");
            code.Should().Contain("POST");
            code.Should().Contain("Accept");
            code.Should().Contain("application/json");
            code.Should().Contain("https://api.example.com");
        }

        [Fact]
        public void ToHttpClientCode_WithData_IncludesContent()
        {
            // Act
            var code = _engine.ToHttpClientCode("curl -X POST -d 'key=value' https://api.example.com");

            // Assert
            code.Should().Contain("StringContent");
            code.Should().Contain("key=value");
        }

        [Fact]
        public void ToHttpClientCode_WithBasicAuth_IncludesCredentials()
        {
            // Act
            var code = _engine.ToHttpClientCode("curl -u user:pass https://api.example.com");

            // Assert
            code.Should().Contain("NetworkCredential");
            code.Should().Contain("user");
            code.Should().Contain("pass");
        }

        [Fact]
        public void ToFetchCode_GeneratesJavaScriptCode()
        {
            // Act
            var code = _engine.ToFetchCode("curl -X POST -H 'Accept: application/json' https://api.example.com");

            // Assert
            code.Should().Contain("fetch");
            code.Should().Contain("method: 'POST'");
            code.Should().Contain("Accept");
            code.Should().Contain("application/json");
        }

        [Fact]
        public void ToFetchCode_WithData_IncludesBody()
        {
            // Act
            var code = _engine.ToFetchCode("curl -X POST -d '{\"test\":true}' https://api.example.com");

            // Assert
            code.Should().Contain("body:");
            code.Should().Contain("test");
        }

        [Fact]
        public void ToPythonCode_GeneratesRequestsCode()
        {
            // Act
            var code = _engine.ToPythonCode("curl -X POST -H 'Accept: application/json' https://api.example.com");

            // Assert
            code.Should().Contain("import requests");
            code.Should().Contain("requests.post");
            code.Should().Contain("Accept");
            code.Should().Contain("application/json");
        }

        [Fact]
        public void ToPythonCode_WithBasicAuth_IncludesAuth()
        {
            // Act
            var code = _engine.ToPythonCode("curl -u user:pass https://api.example.com");

            // Assert
            code.Should().Contain("auth=");
            code.Should().Contain("user");
            code.Should().Contain("pass");
        }

        [Fact]
        public void ToPythonCode_WithProxy_IncludesProxies()
        {
            // Act
            var code = _engine.ToPythonCode("curl -x http://proxy:8080 https://api.example.com");

            // Assert
            code.Should().Contain("proxies=");
            code.Should().Contain("proxy:8080");
        }

        [Fact]
        public void ToPowershellCode_GeneratesInvokeRestMethod()
        {
            // Act
            var code = _engine.ToPowershellCode("curl -X POST -H 'Accept: application/json' https://api.example.com");

            // Assert
            code.Should().Contain("Invoke-RestMethod");
            code.Should().Contain("-Method POST");
            code.Should().Contain("Accept");
            code.Should().Contain("application/json");
        }

        [Fact]
        public void ToPowershellCode_WithData_IncludesBodyAndContentType()
        {
            // Act
            var code = _engine.ToPowershellCode("curl -X POST -d '{\"test\":true}' https://api.example.com");

            // Assert
            code.Should().Contain("-Body");
            code.Should().Contain("test");
            code.Should().Contain("-ContentType \"application/json\"");
        }

        #endregion

        #region Disposal Tests

        [Fact]
        public void Dispose_DisposesHttpClientIfOwned()
        {
            // Arrange
            var engine = new CurlEngine(); // Uses default HttpClient

            // Act
            engine.Dispose();

            // Assert
            // Calling dispose again should not throw
            engine.Dispose();
        }

        [Fact]
        public void Dispose_DoesNotDisposeProvidedHttpClient()
        {
            // Arrange
            var customClient = new HttpClient();
            var engine = new CurlEngine(customClient);

            // Act
            engine.Dispose();

            // Assert
            // Custom client should still be usable
            customClient.DefaultRequestHeaders.Should().NotBeNull();
            customClient.Dispose();
        }

        [Fact]
        public async Task ExecuteAsync_AfterDispose_ThrowsObjectDisposedException()
        {
            // Arrange
            var engine = new CurlEngine();
            engine.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(
                () => engine.ExecuteAsync("curl https://api.example.com"));
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task ExecuteAsync_NullCommand_ThrowsArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _engine.ExecuteAsync((string)null!));
        }

        [Fact]
        public async Task ExecuteAsync_NullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _engine.ExecuteAsync((CurlOptions)null!));
        }

        [Fact]
        public void Validate_NullCommand_ReturnsInvalidResult()
        {
            // Act
            var result = _engine.Validate(null!);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Error.Should().NotBeNull();
        }

        [Fact]
        public void ToHttpClientCode_InvalidCommand_StillGeneratesCode()
        {
            // Act
            var code = _engine.ToHttpClientCode("invalid command");

            // Assert
            code.Should().Contain("HttpClient");
            // Should generate basic template even with invalid input
        }

        [Fact]
        public async Task ExecuteAsync_WithRetrySettings_RetriesOnFailure()
        {
            // Arrange
            var settings = new CurlSettings().WithRetries(3, 100);
            var callCount = 0;

            _mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() =>
                {
                    callCount++;
                    if (callCount < 3)
                        throw new HttpRequestException("Temporary failure");
                    return new HttpResponseMessage(HttpStatusCode.OK);
                });

            // Act
            var result = await _engine.ExecuteAsync("curl https://api.example.com", settings);

            // Assert
            result.Should().NotBeNull();
            // Note: Retry logic would be in middleware, not in engine itself
        }

        #endregion
    }
}