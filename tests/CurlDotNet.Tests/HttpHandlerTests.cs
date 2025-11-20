using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Core.Handlers;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;

namespace CurlDotNet.Tests
{
    public class HttpHandlerTests
    {
        [Fact]
        public async Task ExecuteAsync_HeadersAreCaseInsensitive()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("test", Encoding.UTF8, "application/json"),
                    Headers =
                    {
                        { "X-Custom-Header", "value" }
                    }
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var httpHandler = new HttpHandler(httpClient);
            var options = new CurlOptions { Url = "http://example.com" };

            // Act
            var result = await httpHandler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            result.Headers.Should().ContainKey("Content-Type");
            result.Headers.Should().ContainKey("content-type"); // Case insensitive check
            result.Headers.Should().ContainKey("X-CUSTOM-HEADER"); // Case insensitive check
            result.GetHeader("content-type").Should().Be("application/json");
        }

        [Fact]
        public async Task ExecuteAsync_FollowsRedirects()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            
            // Setup redirect sequence
            handlerMock
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.MovedPermanently,
                    Headers = { Location = new Uri("http://example.com/new") }
                })
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("final destination")
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var httpHandler = new HttpHandler(httpClient);
            var options = new CurlOptions 
            { 
                Url = "http://example.com",
                FollowLocation = true 
            };

            // Act
            var result = await httpHandler.ExecuteAsync(options, CancellationToken.None);

            // Assert
            result.StatusCode.Should().Be(200);
            result.Body.Should().Be("final destination");
            
            // Verify calls
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(2),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}