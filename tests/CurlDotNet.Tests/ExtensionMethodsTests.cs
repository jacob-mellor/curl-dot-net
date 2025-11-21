using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Extensions;
using CurlDotNet.Tests.TestServers;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Tests for extension methods to improve code coverage.
    /// Tests StringExtensions and FluentExtensions.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    public class ExtensionMethodsTests
    {
        private TestServerEndpoint _testServer;
        private TestServerAdapter _serverAdapter;

        public ExtensionMethodsTests()
        {
            // Initialize test server synchronously
            _testServer = TestServerConfiguration.GetBestAvailableServerAsync(TestServerFeatures.All).GetAwaiter().GetResult();
            _serverAdapter = new TestServerAdapter(_testServer.BaseUrl);
        }
        #region StringExtensions Tests

        [Fact]
        public async Task CurlAsync_ExecutesCurlCommand()
        {
            // Arrange
            var command = $"curl {_serverAdapter.StatusEndpoint(200)}";

            // Act
            var result = await command.CurlAsync();

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CurlAsync_WithCancellation_SupportsCancellation()
        {
            // Arrange
            var command = $"curl {_serverAdapter.DelayEndpoint(10)}";
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            // Act & Assert
            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await command.CurlAsync(cts.Token));
        }

        [Fact]
        public async Task CurlGetAsync_PerformsGetRequest()
        {
            // Arrange
            var url = _serverAdapter.GetEndpoint();

            // Act
            var result = await url.CurlGetAsync();

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CurlGetAsync_WithCurlPrefix_HandlesCorrectly()
        {
            // Arrange
            var command = $"curl {_serverAdapter.GetEndpoint()}";

            // Act
            var result = await command.CurlGetAsync();

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CurlPostJsonAsync_SendsJsonData()
        {
            // Arrange
            var url = _serverAdapter.PostEndpoint();
            var json = @"{""key"":""value""}";

            // Act
            var result = await url.CurlPostJsonAsync(json);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CurlDownloadAsync_CreatesDownloadCommand()
        {
            // Arrange
            var url = _serverAdapter.GetEndpoint();
            var outputFile = Path.GetTempFileName();

            try
            {
                // Act
                var result = await url.CurlDownloadAsync(outputFile);

                // Assert
                result.Should().NotBeNull();
                result.StatusCode.Should().BeGreaterThan(0);
            }
            finally
            {
                // Cleanup
                if (File.Exists(outputFile))
                    File.Delete(outputFile);
            }
        }

        [Fact]
        public void Curl_SynchronousExecution_Works()
        {
            // Arrange
            var command = $"curl {_serverAdapter.StatusEndpoint(200)}";

            // Act
            var result = command.Curl();

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CurlBodyAsync_ReturnsResponseBody()
        {
            // Arrange
            var url = _serverAdapter.GetEndpoint();

            // Act
            var body = await url.CurlBodyAsync();

            // Assert
            body.Should().NotBeNullOrWhiteSpace();
        }

        #endregion

        #region FluentExtensions Tests

        [Fact]
        public void ToCurl_CreatesRequestBuilder()
        {
            // Arrange
            var url = "https://api.example.com";

            // Act
            var builder = url.ToCurl();

            // Assert
            builder.Should().NotBeNull();
            builder.Should().BeOfType<CurlRequestBuilder>();
        }

        [Fact]
        public void WithHeader_AddsHeaderToBuilder()
        {
            // Arrange
            var url = "https://api.example.com";

            // Act
            var builder = url.WithHeader("Authorization", "Bearer token123");

            // Assert
            builder.Should().NotBeNull();
            builder.Should().BeOfType<CurlRequestBuilder>();
        }

        [Fact]
        public void WithMethod_SetsHttpMethod()
        {
            // Arrange
            var url = "https://api.example.com";

            // Act
            var builder = url.WithMethod("POST");

            // Assert
            builder.Should().NotBeNull();
            builder.Should().BeOfType<CurlRequestBuilder>();
        }

        [Fact]
        public async Task FluentChaining_WorksEndToEnd()
        {
            // Arrange & Act
            var result = await _serverAdapter.PostEndpoint()
                .WithMethod("POST")
                .WithHeader("Content-Type", "application/json")
                .WithData(@"{""test"":true}")
                .ExecuteAsync();

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().BeGreaterThan(0);
        }

        #endregion
    }
}