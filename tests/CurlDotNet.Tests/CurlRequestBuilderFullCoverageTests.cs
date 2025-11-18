using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Comprehensive tests for CurlRequestBuilder to achieve 100% code coverage.
    /// Tests all factory methods, builder methods, and execution paths.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    public class CurlRequestBuilderFullCoverageTests
    {
        #region Factory Method Tests

        [Fact]
        public void Get_CreatesBuilderWithCorrectMethodAndUrl()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com");
            var options = builder.GetOptions();

            // Assert
            options.Method.Should().Be("GET");
            options.Url.Should().Be("https://api.example.com");
        }

        [Fact]
        public void Post_CreatesBuilderWithPostMethod()
        {
            // Act
            var builder = CurlRequestBuilder.Post("https://api.example.com");
            var options = builder.GetOptions();

            // Assert
            options.Method.Should().Be("POST");
            options.Url.Should().Be("https://api.example.com");
        }

        [Fact]
        public void Put_CreatesBuilderWithPutMethod()
        {
            // Act
            var builder = CurlRequestBuilder.Put("https://api.example.com");
            var options = builder.GetOptions();

            // Assert
            options.Method.Should().Be("PUT");
            options.Url.Should().Be("https://api.example.com");
        }

        [Fact]
        public void Delete_CreatesBuilderWithDeleteMethod()
        {
            // Act
            var builder = CurlRequestBuilder.Delete("https://api.example.com");
            var options = builder.GetOptions();

            // Assert
            options.Method.Should().Be("DELETE");
            options.Url.Should().Be("https://api.example.com");
        }

        [Fact]
        public void Patch_CreatesBuilderWithPatchMethod()
        {
            // Act
            var builder = CurlRequestBuilder.Patch("https://api.example.com");
            var options = builder.GetOptions();

            // Assert
            options.Method.Should().Be("PATCH");
            options.Url.Should().Be("https://api.example.com");
        }

        [Fact]
        public void Head_CreatesBuilderWithHeadMethod()
        {
            // Act
            var builder = CurlRequestBuilder.Head("https://api.example.com");
            var options = builder.GetOptions();

            // Assert
            options.Method.Should().Be("HEAD");
            options.Url.Should().Be("https://api.example.com");
        }

        [Fact]
        public void Request_CreatesBuilderWithCustomMethod()
        {
            // Act
            var builder = CurlRequestBuilder.Request("OPTIONS", "https://api.example.com");
            var options = builder.GetOptions();

            // Assert
            options.Method.Should().Be("OPTIONS");
            options.Url.Should().Be("https://api.example.com");
        }

        [Fact]
        public void Request_ConvertsMethodToUpperCase()
        {
            // Act
            var builder = CurlRequestBuilder.Request("options", "https://api.example.com");
            var options = builder.GetOptions();

            // Assert
            options.Method.Should().Be("OPTIONS");
        }

        [Fact]
        public void Get_ThrowsOnNullUrl()
        {
            // Act & Assert
            Action act = () => CurlRequestBuilder.Get(null!);
            act.Should().Throw<ArgumentNullException>().WithParameterName("url");
        }

        #endregion

        #region Header Method Tests

        [Fact]
        public void WithHeader_AddsHeaderToOptions()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithHeader("Accept", "application/json");
            var options = builder.GetOptions();

            // Assert
            options.Headers.Should().ContainKey("Accept");
            options.Headers["Accept"].Should().Be("application/json");
        }

        [Fact]
        public void WithHeader_OverwritesExistingHeader()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithHeader("Accept", "text/html")
                .WithHeader("Accept", "application/json");
            var options = builder.GetOptions();

            // Assert
            options.Headers["Accept"].Should().Be("application/json");
        }

        [Fact]
        public void WithHeaders_AddsMultipleHeaders()
        {
            // Arrange
            var headers = new Dictionary<string, string>
            {
                ["Accept"] = "application/json",
                ["User-Agent"] = "TestAgent",
                ["X-Custom"] = "Value"
            };

            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithHeaders(headers);
            var options = builder.GetOptions();

            // Assert
            options.Headers.Should().HaveCount(3);
            options.Headers["Accept"].Should().Be("application/json");
            options.Headers["User-Agent"].Should().Be("TestAgent");
            options.Headers["X-Custom"].Should().Be("Value");
        }

        [Fact]
        public void WithUserAgent_SetsUserAgentOption()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithUserAgent("MyApp/1.0");
            var options = builder.GetOptions();

            // Assert
            options.UserAgent.Should().Be("MyApp/1.0");
        }

        [Fact]
        public void WithReferer_SetsRefererOption()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithReferer("https://referrer.example.com");
            var options = builder.GetOptions();

            // Assert
            options.Referer.Should().Be("https://referrer.example.com");
        }

        #endregion

        #region Body/Data Method Tests

        [Fact]
        public void WithData_SetsDataAndChangesMethodToPost()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithData("key=value");
            var options = builder.GetOptions();

            // Assert
            options.Data.Should().Be("key=value");
            options.Method.Should().Be("POST");
        }

        [Fact]
        public void WithData_DoesNotChangeMethodIfAlreadyPost()
        {
            // Act
            var builder = CurlRequestBuilder.Put("https://api.example.com")
                .WithData("key=value");
            var options = builder.GetOptions();

            // Assert
            options.Data.Should().Be("key=value");
            options.Method.Should().Be("PUT");
        }

        [Fact]
        public void WithJson_SetsJsonDataAndContentType()
        {
            // Act
            var builder = CurlRequestBuilder.Post("https://api.example.com")
                .WithJson(new { name = "John", age = 30 });
            var options = builder.GetOptions();

            // Assert
            options.Data.Should().Contain("\"name\"");
            options.Data.Should().Contain("\"John\"");
            options.Data.Should().Contain("\"age\"");
            options.Data.Should().Contain("30");
            options.Headers["Content-Type"].Should().Be("application/json");
        }

        [Fact]
        public void WithJson_ChangesMethodToPostForGetRequest()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithJson(new { test = "value" });
            var options = builder.GetOptions();

            // Assert
            options.Method.Should().Be("POST");
        }

        [Fact]
        public void WithFormData_EncodesFormDataCorrectly()
        {
            // Arrange
            var formData = new Dictionary<string, string>
            {
                ["name"] = "John Doe",
                ["email"] = "john@example.com",
                ["special"] = "value&with=special#chars"
            };

            // Act
            var builder = CurlRequestBuilder.Post("https://api.example.com")
                .WithFormData(formData);
            var options = builder.GetOptions();

            // Assert
            options.Data.Should().Contain("name=John%20Doe");
            options.Data.Should().Contain("email=john%40example.com");
            options.Data.Should().Contain("special=value%26with%3Dspecial%23chars");
            options.Headers["Content-Type"].Should().Be("application/x-www-form-urlencoded");
        }

        [Fact]
        public void WithMultipartForm_SetsFormDataAndFiles()
        {
            // Arrange
            var fields = new Dictionary<string, string> { ["field1"] = "value1" };
            var files = new Dictionary<string, string> { ["file1"] = "path/to/file.txt" };

            // Act
            var builder = CurlRequestBuilder.Post("https://api.example.com")
                .WithMultipartForm(fields, files);
            var options = builder.GetOptions();

            // Assert
            options.FormData.Should().ContainKey("field1");
            options.FormData["field1"].Should().Be("value1");
            options.Files.Should().ContainKey("file1");
            options.Files["file1"].Should().Be("path/to/file.txt");
            options.Method.Should().Be("POST");
        }

        [Fact]
        public void WithMultipartForm_HandlesNullFiles()
        {
            // Arrange
            var fields = new Dictionary<string, string> { ["field1"] = "value1" };

            // Act
            var builder = CurlRequestBuilder.Post("https://api.example.com")
                .WithMultipartForm(fields, null);
            var options = builder.GetOptions();

            // Assert
            options.FormData.Should().HaveCount(1);
            options.Files.Should().BeEmpty();
        }

        [Fact]
        public void WithBinaryData_SetsBinaryDataAndChangesMethod()
        {
            // Arrange
            var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };

            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithBinaryData(data);
            var options = builder.GetOptions();

            // Assert
            options.BinaryData.Should().BeEquivalentTo(data);
            options.Method.Should().Be("POST");
        }

        [Fact]
        public void WithFile_AddsFileToOptions()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "test content");

            try
            {
                // Act
                var builder = CurlRequestBuilder.Post("https://api.example.com")
                    .WithFile("document", tempFile);
                var options = builder.GetOptions();

                // Assert
                options.Files.Should().ContainKey("document");
                options.Files["document"].Should().Be(tempFile);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Fact]
        public void WithFile_ThrowsForNonExistentFile()
        {
            // Act & Assert
            Action act = () => CurlRequestBuilder.Post("https://api.example.com")
                .WithFile("document", "nonexistent.txt");
            act.Should().Throw<FileNotFoundException>().WithMessage("*nonexistent.txt*");
        }

        #endregion

        #region Authentication Method Tests

        [Fact]
        public void WithBasicAuth_SetsCredentials()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithBasicAuth("username", "password");
            var options = builder.GetOptions();

            // Assert
            options.Credentials.Should().NotBeNull();
            options.Credentials.UserName.Should().Be("username");
            options.Credentials.Password.Should().Be("password");
        }

        [Fact]
        public void WithBearerToken_SetsAuthorizationHeader()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithBearerToken("token123");
            var options = builder.GetOptions();

            // Assert
            options.Headers["Authorization"].Should().Be("Bearer token123");
        }

        [Fact]
        public void WithAuth_SetsCustomAuthorizationHeader()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithAuth("Basic dXNlcjpwYXNz");
            var options = builder.GetOptions();

            // Assert
            options.Headers["Authorization"].Should().Be("Basic dXNlcjpwYXNz");
        }

        #endregion

        #region Options Method Tests

        [Fact]
        public void WithTimeout_SetsMaxTimeInSeconds()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithTimeout(TimeSpan.FromSeconds(30));
            var options = builder.GetOptions();

            // Assert
            options.MaxTime.Should().Be(30);
        }

        [Fact]
        public void WithConnectTimeout_SetsConnectTimeoutInSeconds()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithConnectTimeout(TimeSpan.FromSeconds(10));
            var options = builder.GetOptions();

            // Assert
            options.ConnectTimeout.Should().Be(10);
        }

        [Fact]
        public void FollowRedirects_EnablesRedirectFollowing()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .FollowRedirects();
            var options = builder.GetOptions();

            // Assert
            options.FollowLocation.Should().BeTrue();
        }

        [Fact]
        public void FollowRedirects_CanBeDisabled()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .FollowRedirects(false);
            var options = builder.GetOptions();

            // Assert
            options.FollowLocation.Should().BeFalse();
        }

        [Fact]
        public void WithMaxRedirects_SetsMaximumRedirects()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithMaxRedirects(5);
            var options = builder.GetOptions();

            // Assert
            options.MaxRedirects.Should().Be(5);
        }

        [Fact]
        public void Insecure_EnablesInsecureMode()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .Insecure();
            var options = builder.GetOptions();

            // Assert
            options.Insecure.Should().BeTrue();
        }

        [Fact]
        public void Insecure_CanBeDisabled()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .Insecure(false);
            var options = builder.GetOptions();

            // Assert
            options.Insecure.Should().BeFalse();
        }

        [Fact]
        public void WithProxy_SetsProxyUrl()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithProxy("http://proxy.example.com:8080");
            var options = builder.GetOptions();

            // Assert
            options.Proxy.Should().Be("http://proxy.example.com:8080");
        }

        [Fact]
        public void WithProxy_SetsProxyWithCredentials()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithProxy("http://proxy.example.com:8080", "proxyuser", "proxypass");
            var options = builder.GetOptions();

            // Assert
            options.Proxy.Should().Be("http://proxy.example.com:8080");
            options.ProxyCredentials.Should().NotBeNull();
            options.ProxyCredentials.UserName.Should().Be("proxyuser");
            options.ProxyCredentials.Password.Should().Be("proxypass");
        }

        [Fact]
        public void WithCookie_SetsCookieString()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithCookie("session=abc123; user=john");
            var options = builder.GetOptions();

            // Assert
            options.Cookie.Should().Be("session=abc123; user=john");
        }

        [Fact]
        public void WithCookieJar_SetsCookieJarPath()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithCookieJar("/tmp/cookies.txt");
            var options = builder.GetOptions();

            // Assert
            options.CookieJar.Should().Be("/tmp/cookies.txt");
        }

        [Fact]
        public void SaveToFile_SetsOutputFile()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .SaveToFile("/tmp/output.json");
            var options = builder.GetOptions();

            // Assert
            options.OutputFile.Should().Be("/tmp/output.json");
        }

        [Fact]
        public void SaveWithRemoteName_EnablesRemoteFileName()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com/file.pdf")
                .SaveWithRemoteName();
            var options = builder.GetOptions();

            // Assert
            options.UseRemoteFileName.Should().BeTrue();
        }

        [Fact]
        public void IncludeHeaders_EnablesHeaderInclusion()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .IncludeHeaders();
            var options = builder.GetOptions();

            // Assert
            options.IncludeHeaders.Should().BeTrue();
        }

        [Fact]
        public void IncludeHeaders_CanBeDisabled()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .IncludeHeaders(false);
            var options = builder.GetOptions();

            // Assert
            options.IncludeHeaders.Should().BeFalse();
        }

        [Fact]
        public void Verbose_EnablesVerboseMode()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .Verbose();
            var options = builder.GetOptions();

            // Assert
            options.Verbose.Should().BeTrue();
        }

        [Fact]
        public void Verbose_CanBeDisabled()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .Verbose(false);
            var options = builder.GetOptions();

            // Assert
            options.Verbose.Should().BeFalse();
        }

        [Fact]
        public void Silent_EnablesSilentMode()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .Silent();
            var options = builder.GetOptions();

            // Assert
            options.Silent.Should().BeTrue();
        }

        [Fact]
        public void Silent_CanBeDisabled()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .Silent(false);
            var options = builder.GetOptions();

            // Assert
            options.Silent.Should().BeFalse();
        }

        [Fact]
        public void FailOnError_EnablesFailOnError()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .FailOnError();
            var options = builder.GetOptions();

            // Assert
            options.FailOnError.Should().BeTrue();
        }

        [Fact]
        public void FailOnError_CanBeDisabled()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .FailOnError(false);
            var options = builder.GetOptions();

            // Assert
            options.FailOnError.Should().BeFalse();
        }

        [Fact]
        public void WithHttpVersion_SetsHttpVersion()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithHttpVersion("2.0");
            var options = builder.GetOptions();

            // Assert
            options.HttpVersion.Should().Be("2.0");
        }

        [Fact]
        public void Compressed_EnablesCompression()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .Compressed();
            var options = builder.GetOptions();

            // Assert
            options.Compressed.Should().BeTrue();
        }

        [Fact]
        public void Compressed_CanBeDisabled()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .Compressed(false);
            var options = builder.GetOptions();

            // Assert
            options.Compressed.Should().BeFalse();
        }

        [Fact]
        public void WithRange_SetsRangeHeader()
        {
            // Act
            var builder = CurlRequestBuilder.Get("https://api.example.com/file.bin")
                .WithRange("0-1023");
            var options = builder.GetOptions();

            // Assert
            options.Range.Should().Be("0-1023");
        }

        #endregion

        #region ToCurlCommand Tests

        [Fact]
        public void ToCurlCommand_GeneratesCorrectCommandForGet()
        {
            // Act
            var command = CurlRequestBuilder.Get("https://api.example.com")
                .ToCurlCommand();

            // Assert
            command.Should().StartWith("curl");
            command.Should().EndWith("https://api.example.com");
            command.Should().NotContain("-X GET"); // GET is default
        }

        [Fact]
        public void ToCurlCommand_IncludesMethodForNonGet()
        {
            // Act
            var command = CurlRequestBuilder.Post("https://api.example.com")
                .ToCurlCommand();

            // Assert
            command.Should().Contain("-X POST");
        }

        [Fact]
        public void ToCurlCommand_IncludesHeaders()
        {
            // Act
            var command = CurlRequestBuilder.Get("https://api.example.com")
                .WithHeader("Accept", "application/json")
                .WithHeader("X-Custom", "Value")
                .ToCurlCommand();

            // Assert
            command.Should().Contain("-H 'Accept: application/json'");
            command.Should().Contain("-H 'X-Custom: Value'");
        }

        [Fact]
        public void ToCurlCommand_IncludesUserAgent()
        {
            // Act
            var command = CurlRequestBuilder.Get("https://api.example.com")
                .WithUserAgent("MyApp/1.0")
                .ToCurlCommand();

            // Assert
            command.Should().Contain("-A 'MyApp/1.0'");
        }

        [Fact]
        public void ToCurlCommand_IncludesReferer()
        {
            // Act
            var command = CurlRequestBuilder.Get("https://api.example.com")
                .WithReferer("https://referrer.com")
                .ToCurlCommand();

            // Assert
            command.Should().Contain("-e 'https://referrer.com'");
        }

        [Fact]
        public void ToCurlCommand_IncludesData()
        {
            // Act
            var command = CurlRequestBuilder.Post("https://api.example.com")
                .WithData("key=value")
                .ToCurlCommand();

            // Assert
            command.Should().Contain("-d 'key=value'");
        }

        [Fact]
        public void ToCurlCommand_IncludesFormData()
        {
            // Act
            var command = CurlRequestBuilder.Post("https://api.example.com")
                .WithMultipartForm(
                    new Dictionary<string, string> { ["field"] = "value" },
                    new Dictionary<string, string> { ["file"] = "path.txt" })
                .ToCurlCommand();

            // Assert
            command.Should().Contain("-F 'field=value'");
            command.Should().Contain("-F 'file=@path.txt'");
        }

        [Fact]
        public void ToCurlCommand_IncludesBasicAuth()
        {
            // Act
            var command = CurlRequestBuilder.Get("https://api.example.com")
                .WithBasicAuth("user", "pass")
                .ToCurlCommand();

            // Assert
            command.Should().Contain("-u 'user:pass'");
        }

        [Fact]
        public void ToCurlCommand_IncludesAllOptions()
        {
            // Act
            var command = CurlRequestBuilder.Get("https://api.example.com")
                .FollowRedirects()
                .Insecure()
                .Verbose()
                .Silent()
                .FailOnError()
                .IncludeHeaders()
                .Compressed()
                .WithTimeout(TimeSpan.FromSeconds(30))
                .WithConnectTimeout(TimeSpan.FromSeconds(10))
                .WithMaxRedirects(5)
                .WithProxy("http://proxy:8080")
                .WithCookie("session=123")
                .WithCookieJar("/tmp/cookies")
                .SaveToFile("/tmp/output")
                .SaveWithRemoteName()
                .ToCurlCommand();

            // Assert
            command.Should().Contain("-L");
            command.Should().Contain("-k");
            command.Should().Contain("-v");
            command.Should().Contain("-s");
            command.Should().Contain("-f");
            command.Should().Contain("-i");
            command.Should().Contain("--compressed");
            command.Should().Contain("--max-time 30");
            command.Should().Contain("--connect-timeout 10");
            command.Should().Contain("--max-redirs 5");
            command.Should().Contain("-x http://proxy:8080");
            command.Should().Contain("-b 'session=123'");
            command.Should().Contain("-c /tmp/cookies");
            command.Should().Contain("-o /tmp/output");
            command.Should().Contain("-O");
        }

        [Fact]
        public void ToCurlCommand_DoesNotIncludeDefaultMaxRedirects()
        {
            // Act
            var command = CurlRequestBuilder.Get("https://api.example.com")
                .FollowRedirects()
                .ToCurlCommand();

            // Assert (default is 50, shouldn't be included)
            command.Should().NotContain("--max-redirs");
        }

        #endregion

        #region Method Chaining Tests

        [Fact]
        public void MethodChaining_WorksCorrectly()
        {
            // Act
            var builder = CurlRequestBuilder
                .Post("https://api.example.com")
                .WithHeader("Accept", "application/json")
                .WithBearerToken("token123")
                .WithJson(new { test = "value" })
                .FollowRedirects()
                .WithTimeout(TimeSpan.FromSeconds(30))
                .Verbose();

            var options = builder.GetOptions();

            // Assert
            options.Method.Should().Be("POST");
            options.Headers["Accept"].Should().Be("application/json");
            options.Headers["Authorization"].Should().Be("Bearer token123");
            options.Headers["Content-Type"].Should().Be("application/json");
            options.FollowLocation.Should().BeTrue();
            options.MaxTime.Should().Be(30);
            options.Verbose.Should().BeTrue();
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void GetOptions_ReturnsClonedOptions()
        {
            // Arrange
            var builder = CurlRequestBuilder.Get("https://api.example.com")
                .WithHeader("Test", "Value");

            // Act
            var options1 = builder.GetOptions();
            var options2 = builder.GetOptions();

            // Assert
            options1.Should().NotBeSameAs(options2);
            options1.Headers.Should().BeEquivalentTo(options2.Headers);
        }

        [Fact]
        public void WithFormData_HandlesEmptyDictionary()
        {
            // Act
            var builder = CurlRequestBuilder.Post("https://api.example.com")
                .WithFormData(new Dictionary<string, string>());
            var options = builder.GetOptions();

            // Assert
            options.Data.Should().BeEmpty();
            options.Headers["Content-Type"].Should().Be("application/x-www-form-urlencoded");
        }

        [Fact]
        public void WithMultipartForm_HandlesNullFields()
        {
            // Act
            var builder = CurlRequestBuilder.Post("https://api.example.com")
                .WithMultipartForm(null, null);
            var options = builder.GetOptions();

            // Assert
            options.FormData.Should().BeEmpty();
            options.Files.Should().BeEmpty();
        }

        #endregion
    }
}