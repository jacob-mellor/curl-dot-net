---
layout: default
title: Building a Reusable API Client Class with curl for C# and .NET
description: Learn how to create a professional, reusable API client wrapper using CurlDotNet for C# applications
keywords: C# API client, curl REST client, .NET HTTP wrapper, reusable API class, CurlDotNet patterns
---

# Building a Reusable API Client Class

## Create Professional API Clients with curl for C# and .NET

Learn how to build a robust, reusable API client class using CurlDotNet that handles authentication, errors, retries, and more.

## Prerequisites

- Basic C# knowledge
- Understanding of REST APIs
- CurlDotNet package installed

```bash
dotnet add package CurlDotNet
```

## The Complete API Client Pattern

### Basic Structure

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;
using System.Text.Json;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class ApiClient : IDisposable
{
    private readonly Curl _curl;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    public ApiClient(string baseUrl, string apiKey = null)
    {
        _baseUrl = baseUrl.TrimEnd('/');
        _curl = new Curl(_baseUrl)
            .WithTimeout(30)
            .WithHeader("Accept", "application/json")
            .WithHeader("User-Agent", "MyApp/1.0 (CurlDotNet)");

        if (!string.IsNullOrEmpty(apiKey))
        {
            _curl.WithHeader("Authorization", $"Bearer {apiKey}");
        }

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<T> GetAsync<T>(string endpoint,
        Dictionary<string, string> queryParams = null)
    {
        var url = BuildUrl(endpoint, queryParams);
        var response = await _curl.GetAsync(url);
        return DeserializeResponse<T>(response);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        string endpoint, TRequest data)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        var response = await _curl.PostAsync(BuildUrl(endpoint))
            .WithHeader("Content-Type", "application/json")
            .WithBody(json)
            .ExecuteAsync();
        return DeserializeResponse<TResponse>(response);
    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(
        string endpoint, TRequest data)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        var response = await _curl.PutAsync(BuildUrl(endpoint))
            .WithHeader("Content-Type", "application/json")
            .WithBody(json)
            .ExecuteAsync();
        return DeserializeResponse<TResponse>(response);
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        var response = await _curl.DeleteAsync(BuildUrl(endpoint));
        return response.StatusCode >= 200 && response.StatusCode < 300;
    }

    private string BuildUrl(string endpoint,
        Dictionary<string, string> queryParams = null)
    {
        var url = $"{_baseUrl}/{endpoint.TrimStart('/')}";

        if (queryParams?.Any() == true)
        {
            var query = string.Join("&",
                queryParams.Select(kvp =>
                    $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            url = $"{url}?{query}";
        }

        return url;
    }

    private T DeserializeResponse<T>(CurlResult response)
    {
        if (response.StatusCode >= 400)
        {
            throw new ApiException($"API error: {response.StatusCode}",
                response.StatusCode, response.Body);
        }

        return JsonSerializer.Deserialize<T>(response.Body, _jsonOptions);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _curl?.Dispose();
            _disposed = true;
        }
    }
}

public class ApiException : Exception
{
    public int StatusCode { get; }
    public string ResponseBody { get; }

    public ApiException(string message, int statusCode, string responseBody)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}
```

## Advanced Features Implementation

### Adding Retry Logic with Exponential Backoff

```csharp
using CurlDotNet;
using Polly;
using Polly.Extensions.Http;
// Install: dotnet add package CurlDotNet
// Install: dotnet add package Microsoft.Extensions.Http.Polly
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class ResilientApiClient : ApiClient
{
    private readonly IAsyncPolicy<CurlResult> _retryPolicy;

    public ResilientApiClient(string baseUrl, string apiKey = null)
        : base(baseUrl, apiKey)
    {
        _retryPolicy = Policy
            .HandleResult<CurlResult>(r =>
                r.StatusCode >= 500 || r.StatusCode == 429)
            .OrResult(r => r.StatusCode == 0) // Network errors
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    var result = outcome.Result;
                    Console.WriteLine($"Retry {retryCount} after {timespan}s, " +
                        $"StatusCode: {result?.StatusCode}");
                });
    }

    protected override async Task<CurlResult> ExecuteWithRetryAsync(
        Func<Task<CurlResult>> operation)
    {
        return await _retryPolicy.ExecuteAsync(operation);
    }
}
```

### Adding Request/Response Logging

```csharp
using CurlDotNet;
using Microsoft.Extensions.Logging;
// Install: dotnet add package CurlDotNet
// Install: dotnet add package Microsoft.Extensions.Logging
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class LoggingApiClient : ApiClient
{
    private readonly ILogger<LoggingApiClient> _logger;

    public LoggingApiClient(string baseUrl, ILogger<LoggingApiClient> logger,
        string apiKey = null) : base(baseUrl, apiKey)
    {
        _logger = logger;
    }

    protected override async Task<CurlResult> ExecuteRequestAsync(
        string method, string url, object body = null)
    {
        var requestId = Guid.NewGuid().ToString("N")[..8];

        _logger.LogInformation("API Request [{RequestId}]: {Method} {Url}",
            requestId, method, url);

        if (body != null)
        {
            _logger.LogDebug("Request Body [{RequestId}]: {Body}",
                requestId, JsonSerializer.Serialize(body));
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await base.ExecuteRequestAsync(method, url, body);

            _logger.LogInformation(
                "API Response [{RequestId}]: {StatusCode} in {ElapsedMs}ms",
                requestId, response.StatusCode, stopwatch.ElapsedMilliseconds);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Response Body [{RequestId}]: {Body}",
                    requestId, response.Body);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "API Error [{RequestId}]: {Method} {Url} failed after {ElapsedMs}ms",
                requestId, method, url, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
```

### Adding Caching Support

```csharp
using CurlDotNet;
using Microsoft.Extensions.Caching.Memory;
// Install: dotnet add package CurlDotNet
// Install: dotnet add package Microsoft.Extensions.Caching.Memory
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class CachedApiClient : ApiClient
{
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _defaultCacheOptions;

    public CachedApiClient(string baseUrl, IMemoryCache cache,
        string apiKey = null) : base(baseUrl, apiKey)
    {
        _cache = cache;
        _defaultCacheOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(5),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
        };
    }

    public async Task<T> GetCachedAsync<T>(string endpoint,
        TimeSpan? cacheDuration = null)
    {
        var cacheKey = $"api_get_{endpoint}";

        if (_cache.TryGetValue<T>(cacheKey, out var cachedResult))
        {
            return cachedResult;
        }

        var result = await GetAsync<T>(endpoint);

        var cacheOptions = cacheDuration.HasValue
            ? new MemoryCacheEntryOptions
                { AbsoluteExpirationRelativeToNow = cacheDuration }
            : _defaultCacheOptions;

        _cache.Set(cacheKey, result, cacheOptions);

        return result;
    }

    public void InvalidateCache(string endpoint = null)
    {
        if (string.IsNullOrEmpty(endpoint))
        {
            // Clear all cache if no endpoint specified
            if (_cache is MemoryCache memCache)
            {
                memCache.Compact(1.0);
            }
        }
        else
        {
            _cache.Remove($"api_get_{endpoint}");
        }
    }
}
```

## Real-World Example: GitHub API Client

```csharp
using CurlDotNet;
using System.Text.Json;
using System.Text.Json.Serialization;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class GitHubClient : IDisposable
{
    private readonly Curl _curl;
    private readonly string _token;
    private bool _disposed;

    public GitHubClient(string token)
    {
        _token = token;
        _curl = new Curl("https://api.github.com")
            .WithHeader("Authorization", $"Bearer {token}")
            .WithHeader("Accept", "application/vnd.github.v3+json")
            .WithHeader("User-Agent", "CurlDotNet-GitHub-Client/1.0")
            .WithTimeout(30);
    }

    // Get user information
    public async Task<GitHubUser> GetUserAsync(string username)
    {
        var response = await _curl.GetAsync($"/users/{username}");
        return JsonSerializer.Deserialize<GitHubUser>(response.Body);
    }

    // List user repositories
    public async Task<List<GitHubRepository>> GetUserRepositoriesAsync(
        string username, int page = 1, int perPage = 30)
    {
        var response = await _curl.GetAsync(
            $"/users/{username}/repos?page={page}&per_page={perPage}");
        return JsonSerializer.Deserialize<List<GitHubRepository>>(response.Body);
    }

    // Create a new repository
    public async Task<GitHubRepository> CreateRepositoryAsync(
        string name, string description, bool isPrivate = false)
    {
        var payload = new
        {
            name,
            description,
            @private = isPrivate,
            auto_init = true
        };

        var response = await _curl.PostJsonAsync("/user/repos", payload);
        return JsonSerializer.Deserialize<GitHubRepository>(response.Body);
    }

    // Create an issue
    public async Task<GitHubIssue> CreateIssueAsync(
        string owner, string repo, string title, string body,
        string[] labels = null)
    {
        var payload = new
        {
            title,
            body,
            labels = labels ?? Array.Empty<string>()
        };

        var response = await _curl.PostJsonAsync(
            $"/repos/{owner}/{repo}/issues", payload);
        return JsonSerializer.Deserialize<GitHubIssue>(response.Body);
    }

    // Star a repository
    public async Task<bool> StarRepositoryAsync(string owner, string repo)
    {
        var response = await _curl.PutAsync(
            $"/user/starred/{owner}/{repo}");
        return response.StatusCode == 204;
    }

    // Search repositories
    public async Task<GitHubSearchResult<GitHubRepository>> SearchRepositoriesAsync(
        string query, string sort = "stars", int page = 1)
    {
        var encodedQuery = Uri.EscapeDataString(query);
        var response = await _curl.GetAsync(
            $"/search/repositories?q={encodedQuery}&sort={sort}&page={page}");
        return JsonSerializer.Deserialize<GitHubSearchResult<GitHubRepository>>(
            response.Body);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _curl?.Dispose();
            _disposed = true;
        }
    }
}

