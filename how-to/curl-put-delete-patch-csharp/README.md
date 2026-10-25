---
title: "How to Send curl PUT, DELETE and PATCH in C#"
description: "curl PUT, DELETE and PATCH in C#: run curl -X PUT / -X PATCH / -X DELETE with CurlDotNet, or use the builder's Put, Patch and Delete methods."
---

# How to send curl PUT, DELETE and PATCH requests in C#

To send PUT, DELETE or PATCH requests in C# the curl way, set the method with `-X` in the command you pass to `Curl.ExecuteAsync` (`-X PUT`, `-X PATCH`, `-X DELETE`), or start the fluent builder with `CurlRequestBuilder.Put`, `.Patch` or `.Delete`.

```bash
dotnet add package CurlDotNet
```

## PUT: replace a resource

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("""
    curl -X PUT https://httpbin.org/put \
      -H "Content-Type: application/json" \
      -d '{"id":42,"name":"Ada","role":"admin"}'
    """);

Console.WriteLine(result.StatusCode);  // 200
```

## PATCH: change part of a resource

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    """curl -X PATCH https://httpbin.org/patch --json '{"role":"owner"}'""");

Console.WriteLine(result.StatusCode);
```

## DELETE

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -X DELETE https://httpbin.org/delete -H 'Authorization: Bearer my-token'");

Console.WriteLine(result.IsSuccess);  // many APIs answer 204 No Content
```

## The fluent builder

```csharp
using CurlDotNet.Core;

var put = await CurlRequestBuilder.Put("https://httpbin.org/put")
    .WithJson(new { id = 42, name = "Ada" }).ExecuteAsync();

var patch = await CurlRequestBuilder.Patch("https://httpbin.org/patch")
    .WithJson(new { role = "owner" }).ExecuteAsync();

var delete = await CurlRequestBuilder.Delete("https://httpbin.org/delete")
    .WithBearerToken("my-token").ExecuteAsync();
```

Other verbs go through `CurlRequestBuilder.Request("OPTIONS", url)` or `-X OPTIONS`.

## A small REST client

`CurlApiClient` wraps a base URL and sends JSON for you:

```csharp
using CurlDotNet;

var api = new CurlApiClient("https://httpbin.org", TimeSpan.FromSeconds(15));

var created = await api.PostJsonAsync("/post", new { name = "Ada" });
var updated = await api.PutJsonAsync("/put", new { name = "Ada L." });
var patched = await api.PatchJsonAsync("/patch", new { role = "owner" });
var deleted = await api.DeleteAsync("/delete");
```

## With LibCurl

`LibCurl.PutAsync` and `PatchAsync` serialize an object body to JSON, but they do not add a JSON `Content-Type` themselves (without one the body goes out as `application/x-www-form-urlencoded`, as with `curl -d`). Add it as a default header:

```csharp
using CurlDotNet.Lib;

using var curl = new LibCurl();
curl.WithHeader("Content-Type", "application/json");

var result = await curl.PutAsync("https://httpbin.org/put", new { id = 42 });
Console.WriteLine(result.StatusCode);
```

## Checking the result

Expect `200 OK`, `201 Created`, `202 Accepted` or `204 No Content` on success; `result.IsSuccess` covers all of them. A `409 Conflict` or `412 Precondition Failed` does not throw, so check the status, call `EnsureSuccess()` to turn any non-2xx status into an exception, or add `-f` to the command to have curl's `CurlHttpReturnedErrorException` (exit code 22) thrown for any status of 400 or above.

`-T file` is a shortcut for a PUT of a file's bytes, and `-X GET -d '...'` sends a GET with a body, both as in curl. With `-L`, a method set with `-X` is kept on every redirect hop.

## Related

- [POST JSON](../curl-post-json-csharp/README.md)
- [Error handling](../curl-error-handling-csharp/README.md)
- [Use CurlDotNet in ASP.NET Core](../curl-in-aspnet-core/README.md)
- [All how-to guides](../README.md)
