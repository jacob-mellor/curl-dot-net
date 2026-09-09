# Tutorial 13: Parallel Requests

Learn how to execute multiple HTTP requests concurrently to improve performance and reduce overall execution time with CurlDotNet.

## Why Parallel Requests?

Sequential requests waste time waiting:
```
Request 1: [====] 200ms
Request 2:       [====] 200ms
Request 3:             [====] 200ms
Total: 600ms
```

Parallel requests save time:
```
Request 1: [====] 200ms
Request 2: [====] 200ms
Request 3: [====] 200ms
Total: 200ms
```

## Basic Parallel Requests

### Using Task.WhenAll
```csharp
public async Task BasicParallelRequests()
{
    var curl = new Curl();
    var urls = new[]
    {
        "https://api.example.com/users/1",
        "https://api.example.com/users/2",
        "https://api.example.com/users/3"
    };

    // Start all requests simultaneously
    var tasks = urls.Select(url => curl.GetAsync(url)).ToArray();

    // Wait for all to complete
    var results = await Task.WhenAll(tasks);

    // Process results
    for (int i = 0; i < results.Length; i++)
    {
        if (results[i].IsSuccess)
        {
            Console.WriteLine($"User {i + 1}: {results[i].Data}");
        }
    }
}
```

### Parallel.ForEachAsync (.NET 6+)
```csharp
public async Task ModernParallelRequests()
{
    var curl = new Curl();
    var userIds = Enumerable.Range(1, 100).ToList();
    var results = new ConcurrentBag<User>();

    await Parallel.ForEachAsync(userIds, async (userId, ct) =>
    {
        var result = await curl.GetAsync($"https://api.example.com/users/{userId}", ct);

        if (result.IsSuccess)
        {
            var user = JsonSerializer.Deserialize<User>(result.Data);
            results.Add(user);
        }
    });

    Console.WriteLine($"Retrieved {results.Count} users");
}
```

## Controlling Concurrency

### Limited Parallelism
```csharp
public async Task LimitedConcurrency()
{
    var curl = new Curl();
    var urls = GetManyUrls(); // Assume 1000 URLs

    // Limit to 5 concurrent requests
    var semaphore = new SemaphoreSlim(5);
    var tasks = new List<Task<CurlResult>>();

    foreach (var url in urls)
    {
        await semaphore.WaitAsync();

        var task = Task.Run(async () =>
        {
            try
            {
                return await curl.GetAsync(url);
            }
            finally
            {
                semaphore.Release();
            }
        });

        tasks.Add(task);
    }

    var results = await Task.WhenAll(tasks);
    Console.WriteLine($"Completed {results.Length} requests");
}
```

### Using Channels for Producer-Consumer
```csharp
public class RequestProcessor
{
    private readonly Channel<string> _urlChannel;
    private readonly Curl _curl = new Curl();

    public RequestProcessor(int maxConcurrency = 10)
    {
        _urlChannel = Channel.CreateUnbounded<string>();

        // Start consumer tasks
        for (int i = 0; i < maxConcurrency; i++)
        {
            _ = Task.Run(ProcessRequests);
        }
    }

    public async Task AddUrl(string url)
    {
        await _urlChannel.Writer.WriteAsync(url);
    }

    private async Task ProcessRequests()
    {
        await foreach (var url in _urlChannel.Reader.ReadAllAsync())
        {
            try
            {
                var result = await _curl.GetAsync(url);
                if (result.IsSuccess)
                {
                    await ProcessResult(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {url}: {ex.Message}");
            }
        }
    }

    private async Task ProcessResult(CurlResult result)
    {
        // Process the result
        await Task.Delay(10); // Simulate processing
    }

    public void Complete()
    {
        _urlChannel.Writer.Complete();
    }
}
```

## Batch Processing

### Processing in Batches
```csharp
public async Task ProcessInBatches<T>(List<string> urls, int batchSize = 10)
{
    var curl = new Curl();
    var allResults = new List<T>();

    for (int i = 0; i < urls.Count; i += batchSize)
    {
        var batch = urls.Skip(i).Take(batchSize);
        Console.WriteLine($"Processing batch {i / batchSize + 1}...");

        var tasks = batch.Select(url => curl.GetAsync(url));
        var results = await Task.WhenAll(tasks);

        foreach (var result in results.Where(r => r.IsSuccess))
        {
            var data = JsonSerializer.Deserialize<T>(result.Data);
            allResults.Add(data);
        }

        // Optional: Add delay between batches to avoid overwhelming the server
        if (i + batchSize < urls.Count)
        {
            await Task.Delay(100);
        }
    }

    return allResults;
}
```

## Aggregating Results

