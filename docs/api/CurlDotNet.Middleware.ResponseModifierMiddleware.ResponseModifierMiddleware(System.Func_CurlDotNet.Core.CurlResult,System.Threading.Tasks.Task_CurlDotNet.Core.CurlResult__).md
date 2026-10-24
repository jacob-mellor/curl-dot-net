#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[ResponseModifierMiddleware](CurlDotNet.Middleware.ResponseModifierMiddleware.md 'CurlDotNet\.Middleware\.ResponseModifierMiddleware')

## ResponseModifierMiddleware\(Func\<CurlResult,Task\<CurlResult\>\>\) Constructor

Creates the middleware with the function that turns each request's result into the result returned to the caller\.

```csharp
public ResponseModifierMiddleware(System.Func<CurlDotNet.Core.CurlResult,System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>> modifier);
```
#### Parameters

<a name='CurlDotNet.Middleware.ResponseModifierMiddleware.ResponseModifierMiddleware(System.Func_CurlDotNet.Core.CurlResult,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).modifier'></a>

`modifier` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

Receives the result and returns the result to hand back \(the same or a new one\); must not be null\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[modifier](CurlDotNet.Middleware.ResponseModifierMiddleware.ResponseModifierMiddleware(System.Func_CurlDotNet.Core.CurlResult,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md#CurlDotNet.Middleware.ResponseModifierMiddleware.ResponseModifierMiddleware(System.Func_CurlDotNet.Core.CurlResult,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).modifier 'CurlDotNet\.Middleware\.ResponseModifierMiddleware\.ResponseModifierMiddleware\(System\.Func\<CurlDotNet\.Core\.CurlResult,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)\.modifier') is null\. Fix: pass a function such as
            `r => Task.FromResult(r)`\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/guides/logging\-observability\.md

### Remarks

Example: `new ResponseModifierMiddleware(async r => { await metrics.RecordAsync(r.StatusCode); return r; })`

### See Also
- [RequestModifierMiddleware\(Action&lt;CurlContext&gt;\)](CurlDotNet.Middleware.RequestModifierMiddleware.RequestModifierMiddleware(System.Action_CurlDotNet.Middleware.CurlContext_).md 'CurlDotNet\.Middleware\.RequestModifierMiddleware\.RequestModifierMiddleware\(System\.Action\<CurlDotNet\.Middleware\.CurlContext\>\)')