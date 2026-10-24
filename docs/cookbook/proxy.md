---
title: "curl Proxy in C# (curl -x and SOCKS5) with CurlDotNet"
description: "Send a curl request through a proxy in C# with curl -x, --proxy-user and --socks5 in CurlDotNet, plus the CurlRequestBuilder and LibCurl WithProxy methods."
---

# curl proxy in C# (curl -x and SOCKS5)

To run curl through a proxy in C#, add `-x` (or `--proxy`) with the proxy URL to the command you pass to `Curl.ExecuteAsync`. `--proxy-user` adds proxy credentials and `--socks5` routes the request through a SOCKS5 proxy.

## HTTP proxy

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -x http://proxy.example.com:8080 https://httpbin.org/ip");

Console.WriteLine(result.Body);   // {"origin": "<proxy's address>"}
```

If the proxy URL has no scheme, `http://` is assumed.

## Proxy that needs a login

```csharp
var result = await Curl.ExecuteAsync(@"
    curl -x http://proxy.example.com:8080 \
         --proxy-user alice:s3cret \
         https://httpbin.org/ip");

Console.WriteLine(result.StatusCode);
```

## SOCKS5 proxy

```csharp
var result = await Curl.ExecuteAsync(
    "curl --socks5 127.0.0.1:1080 https://httpbin.org/ip");

Console.WriteLine(result.Body);
```

## Builder style

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Get("https://httpbin.org/ip")
    .WithProxy("http://proxy.example.com:8080", "alice", "s3cret")
    .ExecuteAsync();

Console.WriteLine(result.Body);
```

`WithProxy(url)` without credentials is the `-x` equivalent.

## Reusable client with a proxy

```csharp
using CurlDotNet.Lib;

using var client = new LibCurl()
    .WithProxy("http://proxy.example.com:8080", "alice", "s3cret")
    .WithTimeout(TimeSpan.FromSeconds(20));

var ip = await client.GetAsync("https://httpbin.org/ip");
Console.WriteLine(ip.Body);
```

## When the proxy is unreachable

A proxy that refuses the connection or cannot be resolved surfaces as `CurlCouldntConnectException`, the same exception you get for a direct connection failure:

```csharp
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl -x http://127.0.0.1:9 https://httpbin.org/ip");
}
catch (CurlCouldntConnectException ex)
{
    Console.WriteLine($"Could not connect: {ex.Message}");
}
```

Add `-v` to the command to see which proxy was used; the verbose log is prepended to `result.Body`.

## Gotchas

- An HTTPS target through an HTTP proxy uses `CONNECT` tunnelling automatically; you do not need `-p`.
- Inspecting traffic with Fiddler, Charles or mitmproxy? Combine `-x http://127.0.0.1:8888` with `-k` so the tool's certificate is accepted (development only).
- Without `-x`, CurlDotNet uses the system proxy settings, as the .NET runtime does.

## Related

- [curl timeout in C#](timeouts.html) - slow proxies need a sensible `--max-time`
- [DNS and connection errors](../exceptions/dns-errors.html)
- [SSL errors](../exceptions/ssl-errors.html) - about `-k`
- [Cookbook index](index.html)
