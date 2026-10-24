/***************************************************************************
 * CookieJar - curl's cookie engine (lib/cookie.c) for -b FILE, -c JAR and -j
 *
 * The engine is only switched on when curl's would be: -b with a value that is not a
 * name=value string (a file name, even a missing one), or -c. A plain -b "a=b" string is sent
 * verbatim and nothing is stored (issue #41 behaviour).
 *
 * - Reads Netscape cookie files (with #HttpOnly_ lines) and files of Set-Cookie: headers.
 * - Stores Set-Cookie from every response of the transfer (auth challenges and redirect hops),
 *   applying curl's domain, path, secure, prefix, size and expiry rules.
 * - Sends matching cookies sorted like curl (longest path, longest domain, longest name,
 *   newest first), capped at 150 cookies / 8190 bytes, followed by the -b string.
 * - Writes the jar in Netscape format, newest cookie first, with curl's header comment.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>
    /// One cookie as curl stores it.
    /// </summary>
    internal sealed class StoredCookie
    {
        public string Name = string.Empty;
        public string Value = string.Empty;
        /// <summary>Lower-case domain without a leading dot.</summary>
        public string Domain = string.Empty;
        /// <summary>The domain as the server (or the cookie file) wrote it, used when saving the jar.</summary>
        public string? DomainAsGiven;
        /// <summary>True when the cookie also applies to subdomains (Domain= was given).</summary>
        public bool TailMatch;
        public string Path = "/";
        public bool Secure;
        public bool HttpOnly;
        /// <summary>Expiry as Unix seconds; 0 for a session cookie.</summary>
        public long Expires;
        public long Created;
    }

    /// <summary>
    /// The cookie engine of one transfer.
    /// </summary>
    internal sealed class CookieEngine
    {
        private const int MaxCookieHeader = 8190;
        private const int MaxCookiesSent = 150;
        private const int MaxNameValue = 4096;
        private const int MaxSetCookiesPerResponse = 50;
        private static readonly long MaxAgeCap = 400L * 24 * 3600; // RFC 6265bis: at most 400 days

        private readonly List<StoredCookie> _cookies = new List<StoredCookie>();
        private string? _jarPath;
        private long _counter;

        private CookieEngine(string? jarPath)
        {
            _jarPath = jarPath;
        }

        /// <summary>
        /// Returns the engine when the command enables it (-b FILE or -c JAR), otherwise null so the
        /// -b string is sent as a plain header by the request builder.
        /// </summary>
        public static CookieEngine? Create(CurlOptions options)
        {
            var cookie = options.Cookie;
            var files = new List<string>(options.CookieFiles);
            if (cookie != null && cookie.IndexOf('=') < 0)
            {
                files.Add(cookie);
            }
            if (files.Count == 0 && string.IsNullOrEmpty(options.CookieJar))
            {
                return null;
            }

            // Every transfer of one command line shares one cookie store (curl's share handle), so a
            // cookie set by the first URL is sent to the second; each transfer reads its -b files again.
            var session = options.Session;
            var engine = session?.Cookies ?? new CookieEngine(options.CookieJar);
            if (session != null)
            {
                session.Cookies = engine;
                if (!string.IsNullOrEmpty(options.CookieJar))
                {
                    engine._jarPath = options.CookieJar;
                }
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    engine.Load(file, options.JunkSessionCookies);
                }
            }
            return engine;
        }

        /// <summary>Number of cookies currently stored.</summary>
        public int Count => _cookies.Count;

        /// <summary>Reads a Netscape cookie file or a file of Set-Cookie headers; a missing file is ignored.</summary>
        internal void Load(string path, bool junkSession)
        {
            string[] lines;
            try
            {
                if (path == "-" || !File.Exists(path))
                {
                    return; // curl silently starts with an empty jar
                }
                lines = File.ReadAllLines(path, WireText.Latin1);
            }
            catch (IOException)
            {
                return;
            }
            catch (UnauthorizedAccessException)
            {
                return;
            }

            var now = Now();
            foreach (var raw in lines)
            {
                var line = raw.TrimEnd('\r', '\n');
                if (line.StartsWith("Set-Cookie:", StringComparison.OrdinalIgnoreCase))
                {
                    // header-style cookie file: no request host to match against, so accept as given
                    var parsed = ParseSetCookie(line.Substring(11), null, "/", secureOrigin: true, now);
                    if (parsed != null && !(junkSession && parsed.Expires == 0))
                    {
                        Add(parsed, now);
                    }
                    continue;
                }

                var httpOnly = false;
                if (line.StartsWith("#HttpOnly_", StringComparison.Ordinal))
                {
                    httpOnly = true;
                    line = line.Substring(10);
                }
                else if (line.StartsWith("#", StringComparison.Ordinal) || line.Trim().Length == 0)
                {
                    continue;
                }

                var fields = line.Split('\t');
                if (fields.Length < 6)
                {
                    continue;
                }

                if (!long.TryParse(fields[4], NumberStyles.Integer, CultureInfo.InvariantCulture, out var expires))
                {
                    continue;
                }

                var domain = fields[0];
                var cookie = new StoredCookie
                {
                    TailMatch = string.Equals(fields[1], "TRUE", StringComparison.OrdinalIgnoreCase),
                    Domain = domain.TrimStart('.').ToLowerInvariant(),
                    DomainAsGiven = domain.TrimStart('.'),
                    Path = fields[2].Length == 0 ? "/" : fields[2],
                    Secure = string.Equals(fields[3], "TRUE", StringComparison.OrdinalIgnoreCase),
                    Expires = expires,
                    Name = fields[5],
                    Value = fields.Length > 6 ? fields[6] : string.Empty,
                    HttpOnly = httpOnly,
                };

                if (junkSession && cookie.Expires == 0)
                {
                    continue;
                }
                if (cookie.Expires != 0 && cookie.Expires <= now)
                {
                    continue;
                }
                if (HasControl(cookie.Name) || HasControl(cookie.Value) || cookie.Path.IndexOf('\t') >= 0)
                {
                    continue;
                }
                if (!PrefixOk(cookie, explicitDomain: cookie.TailMatch))
                {
                    continue;
                }
                Add(cookie, now);
            }
        }

        /// <summary>Replaces the request's Cookie header with matching jar cookies plus the -b string.</summary>
        public void ApplyTo(HttpRequestMessage request, CurlOptions hop)
        {
            request.Headers.Remove("Cookie");
            var uri = request.RequestUri!;
            var header = BuildCookieHeader(uri, HostFor(uri, hop));
            var literal = hop.Cookie != null && hop.Cookie.IndexOf('=') >= 0 ? hop.Cookie : null;

            // a custom -H "Cookie: ..." is appended by curl after its own cookies
            hop.Headers.TryGetValue("Cookie", out var custom);

            var parts = new List<string>();
            if (!string.IsNullOrEmpty(header)) parts.Add(header!);
            if (!string.IsNullOrEmpty(literal)) parts.Add(literal!);
            if (parts.Count > 0)
            {
                request.Headers.TryAddWithoutValidation("Cookie", string.Join("; ", parts));
            }
            if (!string.IsNullOrEmpty(custom))
            {
                request.Headers.TryAddWithoutValidation("Cookie", custom);
            }
        }

        /// <summary>The host cookies are matched against: a custom Host header wins, like in curl.</summary>
        private static string HostFor(Uri uri, CurlOptions hop)
        {
            if (hop.Headers.TryGetValue("Host", out var host) && !string.IsNullOrWhiteSpace(host))
            {
                var h = host.Trim();
                if (h.StartsWith("[", StringComparison.Ordinal))
                {
                    var close = h.IndexOf(']');
                    return close > 0 ? h.Substring(1, close - 1) : h;
                }
                var colon = h.LastIndexOf(':');
                return (colon > 0 ? h.Substring(0, colon) : h).ToLowerInvariant();
            }
            return uri.Host.Trim('[', ']').ToLowerInvariant();
        }

        /// <summary>Builds the jar part of the Cookie header for a request, or null.</summary>
        internal string? BuildCookieHeader(Uri uri, string host)
        {
            var now = Now();
            var secure = IsSecureContext(uri, host);
            var path = string.IsNullOrEmpty(uri.AbsolutePath) ? "/" : uri.AbsolutePath;
            var matches = _cookies
                .Where(c => (c.Expires == 0 || c.Expires > now)
                            && DomainMatches(c, host)
                            && PathMatches(c.Path, path)
                            && (!c.Secure || secure))
                .OrderByDescending(c => c.Path.Length)
                .ThenByDescending(c => c.Domain.Length)
                .ThenByDescending(c => c.Name.Length)
                .ThenByDescending(c => c.Created)
                .ToList();

            if (matches.Count == 0)
            {
                return null;
            }

            var sb = new StringBuilder();
            var count = 0;
            foreach (var c in matches)
            {
                var pair = c.Value.Length > 0 || c.Name.Length == 0 ? c.Name + "=" + c.Value : c.Name + "=";
                if (sb.Length + pair.Length + 2 + "Cookie: ".Length > MaxCookieHeader || count >= MaxCookiesSent)
                {
                    break;
                }
                if (sb.Length > 0) sb.Append("; ");
                sb.Append(pair);
                count++;
            }
            return sb.Length == 0 ? null : sb.ToString();
        }

        /// <summary>Stores the cookies of one response.</summary>
        public void StoreFromResponse(Uri requestUri, IEnumerable<string> setCookieValues)
        {
            var now = Now();
            var host = requestUri.Host.Trim('[', ']').ToLowerInvariant();
            var defaultPath = DefaultPath(requestUri.AbsolutePath);
            var secure = IsSecureContext(requestUri, host);
            var stored = 0;
            foreach (var value in setCookieValues)
            {
                if (stored >= MaxSetCookiesPerResponse)
                {
                    break;
                }
                var cookie = ParseSetCookie(value, host, defaultPath, secure, now);
                if (cookie == null)
                {
                    continue;
                }
                stored++;
                if (cookie.Expires != 0 && cookie.Expires <= now)
                {
                    _cookies.RemoveAll(c => Same(c, cookie));
                    continue;
                }
                Add(cookie, now);
            }
        }

        /// <summary>Stores cookies using the Host header override when the request had one.</summary>
        public void StoreFromResponse(Uri requestUri, CurlOptions hop, IEnumerable<string> setCookieValues)
        {
            var host = HostFor(requestUri, hop);
            var builder = new UriBuilder(requestUri) { Host = host.Contains(":") ? "[" + host + "]" : host };
            StoreFromResponse(builder.Uri, setCookieValues);
        }

        private void Add(StoredCookie cookie, long now)
        {
            var existing = _cookies.FindIndex(c => Same(c, cookie));
            if (existing >= 0)
            {
                cookie.Created = _cookies[existing].Created;
                _cookies[existing] = cookie;
                return;
            }
            cookie.Created = ++_counter;
            _cookies.Add(cookie);
        }

        private static bool Same(StoredCookie a, StoredCookie b) =>
            a.Name == b.Name && a.Domain == b.Domain && a.Path == b.Path;

        /// <summary>Writes the jar (-c FILE, or -c - for standard output text returned to the caller).</summary>
        /// <returns>The jar text when the target is <c>-</c>, otherwise null.</returns>
        public string? Save(string? command)
        {
            if (string.IsNullOrEmpty(_jarPath))
            {
                return null;
            }

            var now = Now();
            var sb = new StringBuilder();
            sb.Append("# Netscape HTTP Cookie File\n");
            sb.Append("# https://curl.se/docs/http-cookies.html\n");
            sb.Append("# This file was generated by libcurl! Edit at your own risk.\n\n");
            foreach (var c in _cookies.Where(c => c.Expires == 0 || c.Expires > now).OrderByDescending(c => c.Created))
            {
                if (c.HttpOnly) sb.Append("#HttpOnly_");
                var domain = c.DomainAsGiven ?? c.Domain;
                sb.Append(c.TailMatch ? "." + domain : domain).Append('\t');
                sb.Append(c.TailMatch ? "TRUE" : "FALSE").Append('\t');
                sb.Append(c.Path).Append('\t');
                sb.Append(c.Secure ? "TRUE" : "FALSE").Append('\t');
                sb.Append(c.Expires.ToString(CultureInfo.InvariantCulture)).Append('\t');
                sb.Append(c.Name).Append('\t');
                sb.Append(c.Value).Append('\n');
            }

            if (_jarPath == "-")
            {
                return sb.ToString();
            }

            try
            {
                var dir = Path.GetDirectoryName(Path.GetFullPath(_jarPath));
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    throw new DirectoryNotFoundException($"directory '{dir}' does not exist");
                }
                File.WriteAllText(_jarPath, sb.ToString(), WireText.Latin1);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                throw new CurlWriteErrorException(_jarPath!,
                    $"could not save the cookie jar given with -c/--cookie-jar: {ex.Message}. Point -c at a writable file in an existing directory." +
                    "\nFor cookie handling, see: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/cookie-errors.md",
                    command);
            }
            return null;
        }

        /// <summary>Parses one Set-Cookie value; returns null when curl would reject it.</summary>
        internal static StoredCookie? ParseSetCookie(string header, string? host, string defaultPath, bool secureOrigin, long now)
        {
            var parts = header.Split(';');
            var first = parts[0];
            var eq = first.IndexOf('=');
            if (eq < 0)
            {
                return null;
            }

            var name = first.Substring(0, eq).Trim(' ', '\t');
            var value = first.Substring(eq + 1).Trim(' ', '\t');
            if (name.Length == 0 || name.Length + value.Length > MaxNameValue || HasControl(name) || HasControl(value))
            {
                return null;
            }
            if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"')
            {
                // quoted values are stored with their quotes, exactly as curl does
            }

            var cookie = new StoredCookie { Name = name, Value = value, Path = defaultPath };
            string? domain = null;
            long? maxAge = null;
            long? expires = null;
            var badExpires = false;
            for (var i = 1; i < parts.Length; i++)
            {
                var attr = parts[i];
                var aeq = attr.IndexOf('=');
                var key = (aeq < 0 ? attr : attr.Substring(0, aeq)).Trim(' ', '\t').ToLowerInvariant();
                var val = aeq < 0 ? string.Empty : attr.Substring(aeq + 1).Trim(' ', '\t');
                switch (key)
                {
                    case "secure":
                        cookie.Secure = true;
                        break;
                    case "httponly":
                        cookie.HttpOnly = true;
                        break;
                    case "domain":
                        if (val.Length > 0) domain = val;
                        break;
                    case "path":
                        if (val.IndexOf('\t') >= 0 || HasControl(val)) return null;
                        cookie.Path = SanitizePath(val);
                        break;
                    case "max-age":
                        if (long.TryParse(val.Trim('"'), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var age))
                        {
                            maxAge = age;
                        }
                        break;
                    case "expires":
                        var parsed = CurlDate.Parse(val);
                        if (parsed.HasValue) expires = parsed.Value;
                        else badExpires = true;
                        break;
                }
            }

            if (cookie.Secure && !secureOrigin)
            {
                return null; // curl refuses secure cookies from plain-HTTP origins (except localhost)
            }

            if (maxAge.HasValue)
            {
                cookie.Expires = maxAge.Value <= 0 ? 1 : now + Math.Min(maxAge.Value, MaxAgeCap);
            }
            else if (expires.HasValue)
            {
                cookie.Expires = expires.Value <= 0 ? 1 : Math.Min(expires.Value, now + MaxAgeCap);
            }
            else if (badExpires)
            {
                cookie.Expires = 0;
            }

            if (domain != null)
            {
                var given = domain.Trim();
                if (given.StartsWith(".", StringComparison.Ordinal)) given = given.Substring(1);
                given = given.TrimEnd('.');
                var d = given.ToLowerInvariant();
                if (d.Length == 0 || d.StartsWith(".", StringComparison.Ordinal))
                {
                    return null;
                }

                if (host != null)
                {
                    var hostIsIp = IPAddress.TryParse(host, out _);
                    if (hostIsIp)
                    {
                        if (d != host) return null;
                        cookie.TailMatch = false;
                    }
                    else
                    {
                        if (d != host && !host.EndsWith("." + d, StringComparison.Ordinal)) return null;
                        if (d.IndexOf('.') < 0 && d != host) return null; // a bare TLD such as "com"
                        cookie.TailMatch = true;
                    }
                }
                else
                {
                    cookie.TailMatch = true;
                }
                cookie.Domain = d;
                cookie.DomainAsGiven = given;
            }
            else
            {
                if (host == null)
                {
                    return null;
                }
                cookie.Domain = host;
                cookie.TailMatch = false;
            }

            if (!PrefixOk(cookie, explicitDomain: domain != null))
            {
                return null;
            }

            return cookie;
        }

        private static bool PrefixOk(StoredCookie cookie, bool explicitDomain)
        {
            if (cookie.Name.StartsWith("__Secure-", StringComparison.Ordinal) && !cookie.Secure)
            {
                return false;
            }
            if (cookie.Name.StartsWith("__Host-", StringComparison.Ordinal) && (!cookie.Secure || explicitDomain || cookie.Path != "/"))
            {
                return false;
            }
            return true;
        }

        private static bool HasControl(string s)
        {
            foreach (var c in s)
            {
                if ((c < 0x20 && c != '\t') || c == 0x7F) return true;
            }
            return false;
        }

        /// <summary>curl's sanitize_cookie_path: unquote, require a leading slash, drop one trailing slash.</summary>
        private static string SanitizePath(string path)
        {
            var p = path;
            if (p.Length >= 1 && p[0] == '"') p = p.Substring(1);
            if (p.Length >= 1 && p[p.Length - 1] == '"') p = p.Substring(0, p.Length - 1);
            if (p.Length == 0 || p[0] != '/') return "/";
            if (p.Length > 1 && p[p.Length - 1] == '/') p = p.Substring(0, p.Length - 1);
            return p;
        }

        /// <summary>RFC 6265 default-path: the request path up to (not including) its last slash.</summary>
        private static string DefaultPath(string requestPath)
        {
            if (string.IsNullOrEmpty(requestPath) || requestPath[0] != '/') return "/";
            var last = requestPath.LastIndexOf('/');
            return last <= 0 ? "/" : requestPath.Substring(0, last);
        }

        private static bool DomainMatches(StoredCookie cookie, string host)
        {
            if (cookie.TailMatch)
            {
                return host == cookie.Domain || host.EndsWith("." + cookie.Domain, StringComparison.Ordinal);
            }
            return host == cookie.Domain;
        }

        /// <summary>RFC 6265 path-match (case-sensitive).</summary>
        internal static bool PathMatches(string cookiePath, string requestPath)
        {
            if (cookiePath == "/" || cookiePath.Length == 0) return true;
            if (!requestPath.StartsWith(cookiePath, StringComparison.Ordinal)) return false;
            if (requestPath.Length == cookiePath.Length) return true;
            if (cookiePath[cookiePath.Length - 1] == '/') return true;
            return requestPath[cookiePath.Length] == '/';
        }

        /// <summary>HTTPS, or a loopback host that curl treats as secure.</summary>
        private static bool IsSecureContext(Uri uri, string host)
        {
            if (string.Equals(uri.Scheme, "https", StringComparison.OrdinalIgnoreCase)) return true;
            return host == "localhost" || host.EndsWith(".localhost", StringComparison.Ordinal) || host == "127.0.0.1" || host == "::1";
        }

        private static long Now() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    /// <summary>
    /// A tolerant date parser in the spirit of curl_getdate(): accepts RFC 1123, RFC 850, asctime and
    /// the loose "Fri Feb 13 11:56:27 GMT 2037" style, returning Unix seconds.
    /// </summary>
    internal static class CurlDate
    {
        private static readonly string[] Months = { "jan", "feb", "mar", "apr", "may", "jun", "jul", "aug", "sep", "oct", "nov", "dec" };

        public static long? Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            int? day = null, month = null, year = null, hour = null, minute = null, second = null;
            var offsetMinutes = 0;
            var tokens = text.Split(new[] { ' ', ',', '\t', '-' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var raw in tokens)
            {
                var t = raw.Trim('"');
                if (t.IndexOf(':') > 0)
                {
                    var hms = t.Split(':');
                    if (hms.Length >= 2 && int.TryParse(hms[0], out var h) && int.TryParse(hms[1], out var m))
                    {
                        hour = h;
                        minute = m;
                        second = hms.Length > 2 && int.TryParse(hms[2], out var s) ? s : 0;
                    }
                    continue;
                }

                if (t.Length >= 3 && char.IsLetter(t[0]))
                {
                    var idx = Array.IndexOf(Months, t.Substring(0, 3).ToLowerInvariant());
                    if (idx >= 0 && month == null)
                    {
                        month = idx + 1;
                    }
                    continue; // weekday names and GMT/UTC
                }

                if ((t[0] == '+' || (t[0] == '-' && t.Length == 5)) && t.Length == 5 && int.TryParse(t.Substring(1), out var off))
                {
                    var minutes = off / 100 * 60 + off % 100;
                    offsetMinutes = t[0] == '+' ? minutes : -minutes;
                    continue;
                }

                if (int.TryParse(t, NumberStyles.None, CultureInfo.InvariantCulture, out var n))
                {
                    if (t.Length >= 3 || n > 31)
                    {
                        year = n;
                    }
                    else if (day == null)
                    {
                        day = n;
                    }
                    else if (year == null)
                    {
                        year = n < 70 ? 2000 + n : 1900 + n;
                    }
                }
            }

            if (day == null || month == null || year == null)
            {
                return null;
            }

            try
            {
                var y = year.Value;
                if (y < 100) y += y < 70 ? 2000 : 1900;
                if (y > 9999) y = 9999;
                var dt = new DateTimeOffset(y, month.Value, Math.Min(day.Value, DateTime.DaysInMonth(y, month.Value)),
                    hour ?? 0, minute ?? 0, second ?? 0, TimeSpan.Zero);
                return dt.ToUnixTimeSeconds() - offsetMinutes * 60L;
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }
    }
}
