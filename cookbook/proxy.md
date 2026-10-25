---
title: "curl Proxy in C# (curl -x and SOCKS5) with CurlDotNet"
description: "Send a curl request through a proxy in C# with curl -x, -U, --noproxy, proxy environment variables and --socks5 in CurlDotNet, plus the CurlRequestBuilder and LibCurl WithProxy methods."
---

# curl proxy in C# (curl -x and SOCKS5)

To run curl through a proxy in C#, add `-x` (or `--proxy`) with the proxy URL to the command you pass to `Curl.ExecuteAsync`. `-U` / `--proxy-user` adds proxy credentials, `--noproxy` lists hosts to reach directly, `--socks5` routes the request through a SOCKS5 proxy, and the `http_proxy`/`https_proxy`/`all_proxy`/`NO_PROXY` environment variables are honoured exactly as curl honours them.

## HTTP proxy

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -x http://proxy.example.com:8080 https://httpbin.org/ip");

Console.WriteLine(result.Body);   // {"origin": "<proxy's address>"}
```

If the proxy URL has no scheme, `http://` is assumed, and if it has no port, curl's default proxy port 1080 is used (so write the port, `:8080` or `:3128`, for most HTTP proxies).

For an `http://` URL the request goes to the proxy with the absolute URL in the request line and a `Proxy-Connection: Keep-Alive` header, byte-for-byte what curl sends. An `https://` URL is tunnelled through the proxy with `CONNECT`.

## Proxy that needs a login

```csharp
var result = await Curl.ExecuteAsync(@"
    curl -x http://proxy.example.com:8080 \
         --proxy-user alice:s3cret \
         https://httpbin.org/ip");

Console.WriteLine(result.StatusCode);
```

The credentials can also go in the proxy URL, `-x http://alice:s3cret@proxy.example.com:8080`. `-U` splits at the first colon. Either way they are sent pre-emptively as `Proxy-Authorization: Basic ...`.

When the proxy wants a challenge-based scheme, add `--proxy-digest`, `--proxy-ntlm` or `--proxy-anyauth` (negotiated in-library for plain HTTP requests through the proxy, the same way `--digest` and `--ntlm` work for servers); `--proxy-basic` and `--proxy-negotiate` are accepted too:

```csharp
var result = await Curl.ExecuteAsync(
    "curl -x http://proxy.example.com:8080 -U alice:s3cret --proxy-anyauth http://example.com/");
```

## Proxy environment variables and --noproxy

Without `-x`, CurlDotNet reads the same environment variables curl does: `http_proxy` for `http://` URLs (curl ignores upper-case `HTTP_PROXY` for safety, and so does CurlDotNet's own proxy selection), `https_proxy` or `HTTPS_PROXY` for `https://` URLs, and `all_proxy`/`ALL_PROXY` as the fallback. `no_proxy`/`NO_PROXY` and the `--noproxy` option list hosts that are contacted directly: host names (which also match their subdomains), IP addresses, CIDR ranges such as `10.0.0.0/8`, or `*` for everything.

```csharp
// Use the corporate proxy, except for internal hosts
var result = await Curl.ExecuteAsync(
    "curl -x http://proxy.example.com:8080 --noproxy 'localhost,127.0.0.1,.internal.example.com,10.0.0.0/8' https://httpbin.org/ip");
```

`--noproxy ''` overrides `NO_PROXY` and proxies everything; `-x ''` tells CurlDotNet not to pick a proxy from the environment variables. When CurlDotNet selects no proxy at all, the request uses .NET's default `HttpClient` proxy handling (`HttpClient.DefaultProxy`), which on Windows follows the system proxy settings.

## Tunnels and proxy headers

- `-p` / `--proxytunnel` tunnels even `http://` URLs through the proxy with `CONNECT` (on .NET 5 and later; .NET Framework and .NET Standard 2.0 builds cannot open the tunnel themselves).
- `--proxy1.0 HOST:PORT` uses the proxy with HTTP/1.0 for the `CONNECT` request.
- `--proxy-header 'X-Proxy-Token: abc'` sends an extra header meant for the proxy.

## SOCKS5 proxy

```csharp
var result = await Curl.ExecuteAsync(
    "curl --socks5 127.0.0.1:1080 https://httpbin.org/ip");

Console.WriteLine(result.Body);
```

`--socks4`, `--socks4a` and `--socks5-hostname` select the other SOCKS flavours, and `-x socks5://127.0.0.1:1080` (or `socks4://`, `socks4a://`, `socks5h://`) does the same through `-x`. SOCKS uses the SOCKS support built into .NET's `HttpClient` handler.

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

Proxy failures carry the same exit codes as curl, and the message names the proxy rather than the target:

| Exception | Exit code | Meaning |
|-----------|-----------|---------|
| `CurlCouldntResolveProxyException` | 5 | The proxy's host name could not be resolved (or the proxy string is not a usable address). Has `ProxyHost`. |
| `CurlCouldntResolveHostException` | 6 | The target host could not be resolved. |
| `CurlCouldntConnectException` | 7 | The proxy refused the connection, or the tunnel was refused. `ViaProxy` is true when the proxy was involved. |

```csharp
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl -x http://127.0.0.1:9 https://httpbin.org/ip");
}
catch (CurlCouldntResolveProxyException ex)
{
    Console.WriteLine($"Unknown proxy host {ex.ProxyHost}");
}
catch (CurlCouldntConnectException ex) when (ex.ViaProxy)
{
    Console.WriteLine($"Proxy refused the connection: {ex.Message}");
}
```

A proxy that answers `407 Proxy Authentication Required` to a `-p` tunnel is reported as `CurlCouldntConnectException` with a hint to pass `-U`. Add `-v` to the command to see which proxy was used; the verbose log is prepended to `result.Body`. See the [curl exit code reference](../reference/curl-exit-codes.html) for the full list.

## Gotchas

- An HTTPS target through an HTTP proxy uses `CONNECT` tunnelling automatically; you do not need `-p`.
- Inspecting traffic with Fiddler, Charles or mitmproxy? Combine `-x http://127.0.0.1:8888` with `-k` so the tool's certificate is accepted (development only). Better, trust the tool's root certificate with `--cacert fiddler-root.pem` (.NET 5 and later), which keeps verification on.

## Related

- [curl timeout in C#](timeouts.html) - slow proxies need a sensible `--max-time`
- [DNS and connection errors](../exceptions/dns-errors.html)
- [SSL errors](../exceptions/ssl-errors.html) - about `-k`
- [Cookbook index](index.html)
