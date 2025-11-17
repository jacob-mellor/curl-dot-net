---
layout: default
title: Why CurlDotNet Exists - The UserLand.NET Vision
description: Understanding the philosophy behind bringing Unix/Linux command-line tools to the .NET ecosystem
---

# Why CurlDotNet Exists

## The UserLand.NET Vision

CurlDotNet is part of a larger initiative called **UserLand.NET** - a movement to bring the power, flexibility, and proven reliability of Unix/Linux/BSD command-line tools to the .NET ecosystem. This isn't about replacing Windows tools or abandoning the Microsoft ecosystem - it's about empowering .NET developers with the best tools from all worlds.

## The Deploy-Everywhere Promise

.NET has evolved into a truly cross-platform framework. With .NET running on Windows, Linux, macOS, mobile devices, IoT, and cloud platforms, developers need tools that work consistently across all these environments. CurlDotNet delivers on this promise:

- **Write Once, Run Anywhere**: Your HTTP client code works identically on Windows Server, Ubuntu, macOS, Alpine Linux containers, and Azure Functions
- **No Platform-Specific Code**: Eliminate `if (RuntimeInformation.IsOSPlatform())` checks from your networking code
- **Container-Ready**: Optimized for minimal Docker images and serverless environments
- **Cloud-Native**: Perfect for microservices, Kubernetes, and cloud-native architectures

## Why Unix/Linux Tools Matter to .NET Developers

### 1. Battle-Tested Reliability

Tools like curl have been refined over decades:
- curl has been actively developed since 1997
- Used by billions of devices worldwide
- Powers critical infrastructure globally
- Thoroughly tested in every conceivable scenario

### 2. Industry Standard Behavior

When you use CurlDotNet:
- Your application behaves like industry-standard tools
- DevOps teams instantly understand the behavior
- Documentation and knowledge are transferable
- Error messages and patterns are familiar

### 3. Superior Feature Set

Unix/Linux tools often provide capabilities that are missing or complex in native .NET:
- Advanced proxy handling
- Comprehensive authentication methods
- Sophisticated retry mechanisms
- Fine-grained SSL/TLS control
- Protocol-level debugging

## Our Commitment to the Microsoft Ecosystem

CurlDotNet fully embraces the Microsoft and Windows ecosystem:

### Windows-First Development
- Developed and tested primarily on Windows
- Full Visual Studio integration
- Windows-specific optimizations
- PowerShell-friendly APIs

### .NET Native Integration
- Uses .NET idioms and patterns
- Async/await throughout
- IDisposable patterns
- LINQ-friendly collections
- Standard .NET exception hierarchy

### Azure and Microsoft Cloud
- Optimized for Azure Functions
- Azure DevOps CI/CD ready
- Application Insights integration
- Azure Key Vault compatibility

## Clean-Room Development Strategy

CurlDotNet is built using clean-room development principles:

### Pure C# Implementation
- **No P/Invoke**: Everything is pure managed code
- **No Native Dependencies**: No libcurl.dll or curl.exe required
- **No GPL Code**: MIT licensed, business-friendly
- **Original Implementation**: Written from scratch in C#

### Why Clean-Room Matters
1. **Security**: No inherited vulnerabilities from C libraries
2. **Portability**: Runs anywhere .NET runs
3. **Maintainability**: C# developers can understand and contribute
4. **Performance**: JIT optimizations and managed memory
5. **Debugging**: Full .NET debugging experience

## The Open Source Philosophy

### Genuine Open Source vs Copy-Left

CurlDotNet represents genuine open source development:

- **MIT License**: Use it anywhere, modify it freely, sell it if you want
- **No Viral Licensing**: Won't infect your proprietary code
- **Business-Friendly**: Legal departments love MIT
- **True Freedom**: Freedom to use, not obligation to share

Copy-left licenses like GPL, while well-intentioned, can:
- Prevent adoption in commercial products
- Create legal uncertainty
- Limit integration possibilities
- Reduce contribution from businesses

### Enabling Progress

By choosing MIT licensing and clean-room development:
- Businesses can adopt without legal concerns
- Developers can embed without license conflicts
- Innovation happens faster
- The entire .NET ecosystem benefits

## Real-World Impact

### For Enterprises
- Standardize on one HTTP client across all platforms
- Reduce training costs with familiar curl patterns
- Eliminate platform-specific networking code
- Comply with security policies easier

### For Startups
- Ship faster with proven patterns
- Scale without rewriting networking layer
- Deploy anywhere without modifications
- Save money on development time

### For Open Source Projects
- Provide consistent behavior across platforms
- Attract contributors familiar with curl
- Reduce bug reports from platform differences
- Focus on features, not platform quirks

## The UserLand.NET Ecosystem

CurlDotNet is just the beginning. The UserLand.NET vision includes:

### Current Projects
- **CurlDotNet**: HTTP client with curl semantics
- **GrepDotNet**: Text searching with grep patterns (planned)
- **SedDotNet**: Stream editing in .NET (planned)
- **AwkDotNet**: Text processing patterns (planned)

### Future Vision
- Bring 50+ essential Unix tools to .NET
- Maintain 100% managed code
- Preserve original tool semantics
- Provide .NET-idiomatic APIs

## Technical Excellence

### Performance First
- Zero-allocation hot paths
- Span<T> and Memory<T> usage
- Pipeline pattern for efficiency
- Minimal overhead over raw sockets

### Security by Design
- Secure defaults
- Certificate validation
- Modern TLS versions
- OWASP compliance

### Developer Experience
- IntelliSense documentation
- Comprehensive examples
- Familiar patterns
- Great error messages

## Why Not Just Use HttpClient?

HttpClient is good, but CurlDotNet excels at:

1. **Complex Scenarios**: Proxies, authentication chains, certificate handling
2. **Debugging**: Verbose output matching curl -v
3. **Compatibility**: Behaves exactly like curl
4. **Features**: Many advanced features not in HttpClient
5. **Simplicity**: One-line operations that would take 20+ lines with HttpClient

## Community and Contribution

### Built by Developers, for Developers
- Real-world requirements drive features
- Community feedback shapes direction
- Pull requests welcome
- Issues addressed quickly

### Sustainable Development
- MIT license ensures longevity
- Multiple maintainers
- Corporate backing
- Clear governance

## Conclusion

CurlDotNet exists because .NET developers deserve:
- The best tools from all ecosystems
- Consistent behavior across all platforms
- Freedom from platform-specific code
- Enterprise-ready, production-tested patterns

The UserLand.NET vision is simple: **Make .NET the best platform for developers by bringing proven tools to the ecosystem, implemented with technical excellence, and distributed with true open source freedom.**

Whether you're building microservices on Linux, desktop apps on Windows, mobile apps on iOS/Android, or serverless functions in the cloud, CurlDotNet provides the reliable, familiar, and powerful HTTP client you need.

---

## Get Started

```csharp
// The same code works everywhere .NET runs
using CurlDotNet;

var response = await Curl.GetAsync("https://api.example.com/data");
Console.WriteLine(response.Body);
```

**Install**: `dotnet add package CurlDotNet`

**Learn More**:
- [Getting Started Guide](/getting-started/)
- [UserLand.NET Initiative](https://userland.net)
- [Contributing Guidelines](https://github.com/jacob-mellor/curl-dot-net/blob/master/CONTRIBUTING.md)

---

*CurlDotNet is part of the UserLand.NET initiative - bringing the power of Unix to the .NET ecosystem through clean-room, pure C# implementations.*