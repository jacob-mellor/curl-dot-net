#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[RetryMiddleware](CurlDotNet.Middleware.RetryMiddleware.md 'CurlDotNet\.Middleware\.RetryMiddleware')

## RetryMiddleware\.ExecuteAsync\(CurlContext, Func\<Task\<CurlResult\>\>\) Method

Runs the rest of the pipeline, retrying with exponential back\-off on 5xx/429 results and retryable curl
exceptions, and returns the first acceptable result\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(CurlDotNet.Middleware.CurlContext context, System.Func<System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>> next);
```
#### Parameters

<a name='CurlDotNet.Middleware.RetryMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).context'></a>

`context` [CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')

The request; its [CancellationToken](CurlDotNet.Middleware.CurlContext.CancellationToken.md 'CurlDotNet\.Middleware\.CurlContext\.CancellationToken') stops the waits\.

<a name='CurlDotNet.Middleware.RetryMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).next'></a>

`next` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')

Runs the rest of the pipeline; called up to `maxRetries + 1` times\.

Implements [ExecuteAsync\(CurlContext, Func&lt;Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.ICurlMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The first result that is not 5xx/429, or the last result when every attempt returned 5xx/429\.

#### Exceptions

[CurlRetryException](CurlDotNet.Exceptions.CurlRetryException.md 'CurlDotNet\.Exceptions\.CurlRetryException')  
Every attempt threw a retryable [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException'); the message names
            the URL, the number of attempts and the last error, and [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException') is that error\.
            Fix: address the last error \(see its own message\) or allow more or longer retries\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

[CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException')  
The token was cancelled during a back\-off wait \(curl exit code 42\)\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/tutorials/12\-cancellation\-tokens\.md

[System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')  
Any non\-retryable exception from [next](CurlDotNet.Middleware.RetryMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md#CurlDotNet.Middleware.RetryMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).next 'CurlDotNet\.Middleware\.RetryMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)\.next'), rethrown on the first occurrence\.

### Remarks

Each retry calls [next](CurlDotNet.Middleware.RetryMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md#CurlDotNet.Middleware.RetryMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).next 'CurlDotNet\.Middleware\.RetryMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)\.next') again with the same [context](CurlDotNet.Middleware.RetryMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md#CurlDotNet.Middleware.RetryMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).context 'CurlDotNet\.Middleware\.RetryMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)\.context'), so changes later
            middleware made to it during an attempt are still there on the next attempt.

Example: a handler that throws [CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException') twice and then returns 200 gives
            200 after two waits.

### See Also
- [IsRetryable\(\)](CurlDotNet.Exceptions.CurlException.IsRetryable().md 'CurlDotNet\.Exceptions\.CurlException\.IsRetryable\(\)')