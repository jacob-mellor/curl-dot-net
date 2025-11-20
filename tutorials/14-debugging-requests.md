# Tutorial 14: Debugging Requests

Learn how to troubleshoot HTTP requests, diagnose issues, and use debugging tools effectively with CurlDotNet.

## Enabling Debug Output

### Basic Debug Logging
```csharp
public class DebugCurl : Curl
{
    public bool DebugMode { get; set; } = true;

    public override async Task<CurlResult> GetAsync(string url, CancellationToken ct = default)
    {
        if (DebugMode)
        {
            Console.WriteLine($"[DEBUG] GET {url}");
            Console.WriteLine($"[DEBUG] Headers: {string.Join(", ", Headers.Select(h => $"{h.Key}={h.Value}"))}");
        }

        var stopwatch = Stopwatch.StartNew();
        var result = await base.GetAsync(url, ct);
        stopwatch.Stop();

        if (DebugMode)
        {
            Console.WriteLine($"[DEBUG] Status: {result.StatusCode}");
            Console.WriteLine($"[DEBUG] Time: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"[DEBUG] Response size: {result.Data?.Length ?? 0} bytes");

            if (!result.IsSuccess)
            {
                Console.WriteLine($"[DEBUG] Error: {result.Error}");
            }
        }

        return result;
    }
}
```

### Verbose Curl Output
```csharp
public class VerboseCurl : Curl
{
    public VerboseCurl()
    {
        // Enable curl's verbose output
        Options.Verbose = true;

        // Capture debug information
        OnDebugMessage = message =>
        {
            Console.WriteLine($"[CURL] {message}");
        };
    }

    public async Task<CurlResult> GetWithFullDebug(string url)
    {
        Console.WriteLine("=== REQUEST DEBUG INFO ===");
        Console.WriteLine($"URL: {url}");
        Console.WriteLine($"Method: GET");
        Console.WriteLine($"Timeout: {Timeout}");
        Console.WriteLine("\nRequest Headers:");

        foreach (var header in Headers)
        {
            Console.WriteLine($"  {header.Key}: {header.Value}");
        }

        var result = await GetAsync(url);

        Console.WriteLine("\n=== RESPONSE DEBUG INFO ===");
        Console.WriteLine($"Status Code: {result.StatusCode}");
        Console.WriteLine($"Success: {result.IsSuccess}");

        if (result.Headers != null)
        {
            Console.WriteLine("\nResponse Headers:");
            foreach (var header in result.Headers)
            {
                Console.WriteLine($"  {header.Key}: {header.Value}");
            }
        }

        if (!string.IsNullOrEmpty(result.Error))
        {
            Console.WriteLine($"\nError: {result.Error}");
        }

        if (!string.IsNullOrEmpty(result.Data))
        {
            Console.WriteLine($"\nResponse Body (first 500 chars):");
            Console.WriteLine(result.Data.Substring(0, Math.Min(500, result.Data.Length)));
        }

        return result;
    }
}
```

## Request/Response Inspection

### HTTP Traffic Logger
```csharp
public class HttpTrafficLogger
{
    private readonly string _logPath;
    private readonly Curl _curl;

    public HttpTrafficLogger(string logPath = "http_traffic.log")
    {
        _logPath = logPath;
        _curl = new Curl();
    }

    public async Task<CurlResult> LoggedRequest(string url, HttpMethod method, string body = null)
    {
        var requestId = Guid.NewGuid().ToString("N").Substring(0, 8);
        var timestamp = DateTime.UtcNow;

        // Log request
        await LogRequest(requestId, timestamp, url, method, body);

        // Execute request
        var stopwatch = Stopwatch.StartNew();
        CurlResult result;

        switch (method.Method)
        {
            case "GET":
                result = await _curl.GetAsync(url);
                break;
            case "POST":
                result = await _curl.PostAsync(url, body);
                break;
            default:
                throw new NotSupportedException($"Method {method} not supported");
        }

        stopwatch.Stop();

        // Log response
        await LogResponse(requestId, result, stopwatch.ElapsedMilliseconds);

        return result;
    }

    private async Task LogRequest(string requestId, DateTime timestamp, string url, HttpMethod method, string body)
    {
        var log = new StringBuilder();
        log.AppendLine($"=== REQUEST {requestId} at {timestamp:yyyy-MM-dd HH:mm:ss.fff} ===");
        log.AppendLine($"{method} {url}");

        foreach (var header in _curl.Headers)
        {
            log.AppendLine($"{header.Key}: {header.Value}");
        }

        if (!string.IsNullOrEmpty(body))
        {
            log.AppendLine();
            log.AppendLine(body);
        }

        log.AppendLine();

        await File.AppendAllTextAsync(_logPath, log.ToString());
    }

    private async Task LogResponse(string requestId, CurlResult result, long elapsedMs)
    {
        var log = new StringBuilder();
        log.AppendLine($"=== RESPONSE {requestId} ({elapsedMs}ms) ===");
        log.AppendLine($"Status: {result.StatusCode}");

        if (result.Headers != null)
        {
            foreach (var header in result.Headers)
            {
                log.AppendLine($"{header.Key}: {header.Value}");
            }
        }

        if (!string.IsNullOrEmpty(result.Data))
        {
            log.AppendLine();
            log.AppendLine(result.Data);
        }

        if (!string.IsNullOrEmpty(result.Error))
        {
            log.AppendLine($"ERROR: {result.Error}");
        }

        log.AppendLine();
        log.AppendLine();

        await File.AppendAllTextAsync(_logPath, log.ToString());
    }
}
```

