# Tutorial 05: Understanding CurlDotNet Results

Learn how to work with the response data from your HTTP requests.

## 📊 The CurlResult Object

When you make a request with CurlDotNet, you get back a `CurlResult` object. This object contains everything about the response.

## Basic Properties

### IsSuccess - Did It Work?

The most important property tells you if the request succeeded:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");

if (result.IsSuccess)
{
    Console.WriteLine("✅ Request succeeded!");
}
else
{
    Console.WriteLine("❌ Request failed!");
}
```

### What Makes a Request Successful?

A request is considered successful when:
- Connection was established
- Response was received
- Status code is in the 200-299 range

## Response Content

### Getting the Response Body

The `Content` property contains the response body as a string:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

// The raw response content
string responseBody = result.Content;
Console.WriteLine(responseBody);
```

### Example Output

```json
{
  "login": "octocat",
  "id": 583231,
  "avatar_url": "https://avatars.githubusercontent.com/u/583231?v=4",
  "name": "The Octocat",
  "company": "GitHub",
  "blog": "https://github.blog"
}
```

## Status Codes

### Understanding HTTP Status Codes

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");

Console.WriteLine($"Status Code: {result.StatusCode}");

// Check specific status codes
switch (result.StatusCode)
{
    case 200:
        Console.WriteLine("OK - Request succeeded");
        break;
    case 201:
        Console.WriteLine("Created - Resource was created");
        break;
    case 404:
        Console.WriteLine("Not Found - Resource doesn't exist");
        break;
    case 401:
        Console.WriteLine("Unauthorized - Need to login");
        break;
    case 500:
        Console.WriteLine("Server Error - Something went wrong on the server");
        break;
}
```

### Common Status Code Ranges

```csharp
// Helper method to understand status codes
string GetStatusCategory(int statusCode)
{
    return statusCode switch
    {
        >= 100 and < 200 => "Informational",
        >= 200 and < 300 => "Success",
        >= 300 and < 400 => "Redirect",
        >= 400 and < 500 => "Client Error",
        >= 500 and < 600 => "Server Error",
        _ => "Unknown"
    };
}

var category = GetStatusCategory(result.StatusCode);
Console.WriteLine($"Status {result.StatusCode} is: {category}");
```

## Response Headers

### Accessing Headers

Headers provide metadata about the response:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");

// Get all headers
foreach (var header in result.Headers)
{
    Console.WriteLine($"{header.Key}: {header.Value}");
}

// Get specific header
if (result.Headers.TryGetValue("Content-Type", out string contentType))
{
    Console.WriteLine($"Content Type: {contentType}");
}
```

### Common Headers to Check

```csharp
// Content information
var contentType = result.Headers.GetValueOrDefault("Content-Type", "unknown");
var contentLength = result.Headers.GetValueOrDefault("Content-Length", "0");

Console.WriteLine($"Type: {contentType}");
Console.WriteLine($"Size: {contentLength} bytes");

// Caching information
var cacheControl = result.Headers.GetValueOrDefault("Cache-Control", "no-cache");
var etag = result.Headers.GetValueOrDefault("ETag", "");

Console.WriteLine($"Cache: {cacheControl}");
Console.WriteLine($"ETag: {etag}");

// Rate limiting
var rateLimit = result.Headers.GetValueOrDefault("X-RateLimit-Remaining", "");
var rateLimitReset = result.Headers.GetValueOrDefault("X-RateLimit-Reset", "");

if (!string.IsNullOrEmpty(rateLimit))
{
    Console.WriteLine($"API calls remaining: {rateLimit}");
}
```

## Working with JSON Responses

### Parsing JSON

Most APIs return JSON data. CurlDotNet makes it easy to work with:

```csharp
// Method 1: Dynamic parsing
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");
dynamic user = result.ParseJson();

Console.WriteLine($"Name: {user.name}");
Console.WriteLine($"Company: {user.company}");
Console.WriteLine($"Followers: {user.followers}");
```

### Strongly Typed JSON

Better approach with classes:

```csharp
// Define a class for the response
public class GitHubUser
{
    public string Login { get; set; }
    public string Name { get; set; }
    public string Company { get; set; }
    public string Blog { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
}

// Parse to your class
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");
var user = result.ParseJson<GitHubUser>();

Console.WriteLine($"Name: {user.Name}");
Console.WriteLine($"Followers: {user.Followers}");
```

### Working with Arrays

```csharp
public class Repository
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Stars { get; set; }
}

// Get list of repositories
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat/repos");
var repos = result.ParseJson<List<Repository>>();

foreach (var repo in repos)
{
    Console.WriteLine($"📦 {repo.Name}: {repo.Description}");
    Console.WriteLine($"   ⭐ Stars: {repo.Stars}");
}
```

## Error Information

### When Requests Fail

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com/notfound");

if (!result.IsSuccess)
{
    Console.WriteLine($"Error: {result.Error}");
    Console.WriteLine($"Status: {result.StatusCode}");

    // Check if we got an error response body
    if (!string.IsNullOrEmpty(result.Content))
    {
        Console.WriteLine($"Server said: {result.Content}");
    }
}
```

### Common Error Patterns

```csharp
// Many APIs return error details in JSON
if (!result.IsSuccess && !string.IsNullOrEmpty(result.Content))
{
    try
    {
        dynamic error = result.ParseJson();
        Console.WriteLine($"Error: {error.message}");
        Console.WriteLine($"Code: {error.code}");
    }
    catch
    {
        // Not JSON, show raw error
        Console.WriteLine($"Error: {result.Content}");
    }
}
```

## Timing Information

### How Long Did It Take?

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");

Console.WriteLine($"Request took: {result.ElapsedTime.TotalMilliseconds}ms");

if (result.ElapsedTime.TotalSeconds > 5)
{
    Console.WriteLine("⚠️ This request was slow!");
}
```

