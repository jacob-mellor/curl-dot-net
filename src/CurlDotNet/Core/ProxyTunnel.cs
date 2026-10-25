/***************************************************************************
 * ProxyTunnel - the CONNECT tunnels CurlDotNet opens itself, with curl's proxy NTLM handshake
 *
 * Used for -p/--proxytunnel with http:// URLs and for https:// URLs through an HTTP proxy
 * (CurlProxy.SelfTunnels). Follows what curl's own tests 209, 213 and 265 pin down for
 * lib/http_proxy.c (lib/cf-h1-proxy.c in curl 8) and lib/http_ntlm.c:
 * - the CONNECT request carries curl's headers in curl's order (Host, Proxy-Authorization,
 *   User-Agent, Proxy-Connection: Keep-Alive), as HTTP/1.0 with --proxy1.0;
 * - with --proxy-ntlm the first CONNECT sends the NTLM type-1 message, the proxy's 407 carries the
 *   type-2 challenge, and the type-3 answer goes out in a second CONNECT on the same connection
 *   (the same Core/Auth engine as plain proxy requests produces curl's exact bytes);
 * - the body of that 407 is read and ignored before the second CONNECT (test 265);
 * - with -i/--include every CONNECT response header block is printed before the tunnelled
 *   response, the 407 bodies are not.
 * Not implemented here, pending curl reference runs: Digest and --proxy-anyauth on CONNECT, and
 * reconnecting when the proxy closes the connection after a 407.
 * .NET 5+ only: the older targets let HttpClient open CONNECT tunnels.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

#if !NETSTANDARD2_0 && !NET472 && !NET48
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core.Auth;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Opens a CONNECT tunnel through an HTTP proxy on a connection the connect callback opened,
    /// answering an NTLM proxy challenge like curl.
    /// </summary>
    internal static class ProxyTunnel
    {
        /// <summary>
        /// CONNECT requests per tunnel before giving up. NTLM finishes in two; the cap only stops a proxy
        /// that keeps answering with fresh challenges.
        /// </summary>
        private const int MaxRounds = 4;

        /// <summary>Largest CONNECT response header block accepted (curl's MAX_HTTP_RESP_HEADER_SIZE, 300 KB).</summary>
        private const int MaxHeaderBytes = 300 * 1024;

        /// <summary>
        /// Sends CONNECT for <paramref name="target"/> to the proxy in <c>settings.TunnelProxy</c> and
        /// returns <paramref name="stream"/> once the proxy answered 2xx. On failure the stream is disposed.
        /// </summary>
        /// <param name="stream">The fresh connection to the proxy.</param>
        /// <param name="target">The host and port to tunnel to.</param>
        /// <param name="settings">The request's transport settings (proxy, Basic header, version, User-Agent, transcript).</param>
        /// <param name="token">Cancelled by the caller or by --connect-timeout.</param>
        /// <param name="callerToken">Cancelled by the caller only, to tell a connect timeout apart.</param>
        /// <exception cref="CurlConnectFailure">The proxy refused the tunnel or closed the connection (Tunnel), or --connect-timeout expired (Timeout).</exception>
        public static async Task<InfoNetworkStream> OpenAsync(InfoNetworkStream stream, DnsEndPoint target, TransportRequestSettings settings,
            CancellationToken token, CancellationToken callerToken)
        {
            var proxy = settings.TunnelProxy!;
            var authority = target.Host.Contains(":") ? "[" + target.Host + "]:" + target.Port : target.Host + ":" + target.Port;
            var owner = CurlProxy.ForTunnelProxy(proxy);
            var auth = owner?.CreateTunnelAuth();
            var replies = new List<ResponseStreamParser>();

            try
            {
                for (var round = 1; ; round++)
                {
                    // NTLM from the negotiation (type-1, then type-3), otherwise the preemptive Basic header (or none)
                    var authorization = auth != null ? auth.TunnelAuthorization(authority) : settings.TunnelAuthorization;
                    var reply = await ExchangeAsync(stream, BuildRequest(authority, authorization, settings), proxy, token).ConfigureAwait(false);
                    replies.Add(reply);

                    var head = reply.Heads[reply.Heads.Count - 1];
                    var status = head.StatusCode;
                    if (status >= 200 && status <= 299)
                    {
                        RecordTunnel(settings, replies);
                        return stream;
                    }

                    // the 407's Proxy-Authenticate challenge (curl: Curl_http_input_auth + Curl_http_auth_act)
                    var retry = auth != null && status == 407 && round < MaxRounds
                                && auth.TunnelShouldRetry(status, head.GetHeaders("Proxy-Authenticate").ToList());
                    if (!retry)
                    {
                        RecordRefusal(settings, replies);
                        throw Refused(proxy, head, authorization, auth, owner);
                    }

                    // read and ignore the 407 body so the next CONNECT goes out on the same connection
                    if (!await DrainBodyAsync(stream, reply, token).ConfigureAwait(false))
                    {
                        RecordRefusal(settings, replies);
                        throw new CurlConnectFailure(ConnectFailureKind.Tunnel, proxy.Host, proxy.Port,
                            $"CONNECT tunnel failed, response {status}; the proxy's 407 has no Content-Length and is not chunked, " +
                            "so the NTLM answer cannot follow on the same connection (CurlDotNet does not reconnect for it)") { TunnelStatus = status };
                    }
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested && !callerToken.IsCancellationRequested)
            {
                // --connect-timeout covers the tunnel set-up, like the TCP connect before it
                stream.Dispose();
                throw new CurlConnectFailure(ConnectFailureKind.Timeout, proxy.Host, proxy.Port,
                    "Connection timed out while the proxy set up the CONNECT tunnel");
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }

        /// <summary>curl's CONNECT request: request line, Host, Proxy-Authorization, User-Agent, Proxy-Connection.</summary>
        private static byte[] BuildRequest(string authority, string? authorization, TransportRequestSettings settings)
        {
            var sb = new StringBuilder();
            sb.Append("CONNECT ").Append(authority).Append(" HTTP/").Append(settings.TunnelVersion).Append("\r\n");
            sb.Append("Host: ").Append(authority).Append("\r\n");
            if (authorization != null)
            {
                sb.Append("Proxy-Authorization: ").Append(authorization).Append("\r\n");
            }
            if (!string.IsNullOrEmpty(settings.TunnelUserAgent))
            {
                sb.Append("User-Agent: ").Append(settings.TunnelUserAgent).Append("\r\n");
            }
            sb.Append("Proxy-Connection: Keep-Alive\r\n\r\n");
            return Encoding.ASCII.GetBytes(sb.ToString());
        }

        /// <summary>
        /// Sends one CONNECT and reads the reply up to the end of its final (non-1xx) header block, one
        /// byte at a time so nothing of the tunnelled data is consumed.
        /// </summary>
        private static async Task<ResponseStreamParser> ExchangeAsync(Stream stream, byte[] request, Uri proxy, CancellationToken token)
        {
            await stream.WriteAsync(request, 0, request.Length, token).ConfigureAwait(false);
            await stream.FlushAsync(token).ConfigureAwait(false);

            var parser = new ResponseStreamParser(isHeadRequest: false);
            var buffer = new byte[1];
            while (parser.Heads.Count == 0 || parser.Heads[parser.Heads.Count - 1].IsInformational)
            {
                var n = await stream.ReadAsync(buffer, 0, 1, token).ConfigureAwait(false);
                if (n <= 0)
                {
                    throw new CurlConnectFailure(ConnectFailureKind.Tunnel, proxy.Host, proxy.Port,
                        "Proxy closed the connection while answering CONNECT");
                }
                parser.Feed(buffer, 0, 1);
                if (parser.PendingHeadBytes > MaxHeaderBytes)
                {
                    throw new CurlConnectFailure(ConnectFailureKind.Tunnel, proxy.Host, proxy.Port,
                        $"The proxy's answer to CONNECT has more than {MaxHeaderBytes / 1024} KB of headers");
                }
            }
            return parser;
        }

        /// <summary>
        /// Reads and ignores the body of a 407 that is answered on the same connection (Content-Length or
        /// chunked). Returns false when there is no length to read up to, or the proxy closed early.
        /// </summary>
        private static async Task<bool> DrainBodyAsync(Stream stream, ResponseStreamParser parser, CancellationToken token)
        {
            var head = parser.Heads[parser.Heads.Count - 1];
            if (head.BodyComplete)
            {
                return true; // Content-Length: 0
            }

            if (head.Chunked)
            {
                var one = new byte[1];
                while (!head.BodyComplete)
                {
                    var n = await stream.ReadAsync(one, 0, 1, token).ConfigureAwait(false);
                    if (n <= 0)
                    {
                        return false;
                    }
                    parser.Feed(one, 0, 1);
                }
                return true;
            }

            if (head.DeclaredLength is long length && length > 0)
            {
                var buffer = new byte[(int)Math.Min(length, 16 * 1024)];
                var remaining = length;
                while (remaining > 0)
                {
                    var n = await stream.ReadAsync(buffer, 0, (int)Math.Min(remaining, buffer.Length), token).ConfigureAwait(false);
                    if (n <= 0)
                    {
                        return false;
                    }
                    remaining -= n;
                }
                head.BodyComplete = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// The tunnel is up: its CONNECT response header blocks become part of the exchange whose request
        /// this connection was opened for, so -i prints them before that request's response (curl writes
        /// them as they arrive). Done once, right before the request is written, so the exchange's
        /// timings are unaffected; no bytes are fed, so -w byte counts stay those of the request.
        /// </summary>
        private static void RecordTunnel(TransportRequestSettings settings, List<ResponseStreamParser> replies)
        {
            var exchange = WireExchange.Current ?? settings.Transcript?.Exchanges.LastOrDefault();
            if (exchange == null)
            {
                return;
            }
            foreach (var reply in replies)
            {
                var segment = exchange.Attach(null, newConnection: false);
                foreach (var head in reply.Heads)
                {
                    head.BodyComplete = true; // a 2xx to CONNECT has no body; a 407's was read and ignored
                    segment.Parser.Heads.Add(head);
                }
            }
        }

        /// <summary>The tunnel failed: its CONNECT response header blocks are recorded as their own exchange, so -i still prints them.</summary>
        private static void RecordRefusal(TransportRequestSettings settings, List<ResponseStreamParser> replies)
        {
            if (settings.Transcript == null)
            {
                return;
            }
            var exchange = settings.Transcript.Begin(isHeadRequest: true);
            var segment = exchange.Attach(null, newConnection: false);
            var raw = WireText.Latin1.GetBytes(string.Concat(replies.SelectMany(r => r.Heads).Select(h => h.Text)));
            exchange.Received(segment, raw, 0, raw.Length);
        }

        /// <summary>
        /// The failure for a final non-2xx CONNECT response. Keeps curl's wording
        /// ("CONNECT tunnel failed, response N") and says why when credentials were involved.
        /// </summary>
        private static CurlConnectFailure Refused(Uri proxy, RawResponseHead head, string? authorization, HttpAuth? auth, CurlProxy? owner)
        {
            var status = head.StatusCode;
            var detail = $"CONNECT tunnel failed, response {status}";
            if (status == 407 && owner?.UserCredentials != null)
            {
                var user = owner.UserCredentials.UserName ?? string.Empty;
                if (user.Length > 40)
                {
                    user = user.Substring(0, 40) + "...";
                }
                if (authorization != null)
                {
                    var method = auth?.TunnelMethodName ?? authorization.Split(' ')[0];
                    detail += $"; the proxy rejected the {method} credentials of proxy user \"{user}\" (check the name, password and DOMAIN\\ prefix given with -U/--proxy-user)";
                }
                else
                {
                    detail += $"; CurlDotNet answers a CONNECT's 407 only for --proxy-basic and --proxy-ntlm so far, not for --proxy-{owner.AuthScheme}";
                }
            }
            return new CurlConnectFailure(ConnectFailureKind.Tunnel, proxy.Host, proxy.Port, detail) { TunnelStatus = status };
        }
    }
}
#endif
