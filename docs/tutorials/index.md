# CurlDotNet Tutorials

Welcome to the CurlDotNet tutorials! These step-by-step guides will help you learn how to use CurlDotNet effectively.

## What is CurlDotNet?

**CurlDotNet** is the pure .NET implementation of curl that allows C# developers to execute curl commands directly in their applications. No translation required, no shell execution, just paste any curl command and it works!

## Installation

Install CurlDotNet from NuGet: **[CurlDotNet on NuGet](https://www.nuget.org/packages/CurlDotNet/)**

```bash
dotnet add package CurlDotNet
```

## Tutorial Series

### Getting Started with .NET and CurlDotNet
1. **[What is .NET and C#?](01-what-is-dotnet.md)** - Understanding the .NET ecosystem
2. **[What is curl?](02-what-is-curl.md)** - Learn about the curl command-line tool
3. **[Understanding Async/Await](03-what-is-async.md)** - Asynchronous programming in C#
4. **[Your First Request](04-your-first-request.md)** - Make your first HTTP request with CurlDotNet

## Learning Path

### Complete Beginner?
Start with tutorial #1 to understand .NET, then progress through each tutorial in order.

### Know .NET but New to curl?
Jump to tutorial #2 to understand curl, then move to tutorial #4 for practical examples.

### Experienced Developer?
Go straight to tutorial #4 to see CurlDotNet in action.

## Quick Example

Here's a simple example to get you started:

```csharp
using CurlDotNet;

// Your first CurlDotNet request
var result = await Curl.ExecuteAsync("curl https://api.github.com");
Console.WriteLine(result.Body);
```

## What You'll Learn

By completing these tutorials, you'll understand:
- ✅ The .NET ecosystem and C# basics
- ✅ How curl works and why it's powerful
- ✅ Asynchronous programming patterns
- ✅ Making HTTP requests with CurlDotNet
- ✅ Handling responses and errors
- ✅ Best practices for API integration

## Additional Resources

- **[Cookbook](../cookbook/)** - Practical recipes for common tasks
- **[API Reference](../api/)** - Complete API documentation
- **[Troubleshooting](../troubleshooting/)** - Solutions to common issues
- **[GitHub Repository](https://github.com/jacob-mellor/curl-dot-net)** - Source code and examples

## Need Help?

- Check our [Troubleshooting Guide](../troubleshooting/)
- Ask questions in [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
- Report issues on [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)

---

Ready to start? Begin with **[What is .NET and C#?](01-what-is-dotnet.md)** →