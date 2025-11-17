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
var result = await new CurlRequestBuilder()
    .SetUrl("https://api.example.com/users")
    .AddHeader("Accept", "application/json")
    .AddHeader("X-API-Version", "2.0")
    .Build()
    .ExecuteAsync();
```

### POST Requests

```csharp
// POST with JSON body
var json = "{\"name\":\"John Doe\",\"email\":\"john@example.com\"}";
var result = await Curl.PostAsync(
    "https://api.example.com/users",
    json,
    "application/json"
);

// POST with form data
var formData = "username=john&password=secret";
var result = await Curl.PostAsync(
    "https://api.example.com/login",
    formData,
    "application/x-www-form-urlencoded"
);

// POST with custom headers
var result = await new CurlRequestBuilder()
    .SetUrl("https://api.example.com/users")
    .SetMethod("POST")
    .SetBody(json)
    .AddHeader("Content-Type", "application/json")
    .AddHeader("X-Request-ID", Guid.NewGuid().ToString())
    .Build()
    .ExecuteAsync();
```

### PUT Requests

```csharp
// PUT to update resource
var updateData = "{\"name\":\"Jane Doe\",\"email\":\"jane@example.com\"}";
var result = await Curl.PutAsync(
    "https://api.example.com/users/123",
    updateData,
    "application/json"
);

// PUT with file upload
var fileContent = File.ReadAllBytes("document.pdf");
var result = await new CurlRequestBuilder()
    .SetUrl("https://api.example.com/files/document.pdf")
    .SetMethod("PUT")
    .SetBody(fileContent)
    .AddHeader("Content-Type", "application/pdf")
    .Build()
    .ExecuteAsync();
```

### DELETE Requests

```csharp
// Simple DELETE
var result = await Curl.DeleteAsync("https://api.example.com/users/123");

// DELETE with confirmation header
var result = await new CurlRequestBuilder()
    .SetUrl("https://api.example.com/users/123")
    .SetMethod("DELETE")
    .AddHeader("X-Confirm-Delete", "true")
    .Build()
    .ExecuteAsync();
```

### PATCH Requests

```csharp
// PATCH for partial updates
var patchData = "[{\"op\":\"replace\",\"path\":\"/email\",\"value\":\"new@example.com\"}]";
var result = await Curl.PatchAsync(
    "https://api.example.com/users/123",
    patchData,
    "application/json-patch+json"
);
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

var result = await Curl.GetAsync("https://api.example.com/user/123");
var user = JsonSerializer.Deserialize<User>(result.Body);
```

### Sending JSON Data

```csharp
// Using anonymous objects
var data = new
{
    name = "John Doe",
    email = "john@example.com",
    age = 30
};

var json = JsonSerializer.Serialize(data);
var result = await Curl.PostAsync("https://api.example.com/users", json, "application/json");

// Using strongly-typed objects
var user = new User { Name = "Jane Doe", Email = "jane@example.com" };
var json = JsonSerializer.Serialize(user);
var result = await Curl.PostAsync("https://api.example.com/users", json, "application/json");
```

### Working with JSON Arrays

```csharp
// Parse JSON array
var result = await Curl.GetAsync("https://api.example.com/users");
var users = JsonSerializer.Deserialize<List<User>>(result.Body);

foreach (var user in users)
{
    Console.WriteLine($"{user.Name} - {user.Email}");
}

// Send JSON array
var users = new[]
{
    new User { Name = "User1", Email = "user1@example.com" },
    new User { Name = "User2", Email = "user2@example.com" }
};

var json = JsonSerializer.Serialize(users);
var result = await Curl.PostAsync("https://api.example.com/users/bulk", json, "application/json");
```

## Authentication

### Basic Authentication

```csharp
// Using curl command syntax
var result = await Curl.ExecuteAsync("curl -u username:password https://api.example.com/secure");

// Using builder
var result = await new CurlRequestBuilder()
    .SetUrl("https://api.example.com/secure")
    .SetAuthentication("username", "password")
    .Build()
    .ExecuteAsync();

