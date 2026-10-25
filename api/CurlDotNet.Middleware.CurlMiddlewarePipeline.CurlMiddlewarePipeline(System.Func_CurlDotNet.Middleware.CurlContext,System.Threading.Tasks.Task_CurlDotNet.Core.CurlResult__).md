#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline')

## CurlMiddlewarePipeline\(Func\<CurlContext,Task\<CurlResult\>\>\) Constructor

Creates an empty pipeline whose last step, after all middleware, is [finalHandler](CurlDotNet.Middleware.CurlMiddlewarePipeline.CurlMiddlewarePipeline(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md#CurlDotNet.Middleware.CurlMiddlewarePipeline.CurlMiddlewarePipeline(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).finalHandler 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.CurlMiddlewarePipeline\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)\.finalHandler')
\(the code that really runs the request\)\.

```csharp
public CurlMiddlewarePipeline(System.Func<CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>> finalHandler);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipeline.CurlMiddlewarePipeline(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).finalHandler'></a>

`finalHandler` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

Runs the request and returns its result; must not be null\. It may throw
            [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') subtypes, which flow back through the middleware\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[finalHandler](CurlDotNet.Middleware.CurlMiddlewarePipeline.CurlMiddlewarePipeline(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md#CurlDotNet.Middleware.CurlMiddlewarePipeline.CurlMiddlewarePipeline(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).finalHandler 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.CurlMiddlewarePipeline\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)\.finalHandler') is null\. Fix: pass a handler such as
            `ctx => Curl.ExecuteAsync(ctx.Command)`\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/guides/logging\-observability\.md

### Remarks

The handler receives the [CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext') as changed by the middleware. Typical handlers:
            `ctx => Curl.ExecuteAsync(ctx.Command, ctx.CancellationToken)` (sends the command text) or
            `ctx => Curl.ExecuteAsync(ctx.Options, ctx.CancellationToken)` (sends the parsed options, so changes
            middleware made to [Options](CurlDotNet.Middleware.CurlContext.Options.md 'CurlDotNet\.Middleware\.CurlContext\.Options') take effect). In tests, a handler that returns a fixed
            `new CurlResult { StatusCode = 200, Body = "ok" }` lets you exercise middleware without a network.

Example: `var pipeline = new CurlMiddlewarePipeline(ctx => Curl.ExecuteAsync(ctx.Command)).Use(new TimingMiddleware());`

### See Also
- [CreateBuilder\(\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.CreateBuilder().md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.CreateBuilder\(\)')
- [WithHandler\(Func&lt;CurlContext,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.WithHandler(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.WithHandler\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)')