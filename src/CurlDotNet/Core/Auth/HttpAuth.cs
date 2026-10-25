/***************************************************************************
 * HttpAuth - curl's HTTP authentication negotiation for one transfer
 *
 * Transpiled from curl's lib/http.c: Curl_http_output_auth() / output_auth_headers() decide the
 * Authorization and Proxy-Authorization header of every request and whether a POST/PUT goes out
 * as a "Content-Length: 0" probe while a multi-pass method (Digest, NTLM) is not done yet;
 * Curl_http_input_auth() reads WWW-Authenticate / Proxy-Authenticate on 401/407; and
 * Curl_http_auth_act() / pickoneauth() choose the method (Digest > NTLM > Basic) and whether
 * the request is sent again.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core.Auth
{
    /// <summary>curl's CURLAUTH_* bits for the methods CurlDotNet negotiates itself.</summary>
    [Flags]
    internal enum AuthMethods
    {
        None = 0,
        Basic = 1,
        Digest = 2,
        Ntlm = 8,

        /// <summary>curl's CURLAUTH_PICKNONE: a round trip offered nothing usable; no method is tried again.</summary>
        PickNone = 1 << 30,
    }

    /// <summary>One side of the negotiation (curl's struct auth): the server or the proxy.</summary>
    internal sealed class AuthSide
    {
        public AuthSide(bool proxy, AuthMethods want)
        {
            IsProxy = proxy;
            Want = want;
        }

        public bool IsProxy { get; }

        /// <summary>The methods the command allows.</summary>
        public AuthMethods Want { get; }

        /// <summary>The method in use (all wanted ones until a server round trip picked one).</summary>
        public AuthMethods Picked { get; set; }

        /// <summary>The methods the last 401/407 offered.</summary>
        public AuthMethods Available { get; set; }

        /// <summary>True when this side needs no more round trips.</summary>
        public bool Done { get; set; }

        /// <summary>True when the picked method needs another round trip (curl's multipass).</summary>
        public bool Multipass { get; set; }

        public DigestState Digest { get; private set; } = new DigestState();

        /// <summary>Forgets the Digest challenge (a redirect left the server that sent it).</summary>
        public void ResetDigest() => Digest = new DigestState();

        public NtlmState Ntlm { get; } = new NtlmState();
    }

    /// <summary>
    /// The authentication state of one transfer. <see cref="Apply"/> runs for every request it sends,
    /// <see cref="ShouldResend"/> for every response; together they reproduce curl's request sequence
    /// for <c>-u</c> with <c>--basic</c>, <c>--digest</c>, <c>--ntlm</c>, <c>--anyauth</c> and the proxy
    /// equivalents.
    /// </summary>
    internal sealed class HttpAuth
    {
        private readonly AuthSide? _host;
        private readonly AuthSide? _proxy;
        private readonly NetworkCredential? _proxyCredentials;
        private bool _authNegotiating;

        private HttpAuth(AuthSide? host, AuthSide? proxy, NetworkCredential? proxyCredentials)
        {
            _host = host;
            _proxy = proxy;
            _proxyCredentials = proxyCredentials;
        }

        /// <summary>curl's data->state.authproblem: once set, no more authentication round trips.</summary>
        public bool Problem { get; private set; }

        /// <summary>
        /// The engine for a transfer, or null when CurlDotNet leaves authentication to the request builder
        /// (no challenge method) or to HttpClient (<c>--negotiate</c>).
        /// </summary>
        /// <param name="options">The command's options.</param>
        /// <param name="proxyOwned">True when proxy authentication of plain (non-CONNECT) proxy requests is negotiated here.</param>
        /// <param name="proxyScheme">The proxy authentication method (basic, digest, ntlm, anyauth).</param>
        /// <param name="proxyCredentials">The proxy credentials from <c>-U</c> or the proxy URL.</param>
        public static HttpAuth? Create(CurlOptions options, bool proxyOwned, string? proxyScheme, NetworkCredential? proxyCredentials)
        {
            var hostWant = WantFor(options.AuthScheme);
            var host = hostWant != AuthMethods.None && hostWant != AuthMethods.Basic ? new AuthSide(false, hostWant) : null;
            AuthSide? proxy = null;
            if (proxyOwned && proxyCredentials != null)
            {
                var proxyWant = WantFor(proxyScheme);
                if (proxyWant != AuthMethods.None && proxyWant != AuthMethods.Basic)
                {
                    proxy = new AuthSide(true, proxyWant);
                }
            }

            return host == null && proxy == null ? null : new HttpAuth(host, proxy, proxyCredentials);
        }

        /// <summary>
        /// The proxy-only engine for one CONNECT tunnel (curl's data->state.authproxy while lib/http_proxy.c
        /// opens the tunnel), or null when CurlDotNet does not negotiate it: no credentials, Basic (sent
        /// preemptively), Negotiate (left to HttpClient), and for now Digest and --proxy-anyauth, whose
        /// CONNECT behaviour is not yet confirmed against curl.
        /// </summary>
        /// <param name="proxyScheme">The proxy authentication method; only ntlm is negotiated here.</param>
        /// <param name="proxyCredentials">The proxy credentials from <c>-U</c> or the proxy URL.</param>
        public static HttpAuth? ForTunnel(string? proxyScheme, NetworkCredential? proxyCredentials)
        {
            var want = WantFor(proxyScheme);
            if (proxyCredentials == null || want != AuthMethods.Ntlm)
            {
                return null;
            }
            return new HttpAuth(null, new AuthSide(true, want), proxyCredentials);
        }

        /// <summary>
        /// The <c>Proxy-Authorization</c> value for the next CONNECT request (curl's
        /// <c>Curl_http_output_auth(data, conn, "CONNECT", HTTPREQ_GET, hostport, TRUE)</c>): the NTLM
        /// type-1 message first, the type-3 answer after the proxy's type-2 challenge, then nothing.
        /// </summary>
        /// <param name="authority">The CONNECT target, <c>host:port</c>.</param>
        /// <exception cref="CurlException">The NTLM type-3 message would exceed curl's 1024-byte limit (exit 100).</exception>
        public string? TunnelAuthorization(string authority)
        {
            var side = _proxy!;
            if (side.Picked == AuthMethods.None)
            {
                side.Picked = side.Want; // curl: authproxy.picked = authproxy.want before the first request
            }
            return OutputValue(side, _proxyCredentials!, "CONNECT", () => authority);
        }

        /// <summary>
        /// Reads a CONNECT response's <c>Proxy-Authenticate</c> challenges and decides, like curl's
        /// Curl_http_auth_act() after a 407 to CONNECT, whether another CONNECT is sent.
        /// </summary>
        /// <param name="status">The CONNECT response code.</param>
        /// <param name="proxyChallenges">The response's <c>Proxy-Authenticate</c> values.</param>
        public bool TunnelShouldRetry(int status, IReadOnlyList<string> proxyChallenges)
            => ShouldResend(status, Array.Empty<string>(), proxyChallenges, hasHostCredentials: false, hasProxyCredentials: true);

        /// <summary>The method the proxy side uses now, for messages ("NTLM", "Digest", "Basic").</summary>
        public string TunnelMethodName
        {
            get
            {
                switch (_proxy?.Picked ?? AuthMethods.None)
                {
                    case AuthMethods.Ntlm: return "NTLM";
                    case AuthMethods.Digest: return "Digest";
                    case AuthMethods.Basic: return "Basic";
                    default: return "proxy";
                }
            }
        }

        /// <summary>True when the server side is negotiated by this engine (so no preemptive Basic is added elsewhere).</summary>
        public bool OwnsHostAuth => _host != null;

        /// <summary>True when the proxy side is negotiated by this engine.</summary>
        public bool OwnsProxyAuth => _proxy != null;

        /// <summary>
        /// The methods a <c>-u</c> scheme option allows; null/basic is Basic only, anyauth is every
        /// method CurlDotNet can answer itself.
        /// </summary>
        internal static AuthMethods WantFor(string? scheme)
        {
            switch ((scheme ?? "basic").ToLowerInvariant())
            {
                case "basic":
                    return AuthMethods.Basic;
                case "digest":
                    return AuthMethods.Digest;
                case "ntlm":
                    return AuthMethods.Ntlm;
                case "anyauth":
                    return AuthMethods.Basic | AuthMethods.Digest | AuthMethods.Ntlm;
                default:
                    return AuthMethods.None; // negotiate: HttpClient's own implementation
            }
        }

        /// <summary>
        /// Adds this request's authentication headers (curl's Curl_http_output_auth) and, while a
        /// multi-pass method is still negotiating, turns a POST/PUT into a <c>Content-Length: 0</c>
        /// probe that carries no body.
        /// </summary>
        public void Apply(HttpRequestMessage request, CurlOptions hop, bool viaPlainProxy)
        {
            var uri = request.RequestUri!;
            var credentials = RequestShaping.GetCredentials(hop, uri);
            var proxyCredentials = viaPlainProxy ? _proxyCredentials : null;

            if (_proxy != null)
            {
                if (proxyCredentials == null)
                {
                    _proxy.Done = true;
                }
                else
                {
                    if (_proxy.Picked == AuthMethods.None)
                    {
                        _proxy.Picked = _proxy.Want;
                    }
                    Output(request, _proxy, proxyCredentials, "Proxy-Authorization");
                }
            }

            if (_host != null)
            {
                if (credentials == null)
                {
                    _host.Done = true; // no user: nothing to authenticate (curl: "no authentication with no user or password")
                }
                else
                {
                    if (_host.Picked == AuthMethods.None)
                    {
                        _host.Picked = _host.Want;
                    }
                    Output(request, _host, credentials, "Authorization");
                }
            }

            var negotiating = ((_host != null && _host.Multipass && !_host.Done) || (_proxy != null && _proxy.Multipass && !_proxy.Done))
                              && request.Content != null;
            _authNegotiating = negotiating;
            if (negotiating)
            {
                StripBody(request);
            }
        }

        /// <summary>curl's output_auth_headers() for one side of a request sent through HttpClient.</summary>
        private static void Output(HttpRequestMessage request, AuthSide side, NetworkCredential credentials, string header)
        {
            // curl hashes the path and query (also for requests sent to a proxy with an absolute URI)
            var value = OutputValue(side, credentials, request.Method.Method, () => request.RequestUri!.PathAndQuery);
            if (value != null && !request.Headers.Contains(header))
            {
                request.Headers.TryAddWithoutValidation(header, value);
            }
        }

        /// <summary>
        /// curl's output_auth_headers(): the header value the picked method sends next (null when it sends
        /// nothing yet), advancing the method's state. <paramref name="target"/> is the request target a
        /// Digest response covers: the path and query, or <c>host:port</c> for CONNECT.
        /// </summary>
        private static string? OutputValue(AuthSide side, NetworkCredential credentials, string method, Func<string> target)
        {
            string? value = null;
            var multipassMethod = false;
            switch (side.Picked)
            {
                case AuthMethods.Ntlm:
                    multipassMethod = true;
                    switch (side.Ntlm.Stage)
                    {
                        case NtlmStage.Type2:
                            value = "NTLM " + (side.Ntlm.Type3(credentials.UserName ?? string.Empty, credentials.Password ?? string.Empty)
                                ?? throw new CurlException(CurlException.Compose(
                                    $"The NTLM {(side.IsProxy ? "proxy " : string.Empty)}user name \"{Shorten(credentials.UserName)}\" is too long: the NTLM type-3 message would exceed {NtlmState.BufferSize} bytes",
                                    $"Use a shorter user (and domain) name in {(side.IsProxy ? "-U/--proxy-user" : "-u/--user")}, or a different scheme such as --basic over https:// if the server offers it.",
                                    DocLinks.Auth),
                                    100));
                            side.Ntlm.Stage = NtlmStage.Type3;
                            side.Done = true;
                            break;
                        case NtlmStage.Type3:
                        case NtlmStage.Last:
                            side.Ntlm.Stage = NtlmStage.Last; // the connection is authenticated
                            side.Done = true;
                            break;
                        default:
                            value = "NTLM " + NtlmState.Type1();
                            side.Ntlm.Stage = NtlmStage.Type1;
                            break;
                    }
                    break;

                case AuthMethods.Digest:
                    multipassMethod = true;
                    if (side.Digest.Nonce == null)
                    {
                        side.Done = false; // no challenge yet
                    }
                    else
                    {
                        value = "Digest " + side.Digest.CreateResponse(credentials.UserName ?? string.Empty, credentials.Password ?? string.Empty,
                            method, target());
                        side.Done = true;
                    }
                    break;

                case AuthMethods.Basic:
                    multipassMethod = true;
                    var raw = (credentials.UserName ?? string.Empty) + ":" + (credentials.Password ?? string.Empty);
                    value = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
                    side.Done = true;
                    break;
            }

            side.Multipass = multipassMethod && !side.Done;
            return value;
        }

        /// <summary>A probe sends no body: <c>Content-Length: 0</c>, keeping the Content-Type of a simple POST.</summary>
        private static void StripBody(HttpRequestMessage request)
        {
            var original = request.Content!;
            var empty = new ByteArrayContent(Array.Empty<byte>());
            if (!(original is MultipartContent) && original.Headers.ContentType != null)
            {
                empty.Headers.TryAddWithoutValidation("Content-Type", original.Headers.ContentType.ToString());
            }
            if (original.Headers.TryGetValues("Content-Range", out var ranges))
            {
                // -T with -r: curl's probe reports an upload size of 0 ("bytes 0-4/0")
                var range = ranges.First();
                var slash = range.LastIndexOf('/');
                empty.Headers.TryAddWithoutValidation("Content-Range", slash >= 0 ? range.Substring(0, slash + 1) + "0" : range);
            }
            request.Content = empty;
            original.Dispose();
            request.Headers.ExpectContinue = false;
        }

        /// <summary>
        /// Reads a response's challenges and decides, like curl's Curl_http_auth_act(), whether the same
        /// URL is requested again: after a 401/407 that offers a usable method, or after a probe the
        /// server answered without asking for authentication.
        /// </summary>
        /// <param name="status">The response code.</param>
        /// <param name="hostChallenges">The WWW-Authenticate values of the response.</param>
        /// <param name="proxyChallenges">The Proxy-Authenticate values of the response.</param>
        /// <param name="hasHostCredentials">True when the request had server credentials.</param>
        /// <param name="hasProxyCredentials">True when the request went through a plain proxy with credentials.</param>
        public bool ShouldResend(int status, IReadOnlyList<string> hostChallenges, IReadOnlyList<string> proxyChallenges,
            bool hasHostCredentials, bool hasProxyCredentials)
        {
            if (status >= 100 && status < 200)
            {
                return false;
            }

            if (status == 401 && _host != null)
            {
                foreach (var challenge in hostChallenges)
                {
                    Input(_host, challenge);
                }
            }
            if (status == 407 && _proxy != null)
            {
                foreach (var challenge in proxyChallenges)
                {
                    Input(_proxy, challenge);
                }
            }

            if (Problem)
            {
                return false;
            }

            var pickHost = false;
            var pickProxy = false;
            if (_host != null && hasHostCredentials && (status == 401 || (_authNegotiating && status < 300)))
            {
                pickHost = Pick(_host);
                if (!pickHost)
                {
                    Problem = true;
                }
            }
            if (_proxy != null && hasProxyCredentials && (status == 407 || (_authNegotiating && status < 300)))
            {
                pickProxy = Pick(_proxy);
                if (!pickProxy)
                {
                    Problem = true;
                }
            }

            if (pickHost || pickProxy)
            {
                return true;
            }

            if (status < 300 && _host != null && !_host.Done && _authNegotiating)
            {
                // the probe got through without authentication: send the real body now
                _host.Done = true;
                return true;
            }
            return false;
        }

        /// <summary>curl's Curl_http_input_auth() for one challenge header.</summary>
        private void Input(AuthSide side, string challenge)
        {
            var text = challenge.TrimStart();
            while (text.Length > 0)
            {
                if (StartsWithScheme(text, "NTLM"))
                {
                    side.Available |= AuthMethods.Ntlm;
                    if (side.Picked == AuthMethods.Ntlm)
                    {
                        var blob = text.Substring(4).Trim();
                        if (blob.Length > 0)
                        {
                            if (side.Ntlm.DecodeType2(blob))
                            {
                                side.Ntlm.Stage = NtlmStage.Type2;
                                Problem = false;
                            }
                            else
                            {
                                Problem = true;
                            }
                        }
                        else if (side.Ntlm.Stage == NtlmStage.Last)
                        {
                            side.Ntlm.Stage = NtlmStage.Type1; // NTLM auth restarted
                        }
                        else if (side.Ntlm.Stage == NtlmStage.Type3)
                        {
                            side.Ntlm.Stage = NtlmStage.None; // handshake rejected (wrong password)
                            Problem = true;
                        }
                        else if (side.Ntlm.Stage >= NtlmStage.Type1)
                        {
                            Problem = true;
                        }
                        else
                        {
                            side.Ntlm.Stage = NtlmStage.Type1;
                        }
                    }
                }
                else if (StartsWithScheme(text, "Digest"))
                {
                    if ((side.Available & AuthMethods.Digest) == 0)
                    {
                        side.Available |= AuthMethods.Digest;
                        // read even when Digest is not picked yet: --anyauth may pick it next
                        if (!side.Digest.Decode(text.Substring(6)))
                        {
                            Problem = true;
                        }
                    }
                    // a second Digest challenge in the same response is ignored, like curl
                }
                else if (StartsWithScheme(text, "Basic"))
                {
                    side.Available |= AuthMethods.Basic;
                    if (side.Picked == AuthMethods.Basic)
                    {
                        // Basic was already sent and refused
                        side.Available = AuthMethods.None;
                        Problem = true;
                    }
                }

                // several challenges may share one header line, separated by commas outside quotes
                var next = NextChallenge(text);
                if (next < 0)
                {
                    break;
                }
                text = text.Substring(next).TrimStart();
            }
        }

        /// <summary>
        /// The connection of the last response is closing. NTLM authenticates a connection, so an
        /// authenticated handshake starts over on the next one, from the wanted methods: <c>--ntlm</c>
        /// sends a type-1 message straight away, <c>--anyauth</c> first asks without credentials.
        /// </summary>
        public void ConnectionClosed()
        {
            foreach (var side in new[] { _host, _proxy })
            {
                if (side != null && side.Picked == AuthMethods.Ntlm && side.Ntlm.Stage >= NtlmStage.Type3)
                {
                    side.Ntlm.Stage = NtlmStage.None;
                    side.Done = false;
                    side.Picked = AuthMethods.None;
                }
            }
        }

        /// <summary>
        /// A redirect is followed. The picked method stays (a Basic answer is sent to the next hop at
        /// once), but a Digest challenge belongs to the server that sent it.
        /// </summary>
        /// <param name="sameOrigin">False when the redirect goes to another scheme, host or port: the
        /// server's Digest nonce is not reused there.</param>
        public void Redirected(bool sameOrigin)
        {
            if (!sameOrigin && _host != null)
            {
                _host.ResetDigest();
            }
        }

        /// <summary>curl's pickoneauth(): the preferred offered method (Digest, then NTLM, then Basic).</summary>
        private static bool Pick(AuthSide side)
        {
            var available = side.Available & side.Want;
            var picked = true;
            if ((available & AuthMethods.Digest) != 0)
            {
                side.Picked = AuthMethods.Digest;
            }
            else if ((available & AuthMethods.Ntlm) != 0)
            {
                side.Picked = AuthMethods.Ntlm;
            }
            else if ((available & AuthMethods.Basic) != 0)
            {
                side.Picked = AuthMethods.Basic;
            }
            else
            {
                side.Picked = AuthMethods.PickNone;
                picked = false;
            }
            side.Available = AuthMethods.None;
            return picked;
        }

        private static bool StartsWithScheme(string text, string scheme)
        {
            if (!text.StartsWith(scheme, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            if (text.Length == scheme.Length)
            {
                return true;
            }
            var c = text[scheme.Length];
            return c == ' ' || c == '\t' || c == ',' || c == '\r' || c == '\n'; // curl's is_valid_auth_separator()
        }

        /// <summary>
        /// Where the next challenge starts on a header line that lists several (<c>Digest ..., NTLM</c>):
        /// after a comma outside quotes that is followed by a token and then a space, comma or end.
        /// Returns -1 when there is none.
        /// </summary>
        private static int NextChallenge(string text)
        {
            var quoted = false;
            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];
                if (c == '\\' && quoted)
                {
                    i++;
                    continue;
                }
                if (c == '"')
                {
                    quoted = !quoted;
                    continue;
                }
                if (c != ',' || quoted)
                {
                    continue;
                }

                var j = i + 1;
                while (j < text.Length && (text[j] == ' ' || text[j] == '\t')) j++;
                var start = j;
                while (j < text.Length && (char.IsLetterOrDigit(text[j]) || text[j] == '-' || text[j] == '_')) j++;
                if (j > start && (j == text.Length || text[j] == ' ' || text[j] == '\t' || text[j] == ','))
                {
                    var scheme = text.Substring(start, j - start);
                    if (string.Equals(scheme, "Basic", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(scheme, "Digest", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(scheme, "NTLM", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(scheme, "Negotiate", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                    {
                        return start;
                    }
                }
            }
            return -1;
        }

        private static string Shorten(string? text)
        {
            text ??= string.Empty;
            return text.Length <= 40 ? text : text.Substring(0, 40) + "...";
        }
    }
}
