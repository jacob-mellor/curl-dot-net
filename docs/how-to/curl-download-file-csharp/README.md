---
title: "How to Do a curl -o File Download in C#"
description: "curl -o download file in C#: save any URL to disk with CurlDotNet. Binary-safe for images, PDFs and .xlsx files; also -O, DownloadAsync and BinaryData."
---

# How to do a curl -o file download in C#

To download a file in C# the way `curl -o` does, run `curl -o <path> <url>` with `Curl.ExecuteAsync`, or call `Curl.DownloadAsync(url, path)`. The bytes are written exactly as received, so images, PDFs, ZIPs and Office files are not corrupted.

```bash
dotnet add package CurlDotNet
```

## The curl command

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -L -o logo.png https://httpbin.org/image/png");

Console.WriteLine(result.StatusCode);              // 200
Console.WriteLine(string.Join(", ", result.OutputFiles));  // logo.png
Console.WriteLine(new FileInfo("logo.png").Length);       // bytes on disk
```

Add `-L` when the download URL redirects (most CDN and release links do). `-O` saves under the remote file name (the last URL path segment) in the current directory.

## Helper method and fluent builder

```csharp
using CurlDotNet;
using CurlDotNet.Core;

await Curl.DownloadAsync("https://httpbin.org/image/jpeg", "photo.jpg");

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/image/png")
    .FollowRedirects()
    .WithOutput("logo.png")
    .ExecuteAsync();
```

## Why it is binary-safe

CurlDotNet always reads the response as raw bytes first, then decides whether it is text. Known binary MIME types (images, audio, video, fonts, PDF, ZIP, Office Open XML such as `.xlsx` and `.docx`) stay binary, and a missing `Content-Type` is treated as binary. Even a `text/*` response is checked byte by byte, so a file served with a misleading text type is not decoded into a corrupted string.

For binary responses `result.IsBinary` is `true`, `result.BinaryData` holds the bytes and `result.Body` is empty.

## Keep the bytes in memory instead

Skip `-o` and work with `BinaryData` directly, or save later:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/image/png");
if (result.IsSuccess && result.IsBinary)
{
    byte[] png = result.BinaryData;
    result.SaveToFile("copy.png");
    Console.WriteLine($"{png.Length} bytes");
}
```

`result.ToStream()` gives you a `Stream` over the same data for libraries that take one.

## Partial downloads

`-r` requests a byte range, and the server answers `206 Partial Content`:

```csharp
using CurlDotNet;

var part = await Curl.ExecuteAsync("curl -r 0-99 https://httpbin.org/range/1000");
Console.WriteLine($"{part.StatusCode} {part.BinaryData.Length}");  // 206 100
```

## Things to know

- The whole response is buffered in memory before it is written, so this suits files that fit comfortably in RAM. For multi-gigabyte downloads, stream with `HttpClient` directly.
- `-o` is applied whatever the status code, and `result.OutputFiles` lists the path even for a 404. Check `result.IsSuccess` before trusting the file.
- `-m` (default 30 seconds) limits only the wait for the response headers. Once the body starts arriving it is read to the end, so a large download is not cut off, but nor is a stalled one. See [timeouts](../curl-timeout-csharp/README.md).

## Related

- [Multipart upload with -F](../curl-file-upload-csharp/README.md)
- [Follow redirects with -L](../curl-follow-redirects-csharp/README.md)
- [HEAD request to check size first](../curl-head-request-csharp/README.md)
- [Error handling](../curl-error-handling-csharp/README.md)
- [All how-to guides](../README.md)
