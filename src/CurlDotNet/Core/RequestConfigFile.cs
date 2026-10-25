/***************************************************************************
 * RequestConfigFile - curl's -K / --config files
 *
 * Part of CommandParser. Follows curl's src/tool_parsecfg.c: one option per line, written
 * with or without leading dashes ("--data x", "data = x", "data: x", "-d x"), an argument
 * that is either a word or a "double quoted string" with \t \n \r \v and \X escapes,
 * '#' comment lines, and nested -K. The default ~/.curlrc is NOT read unless the application opts
 * in with Curl.DefaultReadConfigFile: a library should not change its requests because of a file in
 * the user's home directory. When it is on, the file is found like curl's src/tool_findfile.c.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
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
                    CurlException.Compose(
                        $"-K/--config files are nested more than {MaxConfigDepth} levels deep: \"{path}\" was included again at level {context.ConfigDepth + 1}.",
                        "Look for a config file with a config = \"...\" (or -K) line that names itself or a file that includes it back, and remove that line.",
                        DocLinks.CliCompatibility),
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
        /// True when the command's first argument switches the default config file off, like curl's
        /// <c>operate()</c>: any first argument starting with <c>-q</c> (so <c>-q</c> and <c>-qs</c>) or exactly <c>--disable</c>.
        /// </summary>
        internal static bool DisablesDefaultConfigFile(IList<string> args)
        {
            if (args.Count == 0)
            {
                return false;
            }

            var first = args[0];
            return first.StartsWith("-q", StringComparison.Ordinal) || first == "--disable";
        }

        /// <summary>
        /// Reads the default config file (<c>.curlrc</c>) before the command's own arguments when
        /// <see cref="Curl.DefaultReadConfigFile"/> is on and the command does not start with <c>-q</c>/<c>--disable</c>.
        /// </summary>
        /// <returns>The path that was read, or null when the opt-in is off, the command disabled it, or no file was found.</returns>
        private string? IncludeDefaultConfigFile(IList<string> args, ParseContext context)
        {
            if (!Curl.DefaultReadConfigFile || DisablesDefaultConfigFile(args))
            {
                return null;
            }

            var path = FindDefaultConfigFile(Environment.GetEnvironmentVariable, IsWindows, AccountHomeDirectory);
            if (path != null)
            {
                IncludeConfigFile(path, context);
            }

            return path;
        }

        /// <summary>
        /// curl's <c>Note: Read config file from '...'</c>: written to the <c>--stderr</c> file or standard error once the
        /// command is parsed, only when it asks for <c>-v</c>/<c>--trace</c>/<c>--trace-ascii</c> and not <c>-s</c> (curl's
        /// <c>notef()</c> rule).
        /// </summary>
        private static void WriteDefaultConfigNote(string path, ParseContext context)
        {
            var tracing = false;
            var silent = false;
            string? stderrFile = null;
            foreach (var group in context.Groups)
            {
                var o = group.Options;
                tracing |= o.Verbose || !string.IsNullOrEmpty(o.TraceFile) || !string.IsNullOrEmpty(o.TraceAsciiFile);
                silent |= o.Silent;
                if (!string.IsNullOrEmpty(o.StderrFile))
                {
                    stderrFile = o.StderrFile;
                }
            }

            if (!tracing || silent)
            {
                return;
            }

            var text = $"Note: Read config file from '{path}'\n";
            if (stderrFile != null && stderrFile != "-")
            {
                try
                {
                    File.AppendAllText(stderrFile, text);
                    return;
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    // curl falls back to the terminal when --stderr cannot be opened
                }
            }

            Console.Error.Write(text);
        }

        private static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>The account's home directory from the user database (curl's <c>getpwuid()</c> fallback); null on Windows.</summary>
        private static string? AccountHomeDirectory()
        {
            if (IsWindows)
            {
                return null;
            }

            try
            {
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                return string.IsNullOrEmpty(home) ? null : home;
            }
            catch (PlatformNotSupportedException)
            {
                return null;
            }
        }

        /// <summary>
        /// Finds the default config file the way curl's <c>findfile(".curlrc", CURLRC_DOTSCORE)</c> does (src/tool_findfile.c):
        /// <c>$CURL_HOME/.curlrc</c>, <c>$XDG_CONFIG_HOME/curlrc</c>, <c>$HOME/.curlrc</c>, on Windows
        /// <c>%USERPROFILE%</c>, <c>%APPDATA%</c> and <c>%USERPROFILE%\Application Data</c>, then (only when
        /// <c>XDG_CONFIG_HOME</c> is unset or empty) <c>$CURL_HOME/.config/curlrc</c> and <c>$HOME/.config/curlrc</c>, and finally
        /// <c>.curlrc</c> in the directory <paramref name="accountHome"/> returns. On Windows <c>_curlrc</c> is tried after each
        /// <c>.curlrc</c> of the variable-based places. Unset and empty variables are skipped; the first file that can be opened wins.
        /// </summary>
        /// <param name="getEnvironmentVariable">Reads an environment variable (null when unset); injectable for tests.</param>
        /// <param name="windows">True to use the Windows rules (extra places, <c>\</c> separator, <c>_curlrc</c>).</param>
        /// <param name="accountHome">Returns the account's home directory, or null; called only when nothing else matched.</param>
        /// <returns>The full path of the file to read, or null when there is none.</returns>
        internal static string? FindDefaultConfigFile(Func<string, string?> getEnvironmentVariable, bool windows, Func<string?> accountHome)
        {
            // curl's conf_list[]: environment variable, text appended to its value, and whether the file name drops its dot
            var places = new List<ConfigPlace>
            {
                new ConfigPlace("CURL_HOME", string.Empty, false),
                new ConfigPlace("XDG_CONFIG_HOME", string.Empty, true),
                new ConfigPlace("HOME", string.Empty, false),
            };
            if (windows)
            {
                places.Add(new ConfigPlace("USERPROFILE", string.Empty, false));
                places.Add(new ConfigPlace("APPDATA", string.Empty, false));
                places.Add(new ConfigPlace("USERPROFILE", "\\Application Data", false));
            }

            // these are for .curlrc if XDG_CONFIG_HOME is not defined
            places.Add(new ConfigPlace("CURL_HOME", "/.config", true));
            places.Add(new ConfigPlace("HOME", "/.config", true));

            var separator = windows ? "\\" : "/";

            // CURLRC_DOTSCORE is 2 on Windows (".curlrc" then "_curlrc") and 1 elsewhere. findfile() drops it to 0 at the first
            // without-dot place that is set (XDG_CONFIG_HOME), which is what turns the .config fallbacks after it off.
            var dotscore = windows ? 2 : 1;
            foreach (var place in places)
            {
                var home = getEnvironmentVariable(place.Variable);
                if (string.IsNullOrEmpty(home))
                {
                    continue;
                }

                home += place.Append;

                var name = ".curlrc";
                var underscoreToo = dotscore > 1;
                if (place.WithoutDot)
                {
                    if (dotscore == 0)
                    {
                        continue;
                    }

                    name = "curlrc";
                    dotscore = 0;
                    underscoreToo = false;
                }

                var found = CheckHome(home!, name, separator, underscoreToo);
                if (found != null)
                {
                    return found;
                }
            }

            var account = accountHome();
            return string.IsNullOrEmpty(account) ? null : CheckHome(account!, ".curlrc", separator, false);
        }

        /// <summary>One entry of curl's <c>conf_list[]</c> in src/tool_findfile.c.</summary>
        private sealed class ConfigPlace
        {
            public ConfigPlace(string variable, string append, bool withoutDot)
            {
                Variable = variable;
                Append = append;
                WithoutDot = withoutDot;
            }

            /// <summary>The environment variable naming the directory.</summary>
            public string Variable { get; }

            /// <summary>Text appended to the variable's value, such as <c>/.config</c>.</summary>
            public string Append { get; }

            /// <summary>True when the file there is <c>curlrc</c> rather than <c>.curlrc</c>.</summary>
            public bool WithoutDot { get; }
        }

        /// <summary>curl's <c>checkhome()</c>: <c>home/name</c>, and with <paramref name="underscoreToo"/> also <c>home/_curlrc</c>.</summary>
        private static string? CheckHome(string home, string name, string separator, bool underscoreToo)
        {
            var candidates = underscoreToo
                ? new[] { home + separator + name, home + separator + "_" + name.Substring(1) }
                : new[] { home + separator + name };
            foreach (var candidate in candidates)
            {
                try
                {
                    using (File.OpenRead(candidate))
                    {
                        return candidate;
                    }
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException || ex is NotSupportedException)
                {
                    // not there or not readable: try the next place, like curl's failed open()
                }
            }

            return null;
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
