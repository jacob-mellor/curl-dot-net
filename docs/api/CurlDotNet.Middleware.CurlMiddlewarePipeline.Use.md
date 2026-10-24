#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline')

## CurlMiddlewarePipeline\.Use Method

| Overloads | |
| :--- | :--- |
| [Use\(ICurlMiddleware\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware) 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)') | Appends [middleware](CurlDotNet.Middleware.CurlMiddlewarePipeline.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware).middleware 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)\.middleware') to the end of the chain \(it runs after everything added before it\) and returns this pipeline for chaining\. |
| [Use\(Func&lt;CurlContext,Func&lt;Task&lt;CurlResult&gt;&gt;,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__) 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Appends an inline middleware written as a lambda `(context, next) => ...` and returns this pipeline for chaining; handy for one\-off steps that do not deserve a class\. |

<a name='CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware)'></a>

## CurlMiddlewarePipeline\.Use\(ICurlMiddleware\) Method

Appends [middleware](CurlDotNet.Middleware.CurlMiddlewarePipeline.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware).middleware 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)\.middleware') to the end of the chain \(it runs after everything added before it\)
and returns this pipeline for chaining\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipeline Use(CurlDotNet.Middleware.ICurlMiddleware middleware);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware).middleware'></a>

`middleware` [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')

The middleware to add; must not be null\.

#### Returns
[CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline')  
This pipeline\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[middleware](CurlDotNet.Middleware.CurlMiddlewarePipeline.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware).middleware 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)\.middleware') is null\. Fix: pass an
            [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware') instance\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/guides/logging\-observability\.md

### Remarks

The same instance is used for every request, possibly concurrently, so it must be thread-safe.
            Adding the same instance twice makes it run twice per request. Do not call this while
            [ExecuteAsync\(CurlContext\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.ExecuteAsync(CurlDotNet.Middleware.CurlContext).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext\)') is running on another thread.

Example: `pipeline.Use(new LoggingMiddleware(msg => logger.LogInformation(msg))).Use(new RetryMiddleware(2));`

### See Also
- [Use\(Func&lt;CurlContext,Func&lt;Task&lt;CurlResult&gt;&gt;,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__) 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)')
- [Use\(ICurlMiddleware\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(CurlDotNet.Middleware.ICurlMiddleware) 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)')

<a name='CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__)'></a>

## CurlMiddlewarePipeline\.Use\(Func\<CurlContext,Func\<Task\<CurlResult\>\>,Task\<CurlResult\>\>\) Method

Appends an inline middleware written as a lambda `(context, next) => ...` and returns this pipeline
for chaining; handy for one\-off steps that do not deserve a class\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipeline Use(System.Func<CurlDotNet.Middleware.CurlContext,System.Func<System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>>,System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>> middleware);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).middleware'></a>

`middleware` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')[System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')

The step to run; must not be null\.

#### Returns
[CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline')  
This pipeline\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[middleware](CurlDotNet.Middleware.CurlMiddlewarePipeline.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).middleware 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)\.middleware') is null\. Fix: pass a lambda such as
            `(ctx, next) => next()`\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/guides/logging\-observability\.md

### Remarks

The lambda follows the same rules as [ExecuteAsync\(CurlContext, Func&lt;Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.ICurlMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)'):
            call `next()` to continue, skip it to short-circuit.

Example: `pipeline.Use(async (ctx, next) => { ctx.Command += " -H 'X-Trace: 1'"; return await next(); });`

### See Also
- [Use\(ICurlMiddleware\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware) 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)')