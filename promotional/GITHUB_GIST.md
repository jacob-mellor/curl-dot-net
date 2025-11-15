# CurlDotNet - Paste curl Commands in .NET

## The Problem

Every API documentation shows curl commands. But using them in .NET requires manual translation to `HttpClient`—tedious and error-prone.

## The Solution

**CurlDotNet** lets you paste curl commands directly into C#. No translation needed.

```csharp
// Paste any curl command - it just works!
var result = await Curl.ExecuteAsync(@"
  curl https://api.stripe.com/v1/charges \
    -u sk_test_4eC39HqLyjWDarjtT1zdp7dc: \
    -d amount=2000 \
    -d currency=usd \
    -d source=tok_mastercard
");

if (result.IsSuccess)
{
    var charge = result.ParseJson<StripeCharge>();
    Console.WriteLine($"Payment successful! ID: {charge.Id}");
}
```

## Features

✅ **Paste Any curl Command** - Works with or without "curl" prefix  
✅ **All 300+ curl Options Supported** - Complete compatibility  
✅ **Two APIs** - Paste curl strings OR use fluent builder  
✅ **Perfect IntelliSense** - Full documentation and type safety  
✅ **Memory Efficient** - Stream-based for large files  
✅ **Comprehensive Errors** - Every curl error code has its own exception  

## Installation

```bash
dotnet add package CurlDotNet
```

## Quick Examples

### GET Request
```csharp
var result = await Curl.ExecuteAsync(
    "curl https://api.github.com/users/octocat"
);
Console.WriteLine(result.Body);
```

### POST with JSON
```csharp
var result = await Curl.ExecuteAsync(@"
  curl -X POST https://api.example.com/users \
    -H 'Content-Type: application/json' \
    -d '{\""name\"":\""John\"",\""email\"":\""john@example.com\""}'
");
```

### Authentication
```csharp
// Basic auth
var result = await Curl.ExecuteAsync(
    "curl -u username:password https://api.example.com/protected"
);

// Bearer token
var result = await Curl.ExecuteAsync(
    "curl -H 'Authorization: Bearer YOUR_TOKEN' https://api.example.com/data"
);
```

### File Operations
```csharp
// Download and save
var result = await Curl.ExecuteAsync(
    "curl -o image.png https://example.com/image.png"
);

// Also available in memory
Console.WriteLine($"Downloaded {result.BinaryData.Length} bytes");
```

### Fluent Builder API
```csharp
var result = await CurlRequestBuilder
    .Post("https://api.example.com/data")
    .WithHeader("Content-Type", "application/json")
    .WithJson(new { key = "value" })
    .WithTimeout(TimeSpan.FromSeconds(30))
    .FollowRedirects()
    .ExecuteAsync();
```

## Working with Responses

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com/data");

// Check success (200-299)
if (result.IsSuccess)
{
    // Parse JSON
    var data = result.ParseJson<DataModel>();
    
    // Access headers
    var contentType = result.Headers["Content-Type"];
    
    // Save to file
    result.SaveToFile("output.json");
    
    // Get status code
    Console.WriteLine($"Status: {result.StatusCode}");
}

// Or use fluent chaining
var data = result
    .EnsureSuccess()           // Throws if not 200-299
    .SaveToFile("backup.json")
    .ParseJson<DataModel>();
```

## Error Handling

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://example.com/api");
}
catch (CurlDnsException ex)
{
    Console.WriteLine($"DNS error: {ex.Message}");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Timeout: {ex.Message}");
}
catch (CurlHttpException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.Message}");
}
```

## Use Cases

✅ **API Integration** - Use curl commands from API docs directly  
✅ **Rapid Prototyping** - Test endpoints quickly by pasting curl  
✅ **CI/CD Automation** - Migrate bash scripts to .NET easily  
✅ **Documentation** - Use actual curl examples from docs  
✅ **Learning** - Experiment with HTTP using familiar curl syntax  

## Why CurlDotNet?

- **No Translation Errors** - Paste curl commands as-is
- **Works with Any API** - Stripe, GitHub, AWS, Azure, etc.
- **Faster Development** - No need to set up `HttpClient` for every test
- **Easy Migration** - Convert bash scripts to .NET without rewriting

## Supported Platforms

- .NET 8.0
- .NET Standard 2.0
- .NET Framework 4.7.2+ (Windows)

## Resources

- [GitHub](https://github.com/jacob/curl-dot-net)
- [NuGet](https://www.nuget.org/packages/CurlDotNet)
- [Documentation](https://github.com/jacob/curl-dot-net#readme)

---

**Sponsored by [IronSoftware](https://ironsoftware.com)** - Creators of IronPDF, IronOCR, IronXL, and IronBarcode

