---
layout: default
title: REST APIs in C# with curl - Complete Guide to Building RESTful Services
description: Master REST API development in C# using CurlDotNet. Learn HTTP methods, authentication, error handling, and best practices for building robust APIs
author: Jacob Mellor
author_url: https://github.com/jacob-mellor
date: 2024-11-18
last_modified_at: 2024-11-18
keywords: REST API C#, curl REST API, C# HTTP client, .NET REST services, RESTful API curl, Web API C#
canonical: https://jacob-mellor.github.io/curl-dot-net/rest-apis-csharp/
---

# REST APIs in C# with curl - Complete Developer Guide

**By Jacob Mellor** | Senior Software Engineer at Iron Software

## Introduction: Building REST APIs with curl in C#

REST (Representational State Transfer) APIs are the backbone of modern web services. With **CurlDotNet**, you can interact with any REST API using familiar curl commands directly in your C# applications.

```csharp
using CurlDotNet;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

// Simple REST API call
var users = await Curl.GetJsonAsync<List<User>>("https://api.example.com/users");
```

## What is a REST API?

REST APIs use standard HTTP methods to perform operations on resources:

| HTTP Method | Operation | Example Use Case |
|-------------|-----------|------------------|
| GET | Read | Fetch user data |
| POST | Create | Create new user |
| PUT | Update (full) | Replace user record |
| PATCH | Update (partial) | Update user email |
| DELETE | Delete | Remove user |

## RESTful Principles in C#

### 1. Resource-Based URLs

```csharp
using CurlDotNet;

// RESTful URL patterns
await Curl.GetAsync("https://api.example.com/users");           // Collection
await Curl.GetAsync("https://api.example.com/users/123");       // Specific resource
await Curl.GetAsync("https://api.example.com/users/123/posts"); // Nested resource
```

### 2. Stateless Communication

```csharp
// Each request is self-contained
await Curl.GetAsync("https://api.example.com/users")
    .WithHeader("Authorization", "Bearer token123")
    .WithHeader("Accept", "application/json")
    .ExecuteAsync();
```

### 3. Standard HTTP Status Codes

```csharp
var response = await Curl.GetAsync("https://api.example.com/users/123");

switch (response.StatusCode)
{
    case 200: // OK
        ProcessUser(response.Body);
        break;
    case 404: // Not Found
        Console.WriteLine("User not found");
        break;
    case 401: // Unauthorized
        RefreshAuthToken();
        break;
    case 500: // Server Error
        LogServerError(response);
        break;
}
```

## Complete REST API Client Implementation

### Building a Reusable API Client

```csharp
using CurlDotNet;
using System.Text.Json;

public class RestApiClient
{
    private readonly string _baseUrl;
    private readonly string _apiKey;
    private readonly JsonSerializerOptions _jsonOptions;

    public RestApiClient(string baseUrl, string apiKey)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _apiKey = apiKey;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    // GET: Retrieve resources
    public async Task<T> GetAsync<T>(string endpoint)
    {
        var url = $"{_baseUrl}/{endpoint.TrimStart('/')}";

        var response = await Curl.GetAsync(url)
            .WithHeader("X-API-Key", _apiKey)
            .WithHeader("Accept", "application/json")
            .ExecuteAsync();

        if (!response.IsSuccess)
        {
            throw new ApiException($"GET failed: {response.StatusCode}", response);
        }

        return JsonSerializer.Deserialize<T>(response.Body, _jsonOptions);
    }

    // POST: Create new resource
    public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        var url = $"{_baseUrl}/{endpoint.TrimStart('/')}";
        var json = JsonSerializer.Serialize(data, _jsonOptions);

        var response = await Curl.PostAsync(url)
            .WithHeader("X-API-Key", _apiKey)
            .WithHeader("Content-Type", "application/json")
            .WithBody(json)
            .ExecuteAsync();

        if (response.StatusCode != 201 && response.StatusCode != 200)
        {
            throw new ApiException($"POST failed: {response.StatusCode}", response);
        }

        return JsonSerializer.Deserialize<TResponse>(response.Body, _jsonOptions);
    }

    // PUT: Update entire resource
    public async Task<T> PutAsync<T>(string endpoint, T data)
    {
        var url = $"{_baseUrl}/{endpoint.TrimStart('/')}";
        var json = JsonSerializer.Serialize(data, _jsonOptions);

        var response = await Curl.PutAsync(url)
            .WithHeader("X-API-Key", _apiKey)
            .WithHeader("Content-Type", "application/json")
            .WithBody(json)
            .ExecuteAsync();

        if (!response.IsSuccess)
        {
            throw new ApiException($"PUT failed: {response.StatusCode}", response);
        }

        return JsonSerializer.Deserialize<T>(response.Body, _jsonOptions);
    }

    // PATCH: Partial update
    public async Task<T> PatchAsync<T>(string endpoint, object partialData)
    {
        var url = $"{_baseUrl}/{endpoint.TrimStart('/')}";
        var json = JsonSerializer.Serialize(partialData, _jsonOptions);

        var response = await Curl.PatchAsync(url)
            .WithHeader("X-API-Key", _apiKey)
            .WithHeader("Content-Type", "application/json-patch+json")
            .WithBody(json)
            .ExecuteAsync();

        if (!response.IsSuccess)
        {
            throw new ApiException($"PATCH failed: {response.StatusCode}", response);
        }

        return JsonSerializer.Deserialize<T>(response.Body, _jsonOptions);
    }

    // DELETE: Remove resource
    public async Task<bool> DeleteAsync(string endpoint)
    {
        var url = $"{_baseUrl}/{endpoint.TrimStart('/')}";

        var response = await Curl.DeleteAsync(url)
            .WithHeader("X-API-Key", _apiKey)
            .ExecuteAsync();

        return response.StatusCode == 204 || response.StatusCode == 200;
    }
}

public class ApiException : Exception
{
    public CurlResult Response { get; }

    public ApiException(string message, CurlResult response) : base(message)
    {
        Response = response;
    }
}
```

