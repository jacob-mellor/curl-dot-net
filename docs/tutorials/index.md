---
title: "CurlDotNet Tutorials: Learn curl in C# Step by Step"
description: "CurlDotNet tutorials for beginners: 14 step-by-step lessons on running curl commands in C#, from your first request to JSON, auth, files and debugging."
---

# CurlDotNet Tutorials

CurlDotNet tutorials for beginners: learn curl in C# step by step, from what .NET and curl are through your first `Curl.ExecuteAsync` request, reading results, JSON, headers, authentication, file downloads, forms, cancellation, parallel requests and debugging. All 14 tutorials are listed below in order.

## What is CurlDotNet?

CurlDotNet is a pure .NET implementation of curl. You pass a curl command string to `Curl.ExecuteAsync` and it runs the request inside your C# application, with no shell and no curl binary required.

## Installation

Install from NuGet ([CurlDotNet on NuGet](https://www.nuget.org/packages/CurlDotNet/)):

```bash
dotnet add package CurlDotNet
```

See the [installation guide](../getting-started/installation.html) for project setup.

## Tutorial Series

### Foundation

1. [What is .NET and C#?](01-what-is-dotnet.html) - The platform, the language, and your first program
2. [What is curl?](02-what-is-curl.html) - The curl command-line tool and how its options map to CurlDotNet
3. [Understanding Async/Await](03-what-is-async.html) - Why requests are awaited and how to run several at once

### Getting Started

4. [Your First Request](04-your-first-request.html) - Run `Curl.ExecuteAsync` and read the result
5. [Understanding Results](05-understanding-results.html) - Working with `CurlResult`: status, headers, body
6. [Handling Errors](06-handling-errors.html) - Status codes, `EnsureSuccess`, and `CurlException` types

### Working with Data

7. [JSON for Beginners](07-json-for-beginners.html) - Parsing JSON responses with `ParseJson<T>`
8. [Headers Explained](08-headers-explained.html) - Sending and reading HTTP headers
9. [Authentication Basics](09-authentication-basics.html) - Basic auth, bearer tokens and API keys

### Files and Forms

10. [Files and Downloads](10-files-and-downloads.html) - Downloading with `-o` and uploading with `-F`
11. [Forms and Data](11-forms-and-data.html) - Submitting form data and request bodies

### Advanced Concepts

12. [Cancellation Tokens](12-cancellation-tokens.html) - Stopping requests with `CancellationToken`
13. [Parallel Requests](13-parallel-requests.html) - `Task.WhenAll` and `Curl.ExecuteManyAsync`
14. [Debugging Requests](14-debugging-requests.html) - Verbose output, tracing and timings

## Learning Path

### Complete Beginner?

Start with Tutorial 1 to understand .NET, then work through each tutorial in order.

### Know .NET but New to curl?

Jump to Tutorial 2 to understand curl, then move to Tutorial 4 for practical examples.

### Experienced Developer?

Go straight to Tutorial 4, or skip the tutorials and use the [quickstart](../getting-started/quickstart.html) and the [cookbook](../cookbook/index.html).

## Quick Example

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://api.github.com");
Console.WriteLine(result.StatusCode);
Console.WriteLine(result.Body);
```

## What You'll Learn

By completing these tutorials you will understand:

- The .NET ecosystem and C# basics
- How curl works and why API documentation uses it
- Asynchronous programming patterns
- Making HTTP requests with CurlDotNet
- Handling responses and errors
- Working with JSON, headers, authentication and files

## Additional Resources

- [Cookbook](../cookbook/index.html) - Practical recipes for common tasks
- [API guide](../api-guide/README.html) - The `Curl`, `CurlRequestBuilder` and `CurlResult` APIs
- [Exception reference](../exceptions/index.html) - Every CurlDotNet exception and how to handle it
- [Troubleshooting](../troubleshooting/common-issues.html) - Solutions to common issues
- [GitHub repository](https://github.com/jacob-mellor/curl-dot-net) - Source code and examples

## Need Help?

- Check the [troubleshooting guide](../troubleshooting/common-issues.html) and [FAQ](../troubleshooting/faq.html)
- Ask in [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
- Report problems on [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)

Ready to start? Begin with [What is .NET and C#?](01-what-is-dotnet.html).
