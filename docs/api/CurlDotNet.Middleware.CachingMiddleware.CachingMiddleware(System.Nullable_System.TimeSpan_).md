#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CachingMiddleware](CurlDotNet.Middleware.CachingMiddleware.md 'CurlDotNet\.Middleware\.CachingMiddleware')

## CachingMiddleware\(Nullable\<TimeSpan\>\) Constructor

Creates a caching middleware whose stored results are reused for [ttl](CurlDotNet.Middleware.CachingMiddleware.CachingMiddleware(System.Nullable_System.TimeSpan_).md#CurlDotNet.Middleware.CachingMiddleware.CachingMiddleware(System.Nullable_System.TimeSpan_).ttl 'CurlDotNet\.Middleware\.CachingMiddleware\.CachingMiddleware\(System\.Nullable\<System\.TimeSpan\>\)\.ttl') after they were
fetched \(5 minutes when no value is given\)\.

```csharp
public CachingMiddleware(System.Nullable<System.TimeSpan> ttl=null);
```
#### Parameters

<a name='CurlDotNet.Middleware.CachingMiddleware.CachingMiddleware(System.Nullable_System.TimeSpan_).ttl'></a>

`ttl` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

How long a successful result is reused; null means 5 minutes\.

### Remarks

A zero or negative time-to-live stores entries that are already expired, so nothing is ever served from
            the cache.

Example: `new CachingMiddleware(TimeSpan.FromMinutes(1))`

### See Also
- [UseCaching\(Nullable&lt;TimeSpan&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseCaching(System.Nullable_System.TimeSpan_).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseCaching\(System\.Nullable\<System\.TimeSpan\>\)')