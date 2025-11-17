# Tutorial 08: HTTP Headers Explained

## 🎯 What You'll Learn

- What HTTP headers are and why they matter
- Common request headers and when to use them
- Understanding response headers
- How to set custom headers
- Working with authentication headers
- Best practices for header management

## 📚 Prerequisites

- [Tutorial 04: Your First Request](04-your-first-request.md)
- [Tutorial 05: Understanding Results](05-understanding-results.md)
- [Tutorial 07: JSON for Beginners](07-json-for-beginners.md)

## 🤔 What Are HTTP Headers?

Think of HTTP headers like the envelope of a letter:
- The **letter content** is the request/response body
- The **envelope** contains metadata: sender address, recipient, postage, handling instructions

**HTTP headers** are metadata sent with every request and response. They tell servers and clients important information about the data being transferred.

### Why Headers Matter

Headers control:
- **Content type** - Is it JSON, HTML, or an image?
- **Authentication** - Who are you?
- **Caching** - Should this be cached?
- **Encoding** - How is the data compressed?
- **Cookies** - Session information
- **CORS** - Cross-origin access rules

## 📤 Request Headers (What You Send)

### Common Request Headers

| Header | Purpose | Example |
|--------|---------|---------|
| `Accept` | What formats you can handle | `application/json` |
| `Content-Type` | Format of data you're sending | `application/json` |
| `Authorization` | Your credentials | `Bearer token123` |
| `User-Agent` | Your client identifier | `MyApp/1.0` |
| `Accept-Language` | Preferred language | `en-US` |
| `Accept-Encoding` | Compression methods you support | `gzip, deflate` |

### Setting Headers in CurlDotNet

#### Method 1: Using -H Flag

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        var result = await Curl.ExecuteAsync(@"
            curl https://api.github.com/users/octocat \
              -H 'Accept: application/json' \
              -H 'User-Agent: MyApp/1.0'
        ");

        Console.WriteLine($"Status: {result.StatusCode}");
        Console.WriteLine($"Body: {result.Body}");
    }
}
```

#### Method 2: Multiple Headers

```csharp
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com/data \
      -H 'Accept: application/json' \
      -H 'Authorization: Bearer YOUR_TOKEN' \
      -H 'X-Custom-Header: CustomValue' \
      -H 'User-Agent: MyAwesomeApp/2.0'
");
```

#### Method 3: Using Builder API

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://api.github.com/users/octocat")
    .WithHeader("Accept", "application/json")
    .WithHeader("User-Agent", "MyApp/1.0")
    .WithHeader("X-Custom-ID", "12345")
    .ExecuteAsync();
```

## 📥 Response Headers (What You Get Back)

### Common Response Headers

| Header | Purpose | Example |
|--------|---------|---------|
| `Content-Type` | Format of response | `application/json` |
| `Content-Length` | Size in bytes | `1547` |
| `Date` | When response was sent | `Mon, 18 Nov 2025 12:00:00 GMT` |
| `Server` | Server software | `nginx/1.19.0` |
| `Cache-Control` | Caching rules | `max-age=3600` |
| `Set-Cookie` | Session cookies | `sessionid=abc123` |

### Reading Response Headers

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");

// Get specific header
if (result.Headers.TryGetValue("Content-Type", out string contentType))
{
    Console.WriteLine($"Content-Type: {contentType}");
}

// Get all headers
Console.WriteLine("All Response Headers:");
foreach (var header in result.Headers)
{
    Console.WriteLine($"{header.Key}: {header.Value}");
}
```

### Example Output

```
All Response Headers:
Content-Type: application/json; charset=utf-8
Date: Mon, 18 Nov 2025 12:00:00 GMT
Server: GitHub.com
Cache-Control: public, max-age=60
X-GitHub-Request-Id: ABC123:DEF456
```

## 🔑 Content-Type Header

### When Sending Data (Request)

Tell the server what format you're sending:

```csharp
// Sending JSON
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/users \
      -H 'Content-Type: application/json' \
      -d '{""name"":""Alice"",""email"":""alice@example.com""}'
