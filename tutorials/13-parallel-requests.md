---
title: "Parallel curl Requests in C# with Task.WhenAll (CurlDotNet)"
description: "Run parallel curl requests in C# with Task.WhenAll, Curl.ExecuteManyAsync and SemaphoreSlim throttling. Handle partial failures, rate limits and progress."
---

# Tutorial 13: parallel curl requests in C# with Task.WhenAll

This tutorial shows how to run parallel curl requests in C# with Task.WhenAll and CurlDotNet: start many `Curl.ExecuteAsync` calls at once, use the `Curl.ExecuteManyAsync` shortcut, cap concurrency with `SemaphoreSlim`, and handle the failures that happen when some requests succeed and others do not.

`Curl` is a static class. There is nothing to construct, pool or dispose; call it from as many tasks as you like.

One command string can also hold several URLs (`curl URL1 URL2`) or a glob (`https://example.com/page[1-3]`). CurlDotNet runs every one of them, but sequentially, as curl does without `-Z`. `--parallel` (`-Z`) is parsed but not applied (it is listed in `CurlOptions.UnsupportedOptions`), so the transfers still run one after another. For real concurrency use the .NET patterns on this page. See [multiple URLs and globbing](../cookbook/multiple-urls-and-globbing.html) for the single-command form.

## Why Parallel Requests?

Sequential requests wait for each other:

```
Request 1: [====] 200ms
Request 2:       [====] 200ms
Request 3:             [====] 200ms
Total: 600ms
```

Parallel requests overlap:

```
Request 1: [====] 200ms
Request 2: [====] 200ms
Request 3: [====] 200ms
Total: 200ms
```

## Basic Parallel Requests

### Using Task.WhenAll

Start every request first, then await all of them together. The key is to not `await` inside the `Select`:

```csharp
using CurlDotNet;

var urls = new[]
{
    "https://httpbin.org/get?id=1",
    "https://httpbin.org/get?id=2",
    "https://httpbin.org/get?id=3"
};

var tasks = urls.Select(u => Curl.ExecuteAsync($"curl '{u}'"));
var results = await Task.WhenAll(tasks);

foreach (var r in results)
    Console.WriteLine($"{r.StatusCode} in {r.Timings.Total:F0} ms");
```

### Curl.ExecuteManyAsync

When you already have the command strings, `ExecuteManyAsync` does the `Task.WhenAll` for you. Three one-second delays finish in about one second, not three:

```csharp
using System.Diagnostics;

var sw = Stopwatch.StartNew();
var results = await Curl.ExecuteManyAsync(
    "curl https://httpbin.org/delay/1",
    "curl https://httpbin.org/delay/1",
    "curl https://httpbin.org/delay/1");
sw.Stop();

Console.WriteLine($"{results.Length} requests in {sw.ElapsedMilliseconds} ms");   // ~1000
```

### Parallel.ForEachAsync

`Parallel.ForEachAsync` runs a body per item with a built-in concurrency cap and a cancellation token for each iteration:

```csharp
using System.Collections.Concurrent;

var bodies = new ConcurrentBag<string>();

await Parallel.ForEachAsync(Enumerable.Range(1, 20),
    new ParallelOptions { MaxDegreeOfParallelism = 4 },
    async (id, ct) =>
{
    var r = await Curl.ExecuteAsync($"curl 'https://httpbin.org/get?id={id}'", ct);
    if (r.IsSuccess) bodies.Add(r.Body);
});

Console.WriteLine($"Fetched {bodies.Count} of 20");
```

## Controlling Concurrency

### Limiting parallelism with SemaphoreSlim

Firing a thousand requests at once is rude to the server and slow for you. A `SemaphoreSlim` lets only N requests run at a time while the rest queue:

```csharp
using CurlDotNet.Core;

var gate = new SemaphoreSlim(5);   // at most 5 in flight

async Task<CurlResult> Throttled(string url)
{
    await gate.WaitAsync();
    try { return await Curl.ExecuteAsync($"curl '{url}'"); }
    finally { gate.Release(); }
}

var results = await Task.WhenAll(urls.Select(Throttled));
Console.WriteLine($"Completed {results.Length} requests");
```

### Processing in batches

Batches are a simpler alternative when you also want a pause between groups:

```csharp
const int batchSize = 10;

for (int i = 0; i < urls.Length; i += batchSize)
{
    var batch = urls.Skip(i).Take(batchSize)
                    .Select(u => Curl.ExecuteAsync($"curl '{u}'"));
    var results = await Task.WhenAll(batch);

    Console.WriteLine($"Batch {i / batchSize + 1}: {results.Count(r => r.IsSuccess)} ok");
    await Task.Delay(200);   // breathing room for the server
}
```

## Aggregating Results from Different Sources

Different endpoints can run side by side too. Start each task, wait for all, then read them individually:

