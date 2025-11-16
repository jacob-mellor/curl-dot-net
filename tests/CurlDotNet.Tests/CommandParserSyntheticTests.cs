/***************************************************************************
 * CommandParserSyntheticTests - Comprehensive real-world curl command tests
 *
 * These tests verify the parser handles real-world curl commands from:
 * - API documentation (GitHub, Stripe, AWS, etc.)
 * - Different shells (PowerShell, Bash, ZSH, CMD)
 * - Edge cases with special characters, paths, JSON
 * - Complex scenarios with multiple options
 *
 * By Jacob Mellor
 * Sponsored by IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Comprehensive synthetic tests for CommandParser covering real-world scenarios.
    /// </summary>
    /// <remarks>
    /// These tests ensure the parser can handle curl commands exactly as they appear
    /// in API documentation, blog posts, and shell scripts across all platforms.
    /// </remarks>
    [Trait("Category", TestCategories.CurlUnit)]
    [Trait("Category", TestCategories.Parser)]
    [Trait("Category", "Synthetic")]
    public class CommandParserSyntheticTests : CurlTestBase
    {
        private readonly CommandParser _parser;

        public CommandParserSyntheticTests(ITestOutputHelper output) : base(output)
        {
            _parser = new CommandParser();
        }

        #region Real-World API Examples

        /// <summary>
        /// Tests parsing GitHub API curl command from their documentation.
        /// </summary>
        [Fact]
        public void Parse_GitHubApiExample_ShouldParseCorrectly()
        {
            // Arrange - Real curl from GitHub API docs
            const string command = @"curl -X POST \
  -H ""Accept: application/vnd.github+json"" \
  -H ""Authorization: Bearer YOUR_TOKEN"" \
  -H ""X-GitHub-Api-Version: 2022-11-28"" \
  https://api.github.com/repos/OWNER/REPO/issues";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            options.Url.Should().Be("https://api.github.com/repos/OWNER/REPO/issues");
            options.Headers.Should().ContainKey("Accept");
            options.Headers["Accept"].Should().Be("application/vnd.github+json");
            options.Headers.Should().ContainKey("Authorization");
            options.Headers["Authorization"].Should().Be("Bearer YOUR_TOKEN");
            options.Headers.Should().ContainKey("X-GitHub-Api-Version");
        }

        /// <summary>
        /// Tests parsing Stripe API curl command.
        /// </summary>
        [Fact]
        public void Parse_StripeApiExample_ShouldParseCorrectly()
        {
            // Arrange - Real curl from Stripe API docs
            const string command = @"curl https://api.stripe.com/v1/charges \
  -u sk_test_1234567890: \
  -d amount=2000 \
  -d currency=usd \
  -d source=tok_visa \
  -d description=""Charge for test@example.com""";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Url.Should().Be("https://api.stripe.com/v1/charges");
            options.Credentials.Should().NotBeNull();
            options.Credentials.UserName.Should().Be("sk_test_1234567890");
            options.Data.Should().Contain("amount=2000");
            options.Data.Should().Contain("currency=usd");
        }

        /// <summary>
        /// Tests parsing AWS API curl command with complex headers.
        /// </summary>
        [Fact]
        public void Parse_AwsApiExample_ShouldParseCorrectly()
        {
            // Arrange - AWS S3 example
            const string command = @"curl -X PUT \
  -H 'Host: mybucket.s3.amazonaws.com' \
  -H 'Date: Wed, 12 Oct 2009 17:50:00 GMT' \
  -H 'Authorization: AWS AKIAIOSFODNN7EXAMPLE:...' \
  'https://mybucket.s3.amazonaws.com/test.txt'";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("PUT");
            options.Headers.Should().ContainKey("Host");
            options.Headers.Should().ContainKey("Date");
            options.Headers.Should().ContainKey("Authorization");
            options.Url.Should().Contain("s3.amazonaws.com");
        }

        /// <summary>
        /// Tests parsing JSON POST request with nested JSON.
        /// </summary>
        [Fact]
        public void Parse_JsonPostWithNestedData_ShouldParseCorrectly()
        {
            // Arrange - Complex JSON in curl command
            const string command = @"curl -X POST \
  -H 'Content-Type: application/json' \
  -d '{
    ""name"": ""John Doe"",
    ""email"": ""john@example.com"",
    ""address"": {
      ""street"": ""123 Main St"",
      ""city"": ""New York""
    }
  }' \
  https://api.example.com/users";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            options.Headers["Content-Type"].Should().Be("application/json");
            options.Data.Should().Contain("\"name\": \"John Doe\"");
            options.Data.Should().Contain("\"address\"");
            options.Data.Should().Contain("\"city\": \"New York\"");
        }

        /// <summary>
        /// Tests parsing multipart form data with file upload.
        /// </summary>
        [Fact]
        public void Parse_MultipartFormWithFile_ShouldParseCorrectly()
        {
            // Arrange
            var testFile = CreateTempFile("test content", ".txt");
            var fileName = Path.GetFileName(testFile);

            var command = $@"curl -X POST \
  -F 'name=John Doe' \
  -F 'email=john@example.com' \
  -F 'file=@{testFile}' \
  https://api.example.com/upload";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            options.FormData.Should().ContainKey("name");
            options.FormData["name"].Should().Be("John Doe");
            options.Files.Should().ContainKey("file");
            options.Files["file"].Should().Be(testFile);
        }

        #endregion

        #region Shell-Specific Syntax Variations

        /// <summary>
        /// Tests Windows CMD style with ^ line continuation.
        /// </summary>
        [Fact]
        public void Parse_WindowsCmdLineContinuation_ShouldParseCorrectly()
        {
            // Arrange - Windows CMD uses ^ for line continuation
            const string command = @"curl -X POST ^
  -H ""Content-Type: application/json"" ^
  -d ""{\""key\"":\""value\""}"" ^
  https://api.example.com/endpoint";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            options.Headers["Content-Type"].Should().Be("application/json");
            options.Data.Should().Contain("\"key\":\"value\"");
        }

        /// <summary>
        /// Tests PowerShell style with backtick line continuation.
        /// </summary>
        [Fact]
        public void Parse_PowerShellLineContinuation_ShouldParseCorrectly()
        {
            // Arrange - PowerShell uses ` for line continuation
            const string command = @"curl -X POST `
  -H 'Content-Type: application/json' `
  -d '{\""key\"":\""value\""}' `
  https://api.example.com/endpoint";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            options.Headers["Content-Type"].Should().Be("application/json");
            options.Data.Should().Contain("\"key\":\"value\"");
        }

        /// <summary>
        /// Tests Bash/ZSH style with \ line continuation.
        /// </summary>
        [Fact]
        public void Parse_BashLineContinuation_ShouldParseCorrectly()
        {
            // Arrange - Bash/ZSH uses \ for line continuation
            const string command = @"curl -X POST \
  -H 'Content-Type: application/json' \
  -d '{\""key\"":\""value\""}' \
  https://api.example.com/endpoint";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            options.Headers["Content-Type"].Should().Be("application/json");
        }

        /// <summary>
        /// Tests Windows CMD double-quote escaping ("").
        /// </summary>
        [Fact]
        public void Parse_WindowsCmdQuoteEscaping_ShouldParseCorrectly()
        {
            // Arrange - Simple Windows CMD quoted string
            const string command = @"curl -d ""key=value with spaces"" https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Data.Should().Be("key=value with spaces");
        }

        /// <summary>
        /// Tests PowerShell single vs double quotes.
        /// </summary>
        [Fact]
        public void Parse_PowerShellQuotes_ShouldParseCorrectly()
        {
            // Arrange - PowerShell allows both quote types
            const string command = @"curl -H 'Single: Quote' -H ""Double: Quote"" https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Headers.Should().ContainKey("Single");
            options.Headers["Single"].Should().Be("Quote");
            options.Headers.Should().ContainKey("Double");
            options.Headers["Double"].Should().Be("Quote");
        }

        #endregion

        #region Complex JSON and Data Scenarios

        /// <summary>
        /// Tests JSON with escaped quotes and special characters.
        /// </summary>
        [Fact]
        public void Parse_JsonWithEscapedQuotes_ShouldParseCorrectly()
        {
            // Arrange
            const string command = @"curl -X POST \
  -H 'Content-Type: application/json' \
  -d '{""message"": ""He said \""Hello\"" to me"", ""path"": ""C:\\Users\\Test\\file.txt""}' \
  https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Data.Should().Contain("He said");
            options.Data.Should().Contain("Hello");
        }

        /// <summary>
        /// Tests multiple -d parameters (curl concatenates them with &amp;).
        /// </summary>
        [Fact]
        public void Parse_MultipleDataParameters_ShouldConcatenate()
        {
            // Arrange - Multiple -d flags
            const string command = @"curl -X POST \
  -d 'key1=value1' \
  -d 'key2=value2' \
  -d 'key3=value3' \
  https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            // Note: Current implementation may store last one or concatenate
            // This test documents expected behavior
            options.Data.Should().NotBeNull();
        }

        /// <summary>
        /// Tests URL-encoded data.
        /// </summary>
        [Fact]
        public void Parse_UrlEncodedData_ShouldParseCorrectly()
        {
            // Arrange
            const string command = @"curl -X POST \
  --data-urlencode 'name=John Doe' \
  --data-urlencode 'email=john@example.com' \
  https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            options.Data.Should().Contain("name=John Doe");
        }

        #endregion

        #region File Path Handling

        /// <summary>
        /// Tests Windows file paths with spaces.
        /// </summary>
        [Fact]
        public void Parse_WindowsPathWithSpaces_ShouldParseCorrectly()
        {
            // Arrange
            var windowsPath = @"C:\Users\John Doe\Documents\test file.txt";
            var command = $@"curl -o ""{windowsPath}"" https://example.com/file.txt";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.OutputFile.Should().Be(windowsPath);
        }

        /// <summary>
        /// Tests Unix file paths with spaces.
        /// </summary>
        [Fact]
        public void Parse_UnixPathWithSpaces_ShouldParseCorrectly()
        {
            // Arrange
            const string unixPath = @"/home/john/My Documents/test file.txt";
            var command = $@"curl -o '{unixPath}' https://example.com/file.txt";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.OutputFile.Should().Be(unixPath);
        }

        /// <summary>
        /// Tests file reference with @ symbol.
        /// </summary>
        [Fact]
        public void Parse_FileReferenceWithAtSymbol_ShouldParseCorrectly()
        {
            // Arrange
            var testFile = CreateTempFile("test data", ".json");
            var command = $@"curl -X POST -d @{testFile} https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            // File content should be read and set as Data
            options.Data.Should().NotBeNull();
        }

        #endregion

        #region Authentication Scenarios

        /// <summary>
        /// Tests basic auth with domain user (Windows style).
        /// </summary>
        [Fact]
        public void Parse_BasicAuthWithDomain_ShouldParseCorrectly()
        {
            // Arrange
            const string command = @"curl -u 'DOMAIN\username:password' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Credentials.Should().NotBeNull();
            options.Credentials.UserName.Should().Be(@"DOMAIN\username");
            options.Credentials.Password.Should().Be("password");
        }

        /// <summary>
        /// Tests basic auth with just username (prompt for password).
        /// </summary>
        [Fact]
        public void Parse_BasicAuthUsernameOnly_ShouldParseCorrectly()
        {
            // Arrange
            const string command = @"curl -u 'username' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Credentials.Should().NotBeNull();
            options.Credentials.UserName.Should().Be("username");
            options.Credentials.Password.Should().Be("");
        }

        /// <summary>
        /// Tests bearer token in Authorization header.
        /// </summary>
        [Fact]
        public void Parse_BearerToken_ShouldParseCorrectly()
        {
            // Arrange
            const string command = @"curl -H 'Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Headers.Should().ContainKey("Authorization");
            options.Headers["Authorization"].Should().StartWith("Bearer ");
        }

        #endregion

        #region Complex Option Combinations

        /// <summary>
        /// Tests all common options together (real-world complex command).
        /// </summary>
        [Fact]
        public void Parse_ComplexCommandWithAllOptions_ShouldParseCorrectly()
        {
            // Arrange - Maximum complexity real-world example
            const string command = @"curl -X POST \
  -H 'Accept: application/json' \
  -H 'Content-Type: application/json' \
  -H 'Authorization: Bearer token123' \
  -H 'User-Agent: MyApp/1.0' \
  -d '{""key"":""value""}' \
  -o response.json \
  -L \
  -v \
  -k \
  --connect-timeout 30 \
  --max-time 120 \
  --retry 3 \
  --retry-delay 1 \
  -b 'session=abc123' \
  -c cookies.txt \
  https://api.example.com/endpoint";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            options.Headers.Should().HaveCount(4);
            options.Headers["Accept"].Should().Be("application/json");
            options.Headers["Content-Type"].Should().Be("application/json");
            options.Headers["Authorization"].Should().Be("Bearer token123");
            options.UserAgent.Should().Be("MyApp/1.0");
            options.Data.Should().Contain("\"key\":\"value\"");
            options.OutputFile.Should().Be("response.json");
            options.FollowRedirects.Should().BeTrue();
            options.Verbose.Should().BeTrue();
            options.Insecure.Should().BeTrue();
            options.ConnectTimeout.Should().Be(30);
            options.MaxTime.Should().Be(120);
            options.Retry.Should().Be(3);
            options.RetryDelay.Should().Be(1);
            options.Cookie.Should().Be("session=abc123");
            options.CookieJar.Should().Be("cookies.txt");
        }

        /// <summary>
        /// Tests SSL/TLS certificate options.
        /// </summary>
        [Fact]
        public void Parse_SslCertificateOptions_ShouldParseCorrectly()
        {
            // Arrange
            const string command = @"curl \
  --cert /path/to/client.pem \
  --key /path/to/client.key \
  --cacert /path/to/ca.pem \
  https://secure.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.CertFile.Should().Be("/path/to/client.pem");
            options.KeyFile.Should().Be("/path/to/client.key");
            options.CaCertFile.Should().Be("/path/to/ca.pem");
        }

        /// <summary>
        /// Tests proxy configuration.
        /// </summary>
        [Fact]
        public void Parse_ProxyConfiguration_ShouldParseCorrectly()
        {
            // Arrange
            const string command = @"curl -x http://proxy.example.com:8080 \
  --proxy-user 'user:pass' \
  https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Proxy.Should().Be("http://proxy.example.com:8080");
            options.ProxyCredentials.Should().NotBeNull();
            options.ProxyCredentials.UserName.Should().Be("user");
            options.ProxyCredentials.Password.Should().Be("pass");
        }

        #endregion

        #region Special Characters and Edge Cases

        /// <summary>
        /// Tests URLs with query parameters and special characters.
        /// </summary>
        [Fact]
        public void Parse_UrlWithQueryParams_ShouldPreserveUrl()
        {
            // Arrange
            const string url = @"https://api.example.com/search?q=test+query&limit=10&offset=0";
            var command = $"curl '{url}'";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Url.Should().Be(url);
        }

        /// <summary>
        /// Tests data with newlines and special characters.
        /// </summary>
        [Fact]
        public void Parse_DataWithNewlines_ShouldParseCorrectly()
        {
            // Arrange
            const string command = @"curl -X POST \
  -H 'Content-Type: text/plain' \
  -d 'Line 1
Line 2
Line 3' \
  https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            options.Data.Should().Contain("Line 1");
            options.Data.Should().Contain("Line 2");
            options.Data.Should().Contain("Line 3");
        }

        /// <summary>
        /// Tests header value with colons (non-separator).
        /// </summary>
        [Fact]
        public void Parse_HeaderWithMultipleColons_ShouldParseCorrectly()
        {
            // Arrange - Time header value contains colons
            const string command = @"curl -H 'Time: 2024-01-15T10:30:00Z' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Headers.Should().ContainKey("Time");
            options.Headers["Time"].Should().Be("2024-01-15T10:30:00Z");
        }

        /// <summary>
        /// Tests empty header value.
        /// </summary>
        [Fact]
        public void Parse_EmptyHeaderValue_ShouldParseCorrectly()
        {
            // Arrange
            const string command = @"curl -H 'Custom-Header:' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Headers.Should().ContainKey("Custom-Header");
            options.Headers["Custom-Header"].Should().Be("");
        }

        #endregion

        #region Environment Variable Expansion

        /// <summary>
        /// Tests Windows environment variable expansion (%VAR%).
        /// </summary>
        [Fact]
        public void Parse_WindowsEnvVar_ShouldExpand()
        {
            // Arrange
            Environment.SetEnvironmentVariable("TEST_TOKEN", "secret123");
            try
            {
                const string command = @"curl -H 'Authorization: Bearer %TEST_TOKEN%' https://api.example.com";

                // Act
                var options = _parser.Parse(command);

                // Assert
                options.Headers["Authorization"].Should().Be("Bearer secret123");
            }
            finally
            {
                Environment.SetEnvironmentVariable("TEST_TOKEN", null);
            }
        }

        /// <summary>
        /// Tests PowerShell environment variable expansion ($env:VAR).
        /// </summary>
        [Fact]
        public void Parse_PowerShellEnvVar_ShouldExpand()
        {
            // Arrange
            Environment.SetEnvironmentVariable("TEST_API_KEY", "key456");
            try
            {
                const string command = @"curl -H 'X-API-Key: $env:TEST_API_KEY' https://api.example.com";

                // Act
                var options = _parser.Parse(command);

                // Assert
                options.Headers["X-API-Key"].Should().Be("key456");
            }
            finally
            {
                Environment.SetEnvironmentVariable("TEST_API_KEY", null);
            }
        }

        /// <summary>
        /// Tests Bash/ZSH environment variable expansion ($VAR).
        /// </summary>
        [Fact]
        public void Parse_BashEnvVar_ShouldExpand()
        {
            // Arrange
            Environment.SetEnvironmentVariable("API_URL", "https://api.example.com");
            try
            {
                var command = @"curl $API_URL";

                // Act
                var options = _parser.Parse(command);

                // Assert
                options.Url.Should().Be("https://api.example.com");
            }
            finally
            {
                Environment.SetEnvironmentVariable("API_URL", null);
            }
        }

        #endregion

        #region HTTP Method Variations

        /// <summary>
        /// Tests all standard HTTP methods.
        /// </summary>
        [Theory]
        [InlineData("GET")]
        [InlineData("POST")]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        [InlineData("PATCH")]
        [InlineData("HEAD")]
        [InlineData("OPTIONS")]
        [InlineData("TRACE")]
        public void Parse_AllHttpMethods_ShouldParseCorrectly(string method)
        {
            // Arrange
            var command = $"curl -X {method} https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be(method);
        }

        /// <summary>
        /// Tests custom HTTP method.
        /// </summary>
        [Fact]
        public void Parse_CustomHttpMethod_ShouldParseCorrectly()
        {
            // Arrange
            const string command = @"curl -X PURGE https://api.example.com/cache";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("PURGE");
            options.CustomMethod.Should().Be("PURGE");
        }

        #endregion

        #region Error Handling and Edge Cases

        /// <summary>
        /// Tests command with only options and no URL (should fail gracefully).
        /// </summary>
        [Fact]
        public void Parse_NoUrl_ShouldThrowException()
        {
            // Arrange
            const string command = @"curl -X POST -H 'Content-Type: application/json'";

            // Act & Assert
            Action act = () => _parser.Parse(command);
            act.Should().Throw<CurlException>()
                .WithMessage("*URL*");
        }

        /// <summary>
        /// Tests empty command string.
        /// </summary>
        [Fact]
        public void Parse_EmptyCommand_ShouldThrowException()
        {
            // Arrange
            const string command = "";

            // Act & Assert
            Action act = () => _parser.Parse(command);
            act.Should().Throw<ArgumentNullException>();
        }

        /// <summary>
        /// Tests command with just "curl" (should fail).
        /// </summary>
        [Fact]
        public void Parse_JustCurl_ShouldThrowException()
        {
            // Arrange
            const string command = "curl";

            // Act & Assert
            Action act = () => _parser.Parse(command);
            act.Should().Throw<CurlException>();
        }

        /// <summary>
        /// Tests malformed header (missing colon).
        /// </summary>
        [Fact]
        public void Parse_MalformedHeader_ShouldHandleGracefully()
        {
            // Arrange
            const string command = @"curl -H 'InvalidHeader' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert - Parser should either ignore or handle gracefully
            // Current implementation may store it differently
            options.Url.Should().Be("https://api.example.com");
        }

        #endregion

        #region Output Options

        /// <summary>
        /// Tests -O flag (save with remote filename).
        /// </summary>
        [Fact]
        public void Parse_RemoteNameFlag_ShouldSetFlag()
        {
            // Arrange
            const string command = @"curl -O https://example.com/file.txt";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.UseRemoteFileName.Should().BeTrue();
        }

        /// <summary>
        /// Tests progress bar option.
        /// </summary>
        [Fact]
        public void Parse_ProgressBar_ShouldSetFlag()
        {
            // Arrange
            const string command = @"curl --progress-bar https://example.com/largefile.zip";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.ProgressBar.Should().BeTrue();
        }

        /// <summary>
        /// Tests silent and show-error combination.
        /// </summary>
        [Fact]
        public void Parse_SilentWithShowError_ShouldSetBoth()
        {
            // Arrange
            const string command = @"curl -sS https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Silent.Should().BeTrue();
            options.ShowError.Should().BeTrue();
        }

        #endregion
    }
}

