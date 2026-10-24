---
title: "curl -F File Upload in C# (multipart/form-data)"
description: "How to do a curl -F file upload in C# as multipart/form-data with CurlDotNet. Covers extra form fields, ;type= and ;filename=, -T/--upload-file PUT uploads, --data-binary and the request builder."
---

# curl -F file upload in C# (multipart/form-data)

This recipe shows how to do a curl -F file upload in C# with CurlDotNet: the same `curl -F 'file=@path' url` command you use in a shell, run from .NET as a `multipart/form-data` POST laid out the way curl lays it out. It also covers extra form fields, curl's `;type=`, `;filename=` and `;headers=` modifiers, `-T`/`--upload-file` (a PUT of the raw file), raw body uploads with `--data-binary @path`, and the request builder equivalents.

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

The request is sent as `POST` automatically. The part's file name is taken from the path (`report.pdf`) and its content type comes from the extension using curl's own table: `.pdf` is `application/pdf`, `.png`/`.jpg`/`.gif`/`.svg` are images, `.txt`, `.html` and `.xml` get their text types, and anything else is `application/octet-stream`.

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

Several files in one field (`-F 'photos=@a.jpg,b.jpg'`) are sent as a nested `multipart/mixed` part, as curl does.

### Override the content type, file name or part headers

curl's `-F` modifiers work the same way here:

```csharp
var result = await Curl.ExecuteAsync(@"curl \
    -F 'data=@export.bin;type=text/csv;filename=report.csv' \
    -F 'file=@notes.txt;headers=X-Source: nightly-job' \
    -F 'body=<message.txt' \
    https://httpbin.org/post");
```

| Syntax | What is sent |
|--------|--------------|
| `name=value` | A text field. No `Content-Type` header on the part, exactly like curl. |
| `name=@file` | A file part with `filename=` and a content type from the extension. |
| `name=<file` | A text field whose value is read from the file (no `filename=`). |
| `;type=text/csv` | Sets the part's `Content-Type` (a following `;charset=...` belongs to it). |
| `;filename=other.csv` | Changes the file name the server sees. |
| `;headers=X-Name: value` | Adds a header to that part. |

Each request gets a curl-style boundary (24 dashes plus 22 random letters and digits), and names are quoted in `Content-Disposition` with `"` escaped as `%22`. `--form-escape` switches to backslash escaping instead.

Because `@`, `<` and `;` are special in `-F`, use `--form-string` for a value that must be sent literally:

```csharp
var result = await Curl.ExecuteAsync(
    "curl --form-string 'handle=@octocat' --form-string 'note=a;b' https://httpbin.org/post");
```

### Upload the raw file with PUT (-T / --upload-file)

`-T` sends the file's bytes as the body of a `PUT`, with no multipart wrapping. When the URL ends in `/` (or has no path), the file name is appended to it, as curl does:

```csharp
// PUT https://example.com/uploads/backup.zip
var result = await Curl.ExecuteAsync(
    "curl -T backup.zip https://example.com/uploads/");

Console.WriteLine(result.StatusCode);   // 200, 201 or 204 depending on the server
```

No `Content-Type` is sent unless you add `-H 'Content-Type: ...'`. `-X POST -T file url` keeps the file body but uses POST. Reading standard input (`-T -` or `-T .`) is not supported; save the data to a file first.

Without a command string, set the same thing on `CurlOptions` and call `Curl.ExecuteAsync(options)`. Nothing is parsed in that case, so read the bytes yourself and give the full target URL:

```csharp
using CurlDotNet.Core;

var options = new CurlOptions
{
    Url = "https://example.com/uploads/backup.zip",
    Method = "PUT",
    UploadFile = "backup.zip",
    BinaryData = File.ReadAllBytes("backup.zip"),
};

var result = await Curl.ExecuteAsync(options);
```

### Send the raw file as the request body (--data-binary)

Some APIs (object storage, image services, webhook sinks) want the bytes directly, not wrapped in a form. Use `--data-binary @path` and set the Content-Type yourself.

```csharp
var result = await Curl.ExecuteAsync(@"curl -X POST \
    -H 'Content-Type: image/png' \
    --data-binary '@logo.png' \
    https://httpbin.org/post");
```

`--data-binary` sends the file byte-for-byte with POST. Plain `-d @path` strips carriage returns, newlines and NUL bytes (as curl does), which corrupts binary files. Use `-T` instead when the API wants a PUT.

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

**The file does not exist.** In a command string, a missing or unreadable file for `-F name=@path`, `-F name=<path`, `-T path`, `--data-binary @path` or `-d @path` throws `CurlReadErrorException` (from `CurlDotNet.Exceptions`, curl exit code 26) naming the option and the path; you never get a raw `IOException`. The builder's `WithFile` checks up front and throws `System.IO.FileNotFoundException`. Relative paths are resolved against the current directory, so resolve them against `AppContext.BaseDirectory` or pass an absolute path.

```csharp
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl -F 'file=@missing.pdf' https://httpbin.org/post");
}
catch (CurlReadErrorException ex)
{
    Console.WriteLine($"{ex.FilePath}: {ex.Message}");   // exit code 26 in ex.CurlErrorCode
}
```

**The server says "no file" or "missing field".** Check the field name. Many APIs want `file`, some want `upload`, `attachment` or `media`. `result.Body` from httpbin.org shows exactly which part names were received.

**The server rejects the content type.** A file part's type comes from its extension and falls back to `application/octet-stream`. Set it per part with `;type=`, for example `-F 'file=@data.bin;type=application/zip'`. If the API wants the bytes as the whole request, use `-T` or `--data-binary` with an explicit `-H 'Content-Type: ...'`.

**A field value containing `;`, or starting with `@` or `<`, is cut short or read as a file.** That is curl's `-F` syntax. Send such values with `--form-string`.

**HTTP 413 Payload Too Large.** The file exceeds the server's limit. Without `-f`, HTTP errors do not throw; check `result.IsSuccess` and `result.StatusCode`, or call `result.EnsureSuccess()`.

**`CurlOperationTimeoutException` on big uploads.** There is no timeout unless you set one, so this only happens when `-m` is too small. Raise `-m` (seconds, decimals allowed), for example `-m 600`, or use `new CurlSettings().WithTimeout(600)`. See [Timeouts](../timeouts.html).

**Resuming an upload.** curl cannot resume an HTTP upload: `-C` together with `-F`, `-d` or `-T` fails with `CurlBadFunctionArgumentException` (exit code 43).

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
