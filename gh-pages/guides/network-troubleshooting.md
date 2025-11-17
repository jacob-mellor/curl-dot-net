# Network Troubleshooting Guide

## Overview

Network issues are among the most common problems when working with HTTP requests. This comprehensive guide helps you diagnose and fix network-related issues when using CurlDotNet.

## Understanding Network Errors

Network errors fall into several categories:

| Error Type | Cause | Severity | Common Scenarios |
|------------|-------|----------|------------------|
| DNS Resolution | Can't find the server | High | Typos, wrong domain, DNS down |
| Connection | Can't reach the server | High | Server down, firewall, wrong port |
| Timeout | Server too slow | Medium | Slow network, overloaded server |
| SSL/TLS | Certificate problems | Medium | Expired cert, self-signed cert |
| Protocol | HTTP version issues | Low | HTTP/2 not supported |

## DNS Resolution Errors

### What is DNS?

DNS (Domain Name System) translates domain names like `api.github.com` into IP addresses like `140.82.121.6`.

### Common DNS Errors

#### Error: "Could not resolve host"

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.examplee.com");
}
catch (CurlDnsException ex)
{
    Console.WriteLine($"DNS Error: {ex.Message}");
    // Output: Could not resolve host: api.examplee.com
}
```

**Causes:**
- Typo in the domain name
- No internet connection
- DNS server down
- Domain doesn't exist

**Solutions:**

1. Check for typos:
```csharp
// Wrong
await Curl.ExecuteAsync("curl https://api.examplee.com");

// Right
await Curl.ExecuteAsync("curl https://api.example.com");
```

2. Test DNS resolution:
```csharp
using System.Net;

try
{
    var addresses = await Dns.GetHostAddressesAsync("api.github.com");
    Console.WriteLine($"Resolved to: {addresses[0]}");
}
catch (Exception ex)
{
    Console.WriteLine($"DNS failed: {ex.Message}");
}
```

3. Use IP address directly (temporary workaround):
```csharp
// Bypass DNS by using IP directly
var result = await Curl.ExecuteAsync("curl http://140.82.121.6");
```

4. Check your DNS settings:
```csharp
// Use a different DNS server (Google's DNS)
var result = await Curl.ExecuteAsync("curl --dns-servers 8.8.8.8 https://api.github.com");
```

### Advanced DNS Debugging

```csharp
using System;
using System.Net;
using System.Threading.Tasks;
using CurlDotNet;

public class DnsDebugger
{
    public static async Task DiagnoseHostAsync(string hostname)
    {
        Console.WriteLine($"Diagnosing: {hostname}\n");

        // Step 1: Try to resolve DNS
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(hostname);
            Console.WriteLine($"✓ DNS Resolution successful");
            Console.WriteLine($"  IP Addresses:");
            foreach (var addr in addresses)
            {
                Console.WriteLine($"    - {addr}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ DNS Resolution failed: {ex.Message}");
            return;
        }

        // Step 2: Try a simple connection
        try
        {
            var result = await Curl.ExecuteAsync($"curl --connect-timeout 5 https://{hostname}");
            Console.WriteLine($"\n✓ Connection successful (Status: {result.StatusCode})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Connection failed: {ex.Message}");
        }
    }
}

// Usage
await DnsDebugger.DiagnoseHostAsync("api.github.com");
```

## Connection Errors

### Understanding Connection Failures

Connection errors occur when your computer can reach the server's IP but can't establish a connection.

### Common Connection Errors

#### Error: "Failed to connect"

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://example.com:9999");
}
catch (CurlConnectionException ex)
{
    Console.WriteLine($"Connection Error: {ex.Message}");
}
```

**Causes:**
- Wrong port number
- Firewall blocking connection
- Server not running
- Network restrictions

**Solutions:**

1. Verify the correct port:
```csharp
// Most HTTPS APIs use port 443 (default)
var result = await Curl.ExecuteAsync("curl https://api.github.com");

// Custom port
var result = await Curl.ExecuteAsync("curl https://api.example.com:8443");
```

2. Test with verbose output:
```csharp
var result = await Curl.ExecuteAsync("curl -v https://api.github.com");
// Shows detailed connection information
```

3. Check firewall rules:
```csharp
// Try connecting without SSL first
var result = await Curl.ExecuteAsync("curl http://example.com");
```

