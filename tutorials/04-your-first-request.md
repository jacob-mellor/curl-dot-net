# Tutorial 4: Your First Request

## 🎯 What You'll Learn

- How to make your first HTTP request with CurlDotNet
- How to examine the response
- How to check if the request succeeded
- How to troubleshoot basic issues

## 📚 Prerequisites

- [Tutorial 1: What is .NET and C#?](01-what-is-dotnet.html)
- [Tutorial 2: What is curl?](02-what-is-curl.html)
- [Tutorial 3: Understanding Async/Await](03-what-is-async.html)
- .NET SDK installed
- CurlDotNet package added to your project

## 🚀 Setting Up Your Project

If you haven't created a project yet, let's do that now:

```bash
# Create a new directory
mkdir MyFirstCurlRequest
cd MyFirstCurlRequest

# Create a new console application
dotnet new console

# Add CurlDotNet package
dotnet add package CurlDotNet

# Open in your editor
code .
```

## 💻 Your First Request

Let's start with the simplest possible request - fetching a webpage:

### Step 1: The Simplest Request

Replace the contents of `Program.cs` with this code:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Making my first request...");

        // Make the request
        var result = await Curl.ExecuteAsync("curl https://api.github.com");

        // Show what we got back
        Console.WriteLine($"Status Code: {result.StatusCode}");
        Console.WriteLine($"Success: {result.IsSuccess}");
        Console.WriteLine($"Response Length: {result.Body.Length} characters");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
```

### Step 2: Run It

```bash
dotnet run
```

You should see output like:

```
Making my first request...
Status Code: 200
Success: True
Response Length: 1547 characters

Press any key to exit...
```

**Congratulations!** You just made your first HTTP request with CurlDotNet!

## 🔍 Understanding What Happened

Let's break down what the code does:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");
```

1. **`Curl.ExecuteAsync`** - The main method for making requests
2. **`"curl https://api.github.com"`** - The curl command (you can also omit "curl" and just use the URL)
3. **`await`** - Waits for the request to complete
4. **`result`** - Contains the response

### The Result Object

The `result` variable contains everything about the response:

| Property | What It Contains | Example |
|----------|------------------|---------|
| `StatusCode` | HTTP status code | `200` (success) |
| `IsSuccess` | Was it successful? | `true` or `false` |
| `Body` | Response content | `"{"message":"Hello"}"` |
| `Headers` | Response headers | `Content-Type`, `Date`, etc. |
| `ContentType` | Type of content | `"application/json"` |

## 📖 More Detailed Example

Let's look at the response more carefully:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Fetching GitHub API root endpoint...\n");

        var result = await Curl.ExecuteAsync("curl https://api.github.com");

        // Check if successful
        if (result.IsSuccess)
        {
            Console.WriteLine("✓ Request succeeded!");
            Console.WriteLine($"✓ Status Code: {result.StatusCode}");
            Console.WriteLine($"✓ Content Type: {result.ContentType}");
            Console.WriteLine();

            // Show some headers
            Console.WriteLine("Response Headers:");
            foreach (var header in result.Headers)
            {
                Console.WriteLine($"  {header.Key}: {header.Value}");
            }
            Console.WriteLine();

            // Show the response body
            Console.WriteLine("Response Body:");
            Console.WriteLine(result.Body);
        }
        else
        {
            Console.WriteLine("✗ Request failed!");
            Console.WriteLine($"✗ Status Code: {result.StatusCode}");
        }
    }
}
```

## 🎨 Different Ways to Make Requests

CurlDotNet gives you three ways to make the same request:

### Method 1: Full curl Command

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");
```

**Use when:** You have a curl command from API documentation.

### Method 2: Just the URL

```csharp
var result = await Curl.ExecuteAsync("https://api.github.com");
```

**Use when:** You just need a simple GET request.

### Method 3: Fluent Builder API

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://api.github.com")
    .ExecuteAsync();
```

**Use when:** You want IntelliSense and type safety.

All three do exactly the same thing!

## 🧪 Practical Examples

### Example 1: Get Weather Data

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class WeatherApp
{
    static async Task Main()
    {
        Console.WriteLine("Fetching weather data...\n");

        // Public weather API (no key needed)
        var result = await Curl.ExecuteAsync(
            "curl https://wttr.in/London?format=j1"
        );

        if (result.IsSuccess)
        {
            Console.WriteLine("Weather data received!");
            // The response is JSON with weather information
            var weather = result.ParseJson<dynamic>();
            Console.WriteLine($"Temperature: {weather.current_condition[0].temp_C}°C");
        }
    }
}
```

