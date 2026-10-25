#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

## CurlMiddlewarePipelineBuilder\.UseRetry\(int, Nullable\<TimeSpan\>\) Method

Adds a [RetryMiddleware](CurlDotNet.Middleware.RetryMiddleware.md 'CurlDotNet\.Middleware\.RetryMiddleware') that re\-runs the rest of the pipeline on 5xx/429 responses and on
transient curl errors, waiting [delay](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRetry(int,System.Nullable_System.TimeSpan_).md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRetry(int,System.Nullable_System.TimeSpan_).delay 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseRetry\(int, System\.Nullable\<System\.TimeSpan\>\)\.delay'), then twice as long, and so on\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder UseRetry(int maxRetries=3, System.Nullable<System.TimeSpan> delay=null);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRetry(int,System.Nullable_System.TimeSpan_).maxRetries'></a>

`maxRetries` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

Retries after the first attempt; 0 disables retrying\. Default 3\.

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRetry(int,System.Nullable_System.TimeSpan_).delay'></a>

`delay` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

Wait before the first retry, doubled for each further retry; null means 1 second\. Must not be negative\.

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')  
This builder\.

#### Exceptions

[System\.ArgumentOutOfRangeException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception 'System\.ArgumentOutOfRangeException')  
[delay](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRetry(int,System.Nullable_System.TimeSpan_).md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRetry(int,System.Nullable_System.TimeSpan_).delay 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseRetry\(int, System\.Nullable\<System\.TimeSpan\>\)\.delay') is negative\. Fix: pass
            [System\.TimeSpan\.Zero](https://learn.microsoft.com/en-us/dotnet/api/system.timespan.zero 'System\.TimeSpan\.Zero') or a positive delay\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/guides/advanced\.md

### Remarks

With the defaults a failing request is tried 4 times (1 + 3 retries) with waits of 1 s, 2 s and 4 s.
            Retried exceptions are those whose [IsRetryable\(\)](CurlDotNet.Exceptions.CurlException.IsRetryable().md 'CurlDotNet\.Exceptions\.CurlException\.IsRetryable\(\)') is true (DNS failure 6, connect
            failure 7, timeout 28, empty reply 52, send/receive errors 55/56, HTTP 429/5xx); others are rethrown at once.
            When retries run out on an exception, [CurlRetryException](CurlDotNet.Exceptions.CurlRetryException.md 'CurlDotNet\.Exceptions\.CurlRetryException') is thrown; when they run out on a 5xx/429
            response, that last response is returned.

Example: `builder.UseRetry(maxRetries: 5, delay: TimeSpan.FromMilliseconds(200));` waits 0.2, 0.4, 0.8, 1.6 and 3.2 seconds.

### See Also
- [RetryMiddleware](CurlDotNet.Middleware.RetryMiddleware.md 'CurlDotNet\.Middleware\.RetryMiddleware')