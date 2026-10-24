---
title: "Install CurlDotNet: curl for .NET 8, 9, 10 and .NET 4.7.2+"
description: "Install CurlDotNet, curl for .NET 8, 9, 10 and .NET Framework 4.7.2+, from NuGet. Supported target frameworks, verification and troubleshooting."
---

# Install CurlDotNet: curl for .NET 8, 9, 10 and .NET Framework 4.7.2+

Install CurlDotNet, curl for .NET 8, 9, 10 and .NET Framework 4.7.2+, from NuGet with a single command. CurlDotNet is pure C# with no native dependencies, so there is nothing else to install: no libcurl, no curl binary, no platform-specific packages.

## Install from NuGet

.NET CLI:

```bash
dotnet add package CurlDotNet
```

Package Manager Console (Visual Studio):

```powershell
Install-Package CurlDotNet
```

PackageReference in your `.csproj`:

```xml
<ItemGroup>
  <PackageReference Include="CurlDotNet" Version="9.*" />
</ItemGroup>
```

Visual Studio UI: right-click the project, choose **Manage NuGet Packages**, search for `CurlDotNet` and click **Install**.

Package page: [nuget.org/packages/CurlDotNet](https://www.nuget.org/packages/CurlDotNet/)

## Supported target frameworks

| Your project targets | CurlDotNet build used | Notes |
|----------------------|-----------------------|-------|
| .NET 10.0 | `net10.0` | Current release |
| .NET 9.0 | `net9.0` | |
| .NET 8.0 | `net8.0` | LTS |
| .NET Framework 4.8 | `net48` | |
| .NET Framework 4.7.2 | `net472` | Oldest supported .NET Framework |
| .NET 6 / 7, Mono, Unity, Xamarin | `netstandard2.0` | Anything that implements .NET Standard 2.0 |

The package depends only on `System.Text.Json` and `Newtonsoft.Json` (plus `Microsoft.CSharp` for the .NET Standard build and `System.Net.Http` for the .NET Framework builds); there is no Ninject, System.CommandLine or native dependency. `ParseJson<T>()` uses `System.Text.Json` on every build except `netstandard2.0`, which uses Newtonsoft.Json; both match property names case-insensitively. The public API is identical. See [framework compatibility](../framework-compatibility.html) for details.

## Prerequisites: the .NET SDK

Check what you have:

```bash
dotnet --version
```

If the command is not found, install the SDK (not just the runtime) from [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download), or with a package manager:

```powershell
# Windows
winget install Microsoft.DotNet.SDK.10
```

```bash
# macOS
brew install --cask dotnet-sdk
```

```bash
# Ubuntu / Debian
sudo apt-get update && sudo apt-get install -y dotnet-sdk-10.0
```

Restart your terminal after installing. For other Linux distributions follow Microsoft's [Linux install guide](https://learn.microsoft.com/dotnet/core/install/linux).

## Verify the installation

Create a console app, add the package and run one request.

```bash
dotnet new console -n CurlDotNetCheck
cd CurlDotNetCheck
dotnet add package CurlDotNet
```

Replace `Program.cs`:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
Console.WriteLine($"CurlDotNet works. HTTP {result.StatusCode}");
```

```bash
dotnet run
```

Expected output: `CurlDotNet works. HTTP 200`.

## Install CurlDotNet in a .NET Framework project

CurlDotNet ships `net472` and `net48` builds. Install the same NuGet package; no extra configuration is needed. If your project still uses `packages.config`, Visual Studio will offer to migrate to PackageReference, which is recommended. The static `Curl.ExecuteAsync` API and the `CurlRequestBuilder` are available on .NET Framework exactly as on modern .NET.

## Docker

CurlDotNet has no native dependencies, so it runs in the standard Microsoft images including the Alpine and chiseled variants.

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MyApp.dll"]
```

## Troubleshooting

### Package 'CurlDotNet' not found

Make sure nuget.org is a configured source, then clear the cache and retry.

```bash
dotnet nuget list source
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org
dotnet nuget locals all --clear
dotnet add package CurlDotNet
```

### The target framework is not supported

CurlDotNet supports .NET 8+, .NET Framework 4.7.2+ and .NET Standard 2.0. If your project targets .NET 6 or 7 it will use the `netstandard2.0` build, which works but uses `HttpClient`'s parsed headers and so lacks a few wire-level curl features (`--request-target`, removing `Host`, blank headers, `--raw`, `-p` proxy tunnels and a separately enforced `--connect-timeout`). Upgrading the `<TargetFramework>` to `net8.0` or later is recommended.

### dotnet: command not found

The SDK is not on your `PATH`. Reinstall from [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) and open a new terminal.

### SSL or certificate errors on first run

CurlDotNet uses the operating system's certificate store through .NET. Make sure the OS root certificates are up to date (`sudo apt-get install ca-certificates` on Debian/Ubuntu). For development servers with self-signed certificates you can pass `-k` (see [curl ignore SSL certificate in C#](../guides/advanced.html#curl-ignore-ssl-certificate-in-c)); do not use it in production.

More: [Troubleshooting](../troubleshooting/README.html) and [Common issues](../troubleshooting/common-issues.html).

## Next steps

- [Quick start: run your first curl command in C# in 2 minutes](quickstart.html)
- [Your first request, explained](first-request.html)
- [Configuration](configuration.html)
- [New to curl?](../new-to-curl.html)
