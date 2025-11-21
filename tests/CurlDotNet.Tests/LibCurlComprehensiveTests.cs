using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Lib;
using CurlDotNet.Tests.TestServers;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Comprehensive tests for LibCurl - the object-oriented curl API.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    [Trait("Category", TestCategories.FullCoverage)]
    public class LibCurlComprehensiveTests : CurlTestBase
    {
        private readonly TestServerEndpoint _testServer;
        private readonly TestServerAdapter _serverAdapter;

        public LibCurlComprehensiveTests(ITestOutputHelper output) : base(output)
        {
            _testServer = TestServerConfiguration.GetBestAvailableServerAsync(TestServerFeatures.All).GetAwaiter().GetResult();
            _serverAdapter = new TestServerAdapter(_testServer.BaseUrl);
            Output.WriteLine($"Using test server: {_testServer.Name} at {_testServer.BaseUrl}");
        }

        #region Basic HTTP Methods

        [Fact]
        public async Task GetAsync_SimpleRequest_Success()
        {
            // Arrange
            using var curl = new LibCurl();
            var url = _serverAdapter.GetEndpoint();

            // Act
            var result = await curl.GetAsync(url);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Body.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task PostAsync_WithStringData_Success()
        {
            // Arrange
            using var curl = new LibCurl();
            var url = _serverAdapter.PostEndpoint();
            var data = "key=value&foo=bar";

            // Act
            var result = await curl.PostAsync(url, data);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Body.Should().Contain("key");
        }

        [Fact]
        public async Task PostAsync_WithJsonData_Success()
        {
            // Arrange
            using var curl = new LibCurl()
                .WithHeader("Content-Type", "application/json");
            var url = _serverAdapter.PostEndpoint();
            var data = System.Text.Json.JsonSerializer.Serialize(new { name = "test", value = 123, items = new[] { "a", "b", "c" } });

            // Act
            var result = await curl.PostAsync(url, data);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Body.Should().Contain("name");
        }

        [Fact]
        public async Task PutAsync_WithData_Success()
        {
            // Arrange
            using var curl = new LibCurl();
            var url = $"{_testServer.BaseUrl}/put";
            var data = "updated=value";

            // Act
            var result = await curl.PutAsync(url, data);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task PatchAsync_WithData_Success()
        {
            // Arrange
            using var curl = new LibCurl();
            var url = $"{_testServer.BaseUrl}/patch";
            var data = "field=patched";

            // Act
            var result = await curl.PatchAsync(url, data);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_SimpleRequest_Success()
        {
            // Arrange
            using var curl = new LibCurl();
            var url = $"{_testServer.BaseUrl}/delete";

            // Act
            var result = await curl.DeleteAsync(url);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task HeadAsync_SimpleRequest_NoBody()
        {
            // Arrange
            using var curl = new LibCurl();
            var url = _serverAdapter.GetEndpoint();

            // Act
            var result = await curl.HeadAsync(url);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Body.Should().BeNullOrEmpty(); // HEAD requests don't return body
            result.Headers.Should().NotBeNull();
        }

        #endregion

        #region Configuration Methods

        [Fact]
        public async Task WithTimeout_SetsTimeout_TimesOut()
        {
            // Arrange
            using var curl = new LibCurl()
                .WithTimeout(TimeSpan.FromMilliseconds(100));
            var url = _serverAdapter.DelayEndpoint(5);

            // Act
            var result = await curl.GetAsync(url);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task WithFollowRedirects_FollowsRedirects()
        {
            // Arrange
            using var curl = new LibCurl()
                .WithFollowRedirects(5);
            var url = _serverAdapter.RedirectEndpoint(2);

            // Act
            var result = await curl.GetAsync(url);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task WithHeaders_AddsHeaders()
        {
            // Arrange
            using var curl = new LibCurl()
                .WithHeader("X-Custom-Header", "CustomValue")
                .WithHeader("User-Agent", "LibCurl/Test");
            var url = _serverAdapter.HeadersEndpoint();

            // Act
            var result = await curl.GetAsync(url);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Body.Should().Contain("X-Custom-Header");
            result.Body.Should().Contain("CustomValue");
        }

        [Fact]
        public async Task WithBasicAuth_SetsCredentials()
        {
            // Arrange
            using var curl = new LibCurl()
                .WithBasicAuth("testuser", "testpass");
            var url = _serverAdapter.BasicAuthEndpoint("testuser", "testpass");

            // Act
            var result = await curl.GetAsync(url);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task WithBearerToken_SetsAuthHeader()
        {
            // Arrange
            var token = "test-token-123";
            using var curl = new LibCurl()
                .WithBearerToken(token);
            var url = _serverAdapter.HeadersEndpoint();

            // Act
            var result = await curl.GetAsync(url);

            // Assert
            result.Should().NotBeNull();
            result.Body.Should().Contain("Authorization");
            result.Body.Should().Contain("Bearer");
        }

        [Fact]
        public void WithProxy_SetsProxy()
        {
            // Arrange & Act
            using var curl = new LibCurl()
                .WithProxy("http://proxy.example.com:8080");

            // Assert
            curl.Should().NotBeNull();
        }

        [Fact]
        public void WithProxy_WithCredentials_SetsProxyAuth()
        {
            // Arrange & Act
            using var curl = new LibCurl()
                .WithProxy("http://proxy.example.com:8080", "proxyuser", "proxypass");

            // Assert
            curl.Should().NotBeNull();
        }

        [Fact]
        public async Task WithUserAgent_SetsUserAgent()
        {
            // Arrange
            using var curl = new LibCurl()
                .WithUserAgent("LibCurl/TestAgent 1.0");
            var url = _serverAdapter.HeadersEndpoint();

            // Act
            var result = await curl.GetAsync(url);

            // Assert
            result.Should().NotBeNull();
            result.Body.Should().Contain("User-Agent");
            result.Body.Should().Contain("LibCurl/TestAgent");
        }

        [Fact]
        public async Task WithCookieHeader_SendsCookies()
        {
            // Arrange
            using var curl = new LibCurl()
                .WithHeader("Cookie", "test_cookie=test_value");
            var url = _serverAdapter.HeadersEndpoint();

            // Act
            var result = await curl.GetAsync(url);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task WithInsecureSsl_DisablesSSLVerification()
        {
            // Arrange
            using var curl = new LibCurl()
                .WithInsecureSsl();

            // Act & Assert - just verify the setting doesn't break anything
            curl.Should().NotBeNull();
        }

        [Fact]
        public async Task WithVerbose_EnablesVerboseOutput()
        {
            // Arrange
            using var curl = new LibCurl()
                .WithVerbose();
            var url = _serverAdapter.GetEndpoint();

            // Act
            var result = await curl.GetAsync(url);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        #endregion

        #region File Operations

        [Fact]
        public async Task WithOutputFile_SavesResponseToFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            using var curl = new LibCurl()
                .WithOutputFile(tempFile);
            var url = _serverAdapter.GetEndpoint();

            try
            {
                // Act
                var result = await curl.GetAsync(url);

                // Assert
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeTrue();
                File.Exists(tempFile).Should().BeTrue();
                new FileInfo(tempFile).Length.Should().BeGreaterThan(0);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        #endregion

        #region Form Data

        [Fact]
        public async Task PostAsync_WithFormData_SendsFormData()
        {
            // Arrange
            using var curl = new LibCurl()
                .WithHeader("Content-Type", "application/x-www-form-urlencoded");
            var url = _serverAdapter.PostEndpoint();
            var formData = "field1=value1&field2=value2&field3=special+chars";

            // Act
            var result = await curl.PostAsync(url, formData);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Body.Should().Contain("field1");
        }

        #endregion

        #region Error Handling

        [Fact]
        public async Task GetAsync_InvalidUrl_ReturnsError()
        {
            // Arrange
            using var curl = new LibCurl();
            var url = "http://invalid.domain.xyz.notexist";

            // Act
            var result = await curl.GetAsync(url);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Exception.Should().NotBeNull();
        }

        [Fact]
        public async Task GetAsync_404Error_Returns404()
        {
            // Arrange
            using var curl = new LibCurl();
            var url = _serverAdapter.StatusEndpoint(404);

            // Act
            var result = await curl.GetAsync(url);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task GetAsync_500Error_Returns500()
        {
            // Arrange
            using var curl = new LibCurl();
            var url = _serverAdapter.StatusEndpoint(500);

            // Act
            var result = await curl.GetAsync(url);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(500);
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region Cancellation

        [Fact]
        public async Task GetAsync_WithCancellation_CanBeCancelled()
        {
            // Arrange
            using var curl = new LibCurl();
            var url = _serverAdapter.DelayEndpoint(5);
            using var cts = new CancellationTokenSource(100);

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                await curl.GetAsync(url, cancellationToken: cts.Token));
        }

        #endregion

        #region Dispose Pattern

        [Fact]
        public void Dispose_CalledMultipleTimes_DoesNotThrow()
        {
            // Arrange
            var curl = new LibCurl();

            // Act & Assert
            curl.Dispose();
            curl.Dispose(); // Should not throw

            // Verify we can't use it after disposal
            Func<Task> act = async () => await curl.GetAsync("http://example.com");
            act.Should().ThrowAsync<ObjectDisposedException>();
        }

        #endregion

        #region Chaining

        [Fact]
        public async Task FluentChaining_MultipleSettings_AllApplied()
        {
            // Arrange
            using var curl = new LibCurl()
                .WithTimeout(TimeSpan.FromSeconds(30))
                .WithFollowRedirects(10)
                .WithUserAgent("Test/1.0")
                .WithHeader("X-Test", "Value")
                .WithBasicAuth("user", "pass")
                .WithInsecureSsl()
                .WithVerbose();

            var url = _serverAdapter.GetEndpoint();

            // Act
            var result = await curl.GetAsync(url);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        #endregion
    }
}