using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using CurlDotNet;
using CurlDotNet.Core;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Comprehensive tests for CurlSettings to achieve 100% code coverage.
    /// Tests all properties, fluent methods, and edge cases.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    public class CurlSettingsFullCoverageTests
    {
        #region Property Tests

        [Fact]
        public void DefaultConstructor_InitializesPropertiesCorrectly()
        {
            // Arrange & Act
            var settings = new CurlSettings();

            // Assert
            settings.CancellationToken.Should().Be(CancellationToken.None);
            settings.MaxTimeSeconds.Should().BeNull();
            settings.ConnectTimeoutSeconds.Should().BeNull();
            settings.FollowRedirects.Should().BeNull();
            settings.Insecure.Should().BeNull();
            settings.RetryCount.Should().Be(0);
            settings.RetryDelayMs.Should().Be(1000);
            settings.OnProgress.Should().BeNull();
            settings.OnRedirect.Should().BeNull();
            settings.Headers.Should().NotBeNull().And.BeEmpty();
            settings.Proxy.Should().BeNull();
            settings.UserAgent.Should().BeNull();
            settings.Cookies.Should().BeNull();
            settings.AutomaticDecompression.Should().BeTrue();
            settings.BufferSize.Should().Be(8192);
        }

        [Fact]
        public void Properties_CanBeSetDirectly()
        {
            // Arrange
            var settings = new CurlSettings();
            var cts = new CancellationTokenSource();

            // Act
            settings.CancellationToken = cts.Token;
            settings.MaxTimeSeconds = 60;
            settings.ConnectTimeoutSeconds = 10;
            settings.FollowRedirects = true;
            settings.Insecure = false;
            settings.RetryCount = 3;
            settings.RetryDelayMs = 2000;
            settings.OnProgress = (p, t, c) => { };
            settings.OnRedirect = url => { };
            settings.Headers = new Dictionary<string, string> { ["Test"] = "Value" };
            settings.Proxy = new WebProxy("http://proxy.example.com");
            settings.UserAgent = "TestAgent/1.0";
            settings.Cookies = new CookieContainer();
            settings.AutomaticDecompression = false;
            settings.BufferSize = 4096;

            // Assert
            settings.CancellationToken.Should().Be(cts.Token);
            settings.MaxTimeSeconds.Should().Be(60);
            settings.ConnectTimeoutSeconds.Should().Be(10);
            settings.FollowRedirects.Should().BeTrue();
            settings.Insecure.Should().BeFalse();
            settings.RetryCount.Should().Be(3);
            settings.RetryDelayMs.Should().Be(2000);
            settings.OnProgress.Should().NotBeNull();
            settings.OnRedirect.Should().NotBeNull();
            settings.Headers.Should().ContainKey("Test");
            settings.Proxy.Should().NotBeNull();
            settings.UserAgent.Should().Be("TestAgent/1.0");
            settings.Cookies.Should().NotBeNull();
            settings.AutomaticDecompression.Should().BeFalse();
            settings.BufferSize.Should().Be(4096);
        }

        #endregion

        #region Fluent Builder Method Tests

        [Fact]
        public void WithCancellation_SetsCancellationToken()
        {
            // Arrange
            var settings = new CurlSettings();
            var cts = new CancellationTokenSource();

            // Act
            var result = settings.WithCancellation(cts.Token);

            // Assert
            result.Should().BeSameAs(settings); // Fluent return
            settings.CancellationToken.Should().Be(cts.Token);
        }

        [Fact]
        public void WithTimeout_SetsMaxTimeSeconds()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            var result = settings.WithTimeout(30);

            // Assert
            result.Should().BeSameAs(settings);
            settings.MaxTimeSeconds.Should().Be(30);
        }

        [Fact]
        public void WithConnectTimeout_SetsConnectTimeoutSeconds()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            var result = settings.WithConnectTimeout(10);

            // Assert
            result.Should().BeSameAs(settings);
            settings.ConnectTimeoutSeconds.Should().Be(10);
        }

        [Fact]
        public void WithFollowRedirects_SetsFollowRedirectsTrue()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            var result = settings.WithFollowRedirects();

            // Assert
            result.Should().BeSameAs(settings);
            settings.FollowRedirects.Should().BeTrue();
        }

        [Fact]
        public void WithFollowRedirects_SetsFollowRedirectsFalse()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            var result = settings.WithFollowRedirects(false);

            // Assert
            result.Should().BeSameAs(settings);
            settings.FollowRedirects.Should().BeFalse();
        }

        [Fact]
        public void WithInsecure_SetsInsecureTrue()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            var result = settings.WithInsecure();

            // Assert
            result.Should().BeSameAs(settings);
            settings.Insecure.Should().BeTrue();
        }

        [Fact]
        public void WithInsecure_SetsInsecureFalse()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            var result = settings.WithInsecure(false);

            // Assert
            result.Should().BeSameAs(settings);
            settings.Insecure.Should().BeFalse();
        }

        [Fact]
        public void WithRetries_SetsRetryCountAndDelay()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            var result = settings.WithRetries(5, 2000);

            // Assert
            result.Should().BeSameAs(settings);
            settings.RetryCount.Should().Be(5);
            settings.RetryDelayMs.Should().Be(2000);
        }

        [Fact]
        public void WithRetries_UsesDefaultDelayWhenNotSpecified()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            var result = settings.WithRetries(3);

            // Assert
            result.Should().BeSameAs(settings);
            settings.RetryCount.Should().Be(3);
            settings.RetryDelayMs.Should().Be(1000);
        }

        [Fact]
        public void WithProgress_SetsProgressCallback()
        {
            // Arrange
            var settings = new CurlSettings();
            var progressCalled = false;
            Action<double, long, long> callback = (p, t, c) => progressCalled = true;

            // Act
            var result = settings.WithProgress(callback);

            // Assert
            result.Should().BeSameAs(settings);
            settings.OnProgress.Should().NotBeNull();
            settings.OnProgress(50, 100, 50);
            progressCalled.Should().BeTrue();
        }

        [Fact]
        public void WithRedirectHandler_SetsRedirectCallback()
        {
            // Arrange
            var settings = new CurlSettings();
            string redirectUrl = null;
            Action<string> callback = url => redirectUrl = url;

            // Act
            var result = settings.WithRedirectHandler(callback);

            // Assert
            result.Should().BeSameAs(settings);
            settings.OnRedirect.Should().NotBeNull();
            settings.OnRedirect("https://redirect.example.com");
            redirectUrl.Should().Be("https://redirect.example.com");
        }

        [Fact]
        public void WithHeader_AddsSingleHeader()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            var result = settings.WithHeader("Authorization", "Bearer token123");

            // Assert
            result.Should().BeSameAs(settings);
            settings.Headers.Should().ContainKey("Authorization");
            settings.Headers["Authorization"].Should().Be("Bearer token123");
        }

        [Fact]
        public void WithHeader_OverwritesExistingHeader()
        {
            // Arrange
            var settings = new CurlSettings();
            settings.WithHeader("Test", "OldValue");

            // Act
            var result = settings.WithHeader("Test", "NewValue");

            // Assert
            result.Should().BeSameAs(settings);
            settings.Headers["Test"].Should().Be("NewValue");
        }

        [Fact]
        public void WithHeaders_AddsMultipleHeaders()
        {
            // Arrange
            var settings = new CurlSettings();
            var headers = new Dictionary<string, string>
            {
                ["Header1"] = "Value1",
                ["Header2"] = "Value2",
                ["Header3"] = "Value3"
            };

            // Act
            var result = settings.WithHeaders(headers);

            // Assert
            result.Should().BeSameAs(settings);
            settings.Headers.Should().HaveCount(3);
            settings.Headers["Header1"].Should().Be("Value1");
            settings.Headers["Header2"].Should().Be("Value2");
            settings.Headers["Header3"].Should().Be("Value3");
        }

        [Fact]
        public void WithHeaders_OverwritesExistingHeaders()
        {
            // Arrange
            var settings = new CurlSettings();
            settings.WithHeader("Header1", "OldValue");
            var headers = new Dictionary<string, string>
            {
                ["Header1"] = "NewValue",
                ["Header2"] = "Value2"
            };

            // Act
            var result = settings.WithHeaders(headers);

            // Assert
            result.Should().BeSameAs(settings);
            settings.Headers["Header1"].Should().Be("NewValue");
            settings.Headers["Header2"].Should().Be("Value2");
        }

        [Fact]
        public void WithProxy_SetsProxyWithUrl()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            var result = settings.WithProxy("http://proxy.example.com:8080");

            // Assert
            result.Should().BeSameAs(settings);
            settings.Proxy.Should().NotBeNull();
            var webProxy = settings.Proxy as WebProxy;
            webProxy.Should().NotBeNull();
            webProxy.Address.Should().NotBeNull();
            webProxy.Address.ToString().Should().Contain("proxy.example.com");
        }

        [Fact]
        public void WithProxy_SetsProxyWithCredentials()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            var result = settings.WithProxy("http://proxy.example.com:8080", "username", "password");

            // Assert
            result.Should().BeSameAs(settings);
            settings.Proxy.Should().NotBeNull();
            var webProxy = settings.Proxy as WebProxy;
            webProxy.Should().NotBeNull();
            webProxy.Credentials.Should().NotBeNull();
            var credentials = webProxy.Credentials as NetworkCredential;
            credentials.Should().NotBeNull();
            credentials.UserName.Should().Be("username");
            credentials.Password.Should().Be("password");
        }

        [Fact]
        public void WithUserAgent_SetsUserAgent()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            var result = settings.WithUserAgent("MyApp/1.0");

            // Assert
            result.Should().BeSameAs(settings);
            settings.UserAgent.Should().Be("MyApp/1.0");
        }

        [Fact]
        public void WithCookies_CreatesNewCookieContainer()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            var result = settings.WithCookies();

            // Assert
            result.Should().BeSameAs(settings);
            settings.Cookies.Should().NotBeNull();
        }

        [Fact]
        public void WithCookies_UsesProvidedCookieContainer()
        {
            // Arrange
            var settings = new CurlSettings();
            var container = new CookieContainer();
            container.Add(new Uri("http://example.com"), new Cookie("test", "value"));

            // Act
            var result = settings.WithCookies(container);

            // Assert
            result.Should().BeSameAs(settings);
            settings.Cookies.Should().BeSameAs(container);
            settings.Cookies.Count.Should().Be(1);
        }

        [Fact]
        public void WithAutoDecompression_EnablesDecompression()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            var result = settings.WithAutoDecompression();

            // Assert
            result.Should().BeSameAs(settings);
            settings.AutomaticDecompression.Should().BeTrue();
        }

        [Fact]
        public void WithAutoDecompression_DisablesDecompression()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            var result = settings.WithAutoDecompression(false);

            // Assert
            result.Should().BeSameAs(settings);
            settings.AutomaticDecompression.Should().BeFalse();
        }

        [Fact]
        public void WithBufferSize_SetsBufferSize()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            var result = settings.WithBufferSize(16384);

            // Assert
            result.Should().BeSameAs(settings);
            settings.BufferSize.Should().Be(16384);
        }

        #endregion

        #region Chaining Tests

        [Fact]
        public void FluentMethods_CanBeChained()
        {
            // Arrange & Act
            var settings = new CurlSettings()
                .WithTimeout(30)
                .WithConnectTimeout(10)
                .WithFollowRedirects()
                .WithInsecure(false)
                .WithRetries(3, 2000)
                .WithHeader("Accept", "application/json")
                .WithUserAgent("TestApp/1.0")
                .WithCookies()
                .WithAutoDecompression()
                .WithBufferSize(4096);

            // Assert
            settings.MaxTimeSeconds.Should().Be(30);
            settings.ConnectTimeoutSeconds.Should().Be(10);
            settings.FollowRedirects.Should().BeTrue();
            settings.Insecure.Should().BeFalse();
            settings.RetryCount.Should().Be(3);
            settings.RetryDelayMs.Should().Be(2000);
            settings.Headers["Accept"].Should().Be("application/json");
            settings.UserAgent.Should().Be("TestApp/1.0");
            settings.Cookies.Should().NotBeNull();
            settings.AutomaticDecompression.Should().BeTrue();
            settings.BufferSize.Should().Be(4096);
        }

        #endregion

        #region FromDefaults Tests

        [Fact]
        public void FromDefaults_UsesGlobalCurlSettings()
        {
            // Arrange - Save original values
            var originalMaxTime = Curl.DefaultMaxTimeSeconds;
            var originalConnectTimeout = Curl.DefaultConnectTimeoutSeconds;
            var originalFollowRedirects = Curl.DefaultFollowRedirects;
            var originalInsecure = Curl.DefaultInsecure;

            try
            {
                // Set test values
                Curl.DefaultMaxTimeSeconds = 60;
                Curl.DefaultConnectTimeoutSeconds = 15;
                Curl.DefaultFollowRedirects = true;
                Curl.DefaultInsecure = false;

                // Act
                var settings = CurlSettings.FromDefaults();

                // Assert
                settings.MaxTimeSeconds.Should().Be(60);
                settings.ConnectTimeoutSeconds.Should().Be(15);
                settings.FollowRedirects.Should().BeTrue();
                settings.Insecure.Should().BeFalse();
            }
            finally
            {
                // Restore original values
                Curl.DefaultMaxTimeSeconds = originalMaxTime;
                Curl.DefaultConnectTimeoutSeconds = originalConnectTimeout;
                Curl.DefaultFollowRedirects = originalFollowRedirects;
                Curl.DefaultInsecure = originalInsecure;
            }
        }

        [Fact]
        public void FromDefaults_HandlesZeroTimeouts()
        {
            // Arrange - Save original values
            var originalMaxTime = Curl.DefaultMaxTimeSeconds;
            var originalConnectTimeout = Curl.DefaultConnectTimeoutSeconds;

            try
            {
                // Set zero timeouts
                Curl.DefaultMaxTimeSeconds = 0;
                Curl.DefaultConnectTimeoutSeconds = 0;

                // Act
                var settings = CurlSettings.FromDefaults();

                // Assert
                settings.MaxTimeSeconds.Should().BeNull();
                settings.ConnectTimeoutSeconds.Should().BeNull();
            }
            finally
            {
                // Restore original values
                Curl.DefaultMaxTimeSeconds = originalMaxTime;
                Curl.DefaultConnectTimeoutSeconds = originalConnectTimeout;
            }
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Headers_InitializedAsEmptyDictionary()
        {
            // Arrange & Act
            var settings = new CurlSettings();

            // Assert
            settings.Headers.Should().NotBeNull();
            settings.Headers.Should().BeEmpty();

            // Should be able to add without null reference
            settings.Headers.Add("Test", "Value");
            settings.Headers["Test"].Should().Be("Value");
        }

        [Fact]
        public void MultipleCallsToSameFluentMethod_OverwritesPreviousValue()
        {
            // Arrange
            var settings = new CurlSettings();

            // Act
            settings
                .WithTimeout(30)
                .WithTimeout(60)
                .WithUserAgent("Agent1")
                .WithUserAgent("Agent2");

            // Assert
            settings.MaxTimeSeconds.Should().Be(60);
            settings.UserAgent.Should().Be("Agent2");
        }

        #endregion
    }
}