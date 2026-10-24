---
title: "Your First curl Request in C# with CurlDotNet"
description: "Make your first curl request in C# with CurlDotNet: run Curl.ExecuteAsync, read the status code, headers and body, and fix the most common beginner errors."
---

# Tutorial 4: Your First Request

Your first curl request in C# with CurlDotNet takes one line: `await Curl.ExecuteAsync("curl https://api.github.com")`. This tutorial walks through creating the project, running that request, reading the `CurlResult` it returns, understanding HTTP status codes, and fixing the problems beginners hit most often.

## What You'll Learn

- How to make your first HTTP request with CurlDotNet
- How to examine the response
- How to check whether the request succeeded
- How to troubleshoot basic issues

## Prerequisites

- [Tutorial 1: What is .NET and C#?](01-what-is-dotnet.html)
- [Tutorial 2: What is curl?](02-what-is-curl.html)
- [Tutorial 3: Understanding Async/Await](03-what-is-async.html)
- .NET SDK installed
- CurlDotNet package added to your project

## Setting Up Your Project

If you have not created a project yet:

```bash
# Create a new directory
mkdir MyFirstCurlRequest
cd MyFirstCurlRequest

# Create a new console application
dotnet new console

# Add the CurlDotNet package
dotnet add package CurlDotNet

# Open in your editor
code .
```

## Your First Request

### Step 1: The Simplest Request

Replace the contents of `Program.cs` with:

```csharp
using System;
using CurlDotNet;

Console.WriteLine("Making my first request...");

// Make the request
var result = await Curl.ExecuteAsync("curl https://api.github.com");

// Show what we got back
Console.WriteLine($"Status Code: {result.StatusCode}");
Console.WriteLine($"Success: {result.IsSuccess}");
Console.WriteLine($"Response Length: {result.Body.Length} characters");
```

(This uses C# top-level statements, so no `class Program` or `Main` method is needed.)

### Step 2: Run It

```bash
dotnet run
```

You should see output like:

```text
Making my first request...
Status Code: 200
Success: True
Response Length: 2000 characters
```

You just made your first HTTP request with CurlDotNet.

## Understanding What Happened

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");
```

1. **`Curl.ExecuteAsync`** is the main method for running a curl command
2. **`"curl https://api.github.com"`** is the curl command (the leading `curl` is optional)
3. **`await`** waits for the request to complete without freezing the program
4. **`result`** is a `CurlResult` that contains the response

### The Result Object

| Property or method | What It Contains | Example |
|--------------------|------------------|---------|
| `StatusCode` | HTTP status code | `200` |
| `IsSuccess` | `true` for 2xx status codes | `true` or `false` |
| `Body` | Response content as text | `{"message":"Hello"}` |
| `Headers` | Response headers (case-insensitive dictionary) | `Content-Type`, `Date`, ... |
| `GetHeader("Content-Type")` | One header value, or `null` if absent | `"application/json; charset=utf-8"` |
| `Timings.Total` | Total request time in milliseconds | `312.5` |

`CurlResult` lives in the `CurlDotNet.Core` namespace; add `using CurlDotNet.Core;` when you declare a variable of that type. [Tutorial 5](05-understanding-results.html) covers the result object in depth.

## More Detailed Example

Looking at the response more carefully:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");

if (!result.IsSuccess)
{
    Console.WriteLine($"Request failed with status {result.StatusCode}");
    return;
}

Console.WriteLine($"Content-Type: {result.GetHeader("Content-Type")}");

foreach (var header in result.Headers)
    Console.WriteLine($"  {header.Key}: {header.Value}");

Console.WriteLine(result.Body);
```

## Different Ways to Make Requests

CurlDotNet gives you three ways to make the same request.

### Method 1: Full curl Command

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");
```

**Use when:** you have a curl command from API documentation.

### Method 2: Convenience Method

```csharp
var result = await Curl.GetAsync("https://api.github.com");
```

**Use when:** you just need a simple GET. `Curl.PostAsync`, `Curl.PostJsonAsync` and `Curl.DownloadAsync` work the same way.

### Method 3: Fluent Builder

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://api.github.com")
    .WithHeader("Accept", "application/vnd.github+json")
    .ExecuteAsync();
```

**Use when:** you want IntelliSense while composing the request.

All three return the same `CurlResult`.

## Practical Examples

### Example 1: Read JSON from a Public API

```csharp
using System;
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

if (result.IsSuccess)
{
    dynamic user = result.AsJsonDynamic();
    Console.WriteLine($"User: {user.login}");
    Console.WriteLine($"Public repos: {user.public_repos}");
}
```

### Example 2: Check Website Status

```csharp
string[] websites = { "https://github.com", "https://httpbin.org/status/200", "https://httpbin.org/status/503" };

foreach (var website in websites)
{
    var result = await Curl.ExecuteAsync($"curl -L {website}");

    var status = result.IsSuccess ? "UP" : "DOWN";
    Console.WriteLine($"{status} - {website} ({result.StatusCode})");
}
```

`-L` follows redirects, so `https://github.com` lands on its final page instead of reporting a 301.

### Example 3: API with Custom Headers

```csharp
var result = await Curl.ExecuteAsync(@"
    curl https://api.github.com/users/octocat \
      -H 'Accept: application/vnd.github+json' \
      -H 'User-Agent: MyApp/1.0'
");

if (result.IsSuccess)
{
    dynamic user = result.AsJsonDynamic();
    Console.WriteLine($"Name: {user.name}, Followers: {user.followers}");
}
```

## Understanding Status Codes

HTTP status codes tell you what happened with your request.

### Success Codes (2xx)

| Code | Meaning | What It Means |
|------|---------|---------------|
| 200 | OK | You got what you asked for |
| 201 | Created | Something new was created |
| 204 | No Content | Success, but no data to return |

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
| 401 | Unauthorized | You need to authenticate first |
| 403 | Forbidden | You do not have permission |
| 404 | Not Found | The resource does not exist |
| 429 | Too Many Requests | You are making requests too quickly |

### Server Error Codes (5xx)

| Code | Meaning | What It Means |
|------|---------|---------------|
| 500 | Internal Server Error | The server hit an error |
| 502 | Bad Gateway | The server could not reach another server |
| 503 | Service Unavailable | The server is overloaded or down |

### Checking Status in Code

A 4xx or 5xx response does not throw an exception; you get a result with `IsSuccess == false` and the status code:

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/404");

if (result.StatusCode == 200)
    Console.WriteLine("Perfect! Everything worked.");
else if (result.StatusCode == 404)
    Console.WriteLine("Not found - check your URL.");
else if (result.StatusCode >= 500)
    Console.WriteLine("Server error - try again later.");
else
    Console.WriteLine($"Something went wrong: {result.StatusCode}");
```

If you prefer an exception for error status codes, call `result.EnsureSuccess()`, which throws `CurlDotNet.Core.CurlHttpException`. See [Tutorial 6: Handling Errors](06-handling-errors.html).

## Try It Yourself

### Exercise 1: Fetch Your Own GitHub Profile

Replace `YOUR_USERNAME` with your GitHub username:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/YOUR_USERNAME");
```

### Exercise 2: Check Multiple APIs

Write a program that checks whether these endpoints respond with 200:

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
Console.WriteLine($"Content-Type: {result.GetHeader("Content-Type")}");
Console.WriteLine($"Contains 'API': {result.Body.Contains("API")}");
```

</details>

## Common Problems and Solutions

### Problem 1: Could Not Connect or Resolve the Host

**Exception:**

```text
CurlCouldntConnectException: Failed to connect to api.examplee.com:443
```

**Cause:** a typo in the host name, no internet connection, or a server that is not listening.

**Solution:**

1. Check the URL for typos
2. Make sure you have an internet connection
3. Try the URL in your web browser first

See [DNS and connection errors](../exceptions/dns-errors.html).

### Problem 2: Timeout

**Exception:**

```text
CurlOperationTimeoutException: Operation timed out after 5 seconds
```

**Cause:** you set `-m`/`--max-time` and the server did not finish in time.

**Solution:** try again, or allow more time:

```csharp
var result = await Curl.ExecuteAsync("curl --max-time 30 https://httpbin.org/delay/3");
```

See [timeout errors](../exceptions/timeout-errors.html).

### Problem 3: 404 Not Found

**Output:**

```text
Status Code: 404
```

**Cause:** the URL does not exist or is wrong. No exception is thrown; check `result.IsSuccess`.

**Solution:** double-check the URL against the API documentation and try it in your browser. See [HTTP errors](../exceptions/http-errors.html).

### Problem 4: SSL Certificate Error

**Exception:**

```text
CurlCouldntConnectException: Failed to connect to self-signed.badssl.com:443
```

**Cause:** the TLS handshake failed, usually because the server's certificate cannot be verified.

**Solution for development only:**

```csharp
// WARNING: -k disables certificate verification. Development only.
var result = await Curl.ExecuteAsync("curl -k https://self-signed.badssl.com/");
```

**Solution for production:** fix the server certificate or install the issuing CA in the machine's trust store. See [SSL errors](../exceptions/ssl-errors.html).

### Problem 5: Forgot 'await'

**Compiler error:**

```text
Cannot implicitly convert type 'Task<CurlResult>' to 'CurlResult'
```

**Cause:** you forgot the `await` keyword.

```csharp
// Wrong
CurlResult result = Curl.ExecuteAsync("curl https://api.github.com");

// Right
CurlResult result = await Curl.ExecuteAsync("curl https://api.github.com");
```

## Debugging Your Request

### Using Verbose Mode

```csharp
var result = await Curl.ExecuteAsync("curl -v https://api.github.com");
```

With `-v`, the request line, request headers and response headers are written at the start of `result.Body`, the same information curl prints with `-v`. See [Tutorial 14: Debugging Requests](14-debugging-requests.html).

### Saving the Response

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");
result.SaveToFile("response.json");
Console.WriteLine("Response saved to response.json");
```

### Checking Headers

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");

Console.WriteLine($"Rate limit remaining: {result.GetHeader("X-RateLimit-Remaining")}");

foreach (var header in result.Headers)
    Console.WriteLine($"{header.Key}: {header.Value}");
```

## Quick Reference

### Basic Request Pattern

```csharp
using System;
using CurlDotNet;

// 1. Make the request
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

// 2. Check whether it succeeded
if (result.IsSuccess)
    Console.WriteLine(result.Body);        // 3. Use the response
else
    Console.WriteLine($"Error: {result.StatusCode}");  // 4. Handle errors
```

### Common Properties

```csharp
// Status information
result.StatusCode                   // e.g. 200
result.IsSuccess                    // true if 200-299

// Content
result.Body                         // response as string
result.GetHeader("Content-Type")    // e.g. "application/json"

// Headers
result.Headers                      // dictionary of all headers
result.HasHeader("ETag")            // true if the header is present

// Timing (milliseconds)
result.Timings.Total
```

## Key Takeaways

- Making a request is one line: `await Curl.ExecuteAsync("curl https://...")`
- Always use `await` when calling async methods
- Check `result.IsSuccess` before using the response
- Status codes tell you what happened; 4xx/5xx do not throw
- The response body is in `result.Body`
- Use verbose mode (`-v`) to debug issues

## Questions You Might Have

**Q: Why do I need `await`?**
A: HTTP requests take time. `await` lets your program wait without freezing. See [Tutorial 3](03-what-is-async.html).

**Q: What if the request fails?**
A: Check `result.IsSuccess`, then `result.StatusCode`. Network failures (no connection, timeout) throw a `CurlException` subclass; see [Tutorial 6](06-handling-errors.html).

**Q: Can I make multiple requests at once?**
A: Yes. `Curl.ExecuteManyAsync(...)` runs several commands in parallel. See [Tutorial 13](13-parallel-requests.html).

**Q: How do I send data with my request?**
A: Use `-d`:

```csharp
var result = await Curl.ExecuteAsync("curl -X POST -d 'key=value' https://httpbin.org/post");
```

[Tutorial 11: Forms and Data](11-forms-and-data.html) covers this in detail.

**Q: Is this free to use?**
A: Yes. CurlDotNet is open source.

## Summary

You have made your first HTTP request with CurlDotNet. You can now execute GET requests, examine the response, check status codes, handle success and errors, and debug issues. This is the foundation for everything else in the series.

**Next tutorial:** [Understanding Results](05-understanding-results.html)

**Need help?** Check the [troubleshooting guide](../troubleshooting/common-issues.html) or ask in [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions).
