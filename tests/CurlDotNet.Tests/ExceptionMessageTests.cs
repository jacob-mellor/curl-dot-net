using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CurlDotNet.Tests
{
    /// <summary>
    /// Every exception CurlDotNet throws must say three things: what happened (with the concrete values),
    /// how to fix it, and where to read more ("See https://github.com/jacob-mellor/curl-dot-net/...").
    /// </summary>
    [Trait("Category", TestCategories.Unit)]
    public class ExceptionMessageTests
    {
        private const string DocsPrefix = "See https://github.com/jacob-mellor/curl-dot-net/";
        private readonly ITestOutputHelper _output;

        public ExceptionMessageTests(ITestOutputHelper output)
        {
            _output = output;
        }

        // ------------------------------------------------------------------ helpers

        internal static string RepoRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && !(Directory.Exists(Path.Combine(dir.FullName, "docs")) && Directory.Exists(Path.Combine(dir.FullName, "src"))))
            {
                dir = dir.Parent;
            }
            return dir?.FullName ?? throw new InvalidOperationException("Could not find the repository root (a folder with docs/ and src/) above " + AppContext.BaseDirectory);
        }

        private static string LocalPathOf(string url)
        {
            var marker = "/blob/master/";
            var index = url.IndexOf(marker, StringComparison.Ordinal);
            index.Should().BeGreaterThan(0, $"'{url}' should be a GitHub blob URL");
            var path = url.Substring(index + marker.Length);
            var hash = path.IndexOf('#');
            return hash >= 0 ? path.Substring(0, hash) : path;
        }

        /// <summary>Asserts the three parts on one exception and returns its message for further checks.</summary>
        internal static string AssertWhatFixLink(CurlException ex, string because = "")
        {
            var message = ex.Message;
            message.Should().Contain(DocsPrefix, $"the message must end with a documentation link {because}:\n{message}");
            CountOf(message, "See https://").Should().Be(1, $"exactly one link, never 'See ... See ...' {because}:\n{message}");
            message.TrimEnd().Split('\n').Last().Should().StartWith("See https://", $"the link is the last line {because}:\n{message}");
            ex.WhatHappened.Should().NotBeNullOrWhiteSpace(because);
            ex.WhatHappened.Should().NotContain("https://github.com/jacob-mellor/curl-dot-net/", because);
            ex.Suggestions.Should().NotBeEmpty($"there must be a fix {because}:\n{message}");
            ex.Suggestions.Should().OnlyContain(s => s.Trim().Length > 15, $"each fix is a real sentence {because}");
            ex.DocumentationUrl.Should().NotBeNull(because);
            File.Exists(Path.Combine(RepoRoot(), LocalPathOf(ex.DocumentationUrl!))).Should().BeTrue($"{ex.DocumentationUrl} must exist in docs/ {because}");
            ex.ToString().Should().Contain(message, "ToString() uses the same normalised message");
            return message;
        }

        private static int CountOf(string text, string part)
        {
            var count = 0;
            for (var i = text.IndexOf(part, StringComparison.Ordinal); i >= 0; i = text.IndexOf(part, i + part.Length, StringComparison.Ordinal))
            {
                count++;
            }
            return count;
        }

        // ------------------------------------------------------------------ links

        [Fact]
        public void EveryDocLinkPointsAtAFileInTheRepository()
        {
            var root = RepoRoot();
            var missing = DocLinks.All().Where(url => !File.Exists(Path.Combine(root, LocalPathOf(url)))).ToList();
            missing.Should().BeEmpty("every DocLinks constant must name a page that exists under docs/");
        }

        [Fact]
        public void EveryDocsUrlInTheSourceCodePointsAtAFileInTheRepository()
        {
            var root = RepoRoot();
            var pattern = new System.Text.RegularExpressions.Regex(@"https://github\.com/jacob-mellor/curl-dot-net/blob/master/[A-Za-z0-9_./#-]+");
            var missing = new List<string>();
            foreach (var file in Directory.GetFiles(Path.Combine(root, "src", "CurlDotNet"), "*.cs", SearchOption.AllDirectories)
                         .Where(f => !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar)))
            {
                foreach (System.Text.RegularExpressions.Match match in pattern.Matches(File.ReadAllText(file)))
                {
                    var url = match.Value.TrimEnd('.');
                    if (!File.Exists(Path.Combine(root, LocalPathOf(url))) && !Directory.Exists(Path.Combine(root, LocalPathOf(url))))
                    {
                        missing.Add($"{Path.GetFileName(file)}: {url}");
                    }
                }
            }
            missing.Distinct().Should().BeEmpty("documentation links in XML docs and messages must not be dead");
        }

        // ------------------------------------------------------------------ the message format

        [Fact]
        public void Message_LegacyForSeeFormat_IsNormalisedToOneTrailingSeeLine()
        {
            var ex = new CurlException("Could not frob 'x.txt'.\nRename it.\nFor frob errors, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/index.md\nSee https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md", 3);

            ex.Message.Should().Be("Could not frob 'x.txt'.\nRename it.\nSee https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/index.md");
            ex.WhatHappened.Should().Be("Could not frob 'x.txt'.");
            ex.Suggestions.Should().Equal("Rename it.");
            ex.DocumentationUrl.Should().EndWith("exceptions/index.md");
        }

        [Fact]
        public void Message_PlainUserMessageWithoutCode_IsUnchanged()
        {
            new CurlException("Test message").Message.Should().Be("Test message");
        }

        [Fact]
        public void Message_WithExitCodeButNoFix_GetsTheGenericFixAndLinkForThatCode()
        {
            var ex = new CurlException("Something about host x failed.", 6);
            AssertWhatFixLink(ex);
            ex.DocumentationUrl.Should().EndWith("dns-errors.md");
        }

        [Fact]
        public void Message_WrappingAnException_ExplainsTheInnerCauseInWords()
        {
            var inner = new IOException("The disk is full");
            var ex = new CurlException("Could not write 'out.json'.\nFree some disk space.", 23, "curl -o out.json https://x", inner);

            ex.InnerException.Should().BeSameAs(inner);
            ex.WhatHappened.Should().Contain("IOException").And.Contain("The disk is full");
            AssertWhatFixLink(ex);
        }

        [Fact]
        public void ToUserFriendlyMessage_ShowsWhatFixesAndLinkOnce()
        {
            var ex = new CurlException("Could not frob.\nRename it.", 3);
            var text = ex.ToUserFriendlyMessage();
            text.Should().Contain("Error: Could not frob.").And.Contain("• Rename it.").And.Contain("More help: https://");
            CountOf(text, "Rename it.").Should().Be(1);
        }

        // ------------------------------------------------------------------ every exception type

        /// <summary>A plausible argument for a constructor parameter, chosen by name and type.</summary>
        private static object? ArgumentFor(ParameterInfo p)
        {
            var t = Nullable.GetUnderlyingType(p.ParameterType) ?? p.ParameterType;
            var name = p.Name ?? string.Empty;
            if (t == typeof(string))
            {
                switch (name)
                {
                    case "command": return "curl https://api.example.com/v1/items";
                    case "url":
                    case "lastUrl": return "https://api.example.com/v1/items";
                    case "host":
                    case "hostname":
                    case "proxyHost": return "api.example.com";
                    case "filePath":
                    case "fileName":
                    case "cookieJarPath":
                    case "path": return "/tmp/curldotnet/out.json";
                    case "protocol": return "gopher";
                    case "statusText": return "Not Found";
                    case "responseBody":
                    case "body": return "{\"error\":\"nope\"}";
                    default: return $"the {name} value 'x1'";
                }
            }
            if (t == typeof(int)) return name.IndexOf("status", StringComparison.OrdinalIgnoreCase) >= 0 ? 404 : name.IndexOf("port", StringComparison.OrdinalIgnoreCase) >= 0 ? 8443 : name == "curlErrorCode" ? 7 : name == "ftpCode" ? 550 : 3;
            if (t == typeof(long)) return 1024L;
            if (t == typeof(double)) return 5.0;
            if (t == typeof(bool)) return name == "redirect";
            if (t == typeof(TimeSpan)) return TimeSpan.FromSeconds(30);
            if (t == typeof(Type)) return typeof(Dictionary<string, object>);
            if (typeof(Exception).IsAssignableFrom(t)) return new IOException("Access to the path is denied");
            if (t.IsEnum) return Enum.GetValues(t).GetValue(0);
            return p.HasDefaultValue ? p.DefaultValue : null;
        }

        public static IEnumerable<object[]> ExceptionConstructors()
        {
            var types = typeof(CurlException).Assembly.GetTypes()
                .Where(t => t.IsPublic && !t.IsAbstract && typeof(CurlException).IsAssignableFrom(t))
                .OrderBy(t => t.FullName);
            foreach (var type in types)
            {
                foreach (var ctor in type.GetConstructors().Where(c => !c.GetParameters().Any(p => p.ParameterType == typeof(System.Runtime.Serialization.SerializationInfo))))
                {
                    if (type == typeof(CurlException) && !ctor.GetParameters().Any(p => p.Name == "curlErrorCode"))
                    {
                        continue; // your own `new CurlException("text")` keeps your text (Message_PlainUserMessageWithoutCode_IsUnchanged)
                    }
                    yield return new object[] { type.FullName!, string.Join(",", ctor.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name)) };
                }
            }
        }

        [Theory]
        [MemberData(nameof(ExceptionConstructors))]
        public void EveryExceptionConstructor_ProducesWhatFixAndLink(string typeName, string signature)
        {
            var type = typeof(CurlException).Assembly.GetType(typeName)!;
            var ctor = type.GetConstructors().Single(c => string.Join(",", c.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name)) == signature);
            var args = ctor.GetParameters().Select(ArgumentFor).ToArray();
            var ex = (CurlException)ctor.Invoke(args);

            var message = AssertWhatFixLink(ex, $"for new {type.Name}({signature})");
            _output.WriteLine(message);

            // CurlDotNet.Core.CurlFtpException lives in FtpHandler.cs and is covered by the transport owners
            if (type != typeof(CurlException) && type.Namespace == "CurlDotNet.Exceptions" && ex.CurlErrorCode.HasValue)
            {
                ex.Suggestions[0].Should().NotBe(CurlException.DefaultFix(ex.CurlErrorCode.Value),
                    $"new {type.Name}({signature}) must give advice specific to its error, not the generic text for exit code {ex.CurlErrorCode}");
            }
        }

        private static CurlException Construct(ConstructorInfo ctor) =>
            (CurlException)ctor.Invoke(ctor.GetParameters().Select(ArgumentFor).ToArray());

        [Fact]
        public void DifferentConstructorsOfTheSameType_GiveDifferentMessages()
        {
            var types = typeof(CurlException).Assembly.GetTypes()
                .Where(t => t.IsPublic && !t.IsAbstract && typeof(CurlException).IsAssignableFrom(t) && t != typeof(CurlException));
            var duplicates = new List<string>();
            foreach (var type in types)
            {
                var messages = type.GetConstructors()
                    .Where(c => !c.GetParameters().Any(p => p.ParameterType == typeof(System.Runtime.Serialization.SerializationInfo)))
                    .Select(c => (Signature: string.Join(",", c.GetParameters().Select(p => p.Name)), Message: Construct(c).Message))
                    .ToList();
                foreach (var group in messages.GroupBy(m => m.Message).Where(g => g.Count() > 1))
                {
                    duplicates.Add($"{type.Name}: {string.Join(" | ", group.Select(g => g.Signature))}");
                }
            }
            duplicates.Should().BeEmpty("each constructor exists for a different situation, so its message must differ");
        }

        [Fact]
        public void SameTypeDifferentSituation_GivesDifferentAdvice()
        {
            var server = new CurlCouldntConnectException("api.example.com", 443, "Connection refused", viaProxy: false, "curl https://api.example.com");
            var proxy = new CurlCouldntConnectException("proxy.local", 3128, "Connection refused", viaProxy: true, "curl -x proxy.local:3128 https://api.example.com");
            server.Suggestions[0].Should().NotBe(proxy.Suggestions[0]);
            proxy.Message.Should().Contain("proxy.local").And.Contain("3128").And.Contain("--noproxy");
            server.Message.Should().Contain("api.example.com").And.Contain("443");

            var notFound = new CurlHttpReturnedErrorException(404, "Not Found", null, "https://api.example.com/v1/x?token=secret", false, null);
            var unauthorized = new CurlHttpReturnedErrorException(401, "Unauthorized", null, "https://api.example.com/v1/x", false, null);
            notFound.Message.Should().Contain("404").And.Contain("https://api.example.com/v1/x").And.NotContain("token=secret");
            notFound.Suggestions.Should().Contain(s => s.Contains("URL"));
            unauthorized.Suggestions.Should().Contain(s => s.Contains("credentials"));
            notFound.Suggestions.Should().NotBeEquivalentTo(unauthorized.Suggestions);

            var write = new CurlFileException("Could not save.", "/tmp/a.json", CurlFileException.FileOperation.Write);
            var read = new CurlFileException("Could not load.", "/tmp/a.json", CurlFileException.FileOperation.Read);
            write.Suggestions[0].Should().NotBe(read.Suggestions[0]);
        }

        [Theory]
        [InlineData(typeof(CurlSslEngineNotFoundException), 53)]
        [InlineData(typeof(CurlSslEngineSetFailedException), 54)]
        [InlineData(typeof(CurlSslCertificateProblemException), 58)]
        [InlineData(typeof(CurlSslCipherException), 59)]
        [InlineData(typeof(CurlUseSslFailedException), 64)]
        [InlineData(typeof(CurlPeerFailedVerificationException), 60)]
        [InlineData(typeof(CurlSslConnectErrorException), 35)]
        [InlineData(typeof(CurlFtpAcceptTimeoutException), 12)]
        public void ExitCode_MatchesCurlH(Type type, int code)
        {
            var ex = (CurlException)Activator.CreateInstance(type, "detail of the problem", null)!;
            ex.CurlErrorCode.Should().Be(code);
        }

        [Fact]
        public void HttpException_StatusAdvice_GoesIntoSuggestionsWithoutBeingRepeatedInTheMessage()
        {
            var ex = new CurlDotNet.Exceptions.CurlHttpException("GET https://api.example.com/x failed", 429);
            ex.Suggestions.Count.Should().BeGreaterThan(1);
            ex.Suggestions[0].Should().Contain("rate limit");
            CountOf(ex.Message, ex.Suggestions[0]).Should().Be(1);
            AssertWhatFixLink(ex);
        }

        // ------------------------------------------------------------------ real failures

        [Fact]
        public async Task RealFailure_ConnectionRefused_NamesHostAndPortAndHowToFix()
        {
            var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
            listener.Start();
            var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            Func<Task> act = () => Curl.ExecuteAsync($"curl http://127.0.0.1:{port}/");

            var ex = (await act.Should().ThrowAsync<CurlException>()).Which;
            ex.CurlErrorCode.Should().Be(7);
            var message = AssertWhatFixLink(ex);
            message.Should().Contain("127.0.0.1").And.Contain(port.ToString());
        }

        [Fact]
        public async Task RealFailure_MissingDataFile_NamesTheFile()
        {
            var missing = Path.Combine(Path.GetTempPath(), "curldotnet-missing-" + Guid.NewGuid().ToString("N") + ".json");

            Func<Task> act = () => Curl.ExecuteAsync($"curl -d @{missing} http://127.0.0.1:9/");

            var ex = (await act.Should().ThrowAsync<CurlException>()).Which;
            var message = AssertWhatFixLink(ex);
            message.Should().Contain(Path.GetFileName(missing));
        }

        [Fact]
        public async Task RealFailure_MalformedUrl_NamesTheUrl()
        {
            Func<Task> act = () => Curl.ExecuteAsync("curl \"http://exa mple.com/\"");

            var ex = (await act.Should().ThrowAsync<CurlException>()).Which;
            AssertWhatFixLink(ex);
        }

        [Fact]
        public async Task RealFailure_OutputToMissingFolder_IsWriteErrorWithPath()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 2\r\n\r\nok");
            var target = Path.Combine(Path.GetTempPath(), "curldotnet-nodir-" + Guid.NewGuid().ToString("N"), "out.txt");

            Func<Task> act = () => Curl.ExecuteAsync($"curl -o {CommandParser.ShellQuote(target)} {server.Url()}");

            var ex = (await act.Should().ThrowAsync<CurlWriteErrorException>()).Which;
            ex.CurlErrorCode.Should().Be(23);
            AssertWhatFixLink(ex).Should().Contain("out.txt");
        }

        [Fact]
        public async Task RealFailure_FailOnHttpError_SaysStatusUrlAndFix()
        {
            using var server = new RawReplyServer("HTTP/1.1 404 Not Found\r\nContent-Length: 0\r\n\r\n");

            Func<Task> act = () => Curl.ExecuteAsync($"curl -f {server.Url("/missing")}");

            var ex = (await act.Should().ThrowAsync<CurlHttpReturnedErrorException>()).Which;
            AssertWhatFixLink(ex).Should().Contain("404").And.Contain("/missing");
        }

        [Fact]
        public async Task RealFailure_Timeout_SaysTheLimit()
        {
            using var server = new RawReplyServer("HTTP/1.1 200 OK\r\nContent-Length: 100\r\n\r\nshort") { StallMilliseconds = 5000 };

            Func<Task> act = () => Curl.ExecuteAsync($"curl -m 0.5 {server.Url()}");

            var ex = (await act.Should().ThrowAsync<CurlException>()).Which;
            ex.CurlErrorCode.Should().Be(28);
            AssertWhatFixLink(ex).Should().Contain("--max-time");
        }

        [Fact]
        public void RealFailure_UnknownOption_NamesTheOption()
        {
            Action act = () => Curl.Execute("curl --no-such-option https://example.com");

            var ex = act.Should().Throw<CurlException>().Which;
            AssertWhatFixLink(ex).Should().Contain("--no-such-option");
        }
    }
}
