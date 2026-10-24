/***************************************************************************
 * Comprehensive Exception Types for CurlDotNet
 *
 * Meaningful, catchable exceptions for every failure scenario
 *
 * Based on curl's error handling in libcurl by Daniel Stenberg and contributors
 * Original curl Copyright (C) 1996-2025, Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024-2025 Jacob Mellor and IronSoftware
 *
 * By Jacob Mellor
 * GitHub: https://github.com/jacob-mellor
 * Sponsored by IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CurlDotNet.Exceptions
{
    /// <summary>
    /// Base exception for all curl operations. This is the base class for all curl-specific exceptions.
    /// </summary>
    /// <remarks>
    /// <para>This exception provides common properties for all curl errors including the command that was executed and the curl error code.</para>
    /// <para>Curl error codes match the original curl error codes from the C implementation.</para>
    /// <para>AI-Usage: Catch this exception type to handle any curl-related error generically.</para>
    /// <para>AI-Pattern: Use specific derived exceptions for targeted error handling.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     var result = await curl.ExecuteAsync("curl https://api.example.com");
    /// }
    /// catch (CurlConnectionException ex)
    /// {
    ///     // Handle connection-specific issues
    ///     Console.WriteLine($"Failed to connect to {ex.Host}:{ex.Port}");
    /// }
    /// catch (CurlException ex)
    /// {
    ///     // Handle any other curl error
    ///     Console.WriteLine($"Curl failed: {ex.Message}");
    ///     Console.WriteLine($"Command: {ex.Command}");
    ///     Console.WriteLine($"Error code: {ex.CurlErrorCode}");
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public class CurlException : Exception
    {
        /// <summary>
        /// Gets the curl command that was being executed when the exception occurred.
        /// </summary>
        /// <value>The full curl command string, or null if not applicable.</value>
        /// <remarks>
        /// <para>This property contains the exact command that was passed to the Execute method.</para>
        /// <para>AI-Usage: Use this for logging and debugging to understand what command failed.</para>
        /// </remarks>
        public string Command { get; }

        /// <summary>
        /// Gets the curl error code matching the original curl implementation.
        /// </summary>
        /// <value>The curl error code (e.g., 6 for DNS resolution failure, 28 for timeout), or null if not a curl-specific error.</value>
        /// <remarks>
        /// <para>Error codes match the CURLE_* constants from curl.h</para>
        /// <para>Common codes: 6=DNS failure, 7=connection failed, 28=timeout, 35=SSL error</para>
        /// <para>AI-Usage: Use this to determine the specific type of curl error programmatically.</para>
        /// </remarks>
        public int? CurlErrorCode { get; }

        /// <summary>
        /// Key in <see cref="Exception.Data"/> under which a failed command keeps the text curl would
        /// already have printed to standard output.
        /// </summary>
        /// <remarks>
        /// <para>curl writes as it goes: with <c>-i</c> the headers of a <c>-f</c> failure are on screen,
        /// <c>--fail-with-body</c> prints the error page, <c>-w</c> still expands for a failed transfer, and
        /// with several URLs (<c>url1 url2</c>, globs, <c>--next</c>) the output of every transfer that ran
        /// before the failure is there. CurlDotNet throws instead of returning a result, so that text
        /// travels on the exception under this key (a <see cref="string"/>). It is the complete standard
        /// output, <c>-w</c> text included; <see cref="WriteOutDataKey"/> holds only the <c>-w</c> part.</para>
        /// <para>Usage: <c>catch (CurlException ex) { Console.Write(ex.StandardOutput); }</c> reproduces what
        /// the curl tool would have shown before it exited with <see cref="CurlErrorCode"/>.</para>
        /// </remarks>
        /// <seealso cref="StandardOutput"/>
        /// <seealso cref="WriteOutDataKey"/>
        public const string StandardOutputDataKey = "CurlDotNet.Stdout";

        /// <summary>
        /// Key in <see cref="Exception.Data"/> holding the standard-output part of <c>-w</c>/<c>--write-out</c>
        /// for a failed transfer.
        /// </summary>
        /// <remarks>
        /// <para>curl expands <c>-w</c> even when a transfer fails, so <c>%{exitcode}</c>, <c>%{errormsg}</c>
        /// and <c>%{onerror}</c> are useful. The expanded text (without <c>%{stderr}</c> parts) is stored
        /// under this key as a <see cref="string"/>; it is also included in <see cref="StandardOutput"/>.</para>
        /// <para>Usage: <c>curl -w '%{exitcode} %{errormsg}' URL</c>, then read
        /// <c>ex.Data[CurlException.WriteOutDataKey]</c> in the <c>catch</c> block.</para>
        /// </remarks>
        /// <seealso cref="StandardOutputDataKey"/>
        public const string WriteOutDataKey = "CurlDotNet.WriteOut";

        /// <summary>
        /// The text curl would have written to standard output before this failure, or null when nothing was written.
        /// </summary>
        /// <value>Response headers (<c>-i</c>), bodies, <c>-w</c> output and the output of earlier transfers of
        /// the same command, exactly as the curl tool prints them.</value>
        /// <remarks>
        /// <para>Usage: run <c>curl -i -f URL</c>; on a 404 the <see cref="CurlHttpReturnedErrorException"/>
        /// has <c>StandardOutput</c> set to the response headers, just like the terminal output of curl.
        /// With <c>curl URL1 URL2</c> where the second fails, it holds the body of the first.</para>
        /// </remarks>
        /// <seealso cref="StandardOutputDataKey"/>
        public string StandardOutput => Data[StandardOutputDataKey] as string;

        /// <summary>
        /// Extra key/value facts attached with <see cref="WithContext(string, object)"/>, such as a request id
        /// or the tenant a call was made for, so that logs show them next to the error.
        /// </summary>
        /// <value>A mutable dictionary; empty unless your own code (or middleware) adds entries. CurlDotNet
        /// itself never adds keys here.</value>
        /// <remarks>
        /// <para>The entries are printed by <see cref="ToDetailedString"/>, serialized by <see cref="ToJson"/> and
        /// returned by <see cref="GetDiagnosticInfo"/>. They do not change <see cref="Message"/>.</para>
        /// <para>Example: <c>catch (CurlException ex) { throw ex.WithContext("orderId", 1234); }</c> then
        /// <c>logger.LogError(ex.ToDetailedString())</c> prints <c>orderId: 1234</c> in the Context section.</para>
        /// </remarks>
        /// <seealso cref="WithContext(string, object)"/>
        /// <seealso cref="WithContext(Dictionary{string, object})"/>
        public Dictionary<string, object> Context { get; } = new Dictionary<string, object>();

        /// <summary>
        /// How to fix this error, one sentence per entry, taken from the message (every line between the
        /// first line and the <c>See https://...</c> link) or, when the message has none, from curl's exit code.
        /// </summary>
        /// <value>Never null. Every exception CurlDotNet throws has at least one entry; an exception you
        /// construct yourself with a one-line message and no exit code has none until you call
        /// <see cref="WithSuggestion(string)"/>.</value>
        /// <remarks>
        /// <para>The same text already appears in <see cref="Message"/>; this list exists so that a UI or a log
        /// can show "what happened" (<see cref="WhatHappened"/>) and "how to fix it" separately, for example as
        /// bullet points under an error banner.</para>
        /// <para>Example: <c>catch (CurlException ex) { foreach (var fix in ex.Suggestions) Console.WriteLine("- " + fix); }</c>
        /// prints, for <c>curl https://no-such-host.invalid</c>, a line telling you to check the spelling of the
        /// host and your DNS settings.</para>
        /// <para>HTTP errors (<see cref="CurlHttpException"/>) add status-specific advice, such as checking your
        /// token for a 401 or backing off for a 429.</para>
        /// </remarks>
        /// <seealso cref="WithSuggestion(string)"/>
        /// <seealso cref="WhatHappened"/>
        /// <seealso cref="DocumentationUrl"/>
        /// <seealso cref="ToUserFriendlyMessage"/>
        public List<string> Suggestions
        {
            get
            {
                EnsureParsed();
                return _suggestions!;
            }
        }

        /// <summary>
        /// The first line of <see cref="Message"/>: what went wrong, with the concrete values involved
        /// (URL, host, option, file path, status code), but without the fix or the documentation link.
        /// </summary>
        /// <value>A single line such as <c>Could not resolve host: api.exmaple.com (nodename nor servname provided)</c>.</value>
        /// <remarks>
        /// <para>This is the text curl itself prints after <c>curl: (6)</c>, and what <c>-w '%{errormsg}'</c>
        /// expands to. Use it for short status lines; use <see cref="Message"/> or <see cref="ToUserFriendlyMessage"/>
        /// when the reader also needs the fix.</para>
        /// <para>Example: <c>statusLabel.Text = ex.WhatHappened;</c> and <c>helpLink.NavigateUri = ex.DocumentationUrl;</c>.</para>
        /// </remarks>
        /// <seealso cref="Suggestions"/>
        /// <seealso cref="DocumentationUrl"/>
        public string WhatHappened
        {
            get
            {
                EnsureParsed();
                return _what!;
            }
        }

        /// <summary>
        /// The documentation page that explains this error and how to fix it: the <c>See https://...</c> link
        /// at the end of <see cref="Message"/>.
        /// </summary>
        /// <value>An absolute <c>https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/...</c> URL, or null
        /// for an exception you constructed yourself without a link and without an exit code.</value>
        /// <remarks>
        /// <para>When the message carries no link but <see cref="CurlErrorCode"/> is set, the page for that exit
        /// code is used (for example the DNS page for 6, the timeout page for 28, the TLS page for 60).</para>
        /// <para>Example: <c>Process.Start(new ProcessStartInfo(ex.DocumentationUrl) { UseShellExecute = true });</c>
        /// opens the page in the browser from a desktop tool.</para>
        /// </remarks>
        /// <seealso cref="WhatHappened"/>
        /// <seealso cref="Suggestions"/>
        public string? DocumentationUrl
        {
            get
            {
                EnsureParsed();
                return _link;
            }
        }

        /// <summary>
        /// The full error text: what happened (first line), how to fix it (following lines) and a
        /// <c>See https://...</c> documentation link (last line).
        /// </summary>
        /// <value>For example:
        /// <c>Could not resolve host: api.exmaple.com (No such host is known)</c>, then
        /// <c>Check the spelling of the host in the URL and your DNS settings; --resolve HOST:PORT:ADDRESS pins a name to an address without DNS.</c>,
        /// then <c>See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md</c>.</value>
        /// <remarks>
        /// <para>Older message formats such as <c>For DNS troubleshooting, see: URL</c> are normalised to a
        /// single trailing <c>See URL</c> line, so a message never carries two links. When a CurlDotNet
        /// exception has no link of its own, the page for its exit code is appended. A message you pass to a
        /// constructor yourself is returned unchanged unless it contains such a link or an exit code is set.</para>
        /// <para><see cref="Exception.ToString"/>, <see cref="ToDetailedString"/>, <see cref="ToJson"/> and
        /// <see cref="ToUserFriendlyMessage"/> all use this normalised text.</para>
        /// </remarks>
        /// <seealso cref="WhatHappened"/>
        /// <seealso cref="Suggestions"/>
        /// <seealso cref="DocumentationUrl"/>
        public override string Message
        {
            get
            {
                EnsureParsed();
                return _message!;
            }
        }

        /// <summary>
        /// Free-form diagnostic text attached with <see cref="WithDiagnostics(string)"/>, such as the
        /// output of <c>-v</c> or a trace file path, printed by <see cref="ToDetailedString"/>.
        /// </summary>
        /// <value>Null unless your code sets it; CurlDotNet does not fill it.</value>
        /// <remarks>
        /// <para>Example: <c>throw ex.WithDiagnostics(File.ReadAllText("trace.txt"));</c> after running the command
        /// with <c>--trace-ascii trace.txt</c>, so the full wire log travels with the exception.</para>
        /// </remarks>
        /// <seealso cref="WithDiagnostics(string)"/>
        /// <seealso cref="ToDetailedString"/>
        public string DiagnosticInfo { get; set; }

        private string? _message;
        private string? _what;
        private string? _link;
        private List<string>? _suggestions;

        /// <summary>
        /// Creates an exception with a message and, optionally, the command that failed and the exception
        /// that caused it.
        /// </summary>
        /// <remarks>
        /// <para>Write <paramref name="message"/> as three parts: what happened on the first line (with the
        /// concrete URL, file or option), how to fix it on the following line(s), and
        /// <c>See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/...</c> on the last line. The fix
        /// lines become <see cref="Suggestions"/> and the link becomes <see cref="DocumentationUrl"/>.</para>
        /// <para>Example: <c>throw new CurlException("Config file 'app.curlrc' is empty.\nAdd at least one URL line.\nSee https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md", command);</c></para>
        /// <para>This overload leaves <see cref="CurlErrorCode"/> null. Use
        /// <see cref="CurlException(string, int, string, Exception)"/> when you know the curl exit code.</para>
        /// </remarks>
        /// <param name="message">What happened, how to fix it, and a <c>See URL</c> line, separated by newlines.
        /// A plain one-line message is also accepted and returned unchanged by <see cref="Message"/>.</param>
        /// <param name="command">The curl command (or a description of the API call) that failed, for
        /// <see cref="Command"/>; null when there was none.</param>
        /// <param name="innerException">The .NET exception that caused this one, kept as
        /// <see cref="Exception.InnerException"/>; null when there is none.</param>
        /// <seealso cref="CurlException(string, int, string, Exception)"/>
        public CurlException(string message, string command = null, Exception innerException = null)
            : base(message, innerException)
        {
            Command = command;
        }

        /// <summary>
        /// Creates an exception that carries curl's exit code, so callers can switch on
        /// <see cref="CurlErrorCode"/> exactly like a shell script switches on <c>$?</c>.
        /// </summary>
        /// <remarks>
        /// <para>Exit codes are the <c>CURLE_*</c> values from <c>curl.h</c>: 6 = could not resolve host,
        /// 7 = could not connect, 22 = HTTP error with <c>-f</c>, 23 = write error, 28 = timeout, 60 = certificate
        /// not trusted. The full table is at
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md.</para>
        /// <para>Example: <c>throw new CurlException("Upload of 'big.iso' was refused by the server.\nSplit the file or raise the server limit.", 25, command);</c>
        /// produces a message ending with the documentation link for exit code 25.</para>
        /// </remarks>
        /// <param name="message">What happened, how to fix it and (optionally) a <c>See URL</c> line, separated by
        /// newlines. When no link is given, the page for <paramref name="curlErrorCode"/> is appended.</param>
        /// <param name="curlErrorCode">The curl exit code, 1 to 100 (see <c>man curl</c>, EXIT CODES).</param>
        /// <param name="command">The curl command that failed, or null.</param>
        /// <seealso cref="CurlException(string, int, string, Exception)"/>
        /// <seealso cref="GetCurlErrorName"/>
        public CurlException(string message, int curlErrorCode, string command = null)
            : base(message)
        {
            Command = command;
            CurlErrorCode = curlErrorCode;
        }

        /// <summary>
        /// Creates an exception that carries curl's exit code and wraps the .NET exception that caused it
        /// (for example an <see cref="System.IO.IOException"/> while writing <c>-o</c> output).
        /// </summary>
        /// <remarks>
        /// <para>The wrapped exception stays available as <see cref="Exception.InnerException"/>; the message
        /// should still say in words what the inner exception means, because many loggers only print the
        /// outer message.</para>
        /// <para>Example: <c>catch (IOException io) { throw new CurlException($"Could not write 'out.json': {io.Message}\nCheck that the folder exists and is writable.", 23, command, io); }</c></para>
        /// </remarks>
        /// <param name="message">What happened (including the inner cause in words), how to fix it, and
        /// optionally a <c>See URL</c> line, separated by newlines.</param>
        /// <param name="curlErrorCode">The curl exit code, 1 to 100.</param>
        /// <param name="command">The curl command that failed, or null.</param>
        /// <param name="innerException">The exception that caused this one, or null.</param>
        /// <seealso cref="CurlException(string, int, string)"/>
        /// <seealso cref="CurlException(string, string, Exception)"/>
        public CurlException(string message, int curlErrorCode, string command, Exception innerException)
            : base(message, innerException)
        {
            Command = command;
            CurlErrorCode = curlErrorCode;
        }

        /// <summary>
        /// Builds a three-part message: <paramref name="what"/> on the first line, <paramref name="fix"/> on
        /// the second, and <c>See </c><paramref name="docUrl"/> on the last.
        /// </summary>
        internal static string Compose(string what, string fix, string docUrl)
        {
            var text = what.TrimEnd();
            if (!string.IsNullOrWhiteSpace(fix))
            {
                text += "\n" + fix.Trim();
            }
            if (!string.IsNullOrEmpty(docUrl))
            {
                text += "\nSee " + docUrl;
            }
            return text;
        }

        private static readonly System.Text.RegularExpressions.Regex LinkPattern = new System.Text.RegularExpressions.Regex(
            @"(?<![^\s.(])(?:(?:for|more)\b[^\n.:;]*?,?\s*)?(?:see|see:|see also:?|docs?:|help:)\s*(?<url>https://github\.com/jacob-mellor/curl-dot-net/[^\s)]+?)[.)]?\s*$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.CultureInvariant);

        /// <summary>
        /// Splits a raw message into what happened, the fix lines and the documentation link.
        /// </summary>
        internal static (string What, List<string> Fixes, string? Link) ParseMessage(string? raw)
        {
            var fixes = new List<string>();
            string? link = null;
            var kept = new List<string>();
            foreach (var original in (raw ?? string.Empty).Replace("\r\n", "\n").Split('\n'))
            {
                var line = original.TrimEnd();
                var match = LinkPattern.Match(line);
                if (match.Success)
                {
                    link ??= match.Groups["url"].Value.TrimEnd('.');
                    line = line.Substring(0, match.Index).TrimEnd();
                    if (line.EndsWith(",", StringComparison.Ordinal) || line.EndsWith(";", StringComparison.Ordinal))
                    {
                        line = line.Substring(0, line.Length - 1);
                    }
                }
                if (line.Trim().Length > 0)
                {
                    kept.Add(line.Trim());
                }
            }

            var what = kept.Count > 0 ? kept[0] : string.Empty;
            for (var i = 1; i < kept.Count; i++)
            {
                fixes.Add(kept[i]);
            }
            return (what, fixes, link);
        }

        private void EnsureParsed()
        {
            if (_message != null)
            {
                return;
            }

            var raw = base.Message;
            var (what, fixes, link) = ParseMessage(raw);

            // A wrapper must say in words what the wrapped exception reported, because many loggers
            // print only the outer message.
            var cause = InnerCause(InnerException);
            if (cause != null && raw.IndexOf(cause.Value.Detail, StringComparison.Ordinal) < 0)
            {
                what = what.TrimEnd('.') + $" (caused by {cause.Value.Type}: {cause.Value.Detail})";
            }

            var suggestions = new List<string>(fixes);
            link ??= DefaultDocumentationUrl ?? (CurlErrorCode.HasValue ? DocLinks.ForExitCode(CurlErrorCode) : null);
            var addedFix = false;
            if (suggestions.Count == 0)
            {
                var fallback = DefaultSuggestion ?? (CurlErrorCode.HasValue ? DefaultFix(CurlErrorCode.Value) : null);
                if (fallback != null)
                {
                    suggestions.Add(fallback);
                    addedFix = true;
                }
            }

            string message;
            if (link == null && fixes.Count == 0 && !addedFix && cause == null)
            {
                message = raw; // a caller's own one-line message stays exactly as given
            }
            else
            {
                var sb = new StringBuilder(what);
                foreach (var fix in fixes)
                {
                    sb.Append('\n').Append(fix);
                }
                if (addedFix)
                {
                    sb.Append('\n').Append(suggestions[0]);
                }
                if (link != null)
                {
                    sb.Append("\nSee ").Append(link);
                }
                message = sb.ToString();
            }

            _what = what;
            _link = link;
            _suggestions = _suggestions ?? suggestions;
            _message = message;
        }

        /// <summary>The wrapped exception's type and first message line, or null when there is none.</summary>
        private static (string Type, string Detail)? InnerCause(Exception? inner)
        {
            if (inner == null || string.IsNullOrWhiteSpace(inner.Message))
            {
                return null;
            }
            var firstLine = inner is CurlException curl ? curl.WhatHappened : inner.Message;
            var newline = firstLine.IndexOf('\n');
            if (newline >= 0)
            {
                firstLine = firstLine.Substring(0, newline);
            }
            firstLine = firstLine.Trim().TrimEnd('.');
            if (firstLine.Length > 200)
            {
                firstLine = firstLine.Substring(0, 200) + "...";
            }
            return firstLine.Length == 0 ? ((string, string)?)null : (inner.GetType().Name, firstLine);
        }

        /// <summary>
        /// The documentation page for this exception type, used when the message does not end with its own
        /// <c>See https://...</c> link.
        /// </summary>
        /// <value>Null in <see cref="CurlException"/> itself (the page for <see cref="CurlErrorCode"/> is used
        /// instead, when there is a code). Derived types return the page that explains their error, for example
        /// the cookie page for <see cref="CurlCookieException"/>.</value>
        /// <remarks>
        /// <para>Override this in your own exception types that derive from <see cref="CurlException"/> so that
        /// <see cref="Message"/> and <see cref="DocumentationUrl"/> always point somewhere useful. It is read
        /// lazily, the first time <see cref="Message"/> is used, so overrides may use the derived type's
        /// properties.</para>
        /// <para>Example: <c>protected override string? DefaultDocumentationUrl =&gt; "https://wiki.example.com/payments-api-errors";</c></para>
        /// </remarks>
        /// <seealso cref="DefaultSuggestion"/>
        /// <seealso cref="DocumentationUrl"/>
        protected virtual string? DefaultDocumentationUrl => null;

        /// <summary>
        /// How to fix this kind of error in one sentence, used as <see cref="Suggestions"/> (and as the second
        /// line of <see cref="Message"/>) when the message itself does not say how to fix it.
        /// </summary>
        /// <value>Null in <see cref="CurlException"/> itself (the generic advice for <see cref="CurlErrorCode"/>
        /// is used instead, when there is a code). Derived types return advice specific to their error.</value>
        /// <remarks>
        /// <para>Read lazily, the first time <see cref="Message"/> or <see cref="Suggestions"/> is used, so an
        /// override can mention the derived type's properties, such as the file path.</para>
        /// <para>Example: <c>protected override string? DefaultSuggestion =&gt; $"Check that '{FilePath}' exists and is readable.";</c></para>
        /// </remarks>
        /// <seealso cref="DefaultDocumentationUrl"/>
        /// <seealso cref="Suggestions"/>
        protected virtual string? DefaultSuggestion => null;

        /// <summary>
        /// The generic fix for a curl exit code, used when a message does not say how to fix the problem.
        /// </summary>
        internal static string DefaultFix(int code)
        {
            switch (code)
            {
                case 1: return "Use an http://, https://, ftp://, ftps:// or file:// URL; other schemes are not implemented by CurlDotNet.";
                case 2: return "Check the command for an option whose value could not be used (an unreadable file, an invalid certificate path) and correct it.";
                case 3: return "Check the URL: it needs a scheme such as https://, a host name, and percent-encoding (%20) for spaces; quote it when it contains & or ?.";
                case 4: return "This curl feature is not part of CurlDotNet; remove the option or use the native curl tool for it.";
                case 5: return "Check the proxy host in -x/--proxy (or http_proxy/https_proxy/all_proxy) for typos, or use --noproxy to connect directly.";
                case 6: return "Check the spelling of the host name and your network/DNS settings; --resolve HOST:PORT:ADDRESS pins a name to an address without DNS.";
                case 7: return "Check that the server is running and listening on that host and port, and that no firewall or proxy blocks the connection.";
                case 8: return "The server's reply is not valid for the protocol; check that the URL scheme (http vs https) and port match the service.";
                case 9: return "The server denied access to the resource; check the credentials (-u user:password) and the path.";
                case 18: return "The connection closed before the whole body arrived; retry, or resume the download with -C -.";
                case 22: return "The server answered with an HTTP error and -f/--fail was used; check the URL and credentials, or drop -f to get the response as a CurlResult.";
                case 23: return "Check that the output path (-o, -D, -c, --trace, --etag-save) points to an existing, writable folder, or add --create-dirs.";
                case 25: return "The server refused the upload; check the target path, permissions and the server's size limit.";
                case 26: return "Check that the file named after @ (in -d @file, -F name=@file, -T file or -K file) exists and is readable.";
                case 27: return "The response or request was too large to hold in memory; download to a file with -o instead of reading Body.";
                case 28: return "Raise -m/--max-time or --connect-timeout (seconds), or check that the server is reachable and responding.";
                case 33: return "The server ignored the byte range; download the whole file again without -C/--continue-at or -r/--range.";
                case 35: return "The TLS handshake failed; check that the port speaks TLS, that the server supports TLS 1.2 or later, and any client certificate (--cert).";
                case 36: return "The resume offset does not match the server's file; delete the partial download and start again without -C.";
                case 37: return "Check that the file:// path exists and is readable by the current user.";
                case 42: return "The transfer was cancelled by your CancellationToken or a callback; pass a token that is not cancelled, or a longer timeout.";
                case 43: return "One option value or combination is not valid for this request; correct the option named in the message.";
                case 45: return "Check the network interface name or address given with --interface.";
                case 47: return "Raise --max-redirs, or fix the redirect loop on the server (check the chain with -i -L).";
                case 48: return "Check the spelling of the option; run curl --help all for the list of options.";
                case 49: return "Correct the syntax of the option value named in the message.";
                case 52: return "The server closed the connection without replying; check that the port and scheme are right (http vs https) and look at the server logs.";
                case 55: return "Sending the request failed; check the network connection and retry (--retry).";
                case 56: return "Receiving the response failed; check the network connection and retry (--retry).";
                case 58: return "Check the client certificate and key given with --cert/--key (path, format and password).";
                case 59: return "Use a cipher list (--ciphers) that the server and this platform both support.";
                case 60: return "The server certificate is not trusted; install the issuing CA (--cacert), fix the server certificate, or use -k/--insecure for testing only.";
                case 61: return "The response encoding could not be decoded; drop --compressed or check the server's Content-Encoding header.";
                case 63: return "Raise --max-filesize, or download a smaller resource.";
                case 64: return "The server does not support the TLS level required by --ssl-reqd; use a server that supports TLS or drop the option.";
                case 67: return "The login was rejected; check the user name and password (-u user:password).";
                case 77: return "Check the CA certificate file or folder given with --cacert/--capath.";
                case 94: return "The authentication exchange failed; check the credentials and the scheme (--basic, --digest, --ntlm).";
                case 100: return "The server sent more header data than curl accepts; this is a server problem.";
                default: return $"See the curl exit code {code} entry in the linked documentation for the cause and the fix.";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The streaming context.</param>
        protected CurlException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Command = info.GetString(nameof(Command));
            CurlErrorCode = (int?)info.GetValue(nameof(CurlErrorCode), typeof(int?));
        }

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The streaming context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Command), Command);
            info.AddValue(nameof(CurlErrorCode), CurlErrorCode);
        }

        #region Fluent Methods for Easy Inspection

        /// <summary>
        /// Add contextual information to the exception (fluent).
        /// </summary>
        /// <param name="key">Context key</param>
        /// <param name="value">Context value</param>
        /// <returns>This exception for chaining</returns>
        public CurlException WithContext(string key, object value)
        {
            Context[key] = value;
            return this;
        }

        /// <summary>
        /// Add multiple context values (fluent).
        /// </summary>
        /// <param name="context">Dictionary of context values</param>
        /// <returns>This exception for chaining</returns>
        public CurlException WithContext(Dictionary<string, object> context)
        {
            foreach (var kvp in context)
            {
                Context[kvp.Key] = kvp.Value;
            }
            return this;
        }

        /// <summary>
        /// Add a suggestion for resolving this error (fluent).
        /// </summary>
        /// <param name="suggestion">Helpful suggestion text</param>
        /// <returns>This exception for chaining</returns>
        public CurlException WithSuggestion(string suggestion)
        {
            if (!Suggestions.Contains(suggestion))
            {
                Suggestions.Add(suggestion);
            }
            return this;
        }

        /// <summary>
        /// Add diagnostic information (fluent).
        /// </summary>
        /// <param name="diagnosticInfo">Diagnostic details</param>
        /// <returns>This exception for chaining</returns>
        public CurlException WithDiagnostics(string diagnosticInfo)
        {
            DiagnosticInfo = diagnosticInfo;
            return this;
        }

        /// <summary>
        /// Get a detailed string representation of the exception.
        /// </summary>
        /// <returns>Detailed error information</returns>
        public virtual string ToDetailedString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"=== CURL EXCEPTION DETAILS ===");
            sb.AppendLine($"Type: {GetType().Name}");
            sb.AppendLine($"What happened: {WhatHappened}");
            if (!string.IsNullOrEmpty(DocumentationUrl))
                sb.AppendLine($"Documentation: {DocumentationUrl}");

            if (!string.IsNullOrEmpty(Command))
                sb.AppendLine($"Command: {Command}");

            if (CurlErrorCode.HasValue)
            {
                sb.AppendLine($"Curl Error Code: {CurlErrorCode} ({GetCurlErrorName()})");
            }

            if (Context.Count > 0)
            {
                sb.AppendLine("Context:");
                foreach (var kvp in Context)
                {
                    sb.AppendLine($"  {kvp.Key}: {kvp.Value}");
                }
            }

            if (Suggestions.Count > 0)
            {
                sb.AppendLine("How to fix:");
                foreach (var suggestion in Suggestions)
                {
                    sb.AppendLine($"  - {suggestion}");
                }
            }

            if (!string.IsNullOrEmpty(DiagnosticInfo))
            {
                sb.AppendLine($"Diagnostics: {DiagnosticInfo}");
            }

            if (InnerException != null)
            {
                sb.AppendLine($"Inner Exception: {InnerException.GetType().Name}: {InnerException.Message}");
            }

            sb.AppendLine($"Stack Trace:\n{StackTrace}");
            sb.AppendLine("=============================");

            return sb.ToString();
        }

        /// <summary>
        /// Get the exception as a structured JSON object for logging.
        /// </summary>
        /// <returns>JSON representation of the exception</returns>
        public virtual string ToJson()
        {
            var errorObject = new Dictionary<string, object>
            {
                ["type"] = GetType().Name,
                ["message"] = Message,
                ["whatHappened"] = WhatHappened,
                ["documentationUrl"] = DocumentationUrl,
                ["command"] = Command,
                ["curlErrorCode"] = CurlErrorCode,
                ["curlErrorName"] = GetCurlErrorName(),
                ["context"] = Context,
                ["suggestions"] = Suggestions,
                ["diagnostics"] = DiagnosticInfo,
                ["isRetryable"] = IsRetryable(),
                ["stackTrace"] = StackTrace,
                ["innerException"] = InnerException?.Message
            };

#if NETSTANDARD2_0
            return Newtonsoft.Json.JsonConvert.SerializeObject(errorObject, Newtonsoft.Json.Formatting.Indented);
#else
            return System.Text.Json.JsonSerializer.Serialize(errorObject, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
#endif
        }

        /// <summary>
        /// Get diagnostic information for debugging.
        /// </summary>
        /// <returns>Diagnostic details</returns>
        public virtual Dictionary<string, object> GetDiagnosticInfo()
        {
            return new Dictionary<string, object>
            {
                ["ExceptionType"] = GetType().FullName,
                ["Message"] = Message,
                ["Command"] = Command ?? "N/A",
                ["CurlErrorCode"] = CurlErrorCode?.ToString() ?? "N/A",
                ["CurlErrorName"] = GetCurlErrorName(),
                ["Timestamp"] = DateTime.UtcNow.ToString("O"),
                ["MachineName"] = Environment.MachineName,
                ["OSVersion"] = Environment.OSVersion.ToString(),
                ["IsRetryable"] = IsRetryable(),
                ["Context"] = Context,
                ["Suggestions"] = Suggestions,
                ["HasInnerException"] = InnerException != null,
                ["InnerExceptionType"] = InnerException?.GetType().Name,
                ["InnerExceptionMessage"] = InnerException?.Message
            };
        }

        /// <summary>
        /// Get the CURLE_* constant name for the error code.
        /// </summary>
        /// <returns>Curl error constant name</returns>
        public string GetCurlErrorName()
        {
            if (!CurlErrorCode.HasValue) return null;

            return CurlErrorCode.Value switch
            {
                0 => "CURLE_OK",
                1 => "CURLE_UNSUPPORTED_PROTOCOL",
                2 => "CURLE_FAILED_INIT",
                3 => "CURLE_URL_MALFORMAT",
                4 => "CURLE_NOT_BUILT_IN",
                5 => "CURLE_COULDNT_RESOLVE_PROXY",
                6 => "CURLE_COULDNT_RESOLVE_HOST",
                7 => "CURLE_COULDNT_CONNECT",
                8 => "CURLE_WEIRD_SERVER_REPLY",
                9 => "CURLE_FTP_ACCESS_DENIED",
                18 => "CURLE_PARTIAL_FILE",
                22 => "CURLE_HTTP_RETURNED_ERROR",
                23 => "CURLE_WRITE_ERROR",
                26 => "CURLE_READ_ERROR",
                27 => "CURLE_OUT_OF_MEMORY",
                28 => "CURLE_OPERATION_TIMEDOUT",
                35 => "CURLE_SSL_CONNECT_ERROR",
                33 => "CURLE_RANGE_ERROR",
                43 => "CURLE_BAD_FUNCTION_ARGUMENT",
                47 => "CURLE_TOO_MANY_REDIRECTS",
                51 => "CURLE_PEER_FAILED_VERIFICATION",
                52 => "CURLE_GOT_NOTHING",
                53 => "CURLE_SSL_ENGINE_NOTFOUND",
                54 => "CURLE_SSL_ENGINE_SETFAILED",
                55 => "CURLE_SEND_ERROR",
                56 => "CURLE_RECV_ERROR",
                58 => "CURLE_SSL_CERTPROBLEM",
                59 => "CURLE_SSL_CIPHER",
                60 => "CURLE_PEER_FAILED_VERIFICATION",
                61 => "CURLE_BAD_CONTENT_ENCODING",
                63 => "CURLE_FILESIZE_EXCEEDED",
                67 => "CURLE_LOGIN_DENIED",
                94 => "CURLE_AUTH_ERROR",
                100 => "CURLE_TOO_LARGE",
                _ => $"CURLE_UNKNOWN_{CurlErrorCode}"
            };
        }

        /// <summary>
        /// Check if this error is potentially retryable.
        /// </summary>
        /// <returns>True if the error might succeed on retry</returns>
        public virtual bool IsRetryable()
        {
            if (!CurlErrorCode.HasValue) return false;

            // These error codes are potentially transient and worth retrying
            return CurlErrorCode.Value switch
            {
                6 => true,  // DNS resolution might be temporary
                7 => true,  // Connection failed - might be temporary
                28 => true, // Timeout - might work with longer timeout
                35 => false, // SSL errors usually require configuration changes
                52 => true, // Got nothing - server might be temporarily overloaded
                55 => true, // Send error - network glitch
                56 => true, // Recv error - network glitch
                _ => false
            };
        }

        /// <summary>
        /// Log this exception with structured data.
        /// </summary>
        /// <param name="logger">Action to perform logging</param>
        public void Log(Action<string, Dictionary<string, object>> logger)
        {
            logger(ToDetailedString(), GetDiagnosticInfo());
        }

        /// <summary>
        /// Create a user-friendly error message.
        /// </summary>
        /// <returns>Simplified error message for end users</returns>
        public virtual string ToUserFriendlyMessage()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Error: {WhatHappened}");

            if (Suggestions.Count > 0)
            {
                sb.AppendLine("\nPossible solutions:");
                foreach (var suggestion in Suggestions)
                {
                    sb.AppendLine($"• {suggestion}");
                }
            }

            if (IsRetryable())
            {
                sb.AppendLine("\nThis error might be temporary. Please try again.");
            }

            if (!string.IsNullOrEmpty(DocumentationUrl))
            {
                sb.AppendLine($"\nMore help: {DocumentationUrl}");
            }

            return sb.ToString();
        }

        #endregion
    }

    /// <summary>
    /// Thrown when the curl command syntax is invalid or cannot be parsed.
    /// </summary>
    /// <remarks>
    /// <para>This exception indicates a problem with the curl command syntax, not a network or execution error.</para>
    /// <para>AI-Usage: Catch this to handle command syntax errors separately from execution errors.</para>
    /// <para>AI-Pattern: Validate commands before execution to avoid this exception.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     // Missing URL will throw CurlInvalidCommandException
    ///     var result = await curl.ExecuteAsync("curl -X POST");
    /// }
    /// catch (CurlInvalidCommandException ex)
    /// {
    ///     Console.WriteLine($"Invalid command syntax: {ex.Message}");
    ///     Console.WriteLine($"Problem with: {ex.InvalidPart}");
    ///     // Suggest correction to user
    ///     Console.WriteLine("Did you forget to specify a URL?");
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public class CurlInvalidCommandException : CurlException
    {
        /// <summary>
        /// Gets the part of the command that is invalid.
        /// </summary>
        /// <value>The specific option, argument, or syntax element that caused the parsing error.</value>
        /// <remarks>
        /// <para>This helps identify exactly what part of the command is wrong.</para>
        /// <para>AI-Usage: Use this to provide specific feedback about what needs to be corrected.</para>
        /// </remarks>
        public string InvalidPart { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlInvalidCommandException"/> class.
        /// </summary>
        /// <param name="message">The error message describing the invalid syntax.</param>
        /// <param name="invalidPart">The specific part of the command that is invalid.</param>
        /// <param name="command">The full curl command that failed to parse.</param>
        public CurlInvalidCommandException(string message, string invalidPart = null, string command = null)
            : base(message, command)
        {
            InvalidPart = invalidPart;
        }

        /// <summary>
        /// Initializes a new instance that carries the curl exit code of the problem.
        /// </summary>
        /// <param name="message">The error message describing the invalid syntax and how to fix it.</param>
        /// <param name="curlErrorCode">The curl exit code (for example 49 for an option value curl cannot parse).</param>
        /// <param name="invalidPart">The specific part of the command that is invalid.</param>
        /// <param name="command">The full curl command that failed.</param>
        protected CurlInvalidCommandException(string message, int curlErrorCode, string invalidPart, string command)
            : base(message, curlErrorCode, command)
        {
            InvalidPart = invalidPart;
        }

        /// <summary>
        /// Initializes a new instance with serialized data.
        /// </summary>
        protected CurlInvalidCommandException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            InvalidPart = info.GetString(nameof(InvalidPart));
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(InvalidPart), InvalidPart);
        }
    }

    /// <summary>
    /// Thrown when a network connection cannot be established to the target host.
    /// </summary>
    /// <remarks>
    /// <para>This exception indicates network-level connection failures, not HTTP errors.</para>
    /// <para>Common causes include: host unreachable, port closed, firewall blocking, network timeout.</para>
    /// <para>AI-Usage: Catch this to implement retry logic or fallback servers.</para>
    /// <para>AI-Pattern: Check Host and Port properties to log connection details.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     var result = await curl.ExecuteAsync("curl https://internal-server:8443/api");
    /// }
    /// catch (CurlDnsException ex)
    /// {
    ///     // DNS couldn't resolve the hostname
    ///     Console.WriteLine($"DNS lookup failed for {ex.Host}");
    ///     // Try fallback server
    ///     result = await curl.ExecuteAsync("curl https://backup-server/api");
    /// }
    /// catch (CurlConnectionException ex)
    /// {
    ///     // Connection failed but DNS worked
    ///     Console.WriteLine($"Cannot connect to {ex.Host}:{ex.Port}");
    ///     Console.WriteLine("Check if the service is running and firewall rules");
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public class CurlConnectionException : CurlException
    {
        /// <summary>
        /// Gets the host that could not be connected to.
        /// </summary>
        /// <value>The hostname or IP address that failed to connect.</value>
        /// <remarks>
        /// <para>This may be a hostname (e.g., "api.example.com") or IP address (e.g., "192.168.1.1").</para>
        /// <para>AI-Usage: Use this to implement host-specific retry or fallback logic.</para>
        /// </remarks>
        public string Host { get; }

        /// <summary>
        /// Gets the port number that was attempted.
        /// </summary>
        /// <value>The TCP port number, or null if using the default port for the protocol.</value>
        /// <remarks>
        /// <para>Default ports: HTTP=80, HTTPS=443, FTP=21, FTPS=990.</para>
        /// <para>AI-Usage: Check if non-standard ports might be blocked by firewalls.</para>
        /// </remarks>
        public int? Port { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlConnectionException"/> class.
        /// </summary>
        /// <param name="message">The error message describing the connection failure.</param>
        /// <param name="host">The host that could not be connected to.</param>
        /// <param name="port">The port number that was attempted.</param>
        /// <param name="command">The curl command that was executing.</param>
        public CurlConnectionException(string message, string host, int? port = null, string command = null)
            : base(message, command)
        {
            Host = host;
            Port = port;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlConnectionException"/> class with an error code.
        /// </summary>
        /// <param name="message">The error message describing the connection failure.</param>
        /// <param name="curlErrorCode">The curl error code.</param>
        /// <param name="host">The host that could not be connected to.</param>
        /// <param name="port">The port number that was attempted.</param>
        /// <param name="command">The curl command that was executing.</param>
        protected CurlConnectionException(string message, int curlErrorCode, string host, int? port = null, string command = null)
            : base(message, curlErrorCode, command)
        {
            Host = host;
            Port = port;
        }

        /// <summary>
        /// Initializes a new instance with serialized data.
        /// </summary>
        protected CurlConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Host = info.GetString(nameof(Host));
            Port = (int?)info.GetValue(nameof(Port), typeof(int?));
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Host), Host);
            info.AddValue(nameof(Port), Port);
        }
    }

    /// <summary>
    /// Thrown when DNS resolution fails for a hostname.
    /// </summary>
    /// <remarks>
    /// <para>This exception indicates the hostname could not be resolved to an IP address.</para>
    /// <para>Curl error code: CURLE_COULDNT_RESOLVE_HOST (6)</para>
    /// <para>AI-Usage: Catch this to handle DNS failures separately from other connection issues.</para>
    /// <para>AI-Pattern: Check for typos in hostname or DNS server configuration.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     var result = await curl.ExecuteAsync("curl https://non-existent-domain.invalid");
    /// }
    /// catch (CurlDnsException ex)
    /// {
    ///     Console.WriteLine($"DNS lookup failed for: {ex.Host}");
    ///
    ///     // Suggest alternatives
    ///     if (ex.Host.Contains("github"))
    ///         Console.WriteLine("Did you mean: github.com?");
    ///
    ///     // Or try with IP address directly
    ///     var ipAddress = "140.82.114.3"; // github.com IP
    ///     result = await curl.ExecuteAsync($"curl https://{ipAddress}");
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public class CurlDnsException : CurlConnectionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlDnsException"/> class.
        /// </summary>
        /// <param name="hostname">The hostname that could not be resolved.</param>
        /// <param name="command">The curl command that was executing.</param>
        public CurlDnsException(string hostname, string command = null)
            : base($"Could not resolve host: {hostname}\n" +
                   $"For DNS troubleshooting, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md",
                   6, hostname, null, command)
        {
            // CURLE_COULDNT_RESOLVE_HOST = 6
        }

        /// <summary>
        /// Initializes a new instance with serialized data.
        /// </summary>
        protected CurlDnsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when an operation exceeds the configured timeout period.
    /// </summary>
    /// <remarks>
    /// <para>This can occur for connection timeout or total operation timeout.</para>
    /// <para>Curl error code: CURLE_OPERATION_TIMEDOUT (28)</para>
    /// <para>AI-Usage: Catch this to implement retry with longer timeout or fail fast.</para>
    /// <para>AI-Pattern: Log timeout value to help diagnose if timeout is too short.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     // Set a 5 second timeout
    ///     var result = await curl.ExecuteAsync("curl --max-time 5 https://slow-server.com/large-file");
    /// }
    /// catch (CurlTimeoutException ex)
    /// {
    ///     Console.WriteLine($"Operation timed out after {ex.Timeout?.TotalSeconds ?? 0} seconds");
    ///
    ///     // Retry with longer timeout
    ///     Console.WriteLine("Retrying with 30 second timeout...");
    ///     result = await curl.ExecuteAsync("curl --max-time 30 https://slow-server.com/large-file");
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public class CurlTimeoutException : CurlException
    {
        /// <summary>
        /// Gets the timeout duration that was exceeded.
        /// </summary>
        /// <value>The timeout duration, or null if not specified.</value>
        /// <remarks>
        /// <para>This represents the --max-time or --connect-timeout value that was exceeded.</para>
        /// <para>AI-Usage: Use this to determine if timeout should be increased.</para>
        /// </remarks>
        public TimeSpan? Timeout { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlTimeoutException"/> class.
        /// </summary>
        /// <param name="message">The error message describing the timeout.</param>
        /// <param name="command">The curl command that was executing.</param>
        /// <param name="timeout">The timeout duration that was exceeded.</param>
        public CurlTimeoutException(string message, string command = null, TimeSpan? timeout = null)
            : base($"{message}\nFor timeout configuration, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/timeout-errors.md",
                   28, command) // CURLE_OPERATION_TIMEDOUT = 28
        {
            Timeout = timeout;
        }

        /// <summary>
        /// Initializes a new instance with serialized data.
        /// </summary>
        protected CurlTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Timeout = (TimeSpan?)info.GetValue(nameof(Timeout), typeof(TimeSpan?));
        }

        /// <inheritdoc/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Timeout), Timeout);
        }
    }

    /// <summary>
    /// Thrown when SSL/TLS certificate validation fails
    /// </summary>
    public class CurlSslException : CurlException
    {
        /// <summary>
        /// Details about the certificate error that occurred.
        /// </summary>
        public string? CertificateError { get; }

        /// <summary>
        /// Initializes a new instance of CurlSslException.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="certError">Details about the certificate error.</param>
        /// <param name="command">The curl command that failed.</param>
        public CurlSslException(string message, string? certError = null, string? command = null)
            : base($"{message}\nFor SSL/TLS troubleshooting, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md",
                   60, command) // CURLE_PEER_FAILED_VERIFICATION = 60
        {
            CertificateError = certError;
        }

        /// <summary>
        /// Initializes a new instance with a specific curl exit code, such as 35 (CURLE_SSL_CONNECT_ERROR)
        /// for a TLS handshake that failed for a reason other than certificate verification.
        /// </summary>
        /// <param name="message">What happened and how to fix it (the documentation link is appended).</param>
        /// <param name="certError">Details about the certificate error, if any.</param>
        /// <param name="command">The curl command that failed.</param>
        /// <param name="curlErrorCode">The curl exit code.</param>
        protected CurlSslException(string message, string? certError, string? command, int curlErrorCode)
            : base($"{message}\nFor SSL/TLS troubleshooting, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md",
                   curlErrorCode, command)
        {
            CertificateError = certError;
        }
    }

    /// <summary>
    /// Thrown when authentication fails
    /// </summary>
    public class CurlAuthenticationException : CurlException
    {
        /// <summary>
        /// Gets the authentication method that failed.
        /// </summary>
        public string AuthMethod { get; }

        /// <summary>
        /// Initializes a new instance of the CurlAuthenticationException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="authMethod">The authentication method that failed.</param>
        /// <param name="command">The curl command that caused the error.</param>
        public CurlAuthenticationException(string message, string authMethod = null, string command = null)
            : base($"{message}\nFor authentication help, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/auth-errors.md",
                   67, command) // CURLE_LOGIN_DENIED = 67
        {
            AuthMethod = authMethod;
        }
    }

    /// <summary>
    /// Thrown for HTTP error responses when using ThrowOnError()
    /// </summary>
    public class CurlHttpException : CurlException
    {
        /// <summary>
        /// Gets the HTTP status code of the response.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Gets the HTTP status text of the response.
        /// </summary>
        public string StatusText { get; }

        /// <summary>
        /// Gets the response body content.
        /// </summary>
        public string ResponseBody { get; }

        /// <summary>
        /// Gets or sets the response headers.
        /// </summary>
        public Dictionary<string, string> ResponseHeaders { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the CurlHttpException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="statusText">The HTTP status text.</param>
        /// <param name="responseBody">The response body content.</param>
        /// <param name="command">The curl command that caused the error.</param>
        public CurlHttpException(string message, int statusCode, string statusText = null, string responseBody = null, string command = null)
            : base($"{message}\nFor HTTP error handling, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md",
                   command)
        {
            StatusCode = statusCode;
            StatusText = statusText;
            ResponseBody = responseBody;

            // Add automatic suggestions based on status code
            AutoAddSuggestions();
        }

        /// <summary>
        /// Initializes a new instance that also carries a curl exit code, for HTTP failures that end a
        /// command (such as <c>-f</c>/<c>--fail</c>, exit code 22) rather than being returned as a result.
        /// </summary>
        /// <param name="message">What happened and how to fix it (the documentation link is appended).</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="statusText">The reason phrase from the status line.</param>
        /// <param name="responseBody">The response body, when it was read.</param>
        /// <param name="command">The curl command that caused the error.</param>
        /// <param name="curlErrorCode">The curl exit code, for example 22 (CURLE_HTTP_RETURNED_ERROR).</param>
        protected CurlHttpException(string message, int statusCode, string? statusText, string? responseBody, string? command, int curlErrorCode)
            : base($"{message}\nFor HTTP error handling, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md",
                   curlErrorCode, command)
        {
            StatusCode = statusCode;
            StatusText = statusText;
            ResponseBody = responseBody;
            AutoAddSuggestions();
        }

        /// <summary>
        /// Check if this is a client error (4xx)
        /// </summary>
        public bool IsClientError => StatusCode >= 400 && StatusCode < 500;

        /// <summary>
        /// Check if this is a server error (5xx)
        /// </summary>
        public bool IsServerError => StatusCode >= 500 && StatusCode < 600;

        /// <summary>
        /// Check if this is a specific status code.
        /// </summary>
        public bool IsStatus(int code) => StatusCode == code;

        /// <summary>
        /// Check if this is unauthorized (401).
        /// </summary>
        public bool IsUnauthorized => StatusCode == 401;

        /// <summary>
        /// Check if this is forbidden (403).
        /// </summary>
        public bool IsForbidden => StatusCode == 403;

        /// <summary>
        /// Check if this is not found (404).
        /// </summary>
        public bool IsNotFound => StatusCode == 404;

        /// <summary>
        /// Check if this is a rate limit error (429).
        /// </summary>
        public bool IsRateLimited => StatusCode == 429;

        /// <summary>
        /// Add response headers (fluent).
        /// </summary>
        public CurlHttpException WithHeaders(Dictionary<string, string> headers)
        {
            ResponseHeaders = headers;
            return this;
        }

        /// <summary>
        /// Get retry-after value if present in headers.
        /// </summary>
        public TimeSpan? GetRetryAfter()
        {
            if (ResponseHeaders.TryGetValue("Retry-After", out var retryAfter))
            {
                if (int.TryParse(retryAfter, out var seconds))
                {
                    return TimeSpan.FromSeconds(seconds);
                }
            }
            return null;
        }

        private void AutoAddSuggestions()
        {
            switch (StatusCode)
            {
                case 400:
                    Suggestions.Add("Check the request parameters and body format");
                    Suggestions.Add("Ensure all required fields are included");
                    break;
                case 401:
                    Suggestions.Add("Check your authentication credentials");
                    Suggestions.Add("Ensure your API key or token is valid");
                    Suggestions.Add("Check if the token has expired");
                    break;
                case 403:
                    Suggestions.Add("Check if you have permission to access this resource");
                    Suggestions.Add("Verify your API key has the required scopes");
                    break;
                case 404:
                    Suggestions.Add("Verify the URL is correct");
                    Suggestions.Add("Check if the resource exists");
                    break;
                case 429:
                    Suggestions.Add("You've hit a rate limit - wait before retrying");
                    Suggestions.Add("Consider implementing exponential backoff");
                    break;
                case 500:
                case 502:
                case 503:
                    Suggestions.Add("This is a server error - try again later");
                    Suggestions.Add("If the problem persists, contact the API provider");
                    break;
            }
        }

        /// <summary>
        /// Returns a detailed string representation of the exception including HTTP status information.
        /// </summary>
        /// <returns>A detailed string representation of the exception.</returns>
        public override string ToDetailedString()
        {
            var details = base.ToDetailedString();
            var sb = new StringBuilder(details);

            sb.AppendLine($"HTTP Status: {StatusCode} {StatusText}");
            sb.AppendLine($"Is Client Error: {IsClientError}");
            sb.AppendLine($"Is Server Error: {IsServerError}");

            if (!string.IsNullOrEmpty(ResponseBody))
            {
                sb.AppendLine($"Response Body: {ResponseBody}");
            }

            if (ResponseHeaders.Count > 0)
            {
                sb.AppendLine("Response Headers:");
                foreach (var header in ResponseHeaders)
                {
                    sb.AppendLine($"  {header.Key}: {header.Value}");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Determines whether the HTTP error is retryable.
        /// </summary>
        /// <returns>true if the error is retryable (429 or 5xx status codes); otherwise, false.</returns>
        public override bool IsRetryable()
        {
            // Server errors and rate limits are typically retryable
            return StatusCode == 429 || StatusCode >= 500;
        }
    }

    /// <summary>
    /// Thrown when file operations fail
    /// </summary>
    public class CurlFileException : CurlException
    {
        /// <summary>
        /// Gets the file path that caused the error.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the file operation that failed.
        /// </summary>
        public FileOperation Operation { get; }

        /// <summary>
        /// Specifies the type of file operation.
        /// </summary>
        public enum FileOperation
        {
            /// <summary>
            /// Reading from a file.
            /// </summary>
            Read,
            /// <summary>
            /// Writing to a file.
            /// </summary>
            Write,
            /// <summary>
            /// Creating a file.
            /// </summary>
            Create,
            /// <summary>
            /// Deleting a file.
            /// </summary>
            Delete,
            /// <summary>
            /// Uploading a file.
            /// </summary>
            Upload,
            /// <summary>
            /// Downloading a file.
            /// </summary>
            Download
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlFileException"/> class
        /// </summary>
        /// <param name="message">The error message describing the file operation failure</param>
        /// <param name="filePath">The path to the file that caused the error</param>
        /// <param name="operation">The type of file operation that failed</param>
        /// <param name="command">The original curl command that was executed</param>
        /// <param name="innerException">The underlying exception that caused this error</param>
        public CurlFileException(string message, string filePath, FileOperation operation, string command = null, Exception innerException = null)
            : base(message, command, innerException)
        {
            FilePath = filePath;
            Operation = operation;
        }
    }

    /// <summary>
    /// Thrown when retry attempts are exhausted
    /// </summary>
    public class CurlRetryException : CurlException
    {
        /// <summary>
        /// Gets the number of retry attempts that were made before failing
        /// </summary>
        public int RetryCount { get; }
        /// <summary>
        /// Gets the exception from the last retry attempt
        /// </summary>
        public Exception LastAttemptException { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlRetryException"/> class
        /// </summary>
        /// <param name="message">The error message describing the retry failure</param>
        /// <param name="command">The original curl command that was executed</param>
        /// <param name="retryCount">The number of retry attempts that were made</param>
        /// <param name="lastException">The exception from the final retry attempt</param>
        public CurlRetryException(string message, string command, int retryCount, Exception lastException)
            : base(message, command, lastException)
        {
            RetryCount = retryCount;
            LastAttemptException = lastException;
        }
    }

    /// <summary>
    /// Thrown when FTP operations fail
    /// </summary>
    public class CurlFtpException : CurlException
    {
        /// <summary>
        /// Gets the FTP response code that caused the error
        /// </summary>
        public int FtpCode { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlFtpException"/> class
        /// </summary>
        /// <param name="message">The error message describing the FTP failure</param>
        /// <param name="ftpCode">The FTP response code</param>
        /// <param name="command">The original curl command that was executed</param>
        public CurlFtpException(string message, int ftpCode, string command = null)
            : base(message, 9, command) // CURLE_FTP_ACCESS_DENIED = 9
        {
            FtpCode = ftpCode;
        }
    }

    /// <summary>
    /// Thrown when proxy connection fails
    /// </summary>
    public class CurlProxyException : CurlConnectionException
    {
        /// <summary>
        /// Gets the proxy hostname or IP address that failed to connect
        /// </summary>
        public string ProxyHost { get; }
        /// <summary>
        /// Gets the proxy port number, if specified
        /// </summary>
        public int? ProxyPort { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlProxyException"/> class
        /// </summary>
        /// <param name="message">The error message describing the proxy connection failure</param>
        /// <param name="proxyHost">The proxy hostname or IP address</param>
        /// <param name="proxyPort">The proxy port number, if applicable</param>
        /// <param name="command">The original curl command that was executed</param>
        public CurlProxyException(string message, string proxyHost, int? proxyPort = null, string command = null)
            : base(message, 5, proxyHost, proxyPort, command)
        {
            ProxyHost = proxyHost;
            ProxyPort = proxyPort;
            // CURLE_COULDNT_RESOLVE_PROXY = 5
        }
    }

    /// <summary>
    /// Thrown when content parsing fails
    /// </summary>
    public class CurlParsingException : CurlException
    {
        /// <summary>
        /// Gets the content type of the data that failed to parse
        /// </summary>
        public string ContentType { get; }
        /// <summary>
        /// Gets the expected type that the content could not be parsed into
        /// </summary>
        public Type ExpectedType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlParsingException"/> class
        /// </summary>
        /// <param name="message">The error message describing the parsing failure</param>
        /// <param name="contentType">The content type of the unparseable data</param>
        /// <param name="expectedType">The type that was expected</param>
        /// <param name="command">The original curl command that was executed</param>
        /// <param name="innerException">The underlying parsing exception</param>
        public CurlParsingException(string message, string contentType, Type expectedType, string command = null, Exception innerException = null)
            : base(message, command, innerException)
        {
            ContentType = contentType;
            ExpectedType = expectedType;
        }
    }

    /// <summary>
    /// Thrown when rate limiting is encountered
    /// </summary>
    public class CurlRateLimitException : CurlHttpException
    {
        /// <summary>
        /// Gets the time to wait before retrying, if provided by the server
        /// </summary>
        public TimeSpan? RetryAfter { get; }
        /// <summary>
        /// Gets the number of remaining requests allowed, if provided
        /// </summary>
        public int? RemainingLimit { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlRateLimitException"/> class
        /// </summary>
        /// <param name="message">The error message describing the rate limit</param>
        /// <param name="retryAfter">The time to wait before retrying</param>
        /// <param name="remainingLimit">The number of requests remaining</param>
        /// <param name="command">The original curl command that was executed</param>
        public CurlRateLimitException(string message, TimeSpan? retryAfter = null, int? remainingLimit = null, string command = null)
            : base(message, 429, "Too Many Requests", null, command)
        {
            RetryAfter = retryAfter;
            RemainingLimit = remainingLimit;
        }
    }

    /// <summary>
    /// Thrown when execution fails for general reasons
    /// </summary>
    public class CurlExecutionException : CurlException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurlExecutionException"/> class
        /// </summary>
        /// <param name="message">The error message describing the execution failure</param>
        /// <param name="command">The original curl command that was executed</param>
        /// <param name="innerException">The underlying exception that caused the failure</param>
        public CurlExecutionException(string message, string command = null, Exception innerException = null)
            : base(message, command, innerException)
        {
        }
    }

    /// <summary>
    /// Thrown when a required feature is not supported
    /// </summary>
    public class CurlNotSupportedException : CurlException
    {
        /// <summary>
        /// Gets the name of the feature that is not supported
        /// </summary>
        public string Feature { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlNotSupportedException"/> class
        /// </summary>
        /// <param name="feature">The name of the unsupported feature</param>
        /// <param name="command">The original curl command that was executed</param>
        public CurlNotSupportedException(string feature, string command = null)
            : base($"Feature not supported: {feature}", 1, command)
        {
            Feature = feature;
            // CURLE_UNSUPPORTED_PROTOCOL = 1
        }
    }

    /// <summary>
    /// Thrown when cookie operations fail during HTTP requests.
    /// </summary>
    /// <remarks>
    /// <para>This exception indicates problems with cookie handling, including cookie jar file access, cookie parsing, or cookie storage.</para>
    /// <para>Common causes: invalid cookie jar path, file permissions, malformed cookies, or disk space issues.</para>
    /// <para>AI-Usage: Catch this to handle cookie-related failures separately from other HTTP errors.</para>
    /// <para>AI-Pattern: Check CookieJarPath property to identify file access issues.</para>
    /// <para>For more information, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/cookie-errors.md</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     var result = await curl.ExecuteAsync("curl --cookie-jar cookies.txt https://example.com");
    /// }
    /// catch (CurlCookieException ex)
    /// {
    ///     Console.WriteLine($"Cookie error: {ex.Message}");
    ///     if (!string.IsNullOrEmpty(ex.CookieJarPath))
    ///     {
    ///         Console.WriteLine($"Cookie jar path: {ex.CookieJarPath}");
    ///         // Check file permissions or try alternative path
    ///     }
    /// }
    /// </code>
    /// </example>
    public class CurlCookieException : CurlException
    {
        /// <summary>
        /// Gets the path to the cookie jar file that caused the error, if applicable.
        /// </summary>
        /// <value>The file path to the cookie jar, or null if not file-related.</value>
        /// <remarks>
        /// <para>This property helps identify file access issues with cookie storage.</para>
        /// <para>AI-Usage: Use this to check file permissions or suggest alternative paths.</para>
        /// </remarks>
        public string CookieJarPath { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlCookieException"/> class.
        /// </summary>
        /// <param name="message">The error message describing the cookie operation failure.</param>
        /// <param name="cookieJarPath">The path to the cookie jar file, if applicable.</param>
        /// <param name="command">The curl command that was executing.</param>
        /// <param name="innerException">The underlying exception that caused this error.</param>
        public CurlCookieException(string message, string cookieJarPath = null, string command = null, Exception innerException = null)
            : base($"{message}\nFor cookie handling help, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/cookie-errors.md", command, innerException)
        {
            CookieJarPath = cookieJarPath;
        }
    }

    /// <summary>
    /// Thrown when redirect limit is exceeded during HTTP requests.
    /// </summary>
    /// <remarks>
    /// <para>This exception indicates that the maximum number of HTTP redirects has been exceeded.</para>
    /// <para>Curl error code: CURLE_TOO_MANY_REDIRECTS (47)</para>
    /// <para>Common causes: redirect loops, misconfigured servers, or intentional redirect chains.</para>
    /// <para>AI-Usage: Catch this to handle redirect issues separately from other HTTP errors.</para>
    /// <para>AI-Pattern: Check RedirectCount and LastUrl to debug redirect chains.</para>
    /// <para>For more information, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/redirect-errors.md</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     // Default limit is usually 50 redirects
    ///     var result = await curl.ExecuteAsync("curl -L https://example.com/redirect-loop");
    /// }
    /// catch (CurlRedirectException ex)
    /// {
    ///     Console.WriteLine($"Too many redirects: {ex.RedirectCount}");
    ///     Console.WriteLine($"Last URL attempted: {ex.LastUrl}");
    ///
    ///     // Try again with limited redirects
    ///     result = await curl.ExecuteAsync("curl --max-redirs 5 https://example.com/redirect-loop");
    /// }
    /// </code>
    /// </example>
    public class CurlRedirectException : CurlException
    {
        /// <summary>
        /// Gets the number of redirects that were followed before the limit was exceeded.
        /// </summary>
        /// <value>The count of redirects attempted.</value>
        /// <remarks>
        /// <para>This helps identify potential redirect loops or excessive redirect chains.</para>
        /// <para>AI-Usage: If count is high, suspect a redirect loop.</para>
        /// </remarks>
        public int RedirectCount { get; }

        /// <summary>
        /// Gets the last URL that was attempted before the redirect limit was exceeded.
        /// </summary>
        /// <value>The final URL in the redirect chain, or null if not available.</value>
        /// <remarks>
        /// <para>This URL can help identify where the redirect loop or chain ends.</para>
        /// <para>AI-Usage: Use this to debug redirect chains and identify problematic URLs.</para>
        /// </remarks>
        public string LastUrl { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurlRedirectException"/> class.
        /// </summary>
        /// <param name="message">The error message describing the redirect limit exceeded.</param>
        /// <param name="redirectCount">The number of redirects that were attempted.</param>
        /// <param name="lastUrl">The last URL in the redirect chain.</param>
        /// <param name="command">The curl command that was executing.</param>
        public CurlRedirectException(string message, int redirectCount, string lastUrl = null, string command = null)
            : base($"{message}\nFor redirect handling help, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/redirect-errors.md", 47, command) // CURLE_TOO_MANY_REDIRECTS = 47
        {
            RedirectCount = redirectCount;
            LastUrl = lastUrl;
        }
    }
}