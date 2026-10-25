using System.Text;
using System.Text.RegularExpressions;

namespace CurlDotNet.CurlSuite;

/// <summary>
/// One section of a curl test file, e.g. <c>&lt;data crlf="headers"&gt;</c> inside <c>&lt;reply&gt;</c>.
/// Content is kept as lines without their newline; every line in the file is newline-terminated.
/// </summary>
public sealed class CurlTestSection
{
    public CurlTestSection(string part, string name, Dictionary<string, string> attributes)
    {
        Part = part;
        Name = name;
        Attributes = attributes;
    }

    /// <summary>Main part the section belongs to: info, reply, client or verify.</summary>
    public string Part { get; }

    /// <summary>Tag name, e.g. data, data2, protocol, command.</summary>
    public string Name { get; }

    public Dictionary<string, string> Attributes { get; }

    public List<string> Lines { get; } = new();

    public string? Attr(string name) => Attributes.TryGetValue(name, out var v) ? v : null;

    public bool HasAttr(string name, string value) =>
        string.Equals(Attr(name), value, StringComparison.OrdinalIgnoreCase);

    /// <summary>Raw text with every line newline-terminated (before preprocessing).</summary>
    public string RawText => Lines.Count == 0 ? string.Empty : string.Join("\n", Lines) + "\n";
}

/// <summary>
/// Line-based parser for curl's test file format (docs/tests/FILEFORMAT.md). The files look
/// like XML but section bodies routinely contain unescaped HTML, so, exactly like curl's own
/// getpart.pm, a sub-section only ends at its own closing tag and everything in between is data.
/// </summary>
public sealed class CurlTestFile
{
    private static readonly HashSet<string> MainParts = new(StringComparer.Ordinal)
    {
        "testcase", "info", "reply", "client", "verify"
    };

    private static readonly Regex OpenTag = new(@"^<([A-Za-z][\w-]*)((?:\s+[\w-]+=(?:""[^""]*""|'[^']*'))*)\s*>\s*$", RegexOptions.Compiled);
    private static readonly Regex OneLineTag = new(@"^<([A-Za-z][\w-]*)((?:\s+[\w-]+=(?:""[^""]*""|'[^']*'))*)\s*>(.*)</\1>\s*$", RegexOptions.Compiled);
    private static readonly Regex CloseTag = new(@"^</([A-Za-z][\w-]*)>\s*$", RegexOptions.Compiled);
    private static readonly Regex AttrPattern = new(@"([\w-]+)=(?:""([^""]*)""|'([^']*)')", RegexOptions.Compiled);

    public CurlTestFile(int number, string path, List<CurlTestSection> sections)
    {
        Number = number;
        Path = path;
        Sections = sections;
    }

    public int Number { get; }
    public string Path { get; }
    public List<CurlTestSection> Sections { get; }

    public CurlTestSection? Get(string part, string name) =>
        Sections.FirstOrDefault(s => s.Part == part && s.Name == name);

    public bool Has(string part, string name) => Get(part, name) != null;

    public IEnumerable<CurlTestSection> All(string part) => Sections.Where(s => s.Part == part);

    public string Name => Get("client", "name")?.RawText.Trim() ?? $"test{Number}";

    public static CurlTestFile Load(string path)
    {
        // Latin-1 keeps every byte of the file intact so %hex data and binary bodies survive.
        var text = File.ReadAllText(path, Encoding.Latin1);
        var fileName = System.IO.Path.GetFileName(path);
        var number = int.Parse(fileName.Substring(4));
        return Parse(number, path, text);
    }

    public static CurlTestFile Parse(int number, string path, string text)
    {
        var sections = new List<CurlTestSection>();
        var lines = text.Replace("\r\n", "\n").Split('\n');
        if (lines.Length > 0 && lines[^1].Length == 0)
        {
            lines = lines[..^1]; // trailing newline at end of file
        }

        string mainPart = "testcase";
        CurlTestSection? current = null;

        foreach (var line in lines)
        {
            if (current != null)
            {
                var close = CloseTag.Match(line);
                if (close.Success && close.Groups[1].Value == current.Name)
                {
                    sections.Add(current);
                    current = null;
                }
                else
                {
                    current.Lines.Add(line);
                }
                continue;
            }

            if (line.StartsWith("<?xml", StringComparison.Ordinal) || line.Trim().Length == 0)
            {
                continue;
            }

            var oneLine = OneLineTag.Match(line);
            if (oneLine.Success && !MainParts.Contains(oneLine.Groups[1].Value))
            {
                var s = new CurlTestSection(mainPart, oneLine.Groups[1].Value, ParseAttributes(oneLine.Groups[2].Value));
                s.Lines.Add(oneLine.Groups[3].Value);
                sections.Add(s);
                continue;
            }

            var closeMain = CloseTag.Match(line);
            if (closeMain.Success)
            {
                if (MainParts.Contains(closeMain.Groups[1].Value))
                {
                    mainPart = "testcase";
                }
                continue;
            }

            var open = OpenTag.Match(line);
            if (open.Success)
            {
                var name = open.Groups[1].Value;
                if (MainParts.Contains(name))
                {
                    mainPart = name;
                }
                else
                {
                    current = new CurlTestSection(mainPart, name, ParseAttributes(open.Groups[2].Value));
                }
            }
        }

        if (current != null)
        {
            sections.Add(current); // unterminated section: keep what we have
        }

        return new CurlTestFile(number, path, sections);
    }

