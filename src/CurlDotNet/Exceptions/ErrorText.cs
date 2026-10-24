/***************************************************************************
 * ErrorText - small helpers for writing exception messages that name the concrete values involved
 * without leaking secrets (passwords in URLs, API keys in query strings, -u user:password).
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Text.RegularExpressions;

namespace CurlDotNet.Exceptions
{
    /// <summary>
    /// Helpers for exception messages: safe URLs, short previews and option lists.
    /// </summary>
    internal static class ErrorText
    {
        private static readonly Regex UrlInCommand = new Regex(@"(?:https?|ftps?|file|scp|sftp)://[^\s'""]+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /// <summary>
        /// The URL without user:password and without the query string, safe to print in a message
        /// (for example <c>https://api.example.com/v1/users</c>). Returns the input when it is not a URL.
        /// </summary>
        public static string SafeUrl(string? url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return "(no URL)";
            }
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri) && !string.IsNullOrEmpty(uri.Host))
            {
                var port = uri.IsDefaultPort ? string.Empty : ":" + uri.Port;
                var query = string.IsNullOrEmpty(uri.Query) ? string.Empty : "?...";
                return $"{uri.Scheme}://{uri.Host}{port}{uri.AbsolutePath}{query}";
            }
            // not parseable: still never print user:password@ or a query string
            var text = UserInfo.Replace(url!, "$1");
            var q = text.IndexOf('?');
            if (q >= 0)
            {
                text = text.Substring(0, q) + "?...";
            }
            return Preview(text, 200);
        }

        private static readonly Regex UserInfo = new Regex(@"^([A-Za-z][A-Za-z0-9+.-]*://)[^/@]*@", RegexOptions.CultureInvariant);

        /// <summary>
        /// The first URL in a curl command, made safe with <see cref="SafeUrl"/>; null when the command has none.
        /// </summary>
        public static string? UrlFromCommand(string? command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return null;
            }
            var match = UrlInCommand.Match(command);
            return match.Success ? SafeUrl(match.Value) : null;
        }

        private static readonly Regex SecretOption = new Regex(
            // -u/-U/--oauth2-bearer/--pass/--proxy-pass VALUE (quoted or not), and -uVALUE written without a space
            @"(?<flag>(?:^|\s)(?:-u|--user|-U|--proxy-user|--oauth2-bearer|--pass|--proxy-pass|--proxy-tlspassword|--tlspassword)(?:\s+|=))(?<value>'[^']*'|""[^""]*""|\S+)" +
            @"|(?<flag>(?:^|\s)-[uU])(?<value>[^\s-]\S*)" +
            // secret header values: Authorization, Proxy-Authorization, Cookie, API keys
            @"|(?<flag>(?:Proxy-)?Authorization\s*:\s*|Cookie\s*:\s*|X-Api-Key\s*:\s*|Api-Key\s*:\s*|X-Amz-Security-Token\s*:\s*)(?<value>[^'""\r\n]+)",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private static readonly Regex CookieString = new Regex(
            // -b/--cookie NAME=VALUE strings (a value without '=' is a cookie file name, which is kept)
            @"(?<flag>(?:^|\s)(?:-b|--cookie)\s+)(?<value>'[^']*=[^']*'|""[^""]*=[^""]*""|[^\s'""]*=\S*)", RegexOptions.CultureInvariant);

        private static readonly Regex CertPassword = new Regex(
            @"(?<flag>(?:^|\s)(?:-E|--cert|--proxy-cert)\s+)(?<value>'[^']*'|""[^""]*""|\S+)", RegexOptions.CultureInvariant);

        private static readonly Regex UrlPassword = new Regex(
            @"(?<prefix>[A-Za-z][A-Za-z0-9+.-]*://[^/\s'""@:]*):[^/\s'""@]*@", RegexOptions.CultureInvariant);

        /// <summary>
        /// The command with every secret replaced by <c>***</c>, safe for logs and messages: the values of <c>-u</c>,
        /// <c>-U</c>, <c>--oauth2-bearer</c>, <c>--pass</c>, <c>-b NAME=VALUE</c> cookie strings, the password part of
        /// <c>--cert FILE:PASSWORD</c>, the values of
        /// <c>Authorization</c>, <c>Proxy-Authorization</c>, <c>Cookie</c> and API-key headers, and the password of every
        /// <c>scheme://user:password@host</c> URL (the user name stays, like <see cref="SafeUrl"/> drops only what is secret).
        /// </summary>
        public static string RedactCommand(string? command)
        {
            if (string.IsNullOrEmpty(command))
            {
                return string.Empty;
            }

            var text = SecretOption.Replace(command!, m => m.Groups["flag"].Value + "***");
            text = CookieString.Replace(text, m => m.Groups["flag"].Value + "***");
            text = CertPassword.Replace(text, m =>
            {
                var value = m.Groups["value"].Value;
                var quote = value.Length > 1 && (value[0] == '\'' || value[0] == '"') ? value[0].ToString() : string.Empty;
                var inner = quote.Length > 0 ? value.Substring(1, value.Length - 2) : value;
                var (file, password) = CurlDotNet.Core.CommandParser.SplitCertArgument(inner);
                return password == null ? m.Value : m.Groups["flag"].Value + quote + file + ":***" + quote;
            });
            return UrlPassword.Replace(text, "${prefix}:***@");
        }

        /// <summary>
        /// A one-line preview of <paramref name="text"/>, cut at <paramref name="max"/> characters with "...".
        /// </summary>
        public static string Preview(string? text, int max = 80)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
            var oneLine = text!.Replace("\r", "\\r").Replace("\n", "\\n");
            return oneLine.Length <= max ? oneLine : oneLine.Substring(0, max) + "...";
        }
    }
}
