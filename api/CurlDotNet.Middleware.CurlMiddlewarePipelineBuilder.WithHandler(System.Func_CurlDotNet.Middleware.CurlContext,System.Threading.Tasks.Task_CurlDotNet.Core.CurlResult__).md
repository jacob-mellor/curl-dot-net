#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

## CurlMiddlewarePipelineBuilder\.WithHandler\(Func\<CurlContext,Task\<CurlResult\>\>\) Method

Sets the final handler that runs the request after all middleware \(required before [Build\(\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Build().md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.Build\(\)')\);
calling it again replaces the previous handler\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder WithHandler(System.Func<CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>> handler);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.WithHandler(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).handler'></a>

`handler` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

Runs the request; null is accepted here but makes [Build\(\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Build().md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.Build\(\)') throw\.

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')  
This builder\.

### Remarks

Use `ctx => Curl.ExecuteAsync(ctx.Command, ctx.CancellationToken)` to send the command text, or
            `ctx => Curl.ExecuteAsync(ctx.Options, ctx.CancellationToken)` when middleware change
            [Options](CurlDotNet.Middleware.CurlContext.Options.md 'CurlDotNet\.Middleware\.CurlContext\.Options'). In unit tests return a canned [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult').

Example: `builder.WithHandler(ctx => Task.FromResult(new CurlResult { StatusCode = 200, Body = "{}" }));`

### See Also
- [Build\(\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Build().md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.Build\(\)')
- [CurlMiddlewarePipeline\(Func&lt;CurlContext,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.CurlMiddlewarePipeline(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.CurlMiddlewarePipeline\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)')