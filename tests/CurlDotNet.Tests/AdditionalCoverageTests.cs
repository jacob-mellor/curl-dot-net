using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;
using CurlDotNet.Tests.TestServers;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Comprehensive tests for DotNetCurl to achieve full code coverage.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    [Trait("Category", TestCategories.FullCoverage)]
    public class DotNetCurlAdditionalTests : CurlTestBase
    {
        private TestServerEndpoint _testServer;
        private TestServerAdapter _serverAdapter;

        public DotNetCurlAdditionalTests(ITestOutputHelper output) : base(output)
        {
            _testServer = TestServerConfiguration.GetBestAvailableServerAsync(TestServerFeatures.All).GetAwaiter().GetResult();
            _serverAdapter = new TestServerAdapter(_testServer.BaseUrl);
        }

        [Fact]
        public async Task DotNetCurl_Curl_WithSettings_Success()
        {
            // Arrange
            var command = _serverAdapter.GetEndpoint();
            var settings = CurlSettings.FromDefaults()
                .WithTimeout(30);

            // Act
            var result = await DotNetCurl.CurlAsync(command, settings);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void DotNetCurl_Validate_ReturnsValidationResult()
        {
            // Arrange
            var command = $"curl {_serverAdapter.GetEndpoint()}";

            // Act
            var validation = DotNetCurl.Validate(command);

            // Assert
            validation.Should().NotBeNull();
validation.IsValid.Should().BeTrue();
        }

        [Fact]
        public void DotNetCurl_ToHttpClient_GeneratesCode()
        {
            // Arrange
            var command = $"curl {_serverAdapter.GetEndpoint()}";

            // Act
            var code = DotNetCurl.ToHttpClient(command);

            // Assert
            code.Should().NotBeNullOrEmpty();
            code.Should().Contain("HttpClient");
        }

        [Fact]
        public void DotNetCurl_ToFetch_GeneratesJavaScript()
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
        public void DotNetCurl_ToPython_GeneratesPythonCode()
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
        public async Task DotNetCurl_Download_SavesFile()
        {
            // Arrange
            var url = $"{_testServer.BaseUrl}/bytes/100";
            var tempFile = Path.GetTempFileName();

            try
            {
                // Act
                var result = await DotNetCurl.DownloadAsync(url, tempFile);

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
        public void DotNetCurl_DefaultMaxTimeSeconds_CanBeSet()
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
        public void DotNetCurl_DefaultConnectTimeoutSeconds_CanBeSet()
        {
            // Arrange
            var original = DotNetCurl.DefaultConnectTimeoutSeconds;

            try
            {
                // Act
                DotNetCurl.DefaultConnectTimeoutSeconds = 60;

                // Assert
                DotNetCurl.DefaultConnectTimeoutSeconds.Should().Be(60);
            }
            finally
            {
                DotNetCurl.DefaultConnectTimeoutSeconds = original;
            }
        }

        [Fact]
        public void DotNetCurl_DefaultFollowRedirects_CanBeSet()
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
        public void DotNetCurl_DefaultInsecure_CanBeSet()
        {
            // Arrange
            var original = DotNetCurl.DefaultInsecure;

            try
            {
                // Act
                DotNetCurl.DefaultInsecure = false;

                // Assert
                DotNetCurl.DefaultInsecure.Should().BeFalse();
            }
            finally
            {
                DotNetCurl.DefaultInsecure = original;
            }
        }
    }

    /// <summary>
    /// Tests for ErgonomicExtensions and CurlApiClient.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    [Trait("Category", TestCategories.FullCoverage)]
    public class ErgonomicExtensionsAdditionalTests : CurlTestBase
    {
        private TestServerEndpoint _testServer;
        private TestServerAdapter _serverAdapter;

        public ErgonomicExtensionsAdditionalTests(ITestOutputHelper output) : base(output)
        {
            _testServer = TestServerConfiguration.GetBestAvailableServerAsync(TestServerFeatures.All).GetAwaiter().GetResult();
            _serverAdapter = new TestServerAdapter(_testServer.BaseUrl);
        }

        [Fact]
        public async Task CurlApiClient_GetAsync_Works()
        {
            // Arrange
            var client = new CurlApiClient(_testServer.BaseUrl);

            // Act
            var result = await client.GetAsync("get");

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task CurlApiClient_PostJsonAsync_Works()
        {
            // Arrange
            var client = new CurlApiClient(_testServer.BaseUrl);
            var data = new { name = "test" };

            // Act
            var result = await client.PostJsonAsync("post", data);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task CurlApiClient_PutJsonAsync_Works()
        {
            // Arrange
            var client = new CurlApiClient(_testServer.BaseUrl);
            var data = new { value = "updated" };

            // Act
            var result = await client.PutJsonAsync("put", data);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task CurlApiClient_DeleteAsync_Works()
        {
            // Arrange
            var client = new CurlApiClient(_testServer.BaseUrl);

            // Act
            var result = await client.DeleteAsync("delete");

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task CurlApiClient_PatchJsonAsync_Works()
        {
            // Arrange
            var client = new CurlApiClient(_testServer.BaseUrl);
            var data = new { field = "patched" };

            // Act
            var result = await client.PatchJsonAsync("patch", data);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ParseJson_ValidResponse_Deserializes()
        {
            // Arrange
            var result = await Curl.ExecuteAsync($"curl {_serverAdapter.GetEndpoint()}");

            // Act
            var json = result.ParseJson<JsonElement>();

            // Assert
            json.Should().NotBeNull();
        }

        [Fact]
        public async Task TryParseJson_ValidResponse_ReturnsTrue()
        {
            // Arrange
            var result = await Curl.ExecuteAsync($"curl {_serverAdapter.GetEndpoint()}");

            // Act
            var success = result.TryParseJson<object>(out var json);

            // Assert
            success.Should().BeTrue();
            json.Should().NotBeNull();
        }

        [Fact]
        public async Task SaveToFile_WithBody_SavesFile()
        {
            // Arrange
            var result = await Curl.ExecuteAsync($"curl {_serverAdapter.GetEndpoint()}");
            var tempFile = Path.GetTempFileName();

            try
            {
                // Act
                var bytes = result.SaveToFile(tempFile);

                // Assert
                bytes.Should().NotBeNull();
                File.Exists(tempFile).Should().BeTrue();
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public async Task GetHeader_ValidHeader_ReturnsValue()
        {
            // Arrange
            var result = await Curl.ExecuteAsync($"curl {_serverAdapter.GetEndpoint()}");

            // Act
            var contentType = result.GetHeader("Content-Type");

            // Assert
            contentType.Should().NotBeNull();
        }

        [Fact]
        public async Task HasContentType_MatchingType_ReturnsTrue()
        {
            // Arrange
            var result = await Curl.ExecuteAsync($"curl {_serverAdapter.GetEndpoint()}");

            // Act
            var hasJson = result.HasContentType("json");

            // Assert
            hasJson.Should().BeTrue();
        }

        [Fact]
        public async Task EnsureSuccessStatusCode_SuccessResponse_ReturnsResult()
        {
            // Arrange
           var result = await Curl.ExecuteAsync($"curl {_serverAdapter.StatusEndpoint(200)}");

            // Act
            var chained = result.EnsureSuccessStatusCode();

            // Assert
            chained.Should().BeSameAs(result);
        }

        [Fact]
        public async Task ToSimple_SuccessResponse_ReturnsTuple()
        {
            // Arrange
            var result = await Curl.ExecuteAsync($"curl {_serverAdapter.GetEndpoint()}");

            // Act
            var (success, body, error) = result.ToSimple();

            // Assert
            success.Should().BeTrue();
            body.Should().NotBeNullOrEmpty();
            error.Should().BeNull();
        }
    }
}

