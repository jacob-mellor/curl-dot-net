/***************************************************************************
 * RedirectHandler - follows Location: like curl's lib/transfer.c Curl_follow()
 *
 * - 301/302: a POST (-d, -F) becomes a GET without body unless --post301/--post302.
 * - 303: every method except HEAD becomes GET unless it is a POST and --post303 was given.
 * - 307/308: method and body are kept.
 * - A method set with -X is kept for every hop (curl's documented -L caveat) unless --follow.
 * - Authorization (from -u, -H or the URL) and custom Cookie headers are only sent to the
 *   host/port/scheme of the first request, unless --location-trusted.
 * - --max-redirs N allows exactly N hops; -1 means unlimited; the next hop fails with exit 47.
 * - The Location value is trimmed, spaces are encoded (%20 in the path, + in the query),
 *   bytes above 0x7F are percent-encoded, and relative references are resolved (RFC 3986).
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core.Handlers
{
    internal class RedirectHandler
    {
        private static readonly Regex SchemePrefix = new Regex(@"^[A-Za-z][A-Za-z0-9+.\-]*:", RegexOptions.Compiled);

        /// <summary>
        /// Follows redirects starting from <paramref name="response"/> until a non-redirect response,
        /// a redirect without Location, or the --max-redirs limit.
        /// </summary>
        public async Task<(HttpResponseMessage Response, HttpRequestMessage Request)> FollowAsync(
            HttpTransfer transfer,
            HttpResponseMessage response,
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var current = response;
            var currentRequest = request;
            var options = transfer.Options;

            try
            {
            while (IsRedirect(current.StatusCode))
            {
                var location = transfer.GetLocation(current);
                if (string.IsNullOrWhiteSpace(location))
                {
                    break; // curl stops quietly at a 3xx without (usable) Location
                }

                var target = ResolveLocation(currentRequest.RequestUri!, location!, options.OriginalCommand);
                if (target == null)
                {
                    break;
                }

                // --location-trusted implies -L in curl
                if (!options.FollowLocation && !options.LocationTrusted)
                {
                    transfer.RedirectUrl = target.AbsoluteUri;
                    break;
                }

                if (options.MaxRedirects >= 0 && transfer.RedirectCount >= options.MaxRedirects)
                {
                    transfer.RedirectUrl = target.AbsoluteUri; // %{redirect_url}: where the next hop would have gone
                    throw new CurlTooManyRedirectsException(transfer.RedirectCount, options.MaxRedirects, options.OriginalCommand);
                }

                var scheme = target.Scheme.ToLowerInvariant();
                if (scheme != "http" && scheme != "https")
                {
                    throw new CurlUnsupportedProtocolException(scheme, options.OriginalCommand, redirect: true);
                }

                var hop = BuildHopOptions(transfer, (int)current.StatusCode, target);

                // Finish the previous response so its connection can be reused (curl reads and
                // discards redirect bodies too).
                transfer.RecordFallbackHeaders(current);
                await DrainAsync(current, cancellationToken).ConfigureAwait(false);

                transfer.RedirectCount++;
                transfer.HopOptions = hop;
                transfer.Auth?.Redirected(SameOrigin(currentRequest.RequestUri!, target));
                currentRequest = transfer.PrepareRequest(hop);
                transfer.AppendVerboseRequest(currentRequest);
                var previous = current;
                current = await transfer.SendAsync(currentRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                if (!ReferenceEquals(previous, current) && !ReferenceEquals(previous, response))
                {
                    previous.Dispose(); // drained intermediate hop (the first response belongs to the caller)
                }
                transfer.AppendVerboseResponse(current);
                var authenticated = await transfer.AuthenticateAsync(current, currentRequest, hop, cancellationToken).ConfigureAwait(false);
                current = authenticated.Response;
                currentRequest = authenticated.Request;
                transfer.Timings.Redirect = (DateTime.UtcNow - transfer.StartTime).TotalMilliseconds;
            }
            }
            catch
            {
                if (!ReferenceEquals(current, response))
                {
                    current.Dispose();
                }
                throw;
            }

            return (current, currentRequest);
        }

        /// <summary>
        /// Computes the options for the next hop: URL, method/body changes and credential stripping.
        /// The caller's options are never modified.
        /// </summary>
        internal static CurlOptions BuildHopOptions(HttpTransfer transfer, int status, Uri target)
        {
            var previous = transfer.HopOptions;
            var hop = previous.ShallowCopy();
            hop.Headers = new Dictionary<string, string>(previous.Headers, StringComparer.OrdinalIgnoreCase);

            // URL credentials (http://user:pass@host/) apply to that hop only
            var url = target.AbsoluteUri;
            if (!string.IsNullOrEmpty(target.UserInfo))
            {
                var info = target.UserInfo;
                var colon = info.IndexOf(':');
                hop.Credentials = new NetworkCredential(
                    Uri.UnescapeDataString(colon < 0 ? info : info.Substring(0, colon)),
                    colon < 0 ? string.Empty : Uri.UnescapeDataString(info.Substring(colon + 1)));
                url = new UriBuilder(target) { UserName = string.Empty, Password = string.Empty }.Uri.AbsoluteUri;
            }
            hop.Url = url;

            var method = EffectiveMethod(previous);
            var isPost = method == "POST" && HasBody(previous);
            var switchToGet = false;
            switch (status)
            {
                case 301:
                    switchToGet = isPost && !previous.Post301;
                    break;
                case 302:
                    switchToGet = isPost && !previous.Post302;
                    break;
                case 303:
                    switchToGet = method != "GET" && method != "HEAD" && !(isPost && previous.Post303);
                    break;
            }

            if (switchToGet)
            {
                hop.Method = "GET";
                hop.Data = null;
                hop.BinaryData = null;
                hop.FormData = new Dictionary<string, string>();
                hop.Files = new Dictionary<string, string>();
                hop.FormParts = new List<CurlFormPart>();
                // -X is kept for every hop with -L (documented curl behaviour); --follow obeys the code
                if (previous.FollowObeysMethod)
                {
                    hop.CustomMethod = null;
                }
                foreach (var name in new[] { "Content-Type", "Content-Length" })
                {
                    hop.Headers.Remove(name);
                }
            }

            if (previous.CredentialsFromUrl && string.IsNullOrEmpty(target.UserInfo) && !SameOrigin(transfer.FirstUri, target))
            {
                // user:password@ in a URL belongs to that host only, even with --location-trusted (which
                // is about -u credentials)
                hop.Credentials = null;
            }

            if (!previous.LocationTrusted && !SameOrigin(transfer.FirstUri, target))
            {
                if (string.IsNullOrEmpty(target.UserInfo))
                {
                    hop.Credentials = null;
                }
                hop.Headers.Remove("Authorization");
                hop.Headers.Remove("Cookie");
                hop.AwsSigV4 = null;
                hop.CrossOriginHop = true;
                if (hop.Cookie != null && hop.Cookie.IndexOf('=') >= 0)
                {
                    hop.Cookie = null; // a -b "name=value" string only goes to the first origin
                }
            }

            if (transfer.AutoReferer && transfer.CurrentRequest?.RequestUri != null)
            {
                // --referer ";auto": the page that redirected, without credentials or fragment
                var from = new UriBuilder(transfer.CurrentRequest.RequestUri) { UserName = string.Empty, Password = string.Empty, Fragment = string.Empty };
                hop.Referer = from.Uri.AbsoluteUri;
            }

            if (!string.Equals(previous.Url.Length > 0 ? new Uri(previous.Url).Host : string.Empty, target.Host, StringComparison.OrdinalIgnoreCase))
            {
                // a custom -H "Host: ..." names the first server; curl drops it when the redirect changes host
                hop.Headers.Remove("Host");
            }

            return hop;
        }

        private static string EffectiveMethod(CurlOptions options)
        {
            if (!string.IsNullOrEmpty(options.CustomMethod) && !options.HeadOnly)
            {
                // -X only renames the request line; what curl switches on is the request type
                if (HasBody(options)) return "POST";
                return options.Method?.ToUpperInvariant() == "HEAD" ? "HEAD" : (options.Method?.ToUpperInvariant() ?? "GET");
            }
            if (options.HeadOnly) return "HEAD";
            return string.IsNullOrEmpty(options.Method) ? (HasBody(options) ? "POST" : "GET") : options.Method!.ToUpperInvariant();
        }

        private static bool HasBody(CurlOptions options)
        {
            return options.Data != null || options.BinaryData != null || options.FormData.Count > 0
                || options.Files.Count > 0 || options.FormParts.Count > 0;
        }

        private static bool SameOrigin(Uri first, Uri target)
        {
            return string.Equals(first.Scheme, target.Scheme, StringComparison.OrdinalIgnoreCase)
                && string.Equals(first.Host, target.Host, StringComparison.OrdinalIgnoreCase)
                && first.Port == target.Port;
        }

        /// <summary>
        /// Resolves a raw Location value against the request URL the way curl does.
        /// Returns null when the value cannot form a URL.
        /// </summary>
        internal static Uri? ResolveLocation(Uri baseUri, string location, string? command = null)
        {
            var value = location.Trim(' ', '\t', '\r', '\n');
            if (value.Length == 0)
            {
                return null;
            }

            var hash = value.IndexOf('#');
            if (hash >= 0)
            {
                value = value.Substring(0, hash);
            }

            var encoded = EncodeLocation(value);

            // curl accepts "http:///host/path" (three slashes) but rejects four or more (exit 3)
            var schemeMatch = Regex.Match(encoded, @"^([A-Za-z][A-Za-z0-9+.\-]*):(/{3,})");
            if (schemeMatch.Success)
            {
                if (schemeMatch.Groups[2].Value.Length > 3)
                {
                    throw new CurlMalformedUrlException(location,
                        $"The redirect target \"{location}\" is not a valid URL: it has {schemeMatch.Groups[2].Value.Length} slashes after the scheme. Fix the server's Location header (scheme://host/path), or fetch the intended URL directly.", command);
                }
                encoded = schemeMatch.Groups[1].Value + "://" + encoded.Substring(schemeMatch.Length);
            }

            try
            {
                if (encoded.StartsWith("//", StringComparison.Ordinal))
                {
                    return new Uri(baseUri.Scheme + ":" + encoded);
                }

                if (SchemePrefix.IsMatch(encoded))
                {
                    if (Uri.TryCreate(encoded, UriKind.Absolute, out var absolute))
                    {
                        return absolute;
                    }
                    return null;
                }

                return new Uri(baseUri, encoded);
            }
            catch (UriFormatException)
            {
                return null;
            }
        }

        /// <summary>
        /// curl's CURLU_URLENCODE for redirects: spaces become %20 before the query and + inside it,
        /// control characters and bytes above 0x7F become %XX.
        /// </summary>
        private static string EncodeLocation(string value)
        {
            var sb = new StringBuilder(value.Length + 16);
            var inQuery = false;
            foreach (var c in value)
            {
                if (c == '?')
                {
                    inQuery = true;
                    sb.Append(c);
                }
                else if (c == ' ')
                {
                    sb.Append(inQuery ? "+" : "%20");
                }
                else if (c < 0x20 || c == 0x7F)
                {
                    sb.Append('%').Append(((int)c).ToString("X2"));
                }
                else if (c > 0x7F)
                {
                    // Header text is decoded byte-per-char (ISO-8859-1), so each char is one wire byte.
                    if (c <= 0xFF)
                    {
                        sb.Append('%').Append(((int)c).ToString("X2"));
                    }
                    else
                    {
                        foreach (var b in Encoding.UTF8.GetBytes(c.ToString()))
                        {
                            sb.Append('%').Append(b.ToString("X2"));
                        }
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>The most bytes of a skipped (redirect or authentication) response body that are read to keep the connection.</summary>
        private const long DrainLimit = 1024 * 1024;

        internal static async Task DrainAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.Content == null)
            {
                return;
            }

            try
            {
                // read and discard, like curl; a body over 1 MB is abandoned (the connection is then not
                // reused) instead of being held in memory
#if NETSTANDARD2_0 || NET472 || NET48
                using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
#else
                using var stream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
#endif
                var buffer = new byte[16384];
                long total = 0;
                int n;
                while ((n = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                {
                    total += n;
                    if (total > DrainLimit)
                    {
                        return;
                    }
                }
            }
            catch (HttpRequestException)
            {
                // a broken redirect body does not matter: the connection is simply not reused
            }
            catch (System.IO.IOException)
            {
            }
        }

        public bool IsRedirect(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.MovedPermanently ||
                   statusCode == HttpStatusCode.Found ||
                   statusCode == HttpStatusCode.SeeOther ||
                   statusCode == HttpStatusCode.TemporaryRedirect ||
#if NETSTANDARD2_0 || NET472 || NET48
                   statusCode == (HttpStatusCode)308; // PermanentRedirect
#else
                   statusCode == HttpStatusCode.PermanentRedirect;
#endif
        }
    }
}
