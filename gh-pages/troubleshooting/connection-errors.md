# Connection Errors Troubleshooting

This guide helps you diagnose and fix connection-related errors in CurlDotNet.

## Quick Diagnosis

```csharp
// Test basic connectivity first
var result = await Curl.ExecuteAsync("curl -v https://www.google.com");
if (!result.IsSuccess)
{
    Console.WriteLine($"Basic connectivity test failed: {result.Error}");
}
```

## Common Connection Errors

### CurlCouldntConnectException

**Error:** "Failed to connect to host"

**Causes:**
- Server is down
- Firewall blocking connection
- Wrong port number
- Network connectivity issues

**Solutions:**

1. **Check if server is running:**
   ```bash
   # Test with ping (ICMP)
   ping api.example.com

   # Test specific port with telnet
   telnet api.example.com 443
   ```

2. **Check firewall settings:**
   ```csharp
   // Try different ports
   var result = await Curl.ExecuteAsync("curl http://api.example.com:80");  // HTTP
   var result = await Curl.ExecuteAsync("curl https://api.example.com:443"); // HTTPS
   ```

3. **Use IP address directly:**
   ```csharp
   // Bypass DNS to test connectivity
   var result = await Curl.ExecuteAsync("curl https://93.184.216.34");
   ```

### CurlCouldntResolveHostException

**Error:** "Could not resolve host"

**Causes:**
- DNS server issues
- Hostname doesn't exist
- DNS cache problems
- Network configuration issues

**Solutions:**

1. **Verify hostname:**
   ```bash
   # Check DNS resolution
   nslookup api.example.com
   dig api.example.com
   host api.example.com
   ```

2. **Use different DNS:**
   ```csharp
   // Specify DNS server
   var result = await Curl.ExecuteAsync("curl --dns-servers 8.8.8.8 https://api.example.com");
   ```

3. **Clear DNS cache:**
   ```bash
   # Windows
   ipconfig /flushdns

   # macOS
   sudo dscacheutil -flushcache

   # Linux
   sudo systemd-resolve --flush-caches
   ```

### CurlCouldntResolveProxyException

**Error:** "Could not resolve proxy"

**Causes:**
- Proxy server down
- Proxy hostname wrong
- Proxy authentication required

**Solutions:**

1. **Check proxy settings:**
   ```csharp
   // Test without proxy
   var result = await Curl.ExecuteAsync("curl --noproxy '*' https://api.example.com");

   // Use specific proxy
   var result = await Curl.ExecuteAsync("curl -x http://proxy.company.com:8080 https://api.example.com");
   ```

2. **Proxy with authentication:**
   ```csharp
   var result = await Curl.ExecuteAsync(@"
       curl -x http://proxy.company.com:8080 \
            -U username:password \
            https://api.example.com
   ");
   ```

### CurlOperationTimeoutException

**Error:** "Connection timed out"

**Causes:**
- Server taking too long to respond
- Network latency
- Packet loss
- Firewall dropping packets

**Solutions:**

1. **Increase timeouts:**
   ```csharp
   // Separate connection and total timeout
   var result = await Curl.ExecuteAsync(@"
       curl --connect-timeout 30 \
            --max-time 120 \
            https://slow-api.example.com
   ");
   ```

2. **Test with shorter timeouts to isolate issue:**
   ```csharp
   // Quick timeout to test if server responds at all
   var result = await Curl.ExecuteAsync("curl --connect-timeout 5 https://api.example.com");
   ```

## Network Diagnostics

### 1. Test Network Path

```csharp
// Trace route to server
var result = await Curl.ExecuteAsync("curl --trace-time -v https://api.example.com");
```

### 2. Check MTU Issues

```csharp
// Reduce packet size for problematic networks
var result = await Curl.ExecuteAsync("curl --tcp-nodelay https://api.example.com");
```

### 3. IPv4 vs IPv6

```csharp
// Force IPv4
var result = await Curl.ExecuteAsync("curl -4 https://api.example.com");

// Force IPv6
var result = await Curl.ExecuteAsync("curl -6 https://api.example.com");
```

## Proxy Configuration

### Corporate Proxy

```csharp
// Auto-detect proxy from environment
var result = await Curl.ExecuteAsync("curl https://api.example.com");

// Manual proxy configuration
var result = await Curl.ExecuteAsync(@"
    curl -x http://proxy.company.com:8080 \
         -U domain\\username:password \
         https://external-api.com
");
```

### SOCKS Proxy

```csharp
// SOCKS5 proxy
var result = await Curl.ExecuteAsync("curl --socks5 localhost:1080 https://api.example.com");

// SOCKS5 with authentication
var result = await Curl.ExecuteAsync("curl --socks5 user:pass@localhost:1080 https://api.example.com");
```

## Firewall Issues

### Test Different Protocols

```csharp
// Try HTTP instead of HTTPS
var result = await Curl.ExecuteAsync("curl http://api.example.com");

// Try different ports
var result = await Curl.ExecuteAsync("curl https://api.example.com:8443");
```

### Bypass SSL/TLS Issues

```csharp
// Test without SSL (development only!)
var result = await Curl.ExecuteAsync("curl -k https://api.example.com");

// Use specific TLS version
var result = await Curl.ExecuteAsync("curl --tlsv1.2 https://api.example.com");
```

## Advanced Debugging

### Enable All Verbose Output

```csharp
var result = await Curl.ExecuteAsync("curl -vvv --trace-ascii debug.txt https://api.example.com");
Console.WriteLine("Check debug.txt for full trace");
```

### Network Interface Selection

```csharp
// Use specific network interface
var result = await Curl.ExecuteAsync("curl --interface eth0 https://api.example.com");

// Bind to specific local IP
var result = await Curl.ExecuteAsync("curl --interface 192.168.1.100 https://api.example.com");
```

### Keep-Alive Settings

```csharp
// Disable keep-alive
var result = await Curl.ExecuteAsync("curl --no-keepalive https://api.example.com");

// Set keep-alive time
var result = await Curl.ExecuteAsync("curl --keepalive-time 60 https://api.example.com");
```

## Platform-Specific Issues

### Windows Firewall

```powershell
# Check Windows Firewall rules
netsh advfirewall firewall show rule name=all | findstr "YourApp"
```

### Linux iptables

```bash
# Check iptables rules
sudo iptables -L -n
```

### Docker Networking

```dockerfile
# Ensure proper network mode
docker run --network host myapp

# Or use bridge with port mapping
docker run -p 8080:8080 myapp
```

## Connection Pooling

```csharp
// Reuse connections for better performance
var curl = new LibCurl();
curl.ConfigureConnection(options =>
{
    options.MaxConnections = 10;
    options.ConnectionTimeout = TimeSpan.FromSeconds(30);
});

// Make multiple requests with same connection
for (int i = 0; i < 100; i++)
{
    var result = await curl.GetAsync($"https://api.example.com/item/{i}");
}
```

## Retry Logic

```csharp
// Implement retry for transient failures
int maxRetries = 3;
for (int i = 0; i < maxRetries; i++)
{
    try
    {
        var result = await Curl.ExecuteAsync("curl https://api.example.com");
        if (result.IsSuccess) break;
    }
    catch (CurlConnectionException ex) when (i < maxRetries - 1)
    {
        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i))); // Exponential backoff
    }
}
```

## Related Pages

- [Troubleshooting Home](README.md)
- [Common Issues](common-issues.md)
- [DNS Errors](/exceptions/dns-errors.md)
- [Timeout Errors](/exceptions/timeout-errors.md)
- [Network Guide](/guides/network-troubleshooting.md)

---
*For persistent connection issues, check your network administrator or ISP.*