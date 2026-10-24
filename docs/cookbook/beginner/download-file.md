---
title: "curl Download File in C# (curl -o) with CurlDotNet"
description: "How to curl download a file in C# with curl -o, -O, -J, Curl.DownloadAsync and result.SaveToFile. Covers --output-dir, resuming with -C -, redirects (-L), byte ranges (-r) and --compressed."
---

# curl download file in C# (curl -o)

This recipe shows how to curl download a file in C# with CurlDotNet: the same `curl -o file url` command you would type in a shell, run from .NET and saved to disk. It also covers `-O` and `-J`, curl's output-file options, resuming with `-C -`, the `Curl.DownloadAsync` shortcut, downloading into memory, redirecting download links, byte ranges and compressed transfers.

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

`result.OutputFiles` lists every file the command wrote. As with the curl CLI, nothing goes to standard output when `-o` or `-O` is used, so `result.Body` is empty: read the file (or `result.OutputFiles`) instead. Check `result.IsSuccess` (HTTP 2xx) before trusting the file: without `-f` a 404 still produces a result (and the error page is saved), it does not throw.

`-O` (capital O) keeps the file name from the URL, exactly like the curl CLI:

```csharp
// Saves as "png" (the last path segment of the URL)
var result = await Curl.ExecuteAsync("curl -O https://httpbin.org/image/png");
```

### Choose where and how the file is written

The curl options that control the output file all work:

| Option | Effect |
|--------|--------|
| `-J` / `--remote-header-name` (with `-O`) | Use the file name from the `Content-Disposition` header. Never overwrites an existing file. |
| `--output-dir DIR` | Put `-o`/`-O` files in `DIR`. |
| `--create-dirs` | Create missing directories in the output path. |
| `--no-clobber` | Do not overwrite: `report.txt` becomes `report-1.txt`, `report-2.txt`, ... |
| `--skip-existing` | Do not even send the request when the target file already exists. |
| `-R` / `--remote-time` | Set the file's modification time from the server's `Last-Modified`. |
| `--remove-on-error` | Delete the partly written file if the transfer fails. |
| `-D FILE` / `--dump-header FILE` | Write the response headers to `FILE` (`-D -` puts them in `result.Body`). |
| `--max-filesize BYTES` | Refuse bigger downloads with `CurlFileSizeExceededException` (exit code 63). |

```csharp
// The server answers with Content-Disposition: attachment; filename="report.pdf"
var result = await Curl.ExecuteAsync(
    "curl -L -O -J --output-dir downloads --create-dirs -R https://example.com/export/42");

Console.WriteLine(result.OutputFiles[0]);   // downloads/report.pdf
```

`-i -o file` writes the response headers into the file in front of the body, as curl does.

### Resume an interrupted download (-C -)

`-C -` looks at the size of the existing file, asks the server for the rest with a `Range` header and appends to the file. `-C 1024` resumes from an explicit byte offset.

```csharp
var result = await Curl.ExecuteAsync(
    "curl -C - -o big.bin https://httpbin.org/range/1048576");
```

If the server ignores the range and sends the whole file again, CurlDotNet throws `CurlRangeErrorException` (exit code 33) instead of corrupting the file.

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

Without `-o`, the body stays in the result. Binary responses (detected from the Content-Type and the bytes themselves) are in `result.BinaryData`; text responses are in `result.Body`.

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

`--compressed` sends `Accept-Encoding` and decodes gzip, deflate and brotli responses before they are written, so the file on disk is the plain content. Without it nothing is decoded, as in curl. Builder: `.Compressed()`.

### Set a timeout for large files

There is no timeout unless you ask for one, so a large download is never cut off by a hidden default. Add one when a stalled transfer should fail:

```csharp
using CurlDotNet.Core;

var settings = new CurlSettings().WithTimeout(300);
var result = await Curl.ExecuteAsync(
    "curl -L -o big.iso https://httpbin.org/bytes/1048576", settings);
```

The command-string form is `-m 300`; it covers the whole transfer, body included. For flaky networks add `--retry 3` (see [Retry](../retry.html)). See [Timeouts](../timeouts.html).

### Download with authentication

```csharp
var result = await Curl.ExecuteAsync(
    "curl -u user:passwd -o report.csv https://httpbin.org/basic-auth/user/passwd");
```

Bearer tokens work the same way with `-H "Authorization: Bearer TOKEN"`. See [Authentication schemes](../auth-schemes.html).

### Download several files

One command can name several URLs, each paired with its own `-o`, exactly like curl; they run one after another:

```csharp
var result = await Curl.ExecuteAsync(
    "curl -o a.png https://httpbin.org/image/png -o b.jpg https://httpbin.org/image/jpeg");

Console.WriteLine(string.Join(", ", result.OutputFiles));   // a.png, b.jpg
```

Separate commands work too:

```csharp
var results = await Curl.ExecuteManyAsync(
    "curl -o a.png https://httpbin.org/image/png",
    "curl -o b.jpg https://httpbin.org/image/jpeg");

foreach (var r in results)
    Console.WriteLine($"{r.StatusCode} {string.Join(",", r.OutputFiles)}");
```

URL globs such as `https://example.com/img[1-3].png -o 'img#1.png'` are covered in [Multiple URLs and globbing](../multiple-urls-and-globbing.html).

## Troubleshooting

**The file is empty or contains HTML.** The URL redirected and you did not pass `-L`, or the server returned an error page. Print `result.StatusCode` and `result.GetHeader("Location")`.

**The image or zip is corrupt.** The server sent a misleading Content-Type and the body was decoded as text. Use the builder with `.AsBinary()`, or check `result.IsBinary` before calling `SaveToFile`.

**`result.Body` is empty after `-o`.** That is curl's behaviour: the body went to the file. Read `result.OutputFiles[0]`.

**`CurlWriteErrorException` (exit code 23).** The output directory does not exist or the file cannot be written. Add `--create-dirs`, fix the path, or check permissions. `-J` and `--no-clobber` also refuse to overwrite existing files this way.

**`CurlOperationTimeoutException`.** The transfer took longer than `-m`. Raise it or remove it for large files.

**`CurlCouldntResolveHostException` or `CurlCouldntConnectException`.** The host name could not be resolved (exit code 6) or the connection was refused (exit code 7). Check the host and your proxy settings. A certificate problem is a separate TLS exception (`CurlSslConnectErrorException` or `CurlPeerFailedVerificationException`); use `-k` only for a self-signed certificate in development.

**`IsSuccess` is false but nothing threw.** That is by design: without `-f`, HTTP 4xx/5xx come back as results. Call `result.EnsureSuccess()`, or add `-f` to get `CurlHttpReturnedErrorException` (exit code 22) and no error page on disk. See [Handle errors](handle-errors.html) and the [curl exit code reference](../../reference/curl-exit-codes.html).

## Related recipes

- [Simple GET request](simple-get.html)
- [Upload a file](upload-file.html)
- [Handle errors](handle-errors.html)
- [Follow redirects](../follow-redirects.html)
- [Timeouts](../timeouts.html)
- [HEAD request (check size before downloading)](../head-request.html)
- [Cookbook index](../index.html)
- Tutorial: [Files and downloads](../../tutorials/10-files-and-downloads.html)