```csharp
var repoTask = Curl.ExecuteAsync("curl https://api.github.com/repos/curl/curl");
var userTask = Curl.ExecuteAsync("curl https://api.github.com/users/bagder");
var ipTask   = Curl.ExecuteAsync("curl https://httpbin.org/ip");

await Task.WhenAll(repoTask, userTask, ipTask);

var repo = await repoTask;   // already finished; this just unwraps the result
Console.WriteLine($"GitHub repo: HTTP {repo.StatusCode}, {repo.Body.Length} chars");
Console.WriteLine($"Public IP:   {(await ipTask).Body.Trim()}");
```

## Error Handling in Parallel Operations

Two kinds of failure show up in a batch. HTTP 4xx/5xx responses come back as normal results with `IsSuccess == false`. Network failures (DNS, refused connection, timeout) throw, and `Task.WhenAll` surfaces only the first exception. Wrap each request so nothing is lost:

```csharp
using CurlDotNet.Exceptions;

async Task<(string Url, CurlResult Result, Exception Error)> TryFetch(string url)
{
    try { return (url, await Curl.ExecuteAsync($"curl '{url}'"), null); }
    catch (CurlException ex) { return (url, null, ex); }
}

foreach (var o in await Task.WhenAll(urls.Select(TryFetch)))
{
    if (o.Error != null)          Console.WriteLine($"{o.Url}: {o.Error.GetType().Name}");
    else if (!o.Result.IsSuccess) Console.WriteLine($"{o.Url}: HTTP {o.Result.StatusCode}");
    else                          Console.WriteLine($"{o.Url}: ok");
}
```

See [Tutorial 6](06-handling-errors.html) for the exception types you may encounter.

## Respecting Rate Limits

Many APIs tell you how much budget is left in response headers. Read them and back off when the server asks:

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/rate_limit");

Console.WriteLine($"Remaining calls: {result.GetHeader("X-RateLimit-Remaining")}");

if (result.StatusCode == 429 || result.StatusCode == 403)
{
    var retryAfter = result.GetHeader("Retry-After") ?? "a while";
    Console.WriteLine($"Rate limited; wait {retryAfter}");
}
```

To stay under a fixed requests-per-second budget, space out the starts:

```csharp
var pace = TimeSpan.FromMilliseconds(200);   // 5 requests per second
var starter = new SemaphoreSlim(1, 1);

async Task<CurlResult> Paced(string url)
{
    await starter.WaitAsync();
    try { await Task.Delay(pace); }
    finally { starter.Release(); }

    return await Curl.ExecuteAsync($"curl '{url}'");
}

var results = await Task.WhenAll(urls.Select(Paced));
```

## Progress Tracking

Thread-safe counters give you a live progress line without any extra classes:

```csharp
int done = 0, failed = 0;

var tasks = urls.Select(async url =>
{
    var r = await Curl.ExecuteAsync($"curl '{url}'");
    if (r.IsSuccess) Interlocked.Increment(ref done);
    else Interlocked.Increment(ref failed);
    Console.Write($"\r{done + failed}/{urls.Length} complete");
});

await Task.WhenAll(tasks);
Console.WriteLine($"\n{done} ok, {failed} failed");
```

## Cancelling a Whole Batch

Pass one token to every request and cancel the source once. Each cancelled request throws the standard `OperationCanceledException`:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

var tasks = urls.Select(u => Curl.ExecuteAsync($"curl '{u}'", cts.Token));

try
{
    var results = await Task.WhenAll(tasks);
}
catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
{
    Console.WriteLine("Batch cancelled before every request finished");
}
```

[Tutorial 12](12-cancellation-tokens.html) covers cancellation in depth.

## Best Practices

1. **Start tasks before awaiting them** - `await` inside `Select` turns parallel code back into sequential code.
2. **Cap concurrency** - `SemaphoreSlim` or `MaxDegreeOfParallelism`; five to ten in flight is plenty for most APIs.
3. **Wrap each request when failures are expected** - `Task.WhenAll` only rethrows the first exception.
4. **Remember that HTTP errors do not throw** - check `IsSuccess` on every result.
5. **Read rate-limit headers** and honour `Retry-After` (`--retry` does this for you on 429 and 503; see [retries](../cookbook/retry.html)).
6. **Use one cancellation token for the batch** so a shutdown stops everything at once.
7. **Quote URLs** in the command string when they contain `?` or `&`.

## Summary

- `Task.WhenAll` over `Curl.ExecuteAsync` runs requests concurrently; `Curl.ExecuteManyAsync` is the shorthand.
- `SemaphoreSlim`, batches and `Parallel.ForEachAsync` limit how many run at once.
- Wrap requests to capture every failure, and read `IsSuccess` for HTTP errors.

## What's Next?

Learn about [debugging requests](14-debugging-requests.html) to see exactly what goes over the wire.

---

[Previous: Cancellation Tokens](12-cancellation-tokens.html) | [Next: Debugging Requests](14-debugging-requests.html)