    private static Dictionary<string, string> ParseAttributes(string attrs)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match m in AttrPattern.Matches(attrs))
        {
            result[m.Groups[1].Value] = m.Groups[2].Success ? m.Groups[2].Value : m.Groups[3].Value;
        }
        return result;
    }
}

/// <summary>
/// Implements the preprocessing step of runtests.pl: %if/%else/%endif on features,
/// %VARIABLE substitution, %b64[...]b64%, %hex[...]hex%, %repeat[N x s]% and the
/// whitespace/special-character macros. All text is Latin-1 so a char is a byte.
/// </summary>
public sealed class CurlTestPreprocessor
{
    private readonly Dictionary<string, string> _variables;
    private readonly HashSet<string> _features;
    private readonly List<KeyValuePair<string, string>> _orderedVariables;

    public CurlTestPreprocessor(Dictionary<string, string> variables, HashSet<string> features)
    {
        _variables = variables;
        _features = features;
        _orderedVariables = variables.OrderByDescending(kv => kv.Key.Length).ToList();
    }

    public string Process(string text)
    {
        text = ApplyConditionals(text);
        text = SubstituteVariables(text);
        text = ApplyEncodingMacros(text);
        text = ApplyWhitespaceMacros(text);
        return text;
    }

    public string ProcessSection(CurlTestSection section)
    {
        var text = Process(section.RawText);
        if (section.HasAttr("nonewline", "yes") && text.EndsWith("\n", StringComparison.Ordinal))
        {
            text = text.Substring(0, text.Length - 1);
        }

        var crlf = section.Attr("crlf");
        if (string.Equals(crlf, "yes", StringComparison.OrdinalIgnoreCase))
        {
            text = ToCrlf(text, headersOnly: false);
        }
        else if (string.Equals(crlf, "headers", StringComparison.OrdinalIgnoreCase))
        {
            text = ToCrlf(text, headersOnly: true);
        }

        return text;
    }

    /// <summary>
    /// Converts LF to CRLF like runtests' <c>subnewlines()</c> (tests/testutil.pm): for every line when
    /// <paramref name="headersOnly"/> is false (<c>crlf="yes"</c>), otherwise (<c>crlf="headers"</c>) only for
    /// lines that look like an HTTP status line, a request line or a <c>Name: value</c> header without a CR,
    /// plus a blank line that directly follows such a line. Every other line keeps its bytes, so a header
    /// line that starts with a space (curl test 1473) stays LF-terminated, exactly as curl's sws sends it.
    /// </summary>
    public static string ToCrlf(string text, bool headersOnly)
    {
        if (!headersOnly)
        {
            return text.Replace("\r\n", "\n").Replace("\n", "\r\n");
        }

        var sb = new StringBuilder(text.Length + 64);
        var prevUpdate = false;
        var start = 0;
        while (start < text.Length)
        {
            var nl = text.IndexOf('\n', start);
            var line = nl < 0 ? text.Substring(start) : text.Substring(start, nl + 1 - start);
            start = nl < 0 ? text.Length : nl + 1;
            if (SubNewlinesMatches(line))
            {
                sb.Append(EnforceCrlf(line));
                prevUpdate = true;
            }
            else
            {
                sb.Append(line == "\n" && prevUpdate ? "\r\n" : line);
                prevUpdate = false;
            }
        }
        return sb.ToString();
    }

