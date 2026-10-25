/***************************************************************************
 * HostAlias - URL host names that System.Uri refuses but curl sends anyway
 *
 * curl puts the host of a URL on the wire as written, even when it is not a valid DNS name
 * (curl test 46: "domain..tld" with --resolve domain..tld:PORT:ADDRESS). System.Uri refuses such a
 * host ("Invalid URI: The hostname could not be parsed"), so HttpClient cannot even build the
 * request. When --resolve or --connect-to says where that host lives, CurlDotNet swaps the host for
 * a reversible alias under the reserved ".invalid" top-level domain (RFC 6761: it never resolves),
 * and every place curl shows or matches the host name maps the alias back:
 *
 *  - the connect callback resolves and --connect-to-maps the original name (CurlTransport),
 *  - the Host header carries the original name (RequestRewrite, on the wire),
 *  - cookies are matched and stored for the original name (CookieJar),
 *  - %{url} and %{url_effective} show the original URL (HttpHandler).
 *
 * The alias is a pure function of the name (hex of its UTF-8 bytes), so no state is kept anywhere
 * and nothing leaks between commands. Only .NET 5+ (which owns the connection and the request head)
 * and only direct connections (a proxy would receive the alias in its absolute-form request) use it.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Globalization;
using System.Text;

namespace CurlDotNet.Core
{
    /// <summary>Reversible stand-in host names for URL hosts that System.Uri cannot parse.</summary>
    internal static class HostAlias
    {
        /// <summary>Every alias ends with this suffix (".invalid" is reserved and never resolves).</summary>
        internal const string Suffix = ".curldotnet-alias.invalid";

        private const int LabelHexLength = 60; // DNS labels are at most 63 characters

        /// <summary>
        /// Replaces the host of <see cref="CurlOptions.Url"/> with its alias when System.Uri refuses the host
        /// but curl would send it: the name only uses letters, digits, '.', '-' and '_', and --resolve or
        /// --connect-to names it (or matches every host), so the connection does not depend on DNS.
        /// Leaves the URL unchanged in every other case (a bad URL then fails with exit code 3 as before).
        /// </summary>
        public static void Apply(CurlOptions options)
        {
            if (!CurlTransport.SupportsRecording || string.IsNullOrEmpty(options.Url)
                || Uri.TryCreate(options.Url, UriKind.Absolute, out _)
                || CurlProxy.IsConfigured(options) || options.ProxyTunnel)
            {
                return;
            }
            if (!TrySplit(options.Url, out var before, out var host, out var after) || !IsSendable(host) || !IsMapped(options, host))
            {
                return;
            }
            var aliased = before + Encode(host) + after;
            if (Uri.TryCreate(aliased, UriKind.Absolute, out _))
            {
                options.Url = aliased;
            }
        }

        /// <summary>The original host name of an alias, or <paramref name="host"/> itself when it is not one.</summary>
        public static string Restore(string host)
        {
            return TryDecode(host, out var original) ? original : host;
        }

        /// <summary>True when <paramref name="host"/> is an alias.</summary>
        public static bool IsAlias(string? host)
        {
            return host != null && host.EndsWith(Suffix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary><paramref name="url"/> with an aliased host written back as the original name.</summary>
        public static string RestoreUrl(string url)
        {
            if (url == null || url.IndexOf(Suffix, StringComparison.OrdinalIgnoreCase) < 0
                || !TrySplit(url, out var before, out var host, out var after) || !TryDecode(host, out var original))
            {
                return url!;
            }
            return before + original + after;
        }

        /// <summary>The alias of <paramref name="host"/>: hex of its UTF-8 bytes in labels of an "x" and up to 60 digits.</summary>
        internal static string Encode(string host)
        {
            var hex = new StringBuilder();
            foreach (var b in Encoding.UTF8.GetBytes(host))
            {
                hex.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            }
            var labels = new StringBuilder();
            for (var i = 0; i < hex.Length; i += LabelHexLength)
            {
                if (labels.Length > 0) labels.Append('.');
                labels.Append('x').Append(hex.ToString(i, Math.Min(LabelHexLength, hex.Length - i)));
            }
            return labels + Suffix;
        }

        private static bool TryDecode(string host, out string original)
        {
            original = host;
            if (!IsAlias(host))
            {
                return false;
            }
            var labels = host.Substring(0, host.Length - Suffix.Length).Split('.');
            var hex = new StringBuilder();
            foreach (var label in labels)
            {
                if (label.Length < 2 || (label[0] != 'x' && label[0] != 'X'))
                {
                    return false;
                }
                hex.Append(label, 1, label.Length - 1);
            }
            if (hex.Length % 2 != 0)
            {
                return false;
            }
            var bytes = new byte[hex.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                if (!byte.TryParse(hex.ToString(i * 2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out bytes[i]))
                {
                    return false;
                }
            }
            original = Encoding.UTF8.GetString(bytes);
            return true;
        }

        /// <summary>
        /// Splits an http(s) URL around its host: <paramref name="before"/> is "scheme://[userinfo@]",
        /// <paramref name="after"/> is ":port/path?query#fragment". IPv6 literals are never split.
        /// </summary>
        private static bool TrySplit(string url, out string before, out string host, out string after)
        {
            before = host = after = string.Empty;
            var scheme = url.IndexOf("://", StringComparison.Ordinal);
            if (scheme <= 0)
            {
                return false;
            }
            var name = url.Substring(0, scheme);
            if (!name.Equals("http", StringComparison.OrdinalIgnoreCase) && !name.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            var start = scheme + 3;
            var end = url.IndexOfAny(new[] { '/', '?', '#' }, start);
            if (end < 0) end = url.Length;
            var at = url.LastIndexOf('@', end - 1, end - start);
            var hostStart = at >= 0 ? at + 1 : start;
            if (hostStart < url.Length && url[hostStart] == '[')
            {
                return false;
            }
            var colon = url.IndexOf(':', hostStart, end - hostStart);
            var hostEnd = colon >= 0 ? colon : end;
            if (hostEnd <= hostStart)
            {
                return false;
            }
            before = url.Substring(0, hostStart);
            host = url.Substring(hostStart, hostEnd - hostStart);
            after = url.Substring(hostEnd);
            return true;
        }

        /// <summary>Letters, digits, '.', '-' and '_' only: a name curl writes on the wire unchanged.</summary>
        private static bool IsSendable(string host)
        {
            foreach (var c in host)
            {
                var ok = (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '.' || c == '-' || c == '_';
                if (!ok)
                {
                    return false;
                }
            }
            return host.Length > 0;
        }

        /// <summary>True when a --resolve entry (or "*") or a --connect-to entry (or an empty HOST1) applies to <paramref name="host"/>.</summary>
        private static bool IsMapped(CurlOptions options, string host)
        {
            foreach (var key in options.Resolve.Keys)
            {
                var colon = key.LastIndexOf(':');
                var name = colon > 0 ? key.Substring(0, colon).TrimStart('+') : key;
                if (name == "*" || string.Equals(name.TrimEnd('.'), host.TrimEnd('.'), StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            foreach (var entry in options.ConnectTo)
            {
                if (CommandParser.TrySplitConnectTo(entry, out var host1, out _, out _, out _)
                    && (host1.Length == 0 || string.Equals(host1, host, StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
