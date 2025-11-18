---
layout: default
title: CurlDotNet Compatibility Guide - .NET Framework and Platform Support
description: Complete compatibility matrix for CurlDotNet across .NET versions, operating systems, and deployment targets
keywords: CurlDotNet compatibility, .NET Framework support, .NET Core, cross-platform, curl C#
---

# CurlDotNet Compatibility Guide

## .NET Version Support Matrix

CurlDotNet provides comprehensive support across all modern .NET platforms and frameworks.

### Supported Target Frameworks

| Framework | Version | Status | Notes |
|-----------|---------|--------|-------|
| **.NET 10** | 10.0+ | ✅ Full Support | Latest features, best performance |
| **.NET 9** | 9.0+ | ✅ Full Support | Current LTS candidate |
| **.NET 8** | 8.0+ | ✅ Full Support | Long Term Support (LTS) |
| **.NET 7** | 7.0+ | ✅ Full Support | Standard Term Support |
| **.NET 6** | 6.0+ | ✅ Full Support | Long Term Support (LTS) |
| **.NET 5** | 5.0+ | ⚠️ Supported | End of life, upgrade recommended |
| **.NET Core** | 3.1+ | ⚠️ Supported | LTS ended, upgrade recommended |
| **.NET Core** | 2.1-3.0 | ⚠️ Limited | Via .NET Standard 2.0 |
| **.NET Standard** | 2.0+ | ✅ Full Support | Maximum compatibility |
| **.NET Framework** | 4.7.2+ | ✅ Full Support | Via .NET Standard 2.0 |
| **.NET Framework** | 4.6.1-4.7.1 | ⚠️ Limited | Basic features only |
| **Mono** | 5.4+ | ✅ Full Support | Via .NET Standard 2.0 |
| **Xamarin.iOS** | 10.14+ | ✅ Full Support | Mobile ready |
| **Xamarin.Android** | 8.0+ | ✅ Full Support | Mobile ready |
| **Unity** | 2018.1+ | ✅ Full Support | Game development ready |
| **UWP** | 10.0.16299+ | ✅ Full Support | Windows Store apps |

### Operating System Compatibility

CurlDotNet works seamlessly across all major operating systems:

#### Windows
- **Windows 11**: ✅ Full Support (All editions)
- **Windows 10**: ✅ Full Support (Version 1607+)
- **Windows Server 2022**: ✅ Full Support
- **Windows Server 2019**: ✅ Full Support
- **Windows Server 2016**: ✅ Full Support
- **Windows 8.1**: ⚠️ Supported (via .NET Framework)
- **Windows 7 SP1**: ⚠️ Limited (via .NET Framework 4.7.2)

#### Linux
- **Ubuntu**: ✅ 20.04 LTS, 22.04 LTS, 24.04 LTS
- **Debian**: ✅ 10+, 11+, 12+
- **RHEL/CentOS**: ✅ 7+, 8+, 9+
- **Fedora**: ✅ 36+
- **Alpine**: ✅ 3.12+ (musl libc compatible)
- **Amazon Linux**: ✅ 2, 2023
- **SUSE**: ✅ Enterprise 12+, openSUSE 15+
- **Arch**: ✅ Rolling release

#### macOS
- **macOS 14 (Sonoma)**: ✅ Full Support
- **macOS 13 (Ventura)**: ✅ Full Support
- **macOS 12 (Monterey)**: ✅ Full Support
- **macOS 11 (Big Sur)**: ✅ Full Support
- **macOS 10.15 (Catalina)**: ✅ Full Support
- **macOS 10.14 (Mojave)**: ⚠️ Supported

#### Mobile Platforms
- **iOS**: ✅ 12.0+ (via Xamarin.iOS or .NET MAUI)
- **Android**: ✅ API 21+ (via Xamarin.Android or .NET MAUI)
- **tvOS**: ✅ 10.0+ (via Xamarin.tvOS)
- **watchOS**: ⚠️ Experimental (via Xamarin.watchOS)

#### Container Platforms
- **Docker**: ✅ All .NET base images
- **Kubernetes**: ✅ Any container runtime
- **OpenShift**: ✅ Full Support
- **AWS ECS/Fargate**: ✅ Full Support
- **Azure Container Instances**: ✅ Full Support
- **Google Cloud Run**: ✅ Full Support

