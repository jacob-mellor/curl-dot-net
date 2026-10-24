---
title: "curl -I HEAD Request in C# with CurlDotNet"
description: "How to send a curl -I HEAD request in C# with CurlDotNet to read response headers such as Content-Length and Last-Modified without downloading the body."
---

# curl -I HEAD request in C#

To send a curl `-I` HEAD request in C#, pass `curl -I <url>` to `Curl.ExecuteAsync`. The server returns only the status line and headers, so you can check a file's size, type or freshness without downloading it. As with the curl CLI, `-I` prints those headers, so they are in `result.Body` as well as in `result.Headers`.

## The curl command

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -I https://httpbin.org/image/png");

Console.WriteLine(result.StatusCode);                    // 200
Console.WriteLine(result.GetHeader("Content-Type"));     // image/png
Console.WriteLine(result.GetHeader("Content-Length"));   // 8090
Console.WriteLine(result.Body);                          // the status line and headers, as curl prints them
```

The parsed headers are in `result.Headers` (a case-insensitive dictionary, values exactly as the server sent them). There is never a response body with HEAD.

## See the raw header text

`result.Body` holds the header block exactly as curl prints it for `-I`, so no `-i` is needed:

```csharp
var result = await Curl.ExecuteAsync("curl -I https://httpbin.org/get");
Console.WriteLine(result.Body);
// HTTP/1.1 200 OK
// Content-Type: application/json
// ...
```

With `-L`, every hop's header block is printed, one after another. `-D headers.txt` writes the same text to a file.

## HEAD with a query string (-G -I)

`-I` cannot be combined with `-d`: curl refuses to send two kinds of request, and CurlDotNet throws a `CurlException` with exit code 2 ("You can only select one HTTP request method"). Add `-G` and the `-d` data moves into the query string of the HEAD request instead:

```csharp
// HEAD https://httpbin.org/get?id=42
var result = await Curl.ExecuteAsync("curl -G -I -d id=42 https://httpbin.org/get");
```

## Builder style

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Head("https://httpbin.org/image/png")
    .ExecuteAsync();

if (result.HasHeader("Content-Length"))
    Console.WriteLine($"{result.GetHeader("Content-Length")} bytes");
```

`CurlRequestBuilder.Request("HEAD", url)` is equivalent. Builder and `LibCurl` HEAD requests put the headers in `result.Headers` only. In a command string, `-X HEAD` sends the same request but, unlike `-I`, does not print the headers into `result.Body` (add `-i` for that); prefer `-I`, as curl's own documentation does.

## Reusable client

```csharp
using CurlDotNet.Lib;

using var client = new LibCurl().WithTimeout(TimeSpan.FromSeconds(10));

var head = await client.HeadAsync("https://httpbin.org/image/png");
Console.WriteLine(head.GetHeader("Content-Type"));
```

## Typical uses

Check whether a download is worth starting:

```csharp
var head = await Curl.ExecuteAsync("curl -I -L https://httpbin.org/redirect-to?url=/image/png");

if (long.TryParse(head.GetHeader("Content-Length"), out var size) && size < 10_000_000)
    await Curl.ExecuteAsync("curl -L -o image.png https://httpbin.org/redirect-to?url=/image/png");
```

Check that a URL is alive without paying for the body:

```csharp
var ping = await Curl.ExecuteAsync("curl -I -m 5 https://api.github.com");
Console.WriteLine(ping.IsSuccess ? "up" : $"down ({ping.StatusCode})");
```

## Gotchas

- Some servers answer HEAD with 405 or omit `Content-Length`. Fall back to `-r 0-0` (a one-byte range GET) to read headers from such servers.
- Redirects are not followed unless you add `-L`; without it a HEAD against a redirecting URL returns 301/302 and a `Location` header.
- `ParseJson<T>()` on a HEAD result throws `InvalidOperationException`: there is no JSON body.
- `-o file` with `-I` writes the header block to the file instead of `result.Body`.

## Related

- [curl follow redirects in C#](follow-redirects.html)
- [curl download file in C#](beginner/download-file.html)
- [Debugging requests with -v and --trace](../tutorials/14-debugging-requests.html)
- [Cookbook index](index.html)
