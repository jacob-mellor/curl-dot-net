using System;
using System.IO;
using System.Threading;
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
    /// Comprehensive tests for Curl string executor - the main entry point.
    /// Testing all static methods on the Curl class.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    [Trait("Category", TestCategories.FullCoverage)]
    public class CurlStringExecutorTests : CurlTestBase
    {
        private readonly TestServerEndpoint _testServer;
        private readonly TestServerAdapter _serverAdapter;

        public CurlStringExecutorTests(ITestOutputHelper output) : base(output)
        {
            _testServer = TestServerConfiguration.GetBestAvailableServerAsync(TestServerFeatures.All).GetAwaiter().GetResult();
            _serverAdapter = new TestServerAdapter(_testServer.BaseUrl);
            Output.WriteLine($"Using test server: {_testServer.Name} at {_testServer.BaseUrl}");
        }

        #region Basic Curl Command Execution

        [Fact]
        public async Task ExecuteAsync_SimpleGet_ReturnsSuccess()
        {
            // Arrange
            var url = _serverAdapter.GetEndpoint();
            var command = $"curl {url}";

            // Act
            var result = await Curl.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Body.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ExecuteAsync_WithHeaders_IncludesHeaders()
        {
            // Arrange
            var url = _serverAdapter.HeadersEndpoint();
            var command = $"curl -H 'X-Test-Header: TestValue' -H 'User-Agent: CurlDotNet/Test' {url}";

            // Act
            var result = await Curl.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Body.Should().Contain("X-Test-Header");
        }

        [Fact]
        public async Task ExecuteAsync_PostWithData_SendsData()
        {
            // Arrange
            var url = _serverAdapter.PostEndpoint();
            var command = $"curl -X POST -d 'test=data&foo=bar' {url}";

            // Act
            var result = await Curl.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Body.Should().Contain("test");
        }

        [Fact]
        public async Task ExecuteAsync_PostWithJson_SendsJson()
        {
            // Arrange
            var url = _serverAdapter.PostEndpoint();
            var json = "{\"name\":\"test\",\"value\":123}";
            var command = $"curl -X POST -H 'Content-Type: application/json' -d '{json}' {url}";

            // Act
            var result = await Curl.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Body.Should().Contain("name");
        }

        #endregion

        #region Cancellation Support

        [Fact]
        public async Task ExecuteAsync_WithCancellationToken_CanBeCancelled()
        {
            // Arrange
            var url = _serverAdapter.DelayEndpoint(5);
            var command = $"curl {url}";
            using var cts = new CancellationTokenSource(100);

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                await Curl.ExecuteAsync(command, cts.Token));
        }

        [Fact]
        public async Task ExecuteAsync_WithSettings_AppliesSettings()
        {
            // Arrange
            var url = _serverAdapter.GetEndpoint();
            var command = $"curl {url}";
            var settings = CurlSettings.FromDefaults()
                .WithTimeout(30)
                .WithFollowRedirects(true)
                .WithHeader("X-Custom", "Value");

            // Act
            var result = await Curl.ExecuteAsync(command, settings);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        #endregion

        #region Convenience Methods

        [Fact]
        public async Task GetAsync_SimpleUrl_ReturnsSuccess()
        {
            // Arrange
            var url = _serverAdapter.GetEndpoint();

            // Act
            var result = await Curl.GetAsync(url);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PostAsync_WithData_SendsData()
        {
            // Arrange
            var url = _serverAdapter.PostEndpoint();
            var data = "field1=value1&field2=value2";

            // Act
            var result = await Curl.PostAsync(url, data);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Body.Should().Contain("field1");
        }

        [Fact]
        public async Task PostJsonAsync_WithObject_SerializesAndSends()
        {
            // Arrange
            var url = _serverAdapter.PostEndpoint();
            var data = new { name = "test", value = 42, nested = new { prop = "value" } };

            // Act
            var result = await Curl.PostJsonAsync(url, data);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Body.Should().Contain("name");
            result.Body.Should().Contain("nested");
        }

        [Fact]
        public async Task DownloadAsync_ValidUrl_DownloadsFile()
        {
            // Arrange
            var url = $"{_testServer.BaseUrl}/bytes/1024";
            var tempFile = Path.GetTempFileName();

            try
            {
                // Act
                var result = await Curl.DownloadAsync(url, tempFile);

                // Assert
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeTrue();
                File.Exists(tempFile).Should().BeTrue();
                var fileInfo = new FileInfo(tempFile);
                fileInfo.Length.Should().BeGreaterThan(0);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        #endregion

        #region Multiple Commands

        [Fact]
        public async Task ExecuteManyAsync_MultipleCommands_ExecutesAll()
        {
            // Arrange
            var url = _serverAdapter.GetEndpoint();
            var commands = new[]
            {
                $"curl {url}",
                $"curl -X POST {_serverAdapter.PostEndpoint()}",
                $"curl -H 'Accept: application/json' {url}"
            };

            // Act
            var results = await Curl.ExecuteManyAsync(commands);

            // Assert
            results.Should().NotBeNull();
            results.Should().HaveCount(3);
            results.Should().AllSatisfy(r => r.Should().NotBeNull());
            results.Should().AllSatisfy(r => r.IsSuccess.Should().BeTrue());
        }

        #endregion

        #region Validation

        [Fact]
        public void Validate_ValidCommand_ReturnsValid()
        {
            // Arrange
            var command = $"curl {_serverAdapter.GetEndpoint()}";

            // Act
            var validation = Curl.Validate(command);

            // Assert
            validation.Should().NotBeNull();
            validation.IsValid.Should().BeTrue();
            validation.Error.Should().BeNullOrEmpty();
        }

        [Fact]
        public void Validate_InvalidCommand_ReturnsInvalid()
        {
            // Arrange
            var command = "curl --invalid-option http://example.com";

            // Act
            var validation = Curl.Validate(command);

            // Assert
            validation.Should().NotBeNull();
            validation.IsValid.Should().BeFalse();
            validation.Error.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Validate_EmptyCommand_ReturnsInvalid()
        {
            // Arrange
            var command = "";

            // Act
            var validation = Curl.Validate(command);

            // Assert
            validation.Should().NotBeNull();
            validation.IsValid.Should().BeFalse();
        }

        #endregion

        #region Code Generation

        [Fact]
        public void ToHttpClient_SimpleGet_GeneratesValidCode()
        {
            // Arrange
            var command = "curl https://api.example.com/data";

            // Act
            var code = Curl.ToHttpClient(command);

            // Assert
            code.Should().NotBeNullOrEmpty();
            code.Should().Contain("HttpClient");
            code.Should().Contain("GetAsync");
            code.Should().Contain("https://api.example.com/data");
        }

        [Fact]
        public void ToHttpClient_PostWithHeaders_GeneratesCorrectCode()
        {
            // Arrange
            var command = "curl -X POST -H 'Content-Type: application/json' -d '{\"key\":\"value\"}' https://api.example.com/data";

            // Act
            var code = Curl.ToHttpClient(command);

            // Assert
            code.Should().NotBeNullOrEmpty();
            code.Should().Contain("PostAsync");
            code.Should().Contain("Content-Type");
            code.Should().Contain("application/json");
        }

        [Fact]
        public void ToFetch_SimpleGet_GeneratesValidJavaScript()
        {
            // Arrange
            var command = "curl https://api.example.com/data";

            // Act
            var code = Curl.ToFetch(command);

            // Assert
            code.Should().NotBeNullOrEmpty();
            code.Should().Contain("fetch");
            code.Should().Contain("https://api.example.com/data");
        }

        [Fact]
        public void ToPythonRequests_SimpleGet_GeneratesValidPython()
        {
            // Arrange
            var command = "curl https://api.example.com/data";

            // Act
            var code = Curl.ToPythonRequests(command);

            // Assert
            code.Should().NotBeNullOrEmpty();
            code.Should().Contain("import requests");
            code.Should().Contain("requests.get");
            code.Should().Contain("https://api.example.com/data");
        }

        #endregion

        #region Error Handling

        [Fact]
        public async Task ExecuteAsync_InvalidUrl_ReturnsError()
        {
            // Arrange
            var command = "curl http://invalid.domain.that.does.not.exist.xyz";

            // Act
            var result = await Curl.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Exception.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_404Error_ReturnsError()
        {
            // Arrange
            var url = _serverAdapter.StatusEndpoint(404);
            var command = $"curl {url}";

            // Act
            var result = await Curl.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task ExecuteAsync_WithFailFlag_ThrowsOn4xx()
        {
            // Arrange
            var url = _serverAdapter.StatusEndpoint(404);
            var command = $"curl -f {url}";

            // Act
            var result = await Curl.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
        }

        #endregion

        #region Synchronous Methods

        [Fact]
        public void Execute_SimpleGet_ReturnsSuccess()
        {
            // Arrange
            var url = _serverAdapter.GetEndpoint();
            var command = $"curl {url}";

            // Act
            var result = Curl.Execute(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public void Get_SimpleUrl_ReturnsSuccess()
        {
            // Arrange
            var url = _serverAdapter.GetEndpoint();

            // Act
            var result = Curl.Get(url);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Post_WithData_SendsData()
        {
            // Arrange
            var url = _serverAdapter.PostEndpoint();
            var data = "test=sync";

            // Act
            var result = Curl.Post(url, data);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void PostJson_WithObject_SerializesAndSends()
        {
            // Arrange
            var url = _serverAdapter.PostEndpoint();
            var data = new { test = "sync", value = 123 };

            // Act
            var result = Curl.PostJson(url, data);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Download_ValidUrl_DownloadsFile()
        {
            // Arrange
            var url = $"{_testServer.BaseUrl}/bytes/512";
            var tempFile = Path.GetTempFileName();

            try
            {
                // Act
                var result = Curl.Download(url, tempFile);

                // Assert
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeTrue();
                File.Exists(tempFile).Should().BeTrue();
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        #endregion

        #region Default Settings

        [Fact]
        public async Task ExecuteAsync_WithDefaultTimeout_AppliesTimeout()
        {
            // Arrange
            Curl.DefaultMaxTimeSeconds = 1;
            var url = _serverAdapter.DelayEndpoint(5);
            var command = $"curl {url}";

            try
            {
                // Act
                var result = await Curl.ExecuteAsync(command);

                // Assert
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeFalse();
            }
            finally
            {
                Curl.DefaultMaxTimeSeconds = 0; // Reset
            }
        }

        [Fact]
        public async Task ExecuteAsync_WithDefaultFollowRedirects_FollowsRedirects()
        {
            // Arrange
            Curl.DefaultFollowRedirects = true;
            var url = _serverAdapter.RedirectEndpoint(1);
            var command = $"curl {url}";

            try
            {
                // Act
                var result = await Curl.ExecuteAsync(command);

                // Assert
                result.Should().NotBeNull();
                result.IsSuccess.Should().BeTrue();
            }
            finally
            {
                Curl.DefaultFollowRedirects = false; // Reset
            }
        }

        #endregion
    }
}