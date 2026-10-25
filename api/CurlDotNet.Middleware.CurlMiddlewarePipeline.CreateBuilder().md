#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline')

## CurlMiddlewarePipeline\.CreateBuilder\(\) Method

Starts a fluent [CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder'), the shortest way to assemble a pipeline with the
built\-in middleware: `CreateBuilder().UseLogging().UseRetry().WithHandler(...).Build()`\.

```csharp
public static CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder CreateBuilder();
```

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')  
A new, empty builder\.

### Remarks

Example: `var pipeline = CurlMiddlewarePipeline.CreateBuilder().UseTiming().UseCaching(TimeSpan.FromMinutes(1)).WithHandler(ctx => Curl.ExecuteAsync(ctx.Command)).Build();`

### See Also
- [CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')
- [Build\(\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Build().md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.Build\(\)')