/***************************************************************************
 * Comprehensive curl exception hierarchy
 *
 * Every curl error code gets its own exception type for precise catching
 * Based on curl's libcurl/include/curl/curl.h error codes
 * Original curl Copyright (C) 1996-2025, Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024-2025 Jacob Mellor and IronSoftware
 *
 * Every message has three parts: what happened (first line, curl's wording where curl has one),
 * how to fix it, and a "See <docs URL>" line. Constructors that receive a caller's message rely on
 * DefaultSuggestion / DefaultDocumentationUrl for the fix and the link when the message has none.
 *
 * By Jacob Mellor
 * Sponsored by IronSoftware
 ***************************************************************************/

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace CurlDotNet.Exceptions
{
    /// <summary>
    /// The URL uses a scheme CurlDotNet does not speak (exit code 1, CURLE_UNSUPPORTED_PROTOCOL), a redirect points
    /// at such a scheme, or the server answered with an HTTP version curl does not accept.
    /// </summary>
    /// <remarks>
    /// <para>CurlDotNet handles <c>http://</c>, <c>https://</c>, <c>ftp://</c>, <c>ftps://</c> and <c>file://</c>. It throws
    /// this type when the URL's scheme is something else (<c>gopher://</c>, <c>ldap://</c>, or a typo such as
    /// <c>htps://</c>), when <c>-L</c> meets a <c>Location:</c> header that points at a non-HTTP scheme, and when a
    /// server's status line announces an HTTP version curl refuses (HTTP/0.9 without <c>--http0.9</c>, <c>HTTP/1.2</c>).
    /// <see cref="Protocol"/> holds the scheme or version.</para>
    /// <para>Example: <c>catch (CurlUnsupportedProtocolException ex) { Console.WriteLine($"Cannot fetch {ex.Protocol} URLs"); }</c></para>
    /// <para>Supported protocols: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/supported-protocols.md</para>
    /// </remarks>
    /// <seealso cref="CurlMalformedUrlException"/>
    /// <seealso cref="CurlNotSupportedException"/>
    [Serializable]
    public class CurlUnsupportedProtocolException : CurlException
    {
        private enum Kind { Scheme, Redirect, ServerReply }

        private readonly Kind _kind;

        /// <summary>
        /// The scheme or protocol version that is not supported, such as <c>gopher</c>, <c>htps</c> or <c>HTTP/1.2</c>.
        /// </summary>
        /// <value>The scheme without <c>://</c>, or the status-line version for a server reply.</value>
        /// <remarks><para>Example: <c>if (ex.Protocol == "htps") Console.WriteLine("Typo: use https://");</c></para></remarks>
        /// <seealso cref="CurlException.CurlErrorCode"/>
        public string Protocol { get; }

        /// <summary>
        /// Creates the error for a URL whose scheme is not supported, with curl's wording
        /// <c>Protocol "gopher" not supported</c> and advice to fix the scheme.
        /// </summary>
        /// <remarks>
        /// <para>Thrown before connecting. Example: <c>curl gopher://example.com/</c> or a typo like <c>htps://</c>.
        /// The fix lists the schemes CurlDotNet supports.</para>
        /// </remarks>
        /// <param name="protocol">The scheme from the URL that is not supported, such as <c>gopher</c>.</param>
        /// <param name="command">The curl command that was executing when the error occurred, or null.</param>
        /// <seealso cref="CurlUnsupportedProtocolException(string, string, bool)"/>
        public CurlUnsupportedProtocolException(string protocol, string command = null)
            : base($"Protocol \"{protocol}\" not supported (unsupported protocol: {protocol})", 1, command)
        {
            Protocol = protocol;
            _kind = Kind.Scheme;
        }

        /// <summary>
        /// Creates the error for a redirect (<c>Location:</c> header) that points at a scheme curl refuses to follow,
        /// such as <c>file://</c> or <c>gopher://</c>.
        /// </summary>
        /// <remarks>
        /// <para>With <c>-L</c>, curl only follows redirects to <c>http://</c> and <c>https://</c>; a server sending
        /// <c>Location: gopher://...</c> or <c>file:///etc/passwd</c> ends the transfer with exit code 1 instead of
        /// silently switching protocols. The fix says to fetch the target explicitly if you trust it.</para>
        /// </remarks>
        /// <param name="protocol">The scheme of the redirect target, such as <c>gopher</c>.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <param name="redirect">Must be true; distinguishes this overload from the others.</param>
        /// <seealso cref="CurlTooManyRedirectsException"/>
        public CurlUnsupportedProtocolException(string protocol, string? command, bool redirect)
            : base(CurlException.Compose(
                       $"Protocol \"{protocol}\" not supported for redirects: the server's Location header points to a {protocol}:// URL, and curl only follows redirects to http:// and https://.",
                       "Fetch that URL explicitly if you trust it, or drop -L to see the redirect response itself.",
                       DocLinks.Redirect),
                   1, command)
        {
            Protocol = protocol;
            _kind = Kind.Redirect;
        }

        /// <summary>
        /// Creates the error for a server reply whose status line uses a protocol version curl does not speak
        /// (HTTP/0.9 without <c>--http0.9</c>, <c>HTTP/1.2</c>, or a malformed status code).
        /// </summary>
        /// <remarks>
        /// <para>The <paramref name="detail"/> text comes from the response validator and already explains the problem;
        /// the fix points at <c>--http0.9</c> and at checking the port.</para>
        /// </remarks>
        /// <param name="protocol">What the server announced, such as <c>HTTP/1.2</c> or <c>HTTP/0.9</c>.</param>
        /// <param name="detail">Explanation (and optionally a fix), already worded for the reader.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlWeirdServerReplyException"/>
        public CurlUnsupportedProtocolException(string protocol, string detail, string? command)
            : base(detail, 1, command)
        {
            Protocol = protocol;
            _kind = Kind.ServerReply;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl =>
            _kind == Kind.ServerReply ? DocLinks.Http : _kind == Kind.Redirect ? DocLinks.Redirect : DocLinks.SupportedProtocols;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion => _kind == Kind.ServerReply
            ? $"The server answered with {Protocol}, which curl does not accept: check that the port really serves HTTP, or add --http0.9 if it is a legacy HTTP/0.9 server."
            : $"Check the URL's scheme for a typo (\"{Protocol}\"): CurlDotNet handles http://, https://, ftp://, ftps:// and file://; use the native curl tool for other protocols.";
    }

    /// <summary>
    /// The transfer could not be set up at all (exit code 2, CURLE_FAILED_INIT), for example because a required
    /// option value or local resource could not be prepared before connecting.
    /// </summary>
    /// <remarks>
    /// <para>Nothing was sent. The message names what could not be initialised; fix that option or resource and run the
    /// command again. Example: <c>catch (CurlFailedInitException ex) { Console.WriteLine(ex.WhatHappened); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/init-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlInvalidCommandException"/>
    [Serializable]
    public class CurlFailedInitException : CurlException
    {
        /// <summary>
        /// Creates an initialisation failure (exit code 2) with a message naming what could not be prepared before the
        /// transfer started.
        /// </summary>
        /// <remarks>
        /// <para>When <paramref name="message"/> has no fix, the suggestion tells the reader to check the options that need
        /// local resources. Example: <c>throw new CurlFailedInitException("Client certificate store could not be opened.", command);</c></para>
        /// </remarks>
        /// <param name="message">What could not be initialised; optionally followed by fix lines and a <c>See URL</c> line.</param>
        /// <param name="command">The curl command that was executing when the error occurred, or null.</param>
        /// <seealso cref="CurlBadFunctionArgumentException"/>
        public CurlFailedInitException(string message, string command = null)
            : base(message, 2, command) { }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Init;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "Check the options that need local resources (certificate and key files, --interface, --unix-socket, config files): the path must exist and be readable, and the value must be valid on this platform.";
    }

    /// <summary>
    /// The URL could not be parsed (exit code 3, CURLE_URL_MALFORMAT): missing scheme or host, a bad port, spaces
    /// or illegal characters, or a redirect <c>Location</c> that is not a valid URL.
    /// </summary>
    /// <remarks>
    /// <para>Thrown before connecting for the command's URL, and while following <c>-L</c> redirects for a broken
    /// <c>Location</c> header. <see cref="MalformedUrl"/> holds the URL as given.</para>
    /// <para>Fixes: include the scheme (<c>https://</c>), percent-encode spaces (<c>%20</c>), quote the URL when it contains
    /// <c>&amp;</c>, <c>?</c> or <c>[ ]</c> (or use <c>-g</c> to turn off globbing), and keep ports between 1 and 65535.
    /// Example: <c>catch (CurlMalformedUrlException ex) { Console.WriteLine($"Bad URL: {ex.MalformedUrl}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlUnsupportedProtocolException"/>
    /// <seealso cref="CurlInvalidCommandException"/>
    [Serializable]
    public class CurlMalformedUrlException : CurlException
    {
        /// <summary>
        /// The URL that could not be parsed, exactly as it appeared in the command or in the redirect's <c>Location</c>
        /// header.
        /// </summary>
        /// <value>The raw URL text; it may contain credentials, so mask it before logging.</value>
        /// <remarks><para>Example: <c>Console.WriteLine(Uri.EscapeUriString(ex.MalformedUrl));</c> shows what an encoded version would look like.</para></remarks>
        /// <seealso cref="CurlException.Command"/>
        public string MalformedUrl { get; }

        /// <summary>
        /// Creates the error for a URL from the command that cannot be parsed, with advice on scheme, encoding and
        /// quoting.
        /// </summary>
        /// <remarks>
        /// <para>Message: <c>Malformed URL: URL</c>, the fix, and the URL-errors page.
        /// Example: <c>throw new CurlMalformedUrlException("http://exa mple.com/", command);</c></para>
        /// </remarks>
        /// <param name="url">The malformed URL that caused the error, as given.</param>
        /// <param name="command">The curl command that was executing when the error occurred, or null.</param>
        /// <seealso cref="CurlMalformedUrlException(string, string, string)"/>
        public CurlMalformedUrlException(string url, string command = null)
            : base($"Malformed URL: {url}", 3, command)
        {
            MalformedUrl = url;
        }

        /// <summary>
        /// Creates the error with an explanation of what is wrong with the URL, for URLs that did not come from the
        /// command line (such as a redirect's <c>Location</c> header).
        /// </summary>
        /// <remarks>
        /// <para>The <paramref name="detail"/> explains the defect, for example "the port 99999 is out of range".</para>
        /// </remarks>
        /// <param name="url">The malformed URL, as received.</param>
        /// <param name="detail">What is wrong with it and, optionally, how to fix it.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlMalformedUrlException(string, string)"/>
        public CurlMalformedUrlException(string url, string detail, string? command)
            : base($"Malformed URL: {detail}", 3, command)
        {
            MalformedUrl = url;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Url;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Check '{ErrorText.Preview(MalformedUrl, 120)}': it needs a scheme such as https://, a host, a port between 1 and 65535 if any, and %20 instead of spaces; quote it when it contains & or ?, and add -g when it contains [ ] or {{ }}.";
    }

    /// <summary>
    /// The command asks for a curl feature that this build does not include (exit code 4, CURLE_NOT_BUILT_IN);
    /// <see cref="Feature"/> names it.
    /// </summary>
    /// <remarks>
    /// <para>curl compiled without a feature reports exit code 4; CurlDotNet uses the same code for curl features it does not
    /// implement on the current platform. Remove the option, or run the command with the native curl tool.
    /// Example: <c>catch (CurlNotBuiltInException ex) { Console.WriteLine($"{ex.Feature} needs native curl"); }</c></para>
    /// <para>Compatibility list: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/reference/curl-cli-compatibility.md</para>
    /// </remarks>
    /// <seealso cref="CurlNotSupportedException"/>
    [Serializable]
    public class CurlNotBuiltInException : CurlException
    {
        /// <summary>
        /// The feature or option that is not available in this build or platform, such as <c>--http3</c> on .NET
        /// Standard 2.0.
        /// </summary>
        /// <value>The feature name as passed to the constructor.</value>
        /// <remarks><para>Use it to decide whether to fall back to another option.</para></remarks>
        /// <seealso cref="CurlNotSupportedException.Feature"/>
        public string Feature { get; }

        /// <summary>
        /// Creates the "feature not built in" error (exit code 4) for <paramref name="feature"/>, with advice to remove it
        /// or use the native curl tool.
        /// </summary>
        /// <remarks>
        /// <para>Message: <c>Feature not available: FEATURE</c>. Example: <c>throw new CurlNotBuiltInException("--http3 on .NET Standard 2.0", command);</c></para>
        /// </remarks>
        /// <param name="feature">The feature that is not available in this build, such as an option name.</param>
        /// <param name="command">The curl command that was executing when the error occurred, or null.</param>
        /// <seealso cref="CurlNotSupportedException(string, string)"/>
        public CurlNotBuiltInException(string feature, string command = null)
            : base($"Feature not available: {feature}", 4, command)
        {
            Feature = feature;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.CliCompatibility;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"{Feature} is not part of CurlDotNet on this platform: remove it from the command, use a newer target framework (.NET 8 or later supports more transport options), or run the native curl tool.";
    }

    /// <summary>
    /// The proxy's host name could not be resolved (exit code 5, CURLE_COULDNT_RESOLVE_PROXY), so no request was sent.
    /// </summary>
    /// <remarks>
    /// <para>The proxy comes from <c>-x/--proxy</c> or the <c>http_proxy</c>, <c>https_proxy</c>, <c>all_proxy</c> environment
    /// variables. <see cref="ProxyHost"/> names it. Fixes: correct the proxy host, check DNS/VPN, or bypass the proxy with
    /// <c>--noproxy HOST</c> (or <c>--noproxy '*'</c>).
    /// Example: <c>catch (CurlCouldntResolveProxyException ex) { Console.WriteLine($"Proxy {ex.ProxyHost} unknown"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlProxyException"/>
    /// <seealso cref="CurlCouldntResolveHostException"/>
    [Serializable]
    public class CurlCouldntResolveProxyException : CurlException
    {
        /// <summary>
        /// The proxy host name that could not be resolved, as given with <c>-x</c> or in a proxy environment variable.
        /// </summary>
        /// <value>A host name such as <c>proxy.corp.local</c>.</value>
        /// <remarks><para>Example: <c>Console.WriteLine($"https_proxy={Environment.GetEnvironmentVariable("https_proxy")}");</c> shows where it came from.</para></remarks>
        /// <seealso cref="CurlProxyException.ProxyHost"/>
        public string ProxyHost { get; }

        /// <summary>
        /// Creates the error with curl's wording <c>Could not resolve proxy: HOST</c> and advice to check the proxy
        /// setting.
        /// </summary>
        /// <remarks><para>Example: <c>throw new CurlCouldntResolveProxyException("proxy.corp.local", command);</c></para></remarks>
        /// <param name="proxyHost">The proxy hostname that could not be resolved.</param>
        /// <param name="command">The curl command that was executing when the error occurred, or null.</param>
        /// <seealso cref="CurlCouldntResolveProxyException(string, string, string)"/>
        public CurlCouldntResolveProxyException(string proxyHost, string command = null)
            : base($"Could not resolve proxy: {proxyHost}", 5, command)
        {
            ProxyHost = proxyHost;
        }

        /// <summary>
        /// Creates the error with the resolver's explanation of why the proxy name failed, for example "nodename nor
        /// servname provided".
        /// </summary>
        /// <remarks><para>The resolver's text is shown in parentheses after the proxy name.</para></remarks>
        /// <param name="proxyHost">The proxy host name (from <c>-x</c>/<c>--proxy</c> or a proxy environment variable).</param>
        /// <param name="detail">What the resolver reported, in its own words.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlCouldntResolveProxyException(string, string)"/>
        public CurlCouldntResolveProxyException(string proxyHost, string detail, string? command)
            : base($"Could not resolve proxy: {proxyHost} ({detail})", 5, command)
        {
            ProxyHost = proxyHost;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.HowToProxy;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Check the proxy host '{ProxyHost}' given with -x/--proxy (or the http_proxy, https_proxy and all_proxy environment variables) for typos, or use --noproxy HOST to reach the host directly.";
    }

    /// <summary>
    /// The host name in the URL could not be resolved to an IP address (exit code 6, CURLE_COULDNT_RESOLVE_HOST):
    /// a typo, a name that does not exist, or no DNS (offline, VPN down).
    /// </summary>
    /// <remarks>
    /// <para>Thrown by the transport before any connection is made. <see cref="Hostname"/> holds the name. Fixes: check
    /// the spelling, run <c>nslookup NAME</c> on the same machine, check VPN or corporate DNS, or pin the name with
    /// <c>--resolve NAME:443:1.2.3.4</c>. Retrying helps only for temporary DNS outages
    /// (<see cref="CurlException.IsRetryable"/> is true).</para>
    /// <para>Example: <c>catch (CurlCouldntResolveHostException ex) { Console.WriteLine($"Unknown host {ex.Hostname} - check the URL"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlCouldntConnectException"/>
    /// <seealso cref="CurlDnsException"/>
    [Serializable]
    public class CurlCouldntResolveHostException : CurlException
    {
        /// <summary>
        /// The host name from the URL that could not be resolved, such as <c>api.exmaple.com</c>.
        /// </summary>
        /// <value>The host part of the URL, without scheme or port.</value>
        /// <remarks><para>Example: <c>Dns.GetHostAddresses(ex.Hostname)</c> reproduces the lookup outside CurlDotNet.</para></remarks>
        /// <seealso cref="CurlDnsException"/>
        public string Hostname { get; }

        /// <summary>
        /// Creates the error with curl's wording <c>Could not resolve host: NAME</c> and advice on spelling, DNS and
        /// <c>--resolve</c>.
        /// </summary>
        /// <remarks><para>Example: <c>throw new CurlCouldntResolveHostException("api.exmaple.com", command);</c></para></remarks>
        /// <param name="hostname">The hostname that could not be resolved.</param>
        /// <param name="command">The curl command that was executing when the error occurred, or null.</param>
        /// <seealso cref="CurlCouldntResolveHostException(string, string, string)"/>
        public CurlCouldntResolveHostException(string hostname, string command = null)
            : base($"Could not resolve host: {hostname}", 6, command)
        {
            Hostname = hostname;
        }

        /// <summary>
        /// Creates the error with the resolver's explanation of why the name failed, such as "No such host is known".
        /// </summary>
        /// <remarks><para>Thrown by the transport; the resolver's text is shown in parentheses after the host name.</para></remarks>
        /// <param name="hostname">The host name from the URL.</param>
        /// <param name="detail">What the resolver reported, in its own words.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlCouldntResolveHostException(string, string)"/>
        public CurlCouldntResolveHostException(string hostname, string detail, string? command)
            : base($"Could not resolve host: {hostname} ({detail})", 6, command)
        {
            Hostname = hostname;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Dns;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Check the spelling of the host in the URL and your DNS settings (try nslookup {Hostname}); --resolve {Hostname}:443:ADDRESS pins a name to an address without DNS.";
    }

    /// <summary>
    /// The TCP connection to the server or proxy failed (exit code 7, CURLE_COULDNT_CONNECT): refused, unreachable,
    /// or blocked; <see cref="Host"/>, <see cref="Port"/> and <see cref="ViaProxy"/> say where.
    /// </summary>
    /// <remarks>
    /// <para>The name resolved but nothing accepted the connection. Usual causes: the service is not running or listens on
    /// another port, a firewall drops the traffic, the URL uses <c>http</c> on an <c>https</c>-only port (or the reverse),
    /// or a proxy is down. Retrying may help (<c>--retry 3 --retry-connrefused</c>).</para>
    /// <para>Example: <c>catch (CurlCouldntConnectException ex) when (ex.ViaProxy) { Console.WriteLine($"Proxy {ex.Host}:{ex.Port} is down"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlCouldntResolveHostException"/>
    /// <seealso cref="CurlOperationTimeoutException"/>
    [Serializable]
    public class CurlCouldntConnectException : CurlException
    {
        /// <summary>
        /// The host that refused or did not answer: the URL's host, or the proxy's host when <see cref="ViaProxy"/> is
        /// true.
        /// </summary>
        /// <value>A host name or IP address such as <c>127.0.0.1</c>.</value>
        /// <remarks><para>Example: <c>Console.WriteLine($"Is anything listening on {ex.Host}:{ex.Port}?");</c></para></remarks>
        /// <seealso cref="Port"/>
        public string Host { get; }

        /// <summary>
        /// The TCP port that was tried: the URL's port (80/443 by default) or the proxy's port when
        /// <see cref="ViaProxy"/> is true.
        /// </summary>
        /// <value>A port number from 1 to 65535.</value>
        /// <remarks><para>A refused connection on a non-standard port usually means the service listens elsewhere.</para></remarks>
        /// <seealso cref="Host"/>
        public int Port { get; }

        /// <summary>
        /// Creates the error with curl's wording <c>Failed to connect to HOST port PORT</c> and advice to check the
        /// service, port and firewall.
        /// </summary>
        /// <remarks><para>Example: <c>throw new CurlCouldntConnectException("db.internal", 5432, command);</c></para></remarks>
        /// <param name="host">The host that could not be connected to.</param>
        /// <param name="port">The port number that was attempted.</param>
        /// <param name="command">The curl command that was executing when the error occurred, or null.</param>
        /// <seealso cref="CurlCouldntConnectException(string, int, string, bool, string)"/>
        public CurlCouldntConnectException(string host, int port, string command = null)
            : base($"Failed to connect to {host} port {port}", 7, command)
        {
            Host = host;
            Port = port;
        }

        /// <summary>
        /// Creates the error with the socket layer's reason and whether the unreachable host was the server or the proxy,
        /// which changes the advice.
        /// </summary>
        /// <remarks>
        /// <para>When a proxy is in use (<c>-x</c>, <c>--socks5</c> or a proxy environment variable) curl connects to the
        /// proxy, not to the URL's host, so <paramref name="host"/> and <paramref name="port"/> name the proxy and
        /// <see cref="ViaProxy"/> is true.</para>
        /// </remarks>
        /// <param name="host">The host that refused or did not answer (the proxy when <paramref name="viaProxy"/> is true).</param>
        /// <param name="port">The port that was tried.</param>
        /// <param name="detail">What the socket layer reported, such as <c>Connection refused</c>.</param>
        /// <param name="viaProxy">True when <paramref name="host"/> is the proxy rather than the URL's host.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlCouldntConnectException(string, int, string)"/>
        public CurlCouldntConnectException(string host, int port, string detail, bool viaProxy, string? command)
            : base(viaProxy
                    ? $"Failed to connect to proxy {host} port {port}: {detail}"
                    : $"Failed to connect to {host} port {port}: {detail}",
                   7, command)
        {
            Host = host;
            Port = port;
            ViaProxy = viaProxy;
        }

        /// <summary>
        /// True when the failed connection was to a proxy rather than to the URL's host, so the proxy setting (not the
        /// server) needs fixing.
        /// </summary>
        /// <value>True for proxy connections (<c>-x</c>, <c>--socks5</c>, proxy environment variables); otherwise false.</value>
        /// <remarks><para>Example: <c>catch (CurlCouldntConnectException ex) when (ex.ViaProxy) { /* try without the proxy: --noproxy '*' */ }</c></para></remarks>
        /// <seealso cref="CurlCouldntResolveProxyException"/>
        public bool ViaProxy { get; }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Dns;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion => ViaProxy
            ? $"Check that the proxy {Host}:{Port} given with -x/--proxy (or the http_proxy/https_proxy/all_proxy environment variable) is running and reachable, or add the target host to --noproxy to connect directly."
            : $"Check that the server is running and listening on {Host} port {Port}, that the host and port in the URL are right (http vs https), and that no firewall blocks the connection; --retry 3 --retry-connrefused retries refused connections.";
    }

    /// <summary>
    /// The server's reply broke the protocol's rules (exit code 8, CURLE_WEIRD_SERVER_REPLY): an invalid status line,
    /// malformed or smuggling-prone headers, or bytes that are not HTTP at all.
    /// </summary>
    /// <remarks>
    /// <para>Thrown while reading the response. Common causes: talking HTTP to a non-HTTP port (a database, SSH, or TLS on
    /// a plain-http URL), a buggy server or proxy, or header combinations curl rejects to prevent response smuggling
    /// (duplicate <c>Content-Length</c>, NUL bytes, bad chunk trailers). There is no client option that accepts such a
    /// reply; fix the server or the URL.</para>
    /// <para>Example: <c>catch (CurlWeirdServerReplyException ex) { Console.WriteLine("Not an HTTP server? " + ex.WhatHappened); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlUnsupportedProtocolException"/>
    /// <seealso cref="CurlReceiveErrorException"/>
    [Serializable]
    public class CurlWeirdServerReplyException : CurlException
    {
        /// <summary>
        /// Creates the error with a description of what was wrong with the reply, such as "the status line is not
        /// HTTP/1.x".
        /// </summary>
        /// <remarks>
        /// <para>When <paramref name="message"/> has no fix, the suggestion tells the reader to check the port and scheme and
        /// to inspect the reply with <c>-v</c>. Example: <c>throw new CurlWeirdServerReplyException($"Weird server reply from {host}: two Content-Length headers.", command);</c></para>
        /// </remarks>
        /// <param name="message">What was wrong with the reply, naming the host; optionally followed by fix lines.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlReceiveErrorException"/>
        public CurlWeirdServerReplyException(string message, string command = null)
            : base(message, 8, command) { }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Http;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "Check that the URL's scheme and port point at an HTTP server (http vs https, the right port), and inspect the raw reply with -v or --trace-ascii -; a reply that breaks HTTP framing must be fixed on the server.";
    }

    /// <summary>
    /// The server refused access to the remote resource (exit code 9, CURLE_REMOTE_ACCESS_DENIED), typically an FTP
    /// login or directory the account may not use; <see cref="Resource"/> names it.
    /// </summary>
    /// <remarks>
    /// <para>Fixes: check the credentials (<c>-u user:password</c>), the remote path (case-sensitive) and the account's
    /// permissions. Example: <c>catch (CurlRemoteAccessDeniedException ex) { Console.WriteLine($"No access to {ex.Resource}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/auth-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlFtpException"/>
    /// <seealso cref="CurlLoginDeniedException"/>
    [Serializable]
    public class CurlRemoteAccessDeniedException : CurlException
    {
        /// <summary>
        /// The remote resource (path, directory or URL) the server refused access to, as requested.
        /// </summary>
        /// <value>A remote path such as <c>/pub/private/report.csv</c>.</value>
        /// <remarks><para>Compare it with what the account may see, for example by listing the parent directory.</para></remarks>
        /// <seealso cref="CurlException.Command"/>
        public string Resource { get; }

        /// <summary>
        /// Creates the access-denied error (exit code 9) for <paramref name="resource"/>, with advice on credentials,
        /// path and permissions.
        /// </summary>
        /// <remarks><para>Message: <c>Access denied to: RESOURCE</c>. Example: <c>throw new CurlRemoteAccessDeniedException("/pub/private", command);</c></para></remarks>
        /// <param name="resource">The resource that was denied access, such as a remote path.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlFtpException"/>
        public CurlRemoteAccessDeniedException(string resource, string command = null)
            : base($"Access denied to: {resource}", 9, command)
        {
            Resource = resource;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Auth;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Check the user name and password (-u user:password), that the remote path '{Resource}' is spelled exactly right (it is case-sensitive), and that the account may access it.";
    }

    /// <summary>
    /// In active FTP mode the server never connected back to accept the data connection (exit code 10,
    /// CURLE_FTP_ACCEPT_FAILED), usually because a firewall or NAT blocks incoming connections.
    /// </summary>
    /// <remarks>
    /// <para>Use passive mode (the default; do not pass <c>-P</c>/<c>--ftp-port</c>) or open the port range on the firewall.
    /// Example: <c>catch (CurlFtpAcceptFailedException) { /* retry without -P */ }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</para>
    /// </remarks>
    /// <seealso cref="CurlFtpAcceptTimeoutException"/>
    [Serializable]
    public class CurlFtpAcceptFailedException : CurlException
    {
        /// <summary>
        /// Creates the error for an active-mode FTP data connection that the server could not open back to this
        /// machine.
        /// </summary>
        /// <remarks><para>When <paramref name="message"/> has no fix, the suggestion recommends passive mode.</para></remarks>
        /// <param name="message">What failed, naming the FTP server; optionally followed by fix lines.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlFtpAcceptTimeoutException"/>
        public CurlFtpAcceptFailedException(string message, string command = null)
            : base(message, 10, command) { }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.ErrorReference;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "Use passive FTP (the default - remove -P/--ftp-port), or allow incoming connections from the FTP server through your firewall or NAT.";
    }

    /// <summary>
    /// The FTP server answered the password (PASS) command with something unexpected (exit code 11,
    /// CURLE_FTP_WEIRD_PASS_REPLY), usually a rejected or malformed login.
    /// </summary>
    /// <remarks>
    /// <para>Check <c>-u user:password</c> (quote it when the password has special characters) and whether the server
    /// requires FTPS (<c>--ssl-reqd</c>). Example: <c>catch (CurlFtpWeirdPassReplyException ex) { Console.WriteLine(ex.WhatHappened); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/auth-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlLoginDeniedException"/>
    [Serializable]
    public class CurlFtpWeirdPassReplyException : CurlException
    {
        /// <summary>
        /// Creates the error for an unexpected reply to the FTP password command, with advice on credentials and FTPS.
        /// </summary>
        /// <remarks><para>When <paramref name="message"/> has no fix, the suggestion covers <c>-u</c> and <c>--ssl-reqd</c>.</para></remarks>
        /// <param name="message">What the server answered, quoting its reply line; optionally followed by fix lines.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlLoginDeniedException"/>
        public CurlFtpWeirdPassReplyException(string message, string command = null)
            : base(message, 11, command) { }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Auth;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "Check the FTP user name and password given with -u user:password (quote it when it contains special characters), and add --ssl-reqd if the server only accepts logins over TLS.";
    }

    /// <summary>
    /// In active FTP mode the server did not connect back within the accept timeout (exit code 12,
    /// CURLE_FTP_ACCEPT_TIMEOUT); a <see cref="CurlTimeoutException"/> so timeout handlers catch it too.
    /// </summary>
    /// <remarks>
    /// <para>Use passive mode (no <c>-P</c>) or raise <c>--ftp-pasv</c>/<c>--connect-timeout</c>. Example:
    /// <c>catch (CurlFtpAcceptTimeoutException) { /* run again without -P */ }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/timeout-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlFtpAcceptFailedException"/>
    /// <seealso cref="CurlTimeoutException"/>
    [Serializable]
    public class CurlFtpAcceptTimeoutException : CurlTimeoutException
    {
        /// <summary>
        /// Creates the error (exit code 12) for an FTP server that did not open the active-mode data connection in time.
        /// </summary>
        /// <remarks><para>When <paramref name="message"/> has no fix, the suggestion recommends passive mode.</para></remarks>
        /// <param name="message">What timed out, naming the FTP server; optionally followed by fix lines.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlFtpAcceptFailedException"/>
        public CurlFtpAcceptTimeoutException(string message, string command = null)
            : base(message, command, null, 12) // CURLE_FTP_ACCEPT_TIMEOUT = 12
        {
        }

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "The FTP server never connected back for the data transfer: use passive FTP (remove -P/--ftp-port), or allow its incoming connection through your firewall.";
    }

    /// <summary>
    /// The server answered with an HTTP error status (400 or above) and the command used <c>-f</c>/<c>--fail</c> or
    /// <c>--fail-with-body</c> (exit code 22, CURLE_HTTP_RETURNED_ERROR).
    /// </summary>
    /// <remarks>
    /// <para>Without <c>-f</c> CurlDotNet returns the 4xx/5xx response as a normal <c>CurlResult</c>; with it, curl's rule
    /// "fail on HTTP errors" applies and this exception is thrown after the headers arrive. With <c>-f</c> the error body is
    /// discarded; with <c>--fail-with-body</c> it is written to the output and kept in
    /// <see cref="CurlHttpException.ResponseBody"/>. The message's advice depends on the status (401: credentials, 404:
    /// URL, 429: back off, 5xx: retry).</para>
    /// <para>Example: <c>try { await Curl.ExecuteAsync($"curl -f {url}"); } catch (CurlHttpReturnedErrorException ex) when (ex.StatusCode == 404) { return null; }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlHttpException"/>
    /// <seealso cref="CurlRateLimitException"/>
    [Serializable]
    public class CurlHttpReturnedErrorException : CurlHttpException
    {
        /// <summary>
        /// Creates the error (exit code 22) from a status code, reason phrase and body, with status-specific advice in the
        /// message.
        /// </summary>
        /// <remarks>
        /// <para>Message: <c>HTTP error 404: Not Found</c>, the advice for that status, and the HTTP errors page.
        /// Example: <c>throw new CurlHttpReturnedErrorException(503, "Service Unavailable", body, command);</c></para>
        /// </remarks>
        /// <param name="statusCode">The HTTP status code of the response, 400 or above.</param>
        /// <param name="statusText">The reason phrase from the status line, such as <c>Not Found</c>, or null.</param>
        /// <param name="body">The response body, or null when it was not read.</param>
        /// <param name="command">The curl command that failed, or null.</param>
        /// <seealso cref="CurlHttpReturnedErrorException(int, string, string, string, bool, string)"/>
        public CurlHttpReturnedErrorException(int statusCode, string statusText, string body, string? command = null)
            : base($"HTTP error {statusCode}: {statusText}", statusCode, statusText, body, command, 22)
        {
        }

        /// <summary>
        /// Creates the error for <c>-f</c>/<c>--fail</c> or <c>--fail-with-body</c> with curl's wording
        /// <c>The requested URL returned error: 404</c> and the URL (without credentials or query string).
        /// </summary>
        /// <remarks>
        /// <para>With <c>-f</c> the body was not written anywhere; with <c>--fail-with-body</c> it was written to the output
        /// as usual and is also available in <see cref="CurlHttpException.ResponseBody"/>.
        /// <see cref="CurlException.CurlErrorCode"/> is 22 either way. The second line explains what <c>-f</c> did; the
        /// status-specific advice follows in <see cref="CurlException.Suggestions"/>.</para>
        /// </remarks>
        /// <param name="statusCode">The HTTP status code (400 or above).</param>
        /// <param name="statusText">The reason phrase from the status line, or null.</param>
        /// <param name="body">The response body text, if any, otherwise null.</param>
        /// <param name="url">The URL that returned the error; printed without user:password and query string.</param>
        /// <param name="bodyKept">True for <c>--fail-with-body</c>, false for <c>-f</c>.</param>
        /// <param name="command">The curl command that failed, or null.</param>
        /// <seealso cref="CurlHttpReturnedErrorException(int, string, string, string)"/>
        public CurlHttpReturnedErrorException(int statusCode, string? statusText, string? body, string url, bool bodyKept, string? command)
            : base(CurlException.Compose(
                       $"The requested URL returned error: {statusCode}{(string.IsNullOrEmpty(statusText) ? string.Empty : " " + statusText)} ({ErrorText.SafeUrl(url)})",
                       (bodyKept
                           ? "--fail-with-body wrote the response body to the output; it is also in ResponseBody. "
                           : "-f/--fail turns HTTP errors into exit code 22 and discards the body; use --fail-with-body to keep it, or drop -f to get a CurlResult with the status code. ")
                       + StatusAdvice(statusCode)[0],
                       DocLinks.Http),
                   statusCode, statusText, body, command, 22)
        {
        }
    }

    /// <summary>
    /// Output could not be written (exit code 23, CURLE_WRITE_ERROR): the file for <c>-o</c>, <c>-O</c>, <c>-D</c>,
    /// <c>-c</c> or <c>--etag-save</c> could not be created or written; <see cref="FilePath"/> names it.
    /// </summary>
    /// <remarks>
    /// <para>Thrown when the folder does not exist (without <c>--create-dirs</c>), the user may not write there, the disk is
    /// full, the name is invalid on this OS, <c>--no-clobber</c> ran out of names, or <c>-O -J</c> would overwrite a file.
    /// <c>-D</c> is checked before connecting, like curl. Not covered: <c>--trace</c>/<c>--trace-ascii</c> create their
    /// folder themselves, <c>--stderr</c> falls back to the console, and an <c>--etag-save</c> folder that does not exist
    /// skips the transfer with a warning instead of failing.</para>
    /// <para>Example: <c>catch (CurlWriteErrorException ex) { Console.WriteLine($"Cannot write {ex.FilePath}: {ex.Suggestions[0]}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlReadErrorException"/>
    /// <seealso cref="CurlFileException"/>
    [Serializable]
    public class CurlWriteErrorException : CurlException
    {
        /// <summary>
        /// The file that could not be written, as given in the command (relative to the process's current directory).
        /// </summary>
        /// <value>A path such as <c>downloads/report.pdf</c>.</value>
        /// <remarks><para>Use <c>Path.GetFullPath(ex.FilePath)</c> to see the absolute location the process tried.</para></remarks>
        /// <seealso cref="CurlReadErrorException.FilePath"/>
        public string FilePath { get; }

        /// <summary>
        /// Creates the write error (exit code 23) for <paramref name="filePath"/>, prefixing curl's
        /// <c>Write error for PATH:</c> to the caller's explanation.
        /// </summary>
        /// <remarks>
        /// <para>When <paramref name="message"/> has no fix, the suggestion names the folder and mentions
        /// <c>--create-dirs</c>, permissions and disk space.
        /// Example: <c>throw new CurlWriteErrorException(path, "the directory 'out' does not exist.", command);</c></para>
        /// </remarks>
        /// <param name="filePath">The file path where the error occurred.</param>
        /// <param name="message">Why the write failed and, optionally, how to fix it (newline-separated).</param>
        /// <param name="command">The curl command that failed, or null.</param>
        /// <seealso cref="CurlReadErrorException"/>
        public CurlWriteErrorException(string filePath, string message, string? command = null)
            : base($"Write error for {filePath}: {message}", 23, command)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Creates the write error (exit code 23) for <paramref name="filePath"/> and keeps the .NET exception that
        /// caused it (an <see cref="System.IO.IOException"/>, <see cref="UnauthorizedAccessException"/> and so on) as
        /// <see cref="Exception.InnerException"/>.
        /// </summary>
        /// <remarks>
        /// <para>The message should still say in words what the inner exception reported; if it does not, the first
        /// line of the inner message is appended as <c>(caused by IOException: ...)</c>.
        /// Example: <c>catch (IOException io) { throw new CurlWriteErrorException(path, $"the disk is full ({io.Message}).\nFree space or write elsewhere.", command, io); }</c></para>
        /// </remarks>
        /// <param name="filePath">The file that could not be written.</param>
        /// <param name="message">Why the write failed and how to fix it (newline-separated).</param>
        /// <param name="command">The curl command that failed, or null.</param>
        /// <param name="innerException">The .NET exception that caused the failure, or null.</param>
        /// <seealso cref="CurlWriteErrorException(string, string, string)"/>
        public CurlWriteErrorException(string filePath, string message, string? command, Exception? innerException)
            : base($"Write error for {filePath}: {message}", 23, command, innerException)
        {
            FilePath = filePath;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.HowToDownload;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Check that the folder of '{FilePath}' exists (or add --create-dirs), that the current user may write there, and that the disk is not full.";
    }

    /// <summary>
    /// An upload was rejected by the server (exit code 25, CURLE_UPLOAD_FAILED), for example an FTP STOR refused or an
    /// upload stopped by the server; <see cref="FileName"/> names the file.
    /// </summary>
    /// <remarks>
    /// <para>Fixes: check the target path and that the account may write there, the server's size limit and free space.
    /// Example: <c>catch (CurlUploadFailedException ex) { Console.WriteLine($"Upload of {ex.FileName} refused"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlReadErrorException"/>
    [Serializable]
    public class CurlUploadFailedException : CurlException
    {
        /// <summary>
        /// The name of the file whose upload the server refused, as given with <c>-T</c> or <c>-F name=@file</c>.
        /// </summary>
        /// <value>A local file name or path.</value>
        /// <remarks><para>Check its size against the server's upload limit.</para></remarks>
        /// <seealso cref="CurlException.Command"/>
        public string FileName { get; }

        /// <summary>
        /// Creates the upload error (exit code 25) for <paramref name="fileName"/>, prefixing <c>Upload failed for FILE:</c>
        /// to the caller's explanation.
        /// </summary>
        /// <remarks><para>Example: <c>throw new CurlUploadFailedException("big.iso", "the server answered 552 (quota exceeded).", command);</c></para></remarks>
        /// <param name="fileName">The name of the file that failed to upload.</param>
        /// <param name="message">Why the upload failed and, optionally, how to fix it (newline-separated).</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlReadErrorException"/>
        public CurlUploadFailedException(string fileName, string message, string command = null)
            : base($"Upload failed for {fileName}: {message}", 25, command)
        {
            FileName = fileName;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.HowToUpload;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Check the upload URL's path, that the account may write there, and that '{FileName}' is within the server's size limit and free space.";
    }

    /// <summary>
    /// A local file the command needs could not be read (exit code 26, CURLE_READ_ERROR): the file after <c>@</c> in
    /// <c>-d @file</c>, <c>--data-binary @file</c>, <c>-F name=@file</c>, <c>-T file</c>, <c>-K file</c> or <c>.netrc</c>.
    /// </summary>
    /// <remarks>
    /// <para>Thrown before the request is sent. <see cref="FilePath"/> names the file. Fixes: check the path (relative paths
    /// start in the process's current directory, not next to your .exe), the permissions, and that no other program holds
    /// it locked. Example: <c>catch (CurlReadErrorException ex) { Console.WriteLine($"Cannot read {Path.GetFullPath(ex.FilePath)}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlWriteErrorException"/>
    /// <seealso cref="CurlFileCouldntReadException"/>
    [Serializable]
    public class CurlReadErrorException : CurlException
    {
        /// <summary>
        /// The local file that could not be read, as given in the command (relative to the process's current directory).
        /// </summary>
        /// <value>A path such as <c>payload.json</c>.</value>
        /// <remarks><para>Use <c>Path.GetFullPath(ex.FilePath)</c> to see where the process looked.</para></remarks>
        /// <seealso cref="CurlWriteErrorException.FilePath"/>
        public string FilePath { get; }

        /// <summary>
        /// Creates the read error (exit code 26) for <paramref name="filePath"/>, prefixing <c>Read error for PATH:</c> to
        /// the caller's explanation.
        /// </summary>
        /// <remarks>
        /// <para>When <paramref name="message"/> has no fix, the suggestion names the full path and the current directory.
        /// Example: <c>throw new CurlReadErrorException("body.json", "the file does not exist.", command);</c></para>
        /// </remarks>
        /// <param name="filePath">The file path where the error occurred.</param>
        /// <param name="message">Why the read failed and, optionally, how to fix it (newline-separated).</param>
        /// <param name="command">The curl command that failed, or null.</param>
        /// <seealso cref="CurlWriteErrorException"/>
        public CurlReadErrorException(string filePath, string message, string? command = null)
            : base($"Read error for {filePath}: {message}", 26, command)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Creates the read error (exit code 26) for <paramref name="filePath"/> and keeps the .NET exception that
        /// caused it (<see cref="System.IO.FileNotFoundException"/>, <see cref="UnauthorizedAccessException"/>...) as
        /// <see cref="Exception.InnerException"/>.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>catch (UnauthorizedAccessException denied) { throw new CurlReadErrorException(path, $"access is denied ({denied.Message}).\nGrant read permission or copy the file somewhere readable.", command, denied); }</c></para>
        /// </remarks>
        /// <param name="filePath">The file that could not be read.</param>
        /// <param name="message">Why the read failed and how to fix it (newline-separated).</param>
        /// <param name="command">The curl command that failed, or null.</param>
        /// <param name="innerException">The .NET exception that caused the failure, or null.</param>
        /// <seealso cref="CurlReadErrorException(string, string, string)"/>
        public CurlReadErrorException(string filePath, string message, string? command, Exception? innerException)
            : base($"Read error for {filePath}: {message}", 26, command, innerException)
        {
            FilePath = filePath;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.HowToUpload;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Check that '{FilePath}' exists (relative paths start in {Environment.CurrentDirectory}), that the current user may read it, and that no other program holds it locked.";
    }

    /// <summary>
    /// Memory ran out while building the request or holding the response (exit code 27, CURLE_OUT_OF_MEMORY),
    /// typically for a very large download read into <c>Body</c>.
    /// </summary>
    /// <remarks>
    /// <para>CurlDotNet keeps the response in memory unless it is written to a file. For large downloads use <c>-o file</c>
    /// (or <c>Curl.DownloadAsync</c>) and avoid reading <c>Body</c>; limit sizes with <c>--max-filesize</c>.
    /// Example: <c>catch (CurlOutOfMemoryException) { await Curl.ExecuteAsync($"curl -o big.bin {url}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlFileSizeExceededException"/>
    [Serializable]
    public class CurlOutOfMemoryException : CurlException
    {
        /// <summary>
        /// Creates the out-of-memory error (exit code 27) with advice to stream large responses to a file instead of
        /// memory.
        /// </summary>
        /// <remarks><para>Message: <c>Out of memory</c>, the advice to use <c>-o</c> and <c>--max-filesize</c>, and the download guide.</para></remarks>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlFileSizeExceededException"/>
        public CurlOutOfMemoryException(string command = null)
            : base("Out of memory while building the request or holding the response", 27, command) { }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.HowToDownload;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "Save large responses to a file with -o FILE (or Curl.DownloadAsync) instead of reading Body, and cap the size with --max-filesize BYTES.";
    }

    /// <summary>
    /// The transfer ran out of time (exit code 28, CURLE_OPERATION_TIMEDOUT): <c>--connect-timeout</c> expired while
    /// connecting, or <c>-m/--max-time</c> expired before the whole response arrived.
    /// </summary>
    /// <remarks>
    /// <para>Thrown by the HTTP handler and the transport; the first line uses curl's wording
    /// (<c>Operation timed out after 5000 milliseconds with 1024 bytes received</c>,
    /// <c>Connection timed out after 2000 milliseconds</c>). <see cref="CurlTimeoutException.Timeout"/> holds the limit.
    /// Cancelling your own token throws <see cref="OperationCanceledException"/> instead (with a
    /// <see cref="CurlAbortedByCallbackException"/> as its InnerException).</para>
    /// <para>Example: <c>catch (CurlOperationTimeoutException ex) { Console.WriteLine($"Timed out ({ex.Timeout?.TotalSeconds} s) - raising -m"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/timeout-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlTimeoutException"/>
    /// <seealso cref="CurlAbortedByCallbackException"/>
    [Serializable]
    public class CurlOperationTimeoutException : CurlTimeoutException
    {
        /// <summary>
        /// Creates the timeout error with curl's wording <c>Operation timed out after N milliseconds</c> for the limit
        /// <paramref name="timeoutSeconds"/>.
        /// </summary>
        /// <remarks><para>The suggestion names the limit and tells the reader to raise <c>-m</c> or <c>--connect-timeout</c>.</para></remarks>
        /// <param name="timeoutSeconds">The timeout value in seconds that was exceeded (decimals allowed).</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlOperationTimeoutException(double, string, string)"/>
        public CurlOperationTimeoutException(double timeoutSeconds, string command = null)
            : base($"Operation timed out after {((long)(timeoutSeconds * 1000)).ToString(CultureInfo.InvariantCulture)} milliseconds", command, TimeSpan.FromSeconds(timeoutSeconds))
        {
        }

        /// <summary>
        /// Creates the timeout error with a message describing which limit expired and what had been received, such as
        /// curl's <c>Connection timed out after 2000 milliseconds</c>.
        /// </summary>
        /// <remarks><para>When <paramref name="message"/> has no fix, the suggestion names the limit.</para></remarks>
        /// <param name="timeoutSeconds">The limit that expired (<c>--connect-timeout</c> or <c>-m/--max-time</c>), 0 when none was set.</param>
        /// <param name="message">What happened and, optionally, how to fix it (newline-separated).</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlOperationTimeoutException(double, string)"/>
        public CurlOperationTimeoutException(double timeoutSeconds, string message, string? command)
            : base(message, command, TimeSpan.FromSeconds(timeoutSeconds))
        {
        }
    }

    /// <summary>
    /// A multipart/form POST could not be built or sent (exit code 34, CURLE_HTTP_POST_ERROR), for example a
    /// <c>-F</c> field whose file or syntax is wrong.
    /// </summary>
    /// <remarks>
    /// <para>Fixes: check every <c>-F name=value</c> / <c>-F name=@file;type=mime</c> field and the files it names.
    /// Example: <c>catch (CurlHttpPostErrorException ex) { Console.WriteLine(ex.WhatHappened); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlReadErrorException"/>
    [Serializable]
    public class CurlHttpPostErrorException : CurlException
    {
        /// <summary>
        /// Creates the form-post error (exit code 34), prefixing <c>HTTP POST error:</c> to the caller's explanation of
        /// which field failed.
        /// </summary>
        /// <remarks><para>Example: <c>throw new CurlHttpPostErrorException("field 'avatar' names a folder, not a file.", command);</c></para></remarks>
        /// <param name="message">Which form field failed and why; optionally followed by fix lines.</param>
        /// <param name="command">The curl command that failed, or null.</param>
        /// <seealso cref="CurlUploadFailedException"/>
        public CurlHttpPostErrorException(string message, string? command = null)
            : base($"HTTP POST error: {message}", 34, command) { }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.HowToUpload;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "Check each -F field: name=value for text, name=@path for a file that exists and is readable, and ;type=mime/type only after the file name.";
    }

    /// <summary>
    /// The TLS handshake failed for a reason other than an untrusted certificate (exit code 35,
    /// CURLE_SSL_CONNECT_ERROR): protocol or cipher mismatch, a non-TLS port, or a rejected client certificate.
    /// </summary>
    /// <remarks>
    /// <para>Thrown by the transport when <c>SslStream</c> authentication fails. Fixes: check the port speaks TLS (https on 443,
    /// not http on 80), that the server supports TLS 1.2 or later, any <c>--cert</c>/<c>--key</c> client certificate, and
    /// corporate TLS inspection. Example: <c>catch (CurlSslConnectErrorException ex) { Console.WriteLine(ex.WhatHappened); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlPeerFailedVerificationException"/>
    /// <seealso cref="CurlSslException"/>
    [Serializable]
    public class CurlSslConnectErrorException : CurlSslException
    {
        /// <summary>
        /// Creates the TLS handshake error (exit code 35), prefixing <c>SSL connect error:</c> to the TLS layer's
        /// explanation.
        /// </summary>
        /// <remarks><para>When <paramref name="message"/> has no fix, the suggestion covers ports, TLS versions and client certificates.</para></remarks>
        /// <param name="message">What the TLS layer reported, naming the host; optionally followed by fix lines.</param>
        /// <param name="command">The curl command that failed, or null.</param>
        /// <seealso cref="CurlPeerFailedVerificationException"/>
        public CurlSslConnectErrorException(string message, string? command = null)
            : base($"SSL connect error: {message}", null, command, 35)
        {
        }

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "Check that the URL's port speaks TLS (https://host:443, not an http-only port), that the server supports TLS 1.2 or later, and any client certificate given with --cert/--key; -v shows how far the handshake got.";
    }

    /// <summary>
    /// A resumed download could not continue from the requested offset (exit code 36, CURLE_BAD_DOWNLOAD_RESUME);
    /// <see cref="ResumeOffset"/> says where it tried to resume.
    /// </summary>
    /// <remarks>
    /// <para>Happens with <c>-C OFFSET</c>/<c>-C -</c> when the offset is beyond the remote file's size (the remote file
    /// changed or is shorter). Delete the partial file and download again without <c>-C</c>.
    /// Example: <c>catch (CurlBadDownloadResumeException ex) { File.Delete(partial); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlRangeErrorException"/>
    /// <seealso cref="CurlPartialFileException"/>
    [Serializable]
    public class CurlBadDownloadResumeException : CurlException
    {
        /// <summary>
        /// The byte offset the download tried to resume from (the size of the partial local file for <c>-C -</c>).
        /// </summary>
        /// <value>A byte offset, 0 or more.</value>
        /// <remarks><para>Compare it with the remote file's Content-Length from a HEAD request (<c>curl -I</c>).</para></remarks>
        /// <seealso cref="CurlRangeErrorException.Offset"/>
        public long ResumeOffset { get; }

        /// <summary>
        /// Creates the resume error (exit code 36) for <paramref name="offset"/>, with advice to restart the download.
        /// </summary>
        /// <remarks><para>Message: <c>Bad download resume at offset N</c>. Example: <c>throw new CurlBadDownloadResumeException(1048576, command);</c></para></remarks>
        /// <param name="offset">The byte offset where the resume was attempted.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlRangeErrorException"/>
        public CurlBadDownloadResumeException(long offset, string command = null)
            : base($"Bad download resume at offset {offset}: the remote file is not that long or has changed", 36, command)
        {
            ResumeOffset = offset;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.HowToDownload;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Delete the partial local file and download again without -C/--continue-at; resuming from byte {ResumeOffset} only works when the remote file is unchanged and at least that long.";
    }

    /// <summary>
    /// A local file could not be opened (exit code 37, CURLE_FILE_COULDNT_READ_FILE): the path of a <c>file://</c>
    /// URL, or a missing file after <c>-d @</c>/<c>--data-binary @</c>; <see cref="FilePath"/> names it.
    /// </summary>
    /// <remarks>
    /// <para>Thrown by the file handler for <c>file://</c> URLs it may not read, and by the HTTP handler when the file named
    /// in <c>-d @file</c> does not exist (a file that exists but cannot be read is <see cref="CurlReadErrorException"/>, exit 26).
    /// Fixes: check the path (use three slashes for absolute paths:
    /// <c>file:///C:/data/x.txt</c> or <c>file:///home/me/x.txt</c>), that it is a file and not a folder, and the
    /// permissions. Example: <c>catch (CurlFileCouldntReadException ex) { Console.WriteLine($"No such file: {ex.FilePath}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</para>
    /// </remarks>
    /// <seealso cref="CurlReadErrorException"/>
    [Serializable]
    public class CurlFileCouldntReadException : CurlException
    {
        /// <summary>
        /// The local path from the <c>file://</c> URL that could not be opened or read by the current user.
        /// </summary>
        /// <value>An absolute path such as <c>/home/me/data.json</c> or <c>C:\data\x.txt</c>.</value>
        /// <remarks><para>Check <c>File.Exists(ex.FilePath)</c> to tell a missing file from a permission problem.</para></remarks>
        /// <seealso cref="CurlReadErrorException.FilePath"/>
        public string FilePath { get; }

        /// <summary>
        /// Creates the error (exit code 37) for a <c>file://</c> path that cannot be opened, with curl's wording
        /// <c>Couldn't open file PATH</c>.
        /// </summary>
        /// <remarks><para>Example: <c>throw new CurlFileCouldntReadException("/data/missing.json", command);</c></para></remarks>
        /// <param name="filePath">The file path that could not be read.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlReadErrorException"/>
        public CurlFileCouldntReadException(string filePath, string command = null)
            : base($"Couldn't open file {filePath} (could not read file: {filePath})", 37, command)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Creates the error (exit code 37) for <paramref name="filePath"/> with the caller's own explanation and fix,
        /// for situations other than a <c>file://</c> URL (such as <c>-d @missing.json</c>).
        /// </summary>
        /// <remarks><para>Example: <c>throw new CurlFileCouldntReadException(path, "Couldn't open file 'body.json' given with -d @body.json: it does not exist.\nCheck the path.", command);</c></para></remarks>
        /// <param name="filePath">The file path that could not be opened.</param>
        /// <param name="message">What happened (first line) and how to fix it (following lines).</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlFileCouldntReadException(string, string)"/>
        public CurlFileCouldntReadException(string filePath, string message, string? command)
            : base(message, 37, command)
        {
            FilePath = filePath;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.ErrorReference;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Check that '{FilePath}' exists, is a file (not a folder) and is readable by the current user; absolute file URLs need three slashes, such as file:///home/me/data.json or file:///C:/data/x.txt.";
    }

    /// <summary>
    /// A required function (for example an LDAP or crypto routine) is not available on this platform (exit code 41,
    /// CURLE_FUNCTION_NOT_FOUND); <see cref="FunctionName"/> names it.
    /// </summary>
    /// <remarks>
    /// <para>Usually means a platform API is missing on the current OS or target framework. Remove the option that needs
    /// it or run on a platform that has it. Example: <c>catch (CurlFunctionNotFoundException ex) { Console.WriteLine(ex.FunctionName); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/reference/curl-cli-compatibility.md</para>
    /// </remarks>
    /// <seealso cref="CurlNotBuiltInException"/>
    [Serializable]
    public class CurlFunctionNotFoundException : CurlException
    {
        /// <summary>
        /// The function or platform API that could not be found, so you can tell which option depends on it.
        /// </summary>
        /// <value>The function name as passed to the constructor.</value>
        /// <remarks><para>Search the compatibility list for the option that needs it.</para></remarks>
        /// <seealso cref="CurlNotBuiltInException.Feature"/>
        public string FunctionName { get; }

        /// <summary>
        /// Creates the error (exit code 41) for a missing function, with advice to remove the option or change
        /// platform.
        /// </summary>
        /// <remarks><para>Message: <c>Function not found: NAME</c>.</para></remarks>
        /// <param name="functionName">The function name that was not found.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlNotBuiltInException"/>
        public CurlFunctionNotFoundException(string functionName, string command = null)
            : base($"Function not found: {functionName}", 41, command)
        {
            FunctionName = functionName;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.CliCompatibility;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"{FunctionName} is not available on this OS or target framework: remove the option that needs it, or run on .NET 8 or later where more platform APIs exist.";
    }

    /// <summary>
    /// The transfer was stopped on purpose (exit code 42, CURLE_ABORTED_BY_CALLBACK): your progress callback threw,
    /// or (seen only as an InnerException) your <c>CancellationToken</c> was cancelled.
    /// </summary>
    /// <remarks>
    /// <para>You catch this type directly when a handler passed to <c>CurlSettings.WithProgress</c> /
    /// <c>CurlOptions.ProgressHandler</c> throws; the handler's exception is the <see cref="Exception.InnerException"/>.</para>
    /// <para>Cancelling your own token is different: the public APIs (<c>Curl.ExecuteAsync</c>, <c>CurlRequestBuilder</c>,
    /// <c>LibCurl</c>) throw the standard <see cref="OperationCanceledException"/>, with this exception as its InnerException,
    /// so write <c>catch (OperationCanceledException) when (cts.IsCancellationRequested) { /* user pressed Cancel */ }</c>.
    /// Timeouts set with <c>-m</c> are <see cref="CurlOperationTimeoutException"/> instead.</para>
    /// <para>Example: <c>catch (CurlAbortedByCallbackException ex) { Console.WriteLine($"Progress handler failed: {ex.InnerException?.Message}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</para>
    /// </remarks>
    /// <seealso cref="CurlOperationTimeoutException"/>
    [Serializable]
    public class CurlAbortedByCallbackException : CurlException
    {
        /// <summary>
        /// Creates the error (exit code 42) for a transfer cancelled through the caller's <c>CancellationToken</c>.
        /// </summary>
        /// <remarks>
        /// <para>Message: <c>Operation aborted by callback</c> with advice to check who cancelled the token. Thrown when the
        /// token passed to <c>ExecuteAsync</c> is cancelled.</para>
        /// </remarks>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlAbortedByCallbackException(string, string, Exception)"/>
        public CurlAbortedByCallbackException(string command = null)
            : base("Operation aborted by callback: the CancellationToken passed to the call was cancelled before the transfer finished", 42, command) { }

        /// <summary>
        /// Creates the exception for a transfer that your own callback (for example a progress handler set
        /// with <see cref="CurlDotNet.Core.CurlSettings.WithProgress"/>) stopped by throwing.
        /// </summary>
        /// <remarks>
        /// <para>curl ends a transfer with exit code 42 when its progress callback returns non-zero; CurlDotNet
        /// does the same when a callback throws, and keeps the callback's exception as
        /// <see cref="Exception.InnerException"/>.</para>
        /// <para>Example: <c>settings.WithProgress((pct, got, total) =&gt; { if (got &gt; limit) throw new OperationCanceledException(); })</c>
        /// ends the download with this exception once <c>limit</c> bytes have arrived.</para>
        /// </remarks>
        /// <param name="message">What the callback did and how to fix it, separated by a newline.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <param name="innerException">The exception the callback threw.</param>
        /// <seealso cref="CurlDotNet.Core.CurlOptions.ProgressHandler"/>
        public CurlAbortedByCallbackException(string message, string? command, Exception? innerException)
            : base(message, 42, command, innerException) { }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.TutorialCancellation;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion => InnerException != null
            ? $"Your callback threw {InnerException.GetType().Name}; fix it, or throw only when you mean to stop the transfer."
            : "Check which code cancelled the CancellationToken (a timeout CancellationTokenSource, a request-aborted token, a Cancel button); use -m/--max-time for time limits instead.";
    }

    /// <summary>
    /// An option value or combination is not valid for this request (exit code 43, CURLE_BAD_FUNCTION_ARGUMENT);
    /// <see cref="ArgumentName"/> names the option, for example <c>-C/--continue-at</c> with an upload.
    /// </summary>
    /// <remarks>
    /// <para>Thrown by the HTTP handler before sending, for example when <c>-C</c> is combined with a request body (curl
    /// cannot resume uploads over HTTP). The message says which options clash and what to use instead.
    /// Example: <c>catch (CurlBadFunctionArgumentException ex) { Console.WriteLine($"Remove {ex.ArgumentName}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</para>
    /// </remarks>
    /// <seealso cref="CurlInvalidCommandException"/>
    /// <seealso cref="CurlOptionSyntaxException"/>
    [Serializable]
    public class CurlBadFunctionArgumentException : CurlException
    {
        /// <summary>
        /// The option or argument that cannot be used this way, such as <c>-C/--continue-at</c>.
        /// </summary>
        /// <value>The option name as shown to the user.</value>
        /// <remarks><para>Remove or change that option in the command.</para></remarks>
        /// <seealso cref="CurlInvalidCommandException.InvalidPart"/>
        public string ArgumentName { get; }

        /// <summary>
        /// Creates the error (exit code 43) naming the invalid argument, with advice to correct or remove it.
        /// </summary>
        /// <remarks><para>Message: <c>Bad function argument: NAME</c>.</para></remarks>
        /// <param name="argumentName">The name of the argument or option that was invalid.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlBadFunctionArgumentException(string, string, string)"/>
        public CurlBadFunctionArgumentException(string argumentName, string command = null)
            : base($"Bad function argument: {argumentName}", 43, command)
        {
            ArgumentName = argumentName;
        }

        /// <summary>
        /// Creates the error for a combination of options curl refuses, explaining which options clash and what to use
        /// instead.
        /// </summary>
        /// <remarks><para>Example: <c>-C 100 -d data</c> gives "cannot be combined with a request body" and the advice to send the whole body again.</para></remarks>
        /// <param name="argumentName">The option that cannot be used this way, such as <c>-C/--continue-at</c>.</param>
        /// <param name="detail">What is wrong and how to fix it (a fix sentence may follow on the same line).</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlBadFunctionArgumentException(string, string)"/>
        public CurlBadFunctionArgumentException(string argumentName, string detail, string? command)
            : base(detail, 43, command)
        {
            ArgumentName = argumentName;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.ErrorReference;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Remove {ArgumentName} or change its value so it fits this request (check curl's manual for the options it cannot be combined with).";
    }

    /// <summary>
    /// The local network interface or source address given with <c>--interface</c> could not be used (exit code 45,
    /// CURLE_INTERFACE_FAILED); <see cref="InterfaceName"/> names it.
    /// </summary>
    /// <remarks>
    /// <para>Fixes: use an interface name or IP address that exists on this machine (<c>ipconfig</c>/<c>ip addr</c>) and is
    /// up. Example: <c>catch (CurlInterfaceFailedException ex) { Console.WriteLine($"No interface {ex.InterfaceName}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlCouldntConnectException"/>
    [Serializable]
    public class CurlInterfaceFailedException : CurlException
    {
        /// <summary>
        /// The interface name or local address from <c>--interface</c> that could not be bound, such as <c>eth1</c> or
        /// <c>10.0.0.5</c>.
        /// </summary>
        /// <value>The value as given in the command.</value>
        /// <remarks><para>Compare it with <c>NetworkInterface.GetAllNetworkInterfaces()</c>.</para></remarks>
        /// <seealso cref="CurlException.Command"/>
        public string InterfaceName { get; }

        /// <summary>
        /// Creates the error (exit code 45) for an interface that cannot be used, with advice to pick an existing one.
        /// </summary>
        /// <remarks><para>Message: <c>Interface failed: NAME</c>.</para></remarks>
        /// <param name="interfaceName">The name or address of the network interface that failed.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlCouldntConnectException"/>
        public CurlInterfaceFailedException(string interfaceName, string command = null)
            : base($"Interface failed: {interfaceName}", 45, command)
        {
            InterfaceName = interfaceName;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Dns;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Use an interface name or local IP address that exists and is up on this machine instead of '{InterfaceName}' (list them with ipconfig or ip addr), or drop --interface.";
    }

    /// <summary>
    /// <c>-L</c> followed more redirects than <c>--max-redirs</c> allows (exit code 47, CURLE_TOO_MANY_REDIRECTS; the
    /// default limit is 50), usually a redirect loop.
    /// </summary>
    /// <remarks>
    /// <para>Thrown by the redirect handler with curl's wording <c>Maximum (50) redirects followed</c>.
    /// <see cref="RedirectCount"/> says how many hops were followed. Loops are often caused by a login page or a cookie the
    /// client does not keep; use <c>-b cookies.txt -c cookies.txt</c>.</para>
    /// <para>Example: <c>catch (CurlTooManyRedirectsException ex) { Console.WriteLine($"Loop? {ex.RedirectCount} hops"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/redirect-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlRedirectException"/>
    /// <seealso cref="CurlUnsupportedProtocolException"/>
    [Serializable]
    public class CurlTooManyRedirectsException : CurlException
    {
        /// <summary>
        /// How many redirects were followed before the next one was refused; equal to the limit when the chain loops.
        /// </summary>
        /// <value>The number of hops followed.</value>
        /// <remarks><para>Example: <c>if (ex.RedirectCount &gt;= 50) Console.WriteLine("Redirect loop");</c></para></remarks>
        /// <seealso cref="CurlRedirectException.RedirectCount"/>
        public int RedirectCount { get; }

        /// <summary>
        /// Creates the error (exit code 47) for a redirect chain of <paramref name="count"/> hops, with advice to look for a
        /// loop or raise the limit.
        /// </summary>
        /// <remarks><para>Message: <c>Too many redirects: N</c>.</para></remarks>
        /// <param name="count">The number of redirects that were followed before failing.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlTooManyRedirectsException(int, int, string)"/>
        public CurlTooManyRedirectsException(int count, string command = null)
            : base($"Too many redirects: {count}", 47, command)
        {
            RedirectCount = count;
        }

        /// <summary>
        /// Creates the error using curl's wording <c>Maximum (N) redirects followed</c>, naming both the limit and the
        /// number of hops.
        /// </summary>
        /// <remarks><para>Thrown by the redirect handler when <c>--max-redirs</c> is reached.</para></remarks>
        /// <param name="count">How many redirects were followed before the next one was refused.</param>
        /// <param name="limit">The <c>--max-redirs</c> limit (default 50).</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlTooManyRedirectsException(int, string)"/>
        public CurlTooManyRedirectsException(int count, int limit, string? command)
            : base(CurlException.Compose(
                       $"Maximum ({limit}) redirects followed: the server sent another redirect after {count} hop(s).",
                       "Raise the limit with --max-redirs N (-1 means unlimited), or check for a redirect loop with -i -L.",
                       DocLinks.Redirect),
                   47, command)
        {
            RedirectCount = count;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Redirect;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"After {RedirectCount} redirects: check the chain with curl -i -L for a loop (often a login page or a cookie that is not kept - use -b cookies.txt -c cookies.txt), or raise --max-redirs.";
    }

    /// <summary>
    /// The command uses an option curl does not have (CURLE_UNKNOWN_OPTION); <see cref="OptionName"/> names it, usually
    /// a typo or an option from a newer curl.
    /// </summary>
    /// <remarks>
    /// <para>A <see cref="CurlInvalidCommandException"/>; nothing was sent. Fixes: check the spelling (<c>--header</c>, not
    /// <c>--headers</c>), and the option list in <c>curl --help all</c>. <c>Curl.Validate(command)</c> reports unknown options
    /// without throwing. Example: <c>catch (CurlUnknownOptionException ex) { Console.WriteLine($"Unknown: {ex.OptionName}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/reference/curl-cli-compatibility.md</para>
    /// </remarks>
    /// <seealso cref="CurlInvalidCommandException"/>
    /// <seealso cref="CurlOptionSyntaxException"/>
    [Serializable]
    public class CurlUnknownOptionException : CurlInvalidCommandException
    {
        /// <summary>
        /// The option that curl does not recognise, exactly as written, such as <c>--headers</c>.
        /// </summary>
        /// <value>The option text including dashes.</value>
        /// <remarks><para>Also available as <see cref="CurlInvalidCommandException.InvalidPart"/>.</para></remarks>
        /// <seealso cref="CurlInvalidCommandException.InvalidPart"/>
        public string OptionName { get; }

        /// <summary>
        /// Creates the error for an unknown option, with curl's wording <c>option NAME: is unknown</c> and advice to check the
        /// spelling.
        /// </summary>
        /// <remarks><para>Example: <c>throw new CurlUnknownOptionException("--headers", command);</c></para></remarks>
        /// <param name="optionName">The name of the unknown option that was specified, with its dashes.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlOptionSyntaxException"/>
        public CurlUnknownOptionException(string optionName, string command = null)
            : base($"option {optionName}: is unknown (unknown option: {optionName})", optionName, command)
        {
            OptionName = optionName;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.CliCompatibility;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Check the spelling of {OptionName} against curl's option list (curl --help all); options are case-sensitive, long ones start with --, and values that start with - must be quoted.";
    }

    /// <summary>
    /// An option's value has the wrong syntax (CURLE_SETOPT_OPTION_SYNTAX, exit code 49 for the overload that sets it),
    /// for example <c>--resolve</c> or <c>--connect-to</c> entries with a port that is not a number.
    /// </summary>
    /// <remarks>
    /// <para>A <see cref="CurlInvalidCommandException"/>; nothing was sent. The message explains the expected format, such
    /// as <c>--resolve HOST:PORT:ADDRESS</c>. Example: <c>catch (CurlOptionSyntaxException ex) { Console.WriteLine(ex.Suggestions[0]); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</para>
    /// </remarks>
    /// <seealso cref="CurlUnknownOptionException"/>
    /// <seealso cref="CurlInvalidCommandException"/>
    [Serializable]
    public class CurlOptionSyntaxException : CurlInvalidCommandException
    {
        /// <summary>
        /// Creates a syntax error for <paramref name="option"/> without an exit code, with advice to check the value's
        /// format in curl's manual.
        /// </summary>
        /// <remarks><para>Message: <c>Option syntax error: OPTION</c>.</para></remarks>
        /// <param name="option">The option whose value has the wrong syntax, such as <c>--resolve</c>.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlOptionSyntaxException(string, string, string)"/>
        public CurlOptionSyntaxException(string option, string command = null)
            : base($"Option syntax error: {option}", option, command)
        {
        }

        /// <summary>
        /// Creates the error for an option value curl refuses to parse (exit code 49), such as <c>--resolve</c> or
        /// <c>--connect-to</c> entries with a port that is not a number.
        /// </summary>
        /// <param name="option">The option whose value is malformed (for example <c>--resolve</c>).</param>
        /// <param name="message">What is wrong with the value and how to write it correctly.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <remarks>
        /// <para>Usage: <c>curl --resolve example.com:80:127.0.0.1 http://example.com/</c> is valid; swapping the
        /// host and the port (<c>--resolve 80:example.com:127.0.0.1</c>) throws this exception with
        /// <see cref="CurlException.CurlErrorCode"/> 49, exactly where the curl tool exits with 49.</para>
        /// </remarks>
        /// <seealso cref="CurlOptionSyntaxException(string, string)"/>
        public CurlOptionSyntaxException(string option, string message, string command)
            : base(message, 49, option, command)
        {
        }

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Correct the value of {InvalidPart}: check its format in curl's manual (for example --resolve HOST:PORT:ADDRESS, --connect-to HOST1:PORT1:HOST2:PORT2) and quote it when it contains spaces.";
    }

    /// <summary>
    /// The server accepted the connection and the request, then closed it without sending a single byte (exit code 52,
    /// CURLE_GOT_NOTHING, "Empty reply from server").
    /// </summary>
    /// <remarks>
    /// <para>Thrown by the transport. Usual causes: the server crashed or rejected the request, plain HTTP sent to a TLS port
    /// (or the reverse), a request too large for the server, or an idle keep-alive connection closed by a proxy. Retrying
    /// may help (<see cref="CurlException.IsRetryable"/> is true).</para>
    /// <para>Example: <c>catch (CurlGotNothingException ex) { Console.WriteLine(ex.WhatHappened); /* check server logs */ }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlReceiveErrorException"/>
    /// <seealso cref="CurlWeirdServerReplyException"/>
    [Serializable]
    public class CurlGotNothingException : CurlException
    {
        /// <summary>
        /// Creates the empty-reply error (exit code 52) with curl's wording <c>Empty reply from server</c> and advice on
        /// ports, TLS and server logs.
        /// </summary>
        /// <remarks><para>Used when the host is not known; the transport normally uses the overload that names it.</para></remarks>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlGotNothingException(string, string, string)"/>
        public CurlGotNothingException(string command = null)
            : base("Empty reply from server: the server closed the connection without sending a single byte (an empty reply)", 52, command) { }

        /// <summary>
        /// Creates the empty-reply error naming the server that closed the connection without replying, plus any extra
        /// context such as the request that was sent.
        /// </summary>
        /// <remarks><para>Thrown by the transport; <paramref name="detail"/> may be empty.</para></remarks>
        /// <param name="host">The server (or proxy) that accepted the request and closed the connection.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <param name="detail">Extra context, such as the request that was sent; may be empty.</param>
        /// <seealso cref="CurlGotNothingException(string)"/>
        public CurlGotNothingException(string host, string? command, string detail)
            : base(CurlException.Compose(
                       $"Empty reply from server: {host} accepted the connection and the request, then closed it without sending a single byte. {detail}".TrimEnd(),
                       "This usually means the server crashed or rejected the request (wrong port, TLS expected, request too large); check the server's logs, try -v, or retry with --retry.",
                       DocLinks.Http),
                   52, command) { }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Http;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "Check that the URL's scheme and port match the server (http vs https), look at the server's logs for a crash or a rejected request, and retry with --retry 3 for transient failures.";
    }

    /// <summary>
    /// The crypto engine named with <c>--engine</c> was not found (exit code 53, CURLE_SSL_ENGINE_NOTFOUND); .NET's TLS
    /// stack has no OpenSSL engines.
    /// </summary>
    /// <remarks>
    /// <para>Remove <c>--engine</c>: CurlDotNet uses the operating system's TLS (SChannel, OpenSSL via .NET, or Secure Transport).
    /// Example: <c>catch (CurlSslEngineNotFoundException) { /* drop --engine */ }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlSslEngineSetFailedException"/>
    /// <seealso cref="CurlSslException"/>
    [Serializable]
    public class CurlSslEngineNotFoundException : CurlSslException
    {
        /// <summary>
        /// Creates the error (exit code 53) for an <c>--engine</c> that does not exist, with advice to remove the option.
        /// </summary>
        /// <remarks><para>Message: <c>SSL engine not found: NAME</c>.</para></remarks>
        /// <param name="engine">The SSL engine that was not found, as given with <c>--engine</c>.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlSslEngineSetFailedException"/>
        public CurlSslEngineNotFoundException(string engine, string command = null)
            : base($"SSL engine not found: {engine}", null, command, 53) // CURLE_SSL_ENGINE_NOTFOUND = 53
        {
        }

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "Remove --engine: CurlDotNet uses the operating system's TLS stack through .NET, which has no OpenSSL engines.";
    }

    /// <summary>
    /// The crypto engine could not be set as the default (exit code 54, CURLE_SSL_ENGINE_SETFAILED).
    /// </summary>
    /// <remarks>
    /// <para>Remove <c>--engine</c>; .NET's TLS stack does not use engines. Example:
    /// <c>catch (CurlSslEngineSetFailedException ex) { Console.WriteLine(ex.WhatHappened); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlSslEngineNotFoundException"/>
    [Serializable]
    public class CurlSslEngineSetFailedException : CurlSslException
    {
        /// <summary>
        /// Creates the error (exit code 54) for an engine that could not be made the default, prefixing
        /// <c>Failed to set SSL engine:</c> to the reason.
        /// </summary>
        /// <remarks><para>When <paramref name="message"/> has no fix, the suggestion says to remove <c>--engine</c>.</para></remarks>
        /// <param name="message">Why the engine could not be set; optionally followed by fix lines.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlSslEngineNotFoundException"/>
        public CurlSslEngineSetFailedException(string message, string command = null)
            : base($"Failed to set SSL engine: {message}", null, command, 54) // CURLE_SSL_ENGINE_SETFAILED = 54
        {
        }

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "Remove --engine (and --engine-specific key options): .NET's TLS stack does not use OpenSSL engines.";
    }

    /// <summary>
    /// Sending the request failed part-way (exit code 55, CURLE_SEND_ERROR): the connection was reset or closed while
    /// the request or its body was being written.
    /// </summary>
    /// <remarks>
    /// <para>Often transient (network glitch, server restart) or caused by a server that refuses large uploads by closing the
    /// connection. Retry with <c>--retry 3</c>; for uploads check the server's size limit.
    /// Example: <c>catch (CurlSendErrorException ex) when (ex.IsRetryable()) { /* retry */ }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</para>
    /// </remarks>
    /// <seealso cref="CurlReceiveErrorException"/>
    [Serializable]
    public class CurlSendErrorException : CurlException
    {
        /// <summary>
        /// Creates the send error (exit code 55), prefixing <c>Send error:</c> to the socket layer's explanation.
        /// </summary>
        /// <remarks><para>When <paramref name="message"/> has no fix, the suggestion recommends a retry and checking upload limits.</para></remarks>
        /// <param name="message">What the network layer reported, naming the host; optionally followed by fix lines.</param>
        /// <param name="command">The curl command that failed, or null.</param>
        /// <seealso cref="CurlReceiveErrorException"/>
        public CurlSendErrorException(string message, string? command = null)
            : base($"Send error: {message}", 55, command) { }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.ErrorReference;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "The connection broke while the request was being sent: retry (--retry 3), and for large uploads check the server's request size limit.";
    }

    /// <summary>
    /// Receiving the response failed part-way (exit code 56, CURLE_RECV_ERROR): the connection was reset, a proxy
    /// dropped it, or the response framing was broken (bad chunked encoding, oversized headers).
    /// </summary>
    /// <remarks>
    /// <para>Thrown by the transport and the HTTP handler. Transient causes are worth a retry (<c>--retry 3</c>); a reply that
    /// is consistently broken needs a server fix. Inspect it with <c>-v</c> or <c>--trace-ascii -</c>.
    /// Example: <c>catch (CurlReceiveErrorException ex) { Console.WriteLine(ex.WhatHappened); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlSendErrorException"/>
    /// <seealso cref="CurlPartialFileException"/>
    [Serializable]
    public class CurlReceiveErrorException : CurlException
    {
        /// <summary>
        /// Creates the receive error (exit code 56), prefixing <c>Receive error:</c> to the explanation of what broke.
        /// </summary>
        /// <remarks><para>When <paramref name="message"/> has no fix, the suggestion recommends a retry and <c>-v</c>.</para></remarks>
        /// <param name="message">What broke while receiving, naming the host; optionally followed by fix lines.</param>
        /// <param name="command">The curl command that failed, or null.</param>
        /// <seealso cref="CurlSendErrorException"/>
        public CurlReceiveErrorException(string message, string? command = null)
            : base($"Receive error: {message}", 56, command) { }

        /// <summary>
        /// Creates the receive error (exit code 56) and keeps the socket, I/O or <see cref="System.Net.Http.HttpRequestException"/>
        /// that broke the transfer as <see cref="Exception.InnerException"/>.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>catch (IOException io) { throw new CurlReceiveErrorException($"the connection to api.example.com was reset after 1200 bytes ({io.Message}).\nRetry with --retry 3.", command, io); }</c></para>
        /// </remarks>
        /// <param name="message">What broke while receiving, naming the host and the inner cause in words; optionally followed by fix lines.</param>
        /// <param name="command">The curl command that failed, or null.</param>
        /// <param name="innerException">The .NET exception that broke the transfer, or null.</param>
        /// <seealso cref="CurlReceiveErrorException(string, string)"/>
        public CurlReceiveErrorException(string message, string? command, Exception? innerException)
            : base($"Receive error: {message}", 56, command, innerException) { }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Http;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "The connection broke or the server sent data that is not valid HTTP: retry with --retry 3, or inspect the exchange with -v or --trace-ascii -.";
    }

    /// <summary>
    /// Your client certificate or key could not be used (exit code 58, CURLE_SSL_CERTPROBLEM): wrong path, format or
    /// password for <c>--cert</c>/<c>--key</c>.
    /// </summary>
    /// <remarks>
    /// <para>Fixes: check the file exists, the format (<c>--cert-type PEM|DER|P12</c>), the password
    /// (<c>--cert file.p12:password</c>), and that the key matches the certificate.
    /// Example: <c>catch (CurlSslCertificateProblemException ex) { Console.WriteLine(ex.CertificateError); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlSslException"/>
    [Serializable]
    public class CurlSslCertificateProblemException : CurlSslException
    {
        /// <summary>
        /// Creates the error (exit code 58) for an unusable client certificate, with advice on path, type, password and
        /// key.
        /// </summary>
        /// <remarks><para>Message: <c>Problem with local certificate: DETAIL</c>.</para></remarks>
        /// <param name="certError">What is wrong with the local certificate, such as "file not found" or "bad password".</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlSslException.CertificateError"/>
        public CurlSslCertificateProblemException(string certError, string command = null)
            : base($"Problem with local certificate: {certError}", certError, command, 58) // CURLE_SSL_CERTPROBLEM = 58
        {
        }

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "Check the client certificate given with --cert (path, --cert-type PEM/DER/P12, and password as --cert file.p12:password) and that --key names the matching private key.";
    }

    /// <summary>
    /// The cipher list given with <c>--ciphers</c> could not be used (exit code 59, CURLE_SSL_CIPHER);
    /// <see cref="CipherName"/> names it.
    /// </summary>
    /// <remarks>
    /// <para>.NET only lets you choose cipher suites on some platforms (Linux with OpenSSL). Remove <c>--ciphers</c> or use
    /// names this platform knows. Example: <c>catch (CurlSslCipherException ex) { Console.WriteLine(ex.CipherName); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlSslException"/>
    [Serializable]
    public class CurlSslCipherException : CurlSslException
    {
        /// <summary>
        /// The cipher (or cipher list) from <c>--ciphers</c> that could not be used, such as <c>TLS_AES_256_GCM_SHA384</c>.
        /// </summary>
        /// <value>The value as given in the command.</value>
        /// <remarks><para>Check the platform's supported cipher suites before setting one.</para></remarks>
        /// <seealso cref="CurlSslException.CertificateError"/>
        public string CipherName { get; }

        /// <summary>
        /// Creates the error (exit code 59) for a cipher that cannot be used, with advice to remove or change
        /// <c>--ciphers</c>.
        /// </summary>
        /// <remarks><para>Message: <c>Could not use SSL cipher: NAME</c>.</para></remarks>
        /// <param name="cipher">The name of the SSL cipher (or cipher list) that could not be used.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlSslException"/>
        public CurlSslCipherException(string cipher, string command = null)
            : base($"Could not use SSL cipher: {cipher}", null, command, 59) // CURLE_SSL_CIPHER = 59
        {
            CipherName = cipher;
        }

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Remove --ciphers or replace '{CipherName}' with suites this platform supports; .NET only allows choosing cipher suites on Linux with OpenSSL.";
    }

    /// <summary>
    /// The server's certificate was rejected (exit code 60, CURLE_PEER_FAILED_VERIFICATION): untrusted issuer (for
    /// example self-signed), expired, or issued for a different host name.
    /// </summary>
    /// <remarks>
    /// <para>Thrown by the transport when certificate validation fails. Fixes: install the issuing CA or pass it with
    /// <c>--cacert ca.pem</c>, use the host name the certificate was issued for, renew an expired certificate, or for local
    /// testing only use <c>-k/--insecure</c>. Corporate TLS inspection proxies need their root CA installed.</para>
    /// <para>Example: <c>catch (CurlPeerFailedVerificationException ex) { Console.WriteLine(ex.WhatHappened); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md how-to:
    /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlSslConnectErrorException"/>
    /// <seealso cref="CurlSslException"/>
    [Serializable]
    public class CurlPeerFailedVerificationException : CurlSslException
    {
        /// <summary>
        /// Creates the certificate-rejected error (exit code 60), prefixing <c>Peer certificate verification failed:</c>
        /// to the TLS layer's reason.
        /// </summary>
        /// <remarks><para>When <paramref name="message"/> has no fix, the suggestion covers <c>--cacert</c>, host names and <c>-k</c>.</para></remarks>
        /// <param name="message">Why the certificate was rejected, naming the host; optionally followed by fix lines.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlSslException(string, string, string)"/>
        public CurlPeerFailedVerificationException(string message, string command = null)
            : base($"Peer certificate verification failed: {message}", null, command)
        {
        }
    }

    /// <summary>
    /// The response body's encoding could not be decoded (exit code 61, CURLE_BAD_CONTENT_ENCODING): unknown or
    /// corrupt gzip/deflate/br data, or a forbidden Transfer-Encoding.
    /// </summary>
    /// <remarks>
    /// <para>Thrown by the HTTP handler with <c>--compressed</c> or <c>--tr-encoding</c>, and by the transport for illegal
    /// framing. Fixes: drop <c>--compressed</c> to receive the raw bytes, or have the server fix its Content-Encoding.
    /// Example: <c>catch (CurlBadContentEncodingException ex) { Console.WriteLine($"Bad encoding {ex.Encoding}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlWeirdServerReplyException"/>
    [Serializable]
    public class CurlBadContentEncodingException : CurlException
    {
        private readonly bool _framing;

        /// <summary>
        /// The encoding header value that could not be decoded, such as <c>gzip</c>, <c>br</c> or <c>chunked, gzip</c>.
        /// </summary>
        /// <value>The Content-Encoding or Transfer-Encoding value the server sent.</value>
        /// <remarks><para>Compare it with what the server documents; a mislabelled body is a server bug.</para></remarks>
        /// <seealso cref="CurlException.Command"/>
        public string Encoding { get; }

        /// <summary>
        /// Creates the error (exit code 61) for a body whose Content-Encoding could not be decoded, with advice to drop
        /// <c>--compressed</c>.
        /// </summary>
        /// <remarks><para>Message: <c>Bad content encoding: ENCODING</c>.</para></remarks>
        /// <param name="encoding">The content encoding that was not recognized or could not be decoded.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlBadContentEncodingException(string, string, string)"/>
        public CurlBadContentEncodingException(string encoding, string command = null)
            : base($"Bad content encoding: {encoding}", 61, command)
        {
            Encoding = encoding;
        }

        /// <summary>
        /// Creates the error explaining why the server's Transfer-Encoding or Content-Encoding was refused, for framing
        /// that no client option can accept.
        /// </summary>
        /// <remarks><para>The suggestion says this is a server bug (or an attack on HTTP framing).</para></remarks>
        /// <param name="encoding">The encoding header value the server sent.</param>
        /// <param name="detail">What is wrong with it, in words.</param>
        /// <param name="command">The curl command that was executing, or null.</param>
        /// <seealso cref="CurlBadContentEncodingException(string, string)"/>
        public CurlBadContentEncodingException(string encoding, string detail, string? command)
            : base($"Bad content encoding \"{encoding}\": {detail}", 61, command)
        {
            Encoding = encoding;
            _framing = true;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.Http;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion => _framing
            ? "This is a server bug (or an attack on the HTTP framing); no client option accepts it - fix the server or the proxy in between."
            : $"Drop --compressed (or --tr-encoding) to receive the raw bytes, or have the server send a valid '{Encoding}' body or the right Content-Encoding header.";
    }

    /// <summary>
    /// The response is larger than <c>--max-filesize</c> allows (exit code 63, CURLE_FILESIZE_EXCEEDED);
    /// <see cref="MaxSize"/> and <see cref="ActualSize"/> give both sizes in bytes.
    /// </summary>
    /// <remarks>
    /// <para>Thrown by the HTTP handler either before reading (when Content-Length is announced) or while reading. Raise the
    /// limit or download something smaller. Example: <c>catch (CurlFileSizeExceededException ex) { Console.WriteLine($"{ex.ActualSize} &gt; {ex.MaxSize}"); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlOutOfMemoryException"/>
    [Serializable]
    public class CurlFileSizeExceededException : CurlException
    {
        /// <summary>
        /// The size limit set with <c>--max-filesize</c>, in bytes, that the response was not allowed to exceed.
        /// </summary>
        /// <value>A byte count.</value>
        /// <remarks><para>Compare with <see cref="ActualSize"/> to decide on a new limit.</para></remarks>
        /// <seealso cref="ActualSize"/>
        public long MaxSize { get; }

        /// <summary>
        /// The size that exceeded the limit, in bytes: the announced Content-Length, or the bytes read so far.
        /// </summary>
        /// <value>A byte count larger than <see cref="MaxSize"/> (<c>long.MaxValue</c> for an absurdly large announced length).</value>
        /// <remarks><para>Example: <c>Console.WriteLine($"{ex.ActualSize / 1048576} MB");</c></para></remarks>
        /// <seealso cref="MaxSize"/>
        public long ActualSize { get; }

        /// <summary>
        /// Creates the error (exit code 63) with curl's wording <c>Exceeded the maximum allowed file size (MAX)</c> and the
        /// actual size.
        /// </summary>
        /// <remarks><para>Example: <c>throw new CurlFileSizeExceededException(10_000_000, 52_428_800, command);</c></para></remarks>
        /// <param name="maxSize">The maximum allowed file size in bytes (the --max-filesize value).</param>
        /// <param name="actualSize">The actual or announced size in bytes that exceeded the limit.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlOutOfMemoryException"/>
        public CurlFileSizeExceededException(long maxSize, long actualSize, string command = null)
            : base($"Exceeded the maximum allowed file size ({maxSize}) with {actualSize} bytes (file size {actualSize} exceeded maximum {maxSize})", 63, command)
        {
            MaxSize = maxSize;
            ActualSize = actualSize;
        }

        /// <inheritdoc/>
        protected override string? DefaultDocumentationUrl => DocLinks.HowToDownload;

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            $"Raise --max-filesize above {ActualSize} bytes (currently {MaxSize}), or download a smaller resource (for example with -r 0-{Math.Max(0, MaxSize - 1)} for the first part).";
    }

    /// <summary>
    /// TLS was required (<c>--ssl-reqd</c>) but the server does not offer it (exit code 64, CURLE_USE_SSL_FAILED), for
    /// example an FTP server without AUTH TLS.
    /// </summary>
    /// <remarks>
    /// <para>Use a server or port that supports TLS (FTPS), or drop <c>--ssl-reqd</c> if a plain connection is acceptable.
    /// Example: <c>catch (CurlUseSslFailedException ex) { Console.WriteLine(ex.WhatHappened); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlSslException"/>
    [Serializable]
    public class CurlUseSslFailedException : CurlSslException
    {
        /// <summary>
        /// Creates the error (exit code 64) for a server that could not provide the required TLS level, prefixing
        /// <c>Required SSL level failed:</c>.
        /// </summary>
        /// <remarks><para>When <paramref name="message"/> has no fix, the suggestion mentions FTPS and <c>--ssl-reqd</c>.</para></remarks>
        /// <param name="message">What the server answered, naming it; optionally followed by fix lines.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlSslException"/>
        public CurlUseSslFailedException(string message, string command = null)
            : base($"Required SSL level failed: {message}", null, command, 64) // CURLE_USE_SSL_FAILED = 64
        {
        }

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "The server does not offer TLS: use an FTPS-capable server or port (ftps://), or drop --ssl-reqd if an unencrypted connection is acceptable.";
    }

    /// <summary>
    /// The server rejected the login (exit code 67, CURLE_LOGIN_DENIED), for example an FTP or IMAP user/password that
    /// is wrong.
    /// </summary>
    /// <remarks>
    /// <para>A <see cref="CurlAuthenticationException"/>. Check <c>-u user:password</c> (quote it when the password has special
    /// characters) and the account's status. Example: <c>catch (CurlLoginDeniedException ex) { Console.WriteLine(ex.WhatHappened); }</c></para>
    /// <para>Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/auth-errors.md</para>
    /// </remarks>
    /// <seealso cref="CurlAuthenticationException"/>
    /// <seealso cref="CurlAuthErrorException"/>
    [Serializable]
    public class CurlLoginDeniedException : CurlAuthenticationException
    {
        /// <summary>
        /// Creates the login-denied error (exit code 67), prefixing <c>Login denied:</c> to the server's answer.
        /// </summary>
        /// <remarks><para>When <paramref name="message"/> has no fix, the suggestion covers <c>-u</c> and quoting.</para></remarks>
        /// <param name="message">What the server answered, naming the user (never the password); optionally followed by fix lines.</param>
        /// <param name="command">The curl command that caused the exception, or null.</param>
        /// <seealso cref="CurlAuthenticationException"/>
        public CurlLoginDeniedException(string message, string command = null)
            : base($"Login denied: {message}", null, command)
        {
        }

        /// <inheritdoc/>
        protected override string? DefaultSuggestion =>
            "Check the user name and password given with -u user:password (quote the whole value when the password contains special characters such as $, ! or &), and that the account is not locked.";
    }
}