### Connection Retry Pattern

Implement automatic retries for transient connection issues:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class RetryHelper
{
    public static async Task<CurlResult> ExecuteWithRetryAsync(
        string curlCommand,
        int maxRetries = 3,
        int delayMs = 1000)
    {
        Exception lastException = null;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                Console.WriteLine($"Attempt {attempt}/{maxRetries}...");
                return await Curl.ExecuteAsync(curlCommand);
            }
            catch (CurlConnectionException ex)
            {
                lastException = ex;
                Console.WriteLine($"Connection failed: {ex.Message}");

                if (attempt < maxRetries)
                {
                    Console.WriteLine($"Waiting {delayMs}ms before retry...");
                    await Task.Delay(delayMs);
                    delayMs *= 2; // Exponential backoff
                }
            }
        }

        throw new Exception($"Failed after {maxRetries} attempts", lastException);
    }
}

// Usage
try
{
    var result = await RetryHelper.ExecuteWithRetryAsync(
        "curl https://api.example.com",
        maxRetries: 3,
        delayMs: 1000
    );
    Console.WriteLine($"Success! Status: {result.StatusCode}");
}
catch (Exception ex)
{
    Console.WriteLine($"All retries failed: {ex.Message}");
}
```

## Timeout Errors

### Understanding Timeouts

Timeouts prevent your application from hanging indefinitely when a server is unresponsive.

### Types of Timeouts

1. **Connection Timeout** - Time limit to establish a connection
2. **Transfer Timeout** - Time limit for the entire request
3. **Speed Limit Timeout** - Minimum transfer speed required

### Common Timeout Errors

#### Error: "Operation timeout"

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://slow-server.com");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Timeout: {ex.Message}");
}
```

**Solutions:**

1. Increase connection timeout:
```csharp
// Default is usually 5-10 seconds
var result = await Curl.ExecuteAsync(
    "curl --connect-timeout 30 https://api.example.com"
);
```

2. Increase total timeout:
```csharp
// Set maximum time for the entire operation
var result = await Curl.ExecuteAsync(
    "curl --max-time 60 https://api.example.com"
);
```

3. Set low speed limit:
```csharp
// Fail if average speed is below 1000 bytes/sec for 30 seconds
var result = await Curl.ExecuteAsync(
    "curl --speed-limit 1000 --speed-time 30 https://api.example.com"
);
```

### Smart Timeout Configuration

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class TimeoutConfig
{
    // Development: More forgiving timeouts
    public static class Development
    {
        public const int ConnectTimeout = 30;
        public const int TotalTimeout = 120;
    }

    // Production: Stricter timeouts
    public static class Production
    {
        public const int ConnectTimeout = 10;
        public const int TotalTimeout = 30;
    }

    public static async Task<CurlResult> ExecuteWithEnvironmentTimeoutAsync(
        string url,
        bool isDevelopment = false)
    {
        var config = isDevelopment ? Development : Production;

        var command = $"curl " +
            $"--connect-timeout {config.ConnectTimeout} " +
            $"--max-time {config.TotalTimeout} " +
            $"{url}";

        return await Curl.ExecuteAsync(command);
    }
}

// Usage
#if DEBUG
    var result = await TimeoutConfig.ExecuteWithEnvironmentTimeoutAsync(
        "https://api.example.com",
        isDevelopment: true
    );
#else
    var result = await TimeoutConfig.ExecuteWithEnvironmentTimeoutAsync(
        "https://api.example.com",
        isDevelopment: false
    );
#endif
```

## SSL/TLS Certificate Errors

### Understanding SSL Certificates

SSL/TLS certificates verify the identity of servers and encrypt data in transit.

### Common SSL Errors

#### Error: "SSL certificate problem"

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://self-signed.badssl.com");
}
catch (CurlSslException ex)
{
    Console.WriteLine($"SSL Error: {ex.Message}");
}
```

**Causes:**
- Expired certificate
- Self-signed certificate
- Certificate not trusted
- Hostname mismatch

**Solutions:**

1. For development/testing only - disable certificate verification:
```csharp
// WARNING: NEVER use in production!
var result = await Curl.ExecuteAsync("curl -k https://self-signed.example.com");
// or
var result = await Curl.ExecuteAsync("curl --insecure https://self-signed.example.com");
```

