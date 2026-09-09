# Migrating from RestSharp to CurlDotNet

RestSharp has been a popular REST client for .NET, but CurlDotNet offers a simpler API, better performance, and no native dependencies. This guide helps you migrate your RestSharp code to CurlDotNet.

## Key Differences

| Feature | RestSharp | CurlDotNet |
|---------|-----------|------------|
| Dependencies | Multiple packages | Single, lightweight package |
| Native Code | Platform-specific | Pure C#, no P/Invoke |
| API Style | Builder pattern | Direct method calls |
| Serialization | Built-in or custom | Flexible, bring your own |
| curl Compatibility | None | 100% compatible |

## Basic Migration Examples

### Simple GET Request

**RestSharp:**
```csharp
var client = new RestClient("https://api.example.com");
var request = new RestRequest("users", Method.GET);
var response = await client.ExecuteAsync(request);

if (response.IsSuccessful)
{
    Console.WriteLine(response.Content);
}
else
{
    Console.WriteLine($"Error: {response.ErrorMessage}");
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

**RestSharp:**
```csharp
var client = new RestClient("https://api.example.com");
var request = new RestRequest("users", Method.POST);
request.AddJsonBody(new { name = "John", age = 30 });

var response = await client.ExecuteAsync(request);
```

**CurlDotNet:**
```csharp
var curl = new Curl();
var result = await curl.PostJsonAsync(
    "https://api.example.com/users",
    new { name = "John", age = 30 }
);
```

### Request with Headers

**RestSharp:**
```csharp
var client = new RestClient("https://api.example.com");
var request = new RestRequest("protected", Method.GET);
request.AddHeader("Authorization", "Bearer token123");
request.AddHeader("X-Custom-Header", "value");

var response = await client.ExecuteAsync(request);
```

**CurlDotNet:**
```csharp
var curl = new Curl();
curl.Headers.Add("Authorization", "Bearer token123");
curl.Headers.Add("X-Custom-Header", "value");

var result = await curl.GetAsync("https://api.example.com/protected");
```

### Query Parameters

**RestSharp:**
```csharp
var client = new RestClient("https://api.example.com");
var request = new RestRequest("search", Method.GET);
request.AddQueryParameter("q", "search term");
request.AddQueryParameter("page", "1");
request.AddQueryParameter("limit", "10");

var response = await client.ExecuteAsync(request);
```

**CurlDotNet:**
```csharp
var curl = new Curl();
var result = await curl.GetAsync(
    "https://api.example.com/search?q=search+term&page=1&limit=10"
);

// Or using a helper method
var url = curl.BuildUrl("https://api.example.com/search", new Dictionary<string, string>
{
    ["q"] = "search term",
    ["page"] = "1",
    ["limit"] = "10"
});
var result = await curl.GetAsync(url);
```

## Advanced Scenarios

### Typed Responses

**RestSharp:**
```csharp
public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
}

var client = new RestClient("https://api.example.com");
var request = new RestRequest("users/123", Method.GET);
var response = await client.ExecuteAsync<User>(request);

if (response.IsSuccessful)
{
    User user = response.Data;
    Console.WriteLine($"Name: {user.Name}");
}
```

**CurlDotNet:**
```csharp
public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
}

var curl = new Curl();
var result = await curl.GetAsync("https://api.example.com/users/123");

if (result.IsSuccess)
{
    var user = JsonSerializer.Deserialize<User>(result.Data);
    Console.WriteLine($"Name: {user.Name}");
}

// Or with extension method
var user = await curl.GetJsonAsync<User>("https://api.example.com/users/123");
```

### File Upload

**RestSharp:**
```csharp
var client = new RestClient("https://api.example.com");
var request = new RestRequest("upload", Method.POST);
request.AddFile("file", "/path/to/file.pdf");
request.AddParameter("description", "Important document");

var response = await client.ExecuteAsync(request);
```

**CurlDotNet:**
```csharp
var curl = new Curl();
var result = await curl.UploadFileAsync(
    "https://api.example.com/upload",
    "/path/to/file.pdf",
    "file",
    new Dictionary<string, string>
    {
        ["description"] = "Important document"
    }
);
```

### Authentication

**RestSharp Basic Auth:**
```csharp
var client = new RestClient("https://api.example.com");
client.Authenticator = new HttpBasicAuthenticator("username", "password");
var request = new RestRequest("secure", Method.GET);
var response = await client.ExecuteAsync(request);
```

**CurlDotNet Basic Auth:**
```csharp
var curl = new Curl();
curl.SetBasicAuth("username", "password");
var result = await curl.GetAsync("https://api.example.com/secure");
```

**RestSharp OAuth:**
```csharp
var client = new RestClient("https://api.example.com");
client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(
    accessToken, "Bearer"
);
```

**CurlDotNet OAuth:**
```csharp
var curl = new Curl();
curl.Headers.Add("Authorization", $"Bearer {accessToken}");
```

### Request Interceptors

**RestSharp:**
```csharp
var client = new RestClient("https://api.example.com");
client.AddDefaultHeader("X-App-Version", "1.0.0");

// Interceptor pattern
client.ConfigureWebRequest(request =>
{
    request.UserAgent = "MyApp/1.0";
    request.KeepAlive = true;
});
```

**CurlDotNet:**
```csharp
var curl = new Curl
{
    Headers =
    {
        ["X-App-Version"] = "1.0.0",
        ["User-Agent"] = "MyApp/1.0"
    }
};

