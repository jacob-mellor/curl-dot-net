---
title: "curl -d Form Data and POST in C# with CurlDotNet"
description: "curl -d form data and POST in C#: URL-encoded forms, --data-urlencode, multipart -F, JSON with --json, XML and query strings using CurlDotNet."
---

# Tutorial 11: curl -d form data and POST in C#

This tutorial covers curl -d form data and POST in C# with CurlDotNet: URL-encoded form submissions with `-d` and `--data-urlencode`, multipart forms with `-F`, JSON with `--json`, XML and plain text bodies, and query strings. Every example targets https://httpbin.org/post, which echoes back whatever it receives.

## HTML Form Submission

### Simple form POST (-d)

`-d` switches curl to POST and sets `Content-Type: application/x-www-form-urlencoded`. The value is the form body, already encoded (`%40` is `@`):

```csharp
using CurlDotNet;

// Same as: curl -d 'username=john_doe&email=john%40example.com' https://httpbin.org/post
var result = await Curl.ExecuteAsync(
    "curl -d 'username=john_doe&email=john%40example.com' https://httpbin.org/post");

Console.WriteLine(result.StatusCode);   // 200
Console.WriteLine(result.Body);         // JSON with a "form": { "username": ..., "email": ... }
```

You can repeat `-d` once per field; curl joins them with `&`. `-d ''` sends an empty body, and `--data-ascii` is an alias for `-d`:

```csharp
var result = await Curl.ExecuteAsync(@"curl \
  -d 'username=john_doe' \
  -d 'subscribe=true' \
  https://httpbin.org/post");
```

### Let curl encode the values (--data-urlencode)

When a value contains spaces, `&`, `@` or other special characters, use `--data-urlencode` and curl encodes it for you:

```csharp
var result = await Curl.ExecuteAsync(@"curl \
  --data-urlencode 'comment=Hello world & friends' \
  --data-urlencode 'email=john@example.com' \
  https://httpbin.org/post");
// body: comment=Hello+world+%26+friends&email=john%40example.com
```

The encoding is curl's: spaces become `+` (not `%20`) and only the part after the first `=` is encoded. All five curl forms work:

| Form | Sends |
|------|-------|
| `--data-urlencode 'content'` | `content` encoded |
| `--data-urlencode '=content'` | `content` encoded (use this when the content itself contains `=`) |
| `--data-urlencode 'name=content'` | `name=` followed by `content` encoded |
| `--data-urlencode '@file'` | the file's contents encoded |
| `--data-urlencode 'name@file'` | `name=` followed by the file's contents encoded |

### Convenience helper and builder

`Curl.PostAsync` takes an already-encoded body. The builder's `WithFormData` encodes a dictionary for you:

```csharp
using CurlDotNet.Core;

// Helper: POST a form-encoded body
var quick = await Curl.PostAsync("https://httpbin.org/post", "username=john_doe&age=25");

// Builder: each key and value is URL-encoded automatically
var form = new Dictionary<string, string>
{
    ["username"] = "john_doe",
    ["email"] = "john@example.com"
};
var result = await CurlRequestBuilder
    .Post("https://httpbin.org/post")
    .WithFormData(form)
    .ExecuteAsync();
```

## Multipart Forms (-F)

Use `-F` when the form includes a file, or when the server expects `multipart/form-data`. Text fields and files mix freely:

```csharp
var result = await Curl.ExecuteAsync(@"curl \
  -F 'name=Jane Smith' \
  -F 'position=Software Developer' \
  -F 'resume=@resume.pdf' \
  https://httpbin.org/post");

Console.WriteLine(result.Body);   // "form": {...} and "files": { "resume": ... }
```

The builder equivalent is `WithMultipartForm(fields, files)`:

```csharp
var fields = new Dictionary<string, string>
{
    ["name"] = "Jane Smith",
    ["position"] = "Software Developer"
};
var files = new Dictionary<string, string> { ["resume"] = "resume.pdf" };

var result = await CurlRequestBuilder
    .Post("https://httpbin.org/post")
    .WithMultipartForm(fields, files)
    .ExecuteAsync();
```

File uploads are covered in more depth in [Tutorial 10](10-files-and-downloads.html).

## Working with Different Content Types

### JSON data

`--json` sets both `Content-Type` and `Accept` to `application/json` in one flag:

```csharp
// Same as: curl --json '{"name":"Jane","newsletter":true}' https://httpbin.org/post
var result = await Curl.ExecuteAsync(
    @"curl --json '{""name"":""Jane"",""newsletter"":true}' https://httpbin.org/post");
```

When you have a C# object, let the library serialize it:

```csharp
var user = new { name = "Jane Smith", email = "jane@example.com" };

// Helper
var result = await Curl.PostJsonAsync("https://httpbin.org/post", user);

// Builder
var result2 = await CurlRequestBuilder
    .Post("https://httpbin.org/post")
    .WithJson(user)
    .ExecuteAsync();
```

JSON is covered in detail in [Tutorial 7](07-json-for-beginners.html) and the [Send JSON](../cookbook/beginner/send-json.html) recipe.

