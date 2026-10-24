using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;
using CurlDotNet.Tests.TestServers;
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
        private TestServerEndpoint _testServer;
        private TestServerAdapter _serverAdapter;

        public CurlStaticMethodsTests()
        {
            // Initialize test server synchronously
            _testServer = TestServerConfiguration.GetBestAvailableServerAsync(TestServerFeatures.All).GetAwaiter().GetResult();
            _serverAdapter = new TestServerAdapter(_testServer.BaseUrl);
        }
        #region ExecuteAsync Tests

        [Fact]
        public async Task ExecuteAsync_SimpleCommand_ReturnsResult()
        {
            // Act
            var result = await Curl.ExecuteAsync($"curl {_serverAdapter.StatusEndpoint(200)}");

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
                await Curl.ExecuteAsync($"curl {_serverAdapter.DelayEndpoint(10)}", cts.Token));
        }

        [Fact]
        public async Task ExecuteAsync_WithSettings_UsesSettings()
        {
            // Arrange
            var settings = new CurlSettings()
                .WithTimeout(5)
                .WithFollowRedirects(true);

            // Act
            var result = await Curl.ExecuteAsync($"curl {_serverAdapter.GetEndpoint()}", settings);

            // Assert
            result.Should().NotBeNull();
        }

        #endregion

        #region HTTP Method Tests

        [Fact]
        public async Task GetAsync_SimpleUrl_PerformsGet()
        {
            // Act
            var result = await Curl.GetAsync(_serverAdapter.GetEndpoint());

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task PostAsync_WithData_SendsData()
        {
            // Act
            var result = await Curl.PostAsync(_serverAdapter.PostEndpoint(), "test=data");

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
            var result = await Curl.PostJsonAsync(_serverAdapter.PostEndpoint(), data);

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
                var result = await Curl.DownloadAsync(_serverAdapter.GetEndpoint(), tempFile);

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
                $"curl {_serverAdapter.StatusEndpoint(200)}",
                $"curl {_serverAdapter.StatusEndpoint(201)}"
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

        #region Quote-safe convenience helpers (issue #45)

        [Fact]
        public async Task PostAsync_DataContainingSingleQuote_IsSentIntact()
        {
            const string payload = "{\"name\":\"O'Brien\",\"note\":\"it's fine\"}";

            var result = await Curl.PostAsync(_serverAdapter.PostEndpoint(), payload);

            result.StatusCode.Should().Be(200);
            EchoedData(result).Should().Be(payload);
        }

        /// <summary>The local echo server returns {"data": "...", "form": {...}} as JSON; pull the raw body back out.</summary>
        private static string EchoedData(CurlResult result)
        {
            using var doc = System.Text.Json.JsonDocument.Parse(result.Body);
            var root = doc.RootElement;
            var data = root.TryGetProperty("data", out var d) ? d.GetString() : null;
            if (!string.IsNullOrEmpty(data)) return data!;
            if (root.TryGetProperty("form", out var form))
            {
                return string.Join("&", form.EnumerateObject().Select(p => $"{p.Name}={p.Value.GetString()}"));
            }
            return string.Empty;
        }

        [Fact]
        public async Task PostJsonAsync_ObjectContainingApostropheAndQuotes_IsSentIntact()
        {
            var data = new { name = "O'Brien", quote = "say \"hi\"" };

            var result = await Curl.PostJsonAsync(_serverAdapter.PostEndpoint(), data);

            result.StatusCode.Should().Be(200);
            EchoedData(result).Should().Be("{\"name\":\"O'Brien\",\"quote\":\"say \\\"hi\\\"\"}");
            result.Body.Should().Contain("application/json");
        }

        [Fact]
        public void Post_SyncDataContainingSingleQuote_IsSentIntact()
        {
            var result = Curl.Post(_serverAdapter.PostEndpoint(), "msg=don't panic");

            result.StatusCode.Should().Be(200);
            EchoedData(result).Should().Be("msg=don't panic");
        }

        [Fact]
        public async Task ExecuteAsync_WithCurlOptions_SendsRequestWithoutParsing()
        {
            var options = new CurlOptions
            {
                Url = _serverAdapter.PostEndpoint(),
                Method = "POST",
                Data = "{\"a\":\"b 'c' \\\"d\\\"\"}"
            };
            options.Headers["Content-Type"] = "application/json";

            var result = await Curl.ExecuteAsync(options);

            result.StatusCode.Should().Be(200);
            EchoedData(result).Should().Be(options.Data);
        }

        [Fact]
        public async Task DownloadAsync_PathWithSpacesAndApostrophe_SavesFile()
        {
            var dir = Path.Combine(Path.GetTempPath(), "curldotnet tests " + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, "o'brien's file.json");

            try
            {
                var result = await Curl.DownloadAsync(_serverAdapter.GetEndpoint(), path);

                result.StatusCode.Should().Be(200);
                File.Exists(path).Should().BeTrue();
                new FileInfo(path).Length.Should().BeGreaterThan(0);
            }
            finally
            {
                Directory.Delete(dir, recursive: true);
            }
        }

        [Fact]
        public async Task PostAsync_EmptyUrl_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => Curl.PostAsync("", "data"));
        }

        #endregion
    }
}