#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## RetryMiddleware Class

Middleware for retry logic with exponential backoff\.

```csharp
public class RetryMiddleware : CurlDotNet.Middleware.ICurlMiddleware
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; RetryMiddleware

Implements [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')

| Constructors | |
| :--- | :--- |
| [RetryMiddleware\(int, Nullable&lt;TimeSpan&gt;\)](CurlDotNet.Middleware.RetryMiddleware.RetryMiddleware(int,System.Nullable_System.TimeSpan_).md 'CurlDotNet\.Middleware\.RetryMiddleware\.RetryMiddleware\(int, System\.Nullable\<System\.TimeSpan\>\)') | |

| Methods | |
| :--- | :--- |
| [ExecuteAsync\(CurlContext, Func&lt;Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.RetryMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.RetryMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | |
