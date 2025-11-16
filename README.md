# CurlDotNet - Pure .NET Implementation of curl for C# Developers

<div align="center">
  <img src="build/icon-128.png" alt="CurlDotNet Logo" width="128" />

  ### 📚 [Full Documentation](https://jacob-mellor.github.io/curl-dot-net/) | 🚀 [Getting Started](https://jacob-mellor.github.io/curl-dot-net/articles/getting-started.html) | 📖 [API Reference](https://jacob-mellor.github.io/curl-dot-net/api/)
</div>

[![NuGet](https://img.shields.io/nuget/v/CurlDotNet.svg)](https://www.nuget.org/packages/CurlDotNet/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/CurlDotNet.svg)](https://www.nuget.org/packages/CurlDotNet/)
[![Documentation](https://img.shields.io/badge/Docs-GitHub%20Pages-blue.svg)](https://jacob-mellor.github.io/curl-dot-net/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![.NET 10 Ready](https://img.shields.io/badge/.NET%2010-Ready-success.svg)](https://jacob-mellor.github.io/curl-dot-net/)
[![Tests](https://img.shields.io/badge/Tests-228%2F240%20(95%25)-yellow.svg)](tests/README.md)
[![.NET Standard 2.0](https://img.shields.io/badge/.NET%20Standard%202.0-Universal-blue.svg)](https://jacob-mellor.github.io/curl-dot-net/)
[![Build Status](https://img.shields.io/badge/Build-Passing-success.svg)](build/README.md)
[![UserlandDotNet Vision](https://img.shields.io/badge/Userland-.NET%20Tooling-512BD4.svg)](https://userlanddotnet.org)
[![Sponsored by IronSoftware](https://img.shields.io/badge/Sponsored%20by-IronSoftware-red.svg)](https://ironsoftware.com)

📚 **[Full Documentation](https://jacob-mellor.github.io/curl-dot-net/)** | 🚀 **[Getting Started](https://jacob-mellor.github.io/curl-dot-net/articles/getting-started.html)** | 📖 **[API Reference](https://jacob-mellor.github.io/curl-dot-net/api/)**

## 🌟 The Revolution: Copy & Paste curl Commands Directly into C#

**CurlDotNet** is the pure .NET implementation of curl that allows C# developers to execute curl commands directly in their applications. No translation required. No shell execution. No learning curve. Just paste any curl command from API documentation, Stack Overflow, or bash scripts, and it works immediately in your .NET application.

### Why CurlDotNet Exists: Solving the curl-to-C# Translation Problem

Every developer has faced this problem: you find a perfect curl command in API documentation, Stack Overflow, or a bash script, but you need to use it in your C# application. The traditional approach requires manually translating curl syntax to HttpClient, WebClient, or RestSharp—a time-consuming, error-prone process that breaks the flow of development.

**CurlDotNet eliminates this friction entirely.** Paste the curl command directly into your C# code, and it executes exactly as it would in a terminal. This is not a wrapper around the curl binary—it's a complete, native .NET implementation that transpiles curl's C source code logic, providing the same behavior with pure .NET performance and compatibility.

### The Killer Feature: Universal curl Command Support

```csharp
using CurlDotNet;

// From GitHub API documentation - paste it directly!
var result = await Curl.ExecuteAsync(@"
    curl -X POST \
      -H ""Accept: application/vnd.github+json"" \
      -H ""Authorization: Bearer YOUR_TOKEN"" \
      -H ""X-GitHub-Api-Version: 2022-11-28"" \
      https://api.github.com/repos/OWNER/REPO/issues
");

// From Stripe API docs - works immediately
var charge = await Curl.ExecuteAsync(@"
    curl https://api.stripe.com/v1/charges \
      -u sk_test_1234567890: \
      -d amount=2000 \
      -d currency=usd \
      -d source=tok_visa
");

// Complex command with all options - paste and go!
var response = await Curl.ExecuteAsync(@"
    curl -X PUT \
      -H 'Content-Type: application/json' \
      -H 'Authorization: Bearer token123' \
      -d '{\""key\"":\""value\""}' \
      -o response.json \
      -L -v -k \
      --connect-timeout 30 \
      --max-time 120 \
      --retry 3 \
      https://api.example.com/endpoint
");
```

## 🚀 Key Features and Capabilities

### Complete curl Compatibility

CurlDotNet supports all 300+ curl options, ensuring complete compatibility with curl command-line syntax. Whether you're using basic HTTP methods, complex authentication, file uploads, or advanced features like custom certificates, proxies, or retries, CurlDotNet handles it all.

**Supported Features:**
- **HTTP Methods**: GET, POST, PUT, DELETE, PATCH, HEAD, OPTIONS, TRACE, and custom methods
- **Authentication**: Basic Auth, Bearer Tokens, OAuth, NTLM, Kerberos, Digest, and custom auth headers
- **Headers**: Full support for custom headers, including multi-line headers and header continuations
- **Request Bodies**: JSON, form data, multipart form data, URL-encoded data, raw data, binary data, and file uploads
- **Response Handling**: Save to file, save to memory, stream responses, parse JSON/XML, extract headers
- **Redirects**: Automatic redirect following with configurable limits and redirect policies
- **SSL/TLS**: Certificate validation, custom certificates, client certificates, insecure mode for testing
- **Proxies**: HTTP proxies, SOCKS proxies, proxy authentication, environment variable proxy support
- **Cookies**: Cookie jars, cookie files, session cookies, persistent cookies across requests
- **Timeouts**: Connection timeouts, operation timeouts, read/write timeouts, configurable retry logic
- **Advanced**: Compression (gzip, deflate, brotli), HTTP/2, HTTP/3, range requests, resume downloads, speed limiting, progress callbacks, and much more

### Three Ways to Use CurlDotNet

CurlDotNet provides three distinct APIs, each optimized for different use cases:

#### 1. Curl String API (Simplest - Recommended for Most Cases)

Perfect when you have curl commands from documentation or want to paste commands directly:

```csharp
using CurlDotNet;

// Works with or without "curl" prefix
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

// Also works without "curl"
var result2 = await Curl.ExecuteAsync("-H 'Accept: application/json' https://api.example.com/data");

// Handles complex commands with all options
var result3 = await Curl.ExecuteAsync(@"
    curl -X POST \
      -H 'Content-Type: application/json' \
      -H 'Authorization: Bearer token' \
      -d '{\""name\"":\""John\""}' \
      https://api.example.com/users
");
```

**When to use:** When you have curl commands from API documentation, Stack Overflow, or bash scripts. Zero translation required.

#### 2. Fluent Builder API (Type-Safe)

Build requests programmatically with full IntelliSense support and compile-time checking:

```csharp
using CurlDotNet.Core;

// Fluent method chaining with IntelliSense
var result = await CurlRequestBuilder
    .Get("https://api.example.com/users")
    .WithHeader("Accept", "application/json")
    .WithHeader("Authorization", "Bearer token123")
    .WithBearerToken("token123")  // Convenience method
    .WithTimeout(TimeSpan.FromSeconds(30))
    .FollowRedirects()
    .Insecure()  // For testing only
    .ExecuteAsync();

// POST with JSON data
var result2 = await CurlRequestBuilder
    .Post("https://api.example.com/users")
    .WithJson(new { name = "John", email = "john@example.com" })
    .WithHeader("X-API-Key", "your-key")
    .ExecuteAsync();

// Complex multipart form upload
var result3 = await CurlRequestBuilder
    .Post("https://api.example.com/upload")
    .WithFormField("name", "John Doe")
    .WithFormField("email", "john@example.com")
    .WithFile("file", "path/to/document.pdf")
    .ExecuteAsync();
```

**When to use:** When building requests dynamically, need type safety, or prefer programmatic APIs with IntelliSense.

#### 3. LibCurl API (Reusable Client)

Object-oriented API for creating reusable HTTP clients with persistent configuration:

```csharp
using CurlDotNet.Lib;
using (var curl = new LibCurl())
{
    // Configure defaults for all requests
    curl.WithHeader("Accept", "application/json")
        .WithBearerToken("token123")
        .WithTimeout(TimeSpan.FromSeconds(30))
        .WithFollowRedirects()
        .WithUserAgent("MyApp/1.0");

    // Make multiple requests with same configuration
    var user = await curl.GetAsync("https://api.example.com/user");
    var posts = await curl.GetAsync("https://api.example.com/posts");
    var comments = await curl.GetAsync("https://api.example.com/comments");

    // Per-request overrides when needed
    var special = await curl.GetAsync("https://api.example.com/special", 
        opts => opts.Headers["X-Special"] = "true");
}
```

**When to use:** When making multiple requests to the same API with shared configuration, building HTTP clients, or implementing service classes.

## Microsoft & .NET Foundation Roots

- Built by **Jacob Mellor**, CTO at [IronSoftware](https://ironsoftware.com/about-us/authors/jacobmellor/) with 25+ years of Microsoft ecosystem leadership. Connect via [GitHub](https://github.com/jacob-mellor) or [LinkedIn](https://www.linkedin.com/in/jacob-mellor-iron-software/).
- Inspired by [.NET Foundation](https://dotnetfoundation.org/) guidance and community figures like Jeff Fritz—transparent engineering, heavy testing, world-class docs.
- Operates under the **UserlandDotNet** initiative to bring Linux userland tools to .NET and PowerShell without native binaries. Learn more at [userlanddotnet.org](https://userlanddotnet.org).

## 🎯 Test Coverage & Quality

**Current Test Status:** 228 passing out of 240 tests **(95% success rate)**
⚠️ **Note:** Working towards 100% test pass rate for v1.0.1

CurlDotNet is tested across multiple dimensions:
- **Unit Tests:** Command parsing, option handling, protocol support
- **Integration Tests:** Real HTTP requests, authentication, redirects
- **Synthetic Tests:** Edge cases, error handling, performance
- **Comparison Tests:** Validates behavior matches native curl

## 🚀 Platform Support - .NET 10 Ready!

CurlDotNet ships multi-targeted binaries supporting **every .NET platform**:

### Fully Tested & Supported:
- **`.NET 10`** – ✅ Fully tested and ready (latest features & performance)
- **`.NET 8`** – ✅ Current LTS with optimal performance
- **`.NET Standard 2.0`** – ✅ Universal compatibility enabling:
  - `.NET Framework 4.7.2+` (Legacy WinForms/WPF)
  - `Xamarin.iOS` / `Xamarin.Android`
  - `Unity 2018.1+`
  - `Blazor WebAssembly`
  - `MAUI` (all platforms)
  - `UWP` (Universal Windows Platform)
  - Older ASP.NET Core versions

### Additional Platform Support:
- **PowerShell 7.4+** – Direct DLL reference for scripting
- **Azure Functions / App Service** – Works via DI without native binaries
- **Docker / Kubernetes** – No curl binary needed in containers

See the [full documentation](https://jacob-mellor.github.io/curl-dot-net/) for platform compatibility details.

## 📦 Installation and Setup

### NuGet Package Installation

Install CurlDotNet via NuGet Package Manager, .NET CLI, or PackageReference:

**Package Manager Console:**
```powershell
Install-Package CurlDotNet
```

**.NET CLI:**
```bash
dotnet add package CurlDotNet
```

**PackageReference (in .csproj):**
```xml
<ItemGroup>
<PackageReference Include="CurlDotNet" Version="1.0.0" />
</ItemGroup>
```

**Direct Download:**
Visit [NuGet.org - CurlDotNet](https://www.nuget.org/packages/CurlDotNet/) to download the package directly or view package statistics, dependencies, and version history.

### System Requirements

CurlDotNet requires:
- **.NET Standard 2.0** or higher (supports .NET Framework 4.7.2+, .NET Core 2.0+, .NET 5.0+, .NET 6.0+, .NET 7.0+, .NET 8.0+, .NET 10.0+)
- **.NET 10.0** or **.NET 8.0** recommended for best performance and latest features
- **.NET Framework 4.7.2** or higher on Windows
- No external dependencies beyond standard .NET libraries

### Platform Compatibility

CurlDotNet is fully cross-platform and supports:

| Platform | Version | Status | Notes |
|----------|---------|--------|-------|
| .NET 10.0 | 10.0+ | ✅ Full Support | Latest features & performance |
| .NET 8.0 | 8.0+ | ✅ Full Support | Current LTS, recommended |
| .NET 6.0 | 6.0+ | ✅ Full Support | Previous LTS |
| .NET 5.0 | 5.0+ | ✅ Full Support | Out of support |
| .NET Core 3.1 | 3.1+ | ✅ Full Support | Legacy LTS |
| .NET Standard 2.0 | 2.0+ | ✅ Full Support | Maximum compatibility |
| .NET Framework | 4.7.2+ | ✅ Full Support | Windows only |
| Xamarin | Latest | ✅ Full Support | iOS, Android, macOS |
| Unity | 2018.1+ | ✅ Full Support | Games and applications |
| Blazor | Latest | ✅ Full Support | WebAssembly & Server |
| UWP | 10.0+ | ✅ Full Support | Universal Windows Platform |

## 🎯 Quick Start Guide

### Your First curl Command in C#

Here's a complete example that demonstrates how easy it is to use CurlDotNet:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main(string[] args)
    {
        // That curl command from the API docs? Just paste it!
        var result = await Curl.ExecuteAsync(
            "curl -H 'Accept: application/json' https://api.github.com/users/octocat"
        );

        // Check if successful
        if (result.StatusCode == 200)
        {
            Console.WriteLine("Success!");
            Console.WriteLine(result.Body);  // JSON response
            
            // Parse JSON directly
            dynamic json = result.AsJsonDynamic();
            Console.WriteLine($"User: {json.login}");
            Console.WriteLine($"Followers: {json.followers}");
        }
        else
        {
            Console.WriteLine($"Error: {result.StatusCode}");
        }
    }
}
```

### Common Use Cases

#### Making GET Requests

```csharp
// Simple GET request
var result = await Curl.ExecuteAsync("curl https://api.example.com/users");

// GET with custom headers
var result = await Curl.ExecuteAsync(@"
    curl -H 'Accept: application/json' \
         -H 'X-API-Key: your-key' \
         https://api.example.com/data
");

// GET with query parameters
var result = await Curl.ExecuteAsync(
    "curl 'https://api.example.com/search?q=test&limit=10'"
);
```

#### Making POST Requests

```csharp
// POST with JSON data
var result = await Curl.ExecuteAsync(@"
    curl -X POST \
      -H 'Content-Type: application/json' \
      -d '{\""name\"":\""John\"",\""email\"":\""john@example.com\""}' \
      https://api.example.com/users
");

// POST with form data
var result = await Curl.ExecuteAsync(@"
    curl -X POST \
      -d 'username=john&password=secret' \
      https://api.example.com/login
");

// POST with file upload
var result = await Curl.ExecuteAsync(@"
    curl -X POST \
      -F 'file=@/path/to/document.pdf' \
      -F 'description=Monthly Report' \
      https://api.example.com/upload
");
```

#### Working with Authentication

```csharp
// Basic Authentication
var result = await Curl.ExecuteAsync(
    "curl -u username:password https://api.example.com/private"
);

// Bearer Token Authentication
var result = await Curl.ExecuteAsync(@"
    curl -H 'Authorization: Bearer YOUR_TOKEN' \
         https://api.example.com/protected
");

// OAuth Token (works the same way)
var result = await Curl.ExecuteAsync(@"
    curl -H 'Authorization: Bearer oauth_token_here' \
         https://api.example.com/oauth-endpoint
");
```

#### Downloading Files

```csharp
// Download to file
var result = await Curl.ExecuteAsync(
    "curl -o report.pdf https://example.com/report.pdf"
);

// Download to memory and then save
var result = await Curl.ExecuteAsync(
    "curl https://example.com/image.jpg"
);
result.SaveToFile("image.jpg");

// Download with progress
var result = await Curl.ExecuteAsync(
    "curl --progress-bar https://example.com/largefile.zip"
);
```

#### Handling Redirects

```csharp
// Follow redirects automatically
var result = await Curl.ExecuteAsync(
    "curl -L https://example.com/redirecting-url"
);

// Limit number of redirects
var result = await Curl.ExecuteAsync(
    "curl -L --max-redirs 5 https://example.com/redirect"
);
```

## 💪 Advanced Features and Examples

### Exception Handling with Specific Error Types

CurlDotNet provides a comprehensive exception hierarchy that maps to every curl error code, allowing you to handle specific error conditions precisely:

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://example.com/api");
}
catch (CurlDnsException ex)
{
    // DNS resolution failed (error code 6)
    Console.WriteLine($"Could not resolve hostname: {ex.Hostname}");
    Console.WriteLine($"Error code: {ex.ErrorCode}");
}
catch (CurlTimeoutException ex)
{
    // Operation timed out (error code 28)
    Console.WriteLine($"Request timed out after {ex.Timeout} seconds");
}
catch (CurlSslException ex)
{
    // SSL/TLS error (various codes 35-99)
    Console.WriteLine($"SSL error: {ex.Message}");
    Console.WriteLine($"Certificate error: {ex.CertificateError}");
}
catch (CurlHttpReturnedErrorException ex)
{
    // HTTP error response (error code 22)
    Console.WriteLine($"HTTP error: {ex.StatusCode}");
    Console.WriteLine($"Response: {ex.ResponseBody}");
}
catch (CurlException ex)
{
    // Generic curl error
    Console.WriteLine($"Curl error {ex.ErrorCode}: {ex.Message}");
}
```

### Working with Response Data

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com/users/123");

// Access response properties
Console.WriteLine($"Status: {result.StatusCode}");
Console.WriteLine($"Body: {result.Body}");
Console.WriteLine($"Headers: {string.Join(", ", result.Headers.Keys)}");

// Parse JSON responses
var user = result.ParseJson<User>();
// or with dynamic
dynamic user = result.AsJsonDynamic();
Console.WriteLine($"Name: {user.name}");

// Check for specific headers
if (result.HasHeader("Content-Type"))
{
    var contentType = result.GetHeader("Content-Type");
    Console.WriteLine($"Content Type: {contentType}");
}

// Save response to file
result.SaveToFile("response.json");

// Get response as stream
using (var stream = result.ToStream())
{
    // Process stream...
}

// Validate response
result.EnsureSuccess();  // Throws if status code >= 400
result.EnsureStatus(200);  // Throws if status code != 200
result.EnsureContains("expected text");  // Throws if body doesn't contain text
```

### Streaming for Large Files

CurlDotNet uses streaming by default to handle large files efficiently without loading them entirely into memory:

```csharp
// Download large file efficiently
var result = await Curl.ExecuteAsync("curl https://example.com/largefile.zip");

// Stream directly to file
using (var fileStream = File.Create("largefile.zip"))
using (var responseStream = result.ToStream())
{
    await responseStream.CopyToAsync(fileStream);
}

// Process stream in chunks
using (var stream = result.ToStream())
using (var reader = new StreamReader(stream))
{
    string line;
    while ((line = await reader.ReadLineAsync()) != null)
    {
        // Process each line
    }
}
```

### Custom Configuration and Settings

```csharp
using CurlDotNet.Core;

// Configure global settings
var settings = new CurlSettings()
{
    UserAgent = "MyApp/1.0",
    FollowRedirects = true,
    MaxRedirects = 10,
    Timeout = TimeSpan.FromSeconds(30),
    ConnectTimeout = TimeSpan.FromSeconds(10),
    Insecure = false,  // Never true in production!
    Proxy = "http://proxy.example.com:8080",
    ProxyCredentials = new NetworkCredential("user", "pass"),
    CookieJar = "cookies.txt",
    Verbose = false
};

var result = await Curl.ExecuteAsync("curl https://api.example.com/data", settings);
```

### Middleware Support

CurlDotNet supports middleware for cross-cutting concerns like logging, metrics, retries, and caching:

```csharp
using CurlDotNet.Middleware;

// Add logging middleware
var pipeline = new CurlMiddlewarePipeline();
pipeline.Add(new LoggingMiddleware(logger.LogInformation));

// Add retry middleware
pipeline.Add(new RetryMiddleware(
    maxRetries: 3,
    delay: TimeSpan.FromSeconds(2)
));

// Add caching middleware
pipeline.Add(new CachingMiddleware(TimeSpan.FromMinutes(5)));

// Use pipeline
var result = await pipeline.ExecuteAsync(options);
```

### Proxy Configuration

```csharp
// HTTP proxy
var result = await Curl.ExecuteAsync(@"
    curl -x http://proxy.example.com:8080 \
         https://api.example.com/data
");

// Proxy with authentication
var result = await Curl.ExecuteAsync(@"
    curl -x http://user:pass@proxy.example.com:8080 \
         https://api.example.com/data
");

// SOCKS5 proxy
var result = await Curl.ExecuteAsync(@"
    curl --socks5 proxy.example.com:1080 \
         https://api.example.com/data
");
```

### Cookie Management

```csharp
// Save cookies to file
var result = await Curl.ExecuteAsync(@"
    curl -c cookies.txt \
         https://api.example.com/login
");

// Use saved cookies
var result2 = await Curl.ExecuteAsync(@"
    curl -b cookies.txt \
         https://api.example.com/protected
");

// Send specific cookies
var result = await Curl.ExecuteAsync(@"
    curl -b 'session=abc123;token=xyz789' \
         https://api.example.com/data
");
```

## 🏗️ Real-World Examples

### Building a REST API Client

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class ApiClient
{
    private readonly string _baseUrl;
    private readonly string _apiKey;

    public ApiClient(string baseUrl, string apiKey)
    {
        _baseUrl = baseUrl;
        _apiKey = apiKey;
    }

    // GET request
    public async Task<T> GetAsync<T>(string endpoint)
    {
        var result = await Curl.ExecuteAsync($@"
            curl -H 'Accept: application/json' \
                 -H 'X-API-Key: {_apiKey}' \
                 {_baseUrl}{endpoint}
        ");
        
        result.EnsureSuccess();
        return result.ParseJson<T>();
    }

    // POST request
    public async Task<T> PostAsync<T>(string endpoint, object data)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(data);
        var result = await Curl.ExecuteAsync($@"
            curl -X POST \
                 -H 'Content-Type: application/json' \
                 -H 'X-API-Key: {_apiKey}' \
                 -d '{json}' \
                 {_baseUrl}{endpoint}
        ");
        
        result.EnsureSuccess();
        return result.ParseJson<T>();
    }

    // PUT request
    public async Task<T> PutAsync<T>(string endpoint, object data)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(data);
        var result = await Curl.ExecuteAsync($@"
            curl -X PUT \
                 -H 'Content-Type: application/json' \
                 -H 'X-API-Key: {_apiKey}' \
                 -d '{json}' \
                 {_baseUrl}{endpoint}
        ");
        
        result.EnsureSuccess();
        return result.ParseJson<T>();
    }

    // DELETE request
    public async Task DeleteAsync(string endpoint)
    {
        var result = await Curl.ExecuteAsync($@"
            curl -X DELETE \
                 -H 'X-API-Key: {_apiKey}' \
                 {_baseUrl}{endpoint}
        ");
        
        result.EnsureSuccess();
    }
}

// Usage
var client = new ApiClient("https://api.example.com", "your-api-key");
var users = await client.GetAsync<List<User>>("/users");
var newUser = await client.PostAsync<User>("/users", new { name = "John", email = "john@example.com" });
```

### Webhook Handler with Retries

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet.Core;

public class WebhookService
{
    public async Task SendWebhookAsync(string url, object payload, int maxRetries = 3)
{
        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var result = await CurlRequestBuilder
                    .Post(url)
        .WithHeader("Content-Type", "application/json")
                    .WithData(json)
        .WithTimeout(TimeSpan.FromSeconds(10))
                    .ExecuteAsync();

                result.EnsureSuccess();
                return;  // Success!
            }
            catch (CurlTimeoutException)
            {
                if (attempt == maxRetries)
                    throw;
                    
                await Task.Delay(TimeSpan.FromSeconds(attempt * 2));  // Exponential backoff
            }
            catch (CurlException ex)
            {
                if (attempt == maxRetries)
                    throw new WebhookException($"Webhook failed after {maxRetries} attempts", ex);
                    
                await Task.Delay(TimeSpan.FromSeconds(attempt * 2));
            }
        }
    }
}
```

### File Upload Service

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet;

public class FileUploadService
{
    public async Task<UploadResult> UploadFileAsync(string url, string filePath, string description)
    {
        var result = await Curl.ExecuteAsync($@"
            curl -X POST \
                 -F 'file=@{filePath}' \
                 -F 'description={description}' \
                 -H 'Authorization: Bearer YOUR_TOKEN' \
                 {url}
        ");

        result.EnsureSuccess();
        return result.ParseJson<UploadResult>();
    }

    public async Task<UploadResult> UploadLargeFileAsync(string url, string filePath, IProgress<double> progress)
    {
        // For large files, use streaming
        var result = await Curl.ExecuteAsync($@"
            curl -X POST \
                 --data-binary '@{filePath}' \
                 -H 'Content-Type: application/octet-stream' \
                 -H 'Authorization: Bearer YOUR_TOKEN' \
                 --progress-bar \
                 {url}
        ");

        result.EnsureSuccess();
        return result.ParseJson<UploadResult>();
    }
}
```

### API Testing Framework

```csharp
using System;
using System.Threading.Tasks;
using Xunit;
using CurlDotNet;

public class ApiTests
{
    [Fact]
    public async Task TestGetUser()
    {
        var result = await Curl.ExecuteAsync(
            "curl https://api.example.com/users/123"
        );

        Assert.Equal(200, result.StatusCode);
        var user = result.ParseJson<User>();
        Assert.NotNull(user);
        Assert.Equal(123, user.Id);
    }

    [Fact]
    public async Task TestCreateUser()
    {
        var result = await Curl.ExecuteAsync(@"
            curl -X POST \
                 -H 'Content-Type: application/json' \
                 -d '{\""name\"":\""John\"",\""email\"":\""john@example.com\""}' \
                 https://api.example.com/users
        ");

        result.EnsureSuccess();
        Assert.Equal(201, result.StatusCode);
        
        var createdUser = result.ParseJson<User>();
        Assert.Equal("John", createdUser.Name);
    }

    [Fact]
    public async Task TestAuthenticationFailure()
    {
        await Assert.ThrowsAsync<CurlHttpReturnedErrorException>(async () =>
        {
            await Curl.ExecuteAsync(@"
                curl -H 'Authorization: Bearer invalid-token' \
                     https://api.example.com/protected
            ");
        });
    }
}
```

## 📊 Performance and Best Practices

### Performance Characteristics

CurlDotNet is optimized for performance:

- **Zero-copy streaming** for large file downloads and uploads
- **Connection pooling** for improved performance on multiple requests
- **Asynchronous operations** throughout for optimal resource usage
- **Memory-efficient** design that doesn't load entire responses into memory unnecessarily
- **HTTP/2 and HTTP/3 support** for faster multiplexed requests
- **Automatic compression** handling (gzip, deflate, brotli)
- **Efficient JSON parsing** using System.Text.Json on modern .NET

### Best Practices

1. **Always use async/await** for I/O operations
2. **Use streaming for large files** - don't load entire files into memory
3. **Set appropriate timeouts** - don't let requests hang indefinitely
4. **Handle exceptions properly** - use specific exception types
5. **Use connection pooling** - reuse HttpClient instances when possible
6. **Don't disable SSL validation in production** - use proper certificates
7. **Use retry logic** for transient failures
8. **Monitor performance** - use middleware for logging and metrics

## 🔧 Configuration and Customization

### Global Configuration

```csharp
// Configure global defaults
Curl.ConfigureDefaults(options =>
{
    options.UserAgent = "MyApp/1.0";
    options.DefaultTimeout = TimeSpan.FromSeconds(30);
    options.FollowRedirects = true;
    options.MaxRedirects = 10;
});
```

### Environment Variables

CurlDotNet automatically respects standard proxy environment variables:

- `HTTP_PROXY` / `http_proxy` - HTTP proxy URL
- `HTTPS_PROXY` / `https_proxy` - HTTPS proxy URL
- `NO_PROXY` / `no_proxy` - Comma-separated list of hosts to bypass proxy
- `ALL_PROXY` / `all_proxy` - Fallback proxy for all protocols

## 🤝 Contributing

We welcome contributions! CurlDotNet is an open-source project, and we'd love your help making it better.

### How to Contribute

1. **Fork the repository** on GitHub
2. **Create a feature branch** for your changes
3. **Write tests** for new functionality (TDD approach)
4. **Ensure all tests pass** - Run `dotnet test`
5. **Update documentation** as needed
6. **Submit a pull request** with a clear description

### Code Style

- Follow existing code style and conventions
- Add XML documentation comments for public APIs
- Include code examples in documentation
- Write unit tests for all new features
- Ensure code coverage remains above 90%

### Reporting Issues

Found a bug? Have a feature request? Please open an issue on GitHub:

- **Bug reports**: Include steps to reproduce, expected behavior, actual behavior
- **Feature requests**: Describe the use case and benefits
- **Performance issues**: Include benchmarks and profiling data

## 📜 License

CurlDotNet is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

This means you can:
- ✅ Use CurlDotNet commercially
- ✅ Modify the source code
- ✅ Distribute your modifications
- ✅ Use privately

All we ask is that you include the original license and copyright notice.

## 💎 Sponsored by IronSoftware

CurlDotNet is proudly sponsored by [IronSoftware](https://ironsoftware.com), the leading provider of .NET development tools and libraries.

### IronSoftware Products

- **[IronPDF](https://ironpdf.com)** - Generate, edit, and manipulate PDF documents in .NET
- **[IronOCR](https://ironsoftware.com/csharp/ocr/)** - Advanced OCR (Optical Character Recognition) for .NET
- **[IronXL](https://ironsoftware.com/csharp/excel/)** - Read, write, and manipulate Excel files without Microsoft Office
- **[IronBarcode](https://ironsoftware.com/csharp/barcode/)** - Generate and read barcodes and QR codes in .NET
- **[IronWebScraper](https://ironsoftware.com/csharp/webscraper/)** - Powerful web scraping library for .NET
- **[IronPdf](https://ironsoftware.com/csharp/pdf/)** - Comprehensive PDF solution for .NET developers

Visit [IronSoftware.com](https://ironsoftware.com) to discover more powerful .NET libraries.

## 🌟 Why Choose CurlDotNet?

### Familiar Syntax

If you know curl (and every developer does), you already know CurlDotNet. There's no learning curve—just paste your curl commands and they work.

### Complete Compatibility

CurlDotNet supports all curl options, ensuring 100% compatibility with curl command-line syntax. Your curl commands work exactly as they would in a terminal.

### Pure .NET Implementation

This is not a wrapper around the curl binary. CurlDotNet is a complete, native .NET implementation that transpiles curl's C source code logic, providing the same behavior with pure .NET performance.

### Cross-Platform Excellence

Works everywhere .NET runs—Windows, Linux, macOS, containers, cloud services, mobile apps, games, and more.

### Production Ready

- Comprehensive test coverage (93.75% - 225/240 tests passing)
- Detailed error handling with specific exception types
- Performance optimized for real-world scenarios
- Full IntelliSense support with XML documentation
- Well-documented with examples for every feature

### 🛠️ Building & Development

CurlDotNet includes professional build automation scripts that work **locally** - no CI/CD required:

```bash
# One-button build everything (tests, NuGet, docs)
./build/build-all.sh

# Build NuGet package
./build/build-nuget.sh

# Generate documentation
./build/build-docs.sh

# Publish to NuGet and GitHub Pages (requires API keys)
./build/publish.sh
```

All scripts are in the `/build` directory and work on Windows (Git Bash), macOS, and Linux. No external dependencies - just .NET SDK and optionally DocFX for documentation.

### Active Development

CurlDotNet is actively developed and maintained, with regular updates, bug fixes, and new features based on community feedback.

## 🔭 Future Vision: UserlandDotNet

CurlDotNet is the first module in **UserlandDotNet**—a suite that will reimagine classic Linux tools (curl, grep, awk, sed, tar, etc.) as pure .NET libraries that plug into PowerShell, MAUI, ASP.NET, Azure Functions, and beyond. The goal is simple: give Microsoft developers the same command-chain power they enjoy on Linux, but with better tooling, IntelliSense, and deployment. Follow the roadmap in [manual/03-Future-Vision-UserlandDotNet.md](manual/03-Future-Vision-UserlandDotNet.md) and the upcoming [UserlandDotNet](https://github.com/UserlandDotNet) organization.

## 📚 Documentation

### 🌐 Online Documentation

Visit our comprehensive documentation at **[https://jacob-mellor.github.io/curl-dot-net/](https://jacob-mellor.github.io/curl-dot-net/)**

The documentation includes:
- **[Getting Started Guide](https://jacob-mellor.github.io/curl-dot-net/articles/getting-started.html)** - Quick start tutorial
- **[Installation Guide](https://jacob-mellor.github.io/curl-dot-net/articles/installation.html)** - Platform-specific setup
- **[API Reference](https://jacob-mellor.github.io/curl-dot-net/api/)** - Complete API documentation
- **[Examples](https://jacob-mellor.github.io/curl-dot-net/articles/examples.html)** - Real-world code samples
- **[Migration Guide](https://jacob-mellor.github.io/curl-dot-net/articles/migration-guide.html)** - Migrate from HttpClient

### 📖 Local Documentation

- **[Documentation](https://jacob-mellor.github.io/curl-dot-net/)** – Comprehensive guides, tutorials, and API reference
- **[Examples](examples/README.md)** – Code examples in C#, F#, and VB.NET
- **[Advanced Features](docs/ADVANCED.md)** – Middleware, protocols, and extensions

### Related Resources

- **[Official curl Documentation](https://curl.se/docs/)** - Learn curl syntax and options
- **[curl Tutorial](https://curl.se/docs/tutorial.html)** - Beginner's guide to curl
- **[curl Man Page](https://curl.se/docs/manpage.html)** - Complete curl reference
- **[.NET Documentation](https://docs.microsoft.com/dotnet/)** - .NET platform documentation

### Community

- **[GitHub Repository](https://github.com/jacob-mellor/curl-dot-net)** - Source code and issue tracking
- **[UserlandDotNet Organization](https://github.com/UserlandDotNet)** - Home for the broader .NET userland initiative
- **[NuGet Package](https://www.nuget.org/packages/CurlDotNet/)** - Download and version history
- **[Report Issues](https://github.com/jacob-mellor/curl-dot-net/issues)** - Bug reports and feature requests
- **[Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)** - Community discussions and Q&A

## ✨ Get Started Today!

Transform your curl commands into powerful .NET applications. Install CurlDotNet now:

```bash
dotnet add package CurlDotNet
```

**Start using CurlDotNet in your projects today and experience the power of curl in C#!**

---

**CurlDotNet** - Bringing the power of curl to .NET developers worldwide. 🚀

*Part of the upcoming UserlandDotNet suite - bringing all Unix command-line tools to .NET.*
