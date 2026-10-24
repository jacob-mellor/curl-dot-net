---
title: "curl -X POST JSON in C# - CurlDotNet Recipe"
description: "Send JSON with curl -X POST in C# using CurlDotNet: -d plus Content-Type header, --json, the WithJson builder, Curl.PostJsonAsync and ParseJson<T>."
---

# curl -X POST JSON in C#

To send JSON with `curl -X POST` in C#, give CurlDotNet the same command you would run in a terminal: `-H 'Content-Type: application/json'` plus `-d '{...}'`, or the shorter `--json '{...}'`. This recipe shows the curl-string form, the `WithJson` builder, the `Curl.PostJsonAsync` shortcut, and how to read the JSON reply with `ParseJson<T>()`.

## The problem

You need to POST a JSON document to an API (create a record, call a webhook, submit an event) and read the JSON the server sends back.

## Solution 1: run the curl command string

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

var result = await Curl.ExecuteAsync(@"
    curl -X POST https://httpbin.org/post \
      -H 'Content-Type: application/json' \
      -d '{""name"": ""Alice"", ""age"": 30}'");

Console.WriteLine(result.StatusCode);   // 200
Console.WriteLine(result.Body);         // httpbin echoes the JSON it received
```

Inside a C# verbatim string (`@"..."`) a double quote is written as `""`. Wrapping the JSON in single quotes, exactly as in a shell, keeps the double quotes inside it intact.

The `--json` option sets `Content-Type: application/json` and `Accept: application/json` for you, so the command gets shorter:

```csharp
var result = await Curl.ExecuteAsync(@"
    curl --json '{""name"": ""Alice"", ""age"": 30}' https://httpbin.org/post");
```

`-X POST` is implied whenever `-d` or `--json` is present, so you can leave it out.

### Serialize a C# object

Build the JSON with `System.Text.Json` instead of typing it by hand.

```csharp
using System.Text.Json;

var user = new { name = "Alice", age = 30, email = "alice@example.com" };
string json = JsonSerializer.Serialize(user);

var result = await Curl.ExecuteAsync($"curl --json '{json}' https://httpbin.org/post");
```

`Curl.PostJsonAsync(url, object)` does the serialization and the headers in one call. Nothing goes through a command string, so apostrophes and quotes in the values (`O'Brien`) need no escaping and stay readable in the payload:

```csharp
var result = await Curl.PostJsonAsync("https://httpbin.org/post",
    new { name = "Alice", age = 30 });

Console.WriteLine(result.IsSuccess);
```

## Solution 2: the fluent builder

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Post("https://httpbin.org/post")
    .WithJson(new { name = "Alice", age = 30 })
    .WithHeader("Accept", "application/json")
    .ExecuteAsync();

Console.WriteLine(result.StatusCode);
```

`WithJson(object)` serializes the object and sets `Content-Type: application/json`. Call `.ToCurlCommand()` on the builder to see the equivalent curl string.

## Read the JSON reply

```csharp
public class HttpBinPost
{
    public EchoedUser Json { get; set; }   // binds to "json"
    public string Url { get; set; }        // binds to "url"
}
public class EchoedUser
{
    public string Name { get; set; }       // binds to "name"
}

var echo = result.ParseJson<HttpBinPost>();
Console.WriteLine(echo.Json.Name);   // "Alice"
```

`ParseJson<T>()` matches property names case-insensitively (`System.Text.Json` with `PropertyNameCaseInsensitive`; Newtonsoft.Json on .NET Standard 2.0), so camelCase or lower-case JSON binds to ordinary PascalCase properties. snake_case names such as `created_at` still need `[JsonPropertyName("created_at")]` (from `System.Text.Json.Serialization`). An empty body or malformed JSON throws `InvalidOperationException` with the parser's error as the inner exception. `result.AsJsonDynamic()` gives you a `dynamic` when you do not want a class.

## Variations

### PUT or PATCH instead of POST

```csharp
var put = await Curl.ExecuteAsync(@"
    curl -X PUT https://httpbin.org/put --json '{""name"": ""Alice""}'");

var patch = await CurlRequestBuilder
    .Patch("https://httpbin.org/patch")
    .WithJson(new { age = 31 })
    .ExecuteAsync();
```

See [HTTP methods](../http-methods.html) for the full set.

### Add a bearer token

```csharp
var result = await Curl.ExecuteAsync(@"
    curl https://httpbin.org/post \
      -H 'Authorization: Bearer YOUR_TOKEN' \
      --json '{""event"": ""signup""}'");

// builder equivalent
var r2 = await CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithBearerToken("YOUR_TOKEN")
    .WithJson(new { @event = "signup" })
    .ExecuteAsync();
```

### JSON from a file (-d @file)

```csharp
var result = await Curl.ExecuteAsync(@"
    curl https://httpbin.org/post \
      -H 'Content-Type: application/json' \
      -d @payload.json");
```

The path is relative to the process working directory. Like curl, `-d @file` removes carriage returns and newlines from the file, which is harmless for JSON; use `--data-binary @payload.json` to send it byte-for-byte. A missing file throws `CurlReadErrorException` (exit code 26).

### Check the status before parsing

```csharp
using CurlDotNet.Exceptions;

var result = await Curl.PostJsonAsync("https://httpbin.org/status/422", new { x = 1 });

if (!result.IsSuccess)
    Console.WriteLine($"HTTP {result.StatusCode}: {result.Body}");

// or throw CurlDotNet.Core.CurlHttpException on non-2xx:
var ok = result.EnsureSuccess().ParseJson<HttpBinPost>();
```

Without `-f`, HTTP 4xx/5xx replies never throw on their own; `EnsureSuccess()` turns them into a `CurlDotNet.Core.CurlHttpException` carrying `StatusCode` and `ResponseBody`. In a command string, `--fail-with-body` throws `CurlHttpReturnedErrorException` (exit code 22) with the error body instead. See [Handle Errors](handle-errors.html).

## Troubleshooting

**400 Bad Request.** The body is usually malformed JSON. Serialize with `JsonSerializer.Serialize` rather than concatenating strings, and print `result.Body` for the server's explanation.

**415 Unsupported Media Type.** The `Content-Type` header is missing. With `-d` you must add `-H 'Content-Type: application/json'` yourself; `--json`, `WithJson` and `PostJsonAsync` set it for you.

**Quotes break the command.** In a verbatim string write `""` for each `"`. If the JSON itself contains a single quote (`O'Brien`), prefer `--json` with a serialized object, the builder, or `-d @file`.

**Special characters or non-ASCII text.** The body is sent as UTF-8. If the server mangles it, make sure your `Content-Type` includes `charset=utf-8` (`--json` already implies UTF-8 JSON).

**`ParseJson<T>()` returns nulls.** Property names do not match. Case does not matter, but underscores do: add `[JsonPropertyName("...")]` for snake_case names, or inspect `result.Body` with `AsJsonDynamic()`.

## Related

- [Call a REST API](call-api.html) - GET/POST/PUT/DELETE with authentication
- [POST Form Data](post-form.html) - `application/x-www-form-urlencoded` instead of JSON
- [Upload a File](upload-file.html) - multipart `-F` uploads
- [Handle Errors](handle-errors.html) - status codes versus exceptions
- [Authentication schemes](../auth-schemes.html) - basic, bearer, digest
- [Tutorial: JSON for Beginners](../../tutorials/07-json-for-beginners.html)
- [Cookbook index](../index.html)
