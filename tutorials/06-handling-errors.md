# Tutorial 6: Handling Errors

Network requests can fail for many reasons. This tutorial teaches you how to handle errors gracefully and build resilient applications with CurlDotNet.

## Types of Errors

### 1. Network Errors
These occur when the request can't reach the server:

```csharp
var result = await curl.GetAsync("https://unreachable-server.example.com");

if (!result.IsSuccess)
{
    // Common network errors:
    // - "Could not resolve host"
    // - "Connection timeout"
    // - "Connection refused"
    // - "Network is unreachable"
    Console.WriteLine($"Network error: {result.Error}");
}
```

### 2. HTTP Errors
The server responds but with an error status:

```csharp
var result = await curl.GetAsync("https://api.example.com/missing");

// Check both IsSuccess and StatusCode
if (result.StatusCode == HttpStatusCode.NotFound)
{
    Console.WriteLine("Resource not found (404)");
}
else if (result.StatusCode == HttpStatusCode.InternalServerError)
{
    Console.WriteLine("Server error (500)");
}
```

### 3. Timeout Errors
The request takes too long:

```csharp
var curl = new Curl
{
    Timeout = TimeSpan.FromSeconds(5)  // 5 second timeout
};

var result = await curl.GetAsync("https://slow-api.example.com");

if (!result.IsSuccess && result.Error.Contains("timeout"))
{
    Console.WriteLine("Request timed out after 5 seconds");
}
```

## Error Handling Patterns

### Basic Error Handling
```csharp
public async Task<string> GetDataSafely(string url)
{
    var curl = new Curl();
    var result = await curl.GetAsync(url);

    if (result.IsSuccess)
    {
        return result.Data;
    }
    else
    {
        // Log the error
        _logger.LogError($"Failed to get {url}: {result.Error}");

        // Return a default or throw
        return null;  // or throw new ApplicationException(result.Error);
    }
}
```

### Detailed Error Handling
```csharp
public async Task<User> GetUserWithErrorHandling(int userId)
{
    var curl = new Curl();
    var result = await curl.GetAsync($"/api/users/{userId}");

    // Handle specific status codes
    switch (result.StatusCode)
    {
        case HttpStatusCode.OK:
            return JsonSerializer.Deserialize<User>(result.Data);

        case HttpStatusCode.NotFound:
            // User doesn't exist - might be expected
            _logger.LogInformation($"User {userId} not found");
            return null;

        case HttpStatusCode.Unauthorized:
            // Need to authenticate
            throw new UnauthorizedException("Please log in");

        case HttpStatusCode.TooManyRequests:
            // Rate limited
            throw new RateLimitException("Too many requests, please wait");

        case HttpStatusCode.InternalServerError:
        case HttpStatusCode.BadGateway:
        case HttpStatusCode.ServiceUnavailable:
            // Server errors - might want to retry
            throw new ServerException($"Server error: {result.StatusCode}");

        default:
            // Unexpected error
            throw new ApplicationException($"Unexpected error: {result.StatusCode} - {result.Error}");
    }
}
```

## Retry Logic

### Simple Retry
```csharp
public async Task<CurlResult> GetWithRetry(string url, int maxRetries = 3)
{
    var curl = new Curl();

    for (int attempt = 0; attempt < maxRetries; attempt++)
    {
        var result = await curl.GetAsync(url);

        if (result.IsSuccess)
        {
            return result;
        }

        // Log retry attempt
        _logger.LogWarning($"Attempt {attempt + 1} failed: {result.Error}");

        // Wait before retrying (except on last attempt)
        if (attempt < maxRetries - 1)
        {
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));  // Exponential backoff
        }
    }

    // All retries failed
    throw new ApplicationException($"Failed after {maxRetries} attempts");
}
```

### Smart Retry (Only for Retryable Errors)
```csharp
public async Task<CurlResult> SmartRetry(string url, int maxRetries = 3)
{
    var curl = new Curl();

    for (int attempt = 0; attempt < maxRetries; attempt++)
    {
        var result = await curl.GetAsync(url);

        if (result.IsSuccess)
        {
            return result;
        }

        // Check if error is retryable
        bool shouldRetry = IsRetryableError(result);

        if (!shouldRetry)
        {
            // Don't retry for permanent failures
            return result;
        }

        // Calculate delay with exponential backoff
        if (attempt < maxRetries - 1)
        {
            var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
            _logger.LogInformation($"Retrying in {delay.TotalSeconds} seconds...");
            await Task.Delay(delay);
        }
    }

    return new CurlResult { IsSuccess = false, Error = "Max retries exceeded" };
}

private bool IsRetryableError(CurlResult result)
{
    // Network errors are often retryable
    if (result.Error.Contains("timeout") ||
        result.Error.Contains("connection") ||
        result.Error.Contains("network"))
    {
        return true;
    }

    // Some HTTP status codes are retryable
    var retryableStatuses = new[]
    {
        HttpStatusCode.RequestTimeout,
        HttpStatusCode.TooManyRequests,
        HttpStatusCode.InternalServerError,
        HttpStatusCode.BadGateway,
        HttpStatusCode.ServiceUnavailable,
        HttpStatusCode.GatewayTimeout
    };

    return retryableStatuses.Contains(result.StatusCode);
}
```

## Circuit Breaker Pattern

Prevent overwhelming a failing service:

