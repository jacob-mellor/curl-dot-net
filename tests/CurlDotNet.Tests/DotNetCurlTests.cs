using System;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;
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
        [Fact]
        public void Curl_SynchronousExecution_ReturnsResult()
        {
            // Arrange
            var command = "curl https://httpbin.org/status/200";

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
            var command = "curl https://httpbin.org/delay/10";
            var timeoutSeconds = 1;

            // Act & Assert
            Assert.ThrowsAny<Exception>(() =>
                DotNetCurl.Curl(command, timeoutSeconds));
        }

        [Fact]
        public async Task CurlAsync_AsynchronousExecution_ReturnsResult()
        {
            // Arrange
            var command = "curl https://httpbin.org/get";

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
            var command = "curl https://httpbin.org/delay/10";
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
                "curl https://httpbin.org/status/200",
                "curl https://httpbin.org/status/201"
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
                "curl https://httpbin.org/status/200",
                "curl https://httpbin.org/status/201"
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
            var url = "https://httpbin.org/status/200";

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
            var command = @"curl -X POST https://httpbin.org/post
                -H 'Content-Type: application/json'
                -d '{""test"":true}'";

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
            var command = "curl https://httpbin.org/json";

            // Act
            var result = await DotNetCurl.CurlAsync(command);

            // Assert
            result.Body.Should().NotBeNullOrWhiteSpace();
            result.Body.Should().Contain("slideshow");
        }

        [Fact]
        public void Curl_HeadRequest_ReturnsHeaders()
        {
            // Arrange
            var command = "curl -I https://httpbin.org/status/200";

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
            var command = "curl -A 'DotNetCurl/1.0' https://httpbin.org/user-agent";

            // Act
            var result = await DotNetCurl.CurlAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Body.Should().Contain("DotNetCurl");
        }
    }
}