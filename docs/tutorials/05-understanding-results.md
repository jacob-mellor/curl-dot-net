# Tutorial 5: Understanding Results

Every CurlDotNet operation returns a `CurlResult` object. Understanding this result structure is crucial for building robust applications.

## The CurlResult Object

When you make any request with CurlDotNet, you get back a `CurlResult`:

```csharp
var curl = new Curl();
CurlResult result = await curl.GetAsync("https://api.example.com/data");
```

## Key Properties

### IsSuccess
The most important property - tells you if the request completed successfully:

```csharp
if (result.IsSuccess)
{
    // Request succeeded - safe to use result.Data
    Console.WriteLine("Success! Data received:");
    Console.WriteLine(result.Data);
}
else
{
    // Request failed - check result.Error
    Console.WriteLine($"Request failed: {result.Error}");
}
```

### Data
Contains the response body as a string when the request succeeds:

```csharp
if (result.IsSuccess)
{
    string responseBody = result.Data;

    // Parse JSON if that's what you're expecting
    var jsonData = JsonSerializer.Deserialize<MyModel>(result.Data);
}
```

### StatusCode
The HTTP status code returned by the server:

```csharp
Console.WriteLine($"Server returned: {result.StatusCode}");

// Check specific status codes
if (result.StatusCode == HttpStatusCode.NotFound)
{
    Console.WriteLine("The requested resource was not found.");
}
else if (result.StatusCode == HttpStatusCode.Unauthorized)
{
    Console.WriteLine("Authentication required.");
}
```

### Headers
A dictionary containing all response headers:

```csharp
if (result.Headers.ContainsKey("Content-Type"))
{
    string contentType = result.Headers["Content-Type"];
    Console.WriteLine($"Response type: {contentType}");
}

// Iterate through all headers
foreach (var header in result.Headers)
{
    Console.WriteLine($"{header.Key}: {header.Value}");
}
```

### Error
Contains error information when the request fails:

```csharp
if (!result.IsSuccess)
{
    Console.WriteLine($"Error details: {result.Error}");

    // The error message is descriptive and helpful
    // Examples:
    // - "Connection timeout after 30 seconds"
    // - "DNS resolution failed for hostname"
    // - "SSL certificate verification failed"
}
```

## Common Patterns

### Pattern 1: Simple Success Check
```csharp
var result = await curl.GetAsync(url);

if (result.IsSuccess)
{
    ProcessData(result.Data);
}
else
{
    LogError(result.Error);
}
```

### Pattern 2: Detailed Status Handling
```csharp
var result = await curl.GetAsync(url);

switch (result.StatusCode)
{
    case HttpStatusCode.OK:
        ProcessData(result.Data);
        break;

    case HttpStatusCode.NotModified:
        Console.WriteLine("Content hasn't changed");
        break;

    case HttpStatusCode.NotFound:
        Console.WriteLine("Resource not found");
        break;

    case HttpStatusCode.Unauthorized:
        Console.WriteLine("Please authenticate");
        break;

    default:
        Console.WriteLine($"Unexpected status: {result.StatusCode}");
        break;
}
```

### Pattern 3: Working with Headers
```csharp
var result = await curl.GetAsync(url);

if (result.IsSuccess)
{
    // Check content type before parsing
    if (result.Headers.TryGetValue("Content-Type", out string contentType))
    {
        if (contentType.Contains("application/json"))
        {
            var jsonData = JsonSerializer.Deserialize<MyModel>(result.Data);
        }
        else if (contentType.Contains("text/html"))
        {
            ProcessHtml(result.Data);
        }
    }

    // Check for rate limiting headers
    if (result.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining))
    {
        Console.WriteLine($"API calls remaining: {remaining}");
    }
}
```

### Pattern 4: Response Time Tracking
```csharp
var stopwatch = Stopwatch.StartNew();
var result = await curl.GetAsync(url);
stopwatch.Stop();

Console.WriteLine($"Request took: {stopwatch.ElapsedMilliseconds}ms");
Console.WriteLine($"Status: {result.StatusCode}");
Console.WriteLine($"Success: {result.IsSuccess}");
```

## Working with Different Content Types

