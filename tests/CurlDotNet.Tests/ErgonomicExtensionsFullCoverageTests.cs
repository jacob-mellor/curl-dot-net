using System;
using System.IO;
using System.Text.Json;
using CurlDotNet;
using CurlDotNet.Core;
using CurlDotNet.Extensions;
using CurlDotNet.Tests.TestServers;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Comprehensive tests for ErgonomicExtensions to boost coverage from 38.1% to 90%+
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    [Trait("Category", TestCategories.FullCoverage)]
    public class ErgonomicExtensionsFullCoverageTests : CurlTestBase
    {
        private TestServerEndpoint _testServer;
        private TestServerAdapter _serverAdapter;

        public ErgonomicExtensionsFullCoverageTests(ITestOutputHelper output) : base(output)
        {
            _testServer = TestServerConfiguration.GetBestAvailableServerAsync(TestServerFeatures.All).GetAwaiter().GetResult();
            _serverAdapter = new TestServerAdapter(_testServer.BaseUrl);
        }

        #region ParseJson Tests

        [Fact]
        public void ParseJson_ValidJson_DeserializesSuccessfully()
        {
            // Arrange
            var result = new CurlResult
            {
                Body = "{\"name\":\"test\",\"value\":123}",
                StatusCode = 200
            };

            // Act
            var obj = result.ParseJson<TestData>();

            // Assert
            obj.Should().NotBeNull();
            obj.Name.Should().Be("test");
            obj.Value.Should().Be(123);
        }

        [Fact]
        public void ParseJson_InvalidJson_ThrowsJsonException()
        {
            // Arrange
            var result = new CurlResult
            {
                Body = "not valid json",
                StatusCode = 200
            };

            // Act & Assert
            Assert.Throws<JsonException>(() => result.ParseJson<TestData>());
        }

        [Fact]
        public void ParseJson_NullBody_ThrowsException()
        {
            // Arrange
            var result = new CurlResult
            {
                Body = null,
                StatusCode = 200
            };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => result.ParseJson<TestData>());
        }

        [Fact]
        public void ParseJson_EmptyBody_ThrowsException()
        {
            // Arrange
            var result = new CurlResult
            {
                Body = "",
                StatusCode = 200
            };

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => result.ParseJson<TestData>());
        }

        #endregion

        #region TryParseJson Tests

        [Fact]
        public void TryParseJson_ValidJson_ReturnsTrue()
        {
            // Arrange
            var result = new CurlResult
            {
                Body = "{\"name\":\"test\",\"value\":456}",
                StatusCode = 200
            };

            // Act
            var success = result.TryParseJson<TestData>(out var obj);

            // Assert
            success.Should().BeTrue();
            obj.Should().NotBeNull();
            obj.Name.Should().Be("test");
            obj.Value.Should().Be(456);
        }

        [Fact]
        public void TryParseJson_InvalidJson_ReturnsFalse()
        {
            // Arrange
            var result = new CurlResult
            {
                Body = "invalid json",
                StatusCode = 200
            };

            // Act
            var success = result.TryParseJson<TestData>(out var obj);

            // Assert
            success.Should().BeFalse();
            obj.Should().BeNull();
        }

        [Fact]
        public void TryParseJson_NullBody_ReturnsFalse()
        {
            // Arrange
            var result = new CurlResult
            {
                Body = null,
                StatusCode = 200
            };

            // Act
            var success = result.TryParseJson<TestData>(out var obj);

            // Assert
            success.Should().BeFalse();
            obj.Should().BeNull();
        }

        #endregion

        #region SaveToFile Tests

        [Fact]
        public void SaveToFile_WithBinaryData_SavesFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var result = new CurlResult
            {
                BinaryData = new byte[] { 1, 2, 3, 4, 5 },
                StatusCode = 200,
            };

            try
            {
                // Act
                var bytesWritten = result.SaveToFile(tempFile);

                // Assert
                bytesWritten.Should().Be(5);
                File.Exists(tempFile).Should().BeTrue();
                var fileBytes = File.ReadAllBytes(tempFile);
                fileBytes.Should().Equal(new byte[] { 1, 2, 3, 4, 5 });
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public void SaveToFile_WithTextData_SavesFile()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var result = new CurlResult
            {
                Body = "test content",
                StatusCode = 200,
            };

            try
            {
                // Act
                var bytesWritten = result.SaveToFile(tempFile);

                // Assert
                bytesWritten.Should().Be(12); // "test content" length
                File.Exists(tempFile).Should().BeTrue();
                var fileContent = File.ReadAllText(tempFile);
                fileContent.Should().Be("test content");
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public void SaveToFile_FailedResult_ThrowsException()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var result = new CurlResult
            {
                StatusCode = 404,
            };

            try
            {
                // Act & Assert
                Assert.Throws<InvalidOperationException>(() => result.SaveToFile(tempFile));
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Fact]
        public void SaveToFile_NoContent_ThrowsException()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            var result = new CurlResult
            {
                Body = null,
                BinaryData = null,
                StatusCode = 200,
            };

            try
            {
                // Act & Assert
                Assert.Throws<InvalidOperationException>(() => result.SaveToFile(tempFile));
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
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
                    { "Content-Type", "application/json" },
                    { "X-Custom-Header", "test-value" }
                },
                StatusCode = 200
            };

            // Act
            var value = result.GetHeader("Content-Type");

            // Assert
            value.Should().Be("application/json");
        }

        [Fact]
        public void GetHeader_CaseInsensitive_ReturnsValue()
        {
            // Arrange
            var result = new CurlResult
            {
                Headers = new System.Collections.Generic.Dictionary<string, string>
                {
                    { "Content-Type", "application/json" }
                },
                StatusCode = 200
            };

            // Act
            var value = result.GetHeader("content-type");

            // Assert
            value.Should().Be("application/json");
        }

        [Fact]
        public void GetHeader_NonExistingHeader_ReturnsNull()
        {
            // Arrange
            var result = new CurlResult
            {
                Headers = new System.Collections.Generic.Dictionary<string, string>(),
                StatusCode = 200
            };

            // Act
            var value = result.GetHeader("NonExisting");

            // Assert
            value.Should().BeNull();
        }

        [Fact]
        public void GetHeader_NullHeaders_ReturnsNull()
        {
            // Arrange
            var result = new CurlResult
            {
                Headers = null,
                StatusCode = 200
            };

            // Act
            var value = result.GetHeader("Any-Header");

            // Assert
            value.Should().BeNull();
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
                    { "Content-Type", "application/json; charset=utf-8" }
                },
                StatusCode = 200
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
                    { "Content-Type", "text/html" }
                },
                StatusCode = 200
            };

            // Act
            var hasJson = result.HasContentType("application/json");

            // Assert
            hasJson.Should().BeFalse();
        }

        [Fact]
        public void HasContentType_NoContentTypeHeader_ReturnsFalse()
        {
            // Arrange
            var result = new CurlResult
            {
                Headers = new System.Collections.Generic.Dictionary<string, string>(),
                StatusCode = 200
            };

            // Act
            var hasJson = result.HasContentType("application/json");

            // Assert
            hasJson.Should().BeFalse();
        }

        #endregion

        #region EnsureSuccessStatusCode Tests

        [Fact]
        public void EnsureSuccessStatusCode_SuccessStatus_ReturnsSelf()
        {
            // Arrange
            var result = new CurlResult
            {
                StatusCode = 200,
            };

            // Act
            var returned = result.EnsureSuccessStatusCode();

            // Assert
            returned.Should().BeSameAs(result);
        }

        [Fact]
        public void EnsureSuccessStatusCode_FailureStatus_ThrowsException()
        {
            // Arrange
            var result = new CurlResult
            {
                StatusCode = 404,
                Body = "Not Found"
            };

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => result.EnsureSuccessStatusCode());
        }

        #endregion

        #region ToSimple Tests

        [Fact]
        public void ToSimple_SuccessResult_ReturnsStatusAndBody()
        {
            // Arrange
            var result = new CurlResult
            {
                StatusCode = 200,
                Body = "test body",
            };

            // Act
            var (success, body, error) = result.ToSimple();

            // Assert
            success.Should().BeTrue();
            body.Should().Be("test body");
            error.Should().BeNull();
        }

        [Fact]
        public void ToSimple_WithNullBody_ReturnsStatusAndNull()
        {
            // Arrange
            var result = new CurlResult
            {
                StatusCode = 204,
                Body = null,
            };

            // Act
            var (success, body, error) = result.ToSimple();

            // Assert
            success.Should().BeTrue();
            body.Should().BeNull();
            error.Should().BeNull();
        }

        #endregion

        #region Helper Classes

        private class TestData
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        #endregion
    }
}
