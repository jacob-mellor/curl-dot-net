---
title: CurlDotNet Articles - CI/CD and Patterns for curl in C#
description: CurlDotNet articles covering CI/CD integration, implementation patterns and tutorial projects for running curl commands in C# and .NET.
---

# CurlDotNet Articles: CI/CD Integration and Implementation Patterns for curl in C#

CurlDotNet articles cover CI/CD integration and implementation patterns for running curl commands in C#. CurlDotNet is a pure .NET library: you paste a curl command string from API documentation into your code and `Curl.ExecuteAsync` runs it, with no shell and no native curl binary.

## Installation

Install from NuGet: [CurlDotNet on NuGet](https://www.nuget.org/packages/CurlDotNet/)

```bash
dotnet add package CurlDotNet
```

## Articles in This Section

- [CI/CD Integration](ci-cd-integration.html) - Run curl-based smoke tests inside GitHub Actions, Azure DevOps and containers
- [Implementation Patterns](userland-patterns.html) - Reusable C# patterns: observability, retries, environment matrices, replay harnesses
- [Tutorial Projects](userland-tutorials.html) - Small end-to-end projects (minimal APIs, CLIs, mock servers) built on CurlDotNet

## Other Documentation Sections

### Getting Started
- [Installation](../getting-started/installation.html) - Add the package and verify it works
- [Quick Start](../getting-started/quickstart.html) - Your first request in a few minutes
- [First Request](../getting-started/first-request.html) - Walkthrough of a complete request and response

### Tutorials
- [Tutorial Series](../tutorials/) - Step-by-step learning path
- [What is .NET?](../tutorials/01-what-is-dotnet.html) - For developers new to .NET
- [What is curl?](../tutorials/02-what-is-curl.html) - curl fundamentals
- [Your First Request](../tutorials/04-your-first-request.html) - Make your first HTTP request

### API and Reference
- [API Guide](../api-guide/README.html) - String API, builder API and LibCurl instance API
- [Reference](../reference/) - libcurl mapping and curl CLI flag compatibility
- [Framework Compatibility](../framework-compatibility.html) - .NET Framework 4.8, .NET 8/9/10
- [Supported Protocols](../supported-protocols.html) - HTTP, HTTPS, FTP, FTPS, file

### Cookbook
- [Cookbook](../cookbook/) - Practical recipes
- [Simple GET Request](../cookbook/beginner/simple-get.html)
- [POST JSON Data](../cookbook/beginner/send-json.html)
- [Upload a File](../cookbook/beginner/upload-file.html)
- [Handle Errors](../cookbook/beginner/handle-errors.html)

### Troubleshooting
- [Common Issues](../troubleshooting/common-issues.html)
- [FAQ](../troubleshooting/faq.html)

## Quick Examples

### Run a curl command string

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://api.github.com");
Console.WriteLine(result.StatusCode);
Console.WriteLine(result.Body);
```

### POST JSON

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(@"curl -X POST https://httpbin.org/post \
    -H 'Content-Type: application/json' \
    -d '{""name"":""Ada"",""role"":""admin""}'");
Console.WriteLine(result.IsSuccess);
```

### Builder API

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Get("https://api.github.com/user")
    .WithBearerToken("your-token")
    .WithHeader("Accept", "application/vnd.github+json")
    .ExecuteAsync();
Console.WriteLine(result.StatusCode);
```

## Support

- [Documentation site](https://jacob-mellor.github.io/curl-dot-net/)
- [GitHub repository](https://github.com/jacob-mellor/curl-dot-net)
- [Report an issue](https://github.com/jacob-mellor/curl-dot-net/issues)
- [NuGet package](https://www.nuget.org/packages/CurlDotNet/)
