using System;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Command-parsing errors say what was wrong (with the option, value, file or position), how to
    /// write it correctly, and link to the documentation; none of them leaks credentials.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class ParserMessageTests : IDisposable
    {
        private readonly CommandParser _parser = new CommandParser();
        private readonly string _dir;

        public ParserMessageTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "curldotnet-parsermsg-" + Guid.NewGuid().ToString("N"));
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

        private string P(string name) => Path.Combine(_dir, name);

        private static string Q(string path) => CommandParser.ShellQuote(path);

        private T Fails<T>(string command, CurlFlavor flavor = CurlFlavor.Posix) where T : CurlException
        {
            Action act = () => _parser.Parse(command, flavor);
            var ex = act.Should().Throw<T>().Which;
            ExceptionMessageTests.AssertWhatFixLink(ex, $"for {command}");
            return ex;
        }

        // ------------------------------------------------------------------ options

        [Fact]
        public void MissingValue_SaysCurlsWordingAndShowsAnExampleValue()
        {
            var ex = Fails<CurlException>("curl https://example.com -H");
            ex.CurlErrorCode.Should().Be(2);
            ex.WhatHappened.Should().StartWith("option -H: requires parameter");
            ex.Suggestions[0].Should().Contain("-H 'Accept: application/json'");
        }

        [Fact]
        public void MissingValue_LongOption_UsesTheNameAsTyped()
        {
            var ex = Fails<CurlException>("curl https://example.com --max-time");
            ex.WhatHappened.Should().StartWith("option --max-time: requires parameter");
            ex.Suggestions[0].Should().Contain("--max-time 30");
        }

        [Fact]
        public void TwoRequestMethods_NamesBothOptions()
        {
            var ex = Fails<CurlException>("curl -d x=1 -I https://example.com");
            ex.CurlErrorCode.Should().Be(2);
            ex.WhatHappened.Should().Contain("You can only select one HTTP request method").And.Contain("POST").And.Contain("HEAD");
        }

        [Fact]
        public void Resolve_NonNumericPort_NamesThePort()
        {
            var ex = Fails<CurlOptionSyntaxException>("curl --resolve example.com:http:127.0.0.1 https://example.com");
            ex.CurlErrorCode.Should().Be(49);
            ex.WhatHappened.Should().Contain("\"http\"").And.Contain("not a number");
        }

        [Fact]
        public void Resolve_TooFewParts_SaysHowManyPartsItHas()
        {
            var ex = Fails<CurlOptionSyntaxException>("curl --resolve example.com:443 https://example.com");
            ex.WhatHappened.Should().Contain("2 part(s)");
        }

        [Fact]
        public void ConnectTo_Malformed_ShowsTheValueAndAnExample()
        {
            var ex = Fails<CurlOptionSyntaxException>("curl --connect-to a:b:c:d https://example.com");
            ex.WhatHappened.Should().Contain("a:b:c:d");
            ex.Suggestions[0].Should().Contain("--connect-to example.com:443:127.0.0.1:8443");
        }

        [Fact]
        public void MaxFileSize_NotASize_ShowsTheValue()
        {
            var ex = Fails<CurlException>("curl --max-filesize lots https://example.com");
            ex.CurlErrorCode.Should().Be(2);
            ex.WhatHappened.Should().Contain("\"lots\"");
        }

        [Fact]
        public void NoUrl_DoesNotLeakTheUserPassword()
        {
            var ex = Fails<CurlInvalidCommandException>("curl -s -u alice:s3cretPw -H 'Authorization: Bearer tok123'");
            ex.WhatHappened.Should().Contain("No URL specified");
            ex.Message.Should().NotContain("s3cretPw").And.NotContain("tok123");
        }

        [Fact]
        public void OnlyTheProgramName_IsNoUrl()
        {
            var ex = Fails<CurlException>("curl");
            ex.CurlErrorCode.Should().Be(2, "curl exits with 2 when no URL is given");
        }

        [Fact]
        public void EmptyCommand_IsArgumentNullExceptionThatExplainsAndLinks()
        {
            Action act = () => _parser.Parse("   ", CurlFlavor.Auto);
            var ex = act.Should().Throw<ArgumentNullException>().Which;
            ex.Message.Should().Contain("null").And.Contain("curl https://example.com").And.Contain("See https://github.com/jacob-mellor/curl-dot-net/");
        }

        [Fact]
        public void UnknownFlavorValue_ExplainsTheValidValues()
        {
            Action act = () => _parser.Parse("curl https://example.com", (CurlFlavor)42);
            act.Should().Throw<ArgumentOutOfRangeException>().Which.Message
                .Should().Contain("(CurlFlavor)42").And.Contain("CurlFlavor.Auto").And.Contain("See https://github.com/jacob-mellor/curl-dot-net/");
        }

        [Fact]
        public void UnknownOption_IsKeptForValidateAndDoesNotThrowAtParse()
        {
            // curl exits with 2 here; CurlDotNet deliberately tolerates it at parse time and reports it from Curl.Validate().
            var options = _parser.Parse("curl --no-such-option https://example.com", CurlFlavor.Posix);
            options.UnknownOptions.Should().Contain("--no-such-option");
        }

        // ------------------------------------------------------------------ globs

        [Theory]
        [InlineData("curl 'https://example.com/{a,b'", "unmatched brace in URL position 21")]
        [InlineData("curl 'https://example.com/a}'", "unmatched close brace '}' in URL position 22")]
        [InlineData("curl 'https://example.com/[1-5'", "unmatched bracket in URL position 21")]
        [InlineData("curl 'https://example.com/{a,{b}}'", "nested brace in URL position 24")]
        [InlineData("curl 'https://example.com/[5-1]'", "bad range in URL position 21")]
        [InlineData("curl 'https://example.com/[z-a]'", "letter range [z-a] runs backwards")]
        [InlineData("curl 'https://example.com/[1-9:0]'", "the step in [1-9:0]")]
        [InlineData("curl 'https://example.com/[1]'", "[1] is not a start-end range")]
        public void Glob_Errors_NameThePositionAndProblem(string command, string expected)
        {
            var ex = Fails<CurlException>(command);
            ex.CurlErrorCode.Should().Be(3);
            ex.WhatHappened.Should().Contain(expected);
            ex.DocumentationUrl.Should().EndWith("url-errors.md");
        }

        // ------------------------------------------------------------------ variables and config files

        [Fact]
        public void Variable_MissingEnvironmentVariable_NamesIt()
        {
            var ex = Fails<CurlException>("curl --variable %CURLDOTNET_NO_SUCH_VARIABLE_42 https://example.com");
            ex.CurlErrorCode.Should().Be(2);
            ex.WhatHappened.Should().Contain("CURLDOTNET_NO_SUCH_VARIABLE_42");
            ex.Suggestions[0].Should().Contain("--variable %CURLDOTNET_NO_SUCH_VARIABLE_42=default");
        }

        [Fact]
        public void Variable_UnknownFunction_ListsTheValidOnes()
        {
            var ex = Fails<CurlException>("curl --variable a=1 --expand-url 'https://example.com/{{a:nope}}'");
            ex.WhatHappened.Should().Contain("\"nope\"");
            ex.Suggestions[0].Should().Contain("trim, json, url, b64 or 64dec");
        }

        [Fact]
        public void Config_IncludingItself_SaysWhichFile()
        {
            var config = P("loop.curlrc");
            File.WriteAllText(config, $"config = \"{config}\"\n");

            var ex = Fails<CurlException>($"curl -K {Q(config)} https://example.com");
            ex.CurlErrorCode.Should().Be(2);
            ex.WhatHappened.Should().Contain("loop.curlrc").And.Contain("nested more than");
        }

        // ------------------------------------------------------------------ files named on the command line

        [Fact]
        public void ConfigFile_Missing_NamesTheFileTheOptionAndTheCurrentDirectory()
        {
            var missing = P("nope.curlrc");
            var ex = Fails<CurlReadErrorException>($"curl -K {Q(missing)} https://example.com");
            ex.CurlErrorCode.Should().Be(26);
            ex.WhatHappened.Should().Contain("-K/--config").And.Contain("nope.curlrc").And.Contain("does not exist");
            ex.Suggestions[0].Should().Contain(Directory.GetCurrentDirectory());
        }

        [Fact]
        public void DataFromStandardInput_ExplainsWhyItCannotWork()
        {
            var ex = Fails<CurlReadErrorException>("curl -d @- https://example.com");
            ex.WhatHappened.Should().Contain("standard input");
        }

        [Fact]
        public void UploadFromStandardInput_ExplainsWhyItCannotWork()
        {
            var ex = Fails<CurlReadErrorException>("curl -T - https://example.com/up");
            ex.WhatHappened.Should().Contain("-T -").And.Contain("standard input");
            ex.Suggestions[0].Should().Contain("-T path/to/file");
        }

        [Fact]
        public void ResumeWithUpload_SaysWhichUrlAndOptions()
        {
            var body = P("body.txt");
            File.WriteAllText(body, "x");
            var ex = Fails<CurlBadFunctionArgumentException>($"curl -C 100 -T {Q(body)} https://example.com/up");
            ex.CurlErrorCode.Should().Be(43);
            ex.WhatHappened.Should().Contain("-C/--continue-at 100").And.Contain("https://example.com/up");
        }

        [Fact]
        public void ResumeWithNoClobber_ExplainsTheClash()
        {
            var ex = Fails<CurlException>("curl -C - --no-clobber -o out.bin https://example.com/file");
            ex.CurlErrorCode.Should().Be(2);
            ex.WhatHappened.Should().Contain("--continue-at is mutually exclusive with --no-clobber");
        }

        // ------------------------------------------------------------------ shells

        [Fact]
        public void Posix_UnclosedQuote_NamesTheFlavorAndWhere()
        {
            var ex = Fails<CurlInvalidCommandException>("curl 'https://example.com/a", CurlFlavor.Posix);
            ex.WhatHappened.Should().Contain("single quote is never closed").And.Contain("CurlFlavor.Posix").And.Contain("https://example.com/a");
            ex.DocumentationUrl.Should().EndWith("curl-flavors-bash-cmd-powershell/README.md");
        }

        [Fact]
        public void Posix_AnsiCEscapeOutOfRange_DoesNotCrash()
        {
            var options = _parser.Parse("curl $'https://example.com/\\UFFFFFFFF'", CurlFlavor.Posix);
            options.Url.Should().StartWith("https://example.com/");
        }

        [Fact]
        public void InvokeWebRequest_BodyAndInFile_SaysWhatToDo()
        {
            var ex = Fails<CurlInvalidCommandException>("Invoke-WebRequest -Uri https://example.com -Method Post -Body 'a' -InFile b.txt", CurlFlavor.InvokeWebRequest);
            ex.WhatHappened.Should().Contain("-Body and -InFile are both given").And.Contain("Invoke-WebRequest syntax");
            ex.Suggestions[0].Should().Contain("not both");
        }

        [Fact]
        public void InvokeWebRequest_UnclosedHashtable_ShowsWhere()
        {
            var ex = Fails<CurlInvalidCommandException>("Invoke-WebRequest -Uri https://example.com -Headers @{ 'A' = 'b'", CurlFlavor.InvokeWebRequest);
            ex.WhatHappened.Should().Contain("never closed").And.Contain("near \"@{");
        }

        // ------------------------------------------------------------------ request building

        [Fact]
        public void HeaderWithLineBreak_HidesSecretValues()
        {
            Action act = () => RequestRewrite.RejectLineBreaks("Authorization: Bearer tok123\r\nX-Evil: 1", "-H/--header", "curl -H ...");
            var ex = act.Should().Throw<CurlBadFunctionArgumentException>().Which;
            ExceptionMessageTests.AssertWhatFixLink(ex);
            ex.Message.Should().NotContain("tok123").And.Contain("Authorization: (value hidden)");
            ex.Suggestions[0].Should().Contain("-H 'Accept: application/json'");
        }

        [Fact]
        public async Task CookieJar_InMissingFolder_NamesTheFolder()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 2\r\nSet-Cookie: a=1\r\n\r\nok");
            var jar = Path.Combine(_dir, "no-such-folder", "jar.txt");

            Func<Task> act = () => Curl.ExecuteAsync($"curl -c {Q(jar)} {server.Url()}");

            var ex = (await act.Should().ThrowAsync<CurlWriteErrorException>()).Which;
            ExceptionMessageTests.AssertWhatFixLink(ex);
            ex.WhatHappened.Should().Contain("no-such-folder").And.Contain("does not exist");
        }

        [Fact]
        public async Task FormFile_Missing_NamesTheFieldAndFile()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 2\r\n\r\nok");
            var missing = P("photo.jpg");

            Func<Task> act = () => Curl.ExecuteAsync($"curl -F {Q("avatar=@" + missing)} {server.Url()}");

            var ex = (await act.Should().ThrowAsync<CurlReadErrorException>()).Which;
            ExceptionMessageTests.AssertWhatFixLink(ex);
            ex.WhatHappened.Should().Contain("avatar=@").And.Contain("photo.jpg").And.Contain("does not exist");
            ex.Suggestions[0].Should().Contain("--form-string");
        }

        [Fact]
        public async Task Netrc_UnclosedQuote_NamesTheLineButNotThePassword()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 2\r\n\r\nok");
            var netrc = P("netrc");
            File.WriteAllText(netrc, "# credentials\nmachine 127.0.0.1 login alice password \"pw-12345\n");

            Func<Task> act = () => Curl.ExecuteAsync($"curl --netrc-file {Q(netrc)} {server.Url()}");

            var ex = (await act.Should().ThrowAsync<CurlReadErrorException>()).Which;
            ExceptionMessageTests.AssertWhatFixLink(ex);
            ex.WhatHappened.Should().Contain("line 2");
            ex.Message.Should().NotContain("pw-12345");
        }
    }
}
