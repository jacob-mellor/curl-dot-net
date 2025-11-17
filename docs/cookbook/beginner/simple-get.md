# Recipe: Simple GET Request

## 🎯 What You'll Build

A program that fetches data from a web API using a GET request - the most common HTTP operation using **CurlDotNet**, the pure .NET implementation of curl.

## 🥘 Ingredients

- CurlDotNet package - Install from [NuGet](https://www.nuget.org/packages/CurlDotNet/): `dotnet add package CurlDotNet`
- 5 minutes
- Internet connection

## 📖 What is a GET Request?

A GET request is how you ask a server to send you data. Think of it like:
- Opening a webpage in your browser
- Asking a librarian for a book
- Looking up a word in a dictionary

GET requests:
- **Don't change anything** on the server (read-only)
- **Can be bookmarked** and shared
- **Can be cached** by browsers and proxies
- **Are the default** HTTP method

## 🍳 The Recipe

### Step 1: Simplest Possible GET

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        // Just fetch a URL
        var result = await Curl.ExecuteAsync("https://api.github.com");

        Console.WriteLine(result.Body);
    }
}
```

That's it! The simplest GET request possible.

### Step 2: GET with Status Check

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        var result = await Curl.ExecuteAsync("curl https://api.github.com");

        if (result.IsSuccess)
        {
            Console.WriteLine("Success!");
            Console.WriteLine(result.Body);
        }
        else
        {
            Console.WriteLine($"Failed with status: {result.StatusCode}");
        }
    }
}
```

### Step 3: GET with Headers

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        // Add custom headers
        var result = await Curl.ExecuteAsync(@"
            curl https://api.github.com \
              -H 'Accept: application/json' \
              -H 'User-Agent: MyApp/1.0'
        ");

        if (result.IsSuccess)
        {
            Console.WriteLine($"Content-Type: {result.ContentType}");
            Console.WriteLine($"Response Length: {result.Body.Length}");
        }
    }
}
```

## 🎨 Complete Examples

### Example 1: Fetch User Data

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class FetchUser
{
    static async Task Main()
    {
        Console.WriteLine("Fetching user data from GitHub...\n");

        // GET request to GitHub API
        var result = await Curl.ExecuteAsync(
            "curl https://api.github.com/users/octocat"
        );

        if (result.IsSuccess)
        {
            // Parse JSON response
            dynamic user = result.AsJsonDynamic();

            Console.WriteLine($"Username: {user.login}");
            Console.WriteLine($"Name: {user.name}");
            Console.WriteLine($"Location: {user.location}");
            Console.WriteLine($"Public Repos: {user.public_repos}");
            Console.WriteLine($"Followers: {user.followers}");
            Console.WriteLine($"Following: {user.following}");
        }
        else
        {
            Console.WriteLine($"Error: {result.StatusCode}");
        }
    }
}
```

### Example 2: Fetch List of Items

```csharp
using System;
using System.Threading.Tasks;
using System.Text.Json;
using CurlDotNet;

public class Post
{
    public int UserId { get; set; }
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
}

class FetchPosts
{
    static async Task Main()
    {
        Console.WriteLine("Fetching posts...\n");

        // GET request to JSONPlaceholder API
        var result = await Curl.ExecuteAsync(
            "curl https://jsonplaceholder.typicode.com/posts"
        );

        if (result.IsSuccess)
        {
            // Parse JSON array
            var posts = result.ParseJson<Post[]>();

            Console.WriteLine($"Retrieved {posts.Length} posts:\n");

            // Show first 5 posts
            foreach (var post in posts.Take(5))
            {
                Console.WriteLine($"#{post.Id}: {post.Title}");
                Console.WriteLine($"   {post.Body.Substring(0, 60)}...\n");
            }
        }
    }
}
```

### Example 3: GET with Query Parameters

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class SearchExample
{
    static async Task Main()
    {
        // Search with query parameters
        string searchTerm = "dotnet";
        int page = 1;
        int perPage = 10;

        // Build URL with query parameters
        string url = $"https://api.github.com/search/repositories?q={searchTerm}&page={page}&per_page={perPage}";

        Console.WriteLine($"Searching for '{searchTerm}'...\n");

        var result = await Curl.ExecuteAsync($"curl '{url}'");

        if (result.IsSuccess)
        {
            dynamic data = result.AsJsonDynamic();

            Console.WriteLine($"Found {data.total_count} repositories:\n");

            foreach (var repo in data.items)
            {
                Console.WriteLine($"{repo.full_name}");
                Console.WriteLine($"  ⭐ {repo.stargazers_count} stars");
                Console.WriteLine($"  {repo.description}");
                Console.WriteLine();
            }
        }
    }
}
```

### Example 4: GET with Authentication

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class AuthenticatedGet
{
    static async Task Main()
    {
        // Your API token (keep this secret!)
        string apiToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN")
                          ?? "your-token-here";

        Console.WriteLine("Fetching authenticated user data...\n");

        // GET with Bearer token authentication
        var result = await Curl.ExecuteAsync($@"
            curl https://api.github.com/user \
              -H 'Authorization: Bearer {apiToken}' \
              -H 'Accept: application/vnd.github.v3+json'
        ");

        if (result.IsSuccess)
        {
            dynamic user = result.AsJsonDynamic();
            Console.WriteLine($"Hello, {user.name}!");
            Console.WriteLine($"Email: {user.email}");
            Console.WriteLine($"Private repos: {user.total_private_repos}");
        }
        else if (result.StatusCode == 401)
        {
            Console.WriteLine("Authentication failed. Check your token.");
        }
        else
        {
            Console.WriteLine($"Error: {result.StatusCode}");
        }
    }
}
```

