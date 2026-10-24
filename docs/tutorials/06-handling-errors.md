---
title: "Handling curl Errors and Exceptions in C#"
description: "Handle curl errors and exceptions in C#: status codes on CurlResult, CurlOperationTimeoutException, CurlCouldntConnectException and EnsureSuccess."
---

# Tutorial 6: Handling curl Errors and Exceptions in C#

Handling curl errors and exceptions in C# comes down to one rule: HTTP failures (404, 500) come back as a `CurlResult` with `IsSuccess == false`, while network failures (DNS, connection refused, timeouts) throw an exception derived from `CurlException`. This tutorial shows both paths and how to build retries and fallbacks on top of them.

## Types of Errors

### 1. HTTP Errors (no exception)

The server answered, but with an error status. Nothing is thrown; inspect `StatusCode`.

```csharp
using CurlDotNet;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/404");

if (result.StatusCode == 404)
    Console.WriteLine("Resource not found (404)");
else if (result.StatusCode >= 500)
    Console.WriteLine($"Server error ({result.StatusCode})");
```

### 2. Network Errors (exception)

The request never got a response. These throw, so wrap the call in `try/catch`.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://no-such-host.invalid/");
}
catch (CurlCouldntConnectException ex)
{
    // DNS failure, connection refused or TLS handshake failure
    Console.WriteLine($"Could not connect to {ex.Host}: {ex.Message}");
}
```

### 3. Timeout Errors (exception)

Use `-m`/`--max-time` to cap the whole request, connection included (`--connect-timeout` is parsed but not enforced for HTTP yet). When the limit expires you get `CurlOperationTimeoutException`.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl -m 2 https://httpbin.org/delay/5");
}
catch (CurlOperationTimeoutException ex)
{
    Console.WriteLine($"Request timed out after {ex.Timeout}");
}
```

### 4. Cancellation

Passing a `CancellationToken` that gets cancelled throws `CurlAbortedByCallbackException`, not `OperationCanceledException`. [Tutorial 12](12-cancellation-tokens.html) covers this in depth.

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
try
{
    await Curl.ExecuteAsync("curl https://httpbin.org/delay/5", cts.Token);
}
catch (CurlAbortedByCallbackException)
{
    Console.WriteLine("Request was cancelled");
}
```

## Error Handling Patterns

### Basic Error Handling

Catch the base type `CurlException` to handle every network-level failure in one place. Each exception carries the `Command` that failed and a list of `Suggestions`.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/get");
    Console.WriteLine(result.IsSuccess ? result.Body : $"HTTP {result.StatusCode}");
}
catch (CurlException ex)
{
    Console.WriteLine($"curl failed: {ex.Message}");
    foreach (var tip in ex.Suggestions)
        Console.WriteLine($"  - {tip}");
}
```

### Turning HTTP Errors into Exceptions

When you would rather have a single `try/catch`, call `EnsureSuccess()`. It throws `CurlDotNet.Core.CurlHttpException` for any non-2xx status. Because a class with the same name also exists in `CurlDotNet.Exceptions`, qualify it when both namespaces are imported.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/503");
    result.EnsureSuccess();
}
catch (CurlDotNet.Core.CurlHttpException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}");
    Console.WriteLine(ex.ResponseBody);
}
```

### Detailed Error Handling

Order your `catch` blocks from most specific to least specific.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl -m 5 https://api.github.com/users/octocat");
    return result.EnsureSuccess().Body;
}
catch (CurlOperationTimeoutException) { return null; }           // slow, try later
catch (CurlCouldntConnectException)   { return null; }           // offline
catch (CurlDotNet.Core.CurlHttpException ex) when (ex.StatusCode == 404)
{
    return null;                                                   // expected miss
}
```

## Retry Logic

### Retry with Exponential Backoff

A small loop is all you need. Retry only on transient failures: timeouts, connection problems and 5xx or 429 responses.

```csharp
static async Task<CurlResult> ExecuteWithRetries(string command, int maxAttempts = 3)
{
    for (int attempt = 1; ; attempt++)
    {
        try
        {
            var result = await Curl.ExecuteAsync(command);
            if (result.IsSuccess || attempt == maxAttempts) return result;
            if (result.StatusCode != 429 && result.StatusCode < 500) return result;
        }
        catch (CurlException) when (attempt < maxAttempts) { }

        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt - 1)));
    }
}
```

