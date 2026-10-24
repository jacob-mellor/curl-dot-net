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
    /// Base class of every error CurlDotNet throws: catch it to handle any failed curl command, read
    /// <see cref="CurlErrorCode"/> for curl's exit code and <see cref="Message"/> for what happened and how to fix it.
    /// </summary>
    /// <remarks>
    /// <para>Every failure of <c>Curl.ExecuteAsync</c>, <c>Curl.Execute</c>, <see cref="CurlDotNet.Core.CurlRequestBuilder"/>
    /// and <see cref="CurlDotNet.Lib.LibCurl"/> surfaces as a <see cref="CurlException"/> subclass. There is one subclass
    /// per curl exit code (for example <see cref="CurlCouldntResolveHostException"/> for exit code 6,
    /// <see cref="CurlOperationTimeoutException"/> for 28, <see cref="CurlPeerFailedVerificationException"/> for 60), so
    /// you can catch exactly the failures you can handle and let the rest bubble up.</para>
    /// <para>An HTTP error status (404, 500...) is not an exception: it comes back as a normal
    /// <see cref="CurlDotNet.Core.CurlResult"/> with <c>StatusCode</c> set, unless the command uses <c>-f</c>/<c>--fail</c>,
    /// in which case <see cref="CurlHttpReturnedErrorException"/> (exit code 22) is thrown.</para>
    /// <para>The message has three parts: what happened (first line, also <see cref="WhatHappened"/>), how to fix it (the
    /// following lines, also <see cref="Suggestions"/>) and a <c>See https://...</c> documentation link (last line, also
    /// <see cref="DocumentationUrl"/>). <see cref="Command"/> holds the command that failed and
    /// <see cref="StandardOutput"/> what curl would already have printed.</para>
    /// <para>Example: <c>try { var r = await Curl.ExecuteAsync("curl https://api.example.com/users"); }
    /// catch (CurlCouldntResolveHostException ex) { Console.WriteLine($"Bad host {ex.Hostname}"); }
    /// catch (CurlTimeoutException) { /* retry later */ } catch (CurlException ex) { Console.WriteLine($"curl exit {ex.CurlErrorCode}: {ex.Message}"); }</c></para>
    /// <para>Exit code table and handling patterns: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md.
    /// Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     var result = await Curl.ExecuteAsync("curl https://api.example.com");
    /// }
    /// catch (CurlConnectionException ex)
    /// {
    ///     Console.WriteLine($"Failed to connect to {ex.Host}:{ex.Port}");
    /// }
    /// catch (CurlException ex)
    /// {
    ///     Console.WriteLine(ex.WhatHappened);
    ///     foreach (var fix in ex.Suggestions) Console.WriteLine("  fix: " + fix);
    ///     Console.WriteLine($"exit code {ex.CurlErrorCode}, docs: {ex.DocumentationUrl}");
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="CurlHttpReturnedErrorException"/>
    /// <seealso cref="CurlTimeoutException"/>
    /// <seealso cref="CurlConnectionException"/>
    /// <seealso cref="CurlInvalidCommandException"/>
    [Serializable]
    public class CurlException : Exception
    {
        /// <summary>
        /// The curl command (or a description of the API call) that was running when the error happened, exactly
        /// as it was passed in, so you can log it or re-run it in a terminal.
        /// </summary>
        /// <value>The full command string such as <c>curl -sS -H 'Accept: application/json' https://api.example.com</c>;
        /// for <see cref="CurlDotNet.Core.CurlRequestBuilder"/> calls, the equivalent produced by
        /// <c>ToCurlCommand()</c>; null when the error did not come from a command.</value>
        /// <remarks>
        /// <para>The command may contain secrets (<c>-u user:password</c>, <c>-H 'Authorization: Bearer ...'</c>). Mask it
        /// before writing it to shared logs. <see cref="Message"/> itself never includes the whole command.</para>
        /// <para>Example: <c>catch (CurlException ex) { logger.LogWarning("curl failed: {Cmd}", Mask(ex.Command)); }</c></para>
        /// </remarks>
        /// <seealso cref="CurlErrorCode"/>
        /// <seealso cref="StandardOutput"/>
        public string Command { get; }

        /// <summary>
        /// curl's exit code for this failure (the <c>CURLE_*</c> value from <c>curl.h</c>), for example 6 when the
        /// host name cannot be resolved, 7 when the connection is refused, 22 for <c>-f</c> HTTP errors and 28 for a timeout.
        /// </summary>
        /// <value>An integer from 1 to 100 that matches what the curl command-line tool would return as <c>$?</c>; null only
        /// for exceptions that have no curl equivalent (such as a JSON parsing error in <c>ParseJson</c>).</value>
        /// <remarks>
        /// <para>Common codes: 3 malformed URL, 5 proxy not resolved, 6 host not resolved, 7 connection failed, 18 partial body,
        /// 22 HTTP error with <c>-f</c>, 23 write error, 26 read error, 28 timeout, 35 TLS handshake failed, 47 too many redirects,
        /// 52 empty reply, 56 receive error, 60 certificate not trusted. <see cref="GetCurlErrorName"/> returns the
        /// <c>CURLE_</c> name.</para>
        /// <para>Example: <c>catch (CurlException ex) when (ex.CurlErrorCode == 28) { /* timed out: retry with a longer -m */ }</c></para>
        /// <para>Full table: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</para>
        /// </remarks>
        /// <seealso cref="GetCurlErrorName"/>
        /// <seealso cref="IsRetryable"/>
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

            var extra = ExtraSuggestions;
            if (extra != null)
            {
                foreach (var more in extra)
                {
                    if (!string.IsNullOrWhiteSpace(more) && !suggestions.Contains(more))
                    {
                        suggestions.Add(more);
                    }
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
        /// Further fixes appended to <see cref="Suggestions"/> after the ones in the message, for errors where
        /// several remedies apply (for example the status-specific advice of <see cref="CurlHttpException"/>).
        /// </summary>
        /// <value>Null or empty in <see cref="CurlException"/> itself; derived types may return extra sentences.
        /// Entries already in the list are skipped. They are not added to <see cref="Message"/>.</value>
        /// <remarks>
        /// <para>Read lazily, the first time <see cref="Suggestions"/> or <see cref="Message"/> is used, so an override
        /// may use properties set by the derived constructor.</para>
        /// <para>Example: <c>protected override IEnumerable&lt;string&gt;? ExtraSuggestions =&gt; new[] { "Check the service status page." };</c></para>
        /// </remarks>
        /// <seealso cref="DefaultSuggestion"/>
        /// <seealso cref="Suggestions"/>
        protected virtual IEnumerable<string>? ExtraSuggestions => null;

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
        /// Restores an exception (its message, <see cref="Command"/> and <see cref="CurlErrorCode"/>) from serialized
        /// data; used by remoting and binary serialization on .NET Framework.
        /// </summary>
        /// <remarks>
        /// <para>You do not call this yourself. Derived types that add state must call it and restore their own fields.
        /// Binary serialization is obsolete on .NET 8 and later; prefer <see cref="ToJson"/> to move errors between processes.</para>
        /// </remarks>
        /// <param name="info">The serialized object data written by <see cref="GetObjectData"/>.</param>
        /// <param name="context">The source of the serialized stream, supplied by the serializer.</param>
        /// <seealso cref="GetObjectData"/>
        protected CurlException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Command = info.GetString(nameof(Command));
            CurlErrorCode = (int?)info.GetValue(nameof(CurlErrorCode), typeof(int?));
        }

        /// <summary>
        /// Writes the exception, including <see cref="Command"/> and <see cref="CurlErrorCode"/>, into serialization data
        /// so it can cross an AppDomain or remoting boundary on .NET Framework.
        /// </summary>
        /// <remarks>
        /// <para>Called by the serializer, not by your code. On .NET 8 and later binary serialization is obsolete; use
        /// <see cref="ToJson"/> for logs and message queues instead.</para>
        /// </remarks>
        /// <param name="info">The object that receives the serialized values (message, command, exit code).</param>
        /// <param name="context">The destination of the serialized stream, supplied by the serializer.</param>
        /// <seealso cref="ToJson"/>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Command), Command);
            info.AddValue(nameof(CurlErrorCode), CurlErrorCode);
        }

        #region Fluent Methods for Easy Inspection

        /// <summary>
        /// Attaches one key/value fact (a request id, a tenant, a retry number) to this exception and returns the same
        /// exception, so you can enrich and rethrow it in one expression.
        /// </summary>
        /// <remarks>
        /// <para>The value is stored in <see cref="Context"/>, replacing an existing value with the same key, and appears in
        /// <see cref="ToDetailedString"/>, <see cref="ToJson"/> and <see cref="GetDiagnosticInfo"/>. It does not change
        /// <see cref="Message"/>.</para>
        /// <para>Example: <c>catch (CurlException ex) { throw ex.WithContext("orderId", order.Id).WithContext("attempt", 3); }</c>
        /// (use <c>throw;</c> instead when you want to keep the original stack trace).</para>
        /// </remarks>
        /// <param name="key">Name of the fact, such as <c>"orderId"</c>; any non-null string.</param>
        /// <param name="value">The value to record; any object, printed with <c>ToString()</c> in the detailed text.</param>
        /// <returns>This same exception instance, for chaining.</returns>
        /// <seealso cref="WithContext(Dictionary{string, object})"/>
        /// <seealso cref="Context"/>
        public CurlException WithContext(string key, object value)
        {
            Context[key] = value;
            return this;
        }

        /// <summary>
        /// Attaches several key/value facts at once (copied into <see cref="Context"/>) and returns the same exception for
        /// chaining, for example everything your logging scope already knows.
        /// </summary>
        /// <remarks>
        /// <para>Existing keys are overwritten. Example: <c>throw ex.WithContext(new Dictionary&lt;string, object&gt; { ["user"] = userId, ["region"] = "eu" });</c></para>
        /// </remarks>
        /// <param name="context">The facts to copy; each key/value pair is added to <see cref="Context"/>. Must not be null.</param>
        /// <returns>This same exception instance, for chaining.</returns>
        /// <seealso cref="WithContext(string, object)"/>
        /// <seealso cref="Context"/>
        public CurlException WithContext(Dictionary<string, object> context)
        {
            foreach (var kvp in context)
            {
                Context[kvp.Key] = kvp.Value;
            }
            return this;
        }

        /// <summary>
        /// Adds your own advice on how to fix the problem to <see cref="Suggestions"/> (ignored if the same text is already
        /// there) and returns the same exception for chaining.
        /// </summary>
        /// <remarks>
        /// <para>Use it in middleware or wrappers that know something CurlDotNet does not, such as which configuration key
        /// holds the API token. The text is shown by <see cref="ToUserFriendlyMessage"/> and <see cref="ToDetailedString"/>;
        /// <see cref="Message"/> is not changed.</para>
        /// <para>Example: <c>catch (CurlHttpReturnedErrorException ex) when (ex.StatusCode == 401) { throw ex.WithSuggestion("Renew the token in appsettings.json:Api:Token."); }</c></para>
        /// </remarks>
        /// <param name="suggestion">One sentence telling the reader what to do, such as <c>"Renew the API token."</c>.</param>
        /// <returns>This same exception instance, for chaining.</returns>
        /// <seealso cref="Suggestions"/>
        /// <seealso cref="ToUserFriendlyMessage"/>
        public CurlException WithSuggestion(string suggestion)
        {
            if (!Suggestions.Contains(suggestion))
            {
                Suggestions.Add(suggestion);
            }
            return this;
        }

        /// <summary>
        /// Stores free-form diagnostic text (a verbose log, a trace excerpt, environment details) in
        /// <see cref="DiagnosticInfo"/> and returns the same exception for chaining.
        /// </summary>
        /// <remarks>
        /// <para>Replaces any previous value. Printed by <see cref="ToDetailedString"/> and included in <see cref="ToJson"/>.</para>
        /// <para>Example: run the command with <c>--trace-ascii trace.txt</c>, then <c>throw ex.WithDiagnostics(File.ReadAllText("trace.txt"));</c></para>
        /// </remarks>
        /// <param name="diagnosticInfo">The text to attach; any string, may span several lines; null clears it.</param>
        /// <returns>This same exception instance, for chaining.</returns>
        /// <seealso cref="DiagnosticInfo"/>
        /// <seealso cref="ToDetailedString"/>
        public CurlException WithDiagnostics(string diagnosticInfo)
        {
            DiagnosticInfo = diagnosticInfo;
            return this;
        }

        /// <summary>
        /// Returns a multi-line report for logs: type, what happened, documentation link, command, exit code and name,
        /// context, how-to-fix list, diagnostics, inner exception and stack trace.
        /// </summary>
        /// <remarks>
        /// <para>Use it where a human will read the log later; use <see cref="ToJson"/> for structured logging and
        /// <see cref="ToUserFriendlyMessage"/> for end users. The command is printed as-is, so mask secrets first if the log
        /// is shared. <see cref="CurlHttpException"/> adds the status, headers and body.</para>
        /// <para>Example: <c>catch (CurlException ex) { logger.LogError(ex.ToDetailedString()); }</c></para>
        /// </remarks>
        /// <returns>A string that starts with <c>=== CURL EXCEPTION DETAILS ===</c> and has one labelled line per fact.</returns>
        /// <seealso cref="ToJson"/>
        /// <seealso cref="ToUserFriendlyMessage"/>
        /// <seealso cref="Log"/>
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
        /// Serializes the exception to indented JSON (type, message, whatHappened, documentationUrl, command, exit code and
        /// name, context, suggestions, diagnostics, retryability, stack trace, inner message) for structured logs.
        /// </summary>
        /// <remarks>
        /// <para>Uses System.Text.Json, or Newtonsoft.Json on the .NET Standard 2.0 build. Values in <see cref="Context"/> must
        /// be serializable. Example: <c>catch (CurlException ex) { telemetry.TrackTrace(ex.ToJson()); }</c></para>
        /// </remarks>
        /// <returns>An indented JSON object such as <c>{ "type": "CurlOperationTimeoutException", "curlErrorCode": 28, ... }</c>.</returns>
        /// <seealso cref="GetDiagnosticInfo"/>
        /// <seealso cref="ToDetailedString"/>
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
        /// Returns the facts about this error as a dictionary (type, message, command, exit code and name, timestamp,
        /// machine, OS, retryability, context, suggestions, inner exception) for logging scopes and telemetry.
        /// </summary>
        /// <remarks>
        /// <para>Missing values are reported as <c>"N/A"</c>. Example:
        /// <c>using (logger.BeginScope(ex.GetDiagnosticInfo())) logger.LogError(ex.WhatHappened);</c></para>
        /// </remarks>
        /// <returns>A new dictionary with keys such as <c>ExceptionType</c>, <c>CurlErrorCode</c>, <c>CurlErrorName</c>,
        /// <c>IsRetryable</c> and <c>Suggestions</c>.</returns>
        /// <seealso cref="ToJson"/>
        /// <seealso cref="Log"/>
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
        /// Returns the <c>CURLE_*</c> constant name for <see cref="CurlErrorCode"/>, such as <c>CURLE_COULDNT_RESOLVE_HOST</c>
        /// for 6, so logs can be searched with the same names as libcurl's documentation.
        /// </summary>
        /// <remarks>
        /// <para>Unknown codes are reported as <c>CURLE_UNKNOWN_n</c>; null when there is no exit code. Example:
        /// <c>Console.WriteLine($"{ex.CurlErrorCode} {ex.GetCurlErrorName()}");</c> prints <c>28 CURLE_OPERATION_TIMEDOUT</c>.</para>
        /// <para>Meaning of each code: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</para>
        /// </remarks>
        /// <returns>The constant name, or null when <see cref="CurlErrorCode"/> is null.</returns>
        /// <seealso cref="CurlErrorCode"/>
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
        /// Tells whether running the same command again might succeed: true for transient network failures (DNS, refused
        /// or reset connections, timeouts, empty replies), false for problems that need a change first (bad URL, TLS, auth).
        /// </summary>
        /// <remarks>
        /// <para>Retryable exit codes: 6, 7, 28, 52, 55, 56. <see cref="CurlHttpException"/> also treats 429 and 5xx as
        /// retryable. Use it to decide on a retry loop with backoff, or use curl's own <c>--retry 3</c> in the command.</para>
        /// <para>Example: <c>catch (CurlException ex) when (ex.IsRetryable() &amp;&amp; attempt &lt; 3) { await Task.Delay(1000 * attempt); }</c></para>
        /// </remarks>
        /// <returns>True when a retry may help; false when the command or the environment must change first.</returns>
        /// <seealso cref="CurlErrorCode"/>
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
        /// Sends this exception to your logging callback as two values: the human-readable <see cref="ToDetailedString"/>
        /// text and the structured <see cref="GetDiagnosticInfo"/> dictionary.
        /// </summary>
        /// <remarks>
        /// <para>Example with Microsoft.Extensions.Logging:
        /// <c>ex.Log((text, data) =&gt; { using (logger.BeginScope(data)) logger.LogError(text); });</c></para>
        /// </remarks>
        /// <param name="logger">Callback receiving the detailed text and the diagnostic dictionary; must not be null.</param>
        /// <seealso cref="ToDetailedString"/>
        /// <seealso cref="GetDiagnosticInfo"/>
        public void Log(Action<string, Dictionary<string, object>> logger)
        {
            logger(ToDetailedString(), GetDiagnosticInfo());
        }

        /// <summary>
        /// Returns short text for end users: <c>Error:</c> what happened, a bullet list of possible solutions, a note when a
        /// retry may help, and the documentation link, without stack traces or the command.
        /// </summary>
        /// <remarks>
        /// <para>Suitable for a dialog or a CLI tool's error output. Example: <c>catch (CurlException ex) { Console.Error.WriteLine(ex.ToUserFriendlyMessage()); return ex.CurlErrorCode ?? 1; }</c></para>
        /// </remarks>
        /// <returns>Several lines: <c>Error: ...</c>, <c>Possible solutions:</c> with <c>•</c> bullets, optionally
        /// <c>This error might be temporary</c>, and <c>More help: https://...</c>.</returns>
        /// <seealso cref="Suggestions"/>
        /// <seealso cref="WhatHappened"/>
        /// <seealso cref="ToDetailedString"/>
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
    /// Thrown before anything is sent when the curl command string cannot be understood: no URL, an unclosed
    /// quote, a missing option value, or syntax from a different shell than the one CurlDotNet was told to read.
    /// </summary>
    /// <remarks>
    /// <para>Raised by the command parser (<c>Curl.ExecuteAsync</c>, <c>Curl.Execute</c>, <c>Curl.Validate</c>) and by the
    /// shell-flavor readers (bash, cmd, PowerShell, Invoke-WebRequest). Nothing has touched the network, so this is
    /// always a problem in the command text, never in the server. <see cref="InvalidPart"/> names the piece that
    /// could not be read. Derived types: <see cref="CurlUnknownOptionException"/> (an option curl does not have,
    /// exit code 48 in curl) and <see cref="CurlOptionSyntaxException"/> (an option value curl cannot parse, exit code 49).</para>
    /// <para>Usual fixes: start with the URL or <c>curl URL</c>; quote values with spaces (<c>-H 'X-Name: a b'</c>);
    /// close every quote; when the command was copied from Windows cmd or PowerShell, pass the matching
    /// <see cref="CurlDotNet.CurlFlavor"/> (or <c>CurlFlavor.Auto</c>).</para>
    /// <para>Example: <c>try { await Curl.ExecuteAsync("curl -X POST"); } catch (CurlInvalidCommandException ex) { Console.WriteLine($"Fix the command near '{ex.InvalidPart}': {ex.Suggestions[0]}"); }</c>.
    /// Call <c>Curl.Validate(command)</c> first to check user-supplied commands without throwing.</para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md and
    /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     var result = await Curl.ExecuteAsync("curl -X POST");   // no URL
    /// }
    /// catch (CurlInvalidCommandException ex)
    /// {
    ///     Console.WriteLine(ex.WhatHappened);
    ///     Console.WriteLine($"Problem with: {ex.InvalidPart}");
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="CurlUnknownOptionException"/>
    /// <seealso cref="CurlOptionSyntaxException"/>
    /// <seealso cref="CurlMalformedUrlException"/>
    [Serializable]
    public class CurlInvalidCommandException : CurlException
    {
        /// <summary>
        /// The piece of the command that could not be read: an option name such as <c>--frobnicate</c>, an option
        /// whose value is missing, or the unclosed quoted text.
        /// </summary>
        /// <value>The offending token exactly as it appeared in the command, or null when the problem is the
        /// command as a whole (for example "no URL specified").</value>
        /// <remarks>
        /// <para>Use it to highlight the error in an editor or form: <c>commandBox.Select(cmd.IndexOf(ex.InvalidPart), ex.InvalidPart.Length);</c></para>
        /// </remarks>
        /// <seealso cref="CurlException.Command"/>
        public string InvalidPart { get; }

        /// <summary>
        /// Creates the exception for a command that cannot be parsed, naming the part that is wrong so the caller
        /// can point at it.
        /// </summary>
        /// <remarks>
        /// <para>The exit code stays null because the command never ran. When <paramref name="message"/> carries no fix, the
        /// suggestion tells the reader to correct <paramref name="invalidPart"/> and to quote values; the link points at
        /// the error reference.</para>
        /// <para>Example: <c>throw new CurlInvalidCommandException("Option -H needs a value.", "-H", command);</c></para>
        /// </remarks>
        /// <param name="message">What is wrong with the command, optionally followed by fix lines and a <c>See URL</c> line.</param>
        /// <param name="invalidPart">The token that could not be read (an option or quoted text), or null for the whole command.</param>
        /// <param name="command">The full command text that failed to parse, or null.</param>
        /// <seealso cref="CurlUnknownOptionException"/>
        public CurlInvalidCommandException(string message, string invalidPart = null, string command = null)
            : base(message, command)
        {
            InvalidPart = invalidPart;
        }

        /// <summary>
        /// Creates the exception with curl's exit code for the problem, for derived types such as
        /// <see cref="CurlOptionSyntaxException"/> (49) that curl reports with a specific code.
        /// </summary>
        /// <remarks>
        /// <para>Used by derived types; <see cref="CurlException.CurlErrorCode"/> is set to <paramref name="curlErrorCode"/>.</para>
        /// </remarks>
        /// <param name="message">What is wrong with the command and how to fix it, separated by newlines.</param>
        /// <param name="curlErrorCode">The curl exit code, for example 49 for an option value curl cannot parse.</param>
        /// <param name="invalidPart">The specific option or value that is invalid, or null.</param>
        /// <param name="command">The full curl command that failed, or null.</param>
        /// <seealso cref="CurlOptionSyntaxException"/>
        protected CurlInvalidCommandException(string message, int curlErrorCode, string invalidPart, string command)
            : base(message, curlErrorCode, command)
        {
            InvalidPart = invalidPart;
        }

        /// <summary>
        /// Restores the exception, including <see cref="InvalidPart"/>, from serialized data on .NET Framework.
        /// </summary>
        /// <remarks><para>Called by the serializer; not used directly.</para></remarks>
        /// <param name="info">The serialized object data written by <see cref="GetObjectData"/>.</param>
        /// <param name="context">The source of the serialized stream, supplied by the serializer.</param>
        /// <seealso cref="GetObjectData"/>
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

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.ErrorReference;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion => string.IsNullOrEmpty(InvalidPart)
            ? "Write the command as you would type it in a terminal (curl [options] URL), close every quote, and quote values that contain spaces, & or ?."
            : $"Correct or remove '{InvalidPart}' in the command: check its spelling and value against curl's manual, and quote values that contain spaces, & or ?.";
    }

    /// <summary>
    /// Base type for failures to reach the server or proxy at the network level (name resolution, refused or
    /// unreachable connection); catch it to handle "could not connect" in one place.
    /// </summary>
    /// <remarks>
    /// <para>No HTTP exchange happened: the request was never answered. <see cref="Host"/> and <see cref="Port"/> say
    /// what was tried. Derived types: <see cref="CurlDnsException"/> (exit code 6) and <see cref="CurlProxyException"/>
    /// (exit code 5). The transport itself throws the per-exit-code types <see cref="CurlCouldntResolveHostException"/>
    /// (6), <see cref="CurlCouldntResolveProxyException"/> (5) and <see cref="CurlCouldntConnectException"/> (7), which
    /// are not subclasses of this type, so catch those too (or catch <see cref="CurlException"/> and check
    /// <see cref="CurlException.IsRetryable"/>).</para>
    /// <para>Usual causes: a typo in the host, the service is down, the port is wrong, a firewall or VPN blocks the
    /// connection, or a proxy is required. Example: <c>catch (CurlConnectionException ex) { Console.WriteLine($"Cannot reach {ex.Host}:{ex.Port}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     var result = await Curl.ExecuteAsync("curl https://internal-server:8443/api");
    /// }
    /// catch (CurlConnectionException ex)
    /// {
    ///     Console.WriteLine($"Cannot connect to {ex.Host}:{ex.Port} - {ex.Suggestions[0]}");
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="CurlDnsException"/>
    /// <seealso cref="CurlProxyException"/>
    /// <seealso cref="CurlCouldntConnectException"/>
    /// <seealso cref="CurlCouldntResolveHostException"/>
    [Serializable]
    public class CurlConnectionException : CurlException
    {
        /// <summary>
        /// The host name or IP address that could not be reached, exactly as it appears in the URL (or the proxy's
        /// host for <see cref="CurlProxyException"/>).
        /// </summary>
        /// <value>A name such as <c>api.example.com</c> or an address such as <c>192.168.1.10</c>; null when unknown.</value>
        /// <remarks>
        /// <para>Use it for per-host fallback: <c>catch (CurlConnectionException ex) when (ex.Host == primary) { url = backupUrl; }</c></para>
        /// </remarks>
        /// <seealso cref="Port"/>
        public string Host { get; }

        /// <summary>
        /// The TCP port that was tried, or null when the URL used the scheme's default (80 for http, 443 for https,
        /// 21 for ftp).
        /// </summary>
        /// <value>A port number from 1 to 65535, or null for the default port.</value>
        /// <remarks>
        /// <para>Non-standard ports such as 8443 are often blocked by corporate firewalls; try the service on 443 first.</para>
        /// </remarks>
        /// <seealso cref="Host"/>
        public int? Port { get; }

        /// <summary>
        /// Creates a connection failure for <paramref name="host"/> (and <paramref name="port"/>) without a curl exit
        /// code, for code of your own that reports an unreachable endpoint.
        /// </summary>
        /// <remarks>
        /// <para>When <paramref name="message"/> has no fix, the suggestion names the host and port and the link points at
        /// the DNS and connection page. Example: <c>throw new CurlConnectionException($"Health check could not reach {host}.", host, 8443, command);</c></para>
        /// </remarks>
        /// <param name="message">What failed, naming the host; optionally followed by fix lines and a <c>See URL</c> line.</param>
        /// <param name="host">The host name or IP address that could not be reached.</param>
        /// <param name="port">The TCP port that was tried, or null for the scheme's default port.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlCouldntConnectException"/>
        public CurlConnectionException(string message, string host, int? port = null, string command = null)
            : base(message, command)
        {
            Host = host;
            Port = port;
        }

        /// <summary>
        /// Creates a connection failure that carries curl's exit code, for derived types such as
        /// <see cref="CurlDnsException"/> (6) and <see cref="CurlProxyException"/> (5).
        /// </summary>
        /// <remarks><para>Used by derived types to set <see cref="CurlException.CurlErrorCode"/>.</para></remarks>
        /// <param name="message">What failed and how to fix it, separated by newlines.</param>
        /// <param name="curlErrorCode">The curl exit code, for example 6 (could not resolve host).</param>
        /// <param name="host">The host name or IP address that could not be reached.</param>
        /// <param name="port">The TCP port that was tried, or null for the default port.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlDnsException"/>
        protected CurlConnectionException(string message, int curlErrorCode, string host, int? port = null, string command = null)
            : base(message, curlErrorCode, command)
        {
            Host = host;
            Port = port;
        }

        /// <summary>
        /// Restores the exception, including <see cref="Host"/> and <see cref="Port"/>, from serialized data.
        /// </summary>
        /// <remarks><para>Called by the serializer; not used directly.</para></remarks>
        /// <param name="info">The serialized object data written by <see cref="GetObjectData"/>.</param>
        /// <param name="context">The source of the serialized stream, supplied by the serializer.</param>
        /// <seealso cref="GetObjectData"/>
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

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Dns;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Check that {Host ?? "the host"}{(Port.HasValue ? ":" + Port.Value : string.Empty)} is the right host and port, that the service is running, and that no firewall, VPN or required proxy (-x) is in the way.";
    }

    /// <summary>
    /// A host name that could not be turned into an IP address (curl exit code 6, CURLE_COULDNT_RESOLVE_HOST):
    /// the name is misspelled, does not exist, or DNS is unreachable.
    /// </summary>
    /// <remarks>
    /// <para>A <see cref="CurlConnectionException"/> with <see cref="CurlConnectionException.Host"/> set to the name. The
    /// transport reports DNS failures as <see cref="CurlCouldntResolveHostException"/>; this type is for your own code
    /// and older callers that catch it.</para>
    /// <para>Fixes: check the spelling of the host in the URL, try <c>nslookup host</c> on the same machine, check VPN or
    /// proxy settings, or pin the name with <c>--resolve host:443:1.2.3.4</c>.
    /// Example: <c>catch (CurlDnsException ex) { Console.WriteLine($"Unknown host {ex.Host}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     var result = await Curl.ExecuteAsync("curl https://non-existent-domain.invalid");
    /// }
    /// catch (CurlDnsException ex)
    /// {
    ///     Console.WriteLine($"DNS lookup failed for: {ex.Host}");
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="CurlCouldntResolveHostException"/>
    /// <seealso cref="CurlConnectionException"/>
    [Serializable]
    public class CurlDnsException : CurlConnectionException
    {
        /// <summary>
        /// Creates the exception for <paramref name="hostname"/>, with curl's message <c>Could not resolve host: NAME</c>
        /// and exit code 6.
        /// </summary>
        /// <remarks>
        /// <para>The fix line tells the reader to check the spelling and DNS and shows the <c>--resolve</c> workaround for that
        /// name. Example: <c>throw new CurlDnsException("api.exmaple.com", command);</c></para>
        /// </remarks>
        /// <param name="hostname">The host name from the URL that could not be resolved, such as <c>api.example.com</c>.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlCouldntResolveHostException(string, string, string)"/>
        public CurlDnsException(string hostname, string command = null)
            : base($"Could not resolve host: {hostname}", 6, hostname, null, command)
        {
        }

        /// <summary>
        /// Restores the exception from serialized data on .NET Framework (host name and exit code included).
        /// </summary>
        /// <remarks><para>Called by the serializer; not used directly.</para></remarks>
        /// <param name="info">The serialized object data written by GetObjectData.</param>
        /// <param name="context">The source of the serialized stream, supplied by the serializer.</param>
        /// <seealso cref="CurlConnectionException"/>
        protected CurlDnsException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Check the spelling of '{Host}' in the URL and that this machine's DNS (or VPN) can resolve it (try nslookup {Host}); --resolve {Host}:443:ADDRESS pins the name to an address without DNS.";
    }

    /// <summary>
    /// Base type for every timeout (curl exit code 28, CURLE_OPERATION_TIMEDOUT): the connection or the whole
    /// transfer took longer than <c>--connect-timeout</c> or <c>-m/--max-time</c> allowed.
    /// </summary>
    /// <remarks>
    /// <para>CurlDotNet throws the derived <see cref="CurlOperationTimeoutException"/> when <c>-m/--max-time</c> or
    /// <c>--connect-timeout</c> (or <c>Curl.DefaultMaxTimeSeconds</c> / <c>Curl.DefaultConnectTimeoutSeconds</c>)
    /// expires, and when a caller-supplied <c>HttpClient</c> gives up on its own <c>Timeout</c>. Cancelling your own
    /// <c>CancellationToken</c> is not a timeout: it ends with <see cref="CurlAbortedByCallbackException"/> (42).</para>
    /// <para>Fixes: raise the limit (<c>-m 120</c> for large downloads, <c>--connect-timeout 10</c> for slow networks),
    /// check that the server is up, or add <c>--retry 3</c>. <see cref="Timeout"/> tells which limit expired.
    /// Example: <c>catch (CurlTimeoutException ex) { Console.WriteLine($"Gave up after {ex.Timeout?.TotalSeconds} s"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/timeout-errors.md;
    /// tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     var result = await Curl.ExecuteAsync("curl --max-time 5 https://slow-server.com/large-file");
    /// }
    /// catch (CurlTimeoutException ex)
    /// {
    ///     Console.WriteLine($"Timed out after {ex.Timeout?.TotalSeconds ?? 0} seconds; retrying with 30");
    ///     var retry = await Curl.ExecuteAsync("curl --max-time 30 https://slow-server.com/large-file");
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="CurlOperationTimeoutException"/>
    /// <seealso cref="CurlAbortedByCallbackException"/>
    [Serializable]
    public class CurlTimeoutException : CurlException
    {
        /// <summary>
        /// The limit that expired: the <c>-m/--max-time</c> or <c>--connect-timeout</c> value, so you can decide
        /// whether raising it makes sense.
        /// </summary>
        /// <value>The configured limit as a <see cref="TimeSpan"/>; <see cref="TimeSpan.Zero"/> or null when no explicit
        /// limit was set (for example a caller-supplied HttpClient timed out).</value>
        /// <remarks>
        /// <para>Example: <c>var next = (ex.Timeout ?? TimeSpan.FromSeconds(30)).TotalSeconds * 2;</c> then run the command
        /// again with <c>-m {next}</c>.</para>
        /// </remarks>
        /// <seealso cref="CurlException.CurlErrorCode"/>
        public TimeSpan? Timeout { get; }

        /// <summary>
        /// Creates a timeout error (exit code 28) with the limit that expired, for transfers that ran out of time.
        /// </summary>
        /// <remarks>
        /// <para>When <paramref name="message"/> has no fix, the suggestion names the limit and tells the reader to raise
        /// <c>-m/--max-time</c> or <c>--connect-timeout</c>; the link points at the timeout page.
        /// Example: <c>throw new CurlTimeoutException("Upload to backup.example.com timed out after 30 s.", command, TimeSpan.FromSeconds(30));</c></para>
        /// </remarks>
        /// <param name="message">What timed out, naming the host or URL; optionally followed by fix lines and a <c>See URL</c> line.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <param name="timeout">The limit that expired, or null when none was configured.</param>
        /// <seealso cref="CurlOperationTimeoutException(double, string, string)"/>
        public CurlTimeoutException(string message, string command = null, TimeSpan? timeout = null)
            : base(message, 28, command) // CURLE_OPERATION_TIMEDOUT = 28
        {
            Timeout = timeout;
        }

        /// <summary>
        /// Creates a timeout error with a curl exit code other than 28, for timeouts curl reports separately such as
        /// <see cref="CurlFtpAcceptTimeoutException"/> (12, the FTP server never connected back).
        /// </summary>
        /// <remarks><para>Used by derived types to set <see cref="CurlException.CurlErrorCode"/>.</para></remarks>
        /// <param name="message">What timed out and how to fix it, separated by newlines.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <param name="timeout">The limit that expired, or null when none was configured.</param>
        /// <param name="curlErrorCode">The curl exit code, for example 12 (CURLE_FTP_ACCEPT_TIMEOUT).</param>
        /// <seealso cref="CurlFtpAcceptTimeoutException"/>
        protected CurlTimeoutException(string message, string? command, TimeSpan? timeout, int curlErrorCode)
            : base(message, curlErrorCode, command)
        {
            Timeout = timeout;
        }

        /// <summary>
        /// Restores the exception, including <see cref="Timeout"/>, from serialized data on .NET Framework.
        /// </summary>
        /// <remarks><para>Called by the serializer; not used directly.</para></remarks>
        /// <param name="info">The serialized object data written by <see cref="GetObjectData"/>.</param>
        /// <param name="context">The source of the serialized stream, supplied by the serializer.</param>
        /// <seealso cref="GetObjectData"/>
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

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Timeout;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion => Timeout.HasValue && Timeout.Value > TimeSpan.Zero
            ? $"The limit was {Timeout.Value.TotalSeconds.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)} s: raise -m/--max-time (whole transfer) or --connect-timeout (connecting only), add --retry 3, or find out why the server answers so slowly."
            : "No limit was set by the command: set -m/--max-time (seconds) to control it yourself, and check that the server and the network path are responding.";
    }

    /// <summary>
    /// Base type for TLS/SSL failures; this constructor reports exit code 60 (CURLE_PEER_FAILED_VERIFICATION):
    /// the server's certificate is not trusted, has expired, or does not match the host name.
    /// </summary>
    /// <remarks>
    /// <para>Derived types carry curl's specific codes: <see cref="CurlSslConnectErrorException"/> (35, handshake failed),
    /// <see cref="CurlPeerFailedVerificationException"/> (60, certificate rejected), <see cref="CurlSslCertificateProblemException"/>
    /// (58, your client certificate), <see cref="CurlSslCipherException"/> (59), <see cref="CurlUseSslFailedException"/> (64),
    /// <see cref="CurlSslEngineNotFoundException"/> (53) and <see cref="CurlSslEngineSetFailedException"/> (54).
    /// Catch this type to handle them all.</para>
    /// <para>Fixes for an untrusted certificate: install the issuing CA on the machine or pass it with
    /// <c>--cacert ca.pem</c>, fix an expired or mismatched certificate on the server, or (for local testing only) use
    /// <c>-k/--insecure</c>. Example: <c>catch (CurlSslException ex) { Console.WriteLine(ex.CertificateError ?? ex.WhatHappened); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md;
    /// how-to: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlPeerFailedVerificationException"/>
    /// <seealso cref="CurlSslConnectErrorException"/>
    public class CurlSslException : CurlException
    {
        /// <summary>
        /// The TLS layer's own description of the certificate problem, such as <c>RemoteCertificateNameMismatch</c>
        /// or <c>UntrustedRoot</c>, when it was reported.
        /// </summary>
        /// <value>The platform's error text, or null when the failure was not about a certificate.</value>
        /// <remarks>
        /// <para>Example: <c>if (ex.CertificateError?.Contains("NameMismatch") == true) Console.WriteLine("Use the host name the certificate was issued for.");</c></para>
        /// </remarks>
        /// <seealso cref="CurlPeerFailedVerificationException"/>
        public string? CertificateError { get; }

        /// <summary>
        /// Creates a certificate-verification failure (exit code 60) for a server certificate this machine does not
        /// trust, with the TLS layer's detail.
        /// </summary>
        /// <remarks>
        /// <para>When <paramref name="message"/> has no fix, the suggestion explains <c>--cacert</c>, fixing the server and
        /// <c>-k</c> for testing. Example: <c>throw new CurlSslException("Certificate for intranet.local is self-signed.", "UntrustedRoot", command);</c></para>
        /// </remarks>
        /// <param name="message">What was rejected, naming the host; optionally followed by fix lines and a <c>See URL</c> line.</param>
        /// <param name="certError">The TLS layer's description of the certificate problem, or null.</param>
        /// <param name="command">The curl command that failed, or null.</param>
        /// <seealso cref="CurlPeerFailedVerificationException"/>
        public CurlSslException(string message, string? certError = null, string? command = null)
            : base(message, 60, command) // CURLE_PEER_FAILED_VERIFICATION = 60
        {
            CertificateError = certError;
        }

        /// <summary>
        /// Creates a TLS failure with a specific curl exit code, such as 35 (CURLE_SSL_CONNECT_ERROR) for a handshake
        /// that failed for a reason other than certificate verification.
        /// </summary>
        /// <remarks><para>Used by the derived TLS types to set <see cref="CurlException.CurlErrorCode"/>.</para></remarks>
        /// <param name="message">What happened and how to fix it, separated by newlines.</param>
        /// <param name="certError">Details about the certificate error, if any, or null.</param>
        /// <param name="command">The curl command that failed, or null.</param>
        /// <param name="curlErrorCode">The curl exit code, for example 35, 53, 54, 58, 59, 60 or 64.</param>
        /// <seealso cref="CurlSslConnectErrorException"/>
        protected CurlSslException(string message, string? certError, string? command, int curlErrorCode)
            : base(message, curlErrorCode, command)
        {
            CertificateError = certError;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Ssl;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "Make the server certificate trusted: install the issuing CA on this machine or pass it with --cacert ca.pem, renew an expired certificate, use the host name the certificate was issued for, or use -k/--insecure for local testing only.";
    }

    /// <summary>
    /// The server rejected the login (curl exit code 67, CURLE_LOGIN_DENIED): the user name or password is wrong,
    /// or the account may not use the requested authentication method.
    /// </summary>
    /// <remarks>
    /// <para>For HTTP a wrong password normally comes back as a 401 <c>CurlResult</c> (or
    /// <see cref="CurlHttpReturnedErrorException"/> with <c>-f</c>), not as this exception; this type is used for
    /// protocols with an explicit login step and by <see cref="CurlLoginDeniedException"/>. Challenge negotiation
    /// failures (Digest, NTLM) are <see cref="CurlAuthErrorException"/> (94).</para>
    /// <para>Fixes: check <c>-u user:password</c> (quote it if the password has special characters), the account's
    /// permissions, and the scheme (<c>--basic</c>, <c>--digest</c>, <c>--ntlm</c>).
    /// Example: <c>catch (CurlAuthenticationException ex) { Console.WriteLine($"Login failed ({ex.AuthMethod ?? "any method"})"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/auth-errors.md;
    /// how-to: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-basic-auth-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlLoginDeniedException"/>
    /// <seealso cref="CurlAuthErrorException"/>
    public class CurlAuthenticationException : CurlException
    {
        /// <summary>
        /// The authentication scheme that was rejected, such as <c>Basic</c>, <c>Digest</c>, <c>NTLM</c> or an FTP
        /// <c>USER/PASS</c> login, when known.
        /// </summary>
        /// <value>The scheme name as reported by the code that threw, or null when it is not known.</value>
        /// <remarks>
        /// <para>Example: <c>if (ex.AuthMethod == "Basic") Console.WriteLine("Try --digest; the server may not accept Basic.");</c></para>
        /// </remarks>
        /// <seealso cref="CurlAuthErrorException.Scheme"/>
        public string AuthMethod { get; }

        /// <summary>
        /// Creates a login-denied error (exit code 67) naming the authentication method that was refused.
        /// </summary>
        /// <remarks>
        /// <para>When <paramref name="message"/> has no fix, the suggestion tells the reader to check <c>-u user:password</c>
        /// and the scheme. Example: <c>throw new CurlAuthenticationException("ftp.example.com refused user 'bob'.", "USER/PASS", command);</c></para>
        /// </remarks>
        /// <param name="message">What was refused, naming the server and the user (never the password).</param>
        /// <param name="authMethod">The scheme that failed, such as <c>Basic</c> or <c>Digest</c>, or null.</param>
        /// <param name="command">The curl command that caused the error, or null.</param>
        /// <seealso cref="CurlLoginDeniedException"/>
        public CurlAuthenticationException(string message, string authMethod = null, string command = null)
            : base(message, 67, command) // CURLE_LOGIN_DENIED = 67
        {
            AuthMethod = authMethod;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Auth;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Check the user name and password given with -u user:password (quote it when the password contains special characters){(string.IsNullOrEmpty(AuthMethod) ? string.Empty : $" and that the server accepts {AuthMethod} authentication")}; try --basic, --digest or --ntlm if the server expects another scheme.";
    }

    /// <summary>
    /// An HTTP error response turned into an exception: holds the status code, reason phrase, response body and
    /// headers, plus status-specific advice (401: check the token, 429: back off, 5xx: retry later).
    /// </summary>
    /// <remarks>
    /// <para>CurlDotNet returns 4xx/5xx responses as a normal <c>CurlResult</c>. It throws the derived
    /// <see cref="CurlHttpReturnedErrorException"/> (exit code 22) when the command uses <c>-f</c>/<c>--fail</c> or
    /// <c>--fail-with-body</c>; <see cref="CurlRateLimitException"/> is the 429 variant. Construct it yourself to turn a
    /// result into an exception: <c>if (result.StatusCode &gt;= 400) throw new CurlHttpException($"GET {url} failed", result.StatusCode, null, result.Body);</c></para>
    /// <para>Use <see cref="IsClientError"/>/<see cref="IsServerError"/> and the <c>Is*</c> shortcuts to branch, and
    /// <see cref="GetRetryAfter"/> to honour the server's back-off. Example:
    /// <c>catch (CurlHttpException ex) when (ex.IsUnauthorized) { await RefreshTokenAsync(); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md;
    /// tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlHttpReturnedErrorException"/>
    /// <seealso cref="CurlRateLimitException"/>
    public class CurlHttpException : CurlException
    {
        /// <summary>
        /// The HTTP status code the server answered with, such as 404 or 503, so you can branch on it without
        /// parsing the message.
        /// </summary>
        /// <value>The numeric status from the status line, normally 400 to 599.</value>
        /// <remarks>
        /// <para>Example: <c>switch (ex.StatusCode) { case 404: return null; case 409: return await MergeAsync(); default: throw; }</c></para>
        /// </remarks>
        /// <seealso cref="IsStatus(int)"/>
        /// <seealso cref="StatusText"/>
        public int StatusCode { get; }

        /// <summary>
        /// The reason phrase from the response's status line, such as <c>Not Found</c> or <c>Service Unavailable</c>,
        /// when the server sent one.
        /// </summary>
        /// <value>The text after the status code; null or empty when the server sent none (HTTP/2 never does).</value>
        /// <remarks><para>For display only; branch on <see cref="StatusCode"/>, never on this text.</para></remarks>
        /// <seealso cref="StatusCode"/>
        public string StatusText { get; }

        /// <summary>
        /// The body of the error response, which usually holds the server's own explanation (a JSON error object or
        /// an HTML error page).
        /// </summary>
        /// <value>The decoded response text; null when the body was not read (<c>-f</c> without <c>--fail-with-body</c>
        /// discards it) or empty.</value>
        /// <remarks>
        /// <para>Example: <c>var problem = JsonSerializer.Deserialize&lt;ProblemDetails&gt;(ex.ResponseBody ?? "{}");</c>.
        /// Use <c>--fail-with-body</c> instead of <c>-f</c> to keep the body.</para>
        /// </remarks>
        /// <seealso cref="ResponseHeaders"/>
        public string ResponseBody { get; }

        /// <summary>
        /// The response headers of the error response (for example <c>Retry-After</c>, <c>WWW-Authenticate</c>,
        /// <c>X-Request-Id</c>), keyed by header name.
        /// </summary>
        /// <value>A dictionary of header name to value; empty when headers were not attached.</value>
        /// <remarks>
        /// <para>Example: <c>ex.ResponseHeaders.TryGetValue("X-Request-Id", out var id)</c> gives the id to quote to the API
        /// provider's support. <see cref="GetRetryAfter"/> reads <c>Retry-After</c> for you.</para>
        /// </remarks>
        /// <seealso cref="GetRetryAfter"/>
        /// <seealso cref="WithHeaders(Dictionary{string, string})"/>
        public Dictionary<string, string> ResponseHeaders { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Creates an HTTP error from a status code and the response, for code that turns a <c>CurlResult</c> with a
        /// 4xx/5xx status into an exception.
        /// </summary>
        /// <remarks>
        /// <para><see cref="CurlException.CurlErrorCode"/> stays null (curl only reports 22 with <c>-f</c>). When
        /// <paramref name="message"/> carries no fix, the advice for <paramref name="statusCode"/> is used (401: check the
        /// credentials, 404: check the URL, 429: wait, 5xx: retry later) and all advice lines go into
        /// <see cref="CurlException.Suggestions"/>.</para>
        /// <para>Example: <c>throw new CurlHttpException($"POST {url} returned {r.StatusCode}", r.StatusCode, null, r.Body, r.Command);</c></para>
        /// </remarks>
        /// <param name="message">What failed, naming the method and URL; optionally followed by fix lines and a <c>See URL</c> line.</param>
        /// <param name="statusCode">The HTTP status code of the response, such as 404.</param>
        /// <param name="statusText">The reason phrase from the status line, such as <c>Not Found</c>, or null.</param>
        /// <param name="responseBody">The response body text, or null when it was not read.</param>
        /// <param name="command">The curl command that caused the error, or null.</param>
        /// <seealso cref="CurlHttpReturnedErrorException"/>
        public CurlHttpException(string message, int statusCode, string statusText = null, string responseBody = null, string command = null)
            : base(message, command)
        {
            StatusCode = statusCode;
            StatusText = statusText;
            ResponseBody = responseBody;
        }

        /// <summary>
        /// Creates an HTTP error that also carries a curl exit code, for HTTP failures that end a command (such as
        /// <c>-f</c>/<c>--fail</c>, exit code 22) rather than being returned as a result.
        /// </summary>
        /// <remarks><para>Used by <see cref="CurlHttpReturnedErrorException"/>.</para></remarks>
        /// <param name="message">What happened and how to fix it, separated by newlines.</param>
        /// <param name="statusCode">The HTTP status code of the response, such as 404.</param>
        /// <param name="statusText">The reason phrase from the status line, or null.</param>
        /// <param name="responseBody">The response body, when it was read; otherwise null.</param>
        /// <param name="command">The curl command that caused the error, or null.</param>
        /// <param name="curlErrorCode">The curl exit code, for example 22 (CURLE_HTTP_RETURNED_ERROR).</param>
        /// <seealso cref="CurlHttpReturnedErrorException"/>
        protected CurlHttpException(string message, int statusCode, string? statusText, string? responseBody, string? command, int curlErrorCode)
            : base(message, curlErrorCode, command)
        {
            StatusCode = statusCode;
            StatusText = statusText;
            ResponseBody = responseBody;
        }

        /// <summary>
        /// True for 4xx statuses (400 to 499): the request itself is wrong (URL, credentials, body) and repeating it
        /// unchanged will fail again, except for 408 and 429.
        /// </summary>
        /// <value>True when <see cref="StatusCode"/> is between 400 and 499.</value>
        /// <remarks><para>Example: <c>if (ex.IsClientError) log.LogWarning("Fix the request: {Body}", ex.ResponseBody);</c></para></remarks>
        /// <seealso cref="IsServerError"/>
        public bool IsClientError => StatusCode >= 400 && StatusCode < 500;

        /// <summary>
        /// True for 5xx statuses (500 to 599): the server failed; the same request may work later, so it is a
        /// candidate for a retry with backoff.
        /// </summary>
        /// <value>True when <see cref="StatusCode"/> is between 500 and 599.</value>
        /// <remarks><para>Example: <c>catch (CurlHttpException ex) when (ex.IsServerError) { await Task.Delay(2000); /* retry */ }</c></para></remarks>
        /// <seealso cref="IsClientError"/>
        /// <seealso cref="IsRetryable"/>
        public bool IsServerError => StatusCode >= 500 && StatusCode < 600;

        /// <summary>
        /// Checks for one exact status code, for <c>when</c> filters such as <c>catch (CurlHttpException ex) when (ex.IsStatus(409))</c>.
        /// </summary>
        /// <remarks><para>Same as <c>ex.StatusCode == code</c>; reads better in exception filters.</para></remarks>
        /// <param name="code">The HTTP status code to compare with, such as 404 or 409.</param>
        /// <returns>True when <see cref="StatusCode"/> equals <paramref name="code"/>.</returns>
        /// <seealso cref="StatusCode"/>
        public bool IsStatus(int code) => StatusCode == code;

        /// <summary>
        /// True for 401 Unauthorized: the credentials are missing, wrong or expired (refresh the token or check
        /// <c>-u</c>/<c>-H 'Authorization: ...'</c>).
        /// </summary>
        /// <value>True when <see cref="StatusCode"/> is 401.</value>
        /// <remarks><para>See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-bearer-token-csharp/README.md</para></remarks>
        /// <seealso cref="IsForbidden"/>
        public bool IsUnauthorized => StatusCode == 401;

        /// <summary>
        /// True for 403 Forbidden: the server knows who you are but this account or token may not do this (missing
        /// scope, role or IP allow-list).
        /// </summary>
        /// <value>True when <see cref="StatusCode"/> is 403.</value>
        /// <remarks><para>Re-sending the same credentials will not help; request the permission or use another account.</para></remarks>
        /// <seealso cref="IsUnauthorized"/>
        public bool IsForbidden => StatusCode == 403;

        /// <summary>
        /// True for 404 Not Found: the URL path (or the resource id in it) does not exist on this server.
        /// </summary>
        /// <value>True when <see cref="StatusCode"/> is 404.</value>
        /// <remarks><para>Check the path, the API version prefix and the id; many APIs also answer 404 for resources you may not see.</para></remarks>
        /// <seealso cref="IsStatus(int)"/>
        public bool IsNotFound => StatusCode == 404;

        /// <summary>
        /// True for 429 Too Many Requests: you are being rate limited; wait (see <see cref="GetRetryAfter"/>) before
        /// sending more requests.
        /// </summary>
        /// <value>True when <see cref="StatusCode"/> is 429.</value>
        /// <remarks><para>Example: <c>catch (CurlHttpException ex) when (ex.IsRateLimited) { await Task.Delay(ex.GetRetryAfter() ?? TimeSpan.FromSeconds(30)); }</c></para></remarks>
        /// <seealso cref="GetRetryAfter"/>
        /// <seealso cref="CurlRateLimitException"/>
        public bool IsRateLimited => StatusCode == 429;

        /// <summary>
        /// Attaches the response headers to <see cref="ResponseHeaders"/> and returns the same exception, for code that
        /// builds the exception from a result.
        /// </summary>
        /// <remarks><para>Example: <c>throw new CurlHttpException("GET failed", r.StatusCode, null, r.Body).WithHeaders(r.Headers);</c></para></remarks>
        /// <param name="headers">The response headers, header name to value; replaces the current dictionary.</param>
        /// <returns>This same exception instance, for chaining.</returns>
        /// <seealso cref="ResponseHeaders"/>
        public CurlHttpException WithHeaders(Dictionary<string, string> headers)
        {
            ResponseHeaders = headers;
            return this;
        }

        /// <summary>
        /// Reads the server's <c>Retry-After</c> header (in seconds) so you know how long to wait before the next
        /// request after a 429 or 503.
        /// </summary>
        /// <remarks>
        /// <para>Only the delay-seconds form (<c>Retry-After: 120</c>) is understood; an HTTP-date value returns null.
        /// Example: <c>await Task.Delay(ex.GetRetryAfter() ?? TimeSpan.FromSeconds(10));</c></para>
        /// </remarks>
        /// <returns>The delay, or null when the header is missing or not a number of seconds.</returns>
        /// <seealso cref="IsRateLimited"/>
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

        /// <summary>The advice for a status code, most useful first.</summary>
        internal static string[] StatusAdvice(int status)
        {
            switch (status)
            {
                case 400: return new[] { "The server rejected the request as malformed (400): check the request body, its Content-Type header and the query parameters against the API's documentation; ResponseBody usually says which field is wrong.", "Ensure all required fields are included." };
                case 401: return new[] { "The request was not authenticated (401): check the credentials (-u user:password, or -H 'Authorization: Bearer TOKEN'), and renew the token if it has expired.", "Ensure your API key or token is valid and sent in the header the API expects." };
                case 403: return new[] { "The credentials were accepted but may not do this (403): check the account's permissions or the token's scopes, or ask the API owner for access.", "Verify your API key has the required scopes." };
                case 404: return new[] { "The URL was not found (404): check the path, the API version prefix and any id in the URL; some APIs also answer 404 for resources you may not see.", "Check that the resource exists." };
                case 405: return new[] { "The method is not allowed for this URL (405): check -X (GET, POST, PUT, DELETE) against the API's documentation; the Allow response header lists the allowed methods." };
                case 408: return new[] { "The server timed out waiting for the request (408): retry, and send the body faster or in smaller pieces." };
                case 409: return new[] { "The request conflicts with the resource's current state (409): fetch the latest version, merge your change and send it again." };
                case 410: return new[] { "The resource is gone for good (410): stop requesting it and update the URL your code uses." };
                case 413: return new[] { "The request body is too large for the server (413): send a smaller body or upload in parts." };
                case 415: return new[] { "The server does not accept this body format (415): set the Content-Type header the API expects, for example -H 'Content-Type: application/json'." };
                case 422: return new[] { "The request was understood but failed validation (422): ResponseBody lists the fields to correct." };
                case 429: return new[] { "You've hit a rate limit (429) - wait before retrying: honour the Retry-After header (GetRetryAfter()) and slow down.", "Consider implementing exponential backoff, or add --retry 3 to the command." };
                case 500: return new[] { "The server failed while handling the request (500): this is a server error - try again later; if it persists, send the API provider the time and any request id from ResponseHeaders.", "If the problem persists, contact the API provider." };
                case 502: return new[] { "A gateway or proxy got a bad answer from the server behind it (502): this is a server error - try again later.", "If the problem persists, contact the API provider." };
                case 503: return new[] { "The service is unavailable or overloaded (503): this is a server error - try again later, honouring the Retry-After header.", "If the problem persists, contact the API provider." };
                case 504: return new[] { "A gateway timed out waiting for the server behind it (504): retry later, or ask for a smaller result (paging) so the server answers faster." };
            }
            if (status >= 500)
            {
                return new[] { $"The server reported an error ({status}): retry later with backoff, and report it to the API provider if it persists." };
            }
            if (status >= 400)
            {
                return new[] { $"The server refused the request ({status}): read ResponseBody for its explanation and correct the URL, headers or body." };
            }
            return new[] { $"The server answered {status}: read ResponseBody and ResponseHeaders for its explanation." };
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Http;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion => StatusAdvice(StatusCode)[0];

        /// <inheritdoc/>
        protected override IEnumerable<string>? ExtraSuggestions => StatusAdvice(StatusCode);

        /// <summary>
        /// Returns the base detailed report plus the HTTP status line, client/server classification, response body
        /// and response headers, for logs.
        /// </summary>
        /// <remarks>
        /// <para>The body is printed in full; trim it before logging very large error pages. Example:
        /// <c>logger.LogError(ex.ToDetailedString());</c></para>
        /// </remarks>
        /// <returns>The text of <see cref="CurlException.ToDetailedString"/> followed by <c>HTTP Status: 404 Not Found</c>
        /// and the body and header lines.</returns>
        /// <seealso cref="CurlException.ToDetailedString"/>
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
        /// Tells whether the same request may succeed later: true for 429 (rate limited) and every 5xx server error,
        /// false for other 4xx errors, which need the request to change first.
        /// </summary>
        /// <remarks><para>Example: <c>catch (CurlHttpException ex) when (ex.IsRetryable()) { await Task.Delay(ex.GetRetryAfter() ?? TimeSpan.FromSeconds(5)); }</c></para></remarks>
        /// <returns>True for status 429 or 500 and above; otherwise false.</returns>
        /// <seealso cref="GetRetryAfter"/>
        /// <seealso cref="IsServerError"/>
        public override bool IsRetryable()
        {
            // Server errors and rate limits are typically retryable
            return StatusCode == 429 || StatusCode >= 500;
        }
    }

    /// <summary>
    /// A local file could not be read, written, created, deleted, uploaded or downloaded; <see cref="FilePath"/> and
    /// <see cref="Operation"/> say which file and what was being done.
    /// </summary>
    /// <remarks>
    /// <para>CurlDotNet's own transfers report file problems with curl's exit codes instead:
    /// <see cref="CurlWriteErrorException"/> (23) for <c>-o</c>, <c>-D</c>, <c>-c</c>, <c>--trace</c> output and
    /// <see cref="CurlReadErrorException"/> (26) for <c>-d @file</c>, <c>-F name=@file</c>, <c>-T</c> and <c>-K</c>. This
    /// type has no exit code and is meant for application code (for example a download helper) that wants to say
    /// which file failed. Keep the original <c>IOException</c> as the inner exception.</para>
    /// <para>Example: <c>catch (IOException io) { throw new CurlFileException($"Could not save {path}", path, CurlFileException.FileOperation.Write, cmd, io); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md and
    /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlWriteErrorException"/>
    /// <seealso cref="CurlReadErrorException"/>
    public class CurlFileException : CurlException
    {
        /// <summary>
        /// The path of the file that failed, as it was given (relative paths are relative to the process's current
        /// directory).
        /// </summary>
        /// <value>A file path such as <c>downloads/report.pdf</c>; null when not known.</value>
        /// <remarks><para>Use <c>Path.GetFullPath(ex.FilePath)</c> to see where the process actually looked.</para></remarks>
        /// <seealso cref="Operation"/>
        public string FilePath { get; }

        /// <summary>
        /// What was being done with <see cref="FilePath"/> when it failed (read, write, create, delete, upload or
        /// download), so you can word the fix.
        /// </summary>
        /// <value>One of the <see cref="FileOperation"/> values.</value>
        /// <remarks><para>Example: <c>if (ex.Operation == CurlFileException.FileOperation.Write) Console.WriteLine("Is the disk full?");</c></para></remarks>
        /// <seealso cref="FilePath"/>
        public FileOperation Operation { get; }

        /// <summary>
        /// The kind of file access that failed, used by <see cref="Operation"/> to choose the advice in the message.
        /// </summary>
        /// <remarks><para>Read/Upload point at the upload guide, the others at the download guide.</para></remarks>
        /// <seealso cref="Operation"/>
        public enum FileOperation
        {
            /// <summary>
            /// Reading an existing file (for example a request body for <c>-d @file</c>) failed: missing file,
            /// no read permission, or the file is locked.
            /// </summary>
            /// <remarks><para>Check the path and the permissions of the current user.</para></remarks>
            /// <seealso cref="Upload"/>
            Read,
            /// <summary>
            /// Writing to a file (for example a response saved with <c>-o</c>) failed: no write permission, disk
            /// full, or the folder does not exist.
            /// </summary>
            /// <remarks><para>Check the folder exists (or use <c>--create-dirs</c>) and the disk has space.</para></remarks>
            /// <seealso cref="Create"/>
            Write,
            /// <summary>
            /// Creating a new file failed: the folder is missing, read-only, or a file with that name is locked.
            /// </summary>
            /// <remarks><para>Create the folder first or add <c>--create-dirs</c>.</para></remarks>
            /// <seealso cref="Write"/>
            Create,
            /// <summary>
            /// Deleting a file (for example a partial download after <c>--remove-on-error</c>) failed because it is
            /// locked or read-only.
            /// </summary>
            /// <remarks><para>Close programs that hold the file open and check its attributes.</para></remarks>
            /// <seealso cref="Write"/>
            Delete,
            /// <summary>
            /// Reading a file to upload it (<c>-T file</c> or <c>-F name=@file</c>) failed: missing file or no read
            /// permission.
            /// </summary>
            /// <remarks><para>See the upload guide linked from <see cref="CurlFileException"/>.</para></remarks>
            /// <seealso cref="Read"/>
            Upload,
            /// <summary>
            /// Saving a downloaded file (<c>-o file</c> or <c>-O</c>) failed: the folder is missing, not writable, or
            /// the disk is full.
            /// </summary>
            /// <remarks><para>See the download guide linked from <see cref="CurlFileException"/>.</para></remarks>
            /// <seealso cref="Write"/>
            Download
        }

        /// <summary>
        /// Creates a file error naming the path and the operation, keeping the original I/O exception as the inner
        /// exception so its details are not lost.
        /// </summary>
        /// <remarks>
        /// <para>When <paramref name="message"/> has no fix, the advice depends on <paramref name="operation"/> (check that
        /// the file exists for reads, that the folder exists and is writable for writes) and names
        /// <paramref name="filePath"/>; the inner exception's message is added to the first line when the message does
        /// not already contain it.</para>
        /// <para>Example: <c>throw new CurlFileException($"Could not save {path}", path, CurlFileException.FileOperation.Download, cmd, ioException);</c></para>
        /// </remarks>
        /// <param name="message">What failed, naming the file; optionally followed by fix lines and a <c>See URL</c> line.</param>
        /// <param name="filePath">The path of the file that failed, as it was given.</param>
        /// <param name="operation">What was being done with the file: read, write, create, delete, upload or download.</param>
        /// <param name="command">The original curl command that was executed, or null.</param>
        /// <param name="innerException">The underlying I/O exception, kept as InnerException; may be null.</param>
        /// <seealso cref="CurlWriteErrorException"/>
        public CurlFileException(string message, string filePath, FileOperation operation, string command = null, Exception innerException = null)
            : base(message, command, innerException)
        {
            FilePath = filePath;
            Operation = operation;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl =>
            Operation == FileOperation.Read || Operation == FileOperation.Upload ? DocLinks.HowToUpload : DocLinks.HowToDownload;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion
        {
            get
            {
                var path = FilePath ?? "the file";
                switch (Operation)
                {
                    case FileOperation.Read:
                    case FileOperation.Upload:
                        return $"Check that '{path}' exists (relative paths start in {Environment.CurrentDirectory}), that the current user may read it, and that no other program holds it locked.";
                    case FileOperation.Delete:
                        return $"Close any program that holds '{path}' open and check that it is not read-only, then try again.";
                    default:
                        return $"Check that the folder of '{path}' exists (or add --create-dirs), that the current user may write there, and that the disk is not full.";
                }
            }
        }
    }

    /// <summary>
    /// Every attempt of a retried request failed; <see cref="RetryCount"/> says how many were made and
    /// <see cref="LastAttemptException"/> holds the error of the final one.
    /// </summary>
    /// <remarks>
    /// <para>Thrown by the retry middleware (<c>RetryMiddleware</c> in <c>CurlDotNet.Middleware</c>) after its last attempt;
    /// curl's own <c>--retry N</c> instead rethrows the last error with its exit code. The real cause is the inner
    /// exception: fix that (DNS, timeout, 5xx), then decide whether more attempts or a longer delay make sense.</para>
    /// <para>Example: <c>catch (CurlRetryException ex) { Console.WriteLine($"Gave up after {ex.RetryCount} tries: {ex.LastAttemptException?.Message}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlException.IsRetryable"/>
    public class CurlRetryException : CurlException
    {
        /// <summary>
        /// How many retry attempts were made before giving up (not counting the first try, when the middleware is
        /// configured that way), useful for logs and alerts.
        /// </summary>
        /// <value>The configured maximum number of retries that were all used up.</value>
        /// <remarks><para>Example: <c>metrics.Increment("curl.retries_exhausted", ex.RetryCount);</c></para></remarks>
        /// <seealso cref="LastAttemptException"/>
        public int RetryCount { get; }

        /// <summary>
        /// The exception thrown by the final attempt (also <see cref="Exception.InnerException"/>), which tells you why
        /// the request keeps failing.
        /// </summary>
        /// <value>Usually a <see cref="CurlException"/> subclass such as <see cref="CurlOperationTimeoutException"/>; null
        /// when the last attempt returned a failing result instead of throwing.</value>
        /// <remarks><para>Example: <c>if (ex.LastAttemptException is CurlCouldntResolveHostException) /* no point retrying */;</c></para></remarks>
        /// <seealso cref="RetryCount"/>
        public Exception LastAttemptException { get; }

        /// <summary>
        /// Creates the "retries exhausted" error with the number of attempts and the last attempt's exception, which
        /// becomes the inner exception and is summarised in the first line.
        /// </summary>
        /// <remarks>
        /// <para>When <paramref name="message"/> has no fix, the suggestion tells the reader to fix the last error's cause
        /// before adding retries. Example: <c>throw new CurlRetryException($"Failed after {max} retries", command, max, last);</c></para>
        /// </remarks>
        /// <param name="message">What failed, such as <c>Failed after 3 retries</c>; optionally followed by fix lines.</param>
        /// <param name="command">The original curl command that was executed, or null.</param>
        /// <param name="retryCount">The number of retry attempts that were made.</param>
        /// <param name="lastException">The exception from the final attempt, kept as InnerException; may be null.</param>
        /// <seealso cref="LastAttemptException"/>
        public CurlRetryException(string message, string command, int retryCount, Exception lastException)
            : base(message, command, lastException)
        {
            RetryCount = retryCount;
            LastAttemptException = lastException;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl =>
            (LastAttemptException as CurlException)?.DocumentationUrl ?? DocLinks.HowToErrorHandling;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"All {RetryCount} retries failed the same way: fix the cause reported by the last attempt ({LastAttemptException?.GetType().Name ?? "no exception"}) first; more retries or a longer delay only help with temporary outages.";
    }

    /// <summary>
    /// An FTP server refused a command (curl exit code 9, CURLE_REMOTE_ACCESS_DENIED); <see cref="FtpCode"/> holds the
    /// server's reply code, such as 530 (not logged in) or 550 (file unavailable).
    /// </summary>
    /// <remarks>
    /// <para>The FTP handler raises its own <c>CurlDotNet.Core.CurlFtpException</c>; this type is for application code and
    /// callers that catch it. Fixes by reply code: 530 check <c>-u user:password</c>; 550 check the remote path and
    /// permissions; 553 check the file name; 421/425/426 retry or switch between passive and active mode.</para>
    /// <para>Example: <c>catch (CurlFtpException ex) when (ex.FtpCode == 550) { Console.WriteLine("No such remote file"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</para>
    /// </remarks>
    /// <seealso cref="CurlRemoteAccessDeniedException"/>
    public class CurlFtpException : CurlException
    {
        /// <summary>
        /// The three-digit FTP reply code the server answered with, for example 530 (not logged in), 550 (file not
        /// found or no permission) or 421 (service not available).
        /// </summary>
        /// <value>An FTP reply code from 100 to 599.</value>
        /// <remarks><para>4xx codes are temporary (retry), 5xx codes are permanent (change the request).</para></remarks>
        /// <seealso cref="CurlException.CurlErrorCode"/>
        public int FtpCode { get; }

        /// <summary>
        /// Creates an FTP error (exit code 9) with the server's reply code, whose meaning chooses the advice in the
        /// message.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>throw new CurlFtpException("RETR /pub/report.csv was refused", 550, command);</c> produces advice to
        /// check the remote path and permissions.</para>
        /// </remarks>
        /// <param name="message">What the server refused, naming the remote path; optionally followed by fix lines.</param>
        /// <param name="ftpCode">The FTP reply code the server sent, such as 550.</param>
        /// <param name="command">The original curl command that was executed, or null.</param>
        /// <seealso cref="CurlRemoteAccessDeniedException"/>
        public CurlFtpException(string message, int ftpCode, string command = null)
            : base(message, 9, command) // CURLE_REMOTE_ACCESS_DENIED = 9
        {
            FtpCode = ftpCode;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.ErrorReference;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion
        {
            get
            {
                switch (FtpCode)
                {
                    case 530: return "The FTP server did not accept the login (530): check the user name and password given with -u user:password.";
                    case 550: return "The FTP server refused access to the file or folder (550): check the remote path (it is case-sensitive) and the account's permissions.";
                    case 553: return "The FTP server refused the file name (553): use a name without characters the server does not allow, in a folder you may write to.";
                    case 421:
                    case 425:
                    case 426: return $"The FTP data connection failed ({FtpCode}): retry, and switch between passive (default) and active mode (-P -) if a firewall blocks it.";
                    default: return $"The FTP server answered {FtpCode}: check the remote path, the credentials (-u) and the server's log; 4xx codes are temporary and worth a retry.";
                }
            }
        }
    }

    /// <summary>
    /// The proxy could not be used (curl exit code 5, CURLE_COULDNT_RESOLVE_PROXY): its name did not resolve or it
    /// could not be reached; <see cref="ProxyHost"/> and <see cref="ProxyPort"/> say which proxy.
    /// </summary>
    /// <remarks>
    /// <para>The transport reports proxy problems as <see cref="CurlCouldntResolveProxyException"/> (5) and
    /// <see cref="CurlCouldntConnectException"/> with <c>ViaProxy</c> set (7); this type is for application code.
    /// The proxy comes from <c>-x/--proxy</c> or the <c>http_proxy</c>, <c>https_proxy</c> and <c>all_proxy</c>
    /// environment variables.</para>
    /// <para>Fixes: correct the proxy URL (<c>-x http://proxy.corp:3128</c>), check it is running, or bypass it for a host
    /// with <c>--noproxy host</c>. Example: <c>catch (CurlProxyException ex) { Console.WriteLine($"Proxy {ex.ProxyHost}:{ex.ProxyPort} is down"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlCouldntResolveProxyException"/>
    /// <seealso cref="CurlConnectionException"/>
    public class CurlProxyException : CurlConnectionException
    {
        /// <summary>
        /// The proxy's host name or IP address, from <c>-x/--proxy</c> or a proxy environment variable.
        /// </summary>
        /// <value>A host such as <c>proxy.corp.local</c>; also available as <see cref="CurlConnectionException.Host"/>.</value>
        /// <remarks><para>Compare it with <c>Environment.GetEnvironmentVariable("https_proxy")</c> to find out where it came from.</para></remarks>
        /// <seealso cref="ProxyPort"/>
        public string ProxyHost { get; }

        /// <summary>
        /// The proxy's port, such as 3128 or 8080, or null when the proxy URL did not give one (curl then uses 1080).
        /// </summary>
        /// <value>A TCP port number, or null.</value>
        /// <remarks><para>Also available as <see cref="CurlConnectionException.Port"/>.</para></remarks>
        /// <seealso cref="ProxyHost"/>
        public int? ProxyPort { get; }

        /// <summary>
        /// Creates a proxy failure (exit code 5) naming the proxy host and port, with advice to check the proxy setting
        /// or bypass it.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>throw new CurlProxyException("Proxy proxy.corp did not answer.", "proxy.corp", 3128, command);</c></para>
        /// </remarks>
        /// <param name="message">What failed with the proxy; optionally followed by fix lines and a <c>See URL</c> line.</param>
        /// <param name="proxyHost">The proxy host name or IP address.</param>
        /// <param name="proxyPort">The proxy port number, or null when none was given.</param>
        /// <param name="command">The original curl command that was executed, or null.</param>
        /// <seealso cref="CurlCouldntResolveProxyException"/>
        public CurlProxyException(string message, string proxyHost, int? proxyPort = null, string command = null)
            : base(message, 5, proxyHost, proxyPort, command) // CURLE_COULDNT_RESOLVE_PROXY = 5
        {
            ProxyHost = proxyHost;
            ProxyPort = proxyPort;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.HowToProxy;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Check the proxy {ProxyHost}{(ProxyPort.HasValue ? ":" + ProxyPort.Value : string.Empty)} given with -x/--proxy (or the http_proxy/https_proxy/all_proxy environment variables): is the name right and the proxy running? Use --noproxy HOST to connect to a host directly.";
    }

    /// <summary>
    /// A response could not be converted to the type you asked for (for example <c>ParseJson&lt;T&gt;()</c> or
    /// <c>AsJsonDynamic()</c> on an empty body or an HTML error page); the parser's exception is the inner exception.
    /// </summary>
    /// <remarks>
    /// <para>Thrown by <see cref="CurlDotNet.Core.CurlResult"/>'s JSON helpers. It has no curl exit code because the
    /// transfer itself worked. <see cref="ContentType"/> is the response's Content-Type and <see cref="ExpectedType"/>
    /// the type you asked for.</para>
    /// <para>Fixes: check <c>result.StatusCode</c> before parsing (a 401/404/500 page is not your JSON), print
    /// <c>result.Body</c>, send <c>-H 'Accept: application/json'</c>, and make the class match the JSON
    /// (<c>JsonPropertyName</c> attributes for different names).
    /// Example: <c>catch (CurlParsingException ex) { Console.WriteLine($"Got {ex.ContentType}, wanted {ex.ExpectedType?.Name}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlDotNet.Core.CurlResult.ParseJson{T}"/>
    /// <seealso cref="CurlDotNet.Core.CurlResult.AsJsonDynamic"/>
    public class CurlParsingException : CurlException
    {
        /// <summary>
        /// The Content-Type of the response that could not be parsed, such as <c>text/html; charset=utf-8</c>, which
        /// usually explains the failure (an HTML error page instead of JSON).
        /// </summary>
        /// <value>The header value, or null when the response had none.</value>
        /// <remarks><para>Example: <c>if (ex.ContentType?.StartsWith("text/html") == true) Console.WriteLine("The server sent an error page");</c></para></remarks>
        /// <seealso cref="ExpectedType"/>
        public string ContentType { get; }

        /// <summary>
        /// The .NET type the body was supposed to become, such as <c>List&lt;User&gt;</c>; <c>object</c> for
        /// <c>AsJsonDynamic()</c>.
        /// </summary>
        /// <value>The requested type, or null when not known.</value>
        /// <remarks><para>Compare its properties with the JSON in <c>result.Body</c> when the shapes do not match.</para></remarks>
        /// <seealso cref="ContentType"/>
        public Type ExpectedType { get; }

        /// <summary>
        /// Creates a parsing error naming the response's Content-Type and the type you asked for, with the parser's own
        /// exception kept as the inner exception.
        /// </summary>
        /// <remarks>
        /// <para>When <paramref name="message"/> has no fix, the suggestion tells the reader to check the status code and the
        /// body. Example: <c>throw new CurlParsingException("Body of GET /users is not JSON.", result.GetHeader("Content-Type"), typeof(User[]), result.Command, jsonException);</c></para>
        /// </remarks>
        /// <param name="message">What could not be parsed, naming the URL; optionally followed by fix lines and a <c>See URL</c> line.</param>
        /// <param name="contentType">The response's Content-Type header value, or null.</param>
        /// <param name="expectedType">The type the body was supposed to be converted to, or null.</param>
        /// <param name="command">The original curl command that was executed, or null.</param>
        /// <param name="innerException">The JSON parser's exception, kept as InnerException; may be null.</param>
        /// <seealso cref="CurlDotNet.Core.CurlResult.ParseJson{T}"/>
        public CurlParsingException(string message, string contentType, Type expectedType, string command = null, Exception innerException = null)
            : base(message, command, innerException)
        {
            ContentType = contentType;
            ExpectedType = expectedType;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.HowToParseJson;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"The response ({ContentType ?? "no Content-Type"}) could not become {ExpectedType?.Name ?? "the requested type"}: check StatusCode, print Body to see what the server sent, and ask for JSON with -H 'Accept: application/json'.";
    }

    /// <summary>
    /// The server rate limited the request (HTTP 429 Too Many Requests); <see cref="RetryAfter"/> says how long to
    /// wait and <see cref="RemainingLimit"/> how many calls are left, when the server said so.
    /// </summary>
    /// <remarks>
    /// <para>A <see cref="CurlHttpException"/> with <see cref="CurlHttpException.StatusCode"/> 429 and no curl exit code.
    /// CurlDotNet does not throw it on its own (a 429 comes back as a <c>CurlResult</c>, or as
    /// <see cref="CurlHttpReturnedErrorException"/> with <c>-f</c>); create it in your API client from the response's
    /// <c>Retry-After</c> and <c>X-RateLimit-Remaining</c> headers.</para>
    /// <para>Example: <c>catch (CurlRateLimitException ex) { await Task.Delay(ex.RetryAfter ?? TimeSpan.FromSeconds(60)); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlHttpException.IsRateLimited"/>
    /// <seealso cref="CurlHttpException.GetRetryAfter"/>
    public class CurlRateLimitException : CurlHttpException
    {
        /// <summary>
        /// How long the server asked you to wait before the next request (its <c>Retry-After</c> header), or null when it
        /// did not say.
        /// </summary>
        /// <value>The wait time, or null.</value>
        /// <remarks><para>Example: <c>await Task.Delay(ex.RetryAfter ?? TimeSpan.FromSeconds(30));</c></para></remarks>
        /// <seealso cref="RemainingLimit"/>
        public TimeSpan? RetryAfter { get; }

        /// <summary>
        /// How many requests the server still allows in the current window (for example from <c>X-RateLimit-Remaining</c>),
        /// or null when unknown.
        /// </summary>
        /// <value>A non-negative count, usually 0 for a 429, or null.</value>
        /// <remarks><para>Log it to see how close callers run to the limit.</para></remarks>
        /// <seealso cref="RetryAfter"/>
        public int? RemainingLimit { get; }

        /// <summary>
        /// Creates a 429 rate-limit error carrying the server's wait time and remaining quota, which the advice in the
        /// message uses.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>throw new CurlRateLimitException("api.github.com rate limit reached", TimeSpan.FromSeconds(60), 0, command);</c>
        /// gives the advice to wait 60 s.</para>
        /// </remarks>
        /// <param name="message">What was rate limited, naming the API; optionally followed by fix lines.</param>
        /// <param name="retryAfter">The time to wait before retrying (the Retry-After header), or null.</param>
        /// <param name="remainingLimit">The number of requests remaining in the window, or null.</param>
        /// <param name="command">The original curl command that was executed, or null.</param>
        /// <seealso cref="CurlHttpException"/>
        public CurlRateLimitException(string message, TimeSpan? retryAfter = null, int? remainingLimit = null, string command = null)
            : base(message, 429, "Too Many Requests", null, command)
        {
            RetryAfter = retryAfter;
            RemainingLimit = remainingLimit;
        }

        /// <inheritdoc/>
        protected override string? DefaultSuggestion => RetryAfter.HasValue
            ? $"Wait {RetryAfter.Value.TotalSeconds.ToString("0", System.Globalization.CultureInfo.InvariantCulture)} s (the server's Retry-After) before the next request, and slow down or batch calls so you stay under the limit."
            : "Wait before the next request (start with 30 s and back off exponentially), and slow down or batch calls so you stay under the limit.";
    }

    /// <summary>
    /// A command failed for a reason that has no more specific type; the inner exception holds the underlying .NET
    /// error and the message says in words what it was.
    /// </summary>
    /// <remarks>
    /// <para>Use it in wrappers and middleware that run curl commands and need to report an unexpected failure without
    /// losing the original exception. It carries no curl exit code. Prefer a specific type
    /// (<see cref="CurlCouldntConnectException"/>, <see cref="CurlWriteErrorException"/>...) when one fits.</para>
    /// <para>Example: <c>catch (Exception ex) when (!(ex is CurlException)) { throw new CurlExecutionException($"Running '{name}' failed.", command, ex); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</para>
    /// </remarks>
    /// <seealso cref="CurlException"/>
    public class CurlExecutionException : CurlException
    {
        /// <summary>
        /// Creates a general execution failure that wraps the original exception and summarises it in the first line
        /// of the message.
        /// </summary>
        /// <remarks>
        /// <para>When <paramref name="message"/> has no fix, the suggestion tells the reader to look at the inner exception
        /// and to rerun the command with <c>-v</c>. Example: <c>throw new CurlExecutionException("Nightly sync failed.", command, ex);</c></para>
        /// </remarks>
        /// <param name="message">What failed; optionally followed by fix lines and a <c>See URL</c> line.</param>
        /// <param name="command">The original curl command that was executed, or null.</param>
        /// <param name="innerException">The underlying exception that caused the failure, or null.</param>
        /// <seealso cref="CurlException(string, int, string, Exception)"/>
        public CurlExecutionException(string message, string command = null, Exception innerException = null)
            : base(message, command, innerException)
        {
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.ErrorReference;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion => InnerException != null
            ? $"Fix the {InnerException.GetType().Name} reported above (see InnerException), then run the command again with -v or --trace-ascii - to confirm where it fails."
            : "Run the same command with -v (or --trace-ascii -) to see where it fails, and compare it with the native curl tool.";
    }

    /// <summary>
    /// The command uses a curl feature CurlDotNet does not implement (exit code 1); <see cref="Feature"/> names it so
    /// you can remove it or fall back to the native curl tool.
    /// </summary>
    /// <remarks>
    /// <para>For unsupported URL schemes (gopher://, ldap://...) CurlDotNet throws <see cref="CurlUnsupportedProtocolException"/>;
    /// for compiled-out options <see cref="CurlNotBuiltInException"/> (4). This type is for other unsupported features.</para>
    /// <para>Example: <c>catch (CurlNotSupportedException ex) { Console.WriteLine($"{ex.Feature} is not available in CurlDotNet"); }</c></para>
    /// <para>What is supported: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/reference/curl-cli-compatibility.md</para>
    /// </remarks>
    /// <seealso cref="CurlNotBuiltInException"/>
    /// <seealso cref="CurlUnsupportedProtocolException"/>
    public class CurlNotSupportedException : CurlException
    {
        /// <summary>
        /// The curl feature or option that is not available, such as <c>--libcurl</c> or a specific protocol, so you can
        /// remove it from the command.
        /// </summary>
        /// <value>The feature name as given to the constructor.</value>
        /// <remarks><para>Check the compatibility list linked from the type before relying on an option.</para></remarks>
        /// <seealso cref="CurlNotBuiltInException.Feature"/>
        public string Feature { get; }

        /// <summary>
        /// Creates the "not supported" error (exit code 1) for <paramref name="feature"/>, with advice to remove it or use
        /// the native curl tool.
        /// </summary>
        /// <remarks>
        /// <para>Message: <c>Feature not supported: FEATURE</c>, then the fix and a link to the compatibility list.
        /// Example: <c>throw new CurlNotSupportedException("--libcurl", command);</c></para>
        /// </remarks>
        /// <param name="feature">The name of the unsupported feature or option, such as <c>--libcurl</c>.</param>
        /// <param name="command">The original curl command that was executed, or null.</param>
        /// <seealso cref="CurlNotBuiltInException(string, string)"/>
        public CurlNotSupportedException(string feature, string command = null)
            : base($"Feature not supported: {feature}", 1, command) // CURLE_UNSUPPORTED_PROTOCOL = 1
        {
            Feature = feature;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.CliCompatibility;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"CurlDotNet does not implement {Feature}: remove it from the command, or run that command with the native curl tool.";
    }

    /// <summary>
    /// Reading or writing cookies failed: the cookie jar file (<c>-c</c>) could not be saved, the cookie file
    /// (<c>-b file</c>) could not be read, or a cookie could not be parsed; <see cref="CookieJarPath"/> names the file.
    /// </summary>
    /// <remarks>
    /// <para>CurlDotNet reports a cookie jar that cannot be written as <see cref="CurlWriteErrorException"/> (23), like curl;
    /// this type is for application code that manages cookie files. Cookie files use the Netscape format written by
    /// <c>-c</c>; <c>-b</c> also accepts a literal <c>name=value; other=2</c> string.</para>
    /// <para>Example: <c>catch (CurlCookieException ex) { Console.WriteLine($"Cookie file problem: {ex.CookieJarPath}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/cookie-errors.md;
    /// cookbook: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/cookies.md</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     var result = await Curl.ExecuteAsync("curl --cookie-jar cookies.txt https://example.com");
    /// }
    /// catch (CurlCookieException ex)
    /// {
    ///     Console.WriteLine($"Cookie error: {ex.WhatHappened} ({ex.CookieJarPath})");
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="CurlWriteErrorException"/>
    public class CurlCookieException : CurlException
    {
        /// <summary>
        /// The cookie file that could not be read or written, as given to <c>-b</c>/<c>--cookie</c> or
        /// <c>-c</c>/<c>--cookie-jar</c>.
        /// </summary>
        /// <value>A file path, or null when the problem is a cookie string rather than a file.</value>
        /// <remarks><para>Example: <c>File.Exists(ex.CookieJarPath)</c> tells whether the file is missing or unreadable.</para></remarks>
        /// <seealso cref="CurlException.Command"/>
        public string CookieJarPath { get; }

        /// <summary>
        /// Creates a cookie error naming the cookie file, keeping the original I/O or parsing exception as the inner
        /// exception.
        /// </summary>
        /// <remarks>
        /// <para>When <paramref name="message"/> has no fix, the advice names the file and its folder; the link points at the
        /// cookie page. Example: <c>throw new CurlCookieException("Cookie jar could not be saved.", "state/cookies.txt", command, ioException);</c></para>
        /// </remarks>
        /// <param name="message">What failed with the cookies; optionally followed by fix lines and a <c>See URL</c> line.</param>
        /// <param name="cookieJarPath">The path to the cookie file, or null when no file is involved.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <param name="innerException">The underlying exception that caused this error, or null.</param>
        /// <seealso cref="CurlWriteErrorException"/>
        public CurlCookieException(string message, string cookieJarPath = null, string command = null, Exception innerException = null)
            : base(message, command, innerException)
        {
            CookieJarPath = cookieJarPath;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Cookie;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion => string.IsNullOrEmpty(CookieJarPath)
            ? "Check the cookie text given with -b: it must look like name=value; other=2, or be the path of a Netscape cookie file written by -c."
            : $"Check that '{CookieJarPath}' is a Netscape-format cookie file (as written by -c) and that its folder exists and is readable and writable by the current user.";
    }

    /// <summary>
    /// A redirect chain was longer than allowed (curl exit code 47, CURLE_TOO_MANY_REDIRECTS);
    /// <see cref="RedirectCount"/> and <see cref="LastUrl"/> show how far it got.
    /// </summary>
    /// <remarks>
    /// <para>The transport throws <see cref="CurlTooManyRedirectsException"/> for <c>-L</c> chains longer than
    /// <c>--max-redirs</c> (50 by default); this type is for application code that follows redirects itself.
    /// A long chain is usually a loop (A redirects to B, B back to A), often caused by cookies or a login page.</para>
    /// <para>Fixes: inspect the chain with <c>curl -i -L URL</c>, keep cookies with <c>-b cookies.txt -c cookies.txt</c>,
    /// or raise <c>--max-redirs</c>. Example: <c>catch (CurlRedirectException ex) { Console.WriteLine($"Stopped after {ex.RedirectCount} hops at {ex.LastUrl}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/redirect-errors.md;
    /// how-to: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-follow-redirects-csharp/README.md</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// try
    /// {
    ///     var result = await Curl.ExecuteAsync("curl -L https://example.com/redirect-loop");
    /// }
    /// catch (CurlRedirectException ex)
    /// {
    ///     Console.WriteLine($"Too many redirects: {ex.RedirectCount}, last URL {ex.LastUrl}");
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="CurlTooManyRedirectsException"/>
    public class CurlRedirectException : CurlException
    {
        /// <summary>
        /// How many redirects were followed before the limit stopped the chain; a value equal to the limit usually
        /// means a redirect loop.
        /// </summary>
        /// <value>The number of hops followed.</value>
        /// <remarks><para>Example: <c>if (ex.RedirectCount &gt;= 50) Console.WriteLine("Redirect loop - check cookies and login");</c></para></remarks>
        /// <seealso cref="LastUrl"/>
        public int RedirectCount { get; }

        /// <summary>
        /// The last URL in the chain before it was stopped, which shows where the loop or the long chain goes.
        /// </summary>
        /// <value>An absolute URL, or null when not recorded.</value>
        /// <remarks><para>Open it with <c>curl -i</c> (without <c>-L</c>) to see its <c>Location</c> header.</para></remarks>
        /// <seealso cref="RedirectCount"/>
        public string LastUrl { get; }

        /// <summary>
        /// Creates a too-many-redirects error (exit code 47) with the number of hops and the last URL, which the advice in
        /// the message uses.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>throw new CurlRedirectException("Login keeps redirecting.", 20, lastUrl, command);</c></para>
        /// </remarks>
        /// <param name="message">What happened, naming the start URL; optionally followed by fix lines and a <c>See URL</c> line.</param>
        /// <param name="redirectCount">The number of redirects that were followed.</param>
        /// <param name="lastUrl">The last URL in the redirect chain, or null.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlTooManyRedirectsException(int, int, string)"/>
        public CurlRedirectException(string message, int redirectCount, string lastUrl = null, string command = null)
            : base(message, 47, command) // CURLE_TOO_MANY_REDIRECTS = 47
        {
            RedirectCount = redirectCount;
            LastUrl = lastUrl;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Redirect;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"After {RedirectCount} redirect(s){(string.IsNullOrEmpty(LastUrl) ? string.Empty : " ending at " + ErrorText.SafeUrl(LastUrl))}: check the chain with curl -i -L for a loop, keep cookies with -b cookies.txt -c cookies.txt, or raise --max-redirs.";
    }
}
