/***************************************************************************
 * HttpHandler - HTTP/HTTPS protocol handler
 *
 * Transpiled from curl's lib/http.c and lib/url.c by Daniel Stenberg and contributors
 * Original curl Copyright (C) 1996-2025, Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024-2025 Jacob Mellor and IronSoftware
 *
 * TRANSPILATION APPROACH:
 * This class implements curl's HTTP protocol logic by transpiling the behavior
 * and state machines from curl's C source code. While we use .NET's HttpClient
 * as the underlying transport layer (it's efficient and cross-platform), all
 * the protocol behavior, option handling, redirect logic, and error handling
 * match curl's implementation exactly.
 *
 * The value is NOT in wrapping HttpClient - it's in transpiling curl's protocol
 * logic, command parsing, and behavior so curl commands work identically in .NET.
 *
 * By Jacob Mellor
 * GitHub: https://github.com/jacob-mellor
 * Sponsored by IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core.Handlers;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Handler for HTTP and HTTPS protocols.
    /// </summary>
    internal class HttpHandler : IProtocolHandler
    {
        private readonly HttpClient _httpClient;
        private readonly bool _ownsHttpClient;
        private readonly RedirectHandler _redirectHandler;

        /// <summary>
        /// Create handler with default HttpClient.
        /// </summary>
        public HttpHandler() : this(CreateDefaultHttpClient(), true)
        {
        }

        /// <summary>
        /// Create handler with custom HttpClient.
        /// </summary>
        public HttpHandler(HttpClient httpClient, bool ownsHttpClient = false)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _ownsHttpClient = ownsHttpClient;
            _redirectHandler = new RedirectHandler();
        }

        /// <summary>Key in <see cref="Exception.Data"/> holding the standard-output part of -w for a failed transfer.</summary>
        internal const string WriteOutDataKey = CurlException.WriteOutDataKey;

        public async Task<CurlResult> ExecuteAsync(CurlOptions options, CancellationToken cancellationToken)
        {
            try
            {
                return await ExecuteWithRetryAsync(options, cancellationToken);
            }
            catch (CurlException ex)
            {
                // curl expands -w for failed transfers too (%{exitcode}, %{errormsg}, %{onerror}): the
                // %{stderr} part goes to --stderr, the stdout part travels on the exception (failures
                // after a response arrived were already handled with the full transfer context)
                AttachFailureOutput(ex, null, null, null, null, options);
                throw;
            }
        }

        private async Task<CurlResult> ExecuteWithRetryAsync(CurlOptions options, CancellationToken cancellationToken)
        {
            if (options.Retry <= 0)
            {
                return await ExecuteAttemptAsync(options, cancellationToken);
            }

            // curl --retry: transient errors are timeouts and HTTP 408/429/500/502/503/504; the wait
            // doubles from 1 s up to 10 minutes unless --retry-delay fixes it; --retry-max-time caps it all.
            // Output that went to stdout cannot be taken back, so each attempt's stdout is kept, like curl.
            var retryStarted = DateTime.UtcNow;
            var earlierStdout = new StringBuilder();
            // curl truncates the output file before a retry only when the failed attempt wrote body
            // bytes to it; header-only attempts (-i, -f) stay in the file
            var state = new AttemptState();
            var toStdout = string.IsNullOrEmpty(options.OutputFile) && !options.UseRemoteFileName;
            for (var attempt = 0; ; attempt++)
            {
                var retriesLeft = options.Retry - attempt;
                CurlResult? result = null;
                string? reason = null;
                TimeSpan? retryAfter = null;
                try
                {
                    state.WroteBody = false;
                    result = await ExecuteAttemptAsync(options, cancellationToken, state);
                    if (retriesLeft > 0 && IsTransientStatus(result.StatusCode))
                    {
                        reason = $"HTTP {result.StatusCode}";
                        retryAfter = RetryAfter(result.Headers);
                    }
                }
                catch (CurlException ex)
                {
                    if (retriesLeft > 0)
                    {
                        if (ex is CurlOperationTimeoutException)
                        {
                            reason = "timeout";
                        }
                        else if (ex is CurlHttpReturnedErrorException http && (IsTransientStatus(http.StatusCode) || options.RetryAllErrors))
                        {
                            reason = $"HTTP {http.StatusCode}";
                            retryAfter = RetryAfter(http.ResponseHeaders);
                        }
                        else if (ex is CurlCouldntConnectException && (options.RetryAllErrors || options.RetryConnRefused))
                        {
                            reason = "connection failed";
                        }
                        else if (options.RetryAllErrors)
                        {
                            reason = "error";
                        }
                    }

                    if (reason == null)
                    {
                        // the last attempt failed: what earlier attempts printed comes first
                        TransferStdout.Prepend(ex, earlierStdout.ToString());
                        throw;
                    }

                    // a failed attempt that is retried has printed its headers / --fail-with-body page
                    // (curl expands -w only once, after the last attempt)
                    if (toStdout && FailureOutputAttached.TryGetValue(ex, out var attached))
                    {
                        earlierStdout.Append(((FailureOutput)attached).BeforeWriteOut);
                    }
                }

                if (reason == null)
                {
                    if (earlierStdout.Length > 0 && result != null)
                    {
                        result.Body = earlierStdout + (result.Body ?? string.Empty);
                    }
                    return result!;
                }

                if (toStdout && result?.Body != null)
                {
                    earlierStdout.Append(result.Body);
                }
                state.AppendOutput = state.OutputPath != null && !state.WroteBody;

                var delaySeconds = retryAfter?.TotalSeconds
                    ?? (options.RetryDelay > 0 ? options.RetryDelay : Math.Min(Math.Pow(2, attempt), 600));
                if (options.RetryMaxTime > 0 && (DateTime.UtcNow - retryStarted).TotalSeconds + delaySeconds > options.RetryMaxTime)
                {
                    if (result != null && earlierStdout.Length > 0)
                    {
                        result.Body = earlierStdout.ToString();
                    }
                    return result ?? throw new CurlOperationTimeoutException(options.RetryMaxTime, options.OriginalCommand);
                }

                await Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken);
            }
        }

        /// <summary>curl's "transient" HTTP responses for --retry.</summary>
        private static bool IsTransientStatus(int statusCode) =>
            statusCode == 408 || statusCode == 429 || statusCode == 500 || statusCode == 502 || statusCode == 503 || statusCode == 504;

        /// <summary>A Retry-After header in seconds (curl honours it for --retry), capped at 10 minutes.</summary>
        private static TimeSpan? RetryAfter(IDictionary<string, string>? headers)
        {
            if (headers != null && headers.TryGetValue("Retry-After", out var value)
                && int.TryParse(value.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var seconds))
            {
                return TimeSpan.FromSeconds(Math.Min(seconds, 600));
            }
            return null;
        }

        /// <summary>What one --retry attempt left behind, so the next attempt can continue the output file.</summary>
        private sealed class AttemptState
        {
            /// <summary>Set by the caller: append to the output file instead of truncating it.</summary>
            public bool AppendOutput;

            /// <summary>Set by the attempt: body bytes were written to the output file.</summary>
            public bool WroteBody;

            /// <summary>Set by the attempt: the file that was written.</summary>
            public string? OutputPath;
        }

        private async Task<CurlResult> ExecuteAttemptAsync(CurlOptions options, CancellationToken cancellationToken, AttemptState? attempt = null)
        {
            var verboseLog = options.Verbose ? new StringBuilder() : null;
            var trace = TraceWriter.TryCreate(options);

            // Choose the transport. Most requests share the pooled HttpClient, but when the command
            // asks for a proxy, relaxed TLS, challenge auth or pinned name resolution we build a
            // dedicated client so those settings are honored (and visible in the trace).
            var proxy = CurlProxy.Resolve(options);
            var client = _httpClient;
            HttpClient? perRequestClient = null;
            if (_ownsHttpClient && (NeedsCustomTransport(options) || proxy != null))
            {
                perRequestClient = CurlTransport.CreateClient(options, options.ProxyTunnel ? null : proxy);
                client = perRequestClient;
            }

            var cookies = CookieEngine.Create(options);
            var transfer = new HttpTransfer(options, client, proxy, cookies, verboseLog,
                CreateRequest, AppendVerboseRequest, AppendVerboseResponseHeaders, BuildHeaderBlock);
            var timings = transfer.Timings;
            var startTime = transfer.StartTime;
            HttpResponseMessage? response = null;

            // Configure timeout. Without --max-time the caller's token is used as-is.
            var timeoutSeconds = GetTimeoutSeconds(options);
            using var cts = timeoutSeconds > 0 ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken) : null;
            if (cts != null)
            {
                cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
            }
            var token = cts?.Token ?? cancellationToken;

            var readingBody = false;
            try
            {
                if (options.SkipExisting)
                {
                    var target = TransferOutput.ResolveOutputPath(options, transfer, null, options.Url);
                    if (target != null && File.Exists(target))
                    {
                        // --skip-existing: curl does not even connect when the file is already there
                        trace?.Info($"Skipping {options.Url}: '{target}' already exists (--skip-existing)");
                        return new CurlResult { StatusCode = 0, Body = string.Empty, Command = options.OriginalCommand, Timings = timings };
                    }
                }

                if (!string.IsNullOrEmpty(options.EtagSave) && options.EtagSave != "-" && !options.CreateDirs)
                {
                    var etagDir = Path.GetDirectoryName(Path.GetFullPath(options.EtagSave));
                    if (!string.IsNullOrEmpty(etagDir) && !Directory.Exists(etagDir))
                    {
                        // curl: "Failed creating file for saving etags ... Skip this transfer" (no request, no error)
                        trace?.Info($"Skipping {options.Url}: --etag-save directory '{etagDir}' does not exist");
                        if (!options.Silent)
                        {
                            WriteStderr(options, $"Warning: Failed creating file for saving etags: \"{options.EtagSave}\". Skip this transfer\n");
                        }
                        return new CurlResult { StatusCode = 0, Body = string.Empty, Command = options.OriginalCommand, Timings = timings };
                    }
                }

                var request = transfer.PrepareRequest(options);
                if (options.ResumeFrom.HasValue && options.ResumeFrom.Value != 0 && request.Content != null)
                {
                    // curl cannot resume an upload over HTTP (exit 43) and refuses to try
                    throw new CurlBadFunctionArgumentException("-C/--continue-at",
                        $"-C/--continue-at {(options.ResumeFrom.Value < 0 ? "-" : options.ResumeFrom.Value.ToString(System.Globalization.CultureInfo.InvariantCulture))} cannot be combined with a request body (-d, -F or -T): resuming an HTTP upload is not supported. " +
                        "Send the whole body again without -C.", options.OriginalCommand);
                }

                if (trace != null)
                {
                    var uri = request.RequestUri;
                    trace.Info($"Trying {uri.Host}:{uri.Port}...");
                    var proxyInfo = DescribeProxy(options, uri);
                    if (proxyInfo != null)
                    {
                        trace.Info(proxyInfo);
                    }
                    trace.Info($"Connected to {uri.Host} ({uri.Host}) port {uri.Port}");
                    if (string.Equals(uri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
                    {
                        trace.Info(options.Insecure
                            ? "TLS: certificate verification DISABLED (-k/--insecure)"
                            : "TLS: certificate verification enabled");
                    }
                    trace.SendHeaders(request);
                    await trace.SendDataAsync(request);
                }

                AppendVerboseRequest(verboseLog, request);

                // Send request
                timings.PreTransfer = (DateTime.UtcNow - startTime).TotalMilliseconds;
                response = await transfer.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);
                AppendVerboseResponseHeaders(verboseLog, response);
                // 401/407 challenges (Digest, NTLM, --anyauth) and auth probes are answered like curl
                var authenticated = await transfer.AuthenticateAsync(response, request, options, token);
                response = authenticated.Response;
                request = authenticated.Request;
                timings.StartTransfer = (DateTime.UtcNow - startTime).TotalMilliseconds;

                // Redirects are followed by hand, like curl (and redirect_url is computed without -L)
                if (_redirectHandler.IsRedirect(response.StatusCode))
                {
                    if (options.FollowLocation || options.LocationTrusted)
                    {
                        trace?.Info($"Received HTTP {(int)response.StatusCode}; following redirect (-L)");
                    }

                    var followed = await _redirectHandler.FollowAsync(transfer, response, request, token);
                    response = followed.Response;
                    request = followed.Request;
                }

                trace?.RecvHeaders(response);

                readingBody = true;
                return await FinishTransferAsync(transfer, request, response, token, verboseLog, trace, attempt);
            }
            catch (CurlException ex)
            {
                if (!readingBody && (ex is CurlTooManyRedirectsException || ex is CurlUnsupportedProtocolException || ex is CurlMalformedUrlException))
                {
                    // a redirect could not be followed: curl has already printed the -i headers it received
                    var includeHeaders = options.IncludeHeaders || (options.HeadOnly && !string.IsNullOrEmpty(options.OriginalCommand));
                    var toStdout = string.IsNullOrEmpty(options.OutputFile) && !options.UseRemoteFileName;
                    var headers = includeHeaders && toStdout && transfer.Transcript.HasRecording
                        ? string.Concat(transfer.Transcript.AllHeads.Select(h => h.Text))
                        : null;
                    AttachFailureOutput(ex, transfer, headers, null, null);
                }
                throw;
            }
            catch (OperationCanceledException)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    trace?.Error("Request was canceled by the caller before a response was received.");
                    throw new CurlAbortedByCallbackException(options.OriginalCommand);
                }

                // Only our own linked token can have fired: this is --max-time (exit code 28), like curl
                var received = transfer.LastExchange?.BytesReceived ?? 0;
                if (timeoutSeconds <= 0)
                {
                    // no --max-time: HttpClient gave up on its own (for example a stalled TLS handshake)
                    trace?.Error("HttpClient cancelled the request without a --max-time limit.");
                    throw new CurlOperationTimeoutException(0,
                        $"The transfer to {transfer.CurrentRequest?.RequestUri?.Host ?? options.Url} was abandoned by HttpClient's internal timeout after {received} bytes " +
                        "(the server stopped answering, often during a TLS handshake). Check that the port speaks the protocol of the URL; set -m/--max-time to control the limit yourself.",
                        options.OriginalCommand);
                }
                trace?.Error($"Operation timed out after {timeoutSeconds} seconds.");
                throw new CurlOperationTimeoutException(timeoutSeconds,
                    $"Operation timed out after {(long)(timeoutSeconds * 1000)} milliseconds with {received} bytes received " +
                    $"(-m/--max-time {timeoutSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture)}). Raise --max-time for slow servers or large downloads.",
                    options.OriginalCommand);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is IOException)
            {
                trace?.Error($"Transfer failed: {ex.Message}");
                for (var inner = ex.InnerException; inner != null; inner = inner.InnerException)
                {
                    trace?.Error($"  caused by: {inner.GetType().Name}: {inner.Message}");
                }

                if (options.AllowHttp09 && !readingBody)
                {
                    var http09 = TryHttp09Result(transfer, options);
                    if (http09 != null)
                    {
                        return http09;
                    }
                }

                var mapped = TransportErrors.Translate(ex, transfer, readingBody);
                if (mapped != null)
                {
                    throw mapped;
                }
                throw;
            }
            catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException || ex is FormatException
                                       || ex is NotSupportedException || ex is System.ComponentModel.Win32Exception)
            {
                // HttpClient's own Digest/NTLM/Negotiate code (or its URI handling) failed on something
                // the server sent. curl would have handled it; report what happened instead of a raw
                // framework exception.
                trace?.Error($"Transfer failed inside HttpClient: {ex.GetType().Name}: {ex.Message}");
                throw TransportErrors.FromFrameworkFailure(ex, transfer);
            }
            finally
            {
                response?.Dispose();
                perRequestClient?.Dispose();
                trace?.Dispose();
            }
        }

        /// <summary>
        /// Determines whether the request needs a dedicated HttpClient because it
        /// specifies transport-level options (proxy, relaxed TLS, challenge auth, --resolve,
        /// --expect100-timeout) that the shared pooled client does not carry.
        /// </summary>
        private static bool NeedsCustomTransport(CurlOptions options)
        {
            return CurlProxy.IsConfigured(options)
                || options.Insecure
                || UsesChallengeAuth(options)
                || options.Resolve.Count > 0
                || options.ProxyTunnel
                || options.Expect100TimeoutSeconds.HasValue;
        }

        /// <summary>
        /// True when the command selected a challenge-response auth scheme
        /// (--digest, --ntlm, --negotiate, --anyauth) with credentials. These schemes
        /// are performed by the handler's 401 challenge flow, not a preemptive header,
        /// so they need a dedicated HttpClient carrying the credentials.
        /// </summary>
        internal static bool UsesChallengeAuth(CurlOptions options)
        {
            // decided by the scheme alone: credentials may also come from the URL or .netrc, and a
            // preemptive Basic header must never leak to a server that is asked for Digest/NTLM
            if (string.IsNullOrEmpty(options.AuthScheme))
                return false;
            switch (options.AuthScheme.ToLowerInvariant())
            {
                case "digest":
                case "ntlm":
                case "negotiate":
                case "anyauth":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Produces a human-readable description of the proxy that will be used for the
        /// request (explicit, environment, system, or none) for inclusion in the trace.
        /// </summary>
        private static string DescribeProxy(CurlOptions options, Uri uri)
        {
            try
            {
                var proxy = CurlProxy.Resolve(options);
                if (proxy != null)
                {
                    var target = proxy.ProxyFor(uri);
                    if (target == null)
                    {
                        return "Proxy bypassed for this host (--noproxy / NO_PROXY)";
                    }
                    var creds = proxy.UserCredentials != null ? " (with credentials)" : string.Empty;
                    var origin = options.Proxy != null || !string.IsNullOrEmpty(options.Socks5Proxy) ? "explicit" : "environment";
                    return $"Using {origin} proxy {target.Scheme}://{target.Host}:{target.Port}{creds}";
                }

                var systemProxy = WebRequest.DefaultWebProxy;
                if (systemProxy != null && uri != null && !systemProxy.IsBypassed(uri))
                {
                    var resolved = systemProxy.GetProxy(uri);
                    if (resolved != null && !string.Equals(resolved.AbsoluteUri, uri.AbsoluteUri, StringComparison.OrdinalIgnoreCase))
                    {
                        return $"Using system proxy {resolved.Host}:{resolved.Port}";
                    }
                }
            }
            catch
            {
                // Proxy resolution is best-effort for diagnostics only.
            }

            return "No proxy in use (direct connection)";
        }

        public bool SupportsProtocol(string protocol)
        {
            return protocol == "http" || protocol == "https";
        }

        private HttpRequestMessage CreateRequest(CurlOptions options)
        {
            var method = GetHttpMethod(options);
            var uri = RequestShaping.BuildRequestUri(options);
            var request = new HttpRequestMessage(method, uri);

            // --http1.0 / --http1.1 / --http2 / --http2-prior-knowledge / --http3
            RequestShaping.ApplyHttpVersion(request, options);

            // Add headers. An empty value (-H "Accept:") means "do not send this header at all",
            // exactly as in curl, which also suppresses our defaults below.
            var suppressed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in options.Headers)
            {
                if (header.Key.StartsWith("@", StringComparison.Ordinal))
                {
                    continue; // unread -H @file reference
                }

                if (string.IsNullOrEmpty(header.Value))
                {
                    suppressed.Add(header.Key);
                    continue;
                }

                if (RequestShaping.IsContentHeader(header.Key))
                {
                    // Will be added with content
                    continue;
                }

                if (header.Key.Equals("User-Agent", StringComparison.OrdinalIgnoreCase) && options.UserAgent != null)
                {
                    continue; // -A / -H "User-Agent:" both land in options.UserAgent; add it once below
                }

                if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
                {
                    request.Headers.Host = header.Value; // -H "Host: name" replaces curl's Host header
                    continue;
                }

                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // -H "Name;" sends the header with an empty value
            foreach (var name in options.EmptyHeaders)
            {
                if ((options.Headers.TryGetValue(name, out var given) && !string.IsNullOrEmpty(given)) ||
                    RequestShaping.IsContentHeader(name) || name.Equals("Host", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                request.Headers.TryAddWithoutValidation(name, string.Empty);
            }

            // User-Agent: -A wins (-A "" sends none at all), then any -H value, then curl's default (curl/8.7.1)
            if (options.UserAgent != null)
            {
                if (options.UserAgent.Length > 0 && !suppressed.Contains("User-Agent"))
                {
                    request.Headers.TryAddWithoutValidation("User-Agent", options.UserAgent);
                }
            }
            else if (!suppressed.Contains("User-Agent") && !request.Headers.Contains("User-Agent") && !string.IsNullOrEmpty(Curl.DefaultUserAgent))
            {
                request.Headers.TryAddWithoutValidation("User-Agent", Curl.DefaultUserAgent);
            }

            // curl always sends Accept: */* unless the command sets or suppresses it
            if (!suppressed.Contains("Accept") && !request.Headers.Contains("Accept"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "*/*");
            }

            // Referer (-e) is sent verbatim, even when it is not an absolute URL (-e firstone.html)
            if (!string.IsNullOrEmpty(options.Referer) && !suppressed.Contains("Referer") && !request.Headers.Contains("Referer"))
            {
                request.Headers.TryAddWithoutValidation("Referer", options.Referer);
            }

            // Cookies (-b): curl sends a value containing '=' as cookie data; without '=' it names a cookie file
            if (!string.IsNullOrEmpty(options.Cookie) && options.Cookie!.IndexOf('=') >= 0)
            {
                request.Headers.TryAddWithoutValidation("Cookie", options.Cookie);
            }

            // Authorization: an explicit -H wins, then --oauth2-bearer, then AWS SigV4 (signed below),
            // then preemptive Basic from -u or from user:password@ in the URL being requested.
            if (!request.Headers.Contains("Authorization") && !suppressed.Contains("Authorization"))
            {
                if (!string.IsNullOrEmpty(options.OAuth2Bearer))
                {
                    // the token belongs to the first origin: a redirect elsewhere gets no Authorization at all
                    if (!options.CrossOriginHop)
                    {
                        request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + options.OAuth2Bearer);
                    }
                }
                else if (options.AwsSigV4 != null && options.Credentials != null)
                {
                    // AWS SigV4 signing needs the payload hash, so it happens after content is set
                }
                else
                {
                    // Challenge schemes (--digest etc.) are answered by the handler's Credentials instead;
                    // sending Basic here would leak the password and break digest (issue #40).
                    var credentials = RequestShaping.GetCredentials(options, uri);
                    if (credentials != null && !UsesChallengeAuth(options))
                    {
                        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{credentials.UserName}:{credentials.Password}"));
                        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
                    }
                }
            }

            // Through a plain HTTP proxy: Proxy-Connection: Keep-Alive, preemptive Proxy-Authorization, --proxy-header
            RequestShaping.ApplyProxyHeaders(request, options, uri, suppressed);

            // Range (-r, and -C OFFSET): curl sends the value verbatim after "bytes=", multi-ranges included
            if (!string.IsNullOrEmpty(options.Range) && options.UploadFile == null && !suppressed.Contains("Range") && !request.Headers.Contains("Range"))
            {
                request.Headers.TryAddWithoutValidation("Range", "bytes=" + options.Range);
            }

            // Time condition (-z): If-Modified-Since by default, If-Unmodified-Since for -z -DATE
            if (options.TimeCondition.HasValue && options.TimeConditionType != CurlTimeCondition.LastModified)
            {
                var conditionHeader = options.TimeConditionType == CurlTimeCondition.IfUnmodifiedSince ? "If-Unmodified-Since" : "If-Modified-Since";
                if (!suppressed.Contains(conditionHeader) && !request.Headers.Contains(conditionHeader))
                {
                    request.Headers.TryAddWithoutValidation(conditionHeader, RequestShaping.FormatHttpDate(options.TimeCondition.Value));
                }
            }

            // Set compression acceptance (--compressed flag). Decoding happens in CreateResult so the
            // headers curl would show (Content-Encoding, Content-Length) stay intact for -i.
            if (options.Compressed && !suppressed.Contains("Accept-Encoding") && !request.Headers.Contains("Accept-Encoding"))
            {
#if NETSTANDARD2_0 || NET472 || NET48
                request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
#else
                request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate, br");
#endif
            }

            // Add content. Parsed commands carry their exact body; curl sends it whatever the method
            // (-X GET -d x is a GET with a body). Hand-built options keep the classic rules.
            var parsedBody = options.GetParsedBody();
            if (options.Files.Any() || options.FormData.Any())
            {
                if (ShouldHaveContent(method, options))
                {
                    request.Content = CreateContent(options); // multipart (-F)
                }
            }
            else if (options.UploadFile != null && options.BinaryData != null)
            {
                if (RequestShaping.CanSendBody(method))
                {
                    request.Content = RequestShaping.CreateUploadContent(options.BinaryData, options); // -T

                    // -T with -r uploads a part: curl sends Content-Range: bytes RANGE/TOTAL instead of Range
                    if (!string.IsNullOrEmpty(options.Range) && options.ResumeFrom == null && !options.Headers.ContainsKey("Content-Range"))
                    {
                        request.Content.Headers.TryAddWithoutValidation("Content-Range", $"bytes {options.Range}/{options.BinaryData.Length}");
                    }
                }
            }
            else if (parsedBody != null)
            {
                if (RequestShaping.CanSendBody(method))
                {
                    request.Content = RequestShaping.CreateDataContent(parsedBody, options); // -d, --data-binary, --json ...
                }
            }
            else if (ShouldHaveContent(method, options))
            {
                request.Content = CreateContent(options);
            }

            // Apply content headers that were skipped from request.Headers above.
            // In .NET HttpClient, headers like Content-Range, Content-Encoding, etc.
            // must be set on HttpContent.Headers, not HttpRequestMessage.Headers.
            if (request.Content != null)
            {
                foreach (var header in options.Headers)
                {
                    if (RequestShaping.IsContentHeader(header.Key) && !string.IsNullOrEmpty(header.Value) &&
                        !string.Equals(header.Key, "Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        // Content-Type is already handled with the content
                        request.Content.Headers.Remove(header.Key);
                        request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }

            // Apply AWS SigV4 signing after content is set (signing requires payload hash)
            if (options.AwsSigV4 != null && options.Credentials != null && string.IsNullOrEmpty(options.OAuth2Bearer))
            {
                AwsSigV4Signer.SignRequest(
                    request,
                    options.AwsSigV4,
                    options.Credentials.UserName,
                    options.Credentials.Password);
            }

            return request;
        }

        private static double GetTimeoutSeconds(CurlOptions options)
        {
            // Like curl, no --max-time means no transfer timeout (HttpClient's own 100 s cap still applies)
            var timeout = options.MaxTime ?? Curl.DefaultMaxTimeSeconds;
            return timeout > 0 ? timeout : 0;
        }

        private HttpMethod GetHttpMethod(CurlOptions options)
        {
            if (!string.IsNullOrEmpty(options.CustomMethod))
            {
                return new HttpMethod(options.CustomMethod);
            }

            return options.Method?.ToUpper() switch
            {
                "GET" => HttpMethod.Get,
                "POST" => HttpMethod.Post,
                "PUT" => HttpMethod.Put,
                "DELETE" => HttpMethod.Delete,
                "HEAD" => HttpMethod.Head,
                "OPTIONS" => HttpMethod.Options,
                "PATCH" => new HttpMethod("PATCH"),
                _ => HttpMethod.Get
            };
        }

        private bool ShouldHaveContent(HttpMethod method, CurlOptions options)
        {
            if (method == HttpMethod.Get || method == HttpMethod.Head)
                return false;

            return !string.IsNullOrEmpty(options.Data) ||
                   options.BinaryData != null ||
                   options.FormData.Any() ||
                   options.Files.Any() ||
                   options.FormParts.Count > 0;
        }

        private HttpContent CreateContent(CurlOptions options)
        {
            // Multipart form data (-F / --form-string), laid out byte-for-byte like curl's
            if (options.FormParts.Count > 0 || options.Files.Any() || options.FormData.Any())
            {
                return MultipartFormBuilder.Build(MultipartFormBuilder.PartsFor(options), options);
            }

            // Binary data
            if (options.BinaryData != null)
            {
                return new ByteArrayContent(options.BinaryData);
            }

            // Text data
            if (!string.IsNullOrEmpty(options.Data))
            {
                var data = options.Data;

                // Resolve @file references (curl reads file contents when data starts with @)
                // --data-raw sets DataRaw=true which disables @ expansion (like real curl)
                if (!options.DataRaw && data.StartsWith("@"))
                {
                    var filePath = data.Substring(1);
                    if (!File.Exists(filePath))
                    {
                        throw new CurlFileCouldntReadException(filePath, options.OriginalCommand);
                    }

                    // Read as raw bytes to preserve binary data integrity.
                    // File.ReadAllText() corrupts binary files by decoding invalid UTF-8
                    // sequences as replacement characters (U+FFFD) then re-encoding.
                    var fileBytes = File.ReadAllBytes(filePath);
                    var byteContent = new ByteArrayContent(fileBytes);

                    if (options.Headers.TryGetValue("Content-Type", out var fileContentType))
                    {
                        byteContent.Headers.ContentType = MediaTypeHeaderValue.Parse(fileContentType);
                    }
                    else
                    {
                        byteContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
                    }

                    return byteContent;
                }

                var content = new StringContent(data, Encoding.UTF8);

                // Set content type from headers if specified
                if (options.Headers.TryGetValue("Content-Type", out var contentType))
                {
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                }
                else
                {
                    // Default to application/x-www-form-urlencoded for POST data
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
                }

                return content;
            }

            return null;
        }

        /// <summary>
        /// Reads the body, applies -f/--fail, writes -D/-o/-O output, expands -w and saves the
        /// cookie jar, in the order curl does.
        /// </summary>
        private async Task<CurlResult> FinishTransferAsync(HttpTransfer transfer, HttpRequestMessage request, HttpResponseMessage response,
            CancellationToken cancellationToken, StringBuilder? verboseLog, TraceWriter? trace, AttemptState? attempt)
        {
            var options = transfer.Options;
            var command = options.OriginalCommand;
            var status = (int)response.StatusCode;
            var isHead = request.Method == HttpMethod.Head || options.HeadOnly;
            var finalHead = transfer.FinalHead;
            var timings = transfer.Timings;

            // --max-filesize refuses an announced size before reading anything, like curl (a length
            // too big for a 64-bit number is over any limit)
            var announced = response.Content?.Headers.ContentLength;
            var rawLength = finalHead?.GetHeader("Content-Length")?.Trim();
            if (rawLength != null && rawLength.Length > 18 && rawLength.All(char.IsDigit))
            {
                announced = long.MaxValue;
            }
            if (options.MaxFileSize.HasValue && announced.HasValue && announced.Value > options.MaxFileSize.Value && !isHead)
            {
                throw new CurlFileSizeExceededException(options.MaxFileSize.Value, announced.Value, command);
            }

            var rawBody = Array.Empty<byte>();
            Exception? bodyError = null;
            if (!isHead && response.Content != null)
            {
                (rawBody, bodyError) = await ReadBodyAsync(response.Content, cancellationToken, options.MaxFileSize);
            }
            timings.Total = (DateTime.UtcNow - transfer.StartTime).TotalMilliseconds;

            if (bodyError is OperationCanceledException canceled)
            {
                throw canceled;
            }

            // curl's -I means "no body, show headers" (tool_getparam sets both); options built through
            // the API (LibCurl.HeadAsync) only ask for no body, so they keep headers in result.Headers.
            var includeHeaders = options.IncludeHeaders || (options.HeadOnly && !string.IsNullOrEmpty(options.OriginalCommand));
            var headerText = transfer.BuildHeaderText(response);

            // -D FILE gets the header blocks as soon as they arrived (and chunked trailers after the
            // body), even if the body then fails
            if (!string.IsNullOrEmpty(options.DumpHeaderFile) && options.DumpHeaderFile != "-")
            {
                TransferOutput.DumpHeaders(options, headerText + (finalHead?.Trailers ?? string.Empty));
            }

            CurlException? failure = null;
            if (bodyError == null && finalHead?.Trailers != null)
            {
                failure = TransportErrors.ValidateTrailers(finalHead, transfer);
            }

            if (bodyError is FileSizeLimitReached)
            {
                failure = new CurlFileSizeExceededException(options.MaxFileSize!.Value, rawBody.Length, command);
            }
            else if (bodyError != null)
            {
                failure = TransportErrors.Translate(bodyError, transfer, readingBody: true)
                          ?? new CurlReceiveErrorException(bodyError.Message, command);
                if (failure is CurlPartialFileException partial && partial.BytesExpected.HasValue
                    && (options.IgnoreContentLength || (options.Raw && finalHead?.GetHeader("Transfer-Encoding") != null)))
                {
                    failure = null; // --ignore-content-length: whatever arrived before the close is the body
                }
            }

            var body = rawBody;
            if (failure == null && options.TransferEncoding && finalHead != null)
            {
                // --tr-encoding: undo gzip/deflate transfer-codings (HttpClient only removes chunked)
                var codings = string.Join(",", finalHead.GetHeaders("Transfer-Encoding")).Split(',')
                    .Select(t => t.Trim().ToLowerInvariant()).Where(t => t == "gzip" || t == "x-gzip" || t == "deflate").ToList();
                if (codings.Count > 0)
                {
                    body = DecodeOrFail(body, codings, "Transfer-Encoding", command);
                }
            }
            if (failure == null && options.Compressed)
            {
                body = DecodeOrFail(body, response.Content!.Headers.ContentEncoding, "Content-Encoding", command);
            }

            // -z: a document whose Last-Modified does not meet the time condition is not downloaded;
            // curl keeps the headers, drops the body and reports a simulated 304
            var timeConditionUnmet = failure == null && !isHead && status < 300 && !MeetsTimeCondition(options, finalHead?.GetHeader("Last-Modified")
                ?? (response.Content?.Headers.LastModified?.ToString("r", System.Globalization.CultureInfo.InvariantCulture)));
            if (timeConditionUnmet)
            {
                body = Array.Empty<byte>();
                rawBody = Array.Empty<byte>();
                trace?.Info("The requested document is not new enough (-z); simulating an HTTP 304 response.");
            }

            // -C/--continue-at: 416 means "already complete" (curl ignores the body); a reply without
            // Content-Range would corrupt the partial file (exit 33)
            var resuming = transfer.ResumeOffset > 0;
            if (resuming && failure == null)
            {
                var contentRange = finalHead?.GetHeader("Content-Range") ?? ContentRangeOf(response);
                if (status == 416)
                {
                    body = Array.Empty<byte>();
                }
                else if (contentRange == null && request.Method == HttpMethod.Get)
                {
                    if (announced.HasValue && announced.Value == transfer.ResumeOffset)
                    {
                        body = Array.Empty<byte>(); // "the entire document is already downloaded"
                    }
                    else
                    {
                        throw new CurlRangeErrorException(transfer.ResumeOffset, status, command);
                    }
                }
            }

            var (result, responseText, responseBinary) = CreateResult(response, options, isHead ? null : body, finalHead);
            result.Timings = timings;
            FillTimings(transfer, timings);

            if (trace != null)
            {
                if (responseBinary != null)
                {
                    trace.RecvData(responseBinary);
                }
                else
                {
                    trace.RecvData(responseText);
                }
                trace.Info($"Transfer complete: HTTP {result.StatusCode}, {rawBody.Length} bytes received.");
            }

            var etag = finalHead?.GetHeader("ETag") ?? result.GetHeader("ETag");
            if (!string.IsNullOrEmpty(options.EtagSave) && options.EtagSave != "-" && failure == null)
            {
                SaveEtag(options, etag);
            }

            var effectiveUrl = request.RequestUri!.AbsoluteUri;
            // a 416 while resuming means "already complete", which is not a failure even with -f
            var failOnHttp = status >= 400 && (options.FailOnError || options.FailWithBody) && !(resuming && status == 416);
            string? outputPath = null;
            string? writtenPath = null;
            // A 304 with nothing to write (no -i headers, no body) leaves an existing file alone: curl
            // only creates the output file when the first byte arrives (--etag-compare relies on it)
            var notModified = (status == 304 || timeConditionUnmet) && !includeHeaders && body.Length == 0;
            // -f writes the -i header blocks but never the error body; --fail-with-body keeps it
            var outputBody = failOnHttp && !options.FailWithBody ? Array.Empty<byte>() : (failure == null ? body : rawBody);
            var nothingToWrite = failOnHttp && !options.FailWithBody && !includeHeaders;
            if (!notModified && !nothingToWrite)
            {
                outputPath = TransferOutput.ResolveOutputPath(options, transfer, finalHead, effectiveUrl);
            }

            if (outputPath != null && (failure == null || !options.RemoveOnError))
            {
                var fromHeader = options.UseRemoteFileName && options.RemoteHeaderName && string.IsNullOrEmpty(options.OutputFile)
                                 && TransferOutput.ContentDispositionFileName(finalHead?.GetHeader("Content-Disposition") ?? result.GetHeader("Content-Disposition")) != null;
                // a resumed download, or a retry after an attempt that wrote no body, continues the file
                var append = resuming || (attempt?.AppendOutput ?? false);
                writtenPath = append ? outputPath : TransferOutput.PrepareTarget(outputPath, options, fromHeader);
                if (append)
                {
                    TransferOutput.EnsureDirectory(writtenPath, options, "-o/--output");
                }
                var bytes = BuildOutputBytes(includeHeaders ? headerText : null, outputBody, includeHeaders ? finalHead?.Trailers : null);
                TransferOutput.WriteFile(writtenPath, bytes, append, options, options.UseRemoteFileName && string.IsNullOrEmpty(options.OutputFile) ? "-O/--remote-name" : "-o/--output");
                if (attempt != null)
                {
                    attempt.WroteBody |= outputBody.Length > 0;
                    attempt.OutputPath = writtenPath;
                }
                result.OutputFiles.Add(writtenPath);
                if (options.RemoteTime && failure == null)
                {
                    TransferOutput.ApplyRemoteTime(writtenPath, finalHead?.GetHeader("Last-Modified") ?? result.GetHeader("Last-Modified"));
                }
            }

            var jarText = SaveCookies(transfer);

            if (failure == null && failOnHttp)
            {
                failure = new CurlHttpReturnedErrorException(status, finalHead?.ReasonPhrase ?? response.ReasonPhrase, responseText,
                    effectiveUrl, options.FailWithBody, command)
                {
                    ResponseHeaders = result.Headers
                };
            }

            // Standard output: -v log, -D - headers, -i/-I headers, body, -w text. A failed transfer has
            // written all but the -w text by now (-f never writes the error body; --fail-with-body does).
            var stdoutText = BuildStdoutText(options, verboseLog, headerText, etag, includeHeaders && !options.DiscardOutput, outputPath,
                (failOnHttp && !options.FailWithBody) || options.DiscardOutput ? null : responseText, finalHead?.Trailers);

            if (failure != null)
            {
                var failureContext = string.IsNullOrEmpty(options.WriteOut)
                    ? null
                    : BuildWriteOutContext(transfer, request, response, finalHead, result, rawBody.Length, writtenPath);
                AttachFailureOutput(failure, transfer, stdoutText, failureContext, jarText);
                throw failure;
            }

            result.BinaryData = responseBinary;
            if (timeConditionUnmet)
            {
                result.StatusCode = 304; // curl's %{response_code} for an unmet -z condition
            }

            var builder = new StringBuilder(stdoutText);

            if (!string.IsNullOrEmpty(options.WriteOut))
            {
                var context = BuildWriteOutContext(transfer, request, response, finalHead, result, rawBody.Length, writtenPath);
                if (timeConditionUnmet)
                {
                    context.ResponseCode = 304;
                }
                var expanded = WriteOutFormatter.Format(options.WriteOut!, context);
                builder.Append(expanded.Stdout);
                WriteStderr(options, expanded.Stderr);
            }

            if (jarText != null)
            {
                builder.Append(jarText);
            }

            result.Body = builder.ToString();
            return result;
        }

        /// <summary>
        /// curl's Curl_meets_timecondition(): with <c>-z</c>, a document is only wanted when its
        /// Last-Modified is newer (<c>-z DATE</c>), not newer (<c>-z -DATE</c>) or equal (<c>-z =DATE</c>).
        /// A reply without a usable Last-Modified always meets the condition.
        /// </summary>
        private static bool MeetsTimeCondition(CurlOptions options, string? lastModified)
        {
            if (!options.TimeCondition.HasValue || string.IsNullOrWhiteSpace(lastModified))
            {
                return true;
            }

            var document = CurlDate.Parse(lastModified!);
            if (!document.HasValue)
            {
                return true;
            }

            var condition = options.TimeCondition.Value.ToUnixTimeSeconds();
            switch (options.TimeConditionType)
            {
                case CurlTimeCondition.IfUnmodifiedSince:
                    return document.Value <= condition;
                case CurlTimeCondition.LastModified:
                    return document.Value == condition;
                default:
                    return document.Value > condition;
            }
        }

        /// <summary>What a failed transfer printed before its -w text.</summary>
        private sealed class FailureOutput
        {
            public string BeforeWriteOut = string.Empty;
        }

        /// <summary>Marks exceptions whose standard output and -w text were already attached.</summary>
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<Exception, object> FailureOutputAttached =
            new System.Runtime.CompilerServices.ConditionalWeakTable<Exception, object>();

        /// <summary>
        /// Puts what curl would have printed before failing on the exception: <paramref name="stdoutSoFar"/>
        /// (headers, body), the -w text expanded with <c>%{exitcode}</c>/<c>%{errormsg}</c> (its
        /// <c>%{stderr}</c> part goes to --stderr), and the <c>-c -</c> cookie jar. Runs once per exception.
        /// </summary>
        internal static void AttachFailureOutput(CurlException failure, HttpTransfer? transfer, string? stdoutSoFar,
            WriteOutContext? context, string? jarText, CurlOptions? options = null)
        {
            if (FailureOutputAttached.TryGetValue(failure, out _))
            {
                return;
            }
            var record = new FailureOutput { BeforeWriteOut = stdoutSoFar ?? string.Empty };
            FailureOutputAttached.Add(failure, record);

            options ??= transfer?.Options;
            if (options == null)
            {
                return;
            }

            var text = new StringBuilder(stdoutSoFar ?? string.Empty);
            if (!string.IsNullOrEmpty(options.WriteOut))
            {
                context ??= transfer != null ? BuildFailureWriteOutContext(transfer) : null;
                context ??= new WriteOutContext
                {
                    UrlNum = options.TransferNumber,
                    Url = options.Url,
                    UrlEffective = options.Url,
                    Method = (options.CustomMethod ?? options.Method ?? "GET").ToUpperInvariant(),
                };
                if (failure is CurlDotNet.Exceptions.CurlHttpException http && context.ResponseCode == 0)
                {
                    context.ResponseCode = http.StatusCode;
                }
                var message = failure.Message;
                var newline = message.IndexOf('\n');
                context.ExitCode = failure.CurlErrorCode ?? 1;
                context.ErrorMessage = newline < 0 ? message : message.Substring(0, newline);
                var expanded = WriteOutFormatter.Format(options.WriteOut!, context);
                WriteStderr(options, expanded.Stderr);
                if (expanded.Stdout.Length > 0)
                {
                    failure.Data[WriteOutDataKey] = expanded.Stdout;
                    text.Append(expanded.Stdout);
                }
            }

            text.Append(jarText);
            TransferStdout.Append(failure, text.ToString());
        }

        /// <summary>
        /// The -w context of a transfer that failed before a final response was read (too many redirects,
        /// a connection that broke): what the last recorded response head and request say.
        /// </summary>
        private static WriteOutContext BuildFailureWriteOutContext(HttpTransfer transfer)
        {
            var options = transfer.Options;
            var head = transfer.FinalHead;
            var uri = transfer.CurrentRequest?.RequestUri;
            var ctx = new WriteOutContext
            {
                ResponseCode = head?.StatusCode ?? 0,
                UrlNum = options.TransferNumber,
                HttpVersion = head == null ? "0" : (head.HttpVersion == "2.0" ? "2" : head.HttpVersion),
                ContentType = head?.GetHeader("Content-Type"),
                Url = transfer.OriginalUrl,
                UrlEffective = uri?.AbsoluteUri ?? options.Url,
                RedirectUrl = transfer.RedirectUrl ?? string.Empty,
                Method = transfer.CurrentRequest?.Method.Method ?? (options.CustomMethod ?? options.Method ?? "GET").ToUpperInvariant(),
                Scheme = uri?.Scheme.ToLowerInvariant() ?? string.Empty,
                NumRedirects = transfer.RedirectCount,
                ProxyUsed = transfer.CurrentViaProxy,
                NumConnects = transfer.Transcript.HasRecording ? transfer.Transcript.NewConnections : 0,
            };
            if (head != null)
            {
                ctx.Headers = head.Headers;
                ctx.NumHeaders = head.Headers.Count;
            }
            if (transfer.Transcript.HasRecording)
            {
                ctx.SizeHeader = transfer.Transcript.HeaderBytes;
            }
            ctx.TimeTotal = (DateTime.UtcNow - transfer.StartTime).TotalSeconds;
            return ctx;
        }

        /// <summary>
        /// Standard output of one transfer without the -w text, in curl's order: -v log, <c>-D -</c>
        /// headers, <c>--etag-save -</c>, <c>-i</c> headers, body, trailers. Headers and body only go
        /// to standard output when there is no output file.
        /// </summary>
        private static string BuildStdoutText(CurlOptions options, StringBuilder? verboseLog, string headerText, string? etag,
            bool includeHeaders, string? outputPath, string? responseText, string? trailers)
        {
            var builder = new StringBuilder();
            if (options.Verbose && verboseLog != null)
            {
                builder.AppendLine(verboseLog.ToString().TrimEnd());
            }

            if (options.DumpHeaderFile == "-")
            {
                builder.Append(headerText);
            }

            if (options.EtagSave == "-")
            {
                builder.Append(string.IsNullOrEmpty(etag) ? string.Empty : etag + "\n");
            }

            if (includeHeaders && outputPath == null)
            {
                if (options.Verbose && builder.Length > 0)
                    builder.AppendLine();
                builder.Append(headerText);
            }

            if (outputPath == null && !string.IsNullOrEmpty(responseText))
            {
                if (options.Verbose && !includeHeaders && builder.Length > 0)
                    builder.AppendLine();
                builder.Append(responseText);
            }

            if (outputPath == null && (includeHeaders || options.DumpHeaderFile == "-") && trailers != null)
            {
                builder.Append(trailers);
            }

            return builder.ToString();
        }

        /// <summary>
        /// --http0.9: when the reply had no status line, everything the server sent is the body.
        /// Returns null when the reply was not HTTP/0.9 (or was a HEAD request, which cannot be).
        /// </summary>
        private CurlResult? TryHttp09Result(HttpTransfer transfer, CurlOptions options)
        {
            var exchange = transfer.LastExchange;
            if (exchange == null || exchange.IsHeadRequest || options.HeadOnly || exchange.FinalHead != null)
            {
                return null;
            }

            var segment = exchange.Segments.LastOrDefault(s => s.Parser.TotalBytes > 0);
            if (segment == null)
            {
                return null;
            }

            var raw = segment.Parser.RawPrefix;
            var text = WireText.Latin1.GetString(raw, 0, Math.Min(raw.Length, 5));
            if (text.StartsWith("HTTP/", StringComparison.Ordinal))
            {
                return null;
            }

            var result = new CurlResult { StatusCode = 0, Command = options.OriginalCommand, Timings = transfer.Timings };
            var outputPath = TransferOutput.ResolveOutputPath(options, transfer, null, options.Url);
            if (outputPath != null)
            {
                var path = TransferOutput.PrepareTarget(outputPath, options, fromHeaderName: false);
                TransferOutput.WriteFile(path, raw, append: false, options, "-o/--output");
                result.OutputFiles.Add(path);
                result.Body = string.Empty;
            }
            else
            {
                result.Body = LooksBinary(raw) ? string.Empty : DecodeTextBody(raw, null);
                result.BinaryData = LooksBinary(raw) ? raw : null;
            }
            return result;
        }

        private static string? ContentRangeOf(HttpResponseMessage response) =>
            response.Content?.Headers.ContentRange?.ToString();

        /// <summary>Signals that --max-filesize was exceeded while reading.</summary>
        private sealed class FileSizeLimitReached : Exception
        {
        }

        private static async Task<(byte[] Body, Exception? Error)> ReadBodyAsync(HttpContent content, CancellationToken cancellationToken, long? maxFileSize)
        {
            var buffer = new MemoryStream();
            try
            {
#if NETSTANDARD2_0 || NET472 || NET48
                using var stream = await content.ReadAsStreamAsync().ConfigureAwait(false);
#else
                using var stream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
#endif
                var chunk = new byte[81920];
                while (true)
                {
                    // --max-time bounds the body download too
                    var n = await stream.ReadAsync(chunk, 0, chunk.Length, cancellationToken).ConfigureAwait(false);
                    if (n <= 0)
                    {
                        break;
                    }
                    buffer.Write(chunk, 0, n);
                    if (maxFileSize.HasValue && buffer.Length > maxFileSize.Value)
                    {
                        return (buffer.ToArray(), new FileSizeLimitReached());
                    }
                }
                return (buffer.ToArray(), null);
            }
            catch (OperationCanceledException ex)
            {
                return (buffer.ToArray(), ex);
            }
            catch (Exception ex) when (ex is IOException || ex is HttpRequestException)
            {
                return (buffer.ToArray(), ex);
            }
        }

        private static byte[] BuildOutputBytes(string? headerText, byte[] body, string? trailers)
        {
            if (headerText == null)
            {
                return body;
            }

            var head = WireText.Latin1.GetBytes(headerText);
            var tail = trailers == null ? Array.Empty<byte>() : WireText.Latin1.GetBytes(trailers);
            var combined = new byte[head.Length + body.Length + tail.Length];
            Buffer.BlockCopy(head, 0, combined, 0, head.Length);
            Buffer.BlockCopy(body, 0, combined, head.Length, body.Length);
            Buffer.BlockCopy(tail, 0, combined, head.Length + body.Length, tail.Length);
            return combined;
        }

        /// <summary>--etag-save: the ETag (or nothing) followed by a newline.</summary>
        private static void SaveEtag(CurlOptions options, string? etag)
        {
            TransferOutput.EnsureDirectory(options.EtagSave!, options, "--etag-save");
            var text = string.IsNullOrEmpty(etag) ? string.Empty : etag + "\n";
            TransferOutput.WriteFile(options.EtagSave!, WireText.Latin1.GetBytes(text), append: false, options, "--etag-save");
        }

        private static string? SaveCookies(HttpTransfer transfer)
        {
            return transfer.Cookies?.Save(transfer.Options.OriginalCommand);
        }

        /// <summary>The %{stderr} part of -w goes to --stderr FILE (or standard error).</summary>
        private static void WriteStderr(CurlOptions options, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (!string.IsNullOrEmpty(options.StderrFile) && options.StderrFile != "-")
            {
                try
                {
                    File.AppendAllText(options.StderrFile!, text);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    // curl falls back to the terminal when --stderr cannot be opened
                    Console.Error.Write(text);
                }
                return;
            }

            Console.Error.Write(text);
        }

        /// <summary>Fills CurlResult.Timings (milliseconds) from the recorded connection facts.</summary>
        private static void FillTimings(HttpTransfer transfer, CurlTimings timings)
        {
            var transcript = transfer.Transcript;
            var connection = transcript.Exchanges.Select(e => e.FirstNewConnection).FirstOrDefault(c => c != null);
            if (connection == null)
            {
                return;
            }
            timings.NameLookup = transcript.SecondsSinceStart(connection.ResolvedTimestamp) * 1000;
            timings.Connect = transcript.SecondsSinceStart(connection.ConnectedTimestamp) * 1000;
            if (connection.Secure)
            {
                timings.AppConnect = transcript.SecondsSinceStart(connection.ReadyTimestamp) * 1000;
            }
        }

        private static WriteOutContext BuildWriteOutContext(HttpTransfer transfer, HttpRequestMessage request, HttpResponseMessage response,
            RawResponseHead? finalHead, CurlResult result, long downloadSize, string? writtenPath)
        {
            var transcript = transfer.Transcript;
            var timings = transfer.Timings;
            var uri = request.RequestUri!;
            var ctx = new WriteOutContext
            {
                ResponseCode = (int)response.StatusCode,
                UrlNum = transfer.Options.TransferNumber,
                HttpVersion = finalHead != null
                    ? (finalHead.HttpVersion == "2.0" ? "2" : finalHead.HttpVersion)
                    : (response.Version.Major >= 2 ? response.Version.Major.ToString(System.Globalization.CultureInfo.InvariantCulture) : $"{response.Version.Major}.{response.Version.Minor}"),
                ContentType = finalHead != null ? finalHead.GetHeader("Content-Type") : response.Content?.Headers.ContentType?.ToString(),
                SizeDownload = downloadSize,
                SizeUpload = request.Content?.Headers.ContentLength ?? 0,
                Url = transfer.OriginalUrl,
                UrlEffective = uri.AbsoluteUri,
                RedirectUrl = transfer.RedirectUrl ?? string.Empty,
                Method = request.Method.Method,
                Scheme = uri.Scheme.ToLowerInvariant(),
                Referer = request.Headers.TryGetValues("Referer", out var referers) ? referers.FirstOrDefault() ?? string.Empty : string.Empty,
                NumRedirects = transfer.RedirectCount,
                FilenameEffective = writtenPath ?? string.Empty,
                ProxyUsed = transfer.CurrentViaProxy,
            };

            if (finalHead != null)
            {
                ctx.Headers = finalHead.Headers;
                ctx.NumHeaders = finalHead.Headers.Count;
            }
            else
            {
                foreach (var h in result.Headers)
                {
                    ctx.Headers.Add(new KeyValuePair<string, string>(h.Key, h.Value));
                }
                ctx.NumHeaders = result.Headers.Count;
            }

            if (transcript.HasRecording)
            {
                ctx.SizeHeader = transcript.HeaderBytes;
                var sent = transcript.Exchanges.Sum(e => e.BytesSent);
                ctx.SizeRequest = Math.Max(0, sent - ctx.SizeUpload * Math.Max(1, transfer.RequestsSent));
                ctx.NumConnects = transcript.NewConnections;

                var first = transcript.Exchanges.Select(e => e.FirstNewConnection).FirstOrDefault(c => c != null);
                if (first != null)
                {
                    ctx.TimeNameLookup = transcript.SecondsSinceStart(first.ResolvedTimestamp);
                    ctx.TimeConnect = transcript.SecondsSinceStart(first.ConnectedTimestamp);
                    ctx.TimeAppConnect = first.Secure ? transcript.SecondsSinceStart(first.ReadyTimestamp) : 0;
                }

                var last = transfer.LastExchange!;
                var connection = last.LastConnection;
                if (connection != null)
                {
                    ctx.LocalIp = connection.LocalEndPoint?.Address.ToString() ?? string.Empty;
                    ctx.LocalPort = connection.LocalEndPoint?.Port ?? 0;
                    ctx.RemoteIp = connection.RemoteEndPoint?.Address.ToString() ?? string.Empty;
                    ctx.RemotePort = connection.RemoteEndPoint?.Port ?? 0;
                }

                ctx.TimePreTransfer = transcript.SecondsSinceStart(last.FirstWriteTimestamp);
                ctx.TimeStartTransfer = transcript.SecondsSinceStart(last.FirstByteTimestamp);
                ctx.TimeRedirect = transfer.RedirectCount > 0 ? transcript.SecondsSinceStart(last.StartedTimestamp) : 0;
            }
            else
            {
                ctx.SizeHeader = Encoding.UTF8.GetByteCount(transfer.BuildHeaderText(response));
                ctx.NumConnects = 1;
                ctx.TimePreTransfer = timings.PreTransfer / 1000;
                ctx.TimeStartTransfer = timings.StartTransfer / 1000;
                ctx.TimeRedirect = timings.Redirect / 1000;
                if (uri.HostNameType == UriHostNameType.IPv4 || uri.HostNameType == UriHostNameType.IPv6)
                {
                    ctx.RemoteIp = uri.Host.Trim('[', ']');
                    ctx.RemotePort = uri.Port;
                }
            }

            ctx.TimeTotal = (DateTime.UtcNow - transfer.StartTime).TotalSeconds;
            ctx.TimePostTransfer = ctx.TimePreTransfer;
            if (ctx.TimeStartTransfer < ctx.TimePreTransfer) ctx.TimeStartTransfer = ctx.TimePreTransfer;
            if (ctx.TimeTotal < ctx.TimeStartTransfer) ctx.TimeTotal = ctx.TimeStartTransfer;
            return ctx;
        }

        private (CurlResult Result, string? ResponseText, byte[]? ResponseBinary) CreateResult(HttpResponseMessage response, CurlOptions options,
            byte[]? bodyBytes, RawResponseHead? finalHead)
        {
            // FIX: Use StringComparer.OrdinalIgnoreCase for case-insensitive headers
            var result = new CurlResult
            {
                StatusCode = (int)response.StatusCode,
                Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                Command = options.OriginalCommand
            };

            if (finalHead != null)
            {
                // Header values exactly as sent (HttpClient would split "Server: a b" into tokens);
                // repeated headers are joined with ", " as before.
                foreach (var header in finalHead.Headers)
                {
                    result.Headers[header.Key] = result.Headers.TryGetValue(header.Key, out var existing)
                        ? existing + ", " + header.Value
                        : header.Value;
                }
            }
            else
            {
                foreach (var header in response.Headers)
                {
                    result.Headers[header.Key] = string.Join(", ", header.Value);
                }
                if (response.Content != null)
                {
                    foreach (var header in response.Content.Headers)
                    {
                        result.Headers[header.Key] = string.Join(", ", header.Value);
                    }
                }
            }

            string? responseText = null;
            byte[]? responseBinary = null;

            // Always read raw bytes first so binary data served under a lying text Content-Type
            // (e.g. an .xlsx sent as text/plain) is never corrupted by string decoding; curl writes
            // wire bytes verbatim. HEAD (-I or -X HEAD) never has a body to print.
            if (response.Content != null && bodyBytes != null)
            {
                var charset = response.Content.Headers.ContentType?.CharSet;
                var mediaType = response.Content.Headers.ContentType?.MediaType;

                var text = mediaType == null
                    ? !options.ForceBinary && !LooksBinary(bodyBytes, charset)   // no Content-Type: sniff, like curl writes it either way
                    : IsTextContent(response.Content, options) && (IsUserForcedText(response.Content, options) || !LooksBinary(bodyBytes, charset));
                if (text)
                {
                    responseText = DecodeTextBody(bodyBytes, charset);
                }
                else
                {
                    responseBinary = bodyBytes;
                }
            }

            return (result, responseText, responseBinary);
        }

        /// <summary>
        /// Rebuilds the response header block the way curl -i prints it: status line, one header per
        /// line, CRLF line endings, no trailing blank line. Used when the raw bytes were not recorded.
        /// </summary>
        private string BuildHeaderBlock(HttpResponseMessage response)
        {
            var builder = new StringBuilder();
            builder.Append(BuildStatusLine(response)).Append("\r\n");
#if NET6_0_OR_GREATER
            // NonValidated hands back the header lines as received instead of re-joined parsed tokens
            // ("Server: test-server/fake swsclose" would otherwise print as "test-server/fake, swsclose").
            foreach (var header in response.Headers.NonValidated)
            {
                foreach (var value in header.Value)
                {
                    builder.Append(header.Key).Append(": ").Append(value).Append("\r\n");
                }
            }
            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers.NonValidated)
                {
                    foreach (var value in header.Value)
                    {
                        builder.Append(header.Key).Append(": ").Append(value).Append("\r\n");
                    }
                }
            }
#else
            foreach (var header in response.Headers)
            {
                builder.Append(header.Key).Append(": ").Append(string.Join(", ", header.Value)).Append("\r\n");
            }
            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                {
                    builder.Append(header.Key).Append(": ").Append(string.Join(", ", header.Value)).Append("\r\n");
                }
            }
