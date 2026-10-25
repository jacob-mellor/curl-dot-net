using System.Text;
using System.Text.RegularExpressions;

namespace CurlDotNet.CurlSuite;

/// <summary>
/// One line of a <c>&lt;stripfile&gt;</c>/<c>&lt;stripfileN&gt;</c>/<c>&lt;strippart&gt;</c> section: a perl statement that
/// runtests.pl <c>eval</c>s once per line of the output with <c>$_</c> holding the line (newline included).
/// Only the forms curl's tests use are replayed: <c>s/regex/replacement/flags</c> with any delimiter, and
/// <c>$_ = '' if CONDITION</c> where the condition is <c>/regex/</c> or <c>$_ =~ /regex/</c> / <c>$_ !~ /regex/</c>
/// terms joined with <c>and</c>/<c>or</c>/<c>&amp;&amp;</c>/<c>||</c>. Anything else is reported as unsupported so the
/// test is skipped instead of being compared with a guess.
/// </summary>
public sealed class PerlLineRule
{
    private readonly Regex? _regex;
    private readonly string _replacement = string.Empty;
    private readonly bool _global;
    private readonly Func<string, bool>? _clearIf;

    private PerlLineRule(Regex regex, string replacement, bool global)
    {
        _regex = regex;
        _replacement = replacement;
        _global = global;
    }

    private PerlLineRule(Func<string, bool> clearIf)
    {
        _clearIf = clearIf;
    }

    /// <summary>The statement as written in the test file.</summary>
    public string Source { get; private init; } = string.Empty;

    /// <summary>Runs the statement on one line (with its newline) and returns the new <c>$_</c>.</summary>
    public string Apply(string line)
    {
        if (_clearIf != null)
        {
            return _clearIf(line) ? string.Empty : line;
        }

        return _global ? _regex!.Replace(line, _replacement) : _regex!.Replace(line, _replacement, 1);
    }

    /// <summary>
    /// Parses every statement of a section. Blank lines and <c># comments</c> are no-ops in perl and are dropped.
    /// </summary>
    /// <returns>The rules, or null with <paramref name="error"/> naming the first statement that cannot be replayed.</returns>
    public static List<PerlLineRule>? ParseAll(IEnumerable<string> statements, out string? error)
    {
        var rules = new List<PerlLineRule>();
        foreach (var raw in statements)
        {
            var text = raw.Trim();
            if (text.Length == 0 || text[0] == '#')
            {
                continue;
            }

            var rule = TryParse(text, out error);
            if (rule == null)
            {
                return null;
            }

            rules.Add(rule);
        }

        error = null;
        return rules;
    }

