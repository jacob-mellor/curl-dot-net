---
title: "Parse JSON from a curl Response in C# (Beginner Guide)"
description: "Parse JSON from a curl response in C# with result.ParseJson<T>() and JsonDocument, and send JSON with curl -d, --json, PostJsonAsync or WithJson."
---

# Tutorial 7: Parse JSON from a curl Response in C#

To parse JSON from a curl response in C#, run the command with `Curl.ExecuteAsync` and call `result.ParseJson<T>()` on the `CurlResult`; to send JSON, use `curl -d` with a JSON body, `Curl.PostJsonAsync`, or the builder's `WithJson`. This tutorial covers receiving, parsing and sending JSON step by step.

## What is JSON?

JSON (JavaScript Object Notation) is a human-readable text format for structured data, and the default format for most modern APIs:

```json
{
  "name": "John Doe",
  "age": 30,
  "email": "john@example.com",
  "isActive": true,
  "scores": [95, 87, 92],
  "address": { "street": "123 Main St", "city": "Springfield" }
}
```

## Receiving JSON Data

### Basic JSON GET Request

The JSON text arrives in `result.Body`, exactly as curl would print it to the terminal.

```csharp
using CurlDotNet;
using CurlDotNet.Core;

var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

if (result.IsSuccess)
{
    Console.WriteLine(result.Body);
    // {"login":"octocat","id":583231,"public_repos":8,...}
}
```

### Parsing JSON to Objects with ParseJson

Define a class whose property names match the JSON keys, then call `ParseJson<T>()`. Matching is case-insensitive, so camelCase keys such as `login` bind to PascalCase properties such as `Login` with no attributes. (`ParseJson<T>()` uses `System.Text.Json` with `PropertyNameCaseInsensitive = true`, or Newtonsoft.Json on .NET Standard 2.0, which is case-insensitive too.) Keys that differ by more than case, such as snake_case `public_repos`, still need `[JsonPropertyName]`.

```csharp
using System.Text.Json.Serialization;

public class GitHubUser
{
    public string Login { get; set; }                                   // "login" binds without an attribute
    [JsonPropertyName("public_repos")] public int PublicRepos { get; set; }
    [JsonPropertyName("html_url")] public string HtmlUrl { get; set; }
}

var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");
var user = result.ParseJson<GitHubUser>();
Console.WriteLine($"{user.Login} has {user.PublicRepos} public repos");
```