### Example 5: Multiple GET Requests

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class MultipleRequests
{
    static async Task Main()
    {
        Console.WriteLine("Checking multiple services...\n");

        string[] endpoints = {
            "https://api.github.com",
            "https://httpbin.org/status/200",
            "https://jsonplaceholder.typicode.com/posts/1"
        };

        foreach (var endpoint in endpoints)
        {
            try
            {
                var result = await Curl.ExecuteAsync($"curl {endpoint}");

                Console.WriteLine($"✓ {endpoint}");
                Console.WriteLine($"  Status: {result.StatusCode}");
                Console.WriteLine($"  Size: {result.Body.Length} bytes");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ {endpoint}");
                Console.WriteLine($"  Error: {ex.Message}");
                Console.WriteLine();
            }
        }
    }
}
```

## 🎨 Variations

### Using Fluent API

```csharp
using CurlDotNet.Core;

// Instead of curl string
var result = await CurlRequestBuilder
    .Get("https://api.github.com/users/octocat")
    .WithHeader("Accept", "application/json")
    .WithHeader("User-Agent", "MyApp/1.0")
    .ExecuteAsync();
```

### GET with Timeout

```csharp
// Set a 10-second timeout
var result = await Curl.ExecuteAsync(@"
    curl --connect-timeout 10 \
         --max-time 30 \
         https://api.example.com
");
```

### GET with Redirect Following

```csharp
// Follow redirects automatically
var result = await Curl.ExecuteAsync(@"
    curl -L https://bit.ly/some-shortened-url
");
```

### GET and Save to File

```csharp
// Save directly to file
var result = await Curl.ExecuteAsync(@"
    curl -o data.json https://api.example.com/data
");

// Or save after fetching
var result = await Curl.ExecuteAsync("curl https://api.example.com/data");
result.SaveToFile("data.json");
```

## 🔧 Working with Different Response Types

### JSON Response

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");

// Method 1: Parse to dynamic
dynamic data = result.AsJsonDynamic();
Console.WriteLine(data.current_user_url);

// Method 2: Parse to strongly-typed object
var apiInfo = result.ParseJson<GitHubApiInfo>();
Console.WriteLine(apiInfo.CurrentUserUrl);
```

### XML Response

```csharp
var result = await Curl.ExecuteAsync("curl https://example.com/data.xml");
string xml = result.Body;

// Parse using your preferred XML library
var doc = System.Xml.Linq.XDocument.Parse(xml);
```

### Plain Text Response

```csharp
var result = await Curl.ExecuteAsync("curl https://example.com/readme.txt");
string text = result.Body;
Console.WriteLine(text);
```

### Binary Response

```csharp
var result = await Curl.ExecuteAsync("curl https://example.com/image.jpg");

// Save binary data
await File.WriteAllBytesAsync("image.jpg",
    System.Text.Encoding.UTF8.GetBytes(result.Body));
```

## 🐛 Troubleshooting

### Problem: "404 Not Found"

**Cause:** The URL doesn't exist or is wrong.

**Solution:**
```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

if (result.StatusCode == 404)
{
    Console.WriteLine("User not found. Check the username.");
}
```

For more details, see our [HTTP error troubleshooting guide](https://jacob-mellor.github.io/curl-dot-net/troubleshooting/common-issues.html#http-errors).

### Problem: "403 Forbidden"

**Cause:** You need authentication or don't have permission.

**Solution:**
```csharp
// Add authentication
var result = await Curl.ExecuteAsync(@"
    curl https://api.github.com/user \
      -H 'Authorization: Bearer YOUR_TOKEN'
");
```

For more details, see our [authentication troubleshooting guide](https://jacob-mellor.github.io/curl-dot-net/troubleshooting/common-issues.html#authentication-errors).

### Problem: Response is Empty

**Cause:** The endpoint might not return data, or there's an error.

**Solution:**
```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");

if (string.IsNullOrEmpty(result.Body))
{
    Console.WriteLine("Empty response received");
    Console.WriteLine($"Status: {result.StatusCode}");
    Console.WriteLine($"Headers: {string.Join(", ", result.Headers.Keys)}");
}
```

### Problem: Invalid JSON

**Cause:** The response isn't valid JSON.

**Solution:**
```csharp
try
{
    var data = result.ParseJson<MyType>();
}
catch (System.Text.Json.JsonException ex)
{
    Console.WriteLine("Response is not valid JSON:");
    Console.WriteLine(result.Body);
}
```

For more details, see our [JSON troubleshooting guide](https://jacob-mellor.github.io/curl-dot-net/troubleshooting/common-issues.html#json-parsing-errors).

### Problem: Timeout

**Cause:** Server is slow or not responding.

**Solution:**
```csharp
// Increase timeout
var result = await Curl.ExecuteAsync(@"
    curl --connect-timeout 30 \
         --max-time 60 \
         https://slow-api.example.com
");
```

For more details, see our [timeout troubleshooting guide](https://jacob-mellor.github.io/curl-dot-net/troubleshooting/common-issues.html#timeout-errors).

## 📊 Query Parameters Guide

### Building URLs with Parameters

```csharp
// Method 1: Manual string building
string url = $"https://api.example.com/search?q={searchTerm}&limit={limit}";

// Method 2: Uri.EscapeDataString for special characters
string safeSearch = Uri.EscapeDataString("C# programming");
string url = $"https://api.example.com/search?q={safeSearch}";

// Method 3: Using query builder
var queryParams = new Dictionary<string, string>
{
    ["q"] = "dotnet",
    ["page"] = "1",
    ["per_page"] = "20",
    ["sort"] = "stars"
};

string query = string.Join("&", queryParams.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
string url = $"https://api.example.com/search?{query}";
```

### Common Query Parameters

| Parameter | Purpose | Example |
|-----------|---------|---------|
| `q` | Search query | `?q=dotnet` |
| `page` | Pagination page number | `?page=2` |
| `per_page` / `limit` | Items per page | `?per_page=20` |
| `sort` | Sort order | `?sort=created` |
| `order` | Ascending/descending | `?order=desc` |
| `filter` | Filter results | `?filter=active` |

## 🎓 Best Practices

### 1. Always Check Status

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");

if (!result.IsSuccess)
{
    Console.WriteLine($"Request failed: {result.StatusCode}");
    return;
}

// Proceed with result.Body
```

### 2. Use Strong Types

```csharp
// Define your model
public class User
{
    public string Login { get; set; }
    public string Name { get; set; }
    public int PublicRepos { get; set; }
}

// Parse to strong type
var user = result.ParseJson<User>();
Console.WriteLine(user.Name); // IntelliSense works!
```

### 3. Handle Exceptions

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
    // Process result
}
catch (CurlDnsException ex)
{
    Console.WriteLine($"DNS error: {ex.Message}");
    // See: https://jacob-mellor.github.io/curl-dot-net/troubleshooting/common-issues.html#dns-errors
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Timeout: {ex.Message}");
    // See: https://jacob-mellor.github.io/curl-dot-net/troubleshooting/common-issues.html#timeout-errors
}
catch (CurlException ex)
{
    Console.WriteLine($"Curl error: {ex.Message}");
    // See: https://jacob-mellor.github.io/curl-dot-net/troubleshooting/
}
```

### 4. Set Appropriate Headers

```csharp
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com \
      -H 'Accept: application/json' \
      -H 'User-Agent: MyApp/1.0' \
      -H 'Accept-Language: en-US'
");
```

### 5. Use Environment Variables for Secrets

```csharp
// Never hardcode tokens!
string apiKey = Environment.GetEnvironmentVariable("API_KEY");

var result = await Curl.ExecuteAsync($@"
    curl https://api.example.com \
      -H 'Authorization: Bearer {apiKey}'
");
```

## 🚀 Next Steps

Now that you can make GET requests:

1. Learn to [Send JSON Data](send-json.md) with POST requests
2. Try [Downloading Files](download-file.md)
3. Explore [Handling Errors](handle-errors.md)
4. Build an [API Client](call-api.md)

## 📚 Related Recipes

- [Call an API](call-api.md) - Building a complete API client
- [Send JSON](send-json.md) - POST requests with JSON
- [Handle Errors](handle-errors.md) - Robust error handling
- [Download Files](download-file.md) - Saving files from URLs

## 🎓 Key Takeaways

- GET is for reading data (doesn't change anything)
- Simplest form: `Curl.ExecuteAsync(url)`
- Always check `result.IsSuccess` before using data
- Use query parameters for filtering and pagination
- Add headers for authentication and content negotiation
- Parse JSON with `ParseJson<T>()` or `AsJsonDynamic()`
- Handle exceptions for robust applications

## 📖 Quick Reference

```csharp
// Basic GET
var result = await Curl.ExecuteAsync("https://api.example.com");

// GET with headers
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com \
      -H 'Accept: application/json' \
      -H 'Authorization: Bearer token'
");

// GET with query parameters
var result = await Curl.ExecuteAsync(
    "curl 'https://api.example.com/search?q=test&limit=10'"
);

// Parse response
if (result.IsSuccess)
{
    var data = result.ParseJson<MyType>();
    // or
    dynamic data = result.AsJsonDynamic();
}
```

---

**Need help?** Check [Troubleshooting](../../troubleshooting/common-issues.md) | **Have questions?** Ask in [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
