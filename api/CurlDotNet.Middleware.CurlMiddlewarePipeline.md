#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## CurlMiddlewarePipeline Class

Manages the middleware pipeline for curl operations\.

```csharp
public class CurlMiddlewarePipeline
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlMiddlewarePipeline

| Constructors | |
| :--- | :--- |
| [CurlMiddlewarePipeline\(Func&lt;CurlContext,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.CurlMiddlewarePipeline(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.CurlMiddlewarePipeline\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Initialize a new middleware pipeline\. |

| Properties | |
| :--- | :--- |
| [Count](CurlDotNet.Middleware.CurlMiddlewarePipeline.Count.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Count') | Get the count of middleware in the pipeline\. |

| Methods | |
| :--- | :--- |
| [Clear\(\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Clear().md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Clear\(\)') | Clear all middleware from the pipeline\. |
| [CreateBuilder\(\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.CreateBuilder().md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.CreateBuilder\(\)') | Create a new pipeline builder\. |
| [ExecuteAsync\(CurlContext\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.ExecuteAsync(CurlDotNet.Middleware.CurlContext).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext\)') | Execute the pipeline\. |
| [Use\(ICurlMiddleware\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware) 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)') | Add middleware to the pipeline \(fluent\)\. |
| [Use\(Func&lt;CurlContext,Func&lt;Task&lt;CurlResult&gt;&gt;,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__) 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Add middleware using a delegate \(fluent\)\. |
