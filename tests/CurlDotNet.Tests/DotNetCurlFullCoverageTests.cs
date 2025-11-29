using System;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Tests.TestServers;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Comprehensive tests for DotNetCurl to boost coverage from 38.8% to 90%+
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    [Trait("Category", TestCategories.FullCoverage)]
    public class DotNetCurlFullCoverageTests : CurlTestBase
    {
        private TestServerEndpoint _testServer;
        private TestServerAdapter _serverAdapter;

        public DotNetCurlFullCoverageTests(ITestOutputHelper output) : base(output)
        {
            _testServer = TestServerConfiguration.GetBestAvailableServerAsync(TestServerFeatures.All).GetAwaiter().GetResult();
            _serverAdapter = new TestServerAdapter(_testServer.BaseUrl);
        }

        #region Global Settings Tests

        [Fact]
        public void DefaultMaxTimeSeconds_SetAndGet_Works()
        {
            // Arrange
            var original = DotNetCurl.DefaultMaxTimeSeconds;
            
            try
            {
                // Act
                DotNetCurl.DefaultMaxTimeSeconds = 120;
                
                // Assert
                DotNetCurl.DefaultMaxTimeSeconds.Should().Be(120);
            }
            finally
            {
                DotNetCurl.DefaultMaxTimeSeconds = original;
            }
        }

        [Fact]
        public void DefaultConnectTimeoutSeconds_SetAndGet_Works()
        {
            // Arrange
            var original = DotNetCurl.DefaultConnectTimeoutSeconds;
            
            try
            {
                // Act
                DotNetCurl.DefaultConnectTimeoutSeconds = 45;
                
                // Assert
                DotNetCurl.DefaultConnectTimeoutSeconds.Should().Be(45);
            }
            finally
            {
                DotNetCurl.DefaultConnectTimeoutSeconds = original;
            }
        }

        [Fact]
        public void DefaultFollowRedirects_SetAndGet_Works()
        {
            // Arrange
            var original = DotNetCurl.DefaultFollowRedirects;
            
            try
            {
                // Act
                DotNetCurl.DefaultFollowRedirects = true;
                
                // Assert
                DotNetCurl.DefaultFollowRedirects.Should().BeTrue();
            }
            finally
            {
                DotNetCurl.DefaultFollowRedirects = original;
            }
        }

        [Fact]
        public void DefaultInsecure_SetAndGet_Works()
        {
            // Arrange
            var original = DotNetCurl.DefaultInsecure;
            
            try
            {
                // Act
                DotNetCurl.DefaultInsecure = true;
                
                // Assert
                DotNetCurl.DefaultInsecure.Should().BeTrue();
            }
            finally
            {
                DotNetCurl.DefaultInsecure = original;
            }
        }

        #endregion

        #region Download Tests

        [Fact]
        public async Task DownloadAsync_ValidUrl_DownloadsFile()
        {
            // Arrange
            var tempFile = System.IO.Path.GetTempFileName();
            
            try
            {
                // Act
                var result = await DotNetCurl.DownloadAsync(_serverAdapter.GetEndpoint(), tempFile);
                
                // Assert
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeTrue();
                System.IO.File.Exists(tempFile).Should().BeTrue();
            }
            finally
            {
                if (System.IO.File.Exists(tempFile))
                    System.IO.File.Delete(tempFile);
            }
        }

        [Fact]
        public void Download_Synchronous_DownloadsFile()
        {
            // Arrange
            var tempFile = System.IO.Path.GetTempFileName();
            
            try
            {
                // Act
                var result = DotNetCurl.Download(_serverAdapter.GetEndpoint(), tempFile);
                
                // Assert
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeTrue();
                System.IO.File.Exists(tempFile).Should().BeTrue();
            }
            finally
            {
                if (System.IO.File.Exists(tempFile))
                    System.IO.File.Delete(tempFile);
            }
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void Validate_ValidCommand_ReturnsTrue()
        {
            // Act
            var result = DotNetCurl.Validate($"curl {_serverAdapter.GetEndpoint()}");
            
            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_InvalidCommand_ReturnsFalse()
        {
            // Act
            var result = DotNetCurl.Validate("curl --invalid-flag-xyz");
            
            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_EmptyCommand_ReturnsFalse()
        {
            // Act
            var result = DotNetCurl.Validate("");
            
            // Assert
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_NullCommand_ReturnsFalse()
        {
            // Act
            var result = DotNetCurl.Validate(null);
            
            // Assert
            result.IsValid.Should().BeFalse();
        }

        #endregion

        #region Code Generation Tests

        [Fact]
        public void ToHttpClient_SimpleGet_GeneratesCode()
        {
            // Arrange
            var command = $"curl {_serverAdapter.GetEndpoint()}";
            
            // Act
            var code = DotNetCurl.ToHttpClient(command);
            
            // Assert
            code.Should().NotBeNullOrEmpty();
            code.Should().Contain("HttpClient");
            code.Should().Contain("GetAsync");
        }

        [Fact]
        public void ToHttpClient_WithHeaders_GeneratesCodeWithHeaders()
        {
            // Arrange
            var command = $"curl -H 'Authorization: Bearer token' {_serverAdapter.GetEndpoint()}";
            
            // Act
            var code = DotNetCurl.ToHttpClient(command);
            
            // Assert
            code.Should().Contain("Authorization");
            code.Should().Contain("Bearer token");
        }

        [Fact]
        public void ToFetch_SimpleGet_GeneratesJavaScript()
        {
            // Arrange
            var command = $"curl {_serverAdapter.GetEndpoint()}";
            
            // Act
            var code = DotNetCurl.ToFetch(command);
            
            // Assert
            code.Should().NotBeNullOrEmpty();
            code.Should().Contain("fetch");
        }

        [Fact]
        public void ToFetch_WithMethod_GeneratesCodeWithMethod()
        {
            // Arrange
            var command = $"curl -X POST {_serverAdapter.PostEndpoint()}";
            
            // Act
            var code = DotNetCurl.ToFetch(command);
            
            // Assert
            code.Should().Contain("method");
            code.Should().Contain("POST");
        }

        [Fact]
        public void ToPython_SimpleGet_GeneratesPythonCode()
        {
            // Arrange
            var command = $"curl {_serverAdapter.GetEndpoint()}";
            
            // Act
            var code = DotNetCurl.ToPython(command);
            
            // Assert
            code.Should().NotBeNullOrEmpty();
            code.Should().Contain("requests");
        }

        [Fact]
        public void ToPython_WithData_GeneratesCodeWithData()
        {
            // Arrange
            var command = $"curl -d 'test=data' {_serverAdapter.PostEndpoint()}";
            
            // Act
            var code = DotNetCurl.ToPython(command);
            
            // Assert
            code.Should().Contain("data");
        }

        #endregion

        #region CurlAsync with Global Settings Tests

        [Fact]
        public async Task CurlAsync_WithGlobalTimeout_UsesTimeout()
        {
            // Arrange
            var originalTimeout = DotNetCurl.DefaultMaxTimeSeconds;
            
            try
            {
                DotNetCurl.DefaultMaxTimeSeconds = 60;
                
                // Act
                var result = await DotNetCurl.CurlAsync($"curl {_serverAdapter.GetEndpoint()}");
                
                // Assert
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeTrue();
            }
            finally
            {
                DotNetCurl.DefaultMaxTimeSeconds = originalTimeout;
            }
        }

        [Fact]
        public async Task CurlAsync_WithGlobalFollowRedirects_FollowsRedirects()
        {
            // Arrange
            var originalFollow = DotNetCurl.DefaultFollowRedirects;
            
            try
            {
                DotNetCurl.DefaultFollowRedirects = true;
                
                // Act
                var result = await DotNetCurl.CurlAsync($"curl {_serverAdapter.RedirectEndpoint(1)}");
                
                // Assert
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeTrue();
            }
            finally
            {
                DotNetCurl.DefaultFollowRedirects = originalFollow;
            }
        }

        #endregion

        #region CurlMany Tests

        [Fact]
        public async Task CurlManyAsync_MultipleCommands_ExecutesAll()
        {
            // Arrange
            var commands = new[]
            {
                $"curl {_serverAdapter.GetEndpoint()}",
                $"curl {_serverAdapter.StatusEndpoint(200)}",
                $"curl {_serverAdapter.StatusEndpoint(201)}"
            };
            
            // Act
            var results = await DotNetCurl.CurlManyAsync(commands);
            
            // Assert
            results.Should().NotBeNull();
            results.Should().HaveCount(3);
            results[0].IsSuccess.Should().BeTrue();
            results[1].IsSuccess.Should().BeTrue();
            results[2].IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void CurlMany_Synchronous_ExecutesAll()
        {
            // Arrange
            var commands = new[]
            {
                $"curl {_serverAdapter.GetEndpoint()}",
                $"curl {_serverAdapter.StatusEndpoint(200)}"
            };
            
            // Act
            var results = DotNetCurl.CurlMany(commands);
            
            // Assert
            results.Should().NotBeNull();
            results.Should().HaveCount(2);
        }

        [Fact]
        public async Task CurlManyAsync_EmptyArray_ReturnsEmpty()
        {
            // Act
            var results = await DotNetCurl.CurlManyAsync(Array.Empty<string>());
            
            // Assert
            results.Should().NotBeNull();
            results.Should().BeEmpty();
        }

        #endregion

        #region Curl Synchronous Tests

        [Fact]
        public void Curl_SimpleCommand_ExecutesSuccessfully()
        {
            // Act
            var result = DotNetCurl.Curl($"curl {_serverAdapter.GetEndpoint()}");
            
            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Curl_WithTimeout_RespectsTimeout()
        {
            // Arrange - delay endpoint would timeout
            var command = $"curl {_serverAdapter.DelayEndpoint(30)}";
            
            // Act & Assert - should timeout with 1 second limit
            Assert.ThrowsAny<Exception>(() => DotNetCurl.Curl(command, 1));
        }

        #endregion
    }
}
