---
layout: page
title: "Examples & Samples"
description: "CurlDotNet examples and samples: curl commands in C# with the static Curl API, the request builder, LibCurl, middleware and exception handling."
permalink: /samples/
---

# CurlDotNet Examples

## Basic Examples

### Simple GET Request
```csharp
using CurlDotNet;

// Curl is a static class: nothing to construct or dispose
var result = await Curl.ExecuteAsync("curl https://api.example.com/data");
Console.WriteLine(result.Body);

// Shorthand for a plain GET
var same = await Curl.GetAsync("https://api.example.com/data");
```

### POST with JSON
```csharp
// Serialize an object and POST it with Content-Type: application/json
var result = await Curl.PostJsonAsync("https://api.example.com/users", new { name = "John", age = 30 });

// Or the curl way: --json sets Content-Type and Accept for you
var result2 = await Curl.ExecuteAsync(
    @"curl --json '{""name"":""John"",""age"":30}' https://api.example.com/users");
```

### Headers and Authentication
```csharp
using CurlDotNet.Lib;

// LibCurl keeps headers and auth across calls
using var curl = new LibCurl()
    .WithBearerToken("YOUR_TOKEN")
    .WithHeader("Accept", "application/json");

var result = await curl.GetAsync("https://api.example.com/protected");
```

## Advanced Examples

### Using Middleware
```csharp
using CurlDotNet.Middleware;

var pipeline = CurlMiddlewarePipeline.CreateBuilder()
    .UseRetry(maxRetries: 3)
    .UseRateLimit(requestsPerSecond: 10)
    .UseCaching()
    .WithHandler(ctx => Curl.ExecuteAsync(ctx.Command, ctx.CancellationToken))
    .Build();

var result = await pipeline.ExecuteAsync(new CurlContext { Command = "curl https://api.example.com/data" });
```

For plain retries you do not need middleware: `curl --retry 3 ...` retries timeouts and HTTP 408/429/500/502/503/504 like curl does. See [curl --retry in C#](../cookbook/retry.html).

### Error Handling
```csharp
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync($"curl -m 10 {url}");   // no timeout unless you set one
    Console.WriteLine(result.IsSuccess ? $"Success: {result.StatusCode}" : $"HTTP error: {result.StatusCode}");
}
catch (CurlOperationTimeoutException ex)
{
    Console.WriteLine($"Request timed out after {ex.Timeout}");
}
catch (CurlException ex)
{
    Console.WriteLine($"Curl error {ex.CurlErrorCode}: {ex.Message}");
}
```

HTTP 4xx/5xx responses do not throw unless you add `-f`; a cancelled `CancellationToken` throws the standard `OperationCanceledException`.

### File Downloads
```csharp
// Straight to disk, like curl -o (result.Body stays empty)
await Curl.DownloadAsync("https://example.com/file.pdf", "downloaded.pdf");

// Or keep the bytes in memory
var result = await Curl.GetAsync("https://example.com/file.pdf");
await File.WriteAllBytesAsync("downloaded.pdf", result.BinaryData);
```

### Form Data
```csharp
var formData = new Dictionary<string, string>
{
    ["username"] = "john",
    ["password"] = "secret"
};

using CurlDotNet.Core;

var result = await CurlRequestBuilder.Post("https://api.example.com/login")
    .WithFormData(formData)   // URL-encodes each key and value
    .ExecuteAsync();

// Or the curl way
var result2 = await Curl.ExecuteAsync(
    "curl --data-urlencode username=john --data-urlencode password=secret https://api.example.com/login");
```
