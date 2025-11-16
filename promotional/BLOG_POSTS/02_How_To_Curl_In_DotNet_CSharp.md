---
title: "How to Use curl in .NET (C#)"
published: true
tags: ['dotnet', 'csharp', 'http', 'api', 'curl', 'rest', 'tutorial', 'beginner']
date: 2025-01-20
canonical_url: https://dev.to/jacob/how-to-curl-in-dotnet-csharp
---

# How to Use curl in .NET (C#)

If you've ever worked with APIs, you've probably seen curl commands in documentation. curl is the universal tool for making HTTP requests from the command line. But what if you want to use those same requests in your C# application? 

Traditional approaches require translating curl commands into `HttpClient` code, which can be tedious and error-prone. But there's a better way: CurlDotNet lets you paste curl commands directly into C# code.

## Installing CurlDotNet

First, add the NuGet package to your project:

```bash
dotnet add package CurlDotNet
```

Or via Package Manager Console in Visual Studio:

```powershell
Install-Package CurlDotNet
```

Or add it directly to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="CurlDotNet" Version="1.0.0" />
</ItemGroup>
```

## Basic Usage

The simplest way to use CurlDotNet is to paste a curl command as a string:

```csharp
using CurlDotNet;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // Simple GET request
        var result = await Curl.ExecuteAsync(
            "curl https://api.github.com/users/octocat"
        );
        
        Console.WriteLine($"Status: {result.StatusCode}");
        Console.WriteLine($"Body: {result.Body}");
    }
}
```

That's it! The curl command is executed and you get a `CurlResult` object back with the response.

## Working with Different HTTP Methods

### GET Requests

```csharp
var result = await Curl.ExecuteAsync(
    "curl https://jsonplaceholder.typicode.com/posts/1"
);

var post = result.ParseJson<Post>();
Console.WriteLine($"Title: {post.Title}");
```

### POST Requests

```csharp
var result = await Curl.ExecuteAsync(@"
  curl -X POST https://jsonplaceholder.typicode.com/posts \
    -H 'Content-Type: application/json' \
    -d '{
      \"title\": \"My Post\",
      \"body\": \"This is the content\",
      \"userId\": 1
    }'
");

if (result.IsSuccess)
{
    var newPost = result.ParseJson<Post>();
    Console.WriteLine($"Created post ID: {newPost.Id}");
}
```

### PUT Requests

```csharp
var result = await Curl.ExecuteAsync(@"
  curl -X PUT https://jsonplaceholder.typicode.com/posts/1 \
    -H 'Content-Type: application/json' \
    -d '{
      \"title\": \"Updated Title\",
      \"body\": \"Updated content\",
      \"userId\": 1
    }'
");
```

### DELETE Requests

```csharp
var result = await Curl.ExecuteAsync(
    "curl -X DELETE https://jsonplaceholder.typicode.com/posts/1"
);

