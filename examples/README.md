# CurlDotNet Examples

This directory contains comprehensive examples demonstrating how to use CurlDotNet effectively.

## Quick Start

```bash
cd examples/CurlDotNet.Examples
dotnet run
```

## What's Included

The example project demonstrates:

1. **Hello World** - Simplest GET request (5-minute quickstart)
2. **Direct curl commands** - Paste any curl command directly
3. **Result object usage** - Understanding the core response object
4. **JSON parsing** - Parse responses to strongly-typed objects
5. **File downloads** - Handle binary data and save to disk
6. **Error handling** - Deal with failures gracefully
7. **Headers & status codes** - Work with response metadata
8. **Fluent builder API** - Build requests programmatically
9. **API client pattern** - Simplified REST operations
10. **Extension methods** - Ergonomic helper methods

## Key Concepts

### The Result Object

Every CurlDotNet operation returns a `CurlResult` object:

```csharp
// NuGet: https://www.nuget.org/packages/CurlDotNet/
using CurlDotNet;

var result = await Curl.GetAsync("https://api.example.com/data");

// Check success
if (result.IsSuccess)  // 200-299 status codes
{
    // Access the response
    string body = result.Body;
    int status = result.StatusCode;
    var headers = result.Headers;
    var elapsed = result.ElapsedTime;
}
```

### Using Extension Methods

All extension methods are available with just `using CurlDotNet;`:

```csharp
// Parse JSON
var data = result.ParseJson<MyType>();

// Save to file
result.SaveToFile("response.json");

// Get specific header
var contentType = result.GetHeader("Content-Type");

// Ensure success (throws if failed)
result.EnsureSuccessStatusCode();
```

### Error Handling Patterns

```csharp
// Pattern 1: Check IsSuccess
if (!result.IsSuccess)
{
    Console.WriteLine($"Failed: {result.StatusCode}");
}

// Pattern 2: Use EnsureSuccessStatusCode
try
{
    result.EnsureSuccessStatusCode();
    // Process success case
}
catch (CurlHttpException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.Message}");
}
```

## Running the Examples

### With Project Reference (Development)

```bash
dotnet run
```

### With NuGet Package (Production)

1. Edit `CurlDotNet.Examples.csproj`
2. Comment out the ProjectReference
3. Uncomment the PackageReference
4. Run `dotnet restore` then `dotnet run`

## Learn More

- [Full Documentation](https://jacob-mellor.github.io/curl-dot-net/)
- [API Reference](https://jacob-mellor.github.io/curl-dot-net/api/)
- [NuGet Package](https://www.nuget.org/packages/CurlDotNet/)