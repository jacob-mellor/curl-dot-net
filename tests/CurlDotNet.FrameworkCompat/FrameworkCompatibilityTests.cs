using System;
using System.Net;
using System.Threading.Tasks;
using CurlDotNet;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.FrameworkCompat
{
    /// <summary>
    /// Tests to ensure CurlDotNet works correctly with .NET Framework 4.7.2
    /// These tests use only .NET Standard 2.0 APIs to simulate Framework constraints
    /// </summary>
    public class FrameworkCompatibilityTests
    {
        [Fact]
        public async Task BasicCurlCommand_WorksInFramework()
        {
            // Act - using a simple echo service
            var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public void SyncExecution_WorksInFramework()
        {
            // Act - Framework often uses sync methods
            var result = Curl.Execute("curl https://httpbin.org/status/200");

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PostData_WorksInFramework()
        {
            // Act - Test POST with data
            var result = await Curl.ExecuteAsync("curl -X POST -d 'test=data' https://httpbin.org/post");

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void WebRequestCompatibility_Works()
        {
            // Test that we can work alongside legacy WebRequest code
            // (common in Framework apps)

            // Legacy Framework code pattern
            var request = WebRequest.Create("https://httpbin.org/get");
            request.Method = "GET";

            // Our library should work in the same app
            var result = Curl.Execute("curl https://httpbin.org/get");

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task HttpClientHandler_WorksInFramework()
        {
            // Ensure we're compatible with Framework's HttpClient limitations
            var result = await Curl.ExecuteAsync("curl -X GET https://httpbin.org/headers");

            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public void ServicePointManager_Compatibility()
        {
            // Framework apps often configure ServicePointManager
            // Ensure we don't break when it's configured

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.DefaultConnectionLimit = 10;

            var result = Curl.Execute("curl https://httpbin.org/get");

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void NetStandard20_ApiSurface()
        {
            // Verify we expose the expected API surface for .NET Standard 2.0

            // Static methods should be available
            var type = typeof(Curl);
            type.Should().NotBeNull();

            var executeMethod = type.GetMethod("Execute", new[] { typeof(string) });
            executeMethod.Should().NotBeNull("Execute method should be available");

            var executeAsyncMethod = type.GetMethod("ExecuteAsync", new[] { typeof(string) });
            executeAsyncMethod.Should().NotBeNull("ExecuteAsync method should be available");
        }

        [Fact]
        public async Task JsonSerialization_WorksInFramework()
        {
            // Framework apps might not have System.Text.Json built-in
            // but we bundle it via NuGet for netstandard2.0

            var result = await Curl.ExecuteAsync("curl https://httpbin.org/json");

            result.Should().NotBeNull();
            result.Body.Should().Contain("slideshow");
            result.IsSuccess.Should().BeTrue();
        }
    }
}