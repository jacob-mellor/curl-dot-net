#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')

## CurlContext\.StartTime Property

When this context was created \(UTC\), useful for measuring how long the whole pipeline took,
including retries and rate\-limit waits\.

```csharp
public System.DateTime StartTime { get; set; }
```

#### Property Value
[System\.DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime 'System\.DateTime')  
[System\.DateTime\.UtcNow](https://learn.microsoft.com/en-us/dotnet/api/system.datetime.utcnow 'System\.DateTime\.UtcNow') at construction unless you set it; always in UTC\.

### Remarks

Example: `var elapsed = DateTime.UtcNow - context.StartTime;` after `await next()`.
            For the transfer alone, use [Timings](CurlDotNet.Core.CurlResult.Timings.md 'CurlDotNet\.Core\.CurlResult\.Timings').

### See Also
- [TimingMiddleware](CurlDotNet.Middleware.TimingMiddleware.md 'CurlDotNet\.Middleware\.TimingMiddleware')
- [CurlTimings](CurlDotNet.Core.CurlTimings.md 'CurlDotNet\.Core\.CurlTimings')