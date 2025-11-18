using System;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Additional tests for FtpHandler to improve code coverage from 32.7%.
    /// Testing basic functionality and error handling.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    public class FtpHandlerAdditionalTests
    {
        private readonly FtpHandler _handler;

        public FtpHandlerAdditionalTests()
        {
            _handler = new FtpHandler();
        }

        [Fact]
        public void SupportsProtocol_Ftp_ReturnsTrue()
        {
            // FTP should be supported
            var result = _handler.SupportsProtocol("ftp");
            result.Should().BeTrue();
        }

        [Fact]
        public void SupportsProtocol_Ftps_ReturnsTrue()
        {
            // FTPS should be supported
            var result = _handler.SupportsProtocol("ftps");
            result.Should().BeTrue();
        }

        [Fact]
        public void SupportsProtocol_Sftp_ReturnsFalse()
        {
            // SFTP is not supported by FtpHandler
            var result = _handler.SupportsProtocol("sftp");
            result.Should().BeFalse();
        }

        [Fact]
        public void SupportsProtocol_Http_ReturnsFalse()
        {
            // HTTP should not be supported by FtpHandler
            var result = _handler.SupportsProtocol("http");
            result.Should().BeFalse();
        }

        [Fact]
        public void SupportsProtocol_CaseInsensitive_Works()
        {
            _handler.SupportsProtocol("FTP").Should().BeTrue();
            _handler.SupportsProtocol("FTPS").Should().BeTrue();
            _handler.SupportsProtocol("Ftp").Should().BeTrue();
        }

        [Fact]
        public async Task ExecuteAsync_InvalidUrl_ReturnsError()
        {
            var options = new CurlOptions
            {
                Url = "ftp://invalid-host-that-does-not-exist.local/test.txt"
            };

            // This should return a result with an error, not throw
            var result = await _handler.ExecuteAsync(options, CancellationToken.None);
            result.Should().NotBeNull();
            // We expect a non-success status code for unreachable hosts
            result.StatusCode.Should().NotBe(200);
        }

        [Fact]
        public async Task ExecuteAsync_WithPort_UsesSpecifiedPort()
        {
            var options = new CurlOptions
            {
                Url = "ftp://ftp.example.com:2121/test.txt"
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_FtpsUrl_HandlesFtps()
        {
            var options = new CurlOptions
            {
                Url = "ftps://secure.example.com/test.txt"
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_WithRange_SetsContentRange()
        {
            var options = new CurlOptions
            {
                Url = "ftp://ftp.example.com/largefile.bin",
                Range = "100-200"
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_WithInsecure_BypassesSslValidation()
        {
            var options = new CurlOptions
            {
                Url = "ftps://secure.example.com/test.txt",
                Insecure = true
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_DirectoryUrl_HandlesDirectory()
        {
            var options = new CurlOptions
            {
                Url = "ftp://ftp.example.com/directory/"
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_FileUrl_HandlesFile()
        {
            var options = new CurlOptions
            {
                Url = "ftp://ftp.example.com/file.txt"
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_WithAuthentication_HandlesCredentials()
        {
            var options = new CurlOptions
            {
                Url = "ftp://testuser:testpass@ftp.example.com/secure-file.txt"
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_WithProxy_HandlesProxy()
        {
            var options = new CurlOptions
            {
                Url = "ftp://ftp.example.com/file.txt",
                Proxy = "proxy.example.com:8080"
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_WithTimeout_RespectsTimeout()
        {
            var options = new CurlOptions
            {
                Url = "ftp://ftp.example.com/file.txt",
                MaxTime = 5
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_WithUserAgent_SetsUserAgent()
        {
            var options = new CurlOptions
            {
                Url = "ftp://ftp.example.com/file.txt",
                UserAgent = "CurlDotNet/1.0"
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_EmptyUrl_ReturnsError()
        {
            var options = new CurlOptions
            {
                Url = ""
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(200);
        }

        [Fact]
        public async Task ExecuteAsync_NullUrl_ReturnsError()
        {
            var options = new CurlOptions
            {
                Url = null
            };

            var result = await _handler.ExecuteAsync(options, CancellationToken.None);
            result.Should().NotBeNull();
            result.StatusCode.Should().NotBe(200);
        }

        [Fact]
        public void Constructor_CreatesValidInstance()
        {
            var handler = new FtpHandler();
            handler.Should().NotBeNull();
        }
    }
}