");
```

### When Receiving Data (Response)

The server tells you what format it's sending:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com/data");

string contentType = result.Headers.GetValueOrDefault("Content-Type", "unknown");

if (contentType.Contains("application/json"))
{
    var data = result.ParseJson<dynamic>();
    Console.WriteLine($"Got JSON: {data}");
}
else if (contentType.Contains("text/html"))
{
    Console.WriteLine($"Got HTML: {result.Body}");
}
else if (contentType.Contains("image/"))
{
    Console.WriteLine("Got an image");
}
```

### Common Content-Type Values

| Value | Description | Use Case |
|-------|-------------|----------|
| `application/json` | JSON data | APIs |
| `application/xml` | XML data | SOAP APIs |
| `text/html` | HTML webpage | Websites |
| `text/plain` | Plain text | Simple data |
| `application/x-www-form-urlencoded` | Form data | HTML forms |
| `multipart/form-data` | File uploads | File upload forms |
| `image/jpeg`, `image/png` | Images | Image files |

## 🔐 Accept Header

Tell the server what formats you can handle:

### Basic Example

```csharp
// Request JSON specifically
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com/data \
      -H 'Accept: application/json'
");
```

### Multiple Formats with Priority

```csharp
// Prefer JSON, but accept XML too
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com/data \
      -H 'Accept: application/json, application/xml;q=0.9, */*;q=0.8'
");
```

The `q` values indicate priority:
- `q=1.0` (default) - Most preferred
- `q=0.9` - Second choice
- `q=0.8` - Third choice

## 🔐 Authorization Header

### Bearer Token (Most Common)

Used by most modern APIs:

```csharp
var token = "your_api_token_here";

var result = await Curl.ExecuteAsync($@"
    curl https://api.example.com/protected \
      -H 'Authorization: Bearer {token}'
");

if (result.IsSuccess)
{
    Console.WriteLine("Authentication successful!");
    Console.WriteLine(result.Body);
}
else if (result.StatusCode == 401)
{
    Console.WriteLine("Authentication failed - invalid token");
}
```

### Basic Authentication

Username and password:

```csharp
var username = "alice";
var password = "secret123";
var credentials = Convert.ToBase64String(
    System.Text.Encoding.UTF8.GetBytes($"{username}:{password}")
);

var result = await Curl.ExecuteAsync($@"
    curl https://api.example.com/data \
      -H 'Authorization: Basic {credentials}'
");
```

Or use curl's built-in basic auth:

```csharp
var result = await Curl.ExecuteAsync(@"
    curl -u alice:secret123 https://api.example.com/data
");
```

### API Key in Header

Some APIs use custom headers:

```csharp
var apiKey = "your_api_key_here";

var result = await Curl.ExecuteAsync($@"
    curl https://api.example.com/data \
      -H 'X-API-Key: {apiKey}'
");
```

## 🌐 User-Agent Header

Identifies your application:

### Why It Matters

- Some APIs require it
- Helps API providers track usage
- Enables API providers to contact you if needed

### Setting User-Agent

```csharp
var result = await Curl.ExecuteAsync(@"
    curl https://api.github.com \
      -H 'User-Agent: MyApp/1.0 (contact@example.com)'
");
```

### Best Practice Format

```
AppName/Version (Contact Info)
```

Examples:
- `MyWeatherApp/2.1 (support@myapp.com)`
- `DataAnalyzer/1.0 (github.com/user/repo)`
- `MyBot/3.5 (admin@example.com; +http://mybot.com)`

## 🔄 Custom Headers

### API-Specific Headers

Many APIs use custom headers (usually prefixed with `X-`):

