---
title: "How to Use curl in ASP.NET Core (C#)"
description: "Use curl in ASP.NET Core: register CurlDotNet's LibCurl in dependency injection and call it from minimal APIs or controllers, with cancellation."
---

# How to use curl in ASP.NET Core

To use curl in ASP.NET Core, add the `CurlDotNet` package, register a configured `LibCurl` instance as a singleton in `Program.cs`, and inject it into your minimal API handlers or controllers. For one-off calls, the static `Curl.ExecuteAsync` works without any registration.

```bash
dotnet add package CurlDotNet
```

## Register LibCurl as a singleton

`LibCurl` owns one `HttpClient`, so a single shared instance reuses connections across requests. Configure its defaults once, at startup:

```csharp
using CurlDotNet.Lib;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(sp => new LibCurl()
    .WithBearerToken(builder.Configuration["Upstream:Token"] ?? "")
    .WithUserAgent("MyService/1.0")
    .WithTimeout(TimeSpan.FromSeconds(10)));

var app = builder.Build();
```

The `With...` methods change the instance's shared defaults. Call them only during registration, never inside a request handler, or concurrent requests will see each other's settings. Per-request options go in the `Action<CurlOptions>` argument instead (see below). The container disposes the singleton at shutdown.

## Minimal API

```csharp
using CurlDotNet.Lib;

var app = WebApplication.CreateBuilder(args).Build();
using var curl = new LibCurl();

app.MapGet("/weather/{city}", async (string city, HttpContext ctx) =>
{
    var url = $"https://wttr.in/{Uri.EscapeDataString(city)}?format=j1";
    var result = await curl.GetAsync(url, cancellationToken: ctx.RequestAborted);
    return result.IsSuccess
        ? Results.Content(result.Body, "application/json")
        : Results.StatusCode(502);
});

app.Run();
```

In a real app, take `LibCurl curl` as a handler parameter so it comes from the container.

## Controller

```csharp
using CurlDotNet.Lib;
using Microsoft.AspNetCore.Mvc;

[ApiController, Route("api/[controller]")]
public class ReposController(LibCurl curl) : ControllerBase
{
    [HttpGet("{owner}/{repo}")]
    public async Task<IActionResult> Get(string owner, string repo, CancellationToken ct)
    {
        var url = $"https://api.github.com/repos/{Uri.EscapeDataString(owner)}/{Uri.EscapeDataString(repo)}";
        var result = await curl.GetAsync(url, o => o.Headers["Accept"] = "application/vnd.github+json", ct);
        return StatusCode(result.StatusCode, result.Body);
    }
}
```

The lambda adds a header to this call only. `LibCurl` merges its defaults into a fresh `CurlOptions` for each call, so per-call changes do not leak.

## Using curl strings in a service

Running a copied curl command is fine for fixed requests. Never paste user input into a command string: a value such as `x -o /app/appsettings.json` would be read as extra curl flags. Use `LibCurl` or `CurlRequestBuilder` for anything user-controlled:

```csharp
using CurlDotNet.Core;

static Task<CurlResult> Search(string userQuery, CancellationToken ct) =>
    CurlRequestBuilder
        .Get($"https://httpbin.org/get?q={Uri.EscapeDataString(userQuery)}")
        .WithTimeout(TimeSpan.FromSeconds(5))
        .ExecuteAsync(ct);
```

## Notes

- Always pass `HttpContext.RequestAborted` (or the action's `CancellationToken`) so abandoned requests stop calling upstream.
- Set a timeout shorter than your own response budget; see [timeouts](../curl-timeout-csharp/README.md).
- Do not use the synchronous `Curl.Execute` in request handlers; it blocks a thread-pool thread.
- CurlDotNet has no `AddCurl()` extension; register `LibCurl` or your own wrapper type as shown.

## Related

- [Blazor and .NET MAUI](../curl-in-blazor-maui/README.md)
- [Azure Functions and AWS Lambda](../curl-in-azure-functions-aws-lambda/README.md)
- [Error handling](../curl-error-handling-csharp/README.md)
- [CurlDotNet vs HttpClient](../curl-vs-httpclient-csharp/README.md)
- [All how-to guides](../README.md)