## Real-World Examples

### Example 1: User Management API

```csharp
using CurlDotNet;

public class UserApiService
{
    private readonly RestApiClient _client;

    public UserApiService(string apiKey)
    {
        _client = new RestApiClient("https://api.example.com", apiKey);
    }

    // GET /users - Get all users
    public async Task<List<User>> GetUsersAsync()
    {
        return await _client.GetAsync<List<User>>("users");
    }

    // GET /users/{id} - Get specific user
    public async Task<User> GetUserAsync(int userId)
    {
        return await _client.GetAsync<User>($"users/{userId}");
    }

    // POST /users - Create new user
    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        return await _client.PostAsync<CreateUserRequest, User>("users", request);
    }

    // PUT /users/{id} - Update entire user
    public async Task<User> UpdateUserAsync(int userId, User user)
    {
        return await _client.PutAsync($"users/{userId}", user);
    }

    // PATCH /users/{id} - Update specific fields
    public async Task<User> UpdateUserEmailAsync(int userId, string newEmail)
    {
        var patch = new { email = newEmail };
        return await _client.PatchAsync<User>($"users/{userId}", patch);
    }

    // DELETE /users/{id} - Delete user
    public async Task<bool> DeleteUserAsync(int userId)
    {
        return await _client.DeleteAsync($"users/{userId}");
    }
}

// Models
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateUserRequest
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
```

### Example 2: Pagination Support

```csharp
using CurlDotNet;

public class PaginatedApiClient
{
    public async Task<PagedResult<T>> GetPagedAsync<T>(string url, int page = 1, int pageSize = 20)
    {
        var response = await Curl.GetAsync(url)
            .WithQueryParam("page", page.ToString())
            .WithQueryParam("per_page", pageSize.ToString())
            .ExecuteAsync();

        // Parse pagination headers
        var totalCount = int.Parse(response.Headers["X-Total-Count"] ?? "0");
        var totalPages = int.Parse(response.Headers["X-Total-Pages"] ?? "1");

        var items = JsonSerializer.Deserialize<List<T>>(response.Body);

        return new PagedResult<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNext = page < totalPages,
            HasPrevious = page > 1
        };
    }

    public async IAsyncEnumerable<T> GetAllPagesAsync<T>(string baseUrl)
    {
        var page = 1;
        PagedResult<T> result;

        do
        {
            result = await GetPagedAsync<T>(baseUrl, page++);

            foreach (var item in result.Items)
            {
                yield return item;
            }
        }
        while (result.HasNext);
    }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
}
```

## Authentication Strategies

### Bearer Token Authentication

