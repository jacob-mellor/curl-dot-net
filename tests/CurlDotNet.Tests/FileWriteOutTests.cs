using System;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet.Core;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// -w/--write-out on file:// URLs. curl runs the write-out for every protocol; for FILE it reports
    /// no response code (000), scheme "file" and the file size as %{size_download}.
    /// </summary>
    [Trait("Category", TestCategories.Synthetic)]
    public class FileWriteOutTests : IDisposable
    {
        private readonly string _dir = Path.Combine(Path.GetTempPath(), $"curl-file-wo-{Guid.NewGuid():N}");

        public FileWriteOutTests()
        {
            Directory.CreateDirectory(_dir);
        }

        public void Dispose()
        {
            try { Directory.Delete(_dir, true); } catch (IOException) { }
        }

        private string Url(string name) => new Uri(Path.Combine(_dir, name)).AbsoluteUri;

        [Fact]
        public async Task WriteOut_AfterFileContent_PrintsCurlFileValues()
        {
            File.WriteAllText(Path.Combine(_dir, "hello.txt"), "hello");

            var result = await Curl.ExecuteAsync(
                $"curl {Url("hello.txt")} -w \"|%{{response_code}}|%{{http_code}}|%{{size_download}}|%{{scheme}}|%{{exitcode}}|%{{url_effective}}\"");

            result.Body.Should().Be($"hello|000|000|5|file|0|{Url("hello.txt")}");
        }

        [Fact]
        public async Task WriteOut_WithOutputFile_BodyHoldsOnlyWriteOut()
        {
            File.WriteAllText(Path.Combine(_dir, "data.json"), "{\"a\":1}");
            var target = Path.Combine(_dir, "copy.json");

            var result = await Curl.ExecuteAsync($"curl {Url("data.json")} -o \"{target}\" -w \"%{{filename_effective}} %{{size_download}}\"");

            File.ReadAllText(target).Should().Be("{\"a\":1}");
            result.Body.Should().Be($"{target} 7");
        }

        [Fact]
        public async Task WriteOut_MissingFile_ReportsCurlExit37()
        {
            var result = await Curl.ExecuteAsync($"curl {Url("missing.txt")} -w \"\\n%{{exitcode}} %{{errormsg}}\"");

            result.Body.Should().EndWith($"\n37 Couldn't open file {Path.Combine(_dir, "missing.txt")}");
        }

        [Fact]
        public async Task WriteOut_StderrSwitch_GoesToStderrFile()
        {
            File.WriteAllText(Path.Combine(_dir, "a.txt"), "abc");
            var log = Path.Combine(_dir, "err.log");

            var result = await Curl.ExecuteAsync($"curl {Url("a.txt")} --stderr \"{log}\" -w \"%{{stderr}}size=%{{size_download}}\"");

            result.Body.Should().Be("abc");
            File.ReadAllText(log).Should().Be("size=3");
        }
    }
}
