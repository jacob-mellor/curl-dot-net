---
title: "How to Send curl -X POST JSON in C#"
description: "curl -X POST JSON in C#: run the curl -d or --json command with CurlDotNet, or use CurlRequestBuilder.WithJson to serialize a C# object."
---

# How to send curl -X POST JSON in C#

To send `curl -X POST` JSON in C#, pass the curl command with `-H "Content-Type: application/json"` and `-d '<json>'` to `Curl.ExecuteAsync`, or call `CurlRequestBuilder.Post(url).WithJson(obj)` to serialize a C# object for you.

```bash
dotnet add package CurlDotNet
```

## The curl command

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("""
    curl -X POST https://httpbin.org/post \
      -H "Content-Type: application/json" \
      -d '{"name":"Ada Lovelace","age":36}'
    """);

Console.WriteLine(result.StatusCode);  // 200
Console.WriteLine(result.Body);
```

`--json` is shorter: it sets the body and adds both `Content-Type: application/json` and `Accept: application/json`.

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    """curl --json '{"name":"Ada"}' https://httpbin.org/post""");

Console.WriteLine(result.StatusCode);
```

As with the curl CLI, `-d` on its own sends `Content-Type: application/x-www-form-urlencoded`. Many JSON APIs reject that, so always add the header (or use `--json`).

## The fluent builder

`WithJson` serializes any object (System.Text.Json on .NET Framework and .NET 8+, Newtonsoft.Json on the netstandard2.0 build) and sets the `Content-Type` header:

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Post("https://httpbin.org/post")
    .WithJson(new { name = "Ada Lovelace", age = 36 })
    .WithBearerToken("my-token")
    .ExecuteAsync();

Console.WriteLine(result.StatusCode);
```

## One-liner helper

```csharp
using CurlDotNet;

var result = await Curl.PostJsonAsync("https://httpbin.org/post", new { name = "Ada" });
Console.WriteLine(result.Body);
```

## Build the JSON from variables safely

When values come from variables, prefer `WithJson` over string concatenation. It escapes quotes, backslashes and newlines correctly, and it keeps user input from being read as curl flags. If you do build a curl string, a C# raw interpolated string keeps the JSON braces readable:

```csharp
using CurlDotNet;

var name = "Ada";
var result = await Curl.ExecuteAsync($$"""
    curl --json '{"name":"{{name}}"}' https://httpbin.org/post
    """);
Console.WriteLine(result.StatusCode);
```

## Read the JSON response

```csharp
using System.Text.Json;
using CurlDotNet;

var result = await Curl.PostJsonAsync("https://httpbin.org/post", new { id = 7 });
var echoed = result.ParseJson<JsonElement>().GetProperty("json");
Console.WriteLine(echoed.GetProperty("id").GetInt32());  // 7
```

## Related

- [Parse JSON from a curl response](../parse-json-from-curl-in-csharp/README.md)
- [PUT, PATCH and DELETE](../curl-put-delete-patch-csharp/README.md)
- [Bearer token authentication](../curl-bearer-token-csharp/README.md)
- [Multipart file upload with -F](../curl-file-upload-csharp/README.md)
- [All how-to guides](../README.md)
