#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline')

## CurlMiddlewarePipeline\.Count Property

How many middleware are registered \(the final handler is not counted\); 0 means requests go straight to
the handler\.

```csharp
public int Count { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
The number of [Use\(ICurlMiddleware\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Use.md#CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware) 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Use\(CurlDotNet\.Middleware\.ICurlMiddleware\)') registrations, including duplicates\.

### Remarks

Example: `Debug.Assert(pipeline.Count == 3);` after building with logging, retry and timing.

### See Also
- [Clear\(\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.Clear().md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.Clear\(\)')