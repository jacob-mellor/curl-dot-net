# Advanced CurlDotNet Features

Advanced capabilities live here so newcomers can stay focused on the copy/paste experience, while power users discover everything else available in CurlDotNet.

## Namespaces to Explore

- `CurlDotNet.Middleware` – Logging, retry, caching, rate limiting, and custom middleware pipelines.
- `CurlDotNet.Core.Handlers` – Extend or override protocol handlers (HTTP/HTTPS, FTP/FTPS, file URLs).
- `CurlDotNet.Core.CurlRequestBuilder` – Fluent builder with hooks for interception and request cloning.
- `CurlDotNet.Lib` – Object-oriented API inspired by libcurl with reusable clients.

## Middleware Examples

```csharp
using CurlDotNet.Middleware;

var pipeline = new CurlMiddlewarePipeline()
    .Use(new LoggingMiddleware(options => options.LogHeaders = true))
    .Use(new RetryMiddleware(maxRetries: 5, jitter: TimeSpan.FromMilliseconds(150)))
    .Use(new RateLimitingMiddleware(TimeSpan.FromSeconds(1)));

var result = await pipeline.ExecuteAsync(options, cancellationToken);
```

## Extending Protocol Handlers

```csharp
public sealed class CustomHttpHandler : HttpHandler
{
    protected override HttpRequestMessage CreateRequest(CurlOptions options)
    {
        var request = base.CreateRequest(options);
        request.Headers.Add("X-Trace-Id", Guid.NewGuid().ToString("N"));
        return request;
    }
}
```

Register with the engine:

```csharp
var engine = new CurlEngine(new CustomHttpHandler(), new FtpHandler(), new FileHandler());
```

## Builder Hooks

```csharp
var builder = CurlRequestBuilder
    .Post("https://api.example.com/ingest")
    .WithJson(payload)
    .Configure(opts =>
    {
        opts.Timeout = TimeSpan.FromSeconds(10);
        opts.Headers["X-Correlation-Id"] = correlationId;
    });
```

## LibCurl Fluent Client

```csharp
var client = new LibCurl()
    .WithUserAgent("UserlandNet/1.0")
    .WithProxy("http://proxy.internal:8080")
    .WithVerbose(true);

var data = await client.GetAsync("https://internal-api.local/data");
```

## Guidelines

- Keep advanced APIs in their namespaces so junior developers see only the basics by default.
- Document every public method with XML `<example>` blocks so IntelliSense teaches the feature.
- Reference this file from the main README under an **Advanced** section.

