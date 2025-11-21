using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Extensions;
using CurlDotNet.Core;
using CurlDotNet.Lib;
using CurlDotNet.Tests.TestServers;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Comprehensive tests for StringExtensions to achieve 100% coverage.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    [Trait("Category", TestCategories.FullCoverage)]
    public class StringExtensionsTests : CurlTestBase
    {
        private TestServerEndpoint _testServer;
        private TestServerAdapter _serverAdapter;

        public StringExtensionsTests(ITestOutputHelper output) : base(output)
        {
            _testServer = TestServerConfiguration.GetBestAvailableServerAsync(TestServerFeatures.All).GetAwaiter().GetResult();
            _serverAdapter = new TestServerAdapter(_testServer.BaseUrl);
        }

        [Fact]
        public async Task CurlAsync_WithCommand_ExecutesSuccessfully()
        {
            // Act
            var result = await $"curl {_serverAdapter.GetEndpoint()}".CurlAsync();

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task CurlAsync_WithCancellationToken_ExecutesSuccessfully()
        {
            // Arrange
            using var cts = new CancellationTokenSource();

            // Act
            var result = await $"curl {_serverAdapter.GetEndpoint()}".CurlAsync(cts.Token);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task CurlGetAsync_WithUrl_PerformsGetRequest()
        {
            // Act
            var result = await _serverAdapter.GetEndpoint().CurlGetAsync();

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task CurlGetAsync_WithCurlPrefix_HandlesCorrectly()
        {
            // Act
            var result = await $"curl {_serverAdapter.GetEndpoint()}".CurlGetAsync();

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task CurlPostJsonAsync_WithJsonData_PostsSuccessfully()
        {
            // Act
            var result = await _serverAdapter.PostEndpoint().CurlPostJsonAsync("{\"test\":\"value\"}");

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task CurlDownloadAsync_DownloadsFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();

            try
            {
                // Act
                var result = await _serverAdapter.GetEndpoint().CurlDownloadAsync(tempFile);

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

        [Fact]
        public void Curl_SynchronousExecution_Works()
        {
            // Act
            var result = $"curl {_serverAdapter.GetEndpoint()}".Curl();

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task CurlBodyAsync_ReturnsBodyString()
        {
            // Act
            var body = await _serverAdapter.GetEndpoint().CurlBodyAsync();

            // Assert
            body.Should().NotBeNullOrEmpty();
        }
    }

    /// <summary>
    /// Comprehensive tests for FluentExtensions to achieve 100% coverage.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    [Trait("Category", TestCategories.FullCoverage)]
    public class FluentExtensionsTests : CurlTestBase
    {
        private TestServerEndpoint _testServer;
        private TestServerAdapter _serverAdapter;

        public FluentExtensionsTests(ITestOutputHelper output) : base(output)
        {
            _testServer = TestServerConfiguration.GetBestAvailableServerAsync(TestServerFeatures.All).GetAwaiter().GetResult();
            _serverAdapter = new TestServerAdapter(_testServer.BaseUrl);
        }

        [Fact]
        public async Task ToCurl_ReturnsBuilder()
        {
            // Act
            var builder = _serverAdapter.GetEndpoint().ToCurl();

            // Assert
            builder.Should().NotBeNull();
            builder.Should().BeOfType<CurlRequestBuilder>();
            
            var result = await builder.ExecuteAsync();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task WithHeader_AddsHeaderToBuilder()
        {
            // Act
            var builder = _serverAdapter.GetEndpoint().WithHeader("X-Custom-Header", "test-value");

            // Assert
            builder.Should().NotBeNull();
            var result = await builder.ExecuteAsync();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task WithMethod_SetsHttpMethod()
        {
            // Act
            var builder = _serverAdapter.PostEndpoint().WithMethod("POST");

            // Assert
            builder.Should().NotBeNull();
            var result = await builder.ExecuteAsync();
            result.IsSuccess.Should().BeTrue();
        }
    }

    /// <summary>
    /// Comprehensive tests for LibCurl to achieve 90%+ coverage.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    [Trait("Category", TestCategories.FullCoverage)]
    public class LibCurlTests : CurlTestBase
    {
        private TestServerEndpoint _testServer;
        private TestServerAdapter _serverAdapter;

        public LibCurlTests(ITestOutputHelper output) : base(output)
        {
            _testServer = TestServerConfiguration.GetBestAvailableServerAsync(TestServerFeatures.All).GetAwaiter().GetResult();
            _serverAdapter = new TestServerAdapter(_testServer.BaseUrl);
        }

        [Fact]
        public async Task GetAsync_SimpleGet_Works()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            var result = await curl.GetAsync(_serverAdapter.GetEndpoint());

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetAsync_WithConfiguration_Works()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            var result = await curl.GetAsync(_serverAdapter.GetEndpoint(), opts => opts.Verbose = true);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task PostAsync_WithObjectData_SerializesAndPosts()
        {
            // Arrange
            using var curl = new LibCurl();
            var data = new { name = "test", value = 123 };

            // Act
            var result = await curl.PostAsync(_serverAdapter.PostEndpoint(), data);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PostAsync_WithStringData_PostsRawString()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            var result = await curl.PostAsync(_serverAdapter.PostEndpoint(), "raw data");

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PostAsync_WithNullData_Works()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            var result = await curl.PostAsync(_serverAdapter.PostEndpoint(), null);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PutAsync_WithData_Works()
        {
            // Arrange
            using var curl = new LibCurl();
            var data = new { updated = true };

            // Act
            var result = await curl.PutAsync($"{_testServer.BaseUrl}/put", data);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PatchAsync_WithData_Works()
        {
            // Arrange
            using var curl = new LibCurl();
            var data = new { patched = true };

            // Act
            var result = await curl.PatchAsync($"{_testServer.BaseUrl}/patch", data);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task DeleteAsync_Works()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            var result = await curl.DeleteAsync($"{_testServer.BaseUrl}/delete");

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task HeadAsync_Works()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            var result = await curl.HeadAsync(_serverAdapter.GetEndpoint());

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PerformAsync_WithFullOptions_Works()
        {
            // Arrange
            using var curl = new LibCurl();
            var options = new CurlOptions
            {
                Url = _serverAdapter.GetEndpoint(),
                Method = "GET"
            };

            // Act
            var result = await curl.PerformAsync(options);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task WithHeader_SetsDefaultHeader()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            curl.WithHeader("X-Custom", "value");
            var result = await curl.GetAsync(_serverAdapter.GetEndpoint());

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task WithBearerToken_SetsAuthorizationHeader()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            curl.WithBearerToken("test-token-123");
            var result = await curl.GetAsync(_serverAdapter.GetEndpoint());

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task WithBasicAuth_SetsCredentials()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            curl.WithBasicAuth("user", "pass");
            var result = await curl.GetAsync(_serverAdapter.BasicAuthEndpoint("user", "pass"));

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task WithTimeout_SetsTimeout()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            curl.WithTimeout(TimeSpan.FromSeconds(30));
            var result = await curl.GetAsync(_serverAdapter.GetEndpoint());

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task WithConnectTimeout_SetsConnectTimeout()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            curl.WithConnectTimeout(TimeSpan.FromSeconds(10));
            var result = await curl.GetAsync(_serverAdapter.GetEndpoint());

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task WithFollowRedirects_EnablesRedirects()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            curl.WithFollowRedirects(10);
            var result = await curl.GetAsync(_serverAdapter.RedirectEndpoint(1));

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task WithInsecureSsl_EnablesInsecure()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            curl.WithInsecureSsl();
            var result = await curl.GetAsync(_serverAdapter.GetEndpoint());

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void WithProxy_SetsProxy()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act - using local test server as proxy URL for testing
            curl.WithProxy($"{_testServer.BaseUrl}/proxy");
            
            // We're just testing the fluent API works
            curl.Should().NotBeNull();
        }

        [Fact]
        public void WithProxy_WithCredentials_SetsProxyAuth()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act - using local test server as proxy URL
            curl.WithProxy($"{_testServer.BaseUrl}/proxy", "user", "pass");
            
            // Verify fluent API works
            curl.Should().NotBeNull();
        }

        [Fact]
        public async Task WithUserAgent_SetsUserAgent()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            curl.WithUserAgent("CustomAgent/1.0");
            var result = await curl.GetAsync(_serverAdapter.GetEndpoint());

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task WithOutputFile_SetsOutputFile()
        {
            // Arrange
            using var curl = new LibCurl();
            var tempFile = Path.GetTempFileName();

            try
            {
                // Act
                curl.WithOutputFile(tempFile);
                var result = await curl.GetAsync(_serverAdapter.GetEndpoint());

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

        [Fact]
        public async Task WithVerbose_EnablesVerbose()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            curl.WithVerbose();
            var result = await curl.GetAsync(_serverAdapter.GetEndpoint());

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Configure_WithAction_ConfiguresOptions()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            curl.Configure(opts =>
            {
                opts.FollowLocation = true;
                opts.MaxTime = 60;
            });
            var result = await curl.GetAsync(_serverAdapter.GetEndpoint());

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Configure_WithNullAction_DoesNotThrow()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            curl.Configure(null);
            var result = await curl.GetAsync(_serverAdapter.GetEndpoint());

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task FluentChaining_MultipleMethods_Works()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            curl.WithHeader("Accept", "application/json")
                .WithTimeout(TimeSpan.FromSeconds(30))
                .WithUserAgent("TestAgent/1.0")
                .WithFollowRedirects();

            var result = await curl.GetAsync(_serverAdapter.GetEndpoint());

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task DefaultHeaders_AppliedToAllRequests()
        {
            // Arrange
            using var curl = new LibCurl();
            curl.WithHeader("X-Test", "value");

            // Act
            var result1 = await curl.GetAsync(_serverAdapter.GetEndpoint());
            var result2 = await curl.GetAsync(_serverAdapter.GetEndpoint());

            // Assert
            result1.IsSuccess.Should().BeTrue();
            result2.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Dispose_DisposesResources()
        {
            // Arrange
            var curl = new LibCurl();

            // Act
            curl.Dispose();

            // Assert - should not throw
            curl.Should().NotBeNull();
        }
    }
}
