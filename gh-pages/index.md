---
layout: home
title: CurlDotNet - Pure .NET curl for C#
---

# CurlDotNet

A pure .NET implementation of curl for C#, supporting .NET Standard 2.0, .NET 8.0, and .NET 10.0.

## Quick Start

```csharp
using CurlDotNet;

var curl = new Curl();
var result = await curl.GetAsync("https://api.example.com/data");
Console.WriteLine(result.Body);
```

## Documentation

- [API Reference](api/) - Complete API documentation
- [Getting Started](getting-started/) - Installation and first steps
- [Tutorials](tutorials/) - Step-by-step guides
- [Examples](examples/) - Code samples

## Installation

```bash
dotnet add package CurlDotNet
```

Or via Package Manager:

```powershell
Install-Package CurlDotNet
```

## Links

- [GitHub Repository](https://github.com/jacob-mellor/curl-dot-net)
- [NuGet Package](https://www.nuget.org/packages/CurlDotNet/)
- [Report Issues](https://github.com/jacob-mellor/curl-dot-net/issues)
