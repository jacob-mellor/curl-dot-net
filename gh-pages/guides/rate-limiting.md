# Rate Limiting Guide

## Overview

Rate limiting restricts the number of API requests you can make within a time period. This guide teaches you how to handle rate limits effectively when using CurlDotNet.

## Understanding Rate Limits

### What is Rate Limiting?

Rate limiting protects servers from abuse and ensures fair resource distribution among users.

### Common Rate Limit Types

| Type | Description | Example | Use Case |
|------|-------------|---------|----------|
| **Fixed Window** | X requests per time window | 100 req/hour | Simple APIs |
| **Sliding Window** | X requests per rolling period | 100 req/60min | Fair distribution |
| **Token Bucket** | Tokens refill over time | 10/sec burst, 100/hour sustained | Flexible limits |
| **Concurrent Requests** | Max simultaneous connections | 5 concurrent | Resource protection |

### Rate Limit HTTP Headers

Most APIs communicate rate limits through HTTP headers:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");

// Common rate limit headers
var limit = result.GetHeader("X-RateLimit-Limit");           // Total allowed
var remaining = result.GetHeader("X-RateLimit-Remaining");   // Requests left
var reset = result.GetHeader("X-RateLimit-Reset");           // When limit resets
var retryAfter = result.GetHeader("Retry-After");            // Seconds to wait
```

## Detecting Rate Limits

### HTTP 429 Status Code

The standard rate limit response:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://api.example.com");

if (result.StatusCode == 429)
{
    Console.WriteLine("Rate limit exceeded!");

    // Check when we can retry
    var retryAfter = result.GetHeader("Retry-After");
    if (!string.IsNullOrEmpty(retryAfter))
    {
        Console.WriteLine($"Retry after {retryAfter} seconds");
    }
}
```

### Other Rate Limit Indicators

```csharp
// Some APIs use 403 Forbidden
if (result.StatusCode == 403 && result.Body.Contains("rate limit"))
{
    Console.WriteLine("Rate limited (403 response)");
}

// Some use 503 Service Unavailable
if (result.StatusCode == 503)
{
    Console.WriteLine("Service unavailable (possibly rate limited)");
}
```

## Reading Rate Limit Information

### Extracting Rate Limit Details

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class RateLimitInfo
{
    public int Limit { get; set; }
    public int Remaining { get; set; }
    public DateTime Reset { get; set; }
    public int RetryAfterSeconds { get; set; }

    public static RateLimitInfo FromResult(CurlResult result)
    {
        var info = new RateLimitInfo();

        // Parse limit
        if (int.TryParse(result.GetHeader("X-RateLimit-Limit"), out int limit))
        {
            info.Limit = limit;
        }

        // Parse remaining
        if (int.TryParse(result.GetHeader("X-RateLimit-Remaining"), out int remaining))
        {
            info.Remaining = remaining;
        }

        // Parse reset timestamp
        if (long.TryParse(result.GetHeader("X-RateLimit-Reset"), out long resetTimestamp))
        {
            info.Reset = DateTimeOffset.FromUnixTimeSeconds(resetTimestamp).DateTime;
        }

        // Parse retry-after
        if (int.TryParse(result.GetHeader("Retry-After"), out int retryAfter))
        {
            info.RetryAfterSeconds = retryAfter;
        }

        return info;
    }

    public void Display()
    {
        Console.WriteLine($"Rate Limit: {Remaining}/{Limit}");
        if (Reset != DateTime.MinValue)
        {
            var timeUntilReset = Reset - DateTime.UtcNow;
            Console.WriteLine($"Resets in: {timeUntilReset.TotalMinutes:F1} minutes");
        }
        if (RetryAfterSeconds > 0)
        {
            Console.WriteLine($"Retry after: {RetryAfterSeconds} seconds");
        }
    }
}

