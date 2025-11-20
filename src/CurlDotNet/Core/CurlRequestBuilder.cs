/***************************************************************************
 * CurlRequestBuilder - Fluent builder API for curl requests
 *
 * For developers who prefer programmatic API over curl strings
 * Based on curl (https://curl.se) by Daniel Stenberg and contributors
 * Original curl Copyright (C) 1996-2025, Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024-2025 Jacob Mellor and IronSoftware
 *
 * By Jacob Mellor
 * Sponsored by IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CurlDotNet.Core
{
    /// <summary>
    /// <para><b>🎨 Fluent Builder API - Build curl requests programmatically!</b></para>
    ///
    /// <para>For developers who prefer a fluent API over curl command strings.
    /// This builder lets you construct HTTP requests using method chaining,
    /// perfect for IntelliSense and compile-time checking.</para>
    ///
    /// <para><b>When to use Builder vs Curl String:</b></para>
    /// <list type="bullet">
    /// <item><b>Use Builder</b> - When building requests dynamically, need IntelliSense, or prefer type safety</item>
    /// <item><b>Use Curl String</b> - When you have curl commands from docs/examples (paste and go!)</item>
    /// </list>
    ///
    /// <para><b>Quick Example:</b></para>
    /// <code>
    /// // Build a request fluently
    /// var result = await CurlRequestBuilder
    ///     .Get("https://api.example.com/users")
    ///     .WithHeader("Accept", "application/json")
    ///     .WithHeader("Authorization", "Bearer token123")
    ///     .WithTimeout(TimeSpan.FromSeconds(30))
    ///     .ExecuteAsync();
    ///
    /// // Same as: curl -H 'Accept: application/json' -H 'Authorization: Bearer token123' --max-time 30 https://api.example.com/users
    /// </code>
    /// </summary>
    /// <remarks>
    /// <para>The builder provides a type-safe, IntelliSense-friendly way to build HTTP requests.
    /// All builder methods return the builder itself for method chaining.</para>
    ///
    /// <para>You can convert a builder to a curl command string using <see cref="ToCurlCommand()"/>.</para>
    /// </remarks>
    public class CurlRequestBuilder
    {
        private readonly CurlOptions _options = new CurlOptions();

        private CurlRequestBuilder(string url, string method = "GET")
        {
            _options.Url = url ?? throw new ArgumentNullException(nameof(url));
            _options.Method = method;
        }

        #region Factory Methods

        /// <summary>
        /// Create a GET request builder.
        /// </summary>
        /// <param name="url">The URL to GET</param>
        /// <returns>Builder for method chaining</returns>
        /// <example>
        /// <code language="csharp">
        /// // Simple GET request
        /// var result = await CurlRequestBuilder
        ///     .Get("https://api.github.com/users/octocat")
        ///     .ExecuteAsync();
        ///
        /// // GET with headers
        /// var result = await CurlRequestBuilder
        ///     .Get("https://api.example.com/data")
        ///     .WithHeader("Accept", "application/json")
        ///     .WithHeader("X-API-Key", "your-key")
        ///     .ExecuteAsync();
        ///
        /// // GET with authentication
        /// var result = await CurlRequestBuilder
        ///     .Get("https://api.example.com/protected")
        ///     .WithBearerToken("your-token")
        ///     .FollowRedirects()
        ///     .ExecuteAsync();
        /// </code>
        /// </example>
        public static CurlRequestBuilder Get(string url)
        {
            return new CurlRequestBuilder(url, "GET");
        }

        /// <summary>
        /// Create a POST request builder.
        /// </summary>
        /// <param name="url">The URL to POST to</param>
        /// <returns>Builder for method chaining</returns>
        /// <example>
        /// <code language="csharp">
        /// // POST with JSON
        /// var result = await CurlRequestBuilder
        ///     .Post("https://api.example.com/users")
        ///     .WithJson(new { name = "John", email = "john@example.com" })
        ///     .ExecuteAsync();
        ///
        /// // POST with form data
        /// var result = await CurlRequestBuilder
        ///     .Post("https://api.example.com/login")
        ///     .WithFormData(new Dictionary&lt;string, string&gt; {
        ///         { "username", "user123" },
        ///         { "password", "pass456" }
        ///     })
        ///     .ExecuteAsync();
        ///
        /// // POST with raw string data
        /// var result = await CurlRequestBuilder
        ///     .Post("https://api.example.com/data")
        ///     .WithData("key1=value1&amp;key2=value2")
        ///     .WithHeader("Content-Type", "application/x-www-form-urlencoded")
        ///     .ExecuteAsync();
        /// </code>
        /// </example>
        public static CurlRequestBuilder Post(string url)
        {
            return new CurlRequestBuilder(url, "POST");
        }

        /// <summary>
        /// Create a PUT request builder.
        /// </summary>
        public static CurlRequestBuilder Put(string url)
        {
            return new CurlRequestBuilder(url, "PUT");
        }

        /// <summary>
        /// Create a DELETE request builder.
        /// </summary>
        public static CurlRequestBuilder Delete(string url)
        {
            return new CurlRequestBuilder(url, "DELETE");
        }

        /// <summary>
        /// Create a PATCH request builder.
        /// </summary>
        public static CurlRequestBuilder Patch(string url)
        {
            return new CurlRequestBuilder(url, "PATCH");
        }

        /// <summary>
        /// Create a HEAD request builder.
        /// </summary>
        public static CurlRequestBuilder Head(string url)
        {
            return new CurlRequestBuilder(url, "HEAD");
        }

        /// <summary>
        /// Create a custom method request builder.
        /// </summary>
        public static CurlRequestBuilder Request(string method, string url)
        {
            return new CurlRequestBuilder(url, method.ToUpper());
        }

        #endregion

        #region Headers

        /// <summary>
        /// Add a header to the request.
        /// </summary>
        /// <param name="name">Header name (e.g., "Content-Type")</param>
        /// <param name="value">Header value (e.g., "application/json")</param>
        /// <returns>Builder for method chaining</returns>
        /// <example>
        /// <code language="csharp">
        /// // Add Content-Type header
        /// var result = await CurlRequestBuilder
        ///     .Post("https://api.example.com/data")
        ///     .WithHeader("Content-Type", "application/json")
        ///     .WithJson(new { key = "value" })
        ///     .ExecuteAsync();
        ///
        /// // Add custom API key header
        /// var result = await CurlRequestBuilder
        ///     .Get("https://api.example.com/protected")
        ///     .WithHeader("X-API-Key", "your-api-key-here")
        ///     .WithHeader("X-Client-Version", "1.0.0")
        ///     .ExecuteAsync();
        ///
        /// // Add Accept header for API versioning
        /// var result = await CurlRequestBuilder
        ///     .Get("https://api.github.com/user")
        ///     .WithHeader("Accept", "application/vnd.github.v3+json")
        ///     .WithBearerToken("your-token")
        ///     .ExecuteAsync();
        /// </code>
        /// </example>
        public CurlRequestBuilder WithHeader(string name, string value)
        {
            _options.Headers[name] = value;
            return this;
        }

        /// <summary>
        /// Add multiple headers at once.
        /// </summary>
        public CurlRequestBuilder WithHeaders(Dictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                _options.Headers[header.Key] = header.Value;
            }
            return this;
        }

        /// <summary>
        /// Set the User-Agent header.
        /// </summary>
        public CurlRequestBuilder WithUserAgent(string userAgent)
        {
            _options.UserAgent = userAgent;
            return this;
        }

        /// <summary>
        /// Set the Referer header.
        /// </summary>
        public CurlRequestBuilder WithReferer(string referer)
        {
            _options.Referer = referer;
            return this;
        }

        #endregion

        #region Body/Data

        /// <summary>
        /// Add POST/PUT data as string.
        /// </summary>
        public CurlRequestBuilder WithData(string data)
        {
            _options.Data = data;
            if (string.IsNullOrEmpty(_options.Method) || _options.Method == "GET")
                _options.Method = "POST";
            return this;
        }

        /// <summary>
        /// Add POST/PUT data as JSON (automatically serializes and sets Content-Type).
        /// </summary>
        /// <param name="data">The object to serialize as JSON. Can be any class, anonymous object, or built-in type.</param>
        /// <returns>Builder for method chaining</returns>
        /// <example>
        /// <code language="csharp">
        /// // POST with anonymous object
        /// var result = await CurlRequestBuilder
        ///     .Post("https://api.example.com/users")
        ///     .WithJson(new { name = "John", email = "john@example.com", age = 30 })
        ///     .ExecuteAsync();
        ///
        /// // POST with typed class
        /// public class User {
        ///     public string Name { get; set; }
        ///     public string Email { get; set; }
        /// }
        /// var user = new User { Name = "John", Email = "john@example.com" };
        /// var result = await CurlRequestBuilder
        ///     .Post("https://api.example.com/users")
        ///     .WithJson(user)
        ///     .ExecuteAsync();
        ///
        /// // PUT with JSON update
        /// var result = await CurlRequestBuilder
        ///     .Put("https://api.example.com/users/123")
        ///     .WithJson(new { name = "Jane", email = "jane@example.com" })
        ///     .WithBearerToken("your-token")
        ///     .ExecuteAsync();
        /// </code>
        /// </example>
        public CurlRequestBuilder WithJson(object data)
        {
#if NETSTANDARD2_0
            _options.Data = Newtonsoft.Json.JsonConvert.SerializeObject(data);
#else
            _options.Data = System.Text.Json.JsonSerializer.Serialize(data);
#endif
            _options.Headers["Content-Type"] = "application/json";
            if (string.IsNullOrEmpty(_options.Method) || _options.Method == "GET")
                _options.Method = "POST";
            return this;
        }

        /// <summary>
        /// Add form data (application/x-www-form-urlencoded).
        /// </summary>
        public CurlRequestBuilder WithFormData(Dictionary<string, string> formData)
        {
            var pairs = new List<string>();
            foreach (var kvp in formData)
            {
                pairs.Add($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
            }
            _options.Data = string.Join("&", pairs);
            _options.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            if (string.IsNullOrEmpty(_options.Method) || _options.Method == "GET")
                _options.Method = "POST";
            return this;
        }

        /// <summary>
        /// Add multipart form data with file uploads.
        /// </summary>
        public CurlRequestBuilder WithMultipartForm(Dictionary<string, string> fields, Dictionary<string, string> files = null)
        {
            _options.FormData = fields ?? new Dictionary<string, string>();
            _options.Files = files ?? new Dictionary<string, string>();
            if (string.IsNullOrEmpty(_options.Method) || _options.Method == "GET")
                _options.Method = "POST";
            return this;
        }

        /// <summary>
        /// Add binary data for upload.
        /// </summary>
        public CurlRequestBuilder WithBinaryData(byte[] data)
        {
            _options.BinaryData = data;
            if (string.IsNullOrEmpty(_options.Method) || _options.Method == "GET")
                _options.Method = "POST";
            return this;
        }

        /// <summary>
        /// Upload a file.
        /// </summary>
        public CurlRequestBuilder WithFile(string fieldName, string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            _options.Files[fieldName] = filePath;
            if (string.IsNullOrEmpty(_options.Method) || _options.Method == "GET")
                _options.Method = "POST";
            return this;
        }

        #endregion

        #region Authentication

        /// <summary>
        /// Set basic authentication (username:password).
        /// </summary>
        public CurlRequestBuilder WithBasicAuth(string username, string password)
        {
            _options.Credentials = new NetworkCredential(username, password);
            return this;
        }

        /// <summary>
        /// Set bearer token authentication.
        /// </summary>
        public CurlRequestBuilder WithBearerToken(string token)
        {
            _options.Headers["Authorization"] = $"Bearer {token}";
            return this;
        }

        /// <summary>
        /// Set custom authentication header.
        /// </summary>
        public CurlRequestBuilder WithAuth(string authHeader)
        {
            _options.Headers["Authorization"] = authHeader;
            return this;
        }

        #endregion

        #region Options

        /// <summary>
        /// Set timeout for the entire operation.
        /// </summary>
        public CurlRequestBuilder WithTimeout(TimeSpan timeout)
        {
            _options.MaxTime = (int)timeout.TotalSeconds;
            return this;
        }

        /// <summary>
        /// Set connection timeout.
        /// </summary>
        public CurlRequestBuilder WithConnectTimeout(TimeSpan timeout)
        {
            _options.ConnectTimeout = (int)timeout.TotalSeconds;
            return this;
        }

        /// <summary>
        /// Enable following redirects (301, 302, etc.).
        /// </summary>
        public CurlRequestBuilder FollowRedirects(bool follow = true)
        {
            _options.FollowLocation = follow;
            return this;
        }

        /// <summary>
        /// Set maximum number of redirects to follow.
        /// </summary>
        public CurlRequestBuilder WithMaxRedirects(int maxRedirects)
        {
            _options.MaxRedirects = maxRedirects;
            return this;
        }

        /// <summary>
        /// Ignore SSL certificate errors (not recommended for production!).
        /// </summary>
        public CurlRequestBuilder Insecure(bool insecure = true)
        {
            _options.Insecure = insecure;
            return this;
        }

        /// <summary>
        /// Set proxy URL.
        /// </summary>
        public CurlRequestBuilder WithProxy(string proxyUrl)
        {
            _options.Proxy = proxyUrl;
            return this;
        }

        /// <summary>
        /// Set proxy with authentication.
        /// </summary>
        public CurlRequestBuilder WithProxy(string proxyUrl, string username, string password)
        {
            _options.Proxy = proxyUrl;
            _options.ProxyCredentials = new NetworkCredential(username, password);
            return this;
        }

        /// <summary>
        /// Set cookie string.
        /// </summary>
        public CurlRequestBuilder WithCookie(string cookie)
        {
            _options.Cookie = cookie;
            return this;
        }

        /// <summary>
        /// Set cookie jar file path.
        /// </summary>
        public CurlRequestBuilder WithCookieJar(string cookieJarPath)
        {
            _options.CookieJar = cookieJarPath;
            return this;
        }

        /// <summary>
        /// Upload a file (multipart/form-data).
        /// </summary>
        /// <param name="parameterName">The form field name</param>
        /// <param name="filePath">Path to the file</param>
        public CurlRequestBuilder WithUploadFile(string parameterName, string filePath)
        {
            _options.Files[parameterName] = filePath;
            return this;
        }

        /// <summary>
        /// Upload a file (multipart/form-data). Alias for WithUploadFile.
        /// </summary>
        /// <param name="filePath">Path to the file</param>
        /// <returns></returns>
        public CurlRequestBuilder WithFile(string filePath)
        {
            return WithUploadFile("file", filePath);
        }

        /// <summary>
        /// Save output to a file.
        /// </summary>
        /// <param name="filePath">Path to save the file</param>
        public CurlRequestBuilder WithOutput(string filePath)
        {
            _options.OutputFile = filePath;
            return this;
        }

        /// <summary>
        /// Save output to a file. Alias for WithOutput.
        /// </summary>
        /// <param name="filePath">Path to save the file</param>
        /// <returns></returns>
        public CurlRequestBuilder SaveToFile(string filePath)
        {
            return WithOutput(filePath);
        }

        /// <summary>
        /// Use remote filename for output (like curl -O).
        /// </summary>
        public CurlRequestBuilder SaveWithRemoteName()
        {
            _options.UseRemoteFileName = true;
            return this;
        }

        /// <summary>
        /// Include headers in response output (like curl -i).
        /// </summary>
        public CurlRequestBuilder IncludeHeaders(bool include = true)
        {
            _options.IncludeHeaders = include;
            return this;
        }

        /// <summary>
        /// Enable verbose output (like curl -v).
        /// </summary>
        public CurlRequestBuilder Verbose(bool verbose = true)
        {
            _options.Verbose = verbose;
            return this;
        }

        /// <summary>
        /// Enable silent mode (like curl -s).
        /// </summary>
        public CurlRequestBuilder Silent(bool silent = true)
        {
            _options.Silent = silent;
            return this;
        }

        /// <summary>
        /// Fail on HTTP errors (like curl -f).
        /// </summary>
        public CurlRequestBuilder FailOnError(bool fail = true)
        {
            _options.FailOnError = fail;
            return this;
        }

        /// <summary>
        /// Set HTTP version (1.0, 1.1, or 2.0).
        /// </summary>
        public CurlRequestBuilder WithHttpVersion(string version)
        {
            _options.HttpVersion = version;
            return this;
        }

        /// <summary>
        /// Enable compression (gzip, deflate, etc.).
        /// </summary>
        public CurlRequestBuilder Compressed(bool compressed = true)
        {
            _options.Compressed = compressed;
            return this;
        }

        /// <summary>
        /// Set range for partial downloads (like curl -r).
        /// </summary>
        public CurlRequestBuilder WithRange(string range)
        {
            _options.Range = range;
            return this;
        }

        #endregion

        #region Execution

        /// <summary>
        /// Execute the request synchronously.
        /// </summary>
        public CurlResult Execute()
        {
            return ExecuteAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Execute the request asynchronously.
        /// </summary>
        public async Task<CurlResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var engine = new CurlEngine();
            try
            {
                _options.OriginalCommand = ToCurlCommand();
                return await engine.ExecuteAsync(_options, cancellationToken);
            }
            finally
            {
                engine.Dispose();
            }
        }

        /// <summary>
        /// Execute the request with custom settings.
        /// </summary>
        public async Task<CurlResult> ExecuteAsync(CurlSettings settings, CancellationToken cancellationToken = default)
        {
            var engine = new CurlEngine();
            try
            {
                _options.OriginalCommand = ToCurlCommand();
                return await engine.ExecuteAsync(_options, cancellationToken);
            }
            finally
            {
                engine.Dispose();
            }
        }

        /// <summary>
        /// Convert this builder to a curl command string.
        /// Useful for debugging or logging what will be executed.
        /// </summary>
        public string ToCurlCommand()
        {
            var parts = new List<string> { "curl" };

            // URL (last, like curl expects)
            if (!string.IsNullOrEmpty(_options.Method) && _options.Method != "GET")
            {
                parts.Add($"-X {_options.Method}");
            }

            // Headers
            foreach (var header in _options.Headers)
            {
                parts.Add($"-H '{header.Key}: {header.Value}'");
            }

            // User-Agent
            if (!string.IsNullOrEmpty(_options.UserAgent))
            {
                parts.Add($"-A '{_options.UserAgent}'");
            }

            // Referer
            if (!string.IsNullOrEmpty(_options.Referer))
            {
                parts.Add($"-e '{_options.Referer}'");
            }

            // Data
            if (!string.IsNullOrEmpty(_options.Data))
            {
                parts.Add($"-d '{_options.Data}'");
            }

            // Form data
            if (_options.FormData != null && _options.FormData.Count > 0)
            {
                foreach (var field in _options.FormData)
                {
                    parts.Add($"-F '{field.Key}={field.Value}'");
                }
            }

            // Files
            if (_options.Files != null && _options.Files.Count > 0)
            {
                foreach (var file in _options.Files)
                {
                    parts.Add($"-F '{file.Key}=@{file.Value}'");
                }
            }

            // Authentication
            if (_options.Credentials != null)
            {
                parts.Add($"-u '{_options.Credentials.UserName}:{_options.Credentials.Password}'");
            }

            // Options
            if (_options.FollowLocation)
                parts.Add("-L");
            if (_options.Insecure)
                parts.Add("-k");
            if (_options.Verbose)
                parts.Add("-v");
            if (_options.Silent)
                parts.Add("-s");
            if (_options.FailOnError)
                parts.Add("-f");
            if (_options.IncludeHeaders)
                parts.Add("-i");
            if (_options.Compressed)
                parts.Add("--compressed");

            if (_options.MaxTime.HasValue && _options.MaxTime.Value > 0)
                parts.Add($"--max-time {_options.MaxTime.Value}");
            if (_options.ConnectTimeout.HasValue && _options.ConnectTimeout.Value > 0)
                parts.Add($"--connect-timeout {_options.ConnectTimeout.Value}");
            if (_options.MaxRedirects > 0 && _options.MaxRedirects != 50)
                parts.Add($"--max-redirs {_options.MaxRedirects}");

            if (!string.IsNullOrEmpty(_options.Proxy))
                parts.Add($"-x {_options.Proxy}");

            if (!string.IsNullOrEmpty(_options.Cookie))
                parts.Add($"-b '{_options.Cookie}'");
            if (!string.IsNullOrEmpty(_options.CookieJar))
                parts.Add($"-c {_options.CookieJar}");

            if (!string.IsNullOrEmpty(_options.OutputFile))
                parts.Add($"-o {_options.OutputFile}");
            if (_options.UseRemoteFileName)
                parts.Add("-O");

            // URL goes last
            parts.Add(_options.Url);

            return string.Join(" ", parts);
        }

        /// <summary>
        /// Get the underlying options object (for advanced scenarios).
        /// </summary>
        public CurlOptions GetOptions() => _options.Clone();

        #endregion
    }
}

