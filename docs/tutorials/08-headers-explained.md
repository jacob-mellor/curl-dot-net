# Tutorial 8: Headers Explained

HTTP headers are key-value pairs that provide additional information about requests and responses. Understanding headers is crucial for working with APIs effectively.

## What Are Headers?

Headers are metadata sent with HTTP requests and responses:

```http
GET /api/users HTTP/1.1
Host: api.example.com
User-Agent: CurlDotNet/1.0
Accept: application/json
Authorization: Bearer token123
```

## Setting Request Headers

### Basic Header Setting
```csharp
var curl = new Curl();

// Add individual headers
curl.Headers.Add("Authorization", "Bearer your-token-here");
curl.Headers.Add("X-API-Key", "your-api-key");
curl.Headers.Add("Accept", "application/json");

var result = await curl.GetAsync("https://api.example.com/data");
```

### Headers for Specific Requests
```csharp
// Headers for a single request
var request = new CurlRequest
{
    Url = "https://api.example.com/data",
    Method = HttpMethod.Get,
    Headers = new Dictionary<string, string>
    {
        ["Authorization"] = "Bearer token123",
        ["X-Request-ID"] = Guid.NewGuid().ToString()
    }
};

var result = await curl.SendAsync(request);
```

## Common Request Headers

### Authorization
```csharp
// Bearer token (OAuth 2.0, JWT)
curl.Headers.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIs...");

// Basic authentication
var credentials = Convert.ToBase64String(
    Encoding.UTF8.GetBytes($"{username}:{password}")
);
curl.Headers.Add("Authorization", $"Basic {credentials}");

// API Key
curl.Headers.Add("X-API-Key", "your-api-key-here");
```

### Content-Type
```csharp
// JSON content
curl.Headers.Add("Content-Type", "application/json");

// Form data
curl.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

// Multipart form (file uploads)
curl.Headers.Add("Content-Type", "multipart/form-data");

// XML content
curl.Headers.Add("Content-Type", "application/xml");
```

### Accept
```csharp
// Specify what content types you can handle
curl.Headers.Add("Accept", "application/json");
curl.Headers.Add("Accept", "application/json, text/plain, */*");
curl.Headers.Add("Accept", "application/vnd.api+json");  // JSON API spec
```

### User-Agent
```csharp
// Identify your application
curl.Headers.Add("User-Agent", "MyApp/1.0.0 (Windows 10)");
curl.Headers.Add("User-Agent", "CurlDotNet/1.0 MyApplication/2.0");
```

### Cache Control
```csharp
// Prevent caching
curl.Headers.Add("Cache-Control", "no-cache");
curl.Headers.Add("Pragma", "no-cache");

// Allow caching
curl.Headers.Add("Cache-Control", "max-age=3600");  // Cache for 1 hour
```

## Reading Response Headers

### Access Response Headers
```csharp
var result = await curl.GetAsync("https://api.example.com/data");

if (result.IsSuccess)
{
    // Check if header exists
    if (result.Headers.ContainsKey("Content-Type"))
    {
        string contentType = result.Headers["Content-Type"];
        Console.WriteLine($"Content type: {contentType}");
    }

    // Safe access with TryGetValue
    if (result.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining))
    {
        Console.WriteLine($"API calls remaining: {remaining}");
    }

    // Iterate all headers
    foreach (var header in result.Headers)
    {
        Console.WriteLine($"{header.Key}: {header.Value}");
    }
}
```

### Common Response Headers
```csharp
// Content-Type - Media type of the response
var contentType = result.Headers["Content-Type"];  // "application/json; charset=utf-8"

// Content-Length - Size of response body
var length = result.Headers["Content-Length"];  // "1234"

// Location - URL for redirects or created resources
var location = result.Headers["Location"];  // "https://api.example.com/users/123"

// Set-Cookie - Cookies from server
var setCookie = result.Headers["Set-Cookie"];  // "session=abc123; HttpOnly"

// ETag - Version identifier for caching
var etag = result.Headers["ETag"];  // "W/\"123456789\""

// Last-Modified - When resource was last changed
var lastModified = result.Headers["Last-Modified"];  // "Wed, 21 Oct 2023 07:28:00 GMT"
```

## Working with Cookies

### Sending Cookies
```csharp
// Single cookie
curl.Headers.Add("Cookie", "session=abc123");

// Multiple cookies
curl.Headers.Add("Cookie", "session=abc123; user_id=456; theme=dark");

// Cookie with attributes
var cookie = "session=abc123; Path=/; Domain=.example.com";
curl.Headers.Add("Cookie", cookie);
```

### Receiving Cookies
```csharp
var result = await curl.GetAsync("https://api.example.com/login");

if (result.Headers.TryGetValue("Set-Cookie", out string setCookie))
{
    // Parse cookie
    // Set-Cookie: session=xyz789; Path=/; HttpOnly; Secure
    var parts = setCookie.Split(';');
    var sessionCookie = parts[0];  // "session=xyz789"

    // Use cookie in subsequent requests
    curl.Headers.Add("Cookie", sessionCookie);
}
```

## Custom Headers

### API-Specific Headers
```csharp
// Stripe API version
curl.Headers.Add("Stripe-Version", "2023-10-16");

// GitHub API version
curl.Headers.Add("X-GitHub-Api-Version", "2022-11-28");

// Custom correlation ID for tracking
curl.Headers.Add("X-Correlation-ID", Guid.NewGuid().ToString());

// Custom client information
curl.Headers.Add("X-Client-Version", "2.0.0");
curl.Headers.Add("X-Client-Platform", Environment.OSVersion.ToString());
```

