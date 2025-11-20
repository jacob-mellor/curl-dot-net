using System;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;
using CurlDotNet.Tests.TestServers;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Tests for DotNetCurl static class to improve code coverage.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    public class DotNetCurlTests
    {
        private TestServerEndpoint _testServer;
        private TestServerAdapter _serverAdapter;

        public DotNetCurlTests()
        {
            // Initialize test server synchronously
            _testServer = TestServerConfiguration.GetBestAvailableServerAsync(TestServerFeatures.All).GetAwaiter().GetResult();
            _serverAdapter = new TestServerAdapter(_testServer.BaseUrl);
        }
        [Fact]
        public void Curl_SynchronousExecution_ReturnsResult()
        {
            // Arrange
            var command = $"curl {_serverAdapter.StatusEndpoint(200)}";

            // Act
            var result = DotNetCurl.Curl(command);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Curl_WithTimeout_RespectsTimeout()
        {
            // Arrange
            var command = $"curl {_serverAdapter.DelayEndpoint(10)}";
            var timeoutSeconds = 1;

            // Act & Assert
            Assert.ThrowsAny<Exception>(() =>
                DotNetCurl.Curl(command, timeoutSeconds));
        }

        [Fact]
        public async Task CurlAsync_AsynchronousExecution_ReturnsResult()
        {
            // Arrange
            var command = $"curl {_serverAdapter.GetEndpoint()}";

            // Act
            var result = await DotNetCurl.CurlAsync(command);

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
                await DotNetCurl.CurlAsync(command, cts.Token));
        }

        [Fact]
        public async Task CurlManyAsync_MultipleCommands_ExecutesAll()
        {
            // Arrange
            var commands = new[]
            {
                $"curl {_serverAdapter.StatusEndpoint(200)}",
                $"curl {_serverAdapter.StatusEndpoint(201)}"
            };

            // Act
            var results = await DotNetCurl.CurlManyAsync(commands);

            // Assert
            results.Should().NotBeNull();
            results.Should().HaveCount(2);
            results[0].StatusCode.Should().Be(200);
            results[1].StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task CurlManyAsync_EmptyArray_ReturnsEmptyResults()
        {
            // Arrange
            var commands = Array.Empty<string>();

            // Act
            var results = await DotNetCurl.CurlManyAsync(commands);

            // Assert
            results.Should().NotBeNull();
            results.Should().BeEmpty();
        }

        [Fact]
        public void CurlMany_SynchronousMultiple_ReturnsResults()
        {
            // Arrange
            var commands = new[]
            {
                $"curl {_serverAdapter.StatusEndpoint(200)}",
                $"curl {_serverAdapter.StatusEndpoint(201)}"
            };

            // Act
            var results = DotNetCurl.CurlMany(commands);

            // Assert
            results.Should().NotBeNull();
            results.Should().HaveCount(2);
        }

        [Fact]
        public void Curl_WithoutCurlPrefix_StillWorks()
        {
            // Arrange
            var url = _serverAdapter.StatusEndpoint(200);

            // Act
            var result = DotNetCurl.Curl(url);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public void Curl_ComplexCommand_ParsesCorrectly()
        {
            // Arrange
            var command = $@"curl -X POST {_serverAdapter.PostEndpoint()}
                -H 'Content-Type: application/json'
                -d '{{""test"":true}}'";

            // Act
            var result = DotNetCurl.Curl(command);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CurlAsync_GetRequest_ReturnsBody()
        {
            // Arrange
            var command = $"curl {_serverAdapter.GetEndpoint()}";

            // Act
            var result = await DotNetCurl.CurlAsync(command);

            // Assert
            result.Body.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void Curl_HeadRequest_ReturnsHeaders()
        {
            // Arrange
            var command = $"curl -I {_serverAdapter.StatusEndpoint(200)}";

            // Act
            var result = DotNetCurl.Curl(command);

            // Assert
            result.Should().NotBeNull();
            result.Headers.Should().NotBeNull();
        }

        [Fact]
        public async Task CurlAsync_UserAgent_SetsHeader()
        {
            // Arrange
            var command = $"curl -A 'DotNetCurl/1.0' {_serverAdapter.HeadersEndpoint()}";

            // Act
            var result = await DotNetCurl.CurlAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Body.Should().Contain("DotNetCurl");
        }
    }
}