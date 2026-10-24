---
title: "Handling curl Errors and Exceptions in C#"
description: "Handle curl errors and exceptions in C#: status codes on CurlResult, CurlOperationTimeoutException, CurlCouldntConnectException and EnsureSuccess."
---

# Tutorial 6: Handling curl Errors and Exceptions in C#

Handling curl errors and exceptions in C# comes down to one rule: HTTP failures (404, 500) come back as a `CurlResult` with `IsSuccess == false`, while network failures (DNS, connection refused, timeouts) throw an exception derived from `CurlException`, carrying the same exit code curl would return. Add `-f`/`--fail` and HTTP errors throw too, exactly as curl exits with code 22. This tutorial shows each path and how to build retries and fallbacks on top of them.

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
catch (CurlCouldntResolveHostException ex)
{
    // DNS failure: curl exit code 6
    Console.WriteLine($"Could not resolve {ex.Hostname}: {ex.Message}");
}
catch (CurlCouldntConnectException ex)
{
    // host resolved but the connection was refused or unreachable: exit code 7
    Console.WriteLine($"Could not connect to {ex.Host}: {ex.Message}");
}
```

CurlDotNet tells these apart the way curl does: 5 (`CurlCouldntResolveProxyException`), 6 (`CurlCouldntResolveHostException`), 7 (`CurlCouldntConnectException`), and 35 or 60 for TLS handshake and certificate failures (`CurlSslConnectErrorException`, `CurlPeerFailedVerificationException`).

### 3. Timeout Errors (exception)

Like curl, CurlDotNet has no timeout unless you ask for one. Use `-m`/`--max-time` to cap the whole request, from connection to the last byte of the body, and `--connect-timeout` to cap connection setup (enforced on .NET 5 and later, best-effort on .NET Standard 2.0 / .NET Framework). Both accept decimals such as `-m 2.5`. When a limit expires you get `CurlOperationTimeoutException` (curl exit code 28).

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

Passing a `CancellationToken` that gets cancelled throws the standard .NET `OperationCanceledException`, just like `HttpClient`. It is not a `CurlException`. [Tutorial 12](12-cancellation-tokens.html) covers this in depth.

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
try
{
    await Curl.ExecuteAsync("curl https://httpbin.org/delay/5", cts.Token);
}
catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
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

The curl way is `-f`/`--fail`: an HTTP status of 400 or above then throws `CurlHttpReturnedErrorException` (exit code 22) instead of returning a result. `--fail-with-body` does the same but keeps the response body in the exception.

```csharp
try
{
    await Curl.ExecuteAsync("curl -f https://httpbin.org/status/503");
}
catch (CurlHttpReturnedErrorException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode} (curl exit code {ex.CurlErrorCode})");   // 503, 22
}
```

Without `-f`, call `EnsureSuccess()` on the result. It throws `CurlDotNet.Core.CurlHttpException` for any non-2xx status. Because a class with the same name also exists in `CurlDotNet.Exceptions`, qualify it when both namespaces are imported.

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

### curl's --retry

CurlDotNet implements curl's `--retry`: it retries timeouts and HTTP 408, 429, 500, 502, 503 and 504, doubling the wait from 1 second and honouring `Retry-After`. Add `--retry-connrefused` to retry refused connections, or `--retry-all-errors` to retry anything. `CurlSettings.WithRetries(count, delayMs)` is the .NET shortcut and retries any failure.

```csharp
var result = await Curl.ExecuteAsync("curl --retry 3 -m 10 https://httpbin.org/status/503");

var r2 = await Curl.ExecuteAsync("curl https://httpbin.org/get",
    new CurlSettings().WithRetries(count: 3, delayMs: 500));
```

The [retry cookbook page](../cookbook/retry.html) covers `--retry-delay`, `--retry-max-time` and the rest.

### Retry with Exponential Backoff

When you need your own policy, a small loop is all you need. Retry only on transient failures: timeouts, connection problems and 5xx or 429 responses.

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
| `CurlCouldntResolveHostException` | DNS lookup failed (exit 6, `Hostname`) |
| `CurlCouldntConnectException` | Connection refused or unreachable (exit 7, `Host`, `Port`) |
| `CurlSslConnectErrorException`, `CurlPeerFailedVerificationException` | TLS handshake or certificate failure (exit 35, 60) |
| `CurlHttpReturnedErrorException` | `-f`/`--fail` and the server returned 400 or above (exit 22) |
| `CurlTooManyRedirectsException` | `--max-redirs` exceeded (exit 47, `RedirectCount`) |
| `CurlMalformedUrlException` | URL could not be parsed (exit 3) |
| `CurlInvalidCommandException` | Command string is invalid, for example no URL |
| `CurlReadErrorException` | `-d @file`, `-T file` or `-F field=@file` points at a missing file (exit 26) |
| `CurlWriteErrorException` | `-o` could not write its file, for example a missing directory (exit 23) |
| `CurlDotNet.Core.CurlHttpException` | Only from `EnsureSuccess()` / `EnsureStatus()` |
| `OperationCanceledException` (.NET) | Your `CancellationToken` was cancelled |

The full list is in the [exceptions reference](../exceptions/index.html), and every exit code is listed in [curl exit codes](../reference/curl-exit-codes.html).

## Best Practices

1. Check `IsSuccess` or call `EnsureSuccess()`; never assume a response is 2xx
2. Always set `-m`/`--max-time` so a request cannot hang indefinitely
3. Retry only transient failures (timeouts, connection errors, 429, 5xx); `--retry N` does exactly that
4. Use exponential backoff between retries (`--retry` backs off 1 s, 2 s, 4 s... on its own)
5. Fail fast on 400, 401, 403 and 404
6. Log the status code and timings, never credentials
7. Read `ex.Suggestions`; the messages are written to help fix the problem

## Summary

Error handling in CurlDotNet follows two paths:

- HTTP errors arrive as a `CurlResult` with `IsSuccess == false` and a `StatusCode`
- Network errors throw a `CurlException` subclass such as `CurlOperationTimeoutException` or `CurlCouldntConnectException`, with curl's exit code in `CurlErrorCode`
- `-f` makes HTTP errors throw `CurlHttpReturnedErrorException`; `EnsureSuccess()` converts them into `CurlDotNet.Core.CurlHttpException` after the fact
- Cancellation throws the standard `OperationCanceledException`
- `--retry N` (or `CurlSettings.WithRetries`) handles transient failures; a small loop gives you a custom policy

## What's Next?

In the next tutorial, we learn about [working with JSON data](07-json-for-beginners.html), the most common format for modern APIs.

See also: the [error handling recipe](../cookbook/beginner/handle-errors.html) and [timeout errors](../exceptions/timeout-errors.html).

---

[Previous: Understanding Results](05-understanding-results.html) | [Next: JSON for Beginners](07-json-for-beginners.html)