// Using Authorization header
var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("username:password"));
var result = await new CurlRequestBuilder()
    .SetUrl("https://api.example.com/secure")
    .AddHeader("Authorization", $"Basic {credentials}")
    .Build()
    .ExecuteAsync();
```

### Bearer Token Authentication

```csharp
// Using Authorization header
var result = await new CurlRequestBuilder()
    .SetUrl("https://api.example.com/secure")
    .AddHeader("Authorization", "Bearer your-token-here")
    .Build()
    .ExecuteAsync();

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
        return await new CurlRequestBuilder()
            .SetUrl($"https://api.example.com{endpoint}")
            .AddHeader("Authorization", $"Bearer {_token}")
            .Build()
            .ExecuteAsync();
    }
}
```

### API Key Authentication

```csharp
// API key in header
var result = await new CurlRequestBuilder()
    .SetUrl("https://api.example.com/data")
    .AddHeader("X-API-Key", "your-api-key")
    .Build()
    .ExecuteAsync();

// API key in query parameter
var result = await Curl.GetAsync("https://api.example.com/data?api_key=your-api-key");
```

### OAuth 2.0 Flow

```csharp
// Step 1: Get access token
var tokenRequest = new
{
    grant_type = "client_credentials",
    client_id = "your-client-id",
    client_secret = "your-client-secret"
};

var tokenResult = await Curl.PostAsync(
    "https://auth.example.com/token",
    JsonSerializer.Serialize(tokenRequest),
    "application/json"
);

var tokenResponse = JsonDocument.Parse(tokenResult.Body);
var accessToken = tokenResponse.RootElement.GetProperty("access_token").GetString();

// Step 2: Use access token
var result = await new CurlRequestBuilder()
    .SetUrl("https://api.example.com/protected")
    .AddHeader("Authorization", $"Bearer {accessToken}")
    .Build()
    .ExecuteAsync();
```

## File Operations

### Downloading Files

```csharp
// Download to memory
var result = await Curl.GetAsync("https://example.com/file.pdf");
File.WriteAllBytes("downloaded.pdf", Encoding.UTF8.GetBytes(result.Body));

// Download with progress tracking
var result = await new CurlRequestBuilder()
    .SetUrl("https://example.com/largefile.zip")
    .SetProgressCallback((downloaded, total) =>
    {
        var percent = (downloaded / (double)total) * 100;
        Console.WriteLine($"Downloaded: {percent:F2}%");
    })
    .Build()
    .ExecuteAsync();
```

### Uploading Files

```csharp
// Upload file as binary
var fileBytes = File.ReadAllBytes("document.pdf");
var result = await new CurlRequestBuilder()
    .SetUrl("https://api.example.com/upload")
    .SetMethod("POST")
    .SetBody(fileBytes)
    .AddHeader("Content-Type", "application/pdf")
    .Build()
    .ExecuteAsync();

// Multipart form upload
var boundary = $"----Boundary{Guid.NewGuid():N}";
var content = new MultipartFormDataContent(boundary);
content.Add(new StringContent("John Doe"), "name");
content.Add(new ByteArrayContent(fileBytes), "file", "document.pdf");

var result = await new CurlRequestBuilder()
    .SetUrl("https://api.example.com/upload")
    .SetMethod("POST")
    .SetBody(await content.ReadAsStringAsync())
    .AddHeader("Content-Type", $"multipart/form-data; boundary={boundary}")
    .Build()
    .ExecuteAsync();
```

### Working with Streams

```csharp
// Stream response handling
var result = await Curl.GetAsync("https://api.example.com/stream");
using var stream = new MemoryStream(Encoding.UTF8.GetBytes(result.Body));
using var reader = new StreamReader(stream);

string line;
while ((line = await reader.ReadLineAsync()) != null)
{
    Console.WriteLine($"Received: {line}");
}
```

## Advanced Patterns

### Retry Logic

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

// Parallel with cancellation
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
var tasks = urls.Select(url => Curl.GetAsync(url, cts.Token));
var results = await Task.WhenAll(tasks);
```

