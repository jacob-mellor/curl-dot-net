---
title: "curl Requests with CancellationToken in C# (CurlDotNet)"
description: "Cancel curl requests with CancellationToken in C#: Curl.ExecuteAsync(cmd, token), CancelAfter, linked tokens, and how cancellation differs from -m timeouts"
---

# Tutorial 12: curl requests with CancellationToken in C#

This tutorial shows how to run curl requests with CancellationToken in C# using CurlDotNet: pass a token to `Curl.ExecuteAsync`, cancel from a timer or a user action, combine several cancellation sources, and tell a cancelled request apart from a `-m` timeout.

## What Are Cancellation Tokens?

A `CancellationToken` is .NET's cooperative way to say "stop what you are doing". You create a `CancellationTokenSource`, hand its `Token` to the async call, and call `Cancel()` (or let `CancelAfter` do it) when you want out. CurlDotNet checks the token while the request runs and aborts the transfer.

One fact to learn up front: when a CurlDotNet request is cancelled, it throws the standard .NET `OperationCanceledException` (the same exception `HttpClient` and `Task.Delay` use; `TaskCanceledException` derives from it). `Curl.ExecuteAsync`, `CurlEngine`, the builder and `LibCurl` all follow that contract, so the usual `catch (OperationCanceledException) when (token.IsCancellationRequested)` pattern works unchanged. Cancellation is not a `CurlException`.

## Basic Cancellation

### Cancel after a delay

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

using var cts = new CancellationTokenSource();
cts.CancelAfter(TimeSpan.FromSeconds(2));

try
{
    // httpbin waits 10 seconds before answering; we give up after 2
    await Curl.ExecuteAsync("curl https://httpbin.org/delay/10", cts.Token);
}
catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
{
    Console.WriteLine("Request cancelled after 2 seconds");
}
```

### User-initiated cancellation

Start the request, keep the task, and cancel when the user asks:

```csharp
using var cts = new CancellationTokenSource();
var download = Curl.ExecuteAsync("curl -o big.bin https://httpbin.org/delay/10", cts.Token);

Console.WriteLine("Press any key to cancel...");
Console.ReadKey(true);
cts.Cancel();

try { await download; }
catch (OperationCanceledException) when (cts.Token.IsCancellationRequested) { Console.WriteLine("Cancelled by user"); }
```

## Cancellation Versus Timeouts

curl has its own deadline flags: `-m` (`--max-time`) bounds the whole transfer, body included, and `--connect-timeout` bounds connection setup (enforced on .NET 5 and later, best-effort on .NET Standard 2.0 / .NET Framework). Both accept decimals (`-m 2.5`). Like curl, there is no timeout at all unless you set one (or set `Curl.DefaultMaxTimeSeconds`). An expired `-m` produces a different exception from a cancelled token, so you can tell the two situations apart:

```csharp
using var cts = new CancellationTokenSource();
try
{
    // -m 3: curl's own deadline for the whole transfer
    await Curl.ExecuteAsync("curl -m 3 https://httpbin.org/delay/10", cts.Token);
}
catch (CurlOperationTimeoutException ex)
{
    Console.WriteLine($"Timed out after {ex.Timeout} (curl exit code {ex.CurlErrorCode})");   // 28
}
catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
{
    Console.WriteLine("Cancelled by a CancellationToken");
}
```

| Situation | Exception |
|-----------|-----------|
| `-m` / `--max-time` expired | `CurlOperationTimeoutException` (derives from `CurlTimeoutException` and `CurlException`, exit code 28, has `Timeout`) |
| Your `CancellationToken` was cancelled | `OperationCanceledException` (standard .NET, not a `CurlException`) |

Use `-m` for "this request should never take longer than N seconds" and a token for "stop because the caller no longer cares". A `catch (CurlException)` block does not catch cancellation, which is usually what you want: cancellation is not a failure to report.

## Passing Tokens Through Every API Style

The command-string API, the builder, `CurlSettings` and the `LibCurl` instance class all accept a token:

```csharp
using CurlDotNet.Core;
using CurlDotNet.Lib;

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

// Builder
var r1 = await CurlRequestBuilder.Get("https://httpbin.org/get").ExecuteAsync(cts.Token);

// CurlSettings
var r2 = await Curl.ExecuteAsync("curl https://httpbin.org/get",
    new CurlSettings().WithCancellation(cts.Token));

// LibCurl instance
using var curl = new LibCurl();
var r3 = await curl.GetAsync("https://httpbin.org/get", null, cts.Token);
```

## Combining Multiple Cancellation Sources

### Linked tokens

Link an application-wide shutdown token with a per-request limit. Whichever fires first cancels the request:

```csharp
using var appShutdown = new CancellationTokenSource();           // e.g. from your host
using var perRequest = new CancellationTokenSource(TimeSpan.FromSeconds(5));
using var linked = CancellationTokenSource.CreateLinkedTokenSource(
    appShutdown.Token, perRequest.Token);