#### Serverless Platforms
- **AWS Lambda**: ✅ Full Support (.NET 6/8 runtime)
- **Azure Functions**: ✅ Full Support (All versions)
- **Google Cloud Functions**: ✅ Full Support
- **Cloudflare Workers**: ⚠️ Experimental (via WASM)

## Architecture Support

CurlDotNet supports all major CPU architectures:

| Architecture | Support | Notes |
|--------------|---------|-------|
| **x64/AMD64** | ✅ Full | Primary development target |
| **x86** | ✅ Full | 32-bit Windows compatibility |
| **ARM64** | ✅ Full | Apple Silicon, AWS Graviton |
| **ARM32** | ✅ Full | Raspberry Pi, IoT devices |
| **WASM** | ⚠️ Experimental | Blazor WebAssembly |

## Feature Compatibility Matrix

Not all features are available on all platforms:

| Feature | .NET 6+ | .NET Framework | .NET Standard 2.0 | Mobile |
|---------|---------|----------------|-------------------|--------|
| **HTTP/1.1** | ✅ | ✅ | ✅ | ✅ |
| **HTTP/2** | ✅ | ⚠️ | ⚠️ | ✅ |
| **HTTP/3** | ✅ | ❌ | ❌ | ⚠️ |
| **TLS 1.3** | ✅ | ⚠️ OS-dependent | ⚠️ | ✅ |
| **Compression** | ✅ | ✅ | ✅ | ✅ |
| **Cookies** | ✅ | ✅ | ✅ | ✅ |
| **Proxy** | ✅ | ✅ | ✅ | ✅ |
| **Async/Await** | ✅ | ✅ | ✅ | ✅ |
| **Cancellation** | ✅ | ✅ | ✅ | ✅ |
| **Streams** | ✅ | ✅ | ✅ | ✅ |
| **WebSockets** | ✅ | ⚠️ | ⚠️ | ✅ |
| **Unix Sockets** | ✅ | ❌ | ⚠️ | ❌ |

## curl Command Line Compatibility

CurlDotNet maintains high compatibility with curl command-line options:

### Fully Supported curl Options
```bash
# These curl options have direct C# equivalents
curl -X GET                    # Curl.GetAsync()
curl -X POST                   # Curl.PostAsync()
curl -H "Header: Value"        # .WithHeader()
curl -d "data"                 # .WithBody()
curl -u user:pass              # .WithBasicAuth()
curl -L                        # .FollowRedirects()
curl -o file                   # .DownloadFileAsync()
curl -T file                   # .UploadFileAsync()
curl --connect-timeout 30      # .WithTimeout()
curl --max-time 60             # .WithMaxTime()
curl --retry 3                 # .WithRetry()
curl -v                        # .WithVerbose()
curl -s                        # .Silent()
curl -k                        # .InsecureSkipVerify()
curl --compressed              # .WithCompression()
curl -A "User-Agent"           # .WithUserAgent()
curl -e "Referer"              # .WithReferer()
curl -b "cookie=value"         # .WithCookie()
curl -c cookiejar              # .SaveCookies()
curl --proxy http://proxy      # .WithProxy()
```

### Partially Supported Options
These options work but may have slight differences:

```bash
curl --interface eth0          # Limited to IP binding
curl --resolve                 # Custom DNS available
curl --cacert                  # Certificate validation differs
curl --cert                    # Client certificates supported
```

### Platform-Specific Limitations

#### Windows-Specific
- Unix socket support requires Windows 10 version 1803+
- Some certificate stores behave differently
- Path separators must use backslash or forward slash

#### Linux-Specific
- Requires libicu for globalization on some distros
- May need CA certificate bundle installation

#### macOS-Specific
- Keychain integration for certificates
- Gatekeeper may require code signing

#### Container-Specific
- Alpine Linux requires additional packages for culture support
- Minimal images may lack timezone data
- CA certificates must be explicitly included

## Version Migration Guide

### From .NET Framework to .NET 6+

```csharp
// .NET Framework 4.7.2 (using .NET Standard 2.0 package)
using CurlDotNet;

var client = new Curl();
var response = client.Get("https://api.example.com/data");

// .NET 6+ (using native package with all features)
using CurlDotNet;

var response = await Curl.GetAsync("https://api.example.com/data");
// Additional features available: HTTP/3, Unix sockets, better performance
```

