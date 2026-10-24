#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

## CurlMiddlewarePipelineBuilder\.UseRateLimit\(int\) Method

Adds a [RateLimitMiddleware](CurlDotNet.Middleware.RateLimitMiddleware.md 'CurlDotNet\.Middleware\.RateLimitMiddleware') that delays requests so that no more than
[requestsPerSecond](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRateLimit(int).md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRateLimit(int).requestsPerSecond 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseRateLimit\(int\)\.requestsPerSecond') complete in any one\-second window\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder UseRateLimit(int requestsPerSecond);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRateLimit(int).requestsPerSecond'></a>

`requestsPerSecond` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

Requests allowed per second; 1 or more\.

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')  
This builder\.

#### Exceptions

[System\.ArgumentOutOfRangeException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception 'System\.ArgumentOutOfRangeException')  
[requestsPerSecond](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRateLimit(int).md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRateLimit(int).requestsPerSecond 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseRateLimit\(int\)\.requestsPerSecond') is 0 or negative\. Fix: pass 1
            or more\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/guides/advanced\.md

### Remarks

Example: `builder.UseRateLimit(5);` keeps a loop of calls under an API's 5-requests-per-second limit.

### See Also
- [RateLimitMiddleware](CurlDotNet.Middleware.RateLimitMiddleware.md 'CurlDotNet\.Middleware\.RateLimitMiddleware')