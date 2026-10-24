#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## RetryMiddleware Class

Re\-runs the rest of the pipeline when a request fails transiently \(HTTP 5xx or 429 responses, DNS,
connect, timeout and network errors\), waiting longer before each attempt \(exponential back\-off\)\.

```csharp
public class RetryMiddleware : CurlDotNet.Middleware.ICurlMiddleware
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; RetryMiddleware

Implements [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')

### Remarks

Attempts: the request runs once, then up to `maxRetries` more times. The wait before retry
            `n` (1-based) is `initialDelay × 2^(n-1)`: with the defaults (3 retries, 1 second) that is 1 s, 2 s
            and 4 s. Waits honour [CancellationToken](CurlDotNet.Middleware.CurlContext.CancellationToken.md 'CurlDotNet\.Middleware\.CurlContext\.CancellationToken').

What is retried: a returned [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with status 500-599 or 429, and any
            [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') whose [IsRetryable\(\)](CurlDotNet.Exceptions.CurlException.IsRetryable().md 'CurlDotNet\.Exceptions\.CurlException\.IsRetryable\(\)') is true (exit codes 6, 7, 28, 52, 55, 56
            and `-f` HTTP errors 429/5xx). Every other exception, including your own and non-curl exceptions, is
            rethrown immediately. A `Retry-After` header is not read; the computed back-off is used.

When attempts run out: for a 5xx/429 response the last response is returned (check
            [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode')); for an exception, [CurlRetryException](CurlDotNet.Exceptions.CurlRetryException.md 'CurlDotNet\.Exceptions\.CurlRetryException') is thrown with the last
            failure as [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException') and [LastAttemptException](CurlDotNet.Exceptions.CurlRetryException.LastAttemptException.md 'CurlDotNet\.Exceptions\.CurlRetryException\.LastAttemptException').

Place it after logging/timing (so they see one logical request) and before
            [AuthenticationMiddleware](CurlDotNet.Middleware.AuthenticationMiddleware.md 'CurlDotNet\.Middleware\.AuthenticationMiddleware') (so each attempt gets a fresh token). curl's own `--retry N` option
            does the same inside a single command without middleware. The instance keeps no per-request state and is
            thread-safe.

Example: `builder.Use(new RetryMiddleware(maxRetries: 2, initialDelay: TimeSpan.FromMilliseconds(250)))`
            tries a request up to 3 times, waiting 0.25 s and then 0.5 s.

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/advanced.md

### See Also
- [UseRetry\(int, Nullable&lt;TimeSpan&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRetry(int,System.Nullable_System.TimeSpan_).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseRetry\(int, System\.Nullable\<System\.TimeSpan\>\)')
- [CurlRetryException](CurlDotNet.Exceptions.CurlRetryException.md 'CurlDotNet\.Exceptions\.CurlRetryException')
- [IsRetryable\(\)](CurlDotNet.Exceptions.CurlException.IsRetryable().md 'CurlDotNet\.Exceptions\.CurlException\.IsRetryable\(\)')

| Constructors | |
| :--- | :--- |
| [RetryMiddleware\(int, Nullable&lt;TimeSpan&gt;\)](CurlDotNet.Middleware.RetryMiddleware.RetryMiddleware(int,System.Nullable_System.TimeSpan_).md 'CurlDotNet\.Middleware\.RetryMiddleware\.RetryMiddleware\(int, System\.Nullable\<System\.TimeSpan\>\)') | Creates a retry middleware that allows [maxRetries](CurlDotNet.Middleware.RetryMiddleware.RetryMiddleware(int,System.Nullable_System.TimeSpan_).md#CurlDotNet.Middleware.RetryMiddleware.RetryMiddleware(int,System.Nullable_System.TimeSpan_).maxRetries 'CurlDotNet\.Middleware\.RetryMiddleware\.RetryMiddleware\(int, System\.Nullable\<System\.TimeSpan\>\)\.maxRetries') retries after the first attempt, waiting [initialDelay](CurlDotNet.Middleware.RetryMiddleware.RetryMiddleware(int,System.Nullable_System.TimeSpan_).md#CurlDotNet.Middleware.RetryMiddleware.RetryMiddleware(int,System.Nullable_System.TimeSpan_).initialDelay 'CurlDotNet\.Middleware\.RetryMiddleware\.RetryMiddleware\(int, System\.Nullable\<System\.TimeSpan\>\)\.initialDelay') before the first retry and doubling the wait each time\. |

| Methods | |
| :--- | :--- |
| [ExecuteAsync\(CurlContext, Func&lt;Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.RetryMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.RetryMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Runs the rest of the pipeline, retrying with exponential back\-off on 5xx/429 results and retryable curl exceptions, and returns the first acceptable result\. |
