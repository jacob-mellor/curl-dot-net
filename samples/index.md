---
layout: page
title: Examples & Samples
permalink: /samples/
---

# CurlDotNet Examples

## Basic Examples

### Simple GET Request
```csharp
using CurlDotNet;

var curl = new Curl();
var result = await curl.GetAsync("https://api.example.com/data");
Console.WriteLine(result.Body);
```

### POST with JSON
```csharp
var curl = new Curl();
var json = "{\"name\":\"John\",\"age\":30}";
var result = await curl.PostAsync("https://api.example.com/users", json);
```

### Headers and Authentication
```csharp
var curl = new Curl()
    .WithHeader("Authorization", "Bearer YOUR_TOKEN")
    .WithHeader("Accept", "application/json");

var result = await curl.GetAsync("https://api.example.com/protected");
```

## Advanced Examples

### Using Middleware
```csharp
var pipeline = new CurlMiddlewarePipelineBuilder()
    .Use<RetryMiddleware>(options => options.MaxRetries = 3)
    .Use<RateLimitMiddleware>(options => options.RequestsPerSecond = 10)
    .Use<CachingMiddleware>()
    .Build();

var curl = new Curl(pipeline);
```

### Error Handling
```csharp
try
{
    var result = await curl.GetAsync(url);
    Console.WriteLine($"Success: {result.StatusCode}");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Request timed out after {ex.TimeoutSeconds}s");
}
catch (CurlException ex)
{
    Console.WriteLine($"Curl error: {ex.Message}");
}
```

### File Downloads
```csharp
var curl = new Curl();
var fileData = await curl.DownloadAsync("https://example.com/file.pdf");
await File.WriteAllBytesAsync("downloaded.pdf", fileData);
```

### Form Data
```csharp
var formData = new Dictionary<string, string>
{
    ["username"] = "john",
    ["password"] = "secret"
};

var curl = new Curl();
var result = await curl.PostFormAsync("https://api.example.com/login", formData);
```
