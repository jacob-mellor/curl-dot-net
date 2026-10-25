/***************************************************************************
 * RequestGlob - curl's URL globbing ({a,b} sets and [1-10] / [a-z] ranges)
 *
 * Port of the behaviour of curl's src/tool_urlglob.c: sets, numeric ranges with zero
 * padding and steps, letter ranges, backslash escapes, IPv6 literals left alone, named
 * globs ({<name>a,b} referenced as #<name>) and #N placeholders in output file names.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>One URL produced by glob expansion, with the text each glob matched.</summary>
    internal sealed class RequestGlobResult
    {
        public RequestGlobResult(string url, List<string> matches, Dictionary<string, string> named)
        {
            Url = url;
            Matches = matches;
            Named = named;
        }

        /// <summary>The expanded URL (or file name).</summary>
        public string Url { get; }

        /// <summary>The text matched by glob 1, 2, 3... (for <c>#1</c>, <c>#2</c>...).</summary>
        public List<string> Matches { get; }

        /// <summary>The text matched by named globs (for <c>#&lt;name&gt;</c>).</summary>
        public Dictionary<string, string> Named { get; }
    }

    /// <summary>Expands curl URL globs.</summary>
    internal static class RequestGlob
    {
        /// <summary>curl refuses globs that would produce more URLs than this.</summary>
        private const long MaxUrls = 100000000;

        private abstract class Pattern
        {
            public string? Name { get; set; }

            public abstract IList<string> Values { get; }
        }

        private sealed class LiteralPattern : Pattern
        {
            private readonly string[] _values;

            public LiteralPattern(string text)
            {
                _values = new[] { text };
            }

            public override IList<string> Values => _values;
        }

        private sealed class ListPattern : Pattern
        {
            private readonly List<string> _values;

            public ListPattern(List<string> values)
            {
                _values = values;
            }

            public override IList<string> Values => _values;
        }

        /// <summary>
        /// True when <paramref name="text"/> contains glob syntax (<c>{</c> or <c>[</c>) that is not escaped.
        /// </summary>
        internal static bool HasGlob(string text)
        {
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '\\' && i + 1 < text.Length)
                {
                    i++;
                    continue;
                }

                if (text[i] == '{' || text[i] == '[')
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Expands every <c>{a,b}</c> set and <c>[N-M]</c> / <c>[a-z]</c> range in <paramref name="text"/>,
        /// first glob varying slowest, exactly like curl. Text without globs comes back unchanged (with
        /// glob escapes such as <c>\{</c> resolved).
        /// </summary>
        /// <exception cref="CurlException">Exit code 3 for malformed globs, curl's "bad range" errors.</exception>
        internal static List<RequestGlobResult> Expand(string text, string? command)
        {
            var patterns = ParsePatterns(text, command);

            long total = 1;
            foreach (var pattern in patterns)
            {
                total *= pattern.Values.Count;
                if (total > MaxUrls)
                {
                    throw GlobError(text, $"the glob expands to more than {MaxUrls.ToString("N0", CultureInfo.InvariantCulture)} URLs ({string.Join(" x ", patterns.Select(p => p.Values.Count))})",
                        "Split the job into several commands with smaller ranges (for example [1-1000] and [1001-2000]), or use a step such as [1-100000:100].", command);
                }
            }

            var results = new List<RequestGlobResult>();
            var indexes = new int[patterns.Count];
            while (true)
            {
                var sb = new StringBuilder();
                var matches = new List<string>();
                var named = new Dictionary<string, string>(StringComparer.Ordinal);
                for (var p = 0; p < patterns.Count; p++)
                {
                    var value = patterns[p].Values[indexes[p]];
                    sb.Append(value);
                    if (!(patterns[p] is LiteralPattern))
                    {
                        matches.Add(value);
                        if (patterns[p].Name != null)
                        {
                            named[patterns[p].Name!] = value;
                        }
                    }
                }

                results.Add(new RequestGlobResult(sb.ToString(), matches, named));

                // Advance like an odometer: the last glob varies fastest
                var position = patterns.Count - 1;
                while (position >= 0)
                {
                    indexes[position]++;
                    if (indexes[position] < patterns[position].Values.Count)
                    {
                        break;
                    }

                    indexes[position] = 0;
                    position--;
                }

                if (position < 0)
                {
                    return results;
                }
            }
        }

        /// <summary>
        /// Replaces <c>#1</c>, <c>#2</c>... and <c>#&lt;name&gt;</c> in an output file name with the text the
        /// globs matched. Numbers beyond the number of globs are left as typed, like curl.
        /// </summary>
        /// <exception cref="CurlException">Exit code 43 when <c>#&lt;name&gt;</c> refers to a glob that does not exist.</exception>
        internal static string SubstituteOutputName(string pattern, IList<string> matches, IDictionary<string, string> named, string? command)
        {
            if (pattern.IndexOf('#') < 0)
            {
                return pattern;
            }

            var sb = new StringBuilder(pattern.Length);
            for (var i = 0; i < pattern.Length; i++)
            {
                var c = pattern[i];
                if (c == '#' && i + 1 < pattern.Length)
                {
                    if (pattern[i + 1] == '<')
                    {
                        var close = pattern.IndexOf('>', i + 2);
                        if (close > i + 2)
                        {
                            var name = pattern.Substring(i + 2, close - i - 2);
                            if (!named.TryGetValue(name, out var value))
                            {
                                throw new CurlException(
                                    CurlException.Compose(
                                        $"The output name \"{pattern}\" uses #<{name}>, but the URL has no glob named \"{name}\"" +
                                        (named.Count == 0 ? " (it has no named globs at all)." : $" (its named globs are {string.Join(", ", named.Keys.Select(k => "<" + k + ">"))})."),
                                        "Name a glob in the URL, as in \"https://example.com/{<" + name + ">a,b}\" or \"https://example.com/[<" + name + ">1-9]\", or use #1, #2... for globs by position.",
                                        DocLinks.Url),
                                    43,
                                    command).WithCurlErrorMessage(GlobErrorText("no glob exists with this name", close + 1, pattern));
                            }

                            sb.Append(value);
                            i = close;
                            continue;
                        }
                    }
                    else if (char.IsDigit(pattern[i + 1]))
                    {
                        var start = i + 1;
                        var end = start;
                        while (end < pattern.Length && char.IsDigit(pattern[end]))
                        {
                            end++;
                        }

                        if (int.TryParse(pattern.Substring(start, end - start), NumberStyles.None, CultureInfo.InvariantCulture, out var number) &&
                            number >= 1 && number <= matches.Count)
                        {
                            sb.Append(matches[number - 1]);
                            i = end - 1;
                            continue;
                        }
                    }
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// curl's <c>glob_show_error()</c> text (src/tool_urlglob.c): <c>ERROR in position POS:</c>, the text, and a line with
        /// a <c>^</c> in column <paramref name="position"/> (curl pads with <c>%*s</c>, so at least one space).
        /// </summary>
        internal static string GlobErrorText(string error, int position, string text) =>
            $"{error} in position {position.ToString(CultureInfo.InvariantCulture)}:\n{text}\n{new string(' ', Math.Max(1, position - 1))}^";

        private static List<Pattern> ParsePatterns(string text, string? command)
        {
            var patterns = new List<Pattern>();
            var literal = new StringBuilder();

            void FlushLiteral()
            {
                if (literal.Length > 0)
                {
                    patterns.Add(new LiteralPattern(literal.ToString()));
                    literal.Clear();
                }
            }

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (c == '\\' && i + 1 < text.Length && "{}[],\\".IndexOf(text[i + 1]) >= 0)
                {
                    literal.Append(text[++i]);
                    continue;
                }

                // curl leaves empty "[]" and "{}" alone
                if ((c == '{' || c == '[') && i + 1 < text.Length && text[i + 1] == (c == '{' ? '}' : ']'))
                {
                    literal.Append(c).Append(text[++i]);
                    continue;
                }

                if (c == '{')
                {
                    FlushLiteral();
                    i = ParseSet(text, i, patterns, command);
                    continue;
                }

                if (c == '[')
                {
                    var close = text.IndexOf(']', i + 1);
                    if (close > i && IsIpv6Literal(text, i, close))
                    {
                        literal.Append(text, i, close - i + 1);
                        i = close;
                        continue;
                    }

                    FlushLiteral();
                    i = ParseRange(text, i, patterns, command);
                    continue;
                }

                if (c == '}' || c == ']')
                {
                    throw GlobError(text, $"unmatched close {(c == '}' ? "brace" : "bracket")} '{c}' in URL position {i + 1}",
                        $"Remove the stray '{c}', escape it as \\{c} to send it literally, or turn globbing off with -g/--globoff.", command);
                }

                literal.Append(c);
            }

            FlushLiteral();
            return patterns;
        }

        private static string? ReadGlobName(string text, ref int position)
        {
            if (position < text.Length && text[position] == '<')
            {
                var close = text.IndexOf('>', position + 1);
                if (close > position + 1)
                {
                    var name = text.Substring(position + 1, close - position - 1);
                    position = close + 1;
                    return name;
                }
            }

            return null;
        }

        private static int ParseSet(string text, int open, List<Pattern> patterns, string? command)
        {
            var i = open + 1;
            var name = ReadGlobName(text, ref i);
            var values = new List<string>();
            var current = new StringBuilder();
            for (; i < text.Length; i++)
            {
                var c = text[i];
                if (c == '\\' && i + 1 < text.Length)
                {
                    current.Append(text[++i]);
                    continue;
                }

                if (c == ',')
                {
                    values.Add(current.ToString());
                    current.Clear();
                    continue;
                }

                if (c == '}')
                {
                    values.Add(current.ToString());
                    patterns.Add(new ListPattern(values) { Name = name });
                    return i;
                }

                if (c == '{' || c == '[')
                {
                    throw GlobError(text, $"nested brace in URL position {i + 1}: '{c}' inside a {{}} set",
                        "A {a,b} set cannot contain another { or [; list every combination instead, for example {a1,a2,b1,b2}, or escape the character as \\" + c + ".", command);
                }

                current.Append(c);
            }

            throw GlobError(text, $"unmatched brace in URL position {open + 1}: the {{ set is never closed",
                "Close the set with }, as in {one,two}, or escape a literal { as \\{ (or use -g/--globoff to turn globbing off).", command);
        }

        private static int ParseRange(string text, int open, List<Pattern> patterns, string? command)
        {
            var close = text.IndexOf(']', open + 1);
            if (close < 0)
            {
                throw GlobError(text, $"unmatched bracket in URL position {open + 1}: the [ range is never closed",
                    "Close the range with ], as in [1-10], or escape a literal [ as \\[ (IPv6 addresses such as http://[::1]/ also need -g/--globoff).", command);
            }

            var i = open + 1;
            var name = ReadGlobName(text, ref i);
            var body = text.Substring(i, close - i);

            long step = 1;
            var colon = body.IndexOf(':');
            if (colon >= 0)
            {
                if (!long.TryParse(body.Substring(colon + 1), NumberStyles.None, CultureInfo.InvariantCulture, out step) || step < 1)
                {
                    throw GlobError(text, $"bad range in URL position {open + 1}: the step in [{body}] is not a whole number of 1 or more",
                        "Write the step after a colon, for example [1-100:10] for every tenth number.", command);
                }

                body = body.Substring(0, colon);
            }

            var dash = body.IndexOf('-', 1);
            if (dash <= 0 || dash == body.Length - 1)
            {
                throw GlobError(text, $"bad range in URL position {open + 1}: [{body}] is not a start-end range",
                    "Write [start-end], such as [1-10], [001-100] (zero-padded) or [a-z]; for a list of words use {one,two} instead.", command);
            }

            var from = body.Substring(0, dash);
            var to = body.Substring(dash + 1);
            var values = new List<string>();

            if (from.Length == 1 && to.Length == 1 && char.IsLetter(from[0]) && char.IsLetter(to[0]))
            {
                if (to[0] < from[0] || char.IsUpper(from[0]) != char.IsUpper(to[0]))
                {
                    throw GlobError(text, $"bad range in URL position {open + 1}: the letter range [{body}] runs backwards or mixes upper and lower case",
                        "Use a range in one case that counts up, such as [a-z] or [A-F].", command);
                }

                for (long ch = from[0]; ch <= to[0]; ch += step)
                {
                    values.Add(((char)ch).ToString());
                }
            }
            else
            {
                if (!long.TryParse(from, NumberStyles.None, CultureInfo.InvariantCulture, out var min) ||
                    !long.TryParse(to, NumberStyles.None, CultureInfo.InvariantCulture, out var max) ||
                    max < min || max == long.MaxValue)
                {
                    throw GlobError(text, $"bad range in URL position {open + 1}: [{body}] is not a range of whole numbers counting up",
                        "Both ends must be non-negative whole numbers with start <= end, for example [1-10] or [0-255].", command);
                }

                if ((max - min) / step >= MaxUrls)
                {
                    throw GlobError(text, $"the range [{body}] in URL position {open + 1} expands to more than {MaxUrls.ToString("N0", CultureInfo.InvariantCulture)} URLs",
                        "Split the job into several commands with smaller ranges, or use a step such as [1-100000000:1000].", command);
                }

                // A leading zero asks for zero padding to the width of the start value: [001-100]
                var width = from.Length > 1 && from[0] == '0' ? from.Length : 0;
                for (var n = min; n <= max; n += step)
                {
                    values.Add(width > 0 ? n.ToString(CultureInfo.InvariantCulture).PadLeft(width, '0') : n.ToString(CultureInfo.InvariantCulture));
                    if (n > long.MaxValue - step)
                    {
                        break;
                    }
                }
            }

            patterns.Add(new ListPattern(values) { Name = name });
            return close;
        }

        /// <summary>
        /// curl leaves <c>[...]</c> alone when it holds an IPv6 address (hex digits, colons, dots and an
        /// optional <c>%zone</c>) right where a host name belongs.
        /// </summary>
        private static bool IsIpv6Literal(string text, int open, int close)
        {
            var body = text.Substring(open + 1, close - open - 1);
            if (body.IndexOf(':') < 0)
            {
                return false;
            }

            foreach (var ch in body)
            {
                if (!(Uri.IsHexDigit(ch) || ch == ':' || ch == '.' || ch == '%' || char.IsLetterOrDigit(ch)))
                {
                    return false;
                }
            }

            var before = text.Substring(0, open);
            return open == 0 || before.EndsWith("://", StringComparison.Ordinal) || before.EndsWith("@", StringComparison.Ordinal);
        }

        private static CurlException GlobError(string text, string problem, string fix, string? command)
        {
            return new CurlException(
                CurlException.Compose($"{problem} (URL glob \"{ErrorText.Preview(text, 200)}\").", fix, DocLinks.Url),
                3,
                command);
        }
    }
}
