# Tutorial 12: Cancellation Tokens

Learn how to cancel long-running requests, implement timeouts, and manage request lifecycle with CancellationTokens in CurlDotNet.

## What Are Cancellation Tokens?

CancellationTokens provide a cooperative way to cancel asynchronous operations in .NET:

```csharp
// Create a cancellation token source
var cts = new CancellationTokenSource();

// Pass the token to an async operation
var result = await curl.GetAsync(url, cts.Token);

// Cancel the operation from anywhere
cts.Cancel();
```

## Basic Cancellation

### Simple Request Cancellation
```csharp
public async Task BasicCancellation()
{
    var curl = new Curl();
    var cts = new CancellationTokenSource();

    // Cancel after 5 seconds
    cts.CancelAfter(TimeSpan.FromSeconds(5));

    try
    {
        var result = await curl.GetAsync(
            "https://slow-api.example.com/data",
            cts.Token
        );

        Console.WriteLine("Request completed!");
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Request was cancelled after 5 seconds");
    }
}
```

### User-Initiated Cancellation
```csharp
public async Task UserCancellation()
{
    var curl = new Curl();
    var cts = new CancellationTokenSource();

    // Start request in background
    var task = Task.Run(async () =>
    {
        try
        {
            var result = await curl.GetAsync(
                "https://api.example.com/large-dataset",
                cts.Token
            );
            return result;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Download cancelled by user");
            return null;
        }
    });

    // Allow user to cancel
    Console.WriteLine("Press 'c' to cancel download...");
    if (Console.ReadKey().Key == ConsoleKey.C)
    {
        cts.Cancel();
        Console.WriteLine("\nCancelling...");
    }

    await task;
}
```

## Timeout Patterns

### Request-Specific Timeout
```csharp
public async Task<CurlResult> GetWithTimeout(string url, int timeoutSeconds)
{
    var curl = new Curl();
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

    try
    {
        return await curl.GetAsync(url, cts.Token);
    }
    catch (OperationCanceledException)
    {
        return new CurlResult
        {
            IsSuccess = false,
            Error = $"Request timed out after {timeoutSeconds} seconds"
        };
    }
}
```

### Different Timeouts for Different Operations
```csharp
public class TimeoutManager
{
    private readonly Curl _curl = new Curl();

    public async Task<CurlResult> QuickGet(string url)
    {
        // Short timeout for quick operations
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        return await _curl.GetAsync(url, cts.Token);
    }

    public async Task<CurlResult> LongRunningPost(string url, object data)
    {
        // Longer timeout for complex operations
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        return await _curl.PostJsonAsync(url, data, cts.Token);
    }

    public async Task<byte[]> DownloadFile(string url)
    {
        // Very long timeout for file downloads
        using var cts = new CancellationTokenSource(TimeSpan.FromHours(1));
        return await _curl.GetBytesAsync(url, cts.Token);
    }
}
```

## Combining Multiple Cancellation Sources

### Linked Cancellation Tokens
```csharp
public async Task LinkedCancellation()
{
    // Global timeout for all operations
    var globalCts = new CancellationTokenSource(TimeSpan.FromMinutes(10));

    // User cancellation source
    var userCts = new CancellationTokenSource();

    // Combine both sources
    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
        globalCts.Token,
        userCts.Token
    );

    var curl = new Curl();

    try
    {
        // Cancelled if either source triggers
        var result = await curl.GetAsync(
            "https://api.example.com/data",
            linkedCts.Token
        );
    }
    catch (OperationCanceledException)
    {
        if (globalCts.IsCancellationRequested)
            Console.WriteLine("Global timeout reached");
        else if (userCts.IsCancellationRequested)
            Console.WriteLine("Cancelled by user");
    }
}
```

## Progress with Cancellation

