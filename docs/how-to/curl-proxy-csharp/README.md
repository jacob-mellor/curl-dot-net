---
title: "How to Use a curl Proxy (-x) in C#"
description: "Use a curl proxy in C#: pass -x, --proxy-user or --socks5 to CurlDotNet, or call WithProxy on the builder or LibCurl. HTTP and SOCKS5 proxies."
---

# How to use a curl proxy in C#

To send a request through a proxy in C# the curl way, add `-x http://proxy-host:port` to the command you pass to `Curl.ExecuteAsync`, with `--proxy-user user:password` if the proxy needs credentials. The builder equivalent is `.WithProxy(url)` or `.WithProxy(url, user, password)`.

```bash
dotnet add package CurlDotNet
```

## The curl command

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -x http://proxy.internal:8080 https://httpbin.org/ip");

Console.WriteLine(result.Body);  // shows the proxy's public IP, not yours
```

`-x proxy.internal:8080` without a scheme also works; CurlDotNet assumes `http://`, as curl does.

## Authenticated proxy

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("""
    curl -x http://proxy.internal:8080 \
         --proxy-user alice:s3cret \
         https://httpbin.org/ip
    """);
```

## SOCKS5

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl --socks5 127.0.0.1:1080 https://httpbin.org/ip");
```

SOCKS support comes from .NET's own `HttpClientHandler`, which handles `socks5://` proxies from .NET 6 onward. On .NET Framework (the net472 and net48 builds), SOCKS proxies are not available; use an HTTP proxy there.

## The fluent builder

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/ip")
    .WithProxy("http://proxy.internal:8080", "alice", "s3cret")
    .ExecuteAsync();

Console.WriteLine(result.StatusCode);
```

For every request from one instance, `LibCurl` has `WithProxy(url, user, pass)`:

```csharp
using CurlDotNet.Lib;

using var curl = new LibCurl();
curl.WithProxy("http://proxy.internal:8080");
var result = await curl.GetAsync("https://httpbin.org/ip");
```

## When you do not pass -x

Requests without `-x` or `--socks5` use the shared `HttpClient`, which follows .NET's default proxy behavior. On .NET Core and .NET 5+ that means the `HTTPS_PROXY` / `HTTP_PROXY` / `NO_PROXY` environment variables, or the system proxy settings on Windows.

## How it works

`-x` and `--socks5` make CurlDotNet build a dedicated `HttpClientHandler` for that request with a `WebProxy` (and `NetworkCredential` when `--proxy-user` is given). Other requests keep the pooled client. A proxy that cannot be reached surfaces as a `CurlException`; see [error handling](../curl-error-handling-csharp/README.md).

## Debugging

Add `--trace trace.txt` to write a trace file that records which proxy (explicit or system) the request went through. `-v` adds the verbose request and response log to the start of `result.Body`.

The proxy behavior on this page was checked against the 9.7.0 source and its option parsing, not against a live proxy server.

## Related

- [Ignore SSL errors with -k](../curl-ignore-ssl-certificate-csharp/README.md)
- [Timeouts](../curl-timeout-csharp/README.md)
- [Basic authentication](../curl-basic-auth-csharp/README.md)
- [All how-to guides](../README.md)
