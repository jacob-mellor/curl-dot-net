# CurlDotNet API Guide

Complete reference guide for all CurlDotNet APIs and features.

## 📚 API Overview

CurlDotNet provides three different APIs to suit your needs:

1. **[String API](#string-api)** - Execute curl commands as strings
2. **[Builder API](#builder-api)** - Type-safe fluent interface
3. **[LibCurl API](#libcurl-api)** - Object-oriented wrapper

## String API

The simplest way to use CurlDotNet - just paste curl commands as strings.

### Basic Usage

```csharp
using CurlDotNet;

// Simple GET request
var result = await Curl.ExecuteAsync("curl https://api.github.com");

// Check if successful
if (result.IsSuccess)
{
    Console.WriteLine(result.Content);
}
```

### With Headers

```csharp
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com/users \
    -H 'Authorization: Bearer token123' \
    -H 'Accept: application/json'
");
```

### POST with JSON

```csharp
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/users \
    -H 'Content-Type: application/json' \
    -d '{""name"": ""John"", ""email"": ""john@example.com""}'
");
```

### File Upload

```csharp
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/upload \
    -F 'file=@/path/to/file.pdf' \
    -F 'description=My PDF file'
");
```

### Download File

```csharp
var result = await Curl.ExecuteAsync("curl -o output.zip https://example.com/file.zip");
```

## Builder API

Type-safe, IntelliSense-friendly API for building requests programmatically.

### Basic Usage

```csharp
using CurlDotNet.Core;

var result = await new CurlRequestBuilder()
    .WithUrl("https://api.example.com")
    .WithMethod("GET")
    .ExecuteAsync();
```

### With Headers

```csharp
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.example.com/users")
    .WithHeader("Authorization", "Bearer token123")
    .WithHeader("Accept", "application/json")
    .ExecuteAsync();
```

### POST with JSON

```csharp
var user = new { name = "John", email = "john@example.com" };

var result = await new CurlRequestBuilder()
    .WithUrl("https://api.example.com/users")
    .WithMethod("POST")
    .WithJsonBody(user)
    .ExecuteAsync();
```

### Form Data

```csharp
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.example.com/login")
    .WithMethod("POST")
    .WithFormData("username", "john")
    .WithFormData("password", "secret123")
    .ExecuteAsync();
```

### Timeout and Retry

```csharp
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.example.com")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithRetry(3)
    .ExecuteAsync();
```

## LibCurl API

Object-oriented API for advanced scenarios and connection reuse.

### Basic Usage

```csharp
using CurlDotNet.Lib;

var curl = new LibCurl();
var result = await curl.GetAsync("https://api.example.com");
```

### Configuration

```csharp
var curl = new LibCurl()
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithMaxRedirects(5)
    .WithUserAgent("MyApp/1.0");
```

### Authentication

```csharp
// Bearer token
var curl = new LibCurl()
    .WithBearerToken("token123");

// Basic auth
var curl = new LibCurl()
    .WithBasicAuth("username", "password");

// API key
var curl = new LibCurl()
    .WithHeader("X-API-Key", "key123");
```

### Connection Reuse

```csharp
var curl = new LibCurl();

// Make multiple requests with the same connection
for (int i = 1; i <= 10; i++)
{
    var result = await curl.GetAsync($"https://api.example.com/item/{i}");
    Console.WriteLine($"Item {i}: {result.Content}");
}
```

### Advanced Configuration

```csharp
var curl = new LibCurl()
    .WithProxy("http://proxy.company.com:8080")
    .WithProxyAuth("proxyuser", "proxypass")
    .WithCookieJar("/tmp/cookies.txt")
    .WithReferer("https://example.com")
    .WithFollowRedirects(true)
    .WithVerifySsl(false); // Development only!
```

## Response Handling

### CurlResult Object

All APIs return a `CurlResult` object with these properties:

```csharp
public class CurlResult
{
    public bool IsSuccess { get; }
    public string Content { get; }
    public int StatusCode { get; }
    public Dictionary<string, string> Headers { get; }
    public string Error { get; }
    public TimeSpan ElapsedTime { get; }
    public string VerboseOutput { get; }
}
```

### JSON Parsing

```csharp
// Parse to dynamic
var result = await Curl.ExecuteAsync("curl https://api.example.com/user/1");
dynamic user = result.ParseJson();
Console.WriteLine(user.name);

// Parse to strongly typed
public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
}

var user = result.ParseJson<User>();
```

### Error Handling

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
    if (!result.IsSuccess)
    {
        Console.WriteLine($"Request failed: {result.Error}");
        Console.WriteLine($"Status code: {result.StatusCode}");
    }
}
catch (CurlException ex)
{
    Console.WriteLine($"Curl error: {ex.Message}");
}
```

## Middleware

Add cross-cutting concerns with middleware.

### Built-in Middleware

```csharp
var curl = new LibCurl()
    .UseLogging()
    .UseRetry(3)
    .UseRateLimiting(100) // 100 requests per minute
    .UseCaching(TimeSpan.FromMinutes(5));
```

### Custom Middleware

```csharp
public class TimingMiddleware : ICurlMiddleware
{
    public async Task<CurlResult> ExecuteAsync(
        CurlContext context,
        Func<CurlContext, Task<CurlResult>> next)
    {
        var sw = Stopwatch.StartNew();
        var result = await next(context);
        Console.WriteLine($"Request took {sw.ElapsedMilliseconds}ms");
        return result;
    }
}

// Use it
var curl = new LibCurl()
    .UseMiddleware(new TimingMiddleware());
```

## Async/Sync Operations

### Async (Recommended)

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");
```

### Sync

```csharp
var result = Curl.Execute("curl https://api.example.com");
```

## Common Patterns

### API Client Pattern

```csharp
public class GitHubClient
{
    private readonly LibCurl _curl;

    public GitHubClient(string token)
    {
        _curl = new LibCurl()
            .WithBearerToken(token)
            .WithHeader("Accept", "application/vnd.github.v3+json")
            .WithUserAgent("MyApp/1.0");
    }

    public async Task<User> GetUserAsync(string username)
    {
        var result = await _curl.GetAsync($"https://api.github.com/users/{username}");
        return result.ParseJson<User>();
    }

    public async Task<List<Repo>> GetReposAsync(string username)
    {
        var result = await _curl.GetAsync($"https://api.github.com/users/{username}/repos");
        return result.ParseJson<List<Repo>>();
    }
}
```

### Retry with Exponential Backoff

```csharp
public static async Task<CurlResult> ExecuteWithRetryAsync(string command, int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        var result = await Curl.ExecuteAsync(command);
        if (result.IsSuccess) return result;

        if (i < maxRetries - 1)
        {
            var delay = TimeSpan.FromSeconds(Math.Pow(2, i));
            await Task.Delay(delay);
        }
    }

    throw new Exception("Max retries exceeded");
}
```

### Parallel Requests

```csharp
var urls = new[] { "url1", "url2", "url3", "url4", "url5" };

var tasks = urls.Select(url =>
    Curl.ExecuteAsync($"curl {url}")
).ToArray();

var results = await Task.WhenAll(tasks);
```

## Performance Tips

1. **Reuse Connections**: Use `LibCurl` for multiple requests
2. **Enable Compression**: Add `--compressed` flag
3. **Use HTTP/2**: Add `--http2` flag
4. **Connection Pooling**: Configure `MaxConnections`
5. **Async Operations**: Always prefer async over sync

## Advanced Features

### Custom HTTP Methods

```csharp
var result = await Curl.ExecuteAsync("curl -X PATCH https://api.example.com/resource");
```

### Custom Headers

```csharp
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.example.com")
    .WithHeader("X-Custom-Header", "value")
    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
    .ExecuteAsync();
```

### Cookie Management

```csharp
// Save cookies
var result = await Curl.ExecuteAsync("curl -c cookies.txt https://example.com/login");

// Use saved cookies
var result = await Curl.ExecuteAsync("curl -b cookies.txt https://example.com/profile");
```

### Client Certificates

```csharp
var result = await Curl.ExecuteAsync(@"
    curl --cert /path/to/client.pem \
         --key /path/to/client.key \
         https://secure-api.example.com
");
```

## API Reference Links

- [Curl Class](/api/CurlDotNet.Curl.md)
- [CurlRequestBuilder Class](/api/CurlDotNet.Core.CurlRequestBuilder.md)
- [LibCurl Class](/api/CurlDotNet.Lib.LibCurl.md)
- [CurlResult Class](/api/CurlDotNet.Core.CurlResult.md)
- [Exception Types](/exceptions/)

## Related Pages

- [Getting Started](/getting-started/)
- [Tutorials](/tutorials/)
- [Cookbook](/cookbook/)
- [Troubleshooting](/troubleshooting/)

---
*Complete API documentation for CurlDotNet*