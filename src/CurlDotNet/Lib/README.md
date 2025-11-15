# LibCurl API

This directory contains the object-oriented, programmatic API similar to libcurl.

## Purpose

The LibCurl namespace provides a programmatic, object-oriented API for developers who prefer building requests through code rather than parsing curl command strings. This API is inspired by curl's libcurl C API but adapted for .NET patterns.

## Components

### LibCurl Class

**`LibCurl.cs`** - Main class providing fluent API for building and executing requests. Provides:

#### HTTP Methods
- `GetAsync(string url)` - Execute GET request
- `PostAsync(string url, object? data = null)` - Execute POST request
- `PutAsync(string url, object? data = null)` - Execute PUT request
- `DeleteAsync(string url)` - Execute DELETE request
- `PatchAsync(string url, object? data = null)` - Execute PATCH request
- `HeadAsync(string url)` - Execute HEAD request

#### Configuration Methods (Fluent)
- `WithTimeout(TimeSpan timeout)` - Set request timeout
- `WithConnectTimeout(TimeSpan timeout)` - Set connection timeout
- `WithFollowRedirects(bool follow)` - Enable/disable redirect following
- `WithInsecureSsl(bool insecure)` - Skip SSL certificate validation
- `WithProxy(string proxyUrl)` - Set proxy server
- `WithUserAgent(string userAgent)` - Set user agent
- `WithOutputFile(string filePath)` - Save response to file
- `WithVerbose(bool verbose)` - Enable verbose logging

#### Advanced Configuration
- `Configure(Action<CurlOptions> configure)` - Advanced option configuration

#### Execution
- `ExecuteAsync()` - Execute the configured request

## Design Principles

1. **Fluent Interface** - Method chaining for intuitive API usage
2. **Type Safety** - Strongly-typed parameters and return values
3. **IntelliSense Friendly** - Discoverable through autocomplete
4. **Optional Parameters** - Sensible defaults, complexity hidden

## Usage Examples

```csharp
// Simple GET request
var result = await LibCurl.GetAsync("https://api.example.com/data");

// POST with JSON data
var result = await LibCurl
    .PostAsync("https://api.example.com/users", new { name = "John" })
    .WithHeader("Authorization", "Bearer token123")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .ExecuteAsync();

// Complex request with multiple options
var result = await LibCurl
    .GetAsync("https://api.example.com/data")
    .WithFollowRedirects(true)
    .WithProxy("http://proxy.example.com:8080")
    .WithOutputFile("response.json")
    .ExecuteAsync();
```

## When to Use

- **Use LibCurl API**: When building requests programmatically in code, when you want type safety and IntelliSense support
- **Use Curl.ExecuteAsync(string)**: When you have curl commands from documentation or want to copy-paste directly

Both APIs provide the same functionality—choose based on your preference and use case.

## See Also

- [CurlRequestBuilder](../Core/CurlRequestBuilder.cs) - Similar fluent API in Core namespace
- [Main Curl Class](../Curl.cs) - String-based API