Permanent failures such as 400, 401, 403 and 404 are returned immediately; retrying them only wastes time.

## Fallback Strategies

### Fallback to Cached Data

```csharp
var cache = new Dictionary<string, string>();
string url = "https://api.github.com/users/octocat";

try
{
    var result = (await Curl.ExecuteAsync($"curl -m 5 {url}")).EnsureSuccess();
    cache[url] = result.Body;
}
catch (Exception ex) when (ex is CurlException || ex is CurlDotNet.Core.CurlHttpException)
{
    if (cache.TryGetValue(url, out var cached))
        Console.WriteLine($"Using cached copy: {ex.Message}");
}
```

### Fallback to an Alternative Endpoint

```csharp
static async Task<string> GetWithFallback(string primary, string backup)
{
    try
    {
        return (await Curl.ExecuteAsync($"curl -m 5 {primary}")).EnsureSuccess().Body;
    }
    catch (Exception ex) when (ex is CurlException || ex is CurlDotNet.Core.CurlHttpException)
    {
        Console.WriteLine($"Primary failed ({ex.Message}), trying backup");
        return (await Curl.ExecuteAsync($"curl -m 5 {backup}")).EnsureSuccess().Body;
    }
}
```

## Logging and Monitoring

`CurlResult.Timings` and the exception's `Command` property give you everything needed for structured logs without a stopwatch.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/get");
    logger.LogInformation("curl {Status} in {Ms} ms, {Bytes} bytes",
        result.StatusCode, result.Timings.Total, result.Body?.Length ?? 0);
}
catch (CurlException ex)
{
    logger.LogError(ex, "curl failed: {Command} (curl code {Code})",
        ex.Command, ex.CurlErrorCode);
}
```

Never log the full command when it contains credentials (`-u user:pass` or an `Authorization` header).

## Exception Reference

The exceptions you will meet most often, all in `CurlDotNet.Exceptions` unless noted:

| Exception | When it is thrown |
|-----------|-------------------|
| `CurlOperationTimeoutException` | `-m`/`--max-time` expired (`Timeout` property) |
| `CurlCouldntConnectException` | DNS failure, connection refused, TLS handshake failure (`Host`, `Port`) |
| `CurlAbortedByCallbackException` | Your `CancellationToken` was cancelled |
| `CurlTooManyRedirectsException` | `--max-redirs` exceeded (`RedirectCount`) |
| `CurlMalformedUrlException` | URL could not be parsed |
| `CurlInvalidCommandException` | Command string is invalid, for example no URL |
| `CurlFileCouldntReadException` | `-d @file` or `-F field=@file` points at a missing file |
| `CurlDotNet.Core.CurlHttpException` | Only from `EnsureSuccess()` / `EnsureStatus()` |

The full list is in the [exceptions reference](../exceptions/index.html).

## Best Practices

1. Check `IsSuccess` or call `EnsureSuccess()`; never assume a response is 2xx
2. Always set `-m`/`--max-time` so a request cannot hang indefinitely
3. Retry only transient failures (timeouts, connection errors, 429, 5xx)
4. Use exponential backoff between retries
5. Fail fast on 400, 401, 403 and 404
6. Log the status code and timings, never credentials
7. Read `ex.Suggestions`; the messages are written to help fix the problem

## Summary

Error handling in CurlDotNet follows two paths:

- HTTP errors arrive as a `CurlResult` with `IsSuccess == false` and a `StatusCode`
- Network errors throw a `CurlException` subclass such as `CurlOperationTimeoutException` or `CurlCouldntConnectException`
- `EnsureSuccess()` converts HTTP errors into `CurlDotNet.Core.CurlHttpException` when you prefer a single `try/catch`
- A small retry loop with exponential backoff handles transient failures

## What's Next?

In the next tutorial, we learn about [working with JSON data](07-json-for-beginners.html), the most common format for modern APIs.

See also: the [error handling recipe](../cookbook/beginner/handle-errors.html) and [timeout errors](../exceptions/timeout-errors.html).

---

[Previous: Understanding Results](05-understanding-results.html) | [Next: JSON for Beginners](07-json-for-beginners.html)
