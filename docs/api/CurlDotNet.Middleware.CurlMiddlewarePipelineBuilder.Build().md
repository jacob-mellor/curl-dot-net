#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

## CurlMiddlewarePipelineBuilder\.Build\(\) Method

Creates the [CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline') with the middleware added so far, in the same order, and the
handler set with [WithHandler\(Func&lt;CurlContext,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.WithHandler(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.WithHandler\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)')\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipeline Build();
```

#### Returns
[CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline')  
A ready\-to\-use pipeline\.

#### Exceptions

[System\.InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/system.invalidoperationexception 'System\.InvalidOperationException')  
No handler was set \(or it was set to null\)\. Fix: call
            `WithHandler(ctx => Curl.ExecuteAsync(ctx.Command))` before `Build()`\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/guides/logging\-observability\.md

### Remarks

The pipeline gets its own copy of the middleware list, so later `Use` calls on the builder do not
            change pipelines that were already built (the middleware instances themselves are shared).

Example: `var pipeline = builder.WithHandler(ctx => Curl.ExecuteAsync(ctx.Command)).Build();`

### See Also
- [WithHandler\(Func&lt;CurlContext,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.WithHandler(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.WithHandler\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)')
- [CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline')