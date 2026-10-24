#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[RateLimitMiddleware](CurlDotNet.Middleware.RateLimitMiddleware.md 'CurlDotNet\.Middleware\.RateLimitMiddleware')

## RateLimitMiddleware\(int\) Constructor

Creates a rate limiter that allows [requestsPerSecond](CurlDotNet.Middleware.RateLimitMiddleware.RateLimitMiddleware(int).md#CurlDotNet.Middleware.RateLimitMiddleware.RateLimitMiddleware(int).requestsPerSecond 'CurlDotNet\.Middleware\.RateLimitMiddleware\.RateLimitMiddleware\(int\)\.requestsPerSecond') requests to complete per second\.

```csharp
public RateLimitMiddleware(int requestsPerSecond);
```
#### Parameters

<a name='CurlDotNet.Middleware.RateLimitMiddleware.RateLimitMiddleware(int).requestsPerSecond'></a>

`requestsPerSecond` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

Requests per second, 1 or more \(for example the limit in the API's documentation\)\.

#### Exceptions

[System\.ArgumentOutOfRangeException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception 'System\.ArgumentOutOfRangeException')  
[requestsPerSecond](CurlDotNet.Middleware.RateLimitMiddleware.RateLimitMiddleware(int).md#CurlDotNet.Middleware.RateLimitMiddleware.RateLimitMiddleware(int).requestsPerSecond 'CurlDotNet\.Middleware\.RateLimitMiddleware\.RateLimitMiddleware\(int\)\.requestsPerSecond') is 0 or negative\. Fix: pass 1 or
            more\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/guides/advanced\.md

### Remarks

Example: `new RateLimitMiddleware(2)` spaces sequential requests about half a second apart once two
            have completed within a second.

### See Also
- [UseRateLimit\(int\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRateLimit(int).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseRateLimit\(int\)')