# Recipe: Handle Errors

## 🎯 What You'll Build

Robust applications that gracefully handle network errors, API failures, and unexpected responses.

## 🥘 Ingredients

- CurlDotNet package
- Understanding of try-catch
- 15 minutes

## 📖 The Recipe

### Step 1: Basic Error Handling

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        try
        {
            var result = await Curl.ExecuteAsync("curl https://api.example.com");

            if (result.IsSuccess)
            {
                Console.WriteLine("✓ Request successful!");
                Console.WriteLine(result.Body);
            }
            else
            {
                Console.WriteLine($"✗ Request failed with status {result.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error: {ex.Message}");
        }
    }
}
```

### Step 2: Specific Exception Handling

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class RobustErrorHandling
{
    static async Task Main()
    {
        try
        {
            var result = await Curl.ExecuteAsync("curl https://api.example.com");
            Console.WriteLine("Success!");
        }
        catch (CurlDnsException ex)
        {
            Console.WriteLine($"DNS Error: Could not resolve hostname");
            Console.WriteLine($"Details: {ex.Message}");
            // Link: https://github.com/jacob-mellor/curl-dot-net/docs/troubleshooting/common-issues.md#dns-errors
        }
        catch (CurlTimeoutException ex)
        {
            Console.WriteLine($"Timeout: Request took too long");
            Console.WriteLine($"Details: {ex.Message}");
            // Link: https://github.com/jacob-mellor/curl-dot-net/docs/troubleshooting/common-issues.md#timeout-errors
        }
        catch (CurlSslException ex)
        {
            Console.WriteLine($"SSL Error: Certificate problem");
            Console.WriteLine($"Details: {ex.Message}");
            // Link: https://github.com/jacob-mellor/curl-dot-net/docs/troubleshooting/common-issues.md#ssl-errors
        }
        catch (CurlException ex)
        {
            Console.WriteLine($"Curl Error: {ex.Message}");
            Console.WriteLine($"Error Code: {ex.ErrorCode}");
        }
    }
}
```

## 🍳 Complete Examples