## Network Diagnostics

### Connection Testing
```csharp
public class NetworkDiagnostics
{
    private readonly Curl _curl = new Curl();

    public async Task<DiagnosticReport> RunDiagnostics(string url)
    {
        var report = new DiagnosticReport { Url = url };

        // Test DNS resolution
        report.DnsTest = await TestDns(url);

        // Test connection
        report.ConnectionTest = await TestConnection(url);

        // Test SSL/TLS
        if (url.StartsWith("https"))
        {
            report.SslTest = await TestSsl(url);
        }

        // Test response time
        report.ResponseTimeTest = await TestResponseTime(url);

        // Test different methods
        report.MethodTests = await TestMethods(url);

        return report;
    }

    private async Task<TestResult> TestDns(string url)
    {
        try
        {
            var uri = new Uri(url);
            var addresses = await Dns.GetHostAddressesAsync(uri.Host);

            return new TestResult
            {
                Success = true,
                Message = $"Resolved to {addresses.Length} IP addresses",
                Details = string.Join(", ", addresses.Select(a => a.ToString()))
            };
        }
        catch (Exception ex)
        {
            return new TestResult
            {
                Success = false,
                Message = "DNS resolution failed",
                Error = ex.Message
            };
        }
    }

    private async Task<TestResult> TestConnection(string url)
    {
        var result = await _curl.GetAsync(url);

        return new TestResult
        {
            Success = result.IsSuccess,
            Message = result.IsSuccess ? "Connection successful" : "Connection failed",
            Error = result.Error,
            StatusCode = result.StatusCode
        };
    }

    private async Task<TestResult> TestSsl(string url)
    {
        try
        {
            var curl = new Curl { Options = { VerifySsl = true } };
            var result = await curl.GetAsync(url);

            return new TestResult
            {
                Success = result.IsSuccess,
                Message = "SSL/TLS verification passed",
                Details = "Certificate is valid and trusted"
            };
        }
        catch (Exception ex)
        {
            return new TestResult
            {
                Success = false,
                Message = "SSL/TLS verification failed",
                Error = ex.Message
            };
        }
    }

    private async Task<ResponseTimeTest> TestResponseTime(string url)
    {
        var times = new List<long>();

        for (int i = 0; i < 5; i++)
        {
            var sw = Stopwatch.StartNew();
            await _curl.GetAsync(url);
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);
        }

        return new ResponseTimeTest
        {
            Success = true,
            AverageMs = times.Average(),
            MinMs = times.Min(),
            MaxMs = times.Max(),
            Samples = times
        };
    }

    private async Task<Dictionary<string, TestResult>> TestMethods(string url)
    {
        var results = new Dictionary<string, TestResult>();

        // Test GET
        var getResult = await _curl.GetAsync(url);
        results["GET"] = new TestResult
        {
            Success = getResult.IsSuccess,
            StatusCode = getResult.StatusCode
        };

        // Test HEAD
        var headResult = await _curl.HeadAsync(url);
        results["HEAD"] = new TestResult
        {
            Success = headResult.IsSuccess,
            StatusCode = headResult.StatusCode
        };

        // Test OPTIONS
        var optionsResult = await _curl.OptionsAsync(url);
        results["OPTIONS"] = new TestResult
        {
            Success = optionsResult.IsSuccess,
            StatusCode = optionsResult.StatusCode,
            Details = optionsResult.Headers?.GetValueOrDefault("Allow")
        };

        return results;
    }
}

public class DiagnosticReport
{
    public string Url { get; set; }
    public TestResult DnsTest { get; set; }
    public TestResult ConnectionTest { get; set; }
    public TestResult SslTest { get; set; }
    public ResponseTimeTest ResponseTimeTest { get; set; }
    public Dictionary<string, TestResult> MethodTests { get; set; }

    public void PrintReport()
    {
        Console.WriteLine($"\n=== DIAGNOSTIC REPORT FOR {Url} ===\n");

        Console.WriteLine($"DNS Resolution: {(DnsTest.Success ? "✓" : "✗")} {DnsTest.Message}");
        if (!string.IsNullOrEmpty(DnsTest.Details))
            Console.WriteLine($"  {DnsTest.Details}");

        Console.WriteLine($"Connection: {(ConnectionTest.Success ? "✓" : "✗")} {ConnectionTest.Message}");

        if (SslTest != null)
            Console.WriteLine($"SSL/TLS: {(SslTest.Success ? "✓" : "✗")} {SslTest.Message}");

        Console.WriteLine($"\nResponse Times:");
        Console.WriteLine($"  Average: {ResponseTimeTest.AverageMs:F0}ms");
        Console.WriteLine($"  Min: {ResponseTimeTest.MinMs}ms, Max: {ResponseTimeTest.MaxMs}ms");

        Console.WriteLine($"\nMethod Support:");
        foreach (var method in MethodTests)
        {
            Console.WriteLine($"  {method.Key}: {(method.Value.Success ? "✓" : "✗")} (HTTP {method.Value.StatusCode})");
        }
    }
}

public class TestResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string Error { get; set; }
    public string Details { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}

public class ResponseTimeTest : TestResult
{
    public double AverageMs { get; set; }
    public long MinMs { get; set; }
    public long MaxMs { get; set; }
    public List<long> Samples { get; set; }
}
```