### Cancellable Download with Progress
```csharp
public async Task DownloadWithProgressAndCancellation(
    string url,
    string outputPath,
    CancellationToken cancellationToken)
{
    var curl = new Curl();
    var progress = new Progress<DownloadProgress>(p =>
    {
        Console.Write($"\rDownloading: {p.PercentComplete:F1}% " +
                     $"({p.BytesDownloaded}/{p.TotalBytes} bytes)");

        // Check if user wants to cancel
        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
        {
            Console.WriteLine("\nCancellation requested...");
            // Trigger cancellation through external mechanism
        }
    });

    try
    {
        await curl.DownloadFileAsync(url, outputPath, progress, cancellationToken);
        Console.WriteLine("\nDownload completed!");
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("\nDownload cancelled!");

        // Clean up partial file
        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }
    }
}

public class DownloadProgress
{
    public long BytesDownloaded { get; set; }
    public long TotalBytes { get; set; }
    public double PercentComplete => (double)BytesDownloaded / TotalBytes * 100;
}
```

## Batch Operations with Cancellation

### Cancel Multiple Requests
```csharp
public async Task BatchRequestsWithCancellation(List<string> urls)
{
    var curl = new Curl();
    var cts = new CancellationTokenSource();
    var results = new ConcurrentBag<CurlResult>();

    // Set overall timeout
    cts.CancelAfter(TimeSpan.FromMinutes(5));

    try
    {
        await Parallel.ForEachAsync(urls, cts.Token, async (url, ct) =>
        {
            try
            {
                var result = await curl.GetAsync(url, ct);
                results.Add(result);
                Console.WriteLine($"✓ Completed: {url}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"✗ Cancelled: {url}");
            }
        });
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine($"Batch operation cancelled. Completed {results.Count}/{urls.Count} requests");
    }
}
```

## Graceful Cancellation

### Clean Shutdown Pattern
```csharp
public class GracefulHttpClient : IDisposable
{
    private readonly Curl _curl = new Curl();
    private readonly CancellationTokenSource _shutdownCts = new();
    private readonly List<Task> _activeTasks = new();

    public async Task<CurlResult> GetAsync(string url)
    {
        var tcs = new TaskCompletionSource<CurlResult>();

        var task = Task.Run(async () =>
        {
            try
            {
                var result = await _curl.GetAsync(url, _shutdownCts.Token);
                tcs.SetResult(result);
            }
            catch (OperationCanceledException)
            {
                tcs.SetCanceled();
            }
        });

        lock (_activeTasks)
        {
            _activeTasks.Add(task);
        }

        return await tcs.Task;
    }

    public async Task ShutdownAsync()
    {
        Console.WriteLine("Initiating graceful shutdown...");

        // Signal all operations to cancel
        _shutdownCts.Cancel();

        // Wait for all active tasks to complete
        Task[] tasks;
        lock (_activeTasks)
        {
            tasks = _activeTasks.ToArray();
        }

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown
        }

        Console.WriteLine("Shutdown complete");
    }

    public void Dispose()
    {
        _shutdownCts?.Dispose();
    }
}
```

## Retry with Cancellation

### Cancellable Retry Logic
```csharp
public async Task<CurlResult> RetryWithCancellation(
    string url,
    int maxRetries = 3,
    CancellationToken cancellationToken = default)
{
    var curl = new Curl();

    for (int attempt = 0; attempt < maxRetries; attempt++)
    {
        // Check for cancellation before each attempt
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            Console.WriteLine($"Attempt {attempt + 1} of {maxRetries}...");

            // Create timeout for this attempt
            using var attemptCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken,
                attemptCts.Token
            );

            var result = await curl.GetAsync(url, linkedCts.Token);

            if (result.IsSuccess)
                return result;

            // Check if we should retry
            if (attempt < maxRetries - 1)
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                Console.WriteLine($"Retrying in {delay.TotalSeconds} seconds...");

                await Task.Delay(delay, cancellationToken);
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            // Individual attempt timed out, but overall operation not cancelled
            Console.WriteLine($"Attempt {attempt + 1} timed out");
        }
    }

    throw new Exception($"Failed after {maxRetries} attempts");
}
```

