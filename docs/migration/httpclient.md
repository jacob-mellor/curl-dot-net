# Migrating from HttpClient to CurlDotNet

This guide helps you migrate from `System.Net.Http.HttpClient` to CurlDotNet. While HttpClient is built into .NET, CurlDotNet offers simpler APIs, better error handling, and curl's proven reliability.

## Key Differences

| Feature | HttpClient | CurlDotNet |
|---------|------------|------------|
| Initialization | Complex setup with handlers | Simple, ready to use |
| Error Handling | Exceptions + status codes | Result pattern with clear errors |
| Content Reading | Multiple steps | Direct access to data |
| Default Behavior | Must configure everything | Sensible defaults |
| curl Compatibility | None | 100% compatible |

## Basic Migration Examples

### Simple GET Request

**HttpClient:**
```csharp
using var client = new HttpClient();
try
{
    var response = await client.GetAsync("https://api.example.com/users");
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStringAsync();
    Console.WriteLine(content);
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

**CurlDotNet:**
```csharp
var curl = new Curl();
var result = await curl.GetAsync("https://api.example.com/users");

if (result.IsSuccess)
{
    Console.WriteLine(result.Data);
}
else
{
    Console.WriteLine($"Error: {result.Error}");
}
```

### POST with JSON

**HttpClient:**
```csharp
using var client = new HttpClient();
var json = JsonSerializer.Serialize(new { name = "John", age = 30 });
var content = new StringContent(json, Encoding.UTF8, "application/json");

try
{
    var response = await client.PostAsync("https://api.example.com/users", content);
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadAsStringAsync();
    Console.WriteLine(result);
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

**CurlDotNet:**
```csharp
var curl = new Curl();
var data = new { name = "John", age = 30 };
var result = await curl.PostJsonAsync("https://api.example.com/users", data);

if (result.IsSuccess)
{
    Console.WriteLine(result.Data);
}
else
{
    Console.WriteLine($"Error: {result.Error}");
}
```

### Setting Headers

**HttpClient:**
```csharp
using var client = new HttpClient();
client.DefaultRequestHeaders.Add("Authorization", "Bearer token123");
client.DefaultRequestHeaders.Add("X-Custom-Header", "value");

var response = await client.GetAsync("https://api.example.com/protected");
```

**CurlDotNet:**
```csharp
var curl = new Curl();
curl.Headers.Add("Authorization", "Bearer token123");
curl.Headers.Add("X-Custom-Header", "value");

var result = await curl.GetAsync("https://api.example.com/protected");
```

### Timeout Configuration

**HttpClient:**
```csharp
using var client = new HttpClient();
client.Timeout = TimeSpan.FromSeconds(30);

try
{
    var response = await client.GetAsync("https://slow-api.example.com");
}
catch (TaskCanceledException)
{
    Console.WriteLine("Request timed out");
}
```

**CurlDotNet:**
```csharp
var curl = new Curl
{
    Timeout = TimeSpan.FromSeconds(30)
};

var result = await curl.GetAsync("https://slow-api.example.com");
if (!result.IsSuccess && result.Error.Contains("timeout"))
{
    Console.WriteLine("Request timed out");
}
```

## Advanced Scenarios

### Custom Request with Multiple Headers

**HttpClient:**
```csharp
using var client = new HttpClient();
using var request = new HttpRequestMessage(HttpMethod.Put, "https://api.example.com/resource");
request.Headers.Add("Authorization", "Bearer token");
request.Headers.Add("X-Request-ID", Guid.NewGuid().ToString());
request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

var response = await client.SendAsync(request);
```

**CurlDotNet:**
```csharp
var curl = new Curl();
var result = await curl.SendAsync(new CurlRequest
{
    Url = "https://api.example.com/resource",
    Method = HttpMethod.Put,
    Headers = new Dictionary<string, string>
    {
        ["Authorization"] = "Bearer token",
        ["X-Request-ID"] = Guid.NewGuid().ToString()
    },
    Content = jsonData
});
```

### Form Data Submission

**HttpClient:**
```csharp
using var client = new HttpClient();
var formData = new FormUrlEncodedContent(new[]
{
    new KeyValuePair<string, string>("username", "user@example.com"),
    new KeyValuePair<string, string>("password", "secure123")
});

var response = await client.PostAsync("https://example.com/login", formData);
```

**CurlDotNet:**
```csharp
var curl = new Curl();
var formData = new Dictionary<string, string>
{
    ["username"] = "user@example.com",
    ["password"] = "secure123"
};

var result = await curl.PostFormAsync("https://example.com/login", formData);
```

### File Upload

**HttpClient:**
```csharp
using var client = new HttpClient();
using var content = new MultipartFormDataContent();
using var fileStream = File.OpenRead("document.pdf");
content.Add(new StreamContent(fileStream), "file", "document.pdf");

var response = await client.PostAsync("https://api.example.com/upload", content);
```

**CurlDotNet:**
```csharp
var curl = new Curl();
var result = await curl.UploadFileAsync(
    "https://api.example.com/upload",
    "document.pdf",
    "file"
);
```

## Error Handling Comparison

### HttpClient Error Handling
```csharp
try
{
    var response = await client.GetAsync(url);

    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();
        // Process content
    }
    else
    {
        // Handle HTTP error status codes
        switch (response.StatusCode)
        {
            case HttpStatusCode.NotFound:
                Console.WriteLine("Resource not found");
                break;
            case HttpStatusCode.Unauthorized:
                Console.WriteLine("Authentication required");
                break;
            default:
                Console.WriteLine($"Error: {response.StatusCode}");
                break;
        }
    }
}
catch (HttpRequestException ex)
{
    // Network errors
    Console.WriteLine($"Request failed: {ex.Message}");
}
catch (TaskCanceledException)
{
    // Timeout
    Console.WriteLine("Request timed out");
}
catch (Exception ex)
{
    // Other errors
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

### CurlDotNet Error Handling
```csharp
var result = await curl.GetAsync(url);

if (result.IsSuccess)
{
    // Process result.Data
}
else
{
    // All errors in one place
    Console.WriteLine($"Error: {result.Error}");
    Console.WriteLine($"Status: {result.StatusCode}");

    // Specific error handling if needed
    if (result.StatusCode == HttpStatusCode.NotFound)
    {
        Console.WriteLine("Resource not found");
    }
}
```

## Dependency Injection

### HttpClient with DI
```csharp
// In Startup.cs or Program.cs
services.AddHttpClient<MyService>(client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.DefaultRequestHeaders.Add("User-Agent", "MyApp");
});

