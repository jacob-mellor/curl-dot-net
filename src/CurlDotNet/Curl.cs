/***************************************************************************
 * CurlDotNet - Pure .NET implementation of curl
 *
 * Inspired by and based on curl (https://curl.se) by Daniel Stenberg
 * Original curl Copyright (C) Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024 IronSoftware
 *
 * The killer feature: Copy curl commands from anywhere and they just work!
 *
 * This could revolutionize how developers use HTTP in every language.
 * Imagine: Same curl commands working in C#, Rust, JavaScript, Python...
 *
 * By Jacob Mellor
 * GitHub: https://github.com/jacob-mellor
 *
 * This file structure inspired by curl's src/tool_operate.c
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;
using CurlRequestBuilder = CurlDotNet.Core.CurlRequestBuilder;

namespace CurlDotNet
{
    /// <summary>
    /// Runs curl commands from C#: pass the command text you would type in a terminal (for example
    /// <c>curl -H 'Accept: application/json' https://api.example.com</c>) and get a <see cref="CurlResult"/> back.
    /// </summary>
    /// <remarks>
    /// <para><b>Start here.</b> <see cref="ExecuteAsync(string)"/> is the main method: copy a command from API
    /// documentation, Stack Overflow, Postman ("Code &gt; cURL") or a browser's "Copy as cURL" and pass it as a string.
    /// The leading <c>curl</c> is optional. Shell quoting, line continuations (<c>\</c>, <c>^</c>, backtick) and
    /// environment variables are read the way the shell it came from would read them (see <see cref="CurlFlavor"/>).</para>
    /// <para><b>Four ways to make a request</b>, all producing the same <see cref="CurlResult"/>:
    /// a command string with <see cref="ExecuteAsync(string)"/>; a typed <see cref="CurlOptions"/> object with
    /// <see cref="ExecuteAsync(CurlOptions)"/> (nothing is parsed, so data needs no quoting); the fluent
    /// <see cref="CurlRequestBuilder"/> (<c>CurlRequestBuilder.Get(url).WithHeader(...).ExecuteAsync()</c>); and the
    /// libcurl-style <see cref="Lib.LibCurl"/> instance API with default headers and timeouts. The quick helpers
    /// <see cref="GetAsync"/>, <see cref="PostAsync"/>, <see cref="PostJsonAsync"/> and <see cref="DownloadAsync"/>
    /// cover one-line cases.</para>
    /// <para><b>What counts as a failure.</b> Like the curl tool, an HTTP error status (404, 500...) is not an
    /// exception: you get a result with <see cref="CurlResult.StatusCode"/> set, and can call
    /// <see cref="CurlResult.EnsureSuccess"/>. Add <c>-f</c>/<c>--fail</c> to the command to turn HTTP errors into a
    /// <see cref="CurlHttpReturnedErrorException"/> (exit code 22). Everything curl reports with a non-zero exit code
    /// (DNS failure 6, connection refused 7, timeout 28, TLS problem 35/60, write error 23...) throws a
    /// <see cref="CurlException"/> subtype whose <see cref="CurlException.CurlErrorCode"/> is curl's exit code and whose
    /// message says what happened, how to fix it and where to read more.</para>
    /// <para><b>Sync and async.</b> Every <c>...Async</c> method has a blocking twin without the suffix
    /// (<see cref="Execute(string)"/>, <see cref="Get"/>, <see cref="Post"/>, <see cref="PostJson"/>,
    /// <see cref="Download"/>). Use the async ones in UI, ASP.NET and any code that is already async.</para>
    /// <para><b>Thread safety.</b> All static methods are safe to call from many threads at once; they share one
    /// pooled connection engine. The <c>Default*</c> properties are process-wide settings read at the start of every
    /// request, so set them once at start-up.</para>
    /// <para><b>Memory.</b> The response body is read into memory (<see cref="CurlResult.Body"/> or
    /// <see cref="CurlResult.BinaryData"/>) even when <c>-o</c> also writes it to a file, so for multi-gigabyte
    /// downloads use a streaming <c>HttpClient</c> instead.</para>
    /// <para>Example: <c>var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");</c> then
    /// <c>Console.WriteLine($"{result.StatusCode}: {result.Body}");</c>.</para>
    /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md
    /// - error handling: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</para>
    /// <para>Sponsored by <see href="https://ironsoftware.com">IronSoftware</see> - creators of IronPDF, IronOCR, IronXL, and IronBarcode.</para>
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// // Just paste any curl command as a string:
    /// var response = await Curl.ExecuteAsync("curl https://api.github.com");
    /// Console.WriteLine(response.Body);
    ///
    /// // Apply CurlSettings for retries/timeouts
    /// var settings = new CurlSettings()
    ///     .WithTimeout(seconds: 10)
    ///     .WithRetries(count: 2, delayMs: 500);
    /// var json = await Curl.ExecuteAsync("curl https://httpbin.org/json", settings);
    /// json.EnsureSuccess();
    ///
    /// // Or build the request in code
    /// var result = await CurlRequestBuilder.Post("https://api.example.com/data")
    ///     .WithJson(new { key = "value" })
    ///     .ExecuteAsync();
    /// </code>
    /// </example>
    /// <seealso cref="DotNetCurl"/>
    /// <seealso cref="CurlRequestBuilder"/>
    /// <seealso cref="Lib.LibCurl"/>
    /// <seealso cref="CurlResult"/>
    /// <seealso cref="CurlException"/>
    public static partial class Curl
    {
        private static readonly CurlEngine _engine = new CurlEngine();

        // Global settings - thread-safe with volatile
        private static volatile int _defaultMaxTimeSeconds = 0; // 0 = no timeout

        /// <summary>
        /// The curl release (<c>8.7.1</c>) whose command-line behaviour CurlDotNet reproduces; it is also the
        /// version in the default <c>User-Agent: curl/8.7.1</c> header.
        /// </summary>
        /// <remarks>
        /// <para>Option parsing, exit codes, <c>-w</c> variables and wire behaviour follow this curl version. When a
        /// newer curl behaves differently, CurlDotNet behaves like this one.</para>
        /// <para>Example: <c>Console.WriteLine($"Compatible with curl {Curl.CurlCompatVersion}");</c>. To change the
        /// User-Agent use <see cref="DefaultUserAgent"/> or <c>-A</c>; this constant does not change.</para>
        /// </remarks>
        /// <seealso cref="DefaultUserAgent"/>
        public const string CurlCompatVersion = "8.7.1";

        /// <summary>
        /// The <c>User-Agent</c> header sent when a command does not set one; <c>curl/8.7.1</c> unless you change it.
        /// Set it to null or an empty string to send no User-Agent at all.
        /// </summary>
        /// <remarks>
        /// <para>Real curl always identifies itself as <c>curl/&lt;version&gt;</c>, and many APIs (GitHub's, for
        /// example) reject requests with no User-Agent, so CurlDotNet sends one by default. Per request, <c>-A</c> /
        /// <c>--user-agent "MyApp/1.0"</c> or <c>-H "User-Agent: MyApp/1.0"</c> wins over this default, and
        /// <c>-H "User-Agent:"</c> (empty value) suppresses the header for that request.</para>
        /// <para>Process-wide: read at the start of every HTTP request (and for the CONNECT request of a proxy
        /// tunnel). Set it once at start-up; it is safe to read from many threads.</para>
        /// <para>Example: <c>Curl.DefaultUserAgent = "MyApp/2.0 (+https://example.com)";</c> then every
        /// <c>Curl.ExecuteAsync("curl https://api.github.com")</c> sends <c>User-Agent: MyApp/2.0 (+https://example.com)</c>.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md</para>
        /// </remarks>
        /// <value>Any header value; default <c>"curl/" + CurlCompatVersion</c>. Null or empty sends no header.</value>
        /// <seealso cref="CurlCompatVersion"/>
        /// <seealso cref="CurlRequestBuilder.WithUserAgent(string)"/>
        /// <seealso cref="CurlOptions.UserAgent"/>
        public static string DefaultUserAgent { get; set; } = "curl/" + CurlCompatVersion;

        private static volatile CurlFlavor _defaultFlavor = CurlFlavor.Auto;

        /// <summary>
        /// The shell (bash, cmd.exe, PowerShell, Invoke-WebRequest) whose quoting rules are used to read command strings
        /// when a call does not name one; <see cref="CurlFlavor.Auto"/> by default, which detects it.
        /// </summary>
        /// <remarks>
        /// <para>A pasted curl command is shell text: bash, Windows cmd.exe and PowerShell quote, escape,
        /// continue lines and expand variables differently, so the same text can mean different bytes (see
        /// <see cref="CurlFlavor"/> for every rule). The default, <see cref="CurlFlavor.Auto"/>, recognises the
        /// shell from telltales such as backtick or caret line continuations, <c>$env:</c>, <c>%VAR%</c> or
        /// <c>-Uri</c>, and otherwise reads the command permissively as bash. Set this once at start-up when every
        /// command your application runs comes from one place, for example
        /// <c>Curl.DefaultFlavor = CurlFlavor.WindowsCmd;</c> for commands typed by Windows users, or
        /// <c>Curl.DefaultFlavor = CurlFlavor.Posix;</c> to get exact bash behaviour with no Windows conveniences.</para>
        /// <para>Applies to <see cref="ExecuteAsync(string)"/>, <see cref="Execute(string)"/>,
        /// <see cref="Validate(string)"/>, the code converters and every other method that takes a command string
        /// without a flavor. A flavor passed to <see cref="ExecuteAsync(string, CurlFlavor)"/> or set with
        /// <see cref="CurlSettings.WithFlavor(CurlFlavor)"/> wins over it. It is process-wide and read at the
        /// start of each call; reads and writes are thread-safe.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md</para>
        /// </remarks>
        /// <value>One of the <see cref="CurlFlavor"/> values. Default is <see cref="CurlFlavor.Auto"/>.</value>
        /// <exception cref="ArgumentOutOfRangeException">The value is not a defined <see cref="CurlFlavor"/> (for
        /// example a number cast to the enum). Fix: assign <c>CurlFlavor.Auto</c>, <c>Posix</c>, <c>WindowsCmd</c>,
        /// <c>PowerShell</c> or <c>InvokeWebRequest</c>. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md</exception>
        /// <seealso cref="DetectFlavor(string)"/>
        /// <seealso cref="CurlOptions.Flavor"/>
        /// <seealso cref="CurlSettings.WithFlavor(CurlFlavor)"/>
        /// <seealso href="https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md">curl flavors guide</seealso>
        public static CurlFlavor DefaultFlavor
        {
            get => _defaultFlavor;
            set
            {
                if (value < CurlFlavor.Auto || value > CurlFlavor.InvokeWebRequest)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, CurlException.Compose(
                        $"Curl.DefaultFlavor was set to {(int)value}, which is not a CurlFlavor value.",
                        "Assign CurlFlavor.Auto (detect the shell), Posix (bash/zsh), WindowsCmd (cmd.exe), PowerShell (curl.exe in PowerShell) or InvokeWebRequest (Invoke-WebRequest/Invoke-RestMethod syntax).",
                        DocLinks.HowToFlavors));
                }

                _defaultFlavor = value;
            }
        }

        private static volatile int _defaultConnectTimeoutSeconds = 0;
        private static volatile bool _defaultFollowRedirects = false;
        private static volatile bool _defaultInsecure = false;

        /// <summary>
        /// A process-wide limit, in whole seconds, on how long any HTTP transfer may take, as if <c>-m</c>/<c>--max-time</c>
        /// were added to every command that does not set its own. 0 (the default) means no limit.
        /// </summary>
        /// <remarks>
        /// <para>Values: 0 or a negative number means no limit (like curl without <c>--max-time</c>); any positive
        /// number is the total time allowed for connect, request, response headers and body together. A command's own
        /// <c>-m 60</c> / <c>--max-time 60</c> (decimals allowed there, such as <c>-m 2.5</c>), or
        /// <see cref="CurlSettings.WithTimeout(int)"/>, wins over this default.</para>
        /// <para>When the limit expires the call throws <see cref="CurlOperationTimeoutException"/> (a
        /// <see cref="CurlTimeoutException"/>, exit code 28) whose message says how many bytes had arrived. Your
        /// own <see cref="CancellationToken"/> is separate: cancelling it throws <see cref="OperationCanceledException"/>.</para>
        /// <para>Process-wide and thread-safe; read at the start of every transfer. Applies to http:// and https://.</para>
        /// <para>Example: <c>Curl.DefaultMaxTimeSeconds = 30;</c> then <c>await Curl.ExecuteAsync("curl https://slow.example.com")</c>
        /// fails after 30 seconds, while <c>await Curl.ExecuteAsync("curl -m 120 https://slow.example.com/big.zip -o big.zip")</c>
        /// gets 120.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// Curl.DefaultMaxTimeSeconds = 30;
        /// await Curl.ExecuteAsync("curl https://slow-api.example.com");                 // times out after 30 s
        /// await Curl.ExecuteAsync("curl --max-time 60 https://very-slow-api.example.com"); // 60 s for this one
        /// </code>
        /// </example>
        /// <value>Seconds; 0 (default) or less means no limit.</value>
        /// <seealso cref="DefaultConnectTimeoutSeconds"/>
        /// <seealso cref="CurlOptions.MaxTime"/>
        /// <seealso cref="CurlSettings.WithTimeout(int)"/>
        /// <seealso cref="CurlRequestBuilder.WithTimeout(TimeSpan)"/>
        /// <seealso cref="DotNetCurl.DefaultMaxTimeSeconds"/>
        public static int DefaultMaxTimeSeconds
        {
            get => _defaultMaxTimeSeconds;
            set => _defaultMaxTimeSeconds = value;
        }

        /// <summary>
        /// A process-wide limit, in whole seconds, on how long establishing a connection may take, as if
        /// <c>--connect-timeout</c> were added to every command that does not set its own. 0 (the default) means no limit.
        /// </summary>
        /// <remarks>
        /// <para>This covers the DNS lookup, the TCP connect and the TLS handshake only, not the response. Values: 0 or
        /// less means no connect limit; a positive number is seconds. A command's own <c>--connect-timeout 5</c>
        /// (decimals allowed, such as <c>--connect-timeout 2.5</c>) or <see cref="CurlSettings.WithConnectTimeout(int)"/>
        /// wins over this default.</para>
        /// <para>When it expires the call throws <see cref="CurlOperationTimeoutException"/> (exit code 28) with a
        /// message such as <c>Connection timed out after 5000 milliseconds</c>.</para>
        /// <para>Tip: set it well below <see cref="DefaultMaxTimeSeconds"/> so that dead servers fail fast while slow
        /// downloads still get time. Process-wide and thread-safe; applies to every protocol.</para>
        /// <para>Example: <c>Curl.DefaultConnectTimeoutSeconds = 10;</c> then a request to a server that never accepts
        /// the connection fails after 10 seconds instead of the operating system's default (often over a minute).</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</para>
        /// </remarks>
        /// <value>Seconds; 0 (default) or less means no connect limit.</value>
        /// <seealso cref="DefaultMaxTimeSeconds"/>
        /// <seealso cref="CurlOptions.ConnectTimeout"/>
        /// <seealso cref="CurlSettings.WithConnectTimeout(int)"/>
        /// <seealso cref="CurlRequestBuilder.WithConnectTimeout(TimeSpan)"/>
        /// <seealso cref="DotNetCurl.DefaultConnectTimeoutSeconds"/>
        public static int DefaultConnectTimeoutSeconds
        {
            get => _defaultConnectTimeoutSeconds;
            set => _defaultConnectTimeoutSeconds = value;
        }

        /// <summary>
        /// When true, every request follows HTTP redirects (301, 302, 303, 307, 308) as if <c>-L</c>/<c>--location</c>
        /// were in every command. False (the default) returns the redirect response itself, like curl.
        /// </summary>
        /// <remarks>
        /// <para>While this is true it cannot be switched off for a single command; leave it false and add <c>-L</c>
        /// to the commands that need it if only some should follow redirects. The redirect limit is
        /// <c>--max-redirs</c> (curl's default is 50); exceeding it throws <see cref="CurlTooManyRedirectsException"/>
        /// (exit code 47). As in curl, credentials given with <c>-u</c> are not sent to a different host after a
        /// redirect unless the command has <c>--location-trusted</c>.</para>
        /// <para>Process-wide and thread-safe; read at the start of every transfer.</para>
        /// <para>Example: <c>Curl.DefaultFollowRedirects = true;</c> then <c>(await Curl.ExecuteAsync("curl https://bit.ly/example")).StatusCode</c>
        /// is the status of the final page instead of 301.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-follow-redirects-csharp/README.md</para>
        /// </remarks>
        /// <value>true to follow redirects everywhere; false (default) to follow only with <c>-L</c>.</value>
        /// <seealso cref="CurlOptions.FollowLocation"/>
        /// <seealso cref="CurlSettings.WithFollowRedirects(bool)"/>
        /// <seealso cref="CurlRequestBuilder.FollowRedirects(bool)"/>
        /// <seealso cref="DotNetCurl.DefaultFollowRedirects"/>
        public static bool DefaultFollowRedirects
        {
            get => _defaultFollowRedirects;
            set => _defaultFollowRedirects = value;
        }

        /// <summary>
        /// When true, TLS certificates are not verified for any request, as if <c>-k</c>/<c>--insecure</c> were in every
        /// command. Development and testing only: it accepts self-signed, expired and wrong-host certificates.
        /// </summary>
        /// <remarks>
        /// <para>Turning certificate checks off makes every https:// request open to man-in-the-middle attacks. Prefer
        /// trusting the specific certificate with <c>--cacert ca.pem</c>, or add <c>-k</c> only to the commands that talk
        /// to a local development server. While this is true it cannot be switched off for a single command.</para>
        /// <para>Without it, a certificate problem throws <see cref="CurlPeerFailedVerificationException"/> (exit code 60)
        /// or another <see cref="CurlSslException"/>.</para>
        /// <para>Process-wide and thread-safe; read at the start of every transfer.</para>
        /// <para>Example: <c>#if DEBUG Curl.DefaultInsecure = true; #endif</c> lets
        /// <c>await Curl.ExecuteAsync("curl https://localhost:5001/api")</c> reach a dev server with a self-signed
        /// certificate, while release builds keep checking.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md</para>
        /// </remarks>
        /// <value>true to skip certificate verification (dangerous); false (default) to verify.</value>
        /// <seealso cref="CurlOptions.Insecure"/>
        /// <seealso cref="CurlSettings.WithInsecure(bool)"/>
        /// <seealso cref="CurlRequestBuilder.Insecure(bool)"/>
        /// <seealso cref="DotNetCurl.DefaultInsecure"/>
        public static bool DefaultInsecure
        {
            get => _defaultInsecure;
            set => _defaultInsecure = value;
        }

        /// <summary>
        /// Runs a curl command (the same text you would type in a terminal) and returns the response; this is the main
        /// method of CurlDotNet.
        /// </summary>
        /// <remarks>
        /// <para><b>The command.</b> Any curl command line, with or without the leading <c>curl</c> / <c>curl.exe</c>:
        /// <c>curl -X POST -H 'Content-Type: application/json' -d '{"a":1}' https://api.example.com</c>. Multi-line
        /// commands with <c>\</c>, <c>^</c> or backtick continuations work. It is read with <see cref="DefaultFlavor"/>
        /// (auto-detected shell by default); pass a <see cref="CurlFlavor"/> to
        /// <see cref="ExecuteAsync(string, CurlFlavor)"/> when you know where it came from. Several URLs, URL globs
        /// (<c>https://x/[1-3].json</c>) and <c>--next</c> run one after another like curl's transfer loop; the result
        /// describes the last transfer and <see cref="CurlResult.Body"/> holds the output of all of them.</para>
        /// <para><b>What you get back.</b> HTTP error statuses are not exceptions: a 404 returns a result with
        /// <see cref="CurlResult.StatusCode"/> 404 (add <c>-f</c> to throw instead). <see cref="CurlResult.Body"/> holds
        /// what curl would print on standard output (the body, plus headers with <c>-i</c>, plus <c>-w</c> text);
        /// binary responses are in <see cref="CurlResult.BinaryData"/>; files written with <c>-o</c>/<c>-O</c> are listed
        /// in <see cref="CurlResult.OutputFiles"/>.</para>
        /// <para><b>Unknown options</b> are ignored, as a lenient curl would; call <see cref="Validate(string)"/> first
        /// when the command comes from users and you want typos reported.</para>
        /// <para><b>Defaults</b> that apply when the command does not say otherwise: <see cref="DefaultMaxTimeSeconds"/>
        /// (<c>-m</c>), <see cref="DefaultConnectTimeoutSeconds"/> (<c>--connect-timeout</c>),
        /// <see cref="DefaultFollowRedirects"/> (<c>-L</c>), <see cref="DefaultInsecure"/> (<c>-k</c>) and
        /// <see cref="DefaultUserAgent"/> (<c>-A</c>).</para>
        /// <para>Thread-safe; many calls may run at once and share pooled connections.</para>
        /// <para>Example: <c>var result = await Curl.ExecuteAsync("curl -u alice:secret https://api.example.com/me");</c>
        /// then <c>if (result.IsSuccess) Console.WriteLine(result.Body); else Console.WriteLine($"HTTP {result.StatusCode}");</c>.</para>
        /// <para>Example with error handling: <c>try { await Curl.ExecuteAsync("curl -f https://api.example.com/x"); }
        /// catch (CurlHttpReturnedErrorException ex) { /* ex.StatusCode is 404 */ } catch (CurlException ex) { Console.WriteLine(ex.Message); /* what, fix, link */ }</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md
        /// - errors: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// // Copy any curl command from documentation and paste it here:
        /// var response = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");
        /// Console.WriteLine($"Status: {response.StatusCode}");  // 200
        /// var user = response.ParseJson&lt;GitHubUser&gt;();
        ///
        /// // The exact command from Stripe's documentation:
        /// var charge = await Curl.ExecuteAsync(@"
        ///     curl https://api.stripe.com/v1/charges \
        ///       -u sk_test_4eC39HqLyjWDarjtT1zdp7dc: \
        ///       -d amount=2000 \
        ///       -d currency=usd \
        ///       -d source=tok_mastercard");
        ///
        /// // Common options
        /// await Curl.ExecuteAsync("curl -H 'Authorization: Bearer token123' https://api.example.com");
        /// await Curl.ExecuteAsync("curl -o download.pdf https://example.com/file.pdf");
        /// await Curl.ExecuteAsync("curl -L --max-time 30 https://short.link/abc");
        /// </code>
        /// </example>
        /// <param name="command">The curl command line as text, for example <c>curl -s https://api.example.com/users?page=2</c>
        /// or just <c>https://api.example.com</c>. Must contain a URL. Quote values with spaces as the source shell
        /// would (<c>-H 'Accept: application/json'</c>).</param>
        /// <returns>A <see cref="CurlResult"/>: <see cref="CurlResult.StatusCode"/> (the HTTP status; for <c>file://</c> 200, or
        /// 404 when the file does not exist; 0 for a transfer skipped by <c>--skip-existing</c>), <see cref="CurlResult.Body"/> (standard output text), <see cref="CurlResult.Headers"/>
        /// (response headers, case-insensitive), <see cref="CurlResult.BinaryData"/> (for non-text responses),
        /// <see cref="CurlResult.OutputFiles"/>, <see cref="CurlResult.Timings"/> and <see cref="CurlResult.Command"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="command"/> is null, empty or only whitespace. Fix: pass a command
        /// that contains a URL. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md</exception>
        /// <exception cref="CurlInvalidCommandException">A <c>--resolve</c> or <c>--connect-to</c> value curl cannot parse
        /// (<see cref="CurlOptionSyntaxException"/>, exit code 49), a command with several URL groups but no URL, or shell
        /// syntax the chosen <see cref="CurlFlavor"/> cannot read. The message names the option. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <exception cref="CurlException">Thrown as the base type with exit code 2 for an option missing its value
        /// (<c>-H</c> at the end) or a value that is not a number (<c>--max-filesize abc</c>), and with exit code 3 for a
        /// command without any URL. The message names the option and shows a valid value. (Invalid numbers for
        /// <c>-m</c>, <c>--connect-timeout</c>, <c>--max-redirs</c> and <c>--retry</c> are currently ignored rather than
        /// reported.) See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <exception cref="CurlMalformedUrlException">A URL that cannot be parsed (exit code 3). Fix: check the
        /// scheme and host, percent-encode spaces. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</exception>
        /// <exception cref="CurlUnsupportedProtocolException">A scheme CurlDotNet does not implement, such as
        /// <c>gopher://</c> (exit code 1). See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/supported-protocols.md</exception>
        /// <exception cref="CurlCouldntResolveHostException">DNS could not resolve the host (exit code 6). Fix: check the
        /// host name; <c>--resolve host:443:1.2.3.4</c> skips DNS. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md</exception>
        /// <exception cref="CurlCouldntConnectException">Nothing accepted the connection on that host and port, or the
        /// proxy was unreachable (exit code 7). See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md</exception>
        /// <exception cref="CurlTimeoutException"><c>-m</c>/<c>--max-time</c>, <c>--connect-timeout</c> or
        /// <see cref="DefaultMaxTimeSeconds"/> expired (exit code 28; thrown as <see cref="CurlOperationTimeoutException"/>).
        /// Fix: raise the limit. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/timeout-errors.md</exception>
        /// <exception cref="CurlSslException">The TLS handshake failed (exit code 35) or the certificate is not trusted
        /// (exit code 60, <see cref="CurlPeerFailedVerificationException"/>). Fix: trust the CA with <c>--cacert</c>; use
        /// <c>-k</c> for testing only. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md</exception>
        /// <exception cref="CurlHttpReturnedErrorException">Only with <c>-f</c>/<c>--fail</c> or <c>--fail-with-body</c>:
        /// the server answered 400 or above (exit code 22). <see cref="Exceptions.CurlHttpException.StatusCode"/> has the status.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md</exception>
        /// <exception cref="CurlTooManyRedirectsException">With <c>-L</c>, more redirects than <c>--max-redirs</c> allows
        /// (exit code 47). See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/redirect-errors.md</exception>
        /// <exception cref="CurlWriteErrorException">An output file (<c>-o</c>, <c>-O</c>, <c>-D</c>, <c>-c</c>) could
        /// not be written, for example because its folder does not exist (exit code 23). (<c>--trace</c> creates its
        /// folder itself; an <c>--etag-save</c> folder that does not exist skips the transfer with a warning on standard error.) Fix: create the folder or add
        /// <c>--create-dirs</c>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <exception cref="CurlReadErrorException">A file named with <c>@</c> (<c>-d @body.json</c>, <c>-F file=@a.png</c>,
        /// <c>-T</c>) could not be read (exit code 26); a <c>-d @file</c> that does not exist at all is
        /// <see cref="CurlFileCouldntReadException"/> (exit code 37). See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <exception cref="CurlException">Any other curl failure; <see cref="CurlException.CurlErrorCode"/> is curl's exit
        /// code and the message says what happened, how to fix it, and links the page for that code. Full table:
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="Execute(string)">Blocking version</seealso>
        /// <seealso cref="ExecuteAsync(string, CancellationToken)">With cancellation</seealso>
        /// <seealso cref="ExecuteAsync(string, CurlFlavor)">Command from a specific shell</seealso>
        /// <seealso cref="ExecuteAsync(string, CurlSettings)">With timeouts, retries and progress set in code</seealso>
        /// <seealso cref="ExecuteAsync(CurlOptions)">Typed options, no command string</seealso>
        /// <seealso cref="ExecuteManyAsync(string[])">Several commands in parallel</seealso>
        /// <seealso cref="Validate(string)">Check a command without sending it</seealso>
        /// <seealso cref="CurlRequestBuilder">Fluent builder equivalent</seealso>
        /// <seealso cref="Lib.LibCurl.PerformAsync(CurlOptions, CancellationToken)">libcurl-style equivalent</seealso>
        /// <seealso cref="DotNetCurl.CurlAsync(string)">Same method under the DotNetCurl name</seealso>
        /// <seealso href="https://curl.se/docs/manpage.html">curl manual</seealso>
        public static async Task<CurlResult> ExecuteAsync(string command)
        {
            RequireCommand(command, "Curl.ExecuteAsync(command)");
            return await _engine.ExecuteAsync(command);
        }

        /// <summary>
        /// Runs a curl command like <see cref="ExecuteAsync(string)"/> and stops it when <paramref name="cancellationToken"/>
        /// is cancelled, for user "Cancel" buttons, request aborts in ASP.NET Core and your own deadlines.
        /// </summary>
        /// <remarks>
        /// <para>Cancelling the token stops the transfer at once (during connect, while waiting for headers or while
        /// reading the body) and throws <see cref="OperationCanceledException"/> - the standard .NET contract, so
        /// <c>catch (OperationCanceledException)</c> works. A limit in the command itself (<c>-m 30</c>) or
        /// <see cref="DefaultMaxTimeSeconds"/> is different: it throws <see cref="CurlOperationTimeoutException"/> (exit
        /// code 28). Files already written by <c>-o</c> before cancellation may be incomplete.</para>
        /// <para>Everything else (the command format, the result, the other exceptions) is the same as
        /// <see cref="ExecuteAsync(string)"/>.</para>
        /// <para>Example: <c>using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));</c> then
        /// <c>var result = await Curl.ExecuteAsync("curl -o big.zip https://example.com/big.zip", cts.Token);</c> - after
        /// 30 seconds it throws <see cref="OperationCanceledException"/>.</para>
        /// <para>Example in ASP.NET Core: <c>public async Task&lt;IActionResult&gt; Proxy(CancellationToken ct) =&gt; Ok((await Curl.ExecuteAsync("curl https://upstream.example.com/data", ct)).Body);</c>
        /// stops the upstream call when the browser disconnects.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// private CancellationTokenSource _downloadCts;
        ///
        /// async Task StartDownload()
        /// {
        ///     _downloadCts = new CancellationTokenSource();
        ///     try
        ///     {
        ///         await Curl.ExecuteAsync("curl -o large-file.zip https://example.com/huge-file.zip", _downloadCts.Token);
        ///         MessageBox.Show("Download complete!");
        ///     }
        ///     catch (OperationCanceledException)
        ///     {
        ///         MessageBox.Show("Download cancelled by user");
        ///     }
        /// }
        ///
        /// void CancelButton_Click() =&gt; _downloadCts?.Cancel();
        /// </code>
        /// </example>
        /// <param name="command">The curl command line as text, exactly as for <see cref="ExecuteAsync(string)"/>,
        /// for example <c>curl -L https://example.com/file.zip -o file.zip</c>.</param>
        /// <param name="cancellationToken">Cancels the request when signalled: from a
        /// <c>CancellationTokenSource</c> (optionally with a timeout, <c>new CancellationTokenSource(TimeSpan.FromSeconds(30))</c>),
        /// an ASP.NET Core action parameter, or <c>CancellationToken.None</c> for no cancellation.</param>
        /// <returns>The same <see cref="CurlResult"/> as <see cref="ExecuteAsync(string)"/>.</returns>
        /// <exception cref="OperationCanceledException">The token was cancelled before the transfer finished. Fix: nothing
        /// to fix if you cancelled on purpose; otherwise use a later deadline. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</exception>
        /// <exception cref="ArgumentException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlException">The transfer failed; the subtypes and exit codes are listed on
        /// <see cref="ExecuteAsync(string)"/>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="Execute(string, CancellationToken)">Blocking version</seealso>
        /// <seealso cref="ExecuteAsync(string)"/>
        /// <seealso cref="ExecuteAsync(CurlOptions, CancellationToken)"/>
        /// <seealso cref="CurlRequestBuilder.ExecuteAsync(CancellationToken)"/>
        /// <seealso cref="DotNetCurl.CurlAsync(string, CancellationToken)"/>
        public static async Task<CurlResult> ExecuteAsync(string command, CancellationToken cancellationToken)
        {
            RequireCommand(command, "Curl.ExecuteAsync(command, cancellationToken)");
            return await _engine.ExecuteAsync(command, cancellationToken);
        }

        /// <summary>
        /// Runs a curl command that was copied from a specific shell (bash, cmd.exe, PowerShell or
        /// Invoke-WebRequest), reading its quotes, escapes, line continuations and variables the way that shell would.
        /// </summary>
        /// <param name="command">The command exactly as it was pasted, with or without the leading <c>curl</c>,
        /// <c>curl.exe</c> or (for <see cref="CurlFlavor.InvokeWebRequest"/>) <c>Invoke-WebRequest</c>.</param>
        /// <param name="flavor">
        /// Where the command came from: <see cref="CurlFlavor.Posix"/> for bash/zsh (macOS, Linux, Git Bash, WSL);
        /// <see cref="CurlFlavor.WindowsCmd"/> for <c>curl.exe</c> in the Windows Command Prompt;
        /// <see cref="CurlFlavor.PowerShell"/> for <c>curl.exe</c> in PowerShell;
        /// <see cref="CurlFlavor.InvokeWebRequest"/> for PowerShell's <c>Invoke-WebRequest</c> / <c>Invoke-RestMethod</c>
        /// syntax (also the <c>curl</c>, <c>iwr</c> and <c>irm</c> aliases); <see cref="CurlFlavor.Auto"/> to detect it.
        /// Overrides <see cref="DefaultFlavor"/> for this call.
        /// </param>
        /// <returns>The <see cref="CurlResult"/> of the request, as described on <see cref="ExecuteAsync(string)"/>. To see
        /// which flavor and which parsed values a command produces before sending it, use
        /// <see cref="Validate(string, CurlFlavor)"/> and read <see cref="CurlOptions.Flavor"/> on its <c>ParsedOptions</c>.</returns>
        /// <exception cref="ArgumentException">The command is null, empty or only whitespace. Fix: pass the pasted text.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The flavor is not a defined <see cref="CurlFlavor"/>.</exception>
        /// <exception cref="CurlInvalidCommandException">The text breaks the flavor's rules (for example an
        /// unterminated quote), or uses an <c>Invoke-WebRequest</c> parameter that cannot be translated, such as
        /// <c>-Credential</c>. The message says how to rewrite it. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md</exception>
        /// <exception cref="CurlException">The request itself fails, exactly as for <see cref="ExecuteAsync(string)"/>.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <remarks>
        /// <para>The same characters mean different bytes in different shells. With
        /// <c>-d '{\"a\":1}'</c>, <see cref="CurlFlavor.PowerShell"/> sends <c>{"a":1}</c> (the Windows C runtime turns
        /// <c>\"</c> into a quote) while <see cref="CurlFlavor.Posix"/> sends <c>{\"a\":1}</c> with the backslashes.
        /// With <c>"%TOKEN%"</c>, <see cref="CurlFlavor.WindowsCmd"/> substitutes the environment variable and the
        /// other flavors do not.</para>
        /// <para>Example: <c>var result = await Curl.ExecuteAsync("curl.exe -H 'Content-Type: application/json' -d '{\"a\":1}' https://api.example.com", CurlFlavor.PowerShell);</c></para>
        /// <para>Example: <c>var result = await Curl.ExecuteAsync("Invoke-RestMethod -Uri https://api.example.com -Method Post -Body '{\"a\":1}' -ContentType 'application/json'", CurlFlavor.InvokeWebRequest);</c></para>
        /// <para>Use <see cref="DetectFlavor(string)"/> to see what <see cref="CurlFlavor.Auto"/> would choose, and
        /// <see cref="Validate(string, CurlFlavor)"/> to check a command without sending it.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md</para>
        /// </remarks>
        /// <seealso cref="Execute(string, CurlFlavor)">Blocking version</seealso>
        /// <seealso cref="ExecuteAsync(string, CurlFlavor, CancellationToken)">With cancellation</seealso>
        /// <seealso cref="DefaultFlavor"/>
        /// <seealso cref="CurlSettings.WithFlavor(CurlFlavor)"/>
        /// <seealso href="https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md">curl flavors guide</seealso>
        public static async Task<CurlResult> ExecuteAsync(string command, CurlFlavor flavor)
        {
            RequireCommand(command, "Curl.ExecuteAsync(command, flavor)");
            return await _engine.ExecuteAsync(command, flavor, CancellationToken.None);
        }

        /// <summary>
        /// Runs a curl command copied from a specific shell, like <see cref="ExecuteAsync(string, CurlFlavor)"/>, and
        /// stops it when <paramref name="cancellationToken"/> is cancelled.
        /// </summary>
        /// <param name="command">The command exactly as it was pasted, for example
        /// <c>curl.exe -H "Accept: application/json" https://api.example.com</c>.</param>
        /// <param name="flavor">The shell it was written for: <see cref="CurlFlavor.Posix"/>, <see cref="CurlFlavor.WindowsCmd"/>,
        /// <see cref="CurlFlavor.PowerShell"/>, <see cref="CurlFlavor.InvokeWebRequest"/> or <see cref="CurlFlavor.Auto"/>
        /// (see <see cref="ExecuteAsync(string, CurlFlavor)"/>). Overrides <see cref="DefaultFlavor"/> for this call.</param>
        /// <param name="cancellationToken">Cancels the request when signalled; <c>CancellationToken.None</c> for no
        /// cancellation.</param>
        /// <returns>The <see cref="CurlResult"/>, as described on <see cref="ExecuteAsync(string)"/>.</returns>
        /// <exception cref="OperationCanceledException">The token was cancelled before the transfer finished. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</exception>
        /// <exception cref="ArgumentException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlInvalidCommandException">The text breaks the flavor's rules or uses an
        /// untranslatable <c>Invoke-WebRequest</c> parameter; the message says how to rewrite it. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md</exception>
        /// <exception cref="CurlException">The transfer failed, as for <see cref="ExecuteAsync(string)"/>.</exception>
        /// <remarks>
        /// <para>Example: <c>using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));</c> then
        /// <c>var result = await Curl.ExecuteAsync(pasted, CurlFlavor.WindowsCmd, cts.Token);</c>.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md</para>
        /// </remarks>
        /// <seealso cref="ExecuteAsync(string, CurlFlavor)"/>
        /// <seealso cref="Execute(string, CurlFlavor)"/>
        /// <seealso cref="ExecuteAsync(string, CancellationToken)"/>
        public static async Task<CurlResult> ExecuteAsync(string command, CurlFlavor flavor, CancellationToken cancellationToken)
        {
            RequireCommand(command, "Curl.ExecuteAsync(command, flavor, cancellationToken)");
            return await _engine.ExecuteAsync(command, flavor, cancellationToken);
        }

        /// <summary>
        /// Returns the shell flavor <see cref="CurlFlavor.Auto"/> would read <paramref name="command"/> with, without
        /// parsing or sending anything.
        /// </summary>
        /// <param name="command">The command text as pasted. Null or empty text returns <see cref="CurlFlavor.Posix"/>.</param>
        /// <returns>
        /// <see cref="CurlFlavor.InvokeWebRequest"/> when it starts with <c>Invoke-WebRequest</c>,
        /// <c>Invoke-RestMethod</c>, <c>iwr</c>, <c>irm</c> or <c>wget</c>, or is a <c>curl</c> command using cmdlet
        /// parameters such as <c>-Uri</c>, <c>-Method</c> or <c>-Headers @{...}</c>;
        /// <see cref="CurlFlavor.PowerShell"/> when a line ends in a backtick or it contains <c>$env:</c>,
        /// <c>`"</c> or <c>--%</c>; <see cref="CurlFlavor.WindowsCmd"/> when a line ends in <c>^</c> or it contains
        /// <c>^"</c> or <c>%NAME%</c> naming a set environment variable; otherwise <see cref="CurlFlavor.Posix"/>.
        /// Never returns <see cref="CurlFlavor.Auto"/>.
        /// </returns>
        /// <remarks>
        /// <para>Never throws. Use it in diagnostics, for example
        /// <c>logger.LogDebug("Running {Flavor} command {Command}", Curl.DetectFlavor(pasted), pasted);</c>, or to
        /// show users which shell rules their command will be read with before running it. When the guess is wrong,
        /// pass the right flavor to <see cref="ExecuteAsync(string, CurlFlavor)"/>.</para>
        /// <para>Example: <c>Curl.DetectFlavor("curl -H ^\"Accept: */*^\" https://x.example")</c> returns
        /// <see cref="CurlFlavor.WindowsCmd"/>.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md</para>
        /// </remarks>
        /// <seealso cref="DefaultFlavor"/>
        /// <seealso cref="CurlOptions.Flavor"/>
        /// <seealso cref="Validate(string, CurlFlavor)"/>
        public static CurlFlavor DetectFlavor(string command)
        {
            return string.IsNullOrWhiteSpace(command) ? CurlFlavor.Posix : CommandParser.DetectFlavor(command);
        }

        /// <summary>
        /// Runs a curl command with timeouts, retries, extra headers, a progress callback or a cancellation token set
        /// in code through a <see cref="CurlSettings"/> object instead of in the command text.
        /// </summary>
        /// <remarks>
        /// <para><b>Settings that this method applies</b> (each overrides the same option in the command):
        /// <see cref="CurlSettings.MaxTimeSeconds"/> (<c>-m</c>), <see cref="CurlSettings.ConnectTimeoutSeconds"/>
        /// (<c>--connect-timeout</c>), <see cref="CurlSettings.FollowRedirects"/> (<c>-L</c>),
        /// <see cref="CurlSettings.Insecure"/> (<c>-k</c>), <see cref="CurlSettings.Headers"/> (one <c>-H</c> each;
        /// replaces a header of the same name), <see cref="CurlSettings.OnProgress"/> (called for every chunk of the
        /// response body with the percentage or -1 when the size is unknown, the bytes received so far, and the total
        /// size or -1), <see cref="CurlSettings.RetryCount"/> with <see cref="CurlSettings.RetryDelayMs"/> (retries any
        /// failure, like <c>--retry N --retry-delay S --retry-all-errors</c>), <see cref="CurlSettings.Flavor"/> (how the
        /// command text is read; default <see cref="DefaultFlavor"/>) and <see cref="CurlSettings.CancellationToken"/>.</para>
        /// <para>The other <see cref="CurlSettings"/> properties (<c>Proxy</c>, <c>UserAgent</c>, <c>Cookies</c>,
        /// <c>OnRedirect</c>, <c>BufferSize</c>, <c>AutomaticDecompression</c>) are not used by this method; put
        /// <c>-x</c>, <c>-A</c>, <c>-b</c>/<c>-c</c> or <c>--compressed</c> in the command instead.</para>
        /// <para>A progress callback that throws aborts the transfer with <see cref="CurlAbortedByCallbackException"/>
        /// (exit code 42). Cancelling the settings' token throws <see cref="OperationCanceledException"/>.</para>
        /// <para>Example: <c>var settings = new CurlSettings().WithTimeout(10).WithRetries(3, 500).WithProgress((pct, got, total) =&gt; Console.Write($"\r{pct:F0}%"));</c>
        /// then <c>var result = await Curl.ExecuteAsync("curl -o big.zip https://example.com/big.zip", settings);</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// var settings = new CurlSettings()
        ///     .WithTimeout(30)
        ///     .WithRetries(count: 3, delayMs: 2000)
        ///     .WithHeader("X-Request-Id", Guid.NewGuid().ToString())
        ///     .WithProgress((percent, received, total) =&gt;
        ///         Console.WriteLine(total &gt; 0 ? $"{percent:F1}%" : $"{received} bytes"));
        ///
        /// var result = await Curl.ExecuteAsync("curl -O https://example.com/large-file.zip", settings);
        /// </code>
        /// </example>
        /// <param name="command">The curl command line as text, as for <see cref="ExecuteAsync(string)"/>.</param>
        /// <param name="settings">The settings to apply on top of the command, built with <c>new CurlSettings().With...()</c>
        /// or <see cref="CurlSettings.FromDefaults"/>. Null runs the command unchanged (no cancellation).</param>
        /// <returns>The <see cref="CurlResult"/>, as described on <see cref="ExecuteAsync(string)"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="OperationCanceledException"><see cref="CurlSettings.CancellationToken"/> was cancelled. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</exception>
        /// <exception cref="CurlAbortedByCallbackException">The <see cref="CurlSettings.OnProgress"/> callback threw (exit
        /// code 42); the callback's exception is the <see cref="Exception.InnerException"/>.</exception>
        /// <exception cref="CurlTimeoutException">The settings' (or the command's) time limit expired (exit code 28). See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/timeout-errors.md</exception>
        /// <exception cref="CurlException">The transfer failed after all retries, as for <see cref="ExecuteAsync(string)"/>.</exception>
        /// <seealso cref="Execute(string, CurlSettings)">Blocking version</seealso>
        /// <seealso cref="CurlSettings"/>
        /// <seealso cref="CurlRequestBuilder.ExecuteAsync(CurlSettings, CancellationToken)">Builder equivalent</seealso>
        /// <seealso cref="DotNetCurl.CurlAsync(string, CurlSettings)"/>
        public static async Task<CurlResult> ExecuteAsync(string command, CurlSettings settings)
        {
            RequireCommand(command, "Curl.ExecuteAsync(command, settings)");
            return await _engine.ExecuteAsync(command, settings);
        }

        /// <summary>
        /// Sends a request described by a <see cref="CurlOptions"/> object instead of a command string, so values from
        /// variables are sent exactly as given with no shell quoting to get wrong.
        /// </summary>
        /// <remarks>
        /// <para>Nothing is parsed: <see cref="CurlOptions.Data"/> containing quotes, spaces or backslashes is sent
        /// byte for byte. Set at least <see cref="CurlOptions.Url"/>; every other property mirrors a curl option
        /// (<see cref="CurlOptions.Method"/> is <c>-X</c>, <see cref="CurlOptions.Headers"/> is <c>-H</c>,
        /// <see cref="CurlOptions.FollowLocation"/> is <c>-L</c>, <see cref="CurlOptions.MaxTime"/> is <c>-m</c>...).
        /// The quick helpers <see cref="PostAsync"/>, <see cref="PostJsonAsync"/> and <see cref="DownloadAsync"/> are
        /// built on this method. The <c>Default*</c> properties of this class still apply when the options leave them
        /// unset.</para>
        /// <para>To start from a command and adjust it, parse it with <see cref="Validate(string)"/> and use
        /// <c>ParsedOptions</c>.</para>
        /// <para>Example: <c>var result = await Curl.ExecuteAsync(new CurlOptions { Url = "https://api.example.com/users", Method = "POST", Data = "{\"name\":\"O'Brien\"}", Headers = { ["Content-Type"] = "application/json" } });</c></para>
        /// <para>Mapping of every option to libcurl: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/reference/libcurl-mapping.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// var options = new CurlOptions
        /// {
        ///     Url = "https://api.example.com/users",
        ///     Method = "POST",
        ///     Data = "{\"name\":\"O'Brien\"}",
        ///     Headers = { ["Content-Type"] = "application/json" },
        ///     FollowLocation = true
        /// };
        ///
        /// var result = await Curl.ExecuteAsync(options);
        /// </code>
        /// </example>
        /// <param name="options">The request to send; <see cref="CurlOptions.Url"/> must be set to an absolute URL such
        /// as <c>https://api.example.com/v1/items</c>. The object may be modified while the request runs (URL
        /// normalisation, defaults), so do not share one instance between concurrent calls.</param>
        /// <returns>The <see cref="CurlResult"/>, as described on <see cref="ExecuteAsync(string)"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is null. Fix: pass a <c>new CurlOptions { Url = ... }</c>.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md</exception>
        /// <exception cref="CurlException"><see cref="CurlOptions.Url"/> is empty (exit code 3), or the transfer failed;
        /// the subtypes are listed on <see cref="ExecuteAsync(string)"/>. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="Execute(CurlOptions)">Blocking version</seealso>
        /// <seealso cref="ExecuteAsync(CurlOptions, CancellationToken)">With cancellation</seealso>
        /// <seealso cref="Lib.LibCurl.PerformAsync(CurlOptions, CancellationToken)">libcurl-style equivalent</seealso>
        /// <seealso cref="CurlRequestBuilder.GetOptions"/>
        public static async Task<CurlResult> ExecuteAsync(CurlOptions options)
        {
            RequireOptions(options, "Curl.ExecuteAsync(options)");
            return await _engine.ExecuteAsync(options);
        }

        /// <summary>
        /// Sends a request described by a <see cref="CurlOptions"/> object, like <see cref="ExecuteAsync(CurlOptions)"/>,
        /// and stops it when <paramref name="cancellationToken"/> is cancelled.
        /// </summary>
        /// <remarks>
        /// <para>Cancelling throws <see cref="OperationCanceledException"/>; a <see cref="CurlOptions.MaxTime"/> limit
        /// throws <see cref="CurlOperationTimeoutException"/> (exit code 28) instead.</para>
        /// <para>Example: <c>using var cts = new CancellationTokenSource(5000);</c> then
        /// <c>var result = await Curl.ExecuteAsync(new CurlOptions { Url = "https://api.example.com/health" }, cts.Token);</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</para>
        /// </remarks>
        /// <param name="options">The request to send; <see cref="CurlOptions.Url"/> must be set. Do not share one instance
        /// between concurrent calls.</param>
        /// <param name="cancellationToken">Cancels the request when signalled; <c>CancellationToken.None</c> for no
        /// cancellation.</param>
        /// <returns>The <see cref="CurlResult"/>, as described on <see cref="ExecuteAsync(string)"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is null.</exception>
        /// <exception cref="OperationCanceledException">The token was cancelled before the transfer finished.</exception>
        /// <exception cref="CurlException">No URL (exit code 3) or the transfer failed; see <see cref="ExecuteAsync(string)"/>.</exception>
        /// <seealso cref="ExecuteAsync(CurlOptions)"/>
        /// <seealso cref="Execute(CurlOptions)"/>
        /// <seealso cref="Lib.LibCurl.PerformAsync(CurlOptions, CancellationToken)"/>
        public static async Task<CurlResult> ExecuteAsync(CurlOptions options, CancellationToken cancellationToken)
        {
            RequireOptions(options, "Curl.ExecuteAsync(options, cancellationToken)");
            return await _engine.ExecuteAsync(options, cancellationToken);
        }

        /// <summary>Throws a helpful ArgumentException (the type these methods have always thrown) for a missing command string.</summary>
        private static void RequireCommand(string command, string method)
        {
            if (command == null)
            {
                throw new ArgumentException(CurlException.Compose(
                    $"{method} was called with a null command.",
                    "Pass the curl command line as text, for example \"curl https://api.example.com\" (the leading \"curl\" is optional).",
                    DocLinks.HowToRunCommand), nameof(command));
            }

            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentException(CurlException.Compose(
                    $"{method} was called with an empty command (\"{command}\").",
                    "Pass a curl command that contains at least a URL, for example \"curl https://api.example.com\"; check that the variable holding the command was filled in.",
                    DocLinks.HowToRunCommand), nameof(command));
            }
        }

        /// <summary>Throws a helpful ArgumentNullException for missing CurlOptions.</summary>
        private static void RequireOptions(CurlOptions options, string method)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options), CurlException.Compose(
                    $"{method} was called with null options.",
                    "Pass a CurlOptions object with at least Url set, for example new CurlOptions { Url = \"https://api.example.com\" }, or use the command-string overload.",
                    DocLinks.HowToRunCommand));
            }
        }

        /// <summary>
        /// Builds the <see cref="CurlOptions"/> for the quick helper methods directly, bypassing the
        /// command parser so user data never has to be shell-quoted. <see cref="CurlOptions.OriginalCommand"/>
        /// is still populated with an equivalent, correctly quoted command for diagnostics.
        /// </summary>
        private static CurlOptions BuildQuickOptions(string url, string? method, string? data, string? contentType, string? outputFile, string helper)
        {
            if (url == null)
            {
                throw new ArgumentException(CurlException.Compose(
                    $"Curl.{helper} was called with a null URL.",
                    "Pass an absolute URL such as \"https://api.example.com/items\"; check that the variable holding it was set.",
                    DocLinks.Url), nameof(url));
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException(CurlException.Compose(
                    $"Curl.{helper} was called with an empty URL (\"{url}\").",
                    "Pass an absolute URL such as \"https://api.example.com/items\", including the scheme (https://) and host.",
                    DocLinks.Url), nameof(url));
            }

            var options = new CurlOptions
            {
                Url = url.Trim(),
                Method = method,
                Data = data,
                OutputFile = outputFile
            };

            var parts = new List<string> { "curl" };
            if (!string.IsNullOrEmpty(method))
            {
                parts.Add("-X " + method);
            }

            if (!string.IsNullOrEmpty(contentType))
            {
                options.Headers["Content-Type"] = contentType!;
                parts.Add("-H " + CommandParser.ShellQuote("Content-Type: " + contentType));
            }

            if (data != null)
            {
                parts.Add("-d " + CommandParser.ShellQuote(data));
            }

            if (!string.IsNullOrEmpty(outputFile))
            {
                parts.Add("-o " + CommandParser.ShellQuoteIfNeeded(outputFile));
            }

            parts.Add(CommandParser.ShellQuoteIfNeeded(options.Url));
            options.OriginalCommand = string.Join(" ", parts);
            return options;
        }

        /// <summary>
        /// Sends a plain GET request to <paramref name="url"/> (like <c>curl URL</c>) and returns the response; use it
        /// when you need no headers, auth or other options.
        /// </summary>
        /// <remarks>
        /// <para>The URL is sent as given; nothing is parsed or shell-quoted, so <c>&amp;</c>, <c>?</c> and quotes in it
        /// need no escaping. The <c>Default*</c> properties (timeouts, <see cref="DefaultFollowRedirects"/>,
        /// <see cref="DefaultUserAgent"/>) apply. HTTP errors are returned, not thrown: check
        /// <see cref="CurlResult.IsSuccess"/> or call <see cref="CurlResult.EnsureSuccess"/>.</para>
        /// <para>For headers or authentication use <see cref="ExecuteAsync(string)"/>
        /// (<c>curl -H 'Authorization: Bearer TOKEN' URL</c>) or <see cref="CurlRequestBuilder.Get(string)"/>.</para>
        /// <para>Example: <c>var user = (await Curl.GetAsync("https://api.github.com/users/octocat")).ParseJson&lt;GitHubUser&gt;();</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// var response = await Curl.GetAsync("https://api.github.com/users/octocat");
        /// var user = response.ParseJson&lt;GitHubUser&gt;();
        /// Console.WriteLine($"Followers: {user.Followers}");
        /// </code>
        /// </example>
        /// <param name="url">An absolute URL including the scheme, for example
        /// <c>https://api.example.com/users?page=1&amp;limit=10</c>; http, https, ftp, ftps and file URLs are supported.</param>
        /// <returns>The <see cref="CurlResult"/>: <see cref="CurlResult.StatusCode"/>, <see cref="CurlResult.Body"/>
        /// (the response text), <see cref="CurlResult.Headers"/> and, for binary content, <see cref="CurlResult.BinaryData"/>.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> is null, empty or whitespace. Fix: pass an absolute URL.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</exception>
        /// <exception cref="CurlMalformedUrlException">The URL cannot be parsed (exit code 3).</exception>
        /// <exception cref="CurlException">The transfer failed (DNS, connect, timeout, TLS...); see
        /// <see cref="ExecuteAsync(string)"/> for the subtypes.</exception>
        /// <seealso cref="Get(string)">Blocking version</seealso>
        /// <seealso cref="CurlRequestBuilder.Get(string)">Builder equivalent</seealso>
        /// <seealso cref="Lib.LibCurl.GetAsync(string, Action{CurlOptions}, CancellationToken)">libcurl-style equivalent</seealso>
        /// <seealso cref="DotNetCurl.GetAsync(string)"/>
        public static async Task<CurlResult> GetAsync(string url)
        {
            return await ExecuteAsync(BuildQuickOptions(url, null, null, null, null, "GetAsync(url)"));
        }

        /// <summary>
        /// Sends a POST request with <paramref name="data"/> as the body (like <c>curl -d DATA URL</c>), with no quoting or
        /// escaping needed for the data.
        /// </summary>
        /// <remarks>
        /// <para>The body is sent exactly as given; quotes, apostrophes, spaces and newlines need no escaping. As with
        /// <c>curl -d</c>, the request has <c>Content-Type: application/x-www-form-urlencoded</c> unless you send
        /// something else; for JSON use <see cref="PostJsonAsync"/>, which sets <c>Content-Type: application/json</c>.
        /// The data is not URL-encoded for you: for form fields write <c>name=value&amp;other=2</c> and encode values
        /// with <c>Uri.EscapeDataString</c>.</para>
        /// <para>HTTP errors are returned, not thrown; check <see cref="CurlResult.IsSuccess"/>.</para>
        /// <para>Example: <c>var result = await Curl.PostAsync("https://api.example.com/login", "username=john&amp;password=" + Uri.EscapeDataString(pw));</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// var response = await Curl.PostAsync("https://api.example.com/login", "username=john&amp;password=secret123");
        /// var created = await Curl.PostAsync("https://api.example.com/notes", "It's \"quoted\" and that's fine");
        /// </code>
        /// </example>
        /// <param name="url">An absolute URL to post to, for example <c>https://api.example.com/users</c>.</param>
        /// <param name="data">The request body as text: form fields (<c>key1=value1&amp;key2=value2</c>), JSON
        /// (<c>{"key":"value"}</c>), XML or anything else. Null sends an empty body.</param>
        /// <returns>The <see cref="CurlResult"/> with the server's status, headers and body.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> is null, empty or whitespace. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</exception>
        /// <exception cref="CurlException">The transfer failed; see <see cref="ExecuteAsync(string)"/>.</exception>
        /// <seealso cref="Post(string, string)">Blocking version</seealso>
        /// <seealso cref="PostJsonAsync(string, object)">Post an object as JSON</seealso>
        /// <seealso cref="CurlRequestBuilder.Post(string)">Builder equivalent</seealso>
        /// <seealso cref="Lib.LibCurl.PostAsync(string, object, Action{CurlOptions}, CancellationToken)">libcurl-style equivalent</seealso>
        /// <seealso cref="DotNetCurl.PostAsync(string, string)"/>
        public static async Task<CurlResult> PostAsync(string url, string data)
        {
            return await ExecuteAsync(BuildQuickOptions(url, "POST", data ?? string.Empty, null, null, "PostAsync(url, data)"));
        }

        /// <summary>
        /// Serializes <paramref name="data"/> to JSON and POSTs it with <c>Content-Type: application/json</c>
        /// (like <c>curl -H 'Content-Type: application/json' -d '{...}' URL</c>).
        /// </summary>
        /// <remarks>
        /// <para>Serialization: System.Text.Json on .NET 8/9/10 and .NET Framework (property names as declared,
        /// apostrophes and non-ASCII characters kept literal rather than <c>\u0027</c>-escaped), Newtonsoft.Json on the
        /// .NET Standard 2.0 build. Anonymous objects, your classes, arrays, lists and dictionaries all work. To control
        /// naming (camelCase) serialize yourself and call <see cref="PostAsync"/> with the string.</para>
        /// <para>HTTP errors are returned, not thrown; check <see cref="CurlResult.IsSuccess"/> and parse the answer
        /// with <see cref="CurlResult.ParseJson{T}"/>.</para>
        /// <para>Example: <c>var result = await Curl.PostJsonAsync("https://api.example.com/users", new { name = "John", age = 30 });</c>
        /// sends the body <c>{"name":"John","age":30}</c>; then <c>var created = result.ParseJson&lt;User&gt;();</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// var response = await Curl.PostJsonAsync("https://api.example.com/users",
        ///     new { name = "John Smith", email = "john@example.com", age = 30 });
        ///
        /// if (response.IsSuccess)
        /// {
        ///     var created = response.ParseJson&lt;User&gt;();
        ///     Console.WriteLine($"User created with ID: {created.Id}");
        /// }
        ///
        /// await Curl.PostJsonAsync(url, new Dictionary&lt;string, object&gt; { ["key"] = "value" });
        /// </code>
        /// </example>
        /// <param name="url">An absolute URL to post to, for example <c>https://api.example.com/users</c>.</param>
        /// <param name="data">The object to send as JSON: an anonymous object (<c>new { name = "John" }</c>), an instance
        /// of your class, an array/list or a <c>Dictionary&lt;string, object&gt;</c>. Null sends the JSON literal <c>null</c>.</param>
        /// <returns>The <see cref="CurlResult"/> with the server's status, headers and body.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlParsingException"><paramref name="data"/> cannot be serialized to JSON (for example it
        /// contains a reference cycle or a type the serializer does not support); the serializer's exception is the
        /// <see cref="Exception.InnerException"/>. Fix: send a simpler object (a DTO) or serialize it yourself. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</exception>
        /// <exception cref="CurlException">The transfer failed; see <see cref="ExecuteAsync(string)"/>.</exception>
        /// <seealso cref="PostJson(string, object)">Blocking version</seealso>
        /// <seealso cref="PostAsync(string, string)">Post a string body</seealso>
        /// <seealso cref="CurlRequestBuilder.WithJson(object)">Builder equivalent</seealso>
        /// <seealso cref="CurlResult.ParseJson{T}">Parse the JSON answer</seealso>
        /// <seealso cref="DotNetCurl.PostJsonAsync(string, object)"/>
        public static async Task<CurlResult> PostJsonAsync(string url, object data)
        {
            var json = SerializeJsonForPost(data, url);
            return await ExecuteAsync(BuildQuickOptions(url, "POST", json, "application/json", null, "PostJsonAsync(url, data)"));
        }

        /// <summary>Serializes the PostJson payload, turning serializer failures into a CurlParsingException.</summary>
        private static string SerializeJsonForPost(object data, string url) =>
            SerializeJsonOrThrow(data, "Curl.PostJsonAsync", url, "or serialize it yourself and call Curl.PostAsync(url, json)");

        /// <summary>
        /// Serializes a request body to JSON (System.Text.Json, or Newtonsoft.Json on .NET Standard 2.0), turning a
        /// serializer failure (reference cycle, unsupported member type) into a <see cref="CurlParsingException"/> that
        /// names the API, the object type and the URL.
        /// </summary>
        internal static string SerializeJsonOrThrow(object data, string api, string? url, string alternative)
        {
            try
            {
                return SerializeJson(data);
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException))
            {
                throw new CurlParsingException(CurlException.Compose(
                    $"{api} could not serialize the {data?.GetType().Name ?? "null"} object to JSON for {ErrorText.SafeUrl(url)}: {ex.Message}",
                    $"Send a plain data object (a DTO without reference cycles, delegates or streams), {alternative}.",
                    DocLinks.HowToPostJson),
                    "application/json", data?.GetType() ?? typeof(object), null, ex);
            }
        }

        /// <summary>
        /// Downloads <paramref name="url"/> and saves the response body to <paramref name="outputPath"/> (like
        /// <c>curl -o PATH URL</c>); paths with spaces or quotes need no escaping.
        /// </summary>
        /// <remarks>
        /// <para>The folder of <paramref name="outputPath"/> must exist (curl's <c>--create-dirs</c> is not added); an
        /// existing file is overwritten. The file is written even for an HTTP error status, exactly like
        /// <c>curl -o</c> without <c>-f</c>, so check <see cref="CurlResult.IsSuccess"/> afterwards (and delete the file
        /// if it holds an error page). Redirects are followed only when <see cref="DefaultFollowRedirects"/> is true; for
        /// download links that redirect, use <c>await Curl.ExecuteAsync("curl -L -f -o file.zip URL")</c>.</para>
        /// <para>The whole body is also kept in memory (<see cref="CurlResult.BinaryData"/> or <see cref="CurlResult.Body"/>).
        /// For a progress bar use <see cref="ExecuteAsync(string, CurlSettings)"/> with
        /// <see cref="CurlSettings.WithProgress"/>.</para>
        /// <para>Example: <c>var result = await Curl.DownloadAsync("https://example.com/manual.pdf", @"C:\Downloads\manual.pdf");</c>
        /// then <c>result.OutputFiles[0]</c> is the path written.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// try
        /// {
        ///     var result = await Curl.DownloadAsync("https://example.com/manual.pdf", "downloads/manual.pdf");
        ///     if (!result.IsSuccess) Console.WriteLine($"Server answered {result.StatusCode}");
        /// }
        /// catch (CurlWriteErrorException ex)
        /// {
        ///     Console.WriteLine(ex.Message); // folder missing or not writable, and how to fix it
        /// }
        /// </code>
        /// </example>
        /// <param name="url">An absolute URL of the file, for example <c>https://example.com/files/report.pdf</c>.</param>
        /// <param name="outputPath">Where to save it: an absolute path (<c>@"C:\Downloads\file.pdf"</c>,
        /// <c>/tmp/file.pdf</c>), a relative path (<c>downloads/file.pdf</c>, relative to the current directory) or just a
        /// file name. Its folder must already exist.</param>
        /// <returns>The <see cref="CurlResult"/>: <see cref="CurlResult.StatusCode"/>, <see cref="CurlResult.Headers"/>,
        /// <see cref="CurlResult.OutputFiles"/> (contains <paramref name="outputPath"/>). The body is in the file, not in
        /// <see cref="CurlResult.Body"/> (empty); <see cref="CurlResult.BinaryData"/> also holds it for binary downloads.</returns>
        /// <exception cref="ArgumentException"><paramref name="url"/> or <paramref name="outputPath"/> is null, empty or whitespace.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</exception>
        /// <exception cref="CurlWriteErrorException">The file could not be written: its folder does not exist, the path is
        /// invalid, or the disk is full or read-only (exit code 23). Fix: create the folder first. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <exception cref="CurlException">The transfer failed (DNS, connect, timeout, TLS...); see <see cref="ExecuteAsync(string)"/>.</exception>
        /// <seealso cref="Download(string, string)">Blocking version</seealso>
        /// <seealso cref="CurlRequestBuilder.WithOutput(string)">Builder equivalent</seealso>
        /// <seealso cref="Lib.LibCurl.WithOutputFile(string)">libcurl-style equivalent</seealso>
        /// <seealso cref="CurlResult.SaveToFile(string)">Save a response you already have</seealso>
        /// <seealso cref="DotNetCurl.DownloadAsync(string, string)"/>
        public static async Task<CurlResult> DownloadAsync(string url, string outputPath)
        {
            if (outputPath == null)
            {
                throw new ArgumentException(CurlException.Compose(
                    $"Curl.DownloadAsync(url, outputPath) was called with a null output path for {ErrorText.SafeUrl(url)}.",
                    "Pass the file to save to, for example \"downloads/report.pdf\" or an absolute path; its folder must exist.",
                    DocLinks.HowToDownload), nameof(outputPath));
            }

            if (string.IsNullOrWhiteSpace(outputPath))
            {
                throw new ArgumentException(CurlException.Compose(
                    $"Curl.DownloadAsync(url, outputPath) was called with an empty output path (\"{outputPath}\") for {ErrorText.SafeUrl(url)}.",
                    "Pass the file to save to, for example \"downloads/report.pdf\"; to keep the body in memory only, use Curl.GetAsync(url) instead.",
                    DocLinks.HowToDownload), nameof(outputPath));
            }

            return await ExecuteAsync(BuildQuickOptions(url, null, null, null, outputPath, "DownloadAsync(url, outputPath)"));
        }

        /// <summary>
        /// Runs several curl commands at the same time and returns their results in the same order, so N requests take
        /// about as long as the slowest one instead of the sum.
        /// </summary>
        /// <remarks>
        /// <para>All commands start immediately (there is no concurrency limit), share the connection pool, and run in
        /// parallel. The returned array has one <see cref="CurlResult"/> per command, in the order given.</para>
        /// <para><b>Failures:</b> an HTTP error status is not a failure (that result just has its status code). If any
        /// command throws (DNS failure, timeout, <c>-f</c> with a 404...), this method throws that exception after all
        /// commands have finished, and the results of the successful ones are not returned. To keep every outcome, run
        /// the tasks yourself: <c>var tasks = commands.Select(c =&gt; Curl.ExecuteAsync(c)).ToList(); try { await Task.WhenAll(tasks); } catch { }</c>
        /// and then inspect each task's <c>Status</c>, <c>Result</c> or <c>Exception</c>.</para>
        /// <para>Be considerate to servers: hundreds of simultaneous requests to one API can trigger rate limits
        /// (HTTP 429); batch them with <c>Chunk</c> or a <c>SemaphoreSlim</c>.</para>
        /// <para>Example: <c>var results = await Curl.ExecuteManyAsync("curl https://api.github.com/users/microsoft", "curl https://api.github.com/users/dotnet");</c>
        /// then <c>results[1].Body</c> is the dotnet user.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/13-parallel-requests.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// var results = await Curl.ExecuteManyAsync(
        ///     "curl https://api.github.com/users/microsoft",
        ///     "curl https://api.github.com/users/dotnet",
        ///     "curl https://api.github.com/users/azure");
        ///
        /// for (int i = 0; i &lt; results.Length; i++)
        ///     Console.WriteLine($"{i}: HTTP {results[i].StatusCode}");
        /// </code>
        /// </example>
        /// <param name="commands">The curl command lines to run, each as for <see cref="ExecuteAsync(string)"/>; pass them
        /// as separate arguments (<c>ExecuteManyAsync(cmd1, cmd2)</c>) or as an array. An empty array returns an empty
        /// array.</param>
        /// <returns>One <see cref="CurlResult"/> per command, in the same order as <paramref name="commands"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="commands"/> is null.</exception>
        /// <exception cref="ArgumentException">One of the commands is null, empty or whitespace (the message gives its index).
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/13-parallel-requests.md</exception>
        /// <exception cref="CurlException">At least one command failed; the exception of the first failed command (in
        /// array order) is thrown. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="ExecuteAsync(string)"/>
        /// <seealso cref="DotNetCurl.CurlManyAsync(string[])"/>
        /// <seealso cref="DotNetCurl.CurlMany(string[])">Blocking version</seealso>
        public static async Task<CurlResult[]> ExecuteManyAsync(params string[] commands)
        {
            if (commands == null)
            {
                throw new ArgumentNullException(nameof(commands), CurlException.Compose(
                    "Curl.ExecuteManyAsync(commands) was called with a null array.",
                    "Pass the commands as separate arguments (Curl.ExecuteManyAsync(cmd1, cmd2)) or as a string array; an empty array is allowed and returns no results.",
                    DocLinks.Base + "tutorials/13-parallel-requests.md"));
            }

            var tasks = commands.Select((cmd, i) => ExecuteAsync(cmd ?? throw new ArgumentException(CurlException.Compose(
                $"Curl.ExecuteManyAsync(commands): the command at index {i} of {commands.Length} is null.",
                "Remove the null entry or fill it with a curl command such as \"curl https://api.example.com\".",
                DocLinks.Base + "tutorials/13-parallel-requests.md"), nameof(commands))));
            return await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Checks a curl command without sending anything and reports whether it can run, what is wrong with it, and
        /// exactly which URL, method, headers and body it would send.
        /// </summary>
        /// <remarks>
        /// <para>Validation parses the command with <see cref="DefaultFlavor"/> and checks that it has a URL, that every
        /// option is known (typos such as <c>--heder</c> are reported here, while <see cref="ExecuteAsync(string)"/>
        /// ignores them), that the URL parses and that its scheme is supported. It does not contact the server, so a URL
        /// that does not exist is still "valid".</para>
        /// <para>Never throws for bad input: null or empty text gives <c>IsValid == false</c> with the reason in
        /// <c>Error</c>.</para>
        /// <para>Example: <c>var v = Curl.Validate(userInput);</c> then
        /// <c>if (!v.IsValid) ShowError(v.Error); else Console.WriteLine($"{v.ParsedOptions.Method ?? "GET"} {v.ParsedOptions.Url}");</c>.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// var validation = Curl.Validate("curl -X POST --heder 'A: b' https://api.example.com");
        /// if (!validation.IsValid)
        ///     Console.WriteLine($"Invalid command: {validation.Error}");   // Unknown option: --heder
        /// else
        ///     Console.WriteLine(validation.ParsedOptions.Url);
        /// </code>
        /// </example>
        /// <param name="command">The curl command line to check, as for <see cref="ExecuteAsync(string)"/>. Null or empty
        /// text is reported as invalid.</param>
        /// <returns>A <see cref="ValidationResult"/>: <c>IsValid</c> (true when the command can run), <c>Error</c> (null
        /// when valid; otherwise the reason, such as <c>No URL specified</c>, <c>Unknown option: --heder</c>,
        /// <c>Unsupported protocol: gopher</c> or the parser's message with its fix) and <c>ParsedOptions</c> (the
        /// <see cref="CurlOptions"/> the command produced; set when valid and for unknown options, null otherwise).</returns>
        /// <seealso cref="Validate(string, CurlFlavor)"/>
        /// <seealso cref="ExecuteAsync(string)"/>
        /// <seealso cref="DotNetCurl.Validate(string)"/>
        public static ValidationResult Validate(string command)
        {
            return _engine.Validate(command);
        }

        /// <summary>
        /// Checks a curl command copied from a specific shell without executing it, reading it with that shell's
        /// quoting, escaping, line-continuation and variable rules.
        /// </summary>
        /// <param name="command">The command exactly as pasted. Null or empty text is reported as invalid.</param>
        /// <param name="flavor">The shell it was written for (<see cref="CurlFlavor.Auto"/> detects it); overrides
        /// <see cref="DefaultFlavor"/> for this call.</param>
        /// <returns>
        /// A <see cref="ValidationResult"/>: <c>IsValid</c>, <c>Error</c> (for example an unterminated quote, an unknown
        /// option or an untranslatable <c>Invoke-WebRequest</c> parameter, with the fix in the text) and
        /// <c>ParsedOptions</c>, whose <see cref="CurlOptions.Flavor"/> is the flavor that was used and whose
        /// <c>Data</c>, <c>Headers</c> and <c>Url</c> show exactly what would be sent. Never throws for bad input.
        /// </returns>
        /// <remarks>
        /// <para>Example: <c>var v = Curl.Validate("curl -d '{\"a\":1}' https://x.example", CurlFlavor.Posix);</c>
        /// gives <c>v.ParsedOptions.Data == "{\\\"a\\\":1}"</c> (backslashes kept), while the same text with
        /// <see cref="CurlFlavor.PowerShell"/> gives <c>{"a":1}</c>. Comparing the two is the quickest way to explain
        /// why a command behaves differently on another machine.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md</para>
        /// </remarks>
        /// <seealso cref="Validate(string)"/>
        /// <seealso cref="ExecuteAsync(string, CurlFlavor)"/>
        /// <seealso cref="DetectFlavor(string)"/>
        public static ValidationResult Validate(string command, CurlFlavor flavor)
        {
            return _engine.Validate(command, flavor);
        }

        /// <summary>
        /// Translates a curl command into equivalent C# <c>HttpClient</c> code, for learning what the command does or
        /// for moving a request to plain <c>HttpClient</c>.
        /// </summary>
        /// <remarks>
        /// <para>The generated code covers the method, URL, headers, body, basic credentials (<c>-u</c>), proxy
        /// (<c>-x</c>), <c>-k</c> and <c>-L</c>; options with no <c>HttpClient</c> equivalent are left out. Nothing is
        /// sent. The output contains any password or token from the command in clear text, so do not log it.</para>
        /// <para>Example: <c>string code = Curl.ToHttpClient("curl -X POST -H 'Content-Type: application/json' -d '{\"a\":1}' https://api.example.com");</c>
        /// returns code that builds an <c>HttpRequestMessage(HttpMethod.Post, ...)</c> with a <c>StringContent</c> body.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// string code = Curl.ToHttpClient(@"curl -X POST https://api.example.com/users \
        ///     -H 'Authorization: Bearer token123' -d '{""name"":""John""}'");
        /// Console.WriteLine(code);
        /// </code>
        /// </example>
        /// <param name="command">The curl command line to translate, read with <see cref="DefaultFlavor"/>, for example
        /// <c>curl -H 'Accept: application/json' https://api.example.com</c>.</param>
        /// <returns>C# source text (several lines, starting with a <c>// Generated by CurlDotNet from: ...</c> comment) that
        /// sends the same request with <c>HttpClient</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is null, empty or whitespace. Fix: pass a
        /// command with a URL. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md</exception>
        /// <exception cref="CurlInvalidCommandException">The command text cannot be read (unterminated quote, option
        /// without its value); the message names the problem.</exception>
        /// <exception cref="CurlException">Another parse error, such as a file named with <c>-d @file</c> that cannot be read.</exception>
        /// <seealso cref="ToFetch(string)"/>
        /// <seealso cref="ToPythonRequests(string)"/>
        /// <seealso cref="ToPowershellCode(string)"/>
        /// <seealso cref="DotNetCurl.ToHttpClient(string)"/>
        public static string ToHttpClient(string command)
        {
            return _engine.ToHttpClientCode(command);
        }

        /// <summary>
        /// Translates a curl command into JavaScript <c>fetch()</c> code that sends the same request, for web front ends
        /// and Node.js.
        /// </summary>
        /// <remarks>
        /// <para>The generated code covers the URL, method, headers and body. Nothing is sent. Credentials from the
        /// command appear in the output.</para>
        /// <para>Example: <c>string js = Curl.ToFetch("curl -H 'Authorization: Bearer token' https://api.example.com/data");</c>
        /// returns <c>const response = await fetch('https://api.example.com/data', { ... headers: { 'Authorization': 'Bearer token' } ... });</c>.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// string jsCode = Curl.ToFetch("curl -X GET https://api.example.com/data -H 'Authorization: Bearer token'");
        /// Console.WriteLine(jsCode);
        /// </code>
        /// </example>
        /// <param name="command">The curl command line to translate, read with <see cref="DefaultFlavor"/>.</param>
        /// <returns>JavaScript source text using <c>fetch()</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlInvalidCommandException">The command text cannot be read; the message names the problem.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="ToHttpClient(string)"/>
        /// <seealso cref="ToPythonRequests(string)"/>
        /// <seealso cref="DotNetCurl.ToFetch(string)"/>
        public static string ToFetch(string command)
        {
            return _engine.ToFetchCode(command);
        }

        /// <summary>
        /// Translates a curl command into Python code using the <c>requests</c> library, for sharing a request with
        /// Python scripts and notebooks.
        /// </summary>
        /// <remarks>
        /// <para>The generated code covers the method, URL, headers, body and <c>-u</c> credentials (as
        /// <c>auth=('user', 'pass')</c>). Nothing is sent. Credentials appear in the output.</para>
        /// <para>Example: <c>string py = Curl.ToPythonRequests("curl -u user:pass https://api.example.com/data");</c>
        /// returns code starting with <c>import requests</c> that calls <c>requests.get(...)</c>.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md</para>
        /// </remarks>
        /// <param name="command">The curl command line to translate, read with <see cref="DefaultFlavor"/>.</param>
        /// <returns>Python source text using <c>requests</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlInvalidCommandException">The command text cannot be read; the message names the problem.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="ToHttpClient(string)"/>
        /// <seealso cref="ToFetch(string)"/>
        /// <seealso cref="DotNetCurl.ToPython(string)"/>
        public static string ToPythonRequests(string command)
        {
            return _engine.ToPythonCode(command);
        }

        /// <summary>
        /// Translates a curl command into a PowerShell <c>Invoke-RestMethod</c> call that sends the same request, for
        /// Windows admins and build scripts.
        /// </summary>
        /// <remarks>
        /// <para>The generated command covers <c>-Uri</c>, <c>-Method</c>, headers and <c>-Body</c>. Nothing is sent.
        /// To go the other way (run an <c>Invoke-WebRequest</c> command), use
        /// <see cref="ExecuteAsync(string, CurlFlavor)"/> with <see cref="CurlFlavor.InvokeWebRequest"/>.</para>
        /// <para>Example: <c>Curl.ToPowershellCode("curl -X POST https://api.example.com/data -d '{\"key\":\"value\"}'")</c>
        /// returns <c>Invoke-RestMethod -Uri "https://api.example.com/data" -Method POST ...</c>.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md</para>
        /// </remarks>
        /// <param name="command">The curl command line to translate, read with <see cref="DefaultFlavor"/>.</param>
        /// <returns>One PowerShell command line using <c>Invoke-RestMethod</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is null, empty or whitespace.</exception>
        /// <exception cref="CurlInvalidCommandException">The command text cannot be read; the message names the problem.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md</exception>
        /// <seealso cref="ToHttpClient(string)"/>
        /// <seealso cref="ExecuteAsync(string, CurlFlavor)"/>
        public static string ToPowershellCode(string command)
        {
            return _engine.ToPowershellCode(command);
        }

        /// <summary>
        /// Serializes an object to JSON the way a curl user would type it: apostrophes, quotes and
        /// non-ASCII characters are kept literal instead of being \uXXXX-escaped.
        /// </summary>
        internal static string SerializeJson(object data)
        {
#if NETSTANDARD2_0
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
#else
            return System.Text.Json.JsonSerializer.Serialize(data, JsonPayloadOptions);
#endif
        }

#if !NETSTANDARD2_0
        private static readonly System.Text.Json.JsonSerializerOptions JsonPayloadOptions = new System.Text.Json.JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
#endif
    }
}
