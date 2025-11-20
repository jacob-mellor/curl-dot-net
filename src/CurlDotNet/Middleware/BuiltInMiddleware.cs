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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Middleware
{
    /// <summary>
    /// Middleware for logging curl operations.
    /// </summary>
    public class LoggingMiddleware : ICurlMiddleware
    {
        private readonly Action<string> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingMiddleware"/> class.
        /// </summary>
        /// <param name="logger">Optional custom logger. If null, uses Console.WriteLine.</param>
        public LoggingMiddleware(Action<string> logger = null)
        {
            _logger = logger ?? Console.WriteLine;
        }

        /// <summary>
        /// Executes the middleware, logging the request and response.
        /// </summary>
        /// <param name="context">The curl execution context.</param>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <returns>The curl result.</returns>
        public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
        {
            _logger($"[CURL] Executing: {context.Command}");
            var startTime = DateTime.UtcNow;

            try
            {
                var result = await next();
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
    /// Middleware for retry logic with exponential backoff.
    /// </summary>
    public class RetryMiddleware : ICurlMiddleware
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _initialDelay;

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryMiddleware"/> class.
        /// </summary>
        /// <param name="maxRetries">Maximum number of retry attempts. Defaults to 3.</param>
        /// <param name="initialDelay">Initial delay before the first retry. Defaults to 1 second.</param>
        public RetryMiddleware(int maxRetries = 3, TimeSpan? initialDelay = null)
        {
            _maxRetries = maxRetries;
            _initialDelay = initialDelay ?? TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Executes the middleware with retry logic and exponential backoff.
        /// </summary>
        /// <param name="context">The curl execution context.</param>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <returns>The curl result.</returns>
        public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
        {
            Exception lastException = null;

            for (int attempt = 0; attempt <= _maxRetries; attempt++)
            {
                try
                {
                    var result = await next();

                    // Check if HTTP error is retryable
                    if (result.StatusCode >= 500 || result.StatusCode == 429)
                    {
                        if (attempt < _maxRetries)
                        {
                            var delay = TimeSpan.FromMilliseconds(_initialDelay.TotalMilliseconds * Math.Pow(2, attempt));
                            await Task.Delay(delay, context.CancellationToken);
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
                        await Task.Delay(delay, context.CancellationToken);
                        continue;
                    }
                    
                    // If we are out of retries, we swallow the exception here
                    // The loop will finish and throw CurlRetryException
                }
            }

            throw new CurlRetryException($"Failed after {_maxRetries} retries", context.Command, _maxRetries, lastException);
        }
    }

    /// <summary>
    /// Middleware for timing curl operations.
    /// </summary>
    public class TimingMiddleware : ICurlMiddleware
    {
        /// <summary>
        /// Executes the middleware, tracking the execution time.
        /// </summary>
        /// <param name="context">The curl execution context.</param>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <returns>The curl result with timing information.</returns>
        public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
        {
            var startTime = DateTime.UtcNow;
            context.Properties["TimingStart"] = startTime;

            var result = await next();

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
    /// Simple in-memory caching middleware.
    /// </summary>
    public class CachingMiddleware : ICurlMiddleware
    {
        private static readonly ConcurrentDictionary<string, CacheEntry> _cache = new ConcurrentDictionary<string, CacheEntry>();
        private readonly TimeSpan _ttl;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingMiddleware"/> class.
        /// </summary>
        /// <param name="ttl">Time-to-live for cached entries. Defaults to 5 minutes.</param>
        public CachingMiddleware(TimeSpan? ttl = null)
        {
            _ttl = ttl ?? TimeSpan.FromMinutes(5);
        }

        /// <summary>
        /// Executes the middleware, caching GET request results.
        /// </summary>
        /// <param name="context">The curl execution context.</param>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <returns>The curl result, either from cache or fresh execution.</returns>
        public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
        {
            // Only cache GET requests
            if (context.Options?.Method != null && context.Options.Method != "GET")
            {
                return await next();
            }

            var cacheKey = GetCacheKey(context);

            // Check cache
            if (_cache.TryGetValue(cacheKey, out var entry) && entry.Expiry > DateTime.UtcNow)
            {
                // Clone the cached result
                return CloneResult(entry.Result);
            }

            // Execute and cache
            var result = await next();

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
        /// Clear the cache.
        /// </summary>
        public static void ClearCache()
        {
            _cache.Clear();
        }
    }

    /// <summary>
    /// Middleware for adding authentication headers.
    /// </summary>
    public class AuthenticationMiddleware : ICurlMiddleware
    {
        private readonly Func<CurlContext, Task<string>> _tokenProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationMiddleware"/> class.
        /// </summary>
        /// <param name="tokenProvider">A function that provides authentication tokens.</param>
        public AuthenticationMiddleware(Func<CurlContext, Task<string>> tokenProvider)
        {
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        }

        /// <summary>
        /// Executes the middleware, adding authentication headers to the request.
        /// </summary>
        /// <param name="context">The curl execution context.</param>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <returns>The curl result.</returns>
        public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
        {
            // Get token
            var token = await _tokenProvider(context);

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

                // Update command if needed
                if (!context.Command.Contains("Authorization"))
                {
                    context.Command = context.Command + $" -H 'Authorization: Bearer {token}'";
                }
            }

            return await next();
        }
    }

    /// <summary>
    /// Simple rate limiting middleware.
    /// </summary>
    public class RateLimitMiddleware : ICurlMiddleware
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly int _requestsPerSecond;
        private readonly Queue<DateTime> _requestTimes = new Queue<DateTime>();
        private readonly object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimitMiddleware"/> class.
        /// </summary>
        /// <param name="requestsPerSecond">Maximum number of requests allowed per second.</param>
        public RateLimitMiddleware(int requestsPerSecond)
        {
            _requestsPerSecond = requestsPerSecond;
            _semaphore = new SemaphoreSlim(requestsPerSecond, requestsPerSecond);
        }

        /// <summary>
        /// Executes the middleware, enforcing rate limits on requests.
        /// </summary>
        /// <param name="context">The curl execution context.</param>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <returns>The curl result.</returns>
        public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
        {
            await WaitIfNeeded(context.CancellationToken);

            try
            {
                return await next();
            }
            finally
            {
                RecordRequest();
            }
        }

        private async Task WaitIfNeeded(CancellationToken cancellationToken)
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
                await Task.Delay(waitTime, cancellationToken);
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
    /// Middleware for modifying requests.
    /// </summary>
    public class RequestModifierMiddleware : ICurlMiddleware
    {
        private readonly Action<CurlContext> _modifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestModifierMiddleware"/> class.
        /// </summary>
        /// <param name="modifier">An action that modifies the request context before execution.</param>
        public RequestModifierMiddleware(Action<CurlContext> modifier)
        {
            _modifier = modifier ?? throw new ArgumentNullException(nameof(modifier));
        }

        /// <summary>
        /// Executes the middleware, modifying the request before passing it to the next middleware.
        /// </summary>
        /// <param name="context">The curl execution context.</param>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <returns>The curl result.</returns>
        public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
        {
            _modifier(context);
            return await next();
        }
    }

    /// <summary>
    /// Middleware for modifying responses.
    /// </summary>
    public class ResponseModifierMiddleware : ICurlMiddleware
    {
        private readonly Func<CurlResult, Task<CurlResult>> _modifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseModifierMiddleware"/> class.
        /// </summary>
        /// <param name="modifier">A function that modifies the response after execution.</param>
        public ResponseModifierMiddleware(Func<CurlResult, Task<CurlResult>> modifier)
        {
            _modifier = modifier ?? throw new ArgumentNullException(nameof(modifier));
        }

        /// <summary>
        /// Executes the middleware, modifying the response after the next middleware completes.
        /// </summary>
        /// <param name="context">The curl execution context.</param>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <returns>The modified curl result.</returns>
        public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
        {
            var result = await next();
            return await _modifier(result);
        }
    }
}