---
title: "CurlTimeoutException: curl Timeout Errors in C# (--max-time)"
description: "Handle CurlTimeoutException in C#: set -m/--max-time, WithTimeout or CurlSettings.WithTimeout in CurlDotNet and catch CurlOperationTimeoutException."
---

# CurlTimeoutException: curl Timeout Errors in C# (--max-time and --connect-timeout)

`CurlTimeoutException` is what you catch for curl timeout errors in C#: when a request runs longer than `-m`/`--max-time` allows, CurlDotNet throws `CurlOperationTimeoutException` (a subclass of `CurlTimeoutException`, curl error code 28) with the `Timeout` that was exceeded. This page shows how to set timeouts with curl flags, the builder, `CurlSettings` and process-wide defaults, and how to handle the exception.

## What Is Thrown

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    // httpbin waits 5 seconds; we allow 2
    var result = await Curl.ExecuteAsync("curl -m 2 https://httpbin.org/delay/5");
}
catch (CurlOperationTimeoutException ex)
{
    Console.WriteLine(ex.Message);        // "Operation timed out after 2 seconds ..."
    Console.WriteLine(ex.Timeout);        // 00:00:02
    Console.WriteLine(ex.CurlErrorCode);  // 28
}
```

Catching the base type `CurlTimeoutException` works too. A cancelled `CancellationToken` is different: that throws the standard `OperationCanceledException`, not a timeout (see [Timeouts vs Cancellation](#timeouts-vs-cancellation)).

## Timeout Exception Types

| Type | `CurlErrorCode` | Thrown when |
|------|----:|-------------|
| `CurlTimeoutException` | 28 | Base class of the timeout types. Thrown directly only on a rare fallback path, so `catch (CurlTimeoutException)` covers every timeout |
| `CurlOperationTimeoutException` | 28 | `-m`/`--max-time` expired (message: "Operation timed out after N milliseconds with M bytes received"); `--connect-timeout` expired ("Connection timed out after N milliseconds while connecting to host port P"); `--retry-max-time` ran out before any response arrived |
| `CurlFtpAcceptTimeoutException` | 28 | Reserved: nothing throws it today (curl's own code for this case is 12) |
| `CurlAbortedByCallbackException` | 42 | Internal only: a cancelled token is converted to `OperationCanceledException` before it reaches your code |

`Timeout` holds the limit that expired as a `TimeSpan?`.

## Setting a Timeout

### -m / --max-time

Total time allowed for the whole transfer, in seconds - connecting, sending, waiting and reading the whole body. Decimals work (`--max-time 2.5`):

```csharp
var result = await Curl.ExecuteAsync("curl --max-time 30 https://httpbin.org/delay/3");
```

### Builder: WithTimeout(TimeSpan)

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/delay/3")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .ExecuteAsync();
```

### CurlSettings.WithTimeout(int seconds)

```csharp
var settings = new CurlSettings().WithTimeout(30);
var result = await Curl.ExecuteAsync("curl https://httpbin.org/delay/3", settings);
```

### Process-Wide Default

There is no implicit timeout. Like the curl tool, a command without `-m` waits as long as the server keeps the connection open (`Curl.DefaultMaxTimeSeconds` is 0). Set `Curl.DefaultMaxTimeSeconds` to give every command that does not specify `-m` itself a limit:

```csharp
Curl.DefaultMaxTimeSeconds = 30;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/delay/3");            // 30-second limit
var slow = await Curl.ExecuteAsync("curl -m 120 https://httpbin.org/delay/3");       // per-command override
```

### --connect-timeout

`--connect-timeout <seconds>` (builder `WithConnectTimeout(TimeSpan)`, settings `WithConnectTimeout(int)`, default `Curl.DefaultConnectTimeoutSeconds`) limits only the connection phase, and accepts decimals. On .NET 5 and later it is enforced on the socket connect; on the netstandard2.0 / .NET Framework builds it is best-effort, so pair it with `-m` there. When it expires, `CurlOperationTimeoutException` says "Connection timed out":

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl --connect-timeout 2 --max-time 60 https://httpbin.org/get");
}
catch (CurlOperationTimeoutException ex) when (ex.Message.StartsWith("Connection timed out"))
{
    Console.WriteLine("Host did not accept the connection in time");
}
```

## Retrying After a Timeout

The curl way is `--retry N`: timeouts (and HTTP 408, 429, 500, 502, 503, 504) are retried with a back-off that starts at 1 second and doubles, `--retry-delay` sets a fixed pause, and `--retry-max-time` caps the total time. Each attempt keeps its own `-m` limit:

```csharp
var result = await Curl.ExecuteAsync("curl --retry 3 --retry-max-time 60 -m 10 https://httpbin.org/delay/3");
```

`new CurlSettings().WithRetries(3, 1000)` does the same from code and retries any failure. See [curl --retry in C#](../cookbook/retry.html).

To retry with a longer limit each time instead, loop yourself and cap the attempts so a dead server does not hang your application:

```csharp
async Task<CurlResult> RetryWithLongerTimeout(string url)
{
    foreach (var seconds in new[] { 5, 15, 45 })
    {
        try { return await Curl.ExecuteAsync($"curl -m {seconds} {url}"); }
        catch (CurlOperationTimeoutException) when (seconds < 45) { /* try again with more time */ }
    }
    throw new InvalidOperationException("unreachable");
}
```

(`CurlResult` is in `CurlDotNet.Core`.)

## Timeouts vs Cancellation

Pass a `CancellationToken` when the caller, not the clock, decides to stop:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

try
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/delay/30", cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Cancelled by the caller's token");
}
```

Cancellation surfaces as the standard `OperationCanceledException` (or its subclass `TaskCanceledException`) from `Curl.ExecuteAsync`, `Curl.Execute` and `LibCurl`, so it composes with the rest of your async code. It is not a `CurlException`, so a `catch (CurlException)` block does not hide it.

Use `-m` for "the server is too slow" and a token for "the user navigated away" or "the host is shutting down". See [Tutorial 12: Cancellation Tokens](../tutorials/12-cancellation-tokens.html).

## Measuring How Long Requests Take

`result.Timings` reports the phases in milliseconds, which helps you choose a sensible limit:

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
Console.WriteLine($"Connect: {result.Timings.Connect} ms, total: {result.Timings.Total} ms");
```

## Choosing Values

- Interactive requests: `-m 10` to `-m 30`
- Background jobs: `-m 60` or more, with retries
- Large downloads: a generous `-m` (for example `-m 600`), plus a `CancellationToken` you control
- Health checks: `-m 5`, fail fast

## Related Documentation

- [DNS and connection errors](dns-errors.html)
- [Cookbook](../cookbook/index.html)
- [Tutorial 12: Cancellation Tokens](../tutorials/12-cancellation-tokens.html)
- [Exception reference](index.html)
