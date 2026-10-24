/***************************************************************************
 * Built-in Middleware Implementations for CurlDotNet
 *
 * Common middleware for logging, retry, caching, etc.
 *
 * Based on curl's callback system (src/tool_cb_*.c) by Daniel Stenberg and contributors
 * Original curl Copyright (C) 1996-2025, Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024-2025 Jacob Mellor and IronSoftware
 *
 * By Jacob Mellor
 * Sponsored by IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Middleware
{
    /// <summary>
    /// Writes one log line when a request starts (its command) and one when it ends (HTTP status and elapsed
    /// milliseconds, or the error message), to the console or your own logger.
    /// </summary>
    /// <remarks>
    /// <para>Lines written: <c>[CURL] Executing: {Command}</c> before the request, then either
    /// <c>[CURL] Success: Status={StatusCode}, Time={ms}ms</c> or <c>[CURL] Failed: {exception message}, Time={ms}ms</c>.
    /// The time covers everything registered after this middleware (retries included). Failures are logged and
    /// rethrown unchanged.</para>
    /// <para>Credentials in the command are masked before it is logged: the values of <c>-u</c>/<c>-U</c>,
    /// <c>--oauth2-bearer</c> and <c>--pass</c>, the password of <c>--cert FILE:PASSWORD</c>, <c>Authorization</c>,
    /// <c>Proxy-Authorization</c>, <c>Cookie</c> and API-key header values (including the bearer token
    /// <see cref="AuthenticationMiddleware"/> adds) and URL passwords all appear as <c>***</c>. Other values (for example
    /// a password inside <c>-d</c> form data) are logged as written. The instance holds no per-request state and is
    /// thread-safe as long as your logger is.</para>
    /// <para>Example: <c>CurlMiddlewarePipeline.CreateBuilder().Use(new LoggingMiddleware(msg =&gt; logger.LogInformation(msg))).WithHandler(ctx =&gt; Curl.ExecuteAsync(ctx.Command)).Build();</c></para>
    /// <para>Structured logging (Serilog, Microsoft.Extensions.Logging) guide:
    /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md</para>
    /// </remarks>
    /// <seealso cref="CurlMiddlewarePipelineBuilder.UseLogging(Action{string})"/>
    /// <seealso cref="TimingMiddleware"/>
    public class LoggingMiddleware : ICurlMiddleware
    {
        private readonly Action<string> _logger;

        /// <summary>
        /// Creates a logging middleware that sends every line to <paramref name="logger"/>, or to
        /// <see cref="Console.WriteLine(string)"/> when no logger is given.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>new LoggingMiddleware(line =&gt; Debug.WriteLine(line))</c> logs to the debugger output.</para>
        /// </remarks>
        /// <param name="logger">Receives each complete line, such as <c>[CURL] Success: Status=200, Time=87.2ms</c>;
        /// null (the default) uses the console.</param>
        /// <seealso cref="CurlMiddlewarePipelineBuilder.UseLogging(Action{string})"/>
        public LoggingMiddleware(Action<string> logger = null)
        {
            _logger = logger ?? Console.WriteLine;
        }

        /// <summary>
        /// Logs the command, runs the rest of the pipeline, then logs the status and elapsed time (or the failure,
        /// which is rethrown unchanged).
        /// </summary>
        /// <remarks>
        /// <para>Example: with a handler that returns 200, the logger receives <c>[CURL] Executing: curl https://api.example.com</c>
        /// and then <c>[CURL] Success: Status=200, Time=12.3ms</c>.</para>
        /// </remarks>
        /// <param name="context">The request; <see cref="CurlContext.Command"/> is logged as-is (null logs as empty).</param>
        /// <param name="next">Runs the rest of the pipeline.</param>
        /// <returns>The result from <paramref name="next"/>, unchanged.</returns>
        /// <exception cref="Exception">Whatever <paramref name="next"/> throws, after it has been logged. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="ICurlMiddleware.ExecuteAsync(CurlContext, Func{Task{CurlResult}})"/>
        public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
        {
            // never log credentials: -u/-U values, bearer tokens, Authorization/Cookie headers, URL passwords
            _logger($"[CURL] Executing: {ErrorText.RedactCommand(context.Command)}");
            var startTime = DateTime.UtcNow;

            try
            {
                var result = await next().ConfigureAwait(false);
                var elapsed = DateTime.UtcNow - startTime;
                _logger($"[CURL] Success: Status={result.StatusCode}, Time={elapsed.TotalMilliseconds}ms");
                return result;
            }
            catch (Exception ex)
            {
                var elapsed = DateTime.UtcNow - startTime;
                _logger($"[CURL] Failed: {ex.Message}, Time={elapsed.TotalMilliseconds}ms");
                throw;
            }
        }
    }

    /// <summary>
    /// Re-runs the rest of the pipeline when a request fails transiently (HTTP 5xx or 429 responses, DNS,
    /// connect, timeout and network errors), waiting longer before each attempt (exponential back-off).
    /// </summary>
    /// <remarks>
    /// <para>Attempts: the request runs once, then up to <c>maxRetries</c> more times. The wait before retry
    /// <c>n</c> (1-based) is <c>initialDelay × 2^(n-1)</c>: with the defaults (3 retries, 1 second) that is 1 s, 2 s
    /// and 4 s. Waits honour <see cref="CurlContext.CancellationToken"/>.</para>
    /// <para>What is retried: a returned <see cref="CurlResult"/> with status 500-599 or 429, and any
    /// <see cref="CurlException"/> whose <see cref="CurlException.IsRetryable"/> is true (exit codes 6, 7, 28, 52, 55, 56
    /// and <c>-f</c> HTTP errors 429/5xx). Every other exception, including your own and non-curl exceptions, is
    /// rethrown immediately. A <c>Retry-After</c> header is not read; the computed back-off is used.</para>
    /// <para>When attempts run out: for a 5xx/429 response the last response is returned (check
    /// <see cref="CurlResult.StatusCode"/>); for an exception, <see cref="CurlRetryException"/> is thrown with the last
    /// failure as <see cref="Exception.InnerException"/> and <see cref="CurlRetryException.LastAttemptException"/>.</para>
    /// <para>Place it after logging/timing (so they see one logical request) and before
    /// <see cref="AuthenticationMiddleware"/> (so each attempt gets a fresh token). curl's own <c>--retry N</c> option
    /// does the same inside a single command without middleware. The instance keeps no per-request state and is
    /// thread-safe.</para>
    /// <para>Example: <c>builder.Use(new RetryMiddleware(maxRetries: 2, initialDelay: TimeSpan.FromMilliseconds(250)))</c>
    /// tries a request up to 3 times, waiting 0.25 s and then 0.5 s.</para>
    /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/advanced.md</para>
    /// </remarks>
    /// <seealso cref="CurlMiddlewarePipelineBuilder.UseRetry(int, TimeSpan?)"/>
    /// <seealso cref="CurlRetryException"/>
    /// <seealso cref="CurlException.IsRetryable"/>
    public class RetryMiddleware : ICurlMiddleware
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _initialDelay;

        /// <summary>
        /// Creates a retry middleware that allows <paramref name="maxRetries"/> retries after the first attempt,
        /// waiting <paramref name="initialDelay"/> before the first retry and doubling the wait each time.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>new RetryMiddleware()</c> = 3 retries at 1 s, 2 s, 4 s; <c>new RetryMiddleware(0)</c> never
        /// retries (useful to switch retrying off by configuration).</para>
        /// </remarks>
        /// <param name="maxRetries">Retries after the first attempt, 0 or more; default 3. A negative value means no
        /// attempt is made at all and every request fails with <see cref="CurlRetryException"/>.</param>
        /// <param name="initialDelay">Wait before the first retry; null means 1 second, <see cref="TimeSpan.Zero"/>
        /// retries immediately. Must not be negative.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialDelay"/> is negative. Fix: pass
        /// <see cref="TimeSpan.Zero"/> or a positive value. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/advanced.md</exception>
        /// <seealso cref="CurlMiddlewarePipelineBuilder.UseRetry(int, TimeSpan?)"/>
        public RetryMiddleware(int maxRetries = 3, TimeSpan? initialDelay = null)
        {
            if (initialDelay.HasValue && initialDelay.Value < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(initialDelay), initialDelay.Value, CurlException.Compose(
                    $"RetryMiddleware was given a negative initial delay ({initialDelay.Value}), which Task.Delay would reject on the first retry.",
                    "Pass TimeSpan.Zero to retry immediately, or a positive delay such as TimeSpan.FromSeconds(1).",
                    DocLinks.Base + "guides/advanced.md"));
            }
            _maxRetries = maxRetries;
            _initialDelay = initialDelay ?? TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Runs the rest of the pipeline, retrying with exponential back-off on 5xx/429 results and retryable curl
        /// exceptions, and returns the first acceptable result.
        /// </summary>
        /// <remarks>
        /// <para>Each retry calls <paramref name="next"/> again with the same <paramref name="context"/>, so changes later
        /// middleware made to it during an attempt are still there on the next attempt.</para>
        /// <para>Example: a handler that throws <see cref="CurlCouldntConnectException"/> twice and then returns 200 gives
        /// 200 after two waits.</para>
        /// </remarks>
        /// <param name="context">The request; its <see cref="CurlContext.CancellationToken"/> stops the waits.</param>
        /// <param name="next">Runs the rest of the pipeline; called up to <c>maxRetries + 1</c> times.</param>
        /// <returns>The first result that is not 5xx/429, or the last result when every attempt returned 5xx/429.</returns>
        /// <exception cref="CurlRetryException">Every attempt threw a retryable <see cref="CurlException"/>; the message names
        /// the URL, the number of attempts and the last error, and <see cref="Exception.InnerException"/> is that error.
        /// Fix: address the last error (see its own message) or allow more or longer retries. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <exception cref="CurlAbortedByCallbackException">The token was cancelled during a back-off wait (curl exit code 42).
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</exception>
        /// <exception cref="Exception">Any non-retryable exception from <paramref name="next"/>, rethrown on the first occurrence.</exception>
        /// <seealso cref="CurlException.IsRetryable"/>
        public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
        {
            Exception lastException = null;

            for (int attempt = 0; attempt <= _maxRetries; attempt++)
            {
                try
                {
                    var result = await next().ConfigureAwait(false);

                    // Check if HTTP error is retryable
                    if (result.StatusCode >= 500 || result.StatusCode == 429)
                    {
                        if (attempt < _maxRetries)
                        {
                            var delay = TimeSpan.FromMilliseconds(_initialDelay.TotalMilliseconds * Math.Pow(2, attempt));
                            await MiddlewareDelay.WaitAsync(delay, context, $"RetryMiddleware was waiting {delay.TotalSeconds.ToString("0.###", CultureInfo.InvariantCulture)} s before retry {attempt + 1} of {_maxRetries} (the previous attempt returned HTTP {result.StatusCode})").ConfigureAwait(false);
                            continue;
                        }
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    // Check if exception is retryable
                    bool isRetryable = ex is CurlException curlEx && curlEx.IsRetryable();

                    if (!isRetryable)
                    {
                        throw;
                    }

                    lastException = ex;
                    if (attempt < _maxRetries)
                    {
                        var delay = TimeSpan.FromMilliseconds(_initialDelay.TotalMilliseconds * Math.Pow(2, attempt));
                        await MiddlewareDelay.WaitAsync(delay, context, $"RetryMiddleware was waiting {delay.TotalSeconds.ToString("0.###", CultureInfo.InvariantCulture)} s before retry {attempt + 1} of {_maxRetries} (the previous attempt failed with {ex.GetType().Name})").ConfigureAwait(false);
                        continue;
                    }

                    // If we are out of retries, we swallow the exception here
                    // The loop will finish and throw CurlRetryException
                }
            }

            throw new CurlRetryException(RetryFailureMessage(context, lastException), context.Command, _maxRetries, lastException);
        }

        private string RetryFailureMessage(CurlContext context, Exception lastException)
        {
            var target = ErrorText.SafeUrl(context.Options?.Url ?? ErrorText.UrlFromCommand(context.Command));
            if (lastException == null)
            {
                return CurlException.Compose(
                    $"Failed after {_maxRetries} retries: RetryMiddleware for {target} was created with maxRetries = {_maxRetries}, so the request was never attempted.",
                    "Pass 0 or more to RetryMiddleware / UseRetry (0 means one attempt and no retries).",
                    DocLinks.Base + "guides/advanced.md");
            }

            var last = lastException is CurlException curl ? curl.WhatHappened : lastException.Message;
            var fix = lastException is CurlException withFix && withFix.Suggestions.Count > 0 ? withFix.Suggestions[0] + " " : string.Empty;
            return CurlException.Compose(
                $"Failed after {_maxRetries} retries: {target} failed on all {_maxRetries + 1} attempts; the last attempt threw {lastException.GetType().Name}: {last}",
                fix + "If the failure is temporary, allow more retries or a longer back-off, for example UseRetry(5, TimeSpan.FromSeconds(2)); InnerException holds the last error.",
                DocLinks.HowToErrorHandling);
        }
    }

    /// <summary>
    /// Records how long the rest of the pipeline took and writes it to <see cref="CurlTimings.Total"/> on the result
    /// and to <c>TimingStart</c>/<c>TimingEnd</c>/<c>TimingDuration</c> in <see cref="CurlContext.Properties"/>.
    /// </summary>
    /// <remarks>
    /// <para>The measured time covers everything after this middleware: retries, rate-limit waits and the transfer,
    /// so it replaces the transfer's own <see cref="CurlTimings.Total"/> (DNS/connect/TLS timings are kept). Put it
    /// first to time the whole logical request, or last to time just the transfer. Nothing is recorded when the
    /// request throws. Stateless and thread-safe.</para>
    /// <para>Properties written: <c>TimingStart</c> and <c>TimingEnd</c> (<see cref="DateTime"/>, UTC) and
    /// <c>TimingDuration</c> (<see cref="TimeSpan"/>); read them with <c>context.GetProperty&lt;TimeSpan&gt;("TimingDuration")</c>.</para>
    /// <para>Example: <c>var result = await CurlMiddlewarePipeline.CreateBuilder().UseTiming().WithHandler(ctx =&gt; Curl.ExecuteAsync(ctx.Command)).Build().ExecuteAsync(ctx);</c>
    /// then <c>Console.WriteLine($"{result.Timings.Total:F0} ms");</c>.</para>
    /// <para>Metrics guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md</para>
    /// </remarks>
    /// <seealso cref="CurlTimings"/>
    /// <seealso cref="CurlMiddlewarePipelineBuilder.UseTiming"/>
    /// <seealso cref="LoggingMiddleware"/>
    public class TimingMiddleware : ICurlMiddleware
    {
        /// <summary>
        /// Notes the start time, runs the rest of the pipeline, then stores the elapsed time on the result and in the
        /// context properties.
        /// </summary>
        /// <remarks>
        /// <para>When the result has no <see cref="CurlResult.Timings"/>, a new <see cref="CurlTimings"/> is created with only
        /// <see cref="CurlTimings.Total"/> set.</para>
        /// <para>Example: <c>var took = context.GetProperty&lt;TimeSpan&gt;("TimingDuration");</c> after the call.</para>
        /// </remarks>
        /// <param name="context">The request; receives the <c>Timing*</c> properties.</param>
        /// <param name="next">Runs the rest of the pipeline.</param>
        /// <returns>The same result object, with <see cref="CurlTimings.Total"/> set to the elapsed milliseconds.</returns>
        /// <exception cref="Exception">Whatever <paramref name="next"/> throws, unchanged (no timing is recorded then).</exception>
        /// <seealso cref="CurlContext.GetProperty{T}(string)"/>
        public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
        {
            var startTime = DateTime.UtcNow;
            context.Properties["TimingStart"] = startTime;

            var result = await next().ConfigureAwait(false);

            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;

            // Add timing to result
            if (result.Timings == null)
            {
                result.Timings = new CurlTimings();
            }
            result.Timings.Total = duration.TotalMilliseconds;

            // Add to context for other middleware
            context.Properties["TimingEnd"] = endTime;
            context.Properties["TimingDuration"] = duration;

            return result;
        }
    }

    /// <summary>
    /// Answers repeated GET requests from an in-memory cache for a fixed time (5 minutes by default) instead of
    /// sending them again; only successful (2xx) results are stored.
    /// </summary>
    /// <remarks>
    /// <para>Cache key: <see cref="CurlContext.Command"/>, or <see cref="CurlOptions.Url"/> when there is no command, so two
    /// commands that differ only in header order are cached separately. Requests whose
    /// <see cref="CurlOptions.Method"/> is set to anything other than <c>GET</c> bypass the cache; requests without
    /// options (command only) are treated as GET, so do not use it in front of <c>-X POST</c>/<c>-d</c> commands
    /// unless the context also carries options.</para>
    /// <para>Scope: the cache is a static, process-wide <see cref="ConcurrentDictionary{TKey, TValue}"/> shared by every
    /// <see cref="CachingMiddleware"/> instance and every pipeline; each instance only decides the time-to-live of the
    /// entries it stores. Expired entries are skipped on lookup and swept when more than 100 entries exist. Clear it with
    /// <see cref="ClearCache"/> (for example between tests). A cache hit returns a copy of the stored result with the same
    /// status, body, headers and command; <see cref="CurlResult.BinaryData"/> and <see cref="CurlResult.Timings"/> are
    /// shared with the cached instance, so do not modify them. Thread-safe.</para>
    /// <para>Example: <c>CurlMiddlewarePipeline.CreateBuilder().UseCaching(TimeSpan.FromSeconds(30)).WithHandler(ctx =&gt; Curl.ExecuteAsync(ctx.Command)).Build()</c>;
    /// calling it twice with <c>curl https://api.example.com/config</c> within 30 seconds sends one request.</para>
    /// </remarks>
    /// <seealso cref="CurlMiddlewarePipelineBuilder.UseCaching(TimeSpan?)"/>
    /// <seealso cref="ClearCache"/>
    public class CachingMiddleware : ICurlMiddleware
    {
        private static readonly ConcurrentDictionary<string, CacheEntry> _cache = new ConcurrentDictionary<string, CacheEntry>();
        private readonly TimeSpan _ttl;

        /// <summary>
        /// Creates a caching middleware whose stored results are reused for <paramref name="ttl"/> after they were
        /// fetched (5 minutes when no value is given).
        /// </summary>
        /// <remarks>
        /// <para>A zero or negative time-to-live stores entries that are already expired, so nothing is ever served from
        /// the cache.</para>
        /// <para>Example: <c>new CachingMiddleware(TimeSpan.FromMinutes(1))</c></para>
        /// </remarks>
        /// <param name="ttl">How long a successful result is reused; null means 5 minutes.</param>
        /// <seealso cref="CurlMiddlewarePipelineBuilder.UseCaching(TimeSpan?)"/>
        public CachingMiddleware(TimeSpan? ttl = null)
        {
            _ttl = ttl ?? TimeSpan.FromMinutes(5);
        }

        /// <summary>
        /// Returns a cached copy when an unexpired result exists for this request; otherwise runs the rest of the pipeline
        /// and stores the result if it is a 2xx.
        /// </summary>
        /// <remarks>
        /// <para>A cache hit short-circuits: later middleware and the handler do not run.</para>
        /// <para>Example: <c>var first = await pipeline.ExecuteAsync(ctx); var second = await pipeline.ExecuteAsync(new CurlContext { Command = ctx.Command });</c>
        /// sends one request when the first was a 200.</para>
        /// </remarks>
        /// <param name="context">The request; its command (or URL) is the cache key and its method decides whether caching applies.</param>
        /// <param name="next">Runs the rest of the pipeline on a cache miss.</param>
        /// <returns>A copy of the cached result, or the fresh result from <paramref name="next"/>.</returns>
        /// <exception cref="Exception">Whatever <paramref name="next"/> throws on a cache miss, unchanged (nothing is cached).</exception>
        /// <seealso cref="ClearCache"/>
        public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
        {
            // Only cache GET requests
            if (context.Options?.Method != null && context.Options.Method != "GET")
            {
                return await next().ConfigureAwait(false);
            }

            var cacheKey = GetCacheKey(context);

            // Check cache
            if (_cache.TryGetValue(cacheKey, out var entry) && entry.Expiry > DateTime.UtcNow)
            {
                // Clone the cached result
                return CloneResult(entry.Result);
            }

            // Execute and cache
            var result = await next().ConfigureAwait(false);

            if (result.IsSuccess)
            {
                _cache[cacheKey] = new CacheEntry
                {
                    Result = result,
                    Expiry = DateTime.UtcNow.Add(_ttl)
                };

                // Clean expired entries periodically
                if (_cache.Count > 100)
                {
                    CleanExpiredEntries();
                }
            }

            return result;
        }

        private string GetCacheKey(CurlContext context)
        {
            // Simple cache key based on command
            // In production, parse URL and headers for better key
            return context.Command ?? context.Options?.Url ?? "";
        }

        private CurlResult CloneResult(CurlResult original)
        {
            // Simple clone - in production use proper cloning
            return new CurlResult
            {
                StatusCode = original.StatusCode,
                Body = original.Body,
                Headers = new Dictionary<string, string>(original.Headers),
                BinaryData = original.BinaryData,
                Command = original.Command,
                Timings = original.Timings
            };
        }

        private void CleanExpiredEntries()
        {
            var expired = _cache.Where(kvp => kvp.Value.Expiry < DateTime.UtcNow)
                                .Select(kvp => kvp.Key)
                                .ToList();

            foreach (var key in expired)
            {
                _cache.TryRemove(key, out _);
            }
        }

        private class CacheEntry
        {
            public CurlResult Result { get; set; }
            public DateTime Expiry { get; set; }
        }

        /// <summary>
        /// Removes every cached result for all <see cref="CachingMiddleware"/> instances in the process, so the next
        /// request of each kind is sent again.
        /// </summary>
        /// <remarks>
        /// <para>Thread-safe; requests running at the same time may still store their results afterwards. Call it in test
        /// set-up, or after a write that makes cached GET results stale.</para>
        /// <para>Example: <c>await Curl.ExecuteAsync("curl -X PUT -d @config.json https://api.example.com/config"); CachingMiddleware.ClearCache();</c></para>
        /// </remarks>
        /// <seealso cref="CachingMiddleware"/>
        public static void ClearCache()
        {
            _cache.Clear();
        }
    }

    /// <summary>
    /// Adds <c>Authorization: Bearer &lt;token&gt;</c> to every request, asking your token provider for the token each
    /// time (so a provider that refreshes expired tokens keeps requests authorised).
    /// </summary>
    /// <remarks>
    /// <para>Before each request the provider is awaited with the <see cref="CurlContext"/>. A non-empty token is written
    /// to <c>context.Options.Headers["Authorization"]</c> (creating <see cref="CurlContext.Options"/> when it is null)
    /// and, when the command text does not already mention <c>Authorization</c>, appended to
    /// <see cref="CurlContext.Command"/> as <c>-H 'Authorization: Bearer &lt;token&gt;'</c>, so it works with handlers that
    /// send either the options or the command. A null or empty token sends the request without the header.</para>
    /// <para>Exceptions thrown by your provider propagate unchanged and the request is not sent. The token ends up in the
    /// command text so that handlers which run <see cref="CurlContext.Command"/> send it; <see cref="LoggingMiddleware"/>
    /// and the exception log helpers (<c>ToDetailedString</c>, <c>ToJson</c>, <c>GetDiagnosticInfo</c>) print it as
    /// <c>Authorization: ***</c>, wherever the logging middleware sits in the pipeline. If your own code logs
    /// <see cref="CurlContext.Command"/>, mask it the same way. The middleware itself is stateless and thread-safe if your
    /// provider is.</para>
    /// <para>Example: <c>builder.UseAuthentication(async ctx =&gt; (await credential.GetTokenAsync(scopes, ctx.CancellationToken)).Token);</c></para>
    /// <para>Bearer token tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-bearer-token-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlMiddlewarePipelineBuilder.UseAuthentication(Func{CurlContext, Task{string}})"/>
    /// <seealso cref="CurlRequestBuilder.WithBearerToken(string)"/>
    public class AuthenticationMiddleware : ICurlMiddleware
    {
        private readonly Func<CurlContext, Task<string>> _tokenProvider;

        /// <summary>
        /// Creates the middleware with the function that supplies a bearer token for each request.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>new AuthenticationMiddleware(ctx =&gt; Task.FromResult(Environment.GetEnvironmentVariable("API_TOKEN")))</c></para>
        /// </remarks>
        /// <param name="tokenProvider">Returns the token (without the <c>Bearer </c> prefix) for the given request; return
        /// null or empty to send no header. Must not be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tokenProvider"/> is null. Fix: pass a function that returns
        /// the token. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-bearer-token-csharp/README.md</exception>
        /// <seealso cref="CurlMiddlewarePipelineBuilder.UseAuthentication(Func{CurlContext, Task{string}})"/>
        public AuthenticationMiddleware(Func<CurlContext, Task<string>> tokenProvider)
        {
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider), CurlException.Compose(
                "AuthenticationMiddleware was created with a null token provider, so it cannot add an Authorization header.",
                "Pass a function that returns the bearer token, for example ctx => Task.FromResult(myToken).",
                DocLinks.HowToBearerToken));
        }

        /// <summary>
        /// Gets a token from the provider, adds it as a bearer <c>Authorization</c> header to the options and the command,
        /// then runs the rest of the pipeline.
        /// </summary>
        /// <remarks>
        /// <para>Example: with a provider returning <c>abc</c> and the command <c>curl https://api.example.com</c>, the handler
        /// sees <c>curl https://api.example.com -H 'Authorization: Bearer abc'</c> and
        /// <c>context.Options.Headers["Authorization"] == "Bearer abc"</c>.</para>
        /// </remarks>
        /// <param name="context">The request to authorise; <see cref="CurlContext.Options"/> and <see cref="CurlContext.Command"/>
        /// are updated in place (a null command is left null).</param>
        /// <param name="next">Runs the rest of the pipeline.</param>
        /// <returns>The result from <paramref name="next"/>, unchanged.</returns>
        /// <exception cref="Exception">Whatever the token provider or <paramref name="next"/> throws, unchanged. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="CurlContext.Options"/>
        public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
        {
            // Get token
            var token = await _tokenProvider(context).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(token))
            {
                // Add authorization header to options
                if (context.Options == null)
                {
                    context.Options = new CurlOptions();
                }

                if (context.Options.Headers == null)
                {
                    context.Options.Headers = new Dictionary<string, string>();
                }

                context.Options.Headers["Authorization"] = $"Bearer {token}";

                // Update command if needed (a context without a command is described by its options only)
                if (context.Command != null && !context.Command.Contains("Authorization"))
                {
                    context.Command = context.Command + $" -H 'Authorization: Bearer {token}'";
                }
            }

            return await next().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Delays requests so that at most a given number complete in any one-second window, to stay under an API's
    /// requests-per-second limit (and avoid HTTP 429 responses).
    /// </summary>
    /// <remarks>
    /// <para>How it counts: the middleware remembers when each request finished. Before a request starts, if
    /// <c>requestsPerSecond</c> requests finished within the last second, it waits until the oldest of them is one second
    /// old. Requests still in flight are not counted, so many requests started at the same moment (for example with
    /// <c>Task.WhenAll</c>) can exceed the limit; limit concurrency separately (a <see cref="SemaphoreSlim"/>) if you
    /// fire requests in parallel.</para>
    /// <para>The limit applies per instance: share one instance (one pipeline) between all callers of the same API.
    /// The bookkeeping is lock-protected, so the instance is thread-safe. A cancelled
    /// <see cref="CurlContext.CancellationToken"/> during the wait ends the request with
    /// <see cref="CurlAbortedByCallbackException"/> (curl exit code 42).</para>
    /// <para>Example: <c>builder.UseRateLimit(10)</c>, then a loop of 50 sequential calls takes about 5 seconds.</para>
    /// <para>Retries and back-off guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/advanced.md</para>
    /// </remarks>
    /// <seealso cref="CurlMiddlewarePipelineBuilder.UseRateLimit(int)"/>
    /// <seealso cref="RetryMiddleware"/>
    public class RateLimitMiddleware : ICurlMiddleware
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly int _requestsPerSecond;
        private readonly Queue<DateTime> _requestTimes = new Queue<DateTime>();
        private readonly object _lock = new object();

        /// <summary>
        /// Creates a rate limiter that allows <paramref name="requestsPerSecond"/> requests to complete per second.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>new RateLimitMiddleware(2)</c> spaces sequential requests about half a second apart once two
        /// have completed within a second.</para>
        /// </remarks>
        /// <param name="requestsPerSecond">Requests per second, 1 or more (for example the limit in the API's documentation).</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="requestsPerSecond"/> is 0 or negative. Fix: pass 1 or
        /// more. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/advanced.md</exception>
        /// <seealso cref="CurlMiddlewarePipelineBuilder.UseRateLimit(int)"/>
        public RateLimitMiddleware(int requestsPerSecond)
        {
            if (requestsPerSecond < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(requestsPerSecond), requestsPerSecond, CurlException.Compose(
                    $"RateLimitMiddleware was given requestsPerSecond = {requestsPerSecond}; at least 1 request per second must be allowed.",
                    "Pass 1 or more (for example the limit from the API's documentation). To pause all requests, do not call the pipeline.",
                    DocLinks.Base + "guides/advanced.md"));
            }
            _requestsPerSecond = requestsPerSecond;
            _semaphore = new SemaphoreSlim(requestsPerSecond, requestsPerSecond);
        }

        /// <summary>
        /// Waits, if needed, until another request may run within the per-second limit, then runs the rest of the pipeline
        /// and records when it finished (also when it failed).
        /// </summary>
        /// <remarks>
        /// <para>Example: the 11th request within one second on a <c>UseRateLimit(10)</c> pipeline waits until the first of
        /// those ten is a second old.</para>
        /// </remarks>
        /// <param name="context">The request; its <see cref="CurlContext.CancellationToken"/> cancels the wait.</param>
        /// <param name="next">Runs the rest of the pipeline.</param>
        /// <returns>The result from <paramref name="next"/>, unchanged.</returns>
        /// <exception cref="CurlAbortedByCallbackException">The token was cancelled while waiting for a slot (exit code 42).
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</exception>
        /// <exception cref="Exception">Whatever <paramref name="next"/> throws, unchanged.</exception>
        /// <seealso cref="RateLimitMiddleware"/>
        public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
        {
            await WaitIfNeeded(context).ConfigureAwait(false);

            try
            {
                return await next().ConfigureAwait(false);
            }
            finally
            {
                RecordRequest();
            }
        }

        private async Task WaitIfNeeded(CurlContext context)
        {
            TimeSpan waitTime = TimeSpan.Zero;

            lock (_lock)
            {
                // Remove old requests outside the window
                var cutoff = DateTime.UtcNow.AddSeconds(-1);
                while (_requestTimes.Count > 0 && _requestTimes.Peek() < cutoff)
                {
                    _requestTimes.Dequeue();
                }

                // If at limit, calculate wait time
                if (_requestTimes.Count >= _requestsPerSecond)
                {
                    var oldestRequest = _requestTimes.Peek();
                    waitTime = oldestRequest.AddSeconds(1) - DateTime.UtcNow;
                }
            }

            // Wait outside the lock to avoid CS1996 error
            if (waitTime > TimeSpan.Zero)
            {
                await MiddlewareDelay.WaitAsync(waitTime, context,
                    $"RateLimitMiddleware was waiting {waitTime.TotalMilliseconds.ToString("0", CultureInfo.InvariantCulture)} ms for a slot under its limit of {_requestsPerSecond} requests per second").ConfigureAwait(false);
            }
        }

        private void RecordRequest()
        {
            lock (_lock)
            {
                _requestTimes.Enqueue(DateTime.UtcNow);

                // Keep only requests in the last second
                var cutoff = DateTime.UtcNow.AddSeconds(-1);
                while (_requestTimes.Count > 0 && _requestTimes.Peek() < cutoff)
                {
                    _requestTimes.Dequeue();
                }
            }
        }
    }

    /// <summary>
    /// Runs your synchronous action on the <see cref="CurlContext"/> before each request, to add headers, change the
    /// URL or options, or stash values in <see cref="CurlContext.Properties"/>.
    /// </summary>
    /// <remarks>
    /// <para>The action runs on the way in, then the rest of the pipeline runs with the modified context. Exceptions from
    /// the action propagate unchanged and the request is not sent. For asynchronous work use a lambda with
    /// <see cref="CurlMiddlewarePipelineBuilder.Use(Func{CurlContext, Func{Task{CurlResult}}, Task{CurlResult}})"/> instead.
    /// Thread-safe if your action is.</para>
    /// <para>Example: <c>builder.Use(new RequestModifierMiddleware(ctx =&gt; ctx.Options.Headers["X-Client"] = "billing-service"));</c>
    /// (with a handler that sends <c>ctx.Options</c>).</para>
    /// </remarks>
    /// <seealso cref="ResponseModifierMiddleware"/>
    /// <seealso cref="CurlContext"/>
    public class RequestModifierMiddleware : ICurlMiddleware
    {
        private readonly Action<CurlContext> _modifier;

        /// <summary>
        /// Creates the middleware with the action that changes each request's <see cref="CurlContext"/> before it runs.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>new RequestModifierMiddleware(ctx =&gt; ctx.Command += " --compressed")</c></para>
        /// </remarks>
        /// <param name="modifier">Changes the context in place; must not be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="modifier"/> is null. Fix: pass an action such as
        /// <c>ctx =&gt; ctx.Command += " -s"</c>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md</exception>
        /// <seealso cref="ResponseModifierMiddleware(Func{CurlResult, Task{CurlResult}})"/>
        public RequestModifierMiddleware(Action<CurlContext> modifier)
        {
            _modifier = modifier ?? throw new ArgumentNullException(nameof(modifier), CurlException.Compose(
                "RequestModifierMiddleware was created with a null modifier, so it has nothing to apply to requests.",
                "Pass an action that changes the context, for example ctx => ctx.Command += \" --compressed\".",
                DocLinks.LoggingGuide));
        }

        /// <summary>
        /// Applies the modifier to <paramref name="context"/> and then runs the rest of the pipeline with it.
        /// </summary>
        /// <remarks>
        /// <para>Example: a modifier <c>ctx =&gt; ctx.Properties["attempted"] = true</c> lets later middleware see the flag.</para>
        /// </remarks>
        /// <param name="context">The request to modify.</param>
        /// <param name="next">Runs the rest of the pipeline.</param>
        /// <returns>The result from <paramref name="next"/>, unchanged.</returns>
        /// <exception cref="Exception">Whatever the modifier or <paramref name="next"/> throws, unchanged.</exception>
        /// <seealso cref="ICurlMiddleware.ExecuteAsync(CurlContext, Func{Task{CurlResult}})"/>
        public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
        {
            _modifier(context);
            return await next().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Runs your asynchronous function on each <see cref="CurlResult"/> after the request, to inspect, change or replace
    /// it (for example to unwrap an envelope, redact a body or record metrics).
    /// </summary>
    /// <remarks>
    /// <para>The function receives the result from the rest of the pipeline and its return value is what the caller gets
    /// (return the same object to pass it through). It does not run when the request throws. Exceptions from the function
    /// propagate unchanged. Thread-safe if your function is.</para>
    /// <para>Example: <c>builder.Use(new ResponseModifierMiddleware(r =&gt; { r.Body = r.Body?.Trim(); return Task.FromResult(r); }));</c></para>
    /// </remarks>
    /// <seealso cref="RequestModifierMiddleware"/>
    /// <seealso cref="CurlResult"/>
    public class ResponseModifierMiddleware : ICurlMiddleware
    {
        private readonly Func<CurlResult, Task<CurlResult>> _modifier;

        /// <summary>
        /// Creates the middleware with the function that turns each request's result into the result returned to the caller.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>new ResponseModifierMiddleware(async r =&gt; { await metrics.RecordAsync(r.StatusCode); return r; })</c></para>
        /// </remarks>
        /// <param name="modifier">Receives the result and returns the result to hand back (the same or a new one); must not be null.</param>
        /// <exception cref="ArgumentNullException"><paramref name="modifier"/> is null. Fix: pass a function such as
        /// <c>r =&gt; Task.FromResult(r)</c>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md</exception>
        /// <seealso cref="RequestModifierMiddleware(Action{CurlContext})"/>
        public ResponseModifierMiddleware(Func<CurlResult, Task<CurlResult>> modifier)
        {
            _modifier = modifier ?? throw new ArgumentNullException(nameof(modifier), CurlException.Compose(
                "ResponseModifierMiddleware was created with a null modifier, so it has nothing to apply to results.",
                "Pass a function that returns the result to hand back, for example r => Task.FromResult(r).",
                DocLinks.LoggingGuide));
        }

        /// <summary>
        /// Runs the rest of the pipeline and returns what the modifier makes of its result.
        /// </summary>
        /// <remarks>
        /// <para>Example: a modifier returning <c>new CurlResult { StatusCode = 200, Body = "{}" }</c> hides every real response
        /// (useful for a feature flag that stubs an API).</para>
        /// </remarks>
        /// <param name="context">The request (not passed to the modifier).</param>
        /// <param name="next">Runs the rest of the pipeline.</param>
        /// <returns>The value returned by the modifier.</returns>
        /// <exception cref="Exception">Whatever <paramref name="next"/> or the modifier throws, unchanged.</exception>
        /// <seealso cref="ICurlMiddleware.ExecuteAsync(CurlContext, Func{Task{CurlResult}})"/>
        public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
        {
            var result = await next().ConfigureAwait(false);
            return await _modifier(result).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Task.Delay for middleware waits: a cancelled token ends the request with exit code 42, like a cancelled transfer.
    /// </summary>
    internal static class MiddlewareDelay
    {
        public static async Task WaitAsync(TimeSpan delay, CurlContext context, string what)
        {
            try
            {
                await Task.Delay(delay, context.CancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException ex)
            {
                throw new CurlAbortedByCallbackException(CurlException.Compose(
                    $"The request to {ErrorText.SafeUrl(context.Options?.Url ?? ErrorText.UrlFromCommand(context.Command))} was cancelled: {what} when CurlContext.CancellationToken was cancelled.",
                    "Cancel only when you want to stop the request; otherwise pass a token that is not cancelled (or a later deadline), or reduce the retry delay / raise the rate limit.",
                    DocLinks.TutorialCancellation), context.Command, ex);
            }
        }
    }
}
