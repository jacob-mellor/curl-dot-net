#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

## CurlMiddlewarePipelineBuilder\.UseCaching\(Nullable\<TimeSpan\>\) Method

Adds a [CachingMiddleware](CurlDotNet.Middleware.CachingMiddleware.md 'CurlDotNet\.Middleware\.CachingMiddleware') that answers repeated successful GET requests from memory for
[ttl](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseCaching(System.Nullable_System.TimeSpan_).md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseCaching(System.Nullable_System.TimeSpan_).ttl 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseCaching\(System\.Nullable\<System\.TimeSpan\>\)\.ttl') instead of sending them again\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder UseCaching(System.Nullable<System.TimeSpan> ttl=null);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseCaching(System.Nullable_System.TimeSpan_).ttl'></a>

`ttl` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

How long a stored result is reused; null means 5 minutes\.

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')  
This builder\.

### Remarks

The cache is shared by every [CachingMiddleware](CurlDotNet.Middleware.CachingMiddleware.md 'CurlDotNet\.Middleware\.CachingMiddleware') in the process and keyed on
            [Command](CurlDotNet.Middleware.CurlContext.Command.md 'CurlDotNet\.Middleware\.CurlContext\.Command') (or the URL when there is no command). Only 2xx results are stored.

Example: `builder.UseCaching(TimeSpan.FromSeconds(30));`

### See Also
- [CachingMiddleware](CurlDotNet.Middleware.CachingMiddleware.md 'CurlDotNet\.Middleware\.CachingMiddleware')
- [ClearCache\(\)](CurlDotNet.Middleware.CachingMiddleware.ClearCache().md 'CurlDotNet\.Middleware\.CachingMiddleware\.ClearCache\(\)')