### XML data

Set the `Content-Type` yourself and pass the document with `-d`:

```csharp
var result = await Curl.ExecuteAsync(@"curl \
  -H 'Content-Type: application/xml' \
  -d '<user><name>John Doe</name><age>30</age></user>' \
  https://httpbin.org/post");
```

For a document stored on disk use `--data-binary @body.xml`. Plain `-d @file` strips newlines, exactly as curl does; `--data-binary` sends the file byte for byte.

### Plain text

```csharp
var result = await Curl.ExecuteAsync(@"curl \
  -H 'Content-Type: text/plain' \
  -d 'This is a plain text message.' \
  https://httpbin.org/post");
```

## Query Parameters

### Let curl build the query (-G)

`-G` (`--get`) turns every `-d` and `--data-urlencode` value into the query string of a GET request instead of a POST body, exactly as in curl:

```csharp
// Same as: curl -G --data-urlencode 'q=curl tutorial' -d page=1 https://httpbin.org/get
var result = await Curl.ExecuteAsync(
    "curl -G --data-urlencode 'q=curl tutorial' -d page=1 https://httpbin.org/get");
// requests https://httpbin.org/get?q=curl+tutorial&page=1
```

`-G -I` does the same for a HEAD request, and `--url-query` appends query parameters without touching the body. If you really want a GET with a body, write `-X GET -d '...'`.

### Build the URL yourself

Query strings belong in the URL. Encode each key and value with `Uri.EscapeDataString`, then quote the URL so `&` is not misread:

```csharp
var parameters = new Dictionary<string, string>
{
    ["q"] = "curl tutorial",
    ["page"] = "1"
};
var query = string.Join("&", parameters.Select(kv =>
    $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

var result = await Curl.ExecuteAsync($"curl 'https://httpbin.org/get?{query}'");
Console.WriteLine(result.Body);   // "args": { "page": "1", "q": "curl tutorial" }
```

## Checking the Server's Answer

A 4xx or 5xx response does not throw; the result simply reports `IsSuccess == false`:

```csharp
var result = await Curl.ExecuteAsync("curl -d 'username=' https://httpbin.org/status/422");

if (!result.IsSuccess)
{
    Console.WriteLine($"Server rejected the form: HTTP {result.StatusCode}");
    Console.WriteLine(result.Body);   // often contains validation messages
}
```

If you prefer exceptions, `EnsureSuccess()` throws `CurlHttpException` with the status and body attached:

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl -d 'username=' https://httpbin.org/status/422");
    result.EnsureSuccess();
}
catch (CurlHttpException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.ResponseBody}");
}
```

Error handling in general is the subject of [Tutorial 6](06-handling-errors.html).

## Forms That Need a Session Token

Some sites put a CSRF token in the form page and expect it back with the submission. Fetch the page, pull the token out, then post it along with the cookie string the server set:

```csharp
var page = await Curl.ExecuteAsync("curl https://example.com/login");

var token = System.Text.RegularExpressions.Regex
    .Match(page.Body, @"name=""csrf_token"" value=""([^""]+)""").Groups[1].Value;
var session = page.GetHeader("Set-Cookie")?.Split(';')[0];   // e.g. session=abc123

var result = await Curl.ExecuteAsync(
    $"curl -b '{session}' --data-urlencode 'csrf_token={token}' " +
    "-d 'username=john_doe' https://example.com/login");
```

curl's cookie jar works too: `-c cookies.txt` on the first request saves the cookies in Netscape format, and `-b cookies.txt` on the second sends them back, following curl's domain, path and expiry rules. See the [cookies recipe](../cookbook/cookies.html).

Use a real HTML parser in production code; the regex is only there to keep the example short.

## Best Practices

1. **Prefer `--data-urlencode` or `WithFormData`** - hand-encoding form values is a common source of bugs.
2. **Use `-F` only when you need multipart** - plain `-d` is smaller and what most HTML forms send.
3. **Set `Content-Type` explicitly for XML and text** - `-d` defaults to form encoding.
4. **Use `--data-binary @file` for files** - `-d @file` strips newlines.
5. **Quote URLs containing `&`** so the shell-style parser does not split them.
6. **Check `IsSuccess` or call `EnsureSuccess()`** - HTTP errors do not throw on their own.
7. **Send credentials and personal data over HTTPS only.**

## Summary

- `-d 'a=1&b=2'` posts a URL-encoded form; `--data-urlencode` encodes values for you (spaces as `+`).
- `-G` moves the data into the query string of a GET.
- `-F` builds multipart forms with text fields and files.
- `--json`, `Curl.PostJsonAsync` and `WithJson` send JSON; `-H 'Content-Type: ...' -d` handles XML and text.
- Query strings are built with `Uri.EscapeDataString` and placed in the URL.

See also the [POST a Form](../cookbook/beginner/post-form.html) recipe.

## What's Next?

Learn about [cancellation tokens](12-cancellation-tokens.html) for stopping long-running requests.

---

[Previous: Files and Downloads](10-files-and-downloads.html) | [Next: Cancellation Tokens](12-cancellation-tokens.html)
