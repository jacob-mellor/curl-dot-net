---
title: "Understanding the CurlResult Response in C#"
description: "Read a CurlResult in C#: StatusCode, Body, IsSuccess, Headers, GetHeader, ParseJson and EnsureSuccess after running a curl command with CurlDotNet."
---

# Tutorial 5: Understanding the CurlResult Response in C#

Every CurlDotNet request returns a `CurlResult`, and understanding the CurlResult response in C# (its `StatusCode`, `Body`, `IsSuccess` and `Headers`) is the foundation for everything that follows in these tutorials. This page walks through each property and the patterns you will use most.

## The CurlResult Object

Run any curl command string and you get a `CurlResult` back. The same object comes back from the fluent `CurlRequestBuilder`.

```csharp
using CurlDotNet;
using CurlDotNet.Core;

CurlResult result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

// The builder returns the same type
CurlResult same = await CurlRequestBuilder.Get("https://httpbin.org/get").ExecuteAsync();
```

## Key Properties

### IsSuccess

`IsSuccess` is `true` when the server answered with a 2xx status code. HTTP errors such as 404 or 500 do not throw; you get a result with `IsSuccess == false`.

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/404");

if (result.IsSuccess)
{
    Console.WriteLine(result.Body);
}
else
{
    Console.WriteLine($"Request failed with HTTP {result.StatusCode}");
}
```

### Body

`Body` holds what curl would have printed to standard output, as a string. For a plain request that is the response body; for JSON APIs it is the raw JSON text you will parse in [Tutorial 7](07-json-for-beginners.html). Options change it the way they change curl's output: `-i` puts the response headers in front of the body, `-I` gives you only the headers, `-v` adds the request/response log, `-w` appends its text after the body, and `-o`/`-O` send the body to a file and leave `Body` empty.

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

string responseBody = result.Body;
Console.WriteLine(responseBody.Length + " characters received");
```

### StatusCode

`StatusCode` is a plain `int` holding the HTTP status code the server returned.

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

Console.WriteLine($"Server returned: {result.StatusCode}");

if (result.StatusCode == 404)
    Console.WriteLine("The requested resource was not found.");
else if (result.StatusCode == 401)
    Console.WriteLine("Authentication required.");
```

### Headers

`Headers` is a case-insensitive `Dictionary<string, string>` of response headers, with values exactly as the server sent them (for example `Content-Encoding` and `Content-Length` still describe the compressed body after `--compressed` has decoded it). `GetHeader` returns `null` instead of throwing when a header is missing, and `HasHeader` checks for presence.

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

string contentType = result.GetHeader("Content-Type");
Console.WriteLine($"Response type: {contentType}");

if (result.HasHeader("Date"))
    Console.WriteLine($"Server time: {result.GetHeader("date")}");

foreach (var header in result.Headers)
    Console.WriteLine($"{header.Key}: {header.Value}");
```

### What About Network Errors?

There is no error string on the result. Network-level failures (DNS, connection refused, timeouts) never produce a `CurlResult`; they throw an exception derived from `CurlException`, carrying curl's exit code, which [Tutorial 6](06-handling-errors.html) covers in detail. The same happens for HTTP errors when you add `-f`/`--fail` (`CurlHttpReturnedErrorException`, exit 22).

```csharp
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -m 5 https://no-such-host.invalid/");
}
catch (CurlException ex)
{
    Console.WriteLine($"Request never completed: {ex.Message}");
}
```

## Common Patterns

### Pattern 1: Simple Success Check

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

if (result.IsSuccess)
    Console.WriteLine(result.Body);
else
    Console.WriteLine($"HTTP {result.StatusCode}");
```

### Pattern 2: Detailed Status Handling

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/repos/dotnet/runtime");

switch (result.StatusCode)
{
    case 200: Console.WriteLine(result.Body); break;
    case 304: Console.WriteLine("Content has not changed"); break;
    case 404: Console.WriteLine("Resource not found"); break;
    case 401: Console.WriteLine("Please authenticate"); break;
    default:  Console.WriteLine($"Unexpected status: {result.StatusCode}"); break;
}
```

### Pattern 3: Throw on Failure with EnsureSuccess

`EnsureSuccess()` throws `CurlDotNet.Core.CurlHttpException` for any non-2xx status and returns the result otherwise, so it chains nicely.

```csharp
try
{
    var body = (await Curl.ExecuteAsync("curl https://httpbin.org/status/500"))
        .EnsureSuccess()
        .Body;
}
catch (CurlDotNet.Core.CurlHttpException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.ResponseBody}");
}
```

### Pattern 4: Working with Headers

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/rate_limit");

