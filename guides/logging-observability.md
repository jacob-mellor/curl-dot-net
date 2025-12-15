# Logging & Observability with CurlDotNet

Reliable HTTP automation depends on rich diagnostics. This guide shows how to capture verbose traces, integrate with your logging framework, and emit metrics for production observability.

## 1. Built-in Verbose Output

When troubleshooting, start with curl’s native verbose mode:

```csharp
var response = await Curl.ExecuteAsync(@"
    curl -v https://api.example.com/users/42
      -H 'Accept: application/json'
");

Console.WriteLine(response.Body);      // response payload
Console.WriteLine(response.Headers);   // header dictionary
```

The `-v` flag mirrors the CLI, showing DNS, TLS, and header exchanges. Use it sparingly in production because it may reveal secrets.

## 2. Structured Logging with Middleware

Wrap requests in a custom middleware to push structured logs to Serilog, Microsoft.Extensions.Logging, etc.

```csharp
public class SerilogMiddleware : ICurlMiddleware
{
    private readonly ILogger _logger;

    public SerilogMiddleware(ILogger logger) => _logger = logger;

    public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<Task<CurlResult>> next)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var result = await next();
            _logger.Information("Curl {Method} {Url} => {Status} in {Elapsed}ms",
                context.Options.Method ?? "GET",
                context.Options.Url,
                result.StatusCode,
                sw.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Curl {Url} failed after {Elapsed}ms", context.Options.Url, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
```

Register it:

```csharp
var pipeline = CurlMiddlewarePipeline.CreateBuilder()
    .Use(new SerilogMiddleware(logger))
    .WithHandler(ctx => Curl.ExecuteAsync(ctx.Command))
    .Build();
```

## 3. Exporting Metrics

Track latency and failure counts via `CurlResult.Timings`:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com/health");

metrics.Record("curl.request.duration", result.Timings.Total, new TagList
{
    { "endpoint", "/health" },
    { "status", result.StatusCode }
});
```

For Prometheus-style scrapers, emit stats to stdout:

```
curl_request_duration_milliseconds{endpoint="/health",status="200"} 142
```

## 4. Capturing Request/Response Bodies Safely

Never log secrets. Use redaction helpers before writing payloads:

```csharp
string Redact(string body) =>
    body.Replace("access_token", "****").Replace("password", "****");

logger.Debug("Response: {Body}", Redact(result.Body ?? string.Empty));
```

## 5. Integrating with Application Insights

```csharp
var telemetry = new DependencyTelemetry(
    dependencyTypeName: "HTTP",
    target: new Uri(result.Command ?? context.Options.Url).Host,
    dependencyName: context.Options.Method + " " + context.Options.Url,
    data: context.Command,
    startTime: context.StartTime,
    duration: TimeSpan.FromMilliseconds(result.Timings.Total),
    success: result.IsSuccess
);

telemetryClient.TrackDependency(telemetry);
```

## 6. Troubleshooting Checklist

1. **Enable verbose mode** only locally or behind secure storage.
2. **Add correlation IDs** via `.WithHeader("X-Correlation-Id", ...)`.
3. **Log retries** by tracking `settings.RetryCount` and failure reasons.
4. **Surface headers** such as `Request-Id`, `Retry-After`, and rate-limit counters.
5. **Capture curl commands** (from `result.Command`) for quick CLI reproduction.

## Next Steps

- Combine this guide with the [CI/CD Integration](../articles/ci-cd-integration.html) doc to stream logs straight into your pipeline console.
- Extend middleware to push telemetry to OpenTelemetry exporters for unified traces across services.
