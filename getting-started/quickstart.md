# Quick Start Guide

Get up and running with CurlDotNet in under 5 minutes!

## Installation

### Using Package Manager Console
```powershell
Install-Package CurlDotNet
```

### Using .NET CLI
```bash
dotnet add package CurlDotNet
```

### Using PackageReference
```xml
<PackageReference Include="CurlDotNet" Version="1.*" />
```

## Your First Request

### 1. Create a Console Application
```bash
dotnet new console -n MyCurlApp
cd MyCurlApp
dotnet add package CurlDotNet
```

### 2. Write Your Code

Replace the contents of `Program.cs`:

```csharp
using CurlDotNet;

// Create a Curl instance
var curl = new Curl();

// Make a GET request
var result = await curl.GetAsync("https://api.github.com/users/github");

// Check if successful and display the result
if (result.IsSuccess)
{
    Console.WriteLine("Success! Response:");
    Console.WriteLine(result.Data);
}
else
{
    Console.WriteLine($"Error: {result.Error}");
}
```

### 3. Run Your Application
```bash
dotnet run
```

That's it! You've made your first HTTP request with CurlDotNet.

## Common Operations

### GET Request
```csharp
var curl = new Curl();
var result = await curl.GetAsync("https://api.example.com/data");
Console.WriteLine(result.Data);
```

### POST JSON
```csharp
var curl = new Curl();
var data = new { name = "John", age = 30 };
var result = await curl.PostJsonAsync("https://api.example.com/users", data);
```

### With Headers
```csharp
var curl = new Curl();
curl.Headers.Add("Authorization", "Bearer your-token");
curl.Headers.Add("X-API-Key", "your-api-key");

var result = await curl.GetAsync("https://api.example.com/protected");
```

### Download File
```csharp
var curl = new Curl();
await curl.DownloadFileAsync(
    "https://example.com/file.pdf",
    "downloaded-file.pdf"
);
```

### Handle Errors
```csharp
var result = await curl.GetAsync("https://api.example.com/data");

if (result.IsSuccess)
{
    // Process successful response
    ProcessData(result.Data);
}
else
{
    // Handle error
    Console.WriteLine($"Request failed: {result.Error}");
    Console.WriteLine($"Status code: {result.StatusCode}");
}
```

## Complete Example: GitHub User Info

Here's a complete example that fetches and displays GitHub user information:

```csharp
using CurlDotNet;
using System.Text.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Create Curl instance
        var curl = new Curl();

        // GitHub requires a User-Agent header
        curl.Headers.Add("User-Agent", "CurlDotNet-QuickStart");

        // Get user info
        Console.Write("Enter a GitHub username: ");
        var username = Console.ReadLine();

        var result = await curl.GetAsync($"https://api.github.com/users/{username}");

        if (result.IsSuccess)
        {
            // Parse JSON response
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var user = JsonSerializer.Deserialize<GitHubUser>(result.Data, options);

            // Display user info
            Console.WriteLine($"\n=== GitHub User: {user.Login} ===");
            Console.WriteLine($"Name: {user.Name ?? "N/A"}");
            Console.WriteLine($"Company: {user.Company ?? "N/A"}");
            Console.WriteLine($"Location: {user.Location ?? "N/A"}");
            Console.WriteLine($"Bio: {user.Bio ?? "N/A"}");
            Console.WriteLine($"Public Repos: {user.PublicRepos}");
            Console.WriteLine($"Followers: {user.Followers}");
            Console.WriteLine($"Following: {user.Following}");
            Console.WriteLine($"Created: {user.CreatedAt:yyyy-MM-dd}");
        }
        else
        {
            Console.WriteLine($"Error: {result.Error}");

            if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine("User not found. Please check the username.");
            }
        }
    }
}

public class GitHubUser
{
    public string Login { get; set; }
    public string Name { get; set; }
    public string Company { get; set; }
    public string Location { get; set; }
    public string Bio { get; set; }
    public int PublicRepos { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

## What Makes CurlDotNet Special?

### 1. Simplicity
```csharp
// HttpClient way (complex)
using var client = new HttpClient();
var response = await client.GetAsync(url);
response.EnsureSuccessStatusCode();
var content = await response.Content.ReadAsStringAsync();

// CurlDotNet way (simple)
var curl = new Curl();
var result = await curl.GetAsync(url);
var content = result.Data;
```

### 2. Better Error Handling
```csharp
// No try-catch needed
var result = await curl.GetAsync(url);
if (!result.IsSuccess)
{
    Console.WriteLine($"Error: {result.Error}");
}
```

### 3. curl Compatibility
```bash
# curl command
curl -H "Authorization: Bearer token" https://api.example.com/data

# Equivalent CurlDotNet code
var curl = new Curl();
curl.Headers.Add("Authorization", "Bearer token");
var result = await curl.GetAsync("https://api.example.com/data");
```

## Common Patterns

### API Client
```csharp
public class ApiClient
{
    private readonly Curl _curl;

    public ApiClient(string apiKey)
    {
        _curl = new Curl();
        _curl.Headers.Add("X-API-Key", apiKey);
        _curl.Headers.Add("Accept", "application/json");
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        var result = await _curl.GetAsync($"https://api.example.com{endpoint}");

        if (result.IsSuccess)
        {
            return JsonSerializer.Deserialize<T>(result.Data);
        }

        throw new Exception($"API request failed: {result.Error}");
    }
}
```

### Retry Logic
```csharp
public async Task<CurlResult> GetWithRetry(string url, int maxAttempts = 3)
{
    var curl = new Curl();

    for (int i = 0; i < maxAttempts; i++)
    {
        var result = await curl.GetAsync(url);

        if (result.IsSuccess)
            return result;

        if (i < maxAttempts - 1)
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
    }

    throw new Exception($"Failed after {maxAttempts} attempts");
}
```

## Next Steps

Now that you're up and running:

1. **[Read the Tutorials](../tutorials/01-what-is-dotnet.html)** - Step-by-step learning path
2. **[Explore the Cookbook](../cookbook/index.html)** - Ready-to-use recipes
3. **[Check the Examples](https://github.com/jacob-mellor/curl-dot-net/tree/master/examples)** - Complete applications
4. **[API Reference](../api/index.html)** - Detailed documentation

## Need Help?

- **[GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)** - Report bugs or ask questions
- **[Documentation](../index.html)** - Complete documentation
- **[Migration Guides](../migration/index.html)** - Moving from other HTTP clients

## Tips for Success

1. **Reuse Curl instances** - Create once, use many times
2. **Always check IsSuccess** - Before accessing response data
3. **Use async/await** - All methods are async
4. **Set appropriate timeouts** - Don't wait forever
5. **Log errors** - Track what goes wrong

---

Welcome to the CurlDotNet community! Happy coding! 🚀