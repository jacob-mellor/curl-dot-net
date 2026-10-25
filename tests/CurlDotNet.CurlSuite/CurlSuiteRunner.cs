using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using CurlDotNet.Exceptions;

namespace CurlDotNet.CurlSuite;

public enum SuiteStatus { Pass, Fail, Skip }

public sealed class SuiteOutcome
{
    public int Number { get; init; }
    public string Name { get; init; } = string.Empty;
    public SuiteStatus Status { get; init; }
    public string Reason { get; init; } = string.Empty;
    /// <summary>True when every comparison matched byte-for-byte (header order included).</summary>
    public bool Strict { get; init; }
    public string Command { get; init; } = string.Empty;
}

/// <summary>
/// Runs one curl test file against CurlDotNet: serves the canned reply from
/// <see cref="StubHttpServer"/>, executes the command through <see cref="Curl.ExecuteAsync(string)"/>
/// and verifies exit code, the request on the wire, uploaded data, stdout, the output file
/// and any files the test says must or must not exist. Mirrors tests/runtests.pl + runner.pm.
/// </summary>
public static class CurlSuiteRunner
{
    /// <summary>The curl version CurlDotNet claims compatibility with (used for %VERSION).</summary>
    public const string CurlVersion = "8.7.1";

    /// <summary>
    /// Features CurlDotNet provides, in curl's feature vocabulary. Tests requiring anything
    /// else are skipped; tests requiring the absence (!feature) of something not listed pass.
    /// </summary>
    public static readonly HashSet<string> Features = new(StringComparer.OrdinalIgnoreCase)
    {
        "http", "https", "proxy", "cookies", "Largefile", "libz", "verbose-strings", "crypto",
        "digest", "NTLM", "Mime", "aws", "IPv6", "AsynchDNS", "threadsafe", "UnixSockets", "SSL",
    };

    private static readonly string[] UnsupportedSections =
    {
        "tool", "stdin", "servercmd", "precheck", "postcheck", "killserver", "connect", "proxy", "socks", "dns",
    };

    /// <summary>
    /// Like runtests.pl (runner.pm) every command gets <c>--trace-ascii log/traceN --trace-time</c>: besides writing the
    /// trace this turns on curl's notes (<c>notef()</c> needs a trace type), such as "Read config file from".
    /// Set CURL_SUITE_NO_TRACE=1 to leave it out, e.g. to see whether tracing changes what is sent (test 164).
    /// </summary>
    public static bool AddTraceAscii { get; set; } = Environment.GetEnvironmentVariable("CURL_SUITE_NO_TRACE") != "1";