// In service
public class MyService
{
    private readonly HttpClient _client;

    public MyService(HttpClient client)
    {
        _client = client;
    }
}
```

### CurlDotNet with DI
```csharp
// In Startup.cs or Program.cs
services.AddSingleton<ICurl>(sp => new Curl
{
    BaseUrl = "https://api.example.com",
    Headers = { ["User-Agent"] = "MyApp" }
});

// In service
public class MyService
{
    private readonly ICurl _curl;

    public MyService(ICurl curl)
    {
        _curl = curl;
    }
}
```

## Testing

### Mocking HttpClient
```csharp
// Complex setup with HttpMessageHandler
var mockHandler = new Mock<HttpMessageHandler>();
mockHandler.Protected()
    .Setup<Task<HttpResponseMessage>>("SendAsync",
        ItExpr.IsAny<HttpRequestMessage>(),
        ItExpr.IsAny<CancellationToken>())
    .ReturnsAsync(new HttpResponseMessage
    {
        StatusCode = HttpStatusCode.OK,
        Content = new StringContent("test data")
    });

var client = new HttpClient(mockHandler.Object);
```

### Mocking CurlDotNet
```csharp
// Simple interface mocking
var mockCurl = new Mock<ICurl>();
mockCurl.Setup(x => x.GetAsync(It.IsAny<string>()))
    .ReturnsAsync(new CurlResult
    {
        IsSuccess = true,
        Data = "test data",
        StatusCode = HttpStatusCode.OK
    });
```

## Migration Checklist

- [ ] Replace `HttpClient` instances with `Curl`
- [ ] Convert `HttpRequestMessage` to `CurlRequest` or direct method calls
- [ ] Update error handling from exceptions to result pattern
- [ ] Simplify content reading (no more `ReadAsStringAsync`)
- [ ] Update header management
- [ ] Adjust timeout configuration
- [ ] Update dependency injection registration
- [ ] Simplify unit tests
- [ ] Remove unnecessary using statements and disposal patterns

## Benefits After Migration

1. **Simpler Code**: Less boilerplate, more readable
2. **Better Errors**: Clear error messages without exception handling
3. **Consistent Behavior**: Same results on all platforms
4. **Easier Testing**: Simple interface mocking
5. **curl Compatibility**: Use curl knowledge and documentation

## Common Pitfalls

1. **Don't dispose Curl**: Unlike HttpClient, Curl doesn't need disposal
2. **Result pattern**: Remember to check `IsSuccess` instead of catching exceptions
3. **Direct data access**: No need for `ReadAsStringAsync()`, just use `result.Data`
4. **Header persistence**: Headers added to Curl instance persist across requests

## Need Help?

- Check the [examples](https://github.com/jacob-mellor/curl-dot-net/tree/master/examples) for working code
- Review the [API documentation](../api/index.md) for detailed method signatures
- Ask questions in [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)