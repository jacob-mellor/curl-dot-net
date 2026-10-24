---
title: "CurlDotNet Usage Guide - curl Commands in C#"
description: "CurlDotNet usage guide: run curl commands in C#, use the request builder and LibCurl, send data, upload and download files, and handle errors."
---

# CurlDotNet Usage Guide

This comprehensive guide covers common usage scenarios for CurlDotNet, from basic requests to advanced patterns.

## Table of Contents

1. [Getting Started](#getting-started)
2. [Basic HTTP Operations](#basic-http-operations)
3. [Working with JSON](#working-with-json)
4. [Authentication](#authentication)
5. [File Operations](#file-operations)
6. [Advanced Patterns](#advanced-patterns)
7. [Error Handling](#error-handling)
8. [Performance Optimization](#performance-optimization)
9. [Testing and Mocking](#testing-and-mocking)
10. [Troubleshooting](#troubleshooting)

## Getting Started

### Installation

```bash
dotnet add package CurlDotNet
```

### Basic Setup

```csharp
using CurlDotNet;
using CurlDotNet.Core;

// Simplest usage - direct curl command
var result = await Curl.ExecuteAsync("curl https://api.example.com");

// Using convenience methods
var result = await Curl.GetAsync("https://api.example.com");
```

## Basic HTTP Operations

### GET Requests

```csharp
// Simple GET
var result = await Curl.GetAsync("https://api.example.com/users");

// GET with query parameters
var result = await Curl.GetAsync("https://api.example.com/users?page=1&limit=10");

// GET with headers
var result = await CurlRequestBuilder.Get("https://api.example.com/users")
    .WithHeader("Accept", "application/json")
    .WithHeader("X-API-Version", "2.0")
    .ExecuteAsync();

// The same as a curl command
var same = await Curl.ExecuteAsync(
    "curl -H 'Accept: application/json' -H 'X-API-Version: 2.0' https://api.example.com/users");
```

Every request carries `User-Agent: curl/8.7.1` and `Accept: */*` unless you set your own, exactly like curl.

### POST Requests

```csharp
// POST with JSON body: serialize an object...
var result = await Curl.PostJsonAsync("https://api.example.com/users",
    new { name = "John Doe", email = "john@example.com" });

// ...or send JSON text with curl's --json flag (sets Content-Type and Accept)
var result2 = await Curl.ExecuteAsync(
    @"curl --json '{""name"":""John Doe"",""email"":""john@example.com""}' https://api.example.com/users");

// POST with form data (application/x-www-form-urlencoded)
var result3 = await Curl.PostAsync("https://api.example.com/login", "username=john&password=secret");

// POST with custom headers
var result4 = await CurlRequestBuilder.Post("https://api.example.com/users")
    .WithJson(new { name = "John Doe" })
    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
    .ExecuteAsync();
```

### PUT Requests

```csharp
// PUT to update resource
var result = await CurlRequestBuilder.Put("https://api.example.com/users/123")
    .WithJson(new { name = "Jane Doe", email = "jane@example.com" })
    .ExecuteAsync();

// PUT a file's bytes, as curl -T does (the file name is appended when the URL ends in /)
var upload = await Curl.ExecuteAsync(
    "curl -T document.pdf -H 'Content-Type: application/pdf' https://api.example.com/files/");
```

### DELETE Requests

```csharp
// Simple DELETE
var result = await Curl.ExecuteAsync("curl -X DELETE https://api.example.com/users/123");

// DELETE with confirmation header
var result2 = await CurlRequestBuilder.Delete("https://api.example.com/users/123")
    .WithHeader("X-Confirm-Delete", "true")
    .ExecuteAsync();
```

### PATCH Requests

```csharp
// PATCH for partial updates
var patchData = "[{\"op\":\"replace\",\"path\":\"/email\",\"value\":\"new@example.com\"}]";
var result = await CurlRequestBuilder.Patch("https://api.example.com/users/123")
    .WithData(patchData)
    .WithHeader("Content-Type", "application/json-patch+json")
    .ExecuteAsync();
```

## Working with JSON

### Parsing JSON Responses

```csharp
using System.Text.Json;

// Basic JSON parsing
var result = await Curl.GetAsync("https://api.example.com/user/123");
var json = JsonDocument.Parse(result.Body);
var userName = json.RootElement.GetProperty("name").GetString();

// Deserialize to strongly-typed objects
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

var result2 = await Curl.GetAsync("https://api.example.com/user/123");
var user = result2.ParseJson<User>();   // case-insensitive: "name" binds to Name
```

`ParseJson<T>()` matches property names case-insensitively (camelCase JSON binds to PascalCase properties); snake_case keys still need `[JsonPropertyName]`. It throws `InvalidOperationException` for an empty or malformed body.

### Sending JSON Data

```csharp
// Using anonymous objects
var data = new
{
    name = "John Doe",
    email = "john@example.com",
    age = 30
};

var result = await Curl.PostJsonAsync("https://api.example.com/users", data);

// Using strongly-typed objects
var user = new User { Name = "Jane Doe", Email = "jane@example.com" };
var result2 = await Curl.PostJsonAsync("https://api.example.com/users", user);
```

### Working with JSON Arrays

```csharp
// Parse JSON array
var result = await Curl.GetAsync("https://api.example.com/users");
var users = result.ParseJson<List<User>>();

foreach (var user in users)
{
    Console.WriteLine($"{user.Name} - {user.Email}");
}

// Send JSON array
var newUsers = new[]
{
    new User { Name = "User1", Email = "user1@example.com" },
    new User { Name = "User2", Email = "user2@example.com" }
};

var bulk = await Curl.PostJsonAsync("https://api.example.com/users/bulk", newUsers);
```

## Authentication

### Basic Authentication

```csharp
// Using curl command syntax
var result = await Curl.ExecuteAsync("curl -u username:password https://api.example.com/secure");

// Using builder
var result2 = await CurlRequestBuilder.Get("https://api.example.com/secure")
    .WithBasicAuth("username", "password")
    .ExecuteAsync();

// Using Authorization header
var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("username:password"));
var result3 = await CurlRequestBuilder.Get("https://api.example.com/secure")
    .WithHeader("Authorization", $"Basic {credentials}")
    .ExecuteAsync();

// Digest, NTLM and --anyauth are negotiated in-library, like curl
var digest = await Curl.ExecuteAsync("curl --digest -u username:password https://api.example.com/secure");

// Credentials from ~/.netrc instead of the command line
var netrc = await Curl.ExecuteAsync("curl -n https://api.example.com/secure");
```

A wrong password comes back as a normal result with `StatusCode == 401`. Credentials are not forwarded when a redirect goes to another host, unless you pass `--location-trusted`.

### Bearer Token Authentication

```csharp
// Using the builder
var result = await CurlRequestBuilder.Get("https://api.example.com/secure")
    .WithBearerToken("your-token-here")
    .ExecuteAsync();

// Using curl's --oauth2-bearer flag
var result2 = await Curl.ExecuteAsync("curl --oauth2-bearer your-token-here https://api.example.com/secure");

// Reusable authenticated client
public class AuthenticatedApiClient
{
    private readonly string _token;

    public AuthenticatedApiClient(string token)
    {
        _token = token;
    }

    public async Task<CurlResult> GetAsync(string endpoint)
    {
        return await CurlRequestBuilder.Get($"https://api.example.com{endpoint}")
            .WithBearerToken(_token)
            .ExecuteAsync();
    }
}
```

### API Key Authentication

```csharp
// API key in header
var result = await CurlRequestBuilder.Get("https://api.example.com/data")
    .WithHeader("X-API-Key", "your-api-key")
    .ExecuteAsync();

// API key in query parameter
var result2 = await Curl.GetAsync("https://api.example.com/data?api_key=your-api-key");
```

### OAuth 2.0 Flow

```csharp
// Step 1: Get access token (token endpoints take a form-encoded POST)
var tokenResult = await Curl.ExecuteAsync(@"curl https://auth.example.com/token \
    -d grant_type=client_credentials \
    -d client_id=your-client-id \
    -d client_secret=your-client-secret");

using var tokenResponse = JsonDocument.Parse(tokenResult.EnsureSuccess().Body);
var accessToken = tokenResponse.RootElement.GetProperty("access_token").GetString();

// Step 2: Use access token
var result = await CurlRequestBuilder.Get("https://api.example.com/protected")
    .WithBearerToken(accessToken)
    .ExecuteAsync();
```

## File Operations

### Downloading Files

```csharp
// Download to memory: binary content is in BinaryData, never decoded as text
var result = await Curl.GetAsync("https://example.com/file.pdf");
if (result.IsBinary)
    File.WriteAllBytes("downloaded.pdf", result.BinaryData);   // or result.SaveToFile("downloaded.pdf")

// Download straight to disk, as curl -o does (Body stays empty)
var saved = await Curl.ExecuteAsync("curl -o largefile.zip https://example.com/largefile.zip");

// Resume an interrupted download from the size of the existing file
var resumed = await Curl.ExecuteAsync("curl -C - -o largefile.zip https://example.com/largefile.zip");
```

CurlDotNet has no download progress callback today (`CurlSettings.WithProgress` is accepted but not invoked); `-C -` resume and `--retry` are the tools for long downloads.

### Uploading Files

```csharp
// Upload file as binary: --data-binary sends the bytes unchanged
var result = await Curl.ExecuteAsync(
    "curl --data-binary @document.pdf -H 'Content-Type: application/pdf' https://api.example.com/upload");

// The builder equivalent
var fileBytes = File.ReadAllBytes("document.pdf");
var result2 = await CurlRequestBuilder.Post("https://api.example.com/upload")
    .WithBinaryData(fileBytes)
    .WithHeader("Content-Type", "application/pdf")
    .ExecuteAsync();

// Multipart form upload: curl builds the boundary, Content-Disposition and file Content-Type
var result3 = await Curl.ExecuteAsync(
    "curl -F 'name=John Doe' -F 'file=@document.pdf' https://api.example.com/upload");
```

A missing upload file throws `CurlReadErrorException` (curl exit code 26) before anything is sent.

### Working with Streams

```csharp
// The response is fully buffered; ToStream() wraps it for stream-based APIs
var result = await Curl.GetAsync("https://api.example.com/stream");
using var stream = result.ToStream();
using var reader = new StreamReader(stream);

string line;
while ((line = await reader.ReadLineAsync()) != null)
{
    Console.WriteLine($"Received: {line}");
}
```

## Advanced Patterns

### Retry Logic

curl's `--retry` is built in: it retries timeouts and HTTP 408/429/500/502/503/504 with back-off that doubles from 1 second, honours `Retry-After`, and stops at `--retry-max-time`. `CurlSettings.WithRetries` is the .NET shortcut and retries any failure. See [curl --retry in C#](../cookbook/retry.html).

```csharp
var result = await Curl.ExecuteAsync("curl --retry 3 --retry-max-time 60 https://api.example.com/data");

var result2 = await Curl.ExecuteAsync("curl https://api.example.com/data",
    new CurlSettings().WithRetries(count: 3, delayMs: 1000));
```

When you need a custom policy, write the loop yourself:

```csharp
public async Task<CurlResult> ExecuteWithRetry(string url, int maxRetries = 3)
{
    for (int i = 0; i <= maxRetries; i++)
    {
        try
        {
            var result = await Curl.GetAsync(url);
            if (result.IsSuccess)
                return result;

            if (i < maxRetries && IsRetryableStatusCode(result.StatusCode))
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i))); // Exponential backoff
                continue;
            }

            return result;
        }
        catch (CurlTimeoutException) when (i < maxRetries)
        {
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
        }
    }

    throw new Exception($"Failed after {maxRetries} retries");
}

private bool IsRetryableStatusCode(int statusCode)
{
    return statusCode == 429 || statusCode >= 500;
}
```

### Rate Limiting

```csharp
public class RateLimitedClient
{
    private readonly SemaphoreSlim _semaphore;
    private readonly TimeSpan _resetInterval;
    private DateTime _nextReset;

    public RateLimitedClient(int requestsPerInterval, TimeSpan interval)
    {
        _semaphore = new SemaphoreSlim(requestsPerInterval);
        _resetInterval = interval;
        _nextReset = DateTime.UtcNow.Add(interval);
    }

    public async Task<CurlResult> ExecuteAsync(string url)
    {
        await _semaphore.WaitAsync();

        try
        {
            if (DateTime.UtcNow > _nextReset)
            {
                _nextReset = DateTime.UtcNow.Add(_resetInterval);
                _semaphore.Release(Math.Min(_semaphore.CurrentCount + 1, 10));
            }

            return await Curl.GetAsync(url);
        }
        finally
        {
            _ = Task.Delay(_resetInterval).ContinueWith(_ => _semaphore.Release());
        }
    }
}
```

### Parallel Requests

```csharp
// Execute multiple requests in parallel
var urls = new[]
{
    "https://api.example.com/data1",
    "https://api.example.com/data2",
    "https://api.example.com/data3"
};

var tasks = urls.Select(url => Curl.GetAsync(url));
var results = await Task.WhenAll(tasks);

foreach (var result in results)
{
    Console.WriteLine($"Status: {result.StatusCode}, Body length: {result.Body.Length}");
}

// Parallel with cancellation: a cancelled token throws OperationCanceledException
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
var cancellable = urls.Select(url => Curl.ExecuteAsync($"curl {url}", cts.Token));
var cancellableResults = await Task.WhenAll(cancellable);
```

Several URLs inside one command (`curl URL1 URL2` or a glob such as `data[1-3]`) also all run, but sequentially, as curl does; see [multiple URLs and globbing](../cookbook/multiple-urls-and-globbing.html).

### Connection Pooling

The static `Curl` API shares one internal `HttpClient`, so connections are pooled and reused across calls automatically; there is nothing to create or dispose. Commands with their own transport options (a proxy, `-k`, `--resolve` and similar) get a dedicated client configured for them. For a reusable client with shared defaults, use `LibCurl`:

```csharp
using CurlDotNet.Lib;

using var api = new LibCurl()
    .WithBearerToken(token)
    .WithTimeout(TimeSpan.FromSeconds(30));

var data = await api.GetAsync("https://api.example.com/data");
```

## Error Handling

### Basic Error Handling

```csharp
using CurlDotNet.Exceptions;

try
{
    // -f makes HTTP 400+ throw CurlHttpReturnedErrorException (curl exit code 22)
    var result = await Curl.ExecuteAsync("curl -f -m 30 https://api.example.com/data");
    // Process successful result
}
catch (CurlHttpReturnedErrorException httpEx)
{
    Console.WriteLine($"HTTP Error {httpEx.StatusCode}: {httpEx.Message}");
    if (httpEx.IsRateLimited)
    {
        var retryAfter = httpEx.GetRetryAfter();
        Console.WriteLine($"Rate limited. Retry after: {retryAfter}");
    }
}
catch (CurlOperationTimeoutException timeoutEx)
{
    Console.WriteLine($"Request timed out: {timeoutEx.Message}");   // -m expired, exit code 28
}
catch (CurlException curlEx)
{
    Console.WriteLine($"Curl error: {curlEx.Message}");
    foreach (var suggestion in curlEx.Suggestions)
    {
        Console.WriteLine($"  - {suggestion}");
    }
}
```

### Custom Error Handling

```csharp
public class ApiErrorHandler
{
    public async Task<T> ExecuteWithHandling<T>(
        Func<Task<CurlResult>> request,
        Func<CurlResult, T> processResult)
    {
        try
        {
            var result = await request();

            if (!result.IsSuccess)
            {
                throw result.StatusCode switch
                {
                    401 => new UnauthorizedException("Authentication required"),
                    403 => new ForbiddenException("Access denied"),
                    404 => new NotFoundException("Resource not found"),
                    429 => new RateLimitException("Too many requests"),
                    >= 500 => new ServerException("Server error"),
                    _ => new ApiException($"Request failed: {result.StatusCode}")
                };
            }

            return processResult(result);
        }
        catch (CurlException ex)
        {
            throw new ApiException("Network error", ex);
        }
    }
}
```

## Performance Optimization

### Caching Responses

```csharp
public class CachedApiClient
{
    private readonly MemoryCache _cache;

    public CachedApiClient()
    {
        _cache = new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = 100
        });
    }

    public async Task<string> GetCachedAsync(string url)
    {
        if (_cache.TryGetValue(url, out string cached))
        {
            return cached;
        }

        var result = await Curl.GetAsync(url);

        if (result.IsSuccess)
        {
            _cache.Set(url, result.Body, new MemoryCacheEntryOptions
            {
                Size = 1,
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });
        }

        return result.Body;
    }
}
```

### Request Compression

```csharp
// Ask for a compressed response and let CurlDotNet decode it (gzip, deflate, br)
var result = await Curl.ExecuteAsync("curl --compressed https://api.example.com/data");
// result.Body is decoded; result.Headers keeps Content-Encoding exactly as the server sent it

// Send a gzip-compressed request body yourself
var compressed = await CurlRequestBuilder.Post("https://api.example.com/data")
    .WithBinaryData(CompressString(jsonData))
    .WithHeader("Content-Type", "application/json")
    .WithHeader("Content-Encoding", "gzip")
    .ExecuteAsync();

private byte[] CompressString(string text)
{
    var bytes = Encoding.UTF8.GetBytes(text);
    using var output = new MemoryStream();
    using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
    {
        gzip.Write(bytes, 0, bytes.Length);
    }
    return output.ToArray();
}
```

## Testing and Mocking

### Unit Testing with CurlDotNet

```csharp
[TestClass]
public class ApiClientTests
{
    [TestMethod]
    public async Task GetUser_ReturnsValidUser()
    {
        // Arrange
        var expectedUser = new User { Id = 1, Name = "Test User" };

        // Act
        var result = await Curl.GetAsync("https://jsonplaceholder.typicode.com/users/1");
        var user = result.ParseJson<User>();   // case-insensitive, so "id" binds to Id

        // Assert
        Assert.IsNotNull(user);
        Assert.AreEqual(1, user.Id);
        Assert.IsFalse(string.IsNullOrEmpty(user.Name));
    }
}
```

### Integration Testing

```csharp
[TestClass]
[TestCategory("Integration")]
public class ApiIntegrationTests
{
    private string _baseUrl = "https://api.example.com";

    [TestMethod]
    public async Task CreateAndDeleteUser_Workflow()
    {
        // Create user
        var createResult = await Curl.PostJsonAsync($"{_baseUrl}/users", new { name = "Test User" });
        Assert.AreEqual(201, createResult.StatusCode);

        var userId = JsonDocument.Parse(createResult.Body)
            .RootElement.GetProperty("id").GetInt32();

        // Verify user exists
        var getResult = await Curl.GetAsync($"{_baseUrl}/users/{userId}");
        Assert.AreEqual(200, getResult.StatusCode);

        // Delete user
        var deleteResult = await Curl.ExecuteAsync($"curl -X DELETE {_baseUrl}/users/{userId}");
        Assert.AreEqual(204, deleteResult.StatusCode);

        // Verify user deleted
        var verifyResult = await Curl.GetAsync($"{_baseUrl}/users/{userId}");
        Assert.AreEqual(404, verifyResult.StatusCode);
    }
}
```

## Troubleshooting

### Common Issues and Solutions

#### Connection Timeouts
```csharp
// There is no timeout unless you set one; set a generous one for slow endpoints
var result = await CurlRequestBuilder.Get("https://slow-api.example.com/data")
    .WithConnectTimeout(TimeSpan.FromSeconds(10))
    .WithTimeout(TimeSpan.FromMinutes(5))
    .ExecuteAsync();

// Same as: curl --connect-timeout 10 -m 300 https://slow-api.example.com/data
```

#### SSL Certificate Issues
```csharp
// Disable SSL verification (development only!)
var result = await CurlRequestBuilder.Get("https://self-signed.example.com")
    .Insecure()
    .ExecuteAsync();

// Same as: curl -k https://self-signed.example.com
```

`--cacert` and `--cert` are parsed but not applied; certificates are checked against the operating system's trust store.

#### Proxy Configuration
```csharp
// Use proxy server
var result = await CurlRequestBuilder.Get("https://api.example.com/data")
    .WithProxy("http://proxy.company.com:8080", "username", "password")
    .ExecuteAsync();

// Same as: curl -x http://proxy.company.com:8080 -U username:password https://api.example.com/data
```

The `http_proxy`, `https_proxy`, `all_proxy` and `NO_PROXY` environment variables are honoured as in curl.

#### Debugging Requests
```csharp
// Enable verbose output for debugging: the > / < log is at the start of result.Body
var result = await CurlRequestBuilder.Get("https://api.example.com/data")
    .Verbose()
    .ExecuteAsync();

// Full wire dump to a file, and curl's write-out summary
var traced = await Curl.ExecuteAsync(
    "curl --trace-ascii trace.txt -w '\\n%{http_code} %{time_total}s' https://api.example.com/data");
```

See [curl -w in C#](../cookbook/write-out.html) for every write-out variable.

### Logging

```csharp
public class LoggingApiClient
{
    private readonly ILogger<LoggingApiClient> _logger;

    public LoggingApiClient(ILogger<LoggingApiClient> logger)
    {
        _logger = logger;
    }

    public async Task<CurlResult> ExecuteAsync(string url)
    {
        _logger.LogInformation("Executing request to {Url}", url);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await Curl.GetAsync(url);

            _logger.LogInformation(
                "Request completed: {StatusCode} in {ElapsedMs}ms",
                result.StatusCode,
                stopwatch.ElapsedMilliseconds
            );

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Request failed after {ElapsedMs}ms",
                stopwatch.ElapsedMilliseconds
            );
            throw;
        }
    }
}
```

## Best Practices

1. **Always dispose resources**: Use `using` statements or implement `IDisposable`
2. **Handle errors gracefully**: Catch specific exception types
3. **Use timeouts**: There is no default timeout, so set `-m` (or `Curl.DefaultMaxTimeSeconds`) to prevent hanging requests
4. **Validate inputs**: Check URLs and parameters before sending
5. **Log requests**: Track API usage and errors
6. **Use connection pooling**: Reuse HTTP connections
7. **Retry transient failures**: `--retry N` or `CurlSettings.WithRetries`
8. **Respect rate limits**: Implement throttling
9. **Secure credentials**: Never hardcode secrets
10. **Test thoroughly**: Unit and integration tests

## Further Resources

- [CurlDotNet API Reference](https://jacob-mellor.github.io/curl-dot-net)
- [Sample Applications](../samples/)
- [libcurl Mapping Guide](../reference/libcurl-mapping.html)
- [Troubleshooting Guide](../troubleshooting/)