---
layout: default
title: Implementing Retry Logic with Exponential Backoff in C# using curl
description: Master retry patterns and exponential backoff strategies for resilient REST API calls with CurlDotNet
keywords: C# retry logic, exponential backoff, curl retry pattern, .NET resilience, API retry strategy
---

# Retry Logic with Exponential Backoff

## Building Resilient API Calls with curl for C# and .NET

Learn how to implement robust retry logic with exponential backoff, jitter, and circuit breaker patterns using CurlDotNet.

## Why Retry Logic Matters

Network calls can fail for many transient reasons:
- Temporary network issues
- Server overload (429 Too Many Requests)
- Brief service outages (503 Service Unavailable)
- Timeout issues
- Connection problems

Proper retry logic can turn a 95% success rate into 99.99% reliability.

## Basic Retry Pattern

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class BasicRetryClient
{
    private readonly Curl _curl;
    private readonly int _maxRetries;

    public BasicRetryClient(string baseUrl, int maxRetries = 3)
    {
        _curl = new Curl(baseUrl);
        _maxRetries = maxRetries;
    }

    public async Task<CurlResult> ExecuteWithRetryAsync(
        Func<Task<CurlResult>> operation)
    {
        int attempt = 0;
        CurlException lastException = null;

        while (attempt < _maxRetries)
        {
            try
            {
                var result = await operation();

                // Success or client error (4xx) - don't retry
                if (result.StatusCode < 500 && result.StatusCode != 429)
                {
                    return result;
                }

                // Server error or rate limit - consider retry
                if (attempt == _maxRetries - 1)
                {
                    return result; // Last attempt, return the error
                }
            }
            catch (CurlException ex)
            {
                lastException = ex;

                // Don't retry on certain exceptions
                if (ex is CurlAuthenticationException ||
                    ex is CurlMalformedUrlException)
                {
                    throw;
                }

                if (attempt == _maxRetries - 1)
                {
                    throw;
                }
            }

            attempt++;
            await Task.Delay(TimeSpan.FromSeconds(attempt)); // Simple backoff
        }

        throw lastException ?? new CurlException("Retry failed");
    }
}
```

## Exponential Backoff Implementation

```csharp
using CurlDotNet;
using System.Security.Cryptography;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class ExponentialBackoffRetry
{
    private readonly Curl _curl;
    private readonly RetryOptions _options;

    public class RetryOptions
    {
        public int MaxAttempts { get; set; } = 3;
        public int InitialDelayMs { get; set; } = 1000;
        public int MaxDelayMs { get; set; } = 30000;
        public double BackoffMultiplier { get; set; } = 2.0;
        public bool UseJitter { get; set; } = true;
        public int[] RetryStatusCodes { get; set; } = { 429, 500, 502, 503, 504 };
    }

    public ExponentialBackoffRetry(string baseUrl, RetryOptions options = null)
    {
        _curl = new Curl(baseUrl);
        _options = options ?? new RetryOptions();
    }

    public async Task<T> GetWithRetryAsync<T>(string endpoint)
    {
        var result = await ExecuteWithRetryAsync(
            () => _curl.GetAsync($"{endpoint}"));
        return System.Text.Json.JsonSerializer.Deserialize<T>(result.Body);
    }

    public async Task<CurlResult> ExecuteWithRetryAsync(
        Func<Task<CurlResult>> operation)
    {
        int attempt = 0;
        var random = new Random();

        while (attempt < _options.MaxAttempts)
        {
            try
            {
                var result = await operation();

                if (!ShouldRetry(result.StatusCode, attempt))
                {
                    return result;
                }

                await WaitBeforeRetry(attempt, result, random);
            }
            catch (CurlTimeoutException) when (attempt < _options.MaxAttempts - 1)
            {
                await WaitBeforeRetry(attempt, null, random);
            }
            catch (CurlConnectionException) when (attempt < _options.MaxAttempts - 1)
            {
                await WaitBeforeRetry(attempt, null, random);
            }
            catch (CurlException) when (attempt == _options.MaxAttempts - 1)
            {
                throw;
            }

            attempt++;
        }

        throw new CurlRetryException($"Failed after {_options.MaxAttempts} attempts");
    }

    private bool ShouldRetry(int statusCode, int attempt)
    {
        if (attempt >= _options.MaxAttempts - 1)
            return false;

        return _options.RetryStatusCodes.Contains(statusCode);
    }

    private async Task WaitBeforeRetry(int attempt, CurlResult result, Random random)
    {
        var delay = CalculateDelay(attempt, result, random);

        Console.WriteLine($"Retry attempt {attempt + 1} after {delay}ms delay. " +
            $"Status: {result?.StatusCode ?? 0}");

        await Task.Delay(delay);
    }

    private int CalculateDelay(int attempt, CurlResult result, Random random)
    {
        // Check for Retry-After header (rate limiting)
        if (result?.Headers?.TryGetValue("Retry-After", out var retryAfter) == true)
        {
            if (int.TryParse(retryAfter, out var seconds))
            {
                return seconds * 1000;
            }
        }

        // Exponential backoff calculation
        var exponentialDelay = _options.InitialDelayMs *
            Math.Pow(_options.BackoffMultiplier, attempt);

        var delay = Math.Min(exponentialDelay, _options.MaxDelayMs);

        // Add jitter to prevent thundering herd
        if (_options.UseJitter)
        {
            delay = random.Next((int)(delay * 0.5), (int)(delay * 1.5));
        }

        return (int)delay;
    }
}
```

## Advanced Retry with Polly Library

```csharp
using CurlDotNet;
using Polly;
using Polly.Extensions.Http;
using Polly.CircuitBreaker;
// Install: dotnet add package CurlDotNet
// Install: dotnet add package Microsoft.Extensions.Http.Polly
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class PollyRetryClient
{
    private readonly Curl _curl;
    private readonly IAsyncPolicy<CurlResult> _retryPolicy;
    private readonly IAsyncPolicy<CurlResult> _circuitBreakerPolicy;
    private readonly IAsyncPolicy<CurlResult> _combinedPolicy;

    public PollyRetryClient(string baseUrl)
    {
        _curl = new Curl(baseUrl);

        // Retry policy with exponential backoff
        _retryPolicy = Policy
            .HandleResult<CurlResult>(r =>
                r.StatusCode >= 500 || r.StatusCode == 429)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    var result = outcome.Result;
                    Console.WriteLine($"Retry {retryCount} after {timespan}s. " +
                        $"Status: {result?.StatusCode}");

                    // Log to your logging framework
                    context["RetryCount"] = retryCount;
                });

        // Circuit breaker policy
        _circuitBreakerPolicy = Policy
            .HandleResult<CurlResult>(r => r.StatusCode >= 500)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (result, duration) =>
                {
                    Console.WriteLine($"Circuit breaker opened for {duration}");
                },
                onReset: () =>
                {
                    Console.WriteLine("Circuit breaker reset");
                },
                onHalfOpen: () =>
                {
                    Console.WriteLine("Circuit breaker half-open");
                });

        // Combine policies - circuit breaker wraps retry
        _combinedPolicy = Policy.WrapAsync(_circuitBreakerPolicy, _retryPolicy);
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        var context = new Context { ["Endpoint"] = endpoint };

        try
        {
            var result = await _combinedPolicy.ExecuteAsync(
                async (ctx) => await _curl.GetAsync(endpoint),
                context);

            return System.Text.Json.JsonSerializer.Deserialize<T>(result.Body);
        }
        catch (BrokenCircuitException)
        {
            throw new CurlException("Service is temporarily unavailable (circuit open)");
        }
    }

    public async Task<CurlResult> ExecuteAsync(
        Func<Task<CurlResult>> operation,
        string operationKey)
    {
        var context = new Context { ["OperationKey"] = operationKey };
        return await _combinedPolicy.ExecuteAsync(async (ctx) => await operation(), context);
    }
}
```

## Smart Retry with Adaptive Behavior

```csharp
using CurlDotNet;
using System.Collections.Concurrent;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class AdaptiveRetryClient
{
    private readonly Curl _curl;
    private readonly ConcurrentDictionary<string, EndpointStats> _stats;
    private readonly AdaptiveRetryOptions _options;

    public class AdaptiveRetryOptions
    {
        public int InitialRetries { get; set; } = 3;
        public int MaxRetries { get; set; } = 5;
        public double SuccessRateThreshold { get; set; } = 0.8;
        public int StatsWindowSize { get; set; } = 100;
    }

    private class EndpointStats
    {
        private readonly Queue<bool> _results = new();
        private readonly object _lock = new();

        public void RecordResult(bool success, int windowSize)
        {
            lock (_lock)
            {
                _results.Enqueue(success);
                while (_results.Count > windowSize)
                {
                    _results.Dequeue();
                }
            }
        }

        public double GetSuccessRate()
        {
            lock (_lock)
            {
                if (_results.Count == 0) return 1.0;
                return _results.Count(r => r) / (double)_results.Count;
            }
        }

        public int GetRecommendedRetries(int initial, int max, double threshold)
        {
            var successRate = GetSuccessRate();

            if (successRate >= threshold)
            {
                return initial; // Good success rate, use normal retries
            }

            // Poor success rate, increase retries proportionally
            var multiplier = 1 + (1 - successRate);
            return Math.Min((int)(initial * multiplier), max);
        }
    }

    public AdaptiveRetryClient(string baseUrl, AdaptiveRetryOptions options = null)
    {
        _curl = new Curl(baseUrl);
        _stats = new ConcurrentDictionary<string, EndpointStats>();
        _options = options ?? new AdaptiveRetryOptions();
    }

    public async Task<CurlResult> GetWithAdaptiveRetryAsync(string endpoint)
    {
        var stats = _stats.GetOrAdd(endpoint, _ => new EndpointStats());
        var maxRetries = stats.GetRecommendedRetries(
            _options.InitialRetries,
            _options.MaxRetries,
            _options.SuccessRateThreshold);

        int attempt = 0;
        CurlResult lastResult = null;
        Exception lastException = null;

        while (attempt <= maxRetries)
        {
            try
            {
                var result = await _curl.GetAsync(endpoint);

                if (result.StatusCode < 500 && result.StatusCode != 429)
                {
                    stats.RecordResult(result.StatusCode < 400, _options.StatsWindowSize);
                    return result;
                }

                lastResult = result;
            }
            catch (Exception ex)
            {
                lastException = ex;
            }

            if (attempt < maxRetries)
            {
                var delay = CalculateAdaptiveDelay(attempt, stats.GetSuccessRate());
                await Task.Delay(delay);
            }

            attempt++;
        }

        stats.RecordResult(false, _options.StatsWindowSize);

        if (lastException != null)
            throw lastException;

        return lastResult;
    }

    private int CalculateAdaptiveDelay(int attempt, double successRate)
    {
        // Base delay with exponential backoff
        var baseDelay = Math.Pow(2, attempt) * 1000;

        // Adjust based on success rate - longer delays for worse endpoints
        var adjustmentFactor = 2 - successRate; // 1.0 to 2.0
        var adjustedDelay = baseDelay * adjustmentFactor;

        // Cap at 30 seconds
        return Math.Min((int)adjustedDelay, 30000);
    }
}
```

## Retry with Idempotency

```csharp
using CurlDotNet;
using System.Security.Cryptography;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class IdempotentRetryClient
{
    private readonly Curl _curl;
    private readonly HashSet<string> _idempotentMethods = new()
    {
        "GET", "PUT", "DELETE", "HEAD", "OPTIONS"
    };

    public IdempotentRetryClient(string baseUrl)
    {
        _curl = new Curl(baseUrl);
    }

    public async Task<CurlResult> ExecuteWithIdempotencyAsync(
        string method,
        string endpoint,
        object payload = null,
        string idempotencyKey = null)
    {
        // Generate idempotency key if not provided
        if (string.IsNullOrEmpty(idempotencyKey) && !IsIdempotent(method))
        {
            idempotencyKey = GenerateIdempotencyKey(method, endpoint, payload);
        }

        int maxRetries = IsIdempotent(method) ? 3 : 1;
        int attempt = 0;

        while (attempt < maxRetries)
        {
            try
            {
                var request = CreateRequest(method, endpoint, payload);

                if (!string.IsNullOrEmpty(idempotencyKey))
                {
                    request.WithHeader("Idempotency-Key", idempotencyKey);
                }

                var result = await request.ExecuteAsync();

                if (result.StatusCode < 500)
                {
                    return result;
                }

                // Check if server indicates duplicate request
                if (result.Headers?.ContainsKey("X-Idempotent-Replayed") == true)
                {
                    Console.WriteLine("Server replayed idempotent request");
                    return result;
                }
            }
            catch (CurlException) when (attempt < maxRetries - 1)
            {
                // Retry only if idempotent
                if (!IsIdempotent(method) && string.IsNullOrEmpty(idempotencyKey))
                {
                    throw;
                }
            }

            attempt++;
            if (attempt < maxRetries)
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
            }
        }

        throw new CurlRetryException($"Failed after {maxRetries} attempts");
    }

    private bool IsIdempotent(string method)
    {
        return _idempotentMethods.Contains(method.ToUpperInvariant());
    }

    private string GenerateIdempotencyKey(string method, string endpoint, object payload)
    {
        using var sha256 = SHA256.Create();
        var data = $"{method}:{endpoint}:{System.Text.Json.JsonSerializer.Serialize(payload ?? "")}";
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data));
        return Convert.ToBase64String(hash);
    }

    private CurlRequest CreateRequest(string method, string endpoint, object payload)
    {
        return method.ToUpperInvariant() switch
        {
            "GET" => _curl.Get(endpoint),
            "POST" => _curl.Post(endpoint).WithJson(payload),
            "PUT" => _curl.Put(endpoint).WithJson(payload),
            "DELETE" => _curl.Delete(endpoint),
            _ => throw new NotSupportedException($"Method {method} not supported")
        };
    }
}
```

## Real-World Example: Stripe API with Retry

```csharp
using CurlDotNet;
using System.Text.Json;
// Install: dotnet add package CurlDotNet
// NuGet: https://www.nuget.org/packages/CurlDotNet/

