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
using System.Globalization;
using System.IO;
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
    internal partial class CommandParser : ICommandParser
    {
        #region curl option table

        /// <summary>Marks an option that has no single-letter form.</summary>
        private const char NoShort = '\0';

        /// <summary>
        /// One row of curl's option table: the long name (without <c>--</c>), the single-letter alias
        /// (or <see cref="NoShort"/>) and whether the option consumes an argument.
        /// </summary>
        private sealed class CurlOptionSpec
        {
            public CurlOptionSpec(string longName, char shortName, bool takesValue)
            {
                LongName = longName;
                ShortName = shortName;
                TakesValue = takesValue;
            }

            /// <summary>Long name without the leading dashes, for example <c>data-binary</c>.</summary>
            public string LongName { get; }

            /// <summary>Single-letter alias, or <see cref="NoShort"/>.</summary>
            public char ShortName { get; }

            /// <summary>True when the option consumes the next argument (or the attached value).</summary>
            public bool TakesValue { get; }
        }

        private static CurlOptionSpec Opt(string longName, char shortName, bool takesValue) =>
            new CurlOptionSpec(longName, shortName, takesValue);

        /// <summary>
        /// Every option of curl 8.7.1 (<c>curl --help all</c>) plus the newer ones curl's own test suite
        /// uses (<c>--follow</c>, <c>--out-null</c>, <c>--show-headers</c>, <c>--skip-existing</c>...).
        /// The parser uses it so that any real option consumes its argument, even when CurlDotNet does not
        /// implement it, and so that the single-letter aliases match curl's exactly.
        /// </summary>
        private static readonly CurlOptionSpec[] CurlOptionTable =
        {
            Opt("abstract-unix-socket", NoShort, true),
            Opt("alt-svc", NoShort, true),
            Opt("anyauth", NoShort, false),
            Opt("append", 'a', false),
            Opt("aws-sigv4", NoShort, true),
            Opt("basic", NoShort, false),
            Opt("ca-native", NoShort, false),
            Opt("cacert", NoShort, true),
            Opt("capath", NoShort, true),
            Opt("cert", 'E', true),
            Opt("cert-status", NoShort, false),
            Opt("cert-type", NoShort, true),
            Opt("ciphers", NoShort, true),
            Opt("compressed", NoShort, false),
            Opt("compressed-ssh", NoShort, false),
            Opt("config", 'K', true),
            Opt("connect-timeout", NoShort, true),
            Opt("connect-to", NoShort, true),
            Opt("continue-at", 'C', true),
            Opt("cookie", 'b', true),
            Opt("cookie-jar", 'c', true),
            Opt("create-dirs", NoShort, false),
            Opt("create-file-mode", NoShort, true),
            Opt("crlf", NoShort, false),
            Opt("crlfile", NoShort, true),
            Opt("curves", NoShort, true),
            Opt("data", 'd', true),
            Opt("data-ascii", NoShort, true),
            Opt("data-binary", NoShort, true),
            Opt("data-raw", NoShort, true),
            Opt("data-urlencode", NoShort, true),
            Opt("delegation", NoShort, true),
            Opt("digest", NoShort, false),
            Opt("disable", 'q', false),
            Opt("disable-eprt", NoShort, false),
            Opt("disable-epsv", NoShort, false),
            Opt("disallow-username-in-url", NoShort, false),
            Opt("dns-interface", NoShort, true),
            Opt("dns-ipv4-addr", NoShort, true),
            Opt("dns-ipv6-addr", NoShort, true),
            Opt("dns-servers", NoShort, true),
            Opt("doh-cert-status", NoShort, false),
            Opt("doh-insecure", NoShort, false),
            Opt("doh-url", NoShort, true),
            Opt("dump-ca-embed", NoShort, false),
            Opt("dump-header", 'D', true),
            Opt("ech", NoShort, true),
            Opt("egd-file", NoShort, true),
            Opt("engine", NoShort, true),
            Opt("etag-compare", NoShort, true),
            Opt("etag-save", NoShort, true),
            Opt("expect100-timeout", NoShort, true),
            Opt("fail", 'f', false),
            Opt("fail-early", NoShort, false),
            Opt("fail-with-body", NoShort, false),
            Opt("false-start", NoShort, false),
            Opt("follow", NoShort, false),
            Opt("form", 'F', true),
            Opt("form-escape", NoShort, false),
            Opt("form-string", NoShort, true),
            Opt("ftp-account", NoShort, true),
            Opt("ftp-alternative-to-user", NoShort, true),
            Opt("ftp-create-dirs", NoShort, false),
            Opt("ftp-method", NoShort, true),
            Opt("ftp-pasv", NoShort, false),
            Opt("ftp-port", 'P', true),
            Opt("ftp-pret", NoShort, false),
            Opt("ftp-skip-pasv-ip", NoShort, false),
            Opt("ftp-ssl", NoShort, false),
            Opt("ftp-ssl-ccc", NoShort, false),
            Opt("ftp-ssl-ccc-mode", NoShort, true),
            Opt("ftp-ssl-control", NoShort, false),
            Opt("ftp-ssl-reqd", NoShort, false),
            Opt("get", 'G', false),
            Opt("globoff", 'g', false),
            Opt("happy-eyeballs-timeout-ms", NoShort, true),
            Opt("haproxy-clientip", NoShort, true),
            Opt("haproxy-protocol", NoShort, false),
            Opt("head", 'I', false),
            Opt("header", 'H', true),
            Opt("help", 'h', false),
            Opt("hostpubmd5", NoShort, true),
            Opt("hostpubsha256", NoShort, true),
            Opt("hsts", NoShort, true),
            Opt("http0.9", NoShort, false),
            Opt("http1.0", '0', false),
            Opt("http1.1", NoShort, false),
            Opt("http2", NoShort, false),
            Opt("http2-prior-knowledge", NoShort, false),
            Opt("http3", NoShort, false),
            Opt("http3-only", NoShort, false),
            Opt("httpsig-algo", NoShort, true),
            Opt("httpsig-headers", NoShort, true),
            Opt("httpsig-key", NoShort, true),
            Opt("httpsig-keyid", NoShort, true),
            Opt("ignore-content-length", NoShort, false),
            Opt("include", 'i', false),
            Opt("insecure", 'k', false),
            Opt("interface", NoShort, true),
            Opt("ip-tos", NoShort, true),
            Opt("ipfs-gateway", NoShort, true),
            Opt("ipv4", '4', false),
            Opt("ipv6", '6', false),
            Opt("json", NoShort, true),
            Opt("junk-session-cookies", 'j', false),
            Opt("keepalive-cnt", NoShort, true),
            Opt("keepalive-time", NoShort, true),
            Opt("key", NoShort, true),
            Opt("key-type", NoShort, true),
            Opt("knownhosts", NoShort, true),
            Opt("krb", NoShort, true),
            Opt("krb4", NoShort, true),
            Opt("libcurl", NoShort, true),
            Opt("limit-rate", NoShort, true),
            Opt("list-only", 'l', false),
            Opt("local-port", NoShort, true),
            Opt("location", 'L', false),
            Opt("location-trusted", NoShort, false),
            Opt("login-options", NoShort, true),
            Opt("mail-auth", NoShort, true),
            Opt("mail-from", NoShort, true),
            Opt("mail-rcpt", NoShort, true),
            Opt("mail-rcpt-allowfails", NoShort, false),
            Opt("manual", 'M', false),
            Opt("max-filesize", NoShort, true),
            Opt("max-redirs", NoShort, true),
            Opt("max-time", 'm', true),
            Opt("metalink", NoShort, false),
            Opt("mptcp", NoShort, false),
            Opt("negotiate", NoShort, false),
            Opt("netrc", 'n', false),
            Opt("netrc-file", NoShort, true),
            Opt("netrc-optional", NoShort, false),
            Opt("next", ':', false),
            Opt("no-alpn", NoShort, false),
            Opt("no-buffer", 'N', false),
            Opt("no-clobber", NoShort, false),
            Opt("no-keepalive", NoShort, false),
            Opt("no-npn", NoShort, false),
            Opt("no-progress-meter", NoShort, false),
            Opt("no-sessionid", NoShort, false),
            Opt("noproxy", NoShort, true),
            Opt("ntlm", NoShort, false),
            Opt("ntlm-wb", NoShort, false),
            Opt("oauth2-bearer", NoShort, true),
            Opt("out-null", NoShort, false),
            Opt("output", 'o', true),
            Opt("output-dir", NoShort, true),
            Opt("parallel", 'Z', false),
            Opt("parallel-immediate", NoShort, false),
            Opt("parallel-max", NoShort, true),
            Opt("pass", NoShort, true),
            Opt("path-as-is", NoShort, false),
            Opt("pinnedpubkey", NoShort, true),
            Opt("post301", NoShort, false),
            Opt("post302", NoShort, false),
            Opt("post303", NoShort, false),
            Opt("preproxy", NoShort, true),
            Opt("progress-bar", '#', false),
            Opt("proto", NoShort, true),
            Opt("proto-default", NoShort, true),
            Opt("proto-redir", NoShort, true),
            Opt("proxy", 'x', true),
            Opt("proxy-anyauth", NoShort, false),
            Opt("proxy-basic", NoShort, false),
            Opt("proxy-ca-native", NoShort, false),
            Opt("proxy-cacert", NoShort, true),
            Opt("proxy-capath", NoShort, true),
            Opt("proxy-cert", NoShort, true),
            Opt("proxy-cert-type", NoShort, true),
            Opt("proxy-ciphers", NoShort, true),
            Opt("proxy-crlfile", NoShort, true),
            Opt("proxy-digest", NoShort, false),
            Opt("proxy-dump-ca-embed", NoShort, false),
            Opt("proxy-header", NoShort, true),
            Opt("proxy-http2", NoShort, false),
            Opt("proxy-insecure", NoShort, false),
            Opt("proxy-key", NoShort, true),
            Opt("proxy-key-type", NoShort, true),
            Opt("proxy-negotiate", NoShort, false),
            Opt("proxy-ntlm", NoShort, false),
            Opt("proxy-pass", NoShort, true),
            Opt("proxy-pinnedpubkey", NoShort, true),
            Opt("proxy-service-name", NoShort, true),
            Opt("proxy-ssl-allow-beast", NoShort, false),
            Opt("proxy-ssl-auto-client-cert", NoShort, false),
            Opt("proxy-tls13-ciphers", NoShort, true),
            Opt("proxy-tlsauthtype", NoShort, true),
            Opt("proxy-tlspassword", NoShort, true),
            Opt("proxy-tlsuser", NoShort, true),
            Opt("proxy-tlsv1", NoShort, false),
            Opt("proxy-user", 'U', true),
            Opt("proxy1.0", NoShort, true),
            Opt("proxytunnel", 'p', false),
            Opt("pubkey", NoShort, true),
            Opt("quote", 'Q', true),
            Opt("random-file", NoShort, true),
            Opt("range", 'r', true),
            Opt("rate", NoShort, true),
            Opt("raw", NoShort, false),
            Opt("referer", 'e', true),
            Opt("remote-header-name", 'J', false),
            Opt("remote-name", 'O', false),
            Opt("remote-name-all", NoShort, false),
            Opt("remote-time", 'R', false),
            Opt("remove-on-error", NoShort, false),
            Opt("request", 'X', true),
            Opt("request-target", NoShort, true),
            Opt("resolve", NoShort, true),
            Opt("retry", NoShort, true),
            Opt("retry-all-errors", NoShort, false),
            Opt("retry-connrefused", NoShort, false),
            Opt("retry-delay", NoShort, true),
            Opt("retry-max-time", NoShort, true),
            Opt("sasl-authzid", NoShort, true),
            Opt("sasl-ir", NoShort, false),
            Opt("service-name", NoShort, true),
            Opt("show-error", 'S', false),
            Opt("show-headers", NoShort, false),
            Opt("sigalgs", NoShort, true),
            Opt("silent", 's', false),
            Opt("skip-existing", NoShort, false),
            Opt("socks4", NoShort, true),
            Opt("socks4a", NoShort, true),
            Opt("socks5", NoShort, true),
            Opt("socks5-basic", NoShort, false),
            Opt("socks5-gssapi", NoShort, false),
            Opt("socks5-gssapi-nec", NoShort, false),
            Opt("socks5-gssapi-service", NoShort, true),
            Opt("socks5-hostname", NoShort, true),
            Opt("speed-limit", 'Y', true),
            Opt("speed-time", 'y', true),
            Opt("ssl", NoShort, false),
            Opt("ssl-allow-beast", NoShort, false),
            Opt("ssl-auto-client-cert", NoShort, false),
            Opt("ssl-no-revoke", NoShort, false),
            Opt("ssl-reqd", NoShort, false),
            Opt("ssl-revoke-best-effort", NoShort, false),
            Opt("ssl-sessions", NoShort, true),
            Opt("sslv2", '2', false),
            Opt("sslv3", '3', false),
            Opt("stderr", NoShort, true),
            Opt("styled-output", NoShort, false),
            Opt("suppress-connect-headers", NoShort, false),
            Opt("tcp-fastopen", NoShort, false),
            Opt("tcp-nodelay", NoShort, false),
            Opt("telnet-option", 't', true),
            Opt("tftp-blksize", NoShort, true),
            Opt("tftp-no-options", NoShort, false),
            Opt("time-cond", 'z', true),
            Opt("tls-earlydata", NoShort, false),
            Opt("tls-max", NoShort, true),
            Opt("tls13-ciphers", NoShort, true),
            Opt("tlsauthtype", NoShort, true),
            Opt("tlspassword", NoShort, true),
            Opt("tlsuser", NoShort, true),
            Opt("tlsv1", '1', false),
            Opt("tlsv1.0", NoShort, false),
            Opt("tlsv1.1", NoShort, false),
            Opt("tlsv1.2", NoShort, false),
            Opt("tlsv1.3", NoShort, false),
            Opt("tr-encoding", NoShort, false),
            Opt("trace", NoShort, true),
            Opt("trace-ascii", NoShort, true),
            Opt("trace-config", NoShort, true),
            Opt("trace-ids", NoShort, false),
            Opt("trace-time", NoShort, false),
            Opt("unix-socket", NoShort, true),
            Opt("upload-file", 'T', true),
            Opt("upload-flags", NoShort, true),
            Opt("url", NoShort, true),
            Opt("url-query", NoShort, true),
            Opt("use-ascii", 'B', false),
            Opt("user", 'u', true),
            Opt("user-agent", 'A', true),
            Opt("variable", NoShort, true),
            Opt("verbose", 'v', false),
            Opt("version", 'V', false),
            Opt("vlan-priority", NoShort, true),
            Opt("write-out", 'w', true),
            Opt("xattr", NoShort, false),
        };

        private static readonly Dictionary<string, CurlOptionSpec> LongOptionTable = BuildLongOptionTable();

        private static readonly Dictionary<char, CurlOptionSpec> ShortOptionTable = BuildShortOptionTable();

        private static Dictionary<string, CurlOptionSpec> BuildLongOptionTable()
        {
            // curl option names are case-sensitive (--http1.0, -O vs -o), so the lookup is ordinal.
            var table = new Dictionary<string, CurlOptionSpec>(StringComparer.Ordinal);
            foreach (var spec in CurlOptionTable)
            {
                table[spec.LongName] = spec;
            }

            return table;
        }

        private static Dictionary<char, CurlOptionSpec> BuildShortOptionTable()
        {
            var table = new Dictionary<char, CurlOptionSpec>();
            foreach (var spec in CurlOptionTable)
            {
                if (spec.ShortName != NoShort)
                {
                    table[spec.ShortName] = spec;
                }
            }

            return table;
        }

        /// <summary>
        /// Looks up a long option the way curl's <c>getparameter()</c> does: an exact name, a boolean
        /// with a <c>no-</c> prefix (<c>--no-include</c>), the positive form of a <c>no-</c> option
        /// (<c>--clobber</c>, <c>--progress-meter</c>), or any value option behind the <c>expand-</c>
        /// prefix (<c>--expand-data</c>). Names registered only by the transport options are accepted
        /// too, so they consume their argument.
        /// </summary>
        /// <param name="name">The name as typed, without the leading <c>--</c>.</param>
        /// <param name="spec">The matching table row.</param>
        /// <param name="typed">The canonical spelling handed to the option handlers, with <c>--</c>.</param>
        /// <param name="expand">True when the value must go through <c>--variable</c> expansion first.</param>
        /// <returns>True when <paramref name="name"/> is a curl option.</returns>
        private static bool TryResolveLongOption(string name, out CurlOptionSpec spec, out string typed, out bool expand)
        {
            expand = false;
            typed = "--" + name;

            if (LongOptionTable.TryGetValue(name, out spec!))
            {
                return true;
            }

            if (name.StartsWith("no-", StringComparison.Ordinal) &&
                LongOptionTable.TryGetValue(name.Substring(3), out spec!) && !spec.TakesValue)
            {
                return true; // --no-include, --no-basic, --no-http0.9 ...
            }

            if (LongOptionTable.TryGetValue("no-" + name, out spec!) && !spec.TakesValue)
            {
                return true; // --clobber, --buffer, --keepalive, --progress-meter ...
            }

            if (name.StartsWith("expand-", StringComparison.Ordinal) &&
                LongOptionTable.TryGetValue(name.Substring(7), out spec!) && spec.TakesValue)
            {
                expand = true;
                typed = "--" + spec.LongName;
                return true;
            }

            if (TransportOptionsRequiringValue.Contains("--" + name))
            {
                spec = new CurlOptionSpec(name, NoShort, true);
                return true;
            }

            spec = null!;
            return false;
        }

        /// <summary>
        /// True when <paramref name="option"/> (with its leading dashes) names a curl option that takes
        /// an argument. Exposed for the transport partial and for tests.
        /// </summary>
        internal static bool OptionTakesValue(string option)
        {
            if (option.StartsWith("--", StringComparison.Ordinal))
            {
                return TryResolveLongOption(option.Substring(2), out var spec, out _, out _) && spec.TakesValue;
            }

            return option.Length == 2 && option[0] == '-' &&
                   ShortOptionTable.TryGetValue(option[1], out var shortSpec) && shortSpec.TakesValue;
        }

        #endregion

        /// <summary>
        /// Parses a curl command line into a <see cref="CurlOptions"/> describing its first transfer;
        /// further URLs, globs and <c>--next</c> groups are in <see cref="CurlOptions.AdditionalTransfers"/>.
        /// </summary>
        /// <param name="command">The command, with or without the leading <c>curl</c>.</param>
        /// <returns>The parsed options.</returns>
        /// <remarks>The text is read with <see cref="Curl.DefaultFlavor"/> (normally <see cref="CurlFlavor.Auto"/>);
        /// see <see cref="Parse(string, CurlFlavor)"/>.</remarks>
        public CurlOptions Parse(string command)
        {
            return Parse(command, Curl.DefaultFlavor);
        }

        /// <summary>
        /// The permissive reader <see cref="CurlFlavor.Auto"/> uses: accepts bash, cmd.exe and PowerShell quoting,
        /// escapes, continuations and variables at the same time (see <see cref="CurlFlavor.Auto"/>).
        /// </summary>
        private CurlOptions ParsePermissive(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command), CurlException.Compose(
                    $"The curl command is {(command == null ? "null" : "empty or only whitespace")}: there is nothing to run (null, \"\" and blanks are not commands).",
                    "Pass a command such as \"curl https://example.com\" (the leading word curl is optional), or check the variable that should hold it.",
                    DocLinks.HowToRunCommand));

            var originalCommand = command;

            // Normalize command - handle line continuations for different shells
            command = NormalizeLineContinuations(command);

            // Remove "curl", "curl.exe" or "/usr/bin/curl" from the beginning if present
            command = StripCurlPrefix(command.Trim());

            // Expand environment variables (cross-platform support), never inside single quotes
            command = ExpandEnvironmentVariables(command);

            var args = ParseArguments(command);
            return ParseArgumentList(args, originalCommand);
        }

        /// <summary>
        /// Removes the program name from the start of a pasted command: <c>curl</c>, <c>curl.exe</c>,
        /// or a path to either (<c>/usr/bin/curl</c>, <c>"C:\Program Files\curl\curl.exe"</c>).
        /// </summary>
        /// <exception cref="CurlException">Exit code 3 when the command is only the program name.</exception>
        private static string StripCurlPrefix(string command)
        {
            string first;
            int end;
            if (command.Length > 0 && (command[0] == '"' || command[0] == '\''))
            {
                var close = command.IndexOf(command[0], 1);
                if (close < 0)
                {
                    return command;
                }

                first = command.Substring(1, close - 1);
                end = close + 1;
            }
            else
            {
                end = 0;
                while (end < command.Length && !char.IsWhiteSpace(command[end]))
                {
                    end++;
                }

                first = command.Substring(0, end);
            }

            var slash = Math.Max(first.LastIndexOf('/'), first.LastIndexOf('\\'));
            var program = slash >= 0 ? first.Substring(slash + 1) : first;
            if (!program.Equals("curl", StringComparison.OrdinalIgnoreCase) &&
                !program.Equals("curl.exe", StringComparison.OrdinalIgnoreCase))
            {
                return command;
            }

            // A bare word "curl" that is followed by nothing is a command without a URL
            if (end < command.Length && !char.IsWhiteSpace(command[end]))
            {
                return command;
            }

            var rest = command.Substring(end).TrimStart();
            if (rest.Length == 0)
            {
                throw new CurlException(
                    CurlException.Compose(
                        $"No URL specified: the command \"{first}\" is only the program name.",
                        "Add the URL to fetch after it, for example \"curl https://example.com\".",
                        DocLinks.Url),
                    2);
            }

            return rest;
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

            // Normalize whitespace sequences to single space - but only OUTSIDE quotes.
            // Quoted arguments (file paths, -d payloads) may legitimately contain
            // consecutive spaces, tabs or newlines that must be preserved verbatim.
            command = CollapseUnquotedWhitespace(command);

            return command.Trim();
        }

        /// <summary>
        /// Collapses runs of whitespace to a single space outside quoted regions while
        /// leaving the inside of single- and double-quoted strings untouched, so paths
        /// like <c>"C:\Temp\file with  double  spaces.txt"</c> and JSON payloads keep
        /// their exact spacing.
        /// </summary>
        private static string CollapseUnquotedWhitespace(string command)
        {
            var sb = new StringBuilder(command.Length);
            var quote = '\0';
            var lastWasSpace = false;

            for (var i = 0; i < command.Length; i++)
            {
                var c = command[i];

                if (quote != '\0')
                {
                    sb.Append(c);
                    if (c == '\\' && quote == '"' && i + 1 < command.Length)
                    {
                        sb.Append(command[++i]); // escaped char inside double quotes
                    }
                    else if (c == quote)
                    {
                        quote = '\0';
                    }
                    lastWasSpace = false;
                    continue;
                }

                if (c == '"' || c == '\'')
                {
                    quote = c;
                    sb.Append(c);
                    lastWasSpace = false;
                }
                else if (char.IsWhiteSpace(c))
                {
                    if (!lastWasSpace)
                    {
                        sb.Append(' ');
                        lastWasSpace = true;
                    }
                }
                else if (c == '\\' && i + 1 < command.Length && command[i + 1] == ' ')
                {
                    // Backslash-escaped space outside quotes is a literal space
                    sb.Append(c);
                    sb.Append(command[++i]);
                    lastWasSpace = false;
                }
                else
                {
                    sb.Append(c);
                    lastWasSpace = false;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Expands environment variables across different shell formats, the way the shell that the
        /// command was written for would:
        /// - Bash/ZSH: <c>$VAR</c> and <c>${VAR}</c>, outside single quotes only (<c>'$HOME'</c> stays literal)
        /// - PowerShell: <c>$env:VAR</c>, outside single quotes only
        /// - Windows CMD: <c>%VAR%</c>, only when the variable exists and never inside single quotes,
        ///   so percent-encoded URLs such as <c>%5Cuser%22</c> are left alone
        /// A variable that is not set is left as typed. <c>\$</c> keeps a literal dollar sign.
        /// </summary>
        private static string ExpandEnvironmentVariables(string command)
        {
            var sb = new StringBuilder(command.Length);
            var quote = '\0';

            for (var i = 0; i < command.Length; i++)
            {
                var c = command[i];
                var next = i + 1 < command.Length ? command[i + 1] : '\0';

                if (quote == '\'')
                {
                    sb.Append(c);
                    if (c == '\'')
                    {
                        quote = '\0';
                    }

                    continue;
                }

                if (c == '\\' && next != '\0')
                {
                    // Keep escapes for the tokenizer; they must not start or end a quote or a variable here
                    sb.Append(c).Append(next);
                    i++;
                    continue;
                }

                if (c == '"')
                {
                    quote = quote == '"' ? '\0' : '"';
                    sb.Append(c);
                    continue;
                }

                if (c == '\'' && quote == '\0')
                {
                    quote = '\'';
                    sb.Append(c);
                    continue;
                }

                if (c == '$' && TryExpandDollar(command, i, out var value, out var consumed))
                {
                    sb.Append(value);
                    i += consumed - 1;
                    continue;
                }

                if (c == '%')
                {
                    var close = command.IndexOf('%', i + 1);
                    if (close > i + 1)
                    {
                        var name = command.Substring(i + 1, close - i - 1);
                        if (name.IndexOfAny(new[] { ' ', '\t', '"', '\'', '=' }) < 0)
                        {
                            var envValue = Environment.GetEnvironmentVariable(name);
                            if (envValue != null)
                            {
                                sb.Append(envValue);
                                i = close;
                                continue;
                            }
                        }
                    }
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Expands <c>$env:NAME</c>, <c>${NAME}</c> or <c>$NAME</c> starting at <paramref name="index"/>
        /// when the variable is set.
        /// </summary>
        private static bool TryExpandDollar(string command, int index, out string value, out int consumed)
        {
            value = string.Empty;
            consumed = 0;

            var match = Regex.Match(command.Substring(index), @"^\$(?:env:(?<name>[A-Za-z_][A-Za-z0-9_]*)|\{(?<name>[A-Za-z_][A-Za-z0-9_]*)\}|(?<name>[A-Za-z_][A-Za-z0-9_]*))");
            if (!match.Success)
            {
                return false;
            }

            var envValue = Environment.GetEnvironmentVariable(match.Groups["name"].Value);
            if (envValue == null)
            {
                return false;
            }

            value = envValue;
            consumed = match.Length;
            return true;
        }

        /// <summary>
        /// Adds the scheme curl would guess for a URL given without one: <c>ftp.example.com</c>
        /// becomes <c>ftp://ftp.example.com</c>, anything else becomes <c>http://...</c>.
        /// URLs that already carry a scheme are returned unchanged, except that curl's tolerance for
        /// a single slash (<c>http:/example.com/</c>) is applied.
        /// </summary>
        /// <param name="url">The URL as typed on the command line.</param>
        /// <returns>A URL with a scheme.</returns>
        public static string NormalizeUrl(string url)
        {
            return NormalizeUrl(url, null);
        }

        /// <summary>
        /// Like <see cref="NormalizeUrl(string)"/>, with the scheme <c>--proto-default</c> named for URLs
        /// that have none (no <c>ftp.</c> guessing then, as in curl).
        /// </summary>
        internal static string NormalizeUrl(string url, string? defaultScheme)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }

            var trimmed = url.Trim();
            if (Regex.IsMatch(trimmed, @"^[A-Za-z][A-Za-z0-9+.-]*://"))
            {
                return trimmed;
            }

            // curl accepts "http:/host/path" (one slash) for its own schemes
            var oneSlash = Regex.Match(trimmed, @"^(https?|ftps?):/(?!/)", RegexOptions.IgnoreCase);
            if (oneSlash.Success)
            {
                return oneSlash.Groups[1].Value + "://" + trimmed.Substring(oneSlash.Length);
            }

            if (!string.IsNullOrEmpty(defaultScheme))
            {
                return defaultScheme + "://" + trimmed;
            }

            if (trimmed.StartsWith("ftp.", StringComparison.OrdinalIgnoreCase))
            {
                return "ftp://" + trimmed;
            }

            return "http://" + trimmed;
        }

        /// <summary>
        /// Wraps a value in single quotes so it survives a round-trip through <see cref="Parse(string)"/>
        /// (and through a real POSIX shell) unchanged, even when it contains spaces, double quotes,
        /// backslashes or single quotes. Embedded single quotes use the standard <c>'\''</c> idiom.
        /// </summary>
        /// <param name="value">Raw value such as a JSON body, header or URL.</param>
        /// <returns>The single-quoted, shell-safe representation; <c>''</c> for null or empty input.</returns>
        /// <example>
        /// <code language="csharp">
        /// var body = "{\"name\":\"O'Brien\"}";
        /// var cmd = $"curl -d {CommandParser.ShellQuote(body)} https://api.example.com/users";
        /// // curl -d '{"name":"O'\''Brien"}' https://api.example.com/users
        /// </code>
        /// </example>
        public static string ShellQuote(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "''";
            }

            return "'" + value!.Replace("'", "'\\''") + "'";
        }

        /// <summary>
        /// Like <see cref="ShellQuote(string)"/> but leaves values that contain only
        /// shell-safe characters (letters, digits and <c>- _ . / : = @ % ? &amp; + ~ , # ;</c>)
        /// unquoted, which keeps generated commands readable for URLs and file names.
        /// </summary>
        /// <param name="value">Raw value such as a URL or output path.</param>
        /// <returns>The value, quoted only when it needs to be.</returns>
        public static string ShellQuoteIfNeeded(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "''";
            }

            foreach (var c in value!)
            {
                if (char.IsLetterOrDigit(c) || "-_./:=@%?&+~,#;".IndexOf(c) >= 0)
                {
                    continue;
                }

                return ShellQuote(value);
            }

            return value;
        }

        /// <summary>
        /// Rewrites a just-closed single-quoted segment that was written for Windows.
        /// </summary>
        /// <remarks>
        /// On Windows the C runtime that curl.exe uses to split its command line treats
        /// <c>\"</c> as a literal double quote even when PowerShell passed the text through
        /// single quotes, so Windows documentation and Stack Overflow answers are full of
        /// <c>-d '{\"key\":\"value\"}'</c>. A POSIX shell would send the backslashes verbatim,
        /// which is never what the author meant. If a single-quoted segment contains
        /// <c>\"</c> and no bare <c>"</c> at all, every <c>\"</c> is unescaped. A segment with
        /// any bare double quote is treated as genuine JSON (where <c>\"</c> is a real escape
        /// inside a string literal) and left untouched.
        /// </remarks>
        private static void UnescapeWindowsStyleQuotes(StringBuilder current, int start)
        {
            if (start < 0 || start >= current.Length)
            {
                return;
            }

            var segment = current.ToString(start, current.Length - start);
            if (segment.IndexOf("\\\"", StringComparison.Ordinal) < 0)
            {
                return;
            }

            for (var k = 0; k < segment.Length; k++)
            {
                if (segment[k] == '"' && (k == 0 || segment[k - 1] != '\\'))
                {
                    return;
                }
            }

            current.Length = start;
            current.Append(segment.Replace("\\\"", "\""));
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
            var quoteStart = 0;
            var sawQuotes = false; // '' or "" must yield an empty argument, not vanish
            var escape = false;
            var i = 0;

            while (i < command.Length)
            {
                var c = command[i];
                var next = i + 1 < command.Length ? command[i + 1] : '\0';

                if (escape)
                {
                    // `escape` is only set when the backslash precedes a quote or another backslash.
                    // Like a POSIX shell (and unlike C), \n or \t are never turned into control
                    // characters: curl -d "{\"a\":\"x\ny\"}" must stay valid JSON on the wire.
                    if (inQuote && quoteChar == '"')
                    {
                        // Inside double quotes a backslash only escapes " and \ ; \' stays literal.
                        if (c == '\'') current.Append('\\');
                        current.Append(c);
                    }
                    else if (inQuote && quoteChar == '\'')
                    {
                        // Inside single quotes everything is literal except the \' convenience.
                        if (c != '\'') current.Append('\\');
                        current.Append(c);
                    }
                    else
                    {
                        // Outside quotes \" \' and \\ yield the bare character.
                        current.Append(c);
                    }

                    escape = false;
                    i++;
                    continue;
                }

                // Handle escaping (works for all shells)
                if (c == '\\' && next != '\0')
                {
                    // \" \' \\ and \$ are escapes wherever a shell honours them; outside quotes a
                    // backslash also escapes whitespace, so https://x/a\ b is one argument with a space.
                    if (next == '"' || next == '\'' || next == '\\' || next == '$' ||
                        (!inQuote && (next == ' ' || next == '\t')))
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
                    quoteStart = current.Length;
                    sawQuotes = true;
                    i++;
                    continue;
                }

                if (inQuote && c == quoteChar)
                {
                    if (quoteChar == '\'')
                    {
                        UnescapeWindowsStyleQuotes(current, quoteStart);
                    }

                    inQuote = false;
                    quoteChar = ' ';
                    i++;
                    continue;
                }

                // Whitespace handling
                if (!inQuote && char.IsWhiteSpace(c))
                {
                    if (current.Length > 0 || sawQuotes)
                    {
                        args.Add(current.ToString());
                        current.Clear();
                        sawQuotes = false;
                    }
                    i++;
                    continue;
                }

                current.Append(c);
                i++;
            }

            if (current.Length > 0 || sawQuotes)
            {
                args.Add(current.ToString());
            }

            return args;
        }

        /// <summary>
        /// Walks the argument vector exactly like curl's <c>parse_args()</c>: long options with an
        /// optional <c>=value</c>, clusters of single-letter options (<c>-sSL</c>) where the first one
        /// that takes a value swallows the rest of the word (<c>-ofile.txt</c>, <c>-m10</c>) or the next
        /// argument (even when it starts with <c>-</c>, as in <c>-d -5</c>), <c>--</c> to end options, and
        /// everything else as a URL.
        /// </summary>
        private void ProcessArguments(IList<string> args, ParseContext context)
        {
            var stillFlags = true;

            for (var i = 0; i < args.Count; i++)
            {
                var arg = args[i];

                if (stillFlags && arg == "--")
                {
                    stillFlags = false;
                    continue;
                }

                if (!stillFlags || arg.Length < 2 || arg[0] != '-')
                {
                    AddUrl(context, arg);
                    continue;
                }

                if (arg[1] == '-')
                {
                    var name = arg.Substring(2);
                    string? inlineValue = null;
                    if (!TryResolveLongOption(name, out var spec, out var typed, out var expand))
                    {
                        var eq = name.IndexOf('=');
                        if (eq <= 0 || !TryResolveLongOption(name.Substring(0, eq), out spec, out typed, out expand))
                        {
                            // Unknown option: remember it for Validate(), do not consume URL arguments
                            context.Current.Options.UnknownOptions.Add(eq > 0 ? "--" + name.Substring(0, eq) : arg);
                            continue;
                        }

                        inlineValue = name.Substring(eq + 1);
                    }

                    var value = string.Empty;
                    if (spec.TakesValue)
                    {
                        if (inlineValue != null)
                        {
                            value = inlineValue;
                        }
                        else if (i + 1 < args.Count)
                        {
                            value = args[++i];
                        }
                        else
                        {
                            throw MissingOptionValue(typed, context.OriginalCommand);
                        }
                    }

                    Dispatch(context, spec, typed, expand, value);
                    continue;
                }

                // A cluster of single-letter options: -sSL, -ofile.txt, -XPOST, -H"X: y"
                for (var j = 1; j < arg.Length; j++)
                {
                    var letter = arg[j];
                    if (!ShortOptionTable.TryGetValue(letter, out var spec))
                    {
                        context.Current.Options.UnknownOptions.Add("-" + letter);
                        break;
                    }

                    var typed = "--" + spec.LongName;
                    if (!spec.TakesValue)
                    {
                        Dispatch(context, spec, typed, false, string.Empty);
                        continue;
                    }

                    string value;
                    if (j + 1 < arg.Length)
                    {
                        value = arg.Substring(j + 1);
                    }
                    else if (i + 1 < args.Count)
                    {
                        value = args[++i];
                    }
                    else
                    {
                        throw MissingOptionValue("-" + letter, context.OriginalCommand);
                    }

                    Dispatch(context, spec, typed, false, value);
                    break;
                }
            }
        }

        /// <summary>The exception curl's "option -X: requires parameter" (exit code 2) maps to.</summary>
        private static CurlException MissingOptionValue(string option, string? command)
        {
            return new CurlException(
                CurlException.Compose(
                    $"option {option}: requires parameter (it is the last word of the command, so nothing follows it).",
                    $"Put its value right after it, for example \"{option} {ExampleValue(option)}\", and quote the value if it contains spaces.",
                    DocLinks.CliCompatibility),
                2,
                command);
        }

        /// <summary>A typical value for an option, used in "requires parameter" messages.</summary>
        private static string ExampleValue(string option)
        {
            switch (option)
            {
                case "-H": case "--header": return "'Accept: application/json'";
                case "-d": case "--data": case "--data-raw": case "--data-binary": case "--data-urlencode": return "'name=value'";
                case "--json": return "'{\"name\":\"value\"}'";
                case "-o": case "--output": return "out.json";
                case "-u": case "--user": return "user:password";
                case "-X": case "--request": return "POST";
                case "-A": case "--user-agent": return "'MyApp/1.0'";
                case "-e": case "--referer": return "https://example.com/";
                case "-b": case "--cookie": return "'name=value'";
                case "-c": case "--cookie-jar": return "cookies.txt";
                case "-x": case "--proxy": return "http://proxy.example.com:8080";
                case "-m": case "--max-time": case "--connect-timeout": return "30";
                case "-F": case "--form": return "'file=@photo.jpg'";
                case "-T": case "--upload-file": return "file.bin";
                case "-w": case "--write-out": return "'%{http_code}'";
                case "-K": case "--config": return "settings.curlrc";
                default: return "VALUE";
            }
        }

        /// <summary>
        /// Routes one recognised option: structural ones (<c>--next</c>, <c>-K</c>, <c>--url</c>,
        /// <c>--variable</c>, per-URL <c>-o</c>/<c>-O</c>/<c>-T</c>) are handled here, everything else is
        /// recorded for the current <c>--next</c> group and applied to its options.
        /// </summary>
        private void Dispatch(ParseContext context, CurlOptionSpec spec, string typed, bool expand, string value)
        {
            var group = context.Current;

            if (expand)
            {
                value = ExpandVariables(value, context);
            }

            switch (typed)
            {
                case "--next":
                    context.StartNewGroup();
                    return;

                case "--config":
                    IncludeConfigFile(value, context);
                    return;

                case "--url":
                    AddUrlArgument(context, value);
                    return;

                case "--variable":
                    DefineVariable(value, context);
                    return;

                case "--upload-file":
                    group.UploadSlots.Add(value);
                    return;

                case "--output":
                    group.OutputSlots.Add(value);
                    break;

                case "--remote-name":
                    group.OutputSlots.Add(null);
                    break;

                case "--out-null":
                    group.OutputSlots.Add(OutNullSlot);
                    break;

                case "--no-remote-name":
                    // curl: takes the next URL's output slot and sends it to stdout, even with --remote-name-all
                    group.OutputSlots.Add("-");
                    break;

                case "--remote-name-all":
                    group.RemoteNameAll = true;
                    break;

                case "--no-remote-name-all":
                    group.RemoteNameAll = false;
                    break;
            }

            group.Recorded.Add(new KeyValuePair<string, string>(typed, value));
            ApplyOption(group.Options, typed, value, ref group.MethodSpecified, context);
        }

        /// <summary>
        /// Applies one option to <paramref name="options"/>: the transport handlers get the first chance,
        /// then the request-shaping switch; a real curl option nobody implements is recorded in
        /// <see cref="CurlOptions.UnsupportedOptions"/>.
        /// </summary>
        private void ApplyOption(CurlOptions options, string typed, string value, ref bool methodSpecified, ParseContext context)
        {
            RecordProxyRequestShaping(options, typed, value);
            if (typed == "--form" || typed == "--form-string")
            {
                // Multipart options select a multipart POST whichever handler builds the form
                SetRequestKind(options, RequestKind.MimePost, $"multipart POST ({(typed == "--form" ? "-F, --form" : "--form-string")})");
            }

            if (!ProcessOption(typed, value, options, ref methodSpecified))
            {
                options.UnsupportedOptions.Add(typed);
            }
        }

        /// <summary>
        /// Remembers the proxy options that change the request itself (curl adds
        /// <c>Proxy-Connection: Keep-Alive</c>, preemptive <c>Proxy-Authorization: Basic</c> and
        /// <c>--proxy-header</c> lines to requests sent through a plain HTTP proxy), whoever else handles them.
        /// </summary>
        private static void RecordProxyRequestShaping(CurlOptions options, string typed, string value)
        {
            switch (typed)
            {
                case "--proxytunnel":
                    options.ProxyTunnelRequested = true;
                    break;
                case "--no-proxytunnel":
                    options.ProxyTunnelRequested = false;
                    break;
                case "--noproxy":
                    options.NoProxyHosts = value;
                    break;
                case "--proxy-header":
                    options.ProxyHeaderLines.Add(value);
                    break;
                case "--proxy-basic":
                    options.ProxyAuthMethod = "basic";
                    break;
                case "--proxy-digest":
                    options.ProxyAuthMethod = "digest";
                    break;
                case "--proxy-ntlm":
                    options.ProxyAuthMethod = "ntlm";
                    break;
                case "--proxy-negotiate":
                    options.ProxyAuthMethod = "negotiate";
                    break;
                case "--proxy-anyauth":
                    options.ProxyAuthMethod = "anyauth";
                    break;
                case "--proxy1.0":
                    options.ProxyHttp10 = true;
                    break;
                case "--proxy":
                    options.ProxyHttp10 = false;
                    break;
            }
        }

        private bool ProcessOption(string option, string value, CurlOptions options, ref bool methodSpecified)
        {
            // Transport-side options (proxy, cookies, output files, write-out...) live in CommandParser.Transport.cs.
            // They get the first chance, so that file can refine any of the fallbacks below.
            if (ProcessTransportOption(option, value, options, ref methodSpecified))
            {
                return true;
            }

            switch (option)
            {
                case "--request":
                    // curl sends the method exactly as typed (-X igloo stays lower case on the wire)
                    options.Method = value.ToUpperInvariant();
                    options.CustomMethod = value;
                    methodSpecified = true;
                    return true;

                case "--header":
                    ParseHeader(value, options);
                    return true;

                case "--data":
                case "--data-ascii":
                    AddDataPart(options, value, RequestDataFlavor.Data);
                    SetPostMethod(options, ref methodSpecified, RequestKind.Post, "POST (-d, --data)");
                    return true;

                case "--data-raw":
                    options.DataRaw = true;
                    AddDataPart(options, value, RequestDataFlavor.Raw);
                    SetPostMethod(options, ref methodSpecified, RequestKind.Post, "POST (--data-raw)");
                    return true;

                case "--data-binary":
                    AddDataPart(options, value, RequestDataFlavor.Binary);
                    SetPostMethod(options, ref methodSpecified, RequestKind.Post, "POST (--data-binary)");
                    return true;

                case "--data-urlencode":
                    options.DataUrlEncode = true;
                    // Encode now, exactly like curl's tool_getparam.c does, so Data is wire-ready
                    AddDataPart(options, UrlEncodeDataArgument(value), RequestDataFlavor.Raw);
                    SetPostMethod(options, ref methodSpecified, RequestKind.Post, "POST (--data-urlencode)");
                    return true;

                case "--json":
                    AddDataPart(options, value, RequestDataFlavor.Json);
                    // A later -H replaces these instead of being joined to them
                    options.Headers["Content-Type"] = "application/json";
                    options.Headers["Accept"] = "application/json";
                    options.ImplicitHeaders.Add("Content-Type");
                    options.ImplicitHeaders.Add("Accept");

                    SetPostMethod(options, ref methodSpecified, RequestKind.Post, "POST (--json)");
                    return true;

                case "--form":
                    ParseFormField(value, options);
                    SetPostMethod(options, ref methodSpecified, RequestKind.MimePost, "multipart POST (-F, --form)");
                    return true;

                case "--get":
                    options.UseGet = true;
                    return true;

                case "--no-get":
                    options.UseGet = false;
                    return true;

                case "--output":
                    options.OutputFile = value;
                    return true;

                case "--remote-name":
                    options.UseRemoteFileName = true;
                    options.OutputFile = null;
                    return true;

                case "--out-null":
                    options.DiscardOutput = true;
                    options.UseRemoteFileName = false;
                    options.OutputFile = null;
                    return true;

                case "--include":
                case "--show-headers":
                    options.IncludeHeaders = true;
                    return true;

                case "--no-include":
                case "--no-show-headers":
                    options.IncludeHeaders = false;
                    return true;

                case "--head":
                    SetRequestKind(options, RequestKind.Head, "HEAD (-I, --head)");
                    options.HeadOnly = true;
                    options.Method = "HEAD";
                    methodSpecified = true;
                    return true;

                case "--location":
                    options.FollowLocation = true;
                    return true;

                case "--no-location":
                    options.FollowLocation = false;
                    return true;

                case "--insecure":
                    options.Insecure = true;
                    return true;

                case "--no-insecure":
                    options.Insecure = false;
                    return true;

                case "--basic":
                    options.AuthScheme = "basic";
                    return true;

                case "--no-basic":
                    if (string.Equals(options.AuthScheme, "basic", StringComparison.Ordinal))
                    {
                        options.AuthScheme = null;
                    }

                    return true;

                case "--digest":
                    options.AuthScheme = "digest";
                    return true;

                case "--ntlm":
                    options.AuthScheme = "ntlm";
                    return true;

                case "--negotiate":
                    options.AuthScheme = "negotiate";
                    return true;

                case "--anyauth":
                    options.AuthScheme = "anyauth";
                    return true;

                case "--verbose":
                    options.Verbose = true;
                    return true;

                case "--no-verbose":
                    options.Verbose = false;
                    return true;

                case "--trace":
                    options.TraceFile = value;
                    return true;

                case "--trace-ascii":
                    options.TraceAsciiFile = value;
                    return true;

                case "--trace-time":
                    options.TraceTime = true;
                    return true;

                case "--silent":
                    options.Silent = true;
                    return true;

                case "--no-silent":
                    options.Silent = false;
                    return true;

                case "--show-error":
                    options.ShowError = true;
                    return true;

                case "--fail":
                    options.FailOnError = true;
                    return true;

                case "--no-fail":
                    options.FailOnError = false;
                    return true;

                case "--user-agent":
                    // -A "" means "send no User-Agent at all", like curl
                    options.UserAgent = value;
                    return true;

                case "--referer":
                    ParseReferer(value, options);
                    return true;

                case "--cookie":
                    AddCookieArgument(options, value);
                    return true;

                case "--cookie-jar":
                    options.CookieJar = value;
                    return true;

                case "--user":
                    ParseCredentials(value, options);
                    return true;

                case "--oauth2-bearer":
                    options.OAuth2Bearer = value;
                    return true;

                case "--proxy":
                case "--proxy1.0":
                    options.Proxy = value;
                    return true;

                case "--proxy-user":
                    ParseProxyCredentials(value, options);
                    return true;

                case "--max-time":
                    // curl accepts decimal seconds here (e.g. --max-time 2.5)
                    options.MaxTime = ParseSecondsOption(value, "-m/--max-time", options.OriginalCommand);
                    return true;

                case "--connect-timeout":
                    options.ConnectTimeout = ParseSecondsOption(value, "--connect-timeout", options.OriginalCommand);
                    return true;

                case "--max-redirs":
                    // -1 means "no limit", like curl
                    options.MaxRedirects = (int)ParseWholeNumberOption(value, "--max-redirs", -1, options.OriginalCommand,
                        "a whole number of redirects, or -1 for no limit (for example --max-redirs 5)");
                    return true;

                case "--max-filesize":
                    options.MaxFileSize = ParseMaxFileSize(value, options.OriginalCommand);
                    return true;

                case "--compressed":
                    options.Compressed = true;
                    return true;

                case "--no-compressed":
                    options.Compressed = false;
                    return true;

                case "--write-out":
                    options.WriteOut = value;
                    return true;

                case "--range":
                    options.Range = NormalizeRange(value);
                    return true;

                case "--time-cond":
                    ParseTimeCondition(value, options);
                    return true;

                case "--continue-at":
                    if (value == "-")
                    {
                        options.ResumeFrom = -1; // Auto-resume
                    }
                    else if (long.TryParse(value, out var resumeFrom))
                    {
                        options.ResumeFrom = resumeFrom;
                        options.Range = $"{resumeFrom}-"; // Also set Range for compatibility
                    }
                    return true;

                case "--cert":
                    options.CertFile = value;
                    return true;

                case "--key":
                    options.KeyFile = value;
                    return true;

                case "--cacert":
                    options.CaCertFile = value;
                    return true;

                case "--interface":
                    options.Interface = value;
                    return true;

                case "--http1.0":
                    options.HttpVersion = "1.0";
                    options.HttpVersionExact = false;
                    return true;

                case "--http1.1":
                    options.HttpVersion = "1.1";
                    options.HttpVersionExact = false;
                    return true;

                case "--http2":
                    options.HttpVersion = "2.0";
                    options.HttpVersionExact = false;
                    return true;

                case "--http2-prior-knowledge":
                    options.HttpVersion = "2.0";
                    options.HttpVersionExact = true;
                    return true;

                case "--http3":
                    options.HttpVersion = "3.0";
                    options.HttpVersionExact = false;
                    return true;

                case "--http3-only":
                    options.HttpVersion = "3.0";
                    options.HttpVersionExact = true;
                    return true;

                case "--globoff":
                    options.GlobOff = true;
                    return true;

                case "--no-globoff":
                    options.GlobOff = false;
                    return true;

                case "--path-as-is":
                    options.PathAsIs = true;
                    return true;

                case "--no-path-as-is":
                    options.PathAsIs = false;
                    return true;

                case "--url-query":
                    options.UrlQueryParts.Add(value);
                    return true;

                case "--proto-default":
                    options.DefaultProtocol = value.Trim();
                    return true;

                case "--etag-compare":
                    // curl reads the first line of the file now; a missing or empty file compares against ""
                    var etag = File.Exists(value)
                        ? ReadAllLinesForOption(value, "--etag-compare", options.OriginalCommand).FirstOrDefault()?.Trim()
                        : null;
                    options.Headers["If-None-Match"] = string.IsNullOrEmpty(etag) ? "\"\"" : etag!;
                    options.ImplicitHeaders.Add("If-None-Match");
                    return true;

                case "--limit-rate":
                    options.LimitRate = ParseRateOption(value, "--limit-rate", options.OriginalCommand);
                    return true;

                case "--speed-limit":
                    options.SpeedLimit = ParseWholeNumberOption(value, "-Y/--speed-limit", 0, options.OriginalCommand,
                        "a whole number of bytes per second (for example --speed-limit 1000)");
                    return true;

                case "--speed-time":
                    options.SpeedTime = (int)ParseWholeNumberOption(value, "-y/--speed-time", 0, options.OriginalCommand,
                        "a whole number of seconds (for example --speed-time 30)");
                    return true;

                case "--progress-bar":
                    options.ProgressBar = true;
                    return true;

                case "--keepalive-time":
                    if (int.TryParse(value, out var keepAliveTime))
                        options.KeepAliveTime = keepAliveTime;
                    return true;

                case "--dns-servers":
                    options.DnsServers = value;
                    return true;

                case "--resolve":
                    ParseResolve(value, options);
                    return true;

                case "--connect-to":
                    ParseConnectTo(value, options);
                    return true;

                case "--quote":
                    options.Quote.Add(value);
                    return true;

                case "--create-dirs":
                    options.CreateDirs = true;
                    return true;

                case "--ftp-pasv":
                    options.FtpPassive = true;
                    return true;

                case "--ftp-ssl":
                    options.FtpSsl = true;
                    return true;

                case "--disable-epsv":
                    options.DisableEpsv = true;
                    return true;

                case "--disable-eprt":
                    options.DisableEprt = true;
                    return true;

                case "--socks5":
                    options.Socks5Proxy = value;
                    return true;

                case "--socks4":
                case "--socks4a":
                case "--socks5-hostname":
                    // the SOCKS flavour travels as the proxy URL scheme (socks4://, socks4a://, socks5h://)
                    var socksScheme = option == "--socks4" ? "socks4://" : option == "--socks4a" ? "socks4a://" : "socks5h://";
                    options.Socks5Proxy = value.Contains("://") ? value : socksScheme + value;
                    return true;

                case "--retry":
                    options.Retry = (int)ParseWholeNumberOption(value, "--retry", 0, options.OriginalCommand,
                        "a whole number of retries (for example --retry 3)");
                    return true;

                case "--retry-delay":
                    options.RetryDelay = ParseSecondsOption(value, "--retry-delay", options.OriginalCommand);
                    return true;

                case "--retry-all-errors":
                    options.RetryAllErrors = true;
                    return true;

                case "--retry-connrefused":
                    options.RetryConnRefused = true;
                    return true;

                case "--retry-max-time":
                    // whole seconds are stored; a fraction rounds up so the limit is never shorter than asked
                    options.RetryMaxTime = (int)Math.Min(int.MaxValue, Math.Ceiling(ParseSecondsOption(value, "--retry-max-time", options.OriginalCommand)));
                    return true;

                case "--location-trusted":
                    options.LocationTrusted = true;
                    options.FollowLocation = true;
                    return true;

                case "--aws-sigv4":
                    options.AwsSigV4 = AwsSigV4Config.Parse(value, options.OriginalCommand);
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Records which kind of HTTP request an option asks for. curl checks the combination once every
        /// option is known (so <c>-I -d x -G</c> is a valid HEAD with a query); see <c>ValidateRequestKinds</c>.
        /// </summary>
        private static void SetRequestKind(CurlOptions options, RequestKind kind, string description)
        {
            options.RequestKinds.Add(new KeyValuePair<RequestKind, string>(kind, description));
        }

        /// <summary>
        /// Fails like curl when the options ask for two different kinds of request
        /// ("You can only select one HTTP request method!", exit code 2). With <c>-G</c> the data options
        /// build a query string instead of a POST body, so they do not count.
        /// </summary>
        private static void ValidateRequestKinds(CurlOptions options)
        {
            KeyValuePair<RequestKind, string>? first = null;
            if (options.UseGet)
            {
                first = new KeyValuePair<RequestKind, string>(RequestKind.None, "GET (-G, --get)");
            }

            foreach (var kind in options.RequestKinds)
            {
                if (options.UseGet && kind.Key == RequestKind.Post)
                {
                    continue;
                }

                if (first == null || (first.Value.Key == RequestKind.None && kind.Key == RequestKind.Head))
                {
                    first = kind;
                    continue;
                }

                if (first.Value.Key == kind.Key)
                {
                    continue;
                }

                throw new CurlException(
                    CurlException.Compose(
                        $"You can only select one HTTP request method! You asked for both {first.Value.Value} and {kind.Value}.",
                        "Remove one of the two options: -d/--data* and --json send a POST body, -F a multipart form, -T uploads a file with PUT, -I sends HEAD " +
                        "and -G turns the data into a query string; use -X to change only the method name (for example -X PATCH -d '...').",
                        DocLinks.HowToPutDeletePatch),
                    2,
                    options.OriginalCommand);
            }
        }

        /// <summary>
        /// Selects POST for a data or form option unless <c>-X</c> already named a method; -X GET -d x
        /// therefore sends a GET with a body, exactly like curl.
        /// </summary>
        private static void SetPostMethod(CurlOptions options, ref bool methodSpecified, RequestKind kind, string description)
        {
            SetRequestKind(options, kind, description);
            if (string.IsNullOrEmpty(options.CustomMethod) && (string.IsNullOrEmpty(options.Method) || options.Method == "GET"))
            {
                options.Method = "POST";
                methodSpecified = true; // data implies POST
            }
        }

        /// <summary>
        /// Applies one <c>-H</c> argument the way curl does:
        /// <c>Name: value</c> adds a header (a repeated name is joined with <c>", "</c>; for Host the first
        /// one wins), <c>Name:</c> stops curl sending its own header of that name, <c>Name;</c> sends the
        /// header with an empty value, and <c>@file</c> reads one header per line from a file.
        /// </summary>
        private void ParseHeader(string header, CurlOptions options)
        {
            // Handle header from file (@file)
            if (header.StartsWith("@"))
            {
                // curl reads one header per line from the file; a missing or unreadable file (or "-",
                // standard input, which a library does not have) fails with exit code 26 like curl
                var path = header.Substring(1);
                foreach (var line in ReadAllLinesForOption(path, "-H @file", options.OriginalCommand))
                {
                    if (line.Trim().Length > 0)
                    {
                        ParseHeaderLine(line, options);
                    }
                }

                return;
            }

            ParseHeaderLine(header, options);
        }

        private static void ParseHeaderLine(string header, CurlOptions options)
        {
            var colonIndex = header.IndexOf(':');
            var semicolonIndex = header.IndexOf(';');

            if (semicolonIndex > 0 && colonIndex < 0)
            {
                // "Name;" sends an empty header, but only when the semicolon ends the argument
                // ("X-Test;  " and "X-Test;junk" are not headers at all, as in curl's http.c)
                if (semicolonIndex == header.Length - 1)
                {
                    var emptyName = header.Substring(0, semicolonIndex).Trim();
                    if (emptyName.Length > 0)
                    {
                        options.EmptyHeaders.Add(emptyName);
                    }
                }

                return;
            }

            if (colonIndex <= 0)
            {
                return; // curl ignores -H values without a colon or semicolon
            }

            var key = header.Substring(0, colonIndex).Trim();
            if (key.Length == 0)
            {
                return;
            }

            // curl keeps the value byte for byte after the separating whitespace ("X-Test: foo; " keeps its trailing space)
            var value = header.Substring(colonIndex + 1).TrimStart(' ', '\t').TrimEnd('\r', '\n');

            // curl sends the line exactly as written, even "Authorization : x" or " Odd Name : y"; HttpClient
            // cannot, so the written form is kept for the request-head rewrite (RequestRewrite)
            var writtenName = header.Substring(0, colonIndex);
            if (value.Trim().Length > 0 && (writtenName != key || !RequestRewrite.IsToken(key)))
            {
                options.RawHeaderLines[key] = header.TrimEnd('\r', '\n');
            }

            if (value.Trim().Length == 0)
            {
                // "Name:" removes curl's own header; a custom value given earlier for the same name is still sent
                if (!options.Headers.TryGetValue(key, out var earlier) || string.IsNullOrEmpty(earlier))
                {
                    options.Headers[key] = "";
                }

                return;
            }

            // Special handling for User-Agent header
            if (key.Equals("User-Agent", StringComparison.OrdinalIgnoreCase))
            {
                options.UserAgent = value;
            }

            if (options.ImplicitHeaders.Remove(key))
            {
                options.Headers[key] = value; // replaces a header --json added
                return;
            }

            if (options.Headers.TryGetValue(key, out var existing) && !string.IsNullOrEmpty(existing))
            {
                if (key.Equals("Host", StringComparison.OrdinalIgnoreCase))
                {
                    return; // curl uses the first Host: header
                }

                options.Headers[key] = existing + ", " + value;
                return;
            }

            options.Headers[key] = value;
        }

        /// <summary>
        /// Parses one <c>-F</c> argument with curl's rules (<c>name=value</c>, <c>name=@file</c>,
        /// <c>name=&lt;file</c>, quoted words, <c>;type=</c>, <c>;filename=</c>, <c>;headers=</c>) into
        /// <see cref="CurlOptions.FormParts"/>, keeping <see cref="CurlOptions.FormData"/> and
        /// <see cref="CurlOptions.Files"/> filled for code that reads them.
        /// </summary>
        private void ParseFormField(string field, CurlOptions options)
        {
            AddFormPart(field, literal: false, options);
        }

        private void ParseCredentials(string userPass, CurlOptions options)
        {
            // curl splits at the FIRST colon: -u "user:pa:ss" is user "user" with password "pa:ss",
            // and -u user (no colon) is a user with an empty password.
            options.Credentials = SplitCredentials(userPass);
        }

        private void ParseProxyCredentials(string userPass, CurlOptions options)
        {
            options.ProxyCredentials = SplitCredentials(userPass);
        }

        /// <summary>Splits <c>user[:password]</c> at the first colon, as curl does for -u and -U.</summary>
        private static NetworkCredential SplitCredentials(string userPass)
        {
            var colon = userPass.IndexOf(':');
            return colon < 0
                ? new NetworkCredential(userPass, string.Empty)
                : new NetworkCredential(userPass.Substring(0, colon), userPass.Substring(colon + 1));
        }

        private void ParseResolve(string resolve, CurlOptions options)
        {
            // [+]host:port:address[,address]... adds an entry; -host:port removes one (ignored here).
            var parts = resolve.Split(':');
            if (resolve.StartsWith("-", StringComparison.Ordinal))
            {
                return;
            }
            if (parts.Length < 3 || !IsPort(parts[1]))
            {
                throw new CurlOptionSyntaxException("--resolve",
                    CurlException.Compose(
                        parts.Length < 3
                            ? $"Couldn't parse --resolve \"{resolve}\": it has {parts.Length} part(s), but HOST:PORT:ADDRESS needs three."
                            : $"Couldn't parse --resolve \"{resolve}\": the port \"{parts[1]}\" is not a number from 0 to 65535.",
                        "Write it as HOST:PORT:ADDRESS[,ADDRESS], for example --resolve example.com:443:127.0.0.1 (IPv6 addresses in brackets: example.com:443:[::1]).",
                        DocLinks.Dns),
                    options.OriginalCommand);
            }

            var hostPort = parts[0] + ":" + parts[1];
            var address = string.Join(":", parts.Skip(2));
            options.Resolve[hostPort] = address;
        }

        /// <summary>
        /// <c>--connect-to HOST1:PORT1:HOST2:PORT2</c>: requests for HOST1:PORT1 connect to HOST2:PORT2
        /// instead (an empty part means "any host/port" on the left and "unchanged" on the right).
        /// </summary>
        private static void ParseConnectTo(string value, CurlOptions options)
        {
            if (!TrySplitConnectTo(value, out _, out var port1, out _, out var port2)
                || (port1.Length > 0 && !IsPort(port1)) || (port2.Length > 0 && !IsPort(port2)))
            {
                throw new CurlOptionSyntaxException("--connect-to",
                    CurlException.Compose(
                        $"Couldn't parse --connect-to \"{value}\": it must be four colon-separated parts, HOST1:PORT1:HOST2:PORT2, with numeric (or empty) ports.",
                        "Write it as, for example, --connect-to example.com:443:127.0.0.1:8443, or --connect-to ::127.0.0.1:8080 to send every host and port to 127.0.0.1:8080.",
                        DocLinks.Dns),
                    options.OriginalCommand);
            }
            options.ConnectTo.Add(value);
        }

        /// <summary>Splits a <c>--connect-to</c> value into its four parts (IPv6 addresses go in brackets).</summary>
        internal static bool TrySplitConnectTo(string value, out string host1, out string port1, out string host2, out string port2)
        {
            host1 = port1 = host2 = port2 = string.Empty;
            var parts = new List<string>();
            var current = new StringBuilder();
            var bracket = false;
            foreach (var c in value)
            {
                if (c == '[') bracket = true;
                if (c == ']') bracket = false;
                if (c == ':' && !bracket)
                {
                    parts.Add(current.ToString());
                    current.Clear();
                    continue;
                }
                current.Append(c);
            }
            parts.Add(current.ToString());
            if (parts.Count != 4)
            {
                return false;
            }
            host1 = parts[0];
            port1 = parts[1];
            host2 = parts[2];
            port2 = parts[3];
            return true;
        }

        private static bool IsPort(string text) =>
            text.Length > 0 && text.Length <= 5 && text.All(char.IsDigit) && int.Parse(text, CultureInfo.InvariantCulture) <= 65535;

        /// <summary>
        /// Applies curl's <c>--data-urlencode</c> rules to one argument:
        /// <c>content</c>, <c>=content</c>, <c>name=content</c>, <c>@file</c> and <c>name@file</c>.
        /// Only the content part is encoded; the name and the <c>=</c> separator are kept. Like curl 8,
        /// spaces become <c>+</c> and every other byte outside <c>A-Z a-z 0-9 - . _ ~</c> becomes <c>%XX</c>.
        /// </summary>
        /// <param name="value">The raw argument following <c>--data-urlencode</c>.</param>
        /// <returns>The encoded <c>name=value</c> pair (or bare encoded content).</returns>
        /// <exception cref="CurlReadErrorException">Thrown with exit code 26 when a referenced file cannot be read.</exception>
        private static string UrlEncodeDataArgument(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            // curl looks for '=' first; only when there is none does '@' mean "read a file".
            var eq = value.IndexOf('=');
            if (eq >= 0)
            {
                var name = value.Substring(0, eq);
                var content = value.Substring(eq + 1);
                return name.Length > 0 ? name + "=" + FormEncode(content) : FormEncode(content);
            }

            var at = value.IndexOf('@');
            if (at >= 0)
            {
                var name = value.Substring(0, at);
                var path = value.Substring(at + 1);
                var fileContent = Encoding.UTF8.GetString(RequestShaping.ReadFileForOption(path, "--data-urlencode @file", null));
                return name.Length > 0 ? name + "=" + FormEncode(fileContent) : FormEncode(fileContent);
            }

            return FormEncode(value);
        }

        /// <summary>
        /// <see cref="PercentEncode"/> with spaces as <c>+</c>, the encoding curl's <c>--data-urlencode</c>
        /// and <c>--url-query</c> use.
        /// </summary>
        private static string FormEncode(string value)
        {
            return PercentEncode(value).Replace("%20", "+");
        }

        /// <summary>
        /// Percent-encodes a string the way curl's <c>curl_easy_escape</c> does: UTF-8 bytes, with only
        /// ASCII letters, digits and <c>- . _ ~</c> left as-is. Spaces become <c>%20</c>, never <c>+</c>.
        /// </summary>
        /// <param name="value">The text to encode.</param>
        /// <returns>The percent-encoded text.</returns>
        public static string PercentEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            var bytes = Encoding.UTF8.GetBytes(value);
            var sb = new StringBuilder(bytes.Length * 3);
            foreach (var b in bytes)
            {
                var c = (char)b;
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') ||
                    c == '-' || c == '.' || c == '_' || c == '~')
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append('%').Append(b.ToString("X2"));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Records one <c>-d</c>-family argument. Items are joined when the transfer is finished (see
        /// <c>BuildRequestBody</c>): <c>@file</c> items are read then, so <c>-d a=1 -d @file</c> works.
        /// </summary>
        private static void AddDataPart(CurlOptions options, string value, RequestDataFlavor flavor)
        {
            value = value ?? string.Empty;
            var isFile = flavor != RequestDataFlavor.Raw && value.StartsWith("@", StringComparison.Ordinal);
            options.DataParts.Add(new RequestDataPart(flavor, isFile ? value.Substring(1) : value, isFile));
        }

        /// <summary>
        /// Applies curl's <c>-e</c> rules: <c>URL</c> sends that Referer, <c>URL;auto</c> also updates it on
        /// redirects and <c>;auto</c> alone sends none on the first request.
        /// </summary>
        /// <summary>
        /// <c>-b</c> can be repeated: every <c>name=value</c> string is sent, joined with <c>"; "</c>, and
        /// every file (an argument without <c>=</c>) is read into the cookie engine, like curl.
        /// </summary>
        private static void AddCookieArgument(CurlOptions options, string value)
        {
            var current = options.Cookie;
            var isLiteral = value.IndexOf('=') >= 0;
            if (current == null)
            {
                options.Cookie = value;
                return;
            }

            var currentIsLiteral = current.IndexOf('=') >= 0;
            if (isLiteral)
            {
                if (currentIsLiteral)
                {
                    options.Cookie = current + "; " + value;
                }
                else
                {
                    options.CookieFiles.Add(current);
                    options.Cookie = value;
                }
            }
            else if (currentIsLiteral)
            {
                options.CookieFiles.Add(value);
            }
            else
            {
                options.CookieFiles.Add(current);
                options.Cookie = value;
            }
        }

        private static void ParseReferer(string value, CurlOptions options)
        {
            var auto = value.IndexOf(";auto", StringComparison.Ordinal);
            if (auto >= 0)
            {
                options.AutoReferer = true;
                value = value.Substring(0, auto);
            }
            else
            {
                options.AutoReferer = false;
            }

            options.Referer = value.Length > 0 ? value : null;
        }

        /// <summary>
        /// curl's <c>-r</c> handling: a bare number <c>N</c> becomes <c>N-</c>; anything else (multi-ranges
        /// <c>0-99,200-299</c>, suffix <c>-500</c>, prefix <c>500-</c>) is sent unchanged.
        /// </summary>
        private static string NormalizeRange(string value)
        {
            value = value.Trim();
            if (value.Length > 0 && char.IsDigit(value[0]) && value.IndexOf('-') < 0 && value.All(char.IsDigit))
            {
                return value + "-";
            }

            return value;
        }

        /// <summary>
        /// curl's <c>-z</c>: <c>DATE</c> / <c>+DATE</c> for If-Modified-Since, <c>-DATE</c> for
        /// If-Unmodified-Since, <c>=DATE</c> for an exact Last-Modified match; when DATE is not a date it is
        /// tried as a file name whose modification time is used. Neither: the condition is dropped, as curl does.
        /// </summary>
        private static void ParseTimeCondition(string value, CurlOptions options)
        {
            var text = value;
            var kind = CurlTimeCondition.IfModifiedSince;
            if (text.StartsWith("+", StringComparison.Ordinal))
            {
                text = text.Substring(1);
            }
            else if (text.StartsWith("-", StringComparison.Ordinal))
            {
                kind = CurlTimeCondition.IfUnmodifiedSince;
                text = text.Substring(1);
            }
            else if (text.StartsWith("=", StringComparison.Ordinal))
            {
                kind = CurlTimeCondition.LastModified;
                text = text.Substring(1);
            }

            options.TimeConditionType = kind;
            if (RequestDate.TryParse(text, out var date))
            {
                options.TimeCondition = date;
            }
            else if (text.Length > 0 && File.Exists(text))
            {
                options.TimeCondition = new DateTimeOffset(File.GetLastWriteTimeUtc(text), TimeSpan.Zero);
            }
            else
            {
                // curl: "Illegal date format for -z, --time-cond (and not a filename). Disabling time condition."
                options.TimeCondition = null;
            }
        }

        /// <summary>Parses <c>--max-filesize</c>: bytes, or a number with a k/M/G/T/P suffix.</summary>
        /// <exception cref="CurlException">Exit code 2 when the value is not a size.</exception>
        /// <summary>
        /// curl's str2udouble for a seconds option (<c>-m</c>, <c>--connect-timeout</c>, <c>--retry-delay</c>,
        /// <c>--retry-max-time</c>): a non-negative number, decimals allowed. Anything else fails with exit code 2
        /// ("expected a proper numerical parameter"), naming the option and the accepted format.
        /// </summary>
        internal static double ParseSecondsOption(string value, string option, string? command)
        {
            var text = (value ?? string.Empty).Trim();
            if (double.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var seconds)
                && !double.IsInfinity(seconds) && !double.IsNaN(seconds))
            {
                return seconds;
            }

            var negative = text.StartsWith("-", StringComparison.Ordinal) && double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out _);
            throw new CurlException(
                CurlException.Compose(
                    $"option {option}: expected a {(negative ? "positive" : "proper")} numerical parameter, but got \"{value}\".",
                    $"Give {option} a number of seconds, decimals allowed and written with a dot (for example {(option.IndexOf('/') >= 0 ? option.Substring(option.IndexOf('/') + 1) : option)} 2.5).",
                    DocLinks.Timeout),
                2,
                command);
        }

        /// <summary>
        /// curl's str2num/str2unum for a counting option (<c>--retry</c>, <c>--max-redirs</c>, <c>--speed-*</c>):
        /// a whole number not below <paramref name="minimum"/>; anything else fails with exit code 2.
        /// </summary>
        internal static long ParseWholeNumberOption(string value, string option, long minimum, string? command, string format)
        {
            var text = (value ?? string.Empty).Trim();
            if (long.TryParse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var number) && number >= minimum
                && number <= int.MaxValue)
            {
                return number;
            }

            var tooSmall = long.TryParse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var parsed) && parsed < minimum;
            throw new CurlException(
                CurlException.Compose(
                    $"option {option}: expected a {(tooSmall ? "positive" : "proper")} numerical parameter, but got \"{value}\".",
                    $"Give {option} {format}.",
                    DocLinks.CliCompatibility),
                2,
                command);
        }

        /// <summary>
        /// curl's <c>--limit-rate</c> value: bytes per second as a whole number with an optional k, M, G, T or P
        /// suffix (powers of 1024); anything else fails with exit code 2.
        /// </summary>
        private static long ParseRateOption(string value, string option, string? command)
        {
            if (RequestShaping.TryParseSize(value, out var rate))
            {
                return rate;
            }

            throw new CurlException(
                CurlException.Compose(
                    $"option {option}: \"{value}\" is not a transfer rate: it expects a whole number of bytes per second, optionally with a k, M, G, T or P suffix.",
                    $"Write it as, for example, {option} 100k (100 KB per second) or {option} 2M.",
                    DocLinks.CliCompatibility),
                2,
                command);
        }

        private static long ParseMaxFileSize(string value, string? command)
        {
            if (RequestShaping.TryParseSize(value, out var size))
            {
                return size;
            }

            throw new CurlException(
                CurlException.Compose(
                    $"--max-filesize \"{value}\" is not a size: it expects a whole number of bytes, optionally with a k, M, G, T or P suffix.",
                    "Write it as, for example, --max-filesize 500k or --max-filesize 10M (10 megabytes).",
                    DocLinks.CliCompatibility),
                2,
                command);
        }
    }
}

