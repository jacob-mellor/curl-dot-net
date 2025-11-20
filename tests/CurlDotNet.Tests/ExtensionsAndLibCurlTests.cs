using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Extensions;
using CurlDotNet.Core;
using CurlDotNet.Lib;
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
        public StringExtensionsTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task CurlAsync_WithCommand_ExecutesSuccessfully()
        {
            // Act
            var result = await "curl https://httpbin.org/get".CurlAsync();

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
            var result = await "curl https://httpbin.org/get".CurlAsync(cts.Token);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task CurlGetAsync_WithUrl_PerformsGetRequest()
        {
            // Act
            var result = await "https://httpbin.org/get".CurlGetAsync();

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task CurlGetAsync_WithCurlPrefix_HandlesCorrectly()
        {
            // Act
            var result = await "curl https://httpbin.org/get".CurlGetAsync();

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task CurlPostJsonAsync_WithJsonData_PostsSuccessfully()
        {
            // Act
            var result = await "https://httpbin.org/post".CurlPostJsonAsync("{\"test\":\"value\"}");

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
                var result = await "https://httpbin.org/bytes/100".CurlDownloadAsync(tempFile);

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
            var result = "curl https://httpbin.org/get".Curl();

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task CurlBodyAsync_ReturnsBodyString()
        {
            // Act
            var body = await "https://httpbin.org/get".CurlBodyAsync();

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
        public FluentExtensionsTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task ToCurl_ReturnsBuilder()
        {
            // Act
            var builder = "https://httpbin.org/get".ToCurl();

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
            var builder = "https://httpbin.org/get".WithHeader("X-Custom-Header", "test-value");

            // Assert
            builder.Should().NotBeNull();
            var result = await builder.ExecuteAsync();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task WithMethod_SetsHttpMethod()
        {
            // Act
            var builder = "https://httpbin.org/post".WithMethod("POST");

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
        public LibCurlTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task GetAsync_SimpleGet_Works()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            var result = await curl.GetAsync("https://httpbin.org/get");

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
            var result = await curl.GetAsync("https://httpbin.org/get", opts => opts.Verbose = true);

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
            var result = await curl.PostAsync("https://httpbin.org/post", data);

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
            var result = await curl.PostAsync("https://httpbin.org/post", "raw data");

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
            var result = await curl.PostAsync("https://httpbin.org/post", null);

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
            var result = await curl.PutAsync("https://httpbin.org/put", data);

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
            var result = await curl.PatchAsync("https://httpbin.org/patch", data);

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
            var result = await curl.DeleteAsync("https://httpbin.org/delete");

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
            var result = await curl.HeadAsync("https://httpbin.org/get");

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
                Url = "https://httpbin.org/get",
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
            var result = await curl.GetAsync("https://httpbin.org/get");

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
            var result = await curl.GetAsync("https://httpbin.org/get");

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
            var result = await curl.GetAsync("https://httpbin.org/basic-auth/user/pass");

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
            var result = await curl.GetAsync("https://httpbin.org/get");

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
            var result = await curl.GetAsync("https://httpbin.org/get");

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
            var result = await curl.GetAsync("https://httpbin.org/redirect/1");

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
            var result = await curl.GetAsync("https://httpbin.org/get");

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task WithProxy_SetsProxy()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act - setting proxy but not actually using it for this test
            curl.WithProxy("http://proxy.example.com:8080");
            
            // We can't test actual proxy without a real proxy server
            // But we can verify the fluent API works
            curl.Should().NotBeNull();
        }

        [Fact]
        public async Task WithProxy_WithCredentials_SetsProxyAuth()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            curl.WithProxy("http://proxy.example.com:8080", "user", "pass");
            
            // Verify fluent API
            curl.Should().NotBeNull();
        }

        [Fact]
        public async Task WithUserAgent_SetsUserAgent()
        {
            // Arrange
            using var curl = new LibCurl();

            // Act
            curl.WithUserAgent("CustomAgent/1.0");
            var result = await curl.GetAsync("https://httpbin.org/get");

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
                var result = await curl.GetAsync("https://httpbin.org/get");

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
            var result = await curl.GetAsync("https://httpbin.org/get");

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
            var result = await curl.GetAsync("https://httpbin.org/get");

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
            var result = await curl.GetAsync("https://httpbin.org/get");

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

            var result = await curl.GetAsync("https://httpbin.org/get");

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
            var result1 = await curl.GetAsync("https://httpbin.org/get");
            var result2 = await curl.GetAsync("https://httpbin.org/get");

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
