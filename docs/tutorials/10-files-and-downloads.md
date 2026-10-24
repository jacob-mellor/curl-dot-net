---
title: "Download and Upload Files with curl in C# (-o, -O, -F)"
description: "curl download file in C# (curl -o) and upload with -F using CurlDotNet: Curl.DownloadAsync, SaveToFile, BinaryData, -O, ranges and multipart uploads."
---

# Tutorial 10: curl download file in C# (curl -o) and upload with -F

This tutorial shows how to curl download file in C# (curl -o) and upload with -F using CurlDotNet. You will save responses to disk, keep the remote file name with `-O`, work with binary data in memory, resume partial downloads with `-r`, and upload files as multipart forms.

If you have not read [Tutorial 5: Understanding Results](05-understanding-results.html) yet, do that first; this page builds on `CurlResult`.

## Downloading Files

### Simple file download (-o)

In curl you write `curl -o image.png https://httpbin.org/image/png`. CurlDotNet runs exactly that string and records where the file went:

```csharp
using CurlDotNet;

// Same as: curl -o image.png https://httpbin.org/image/png
var result = await Curl.ExecuteAsync("curl -o image.png https://httpbin.org/image/png");

Console.WriteLine(result.StatusCode);                     // 200
Console.WriteLine($"Saved: {result.OutputFiles[0]}");     // Saved: image.png

// Convenience helper with the same effect
await Curl.DownloadAsync("https://httpbin.org/image/png", "image.png");
```

The builder version uses `WithOutput`:

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/image/png")
    .WithOutput("image.png")
    .ExecuteAsync();
```

### Keep the remote file name (-O)

`-O` (capital O) names the local file after the last segment of the URL, just like curl:

```csharp
// Same as: curl -O https://httpbin.org/robots.txt   -> writes ./robots.txt
var result = await Curl.ExecuteAsync("curl -O https://httpbin.org/robots.txt");

Console.WriteLine(string.Join(", ", result.OutputFiles));   // robots.txt
```

### Download to memory

When you leave out `-o`, the response stays in the result. Text lands in `Body`; anything binary lands in `BinaryData`:

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/image/png");

if (result.IsBinary)
{
    byte[] bytes = result.BinaryData;
    Console.WriteLine($"{bytes.Length} bytes of {result.GetHeader("Content-Type")}");

    result.SaveToFile("image.png");            // or: await result.SaveToFileAsync("image.png")
}
```

For a text file the same pattern works through `Body`:

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/robots.txt");

Console.WriteLine(result.Body);                // the file contents as text
await result.SaveToFileAsync("robots.txt");
```

Binary detection looks at the Content-Type header and, when that is missing or misleading, at the bytes themselves. If you need a stream instead of an array, call `result.ToStream()`.

## Uploading Files

### Simple file upload (-F)

curl's `-F field=@path` sends a `multipart/form-data` POST with the file in the named field. httpbin echoes back what it received, which makes it a handy test target:

```csharp
// Same as: curl -F 'file=@report.pdf' https://httpbin.org/post
var result = await Curl.ExecuteAsync(
    "curl -F 'file=@report.pdf' https://httpbin.org/post");

Console.WriteLine(result.StatusCode);   // 200
Console.WriteLine(result.Body);         // JSON containing a "files" section
```

### File plus form fields

Add one `-F` per field. Quote values that contain spaces:

```csharp
var result = await Curl.ExecuteAsync(@"curl \
  -F 'document=@report.pdf' \
  -F 'description=Monthly report' \
  -F 'category=reports' \
  https://httpbin.org/post");
```

The builder has two ways to express the same request. `WithFile` adds one file; `WithMultipartForm` takes fields and files as dictionaries:

```csharp
var fields = new Dictionary<string, string>
{
    ["description"] = "Monthly report",
    ["category"] = "reports"
};
var files = new Dictionary<string, string> { ["document"] = "report.pdf" };

var result = await CurlRequestBuilder
    .Post("https://httpbin.org/post")
    .WithMultipartForm(fields, files)      // or: .WithFile("document", "report.pdf")
    .ExecuteAsync();
```

### Multiple files

Repeat `-F` for each file. Building the command from a list keeps it readable:

```csharp
var paths = new[] { "a.jpg", "b.jpg", "c.jpg" };
var parts = string.Join(" ", paths.Select(p => $"-F 'photos=@{p}'"));

