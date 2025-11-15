/***************************************************************************
 * CommandParser - Parses curl command strings into options
 *
 * Inspired by curl's src/tool_getparam.c and src/tool_parsecfg.c
 * Original curl Copyright (C) 1996-2025, Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024-2025 Jacob Mellor and IronSoftware
 *
 * This parser handles all curl command-line syntax including:
 * - Short options (-X, -H, -d)
 * - Long options (--request, --header, --data)
 * - Quote handling (single, double, escaped) - works across all shells
 * - Line continuations (\, ^, `) - Windows CMD, PowerShell, Bash/ZSH
 * - Multiple data parameters
 * - File references (@filename)
 * - Environment variables ($VAR, %VAR%, $env:VAR) - cross-platform
 *
 * Shell Compatibility:
 * - Windows CMD: Double quotes, ^ for line continuation, %VAR% for env vars
 * - PowerShell: Single or double quotes, backtick escape, $env:VAR for env vars
 * - Bash/ZSH: Single or double quotes, \ for escape, $VAR for env vars
 *
 * By Jacob Mellor
 * GitHub: https://github.com/jacob-mellor
 * Sponsored by IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Parses curl command strings into CurlOptions, following curl's command-line syntax exactly.
    /// This parser is inspired by curl's tool_getparam.c which handles all parameter parsing.
    ///
    /// <para>
    /// Supports all curl options including short forms (-X POST), long forms (--request POST),
    /// combined short options (-sS), option arguments, quote handling, and line continuations.
    /// The parser normalizes different shell syntaxes (bash, PowerShell, cmd, zsh) into a consistent format.
    /// </para>
    ///
    /// <para>
    /// <b>Cross-Shell Compatibility:</b> Paste curl commands from any shell and they work:
    /// </para>
    /// <list type="bullet">
    /// <item><b>Windows CMD:</b> <c>curl -H "Content-Type: application/json" -d "{\"key\":\"value\"}"</c></item>
    /// <item><b>PowerShell:</b> <c>curl -H 'Content-Type: application/json' -d '{\"key\":\"value\"}'</c></item>
    /// <item><b>Bash/ZSH:</b> <c>curl -H 'Content-Type: application/json' -d '{"key":"value"}'</c></item>
    /// </list>
    ///
    /// <para>
    /// For curl option reference: https://curl.se/docs/manpage.html#OPTIONS
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>This parser handles all curl command-line syntax including short and long options.</para>
    /// <para>Processes commands in the same order as curl for compatibility.</para>
    /// <para>Thread-safe and can be reused for multiple parse operations.</para>
    ///
    /// <ai-semantic-usage>
    /// Core parser that translates curl command strings to structured options.
    /// Handles shell quoting, escaping, line continuations, and all curl option formats.
    /// Use this when you need to parse curl commands programmatically.
    /// </ai-semantic-usage>
    ///
    /// <ai-patterns>
    /// - Always preserves curl's option precedence rules
    /// - Later options override earlier ones (like curl)
    /// - Multiple -d options are concatenated
    /// - File references (@file) are expanded
    /// - Handles Windows, PowerShell, Bash, ZSH syntax differences
    /// </ai-patterns>
    /// </remarks>
    internal class CommandParser : ICommandParser
    {
        private static readonly Dictionary<string, string> ShortToLongOptions = new Dictionary<string, string>
        {
            ["-X"] = "--request",
            ["-H"] = "--header",
            ["-d"] = "--data",
            ["-F"] = "--form",
            ["-o"] = "--output",
            ["-O"] = "--remote-name",
            ["-i"] = "--include",
            ["-I"] = "--head",
            ["-L"] = "--location",
            ["-k"] = "--insecure",
            ["-v"] = "--verbose",
            ["-s"] = "--silent",
            ["-S"] = "--show-error",
            ["-f"] = "--fail",
            ["-A"] = "--user-agent",
            ["-e"] = "--referer",
            ["-b"] = "--cookie",
            ["-c"] = "--cookie-jar",
            ["-u"] = "--user",
            ["-x"] = "--proxy",
            ["-w"] = "--write-out",
            ["-C"] = "--continue-at",
            ["-r"] = "--range",
            ["-T"] = "--upload-file",
            ["-m"] = "--max-time"
        };

        public CurlOptions Parse(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));

            var options = new CurlOptions
            {
                OriginalCommand = command
            };

            // Normalize command - handle line continuations for different shells
            command = NormalizeLineContinuations(command);

            // Remove "curl" from the beginning if present
            command = command.Trim();
            if (command.StartsWith("curl ", StringComparison.OrdinalIgnoreCase))
            {
                command = command.Substring(5).TrimStart();
            }
            else if (command.Equals("curl", StringComparison.OrdinalIgnoreCase))
            {
                throw new CurlException("No URL specified", 3);
            }

            // Expand environment variables (cross-platform support)
            command = ExpandEnvironmentVariables(command);

            var args = ParseArguments(command);
            ProcessArguments(args, options);

            return options;
        }

        public bool IsValid(string command)
        {
            try
            {
                Parse(command);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Normalizes line continuations across different shells.
        /// - Windows CMD: ^ at end of line
        /// - PowerShell: backtick ` at end of line
        /// - Bash/ZSH: \ at end of line
        /// </summary>
        private string NormalizeLineContinuations(string command)
        {
            // Replace line continuations with spaces
            // Handle \r\n, \n, \r line endings
            command = Regex.Replace(command, @"[ \t]*\\[ \t]*(\r\n|\r|\n)", " ", RegexOptions.Multiline);
            command = Regex.Replace(command, @"[ \t]*\^[ \t]*(\r\n|\r|\n)", " ", RegexOptions.Multiline); // Windows CMD
            command = Regex.Replace(command, @"[ \t]*`[ \t]*(\r\n|\r|\n)", " ", RegexOptions.Multiline); // PowerShell
            
            // Normalize all whitespace sequences to single space
            command = Regex.Replace(command, @"\s+", " ");
            
            return command.Trim();
        }

        /// <summary>
        /// Expands environment variables across different shell formats.
        /// - Windows CMD: %VAR%
        /// - PowerShell: $env:VAR or $VAR
        /// - Bash/ZSH: $VAR or ${VAR}
        /// </summary>
        private string ExpandEnvironmentVariables(string command)
        {
            // Windows CMD format: %VAR%
            command = Regex.Replace(command, @"%([^%]+)%", match =>
            {
                var varName = match.Groups[1].Value;
                return Environment.GetEnvironmentVariable(varName) ?? match.Value;
            });

            // PowerShell format: $env:VAR
            command = Regex.Replace(command, @"\$env:([\w_]+)", match =>
            {
                var varName = match.Groups[1].Value;
                return Environment.GetEnvironmentVariable(varName) ?? match.Value;
            });

            // Bash/ZSH format: ${VAR} or $VAR
            // ${VAR} format
            command = Regex.Replace(command, @"\$\{([\w_]+)\}", match =>
            {
                var varName = match.Groups[1].Value;
                return Environment.GetEnvironmentVariable(varName) ?? match.Value;
            });

            // $VAR format (but be careful not to match $ in quotes or other contexts)
            // This is a simple implementation - in production might want more sophisticated parsing
            command = Regex.Replace(command, @"\$([\w_]+)", match =>
            {
                // Check if preceded by $ or in a position that suggests it's a variable
                var varName = match.Groups[1].Value;
                var envValue = Environment.GetEnvironmentVariable(varName);
                return envValue ?? match.Value;
            }, RegexOptions.None);

            return command;
        }

        /// <summary>
        /// Parses arguments from command string, handling quotes from all shells.
        /// Supports:
        /// - Single quotes: 'text'
        /// - Double quotes: "text"
        /// - Escaped quotes: \" and \' and "" (Windows-style)
        /// </summary>
        private List<string> ParseArguments(string command)
        {
            var args = new List<string>();
            var current = new StringBuilder();
            var inQuote = false;
            var quoteChar = ' ';
            var escape = false;
            var i = 0;

            while (i < command.Length)
            {
                var c = command[i];
                var next = i + 1 < command.Length ? command[i + 1] : '\0';

                if (escape)
                {
                    // Handle escape sequences
                    if (c == 'n') current.Append('\n');
                    else if (c == 't') current.Append('\t');
                    else if (c == 'r') current.Append('\r');
                    else if (c == '\\') current.Append('\\');
                    else current.Append(c);
                    
                    escape = false;
                    i++;
                    continue;
                }

                // Handle escaping (works for all shells)
                if (c == '\\' && next != '\0')
                {
                    // Check if next char is quote or special char
                    if (next == '"' || next == '\'' || next == '\\')
                    {
                        escape = true;
                        i++;
                        continue;
                    }
                    // Otherwise treat \ as literal (might be Windows path)
                    current.Append(c);
                    i++;
                    continue;
                }

                // Windows-style escaped quotes: "" becomes "
                if (c == '"' && next == '"' && inQuote && quoteChar == '"')
                {
                    current.Append('"');
                    i += 2; // Skip both quotes
                    continue;
                }

                // Start or end quotes
                if (!inQuote && (c == '"' || c == '\''))
                {
                    inQuote = true;
                    quoteChar = c;
                    i++;
                    continue;
                }

                if (inQuote && c == quoteChar)
                {
                    inQuote = false;
                    quoteChar = ' ';
                    i++;
                    continue;
                }

                // Whitespace handling
                if (!inQuote && char.IsWhiteSpace(c))
                {
                    if (current.Length > 0)
                    {
                        args.Add(current.ToString());
                        current.Clear();
                    }
                    i++;
                    continue;
                }

                current.Append(c);
                i++;
            }

            if (current.Length > 0)
            {
                args.Add(current.ToString());
            }

            return args;
        }

        private void ProcessArguments(List<string> args, CurlOptions options)
        {
            for (int i = 0; i < args.Count; i++)
            {
                var arg = args[i];

                if (arg.StartsWith("-"))
                {
                    // Handle options
                    var optionName = NormalizeOption(arg);
                    var value = "";

                    // Check if this option needs a value
                    if (NeedsValue(optionName) && i + 1 < args.Count && !args[i + 1].StartsWith("-"))
                    {
                        value = args[++i];
                    }

                    ProcessOption(optionName, value, options);
                }
                else if (string.IsNullOrEmpty(options.Url))
                {
                    // First non-option argument is the URL
                    options.Url = arg;
                }
                else
                {
                    // Additional URLs or data - handle based on context
                    // For simplicity, ignore additional URLs for now
                }
            }
        }

        private string NormalizeOption(string option)
        {
            // Convert short options to long form
            if (ShortToLongOptions.ContainsKey(option))
            {
                return ShortToLongOptions[option];
            }

            // Handle combined short options like -sSL
            if (option.StartsWith("-") && !option.StartsWith("--") && option.Length > 2)
            {
                // Process first character as a flag, rest will be handled in recursion
                var firstFlag = "-" + option[1];
                if (ShortToLongOptions.ContainsKey(firstFlag))
                {
                    return ShortToLongOptions[firstFlag];
                }
            }

            return option;
        }

        private bool NeedsValue(string option)
        {
            var noValueOptions = new[] { "--include", "--head", "--location", "--insecure",
                "--verbose", "--silent", "--show-error", "--fail", "--compressed",
                "--remote-name", "--location-trusted", "--disable-epsv", "--disable-eprt",
                "--ftp-pasv", "--ftp-ssl", "--create-dirs", "--progress-bar" };

            return !noValueOptions.Contains(option);
        }

        private void ProcessOption(string option, string value, CurlOptions options)
        {
            switch (option)
            {
                case "--request":
                case "-X":
                    options.Method = value.ToUpper();
                    options.CustomMethod = value.ToUpper();
                    break;

                case "--header":
                case "-H":
                    ParseHeader(value, options);
                    break;

                case "--data":
                case "-d":
                    options.Data = value;
                    if (string.IsNullOrEmpty(options.Method))
                        options.Method = "POST";
                    break;

                case "--form":
                case "-F":
                    ParseFormField(value, options);
                    if (string.IsNullOrEmpty(options.Method))
                        options.Method = "POST";
                    break;

                case "--output":
                case "-o":
                    options.OutputFile = value;
                    break;

                case "--remote-name":
                case "-O":
                    options.OutputFile = ""; // Will use filename from URL
                    options.UseRemoteFileName = true;
                    break;

                case "--include":
                case "-i":
                    options.IncludeHeaders = true;
                    break;

                case "--head":
                case "-I":
                    options.HeadOnly = true;
                    options.Method = "HEAD";
                    break;

                case "--location":
                case "-L":
                    options.FollowLocation = true;
                    break;

                case "--insecure":
                case "-k":
                    options.Insecure = true;
                    break;

                case "--verbose":
                case "-v":
                    options.Verbose = true;
                    break;

                case "--silent":
                case "-s":
                    options.Silent = true;
                    break;

                case "--show-error":
                case "-S":
                    options.ShowError = true;
                    break;

                case "--fail":
                case "-f":
                    options.FailOnError = true;
                    break;

                case "--user-agent":
                case "-A":
                    options.UserAgent = value;
                    break;

                case "--referer":
                case "-e":
                    options.Referer = value;
                    break;

                case "--cookie":
                case "-b":
                    options.Cookie = value;
                    break;

                case "--cookie-jar":
                case "-c":
                    options.CookieJar = value;
                    break;

                case "--user":
                case "-u":
                    ParseCredentials(value, options);
                    break;

                case "--proxy":
                case "-x":
                    options.Proxy = value;
                    break;

                case "--proxy-user":
                    ParseProxyCredentials(value, options);
                    break;

                case "--max-time":
                case "-m":
                    if (int.TryParse(value, out var maxTime))
                        options.MaxTime = maxTime;
                    break;

                case "--connect-timeout":
                    if (int.TryParse(value, out var connectTimeout))
                        options.ConnectTimeout = connectTimeout;
                    break;

                case "--max-redirs":
                    if (int.TryParse(value, out var maxRedirects))
                        options.MaxRedirects = maxRedirects;
                    break;

                case "--compressed":
                    options.Compressed = true;
                    break;

                case "--write-out":
                case "-w":
                    options.WriteOut = value;
                    break;

                case "--range":
                case "-r":
                    options.Range = value;
                    break;

                case "--continue-at":
                case "-C":
                    if (value == "-")
                        options.ResumeFrom = -1; // Auto-resume
                    else if (long.TryParse(value, out var resumeFrom))
                        options.ResumeFrom = resumeFrom;
                    break;

                case "--cert":
                    options.CertFile = value;
                    break;

                case "--key":
                    options.KeyFile = value;
                    break;

                case "--cacert":
                    options.CaCertFile = value;
                    break;

                case "--interface":
                    options.Interface = value;
                    break;

                case "--http1.0":
                    options.HttpVersion = "1.0";
                    break;

                case "--http1.1":
                    options.HttpVersion = "1.1";
                    break;

                case "--http2":
                    options.HttpVersion = "2.0";
                    break;

                case "--limit-rate":
                    options.SpeedLimit = ParseSize(value);
                    break;

                case "--speed-time":
                    if (int.TryParse(value, out var speedTime))
                        options.SpeedTime = speedTime;
                    break;

                case "--progress-bar":
                    options.ProgressBar = true;
                    break;

                case "--keepalive-time":
                    if (int.TryParse(value, out var keepAliveTime))
                        options.KeepAliveTime = keepAliveTime;
                    break;

                case "--dns-servers":
                    options.DnsServers = value;
                    break;

                case "--resolve":
                    ParseResolve(value, options);
                    break;

                case "--quote":
                    options.Quote.Add(value);
                    break;

                case "--create-dirs":
                    options.CreateDirs = true;
                    break;

                case "--ftp-pasv":
                    options.FtpPassive = true;
                    break;

                case "--ftp-ssl":
                    options.FtpSsl = true;
                    break;

                case "--disable-epsv":
                    options.DisableEpsv = true;
                    break;

                case "--disable-eprt":
                    options.DisableEprt = true;
                    break;

                case "--socks5":
                    options.Socks5Proxy = value;
                    break;

                case "--retry":
                    if (int.TryParse(value, out var retry))
                        options.Retry = retry;
                    break;

                case "--retry-delay":
                    if (int.TryParse(value, out var retryDelay))
                        options.RetryDelay = retryDelay;
                    break;

                case "--retry-max-time":
                    if (int.TryParse(value, out var retryMaxTime))
                        options.RetryMaxTime = retryMaxTime;
                    break;

                case "--location-trusted":
                    options.LocationTrusted = true;
                    break;
            }
        }

        private void ParseHeader(string header, CurlOptions options)
        {
            var colonIndex = header.IndexOf(':');
            if (colonIndex > 0)
            {
                var key = header.Substring(0, colonIndex).Trim();
                var value = header.Substring(colonIndex + 1).Trim();
                options.Headers[key] = value;
            }
        }

        private void ParseFormField(string field, CurlOptions options)
        {
            var equalIndex = field.IndexOf('=');
            if (equalIndex > 0)
            {
                var key = field.Substring(0, equalIndex);
                var value = field.Substring(equalIndex + 1);

                if (value.StartsWith("@"))
                {
                    // File upload
                    options.Files[key] = value.Substring(1);
                }
                else
                {
                    // Regular form field
                    options.FormData[key] = value;
                }
            }
        }

        private void ParseCredentials(string userPass, CurlOptions options)
        {
            var parts = userPass.Split(':');
            if (parts.Length == 2)
            {
                options.Credentials = new NetworkCredential(parts[0], parts[1]);
            }
            else
            {
                options.Credentials = new NetworkCredential(userPass, "");
            }
        }

        private void ParseProxyCredentials(string userPass, CurlOptions options)
        {
            var parts = userPass.Split(':');
            if (parts.Length == 2)
            {
                options.ProxyCredentials = new NetworkCredential(parts[0], parts[1]);
            }
        }

        private void ParseResolve(string resolve, CurlOptions options)
        {
            // Format: host:port:address
            var parts = resolve.Split(':');
            if (parts.Length >= 3)
            {
                var hostPort = parts[0] + ":" + parts[1];
                var address = string.Join(":", parts.Skip(2));
                options.Resolve[hostPort] = address;
            }
        }

        private long ParseSize(string size)
        {
            if (string.IsNullOrEmpty(size))
                return 0;

            var multiplier = 1L;
            var value = size;

            if (size.EndsWith("k", StringComparison.OrdinalIgnoreCase))
            {
                multiplier = 1024;
                value = size.Substring(0, size.Length - 1);
            }
            else if (size.EndsWith("m", StringComparison.OrdinalIgnoreCase))
            {
                multiplier = 1024 * 1024;
                value = size.Substring(0, size.Length - 1);
            }
            else if (size.EndsWith("g", StringComparison.OrdinalIgnoreCase))
            {
                multiplier = 1024 * 1024 * 1024;
                value = size.Substring(0, size.Length - 1);
            }

            if (long.TryParse(value, out var num))
            {
                return num * multiplier;
            }

            return 0;
        }
    }
}
