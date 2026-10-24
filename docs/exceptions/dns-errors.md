---
title: "curl Could Not Resolve Host in C# (CurlDotNet)"
description: "Fix 'curl could not resolve host' and connection refused in C#: CurlDotNet throws CurlCouldntResolveHostException (6), CurlCouldntResolveProxyException (5) or CurlCouldntConnectException (7)."
---

# curl Could Not Resolve Host in C# (DNS and Connection Errors)

"curl could not resolve host" in C# means the host name in your URL did not resolve to an IP address, and "connection refused" means it resolved but nothing answered on that port. CurlDotNet tells these apart exactly as the curl tool does: `CurlCouldntResolveHostException` (exit code 6) when the URL's host name does not resolve, `CurlCouldntResolveProxyException` (5) when the proxy's name is the one that failed, and `CurlCouldntConnectException` (7) when the connection itself failed. TLS failures have their own types (see [SSL/TLS errors](ssl-errors.md)). This page shows how to catch each one, how to tell the causes apart, and how to fix them.

## What Is Thrown

| Exception | `CurlErrorCode` | Thrown when | Properties |
|-----------|----:|-------------|------------|
| `CurlCouldntResolveHostException` | 6 | The URL's host name does not resolve (no such name, or the DNS server did not answer) | `Hostname` |
| `CurlCouldntResolveProxyException` | 5 | The proxy's name (from `-x`/`--proxy` or the `http_proxy`/`https_proxy`/`all_proxy` environment variables) does not resolve, or the proxy string is not a usable address | `ProxyHost` |
| `CurlCouldntConnectException` | 7 | Connection refused, host unreachable or dropped by a firewall; with a proxy, the proxy could not be reached or refused the `-p` CONNECT tunnel | `Host`, `Port`, `ViaProxy` |
| `CurlOperationTimeoutException` | 28 | `--connect-timeout` (or `-m`) expired before the connection was made | `Timeout` |

```csharp
using System;
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl https://this-host-does-not-exist.invalid/");
    Console.WriteLine(result.StatusCode);
}
catch (CurlCouldntResolveHostException ex)
{
    // exit code 6: the URL's host name did not resolve
    Console.WriteLine($"Could not resolve {ex.Hostname}: {ex.WhatHappened}");
}
catch (CurlCouldntResolveProxyException ex)
{
    // exit code 5: the proxy's name did not resolve
    Console.WriteLine($"Could not resolve the proxy {ex.ProxyHost}");
}
catch (CurlCouldntConnectException ex)
{
    // exit code 7: the name resolved but the connection failed
    Console.WriteLine($"Could not reach {(ex.ViaProxy ? "proxy " : "")}{ex.Host}:{ex.Port}");
}
catch (CurlOperationTimeoutException ex)
{
    // exit code 28: --connect-timeout or -m ran out
    Console.WriteLine($"Timed out: {ex.WhatHappened}");
}
```

Every message says what happened and what to check, and ends with a link to this page, for example `Failed to connect to 127.0.0.1 port 1: Connection refused` followed by the fix. With a proxy in use, the connect message names the proxy rather than the URL's host. Passwords in the URL or the proxy address are never printed.

To handle all of them in one place, catch `CurlException` and switch on `CurlErrorCode`:

```csharp
try
{
    await Curl.ExecuteAsync("curl -m 10 https://api.example.com/health");
}
catch (CurlException ex) when (ex.CurlErrorCode == 5 || ex.CurlErrorCode == 6 || ex.CurlErrorCode == 7)
{
    Console.WriteLine($"Network problem ({ex.CurlErrorCode}): {ex.WhatHappened}");
}
```

### Types That Are Not Thrown

`CurlConnectionException` (base of `CurlDnsException`, code 6, and `CurlProxyException`, code 5) and `CurlInterfaceFailedException` (45) exist in `CurlDotNet.Exceptions` for compatibility, but nothing in the library throws them. Catch the types in the table above.

## Telling the Causes Apart

| Symptom | Likely cause | Check |
|---------|--------------|-------|
| Fails instantly for every URL | No network, or a proxy is required | Open any site in a browser; set `-x proxy:port` if your network needs one |
| `CurlCouldntResolveProxyException`, or a connect error that names the proxy | Wrong `-x` value, or a stale `http_proxy`/`https_proxy`/`all_proxy` environment variable | Fix the proxy, or exclude the host with `--noproxy host` (or `NO_PROXY`); `-x ""` ignores the environment's proxies |
| `CurlCouldntResolveHostException` for one host name only | Typo, or the name does not exist in this network (an internal name seen from outside, a VPN that is off) | `nslookup host` in a terminal |
| Resolves but `CurlCouldntConnectException` | Wrong port, the service is down, or a firewall drops the connection | Try the URL in a browser; check the port in the URL |
| HTTPS fails, HTTP works | TLS handshake failure | See [SSL/TLS errors](ssl-errors.md) |
| Works with an IP address, not with the name | DNS misconfiguration on this machine | Check the machine's DNS settings, or pin the name with `--resolve` (below) |

## Common Fixes

### Check the URL First

```csharp
var check = Curl.Validate("curl https://api.github.com");
if (!check.IsValid)
{
    Console.WriteLine(check.Error);
}
```