2. Specify a custom CA certificate:
```csharp
var result = await Curl.ExecuteAsync(
    "curl --cacert /path/to/ca-bundle.crt https://api.example.com"
);
```

3. Check certificate details:
```csharp
var result = await Curl.ExecuteAsync("curl -v https://api.github.com");
// Look for certificate information in verbose output
```

### SSL Certificate Validator

```csharp
using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CurlDotNet;

public class SslValidator
{
    public static async Task ValidateCertificateAsync(string url)
    {
        Console.WriteLine($"Checking SSL certificate for: {url}\n");

        try
        {
            // Try with verification enabled
            var result = await Curl.ExecuteAsync($"curl -v {url}");
            Console.WriteLine("✓ Certificate is valid and trusted");
        }
        catch (CurlSslException ex)
        {
            Console.WriteLine($"✗ Certificate validation failed: {ex.Message}\n");

            // Try to get more information
            Console.WriteLine("Certificate details:");
            try
            {
                var result = await Curl.ExecuteAsync($"curl -vk {url}");
                Console.WriteLine("  - Certificate exists but is not trusted");
                Console.WriteLine("  - Consider adding the CA to your trust store");
            }
            catch
            {
                Console.WriteLine("  - Cannot retrieve certificate information");
            }
        }
    }
}

// Usage
await SslValidator.ValidateCertificateAsync("https://api.github.com");
```

## Proxy and Firewall Issues

### Working Through Proxies

Many corporate networks require proxy servers for external access.

### Configuring Proxy Settings

```csharp
// HTTP proxy
var result = await Curl.ExecuteAsync(
    "curl --proxy http://proxy.example.com:8080 https://api.github.com"
);

// HTTPS proxy
var result = await Curl.ExecuteAsync(
    "curl --proxy https://proxy.example.com:8443 https://api.github.com"
);

// Proxy with authentication
var result = await Curl.ExecuteAsync(
    "curl --proxy-user username:password --proxy http://proxy.example.com:8080 https://api.github.com"
);

// SOCKS proxy
var result = await Curl.ExecuteAsync(
    "curl --socks5 socks-proxy.example.com:1080 https://api.github.com"
);
```

### Proxy Auto-Detection

```csharp
using System;
using System.Net;
using System.Threading.Tasks;
using CurlDotNet;

public class ProxyHelper
{
    public static async Task<CurlResult> ExecuteWithSystemProxyAsync(string url)
    {
        // Get system proxy settings
        var proxy = WebRequest.GetSystemWebProxy();
        var uri = new Uri(url);
        var proxyUri = proxy.GetProxy(uri);

        string command;
        if (proxyUri.ToString() == uri.ToString())
        {
            // No proxy needed
            command = $"curl {url}";
        }
        else
        {
            // Use detected proxy
            command = $"curl --proxy {proxyUri} {url}";
            Console.WriteLine($"Using proxy: {proxyUri}");
        }

        return await Curl.ExecuteAsync(command);
    }
}

// Usage
var result = await ProxyHelper.ExecuteWithSystemProxyAsync("https://api.github.com");
```

## Network Diagnostics Tool

Here's a comprehensive network diagnostic tool:

