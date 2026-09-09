# Configuration

Learn how to configure CurlDotNet for timeouts, SSL, proxies, and more.

## Global Settings

CurlDotNet provides static properties to configure default behavior for all requests:

```csharp
using CurlDotNet;

// Set default timeout for all requests (30 seconds)
Curl.DefaultMaxTimeSeconds = 30;

// Set connection timeout (10 seconds to establish connection)
Curl.DefaultConnectTimeoutSeconds = 10;

// Enable redirect following globally
Curl.DefaultFollowRedirects = true;

// Skip SSL verification (DEVELOPMENT ONLY!)
#if DEBUG
Curl.DefaultInsecure = true;
#endif
```

## Timeout Configuration

### Max Time (Overall Timeout)

Sets the maximum time a request can take:

```csharp
// Global default for all requests
Curl.DefaultMaxTimeSeconds = 30;

// Per-request override using curl option
var result = await Curl.ExecuteAsync("curl --max-time 60 https://slow-api.example.com");
```

### Connect Timeout

Sets how long to wait for the connection to be established:

```csharp
// Global default
Curl.DefaultConnectTimeoutSeconds = 10;

// Per-request override
var result = await Curl.ExecuteAsync("curl --connect-timeout 5 https://api.example.com");
```

### Timeout with CancellationToken

For more control, use a CancellationToken:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

try
{
    var result = await Curl.ExecuteAsync(
        "curl https://api.example.com",
        cts.Token
    );
}
catch (OperationCanceledException)
{
    Console.WriteLine("Request timed out or was cancelled");
}
```

## SSL/TLS Configuration

### Ignoring SSL Errors (Development Only)

Use `-k` or `--insecure` to skip SSL certificate verification:

```csharp
// WARNING: Only use in development!
var result = await Curl.ExecuteAsync("curl -k https://localhost:5001");

// Or set globally for development
#if DEBUG
Curl.DefaultInsecure = true;
#endif
```

### Custom Certificates

Specify custom CA certificates:

```csharp
// Use a specific CA certificate
var result = await Curl.ExecuteAsync(
    "curl --cacert /path/to/ca-bundle.crt https://api.example.com"
);

// Use client certificate authentication
var result = await Curl.ExecuteAsync(
    "curl --cert /path/to/client.pem --key /path/to/key.pem https://api.example.com"
);
```

## Proxy Configuration

### HTTP Proxy

```csharp
// Use an HTTP proxy
var result = await Curl.ExecuteAsync(
    "curl -x http://proxy.example.com:8080 https://api.example.com"
);

// Proxy with authentication
var result = await Curl.ExecuteAsync(
    "curl -x http://user:pass@proxy.example.com:8080 https://api.example.com"
);
```

### SOCKS Proxy

```csharp
// SOCKS5 proxy
var result = await Curl.ExecuteAsync(
    "curl --socks5 localhost:9050 https://api.example.com"
);

// SOCKS5 with hostname resolution through proxy
var result = await Curl.ExecuteAsync(
    "curl --socks5-hostname localhost:9050 https://api.example.com"
);
```

## Redirect Behavior

### Following Redirects

```csharp
// Enable globally
Curl.DefaultFollowRedirects = true;

// Or per-request with -L flag
var result = await Curl.ExecuteAsync("curl -L https://short.link/abc");

// Limit maximum redirects
var result = await Curl.ExecuteAsync(
    "curl -L --max-redirs 5 https://example.com"
);
```

### Preserving Authentication on Redirects

```csharp
// Keep auth when redirected to different host
var result = await Curl.ExecuteAsync(
    "curl -L --location-trusted -u user:pass https://api.example.com"
);
```

## Advanced Settings with CurlSettings

For advanced scenarios, use the `CurlSettings` class:

```csharp
var settings = new CurlSettings
{
    // Retry configuration
    RetryCount = 3,
    RetryDelay = TimeSpan.FromSeconds(2),

    // Progress callback for downloads
    OnProgress = (bytesReceived, totalBytes) =>
    {
        var percent = (bytesReceived * 100.0) / totalBytes;
        Console.WriteLine($"Progress: {percent:F1}%");
    }
};