### Example 2: Check Website Status

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class StatusChecker
{
    static async Task Main()
    {
        string[] websites = {
            "https://github.com",
            "https://google.com",
            "https://stackoverflow.com"
        };

        Console.WriteLine("Checking website status...\n");

        foreach (var website in websites)
        {
            var result = await Curl.ExecuteAsync($"curl {website}");

            var status = result.IsSuccess ? "✓ UP" : "✗ DOWN";
            Console.WriteLine($"{status} - {website} ({result.StatusCode})");
        }
    }
}
```

### Example 3: API with Custom Headers

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class ApiClient
{
    static async Task Main()
    {
        Console.WriteLine("Calling API with custom headers...\n");

        var result = await Curl.ExecuteAsync(@"
            curl https://api.github.com/users/octocat \
              -H 'Accept: application/vnd.github.v3+json' \
              -H 'User-Agent: MyApp/1.0'
        ");

        if (result.IsSuccess)
        {
            dynamic user = result.AsJsonDynamic();
            Console.WriteLine($"User: {user.login}");
            Console.WriteLine($"Name: {user.name}");
            Console.WriteLine($"Public Repos: {user.public_repos}");
            Console.WriteLine($"Followers: {user.followers}");
        }
    }
}
```

## 🔧 Understanding Status Codes

HTTP status codes tell you what happened with your request:

### Success Codes (2xx)

| Code | Meaning | What It Means |
|------|---------|---------------|
| 200 | OK | Success! You got what you asked for |
| 201 | Created | Success! Something new was created |
| 204 | No Content | Success! But no data to return |

### Redirect Codes (3xx)

| Code | Meaning | What It Means |
|------|---------|---------------|
| 301 | Moved Permanently | The resource moved to a new URL |
| 302 | Found | Temporary redirect to another URL |
| 304 | Not Modified | You already have the latest version |

### Client Error Codes (4xx)

| Code | Meaning | What It Means |
|------|---------|---------------|
| 400 | Bad Request | Your request has invalid syntax |
| 401 | Unauthorized | You need to log in first |
| 403 | Forbidden | You don't have permission |
| 404 | Not Found | The resource doesn't exist |
| 429 | Too Many Requests | You're making requests too quickly |

### Server Error Codes (5xx)

| Code | Meaning | What It Means |
|------|---------|---------------|
| 500 | Internal Server Error | The server crashed |
| 502 | Bad Gateway | The server couldn't reach another server |
| 503 | Service Unavailable | The server is overloaded or down |

### Checking Status in Code

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");

if (result.StatusCode == 200)
{
    Console.WriteLine("Perfect! Everything worked.");
}
else if (result.StatusCode == 404)
{
    Console.WriteLine("Not found - check your URL.");
}
else if (result.StatusCode >= 500)
{
    Console.WriteLine("Server error - try again later.");
}
else
{
    Console.WriteLine($"Something went wrong: {result.StatusCode}");
}
```

## 🎯 Try It Yourself

### Exercise 1: Your First Successful Request

Modify the code to fetch your own GitHub profile (replace "octocat" with your username):

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/YOUR_USERNAME");
```

### Exercise 2: Check Multiple APIs

Create a program that checks if these APIs are responding:
- `https://api.github.com`
- `https://httpbin.org/status/200`
- `https://jsonplaceholder.typicode.com/posts/1`

### Exercise 3: Display Response Details

Modify the basic example to show:
- How long the response body is
- What Content-Type the server sent back
- Whether the response contains the word "API"

<details>
<summary>Click for Solution to Exercise 3</summary>

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");