// Usage
var result = await Curl.ExecuteAsync("curl https://api.github.com");
var rateLimitInfo = RateLimitInfo.FromResult(result);
rateLimitInfo.Display();
```

## Handling Rate Limits

### Strategy 1: Exponential Backoff

Gradually increase wait time between retries:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class ExponentialBackoff
{
    public static async Task<CurlResult> ExecuteWithBackoffAsync(
        string curlCommand,
        int maxRetries = 5)
    {
        int delay = 1000; // Start with 1 second

        for (int attempt = 0; attempt <= maxRetries; attempt++)
        {
            var result = await Curl.ExecuteAsync(curlCommand);

            // Success - return immediately
            if (result.StatusCode != 429)
            {
                return result;
            }

            // Rate limited - check if we should retry
            if (attempt == maxRetries)
            {
                throw new Exception($"Rate limit exceeded after {maxRetries} retries");
            }

            // Calculate wait time
            var retryAfter = result.GetHeader("Retry-After");
            int waitTime = delay;

            if (!string.IsNullOrEmpty(retryAfter) && int.TryParse(retryAfter, out int retrySeconds))
            {
                waitTime = retrySeconds * 1000;
            }

            Console.WriteLine($"Rate limited. Waiting {waitTime / 1000} seconds before retry {attempt + 1}/{maxRetries}...");
            await Task.Delay(waitTime);

            // Exponential increase
            delay *= 2;
        }

        throw new Exception("Max retries exceeded");
    }
}

// Usage
try
{
    var result = await ExponentialBackoff.ExecuteWithBackoffAsync(
        "curl https://api.example.com/data"
    );
    Console.WriteLine($"Success! Status: {result.StatusCode}");
}
catch (Exception ex)
{
    Console.WriteLine($"Failed: {ex.Message}");
}
```

### Strategy 2: Respect Rate Limit Headers

Proactively avoid hitting limits:

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class RateLimitAwareClient
{
    private int _remaining = int.MaxValue;
    private DateTime _reset = DateTime.MinValue;

    public async Task<CurlResult> ExecuteAsync(string curlCommand)
    {
        // Wait if we're at the limit
        await WaitIfNeededAsync();

        // Execute request
        var result = await Curl.ExecuteAsync(curlCommand);

        // Update rate limit info
        UpdateRateLimitInfo(result);

        return result;
    }

    private async Task WaitIfNeededAsync()
    {
        if (_remaining <= 0 && _reset > DateTime.UtcNow)
        {
            var waitTime = _reset - DateTime.UtcNow;
            Console.WriteLine($"Rate limit reached. Waiting {waitTime.TotalSeconds:F0} seconds...");
            await Task.Delay(waitTime);
            _remaining = int.MaxValue; // Reset after waiting
        }
    }

    private void UpdateRateLimitInfo(CurlResult result)
    {
        if (int.TryParse(result.GetHeader("X-RateLimit-Remaining"), out int remaining))
        {
            _remaining = remaining;
            Console.WriteLine($"Rate limit: {remaining} requests remaining");
        }

        if (long.TryParse(result.GetHeader("X-RateLimit-Reset"), out long resetTimestamp))
        {
            _reset = DateTimeOffset.FromUnixTimeSeconds(resetTimestamp).DateTime;
        }
    }
}

// Usage
var client = new RateLimitAwareClient();

for (int i = 0; i < 100; i++)
{
    var result = await client.ExecuteAsync("curl https://api.github.com");
    Console.WriteLine($"Request {i + 1}: Status {result.StatusCode}");
}
```

### Strategy 3: Request Queuing

Queue requests to maintain consistent rate:

```csharp
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet;

public class RateLimitedQueue
{
    private readonly ConcurrentQueue<Func<Task<CurlResult>>> _queue = new();
    private readonly SemaphoreSlim _semaphore;
    private readonly int _requestsPerSecond;

    public RateLimitedQueue(int requestsPerSecond)
    {
        _requestsPerSecond = requestsPerSecond;
        _semaphore = new SemaphoreSlim(requestsPerSecond);
    }

    public async Task<CurlResult> EnqueueAsync(string curlCommand)
    {
        var tcs = new TaskCompletionSource<CurlResult>();

        _queue.Enqueue(async () =>
        {
            try
            {
                await _semaphore.WaitAsync();
                var result = await Curl.ExecuteAsync(curlCommand);
                tcs.SetResult(result);
                return result;
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
                throw;
            }
            finally
            {
                // Release after delay to maintain rate
                _ = Task.Delay(1000 / _requestsPerSecond).ContinueWith(_ => _semaphore.Release());
            }
        });

        // Start processing if not already running
        _ = ProcessQueueAsync();

        return await tcs.Task;
    }

    private async Task ProcessQueueAsync()
    {
        while (_queue.TryDequeue(out var request))
        {
            await request();
        }
    }
}

// Usage
var queue = new RateLimitedQueue(requestsPerSecond: 10);

// These will automatically be rate-limited to 10 req/sec
var tasks = new List<Task<CurlResult>>();
for (int i = 0; i < 100; i++)
{
    tasks.Add(queue.EnqueueAsync($"curl https://api.example.com/item/{i}"));
}

