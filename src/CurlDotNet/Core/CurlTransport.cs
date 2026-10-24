/***************************************************************************
 * CurlTransport - builds the HttpClient transport the way curl would connect
 *
 * - Proxy selection with curl's rules: -x/--proxy, --socks5, then the http_proxy,
 *   https_proxy/HTTPS_PROXY and all_proxy/ALL_PROXY environment variables, filtered by
 *   --noproxy or no_proxy/NO_PROXY (host suffixes, IP addresses, CIDR ranges, "*").
 * - Proxy credentials from -U/--proxy-user or the proxy URL's user:password, sent
 *   preemptively as Basic unless --proxy-digest/--proxy-ntlm/--proxy-anyauth ask for a challenge.
 * - On .NET 5+: SocketsHttpHandler with a connect callback (per-request --connect-timeout,
 *   --resolve, -p/--proxytunnel CONNECT for http:// URLs, connection timings and endpoints for -w)
 *   and a plaintext stream filter that records the exact response bytes (see WireRecording.cs).
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>What went wrong while opening a connection.</summary>
    internal enum ConnectFailureKind
    {
        /// <summary>The host name did not resolve.</summary>
        Resolve,
        /// <summary>The TCP connection was refused or unreachable.</summary>
        Connect,
        /// <summary>--connect-timeout expired.</summary>
        Timeout,
        /// <summary>The proxy refused the CONNECT tunnel.</summary>
        Tunnel,
        /// <summary>A fresh connection was closed without any reply; HttpClient's silent re-send is refused (curl: exit 52).</summary>
        GotNothing
    }

    /// <summary>
    /// Raised inside the connect callback so the handler can tell "could not resolve" (6/5),
    /// "could not connect" (7) and "connect timeout" (28) apart after HttpClient wraps it.
    /// </summary>
    internal sealed class CurlConnectFailure : IOException
    {
        public CurlConnectFailure(ConnectFailureKind kind, string host, int port, string detail, Exception? inner = null)
            : base(detail, inner)
        {
            Kind = kind;
            Host = host;
            Port = port;
        }

        public ConnectFailureKind Kind { get; }
        public string Host { get; }
        public int Port { get; }

        /// <summary>Status code of a refused CONNECT, when <see cref="Kind"/> is Tunnel.</summary>
        public int TunnelStatus { get; set; }
    }

    /// <summary>
    /// curl's NO_PROXY / --noproxy matching.
    /// </summary>
    internal sealed class NoProxyList
    {
        private readonly List<string> _entries = new List<string>();
        private readonly bool _all;

        public NoProxyList(string? list)
        {
            if (string.IsNullOrWhiteSpace(list))
            {
                return;
            }

            foreach (var raw in list!.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var entry = raw.Trim();
                if (entry == "*")
                {
                    _all = true;
                }
                else if (entry.Length > 0)
                {
                    _entries.Add(entry.TrimStart('.').TrimEnd('.').Trim('[', ']').ToLowerInvariant());
                }
            }
        }

        /// <summary>True when <paramref name="host"/> must not use the proxy.</summary>
        public bool Matches(string host)
        {
            if (_all)
            {
                return true;
            }

            var name = host.Trim('[', ']').TrimEnd('.').ToLowerInvariant();
            IPAddress.TryParse(name, out var hostIp);
            foreach (var entry in _entries)
            {
                var slash = entry.IndexOf('/');
                if (hostIp != null && slash > 0 && IPAddress.TryParse(entry.Substring(0, slash), out var net)
                    && int.TryParse(entry.Substring(slash + 1), NumberStyles.None, CultureInfo.InvariantCulture, out var bits))
                {
                    if (InCidr(hostIp, net, bits)) return true;
                    continue;
                }

                if (hostIp != null && IPAddress.TryParse(entry, out var entryIp))
                {
                    if (hostIp.Equals(entryIp)) return true;
                    continue;
                }

                if (name == entry || name.EndsWith("." + entry, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool InCidr(IPAddress address, IPAddress network, int bits)
        {
            var a = address.GetAddressBytes();
            var n = network.GetAddressBytes();
            if (a.Length != n.Length) return false;
            for (var i = 0; i < a.Length && bits > 0; i++, bits -= 8)
            {
                var mask = bits >= 8 ? 0xFF : (byte)(0xFF << (8 - bits));
                if ((a[i] & mask) != (n[i] & mask)) return false;
            }
            return true;
        }
    }

    /// <summary>
    /// The proxy decision for a command: which proxy (if any) each URL goes through, with which
    /// credentials, and which hosts bypass it.
    /// </summary>
    internal sealed class CurlProxy : IWebProxy
    {
        private readonly Uri? _httpProxy;
        private readonly Uri? _httpsProxy;
        private readonly NoProxyList _noProxy;

        private CurlProxy(Uri? httpProxy, Uri? httpsProxy, NoProxyList noProxy, NetworkCredential? credentials, string? authScheme, bool plainRequests)
        {
            _httpProxy = httpProxy;
            _httpsProxy = httpsProxy;
            _noProxy = noProxy;
            UserCredentials = credentials;
            AuthScheme = string.IsNullOrEmpty(authScheme) ? "basic" : authScheme!.ToLowerInvariant();
            // Requests sent to the proxy as-is (http:// URLs without -p) have their 407 answered by
            // CurlDotNet's own negotiation (Core/Auth), like curl; CONNECT tunnels keep HttpClient's.
            NegotiatesPlainRequests = plainRequests && AuthScheme != "basic" && AuthScheme != "negotiate";
            if (credentials != null && !NegotiatesPlainRequests && (AuthScheme != "basic" || !CurlTransport.SupportsRecording))
            {
                // Challenge schemes are answered by the handler after the proxy's 407. Where CurlDotNet
                // cannot open the CONNECT tunnel itself (.NET Framework/netstandard2.0) Basic is
                // answered that way too, one round trip later than curl.
                Credentials = credentials;
            }
        }

        /// <summary>Credentials for the proxy (from -U/--proxy-user or the proxy URL).</summary>
        public NetworkCredential? UserCredentials { get; }

        /// <summary>
        /// True when the 407 challenges of plain proxy requests (Digest, NTLM, --proxy-anyauth) are
        /// answered by CurlDotNet rather than by HttpClient.
        /// </summary>
        public bool NegotiatesPlainRequests { get; }

        /// <summary>basic, digest, ntlm, negotiate or anyauth.</summary>
        public string AuthScheme { get; }

        /// <inheritdoc />
        public ICredentials? Credentials { get; set; }

        /// <summary>The proxy for <paramref name="destination"/>, or null for a direct connection.</summary>
        public Uri? ProxyFor(Uri destination)
        {
            if (_noProxy.Matches(destination.Host))
            {
                return null;
            }
            return string.Equals(destination.Scheme, "https", StringComparison.OrdinalIgnoreCase) ? _httpsProxy : _httpProxy;
        }

        /// <summary>
        /// True when CurlDotNet opens the CONNECT tunnel for <paramref name="destination"/> itself (an
        /// https URL through an HTTP proxy with Basic or no proxy auth), so the CONNECT request carries
        /// curl's headers and a preemptive Proxy-Authorization. HttpClient then connects "directly"
        /// through the tunnel the connect callback built.
        /// </summary>
        public bool SelfTunnels(Uri destination)
        {
            if (!CurlTransport.SupportsRecording || AuthScheme != "basic"
                || !string.Equals(destination.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            var proxy = ProxyFor(destination);
            return proxy != null && (proxy.Scheme == "http");
        }

        /// <inheritdoc />
        public Uri? GetProxy(Uri destination) => IsBypassed(destination) ? destination : ProxyFor(destination);

        /// <inheritdoc />
        public bool IsBypassed(Uri host) => ProxyFor(host) == null || SelfTunnels(host);

        /// <summary>The Basic <c>Proxy-Authorization</c> value to send preemptively, or null.</summary>
        public string? PreemptiveAuthorization
        {
            get
            {
                if (UserCredentials == null || AuthScheme != "basic")
                {
                    return null;
                }
                var raw = (UserCredentials.UserName ?? string.Empty) + ":" + (UserCredentials.Password ?? string.Empty);
                return "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(raw));
            }
        }

        /// <summary>
        /// Applies curl's proxy rules to <paramref name="options"/>. Returns null when no proxy is
        /// configured anywhere (command line or environment).
        /// </summary>
        /// <exception cref="CurlCouldntResolveProxyException">The proxy string is not a usable address.</exception>
        public static CurlProxy? Resolve(CurlOptions options)
        {
            var noProxyText = options.NoProxy ?? Env("no_proxy") ?? Env("NO_PROXY");
            var noProxy = new NoProxyList(noProxyText);

            string? explicitProxy = null;
            if (options.Proxy != null)
            {
                explicitProxy = options.Proxy;
            }
            else if (!string.IsNullOrEmpty(options.Socks5Proxy))
            {
                explicitProxy = options.Socks5Proxy!.Contains("://") ? options.Socks5Proxy : "socks5://" + options.Socks5Proxy;
            }

            Uri? httpProxy;
            Uri? httpsProxy;
            if (explicitProxy != null)
            {
                if (explicitProxy.Trim().Length == 0)
                {
                    return null; // -x "" disables proxies, including the environment's
                }
                httpProxy = httpsProxy = ParseProxyUrl(explicitProxy, options.OriginalCommand);
            }
            else
            {
                var all = Env("all_proxy") ?? Env("ALL_PROXY");
                // curl deliberately ignores upper-case HTTP_PROXY (CGI "httpoxy" safety)
                var http = Env("http_proxy") ?? all;
                var https = Env("https_proxy") ?? Env("HTTPS_PROXY") ?? all;
                httpProxy = string.IsNullOrWhiteSpace(http) ? null : ParseProxyUrl(http!, options.OriginalCommand);
                httpsProxy = string.IsNullOrWhiteSpace(https) ? null : ParseProxyUrl(https!, options.OriginalCommand);
                if (httpProxy == null && httpsProxy == null)
                {
                    return null;
                }
            }

            var credentials = options.ProxyCredentials ?? CredentialsFromUserInfo(httpProxy ?? httpsProxy);
            var plainRequests = !options.ProxyTunnel && options.Url.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
                && httpProxy != null && (httpProxy.Scheme == "http" || httpProxy.Scheme == "https");
            return new CurlProxy(StripUserInfo(httpProxy), StripUserInfo(httpsProxy), noProxy, credentials,
                options.ProxyAuthScheme ?? options.ProxyAuthMethod, plainRequests);
        }

        /// <summary>True when the command or the environment configures any proxy.</summary>
        public static bool IsConfigured(CurlOptions options)
        {
            return options.Proxy != null
                || !string.IsNullOrEmpty(options.Socks5Proxy)
                || options.NoProxy != null
                || !string.IsNullOrEmpty(Env("http_proxy"))
                || !string.IsNullOrEmpty(Env("https_proxy")) || !string.IsNullOrEmpty(Env("HTTPS_PROXY"))
                || !string.IsNullOrEmpty(Env("all_proxy")) || !string.IsNullOrEmpty(Env("ALL_PROXY"));
        }

        private static string? Env(string name)
        {
            var value = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrEmpty(value) ? null : value;
        }

        /// <summary>
        /// Turns curl's proxy syntax (<c>host</c>, <c>host:port</c>, <c>scheme://user:pass@host:port</c>)
        /// into a URI. curl's default proxy port is 1080.
        /// </summary>
        internal static Uri ParseProxyUrl(string proxy, string? command)
        {
            var text = proxy.Trim();
            if (!text.Contains("://"))
            {
                text = "http://" + text;
            }

            if (!Uri.TryCreate(text, UriKind.Absolute, out var uri) || string.IsNullOrEmpty(uri.Host))
            {
                throw new CurlCouldntResolveProxyException(proxy, command);
            }

            var scheme = uri.Scheme.ToLowerInvariant();
            if (scheme == "socks5h") scheme = "socks5";
            if (scheme == "socks4a") scheme = "socks4a";
            var explicitPort = HasExplicitPort(text);
            var port = explicitPort ? uri.Port : (scheme == "https" ? 443 : 1080);
            var builder = new UriBuilder(uri) { Scheme = scheme, Port = port, Path = string.Empty };
            return builder.Uri;
        }

        private static bool HasExplicitPort(string url)
        {
            var afterScheme = url.Substring(url.IndexOf("://", StringComparison.Ordinal) + 3);
            var at = afterScheme.LastIndexOf('@');
            if (at >= 0) afterScheme = afterScheme.Substring(at + 1);
            var slash = afterScheme.IndexOf('/');
            if (slash >= 0) afterScheme = afterScheme.Substring(0, slash);
            if (afterScheme.StartsWith("[", StringComparison.Ordinal))
            {
                return afterScheme.IndexOf("]:", StringComparison.Ordinal) >= 0;
            }
            return afterScheme.IndexOf(':') >= 0;
        }

        private static NetworkCredential? CredentialsFromUserInfo(Uri? proxy)
        {
            if (proxy == null || string.IsNullOrEmpty(proxy.UserInfo))
            {
                return null;
            }
            var info = proxy.UserInfo;
            var colon = info.IndexOf(':');
            var user = Uri.UnescapeDataString(colon < 0 ? info : info.Substring(0, colon));
            var pass = colon < 0 ? string.Empty : Uri.UnescapeDataString(info.Substring(colon + 1));
            return new NetworkCredential(user, pass);
        }

        private static Uri? StripUserInfo(Uri? proxy)
        {
            if (proxy == null || string.IsNullOrEmpty(proxy.UserInfo))
            {
                return proxy;
            }
            return new UriBuilder(proxy) { UserName = string.Empty, Password = string.Empty }.Uri;
        }
    }

    /// <summary>
    /// Per-request transport settings carried to the connect callback through
    /// <c>HttpRequestMessage.Options</c>.
    /// </summary>
    internal sealed class TransportRequestSettings
    {
        /// <summary>--connect-timeout in seconds (0 = none).</summary>
        public double ConnectTimeoutSeconds { get; set; }

        /// <summary>--resolve entries: "host:port" to address list.</summary>
        public Dictionary<string, string>? Resolve { get; set; }

        /// <summary>True when the request goes through a proxy.</summary>
        public bool ViaProxy { get; set; }

        /// <summary>For -p with an http:// URL: the proxy to tunnel through.</summary>
        public Uri? TunnelProxy { get; set; }

        /// <summary>Basic credentials for the CONNECT request.</summary>
        public string? TunnelAuthorization { get; set; }

        /// <summary>HTTP version for CONNECT ("1.1" or "1.0").</summary>
        public string TunnelVersion { get; set; } = "1.1";

        /// <summary>User-Agent to send on CONNECT.</summary>
        public string? TunnelUserAgent { get; set; }

        /// <summary>Transcript that receives the CONNECT response header block.</summary>
        public WireTranscript? Transcript { get; set; }

        /// <summary>The connection the connect callback opened last for this request.</summary>
        public WireConnectionInfo? LastConnection { get; set; }
    }

    /// <summary>
    /// Creates HttpClient instances whose handlers behave like curl's connection layer.
    /// </summary>
    internal static class CurlTransport
    {
#if !NETSTANDARD2_0 && !NET472 && !NET48
        /// <summary>Key for <see cref="TransportRequestSettings"/> in <c>HttpRequestMessage.Options</c>.</summary>
        internal static readonly HttpRequestOptionsKey<TransportRequestSettings> SettingsKey =
            new HttpRequestOptionsKey<TransportRequestSettings>("CurlDotNet.Transport");
#endif

        /// <summary>True when this build records raw response bytes (.NET 5 and later).</summary>
        internal static bool SupportsRecording
        {
            get
            {
#if NETSTANDARD2_0 || NET472 || NET48
                return false;
#else
                return true;
#endif
            }
        }

        /// <summary>The pooled client shared by requests without per-command transport options.</summary>
        public static HttpClient CreateSharedClient()
        {
            return new HttpClient(CreateHandler(null, null)) { Timeout = Timeout.InfiniteTimeSpan };
        }

        /// <summary>A dedicated client for a command with proxy, TLS, auth or name-resolution options.</summary>
        public static HttpClient CreateClient(CurlOptions options, CurlProxy? proxy)
        {
            return new HttpClient(CreateHandler(options, proxy)) { Timeout = Timeout.InfiniteTimeSpan };
        }

        /// <summary>Attaches per-request settings that the connect callback reads.</summary>
        public static void Attach(HttpRequestMessage request, TransportRequestSettings settings)
        {
#if !NETSTANDARD2_0 && !NET472 && !NET48
            request.Options.Set(SettingsKey, settings);
#endif
        }

        private static HttpMessageHandler CreateHandler(CurlOptions? options, CurlProxy? proxy)
        {
#if NETSTANDARD2_0 || NET472 || NET48
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false, // redirects are followed by RedirectHandler, like curl
                AutomaticDecompression = DecompressionMethods.None, // --compressed is decoded by HttpHandler
                // curl only engages a cookie engine for -b/-c; a CookieContainer would also swallow
                // manually supplied Cookie headers (issue #41)
                UseCookies = false
            };
            if (proxy != null)
            {
                handler.Proxy = proxy;
                handler.UseProxy = true;
            }
            if (options != null)
            {
                ApplyServerAuth(options, c => handler.Credentials = c);
                if (options.Insecure)
                {
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                }
            }
            return handler;
#else
            var handler = new SocketsHttpHandler
            {
                AllowAutoRedirect = false, // redirects are followed by RedirectHandler, like curl
                AutomaticDecompression = DecompressionMethods.None, // --compressed is decoded by HttpHandler
                // curl only engages a cookie engine for -b/-c; a CookieContainer would also swallow
                // manually supplied Cookie headers (issue #41)
                UseCookies = false,
                ConnectCallback = ConnectAsync,
                PlaintextStreamFilter = FilterAsync,
                PooledConnectionIdleTimeout = TimeSpan.FromSeconds(30),
                // curl's own limits (300 KB per transfer, 100 KB per line) are enforced by TransportErrors;
                // HttpClient's 64 KB default would fail earlier and less precisely.
                MaxResponseHeadersLength = 2048,
            };
            if (proxy != null)
            {
                handler.Proxy = proxy;
                handler.UseProxy = true;
            }
            if (options != null)
            {
                ApplyServerAuth(options, c => handler.Credentials = c);
                handler.PreAuthenticate = false;
                if (options.Insecure)
                {
                    handler.SslOptions.RemoteCertificateValidationCallback = (sender, cert, chain, errors) => true;
                }
                if (options.Expect100TimeoutSeconds.HasValue && options.Expect100TimeoutSeconds.Value >= 0)
                {
                    handler.Expect100ContinueTimeout = TimeSpan.FromSeconds(options.Expect100TimeoutSeconds.Value);
                }
            }
            return handler;
#endif
        }

        /// <summary>
        /// Challenge-based server auth (--digest, --ntlm, --negotiate, --anyauth): the handler answers
        /// the 401 itself, so it needs the credentials (Basic is sent preemptively by CreateRequest).
        /// </summary>
        private static void ApplyServerAuth(CurlOptions options, Action<ICredentials> set)
        {
            // Basic, Digest, NTLM and --anyauth are negotiated by CurlDotNet itself (Core/Auth), exactly
            // like curl; only Negotiate (Kerberos/SPNEGO) is left to the platform's HttpClient.
            if (options.Credentials == null || !string.Equals(options.AuthScheme, "negotiate", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (!Uri.TryCreate(options.Url, UriKind.Absolute, out var authUri)
                || (authUri.Scheme != Uri.UriSchemeHttp && authUri.Scheme != Uri.UriSchemeHttps))
            {
                set(options.Credentials!);
                return;
            }

            var cache = new CredentialCache();
            var scheme = options.AuthScheme.ToLowerInvariant() switch
            {
                "digest" => "Digest",
                "ntlm" => "NTLM",
                _ => "Negotiate"
            };
            cache.Add(new Uri(authUri, "/"), scheme, options.Credentials!);
            set(cache);
        }

#if !NETSTANDARD2_0 && !NET472 && !NET48
        private static async ValueTask<Stream> ConnectAsync(SocketsHttpConnectionContext context, CancellationToken cancellationToken)
        {
            context.InitialRequestMessage.Options.TryGetValue(SettingsKey, out var settings);

            // HttpClient re-sends a request up to three times when the server closes the connection
            // without a byte of reply. curl only does that for a reused connection; after a fresh
            // connection said nothing it reports "Empty reply from server" (52).
            var previous = settings?.Transcript?.Exchanges.LastOrDefault()?.Segments.LastOrDefault();
            if (previous != null && previous.NewConnection && previous.BytesSent > 0 && previous.Parser.TotalBytes == 0)
            {
                throw new CurlConnectFailure(ConnectFailureKind.GotNothing, context.DnsEndPoint.Host, context.DnsEndPoint.Port,
                    "the server closed the connection without sending a reply");
            }
            var info = new WireConnectionInfo { StartedTimestamp = Stopwatch.GetTimestamp() };

            var tunnel = settings?.TunnelProxy;
            var host = tunnel?.Host ?? context.DnsEndPoint.Host;
            var port = tunnel?.Port ?? context.DnsEndPoint.Port;

            using var timeoutCts = settings != null && settings.ConnectTimeoutSeconds > 0
                ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)
                : null;
            timeoutCts?.CancelAfter(TimeSpan.FromSeconds(settings!.ConnectTimeoutSeconds));
            var token = timeoutCts?.Token ?? cancellationToken;

            IPAddress[] addresses;
            try
            {
                addresses = Lookup(settings, host, port) ?? await Dns.GetHostAddressesAsync(host, token).ConfigureAwait(false);
            }
            catch (SocketException ex)
            {
                throw new CurlConnectFailure(ConnectFailureKind.Resolve, host, port, ex.Message, ex);
            }
            catch (OperationCanceledException) when (timeoutCts != null && timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                throw new CurlConnectFailure(ConnectFailureKind.Timeout, host, port, "Resolving timed out");
            }

            if (addresses.Length == 0)
            {
                throw new CurlConnectFailure(ConnectFailureKind.Resolve, host, port, "No address associated with hostname");
            }
            info.ResolvedTimestamp = Stopwatch.GetTimestamp();

            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
            try
            {
                await socket.ConnectAsync(addresses, port, token).ConfigureAwait(false);
            }
            catch (SocketException ex)
            {
                socket.Dispose();
                throw new CurlConnectFailure(ConnectFailureKind.Connect, host, port, ex.Message, ex);
            }
            catch (OperationCanceledException) when (timeoutCts != null && timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                socket.Dispose();
                throw new CurlConnectFailure(ConnectFailureKind.Timeout, host, port, "Connection timed out");
            }
            catch
            {
                socket.Dispose();
                throw;
            }

            info.ConnectedTimestamp = Stopwatch.GetTimestamp();
            if (settings != null)
            {
                settings.LastConnection = info;
            }
            info.LocalEndPoint = Normalize(socket.LocalEndPoint as IPEndPoint);
            info.RemoteEndPoint = Normalize(socket.RemoteEndPoint as IPEndPoint);
            var stream = new InfoNetworkStream(socket, info);

            if (tunnel != null)
            {
                try
                {
                    await OpenTunnelAsync(stream, context.DnsEndPoint, settings!, token).ConfigureAwait(false);
                }
                catch
                {
                    stream.Dispose();
                    throw;
                }
            }

            return stream;
        }

        /// <summary>
        /// -p/--proxytunnel for an http:// URL: sends CONNECT to the proxy on the fresh socket and
        /// returns once the proxy answered 2xx. The response header block is recorded so -i prints it.
        /// </summary>
        private static async Task OpenTunnelAsync(Stream stream, DnsEndPoint target, TransportRequestSettings settings, CancellationToken token)
        {
            var authority = target.Host.Contains(":") ? "[" + target.Host + "]:" + target.Port : target.Host + ":" + target.Port;
            var sb = new StringBuilder();
            sb.Append("CONNECT ").Append(authority).Append(" HTTP/").Append(settings.TunnelVersion).Append("\r\n");
            sb.Append("Host: ").Append(authority).Append("\r\n");
            if (settings.TunnelAuthorization != null)
            {
                sb.Append("Proxy-Authorization: ").Append(settings.TunnelAuthorization).Append("\r\n");
            }
            if (!string.IsNullOrEmpty(settings.TunnelUserAgent))
            {
                sb.Append("User-Agent: ").Append(settings.TunnelUserAgent).Append("\r\n");
            }
            sb.Append("Proxy-Connection: Keep-Alive\r\n\r\n");
            var bytes = Encoding.ASCII.GetBytes(sb.ToString());
            await stream.WriteAsync(bytes, 0, bytes.Length, token).ConfigureAwait(false);
            await stream.FlushAsync(token).ConfigureAwait(false);

            var parser = new ResponseStreamParser(isHeadRequest: true);
            var buffer = new byte[1];
            while (parser.Heads.Count == 0 || parser.Heads[parser.Heads.Count - 1].IsInformational)
            {
                var n = await stream.ReadAsync(buffer, 0, 1, token).ConfigureAwait(false);
                if (n <= 0)
                {
                    throw new CurlConnectFailure(ConnectFailureKind.Tunnel, settings.TunnelProxy!.Host, settings.TunnelProxy.Port,
                        "Proxy closed the connection while answering CONNECT");
                }
                parser.Feed(buffer, 0, 1);
            }

            var head = parser.Heads[parser.Heads.Count - 1];
            if (settings.Transcript != null)
            {
                var exchange = settings.Transcript.Begin(isHeadRequest: true);
                var segment = exchange.Attach(null, newConnection: false);
                var raw = WireText.Latin1.GetBytes(string.Concat(parser.Heads.Select(h => h.Text)));
                exchange.Received(segment, raw, 0, raw.Length);
            }

            if (head.StatusCode < 200 || head.StatusCode > 299)
            {
                throw new CurlConnectFailure(ConnectFailureKind.Tunnel, settings.TunnelProxy!.Host, settings.TunnelProxy.Port,
                    $"CONNECT tunnel failed, response {head.StatusCode}") { TunnelStatus = head.StatusCode };
            }
        }

        private static IPAddress[]? Lookup(TransportRequestSettings? settings, string host, int port)
        {
            var bare = host.Trim('[', ']');
            if (IPAddress.TryParse(bare, out var literal))
            {
                return new[] { literal };
            }

            if (settings?.Resolve == null)
            {
                return null;
            }

            foreach (var entry in settings.Resolve)
            {
                // "host:port" (or "*:port") -> "addr[,addr...]"
                var colon = entry.Key.LastIndexOf(':');
                if (colon <= 0) continue;
                var name = entry.Key.Substring(0, colon).TrimStart('+');
                var entryPort = entry.Key.Substring(colon + 1);
                if (entryPort != port.ToString(CultureInfo.InvariantCulture)) continue;
                if (name != "*" && !string.Equals(name.TrimEnd('.'), bare.TrimEnd('.'), StringComparison.OrdinalIgnoreCase)) continue;

                var addresses = new List<IPAddress>();
                foreach (var part in entry.Value.Split(','))
                {
                    if (IPAddress.TryParse(part.Trim().Trim('[', ']'), out var ip))
                    {
                        addresses.Add(ip);
                    }
                }
                if (addresses.Count > 0)
                {
                    return addresses.ToArray();
                }
            }
            return null;
        }

        private static IPEndPoint? Normalize(IPEndPoint? endPoint)
        {
            if (endPoint != null && endPoint.Address.IsIPv4MappedToIPv6)
            {
                return new IPEndPoint(endPoint.Address.MapToIPv4(), endPoint.Port);
            }
            return endPoint;
        }

        private static ValueTask<Stream> FilterAsync(SocketsHttpPlaintextStreamFilterContext context, CancellationToken cancellationToken)
        {
            if (context.NegotiatedHttpVersion.Major >= 2)
            {
                return new ValueTask<Stream>(context.PlaintextStream);
            }

            WireConnectionInfo? info = null;
            if (context.PlaintextStream is InfoNetworkStream plain)
            {
                info = plain.Info;
            }
            else if (context.InitialRequestMessage.Options.TryGetValue(SettingsKey, out var settings) && settings.LastConnection != null)
            {
                // TLS wraps the socket stream, so the connect callback left the facts on the request settings
                info = settings.LastConnection;
                info.Secure = true;
            }

            if (info != null)
            {
                info.ReadyTimestamp = Stopwatch.GetTimestamp();
            }

            return new ValueTask<Stream>(new RecordingStream(context.PlaintextStream, info));
        }
#endif
    }
}
