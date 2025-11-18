using System;
using CurlDotNet.Core;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    [Trait("Category", TestCategories.Synthetic)]
    public class CurlRequestBuilderTests
    {
        [Fact]
        public void ToCurlCommand_ShouldIncludeHeadersAndPayload()
        {
            var command = CurlRequestBuilder
                .Post("https://api.example.com/users")
                .WithHeader("Accept", "application/json")
                .WithBearerToken("token123")
                .WithJson(new { name = "Ada" })
                .WithTimeout(TimeSpan.FromSeconds(5))
                .ToCurlCommand();

            command.Should().Contain("-X POST");
            command.Should().Contain("-H 'Accept: application/json'");
            command.Should().Contain("-H 'Authorization: Bearer token123'");
            command.Should().Contain("--max-time 5");
            command.Should().Contain("-d '{\"name\":\"Ada\"}'");
            command.Should().EndWith("https://api.example.com/users");
        }

        [Fact]
        public void GetOptions_ShouldReturnCloneWithConfiguredProperties()
        {
            var builder = CurlRequestBuilder
                .Get("https://api.example.com/data")
                .FollowRedirects()
                .Compressed()
                .WithProxy("http://proxy.local:8080")
                .WithCookie("session=abc");

            var options = builder.GetOptions();

            options.Url.Should().Be("https://api.example.com/data");
            options.FollowLocation.Should().BeTrue();
            options.Compressed.Should().BeTrue();
            options.Proxy.Should().Be("http://proxy.local:8080");
            options.Cookie.Should().Be("session=abc");
        }
    }
}