    public static async Task<SuiteOutcome> RunAsync(string path)
    {
        var test = CurlTestFile.Load(path);
        var name = test.Name;

        var skip = SkipReason(test);
        if (skip != null)
        {
            return new SuiteOutcome { Number = test.Number, Name = name, Status = SuiteStatus.Skip, Reason = skip };
        }

        var root = Path.Combine(Path.GetTempPath(), "curldotnet-suite", test.Number.ToString());
        if (Directory.Exists(root)) Directory.Delete(root, recursive: true);
        var logDir = Path.Combine(root, "log");
        Directory.CreateDirectory(logDir);

        // Preprocess reply parts first with a placeholder port, then start the server and redo
        // everything with the real port (the port is only known once the listener is bound).
        using var server = new StubHttpServer(new Dictionary<int, string>());
        server.Start();

        var variables = BuildVariables(test.Number, server.Port, root, logDir);
        var features = new HashSet<string>(Features, StringComparer.OrdinalIgnoreCase);
        var pre = new CurlTestPreprocessor(variables, features);

        // <stripfile>, <stripfile1..4> and <strippart>: perl run per output line by runtests.pl
        var stripRules = new Dictionary<string, List<PerlLineRule>>(StringComparer.Ordinal);
        foreach (var section in test.All("verify").Where(s => s.Name == "strippart" || Regex.IsMatch(s.Name, @"^stripfile\d?$")))
        {
            var lines = pre.Process(section.RawText).Split('\n');
            var rules = PerlLineRule.ParseAll(lines, out var ruleError);
            if (rules == null)
            {
                return new SuiteOutcome { Number = test.Number, Name = name, Status = SuiteStatus.Skip, Reason = $"<{section.Name}> perl the harness cannot replay: {ruleError}" };
            }
            stripRules[section.Name] = rules;
        }
        List<PerlLineRule> Rules(string sectionName) => stripRules.TryGetValue(sectionName, out var r) ? r : new List<PerlLineRule>();

        var replies = new Dictionary<int, string>();
        var connectReplies = new Dictionary<int, string>();
        foreach (var section in test.All("reply"))
        {
            var m = Regex.Match(section.Name, @"^(data|connect)(\d*)$");
            if (!m.Success) continue;
            var part = m.Groups[2].Value.Length == 0 ? 0 : int.Parse(m.Groups[2].Value);
            var text = pre.ProcessSection(section);
            if (section.Attr("crlf") == null)
            {
                // sws serves bytes verbatim and curl tolerates bare LF; HttpClient does not, so
                // normalise the header block (never the body) to CRLF.
                text = CurlTestPreprocessor.ToCrlf(text, headersOnly: true);
            }
            (m.Groups[1].Value == "connect" ? connectReplies : replies)[part] = text;
        }
        server.SetReplies(replies);
        server.SetConnectReplies(connectReplies);

        // Input files the test needs (relative names are relative to %PWD, i.e. the run root).
        foreach (var section in test.All("client").Where(s => Regex.IsMatch(s.Name, @"^file\d?$")))
        {
            var fileName = pre.Process(section.Attr("name") ?? string.Empty).Trim();
            if (fileName.Length == 0) continue;
            fileName = Path.IsPathRooted(fileName) ? fileName : Path.Combine(root, fileName);
            var content = pre.ProcessSection(section);
            Directory.CreateDirectory(Path.GetDirectoryName(fileName)!);
            File.WriteAllText(fileName, content, Encoding.Latin1);
        }

        // runner.pm: "NAME=value" sets, a bare "NAME" unsets, "#" lines are comments; the old values come back afterwards.
        // (.NET cannot hold an empty variable: "NAME=" unsets it, which is how CurlDotNet reads an empty one anyway.)
        var savedEnvironment = new Dictionary<string, string?>(StringComparer.Ordinal);
        foreach (var section in test.All("client").Where(s => s.Name == "setenv"))
        {
            foreach (var raw in section.Lines)
            {
                var line = raw.Trim();
                if (line.Length == 0 || line[0] == '#') continue;
                var eq = line.IndexOf('=');
                var variable = eq < 0 ? line : line.Substring(0, eq);
                if (variable.Length == 0) continue;
                if (!savedEnvironment.ContainsKey(variable)) savedEnvironment[variable] = Environment.GetEnvironmentVariable(variable);
                Environment.SetEnvironmentVariable(variable, eq < 0 ? null : pre.Process(line.Substring(eq + 1)));
            }
        }

        var commandSection = test.Get("client", "command");
        var option = commandSection?.Attr("option") ?? string.Empty;
        var userCommand = string.Join(" ", (commandSection?.Lines ?? new List<string>()).Select(l => l.Trim())).Trim();
        userCommand = pre.Process(userCommand).Replace("\n", " ").Trim();

        var outFile = Path.Combine(logDir, $"curl{test.Number}.out");
        var hasStdoutSection = test.Has("verify", "stdout");
        var addOutput = !option.Contains("no-output") && (!hasStdoutSection || option.Contains("force-output"));
        var addInclude = !option.Contains("no-include");

        // runtests.pl hands the command to a shell, so "> file" / ">> file" redirect stdout
        string? redirectFile = null;
        var redirectAppend = false;
        var redirect = Regex.Match(userCommand, @"\s(>>|>)\s*(\S+)\s*$");
        if (redirect.Success)
        {
            redirectAppend = redirect.Groups[1].Value == ">>";
            redirectFile = redirect.Groups[2].Value;
            redirectFile = Path.IsPathRooted(redirectFile) ? redirectFile : Path.Combine(root, redirectFile);
            userCommand = userCommand.Substring(0, redirect.Index).TrimEnd();
        }

        // runner.pm: "curl -q" (unless option="no-q") then the output, include and trace options, then the test's command
        var readCurlrc = option.Contains("no-q");
        var prefix = new StringBuilder("curl");
        if (!readCurlrc) prefix.Append(" -q");
        if (addOutput) prefix.Append(" --output '").Append(outFile.Replace("'", "'\\''")).Append('\'');
        if (addInclude) prefix.Append(" --include");
        if (AddTraceAscii) prefix.Append(" --trace-ascii log/trace").Append(test.Number).Append(" --trace-time");
        var command = prefix + " " + userCommand;

        var previousCwd = Directory.GetCurrentDirectory();
        int exitCode;
        string stdout = string.Empty;
        string? failure = null;
        CurlException? curlFailure = null;
        (bool Show, string? StderrFile) errorOutput = (true, null);
        var stderrCapture = new StringWriter();
        var previousStderr = Console.Error;
        var sw = Stopwatch.StartNew();
        try
        {
            Directory.SetCurrentDirectory(root);
            errorOutput = ErrorOutput(command); // parsed before the opt-in below, so it cannot print a note
            // the curl tool always reads .curlrc; CurlDotNet only when asked (runtests' -q switches it off again)
            Curl.DefaultReadConfigFile = true;
            Console.SetError(stderrCapture);
            var task = Curl.ExecuteAsync(command);
            var completed = await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(25)));
            if (completed != task)
            {
                return new SuiteOutcome { Number = test.Number, Name = name, Status = SuiteStatus.Fail, Reason = "timed out after 25s", Command = command };
            }
            var result = await task;
            exitCode = 0;
            stdout = result.Body ?? string.Empty;
        }
        catch (CurlException ex)
        {
            exitCode = ex.CurlErrorCode ?? 1;
            curlFailure = ex;
            // what curl would already have written to stdout before failing (-i headers, partial body, -w).
            // The Stdout key already includes the -w text; WriteOut alone is the fallback for older throw sites.
            stdout = ex.Data["CurlDotNet.Stdout"] as string ?? ex.Data["CurlDotNet.WriteOut"] as string ?? string.Empty;
        }
        catch (Exception ex)
        {
            exitCode = -1;
            failure = $"unexpected {ex.GetType().Name}: {FirstLine(ex.Message)}";
        }
        finally
        {
            Console.SetError(previousStderr);
            Curl.DefaultReadConfigFile = false;
            Directory.SetCurrentDirectory(previousCwd);
            foreach (var kv in savedEnvironment)
            {
                Environment.SetEnvironmentVariable(kv.Key, kv.Value);
            }
        }

        // Give the server a moment to log the last request.
        await Task.Delay(30);

        // runner.pm sends curl's stderr to log/stderrN. The library's own stderr text (notes, -w %{stderr}) was captured
        // above; the final "curl: (N) message" line is the curl tool's, so the harness writes it the way
        // tool_operate.c's post_check_result() does: unless -s is given without -S.
        var stderr = stderrCapture.ToString();
        if (curlFailure != null)
        {
            var (showErrors, stderrFile) = errorOutput;
            if (showErrors)
            {
                var line = $"curl: ({exitCode}) {curlFailure.CurlErrorMessage}\n";
                if (stderrFile != null)
                {
                    // --stderr FILE: curl's tool_stderr is that file
                    File.AppendAllText(Path.IsPathRooted(stderrFile) ? stderrFile : Path.Combine(root, stderrFile), line);
                }
                else
                {
                    stderr += line;
                }
            }
        }
        File.WriteAllBytes(Path.Combine(logDir, $"stderr{test.Number}"), Encoding.UTF8.GetBytes(stderr));
        // compare in the same Latin-1 view the expected sections use (a char is a byte)
        stderr = Encoding.Latin1.GetString(Encoding.UTF8.GetBytes(stderr));

        if (redirectFile != null)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(redirectFile)!);
            if (redirectAppend) File.AppendAllText(redirectFile, stdout, Encoding.Latin1);
            else File.WriteAllText(redirectFile, stdout, Encoding.Latin1);
            stdout = string.Empty;
        }
        else
        {
            // runtests.pl always redirects curl's stdout to %LOGDIR/stdoutNUM; tests verify it with <file name=...>
            File.WriteAllText(Path.Combine(logDir, $"stdout{test.Number}"), stdout, Encoding.Latin1);
        }

        var problems = new List<string>();
        var strict = true;
        if (failure != null) problems.Add(failure);

        // exit code
        var expectedCodes = (test.Get("verify", "errorcode")?.RawText.Trim() ?? "0")
            .Split(',').Select(s => int.TryParse(s.Trim(), out var c) ? c : 0).ToList();
        if (!expectedCodes.Contains(exitCode))
        {
            problems.Add($"exit code {exitCode}, expected {string.Join("/", expectedCodes)}");
        }

        var strips = test.Get("verify", "strip")?.Lines.Where(l => l.Length > 0).Select(l => pre.Process(l).TrimEnd('\n')).ToList() ?? new List<string>();

        // protocol
        var protocol = test.Get("verify", "protocol");
        if (protocol != null)
        {
            var expected = pre.ProcessSection(protocol);
            var actual = string.Concat(server.Requests.Select(r => Encoding.Latin1.GetString(r)));
            var cmp = HttpDumpComparer.Compare(expected, actual, strips, Rules("strippart"));
            if (!cmp.Match) problems.Add("protocol: " + cmp.Message);
            strict &= cmp.Strict;
        }

        // upload
        var upload = test.Get("verify", "upload");
        if (upload != null)
        {
            var expected = pre.ProcessSection(upload);
            var actual = PerlLineRule.ApplyStripPart(string.Concat(server.Bodies.Select(b => Encoding.Latin1.GetString(b))), Rules("strippart"));
            if (Normalize(expected) != Normalize(actual))
            {
                problems.Add("upload: " + HttpDumpComparer.Describe(expected, actual));
            }
            strict &= expected == actual;
        }

        // stdout
        var stdoutSection = test.Get("verify", "stdout");
        if (stdoutSection != null && !option.Contains("force-output"))
        {
            var expected = pre.ProcessSection(stdoutSection);
            if (expected.Trim() == "%EMPTY") expected = string.Empty;
            var cmp = HttpDumpComparer.Compare(expected, PerlLineRule.ApplyStripFile(stdout, Rules("stripfile")), strips);
            if (!cmp.Match) problems.Add("stdout: " + cmp.Message);
            strict &= cmp.Strict;
        }

        // stderr (runtests.pl applies <stripfile> to it as well)
        var stderrSection = test.Get("verify", "stderr");
        if (stderrSection != null)
        {
            var expected = pre.ProcessSection(stderrSection);
            var actual = PerlLineRule.ApplyStripFile(stderr, Rules("stripfile"));
            if (stderrSection.HasAttr("mode", "warn"))
            {
                // runtests: drop "Warning: " and join the wrapped lines, so terminal width does not matter
                expected = UnwrapWarnings(expected);
                actual = UnwrapWarnings(actual);
            }
            var cmp = HttpDumpComparer.Compare(expected, actual, Array.Empty<string>());
            if (!cmp.Match) problems.Add("stderr: " + cmp.Message);
            strict &= cmp.Strict;
        }

        // output file vs <data>/<datacheck>
        var data = test.Get("reply", "data");
        var datacheck = test.Get("reply", "datacheck");
        if (addOutput && exitCode == 0 && (data != null || datacheck != null) && !(data?.HasAttr("nocheck", "yes") ?? false))
        {
            string expected;
            if (datacheck != null)
            {
                var sb = new StringBuilder(pre.ProcessSection(datacheck));
                foreach (var extra in test.All("reply").Where(s => Regex.IsMatch(s.Name, @"^datacheck\d+$")).OrderBy(s => int.Parse(s.Name.Substring(9))))
                {
                    sb.Append(pre.ProcessSection(extra));
                }
                expected = sb.ToString();
            }
            else
            {
                expected = pre.ProcessSection(data!);
            }

            var actual = File.Exists(outFile) ? File.ReadAllText(outFile, Encoding.Latin1) : "<no output file written>";
            var cmp = HttpDumpComparer.Compare(expected, actual, strips);
            if (!cmp.Match) problems.Add("output: " + cmp.Message);
            strict &= cmp.Strict;
        }

        // verify files
        foreach (var section in test.All("verify").Where(s => Regex.IsMatch(s.Name, @"^file\d?$")))
        {
            var fileName = pre.Process(section.Attr("name") ?? string.Empty).Trim();
            if (fileName.Length == 0) continue;
            fileName = Path.IsPathRooted(fileName) ? fileName : Path.Combine(root, fileName);
            var expected = pre.ProcessSection(section);
            if (!File.Exists(fileName))
            {
                if (expected.Length == 0) continue; // runtests.pl reads a missing file as empty
                problems.Add($"file {Path.GetFileName(fileName)}: not written");
                continue;
            }
            // <file> gets <stripfile>, <fileN> gets <stripfileN>
            var actual = PerlLineRule.ApplyStripFile(File.ReadAllText(fileName, Encoding.Latin1), Rules("strip" + section.Name));
            var fileCmp = HttpDumpComparer.Compare(expected, actual, Array.Empty<string>());
            if (!fileCmp.Match)
            {
                problems.Add($"file {Path.GetFileName(fileName)}: " + fileCmp.Message);
            }
            strict &= fileCmp.Strict;
        }

        var notExists = test.Get("verify", "notexists");
        if (notExists != null)
        {
            foreach (var line in notExists.Lines.Where(l => l.Trim().Length > 0))
            {
                var fileName = pre.Process(line).Trim();
                fileName = Path.IsPathRooted(fileName) ? fileName : Path.Combine(root, fileName);
                if (File.Exists(fileName)) problems.Add($"{Path.GetFileName(fileName)} must not exist");
            }
        }

        var status = problems.Count == 0 ? SuiteStatus.Pass : SuiteStatus.Fail;
        return new SuiteOutcome
        {
            Number = test.Number,
            Name = name,
            Status = status,
            Reason = string.Join("; ", problems),
            Strict = status == SuiteStatus.Pass && strict,
            Command = command,
        };
    }

    /// <summary>
    /// Whether curl prints its "curl: (N) ..." line for this command (not with <c>-s</c> unless <c>-S</c> is given, both
    /// global in curl) and the <c>--stderr</c> file it would go to.
    /// </summary>
    private static (bool Show, string? StderrFile) ErrorOutput(string command)
    {
        try
        {
            var options = Curl.Validate(command).ParsedOptions;
            if (options == null)
            {
                return (true, null);
            }

            var transfers = new[] { options }.Concat(options.AdditionalTransfers).ToList();
            var silent = transfers.Any(t => t.Silent);
            var showError = transfers.Any(t => t.ShowError);
            var file = transfers.Select(t => t.StderrFile).LastOrDefault(f => !string.IsNullOrEmpty(f) && f != "-");
            return (!silent || showError, file);
        }
        catch (Exception)
        {
            return (true, null); // a command curl rejects while parsing: curl still reports the error
        }
    }

    private static string? SkipReason(CurlTestFile test)
    {
        var servers = test.Get("client", "server")?.Lines.Select(l => l.Trim()).Where(l => l.Length > 0).ToList() ?? new List<string>();
        if (servers.Count != 1 || !string.Equals(servers[0], "http", StringComparison.OrdinalIgnoreCase))
        {
            return "needs server: " + string.Join(",", servers);
        }

        foreach (var section in UnsupportedSections)
        {
            if (test.Sections.Any(s => s.Name == section))
            {
                return $"uses <{section}>";
            }
        }

        var keywords = test.Get("info", "keywords")?.Lines.Select(l => l.Trim()).ToList() ?? new List<string>();
        foreach (var kw in new[] { "libtest", "unittest", "tunittest" })
        {
            if (keywords.Contains(kw, StringComparer.OrdinalIgnoreCase)) return $"keyword {kw} (runs a C test tool, not curl)";
        }

        var command = test.Get("client", "command");
        if (command == null) return "no command";
        if (command.Attr("type") != null) return $"command type={command.Attr("type")}";

        var disable = test.Get("client", "disable");
        if (disable != null) return "disabled: " + disable.RawText.Trim();

        if (test.Sections.Any(s => s.Lines.Any(l => l.Contains("%include", StringComparison.Ordinal))))
        {
            return "uses %include";
        }

        var features = test.Get("client", "features");
        if (features != null)
        {
            foreach (var raw in features.Lines.Select(l => l.Trim()).Where(l => l.Length > 0))
            {
                var negate = raw.StartsWith("!", StringComparison.Ordinal);
                var feature = negate ? raw.Substring(1) : raw;
                var has = Features.Contains(feature);
                if (negate ? has : !has)
                {
                    return "needs feature: " + raw;
                }
            }
        }

        return null;
    }

    private static Dictionary<string, string> BuildVariables(int number, int port, string root, string logDir)
    {
        var vars = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["HOSTIP"] = "127.0.0.1",
            ["HOST6IP"] = "[::1]",
            ["CLIENTIP"] = "127.0.0.1",
            ["CLIENT6IP"] = "[::1]",
            ["CLIENT6IP-NB"] = "::1",
            ["HTTPPORT"] = port.ToString(),
            ["HTTP6PORT"] = port.ToString(),
            ["HTTPSPORT"] = port.ToString(),
            ["HTTP2PORT"] = port.ToString(),
            ["HTTPTLSPORT"] = port.ToString(),
            ["HTTPTLS6PORT"] = port.ToString(),
            ["HTTPSPROXYPORT"] = port.ToString(),
            ["PROXYPORT"] = port.ToString(),
            ["FTPPORT"] = port.ToString(),
            ["FTP6PORT"] = port.ToString(),
            ["FTPSPORT"] = port.ToString(),
            ["NOLISTENPORT"] = FindClosedPort().ToString(),
            ["TESTNUMBER"] = number.ToString(),
            ["VERSION"] = CurlVersion,
            ["VERNUM"] = CurlVersion,
            ["CURL"] = "curl",
            ["LOGDIR"] = "log", // relative, like curl's globalconfig.pm; the command runs with cwd = %PWD
            ["PWD"] = root,
            ["POSIX_PWD"] = root,
            ["FILE_PWD"] = root,
            ["SRCDIR"] = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar),
            ["USER"] = Environment.UserName,
            ["DATE"] = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            ["DEV_NULL"] = OperatingSystem.IsWindows() ? "NUL" : "/dev/null",
            ["HTTPUNIXPATH"] = Path.Combine(root, "http.sock"),
            ["FTPTIME2"] = "4",
        };
        return vars;
    }

    private static int FindClosedPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    private static string Normalize(string s) => s.Replace("\r\n", "\n");

    /// <summary>runtests.pl's <c>mode="warn"</c>: per line, drop the first "Warning: ", one CR and turn the newline into a space.</summary>
    private static string UnwrapWarnings(string text) =>
        string.Concat(PerlLineRule.SplitKeepingNewlines(text).Select(l =>
        {
            var w = l.IndexOf("Warning: ", StringComparison.Ordinal);
            if (w >= 0) l = l.Remove(w, "Warning: ".Length);
            var cr = l.IndexOf('\r');
            if (cr >= 0) l = l.Remove(cr, 1);
            var nl = l.IndexOf('\n');
            return nl >= 0 ? l.Remove(nl, 1).Insert(nl, " ") : l;
        }));

    private static string FirstLine(string s)
    {
        var nl = s.IndexOf('\n');
        return nl < 0 ? s : s.Substring(0, nl);
    }
}

