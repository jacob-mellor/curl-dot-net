using System;
using System.Net;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;
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
            // Arrange
            var curl = new Curl();

            // Act - using a simple echo service
            var result = await curl.ExecuteAsync("curl https://httpbin.org/get");

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public void SyncExecution_WorksInFramework()
        {
            // Arrange
            var curl = new Curl();

            // Act - Framework often uses sync methods
            var result = curl.Execute("curl https://httpbin.org/status/200");

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PostData_WorksInFramework()
        {
            // Arrange
            var options = new CurlOptions
            {
                Url = "https://httpbin.org/post",
                Method = "POST",
                Data = "test=data"
            };

            // Act
            var engine = new CurlEngine();
            var result = await engine.ExecuteAsync(options);

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
            var curl = new Curl();
            var result = curl.Execute("curl https://httpbin.org/get");

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task HttpClientHandler_WorksInFramework()
        {
            // Ensure we're compatible with Framework's HttpClient limitations
            var engine = new CurlEngine();
            var result = await engine.ExecuteAsync("curl -X GET https://httpbin.org/headers");

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

            var curl = new Curl();
            var result = curl.Execute("curl https://httpbin.org/get");

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }
    }
}