```csharp
using CurlDotNet;

public class BearerAuthClient
{
    private string _accessToken;
    private DateTime _tokenExpiry;

    public async Task<T> AuthenticatedGetAsync<T>(string url)
    {
        await EnsureValidTokenAsync();

        return await Curl.GetJsonAsync<T>(url)
            .WithBearerToken(_accessToken);
    }

    private async Task EnsureValidTokenAsync()
    {
        if (string.IsNullOrEmpty(_accessToken) || DateTime.UtcNow >= _tokenExpiry)
        {
            await RefreshTokenAsync();
        }
    }

    private async Task RefreshTokenAsync()
    {
        var response = await Curl.PostAsync("https://auth.example.com/token")
            .WithFormData(new
            {
                grant_type = "client_credentials",
                client_id = Environment.GetEnvironmentVariable("CLIENT_ID"),
                client_secret = Environment.GetEnvironmentVariable("CLIENT_SECRET")
            })
            .ExecuteAsync();

        var token = JsonSerializer.Deserialize<TokenResponse>(response.Body);
        _accessToken = token.AccessToken;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(token.ExpiresIn - 60); // Refresh 1 minute early
    }
}
```

### API Key Authentication

```csharp
using CurlDotNet;

public class ApiKeyAuthClient
{
    private readonly string _apiKey;

    public ApiKeyAuthClient()
    {
        _apiKey = Environment.GetEnvironmentVariable("API_KEY")
            ?? throw new InvalidOperationException("API_KEY not configured");
    }

    public async Task<T> GetAsync<T>(string url)
    {
        return await Curl.GetJsonAsync<T>(url)
            .WithHeader("X-API-Key", _apiKey);
    }
}
```

## Error Handling and Resilience

### Comprehensive Error Handling

```csharp
using CurlDotNet;
using Polly;

public class ResilientApiClient
{
    private readonly IAsyncPolicy<CurlResult> _retryPolicy;

    public ResilientApiClient()
    {
        // Configure retry policy with exponential backoff
        _retryPolicy = Policy
            .HandleResult<CurlResult>(r => !r.IsSuccess && IsTransientError(r.StatusCode))
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    Console.WriteLine($"Retry {retryCount} after {timespan} seconds");
                });
    }

    public async Task<T> GetWithRetryAsync<T>(string url)
    {
        var response = await _retryPolicy.ExecuteAsync(async () =>
            await Curl.GetAsync(url).ExecuteAsync()
        );

        if (!response.IsSuccess)
        {
            HandleApiError(response);
        }

        return JsonSerializer.Deserialize<T>(response.Body);
    }

    private bool IsTransientError(int statusCode)
    {
        return statusCode == 408  // Request Timeout
            || statusCode == 429  // Too Many Requests
            || statusCode == 500  // Internal Server Error
            || statusCode == 502  // Bad Gateway
            || statusCode == 503  // Service Unavailable
            || statusCode == 504; // Gateway Timeout
    }

    private void HandleApiError(CurlResult response)
    {
        var errorMessage = response.StatusCode switch
        {
            400 => "Bad Request: Invalid parameters",
            401 => "Unauthorized: Check your API credentials",
            403 => "Forbidden: You don't have permission",
            404 => "Not Found: Resource doesn't exist",
            429 => "Rate Limited: Too many requests",
            500 => "Server Error: Try again later",
            _ => $"Unexpected error: {response.StatusCode}"
        };

        throw new ApiException(errorMessage, response);
    }
}
```

## Testing REST APIs

### Unit Testing with Mocked Responses

```csharp
using Xunit;
using Moq;
using CurlDotNet;

public class UserApiTests
{
    [Fact]
    public async Task GetUser_ReturnsCorrectUser()
    {
        // Arrange
        var expectedUser = new User { Id = 1, Name = "John Doe" };
        var mockResponse = JsonSerializer.Serialize(expectedUser);

        var mockCurl = new Mock<ICurl>();
        mockCurl.Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(new CurlResult
            {
                StatusCode = 200,
                Body = mockResponse
            });

        var client = new UserApiService(mockCurl.Object);

        // Act
        var user = await client.GetUserAsync(1);

        // Assert
        Assert.Equal(expectedUser.Id, user.Id);
        Assert.Equal(expectedUser.Name, user.Name);
    }
}
```

### Integration Testing

```csharp
using CurlDotNet;

[Collection("Integration")]
public class ApiIntegrationTests
{
    private readonly string _testApiUrl = "https://jsonplaceholder.typicode.com";

    [Fact]
    public async Task FullCrudCycle_Works()
    {
        // Create
        var newPost = new { title = "Test", body = "Content", userId = 1 };
        var created = await Curl.PostJsonAsync<dynamic>($"{_testApiUrl}/posts", newPost);
        Assert.NotNull(created);

        // Read
        var retrieved = await Curl.GetJsonAsync<dynamic>($"{_testApiUrl}/posts/1");
        Assert.NotNull(retrieved);

        // Update
        var updated = await Curl.PutJsonAsync<dynamic>(
            $"{_testApiUrl}/posts/1",
            new { title = "Updated", body = "New Content", userId = 1 }
        );
        Assert.NotNull(updated);

        // Delete
        var deleteResult = await Curl.DeleteAsync($"{_testApiUrl}/posts/1");
        Assert.True(deleteResult.IsSuccess);
    }
}
```