```csharp
using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using CurlDotNet;

public class NetworkDiagnostics
{
    public static async Task DiagnoseAsync(string url)
    {
        Console.WriteLine($"=== Network Diagnostics for {url} ===\n");

        var uri = new Uri(url);
        var hostname = uri.Host;

        // 1. Check internet connectivity
        Console.WriteLine("1. Internet Connectivity");
        try
        {
            using (var ping = new Ping())
            {
                var reply = await ping.SendPingAsync("8.8.8.8", 3000);
                if (reply.Status == IPStatus.Success)
                {
                    Console.WriteLine($"   ✓ Internet connected (ping: {reply.RoundtripTime}ms)");
                }
                else
                {
                    Console.WriteLine($"   ✗ No internet connection");
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ✗ Cannot test connectivity: {ex.Message}");
        }

        // 2. DNS Resolution
        Console.WriteLine("\n2. DNS Resolution");
        IPAddress[] addresses = null;
        try
        {
            var sw = Stopwatch.StartNew();
            addresses = await Dns.GetHostAddressesAsync(hostname);
            sw.Stop();

            Console.WriteLine($"   ✓ Resolved in {sw.ElapsedMilliseconds}ms");
            foreach (var addr in addresses)
            {
                Console.WriteLine($"     - {addr}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ✗ DNS failed: {ex.Message}");
            return;
        }

        // 3. Ping test
        Console.WriteLine("\n3. Host Reachability");
        if (addresses != null && addresses.Length > 0)
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync(addresses[0], 3000);
                    if (reply.Status == IPStatus.Success)
                    {
                        Console.WriteLine($"   ✓ Host reachable (ping: {reply.RoundtripTime}ms)");
                    }
                    else
                    {
                        Console.WriteLine($"   ! Host not responding to ping (may be blocked)");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ! Cannot ping: {ex.Message}");
            }
        }

        // 4. HTTP/HTTPS Connection
        Console.WriteLine("\n4. HTTP Connection Test");
        try
        {
            var sw = Stopwatch.StartNew();
            var result = await Curl.ExecuteAsync($"curl --connect-timeout 10 -v {url}");
            sw.Stop();

            Console.WriteLine($"   ✓ Connection successful");
            Console.WriteLine($"     - Status: {result.StatusCode}");
            Console.WriteLine($"     - Time: {sw.ElapsedMilliseconds}ms");
            Console.WriteLine($"     - Content-Type: {result.ContentType}");
        }
        catch (CurlSslException ex)
        {
            Console.WriteLine($"   ✗ SSL/TLS Error: {ex.Message}");
        }
        catch (CurlTimeoutException ex)
        {
            Console.WriteLine($"   ✗ Timeout: {ex.Message}");
        }
        catch (CurlConnectionException ex)
        {
            Console.WriteLine($"   ✗ Connection failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ✗ Error: {ex.Message}");
        }

        // 5. Trace route (informational)
        Console.WriteLine("\n5. Network Path");
        Console.WriteLine($"   Use 'traceroute {hostname}' (Linux/Mac) or 'tracert {hostname}' (Windows)");
        Console.WriteLine($"   to see the network path to the server");

        Console.WriteLine("\n=== Diagnostics Complete ===");
    }
}

// Usage
await NetworkDiagnostics.DiagnoseAsync("https://api.github.com");
```

## Testing Network Connectivity

### Simple Connectivity Test

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class ConnectivityTest
{
    public static async Task<bool> HasInternetAccessAsync()
    {
        string[] testUrls = {
            "https://www.google.com",
            "https://www.cloudflare.com",
            "https://www.microsoft.com"
        };

        foreach (var url in testUrls)
        {
            try
            {
                var result = await Curl.ExecuteAsync(
                    $"curl --connect-timeout 5 --max-time 10 {url}"
                );

                if (result.IsSuccess)
                {
                    Console.WriteLine($"✓ Internet access confirmed via {url}");
                    return true;
                }
            }
            catch
            {
                // Try next URL
            }
        }

        Console.WriteLine("✗ No internet access detected");
        return false;
    }
}

// Usage
if (await ConnectivityTest.HasInternetAccessAsync())
{
    // Proceed with your requests
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
}
else
{
    Console.WriteLine("Please check your internet connection");
}
```

## Best Practices

### 1. Always Set Timeouts

```csharp
// Bad: No timeout (could hang forever)
var result = await Curl.ExecuteAsync("curl https://api.example.com");

// Good: Reasonable timeouts
var result = await Curl.ExecuteAsync(
    "curl --connect-timeout 10 --max-time 30 https://api.example.com"
);
```

### 2. Implement Retry Logic

```csharp
// Bad: Single attempt
var result = await Curl.ExecuteAsync("curl https://api.example.com");

// Good: Retry with exponential backoff
var result = await RetryHelper.ExecuteWithRetryAsync(
    "curl https://api.example.com",
    maxRetries: 3,
    delayMs: 1000
);
```

### 3. Handle Specific Exceptions

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
}
catch (CurlDnsException ex)
{
    // DNS-specific handling
    Console.WriteLine($"DNS problem: Check domain name");
}
catch (CurlSslException ex)
{
    // SSL-specific handling
    Console.WriteLine($"SSL problem: Check certificate");
}
catch (CurlTimeoutException ex)
{
    // Timeout-specific handling
    Console.WriteLine($"Timeout: Server too slow or unresponsive");
}
catch (CurlConnectionException ex)
{
    // Connection-specific handling
    Console.WriteLine($"Connection problem: Check firewall");
}
catch (Exception ex)
{
    // General handling
    Console.WriteLine($"Unknown error: {ex.Message}");
}
```