### From Older CurlDotNet Versions

```csharp
// CurlDotNet 0.x (old API)
var curl = new CurlClient();
var result = curl.Execute("https://api.example.com");

// CurlDotNet 1.x+ (modern API)
var result = await Curl.GetAsync("https://api.example.com");
```

## Deployment Scenarios

### Docker Deployment

```dockerfile
# .NET 8 on Alpine (smallest)
FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine
# CurlDotNet works perfectly

# .NET 8 on Debian (most compatible)
FROM mcr.microsoft.com/dotnet/runtime:8.0
# CurlDotNet works perfectly

# .NET Framework on Windows
FROM mcr.microsoft.com/dotnet/framework/runtime:4.8
# CurlDotNet works via .NET Standard 2.0
```

### Azure Functions

```xml
<!-- function.proj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <AzureFunctionsVersion>v4</AzureFunctionsVersion>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="CurlDotNet" Version="*" />
  </ItemGroup>
</Project>
```

### AWS Lambda

```xml
<!-- .csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <GenerateRuntimeConfigurationFiles>true</GenerateRuntimeConfigurationFiles>
    <AWSProjectType>Lambda</AWSProjectType>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="CurlDotNet" Version="*" />
    <PackageReference Include="Amazon.Lambda.Core" Version="*" />
  </ItemGroup>
</Project>
```

## Compatibility Testing

CurlDotNet is tested on:

### Continuous Integration Platforms
- **Windows**: Windows Server 2019, 2022
- **Linux**: Ubuntu 20.04, 22.04
- **macOS**: macOS-12, macOS-13, macOS-14

### Test Matrix
- All supported .NET versions
- All supported operating systems
- Both x64 and ARM64 architectures
- Container environments
- Serverless platforms

### Compatibility Verification

```csharp
using CurlDotNet;
using System.Runtime.InteropServices;

// Check platform compatibility
public static void VerifyCompatibility()
{
    Console.WriteLine($"OS: {RuntimeInformation.OSDescription}");
    Console.WriteLine($"Framework: {RuntimeInformation.FrameworkDescription}");
    Console.WriteLine($"Architecture: {RuntimeInformation.ProcessArchitecture}");

    // Test basic functionality
    try
    {
        var result = Curl.Get("https://httpbin.org/get");
        Console.WriteLine("✅ CurlDotNet is fully compatible!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Compatibility issue: {ex.Message}");
    }
}
```

## Known Limitations

### By Platform

#### .NET Framework 4.7.2
- No HTTP/3 support
- TLS 1.3 depends on Windows version
- Some async patterns less efficient

#### .NET Standard 2.0
- Limited HTTP/2 support
- No Unix socket support
- Some performance optimizations unavailable

#### Mobile Platforms
- Background operations have OS restrictions
- Certificate stores differ from desktop
- Network permissions required

#### Containers
- Alpine requires additional globalization packages
- Minimal images may lack timezone data
- CA certificates must be included

### Workarounds

For most limitations, workarounds are available:

```csharp
// Example: TLS 1.3 on older Windows
var curl = new Curl()
    .WithTlsVersion(TlsVersion.Tls12) // Fallback to TLS 1.2
    .WithOption(CurlOption.TryTls13, true); // Try TLS 1.3 if available
```

## Getting Help

### Compatibility Issues
- Check the [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)
- Review the [FAQ](/faq#compatibility)
- Contact support

### Testing Your Environment
```bash
# Test CurlDotNet compatibility
dotnet tool install -g CurlDotNet.CLI
curldotnet --test-compatibility
```

## Summary

CurlDotNet provides excellent compatibility across:
- ✅ All modern .NET versions (5+)
- ✅ .NET Framework 4.7.2+ via .NET Standard 2.0
- ✅ All major operating systems
- ✅ Container and serverless platforms
- ✅ Mobile platforms via Xamarin/.NET MAUI
- ✅ Multiple CPU architectures

The library is designed to work everywhere .NET runs, maintaining consistent behavior across all platforms while leveraging platform-specific optimizations where available.

---

*CurlDotNet - Write once, run anywhere. The curl experience for C# and .NET developers on every platform.*