`ParseJson<T>()` throws `InvalidOperationException` if the body is empty or is not valid JSON for `T` (the serializer's `JsonException` is the `InnerException`), so check `IsSuccess` first.

### Parsing Arrays

A JSON array maps to a `List<T>`.

```csharp
public class Repo
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("stargazers_count")] public int Stars { get; set; }
}

var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat/repos");
var repos = result.ParseJson<List<Repo>>();
foreach (var repo in repos)
    Console.WriteLine($"{repo.Name}: {repo.Stars} stars");
```

## Sending JSON Data

### POST with JSON (curl string)

This is the same command you would type in a terminal. The `--json` flag sets both `Content-Type` and `Accept` to `application/json` for you.

```csharp
var result = await Curl.ExecuteAsync(@"curl -X POST https://httpbin.org/post \
    -H 'Content-Type: application/json' \
    -d '{""name"":""Jane Smith"",""age"":25}'");

// Shorter: --json implies POST and sets the JSON headers
var same = await Curl.ExecuteAsync(@"curl --json '{""name"":""Jane Smith""}' https://httpbin.org/post");

Console.WriteLine(result.StatusCode);
```

### POST with JSON (from a C# object)

`Curl.PostJsonAsync` serializes any object for you. The builder's `WithJson` does the same and lets you add headers.

```csharp
var newUser = new { name = "Jane Smith", age = 25, email = "jane@example.com" };

// Convenience method
var result = await Curl.PostJsonAsync("https://httpbin.org/post", newUser);

// Builder, with an extra header
var result2 = await CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithJson(newUser)
    .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
    .ExecuteAsync();
```

### PUT and PATCH with JSON

```csharp
var updates = new { age = 26, email = "newemail@example.com" };

// curl string
var put = await Curl.ExecuteAsync(@"curl -X PUT https://httpbin.org/put \
    -H 'Content-Type: application/json' -d '{""age"":26}'");

// Builder: PUT replaces the resource, PATCH updates fields
var put2 = await CurlRequestBuilder.Put("https://httpbin.org/put").WithJson(updates).ExecuteAsync();
var patch = await CurlRequestBuilder.Patch("https://httpbin.org/patch").WithJson(updates).ExecuteAsync();
```

### Sending JSON from a File

Just like the curl command line, `-d @file` reads the body from disk, stripping carriage returns and newlines as curl does (use `--data-binary @file` to send the file byte for byte). A missing file throws `CurlReadErrorException` (curl exit code 26).

```csharp
var result = await Curl.ExecuteAsync(@"curl -X POST https://httpbin.org/post \
    -H 'Content-Type: application/json' -d @payload.json");
```

## Working with Complex JSON

### Nested Objects

Nested JSON objects become nested C# classes. Serialization and parsing work the same way at any depth.

```csharp
public class Address
{
    [JsonPropertyName("city")] public string City { get; set; }
}

public class Person
{
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("address")] public Address Address { get; set; }
}

var person = new Person { Name = "John", Address = new Address { City = "Springfield" } };
var result = await Curl.PostJsonAsync("https://httpbin.org/post", person);
```

### Reading Back What You Sent

httpbin echoes the request body under a `json` key, which makes it a handy way to test your serialization.

```csharp
public class HttpBinEcho
{
    [JsonPropertyName("json")] public Person Json { get; set; }
}

var echo = result.ParseJson<HttpBinEcho>();
Console.WriteLine(echo.Json.Address.City);   // Springfield
```

## JSON Serialization Options

### snake_case Keys Without Attributes

camelCase keys already bind without attributes. If the API uses snake_case and you would rather not annotate every property, deserialize `result.Body` yourself with `System.Text.Json` options (`JsonNamingPolicy.SnakeCaseLower` needs .NET 8 or later).

```csharp
using System.Text.Json;

var options = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true,
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower   // public_repos -> PublicRepos
};

var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");
var user = JsonSerializer.Deserialize<GitHubUser>(result.Body, options);
```

### Pretty Printing

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

using var doc = JsonDocument.Parse(result.Body);
var pretty = JsonSerializer.Serialize(doc.RootElement,
    new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(pretty);
```

## Error Handling with JSON

### Safe Parsing

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

if (!result.IsSuccess)
{
    Console.WriteLine($"HTTP {result.StatusCode}: not parsing");
    return;
}
try
{
    var user = result.ParseJson<GitHubUser>();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Invalid JSON: {ex.Message}. Raw body: {result.Body}");
}
```

### Check the Content-Type First

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/html");

var contentType = result.GetHeader("Content-Type") ?? "";
if (contentType.Contains("application/json"))
    Console.WriteLine("Safe to call ParseJson");
else
    Console.WriteLine($"Not JSON: {contentType}");
```

## Working with Dynamic JSON

### Using AsJsonDynamic

`AsJsonDynamic()` parses the body without a class, but what it returns depends on the build: on .NET 8+ and .NET Framework it is a `System.Text.Json.JsonDocument` (so `user.login` style member access does not work), and on .NET Standard 2.0 it is a Newtonsoft `JObject`. For portable code without a class, use `JsonDocument` directly, as below.

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

using var user = JsonDocument.Parse(result.Body);
Console.WriteLine(user.RootElement.GetProperty("login").GetString());
Console.WriteLine(user.RootElement.GetProperty("public_repos").GetInt32());
```

### Using JsonDocument

`JsonDocument` is the allocation-friendly way to walk unknown JSON structures.

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat/repos");

using var doc = JsonDocument.Parse(result.Body);
foreach (var repo in doc.RootElement.EnumerateArray())
{
    if (repo.TryGetProperty("name", out var name))
        Console.WriteLine(name.GetString());
}
```

## Common JSON Patterns

### Pagination

Many APIs page their results. Read the page, then follow the next link.

```csharp
public class Paged<T>
{
    [JsonPropertyName("items")] public List<T> Items { get; set; }
    [JsonPropertyName("total_count")] public int TotalCount { get; set; }
}

var result = await Curl.ExecuteAsync(
    "curl 'https://api.github.com/search/repositories?q=curl+language:csharp&per_page=10&page=1'");
var page = result.ParseJson<Paged<Repo>>();
Console.WriteLine($"{page.Items.Count} of {page.TotalCount} results");
```

### API Responses with Metadata

```csharp
public class ApiResponse<T>
{
    [JsonPropertyName("success")] public bool Success { get; set; }
    [JsonPropertyName("message")] public string Message { get; set; }
    [JsonPropertyName("data")] public T Data { get; set; }
}

var wrapped = result.ParseJson<ApiResponse<GitHubUser>>();
if (wrapped.Success)
    Console.WriteLine(wrapped.Data.Login);
else
    Console.WriteLine($"API error: {wrapped.Message}");
```

## Best Practices

1. Check `IsSuccess` (or call `EnsureSuccess()`) before parsing
2. Use strongly typed classes when the structure is known; add `[JsonPropertyName]` only for keys that differ by more than case
3. Catch `InvalidOperationException` from `ParseJson<T>()`
4. Use `JsonDocument` for unknown or one-off structures
5. Dispose `JsonDocument` after use
6. Log the raw `Body` when debugging parsing issues
7. Prefer `--json` or `PostJsonAsync` over hand-building JSON strings

## Summary

Working with JSON in CurlDotNet is straightforward:

- Receive JSON as text in `result.Body`
- Parse with `result.ParseJson<T>()` or `JsonDocument`
- Send JSON with `curl -d '{...}'`, `curl --json`, `Curl.PostJsonAsync()` or `WithJson()`
- Property matching is case-insensitive; map snake_case keys with `[JsonPropertyName]` or your own `JsonSerializerOptions`

## What's Next?

Learn about [HTTP headers](08-headers-explained.html) and how they control request and response behaviour.

See also: the [send JSON recipe](../cookbook/beginner/send-json.html) and [calling a REST API](../cookbook/beginner/call-api.html).

---

[Previous: Handling Errors](06-handling-errors.html) | [Next: Headers Explained](08-headers-explained.html)
