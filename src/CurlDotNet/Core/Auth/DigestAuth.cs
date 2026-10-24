/***************************************************************************
 * DigestAuth - HTTP Digest authentication (RFC 2617 / RFC 7616) like curl
 *
 * Transpiled from curl's lib/vauth/digest.c: Curl_auth_digest_get_pair() parses the challenge
 * (quoted strings with \-escapes, sloppy unquoted values), Curl_auth_decode_digest_http_message()
 * keeps nonce/realm/opaque/qop/algorithm/userhash/stale, and auth_create_digest_http_message()
 * builds the Authorization value with curl's field order and quoting.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace CurlDotNet.Core.Auth
{
    /// <summary>The Digest state of one transfer, for the server or for the proxy.</summary>
    internal sealed class DigestState
    {
        private const int MaxValueLength = 256;
        private const int MaxContentLength = 1024;

        /// <summary>Server nonce; null until a challenge was read.</summary>
        public string? Nonce { get; private set; }

        public string? Realm { get; private set; }

        public string? Opaque { get; private set; }

        /// <summary>"auth" or "auth-int" when the server offered quality of protection.</summary>
        public string? Qop { get; private set; }

        /// <summary>The algorithm exactly as the server named it (echoed back), or null.</summary>
        public string? Algorithm { get; private set; }

        public bool Stale { get; private set; }

        public bool UserHash { get; private set; }

        private DigestAlgorithm _algo = DigestAlgorithm.Md5;
        private int _nc;
        private string? _cnonce;

        /// <summary>
        /// Reads one <c>WWW-Authenticate: Digest ...</c> challenge (the text after "Digest"). Returns false
        /// when curl would call it a problem: no nonce, an unknown algorithm, a "-sess" algorithm without
        /// qop, or a second challenge without <c>stale=true</c> (the password was wrong).
        /// </summary>
        public bool Decode(string challenge)
        {
            var before = Nonce != null;
            Nonce = Realm = Opaque = Qop = Algorithm = null;
            Stale = UserHash = false;
            _algo = DigestAlgorithm.Md5;
            _nc = 0;
            _cnonce = null;

            var pos = 0;
            while (true)
            {
                while (pos < challenge.Length && (challenge[pos] == ' ' || challenge[pos] == '\t')) pos++;
                if (!GetPair(challenge, ref pos, out var name, out var content))
                {
                    break;
                }

                switch (name.ToLowerInvariant())
                {
                    case "nonce":
                        Nonce = content;
                        break;
                    case "stale":
                        if (string.Equals(content, "true", StringComparison.OrdinalIgnoreCase))
                        {
                            Stale = true;
                            _nc = 1;
                        }
                        break;
                    case "realm":
                        Realm = content;
                        break;
                    case "opaque":
                        Opaque = content;
                        break;
                    case "qop":
                        var auth = false;
                        var authInt = false;
                        foreach (var raw in content.Split(','))
                        {
                            var token = raw.TrimStart(' ', '\t');
                            if (string.Equals(token, "auth", StringComparison.OrdinalIgnoreCase)) auth = true;
                            else if (string.Equals(token, "auth-int", StringComparison.OrdinalIgnoreCase)) authInt = true;
                        }
                        Qop = auth ? "auth" : authInt ? "auth-int" : null;
                        break;
                    case "algorithm":
                        Algorithm = content;
                        switch (content.ToUpperInvariant())
                        {
                            case "MD5": _algo = DigestAlgorithm.Md5; break;
                            case "MD5-SESS": _algo = DigestAlgorithm.Md5Sess; break;
                            case "SHA-256": _algo = DigestAlgorithm.Sha256; break;
                            case "SHA-256-SESS": _algo = DigestAlgorithm.Sha256Sess; break;
                            default: return false; // SHA-512-256 and anything else: not supported
                        }
                        break;
                    case "userhash":
                        UserHash = string.Equals(content, "true", StringComparison.OrdinalIgnoreCase);
                        break;
                }

                while (pos < challenge.Length && (challenge[pos] == ' ' || challenge[pos] == '\t')) pos++;
                if (pos < challenge.Length && challenge[pos] == ',') pos++;
            }

            if (before && !Stale)
            {
                return false; // a new nonce without stale=true: the credentials were rejected
            }
            if (Nonce == null)
            {
                return false;
            }
            if (Qop == null && (_algo == DigestAlgorithm.Md5Sess || _algo == DigestAlgorithm.Sha256Sess))
            {
                return false;
            }
            return true;
        }

        /// <summary>curl's Curl_auth_digest_get_pair(): one <c>name=value</c> or <c>name="quoted \"value\""</c>.</summary>
        private static bool GetPair(string s, ref int pos, out string name, out string content)
        {
            var nameBuilder = new StringBuilder();
            var count = MaxValueLength - 1;
            while (pos < s.Length && s[pos] != '=' && count-- > 0)
            {
                nameBuilder.Append(s[pos++]);
            }
            name = nameBuilder.ToString();
            content = string.Empty;

            if (pos >= s.Length || s[pos] != '=')
            {
                return false;
            }
            pos++;

            var quoted = false;
            if (pos < s.Length && s[pos] == '"')
            {
                pos++;
                quoted = true;
            }

            var sb = new StringBuilder();
            var escape = false;
            for (count = MaxContentLength - 1; pos < s.Length && count-- > 0; pos++)
            {
                var c = s[pos];
                if (!escape)
                {
                    if (c == '\\' && quoted)
                    {
                        escape = true;
                        continue;
                    }
                    if (c == ',' && !quoted)
                    {
                        pos++;
                        break; // end of an unquoted ("sloppy") value
                    }
                    if (c == '\r' || c == '\n')
                    {
                        if (quoted)
                        {
                            return false; // no closing quote
                        }
                        pos++;
                        break;
                    }
                    if (c == '"')
                    {
                        if (!quoted)
                        {
                            return false;
                        }
                        pos++;
                        quoted = false;
                        break;
                    }
                }
                escape = false;
                sb.Append(c);
            }

            if (escape)
            {
                return false;
            }
            content = sb.ToString();
            return true;
        }

        /// <summary>
        /// The value of the Authorization header (without "Digest ") for <paramref name="method"/> and
        /// <paramref name="uri"/> (path and query, as on the request line). Each call with a qop uses the
        /// next nonce count.
        /// </summary>
        public string CreateResponse(string user, string password, string method, string uri)
        {
            if (_nc == 0)
            {
                _nc = 1;
            }
            if (_cnonce == null)
            {
                _cnonce = Convert.ToBase64String(Encoding.ASCII.GetBytes(RandomAlnum(12)));
            }

            var userh = UserHash ? Hash(user + ":" + (Realm ?? string.Empty)) : null;
            var ha1 = Hash(user + ":" + (Realm ?? string.Empty) + ":" + password);
            if (_algo == DigestAlgorithm.Md5Sess || _algo == DigestAlgorithm.Sha256Sess)
            {
                ha1 = Hash(ha1 + ":" + Nonce + ":" + _cnonce);
            }

            var a2 = method + ":" + uri;
            if (string.Equals(Qop, "auth-int", StringComparison.OrdinalIgnoreCase))
            {
                a2 += ":" + Hash(string.Empty); // like curl: the body is not hashed (auth-int for PUT/POST is unsupported)
            }
            var ha2 = Hash(a2);

            var nc = _nc.ToString("x8", CultureInfo.InvariantCulture);
            var response = Qop != null
                ? Hash(ha1 + ":" + Nonce + ":" + nc + ":" + _cnonce + ":" + Qop + ":" + ha2)
                : Hash(ha1 + ":" + Nonce + ":" + ha2);

            // control characters never reach the header line: curl writes them %XX-encoded (test 3221)
            var userQuoted = userh ?? Quote(EncodeControlCharacters(user));
            var realmQuoted = Quote(Realm ?? string.Empty);
            var uriQuoted = Quote(uri);

            var sb = new StringBuilder();
            if (Qop != null)
            {
                sb.Append("username=\"").Append(userQuoted).Append("\", realm=\"").Append(realmQuoted)
                  .Append("\", nonce=\"").Append(Nonce).Append("\", uri=\"").Append(uriQuoted)
                  .Append("\", cnonce=\"").Append(_cnonce).Append("\", nc=").Append(nc)
                  .Append(", qop=").Append(Qop).Append(", response=\"").Append(response).Append('"');
                _nc++;
            }
            else
            {
                sb.Append("username=\"").Append(userQuoted).Append("\", realm=\"").Append(realmQuoted)
                  .Append("\", nonce=\"").Append(Nonce).Append("\", uri=\"").Append(uriQuoted)
                  .Append("\", response=\"").Append(response).Append('"');
            }

            if (Opaque != null)
            {
                sb.Append(", opaque=\"").Append(Quote(Opaque)).Append('"');
            }
            if (Algorithm != null)
            {
                sb.Append(", algorithm=").Append(Algorithm);
            }
            if (UserHash)
            {
                sb.Append(", userhash=true");
            }
            return sb.ToString();
        }

        /// <summary>Writes control characters (CR, LF, ...) as upper-case <c>%XX</c> so they cannot split the header line.</summary>
        private static string EncodeControlCharacters(string value)
        {
            var sb = new StringBuilder(value.Length);
            foreach (var c in value)
            {
                if (c < 0x20 || c == 0x7F)
                {
                    sb.Append('%').Append(((int)c).ToString("X2", CultureInfo.InvariantCulture));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>curl's auth_digest_string_quoted(): backslash before <c>"</c> and <c>\</c>.</summary>
        private static string Quote(string value)
        {
            if (value.IndexOf('"') < 0 && value.IndexOf('\\') < 0)
            {
                return value;
            }
            var sb = new StringBuilder(value.Length + 4);
            foreach (var c in value)
            {
                if (c == '"' || c == '\\')
                {
                    sb.Append('\\');
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        private string Hash(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            byte[] digest;
            if (_algo == DigestAlgorithm.Sha256 || _algo == DigestAlgorithm.Sha256Sess)
            {
                using (var sha = SHA256.Create())
                {
                    digest = sha.ComputeHash(bytes);
                }
            }
            else
            {
                using (var md5 = MD5.Create())
                {
                    digest = md5.ComputeHash(bytes);
                }
            }

            var sb = new StringBuilder(digest.Length * 2);
            foreach (var b in digest)
            {
                sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }

        private static string RandomAlnum(int length)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            var chars = new char[length];
            for (var i = 0; i < length; i++)
            {
                chars[i] = alphabet[bytes[i] % alphabet.Length];
            }
            return new string(chars);
        }

        private enum DigestAlgorithm
        {
            Md5,
            Md5Sess,
            Sha256,
            Sha256Sess,
        }
    }
}