### 4. Log Network Details

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CurlDotNet;

public class LoggingHelper
{
    public static async Task<CurlResult> ExecuteWithLoggingAsync(string curlCommand)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Starting request...");
            var result = await Curl.ExecuteAsync(curlCommand);
            sw.Stop();

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Request completed:");
            Console.WriteLine($"  - Status: {result.StatusCode}");
            Console.WriteLine($"  - Time: {sw.ElapsedMilliseconds}ms");
            Console.WriteLine($"  - Size: {result.Body.Length} bytes");

            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Request failed after {sw.ElapsedMilliseconds}ms:");
            Console.WriteLine($"  - Error: {ex.GetType().Name}");
            Console.WriteLine($"  - Message: {ex.Message}");
            throw;
        }
    }
}

// Usage
var result = await LoggingHelper.ExecuteWithLoggingAsync("curl https://api.github.com");
```

## Troubleshooting Checklist

When experiencing network issues, work through this checklist:

- [ ] **Check the URL for typos**
  ```csharp
  Console.WriteLine($"Requesting: {url}");
  // Verify it's correct before executing
  ```

- [ ] **Test internet connectivity**
  ```csharp
  var hasInternet = await ConnectivityTest.HasInternetAccessAsync();
  ```

- [ ] **Verify DNS resolution**
  ```csharp
  var addresses = await Dns.GetHostAddressesAsync(hostname);
  ```

- [ ] **Check firewall settings**
  - Allow outbound HTTPS (port 443)
  - Check corporate proxy settings

- [ ] **Test with verbose output**
  ```csharp
  var result = await Curl.ExecuteAsync("curl -v https://api.example.com");
  ```

- [ ] **Try with curl command line**
  ```bash
  curl -v https://api.example.com
  ```

- [ ] **Check SSL certificate**
  ```csharp
  await SslValidator.ValidateCertificateAsync(url);
  ```

- [ ] **Increase timeouts**
  ```csharp
  var result = await Curl.ExecuteAsync(
      "curl --connect-timeout 30 --max-time 60 https://api.example.com"
  );
  ```

- [ ] **Test from different network**
  - Try from phone hotspot
  - Try from different Wi-Fi

- [ ] **Contact API provider**
  - Check their status page
  - Verify your API key
  - Check rate limits

## Common Scenarios

### Scenario 1: Corporate Network

```csharp
// Configure for corporate environment
var result = await Curl.ExecuteAsync(@"
    curl --proxy http://proxy.company.com:8080 \
         --proxy-user username:password \
         --cacert /etc/ssl/certs/company-ca.crt \
         --connect-timeout 30 \
         https://api.example.com
");
```

### Scenario 2: Unreliable Network

```csharp
// Configure for mobile/unreliable networks
var result = await Curl.ExecuteAsync(@"
    curl --connect-timeout 15 \
         --max-time 60 \
         --retry 3 \
         --retry-delay 2 \
         --retry-max-time 180 \
         https://api.example.com
");
```

### Scenario 3: Development with Self-Signed Certificates

```csharp
#if DEBUG
// Only in development!
var result = await Curl.ExecuteAsync("curl -k https://localhost:5001/api");
#else
// Production uses proper certificates
var result = await Curl.ExecuteAsync("curl https://api.example.com");
#endif
```

## Related Resources

- [Error Handling Tutorial](../tutorials/06-handling-errors.md)
- [Debugging Requests Tutorial](../tutorials/14-debugging-requests.md)
- [Common Issues](../troubleshooting/common-issues.md)
- [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)

## Summary

Network troubleshooting requires systematic diagnosis:

1. Verify basic connectivity
2. Check DNS resolution
3. Test the connection
4. Validate SSL certificates
5. Configure proxy if needed
6. Implement proper error handling
7. Use appropriate timeouts
8. Add retry logic for resilience

With these tools and techniques, you can diagnose and resolve most network issues efficiently.

---

**Need more help?** Visit our [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions) for community support.
