/***************************************************************************
 * Middleware Pipeline for CurlDotNet
 *
 * Manages the chain of middleware components
 *
 * Based on curl's callback system by Daniel Stenberg and contributors
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
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Middleware
{
    /// <summary>
    /// Runs curl requests through an ordered chain of <see cref="ICurlMiddleware"/> (logging, retry, caching,
    /// auth...) and then a final handler that actually executes the request.
    /// </summary>
    /// <remarks>
    /// <para>Order: middleware run in the order they were added. The first one added is the outermost: it runs
    /// first before the request and last after it, so put <see cref="LoggingMiddleware"/> or
    /// <see cref="TimingMiddleware"/> first to measure everything, and <see cref="RetryMiddleware"/> before
    /// <see cref="AuthenticationMiddleware"/> if each retry should fetch a fresh token. When every middleware
    /// has called <c>next()</c>, the final handler passed to the constructor runs.</para>
    /// <para>Failures: an exception from the handler or a middleware propagates out of
    /// <see cref="ExecuteAsync(CurlContext)"/> through every middleware that awaited it (each can catch, retry or
    /// rethrow). Nothing is swallowed by the pipeline itself.</para>
    /// <para>Thread-safety: <see cref="ExecuteAsync(CurlContext)"/> can run many requests at once on the same
    /// pipeline, but <see cref="Use(ICurlMiddleware)"/> and <see cref="Clear"/> must not be called while requests are
    /// running; configure the pipeline once at start-up (for example as a singleton in ASP.NET Core).</para>
    /// <para>Example: <c>var pipeline = CurlMiddlewarePipeline.CreateBuilder().UseLogging(Console.WriteLine).UseRetry(3, TimeSpan.FromMilliseconds(500)).WithHandler(ctx =&gt; Curl.ExecuteAsync(ctx.Command, ctx.CancellationToken)).Build();</c>
    /// then <c>var result = await pipeline.ExecuteAsync(new CurlContext { Command = "curl https://api.example.com/health" });</c>.</para>
    /// <para>Logging and metrics guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md;
    /// retries, timeouts and cancellation: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/advanced.md</para>
    /// </remarks>
    /// <seealso cref="CurlMiddlewarePipelineBuilder"/>
    /// <seealso cref="ICurlMiddleware"/>
    /// <seealso cref="CurlContext"/>
    public class CurlMiddlewarePipeline
    {
        private readonly List<ICurlMiddleware> _middleware = new List<ICurlMiddleware>();
        private readonly Func<CurlContext, Task<CurlResult>> _finalHandler;

        /// <summary>
        /// Creates an empty pipeline whose last step, after all middleware, is <paramref name="finalHandler"/>
        /// (the code that really runs the request).
        /// </summary>
        /// <remarks>
        /// <para>The handler receives the <see cref="CurlContext"/> as changed by the middleware. Typical handlers:
        /// <c>ctx =&gt; Curl.ExecuteAsync(ctx.Command, ctx.CancellationToken)</c> (sends the command text) or
        /// <c>ctx =&gt; Curl.ExecuteAsync(ctx.Options, ctx.CancellationToken)</c> (sends the parsed options, so changes
        /// middleware made to <see cref="CurlContext.Options"/> take effect). In tests, a handler that returns a fixed
        /// <c>new CurlResult { StatusCode = 200, Body = "ok" }</c> lets you exercise middleware without a network.</para>
        /// <para>Example: <c>var pipeline = new CurlMiddlewarePipeline(ctx =&gt; Curl.ExecuteAsync(ctx.Command)).Use(new TimingMiddleware());</c></para>
        /// </remarks>
        /// <param name="finalHandler">Runs the request and returns its result; must not be null. It may throw
        /// <see cref="CurlException"/> subtypes, which flow back through the middleware.</param>
        /// <exception cref="ArgumentNullException"><paramref name="finalHandler"/> is null. Fix: pass a handler such as
        /// <c>ctx =&gt; Curl.ExecuteAsync(ctx.Command)</c>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md</exception>
        /// <seealso cref="CreateBuilder"/>
        /// <seealso cref="CurlMiddlewarePipelineBuilder.WithHandler(Func{CurlContext, Task{CurlResult}})"/>
        public CurlMiddlewarePipeline(Func<CurlContext, Task<CurlResult>> finalHandler)
        {
            _finalHandler = finalHandler ?? throw new ArgumentNullException(nameof(finalHandler), CurlException.Compose(
                "CurlMiddlewarePipeline was created with a null final handler, so there is nothing to run the request after the middleware.",
                "Pass the code that executes the request, for example new CurlMiddlewarePipeline(ctx => Curl.ExecuteAsync(ctx.Command, ctx.CancellationToken)).",
                DocLinks.LoggingGuide));
        }

        /// <summary>
        /// Appends <paramref name="middleware"/> to the end of the chain (it runs after everything added before it)
        /// and returns this pipeline for chaining.
        /// </summary>
        /// <remarks>
        /// <para>The same instance is used for every request, possibly concurrently, so it must be thread-safe.
        /// Adding the same instance twice makes it run twice per request. Do not call this while
        /// <see cref="ExecuteAsync(CurlContext)"/> is running on another thread.</para>
        /// <para>Example: <c>pipeline.Use(new LoggingMiddleware(msg =&gt; logger.LogInformation(msg))).Use(new RetryMiddleware(2));</c></para>
        /// </remarks>
        /// <param name="middleware">The middleware to add; must not be null.</param>
        /// <returns>This pipeline.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="middleware"/> is null. Fix: pass an
        /// <see cref="ICurlMiddleware"/> instance. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md</exception>
        /// <seealso cref="Use(Func{CurlContext, Func{Task{CurlResult}}, Task{CurlResult}})"/>
        /// <seealso cref="CurlMiddlewarePipelineBuilder.Use(ICurlMiddleware)"/>
        public CurlMiddlewarePipeline Use(ICurlMiddleware middleware)
        {
            _middleware.Add(middleware ?? throw NullMiddleware(nameof(middleware), "CurlMiddlewarePipeline.Use"));
            return this;
        }

        /// <summary>
        /// Appends an inline middleware written as a lambda <c>(context, next) =&gt; ...</c> and returns this pipeline
        /// for chaining; handy for one-off steps that do not deserve a class.
        /// </summary>
        /// <remarks>
        /// <para>The lambda follows the same rules as <see cref="ICurlMiddleware.ExecuteAsync(CurlContext, Func{Task{CurlResult}})"/>:
        /// call <c>next()</c> to continue, skip it to short-circuit.</para>
        /// <para>Example: <c>pipeline.Use(async (ctx, next) =&gt; { ctx.Command += " -H 'X-Trace: 1'"; return await next(); });</c></para>
        /// </remarks>
        /// <param name="middleware">The step to run; must not be null.</param>
        /// <returns>This pipeline.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="middleware"/> is null. Fix: pass a lambda such as
        /// <c>(ctx, next) =&gt; next()</c>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md</exception>
        /// <seealso cref="Use(ICurlMiddleware)"/>
        public CurlMiddlewarePipeline Use(Func<CurlContext, Func<Task<CurlResult>>, Task<CurlResult>> middleware)
        {
            _middleware.Add(new DelegateMiddleware(middleware ?? throw NullMiddleware(nameof(middleware), "CurlMiddlewarePipeline.Use")));
            return this;
        }

        /// <summary>
        /// Runs <paramref name="context"/> through every middleware in order and then the final handler, and returns
        /// the result as it comes back out of the first middleware.
        /// </summary>
        /// <remarks>
        /// <para>The chain is assembled from the middleware registered at the moment of the call. Each request should
        /// get its own <see cref="CurlContext"/>. Cancellation is whatever <see cref="CurlContext.CancellationToken"/>
        /// says; pass it on in your handler.</para>
        /// <para>Example: <c>var result = await pipeline.ExecuteAsync(new CurlContext { Command = "curl https://api.example.com/users", CancellationToken = ct });</c>
        /// then <c>Console.WriteLine(result.StatusCode);</c>.</para>
        /// </remarks>
        /// <param name="context">The request to run; must not be null.</param>
        /// <returns>The <see cref="CurlResult"/> produced by the handler, possibly replaced or modified by middleware
        /// (for example a cached copy from <see cref="CachingMiddleware"/>).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is null. Fix: pass
        /// <c>new CurlContext { Command = "curl https://..." }</c>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md</exception>
        /// <exception cref="CurlException">Any curl failure from the handler that no middleware handled, for example
        /// <see cref="CurlCouldntResolveHostException"/> or <see cref="CurlRetryException"/> from <see cref="RetryMiddleware"/>;
        /// each message says what failed and how to fix it. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="ICurlMiddleware"/>
        /// <seealso cref="Curl.ExecuteAsync(string)"/>
        public async Task<CurlResult> ExecuteAsync(CurlContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), CurlException.Compose(
                    "CurlMiddlewarePipeline.ExecuteAsync was called with a null CurlContext, so there is no request to run.",
                    "Pass a context describing the request, for example new CurlContext { Command = \"curl https://api.example.com\", CancellationToken = token }.",
                    DocLinks.LoggingGuide));
            }

            // Build the pipeline from the end
            Func<Task<CurlResult>> pipeline = () => _finalHandler(context);

            // Wrap each middleware in reverse order
            for (int i = _middleware.Count - 1; i >= 0; i--)
            {
                var middleware = _middleware[i];
                var next = pipeline;
                pipeline = () => middleware.ExecuteAsync(context, next);
            }

            return await pipeline();
        }

        /// <summary>
        /// Removes every middleware, leaving only the final handler, so <see cref="ExecuteAsync(CurlContext)"/> then runs
        /// the request directly.
        /// </summary>
        /// <remarks>
        /// <para>Useful in tests or to reconfigure at start-up. Not safe to call while requests are running. It does not
        /// clear the shared cache of <see cref="CachingMiddleware"/>; use <see cref="CachingMiddleware.ClearCache"/> for that.</para>
        /// <para>Example: <c>pipeline.Clear(); pipeline.Use(new TimingMiddleware());</c></para>
        /// </remarks>
        /// <seealso cref="Count"/>
        /// <seealso cref="Use(ICurlMiddleware)"/>
        public void Clear()
        {
            _middleware.Clear();
        }

        /// <summary>
        /// How many middleware are registered (the final handler is not counted); 0 means requests go straight to
        /// the handler.
        /// </summary>
        /// <value>The number of <see cref="Use(ICurlMiddleware)"/> registrations, including duplicates.</value>
        /// <remarks>
        /// <para>Example: <c>Debug.Assert(pipeline.Count == 3);</c> after building with logging, retry and timing.</para>
        /// </remarks>
        /// <seealso cref="Clear"/>
        public int Count => _middleware.Count;

        /// <summary>
        /// Starts a fluent <see cref="CurlMiddlewarePipelineBuilder"/>, the shortest way to assemble a pipeline with the
        /// built-in middleware: <c>CreateBuilder().UseLogging().UseRetry().WithHandler(...).Build()</c>.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>var pipeline = CurlMiddlewarePipeline.CreateBuilder().UseTiming().UseCaching(TimeSpan.FromMinutes(1)).WithHandler(ctx =&gt; Curl.ExecuteAsync(ctx.Command)).Build();</c></para>
        /// </remarks>
        /// <returns>A new, empty builder.</returns>
        /// <seealso cref="CurlMiddlewarePipelineBuilder"/>
        /// <seealso cref="CurlMiddlewarePipelineBuilder.Build"/>
        public static CurlMiddlewarePipelineBuilder CreateBuilder()
        {
            return new CurlMiddlewarePipelineBuilder();
        }

        internal static ArgumentNullException NullMiddleware(string parameter, string method)
        {
            return new ArgumentNullException(parameter, CurlException.Compose(
                $"{method} was given a null middleware, which would fail with a NullReferenceException on the first request.",
                "Pass an ICurlMiddleware instance (for example new LoggingMiddleware()) or a lambda such as (ctx, next) => next().",
                DocLinks.LoggingGuide));
        }

        /// <summary>
        /// Delegate-based middleware implementation.
        /// </summary>
        private class DelegateMiddleware : ICurlMiddleware
        {
            private readonly Func<CurlContext, Func<Task<CurlResult>>, Task<CurlResult>> _middleware;

            public DelegateMiddleware(Func<CurlContext, Func<Task<CurlResult>>, Task<CurlResult>> middleware)
            {
                _middleware = middleware;
            }

            public Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
            {
                return _middleware(context, next);
            }
        }
    }

    /// <summary>
    /// Fluent builder that collects middleware (built-in or your own) and a final handler, then produces a
    /// <see cref="CurlMiddlewarePipeline"/> with <see cref="Build"/>.
    /// </summary>
    /// <remarks>
    /// <para>Middleware run in the order of the <c>Use...</c> calls: the first call is the outermost layer.
    /// <see cref="WithHandler(Func{CurlContext, Task{CurlResult}})"/> is required; <see cref="Build"/> throws without it.
    /// The builder is not thread-safe; build once and share the resulting pipeline.</para>
    /// <para>Example: <c>var pipeline = CurlMiddlewarePipeline.CreateBuilder().UseLogging(msg =&gt; logger.LogDebug(msg)).UseRetry(maxRetries: 3, delay: TimeSpan.FromSeconds(1)).UseRateLimit(10).WithHandler(ctx =&gt; Curl.ExecuteAsync(ctx.Command, ctx.CancellationToken)).Build();</c></para>
    /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md</para>
    /// </remarks>
    /// <seealso cref="CurlMiddlewarePipeline"/>
    /// <seealso cref="CurlMiddlewarePipeline.CreateBuilder"/>
    public class CurlMiddlewarePipelineBuilder
    {
        private readonly List<ICurlMiddleware> _middleware = new List<ICurlMiddleware>();
        private Func<CurlContext, Task<CurlResult>> _finalHandler;

        /// <summary>
        /// Adds your own <see cref="ICurlMiddleware"/> at this point of the chain and returns the builder for chaining.
        /// </summary>
        /// <remarks>
        /// <para>The instance is shared by every request the built pipeline runs, so it must be thread-safe.</para>
        /// <para>Example: <c>builder.Use(new SerilogMiddleware(log));</c> (see the logging guide for that class).</para>
        /// </remarks>
        /// <param name="middleware">The middleware to add; must not be null.</param>
        /// <returns>This builder.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="middleware"/> is null. Fix: pass an instance.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md</exception>
        /// <seealso cref="CurlMiddlewarePipeline.Use(ICurlMiddleware)"/>
        /// <seealso cref="Use(Func{CurlContext, Func{Task{CurlResult}}, Task{CurlResult}})"/>
        public CurlMiddlewarePipelineBuilder Use(ICurlMiddleware middleware)
        {
            _middleware.Add(middleware ?? throw CurlMiddlewarePipeline.NullMiddleware(nameof(middleware), "CurlMiddlewarePipelineBuilder.Use"));
            return this;
        }

        /// <summary>
        /// Adds an inline middleware written as a lambda <c>(context, next) =&gt; ...</c> and returns the builder for chaining.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>builder.Use(async (ctx, next) =&gt; { var r = await next(); if (r.StatusCode == 401) await RefreshTokenAsync(); return r; });</c></para>
        /// </remarks>
        /// <param name="middleware">The step to run; must not be null. Call <c>next()</c> to continue the chain.</param>
        /// <returns>This builder.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="middleware"/> is null. Fix: pass a lambda.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md</exception>
        /// <seealso cref="Use(ICurlMiddleware)"/>
        public CurlMiddlewarePipelineBuilder Use(Func<CurlContext, Func<Task<CurlResult>>, Task<CurlResult>> middleware)
        {
            _middleware.Add(new DelegateMiddleware(middleware ?? throw CurlMiddlewarePipeline.NullMiddleware(nameof(middleware), "CurlMiddlewarePipelineBuilder.Use")));
            return this;
        }

        /// <summary>
        /// Adds a <see cref="LoggingMiddleware"/> that writes one line before each request (the command) and one after
        /// (status and milliseconds, or the error message).
        /// </summary>
        /// <remarks>
        /// <para>Lines look like <c>[CURL] Executing: curl https://...</c>, <c>[CURL] Success: Status=200, Time=123.4ms</c>
        /// and <c>[CURL] Failed: &lt;message&gt;, Time=...ms</c>. The command text can contain credentials; prefer your own
        /// middleware that logs <c>context.Options.Url</c> in production.</para>
        /// <para>Example: <c>builder.UseLogging(msg =&gt; logger.LogInformation(msg));</c></para>
        /// </remarks>
        /// <param name="logger">Receives each log line; null (the default) writes to <see cref="Console.WriteLine(string)"/>.</param>
        /// <returns>This builder.</returns>
        /// <seealso cref="LoggingMiddleware"/>
        public CurlMiddlewarePipelineBuilder UseLogging(Action<string> logger = null)
        {
            Use(new LoggingMiddleware(logger));
            return this;
        }

        /// <summary>
        /// Adds a <see cref="RetryMiddleware"/> that re-runs the rest of the pipeline on 5xx/429 responses and on
        /// transient curl errors, waiting <paramref name="delay"/>, then twice as long, and so on.
        /// </summary>
        /// <remarks>
        /// <para>With the defaults a failing request is tried 4 times (1 + 3 retries) with waits of 1 s, 2 s and 4 s.
        /// Retried exceptions are those whose <see cref="CurlException.IsRetryable"/> is true (DNS failure 6, connect
        /// failure 7, timeout 28, empty reply 52, send/receive errors 55/56, HTTP 429/5xx); others are rethrown at once.
        /// When retries run out on an exception, <see cref="CurlRetryException"/> is thrown; when they run out on a 5xx/429
        /// response, that last response is returned.</para>
        /// <para>Example: <c>builder.UseRetry(maxRetries: 5, delay: TimeSpan.FromMilliseconds(200));</c> waits 0.2, 0.4, 0.8, 1.6 and 3.2 seconds.</para>
        /// </remarks>
        /// <param name="maxRetries">Retries after the first attempt; 0 disables retrying. Default 3.</param>
        /// <param name="delay">Wait before the first retry, doubled for each further retry; null means 1 second. Must not be negative.</param>
        /// <returns>This builder.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="delay"/> is negative. Fix: pass
        /// <see cref="TimeSpan.Zero"/> or a positive delay. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/advanced.md</exception>
        /// <seealso cref="RetryMiddleware"/>
        public CurlMiddlewarePipelineBuilder UseRetry(int maxRetries = 3, TimeSpan? delay = null)
        {
            Use(new RetryMiddleware(maxRetries, delay));
            return this;
        }

        /// <summary>
        /// Adds a <see cref="TimingMiddleware"/> that sets <see cref="CurlTimings.Total"/> on the result to the time the
        /// rest of the pipeline took, and records start, end and duration in <see cref="CurlContext.Properties"/>.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>builder.UseTiming();</c> then <c>var took = context.GetProperty&lt;TimeSpan&gt;("TimingDuration");</c>.</para>
        /// </remarks>
        /// <returns>This builder.</returns>
        /// <seealso cref="TimingMiddleware"/>
        public CurlMiddlewarePipelineBuilder UseTiming()
        {
            Use(new TimingMiddleware());
            return this;
        }

        /// <summary>
        /// Adds a <see cref="CachingMiddleware"/> that answers repeated successful GET requests from memory for
        /// <paramref name="ttl"/> instead of sending them again.
        /// </summary>
        /// <remarks>
        /// <para>The cache is shared by every <see cref="CachingMiddleware"/> in the process and keyed on
        /// <see cref="CurlContext.Command"/> (or the URL when there is no command). Only 2xx results are stored.</para>
        /// <para>Example: <c>builder.UseCaching(TimeSpan.FromSeconds(30));</c></para>
        /// </remarks>
        /// <param name="ttl">How long a stored result is reused; null means 5 minutes.</param>
        /// <returns>This builder.</returns>
        /// <seealso cref="CachingMiddleware"/>
        /// <seealso cref="CachingMiddleware.ClearCache"/>
        public CurlMiddlewarePipelineBuilder UseCaching(TimeSpan? ttl = null)
        {
            Use(new CachingMiddleware(ttl));
            return this;
        }

        /// <summary>
        /// Adds an <see cref="AuthenticationMiddleware"/> that asks <paramref name="tokenProvider"/> for a token before
        /// every request and sends it as <c>Authorization: Bearer &lt;token&gt;</c>.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>builder.UseAuthentication(async ctx =&gt; await tokenCache.GetAccessTokenAsync());</c>.
        /// Bearer token tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-bearer-token-csharp/README.md</para>
        /// </remarks>
        /// <param name="tokenProvider">Returns the token for the request (null or empty sends no header); must not be null.</param>
        /// <returns>This builder.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="tokenProvider"/> is null. Fix: pass a function that
        /// returns the token. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-bearer-token-csharp/README.md</exception>
        /// <seealso cref="AuthenticationMiddleware"/>
        public CurlMiddlewarePipelineBuilder UseAuthentication(Func<CurlContext, Task<string>> tokenProvider)
        {
            Use(new AuthenticationMiddleware(tokenProvider));
            return this;
        }

        /// <summary>
        /// Adds a <see cref="RateLimitMiddleware"/> that delays requests so that no more than
        /// <paramref name="requestsPerSecond"/> complete in any one-second window.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>builder.UseRateLimit(5);</c> keeps a loop of calls under an API's 5-requests-per-second limit.</para>
        /// </remarks>
        /// <param name="requestsPerSecond">Requests allowed per second; 1 or more.</param>
        /// <returns>This builder.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="requestsPerSecond"/> is 0 or negative. Fix: pass 1
        /// or more. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/advanced.md</exception>
        /// <seealso cref="RateLimitMiddleware"/>
        public CurlMiddlewarePipelineBuilder UseRateLimit(int requestsPerSecond)
        {
            Use(new RateLimitMiddleware(requestsPerSecond));
            return this;
        }

        /// <summary>
        /// Sets the final handler that runs the request after all middleware (required before <see cref="Build"/>);
        /// calling it again replaces the previous handler.
        /// </summary>
        /// <remarks>
        /// <para>Use <c>ctx =&gt; Curl.ExecuteAsync(ctx.Command, ctx.CancellationToken)</c> to send the command text, or
        /// <c>ctx =&gt; Curl.ExecuteAsync(ctx.Options, ctx.CancellationToken)</c> when middleware change
        /// <see cref="CurlContext.Options"/>. In unit tests return a canned <see cref="CurlResult"/>.</para>
        /// <para>Example: <c>builder.WithHandler(ctx =&gt; Task.FromResult(new CurlResult { StatusCode = 200, Body = "{}" }));</c></para>
        /// </remarks>
        /// <param name="handler">Runs the request; null is accepted here but makes <see cref="Build"/> throw.</param>
        /// <returns>This builder.</returns>
        /// <seealso cref="Build"/>
        /// <seealso cref="CurlMiddlewarePipeline(Func{CurlContext, Task{CurlResult}})"/>
        public CurlMiddlewarePipelineBuilder WithHandler(Func<CurlContext, Task<CurlResult>> handler)
        {
            _finalHandler = handler;
            return this;
        }

        /// <summary>
        /// Creates the <see cref="CurlMiddlewarePipeline"/> with the middleware added so far, in the same order, and the
        /// handler set with <see cref="WithHandler(Func{CurlContext, Task{CurlResult}})"/>.
        /// </summary>
        /// <remarks>
        /// <para>The pipeline gets its own copy of the middleware list, so later <c>Use</c> calls on the builder do not
        /// change pipelines that were already built (the middleware instances themselves are shared).</para>
        /// <para>Example: <c>var pipeline = builder.WithHandler(ctx =&gt; Curl.ExecuteAsync(ctx.Command)).Build();</c></para>
        /// </remarks>
        /// <returns>A ready-to-use pipeline.</returns>
        /// <exception cref="InvalidOperationException">No handler was set (or it was set to null). Fix: call
        /// <c>WithHandler(ctx =&gt; Curl.ExecuteAsync(ctx.Command))</c> before <c>Build()</c>.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md</exception>
        /// <seealso cref="WithHandler(Func{CurlContext, Task{CurlResult}})"/>
        /// <seealso cref="CurlMiddlewarePipeline"/>
        public CurlMiddlewarePipeline Build()
        {
            if (_finalHandler == null)
            {
                throw new InvalidOperationException(CurlException.Compose(
                    $"CurlMiddlewarePipelineBuilder.Build() was called without a final handler ({_middleware.Count} middleware registered, nothing to run the request).",
                    "Call WithHandler(ctx => Curl.ExecuteAsync(ctx.Command, ctx.CancellationToken)) before Build().",
                    DocLinks.LoggingGuide));
            }

            var pipeline = new CurlMiddlewarePipeline(_finalHandler);
            foreach (var middleware in _middleware)
            {
                pipeline.Use(middleware);
            }

            return pipeline;
        }

        /// <summary>
        /// Delegate-based middleware implementation.
        /// </summary>
        private class DelegateMiddleware : ICurlMiddleware
        {
            private readonly Func<CurlContext, Func<Task<CurlResult>>, Task<CurlResult>> _middleware;

            public DelegateMiddleware(Func<CurlContext, Func<Task<CurlResult>>, Task<CurlResult>> middleware)
            {
                _middleware = middleware;
            }

            public Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
            {
                return _middleware(context, next);
            }
        }
    }
}
