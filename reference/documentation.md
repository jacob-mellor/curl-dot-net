# 📚 CurlDotNet Documentation Overview

Welcome to CurlDotNet! This file provides quick access to all documentation, whether you're browsing on GitHub or our documentation site.

## 🌐 Online Documentation
**Full documentation with search**: https://jacob-mellor.github.io/curl-dot-net/

## 📖 Browse Documentation on GitHub

All documentation is available directly in this repository. You can read it on GitHub without leaving this page:

### 🚀 Getting Started
- [**Installation Guide**](../getting-started/installation.html) - Get CurlDotNet installed and configured
- [**Quick Start Tutorial**](../tutorials/04-your-first-request.html) - Make your first request in 5 minutes
- [**README**](../getting-started/) - Overview of getting started

### 📘 User Manual
Complete user manual with all documentation:

#### Core Concepts
- [**What is .NET?**](../tutorials/01-what-is-dotnet.html) - For developers new to .NET
- [**What is curl?**](../tutorials/02-what-is-curl.html) - Understanding curl basics
- [**Async Programming**](../tutorials/03-what-is-async.html) - Understanding async/await

#### API Documentation
- [**API Guide**](../api-guide/) - Complete API reference
- [**API Reference**](../api/index.html) - Generated API documentation

#### Practical Examples
- [**Cookbook**](../cookbook/) - Ready-to-use code recipes
- [**Simple GET Request**](../cookbook/beginner/simple-get.html) - Basic HTTP GET
- [**POST JSON Data**](../cookbook/beginner/send-json.html) - Sending JSON
- [**File Upload**](../cookbook/beginner/upload-file.html) - Uploading files
- [**Error Handling**](../cookbook/beginner/handle-errors.html) - Handle errors properly

#### Authentication
- [**Authentication Tutorial**](../tutorials/09-authentication-basics.html) - All auth methods explained
- [**Call API with Auth**](../cookbook/beginner/call-api.html) - Bearer tokens and API keys

#### Migration Guides
- [**From HttpClient**](../migration/httpclient.html) - Migrate from HttpClient
- [**From RestSharp**](../migration/restsharp.html) - Migrate from RestSharp

#### Troubleshooting
- [**Common Issues**](../troubleshooting/common-issues.html) - Solutions to common problems
- [**Error Reference**](../troubleshooting/error-reference.html) - All exception types
- [**FAQ**](../troubleshooting/faq.html) - Frequently asked questions

## 🔍 Quick Code Examples

### Simple GET Request
```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://api.github.com");
Console.WriteLine(result.Body);
```

### POST with JSON
```csharp
var json = @"{""name"":""John Doe"",""email"":""john@example.com""}";
var result = await Curl.ExecuteAsync($@"
    curl -X POST https://api.example.com/users
    -H 'Content-Type: application/json'
    -d '{json}'
");
```

### Using Builder API
```csharp
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.github.com/user")
    .WithHeader("Authorization", "Bearer your-token")
    .WithMethod("GET")
    .ExecuteAsync();
```

## 📁 Documentation Structure

```
docs/
├── index.md                    # Main documentation index
├── getting-started/            # Installation and setup
│   ├── README.md
│   ├── installation.md
│   ├── quickstart.md
│   ├── first-request.md
│   └── configuration.md
├── tutorials/                  # Step-by-step tutorials (14 tutorials)
│   ├── README.md
│   ├── 01-what-is-dotnet.md
│   ├── 02-what-is-curl.md
│   └── ...
├── api-guide/                  # API overview
│   └── README.md
├── api/                        # Generated API reference
│   └── index.md
├── cookbook/                   # Code recipes
│   ├── README.md
│   └── beginner/
├── migration/                  # Migration guides
│   ├── httpclient.md
│   └── restsharp.md
├── troubleshooting/            # Problem solving
│   ├── README.md
│   ├── common-issues.md
│   ├── error-reference.md
│   └── faq.md
└── ...
```

## 🎯 Quick Navigation

| I want to... | Go to... |
|-------------|----------|
| Install CurlDotNet | [Installation Guide](../getting-started/installation.html) |
| Learn the basics | [Tutorials](../tutorials/) |
| See code examples | [Cookbook](../cookbook/) |
| Read API docs | [API Guide](../api-guide/) |
| Solve a problem | [Troubleshooting](../troubleshooting/) |
| Migrate from HttpClient | [Migration Guide](../migration/httpclient.html) |

## 💡 Why Read Docs on GitHub?

- **No build needed** - Read markdown directly
- **Always up-to-date** - See the latest changes
- **Easy navigation** - Use GitHub's file browser
- **Search support** - Use GitHub's search
- **Direct links** - Share specific sections easily

## 🤝 Contributing

Found an issue in the documentation? Want to add an example?
- [Edit on GitHub](https://github.com/jacob-mellor/curl-dot-net/tree/master/docs)
- [Open an issue](https://github.com/jacob-mellor/curl-dot-net/issues)
- [Start a discussion](https://github.com/jacob-mellor/curl-dot-net/discussions)

## 📦 Installation

```bash
dotnet add package CurlDotNet
```

---

**Full Documentation Site**: https://jacob-mellor.github.io/curl-dot-net/
**NuGet Package**: https://www.nuget.org/packages/CurlDotNet/
**GitHub Repository**: https://github.com/jacob-mellor/curl-dot-net