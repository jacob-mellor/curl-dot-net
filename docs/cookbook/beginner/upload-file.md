---
title: "curl -F File Upload in C# (multipart/form-data)"
description: "How to do a curl -F file upload in C# as multipart/form-data with CurlDotNet. Covers extra form fields, --data-binary uploads and the request builder."
---

# curl -F file upload in C# (multipart/form-data)

This recipe shows how to do a curl -F file upload in C# with CurlDotNet: the same `curl -F 'file=@path' url` command you use in a shell, run from .NET as a `multipart/form-data` POST. It also covers extra form fields, raw body uploads with `--data-binary @path`, and the request builder equivalents.

## The problem

An API expects a file the way a browser form would send it (`multipart/form-data`), or it wants the raw bytes as the request body. You have the file on disk, or already in memory as a `byte[]`.

## Solution 1: the curl command string

`-F name=@path` attaches a file as a multipart part named `name`. The `@` tells curl to read the file; without it the text after `=` is sent as a plain field.

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -F 'file=@report.pdf' https://httpbin.org/post");

Console.WriteLine(result.StatusCode);   // 200
Console.WriteLine(result.Body);         // httpbin echoes the form and files it received
```

The request is sent as `POST` automatically. The part's file name is taken from the path (`report.pdf`) and its content type is `application/octet-stream`.

### Add ordinary form fields next to the file

Repeat `-F` for each field. Values without `@` are sent as text.

```csharp
var result = await Curl.ExecuteAsync(@"curl \
    -F 'file=@report.pdf' \
    -F 'title=Q3 report' \
    -F 'visibility=private' \
    https://httpbin.org/post");

var echo = result.AsJsonDynamic();
Console.WriteLine(echo.form.title);     // Q3 report
```

### Upload several files

```csharp
var result = await Curl.ExecuteAsync(
    "curl -F 'front=@front.jpg' -F 'back=@back.jpg' https://httpbin.org/post");
```

### Send the raw file as the request body (--data-binary)

Some APIs (object storage, image services, webhook sinks) want the bytes directly, not wrapped in a form. Use `--data-binary @path` and set the Content-Type yourself.

```csharp
var result = await Curl.ExecuteAsync(@"curl -X POST \
    -H 'Content-Type: image/png' \
    --data-binary '@logo.png' \
    https://httpbin.org/post");
```

`--data-binary` sends the file byte-for-byte. Plain `-d @path` would strip newlines, which corrupts binary files.

### Upload with authentication

```csharp
var result = await Curl.ExecuteAsync(@"curl \
    -H 'Authorization: Bearer YOUR_TOKEN' \
    -F 'file=@avatar.png' \
    https://httpbin.org/post");

result.EnsureSuccess();   // throws if the server answered 4xx/5xx
```

Basic auth is `-u user:password`. See [Authentication schemes](../auth-schemes.html).

## Solution 2: the request builder

`WithFile(fieldName, path)` adds one multipart file part and switches the method to POST.

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Post("https://httpbin.org/post")
    .WithFile("file", "report.pdf")
    .WithHeader("Authorization", "Bearer YOUR_TOKEN")
    .ExecuteAsync();
```

`WithMultipartForm(fields, files)` sets all fields and files in one call:

```csharp
var fields = new Dictionary<string, string> { ["title"] = "Q3 report" };
var files  = new Dictionary<string, string> { ["file"] = "report.pdf" };

var result = await CurlRequestBuilder
    .Post("https://httpbin.org/post")
    .WithMultipartForm(fields, files)
    .ExecuteAsync();
```

For a raw body from memory, use `WithBinaryData(byte[])` plus a Content-Type header:

```csharp
byte[] bytes = File.ReadAllBytes("logo.png");

var result = await CurlRequestBuilder
    .Post("https://httpbin.org/post")
    .WithHeader("Content-Type", "image/png")
    .WithBinaryData(bytes)
    .ExecuteAsync();
```

Call `.ToCurlCommand()` on any builder to print the equivalent curl command for a bug report or a shell test.

## Troubleshooting

**The file does not exist.** `--data-binary @path` and `-d @path` throw `CurlFileCouldntReadException` (from `CurlDotNet.Exceptions`) when the path is missing. A missing `-F file=@path`, and the builder's `WithFile`, surface as `System.IO.FileNotFoundException`. Resolve relative paths against `AppContext.BaseDirectory` or pass an absolute path.

**The server says "no file" or "missing field".** Check the field name. Many APIs want `file`, some want `upload`, `attachment` or `media`. `result.Body` from httpbin.org shows exactly which part names were received.

**The server rejects the content type.** Multipart parts are sent as `application/octet-stream`. If the API insists on a specific type for the whole request, use the `--data-binary` form with an explicit `-H 'Content-Type: ...'`.

**HTTP 413 Payload Too Large.** The file exceeds the server's limit. HTTP errors do not throw; check `result.IsSuccess` and `result.StatusCode`, or call `result.EnsureSuccess()`.

**`CurlOperationTimeoutException` on big uploads.** Raise `-m` (seconds), for example `-m 600`, or use `new CurlSettings().WithTimeout(600)`. See [Timeouts](../timeouts.html).

**Paths with spaces.** Quote the whole `-F` argument so the space is kept, for example `-F ""file=@My Report.pdf""` inside a C# verbatim string, or use the builder, which takes the path as a normal string.

## Related recipes

- [POST a form](post-form.html)
- [Send JSON](send-json.html)
- [Download a file](download-file.html)
- [Handle errors](handle-errors.html)
- [Authentication schemes](../auth-schemes.html)
- [HTTP methods (PUT, PATCH, DELETE)](../http-methods.html)
- [Cookbook index](../index.html)
- Tutorial: [Forms and data](../../tutorials/11-forms-and-data.html)