`Curl.Validate` catches syntax problems (a missing scheme, a space in the host) before any network call; it does not resolve the host.

### Pin a Name to an Address: --resolve and --connect-to

When DNS is wrong or not yet updated (a new server, a staging box, a container), `--resolve HOST:PORT:ADDRESS` connects to the address you give while keeping the URL's host name for TLS and the `Host` header. `--connect-to HOST1:PORT1:HOST2:PORT2` sends the connection to another host and port instead.

```csharp
var pinned = await Curl.ExecuteAsync("curl --resolve api.example.com:443:203.0.113.10 https://api.example.com/health");

var redirected = await Curl.ExecuteAsync("curl --connect-to api.example.com:443:staging.example.com:8443 https://api.example.com/health");
```

```csharp
using CurlDotNet.Core;

var options = new CurlOptions { Url = "https://api.example.com/health" };
options.Resolve["api.example.com:443"] = "203.0.113.10";
var result = await Curl.ExecuteAsync(options);
```

`--dns-servers` is not applied: names are resolved by the operating system, and the option is listed in `CurlOptions.UnsupportedOptions`. Use `--resolve` to bypass DNS for specific names, or fix the machine's DNS configuration. `--interface`, `-4`/`--ipv4` and `-6`/`--ipv6` are likewise listed there and have no effect.

### Go Through a Proxy

Corporate networks often block direct DNS or outbound connections:

```csharp
var result = await Curl.ExecuteAsync("curl -x http://proxy.corp.example:8080 https://api.github.com");
```

With the builder, `CurlRequestBuilder.Get(url).WithProxy("http://proxy.corp.example:8080")`; per call, `new CurlSettings().WithProxy("http://proxy.corp.example:8080")`. CurlDotNet also reads the `http_proxy`, `https_proxy`, `all_proxy` and `NO_PROXY` environment variables like curl, so a stale variable can send requests to a proxy you did not expect. See [curl proxy in C#](../how-to/curl-proxy-csharp/README.md).

### Retry Transient Failures

Short DNS outages and server restarts do happen. `--retry` retries timeouts and transient HTTP statuses; add `--retry-connrefused` to retry refused connections (7) too, or `--retry-all-errors` to retry any failure, including a name that did not resolve (6). The retries wait 1, 2, 4 ... seconds, or `--retry-delay` seconds, and `--retry-max-time` caps the total; when it runs out, the last attempt's exception is thrown.

```csharp
var result = await Curl.ExecuteAsync("curl --retry 3 --retry-connrefused --connect-timeout 5 -m 30 https://api.github.com");
```

See [curl --retry in C#](../cookbook/retry.md). To control the loop yourself, keep `-m` on each attempt so one hung connection cannot stall it:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

static async Task<CurlResult> RetryRequestAsync(string command, int attempts = 3)
{
    for (var i = 1; ; i++)
    {
        try
        {
            return await Curl.ExecuteAsync(command);
        }
        catch (CurlException ex) when ((ex.CurlErrorCode == 6 || ex.CurlErrorCode == 7) && i < attempts)
        {
            await Task.Delay(TimeSpan.FromSeconds(i));   // 1 s, 2 s, ...
        }
    }
}

var result = await RetryRequestAsync("curl -m 10 https://api.github.com");
```

### Fall Back to Another Host

```csharp
static async Task<CurlResult> FetchFromAnyAsync(params string[] urls)
{
    CurlException? last = null;
    foreach (var url in urls)
    {
        try
        {
            return await Curl.ExecuteAsync($"curl -m 10 {url}");
        }
        catch (CurlException ex) when (ex.CurlErrorCode == 6 || ex.CurlErrorCode == 7)
        {
            last = ex;   // try the next one
        }
    }
    if (last != null)
    {
        throw last;   // every host failed: report the last failure
    }
    throw new ArgumentException("Pass at least one URL.", nameof(urls));
}

var data = await FetchFromAnyAsync("https://primary.example.com/data", "https://backup.example.com/data");
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
    Console.WriteLine(ex.WhatHappened);          // what failed
    foreach (var tip in ex.Suggestions)
    {
        Console.WriteLine($"- {tip}");           // what to check
    }
    Console.WriteLine(ex.DocumentationUrl);      // this page
    Console.WriteLine(ex.ToDetailedString());    // for logs; credentials in the command are masked
}
```

### Test Resolution Outside CurlDotNet

```bash
# Does the name resolve at all?
nslookup api.github.com

# Can the port be reached?
curl -v https://api.github.com
```

If these fail too, the problem is the machine or the network, not your code.

### Resolve in .NET Directly

```csharp
using System.Linq;
using System.Net;

var addresses = await Dns.GetHostAddressesAsync("api.github.com");
Console.WriteLine(string.Join(", ", addresses.Select(a => a.ToString())));
```

## Related Documentation

- [Timeout errors](timeout-errors.md)
- [SSL/TLS errors](ssl-errors.md)
- [curl --retry in C#](../cookbook/retry.md)
- [Cookbook](../cookbook/index.md)
- [Troubleshooting: common issues](../troubleshooting/common-issues.md)
- [Exception reference](index.md)