## Cancellation in Background Services

### Background Download Service
```csharp
public class BackgroundDownloadService : BackgroundService
{
    private readonly ILogger<BackgroundDownloadService> _logger;
    private readonly Queue<DownloadTask> _downloadQueue = new();
    private readonly Curl _curl = new Curl();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_downloadQueue.TryDequeue(out var downloadTask))
            {
                await ProcessDownload(downloadTask, stoppingToken);
            }
            else
            {
                // Wait for new tasks
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    private async Task ProcessDownload(DownloadTask task, CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation($"Starting download: {task.Url}");

            // Combine service stopping token with task-specific timeout
            using var taskCts = new CancellationTokenSource(task.Timeout);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                stoppingToken,
                taskCts.Token
            );

            var result = await _curl.GetBytesAsync(task.Url, linkedCts.Token);

            await File.WriteAllBytesAsync(task.OutputPath, result, stoppingToken);

            _logger.LogInformation($"Download completed: {task.OutputPath}");
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Service is shutting down");
            throw;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning($"Download timed out: {task.Url}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Download failed: {task.Url}");
        }
    }

    public void QueueDownload(string url, string outputPath, TimeSpan timeout)
    {
        _downloadQueue.Enqueue(new DownloadTask
        {
            Url = url,
            OutputPath = outputPath,
            Timeout = timeout
        });
    }

    private class DownloadTask
    {
        public string Url { get; set; }
        public string OutputPath { get; set; }
        public TimeSpan Timeout { get; set; }
    }
}
```

## Testing with Cancellation

### Unit Testing Cancellation
```csharp
[TestClass]
public class CancellationTests
{
    [TestMethod]
    public async Task TestRequestCancellation()
    {
        // Arrange
        var curl = new Mock<ICurl>();
        var cts = new CancellationTokenSource();

        curl.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(async (string url, CancellationToken ct) =>
            {
                await Task.Delay(1000, ct);
                return new CurlResult { IsSuccess = true };
            });

        // Act
        cts.CancelAfter(500);

        // Assert
        await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () =>
        {
            await curl.Object.GetAsync("https://example.com", cts.Token);
        });
    }

    [TestMethod]
    public async Task TestGracefulCancellation()
    {
        // Arrange
        var service = new GracefulHttpClient();

        // Act
        var task1 = service.GetAsync("https://example1.com");
        var task2 = service.GetAsync("https://example2.com");

        await service.ShutdownAsync();

        // Assert
        Assert.IsTrue(task1.IsCanceled || task1.IsCompleted);
        Assert.IsTrue(task2.IsCanceled || task2.IsCompleted);
    }
}
```

## Best Practices

1. **Always dispose CancellationTokenSource** - Use `using` statements
2. **Check for cancellation regularly** - In long-running loops
3. **Clean up on cancellation** - Delete partial files, close connections
4. **Provide meaningful timeouts** - Different operations need different limits
5. **Handle OperationCanceledException** - Distinguish from other errors
6. **Use linked tokens** - Combine multiple cancellation conditions
7. **Test cancellation paths** - Ensure graceful handling
8. **Log cancellations** - Track why operations were cancelled
9. **Respect cancellation immediately** - Don't delay after cancellation
10. **Document cancellation behavior** - Make it clear how operations can be cancelled

## Summary

Cancellation tokens provide powerful control over async operations:
- Cancel long-running requests
- Implement flexible timeout strategies
- Coordinate multiple operations
- Build responsive applications
- Handle shutdowns gracefully

## What's Next?

Learn about [parallel requests](13-parallel-requests.md) to improve performance with concurrent operations.

---

[← Previous: Forms and Data](11-forms-and-data.md) | [Next: Parallel Requests →](13-parallel-requests.md)