#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## TimingMiddleware Class

Records how long the rest of the pipeline took and writes it to [Total](CurlDotNet.Core.CurlTimings.Total.md 'CurlDotNet\.Core\.CurlTimings\.Total') on the result
and to `TimingStart`/`TimingEnd`/`TimingDuration` in [Properties](CurlDotNet.Middleware.CurlContext.Properties.md 'CurlDotNet\.Middleware\.CurlContext\.Properties')\.

```csharp
public class TimingMiddleware : CurlDotNet.Middleware.ICurlMiddleware
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; TimingMiddleware

Implements [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')

### Remarks

The measured time covers everything after this middleware: retries, rate-limit waits and the transfer,
            so it replaces the transfer's own [Total](CurlDotNet.Core.CurlTimings.Total.md 'CurlDotNet\.Core\.CurlTimings\.Total') (DNS/connect/TLS timings are kept). Put it
            first to time the whole logical request, or last to time just the transfer. Nothing is recorded when the
            request throws. Stateless and thread-safe.

Properties written: `TimingStart` and `TimingEnd` ([System\.DateTime](https://learn.microsoft.com/en-us/dotnet/api/system.datetime 'System\.DateTime'), UTC) and
            `TimingDuration` ([System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')); read them with `context.GetProperty<TimeSpan>("TimingDuration")`.

Example: `var result = await CurlMiddlewarePipeline.CreateBuilder().UseTiming().WithHandler(ctx => Curl.ExecuteAsync(ctx.Command)).Build().ExecuteAsync(ctx);`
            then `Console.WriteLine($"{result.Timings.Total:F0} ms");`.

Metrics guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md

### See Also
- [CurlTimings](CurlDotNet.Core.CurlTimings.md 'CurlDotNet\.Core\.CurlTimings')
- [UseTiming\(\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseTiming().md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseTiming\(\)')
- [LoggingMiddleware](CurlDotNet.Middleware.LoggingMiddleware.md 'CurlDotNet\.Middleware\.LoggingMiddleware')

| Methods | |
| :--- | :--- |
| [ExecuteAsync\(CurlContext, Func&lt;Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.TimingMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.TimingMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Notes the start time, runs the rest of the pipeline, then stores the elapsed time on the result and in the context properties\. |
