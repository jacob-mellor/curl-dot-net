#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## CachingMiddleware Class

Simple in\-memory caching middleware\.

```csharp
public class CachingMiddleware : CurlDotNet.Middleware.ICurlMiddleware
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CachingMiddleware

Implements [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')

| Constructors | |
| :--- | :--- |
| [CachingMiddleware\(Nullable&lt;TimeSpan&gt;\)](CurlDotNet.Middleware.CachingMiddleware.CachingMiddleware(System.Nullable_System.TimeSpan_).md 'CurlDotNet\.Middleware\.CachingMiddleware\.CachingMiddleware\(System\.Nullable\<System\.TimeSpan\>\)') | |

| Methods | |
| :--- | :--- |
| [ClearCache\(\)](CurlDotNet.Middleware.CachingMiddleware.ClearCache().md 'CurlDotNet\.Middleware\.CachingMiddleware\.ClearCache\(\)') | Clear the cache\. |
| [ExecuteAsync\(CurlContext, Func&lt;Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.CachingMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.CachingMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | |
