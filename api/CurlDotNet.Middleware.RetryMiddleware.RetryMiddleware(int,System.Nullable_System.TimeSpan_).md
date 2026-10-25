#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[RetryMiddleware](CurlDotNet.Middleware.RetryMiddleware.md 'CurlDotNet\.Middleware\.RetryMiddleware')

## RetryMiddleware\(int, Nullable\<TimeSpan\>\) Constructor

Creates a retry middleware that allows [maxRetries](CurlDotNet.Middleware.RetryMiddleware.RetryMiddleware(int,System.Nullable_System.TimeSpan_).md#CurlDotNet.Middleware.RetryMiddleware.RetryMiddleware(int,System.Nullable_System.TimeSpan_).maxRetries 'CurlDotNet\.Middleware\.RetryMiddleware\.RetryMiddleware\(int, System\.Nullable\<System\.TimeSpan\>\)\.maxRetries') retries after the first attempt,
waiting [initialDelay](CurlDotNet.Middleware.RetryMiddleware.RetryMiddleware(int,System.Nullable_System.TimeSpan_).md#CurlDotNet.Middleware.RetryMiddleware.RetryMiddleware(int,System.Nullable_System.TimeSpan_).initialDelay 'CurlDotNet\.Middleware\.RetryMiddleware\.RetryMiddleware\(int, System\.Nullable\<System\.TimeSpan\>\)\.initialDelay') before the first retry and doubling the wait each time\.

```csharp
public RetryMiddleware(int maxRetries=3, System.Nullable<System.TimeSpan> initialDelay=null);
```
#### Parameters

<a name='CurlDotNet.Middleware.RetryMiddleware.RetryMiddleware(int,System.Nullable_System.TimeSpan_).maxRetries'></a>

`maxRetries` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

Retries after the first attempt, 0 or more; default 3\. A negative value means no
            attempt is made at all and every request fails with [CurlRetryException](CurlDotNet.Exceptions.CurlRetryException.md 'CurlDotNet\.Exceptions\.CurlRetryException')\.

<a name='CurlDotNet.Middleware.RetryMiddleware.RetryMiddleware(int,System.Nullable_System.TimeSpan_).initialDelay'></a>

`initialDelay` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

Wait before the first retry; null means 1 second, [System\.TimeSpan\.Zero](https://learn.microsoft.com/en-us/dotnet/api/system.timespan.zero 'System\.TimeSpan\.Zero')
            retries immediately\. Must not be negative\.

#### Exceptions

[System\.ArgumentOutOfRangeException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception 'System\.ArgumentOutOfRangeException')  
[initialDelay](CurlDotNet.Middleware.RetryMiddleware.RetryMiddleware(int,System.Nullable_System.TimeSpan_).md#CurlDotNet.Middleware.RetryMiddleware.RetryMiddleware(int,System.Nullable_System.TimeSpan_).initialDelay 'CurlDotNet\.Middleware\.RetryMiddleware\.RetryMiddleware\(int, System\.Nullable\<System\.TimeSpan\>\)\.initialDelay') is negative\. Fix: pass
            [System\.TimeSpan\.Zero](https://learn.microsoft.com/en-us/dotnet/api/system.timespan.zero 'System\.TimeSpan\.Zero') or a positive value\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/guides/advanced\.md

### Remarks

Example: `new RetryMiddleware()` = 3 retries at 1 s, 2 s, 4 s; `new RetryMiddleware(0)` never
            retries (useful to switch retrying off by configuration).

### See Also
- [UseRetry\(int, Nullable&lt;TimeSpan&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRetry(int,System.Nullable_System.TimeSpan_).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseRetry\(int, System\.Nullable\<System\.TimeSpan\>\)')