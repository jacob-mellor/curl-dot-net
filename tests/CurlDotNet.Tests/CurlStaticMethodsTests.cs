using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Tests for Curl static methods to improve code coverage.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    public class CurlStaticMethodsTests
    {
        #region ExecuteAsync Tests

        [Fact]
        public async Task ExecuteAsync_SimpleCommand_ReturnsResult()
        {
            // Act
            var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/200");

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task ExecuteAsync_WithCancellation_SupportsCancellation()
        {
            // Arrange
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(100);

            // Act & Assert
            await Assert.ThrowsAnyAsync<Exception>(async () =>
                await Curl.ExecuteAsync("curl https://httpbin.org/delay/10", cts.Token));
        }

        [Fact]
        public async Task ExecuteAsync_WithSettings_UsesSettings()
        {
            // Arrange
            var settings = new CurlSettings()
                .WithTimeout(5)
                .WithFollowRedirects(true);

            // Act
            var result = await Curl.ExecuteAsync("curl https://httpbin.org/get", settings);

            // Assert
            result.Should().NotBeNull();
        }

        #endregion

        #region HTTP Method Tests

        [Fact]
        public async Task GetAsync_SimpleUrl_PerformsGet()
        {
            // Act
            var result = await Curl.GetAsync("https://httpbin.org/get");

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PostAsync_WithData_SendsData()
        {
            // Act
            var result = await Curl.PostAsync("https://httpbin.org/post", "test=data");

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PostJsonAsync_WithObject_SendsJson()
        {
            // Arrange
            var data = new { name = "test", value = 123 };

            // Act
            var result = await Curl.PostJsonAsync("https://httpbin.org/post", data);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Body.Should().Contain("test");
        }

        [Fact]
        public async Task DownloadAsync_SavesFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();

            try
            {
                // Act
                var result = await Curl.DownloadAsync("https://httpbin.org/bytes/100", tempFile);

                // Assert
                result.Should().NotBeNull();
                File.Exists(tempFile).Should().BeTrue();
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        #endregion

        #region ExecuteManyAsync Tests

        [Fact]
        public async Task ExecuteManyAsync_MultipleCommands_ExecutesAll()
        {
            // Arrange
            var commands = new[]
            {
                "curl https://httpbin.org/status/200",
                "curl https://httpbin.org/status/201"
            };

            // Act
            var results = await Curl.ExecuteManyAsync(commands);

            // Assert
            results.Should().NotBeNull();
            results.Should().HaveCount(2);
            results[0].StatusCode.Should().Be(200);
            results[1].StatusCode.Should().Be(201);
        }

        #endregion

        #region Global Settings Tests

        [Fact]
        public void DefaultMaxTimeSeconds_GetSet_Works()
        {
            // Arrange
            var original = Curl.DefaultMaxTimeSeconds;

            try
            {
                // Act
                Curl.DefaultMaxTimeSeconds = 30;

                // Assert
                Curl.DefaultMaxTimeSeconds.Should().Be(30);
            }
            finally
            {
                Curl.DefaultMaxTimeSeconds = original;
            }
        }

        [Fact]
        public void DefaultConnectTimeoutSeconds_GetSet_Works()
        {
            // Arrange
            var original = Curl.DefaultConnectTimeoutSeconds;

            try
            {
                // Act
                Curl.DefaultConnectTimeoutSeconds = 10;

                // Assert
                Curl.DefaultConnectTimeoutSeconds.Should().Be(10);
            }
            finally
            {
                Curl.DefaultConnectTimeoutSeconds = original;
            }
        }

        [Fact]
        public void DefaultFollowRedirects_GetSet_Works()
        {
            // Arrange
            var original = Curl.DefaultFollowRedirects;

            try
            {
                // Act
                Curl.DefaultFollowRedirects = true;

                // Assert
                Curl.DefaultFollowRedirects.Should().BeTrue();
            }
            finally
            {
                Curl.DefaultFollowRedirects = original;
            }
        }

        [Fact]
        public void DefaultInsecure_GetSet_Works()
        {
            // Arrange
            var original = Curl.DefaultInsecure;

            try
            {
                // Act
                Curl.DefaultInsecure = true;

                // Assert
                Curl.DefaultInsecure.Should().BeTrue();
            }
            finally
            {
                Curl.DefaultInsecure = original;
            }
        }

        #endregion

        #region Validate Tests

        [Fact]
        public void Validate_ValidCommand_ReturnsValid()
        {
            // Act
            var result = Curl.Validate("curl https://example.com");

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_InvalidCommand_ReturnsInvalid()
        {
            // Act
            var result = Curl.Validate("curl");

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
        }

        #endregion

        #region Code Generation Tests

        [Fact]
        public void ToPythonRequests_GeneratesPythonCode()
        {
            // Act
            var code = Curl.ToPythonRequests("curl https://example.com");

            // Assert
            code.Should().NotBeNullOrWhiteSpace();
            code.Should().Contain("import requests");
            code.Should().Contain("example.com");
        }

        [Fact]
        public void ToFetch_GeneratesFetchCode()
        {
            // Act
            var code = Curl.ToFetch("curl https://example.com");

            // Assert
            code.Should().NotBeNullOrWhiteSpace();
            code.Should().Contain("fetch");
            code.Should().Contain("example.com");
        }

        [Fact]
        public void ToHttpClient_GeneratesCSharpCode()
        {
            // Act
            var code = Curl.ToHttpClient("curl https://example.com");

            // Assert
            code.Should().NotBeNullOrWhiteSpace();
            code.Should().Contain("HttpClient");
            code.Should().Contain("example.com");
        }

        #endregion
    }
}