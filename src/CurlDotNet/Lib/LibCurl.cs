/***************************************************************************
 * CurlDotNet.Lib - Object-oriented libcurl for .NET
 *
 * Programmatic API similar to libcurl (libcurl/include/curl/curl.h)
 *
 * Based on libcurl API by Daniel Stenberg and contributors
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
using System.IO;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;

namespace CurlDotNet.Lib
{
    /// <summary>
    /// <para><b>🔧 LibCurl - Object-Oriented API for Programmatic HTTP</b></para>
    ///
    /// <para>Provides a libcurl-style object-oriented API for .NET developers who prefer
    /// programmatic control over curl command strings. Perfect for building HTTP clients
    /// where you need fine-grained control and reusable configurations.</para>
    ///
    /// <para><b>When to use LibCurl vs Curl String vs CurlRequestBuilder:</b></para>
    /// <list type="bullet">
    /// <item><b>LibCurl</b> - When you need persistent configuration, reusable instances, or object-oriented patterns</item>
    /// <item><b>CurlRequestBuilder</b> - When you want fluent method chaining with one-off requests</item>
    /// <item><b>Curl String</b> - When you have curl commands from docs/examples (paste and go!)</item>
    /// </list>
    ///
    /// <para><b>Quick Example:</b></para>
    /// <code>
    /// using (var curl = new LibCurl())
    /// {
    ///     // Configure defaults for all requests
    ///     curl.WithHeader("Accept", "application/json")
    ///         .WithBearerToken("your-token")
    ///         .WithTimeout(TimeSpan.FromSeconds(30));
    ///
    ///     // Make multiple requests with same configuration
    ///     var user = await curl.GetAsync("https://api.example.com/user");
    ///     var posts = await curl.GetAsync("https://api.example.com/posts");
    /// }
    /// </code>
    /// </summary>
    /// <remarks>
    /// <para>LibCurl provides a stateful, reusable client with default configurations
    /// that persist across requests. This is ideal for scenarios where you make multiple
    /// requests to the same API with the same authentication and headers.</para>
    ///
    /// <para>All methods are thread-safe and can be used concurrently.</para>
    ///
    /// <para>Implements IDisposable - always use with <c>using</c> statement or dispose manually.</para>
    /// </remarks>
    public class LibCurl : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly CurlEngine _engine;
        private readonly Dictionary<string, string> _defaultHeaders;
        private readonly CurlOptions _defaultOptions;

        /// <summary>
        /// Creates a new LibCurl instance.
        /// </summary>
        /// <example>
        /// <code language="csharp">
        /// using (var curl = new LibCurl())
        /// {
        ///     var result = await curl.GetAsync("https://api.example.com/data");
        /// }
        /// </code>
        /// </example>
        public LibCurl()
        {
            _httpClient = new HttpClient();
            _engine = new CurlEngine(_httpClient);
            _defaultHeaders = new Dictionary<string, string>();
            _defaultOptions = new CurlOptions();
        }

        #region HTTP Methods

        /// <summary>
        /// Perform a GET request.
        /// </summary>
        /// <param name="url">The URL to GET</param>
        /// <param name="configure">Optional configuration action for this request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The result of the request</returns>
        /// <example>
        /// <code language="csharp">
        /// using (var curl = new LibCurl())
        /// {
        ///     // Simple GET
        ///     var result = await curl.GetAsync("https://api.example.com/users");
        ///
        ///     // GET with per-request configuration
        ///     var result = await curl.GetAsync("https://api.example.com/users", 
        ///         opts => opts.FollowRedirects = true);
        /// }
        /// </code>
        /// </example>
        public async Task<CurlResult> GetAsync(string url, Action<CurlOptions> configure = null, CancellationToken cancellationToken = default)
        {
            var options = new CurlOptions { Url = url, Method = "GET" };
            MergeDefaults(options);
            configure?.Invoke(options);
            return await _engine.ExecuteAsync(options, cancellationToken);
        }

        /// <summary>
        /// Perform a POST request.
        /// </summary>
        /// <param name="url">The URL to POST to</param>
        /// <param name="data">Data to send (object will be JSON serialized, string sent as-is)</param>
        /// <param name="configure">Optional configuration action for this request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The result of the request</returns>
        /// <example>
        /// <code language="csharp">
        /// using (var curl = new LibCurl())
        /// {
        ///     // POST with object (auto-serialized to JSON)
        ///     var result = await curl.PostAsync("https://api.example.com/users",
        ///         new { name = "John", email = "john@example.com" });
        ///
        ///     // POST with string data
        ///     var result = await curl.PostAsync("https://api.example.com/data",
        ///         "key1=value1&amp;key2=value2",
        ///         opts => opts.Headers["Content-Type"] = "application/x-www-form-urlencoded");
        /// }
        /// </code>
        /// </example>
        public async Task<CurlResult> PostAsync(string url, object data = null, Action<CurlOptions> configure = null, CancellationToken cancellationToken = default)
        {
            var options = new CurlOptions
            {
                Url = url,
                Method = "POST",
                Data = SerializeData(data)
            };
            MergeDefaults(options);
            configure?.Invoke(options);
            return await _engine.ExecuteAsync(options, cancellationToken);
        }

        /// <summary>
        /// Perform a PUT request.
        /// </summary>
        /// <param name="url">The URL to PUT to</param>
        /// <param name="data">Data to send (object will be JSON serialized, string sent as-is)</param>
        /// <param name="configure">Optional configuration action for this request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The result of the request</returns>
        public async Task<CurlResult> PutAsync(string url, object data = null, Action<CurlOptions> configure = null, CancellationToken cancellationToken = default)
        {
            var options = new CurlOptions
            {
                Url = url,
                Method = "PUT",
                Data = SerializeData(data)
            };
            MergeDefaults(options);
            configure?.Invoke(options);
            return await _engine.ExecuteAsync(options, cancellationToken);
        }

        /// <summary>
        /// Perform a PATCH request.
        /// </summary>
        /// <param name="url">The URL to PATCH</param>
        /// <param name="data">Data to send (object will be JSON serialized, string sent as-is)</param>
        /// <param name="configure">Optional configuration action for this request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The result of the request</returns>
        public async Task<CurlResult> PatchAsync(string url, object data = null, Action<CurlOptions> configure = null, CancellationToken cancellationToken = default)
        {
            var options = new CurlOptions
            {
                Url = url,
                Method = "PATCH",
                Data = SerializeData(data)
            };
            MergeDefaults(options);
            configure?.Invoke(options);
            return await _engine.ExecuteAsync(options, cancellationToken);
        }

        /// <summary>
        /// Perform a DELETE request.
        /// </summary>
        /// <param name="url">The URL to DELETE</param>
        /// <param name="configure">Optional configuration action for this request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The result of the request</returns>
        public async Task<CurlResult> DeleteAsync(string url, Action<CurlOptions> configure = null, CancellationToken cancellationToken = default)
        {
            var options = new CurlOptions { Url = url, Method = "DELETE" };
            MergeDefaults(options);
            configure?.Invoke(options);
            return await _engine.ExecuteAsync(options, cancellationToken);
        }

        /// <summary>
        /// Perform a HEAD request.
        /// </summary>
        /// <param name="url">The URL to HEAD</param>
        /// <param name="configure">Optional configuration action for this request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The result of the request</returns>
        public async Task<CurlResult> HeadAsync(string url, Action<CurlOptions> configure = null, CancellationToken cancellationToken = default)
        {
            var options = new CurlOptions { Url = url, Method = "HEAD", HeadOnly = true };
            MergeDefaults(options);
            configure?.Invoke(options);
            return await _engine.ExecuteAsync(options, cancellationToken);
        }

        /// <summary>
        /// Perform a custom request with full control.
        /// </summary>
        /// <param name="options">Fully configured CurlOptions</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The result of the request</returns>
        public async Task<CurlResult> PerformAsync(CurlOptions options, CancellationToken cancellationToken = default)
        {
            MergeDefaults(options);
            return await _engine.ExecuteAsync(options, cancellationToken);
        }

        #endregion

        #region Configuration Methods (Fluent API)

        /// <summary>
        /// Set a default header for all requests.
        /// </summary>
        /// <param name="key">Header name</param>
        /// <param name="value">Header value</param>
        /// <returns>This instance for method chaining</returns>
        /// <example>
        /// <code language="csharp">
        /// using (var curl = new LibCurl())
        /// {
        ///     curl.WithHeader("Accept", "application/json")
        ///         .WithHeader("X-API-Key", "your-key");
        ///     
        ///     // All subsequent requests will include these headers
        ///     var result = await curl.GetAsync("https://api.example.com/data");
        /// }
        /// </code>
        /// </example>
        public LibCurl WithHeader(string key, string value)
        {
            _defaultHeaders[key] = value;
            return this;
        }

        /// <summary>
        /// Set bearer token authentication for all requests.
        /// </summary>
        /// <param name="token">Bearer token</param>
        /// <returns>This instance for method chaining</returns>
        public LibCurl WithBearerToken(string token)
        {
            return WithHeader("Authorization", $"Bearer {token}");
        }

        /// <summary>
        /// Set basic authentication for all requests.
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>This instance for method chaining</returns>
        public LibCurl WithBasicAuth(string username, string password)
        {
            _defaultOptions.Credentials = new NetworkCredential(username, password);
            return this;
        }

        /// <summary>
        /// Set timeout for all requests.
        /// </summary>
        /// <param name="timeout">Timeout duration</param>
        /// <returns>This instance for method chaining</returns>
        public LibCurl WithTimeout(TimeSpan timeout)
        {
            _defaultOptions.MaxTime = (int)timeout.TotalSeconds;
            return this;
        }

        /// <summary>
        /// Set connection timeout for all requests.
        /// </summary>
        /// <param name="timeout">Connection timeout duration</param>
        /// <returns>This instance for method chaining</returns>
        public LibCurl WithConnectTimeout(TimeSpan timeout)
        {
            _defaultOptions.ConnectTimeout = (int)timeout.TotalSeconds;
            return this;
        }

        /// <summary>
        /// Enable following redirects for all requests.
        /// </summary>
        /// <param name="maxRedirects">Maximum number of redirects (default: 50)</param>
        /// <returns>This instance for method chaining</returns>
        public LibCurl WithFollowRedirects(int maxRedirects = 50)
        {
            _defaultOptions.FollowLocation = true;
            _defaultOptions.MaxRedirects = maxRedirects;
            return this;
        }

        /// <summary>
        /// Enable ignoring SSL certificate errors (for testing only).
        /// </summary>
        /// <returns>This instance for method chaining</returns>
        public LibCurl WithInsecureSsl()
        {
            _defaultOptions.Insecure = true;
            return this;
        }

        /// <summary>
        /// Set proxy for all requests.
        /// </summary>
        /// <param name="proxyUrl">Proxy URL (e.g., "http://proxy.example.com:8080")</param>
        /// <param name="username">Optional proxy username</param>
        /// <param name="password">Optional proxy password</param>
        /// <returns>This instance for method chaining</returns>
        public LibCurl WithProxy(string proxyUrl, string username = null, string password = null)
        {
            _defaultOptions.Proxy = proxyUrl;
            if (!string.IsNullOrEmpty(username))
            {
                _defaultOptions.ProxyCredentials = new NetworkCredential(username, password ?? "");
            }
            return this;
        }

        /// <summary>
        /// Set user agent for all requests.
        /// </summary>
        /// <param name="userAgent">User agent string</param>
        /// <returns>This instance for method chaining</returns>
        public LibCurl WithUserAgent(string userAgent)
        {
            _defaultOptions.UserAgent = userAgent;
            return this;
        }

        /// <summary>
        /// Set default output file for all requests.
        /// </summary>
        /// <param name="filePath">Output file path</param>
        /// <returns>This instance for method chaining</returns>
        public LibCurl WithOutputFile(string filePath)
        {
            _defaultOptions.OutputFile = filePath;
            return this;
        }

        /// <summary>
        /// Enable verbose output for debugging.
        /// </summary>
        /// <returns>This instance for method chaining</returns>
        public LibCurl WithVerbose()
        {
            _defaultOptions.Verbose = true;
            return this;
        }

        /// <summary>
        /// Configure default options using an action.
        /// </summary>
        /// <param name="configure">Configuration action</param>
        /// <returns>This instance for method chaining</returns>
        /// <example>
        /// <code language="csharp">
        /// using (var curl = new LibCurl())
        /// {
        ///     curl.Configure(opts => {
        ///         opts.FollowRedirects = true;
        ///         opts.MaxTime = 60;
        ///         opts.Insecure = false;
        ///     });
        /// }
        /// </code>
        /// </example>
        public LibCurl Configure(Action<CurlOptions> configure)
        {
            configure?.Invoke(_defaultOptions);
            return this;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Merge default options into request options.
        /// </summary>
        private void MergeDefaults(CurlOptions options)
        {
            // Merge default headers
            foreach (var header in _defaultHeaders)
            {
#if NETSTANDARD2_0 || NET48
                if (!options.Headers.ContainsKey(header.Key))
                {
                    options.Headers[header.Key] = header.Value;
                }
#else
                options.Headers.TryAdd(header.Key, header.Value);
#endif
            }

            // Merge default options (only if not already set)
            if (string.IsNullOrEmpty(options.UserAgent) && !string.IsNullOrEmpty(_defaultOptions.UserAgent))
                options.UserAgent = _defaultOptions.UserAgent;

            if (options.Credentials == null && _defaultOptions.Credentials != null)
                options.Credentials = _defaultOptions.Credentials;

            if (options.MaxTime == 0 && _defaultOptions.MaxTime > 0)
                options.MaxTime = _defaultOptions.MaxTime;

            if (options.ConnectTimeout == 0 && _defaultOptions.ConnectTimeout > 0)
                options.ConnectTimeout = _defaultOptions.ConnectTimeout;

            if (!options.FollowLocation && _defaultOptions.FollowLocation)
            {
                options.FollowLocation = true;
                options.MaxRedirects = _defaultOptions.MaxRedirects;
        }

            if (!options.Insecure && _defaultOptions.Insecure)
                options.Insecure = true;

            if (string.IsNullOrEmpty(options.Proxy) && !string.IsNullOrEmpty(_defaultOptions.Proxy))
            {
                options.Proxy = _defaultOptions.Proxy;
                options.ProxyCredentials = _defaultOptions.ProxyCredentials;
            }

            if (string.IsNullOrEmpty(options.OutputFile) && !string.IsNullOrEmpty(_defaultOptions.OutputFile))
                options.OutputFile = _defaultOptions.OutputFile;

            if (!options.Verbose && _defaultOptions.Verbose)
                options.Verbose = true;
        }

        /// <summary>
        /// Serialize data object to string.
        /// </summary>
        private string SerializeData(object data)
        {
            if (data == null) return null;
            if (data is string s) return s;

            #if NETSTANDARD2_0
            // Use Newtonsoft.Json for older frameworks
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
            #else
            return System.Text.Json.JsonSerializer.Serialize(data);
            #endif
        }

        #endregion

        /// <summary>
        /// Dispose resources.
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
            _engine?.Dispose();
        }
    }
}