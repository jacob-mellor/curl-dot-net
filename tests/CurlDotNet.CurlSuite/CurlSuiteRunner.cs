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
        "tool", "stdin", "servercmd", "precheck", "postcheck", "strippart", "killserver", "connect", "proxy", "socks", "dns",
    };

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

        var replies = new Dictionary<int, string>();
        foreach (var section in test.All("reply"))
        {
            var m = Regex.Match(section.Name, @"^data(\d*)$");
            if (!m.Success) continue;
            var part = m.Groups[1].Value.Length == 0 ? 0 : int.Parse(m.Groups[1].Value);
            var text = pre.ProcessSection(section);
            if (section.Attr("crlf") == null)
            {
                // sws serves bytes verbatim and curl tolerates bare LF; HttpClient does not, so
                // normalise the header block (never the body) to CRLF.
                text = CurlTestPreprocessor.ToCrlf(text, headersOnly: true);
            }
            replies[part] = text;
        }
        server.SetReplies(replies);

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

        foreach (var section in test.All("client").Where(s => s.Name == "setenv"))
        {
            foreach (var line in section.Lines)
            {
                var eq = line.IndexOf('=');
                if (eq > 0) Environment.SetEnvironmentVariable(line.Substring(0, eq), pre.Process(line.Substring(eq + 1)));
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

        var prefix = new StringBuilder("curl");
        if (addOutput) prefix.Append(" --output '").Append(outFile.Replace("'", "'\\''")).Append('\'');
        if (addInclude) prefix.Append(" --include");
        var command = prefix + " " + userCommand;

        var previousCwd = Directory.GetCurrentDirectory();
        int exitCode;
        string stdout = string.Empty;
        string? failure = null;
        var sw = Stopwatch.StartNew();
        try
        {
            Directory.SetCurrentDirectory(root);
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
            Directory.SetCurrentDirectory(previousCwd);
            foreach (var section in test.All("client").Where(s => s.Name == "setenv"))
            {
                foreach (var line in section.Lines)
                {
                    var eq = line.IndexOf('=');
                    if (eq > 0) Environment.SetEnvironmentVariable(line.Substring(0, eq), null);
                }
            }
        }

        // Give the server a moment to log the last request.
        await Task.Delay(30);

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
            var cmp = HttpDumpComparer.Compare(expected, actual, strips);
            if (!cmp.Match) problems.Add("protocol: " + cmp.Message);
            strict &= cmp.Strict;
        }

        // upload
        var upload = test.Get("verify", "upload");
        if (upload != null)
        {
            var expected = pre.ProcessSection(upload);
            var actual = string.Concat(server.Bodies.Select(b => Encoding.Latin1.GetString(b)));
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
            var cmp = HttpDumpComparer.Compare(expected, stdout, strips);
            if (!cmp.Match) problems.Add("stdout: " + cmp.Message);
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
        var hasStripFile = test.All("verify").Any(s => s.Name.StartsWith("stripfile", StringComparison.Ordinal));
        foreach (var section in test.All("verify").Where(s => Regex.IsMatch(s.Name, @"^file\d?$")))
        {
            if (hasStripFile) break; // perl post-processing we cannot replay
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
            var actual = File.ReadAllText(fileName, Encoding.Latin1);
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

    public static CompareResult Compare(string expected, string actual, IReadOnlyList<string> strips)
    {
        var expectedLines = Strip(Lines(expected), strips);
        var actualLines = Strip(Lines(actual), strips);

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