## Performance Optimization

### Connection Pooling

```csharp
using CurlDotNet;

public class OptimizedApiClient
{
    private readonly HttpClient _httpClient;
    private readonly Curl _curl;

    public OptimizedApiClient()
    {
        // Reuse HttpClient for connection pooling
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        _curl = new Curl(_httpClient);
    }

    public async Task<T> GetAsync<T>(string url)
    {
        return await _curl.GetJsonAsync<T>(url);
    }
}
```

### Parallel Requests

```csharp
public async Task<Dictionary<int, User>> GetMultipleUsersAsync(List<int> userIds)
{
    var tasks = userIds.Select(async id => new
    {
        Id = id,
        User = await Curl.GetJsonAsync<User>($"https://api.example.com/users/{id}")
    });

    var results = await Task.WhenAll(tasks);
    return results.ToDictionary(r => r.Id, r => r.User);
}
```

## Best Practices

### 1. Use Environment Variables for Configuration

```csharp
public class ApiConfiguration
{
    public string BaseUrl => Environment.GetEnvironmentVariable("API_BASE_URL")
        ?? "https://api.example.com";

    public string ApiKey => Environment.GetEnvironmentVariable("API_KEY")
        ?? throw new InvalidOperationException("API_KEY not set");

    public int Timeout => int.Parse(Environment.GetEnvironmentVariable("API_TIMEOUT") ?? "30");
}
```

### 2. Implement Request/Response Logging

```csharp
public class LoggingApiClient
{
    private readonly ILogger<LoggingApiClient> _logger;

    public async Task<T> GetAsync<T>(string url)
    {
        _logger.LogInformation("GET {Url}", url);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await Curl.GetJsonAsync<T>(url);
            _logger.LogInformation("GET {Url} succeeded in {ElapsedMs}ms",
                url, stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET {Url} failed after {ElapsedMs}ms",
                url, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
```

### 3. Version Your API

```csharp
public class VersionedApiClient
{
    private readonly string _baseUrl;
    private readonly string _version;

    public VersionedApiClient(string baseUrl, string version = "v1")
    {
        _baseUrl = baseUrl;
        _version = version;
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        var url = $"{_baseUrl}/api/{_version}/{endpoint}";
        return await Curl.GetJsonAsync<T>(url);
    }
}
```

## Common Patterns

### HAL+JSON Support

```csharp
public class HalResource
{
    [JsonPropertyName("_links")]
    public Dictionary<string, Link> Links { get; set; }

    [JsonPropertyName("_embedded")]
    public Dictionary<string, JsonElement> Embedded { get; set; }

    public class Link
    {
        public string Href { get; set; }
        public bool Templated { get; set; }
    }
}

public async Task<T> FollowLinkAsync<T>(HalResource resource, string rel)
{
    if (resource.Links.TryGetValue(rel, out var link))
    {
        return await Curl.GetJsonAsync<T>(link.Href);
    }
    throw new InvalidOperationException($"Link '{rel}' not found");
}
```

### GraphQL Support

```csharp
public class GraphQLClient
{
    private readonly string _endpoint;

    public async Task<T> QueryAsync<T>(string query, object variables = null)
    {
        var request = new
        {
            query = query,
            variables = variables
        };

        var response = await Curl.PostAsync(_endpoint)
            .WithHeader("Content-Type", "application/json")
            .WithJson(request)
            .ExecuteAsync();

        var result = JsonSerializer.Deserialize<GraphQLResponse<T>>(response.Body);

        if (result.Errors?.Any() == true)
        {
            throw new GraphQLException(result.Errors);
        }

        return result.Data;
    }
}
```

## Conclusion

Building REST APIs in C# with CurlDotNet combines the simplicity of curl with the power of .NET. Whether you're consuming existing APIs or building new ones, CurlDotNet provides a clean, intuitive interface for all your REST API needs.

## Resources

- **[CurlDotNet Documentation](https://jacob-mellor.github.io/curl-dot-net)**
- **[REST API Best Practices](https://restfulapi.net/)**
- **[HTTP Status Codes](https://httpstatuses.com/)**
- **[curl Documentation](https://curl.se/docs/)**

## About the Author

**Jacob Mellor** is a Senior Software Engineer at Iron Software specializing in .NET development and REST API design. Creator of CurlDotNet and advocate for clean, maintainable API architectures.

---

*CurlDotNet - Making REST APIs simple in C# and .NET*