# CurlDotNet Examples

Comprehensive examples demonstrating the power and simplicity of CurlDotNet for C# and .NET developers.

## 📚 Example Categories

### 🎯 Basic Examples
Foundation examples for getting started with CurlDotNet:

- **[00-CompleteSample.cs](BasicExamples/00-CompleteSample.cs)** - Complete end-to-end example
- **[01-SimpleGetRequest.cs](BasicExamples/01-SimpleGetRequest.cs)** - Simple GET requests
- **[02-PostJsonData.cs](BasicExamples/02-PostJsonData.cs)** - POST JSON data
- **[03-ErrorHandling.cs](BasicExamples/03-ErrorHandling.cs)** - Robust error handling
- **[04-SimpleGetFromOld.cs](BasicExamples/04-SimpleGetFromOld.cs)** - Legacy GET example
- **[05-PostJsonFromOld.cs](BasicExamples/05-PostJsonFromOld.cs)** - Legacy POST example

### 🔐 Authentication
Examples for various authentication methods:

- **[01-BearerToken.cs](Authentication/01-BearerToken.cs)** - Bearer token authentication
- **[02-BearerTokenFromOld.cs](Authentication/02-BearerTokenFromOld.cs)** - Legacy bearer token example

### 📁 File Operations
Working with files - uploads, downloads, and more:

- **[01-DownloadFile.cs](FileOperations/01-DownloadFile.cs)** - Download files with progress
- **[02-DownloadFileFromOld.cs](FileOperations/02-DownloadFileFromOld.cs)** - Legacy download example

### 🚀 Advanced Scenarios
Complex scenarios and advanced features:

- **[01-ProxyUsage.cs](AdvancedScenarios/01-ProxyUsage.cs)** - Complete proxy examples (HTTP, SOCKS5, rotating)

### 🌍 Real World Examples
Complete implementations for real-world use cases:

- **[01-GitHubApi.cs](RealWorld/01-GitHubApi.cs)** - Full GitHub API integration
- **[02-WebScraping.cs](RealWorld/02-WebScraping.cs)** - Web scraping with cookies, proxies, pagination

## 🚀 Quick Start

### Install CurlDotNet

```bash
dotnet add package CurlDotNet
```

### Run an Example

```csharp
using CurlDotNet;

// Simple GET request - paste any curl command!
var result = await Curl.ExecuteAsync("curl https://api.github.com");
Console.WriteLine(result.Body);

// Or use the fluent API
var response = await Curl.GetAsync("https://api.github.com/users/octocat")
    .WithHeader("Accept", "application/json")
    .ExecuteAsync();
```

## 📖 Example Highlights

### 🔄 Direct curl Command Execution
```csharp
// Paste curl commands directly from documentation
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/users \
      -H 'Content-Type: application/json' \
      -H 'Authorization: Bearer token123' \
      -d '{"name":"John","email":"john@example.com"}'
");
```

### 🛠️ Fluent Builder Pattern
```csharp
var result = await Curl.PostAsync("https://api.example.com/users")
    .WithBearerToken("token123")
    .WithJson(new { name = "John", email = "john@example.com" })
    .WithTimeout(TimeSpan.FromSeconds(30))
    .ExecuteAsync();
```

### 🌐 Proxy Support
```csharp
// HTTP proxy with authentication
var result = await Curl.GetAsync("https://api.example.com")
    .WithProxy("http://proxy.company.com:8080")
    .WithProxyAuth("username", "password")
    .ExecuteAsync();

// SOCKS5 proxy (Tor)
var torResult = await Curl.GetAsync("https://check.torproject.org")
    .WithSocks5Proxy("socks5://127.0.0.1:9050")
    .ExecuteAsync();
```

### 📊 Progress Tracking
```csharp
await Curl.DownloadFileAsync(
    "https://example.com/large-file.zip",
    "local-file.zip",
    progress: (percent) => Console.WriteLine($"Progress: {percent:F1}%")
);
```

### 🔄 Retry Logic
```csharp
var result = await Curl.GetAsync("https://api.example.com")
    .WithRetry(maxAttempts: 3)
    .WithExponentialBackoff()
    .ExecuteAsync();
```

### 🍪 Cookie Management
```csharp
// Send cookies
var result = await Curl.GetAsync("https://example.com")
    .WithCookie("session=abc123; user=john")
    .ExecuteAsync();

// Extract cookies from response
if (result.Headers.ContainsKey("Set-Cookie"))
{
    var cookies = result.Headers["Set-Cookie"];
    // Use cookies in subsequent requests
}
```

## 📝 Key Concepts

### The CurlResult Object

Every CurlDotNet operation returns a `CurlResult`:

```csharp
var result = await Curl.GetAsync("https://api.example.com");

// Check success (2xx status codes)
if (result.IsSuccess)
{
    string body = result.Body;           // Response body as string
    int status = result.StatusCode;      // HTTP status code
    var headers = result.Headers;        // Response headers dictionary
    byte[] binary = result.BinaryData;   // Binary data (if applicable)
}
```

### Error Handling Patterns

```csharp
// Pattern 1: Check IsSuccess
if (!result.IsSuccess)
{
    Console.WriteLine($"Failed: {result.StatusCode}");
}

// Pattern 2: Use try-catch for specific exceptions
try
{
    var result = await Curl.GetAsync("https://api.example.com")
        .EnsureSuccessStatusCode()
        .ExecuteAsync();
}
catch (CurlHttpException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.Message}");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Timeout after {ex.TimeoutSeconds}s");
}
```

## 🔗 Useful Links

- **[Full Documentation](https://jacob-mellor.github.io/curl-dot-net/)**
- **[API Reference](https://jacob-mellor.github.io/curl-dot-net/api/)**
- **[NuGet Package](https://www.nuget.org/packages/CurlDotNet/)**
- **[GitHub Repository](https://github.com/jacob-mellor/curl-dot-net)**

## 📄 License

All examples are MIT licensed and free to use in your projects.

---

*Built with ❤️ by the CurlDotNet community*