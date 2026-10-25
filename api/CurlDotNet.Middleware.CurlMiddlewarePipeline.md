#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## CurlMiddlewarePipeline Class

Runs curl requests through an ordered chain of [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware') \(logging, retry, caching,
auth\.\.\.\) and then a final handler that actually executes the request\.

```csharp
public class CurlMiddlewarePipeline
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlMiddlewarePipeline

### Remarks

Order: middleware run in the order they were added. The first one added is the outermost: it runs
            first before the request and last after it, so put [LoggingMiddleware](CurlDotNet.Middleware.LoggingMiddleware.md 'CurlDotNet\.Middleware\.LoggingMiddleware') or
            [TimingMiddleware](CurlDotNet.Middleware.TimingMiddleware.md 'CurlDotNet\.Middleware\.TimingMiddleware') first to measure everything, and [RetryMiddleware](CurlDotNet.Middleware.RetryMiddleware.md 'CurlDotNet\.Middleware\.RetryMiddleware') before
            [AuthenticationMiddleware](CurlDotNet.Middleware.AuthenticationMiddleware.md 'CurlDotNet\.Middleware\.AuthenticationMiddleware') if each retry should fetch a fresh token. When every middleware
            has called `next()`, the final handler passed to the constructor runs.

Failures: an exception from the handler or a middleware propagates out of
            [ExecuteAsync\(CurlContext\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.ExecuteAsync(CurlDotNet.Middleware.CurlContext).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext\)') through every middleware that awaited it (each can catch, retry or
            rethrow). Nothing is swallowed by the pipeline itself.

Thread-safety: [ExecuteAsync\(CurlContext\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.ExecuteAsync(CurlDotNet.Middleware.CurlContext).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext\)') can run many requests at once on the same
            pipeline, but [Use\(ICurlMiddleware\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware) 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)') and [Clear\(\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Clear().md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Clear\(\)') must not be called while requests are
            running; configure the pipeline once at start-up (for example as a singleton in ASP.NET Core).

Example: `var pipeline = CurlMiddlewarePipeline.CreateBuilder().UseLogging(Console.WriteLine).UseRetry(3, TimeSpan.FromMilliseconds(500)).WithHandler(ctx => Curl.ExecuteAsync(ctx.Command, ctx.CancellationToken)).Build();`
            then `var result = await pipeline.ExecuteAsync(new CurlContext { Command = "curl https://api.example.com/health" });`.

Logging and metrics guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md
            retries, timeouts and cancellation: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/advanced.md

### See Also
- [CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')
- [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')
- [CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')

| Constructors | |
| :--- | :--- |
| [CurlMiddlewarePipeline\(Func&lt;CurlContext,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.CurlMiddlewarePipeline(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.CurlMiddlewarePipeline\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Creates an empty pipeline whose last step, after all middleware, is [finalHandler](CurlDotNet.Middleware.CurlMiddlewarePipeline.CurlMiddlewarePipeline(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md#CurlDotNet.Middleware.CurlMiddlewarePipeline.CurlMiddlewarePipeline(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).finalHandler 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.CurlMiddlewarePipeline\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)\.finalHandler') \(the code that really runs the request\)\. |

| Properties | |
| :--- | :--- |
| [Count](CurlDotNet.Middleware.CurlMiddlewarePipeline.Count.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Count') | How many middleware are registered \(the final handler is not counted\); 0 means requests go straight to the handler\. |

| Methods | |
| :--- | :--- |
| [Clear\(\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Clear().md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Clear\(\)') | Removes every middleware, leaving only the final handler, so [ExecuteAsync\(CurlContext\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.ExecuteAsync(CurlDotNet.Middleware.CurlContext).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext\)') then runs the request directly\. |
| [CreateBuilder\(\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.CreateBuilder().md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.CreateBuilder\(\)') | Starts a fluent [CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder'), the shortest way to assemble a pipeline with the built\-in middleware: `CreateBuilder().UseLogging().UseRetry().WithHandler(...).Build()`\. |
| [ExecuteAsync\(CurlContext\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.ExecuteAsync(CurlDotNet.Middleware.CurlContext).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext\)') | Runs [context](CurlDotNet.Middleware.CurlMiddlewarePipeline.ExecuteAsync(CurlDotNet.Middleware.CurlContext).md#CurlDotNet.Middleware.CurlMiddlewarePipeline.ExecuteAsync(CurlDotNet.Middleware.CurlContext).context 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext\)\.context') through every middleware in order and then the final handler, and returns the result as it comes back out of the first middleware\. |
| [Use\(ICurlMiddleware\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware) 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)') | Appends [middleware](CurlDotNet.Middleware.CurlMiddlewarePipeline.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware).middleware 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)\.middleware') to the end of the chain \(it runs after everything added before it\) and returns this pipeline for chaining\. |
| [Use\(Func&lt;CurlContext,Func&lt;Task&lt;CurlResult&gt;&gt;,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__) 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Appends an inline middleware written as a lambda `(context, next) => ...` and returns this pipeline for chaining; handy for one\-off steps that do not deserve a class\. |
