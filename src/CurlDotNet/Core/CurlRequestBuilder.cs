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
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Builds one HTTP request with chained, IntelliSense-friendly methods (one method per curl flag)
    /// and runs it with <see cref="ExecuteAsync(CancellationToken)"/>; <see cref="ToCurlCommand"/> shows
    /// the equivalent curl command.
    /// </summary>
    /// <remarks>
    /// <para>Start with a factory for the HTTP method (<see cref="Get"/>, <see cref="Post"/>, <see cref="Put"/>,
    /// <see cref="Patch"/>, <see cref="Delete"/>, <see cref="Head"/> or <see cref="Request"/> for any other verb),
    /// chain the options you need, then call <see cref="ExecuteAsync(CancellationToken)"/> (or
    /// <see cref="Execute"/> from synchronous code). Every <c>With...</c> method returns the same builder, so
    /// the order of calls does not matter, and calling a method twice keeps the last value (headers are
    /// replaced by name, case-insensitively).</para>
    /// <para>Each method maps to one curl flag: <see cref="WithHeader"/> is <c>-H</c>, <see cref="WithData"/>
    /// is <c>-d</c>, <see cref="WithBasicAuth"/> is <c>-u</c>, <see cref="WithTimeout"/> is <c>-m</c>, and so on;
    /// the remarks of every method name its flag. The request is executed by the same engine as
    /// <see cref="Curl.ExecuteAsync(string)"/>, so it behaves exactly like the curl command printed by
    /// <see cref="ToCurlCommand"/>.</para>
    /// <para>Example: <c>var result = await CurlRequestBuilder.Get("https://api.github.com/users/octocat").WithHeader("Accept", "application/json").WithTimeout(TimeSpan.FromSeconds(30)).ExecuteAsync();</c>
    /// is the same request as <c>curl -H 'Accept: application/json' --max-time 30 https://api.github.com/users/octocat</c>;
    /// then read <c>result.StatusCode</c> and <c>result.Body</c>.</para>
    /// <para>Which API to use: paste a curl command into <see cref="Curl.ExecuteAsync(string)"/> when you have
    /// one from documentation; use this builder when the request is assembled in code (values from
    /// variables, conditional headers); use <see cref="CurlDotNet.Lib.LibCurl"/> when many requests share
    /// the same defaults (base headers, token, timeout).</para>
    /// <para>Thread safety: a builder is a mutable object for one request; do not share one instance
    /// between threads. Build a new one per request (they are cheap).</para>
    /// <para>Tutorials: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md
    /// and https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md</para>
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// var result = await CurlRequestBuilder
    ///     .Get("https://api.example.com/users")
    ///     .WithHeader("Accept", "application/json")
    ///     .WithBearerToken(token)
    ///     .WithTimeout(TimeSpan.FromSeconds(30))
    ///     .ExecuteAsync();
    ///
    /// // Same as: curl -H 'Accept: application/json' -H 'Authorization: Bearer ...' --max-time 30 https://api.example.com/users
    /// </code>
    /// </example>
    /// <seealso cref="Curl.ExecuteAsync(string)"/>
    /// <seealso cref="CurlDotNet.Lib.LibCurl"/>
    /// <seealso cref="CurlOptions"/>
    /// <seealso cref="CurlResult"/>
    public class CurlRequestBuilder
    {
        private readonly CurlOptions _options = new CurlOptions();

        private CurlRequestBuilder(string url, string method = "GET")
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url), CurlException.Compose(
                    $"CurlRequestBuilder.{FactoryName(method)}(url) was called with a null URL.",
                    "Pass an absolute URL such as \"https://api.example.com/items\" (check the variable or configuration value you read it from).",
                    DocLinks.Url));
            }
            _options.Url = url;
            _options.Method = method;
        }

        private static string FactoryName(string method)
        {
            switch (method)
            {
                case "GET": return "Get";
                case "POST": return "Post";
                case "PUT": return "Put";
                case "DELETE": return "Delete";
                case "PATCH": return "Patch";
                case "HEAD": return "Head";
                default: return "Request";
            }
        }

        #region Factory Methods

        /// <summary>
        /// Starts a GET request for <paramref name="url"/>, the default curl request (no method flag);
        /// chain options and call <see cref="ExecuteAsync(CancellationToken)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Equivalent curl: <c>curl URL</c>. Adding a body later with <see cref="WithData"/>,
        /// <see cref="WithJson"/>, <see cref="WithFormData"/>, <see cref="WithMultipartForm"/>,
        /// <see cref="WithBinaryData"/> or a file upload turns the request into a POST, exactly as curl does when
        /// <c>-d</c> or <c>-F</c> is added.</para>
        /// <para>The URL is used as given (curl syntax): include the scheme (<c>https://</c>, <c>http://</c>,
        /// <c>ftp://</c>, <c>file://</c>), percent-encode spaces as <c>%20</c>, and add query strings yourself,
        /// for example <c>$"https://api.example.com/search?q={Uri.EscapeDataString(term)}"</c>. A URL without a
        /// scheme is treated as <c>http://</c>, like curl.</para>
        /// <para>Example: <c>var user = await CurlRequestBuilder.Get("https://api.github.com/users/octocat").WithHeader("Accept", "application/json").ExecuteAsync();</c>
        /// then <c>user.ParseJson&lt;GitHubUser&gt;()</c>.</para>
        /// </remarks>
        /// <param name="url">Absolute URL to fetch, such as <c>https://api.example.com/users?page=2</c>. Must not be null;
        /// an invalid URL is reported when the request runs (<see cref="CurlMalformedUrlException"/>, exit code 3).</param>
        /// <returns>A new builder whose method is GET; nothing is sent until you execute it.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> is null. Pass the URL string; see
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</exception>
        /// <example>
        /// <code language="csharp">
        /// var result = await CurlRequestBuilder
        ///     .Get("https://api.example.com/protected")
        ///     .WithBearerToken("your-token")
        ///     .FollowRedirects()
        ///     .ExecuteAsync();
        /// </code>
        /// </example>
        /// <seealso cref="Curl.GetAsync(string)"/>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.GetAsync"/>
        /// <seealso cref="Head"/>
        public static CurlRequestBuilder Get(string url)
        {
            return new CurlRequestBuilder(url, "GET");
        }

        /// <summary>
        /// Starts a POST request (<c>curl -X POST URL</c>); add the body with <see cref="WithJson"/>,
        /// <see cref="WithData"/>, <see cref="WithFormData"/> or <see cref="WithMultipartForm"/>.
        /// </summary>
        /// <remarks>
        /// <para>A POST with no body sends <c>Content-Length: 0</c>. The body methods set the matching
        /// <c>Content-Type</c>: <see cref="WithJson"/> uses <c>application/json</c>, <see cref="WithData"/> and
        /// <see cref="WithFormData"/> use <c>application/x-www-form-urlencoded</c> (like <c>curl -d</c>),
        /// <see cref="WithMultipartForm"/> and the file methods use <c>multipart/form-data</c> (like <c>curl -F</c>).</para>
        /// <para>Example: <c>var created = await CurlRequestBuilder.Post("https://api.example.com/users").WithJson(new { name = "Ada", email = "ada@example.com" }).ExecuteAsync();</c>
        /// then check <c>created.StatusCode == 201</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">Absolute URL to post to, such as <c>https://api.example.com/users</c>. Must not be null.</param>
        /// <returns>A new builder whose method is POST.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> is null. Pass the URL string; see
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</exception>
        /// <example>
        /// <code language="csharp">
        /// var result = await CurlRequestBuilder
        ///     .Post("https://api.example.com/login")
        ///     .WithFormData(new Dictionary&lt;string, string&gt; { ["username"] = "user123", ["password"] = "pass456" })
        ///     .ExecuteAsync();
        /// </code>
        /// </example>
        /// <seealso cref="Curl.PostAsync(string, string)"/>
        /// <seealso cref="Curl.PostJsonAsync(string, object)"/>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.PostAsync"/>
        public static CurlRequestBuilder Post(string url)
        {
            return new CurlRequestBuilder(url, "POST");
        }

        /// <summary>
        /// Starts a PUT request (<c>curl -X PUT URL</c>), usually to replace a resource; add the body with
        /// <see cref="WithJson"/>, <see cref="WithData"/> or <see cref="WithBinaryData"/>.
        /// </summary>
        /// <remarks>
        /// <para>Unlike a GET, adding a body keeps the method PUT. To upload a file's raw bytes as the PUT
        /// body (curl's <c>-T file</c>), read it with <c>File.ReadAllBytes</c> and pass it to
        /// <see cref="WithBinaryData"/>.</para>
        /// <para>Example: <c>await CurlRequestBuilder.Put("https://api.example.com/users/42").WithJson(new { name = "Jane" }).WithBearerToken(token).ExecuteAsync();</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">Absolute URL of the resource, such as <c>https://api.example.com/users/42</c>. Must not be null.</param>
        /// <returns>A new builder whose method is PUT.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> is null. Pass the URL string; see
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</exception>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.PutAsync"/>
        /// <seealso cref="Patch"/>
        public static CurlRequestBuilder Put(string url)
        {
            return new CurlRequestBuilder(url, "PUT");
        }

        /// <summary>
        /// Starts a DELETE request (<c>curl -X DELETE URL</c>) to remove the resource at
        /// <paramref name="url"/>; a body is optional.
        /// </summary>
        /// <remarks>
        /// <para>Most APIs answer 200, 202 or 204 (no body). Add <see cref="FailOnError"/> to turn a 404 or 403
        /// into an exception instead of a result with that status code.</para>
        /// <para>Example: <c>var r = await CurlRequestBuilder.Delete("https://api.example.com/users/42").WithBearerToken(token).ExecuteAsync();</c>
        /// then <c>if (r.StatusCode == 204) Console.WriteLine("deleted");</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">Absolute URL of the resource to delete. Must not be null.</param>
        /// <returns>A new builder whose method is DELETE.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> is null. Pass the URL string; see
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</exception>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.DeleteAsync"/>
        public static CurlRequestBuilder Delete(string url)
        {
            return new CurlRequestBuilder(url, "DELETE");
        }

        /// <summary>
        /// Starts a PATCH request (<c>curl -X PATCH URL</c>) for partial updates; add the changed fields with
        /// <see cref="WithJson"/> or <see cref="WithData"/>.
        /// </summary>
        /// <remarks>
        /// <para>For JSON Merge Patch or JSON Patch set the media type yourself, for example
        /// <c>.WithData(patchJson).WithHeader("Content-Type", "application/merge-patch+json")</c>.</para>
        /// <para>Example: <c>await CurlRequestBuilder.Patch("https://api.example.com/users/42").WithJson(new { email = "new@example.com" }).ExecuteAsync();</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">Absolute URL of the resource to update. Must not be null.</param>
        /// <returns>A new builder whose method is PATCH.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> is null. Pass the URL string; see
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</exception>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.PatchAsync"/>
        /// <seealso cref="Put"/>
        public static CurlRequestBuilder Patch(string url)
        {
            return new CurlRequestBuilder(url, "PATCH");
        }

        /// <summary>
        /// Starts a HEAD request: the server sends only the status line and headers, so you can check
        /// existence, size (<c>Content-Length</c>) or <c>Last-Modified</c> without downloading the body.
        /// </summary>
        /// <remarks>
        /// <para>The result has <see cref="CurlResult.StatusCode"/> and <see cref="CurlResult.Headers"/> filled and
        /// an empty <see cref="CurlResult.Body"/>. curl's <c>-I</c> also prints the headers as the output; add
        /// <see cref="IncludeHeaders"/> if you want them in <see cref="CurlResult.Body"/> too.
        /// <see cref="ToCurlCommand"/> prints this request as <c>curl -X HEAD URL</c>.</para>
        /// <para>Example: <c>var head = await CurlRequestBuilder.Head("https://example.com/big.iso").ExecuteAsync();</c>
        /// then <c>long size = long.Parse(head.GetHeader("Content-Length"));</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-head-request-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">Absolute URL to probe. Must not be null.</param>
        /// <returns>A new builder whose method is HEAD.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> is null. Pass the URL string; see
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</exception>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.HeadAsync"/>
        /// <seealso cref="Get"/>
        public static CurlRequestBuilder Head(string url)
        {
            return new CurlRequestBuilder(url, "HEAD");
        }

        /// <summary>
        /// Starts a request with any HTTP method, such as <c>OPTIONS</c>, <c>PROPFIND</c> or <c>PURGE</c>
        /// (curl's <c>-X METHOD</c>); the method name is upper-cased.
        /// </summary>
        /// <remarks>
        /// <para>GET, POST, PUT, DELETE, HEAD, OPTIONS and PATCH behave exactly like their dedicated factories, and a
        /// body added with <see cref="WithData"/> or <see cref="WithJson"/> is sent with them.</para>
        /// <para>Known limitation: any other name (<c>PROPFIND</c>, <c>PURGE</c>, <c>MKCOL</c>...) is kept in
        /// <see cref="CurlOptions.Method"/> and printed by <see cref="ToCurlCommand"/> as <c>-X NAME</c>, but the builder
        /// does not set <see cref="CurlOptions.CustomMethod"/>, so the request goes out as GET. Until that changes, send
        /// such verbs from a command string, where <c>-X</c> is honoured:
        /// <c>await Curl.ExecuteAsync("curl -X PROPFIND -H 'Depth: 1' https://dav.example.com/files/");</c></para>
        /// <para>Example: <c>var r = await CurlRequestBuilder.Request("options", "https://api.example.com/items").ExecuteAsync();</c>
        /// then <c>r.GetHeader("Allow")</c> lists the methods the server accepts.</para>
        /// </remarks>
        /// <param name="method">The HTTP method token, such as <c>"OPTIONS"</c> or <c>"purge"</c> (case does not
        /// matter; it is sent upper-case). Must be a non-empty token without spaces.</param>
        /// <param name="url">Absolute URL of the request. Must not be null.</param>
        /// <returns>A new builder with that method.</returns>
        /// <exception cref="ArgumentException"><paramref name="method"/> is null, empty or contains whitespace.
        /// Pass a method name such as <c>"OPTIONS"</c>; see
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md</exception>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> is null. Pass the URL string; see
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</exception>
        /// <seealso cref="Get"/>
        /// <seealso cref="CurlOptions.CustomMethod"/>
        public static CurlRequestBuilder Request(string method, string url)
        {
            if (string.IsNullOrWhiteSpace(method) || method.Any(char.IsWhiteSpace))
            {
                throw new ArgumentException(CurlException.Compose(
                    $"CurlRequestBuilder.Request(method, url) was called with the method {(method == null ? "null" : "\"" + method + "\"")} for {ErrorText.SafeUrl(url)}; an HTTP method must be a single non-empty word.",
                    "Pass a method name such as \"OPTIONS\", \"PROPFIND\" or \"PURGE\", or use Get/Post/Put/Patch/Delete/Head.",
                    DocLinks.HowToPutDeletePatch), nameof(method));
            }

            return new CurlRequestBuilder(url, method.ToUpperInvariant());
        }

        #endregion

        #region Headers

        /// <summary>
        /// Adds a request header (curl's <c>-H 'Name: value'</c>); calling it again with the same name
        /// (in any letter case) replaces the earlier value.
        /// </summary>
        /// <remarks>
        /// <para><paramref name="name"/> is the header field name without the colon, for example
        /// <c>Accept</c>, <c>X-Api-Key</c> or <c>Content-Type</c>; <paramref name="value"/> is the text after
        /// <c>": "</c>, sent unchanged. An empty value sends the header with no value. Headers you set win over
        /// the defaults (<c>User-Agent: curl/8.x</c>, <c>Accept: */*</c>).</para>
        /// <para><c>Content-Type</c> is applied to text bodies from <see cref="WithData"/>; <see cref="WithJson"/>,
        /// <see cref="WithFormData"/> and multipart bodies set their own.</para>
        /// <para>Example: <c>CurlRequestBuilder.Get(url).WithHeader("Accept", "application/vnd.github.v3+json").WithHeader("X-Request-Id", Guid.NewGuid().ToString())</c>
        /// is <c>curl -H 'Accept: application/vnd.github.v3+json' -H 'X-Request-Id: ...' URL</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md</para>
        /// </remarks>
        /// <param name="name">Header field name such as <c>"Accept"</c> (no colon, no spaces).</param>
        /// <param name="value">Header value such as <c>"application/json"</c>, sent exactly as given.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <example>
        /// <code language="csharp">
        /// var result = await CurlRequestBuilder
        ///     .Get("https://api.example.com/protected")
        ///     .WithHeader("X-API-Key", "your-api-key-here")
        ///     .WithHeader("X-Client-Version", "1.0.0")
        ///     .ExecuteAsync();
        /// </code>
        /// </example>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null, empty or whitespace. Fix: pass a header
        /// name such as <c>"Accept"</c>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md</exception>
        /// <seealso cref="WithHeaders"/>
        /// <seealso cref="WithUserAgent"/>
        /// <seealso cref="CurlOptions.Headers"/>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.WithHeader"/>
        public CurlRequestBuilder WithHeader(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name), CurlException.Compose(
                    $"CurlRequestBuilder.WithHeader(name, value) was called with {(name == null ? "a null" : "an empty")} header name for {ErrorText.SafeUrl(_options.Url)}.",
                    "Pass a header name such as \"Accept\" or \"X-Api-Key\"; the value goes in the second argument.",
                    DocLinks.HowToHeaders));
            }
            _options.Headers[name] = value;
            return this;
        }

        /// <summary>
        /// Adds several request headers at once (one <c>-H</c> per entry); existing headers with the same names
        /// are replaced.
        /// </summary>
        /// <remarks>
        /// <para>Each key is a header name (<c>"Accept"</c>) and each value the text after the colon. Handy for
        /// headers kept in configuration: <c>.WithHeaders(config.GetSection("ApiHeaders").Get&lt;Dictionary&lt;string, string&gt;&gt;())</c>.</para>
        /// <para>Example: <c>.WithHeaders(new Dictionary&lt;string, string&gt; { ["Accept"] = "application/json", ["X-Tenant"] = "acme" })</c>
        /// is <c>-H 'Accept: application/json' -H 'X-Tenant: acme'</c>.</para>
        /// </remarks>
        /// <param name="headers">Header names mapped to values; must not be null (an empty dictionary adds nothing).</param>
        /// <returns>This builder, for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="headers"/> is null. Pass an empty dictionary
        /// to add nothing; see https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md</exception>
        /// <seealso cref="WithHeader"/>
        /// <seealso cref="CurlOptions.Headers"/>
        public CurlRequestBuilder WithHeaders(Dictionary<string, string> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers), CurlException.Compose(
                    "CurlRequestBuilder.WithHeaders(headers) was called with a null dictionary.",
                    "Pass a Dictionary<string, string> of header names and values, or an empty dictionary to add none.",
                    DocLinks.HowToHeaders));
            }
            foreach (var header in headers)
            {
                _options.Headers[header.Key] = header.Value;
            }
            return this;
        }

        /// <summary>
        /// Sets the <c>User-Agent</c> header that identifies your application (curl's <c>-A</c> /
        /// <c>--user-agent</c>); without it CurlDotNet sends <c>curl/8.x</c> like the curl tool.
        /// </summary>
        /// <remarks>
        /// <para>Many APIs (GitHub, Reddit, Wikipedia) reject or throttle requests without a descriptive
        /// agent; use a value such as <c>"MyCompany-Sync/2.1 (+https://mycompany.example/contact)"</c>.</para>
        /// <para>Example: <c>CurlRequestBuilder.Get("https://api.github.com/repos/dotnet/runtime").WithUserAgent("MyApp/1.0")</c>
        /// is <c>curl -A 'MyApp/1.0' https://api.github.com/repos/dotnet/runtime</c>.</para>
        /// </remarks>
        /// <param name="userAgent">The full User-Agent value, such as <c>"MyApp/1.0"</c>; null or empty keeps the default.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithHeader"/>
        /// <seealso cref="CurlOptions.UserAgent"/>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.WithUserAgent"/>
        public CurlRequestBuilder WithUserAgent(string userAgent)
        {
            _options.UserAgent = userAgent;
            return this;
        }

        /// <summary>
        /// Sets the <c>Referer</c> header, the page the request claims to come from (curl's <c>-e</c> /
        /// <c>--referer</c>); some sites require it for downloads or hot-link protection.
        /// </summary>
        /// <remarks>
        /// <para>Pass a full URL such as <c>"https://www.example.com/products"</c>. curl's <c>-e ';auto'</c>
        /// (update the referer on redirects) is not available here; use the command string for that.</para>
        /// <para>Example: <c>CurlRequestBuilder.Get(imageUrl).WithReferer("https://www.example.com/gallery").SaveToFile("photo.jpg")</c>
        /// is <c>curl -e 'https://www.example.com/gallery' -o photo.jpg IMAGEURL</c>.</para>
        /// </remarks>
        /// <param name="referer">The referring URL; null or empty sends no Referer header.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithHeader"/>
        /// <seealso cref="CurlOptions.Referer"/>
        public CurlRequestBuilder WithReferer(string referer)
        {
            _options.Referer = referer;
            return this;
        }

        #endregion

        #region Body/Data

        /// <summary>
        /// Sends <paramref name="data"/> as the request body (curl's <c>-d</c> / <c>--data</c>) with
        /// <c>Content-Type: application/x-www-form-urlencoded</c>, switching a GET to POST.
        /// </summary>
        /// <remarks>
        /// <para>The string is sent as UTF-8 exactly as given; it is not URL-encoded for you, so
        /// <c>"name=Ada Lovelace&amp;lang=en"</c> must already be encoded as <c>"name=Ada%20Lovelace&amp;lang=en"</c>
        /// (or use <see cref="WithFormData"/>, which encodes each pair). A value starting with <c>@</c>, such as
        /// <c>"@payload.json"</c>, sends the bytes of that file instead, like <c>curl -d @payload.json</c>.</para>
        /// <para>To send another media type, add <c>.WithHeader("Content-Type", "text/plain")</c>; for JSON
        /// objects prefer <see cref="WithJson"/>. A request that was GET (or had no method) becomes POST; PUT,
        /// PATCH and DELETE keep their method.</para>
        /// <para>Example: <c>CurlRequestBuilder.Post("https://httpbin.org/post").WithData("key1=value1&amp;key2=value2")</c>
        /// is <c>curl -X POST -d 'key1=value1&amp;key2=value2' https://httpbin.org/post</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/11-forms-and-data.md</para>
        /// </remarks>
        /// <param name="data">The body text, such as <c>"a=1&amp;b=2"</c> or a JSON string, or <c>"@file"</c> to send a
        /// file's contents. Null or empty sends no body.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithJson"/>
        /// <seealso cref="WithFormData"/>
        /// <seealso cref="WithBinaryData"/>
        /// <seealso cref="CurlOptions.Data"/>
        /// <seealso cref="Curl.PostAsync(string, string)"/>
        public CurlRequestBuilder WithData(string data)
        {
            _options.Data = data;
            if (string.IsNullOrEmpty(_options.Method) || _options.Method == "GET")
                _options.Method = "POST";
            return this;
        }

        /// <summary>
        /// Serializes <paramref name="data"/> to JSON and sends it as the body with
        /// <c>Content-Type: application/json</c> (curl's <c>-H 'Content-Type: application/json' -d JSON</c>), switching a GET to POST.
        /// </summary>
        /// <remarks>
        /// <para>Any object works: an anonymous object (<c>new { name = "Ada" }</c>), your own class, a dictionary
        /// or a list. Serialization uses System.Text.Json on .NET 8/9/10 and .NET Framework 4.7.2+/4.8 (the same
        /// settings as <see cref="Curl.PostJsonAsync(string, object)"/>) and Newtonsoft.Json on .NET Standard 2.0;
        /// property names are written as declared in C#. A string is serialized as a JSON string literal, so
        /// to send JSON text you already have, use <c>.WithData(json).WithHeader("Content-Type", "application/json")</c>.</para>
        /// <para>The Content-Type header is set to <c>application/json</c>, replacing an earlier value; add an
        /// <c>Accept</c> header yourself if the API needs one. PUT and PATCH keep their method.</para>
        /// <para>Example: <c>var r = await CurlRequestBuilder.Post("https://api.example.com/users").WithJson(new { name = "John", age = 30 }).ExecuteAsync();</c>
        /// sends <c>{"name":"John","age":30}</c>, the same as <c>curl -H 'Content-Type: application/json' -d '{"name":"John","age":30}' https://api.example.com/users</c>
        /// (curl's <c>--json</c> also adds <c>Accept: application/json</c>; add <c>.WithHeader("Accept", "application/json")</c> for that).</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</para>
        /// </remarks>
        /// <param name="data">The object to serialize: anonymous object, class instance, dictionary, list or value. Null sends the JSON literal <c>null</c>.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <example>
        /// <code language="csharp">
        /// var user = new User { Name = "John", Email = "john@example.com" };
        /// var result = await CurlRequestBuilder
        ///     .Put("https://api.example.com/users/123")
        ///     .WithJson(user)
        ///     .WithBearerToken("your-token")
        ///     .ExecuteAsync();
        /// </code>
        /// </example>
        /// <exception cref="CurlParsingException"><paramref name="data"/> cannot be serialized to JSON (a reference cycle,
        /// a delegate or stream member); the serializer's exception is the InnerException. Fix: pass a plain DTO, or
        /// serialize it yourself and use <see cref="WithData"/>. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</exception>
        /// <seealso cref="WithData"/>
        /// <seealso cref="Curl.PostJsonAsync(string, object)"/>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.PostAsync"/>
        /// <seealso cref="CurlResult.ParseJson{T}"/>
        public CurlRequestBuilder WithJson(object data)
        {
            _options.Data = Curl.SerializeJsonOrThrow(data, "CurlRequestBuilder.WithJson(data)", _options.Url,
                "or serialize it yourself and call WithData(json) plus WithHeader(\"Content-Type\", \"application/json\")");
            _options.Headers["Content-Type"] = "application/json";
            if (string.IsNullOrEmpty(_options.Method) || _options.Method == "GET")
                _options.Method = "POST";
            return this;
        }

        /// <summary>
        /// Sends an HTML-form body (<c>application/x-www-form-urlencoded</c>), URL-encoding every name and value
        /// for you; switches a GET to POST. Equivalent to one <c>--data-urlencode name=value</c> per pair.
        /// </summary>
        /// <remarks>
        /// <para>Each entry becomes <c>name=value</c> joined with <c>&amp;</c>, both sides escaped with
        /// <see cref="Uri.EscapeDataString(string)"/> (space becomes <c>%20</c>, <c>&amp;</c> becomes <c>%26</c>).
        /// The body replaces anything set earlier with <see cref="WithData"/>, and the Content-Type header is set to
        /// <c>application/x-www-form-urlencoded</c>. For file fields use <see cref="WithMultipartForm"/> instead.</para>
        /// <para><see cref="ToCurlCommand"/> prints the encoded body as <c>-d 'user=ada&amp;pass=p%40ss'</c> plus the
        /// <c>-H 'Content-Type: ...'</c> header.</para>
        /// <para>Example: <c>.WithFormData(new Dictionary&lt;string, string&gt; { ["username"] = "ada", ["password"] = "p@ss word" })</c>
        /// sends <c>username=ada&amp;password=p%40ss%20word</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/11-forms-and-data.md</para>
        /// </remarks>
        /// <param name="formData">Field names mapped to plain (unencoded) values; must not be null.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="formData"/> is null. Pass the fields to send; see
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/11-forms-and-data.md</exception>
        /// <seealso cref="WithData"/>
        /// <seealso cref="WithMultipartForm"/>
        /// <seealso cref="CurlOptions.Data"/>
        public CurlRequestBuilder WithFormData(Dictionary<string, string> formData)
        {
            if (formData == null)
            {
                throw new ArgumentNullException(nameof(formData), CurlException.Compose(
                    "CurlRequestBuilder.WithFormData(formData) was called with a null dictionary.",
                    "Pass a Dictionary<string, string> of field names and plain values (they are URL-encoded for you).",
                    DocLinks.TutorialFormsAndData));
            }
            var pairs = new List<string>();
            foreach (var kvp in formData)
            {
                pairs.Add($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value ?? string.Empty)}");
            }
            _options.Data = string.Join("&", pairs);
            _options.Headers["Content-Type"] = "application/x-www-form-urlencoded";
            if (string.IsNullOrEmpty(_options.Method) || _options.Method == "GET")
                _options.Method = "POST";
            return this;
        }

        /// <summary>
        /// Sends a <c>multipart/form-data</c> body with text fields and file uploads (one curl <c>-F</c> per
        /// entry), switching a GET to POST; this is how browsers upload files.
        /// </summary>
        /// <remarks>
        /// <para><paramref name="fields"/> maps form field names to text values (<c>-F 'name=value'</c>).
        /// <paramref name="files"/> maps field names to local file paths (<c>-F 'name=@path'</c>); each file is read
        /// when the request runs and sent with its file name and a Content-Type guessed from the extension.
        /// This call replaces fields and files set earlier by this method, <see cref="WithFile(string, string)"/>
        /// or <see cref="WithUploadFile"/>; call it first and add more files afterwards if needed.</para>
        /// <para>A missing or unreadable file is reported when the request runs, as a
        /// <see cref="CurlException"/> with exit code 26 naming the path.</para>
        /// <para>Example: <c>.WithMultipartForm(new Dictionary&lt;string, string&gt; { ["title"] = "Q3 report" }, new Dictionary&lt;string, string&gt; { ["document"] = @"C:\reports\q3.pdf" })</c>
        /// is <c>curl -F 'title=Q3 report' -F 'document=@C:\reports\q3.pdf' URL</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md</para>
        /// </remarks>
        /// <param name="fields">Text fields (name to value); null means none.</param>
        /// <param name="files">File fields (name to local path, absolute or relative to the current directory); null means none.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithFile(string, string)"/>
        /// <seealso cref="WithUploadFile"/>
        /// <seealso cref="WithFormData"/>
        /// <seealso cref="CurlOptions.FormData"/>
        /// <seealso cref="CurlOptions.Files"/>
        public CurlRequestBuilder WithMultipartForm(Dictionary<string, string> fields, Dictionary<string, string> files = null)
        {
            _options.FormData = fields ?? new Dictionary<string, string>();
            _options.Files = files ?? new Dictionary<string, string>();
            if (string.IsNullOrEmpty(_options.Method) || _options.Method == "GET")
                _options.Method = "POST";
            return this;
        }

        /// <summary>
        /// Sends raw bytes as the request body exactly as given (curl's <c>--data-binary</c>), switching a GET
        /// to POST; use it for images, protobuf, gzip payloads or a file's contents.
        /// </summary>
        /// <remarks>
        /// <para>The bytes are sent without a <c>Content-Type</c> header. A multipart form (<see cref="WithMultipartForm"/>) takes precedence over these
        /// bytes, and these bytes take precedence over <see cref="WithData"/> text. PUT and PATCH keep their method,
        /// so <c>Put(url).WithBinaryData(File.ReadAllBytes("disk.img"))</c> is the builder form of <c>curl -T disk.img URL</c>.</para>
        /// <para><see cref="ToCurlCommand"/> can only show bytes that are plain UTF-8 text: they are printed as
        /// <c>--data-binary 'TEXT'</c>. Real binary data cannot be written on a command line and is left out of
        /// the printed command (save it to a file and use <c>--data-binary @file</c> if you need to reproduce it in a shell).</para>
        /// <para>Example: <c>await CurlRequestBuilder.Put("https://storage.example.com/bucket/cat.png").WithBinaryData(File.ReadAllBytes("cat.png")).ExecuteAsync();</c>
        /// uploads the image bytes unchanged.</para>
        /// </remarks>
        /// <param name="data">The exact body bytes; null sends no body.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithData"/>
        /// <seealso cref="CurlOptions.BinaryData"/>
        public CurlRequestBuilder WithBinaryData(byte[] data)
        {
            _options.BinaryData = data;
            if (string.IsNullOrEmpty(_options.Method) || _options.Method == "GET")
                _options.Method = "POST";
            return this;
        }

        /// <summary>
        /// Adds a file upload field to a <c>multipart/form-data</c> body (curl's <c>-F 'field=@path'</c>) after
        /// checking the file exists; switches a GET to POST.
        /// </summary>
        /// <remarks>
        /// <para>The file is read when the request runs and sent with its file name and a Content-Type guessed
        /// from the extension. Call it several times to upload several files; a later call with the same
        /// <paramref name="fieldName"/> replaces the earlier path.</para>
        /// <para>Unlike <see cref="WithUploadFile"/>, this method checks immediately that the file exists, so a
        /// typo fails at the line that caused it.</para>
        /// <para>Example: <c>await CurlRequestBuilder.Post("https://api.example.com/upload").WithFile("avatar", "me.jpg").ExecuteAsync();</c>
        /// is <c>curl -F 'avatar=@me.jpg' https://api.example.com/upload</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md</para>
        /// </remarks>
        /// <param name="fieldName">The form field name the server expects, such as <c>"file"</c> or <c>"avatar"</c>.</param>
        /// <param name="filePath">Path of the file to upload, absolute or relative to the current directory.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <exception cref="CurlReadErrorException">The file does not exist (exit code 26, curl's
        /// "Failed to open/read local data"). The message names the path, the field and the current directory;
        /// fix the path or use an absolute one. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md</exception>
        /// <seealso cref="WithUploadFile"/>
        /// <seealso cref="WithMultipartForm"/>
        /// <seealso cref="CurlOptions.Files"/>
        public CurlRequestBuilder WithFile(string fieldName, string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new CurlReadErrorException(filePath ?? "(null)", CurlException.Compose(
                    $"WithFile(\"{fieldName}\", \"{filePath}\") cannot upload the file: it does not exist (looked in '{(string.IsNullOrEmpty(filePath) ? Directory.GetCurrentDirectory() : Path.GetFullPath(filePath))}', current directory '{Directory.GetCurrentDirectory()}').",
                    "Check the spelling of the path, or pass an absolute path (for example Path.Combine(AppContext.BaseDirectory, \"file.pdf\")) so it does not depend on the current directory.",
                    DocLinks.HowToUpload));
            }

            _options.Files[fieldName] = filePath;
            if (string.IsNullOrEmpty(_options.Method) || _options.Method == "GET")
                _options.Method = "POST";
            return this;
        }

        #endregion

        #region Authentication

        /// <summary>
        /// Sends HTTP Basic authentication (curl's <c>-u user:password</c>): an
        /// <c>Authorization: Basic base64(user:password)</c> header on every request, redirects to the same host included.
        /// </summary>
        /// <remarks>
        /// <para>Basic auth sends the password base64-encoded, not encrypted; only use it over <c>https://</c>.
        /// The credentials are sent preemptively (curl's default); for Digest or NTLM use a command string with
        /// <c>--digest</c> or <c>--ntlm</c>.</para>
        /// <para><see cref="ToCurlCommand"/> prints <c>-u 'user:password'</c> with the password in clear text, so
        /// do not log that string in production.</para>
        /// <para>Example: <c>CurlRequestBuilder.Get("https://api.example.com/me").WithBasicAuth("ada", "s3cret")</c>
        /// is <c>curl -u 'ada:s3cret' https://api.example.com/me</c>; a wrong password gives a result with
        /// <c>StatusCode == 401</c>, not an exception.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-basic-auth-csharp/README.md</para>
        /// </remarks>
        /// <param name="username">The user name (may contain <c>@</c>, for example an email address).</param>
        /// <param name="password">The password; may be empty.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithBearerToken"/>
        /// <seealso cref="CurlOptions.Credentials"/>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.WithBasicAuth"/>
        public CurlRequestBuilder WithBasicAuth(string username, string password)
        {
            _options.Credentials = new NetworkCredential(username, password);
            return this;
        }

        /// <summary>
        /// Sends OAuth 2 / JWT bearer authentication: the header <c>Authorization: Bearer TOKEN</c>
        /// (curl's <c>-H 'Authorization: Bearer TOKEN'</c> or <c>--oauth2-bearer TOKEN</c>).
        /// </summary>
        /// <remarks>
        /// <para>Pass the token alone, without the word <c>Bearer</c>. It replaces any Authorization header set
        /// earlier (including one from <see cref="WithAuth"/>). Expired tokens usually give a 401 result; refresh
        /// the token and build the request again.</para>
        /// <para>Example: <c>CurlRequestBuilder.Get("https://api.github.com/user").WithBearerToken(Environment.GetEnvironmentVariable("GITHUB_TOKEN"))</c>
        /// is <c>curl -H 'Authorization: Bearer ghp_...' https://api.github.com/user</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-bearer-token-csharp/README.md</para>
        /// </remarks>
        /// <param name="token">The access token, such as <c>"eyJhbGciOi..."</c> or <c>"ghp_..."</c>.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithAuth"/>
        /// <seealso cref="WithBasicAuth"/>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.WithBearerToken"/>
        public CurlRequestBuilder WithBearerToken(string token)
        {
            _options.Headers["Authorization"] = $"Bearer {token}";
            return this;
        }

        /// <summary>
        /// Sets the whole <c>Authorization</c> header value yourself, for schemes other than Basic and Bearer
        /// (for example <c>"Token abc123"</c>, <c>"ApiKey xyz"</c> or <c>"AWS4-HMAC-SHA256 ..."</c>).
        /// </summary>
        /// <remarks>
        /// <para>The value is sent exactly as given, equivalent to <c>curl -H 'Authorization: VALUE'</c>, and replaces
        /// any value set by <see cref="WithBearerToken"/>.</para>
        /// <para>Example: <c>CurlRequestBuilder.Get(url).WithAuth("Token 9944b09199c62bcf")</c> for Django REST
        /// Framework token authentication.</para>
        /// <para>Guide to the schemes: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/auth-schemes.md</para>
        /// </remarks>
        /// <param name="authHeader">The complete header value including the scheme name, such as <c>"Token abc123"</c>.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithBearerToken"/>
        /// <seealso cref="WithHeader"/>
        public CurlRequestBuilder WithAuth(string authHeader)
        {
            _options.Headers["Authorization"] = authHeader;
            return this;
        }

        #endregion

        #region Options

        /// <summary>
        /// Limits the whole transfer (connect, send, wait and download) to <paramref name="timeout"/>
        /// (curl's <c>-m</c> / <c>--max-time</c>); without it there is no limit, like curl.
        /// </summary>
        /// <remarks>
        /// <para>The TimeSpan is converted to seconds with decimals, so <c>TimeSpan.FromMilliseconds(1500)</c> is
        /// <c>--max-time 1.5</c>. <see cref="TimeSpan.Zero"/> means no limit. When the limit expires the request
        /// fails with <see cref="CurlOperationTimeoutException"/> (exit code 28). <see cref="Curl.DefaultMaxTimeSeconds"/>
        /// sets a process-wide default for requests that do not set one.</para>
        /// <para>Example: <c>CurlRequestBuilder.Get(url).WithTimeout(TimeSpan.FromSeconds(30)).WithConnectTimeout(TimeSpan.FromSeconds(5))</c>
        /// is <c>curl --max-time 30 --connect-timeout 5 URL</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</para>
        /// </remarks>
        /// <param name="timeout">Maximum total time; <see cref="TimeSpan.Zero"/> means unlimited.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithConnectTimeout"/>
        /// <seealso cref="CurlOptions.MaxTime"/>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.WithTimeout"/>
        public CurlRequestBuilder WithTimeout(TimeSpan timeout)
        {
            _options.MaxTime = timeout.TotalSeconds;
            return this;
        }

        /// <summary>
        /// Limits only the connection phase (DNS lookup, TCP connect and TLS handshake) to
        /// <paramref name="timeout"/> (curl's <c>--connect-timeout</c>); the download itself is not limited.
        /// </summary>
        /// <remarks>
        /// <para>Converted to seconds with decimals (<c>TimeSpan.FromMilliseconds(500)</c> is
        /// <c>--connect-timeout 0.5</c>). <see cref="TimeSpan.Zero"/> means the default (no extra limit). An unreachable
        /// host then fails fast with <see cref="CurlOperationTimeoutException"/> (exit code 28) instead of waiting
        /// for the operating system's TCP timeout.</para>
        /// <para>Example: <c>.WithConnectTimeout(TimeSpan.FromSeconds(3))</c> for a health check that should give up
        /// quickly on a dead server.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</para>
        /// </remarks>
        /// <param name="timeout">Maximum time to establish the connection; <see cref="TimeSpan.Zero"/> means no limit.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithTimeout"/>
        /// <seealso cref="CurlOptions.ConnectTimeout"/>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.WithConnectTimeout"/>
        public CurlRequestBuilder WithConnectTimeout(TimeSpan timeout)
        {
            _options.ConnectTimeout = timeout.TotalSeconds;
            return this;
        }

        /// <summary>
        /// Follows <c>Location:</c> redirects (301, 302, 303, 307, 308) to the final page, like curl's
        /// <c>-L</c> / <c>--location</c>; without it you get the redirect response itself.
        /// </summary>
        /// <remarks>
        /// <para>Up to 50 redirects are followed (change with <see cref="WithMaxRedirects"/>); more fail with
        /// <see cref="CurlTooManyRedirectsException"/> (exit code 47). As in curl, a 301/302/303 after a POST is
        /// followed with GET, and <see cref="WithBasicAuth"/> credentials are not sent to a different host.</para>
        /// <para>Example: <c>var r = await CurlRequestBuilder.Get("http://github.com").FollowRedirects().ExecuteAsync();</c>
        /// ends at <c>https://github.com/</c> with status 200, like <c>curl -L http://github.com</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-follow-redirects-csharp/README.md</para>
        /// </remarks>
        /// <param name="follow">True (the default) to follow redirects; false to return the 3xx response.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithMaxRedirects"/>
        /// <seealso cref="CurlOptions.FollowLocation"/>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.WithFollowRedirects"/>
        public CurlRequestBuilder FollowRedirects(bool follow = true)
        {
            _options.FollowLocation = follow;
            return this;
        }

        /// <summary>
        /// Sets how many redirects <see cref="FollowRedirects"/> may follow before failing (curl's
        /// <c>--max-redirs</c>, default 50).
        /// </summary>
        /// <remarks>
        /// <para>Use a small number such as 5 to detect redirect loops quickly; <c>-1</c> means unlimited. When the
        /// limit is exceeded the request fails with <see cref="CurlTooManyRedirectsException"/> (exit code 47).
        /// Has no effect unless redirects are followed. <see cref="ToCurlCommand"/> omits the flag for the default 50.</para>
        /// <para>Example: <c>.FollowRedirects().WithMaxRedirects(5)</c> is <c>curl -L --max-redirs 5 URL</c>.</para>
        /// </remarks>
        /// <param name="maxRedirects">Maximum number of redirects, such as 5; -1 for unlimited.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="FollowRedirects"/>
        /// <seealso cref="CurlOptions.MaxRedirects"/>
        public CurlRequestBuilder WithMaxRedirects(int maxRedirects)
        {
            _options.MaxRedirects = maxRedirects;
            return this;
        }

        /// <summary>
        /// Skips TLS certificate verification (curl's <c>-k</c> / <c>--insecure</c>) so self-signed or expired
        /// certificates are accepted. For local development and testing only.
        /// </summary>
        /// <remarks>
        /// <para>With verification off anyone on the network path can impersonate the server; never ship this in
        /// production code. The better fix for an internal CA is to trust its certificate (<c>--cacert</c> in a
        /// command string). Without this option an untrusted certificate fails with
        /// <see cref="CurlPeerFailedVerificationException"/> (exit code 60).</para>
        /// <para>Example: <c>CurlRequestBuilder.Get("https://localhost:5001/api/health").Insecure()</c> is
        /// <c>curl -k https://localhost:5001/api/health</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md</para>
        /// </remarks>
        /// <param name="insecure">True (the default) to skip verification; false to verify (the normal behaviour).</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="CurlOptions.Insecure"/>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.WithInsecureSsl"/>
        public CurlRequestBuilder Insecure(bool insecure = true)
        {
            _options.Insecure = insecure;
            return this;
        }

        /// <summary>
        /// Sends the request through a proxy server (curl's <c>-x</c> / <c>--proxy</c>), for example a corporate
        /// proxy, Fiddler or a SOCKS tunnel.
        /// </summary>
        /// <remarks>
        /// <para>Accepted forms: <c>http://proxy.corp:8080</c>, <c>https://proxy.corp:443</c>,
        /// <c>socks5://127.0.0.1:1080</c>, <c>socks5h://...</c> (DNS through the proxy), <c>socks4://...</c>, or just
        /// <c>proxy.corp:8080</c> (treated as http). Credentials can be embedded (<c>http://user:pass@proxy:8080</c>)
        /// or given with <see cref="WithProxy(string, string, string)"/>. Without this option the
        /// <c>http_proxy</c>/<c>https_proxy</c>/<c>all_proxy</c> environment variables apply, like curl.</para>
        /// <para>An unreachable proxy fails with <see cref="CurlCouldntConnectException"/> (exit code 7) naming the
        /// proxy; an unknown proxy host with <see cref="CurlCouldntResolveProxyException"/> (exit code 5).</para>
        /// <para>Example: <c>CurlRequestBuilder.Get("https://example.com").WithProxy("http://127.0.0.1:8888")</c>
        /// routes through Fiddler, like <c>curl -x http://127.0.0.1:8888 https://example.com</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md</para>
        /// </remarks>
        /// <param name="proxyUrl">Proxy URL such as <c>"http://proxy.corp:8080"</c> or <c>"socks5://127.0.0.1:1080"</c>; null or empty means no explicit proxy.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithProxy(string, string, string)"/>
        /// <seealso cref="CurlOptions.Proxy"/>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.WithProxy"/>
        public CurlRequestBuilder WithProxy(string proxyUrl)
        {
            _options.Proxy = proxyUrl;
            return this;
        }

        /// <summary>
        /// Sends the request through a proxy that requires a user name and password (curl's
        /// <c>-x URL --proxy-user user:password</c>).
        /// </summary>
        /// <remarks>
        /// <para>The proxy URL accepts the same forms as <see cref="WithProxy(string)"/>. The credentials are sent
        /// to the proxy as Basic authentication (<c>Proxy-Authorization</c>); they are never sent to the target
        /// server. <see cref="ToCurlCommand"/> prints them as <c>--proxy-user 'user:password'</c> in clear text.</para>
        /// <para>A rejected login gives a result with <c>StatusCode == 407</c> (or, through HTTPS tunnels, a
        /// <see cref="CurlException"/> describing the 407).</para>
        /// <para>Example: <c>.WithProxy("http://proxy.corp:8080", "DOMAIN\\ada", password)</c> is
        /// <c>curl -x http://proxy.corp:8080 --proxy-user 'DOMAIN\ada:...' URL</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md</para>
        /// </remarks>
        /// <param name="proxyUrl">Proxy URL such as <c>"http://proxy.corp:8080"</c>.</param>
        /// <param name="username">Proxy user name, for example <c>"ada"</c> or <c>"DOMAIN\\ada"</c>.</param>
        /// <param name="password">Proxy password; may be empty.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithProxy(string)"/>
        /// <seealso cref="CurlOptions.ProxyCredentials"/>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.WithProxy"/>
        public CurlRequestBuilder WithProxy(string proxyUrl, string username, string password)
        {
            _options.Proxy = proxyUrl;
            _options.ProxyCredentials = new NetworkCredential(username, password);
            return this;
        }

        /// <summary>
        /// Sends cookies with the request (curl's <c>-b</c> / <c>--cookie</c>): either a cookie string such as
        /// <c>"session=abc123; theme=dark"</c> or the path of a cookie file to read.
        /// </summary>
        /// <remarks>
        /// <para>A value containing <c>=</c> is sent as the <c>Cookie:</c> header exactly as written (pairs separated
        /// by <c>"; "</c>). A value without <c>=</c> is a file name: a Netscape/Mozilla cookie file (the format
        /// written by <see cref="WithCookieJar"/> and by browsers' export tools) or a file of raw <c>Set-Cookie</c>
        /// lines; matching cookies are sent and the cookie engine is turned on for redirects.</para>
        /// <para>To keep a login session across requests, read and write the same file:
        /// <c>.WithCookie("cookies.txt").WithCookieJar("cookies.txt")</c> is <c>curl -b cookies.txt -c cookies.txt URL</c>.</para>
        /// <para>Example: <c>CurlRequestBuilder.Get("https://example.com/account").WithCookie("session=abc123; lang=en")</c>
        /// is <c>curl -b 'session=abc123; lang=en' https://example.com/account</c>.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/cookies.md</para>
        /// </remarks>
        /// <param name="cookie">A cookie string like <c>"name=value; other=2"</c>, or the path of a cookie file.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithCookieJar"/>
        /// <seealso cref="CurlOptions.Cookie"/>
        public CurlRequestBuilder WithCookie(string cookie)
        {
            _options.Cookie = cookie;
            return this;
        }

        /// <summary>
        /// Saves every cookie the server sets to a Netscape-format file after the transfer (curl's <c>-c</c> /
        /// <c>--cookie-jar</c>), so a later request can send them back with <see cref="WithCookie"/>.
        /// </summary>
        /// <remarks>
        /// <para>The file is written when the request finishes (also after redirects) and overwritten each time;
        /// <c>"-"</c> writes the jar to the end of <see cref="CurlResult.Body"/> instead of a file. The folder must
        /// exist; otherwise the request fails with a <see cref="CurlException"/> naming the path. Session cookies
        /// are included.</para>
        /// <para>Example, log in then call an API with the session: <c>await CurlRequestBuilder.Post(loginUrl).WithFormData(creds).WithCookieJar("session.txt").ExecuteAsync();</c>
        /// then <c>await CurlRequestBuilder.Get(apiUrl).WithCookie("session.txt").ExecuteAsync();</c>.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/cookies.md</para>
        /// </remarks>
        /// <param name="cookieJarPath">Path of the cookie file to write, such as <c>"cookies.txt"</c>, or <c>"-"</c> for the response body.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithCookie"/>
        /// <seealso cref="CurlOptions.CookieJar"/>
        public CurlRequestBuilder WithCookieJar(string cookieJarPath)
        {
            _options.CookieJar = cookieJarPath;
            return this;
        }

        /// <summary>
        /// Adds a file upload field to a <c>multipart/form-data</c> body (curl's <c>-F 'name=@path'</c>) without
        /// checking the file now; start from <see cref="Post"/> or <see cref="Put"/>, because the method is not changed.
        /// </summary>
        /// <remarks>
        /// <para>Unlike <see cref="WithFile(string, string)"/> and the other body methods, this one does not switch a GET
        /// to POST: on a builder created with <see cref="Get"/> the file is not sent (a GET carries no body). Use
        /// <c>CurlRequestBuilder.Post(url).WithUploadFile(...)</c>.</para>
        /// <para>The file is read when the request runs; a missing file then fails with a <see cref="CurlException"/>
        /// (exit code 26) naming the path. Use <see cref="WithFile(string, string)"/> to check the path immediately.
        /// Calling it again with the same <paramref name="parameterName"/> replaces the earlier path.</para>
        /// <para>Example: <c>CurlRequestBuilder.Post("https://api.example.com/import").WithUploadFile("csv", "export.csv")</c>
        /// is <c>curl -F 'csv=@export.csv' https://api.example.com/import</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md</para>
        /// </remarks>
        /// <param name="parameterName">The form field name the server expects, such as <c>"file"</c>.</param>
        /// <param name="filePath">Path of the file to upload, absolute or relative to the current directory.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parameterName"/> is null or empty. Fix: pass the form field
        /// name the server expects, such as <c>"file"</c>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md</exception>
        /// <seealso cref="WithFile(string, string)"/>
        /// <seealso cref="WithFile(string)"/>
        /// <seealso cref="CurlOptions.Files"/>
        public CurlRequestBuilder WithUploadFile(string parameterName, string filePath)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                throw new ArgumentNullException(nameof(parameterName), CurlException.Compose(
                    $"CurlRequestBuilder.WithUploadFile(parameterName, filePath) was called with {(parameterName == null ? "a null" : "an empty")} form field name for '{filePath}'.",
                    "Pass the field name the server expects, such as \"file\" or \"attachment\" (curl's -F 'file=@path').",
                    DocLinks.HowToUpload));
            }
            _options.Files[parameterName] = filePath;
            return this;
        }

        /// <summary>
        /// Uploads one file in a <c>multipart/form-data</c> body under the field name <c>file</c> (curl's
        /// <c>-F 'file=@path'</c>); start from <see cref="Post"/> or <see cref="Put"/>, because the method is not changed.
        /// </summary>
        /// <remarks>
        /// <para>Like <see cref="WithUploadFile"/>, this does not switch a GET to POST, so call it on a builder created with
        /// <see cref="Post"/> or <see cref="Put"/>.</para>
        /// <para>Shorthand for <c>WithUploadFile("file", filePath)</c>; use <see cref="WithFile(string, string)"/> when the
        /// server expects another field name. The file is read when the request runs; a missing file fails with exit code 26.</para>
        /// <para>Example: <c>await CurlRequestBuilder.Post("https://httpbin.org/post").WithFile("report.pdf").ExecuteAsync();</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md</para>
        /// </remarks>
        /// <param name="filePath">Path of the file to upload, absolute or relative to the current directory.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithUploadFile"/>
        /// <seealso cref="WithFile(string, string)"/>
        public CurlRequestBuilder WithFile(string filePath)
        {
            return WithUploadFile("file", filePath);
        }

        /// <summary>
        /// Writes the response body to a file instead of keeping it only in memory (curl's <c>-o</c> /
        /// <c>--output</c>); binary downloads are saved byte for byte.
        /// </summary>
        /// <remarks>
        /// <para>The path may be absolute or relative to the current directory; an existing file is overwritten.
        /// The folder must exist: a missing folder fails with <see cref="CurlWriteErrorException"/> (exit code 23)
        /// naming the path (curl's <c>--create-dirs</c> in a command string creates it). <c>"-"</c> means no file
        /// (standard output, i.e. <see cref="CurlResult.Body"/>). The written path is listed in
        /// <see cref="CurlResult.OutputFiles"/>.</para>
        /// <para>Example: <c>await CurlRequestBuilder.Get("https://example.com/report.xlsx").WithOutput(@"C:\downloads\report.xlsx").ExecuteAsync();</c>
        /// is <c>curl -o 'C:\downloads\report.xlsx' https://example.com/report.xlsx</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <param name="filePath">Destination file path, such as <c>"data/out.json"</c>.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="SaveToFile"/>
        /// <seealso cref="SaveWithRemoteName"/>
        /// <seealso cref="CurlOptions.OutputFile"/>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.WithOutputFile"/>
        /// <seealso cref="Curl.DownloadAsync(string, string)"/>
        public CurlRequestBuilder WithOutput(string filePath)
        {
            _options.OutputFile = filePath;
            return this;
        }

        /// <summary>
        /// Writes the response body to <paramref name="filePath"/> (curl's <c>-o</c>); same as
        /// <see cref="WithOutput"/>, named for readability in download code.
        /// </summary>
        /// <remarks>
        /// <para>Overwrites an existing file; the folder must exist (otherwise exit code 23 with the path in the
        /// message). The saved path appears in <see cref="CurlResult.OutputFiles"/>.</para>
        /// <para>Example: <c>await CurlRequestBuilder.Get(url).SaveToFile("logo.png").ExecuteAsync();</c> is <c>curl -o logo.png URL</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <param name="filePath">Destination file path, absolute or relative to the current directory.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithOutput"/>
        /// <seealso cref="Curl.DownloadAsync(string, string)"/>
        public CurlRequestBuilder SaveToFile(string filePath)
        {
            return WithOutput(filePath);
        }

        /// <summary>
        /// Saves the response body in the current directory under the file name taken from the URL
        /// (curl's <c>-O</c> / <c>--remote-name</c>), for example <c>setup.exe</c> for <c>https://x.com/dl/setup.exe</c>.
        /// </summary>
        /// <remarks>
        /// <para>The name is the last path segment of the URL without the query string; a URL ending in <c>/</c>
        /// saves as <c>curl_response</c>. An existing file is overwritten. The saved path appears in
        /// <see cref="CurlResult.OutputFiles"/>.</para>
        /// <para>Example: <c>await CurlRequestBuilder.Get("https://example.com/files/data.zip").SaveWithRemoteName().ExecuteAsync();</c>
        /// creates <c>data.zip</c>, like <c>curl -O https://example.com/files/data.zip</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithOutput"/>
        /// <seealso cref="CurlOptions.UseRemoteFileName"/>
        public CurlRequestBuilder SaveWithRemoteName()
        {
            _options.UseRemoteFileName = true;
            return this;
        }

        /// <summary>
        /// Puts the response status line and headers in front of the body in <see cref="CurlResult.Body"/>
        /// (and in the <c>-o</c> file), like curl's <c>-i</c> / <c>--include</c>.
        /// </summary>
        /// <remarks>
        /// <para>Headers are always available in <see cref="CurlResult.Headers"/>; use this only when you want the
        /// raw text curl would print, for example to log the full response.</para>
        /// <para>Example: <c>(await CurlRequestBuilder.Get(url).IncludeHeaders().ExecuteAsync()).Body</c> starts with
        /// <c>HTTP/1.1 200 OK</c>, like <c>curl -i URL</c>.</para>
        /// </remarks>
        /// <param name="include">True (the default) to include headers; false for the body only.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="CurlOptions.IncludeHeaders"/>
        /// <seealso cref="Head"/>
        public CurlRequestBuilder IncludeHeaders(bool include = true)
        {
            _options.IncludeHeaders = include;
            return this;
        }

        /// <summary>
        /// Records curl's <c>-v</c> / <c>--verbose</c> log (request lines with <c>&gt;</c>, response headers with
        /// <c>&lt;</c>, connection notes with <c>*</c>) for troubleshooting a request.
        /// </summary>
        /// <remarks>
        /// <para>The verbose text is placed at the start of <see cref="CurlResult.Body"/>, before the response, in the
        /// same order the curl tool prints it, so use it while debugging rather than when parsing the body.
        /// For a complete byte-level log written to a file use <see cref="WithTrace"/> or
        /// <see cref="WithTraceAscii"/>.</para>
        /// <para>Example: <c>CurlRequestBuilder.Get(url).Verbose()</c> is <c>curl -v URL</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md</para>
        /// </remarks>
        /// <param name="verbose">True (the default) to record the verbose log.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithTrace"/>
        /// <seealso cref="CurlOptions.Verbose"/>
        /// <seealso cref="CurlDotNet.Lib.LibCurl.WithVerbose"/>
        public CurlRequestBuilder Verbose(bool verbose = true)
        {
            _options.Verbose = verbose;
            return this;
        }

        /// <summary>
        /// Writes a full diagnostic trace of the transfer to a file (curl's <c>--trace FILE</c>): every byte sent
        /// and received as hex plus text, connection and TLS notes, and the exact error if it fails.
        /// </summary>
        /// <remarks>
        /// <para>The trace records proxy selection, TLS verification, request and response bytes, timings and the
        /// precise failure, which makes it the first thing to capture for a request that works in curl but not
        /// here (or behind a proxy). <c>"-"</c> writes the trace to standard output. The folder must exist.</para>
        /// <para><paramref name="includeTimestamps"/> adds curl's <c>--trace-time</c> (a time prefix on every line).
        /// <see cref="ToCurlCommand"/> prints <c>--trace FILE</c> and, when set, <c>--trace-time</c>.</para>
        /// <para>Example: <c>CurlRequestBuilder.Get(url).WithTrace("trace.log", includeTimestamps: true)</c> is
        /// <c>curl --trace trace.log --trace-time URL</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md</para>
        /// </remarks>
        /// <param name="filePath">Path of the trace file to write. Use <c>"-"</c> for standard output.</param>
        /// <param name="includeTimestamps">When <c>true</c>, prefixes every line with a timestamp (like curl <c>--trace-time</c>).</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithTraceAscii"/>
        /// <seealso cref="Verbose"/>
        /// <seealso cref="CurlOptions.TraceFile"/>
        /// <seealso cref="CurlOptions.TraceTime"/>
        public CurlRequestBuilder WithTrace(string filePath, bool includeTimestamps = false)
        {
            _options.TraceFile = filePath;
            if (includeTimestamps)
            {
                _options.TraceTime = true;
            }
            return this;
        }

        /// <summary>
        /// Writes a readable text trace of the transfer to a file (curl's <c>--trace-ascii FILE</c>): the same
        /// content as <see cref="WithTrace"/> without the hexadecimal column.
        /// </summary>
        /// <remarks>
        /// <para>Best for reading headers and JSON bodies in a log; use <see cref="WithTrace"/> when you need to
        /// see exact bytes (encodings, line endings). <c>"-"</c> writes to standard output.
        /// <paramref name="includeTimestamps"/> adds <c>--trace-time</c>.</para>
        /// <para>Example: <c>CurlRequestBuilder.Post(url).WithJson(order).WithTraceAscii("order-call.txt")</c> is
        /// <c>curl -X POST -H 'Content-Type: application/json' -d '...' --trace-ascii order-call.txt URL</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md</para>
        /// </remarks>
        /// <param name="filePath">Path of the trace file to write. Use <c>"-"</c> for standard output.</param>
        /// <param name="includeTimestamps">When <c>true</c>, prefixes every line with a timestamp (like curl <c>--trace-time</c>).</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithTrace"/>
        /// <seealso cref="CurlOptions.TraceAsciiFile"/>
        public CurlRequestBuilder WithTraceAscii(string filePath, bool includeTimestamps = false)
        {
            _options.TraceAsciiFile = filePath;
            if (includeTimestamps)
            {
                _options.TraceTime = true;
            }
            return this;
        }

        /// <summary>
        /// Turns on curl's silent mode (<c>-s</c> / <c>--silent</c>): no progress meter and no warnings written to
        /// standard error. The result and any exception are unchanged.
        /// </summary>
        /// <remarks>
        /// <para>Useful in console tools where CurlDotNet would otherwise write curl-style warnings to the console
        /// (for example "Failed creating file for saving etags"). Errors are still thrown as exceptions.</para>
        /// <para>Example: <c>CurlRequestBuilder.Get(url).Silent()</c> is <c>curl -s URL</c>.</para>
        /// </remarks>
        /// <param name="silent">True (the default) for silent mode.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="Verbose"/>
        /// <seealso cref="CurlOptions.Silent"/>
        public CurlRequestBuilder Silent(bool silent = true)
        {
            _options.Silent = silent;
            return this;
        }

        /// <summary>
        /// Makes HTTP error statuses (400 and above) throw <see cref="CurlHttpReturnedErrorException"/>
        /// (exit code 22) instead of returning a result, like curl's <c>-f</c> / <c>--fail</c>.
        /// </summary>
        /// <remarks>
        /// <para>Without it a 404 or 500 is a normal <see cref="CurlResult"/> with that <see cref="CurlResult.StatusCode"/>
        /// (check it, or call <c>result.EnsureSuccess()</c>). With it the exception carries the status code, the
        /// URL, the response headers and the error page text in <see cref="CurlHttpException.ResponseBody"/>; nothing
        /// is written to a <see cref="WithOutput"/> file, like curl.</para>
        /// <para>Example: <c>try { await CurlRequestBuilder.Get(url).FailOnError().ExecuteAsync(); } catch (CurlHttpReturnedErrorException ex) when (ex.StatusCode == 404) { ... }</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</para>
        /// </remarks>
        /// <param name="fail">True (the default) to throw on HTTP errors; false to return them as results.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="CurlOptions.FailOnError"/>
        /// <seealso cref="CurlResult.EnsureSuccess"/>
        public CurlRequestBuilder FailOnError(bool fail = true)
        {
            _options.FailOnError = fail;
            return this;
        }

        /// <summary>
        /// Chooses the HTTP protocol version to request: <c>"1.0"</c>, <c>"1.1"</c>, <c>"2"</c>/<c>"2.0"</c> or
        /// <c>"3"</c>/<c>"3.0"</c> (curl's <c>--http1.0</c>, <c>--http1.1</c>, <c>--http2</c>, <c>--http3</c>).
        /// </summary>
        /// <remarks>
        /// <para>Without it .NET negotiates (normally HTTP/1.1, or HTTP/2 over TLS when the server offers it).
        /// <c>"2"</c> and <c>"3"</c> ask for that version and fall back to a lower one when the server or platform
        /// cannot do it, like curl; HTTP/3 also needs QUIC support in the operating system (msquic/libmsquic).
        /// HTTP/2 and HTTP/3 are available on the .NET 8/9/10 builds; on the .NET Standard 2.0 and .NET Framework
        /// builds use 1.0 or 1.1. <c>"1"</c> is accepted as 1.0. Unknown strings are ignored (no flag is printed by
        /// <see cref="ToCurlCommand"/> either).</para>
        /// <para>Example: <c>CurlRequestBuilder.Get("https://www.google.com").WithHttpVersion("2")</c> is
        /// <c>curl --http2 https://www.google.com</c>; the version used is in the response's status line.</para>
        /// </remarks>
        /// <param name="version">One of <c>"1.0"</c>, <c>"1.1"</c>, <c>"2"</c>, <c>"2.0"</c>, <c>"3"</c>, <c>"3.0"</c>.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="CurlOptions.HttpVersion"/>
        public CurlRequestBuilder WithHttpVersion(string version)
        {
            _options.HttpVersion = version;
            return this;
        }

        /// <summary>
        /// Asks for a compressed response (<c>Accept-Encoding: gzip, deflate, br</c>) and decompresses it for you,
        /// like curl's <c>--compressed</c>; saves bandwidth on large JSON or HTML.
        /// </summary>
        /// <remarks>
        /// <para>Brotli (<c>br</c>) is offered on .NET 8/9/10; the .NET Standard 2.0 and .NET Framework builds offer
        /// <c>gzip, deflate</c>. <see cref="CurlResult.Body"/> and the <c>-o</c> file contain the decompressed data. Without this option
        /// nothing is requested and a server that compresses anyway delivers the compressed bytes, exactly like curl.
        /// A body that cannot be decoded fails with <see cref="CurlBadContentEncodingException"/> (exit code 61).</para>
        /// <para>Example: <c>CurlRequestBuilder.Get("https://api.example.com/large-list").Compressed()</c> is
        /// <c>curl --compressed https://api.example.com/large-list</c>.</para>
        /// </remarks>
        /// <param name="compressed">True (the default) to request and decode compression.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="CurlOptions.Compressed"/>
        public CurlRequestBuilder Compressed(bool compressed = true)
        {
            _options.Compressed = compressed;
            return this;
        }

        /// <summary>
        /// Downloads only part of the resource (curl's <c>-r</c> / <c>--range</c>), sent as
        /// <c>Range: bytes=RANGE</c>; useful for resuming or sampling large files.
        /// </summary>
        /// <remarks>
        /// <para>Formats: <c>"0-499"</c> (first 500 bytes), <c>"500-999"</c>, <c>"500-"</c> (from byte 500 to the end),
        /// <c>"-500"</c> (last 500 bytes), or several ranges <c>"0-99,200-299"</c> (the server may answer multipart).
        /// A server that supports ranges answers <c>206 Partial Content</c>; one that does not sends the whole file
        /// with 200, which you can detect from <see cref="CurlResult.StatusCode"/>.</para>
        /// <para>Example: <c>CurlRequestBuilder.Get("https://example.com/video.mp4").WithRange("0-1048575").SaveToFile("first-mb.mp4")</c>
        /// is <c>curl -r 0-1048575 -o first-mb.mp4 https://example.com/video.mp4</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <param name="range">Byte range such as <c>"0-499"</c>, <c>"500-"</c>, <c>"-500"</c> or <c>"0-99,200-299"</c>, without the <c>bytes=</c> prefix.</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="CurlOptions.Range"/>
        public CurlRequestBuilder WithRange(string range)
        {
            _options.Range = range;
            return this;
        }

        /// <summary>
        /// Treats the response as binary whatever its Content-Type says, so the bytes land in
        /// <see cref="CurlResult.BinaryData"/> untouched instead of being decoded into <see cref="CurlResult.Body"/>.
        /// </summary>
        /// <remarks>
        /// <para>Use this for servers that label files wrongly (for example an .xlsx served as <c>text/plain</c>),
        /// which would otherwise be decoded as text and corrupted. Common binary types (Office documents, PDF,
        /// archives, images, audio, video, fonts) are already detected without it. Files written with
        /// <see cref="WithOutput"/> are always byte-exact; this only changes what the result holds.</para>
        /// <para>This is a CurlDotNet option with no curl flag, so <see cref="ToCurlCommand"/> does not print it.</para>
        /// <para>Example: <c>var r = await CurlRequestBuilder.Get("https://example.com/report.xlsx").AsBinary().ExecuteAsync();</c>
        /// then <c>File.WriteAllBytes("report.xlsx", r.BinaryData);</c>.</para>
        /// </remarks>
        /// <returns>This builder, for chaining.</returns>
        /// <example>
        /// <code language="csharp">
        /// var result = await CurlRequestBuilder
        ///     .Get("https://example.com/report.xlsx")
        ///     .AsBinary()
        ///     .WithOutput("report.xlsx")
        ///     .ExecuteAsync();
        /// </code>
        /// </example>
        /// <seealso cref="WithBinaryContentType"/>
        /// <seealso cref="WithTextContentType"/>
        /// <seealso cref="CurlOptions.ForceBinary"/>
        /// <seealso cref="CurlResult.BinaryData"/>
        public CurlRequestBuilder AsBinary()
        {
            _options.ForceBinary = true;
            return this;
        }

        /// <summary>
        /// Registers a MIME type (such as <c>application/x-custom-binary</c>) whose responses are always kept as
        /// bytes in <see cref="CurlResult.BinaryData"/> rather than decoded as text.
        /// </summary>
        /// <remarks>
        /// <para>The library already recognizes common binary types (Office documents, PDFs, archives, images,
        /// audio, video, fonts). Use this for a custom or unusual type your server uses for binary content; the
        /// match is on the media type without parameters, case-insensitively. Call it once per type.</para>
        /// <para>This is a CurlDotNet option with no curl flag, so <see cref="ToCurlCommand"/> does not print it.</para>
        /// <para>Example: <c>CurlRequestBuilder.Get(url).WithBinaryContentType("application/x-protobuf")</c>.</para>
        /// </remarks>
        /// <param name="mimeType">The MIME type to treat as binary (e.g., "application/x-custom-binary").</param>
        /// <returns>This builder, for chaining.</returns>
        /// <example>
        /// <code language="csharp">
        /// var result = await CurlRequestBuilder
        ///     .Get("https://example.com/data")
        ///     .WithBinaryContentType("application/x-custom-format")
        ///     .WithOutput("data.bin")
        ///     .ExecuteAsync();
        /// </code>
        /// </example>
        /// <seealso cref="AsBinary"/>
        /// <seealso cref="WithTextContentType"/>
        /// <seealso cref="CurlOptions.BinaryContentTypes"/>
        public CurlRequestBuilder WithBinaryContentType(string mimeType)
        {
            _options.BinaryContentTypes.Add(mimeType);
            return this;
        }

        /// <summary>
        /// Registers a MIME type (such as <c>application/x-custom-config</c>) whose responses are always decoded as
        /// text into <see cref="CurlResult.Body"/>, even though the library would treat it as binary.
        /// </summary>
        /// <remarks>
        /// <para>Use this for custom types you know contain text; the body is decoded with the response's charset
        /// (UTF-8 when none is given). Call it once per type.</para>
        /// <para>This is a CurlDotNet option with no curl flag, so <see cref="ToCurlCommand"/> does not print it.</para>
        /// <para>Example: <c>var r = await CurlRequestBuilder.Get(url).WithTextContentType("application/x-custom-config").ExecuteAsync();</c>
        /// then <c>Console.WriteLine(r.Body);</c>.</para>
        /// </remarks>
        /// <param name="mimeType">The MIME type to treat as text (e.g., "application/x-custom-text").</param>
        /// <returns>This builder, for chaining.</returns>
        /// <seealso cref="WithBinaryContentType"/>
        /// <seealso cref="AsBinary"/>
        /// <seealso cref="CurlOptions.TextContentTypes"/>
        public CurlRequestBuilder WithTextContentType(string mimeType)
        {
            _options.TextContentTypes.Add(mimeType);
            return this;
        }

        #endregion

        #region Execution

        /// <summary>
        /// Sends the request and blocks the calling thread until the response is complete; the synchronous
        /// form of <see cref="ExecuteAsync(CancellationToken)"/> for console apps, scripts and tests.
        /// </summary>
        /// <remarks>
        /// <para>Prefer <see cref="ExecuteAsync(CancellationToken)"/> in ASP.NET, UI and other async code: blocking
        /// a thread pool or UI thread on network I/O wastes threads and can freeze the UI.</para>
        /// <para>Example: <c>var result = CurlRequestBuilder.Get("https://api.example.com/health").Execute(); Console.WriteLine(result.StatusCode);</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-synchronously-in-csharp/README.md</para>
        /// </remarks>
        /// <returns>The <see cref="CurlResult"/>: <see cref="CurlResult.StatusCode"/>, <see cref="CurlResult.Headers"/>,
        /// <see cref="CurlResult.Body"/> (text) or <see cref="CurlResult.BinaryData"/> (binary), saved paths in
        /// <see cref="CurlResult.OutputFiles"/> and <see cref="CurlResult.Timings"/>. HTTP error statuses are returned
        /// as results unless <see cref="FailOnError"/> was used.</returns>
        /// <exception cref="CurlException">The transfer failed. The subtype and <see cref="CurlException.CurlErrorCode"/>
        /// say why, for example <see cref="CurlCouldntResolveHostException"/> (6, wrong host name),
        /// <see cref="CurlCouldntConnectException"/> (7, server down or port closed),
        /// <see cref="CurlOperationTimeoutException"/> (28, raise <see cref="WithTimeout"/>),
        /// <see cref="CurlPeerFailedVerificationException"/> (60, untrusted certificate) or
        /// <see cref="CurlHttpReturnedErrorException"/> (22, with <see cref="FailOnError"/>). Each message says what
        /// happened, how to fix it and links the page; see
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="ExecuteAsync(CancellationToken)"/>
        /// <seealso cref="Curl.Execute(string)"/>
        public CurlResult Execute()
        {
            return ExecuteAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sends the request asynchronously and returns the complete response; pass a
        /// <see cref="CancellationToken"/> to abandon it (for example when the HTTP caller disconnects).
        /// </summary>
        /// <remarks>
        /// <para>The builder is turned into <see cref="CurlOptions"/> and run by the same engine as
        /// <see cref="Curl.ExecuteAsync(string)"/>; <see cref="CurlResult.Command"/> holds the equivalent command from
        /// <see cref="ToCurlCommand"/>. The builder can be executed again (each call is a new transfer).</para>
        /// <para>Cancellation: when <paramref name="cancellationToken"/> is cancelled the transfer stops and an
        /// <see cref="OperationCanceledException"/> is thrown (use <see cref="WithTimeout"/> for time limits, which
        /// throw <see cref="CurlOperationTimeoutException"/> instead).</para>
        /// <para>Example: <c>var result = await CurlRequestBuilder.Get("https://api.example.com/orders").WithBearerToken(token).ExecuteAsync(HttpContext.RequestAborted);</c>
        /// then <c>if (result.IsSuccess) orders = result.ParseJson&lt;List&lt;Order&gt;&gt;();</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</para>
        /// </remarks>
        /// <param name="cancellationToken">Token that cancels the transfer; <c>default</c> means it cannot be cancelled.</param>
        /// <returns>A task with the <see cref="CurlResult"/>: status code, headers, body or binary data, output files
        /// and timings. HTTP error statuses (404, 500) are results, not exceptions, unless <see cref="FailOnError"/> was used.</returns>
        /// <exception cref="CurlException">The transfer failed (DNS 6, connect 7, timeout 28, TLS 35/60, write 23,
        /// HTTP error with <see cref="FailOnError"/> 22, ...). The subtype and <see cref="CurlException.CurlErrorCode"/>
        /// identify the cause; the message says what happened and how to fix it; see
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled before the
        /// response completed. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</exception>
        /// <example>
        /// <code language="csharp">
        /// try
        /// {
        ///     var result = await CurlRequestBuilder.Get("https://api.example.com/orders").ExecuteAsync(token);
        ///     result.EnsureSuccess();
        /// }
        /// catch (CurlCouldntResolveHostException ex)
        /// {
        ///     Console.WriteLine(ex.Message); // what happened, how to fix it, and a documentation link
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="Execute"/>
        /// <seealso cref="ExecuteAsync(CurlSettings, CancellationToken)"/>
        /// <seealso cref="Curl.ExecuteAsync(CurlOptions, CancellationToken)"/>
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
        /// Sends the request with extra per-call <see cref="CurlSettings"/> (progress callback, retries, timeouts,
        /// headers, redirects, TLS) that override the builder for this call only.
        /// </summary>
        /// <remarks>
        /// <para>Applied from <paramref name="settings"/>: <c>MaxTimeSeconds</c> (like <see cref="WithTimeout"/>),
        /// <c>ConnectTimeoutSeconds</c>, <c>FollowRedirects</c>, <c>Insecure</c>, <c>Headers</c> (added or replacing by
        /// name), <c>OnProgress</c> (called for every chunk of the body with percent or -1, bytes received and
        /// total bytes or -1) and <c>RetryCount</c>/<c>RetryDelayMs</c> (retry any failure). Settings that are not set
        /// leave the builder's values alone. After the call the builder is restored, so the same builder can be
        /// executed again without the settings.</para>
        /// <para>Cancellation: <paramref name="cancellationToken"/> wins when it can be cancelled; otherwise
        /// <c>settings.CancellationToken</c> is used.</para>
        /// <para>Example, a download with a progress bar: <c>await CurlRequestBuilder.Get("https://example.com/big.zip").SaveToFile("big.zip").ExecuteAsync(new CurlSettings().WithProgress((pct, got, total) =&gt; Console.Write($"\r{pct:F0}%")).WithRetries(3));</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <param name="settings">Per-call settings; null behaves like <see cref="ExecuteAsync(CancellationToken)"/>.</param>
        /// <param name="cancellationToken">Token that cancels the transfer; when it cannot be cancelled (<c>default</c>),
        /// <c>settings.CancellationToken</c> is used.</param>
        /// <returns>A task with the <see cref="CurlResult"/>, as for <see cref="ExecuteAsync(CancellationToken)"/>.</returns>
        /// <exception cref="CurlException">The transfer failed (after the configured retries); the subtype and
        /// <see cref="CurlException.CurlErrorCode"/> say why. A progress callback that throws ends the transfer with
        /// <see cref="CurlAbortedByCallbackException"/> (exit code 42) carrying your exception as InnerException. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <exception cref="OperationCanceledException">The effective token was cancelled. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</exception>
        /// <seealso cref="ExecuteAsync(CancellationToken)"/>
        /// <seealso cref="CurlSettings"/>
        /// <seealso cref="CurlSettings.WithProgress"/>
        /// <seealso cref="Curl.ExecuteAsync(string, CurlSettings)"/>
        public async Task<CurlResult> ExecuteAsync(CurlSettings settings, CancellationToken cancellationToken = default)
        {
            var engine = new CurlEngine();
            // The settings apply to this call only: remember what they may change and put it back afterwards
            var saved = (_options.MaxTime, _options.ConnectTimeout, _options.FollowLocation, _options.Insecure,
                new Dictionary<string, string>(_options.Headers, StringComparer.OrdinalIgnoreCase), _options.ProgressHandler,
                _options.Retry, _options.RetryDelay, _options.RetryAllErrors);
            try
            {
                _options.OriginalCommand = ToCurlCommand();
                if (settings != null)
                {
                    CurlEngine.ApplySettings(_options, settings);
                }
                var token = cancellationToken.CanBeCanceled ? cancellationToken : (settings?.CancellationToken ?? CancellationToken.None);
                return await engine.ExecuteAsync(_options, token);
            }
            finally
            {
                (_options.MaxTime, _options.ConnectTimeout, _options.FollowLocation, _options.Insecure,
                    _options.Headers, _options.ProgressHandler,
                    _options.Retry, _options.RetryDelay, _options.RetryAllErrors) = saved;
                engine.Dispose();
            }
        }

        /// <summary>
        /// Returns the curl command line equivalent to this builder, such as
        /// <c>curl -X POST -H 'Content-Type: application/json' -d '{"a":1}' https://api.example.com/items</c>,
        /// for logging, bug reports or running the same request in a shell.
        /// </summary>
        /// <remarks>
        /// <para>Values are quoted for a POSIX shell (bash, zsh) with single quotes, so the string can be pasted into
        /// a terminal or back into <see cref="Curl.ExecuteAsync(string)"/>, which parses it into the same request.
        /// Flags printed: <c>-X</c> (non-GET methods), <c>-H</c>, <c>-A</c>, <c>-e</c>, <c>-d</c>, <c>--data-binary</c>
        /// (only for bytes that are plain UTF-8 text), <c>-F</c>, <c>-u</c>, <c>-L</c>, <c>-k</c>, <c>-v</c>,
        /// <c>-s</c>, <c>-f</c>, <c>-i</c>, <c>--compressed</c>, <c>--max-time</c>, <c>--connect-timeout</c>,
        /// <c>--max-redirs</c> (when not 50), <c>--http1.0</c>/<c>--http1.1</c>/<c>--http2</c>/<c>--http3</c>,
        /// <c>-r</c>, <c>-x</c>, <c>--proxy-user</c>, <c>-b</c>, <c>-c</c>, <c>-o</c>, <c>-O</c>, <c>--trace</c>,
        /// <c>--trace-ascii</c>, <c>--trace-time</c>, then the URL.</para>
        /// <para>Not printed, because curl has no flag for them: real binary bodies from <see cref="WithBinaryData"/>,
        /// <see cref="AsBinary"/>, <see cref="WithBinaryContentType"/> and <see cref="WithTextContentType"/>.</para>
        /// <para>Security: passwords from <see cref="WithBasicAuth"/> and <see cref="WithProxy(string, string, string)"/>
        /// and tokens in headers appear in clear text; do not write this string to shared logs.</para>
        /// <para>Example: <c>Console.WriteLine(CurlRequestBuilder.Get("https://example.com").WithHeader("Accept", "text/html").FollowRedirects().ToCurlCommand());</c>
        /// prints <c>curl -H 'Accept: text/html' -L https://example.com</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md</para>
        /// </remarks>
        /// <returns>A single-line command starting with <c>curl</c> and ending with the URL.</returns>
        /// <seealso cref="Curl.ExecuteAsync(string)"/>
        /// <seealso cref="CurlResult.Command"/>
        /// <seealso cref="GetOptions"/>
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
                parts.Add("-H " + CommandParser.ShellQuote($"{header.Key}: {header.Value}"));
            }

            // User-Agent
            if (!string.IsNullOrEmpty(_options.UserAgent))
            {
                parts.Add("-A " + CommandParser.ShellQuote(_options.UserAgent));
            }

            // Referer
            if (!string.IsNullOrEmpty(_options.Referer))
            {
                parts.Add("-e " + CommandParser.ShellQuote(_options.Referer));
            }

            // Data
            if (!string.IsNullOrEmpty(_options.Data))
            {
                parts.Add("-d " + CommandParser.ShellQuote(_options.Data));
            }

            // WithBinaryData: only bytes that are plain text can be written on a command line
            var binaryText = BinaryDataAsText(_options.BinaryData);
            if (binaryText != null)
            {
                parts.Add("--data-binary " + CommandParser.ShellQuote(binaryText));
            }

            // Form data
            if (_options.FormData != null && _options.FormData.Count > 0)
            {
                foreach (var field in _options.FormData)
                {
                    parts.Add("-F " + CommandParser.ShellQuote($"{field.Key}={field.Value}"));
                }
            }

            // Files
            if (_options.Files != null && _options.Files.Count > 0)
            {
                foreach (var file in _options.Files)
                {
                    parts.Add("-F " + CommandParser.ShellQuote($"{file.Key}=@{file.Value}"));
                }
            }

            // Authentication
            if (_options.Credentials != null)
            {
                parts.Add("-u " + CommandParser.ShellQuote($"{_options.Credentials.UserName}:{_options.Credentials.Password}"));
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
                parts.Add($"--max-time {_options.MaxTime.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            if (_options.ConnectTimeout.HasValue && _options.ConnectTimeout.Value > 0)
                parts.Add($"--connect-timeout {_options.ConnectTimeout.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            if (_options.MaxRedirects > 0 && _options.MaxRedirects != 50)
                parts.Add($"--max-redirs {_options.MaxRedirects}");

            var httpVersionFlag = HttpVersionFlag(_options.HttpVersion);
            if (httpVersionFlag != null)
                parts.Add(httpVersionFlag);
            if (!string.IsNullOrEmpty(_options.Range))
                parts.Add("-r " + CommandParser.ShellQuoteIfNeeded(_options.Range));

            if (!string.IsNullOrEmpty(_options.Proxy))
                parts.Add("-x " + CommandParser.ShellQuoteIfNeeded(_options.Proxy));
            if (_options.ProxyCredentials != null)
                parts.Add("--proxy-user " + CommandParser.ShellQuote($"{_options.ProxyCredentials.UserName}:{_options.ProxyCredentials.Password}"));

            if (!string.IsNullOrEmpty(_options.Cookie))
                parts.Add("-b " + CommandParser.ShellQuote(_options.Cookie));
            if (!string.IsNullOrEmpty(_options.CookieJar))
                parts.Add("-c " + CommandParser.ShellQuoteIfNeeded(_options.CookieJar));

            if (!string.IsNullOrEmpty(_options.OutputFile))
                parts.Add("-o " + CommandParser.ShellQuoteIfNeeded(_options.OutputFile));
            if (_options.UseRemoteFileName)
                parts.Add("-O");

            if (!string.IsNullOrEmpty(_options.TraceFile))
                parts.Add("--trace " + CommandParser.ShellQuoteIfNeeded(_options.TraceFile));
            if (!string.IsNullOrEmpty(_options.TraceAsciiFile))
                parts.Add("--trace-ascii " + CommandParser.ShellQuoteIfNeeded(_options.TraceAsciiFile));
            if (_options.TraceTime)
                parts.Add("--trace-time");

            // URL goes last
            parts.Add(CommandParser.ShellQuoteIfNeeded(_options.Url));

            return string.Join(" ", parts);
        }

        /// <summary>The curl flag for a <see cref="WithHttpVersion"/> value, or null for an unknown value.</summary>
        private static string? HttpVersionFlag(string? version)
        {
            switch (version?.Trim())
            {
                case "1":
                case "1.0": return "--http1.0";
                case "1.1": return "--http1.1";
                case "2":
                case "2.0": return "--http2";
                case "3":
                case "3.0": return "--http3";
                default: return null;
            }
        }

        /// <summary>
        /// The bytes of <see cref="WithBinaryData"/> as text when they are valid UTF-8 without control
        /// characters (other than tab, CR and LF); null for real binary data or no data.
        /// </summary>
        private static string? BinaryDataAsText(byte[]? data)
        {
            if (data == null || data.Length == 0)
            {
                return null;
            }
            try
            {
                var text = new System.Text.UTF8Encoding(false, true).GetString(data);
                return text.Any(c => char.IsControl(c) && c != '\t' && c != '\r' && c != '\n') ? null : text;
            }
            catch (System.Text.DecoderFallbackException)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a copy of the <see cref="CurlOptions"/> this builder has assembled, to inspect it in tests or
        /// to tweak options that have no builder method and run it with <see cref="Curl.ExecuteAsync(CurlOptions)"/>.
        /// </summary>
        /// <remarks>
        /// <para>The copy is independent: changing it does not change the builder. Every property set by the
        /// builder methods is copied (URL, method, headers, body, form fields and files, credentials, timeouts,
        /// redirects, proxy, cookies, output, trace, HTTP version, range and binary/text type lists).</para>
        /// <para>Example: <c>var options = CurlRequestBuilder.Get(url).WithBearerToken(t).GetOptions(); options.Retry = 3; var result = await Curl.ExecuteAsync(options);</c></para>
        /// </remarks>
        /// <returns>A new <see cref="CurlOptions"/> with this builder's settings.</returns>
        /// <seealso cref="CurlOptions"/>
        /// <seealso cref="Curl.ExecuteAsync(CurlOptions)"/>
        /// <seealso cref="ToCurlCommand"/>
        public CurlOptions GetOptions() => _options.Clone();

        #endregion
    }
}
