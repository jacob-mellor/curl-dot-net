# .NET Framework Compatibility Guide

CurlDotNet fully supports .NET Framework 4.7.2 and later through our .NET Standard 2.0 build.

## Supported Frameworks

| Framework | Status | Build Target |
|-----------|--------|--------------|
| .NET Framework 4.7.2 | ✅ Supported | netstandard2.0 / net472 |
| .NET Framework 4.8 | ✅ Supported | netstandard2.0 / net48 |
| .NET Standard 2.0 | ✅ Supported | netstandard2.0 |
| .NET Core 3.1+ | ✅ Supported | netstandard2.0 |
| .NET 5.0+ | ✅ Supported | net8.0+ |

## Installation for Framework Projects

```xml
<PackageReference Include="CurlDotNet" Version="1.2.35" />
```

## Usage in .NET Framework

```csharp
using CurlDotNet;
using System;

class Program
{
    static void Main()
    {
        // Synchronous execution (common in Framework apps)
        var result = Curl.Execute("curl https://api.example.com/data");
        Console.WriteLine($"Status: {result.StatusCode}");
        Console.WriteLine($"Body: {result.Body}");
    }

    static async Task AsyncExample()
    {
        // Async execution also supported
        var result = await Curl.ExecuteAsync("curl -X POST -d 'data' https://api.example.com/submit");
        Console.WriteLine($"Success: {result.IsSuccess}");
    }
}
```

## Framework-Specific Considerations

### 1. TLS/SSL Configuration

.NET Framework apps often need explicit TLS configuration:

```csharp
// Add this at application startup
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
```

### 2. JSON Serialization

System.Text.Json is included via NuGet for Framework projects. No additional packages needed.

```csharp
// This works in Framework 4.7.2+
var result = await Curl.ExecuteAsync("curl https://api.example.com/json");
var data = result.ParseJson<MyDataClass>();
```

### 3. HttpClient Differences

The library handles HttpClient lifecycle management appropriately for Framework:

```csharp
// No need to manage HttpClient instances
// CurlDotNet handles this internally
var result = Curl.Execute("curl https://api.example.com");
```

### 4. WebRequest Compatibility

CurlDotNet works alongside legacy WebRequest code:

```csharp
// Legacy code
var webRequest = WebRequest.Create("https://old-api.example.com");

// Modern CurlDotNet code in the same app
var result = Curl.Execute("curl https://new-api.example.com");
```

## Testing Framework Compatibility

### On Windows

Run the Framework-specific tests:

```bash
dotnet test tests/CurlDotNet.FrameworkCompat/CurlDotNet.FrameworkCompat.csproj
```

### On macOS/Linux

Use the compatibility script (tests .NET Standard 2.0 build):

```bash
dotnet script scripts/test-framework-compat.csx
```

### CI/CD

GitHub Actions automatically test Framework compatibility on Windows:

```yaml
- name: Test Framework 4.7.2
  run: dotnet build -f net472 && dotnet test
```

## Known Limitations

1. **FTP Protocol**: .NET Core/5+ removed FTP support. Use HTTP(S) alternatives where possible.

2. **Async Patterns**: Framework uses older async patterns. We provide both sync and async methods.

3. **Performance**: Framework's HttpClient is less optimized than .NET Core's. Consider upgrading if performance is critical.

## Migration from Framework

If migrating from Framework to .NET 6+:

1. No code changes needed for basic usage
2. Consider switching to async methods for better performance
3. Remove ServicePointManager configurations (handled automatically in .NET Core+)

## Troubleshooting

### "Could not load file or assembly"

Ensure all dependencies are restored:
```bash
nuget restore
```

### SSL/TLS Errors

Add to application startup:
```csharp
ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
```

### JSON Serialization Issues

The package includes System.Text.Json. If conflicts occur, use:
```xml
<PackageReference Include="System.Text.Json" Version="8.0.0" />
```

## Support

Framework support is tested via:
- Windows CI builds with actual Framework runtimes
- .NET Standard 2.0 compatibility tests
- Integration tests with Framework-specific patterns

Report Framework-specific issues: https://github.com/jacob-mellor/curl-dot-net/issues