// Data models
public class GitHubUser
{
    [JsonPropertyName("login")]
    public string Login { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("bio")]
    public string Bio { get; set; }

    [JsonPropertyName("public_repos")]
    public int PublicRepos { get; set; }

    [JsonPropertyName("followers")]
    public int Followers { get; set; }
}

public class GitHubRepository
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("full_name")]
    public string FullName { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("private")]
    public bool IsPrivate { get; set; }

    [JsonPropertyName("stargazers_count")]
    public int Stars { get; set; }
}

public class GitHubIssue
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("body")]
    public string Body { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }
}

public class GitHubSearchResult<T>
{
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    [JsonPropertyName("items")]
    public List<T> Items { get; set; }
}
```

## Usage Examples

### Basic Usage

```csharp
using CurlDotNet;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

// Simple API client
using var client = new ApiClient("https://api.example.com", "your-api-key");

// GET request
var users = await client.GetAsync<List<User>>("/users");

// POST request
var newUser = new User { Name = "John", Email = "john@example.com" };
var created = await client.PostAsync<User, User>("/users", newUser);

// PUT request
newUser.Name = "John Doe";
var updated = await client.PutAsync<User, User>($"/users/{created.Id}", newUser);

// DELETE request
var deleted = await client.DeleteAsync($"/users/{created.Id}");
```

### With Dependency Injection

```csharp
using Microsoft.Extensions.DependencyInjection;
using CurlDotNet;
// Install: dotnet add package CurlDotNet
// Install: dotnet add package Microsoft.Extensions.DependencyInjection
// NuGet: https://www.nuget.org/packages/CurlDotNet/

