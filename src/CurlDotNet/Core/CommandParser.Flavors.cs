/***************************************************************************
 * CommandParser.Flavors - shell-specific readers for pasted curl commands
 *
 * Part of CommandParser. Turns the text of a command written for bash/zsh,
 * cmd.exe or PowerShell into curl's argument list using that shell's exact
 * quoting, escaping, line-continuation and variable rules (see CurlFlavor),
 * detects the flavor of a command for CurlFlavor.Auto, and routes
 * Invoke-WebRequest syntax to InvokeWebRequestTranslator.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    internal partial class CommandParser
    {
        /// <summary>The guide that explains every flavor rule; linked from each flavor error message.</summary>
        internal const string FlavorDocs = "https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md";

        /// <summary>
        /// Parses a curl command written for the given shell <paramref name="flavor"/> into a
        /// <see cref="CurlOptions"/> describing its first transfer (further URLs and <c>--next</c> groups are in
        /// <see cref="CurlOptions.AdditionalTransfers"/>).
        /// </summary>
        /// <param name="command">The command, with or without the leading <c>curl</c> / <c>curl.exe</c> (or the
        /// <c>Invoke-WebRequest</c> cmdlet name for <see cref="CurlFlavor.InvokeWebRequest"/>).</param>
        /// <param name="flavor">The shell the text was written for; <see cref="CurlFlavor.Auto"/> detects it.</param>
        /// <returns>The parsed options; <see cref="CurlOptions.Flavor"/> holds the flavor that was used (never
        /// <see cref="CurlFlavor.Auto"/>).</returns>
        /// <exception cref="ArgumentNullException">The command is null, empty or only whitespace.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The flavor is not a defined <see cref="CurlFlavor"/> value.</exception>
        /// <exception cref="CurlInvalidCommandException">The text breaks the flavor's syntax (for example an
        /// unterminated quote) or uses an Invoke-WebRequest feature that cannot be translated.</exception>
        public CurlOptions Parse(string command, CurlFlavor flavor)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));

            if (flavor < CurlFlavor.Auto || flavor > CurlFlavor.InvokeWebRequest)
            {
                throw new ArgumentOutOfRangeException(nameof(flavor), flavor,
                    "Unknown CurlFlavor value. Use CurlFlavor.Auto, Posix, WindowsCmd, PowerShell or InvokeWebRequest. See " + FlavorDocs);
            }

            var used = flavor;
            var exact = flavor != CurlFlavor.Auto;
            if (flavor == CurlFlavor.Auto)
            {
                used = DetectFlavor(command, out exact);
            }

            CurlOptions options;
            if (used == CurlFlavor.InvokeWebRequest)
            {
                options = ParseArgumentList(InvokeWebRequestTranslator.Translate(command), command);
            }
            else if (!exact)
            {
                options = ParsePermissive(command);
            }
            else
            {
                var args = used == CurlFlavor.Posix ? TokenizePosix(command)
                    : used == CurlFlavor.WindowsCmd ? TokenizeWindowsCmd(command)
                    : TokenizePowerShell(command);
                options = ParseArgumentList(StripProgramName(args, command, used), command);
            }

            options.Flavor = used;
            foreach (var transfer in options.AdditionalTransfers)
            {
                transfer.Flavor = used;
            }

            return options;
        }

        /// <summary>
        /// Returns the flavor <see cref="CurlFlavor.Auto"/> picks for <paramref name="command"/>; see
        /// <see cref="CurlFlavor.Auto"/> for the rules.
        /// </summary>
        internal static CurlFlavor DetectFlavor(string command) => DetectFlavor(command ?? string.Empty, out _);

        /// <summary>
        /// Detects the flavor of a command. <paramref name="exact"/> is true when the text contains a construct
        /// that only that flavor's exact rules can read (<c>^"</c> for cmd.exe, <c>`"</c> or <c>--%</c> for
        /// PowerShell); otherwise Auto reads the command with the permissive union reader.
        /// </summary>
        private static CurlFlavor DetectFlavor(string command, out bool exact)
        {
            exact = false;
            if (InvokeWebRequestTranslator.LooksLikeInvokeWebRequest(command))
            {
                return CurlFlavor.InvokeWebRequest;
            }

            var backtickQuote = command.IndexOf("`\"", StringComparison.Ordinal) >= 0;
            var stopParsing = Regex.IsMatch(command, @"(^|\s)--%(\s|$)");
            if (backtickQuote || stopParsing ||
                Regex.IsMatch(command, @"`[ \t]*(\r\n|\r|\n)") ||
                command.IndexOf("$env:", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                exact = backtickQuote || stopParsing;
                return CurlFlavor.PowerShell;
            }

            var caretQuote = Regex.IsMatch(command, @"(^|[\s=])\^""");
            if (caretQuote || Regex.IsMatch(command, @"\^[ \t]*(\r\n|\r|\n)") || HasSetCmdVariable(command))
            {
                exact = caretQuote;
                return CurlFlavor.WindowsCmd;
            }

            return CurlFlavor.Posix;
        }

        /// <summary>True when the text contains <c>%NAME%</c> and <c>NAME</c> is a set environment variable.</summary>
        private static bool HasSetCmdVariable(string command)
        {
            for (var i = command.IndexOf('%'); i >= 0 && i < command.Length; i = command.IndexOf('%', i + 1))
            {
                if (ShellVariableNameLength(command, i + 1) > 0 && TryExpandCmdVariable(command, i, out _, out _))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the program name from an argument list produced by an exact flavor reader: <c>curl</c>,
        /// <c>curl.exe</c> or a path to either, after PowerShell's <c>&amp;</c> call operator.
        /// </summary>
        /// <exception cref="CurlException">Exit code 3 when the command is only the program name.</exception>
        private static List<string> StripProgramName(List<string> args, string command, CurlFlavor flavor)
        {
            if (flavor == CurlFlavor.PowerShell && args.Count > 0 && args[0] == "&")
            {
                args.RemoveAt(0);
            }

            if (args.Count == 0)
            {
                return args;
            }

            var first = args[0];
            var slash = Math.Max(first.LastIndexOf('/'), first.LastIndexOf('\\'));
            var program = slash >= 0 ? first.Substring(slash + 1) : first;
            if (program.Equals("curl", StringComparison.OrdinalIgnoreCase) ||
                program.Equals("curl.exe", StringComparison.OrdinalIgnoreCase))
            {
                args.RemoveAt(0);
                if (args.Count == 0)
                {
                    throw new CurlException(
                        "No URL specified: the command is only the program name. Add the URL to fetch, for example " +
                        "\"curl https://example.com\". See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md",
                        3, command);
                }
            }

            return args;
        }

        /// <summary>The exception for text that breaks a flavor's own syntax rules.</summary>
        private static CurlInvalidCommandException FlavorSyntaxError(CurlFlavor flavor, string problem, string fix, string part, string command)
        {
            return new CurlInvalidCommandException(
                $"{problem} (reading the command as {flavor}). {fix} " +
                "If the command was copied from a different shell, pass that CurlFlavor instead, or CurlFlavor.Auto. See " + FlavorDocs,
                part, command);
        }

        /// <summary>Length of the line break at <paramref name="index"/> (<c>\r\n</c> = 2, <c>\n</c> or <c>\r</c> = 1), or 0.</summary>
        private static int NewlineLength(string text, int index)
        {
            if (index >= text.Length) return 0;
            if (text[index] == '\n') return 1;
            if (text[index] == '\r') return index + 1 < text.Length && text[index + 1] == '\n' ? 2 : 1;
            return 0;
        }

        private static bool IsShellSpace(char c) => c == ' ' || c == '\t' || c == '\r' || c == '\n';

        #region Posix (bash, zsh, sh)

        /// <summary>
        /// Splits text the way bash does: single quotes literal, double quotes with <c>\" \\ \$ \`</c> escapes,
        /// <c>$'...'</c> ANSI-C quotes, backslash escaping any character outside quotes, backslash-newline
        /// continuation, <c>$VAR</c> / <c>${VAR}</c> expansion outside single quotes and <c>#</c> comments.
        /// </summary>
        internal static List<string> TokenizePosix(string text)
        {
            var args = new List<string>();
            var current = new StringBuilder();
            var inWord = false;
            var i = 0;
            while (i < text.Length)
            {
                var c = text[i];
                if (IsShellSpace(c))
                {
                    if (inWord)
                    {
                        args.Add(current.ToString());
                        current.Clear();
                        inWord = false;
                    }

                    i++;
                    continue;
                }

                if (c == '#' && !inWord)
                {
                    while (i < text.Length && text[i] != '\n' && text[i] != '\r') i++;
                    continue;
                }

                if (c == '|' || c == ';' || c == '&' || c == '>' || c == '<')
                {
                    // the shell ends the command here: "| jq .", "&& echo ok", "> out.txt", "2>/dev/null"
                    DropRedirectionHandle(c, ref inWord, current);
                    break;
                }

                if (c == '\\')
                {
                    if (i + 1 >= text.Length)
                    {
                        i++; // a backslash that ends the text continues onto nothing
                        continue;
                    }

                    var newline = NewlineLength(text, i + 1);
                    if (newline > 0)
                    {
                        i += 1 + newline;
                        continue;
                    }

                    current.Append(text[i + 1]);
                    inWord = true;
                    i += 2;
                    continue;
                }

                if (c == '\'')
                {
                    var close = text.IndexOf('\'', i + 1);
                    if (close < 0)
                    {
                        throw FlavorSyntaxError(CurlFlavor.Posix, "A single quote is never closed",
                            "Add the closing ' (inside single quotes nothing is special; write a quote as '\\'').", text.Substring(i), text);
                    }

                    current.Append(text, i + 1, close - i - 1);
                    inWord = true;
                    i = close + 1;
                    continue;
                }

                if (c == '$' && i + 1 < text.Length && text[i + 1] == '\'')
                {
                    i = ReadAnsiCQuoted(text, i + 2, current);
                    inWord = true;
                    continue;
                }

                if (c == '"')
                {
                    i = ReadPosixDoubleQuoted(text, i + 1, current);
                    inWord = true;
                    continue;
                }

                if (c == '$' && TryExpandPosixVariable(text, i, out var value, out var consumed))
                {
                    // an unquoted expansion is split into words on spaces, tabs and newlines, as bash does
                    foreach (var v in value)
                    {
                        if (v == ' ' || v == '\t' || v == '\n')
                        {
                            if (inWord)
                            {
                                args.Add(current.ToString());
                                current.Clear();
                                inWord = false;
                            }
                        }
                        else
                        {
                            current.Append(v);
                            inWord = true;
                        }
                    }

                    i += consumed;
                    continue;
                }

                current.Append(c);
                inWord = true;
                i++;
            }

            if (inWord)
            {
                args.Add(current.ToString());
            }

            return args;
        }

        /// <summary>
        /// At a redirection (<c>&gt;</c> or <c>&lt;</c>) directly after a file-descriptor number, as in
        /// <c>2&gt;/dev/null</c> or <c>2&gt;nul</c>, the number belongs to the redirection, not to the arguments.
        /// </summary>
        private static void DropRedirectionHandle(char c, ref bool inWord, StringBuilder current)
        {
            if (!inWord || (c != '>' && c != '<') || current.Length == 0)
            {
                return;
            }

            for (var k = 0; k < current.Length; k++)
            {
                if (current[k] < '0' || current[k] > '9')
                {
                    return;
                }
            }

            current.Clear();
            inWord = false;
        }

        /// <summary>Reads a bash double-quoted string starting after the opening quote; returns the index after the closing quote.</summary>
        private static int ReadPosixDoubleQuoted(string text, int start, StringBuilder current)
        {
            var i = start;
            while (i < text.Length)
            {
                var c = text[i];
                if (c == '"')
                {
                    return i + 1;
                }

                if (c == '\\' && i + 1 < text.Length)
                {
                    var next = text[i + 1];
                    if (next == '"' || next == '\\' || next == '$' || next == '`')
                    {
                        current.Append(next);
                        i += 2;
                        continue;
                    }

                    var newline = NewlineLength(text, i + 1);
                    if (newline > 0)
                    {
                        i += 1 + newline;
                        continue;
                    }

                    current.Append('\\');
                    i++;
                    continue;
                }

                if (c == '$' && TryExpandPosixVariable(text, i, out var value, out var consumed))
                {
                    current.Append(value);
                    i += consumed;
                    continue;
                }

                current.Append(c);
                i++;
            }

            throw FlavorSyntaxError(CurlFlavor.Posix, "A double quote is never closed",
                "Add the closing \" (inside double quotes write a literal quote as \\\").", text.Substring(start - 1), text);
        }

        /// <summary>Reads a bash <c>$'...'</c> string starting after <c>$'</c>; returns the index after the closing quote.</summary>
        private static int ReadAnsiCQuoted(string text, int start, StringBuilder current)
        {
            var i = start;
            while (i < text.Length)
            {
                var c = text[i];
                if (c == '\'')
                {
                    return i + 1;
                }

                if (c != '\\' || i + 1 >= text.Length)
                {
                    current.Append(c);
                    i++;
                    continue;
                }

                var e = text[i + 1];
                i += 2;
                switch (e)
                {
                    case 'a': current.Append('\a'); break;
                    case 'b': current.Append('\b'); break;
                    case 'e':
                    case 'E': current.Append('\u001b'); break;
                    case 'f': current.Append('\f'); break;
                    case 'n': current.Append('\n'); break;
                    case 'r': current.Append('\r'); break;
                    case 't': current.Append('\t'); break;
                    case 'v': current.Append('\v'); break;
                    case '\\':
                    case '\'':
                    case '"':
                    case '?': current.Append(e); break;
                    case 'x':
                        i = AppendHexCode(text, i, 2, current, "\\x");
                        break;
                    case 'u':
                        i = AppendHexCode(text, i, 4, current, "\\u");
                        break;
                    case 'U':
                        i = AppendHexCode(text, i, 8, current, "\\U");
                        break;
                    case 'c':
                        if (i < text.Length)
                        {
                            current.Append((char)(text[i] & 0x1f));
                            i++;
                        }

                        break;
                    default:
                        if (e >= '0' && e <= '7')
                        {
                            var code = e - '0';
                            var digits = 1;
                            while (digits < 3 && i < text.Length && text[i] >= '0' && text[i] <= '7')
                            {
                                code = code * 8 + (text[i] - '0');
                                i++;
                                digits++;
                            }

                            current.Append((char)(code & 0xff));
                        }
                        else
                        {
                            current.Append('\\').Append(e);
                        }

                        break;
                }
            }

            throw FlavorSyntaxError(CurlFlavor.Posix, "An ANSI-C quoted string $'...' is never closed",
                "Add the closing ' (inside $'...' write a quote as \\').", text.Substring(start - 2), text);
        }

        /// <summary>Appends the character named by up to <paramref name="maxDigits"/> hex digits at <paramref name="index"/>.</summary>
        private static int AppendHexCode(string text, int index, int maxDigits, StringBuilder current, string prefix)
        {
            var end = index;
            while (end < text.Length && end - index < maxDigits && Uri.IsHexDigit(text[end]))
            {
                end++;
            }

            if (end == index)
            {
                current.Append(prefix);
                return index;
            }

            var code = int.Parse(text.Substring(index, end - index), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            if (code > 0x10FFFF)
            {
                code = 0xFFFD;
            }

            current.Append(char.ConvertFromUtf32(code >= 0xD800 && code <= 0xDFFF ? 0xFFFD : code));
            return end;
        }

        #endregion

        #region WindowsCmd (cmd.exe + C runtime)

        /// <summary>
        /// Splits text the way <c>curl.exe</c> sees it when typed into cmd.exe: first cmd.exe expands
        /// <c>%VAR%</c>, removes <c>^</c> escapes outside double quotes and joins <c>^</c>-continued lines; then
        /// the C runtime splits the resulting command line (see <see cref="SplitWindowsArguments"/>).
        /// </summary>
        internal static List<string> TokenizeWindowsCmd(string text)
        {
            // phase 1: %VAR% expansion runs over the whole line before carets mean anything,
            // so ^%VAR% still expands (a caret cannot protect a percent sign at the prompt)
            text = ExpandCmdVariables(text);

            var line = new StringBuilder(text.Length);
            var quoted = false;
            var i = 0;
            while (i < text.Length)
            {
                var c = text[i];
                if (!quoted && (c == '&' || c == '|' || c == '<' || c == '>'))
                {
                    // cmd.exe ends the command at an unescaped & | < > outside quotes ("| findstr x", "> out.txt", "2>nul")
                    if (c == '>' || c == '<')
                    {
                        var end = line.Length;
                        if (end > 0 && line[end - 1] >= '0' && line[end - 1] <= '9' && (end == 1 || char.IsWhiteSpace(line[end - 2])))
                        {
                            line.Length = end - 1;
                        }
                    }

                    break;
                }

                if (c == '^' && !quoted)
                {
                    if (i + 1 >= text.Length)
                    {
                        i++;
                        continue;
                    }

                    var newline = NewlineLength(text, i + 1);
                    if (newline > 0)
                    {
                        i += 1 + newline;
                        continue;
                    }

                    // the escaped character is passed on, but an escaped quote does not change cmd's quote state
                    line.Append(text[i + 1]);
                    i += 2;
                    continue;
                }

                if (c == '"')
                {
                    quoted = !quoted;
                }

                line.Append(c);
                i++;
            }

            return SplitWindowsArguments(line.ToString());
        }

        /// <summary>Replaces every <c>%NAME%</c> naming a set environment variable; everything else is kept.</summary>
        private static string ExpandCmdVariables(string text)
        {
            if (text.IndexOf('%') < 0)
            {
                return text;
            }

            var expanded = new StringBuilder(text.Length);
            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == '%' && TryExpandCmdVariable(text, i, out var value, out var consumed))
                {
                    expanded.Append(value);
                    i += consumed - 1;
                    continue;
                }

                expanded.Append(text[i]);
            }

            return expanded.ToString();
        }

        /// <summary>
        /// The Microsoft C runtime's command-line splitting (what <c>curl.exe</c>'s <c>main()</c> receives):
        /// spaces and tabs separate arguments outside double quotes; a double quote starts or ends a quoted part;
        /// <c>""</c> inside a quoted part is a literal quote; 2n backslashes before a quote become n backslashes
        /// and the quote delimits, 2n+1 backslashes before a quote become n backslashes and a literal quote;
        /// backslashes anywhere else are literal. Single quotes are ordinary characters.
        /// </summary>
        internal static List<string> SplitWindowsArguments(string line)
        {
            var args = new List<string>();
            var current = new StringBuilder();
            var inArgument = false;
            var inQuotes = false;
            var i = 0;
            while (i < line.Length)
            {
                var c = line[i];
                if (!inQuotes && IsShellSpace(c))
                {
                    if (inArgument)
                    {
                        args.Add(current.ToString());
                        current.Clear();
                        inArgument = false;
                    }

                    i++;
                    continue;
                }

                inArgument = true;
                if (c == '\\')
                {
                    var j = i;
                    while (j < line.Length && line[j] == '\\') j++;
                    var count = j - i;
                    if (j < line.Length && line[j] == '"')
                    {
                        current.Append('\\', count / 2);
                        if (count % 2 == 1)
                        {
                            current.Append('"');
                            i = j + 1;
                        }
                        else
                        {
                            i = j;
                        }
                    }
                    else
                    {
                        current.Append('\\', count);
                        i = j;
                    }

                    continue;
                }

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i += 2;
                        continue;
                    }

                    inQuotes = !inQuotes;
                    i++;
                    continue;
                }

                current.Append(c);
                i++;
            }

            if (inArgument)
            {
                args.Add(current.ToString());
            }

            return args;
        }

        #endregion

        #region PowerShell (curl.exe from pwsh / Windows PowerShell)

        internal static bool IsPowerShellSingleQuote(char c) => c == '\'' || c == '‘' || c == '’' || c == '‚' || c == '‛';

        internal static bool IsPowerShellDoubleQuote(char c) => c == '"' || c == '“' || c == '”' || c == '„';

        /// <summary>Removes a leading <c>$name =</c> assignment, as in <c>$r = curl.exe ...</c>.</summary>
        internal static string StripPowerShellAssignment(string text)
        {
            var match = Regex.Match(text, @"^\s*\$[A-Za-z_][A-Za-z0-9_:]*\s*=(?!=)\s*");
            return match.Success ? text.Substring(match.Length) : text;
        }

        /// <summary>
        /// Splits text the way PowerShell passes it to <c>curl.exe</c>: single quotes literal with <c>''</c>,
        /// double quotes with <c>$env:VAR</c> and backtick escapes, backtick escapes and continuation in bare
        /// words, here-strings, <c>#</c> comments, <c>--%</c> stop-parsing, an unquoted <c>|</c> or <c>;</c>
        /// ending the command, and finally the Windows C runtime's <c>\"</c> rule applied to each argument.
        /// </summary>
        internal static List<string> TokenizePowerShell(string text)
        {
            text = StripPowerShellAssignment(text);
            var args = new List<string>();
            var literalArgs = new HashSet<int>();
            var current = new StringBuilder();
            var inWord = false;
            var i = 0;
            while (i < text.Length)
            {
                var c = text[i];
                if (IsShellSpace(c))
                {
                    if (inWord)
                    {
                        args.Add(current.ToString());
                        current.Clear();
                        inWord = false;
                    }

                    i++;
                    continue;
                }

                if (c == '|' || c == ';' || c == '>' ||
                    (c == '&' && ((i + 1 < text.Length && text[i + 1] == '&') || args.Count > 0 || inWord)))
                {
                    // the rest is another pipeline element, statement, chain or redirection
                    // ("| ConvertFrom-Json", "; echo done", "&& echo ok", "> out.txt", "2> err.txt"); a leading & is the call operator
                    DropRedirectionHandle(c, ref inWord, current);
                    break;
                }

                if (c == '#' && !inWord)
                {
                    while (i < text.Length && text[i] != '\n' && text[i] != '\r') i++;
                    continue;
                }

                if (!inWord && c == '-' && string.CompareOrdinal(text, i, "--%", 0, 3) == 0 &&
                    (i + 3 >= text.Length || IsShellSpace(text[i + 3])))
                {
                    i = ReadStopParsing(text, i + 3, args, literalArgs);
                    break;
                }

                if (c == '`')
                {
                    if (i + 1 >= text.Length)
                    {
                        i++;
                        continue;
                    }

                    var newline = NewlineLength(text, i + 1);
                    if (newline > 0)
                    {
                        i += 1 + newline;
                        continue;
                    }

                    i = AppendPowerShellEscape(text, i + 1, current);
                    inWord = true;
                    continue;
                }

                if (!inWord && c == '@' && i + 1 < text.Length &&
                    (IsPowerShellSingleQuote(text[i + 1]) || IsPowerShellDoubleQuote(text[i + 1])) &&
                    NewlineLength(text, SkipSpacesAndTabs(text, i + 2)) > 0)
                {
                    i = ReadPowerShellHereString(text, i, current);
                    inWord = true;
                    continue;
                }

                if (IsPowerShellSingleQuote(c))
                {
                    i = ReadPowerShellSingleQuoted(text, i + 1, current);
                    inWord = true;
                    continue;
                }

                if (IsPowerShellDoubleQuote(c))
                {
                    i = ReadPowerShellDoubleQuoted(text, i + 1, current);
                    inWord = true;
                    continue;
                }

                if (c == '$' && TryExpandPowerShellVariable(text, i, out var value, out var consumed))
                {
                    current.Append(value);
                    inWord |= value.Length > 0;
                    i += consumed;
                    continue;
                }

                current.Append(c);
                inWord = true;
                i++;
            }

            if (inWord)
            {
                args.Add(current.ToString());
            }

            for (var k = 0; k < args.Count; k++)
            {
                if (!literalArgs.Contains(k))
                {
                    args[k] = ApplyWindowsQuoteEscapes(args[k]);
                }
            }

            return args;
        }

        private static int SkipSpacesAndTabs(string text, int index)
        {
            while (index < text.Length && (text[index] == ' ' || text[index] == '\t')) index++;
            return index;
        }

        /// <summary>
        /// Handles the rest of the line after <c>--%</c>: <c>%NAME%</c> references are expanded and the text is
        /// split with the C runtime rules. Text on later lines is a separate PowerShell statement and is refused.
        /// </summary>
        private static int ReadStopParsing(string text, int start, List<string> args, HashSet<int> literalArgs)
        {
            // stop-parsing lasts until the end of the line or an unquoted pipe ("--% ... | ConvertFrom-Json")
            var end = start;
            var quoted = false;
            while (end < text.Length && text[end] != '\n' && text[end] != '\r' && (quoted || text[end] != '|'))
            {
                if (text[end] == '"') quoted = !quoted;
                end++;
            }

            var raw = text.Substring(start, end - start);
            var expanded = new StringBuilder(raw.Length);
            for (var i = 0; i < raw.Length; i++)
            {
                if (raw[i] == '%' && TryExpandCmdVariable(raw, i, out var value, out var consumed))
                {
                    expanded.Append(value);
                    i += consumed - 1;
                    continue;
                }

                expanded.Append(raw[i]);
            }

            foreach (var argument in SplitWindowsArguments(expanded.ToString()))
            {
                literalArgs.Add(args.Count);
                args.Add(argument);
            }

            if (end < text.Length && text[end] != '|' && text.Substring(end).Trim().Length > 0)
            {
                throw FlavorSyntaxError(CurlFlavor.PowerShell,
                    "The stop-parsing token --% only covers the rest of its own line, but the command continues on the next line",
                    "Put everything after --% on one line (a backtick does not continue a --% line).", "--%", text);
            }

            return text.Length;
        }

        /// <summary>
        /// Appends the character a PowerShell backtick escape stands for; <paramref name="index"/> is the character
        /// after the backtick. Returns the index after the escape.
        /// </summary>
        internal static int AppendPowerShellEscape(string text, int index, StringBuilder current)
        {
            var e = text[index];
            switch (e)
            {
                case '0': current.Append('\0'); break;
                case 'a': current.Append('\a'); break;
                case 'b': current.Append('\b'); break;
                case 'e': current.Append('\u001b'); break;
                case 'f': current.Append('\f'); break;
                case 'n': current.Append('\n'); break;
                case 'r': current.Append('\r'); break;
                case 't': current.Append('\t'); break;
                case 'v': current.Append('\v'); break;
                case 'u':
                    if (index + 1 < text.Length && text[index + 1] == '{')
                    {
                        var close = text.IndexOf('}', index + 2);
                        if (close > index + 2 &&
                            int.TryParse(text.Substring(index + 2, close - index - 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var code) &&
                            code >= 0 && code <= 0x10FFFF && (code < 0xD800 || code > 0xDFFF))
                        {
                            current.Append(char.ConvertFromUtf32(code));
                            return close + 1;
                        }
                    }

                    current.Append('u');
                    break;
                default: current.Append(e); break;
            }

            return index + 1;
        }

        /// <summary>Reads a PowerShell single-quoted string after the opening quote (<c>''</c> is one quote).</summary>
        internal static int ReadPowerShellSingleQuoted(string text, int start, StringBuilder current, CurlFlavor flavor = CurlFlavor.PowerShell)
        {
            var i = start;
            while (i < text.Length)
            {
                var c = text[i];
                if (IsPowerShellSingleQuote(c))
                {
                    if (i + 1 < text.Length && IsPowerShellSingleQuote(text[i + 1]))
                    {
                        current.Append('\'');
                        i += 2;
                        continue;
                    }

                    return i + 1;
                }

                current.Append(c);
                i++;
            }

            throw FlavorSyntaxError(flavor, "A single quote is never closed",
                "Add the closing ' (inside single quotes write a quote as '').", text.Substring(start - 1), text);
        }

        /// <summary>
        /// Reads a PowerShell double-quoted string after the opening quote: backtick escapes, <c>""</c> as one
        /// quote, <c>$env:NAME</c> / <c>$NAME</c> expansion, <c>$( ... )</c> left as typed.
        /// </summary>
        internal static int ReadPowerShellDoubleQuoted(string text, int start, StringBuilder current, CurlFlavor flavor = CurlFlavor.PowerShell)
        {
            var i = start;
            while (i < text.Length)
            {
                var c = text[i];
                if (c == '`' && i + 1 < text.Length)
                {
                    i = AppendPowerShellEscape(text, i + 1, current);
                    continue;
                }

                if (IsPowerShellDoubleQuote(c))
                {
                    if (i + 1 < text.Length && IsPowerShellDoubleQuote(text[i + 1]))
                    {
                        current.Append('"');
                        i += 2;
                        continue;
                    }

                    return i + 1;
                }

                if (c == '$' && i + 1 < text.Length && text[i + 1] == '(')
                {
                    var depth = 0;
                    var j = i + 1;
                    for (; j < text.Length; j++)
                    {
                        if (text[j] == '(') depth++;
                        else if (text[j] == ')' && --depth == 0) break;
                    }

                    var end = Math.Min(j + 1, text.Length);
                    current.Append(text, i, end - i);
                    i = end;
                    continue;
                }

                if (c == '$' && TryExpandPowerShellVariable(text, i, out var value, out var consumed))
                {
                    current.Append(value);
                    i += consumed;
                    continue;
                }

                current.Append(c);
                i++;
            }

            throw FlavorSyntaxError(flavor, "A double quote is never closed",
                "Add the closing \" (inside double quotes write a quote as `\" or \"\").", text.Substring(start - 1), text);
        }

        /// <summary>
        /// Reads a here-string <c>@' ... '@</c> (literal) or <c>@" ... "@</c> (expandable) starting at the
        /// <c>@</c>; the content is the lines between the opening and closing lines.
        /// </summary>
        internal static int ReadPowerShellHereString(string text, int at, StringBuilder current, CurlFlavor flavor = CurlFlavor.PowerShell)
        {
            var expandable = IsPowerShellDoubleQuote(text[at + 1]);
            var contentStart = SkipSpacesAndTabs(text, at + 2);
            contentStart += NewlineLength(text, contentStart);
            // the closing line may directly follow the opening one (an empty here-string)
            var terminator = new Regex(expandable ? "(\\r\\n|\\r|\\n)[\"“”„]@" : "(\\r\\n|\\r|\\n)['‘’‚‛]@");
            var empty = new Regex(expandable ? "\\G[\"“”„]@" : "\\G['‘’‚‛]@");
            var emptyMatch = empty.Match(text, contentStart);
            if (emptyMatch.Success && contentStart > at + 2)
            {
                return emptyMatch.Index + emptyMatch.Length;
            }

            var match = terminator.Match(text, contentStart);
            if (!match.Success)
            {
                throw FlavorSyntaxError(flavor, "A here-string is never closed",
                    "End it with " + (expandable ? "\"@" : "'@") + " at the very start of its own line.", text.Substring(at), text);
            }

            var content = text.Substring(contentStart, match.Index - contentStart);
            if (expandable)
            {
                var expanded = new StringBuilder(content.Length);
                for (var i = 0; i < content.Length;)
                {
                    if (content[i] == '`' && i + 1 < content.Length)
                    {
                        i = AppendPowerShellEscape(content, i + 1, expanded);
                    }
                    else if (content[i] == '$' && TryExpandPowerShellVariable(content, i, out var value, out var consumed))
                    {
                        expanded.Append(value);
                        i += consumed;
                    }
                    else
                    {
                        expanded.Append(content[i]);
                        i++;
                    }
                }

                content = expanded.ToString();
            }

            current.Append(content);
            return match.Index + match.Length;
        }

        /// <summary>
        /// The Windows C runtime's reading of an argument PowerShell built: a run of 2n+1 backslashes followed
        /// by a double quote becomes n backslashes and a literal quote (<c>\"</c> → <c>"</c>), which is how
        /// Windows PowerShell 5.1 (and 7.2 and older) commands are written. An argument that also contains a bare
        /// quote was written for PowerShell 7.3+, which passes arguments through intact, so it is kept as typed
        /// (its <c>\"</c> is a real escape, for example inside a JSON string).
        /// </summary>
        internal static string ApplyWindowsQuoteEscapes(string argument)
        {
            if (argument.IndexOf("\\\"", StringComparison.Ordinal) < 0)
            {
                return argument;
            }

            // An argument with a bare quote was written for PowerShell 7.3+, which passes it through intact
            // (Windows PowerShell 5.1 would have stripped those quotes), so its \" is a real escape such as a JSON one.
            for (var k = 0; k < argument.Length; k++)
            {
                if (argument[k] != '"')
                {
                    continue;
                }

                var backslashes = 0;
                for (var b = k - 1; b >= 0 && argument[b] == '\\'; b--) backslashes++;
                if (backslashes % 2 == 0)
                {
                    return argument;
                }
            }

            var sb = new StringBuilder(argument.Length);
            var i = 0;
            while (i < argument.Length)
            {
                if (argument[i] != '\\')
                {
                    sb.Append(argument[i]);
                    i++;
                    continue;
                }

                var j = i;
                while (j < argument.Length && argument[j] == '\\') j++;
                var count = j - i;
                if (j < argument.Length && argument[j] == '"' && count % 2 == 1)
                {
                    sb.Append('\\', count / 2).Append('"');
                    i = j + 1;
                }
                else
                {
                    sb.Append('\\', count);
                    i = j;
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}
