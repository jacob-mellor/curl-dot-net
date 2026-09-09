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
- **[Getting Started](../getting-started/)** - Complete guide to getting started with CurlDotNet, from installation to your first request
- **[Installation Guide](../getting-started/installation.html)** - Detailed installation instructions for all platforms and environments
- **[CI/CD Integration](ci-cd-integration.html)** - Run CurlDotNet smoke tests inside GitHub Actions, Azure DevOps, or any pipeline

## 📂 Documentation Sections

### Getting Started Guide
- **[Complete Getting Started Section](../getting-started/)** - Comprehensive getting started documentation
  - [Installation Guide](../getting-started/installation.html) - Detailed setup instructions
  - [Quick Start](../getting-started/quickstart.html) - Make your first request in 5 minutes
  - [Full Guide](../getting-started/) - Complete getting started documentation

### Tutorials
- [Complete Tutorial Series](../tutorials/) - Step-by-step learning path
- [What is .NET?](../tutorials/01-what-is-dotnet.html) - For beginners new to .NET
- [What is curl?](../tutorials/02-what-is-curl.html) - Understanding curl fundamentals
- [Your First Request](../tutorials/04-your-first-request.html) - Make your first HTTP request

### API Guide
- [Complete API Reference](../api-guide/) - Detailed API documentation
- String API - Simple curl command execution
- Builder API - Fluent interface for request building
- LibCurl API - Low-level curl implementation

### Cookbook
- [Cookbook](../cookbook/) - Practical recipes and examples
- [Simple GET Request](../cookbook/beginner/simple-get.html) - Basic HTTP GET
- [POST JSON Data](../cookbook/beginner/send-json.html) - Sending JSON payloads
- [File Upload/Download](../cookbook/beginner/upload-file.html) - File operations
- [Error Handling](../cookbook/beginner/handle-errors.html) - Handling errors gracefully

### Troubleshooting
- [Common Issues](../troubleshooting/common-issues.html) - Solutions to frequent problems
- [FAQ](../troubleshooting/) - Frequently asked questions

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

- **Beginners**: Start with [Tutorials](../tutorials/)
- **Quick Reference**: Check the [Cookbook](../cookbook/)
- **API Details**: See the [API Guide](../api-guide/)
- **Having Issues?**: Visit [Troubleshooting](../troubleshooting/)

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
