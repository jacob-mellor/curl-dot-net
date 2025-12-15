# 📚 CurlDotNet Documentation Hub

Welcome to **CurlDotNet** - the pure .NET implementation of curl that allows C# developers to execute curl commands directly in their applications!

**✅ Latest Version:** Fully supports .NET 10, .NET 9, and .NET 8 LTS

## What is CurlDotNet?

CurlDotNet eliminates the friction of translating curl commands to C# code. Simply paste any curl command from API documentation, Stack Overflow, or bash scripts directly into your .NET application, and it works immediately.

## Installation

Install CurlDotNet from NuGet: **[CurlDotNet on NuGet](https://www.nuget.org/packages/CurlDotNet/)**

```bash
dotnet add package CurlDotNet
```

Or via Package Manager:
```powershell
Install-Package CurlDotNet
```

## 🚀 Quick Start Paths

### 🆕 Complete Beginner?
**"I'm new to .NET and programming"**
1. Start here → [What is .NET and C#?](tutorials/01-what-is-dotnet.html)
2. Then → [Your First Request](tutorials/04-your-first-request.html)
3. Next → [Explore Recipes](cookbook/README.html)

### 💼 Experienced Developer?
**"I know .NET, just show me how to use CurlDotNet"**
1. Jump to → [Installation](getting-started/installation.html)
2. Check out → [API Guide](api-guide/README.html)
3. Browse → [Cookbook](cookbook/README.html)

### 🔄 Migrating from Another Library?
**"I'm switching from HttpClient/RestSharp/etc."**
- See the [API Guide](api-guide/README.html) to understand CurlDotNet

## 📖 Documentation Structure

### [🎓 Tutorials](tutorials/README.html)
**Learn the basics in plain English**
- No prior .NET knowledge required
- Step-by-step explanations
- Lots of analogies and examples
- [Start Tutorial Series →](tutorials/README.html)

### [👨‍🍳 Cookbook](cookbook/README.html)
**"How do I..." recipes**
- Task-focused solutions
- Copy-paste ready code
- Real-world scenarios
- [Browse Recipes →](cookbook/README.html)

### [📘 API Guide](api-guide/README.html)
**Complete API reference with examples**
- Every class and method documented
- Multiple examples per feature
- Best practices included
- [Explore API →](api-guide/README.html)

### [👨‍💻 Advanced Features](api-guide/README.html)
**Advanced topics and techniques**
- Working with middleware
- Custom HTTP clients
- Performance optimization
- Testing strategies

## 🎯 Common Tasks

### Basic Operations
- [Make a GET request](cookbook/beginner/simple-get.html)
- [POST JSON data](cookbook/beginner/send-json.html)
- [Download a file](cookbook/beginner/download-file.html)
- [Upload a file](cookbook/beginner/upload-file.html)
- [Handle errors](cookbook/beginner/handle-errors.html)

### Authentication
- [Bearer tokens](cookbook/beginner/call-api.html)
- [API keys](cookbook/beginner/call-api.html)

### Working with APIs
- [Call a REST API](cookbook/beginner/call-api.html)
- [Build API clients](api-guide/README.html)
- [Error handling](cookbook/beginner/handle-errors.html)

### Advanced Features
- [API Reference](api-guide/README.html)
- [Curl String API](api-guide/README.html)
- [Builder API](api-guide/README.html)

## 🔍 Quick Reference

### The Three Ways to Use CurlDotNet

#### 1. String API (Simplest)
```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");
```

#### 2. Builder API (Type-Safe)
```csharp
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.example.com")
    .WithHeader("Accept", "application/json")
    .ExecuteAsync();
```

#### 3. LibCurl API (Reusable)
```csharp
var curl = new LibCurl()
    .WithBearerToken("token123")
    .WithTimeout(TimeSpan.FromSeconds(30));

var result = await curl.GetAsync("https://api.example.com");
```

## 📚 Learning Paths

### Path 1: Absolute Beginner
1. [What is .NET?](tutorials/01-what-is-dotnet.html)
2. [What is curl?](tutorials/02-what-is-curl.html)
3. [Understanding async/await](tutorials/03-what-is-async.html)
4. [Your first request](tutorials/04-your-first-request.html)
5. [API Guide](api-guide/README.html)
6. [Cookbook](cookbook/README.html)

### Path 2: Web Developer
1. [Installation](getting-started/installation.html)
2. [Cookbook](cookbook/README.html)
3. [API Guide](api-guide/README.html)
4. [Error handling](cookbook/beginner/handle-errors.html)
5. [API client patterns](cookbook/beginner/call-api.html)

### Path 3: Enterprise Developer
1. [API Reference](api-guide/README.html)
2. [Error handling](cookbook/beginner/handle-errors.html)
3. [API Guide](api-guide/README.html)
4. [Troubleshooting](troubleshooting/README.html)
5. [Best practices](cookbook/README.html)

## 🆘 Getting Help

### Can't Find Something?
- Use the [search](#) feature
- Check [Troubleshooting](troubleshooting/README.html)
- Browse [Common Issues](troubleshooting/common-issues.html)

### Still Stuck?
- [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/curldotnet)
- [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)

## 📊 Quick Stats

- **300+** curl options supported
- **91** exception types for precise error handling
- **100%** pure .NET - no native dependencies
- **95%** test coverage
- Works on **.NET Framework 4.7.2+**, **.NET Core 2.0+**, **.NET 5-10**

## 🎉 Welcome to CurlDotNet!

Ready to start? Choose your path above and let's begin making HTTP requests the easy way!

---
*Documentation version: 1.0.1 | Last updated: November 2024*