#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')

## ICurlMiddleware\.ExecuteAsync\(CurlContext, Func\<Task\<CurlResult\>\>\) Method

Runs this middleware for one request: do work before and/or after `await next()`, and return
the result \(the one [next](CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md#CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).next 'CurlDotNet\.Middleware\.ICurlMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)\.next') produced, a changed one, or your own\)\.

```csharp
System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(CurlDotNet.Middleware.CurlContext context, System.Func<System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>> next);
```
#### Parameters

<a name='CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).context'></a>

`context` [CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')

The request being run: [Command](CurlDotNet.Middleware.CurlContext.Command.md 'CurlDotNet\.Middleware\.CurlContext\.Command') \(curl command text\),
            [Options](CurlDotNet.Middleware.CurlContext.Options.md 'CurlDotNet\.Middleware\.CurlContext\.Options') \(parsed options, may be null if the caller only set a command\),
            [Properties](CurlDotNet.Middleware.CurlContext.Properties.md 'CurlDotNet\.Middleware\.CurlContext\.Properties') \(a bag for passing data between middleware\) and the
            [CancellationToken](CurlDotNet.Middleware.CurlContext.CancellationToken.md 'CurlDotNet\.Middleware\.CurlContext\.CancellationToken')\. Never null when called by the pipeline\.

<a name='CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).next'></a>

`next` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')

Runs the rest of the pipeline \(the following middleware, then the final handler\)
            and returns its result; it throws whatever they throw\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The result to hand back to the previous middleware \(or to the caller\)\. Returning null is
            allowed by the pipeline but most callers expect a result\.

### Remarks

Call [next](CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md#CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).next 'CurlDotNet\.Middleware\.ICurlMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)\.next') at most once for a normal pass-through, more than once to retry,
            or not at all to short-circuit (then return your own [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'), for example a cached
            one). Changes you make to [Command](CurlDotNet.Middleware.CurlContext.Command.md 'CurlDotNet\.Middleware\.CurlContext\.Command') or [Options](CurlDotNet.Middleware.CurlContext.Options.md 'CurlDotNet\.Middleware\.CurlContext\.Options') before
            calling [next](CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md#CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).next 'CurlDotNet\.Middleware\.ICurlMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)\.next') are seen by the later middleware and by the final handler; whether
            they reach the wire depends on the handler (`ctx => Curl.ExecuteAsync(ctx.Command)` sends the
            command text, `ctx => Curl.ExecuteAsync(ctx.Options)` sends the options).

Exceptions: whatever you throw reaches the caller of [ExecuteAsync\(CurlContext\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.ExecuteAsync(CurlDotNet.Middleware.CurlContext).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext\)')
            through the middleware registered before you. Failed transfers arrive from [next](CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md#CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).next 'CurlDotNet\.Middleware\.ICurlMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)\.next') as
            [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') subtypes; rethrow with `throw;` to keep the
            stack trace. Honour [CancellationToken](CurlDotNet.Middleware.CurlContext.CancellationToken.md 'CurlDotNet\.Middleware\.CurlContext\.CancellationToken') in any waiting you do.

Example: `public async Task<CurlResult> ExecuteAsync(CurlContext ctx, Func<Task<CurlResult>> next) { ctx.Options.Headers["X-Request-Id"] = Guid.NewGuid().ToString(); return await next(); }`
            adds a header to every request whose handler sends `ctx.Options`.

### See Also
- [ExecuteAsync\(CurlContext\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.ExecuteAsync(CurlDotNet.Middleware.CurlContext).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext\)')
- [CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')