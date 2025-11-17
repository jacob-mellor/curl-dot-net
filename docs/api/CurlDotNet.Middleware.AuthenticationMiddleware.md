#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## AuthenticationMiddleware Class

Middleware for adding authentication headers\.

```csharp
public class AuthenticationMiddleware : CurlDotNet.Middleware.ICurlMiddleware
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; AuthenticationMiddleware

Implements [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')

| Constructors | |
| :--- | :--- |
| [AuthenticationMiddleware\(Func&lt;CurlContext,Task&lt;string&gt;&gt;\)](CurlDotNet.Middleware.AuthenticationMiddleware.AuthenticationMiddleware(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_string__).md 'CurlDotNet\.Middleware\.AuthenticationMiddleware\.AuthenticationMiddleware\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<string\>\>\)') | |

| Methods | |
| :--- | :--- |
| [ExecuteAsync\(CurlContext, Func&lt;Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.AuthenticationMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.AuthenticationMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | |
