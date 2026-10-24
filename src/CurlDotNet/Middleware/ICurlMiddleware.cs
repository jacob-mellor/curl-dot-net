/***************************************************************************
 * Middleware/Interceptor Pattern for CurlDotNet
 *
 * Allows intercepting and modifying requests and responses
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
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Middleware
{
    /// <summary>
    /// A step that wraps every curl request in a <see cref="CurlMiddlewarePipeline"/>: it can inspect or
    /// change the request before it runs, decide whether it runs at all, and inspect or replace the result.
    /// </summary>
    /// <remarks>
    /// <para>Implement this interface for cross-cutting concerns such as logging, metrics, retries, caching,
    /// authentication headers or request signing, then register it with
    /// <see cref="CurlMiddlewarePipelineBuilder.Use(ICurlMiddleware)"/> or <see cref="CurlMiddlewarePipeline.Use(ICurlMiddleware)"/>.
    /// The library ships ready-made ones: <see cref="LoggingMiddleware"/>, <see cref="RetryMiddleware"/>,
    /// <see cref="TimingMiddleware"/>, <see cref="CachingMiddleware"/>, <see cref="AuthenticationMiddleware"/>,
    /// <see cref="RateLimitMiddleware"/>, <see cref="RequestModifierMiddleware"/> and <see cref="ResponseModifierMiddleware"/>.</para>
    /// <para>How a call flows: the first middleware registered runs first. Its <c>next</c> delegate runs the
    /// second middleware, and so on; the last <c>next</c> runs the pipeline's final handler (usually
    /// <c>ctx =&gt; Curl.ExecuteAsync(ctx.Command)</c>). Code before <c>await next()</c> runs on the way in,
    /// code after it on the way out, in reverse order. Not calling <c>next()</c> short-circuits the request
    /// (a cache hit, for example); calling it more than once runs the rest of the pipeline again (a retry).
    /// An exception thrown by a middleware, by the handler or by <c>next()</c> travels back out through
    /// every middleware that awaited it, so a <c>try/catch</c> around <c>await next()</c> sees failures
    /// from everything registered after it.</para>
    /// <para>Thread-safety: a pipeline reuses the same middleware instance for every request and may run
    /// several requests at once, so keep per-request state in <see cref="CurlContext.Properties"/> or local
    /// variables, not in fields (or protect the fields with a lock).</para>
    /// <para>Example: a class with <c>public async Task&lt;CurlResult&gt; ExecuteAsync(CurlContext context, Func&lt;Task&lt;CurlResult&gt;&gt; next)</c>
    /// whose body is <c>var sw = Stopwatch.StartNew(); var result = await next(); log.LogInformation("{Url} {Status} {Ms}ms", context.Options?.Url, result.StatusCode, sw.ElapsedMilliseconds); return result;</c>
    /// logs every request; register it with <c>CurlMiddlewarePipeline.CreateBuilder().Use(new MyLoggingMiddleware(log)).WithHandler(ctx =&gt; Curl.ExecuteAsync(ctx.Command)).Build()</c>.</para>
    /// <para>Guide with a Serilog / Microsoft.Extensions.Logging middleware:
    /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class LoggingMiddleware : ICurlMiddleware
    /// {
    ///     public async Task&lt;CurlResult&gt; ExecuteAsync(CurlContext context, Func&lt;Task&lt;CurlResult&gt;&gt; next)
    ///     {
    ///         Console.WriteLine($"Executing: {context.Command}");
    ///         var result = await next();
    ///         Console.WriteLine($"Status: {result.StatusCode}");
    ///         return result;
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="CurlMiddlewarePipeline"/>
    /// <seealso cref="CurlMiddlewarePipelineBuilder"/>
    /// <seealso cref="CurlContext"/>
    public interface ICurlMiddleware
    {
        /// <summary>
        /// Runs this middleware for one request: do work before and/or after <c>await next()</c>, and return
        /// the result (the one <paramref name="next"/> produced, a changed one, or your own).
        /// </summary>
        /// <remarks>
        /// <para>Call <paramref name="next"/> at most once for a normal pass-through, more than once to retry,
        /// or not at all to short-circuit (then return your own <see cref="CurlResult"/>, for example a cached
        /// one). Changes you make to <see cref="CurlContext.Command"/> or <see cref="CurlContext.Options"/> before
        /// calling <paramref name="next"/> are seen by the later middleware and by the final handler; whether
        /// they reach the wire depends on the handler (<c>ctx =&gt; Curl.ExecuteAsync(ctx.Command)</c> sends the
        /// command text, <c>ctx =&gt; Curl.ExecuteAsync(ctx.Options)</c> sends the options).</para>
        /// <para>Exceptions: whatever you throw reaches the caller of <see cref="CurlMiddlewarePipeline.ExecuteAsync(CurlContext)"/>
        /// through the middleware registered before you. Failed transfers arrive from <paramref name="next"/> as
        /// <see cref="CurlDotNet.Exceptions.CurlException"/> subtypes; rethrow with <c>throw;</c> to keep the
        /// stack trace. Honour <see cref="CurlContext.CancellationToken"/> in any waiting you do.</para>
        /// <para>Example: <c>public async Task&lt;CurlResult&gt; ExecuteAsync(CurlContext ctx, Func&lt;Task&lt;CurlResult&gt;&gt; next) { ctx.Options.Headers["X-Request-Id"] = Guid.NewGuid().ToString(); return await next(); }</c>
        /// adds a header to every request whose handler sends <c>ctx.Options</c>.</para>
        /// </remarks>
        /// <param name="context">The request being run: <see cref="CurlContext.Command"/> (curl command text),
        /// <see cref="CurlContext.Options"/> (parsed options, may be null if the caller only set a command),
        /// <see cref="CurlContext.Properties"/> (a bag for passing data between middleware) and the
        /// <see cref="CurlContext.CancellationToken"/>. Never null when called by the pipeline.</param>
        /// <param name="next">Runs the rest of the pipeline (the following middleware, then the final handler)
        /// and returns its result; it throws whatever they throw.</param>
        /// <returns>The result to hand back to the previous middleware (or to the caller). Returning null is
        /// allowed by the pipeline but most callers expect a result.</returns>
        /// <seealso cref="CurlMiddlewarePipeline.ExecuteAsync(CurlContext)"/>
        /// <seealso cref="CurlContext"/>
        Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next);
    }

    /// <summary>
    /// Everything middleware needs to know about one request: the curl command, the parsed options, a
    /// property bag shared by all middleware, the cancellation token and the start time.
    /// </summary>
    /// <remarks>
    /// <para>Create one per request and pass it to <see cref="CurlMiddlewarePipeline.ExecuteAsync(CurlContext)"/>.
    /// Middleware may read and change it on the way in; the final handler decides whether it sends
    /// <see cref="Command"/> or <see cref="Options"/>. A context is not thread-safe and should not be shared
    /// between concurrent requests.</para>
    /// <para>Example: <c>var ctx = new CurlContext { Command = "curl https://api.example.com/users", Options = new CurlOptions { Url = "https://api.example.com/users" }, CancellationToken = ct };</c>
    /// then <c>var result = await pipeline.ExecuteAsync(ctx);</c>.</para>
    /// </remarks>
    /// <seealso cref="ICurlMiddleware"/>
    /// <seealso cref="CurlMiddlewarePipeline"/>
    public class CurlContext
    {
        /// <summary>
        /// The curl command text for this request, such as <c>curl -H 'Accept: application/json' https://api.example.com/users</c>,
        /// usually what the final handler passes to <see cref="Curl.ExecuteAsync(string)"/>.
        /// </summary>
        /// <value>Any string the handler understands; null when the request is described only by <see cref="Options"/>.
        /// Default null.</value>
        /// <remarks>
        /// <para><see cref="LoggingMiddleware"/> logs it with credentials masked (<c>-u user:***</c>, <c>Authorization: ***</c>,
        /// URL passwords), <see cref="CachingMiddleware"/> uses it as the cache key and <see cref="AuthenticationMiddleware"/>
        /// appends <c>-H 'Authorization: Bearer ...'</c> to it. The text itself still contains those credentials, so do not
        /// log it yourself without masking.</para>
        /// <para>Example: <c>context.Command += " --compressed";</c> in a middleware makes the handler request gzip.</para>
        /// </remarks>
        /// <seealso cref="Options"/>
        public string Command { get; set; }

        /// <summary>
        /// The parsed request options (URL, method, headers, timeouts...) that middleware can read or change,
        /// and that a handler such as <c>ctx =&gt; Curl.ExecuteAsync(ctx.Options)</c> sends.
        /// </summary>
        /// <value>A <see cref="CurlOptions"/>, or null when the caller only set <see cref="Command"/>. Default null.</value>
        /// <remarks>
        /// <para>Changes here only reach the wire when the final handler executes the options rather than the
        /// command text. <see cref="CachingMiddleware"/> reads <see cref="CurlOptions.Method"/> (it caches only GET) and
        /// <see cref="AuthenticationMiddleware"/> sets the <c>Authorization</c> header here (creating the options when
        /// they are null).</para>
        /// <para>Example: <c>context.Options.MaxTime = 10;</c> gives the request a 10-second <c>--max-time</c>.</para>
        /// </remarks>
        /// <seealso cref="Command"/>
        /// <seealso cref="CurlOptions"/>
        public CurlOptions Options { get; set; }

        /// <summary>
        /// A per-request bag for passing values between middleware and the final handler, such as a correlation
        /// id, a stopwatch or the user the request is made for.
        /// </summary>
        /// <value>A case-sensitive dictionary, empty by default. <see cref="TimingMiddleware"/> writes
        /// <c>TimingStart</c> and <c>TimingEnd</c> (UTC <see cref="DateTime"/>) and <c>TimingDuration</c> (<see cref="TimeSpan"/>).</value>
        /// <remarks>
        /// <para>Set values with <see cref="WithProperty(string, object)"/> or the indexer, and read them back typed with
        /// <see cref="GetProperty{T}(string)"/>. Keys are ordinary strings; prefix your own keys (for example
        /// <c>"myapp.tenant"</c>) to avoid clashes with other middleware.</para>
        /// <para>Example: <c>context.Properties["requestId"] = Guid.NewGuid();</c> in one middleware and
        /// <c>var id = context.GetProperty&lt;Guid&gt;("requestId");</c> in another.</para>
        /// </remarks>
        /// <seealso cref="WithProperty(string, object)"/>
        /// <seealso cref="GetProperty{T}(string)"/>
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// The token that cancels this request; built-in middleware pass it to their waits (retry back-off,
        /// rate limiting) and your handler should pass it to <c>Curl.ExecuteAsync</c>.
        /// </summary>
        /// <value>Default <see cref="System.Threading.CancellationToken.None"/> (never cancelled).</value>
        /// <remarks>
        /// <para>The pipeline does not pass this token to the handler by itself; write the handler as
        /// <c>ctx =&gt; Curl.ExecuteAsync(ctx.Command, ctx.CancellationToken)</c> so cancelling stops the transfer too.
        /// When it is cancelled during a <see cref="RetryMiddleware"/> or <see cref="RateLimitMiddleware"/> wait, the
        /// request ends with <see cref="CurlDotNet.Exceptions.CurlAbortedByCallbackException"/> (curl exit code 42).</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md</para>
        /// </remarks>
        /// <seealso cref="Curl.ExecuteAsync(string, System.Threading.CancellationToken)"/>
        public CancellationToken CancellationToken { get; set; }

        /// <summary>
        /// When this context was created (UTC), useful for measuring how long the whole pipeline took,
        /// including retries and rate-limit waits.
        /// </summary>
        /// <value><see cref="DateTime.UtcNow"/> at construction unless you set it; always in UTC.</value>
        /// <remarks>
        /// <para>Example: <c>var elapsed = DateTime.UtcNow - context.StartTime;</c> after <c>await next()</c>.
        /// For the transfer alone, use <see cref="CurlResult.Timings"/>.</para>
        /// </remarks>
        /// <seealso cref="TimingMiddleware"/>
        /// <seealso cref="CurlTimings"/>
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Stores <paramref name="value"/> in <see cref="Properties"/> under <paramref name="key"/> (replacing any
        /// earlier value) and returns this context, so calls can be chained.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>var ctx = new CurlContext { Command = "curl https://api.example.com" }.WithProperty("tenant", "acme").WithProperty("attempt", 1);</c></para>
        /// </remarks>
        /// <param name="key">The property name, for example <c>"tenant"</c>. Must not be null.</param>
        /// <param name="value">Any value, including null.</param>
        /// <returns>This same <see cref="CurlContext"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null. Fix: pass a non-null key such as <c>"tenant"</c>.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md</exception>
        /// <seealso cref="GetProperty{T}(string)"/>
        /// <seealso cref="Properties"/>
        public CurlContext WithProperty(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key), CurlException.Compose(
                    "CurlContext.WithProperty(key, value) was called with a null key.",
                    "Pass a non-null key such as \"tenant\" or \"requestId\"; read it back later with GetProperty<T>(key).",
                    DocLinks.LoggingGuide));
            }
            Properties[key] = value;
            return this;
        }

        /// <summary>
        /// Reads a value from <see cref="Properties"/> as <typeparamref name="T"/>, returning <c>default(T)</c> when
        /// the key is missing or the value is of another type (it never throws for a bad cast).
        /// </summary>
        /// <remarks>
        /// <para>The value is cast, not converted: an <c>int</c> stored under a key cannot be read as <c>long</c>
        /// (you get 0). Use <c>Properties.ContainsKey(key)</c> when you need to tell "missing" from a stored default.</para>
        /// <para>Example: <c>var duration = context.GetProperty&lt;TimeSpan&gt;("TimingDuration");</c> after
        /// <see cref="TimingMiddleware"/> has run.</para>
        /// </remarks>
        /// <typeparam name="T">The type the value was stored as.</typeparam>
        /// <param name="key">The property name used with <see cref="WithProperty(string, object)"/>. Must not be null.</param>
        /// <returns>The stored value, or <c>default(T)</c> (null, 0, false...) when it is missing or not a <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null. Fix: pass a non-null key such as <c>"tenant"</c>.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md</exception>
        /// <seealso cref="WithProperty(string, object)"/>
        public T GetProperty<T>(string key)
        {
            // `is` instead of a cast: a stored null read as a value type gives default(T) instead of a
            // NullReferenceException, and a value of another type gives default(T) as before
            if (Properties.TryGetValue(key, out var value) && value is T typed)
            {
                return typed;
            }
            return default(T);
        }
    }
}