// Or use middleware pattern
curl.BeforeRequest = request =>
{
    request.Headers["X-Request-ID"] = Guid.NewGuid().ToString();
    return request;
};
```

### Timeout and Retry

**RestSharp:**
```csharp
var client = new RestClient("https://api.example.com")
{
    Timeout = 30000,  // 30 seconds
    MaxRedirects = 5
};

// Manual retry logic
var maxRetries = 3;
for (int i = 0; i < maxRetries; i++)
{
    var response = await client.ExecuteAsync(request);
    if (response.IsSuccessful) break;
    await Task.Delay(1000 * (i + 1));  // Exponential backoff
}
```

**CurlDotNet:**
```csharp
var curl = new Curl
{
    Timeout = TimeSpan.FromSeconds(30),
    MaxRedirects = 5,
    RetryPolicy = new RetryPolicy
    {
        MaxRetries = 3,
        DelayStrategy = DelayStrategy.ExponentialBackoff
    }
};

var result = await curl.GetAsync("https://api.example.com/data");
```

## Serialization

### RestSharp Custom Serializers
```csharp
public class NewtonsoftJsonSerializer : IRestSerializer
{
    public string Serialize(object obj) =>
        JsonConvert.SerializeObject(obj);

    public T Deserialize<T>(RestResponse response) =>
        JsonConvert.DeserializeObject<T>(response.Content);
}

var client = new RestClient("https://api.example.com");
client.UseSerializer(() => new NewtonsoftJsonSerializer());
```

### CurlDotNet Serialization
```csharp
// Use any serializer you prefer
var curl = new Curl();

// With System.Text.Json (default)
var result = await curl.PostJsonAsync(url, data);
var obj = JsonSerializer.Deserialize<MyType>(result.Data);

// With Newtonsoft.Json
var json = JsonConvert.SerializeObject(data);
var result = await curl.PostAsync(url, json, "application/json");
var obj = JsonConvert.DeserializeObject<MyType>(result.Data);

// Custom extension method
public static class CurlExtensions
{
    public static async Task<T> GetJsonAsync<T>(this Curl curl, string url)
    {
        var result = await curl.GetAsync(url);
        return result.IsSuccess
            ? JsonConvert.DeserializeObject<T>(result.Data)
            : default;
    }
}
```

## Error Handling

### RestSharp Error Handling
```csharp
var response = await client.ExecuteAsync(request);

if (response.IsSuccessful)
{
    // Process response.Content
}
else if (response.ErrorException != null)
{
    // Network or deserialization error
    Console.WriteLine($"Error: {response.ErrorMessage}");
    Console.WriteLine($"Exception: {response.ErrorException}");
}
else
{
    // HTTP error
    Console.WriteLine($"HTTP {response.StatusCode}: {response.StatusDescription}");
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
    // All error information in one place
    Console.WriteLine($"Error: {result.Error}");
    Console.WriteLine($"HTTP {result.StatusCode}");

    // Specific handling if needed
    if (result.StatusCode == HttpStatusCode.TooManyRequests)
    {
        // Handle rate limiting
    }
}
```

## Testing

### Mocking RestSharp
```csharp
// Complex mocking with RestSharp
var mockClient = new Mock<IRestClient>();
var mockResponse = new RestResponse
{
    StatusCode = HttpStatusCode.OK,
    Content = "test data",
    IsSuccessful = true
};

mockClient
    .Setup(x => x.ExecuteAsync(It.IsAny<RestRequest>(), default))
    .ReturnsAsync(mockResponse);
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

- [ ] Replace `RestClient` with `Curl`
- [ ] Convert `RestRequest` to direct method calls
- [ ] Update parameter addition to use dictionaries or URL building
- [ ] Migrate custom serializers to extension methods
- [ ] Update authentication patterns
- [ ] Simplify error handling
- [ ] Update retry logic to use built-in policies
- [ ] Refactor interceptors to middleware pattern
- [ ] Update unit tests with simpler mocks

## Benefits After Migration

1. **No Native Dependencies**: Pure C# means no platform-specific issues
2. **Simpler API**: Less ceremony, more intuitive method calls
3. **Better Performance**: Optimized for modern .NET
4. **curl Compatibility**: Leverage curl documentation and knowledge
5. **Lighter Package**: Smaller footprint, faster CI/CD

## Common Patterns

### RestSharp Fluent API to CurlDotNet
```csharp
// RestSharp fluent style
var response = await new RestClient("https://api.example.com")
    .AddDefaultHeader("X-API-Key", "key123")
    .ExecuteAsync(new RestRequest("data")
        .AddQueryParameter("filter", "active")
        .AddHeader("Accept", "application/json"));

// CurlDotNet direct style
var curl = new Curl
{
    Headers =
    {
        ["X-API-Key"] = "key123",
        ["Accept"] = "application/json"
    }
};
var result = await curl.GetAsync("https://api.example.com/data?filter=active");
```

### Response Processing
```csharp
// RestSharp
var users = response.IsSuccessful
    ? JsonConvert.DeserializeObject<List<User>>(response.Content)
    : new List<User>();

// CurlDotNet
var users = result.IsSuccess
    ? JsonSerializer.Deserialize<List<User>>(result.Data)
    : new List<User>();
```

## Need Help?

- Review [working examples](https://github.com/jacob-mellor/curl-dot-net/tree/master/examples)
- Check the [cookbook](../cookbook/index.html) for common recipes
- Read the [API documentation](../api/index.html) for detailed method information
- Ask questions in [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)