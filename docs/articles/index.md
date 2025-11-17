# CurlDotNet Documentation Articles

Welcome to **CurlDotNet** - the pure .NET implementation of curl that allows C# developers to execute curl commands directly in their applications!

## What is CurlDotNet?

CurlDotNet brings the power of curl to .NET. Simply paste any curl command from API documentation, Stack Overflow, or bash scripts directly into your C# code, and it works immediately. No translation required, no shell execution, just pure .NET performance.

## Installation

Install CurlDotNet from NuGet: **[CurlDotNet on NuGet](https://www.nuget.org/packages/CurlDotNet/)**

```bash
dotnet add package CurlDotNet
```

Or via Package Manager:
```powershell
Install-Package CurlDotNet
```

## 📚 Articles in This Section

### Available Articles
- **[Getting Started](getting-started.html)** - Complete guide to getting started with CurlDotNet, from installation to your first request
- **[Installation Guide](installation.html)** - Detailed installation instructions for all platforms and environments

## 📂 Documentation Sections

### Getting Started Guide
- **[Complete Getting Started Section](../getting-started/)** - Comprehensive getting started documentation
  - [Installation Guide](../getting-started/installation.md) - Detailed setup instructions
  - [Quick Start](../getting-started/quickstart.md) - Make your first request in 5 minutes
  - [Full Guide](../getting-started/README.md) - Complete getting started documentation

### Tutorials
- [Complete Tutorial Series](../tutorials/README.md) - Step-by-step learning path
- [What is .NET?](../tutorials/01-what-is-dotnet.md) - For beginners new to .NET
- [What is curl?](../tutorials/02-what-is-curl.md) - Understanding curl fundamentals
- [Your First Request](../tutorials/04-your-first-request.md) - Make your first HTTP request

### API Guide
- [Complete API Reference](../api-guide/README.md) - Detailed API documentation
- String API - Simple curl command execution
- Builder API - Fluent interface for request building
- LibCurl API - Low-level curl implementation

### Cookbook
- [Cookbook](../cookbook/README.md) - Practical recipes and examples
- [Simple GET Request](../cookbook/beginner/simple-get.md) - Basic HTTP GET
- [POST JSON Data](../cookbook/beginner/send-json.md) - Sending JSON payloads
- [File Upload/Download](../cookbook/beginner/upload-file.md) - File operations
- [Error Handling](../cookbook/beginner/handle-errors.md) - Handling errors gracefully

### Troubleshooting
- [Common Issues](../troubleshooting/common-issues.md) - Solutions to frequent problems
- [FAQ](../troubleshooting/README.md) - Frequently asked questions

## Quick Examples

### Simple GET Request
```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://api.github.com");
Console.WriteLine(result.Body);
```

### POST with JSON
```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/users
    -H 'Content-Type: application/json'
    -d '{""name"":""John Doe"",""email"":""john@example.com""}'
");
```

### Using Builder API
```csharp
using CurlDotNet;

var result = await new CurlRequestBuilder()
    .WithUrl("https://api.github.com/user")
    .WithMethod("GET")
    .WithHeader("Authorization", "Bearer your-token")
    .ExecuteAsync();
```

## Navigation

- **Beginners**: Start with [Tutorials](../tutorials/README.md)
- **Quick Reference**: Check the [Cookbook](../cookbook/README.md)
- **API Details**: See the [API Guide](../api-guide/README.md)
- **Having Issues?**: Visit [Troubleshooting](../troubleshooting/README.md)

## Why CurlDotNet?

- 🚀 **Pure .NET** - No WSL, no native dependencies
- 📦 **NuGet Package** - Easy installation and updates
- 🔧 **Familiar Syntax** - Use curl commands directly in C#
- 🎯 **Modern .NET** - Full async/await support
- 🛡️ **Type Safe** - Strongly typed responses and options

## Contributing

We welcome contributions! Please see our [GitHub repository](https://github.com/jacob-mellor/curl-dot-net) for more information.

## Support

- 📖 [Documentation](https://jacob-mellor.github.io/curl-dot-net/)
- 💬 [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
- 🐛 [Report Issues](https://github.com/jacob-mellor/curl-dot-net/issues)
- 📦 [NuGet Package](https://www.nuget.org/packages/CurlDotNet/)