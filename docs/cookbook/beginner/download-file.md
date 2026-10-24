---
title: "curl Download File in C# (curl -o) with CurlDotNet"
description: "How to curl download a file in C# with curl -o, -O, Curl.DownloadAsync and result.SaveToFile. Covers redirects (-L), byte ranges (-r) and --compressed."
---

# curl download file in C# (curl -o)

This recipe shows how to curl download a file in C# with CurlDotNet: the same `curl -o file url` command you would type in a shell, run from .NET and saved to disk. It also covers `-O`, the `Curl.DownloadAsync` shortcut, downloading into memory, redirecting download links, byte ranges and compressed transfers.

## The problem

You have a URL that points at a file (an image, a PDF, a zip, a CSV export) and you want it on disk, or in a byte array, from C# code.

## Solution 1: the curl command string

`-o` writes the response body to the path you give. The leading `curl` is optional.

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -o logo.png https://httpbin.org/image/png");

Console.WriteLine(result.IsSuccess);          // True
Console.WriteLine(result.OutputFiles[0]);     // logo.png
```

`result.OutputFiles` lists every file the command wrote. Check `result.IsSuccess` (HTTP 2xx) before trusting the file: a 404 still produces a result, it does not throw.

`-O` (capital O) keeps the file name from the URL, exactly like the curl CLI:

```csharp
// Saves as "png" (the last path segment of the URL)
var result = await Curl.ExecuteAsync("curl -O https://httpbin.org/image/png");
```

### The shortcut: Curl.DownloadAsync

When you only have a URL and a path, skip the command string:

```csharp
using CurlDotNet;

var result = await Curl.DownloadAsync("https://httpbin.org/image/png", "logo.png");
Console.WriteLine($"{result.StatusCode} -> {string.Join(", ", result.OutputFiles)}");
```

A synchronous `Curl.Download(url, path)` also exists for console tools and scripts.

## Solution 2: the request builder

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/image/png")
    .WithOutput("logo.png")
    .ExecuteAsync();
```

`WithOutput(path)` is the builder's equivalent of `-o`. Call `.ToCurlCommand()` on the builder to see the curl string it will run.

## Download into memory, then save

Without `-o`, the body stays in the result. Binary responses (detected from the Content-Type) are in `result.BinaryData`; text responses are in `result.Body`.

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/image/png");

if (result.IsBinary)
    Console.WriteLine($"{result.BinaryData.Length} bytes");

result.SaveToFile("logo.png");           // writes BinaryData or Body as appropriate
await result.SaveToFileAsync("copy.png"); // async variant
```

If a server labels a binary file as `text/plain`, force binary handling with the builder's `.AsBinary()` so the bytes are not decoded as text.

## Variations

### Follow redirecting download links (-L)

GitHub release assets, CDNs and "latest" links usually answer with a 302. Add `-L` or the request stops at the redirect and you save an empty or HTML file.

```csharp
var result = await Curl.ExecuteAsync(
    "curl -L -o release.zip https://github.com/jacob-mellor/curl-dot-net/archive/refs/heads/master.zip");

Console.WriteLine(result.StatusCode);   // 200 after following the redirect
```

Builder: `.FollowRedirects()`. See [Follow redirects](../follow-redirects.html).

### Download part of a file (-r)

`-r start-end` sends a `Range` header and returns HTTP 206 with only those bytes.

```csharp
// First 1024 bytes only
var result = await Curl.ExecuteAsync(
    "curl -r 0-1023 -o head.bin https://httpbin.org/range/4096");

Console.WriteLine(result.StatusCode);   // 206
```

Builder: `.WithRange("0-1023")`.

### Ask for a compressed transfer (--compressed)

```csharp
var result = await Curl.ExecuteAsync(
    "curl --compressed -o data.json https://httpbin.org/gzip");
```

The response is decompressed before it is written, so the file on disk is the plain content. Builder: `.Compressed()`.

### Set a timeout for large files

```csharp
using CurlDotNet.Core;

var settings = new CurlSettings().WithTimeout(300);
var result = await Curl.ExecuteAsync(
    "curl -L -o big.iso https://httpbin.org/bytes/1048576", settings);
```

The command-string form is `-m 300`. See [Timeouts](../timeouts.html).

### Download with authentication

```csharp
var result = await Curl.ExecuteAsync(
    "curl -u user:passwd -o report.csv https://httpbin.org/basic-auth/user/passwd");
```

Bearer tokens work the same way with `-H "Authorization: Bearer TOKEN"`. See [Authentication schemes](../auth-schemes.html).

### Download several files

```csharp
var results = await Curl.ExecuteManyAsync(
    "curl -o a.png https://httpbin.org/image/png",
    "curl -o b.jpg https://httpbin.org/image/jpeg");

foreach (var r in results)
    Console.WriteLine($"{r.StatusCode} {string.Join(",", r.OutputFiles)}");
```

## Troubleshooting

**The file is empty or contains HTML.** The URL redirected and you did not pass `-L`, or the server returned an error page. Print `result.StatusCode` and `result.GetHeader("Location")`.

**The image or zip is corrupt.** The server sent a misleading Content-Type and the body was decoded as text. Use the builder with `.AsBinary()`, or check `result.IsBinary` before calling `SaveToFile`.

**`CurlOperationTimeoutException`.** The transfer took longer than `-m`. Raise it or remove it for large files.

**`CurlCouldntConnectException`.** DNS, connection or TLS failure. Check the host, your proxy settings and whether you need `-k` for a self-signed certificate in development.

**`IsSuccess` is false but nothing threw.** That is by design: HTTP 4xx/5xx come back as results. Call `result.EnsureSuccess()` if you want an exception instead. See [Handle errors](handle-errors.html).

## Related recipes

- [Simple GET request](simple-get.html)
- [Upload a file](upload-file.html)
- [Handle errors](handle-errors.html)
- [Follow redirects](../follow-redirects.html)
- [Timeouts](../timeouts.html)
- [HEAD request (check size before downloading)](../head-request.html)
- [Cookbook index](../index.html)
- Tutorial: [Files and downloads](../../tutorials/10-files-and-downloads.html)
