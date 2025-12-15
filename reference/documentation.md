# 📚 CurlDotNet Documentation Overview

Welcome to CurlDotNet! This file provides quick access to all documentation, whether you're browsing on GitHub or our documentation site.

## 🌐 Online Documentation
**Full documentation with search**: https://jacob-mellor.github.io/curl-dot-net/

## 📖 Browse Documentation on GitHub

All documentation is available directly in this repository. You can read it on GitHub without leaving this page:

### 🚀 Getting Started
- [**Installation Guide**](docs/getting-started/installation.html) - Get CurlDotNet installed and configured
- [**Quick Start Tutorial**](docs/tutorials/04-your-first-request.html) - Make your first request in 5 minutes
- [**README**](docs/getting-started/) - Overview of getting started

### 📘 User Manual
Complete user manual with all documentation:

#### Core Concepts
- [**What is .NET?**](docs/tutorials/01-what-is-dotnet.html) - For developers new to .NET
- [**What is curl?**](docs/tutorials/02-what-is-curl.html) - Understanding curl basics
- [**Async Programming**](docs/tutorials/03-what-is-async.html) - Understanding async/await

#### API Documentation
- [**API Guide**](docs/api-guide/) - Complete API reference
- [**String API**](docs/api-guide/curl-class/) - Using curl command strings
- [**Builder API**](docs/api-guide/curl-request-builder/) - Fluent interface
- [**LibCurl API**](docs/api-guide/libcurl/) - Low-level access

#### Practical Examples
- [**Cookbook**](docs/cookbook/) - Ready-to-use code recipes
- [**Simple GET Request**](docs/cookbook/beginner/simple-get.html) - Basic HTTP GET
- [**POST JSON Data**](docs/cookbook/beginner/send-json.html) - Sending JSON
- [**File Upload**](docs/cookbook/beginner/upload-file.html) - Uploading files
- [**Error Handling**](docs/cookbook/beginner/handle-errors.html) - Handle errors properly

#### Authentication
- [**Authentication Guide**](docs/authentication/) - All auth methods
- [**Basic Auth**](docs/authentication/basic-auth.html) - Username/password auth
- [**Bearer Tokens**](docs/authentication/bearer-tokens.html) - Token authentication
- [**API Keys**](docs/authentication/api-keys.html) - API key authentication

#### Advanced Topics
- [**Performance**](docs/advanced/performance.html) - Optimization techniques
- [**Middleware**](docs/advanced/middleware/) - Custom middleware
- [**Testing**](docs/advanced/testing.html) - Testing strategies
- [**Circuit Breaker**](docs/advanced/circuit-breaker.html) - Resilience patterns

#### Migration Guides
- [**From HttpClient**](docs/migration/from-httpclient.html) - Migrate from HttpClient
- [**From RestSharp**](docs/migration/from-restsharp.html) - Migrate from RestSharp

#### Troubleshooting
- [**Common Issues**](docs/troubleshooting/common-issues.html) - Solutions to common problems
- [**FAQ**](docs/troubleshooting/) - Frequently asked questions

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
├── README.md                    # Main documentation index
├── getting-started/            # Installation and setup
│   ├── README.md
│   └── installation.md
├── tutorials/                  # Step-by-step tutorials
│   ├── README.md
│   ├── 01-what-is-dotnet.md
│   ├── 02-what-is-curl.md
│   └── ...
├── api-guide/                  # API reference
│   ├── README.md
│   ├── curl-class/
│   ├── curl-request-builder/
│   └── libcurl/
├── cookbook/                   # Code recipes
│   ├── README.md
│   ├── beginner/
│   ├── intermediate/
│   └── real-world/
├── authentication/            # Auth methods
│   ├── README.md
│   ├── basic-auth.md
│   └── ...
├── troubleshooting/          # Problem solving
│   ├── README.md
│   └── common-issues.md
└── ...
```

## 🎯 Quick Navigation

| I want to... | Go to... |
|-------------|----------|
| Install CurlDotNet | [Installation Guide](docs/getting-started/installation.html) |
| Learn the basics | [Tutorials](docs/tutorials/) |
| See code examples | [Cookbook](docs/cookbook/) |
| Read API docs | [API Guide](docs/api-guide/) |
| Solve a problem | [Troubleshooting](docs/troubleshooting/) |
| Migrate from HttpClient | [Migration Guide](docs/migration/from-httpclient.html) |

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