// In Startup.cs or Program.cs
services.AddSingleton<ApiClient>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    return new ApiClient(
        config["ApiSettings:BaseUrl"],
        config["ApiSettings:ApiKey"]
    );
});

services.AddSingleton<GitHubClient>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    return new GitHubClient(config["GitHub:Token"]);
});

// In your controller or service
public class MyService
{
    private readonly ApiClient _apiClient;
    private readonly GitHubClient _githubClient;

    public MyService(ApiClient apiClient, GitHubClient githubClient)
    {
        _apiClient = apiClient;
        _githubClient = githubClient;
    }

    public async Task DoWorkAsync()
    {
        var data = await _apiClient.GetAsync<MyData>("/data");
        var repos = await _githubClient.GetUserRepositoriesAsync("octocat");
    }
}
```

### With Configuration

```csharp
using CurlDotNet;
using Microsoft.Extensions.Options;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class ApiClientOptions
{
    public string BaseUrl { get; set; }
    public string ApiKey { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxRetries { get; set; } = 3;
}

public class ConfigurableApiClient : ApiClient
{
    public ConfigurableApiClient(IOptions<ApiClientOptions> options)
        : base(options.Value.BaseUrl, options.Value.ApiKey)
    {
        // Apply additional configuration
        _curl.WithTimeout(options.Value.TimeoutSeconds);
        MaxRetries = options.Value.MaxRetries;
    }
}

// In appsettings.json
{
    "ApiClient": {
        "BaseUrl": "https://api.example.com",
        "ApiKey": "your-api-key",
        "TimeoutSeconds": 60,
        "MaxRetries": 5
    }
}

// In Program.cs
builder.Services.Configure<ApiClientOptions>(
    builder.Configuration.GetSection("ApiClient"));
builder.Services.AddSingleton<ConfigurableApiClient>();
```

## Best Practices

### 1. Always Dispose Resources

```csharp
// Use using statement for automatic disposal
using var client = new ApiClient("https://api.example.com");
// Client is automatically disposed
```

### 2. Handle Errors Gracefully

```csharp
try
{
    var result = await client.GetAsync<Data>("/endpoint");
}
catch (CurlTimeoutException ex)
{
    // Handle timeout
    _logger.LogError(ex, "Request timed out");
}
catch (CurlHttpException ex) when (ex.StatusCode == 404)
{
    // Handle not found
    return NotFound();
}
catch (CurlException ex)
{
    // Handle other curl errors
    _logger.LogError(ex, "Curl error occurred");
}
```

### 3. Use Strongly Typed Models

```csharp
// Good: Strongly typed
var user = await client.GetAsync<User>("/users/123");

// Avoid: Dynamic or object
var data = await client.GetAsync<dynamic>("/users/123");
```

### 4. Implement Circuit Breaker for Resilience

```csharp
using Polly;
using Polly.CircuitBreaker;

var circuitBreaker = Policy
    .HandleResult<CurlResult>(r => r.StatusCode >= 500)
    .CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 3,
        durationOfBreak: TimeSpan.FromSeconds(30));
