---
title: "How to Send a curl HEAD Request (-I) in C#"
description: "curl HEAD request in C#: run curl -I with CurlDotNet to read status and headers without a body. Check file size, existence or Last-Modified."
---

# How to send a curl HEAD request in C#

To send a curl HEAD request in C#, run `curl -I <url>` with `Curl.ExecuteAsync`, or call `CurlRequestBuilder.Head(url)`. You get the status code and headers; the body is always empty.

```bash
dotnet add package CurlDotNet
```

## The curl command

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -I https://httpbin.org/get");

Console.WriteLine(result.StatusCode);                   // 200
Console.WriteLine(result.GetHeader("Content-Length"));  // size the GET would return
Console.WriteLine(string.IsNullOrEmpty(result.Body));   // True
```

`-X HEAD` and `--head` do the same. Even if a misbehaving server sends a body with a HEAD response, CurlDotNet discards it.

## The fluent builder and LibCurl

```csharp
using CurlDotNet.Core;
using CurlDotNet.Lib;

var viaBuilder = await CurlRequestBuilder.Head("https://httpbin.org/get").ExecuteAsync();

using var curl = new LibCurl();
var viaLib = await curl.HeadAsync("https://httpbin.org/get");

Console.WriteLine($"{viaBuilder.StatusCode} {viaLib.StatusCode}");
```

## Check whether a URL exists

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -I -L https://httpbin.org/status/404");
Console.WriteLine(result.IsSuccess ? "exists" : $"missing ({result.StatusCode})");
```

Add `-L` so a redirect to the real location is followed.

## Check size before downloading

```csharp
using CurlDotNet;

var head = await Curl.ExecuteAsync("curl -I -L https://httpbin.org/image/png");
if (long.TryParse(head.GetHeader("Content-Length"), out var bytes) && bytes < 10_000_000)
{
    await Curl.ExecuteAsync("curl -L -o image.png https://httpbin.org/image/png");
}
```

Not every server sends `Content-Length` (chunked responses usually do not), so handle a missing header.

## Useful headers to read

| Header | Use |
|---|---|
| `Content-Length` | Size in bytes |
| `Content-Type` | What a GET would return |
| `Last-Modified` / `ETag` | Has the resource changed? |
| `Location` | Redirect target (without `-L`) |
| `Accept-Ranges` | Whether `-r` byte ranges are supported |

Header lookups are case-insensitive: `result.Headers["etag"]` and `result.GetHeader("ETag")` are equivalent.

## HEAD versus -i

`-I` sends a HEAD request. `-i` sends the normal request and also prepends the status line and headers to `result.Body`. Use `-I` when you only need the metadata.

## Related

- [Custom headers and reading response headers](../curl-custom-headers-user-agent-csharp/README.md)
- [Download a file with curl -o](../curl-download-file-csharp/README.md)
- [Follow redirects](../curl-follow-redirects-csharp/README.md)
- [All how-to guides](../README.md)
