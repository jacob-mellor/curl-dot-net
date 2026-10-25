#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CachingMiddleware](CurlDotNet.Middleware.CachingMiddleware.md 'CurlDotNet\.Middleware\.CachingMiddleware')

## CachingMiddleware\.ClearCache\(\) Method

Removes every cached result for all [CachingMiddleware](CurlDotNet.Middleware.CachingMiddleware.md 'CurlDotNet\.Middleware\.CachingMiddleware') instances in the process, so the next
request of each kind is sent again\.

```csharp
public static void ClearCache();
```

### Remarks

Thread-safe; requests running at the same time may still store their results afterwards. Call it in test
            set-up, or after a write that makes cached GET results stale.

Example: `await Curl.ExecuteAsync("curl -X PUT -d @config.json https://api.example.com/config"); CachingMiddleware.ClearCache();`

### See Also
- [CachingMiddleware](CurlDotNet.Middleware.CachingMiddleware.md 'CurlDotNet\.Middleware\.CachingMiddleware')