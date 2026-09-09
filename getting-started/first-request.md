# Your First Request

This guide walks you through making your first HTTP request with CurlDotNet.

## Prerequisites

Before you begin, ensure you have:
- CurlDotNet installed (`dotnet add package CurlDotNet`)
- A .NET project ready to go

See the [Installation Guide](installation.html) if you need help setting up.

## The Simplest Request

The easiest way to make a request is to just paste a curl command:

```csharp
using CurlDotNet;

// Make a GET request - it's that simple!
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

Console.WriteLine(result.Body);
```

That's it! You've made your first HTTP request with CurlDotNet.

## Understanding the Response

Every request returns a `CurlResult` object with useful properties:

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

// Check if the request succeeded (status 200-299)
Console.WriteLine($"Success: {result.IsSuccess}");

// Get the HTTP status code
Console.WriteLine($"Status Code: {result.StatusCode}");

// Get the response body as a string
Console.WriteLine($"Body: {result.Body}");

// Access response headers
foreach (var header in result.Headers)
{
    Console.WriteLine($"{header.Key}: {header.Value}");
}
```

## Three Ways to Make Requests

CurlDotNet offers three approaches - use whichever fits your style:

### 1. String API (Copy & Paste)

Perfect for when you have a curl command from documentation:

```csharp
// Just paste any curl command from API docs, Stack Overflow, etc.
var result = await Curl.ExecuteAsync(@"
    curl https://api.github.com/users/octocat \
      -H 'User-Agent: MyApp'
");
```

### 2. Builder API (Type-Safe)

Get IntelliSense and compile-time checking:

```csharp
var result = await CurlRequestBuilder
    .Get("https://api.github.com/users/octocat")
    .WithHeader("User-Agent", "MyApp")
    .ExecuteAsync();
```

### 3. Quick Methods

Convenience methods for simple operations:

```csharp
// Simple GET
var result = await Curl.GetAsync("https://httpbin.org/get");

// POST with data
var result = await Curl.PostAsync("https://httpbin.org/post", "name=John");

// POST with JSON
var result = await Curl.PostJsonAsync("https://httpbin.org/post", new { name = "John" });

// Download a file
await Curl.DownloadAsync("https://example.com/file.pdf", "output.pdf");
```

## Working with JSON

Most APIs return JSON. CurlDotNet makes it easy to work with:

```csharp
// Define your model
public class GitHubUser
{
    public string Login { get; set; }
    public string Name { get; set; }
    public int Followers { get; set; }
}

// Make the request and parse JSON
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

if (result.IsSuccess)
{
    var user = result.ParseJson<GitHubUser>();
    Console.WriteLine($"User: {user.Name} has {user.Followers} followers");
}
```

## Error Handling

Always handle potential errors:

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com/data");

    if (result.IsSuccess)
    {
        Console.WriteLine("Success!");
        Console.WriteLine(result.Body);
    }
    else
    {
        Console.WriteLine($"HTTP Error: {result.StatusCode}");
    }
}
catch (CurlDotNet.Exceptions.CurlTimeoutException ex)
{
    Console.WriteLine($"Request timed out after {ex.Timeout} seconds");
}
catch (CurlDotNet.Exceptions.CurlCouldntResolveHostException ex)
{
    Console.WriteLine($"Could not resolve host: {ex.Hostname}");
}
catch (CurlDotNet.Exceptions.CurlException ex)
{
    Console.WriteLine($"Curl error: {ex.Message}");
}
```

## Common curl Options

Here are some frequently used curl options:

| Option | Description | Example |
|--------|-------------|---------|
| `-X METHOD` | Set HTTP method | `-X POST` |
| `-H 'Header: Value'` | Add header | `-H 'Authorization: Bearer token'` |
| `-d 'data'` | Send data in body | `-d '{"name":"John"}'` |
| `-u user:pass` | Basic authentication | `-u admin:secret` |
| `-o file` | Save output to file | `-o response.json` |
| `-L` | Follow redirects | `-L` |
| `-k` | Skip SSL verification | `-k` (dev only!) |
| `--max-time 30` | Timeout in seconds | `--max-time 30` |

## Complete Example

Here's a complete example that puts it all together:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Fetching GitHub user information...\n");

        try
        {
            // Make the request
            var result = await Curl.ExecuteAsync(@"
                curl https://api.github.com/users/octocat \
                  -H 'User-Agent: CurlDotNet-Example' \
                  -H 'Accept: application/json'
            ");

            // Check result
            if (result.IsSuccess)
            {
                var user = result.ParseJson<dynamic>();
                Console.WriteLine($"Username: {user.login}");
                Console.WriteLine($"Name: {user.name}");
                Console.WriteLine($"Public Repos: {user.public_repos}");
                Console.WriteLine($"Followers: {user.followers}");
            }
            else
            {
                Console.WriteLine($"Error: {result.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed: {ex.Message}");
        }
    }
}
```

## Next Steps

Now that you've made your first request:

1. [Configure CurlDotNet](configuration.html) - Set timeouts, proxies, and more
2. [Browse the Cookbook](../cookbook/) - Ready-to-use recipes
3. [Learn More Tutorials](../tutorials/) - Deep dive into features
4. [API Reference](../api/index.html) - Complete API documentation

## Need Help?

- [Common Issues](../troubleshooting/common-issues.html) - Solutions to frequent problems
- [FAQ](../troubleshooting/faq.html) - Frequently asked questions
- [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues) - Report bugs or ask questions
