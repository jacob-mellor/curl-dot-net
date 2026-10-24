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

namespace CurlDotNet.Core
{
    /// <summary>
    /// Fluent builder for .NET-specific curl settings.
    /// </summary>
    /// <remarks>
    /// <para>These settings complement curl commands with .NET-specific features.</para>
    /// <para>AI-Usage: Use this for cancellation, progress, retries, and other .NET features.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var settings = new CurlSettings()
    ///     .WithTimeout(30)
    ///     .WithRetries(3)
    ///     .WithProgress((percent, total, current) =&gt; Console.WriteLine($"{percent}%"))
    ///     .WithCancellation(cancellationToken);
    ///
    /// var result = await Curl.Execute("curl https://example.com", settings);
    /// </code>
    /// </example>
    public class CurlSettings
    {
        /// <summary>
        /// Cancellation token for the operation.
        /// </summary>
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        /// <summary>
        /// Maximum time in seconds for the entire operation.
        /// </summary>
        public int? MaxTimeSeconds { get; set; }

        /// <summary>
        /// Connection timeout in seconds.
        /// </summary>
        public int? ConnectTimeoutSeconds { get; set; }

        /// <summary>
        /// Whether to follow redirects.
        /// </summary>
        public bool? FollowRedirects { get; set; }

        /// <summary>
        /// Whether to ignore SSL certificate errors.
        /// </summary>
        public bool? Insecure { get; set; }

        /// <summary>
        /// Number of retry attempts on failure.
        /// </summary>
        public int RetryCount { get; set; } = 0;

        /// <summary>
        /// Delay between retries in milliseconds.
        /// </summary>
        public int RetryDelayMs { get; set; } = 1000;

        /// <summary>
        /// Progress callback (percent, totalBytes, currentBytes).
        /// </summary>
        public Action<double, long, long> OnProgress { get; set; }

        /// <summary>
        /// Callback for each redirect.
        /// </summary>
        public Action<string> OnRedirect { get; set; }

        /// <summary>
        /// Additional headers to add to the request.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Proxy settings.
        /// </summary>
        public IWebProxy Proxy { get; set; }

        /// <summary>
        /// Custom user agent string.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Cookie container for maintaining session.
        /// </summary>
        public CookieContainer Cookies { get; set; }

        /// <summary>
        /// Whether to automatically decompress response.
        /// </summary>
        public bool AutomaticDecompression { get; set; } = true;

        /// <summary>
        /// Buffer size for download operations.
        /// </summary>
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
        /// Set cancellation token.
        /// </summary>
        public CurlSettings WithCancellation(CancellationToken token)
        {
            CancellationToken = token;
            return this;
        }

        /// <summary>
        /// Set maximum time for operation.
        /// </summary>
        public CurlSettings WithTimeout(int seconds)
        {
            MaxTimeSeconds = seconds;
            return this;
        }

        /// <summary>
        /// Set connection timeout.
        /// </summary>
        public CurlSettings WithConnectTimeout(int seconds)
        {
            ConnectTimeoutSeconds = seconds;
            return this;
        }

        /// <summary>
        /// Enable or disable following redirects.
        /// </summary>
        public CurlSettings WithFollowRedirects(bool follow = true)
        {
            FollowRedirects = follow;
            return this;
        }

        /// <summary>
        /// Enable or disable SSL certificate validation.
        /// </summary>
        public CurlSettings WithInsecure(bool insecure = true)
        {
            Insecure = insecure;
            return this;
        }

        /// <summary>
        /// Set retry behavior.
        /// </summary>
        public CurlSettings WithRetries(int count, int delayMs = 1000)
        {
            RetryCount = count;
            RetryDelayMs = delayMs;
            return this;
        }

        /// <summary>
        /// Set progress callback.
        /// </summary>
        public CurlSettings WithProgress(Action<double, long, long> callback)
        {
            OnProgress = callback;
            return this;
        }

        /// <summary>
        /// Set redirect callback.
        /// </summary>
        public CurlSettings WithRedirectHandler(Action<string> callback)
        {
            OnRedirect = callback;
            return this;
        }

        /// <summary>
        /// Add a header.
        /// </summary>
        public CurlSettings WithHeader(string key, string value)
        {
            Headers[key] = value;
            return this;
        }

        /// <summary>
        /// Add multiple headers.
        /// </summary>
        public CurlSettings WithHeaders(Dictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                Headers[header.Key] = header.Value;
            }
            return this;
        }

        /// <summary>
        /// Set proxy.
        /// </summary>
        public CurlSettings WithProxy(string proxyUrl)
        {
            Proxy = new WebProxy(proxyUrl);
            return this;
        }

        /// <summary>
        /// Set proxy with credentials.
        /// </summary>
        public CurlSettings WithProxy(string proxyUrl, string username, string password)
        {
            Proxy = new WebProxy(proxyUrl)
            {
                Credentials = new NetworkCredential(username, password)
            };
            return this;
        }

        /// <summary>
        /// Set custom user agent.
        /// </summary>
        public CurlSettings WithUserAgent(string userAgent)
        {
            UserAgent = userAgent;
            return this;
        }

        /// <summary>
        /// Use cookie container for session management.
        /// </summary>
        public CurlSettings WithCookies(CookieContainer container = null)
        {
            Cookies = container ?? new CookieContainer();
            return this;
        }

        /// <summary>
        /// Set automatic decompression.
        /// </summary>
        public CurlSettings WithAutoDecompression(bool enable = true)
        {
            AutomaticDecompression = enable;
            return this;
        }

        /// <summary>
        /// Set buffer size for downloads.
        /// </summary>
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
        /// Create default settings from global Curl settings.
        /// </summary>
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