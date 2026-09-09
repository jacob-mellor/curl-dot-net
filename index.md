# CurlDotNet Documentation

Welcome to **CurlDotNet** - the pure .NET implementation of curl that allows C# developers to execute curl commands directly in their applications.

**Latest Version:** Fully supports .NET 10, .NET 9, and .NET 8 LTS

## What is CurlDotNet?

CurlDotNet eliminates the friction of translating curl commands to C# code. Simply paste any curl command from API documentation, Stack Overflow, or bash scripts directly into your .NET application, and it works immediately.

## Quick Example

```csharp
using CurlDotNet;

// Just paste any curl command - it works!
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

if (result.IsSuccess)
{
    Console.WriteLine(result.Data);
}
```

## Installation

```bash
dotnet add package CurlDotNet
```

## Getting Started

- [Installation Guide](getting-started/installation.html) - Set up CurlDotNet in your project
- [Quick Start](getting-started/quickstart.html) - Make your first request in minutes
- [First Request](getting-started/first-request.html) - Step-by-step walkthrough
- [Configuration](getting-started/configuration.html) - Configure timeouts, SSL, proxies

## Learning Paths

### New to Programming?

1. [What is .NET?](tutorials/01-what-is-dotnet.html)
2. [What is curl?](tutorials/02-what-is-curl.html)
3. [Understanding async/await](tutorials/03-what-is-async.html)
4. [Your First Request](tutorials/04-your-first-request.html)

### Experienced Developer?

1. [Quick Start](getting-started/quickstart.html)
2. [API Guide](api-guide/)
3. [Cookbook](cookbook/)

## Documentation Sections

| Section | Description |
|---------|-------------|
| [Tutorials](tutorials/) | Step-by-step learning for beginners |
| [Cookbook](cookbook/) | Copy-paste recipes for common tasks |
| [API Guide](api-guide/) | Complete API reference with examples |
| [API Reference](api/index.html) | Generated API documentation |
| [Troubleshooting](troubleshooting/) | Solutions to common problems |

## The Three Ways to Use CurlDotNet

### 1. String API (Simplest)

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");
```

### 2. Builder API (Type-Safe)

```csharp
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.example.com")
    .WithHeader("Accept", "application/json")
    .ExecuteAsync();
```

### 3. LibCurl API (Reusable)

```csharp
var curl = new LibCurl()
    .WithBearerToken("token123")
    .WithTimeout(TimeSpan.FromSeconds(30));

var result = await curl.GetAsync("https://api.example.com");
```

## Platform Support

- .NET 10.0 (Preview)
- .NET 9.0
- .NET 8.0 (LTS - Recommended)
- .NET Framework 4.7.2+
- .NET Standard 2.0

## Getting Help

- [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)
- [Troubleshooting Guide](troubleshooting/)
- [FAQ](troubleshooting/faq.html)

---

*CurlDotNet is sponsored by [IronSoftware](https://ironsoftware.com)*