/// <summary>
/// Compares HTTP dumps (requests, responses, or files containing them). HttpClient does not
/// let us control header ordering, so the lenient comparison requires request/status lines to
/// match in order and all other lines to match as a multiset; <see cref="CompareResult.Strict"/>
/// reports whether the dumps were byte-identical as well.
/// </summary>
public static class HttpDumpComparer
{
    private static readonly Regex StartLine = new(@"^(?:[A-Z]+ \S+ HTTP/\d(?:\.\d)?|HTTP/\d(?:\.\d)? \d{3})", RegexOptions.Compiled);

    public readonly record struct CompareResult(bool Match, bool Strict, string Message);

    private static readonly Regex Alternatives = new(@"%alternatives\[([^,\]]*),([^\]]*)\]", RegexOptions.Compiled);

    /// <summary>
    /// Compares after removing <paramref name="strips"/> lines from both sides; <paramref name="stripPart"/> (the test's
    /// <c>&lt;strippart&gt;</c>) then rewrites the actual lines in place, in runtests.pl's order.
    /// </summary>
    public static CompareResult Compare(string expected, string actual, IReadOnlyList<string> strips, IReadOnlyList<PerlLineRule>? stripPart = null)
    {
        var expectedLines = Strip(Lines(expected), strips);
        List<string> actualLines;
        if (stripPart != null && stripPart.Count > 0)
        {
            actual = PerlLineRule.ApplyStripPart(StripKeepingNewlines(actual, strips), stripPart);
            actualLines = Lines(actual);
        }
        else
        {
            actualLines = Strip(Lines(actual), strips);
        }

        // %days[N] expands to %alternatives[a,b]: pick whichever alternative the actual output used
        if (expectedLines.Any(l => l.Contains("%alternatives[", StringComparison.Ordinal)))
        {
            var actualSet = new HashSet<string>(actualLines, StringComparer.Ordinal);
            expectedLines = expectedLines.Select(l =>
            {
                if (!l.Contains("%alternatives[", StringComparison.Ordinal)) return l;
                var first = Alternatives.Replace(l, m => m.Groups[1].Value);
                var second = Alternatives.Replace(l, m => m.Groups[2].Value);
                return actualSet.Contains(second) ? second : first;
            }).ToList();
            expected = string.Join("\n", expectedLines) + "\n";
        }

        var strict = expected == actual;
        if (expectedLines.SequenceEqual(actualLines))
        {
            return new CompareResult(true, strict, string.Empty);
        }

        var expectedStarts = expectedLines.Where(l => StartLine.IsMatch(l)).ToList();
        var actualStarts = actualLines.Where(l => StartLine.IsMatch(l)).ToList();
        if (!expectedStarts.SequenceEqual(actualStarts))
        {
            return new CompareResult(false, false, Describe(expectedLines, actualLines));
        }

        var expectedRest = expectedLines.Where(l => !StartLine.IsMatch(l)).OrderBy(l => l, StringComparer.Ordinal).ToList();
        var actualRest = actualLines.Where(l => !StartLine.IsMatch(l)).OrderBy(l => l, StringComparer.Ordinal).ToList();
        if (expectedRest.SequenceEqual(actualRest))
        {
            return new CompareResult(true, false, string.Empty);
        }

        return new CompareResult(false, false, Describe(expectedLines, actualLines));
    }

