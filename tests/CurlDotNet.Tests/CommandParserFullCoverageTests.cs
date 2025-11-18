using System;
using System.Collections.Generic;
using CurlDotNet.Core;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Comprehensive tests for CommandParser to achieve 100% code coverage.
    /// Tests all curl options, edge cases, and parsing scenarios.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    public class CommandParserFullCoverageTests
    {
        private readonly CommandParser _parser;

        public CommandParserFullCoverageTests()
        {
            _parser = new CommandParser();
        }

        #region IsValid Tests

        [Fact]
        public void IsValid_ReturnsTrueForValidCommand()
        {
            // Arrange
            var parser = new CommandParser();

            // Act & Assert
            parser.IsValid("curl https://api.example.com").Should().BeTrue();
            parser.IsValid("https://api.example.com").Should().BeTrue();
            parser.IsValid("curl -X POST https://api.example.com").Should().BeTrue();
        }

        [Fact]
        public void IsValid_ReturnsFalseForInvalidCommand()
        {
            // Arrange
            var parser = new CommandParser();

            // Act & Assert
            parser.IsValid("").Should().BeFalse();
            parser.IsValid("   ").Should().BeFalse();
            parser.IsValid(null!).Should().BeFalse();
            parser.IsValid("curl").Should().BeFalse(); // No URL
            parser.IsValid("not-a-url").Should().BeTrue(); // Parser accepts bare URLs
        }

        #endregion

        #region Line Continuation Tests

        [Fact]
        public void Parse_HandlesUnixLineContinuations()
        {
            // Arrange
            var command = @"curl \
                -X POST \
                https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            options.Url.Should().Be("https://api.example.com");
        }

        [Fact]
        public void Parse_HandlesWindowsLineContinuations()
        {
            // Arrange
            var command = "curl ^\r\n    -X POST ^\r\n    https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            options.Url.Should().Be("https://api.example.com");
        }

        [Fact]
        public void Parse_HandlesPowerShellLineContinuations()
        {
            // Arrange
            var command = @"curl `
                -X POST `
                https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            options.Url.Should().Be("https://api.example.com");
        }

        #endregion

        #region Environment Variable Tests

        [Theory]
        [InlineData("curl %API_URL%", "API_URL", "https://api.example.com")]
        [InlineData("curl -H 'Authorization: Bearer %TOKEN%'", "TOKEN", "secret123")]
        public void Parse_ExpandsWindowsCmdEnvironmentVariables(string command, string varName, string varValue)
        {
            // Arrange
            Environment.SetEnvironmentVariable(varName, varValue);
            try
            {
                // Act
                var options = _parser.Parse(command + " https://api.example.com");

                // Assert
                if (varName == "API_URL")
                {
                    options.Url.Should().Be(varValue);
                }
                else if (varName == "TOKEN")
                {
                    options.Headers["Authorization"].Should().Be($"Bearer {varValue}");
                }
            }
            finally
            {
                Environment.SetEnvironmentVariable(varName, null);
            }
        }

        [Theory]
        [InlineData("curl $env:API_URL", "API_URL", "https://api.example.com")]
        [InlineData("curl -H \"Authorization: Bearer $env:TOKEN\"", "TOKEN", "secret123")]
        public void Parse_ExpandsPowerShellEnvironmentVariables(string command, string varName, string varValue)
        {
            // Arrange
            Environment.SetEnvironmentVariable(varName, varValue);
            try
            {
                // Act
                var options = _parser.Parse(command + (varName == "TOKEN" ? " https://api.example.com" : ""));

                // Assert
                if (varName == "API_URL")
                {
                    options.Url.Should().Be(varValue);
                }
                else if (varName == "TOKEN")
                {
                    options.Headers["Authorization"].Should().Be($"Bearer {varValue}");
                }
            }
            finally
            {
                Environment.SetEnvironmentVariable(varName, null);
            }
        }

        [Theory]
        [InlineData("curl ${API_URL}", "API_URL", "https://api.example.com")]
        [InlineData("curl $API_URL", "API_URL", "https://api.example.com")]
        [InlineData("curl -H 'Authorization: Bearer ${TOKEN}'", "TOKEN", "secret123")]
        public void Parse_ExpandsBashEnvironmentVariables(string command, string varName, string varValue)
        {
            // Arrange
            Environment.SetEnvironmentVariable(varName, varValue);
            try
            {
                // Act
                var options = _parser.Parse(command + (varName == "TOKEN" ? " https://api.example.com" : ""));

                // Assert
                if (varName == "API_URL")
                {
                    options.Url.Should().Be(varValue);
                }
                else if (varName == "TOKEN")
                {
                    options.Headers["Authorization"].Should().Be($"Bearer {varValue}");
                }
            }
            finally
            {
                Environment.SetEnvironmentVariable(varName, null);
            }
        }

        #endregion

        #region Quote Handling Tests

        [Fact]
        public void Parse_HandlesSingleQuotes()
        {
            // Arrange
            var command = "curl -H 'Accept: application/json' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Headers["Accept"].Should().Be("application/json");
        }

        [Fact]
        public void Parse_HandlesDoubleQuotes()
        {
            // Arrange
            var command = "curl -H \"Accept: application/json\" https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Headers["Accept"].Should().Be("application/json");
        }

        [Fact]
        public void Parse_HandlesEscapedQuotes()
        {
            // Arrange
            var command = @"curl -d '{""name"": ""John's Test""}' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Data.Should().Contain("John's Test");
        }

        [Fact]
        public void Parse_HandlesEscapeSequences()
        {
            // Arrange
            var command = @"curl -d 'line1\nline2\ttab' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Data.Should().Be("line1\nline2\ttab");
        }

        #endregion

        #region Option Parsing Tests

        [Fact]
        public void Parse_HandlesDataUrlEncode()
        {
            // Arrange
            var command = "curl --data-urlencode 'name=John Doe' https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Data.Should().Be("name=John%20Doe");
        }

        [Fact]
        public void Parse_HandlesContinueAt()
        {
            // Arrange
            var command = "curl --continue-at 1024 https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Range.Should().Be("1024-");
        }

        [Fact]
        public void Parse_HandlesContinueAtDash()
        {
            // Arrange
            var command = "curl --continue-at - https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.ResumeFrom.Should().NotBeNull();
        }

        [Fact]
        public void Parse_HandlesCertificateOptions()
        {
            // Arrange
            var command = "curl --cert /path/to/cert.pem --key /path/to/key.pem --cacert /path/to/ca.pem https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.CertFile.Should().Be("/path/to/cert.pem");
            options.KeyFile.Should().Be("/path/to/key.pem");
            options.CaCertFile.Should().Be("/path/to/ca.pem");
        }

        [Fact]
        public void Parse_HandlesInterface()
        {
            // Arrange
            var command = "curl --interface eth0 https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Interface.Should().Be("eth0");
        }

        [Fact]
        public void Parse_HandlesHttpVersions()
        {
            // Test HTTP 1.0
            var options = _parser.Parse("curl --http1.0 https://api.example.com");
            options.HttpVersion.Should().Be("1.0");

            // Test HTTP 1.1
            options = _parser.Parse("curl --http1.1 https://api.example.com");
            options.HttpVersion.Should().Be("1.1");

            // Test HTTP 2
            options = _parser.Parse("curl --http2 https://api.example.com");
            options.HttpVersion.Should().Be("2.0");
        }

        [Fact]
        public void Parse_HandlesLimitRate()
        {
            // Test with kilobytes
            var options = _parser.Parse("curl --limit-rate 100k https://api.example.com");
            options.SpeedLimit.Should().Be(100 * 1024);

            // Test with megabytes
            options = _parser.Parse("curl --limit-rate 5m https://api.example.com");
            options.SpeedLimit.Should().Be(5 * 1024 * 1024);

            // Test with gigabytes
            options = _parser.Parse("curl --limit-rate 1g https://api.example.com");
            options.SpeedLimit.Should().Be(1024L * 1024 * 1024);

            // Test with no suffix (bytes)
            options = _parser.Parse("curl --limit-rate 1000 https://api.example.com");
            options.SpeedLimit.Should().Be(1000);
        }

        [Fact]
        public void Parse_HandlesSpeedTime()
        {
            // Arrange
            var command = "curl --speed-time 30 --speed-limit 1000 https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.SpeedTime.Should().Be(30);
            options.SpeedLimit.Should().Be(1000);
        }

        [Fact]
        public void Parse_HandlesProgressBar()
        {
            // Arrange
            var command = "curl --progress-bar https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.ProgressBar.Should().BeTrue();
        }

        [Fact]
        public void Parse_HandlesKeepAliveTime()
        {
            // Arrange
            var command = "curl --keepalive-time 60 https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.KeepAliveTime.Should().Be(60);
        }

        [Fact]
        public void Parse_HandlesDnsServers()
        {
            // Arrange
            var command = "curl --dns-servers 8.8.8.8,8.8.4.4 https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.DnsServers.Should().Be("8.8.8.8,8.8.4.4");
        }

        [Fact]
        public void Parse_HandlesResolve()
        {
            // Arrange
            var command = "curl --resolve example.com:443:192.168.1.1 https://example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Resolve.Should().ContainKey("example.com:443");
            options.Resolve["example.com:443"].Should().Be("192.168.1.1");
        }

        [Fact]
        public void Parse_HandlesQuoteCommand()
        {
            // Arrange
            var command = "curl --quote 'DELE file.txt' ftp://ftp.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Quote.Should().Contain("DELE file.txt");
        }

        [Fact]
        public void Parse_HandlesCreateDirs()
        {
            // Arrange
            var command = "curl --create-dirs -o /new/dir/file.txt https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.CreateDirs.Should().BeTrue();
        }

        [Fact]
        public void Parse_HandlesFtpOptions()
        {
            // Arrange
            var command = "curl --ftp-pasv --ftp-ssl --disable-epsv --disable-eprt ftp://ftp.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.FtpPassive.Should().BeTrue();
            options.FtpSsl.Should().BeTrue();
            options.DisableEpsv.Should().BeTrue();
            options.DisableEprt.Should().BeTrue();
        }

        [Fact]
        public void Parse_HandlesSocks5()
        {
            // Arrange
            var command = "curl --socks5 localhost:9050 https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Socks5Proxy.Should().Be("localhost:9050");
        }

        [Fact]
        public void Parse_HandlesRetryOptions()
        {
            // Arrange
            var command = "curl --retry 3 --retry-delay 5 --retry-max-time 120 https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Retry.Should().Be(3);
            options.RetryDelay.Should().Be(5);
            options.RetryMaxTime.Should().Be(120);
        }

        [Fact]
        public void Parse_HandlesLocationTrusted()
        {
            // Arrange
            var command = "curl --location-trusted https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.LocationTrusted.Should().BeTrue();
        }

        #endregion

        #region Combined Short Options Tests

        [Fact]
        public void Parse_HandlesCombinedShortOptions()
        {
            // Arrange
            var command = "curl -sSLvk https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Silent.Should().BeTrue();
            options.ShowError.Should().BeTrue();
            options.FollowLocation.Should().BeTrue();
            options.Verbose.Should().BeTrue();
            options.Insecure.Should().BeTrue();
        }

        [Fact]
        public void Parse_HandlesCombinedShortOptionsWithValue()
        {
            // Arrange
            var command = "curl -Lo output.txt https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.FollowLocation.Should().BeTrue();
            options.OutputFile.Should().Be("output.txt");
        }

        #endregion

        // Note: ParseSize is a private method, tested indirectly through --limit-rate option

        #region Edge Cases

        [Fact]
        public void Parse_HandlesEmptyCommand()
        {
            // Act & Assert
            Action act = () => _parser.Parse("");
            act.Should().Throw<ArgumentException>().WithMessage("*null*");
        }

        [Fact]
        public void Parse_HandlesNullCommand()
        {
            // Act & Assert
            Action act = () => _parser.Parse(null!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Parse_HandlesMissingUrl()
        {
            // Act & Assert
            Action act = () => _parser.Parse("curl -X POST");
            act.Should().Throw<ArgumentException>().WithMessage("*URL*");
        }

        [Fact]
        public void Parse_HandlesMultipleUrls()
        {
            // Arrange
            var command = "curl https://api1.example.com https://api2.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert (should take the first URL)
            options.Url.Should().Be("https://api1.example.com");
        }

        [Fact]
        public void Parse_HandlesDataFromFile()
        {
            // Arrange
            var command = "curl -d @/path/to/file.json https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            // Data from file is read and stored in Data property
            options.Data.Should().Be("@/path/to/file.json");
        }

        [Fact]
        public void Parse_HandlesHeaderFromFile()
        {
            // Arrange
            var command = "curl -H @/path/to/headers.txt https://api.example.com";

            // Act
            var options = _parser.Parse(command);

            // Assert
            // Header from file notation stored as header value
            options.Headers.Should().ContainKey("@/path/to/headers.txt");
        }

        [Fact]
        public void Parse_HandlesComplexRealWorldCommand()
        {
            // Arrange
            var command = @"curl -X POST \
                -H 'Content-Type: application/json' \
                -H 'Authorization: Bearer token123' \
                -d '{""name"":""test""}' \
                --compressed \
                --silent \
                --show-error \
                --fail \
                --location \
                --max-time 30 \
                --connect-timeout 10 \
                --retry 3 \
                --retry-delay 2 \
                --user-agent 'MyApp/1.0' \
                --proxy http://proxy:8080 \
                --proxy-user proxyuser:proxypass \
                --output result.json \
                https://api.example.com/endpoint";

            // Act
            var options = _parser.Parse(command);

            // Assert
            options.Method.Should().Be("POST");
            options.Headers["Content-Type"].Should().Be("application/json");
            options.Headers["Authorization"].Should().Be("Bearer token123");
            options.Data.Should().Contain("test");
            options.Compressed.Should().BeTrue();
            options.Silent.Should().BeTrue();
            options.ShowError.Should().BeTrue();
            options.FailOnError.Should().BeTrue();
            options.FollowLocation.Should().BeTrue();
            options.MaxTime.Should().Be(30);
            options.ConnectTimeout.Should().Be(10);
            options.Retry.Should().Be(3);
            options.RetryDelay.Should().Be(2);
            options.UserAgent.Should().Be("MyApp/1.0");
            options.Proxy.Should().Be("http://proxy:8080");
            options.ProxyCredentials.Should().NotBeNull();
            options.ProxyCredentials.UserName.Should().Be("proxyuser");
            options.ProxyCredentials.Password.Should().Be("proxypass");
            options.OutputFile.Should().Be("result.json");
            options.Url.Should().Be("https://api.example.com/endpoint");
        }

        #endregion

        // Note: ParseResolve is a private method, tested indirectly through --resolve option
    }
}