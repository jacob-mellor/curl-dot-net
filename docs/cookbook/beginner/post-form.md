---
title: "curl -d Form Data POST in C# - CurlDotNet Recipe"
description: "POST application/x-www-form-urlencoded form data with curl -d in C# using CurlDotNet, --data-urlencode for special characters, and WithFormData."
---

# curl -d form data POST in C# (application/x-www-form-urlencoded)

A `curl -d` form data POST in C# sends `application/x-www-form-urlencoded` fields exactly like an HTML form does; CurlDotNet runs the same `-d 'field=value'` command string you would type in a terminal. This recipe covers plain `-d`, `--data-urlencode` for values with spaces and symbols, the `WithFormData` builder, and the usual form-related errors.

## The problem

A server expects a classic form submission (login, search, contact form, legacy API) rather than JSON. The body must look like `username=john&password=secret` and the `Content-Type` must be `application/x-www-form-urlencoded`.

## Solution 1: run the curl command string

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

var result = await Curl.ExecuteAsync(@"
    curl https://httpbin.org/post \
      -d 'username=john' \
      -d 'password=secret123'");

Console.WriteLine(result.StatusCode);   // 200
Console.WriteLine(result.Body);         // httpbin echoes the fields under "form"
```

Each `-d` adds one field; curl joins them with `&`. Passing `-d` switches the method to POST and sets `Content-Type: application/x-www-form-urlencoded`, so `-X POST` is optional. You can also send the whole body in one argument:

```csharp
var result = await Curl.ExecuteAsync(
    "curl -d 'name=John+Doe&email=john%40example.com' https://httpbin.org/post");
```

Note that with plain `-d` the value is sent as written: `John Doe` must already be `John+Doe` or `John%20Doe`. Use `--data-urlencode` to have that done for you.

### Encode values with --data-urlencode

```csharp
var result = await Curl.ExecuteAsync(@"
    curl https://httpbin.org/post \
      --data-urlencode 'name=John Doe' \
      --data-urlencode 'message=Hello & welcome! 100% free' \
      --data-urlencode 'email=john@example.com'");

Console.WriteLine(result.Body);   // "message": "Hello & welcome! 100% free"
```

`--data-urlencode 'name=value'` percent-encodes `value` and leaves `name` alone, matching curl's behaviour. Mixing `-d` and `--data-urlencode` in one command is fine.

### Build the body from a dictionary

```csharp
using System.Collections.Generic;
using System.Linq;

var fields = new Dictionary<string, string>
{
    ["username"] = "john", ["remember"] = "true", ["note"] = "hello world",
};

string body = string.Join("&",
    fields.Select(f => $"{f.Key}={Uri.EscapeDataString(f.Value)}"));

var result = await Curl.ExecuteAsync($"curl -d '{body}' https://httpbin.org/post");
```

`Curl.PostAsync(url, data)` posts a raw string body with the form content type if you prefer a method call over a command.

## Solution 2: the fluent builder

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Post("https://httpbin.org/post")
    .WithFormData(new Dictionary<string, string>
    {
        ["username"] = "john",
        ["password"] = "secret123",
    })
    .ExecuteAsync();

Console.WriteLine(result.StatusCode);
```

`WithFormData` URL-encodes every value and sets the `Content-Type` header. `.ToCurlCommand()` shows the curl command it will run.

## Variations

### Repeated fields (arrays)

Most servers read repeated names, or `name[]`, as a list. Just repeat the field:

```csharp
var result = await Curl.ExecuteAsync(@"
    curl https://httpbin.org/post \
      -d 'tag=csharp' -d 'tag=curl' -d 'tag=http'");

Console.WriteLine(result.Body);   // "tag": ["csharp", "curl", "http"]
```

### Form POST with basic auth

```csharp
var result = await Curl.ExecuteAsync(@"
    curl -u 'user:passwd' https://httpbin.org/post \
      --data-urlencode 'query=hello world'");

// builder equivalent
var r2 = await CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithBasicAuth("user", "passwd")
    .WithFormData(new Dictionary<string, string> { ["query"] = "hello world" })
    .ExecuteAsync();
```

### Form data from a file (-d @file)

```csharp
// body.txt contains: username=john&password=secret123
var result = await Curl.ExecuteAsync("curl -d @body.txt https://httpbin.org/post");
```

A missing file throws `CurlFileCouldntReadException`.

### Multipart instead of URL-encoded (-F)

When the server expects `multipart/form-data` (file uploads, some frameworks), switch `-d` for `-F`:

```csharp
var result = await Curl.ExecuteAsync(@"
    curl https://httpbin.org/post \
      -F 'title=Report' \
      -F 'file=@report.pdf'");

var r2 = await CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithMultipartForm(new Dictionary<string, string> { ["title"] = "Report" })
    .WithFile("file", "report.pdf")
    .ExecuteAsync();
```

Details in [Upload a File](upload-file.html).

### Check the reply

```csharp
var result = await Curl.ExecuteAsync("curl -d 'q=test' https://httpbin.org/post");

if (!result.IsSuccess)
    Console.WriteLine($"HTTP {result.StatusCode}: {result.Body}");

Console.WriteLine(result.GetHeader("Content-Type"));
```

HTTP 4xx/5xx replies do not throw; use `result.IsSuccess`, `result.StatusCode`, or `result.EnsureSuccess()` (throws `CurlDotNet.Core.CurlHttpException`).

## Troubleshooting

**Values arrive truncated at `&` or `+` becomes a space.** Plain `-d` sends the text verbatim. Use `--data-urlencode 'field=value'`, `Uri.EscapeDataString`, or the `WithFormData` builder so reserved characters are percent-encoded.

**400 Bad Request.** A required field is missing or misspelled, or the server wanted JSON. Print `result.Body` for the validation message and compare against the API docs. For JSON see [Send JSON](send-json.html).

**415 Unsupported Media Type.** The server wants `multipart/form-data`; use `-F` as shown above. Conversely, if you used `-F` and the server rejects it, go back to `-d`.

**Fields are not being sent.** A bare `curl url -d` with the data on the wrong side of a line break, or a missing backslash in a multi-line verbatim string, drops the argument. Call `Curl.Validate(command)` and check `IsValid`/`Error`, or add `-v` and read the request line in `result.Body`.

**401 Unauthorized.** Add `-u user:pass` or an `Authorization` header; see [Authentication schemes](../auth-schemes.html).

**Login form sets a cookie I need later.** Pass it back with `-b 'session=abc'` on the next request. See [Cookies](../cookies.html).

## Related

- [Send JSON](send-json.html) - `curl -X POST` with a JSON body instead of a form
- [Upload a File](upload-file.html) - multipart `-F` uploads
- [Call a REST API](call-api.html) - GET/POST/PUT/DELETE with a bearer token
- [Handle Errors](handle-errors.html) - status codes versus exceptions
- [Tutorial: Forms and Data](../../tutorials/11-forms-and-data.html)
- [Cookbook index](../index.html)
