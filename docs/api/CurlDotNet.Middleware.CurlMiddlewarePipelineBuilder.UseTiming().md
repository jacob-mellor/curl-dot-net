#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

## CurlMiddlewarePipelineBuilder\.UseTiming\(\) Method

Adds a [TimingMiddleware](CurlDotNet.Middleware.TimingMiddleware.md 'CurlDotNet\.Middleware\.TimingMiddleware') that sets [Total](CurlDotNet.Core.CurlTimings.Total.md 'CurlDotNet\.Core\.CurlTimings\.Total') on the result to the time the
rest of the pipeline took, and records start, end and duration in [Properties](CurlDotNet.Middleware.CurlContext.Properties.md 'CurlDotNet\.Middleware\.CurlContext\.Properties')\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder UseTiming();
```

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')  
This builder\.

### Remarks

Example: `builder.UseTiming();` then `var took = context.GetProperty<TimeSpan>("TimingDuration");`.

### See Also
- [TimingMiddleware](CurlDotNet.Middleware.TimingMiddleware.md 'CurlDotNet\.Middleware\.TimingMiddleware')