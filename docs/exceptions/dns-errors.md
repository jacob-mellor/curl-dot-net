---
title: "curl Could Not Resolve Host in C# (CurlDotNet)"
description: "Fix 'curl could not resolve host' and connection refused in C#: CurlDotNet throws CurlCouldntConnectException with Host and Port; how to catch and fix it."
---

# curl Could Not Resolve Host in C# (DNS and Connection Errors)

"curl could not resolve host" in C# means the host name in your URL did not resolve to an IP address, and "connection refused" means it resolved but nothing answered on that port. CurlDotNet reports both, and TLS handshake failures, as `CurlDotNet.Exceptions.CurlCouldntConnectException`, which carries the `Host` and `Port` it tried to reach. This page shows how to catch it and how to tell the causes apart.

## What Is Thrown

Any failure that prevents a response from arriving at the network level (`HttpRequestException` inside .NET) is mapped to `CurlCouldntConnectException`, curl error code 7:

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl https://this-host-does-not-exist.example");
}
catch (CurlCouldntConnectException ex)
{
    Console.WriteLine($"Could not reach {ex.Host}:{ex.Port}");
    Console.WriteLine(ex.Message);   // "Failed to connect to this-host-does-not-exist.example:443"
    Console.WriteLine(ex.CurlErrorCode); // 7
}
```

`CurlDnsException` and `CurlCouldntResolveHostException` also exist as types, but the HTTP engine does not currently distinguish DNS failures from other connection failures; catch `CurlCouldntConnectException`.

## Telling the Causes Apart

| Symptom | Likely cause | Check |
|---------|--------------|-------|
| Fails instantly for every URL | No network, or a proxy is required | Open any site in a browser; set `-x proxy:port` if your network needs one |
| Fails only for one host name | Typo, or the name does not exist | `nslookup host` or `ping host` in a terminal |
| Resolves but refuses | Wrong port, or the service is down | Try the URL in a browser; check the port in the URL |
| HTTPS fails, HTTP works | TLS handshake failure (certificate) | See [SSL errors](ssl-errors.html) |
| Works with an IP, not a name | DNS misconfiguration | Check the machine's DNS settings |

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

Builder: `CurlRequestBuilder.Get(url).WithProxy("http://proxy.corp.example:8080")`.

### Retry Transient Failures

Short DNS outages do happen. Retry a few times with a pause, and keep `-m` on each attempt so one hung connection cannot stall the loop:

```csharp
async Task<CurlResult> RetryRequest(string command, int attempts = 3)
{
    for (var i = 1; ; i++)
    {
        try { return await Curl.ExecuteAsync(command); }
        catch (CurlCouldntConnectException) when (i < attempts)
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
        catch (CurlCouldntConnectException) { /* try the next one */ }
    }
    throw new InvalidOperationException("All hosts unreachable");
}
```

## Debugging

### Inspect the Exception

```csharp
try
{
    await Curl.ExecuteAsync("curl https://this-host-does-not-exist.example");
}
catch (CurlCouldntConnectException ex)
{
    Console.WriteLine(ex.Command);     // the command that failed
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
