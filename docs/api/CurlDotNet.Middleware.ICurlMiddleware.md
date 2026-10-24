#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## ICurlMiddleware Interface

A step that wraps every curl request in a [CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline'): it can inspect or
change the request before it runs, decide whether it runs at all, and inspect or replace the result\.

```csharp
public interface ICurlMiddleware
```

Derived  
&#8627; [AuthenticationMiddleware](CurlDotNet.Middleware.AuthenticationMiddleware.md 'CurlDotNet\.Middleware\.AuthenticationMiddleware')  
&#8627; [CachingMiddleware](CurlDotNet.Middleware.CachingMiddleware.md 'CurlDotNet\.Middleware\.CachingMiddleware')  
&#8627; [LoggingMiddleware](CurlDotNet.Middleware.LoggingMiddleware.md 'CurlDotNet\.Middleware\.LoggingMiddleware')  
&#8627; [RateLimitMiddleware](CurlDotNet.Middleware.RateLimitMiddleware.md 'CurlDotNet\.Middleware\.RateLimitMiddleware')  
&#8627; [RequestModifierMiddleware](CurlDotNet.Middleware.RequestModifierMiddleware.md 'CurlDotNet\.Middleware\.RequestModifierMiddleware')  
&#8627; [ResponseModifierMiddleware](CurlDotNet.Middleware.ResponseModifierMiddleware.md 'CurlDotNet\.Middleware\.ResponseModifierMiddleware')  
&#8627; [RetryMiddleware](CurlDotNet.Middleware.RetryMiddleware.md 'CurlDotNet\.Middleware\.RetryMiddleware')  
&#8627; [TimingMiddleware](CurlDotNet.Middleware.TimingMiddleware.md 'CurlDotNet\.Middleware\.TimingMiddleware')

### Example

```csharp
public class LoggingMiddleware : ICurlMiddleware
{
    public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
    {
        Console.WriteLine($"Executing: {context.Command}");
        var result = await next();
        Console.WriteLine($"Status: {result.StatusCode}");
        return result;
    }
}
```

### Remarks

Implement this interface for cross-cutting concerns such as logging, metrics, retries, caching,
            authentication headers or request signing, then register it with
            [Use\(ICurlMiddleware\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(CurlDotNet.Middleware.ICurlMiddleware) 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)') or [Use\(ICurlMiddleware\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware) 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)').
            The library ships ready-made ones: [LoggingMiddleware](CurlDotNet.Middleware.LoggingMiddleware.md 'CurlDotNet\.Middleware\.LoggingMiddleware'), [RetryMiddleware](CurlDotNet.Middleware.RetryMiddleware.md 'CurlDotNet\.Middleware\.RetryMiddleware'),
            [TimingMiddleware](CurlDotNet.Middleware.TimingMiddleware.md 'CurlDotNet\.Middleware\.TimingMiddleware'), [CachingMiddleware](CurlDotNet.Middleware.CachingMiddleware.md 'CurlDotNet\.Middleware\.CachingMiddleware'), [AuthenticationMiddleware](CurlDotNet.Middleware.AuthenticationMiddleware.md 'CurlDotNet\.Middleware\.AuthenticationMiddleware'),
            [RateLimitMiddleware](CurlDotNet.Middleware.RateLimitMiddleware.md 'CurlDotNet\.Middleware\.RateLimitMiddleware'), [RequestModifierMiddleware](CurlDotNet.Middleware.RequestModifierMiddleware.md 'CurlDotNet\.Middleware\.RequestModifierMiddleware') and [ResponseModifierMiddleware](CurlDotNet.Middleware.ResponseModifierMiddleware.md 'CurlDotNet\.Middleware\.ResponseModifierMiddleware').

How a call flows: the first middleware registered runs first. Its `next` delegate runs the
            second middleware, and so on; the last `next` runs the pipeline's final handler (usually
            `ctx => Curl.ExecuteAsync(ctx.Command)`). Code before `await next()` runs on the way in,
            code after it on the way out, in reverse order. Not calling `next()` short-circuits the request
            (a cache hit, for example); calling it more than once runs the rest of the pipeline again (a retry).
            An exception thrown by a middleware, by the handler or by `next()` travels back out through
            every middleware that awaited it, so a `try/catch` around `await next()` sees failures
            from everything registered after it.

Thread-safety: a pipeline reuses the same middleware instance for every request and may run
            several requests at once, so keep per-request state in [Properties](CurlDotNet.Middleware.CurlContext.Properties.md 'CurlDotNet\.Middleware\.CurlContext\.Properties') or local
            variables, not in fields (or protect the fields with a lock).

Example: a class with `public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)`
            whose body is `var sw = Stopwatch.StartNew(); var result = await next(); log.LogInformation("{Url} {Status} {Ms}ms", context.Options?.Url, result.StatusCode, sw.ElapsedMilliseconds); return result;`
            logs every request; register it with `CurlMiddlewarePipeline.CreateBuilder().Use(new MyLoggingMiddleware(log)).WithHandler(ctx => Curl.ExecuteAsync(ctx.Command)).Build()`.

Guide with a Serilog / Microsoft.Extensions.Logging middleware:
            https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md

### See Also
- [CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline')
- [CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')
- [CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')

| Methods | |
| :--- | :--- |
| [ExecuteAsync\(CurlContext, Func&lt;Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.ICurlMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Runs this middleware for one request: do work before and/or after `await next()`, and return the result \(the one [next](CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md#CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).next 'CurlDotNet\.Middleware\.ICurlMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)\.next') produced, a changed one, or your own\)\. |