    public static string Describe(string expected, string actual) => Describe(Lines(expected), Lines(actual));

    private static string Describe(List<string> expected, List<string> actual)
    {
        var missing = expected.Except(actual).Take(6).Select(l => "-" + Show(l)).ToList();
        var extra = actual.Except(expected).Take(6).Select(l => "+" + Show(l)).ToList();
        var parts = new List<string>();
        if (missing.Count > 0) parts.Add("missing " + string.Join(" | ", missing));
        if (extra.Count > 0) parts.Add("unexpected " + string.Join(" | ", extra));
        if (parts.Count == 0) parts.Add($"line order/count differs (expected {expected.Count} lines, got {actual.Count})");
        return string.Join("; ", parts);
    }

    private static string Show(string line)
    {
        var s = line.Replace("\r", "\\r").Replace("\t", "\\t");
        return s.Length > 100 ? s.Substring(0, 97) + "..." : s;
    }

    private static List<string> Lines(string text)
    {
        var normalized = text.Replace("\r\n", "\n");
        var lines = normalized.Split('\n').ToList();
        if (lines.Count > 0 && lines[^1].Length == 0) lines.RemoveAt(lines.Count - 1);
        return lines;
    }

    /// <summary><see cref="Strip"/> on raw text: the same lines go, the rest keep their exact line endings.</summary>
    private static string StripKeepingNewlines(string text, IReadOnlyList<string> strips)
    {
        if (strips.Count == 0) return text;
        var kept = PerlLineRule.SplitKeepingNewlines(text)
            .Where(l => Strip(new List<string> { l.EndsWith("\r\n", StringComparison.Ordinal) ? l[..^2] : l.TrimEnd('\n') }, strips).Count == 1);
        return string.Concat(kept);
    }

    private static List<string> Strip(List<string> lines, IReadOnlyList<string> strips)
    {
        if (strips.Count == 0) return lines;
        var regexes = strips.Select(s =>
        {
            try { return new Regex(s); }
            catch (ArgumentException) { return null; }
        }).Where(r => r != null).ToList();
        return lines.Where(l => !regexes.Any(r => r!.IsMatch(l))).ToList();
    }
}