## Common Issues and Solutions

### Debugging Timeout Issues
```csharp
public class TimeoutDebugger
{
    public async Task DebugTimeouts(string url)
    {
        Console.WriteLine($"Testing timeouts for {url}\n");

        // Test with different timeout values
        var timeouts = new[] { 1, 5, 10, 30, 60 };

        foreach (var timeout in timeouts)
        {
            var curl = new Curl { Timeout = TimeSpan.FromSeconds(timeout) };
            var sw = Stopwatch.StartNew();

            try
            {
                var result = await curl.GetAsync(url);
                sw.Stop();

                if (result.IsSuccess)
                {
                    Console.WriteLine($"✓ {timeout}s timeout: Success in {sw.ElapsedMilliseconds}ms");
                    break;
                }
                else
                {
                    Console.WriteLine($"✗ {timeout}s timeout: Failed - {result.Error}");
                }
            }
            catch (OperationCanceledException)
            {
                sw.Stop();
                Console.WriteLine($"✗ {timeout}s timeout: Timed out after {sw.ElapsedMilliseconds}ms");
            }
        }
    }
}
```

### Debugging Authentication Issues
```csharp
public class AuthDebugger
{
    public async Task DebugAuth(string url, string token)
    {
        var curl = new Curl();

        Console.WriteLine("=== AUTHENTICATION DEBUG ===\n");

        // Test without auth
        Console.WriteLine("1. Testing without authentication:");
        var noAuthResult = await curl.GetAsync(url);
        Console.WriteLine($"   Status: {noAuthResult.StatusCode}");
        if (noAuthResult.StatusCode == HttpStatusCode.Unauthorized)
        {
            Console.WriteLine("   ✓ Server correctly requires authentication");
        }

        // Test with auth
        Console.WriteLine("\n2. Testing with Bearer token:");
        curl.Headers["Authorization"] = $"Bearer {token}";
        var authResult = await curl.GetAsync(url);
        Console.WriteLine($"   Status: {authResult.StatusCode}");

        if (authResult.IsSuccess)
        {
            Console.WriteLine("   ✓ Authentication successful");
        }
        else if (authResult.StatusCode == HttpStatusCode.Unauthorized)
        {
            Console.WriteLine("   ✗ Token rejected - may be expired or invalid");

            // Try to decode JWT to check expiration
            try
            {
                var parts = token.Split('.');
                if (parts.Length == 3)
                {
                    var payload = Encoding.UTF8.GetString(
                        Convert.FromBase64String(parts[1] + "==")
                    );
                    Console.WriteLine($"   Token payload: {payload}");
                }
            }
            catch
            {
                Console.WriteLine("   Could not decode token");
            }
        }
        else if (authResult.StatusCode == HttpStatusCode.Forbidden)
        {
            Console.WriteLine("   ✗ Authenticated but not authorized for this resource");
        }
    }
}
```

### Debugging JSON Issues
```csharp
public class JsonDebugger
{
    public async Task DebugJsonResponse(string url)
    {
        var curl = new Curl();
        var result = await curl.GetAsync(url);

        Console.WriteLine("=== JSON DEBUG ===\n");

        if (!result.IsSuccess)
        {
            Console.WriteLine($"Request failed: {result.Error}");
            return;
        }

        // Check Content-Type
        if (result.Headers.TryGetValue("Content-Type", out var contentType))
        {
            Console.WriteLine($"Content-Type: {contentType}");

            if (!contentType.Contains("json"))
            {
                Console.WriteLine("⚠ Warning: Content-Type is not JSON");
            }
        }

        // Try to parse as JSON
        try
        {
            using var doc = JsonDocument.Parse(result.Data);
            Console.WriteLine("✓ Valid JSON structure");

            // Pretty print
            var options = new JsonSerializerOptions { WriteIndented = true };
            var prettyJson = JsonSerializer.Serialize(doc.RootElement, options);
            Console.WriteLine("\nFormatted JSON:");
            Console.WriteLine(prettyJson);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"✗ Invalid JSON: {ex.Message}");
            Console.WriteLine("\nRaw response (first 1000 chars):");
            Console.WriteLine(result.Data.Substring(0, Math.Min(1000, result.Data.Length)));
        }
    }
}
```

