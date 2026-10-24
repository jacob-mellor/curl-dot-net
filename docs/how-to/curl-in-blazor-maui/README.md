---
title: "How to Use curl in Blazor and .NET MAUI (C#)"
description: "Use curl in Blazor and .NET MAUI: call CurlDotNet from a shared service in Blazor Server or MAUI, and route Blazor WebAssembly calls via your API."
---

# How to use curl in Blazor and .NET MAUI

To use curl in Blazor or .NET MAUI, put your CurlDotNet calls in a small service class, register it in `Program.cs` (Blazor) or `MauiProgram.cs` (MAUI), and inject it into components and pages. The CurlDotNet API is the same everywhere; what differs is where the code runs.

```bash
dotnet add package CurlDotNet
```

## A shared service

```csharp
using CurlDotNet;

public class QuoteService
{
    public async Task<string> GetRawAsync(CancellationToken ct = default)
    {
        var result = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/json", ct);
        return result.IsSuccess ? result.Body : $"Error {result.StatusCode}";
    }
}
```

The fluent builder works the same way:

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/json")
    .WithTimeout(TimeSpan.FromSeconds(10))
    .ExecuteAsync();
```

Register it with `builder.Services.AddSingleton<QuoteService>();` in either host.

## Use it from a Razor component

```razor
@inject QuoteService Quotes

<button @onclick="Load">Load</button>
<pre>@json</pre>

@code {
    private string? json;
    private async Task Load() => json = await Quotes.GetRawAsync();
}
```

The component is the same in Blazor Server and in a MAUI Blazor Hybrid app. Blazor WebAssembly has extra limits, described below.

## Blazor Server

The code runs on your server, so everything on the other how-to pages applies: proxies, `-k`, file output with `-o`, any URL your server can reach. Treat it like [ASP.NET Core](../curl-in-aspnet-core/README.md).

## Blazor WebAssembly: call through your server

In WebAssembly the code runs in the browser, where .NET's `HttpClient` is built on the browser's `fetch`. That brings browser rules:

- The target API must allow your origin through CORS.
- The browser controls TLS, redirects and proxies. On .NET 5 and later CurlDotNet builds a `SocketsHttpHandler` with its own connect callback (redirects and automatic decompression off, and for some flags a certificate callback, proxy or credentials), and a browser cannot open raw sockets, so that handler is not available there.
- `-o` would write to the WebAssembly virtual file system, not the user's disk.

CurlDotNet has not been tested in Blazor WebAssembly for this page, and creating that handler is expected to throw `PlatformNotSupportedException` there. The dependable pattern is to run CurlDotNet in your server API (ASP.NET Core) and have the WebAssembly client call that API with the injected `HttpClient`.

## .NET MAUI

MAUI apps run on .NET 8+ on Android, iOS, macOS and Windows, and CurlDotNet's net8.0 and later builds are pure managed code, so no native library has to ship with the app. Points to check:

- Android needs the `INTERNET` permission. The MAUI template includes it.
- iOS App Transport Security blocks plain `http://` URLs unless you add an exception.
- Use `FileSystem.AppDataDirectory` or `FileSystem.CacheDirectory` for `-o` paths. The current directory is not writable on mobile.
- Keep calls async and off the UI thread; never call `Curl.Execute` from an event handler.

The MAUI notes are platform guidance; CurlDotNet was not run on an Android or iOS device for this page.

## Related

- [Use CurlDotNet in ASP.NET Core](../curl-in-aspnet-core/README.md)
- [Timeouts and cancellation](../curl-timeout-csharp/README.md)
- [Download a file with curl -o](../curl-download-file-csharp/README.md)
- [Run curl synchronously (and when not to)](../curl-synchronously-in-csharp/README.md)
- [All how-to guides](../README.md)
