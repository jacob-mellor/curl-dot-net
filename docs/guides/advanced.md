# Advanced Features

CurlDotNet provides powerful advanced features for complex scenarios and enterprise applications.

## Table of Contents
- [Middleware System](#middleware-system)
- [Protocol Handlers](#protocol-handlers)
- [Custom Authentication](#custom-authentication)
- [Performance Optimization](#performance-optimization)
- [Error Recovery](#error-recovery)
- [Extensions](#extensions)

## Middleware System

CurlDotNet includes a sophisticated middleware pipeline that allows you to intercept and modify requests and responses.

### Built-in Middleware

#### Retry Middleware
```csharp
var options = new CurlOptions()
    .WithRetry(maxAttempts: 3, delay: TimeSpan.FromSeconds(1));

var result = await Curl.PerformAsync(options);
```

#### Logging Middleware
```csharp
var options = new CurlOptions()
    .WithLogging(LogLevel.Debug);

// All requests and responses will be logged
```

#### Rate Limiting Middleware
```csharp
var options = new CurlOptions()
    .WithRateLimit(requestsPerSecond: 10);
```

### Custom Middleware

Create your own middleware by implementing `ICurlMiddleware`:

```csharp
public class CustomHeaderMiddleware : ICurlMiddleware
{
    public async Task<CurlResponse> ExecuteAsync(
        CurlOptions options,
        Func<CurlOptions, Task<CurlResponse>> next)
    {
        // Add custom header before request
        options.Headers.Add("X-Custom-Header", "Value");

        // Execute the request
        var response = await next(options);

        // Process response
        Console.WriteLine($"Response received: {response.StatusCode}");

        return response;
    }
}

// Register middleware
var options = new CurlOptions()
    .UseMiddleware(new CustomHeaderMiddleware());
```

## Protocol Handlers

CurlDotNet supports multiple protocols with specialized handlers:

### HTTP/HTTPS Handler
```csharp
var options = new CurlOptions()
    .WithHttp2()
    .WithHttp3()  // Experimental HTTP/3 support
    .WithConnectionReuse();
```

### FTP/FTPS Handler
```csharp
var options = new CurlOptions("ftp://ftp.example.com/file.txt")
    .WithFtpSsl()
    .WithCredentials("username", "password")
    .WithFtpCreateMissingDirs();
```

### WebSocket Handler
```csharp
var options = new CurlOptions("ws://example.com/socket")
    .WithWebSocket()
    .OnMessage((data) => Console.WriteLine($"Received: {data}"));

var connection = await Curl.ConnectWebSocketAsync(options);
await connection.SendAsync("Hello Server");
```

### Custom Protocol Handler
```csharp
public class CustomProtocolHandler : IProtocolHandler
{
    public bool CanHandle(Uri uri) => uri.Scheme == "custom";

    public async Task<CurlResponse> ExecuteAsync(CurlOptions options)
    {
        // Custom protocol implementation
        return new CurlResponse();
    }
}

CurlOptions.RegisterProtocolHandler(new CustomProtocolHandler());
```

## Custom Authentication

### OAuth 2.0
```csharp
var options = new CurlOptions()
    .WithOAuth2(
        clientId: "your-client-id",
        clientSecret: "your-secret",
        tokenEndpoint: "https://auth.example.com/token",
        scopes: new[] { "read", "write" }
    );
```

### Custom Authentication Provider
```csharp
public class CustomAuthProvider : IAuthenticationProvider
{
    public async Task AuthenticateAsync(CurlOptions options)
    {
        var token = await GetTokenAsync();
        options.Headers["Authorization"] = $"Custom {token}";
    }
}

var options = new CurlOptions()
    .WithAuthentication(new CustomAuthProvider());
```

## Performance Optimization

### Connection Pooling
```csharp
// Global connection pool configuration
CurlOptions.ConfigureConnectionPool(
    maxConnections: 100,
    maxConnectionsPerHost: 10,
    connectionTimeout: TimeSpan.FromSeconds(30),
    idleTimeout: TimeSpan.FromMinutes(5)
);
```

### Request Pipelining
```csharp
var options = new CurlOptions()
    .WithPipelining(maxRequests: 5)
    .WithMultiplexing();  // HTTP/2 multiplexing
```

### Compression
```csharp
var options = new CurlOptions()
    .WithCompression(CompressionAlgorithm.Brotli | CompressionAlgorithm.Gzip)
    .WithAutoDecompression();
```

### DNS Caching
```csharp
var options = new CurlOptions()
    .WithDnsCache(TimeSpan.FromMinutes(5))
    .WithDnsPreResolve(new[] { "api.example.com", "cdn.example.com" });
```

## Error Recovery

### Automatic Retry with Backoff
```csharp
var options = new CurlOptions()
    .WithRetryPolicy(new ExponentialBackoffPolicy
    {
        MaxAttempts = 5,
        InitialDelay = TimeSpan.FromSeconds(1),
        MaxDelay = TimeSpan.FromSeconds(30),
        RetryableStatusCodes = new[] { 429, 503, 504 }
    });
```

### Circuit Breaker Pattern
```csharp
var options = new CurlOptions()
    .WithCircuitBreaker(
        failureThreshold: 5,
        recoveryTimeout: TimeSpan.FromSeconds(30),
        onOpen: () => Console.WriteLine("Circuit opened"),
        onClose: () => Console.WriteLine("Circuit closed")
    );
```

### Fallback Mechanism
```csharp
var options = new CurlOptions("https://primary.example.com/api")
    .WithFallback("https://secondary.example.com/api")
    .WithFallback("https://tertiary.example.com/api");
```

## Extensions

### Metrics and Telemetry
```csharp
var options = new CurlOptions()
    .WithMetrics(metrics =>
    {
        Console.WriteLine($"DNS: {metrics.DnsLookupTime}ms");
        Console.WriteLine($"Connect: {metrics.ConnectTime}ms");
        Console.WriteLine($"TLS: {metrics.TlsHandshakeTime}ms");
        Console.WriteLine($"FirstByte: {metrics.TimeToFirstByte}ms");
        Console.WriteLine($"Total: {metrics.TotalTime}ms");
    });
```

### Request/Response Transformation
```csharp
var options = new CurlOptions()
    .TransformRequest(req =>
    {
        // Modify request before sending
        req.Headers["X-Request-Id"] = Guid.NewGuid().ToString();
        return req;
    })
    .TransformResponse(async resp =>
    {
        // Process response before returning
        if (resp.ContentType == "application/json")
        {
            resp.ParsedContent = JsonSerializer.Deserialize<dynamic>(resp.Body);
        }
        return resp;
    });
```

### Streaming Support
```csharp
// Stream large downloads
var options = new CurlOptions()
    .WithStreaming()
    .OnDataReceived(async (chunk) =>
    {
        await fileStream.WriteAsync(chunk);
    });

// Stream uploads
using var fileStream = File.OpenRead("large-file.zip");
var options = new CurlOptions()
    .WithStreamUpload(fileStream)
    .OnUploadProgress((sent, total) =>
    {
        Console.WriteLine($"Uploaded {sent}/{total} bytes");
    });
```

### Proxy Configuration
```csharp
var options = new CurlOptions()
    .WithProxy("http://proxy.example.com:8080")
    .WithProxyAuth("username", "password")
    .WithProxyType(ProxyType.Socks5)
    .BypassProxyFor(new[] { "*.internal.com", "192.168.*" });
```

## Best Practices

### 1. Resource Management
Always dispose of resources properly:
```csharp
using var curl = new CurlClient();
var response = await curl.ExecuteAsync(options);
```

### 2. Error Handling
Implement comprehensive error handling:
```csharp
try
{
    var response = await Curl.PerformAsync(options);
}
catch (CurlTimeoutException ex)
{
    // Handle timeout
}
catch (CurlNetworkException ex)
{
    // Handle network errors
}
catch (CurlException ex)
{
    // Handle general curl errors
}
```

### 3. Configuration Management
Centralize configuration:
```csharp
public static class CurlConfig
{
    public static CurlOptions DefaultOptions => new CurlOptions()
        .WithTimeout(TimeSpan.FromSeconds(30))
        .WithRetry(3)
        .WithCompression()
        .WithMetrics();
}
```

### 4. Testing
Use the mock provider for testing:
```csharp
// In tests
CurlOptions.UseMockProvider(mock =>
{
    mock.Setup("https://api.example.com/users")
        .Returns(new CurlResponse
        {
            StatusCode = 200,
            Body = "[{\"id\":1,\"name\":\"Test User\"}]"
        });
});
```

## Migration from HttpClient

See the [Migration Guide](https://jacob-mellor.github.io/curl-dot-net/migration/httpclient.md) for detailed instructions on migrating from HttpClient to CurlDotNet.

## Further Resources

- [API Reference](https://jacob-mellor.github.io/curl-dot-net/api/)
- [Examples](../samples/README.md)
- [Troubleshooting](troubleshooting/README.md)
- [Performance Benchmarks](https://github.com/jacob-mellor/curl-dot-net/tree/master/benchmarks)
- [Contributing Guide](https://github.com/jacob-mellor/curl-dot-net/blob/master/CONTRIBUTING.md)