var results = await Task.WhenAll(tasks);
Console.WriteLine($"Completed {results.Length} requests");
```

## Advanced Rate Limiting Patterns

### Token Bucket Implementation

Allows bursts while maintaining average rate:

```csharp
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet;

public class TokenBucket
{
    private readonly int _capacity;
    private readonly int _refillRate;
    private int _tokens;
    private DateTime _lastRefill;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public TokenBucket(int capacity, int refillRatePerSecond)
    {
        _capacity = capacity;
        _refillRate = refillRatePerSecond;
        _tokens = capacity;
        _lastRefill = DateTime.UtcNow;
    }

    public async Task<CurlResult> ExecuteAsync(string curlCommand)
    {
        await AcquireTokenAsync();
        return await Curl.ExecuteAsync(curlCommand);
    }

    private async Task AcquireTokenAsync()
    {
        await _lock.WaitAsync();
        try
        {
            // Refill tokens based on time elapsed
            var now = DateTime.UtcNow;
            var elapsed = (now - _lastRefill).TotalSeconds;
            var tokensToAdd = (int)(elapsed * _refillRate);

            if (tokensToAdd > 0)
            {
                _tokens = Math.Min(_capacity, _tokens + tokensToAdd);
                _lastRefill = now;
            }

            // Wait if no tokens available
            while (_tokens <= 0)
            {
                _lock.Release();
                await Task.Delay(100);
                await _lock.WaitAsync();

                // Try to refill again
                now = DateTime.UtcNow;
                elapsed = (now - _lastRefill).TotalSeconds;
                tokensToAdd = (int)(elapsed * _refillRate);

                if (tokensToAdd > 0)
                {
                    _tokens = Math.Min(_capacity, _tokens + tokensToAdd);
                    _lastRefill = now;
                }
            }

            // Consume a token
            _tokens--;
        }
        finally
        {
            _lock.Release();
        }
    }
}

// Usage
var bucket = new TokenBucket(capacity: 10, refillRatePerSecond: 2);

// Can burst up to 10 requests, then limited to 2 per second
for (int i = 0; i < 20; i++)
{
    var sw = Stopwatch.StartNew();
    var result = await bucket.ExecuteAsync($"curl https://api.example.com/item/{i}");
    Console.WriteLine($"Request {i + 1}: {sw.ElapsedMilliseconds}ms");
}
```

### Per-Endpoint Rate Limiting

Different endpoints may have different limits:

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurlDotNet;

public class MultiEndpointRateLimiter
{
    private readonly Dictionary<string, RateLimitAwareClient> _clients = new();

    public async Task<CurlResult> ExecuteAsync(string endpoint, string curlCommand)
    {
        if (!_clients.ContainsKey(endpoint))
        {
            _clients[endpoint] = new RateLimitAwareClient();
        }

        return await _clients[endpoint].ExecuteAsync(curlCommand);
    }
}

// Usage
var limiter = new MultiEndpointRateLimiter();

// Each endpoint tracked separately
var result1 = await limiter.ExecuteAsync("search", "curl https://api.example.com/search");
var result2 = await limiter.ExecuteAsync("users", "curl https://api.example.com/users");
```

## Monitoring Rate Limits

### Rate Limit Dashboard

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurlDotNet;

public class RateLimitMonitor
{
    private readonly List<RateLimitSnapshot> _snapshots = new();

    public class RateLimitSnapshot
    {
        public DateTime Timestamp { get; set; }
        public int Remaining { get; set; }
        public int Limit { get; set; }
        public double PercentageUsed => Limit > 0 ? ((Limit - Remaining) * 100.0 / Limit) : 0;
    }

    public async Task<CurlResult> ExecuteAndMonitorAsync(string curlCommand)
    {
        var result = await Curl.ExecuteAsync(curlCommand);

        // Record snapshot
        if (int.TryParse(result.GetHeader("X-RateLimit-Remaining"), out int remaining) &&
            int.TryParse(result.GetHeader("X-RateLimit-Limit"), out int limit))
        {
            _snapshots.Add(new RateLimitSnapshot
            {
                Timestamp = DateTime.UtcNow,
                Remaining = remaining,
                Limit = limit
            });
        }

        return result;
    }