    /// <summary>Parses one statement; null with <paramref name="error"/> when it is not a supported form.</summary>
    public static PerlLineRule? TryParse(string statement, out string? error)
    {
        error = null;
        var text = statement.Trim().TrimEnd(';').TrimEnd();

        if (text.StartsWith("s", StringComparison.Ordinal) && text.Length > 1 && IsDelimiter(text[1]))
        {
            return ParseSubstitution(text, statement, out error);
        }

        var clear = Regex.Match(text, @"^\$_\s*=\s*(?:''|"""")\s+if\s*(.+)$");
        if (clear.Success)
        {
            var condition = ParseCondition(clear.Groups[1].Value.Trim(), out error);
            if (condition == null)
            {
                return null;
            }

            return new PerlLineRule(condition) { Source = statement };
        }

        error = $"unsupported perl statement '{statement}'";
        return null;
    }

    private static bool IsDelimiter(char c) => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c) && c != '_';

    private static PerlLineRule? ParseSubstitution(string text, string statement, out string? error)
    {
        error = null;
        var open = text[1];
        var close = ClosingBracket(open);
        var pos = 2;
        var pattern = ReadDelimited(text, ref pos, open, close);
        if (pattern == null)
        {
            error = $"unterminated pattern in '{statement}'";
            return null;
        }

        if (close != open)
        {
            // s{pat}{repl}: the replacement has its own pair of brackets, optionally after whitespace
            while (pos < text.Length && char.IsWhiteSpace(text[pos])) pos++;
            if (pos >= text.Length || text[pos] != open)
            {
                error = $"cannot read the replacement of '{statement}'";
                return null;
            }
            pos++;
        }

        var replacement = ReadDelimited(text, ref pos, open, close);
        if (replacement == null)
        {
            error = $"unterminated replacement in '{statement}'";
            return null;
        }

        var flags = text.Substring(pos);
        var options = RegexOptions.CultureInvariant;
        var global = false;
        foreach (var f in flags)
        {
            switch (f)
            {
                case 'g': global = true; break;
                case 'i': options |= RegexOptions.IgnoreCase; break;
                case 'x': options |= RegexOptions.IgnorePatternWhitespace; break;
                case 's': options |= RegexOptions.Singleline; break;
                case 'm': options |= RegexOptions.Multiline; break;
                default:
                    error = $"unsupported s/// flag '{f}' in '{statement}'";
                    return null;
            }
        }

        var netPattern = TranslatePattern(UnescapeDelimiter(pattern, open, close), out error);
        if (netPattern == null)
        {
            error += $" in '{statement}'";
            return null;
        }

        var netReplacement = TranslateReplacement(replacement, open, close, out error);
        if (netReplacement == null)
        {
            error += $" in '{statement}'";
            return null;
        }

        try
        {
            return new PerlLineRule(new Regex(netPattern, options), netReplacement, global) { Source = statement };
        }
        catch (ArgumentException ex)
        {
            error = $"pattern of '{statement}' is not a .NET regex: {ex.Message}";
            return null;
        }
    }

    private static char ClosingBracket(char open) => open switch
    {
        '{' => '}',
        '(' => ')',
        '[' => ']',
        '<' => '>',
        _ => open,
    };

    /// <summary>Reads up to the closing delimiter (honouring backslash escapes and nested brackets); returns the raw text.</summary>
    private static string? ReadDelimited(string text, ref int pos, char open, char close)
    {
        var sb = new StringBuilder();
        var depth = 0;
        while (pos < text.Length)
        {
            var c = text[pos++];
            if (c == '\\' && pos < text.Length)
            {
                sb.Append(c).Append(text[pos++]);
                continue;
            }

            if (close != open && c == open)
            {
                depth++;
            }
            else if (c == close)
            {
                if (depth == 0)
                {
                    return sb.ToString();
                }
                depth--;
            }

            sb.Append(c);
        }

        return null;
    }

    /// <summary>In perl <c>\/</c> inside <c>s/.../</c> is just the delimiter character; keep every other escape.</summary>
    private static string UnescapeDelimiter(string pattern, char open, char close)
    {
        if (char.IsLetterOrDigit(open))
        {
            return pattern;
        }

        var sb = new StringBuilder(pattern.Length);
        for (var i = 0; i < pattern.Length; i++)
        {
            var c = pattern[i];
            if (c == '\\' && i + 1 < pattern.Length && (pattern[i + 1] == open || pattern[i + 1] == close) && !IsRegexMeta(pattern[i + 1]))
            {
                sb.Append(pattern[++i]);
                continue;
            }

            if (c == '\\' && i + 1 < pattern.Length)
            {
                sb.Append(c).Append(pattern[++i]);
                continue;
            }

            sb.Append(c);
        }

        return sb.ToString();
    }

    private static bool IsRegexMeta(char c) => "\\^$.|?*+()[]{}".IndexOf(c) >= 0;

    /// <summary>
    /// perl and .NET agree on the regex syntax these tests use (classes, \d \s \w, anchors, lookahead, groups); refuse
    /// the perl-only constructs instead of mistranslating them.
    /// </summary>
    private static string? TranslatePattern(string pattern, out string? error)
    {
        error = null;
        if (pattern.Contains("\\Q", StringComparison.Ordinal) || pattern.Contains("\\E", StringComparison.Ordinal))
        {
            error = "perl \\Q...\\E quoting is not replayed";
            return null;
        }

        if (pattern.Contains("[[:", StringComparison.Ordinal))
        {
            error = "POSIX [[:class:]] is not replayed";
            return null;
        }

        if (Regex.IsMatch(pattern, @"(?<!\\)[$@][A-Za-z_{]"))
        {
            // $var / @array interpolation inside the pattern ("$" before ")", "|" or the end is an anchor in perl too)
            error = "variable interpolation in the pattern is not replayed";
            return null;
        }

        return pattern;
    }

    /// <summary>
    /// Turns a perl replacement (a double-quoted string: <c>$1</c>, <c>${1}</c>, <c>$&amp;</c>, <c>\n</c>, <c>\t</c>,
    /// <c>\\</c>, <c>\/</c>) into a .NET replacement pattern, escaping literal <c>$</c> as <c>$$</c>.
    /// </summary>
    private static string? TranslateReplacement(string replacement, char open, char close, out string? error)
    {
        error = null;
        var sb = new StringBuilder();
        for (var i = 0; i < replacement.Length; i++)
        {
            var c = replacement[i];
            if (c == '\\' && i + 1 < replacement.Length)
            {
                var n = replacement[++i];
                switch (n)
                {
                    case 'n': sb.Append('\n'); break;
                    case 'r': sb.Append('\r'); break;
                    case 't': sb.Append('\t'); break;
                    case '$': sb.Append("$$"); break;
                    default:
                        if (char.IsLetterOrDigit(n))
                        {
                            error = $"replacement escape \\{n} is not replayed";
                            return null;
                        }
                        sb.Append(n); // \\ \/ \: \@ ... are the character itself
                        break;
                }
                continue;
            }

            if (c == '$')
            {
                var group = Regex.Match(replacement.Substring(i), @"^\$(?:(\d+)|\{(\d+)\}|(&))");
                if (!group.Success)
                {
                    error = "variable interpolation in the replacement is not replayed";
                    return null;
                }

                sb.Append(group.Groups[3].Success ? "$0" : "${" + (group.Groups[1].Success ? group.Groups[1].Value : group.Groups[2].Value) + "}");
                i += group.Length - 1;
                continue;
            }

            if (c == '@' && i + 1 < replacement.Length && (char.IsLetter(replacement[i + 1]) || replacement[i + 1] == '_' || replacement[i + 1] == '{'))
            {
                error = "array interpolation in the replacement is not replayed";
                return null;
            }

            sb.Append(c);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Parses <c>/re/</c>, <c>($_ =~ /re/)</c>, <c>($_ !~ /re/)</c>, <c>!/re/</c> terms joined by <c>and</c>, <c>or</c>,
    /// <c>&amp;&amp;</c>, <c>||</c> (no mixing of and/or, which would need precedence), with optional surrounding parentheses.
    /// </summary>
    private static Func<string, bool>? ParseCondition(string condition, out string? error)
    {
        error = null;
        var text = StripOuterParens(condition.Trim());
        var andParts = SplitTopLevel(text, new[] { " and ", "&&" });
        var orParts = SplitTopLevel(text, new[] { " or ", "||" });
        if (andParts.Count > 1 && orParts.Count > 1)
        {
            error = $"condition mixes and/or: '{condition}'";
            return null;
        }

        var useAnd = orParts.Count <= 1;
        var parts = useAnd ? andParts : orParts;
        var terms = new List<Func<string, bool>>();
        foreach (var part in parts)
        {
            var term = ParseTerm(StripOuterParens(part.Trim()), out error);
            if (term == null)
            {
                error ??= $"unsupported condition term '{part.Trim()}'";
                return null;
            }
            terms.Add(term);
        }

        return useAnd ? line => terms.All(t => t(line)) : line => terms.Any(t => t(line));
    }

    private static Func<string, bool>? ParseTerm(string term, out string? error)
    {
        error = null;
        var negate = false;
        var m = Regex.Match(term, @"^\$_\s*(=~|!~)\s*(.+)$");
        if (m.Success)
        {
            negate = m.Groups[1].Value == "!~";
            term = m.Groups[2].Value.Trim();
        }
        else if (term.StartsWith("!", StringComparison.Ordinal))
        {
            negate = true;
            term = term.Substring(1).Trim();
        }

        var match = Regex.Match(term, @"^(?:m(\W)|(/))");
        if (!match.Success)
        {
            return null;
        }

        var open = match.Groups[1].Success ? match.Groups[1].Value[0] : '/';
        var close = ClosingBracket(open);
        var pos = match.Length;
        var pattern = ReadDelimited(term, ref pos, open, close);
        if (pattern == null)
        {
            return null;
        }

        var options = RegexOptions.CultureInvariant;
        foreach (var f in term.Substring(pos))
        {
            switch (f)
            {
                case 'i': options |= RegexOptions.IgnoreCase; break;
                case 'x': options |= RegexOptions.IgnorePatternWhitespace; break;
                case 's': options |= RegexOptions.Singleline; break;
                case 'm': options |= RegexOptions.Multiline; break;
                default:
                    error = $"unsupported match flag '{f}'";
                    return null;
            }
        }

        var netPattern = TranslatePattern(UnescapeDelimiter(pattern, open, close), out error);
        if (netPattern == null)
        {
            return null;
        }

        Regex regex;
        try
        {
            regex = new Regex(netPattern, options);
        }
        catch (ArgumentException ex)
        {
            error = $"pattern /{pattern}/ is not a .NET regex: {ex.Message}";
            return null;
        }

        return negate ? line => !regex.IsMatch(line) : line => regex.IsMatch(line);
    }

    private static string StripOuterParens(string text)
    {
        while (text.Length >= 2 && text[0] == '(' && text[^1] == ')' && MatchingParen(text, 0) == text.Length - 1)
        {
            text = text.Substring(1, text.Length - 2).Trim();
        }
        return text;
    }

    private static int MatchingParen(string text, int open)
    {
        var depth = 0;
        var inRegex = false;
        for (var i = open; i < text.Length; i++)
        {
            var c = text[i];
            if (c == '\\') { i++; continue; }
            if (c == '/') { inRegex = !inRegex; continue; }
            if (inRegex) continue;
            if (c == '(') depth++;
            else if (c == ')' && --depth == 0) return i;
        }
        return -1;
    }

    /// <summary>Splits on any separator that is outside parentheses and outside /regex/ literals.</summary>
    private static List<string> SplitTopLevel(string text, string[] separators)
    {
        var parts = new List<string>();
        var depth = 0;
        var inRegex = false;
        var start = 0;
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            if (c == '\\') { i++; continue; }
            if (c == '/') { inRegex = !inRegex; continue; }
            if (inRegex) continue;
            if (c == '(') { depth++; continue; }
            if (c == ')') { depth--; continue; }
            if (depth != 0) continue;
            foreach (var sep in separators)
            {
                if (string.CompareOrdinal(text, i, sep, 0, sep.Length) == 0)
                {
                    parts.Add(text.Substring(start, i - start));
                    start = i + sep.Length;
                    i = start - 1;
                    break;
                }
            }
        }
        parts.Add(text.Substring(start));
        return parts;
    }

    /// <summary>
    /// runtests.pl's stripfile loop: every rule runs on every line (newline included) in order, and a line that
    /// ends up empty (or "0", which perl also treats as false) is dropped.
    /// </summary>
    public static string ApplyStripFile(string text, IReadOnlyList<PerlLineRule> rules)
    {
        if (rules.Count == 0)
        {
            return text;
        }

        var lines = SplitKeepingNewlines(text);
        foreach (var rule in rules)
        {
            var next = new List<string>(lines.Count);
            foreach (var line in lines)
            {
                var result = rule.Apply(line);
                if (result.Length > 0 && result != "0")
                {
                    next.Add(result);
                }
            }
            lines = next;
        }

        return string.Concat(lines);
    }

    /// <summary>runtests.pl's strippart loop: every rule runs on every line in place (an emptied line contributes nothing).</summary>
    public static string ApplyStripPart(string text, IReadOnlyList<PerlLineRule> rules)
    {
        if (rules.Count == 0)
        {
            return text;
        }

        var lines = SplitKeepingNewlines(text);
        foreach (var rule in rules)
        {
            for (var i = 0; i < lines.Count; i++)
            {
                lines[i] = rule.Apply(lines[i]);
            }
        }

        return string.Concat(lines);
    }

    /// <summary>Splits like perl's line reading: every line keeps its "\n"; a last line without one is kept as is.</summary>
    public static List<string> SplitKeepingNewlines(string text)
    {
        var lines = new List<string>();
        var start = 0;
        while (start < text.Length)
        {
            var nl = text.IndexOf('\n', start);
            if (nl < 0)
            {
                lines.Add(text.Substring(start));
                break;
            }
            lines.Add(text.Substring(start, nl - start + 1));
            start = nl + 1;
        }
        return lines;
    }
}
