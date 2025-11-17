# CurlDotNet Code Samples

## Overview

This directory contains practical, real-world code samples demonstrating how to use CurlDotNet effectively. Each sample is complete, tested, and ready to run.

## Sample Categories

### Basic Requests
- **Simple GET Request** - Fetch data from an API
- **POST with Data** - Send data to create resources
- **PUT/PATCH Updates** - Modify existing resources
- **DELETE Operations** - Remove resources

### Working with JSON
- **Parsing JSON Responses** - Deserialize API responses
- **Sending JSON Data** - POST/PUT with JSON bodies
- **Dynamic JSON** - Work with unknown JSON structures
- **JSON Arrays** - Handle lists and collections

### Authentication
- **API Keys** - Query parameters and headers
- **Bearer Tokens** - OAuth 2.0 authentication
- **Basic Authentication** - Username/password auth
- **Custom Headers** - API-specific authentication

### File Operations
- **Download Files** - Save responses to disk
- **Upload Files** - Send files via multipart/form-data
- **Progress Tracking** - Monitor upload/download progress
- **Streaming Large Files** - Handle big files efficiently

### Advanced Topics
- **Parallel Requests** - Multiple requests simultaneously
- **Rate Limiting** - Respect API limits
- **Retry Logic** - Handle transient failures
- **Cancellation** - Cancel long-running requests
- **Custom Error Handling** - Robust error management

## Quick Start Samples

### Example 1: Simple GET Request

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        // Fetch GitHub's API root endpoint
        var result = await Curl.ExecuteAsync("curl https://api.github.com");

        if (result.IsSuccess)
        {
            Console.WriteLine($"Status: {result.StatusCode}");
            Console.WriteLine($"Body: {result.Body}");
        }
    }
}
```

**Output:**
```
Status: 200
Body: {"current_user_url":"https://api.github.com/user",...}
```

### Example 2: POST JSON Data

```csharp
using System;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        var user = new { name = "John Doe", email = "john@example.com" };
        var json = JsonSerializer.Serialize(user);

        var result = await Curl.ExecuteAsync($@"
            curl -X POST https://jsonplaceholder.typicode.com/users \
                 -H 'Content-Type: application/json' \
                 -d '{json}'
        ");

        Console.WriteLine($"Created user: {result.Body}");
    }
}
```

### Example 3: Download a File

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        var url = "https://example.com/file.pdf";
        var result = await Curl.ExecuteAsync($"curl {url}");

        if (result.IsSuccess)
        {
            await File.WriteAllBytesAsync("file.pdf", result.BodyBytes);
            Console.WriteLine("File downloaded successfully!");
        }
    }
}
```

### Example 4: API with Authentication

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        var token = "your_api_token";

        var result = await Curl.ExecuteAsync($@"
            curl https://api.github.com/user \
                 -H 'Authorization: Bearer {token}'
        ");

        Console.WriteLine($"User data: {result.Body}");
    }
}
```

### Example 5: Parallel Requests

```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        var urls = new[]
        {
            "https://api.github.com",
            "https://httpbin.org/get",
            "https://jsonplaceholder.typicode.com/posts/1"
        };

        var tasks = urls.Select(url =>
            Curl.ExecuteAsync($"curl {url}")
        );

        var results = await Task.WhenAll(tasks);

        foreach (var result in results)
        {
            Console.WriteLine($"Status: {result.StatusCode}");
        }
    }
}
```

## Complete Sample Applications

### Weather App

```csharp
using System;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;

public class WeatherApp
{
    public class WeatherData
    {
        public string Location { get; set; }
        public double Temperature { get; set; }
        public string Condition { get; set; }
    }

    public static async Task<WeatherData> GetWeatherAsync(string city)
    {
        var url = $"https://wttr.in/{city}?format=j1";
        var result = await Curl.ExecuteAsync($"curl {url}");

        if (!result.IsSuccess)
        {
            throw new Exception($"Failed to get weather: {result.StatusCode}");
        }

        var json = JsonDocument.Parse(result.Body);
        var current = json.RootElement.GetProperty("current_condition")[0];

        return new WeatherData
        {
            Location = city,
            Temperature = current.GetProperty("temp_C").GetDouble(),
            Condition = current.GetProperty("weatherDesc")[0]
                .GetProperty("value").GetString()
        };
    }

    public static async Task Main()
    {
        var cities = new[] { "London", "NewYork", "Tokyo" };

        foreach (var city in cities)
        {
            var weather = await GetWeatherAsync(city);
            Console.WriteLine($"{weather.Location}: {weather.Temperature}°C, {weather.Condition}");
        }
    }
}
```

### GitHub Repository Browser

```csharp
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet;

public class GitHubBrowser
{
    private const string API_BASE = "https://api.github.com";

