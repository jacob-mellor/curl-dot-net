---
title: "curl Could Not Resolve Host in C# (CurlDotNet)"
description: "Fix 'curl could not resolve host' and connection refused in C#: CurlDotNet throws CurlCouldntResolveHostException (6), CurlCouldntResolveProxyException (5) or CurlCouldntConnectException (7)."
---

# curl Could Not Resolve Host in C# (DNS and Connection Errors)

"curl could not resolve host" in C# means the host name in your URL did not resolve to an IP address, and "connection refused" means it resolved but nothing answered on that port. CurlDotNet tells these apart exactly as the curl tool does: `CurlCouldntResolveHostException` (exit code 6), `CurlCouldntResolveProxyException` (5) when the proxy's name is the one that failed, and `CurlCouldntConnectException` (7) when the connection itself failed. TLS failures have their own types (see [SSL/TLS errors](ssl-errors.html)). This page shows how to catch them and how to tell the causes apart.

## What Is Thrown

| Exception | `CurlErrorCode` | Thrown when | Properties |
|-----------|----:|-------------|------------|
| `CurlCouldntResolveHostException` | 6 | The URL's host name does not resolve | `Hostname` |
| `CurlCouldntResolveProxyException` | 5 | The proxy name (from `-x`/`--proxy` or `http_proxy`/`https_proxy`/`all_proxy`) does not resolve | `ProxyHost` |
| `CurlCouldntConnectException` | 7 | Connection refused, host unreachable or dropped by a firewall; with a proxy, the proxy could not be reached or refused the `-p` CONNECT tunnel | `Host`, `Port`, `ViaProxy` |
| `CurlOperationTimeoutException` | 28 | `--connect-timeout` expired before the connection was made | `Timeout` |

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl https://this-host-does-not-exist.invalid/");
}
catch (CurlCouldntResolveHostException ex)
{
    Console.WriteLine($"Could not resolve {ex.Hostname}");   // exit code 6
}
catch (CurlCouldntConnectException ex)
{
    Console.WriteLine($"Could not reach {(ex.ViaProxy ? "proxy " : "")}{ex.Host}:{ex.Port}");   // exit code 7
}
```

The messages say what to check, for example "Failed to connect to 127.0.0.1 port 1: Connection refused. Check that the server is running...". With a proxy in use, the connect message names the proxy rather than the URL's host.

### Reserved Connection Types

`CurlConnectionException` (no exit code; base of `CurlDnsException` with code 6 and `CurlProxyException` with code 5) and `CurlInterfaceFailedException` (45) exist in `CurlDotNet.Exceptions`, but nothing in the library throws them today. Catch the three types in the table above, or `CurlException` and switch on `CurlErrorCode`.

## Telling the Causes Apart

| Symptom | Likely cause | Check |
|---------|--------------|-------|
| Fails instantly for every URL | No network, or a proxy is required | Open any site in a browser; set `-x proxy:port` if your network needs one |
| `CurlCouldntResolveProxyException` or a connect error that names the proxy | Wrong `-x` value or a stale `http_proxy`/`https_proxy`/`all_proxy` environment variable | Fix the proxy, or exclude the host with `--noproxy host` (or `NO_PROXY`) |
| Fails only for one host name | Typo, or the name does not exist | `nslookup host` or `ping host` in a terminal |
| Resolves but refuses | Wrong port, or the service is down | Try the URL in a browser; check the port in the URL |
| HTTPS fails, HTTP works | TLS handshake failure (certificate) | See [SSL errors](ssl-errors.html) |
| Works with an IP, not a name | DNS misconfiguration | Check the machine's DNS settings, or pin the name with `--resolve host:443:203.0.113.10` |

## Common Fixes

### Check the URL

```csharp
var check = Curl.Validate("curl https://api.github.com");
if (!check.IsValid)
    Console.WriteLine(check.Error);
```

`Curl.Validate` catches syntax problems before any network call; it does not resolve the host.

### Go Through a Proxy

Corporate networks often block direct DNS or outbound connections:

```csharp
var result = await Curl.ExecuteAsync("curl -x http://proxy.corp.example:8080 https://api.github.com");
```

Builder: `CurlRequestBuilder.Get(url).WithProxy("http://proxy.corp.example:8080")`. CurlDotNet also reads the `http_proxy`, `https_proxy`, `all_proxy` and `NO_PROXY` environment variables like curl, so a stale variable can send requests to a proxy you did not expect.

### Retry Transient Failures

Short DNS outages and restarts do happen. By default `--retry` retries timeouts and transient HTTP statuses; add `--retry-connrefused` to retry refused connections too, or `--retry-all-errors` to retry any failure:

```csharp
var result = await Curl.ExecuteAsync("curl --retry 3 --retry-connrefused -m 10 https://api.github.com");
```

See [curl --retry in C#](../cookbook/retry.html). To control the loop yourself, retry with a pause and keep `-m` on each attempt so one hung connection cannot stall it:

```csharp
async Task<CurlResult> RetryRequest(string command, int attempts = 3)
{
    for (var i = 1; ; i++)
    {
        try { return await Curl.ExecuteAsync(command); }
        catch (CurlException ex) when (ex.CurlErrorCode is 6 or 7 && i < attempts)
        {
            await Task.Delay(TimeSpan.FromSeconds(i));   // 1s, 2s, ...
        }
    }
}
var result = await RetryRequest("curl -m 10 https://api.github.com");
```

### Fall Back to Another Host

```csharp
async Task<CurlResult> FetchFromAnyAsync(params string[] urls)
{
    foreach (var url in urls)
    {
        try { return await Curl.ExecuteAsync($"curl -m 10 {url}"); }
        catch (CurlException ex) when (ex.CurlErrorCode is 6 or 7) { /* try the next one */ }
    }
    throw new InvalidOperationException("All hosts unreachable");
}
```

## Debugging

### Inspect the Exception

```csharp
try
{
    await Curl.ExecuteAsync("curl https://this-host-does-not-exist.invalid/");
}
catch (CurlCouldntResolveHostException ex)
{
    Console.WriteLine(ex.Command);     // the command that failed
    Console.WriteLine(ex.Message);     // says what to check
    foreach (var tip in ex.Suggestions) Console.WriteLine($"- {tip}");
}
```

### Test Resolution Outside CurlDotNet

```bash
# Does the name resolve at all?
nslookup api.github.com

# Can the port be reached?
curl -v https://api.github.com
```

If these fail too, the problem is the machine or network, not your code.

### Resolve in .NET Directly

```csharp
using System.Net;

var addresses = await Dns.GetHostAddressesAsync("api.github.com");
Console.WriteLine(string.Join(", ", addresses));
```

## Related Documentation

- [Timeout errors](timeout-errors.html)
- [SSL/TLS errors](ssl-errors.html)
- [Cookbook](../cookbook/index.html)
- [Troubleshooting: common issues](../troubleshooting/common-issues.html)
- [Exception reference](index.html)
