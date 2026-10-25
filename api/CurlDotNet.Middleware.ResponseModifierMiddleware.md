#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## ResponseModifierMiddleware Class

Runs your asynchronous function on each [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') after the request, to inspect, change or replace
it \(for example to unwrap an envelope, redact a body or record metrics\)\.

```csharp
public class ResponseModifierMiddleware : CurlDotNet.Middleware.ICurlMiddleware
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ResponseModifierMiddleware

Implements [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')

### Remarks

The function receives the result from the rest of the pipeline and its return value is what the caller gets
            (return the same object to pass it through). It does not run when the request throws. Exceptions from the function
            propagate unchanged. Thread-safe if your function is.

Example: `builder.Use(new ResponseModifierMiddleware(r => { r.Body = r.Body?.Trim(); return Task.FromResult(r); }));`

### See Also
- [RequestModifierMiddleware](CurlDotNet.Middleware.RequestModifierMiddleware.md 'CurlDotNet\.Middleware\.RequestModifierMiddleware')
- [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

| Constructors | |
| :--- | :--- |
| [ResponseModifierMiddleware\(Func&lt;CurlResult,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.ResponseModifierMiddleware.ResponseModifierMiddleware(System.Func_CurlDotNet.Core.CurlResult,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.ResponseModifierMiddleware\.ResponseModifierMiddleware\(System\.Func\<CurlDotNet\.Core\.CurlResult,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Creates the middleware with the function that turns each request's result into the result returned to the caller\. |

| Methods | |
| :--- | :--- |
| [ExecuteAsync\(CurlContext, Func&lt;Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.ResponseModifierMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.ResponseModifierMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Runs the rest of the pipeline and returns what the modifier makes of its result\. |
