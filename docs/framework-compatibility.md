---
title: CurlDotNet .NET Framework Compatibility (4.8, .NET 8/9/10)
description: CurlDotNet .NET framework compatibility: .NET Framework 4.8 via net48 and netstandard2.0, plus .NET 8, 9 and 10. Targets, install notes and TLS tips.
---

# CurlDotNet .NET Framework Compatibility (.NET Framework 4.8, .NET 8/9/10)

CurlDotNet .NET framework compatibility covers .NET Framework 4.8 (through the `net48` and `netstandard2.0` targets) and modern .NET 8, 9 and 10. The package is pure C# with no native dependencies, so the same curl command string runs on every target.

## Package Targets

The NuGet package is built from `src/CurlDotNet/CurlDotNet.csproj` with these target frameworks:

| Target | Runs on |
|--------|---------|
| `netstandard2.0` | .NET Framework 4.6.1+, .NET Core 2.0+, Mono, Xamarin, Unity |
| `net48` | .NET Framework 4.8 (a `net472` target is also built on Windows) |
| `net8.0` | .NET 8 |
| `net9.0` | .NET 9 |
| `net10.0` | .NET 10 |

NuGet picks the closest target automatically. A .NET Framework 4.8 project gets `net48`; a .NET Framework 4.7.2 project gets `net472` or `netstandard2.0`; .NET 8/9/10 projects get their exact target.

## Installation

```bash
dotnet add package CurlDotNet
```

Or in the project file (any version; the package resolves the right target):

```xml
<PackageReference Include="CurlDotNet" Version="*" />
```

## Usage on .NET Framework

Synchronous and asynchronous APIs both work on .NET Framework. Many Framework apps use the synchronous `Curl.Execute`:

```csharp
using System;
using CurlDotNet;

class Program
{
    static void Main()
    {
        var result = Curl.Execute("curl https://httpbin.org/get");
        Console.WriteLine($"Status: {result.StatusCode}");
        Console.WriteLine(result.Body);
    }
}
```

The async form is identical to modern .NET:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -X POST -d 'name=Ada' https://httpbin.org/post");
Console.WriteLine(result.IsSuccess);
```

## Framework-Specific Considerations

### TLS configuration

Older .NET Framework applications may default to TLS 1.0/1.1, which most HTTPS servers now reject. Enable TLS 1.2 once at startup:

```csharp
using System.Net;

ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
```

On .NET 8/9/10 the operating system negotiates TLS and no setting is required.

### JSON

The package references `System.Text.Json` and `Newtonsoft.Json`, so `result.ParseJson<T>()` works on every target without extra packages:

```csharp
using CurlDotNet;

public record Repo(string Name, string Html_Url);

var result = await Curl.ExecuteAsync("curl https://api.github.com/repos/curl/curl");
var repo = result.ParseJson<Repo>();
Console.WriteLine(repo.Name);
```

### HttpClient lifetime

CurlDotNet manages its internal `HttpClient` for you. You do not create or dispose one, and the library can run alongside legacy `WebRequest` code in the same application.

### FTP

FTP and FTPS use `FtpWebRequest`, which is marked obsolete in modern .NET but still ships in the runtime. See [Supported Protocols](supported-protocols.html).

## Testing Framework Compatibility

Locally on macOS or Linux, the .NET Standard 2.0 surface is checked with:

```bash
dotnet script scripts/test-framework-compat.csx
```

A dedicated test project also exists:

```bash
dotnet test tests/CurlDotNet.FrameworkCompat/CurlDotNet.FrameworkCompat.csproj
```

On Windows build agents the `net472` and `net48` targets are compiled directly, which is the authoritative check for .NET Framework.

## Migration from .NET Framework

Moving an application from .NET Framework to .NET 8 or later needs no CurlDotNet code changes. You can remove the `ServicePointManager` TLS line, and switching from `Curl.Execute` to `await Curl.ExecuteAsync` is optional.

## Troubleshooting

### "Could not load file or assembly"

Restore packages and rebuild: `dotnet restore` (or `nuget restore` for packages.config projects). On .NET Framework, make sure the project uses `PackageReference` or that binding redirects were generated.

### SSL/TLS errors on .NET Framework

Add `ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;` at startup, then retry. If a self-signed certificate is involved, `curl -k` skips certificate validation (development only).

### JSON version conflicts

If another dependency pins a different `System.Text.Json`, add an explicit reference to the version you want in your project and let NuGet unify it.

## Related Pages

- [Installation](getting-started/installation.html)
- [Supported Protocols](supported-protocols.html)
- [Common Issues](troubleshooting/common-issues.html)
- Report framework-specific issues: https://github.com/jacob-mellor/curl-dot-net/issues