    public void DisplayReport()
    {
        if (_snapshots.Count == 0)
        {
            Console.WriteLine("No rate limit data collected");
            return;
        }

        Console.WriteLine("\n=== Rate Limit Report ===");
        Console.WriteLine($"Total Requests: {_snapshots.Count}");

        var latest = _snapshots[^1];
        Console.WriteLine($"\nCurrent Status:");
        Console.WriteLine($"  Remaining: {latest.Remaining}/{latest.Limit}");
        Console.WriteLine($"  Used: {latest.PercentageUsed:F1}%");

        // Show trend
        if (_snapshots.Count >= 2)
        {
            var first = _snapshots[0];
            var requestsMade = first.Remaining - latest.Remaining;
            var duration = (latest.Timestamp - first.Timestamp).TotalSeconds;
            var rate = duration > 0 ? requestsMade / duration : 0;

            Console.WriteLine($"\nUsage Statistics:");
            Console.WriteLine($"  Requests Made: {requestsMade}");
            Console.WriteLine($"  Duration: {duration:F1} seconds");
            Console.WriteLine($"  Average Rate: {rate:F2} req/sec");
        }

        Console.WriteLine("========================\n");
    }
}

// Usage
var monitor = new RateLimitMonitor();

for (int i = 0; i < 50; i++)
{
    await monitor.ExecuteAndMonitorAsync("curl https://api.github.com");
    await Task.Delay(100);
}

monitor.DisplayReport();
```

## API-Specific Examples

### GitHub API

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class GitHubRateLimiter
{
    private const string GITHUB_API = "https://api.github.com";

    public static async Task<CurlResult> ExecuteAsync(string endpoint, string token = null)
    {
        var command = $"curl -H 'Accept: application/vnd.github.v3+json'";

        if (!string.IsNullOrEmpty(token))
        {
            command += $" -H 'Authorization: Bearer {token}'";
        }

        command += $" {GITHUB_API}{endpoint}";

        var result = await Curl.ExecuteAsync(command);

        // GitHub returns detailed rate limit info
        var remaining = result.GetHeader("X-RateLimit-Remaining");
        var limit = result.GetHeader("X-RateLimit-Limit");
        var reset = result.GetHeader("X-RateLimit-Reset");

        Console.WriteLine($"GitHub Rate Limit: {remaining}/{limit}");

        if (long.TryParse(reset, out long resetTimestamp))
        {
            var resetTime = DateTimeOffset.FromUnixTimeSeconds(resetTimestamp);
            var timeUntilReset = resetTime - DateTimeOffset.UtcNow;
            Console.WriteLine($"Resets in: {timeUntilReset.TotalMinutes:F1} minutes");
        }

        return result;
    }

    public static async Task<RateLimitInfo> CheckRateLimitAsync(string token = null)
    {
        var result = await ExecuteAsync("/rate_limit", token);
        return RateLimitInfo.FromResult(result);
    }
}

// Usage
var result = await GitHubRateLimiter.ExecuteAsync("/users/octocat");
var rateLimitStatus = await GitHubRateLimiter.CheckRateLimitAsync();
```

### Twitter API

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class TwitterRateLimiter
{
    // Twitter uses 15-minute windows
    private DateTime _windowReset = DateTime.MinValue;
    private int _remaining = int.MaxValue;

    public async Task<CurlResult> ExecuteAsync(string endpoint, string bearerToken)
    {
        // Wait if we're rate limited
        if (_remaining <= 0 && _windowReset > DateTime.UtcNow)
        {
            var waitTime = _windowReset - DateTime.UtcNow;
            Console.WriteLine($"Twitter rate limit reached. Waiting {waitTime.TotalSeconds:F0}s...");
            await Task.Delay(waitTime);
        }

        var command = $"curl -H 'Authorization: Bearer {bearerToken}' {endpoint}";
        var result = await Curl.ExecuteAsync(command);

        // Update rate limit from headers
        if (int.TryParse(result.GetHeader("x-rate-limit-remaining"), out int remaining))
        {
            _remaining = remaining;
        }

        if (long.TryParse(result.GetHeader("x-rate-limit-reset"), out long resetTimestamp))
        {
            _windowReset = DateTimeOffset.FromUnixTimeSeconds(resetTimestamp).DateTime;
        }

        Console.WriteLine($"Twitter Rate Limit: {_remaining} remaining");

        return result;
    }
}
```

## Best Practices

### 1. Always Check Rate Limit Headers

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");

// Log rate limit info
var remaining = result.GetHeader("X-RateLimit-Remaining");
if (!string.IsNullOrEmpty(remaining))
{
    Console.WriteLine($"Requests remaining: {remaining}");
}
```

