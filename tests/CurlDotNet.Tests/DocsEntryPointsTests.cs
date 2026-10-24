using System;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Argument and serialization errors from the static entry points (Curl, DotNetCurl) say what happened,
    /// how to fix it, and link the documentation.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class DocsEntryPointsTests
    {
        private const string Docs = "See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/";

        [Fact]
        public async Task ExecuteAsync_NullCommand_IsArgumentExceptionWithFixAndLink()
        {
            Func<Task> act = () => Curl.ExecuteAsync((string)null!);

            var ex = (await act.Should().ThrowExactlyAsync<ArgumentException>()).Which;
            ex.ParamName.Should().Be("command");
            ex.Message.Should().Contain("Curl.ExecuteAsync(command)").And.Contain("curl https://api.example.com").And.Contain(Docs);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ExecuteAsync_BlankCommand_IsArgumentExceptionWithFixAndLink(string command)
        {
            Func<Task> act = () => Curl.ExecuteAsync(command, CancellationToken.None);

            var ex = (await act.Should().ThrowExactlyAsync<ArgumentException>()).Which;
            ex.Message.Should().Contain("empty command").And.Contain("contains at least a URL").And.Contain(Docs);
        }

        [Fact]
        public void Execute_BlankCommandWithSettings_ThrowsDirectlyNotAggregate()
        {
            Action act = () => Curl.Execute(" ", new CurlSettings());

            act.Should().ThrowExactly<ArgumentException>().Which.Message.Should().Contain("Curl.ExecuteAsync(command, settings)");
        }

        [Fact]
        public async Task ExecuteAsync_NullOptions_SaysToPassCurlOptionsWithUrl()
        {
            Func<Task> act = () => Curl.ExecuteAsync((CurlOptions)null!);

            var ex = (await act.Should().ThrowAsync<ArgumentNullException>()).Which;
            ex.Message.Should().Contain("new CurlOptions").And.Contain(Docs);
        }

        [Fact]
        public async Task GetAsync_EmptyUrl_KeepsArgumentExceptionAndExplains()
        {
            Func<Task> act = () => Curl.GetAsync("");

            var ex = (await act.Should().ThrowExactlyAsync<ArgumentException>()).Which;
            ex.ParamName.Should().Be("url");
            ex.Message.Should().Contain("Curl.GetAsync(url)").And.Contain("https://").And.Contain("url-errors.md");
        }

        [Fact]
        public async Task DownloadAsync_EmptyOutputPath_NamesTheUrlWithoutSecrets()
        {
            Func<Task> act = () => Curl.DownloadAsync("https://user:pw@example.com/file.zip?token=abc", " ");

            var ex = (await act.Should().ThrowExactlyAsync<ArgumentException>()).Which;
            ex.ParamName.Should().Be("outputPath");
            ex.Message.Should().Contain("https://example.com/file.zip").And.Contain("download-file").And.Contain(Docs);
            ex.Message.Should().NotContain("pw@").And.NotContain("token=abc");
        }

        private sealed class Node
        {
            public Node? Self { get; set; }
        }

        [Fact]
        public async Task PostJsonAsync_UnserializableObject_IsCurlParsingExceptionWithInner()
        {
            var cyclic = new Node();
            cyclic.Self = cyclic;

            Func<Task> act = () => Curl.PostJsonAsync("https://api.example.com/items", cyclic);

            var ex = (await act.Should().ThrowAsync<CurlParsingException>()).Which;
            ex.InnerException.Should().NotBeNull();
            ex.Message.Should().Contain("Node").And.Contain("https://api.example.com/items").And.Contain("PostAsync").And.Contain(Docs);
            ex.Suggestions.Should().NotBeEmpty();
        }

        [Fact]
        public void DefaultFlavor_UndefinedValue_ListsTheValidValues()
        {
            Action act = () => Curl.DefaultFlavor = (CurlFlavor)42;

            var ex = act.Should().Throw<ArgumentOutOfRangeException>().Which;
            ex.Message.Should().Contain("42").And.Contain("WindowsCmd").And.Contain("curl-flavors");
            Curl.DefaultFlavor.Should().Be(CurlFlavor.Auto);
        }

        [Fact]
        public void DotNetCurl_NegativeTimeout_SaysWhatToPass()
        {
            Action act = () => DotNetCurl.Curl("curl https://example.com", -5);

            var ex = act.Should().Throw<ArgumentOutOfRangeException>().Which;
            ex.ParamName.Should().Be("timeoutSeconds");
            ex.Message.Should().Contain("-5").And.Contain("timeout").And.Contain(Docs);
        }

        [Fact]
        public async Task ExecuteManyAsync_NullEntry_NamesTheIndex()
        {
            Func<Task> act = () => Curl.ExecuteManyAsync(null!, "curl http://127.0.0.1:9/");

            var ex = (await act.Should().ThrowExactlyAsync<ArgumentException>()).Which;
            ex.Message.Should().Contain("index 0 of 2").And.Contain(Docs);
        }

        [Fact]
        public async Task ExecuteManyAsync_NullArray_Explains()
        {
            Func<Task> act = () => Curl.ExecuteManyAsync(null!);

            (await act.Should().ThrowAsync<ArgumentNullException>()).Which.Message.Should().Contain("empty array is allowed");
        }

        [Fact]
        public async Task ExecuteManyAsync_EmptyArray_ReturnsEmpty()
        {
            (await Curl.ExecuteManyAsync()).Should().BeEmpty();
        }
    }
}
