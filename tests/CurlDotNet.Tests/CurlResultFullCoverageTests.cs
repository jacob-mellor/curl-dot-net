using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CurlDotNet.Core;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Comprehensive tests for CurlResult to achieve 100% code coverage.
    /// Tests all branches, error paths, and edge cases.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    public class CurlResultFullCoverageTests : IDisposable
    {
        private readonly string _testDirectory = Path.Combine(Path.GetTempPath(), $"curl-tests-{Guid.NewGuid()}");

        public CurlResultFullCoverageTests()
        {
            Directory.CreateDirectory(_testDirectory);
        }

        #region Core Properties Tests

        [Fact]
        public void IsSuccess_ReturnsTrueForSuccessCodes()
        {
            var testCases = new[] { 200, 201, 204, 299 };
            foreach (var code in testCases)
            {
                var result = new CurlResult { StatusCode = code };
                result.IsSuccess.Should().BeTrue($"Status {code} should be considered success");
            }
        }

        [Fact]
        public void IsSuccess_ReturnsFalseForNonSuccessCodes()
        {
            var testCases = new[] { 100, 199, 300, 400, 404, 500, 503 };
            foreach (var code in testCases)
            {
                var result = new CurlResult { StatusCode = code };
                result.IsSuccess.Should().BeFalse($"Status {code} should not be considered success");
            }
        }

        [Fact]
        public void IsBinary_ReturnsTrueWhenBinaryDataExists()
        {
            var result = new CurlResult { BinaryData = new byte[] { 1, 2, 3 } };
            result.IsBinary.Should().BeTrue();
        }

        [Fact]
        public void IsBinary_ReturnsFalseWhenBinaryDataIsNullOrEmpty()
        {
            var result1 = new CurlResult { BinaryData = null };
            result1.IsBinary.Should().BeFalse();

            var result2 = new CurlResult { BinaryData = new byte[0] };
            result2.IsBinary.Should().BeFalse();
        }

        #endregion

        #region JSON Operations Tests

        [Fact]
        public void ParseJson_ThrowsWhenBodyIsNull()
        {
            var result = new CurlResult { Body = null };
            Action act = () => result.ParseJson<Dictionary<string, string>>();
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Cannot parse JSON: Body is empty");
        }

        [Fact]
        public void ParseJson_ThrowsWhenBodyIsEmpty()
        {
            var result = new CurlResult { Body = "" };
            Action act = () => result.ParseJson<Dictionary<string, string>>();
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Cannot parse JSON: Body is empty");
        }

        [Fact]
        public void ParseJson_ThrowsWhenJsonIsInvalid()
        {
            var result = new CurlResult { Body = "not valid json" };
            Action act = () => result.ParseJson<Dictionary<string, string>>();
            act.Should().Throw<InvalidOperationException>()
                .Which.Message.Should().Contain("Failed to parse JSON");
        }

        [Fact]
        public void ParseJson_SuccessfullyParsesComplexTypes()
        {
            var result = new CurlResult
            {
                Body = "[{\"Id\":1,\"Name\":\"Test\"},{\"Id\":2,\"Name\":\"Test2\"}]"
            };

            var items = result.ParseJson<List<TestItem>>();
            items.Should().HaveCount(2);
            items[0].Id.Should().Be(1);
            items[0].Name.Should().Be("Test");
        }

        [Fact]
        public void AsJson_IsAliasForParseJson()
        {
            var result = new CurlResult
            {
                Body = "{\"value\":\"test\"}"
            };

            var parsed1 = result.ParseJson<Dictionary<string, string>>();
            var parsed2 = result.AsJson<Dictionary<string, string>>();

            parsed1["value"].Should().Be(parsed2["value"]);
        }

        [Fact]
        public void AsJsonDynamic_ParsesDynamicJson()
        {
            var result = new CurlResult
            {
                Body = "{\"name\":\"John\",\"age\":30}"
            };

            dynamic json = result.AsJsonDynamic();
            Assert.NotNull(json);
            // The actual dynamic access would vary between .NET versions
        }

        #endregion

        #region Save Operations Tests

        [Fact]
        public void SaveToFile_SavesTextBody()
        {
            var filePath = Path.Combine(_testDirectory, "text-output.txt");
            var result = new CurlResult
            {
                Body = "Test content",
                BinaryData = null
            };

            var returned = result.SaveToFile(filePath);

            returned.Should().BeSameAs(result); // Returns this for chaining
            File.ReadAllText(filePath).Should().Be("Test content");
            result.OutputFiles.Should().Contain(filePath);
        }

        [Fact]
        public void SaveToFile_SavesBinaryData()
        {
            var filePath = Path.Combine(_testDirectory, "binary-output.bin");
            var binaryData = new byte[] { 0xFF, 0xFE, 0xFD };
            var result = new CurlResult
            {
                Body = "This should be ignored",
                BinaryData = binaryData
            };

            result.SaveToFile(filePath);

            File.ReadAllBytes(filePath).Should().BeEquivalentTo(binaryData);
            result.OutputFiles.Should().Contain(filePath);
        }

        [Fact]
        public void SaveToFile_HandlesNullBody()
        {
            var filePath = Path.Combine(_testDirectory, "null-body.txt");
            var result = new CurlResult
            {
                Body = null,
                BinaryData = null
            };

            result.SaveToFile(filePath);

            File.ReadAllText(filePath).Should().BeEmpty();
        }

        [Fact]
        public async Task SaveToFileAsync_SavesTextBodyAsync()
        {
            var filePath = Path.Combine(_testDirectory, "async-text.txt");
            var result = new CurlResult
            {
                Body = "Async test content"
            };

            var returned = await result.SaveToFileAsync(filePath);

            returned.Should().BeSameAs(result);
            File.ReadAllText(filePath).Should().Be("Async test content");
            result.OutputFiles.Should().Contain(filePath);
        }

        [Fact]
        public async Task SaveToFileAsync_SavesBinaryDataAsync()
        {
            var filePath = Path.Combine(_testDirectory, "async-binary.bin");
            var binaryData = new byte[] { 0x01, 0x02, 0x03 };
            var result = new CurlResult
            {
                BinaryData = binaryData
            };

            await result.SaveToFileAsync(filePath);

            File.ReadAllBytes(filePath).Should().BeEquivalentTo(binaryData);
        }

        [Fact]
        public async Task SaveToFileAsync_HandlesNullBodyAsync()
        {
            var filePath = Path.Combine(_testDirectory, "async-null.txt");
            var result = new CurlResult
            {
                Body = null,
                BinaryData = null
            };

            await result.SaveToFileAsync(filePath);

            File.ReadAllText(filePath).Should().BeEmpty();
        }

        [Fact]
        public void SaveAsJson_FormatsJsonProperly()
        {
            var filePath = Path.Combine(_testDirectory, "formatted.json");
            var result = new CurlResult
            {
                Body = "{\"name\":\"John\",\"age\":30}"
            };

            result.SaveAsJson(filePath, indented: true);

            var content = File.ReadAllText(filePath);
            content.Should().Contain("\n"); // Should be indented
            result.OutputFiles.Should().Contain(filePath);
        }

        [Fact]
        public void SaveAsJson_MinifiesWhenIndentedFalse()
        {
            var filePath = Path.Combine(_testDirectory, "minified.json");
            var result = new CurlResult
            {
                Body = "{\"name\":\"John\",\"age\":30}"
            };

            result.SaveAsJson(filePath, indented: false);

            var content = File.ReadAllText(filePath);
            content.Should().NotContain("\n");
        }

        [Fact]
        public void SaveAsJson_HandlesInvalidJson()
        {
            var filePath = Path.Combine(_testDirectory, "invalid.json");
            var result = new CurlResult
            {
                Body = "Not valid JSON"
            };

            result.SaveAsJson(filePath);

            File.ReadAllText(filePath).Should().Be("Not valid JSON");
        }

        [Fact]
        public void SaveAsCsv_ConvertsJsonArrayToCsv()
        {
            var filePath = Path.Combine(_testDirectory, "data.csv");
            var result = new CurlResult
            {
                Body = "[{\"name\":\"John\",\"age\":30},{\"name\":\"Jane\",\"age\":25}]"
            };

            result.SaveAsCsv(filePath);

            var content = File.ReadAllText(filePath);
            content.Should().Contain("name,age");
            content.Should().Contain("John,30");
            content.Should().Contain("Jane,25");
            result.OutputFiles.Should().Contain(filePath);
        }

        [Fact]
        public void SaveAsCsv_HandlesEmptyArray()
        {
            var filePath = Path.Combine(_testDirectory, "empty.csv");
            var result = new CurlResult
            {
                Body = "[]"
            };

            result.SaveAsCsv(filePath);

            File.ReadAllText(filePath).Should().BeEmpty();
        }

        [Fact]
        public void SaveAsCsv_HandlesValuesWithCommasAndQuotes()
        {
            var filePath = Path.Combine(_testDirectory, "special.csv");
            var result = new CurlResult
            {
                Body = "[{\"text\":\"Hello, World\",\"quote\":\"He said \\\"Hi\\\"\"}]"
            };

            result.SaveAsCsv(filePath);

            var content = File.ReadAllText(filePath);
            content.Should().Contain("\"Hello, World\"");
            content.Should().Contain("\"\""); // Escaped quotes
        }

        [Fact]
        public void SaveAsCsv_HandlesInvalidJson()
        {
            var filePath = Path.Combine(_testDirectory, "invalid.csv");
            var result = new CurlResult
            {
                Body = "Not valid JSON"
            };

            result.SaveAsCsv(filePath);

            File.ReadAllText(filePath).Should().Be("Not valid JSON");
        }

        [Fact]
        public void AppendToFile_AppendsTextBody()
        {
            var filePath = Path.Combine(_testDirectory, "append.txt");
            File.WriteAllText(filePath, "Initial content\n");

            var result = new CurlResult
            {
                Body = "Appended content"
            };

            var returned = result.AppendToFile(filePath);

            returned.Should().BeSameAs(result);
            File.ReadAllText(filePath).Should().Be("Initial content\nAppended content");
        }

        [Fact]
        public void AppendToFile_AppendsBinaryData()
        {
            var filePath = Path.Combine(_testDirectory, "append.bin");
            File.WriteAllBytes(filePath, new byte[] { 0x01, 0x02 });

            var result = new CurlResult
            {
                BinaryData = new byte[] { 0x03, 0x04 }
            };

            result.AppendToFile(filePath);

            File.ReadAllBytes(filePath).Should().BeEquivalentTo(new byte[] { 0x01, 0x02, 0x03, 0x04 });
        }

        [Fact]
        public void AppendToFile_HandlesNullBody()
        {
            var filePath = Path.Combine(_testDirectory, "append-null.txt");
            File.WriteAllText(filePath, "Initial");

            var result = new CurlResult
            {
                Body = null,
                BinaryData = null
            };

            result.AppendToFile(filePath);

            File.ReadAllText(filePath).Should().Be("Initial");
        }

        #endregion

        #region Header Operations Tests

        [Fact]
        public void GetHeader_ReturnsHeaderValueCaseInsensitive()
        {
            var result = new CurlResult();
            result.Headers["Content-Type"] = "application/json";

            result.GetHeader("content-type").Should().Be("application/json");
            result.GetHeader("CONTENT-TYPE").Should().Be("application/json");
            result.GetHeader("Content-Type").Should().Be("application/json");
        }

        [Fact]
        public void GetHeader_ReturnsNullForMissingHeader()
        {
            var result = new CurlResult();
            result.GetHeader("Non-Existent").Should().BeNull();
        }

        [Fact]
        public void HasHeader_ReturnsTrueForExistingHeader()
        {
            var result = new CurlResult();
            result.Headers["X-Custom"] = "value";

            result.HasHeader("X-Custom").Should().BeTrue();
            result.HasHeader("x-custom").Should().BeTrue();
        }

        [Fact]
        public void HasHeader_ReturnsFalseForMissingHeader()
        {
            var result = new CurlResult();
            result.HasHeader("Missing").Should().BeFalse();
        }

        #endregion

        #region Validation Operations Tests

        [Fact]
        public void EnsureSuccess_DoesNotThrowForSuccessCodes()
        {
            var successCodes = new[] { 200, 201, 204, 299 };
            foreach (var code in successCodes)
            {
                var result = new CurlResult { StatusCode = code };
                var returned = result.EnsureSuccess();
                returned.Should().BeSameAs(result); // Returns this for chaining
            }
        }

        [Fact]
        public void EnsureSuccess_ThrowsForNonSuccessCodes()
        {
            var result = new CurlResult
            {
                StatusCode = 404,
                Body = "Not Found",
                Headers = new Dictionary<string, string> { ["Server"] = "Test" }
            };

            Action act = () => result.EnsureSuccess();

            var exception = act.Should().Throw<CurlHttpException>().Which;
            exception.StatusCode.Should().Be(404);
            exception.ResponseBody.Should().Be("Not Found");
            exception.ResponseHeaders.Should().ContainKey("Server");
        }

        [Fact]
        public void EnsureStatus_DoesNotThrowWhenStatusMatches()
        {
            var result = new CurlResult { StatusCode = 201 };
            var returned = result.EnsureStatus(201);
            returned.Should().BeSameAs(result);
        }

        [Fact]
        public void EnsureStatus_ThrowsWhenStatusDoesNotMatch()
        {
            var result = new CurlResult { StatusCode = 200 };

            Action act = () => result.EnsureStatus(201);

            act.Should().Throw<CurlHttpException>()
                .Which.StatusCode.Should().Be(200);
        }

        [Fact]
        public void EnsureContains_DoesNotThrowWhenTextExists()
        {
            var result = new CurlResult { Body = "Hello World" };
            var returned = result.EnsureContains("World");
            returned.Should().BeSameAs(result);
        }

        [Fact]
        public void EnsureContains_ThrowsWhenTextNotFound()
        {
            var result = new CurlResult { Body = "Hello World" };

            Action act = () => result.EnsureContains("Missing");

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Response does not contain 'Missing'");
        }

        [Fact]
        public void EnsureContains_ThrowsWhenBodyIsNull()
        {
            var result = new CurlResult { Body = null };

            Action act = () => result.EnsureContains("Text");

            act.Should().Throw<InvalidOperationException>();
        }

        #endregion

        #region Retry Operations Tests

        [Fact]
        public async Task Retry_ThrowsWhenCommandIsNull()
        {
            var result = new CurlResult { Command = null };

            Func<Task> act = async () => await result.Retry();

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Cannot retry: Original command not available");
        }

        [Fact]
        public async Task Retry_ThrowsWhenCommandIsEmpty()
        {
            var result = new CurlResult { Command = "" };

            Func<Task> act = async () => await result.Retry();

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task RetryWith_ThrowsWhenCommandIsNull()
        {
            var result = new CurlResult { Command = null };

            Func<Task> act = async () => await result.RetryWith(s => s.MaxTimeSeconds = 30);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        #endregion

        #region Display Operations Tests

        [Fact]
        public void PrintBody_OutputsBodyAndReturnsResult()
        {
            var result = new CurlResult { Body = "Test output" };
            using var writer = new StringWriter();
            Console.SetOut(writer);

            var returned = result.PrintBody();

            returned.Should().BeSameAs(result);
            writer.ToString().Should().Contain("Test output");
        }

        [Fact]
        public void Print_OutputsStatusAndBody()
        {
            var result = new CurlResult
            {
                StatusCode = 200,
                Body = "Success"
            };
            using var writer = new StringWriter();
            Console.SetOut(writer);

            var returned = result.Print();

            returned.Should().BeSameAs(result);
            var output = writer.ToString();
            output.Should().Contain("Status: 200");
            output.Should().Contain("Success");
        }

        [Fact]
        public void PrintVerbose_OutputsEverything()
        {
            var result = new CurlResult
            {
                StatusCode = 200,
                Body = "Success",
                Headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "text/plain",
                    ["Content-Length"] = "7"
                },
                Timings = new CurlTimings
                {
                    NameLookup = 10,
                    Connect = 20,
                    Total = 100
                }
            };
            using var writer = new StringWriter();
            Console.SetOut(writer);

            var returned = result.PrintVerbose();

            returned.Should().BeSameAs(result);
            var output = writer.ToString();
            output.Should().Contain("Status: 200");
            output.Should().Contain("Content-Type: text/plain");
            output.Should().Contain("DNS: 10ms");
            output.Should().Contain("Total: 100ms");
        }

        [Fact]
        public void PrintVerbose_HandlesNullTimings()
        {
            var result = new CurlResult
            {
                StatusCode = 200,
                Body = "Success",
                Timings = null
            };
            using var writer = new StringWriter();
            Console.SetOut(writer);

            Action act = () => result.PrintVerbose();

            act.Should().NotThrow();
        }

        #endregion

        #region Transformation Operations Tests

        [Fact]
        public void Transform_AppliesTransformationFunction()
        {
            var result = new CurlResult
            {
                StatusCode = 200,
                Body = "{\"value\":42}"
            };

            var transformed = result.Transform(r => new
            {
                IsOk = r.IsSuccess,
                Length = r.Body?.Length ?? 0
            });

            transformed.IsOk.Should().BeTrue();
            transformed.Length.Should().Be(12);
        }

        [Fact]
        public void ToStream_CreatesBinaryStream()
        {
            var binaryData = new byte[] { 0x01, 0x02, 0x03 };
            var result = new CurlResult { BinaryData = binaryData };

            using var stream = result.ToStream();

            var buffer = new byte[3];
            stream.Read(buffer, 0, 3);
            buffer.Should().BeEquivalentTo(binaryData);
        }

        [Fact]
        public void ToStream_CreatesTextStream()
        {
            var result = new CurlResult { Body = "Hello" };

            using var stream = result.ToStream();
            using var reader = new StreamReader(stream);

            reader.ReadToEnd().Should().Be("Hello");
        }

        [Fact]
        public void ToStream_CreatesEmptyStreamWhenBothNull()
        {
            var result = new CurlResult
            {
                Body = null,
                BinaryData = null
            };

            using var stream = result.ToStream();

            stream.Length.Should().Be(0);
        }

        [Fact]
        public void FilterLines_FiltersBodyLines()
        {
            var result = new CurlResult
            {
                Body = "Line 1\nERROR: Something\nLine 3\nERROR: Another"
            };

            var returned = result.FilterLines(line => line.Contains("ERROR"));

            returned.Should().BeSameAs(result);
            result.Body.Should().Be("ERROR: Something\nERROR: Another");
        }

        [Fact]
        public void FilterLines_HandlesNullBody()
        {
            var result = new CurlResult { Body = null };

            var returned = result.FilterLines(line => true);

            returned.Should().BeSameAs(result);
            result.Body.Should().BeNull();
        }

        [Fact]
        public void FilterLines_HandlesEmptyLines()
        {
            var result = new CurlResult
            {
                Body = "Line 1\n\nLine 2\n\n\nLine 3"
            };

            result.FilterLines(line => !string.IsNullOrWhiteSpace(line));

            result.Body.Should().Be("Line 1\nLine 2\nLine 3");
        }

        #endregion

        #region CurlHttpException Tests

        [Fact]
        public void CurlHttpException_StoresPropertiesCorrectly()
        {
            var exception = new CurlHttpException("Test error", 404)
            {
                ResponseBody = "Not Found",
                ResponseHeaders = new Dictionary<string, string> { ["Server"] = "Test" }
            };

            exception.Message.Should().Be("Test error");
            exception.StatusCode.Should().Be(404);
            exception.ResponseBody.Should().Be("Not Found");
            exception.ResponseHeaders["Server"].Should().Be("Test");
        }

        #endregion

        #region CurlTimings Tests

        [Fact]
        public void CurlTimings_AllPropertiesCanBeSetAndRead()
        {
            var timings = new CurlTimings
            {
                NameLookup = 10.5,
                Connect = 20.3,
                AppConnect = 30.1,
                PreTransfer = 40.2,
                Redirect = 50.4,
                StartTransfer = 60.7,
                Total = 100.9
            };

            timings.NameLookup.Should().Be(10.5);
            timings.Connect.Should().Be(20.3);
            timings.AppConnect.Should().Be(30.1);
            timings.PreTransfer.Should().Be(40.2);
            timings.Redirect.Should().Be(50.4);
            timings.StartTransfer.Should().Be(60.7);
            timings.Total.Should().Be(100.9);
        }

        #endregion

        // Helper class for testing
        private class TestItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        // Cleanup
        public void Dispose()
        {
            try
            {
                Directory.Delete(_testDirectory, true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}