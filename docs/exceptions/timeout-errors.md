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

Catching the base type `CurlTimeoutException` works too. A cancelled `CancellationToken` is different: that throws `CurlAbortedByCallbackException`, not a timeout.

## Setting a Timeout

### -m / --max-time

Total time allowed for the whole request, in seconds:

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

When neither `-m` nor a default is set, HTTP requests time out after 30 seconds. `Curl.DefaultMaxTimeSeconds` changes that for every command that does not specify `-m` itself:

```csharp
Curl.DefaultMaxTimeSeconds = 30;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/delay/3");            // 30 s limit
var slow = await Curl.ExecuteAsync("curl -m 120 https://httpbin.org/delay/3");       // per-command override
```

### --connect-timeout

`--connect-timeout <seconds>` (builder `WithConnectTimeout(TimeSpan)`, settings `WithConnectTimeout(int)`, default `Curl.DefaultConnectTimeoutSeconds`) is accepted and is honoured by FTP transfers. For HTTP(S) requests it is not enforced yet; `--max-time` is the limit that applies, so always set `-m` when you need a guaranteed upper bound:

```csharp
var result = await Curl.ExecuteAsync("curl --max-time 60 https://httpbin.org/get");
```

## Retrying After a Timeout

Retry with a longer limit, but cap the attempts so a dead server does not hang your application:

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
catch (CurlAbortedByCallbackException)
{
    Console.WriteLine("Cancelled by the caller's token");
}
```

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
