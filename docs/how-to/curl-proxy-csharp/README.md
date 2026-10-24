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

`-x proxy.internal:8080` without a scheme also works; CurlDotNet assumes `http://`, as curl does, and a proxy given without a port uses curl's default of 1080. Plain `http://` requests are sent to the proxy with the absolute URL and `Proxy-Connection: Keep-Alive`, like curl; `https://` requests go through a `CONNECT` tunnel. `-p` / `--proxytunnel` forces a tunnel for `http://` URLs too (.NET 5 and later), and `--proxy-header` adds headers meant only for the proxy.

## Authenticated proxy

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("""
    curl -x http://proxy.internal:8080 \
         --proxy-user alice:s3cret \
         https://httpbin.org/ip
    """);
```

`-U` is the short form of `--proxy-user`, and `-x http://alice:s3cret@proxy.internal:8080` works too. The credentials are sent preemptively as Basic. For proxies that want a challenge, add `--proxy-digest`, `--proxy-ntlm`, `--proxy-negotiate` or `--proxy-anyauth`; Digest, NTLM and anyauth are negotiated in-library for plain `http://` requests through the proxy.

## SOCKS5

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl --socks5 127.0.0.1:1080 https://httpbin.org/ip");
```

`--socks4`, `--socks4a`, `--socks5-hostname` and `-x socks5h://host:port` are accepted as well. SOCKS support comes from .NET's own handler, which handles `socks4://`, `socks4a://` and `socks5://` proxies from .NET 6 onward. On .NET Framework (the net472 and net48 builds), SOCKS proxies are not available; use an HTTP proxy there.

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

Without `-x` or `--socks5`, CurlDotNet reads the proxy environment variables the way curl does: `http_proxy` (lower case only, as curl ignores `HTTP_PROXY`), `https_proxy` or `HTTPS_PROXY`, and `all_proxy` or `ALL_PROXY`. Hosts listed in `--noproxy` or `no_proxy` / `NO_PROXY` (names, domain suffixes, IP addresses, CIDR ranges, or `*`) go direct. `-x ""` turns proxies off for one command. When none of these variables is set, the request uses the shared `HttpClient`, which keeps .NET's default proxy behavior (the system proxy settings on Windows).

## How it works

A proxy, whether from `-x`, `--socks5` or the environment, makes CurlDotNet build a dedicated handler for that request with the proxy configured. Other requests keep the pooled client. A proxy failure names the proxy, not the target: a proxy host that does not resolve throws `CurlCouldntResolveProxyException` (exit code 5, where an unresolvable target host without a proxy is `CurlCouldntResolveHostException`, 6), and a proxy that refuses the connection throws `CurlCouldntConnectException` (7). See [error handling](../curl-error-handling-csharp/README.md).

## Debugging

Add `--trace trace.txt` to write a trace file that records which proxy (explicit or system) the request went through. `-v` adds the verbose request and response log to the start of `result.Body`.

The proxy behavior on this page was checked against the 9.8.0 source and its tests (which run against an in-process proxy stub), not against a live corporate proxy server.

## Related

- [Ignore SSL errors with -k](../curl-ignore-ssl-certificate-csharp/README.md)
- [Timeouts](../curl-timeout-csharp/README.md)
- [Basic authentication](../curl-basic-auth-csharp/README.md)
- [All how-to guides](../README.md)
