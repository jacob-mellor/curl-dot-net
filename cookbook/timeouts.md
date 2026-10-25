---
title: "curl Timeout in C# (--max-time, --connect-timeout)"
description: "Set a curl timeout in C# with --max-time and --connect-timeout in CurlDotNet, change the default, use the builder and catch CurlOperationTimeoutException."
---

# curl timeout in C# (--max-time and --connect-timeout)

To set a curl timeout in C#, add `-m` / `--max-time` (seconds) to the command you pass to `Curl.ExecuteAsync`. When the deadline passes, CurlDotNet throws `CurlOperationTimeoutException`, the C# equivalent of curl exit code 28. Like curl, there is no timeout at all until you set one.

## The curl command

```csharp
using CurlDotNet;

// httpbin waits 2 seconds; we allow 10, so this succeeds
var result = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/delay/2");
Console.WriteLine(result.StatusCode);   // 200
```

`-m` bounds the whole transfer: connecting, sending, waiting for the headers and reading the entire body, including every redirect hop with `-L`. Decimal values work as in curl: `-m 2.5`, `--connect-timeout 0.5`.

## Catch the timeout

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl --max-time 2 https://httpbin.org/delay/5");
}
catch (CurlOperationTimeoutException ex)
{
    Console.WriteLine($"Timed out after {ex.Timeout?.TotalSeconds}s running: {ex.Command}");
}
```

`CurlOperationTimeoutException` derives from `CurlTimeoutException`; catch the base type if you do not care which timeout fired.

## The default when you set nothing

If the command has no `-m`, there is no transfer timeout, exactly like curl: a slow download is never cut off by a hidden limit. (CurlDotNet's own `HttpClient` has an infinite `Timeout`.) Set a process-wide default if you want one:

```csharp
Curl.DefaultMaxTimeSeconds = 15;          // like adding -m 15 to every command
Curl.DefaultConnectTimeoutSeconds = 5;    // like adding --connect-timeout 5

var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");   // now limited to 15 s
```

A `-m` in the command always wins over the default.

## Builder style

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Get("https://httpbin.org/delay/2")
    .WithTimeout(TimeSpan.FromSeconds(10))
    .ExecuteAsync();

Console.WriteLine(result.StatusCode);
```

`CurlSettings` does the same for a command string, in whole seconds (`WithConnectTimeout(seconds)` sets the connect timeout):

```csharp
var settings = new CurlSettings().WithTimeout(10);
var result = await Curl.ExecuteAsync("curl https://httpbin.org/delay/2", settings);
```

## --connect-timeout

`--connect-timeout` limits only the time to establish the connection (DNS, TCP and TLS), so a dead host fails fast while a slow but working download can still take as long as `-m` allows:

```csharp
var result = await Curl.ExecuteAsync(
    "curl --connect-timeout 3 -m 300 -o big.bin https://httpbin.org/bytes/1048576");
```

On .NET 5 and later it is enforced per connection attempt, as in curl, and a connect timeout also throws `CurlOperationTimeoutException` (exit code 28) with a "Connection timed out after ... milliseconds" message. On .NET Framework and .NET Standard 2.0 builds it is applied on a best-effort basis, so keep `-m` as the hard limit there. The builder equivalent is `WithConnectTimeout(TimeSpan)`, and FTP transfers honour it too.

## Slow downloads: --speed-limit and --limit-rate

`-m` cuts off a healthy but large download too. To give up only when a transfer stalls, use curl's low-speed check: `--speed-limit BYTES --speed-time SECONDS` aborts when the download stays below that rate for that long, with `CurlOperationTimeoutException` (exit code 28) and curl's "Operation too slow" message. `--speed-time` alone uses a limit of 1 byte per second, and `--speed-limit` alone a window of 30 seconds.

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    // give up if fewer than 1000 bytes/s arrive for 15 seconds in a row
    var result = await Curl.ExecuteAsync(
        "curl --speed-limit 1000 --speed-time 15 -o big.bin https://httpbin.org/bytes/1048576");
}
catch (CurlOperationTimeoutException ex) when (ex.Message.StartsWith("Operation too slow"))
{
    Console.WriteLine("The download stalled");
}
```

`--limit-rate` works the other way round: it caps the download speed (`--limit-rate 100k` is 102400 bytes per second; `k`, `M` and `G` suffixes work), which is `CurlOptions.LimitRate`. Both apply to http and https response bodies; uploads are not throttled.

## Timeout versus cancellation

A `CancellationToken` you cancel yourself raises the standard `OperationCanceledException`, not a timeout, so you can tell the two apart:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

try
{
    await Curl.ExecuteAsync("curl -m 30 https://httpbin.org/delay/5", cts.Token);
}
catch (OperationCanceledException) { Console.WriteLine("Cancelled by the caller"); }
catch (CurlOperationTimeoutException) { Console.WriteLine("Hit --max-time"); }
```

`OperationCanceledException` is not a `CurlException`, so a `catch (CurlException)` block does not swallow a cancellation.

## Gotchas

- `-m` is the whole operation, including redirects followed with `-L` and the download itself. A 1 GB download needs a long `-m`, or none.
- The timeout is in seconds and may be fractional: `-m 0.5` is half a second.
- Timeouts are retried when you ask for it: `--retry 3` retries a timed-out or too-slow transfer (and HTTP 408/429/5xx), with `--retry-max-time` capping the total. See [curl --retry in C#](retry.html).

## Related

- [Timeout errors](../exceptions/timeout-errors.html) - exception reference
- [CancellationToken tutorial](../tutorials/12-cancellation-tokens.html)
- [curl --retry in C#](retry.html)
- [curl exit codes](../reference/curl-exit-codes.html)
- [curl download file in C#](beginner/download-file.html)
- [Cookbook index](index.html)