### 2. Implement Graceful Degradation

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");

if (result.StatusCode == 429)
{
    // Fall back to cached data
    Console.WriteLine("Using cached data due to rate limit");
    return GetCachedData();
}

return result;
```

### 3. Use Authentication for Higher Limits

```csharp
// Without auth: 60 requests/hour
var result1 = await Curl.ExecuteAsync("curl https://api.github.com");

// With auth: 5000 requests/hour
var result2 = await Curl.ExecuteAsync(
    "curl -H 'Authorization: Bearer YOUR_TOKEN' https://api.github.com"
);
```

### 4. Batch Requests When Possible

```csharp
// Bad: Multiple requests
for (int i = 0; i < 100; i++)
{
    await Curl.ExecuteAsync($"curl https://api.example.com/user/{i}");
}

// Good: Single batch request
var result = await Curl.ExecuteAsync(
    "curl -X POST https://api.example.com/users/batch -d '{\"ids\": [1,2,3...100]}'"
);
```

### 5. Cache Responses

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurlDotNet;

public class CachedRateLimitedClient
{
    private readonly Dictionary<string, (CurlResult Result, DateTime Expiry)> _cache = new();
    private readonly RateLimitAwareClient _client = new();

    public async Task<CurlResult> ExecuteAsync(string curlCommand, TimeSpan cacheDuration)
    {
        // Check cache first
        if (_cache.TryGetValue(curlCommand, out var cached) && cached.Expiry > DateTime.UtcNow)
        {
            Console.WriteLine("Returning cached result (avoiding rate limit)");
            return cached.Result;
        }

        // Execute and cache
        var result = await _client.ExecuteAsync(curlCommand);
        _cache[curlCommand] = (result, DateTime.UtcNow + cacheDuration);

        return result;
    }
}

// Usage
var client = new CachedRateLimitedClient();
var result = await client.ExecuteAsync(
    "curl https://api.example.com/data",
    cacheDuration: TimeSpan.FromMinutes(5)
);
```

## Testing Rate Limits

### Simulate Rate Limiting

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

public class RateLimitSimulator
{
    private int _requestCount = 0;
    private readonly int _limit;

    public RateLimitSimulator(int requestLimit)
    {
        _limit = requestLimit;
    }

    public async Task<CurlResult> ExecuteAsync(string curlCommand)
    {
        _requestCount++;

        if (_requestCount > _limit)
        {
            // Simulate 429 response
            Console.WriteLine($"SIMULATED: Rate limit exceeded ({_requestCount}/{_limit})");
            throw new Exception("Rate limit exceeded (simulated)");
        }

        Console.WriteLine($"Request {_requestCount}/{_limit}");
        return await Curl.ExecuteAsync(curlCommand);
    }

    public void Reset()
    {
        _requestCount = 0;
    }
}

// Usage for testing
var simulator = new RateLimitSimulator(requestLimit: 10);

try
{
    for (int i = 0; i < 15; i++)
    {
        await simulator.ExecuteAsync("curl https://api.example.com");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Caught expected error: {ex.Message}");
}
```

## Troubleshooting Rate Limits

### Common Issues

**Issue 1: Unexpected 429 Errors**
```csharp
// Check if you're counting requests correctly
var monitor = new RateLimitMonitor();
// Monitor several requests to understand the pattern
```

**Issue 2: Rate Limit Not Resetting**
```csharp
// Verify you're reading the reset time correctly
var reset = result.GetHeader("X-RateLimit-Reset");
var resetTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(reset));
Console.WriteLine($"Rate limit resets at: {resetTime}");
```

**Issue 3: Different Limits for Different Endpoints**
```csharp
// Use per-endpoint tracking
var limiter = new MultiEndpointRateLimiter();
```

## Summary

Effective rate limit handling requires:

1. **Detection** - Monitor rate limit headers and 429 responses
2. **Respect** - Honor rate limits proactively
3. **Retry** - Implement exponential backoff for 429 errors
4. **Queue** - Use request queuing for consistent rates
5. **Cache** - Reduce API calls with intelligent caching
6. **Monitor** - Track usage patterns and optimize
7. **Test** - Simulate rate limits in development

## Related Resources

- [Error Handling Tutorial](../tutorials/06-handling-errors.md)
- [Parallel Requests Tutorial](../tutorials/13-parallel-requests.md)
- [Network Troubleshooting](network-troubleshooting.md)
- [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)

---

**Need help with a specific API's rate limits?** Ask in our [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions).
