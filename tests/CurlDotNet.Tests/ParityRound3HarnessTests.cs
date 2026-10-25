using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// The opt-in default config file (.curlrc, <see cref="Curl.DefaultReadConfigFile"/>) and curl's short error text
    /// (<see cref="CurlException.CurlErrorMessage"/>). Every file lives in a temp folder; the real ~/.curlrc is never read
    /// (the lookup is fed a fake environment, or CURL_HOME points at a temp folder that has the file).
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class ParityRound3HarnessTests : IDisposable
    {
        private const string Hello = "HTTP/1.1 200 OK\r\nContent-Length: 6\r\n\r\nhello\n";
        private const string NotFound = "HTTP/1.1 404 Not Found\r\nContent-Length: 5\r\n\r\ngone\n";
        private readonly string _dir;

        public ParityRound3HarnessTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "curldotnet-round3-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
        }

        public void Dispose()
        {
            Curl.DefaultReadConfigFile = false;
            try
            {
                Directory.Delete(_dir, recursive: true);
            }
            catch (IOException)
            {
            }
        }

        private string Dir(string name)
        {
            var path = Path.Combine(_dir, name);
            Directory.CreateDirectory(path);
            return path;
        }

        private static string Touch(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, "--silent\n");
            return path;
        }

        private static string? Find(Dictionary<string, string> env, bool windows = false, string? account = null) =>
            CommandParser.FindDefaultConfigFile(name => env.TryGetValue(name, out var v) ? v : null, windows, () => account);

        // ------------------------------------------------------------------ lookup order (src/tool_findfile.c)

        [Fact]
        public void Lookup_CurlHomeWinsOverXdgAndHome()
        {
            var curlHome = Dir("curlhome");
            var xdg = Dir("xdg");
            var home = Dir("home");
            var expected = Touch(Path.Combine(curlHome, ".curlrc"));
            Touch(Path.Combine(xdg, "curlrc"));
            Touch(Path.Combine(home, ".curlrc"));

            Find(new Dictionary<string, string> { ["CURL_HOME"] = curlHome, ["XDG_CONFIG_HOME"] = xdg, ["HOME"] = home })
                .Should().Be(expected);
        }

        [Fact]
        public void Lookup_XdgConfigHome_UsesCurlrcWithoutDot_BeforeHome()
        {
            var xdg = Dir("xdg");
            var home = Dir("home");
            Touch(Path.Combine(xdg, ".curlrc")); // not the name curl looks for there
            var expected = Touch(Path.Combine(xdg, "curlrc"));
            Touch(Path.Combine(home, ".curlrc"));

            Find(new Dictionary<string, string> { ["XDG_CONFIG_HOME"] = xdg, ["HOME"] = home }).Should().Be(expected);
        }

        [Fact]
        public void Lookup_FallsBackToHomeDotCurlrc()
        {
            var home = Dir("home");
            var expected = Touch(Path.Combine(home, ".curlrc"));

            Find(new Dictionary<string, string> { ["CURL_HOME"] = Dir("emptycurlhome"), ["HOME"] = home }).Should().Be(expected);
        }

        [Fact]
        public void Lookup_DotConfigCurlrc_UnderCurlHome_WhenXdgIsUnset()
        {
            var curlHome = Dir("curlhome");
            var expected = Touch(Path.Combine(curlHome, ".config", "curlrc"));

            Find(new Dictionary<string, string> { ["CURL_HOME"] = curlHome }).Should().Be(expected);
        }

        [Fact]
        public void Lookup_DotConfigFallbacks_AreSkipped_WhenXdgIsSet()
        {
            var curlHome = Dir("curlhome");
            Touch(Path.Combine(curlHome, ".config", "curlrc"));

            Find(new Dictionary<string, string> { ["CURL_HOME"] = curlHome, ["XDG_CONFIG_HOME"] = Dir("emptyxdg") })
                .Should().BeNull();
        }

        [Fact]
        public void Lookup_EmptyVariables_AreSkipped()
        {
            var curlHome = Dir("curlhome");
            Touch(Path.Combine(curlHome, ".config", "curlrc"));

            // an empty XDG_CONFIG_HOME counts as unset, so the .config fallback still applies
            Find(new Dictionary<string, string> { ["CURL_HOME"] = curlHome, ["XDG_CONFIG_HOME"] = string.Empty, ["HOME"] = string.Empty })
                .Should().Be(Path.Combine(curlHome, ".config", "curlrc"));
        }

        [Fact]
        public void Lookup_AccountHome_IsTheLastResort()
        {
            var account = Dir("account");
            var expected = Touch(Path.Combine(account, ".curlrc"));

            Find(new Dictionary<string, string>(), account: account).Should().Be(expected);
            Find(new Dictionary<string, string>(), account: Dir("emptyaccount")).Should().BeNull();
        }

        [Fact]
        public void Lookup_Windows_TriesUnderscoreName_AndUserProfile()
        {
            var profile = Dir("profile");
            var expected = profile + "\\" + "_curlrc";

            // the Windows rules join with a backslash; on other systems that is part of the file name, which is fine here
            Touch(expected);

            Find(new Dictionary<string, string> { ["USERPROFILE"] = profile }, windows: true).Should().Be(expected);
            Find(new Dictionary<string, string> { ["USERPROFILE"] = profile }, windows: false).Should().BeNull();
        }

        [Theory]
        [InlineData("-q", true)]
        [InlineData("-qs", true)]
        [InlineData("--disable", true)]
        [InlineData("--disable-epsv", false)]
        [InlineData("-s", false)]
        [InlineData("http://example.com/", false)]
        public void FirstArgument_DisablesTheDefaultConfigFile_OnlyForMinusQ(string first, bool disables)
        {
            CommandParser.DisablesDefaultConfigFile(new List<string> { first, "http://example.com/" }).Should().Be(disables);
        }

        // ------------------------------------------------------------------ reading it (opt-in)

        private sealed class CurlHomeScope : IDisposable
        {
            private readonly string? _old = Environment.GetEnvironmentVariable("CURL_HOME");

            public CurlHomeScope(string dir)
            {
                Environment.SetEnvironmentVariable("CURL_HOME", dir);
            }

            public void Dispose()
            {
                Environment.SetEnvironmentVariable("CURL_HOME", _old);
                Curl.DefaultReadConfigFile = false;
            }
        }

        private string WriteCurlrc(string content)
        {
            var home = Dir("curlhome");
            File.WriteAllText(Path.Combine(home, ".curlrc"), content);
            return home;
        }

        [Fact]
        public async Task DefaultConfigFile_IsIgnoredUnlessTheApplicationOptsIn()
        {
            using var server = new RawReplyServer(Hello);
            using var scope = new CurlHomeScope(WriteCurlrc("header = \"X-From-Curlrc: yes\"\n"));

            Curl.DefaultReadConfigFile.Should().BeFalse("reading ~/.curlrc is off by default");
            await Curl.ExecuteAsync($"curl {server.Url("/off")}");

            server.Requests.Single().Should().NotContain("X-From-Curlrc");
        }

        [Fact]
        public async Task DefaultConfigFile_OptIn_AppliesItsOptionsBeforeTheCommand()
        {
            using var server = new RawReplyServer(Hello);
            using var scope = new CurlHomeScope(WriteCurlrc("--next\nheader = \"a: a\"\ndata = \"curlrc read\"\n"));
            Curl.DefaultReadConfigFile = true;

            await Curl.ExecuteAsync($"curl {server.Url("/on")}");

            var request = server.Requests.Single();
            request.Should().StartWith("POST /on HTTP/1.1\r\n");
            request.Should().Contain("\r\na: a\r\n");
            request.Should().EndWith("\r\n\r\ncurlrc read");
        }

        [Theory]
        [InlineData("-q")]
        [InlineData("--disable")]
        public async Task DefaultConfigFile_OptIn_IsSkippedWhenTheCommandStartsWithMinusQ(string disable)
        {
            using var server = new RawReplyServer(Hello);
            using var scope = new CurlHomeScope(WriteCurlrc("header = \"X-From-Curlrc: yes\"\n"));
            Curl.DefaultReadConfigFile = true;

            await Curl.ExecuteAsync($"curl {disable} {server.Url("/q")}");

            server.Requests.Single().Should().NotContain("X-From-Curlrc");
        }

        [Fact]
        public void DefaultConfigFile_OptIn_WritesCurlsNote_OnlyWhenTracingAndNotSilent()
        {
            var home = WriteCurlrc("header = \"a: a\"\n");
            using var scope = new CurlHomeScope(home);
            Curl.DefaultReadConfigFile = true;
            var parser = new CommandParser();
            var path = Path.Combine(home, ".curlrc");

            // parsing only records curl's messages; the engine writes them (Flush) when the command runs
            var quiet = Path.Combine(_dir, "quiet.txt");
            var quietOptions = parser.Parse($"curl --stderr {CommandParser.ShellQuote(quiet)} http://example.com/");
            quietOptions.Messages!.Flush();
            File.Exists(quiet).Should().BeFalse("curl's notef() prints nothing without -v/--trace");
            quietOptions.Messages.Snapshot().Should().BeEmpty();

            var noisy = Path.Combine(_dir, "noisy.txt");
            var noisyOptions = parser.Parse($"curl -v --stderr {CommandParser.ShellQuote(noisy)} http://example.com/");
            File.Exists(noisy).Should().BeFalse("Parse (and so Curl.Validate) writes nothing");
            noisyOptions.Messages!.Flush();
            File.ReadAllText(noisy).Should().Be(ToolMessages.Format("Note: ", $"Read config file from '{path}'", ToolMessages.TerminalColumns()),
                "written like curl's notef(): wrapped at the COLUMNS width");
            noisyOptions.Messages.Snapshot().Should().Equal($"Note: Read config file from '{path}'");

            var silent = Path.Combine(_dir, "silent.txt");
            var silentOptions = parser.Parse($"curl -v -s --stderr {CommandParser.ShellQuote(silent)} http://example.com/");
            silentOptions.Messages!.Flush();
            File.Exists(silent).Should().BeFalse("-s mutes notes");
        }

        [Fact]
        public void Lookup_ReturnsNull_WhenNoPlaceHasTheFile()
        {
            Find(new Dictionary<string, string> { ["CURL_HOME"] = Dir("a"), ["XDG_CONFIG_HOME"] = Dir("b"), ["HOME"] = Dir("c") }, account: Dir("d"))
                .Should().BeNull();
        }

        // ------------------------------------------------------------------ curl's short error text

        [Fact]
        public void CurlErrorMessage_ForFail_IsCurlsExactText()
        {
            var withBody = new CurlHttpReturnedErrorException(404, "Not Found", "gone", "http://example.com/x?y=1", true, "curl --fail-with-body http://example.com/x?y=1");
            var plain = new CurlHttpReturnedErrorException(503, "Service Unavailable", "busy", "curl -f http://example.com/");

            withBody.CurlErrorMessage.Should().Be("The requested URL returned error: 404");
            plain.CurlErrorMessage.Should().Be("The requested URL returned error: 503");
            withBody.WhatHappened.Should().Contain("404 Not Found", "WhatHappened keeps CurlDotNet's longer line");
        }

        [Fact]
        public void CurlErrorMessage_OtherwiseIsTheFirstMessageLine()
        {
            var ex = new CurlException("Something specific went wrong\nDo this to fix it.", 7);

            ex.CurlErrorMessage.Should().Be(ex.WhatHappened);
            ex.CurlErrorMessage.Should().NotContain("\n");
        }

        [Fact]
        public async Task CurlErrorMessage_FromARealFailTransfer()
        {
            using var server = new RawReplyServer(NotFound);

            Func<Task> act = () => Curl.ExecuteAsync($"curl -f {server.Url("/missing")}");

            var thrown = await act.Should().ThrowAsync<CurlHttpReturnedErrorException>();
            thrown.Which.CurlErrorCode.Should().Be(22);
            $"curl: ({thrown.Which.CurlErrorCode}) {thrown.Which.CurlErrorMessage}"
                .Should().Be("curl: (22) The requested URL returned error: 404");
        }
    }
}
