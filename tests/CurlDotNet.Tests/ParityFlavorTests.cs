using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Xunit;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// <see cref="CurlFlavor"/>: the quoting, escaping, line-continuation and variable rules of bash, cmd.exe,
    /// PowerShell (curl.exe) and Invoke-WebRequest, Auto detection, and the public API that selects a flavor.
    /// Requests go to an in-process TCP server, never to the internet.
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class ParityFlavorTests : IDisposable
    {
        private const string Docs = "https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md";
        private const string Ok = "HTTP/1.1 200 OK\r\nContent-Length: 2\r\nContent-Type: text/plain\r\n\r\nok";

        private readonly CommandParser _parser = new CommandParser();
        private readonly string _dir;
        private readonly List<string> _variables = new List<string>();

        public ParityFlavorTests()
        {
            _dir = Path.Combine(Path.GetTempPath(), "curldotnet-flavor-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);
        }

        public void Dispose()
        {
            foreach (var name in _variables)
            {
                Environment.SetEnvironmentVariable(name, null);
            }

            try
            {
                Directory.Delete(_dir, recursive: true);
            }
            catch (IOException)
            {
            }
        }

        /// <summary>Sets a uniquely named environment variable for this test and returns its name.</summary>
        private string SetVariable(string value)
        {
            var name = "CDN_FLAVOR_" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpperInvariant();
            Environment.SetEnvironmentVariable(name, value);
            _variables.Add(name);
            return name;
        }

        private static string UnsetVariable() => "CDN_UNSET_" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpperInvariant();

        private CurlOptions Parse(string command, CurlFlavor flavor) => _parser.Parse(command, flavor);

        #region Posix

        [Fact]
        public void Posix_SingleQuotes_AreLiteral_BackslashQuoteStaysTwoCharacters()
        {
            var options = Parse(@"curl -d '{\""a\"":1}' https://x.example", CurlFlavor.Posix);

            options.Data.Should().Be(@"{\""a\"":1}");
            options.Flavor.Should().Be(CurlFlavor.Posix);
        }

        [Fact]
        public void Posix_SingleQuoteInsideSingleQuotes_UsesTheCloseEscapeReopenIdiom()
        {
            Parse(@"curl -d 'it'\''s' https://x.example", CurlFlavor.Posix).Data.Should().Be("it's");
        }

        [Fact]
        public void Posix_DoubleQuotes_EscapeOnlyQuoteBackslashDollarAndBacktick()
        {
            var options = Parse("curl -d \"a\\\"b\\\\c\\$d\\`e\\nf\" https://x.example", CurlFlavor.Posix);

            options.Data.Should().Be("a\"b\\c$d`e\\nf");
        }

        [Fact]
        public void Posix_BackslashOutsideQuotes_EscapesAnyCharacter()
        {
            var options = Parse(@"curl -H X-Path:a\ b\c https://x.example", CurlFlavor.Posix);

            options.Headers["X-Path"].Should().Be("a bc");
        }

        [Fact]
        public void Posix_BackslashNewline_ContinuesTheLine()
        {
            var options = Parse("curl -X POST \\\n  -d 'x=1' \\\r\n  https://x.example/api", CurlFlavor.Posix);

            options.Method.Should().Be("POST");
            options.Data.Should().Be("x=1");
            options.Url.Should().Be("https://x.example/api");
        }

        [Fact]
        public void Posix_DollarVariables_ExpandOutsideSingleQuotesOnly()
        {
            var name = SetVariable("tok123");

            var options = Parse($"curl -H \"Authorization: Bearer ${name}\" -H 'X-Raw: ${name}' -H X-Brace:${{{name}}} https://x.example", CurlFlavor.Posix);

            options.Headers["Authorization"].Should().Be("Bearer tok123");
            options.Headers["X-Raw"].Should().Be("$" + name);
            options.Headers["X-Brace"].Should().Be("tok123");
        }

        [Fact]
        public void Posix_UnsetVariable_IsLeftAsTyped()
        {
            var name = UnsetVariable();

            Parse($"curl -H \"X-Token: ${name}\" https://x.example", CurlFlavor.Posix).Headers["X-Token"].Should().Be("$" + name);
        }

        [Fact]
        public void Posix_PercentAndEnvColonSyntax_AreOrdinaryText()
        {
            var name = SetVariable("value");

            var options = Parse($"curl -H \"X-Cmd: %{name}%\" -H \"X-Ps: $env:{name}\" https://x.example", CurlFlavor.Posix);

            options.Headers["X-Cmd"].Should().Be("%" + name + "%");
            options.Headers["X-Ps"].Should().Be("$env:" + name);
        }

        [Fact]
        public void Posix_AnsiCQuotes_DecodeEscapes()
        {
            var options = Parse(@"curl --data-raw $'{""a"":""it\'s\n\x41\u00e9\101""}' https://x.example", CurlFlavor.Posix);

            options.Data.Should().Be("{\"a\":\"it's\nA\u00e9A\"}");
        }

        [Fact]
        public void Posix_HashAtWordStart_StartsAComment()
        {
            var options = Parse("curl https://x.example/#frag # a comment -X DELETE\n -H 'A: 1'", CurlFlavor.Posix);

            options.Url.Should().Be("https://x.example/#frag");
            options.Method.Should().NotBe("DELETE");
            options.Headers["A"].Should().Be("1");
        }

        [Fact]
        public void Posix_UnterminatedQuote_ThrowsWithFixAndDocsLink()
        {
            Action act = () => Parse("curl -d '{\"a\":1} https://x.example", CurlFlavor.Posix);

            act.Should().Throw<CurlInvalidCommandException>()
                .Where(e => e.Message.Contains("single quote is never closed") && e.Message.Contains("as Posix") && e.Message.Contains(Docs));
        }

        [Fact]
        public void Posix_UnterminatedDoubleQuote_HasItsOwnMessage()
        {
            Action act = () => Parse("curl -d \"abc https://x.example", CurlFlavor.Posix);

            act.Should().Throw<CurlInvalidCommandException>()
                .Where(e => e.Message.Contains("double quote is never closed") && e.Message.Contains(Docs));
        }

        [Fact]
        public void Posix_CaretIsNotAContinuation()
        {
            var options = Parse("curl -H 'X: a^b' https://x.example", CurlFlavor.Posix);

            options.Headers["X"].Should().Be("a^b");
        }

        #endregion

        #region WindowsCmd

        [Fact]
        public void WindowsCmd_BackslashQuoteInsideDoubleQuotes_IsALiteralQuote()
        {
            var options = Parse(@"curl -H ""Content-Type: application/json"" -d ""{\""a\"":1}"" https://x.example", CurlFlavor.WindowsCmd);

            options.Data.Should().Be("{\"a\":1}");
            options.Headers["Content-Type"].Should().Be("application/json");
            options.Flavor.Should().Be(CurlFlavor.WindowsCmd);
        }

        [Fact]
        public void WindowsCmd_DoubledQuoteInsideQuotes_IsALiteralQuote()
        {
            Parse(@"curl -d ""{""""a"""":1}"" https://x.example", CurlFlavor.WindowsCmd).Data.Should().Be("{\"a\":1}");
        }

        [Fact]
        public void WindowsCmd_SingleQuotes_AreOrdinaryCharacters()
        {
            var options = Parse(@"curl -d '{""a"":1}' https://x.example", CurlFlavor.WindowsCmd);

            options.Data.Should().Be("'{a:1}'");
        }

        [Fact]
        public void WindowsCmd_CaretContinuesLinesAndEscapesOutsideQuotes()
        {
            var options = Parse("curl ^\r\n  -X POST ^\n  \"https://x.example/a?b=1&c=2\" ^\n  -H X-A:1^&2 -H \"X-B: 1^&2\"", CurlFlavor.WindowsCmd);

            options.Method.Should().Be("POST");
            options.Url.Should().Be("https://x.example/a?b=1&c=2");
            options.Headers["X-A"].Should().Be("1&2");
            options.Headers["X-B"].Should().Be("1^&2");
        }

        [Fact]
        public void WindowsCmd_PercentVariables_ExpandInsideAndOutsideQuotes_UnsetStaysAsTyped()
        {
            var name = SetVariable("cmd-secret");
            var unset = UnsetVariable();

            var options = Parse($"curl -H \"Authorization: Bearer %{name}%\" -H X-Bare:%{name}% -H \"X-Unset: %{unset}%\" https://x.example/a%20b", CurlFlavor.WindowsCmd);

            options.Headers["Authorization"].Should().Be("Bearer cmd-secret");
            options.Headers["X-Bare"].Should().Be("cmd-secret");
            options.Headers["X-Unset"].Should().Be("%" + unset + "%");
            options.Url.Should().Be("https://x.example/a%20b");
        }

        [Fact]
        public void WindowsCmd_DollarVariables_DoNotExpand()
        {
            var name = SetVariable("value");

            Parse($"curl -H \"X: ${name}\" https://x.example", CurlFlavor.WindowsCmd).Headers["X"].Should().Be("$" + name);
        }

        [Fact]
        public void WindowsCmd_Backslash_IsAPathSeparator()
        {
            var options = Parse(@"curl -o C:\temp\out\a.txt https://x.example", CurlFlavor.WindowsCmd);

            options.OutputFile.Should().Be(@"C:\temp\out\a.txt");
        }

        [Fact]
        public void WindowsCmd_BrowserCopyAsCurlCmd_IsReadExactly()
        {
            const string command = "curl ^\"https://x.example/api?a=1^&b=2^\" ^\r\n" +
                                   "  -H ^\"accept: */*^\" ^\r\n" +
                                   "  --data-raw ^\"^{^\\^\"a^\\^\":1^}^\"";

            var options = Parse(command, CurlFlavor.WindowsCmd);

            options.Url.Should().Be("https://x.example/api?a=1&b=2");
            options.Headers["accept"].Should().Be("*/*");
            options.Data.Should().Be("{\"a\":1}");
        }

        #endregion

        #region PowerShell (curl.exe)

        [Fact]
        public void PowerShell_BackslashQuoteInSingleQuotes_BecomesAQuote()
        {
            var options = Parse(@"curl.exe -H 'Content-Type: application/json' -d '{\""a\"":1}' https://x.example", CurlFlavor.PowerShell);

            options.Data.Should().Be("{\"a\":1}");
            options.Flavor.Should().Be(CurlFlavor.PowerShell);
        }

        [Fact]
        public void PowerShell_BareQuotesInSingleQuotes_AreKept()
        {
            Parse(@"curl.exe -d '{""a"":1}' https://x.example", CurlFlavor.PowerShell).Data.Should().Be("{\"a\":1}");
        }

        [Fact]
        public void PowerShell_TripleBackslashQuote_LeavesOneBackslash()
        {
            Parse(@"curl.exe -d '{\""p\"":\""C:\\\""}' https://x.example", CurlFlavor.PowerShell).Data.Should().Be("{\"p\":\"C:\\\"}");
        }

        [Fact]
        public void PowerShell_DoubledSingleQuote_IsOneQuote()
        {
            Parse("curl.exe -d 'it''s' https://x.example", CurlFlavor.PowerShell).Data.Should().Be("it's");
        }

        [Fact]
        public void PowerShell_DoubleQuotes_BacktickEscapesAndDoubledQuote()
        {
            var options = Parse("curl.exe -d \"{`\"a`\":`$1,\"\"b\"\":2}`t``\" https://x.example", CurlFlavor.PowerShell);

            options.Data.Should().Be("{\"a\":$1,\"b\":2}\t`");
        }

        [Fact]
        public void PowerShell_BacktickN_IsANewline()
        {
            Parse("curl.exe -d \"a`nb\" https://x.example", CurlFlavor.PowerShell).Data.Should().Be("a\nb");
        }

        [Fact]
        public void PowerShell_EnvVariables_ExpandInDoubleQuotesAndBareWords_NotInSingleQuotes()
        {
            var name = SetVariable("ps-secret");

            var options = Parse($"curl.exe -H \"Authorization: Bearer $env:{name}\" -H X-Bare:$env:{name} -H 'X-Literal: $env:{name}' https://x.example", CurlFlavor.PowerShell);

            options.Headers["Authorization"].Should().Be("Bearer ps-secret");
            options.Headers["X-Bare"].Should().Be("ps-secret");
            options.Headers["X-Literal"].Should().Be("$env:" + name);
        }

        [Fact]
        public void PowerShell_PercentVariables_DoNotExpand()
        {
            var name = SetVariable("value");

            Parse($"curl.exe -H \"X: %{name}%\" https://x.example", CurlFlavor.PowerShell).Headers["X"].Should().Be("%" + name + "%");
        }

        [Fact]
        public void PowerShell_BacktickNewline_ContinuesTheLine()
        {
            var options = Parse("curl.exe -X PUT `\r\n  -d 'v=1' `\n  https://x.example/item", CurlFlavor.PowerShell);

            options.Method.Should().Be("PUT");
            options.Data.Should().Be("v=1");
            options.Url.Should().Be("https://x.example/item");
        }

        [Fact]
        public void PowerShell_StopParsing_PassesTheRestToTheWindowsArgumentRules()
        {
            var name = SetVariable("from-cmd-syntax");

            var options = Parse($"curl.exe -H \"X-Before: %{name}%\" --% -d \"{{\\\"a\\\":1}}\" -H \"X-After: %{name}%\" https://x.example", CurlFlavor.PowerShell);

            options.Data.Should().Be("{\"a\":1}");
            options.Headers["X-Before"].Should().Be("%" + name + "%");
            options.Headers["X-After"].Should().Be("from-cmd-syntax");
        }

        [Fact]
        public void PowerShell_StopParsing_OnlyCoversItsLine()
        {
            Action act = () => Parse("curl.exe --% -d x https://x.example\n-H 'A: 1'", CurlFlavor.PowerShell);

            act.Should().Throw<CurlInvalidCommandException>()
                .Where(e => e.Message.Contains("--% only covers the rest of its own line") && e.Message.Contains(Docs));
        }

        [Fact]
        public void PowerShell_CallOperatorAssignmentAndPipelineTail_AreRemoved()
        {
            var options = Parse("$r = & 'C:\\Program Files\\curl\\curl.exe' -s https://x.example/data | ConvertFrom-Json", CurlFlavor.PowerShell);

            options.Url.Should().Be("https://x.example/data");
            options.Silent.Should().BeTrue();
        }

        [Fact]
        public void PowerShell_HereString_IsOneArgument()
        {
            var options = Parse("curl.exe -d @'\n{\"a\": 1,\n \"b\": \"$x\"}\n'@ https://x.example", CurlFlavor.PowerShell);

            options.Data.Should().Be("{\"a\": 1,\n \"b\": \"$x\"}");
            options.Url.Should().Be("https://x.example");
        }

        [Fact]
        public void PowerShell_UnterminatedQuote_ThrowsPowerShellSpecificFix()
        {
            Action act = () => Parse("curl.exe -d \"abc https://x.example", CurlFlavor.PowerShell);

            act.Should().Throw<CurlInvalidCommandException>()
                .Where(e => e.Message.Contains("as PowerShell") && e.Message.Contains("`\"") && e.Message.Contains(Docs));
        }

        [Fact]
        public void PowerShell_PlainCurl_IsTreatedAsCurlExe()
        {
            Parse("curl -s https://x.example", CurlFlavor.PowerShell).Url.Should().Be("https://x.example");
        }

        #endregion

        #region InvokeWebRequest

        [Fact]
        public void InvokeWebRequest_JsonPost_TranslatesToCurlOptions()
        {
            var options = Parse("Invoke-RestMethod -Uri 'https://x.example/api' -Method Post -ContentType 'application/json' -Body '{\"a\":1}'", CurlFlavor.InvokeWebRequest);

            options.Url.Should().Be("https://x.example/api");
            options.Method.Should().Be("POST");
            options.Headers["Content-Type"].Should().Be("application/json");
            options.Data.Should().Be("{\"a\":1}");
            options.FollowLocation.Should().BeTrue();
            options.MaxRedirects.Should().Be(5);
            options.GlobOff.Should().BeTrue();
            options.Flavor.Should().Be(CurlFlavor.InvokeWebRequest);
        }

        [Fact]
        public void InvokeWebRequest_HeadersHashtable_EachEntryIsAHeader()
        {
            var name = SetVariable("iwr-token");
            var command = "iwr https://x.example -Headers @{\n  'Accept' = 'application/json'\n  \"X-Token\" = \"Bearer $env:" + name + "\"; Empty = ''; X-Bool = $true }";

            var options = Parse(command, CurlFlavor.InvokeWebRequest);

            options.Url.Should().Be("https://x.example");
            options.Headers["Accept"].Should().Be("application/json");
            options.Headers["X-Token"].Should().Be("Bearer iwr-token");
            options.Headers["X-Bool"].Should().Be("True");
            options.EmptyHeaders.Should().Contain("Empty");
        }

        [Fact]
        public void InvokeWebRequest_OptionsMapToCurlOptions()
        {
            var options = Parse("Invoke-WebRequest -UseBasicParsing -DisableKeepAlive https://x.example/f.zip -OutFile 'C:\\tmp\\f.zip' " +
                                "-UserAgent 'MyAgent/1.0' -TimeoutSec 30 -SkipCertificateCheck -Proxy http://proxy.example:8080 " +
                                "-MaximumRetryCount 3 -RetryIntervalSec 2", CurlFlavor.InvokeWebRequest);

            options.Url.Should().Be("https://x.example/f.zip");
            options.OutputFile.Should().Be("C:\\tmp\\f.zip");
            options.UserAgent.Should().Be("MyAgent/1.0");
            options.MaxTime.Should().Be(30);
            options.Insecure.Should().BeTrue();
            options.Proxy.Should().Be("http://proxy.example:8080");
            options.Retry.Should().Be(3);
            options.Method.Should().NotBe("POST");
        }

        [Fact]
        public void InvokeWebRequest_MaximumRedirectionZero_DoesNotFollow()
        {
            Parse("iwr https://x.example -MaximumRedirection 0", CurlFlavor.InvokeWebRequest).FollowLocation.Should().BeFalse();
        }

        [Fact]
        public void InvokeWebRequest_BodyWithoutMethod_StaysGet()
        {
            var options = Parse("iwr https://x.example -Body 'a=1'", CurlFlavor.InvokeWebRequest);

            options.Method.Should().Be("GET");
            options.Data.Should().Be("a=1");
        }

        [Fact]
        public void InvokeWebRequest_HashtableBody_IsQueryForGetAndFormForPost()
        {
            var get = Parse("irm https://x.example/s?x=0 -Body @{ q = 'a b'; n = 1 }", CurlFlavor.InvokeWebRequest);
            var post = Parse("irm https://x.example/s -Method Post -Body @{ q = 'a&b'; n = 1 }", CurlFlavor.InvokeWebRequest);

            get.Url.Should().Be("https://x.example/s?x=0&q=a+b&n=1");
            get.Data.Should().BeNull();
            post.Method.Should().Be("POST");
            post.Data.Should().Be("q=a%26b&n=1");
        }

        [Fact]
        public void InvokeWebRequest_BearerToken_FromStringOrSecureString()
        {
            var plain = Parse("iwr https://x.example -Authentication Bearer -Token 'abc'", CurlFlavor.InvokeWebRequest);
            var secure = Parse("iwr https://x.example -Authentication OAuth -Token (ConvertTo-SecureString 'xyz' -AsPlainText -Force)", CurlFlavor.InvokeWebRequest);

            plain.Headers["Authorization"].Should().Be("Bearer abc");
            secure.Headers["Authorization"].Should().Be("Bearer xyz");
        }

        [Fact]
        public void InvokeWebRequest_AliasesPrefixesAndCase_AreAccepted()
        {
            foreach (var cmdlet in new[] { "Invoke-WebRequest", "invoke-restmethod", "iwr", "IRM", "curl", "wget", "Microsoft.PowerShell.Utility\\Invoke-WebRequest" })
            {
                var options = Parse(cmdlet + " -uri https://x.example -meth put -cont 'text/plain' -bo 'x'", CurlFlavor.InvokeWebRequest);
                options.Url.Should().Be("https://x.example", cmdlet);
                options.Method.Should().Be("PUT", cmdlet);
                options.Headers["Content-Type"].Should().Be("text/plain", cmdlet);
            }
        }

        [Fact]
        public void InvokeWebRequest_ColonValueAndSwitchFalse()
        {
            var options = Parse("iwr -Uri:https://x.example -TimeoutSec:5 -SkipCertificateCheck:$false", CurlFlavor.InvokeWebRequest);

            options.Url.Should().Be("https://x.example");
            options.MaxTime.Should().Be(5);
            options.Insecure.Should().BeFalse();
        }

        [Fact]
        public void InvokeWebRequest_Credential_IsRefusedWithCurlAlternative()
        {
            Action act = () => Parse("iwr https://x.example -Credential (Get-Credential)", CurlFlavor.InvokeWebRequest);

            act.Should().Throw<CurlInvalidCommandException>()
                .Where(e => e.Message.Contains("PSCredential") && e.Message.Contains("-u user:pass") && e.Message.Contains(Docs) && e.InvalidPart == "-Credential");
        }

        [Fact]
        public void InvokeWebRequest_SessionVariable_IsRefusedWithCookieJarAlternative()
        {
            Action act = () => Parse("iwr https://x.example -SessionVariable s", CurlFlavor.InvokeWebRequest);

            act.Should().Throw<CurlInvalidCommandException>()
                .Where(e => e.Message.Contains("WebRequestSession") && e.Message.Contains("-c cookies.txt") && e.Message.Contains(Docs));
        }

        [Fact]
        public void InvokeWebRequest_DifferentProblems_GetDifferentMessages()
        {
            var messages = new[]
            {
                "iwr https://x.example -Bogus 1",
                "iwr https://x.example -Max 1",
                "iwr https://x.example -Headers $h",
                "iwr https://x.example -Body (@{ a = 1 } | ConvertTo-Json) -Method Post",
                "iwr -Method Get",
                "iwr https://x.example -Method Fetch",
                "iwr https://x.example -Authentication Basic",
                "iwr https://x.example -UseDefaultCredentials",
                "iwr https://x.example -TimeoutSec soon",
                "iwr https://x.example https://y.example",
                "iwr https://x.example -Headers @{ 'A' 'b' }",
            }.Select(command =>
            {
                try
                {
                    Parse(command, CurlFlavor.InvokeWebRequest);
                    return "no exception for " + command;
                }
                catch (CurlInvalidCommandException e)
                {
                    e.Message.Should().Contain(Docs, command);
                    return e.Message;
                }
            }).ToList();

            messages.Should().OnlyHaveUniqueItems();
            messages.Should().NotContain(m => m.StartsWith("no exception", StringComparison.Ordinal));
            messages[0].Should().Contain("-Bogus");
            messages[1].Should().Contain("ambiguous");
            messages[2].Should().Contain("hashtable");
            messages[3].Should().Contain("ConvertTo-Json");
            messages[4].Should().Contain("no URI");
        }

        [Fact]
        public void InvokeWebRequest_CurlWithCmdletParameters_IsNotMistakenForCurlExe()
        {
            Curl.DetectFlavor("curl -Uri https://x.example -Method Post").Should().Be(CurlFlavor.InvokeWebRequest);
            Curl.DetectFlavor("curl.exe -Uri https://x.example").Should().NotBe(CurlFlavor.InvokeWebRequest);
        }

        #endregion

        #region Auto detection

        [Theory]
        [InlineData("curl https://x.example", CurlFlavor.Posix)]
        [InlineData("curl -X POST -H 'A: b' -d '{\"a\":1}' https://x.example", CurlFlavor.Posix)]
        [InlineData("curl -d '{\\\"a\\\":1}' https://x.example", CurlFlavor.Posix)]
        [InlineData("curl https://x.example/%E2%9C%93", CurlFlavor.Posix)]
        [InlineData("Invoke-WebRequest https://x.example", CurlFlavor.InvokeWebRequest)]
        [InlineData("Invoke-RestMethod -Uri https://x.example", CurlFlavor.InvokeWebRequest)]
        [InlineData("iwr https://x.example", CurlFlavor.InvokeWebRequest)]
        [InlineData("irm https://x.example", CurlFlavor.InvokeWebRequest)]
        [InlineData("$r = iwr https://x.example", CurlFlavor.InvokeWebRequest)]
        [InlineData("curl -Uri https://x.example", CurlFlavor.InvokeWebRequest)]
        [InlineData("curl https://x.example -Method Post -Body 'x'", CurlFlavor.InvokeWebRequest)]
        [InlineData("curl https://x.example -Headers @{ A = 'b' }", CurlFlavor.InvokeWebRequest)]
        [InlineData("curl https://x.example `\n -H 'A: b'", CurlFlavor.PowerShell)]
        [InlineData("curl.exe -H \"Authorization: $env:TOKEN\" https://x.example", CurlFlavor.PowerShell)]
        [InlineData("curl.exe -d \"{`\"a`\":1}\" https://x.example", CurlFlavor.PowerShell)]
        [InlineData("curl.exe --% -d \"x\" https://x.example", CurlFlavor.PowerShell)]
        [InlineData("curl https://x.example ^\n -H \"A: b\"", CurlFlavor.WindowsCmd)]
        [InlineData("curl ^\"https://x.example^\"", CurlFlavor.WindowsCmd)]
        public void Auto_DetectsTheFlavorFromTelltales(string command, CurlFlavor expected)
        {
            Curl.DetectFlavor(command).Should().Be(expected);
            if (expected != CurlFlavor.InvokeWebRequest)
            {
                _parser.Parse(command, CurlFlavor.Auto).Flavor.Should().Be(expected);
            }
        }

        [Fact]
        public void Auto_PercentVariable_PicksWindowsCmdOnlyWhenTheVariableIsSet()
        {
            var name = SetVariable("x");

            Curl.DetectFlavor($"curl -H \"A: %{name}%\" https://x.example").Should().Be(CurlFlavor.WindowsCmd);
            Curl.DetectFlavor($"curl -H \"A: %{UnsetVariable()}%\" https://x.example").Should().Be(CurlFlavor.Posix);
        }

        [Fact]
        public void Auto_EmptyText_IsPosix()
        {
            Curl.DetectFlavor("").Should().Be(CurlFlavor.Posix);
            Curl.DetectFlavor(null!).Should().Be(CurlFlavor.Posix);
        }

        [Fact]
        public void Auto_KeepsTheWindowsBackslashQuoteConvenience_ForPosixLookingCommands()
        {
            var options = Parse(@"curl -d '{\""a\"":1}' https://x.example", CurlFlavor.Auto);

            options.Data.Should().Be("{\"a\":1}");
            options.Flavor.Should().Be(CurlFlavor.Posix);
        }

        [Fact]
        public void Auto_KeepsRealJsonEscapesWhenBareQuotesExist()
        {
            Parse(@"curl -d '{""a"":""say \""hi\""""}' https://x.example", CurlFlavor.Auto).Data.Should().Be(@"{""a"":""say \""hi\""""}");
        }

        [Fact]
        public void Auto_BacktickQuote_UsesExactPowerShellRules()
        {
            var options = Parse("curl.exe -d \"{`\"a`\":1}\" https://x.example", CurlFlavor.Auto);

            options.Data.Should().Be("{\"a\":1}");
            options.Flavor.Should().Be(CurlFlavor.PowerShell);
        }

        [Fact]
        public void Auto_CaretQuote_UsesExactCmdRules()
        {
            var options = Parse("curl ^\"https://x.example/?a=1^&b=2^\" --data-raw ^\"^{^\\^\"a^\\^\":1^}^\"", CurlFlavor.Auto);

            options.Url.Should().Be("https://x.example/?a=1&b=2");
            options.Data.Should().Be("{\"a\":1}");
            options.Flavor.Should().Be(CurlFlavor.WindowsCmd);
        }

        [Fact]
        public void Auto_InvokeWebRequest_IsTranslated()
        {
            var options = Parse("iwr -Uri https://x.example -Method Delete", CurlFlavor.Auto);

            options.Method.Should().Be("DELETE");
            options.Flavor.Should().Be(CurlFlavor.InvokeWebRequest);
        }

        [Fact]
        public void Auto_FlavorIsStampedOnEveryTransfer()
        {
            var options = Parse("curl https://x.example/a https://x.example/b --next https://x.example/c", CurlFlavor.Auto);

            options.Flavor.Should().Be(CurlFlavor.Posix);
            options.AdditionalTransfers.Should().HaveCount(2);
            options.AdditionalTransfers.Should().OnlyContain(t => t.Flavor == CurlFlavor.Posix);
        }

        #endregion

        #region The same request four ways

        /// <summary>Runs a command against the in-process server and returns the raw request it received.</summary>
        private static async Task<string> WireBytesAsync(string command, CurlFlavor flavor, RawReplyServer server)
        {
            var before = server.Requests.Count;
            await Curl.ExecuteAsync(command, flavor);
            server.Requests.Count.Should().Be(before + 1);
            return server.Requests[before];
        }

        public static IEnumerable<object[]> SameRequestFourWays()
        {
            // 1. JSON POST with a header
            yield return new object[]
            {
                "curl -X POST '{URL}' -H 'Content-Type: application/json' -d '{\"name\":\"Ada\",\"n\":1}'",
                "curl -X POST \"{URL}\" -H \"Content-Type: application/json\" -d \"{\\\"name\\\":\\\"Ada\\\",\\\"n\\\":1}\"",
                "curl.exe -X POST '{URL}' -H 'Content-Type: application/json' -d '{\\\"name\\\":\\\"Ada\\\",\\\"n\\\":1}'",
                "Invoke-RestMethod -Uri '{URL}' -Method Post -ContentType 'application/json' -Body '{\"name\":\"Ada\",\"n\":1}'",
            };

            // 2. Bearer token from an environment variable (each shell's own syntax)
            yield return new object[]
            {
                "curl -H \"Authorization: Bearer $CDN_FLAVOR_SHARED_TOKEN\" '{URL}'",
                "curl -H \"Authorization: Bearer %CDN_FLAVOR_SHARED_TOKEN%\" \"{URL}\"",
                "curl.exe -H \"Authorization: Bearer $env:CDN_FLAVOR_SHARED_TOKEN\" '{URL}'",
                "iwr '{URL}' -Headers @{ Authorization = \"Bearer $env:CDN_FLAVOR_SHARED_TOKEN\" }",
            };

            // 3. Multi-line command with each shell's continuation character
            yield return new object[]
            {
                "curl -X PUT \\\n  -H 'Accept: application/json' \\\n  -d 'v=1' \\\n  '{URL}'",
                "curl -X PUT ^\r\n  -H \"Accept: application/json\" ^\r\n  -d \"v=1\" ^\r\n  \"{URL}\"",
                "curl.exe -X PUT `\r\n  -H 'Accept: application/json' `\r\n  -d 'v=1' `\r\n  '{URL}'",
                "Invoke-WebRequest -Uri '{URL}' `\r\n  -Method Put `\r\n  -Headers @{ Accept = 'application/json' } `\r\n  -Body 'v=1'",
            };

            // 4. A single quote inside the body
            yield return new object[]
            {
                "curl -X POST -d 'name=O'\\''Brien' '{URL}'",
                "curl -X POST -d \"name=O'Brien\" \"{URL}\"",
                "curl.exe -X POST -d \"name=O'Brien\" '{URL}'",
                "irm '{URL}' -Method Post -Body 'name=O''Brien'",
            };

            // 5. A query string with & and a custom User-Agent
            yield return new object[]
            {
                "curl -A 'Probe/1.0' '{URL}?a=1&b=two'",
                "curl -A \"Probe/1.0\" \"{URL}?a=1&b=two\"",
                "curl.exe -A 'Probe/1.0' '{URL}?a=1&b=two'",
                "iwr '{URL}?a=1&b=two' -UserAgent 'Probe/1.0'",
            };
        }

        [Theory]
        [MemberData(nameof(SameRequestFourWays))]
        public async Task SameIntent_FromBashCmdPowerShellAndInvokeWebRequest_SendsIdenticalBytes(string bash, string cmd, string powershell, string iwr)
        {
            Environment.SetEnvironmentVariable("CDN_FLAVOR_SHARED_TOKEN", "t0k3n");
            _variables.Add("CDN_FLAVOR_SHARED_TOKEN");
            using var server = new RawReplyServer(Ok);
            var url = server.Url("/api");

            var expected = await WireBytesAsync(bash.Replace("{URL}", url), CurlFlavor.Posix, server);
            (await WireBytesAsync(cmd.Replace("{URL}", url), CurlFlavor.WindowsCmd, server)).Should().Be(expected, "cmd.exe flavor");
            (await WireBytesAsync(powershell.Replace("{URL}", url), CurlFlavor.PowerShell, server)).Should().Be(expected, "PowerShell flavor");
            (await WireBytesAsync(iwr.Replace("{URL}", url), CurlFlavor.InvokeWebRequest, server)).Should().Be(expected, "Invoke-WebRequest flavor");

            // Auto recognises all four and sends the same bytes again
            foreach (var command in new[] { bash, cmd, powershell, iwr })
            {
                (await WireBytesAsync(command.Replace("{URL}", url), CurlFlavor.Auto, server)).Should().Be(expected, "Auto for " + command);
            }
        }

        [Fact]
        public async Task JsonPost_WireBytes_AreExactlyTheJson()
        {
            using var server = new RawReplyServer(Ok);
            var url = server.Url("/api");

            var request = await WireBytesAsync("Invoke-RestMethod -Uri '" + url + "' -Method Post -ContentType 'application/json' -Body '{\"a\":1}'", CurlFlavor.InvokeWebRequest, server);

            request.Should().StartWith("POST /api HTTP/1.1\r\n");
            request.Should().Contain("\r\nContent-Type: application/json\r\n");
            request.Should().Contain("\r\nContent-Length: 7\r\n");
            request.Should().EndWith("\r\n\r\n{\"a\":1}");
        }

        [Fact]
        public async Task BackslashQuotedJson_PosixKeepsBackslashes_PowerShellAndCmdDoNot()
        {
            using var server = new RawReplyServer(Ok);
            var url = server.Url("/");

            var posix = await WireBytesAsync("curl -d '{\\\"a\\\":1}' " + url, CurlFlavor.Posix, server);
            var powershell = await WireBytesAsync("curl.exe -d '{\\\"a\\\":1}' " + url, CurlFlavor.PowerShell, server);
            var cmd = await WireBytesAsync("curl -d \"{\\\"a\\\":1}\" " + url, CurlFlavor.WindowsCmd, server);

            posix.Should().EndWith("\r\n\r\n{\\\"a\\\":1}");
            powershell.Should().EndWith("\r\n\r\n{\"a\":1}");
            cmd.Should().EndWith("\r\n\r\n{\"a\":1}");
        }

        [Fact]
        public async Task InvokeWebRequest_InFileAndForm_SendTheFileBytes()
        {
            var file = Path.Combine(_dir, "body.bin");
            File.WriteAllText(file, "file-content");
            using var server = new RawReplyServer(Ok);
            var url = server.Url("/up");

            var inFile = await WireBytesAsync($"iwr '{url}' -Method Put -InFile '{file}'", CurlFlavor.InvokeWebRequest, server);
            var form = await WireBytesAsync($"iwr '{url}' -Method Post -Form @{{ name = 'Ada; x'; doc = Get-Item '{file}' }}", CurlFlavor.InvokeWebRequest, server);

            inFile.Should().StartWith("PUT /up HTTP/1.1").And.EndWith("\r\n\r\nfile-content");
            form.Should().StartWith("POST /up HTTP/1.1").And.Contain("multipart/form-data");
            form.Should().Contain("name=\"name\"\r\n\r\nAda; x\r\n");
            form.Should().Contain("name=\"doc\"; filename=\"body.bin\"").And.Contain("file-content");
        }

        #endregion

        #region Edge cases found in review

        [Fact]
        public void InvokeWebRequest_ParenthesesInsideAUrl_BelongToIt()
        {
            Parse("wget https://en.wikipedia.org/wiki/Foo_(bar)", CurlFlavor.Auto).Url.Should().Be("https://en.wikipedia.org/wiki/Foo_(bar)");
        }

        [Theory]
        [InlineData("iwr https://x.example )", "does not start a value")]
        [InlineData("iwr https://x.example -Body $(cat a.json)", "PowerShell expression $(cat a.json)")]
        [InlineData("iwr https://x.example -Body (Get-Content a.json", "( is never closed")]
        [InlineData("iwr -Uri:", "ends with a colon")]
        [InlineData("wget -O out.html https://x.example", "GNU wget")]
        [InlineData("'{\"a\":1}' | irm https://x.example -Method Post", "second element of a pipeline")]
        [InlineData("iwr https://x.example -Body @\"x\"", "as InvokeWebRequest")]
        public void InvokeWebRequest_BrokenSyntax_ThrowsInsteadOfHangingOrGuessing(string command, string expected)
        {
            Action act = () => Parse(command, command.StartsWith("wget", StringComparison.Ordinal) ? CurlFlavor.Auto : CurlFlavor.InvokeWebRequest);

            act.Should().Throw<CurlInvalidCommandException>().Where(e => e.Message.Contains(expected) && e.Message.Contains(Docs));
        }

        [Theory]
        [InlineData("curl -d 'text=hello -Body world' https://x.example", "text=hello -Body world")]
        [InlineData("curl -H 'X-Auth -Token: abc' -d \"a -Uri b\" https://x.example", "a -Uri b")]
        public void Auto_CmdletWordsInsideQuotes_DoNotMakeABashCommandInvokeWebRequest(string command, string data)
        {
            Curl.DetectFlavor(command).Should().Be(CurlFlavor.Posix);
            Parse(command, CurlFlavor.Auto).Data.Should().Be(data);
        }

        [Fact]
        public void WindowsCmd_CaretCannotStopPercentExpansion()
        {
            var name = SetVariable("abc");

            Parse($"curl https://x.example/^%{name}%", CurlFlavor.WindowsCmd).Url.Should().Be("https://x.example/abc");
        }

        [Theory]
        [InlineData("curl https://x.example/a | findstr x", CurlFlavor.WindowsCmd)]
        [InlineData("curl https://x.example/a > out.txt", CurlFlavor.WindowsCmd)]
        [InlineData("curl https://x.example/a 2>nul", CurlFlavor.WindowsCmd)]
        [InlineData("curl https://x.example/a && echo ok", CurlFlavor.WindowsCmd)]
        [InlineData("curl.exe https://x.example/a && echo ok", CurlFlavor.PowerShell)]
        [InlineData("curl.exe https://x.example/a > out.txt", CurlFlavor.PowerShell)]
        [InlineData("curl.exe --% https://x.example/a | ConvertFrom-Json", CurlFlavor.PowerShell)]
        [InlineData("curl https://x.example/a | jq .", CurlFlavor.Posix)]
        [InlineData("curl https://x.example/a 2>/dev/null", CurlFlavor.Posix)]
        [InlineData("curl https://x.example/a && echo ok; echo done", CurlFlavor.Posix)]
        [InlineData("curl https://x.example/a \\", CurlFlavor.Posix)]
        public void ShellOperators_EndTheCurlCommand(string command, CurlFlavor flavor)
        {
            var options = Parse(command, flavor);

            options.Url.Should().Be("https://x.example/a");
            options.AdditionalTransfers.Should().BeEmpty();
        }

        [Fact]
        public void Posix_UnquotedVariable_IsSplitIntoWords_QuotedIsNot()
        {
            var name = SetVariable("-s -L");

            var unquoted = Parse($"curl ${name} https://x.example", CurlFlavor.Posix);
            var quoted = Parse($"curl -H \"X: ${name}\" https://x.example", CurlFlavor.Posix);

            unquoted.Silent.Should().BeTrue();
            unquoted.FollowLocation.Should().BeTrue();
            quoted.Headers["X"].Should().Be("-s -L");
        }

        [Fact]
        public void PowerShell_ArgumentWithBareQuotes_KeepsItsBackslashEscapes()
        {
            var hereString = Parse("curl.exe -d @'\n{\"a\":\"say \\\"hi\\\"\"}\n'@ https://x.example", CurlFlavor.PowerShell);
            var windowsStyle = Parse("curl.exe -d 'say \\\"hi\\\"' https://x.example", CurlFlavor.PowerShell);

            hereString.Data.Should().Be("{\"a\":\"say \\\"hi\\\"\"}");
            windowsStyle.Data.Should().Be("say \"hi\"");
        }

        [Fact]
        public void PowerShell_EmptyHereString_IsAnEmptyArgument()
        {
            Parse("curl.exe -H @'\n'@ https://x.example", CurlFlavor.PowerShell).Url.Should().Be("https://x.example");
        }

        [Fact]
        public void NoUrl_MessageLinksTheFlavorGuide()
        {
            Action act = () => Parse("curl -d \"abc https://x.example", CurlFlavor.WindowsCmd);

            act.Should().Throw<CurlInvalidCommandException>().Where(e => e.Message.Contains("No URL specified") && e.Message.Contains(Docs));
        }

        #endregion

        #region Public API

        [Fact]
        public void Validate_WithFlavor_ReportsTheParsedFlavorAndData()
        {
            var posix = Curl.Validate("curl -d '{\\\"a\\\":1}' https://x.example", CurlFlavor.Posix);
            var powershell = Curl.Validate("curl -d '{\\\"a\\\":1}' https://x.example", CurlFlavor.PowerShell);

            posix.IsValid.Should().BeTrue();
            posix.ParsedOptions!.Data.Should().Be("{\\\"a\\\":1}");
            posix.ParsedOptions.Flavor.Should().Be(CurlFlavor.Posix);
            powershell.ParsedOptions!.Data.Should().Be("{\"a\":1}");
            powershell.ParsedOptions.Flavor.Should().Be(CurlFlavor.PowerShell);
        }

        [Fact]
        public void Validate_WithFlavor_ReportsFlavorErrorsWithoutThrowing()
        {
            var result = Curl.Validate("iwr https://x.example -Credential $c", CurlFlavor.InvokeWebRequest);

            result.IsValid.Should().BeFalse();
            result.Error.Should().Contain("-u user:pass");
        }

        [Fact]
        public async Task ExecuteSyncAndSettings_UseTheGivenFlavor()
        {
            using var server = new RawReplyServer(Ok);
            var command = "curl -d '{\\\"a\\\":1}' " + server.Url("/");

            Curl.Execute(command, CurlFlavor.PowerShell);
            await Curl.ExecuteAsync(command, new CurlSettings().WithFlavor(CurlFlavor.Posix));
            await Curl.ExecuteAsync(command, CurlFlavor.WindowsCmd, default);

            server.Requests[0].Should().EndWith("\r\n\r\n{\"a\":1}");
            server.Requests[1].Should().EndWith("\r\n\r\n{\\\"a\\\":1}");
            server.Requests[2].Should().EndWith("\r\n\r\n'{\"a\":1}'");
        }

        [Fact]
        public void CurlSettings_WithFlavor_SetsAndChains()
        {
            var settings = new CurlSettings();
            settings.Flavor.Should().BeNull();

            settings.WithFlavor(CurlFlavor.WindowsCmd).Should().BeSameAs(settings);
            settings.Flavor.Should().Be(CurlFlavor.WindowsCmd);
        }

        [Fact]
        public void CurlOptions_Flavor_DefaultsToAutoAndIsCloned()
        {
            var options = new CurlOptions();
            options.Flavor.Should().Be(CurlFlavor.Auto);

            options.Flavor = CurlFlavor.PowerShell;
            options.Clone().Flavor.Should().Be(CurlFlavor.PowerShell);
        }

        [Fact]
        public void Parse_UndefinedFlavor_IsRejected()
        {
            Action act = () => _parser.Parse("curl https://x.example", (CurlFlavor)42);

            act.Should().Throw<ArgumentOutOfRangeException>().Where(e => e.Message.Contains(Docs));
        }

        #endregion
    }

    /// <summary>
    /// Tests that change <see cref="Curl.DefaultFlavor"/>, a process-wide setting. They run alone, after the parallel
    /// tests, so no other test parses a command while the default is changed.
    /// </summary>
    [CollectionDefinition(nameof(DefaultFlavorCollection), DisableParallelization = true)]
    public class DefaultFlavorCollection
    {
    }

    /// <summary><see cref="Curl.DefaultFlavor"/> behaviour.</summary>
    [Trait("Category", TestCategories.Unit)]
    [Collection(nameof(DefaultFlavorCollection))]
    public class DefaultFlavorTests
    {
        [Fact]
        public void DefaultFlavor_IsAutoAndAppliesToCallsWithoutAFlavor()
        {
            Curl.DefaultFlavor.Should().Be(CurlFlavor.Auto);
            var parser = new CommandParser();
            try
            {
                Curl.DefaultFlavor = CurlFlavor.Posix;
                parser.Parse("curl -d '{\\\"a\\\":1}' https://x.example").Data.Should().Be("{\\\"a\\\":1}");
                CurlSettings.FromDefaults().Flavor.Should().Be(CurlFlavor.Posix);
                Curl.Validate("curl -d '{\\\"a\\\":1}' https://x.example").ParsedOptions!.Flavor.Should().Be(CurlFlavor.Posix);

                // an explicit flavor still wins over the default
                parser.Parse("curl -d '{\\\"a\\\":1}' https://x.example", CurlFlavor.PowerShell).Data.Should().Be("{\"a\":1}");
            }
            finally
            {
                Curl.DefaultFlavor = CurlFlavor.Auto;
            }

            parser.Parse("curl -d '{\\\"a\\\":1}' https://x.example").Data.Should().Be("{\"a\":1}");
        }

        [Fact]
        public void DefaultFlavor_RejectsUndefinedValues()
        {
            Action act = () => Curl.DefaultFlavor = (CurlFlavor)(-1);

            act.Should().Throw<ArgumentOutOfRangeException>().Where(e => e.Message.Contains("curl-flavors-bash-cmd-powershell"));
            Curl.DefaultFlavor.Should().Be(CurlFlavor.Auto);
        }
    }
}
