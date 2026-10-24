---
title: "Getting Started with CurlDotNet - curl in C#"
description: "Get started with CurlDotNet: install the package, run your first curl command in C#, and configure timeouts, redirects, proxies and TLS."
---

# Getting Started

Quick start guide for CurlDotNet.

## Installation

### NuGet Package Manager
```bash
Install-Package CurlDotNet
```

### .NET CLI
```bash
dotnet add package CurlDotNet
```

### PackageReference
```xml
<PackageReference Include="CurlDotNet" Version="*" />
```

## First Request

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://api.github.com");
Console.WriteLine($"Status: {result.StatusCode}");
Console.WriteLine($"Body: {result.Body}");
```

## Next Steps

- [Installation Guide](installation.html)
- [Tutorials](../tutorials/)
- [API Reference](../api-guide/)
- [Cookbook Recipes](../cookbook/)