```csharp
// GitHub
var result = await Curl.ExecuteAsync(@"
    curl https://api.github.com/user \
      -H 'Authorization: Bearer token' \
      -H 'X-GitHub-Api-Version: 2022-11-28'
");

// Stripe
var result = await Curl.ExecuteAsync(@"
    curl https://api.stripe.com/v1/charges \
      -H 'Authorization: Bearer sk_test_...' \
      -H 'Stripe-Version: 2023-10-16'
");

// Custom business logic
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com/data \
      -H 'X-Request-ID: unique-id-12345' \
      -H 'X-Tenant-ID: tenant-abc' \
      -H 'X-Feature-Flag: new-ui-enabled'
");
```

### Request Tracing

Track requests across systems:

```csharp
var requestId = Guid.NewGuid().ToString();

var result = await Curl.ExecuteAsync($@"
    curl https://api.example.com/data \
      -H 'X-Request-ID: {requestId}' \
      -H 'X-Correlation-ID: {requestId}'
");

Console.WriteLine($"Request ID: {requestId}");
Console.WriteLine($"Status: {result.StatusCode}");
```

## 📦 Content Encoding

### Accepting Compressed Responses

Save bandwidth:

```csharp
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com/large-data \
      -H 'Accept-Encoding: gzip, deflate, br' \
      --compressed
");

Console.WriteLine($"Response size: {result.Body.Length} bytes");
```

### Response Headers for Encoding

```csharp
var result = await Curl.ExecuteAsync("curl --compressed https://api.example.com");

if (result.Headers.TryGetValue("Content-Encoding", out string encoding))
{
    Console.WriteLine($"Response was compressed with: {encoding}");
}
```

## 🍪 Cookies

### Reading Cookies from Response

```csharp
var result = await Curl.ExecuteAsync("curl -i https://example.com");

if (result.Headers.TryGetValue("Set-Cookie", out string cookies))
{
    Console.WriteLine($"Cookies set: {cookies}");
}
```

### Sending Cookies with Request

```csharp
var result = await Curl.ExecuteAsync(@"
    curl https://example.com \
      -H 'Cookie: sessionid=abc123; userid=456'
");
```

### Using curl Cookie Jar

```csharp
// Save cookies
await Curl.ExecuteAsync(@"
    curl https://example.com/login \
      -d 'user=alice&pass=secret' \
      -c cookies.txt
");

// Use saved cookies
var result = await Curl.ExecuteAsync(@"
    curl https://example.com/protected \
      -b cookies.txt
");
```

## 🎯 Complete Example: API Client with Headers

```csharp
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CurlDotNet;

public class ApiClient
{
    private readonly string baseUrl;
    private readonly string apiKey;
    private readonly Dictionary<string, string> defaultHeaders;

    public ApiClient(string baseUrl, string apiKey)
    {
        this.baseUrl = baseUrl;
        this.apiKey = apiKey;

        // Set up default headers
        this.defaultHeaders = new Dictionary<string, string>
        {
            ["Accept"] = "application/json",
            ["Content-Type"] = "application/json",
            ["User-Agent"] = "MyApp/1.0 (contact@example.com)",
            ["X-API-Key"] = apiKey
        };
    }

    public async Task<CurlResult> GetAsync(string endpoint)
    {
        var headerString = BuildHeaderString();

        var result = await Curl.ExecuteAsync($@"
            curl {baseUrl}/{endpoint} \
              {headerString}
        ");

        LogRequest("GET", endpoint, result);
        return result;
    }

    public async Task<CurlResult> PostAsync(string endpoint, string jsonBody)
    {
        var headerString = BuildHeaderString();

        var result = await Curl.ExecuteAsync($@"
            curl -X POST {baseUrl}/{endpoint} \
              {headerString} \
              -d '{jsonBody}'
        ");

        LogRequest("POST", endpoint, result);
        return result;
    }

    private string BuildHeaderString()
    {
        var headers = new List<string>();

        foreach (var header in defaultHeaders)
        {
            headers.Add($"-H '{header.Key}: {header.Value}'");
        }

        return string.Join(" \\\n      ", headers);
    }

    private void LogRequest(string method, string endpoint, CurlResult result)
    {
        Console.WriteLine($"[{method}] {endpoint}");
        Console.WriteLine($"  Status: {result.StatusCode}");
        Console.WriteLine($"  Content-Type: {result.Headers.GetValueOrDefault("Content-Type", "unknown")}");

        if (result.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining))
        {
            Console.WriteLine($"  Rate Limit Remaining: {remaining}");
        }
    }
}

// Usage
class Program
{
    static async Task Main()
    {
        var client = new ApiClient(
            baseUrl: "https://api.example.com",
            apiKey: "your_api_key_here"
        );

        // GET request
        var getResult = await client.GetAsync("users/123");
        if (getResult.IsSuccess)
        {
            var user = getResult.ParseJson<dynamic>();
            Console.WriteLine($"User: {user.name}");
        }

        // POST request
        var postResult = await client.PostAsync(
            "users",
            "{\"name\":\"Alice\",\"email\":\"alice@example.com\"}"
        );

        if (postResult.IsSuccess)
        {
            Console.WriteLine("User created successfully!");
        }
    }
}
```

