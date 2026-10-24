/***************************************************************************
 * RequestVariables - curl's --variable and --expand-OPTION
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
                    throw VariableError($"--variable %{name} imports the environment variable {name}, which is not set. " +
                        $"Set it, or give a default with --variable %{name}=default.", context.OriginalCommand);
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
                    throw VariableError($"--variable \"{input}\" has an invalid byte range; write it as name[START-END] with START <= END " +
                        "(for example name[0-99]@file), or name[START-] for the rest of the content.", context.OriginalCommand);
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
                    throw VariableError($"The expansion of {{{{{inner}}}}} contains a NUL byte, which cannot be sent as text. " +
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
                        throw VariableError($"Unknown variable function \"{function}\" in \"{functions}\". " +
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

        private static CurlException VariableError(string message, string? command)
        {
            return new CurlException(message + " See " + RequestShaping.DocsBase + "index.md", 2, command);
        }
    }
}