### Example 1: Retry on Failure

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class RetryExample
{
    static async Task<CurlResult> ExecuteWithRetry(
        string url,
        int maxRetries = 3,
        int delaySeconds = 2)
    {
        Exception lastException = null;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                Console.WriteLine($"Attempt {attempt}/{maxRetries}...");

                var result = await Curl.ExecuteAsync($"curl {url}");

                if (result.IsSuccess)
                {
                    Console.WriteLine("✓ Request succeeded");
                    return result;
                }

                // Retry on 5xx errors (server errors)
                if (result.StatusCode >= 500 && attempt < maxRetries)
                {
                    Console.WriteLine($"Server error {result.StatusCode}, retrying...");
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds * attempt));
                    continue;
                }

                return result;
            }
            catch (CurlTimeoutException ex)
            {
                lastException = ex;
                Console.WriteLine($"Timeout on attempt {attempt}");

                if (attempt < maxRetries)
                {
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds * attempt));
                }
            }
            catch (CurlException ex) when (ex.ErrorCode == 7) // Connection failed
            {
                lastException = ex;
                Console.WriteLine($"Connection failed on attempt {attempt}");

                if (attempt < maxRetries)
                {
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds * attempt));
                }
            }
        }

        throw lastException ?? new Exception("All retry attempts failed");
    }

    static async Task Main()
    {
        try
        {
            var result = await ExecuteWithRetry(
                "https://api.example.com/unstable-endpoint",
                maxRetries: 3,
                delaySeconds: 2
            );

            Console.WriteLine($"Final result: {result.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed after all retries: {ex.Message}");
        }
    }
}
```

### Example 2: Comprehensive Error Handler

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class ComprehensiveErrorHandler
{
    static async Task<bool> MakeRequest(string url)
    {
        try
        {
            var result = await Curl.ExecuteAsync($"curl {url}");

            // Check HTTP status codes
            if (result.StatusCode == 200)
            {
                Console.WriteLine("✓ Success!");
                return true;
            }
            else if (result.StatusCode == 401)
            {
                Console.WriteLine("✗ Unauthorized - check your credentials");
                return false;
            }
            else if (result.StatusCode == 403)
            {
                Console.WriteLine("✗ Forbidden - you don't have permission");
                return false;
            }
            else if (result.StatusCode == 404)
            {
                Console.WriteLine("✗ Not Found - resource doesn't exist");
                return false;
            }
            else if (result.StatusCode == 429)
            {
                Console.WriteLine("✗ Rate Limited - too many requests");

                // Check Retry-After header
                if (result.Headers.TryGetValue("Retry-After", out string retryAfter))
                {
                    Console.WriteLine($"Retry after {retryAfter} seconds");
                }
                return false;
            }
            else if (result.StatusCode >= 500)
            {
                Console.WriteLine($"✗ Server Error {result.StatusCode} - try again later");
                return false;
            }
            else
            {
                Console.WriteLine($"✗ Unexpected status: {result.StatusCode}");
                return false;
            }
        }
        catch (CurlDnsException ex)
        {
            Console.WriteLine("✗ DNS Error: Could not resolve hostname");
            Console.WriteLine("  Check your internet connection and URL spelling");
            Console.WriteLine($"  Hostname: {ex.Hostname}");
            return false;
        }
        catch (CurlTimeoutException ex)
        {
            Console.WriteLine("✗ Timeout: Request took too long");
            Console.WriteLine("  The server may be slow or not responding");
            Console.WriteLine($"  Timeout: {ex.Timeout}");
            return false;
        }
        catch (CurlSslException ex)
        {
            Console.WriteLine("✗ SSL Error: Certificate problem");
            Console.WriteLine("  The server's SSL certificate could not be verified");
            Console.WriteLine($"  Details: {ex.CertificateError}");
            return false;
        }
        catch (CurlHttpReturnedErrorException ex)
        {
            Console.WriteLine($"✗ HTTP Error {ex.StatusCode}");
            Console.WriteLine($"  Response: {ex.ResponseBody}");
            return false;
        }
        catch (CurlException ex)
        {
            Console.WriteLine($"✗ Curl Error {ex.ErrorCode}: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Unexpected Error: {ex.Message}");
            Console.WriteLine($"  Type: {ex.GetType().Name}");
            return false;
        }
    }

    static async Task Main()
    {
        bool success = await MakeRequest("https://api.example.com");

        if (success)
        {
            Console.WriteLine("\nContinuing with normal flow...");
        }
        else
        {
            Console.WriteLine("\nHandling failure case...");
        }
    }
}
```

### Example 3: Circuit Breaker Pattern

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class CircuitBreaker
{
    private int _failureCount = 0;
    private DateTime _lastFailureTime = DateTime.MinValue;
    private readonly int _failureThreshold;
    private readonly TimeSpan _timeout;

    public CircuitBreaker(int failureThreshold = 5, TimeSpan? timeout = null)
    {
        _failureThreshold = failureThreshold;
        _timeout = timeout ?? TimeSpan.FromMinutes(1);
    }

    public async Task<CurlResult> ExecuteAsync(string url)
    {
        // Check if circuit is open
        if (_failureCount >= _failureThreshold)
        {
            var timeSinceLastFailure = DateTime.UtcNow - _lastFailureTime;

            if (timeSinceLastFailure < _timeout)
            {
                throw new Exception(
                    $"Circuit breaker is open. Too many failures. " +
                    $"Retry after {(_timeout - timeSinceLastFailure).TotalSeconds:F0} seconds"
                );
            }

            // Reset after timeout
            Console.WriteLine("Circuit breaker attempting reset...");
            _failureCount = 0;
        }

        try
        {
            var result = await Curl.ExecuteAsync($"curl {url}");

            if (result.IsSuccess)
            {
                _failureCount = 0; // Reset on success
                return result;
            }

            // Count 5xx errors as failures
            if (result.StatusCode >= 500)
            {
                _failureCount++;
                _lastFailureTime = DateTime.UtcNow;
                Console.WriteLine($"Failure {_failureCount}/{_failureThreshold}");
            }

            return result;
        }
        catch (CurlException)
        {
            _failureCount++;
            _lastFailureTime = DateTime.UtcNow;
            Console.WriteLine($"Failure {_failureCount}/{_failureThreshold}");
            throw;
        }
    }
}

