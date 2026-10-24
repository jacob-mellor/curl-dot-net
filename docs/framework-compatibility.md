---
title: "CurlDotNet .NET Framework Compatibility (4.8, .NET 8/9/10)"
description: "CurlDotNet .NET framework compatibility: .NET Framework 4.8 via net48 and netstandard2.0, plus .NET 8, 9 and 10. Targets, install notes and TLS tips."
---

# CurlDotNet .NET Framework Compatibility (.NET Framework 4.8, .NET 8/9/10)

CurlDotNet .NET framework compatibility covers .NET Framework 4.7.2 and 4.8 (through the `net472`, `net48` and `netstandard2.0` targets) and modern .NET 8, 9 and 10. The package is pure C# with no native dependencies, so the same curl command string runs on every target.

## Package Targets

The NuGet package is built from `src/CurlDotNet/CurlDotNet.csproj` with these target frameworks:

| Target | Runs on |
|--------|---------|
| `netstandard2.0` | .NET Framework 4.6.1+, .NET Core 2.0+, Mono, Xamarin, Unity |
| `net472` | .NET Framework 4.7.2 |
| `net48` | .NET Framework 4.8 |
| `net8.0` | .NET 8 |
| `net9.0` | .NET 9 |
| `net10.0` | .NET 10 |

NuGet picks the closest target automatically. A .NET Framework 4.8 project gets `net48`; a .NET Framework 4.7.2 project gets `net472`; .NET 8/9/10 projects get their exact target; .NET 6/7, Mono and Unity get `netstandard2.0`. The published package is built on Windows, so it contains all six targets.

Dependencies are deliberately small: `System.Text.Json` and `Newtonsoft.Json` on every target, plus `Microsoft.CSharp` on `netstandard2.0` and `System.Net.Http` on `net472`/`net48`. There is no Ninject, System.CommandLine or other framework dependency.

## What Differs on .NET Standard 2.0 and .NET Framework

The same commands parse and run everywhere, but on .NET 8 and later CurlDotNet opens the connections itself (through `SocketsHttpHandler`'s connect callback) and sees the exact bytes on the wire, while the `netstandard2.0`, `net472` and `net48` builds rely on `HttpClient`'s parsed headers. That costs a few wire-level curl features on the older targets:

| Feature | .NET 8/9/10 | .NET Standard 2.0 / .NET Framework |
|---------|-------------|-------------------------------------|
| `--connect-timeout` | Enforced separately (connect callback) | Best-effort; the overall `--max-time` still applies |
| `-p` / `--proxytunnel` (CONNECT) | Yes | No |
| `--http2`, `--http3` | Version and fallback policy applied | Version requested; the runtime's `HttpClient` decides (the .NET Framework one speaks HTTP/1.1) |
| `--request-target`, removing `Host`, blank headers | Yes | No |
| `--raw`, lower-case chunk sizes | Yes | No |
| Response headers in `result.Headers` / `-i` | Exactly as sent | As `HttpClient` parsed them |
| `ParseJson<T>()` | `System.Text.Json`, case-insensitive | `System.Text.Json` on `net472`/`net48`, Newtonsoft.Json on `netstandard2.0`; both case-insensitive |

Everything else on this page (timeouts, retries, cookies, redirects, Digest/NTLM, `-w`, `-o`) behaves the same on every target.

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

The package references `System.Text.Json` and `Newtonsoft.Json`, so `result.ParseJson<T>()` works on every target without extra packages. Property matching is case-insensitive everywhere, so `name` and `html_url` bind to `Name` and `Html_Url` below:

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
