/***************************************************************************
 * RequestConfigFile - curl's -K / --config files
 *
 * Part of CommandParser. Follows curl's src/tool_parsecfg.c: one option per line, written
 * with or without leading dashes ("--data x", "data = x", "data: x", "-d x"), an argument
 * that is either a word or a "double quoted string" with \t \n \r \v and \X escapes,
 * '#' comment lines, and nested -K. The default ~/.curlrc is deliberately NOT read:
 * a library should not change its requests because of a file in the user's home directory.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    internal partial class CommandParser
    {
        /// <summary>curl stops following -K inside -K after this many levels.</summary>
        private const int MaxConfigDepth = 10;

        /// <summary>Reads a <c>-K</c> file and processes its options as if they had been typed in its place.</summary>
        /// <exception cref="CurlReadErrorException">Exit code 26 when the file cannot be read.</exception>
        /// <exception cref="CurlException">Exit code 2 when config files include each other too deeply.</exception>
        private void IncludeConfigFile(string path, ParseContext context)
        {
            if (context.ConfigDepth >= MaxConfigDepth)
            {
                throw new CurlException(
                    $"-K/--config files are nested more than {MaxConfigDepth} levels deep (\"{path}\" was included again). " +
                    "Check the config files for a -K line that includes itself. See " + RequestShaping.DocsBase + "index.md",
                    2,
                    context.OriginalCommand);
            }

            var lines = ReadAllLinesForOption(path, "-K/--config", context.OriginalCommand);
            context.ConfigDepth++;
            try
            {
                foreach (var line in lines)
                {
                    var args = ParseConfigLine(line);
                    if (args.Count > 0)
                    {
                        ProcessArguments(args, context);
                    }
                }
            }
            finally
            {
                context.ConfigDepth--;
            }
        }

        /// <summary>
        /// Turns one config line into the argument vector it stands for: <c>data = "a b"</c> becomes
        /// <c>--data</c>, <c>a b</c>. Options that take no argument drop a stray one, as curl does
        /// (with a warning), and unknown options never swallow it as a URL.
        /// </summary>
        private static List<string> ParseConfigLine(string rawLine)
        {
            var result = new List<string>();
            var line = rawLine.TrimEnd('\r', '\n');
            var i = 0;
            while (i < line.Length && (line[i] == ' ' || line[i] == '\t'))
            {
                i++;
            }

            if (i >= line.Length || line[i] == '#' || line[i] == '/' || line[i] == '*')
            {
                return result;
            }

            var dashed = line[i] == '-';
            var start = i;
            while (i < line.Length && !char.IsWhiteSpace(line[i]) && (dashed || (line[i] != ':' && line[i] != '=')))
            {
                i++;
            }

            var option = line.Substring(start, i - start);

            // Skip the separator: whitespace and, for undashed options, one '=' or ':'
            while (i < line.Length && (char.IsWhiteSpace(line[i]) || (!dashed && (line[i] == ':' || line[i] == '='))))
            {
                i++;
            }

            string? param = null;
            if (i < line.Length)
            {
                if (line[i] == '"')
                {
                    param = ReadQuotedConfigParameter(line, i + 1);
                }
                else
                {
                    var paramStart = i;
                    while (i < line.Length && !char.IsWhiteSpace(line[i]))
                    {
                        i++;
                    }

                    // curl warns "argument with unquoted whitespace" and uses the first word only
                    param = line.Substring(paramStart, i - paramStart);
                }
            }

            if (!dashed)
            {
                option = "--" + option;
            }

            bool takesValue;
            if (option.StartsWith("--", StringComparison.Ordinal))
            {
                var name = option.Substring(2);
                var eq = name.IndexOf('=');
                if (TryResolveLongOption(name, out var spec, out _, out _))
                {
                    takesValue = spec.TakesValue;
                }
                else if (eq > 0 && TryResolveLongOption(name.Substring(0, eq), out spec, out _, out _))
                {
                    takesValue = false; // --name=value already carries its value
                }
                else
                {
                    result.Add(option);
                    return result;
                }
            }
            else
            {
                takesValue = option.Length == 2 && ShortOptionTable.TryGetValue(option[1], out var shortSpec) && shortSpec.TakesValue;
            }

            result.Add(option);
            if (takesValue)
            {
                result.Add(param ?? string.Empty);
            }

            return result;
        }

        /// <summary>Reads a double-quoted config argument starting after its opening quote.</summary>
        private static string ReadQuotedConfigParameter(string line, int start)
        {
            var sb = new StringBuilder();
            for (var i = start; i < line.Length; i++)
            {
                var c = line[i];
                if (c == '"')
                {
                    break;
                }

                if (c == '\\' && i + 1 < line.Length)
                {
                    var next = line[++i];
                    switch (next)
                    {
                        case 't': sb.Append('\t'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 'v': sb.Append('\v'); break;
                        default: sb.Append(next); break;
                    }

                    continue;
                }

                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
