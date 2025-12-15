# DNS Resolution Errors

## Overview

DNS errors occur when CurlDotNet cannot resolve a hostname to an IP address. This is one of the most common network errors and usually indicates connectivity or DNS configuration issues.

## Common Error Scenarios

### 1. Host Not Found

**Problem:** The hostname doesn't exist in DNS.

**Common Causes:**
- Typo in hostname
- Domain doesn't exist
- Domain expired
- Internal hostname on wrong network

**Solutions:**

```csharp
try
{
    var result = await curl.ExecuteAsync("curl https://example.com");
}
catch (CurlDnsException ex)
{
    Console.WriteLine($"DNS lookup failed for: {ex.Host}");

    // Try with IP address instead
    var ipAddress = "93.184.216.34"; // example.com IP
    result = await curl.ExecuteAsync($"curl https://{ipAddress}");

    // Or try alternative domain
    result = await curl.ExecuteAsync("curl https://www.example.com");
}
```

### 2. DNS Server Unreachable

**Problem:** Cannot reach the configured DNS servers.

**Common Causes:**
- No internet connection
- Firewall blocking DNS (port 53)
- Corporate proxy requirements
- DNS server down

**Solutions:**

```csharp
// Test with public DNS servers
public async Task<bool> TestDnsConnectivity()
{
    // Try resolving with different DNS servers
    var testHosts = new[]
    {
        "8.8.8.8",      // Google DNS
        "1.1.1.1",      // Cloudflare DNS
        "208.67.222.222" // OpenDNS
    };

    foreach (var dns in testHosts)
    {
        try
        {
            // Test with IP to bypass DNS
            var result = await curl.ExecuteAsync($"curl http://{dns}");
            return true;
        }
        catch
        {
            continue;
        }
    }
    return false;
}
```

### 3. DNS Cache Issues

**Problem:** Stale DNS cache returning outdated IP addresses.

**Solutions:**

```csharp
// Force fresh DNS lookup
public class DnsHelper
{
    public static void FlushDnsCache()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start("ipconfig", "/flushdns");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("dscacheutil", "-flushcache");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("systemctl", "restart systemd-resolved");
        }
    }
}
```

## Best Practices

### 1. DNS Resolution with Fallback

```csharp
public class DnsResolver
{
    private readonly Dictionary<string, string> _knownHosts = new()
    {
        { "api.example.com", "93.184.216.34" },
        { "cdn.example.com", "151.101.1.140" }
    };

    public async Task<CurlResult> ExecuteWithDnsFallback(string url)
    {
        var uri = new Uri(url);
        var curl = new Curl();

        try
        {
            // Try normal DNS resolution
            return await curl.ExecuteAsync($"curl {url}");
        }
        catch (CurlDnsException ex)
        {
            // Try with known IP if available
            if (_knownHosts.TryGetValue(uri.Host, out var ipAddress))
            {
                var ipUrl = url.Replace(uri.Host, ipAddress);
                // Add Host header for virtual hosting
                return await curl.ExecuteAsync(
                    $"curl -H \"Host: {uri.Host}\" {ipUrl}"
                );
            }
            throw;
        }
    }
}
```

### 2. Custom DNS Resolution

```csharp
public class CustomDnsResolver
{
    public async Task<string> ResolveHost(string hostname)
    {
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(hostname);
            return addresses.FirstOrDefault()?.ToString();
        }
        catch (SocketException)
        {
            return null;
        }
    }

    public async Task<CurlResult> ExecuteWithCustomDns(string url)
    {
        var uri = new Uri(url);
        var ipAddress = await ResolveHost(uri.Host);

        if (ipAddress == null)
        {
            throw new CurlDnsException(uri.Host);
        }

        var curl = new Curl();
        var ipUrl = url.Replace(uri.Host, ipAddress);
        return await curl.ExecuteAsync(
            $"curl --resolve {uri.Host}:{uri.Port ?? 443}:{ipAddress} {url}"
        );
    }
}
```

## Debugging Tips

### 1. Test DNS Resolution

```csharp
public async Task DiagnoseDns(string hostname)
{
    Console.WriteLine($"Diagnosing DNS for: {hostname}");

    // Try system DNS resolution
    try
    {
        var addresses = await Dns.GetHostAddressesAsync(hostname);
        Console.WriteLine($"Resolved to: {string.Join(", ", addresses.Select(a => a.ToString()))}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"System DNS failed: {ex.Message}");
    }

    // Try with curl
    var curl = new Curl();
    var result = await curl.ExecuteAsync($"curl -v https://{hostname}");
    Console.WriteLine($"Curl output: {result.StdErr}");
}
```

### 2. Use Alternative DNS

```csharp
// Use specific DNS server with curl
var result = await curl.ExecuteAsync(
    "curl --dns-servers 8.8.8.8,8.8.4.4 https://example.com"
);
```

### 3. Check Network Connectivity

```csharp
public async Task<bool> CheckConnectivity()
{
    var checks = new Dictionary<string, string>
    {
        { "DNS (Google)", "8.8.8.8" },
        { "DNS (Cloudflare)", "1.1.1.1" },
        { "HTTP", "http://www.google.com" },
        { "HTTPS", "https://www.google.com" }
    };

    foreach (var check in checks)
    {
        try
        {
            var result = await curl.ExecuteAsync($"curl -I --max-time 5 {check.Value}");
            Console.WriteLine($"✓ {check.Key}: OK");
        }
        catch
        {
            Console.WriteLine($"✗ {check.Key}: FAILED");
        }
    }
}
```

## Platform-Specific Solutions

### Windows

```csharp
// Reset Windows network stack
Process.Start("netsh", "winsock reset");
Process.Start("netsh", "int ip reset");
Process.Start("ipconfig", "/release");
Process.Start("ipconfig", "/renew");
Process.Start("ipconfig", "/flushdns");
```

### macOS

```csharp
// Clear DNS cache on macOS
Process.Start("sudo", "killall -HUP mDNSResponder");
Process.Start("sudo", "dscacheutil -flushcache");
```

### Linux

```csharp
// Restart DNS resolver
Process.Start("sudo", "systemctl restart systemd-resolved");
// Or for older systems
Process.Start("sudo", "service nscd restart");
```

## Configuration

### hosts File Override

```csharp
public void AddHostsEntry(string hostname, string ipAddress)
{
    string hostsPath;
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        hostsPath = @"C:\Windows\System32\drivers\etc\hosts";
    }
    else
    {
        hostsPath = "/etc/hosts";
    }

    var entry = $"{ipAddress} {hostname}";
    File.AppendAllText(hostsPath, Environment.NewLine + entry);
}
```

## Related Documentation

- [Connection Errors](connection-errors.md)
- [Timeout Errors](timeout-errors.md)
- [Network Troubleshooting Guide](/guides/network-troubleshooting.md)
- [CurlDotNet DNS API Reference](/api/CurlDotNet.Exceptions.CurlDnsException.md)

## Error Prevention Checklist

- [ ] Validate hostnames before requests
- [ ] Implement DNS fallback mechanisms
- [ ] Cache successful DNS resolutions
- [ ] Handle both IPv4 and IPv6
- [ ] Test with IP addresses to isolate DNS issues
- [ ] Configure appropriate DNS timeouts
- [ ] Use reliable DNS servers
- [ ] Implement retry logic for transient failures
- [ ] Monitor DNS resolution performance
- [ ] Document internal hostnames and IPs