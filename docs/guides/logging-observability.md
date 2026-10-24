---
title: "Logging and Observability for curl in C# - CurlDotNet"
description: "Log and observe curl requests in C# with CurlDotNet: verbose output, traces, -w write-out metrics, timings and middleware."
---

# Logging & Observability with CurlDotNet

Reliable HTTP automation depends on rich diagnostics. This guide shows how to capture verbose traces, integrate with your logging framework, and emit metrics for production observability.

## 1. Built-in Verbose Output

When troubleshooting, start with curl’s native verbose mode:

```csharp
var response = await Curl.ExecuteAsync(@"
    curl -v https://api.example.com/users/42
      -H 'Accept: application/json'
");

Console.WriteLine(response.Body);      // the -v log (> request, < response lines), then the payload
Console.WriteLine(response.Headers);   // header dictionary
```

The `-v` flag mirrors the CLI, showing connection, TLS, and header exchanges. curl prints that log to stderr; CurlDotNet puts it at the start of `result.Body`. Use it sparingly in production because it may reveal secrets (`Authorization` headers appear in the log). `--trace-ascii FILE` writes a fuller dump to a file instead.

## 2. Structured Logging with Middleware

The built-in `LoggingMiddleware` (`CurlMiddlewarePipeline.CreateBuilder().UseLogging(line => logger.LogInformation(line))`) logs each command with its credentials masked: the values of `-u`/`-U`, `--oauth2-bearer`, `--pass`, `-b` cookie strings, the password in `--cert FILE:PASSWORD`, `Authorization`, `Proxy-Authorization` and `Cookie` headers, and passwords in URLs and proxy URLs appear as `***`. Other values, such as a password inside `-d` form data, are logged as written.

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

curl's own timing variables are available too: `-w '%{json}'` appends a JSON object with `time_namelookup`, `time_connect`, `time_starttransfer`, `time_total`, `http_code`, `size_download` and the rest to `result.Body`, which is handy to forward as-is. See [curl -w in C#](../cookbook/write-out.html).

For Prometheus-style scrapers, emit stats to stdout:

```
curl_request_duration_milliseconds{endpoint="/health",status="200"} 142
```

## 4. Capturing Request/Response Bodies Safely

Never log secrets. The library masks credentials in what it writes itself (`LoggingMiddleware`, the `Command:` line of a `--trace` file, and `CurlException.ToDetailedString()`, `ToJson()` and `GetDiagnosticInfo()`), but `ex.Command`, `result.Body` and the `--trace` wire dump hold the raw values. Use redaction helpers before writing payloads:

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
