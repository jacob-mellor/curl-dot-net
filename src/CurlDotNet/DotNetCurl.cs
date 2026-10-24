/***************************************************************************
 * DotNetCurl - Alternative API name for Curl
 *
 * Provides DotNetCurl.Curl() and DotNetCurl.CurlAsync() as specified
 * in the architecture. This is an alias to the main Curl class.
 *
 * The killer feature: Copy curl commands from anywhere and they just work!
 *
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
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

namespace CurlDotNet
{
    /// <summary>
    /// The same API as <see cref="global::CurlDotNet.Curl"/> under different names (<c>DotNetCurl.Curl(command)</c>,
    /// <c>DotNetCurl.CurlAsync(command)</c>), for code that prefers to read "curl this command".
    /// </summary>
    /// <remarks>
    /// <para>Every member forwards to the matching <see cref="global::CurlDotNet.Curl"/> member, so behaviour, results,
    /// exceptions and the process-wide <c>Default*</c> settings are shared: setting
    /// <see cref="DefaultMaxTimeSeconds"/> here is the same as setting <see cref="global::CurlDotNet.Curl.DefaultMaxTimeSeconds"/>.
    /// Pick one class and use it consistently.</para>
    /// <para>Name mapping: <see cref="Curl(string)"/> = <see cref="global::CurlDotNet.Curl.Execute(string)"/>,
    /// <see cref="CurlAsync(string)"/> = <see cref="global::CurlDotNet.Curl.ExecuteAsync(string)"/>,
    /// <see cref="CurlManyAsync(string[])"/> = <see cref="global::CurlDotNet.Curl.ExecuteManyAsync(string[])"/>,
    /// <see cref="ToPython(string)"/> = <see cref="global::CurlDotNet.Curl.ToPythonRequests(string)"/>; the others have
    /// the same names.</para>
    /// <para>All members are static and thread-safe.</para>
    /// <para>Example: <c>var response = DotNetCurl.Curl("curl https://api.github.com/users/octocat");</c> (blocking) or
    /// <c>var response = await DotNetCurl.CurlAsync("curl https://api.github.com/users/octocat");</c>.</para>
    /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md</para>
    /// <para>Sponsored by IronSoftware - creators of IronPDF, IronOCR, IronXL, and more.</para>
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// // Synchronous execution
    /// var response = DotNetCurl.Curl("curl https://api.github.com/users/octocat");
    /// Console.WriteLine(response.Body);
    ///
    /// // Async execution
    /// var response = await DotNetCurl.CurlAsync("curl https://api.github.com/users/octocat");
    ///
    /// // POST request
    /// var result = DotNetCurl.Curl(@"
    ///     curl -X POST https://api.example.com/users \
    ///     -H 'Content-Type: application/json' \
    ///     -d '{""name"":""John"",""email"":""john@example.com""}'
    /// ");
    ///
    /// // Multiple commands in parallel
    /// var results = await DotNetCurl.CurlManyAsync(
    ///     "curl https://api.example.com/users",
    ///     "curl https://api.example.com/posts");
    /// </code>
    /// </example>
    /// <seealso cref="global::CurlDotNet.Curl"/>
    /// <seealso cref="CurlRequestBuilder"/>
    /// <seealso cref="Lib.LibCurl"/>
    public static class DotNetCurl
    {
        /// <summary>
        /// Runs a curl command and blocks the calling thread until the response arrives; same as
        /// <see cref="global::CurlDotNet.Curl.Execute(string)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Paste any curl command, with or without the leading <c>curl</c>; it is read with
        /// <see cref="global::CurlDotNet.Curl.DefaultFlavor"/>. HTTP error statuses are returned in the result, not
        /// thrown (add <c>-f</c> to throw). Blocks the thread: do not call it on a UI thread; use
        /// <see cref="CurlAsync(string)"/> in async code. Exceptions are thrown directly, not wrapped in
        /// <see cref="AggregateException"/>.</para>
        /// <para>Example: <c>var data = DotNetCurl.Curl("https://api.example.com/data");</c> then <c>Console.WriteLine(data.Body);</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-synchronously-in-csharp/README.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// var response = DotNetCurl.Curl("curl https://api.example.com/data");
        /// Console.WriteLine(response.Body);
        ///
        /// // Works without "curl" prefix
        /// var data = DotNetCurl.Curl("https://api.example.com/data");
        /// </code>
        /// </example>
        /// <param name="command">The curl command line as text, for example <c>curl -H 'Accept: application/json' https://api.example.com</c>.</param>
        /// <returns>The <see cref="CurlResult"/>: status code, body (standard output text), headers, binary data, written
        /// files and timings.</returns>
        /// <exception cref="ArgumentException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlException">The command could not be read or the transfer failed (DNS 6, connect 7,
        /// timeout 28, TLS 35/60, <c>-f</c> HTTP error 22...); see <see cref="global::CurlDotNet.Curl.ExecuteAsync(string)"/>.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="CurlAsync(string)">Async version (preferred)</seealso>
        /// <seealso cref="global::CurlDotNet.Curl.Execute(string)"/>
        public static CurlResult Curl(string command)
        {
            // Synchronous wrapper - waits for async operation
            return CurlAsync(command).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Runs a curl command, blocking the calling thread, and cancels it if it has not finished after
        /// <paramref name="timeoutSeconds"/> seconds.
        /// </summary>
        /// <remarks>
        /// <para>The deadline is a cancellation, not curl's <c>--max-time</c>: when it passes, the call throws
        /// <see cref="OperationCanceledException"/> (not <see cref="CurlTimeoutException"/>). If you want curl's exit code
        /// 28 and message instead, put <c>-m 30</c> in the command or use <see cref="DefaultMaxTimeSeconds"/>.</para>
        /// <para>Values: a positive number of seconds; 0 cancels immediately. Negative values are rejected.</para>
        /// <para>Example: <c>var result = DotNetCurl.Curl("curl https://slow.example.com/report", 30);</c> gives up after 30 seconds.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</para>
        /// </remarks>
        /// <param name="command">The curl command line as text, as for <see cref="Curl(string)"/>.</param>
        /// <param name="timeoutSeconds">Seconds to wait before cancelling, for example <c>30</c>; must be 0 or more.</param>
        /// <returns>The <see cref="CurlResult"/> when the request finished in time.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="timeoutSeconds"/> is negative. Fix: pass the
        /// number of seconds to wait. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</exception>
        /// <exception cref="OperationCanceledException">The request did not finish within <paramref name="timeoutSeconds"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlException">The transfer failed before the deadline; see <see cref="global::CurlDotNet.Curl.ExecuteAsync(string)"/>.</exception>
        /// <seealso cref="CurlAsync(string, CancellationToken)"/>
        /// <seealso cref="global::CurlDotNet.Curl.Execute(string, CancellationToken)"/>
        /// <seealso cref="DefaultMaxTimeSeconds"/>
        public static CurlResult Curl(string command, int timeoutSeconds)
        {
            if (timeoutSeconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeoutSeconds), timeoutSeconds, CurlException.Compose(
                    $"DotNetCurl.Curl(command, timeoutSeconds) was called with timeoutSeconds = {timeoutSeconds}, which is negative.",
                    "Pass the number of seconds to wait (for example 30); to wait without a deadline call DotNetCurl.Curl(command) instead.",
                    DocLinks.HowToTimeout));
            }

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds)))
            {
                try
                {
                    return CurlAsync(command, cts.Token).GetAwaiter().GetResult();
                }
                catch (OperationCanceledException ex) when (cts.IsCancellationRequested)
                {
                    // the caller passed a number of seconds, not a token: say that the deadline expired
                    throw new OperationCanceledException(CurlException.Compose(
                        $"DotNetCurl.Curl(command, {timeoutSeconds}) gave up: the request to {ErrorText.UrlFromCommand(command) ?? "the server"} did not finish within {timeoutSeconds} second(s).",
                        timeoutSeconds == 0
                            ? "A timeout of 0 cancels at once; pass the number of seconds to wait, or call DotNetCurl.Curl(command) to wait without a deadline."
                            : "Pass a larger timeoutSeconds, or put -m SECONDS in the command to get curl's timeout error (CurlOperationTimeoutException, exit code 28) instead.",
                        DocLinks.HowToTimeout), ex, cts.Token);
                }
            }
        }

        /// <summary>
        /// Runs a curl command asynchronously and returns the response; same as
        /// <see cref="global::CurlDotNet.Curl.ExecuteAsync(string)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Paste any curl command, with or without the leading <c>curl</c>. Multi-line commands with <c>\</c>,
        /// <c>^</c> or backtick continuations work. HTTP error statuses are returned, not thrown (add <c>-f</c> to throw).</para>
        /// <para>Example: <c>var result = await DotNetCurl.CurlAsync("curl -X POST -H 'Content-Type: application/json' -d '{\"name\":\"John\"}' https://api.example.com/users");</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// var response = await DotNetCurl.CurlAsync("curl https://api.example.com/data");
        /// Console.WriteLine(response.Body);
        /// </code>
        /// </example>
        /// <param name="command">The curl command line as text, for example <c>curl -s https://api.example.com/users</c>.</param>
        /// <returns>The <see cref="CurlResult"/>: status code, body, headers, binary data, written files and timings.</returns>
        /// <exception cref="ArgumentException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlException">The command could not be read or the transfer failed; see
        /// <see cref="global::CurlDotNet.Curl.ExecuteAsync(string)"/>. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="Curl(string)">Blocking version</seealso>
        /// <seealso cref="global::CurlDotNet.Curl.ExecuteAsync(string)"/>
        public static async Task<CurlResult> CurlAsync(string command)
        {
            return await global::CurlDotNet.Curl.ExecuteAsync(command);
        }

        /// <summary>
        /// Runs a curl command asynchronously and stops it when <paramref name="cancellationToken"/> is cancelled; same
        /// as <see cref="global::CurlDotNet.Curl.ExecuteAsync(string, CancellationToken)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Cancelling throws <see cref="OperationCanceledException"/>; a <c>-m</c>/<c>--max-time</c> limit in the
        /// command throws <see cref="CurlOperationTimeoutException"/> (exit code 28) instead.</para>
        /// <para>Example: <c>using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));</c> then
        /// <c>var response = await DotNetCurl.CurlAsync("curl https://slow-api.example.com", cts.Token);</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        /// var response = await DotNetCurl.CurlAsync("curl https://slow-api.com", cts.Token);
        /// </code>
        /// </example>
        /// <param name="command">The curl command line as text, as for <see cref="CurlAsync(string)"/>.</param>
        /// <param name="cancellationToken">Cancels the request when signalled; <c>CancellationToken.None</c> for no cancellation.</param>
        /// <returns>The <see cref="CurlResult"/>.</returns>
        /// <exception cref="OperationCanceledException">The token was cancelled before the transfer finished.</exception>
        /// <exception cref="ArgumentException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlException">The transfer failed; see <see cref="global::CurlDotNet.Curl.ExecuteAsync(string)"/>.</exception>
        /// <seealso cref="Curl(string, int)">Blocking version with a deadline</seealso>
        /// <seealso cref="global::CurlDotNet.Curl.ExecuteAsync(string, CancellationToken)"/>
        public static async Task<CurlResult> CurlAsync(string command, CancellationToken cancellationToken)
        {
            return await global::CurlDotNet.Curl.ExecuteAsync(command, cancellationToken);
        }

        /// <summary>
        /// Runs a curl command asynchronously with timeouts, retries, headers or a progress callback from a
        /// <see cref="CurlSettings"/> object; same as <see cref="global::CurlDotNet.Curl.ExecuteAsync(string, CurlSettings)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Applied settings: <c>MaxTimeSeconds</c> (<c>-m</c>), <c>ConnectTimeoutSeconds</c>
        /// (<c>--connect-timeout</c>), <c>FollowRedirects</c> (<c>-L</c>), <c>Insecure</c> (<c>-k</c>), <c>Headers</c>
        /// (<c>-H</c>), <c>OnProgress</c>, <c>RetryCount</c>/<c>RetryDelayMs</c>, <c>Flavor</c> and
        /// <c>CancellationToken</c>; see the <c>Curl</c> method for details.</para>
        /// <para>Example: <c>var result = await DotNetCurl.CurlAsync("curl https://api.example.com", new CurlSettings().WithTimeout(10).WithRetries(2));</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</para>
        /// </remarks>
        /// <param name="command">The curl command line as text, as for <see cref="CurlAsync(string)"/>.</param>
        /// <param name="settings">Settings applied on top of the command; null runs the command unchanged.</param>
        /// <returns>The <see cref="CurlResult"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="OperationCanceledException">The settings' cancellation token was cancelled.</exception>
        /// <exception cref="CurlException">The transfer failed after all retries; see
        /// <see cref="global::CurlDotNet.Curl.ExecuteAsync(string, CurlSettings)"/>. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="global::CurlDotNet.Curl.ExecuteAsync(string, CurlSettings)"/>
        /// <seealso cref="global::CurlDotNet.Curl.Execute(string, CurlSettings)">Blocking version</seealso>
        /// <seealso cref="CurlSettings"/>
        public static async Task<CurlResult> CurlAsync(string command, CurlSettings settings)
        {
            return await global::CurlDotNet.Curl.ExecuteAsync(command, settings);
        }

        /// <summary>
        /// Runs several curl commands in parallel and blocks until all have finished; results come back in the same
        /// order as the commands.
        /// </summary>
        /// <remarks>
        /// <para>All commands start at once and share the connection pool. If any command throws (DNS failure, timeout,
        /// <c>-f</c> with an error status...), that exception is thrown after all have finished and no results are
        /// returned; HTTP error statuses without <c>-f</c> are not failures. Blocks the calling thread; use
        /// <see cref="CurlManyAsync(string[])"/> in async code.</para>
        /// <para>Example: <c>var results = DotNetCurl.CurlMany("curl https://api.example.com/users", "curl https://api.example.com/posts");</c>
        /// then <c>results[0].Body</c> and <c>results[1].Body</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/13-parallel-requests.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// var results = DotNetCurl.CurlMany(
        ///     "curl https://api.example.com/users",
        ///     "curl https://api.example.com/posts");
        /// foreach (var result in results)
        ///     Console.WriteLine(result.Body);
        /// </code>
        /// </example>
        /// <param name="commands">The curl command lines to run, as separate arguments or an array; an empty array
        /// returns an empty array.</param>
        /// <returns>One <see cref="CurlResult"/> per command, in the same order.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="commands"/> is null.</exception>
        /// <exception cref="ArgumentException">One of the commands is null, empty or whitespace.</exception>
        /// <exception cref="CurlException">At least one command failed; see <see cref="global::CurlDotNet.Curl.ExecuteManyAsync(string[])"/>.</exception>
        /// <seealso cref="CurlManyAsync(string[])">Async version (preferred)</seealso>
        /// <seealso cref="global::CurlDotNet.Curl.ExecuteManyAsync(string[])"/>
        public static CurlResult[] CurlMany(params string[] commands)
        {
            return CurlManyAsync(commands).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Runs several curl commands in parallel and returns their results in the same order; same as
        /// <see cref="global::CurlDotNet.Curl.ExecuteManyAsync(string[])"/>.
        /// </summary>
        /// <remarks>
        /// <para>All commands start at once with no concurrency limit; batch large lists to avoid HTTP 429 rate limits.
        /// If any command throws, that exception is thrown after all have finished and no results are returned.</para>
        /// <para>Example: <c>var results = await DotNetCurl.CurlManyAsync("curl https://a.example.com", "curl https://b.example.com");</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/13-parallel-requests.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// var results = await DotNetCurl.CurlManyAsync(
        ///     "curl https://api.example.com/users",
        ///     "curl https://api.example.com/posts",
        ///     "curl https://api.example.com/comments");
        /// </code>
        /// </example>
        /// <param name="commands">The curl command lines to run, as separate arguments or an array.</param>
        /// <returns>One <see cref="CurlResult"/> per command, in the same order.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="commands"/> is null.</exception>
        /// <exception cref="ArgumentException">One of the commands is null, empty or whitespace.</exception>
        /// <exception cref="CurlException">At least one command failed. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="CurlMany(string[])">Blocking version</seealso>
        /// <seealso cref="global::CurlDotNet.Curl.ExecuteManyAsync(string[])"/>
        public static async Task<CurlResult[]> CurlManyAsync(params string[] commands)
        {
            return await global::CurlDotNet.Curl.ExecuteManyAsync(commands);
        }

        /// <summary>
        /// Sends a plain GET request to <paramref name="url"/> and blocks until the response arrives; same as
        /// <see cref="global::CurlDotNet.Curl.Get(string)"/> (like <c>curl URL</c>).
        /// </summary>
        /// <remarks>
        /// <para>No headers or options; the <c>Default*</c> settings apply. HTTP errors are returned, not thrown.</para>
        /// <para>Example: <c>var result = DotNetCurl.Get("https://api.example.com/health");</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-synchronously-in-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">An absolute URL including the scheme, for example <c>https://api.example.com/users?page=1</c>.</param>
        /// <returns>The <see cref="CurlResult"/> with status code, body and headers.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> is null, empty or whitespace. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</exception>
        /// <exception cref="CurlException">The transfer failed; see <see cref="global::CurlDotNet.Curl.ExecuteAsync(string)"/>.</exception>
        /// <seealso cref="GetAsync(string)">Async version (preferred)</seealso>
        /// <seealso cref="global::CurlDotNet.Curl.Get(string)"/>
        public static CurlResult Get(string url)
        {
            return GetAsync(url).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sends a plain GET request to <paramref name="url"/> asynchronously; same as
        /// <see cref="global::CurlDotNet.Curl.GetAsync(string)"/> (like <c>curl URL</c>).
        /// </summary>
        /// <remarks>
        /// <para>For headers or authentication use <see cref="CurlAsync(string)"/> with <c>-H</c>/<c>-u</c>, or
        /// <see cref="CurlRequestBuilder.Get(string)"/>.</para>
        /// <para>Example: <c>var user = (await DotNetCurl.GetAsync("https://api.github.com/users/octocat")).ParseJson&lt;GitHubUser&gt;();</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">An absolute URL including the scheme, for example <c>https://api.example.com/users?page=1</c>.</param>
        /// <returns>The <see cref="CurlResult"/> with status code, body and headers.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlException">The transfer failed; see <see cref="global::CurlDotNet.Curl.ExecuteAsync(string)"/>.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="Get(string)">Blocking version</seealso>
        /// <seealso cref="global::CurlDotNet.Curl.GetAsync(string)"/>
        public static async Task<CurlResult> GetAsync(string url)
        {
            return await global::CurlDotNet.Curl.GetAsync(url);
        }

        /// <summary>
        /// Sends a POST request with <paramref name="data"/> as the body and blocks until the response arrives; same as
        /// <see cref="global::CurlDotNet.Curl.Post(string, string)"/> (like <c>curl -d DATA URL</c>).
        /// </summary>
        /// <remarks>
        /// <para>The data is sent exactly as given, with <c>Content-Type: application/x-www-form-urlencoded</c>; use
        /// <see cref="PostJson(string, object)"/> for JSON.</para>
        /// <para>Example: <c>var result = DotNetCurl.Post("https://api.example.com/login", "user=john&amp;pass=secret");</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">An absolute URL to post to, for example <c>https://api.example.com/users</c>.</param>
        /// <param name="data">The request body as text (form fields <c>a=1&amp;b=2</c>, JSON, XML...); null sends an empty body.</param>
        /// <returns>The <see cref="CurlResult"/> with the server's status, headers and body.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlException">The transfer failed; see <see cref="global::CurlDotNet.Curl.ExecuteAsync(string)"/>.</exception>
        /// <seealso cref="PostAsync(string, string)">Async version (preferred)</seealso>
        /// <seealso cref="global::CurlDotNet.Curl.Post(string, string)"/>
        public static CurlResult Post(string url, string data)
        {
            return PostAsync(url, data).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sends a POST request with <paramref name="data"/> as the body asynchronously; same as
        /// <see cref="global::CurlDotNet.Curl.PostAsync(string, string)"/> (like <c>curl -d DATA URL</c>).
        /// </summary>
        /// <remarks>
        /// <para>Quotes, apostrophes and spaces in the data need no escaping; the data is not URL-encoded for you.</para>
        /// <para>Example: <c>var result = await DotNetCurl.PostAsync("https://api.example.com/notes", "It's \"quoted\"");</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">An absolute URL to post to, for example <c>https://api.example.com/users</c>.</param>
        /// <param name="data">The request body as text; null sends an empty body.</param>
        /// <returns>The <see cref="CurlResult"/> with the server's status, headers and body.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlException">The transfer failed. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="Post(string, string)">Blocking version</seealso>
        /// <seealso cref="global::CurlDotNet.Curl.PostAsync(string, string)"/>
        public static async Task<CurlResult> PostAsync(string url, string data)
        {
            return await global::CurlDotNet.Curl.PostAsync(url, data);
        }

        /// <summary>
        /// Serializes <paramref name="data"/> to JSON and POSTs it with <c>Content-Type: application/json</c>, blocking
        /// until the response arrives; same as <see cref="global::CurlDotNet.Curl.PostJson(string, object)"/>.
        /// </summary>
        /// <remarks>
        /// <para>System.Text.Json is used (Newtonsoft.Json on the .NET Standard 2.0 build).</para>
        /// <para>Example: <c>var result = DotNetCurl.PostJson("https://api.example.com/users", new { name = "John" });</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">An absolute URL to post to, for example <c>https://api.example.com/users</c>.</param>
        /// <param name="data">The object to send as JSON: anonymous object, your class, array/list or dictionary.</param>
        /// <returns>The <see cref="CurlResult"/> with the server's status, headers and body.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlParsingException"><paramref name="data"/> cannot be serialized to JSON.</exception>
        /// <exception cref="CurlException">The transfer failed; see <see cref="global::CurlDotNet.Curl.ExecuteAsync(string)"/>.</exception>
        /// <seealso cref="PostJsonAsync(string, object)">Async version (preferred)</seealso>
        /// <seealso cref="global::CurlDotNet.Curl.PostJson(string, object)"/>
        public static CurlResult PostJson(string url, object data)
        {
            return PostJsonAsync(url, data).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Serializes <paramref name="data"/> to JSON and POSTs it with <c>Content-Type: application/json</c>
        /// asynchronously; same as <see cref="global::CurlDotNet.Curl.PostJsonAsync(string, object)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Apostrophes and non-ASCII characters are sent literally. Parse the answer with
        /// <see cref="CurlResult.ParseJson{T}"/>.</para>
        /// <para>Example: <c>var created = (await DotNetCurl.PostJsonAsync("https://api.example.com/users", new { name = "John" })).ParseJson&lt;User&gt;();</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">An absolute URL to post to, for example <c>https://api.example.com/users</c>.</param>
        /// <param name="data">The object to send as JSON.</param>
        /// <returns>The <see cref="CurlResult"/> with the server's status, headers and body.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlParsingException"><paramref name="data"/> cannot be serialized to JSON. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</exception>
        /// <exception cref="CurlException">The transfer failed.</exception>
        /// <seealso cref="PostJson(string, object)">Blocking version</seealso>
        /// <seealso cref="global::CurlDotNet.Curl.PostJsonAsync(string, object)"/>
        public static async Task<CurlResult> PostJsonAsync(string url, object data)
        {
            return await global::CurlDotNet.Curl.PostJsonAsync(url, data);
        }

        /// <summary>
        /// Downloads <paramref name="url"/> to <paramref name="outputPath"/> and blocks until the file is written; same as
        /// <see cref="global::CurlDotNet.Curl.Download(string, string)"/> (like <c>curl -o PATH URL</c>).
        /// </summary>
        /// <remarks>
        /// <para>The folder must exist; an existing file is overwritten; the file is written even for an HTTP error
        /// status, so check <see cref="CurlResult.IsSuccess"/>.</para>
        /// <para>Example: <c>var result = DotNetCurl.Download("https://example.com/a.zip", "downloads/a.zip");</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">An absolute URL of the file, for example <c>https://example.com/files/report.pdf</c>.</param>
        /// <param name="outputPath">Where to save it (absolute, relative or a file name); its folder must exist.</param>
        /// <returns>The <see cref="CurlResult"/>; <see cref="CurlResult.OutputFiles"/> contains <paramref name="outputPath"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> or <paramref name="outputPath"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlWriteErrorException">The file could not be written (exit code 23). See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <exception cref="CurlException">The transfer failed.</exception>
        /// <seealso cref="DownloadAsync(string, string)">Async version (preferred)</seealso>
        /// <seealso cref="global::CurlDotNet.Curl.Download(string, string)"/>
        public static CurlResult Download(string url, string outputPath)
        {
            return DownloadAsync(url, outputPath).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Downloads <paramref name="url"/> to <paramref name="outputPath"/> asynchronously; same as
        /// <see cref="global::CurlDotNet.Curl.DownloadAsync(string, string)"/> (like <c>curl -o PATH URL</c>).
        /// </summary>
        /// <remarks>
        /// <para>Paths with spaces or quotes need no escaping. For redirecting download links use
        /// <c>await DotNetCurl.CurlAsync("curl -L -f -o file.zip URL")</c>.</para>
        /// <para>Example: <c>var result = await DotNetCurl.DownloadAsync("https://example.com/manual.pdf", @"C:\Downloads\manual.pdf");</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">An absolute URL of the file.</param>
        /// <param name="outputPath">Where to save it (absolute, relative or a file name); its folder must exist.</param>
        /// <returns>The <see cref="CurlResult"/>; <see cref="CurlResult.OutputFiles"/> contains <paramref name="outputPath"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> or <paramref name="outputPath"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlWriteErrorException">The file could not be written (exit code 23).</exception>
        /// <exception cref="CurlException">The transfer failed. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="Download(string, string)">Blocking version</seealso>
        /// <seealso cref="global::CurlDotNet.Curl.DownloadAsync(string, string)"/>
        public static async Task<CurlResult> DownloadAsync(string url, string outputPath)
        {
            return await global::CurlDotNet.Curl.DownloadAsync(url, outputPath);
        }

        /// <summary>
        /// Checks a curl command without sending it and reports what is wrong (no URL, unknown option, bad URL,
        /// unsupported scheme) or what it would send; same as <see cref="global::CurlDotNet.Curl.Validate(string)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Never throws for bad input. Unknown options, which <see cref="CurlAsync(string)"/> silently ignores,
        /// are reported here.</para>
        /// <para>Example: <c>var v = DotNetCurl.Validate(input); if (!v.IsValid) Console.WriteLine(v.Error);</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md</para>
        /// </remarks>
        /// <param name="command">The curl command line to check; null or empty is reported as invalid.</param>
        /// <returns>A <see cref="ValidationResult"/> with <c>IsValid</c>, <c>Error</c> (the reason, or null) and
        /// <c>ParsedOptions</c> (the parsed <see cref="CurlOptions"/>).</returns>
        /// <seealso cref="global::CurlDotNet.Curl.Validate(string)"/>
        /// <seealso cref="global::CurlDotNet.Curl.Validate(string, CurlFlavor)"/>
        public static ValidationResult Validate(string command)
        {
            return global::CurlDotNet.Curl.Validate(command);
        }

        /// <summary>
        /// Translates a curl command into equivalent C# <c>HttpClient</c> code without sending it; same as
        /// <see cref="global::CurlDotNet.Curl.ToHttpClient(string)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Covers method, URL, headers, body, <c>-u</c>, <c>-x</c>, <c>-k</c> and <c>-L</c>. Credentials from the
        /// command appear in the output in clear text.</para>
        /// <para>Example: <c>Console.WriteLine(DotNetCurl.ToHttpClient("curl -H 'Accept: application/json' https://api.example.com"));</c></para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md</para>
        /// </remarks>
        /// <param name="command">The curl command line to translate.</param>
        /// <returns>C# source text using <c>HttpClient</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlInvalidCommandException">The command text cannot be read; the message names the problem.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="global::CurlDotNet.Curl.ToHttpClient(string)"/>
        /// <seealso cref="ToFetch(string)"/>
        /// <seealso cref="ToPython(string)"/>
        public static string ToHttpClient(string command)
        {
            return global::CurlDotNet.Curl.ToHttpClient(command);
        }

        /// <summary>
        /// Translates a curl command into JavaScript <c>fetch()</c> code without sending it; same as
        /// <see cref="global::CurlDotNet.Curl.ToFetch(string)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Covers URL, method, headers and body. Credentials from the command appear in the output.</para>
        /// <para>Example: <c>string js = DotNetCurl.ToFetch("curl -X POST -d 'a=1' https://api.example.com");</c></para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md</para>
        /// </remarks>
        /// <param name="command">The curl command line to translate.</param>
        /// <returns>JavaScript source text using <c>fetch()</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlInvalidCommandException">The command text cannot be read.</exception>
        /// <seealso cref="global::CurlDotNet.Curl.ToFetch(string)"/>
        /// <seealso cref="ToHttpClient(string)"/>
        public static string ToFetch(string command)
        {
            return global::CurlDotNet.Curl.ToFetch(command);
        }

        /// <summary>
        /// Translates a curl command into Python <c>requests</c> code without sending it; same as
        /// <see cref="global::CurlDotNet.Curl.ToPythonRequests(string)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Covers method, URL, headers, body and <c>-u</c> credentials. Credentials appear in the output.</para>
        /// <para>Example: <c>string py = DotNetCurl.ToPython("curl -u user:pass https://api.example.com/data");</c></para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md</para>
        /// </remarks>
        /// <param name="command">The curl command line to translate.</param>
        /// <returns>Python source text using <c>requests</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlInvalidCommandException">The command text cannot be read.</exception>
        /// <seealso cref="global::CurlDotNet.Curl.ToPythonRequests(string)"/>
        /// <seealso cref="ToHttpClient(string)"/>
        public static string ToPython(string command)
        {
            return global::CurlDotNet.Curl.ToPythonRequests(command);
        }

        /// <summary>
        /// Process-wide limit in whole seconds for every HTTP transfer, like adding <c>-m</c>/<c>--max-time</c> to each
        /// command; 0 means no limit. The same setting as <see cref="global::CurlDotNet.Curl.DefaultMaxTimeSeconds"/>.
        /// </summary>
        /// <remarks>
        /// <para>0 or less means no limit; a command's own <c>-m</c> wins. When it expires the call throws
        /// <see cref="CurlOperationTimeoutException"/> (exit code 28). Thread-safe.</para>
        /// <para>Example: <c>DotNetCurl.DefaultMaxTimeSeconds = 30;</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</para>
        /// </remarks>
        /// <value>Seconds; default 0 (no limit).</value>
        /// <seealso cref="global::CurlDotNet.Curl.DefaultMaxTimeSeconds"/>
        /// <seealso cref="DefaultConnectTimeoutSeconds"/>
        public static int DefaultMaxTimeSeconds
        {
            get => global::CurlDotNet.Curl.DefaultMaxTimeSeconds;
            set => global::CurlDotNet.Curl.DefaultMaxTimeSeconds = value;
        }

        /// <summary>
        /// Process-wide limit in whole seconds for establishing a connection (DNS, TCP, TLS), like
        /// <c>--connect-timeout</c>; 0 means no limit. The same setting as
        /// <see cref="global::CurlDotNet.Curl.DefaultConnectTimeoutSeconds"/>.
        /// </summary>
        /// <remarks>
        /// <para>0 or less means no limit; a command's own <c>--connect-timeout</c> wins. Thread-safe.</para>
        /// <para>Example: <c>DotNetCurl.DefaultConnectTimeoutSeconds = 10;</c> so dead servers fail after 10 seconds.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</para>
        /// </remarks>
        /// <value>Seconds; default 0 (no limit).</value>
        /// <seealso cref="global::CurlDotNet.Curl.DefaultConnectTimeoutSeconds"/>
        /// <seealso cref="DefaultMaxTimeSeconds"/>
        public static int DefaultConnectTimeoutSeconds
        {
            get => global::CurlDotNet.Curl.DefaultConnectTimeoutSeconds;
            set => global::CurlDotNet.Curl.DefaultConnectTimeoutSeconds = value;
        }

        /// <summary>
        /// When true, every request follows HTTP redirects as if <c>-L</c> were in every command; the same setting as
        /// <see cref="global::CurlDotNet.Curl.DefaultFollowRedirects"/>.
        /// </summary>
        /// <remarks>
        /// <para>Default false (curl's behaviour: the redirect response itself is returned). While true it cannot be
        /// turned off per command. Thread-safe.</para>
        /// <para>Example: <c>DotNetCurl.DefaultFollowRedirects = true;</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-follow-redirects-csharp/README.md</para>
        /// </remarks>
        /// <value>true to follow redirects everywhere; default false.</value>
        /// <seealso cref="global::CurlDotNet.Curl.DefaultFollowRedirects"/>
        public static bool DefaultFollowRedirects
        {
            get => global::CurlDotNet.Curl.DefaultFollowRedirects;
            set => global::CurlDotNet.Curl.DefaultFollowRedirects = value;
        }

        /// <summary>
        /// When true, TLS certificates are not verified for any request, as if <c>-k</c> were in every command; the same
        /// setting as <see cref="global::CurlDotNet.Curl.DefaultInsecure"/>. Development and testing only.
        /// </summary>
        /// <remarks>
        /// <para>Disabling verification exposes every https:// request to man-in-the-middle attacks; prefer
        /// <c>--cacert</c> for a private CA. Default false. Thread-safe.</para>
        /// <para>Example: <c>#if DEBUG DotNetCurl.DefaultInsecure = true; #endif</c></para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md</para>
        /// </remarks>
        /// <value>true to skip certificate verification (dangerous); default false.</value>
        /// <seealso cref="global::CurlDotNet.Curl.DefaultInsecure"/>
        public static bool DefaultInsecure
        {
            get => global::CurlDotNet.Curl.DefaultInsecure;
            set => global::CurlDotNet.Curl.DefaultInsecure = value;
        }
    }
}
