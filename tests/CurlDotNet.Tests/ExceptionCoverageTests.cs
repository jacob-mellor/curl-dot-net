using System;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Comprehensive tests for all exception types to boost coverage.
    /// Tests basic construction and message content.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    [Trait("Category", TestCategories.FullCoverage)]
    public class ExceptionCoverageTests
    {
        [Fact]
        public void CurlAuthenticationException_Constructor_Works()
        {
            var ex = new CurlAuthenticationException("auth failed");
            ex.Message.Should().Contain("auth failed");
        }

        [Fact]
        public void CurlBadContentEncodingException_Constructor_Works()
        {
            var ex = new CurlBadContentEncodingException("bad encoding");
            ex.Message.Should().Contain("bad encoding");
        }

        [Fact]
        public void CurlBadDownloadResumeException_Constructor_Works()
        {
            var ex = new CurlBadDownloadResumeException(1024);
            ex.Should().NotBeNull();
        }

        [Fact]
        public void CurlBadFunctionArgumentException_Constructor_Works()
        {
            var ex = new CurlBadFunctionArgumentException("bad argument");
            ex.Message.Should().Contain("bad argument");
        }

        [Fact]
        public void CurlConnectionException_Constructor_Works()
        {
            var ex = new CurlConnectionException("connection failed", "example.com", 443);
            ex.Message.Should().Contain("connection failed");
            ex.Host.Should().Be("example.com");
            ex.Port.Should().Be(443);
        }

        [Fact]
        public void CurlCookieException_Constructor_Works()
        {
            var ex = new CurlCookieException("cookie error");
            ex.Message.Should().Contain("cookie error");
        }

        [Fact]
        public void CurlDnsException_Constructor_Works()
        {
            var ex = new CurlDnsException("dns failed", "example.com");
            ex.Message.Should().Contain("dns failed");
            ex.Host.Should().Be("example.com");
        }

        [Fact]
        public void CurlExecutionException_Constructor_Works()
        {
            var ex = new CurlExecutionException("execution failed");
            ex.Message.Should().Contain("execution failed");
        }

        [Fact]
        public void CurlFailedInitException_Constructor_Works()
        {
            var ex = new CurlFailedInitException("init failed");
            ex.Message.Should().Contain("init failed");
        }

        [Fact]
        public void CurlFileCouldntReadException_Constructor_Works()
        {
            var ex = new CurlFileCouldntReadException("file.txt");
            ex.Message.Should().Contain("file.txt");
        }

        [Fact]
        public void CurlFileException_Constructor_Works()
        {
            var ex = new CurlFileException("file error", "test.txt", CurlFileException.FileOperation.Read);
            ex.Message.Should().Contain("file error");
            ex.FilePath.Should().Be("test.txt");
        }

        [Fact]
        public void CurlFileSizeExceededException_Constructor_Works()
        {
            var ex = new CurlFileSizeExceededException(1000, 500);
            ex.Message.Should().Contain("1000");
            ex.Message.Should().Contain("500");
            ex.ActualSize.Should().Be(1000);
            ex.MaxSize.Should().Be(500);
        }

        [Fact]
        public void CurlFtpAcceptFailedException_Constructor_Works()
        {
            var ex = new CurlFtpAcceptFailedException("accept failed");
            ex.Message.Should().Contain("accept failed");
        }

        [Fact]
        public void CurlFtpAcceptTimeoutException_Constructor_Works()
        {
            var ex = new CurlFtpAcceptTimeoutException("timeout");
            ex.Message.Should().Contain("timeout");
        }

        [Fact]
        public void CurlFtpException_Constructor_Works()
        {
            var ex = new CurlFtpException("ftp error", 500);
            ex.Message.Should().Contain("ftp error");
        }

        [Fact]
        public void CurlFtpWeirdPassReplyException_Constructor_Works()
        {
            var ex = new CurlFtpWeirdPassReplyException("weird reply");
            ex.Message.Should().Contain("weird reply");
        }

        [Fact]
        public void CurlFunctionNotFoundException_Constructor_Works()
        {
            var ex = new CurlFunctionNotFoundException("function");
            ex.Message.Should().Contain("function");
        }

        [Fact]
        public void CurlGotNothingException_Constructor_Works()
        {
            var ex = new CurlGotNothingException("https://example.com");
            ex.Message.Should().Contain("example.com");
        }

        [Fact]
        public void CurlHttpException_Constructor_Works()
        {
            var ex = new CurlHttpException("http error", 404);
            ex.Message.Should().Contain("http error");
            ex.StatusCode.Should().Be(404);
        }

        [Fact]
        public void CurlHttpPostErrorException_Constructor_Works()
        {
            var ex = new CurlHttpPostErrorException("post failed");
            ex.Message.Should().Contain("post failed");
        }

        [Fact]
        public void CurlHttpReturnedErrorException_Constructor_Works()
        {
            var ex = new CurlHttpReturnedErrorException(404, "Not Found", "https://example.com/missing", "GET");
            ex.Message.Should().Contain("404");
            ex.StatusCode.Should().Be(404);
        }

        [Fact]
        public void CurlInterfaceFailedException_Constructor_Works()
        {
            var ex = new CurlInterfaceFailedException("interface");
            ex.Message.Should().Contain("interface");
        }

        [Fact]
        public void CurlLoginDeniedException_Constructor_Works()
        {
            var ex = new CurlLoginDeniedException("auth failed");
            ex.Message.Should().Contain("auth failed");
        }

        [Fact]
        public void CurlNotBuiltInException_Constructor_Works()
        {
            var ex = new CurlNotBuiltInException("feature");
            ex.Message.Should().Contain("feature");
            ex.Feature.Should().Be("feature");
        }

        [Fact]
        public void CurlNotSupportedException_Constructor_Works()
        {
            var ex = new CurlNotSupportedException("not supported");
            ex.Message.Should().Contain("not supported");
        }

        [Fact]
        public void CurlOptionSyntaxException_Constructor_Works()
        {
            var ex = new CurlOptionSyntaxException("--bad-option", "details");
            ex.Message.Should().Contain("--bad-option");
        }

        [Fact]
        public void CurlOutOfMemoryException_Constructor_Works()
        {
            var ex = new CurlOutOfMemoryException("out of memory");
            ex.Message.Should().Contain("out of memory");
        }

        [Fact]
        public void CurlParsingException_Constructor_Works()
        {
            var ex = new CurlParsingException("parsing failed", "application/json", typeof(object), "{invalid}", null);
            ex.Message.Should().Contain("parsing failed");
        }

        [Fact]
        public void CurlPeerFailedVerificationException_Constructor_Works()
        {
            var ex = new CurlPeerFailedVerificationException("example.com", "cert error");
            ex.Message.Should().Contain("example.com");
        }

        [Fact]
        public void CurlProxyException_Constructor_Works()
        {
            var ex = new CurlProxyException("proxy error", "proxy.example.com", 8080);
            ex.Message.Should().Contain("proxy error");
            ex.ProxyHost.Should().Be("proxy.example.com");
            ex.ProxyPort.Should().Be(8080);
        }

        [Fact]
        public void CurlRateLimitException_Constructor_Works()
        {
            var ex = new CurlRateLimitException("rate limited");
            ex.Message.Should().Contain("rate limited");
        }

        [Fact]
        public void CurlReadErrorException_Constructor_Works()
        {
            var ex = new CurlReadErrorException("file.txt", "read failed");
            ex.Message.Should().Contain("file.txt");
            ex.Message.Should().Contain("read failed");
            ex.FilePath.Should().Be("file.txt");
        }

        [Fact]
        public void CurlReceiveErrorException_Constructor_Works()
        {
            var ex = new CurlReceiveErrorException("receive failed");
            ex.Message.Should().Contain("receive failed");
        }

        [Fact]
        public void CurlRedirectException_Constructor_Works()
        {
            var ex = new CurlRedirectException("redirect error", 5, "https://example.com");
            ex.Message.Should().Contain("redirect error");
        }

        [Fact]
        public void CurlRemoteAccessDeniedException_Constructor_Works()
        {
            var ex = new CurlRemoteAccessDeniedException("https://example.com/resource");
            ex.Message.Should().Contain("example.com");
        }

        [Fact]
        public void CurlSendErrorException_Constructor_Works()
        {
            var ex = new CurlSendErrorException("send failed");
            ex.Message.Should().Contain("send failed");
        }

        [Fact]
        public void CurlSslCertificateProblemException_Constructor_Works()
        {
            var ex = new CurlSslCertificateProblemException("example.com", "cert error");
            ex.Message.Should().Contain("example.com");
        }

        [Fact]
        public void CurlSslCipherException_Constructor_Works()
        {
            var ex = new CurlSslCipherException("TLS_AES_256", "example.com");
            ex.Message.Should().Contain("TLS_AES_256");
        }

        [Fact]
        public void CurlSslConnectErrorException_Constructor_Works()
        {
            var ex = new CurlSslConnectErrorException("example.com", "ssl error");
            ex.Message.Should().Contain("example.com");
        }

        [Fact]
        public void CurlSslEngineNotFoundException_Constructor_Works()
        {
            var ex = new CurlSslEngineNotFoundException("openssl", "example.com");
            ex.Message.Should().Contain("openssl");
        }

        [Fact]
        public void CurlSslEngineSetFailedException_Constructor_Works()
        {
            var ex = new CurlSslEngineSetFailedException("openssl", "example.com");
            ex.Message.Should().Contain("openssl");
        }

        [Fact]
        public void CurlSslException_Constructor_Works()
        {
            var ex = new CurlSslException("ssl error", "example.com");
            ex.Message.Should().Contain("ssl error");
        }

        [Fact]
        public void CurlUnknownOptionException_Constructor_Works()
        {
            var ex = new CurlUnknownOptionException("--unknown", "details");
            ex.Message.Should().Contain("--unknown");
        }

        [Fact]
        public void CurlUploadFailedException_Constructor_Works()
        {
            var ex = new CurlUploadFailedException("upload failed", "file.txt", "https://example.com");
            ex.Message.Should().Contain("upload failed");
        }

        [Fact]
        public void CurlUseSslFailedException_Constructor_Works()
        {
            var ex = new CurlUseSslFailedException("example.com", "ssl error");
            ex.Message.Should().Contain("example.com");
        }

        [Fact]
        public void CurlWeirdServerReplyException_Constructor_Works()
        {
            var ex = new CurlWeirdServerReplyException("weird reply");
            ex.Message.Should().Contain("weird reply");
        }

        [Fact]
        public void CurlWriteErrorException_Constructor_Works()
        {
            var ex = new CurlWriteErrorException("write failed", "file.txt", "details");
            ex.Message.Should().Contain("write failed");
        }
    }
}