### Connection Pooling

```csharp
// Reuse connections for multiple requests
public class ApiClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public ApiClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.example.com"),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public async Task<string> GetDataAsync(string endpoint)
    {
        // CurlDotNet can work with HttpClient for connection pooling
        var response = await _httpClient.GetAsync(endpoint);
        return await response.Content.ReadAsStringAsync();
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
```

## Error Handling

### Basic Error Handling

```csharp
try
{
    var result = await Curl.GetAsync("https://api.example.com/data");
    result.EnsureSuccessStatusCode();
    // Process successful result
}
catch (CurlHttpException httpEx)
{
    Console.WriteLine($"HTTP Error {httpEx.StatusCode}: {httpEx.Message}");
    if (httpEx.IsRateLimited)
    {
        var retryAfter = httpEx.GetRetryAfter();
        Console.WriteLine($"Rate limited. Retry after: {retryAfter}");
    }
}
catch (CurlTimeoutException timeoutEx)
{
    Console.WriteLine($"Request timed out: {timeoutEx.Message}");
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
// Enable gzip compression for requests and responses
var result = await new CurlRequestBuilder()
    .SetUrl("https://api.example.com/data")
    .AddHeader("Accept-Encoding", "gzip, deflate")
    .AddHeader("Content-Encoding", "gzip")
    .SetBody(CompressString(jsonData))
    .Build()
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
        var user = JsonSerializer.Deserialize<User>(result.Body);

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
        var createData = JsonSerializer.Serialize(new { name = "Test User" });
        var createResult = await Curl.PostAsync($"{_baseUrl}/users", createData, "application/json");
        Assert.AreEqual(201, createResult.StatusCode);

        var userId = JsonDocument.Parse(createResult.Body)
            .RootElement.GetProperty("id").GetInt32();

        // Verify user exists
        var getResult = await Curl.GetAsync($"{_baseUrl}/users/{userId}");
        Assert.AreEqual(200, getResult.StatusCode);

        // Delete user
        var deleteResult = await Curl.DeleteAsync($"{_baseUrl}/users/{userId}");
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
// Increase timeout for slow endpoints
var result = await new CurlRequestBuilder()
    .SetUrl("https://slow-api.example.com/data")
    .SetTimeout(TimeSpan.FromMinutes(5))
    .Build()
    .ExecuteAsync();
```

#### SSL Certificate Issues
```csharp
// Disable SSL verification (development only!)
var result = await new CurlRequestBuilder()
    .SetUrl("https://self-signed.example.com")
    .SetSslVerification(false)
    .Build()
    .ExecuteAsync();
```

#### Proxy Configuration
```csharp
// Use proxy server
var result = await new CurlRequestBuilder()
    .SetUrl("https://api.example.com/data")
    .SetProxy("http://proxy.company.com:8080")
    .SetProxyAuthentication("username", "password")
    .Build()
    .ExecuteAsync();
```

#### Debugging Requests
```csharp
// Enable verbose output for debugging
var result = await new CurlRequestBuilder()
    .SetUrl("https://api.example.com/data")
    .SetVerbose(true)
    .SetDebugCallback((type, data) =>
    {
        Console.WriteLine($"[{type}] {data}");
    })
    .Build()
    .ExecuteAsync();
```

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
3. **Use timeouts**: Prevent hanging requests
4. **Validate inputs**: Check URLs and parameters before sending
5. **Log requests**: Track API usage and errors
6. **Use connection pooling**: Reuse HTTP connections
7. **Implement retry logic**: Handle transient failures
8. **Respect rate limits**: Implement throttling
9. **Secure credentials**: Never hardcode secrets
10. **Test thoroughly**: Unit and integration tests

## Further Resources

- [CurlDotNet API Reference](https://jacob-mellor.github.io/curl-dot-net)
- [Sample Applications](../samples/README.md)
- [libcurl Mapping Guide](LIBCURL_MAPPING.md)
- [Troubleshooting Guide](../docs/troubleshooting/README.md)