#endif
            return builder.ToString().TrimEnd();
        }

        /// <summary>
        /// Decodes gzip, deflate (zlib or raw) and, where the framework supports it, Brotli bodies
        /// for --compressed. Unknown or identity encodings are returned untouched.
        /// </summary>
        private static byte[] DecodeContentEncoding(byte[] body, ICollection<string> encodings)
        {
            if (body.Length == 0 || encodings == null || encodings.Count == 0)
            {
                return body;
            }

            // Encodings are listed in the order applied; undo them in reverse.
            foreach (var encoding in encodings.Reverse())
            {
                switch (encoding.ToLowerInvariant())
                {
                    case "gzip":
                    case "x-gzip":
                        body = Inflate(body, s => new System.IO.Compression.GZipStream(s, System.IO.Compression.CompressionMode.Decompress));
                        break;
                    case "deflate":
                        // RFC 7230 says zlib-wrapped; some servers send raw deflate. Try zlib first.
                        try
                        {
                            body = Inflate(body, s =>
                            {
                                s.ReadByte(); s.ReadByte(); // skip the 2-byte zlib header
                                return new System.IO.Compression.DeflateStream(s, System.IO.Compression.CompressionMode.Decompress);
                            });
                        }
                        catch (System.IO.InvalidDataException)
                        {
                            body = Inflate(body, s => new System.IO.Compression.DeflateStream(s, System.IO.Compression.CompressionMode.Decompress));
                        }
                        break;
#if !NETSTANDARD2_0 && !NET472 && !NET48
                    case "br":
                        body = Inflate(body, s => new System.IO.Compression.BrotliStream(s, System.IO.Compression.CompressionMode.Decompress));
                        break;
#endif
                    default:
                        break; // identity or unknown: leave as-is, like curl
                }
            }
            return body;
        }

        /// <summary>Decodes a body; corrupt compressed data is curl's "Error while processing content unencoding" (61).</summary>
        private static byte[] DecodeOrFail(byte[] body, ICollection<string> encodings, string header, string? command)
        {
            try
            {
                return DecodeContentEncoding(body, encodings);
            }
            catch (Exception ex) when (ex is InvalidDataException || ex is IOException)
            {
                throw new CurlBadContentEncodingException(string.Join(", ", encodings),
                    $"Error while processing content unencoding: the body announced with {header}: {string.Join(", ", encodings)} could not be decoded ({ex.Message}). " +
                    "Drop --compressed/--tr-encoding to save the raw bytes, or report the broken encoding to the server owner", command);
            }
        }

        private static byte[] Inflate(byte[] body, Func<Stream, Stream> open)
        {
            using var input = new MemoryStream(body);
            using var decoder = open(input);
            using var output = new MemoryStream();
            decoder.CopyTo(output);
            return output.ToArray();
        }

        private string BuildStatusLine(HttpResponseMessage response)
        {
            var version = response.Version != null ? $"{response.Version.Major}.{response.Version.Minor}" : "1.1";
            var reason = response.ReasonPhrase;
            return string.IsNullOrEmpty(reason)
                ? $"HTTP/{version} {(int)response.StatusCode}"
                : $"HTTP/{version} {(int)response.StatusCode} {reason}";
        }

        private void AppendVerboseRequest(StringBuilder? verboseLog, HttpRequestMessage request)
        {
            if (verboseLog == null)
                return;

            var uri = request.RequestUri;
            verboseLog.AppendLine($"*   Trying {uri.Host}...");
            verboseLog.AppendLine($"* Connected to {uri.Host} ({uri.Host}) port {uri.Port} (#0)");
            verboseLog.AppendLine($"> {request.Method} {uri.PathAndQuery} HTTP/{request.Version?.ToString() ?? "1.1"}");

            foreach (var header in request.Headers)
            {
                verboseLog.AppendLine($"> {header.Key}: {string.Join(", ", header.Value)}");
            }
            if (request.Content != null)
            {
                foreach (var header in request.Content.Headers)
                {
                    verboseLog.AppendLine($"> {header.Key}: {string.Join(", ", header.Value)}");
                }
            }
            verboseLog.AppendLine(">");
        }

        private void AppendVerboseResponseHeaders(StringBuilder? verboseLog, HttpResponseMessage response)
        {
            if (verboseLog == null)
                return;

            verboseLog.AppendLine($"< {BuildStatusLine(response)}");
            foreach (var header in response.Headers)
            {
                verboseLog.AppendLine($"< {header.Key}: {string.Join(", ", header.Value)}");
            }
            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                {
                    verboseLog.AppendLine($"< {header.Key}: {string.Join(", ", header.Value)}");
                }
            }
            verboseLog.AppendLine("<");
        }

        /// <summary>
        /// Known binary MIME types that must never be treated as text, even if their
        /// MIME type string contains substrings like "xml" (e.g., openxmlformats).
        /// </summary>
        private static readonly HashSet<string> KnownBinaryMimeTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Microsoft Office Open XML (ZIP-based, contain "xml" in MIME type but are binary)
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",           // .xlsx
            "application/vnd.openxmlformats-officedocument.spreadsheetml.template",        // .xltx
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",     // .docx
            "application/vnd.openxmlformats-officedocument.wordprocessingml.template",     // .dotx
            "application/vnd.openxmlformats-officedocument.presentationml.presentation",   // .pptx
            "application/vnd.openxmlformats-officedocument.presentationml.template",       // .potx
            "application/vnd.openxmlformats-officedocument.presentationml.slideshow",      // .ppsx

            // Legacy Microsoft Office formats (binary)
            "application/vnd.ms-excel",                      // .xls
            "application/vnd.ms-powerpoint",                 // .ppt
            "application/msword",                            // .doc
            "application/vnd.ms-excel.sheet.macroEnabled.12", // .xlsm
            "application/vnd.ms-word.document.macroEnabled.12", // .docm
            "application/vnd.ms-powerpoint.presentation.macroEnabled.12", // .pptm

            // OpenDocument formats (ZIP-based)
            "application/vnd.oasis.opendocument.spreadsheet",    // .ods
            "application/vnd.oasis.opendocument.text",           // .odt
            "application/vnd.oasis.opendocument.presentation",   // .odp

            // Archive/compression formats
            "application/zip",                               // .zip
            "application/x-zip-compressed",                  // .zip (alternative)
            "application/gzip",                              // .gz
            "application/x-gzip",                            // .gz (alternative)
            "application/x-tar",                             // .tar
            "application/x-bzip2",                           // .bz2
            "application/x-7z-compressed",                   // .7z
            "application/x-rar-compressed",                  // .rar
            "application/vnd.rar",                           // .rar (IANA)

            // PDF
            "application/pdf",                               // .pdf

            // Executables and installers
            "application/x-msdownload",                      // .exe, .dll
            "application/x-msi",                             // .msi
            "application/vnd.apple.installer+xml",           // .pkg (despite +xml, this is binary)
            "application/x-apple-diskimage",                 // .dmg
            "application/x-deb",                             // .deb
            "application/x-rpm",                             // .rpm

            // Images
            "image/jpeg",                                    // .jpg, .jpeg
            "image/png",                                     // .png
            "image/gif",                                     // .gif
            "image/bmp",                                     // .bmp
            "image/webp",                                    // .webp
            "image/tiff",                                    // .tiff, .tif
            "image/x-icon",                                  // .ico
            "image/vnd.microsoft.icon",                      // .ico (IANA)

            // Audio
            "audio/mpeg",                                    // .mp3
            "audio/wav",                                     // .wav
            "audio/ogg",                                     // .ogg
            "audio/flac",                                    // .flac
            "audio/aac",                                     // .aac
            "audio/webm",                                    // .weba

            // Video
            "video/mp4",                                     // .mp4
            "video/mpeg",                                    // .mpeg
            "video/webm",                                    // .webm
            "video/x-msvideo",                               // .avi
            "video/quicktime",                               // .mov
            "video/x-matroska",                              // .mkv

            // Fonts
            "font/woff",                                     // .woff
            "font/woff2",                                    // .woff2
            "font/ttf",                                      // .ttf
            "font/otf",                                      // .otf
            "application/font-woff",                         // .woff (legacy)
            "application/font-woff2",                        // .woff2 (legacy)

            // Binary data / streams
            "application/octet-stream",                      // generic binary
            "application/x-binary",                          // generic binary
            "application/macbinary",                         // Mac binary

            // Database
            "application/x-sqlite3",                         // .sqlite, .db
            "application/vnd.sqlite3",                       // .sqlite (alternative)

            // Java
            "application/java-archive",                      // .jar
            "application/x-java-archive",                    // .jar (alternative)

            // WebAssembly
            "application/wasm",                              // .wasm

            // Protobuf
            "application/protobuf",                          // Protocol Buffers
            "application/x-protobuf",                        // Protocol Buffers (alternative)
            "application/grpc",                              // gRPC

            // Serialization formats (binary)
            "application/x-msgpack",                         // MessagePack
            "application/msgpack",                           // MessagePack (alternative)
            "application/cbor",                              // CBOR
            "application/avro",                              // Apache Avro
            "application/thrift",                            // Apache Thrift
        };

        /// <summary>
        /// Determines whether the HTTP content should be treated as text or binary.
        /// Uses precise MIME type matching, a comprehensive known-binary registry,
        /// and user-configurable overrides to avoid corrupting binary data.
        /// </summary>
        /// <param name="content">The HTTP response content to inspect.</param>
        /// <param name="options">The curl options that may contain user overrides for content type detection.</param>
        /// <returns><c>true</c> if the content should be read as text; <c>false</c> for binary.</returns>
        private bool IsTextContent(HttpContent content, CurlOptions options = null)
        {
            // User override: force all content as binary
            if (options?.ForceBinary == true) return false;

            var contentType = content.Headers.ContentType?.MediaType;

            // If no content type, assume binary to avoid corruption
            // (binary data read as text is corrupted, but text read as binary is lossless)
            if (contentType == null) return false;

            // User override: custom binary content types take highest priority
            if (options?.BinaryContentTypes != null && options.BinaryContentTypes.Count > 0)
            {
                foreach (var binaryType in options.BinaryContentTypes)
                {
                    if (contentType.Equals(binaryType, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
            }

            // User override: custom text content types
            if (options?.TextContentTypes != null && options.TextContentTypes.Count > 0)
            {
                foreach (var textType in options.TextContentTypes)
                {
                    if (contentType.Equals(textType, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            // Check against known binary MIME types (catches Office docs, PDFs, archives, etc.)
            if (KnownBinaryMimeTypes.Contains(contentType))
                return false;

            // All binary media type categories
            if (contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) ||
                contentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase) ||
                contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase) ||
                contentType.StartsWith("font/", StringComparison.OrdinalIgnoreCase))
                return false;

            // All text/* types are text
            if (contentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
                return true;

            // JSON types: application/json, application/vnd.api+json, etc.
            if (contentType.EndsWith("+json", StringComparison.OrdinalIgnoreCase) ||
                contentType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
                return true;

            // XML types: application/xml, application/soap+xml, application/rss+xml, etc.
            // But NOT application/vnd.openxmlformats-* (caught above in KnownBinaryMimeTypes)
            if (contentType.EndsWith("+xml", StringComparison.OrdinalIgnoreCase) ||
                contentType.Equals("application/xml", StringComparison.OrdinalIgnoreCase))
                return true;

            // JavaScript types
            if (contentType.Equals("application/javascript", StringComparison.OrdinalIgnoreCase) ||
                contentType.Equals("application/ecmascript", StringComparison.OrdinalIgnoreCase))
                return true;

            // GraphQL
            if (contentType.Equals("application/graphql", StringComparison.OrdinalIgnoreCase))
                return true;

            // YAML
            if (contentType.Equals("application/x-yaml", StringComparison.OrdinalIgnoreCase) ||
                contentType.Equals("application/yaml", StringComparison.OrdinalIgnoreCase))
                return true;

            // Form data (text-based)
            if (contentType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
                return true;

            // Everything else is binary (safe default: binary read as text corrupts, text read as binary is lossless)
            return false;
        }

        /// <summary>
        /// True when the user explicitly registered this response's media type as text
        /// via <see cref="CurlOptions.TextContentTypes"/>. Explicit user overrides beat
        /// content sniffing.
        /// </summary>
        private static bool IsUserForcedText(HttpContent content, CurlOptions options)
        {
            var contentType = content.Headers.ContentType?.MediaType;
            if (contentType == null || options?.TextContentTypes == null)
                return false;
            foreach (var textType in options.TextContentTypes)
            {
                if (contentType.Equals(textType, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Well-known binary file signatures. Servers frequently serve binary files under
        /// text/* or application/xml Content-Types (misconfigured MIME maps default unknown
        /// extensions to text/plain); real curl is immune because it never decodes bytes.
        /// Sniffing the payload lets us match that behavior.
        /// </summary>
        private static readonly byte[][] BinaryMagicNumbers =
        {
            new byte[] { 0x50, 0x4B, 0x03, 0x04 },       // ZIP (also .xlsx/.docx/.pptx/.jar)
            new byte[] { 0x50, 0x4B, 0x05, 0x06 },       // ZIP (empty archive)
            new byte[] { 0x50, 0x4B, 0x07, 0x08 },       // ZIP (spanned)
            new byte[] { 0x25, 0x50, 0x44, 0x46 },       // %PDF
            new byte[] { 0x4D, 0x5A },                   // MZ (.exe/.dll)
            new byte[] { 0x7F, 0x45, 0x4C, 0x46 },       // ELF
            new byte[] { 0x1F, 0x8B },                   // gzip
            new byte[] { 0x42, 0x5A, 0x68 },             // bzip2
            new byte[] { 0xFD, 0x37, 0x7A, 0x58, 0x5A }, // xz
            new byte[] { 0x37, 0x7A, 0xBC, 0xAF },       // 7z
            new byte[] { 0x52, 0x61, 0x72, 0x21 },       // RAR
            new byte[] { 0x89, 0x50, 0x4E, 0x47 },       // PNG
            new byte[] { 0xFF, 0xD8, 0xFF },             // JPEG
            new byte[] { 0x47, 0x49, 0x46, 0x38 },       // GIF8
            new byte[] { 0xD0, 0xCF, 0x11, 0xE0 },       // OLE2 (.xls/.doc/.ppt/.msi)
            new byte[] { 0x53, 0x51, 0x4C, 0x69, 0x74 }, // SQLite
            new byte[] { 0x00, 0x61, 0x73, 0x6D },       // WebAssembly
        };

        /// <summary>
        /// Sniffs a payload that headers classified as text for binary content, so a
        /// mislabeled binary file is stored as bytes instead of being corrupted by
        /// string decoding.
        /// </summary>
        /// <param name="data">The response body bytes.</param>
        /// <param name="charset">The declared charset, if any (UTF-16 text legitimately contains NUL bytes).</param>
        internal static bool LooksBinary(byte[] data, string charset = null)
        {
            if (data == null || data.Length == 0)
                return false;

            foreach (var magic in BinaryMagicNumbers)
            {
                if (data.Length >= magic.Length)
                {
                    var match = true;
                    for (var i = 0; i < magic.Length; i++)
                    {
                        if (data[i] != magic[i]) { match = false; break; }
                    }
                    if (match)
                        return true;
                }
            }

            // UTF-16/UTF-32 text contains NUL bytes by design - don't NUL-sniff it.
            if (charset != null && charset.IndexOf("utf-16", StringComparison.OrdinalIgnoreCase) >= 0)
                return false;
            if (charset != null && charset.IndexOf("utf-32", StringComparison.OrdinalIgnoreCase) >= 0)
                return false;
            if (data.Length >= 2 && ((data[0] == 0xFF && data[1] == 0xFE) || (data[0] == 0xFE && data[1] == 0xFF)))
                return false; // UTF-16 BOM

            // NUL byte in the first block => binary (same heuristic git and grep use)
            var scan = Math.Min(data.Length, 8000);
            for (var i = 0; i < scan; i++)
            {
                if (data[i] == 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Decodes a text body from its raw bytes honoring the declared charset and any
        /// byte-order mark, falling back to UTF-8.
        /// </summary>
        private static string DecodeTextBody(byte[] data, string charset)
        {
            Encoding encoding = Encoding.UTF8;
            if (!string.IsNullOrEmpty(charset))
            {
                try
                {
                    encoding = Encoding.GetEncoding(charset.Trim('"'));
                }
                catch (ArgumentException)
                {
                    // Unknown charset label - fall back to UTF-8
                }
            }

            using (var ms = new MemoryStream(data))
            using (var reader = new StreamReader(ms, encoding, detectEncodingFromByteOrderMarks: true))
            {
                return reader.ReadToEnd();
            }
        }

        private bool IsContentHeader(string headerName)
        {
            var contentHeaders = new[] { "Content-Type", "Content-Length", "Content-Encoding",
                "Content-Language", "Content-Location", "Content-Disposition",
                "Content-Range", "Content-MD5" };
            return contentHeaders.Contains(headerName, StringComparer.OrdinalIgnoreCase);
        }

        private RangeHeaderValue ParseRange(string range)
        {
            // Parse range like "0-499" or "500-"
            var parts = range.Split('-');
            if (parts.Length == 2)
            {
                var from = string.IsNullOrEmpty(parts[0]) ? (long?)null : long.Parse(parts[0]);
                var to = string.IsNullOrEmpty(parts[1]) ? (long?)null : long.Parse(parts[1]);

                return new RangeHeaderValue(from, to);
            }
            return null;
        }

        private static HttpClient CreateDefaultHttpClient()
        {
            // Pooled client with curl's transport behaviour (no automatic redirects, decompression or
            // cookies - issue #41 - plus wire recording on .NET 5+), see CurlTransport.
            return CurlTransport.CreateSharedClient();
        }
    }
}
