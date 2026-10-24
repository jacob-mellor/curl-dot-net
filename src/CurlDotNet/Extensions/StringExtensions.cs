using System;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Extensions
{
    /// <summary>
    /// Run curl straight from a string: <c>await "curl https://api.github.com".CurlAsync()</c>,
    /// <c>await "https://example.com".CurlBodyAsync()</c>, <c>"curl ...".Curl()</c> for synchronous code.
    /// </summary>
    /// <remarks>
    /// <para>Add <c>using CurlDotNet.Extensions;</c> to see these on every <see cref="string"/>. They are thin wrappers over
    /// <see cref="CurlDotNet.Curl"/>: same parsing, same results, same exceptions. The command methods (<see cref="CurlAsync(string)"/>,
    /// <see cref="Curl(string)"/>) take a full command line (the leading <c>curl</c> is optional); the URL methods
    /// (<see cref="CurlGetAsync(string)"/>, <see cref="CurlPostJsonAsync(string, string)"/>, <see cref="CurlDownloadAsync(string, string)"/>,
    /// <see cref="CurlBodyAsync(string)"/>) take a URL.</para>
    /// <para>Example: <c>string zen = await "https://api.github.com/zen".CurlBodyAsync();</c></para>
    /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlDotNet.Curl"/>
    /// <seealso cref="FluentExtensions"/>
    public static class StringExtensions
    {
        /// <summary>
        /// Runs this string as a curl command line (with or without the leading <c>curl</c>) and returns the result - the
        /// same as <see cref="CurlDotNet.Curl.ExecuteAsync(string)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Any curl option works: <c>-X</c>, <c>-H</c>, <c>-d</c>, <c>-u</c>, <c>-L</c>, <c>-o</c>... The string is read with
        /// <see cref="CurlDotNet.Curl.DefaultFlavor"/> shell rules (bash, cmd or PowerShell quoting, detected automatically).
        /// HTTP error statuses are returned in the result; use <c>-f</c> to turn them into exceptions.</para>
        /// <para>Example: <c>var result = await "curl -s -H 'Accept: application/json' https://api.github.com/users/octocat".CurlAsync();</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md</para>
        /// </remarks>
        /// <param name="command">A curl command line such as <c>curl -L https://example.com</c>, as you would type it in a shell.</param>
        /// <returns>A task with the <see cref="CurlResult"/> (status, body, headers, files written).</returns>
        /// <exception cref="CurlException">The command is invalid (<see cref="CurlInvalidCommandException"/>) or the transfer
        /// failed (DNS, connect, timeout, TLS, <c>-f</c> HTTP error); every subclass message says what happened and how to fix it.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="CurlAsync(string, CancellationToken)"/>
        /// <seealso cref="Curl(string)"/>
        /// <seealso cref="CurlDotNet.Curl.ExecuteAsync(string)"/>
        public static async Task<CurlResult> CurlAsync(this string command)
        {
            return await CurlDotNet.Curl.ExecuteAsync(command).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs this string as a curl command line and stops the transfer when <paramref name="cancellationToken"/> is
        /// cancelled - the same as <see cref="CurlDotNet.Curl.ExecuteAsync(string, CancellationToken)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Cancellation throws <see cref="OperationCanceledException"/>. For a curl-style time limit (exit code 28)
        /// put <c>-m SECONDS</c> in the command instead.</para>
        /// <para>Example: <c>using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)); var r = await "curl https://slow.example.com".CurlAsync(cts.Token);</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</para>
        /// </remarks>
        /// <param name="command">A curl command line such as <c>curl -X POST -d a=1 https://example.com</c>.</param>
        /// <param name="cancellationToken">Token that aborts the transfer, for example <c>HttpContext.RequestAborted</c>.</param>
        /// <returns>A task with the <see cref="CurlResult"/>.</returns>
        /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was cancelled before the transfer finished.</exception>
        /// <exception cref="CurlException">The command is invalid or the transfer failed; see the message for the fix.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="CurlAsync(string)"/>
        /// <seealso cref="CurlDotNet.Curl.ExecuteAsync(string, CancellationToken)"/>
        public static async Task<CurlResult> CurlAsync(this string command, CancellationToken cancellationToken)
        {
            return await CurlDotNet.Curl.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a GET request to this URL (<c>curl URL</c>) and returns the result; a string that already starts with
        /// <c>"curl "</c> is run as a command.
        /// </summary>
        /// <remarks>
        /// <para>The URL is placed after <c>curl </c> and read as a command line, so quote a URL that contains <c>&amp;</c>,
        /// <c>;</c>, <c>|</c>, spaces or quotes (<c>"'https://x.example/?a=1&amp;b=2'".CurlGetAsync()</c>), or use
        /// <see cref="CurlDotNet.Curl.GetAsync(string)"/>, which takes the URL literally. Redirects are not followed unless
        /// you write <c>curl -L URL</c>.</para>
        /// <para>Example: <c>var result = await "https://api.github.com/users/octocat".CurlGetAsync(); var user = result.ParseJson&lt;User&gt;();</c></para>
        /// </remarks>
        /// <param name="url">An absolute URL such as <c>https://api.example.com/items</c>, or a full <c>curl ...</c> command.</param>
        /// <returns>A task with the <see cref="CurlResult"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> is null.</exception>
        /// <exception cref="CurlException">The URL is malformed or the transfer failed; see the message for the fix.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="CurlBodyAsync(string)"/>
        /// <seealso cref="CurlDotNet.Curl.GetAsync(string)"/>
        public static async Task<CurlResult> CurlGetAsync(this string url)
        {
            RequireUrl(url, nameof(CurlGetAsync));
            var command = url.StartsWith("curl ") ? url : $"curl {url}";
            return await CurlDotNet.Curl.ExecuteAsync(command).ConfigureAwait(false);
        }

        private static void RequireUrl(string url, string method)
        {
            if (url == null)
            {
                throw new ArgumentNullException(nameof(url), CurlException.Compose(
                    $"{method}() was called on a null string.",
                    "Call it on the URL you want, for example \"https://api.example.com/items\"." + method + "().",
                    DocLinks.HowToRunCommand));
            }
        }

        /// <summary>
        /// POSTs <paramref name="json"/> to this URL with <c>Content-Type: application/json</c> and returns the result - like
        /// <c>curl -X POST -H 'Content-Type: application/json' -d JSON URL</c>, without any shell quoting.
        /// </summary>
        /// <remarks>
        /// <para>The JSON text is sent exactly as given (it is not validated or re-serialized), so apostrophes, spaces and
        /// newlines need no escaping. To send an object instead of text, use <see cref="CurlDotNet.Curl.PostJsonAsync(string, object)"/>
        /// or <see cref="CurlRequestBuilder.WithJson(object)"/>. The URL is used literally.</para>
        /// <para>Example: <c>var r = await "https://api.example.com/users".CurlPostJsonAsync("{\"name\":\"O'Brien\"}"); r.EnsureSuccess();</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">The absolute URL to POST to, such as <c>https://api.example.com/users</c>.</param>
        /// <param name="json">The request body as JSON text, such as <c>{"name":"John"}</c>; null sends an empty body.</param>
        /// <returns>A task with the <see cref="CurlResult"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> is null.</exception>
        /// <exception cref="CurlException">The URL is malformed or the transfer failed; see the message for the fix.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="CurlDotNet.Curl.PostJsonAsync(string, object)"/>
        /// <seealso cref="CurlGetAsync(string)"/>
        public static async Task<CurlResult> CurlPostJsonAsync(this string url, string json)
        {
            RequireUrl(url, nameof(CurlPostJsonAsync));
            // Build options directly so JSON containing apostrophes or spaces needs no shell quoting
            var options = new CurlDotNet.Core.CurlOptions
            {
                Url = url,
                Method = "POST",
                Data = json ?? string.Empty,
                OriginalCommand = $"curl -X POST -H 'Content-Type: application/json' -d {CurlDotNet.Core.CommandParser.ShellQuote(json)} {CurlDotNet.Core.CommandParser.ShellQuoteIfNeeded(url)}"
            };
            options.Headers["Content-Type"] = "application/json";
            return await CurlDotNet.Curl.ExecuteAsync(options).ConfigureAwait(false);
        }

        /// <summary>
        /// Downloads this URL to <paramref name="outputFile"/> (like <c>curl -o FILE URL</c>) and returns the result - the same as
        /// <see cref="CurlDotNet.Curl.DownloadAsync(string, string)"/>.
        /// </summary>
        /// <remarks>
        /// <para>The file is written as the bytes arrive and is listed in <see cref="CurlResult.OutputFiles"/>; the folder must
        /// exist. An HTTP error page (404, 500) is saved like any other body unless you check the status - use
        /// <c>"curl -f -o FILE URL".CurlAsync()</c> to fail instead.</para>
        /// <para>Example: <c>await "https://example.com/files/report.pdf".CurlDownloadAsync("report.pdf");</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">The absolute URL of the file, such as <c>https://example.com/file.zip</c>.</param>
        /// <param name="outputFile">The local path to write, such as <c>downloads/file.zip</c>; must not be empty.</param>
        /// <returns>A task with the <see cref="CurlResult"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="outputFile"/> is null or empty.</exception>
        /// <exception cref="CurlWriteErrorException">The file could not be created (folder missing, no permission); exit code 23.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</exception>
        /// <exception cref="CurlException">The transfer failed (DNS, connect, timeout, TLS); see the message for the fix.</exception>
        /// <seealso cref="CurlDotNet.Curl.DownloadAsync(string, string)"/>
        /// <seealso cref="CurlResult.SaveToFile(string)"/>
        public static async Task<CurlResult> CurlDownloadAsync(this string url, string outputFile)
        {
            return await CurlDotNet.Curl.DownloadAsync(url, outputFile).ConfigureAwait(false);
        }

        /// <summary>
        /// Runs this string as a curl command line and blocks until it finishes - the synchronous form of
        /// <see cref="CurlAsync(string)"/>, for console tools and scripts.
        /// </summary>
        /// <remarks>
        /// <para>Blocking: CurlDotNet awaits with <c>ConfigureAwait(false)</c> throughout, so this call does not deadlock on a UI
        /// or classic ASP.NET <see cref="System.Threading.SynchronizationContext"/> by itself. It still occupies the calling
        /// thread until the transfer ends (a frozen window, fewer free threads under load), and a callback you supply - a
        /// progress or redirect handler, middleware, or your own <c>HttpMessageHandler</c> - that waits for that same context
        /// can still deadlock. Prefer the async method wherever the caller can await.</para>
        /// <para>Blocks the calling thread; in ASP.NET, UI or other async code use <see cref="CurlAsync(string)"/> to avoid
        /// thread-pool starvation.</para>
        /// <para>Example: <c>var result = "curl -s https://api.github.com/zen".Curl(); Console.WriteLine(result.Body);</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-synchronously-in-csharp/README.md</para>
        /// </remarks>
        /// <param name="command">A curl command line such as <c>curl -I https://example.com</c>.</param>
        /// <returns>The <see cref="CurlResult"/>.</returns>
        /// <exception cref="CurlException">The command is invalid or the transfer failed; see the message for the fix.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="CurlAsync(string)"/>
        /// <seealso cref="CurlDotNet.Curl.Execute(string)"/>
        public static CurlResult Curl(this string command)
        {
            return CurlAsync(command).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Fetches this URL with GET and returns only the response text - the shortest way to read a page or a JSON
        /// document into a string.
        /// </summary>
        /// <remarks>
        /// <para>Equivalent to <c>(await url.CurlGetAsync()).Body</c>, with the same quoting rules as
        /// <see cref="CurlGetAsync(string)"/>. The status is not checked: a 404 returns the error page text. Binary responses
        /// return an empty string (use <see cref="CurlGetAsync(string)"/> and <see cref="CurlResult.BinaryData"/>).</para>
        /// <para>Example: <c>string json = await "https://api.github.com/users/octocat".CurlBodyAsync();</c></para>
        /// </remarks>
        /// <param name="url">An absolute URL such as <c>https://example.com/data.json</c>, or a full <c>curl ...</c> command.</param>
        /// <returns>A task with <see cref="CurlResult.Body"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> is null.</exception>
        /// <exception cref="CurlException">The URL is malformed or the transfer failed; see the message for the fix.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="CurlGetAsync(string)"/>
        /// <seealso cref="CurlResult.Body"/>
        public static async Task<string> CurlBodyAsync(this string url)
        {
            var result = await url.CurlGetAsync().ConfigureAwait(false);
            return result.Body;
        }
    }

    /// <summary>
    /// Start a <see cref="CurlRequestBuilder"/> from a URL string: <c>"https://api.example.com".ToCurl()</c>,
    /// <c>.WithHeader(...)</c> or <c>.WithMethod("DELETE")</c>, then chain and <c>ExecuteAsync()</c>.
    /// </summary>
    /// <remarks>
    /// <para>Add <c>using CurlDotNet.Extensions;</c>. Each method returns a new builder; everything else (data, auth, timeouts)
    /// is set on that builder.</para>
    /// <para>Example: <c>var r = await "https://api.example.com/items".WithHeader("Authorization", "Bearer " + token).WithTimeout(TimeSpan.FromSeconds(10)).ExecuteAsync();</c></para>
    /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlRequestBuilder"/>
    /// <seealso cref="StringExtensions"/>
    public static class FluentExtensions
    {
        /// <summary>
        /// Creates a GET <see cref="CurlRequestBuilder"/> for this URL so you can chain headers, data and options before
        /// <c>ExecuteAsync()</c>.
        /// </summary>
        /// <remarks>
        /// <para>Same as <see cref="CurlRequestBuilder.Get(string)"/>; calling <c>WithData</c>/<c>WithJson</c> later switches the
        /// method to POST.</para>
        /// <para>Example: <c>var r = await "https://api.example.com/items".ToCurl().WithJson(new { name = "x" }).ExecuteAsync();</c></para>
        /// </remarks>
        /// <param name="url">The absolute URL, such as <c>https://api.example.com/items</c>.</param>
        /// <returns>A new builder for a GET to <paramref name="url"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> is null.</exception>
        /// <seealso cref="CurlRequestBuilder.Get(string)"/>
        /// <seealso cref="WithMethod(string, string)"/>
        public static CurlRequestBuilder ToCurl(this string url)
        {
            return CurlRequestBuilder.Get(url);
        }

        /// <summary>
        /// Creates a GET <see cref="CurlRequestBuilder"/> for this URL with one header already set (like <c>-H "key: value"</c>),
        /// ready for more chaining.
        /// </summary>
        /// <remarks>
        /// <para>Shorthand for <c>CurlRequestBuilder.Get(url).WithHeader(key, value)</c>.</para>
        /// <para>Example: <c>var r = await "https://api.github.com/user".WithHeader("Authorization", "Bearer " + token).ExecuteAsync();</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">The absolute URL, such as <c>https://api.example.com/items</c>.</param>
        /// <param name="key">Header name without the colon, such as <c>Accept</c>.</param>
        /// <param name="value">Header value, such as <c>application/json</c>.</param>
        /// <returns>A new builder for a GET to <paramref name="url"/> with the header set.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> is null.</exception>
        /// <seealso cref="CurlRequestBuilder.WithHeader(string, string)"/>
        /// <seealso cref="ToCurl(string)"/>
        public static CurlRequestBuilder WithHeader(this string url, string key, string value)
        {
            return CurlRequestBuilder.Get(url).WithHeader(key, value);
        }

        /// <summary>
        /// Creates a <see cref="CurlRequestBuilder"/> for this URL with the HTTP method you name (like <c>-X METHOD</c>), such as
        /// DELETE, PUT, PATCH or OPTIONS.
        /// </summary>
        /// <remarks>
        /// <para>Same as <see cref="CurlRequestBuilder.Request(string, string)"/>; the method is upper-cased.</para>
        /// <para>Example: <c>var r = await "https://api.example.com/items/42".WithMethod("DELETE").ExecuteAsync();</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">The absolute URL, such as <c>https://api.example.com/items/42</c>.</param>
        /// <param name="method">The HTTP method name, in any case, such as <c>delete</c> or <c>PATCH</c>.</param>
        /// <returns>A new builder for <paramref name="method"/> to <paramref name="url"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="url"/> is null.</exception>
        /// <seealso cref="CurlRequestBuilder.Request(string, string)"/>
        /// <seealso cref="ToCurl(string)"/>
        public static CurlRequestBuilder WithMethod(this string url, string method)
        {
            return CurlRequestBuilder.Request(method, url);
        }
    }
}
