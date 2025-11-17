# CurlDotNet Documentation

<div style="text-align: center; padding: 2rem 0;">
  <h2>Pure .NET Implementation of curl for C#</h2>
  <p style="font-size: 1.2em; color: #666;">No WSL Required • No Native Dependencies • Pure C#</p>
</div>

## Quick Start

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

// It's this simple!
var result = await Curl.ExecuteAsync("curl https://api.github.com");
Console.WriteLine(result.Body);
```

## 🎯 Understanding the Result Object

**The Result object is the heart of CurlDotNet** - every operation returns a `CurlResult` containing everything you need:

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

var result = await Curl.GetAsync("https://api.example.com/data");

// Everything you need is in the result
if (result.IsSuccess)          // Was it successful?
{
    var body = result.Body;     // Get the response text
    var data = result.ParseJson<MyType>();  // Parse as JSON
    result.SaveToFile("data.json");         // Save to disk
}
```

[→ Learn more about the Result object](articles/understanding-result.html)

## 📚 Documentation Sections

<div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 1rem; margin: 2rem 0;">

<div style="border: 1px solid #ddd; padding: 1rem; border-radius: 8px;">
  <h3>⚡ Quickstart</h3>
  <p>Hello World in 5 minutes</p>
  <a href="articles/quickstart.html">Start Here →</a>
</div>

<div style="border: 1px solid #ddd; padding: 1rem; border-radius: 8px;">
  <h3>🎯 Result Object</h3>
  <p>Master the CurlResult API</p>
  <a href="articles/understanding-result.html">Learn More →</a>
</div>

<div style="border: 1px solid #ddd; padding: 1rem; border-radius: 8px;">
  <h3>📖 User Manual</h3>
  <p>Complete guide to using CurlDotNet</p>
  <a href="manual/index.html">Browse Manual →</a>
</div>

<div style="border: 1px solid #ddd; padding: 1rem; border-radius: 8px;">
  <h3>🚀 Getting Started</h3>
  <p>Installation and first steps</p>
  <a href="manual/getting-started/index.html">Start Guide →</a>
</div>

<div style="border: 1px solid #ddd; padding: 1rem; border-radius: 8px;">
  <h3>📘 Tutorials</h3>
  <p>Step-by-step learning path</p>
  <a href="manual/tutorials/index.html">Learn More →</a>
</div>

<div style="border: 1px solid #ddd; padding: 1rem; border-radius: 8px;">
  <h3>🍳 Cookbook</h3>
  <p>Ready-to-use code recipes</p>
  <a href="manual/cookbook/index.html">View Recipes →</a>
</div>

<div style="border: 1px solid #ddd; padding: 1rem; border-radius: 8px;">
  <h3>📋 API Reference</h3>
  <p>Complete API documentation</p>
  <a href="api/index.html">API Docs →</a>
</div>

<div style="border: 1px solid #ddd; padding: 1rem; border-radius: 8px;">
  <h3>🔧 Troubleshooting</h3>
  <p>Solutions to common issues</p>
  <a href="manual/troubleshooting/index.html">Get Help →</a>
</div>

</div>

## Why CurlDotNet?

- **🚀 Familiar Syntax** - Use curl commands you already know
- **📦 Pure .NET** - No WSL, no native dependencies, just C#
- **⚡ Modern Async** - Full async/await support throughout
- **🎯 Type Safe** - Strongly typed responses and options
- **🔧 Flexible APIs** - String, Builder, or Low-level access
- **💪 Production Ready** - Battle-tested in real applications

## Three Ways to Use CurlDotNet

### 1. String API - Use curl commands directly
```csharp
var result = await Curl.ExecuteAsync("curl -X POST https://api.example.com -d 'data'");
```

### 2. Builder API - Fluent interface
```csharp
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.example.com")
    .WithMethod("POST")
    .WithBody("data")
    .ExecuteAsync();
```

### 3. LibCurl API - Low-level control
```csharp
using var curl = new LibCurl();
var result = await curl.PostAsync("https://api.example.com", "data");
```

## Installation

```bash
dotnet add package CurlDotNet
```

Or via Package Manager:
```powershell
Install-Package CurlDotNet
```

## Popular Topics

- [Making Your First Request](manual/tutorials/04-your-first-request.html)
- [Authentication Methods](manual/authentication/index.html)
- [Error Handling](manual/cookbook/beginner/handle-errors.html)
- [Working with JSON](manual/cookbook/beginner/send-json.html)
- [File Uploads](manual/cookbook/beginner/upload-file.html)
- [Performance Optimization](manual/advanced/performance.html)

## Get Help

- 📖 [User Manual](manual/index.html) - Complete documentation
- 💬 [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions) - Ask questions
- 🐛 [Report Issues](https://github.com/jacob-mellor/curl-dot-net/issues) - Report bugs
- 📦 [NuGet Package](https://www.nuget.org/packages/CurlDotNet/) - Package details

---

<div style="text-align: center; margin-top: 3rem; padding: 2rem; background: #f8f9fa;">
  <p><strong>Ready to get started?</strong></p>
  <a href="manual/getting-started/index.html" style="display: inline-block; padding: 0.75rem 2rem; background: #007bff; color: white; text-decoration: none; border-radius: 5px;">Get Started with CurlDotNet →</a>
</div>