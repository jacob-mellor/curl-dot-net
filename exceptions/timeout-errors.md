# Timeout Errors

## Overview

Timeout errors occur when operations exceed configured time limits. CurlDotNet supports connection timeouts and total operation timeouts.

## Timeout Types

### Connection Timeout
Time to establish initial connection to the server.

### Operation Timeout
Total time for the entire request/response cycle.

## Common Scenarios & Solutions

### 1. Slow Server Response

```csharp
try
{
    // Default timeout might be too short
    var result = await curl.ExecuteAsync("curl --max-time 5 https://slow-api.example.com");
}
catch (CurlTimeoutException ex)
{
    // Retry with longer timeout
    var result = await curl.ExecuteAsync("curl --max-time 30 https://slow-api.example.com");
}
```

### 2. Large File Downloads

```csharp
// No timeout for large files
var result = await curl.ExecuteAsync("curl --max-time 0 https://example.com/largefile.zip");

// Or set appropriate timeout based on file size
var timeoutSeconds = estimatedFileSizeMB * 10; // 10 seconds per MB
var result = await curl.ExecuteAsync($"curl --max-time {timeoutSeconds} {url}");
```

### 3. Connection vs Operation Timeout

```csharp
// Set different timeouts for connection and total operation
var result = await curl.ExecuteAsync(
    "curl --connect-timeout 10 --max-time 300 https://example.com"
);
```

## Best Practices

```csharp
public class TimeoutManager
{
    public async Task<CurlResult> ExecuteWithRetry(string url, int initialTimeout = 10)
    {
        var timeouts = new[] { initialTimeout, initialTimeout * 2, initialTimeout * 4 };

        foreach (var timeout in timeouts)
        {
            try
            {
                return await curl.ExecuteAsync($"curl --max-time {timeout} {url}");
            }
            catch (CurlTimeoutException)
            {
                if (timeout == timeouts.Last()) throw;
                await Task.Delay(1000); // Wait before retry
            }
        }
    }
}
```

## Related Documentation
- [Connection Errors](connection-errors.md)
- [Network Troubleshooting](/guides/network-troubleshooting.md)