```

## Testing Your API Client

```csharp
using Xunit;
using Moq;
using CurlDotNet;
// Install: dotnet add package CurlDotNet
// Install: dotnet add package xunit
// Install: dotnet add package Moq

public class ApiClientTests
{
    [Fact]
    public async Task GetAsync_ReturnsExpectedData()
    {
        // Arrange
        var mockCurl = new Mock<ICurl>();
        mockCurl.Setup(x => x.GetAsync(It.IsAny<string>()))
            .ReturnsAsync(new CurlResult
            {
                StatusCode = 200,
                Body = "{\"id\":1,\"name\":\"Test\"}"
            });

        var client = new ApiClient("https://api.test.com");

        // Act
        var result = await client.GetAsync<TestData>("/test");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test", result.Name);
    }
}
```

## Troubleshooting

### Connection Issues
- Check base URL format (no trailing slash issues)
- Verify network connectivity
- Check firewall/proxy settings

### Authentication Failures
- Verify API key/token is correct
- Check token expiration
- Ensure proper header format

### Deserialization Errors
- Match JSON property names with model properties
- Use JsonPropertyName attributes if needed
- Handle nullable properties correctly

## Key Takeaways

- ✅ Create reusable API clients with CurlDotNet
- ✅ Implement retry logic and resilience patterns
- ✅ Add logging and monitoring capabilities
- ✅ Use dependency injection for better testability
- ✅ Handle errors gracefully with specific exceptions
- ✅ Implement caching for better performance
- ✅ Use strongly typed models for safety

## Related Examples

- [Retry Logic Pattern](./retry-logic)
- [Rate Limiting](./rate-limiting)
- [Authentication Patterns](../beginner/authentication)
- [Error Handling](../beginner/handle-errors)

---

*Part of the CurlDotNet Cookbook - Professional patterns for C# and .NET developers*