```csharp
public class CircuitBreaker
{
    private int _failureCount = 0;
    private DateTime _lastFailureTime;
    private readonly int _threshold = 5;
    private readonly TimeSpan _timeout = TimeSpan.FromMinutes(1);

    public async Task<CurlResult> ExecuteAsync(Func<Task<CurlResult>> action)
    {
        // Check if circuit is open
        if (_failureCount >= _threshold)
        {
            if (DateTime.UtcNow - _lastFailureTime < _timeout)
            {
                return new CurlResult
                {
                    IsSuccess = false,
                    Error = "Circuit breaker is open - service is down"
                };
            }

            // Reset after timeout
            _failureCount = 0;
        }

        // Try the action
        var result = await action();

        if (result.IsSuccess)
        {
            _failureCount = 0;  // Reset on success
        }
        else
        {
            _failureCount++;
            _lastFailureTime = DateTime.UtcNow;
        }

        return result;
    }
}

// Usage
var breaker = new CircuitBreaker();
var result = await breaker.ExecuteAsync(async () =>
{
    var curl = new Curl();
    return await curl.GetAsync("https://api.example.com/data");
});
```

## Fallback Strategies

### Fallback to Cache
```csharp
public async Task<string> GetDataWithFallback(string url)
{
    var curl = new Curl();
    var result = await curl.GetAsync(url);

    if (result.IsSuccess)
    {
        // Update cache with fresh data
        _cache[url] = result.Data;
        return result.Data;
    }
    else
    {
        // Try to return cached data
        if (_cache.TryGetValue(url, out string cachedData))
        {
            _logger.LogWarning($"Using cached data due to error: {result.Error}");
            return cachedData;
        }

        // No cache available
        throw new ApplicationException($"Request failed and no cache available: {result.Error}");
    }
}
```

### Fallback to Alternative Service
```csharp
public async Task<WeatherData> GetWeatherWithFallback(string city)
{
    var curl = new Curl();

    // Try primary service
    var primaryResult = await curl.GetAsync($"https://primary-weather-api.com/{city}");
    if (primaryResult.IsSuccess)
    {
        return ParseWeatherData(primaryResult.Data);
    }

    _logger.LogWarning("Primary weather service failed, trying backup");

    // Try backup service
    var backupResult = await curl.GetAsync($"https://backup-weather-api.com/{city}");
    if (backupResult.IsSuccess)
    {
        return ParseWeatherData(backupResult.Data);
    }

    // Both failed
    throw new ApplicationException("All weather services are unavailable");
}
```

## Logging and Monitoring

### Structured Logging
```csharp
public async Task<T> ExecuteWithLogging<T>(string url, Func<string, T> parser)
{
    var curl = new Curl();
    var stopwatch = Stopwatch.StartNew();

    try
    {
        var result = await curl.GetAsync(url);
        stopwatch.Stop();

        if (result.IsSuccess)
        {
            _logger.LogInformation("Request succeeded",
                new
                {
                    Url = url,
                    StatusCode = result.StatusCode,
                    Duration = stopwatch.ElapsedMilliseconds,
                    ResponseSize = result.Data?.Length ?? 0
                });

            return parser(result.Data);
        }
        else
        {
            _logger.LogError("Request failed",
                new
                {
                    Url = url,
                    StatusCode = result.StatusCode,
                    Error = result.Error,
                    Duration = stopwatch.ElapsedMilliseconds
                });

            throw new ApplicationException(result.Error);
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error during request",
            new { Url = url, Duration = stopwatch.ElapsedMilliseconds });
        throw;
    }
}
```

## Custom Exception Types

Create specific exceptions for better error handling:

```csharp
public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ResponseBody { get; }

    public ApiException(HttpStatusCode statusCode, string message, string responseBody = null)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}

public class NetworkException : Exception
{
    public NetworkException(string message) : base(message) { }
}

public class TimeoutException : Exception
{
    public TimeSpan Timeout { get; }

    public TimeoutException(TimeSpan timeout)
        : base($"Request timed out after {timeout.TotalSeconds} seconds")
    {
        Timeout = timeout;
    }
}

// Usage
public async Task<T> ExecuteRequest<T>(string url, Func<string, T> parser)
{
    var curl = new Curl { Timeout = TimeSpan.FromSeconds(30) };
    var result = await curl.GetAsync(url);

    if (result.IsSuccess)
    {
        return parser(result.Data);
    }

    // Throw specific exceptions
    if (result.Error.Contains("timeout"))
    {
        throw new TimeoutException(curl.Timeout);
    }
    else if (result.Error.Contains("network") || result.Error.Contains("connection"))
    {
        throw new NetworkException(result.Error);
    }
    else if (result.StatusCode != 0)
    {
        throw new ApiException(result.StatusCode, result.Error, result.Data);
    }
    else
    {
        throw new ApplicationException(result.Error);
    }
}
```

## Best Practices

1. **Always handle errors explicitly** - Don't ignore failed requests
2. **Log errors with context** - Include URL, status code, and timing
3. **Use appropriate retry strategies** - Not all errors should be retried
4. **Implement timeouts** - Prevent requests from hanging indefinitely
5. **Provide fallbacks** - Cache, default values, or alternative services
6. **Monitor failure patterns** - Track error rates and response times
7. **Fail fast for unrecoverable errors** - Don't retry authentication failures
8. **Use circuit breakers** - Protect failing services from overload

## Summary

Proper error handling is essential for building reliable applications. CurlDotNet makes it easy with:
- Clear error messages in `result.Error`
- Status codes for specific handling
- No exceptions to catch for normal failures
- Easy integration with retry and fallback patterns

## What's Next?

In the next tutorial, we'll learn about [working with JSON data](07-json-for-beginners.md), the most common format for modern APIs.

---

[← Previous: Understanding Results](05-understanding-results.md) | [Next: JSON for Beginners →](07-json-for-beginners.md)