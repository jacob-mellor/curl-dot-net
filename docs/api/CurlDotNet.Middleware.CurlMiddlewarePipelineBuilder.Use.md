#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

## CurlMiddlewarePipelineBuilder\.Use Method

| Overloads | |
| :--- | :--- |
| [Use\(ICurlMiddleware\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(CurlDotNet.Middleware.ICurlMiddleware) 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)') | Adds your own [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware') at this point of the chain and returns the builder for chaining\. |
| [Use\(Func&lt;CurlContext,Func&lt;Task&lt;CurlResult&gt;&gt;,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__) 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.Use\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Adds an inline middleware written as a lambda `(context, next) => ...` and returns the builder for chaining\. |

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(CurlDotNet.Middleware.ICurlMiddleware)'></a>

## CurlMiddlewarePipelineBuilder\.Use\(ICurlMiddleware\) Method

Adds your own [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware') at this point of the chain and returns the builder for chaining\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder Use(CurlDotNet.Middleware.ICurlMiddleware middleware);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(CurlDotNet.Middleware.ICurlMiddleware).middleware'></a>

`middleware` [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')

The middleware to add; must not be null\.

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')  
This builder\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[middleware](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(CurlDotNet.Middleware.ICurlMiddleware).middleware 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)\.middleware') is null\. Fix: pass an instance\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/guides/logging\-observability\.md

### Remarks

The instance is shared by every request the built pipeline runs, so it must be thread-safe.

Example: `builder.Use(new SerilogMiddleware(log));` (see the logging guide for that class).

### See Also
- [Use\(ICurlMiddleware\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware) 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)')
- [Use\(Func&lt;CurlContext,Func&lt;Task&lt;CurlResult&gt;&gt;,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__) 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.Use\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)')

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__)'></a>

## CurlMiddlewarePipelineBuilder\.Use\(Func\<CurlContext,Func\<Task\<CurlResult\>\>,Task\<CurlResult\>\>\) Method

Adds an inline middleware written as a lambda `(context, next) => ...` and returns the builder for chaining\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder Use(System.Func<CurlDotNet.Middleware.CurlContext,System.Func<System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>>,System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>> middleware);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).middleware'></a>

`middleware` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')[System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')

The step to run; must not be null\. Call `next()` to continue the chain\.

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')  
This builder\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[middleware](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).middleware 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.Use\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)\.middleware') is null\. Fix: pass a lambda\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/guides/logging\-observability\.md

### Remarks

Example: `builder.Use(async (ctx, next) => { var r = await next(); if (r.StatusCode == 401) await RefreshTokenAsync(); return r; });`

### See Also
- [Use\(ICurlMiddleware\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(CurlDotNet.Middleware.ICurlMiddleware) 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)')