## 🎓 Header Best Practices

### 1. Always Set Content-Type When Sending Data

```csharp
// Good
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/users \
      -H 'Content-Type: application/json' \
      -d '{""name"":""Alice""}'
");

// Bad - server might misinterpret your data
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/users \
      -d '{""name"":""Alice""}'
");
```

### 2. Set Accept Header for APIs

```csharp
// Good - explicitly request JSON
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com/data \
      -H 'Accept: application/json'
");
```

### 3. Use Meaningful User-Agent

```csharp
// Good
-H 'User-Agent: MyApp/1.0 (admin@myapp.com)'

// Bad
-H 'User-Agent: Mozilla/5.0 ...'  // Don't pretend to be a browser
```

### 4. Don't Hardcode Sensitive Headers

```csharp
// Bad
var token = "sk_live_abc123def456";  // Hardcoded!

// Good
var token = Environment.GetEnvironmentVariable("API_TOKEN");
var result = await Curl.ExecuteAsync($@"
    curl https://api.example.com/data \
      -H 'Authorization: Bearer {token}'
");
```

### 5. Check Response Headers

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com/data");

// Check rate limiting
if (result.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining))
{
    int remainingCalls = int.Parse(remaining);
    if (remainingCalls < 10)
    {
        Console.WriteLine($"Warning: Only {remainingCalls} API calls remaining!");
    }
}

// Check for deprecation warnings
if (result.Headers.TryGetValue("Warning", out string warning))
{
    Console.WriteLine($"API Warning: {warning}");
}
```

## 🎓 Key Takeaways

1. **Headers are metadata** about your request and response
2. **Content-Type** tells what format data is in
3. **Accept** tells what formats you can handle
4. **Authorization** provides credentials
5. **User-Agent** identifies your application
6. **Custom headers** (X-*) are API-specific
7. **Response headers** contain important metadata
8. **Always check** rate limit and warning headers

## 🚀 Next Steps

Now that you understand headers:

1. **Next Tutorial** → [Authentication Basics](09-authentication-basics.md)
2. **Practice** - Try different header combinations
3. **Experiment** - Check headers from different APIs
4. **Read** - Check API documentation for required headers

## 📚 Summary

HTTP headers are essential for API communication. You've learned how to:
- Set request headers with `-H` flag
- Read response headers from results
- Use common headers like Content-Type and Authorization
- Implement custom headers for specific APIs
- Follow best practices for header management

Headers give you fine-grained control over your HTTP requests!

---

**Ready to learn about authentication?** → [Authentication Basics](09-authentication-basics.md)

**Need help?** Check the [Troubleshooting Guide](../troubleshooting/README.md) or ask in [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
