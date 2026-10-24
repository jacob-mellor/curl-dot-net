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
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Lib
{
    /// <summary>
    /// A reusable HTTP client in the style of libcurl's easy handle: set defaults once (headers, token,
    /// timeouts, proxy) and make many requests with <see cref="GetAsync"/>, <see cref="PostAsync"/> and friends.
    /// Transfer failures are returned in <see cref="CurlResult.Exception"/> instead of being thrown.
    /// </summary>
    /// <remarks>
    /// <para>Configure the instance with the fluent <c>With...</c> methods (or <see cref="Configure"/>), then call the
    /// request methods. Each request starts from its own <see cref="CurlOptions"/>; the defaults fill in only what
    /// the request did not set itself, and the optional <c>configure</c> callback of each request method runs last,
    /// so it can override anything for that one call.</para>
    /// <para>Error model, like libcurl's <c>CURLcode</c>: a failed transfer (DNS, connection refused, timeout, TLS,
    /// write error) does not throw. The method returns a <see cref="CurlResult"/> with <c>StatusCode == 0</c>, an
    /// empty body and the <see cref="CurlException"/> in <see cref="CurlResult.Exception"/> (its message says what
    /// happened and how to fix it). HTTP error statuses (404, 500) are normal results with that status code. Only
    /// caller cancellation throws (<see cref="OperationCanceledException"/>). If you prefer exceptions, use
    /// <see cref="CurlRequestBuilder"/> or <see cref="Curl.ExecuteAsync(string)"/>, or call
    /// <c>if (result.Exception != null) throw result.Exception;</c>.</para>
    /// <para>The transport behaves like the curl tool: redirects are followed only after
    /// <see cref="WithFollowRedirects"/>, there is no time limit unless <see cref="WithTimeout"/> is set, responses are
    /// not decompressed unless asked, and <see cref="WithProxy"/> / <see cref="WithInsecureSsl"/> apply to every request.</para>
    /// <para>Thread safety: configure the instance first; after that, any number of requests may run concurrently
    /// on it. Do not call the <c>With...</c> methods or <see cref="Configure"/> while requests are running.</para>
    /// <para>Example: <c>using var api = new LibCurl().WithHeader("Accept", "application/json").WithBearerToken(token).WithTimeout(TimeSpan.FromSeconds(30));</c>
    /// then <c>var user = await api.GetAsync("https://api.example.com/user");</c> and
    /// <c>var posts = await api.GetAsync("https://api.example.com/posts");</c> both send the same headers and limit.</para>
    /// <para>How this maps to libcurl's <c>curl_easy_setopt</c> options:
    /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/reference/libcurl-mapping.md</para>
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// using (var curl = new LibCurl())
    /// {
    ///     curl.WithHeader("Accept", "application/json")
    ///         .WithBearerToken("your-token")
    ///         .WithTimeout(TimeSpan.FromSeconds(30));
    ///
    ///     var user = await curl.GetAsync("https://api.example.com/user");
    ///     if (user.Exception != null)
    ///         Console.WriteLine(user.Exception.Message);   // what happened, how to fix it, documentation link
    ///     else if (user.IsSuccess)
    ///         Console.WriteLine(user.Body);
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="CurlRequestBuilder"/>
    /// <seealso cref="Curl"/>
    /// <seealso cref="CurlOptions"/>
    /// <seealso cref="CurlResult.Exception"/>
    public class LibCurl : IDisposable
    {
        private readonly CurlEngine _engine;
        private readonly Dictionary<string, string> _defaultHeaders;
        private readonly CurlOptions _defaultOptions;

        /// <summary>
        /// Creates a client with no defaults: curl's behaviour (no redirects followed, no time limit, TLS
        /// certificates verified, proxy from the <c>http_proxy</c>/<c>https_proxy</c> environment variables).
        /// </summary>
        /// <remarks>
        /// <para>The instance owns a connection pool, so create one per API (or per application) and reuse it rather
        /// than creating one per request; dispose it when you are done (<c>using var curl = new LibCurl();</c>).
        /// In ASP.NET Core register it as a singleton.</para>
        /// <para>Example: <c>using var curl = new LibCurl().WithUserAgent("MyApp/1.0").WithFollowRedirects();</c>
        /// then <c>var page = await curl.GetAsync("http://example.com");</c>.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-in-aspnet-core/README.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// using (var curl = new LibCurl())
        /// {
        ///     var result = await curl.GetAsync("https://api.example.com/data");
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="Dispose"/>
        /// <seealso cref="Configure"/>
        public LibCurl()
        {
            // The engine's own transport behaves like curl (no automatic redirects, decompression or
            // cookies, no implicit timeout) and honours -x/-k per request; a plain `new HttpClient()`
            // would follow redirects by itself, time out after 100 s and ignore WithProxy/WithInsecureSsl.
            _engine = new CurlEngine();
            _defaultHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _defaultOptions = new CurlOptions();
        }

        #region HTTP Methods

        /// <summary>
        /// Sends a GET request (<c>curl URL</c>) with this client's defaults and returns the response; failures
        /// come back in <see cref="CurlResult.Exception"/> instead of being thrown.
        /// </summary>
        /// <remarks>
        /// <para>The URL is used as given: include the scheme and encode query values yourself
        /// (<c>$"https://api.example.com/search?q={Uri.EscapeDataString(term)}"</c>). Use <paramref name="configure"/>
        /// to change any <see cref="CurlOptions"/> for this request only, for example
        /// <c>opts =&gt; opts.Headers["If-None-Match"] = etag</c> or <c>opts =&gt; opts.OutputFile = "data.json"</c>; it runs
        /// after the defaults are applied, so it wins.</para>
        /// <para>Example: <c>var r = await curl.GetAsync("https://api.github.com/repos/dotnet/runtime");</c> then
        /// <c>if (r.Exception == null &amp;&amp; r.IsSuccess) Console.WriteLine(r.Body);</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">Absolute URL to fetch, such as <c>https://api.example.com/users?page=2</c>.</param>
        /// <param name="configure">Optional callback that adjusts this request's <see cref="CurlOptions"/> (headers,
        /// output file, timeouts...); null for none.</param>
        /// <param name="cancellationToken">Token that cancels the request; <c>default</c> for none.</param>
        /// <returns>The <see cref="CurlResult"/>: status code, headers and body for any HTTP answer (including 4xx/5xx);
        /// for a transfer failure <c>StatusCode</c> 0, an empty body and the <see cref="CurlException"/> in
        /// <see cref="CurlResult.Exception"/>.</returns>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</exception>
        /// <example>
        /// <code language="csharp">
        /// var result = await curl.GetAsync("https://api.example.com/users",
        ///     opts => opts.FollowLocation = true);
        /// </code>
        /// </example>
        /// <seealso cref="CurlRequestBuilder.Get"/>
        /// <seealso cref="Curl.GetAsync(string)"/>
        /// <seealso cref="HeadAsync"/>
        public async Task<CurlResult> GetAsync(string url, Action<CurlOptions> configure = null, CancellationToken cancellationToken = default)
        {
            var options = new CurlOptions { Url = url, Method = "GET" };
            MergeDefaults(options);
            configure?.Invoke(options);
            return await RunAsync(options, cancellationToken);
        }

        /// <summary>
        /// Sends a POST request with a body: a string is sent as-is (form-encoded content type), any other object is
        /// serialized to JSON and sent with <c>Content-Type: application/json</c>.
        /// </summary>
        /// <remarks>
        /// <para>Body rules: <c>null</c> sends no body; a <see cref="string"/> such as <c>"a=1&amp;b=2"</c> is sent
        /// unchanged with <c>Content-Type: application/x-www-form-urlencoded</c> (curl's <c>-d</c>), so set
        /// <c>opts =&gt; opts.Headers["Content-Type"] = "application/json"</c> when the string is JSON; any other
        /// object (anonymous object, class, dictionary, list) is serialized with System.Text.Json (Newtonsoft.Json on
        /// .NET Standard 2.0) and sent as <c>application/json</c>. A Content-Type set in <paramref name="configure"/>
        /// wins over both.</para>
        /// <para>Example: <c>var r = await curl.PostAsync("https://api.example.com/users", new { name = "Ada", email = "ada@example.com" });</c>
        /// is <c>curl -H 'Content-Type: application/json' -d '{"name":"Ada","email":"ada@example.com"}' URL</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">Absolute URL to post to.</param>
        /// <param name="data">The body: null, a string sent as-is, or an object serialized to JSON.</param>
        /// <param name="configure">Optional callback that adjusts this request's <see cref="CurlOptions"/>; null for none.</param>
        /// <param name="cancellationToken">Token that cancels the request; <c>default</c> for none.</param>
        /// <returns>The <see cref="CurlResult"/>; for a transfer failure <c>StatusCode</c> 0 and the error in
        /// <see cref="CurlResult.Exception"/>.</returns>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</exception>
        /// <example>
        /// <code language="csharp">
        /// var result = await curl.PostAsync("https://api.example.com/data",
        ///     "key1=value1&amp;key2=value2");
        /// </code>
        /// </example>
        /// <seealso cref="CurlRequestBuilder.Post"/>
        /// <seealso cref="Curl.PostJsonAsync(string, object)"/>
        /// <seealso cref="PutAsync"/>
        public async Task<CurlResult> PostAsync(string url, object data = null, Action<CurlOptions> configure = null, CancellationToken cancellationToken = default)
        {
            var options = CreateWithBody(url, "POST", data);
            MergeDefaults(options);
            configure?.Invoke(options);
            return await RunAsync(options, cancellationToken);
        }

        /// <summary>
        /// Sends a PUT request (<c>curl -X PUT</c>) to create or replace the resource at <paramref name="url"/>;
        /// the body follows the same rules as <see cref="PostAsync"/> (string as-is, object as JSON).
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>await curl.PutAsync("https://api.example.com/users/42", new { name = "Jane" });</c> sends
        /// <c>{"name":"Jane"}</c> with <c>Content-Type: application/json</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">Absolute URL of the resource.</param>
        /// <param name="data">The body: null, a string sent as-is, or an object serialized to JSON.</param>
        /// <param name="configure">Optional callback that adjusts this request's <see cref="CurlOptions"/>; null for none.</param>
        /// <param name="cancellationToken">Token that cancels the request; <c>default</c> for none.</param>
        /// <returns>The <see cref="CurlResult"/>; for a transfer failure <c>StatusCode</c> 0 and the error in
        /// <see cref="CurlResult.Exception"/>.</returns>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</exception>
        /// <seealso cref="CurlRequestBuilder.Put"/>
        /// <seealso cref="PatchAsync"/>
        public async Task<CurlResult> PutAsync(string url, object data = null, Action<CurlOptions> configure = null, CancellationToken cancellationToken = default)
        {
            var options = CreateWithBody(url, "PUT", data);
            MergeDefaults(options);
            configure?.Invoke(options);
            return await RunAsync(options, cancellationToken);
        }

        /// <summary>
        /// Sends a PATCH request (<c>curl -X PATCH</c>) with the fields to change; the body follows the same rules as
        /// <see cref="PostAsync"/> (string as-is, object as JSON).
        /// </summary>
        /// <remarks>
        /// <para>For JSON Merge Patch set the media type: <c>opts =&gt; opts.Headers["Content-Type"] = "application/merge-patch+json"</c>.</para>
        /// <para>Example: <c>await curl.PatchAsync("https://api.example.com/users/42", new { email = "new@example.com" });</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">Absolute URL of the resource.</param>
        /// <param name="data">The body: null, a string sent as-is, or an object serialized to JSON.</param>
        /// <param name="configure">Optional callback that adjusts this request's <see cref="CurlOptions"/>; null for none.</param>
        /// <param name="cancellationToken">Token that cancels the request; <c>default</c> for none.</param>
        /// <returns>The <see cref="CurlResult"/>; for a transfer failure <c>StatusCode</c> 0 and the error in
        /// <see cref="CurlResult.Exception"/>.</returns>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</exception>
        /// <seealso cref="CurlRequestBuilder.Patch"/>
        /// <seealso cref="PutAsync"/>
        public async Task<CurlResult> PatchAsync(string url, object data = null, Action<CurlOptions> configure = null, CancellationToken cancellationToken = default)
        {
            var options = CreateWithBody(url, "PATCH", data);
            MergeDefaults(options);
            configure?.Invoke(options);
            return await RunAsync(options, cancellationToken);
        }

        /// <summary>
        /// Sends a DELETE request (<c>curl -X DELETE URL</c>) to remove the resource at <paramref name="url"/>;
        /// most APIs answer 200, 202 or 204.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>var r = await curl.DeleteAsync("https://api.example.com/users/42");</c> then
        /// <c>if (r.StatusCode == 204) Console.WriteLine("deleted");</c>. To send a body with a DELETE, set
        /// <c>opts =&gt; opts.Data = "..."</c> in <paramref name="configure"/>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">Absolute URL of the resource to delete.</param>
        /// <param name="configure">Optional callback that adjusts this request's <see cref="CurlOptions"/>; null for none.</param>
        /// <param name="cancellationToken">Token that cancels the request; <c>default</c> for none.</param>
        /// <returns>The <see cref="CurlResult"/>; for a transfer failure <c>StatusCode</c> 0 and the error in
        /// <see cref="CurlResult.Exception"/>.</returns>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</exception>
        /// <seealso cref="CurlRequestBuilder.Delete"/>
        public async Task<CurlResult> DeleteAsync(string url, Action<CurlOptions> configure = null, CancellationToken cancellationToken = default)
        {
            var options = new CurlOptions { Url = url, Method = "DELETE" };
            MergeDefaults(options);
            configure?.Invoke(options);
            return await RunAsync(options, cancellationToken);
        }

        /// <summary>
        /// Sends a HEAD request: only the status and headers come back (in <see cref="CurlResult.Headers"/>), so you
        /// can check existence, size or <c>Last-Modified</c> without downloading the body.
        /// </summary>
        /// <remarks>
        /// <para>Unlike curl's <c>-I</c>, the headers are not copied into <see cref="CurlResult.Body"/>; read them with
        /// <c>result.GetHeader("Content-Length")</c>.</para>
        /// <para>Example: <c>var head = await curl.HeadAsync("https://example.com/big.iso");</c> then
        /// <c>Console.WriteLine(head.GetHeader("Content-Length"));</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-head-request-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">Absolute URL to probe.</param>
        /// <param name="configure">Optional callback that adjusts this request's <see cref="CurlOptions"/>; null for none.</param>
        /// <param name="cancellationToken">Token that cancels the request; <c>default</c> for none.</param>
        /// <returns>The <see cref="CurlResult"/> with status code and headers and an empty body; for a transfer
        /// failure <c>StatusCode</c> 0 and the error in <see cref="CurlResult.Exception"/>.</returns>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</exception>
        /// <seealso cref="CurlRequestBuilder.Head"/>
        /// <seealso cref="GetAsync"/>
        public async Task<CurlResult> HeadAsync(string url, Action<CurlOptions> configure = null, CancellationToken cancellationToken = default)
        {
            var options = new CurlOptions { Url = url, Method = "HEAD", HeadOnly = true };
            MergeDefaults(options);
            configure?.Invoke(options);
            return await RunAsync(options, cancellationToken);
        }

        /// <summary>
        /// Runs a request you built yourself as <see cref="CurlOptions"/> (any method, body, form, output file or
        /// curl option), with this client's defaults filled in; libcurl's <c>curl_easy_perform</c>.
        /// </summary>
        /// <remarks>
        /// <para>The defaults are merged into the object you pass (it is modified): default headers are added when the
        /// request has no header of that name, and the default user agent, credentials, timeouts, redirect setting,
        /// insecure flag, proxy, output file and verbose flag are used where the request left them unset.</para>
        /// <para>Example: <c>var opts = new CurlOptions { Url = "https://api.example.com/import", Method = "POST", Files = { ["file"] = "data.csv" } };</c>
        /// then <c>var r = await curl.PerformAsync(opts);</c> uploads <c>data.csv</c> as multipart form data.</para>
        /// <para>Mapping of CurlOptions to curl flags: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/reference/libcurl-mapping.md</para>
        /// </remarks>
        /// <param name="options">The request to run; must not be null and must have <see cref="CurlOptions.Url"/> set.</param>
        /// <param name="cancellationToken">Token that cancels the request; <c>default</c> for none.</param>
        /// <returns>The <see cref="CurlResult"/>; for a transfer failure (including a missing URL, exit code 3)
        /// <c>StatusCode</c> 0 and the error in <see cref="CurlResult.Exception"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is null. Pass a <see cref="CurlOptions"/>
        /// with at least <c>Url</c> set; see https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/reference/libcurl-mapping.md</exception>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</exception>
        /// <seealso cref="Curl.ExecuteAsync(CurlOptions, CancellationToken)"/>
        /// <seealso cref="CurlRequestBuilder.GetOptions"/>
        public async Task<CurlResult> PerformAsync(CurlOptions options, CancellationToken cancellationToken = default)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), CurlException.Compose(
                    "LibCurl.PerformAsync(options) was called with null options.",
                    "Pass a CurlOptions with at least Url set, for example new CurlOptions { Url = \"https://api.example.com/items\" }, or use GetAsync/PostAsync.",
                    DocLinks.LibCurlMapping));
            }
            MergeDefaults(options);
            return await RunAsync(options, cancellationToken);
        }

        #endregion

        #region Configuration Methods (Fluent API)

        /// <summary>
        /// Adds a header that every request from this client sends (curl's <c>-H 'Name: value'</c>), unless the
        /// request sets a header with the same name itself.
        /// </summary>
        /// <remarks>
        /// <para>Names are matched case-insensitively; calling it again with the same name replaces the default value.
        /// A request overrides a default with <c>opts =&gt; opts.Headers["Accept"] = "text/csv"</c> in its
        /// <c>configure</c> callback.</para>
        /// <para>Example: <c>curl.WithHeader("Accept", "application/json").WithHeader("X-Api-Key", key);</c> then every
        /// <see cref="GetAsync"/> sends both headers.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md</para>
        /// </remarks>
        /// <param name="key">Header field name such as <c>"Accept"</c> (no colon); must not be null.</param>
        /// <param name="value">Header value such as <c>"application/json"</c>, sent exactly as given.</param>
        /// <returns>This instance, for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null. Pass the header name; see
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md</exception>
        /// <example>
        /// <code language="csharp">
        /// curl.WithHeader("Accept", "application/json")
        ///     .WithHeader("X-API-Key", "your-key");
        /// var result = await curl.GetAsync("https://api.example.com/data");
        /// </code>
        /// </example>
        /// <seealso cref="CurlRequestBuilder.WithHeader"/>
        /// <seealso cref="WithBearerToken"/>
        public LibCurl WithHeader(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), CurlException.Compose(
                    "LibCurl.WithHeader(key, value) was called with a null header name.",
                    "Pass a header name such as \"Accept\" or \"X-Api-Key\".",
                    DocLinks.HowToHeaders));
            }
            _defaultHeaders[key] = value;
            return this;
        }

        /// <summary>
        /// Sends <c>Authorization: Bearer TOKEN</c> (OAuth 2 / JWT) with every request from this client, like
        /// <c>curl -H 'Authorization: Bearer TOKEN'</c> or <c>--oauth2-bearer TOKEN</c>.
        /// </summary>
        /// <remarks>
        /// <para>Pass the token alone, without the word <c>Bearer</c>. Calling it again replaces the token, so a
        /// refreshed token can be set on the same instance (do it between requests, not while they run).</para>
        /// <para>Example: <c>using var github = new LibCurl().WithBearerToken(Environment.GetEnvironmentVariable("GITHUB_TOKEN")).WithUserAgent("MyApp/1.0");</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-bearer-token-csharp/README.md</para>
        /// </remarks>
        /// <param name="token">The access token, such as <c>"eyJhbGciOi..."</c> or <c>"ghp_..."</c>.</param>
        /// <returns>This instance, for chaining.</returns>
        /// <seealso cref="CurlRequestBuilder.WithBearerToken"/>
        /// <seealso cref="WithBasicAuth"/>
        public LibCurl WithBearerToken(string token)
        {
            return WithHeader("Authorization", $"Bearer {token}");
        }

        /// <summary>
        /// Sends HTTP Basic credentials (curl's <c>-u user:password</c>) with every request that does not set
        /// its own <see cref="CurlOptions.Credentials"/>.
        /// </summary>
        /// <remarks>
        /// <para>Basic auth is only base64-encoded, so use it over <c>https://</c>. As in curl, the credentials are not
        /// sent to a different host after a redirect. A wrong password gives a result with <c>StatusCode == 401</c>.</para>
        /// <para>Example: <c>using var jenkins = new LibCurl().WithBasicAuth("ci-bot", apiToken);</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-basic-auth-csharp/README.md</para>
        /// </remarks>
        /// <param name="username">The user name (may contain <c>@</c>).</param>
        /// <param name="password">The password or API token; may be empty.</param>
        /// <returns>This instance, for chaining.</returns>
        /// <seealso cref="CurlRequestBuilder.WithBasicAuth"/>
        /// <seealso cref="CurlOptions.Credentials"/>
        public LibCurl WithBasicAuth(string username, string password)
        {
            _defaultOptions.Credentials = new NetworkCredential(username, password);
            return this;
        }

        /// <summary>
        /// Limits every request's total time (connect, send, wait, download) to <paramref name="timeout"/>, like
        /// curl's <c>-m</c> / <c>--max-time</c>; without it there is no limit.
        /// </summary>
        /// <remarks>
        /// <para>Converted to seconds with decimals (<c>TimeSpan.FromMilliseconds(2500)</c> is 2.5 s). When it expires the
        /// result carries a <see cref="CurlOperationTimeoutException"/> (exit code 28) in <see cref="CurlResult.Exception"/>.
        /// A request can override it with <c>opts =&gt; opts.MaxTime = 300</c>.</para>
        /// <para>Example: <c>new LibCurl().WithTimeout(TimeSpan.FromSeconds(30)).WithConnectTimeout(TimeSpan.FromSeconds(5))</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</para>
        /// </remarks>
        /// <param name="timeout">Maximum total time per request; <see cref="TimeSpan.Zero"/> means unlimited.</param>
        /// <returns>This instance, for chaining.</returns>
        /// <seealso cref="WithConnectTimeout"/>
        /// <seealso cref="CurlRequestBuilder.WithTimeout"/>
        /// <seealso cref="CurlOptions.MaxTime"/>
        public LibCurl WithTimeout(TimeSpan timeout)
        {
            _defaultOptions.MaxTime = timeout.TotalSeconds;
            return this;
        }

        /// <summary>
        /// Limits only the connection phase (DNS, TCP connect, TLS handshake) of every request, like curl's
        /// <c>--connect-timeout</c>, so a dead server fails fast while slow downloads may continue.
        /// </summary>
        /// <remarks>
        /// <para>Converted to seconds with decimals. When it expires the result carries a
        /// <see cref="CurlOperationTimeoutException"/> (exit code 28) in <see cref="CurlResult.Exception"/>.</para>
        /// <para>Example: <c>new LibCurl().WithConnectTimeout(TimeSpan.FromSeconds(3))</c> for health checks.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</para>
        /// </remarks>
        /// <param name="timeout">Maximum time to establish each connection; <see cref="TimeSpan.Zero"/> means no limit.</param>
        /// <returns>This instance, for chaining.</returns>
        /// <seealso cref="WithTimeout"/>
        /// <seealso cref="CurlRequestBuilder.WithConnectTimeout"/>
        /// <seealso cref="CurlOptions.ConnectTimeout"/>
        public LibCurl WithConnectTimeout(TimeSpan timeout)
        {
            _defaultOptions.ConnectTimeout = timeout.TotalSeconds;
            return this;
        }

        /// <summary>
        /// Follows <c>Location:</c> redirects (301, 302, 303, 307, 308) for every request, like curl's <c>-L</c>,
        /// up to <paramref name="maxRedirects"/> hops; without it you get the 3xx response itself.
        /// </summary>
        /// <remarks>
        /// <para>More redirects than the limit end the request with <see cref="CurlTooManyRedirectsException"/>
        /// (exit code 47) in <see cref="CurlResult.Exception"/>; <c>-1</c> means unlimited (curl's
        /// <c>--max-redirs -1</c>). A 301/302/303 after a POST is followed with GET, and credentials are not sent to
        /// another host, as in curl.</para>
        /// <para>Example: <c>using var curl = new LibCurl().WithFollowRedirects(5);</c> then
        /// <c>(await curl.GetAsync("http://github.com")).StatusCode</c> is 200.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-follow-redirects-csharp/README.md</para>
        /// </remarks>
        /// <param name="maxRedirects">Maximum number of redirects to follow (default 50, curl's default); -1 for unlimited.</param>
        /// <returns>This instance, for chaining.</returns>
        /// <seealso cref="CurlRequestBuilder.FollowRedirects"/>
        /// <seealso cref="CurlOptions.FollowLocation"/>
        /// <seealso cref="CurlOptions.MaxRedirects"/>
        public LibCurl WithFollowRedirects(int maxRedirects = 50)
        {
            _defaultOptions.FollowLocation = true;
            _defaultOptions.MaxRedirects = maxRedirects;
            return this;
        }

        /// <summary>
        /// Skips TLS certificate verification for every request (curl's <c>-k</c> / <c>--insecure</c>), accepting
        /// self-signed and expired certificates. Development and testing only.
        /// </summary>
        /// <remarks>
        /// <para>With verification off anyone on the network path can impersonate the server; never ship it. Without it,
        /// an untrusted certificate gives a <see cref="CurlPeerFailedVerificationException"/> (exit code 60) in
        /// <see cref="CurlResult.Exception"/>.</para>
        /// <para>Example: <c>using var local = new LibCurl().WithInsecureSsl();</c> then
        /// <c>await local.GetAsync("https://localhost:5001/health");</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md</para>
        /// </remarks>
        /// <returns>This instance, for chaining.</returns>
        /// <seealso cref="CurlRequestBuilder.Insecure"/>
        /// <seealso cref="CurlOptions.Insecure"/>
        public LibCurl WithInsecureSsl()
        {
            _defaultOptions.Insecure = true;
            return this;
        }

        /// <summary>
        /// Sends every request through a proxy (curl's <c>-x URL</c>, with <c>--proxy-user user:password</c> when
        /// credentials are given), for corporate proxies, Fiddler or SOCKS tunnels.
        /// </summary>
        /// <remarks>
        /// <para>Accepted proxy forms: <c>http://proxy.corp:8080</c>, <c>https://proxy.corp:443</c>,
        /// <c>socks5://127.0.0.1:1080</c>, <c>socks5h://...</c>, <c>socks4://...</c>, or <c>proxy.corp:8080</c>
        /// (http). When <paramref name="username"/> is null or empty no proxy credentials are sent (credentials embedded
        /// in the URL, <c>http://user:pass@proxy:8080</c>, still work). A request that sets its own
        /// <see cref="CurlOptions.Proxy"/> keeps it.</para>
        /// <para>An unreachable proxy gives <see cref="CurlCouldntConnectException"/> (exit code 7) and an unknown proxy
        /// host <see cref="CurlCouldntResolveProxyException"/> (exit code 5) in <see cref="CurlResult.Exception"/>.</para>
        /// <para>Example: <c>using var curl = new LibCurl().WithProxy("http://proxy.corp:8080", "ada", password);</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md</para>
        /// </remarks>
        /// <param name="proxyUrl">Proxy URL such as <c>"http://proxy.example.com:8080"</c> or <c>"socks5://127.0.0.1:1080"</c>.</param>
        /// <param name="username">Proxy user name, or null for none.</param>
        /// <param name="password">Proxy password, or null (treated as empty).</param>
        /// <returns>This instance, for chaining.</returns>
        /// <seealso cref="CurlRequestBuilder.WithProxy(string, string, string)"/>
        /// <seealso cref="CurlOptions.Proxy"/>
        /// <seealso cref="CurlOptions.ProxyCredentials"/>
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
        /// Sets the <c>User-Agent</c> header for every request (curl's <c>-A</c>); without it CurlDotNet sends
        /// <c>curl/8.x</c> like the curl tool.
        /// </summary>
        /// <remarks>
        /// <para>Many APIs (GitHub among them) reject requests without a descriptive agent; use something like
        /// <c>"MyCompany-Sync/2.1"</c>. A request that sets <see cref="CurlOptions.UserAgent"/> keeps its own.</para>
        /// <para>Example: <c>using var github = new LibCurl().WithUserAgent("MyApp/1.0");</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md</para>
        /// </remarks>
        /// <param name="userAgent">The full User-Agent value, such as <c>"MyApp/1.0"</c>.</param>
        /// <returns>This instance, for chaining.</returns>
        /// <seealso cref="CurlRequestBuilder.WithUserAgent"/>
        /// <seealso cref="CurlOptions.UserAgent"/>
        public LibCurl WithUserAgent(string userAgent)
        {
            _defaultOptions.UserAgent = userAgent;
            return this;
        }

        /// <summary>
        /// Writes the body of every request to <paramref name="filePath"/> (curl's <c>-o</c>), overwriting it each
        /// time, unless a request sets its own <see cref="CurlOptions.OutputFile"/>.
        /// </summary>
        /// <remarks>
        /// <para>Because every request overwrites the same file, this is mostly useful for a client dedicated to one
        /// download; per request prefer <c>opts =&gt; opts.OutputFile = "report.pdf"</c> in the <c>configure</c>
        /// callback. A folder that does not exist gives <see cref="CurlWriteErrorException"/> (exit code 23) in
        /// <see cref="CurlResult.Exception"/>.</para>
        /// <para>Example: <c>await curl.GetAsync("https://example.com/report.pdf", opts =&gt; opts.OutputFile = @"C:\downloads\report.pdf");</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <param name="filePath">Destination file path, absolute or relative to the current directory.</param>
        /// <returns>This instance, for chaining.</returns>
        /// <seealso cref="CurlRequestBuilder.WithOutput"/>
        /// <seealso cref="CurlOptions.OutputFile"/>
        public LibCurl WithOutputFile(string filePath)
        {
            _defaultOptions.OutputFile = filePath;
            return this;
        }

        /// <summary>
        /// Records curl's <c>-v</c> verbose log (request and response headers, connection notes) at the start of
        /// every result's <see cref="CurlResult.Body"/>, for debugging.
        /// </summary>
        /// <remarks>
        /// <para>Turn it off before parsing bodies: the log precedes the response text. For a log written to a file use
        /// <c>opts =&gt; opts.TraceAsciiFile = "trace.txt"</c> (curl's <c>--trace-ascii</c>).</para>
        /// <para>Example: <c>using var debug = new LibCurl().WithVerbose();</c> then <c>Console.WriteLine((await debug.GetAsync(url)).Body);</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md</para>
        /// </remarks>
        /// <returns>This instance, for chaining.</returns>
        /// <seealso cref="CurlRequestBuilder.Verbose"/>
        /// <seealso cref="CurlOptions.Verbose"/>
        public LibCurl WithVerbose()
        {
            _defaultOptions.Verbose = true;
            return this;
        }

        /// <summary>
        /// Edits this client's default <see cref="CurlOptions"/> with a callback, as an alternative to the
        /// <c>With...</c> methods; only the defaults listed in the remarks are applied to requests.
        /// </summary>
        /// <remarks>
        /// <para>The defaults that requests inherit are: <see cref="CurlOptions.UserAgent"/>,
        /// <see cref="CurlOptions.Credentials"/>, <see cref="CurlOptions.MaxTime"/>, <see cref="CurlOptions.ConnectTimeout"/>,
        /// <see cref="CurlOptions.FollowLocation"/> with <see cref="CurlOptions.MaxRedirects"/>, <see cref="CurlOptions.Insecure"/>,
        /// <see cref="CurlOptions.Proxy"/> with <see cref="CurlOptions.ProxyCredentials"/>, <see cref="CurlOptions.OutputFile"/>
        /// and <see cref="CurlOptions.Verbose"/>. Other properties set here (for example <c>Retry</c> or
        /// <c>Headers</c>) are not copied to requests: set headers with <see cref="WithHeader"/> and other options per
        /// request through the <c>configure</c> argument of <see cref="GetAsync"/> and friends.</para>
        /// <para>Example: <c>curl.Configure(opts =&gt; { opts.FollowLocation = true; opts.MaxTime = 60; });</c></para>
        /// </remarks>
        /// <param name="configure">Callback that edits the default options; null does nothing.</param>
        /// <returns>This instance, for chaining.</returns>
        /// <example>
        /// <code language="csharp">
        /// curl.Configure(opts => {
        ///     opts.FollowLocation = true;
        ///     opts.MaxTime = 60;
        ///     opts.Insecure = false;
        /// });
        /// </code>
        /// </example>
        /// <seealso cref="PerformAsync"/>
        /// <seealso cref="CurlOptions"/>
        public LibCurl Configure(Action<CurlOptions> configure)
        {
            configure?.Invoke(_defaultOptions);
            return this;
        }

        #endregion

        #region Helper Methods

        /// <summary>A request with a body: strings as-is, other objects as JSON with a JSON content type.</summary>
        private static CurlOptions CreateWithBody(string url, string method, object data)
        {
            var options = new CurlOptions
            {
                Url = url,
                Method = method,
                Data = SerializeData(data)
            };
            if (data != null && !(data is string))
            {
                options.Headers["Content-Type"] = "application/json"; // the body is JSON, say so (curl --json)
            }
            return options;
        }

        /// <summary>
        /// Merge default options into request options.
        /// </summary>
        private void MergeDefaults(CurlOptions options)
        {
            // Merge default headers
            foreach (var header in _defaultHeaders)
            {
#if NETSTANDARD2_0 || NET472 || NET48
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

            if (!options.MaxTime.HasValue && _defaultOptions.MaxTime.HasValue)
                options.MaxTime = _defaultOptions.MaxTime;

            if (!options.ConnectTimeout.HasValue && _defaultOptions.ConnectTimeout.HasValue)
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
        private static string SerializeData(object data)
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
        /// Runs the request and, like libcurl's <c>CURLcode</c> model, reports transfer failures
        /// through <see cref="CurlResult.Exception"/> instead of throwing. Caller cancellation is
        /// the one exception: it surfaces as <see cref="OperationCanceledException"/> so it composes
        /// with normal .NET cancellation handling.
        /// </summary>
        private async Task<CurlResult> RunAsync(CurlOptions options, CancellationToken cancellationToken)
        {
            try
            {
                return await _engine.ExecuteAsync(options, cancellationToken);
            }
            catch (CurlAbortedByCallbackException ex) when (cancellationToken.IsCancellationRequested)
            {
                throw new OperationCanceledException(CurlException.Compose(
                    $"The LibCurl request to {ErrorText.SafeUrl(options.Url)} was cancelled by the CancellationToken passed to it.",
                    "Nothing to fix if you cancelled on purpose; otherwise pass a token that is not cancelled, and use WithTimeout for time limits instead of a cancelled token.",
                    DocLinks.TutorialCancellation), ex, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (CurlException ex)
            {
                return new CurlResult
                {
                    StatusCode = 0,
                    Body = string.Empty,
                    Command = options.OriginalCommand,
                    Exception = ex,
                };
            }
        }

        /// <summary>
        /// Releases this client's connections and handlers; call it (or use <c>using</c>) when the client is no
        /// longer needed, after its requests have finished.
        /// </summary>
        /// <remarks>
        /// <para>Requests started after <see cref="Dispose"/> fail with <see cref="ObjectDisposedException"/>. Calling it
        /// more than once is harmless.</para>
        /// <para>Example: <c>using (var curl = new LibCurl()) { var r = await curl.GetAsync(url); }</c> disposes the client
        /// at the end of the block.</para>
        /// </remarks>
        /// <seealso cref="LibCurl()"/>
        public void Dispose()
        {
            _engine?.Dispose();
        }
    }
}