public class StripeClientWithRetry
{
    private readonly Curl _curl;
    private readonly string _apiKey;
    private readonly RetryPolicy _retryPolicy;

    public StripeClientWithRetry(string apiKey)
    {
        _apiKey = apiKey;
        _curl = new Curl("https://api.stripe.com/v1")
            .WithHeader("Authorization", $"Bearer {apiKey}")
            .WithTimeout(30);

        _retryPolicy = new RetryPolicy
        {
            MaxAttempts = 3,
            RetryableStatusCodes = new[] { 429, 500, 502, 503, 504 },
            InitialDelay = TimeSpan.FromSeconds(1),
            MaxDelay = TimeSpan.FromSeconds(10)
        };
    }

    public async Task<PaymentIntent> CreatePaymentIntentAsync(
        decimal amount,
        string currency,
        string idempotencyKey = null)
    {
        var payload = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("amount", ((int)(amount * 100)).ToString()),
            new KeyValuePair<string, string>("currency", currency),
            new KeyValuePair<string, string>("automatic_payment_methods[enabled]", "true")
        });

        var result = await ExecuteWithRetryAsync(async () =>
        {
            var request = _curl.Post("/payment_intents")
                .WithHeader("Content-Type", "application/x-www-form-urlencoded");

            if (!string.IsNullOrEmpty(idempotencyKey))
            {
                request.WithHeader("Idempotency-Key", idempotencyKey);
            }

            return await request.WithBody(await payload.ReadAsStringAsync())
                .ExecuteAsync();
        });

        return JsonSerializer.Deserialize<PaymentIntent>(result.Body);
    }

    public async Task<Customer> CreateCustomerAsync(
        string email,
        string name,
        Dictionary<string, string> metadata = null)
    {
        var formData = new List<KeyValuePair<string, string>>
        {
            new("email", email),
            new("name", name)
        };

        if (metadata != null)
        {
            foreach (var (key, value) in metadata)
            {
                formData.Add(new($"metadata[{key}]", value));
            }
        }

        var idempotencyKey = $"customer_{email}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";

        var result = await ExecuteWithRetryAsync(async () =>
            await _curl.Post("/customers")
                .WithHeader("Idempotency-Key", idempotencyKey)
                .WithFormData(formData)
                .ExecuteAsync()
        );

        return JsonSerializer.Deserialize<Customer>(result.Body);
    }

    private async Task<CurlResult> ExecuteWithRetryAsync(Func<Task<CurlResult>> operation)
    {
        int attempt = 0;
        var random = new Random();

        while (attempt < _retryPolicy.MaxAttempts)
        {
            try
            {
                var result = await operation();

                // Success
                if (result.StatusCode < 400)
                {
                    return result;
                }

                // Rate limit - check Retry-After header
                if (result.StatusCode == 429)
                {
                    var retryAfter = GetRetryAfterDelay(result);
                    Console.WriteLine($"Rate limited. Waiting {retryAfter.TotalSeconds}s");
                    await Task.Delay(retryAfter);
                    attempt++;
                    continue;
                }

                // Server error - retry with backoff
                if (_retryPolicy.RetryableStatusCodes.Contains(result.StatusCode))
                {
                    if (attempt < _retryPolicy.MaxAttempts - 1)
                    {
                        var delay = CalculateBackoffDelay(attempt, random);
                        Console.WriteLine($"Server error {result.StatusCode}. Retry {attempt + 1} after {delay.TotalSeconds}s");
                        await Task.Delay(delay);
                        attempt++;
                        continue;
                    }
                }

                // Non-retryable error or last attempt
                return result;
            }
            catch (CurlTimeoutException) when (attempt < _retryPolicy.MaxAttempts - 1)
            {
                var delay = CalculateBackoffDelay(attempt, random);
                Console.WriteLine($"Timeout. Retry {attempt + 1} after {delay.TotalSeconds}s");
                await Task.Delay(delay);
                attempt++;
            }
            catch
            {
                if (attempt == _retryPolicy.MaxAttempts - 1)
                    throw;

                attempt++;
            }
        }

        throw new Exception($"Failed after {_retryPolicy.MaxAttempts} retry attempts");
    }

    private TimeSpan GetRetryAfterDelay(CurlResult result)
    {
        if (result.Headers?.TryGetValue("Retry-After", out var value) == true)
        {
            if (int.TryParse(value, out var seconds))
            {
                return TimeSpan.FromSeconds(seconds);
            }
        }
        return TimeSpan.FromSeconds(1);
    }

    private TimeSpan CalculateBackoffDelay(int attempt, Random random)
    {
        var exponentialDelay = _retryPolicy.InitialDelay.TotalMilliseconds *
            Math.Pow(2, attempt);

        // Add jitter ±25%
        var jitter = random.Next(75, 125) / 100.0;
        var delayMs = exponentialDelay * jitter;

        var delay = TimeSpan.FromMilliseconds(delayMs);
        return delay > _retryPolicy.MaxDelay ? _retryPolicy.MaxDelay : delay;
    }

    private class RetryPolicy
    {
        public int MaxAttempts { get; set; }
        public int[] RetryableStatusCodes { get; set; }
        public TimeSpan InitialDelay { get; set; }
        public TimeSpan MaxDelay { get; set; }
    }

    public class PaymentIntent
    {
        public string Id { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
    }

    public class Customer
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
```

## Testing Retry Logic

```csharp
using Xunit;
using CurlDotNet;
// Install: dotnet add package CurlDotNet
// Install: dotnet add package xunit

public class RetryLogicTests
{
    [Fact]
    public async Task Should_Retry_On_Server_Error()
    {
        // Arrange
        var attemptCount = 0;
        var client = new ExponentialBackoffRetry("https://api.test.com",
            new ExponentialBackoffRetry.RetryOptions
            {
                MaxAttempts = 3,
                InitialDelayMs = 10,
                UseJitter = false
            });

        // Act - simulate failing twice then succeeding
        var result = await client.ExecuteWithRetryAsync(async () =>
        {
            attemptCount++;
            if (attemptCount < 3)
            {
                return new CurlResult { StatusCode = 500, Body = "Server Error" };
            }
            return new CurlResult { StatusCode = 200, Body = "{\"success\":true}" };
        });

        // Assert
        Assert.Equal(3, attemptCount);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task Should_Not_Retry_Client_Errors()
    {
        // Arrange
        var attemptCount = 0;
        var client = new BasicRetryClient("https://api.test.com", 3);

        // Act
        var result = await client.ExecuteWithRetryAsync(async () =>
        {
            attemptCount++;
            return new CurlResult { StatusCode = 404, Body = "Not Found" };
        });

        // Assert
        Assert.Equal(1, attemptCount); // Should not retry 404
        Assert.Equal(404, result.StatusCode);
    }
}
```

## Best Practices

### 1. Choose the Right Strategy
- **Simple retry**: Basic transient failures
- **Exponential backoff**: Avoid overwhelming servers
- **Circuit breaker**: Protect against cascading failures
- **Adaptive retry**: Learn from endpoint behavior

### 2. Configure Appropriately
```csharp
// Good defaults
var options = new RetryOptions
{
    MaxAttempts = 3,              // Don't retry too many times
    InitialDelayMs = 1000,        // Start with 1 second
    MaxDelayMs = 30000,           // Cap at 30 seconds
    UseJitter = true,             // Prevent thundering herd
    RetryStatusCodes = new[] { 429, 500, 502, 503, 504 }
};
```

### 3. Log and Monitor
```csharp
// Always log retry attempts
_logger.LogWarning("Retry {Attempt} for {Endpoint} after {StatusCode}",
    attempt, endpoint, result.StatusCode);
```

### 4. Use Idempotency Keys
```csharp
// For non-idempotent operations
var idempotencyKey = Guid.NewGuid().ToString();
request.WithHeader("Idempotency-Key", idempotencyKey);
```

## Troubleshooting

### Retries Not Working
- Check if exception type is retryable
- Verify status codes are in retry list
- Ensure max attempts > 1

### Too Many Retries
- Implement circuit breaker
- Reduce max attempts
- Check if service is actually down

### Rate Limiting Issues
- Respect Retry-After headers
- Implement proper backoff
- Consider rate limit middleware

## Key Takeaways

- ✅ Always implement retry logic for production APIs
- ✅ Use exponential backoff to avoid overwhelming servers
- ✅ Add jitter to prevent thundering herd
- ✅ Combine retry with circuit breaker for resilience
- ✅ Use idempotency keys for non-idempotent operations
- ✅ Monitor and log retry behavior
- ✅ Test retry logic thoroughly

## Related Examples

- [API Client Class](./api-client-class)
- [Rate Limiting](./rate-limiting)
- [Circuit Breaker Pattern](../advanced/circuit-breaker)
- [Error Handling](../beginner/handle-errors)

---

*Part of the CurlDotNet Cookbook - Professional patterns for C# and .NET developers*