# Simple GET Request Example

## 📦 NuGet Package

Install CurlDotNet from NuGet:
```bash
dotnet add package CurlDotNet
```
Or visit: https://www.nuget.org/packages/CurlDotNet/

## What This Example Demonstrates

This example shows the absolute basics of making HTTP GET requests with CurlDotNet, with a focus on **understanding the Result object** - the heart of CurlDotNet.

## 🎯 The Result Object

Every CurlDotNet operation returns a `CurlResult` object with these key properties:

```csharp
var result = await Curl.GetAsync("https://api.example.com");

// Essential properties
result.IsSuccess        // bool: true if status 200-299
result.Body            // string: response body text
result.StatusCode      // int: HTTP status code (200, 404, etc.)
result.Headers         // Dictionary: response headers
result.ElapsedTime     // TimeSpan: request duration
result.ContentType     // string: Content-Type header value
result.ContentLength   // long?: size of response

// Convenience methods
result.ParseJson<T>()           // Deserialize JSON to type T
result.SaveToFile("file.txt")   // Save response to file
result.GetHeader("X-Custom")    // Get specific header value
result.EnsureSuccessStatusCode() // Throw if not successful
```

## Running This Example

```bash
dotnet run
```

## What You'll See

The example performs 4 different GET requests, each demonstrating different aspects of the Result object:

1. **Basic request** - Shows IsSuccess and Body
2. **Explicit curl syntax** - Same result, different API
3. **Convenience method** - Using Curl.GetAsync
4. **Query parameters** - With detailed result inspection

## Code Highlights

### Checking Success
```csharp
if (result.IsSuccess)  // true for 200-299 status codes
{
    Console.WriteLine($"Success! Got {result.Body.Length} bytes");
}
else
{
    Console.WriteLine($"Failed with status {result.StatusCode}");
}
```

### Accessing Response Data
```csharp
// Get the response body
string content = result.Body;

// Check status code
int status = result.StatusCode;  // e.g., 200, 404, 500

// Get headers
var contentType = result.Headers["Content-Type"];
// or safer:
var userAgent = result.GetHeader("User-Agent") ?? "not set";

// Check timing
Console.WriteLine($"Request took {result.ElapsedTime.TotalMilliseconds}ms");
```

### Error Handling Patterns
```csharp
// Pattern 1: Simple check
if (!result.IsSuccess)
{
    Console.WriteLine($"Error {result.StatusCode}: {result.Body}");
    return;
}

// Pattern 2: Throw on failure
try
{
    result.EnsureSuccessStatusCode();
    // Process successful response
}
catch (CurlHttpException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.Message}");
}
```

## Common Use Cases

- ✅ Fetching data from REST APIs
- ✅ Downloading web pages
- ✅ Health checks and monitoring
- ✅ Testing API endpoints
- ✅ Getting public data

## Key Takeaways

1. **Every request returns a Result object** - This is your response container
2. **Always check IsSuccess** - Don't assume requests succeed
3. **Result has everything you need** - Body, headers, status, timing
4. **Extension methods add power** - ParseJson, SaveToFile, etc.
5. **Error info is in the Result too** - Status code and error body

## Next Steps

- Try modifying the URLs to fetch different APIs
- Add custom headers (see example 02-GetWithHeaders)
- Parse JSON responses (see examples/02-JSON)
- Handle errors properly (see example 05-ErrorHandling)

## Related Documentation

- [Understanding the Result Object](../../../docs/articles/understanding-result.md)
- [Cookbook: Simple GET](../../../docs/cookbook/beginner/simple-get.md)
- [API Reference: CurlResult](https://jacob-mellor.github.io/curl-dot-net/api/CurlDotNet.Core.CurlResult.html)
- [NuGet Package](https://www.nuget.org/packages/CurlDotNet/)