var result = await Curl.ExecuteAsync(
    "curl https://example.com/large-file.zip",
    settings
);
```

## HTTP Version

Force a specific HTTP version:

```csharp
// Force HTTP/1.1
var result = await Curl.ExecuteAsync(
    "curl --http1.1 https://api.example.com"
);

// Force HTTP/2
var result = await Curl.ExecuteAsync(
    "curl --http2 https://api.example.com"
);
```

## User Agent Configuration

Set a custom user agent:

```csharp
// Using -A flag
var result = await Curl.ExecuteAsync(
    "curl -A 'MyApp/1.0' https://api.example.com"
);

// Using --user-agent
var result = await Curl.ExecuteAsync(
    "curl --user-agent 'MyApp/1.0 (Windows 10)' https://api.example.com"
);
```

## Cookie Configuration

### Sending Cookies

```csharp
// Send cookies with request
var result = await Curl.ExecuteAsync(
    "curl -b 'session=abc123; token=xyz' https://api.example.com"
);

// Load cookies from file
var result = await Curl.ExecuteAsync(
    "curl -b cookies.txt https://api.example.com"
);
```

### Saving Cookies

```csharp
// Save cookies to file
var result = await Curl.ExecuteAsync(
    "curl -c cookies.txt https://api.example.com/login"
);
```

## DNS Configuration

### Custom DNS Servers

```csharp
// Use specific DNS servers
var result = await Curl.ExecuteAsync(
    "curl --dns-servers 8.8.8.8,8.8.4.4 https://api.example.com"
);
```

### DNS Override (Host Resolution)

```csharp
// Force hostname to resolve to specific IP
var result = await Curl.ExecuteAsync(
    "curl --resolve api.example.com:443:192.168.1.100 https://api.example.com"
);
```

## Speed Limits

### Download Speed Limit

```csharp
// Limit download to 100KB/s
var result = await Curl.ExecuteAsync(
    "curl --limit-rate 100K https://example.com/file.zip"
);
```

## Keep-Alive Settings

```csharp
// Set keep-alive time
var result = await Curl.ExecuteAsync(
    "curl --keepalive-time 60 https://api.example.com"
);
```

## Environment-Specific Configuration

### Development

```csharp
public static class CurlConfiguration
{
    public static void ConfigureForDevelopment()
    {
        Curl.DefaultMaxTimeSeconds = 60;  // Longer timeout for debugging
        Curl.DefaultConnectTimeoutSeconds = 30;
        Curl.DefaultInsecure = true;  // Accept self-signed certs
        Curl.DefaultFollowRedirects = true;
    }
}
```

### Production

```csharp
public static class CurlConfiguration
{
    public static void ConfigureForProduction()
    {
        Curl.DefaultMaxTimeSeconds = 30;
        Curl.DefaultConnectTimeoutSeconds = 10;
        Curl.DefaultInsecure = false;  // Never skip SSL in production!
        Curl.DefaultFollowRedirects = true;
    }
}
```

### ASP.NET Core Integration

```csharp
// In Program.cs or Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // Configure CurlDotNet based on environment
    if (Environment.IsDevelopment())
    {
        Curl.DefaultInsecure = true;
        Curl.DefaultMaxTimeSeconds = 60;
    }
    else
    {
        Curl.DefaultInsecure = false;
        Curl.DefaultMaxTimeSeconds = 30;
    }
}
```

## Configuration Summary Table

| Setting | Global Property | Curl Option | Default |
|---------|-----------------|-------------|---------|
| Request timeout | `DefaultMaxTimeSeconds` | `--max-time` | 0 (none) |
| Connect timeout | `DefaultConnectTimeoutSeconds` | `--connect-timeout` | 0 (none) |
| Follow redirects | `DefaultFollowRedirects` | `-L` | false |
| Skip SSL verify | `DefaultInsecure` | `-k` | false |
| User agent | (per-request) | `-A` | curl default |
| Proxy | (per-request) | `-x` | none |
| Max redirects | (per-request) | `--max-redirs` | 50 |

## Next Steps

- [First Request](first-request.html) - Make your first request
- [Cookbook](../cookbook/) - Practical examples
- [Troubleshooting](../troubleshooting/) - Solve common issues
- [API Reference](../api/index.html) - Complete API documentation
