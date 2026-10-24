/***************************************************************************
 * CurlSettings - Fluent settings builder for curl operations
 *
 * .NET-specific settings that complement curl commands
 *
 * Based on curl (https://curl.se) by Daniel Stenberg and contributors
 * Original curl Copyright (C) 1996-2025, Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024-2025 Jacob Mellor and IronSoftware
 *
 * By Jacob Mellor
 * GitHub: https://github.com/jacob-mellor
 * Sponsored by IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Per-call .NET settings (cancellation, timeouts, retries, a progress callback, extra headers) that you pass
    /// next to a curl command string instead of writing them as flags.
    /// </summary>
    /// <remarks>
    /// <para>Pass an instance to <see cref="Curl.ExecuteAsync(string, CurlSettings)"/>,
    /// <see cref="Curl.Execute(string, CurlSettings)"/> or <see cref="CurlRequestBuilder.ExecuteAsync(CurlSettings, CancellationToken)"/>.
    /// Values set here override the same option in the command: <see cref="MaxTimeSeconds"/> wins over <c>-m</c>,
    /// <see cref="Headers"/> replace a <c>-H</c> header of the same name, and so on. Settings left at null/0 leave the
    /// command (and the <c>Curl.Default*</c> globals) in charge.</para>
    /// <para>What is applied today: <see cref="CancellationToken"/>, <see cref="MaxTimeSeconds"/> (<c>-m</c>),
    /// <see cref="ConnectTimeoutSeconds"/> (<c>--connect-timeout</c>), <see cref="FollowRedirects"/> (<c>-L</c>),
    /// <see cref="Insecure"/> (<c>-k</c>), <see cref="RetryCount"/>/<see cref="RetryDelayMs"/>
    /// (<c>--retry N --retry-delay S --retry-all-errors</c>), <see cref="OnProgress"/>, <see cref="Headers"/>
    /// (<c>-H</c>) and <see cref="Flavor"/>. <see cref="OnRedirect"/>, <see cref="Proxy"/>, <see cref="UserAgent"/>,
    /// <see cref="Cookies"/>, <see cref="AutomaticDecompression"/> and <see cref="BufferSize"/> are stored but not
    /// yet used by the transfer engine; write the equivalent curl flag (<c>-x</c>, <c>-A</c>, <c>-b</c>/<c>-c</c>,
    /// <c>--compressed</c>) in the command instead.</para>
    /// <para>An instance is a plain mutable object and is not thread-safe while you are changing it; once built
    /// it can be shared by concurrent calls because the engine only reads it.</para>
    /// <para>Example: <c>var settings = new CurlSettings().WithTimeout(30).WithRetries(3).WithProgress((pct, got, total) =&gt; Console.Write($"\r{pct:F0}%")).WithCancellation(token);</c>
    /// then <c>var result = await Curl.ExecuteAsync("curl -O https://example.com/big.zip", settings);</c>.</para>
    /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var settings = new CurlSettings()
    ///     .WithTimeout(30)
    ///     .WithRetries(3)
    ///     .WithProgress((percent, received, total) =&gt; Console.WriteLine(percent &lt; 0 ? $"{received} bytes" : $"{percent:F0}%"))
    ///     .WithCancellation(cancellationToken);
    ///
    /// var result = await Curl.ExecuteAsync("curl https://example.com", settings);
    /// </code>
    /// </example>
    /// <seealso cref="Curl.ExecuteAsync(string, CurlSettings)"/>
    /// <seealso cref="CurlOptions"/>
    /// <seealso cref="FromDefaults"/>
    public class CurlSettings
    {
        /// <summary>
        /// The token that cancels the transfer; when it fires, the call throws <see cref="OperationCanceledException"/>
        /// (its InnerException is a <see cref="CurlAbortedByCallbackException"/>, curl exit code 42).
        /// </summary>
        /// <value>Defaults to <see cref="CancellationToken.None"/> (never cancelled).</value>
        /// <remarks>
        /// <para>Used when the method you call does not take its own token. <see cref="CurlRequestBuilder.ExecuteAsync(CurlSettings, CancellationToken)"/>
        /// prefers the token passed directly when it can be cancelled.</para>
        /// <para>Example: <c>using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)); settings.CancellationToken = cts.Token;</c>
        /// For a time limit that behaves like curl (exit code 28), prefer <see cref="MaxTimeSeconds"/>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</para>
        /// </remarks>
        /// <seealso cref="WithCancellation(CancellationToken)"/>
        /// <seealso cref="MaxTimeSeconds"/>
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        /// <summary>
        /// Limit for the whole transfer in whole seconds, like curl's <c>-m</c>/<c>--max-time</c>; when it expires the
        /// call throws <see cref="CurlOperationTimeoutException"/> (exit code 28).
        /// </summary>
        /// <value>Seconds; null (the default) keeps the command's <c>-m</c> value or <see cref="Curl.DefaultMaxTimeSeconds"/>.
        /// 0 or a negative number means no limit.</value>
        /// <remarks>
        /// <para>Copied to <see cref="CurlOptions.MaxTime"/>, overriding any <c>-m</c> in the command. For fractions of a
        /// second write <c>-m 0.5</c> in the command instead.</para>
        /// <para>Example: <c>new CurlSettings { MaxTimeSeconds = 30 }</c> is the same as adding <c>--max-time 30</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="WithTimeout(int)"/>
        /// <seealso cref="ConnectTimeoutSeconds"/>
        /// <seealso cref="Curl.DefaultMaxTimeSeconds"/>
        /// <seealso cref="CurlOptions.MaxTime"/>
        public int? MaxTimeSeconds { get; set; }

        /// <summary>
        /// Limit for establishing the connection (DNS, TCP and TLS) in whole seconds, like curl's
        /// <c>--connect-timeout</c>; exceeding it throws <see cref="CurlOperationTimeoutException"/> (exit code 28).
        /// </summary>
        /// <value>Seconds; null (the default) keeps the command's <c>--connect-timeout</c> or
        /// <see cref="Curl.DefaultConnectTimeoutSeconds"/>.</value>
        /// <remarks>
        /// <para>Copied to <see cref="CurlOptions.ConnectTimeout"/>. It does not limit the download itself; combine it
        /// with <see cref="MaxTimeSeconds"/> for that.</para>
        /// <para>Example: <c>new CurlSettings { ConnectTimeoutSeconds = 5, MaxTimeSeconds = 60 }</c> fails fast when the
        /// host is down but allows a slow download a full minute.</para>
        /// </remarks>
        /// <seealso cref="WithConnectTimeout(int)"/>
        /// <seealso cref="MaxTimeSeconds"/>
        /// <seealso cref="CurlOptions.ConnectTimeout"/>
        public int? ConnectTimeoutSeconds { get; set; }

        /// <summary>
        /// Whether to follow HTTP 3xx redirects, like curl's <c>-L</c>/<c>--location</c>; null leaves the
        /// command (or <see cref="Curl.DefaultFollowRedirects"/>) in charge.
        /// </summary>
        /// <value>true to follow redirects, false to return the 3xx response itself, null (default) to keep the command's choice.</value>
        /// <remarks>
        /// <para>Copied to <see cref="CurlOptions.FollowLocation"/>. Redirects are limited by <c>--max-redirs</c>
        /// (50 by default); exceeding it throws <see cref="CurlTooManyRedirectsException"/> (exit code 47).</para>
        /// <para>Example: <c>new CurlSettings { FollowRedirects = true }</c> makes <c>curl http://github.com</c> end on the https page.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-follow-redirects-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="WithFollowRedirects(bool)"/>
        /// <seealso cref="CurlOptions.FollowLocation"/>
        /// <seealso cref="Curl.DefaultFollowRedirects"/>
        public bool? FollowRedirects { get; set; }

        /// <summary>
        /// Whether to skip TLS certificate verification, like curl's <c>-k</c>/<c>--insecure</c>; only for testing
        /// against self-signed or local certificates.
        /// </summary>
        /// <value>true to accept any certificate, false to verify, null (default) to keep the command's choice or
        /// <see cref="Curl.DefaultInsecure"/>.</value>
        /// <remarks>
        /// <para>Copied to <see cref="CurlOptions.Insecure"/>. With verification on, an untrusted certificate throws
        /// <see cref="CurlPeerFailedVerificationException"/> (exit code 60); install the CA or use <c>--cacert</c> rather
        /// than turning verification off in production.</para>
        /// <para>Example: <c>new CurlSettings { Insecure = true }</c> for <c>https://localhost:5001</c> with the ASP.NET dev certificate.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="WithInsecure(bool)"/>
        /// <seealso cref="CurlOptions.Insecure"/>
        /// <seealso cref="Curl.DefaultInsecure"/>
        public bool? Insecure { get; set; }

        /// <summary>
        /// How many times to retry a failed transfer (any failure, not only transient ones), like curl's
        /// <c>--retry N --retry-all-errors</c>; 0 (the default) means no retries.
        /// </summary>
        /// <value>Number of extra attempts after the first; 0 or negative disables retrying.</value>
        /// <remarks>
        /// <para>When greater than 0 it sets <see cref="CurlOptions.Retry"/>, <see cref="CurlOptions.RetryDelay"/> (from
        /// <see cref="RetryDelayMs"/>) and <see cref="CurlOptions.RetryAllErrors"/>. The last failure is thrown when all
        /// attempts fail.</para>
        /// <para>Example: <c>new CurlSettings { RetryCount = 3, RetryDelayMs = 2000 }</c> tries up to four times, two seconds apart.</para>
        /// </remarks>
        /// <seealso cref="WithRetries(int, int)"/>
        /// <seealso cref="RetryDelayMs"/>
        /// <seealso cref="CurlOptions.Retry"/>
        public int RetryCount { get; set; } = 0;

        /// <summary>
        /// Pause between retries in milliseconds, like curl's <c>--retry-delay</c> (which takes seconds); only used
        /// when <see cref="RetryCount"/> is greater than 0.
        /// </summary>
        /// <value>Milliseconds; default 1000. Converted to seconds for <see cref="CurlOptions.RetryDelay"/>; 0 lets the
        /// engine use curl's exponential back-off (1, 2, 4 ... seconds).</value>
        /// <remarks>
        /// <para>A <c>Retry-After</c> header from the server takes precedence, like in curl.</para>
        /// <para>Example: <c>settings.WithRetries(5, delayMs: 500)</c>.</para>
        /// </remarks>
        /// <seealso cref="RetryCount"/>
        /// <seealso cref="WithRetries(int, int)"/>
        /// <seealso cref="CurlOptions.RetryDelay"/>
        public int RetryDelayMs { get; set; } = 1000;

        /// <summary>
        /// Callback invoked for every chunk of the response body as it downloads, with
        /// <c>(percent, bytesReceived, totalBytes)</c>.
        /// </summary>
        /// <value>Null (the default) for no callback. Copied to <see cref="CurlOptions.ProgressHandler"/>.</value>
        /// <remarks>
        /// <para>Arguments: <c>percent</c> is 0 to 100, or -1 when the server did not send a <c>Content-Length</c>
        /// (chunked responses); <c>bytesReceived</c> is the body bytes received so far; <c>totalBytes</c> is the announced
        /// length, or -1 when unknown. The last call has <c>bytesReceived</c> equal to the body size.</para>
        /// <para>It runs on the thread pool, once per network read (up to 80 KB), for HTTP and HTTPS response bodies;
        /// it is not called for HEAD requests, uploads or FTP/file transfers. Keep it fast and marshal to the UI thread
        /// yourself. If it throws, the transfer stops with <see cref="CurlAbortedByCallbackException"/> (exit code 42)
        /// carrying your exception as <see cref="Exception.InnerException"/> - throwing is the way to abort a download.</para>
        /// <para>Example: <c>OnProgress = (pct, got, total) =&gt; Console.Write(pct &lt; 0 ? $"\r{got:N0} bytes" : $"\r{pct:F1}% of {total:N0}")</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="WithProgress(Action{double, long, long})"/>
        /// <seealso cref="CurlOptions.ProgressHandler"/>
        public Action<double, long, long> OnProgress { get; set; }

        /// <summary>
        /// Callback meant to receive each redirect URL; stored but not yet called by the transfer engine.
        /// </summary>
        /// <value>Null by default.</value>
        /// <remarks>
        /// <para>To see the redirect chain today, run the command with <c>-L -i</c> (the headers of every hop are in
        /// <see cref="CurlResult.Body"/>) or with <c>-w '%{url_effective}'</c> for the final URL.</para>
        /// <para>Example: <c>settings.WithRedirectHandler(url =&gt; log.Add(url));</c> (reserved for a future version).</para>
        /// </remarks>
        /// <seealso cref="WithRedirectHandler(Action{string})"/>
        /// <seealso cref="FollowRedirects"/>
        public Action<string> OnRedirect { get; set; }

        /// <summary>
        /// Extra request headers added to (or replacing headers of the same name in) the command, like extra
        /// <c>-H "Name: value"</c> flags.
        /// </summary>
        /// <value>Header name to value; empty by default. A name already set by the command is overwritten
        /// (names compare case-insensitively when applied).</value>
        /// <remarks>
        /// <para>Copied into <see cref="CurlOptions.Headers"/>. Do not include the colon in the name.</para>
        /// <para>Example: <c>settings.Headers["Authorization"] = "Bearer " + token;</c> keeps the token out of the
        /// command string (and out of <see cref="CurlResult.Command"/> logs).</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="WithHeader(string, string)"/>
        /// <seealso cref="WithHeaders(Dictionary{string, string})"/>
        /// <seealso cref="CurlOptions.Headers"/>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// A .NET proxy object; stored but not yet used by the transfer engine - write <c>-x http://proxy:3128</c>
        /// in the command (or use <see cref="CurlRequestBuilder.WithProxy(string)"/>) instead.
        /// </summary>
        /// <value>Null by default; <see cref="WithProxy(string)"/> sets a <see cref="WebProxy"/>.</value>
        /// <remarks>
        /// <para>Example of what works today: <c>await Curl.ExecuteAsync("curl -x http://proxy.local:3128 -U user:pass https://example.com")</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="WithProxy(string)"/>
        /// <seealso cref="CurlOptions.Proxy"/>
        public IWebProxy Proxy { get; set; }

        /// <summary>
        /// A User-Agent string; stored but not yet applied - use <c>-A "MyApp/1.0"</c> in the command, or set
        /// <c>Headers["User-Agent"]</c>, which is applied.
        /// </summary>
        /// <value>Null by default (curl sends <c>curl/VERSION</c>, see <see cref="Curl.DefaultUserAgent"/>).</value>
        /// <remarks>
        /// <para>Example that works today: <c>settings.WithHeader("User-Agent", "MyApp/1.0")</c>.</para>
        /// </remarks>
        /// <seealso cref="WithUserAgent(string)"/>
        /// <seealso cref="CurlOptions.UserAgent"/>
        public string UserAgent { get; set; }

        /// <summary>
        /// A .NET cookie container; stored but not yet used - use curl's <c>-b cookies.txt -c cookies.txt</c> to
        /// keep a session across calls.
        /// </summary>
        /// <value>Null by default; <see cref="WithCookies(CookieContainer)"/> creates one.</value>
        /// <remarks>
        /// <para>Example that works today: <c>await Curl.ExecuteAsync("curl -b jar.txt -c jar.txt https://example.com/login -d user=me")</c>,
        /// then the next call with the same <c>-b jar.txt</c> sends the session cookie.</para>
        /// <para>Cookbook: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/cookies.md</para>
        /// </remarks>
        /// <seealso cref="WithCookies(CookieContainer)"/>
        /// <seealso cref="CurlOptions.CookieJar"/>
        public CookieContainer Cookies { get; set; }

        /// <summary>
        /// Intended to switch automatic response decompression; stored but not yet used - add <c>--compressed</c>
        /// to the command to request and decode gzip/deflate/brotli bodies.
        /// </summary>
        /// <value>Default true; currently has no effect.</value>
        /// <remarks>
        /// <para>Like curl, CurlDotNet only decompresses when the command says <c>--compressed</c> (see
        /// <see cref="CurlOptions.Compressed"/>).</para>
        /// <para>Example: <c>curl --compressed https://api.example.com/large.json</c>.</para>
        /// </remarks>
        /// <seealso cref="WithAutoDecompression(bool)"/>
        /// <seealso cref="CurlOptions.Compressed"/>
        public bool AutomaticDecompression { get; set; } = true;

        /// <summary>
        /// Intended read buffer size for downloads; stored but not used - the HTTP engine reads in 80 KB chunks
        /// regardless.
        /// </summary>
        /// <value>Bytes; default 8192; currently has no effect.</value>
        /// <remarks>
        /// <para>Kept for source compatibility. To bound memory for very large downloads, write to a file with
        /// <c>-o file</c> and use <see cref="CurlOptions.MaxFileSize"/> (<c>--max-filesize</c>) as a guard.</para>
        /// <para>Example: <c>settings.WithBufferSize(65536)</c> compiles but does not change the transfer.</para>
        /// </remarks>
        /// <seealso cref="WithBufferSize(int)"/>
        public int BufferSize { get; set; } = 8192;

        /// <summary>
        /// The shell the command string was written for, which decides how its quotes, escapes, line
        /// continuations and variables are read; <c>null</c> (the default) means <see cref="Curl.DefaultFlavor"/>.
        /// </summary>
        /// <value>
        /// <c>null</c> to use <see cref="Curl.DefaultFlavor"/> (normally <see cref="CurlFlavor.Auto"/>), or one of
        /// <see cref="CurlFlavor.Auto"/>, <see cref="CurlFlavor.Posix"/>, <see cref="CurlFlavor.WindowsCmd"/>,
        /// <see cref="CurlFlavor.PowerShell"/> or <see cref="CurlFlavor.InvokeWebRequest"/>.
        /// </value>
        /// <remarks>
        /// <para>Only used by the methods that take a command string together with these settings, such as
        /// <see cref="Curl.ExecuteAsync(string, CurlSettings)"/>. The flavor actually used is reported afterwards in
        /// <see cref="CurlOptions.Flavor"/>.</para>
        /// <para>Example: <c>new CurlSettings { Flavor = CurlFlavor.WindowsCmd }</c> reads
        /// <c>curl -d "{\"a\":1}" https://api.example.com</c> with cmd.exe rules, so the body is <c>{"a":1}</c>.</para>
        /// </remarks>
        /// <seealso cref="WithFlavor(CurlFlavor)"/>
        /// <seealso cref="Curl.DefaultFlavor"/>
        public CurlFlavor? Flavor { get; set; }

        #region Fluent Builder Methods

        /// <summary>
        /// Sets <see cref="CancellationToken"/> so that cancelling <paramref name="token"/> stops the transfer, and
        /// returns these settings for chaining.
        /// </summary>
        /// <remarks>
        /// <para>A cancelled transfer throws <see cref="OperationCanceledException"/> - the standard .NET contract, so
        /// <c>catch (OperationCanceledException)</c> works - with a <see cref="CurlAbortedByCallbackException"/> (exit code 42)
        /// as its InnerException.</para>
        /// <para>Example: <c>new CurlSettings().WithCancellation(HttpContext.RequestAborted)</c> stops the outgoing call
        /// when the browser that asked for it disconnects.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</para>
        /// </remarks>
        /// <param name="token">Any token; <see cref="CancellationToken.None"/> removes cancellation.</param>
        /// <returns>This same <see cref="CurlSettings"/> instance.</returns>
        /// <seealso cref="CancellationToken"/>
        /// <seealso cref="WithTimeout(int)"/>
        public CurlSettings WithCancellation(CancellationToken token)
        {
            CancellationToken = token;
            return this;
        }

        /// <summary>
        /// Limits the whole transfer to <paramref name="seconds"/> (curl <c>-m</c>/<c>--max-time</c>) and returns these
        /// settings for chaining.
        /// </summary>
        /// <remarks>
        /// <para>Sets <see cref="MaxTimeSeconds"/>, which overrides any <c>-m</c> in the command. A transfer that runs
        /// longer throws <see cref="CurlOperationTimeoutException"/> (exit code 28).</para>
        /// <para>Example: <c>await Curl.ExecuteAsync("curl https://slow.example.com", new CurlSettings().WithTimeout(10));</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</para>
        /// </remarks>
        /// <param name="seconds">Whole seconds, for example 30; 0 or negative means no limit.</param>
        /// <returns>This same <see cref="CurlSettings"/> instance.</returns>
        /// <seealso cref="MaxTimeSeconds"/>
        /// <seealso cref="WithConnectTimeout(int)"/>
        /// <seealso cref="CurlRequestBuilder.WithTimeout(TimeSpan)"/>
        public CurlSettings WithTimeout(int seconds)
        {
            MaxTimeSeconds = seconds;
            return this;
        }

        /// <summary>
        /// Limits only the connection phase to <paramref name="seconds"/> (curl <c>--connect-timeout</c>) and returns
        /// these settings for chaining.
        /// </summary>
        /// <remarks>
        /// <para>Sets <see cref="ConnectTimeoutSeconds"/>. A host that does not accept the connection in time throws
        /// <see cref="CurlOperationTimeoutException"/> (exit code 28).</para>
        /// <para>Example: <c>new CurlSettings().WithConnectTimeout(3).WithTimeout(120)</c> - fail fast when the server is
        /// down, but allow two minutes for a large download.</para>
        /// </remarks>
        /// <param name="seconds">Whole seconds, for example 5.</param>
        /// <returns>This same <see cref="CurlSettings"/> instance.</returns>
        /// <seealso cref="ConnectTimeoutSeconds"/>
        /// <seealso cref="WithTimeout(int)"/>
        /// <seealso cref="CurlRequestBuilder.WithConnectTimeout(TimeSpan)"/>
        public CurlSettings WithConnectTimeout(int seconds)
        {
            ConnectTimeoutSeconds = seconds;
            return this;
        }

        /// <summary>
        /// Turns following of 3xx redirects on or off (curl <c>-L</c>) and returns these settings for chaining.
        /// </summary>
        /// <remarks>
        /// <para>Sets <see cref="FollowRedirects"/>. Too many hops throw <see cref="CurlTooManyRedirectsException"/> (exit code 47).</para>
        /// <para>Example: <c>new CurlSettings().WithFollowRedirects()</c> for short links such as <c>https://bit.ly/...</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-follow-redirects-csharp/README.md</para>
        /// </remarks>
        /// <param name="follow">true (default) to follow redirects; false to return the 3xx response itself.</param>
        /// <returns>This same <see cref="CurlSettings"/> instance.</returns>
        /// <seealso cref="FollowRedirects"/>
        /// <seealso cref="CurlRequestBuilder.FollowRedirects(bool)"/>
        public CurlSettings WithFollowRedirects(bool follow = true)
        {
            FollowRedirects = follow;
            return this;
        }

        /// <summary>
        /// Turns TLS certificate verification off (curl <c>-k</c>) or back on, and returns these settings for chaining.
        /// </summary>
        /// <remarks>
        /// <para>Sets <see cref="Insecure"/>. Use only for development against self-signed certificates; in production
        /// trust the CA instead.</para>
        /// <para>Example: <c>await Curl.ExecuteAsync("curl https://localhost:5001/health", new CurlSettings().WithInsecure());</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md</para>
        /// </remarks>
        /// <param name="insecure">true (default) to skip verification; false to verify certificates.</param>
        /// <returns>This same <see cref="CurlSettings"/> instance.</returns>
        /// <seealso cref="Insecure"/>
        /// <seealso cref="CurlRequestBuilder.Insecure(bool)"/>
        public CurlSettings WithInsecure(bool insecure = true)
        {
            Insecure = insecure;
            return this;
        }

        /// <summary>
        /// Retries any failed transfer up to <paramref name="count"/> more times with a fixed pause (curl
        /// <c>--retry N --retry-delay S --retry-all-errors</c>), and returns these settings for chaining.
        /// </summary>
        /// <remarks>
        /// <para>Sets <see cref="RetryCount"/> and <see cref="RetryDelayMs"/>. If every attempt fails, the exception of
        /// the last attempt is thrown.</para>
        /// <para>Example: <c>new CurlSettings().WithRetries(3, delayMs: 2000)</c> makes up to four attempts, two seconds apart.</para>
        /// </remarks>
        /// <param name="count">Extra attempts after the first, for example 3; 0 disables retrying.</param>
        /// <param name="delayMs">Pause between attempts in milliseconds; default 1000.</param>
        /// <returns>This same <see cref="CurlSettings"/> instance.</returns>
        /// <seealso cref="RetryCount"/>
        /// <seealso cref="RetryDelayMs"/>
        /// <seealso cref="CurlOptions.Retry"/>
        public CurlSettings WithRetries(int count, int delayMs = 1000)
        {
            RetryCount = count;
            RetryDelayMs = delayMs;
            return this;
        }

        /// <summary>
        /// Registers a callback that receives <c>(percent, bytesReceived, totalBytes)</c> for every chunk of the response
        /// body, and returns these settings for chaining.
        /// </summary>
        /// <remarks>
        /// <para>Sets <see cref="OnProgress"/>. <c>percent</c> is 0-100, or -1 when the server sent no
        /// <c>Content-Length</c>; <c>bytesReceived</c> counts body bytes so far; <c>totalBytes</c> is the announced size or
        /// -1. Throwing from the callback aborts the transfer with <see cref="CurlAbortedByCallbackException"/> (exit code 42).</para>
        /// <para>Example: <c>await Curl.ExecuteAsync("curl -o big.zip https://example.com/big.zip", new CurlSettings().WithProgress((pct, got, total) =&gt; Console.Write($"\r{pct:F0}% ({got:N0}/{total:N0})")));</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <param name="callback">Called on a thread-pool thread once per network read; null removes the callback.</param>
        /// <returns>This same <see cref="CurlSettings"/> instance.</returns>
        /// <seealso cref="OnProgress"/>
        /// <seealso cref="CurlOptions.ProgressHandler"/>
        public CurlSettings WithProgress(Action<double, long, long> callback)
        {
            OnProgress = callback;
            return this;
        }

        /// <summary>
        /// Stores a callback for redirect URLs (see <see cref="OnRedirect"/>; not yet called by the engine) and returns
        /// these settings for chaining.
        /// </summary>
        /// <remarks>
        /// <para>To inspect redirects today, run with <c>-L -i</c> and read the headers of each hop from
        /// <see cref="CurlResult.Body"/>, or use <c>-w '%{url_effective}'</c>.</para>
        /// <para>Example: <c>settings.WithRedirectHandler(url =&gt; Console.WriteLine("-&gt; " + url));</c></para>
        /// </remarks>
        /// <param name="callback">Receives each redirect target URL; null removes it.</param>
        /// <returns>This same <see cref="CurlSettings"/> instance.</returns>
        /// <seealso cref="OnRedirect"/>
        /// <seealso cref="WithFollowRedirects(bool)"/>
        public CurlSettings WithRedirectHandler(Action<string> callback)
        {
            OnRedirect = callback;
            return this;
        }

        /// <summary>
        /// Adds (or replaces) one request header, like an extra <c>-H "key: value"</c>, and returns these settings for chaining.
        /// </summary>
        /// <remarks>
        /// <para>Stored in <see cref="Headers"/> and applied over the command's headers. Calling it again with the same
        /// <paramref name="key"/> replaces the value.</para>
        /// <para>Example: <c>new CurlSettings().WithHeader("Authorization", "Bearer " + token).WithHeader("Accept", "application/json")</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md</para>
        /// </remarks>
        /// <param name="key">The header name without the colon, such as <c>Accept</c> or <c>X-Api-Key</c>.</param>
        /// <param name="value">The header value, such as <c>application/json</c>.</param>
        /// <returns>This same <see cref="CurlSettings"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null; pass a header name.</exception>
        /// <seealso cref="Headers"/>
        /// <seealso cref="WithHeaders(Dictionary{string, string})"/>
        /// <seealso cref="CurlRequestBuilder.WithHeader(string, string)"/>
        public CurlSettings WithHeader(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), CurlException.Compose(
                    $"CurlSettings.WithHeader was called with a null header name (value: '{ErrorText.Preview(value, 40)}').",
                    "Pass the header name without the colon, for example WithHeader(\"Accept\", \"application/json\").",
                    DocLinks.HowToHeaders));
            }
            Headers[key] = value;
            return this;
        }

        /// <summary>
        /// Adds (or replaces) several request headers at once, like several <c>-H</c> flags, and returns these settings
        /// for chaining.
        /// </summary>
        /// <remarks>
        /// <para>Each entry is copied into <see cref="Headers"/>; existing names are overwritten.</para>
        /// <para>Example: <c>settings.WithHeaders(new Dictionary&lt;string, string&gt; { ["Accept"] = "application/json", ["X-Api-Key"] = key });</c></para>
        /// </remarks>
        /// <param name="headers">Header name to value; must not be null (an empty dictionary is fine).</param>
        /// <returns>This same <see cref="CurlSettings"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="headers"/> is null; pass an empty dictionary instead.</exception>
        /// <seealso cref="WithHeader(string, string)"/>
        /// <seealso cref="Headers"/>
        public CurlSettings WithHeaders(Dictionary<string, string> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers), CurlException.Compose(
                    "CurlSettings.WithHeaders was called with a null dictionary.",
                    "Pass a dictionary of header name to value (an empty one adds nothing), or call WithHeader(name, value) per header.",
                    DocLinks.HowToHeaders));
            }
            foreach (var header in headers)
            {
                Headers[header.Key] = header.Value;
            }
            return this;
        }

        /// <summary>
        /// Stores a <see cref="WebProxy"/> for <paramref name="proxyUrl"/> in <see cref="Proxy"/> (not yet used by the
        /// engine - write <c>-x URL</c> in the command) and returns these settings for chaining.
        /// </summary>
        /// <remarks>
        /// <para>The URL is validated immediately. For a proxy that is actually used today, put
        /// <c>-x http://proxy.local:3128</c> in the command or call <see cref="CurlRequestBuilder.WithProxy(string)"/>.</para>
        /// <para>Example: <c>settings.WithProxy("http://proxy.local:3128")</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md</para>
        /// </remarks>
        /// <param name="proxyUrl">An absolute proxy URL such as <c>http://proxy.local:3128</c>.</param>
        /// <returns>This same <see cref="CurlSettings"/> instance.</returns>
        /// <exception cref="CurlMalformedUrlException"><paramref name="proxyUrl"/> is null or not a valid URL (exit code 3);
        /// write it as <c>http://host:port</c>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</exception>
        /// <seealso cref="Proxy"/>
        /// <seealso cref="WithProxy(string, string, string)"/>
        /// <seealso cref="CurlOptions.Proxy"/>
        public CurlSettings WithProxy(string proxyUrl)
        {
            Proxy = CreateProxy(proxyUrl, nameof(WithProxy));
            return this;
        }

        /// <summary>
        /// Stores a <see cref="WebProxy"/> with user name and password in <see cref="Proxy"/> (not yet used by the
        /// engine - write <c>-x URL -U user:pass</c> in the command) and returns these settings for chaining.
        /// </summary>
        /// <remarks>
        /// <para>For proxy authentication that is actually sent today use <c>curl -x http://proxy:3128 -U user:pass URL</c>
        /// or <see cref="CurlRequestBuilder.WithProxy(string, string, string)"/>.</para>
        /// <para>Example: <c>settings.WithProxy("http://proxy.corp:8080", "alice", secret)</c>.</para>
        /// </remarks>
        /// <param name="proxyUrl">An absolute proxy URL such as <c>http://proxy.corp:8080</c>.</param>
        /// <param name="username">Proxy user name (curl <c>-U user:...</c>).</param>
        /// <param name="password">Proxy password.</param>
        /// <returns>This same <see cref="CurlSettings"/> instance.</returns>
        /// <exception cref="CurlMalformedUrlException"><paramref name="proxyUrl"/> is null or not a valid URL (exit code 3).
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</exception>
        /// <seealso cref="WithProxy(string)"/>
        /// <seealso cref="CurlOptions.ProxyCredentials"/>
        public CurlSettings WithProxy(string proxyUrl, string username, string password)
        {
            var proxy = CreateProxy(proxyUrl, nameof(WithProxy));
            proxy.Credentials = new NetworkCredential(username, password);
            Proxy = proxy;
            return this;
        }

        private static WebProxy CreateProxy(string proxyUrl, string method)
        {
            try
            {
                return new WebProxy(proxyUrl);
            }
            catch (Exception ex) when (ex is UriFormatException || ex is ArgumentException)
            {
                throw new CurlMalformedUrlException(proxyUrl ?? "(null)",
                    $"CurlSettings.{method} could not read the proxy URL '{ErrorText.SafeUrl(proxyUrl)}' ({ex.Message}).\n" +
                    "Write it as an absolute URL with scheme, host and port, for example http://proxy.local:3128.",
                    null);
            }
        }

        /// <summary>
        /// Stores a User-Agent in <see cref="UserAgent"/> (not yet applied - use <c>-A</c> or
        /// <c>WithHeader("User-Agent", ...)</c>) and returns these settings for chaining.
        /// </summary>
        /// <remarks>
        /// <para>Example that changes the request today: <c>settings.WithHeader("User-Agent", "MyApp/2.1 (+https://myapp.example)")</c>.</para>
        /// </remarks>
        /// <param name="userAgent">The product token string, such as <c>MyApp/1.0</c>.</param>
        /// <returns>This same <see cref="CurlSettings"/> instance.</returns>
        /// <seealso cref="UserAgent"/>
        /// <seealso cref="WithHeader(string, string)"/>
        public CurlSettings WithUserAgent(string userAgent)
        {
            UserAgent = userAgent;
            return this;
        }

        /// <summary>
        /// Stores a cookie container in <see cref="Cookies"/> (not yet used - use <c>-b jar.txt -c jar.txt</c>) and returns
        /// these settings for chaining.
        /// </summary>
        /// <remarks>
        /// <para>Example that keeps a session today: run every call with <c>-b session.txt -c session.txt</c>.</para>
        /// <para>Cookbook: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/cookies.md</para>
        /// </remarks>
        /// <param name="container">The container to use; null (default) creates a new empty <see cref="CookieContainer"/>.</param>
        /// <returns>This same <see cref="CurlSettings"/> instance.</returns>
        /// <seealso cref="Cookies"/>
        /// <seealso cref="CurlOptions.CookieJar"/>
        public CurlSettings WithCookies(CookieContainer container = null)
        {
            Cookies = container ?? new CookieContainer();
            return this;
        }

        /// <summary>
        /// Sets <see cref="AutomaticDecompression"/> (currently no effect - add <c>--compressed</c> to the command to
        /// decompress) and returns these settings for chaining.
        /// </summary>
        /// <remarks>
        /// <para>Example that works today: <c>curl --compressed https://api.example.com/data</c>.</para>
        /// </remarks>
        /// <param name="enable">true (default) or false; stored only.</param>
        /// <returns>This same <see cref="CurlSettings"/> instance.</returns>
        /// <seealso cref="AutomaticDecompression"/>
        /// <seealso cref="CurlOptions.Compressed"/>
        public CurlSettings WithAutoDecompression(bool enable = true)
        {
            AutomaticDecompression = enable;
            return this;
        }

        /// <summary>
        /// Sets <see cref="BufferSize"/> (currently no effect - bodies are read in 80 KB chunks) and returns these
        /// settings for chaining.
        /// </summary>
        /// <remarks>
        /// <para>Kept for source compatibility. Example: <c>settings.WithBufferSize(64 * 1024)</c>.</para>
        /// </remarks>
        /// <param name="size">Buffer size in bytes; stored only.</param>
        /// <returns>This same <see cref="CurlSettings"/> instance.</returns>
        /// <seealso cref="BufferSize"/>
        public CurlSettings WithBufferSize(int size)
        {
            BufferSize = size;
            return this;
        }

        /// <summary>
        /// Sets the shell the command string was written for (see <see cref="CurlFlavor"/>) and returns these
        /// settings for chaining.
        /// </summary>
        /// <param name="flavor">
        /// <see cref="CurlFlavor.Auto"/> to detect it from the text; <see cref="CurlFlavor.Posix"/> for bash/zsh
        /// (macOS, Linux, Git Bash); <see cref="CurlFlavor.WindowsCmd"/> for <c>curl.exe</c> in cmd.exe;
        /// <see cref="CurlFlavor.PowerShell"/> for <c>curl.exe</c> in PowerShell; or
        /// <see cref="CurlFlavor.InvokeWebRequest"/> for PowerShell's <c>Invoke-WebRequest</c> / <c>curl</c> alias
        /// syntax. Overrides <see cref="Curl.DefaultFlavor"/> for requests made with these settings.
        /// </param>
        /// <returns>This <see cref="CurlSettings"/> instance.</returns>
        /// <remarks>
        /// <para>Example: <c>await Curl.ExecuteAsync("curl.exe -d '{\"a\":1}' https://api.example.com", new CurlSettings().WithFlavor(CurlFlavor.PowerShell).WithTimeout(30));</c>
        /// sends the body <c>{"a":1}</c>. For a one-off call without other settings,
        /// <see cref="Curl.ExecuteAsync(string, CurlFlavor)"/> is shorter.</para>
        /// </remarks>
        /// <seealso cref="Flavor"/>
        /// <seealso cref="Curl.DefaultFlavor"/>
        /// <seealso cref="CurlOptions.Flavor"/>
        public CurlSettings WithFlavor(CurlFlavor flavor)
        {
            Flavor = flavor;
            return this;
        }

        #endregion

        /// <summary>
        /// Creates settings pre-filled from the global <c>Curl.Default*</c> properties (timeouts, redirects,
        /// certificate checking, flavor), so you can start from the process-wide defaults and tweak one value.
        /// </summary>
        /// <remarks>
        /// <para>Copies <see cref="Curl.DefaultMaxTimeSeconds"/> and <see cref="Curl.DefaultConnectTimeoutSeconds"/>
        /// (0 becomes null, meaning no limit), <see cref="Curl.DefaultFollowRedirects"/>, <see cref="Curl.DefaultInsecure"/>
        /// and <see cref="Curl.DefaultFlavor"/>. It is a snapshot: later changes to the globals do not affect it.</para>
        /// <para>Example: <c>var settings = CurlSettings.FromDefaults().WithTimeout(5);</c> keeps every global default
        /// except the time limit.</para>
        /// </remarks>
        /// <returns>A new <see cref="CurlSettings"/> whose <see cref="MaxTimeSeconds"/>, <see cref="ConnectTimeoutSeconds"/>,
        /// <see cref="FollowRedirects"/>, <see cref="Insecure"/> and <see cref="Flavor"/> mirror the globals; everything else
        /// has its default value.</returns>
        /// <seealso cref="Curl.DefaultMaxTimeSeconds"/>
        /// <seealso cref="Curl.DefaultFollowRedirects"/>
        public static CurlSettings FromDefaults()
        {
            return new CurlSettings
            {
                MaxTimeSeconds = Curl.DefaultMaxTimeSeconds > 0 ? Curl.DefaultMaxTimeSeconds : (int?)null,
                ConnectTimeoutSeconds = Curl.DefaultConnectTimeoutSeconds > 0 ? Curl.DefaultConnectTimeoutSeconds : (int?)null,
                FollowRedirects = Curl.DefaultFollowRedirects,
                Insecure = Curl.DefaultInsecure,
                Flavor = Curl.DefaultFlavor
            };
        }
    }
}
