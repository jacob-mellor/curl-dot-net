using System;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Comprehensive tests for all exception types to achieve 100% coverage.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    [Trait("Category", TestCategories.FullCoverage)]
    public class ExceptionCoverageTests
    {
        [Fact]
        public void CurlAuthenticationException_Constructor_SetsProperties()
        {
            var ex = new CurlAuthenticationException("auth failed");
            ex.Message.Should().Contain("auth failed");
            ex.CurlCode.Should().Be(67); // CURLE_LOGIN_DENIED
        }

        [Fact]
        public void CurlBadContentEncodingException_Constructor_SetsProperties()
        {
            var ex = new CurlBadContentEncodingException("bad encoding");
            ex.Message.Should().Contain("bad encoding");
            ex.CurlCode.Should().Be(61); // CURLE_BAD_CONTENT_ENCODING
        }

        [Fact]
        public void CurlBadDownloadResumeException_Constructor_SetsProperties()
        {
            var ex = new CurlBadDownloadResumeException("resume failed");
            ex.Message.Should().Contain("resume failed");
            ex.CurlCode.Should().Be(36); // CURLE_BAD_DOWNLOAD_RESUME
        }

        [Fact]
        public void CurlBadFunctionArgumentException_Constructor_SetsProperties()
        {
            var ex = new CurlBadFunctionArgumentException("bad argument");
            ex.Message.Should().Contain("bad argument");
            ex.CurlCode.Should().Be(43); // CURLE_BAD_FUNCTION_ARGUMENT
        }

        [Fact]
        public void CurlConnectionException_Constructor_SetsProperties()
        {
            var ex = new CurlConnectionException("connection failed", "example.com", 443);
            ex.Message.Should().Contain("connection failed");
            ex.Host.Should().Be("example.com");
            ex.Port.Should().Be(443);
        }

        [Fact]
        public void CurlCookieException_Constructor_SetsProperties()
        {
            var ex = new CurlCookieException("cookie error");
            ex.Message.Should().Contain("cookie error");
        }

        [Fact]
        public void CurlDnsException_Constructor_SetsProperties()
        {
            var ex = new CurlDnsException("dns failed", "example.com");
            ex.Message.Should().Contain("dns failed");
            ex.Host.Should().Be("example.com");
        }

        [Fact]
        public void CurlExecutionException_Constructor_SetsProperties()
        {
            var ex = new CurlExecutionException("execution failed", 1);
            ex.Message.Should().Contain("execution failed");
            ex.CurlCode.Should().Be(1);
        }

        [Fact]
        public void CurlFailedInitException_Constructor_SetsProperties()
        {
            var ex = new CurlFailedInitException("init failed");
            ex.Message.Should().Contain("init failed");
            ex.CurlCode.Should().Be(2); // CURLE_FAILED_INIT
        }

        [Fact]
        public void CurlFileCouldntReadException_Constructor_SetsProperties()
        {
            var ex = new CurlFileCouldntReadException("file.txt");
            ex.Message.Should().Contain("file.txt");
            ex.CurlCode.Should().Be(37); // CURLE_FILE_COULDNT_READ_FILE
        }

        [Fact]
        public void CurlFileException_Constructor_SetsProperties()
        {
            var ex = new CurlFileException("file error", "test.txt");
            ex.Message.Should().Contain("file error");
            ex.FilePath.Should().Be("test.txt");
        }

        [Fact]
        public void CurlFileSizeExceededException_Constructor_SetsProperties()
        {
            var ex = new CurlFileSizeExceededException(1000, 500);
            ex.Message.Should().Contain("1000");
            ex.Message.Should().Contain("500");
            ex.ActualSize.Should().Be(1000);
            ex.MaxSize.Should().Be(500);
        }

        [Fact]
        public void CurlFtpAcceptFailedException_Constructor_SetsProperties()
        {
            var ex = new CurlFtpAcceptFailedException("accept failed");
            ex.Message.Should().Contain("accept failed");
        }

        [Fact]
        public void CurlFtpAcceptTimeoutException_Constructor_SetsProperties()
        {
            var ex = new CurlFtpAcceptTimeoutException("timeout");
            ex.Message.Should().Contain("timeout");
        }

        [Fact]
        public void CurlFtpException_Constructor_SetsProperties()
        {
            var ex = new CurlFtpException("ftp error", 500);
            ex.Message.Should().Contain("ftp error");
            ex.FtpStatusCode.Should().Be(500);
        }

        [Fact]
        public void CurlFtpWeirdPassReplyException_Constructor_SetsProperties()
        {
            var ex = new CurlFtpWeirdPassReplyException("weird reply");
            ex.Message.Should().Contain("weird reply");
        }

        [Fact]
        public void CurlFunctionNotFoundException_Constructor_SetsProperties()
        {
            var ex = new CurlFunctionNotFoundException("function");
            ex.Message.Should().Contain("function");
            ex.CurlCode.Should().Be(41); // CURLE_FUNCTION_NOT_FOUND
        }

        [Fact]
        public void CurlGotNothingException_Constructor_SetsProperties()
        {
            var ex = new CurlGotNothingException("https://example.com");
            ex.Message.Should().Contain("example.com");
            ex.CurlCode.Should().Be(52); // CURLE_GOT_NOTHING
        }

        [Fact]
        public void CurlHttpException_Constructor_SetsProperties()
        {
            var ex = new CurlHttpException("http error", 404);
            ex.Message.Should().Contain("http error");
            ex.StatusCode.Should().Be(404);
        }

        [Fact]
        public void CurlHttpPostErrorException_Constructor_SetsProperties()
        {
            var ex = new CurlHttpPostErrorException("post failed");
            ex.Message.Should().Contain("post failed");
            ex.CurlCode.Should().Be(34); // CURLE_HTTP_POST_ERROR
        }

        [Fact]
        public void CurlHttpReturnedErrorException_Constructor_SetsProperties()
        {
            var ex = new CurlHttpReturnedErrorException(404);
            ex.Message.Should().Contain("404");
            ex.StatusCode.Should().Be(404);
        }

        [Fact]
        public void CurlInterfaceFailedException_Constructor_SetsProperties()
        {
            var ex = new CurlInterfaceFailedException("interface");
            ex.Message.Should().Contain("interface");
        }

        [Fact]
        public void CurlLoginDeniedException_Constructor_SetsProperties()
        {
            var ex = new CurlLoginDeniedException("user", "example.com");
            ex.Message.Should().Contain("user");
            ex.Message.Should().Contain("example.com");
            ex.Username.Should().Be("user");
            ex.Host.Should().Be("example.com");
        }

        [Fact]
        public void CurlNotBuiltInException_Constructor_SetsProperties()
        {
            var ex = new CurlNotBuiltInException("feature");
            ex.Message.Should().Contain("feature");
            ex.Feature.Should().Be("feature");
        }

        [Fact]
        public void CurlNotSupportedException_Constructor_SetsProperties()
        {
            var ex = new CurlNotSupportedException("not supported");
            ex.Message.Should().Contain("not supported");
        }

        [Fact]
        public void CurlOptionSyntaxException_Constructor_SetsProperties()
        {
            var ex = new CurlOptionSyntaxException("--bad-option");
            ex.Message.Should().Contain("--bad-option");
            ex.Option.Should().Be("--bad-option");
        }

        [Fact]
        public void CurlOutOfMemoryException_Constructor_SetsProperties()
        {
            var ex = new CurlOutOfMemoryException("out of memory");
            ex.Message.Should().Contain("out of memory");
            ex.CurlCode.Should().Be(27); // CURLE_OUT_OF_MEMORY
        }

        [Fact]
        public void CurlParsingException_Constructor_SetsProperties()
        {
            var ex = new CurlParsingException("parsing failed");
            ex.Message.Should().Contain("parsing failed");
        }

        [Fact]
        public void CurlPeerFailedVerificationException_Constructor_SetsProperties()
        {
            var ex = new CurlPeerFailedVerificationException("example.com");
            ex.Message.Should().Contain("example.com");
            ex.Host.Should().Be("example.com");
        }

        [Fact]
        public void CurlProxyException_Constructor_SetsProperties()
        {
            var ex = new CurlProxyException("proxy error", "proxy.example.com", 8080);
            ex.Message.Should().Contain("proxy error");
            ex.ProxyHost.Should().Be("proxy.example.com");
            ex.ProxyPort.Should().Be(8080);
        }

        [Fact]
        public void CurlRateLimitException_Constructor_SetsProperties()
        {
            var ex = new CurlRateLimitException(10);
            ex.Message.Should().Contain("10");
            ex.RetryAfterSeconds.Should().Be(10);
        }

        [Fact]
        public void CurlReadErrorException_Constructor_SetsProperties()
        {
            var ex = new CurlReadErrorException("file.txt", "read failed");
            ex.Message.Should().Contain("file.txt");
            ex.Message.Should().Contain("read failed");
            ex.FilePath.Should().Be("file.txt");
        }

        [Fact]
        public void CurlReceiveErrorException_Constructor_SetsProperties()
        {
            var ex = new CurlReceiveErrorException("receive failed");
            ex.Message.Should().Contain("receive failed");
        }

        [Fact]
        public void CurlRedirectException_Constructor_SetsProperties()
        {
            var ex = new CurlRedirectException("redirect error", "https://example.com");
            ex.Message.Should().Contain("redirect error");
            ex.Url.Should().Be("https://example.com");
        }

        [Fact]
        public void CurlRemoteAccessDeniedException_Constructor_SetsProperties()
        {
            var ex = new CurlRemoteAccessDeniedException("https://example.com/resource");
            ex.Message.Should().Contain("example.com");
            ex.Url.Should().Be("https://example.com/resource");
        }

        [Fact]
        public void CurlSendErrorException_Constructor_SetsProperties()
        {
            var ex = new CurlSendErrorException("send failed");
            ex.Message.Should().Contain("send failed");
        }

        [Fact]
        public void CurlSslCertificateProblemException_Constructor_SetsProperties()
        {
            var ex = new CurlSslCertificateProblemException("cert.pem");
            ex.Message.Should().Contain("cert.pem");
            ex.CertificatePath.Should().Be("cert.pem");
        }

        [Fact]
        public void CurlSslCipherException_Constructor_SetsProperties()
        {
            var ex = new CurlSslCipherException("TLS_AES_256");
            ex.Message.Should().Contain("TLS_AES_256");
            ex.Cipher.Should().Be("TLS_AES_256");
        }

        [Fact]
        public void CurlSslConnectErrorException_Constructor_SetsProperties()
        {
            var ex = new CurlSslConnectErrorException("example.com");
            ex.Message.Should().Contain("example.com");
            ex.Host.Should().Be("example.com");
        }

        [Fact]
        public void CurlSslEngineNotFoundException_Constructor_SetsProperties()
        {
            var ex = new CurlSslEngineNotFoundException("openssl");
            ex.Message.Should().Contain("openssl");
            ex.Engine.Should().Be("openssl");
        }

        [Fact]
        public void CurlSslEngineSetFailedException_Constructor_SetsProperties()
        {
            var ex = new CurlSslEngineSetFailedException("openssl");
            ex.Message.Should().Contain("openssl");
            ex.Engine.Should().Be("openssl");
        }

        [Fact]
        public void CurlSslException_Constructor_SetsProperties()
        {
            var ex = new CurlSslException("ssl error", "example.com");
            ex.Message.Should().Contain("ssl error");
            ex.Host.Should().Be("example.com");
        }

        [Fact]
        public void CurlUnknownOptionException_Constructor_SetsProperties()
        {
            var ex = new CurlUnknownOptionException("--unknown");
            ex.Message.Should().Contain("--unknown");
            ex.Option.Should().Be("--unknown");
        }

        [Fact]
        public void CurlUploadFailedException_Constructor_SetsProperties()
        {
            var ex = new CurlUploadFailedException("upload failed");
            ex.Message.Should().Contain("upload failed");
        }

        [Fact]
        public void CurlUseSslFailedException_Constructor_SetsProperties()
        {
            var ex = new CurlUseSslFailedException("example.com");
            ex.Message.Should().Contain("example.com");
            ex.Host.Should().Be("example.com");
        }

        [Fact]
        public void CurlWeirdServerReplyException_Constructor_SetsProperties()
        {
            var ex = new CurlWeirdServerReplyException("weird reply");
            ex.Message.Should().Contain("weird reply");
        }

        [Fact]
        public void CurlWriteErrorException_Constructor_SetsProperties()
        {
            var ex = new CurlWriteErrorException("write failed");
            ex.Message.Should().Contain("write failed");
        }
    }
}
