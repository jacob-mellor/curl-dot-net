---
title: "curl PUT, DELETE and PATCH Requests in C# (curl -X)"
description: "Send curl PUT, DELETE and PATCH requests in C# with curl -X and CurlDotNet: JSON bodies, the CurlRequestBuilder and the LibCurl reusable client."
---

# curl PUT, DELETE and PATCH requests in C#

To send curl PUT, DELETE and PATCH requests in C#, pass `-X <METHOD>` in the command string to `Curl.ExecuteAsync`, with `-d` for the body where the method takes one. Every verb curl accepts works the same way, so the command from an API's documentation can be used as-is.

## PUT - replace a resource

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(@"
    curl -X PUT https://httpbin.org/put \
         -H 'Content-Type: application/json' \
         -d '{""name"":""Ada"",""role"":""admin""}'");

Console.WriteLine(result.StatusCode);   // 200
```

## PATCH - partial update

```csharp
var result = await Curl.ExecuteAsync(@"
    curl -X PATCH https://httpbin.org/patch \
         -H 'Content-Type: application/json' \
         -d '{""role"":""editor""}'");

Console.WriteLine(result.Body);
```

## DELETE

```csharp
var result = await Curl.ExecuteAsync("curl -X DELETE https://httpbin.org/delete");

Console.WriteLine(result.IsSuccess ? "deleted" : $"HTTP {result.StatusCode}");
```

Many APIs answer DELETE with `204 No Content`; `IsSuccess` is true for any 2xx and `Body` is empty.

## Builder style

```csharp
using CurlDotNet.Core;

var put = await CurlRequestBuilder.Put("https://httpbin.org/put")
    .WithJson(new { name = "Ada", role = "admin" })
    .ExecuteAsync();

var patch = await CurlRequestBuilder.Patch("https://httpbin.org/patch")
    .WithJson(new { role = "editor" })
    .ExecuteAsync();

var del = await CurlRequestBuilder.Delete("https://httpbin.org/delete").ExecuteAsync();
Console.WriteLine($"{put.StatusCode} {patch.StatusCode} {del.StatusCode}");
```

`WithJson` serialises the object and sets `Content-Type: application/json`. For a raw string body use `WithData("...")`, and for any other verb `CurlRequestBuilder.Request("OPTIONS", url)`.

## Reusable client

```csharp
using CurlDotNet.Lib;

using var api = new LibCurl()
    .WithBearerToken(Environment.GetEnvironmentVariable("API_TOKEN"))
    .WithTimeout(TimeSpan.FromSeconds(15));

await api.PutAsync("https://httpbin.org/put", new { name = "Ada" });
await api.PatchAsync("https://httpbin.org/patch", new { role = "editor" });
var deleted = await api.DeleteAsync("https://httpbin.org/delete");
Console.WriteLine(deleted.StatusCode);
```

Object bodies on `PutAsync`/`PatchAsync` are sent as JSON.

## Check the outcome

```csharp
var result = await Curl.ExecuteAsync("curl -X DELETE https://httpbin.org/status/404");

if (!result.IsSuccess)
    Console.WriteLine($"Delete failed: {result.StatusCode} {result.Body}");
```

Without `-f`, HTTP errors are returned, not thrown. Call `result.EnsureSuccess()` if you prefer an exception, or add `-f` / `--fail-with-body` to the command to get `CurlHttpReturnedErrorException` (curl exit code 22).

## Gotchas

- `-d` without `-X` means POST, as in curl. Add `-X PUT` or `-X PATCH` explicitly when you want those verbs. `-X` only changes the method name: `-X GET -d 'q=1'` really sends a GET with a body. Use `-G` to put the data in the query string instead.
- `-T file` uploads a file's bytes with PUT, no `-X` needed; see [Upload a file](beginner/upload-file.html).
- Options that ask for different kinds of request, such as `-I` with `-d` or `-F` with `-d`, are rejected with a `CurlException` carrying exit code 2, just as curl refuses them.
- With `-L`, an `-X` method is reused on every redirect hop (curl's behaviour); see [follow redirects](follow-redirects.html).
- Quotes inside the JSON body: in a C# verbatim string (`@"..."`) double each `"`, or build the JSON with `JsonSerializer.Serialize` and interpolate it.
- A DELETE with a body is legal but some servers ignore it; prefer ids in the URL.

## Related

- [curl -X POST JSON in C#](beginner/send-json.html)
- [Call a REST API with curl in C#](beginner/call-api.html)
- [curl -F file upload and -T in C#](beginner/upload-file.html)
- [curl digest, NTLM and basic auth in C#](auth-schemes.html)
- [Cookbook index](index.html)