    public class Repository
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Stars { get; set; }
        public string Language { get; set; }
        public string Url { get; set; }
    }

    public static async Task<List<Repository>> SearchRepositoriesAsync(string query)
    {
        var encoded = Uri.EscapeDataString(query);
        var url = $"{API_BASE}/search/repositories?q={encoded}&sort=stars&order=desc";

        var result = await Curl.ExecuteAsync($@"
            curl {url} \
                 -H 'Accept: application/vnd.github.v3+json'
        ");

        if (!result.IsSuccess)
        {
            throw new Exception($"Search failed: {result.StatusCode}");
        }

        var json = JsonDocument.Parse(result.Body);
        var items = json.RootElement.GetProperty("items");

        var repos = new List<Repository>();
        foreach (var item in items.EnumerateArray())
        {
            repos.Add(new Repository
            {
                Name = item.GetProperty("full_name").GetString(),
                Description = item.GetProperty("description").GetString() ?? "",
                Stars = item.GetProperty("stargazers_count").GetInt32(),
                Language = item.GetProperty("language").GetString() ?? "Unknown",
                Url = item.GetProperty("html_url").GetString()
            });
        }

        return repos;
    }

    public static async Task Main()
    {
        Console.Write("Search GitHub: ");
        var query = Console.ReadLine();

        var repos = await SearchRepositoriesAsync(query);

        Console.WriteLine($"\nTop {repos.Count} results:\n");
        foreach (var repo in repos.Take(10))
        {
            Console.WriteLine($"{repo.Name} ({repo.Stars:N0} stars)");
            Console.WriteLine($"  {repo.Description}");
            Console.WriteLine($"  Language: {repo.Language}");
            Console.WriteLine($"  {repo.Url}\n");
        }
    }
}
```

### API Client with Rate Limiting

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class RateLimitedApiClient
{
    private readonly string _baseUrl;
    private int _remaining = int.MaxValue;
    private DateTime _resetTime = DateTime.MinValue;

    public RateLimitedApiClient(string baseUrl)
    {
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public async Task<CurlResult> GetAsync(string endpoint)
    {
        await WaitForRateLimitAsync();

        var url = $"{_baseUrl}/{endpoint.TrimStart('/')}";
        var result = await Curl.ExecuteAsync($"curl {url}");

        UpdateRateLimitInfo(result);

        return result;
    }

    private async Task WaitForRateLimitAsync()
    {
        if (_remaining <= 0 && _resetTime > DateTime.UtcNow)
        {
            var waitTime = _resetTime - DateTime.UtcNow;
            Console.WriteLine($"Rate limit reached. Waiting {waitTime.TotalSeconds:F0}s...");
            await Task.Delay(waitTime);
            _remaining = int.MaxValue;
        }
    }

    private void UpdateRateLimitInfo(CurlResult result)
    {
        if (int.TryParse(result.GetHeader("X-RateLimit-Remaining"), out int remaining))
        {
            _remaining = remaining;
        }

        if (long.TryParse(result.GetHeader("X-RateLimit-Reset"), out long resetTs))
        {
            _resetTime = DateTimeOffset.FromUnixTimeSeconds(resetTs).DateTime;
        }

        Console.WriteLine($"Rate limit: {_remaining} remaining");
    }
}

// Usage
var client = new RateLimitedApiClient("https://api.github.com");

for (int i = 0; i < 100; i++)
{
    var result = await client.GetAsync("/users/octocat");
    Console.WriteLine($"Request {i + 1}: {result.StatusCode}");
}
```

## Sample by Use Case

### E-Commerce API Integration

```csharp
public class ProductCatalog
{
    public async Task<List<Product>> GetProductsAsync(string category)
    {
        var result = await Curl.ExecuteAsync(
            $"curl https://api.store.com/products?category={Uri.EscapeDataString(category)}"
        );
        return JsonSerializer.Deserialize<List<Product>>(result.Body);
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        var json = JsonSerializer.Serialize(order);
        var result = await Curl.ExecuteAsync($@"
            curl -X POST https://api.store.com/orders \
                 -H 'Content-Type: application/json' \
                 -H 'Authorization: Bearer {_apiKey}' \
                 -d '{json}'
        ");
        return JsonSerializer.Deserialize<Order>(result.Body);
    }
}
```

### Social Media Bot

{% raw %}
```csharp
public class SocialMediaBot
{
    private readonly string _accessToken;

    public async Task PostTweetAsync(string message)
    {
        var result = await Curl.ExecuteAsync($@"
            curl -X POST https://api.twitter.com/2/tweets \
                 -H 'Authorization: Bearer {_accessToken}' \
                 -H 'Content-Type: application/json' \
                 -d '{{""text"":""{message}""}}'
        ");
        Console.WriteLine($"Posted: {result.StatusCode}");
    }

    public async Task<List<Tweet>> GetTimelineAsync()
    {
        var result = await Curl.ExecuteAsync($@"
            curl https://api.twitter.com/2/tweets \
                 -H 'Authorization: Bearer {_accessToken}'
        ");
        return JsonSerializer.Deserialize<List<Tweet>>(result.Body);
    }
}
```
{% endraw %}

### Webhook Handler

```csharp
public class WebhookProcessor
{
    public async Task SendWebhookAsync(string url, object payload)
    {
        var json = JsonSerializer.Serialize(payload);

        var result = await Curl.ExecuteAsync($@"
            curl -X POST {url} \
                 -H 'Content-Type: application/json' \
                 -d '{json}'
        ");

        if (!result.IsSuccess)
        {
            throw new Exception($"Webhook failed: {result.StatusCode}");
        }
    }
}
```

## Testing Samples

### Mock API Testing

```csharp
public class ApiTests
{
    [Test]
    public async Task TestApiEndpoint()
    {
        // Using httpbin.org for testing
        var result = await Curl.ExecuteAsync(@"
            curl -X POST https://httpbin.org/post \
                 -H 'Content-Type: application/json' \
                 -d '{""test"":""data""}'
        ");

        Assert.True(result.IsSuccess);
        Assert.Equal(200, result.StatusCode);
        Assert.Contains("test", result.Body);
    }
}
```

## Related Resources

- [API Guide](../api-guide/README.md) - Complete API reference
- [Tutorials](../tutorials/README.md) - Step-by-step learning
- [Guides](../guides/README.md) - Topic-specific guides
- [Examples Repository](https://github.com/jacob-mellor/curl-dot-net/tree/master/examples) - Source code for all examples

## Contributing Samples

Have a useful sample to share? We'd love to include it! Please submit a pull request to our [GitHub repository](https://github.com/jacob-mellor/curl-dot-net).

---

**Need help with a specific use case?** Ask in our [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions).