### Request ID Headers
```csharp
// Add unique ID to each request for tracking
public class RequestTrackingCurl : Curl
{
    public RequestTrackingCurl()
    {
        BeforeRequest = request =>
        {
            request.Headers["X-Request-ID"] = Guid.NewGuid().ToString();
            request.Headers["X-Timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            return request;
        };
    }
}
```

## Conditional Requests

### If-None-Match (ETag)
```csharp
// First request - get ETag
var result = await curl.GetAsync("https://api.example.com/resource");
string etag = null;

if (result.Headers.TryGetValue("ETag", out string etagValue))
{
    etag = etagValue;
}

// Subsequent request - use ETag
if (etag != null)
{
    curl.Headers.Add("If-None-Match", etag);
}

var result2 = await curl.GetAsync("https://api.example.com/resource");

if (result2.StatusCode == HttpStatusCode.NotModified)
{
    Console.WriteLine("Resource hasn't changed, use cached version");
}
```

### If-Modified-Since
```csharp
// Check if resource changed since last fetch
var lastCheck = DateTime.UtcNow.AddHours(-1);
curl.Headers.Add("If-Modified-Since", lastCheck.ToString("R"));  // RFC1123 format

var result = await curl.GetAsync("https://api.example.com/data");

if (result.StatusCode == HttpStatusCode.NotModified)
{
    Console.WriteLine("No changes since last check");
}
```

## Rate Limiting Headers

### Reading Rate Limit Info
```csharp
public class RateLimitInfo
{
    public int Limit { get; set; }
    public int Remaining { get; set; }
    public DateTime ResetTime { get; set; }
}

public RateLimitInfo GetRateLimitInfo(CurlResult result)
{
    var info = new RateLimitInfo();

    if (result.Headers.TryGetValue("X-RateLimit-Limit", out string limit))
        info.Limit = int.Parse(limit);

    if (result.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining))
        info.Remaining = int.Parse(remaining);

    if (result.Headers.TryGetValue("X-RateLimit-Reset", out string reset))
    {
        // Unix timestamp to DateTime
        var unixTime = long.Parse(reset);
        info.ResetTime = DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
    }

    return info;
}

// Usage
var result = await curl.GetAsync("https://api.example.com/data");
var rateLimits = GetRateLimitInfo(result);

Console.WriteLine($"Calls remaining: {rateLimits.Remaining}/{rateLimits.Limit}");
Console.WriteLine($"Resets at: {rateLimits.ResetTime}");

if (rateLimits.Remaining == 0)
{
    var waitTime = rateLimits.ResetTime - DateTime.UtcNow;
    Console.WriteLine($"Rate limited. Wait {waitTime.TotalSeconds} seconds");
}
```

## CORS Headers

### Understanding CORS Headers
```csharp
// Request headers for CORS
curl.Headers.Add("Origin", "https://myapp.com");
curl.Headers.Add("Access-Control-Request-Method", "POST");
curl.Headers.Add("Access-Control-Request-Headers", "Content-Type, Authorization");

// Response headers to check
var result = await curl.GetAsync(url);

if (result.Headers.TryGetValue("Access-Control-Allow-Origin", out string allowedOrigin))
{
    Console.WriteLine($"CORS allowed from: {allowedOrigin}");
}

if (result.Headers.TryGetValue("Access-Control-Allow-Methods", out string allowedMethods))
{
    Console.WriteLine($"Allowed methods: {allowedMethods}");
}
```

## Header Management Patterns

### Header Interceptor
```csharp
public class ApiClient
{
    private readonly Curl _curl;
    private readonly string _apiKey;

    public ApiClient(string apiKey)
    {
        _apiKey = apiKey;
        _curl = new Curl();

        // Add default headers
        _curl.Headers.Add("X-API-Key", _apiKey);
        _curl.Headers.Add("Accept", "application/json");
        _curl.Headers.Add("User-Agent", "MyApp/1.0");
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        // Headers are automatically included
        var result = await _curl.GetAsync($"https://api.example.com{endpoint}");

        if (result.IsSuccess)
        {
            return JsonSerializer.Deserialize<T>(result.Data);
        }

        throw new ApiException(result.Error);
    }
}
```

### Dynamic Headers
```csharp
public class AuthenticatedCurl : Curl
{
    private string _token;

    public void SetToken(string token)
    {
        _token = token;
        Headers["Authorization"] = $"Bearer {token}";
    }

    public void ClearToken()
    {
        _token = null;
        Headers.Remove("Authorization");
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_token);
}
```

## Best Practices

1. **Set appropriate User-Agent** - Identify your application
2. **Use correct Content-Type** - Match your request body format
3. **Handle rate limiting** - Respect API limits
4. **Implement caching** - Use ETags and Last-Modified
5. **Add request tracking** - Use correlation IDs
6. **Secure sensitive headers** - Never log auth tokens
7. **Validate response headers** - Check Content-Type before parsing
8. **Use header constants** - Avoid typos in header names

## Summary

Headers control how HTTP requests and responses behave:
- Set request headers to authenticate and configure requests
- Read response headers for metadata and rate limits
- Use standard headers correctly for compatibility
- Implement patterns for header management
- Respect caching and rate limiting headers

## What's Next?

Learn about [authentication basics](09-authentication-basics.md) and how to securely access protected APIs.

---

[← Previous: JSON for Beginners](07-json-for-beginners.md) | [Next: Authentication Basics →](09-authentication-basics.md)