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

With `-o` or `-O`, `result.Body` is empty, because the curl CLI prints nothing to standard output when it writes to a file. Read the file, use `result.OutputFiles`, or (for a binary response) `result.BinaryData`. Add `-i` to have the header block written into the file before the body, as curl does.

## More output options

The curl CLI's file-output flags work the same way:

| Flag | What it does |
|---|---|
| `-J` / `--remote-header-name` (with `-O`) | Use the file name from the `Content-Disposition` header; never overwrites an existing file |
| `--output-dir <dir>` | Directory for `-o` / `-O` files |
| `--create-dirs` | Create missing directories in the output path |
| `--no-clobber` | Do not overwrite: save under a numbered name (`report-1.txt`, `report-2.txt`, ...) instead |
| `--skip-existing` | Skip the download when the output file already exists |
| `-R` / `--remote-time` | Set the file's modification time from `Last-Modified` |
| `--remove-on-error` | Delete a partially written file when the transfer fails |
| `-C <offset>` / `-C -` | Resume: `-C -` continues from the existing file's size and appends to it |
| `--max-filesize <bytes>` | Refuse larger files (`CurlFileSizeExceededException`, exit 63) |
| `-D <file>` / `-D -` | Save the response headers to a file, or put them in `result.Body` |
| `--etag-save` / `--etag-compare` | Download only when the resource's ETag changed |

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -L --create-dirs --output-dir downloads/2026 -O https://httpbin.org/image/png");

Console.WriteLine(string.Join(", ", result.OutputFiles));  // downloads/2026/png
```

A missing output directory without `--create-dirs` throws `CurlWriteErrorException` (exit 23). `-C -` throws `CurlRangeErrorException` (exit 33) when the server ignores the range request.

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
- There is no timeout unless you set one. `-m` caps the whole transfer, body included, so give large downloads a generous value, or use only `--connect-timeout` plus a `CancellationToken`. See [timeouts](../curl-timeout-csharp/README.md).
- To retry a flaky download, add `--retry <n>`; see [curl --retry in C#](../../cookbook/retry.md).

## Related

- [Multipart upload with -F](../curl-file-upload-csharp/README.md)
- [Follow redirects with -L](../curl-follow-redirects-csharp/README.md)
- [HEAD request to check size first](../curl-head-request-csharp/README.md)
- [Error handling](../curl-error-handling-csharp/README.md)
- [All how-to guides](../README.md)
