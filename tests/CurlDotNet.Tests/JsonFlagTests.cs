using System;
using CurlDotNet.Core;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Tests for the --json flag support (curl 7.82+).
    /// The --json flag is shorthand for --data with Content-Type and Accept headers set to application/json.
    /// Resolves GitHub Issue #32: https://github.com/jacob-mellor/curl-dot-net/issues/32
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    [Trait("Category", TestCategories.Unit)]
    public class JsonFlagTests
    {
        private readonly CommandParser _parser;

        public JsonFlagTests()
        {
            _parser = new CommandParser();
        }

        #region Basic --json flag parsing

        [Fact]
        public void Parse_JsonFlag_SetsDataCorrectly()
        {
            // Arrange
            var command = "curl --json '{\"name\":\"test\"}' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Data.Should().Be("{\"name\":\"test\"}");
        }

        [Fact]
        public void Parse_JsonFlag_SetsContentTypeHeader()
        {
            // Arrange
            var command = "curl --json '{\"key\":\"value\"}' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Headers.Should().ContainKey("Content-Type");
            options.Headers["Content-Type"].Should().Be("application/json");
        }

        [Fact]
        public void Parse_JsonFlag_SetsAcceptHeader()
        {
            // Arrange
            var command = "curl --json '{\"key\":\"value\"}' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Headers.Should().ContainKey("Accept");
            options.Headers["Accept"].Should().Be("application/json");
        }

        [Fact]
        public void Parse_JsonFlag_DefaultsToPostMethod()
        {
            // Arrange
            var command = "curl --json '{\"key\":\"value\"}' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
        }

        [Fact]
        public void Parse_JsonFlag_SetsUrlCorrectly()
        {
            // Arrange
            var command = "curl --json '{\"key\":\"value\"}' https://api.example.com/data";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Url.Should().Be("https://api.example.com/data");
        }

        #endregion

        #region --json with explicit method override

        [Fact]
        public void Parse_JsonFlagWithExplicitPut_UsesPutMethod()
        {
            // Arrange
            var command = "curl -X PUT --json '{\"name\":\"updated\"}' https://api.example.com/users/1";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("PUT");
            options.Data.Should().Be("{\"name\":\"updated\"}");
            options.Headers["Content-Type"].Should().Be("application/json");
            options.Headers["Accept"].Should().Be("application/json");
        }

        [Fact]
        public void Parse_JsonFlagWithExplicitPatch_UsesPatchMethod()
        {
            // Arrange
            var command = "curl -X PATCH --json '{\"status\":\"active\"}' https://api.example.com/users/1";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("PATCH");
            options.Data.Should().Be("{\"status\":\"active\"}");
        }

        [Fact]
        public void Parse_JsonFlagMethodAfterJson_OverridesPost()
        {
            // Arrange - method specified AFTER --json should still override
            var command = "curl --json '{\"data\":1}' -X PUT https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("PUT");
        }

        #endregion

        #region Multiple --json flags

        [Fact]
        public void Parse_MultipleJsonFlags_ConcatenatesWithoutSeparator()
        {
            // In curl, multiple --json values are concatenated WITHOUT a separator
            // This differs from --data which uses & as separator
            var command = "curl --json '{\"a\":1}' --json '{\"b\":2}' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert - concatenated without separator (unlike --data which uses &)
            options.Data.Should().Be("{\"a\":1}{\"b\":2}");
        }

        [Fact]
        public void Parse_MultipleJsonFlags_StillSetsHeaders()
        {
            // Arrange
            var command = "curl --json '{\"a\":1}' --json '{\"b\":2}' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Headers["Content-Type"].Should().Be("application/json");
            options.Headers["Accept"].Should().Be("application/json");
        }

        #endregion

        #region --json with file reference (@file)

        [Fact]
        public void Parse_JsonFlagWithFileReference_PreservesFileReference()
        {
            // Arrange - @file syntax should be preserved for later processing
            var command = "curl --json @data.json https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Data.Should().Be("@data.json");
            options.Headers["Content-Type"].Should().Be("application/json");
            options.Headers["Accept"].Should().Be("application/json");
        }

        [Fact]
        public void Parse_JsonFlagWithWindowsFilePath_PreservesPath()
        {
            // Arrange - Windows path with @
            var command = "curl --json @\"c:\\temp\\query.json\" https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Data.Should().Be("@c:\\temp\\query.json");
        }

        #endregion

        #region --json with other flags (real-world scenarios from Issue #32)

        [Fact]
        public void Parse_MassimosExactCommand_ParsesCorrectly()
        {
            // This is the exact command from Issue #32 but with --json instead of requiring manual headers
            var command = @"curl -X POST -H ""Authorization: Bearer xxx"" --json @""c:\t\query.txt"" -L -k -v -f -x http://proxy -o ""c:\t\file.json"" https://graph.microsoft.com/beta/security/runHuntingQuery";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            options.Headers.Should().ContainKey("Authorization");
            options.Headers["Authorization"].Should().Be("Bearer xxx");
            options.Headers["Content-Type"].Should().Be("application/json");
            options.Headers["Accept"].Should().Be("application/json");
            options.Data.Should().Be("@c:\\t\\query.txt");
            options.FollowLocation.Should().BeTrue();
            options.Insecure.Should().BeTrue();
            options.Verbose.Should().BeTrue();
            options.FailOnError.Should().BeTrue();
            options.Proxy.Should().Be("http://proxy");
            options.OutputFile.Should().Be("c:\\t\\file.json");
            options.Url.Should().Be("https://graph.microsoft.com/beta/security/runHuntingQuery");
        }

        [Fact]
        public void Parse_JsonFlagWithAllCommonOptions_ParsesCorrectly()
        {
            // Arrange
            var command = "curl -L -k -v --json '{\"query\":\"search\"}' -o output.json -x http://proxy:8080 https://api.example.com/search";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.FollowLocation.Should().BeTrue();
            options.Insecure.Should().BeTrue();
            options.Verbose.Should().BeTrue();
            options.Data.Should().Be("{\"query\":\"search\"}");
            options.Headers["Content-Type"].Should().Be("application/json");
            options.Headers["Accept"].Should().Be("application/json");
            options.OutputFile.Should().Be("output.json");
            options.Proxy.Should().Be("http://proxy:8080");
            options.Url.Should().Be("https://api.example.com/search");
        }

        [Fact]
        public void Parse_JsonFlagWithAuthentication_ParsesCorrectly()
        {
            // Arrange
            var command = "curl -u admin:password --json '{\"action\":\"reset\"}' https://api.example.com/admin";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Credentials.Should().NotBeNull();
            options.Credentials.UserName.Should().Be("admin");
            options.Credentials.Password.Should().Be("password");
            options.Data.Should().Be("{\"action\":\"reset\"}");
            options.Headers["Content-Type"].Should().Be("application/json");
        }

        #endregion

        #region --json overriding explicit Content-Type header

        [Fact]
        public void Parse_JsonFlagAfterContentTypeHeader_OverridesContentType()
        {
            // --json should set Content-Type regardless of prior -H
            var command = "curl -H 'Content-Type: text/plain' --json '{\"data\":1}' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Headers["Content-Type"].Should().Be("application/json");
        }

        [Fact]
        public void Parse_ContentTypeHeaderAfterJsonFlag_OverridesJson()
        {
            // Later options should override earlier ones (curl behavior)
            var command = "curl --json '{\"data\":1}' -H 'Content-Type: application/xml' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert - later -H overrides --json's Content-Type
            options.Headers["Content-Type"].Should().Be("application/xml");
            // Accept should still be set by --json
            options.Headers["Accept"].Should().Be("application/json");
        }

        #endregion

        #region --json with line continuations

        [Fact]
        public void Parse_JsonFlagWithUnixLineContinuation_ParsesCorrectly()
        {
            // Arrange
            var command = "curl \\\n--json '{\"key\":\"value\"}' \\\nhttps://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Data.Should().Be("{\"key\":\"value\"}");
            options.Headers["Content-Type"].Should().Be("application/json");
        }

        [Fact]
        public void Parse_JsonFlagWithWindowsLineContinuation_ParsesCorrectly()
        {
            // Arrange
            var command = "curl ^\r\n--json \"{\\\"key\\\":\\\"value\\\"}\" ^\r\nhttps://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Data.Should().Be("{\"key\":\"value\"}");
            options.Headers["Content-Type"].Should().Be("application/json");
        }

        #endregion

        #region --json with empty/null data

        [Fact]
        public void Parse_JsonFlagWithNestedObject_ParsesCorrectly()
        {
            // Arrange - complex nested JSON
            var command = "curl --json '{\"user\":{\"name\":\"test\",\"roles\":[\"admin\"]}}' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Data.Should().Be("{\"user\":{\"name\":\"test\",\"roles\":[\"admin\"]}}");
            options.Headers["Content-Type"].Should().Be("application/json");
            options.Headers["Accept"].Should().Be("application/json");
            options.Method.Should().Be("POST");
        }

        #endregion

        #region --json compared to --data behavior

        [Fact]
        public void Parse_DataFlagDoesNotSetAcceptHeader()
        {
            // --data should NOT set Accept header (only --json does)
            var command = "curl -d '{\"key\":\"value\"}' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Headers.Should().NotContainKey("Accept");
            options.Headers.Should().NotContainKey("Content-Type");
        }

        [Fact]
        public void Parse_MultipleDataFlags_ConcatenateWithAmpersand()
        {
            // --data uses & separator, --json does not
            var command = "curl -d 'a=1' -d 'b=2' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Data.Should().Be("a=1&b=2");
        }

        [Fact]
        public void Parse_MultipleJsonFlags_ConcatenateWithoutSeparator()
        {
            // --json concatenates without separator
            var command = "curl --json '{\"a\":1,' --json '\"b\":2}' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert - no separator between parts
            options.Data.Should().Be("{\"a\":1,\"b\":2}");
        }

        #endregion

        #region --json with ConsistencyLevel header (Microsoft Graph API scenario)

        [Fact]
        public void Parse_MicrosoftGraphApiCommand_ParsesCorrectly()
        {
            // Real-world scenario from Issue #32 - Microsoft Graph API hunting query
            var command = @"curl -X POST -H ""ConsistencyLevel: eventual"" -H ""Authorization: Bearer mytoken123"" --json '{""query"":""DeviceProcessEvents | take 10""}' -L -k -v -f https://graph.microsoft.com/beta/security/runHuntingQuery";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            options.Headers["ConsistencyLevel"].Should().Be("eventual");
            options.Headers["Authorization"].Should().Be("Bearer mytoken123");
            options.Headers["Content-Type"].Should().Be("application/json");
            options.Headers["Accept"].Should().Be("application/json");
            options.Data.Should().Be("{\"query\":\"DeviceProcessEvents | take 10\"}");
            options.FollowLocation.Should().BeTrue();
            options.Insecure.Should().BeTrue();
            options.Verbose.Should().BeTrue();
            options.FailOnError.Should().BeTrue();
        }

        #endregion

        #region --json with equals sign syntax

        [Fact]
        public void Parse_JsonFlagWithEqualsSignSyntax_ParsesCorrectly()
        {
            // curl supports --json=<data> syntax
            var command = "curl --json='{\"key\":\"value\"}' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Data.Should().Be("{\"key\":\"value\"}");
            options.Headers["Content-Type"].Should().Be("application/json");
            options.Headers["Accept"].Should().Be("application/json");
        }

        #endregion

        #region --json with double quotes

        [Fact]
        public void Parse_JsonFlagWithDoubleQuotes_ParsesCorrectly()
        {
            // Arrange
            var command = "curl --json \"{\\\"name\\\":\\\"test\\\"}\" https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Data.Should().Be("{\"name\":\"test\"}");
        }

        #endregion
    }
}