var result = await Curl.ExecuteAsync($"curl {parts} https://httpbin.org/post");
Console.WriteLine(result.IsSuccess ? "Uploaded" : $"HTTP {result.StatusCode}");
```

## Partial and Resumed Downloads (-r)

A range request asks for part of a file. The server answers `206 Partial Content` when it honours the range:

```csharp
// Same as: curl -r 0-1023 -o first-kb.bin https://httpbin.org/range/4096
var result = await Curl.ExecuteAsync(
    "curl -r 0-1023 -o first-kb.bin https://httpbin.org/range/4096");

Console.WriteLine(result.StatusCode);   // 206
```

To resume an interrupted download, ask for everything from the bytes you already have and append:

```csharp
const string path = "big.bin";
long have = File.Exists(path) ? new FileInfo(path).Length : 0;

var result = await Curl.ExecuteAsync($"curl -r {have}- https://httpbin.org/range/4096");

if (result.StatusCode == 206)
{
    using var fs = new FileStream(path, FileMode.Append);
    fs.Write(result.BinaryData, 0, result.BinaryData.Length);
}
else if (result.StatusCode == 200)
    result.SaveToFile(path);   // server ignored the range: start over
```

## Verifying a Download

Hash the bytes before you keep them. SHA-256 is the usual choice today:

```csharp
using System.Security.Cryptography;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/image/png");
var hash = Convert.ToHexString(SHA256.HashData(result.BinaryData));

if (hash.Equals(expectedSha256, StringComparison.OrdinalIgnoreCase))
    result.SaveToFile("image.png");
else
    Console.WriteLine($"Checksum mismatch: {hash}");
```

## Temporary Files

Download to a temp path, process it, and always delete it:

```csharp
var tempFile = Path.GetTempFileName();
try
{
    await Curl.DownloadAsync("https://httpbin.org/robots.txt", tempFile);
    Console.WriteLine(File.ReadAllText(tempFile));
}
finally
{
    File.Delete(tempFile);
}
```

## Error Handling for File Operations

A missing upload file is detected before anything is sent and throws `CurlFileCouldntReadException`:

```csharp
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl -F 'file=@missing.pdf' https://httpbin.org/post");
}
catch (CurlFileCouldntReadException ex)
{
    Console.WriteLine($"Upload file not found: {ex.Message}");
}
```

Large downloads need a longer deadline. Set it with `-m` in the command or with `CurlSettings.WithTimeout` (seconds):

```csharp
var settings = new CurlSettings().WithTimeout(300);

var result = await Curl.ExecuteAsync(
    "curl -o big.bin https://httpbin.org/bytes/1048576", settings);

Console.WriteLine(result.IsSuccess ? "Downloaded" : $"HTTP {result.StatusCode}");
```

Remember that HTTP 4xx and 5xx responses do not throw; check `result.IsSuccess` or call `result.EnsureSuccess()`. Timeouts surface as `CurlOperationTimeoutException` (see [Tutorial 12](12-cancellation-tokens.html)).

## Best Practices

1. **Use `-o` or `DownloadAsync` for anything large** - it writes straight to disk instead of holding the body in memory.
2. **Check `IsBinary` before treating a response as text** - `Body` is for text, `BinaryData` is for everything else.
3. **Verify checksums when the publisher provides them.**
4. **Use `-r` to resume** - but handle the `200` case where the server ignores the range.
5. **Delete temporary files in a `finally` block.**
6. **Set a generous `-m` for big transfers** - the default timeout is tuned for API calls, not large downloads.
7. **Quote `-F` values that contain spaces** and use `@` only for file paths.
8. **Catch `CurlFileCouldntReadException`** to give a clear message when an upload path is wrong.

## Summary

- `curl -o file url` and `Curl.DownloadAsync(url, path)` save a download to disk; `-O` keeps the remote name.
- `result.BinaryData`, `result.IsBinary` and `result.SaveToFile(path)` handle downloads kept in memory.
- `-F 'field=@path'` uploads files; the builder offers `WithFile` and `WithMultipartForm`.
- `-r` requests byte ranges for partial and resumed downloads.

See also the cookbook recipes [Download a File](../cookbook/beginner/download-file.html) and [Upload a File](../cookbook/beginner/upload-file.html).

## What's Next?

Learn about [working with forms and data](11-forms-and-data.html) in the next tutorial.

---

[Previous: Authentication Basics](09-authentication-basics.html) | [Next: Forms and Data](11-forms-and-data.html)