try
{
    await Curl.ExecuteAsync("curl https://httpbin.org/delay/10", linked.Token);
}
catch (OperationCanceledException) when (linked.Token.IsCancellationRequested)
{
    var why = appShutdown.IsCancellationRequested ? "shutting down" : "per-request limit";
    Console.WriteLine($"Cancelled: {why}");
}
```

## Cleaning Up After Cancellation

If a cancelled request was writing to a file with `-o`, remove the partial file so nobody mistakes it for a complete download:

```csharp
const string path = "big.bin";
try
{
    await Curl.ExecuteAsync($"curl -o {path} https://httpbin.org/bytes/1048576", ct);
}
catch (OperationCanceledException) when (ct.IsCancellationRequested)
{
    if (File.Exists(path)) File.Delete(path);   // never leave a half-written file behind
    throw;
}
```

## Batch Operations with Cancellation

`Parallel.ForEachAsync` hands each iteration a token linked to the one you pass in. One `CancelAfter` covers the whole batch:

```csharp
var urls = new[] { "https://httpbin.org/delay/1", "https://httpbin.org/delay/2" };
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

await Parallel.ForEachAsync(urls, cts.Token, async (url, ct) =>
{
    try
    {
        var result = await Curl.ExecuteAsync($"curl {url}", ct);
        Console.WriteLine($"{url} -> {result.StatusCode}");
    }
    catch (OperationCanceledException) when (ct.IsCancellationRequested)
    {
        Console.WriteLine($"{url} -> cancelled");
    }
});
```

Parallel patterns continue in [Tutorial 13](13-parallel-requests.html).

## Cancellation in Background Services

A `BackgroundService` receives a `stoppingToken` that the host cancels at shutdown. Pass it straight through:

```csharp
public class PollingService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var r = await Curl.ExecuteAsync("curl https://httpbin.org/get", stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }   // host is stopping
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
```

## Retry with Cancellation

curl's own `--retry` works in CurlDotNet, and it stops as soon as the token is cancelled: the delay between attempts is cancellable too.

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

// Retries timeouts and HTTP 408/429/500/502/503/504, backing off 1 s, 2 s, 4 s...
var result = await Curl.ExecuteAsync(
    "curl --retry 3 -m 5 https://httpbin.org/status/503", cts.Token);

// The .NET convenience: retry any failure 3 times, 500 ms apart
var r2 = await Curl.ExecuteAsync("curl https://httpbin.org/get",
    new CurlSettings().WithRetries(count: 3, delayMs: 500).WithCancellation(cts.Token));
```

If you need your own policy (only certain exceptions, custom back-off), write the loop yourself and check the token before each attempt so a caller who gave up does not have to wait out the remaining retries:

```csharp
async Task<CurlResult> RetryRequest(string cmd, int attempts, CancellationToken ct)
{
    for (int i = 1; ; i++)
    {
        ct.ThrowIfCancellationRequested();            // stop retrying if the caller gave up
        try
        {
            return await Curl.ExecuteAsync(cmd, ct);
        }
        catch (CurlCouldntConnectException) when (i < attempts)
        {
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)), ct);
        }
    }
}
```

The [retry cookbook page](../cookbook/retry.html) covers `--retry`, `--retry-delay`, `--retry-max-time`, `--retry-all-errors` and `--retry-connrefused` in detail.

## Best Practices

1. **Dispose every `CancellationTokenSource`** - `using var` is enough.
2. **Catch `OperationCanceledException` with a `when (token.IsCancellationRequested)` filter** - that is what CurlDotNet throws on cancellation, just like `HttpClient`.
3. **Use `-m` for request deadlines and tokens for caller intent** - they are different tools and different exceptions.
4. **Link tokens instead of nesting try/catch blocks** - one linked source handles shutdown plus per-request limits.
5. **Clean up partial output files** when a `-o` download is cancelled.
6. **Pass the token all the way down** - to `Task.Delay`, file writes and other awaits as well as to CurlDotNet.
7. **Check the token before retrying.**

## Summary

- `Curl.ExecuteAsync(cmd, cancellationToken)` and `CurlRequestBuilder...ExecuteAsync(ct)` accept tokens; so do `CurlSettings.WithCancellation` and `LibCurl.GetAsync(url, null, ct)`.
- Cancellation throws the standard `OperationCanceledException`; `-m` timeouts throw `CurlOperationTimeoutException` (exit code 28).
- `--retry N` and `CurlSettings.WithRetries` both honour the token between attempts.
- `CancelAfter`, linked sources and `BackgroundService` stopping tokens all plug in the same way.

## What's Next?

Learn about [parallel requests](13-parallel-requests.html) to run many requests at once.

---

[Previous: Forms and Data](11-forms-and-data.html) | [Next: Parallel Requests](13-parallel-requests.html)
