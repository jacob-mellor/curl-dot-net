#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## CurlMiddlewarePipelineBuilder Class

Fluent builder for creating middleware pipelines\.

```csharp
public class CurlMiddlewarePipelineBuilder
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlMiddlewarePipelineBuilder

| Methods | |
| :--- | :--- |
| [Build\(\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Build().md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.Build\(\)') | Build the pipeline\. |
| [Use\(ICurlMiddleware\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(CurlDotNet.Middleware.ICurlMiddleware) 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)') | Add middleware to the pipeline\. |
| [Use\(Func&lt;CurlContext,Func&lt;Task&lt;CurlResult&gt;&gt;,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__) 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.Use\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Add middleware using a delegate\. |
| [UseAuthentication\(Func&lt;CurlContext,Task&lt;string&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseAuthentication(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_string__).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseAuthentication\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<string\>\>\)') | Add authentication middleware\. |
| [UseCaching\(Nullable&lt;TimeSpan&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseCaching(System.Nullable_System.TimeSpan_).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseCaching\(System\.Nullable\<System\.TimeSpan\>\)') | Add caching middleware\. |
| [UseLogging\(Action&lt;string&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseLogging(System.Action_string_).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseLogging\(System\.Action\<string\>\)') | Add logging middleware\. |
| [UseRateLimit\(int\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRateLimit(int).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseRateLimit\(int\)') | Add rate limiting middleware\. |
| [UseRetry\(int, Nullable&lt;TimeSpan&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRetry(int,System.Nullable_System.TimeSpan_).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseRetry\(int, System\.Nullable\<System\.TimeSpan\>\)') | Add retry middleware\. |
| [UseTiming\(\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseTiming().md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseTiming\(\)') | Add timing middleware\. |
| [WithHandler\(Func&lt;CurlContext,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.WithHandler(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.WithHandler\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Set the final handler\. |
