#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## CachingMiddleware Class

Answers repeated GET requests from an in\-memory cache for a fixed time \(5 minutes by default\) instead of
sending them again; only successful \(2xx\) results are stored\.

```csharp
public class CachingMiddleware : CurlDotNet.Middleware.ICurlMiddleware
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CachingMiddleware

Implements [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')

### Remarks

Cache key: [Command](CurlDotNet.Middleware.CurlContext.Command.md 'CurlDotNet\.Middleware\.CurlContext\.Command'), or [Url](CurlDotNet.Core.CurlOptions.Url.md 'CurlDotNet\.Core\.CurlOptions\.Url') when there is no command, so two
            commands that differ only in header order are cached separately. Requests whose
            [Method](CurlDotNet.Core.CurlOptions.Method.md 'CurlDotNet\.Core\.CurlOptions\.Method') is set to anything other than `GET` bypass the cache; requests without
            options (command only) are treated as GET, so do not use it in front of `-X POST`/`-d` commands
            unless the context also carries options.

Scope: the cache is a static, process-wide [System\.Collections\.Concurrent\.ConcurrentDictionary&lt;&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentdictionary-2 'System\.Collections\.Concurrent\.ConcurrentDictionary\`2') shared by every
            [CachingMiddleware](CurlDotNet.Middleware.CachingMiddleware.md 'CurlDotNet\.Middleware\.CachingMiddleware') instance and every pipeline; each instance only decides the time-to-live of the
            entries it stores. Expired entries are skipped on lookup and swept when more than 100 entries exist. Clear it with
            [ClearCache\(\)](CurlDotNet.Middleware.CachingMiddleware.ClearCache().md 'CurlDotNet\.Middleware\.CachingMiddleware\.ClearCache\(\)') (for example between tests). A cache hit returns a copy of the stored result with the same
            status, body, headers and command; [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') and [Timings](CurlDotNet.Core.CurlResult.Timings.md 'CurlDotNet\.Core\.CurlResult\.Timings') are
            shared with the cached instance, so do not modify them. Thread-safe.

Example: `CurlMiddlewarePipeline.CreateBuilder().UseCaching(TimeSpan.FromSeconds(30)).WithHandler(ctx => Curl.ExecuteAsync(ctx.Command)).Build()`;
            calling it twice with `curl https://api.example.com/config` within 30 seconds sends one request.

### See Also
- [UseCaching\(Nullable&lt;TimeSpan&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseCaching(System.Nullable_System.TimeSpan_).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseCaching\(System\.Nullable\<System\.TimeSpan\>\)')
- [ClearCache\(\)](CurlDotNet.Middleware.CachingMiddleware.ClearCache().md 'CurlDotNet\.Middleware\.CachingMiddleware\.ClearCache\(\)')

| Constructors | |
| :--- | :--- |
| [CachingMiddleware\(Nullable&lt;TimeSpan&gt;\)](CurlDotNet.Middleware.CachingMiddleware.CachingMiddleware(System.Nullable_System.TimeSpan_).md 'CurlDotNet\.Middleware\.CachingMiddleware\.CachingMiddleware\(System\.Nullable\<System\.TimeSpan\>\)') | Creates a caching middleware whose stored results are reused for [ttl](CurlDotNet.Middleware.CachingMiddleware.CachingMiddleware(System.Nullable_System.TimeSpan_).md#CurlDotNet.Middleware.CachingMiddleware.CachingMiddleware(System.Nullable_System.TimeSpan_).ttl 'CurlDotNet\.Middleware\.CachingMiddleware\.CachingMiddleware\(System\.Nullable\<System\.TimeSpan\>\)\.ttl') after they were fetched \(5 minutes when no value is given\)\. |

| Methods | |
| :--- | :--- |
| [ClearCache\(\)](CurlDotNet.Middleware.CachingMiddleware.ClearCache().md 'CurlDotNet\.Middleware\.CachingMiddleware\.ClearCache\(\)') | Removes every cached result for all [CachingMiddleware](CurlDotNet.Middleware.CachingMiddleware.md 'CurlDotNet\.Middleware\.CachingMiddleware') instances in the process, so the next request of each kind is sent again\. |
| [ExecuteAsync\(CurlContext, Func&lt;Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.CachingMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.CachingMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Returns a cached copy when an unexpired result exists for this request; otherwise runs the rest of the pipeline and stores the result if it is a 2xx\. |
