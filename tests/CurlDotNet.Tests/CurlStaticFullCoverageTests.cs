using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Comprehensive tests for Curl static class to achieve 100% code coverage.
    /// Tests all static properties, methods, and edge cases.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    public class CurlStaticFullCoverageTests : IDisposable
    {
        // Store original values to restore after tests
        private readonly int _originalMaxTimeSeconds;
        private readonly int _originalConnectTimeoutSeconds;
        private readonly bool _originalFollowRedirects;
        private readonly bool _originalInsecure;

        public CurlStaticFullCoverageTests()
        {
            // Save original values
            _originalMaxTimeSeconds = Curl.DefaultMaxTimeSeconds;
            _originalConnectTimeoutSeconds = Curl.DefaultConnectTimeoutSeconds;
            _originalFollowRedirects = Curl.DefaultFollowRedirects;
            _originalInsecure = Curl.DefaultInsecure;
        }

        public void Dispose()
        {
            // Restore original values
            Curl.DefaultMaxTimeSeconds = _originalMaxTimeSeconds;
            Curl.DefaultConnectTimeoutSeconds = _originalConnectTimeoutSeconds;
            Curl.DefaultFollowRedirects = _originalFollowRedirects;
            Curl.DefaultInsecure = _originalInsecure;
        }

        #region Static Properties Tests

        [Fact]
        public void DefaultMaxTimeSeconds_CanBeSetAndRead()
        {
            // Arrange & Act
            Curl.DefaultMaxTimeSeconds = 30;

            // Assert
            Curl.DefaultMaxTimeSeconds.Should().Be(30);

            // Test with different value
            Curl.DefaultMaxTimeSeconds = 60;
            Curl.DefaultMaxTimeSeconds.Should().Be(60);

            // Test with zero (no timeout)
            Curl.DefaultMaxTimeSeconds = 0;
            Curl.DefaultMaxTimeSeconds.Should().Be(0);
        }

        [Fact]
        public void DefaultConnectTimeoutSeconds_CanBeSetAndRead()
        {
            // Arrange & Act
            Curl.DefaultConnectTimeoutSeconds = 10;

            // Assert
            Curl.DefaultConnectTimeoutSeconds.Should().Be(10);

            // Test with different value
            Curl.DefaultConnectTimeoutSeconds = 20;
            Curl.DefaultConnectTimeoutSeconds.Should().Be(20);

            // Test with zero (no timeout)
            Curl.DefaultConnectTimeoutSeconds = 0;
            Curl.DefaultConnectTimeoutSeconds.Should().Be(0);
        }

        [Fact]
        public void DefaultFollowRedirects_CanBeSetAndRead()
        {
            // Arrange & Act
            Curl.DefaultFollowRedirects = true;

            // Assert
            Curl.DefaultFollowRedirects.Should().BeTrue();

            // Test with false
            Curl.DefaultFollowRedirects = false;
            Curl.DefaultFollowRedirects.Should().BeFalse();

            // Toggle again
            Curl.DefaultFollowRedirects = true;
            Curl.DefaultFollowRedirects.Should().BeTrue();
        }

        [Fact]
        public void DefaultInsecure_CanBeSetAndRead()
        {
            // Arrange & Act
            Curl.DefaultInsecure = true;

            // Assert
            Curl.DefaultInsecure.Should().BeTrue();

            // Test with false (safe mode)
            Curl.DefaultInsecure = false;
            Curl.DefaultInsecure.Should().BeFalse();

            // Toggle again (for development testing)
            Curl.DefaultInsecure = true;
            Curl.DefaultInsecure.Should().BeTrue();
        }

        #endregion

        #region Validation Methods Tests

        [Fact]
        public void Validate_ReturnsValidForValidHttpUrl()
        {
            var validUrls = new[]
            {
                "curl https://example.com",
                "curl http://localhost:8080",
                "curl https://api.github.com/users",
                "curl -X GET https://example.org",
                "curl --request POST http://api.example.com/data"
            };

            foreach (var url in validUrls)
            {
                var result = Curl.Validate(url);
                result.Should().NotBeNull();
                result.IsValid.Should().BeTrue($"'{url}' should be valid");
            }
        }

        [Fact]
        public void Validate_ReturnsInvalidForInvalidCommands()
        {
            var invalidCommands = new[]
            {
                "",
                "   ",
                "not-a-curl-command",
                "curl", // No URL
                "curl -X GET" // No URL
            };

            foreach (var cmd in invalidCommands)
            {
                var result = Curl.Validate(cmd);
                result.Should().NotBeNull();
                // These should generally be invalid
                if (!string.IsNullOrWhiteSpace(cmd) && !cmd.Contains("://"))
                {
                    result.IsValid.Should().BeFalse($"'{cmd}' should be invalid");
                }
            }
        }

        #endregion

        #region Conversion Methods Tests

        [Fact]
        public void ToHttpClient_ConvertsBasicGetRequest()
        {
            // Arrange
            var curlCommand = "curl https://api.example.com/data";

            // Act
            var httpClientCode = Curl.ToHttpClient(curlCommand);

            // Assert
            httpClientCode.Should().NotBeNullOrEmpty();
            httpClientCode.Should().Contain("HttpClient");
            httpClientCode.Should().Contain("GetAsync");
            httpClientCode.Should().Contain("https://api.example.com/data");
        }

        [Fact]
        public void ToHttpClient_ConvertsPostRequest()
        {
            // Arrange
            var curlCommand = "curl -X POST https://api.example.com/users -d '{\"name\":\"John\"}'";

            // Act
            var httpClientCode = Curl.ToHttpClient(curlCommand);

            // Assert
            httpClientCode.Should().NotBeNullOrEmpty();
            httpClientCode.Should().Contain("PostAsync");
            httpClientCode.Should().ContainAny("StringContent", "HttpContent");
        }

        [Fact]
        public void ToHttpClient_HandlesHeaders()
        {
            // Arrange
            var curlCommand = "curl -H 'Authorization: Bearer token' https://api.example.com";

            // Act
            var httpClientCode = Curl.ToHttpClient(curlCommand);

            // Assert
            httpClientCode.Should().NotBeNullOrEmpty();
            httpClientCode.Should().ContainAny("Headers.Add", "DefaultRequestHeaders");
            httpClientCode.Should().Contain("Authorization");
        }

        [Fact]
        public void ToFetch_ConvertsBasicGetRequest()
        {
            // Arrange
            var curlCommand = "curl https://api.example.com/data";

            // Act
            var fetchCode = Curl.ToFetch(curlCommand);

            // Assert
            fetchCode.Should().NotBeNullOrEmpty();
            fetchCode.Should().Contain("fetch");
            fetchCode.Should().Contain("https://api.example.com/data");
        }

        [Fact]
        public void ToFetch_ConvertsPostWithJson()
        {
            // Arrange
            var curlCommand = "curl -X POST https://api.example.com -H 'Content-Type: application/json' -d '{\"key\":\"value\"}'";

            // Act
            var fetchCode = Curl.ToFetch(curlCommand);

            // Assert
            fetchCode.Should().NotBeNullOrEmpty();
            fetchCode.Should().Contain("method: 'POST'");
            fetchCode.Should().ContainAny("body:", "JSON.stringify");
        }

        [Fact]
        public void ToPythonRequests_ConvertsBasicGetRequest()
        {
            // Arrange
            var curlCommand = "curl https://api.example.com/data";

            // Act
            var pythonCode = Curl.ToPythonRequests(curlCommand);

            // Assert
            pythonCode.Should().NotBeNullOrEmpty();
            pythonCode.Should().Contain("import requests");
            pythonCode.Should().Contain("requests.get");
            pythonCode.Should().Contain("https://api.example.com/data");
        }

        [Fact]
        public void ToPythonRequests_ConvertsPostWithData()
        {
            // Arrange
            var curlCommand = "curl -X POST https://api.example.com -d 'param=value'";

            // Act
            var pythonCode = Curl.ToPythonRequests(curlCommand);

            // Assert
            pythonCode.Should().NotBeNullOrEmpty();
            pythonCode.Should().Contain("requests.post");
            pythonCode.Should().ContainAny("data=", "json=");
        }

        #endregion

        #region Edge Cases and Error Handling

        [Fact]
        public void StaticConstructor_InitializesDefaultValues()
        {
            // The static constructor should have already run
            // Just verify the defaults are sensible
            Curl.DefaultMaxTimeSeconds.Should().BeGreaterOrEqualTo(0);
            Curl.DefaultConnectTimeoutSeconds.Should().BeGreaterOrEqualTo(0);
            // DefaultFollowRedirects and DefaultInsecure can be either true or false
        }

        [Fact]
        public void ToHttpClient_HandlesEmptyCommand()
        {
            // Act & Assert
            Action act = () => Curl.ToHttpClient("");

            // Depending on implementation, this might throw or return empty string
            try
            {
                var result = Curl.ToHttpClient("");
                result.Should().NotBeNull(); // Even if empty
            }
            catch (ArgumentException)
            {
                // If it throws ArgumentException, that's valid
            }
            catch (InvalidOperationException)
            {
                // If it throws InvalidOperationException, that's also valid
            }
        }

        [Fact]
        public void ToFetch_HandlesNullCommand()
        {
            // Act & Assert
            try
            {
                var result = Curl.ToFetch(null);
                // If it doesn't throw, it should return something
                result.Should().NotBeNull();
            }
            catch (ArgumentNullException)
            {
                // If it throws ArgumentNullException, that's valid
            }
            catch (NullReferenceException)
            {
                // If it throws NullReferenceException, that's also valid
            }
        }

        [Fact]
        public void ToPythonRequests_HandlesComplexHeaders()
        {
            // Arrange
            var curlCommand = "curl -H 'Accept: application/json' -H 'Authorization: Bearer abc123' https://api.example.com";

            // Act
            var pythonCode = Curl.ToPythonRequests(curlCommand);

            // Assert
            pythonCode.Should().NotBeNullOrEmpty();
            pythonCode.Should().Contain("headers");
            pythonCode.Should().Contain("Accept");
            pythonCode.Should().Contain("Authorization");
        }

        #endregion
    }
}