## Performance Profiling

### Request Performance Profiler
```csharp
public class PerformanceProfiler
{
    private readonly Curl _curl = new Curl();

    public async Task<PerformanceProfile> ProfileRequest(string url)
    {
        var profile = new PerformanceProfile { Url = url };

        // DNS lookup time
        var dnsStart = Stopwatch.StartNew();
        var addresses = await Dns.GetHostAddressesAsync(new Uri(url).Host);
        profile.DnsLookupMs = dnsStart.ElapsedMilliseconds;

        // Connection time (approximate)
        var connectStart = Stopwatch.StartNew();
        var tcpClient = new TcpClient();
        try
        {
            await tcpClient.ConnectAsync(new Uri(url).Host, new Uri(url).Port);
            profile.ConnectionMs = connectStart.ElapsedMilliseconds;
            tcpClient.Close();
        }
        catch { }

        // Full request time
        var requestStart = Stopwatch.StartNew();
        var result = await _curl.GetAsync(url);
        profile.TotalMs = requestStart.ElapsedMilliseconds;

        // Response size
        profile.ResponseBytes = result.Data?.Length ?? 0;

        // Calculate derived metrics
        profile.TransferMs = profile.TotalMs - profile.ConnectionMs;
        profile.TransferRateMBps = profile.ResponseBytes / 1024.0 / 1024.0 / (profile.TransferMs / 1000.0);

        return profile;
    }
}

public class PerformanceProfile
{
    public string Url { get; set; }
    public long DnsLookupMs { get; set; }
    public long ConnectionMs { get; set; }
    public long TransferMs { get; set; }
    public long TotalMs { get; set; }
    public int ResponseBytes { get; set; }
    public double TransferRateMBps { get; set; }

    public void Print()
    {
        Console.WriteLine($"\n=== PERFORMANCE PROFILE ===");
        Console.WriteLine($"URL: {Url}");
        Console.WriteLine($"DNS Lookup: {DnsLookupMs}ms");
        Console.WriteLine($"Connection: {ConnectionMs}ms");
        Console.WriteLine($"Transfer: {TransferMs}ms");
        Console.WriteLine($"Total: {TotalMs}ms");
        Console.WriteLine($"Response Size: {ResponseBytes:N0} bytes");
        Console.WriteLine($"Transfer Rate: {TransferRateMBps:F2} MB/s");
    }
}
```

## Integration with Debugging Tools

### Fiddler/Charles Proxy Integration
```csharp
public class ProxyDebugCurl : Curl
{
    public ProxyDebugCurl(bool useDebugProxy = true)
    {
        if (useDebugProxy)
        {
            // Configure to use local debugging proxy
            Options.Proxy = "http://localhost:8888";

            // Trust the proxy's certificate (for HTTPS debugging)
            Options.VerifySsl = false;

            Console.WriteLine("Configured to use debugging proxy at localhost:8888");
            Console.WriteLine("Make sure Fiddler or Charles Proxy is running");
        }
    }
}
```

## Best Practices

1. **Enable debug mode in development** - Not in production
2. **Log to files** - Console output can be overwhelming
3. **Include timestamps** - Track when issues occur
4. **Capture full context** - Headers, body, and metadata
5. **Use correlation IDs** - Track requests across systems
6. **Implement structured logging** - Easy to parse and analyze
7. **Test edge cases** - Timeouts, large responses, auth failures
8. **Profile performance** - Identify bottlenecks
9. **Use debugging proxies** - Fiddler, Charles, Wireshark
10. **Keep sensitive data secure** - Don't log passwords or tokens

## Summary

Effective debugging is crucial for troubleshooting:
- Enable verbose output for detailed information
- Log requests and responses for analysis
- Run diagnostics to identify issues
- Profile performance to find bottlenecks
- Use appropriate tools for different scenarios

## Congratulations!

You've completed all 14 tutorials! You now have a comprehensive understanding of CurlDotNet and can:
- Make various types of HTTP requests
- Handle responses and errors properly
- Work with different data formats
- Implement authentication
- Optimize performance with parallel requests
- Debug issues effectively

---

[← Previous: Parallel Requests](13-parallel-requests.md) | [Back to Tutorials](README.md)