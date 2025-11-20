#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](index.md#CurlDotNet.Middleware 'CurlDotNet\.Middleware')

## ICurlMiddleware Interface

Interface for curl middleware/interceptors\.

```csharp
public interface ICurlMiddleware
```

Derived  
&#8627; [AuthenticationMiddleware](CurlDotNet.Middleware.AuthenticationMiddleware.md 'CurlDotNet\.Middleware\.AuthenticationMiddleware')  
&#8627; [CachingMiddleware](CurlDotNet.Middleware.CachingMiddleware.md 'CurlDotNet\.Middleware\.CachingMiddleware')  
&#8627; [LoggingMiddleware](CurlDotNet.Middleware.LoggingMiddleware.md 'CurlDotNet\.Middleware\.LoggingMiddleware')  
&#8627; [RateLimitMiddleware](CurlDotNet.Middleware.RateLimitMiddleware.md 'CurlDotNet\.Middleware\.RateLimitMiddleware')  
&#8627; [RequestModifierMiddleware](CurlDotNet.Middleware.RequestModifierMiddleware.md 'CurlDotNet\.Middleware\.RequestModifierMiddleware')  
&#8627; [ResponseModifierMiddleware](CurlDotNet.Middleware.ResponseModifierMiddleware.md 'CurlDotNet\.Middleware\.ResponseModifierMiddleware')  
&#8627; [RetryMiddleware](CurlDotNet.Middleware.RetryMiddleware.md 'CurlDotNet\.Middleware\.RetryMiddleware')  
&#8627; [TimingMiddleware](CurlDotNet.Middleware.TimingMiddleware.md 'CurlDotNet\.Middleware\.TimingMiddleware')

### Example

```csharp
public class LoggingMiddleware : ICurlMiddleware
{
    public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
    {
        Console.WriteLine($"Executing: {context.Command}");
        var result = await next();
        Console.WriteLine($"Status: {result.StatusCode}");
        return result;
    }
}
```

### Remarks

Middleware can modify requests before execution and responses after execution.

Multiple middleware can be chained together in a pipeline.

AI-Usage: Implement this interface to add cross-cutting concerns like logging, retry, caching, etc.
### Methods

<a name='CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__)'></a>

## ICurlMiddleware\.ExecuteAsync\(CurlContext, Func\<Task\<CurlResult\>\>\) Method

Execute the middleware logic\.

```csharp
System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(CurlDotNet.Middleware.CurlContext context, System.Func<System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>> next);
```
#### Parameters

<a name='CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).context'></a>

`context` [CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')

The curl context containing request information

<a name='CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).next'></a>

`next` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')

The next middleware in the pipeline

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The curl result