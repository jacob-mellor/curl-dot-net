#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## RateLimitMiddleware Class

Delays requests so that at most a given number complete in any one\-second window, to stay under an API's
requests\-per\-second limit \(and avoid HTTP 429 responses\)\.

```csharp
public class RateLimitMiddleware : CurlDotNet.Middleware.ICurlMiddleware
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; RateLimitMiddleware

Implements [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')

### Remarks

How it counts: the middleware remembers when each request finished. Before a request starts, if
            `requestsPerSecond` requests finished within the last second, it waits until the oldest of them is one second
            old. Requests still in flight are not counted, so many requests started at the same moment (for example with
            `Task.WhenAll`) can exceed the limit; limit concurrency separately (a [System\.Threading\.SemaphoreSlim](https://learn.microsoft.com/en-us/dotnet/api/system.threading.semaphoreslim 'System\.Threading\.SemaphoreSlim')) if you
            fire requests in parallel.

The limit applies per instance: share one instance (one pipeline) between all callers of the same API.
            The bookkeeping is lock-protected, so the instance is thread-safe. A cancelled
            [CancellationToken](CurlDotNet.Middleware.CurlContext.CancellationToken.md 'CurlDotNet\.Middleware\.CurlContext\.CancellationToken') during the wait ends the request with
            [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException') (curl exit code 42).

Example: `builder.UseRateLimit(10)`, then a loop of 50 sequential calls takes about 5 seconds.

Retries and back-off guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/advanced.md

### See Also
- [UseRateLimit\(int\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRateLimit(int).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseRateLimit\(int\)')
- [RetryMiddleware](CurlDotNet.Middleware.RetryMiddleware.md 'CurlDotNet\.Middleware\.RetryMiddleware')

| Constructors | |
| :--- | :--- |
| [RateLimitMiddleware\(int\)](CurlDotNet.Middleware.RateLimitMiddleware.RateLimitMiddleware(int).md 'CurlDotNet\.Middleware\.RateLimitMiddleware\.RateLimitMiddleware\(int\)') | Creates a rate limiter that allows [requestsPerSecond](CurlDotNet.Middleware.RateLimitMiddleware.RateLimitMiddleware(int).md#CurlDotNet.Middleware.RateLimitMiddleware.RateLimitMiddleware(int).requestsPerSecond 'CurlDotNet\.Middleware\.RateLimitMiddleware\.RateLimitMiddleware\(int\)\.requestsPerSecond') requests to complete per second\. |

| Methods | |
| :--- | :--- |
| [ExecuteAsync\(CurlContext, Func&lt;Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.RateLimitMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.RateLimitMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Waits, if needed, until another request may run within the per\-second limit, then runs the rest of the pipeline and records when it finished \(also when it failed\)\. |
