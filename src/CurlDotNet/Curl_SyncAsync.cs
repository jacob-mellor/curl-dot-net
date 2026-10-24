/***************************************************************************
 * CurlDotNet - Synchronous vs Asynchronous API Design
 *
 * Following .NET best practices for sync/async methods:
 * - Async methods end with "Async" suffix
 * - Sync methods have no suffix
 * - Both can accept CancellationToken
 * - Async is preferred, sync is for compatibility
 *
 * Based on curl (https://curl.se) by Daniel Stenberg and contributors
 * Original curl Copyright (C) 1996-2025, Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024-2025 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

namespace CurlDotNet
{
    public static partial class Curl
    {
        // Async methods are defined in Curl.cs; this file only contains the blocking wrappers.
        //
        // How the two families differ:
        // - ...Async methods return Task<CurlResult> and free the calling thread while waiting; use them in UI,
        //   ASP.NET Core and any async code, and to run requests in parallel.
        // - The methods without the suffix block the calling thread until the response has arrived. They exist for
        //   console tools, scripts, tests and legacy code that cannot be async. Exceptions are the same (not wrapped
        //   in AggregateException). Do not call them on a UI thread or in classic ASP.NET request code, where blocking
        //   on async work can deadlock.
        // - A CancellationToken does not make a call asynchronous: Execute(command, token) still blocks.

        #region SYNCHRONOUS Methods (Compatibility) - Blocks the thread

        /// <summary>
        /// Runs a curl command and blocks the calling thread until the response arrives; the synchronous twin of
        /// <see cref="ExecuteAsync(string)"/> for console tools, scripts and code that cannot be async.
        /// </summary>
        /// <remarks>
        /// <para>Behaves exactly like <see cref="ExecuteAsync(string)"/> (same command format, same result, same
        /// exceptions - thrown directly, not wrapped in <see cref="AggregateException"/>) but waits for the result. The
        /// thread does nothing else meanwhile, so do not call it on a UI thread (the window freezes) or in classic
        /// ASP.NET request code (it can deadlock and reduces throughput); use the async method there.</para>
        /// <para>Example: <c>var result = Curl.Execute("curl -s https://api.example.com/status");</c> then
        /// <c>Console.WriteLine(result.Body);</c> in a <c>static void Main()</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-synchronously-in-csharp/README.md</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// static void Main()
        /// {
        ///     var result = Curl.Execute("curl https://api.example.com");
        ///     Console.WriteLine(result.Body);
        /// }
        /// </code>
        /// </example>
        /// <param name="command">The curl command line as text, for example <c>curl -H 'Accept: application/json' https://api.example.com</c>;
        /// the leading <c>curl</c> is optional. Read with <see cref="DefaultFlavor"/>.</param>
        /// <returns>The <see cref="CurlResult"/>: status code, body (standard output text), headers, binary data and
        /// written files, as described on <see cref="ExecuteAsync(string)"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="command"/> is null, empty or whitespace. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md</exception>
        /// <exception cref="CurlException">The command could not be read or the transfer failed; the subtypes (DNS 6,
        /// connect 7, timeout 28, TLS 35/60, <c>-f</c> HTTP error 22...) are listed on <see cref="ExecuteAsync(string)"/>.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="ExecuteAsync(string)">Async version (preferred)</seealso>
        /// <seealso cref="Execute(string, CancellationToken)"/>
        /// <seealso cref="CurlRequestBuilder.Execute"/>
        /// <seealso cref="DotNetCurl.Curl(string)"/>
        public static CurlResult Execute(string command)
        {
            // This BLOCKS the thread until complete
            return ExecuteAsync(command).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Runs a curl command, blocking the calling thread until the response arrives or
        /// <paramref name="cancellationToken"/> is cancelled; the synchronous twin of
        /// <see cref="ExecuteAsync(string, CancellationToken)"/>.
        /// </summary>
        /// <remarks>
        /// <para>The token lets another thread (or a timer) stop the request; the call still blocks until then. Cancelling
        /// throws <see cref="OperationCanceledException"/>. Prefer the async overload whenever the caller can await.</para>
        /// <para>Example: <c>using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));</c> then
        /// <c>var result = Curl.Execute("curl https://api.example.com", cts.Token);</c> gives up after 30 seconds.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</para>
        /// </remarks>
        /// <param name="command">The curl command line as text, as for <see cref="ExecuteAsync(string)"/>.</param>
        /// <param name="cancellationToken">Cancels the request when signalled, for example from a
        /// <c>CancellationTokenSource</c> with a timeout; <c>CancellationToken.None</c> for no cancellation.</param>
        /// <returns>The <see cref="CurlResult"/>, as described on <see cref="ExecuteAsync(string)"/>.</returns>
        /// <exception cref="OperationCanceledException">The token was cancelled before the transfer finished.</exception>
        /// <exception cref="ArgumentException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlException">The transfer failed; see <see cref="ExecuteAsync(string)"/>. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="ExecuteAsync(string, CancellationToken)">Async version (preferred)</seealso>
        /// <seealso cref="Execute(string)"/>
        /// <seealso cref="DotNetCurl.Curl(string, int)"/>
        public static CurlResult Execute(string command, CancellationToken cancellationToken)
        {
            // This BLOCKS the thread until complete or cancelled
            return ExecuteAsync(command, cancellationToken).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Executes a curl command copied from a specific shell and blocks the calling thread until the response
        /// arrives.
        /// </summary>
        /// <param name="command">The command exactly as it was pasted, with or without the leading <c>curl</c> /
        /// <c>curl.exe</c> / <c>Invoke-WebRequest</c>.</param>
        /// <param name="flavor">Where it came from: <see cref="CurlFlavor.Posix"/> (bash/zsh),
        /// <see cref="CurlFlavor.WindowsCmd"/> (curl.exe in cmd.exe), <see cref="CurlFlavor.PowerShell"/> (curl.exe in
        /// PowerShell), <see cref="CurlFlavor.InvokeWebRequest"/> (Invoke-WebRequest / Invoke-RestMethod syntax) or
        /// <see cref="CurlFlavor.Auto"/> to detect it. Overrides <see cref="DefaultFlavor"/> for this call.</param>
        /// <returns>The <see cref="CurlResult"/>, as described on <see cref="ExecuteAsync(string)"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlInvalidCommandException">The text breaks the flavor's rules or uses an
        /// untranslatable <c>Invoke-WebRequest</c> parameter; the message says how to fix it. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md</exception>
        /// <exception cref="CurlException">The request fails; see <see cref="ExecuteAsync(string)"/>.</exception>
        /// <remarks>
        /// <para>Blocks the thread; prefer <see cref="ExecuteAsync(string, CurlFlavor)"/> in async code. See that method
        /// for what changes between flavors.</para>
        /// <para>Example: <c>var result = Curl.Execute("curl -H \"Accept: application/json\" -d \"{\\\"a\\\":1}\" https://api.example.com", CurlFlavor.WindowsCmd);</c>
        /// sends the body <c>{"a":1}</c>.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md</para>
        /// </remarks>
        /// <seealso cref="ExecuteAsync(string, CurlFlavor)"/>
        /// <seealso cref="DefaultFlavor"/>
        public static CurlResult Execute(string command, CurlFlavor flavor)
        {
            // This BLOCKS the thread until complete
            return ExecuteAsync(command, flavor).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Runs a curl command with timeouts, retries, extra headers or a progress callback from a
        /// <see cref="CurlSettings"/> object, blocking the calling thread until the response arrives.
        /// </summary>
        /// <remarks>
        /// <para>Applies the same settings as <see cref="ExecuteAsync(string, CurlSettings)"/>: <c>MaxTimeSeconds</c>
        /// (<c>-m</c>), <c>ConnectTimeoutSeconds</c> (<c>--connect-timeout</c>), <c>FollowRedirects</c> (<c>-L</c>),
        /// <c>Insecure</c> (<c>-k</c>), <c>Headers</c> (<c>-H</c>), <c>OnProgress</c>, <c>RetryCount</c>/<c>RetryDelayMs</c>,
        /// <c>Flavor</c> and <c>CancellationToken</c>. The progress callback runs on a thread-pool thread, not the
        /// blocked caller.</para>
        /// <para>Example: <c>var result = Curl.Execute("curl https://flaky.example.com/api", new CurlSettings().WithRetries(3, 1000).WithTimeout(20));</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-synchronously-in-csharp/README.md</para>
        /// </remarks>
        /// <param name="command">The curl command line as text, as for <see cref="ExecuteAsync(string)"/>.</param>
        /// <param name="settings">Settings applied on top of the command, built with <c>new CurlSettings().With...()</c>;
        /// null runs the command unchanged.</param>
        /// <returns>The <see cref="CurlResult"/>, as described on <see cref="ExecuteAsync(string)"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="OperationCanceledException">The settings' cancellation token was cancelled.</exception>
        /// <exception cref="CurlAbortedByCallbackException">The progress callback threw (exit code 42).</exception>
        /// <exception cref="CurlException">The transfer failed after all retries; see <see cref="ExecuteAsync(string)"/>.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="ExecuteAsync(string, CurlSettings)">Async version (preferred)</seealso>
        /// <seealso cref="CurlSettings"/>
        public static CurlResult Execute(string command, CurlSettings settings)
        {
            // This BLOCKS the thread until complete
            return ExecuteAsync(command, settings).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sends a request described by a <see cref="CurlOptions"/> object and blocks the calling thread until the
        /// response arrives; nothing is parsed, so data with quotes or spaces needs no escaping.
        /// </summary>
        /// <remarks>
        /// <para>The synchronous twin of <see cref="ExecuteAsync(CurlOptions)"/>; see it for how options map to curl flags.</para>
        /// <para>Example: <c>var result = Curl.Execute(new CurlOptions { Url = "https://api.example.com/items/7", Method = "DELETE" });</c>
        /// then <c>Console.WriteLine(result.StatusCode);</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var result = Curl.Execute(new CurlOptions { Url = "https://api.example.com", Method = "DELETE" });
        /// </code>
        /// </example>
        /// <param name="options">The request to send; <see cref="CurlOptions.Url"/> must be set to an absolute URL.</param>
        /// <returns>The <see cref="CurlResult"/>, as described on <see cref="ExecuteAsync(string)"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
        /// <exception cref="CurlException">No URL (exit code 2, like curl's "no URL specified") or the transfer failed; see <see cref="ExecuteAsync(string)"/>.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="ExecuteAsync(CurlOptions)">Async version (preferred)</seealso>
        /// <seealso cref="Lib.LibCurl.PerformAsync(CurlOptions, CancellationToken)"/>
        public static CurlResult Execute(CurlOptions options)
        {
            // This BLOCKS the thread until complete
            return ExecuteAsync(options).GetAwaiter().GetResult();
        }

        #endregion

        #region Quick Helper Methods - Synchronous versions only

        /// <summary>
        /// Sends a plain GET request to <paramref name="url"/> and blocks until the response arrives; the synchronous
        /// twin of <see cref="GetAsync(string)"/> (like <c>curl URL</c>).
        /// </summary>
        /// <remarks>
        /// <para>No headers or options; the <c>Default*</c> properties apply. HTTP errors are returned, not thrown.
        /// Do not call it on a UI thread.</para>
        /// <para>Example: <c>var result = Curl.Get("https://api.example.com/health");</c> then
        /// <c>Console.WriteLine(result.IsSuccess ? "up" : $"down ({result.StatusCode})");</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-synchronously-in-csharp/README.md</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var result = Curl.Get("https://api.example.com"); // blocks until the response arrives
        /// </code>
        /// </example>
        /// <param name="url">An absolute URL including the scheme, for example <c>https://api.example.com/users?page=1</c>.</param>
        /// <returns>The <see cref="CurlResult"/> with the status code, body and headers.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> is null, empty or whitespace. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</exception>
        /// <exception cref="CurlException">The transfer failed; see <see cref="ExecuteAsync(string)"/>.</exception>
        /// <seealso cref="GetAsync(string)">Async version (preferred)</seealso>
        /// <seealso cref="CurlRequestBuilder.Get(string)"/>
        /// <seealso cref="DotNetCurl.Get(string)"/>
        public static CurlResult Get(string url)
        {
            return GetAsync(url).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sends a POST request with <paramref name="data"/> as the body and blocks until the response arrives; the
        /// synchronous twin of <see cref="PostAsync(string, string)"/> (like <c>curl -d DATA URL</c>).
        /// </summary>
        /// <remarks>
        /// <para>The data is sent exactly as given (no escaping needed, no URL-encoding done for you), with curl's default
        /// <c>Content-Type: application/x-www-form-urlencoded</c>; use <see cref="PostJson(string, object)"/> for JSON.</para>
        /// <para>Example: <c>var result = Curl.Post("https://api.example.com/login", "user=john&amp;pass=" + Uri.EscapeDataString(pw));</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">An absolute URL to post to, for example <c>https://api.example.com/users</c>.</param>
        /// <param name="data">The request body as text (form fields, JSON, XML...); null sends an empty body.</param>
        /// <returns>The <see cref="CurlResult"/> with the server's status, headers and body.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> is null, empty or whitespace. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</exception>
        /// <exception cref="CurlException">The transfer failed; see <see cref="ExecuteAsync(string)"/>.</exception>
        /// <seealso cref="PostAsync(string, string)">Async version (preferred)</seealso>
        /// <seealso cref="PostJson(string, object)"/>
        /// <seealso cref="DotNetCurl.Post(string, string)"/>
        public static CurlResult Post(string url, string data)
        {
            return PostAsync(url, data).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Serializes <paramref name="data"/> to JSON and POSTs it with <c>Content-Type: application/json</c>, blocking
        /// until the response arrives; the synchronous twin of <see cref="PostJsonAsync(string, object)"/>.
        /// </summary>
        /// <remarks>
        /// <para>Serialization uses System.Text.Json (Newtonsoft.Json on the .NET Standard 2.0 build); apostrophes and
        /// non-ASCII characters are sent literally.</para>
        /// <para>Example: <c>var result = Curl.PostJson("https://api.example.com/users", new { name = "John", age = 30 });</c>
        /// then <c>var user = result.ParseJson&lt;User&gt;();</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">An absolute URL to post to, for example <c>https://api.example.com/users</c>.</param>
        /// <param name="data">The object to send as JSON: anonymous object, your class, array/list or dictionary.</param>
        /// <returns>The <see cref="CurlResult"/> with the server's status, headers and body.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlParsingException"><paramref name="data"/> cannot be serialized to JSON. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</exception>
        /// <exception cref="CurlException">The transfer failed; see <see cref="ExecuteAsync(string)"/>.</exception>
        /// <seealso cref="PostJsonAsync(string, object)">Async version (preferred)</seealso>
        /// <seealso cref="CurlRequestBuilder.WithJson(object)"/>
        /// <seealso cref="DotNetCurl.PostJson(string, object)"/>
        public static CurlResult PostJson(string url, object data)
        {
            return PostJsonAsync(url, data).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Downloads <paramref name="url"/> to <paramref name="outputPath"/> and blocks until the file is written; the
        /// synchronous twin of <see cref="DownloadAsync(string, string)"/> (like <c>curl -o PATH URL</c>).
        /// </summary>
        /// <remarks>
        /// <para>The folder must exist; an existing file is overwritten; the file is written even for an HTTP error
        /// status, so check <see cref="CurlResult.IsSuccess"/>. Redirects are followed only with
        /// <see cref="DefaultFollowRedirects"/>.</para>
        /// <para>Example: <c>var result = Curl.Download("https://example.com/setup.exe", @"C:\Temp\setup.exe");</c> then
        /// <c>if (!result.IsSuccess) File.Delete(@"C:\Temp\setup.exe");</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <param name="url">An absolute URL of the file, for example <c>https://example.com/files/report.pdf</c>.</param>
        /// <param name="outputPath">Where to save it: absolute, relative to the current directory, or a file name. Its
        /// folder must exist.</param>
        /// <returns>The <see cref="CurlResult"/>; <see cref="CurlResult.OutputFiles"/> contains <paramref name="outputPath"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> or <paramref name="outputPath"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlWriteErrorException">The file could not be written (missing folder, read-only disk;
        /// exit code 23). See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <exception cref="CurlException">The transfer failed; see <see cref="ExecuteAsync(string)"/>.</exception>
        /// <seealso cref="DownloadAsync(string, string)">Async version (preferred)</seealso>
        /// <seealso cref="DotNetCurl.Download(string, string)"/>
        public static CurlResult Download(string url, string outputPath)
        {
            return DownloadAsync(url, outputPath).GetAwaiter().GetResult();
        }

        #endregion
    }
}
