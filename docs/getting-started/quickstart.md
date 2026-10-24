---
title: "CurlDotNet Quick Start - Run curl in C# in 2 Minutes"
description: "CurlDotNet quick start: install the NuGet package and run your first curl command in C# in 2 minutes, then POST JSON and add a bearer token."
---

# CurlDotNet quick start: run your first curl command in C# in 2 minutes

This CurlDotNet quick start takes you from an empty folder to running your first curl command in C# in about 2 minutes. You need the .NET SDK (8, 9 or 10) installed; see [installation](installation.html) if you do not.

## 1. Create a project and add the package

```bash
dotnet new console -n CurlQuickStart
cd CurlQuickStart
dotnet add package CurlDotNet
```

## 2. Replace Program.cs

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -A 'MyApp/1.0' https://api.github.com/users/octocat");

Console.WriteLine($"HTTP {result.StatusCode}");
Console.WriteLine(result.Body);
```

## 3. Run it

```bash
dotnet run
```

You should see `HTTP 200` followed by the JSON for the GitHub user `octocat`. That is the whole workflow: paste a curl command, `await` it, read `result.Body`.

## Common operations

Each example shows the curl-string form first and the fluent `CurlRequestBuilder` form second. Both return the same `CurlResult`.

### curl GET with headers in C#

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -H 'Accept: application/json' -H 'X-Request-Id: 42' https://httpbin.org/headers");
```

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Get("https://httpbin.org/headers")
    .WithHeader("Accept", "application/json")
    .WithHeader("X-Request-Id", "42")
    .ExecuteAsync();
```

### curl -X POST JSON in C#

```csharp
var result = await Curl.ExecuteAsync(@"curl -X POST https://httpbin.org/post
    -H 'Content-Type: application/json'
    -d '{""name"":""Ada"",""age"":36}'");
```

```csharp
var result = await CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithJson(new { name = "Ada", age = 36 })
    .ExecuteAsync();
```

Shortest form, when you already have an object:

```csharp
var result = await Curl.PostJsonAsync("https://httpbin.org/post", new { name = "Ada", age = 36 });
```

### curl bearer token in C#

```csharp
var result = await Curl.ExecuteAsync(
    "curl -H 'Authorization: Bearer token123' https://httpbin.org/bearer");
```

```csharp
var result = await CurlRequestBuilder.Get("https://httpbin.org/bearer")
    .WithBearerToken("token123")
    .ExecuteAsync();
```

### curl basic auth in C#

```csharp
var result = await Curl.ExecuteAsync("curl -u user:passwd https://httpbin.org/basic-auth/user/passwd");
```

```csharp
var result = await CurlRequestBuilder.Get("https://httpbin.org/basic-auth/user/passwd")
    .WithBasicAuth("user", "passwd")
    .ExecuteAsync();
```

### curl download file in C#

```csharp
var result = await Curl.ExecuteAsync("curl -o image.png https://httpbin.org/image/png");
```

```csharp
var result = await Curl.DownloadAsync("https://httpbin.org/image/png", "image.png");
```

Binary responses are also available in memory as `result.BinaryData` (a `byte[]`) when `result.IsBinary` is true.

### curl timeout in C#

```csharp
var result = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/delay/2");
```

```csharp
var result = await CurlRequestBuilder.Get("https://httpbin.org/delay/2")
    .WithTimeout(TimeSpan.FromSeconds(10))
    .ExecuteAsync();
```

### curl follow redirects in C#

```csharp
var result = await Curl.ExecuteAsync("curl -L https://httpbin.org/redirect/2");
Console.WriteLine(result.StatusCode);   // 200, not 302
```

## Parse the JSON response

`ParseJson<T>()` deserializes `result.Body` with `System.Text.Json` (Newtonsoft.Json on .NET Standard 2.0). Property matching is case-sensitive, so map snake_case or lower-case fields with `JsonPropertyName`.

```csharp
using System.Text.Json.Serialization;
using CurlDotNet;

public record GitHubUser(
    [property: JsonPropertyName("login")] string Login,
    [property: JsonPropertyName("public_repos")] int PublicRepos);

var result = await Curl.ExecuteAsync("curl -A 'MyApp/1.0' https://api.github.com/users/octocat");
var user = result.ParseJson<GitHubUser>();
Console.WriteLine($"{user.Login} has {user.PublicRepos} public repos");
```

For one-off access without a class, `result.AsJsonDynamic()` returns a `dynamic`.

## Handle errors

HTTP errors (4xx, 5xx) do not throw; check `IsSuccess`. Network failures throw a `CurlException` subclass.

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -m 5 https://httpbin.org/status/404");

    if (!result.IsSuccess)
        Console.WriteLine($"Server answered HTTP {result.StatusCode}");
}
catch (CurlTimeoutException)       { Console.WriteLine("Timed out"); }
catch (CurlConnectionException ex) { Console.WriteLine($"Could not connect: {ex.Message}"); }
catch (CurlException ex)           { Console.WriteLine($"curl error {ex.CurlErrorCode}: {ex.Message}"); }
```

If you prefer exceptions for HTTP errors, call `result.EnsureSuccess()`; it throws `CurlDotNet.Core.CurlHttpException` with the status code and response body.

## Run several commands at once

```csharp
var results = await Curl.ExecuteManyAsync(
    "curl -A 'MyApp/1.0' https://api.github.com/users/octocat",
    "curl -A 'MyApp/1.0' https://api.github.com/users/torvalds");

foreach (var r in results)
    Console.WriteLine(r.StatusCode);
```

## Next steps

- [First request, explained line by line](first-request.html)
- [Configuration: defaults, timeouts, redirects, SSL](configuration.html)
- [API guide: string API, builder API and LibCurl](../api-guide/README.html)
- [Advanced guide: proxies, cookies, binary downloads, timings](../guides/advanced.html)
- [Cookbook recipes](../cookbook/index.html)
- [Migrating from HttpClient](../migration/httpclient.html) or [RestSharp](../migration/restsharp.html)
