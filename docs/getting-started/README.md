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

- [Installation Guide](installation.md)
- [Tutorials](../tutorials/README.md)
- [API Reference](../api/README.md)
- [Examples](../examples/README.md)