    private static readonly Regex StatusLine = new(@"^HTTP/(1.1|1.0|2|3) ([1-5]|9)[^\r]*\z", RegexOptions.CultureInvariant);
    private static readonly Regex RequestLine = new(@"^(GET|HEAD|POST|PUT|DELETE|CONNECT) \S+ HTTP/\d+(\.\d+)?", RegexOptions.CultureInvariant);
    private static readonly Regex RtspLine = new(@"^(SETUP|GET_PARAMETER|OPTIONS|ANNOUNCE|DESCRIBE) \S+ RTSP/\d+(\.\d+)?", RegexOptions.CultureInvariant);
    private static readonly Regex HeaderLine = new(@"^[a-z0-9_-]+: [^\r]*\z", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private static readonly Regex CurlError = new(@"^curl: \(\d+\) ", RegexOptions.CultureInvariant);

    private static bool SubNewlinesMatches(string line) =>
        StatusLine.IsMatch(line) || RequestLine.IsMatch(line) || RtspLine.IsMatch(line)
        || (HeaderLine.IsMatch(line) && !CurlError.IsMatch(line));

    /// <summary>perl's <c>s/\x0d*\x0a/\x0d\x0a/</c>: the first run of CRs plus LF becomes one CRLF.</summary>
    private static string EnforceCrlf(string line)
    {
        var m = Regex.Match(line, "\r*\n");
        return m.Success ? line.Substring(0, m.Index) + "\r\n" + line.Substring(m.Index + m.Length) : line;
    }

    private string ApplyConditionals(string text)
    {
        if (!text.Contains("%if", StringComparison.Ordinal))
        {
            return text;
        }

        var sb = new StringBuilder(text.Length);
        var stack = new Stack<bool>(); // whether the current block is emitted
        var emitting = true;
        var start = 0;
        while (start <= text.Length)
        {
            var nl = text.IndexOf('\n', start);
            var line = nl < 0 ? text.Substring(start) : text.Substring(start, nl - start);
            var last = nl < 0;

            if (line.StartsWith("%if ", StringComparison.Ordinal))
            {
                var cond = line.Substring(4).Trim();
                var negate = cond.StartsWith("!", StringComparison.Ordinal);
                if (negate) cond = cond.Substring(1);
                var has = _features.Contains(cond);
                stack.Push(emitting);
                emitting = emitting && (negate ? !has : has);
            }
            else if (line.StartsWith("%else", StringComparison.Ordinal))
            {
                var parent = stack.Count > 0 ? stack.Peek() : true;
                emitting = parent && !emitting;
            }
            else if (line.StartsWith("%endif", StringComparison.Ordinal))
            {
                emitting = stack.Count > 0 ? stack.Pop() : true;
            }
            else if (emitting)
            {
                sb.Append(line);
                if (!last) sb.Append('\n');
            }

            if (last) break;
            start = nl + 1;
        }
        return sb.ToString();
    }

    private string SubstituteVariables(string text)
    {
        if (text.IndexOf('%') < 0)
        {
            return text;
        }

        foreach (var kv in _orderedVariables)
        {
            text = text.Replace("%" + kv.Key, kv.Value);
        }
        return text;
    }

    private static readonly Regex B64 = new(@"%b64\[(.*?)\]b64%", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex Hex = new(@"%hex\[(.*?)\]hex%", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex Repeat = new(@"%repeat\[(\d+) x (.*?)\]%", RegexOptions.Compiled | RegexOptions.Singleline);
    private static readonly Regex PercentByte = new(@"%([0-9A-Fa-f]{2})", RegexOptions.Compiled);

    private static string DecodePercentBytes(string s) =>
        PercentByte.Replace(s, m => ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString());

    private static readonly Regex Days = new(@"%days\[(\d+)\]", RegexOptions.Compiled);

    private string ApplyEncodingMacros(string text)
    {
        text = Days.Replace(text, m =>
        {
            // testutil.pm daysit(): now + N days (+30 s), aligned down to a minute, and the next minute as a second option
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var d = (long.Parse(m.Groups[1].Value) * 24 * 3600 + now + 30) / 60 * 60;
            return $"%alternatives[{d},{d + 60}]";
        });
        text = B64.Replace(text, m =>
        {
            var inner = DecodePercentBytes(m.Groups[1].Value);
            return Convert.ToBase64String(Encoding.Latin1.GetBytes(inner));
        });
        text = Hex.Replace(text, m => DecodePercentBytes(m.Groups[1].Value));
        text = Repeat.Replace(text, m =>
        {
            var count = int.Parse(m.Groups[1].Value);
            var unit = DecodePercentBytes(m.Groups[2].Value);
            var sb = new StringBuilder(unit.Length * count);
            for (var i = 0; i < count; i++) sb.Append(unit);
            return sb.ToString();
        });
        return text;
    }

    private static string ApplyWhitespaceMacros(string text)
    {
        if (text.IndexOf('%') < 0) return text;
        return text
            .Replace("%CR\n", "\r\n").Replace("%CR", "\r")
            .Replace("%SP", " ")
            .Replace("%TAB", "\t")
            .Replace("%AMP", "&")
            .Replace("%GT", ">")
            .Replace("%LT", "<");
    }
}
