#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline')

## CurlMiddlewarePipeline\.Clear\(\) Method

Removes every middleware, leaving only the final handler, so [ExecuteAsync\(CurlContext\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.ExecuteAsync(CurlDotNet.Middleware.CurlContext).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext\)') then runs
the request directly\.

```csharp
public void Clear();
```

### Remarks

Useful in tests or to reconfigure at start-up. Not safe to call while requests are running. It does not
            clear the shared cache of [CachingMiddleware](CurlDotNet.Middleware.CachingMiddleware.md 'CurlDotNet\.Middleware\.CachingMiddleware'); use [ClearCache\(\)](CurlDotNet.Middleware.CachingMiddleware.ClearCache().md 'CurlDotNet\.Middleware\.CachingMiddleware\.ClearCache\(\)') for that.

Example: `pipeline.Clear(); pipeline.Use(new TimingMiddleware());`

### See Also
- [Count](CurlDotNet.Middleware.CurlMiddlewarePipeline.Count.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Count')
- [Use\(ICurlMiddleware\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware) 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)')