if (result.StatusCode == 200)
{
    Console.WriteLine("Post deleted successfully");
}
```

## Adding Headers

Headers are added with the `-H` or `--header` flag:

```csharp
var result = await Curl.ExecuteAsync(@"
  curl https://api.github.com/user \
    -H 'Accept: application/vnd.github.v3+json' \
    -H 'Authorization: token YOUR_GITHUB_TOKEN' \
    -H 'User-Agent: MyApp/1.0'
");
```

## Authentication

### Basic Authentication

```csharp
var result = await Curl.ExecuteAsync(
    "curl -u username:password https://api.example.com/protected"
);
```

### Bearer Token

```csharp
var result = await Curl.ExecuteAsync(@"
  curl https://api.example.com/protected \
    -H 'Authorization: Bearer YOUR_TOKEN'
");
```

## Working with JSON

CurlDotNet makes JSON handling easy:

```csharp
// Send JSON
var result = await Curl.ExecuteAsync(@"
  curl -X POST https://api.example.com/users \
    -H 'Content-Type: application/json' \
    -d '{\""name\"":\""John\"",\""email\"":\""john@example.com\""}'
");

// Parse JSON response
var user = result.ParseJson<User>();

// Or use the dynamic API
dynamic dynamicUser = result.AsJsonDynamic();
Console.WriteLine(dynamicUser.name);
```

## File Operations

### Downloading Files

```csharp
// Download and save to file
var result = await Curl.ExecuteAsync(
    "curl -o image.png https://example.com/image.png"
);

// The file is saved automatically, but you can also access it in memory
Console.WriteLine($"Downloaded {result.BinaryData.Length} bytes");
```

### Uploading Files

```csharp
var result = await Curl.ExecuteAsync(@"
  curl -X POST https://api.example.com/upload \
    -F 'file=@/path/to/document.pdf' \
    -F 'description=My Document' \
    -H 'Authorization: Bearer YOUR_TOKEN'
");
```

## Handling Responses

The `CurlResult` object provides many useful properties and methods:

```csharp
var result = await Curl.ExecuteAsync(
    "curl https://api.example.com/data"
);

// Check if successful (200-299)
if (result.IsSuccess)
{
    // Get status code
    Console.WriteLine($"Status: {result.StatusCode}");
    
    // Get response body
    Console.WriteLine($"Body: {result.Body}");
    
    // Access headers
    var contentType = result.Headers["Content-Type"];
    
    // Parse JSON
    var data = result.ParseJson<DataModel>();
    
    // Save to file
    result.SaveToFile("output.json");
}

// Or use fluent chaining
var data = result
    .EnsureSuccess()           // Throws if not 200-299
    .SaveToFile("backup.json")
    .ParseJson<DataModel>();
```

## Error Handling

Handle different types of errors:

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://example.com/api");
}
catch (CurlDnsException ex)
{
    Console.WriteLine($"DNS error: {ex.Message}");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Timeout: {ex.Message}");
}
catch (CurlHttpException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.Message}");
}
catch (CurlException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

## Advanced Features

### Following Redirects

```csharp
var result = await Curl.ExecuteAsync(
    "curl -L https://bit.ly/example"  // -L follows redirects
);
```

### Setting Timeouts

```csharp
var result = await Curl.ExecuteAsync(
    "curl --max-time 30 https://slow-api.example.com"
);
```

### Verbose Output

```csharp
var result = await Curl.ExecuteAsync(
    "curl -v https://api.example.com"  // -v shows detailed output
);
```

### Custom User Agent

```csharp
var result = await Curl.ExecuteAsync(
    "curl -A 'MyApp/1.0' https://api.example.com"
);
```

## Using the Fluent Builder API

For programmatic request building, use the fluent API:

```csharp
var result = await CurlRequestBuilder
    .Post("https://api.example.com/users")
    .WithHeader("Content-Type", "application/json")
    .WithJson(new { name = "John", email = "john@example.com" })
    .WithTimeout(TimeSpan.FromSeconds(30))
    .FollowRedirects()
    .ExecuteAsync();
```

## Real-World Example: GitHub API

Here's a complete example working with GitHub's API:

```csharp
using CurlDotNet;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        
        // Get user information
        var userResult = await Curl.ExecuteAsync($@"
          curl https://api.github.com/user \
            -H 'Accept: application/vnd.github.v3+json' \
            -H 'Authorization: token {token}'
        ");
        
        var user = userResult.ParseJson<GitHubUser>();
        Console.WriteLine($"Logged in as: {user.Login}");
        Console.WriteLine($"Name: {user.Name}");
        Console.WriteLine($"Public repos: {user.PublicRepos}");
        
        // List repositories
        var reposResult = await Curl.ExecuteAsync($@"
          curl https://api.github.com/user/repos \
            -H 'Accept: application/vnd.github.v3+json' \
            -H 'Authorization: token {token}'
        ");
        
        var repos = reposResult.ParseJson<List<Repository>>();
        Console.WriteLine($"\nYou have {repos.Count} repositories:");
        foreach (var repo in repos)
        {
            Console.WriteLine($"  - {repo.FullName}");
        }
        
        // Create a new repository
        var createResult = await Curl.ExecuteAsync($@"
          curl -X POST https://api.github.com/user/repos \
            -H 'Accept: application/vnd.github.v3+json' \
            -H 'Authorization: token {token}' \
            -d '{{\""name\"":\""my-new-repo\"",\""private\"":true,\""description\"":\""Created with CurlDotNet\""}}'
        ");
        
        if (createResult.IsSuccess)
        {
            var newRepo = createResult.ParseJson<Repository>();
            Console.WriteLine($"\nCreated repository: {newRepo.FullName}");
        }
    }
}

public class GitHubUser
{
    public string Login { get; set; }
    public string Name { get; set; }
    public int PublicRepos { get; set; }
}

public class Repository
{
    public string Name { get; set; }
    public string FullName { get; set; }
    public bool Private { get; set; }
}
```

## Conclusion

CurlDotNet makes using curl commands in C# simple and natural. You can paste curl commands from documentation directly into your code without translation. This saves time, reduces errors, and makes API integration much easier.

Try it out in your next project - I think you'll find it as useful as I do!

---

**Learn More:**
- [CurlDotNet on GitHub](https://github.com/jacob/curl-dot-net)
- [NuGet Package](https://www.nuget.org/packages/CurlDotNet)
- [Documentation](https://github.com/jacob/curl-dot-net#readme)

