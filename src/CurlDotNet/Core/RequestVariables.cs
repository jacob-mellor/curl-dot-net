/***************************************************************************
 * RequestVariables - curl's --variable and --expand-OPTION, and the shell
 * variable syntax of each CurlFlavor ($VAR, ${VAR}, $env:VAR, %VAR%)
 *
 * Part of CommandParser. Follows curl's src/var.c: --variable name=content, name@file,
 * %ENVNAME (with optional =default / @file), byte ranges name[5-15]=..., and {{name}} /
 * {{name:trim:json:url:b64:64dec}} expansion in the argument of any --expand-OPTION.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Globalization;
using System.Text;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    internal partial class CommandParser
    {
        /// <summary>curl's limit for a variable name (MAX_VAR_LEN).</summary>
        private const int MaxVariableNameLength = 128;

        /// <summary>Handles one <c>--variable</c> argument.</summary>
        /// <exception cref="CurlException">Exit code 2 for an unset imported variable or a bad byte range; 26 when the file cannot be read.</exception>
        private static void DefineVariable(string input, ParseContext context)
        {
            var line = input;
            var import = line.StartsWith("%", StringComparison.Ordinal);
            var position = import ? 1 : 0;
            var nameStart = position;
            while (position < line.Length && (char.IsLetterOrDigit(line[position]) || line[position] == '_') && line[position] < 128)
            {
                position++;
            }

            var name = line.Substring(nameStart, position - nameStart);
            if (name.Length == 0 || name.Length >= MaxVariableNameLength)
            {
                return; // curl: "Bad variable name length, skipping"
            }

            var rest = line.Substring(position);
            string? content = null;
            if (import)
            {
                var environmentValue = Environment.GetEnvironmentVariable(name);
                if (rest.Length == 0 && environmentValue == null)
                {
                    throw VariableError($"--variable %{name} imports the environment variable {name}, which is not set in this process.",
                        $"Set it before running (Environment.SetEnvironmentVariable(\"{name}\", \"value\") or export {name}=value), or give a default with --variable %{name}=default.",
                        context.OriginalCommand);
                }

                content = environmentValue;
            }

            long start = 0;
            var end = long.MaxValue;
            if (rest.Length > 1 && rest[0] == '[' && char.IsDigit(rest[1]))
            {
                var close = rest.IndexOf(']');
                var range = close > 0 ? rest.Substring(1, close - 1) : string.Empty;
                var dash = range.IndexOf('-');
                if (close < 0 || dash < 0 ||
                    !long.TryParse(range.Substring(0, dash), NumberStyles.None, CultureInfo.InvariantCulture, out start) ||
                    (dash < range.Length - 1 && !long.TryParse(range.Substring(dash + 1), NumberStyles.None, CultureInfo.InvariantCulture, out end)) ||
                    end < start)
                {
                    throw VariableError($"--variable \"{input}\" has an invalid byte range: the part in [ ] must be START-END with whole numbers and START <= END.",
                        "Write it as name[START-END], for example token[0-99]@secret.txt, or name[START-] for everything from START to the end.", context.OriginalCommand);
                }

                rest = rest.Substring(close + 1);
            }

            if (content == null)
            {
                if (rest.StartsWith("@", StringComparison.Ordinal))
                {
                    var bytes = RequestShaping.ReadFileForOption(rest.Substring(1), "--variable name@file", context.OriginalCommand);
                    content = Encoding.UTF8.GetString(Slice(bytes, start, end));
                }
                else if (rest.StartsWith("=", StringComparison.Ordinal))
                {
                    var bytes = Encoding.UTF8.GetBytes(rest.Substring(1));
                    content = Encoding.UTF8.GetString(Slice(bytes, start, end));
                }
                else
                {
                    return; // curl: "Bad --variable syntax, skipping"
                }
            }

            context.Variables[name] = content;
        }

        private static byte[] Slice(byte[] bytes, long start, long end)
        {
            if (start == 0 && end == long.MaxValue)
            {
                return bytes;
            }

            if (start >= bytes.Length)
            {
                return new byte[0];
            }

            var last = Math.Min(end, bytes.Length - 1);
            var result = new byte[last - start + 1];
            Array.Copy(bytes, start, result, 0, result.Length);
            return result;
        }

        /// <summary>
        /// Expands <c>{{name}}</c> and <c>{{name:func:...}}</c> like curl's <c>varexpand()</c>. Unknown
        /// variables expand to nothing, <c>\{{</c> keeps a literal <c>{{</c>, malformed references are kept as
        /// typed, and when nothing was expanded the argument is returned untouched.
        /// </summary>
        /// <exception cref="CurlException">Exit code 2 for an unknown function or a result containing a NUL byte.</exception>
        private static string ExpandVariables(string line, ParseContext context)
        {
            var output = new StringBuilder();
            var added = false;
            var position = 0;

            while (true)
            {
                var open = line.IndexOf("{{", position, StringComparison.Ordinal);
                if (open < 0)
                {
                    break;
                }

                if (open > position && line[open - 1] == '\\')
                {
                    output.Append(line, position, open - position - 1).Append("{{");
                    position = open + 2;
                    continue;
                }

                var close = line.IndexOf("}}", open, StringComparison.Ordinal);
                if (close < 0)
                {
                    break; // curl: "missing close '}}'"; the rest is kept as typed
                }

                var inner = line.Substring(open + 2, close - open - 2);
                var colon = inner.IndexOf(':');
                var name = colon >= 0 ? inner.Substring(0, colon) : inner;
                var validName = name.Length > 0 && name.Length < MaxVariableNameLength;
                foreach (var ch in name)
                {
                    validName &= ch < 128 && (char.IsLetterOrDigit(ch) || ch == '_');
                }

                if (!validName)
                {
                    output.Append(line, position, close + 2 - position);
                    position = close + 2;
                    continue;
                }

                output.Append(line, position, open - position);
                context.Variables.TryGetValue(name, out var value);
                value = value ?? string.Empty;
                if (colon >= 0)
                {
                    value = ApplyVariableFunctions(value, inner.Substring(colon), context.OriginalCommand);
                }

                if (value.IndexOf('\0') >= 0)
                {
                    throw VariableError($"The expansion of {{{{{inner}}}}} contains a NUL byte (\\0), which cannot be placed in a text argument.",
                        "Encode it first, for example {{" + name + ":json}}, {{" + name + ":url}} or {{" + name + ":b64}}.", context.OriginalCommand);
                }

                output.Append(value);
                added = true;
                position = close + 2;
            }

            if (!added)
            {
                return line;
            }

            output.Append(line, position, line.Length - position);
            return output.ToString();
        }

        /// <summary>Runs the <c>:trim:json:url:b64:64dec</c> function chain left to right.</summary>
        private static string ApplyVariableFunctions(string value, string functions, string? command)
        {
            var names = functions.Substring(1).Split(':');
            foreach (var function in names)
            {
                switch (function)
                {
                    case "trim":
                        value = value.Trim(' ', '\t', '\r', '\n', '\v', '\f');
                        break;
                    case "json":
                        value = JsonEscape(value);
                        break;
                    case "url":
                        value = PercentEncode(value);
                        break;
                    case "b64":
                        value = value.Length == 0 ? value : Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
                        break;
                    case "64dec":
                        if (value.Length > 0)
                        {
                            try
                            {
                                value = value.Length % 4 != 0 ? "[64dec-fail]" : Encoding.UTF8.GetString(Convert.FromBase64String(value));
                            }
                            catch (FormatException)
                            {
                                value = "[64dec-fail]";
                            }
                        }

                        break;
                    default:
                        throw VariableError($"Unknown variable function \"{function}\" in \"{functions}\".",
                            "Functions are separated by colons and can be trim, json, url, b64 or 64dec, for example {{name:trim:url}}.", command);
                }
            }

            return value;
        }

        /// <summary>JSON string escaping without the surrounding quotes, as curl's <c>jsonquoted()</c>.</summary>
        private static string JsonEscape(string value)
        {
            var sb = new StringBuilder(value.Length);
            foreach (var c in value)
            {
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < 0x20)
                        {
                            sb.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            sb.Append(c);
                        }

                        break;
                }
            }

            return sb.ToString();
        }

        private static CurlException VariableError(string what, string fix, string? command)
        {
            return new CurlException(CurlException.Compose(what, fix, DocLinks.CliCompatibility), 2, command);
        }

        #region Shell variables (per CurlFlavor)

        /// <summary>
        /// Length of a shell variable name (a letter or underscore, then letters, digits and underscores)
        /// starting at <paramref name="start"/>; 0 when there is none.
        /// </summary>
        internal static int ShellVariableNameLength(string text, int start)
        {
            var i = start;
            if (i >= text.Length || !(IsAsciiLetter(text[i]) || text[i] == '_'))
            {
                return 0;
            }

            while (i < text.Length && (IsAsciiLetter(text[i]) || (text[i] >= '0' && text[i] <= '9') || text[i] == '_'))
            {
                i++;
            }

            return i - start;
        }

        private static bool IsAsciiLetter(char c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');

        /// <summary>
        /// bash/zsh: expands <c>$NAME</c> or <c>${NAME}</c> at <paramref name="index"/> (which holds the
        /// <c>$</c>) from the process environment. Returns false, leaving the text as typed, when there is
        /// no variable reference there or the variable is not set.
        /// </summary>
        internal static bool TryExpandPosixVariable(string text, int index, out string value, out int consumed)
        {
            value = string.Empty;
            consumed = 0;
            if (index + 1 >= text.Length)
            {
                return false;
            }

            string name;
            int length;
            if (text[index + 1] == '{')
            {
                length = ShellVariableNameLength(text, index + 2);
                if (length == 0 || index + 2 + length >= text.Length || text[index + 2 + length] != '}')
                {
                    return false;
                }

                name = text.Substring(index + 2, length);
                length += 3;
            }
            else
            {
                length = ShellVariableNameLength(text, index + 1);
                if (length == 0)
                {
                    return false;
                }

                name = text.Substring(index + 1, length);
                length += 1;
            }

            var found = Environment.GetEnvironmentVariable(name);
            if (found == null)
            {
                return false;
            }

            value = found;
            consumed = length;
            return true;
        }

        /// <summary>
        /// PowerShell: expands <c>$env:NAME</c>, <c>${env:NAME}</c>, <c>${NAME}</c> or <c>$NAME</c> at
        /// <paramref name="index"/> from the process environment (PowerShell session variables are not visible
        /// to .NET, so a plain <c>$NAME</c> is looked up as an environment variable). Returns false, leaving the
        /// text as typed, when there is no reference there or the variable is not set.
        /// </summary>
        internal static bool TryExpandPowerShellVariable(string text, int index, out string value, out int consumed)
        {
            value = string.Empty;
            consumed = 0;
            if (index + 1 >= text.Length)
            {
                return false;
            }

            string name;
            int length;
            if (text[index + 1] == '{')
            {
                var close = text.IndexOf('}', index + 2);
                if (close < 0)
                {
                    return false;
                }

                name = text.Substring(index + 2, close - index - 2);
                if (name.StartsWith("env:", StringComparison.OrdinalIgnoreCase))
                {
                    name = name.Substring(4);
                }

                length = close - index + 1;
            }
            else
            {
                var start = index + 1;
                if (string.Compare(text, start, "env:", 0, 4, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    start += 4;
                }

                var nameLength = ShellVariableNameLength(text, start);
                if (nameLength == 0)
                {
                    return false;
                }

                name = text.Substring(start, nameLength);
                length = start + nameLength - index;
            }

            var found = name.Length == 0 ? null : Environment.GetEnvironmentVariable(name);
            if (found == null)
            {
                return false;
            }

            value = found;
            consumed = length;
            return true;
        }

        /// <summary>
        /// cmd.exe: expands <c>%NAME%</c> at <paramref name="index"/> (which holds the first <c>%</c>) when the
        /// environment variable is set. An unset variable stays as typed, like an interactive cmd.exe prompt, and
        /// so do percent-encoded URL bytes such as <c>%20</c>, which never name a set variable.
        /// </summary>
        internal static bool TryExpandCmdVariable(string text, int index, out string value, out int consumed)
        {
            value = string.Empty;
            consumed = 0;
            var close = text.IndexOf('%', index + 1);
            if (close <= index + 1)
            {
                return false;
            }

            var name = text.Substring(index + 1, close - index - 1);
            foreach (var c in name)
            {
                if (char.IsWhiteSpace(c) || c == '"' || c == '\'' || c == '^' || c == '=')
                {
                    return false;
                }
            }

            var found = Environment.GetEnvironmentVariable(name);
            if (found == null)
            {
                return false;
            }

            value = found;
            consumed = close - index + 1;
            return true;
        }

        #endregion
    }
}