### JSON Responses
```csharp
var result = await curl.GetAsync("https://api.example.com/user/123");

if (result.IsSuccess)
{
    try
    {
        var user = JsonSerializer.Deserialize<User>(result.Data);
        Console.WriteLine($"User: {user.Name}");
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"Failed to parse JSON: {ex.Message}");
    }
}
```

### XML Responses
```csharp
var result = await curl.GetAsync("https://api.example.com/data.xml");

if (result.IsSuccess)
{
    var doc = XDocument.Parse(result.Data);
    var rootElement = doc.Root;
    // Process XML...
}
```

### Binary Data
```csharp
var result = await curl.GetAsync("https://example.com/image.jpg");

if (result.IsSuccess)
{
    // For binary data, use GetBytesAsync instead
    var bytes = await curl.GetBytesAsync("https://example.com/image.jpg");
    File.WriteAllBytes("image.jpg", bytes);
}
```

## Best Practices

### 1. Always Check IsSuccess First
```csharp
// Good
if (result.IsSuccess)
{
    UseData(result.Data);
}

// Bad - Data might be null if request failed
UseData(result.Data);  // Potential null reference
```

### 2. Log Both Success and Failure
```csharp
var result = await curl.GetAsync(url);

if (result.IsSuccess)
{
    _logger.LogInformation($"Request succeeded: {url}");
    ProcessData(result.Data);
}
else
{
    _logger.LogError($"Request failed: {url} - {result.Error}");
}
```

### 3. Use Status Codes for Business Logic
```csharp
var result = await curl.GetAsync($"/api/users/{userId}");

if (result.StatusCode == HttpStatusCode.NotFound)
{
    // User doesn't exist - this might be expected
    return null;
}
else if (!result.IsSuccess)
{
    // Other errors are unexpected
    throw new ApplicationException($"Failed to get user: {result.Error}");
}

return JsonSerializer.Deserialize<User>(result.Data);
```

### 4. Extract Common Patterns
```csharp
public static class CurlResultExtensions
{
    public static T DeserializeJson<T>(this CurlResult result)
    {
        if (!result.IsSuccess)
            throw new InvalidOperationException($"Cannot deserialize failed request: {result.Error}");

        return JsonSerializer.Deserialize<T>(result.Data);
    }

    public static bool IsJson(this CurlResult result)
    {
        return result.Headers.TryGetValue("Content-Type", out var ct)
               && ct.Contains("application/json");
    }
}

// Usage
var user = result.DeserializeJson<User>();
```

## Common Mistakes to Avoid

### Mistake 1: Not Checking IsSuccess
```csharp
// Wrong - might crash if request failed
var data = JsonSerializer.Deserialize<MyData>(result.Data);

// Right
if (result.IsSuccess)
{
    var data = JsonSerializer.Deserialize<MyData>(result.Data);
}
```

### Mistake 2: Ignoring Status Codes
```csharp
// Wrong - treats all failures the same
if (!result.IsSuccess)
{
    throw new Exception("Request failed");
}

// Right - handle different failures differently
if (result.StatusCode == HttpStatusCode.TooManyRequests)
{
    // Wait and retry
}
else if (result.StatusCode == HttpStatusCode.Unauthorized)
{
    // Refresh authentication
}
```

### Mistake 3: Not Logging Errors
```csharp
// Wrong - silent failure
if (result.IsSuccess)
{
    ProcessData(result.Data);
}

// Right - log the error
if (result.IsSuccess)
{
    ProcessData(result.Data);
}
else
{
    _logger.LogError($"Request failed: {result.Error}");
}
```

## Summary

The `CurlResult` object provides everything you need to handle HTTP responses:
- Check `IsSuccess` to determine if the request succeeded
- Access response data through the `Data` property
- Use `StatusCode` for specific HTTP status handling
- Read response headers from the `Headers` dictionary
- Get detailed error information from the `Error` property

Understanding these properties and using them correctly is the foundation for building reliable HTTP clients with CurlDotNet.

## What's Next?

In the next tutorial, we'll dive deeper into [error handling](06-handling-errors.html) and learn how to build resilient applications that gracefully handle network failures.

---

[← Previous: Your First Request](04-your-first-request.html) | [Next: Handling Errors →](06-handling-errors.html)