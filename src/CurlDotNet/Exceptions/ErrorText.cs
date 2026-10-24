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
            return Preview(url!, 200);
        }

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
