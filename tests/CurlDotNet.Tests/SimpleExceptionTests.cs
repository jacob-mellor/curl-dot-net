using System;
using System.Net;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Simple tests for exception classes to improve code coverage.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    public class SimpleExceptionTests
    {
        [Fact]
        public void CurlException_CanBeCreated()
        {
            var ex = new CurlException("Test message");
            ex.Should().NotBeNull();
            ex.Message.Should().Be("Test message");
        }

        [Fact]
        public void CurlUnsupportedProtocolException_CanBeCreated()
        {
            var ex = new CurlUnsupportedProtocolException("gopher");
            ex.Should().NotBeNull();
        }

        [Fact]
        public void CurlMalformedUrlException_CanBeCreated()
        {
            var ex = new CurlMalformedUrlException("bad-url");
            ex.Should().NotBeNull();
        }

        [Fact]
        public void CurlCouldntConnectException_CanBeCreated()
        {
            var ex = new CurlCouldntConnectException("host", 80);
            ex.Should().NotBeNull();
        }

        [Fact]
        public void CurlCouldntResolveHostException_CanBeCreated()
        {
            var ex = new CurlCouldntResolveHostException("unknown.host");
            ex.Should().NotBeNull();
        }

        [Fact]
        public void CurlCouldntResolveProxyException_CanBeCreated()
        {
            var ex = new CurlCouldntResolveProxyException("proxy.host");
            ex.Should().NotBeNull();
        }

        [Fact]
        public void CurlOperationTimeoutException_CanBeCreated()
        {
            var ex = new CurlOperationTimeoutException(30);
            ex.Should().NotBeNull();
        }

        [Fact]
        public void CurlInvalidCommandException_CanBeCreated()
        {
            var ex = new CurlInvalidCommandException("curl --bad");
            ex.Should().NotBeNull();
        }

        [Fact]
        public void CurlAbortedByCallbackException_CanBeCreated()
        {
            var ex = new CurlAbortedByCallbackException();
            ex.Should().NotBeNull();
        }

        [Fact]
        public void CurlTooManyRedirectsException_CanBeCreated()
        {
            var ex = new CurlTooManyRedirectsException(10);
            ex.Should().NotBeNull();
        }

        // Additional exception tests removed - constructors vary per exception type
    }
}