### Performance Monitoring

```csharp
public async Task<CurlResult> MakeRequestWithTiming(string url)
{
    var sw = System.Diagnostics.Stopwatch.StartNew();
    var result = await Curl.ExecuteAsync($"curl {url}");
    sw.Stop();

    Console.WriteLine($"Network time: {result.ElapsedTime.TotalMilliseconds}ms");
    Console.WriteLine($"Total time: {sw.ElapsedMilliseconds}ms");
    Console.WriteLine($"Processing overhead: {sw.ElapsedMilliseconds - result.ElapsedTime.TotalMilliseconds}ms");

    return result;
}
```

## Verbose Output

### Debugging with Verbose Mode

```csharp
var result = await Curl.ExecuteAsync("curl -v https://api.example.com");

// Verbose output shows the entire request/response cycle
Console.WriteLine("=== VERBOSE OUTPUT ===");
Console.WriteLine(result.VerboseOutput);
```

### What's in Verbose Output?

- DNS resolution details
- Connection information
- SSL/TLS handshake
- Request headers sent
- Response headers received
- Timing information

## Complete Example

### Putting It All Together

```csharp
public class ApiResponse
{
    public async Task ProcessApiCall()
    {
        var result = await Curl.ExecuteAsync(@"
            curl https://api.example.com/data \
            -H 'Accept: application/json'
        ");

        // 1. Check if successful
        if (!result.IsSuccess)
        {
            Console.WriteLine($"❌ Request failed: {result.Error}");
            Console.WriteLine($"   Status code: {result.StatusCode}");
            return;
        }

        // 2. Check status code
        Console.WriteLine($"✅ Success! Status: {result.StatusCode}");

        // 3. Check headers
        if (result.Headers.TryGetValue("Content-Type", out string contentType))
        {
            Console.WriteLine($"📄 Content type: {contentType}");
        }

        // 4. Parse content
        if (contentType?.Contains("json") == true)
        {
            try
            {
                dynamic data = result.ParseJson();
                Console.WriteLine($"📊 Got {data.count} items");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse JSON: {ex.Message}");
            }
        }
        else
        {
            // Not JSON, just show content
            Console.WriteLine($"📝 Response: {result.Content}");
        }

        // 5. Performance info
        Console.WriteLine($"⏱️ Took {result.ElapsedTime.TotalMilliseconds:F2}ms");

        // 6. Rate limit check
        if (result.Headers.TryGetValue("X-RateLimit-Remaining", out string remaining))
        {
            Console.WriteLine($"📊 API calls remaining: {remaining}");
        }
    }
}
```

## Best Practices

### 1. Always Check IsSuccess

```csharp
// ❌ Bad: Assuming success
var result = await Curl.ExecuteAsync("curl https://api.example.com");
var data = result.ParseJson(); // Could throw if failed!

// ✅ Good: Check first
var result = await Curl.ExecuteAsync("curl https://api.example.com");
if (result.IsSuccess)
{
    var data = result.ParseJson();
}
```

### 2. Handle JSON Parsing Errors

```csharp
// ✅ Safe JSON parsing
try
{
    var data = result.ParseJson<MyClass>();
    // Use data...
}
catch (JsonException ex)
{
    Console.WriteLine($"Invalid JSON: {ex.Message}");
    Console.WriteLine($"Raw response: {result.Content}");
}
```

### 3. Check Content Type

```csharp
// ✅ Verify content type before parsing
if (result.Headers.GetValueOrDefault("Content-Type", "").Contains("json"))
{
    var data = result.ParseJson();
}
else
{
    Console.WriteLine("Response is not JSON");
}
```

## Common Patterns

### Retry on Failure

```csharp
public async Task<CurlResult> RequestWithRetry(string command, int maxRetries = 3)
{
    CurlResult result = null;

    for (int i = 0; i < maxRetries; i++)
    {
        result = await Curl.ExecuteAsync(command);

        if (result.IsSuccess)
            return result;

        Console.WriteLine($"Attempt {i + 1} failed: {result.Error}");

        if (i < maxRetries - 1)
            await Task.Delay(1000 * (i + 1)); // Wait longer each time
    }

    return result;
}
```

### Extract Pagination Info

```csharp
public class PaginatedResponse<T>
{
    public List<T> Items { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public bool HasMore { get; set; }

    public static PaginatedResponse<T> FromResult(CurlResult result)
    {
        var response = new PaginatedResponse<T>();

        // Parse items from body
        response.Items = result.ParseJson<List<T>>();

        // Get pagination from headers
        if (result.Headers.TryGetValue("X-Total-Pages", out string totalPages))
            response.TotalPages = int.Parse(totalPages);

        if (result.Headers.TryGetValue("X-Current-Page", out string currentPage))
            response.CurrentPage = int.Parse(currentPage);

        response.HasMore = response.CurrentPage < response.TotalPages;

        return response;
    }
}
```

## 🎯 Key Takeaways

1. **Always check `IsSuccess`** before using the response
2. **Status codes** tell you what happened (200=OK, 404=Not Found, etc.)
3. **Headers** contain important metadata
4. **ParseJson()** makes working with JSON easy
5. **Error details** are often in the response body
6. **ElapsedTime** helps monitor performance
7. **VerboseOutput** is great for debugging

## Next Steps

Ready to handle errors properly? Continue to [Tutorial 06: Handling Errors](06-handling-errors.md)

---
*Part 5 of the CurlDotNet Tutorial Series*