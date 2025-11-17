#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## ResponseModifierMiddleware Class

Middleware for modifying responses\.

```csharp
public class ResponseModifierMiddleware : CurlDotNet.Middleware.ICurlMiddleware
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ResponseModifierMiddleware

Implements [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')

| Constructors | |
| :--- | :--- |
| [ResponseModifierMiddleware\(Func&lt;CurlResult,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.ResponseModifierMiddleware.ResponseModifierMiddleware(System.Func_CurlDotNet.Core.CurlResult,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.ResponseModifierMiddleware\.ResponseModifierMiddleware\(System\.Func\<CurlDotNet\.Core\.CurlResult,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | |

| Methods | |
| :--- | :--- |
| [ExecuteAsync\(CurlContext, Func&lt;Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.ResponseModifierMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.ResponseModifierMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | |
