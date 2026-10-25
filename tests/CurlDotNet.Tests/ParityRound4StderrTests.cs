using System;
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
    /// curl's exact standard-error text (round 4): <see cref="CurlException.CurlErrorMessage"/> for upload resume, Transfer-Encoding
    /// and glob errors, command-line usage errors (<see cref="CurlException.IsUsageError"/>), and the "Warning: "/"Note: " lines
    /// collected on <see cref="CurlResult.Warnings"/> / <see cref="CurlException.Warnings"/>. Expected strings are copied from
    /// curl's sources (src/tool_getparam.c, tool_parsecfg.c, tool_operate.c, tool_helpers.c, tool_msgs.c, tool_urlglob.c,
    /// lib/content_encoding.c) and from real curl's output for the matching curl tests.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class ParityRound4StderrTests : IDisposable
    {
        private const string Hello = "HTTP/1.1 200 OK\r\nContent-Length: 6\r\n\r\nhello\n";
        private readonly string _dir;
        private readonly CommandParser _parser = new CommandParser();

        public ParityRound4StderrTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "curldotnet-round4-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(_dir, recursive: true);
            }
            catch (IOException)
            {
            }
        }

        private string FilePath(string name) => Path.Combine(_dir, name);

        private static string Q(string text) => CommandParser.ShellQuote(text);

        private CurlException ParseFails(string command)
        {
            Action parse = () => _parser.Parse(command);
            return parse.Should().Throw<CurlException>().Which;
        }

        private System.Collections.Generic.IReadOnlyList<string> ParsedMessages(string command) =>
            _parser.Parse(command).Messages!.Snapshot();

        // ------------------------------------------------------------------ CurlErrorMessage: curl's error buffer text

        [Theory]
        [InlineData("-d foobar -C 3")]     // curl test 426
        [InlineData("-d foobar -C -")]     // curl test 2271
        [InlineData("-F name=value -C -")] // curl test 2272
        public void UploadResume_CurlErrorMessage_IsCurlsText(string options)
        {
            var ex = ParseFails($"curl http://127.0.0.1:1/x {options}");

            ex.CurlErrorCode.Should().Be(43);
            ex.CurlErrorMessage.Should().Be("HTTP upload cannot be resumed");
            ex.Message.Should().Contain("-C/--continue-at", "Message keeps CurlDotNet's explanation and fix");
        }

        [Fact]
        public void UploadResume_WithFile_CurlErrorMessage_IsCurlsText()
        {
            var file = FilePath("up.txt");
            File.WriteAllText(file, "data");

            var ex = ParseFails($"curl http://127.0.0.1:1/x -T {Q(file)} -C 50");

            ex.CurlErrorMessage.Should().Be("HTTP upload cannot be resumed");
        }

        [Fact]
        public void NamedGlobThatDoesNotExist_CurlErrorMessage_HasPositionNameAndCaret()
        {
            // curl tests 2411 and 2415: glob_show_error() in src/tool_urlglob.c
            var ex = ParseFails("curl \"127.0.0.1:1/{<test>A,B}{<moo>C,D}\" -o \"somewhere/#<foo>\"");

            ex.CurlErrorCode.Should().Be(43);
            ex.CurlErrorMessage.Should().Be("no glob exists with this name in position 16:\nsomewhere/#<foo>\n               ^");
            ParseFails("curl 127.0.0.1:1/test -o \"somewhere/#<foo>\"").CurlErrorMessage
                .Should().Be("no glob exists with this name in position 16:\nsomewhere/#<foo>\n               ^");
        }

        [Fact]
        public void TransferEncoding_Unsolicited_IsCurlsText()
        {
            // curl test 2119
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nTransfer-Encoding: identityx\r\nConnection: close\r\n\r\nabc");

            Func<Task> run = () => Curl.ExecuteAsync($"curl -sS {server.Url("/te")}");

            var ex = run.Should().ThrowAsync<CurlBadContentEncodingException>().GetAwaiter().GetResult().Which;
            ex.CurlErrorCode.Should().Be(61);
            ex.CurlErrorMessage.Should().Be("Unsolicited Transfer-Encoding (identityx) found");
        }

        [Fact]
        public void TransferEncoding_MoreThanFiveDecoders_IsCurlsText_AcrossHeaderLines()
        {
            // curl test 418: nine "Transfer-Encoding: gzip" lines with --tr-encoding
            var headers = string.Concat(Enumerable.Repeat("Transfer-Encoding: gzip\r\n", 9));
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\n" + headers + "Connection: close\r\n\r\nabc");

            Func<Task> run = () => Curl.ExecuteAsync($"curl -sS --tr-encoding {server.Url("/te")}");

            run.Should().ThrowAsync<CurlBadContentEncodingException>().GetAwaiter().GetResult().Which
                .CurlErrorMessage.Should().Be("Reject response exceeding limit of 5 transfer encodings");
        }

        [Fact]
        public void TransferEncoding_GzipAfterChunked_IsCurlsText()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nTransfer-Encoding: chunked, gzip\r\nConnection: close\r\n\r\n0\r\n\r\n");

            Func<Task> run = () => Curl.ExecuteAsync($"curl -sS {server.Url("/te")}");

            run.Should().ThrowAsync<CurlBadContentEncodingException>().GetAwaiter().GetResult().Which
                .CurlErrorMessage.Should().Be("A Transfer-Encoding (gzip) was listed after chunked");
        }

        [Fact]
        public async Task WriteOutErrormsg_IsCurlErrorMessage()
        {
            // curl test 1188: %{errormsg} is curl's error buffer, not CurlDotNet's first message line
            using var server = new RawReplyServer("HTTP/1.1 404 Not Found\r\nContent-Length: 5\r\n\r\ngone\n");

            Func<Task> run = () => Curl.ExecuteAsync($"curl -f -s -w '%{{exitcode}} %{{errormsg}}' {server.Url("/missing")}");

            var ex = (await run.Should().ThrowAsync<CurlException>()).Which;
            ex.Data[CurlException.WriteOutDataKey].Should().Be("22 The requested URL returned error: 404");
        }

        [Fact]
        public async Task WriteOutHeaderJson_ClosingBraceIsOnItsOwnLine()
        {
            // curl test 421: tool_writeout_json.c headerJSON() ends with "\n}"
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nServer: nginx\r\nVary: a\r\nVary: b\r\nContent-Length: 0\r\n\r\n");

            var result = await Curl.ExecuteAsync($"curl -s -w '%{{header_json}}' {server.Url("/")}");

            result.Body.Should().Be("{\"server\":[\"nginx\"],\n\"vary\":[\"a\",\"b\"],\n\"content-length\":[\"0\"]\n}");
        }

        // ------------------------------------------------------------------ usage errors (exit 2, curl's helpf())

        [Theory]
        [InlineData("--no-clobber -C 1", "--continue-at is mutually exclusive with --no-clobber\noption -C: is badly used here")]           // curl test 481
        [InlineData("--remove-on-error -C 1", "--continue-at is mutually exclusive with --remove-on-error\noption -C: is badly used here")] // curl test 482
        [InlineData("-C 1 --no-clobber", "--continue-at is mutually exclusive with --no-clobber\noption --no-clobber: is badly used here")]
        [InlineData("-C 1 --remove-on-error", "--continue-at is mutually exclusive with --remove-on-error\noption --remove-on-error: is badly used here")]
        [InlineData("-r 0-5 -C 1", "--continue-at is mutually exclusive with --range\noption -C: is badly used here")]
        [InlineData("-C 1 -r 0-5", "--continue-at is mutually exclusive with --range\noption -r: is badly used here")]
        public void ResumeConflicts_AreUsageErrors_WithCurlsLines(string options, string expected)
        {
            var ex = ParseFails($"curl http://127.0.0.1:1/x {options} -o out.bin");

            ex.CurlErrorCode.Should().Be(2);
            ex.IsUsageError.Should().BeTrue();
            ex.CurlErrorMessage.Should().Be(expected);
            ex.WhatHappened.Should().StartWith(expected.Split('\n')[0], "the rich message still says what happened first");
        }

        [Fact]
        public void ResumeWithClobberToggledBackOn_IsAccepted()
        {
            _parser.Parse("curl http://127.0.0.1:1/x --no-clobber --clobber -C 1 -o out.bin").ResumeFrom.Should().Be(1);
            _parser.Parse("curl http://127.0.0.1:1/x -C 1 --no-remove-on-error -o out.bin").ResumeFrom.Should().Be(1);
        }

        [Theory]
        [InlineData("curl http://127.0.0.1:1/x -H", "option -H: requires parameter")]
        [InlineData("curl http://127.0.0.1:1/x --header", "option --header: requires parameter")]
        [InlineData("curl http://127.0.0.1:1/x -sH", "option -sH: requires parameter")]
        public void MissingParameter_IsUsageError_NamingTheWordAsTyped(string command, string expected)
        {
            var ex = ParseFails(command);

            ex.CurlErrorCode.Should().Be(2);
            ex.IsUsageError.Should().BeTrue();
            ex.CurlErrorMessage.Should().Be(expected);
        }

        [Fact]
        public void TransferErrors_AreNotUsageErrors()
        {
            ParseFails("curl http://127.0.0.1:1/x -d a -C 3").IsUsageError.Should().BeFalse();
        }

        // ------------------------------------------------------------------ warnings found while reading the command line

        [Theory]
        [InlineData("--fail-with-body --fail", "Warning: --fail deselects --fail-with-body here")] // curl test 360
        [InlineData("-f --fail-with-body", "Warning: --fail-with-body deselects --fail here")]
        [InlineData("-L --follow", "Warning: --follow overrides --location")]                  // curl test 794
        [InlineData("--follow --location", "Warning: --location overrides --follow")]
        [InlineData("-o a -o b", "Warning: Got more output options than URLs")]                 // curl test 371
        public void CommandLineWarnings_AreCollectedWithCurlsText(string options, string expected)
        {
            ParsedMessages($"curl http://127.0.0.1:1/x {options}").Should().Equal(expected);
        }

        [Theory]
        [InlineData("--fail --fail")]
        [InlineData("--fail-with-body --no-fail --fail")]
        [InlineData("-L -L")]
        [InlineData("-o a")]
        [InlineData("-o a --no-remote-name")] // curl test 1278: --no-remote-name takes no URL node without --remote-name-all
        public void CommandLinesWithoutAProblem_HaveNoWarnings(string options)
        {
            ParsedMessages($"curl http://127.0.0.1:1/x {options}").Should().BeEmpty();
        }

        [Fact]
        public void Warnings_RespectTheSilentStateAtThatPoint()
        {
            // curl's warnf() checks global->silent when it is called: -s before mutes, -s after does not (curl test 469)
            ParsedMessages("curl http://127.0.0.1:1/x -s --fail-with-body --fail").Should().BeEmpty();
            ParsedMessages("curl http://127.0.0.1:1/x --fail-with-body --fail -s").Should().Equal("Warning: --fail deselects --fail-with-body here");
            ParsedMessages("curl http://127.0.0.1:1/x -o a -o b -s").Should().BeEmpty("this warning is printed once the whole line is read");
            ParsedMessages("curl http://127.0.0.1:1/x -s --next --no-silent --fail-with-body --fail http://127.0.0.1:1/y")
                .Should().Equal(new[] { "Warning: --fail deselects --fail-with-body here" }, "-s is one global switch, turned off again by --no-silent");
        }

        [Fact]
        public void UnicodeArgument_IsWarnedAbout_ButSecretsAreNotEchoed()
        {
            // curl test 469: has_leading_unicode() in tool_getparam.c
            ParsedMessages("curl http://127.0.0.1:1/x -H '“host: x'")
                .Should().Equal("Warning: The argument '“host: x' starts with a Unicode character. Maybe ASCII was intended?");
            ParsedMessages("curl http://127.0.0.1:1/x -u '“user:pw'")
                .Should().Equal("Warning: The argument to --user starts with a Unicode character. Maybe ASCII was intended?");
            ParsedMessages("curl “http://127.0.0.1:1/x")
                .Should().Equal(new[] { "Warning: The argument '“http://127.0.0.1:1/x' starts with a Unicode character. Maybe ASCII was intended?" },
                    "parse_args() hands a bare URL to getparameter(\"--url\")");
        }

        [Fact]
        public void UnicodeCheck_SeesTheExpandedArgument()
        {
            // curl's varexpand() replaces the argument before has_leading_unicode() looks at it
            ParsedMessages("curl http://127.0.0.1:1/x --variable 'v=“x: y' --expand-header '{{v}}'")
                .Should().Equal("Warning: The argument '“x: y' starts with a Unicode character. Maybe ASCII was intended?");
        }

        [Fact]
        public void ConfigFile_ArgumentWarnings_NameFileLineAndOption()
        {
            // curl tests 459 and 1712: extract_param() in tool_parsecfg.c; comment and empty lines are not counted
            var config = FilePath("config");
            File.WriteAllText(config, "# comment\n\nurl = \"http://127.0.0.1:1/x\"\ndata = arg with space\n-H 'X: y'\ndata = \"quoted is fine\" # ok\n");

            ParsedMessages($"curl --config {Q(config)}").Should().Equal(
                $"Warning: {config}:2 Option 'data' uses argument with unquoted whitespace. This may cause side-effects. Consider double quotes.",
                $"Warning: {config}:3 Option '-H' uses argument with leading single quote. It is probably a mistake. Consider double quotes.",
                $"Warning: {config}:3 Option '-H' uses argument with unquoted whitespace. This may cause side-effects. Consider double quotes.");
        }

        [Fact]
        public void ConfigFile_UnicodeArgument_IsWarnedAbout()
        {
            // curl test 470
            var config = FilePath("input");
            File.WriteAllText(config, "-H “host:fake”\n");

            ParsedMessages($"curl http://127.0.0.1:1/x -K {Q(config)}")
                .Should().Equal("Warning: The argument '“host:fake”' starts with a Unicode character. Maybe ASCII was intended?");
        }

        [Fact]
        public void ParseErrors_CarryTheWarningsFoundBeforeThem()
        {
            var ex = ParseFails("curl http://127.0.0.1:1/x --fail-with-body --fail --no-clobber -C 1");

            ex.Warnings.Should().Equal("Warning: --fail deselects --fail-with-body here");
        }

        // ------------------------------------------------------------------ messages written when the command runs

        [Fact]
        public async Task Warnings_AreOnTheResult_AndInTheStderrFile_WrappedLikeCurl()
        {
            using var server = new RawReplyServer(Hello);
            var stderr = FilePath("stderr.txt");

            var result = await Curl.ExecuteAsync($"curl {server.Url("/w")} --fail-with-body --fail --stderr {Q(stderr)}");

            result.Warnings.Should().Equal("Warning: --fail deselects --fail-with-body here");
            File.ReadAllText(stderr).Should().Be("Warning: --fail deselects --fail-with-body here\n");
        }

        [Fact]
        public async Task UnnecessaryMethodNote_NeedsTracing_AndIsWrittenToTheStderrFile()
        {
            // curl test 1741: customrequest_helper() in tool_helpers.c, a notef() that needs -v/--trace and no -s
            using var server = new RawReplyServer(Hello, Hello, Hello);
            var stderr = FilePath("note.txt");

            var traced = await Curl.ExecuteAsync($"curl -v --stderr {Q(stderr)} -X POST -d foo {server.Url("/n")}");
            traced.Warnings.Should().Equal("Note: Unnecessary use of -X or --request, POST is already inferred.");
            File.ReadAllText(stderr).Should().Be("Note: Unnecessary use of -X or --request, POST is already inferred.\n");

            (await Curl.ExecuteAsync($"curl -X POST -d foo {server.Url("/n")}")).Warnings.Should().BeEmpty("no -v/--trace");
            (await Curl.ExecuteAsync($"curl -v -s --stderr {Q(stderr)} -X get {server.Url("/n")}")).Warnings.Should().BeEmpty("-s mutes notes");
        }

        [Fact]
        public async Task CustomHeadMethod_Warns()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 0\r\n\r\n");

            var result = await Curl.ExecuteAsync($"curl -X HEAD {server.Url("/h")}");

            result.Warnings.Should().Equal("Warning: Setting custom HTTP method to HEAD with -X/--request may not work the way you want. Consider using -I/--head instead.");
        }

        [Fact]
        public async Task SkipExisting_Note_NamesTheFile()
        {
            // curl tests 994 and 996: pre_transfer() in tool_operate.c
            using var server = new RawReplyServer(Hello);
            var existing = FilePath("there");
            File.WriteAllText(existing, "old");
            var stderr = FilePath("skip.txt");

            var result = await Curl.ExecuteAsync($"curl --trace-ascii {Q(FilePath("trace"))} --stderr {Q(stderr)} -o {Q(existing)} --skip-existing {server.Url("/s")}");

            result.Warnings.Should().Equal($"Note: skips transfer, \"{existing}\" exists locally");
            File.ReadAllText(stderr).Should().Be(ToolMessages.Format("Note: ", $"skips transfer, \"{existing}\" exists locally", ToolMessages.TerminalColumns()));
            server.Requests.Should().BeEmpty();
        }

        [Fact]
        public void Format_WrapsLikeCurlsVoutf()
        {
            // curl test 469 at COLUMNS=79: the cut is after the last blank that fits in 79 - 9 bytes (UTF-8)
            ToolMessages.Format("Warning: ", "The argument '“host:' starts with a Unicode character. Maybe ASCII was intended?", 79)
                .Should().Be("Warning: The argument '“host:' starts with a Unicode character. Maybe ASCII \nWarning: was intended?\n");
            ToolMessages.Format("Note: ", "short", 79).Should().Be("Note: short\n");
            ToolMessages.Format("Warning: ", new string('x', 30), 21).Should().Be("Warning: xxxxxxxxxxxx\nWarning: xxxxxxxxxxxx\nWarning: xxxxxx\n");
        }
    }
}
