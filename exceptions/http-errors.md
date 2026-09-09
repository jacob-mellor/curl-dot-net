# HTTP Error Responses

## Overview

HTTP errors occur when the server returns an error status code (4xx or 5xx). CurlDotNet provides detailed information about these errors.

## HTTP Status Code Categories

### Client Errors (4xx)
- **400 Bad Request**: Invalid request syntax
- **401 Unauthorized**: Authentication required
- **403 Forbidden**: Access denied
- **404 Not Found**: Resource doesn't exist
- **429 Too Many Requests**: Rate limited

### Server Errors (5xx)
- **500 Internal Server Error**: Server-side error
- **502 Bad Gateway**: Invalid upstream response
- **503 Service Unavailable**: Server overloaded or maintenance
- **504 Gateway Timeout**: Upstream timeout

## Handling HTTP Errors

```csharp
try
{
    var result = await curl.ExecuteAsync("curl https://api.example.com/resource");
}
catch (CurlHttpException ex)
{
    switch (ex.StatusCode)
    {
        case 401:
            // Refresh authentication
            break;
        case 429:
            // Wait and retry
            var retryAfter = ex.GetRetryAfter();
            await Task.Delay(retryAfter ?? TimeSpan.FromMinutes(1));
            break;
        case 500:
        case 502:
        case 503:
            // Server error - retry with backoff
            break;
        default:
            // Log and handle other errors
            break;
    }
}
```

## Best Practices

### Retry Logic for Server Errors

```csharp
public async Task<CurlResult> ExecuteWithRetry(string url)
{
    var maxRetries = 3;
    var delay = TimeSpan.FromSeconds(1);

    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return await curl.ExecuteAsync($"curl {url}");
        }
        catch (CurlHttpException ex) when (ex.IsServerError)
        {
            if (i == maxRetries - 1) throw;
            await Task.Delay(delay);
            delay = delay * 2; // Exponential backoff
        }
    }
}
```

## Related Documentation
- [Authentication Errors](auth-errors.html)
- [Rate Limiting Guide](/guides/rate-limiting.html)
- [HTTP Status Codes Reference](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status)