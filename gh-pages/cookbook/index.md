# CurlDotNet Cookbook

Welcome to the CurlDotNet Cookbook! This collection of practical recipes shows you how to accomplish common tasks using CurlDotNet.

## What is CurlDotNet?

**CurlDotNet** is the pure .NET implementation of curl that allows you to execute curl commands directly in C# applications. Simply paste any curl command from documentation, Stack Overflow, or bash scripts, and it works immediately!

## Installation

Install CurlDotNet from NuGet: **[CurlDotNet on NuGet](https://www.nuget.org/packages/CurlDotNet/)**

```bash
dotnet add package CurlDotNet
```

Or via Package Manager:
```powershell
Install-Package CurlDotNet
```

## Recipe Categories

### 🍳 [Beginner Recipes](beginner/)
Perfect for getting started with CurlDotNet:
- **[Simple GET Request](beginner/simple-get.md)** - Fetch data from an API
- **[Send JSON Data](beginner/send-json.md)** - POST JSON to an endpoint
- **[Handle Errors](beginner/handle-errors.md)** - Robust error handling
- **[Upload Files](beginner/upload-file.md)** - Upload files to servers
- **[Download Files](beginner/download-file.md)** - Download and save files
- **[POST Forms](beginner/post-form.md)** - Submit form data
- **[Call APIs](beginner/call-api.md)** - Build API client applications

### 🥘 Intermediate Recipes (Coming Soon)
Take your skills to the next level:
- API Client Class - Build reusable API clients
- Retry Logic - Handle transient failures
- Progress Tracking - Monitor upload/download progress
- Session Cookies - Maintain session state
- Rate Limiting - Respect API limits
- File Management - Advanced file operations

### 🍱 Advanced Patterns (Coming Soon)
Master complex scenarios:
- Pagination - Handle paginated API responses
- Polling - Implement efficient polling
- Webhooks - Build webhook handlers
- Batch Processing - Process multiple requests efficiently

### 🌍 Real World Examples (Coming Soon)
Complete implementations for popular services:
- GitHub Integration
- Slack Notifications
- Weather API Client
- OAuth Flow Implementation
- Stripe Payments Integration

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

var json = "{\"name\":\"John\",\"email\":\"john@example.com\"}";
var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com/users \
      -H 'Content-Type: application/json' \
      -d '{json}'
");

if (result.IsSuccess)
{
    Console.WriteLine("User created successfully!");
}
```

### Download a File
```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(@"
    curl -o downloaded.pdf https://example.com/file.pdf
");

if (result.IsSuccess)
{
    Console.WriteLine("File downloaded successfully!");
}
```

## How to Use These Recipes

Each recipe in this cookbook:
1. **Explains the problem** it solves
2. **Shows complete, working code** you can copy and paste
3. **Includes error handling** and best practices
4. **Provides variations** for different scenarios
5. **Links to related recipes** for deeper learning

## Recipe Structure

Every recipe follows this format:
- **🎯 What You'll Build** - Clear goal description
- **🥘 Ingredients** - What you need (packages, time, prerequisites)
- **📖 Explanation** - Understanding the concepts
- **🍳 The Recipe** - Step-by-step implementation
- **🎨 Complete Examples** - Full working programs
- **🐛 Troubleshooting** - Common issues and solutions
- **🎓 Best Practices** - Professional tips
- **🚀 Next Steps** - Where to go from here

## Finding the Right Recipe

### By Task
- **Fetching data?** → [Simple GET Request](beginner/simple-get.md)
- **Sending data?** → [Send JSON Data](beginner/send-json.md)
- **Uploading files?** → [Upload Files](beginner/upload-file.md)
- **Downloading files?** → [Download Files](beginner/download-file.md)
- **Submitting forms?** → [POST Forms](beginner/post-form.md)
- **Building an API client?** → [Call APIs](beginner/call-api.md)
- **Handling errors?** → [Handle Errors](beginner/handle-errors.md)

### By HTTP Method
- **GET** → [Simple GET Request](beginner/simple-get.md)
- **POST** → [Send JSON Data](beginner/send-json.md), [POST Forms](beginner/post-form.md)
- **PUT/PATCH** → [Send JSON Data](beginner/send-json.md) (includes PUT examples)
- **DELETE** → [Call APIs](beginner/call-api.md) (includes DELETE examples)

### By Feature
- **Authentication** → [Call APIs](beginner/call-api.md)
- **Headers** → Most recipes include header examples
- **Error Handling** → [Handle Errors](beginner/handle-errors.md)
- **File Operations** → [Upload Files](beginner/upload-file.md), [Download Files](beginner/download-file.md)

## Contributing Recipes

We welcome recipe contributions! If you have a useful pattern or example:
1. Fork the repository
2. Add your recipe following our format
3. Submit a pull request

## Additional Resources

- **[Tutorials](../tutorials/)** - Step-by-step learning path
- **[API Reference](../api/)** - Complete API documentation
- **[Getting Started Guide](../getting-started/)** - Installation and setup
- **[Troubleshooting](../troubleshooting/)** - Common issues and solutions

## Support

- **Questions?** Ask in [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
- **Found a bug?** Report it on [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)
- **Need help?** Check our [Troubleshooting Guide](../troubleshooting/)

---

Ready to cook? Start with a **[Simple GET Request](beginner/simple-get.md)** →