### Collecting All Results
```csharp
public class ParallelAggregator
{
    private readonly Curl _curl = new Curl();

    public async Task<AggregatedData> AggregateDataFromMultipleSources()
    {
        // Define different data sources
        var weatherTask = GetWeatherDataAsync();
        var newsTask = GetNewsDataAsync();
        var stockTask = GetStockDataAsync();
        var trafficTask = GetTrafficDataAsync();

        // Wait for all tasks to complete
        await Task.WhenAll(weatherTask, newsTask, stockTask, trafficTask);

        // Aggregate results
        return new AggregatedData
        {
            Weather = await weatherTask,
            News = await newsTask,
            Stocks = await stockTask,
            Traffic = await trafficTask,
            Timestamp = DateTime.UtcNow
        };
    }

    private async Task<WeatherData> GetWeatherDataAsync()
    {
        var result = await _curl.GetAsync("https://api.weather.com/current");
        return result.IsSuccess
            ? JsonSerializer.Deserialize<WeatherData>(result.Data)
            : null;
    }

    private async Task<List<NewsItem>> GetNewsDataAsync()
    {
        var result = await _curl.GetAsync("https://api.news.com/latest");
        return result.IsSuccess
            ? JsonSerializer.Deserialize<List<NewsItem>>(result.Data)
            : new List<NewsItem>();
    }

    private async Task<StockData> GetStockDataAsync()
    {
        var result = await _curl.GetAsync("https://api.stocks.com/quotes");
        return result.IsSuccess
            ? JsonSerializer.Deserialize<StockData>(result.Data)
            : null;
    }

    private async Task<TrafficData> GetTrafficDataAsync()
    {
        var result = await _curl.GetAsync("https://api.traffic.com/conditions");
        return result.IsSuccess
            ? JsonSerializer.Deserialize<TrafficData>(result.Data)
            : null;
    }
}
```

## Error Handling in Parallel Operations

### Handling Partial Failures
```csharp
public async Task<BatchResult<T>> ProcessWithErrorHandling<T>(List<string> urls)
{
    var curl = new Curl();
    var successful = new ConcurrentBag<T>();
    var failed = new ConcurrentBag<FailedRequest>();

    var tasks = urls.Select(async url =>
    {
        try
        {
            var result = await curl.GetAsync(url);

            if (result.IsSuccess)
            {
                var data = JsonSerializer.Deserialize<T>(result.Data);
                successful.Add(data);
            }
            else
            {
                failed.Add(new FailedRequest
                {
                    Url = url,
                    Error = result.Error,
                    StatusCode = result.StatusCode
                });
            }
        }
        catch (Exception ex)
        {
            failed.Add(new FailedRequest
            {
                Url = url,
                Error = ex.Message,
                Exception = ex
            });
        }
    });

    await Task.WhenAll(tasks);

    return new BatchResult<T>
    {
        Successful = successful.ToList(),
        Failed = failed.ToList(),
        SuccessRate = (double)successful.Count / urls.Count
    };
}

public class BatchResult<T>
{
    public List<T> Successful { get; set; }
    public List<FailedRequest> Failed { get; set; }
    public double SuccessRate { get; set; }
}

public class FailedRequest
{
    public string Url { get; set; }
    public string Error { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public Exception Exception { get; set; }
}
```

## Performance Optimization

### Connection Pooling
```csharp
public class OptimizedParallelClient
{
    // Reuse Curl instances for better connection pooling
    private readonly ConcurrentBag<Curl> _curlPool = new();
    private readonly int _poolSize;

    public OptimizedParallelClient(int poolSize = 10)
    {
        _poolSize = poolSize;

        // Pre-create Curl instances
        for (int i = 0; i < poolSize; i++)
        {
            _curlPool.Add(new Curl());
        }
    }

    public async Task<CurlResult> ExecuteAsync(string url)
    {
        Curl curl = null;

        try
        {
            // Get a Curl instance from the pool
            if (!_curlPool.TryTake(out curl))
            {
                // Pool is empty, create a new instance
                curl = new Curl();
            }

            return await curl.GetAsync(url);
        }
        finally
        {
            // Return to pool if we haven't exceeded the size
            if (curl != null && _curlPool.Count < _poolSize)
            {
                _curlPool.Add(curl);
            }
        }
    }

    public async Task<List<CurlResult>> ExecuteManyAsync(List<string> urls)
    {
        var tasks = urls.Select(ExecuteAsync);
        var results = await Task.WhenAll(tasks);
        return results.ToList();
    }
}
```

## Rate Limiting

