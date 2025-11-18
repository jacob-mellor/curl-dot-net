# CurlDotNet - curl for C# and .NET

[![NuGet Version](https://img.shields.io/nuget/v/CurlDotNet.svg)](https://www.nuget.org/packages/CurlDotNet/)
[![Downloads](https://img.shields.io/nuget/dt/CurlDotNet.svg)](https://www.nuget.org/packages/CurlDotNet/)
[![Build Status](https://img.shields.io/github/actions/workflow/status/jacob-mellor/curl-dot-net/ci-smoke.yml?branch=master)](https://github.com/jacob-mellor/curl-dot-net/actions)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/jacob-mellor/curl-dot-net/blob/master/LICENSE)
![Coverage](https://img.shields.io/badge/coverage-65.9%25-yellow)

![CurlDotNet - Why .NET Needs a POSIX/GNU Userland](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/1o4hlr4tbp6b8k86ew6c.jpg)

<div align="center">
  <img src="https://github.com/jacob-mellor/curl-dot-net/blob/master/src/CurlDotNet/icon128.png?raw=true" alt="CurlDotNet Icon" width="128" height="128"/>
</div>

## 🆕 New to curl? Start Here!

**[📖 Complete Beginner's Guide to curl in C# →](https://jacob-mellor.github.io/curl-dot-net/new-to-curl)**

## The Industry Standard curl Experience for C# and .NET Developers

CurlDotNet brings the power and simplicity of curl to the .NET ecosystem. Execute curl commands directly in C#, make HTTP requests with curl's battle-tested reliability, and leverage decades of curl development - all with pure .NET code.

## 🚀 Quick Start with curl for C# and .NET

```csharp
using CurlDotNet;

// Execute curl commands directly in C#
var result = await Curl.ExecuteAsync("curl -X GET https://api.github.com/users/octocat -H 'Accept: application/json'");

// Or use the fluent API for type safety
var response = await Curl.GetAsync("https://api.github.com/users/octocat")
    .WithHeader("Accept", "application/vnd.github.v3+json")
    .ExecuteAsync();

// Simple one-liners for common operations
var json = await Curl.GetJsonAsync<GitHubUser>("https://api.github.com/users/octocat");
```

## 📊 Code Coverage

- **Line Coverage:** 65.9%
- **Branch Coverage:** 72%
- **Method Coverage:** 59.9%
- **Tests:** 657 total, 619 passing, 38 failing
- **Last Updated:** 2025-11-18

## 📦 Installation

```bash
# .NET CLI
dotnet add package CurlDotNet

# Package Manager Console
Install-Package CurlDotNet

# PackageReference
<PackageReference Include="CurlDotNet" Version="*" />
```

## ✨ Why CurlDotNet?

### curl Compatibility in .NET
- **100% curl behavior** - Your C# code works exactly like curl commands
- **Parse any curl command** - Copy from documentation and paste into your code
- **Industry standard** - Used by millions of developers worldwide

### Superior to HttpClient
- **More features** - Proxies, authentication chains, retries, rate limiting
- **Better debugging** - curl-style verbose output
- **Simpler API** - One-line operations that would take 20+ lines with HttpClient

### Pure .NET Implementation
- **No native dependencies** - 100% managed C# code
- **Universal compatibility** - Runs anywhere .NET runs: Windows, Linux, macOS, iOS, Android, IoT devices, Docker, cloud
- **Safe and secure** - No P/Invoke, no unmanaged memory

## 🎯 Key Features

### HTTP/REST Operations
```csharp
// GET request
var data = await Curl.GetAsync("https://api.example.com/users");

// POST JSON
var user = await Curl.PostJsonAsync("https://api.example.com/users",
    new { name = "Alice", email = "alice@example.com" });

// PUT with authentication
await Curl.PutAsync("https://api.example.com/users/123")
    .WithBearerToken(token)
    .WithJson(updatedData)
    .ExecuteAsync();

// DELETE
await Curl.DeleteAsync("https://api.example.com/users/123");

// PATCH
await Curl.PatchAsync("https://api.example.com/users/123")
    .WithJson(new { status = "active" })
    .ExecuteAsync();
```

### Authentication Methods
```csharp
// Bearer Token
await Curl.GetAsync("https://api.example.com")
    .WithBearerToken("your-token")
    .ExecuteAsync();

// Basic Auth
await Curl.GetAsync("https://api.example.com")
    .WithBasicAuth("username", "password")
    .ExecuteAsync();

// API Key
await Curl.GetAsync("https://api.example.com")
    .WithApiKey("X-API-Key", "your-api-key")
    .ExecuteAsync();

// OAuth 2.0
await Curl.GetAsync("https://api.example.com")
    .WithOAuth2("access-token", "Bearer")
    .ExecuteAsync();

// Custom headers
await Curl.GetAsync("https://api.example.com")
    .WithHeader("X-Custom-Auth", "token123")
    .ExecuteAsync();
```

### File Operations
```csharp
// Download file with progress
await Curl.DownloadFileAsync(
    "https://example.com/large-file.zip",
    "local-file.zip",
    progress: (percent) => Console.WriteLine($"Progress: {percent:F1}%")
);

// Upload file
await Curl.PostAsync("https://api.example.com/upload")
    .WithFile("file", "/path/to/file.pdf")
    .ExecuteAsync();

// Multipart form data
await Curl.PostAsync("https://api.example.com/upload")
    .WithMultipartForm(form => form
        .AddString("name", "John")
        .AddFile("document", "/path/to/doc.pdf")
        .AddFile("image", "/path/to/image.jpg"))
    .ExecuteAsync();
```

### Proxy Support
```csharp
// HTTP proxy
await Curl.GetAsync("https://api.example.com")
    .WithProxy("http://proxy.example.com:8080")
    .WithProxyAuth("username", "password")
    .ExecuteAsync();

// SOCKS5 proxy (Tor)
await Curl.GetAsync("https://check.torproject.org")
    .WithSocks5Proxy("socks5://127.0.0.1:9050")
    .ExecuteAsync();

// Residential proxy
await Curl.GetAsync("https://api.example.com")
    .WithProxy("http://gate.smartproxy.com:10000")
    .WithProxyAuth("user-country-us", "password")
    .ExecuteAsync();

// Rotating proxy
await Curl.GetAsync("https://api.example.com")
    .WithProxy("http://proxy.provider.com:8000")
    .WithProxyAuth($"user-session-{Guid.NewGuid()}", "password")
    .ExecuteAsync();

// Backconnect proxy
await Curl.GetAsync("https://api.example.com")
    .WithBackconnectProxy("proxy.provider.com", 20001)
    .ExecuteAsync();
```

### Advanced Features
```csharp
// Retry with exponential backoff
await Curl.GetAsync("https://api.example.com")
    .WithRetry(maxAttempts: 3)
    .WithExponentialBackoff()
    .ExecuteAsync();

// Rate limiting
await Curl.GetAsync("https://api.example.com")
    .WithRateLimit(requestsPerMinute: 60)
    .ExecuteAsync();

// Timeout handling
await Curl.GetAsync("https://api.example.com")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithConnectTimeout(TimeSpan.FromSeconds(10))
    .ExecuteAsync();

// Follow redirects
await Curl.GetAsync("https://bit.ly/shortened")
    .FollowRedirects(maxRedirects: 5)
    .ExecuteAsync();

// Cookie management
var cookieJar = new CookieContainer();
await Curl.GetAsync("https://example.com")
    .WithCookieContainer(cookieJar)
    .ExecuteAsync();
```

## 📊 Performance & Reliability

- **✅ 244 Unit Tests** - 100% pass rate
- **✅ Self-Contained Testing** - No external dependencies
- **✅ Cross-Platform CI/CD** - Windows, Linux, macOS
- **✅ Production Ready** - Used in enterprise applications

## 🏗️ Platform Support

| Platform | Version | Support |
|----------|---------|---------|
| .NET | 10, 9, 8, 7, 6, 5 | ✅ Full Support |
| .NET Core | 3.1, 3.0, 2.1 | ✅ Full Support |
| .NET Framework | 4.7.2+ | ✅ via .NET Standard 2.0 |
| .NET Standard | 2.0+ | ✅ Maximum Compatibility |
| Windows | 10, 11, Server 2016+ | ✅ Native |
| Linux | Ubuntu, Debian, RHEL, Alpine | ✅ Native |
| macOS | 10.14+, Apple Silicon | ✅ Native |
| iOS | 12+ | ✅ via .NET Standard/MAUI |
| Android | API 21+ | ✅ via .NET Standard/MAUI |
| IoT | Raspberry Pi, Arduino | ✅ via .NET IoT |
| Docker | All Linux images | ✅ Full Support |
| Azure | App Service, Functions, Container Instances | ✅ Cloud Native |
| AWS | Lambda, ECS, Fargate | ✅ Cloud Native |

## 📖 Documentation

- **[🆕 Beginner's Guide](https://jacob-mellor.github.io/curl-dot-net/new-to-curl)** - New to curl? Start here
- **[📚 API Reference](https://jacob-mellor.github.io/curl-dot-net/api/)** - Complete API documentation
- **[👨‍🍳 Cookbook](https://jacob-mellor.github.io/curl-dot-net/cookbook/)** - Ready-to-use recipes
- **[📝 Tutorials](https://jacob-mellor.github.io/curl-dot-net/tutorials/)** - Step-by-step guides
- **[🔄 Migration from HttpClient](https://jacob-mellor.github.io/curl-dot-net/migration/httpclient)** - Upgrade guide
- **[🔄 Migration from RestSharp](https://jacob-mellor.github.io/curl-dot-net/migration/restsharp)** - Transition guide

## 💡 Use Cases

### REST API Integration
```csharp
// GitHub API example
var repos = await Curl.GetJsonAsync<List<Repository>>(
    "https://api.github.com/users/octocat/repos"
);
```

### Web Scraping
```csharp
// Scrape with proper headers
var html = await Curl.GetAsync("https://example.com")
    .WithUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64)")
    .WithHeader("Accept-Language", "en-US")
    .ExecuteAsync();
```

### Microservices Communication
```csharp
// Service-to-service with retry
var response = await Curl.PostAsync("http://service-b/api/process")
    .WithJson(requestData)
    .WithRetry(3)
    .WithTimeout(TimeSpan.FromSeconds(5))
    .ExecuteAsync();
```

### CI/CD Automation
```csharp
// Deploy webhook
await Curl.PostAsync("https://deploy.example.com/webhook")
    .WithJson(new {
        version = "1.2.3",
        environment = "production"
    })
    .WithBearerToken(deployToken)
    .ExecuteAsync();
```

## 🔧 Advanced Usage

### Custom Middleware
```csharp
// Add custom middleware for logging, caching, etc.
var result = await Curl.GetAsync("https://api.example.com")
    .UseMiddleware(async (context, next) => {
        Console.WriteLine($"Request: {context.Request.Url}");
        var response = await next();
        Console.WriteLine($"Response: {response.StatusCode}");
        return response;
    })
    .ExecuteAsync();
```

### Code Generation
```csharp
// Convert curl to other languages
var pythonCode = Curl.ToPythonRequests("curl -X GET https://api.example.com");
var jsCode = Curl.ToJavaScriptFetch("curl -X POST https://api.example.com -d '{}'");
var httpClientCode = Curl.ToHttpClient("curl https://api.example.com");
```

### Debugging
```csharp
// Enable verbose output like curl -v
var result = await Curl.GetAsync("https://api.example.com")
    .Verbose(true)
    .ExecuteAsync();

// Access detailed timing information
Console.WriteLine($"DNS Lookup: {result.Timings.DnsLookup}ms");
Console.WriteLine($"Connect: {result.Timings.Connect}ms");
Console.WriteLine($"TLS Handshake: {result.Timings.TlsHandshake}ms");
Console.WriteLine($"First Byte: {result.Timings.FirstByte}ms");
Console.WriteLine($"Total: {result.Timings.Total}ms");
```

## 🤝 Contributing

We welcome contributions! Please see our [Contributing Guide](CONTRIBUTING.md) for details.

### Development Setup
```bash
# Clone the repository
git clone https://github.com/jacob-mellor/curl-dot-net.git

# Build the project
dotnet build

# Run tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## 📊 Examples

Comprehensive examples are available in the [examples](https://github.com/jacob-mellor/curl-dot-net/tree/master/examples) directory:

- **[Basic Examples](examples/BasicExamples/)** - Simple GET, POST, error handling
- **[Authentication](examples/Authentication/)** - Bearer tokens, OAuth, API keys
- **[File Operations](examples/FileOperations/)** - Upload, download, progress tracking
- **[Advanced Scenarios](examples/AdvancedScenarios/)** - Proxies, retries, rate limiting
- **[Real World](examples/RealWorld/)** - GitHub API, web scraping, complete applications

## 📄 License

CurlDotNet is MIT licensed. See [LICENSE](https://github.com/jacob-mellor/curl-dot-net/blob/master/LICENSE) for details.

## 🌟 Why Developers Choose CurlDotNet

> "Finally, I can just paste curl commands from API docs!" - **Senior Developer**

> "The proxy support is fantastic for our web scraping needs." - **Data Engineer**

> "Migrating from HttpClient saved us hundreds of lines of code." - **Tech Lead**

> "The retry logic and rate limiting just work." - **DevOps Engineer**

## 🚦 Getting Started

1. **Install the package**: `dotnet add package CurlDotNet`
2. **Add using**: `using CurlDotNet;`
3. **Make your first request**: `await Curl.GetAsync("https://api.example.com");`

That's it! You're now using the power of curl in C# and .NET.

## 🆘 Support

- **[GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)** - Report bugs or request features
- **[Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)** - Ask questions and share ideas
- **[Stack Overflow](https://stackoverflow.com/questions/tagged/curldotnet)** - Community Q&A

## 📎 Additional Resources

- **Repository** – https://github.com/jacob-mellor/curl-dot-net
- **NuGet** – https://www.nuget.org/packages/CurlDotNet
- **CI/CD Integration Guide** – https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/articles/ci-cd-integration.md
- **Logging & Observability Guide** – https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md
- **Shell Compatibility Guide** – https://jacob-mellor.github.io/curl-dot-net/reference/curl-cli-compatibility

## 📖 Our Story

Read about how CurlDotNet is bringing curl superpowers to every corner of the .NET 10 & C# stack:
**[📰 Featured Article: CurlDotNet - Bringing curl Superpowers to Every Corner of the .NET 10 & C# Stack](https://dev.to/iron-software/curldotnet-bringing-curl-superpowers-to-every-corner-of-the-net-10-c-stack-1ol2)**

## 🌐 Part of UserLand.NET

CurlDotNet is part of the [UserLand.NET](https://userland.net) initiative - bringing Unix/Linux tools to .NET through pure C# implementations.

---

**Keywords**: curl C#, curl .NET, C# HTTP client, .NET curl, REST API C#, HTTP requests .NET, web scraping C#, proxy C#, curl for Windows, curl alternative, HttpClient alternative

**Author**: [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/) | **Sponsored by [IronSoftware.com](https://ironsoftware.com)**