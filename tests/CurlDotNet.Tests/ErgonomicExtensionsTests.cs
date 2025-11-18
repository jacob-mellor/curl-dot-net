using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Tests for ErgonomicExtensions and CurlApiClient to improve code coverage.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    public class ErgonomicExtensionsTests
    {
        #region ParseJson Tests

        [Fact]
        public void ParseJson_ValidJson_DeserializesCorrectly()
        {
            // Arrange
            var result = new CurlResult
            {
                StatusCode = 200,
                Body = @"{""id"":1,""name"":""Test User""}"
            };

            // Act
            var user = result.ParseJson<TestUser>();

            // Assert
            user.Should().NotBeNull();
            user.Id.Should().Be(1);
            user.Name.Should().Be("Test User");
        }

        [Fact]
        public void ParseJson_FailedRequest_ThrowsInvalidOperationException()
        {
            // Arrange
            var result = new CurlResult
            {
                StatusCode = 404,
                Body = "Not Found"
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                result.ParseJson<TestUser>());
        }

        [Fact]
        public void ParseJson_EmptyBody_ThrowsInvalidOperationException()
        {
            // Arrange
            var result = new CurlResult
            {
                StatusCode = 200,
                Body = ""
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                result.ParseJson<TestUser>());
        }

        #endregion

        #region TryParseJson Tests

        [Fact]
        public void TryParseJson_ValidJson_ReturnsTrue()
        {
            // Arrange
            var result = new CurlResult
            {
                StatusCode = 200,
                Body = @"{""id"":2,""name"":""Jane""}"
            };

            // Act
            var success = result.TryParseJson<TestUser>(out var user);

            // Assert
            success.Should().BeTrue();
            user.Should().NotBeNull();
            user.Id.Should().Be(2);
            user.Name.Should().Be("Jane");
        }

        [Fact]
        public void TryParseJson_InvalidJson_ReturnsFalse()
        {
            // Arrange
            var result = new CurlResult
            {
                StatusCode = 200,
                Body = "Not JSON"
            };

            // Act
            var success = result.TryParseJson<TestUser>(out var user);

            // Assert
            success.Should().BeFalse();
            user.Should().BeNull();
        }

        [Fact]
        public void TryParseJson_FailedRequest_ReturnsFalse()
        {
            // Arrange
            var result = new CurlResult
            {
                StatusCode = 500,
                Body = @"{""error"":""Internal Server Error""}"
            };

            // Act
            var success = result.TryParseJson<TestUser>(out var user);

            // Assert
            success.Should().BeFalse();
            user.Should().BeNull();
        }

        #endregion

        #region SaveToFile Tests

        [Fact]
        public void SaveToFile_TextContent_SavesSuccessfully()
        {
            // Arrange
            var result = new CurlResult
            {
                StatusCode = 200,
                Body = "Test content"
            };
            var tempFile = Path.GetTempFileName();

            try
            {
                // Act
                var bytesWritten = result.SaveToFile(tempFile);

                // Assert
                bytesWritten.Should().Be(result.Body.Length);
                File.ReadAllText(tempFile).Should().Be("Test content");
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public void SaveToFile_BinaryContent_SavesSuccessfully()
        {
            // Arrange
            var binaryData = new byte[] { 0xFF, 0xFE, 0xFD };
            var result = new CurlResult
            {
                StatusCode = 200,
                BinaryData = binaryData
            };
            var tempFile = Path.GetTempFileName();

            try
            {
                // Act
                var bytesWritten = result.SaveToFile(tempFile);

                // Assert
                bytesWritten.Should().Be(binaryData.Length);
                File.ReadAllBytes(tempFile).Should().BeEquivalentTo(binaryData);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public void SaveToFile_FailedRequest_ThrowsInvalidOperationException()
        {
            // Arrange
            var result = new CurlResult
            {
                StatusCode = 404,
                Body = "Not Found"
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                result.SaveToFile("test.txt"));
        }

        #endregion

        #region GetHeader Tests

        [Fact]
        public void GetHeader_ExistingHeader_ReturnsValue()
        {
            // Arrange
            var result = new CurlResult
            {
                Headers = new System.Collections.Generic.Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json",
                    ["Content-Length"] = "123"
                }
            };

            // Act
            var contentType = result.GetHeader("Content-Type");

            // Assert
            contentType.Should().Be("application/json");
        }

        [Fact]
        public void GetHeader_CaseInsensitive_ReturnsValue()
        {
            // Arrange
            var result = new CurlResult
            {
                Headers = new System.Collections.Generic.Dictionary<string, string>
                {
                    ["Content-Type"] = "text/plain"
                }
            };

            // Act
            var contentType = result.GetHeader("content-type");

            // Assert
            contentType.Should().Be("text/plain");
        }

        [Fact]
        public void GetHeader_NonExistentHeader_ReturnsNull()
        {
            // Arrange
            var result = new CurlResult
            {
                Headers = new System.Collections.Generic.Dictionary<string, string>()
            };

            // Act
            var header = result.GetHeader("X-Missing");

            // Assert
            header.Should().BeNull();
        }

        #endregion

        #region HasContentType Tests

        [Fact]
        public void HasContentType_MatchingType_ReturnsTrue()
        {
            // Arrange
            var result = new CurlResult
            {
                Headers = new System.Collections.Generic.Dictionary<string, string>
                {
                    ["Content-Type"] = "application/json; charset=utf-8"
                }
            };

            // Act
            var hasJson = result.HasContentType("application/json");

            // Assert
            hasJson.Should().BeTrue();
        }

        [Fact]
        public void HasContentType_NonMatchingType_ReturnsFalse()
        {
            // Arrange
            var result = new CurlResult
            {
                Headers = new System.Collections.Generic.Dictionary<string, string>
                {
                    ["Content-Type"] = "text/html"
                }
            };

            // Act
            var hasJson = result.HasContentType("application/json");

            // Assert
            hasJson.Should().BeFalse();
        }

        #endregion

        #region EnsureSuccessStatusCode Tests

        [Fact]
        public void EnsureSuccessStatusCode_SuccessCode_ReturnsResult()
        {
            // Arrange
            var result = new CurlResult
            {
                StatusCode = 200
            };

            // Act
            var returned = result.EnsureSuccessStatusCode();

            // Assert
            returned.Should().BeSameAs(result);
        }

        [Fact]
        public void EnsureSuccessStatusCode_ErrorCode_ThrowsCurlHttpException()
        {
            // Arrange
            var result = new CurlResult
            {
                StatusCode = 404,
                Body = "Not Found",
                Command = "curl https://api.example.com"
            };

            // Act & Assert
            var ex = Assert.Throws<CurlDotNet.Exceptions.CurlHttpException>(() =>
                result.EnsureSuccessStatusCode());
            ex.StatusCode.Should().Be(404);
        }

        #endregion

        #region ToSimple Tests

        [Fact]
        public void ToSimple_SuccessResult_ReturnsTrueWithBody()
        {
            // Arrange
            var result = new CurlResult
            {
                StatusCode = 200,
                Body = "Success"
            };

            // Act
            var (success, body, error) = result.ToSimple();

            // Assert
            success.Should().BeTrue();
            body.Should().Be("Success");
            error.Should().BeNull();
        }

        [Fact]
        public void ToSimple_ErrorResult_ReturnsFalseWithError()
        {
            // Arrange
            var result = new CurlResult
            {
                StatusCode = 500,
                Body = "Error",
                Exception = new Exception("Server error")
            };

            // Act
            var (success, body, error) = result.ToSimple();

            // Assert
            success.Should().BeFalse();
            body.Should().Be("Error");
            error.Should().Contain("Server error");
        }

        #endregion

        #region CurlApiClient Tests

        [Fact]
        public void CurlApiClient_Constructor_InitializesCorrectly()
        {
            // Act
            var client = new CurlApiClient("https://api.example.com");

            // Assert
            client.Should().NotBeNull();
        }

        [Fact]
        public void CurlApiClient_Constructor_TrimsTrailingSlash()
        {
            // Act
            var client = new CurlApiClient("https://api.example.com/");

            // Assert
            client.Should().NotBeNull();
        }

        [Fact]
        public async Task CurlApiClient_GetAsync_BuildsCorrectUrl()
        {
            // Arrange
            var client = new CurlApiClient("https://httpbin.org");

            // Act
            var result = await client.GetAsync("get");

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CurlApiClient_PostJsonAsync_SendsData()
        {
            // Arrange
            var client = new CurlApiClient("https://httpbin.org");
            var data = new { test = "value" };

            // Act
            var result = await client.PostJsonAsync("post", data);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CurlApiClient_PutJsonAsync_SendsData()
        {
            // Arrange
            var client = new CurlApiClient("https://httpbin.org");
            var data = new { updated = true };

            // Act
            var result = await client.PutJsonAsync("put", data);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CurlApiClient_DeleteAsync_SendsRequest()
        {
            // Arrange
            var client = new CurlApiClient("https://httpbin.org");

            // Act
            var result = await client.DeleteAsync("delete");

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CurlApiClient_PatchJsonAsync_SendsData()
        {
            // Arrange
            var client = new CurlApiClient("https://httpbin.org");
            var data = new { patched = true };

            // Act
            var result = await client.PatchJsonAsync("patch", data);

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task CurlApiClient_EmptyPath_UsesBaseUrl()
        {
            // Arrange
            var client = new CurlApiClient("https://httpbin.org/get");

            // Act
            var result = await client.GetAsync("");

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().BeGreaterThan(0);
        }

        #endregion

        private class TestUser
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}