### Respecting API Rate Limits
```csharp
public class RateLimitedParallelClient
{
    private readonly SemaphoreSlim _rateLimiter;
    private readonly Timer _resetTimer;
    private readonly Curl _curl = new Curl();
    private int _requestsRemaining;

    public RateLimitedParallelClient(int requestsPerMinute)
    {
        _requestsRemaining = requestsPerMinute;
        _rateLimiter = new SemaphoreSlim(requestsPerMinute);

        // Reset the limit every minute
        _resetTimer = new Timer(_ => ResetLimit(requestsPerMinute),
            null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    private void ResetLimit(int limit)
    {
        _requestsRemaining = limit;

        // Release all waiting requests up to the limit
        var toRelease = Math.Min(limit, _rateLimiter.CurrentCount);
        if (toRelease > 0)
        {
            _rateLimiter.Release(toRelease);
        }
    }

    public async Task<CurlResult> ExecuteAsync(string url)
    {
        await _rateLimiter.WaitAsync();

        try
        {
            Interlocked.Decrement(ref _requestsRemaining);
            var result = await _curl.GetAsync(url);

            // Check rate limit headers
            if (result.Headers.TryGetValue("X-RateLimit-Remaining", out var remaining))
            {
                _requestsRemaining = int.Parse(remaining);
            }

            return result;
        }
        finally
        {
            if (_requestsRemaining > 0)
            {
                _rateLimiter.Release();
            }
        }
    }
}
```

## Progress Tracking

### Tracking Parallel Progress
```csharp
public class ParallelProgressTracker
{
    private int _completed = 0;
    private int _failed = 0;
    private readonly int _total;
    private readonly IProgress<ProgressReport> _progress;

    public ParallelProgressTracker(int total, IProgress<ProgressReport> progress)
    {
        _total = total;
        _progress = progress;
    }

    public async Task ProcessUrls(List<string> urls)
    {
        var curl = new Curl();
        var tasks = new List<Task>();

        foreach (var url in urls)
        {
            var task = ProcessSingleUrl(curl, url);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        // Final report
        _progress.Report(new ProgressReport
        {
            Completed = _completed,
            Failed = _failed,
            Total = _total,
            IsComplete = true
        });
    }

    private async Task ProcessSingleUrl(Curl curl, string url)
    {
        try
        {
            var result = await curl.GetAsync(url);

            if (result.IsSuccess)
            {
                Interlocked.Increment(ref _completed);
            }
            else
            {
                Interlocked.Increment(ref _failed);
            }

            // Report progress
            _progress.Report(new ProgressReport
            {
                Completed = _completed,
                Failed = _failed,
                Total = _total,
                PercentComplete = (_completed + _failed) * 100.0 / _total
            });
        }
        catch
        {
            Interlocked.Increment(ref _failed);
        }
    }
}

public class ProgressReport
{
    public int Completed { get; set; }
    public int Failed { get; set; }
    public int Total { get; set; }
    public double PercentComplete { get; set; }
    public bool IsComplete { get; set; }
}
```

## Testing Parallel Operations

### Unit Testing
```csharp
[TestClass]
public class ParallelRequestTests
{
    [TestMethod]
    public async Task TestParallelExecution()
    {
        // Arrange
        var curl = new Mock<ICurl>();
        var urls = Enumerable.Range(1, 10).Select(i => $"https://api.example.com/{i}").ToList();

        curl.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CurlResult { IsSuccess = true, Data = "test" });

        // Act
        var sw = Stopwatch.StartNew();
        var tasks = urls.Select(url => curl.Object.GetAsync(url));
        var results = await Task.WhenAll(tasks);
        sw.Stop();

        // Assert
        Assert.AreEqual(10, results.Length);
        Assert.IsTrue(sw.ElapsedMilliseconds < 1000); // Should be fast due to parallelism
    }
}
```

## Best Practices

1. **Limit concurrency** - Don't overwhelm servers with too many parallel requests
2. **Handle errors gracefully** - Some requests may fail in a batch
3. **Use connection pooling** - Reuse HTTP connections when possible
4. **Respect rate limits** - Implement proper rate limiting
5. **Monitor progress** - Provide feedback for long-running operations
6. **Consider server capacity** - Be a good API citizen
7. **Use cancellation tokens** - Allow operations to be cancelled
8. **Aggregate results properly** - Handle partial successes
9. **Test parallel behavior** - Ensure correctness under concurrent load
10. **Log appropriately** - Track success/failure rates

## Summary

Parallel requests can dramatically improve performance:
- Use Task.WhenAll for simple scenarios
- Control concurrency with semaphores
- Handle partial failures gracefully
- Implement rate limiting when needed
- Track progress for user feedback

## What's Next?

Learn about [debugging requests](14-debugging-requests.html) to troubleshoot issues effectively.

---

[← Previous: Cancellation Tokens](12-cancellation-tokens.html) | [Next: Debugging Requests →](14-debugging-requests.html)