Console.WriteLine($"Response Length: {result.Body.Length} characters");
Console.WriteLine($"Content-Type: {result.ContentType}");
Console.WriteLine($"Contains 'API': {result.Body.Contains("API")}");
```

</details>

## ❌ Common Problems and Solutions

### Problem 1: "Could not resolve host"

**Error message:**
```
CurlDnsException: Could not resolve host: api.examplee.com
```

**Cause:** Typo in the URL or no internet connection.

**Solution:**
1. Check the URL for typos
2. Make sure you have internet connection
3. Try the URL in your web browser first

For more details, see our [DNS troubleshooting guide](../troubleshooting/common-issues.md#dns-errors).

### Problem 2: "Timeout"

**Error message:**
```
CurlTimeoutException: Operation timeout
```

**Cause:** The server is too slow or not responding.

**Solution:**
1. Try again - might be temporary
2. Increase the timeout:
```csharp
var result = await Curl.ExecuteAsync(
    "curl --connect-timeout 30 https://api.example.com"
);
```

For more details, see our [timeout troubleshooting guide](../troubleshooting/common-issues.md#timeout-errors).

### Problem 3: "404 Not Found"

**Error message:**
```
Status Code: 404
```

**Cause:** The URL doesn't exist or is wrong.

**Solution:**
1. Double-check the URL
2. Check the API documentation
3. Try the URL in your browser

For more details, see our [HTTP error guide](../troubleshooting/common-issues.md#http-errors).

### Problem 4: SSL Certificate Error

**Error message:**
```
CurlSslException: SSL certificate problem
```

**Cause:** The server's SSL certificate can't be verified.

**Solution for development only:**
```csharp
// WARNING: Only use in development!
var result = await Curl.ExecuteAsync("curl -k https://api.example.com");
```

**Solution for production:**
Update your system's certificate store or use proper certificates.

For more details, see our [SSL troubleshooting guide](../troubleshooting/common-issues.md#ssl-errors).

### Problem 5: Forgot 'await'

**Error message:**
```
Cannot implicitly convert type 'Task<CurlResult>' to 'CurlResult'
```

**Cause:** You forgot the `await` keyword.

**Wrong:**
```csharp
var result = Curl.ExecuteAsync("curl https://api.github.com");
```

**Right:**
```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");
```

## 🔍 Debugging Your Request

### Using Verbose Mode

See exactly what's happening:

```csharp
var result = await Curl.ExecuteAsync("curl -v https://api.github.com");
```

This shows:
- Connection details
- Headers sent
- Headers received
- Timing information

### Saving the Response

Save the response to a file for inspection:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");
result.SaveToFile("response.json");
Console.WriteLine("Response saved to response.json");
```

### Checking Headers

See what headers the server sent:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");

Console.WriteLine("Response Headers:");
foreach (var header in result.Headers)
{
    Console.WriteLine($"{header.Key}: {header.Value}");
}
```

## 📊 Quick Reference

### Basic Request Pattern

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main(string[] args)
    {
        // 1. Make the request
        var result = await Curl.ExecuteAsync("curl https://api.example.com");

        // 2. Check if successful
        if (result.IsSuccess)
        {
            // 3. Use the response
            Console.WriteLine(result.Body);
        }
        else
        {
            // 4. Handle errors
            Console.WriteLine($"Error: {result.StatusCode}");
        }
    }
}
```

### Common Properties

```csharp
// Status information
result.StatusCode         // e.g., 200
result.IsSuccess          // true if 200-299
result.IsSuccessStatusCode // same as IsSuccess

// Content
result.Body               // Response as string
result.ContentType        // e.g., "application/json"

// Headers
result.Headers            // Dictionary of all headers
result.GetHeader("X-Custom-Header")  // Get specific header

// Timing
result.TotalTime          // How long the request took
```

## 🚀 Next Steps

Now that you can make requests:

1. **Next Tutorial** → [Understanding Results](05-understanding-results.html)
2. **Try different APIs** - [Public APIs List](https://github.com/public-apis/public-apis)
3. **Practice** - Make 10 different requests to different endpoints
4. **Experiment** - What happens if you change the URL? Add headers?

## 🎓 Key Takeaways

- Making a request is simple: `Curl.ExecuteAsync(url)`
- Always use `await` when calling async methods
- Check `result.IsSuccess` before using the response
- Status codes tell you what happened
- The response body is in `result.Body`
- Use verbose mode (`-v`) to debug issues

## 🤔 Questions You Might Have

**Q: Why do I need `await`?**
A: HTTP requests take time. `await` tells C# to wait for the request to complete without freezing your program. See [Tutorial 3](03-what-is-async.html) for details.

**Q: What if the request fails?**
A: Check `result.IsSuccess`. If it's `false`, look at `result.StatusCode` to see what went wrong.

**Q: Can I make multiple requests at once?**
A: Yes! You can use `Curl.ExecuteMany()` to run multiple requests in parallel. Check the [API Guide](../api-guide/README.html) for details.

**Q: How do I send data with my request?**
A: We'll cover POST requests in detail in later tutorials. For now, you can use:
```csharp
var result = await Curl.ExecuteAsync(
    "curl -X POST -d 'key=value' https://api.example.com"
);
```

**Q: Is this free to use?**
A: Yes! CurlDotNet is open source and completely free.

## 📚 Summary

You've learned how to make your first HTTP request with CurlDotNet! You can:
- Execute simple GET requests
- Examine the response
- Check status codes
- Handle success and errors
- Debug issues

This is the foundation for everything else you'll do with CurlDotNet.

---

**Ready for more?** → [Check the Tutorials](README.html)

**Need help?** Check the [Troubleshooting Guide](../troubleshooting/README.html) or ask in [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