string contentType = result.GetHeader("Content-Type") ?? "";
if (contentType.Contains("application/json"))
    Console.WriteLine("JSON response, safe to parse");

string remaining = result.GetHeader("X-RateLimit-Remaining");
if (remaining != null)
    Console.WriteLine($"API calls remaining: {remaining}");
```

### Pattern 5: Response Time Tracking

`Timings` exposes the same phases curl reports with `-w`, in milliseconds.

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

Console.WriteLine($"DNS lookup:  {result.Timings.NameLookup} ms");
Console.WriteLine($"Connect:     {result.Timings.Connect} ms");
Console.WriteLine($"First byte:  {result.Timings.StartTransfer} ms");
Console.WriteLine($"Total:       {result.Timings.Total} ms");
```

## Working with Different Content Types

### JSON Responses

`ParseJson<T>()` deserializes `Body` into your class. Property matching is case-insensitive, so `login` binds to `Login`; snake_case keys such as `public_repos` need `[JsonPropertyName]`. It throws `InvalidOperationException` when the body is empty or not valid JSON for that type.

```csharp
using System.Text.Json.Serialization;

public class GitHubUser
{
    public string Login { get; set; }
    [JsonPropertyName("public_repos")] public int PublicRepos { get; set; }
}

var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");
var user = result.ParseJson<GitHubUser>();
Console.WriteLine($"{user.Login} has {user.PublicRepos} public repos");
```

### XML Responses

```csharp
using System.Xml.Linq;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/xml");
var doc = XDocument.Parse(result.Body);
Console.WriteLine(doc.Root.Name);
```

### Binary Data

When the Content-Type is not text, `IsBinary` is `true` and the bytes are in `BinaryData`. `SaveToFile` writes either text or binary content.

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/image/png");

if (result.IsBinary)
{
    Console.WriteLine($"{result.BinaryData.Length} bytes");
    result.SaveToFile("image.png");
}
```

## Best Practices

### 1. Check IsSuccess Before Using Body

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/404");

// Good
if (result.IsSuccess)
    Console.WriteLine(result.Body);

// Risky: Body may be an HTML error page, not the data you expected
Console.WriteLine(result.Body);
```

### 2. Log Both Success and Failure

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

if (result.IsSuccess)
    logger.LogInformation("GET succeeded in {Ms} ms", result.Timings.Total);
else
    logger.LogError("GET failed with HTTP {Status}", result.StatusCode);
```

### 3. Use Status Codes for Business Logic

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/no-such-user-xyz");

if (result.StatusCode == 404)
    return null;              // expected: the user does not exist

return result.EnsureSuccess().ParseJson<GitHubUser>();
```

### 4. Prefer the Built-in Helpers

`ParseJson<T>()`, `GetHeader()`, `EnsureSuccess()` and `SaveToFile()` already cover the common cases, so you rarely need your own extension methods on `CurlResult`.

## Common Mistakes to Avoid

### Mistake 1: Parsing Without Checking the Status

```csharp
// Wrong: a 500 error page is not JSON
var data = result.ParseJson<GitHubUser>();

// Right
if (result.IsSuccess)
{
    var data = result.ParseJson<GitHubUser>();
}
```

### Mistake 2: Treating All Failures the Same

```csharp
if (result.StatusCode == 429)
{
    // Too many requests: wait and try again later
}
else if (result.StatusCode == 401)
{
    // Refresh credentials
}
else if (!result.IsSuccess)
{
    throw new InvalidOperationException($"HTTP {result.StatusCode}");
}
```

### Mistake 3: Indexing Headers Directly

```csharp
// Wrong: throws KeyNotFoundException when the header is absent
var etag = result.Headers["ETag"];

// Right: returns null when absent
var etag = result.GetHeader("ETag");
```

## Summary

The `CurlResult` object gives you everything you need to handle an HTTP response:

- Check `IsSuccess` to see whether the server returned 2xx
- Read the response text from `Body`, or bytes from `BinaryData`
- Use `StatusCode` (an `int`) for specific HTTP status handling
- Read response headers with `GetHeader` and `HasHeader`
- Call `EnsureSuccess()` when a non-2xx status should throw
- Parse JSON with `ParseJson<T>()`
- Network failures throw a `CurlException` instead of returning a result

## What's Next?

In the next tutorial, we look at [handling errors](06-handling-errors.html): the exceptions CurlDotNet throws for network failures and how to retry safely.

See also: [CurlResult in the API guide](../api-guide/README.html) and the [simple GET recipe](../cookbook/beginner/simple-get.html).

---

[Previous: Your First Request](04-your-first-request.html) | [Next: Handling Errors](06-handling-errors.html)