class CircuitBreakerExample
{
    static async Task Main()
    {
        var circuitBreaker = new CircuitBreaker(
            failureThreshold: 3,
            timeout: TimeSpan.FromSeconds(30)
        );

        // Simulate multiple requests
        for (int i = 1; i <= 10; i++)
        {
            try
            {
                Console.WriteLine($"\nRequest {i}:");
                var result = await circuitBreaker.ExecuteAsync(
                    "https://httpbin.org/status/500" // Always fails
                );
                Console.WriteLine($"Status: {result.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            await Task.Delay(1000); // Wait 1 second between requests
        }
    }
}
```

### Example 4: Fallback Pattern

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class FallbackPattern
{
    static async Task<string> GetDataWithFallback()
    {
        // Try primary endpoint
        try
        {
            Console.WriteLine("Trying primary endpoint...");
            var result = await Curl.ExecuteAsync("curl https://api.primary.com/data");

            if (result.IsSuccess)
            {
                Console.WriteLine("✓ Got data from primary");
                return result.Body;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Primary failed: {ex.Message}");
        }

        // Try secondary endpoint
        try
        {
            Console.WriteLine("Trying secondary endpoint...");
            var result = await Curl.ExecuteAsync("curl https://api.secondary.com/data");

            if (result.IsSuccess)
            {
                Console.WriteLine("✓ Got data from secondary");
                return result.Body;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Secondary failed: {ex.Message}");
        }

        // Try cache
        try
        {
            Console.WriteLine("Trying cache...");
            if (System.IO.File.Exists("cache.json"))
            {
                string cached = await System.IO.File.ReadAllTextAsync("cache.json");
                Console.WriteLine("✓ Got data from cache");
                return cached;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cache failed: {ex.Message}");
        }

        // Return default data
        Console.WriteLine("⚠ Returning default data");
        return "{\"data\": [], \"error\": \"All sources unavailable\"}";
    }

    static async Task Main()
    {
        string data = await GetDataWithFallback();
        Console.WriteLine($"\nFinal data: {data}");
    }
}
```

### Example 5: Logging and Monitoring

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class ErrorLogging
{
    static void LogError(string level, string message, Exception ex = null)
    {
        string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        string logMessage = $"[{timestamp}] {level}: {message}";

        if (ex != null)
        {
            logMessage += $"\n  Exception: {ex.GetType().Name}";
            logMessage += $"\n  Message: {ex.Message}";

            if (ex is CurlException curlEx)
            {
                logMessage += $"\n  ErrorCode: {curlEx.ErrorCode}";
            }

            if (ex.StackTrace != null)
            {
                logMessage += $"\n  StackTrace: {ex.StackTrace}";
            }
        }

        Console.WriteLine(logMessage);

        // Also write to file
        System.IO.File.AppendAllText("errors.log", logMessage + "\n\n");
    }

    static async Task<CurlResult> MakeRequestWithLogging(string url)
    {
        try
        {
            LogError("INFO", $"Making request to {url}");

            var result = await Curl.ExecuteAsync($"curl {url}");

            if (result.IsSuccess)
            {
                LogError("INFO", $"Request successful: {result.StatusCode}");
            }
            else
            {
                LogError("WARNING", $"Request failed: {result.StatusCode}");
            }

            return result;
        }
        catch (CurlDnsException ex)
        {
            LogError("ERROR", "DNS resolution failed", ex);
            throw;
        }
        catch (CurlTimeoutException ex)
        {
            LogError("ERROR", "Request timed out", ex);
            throw;
        }
        catch (CurlSslException ex)
        {
            LogError("ERROR", "SSL certificate error", ex);
            throw;
        }
        catch (CurlException ex)
        {
            LogError("ERROR", "Curl error occurred", ex);
            throw;
        }
        catch (Exception ex)
        {
            LogError("CRITICAL", "Unexpected error", ex);
            throw;
        }
    }

    static async Task Main()
    {
        try
        {
            await MakeRequestWithLogging("https://api.example.com");
        }
        catch
        {
            Console.WriteLine("\nCheck errors.log for details");
        }
    }
}
```

### Example 6: Validation and Defensive Programming

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

class DefensiveProgramming
{
    static bool ValidateUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            Console.WriteLine("Error: URL is empty");
            return false;
        }

        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
        {
            Console.WriteLine("Error: Invalid URL format");
            return false;
        }

        if (uri.Scheme != "https" && uri.Scheme != "http")
        {
            Console.WriteLine("Error: URL must be HTTP or HTTPS");
            return false;
        }

        return true;
    }

    static async Task<CurlResult> SafeRequest(string url)
    {
        // Validate input
        if (!ValidateUrl(url))
        {
            throw new ArgumentException("Invalid URL", nameof(url));
        }

        try
        {
            var result = await Curl.ExecuteAsync($"curl {url}");

            // Validate response
            if (result == null)
            {
                throw new Exception("Result is null");
            }

            if (result.Body == null)
            {
                Console.WriteLine("Warning: Response body is null");
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            throw;
        }
    }

    static async Task Main()
    {
        string[] urls = {
            "https://api.example.com",     // Valid
            "",                             // Empty
            "not-a-url",                   // Invalid
            "ftp://example.com",           // Wrong protocol
            "https://api.github.com"       // Valid
        };

        foreach (var url in urls)
        {
            Console.WriteLine($"\nTesting: {url}");
            try
            {
                var result = await SafeRequest(url);
                Console.WriteLine($"✓ Success: {result.StatusCode}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Failed: {ex.Message}");
            }
        }
    }
}
```

## 🎓 Error Handling Best Practices

### 1. Always Use Try-Catch

```csharp
// Never do this
var result = await Curl.ExecuteAsync("curl https://api.example.com");

// Always do this
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

### 2. Handle Specific Exceptions First

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
}
catch (CurlDnsException ex)
{
    // Handle DNS errors
}
catch (CurlTimeoutException ex)
{
    // Handle timeouts
}
catch (CurlException ex)
{
    // Handle all other curl errors
}
catch (Exception ex)
{
    // Handle unexpected errors
}
```

### 3. Provide Actionable Error Messages

```csharp
catch (CurlDnsException ex)
{
    Console.WriteLine("Could not connect to the server.");
    Console.WriteLine("Please check:");
    Console.WriteLine("1. Your internet connection");
    Console.WriteLine("2. The URL spelling");
    Console.WriteLine("3. Your DNS settings");
    Console.WriteLine($"\nTechnical details: {ex.Message}");
    Console.WriteLine($"Documentation: https://github.com/jacob-mellor/curl-dot-net/docs/troubleshooting/common-issues.md#dns-errors");
}
```

### 4. Check Status Codes

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");

if (!result.IsSuccess)
{
    if (result.StatusCode == 404)
    {
        Console.WriteLine("Resource not found");
    }
    else if (result.StatusCode == 401)
    {
        Console.WriteLine("Authentication required");
    }
    else if (result.StatusCode >= 500)
    {
        Console.WriteLine("Server error - try again later");
    }
}
```

### 5. Implement Retry Logic for Transient Errors

```csharp
// Retry on network errors and 5xx server errors
// Don't retry on 4xx client errors
```

## 📊 Common Error Scenarios

| Error Type | When It Happens | How to Handle |
|------------|----------------|---------------|
| CurlDnsException | Hostname can't be resolved | Check URL, internet connection |
| CurlTimeoutException | Request takes too long | Increase timeout, check network |
| CurlSslException | SSL certificate problem | Update certificates, check server |
| HTTP 401 | Authentication failed | Check credentials, refresh token |
| HTTP 403 | No permission | Check permissions, contact admin |
| HTTP 404 | Resource not found | Verify URL, check if resource exists |
| HTTP 429 | Rate limited | Implement rate limiting, wait and retry |
| HTTP 500 | Server error | Retry with backoff, contact support |

## 🚀 Next Steps

Now that you can handle errors:

1. Build [Robust API Client](call-api.html)
2. Learn [Simple GET](simple-get.html) with error handling
3. Explore [Send JSON](send-json.html) with validation
4. Check [Troubleshooting Guide](../../troubleshooting/)

## 📚 Related Recipes

- [Simple GET](simple-get.html) - Basic requests with error handling
- [Call API](call-api.html) - Building robust API clients
- [Send JSON](send-json.html) - JSON requests with validation

## 🎓 Key Takeaways

- Always use try-catch for network requests
- Handle specific exception types for better error messages
- Check HTTP status codes
- Implement retry logic for transient errors
- Use circuit breaker pattern to prevent cascading failures
- Provide fallback options when possible
- Log errors for debugging
- Give users actionable error messages
- Validate inputs before making requests

---

**Need help?** Check [Troubleshooting](../../troubleshooting/common-issues.html) | **Have questions?** Ask in [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
