---
title: "curl --retry in C# (retry, back-off and Retry-After)"
description: "Use curl --retry, --retry-delay, --retry-max-time, --retry-all-errors and --retry-connrefused in C# with CurlDotNet, or CurlSettings.WithRetries, and know which failures are retried."
---

# curl --retry in C#

To retry a request in C# the way curl does, add `--retry N` to the command you pass to `Curl.ExecuteAsync`. CurlDotNet then repeats the transfer up to N more times when it fails with a transient error, waiting between attempts like curl: one second first, doubling each time, capped at ten minutes, and honouring a `Retry-After` header from the server.

## The curl command

```csharp
using CurlDotNet;

// Up to 3 retries for timeouts and HTTP 408, 429, 500, 502, 503 and 504
var result = await Curl.ExecuteAsync("curl --retry 3 -m 10 https://httpbin.org/status/200");
Console.WriteLine(result.StatusCode);   // 200
```

`-m` / `--max-time` limits each attempt, not the whole sequence. Use `--retry-max-time` to cap the total time spent retrying.

## What is retried

| Failure | Retried with `--retry` | Also needs |
|---------|------------------------|------------|
| Timeout (`-m` expired, exit 28) | yes | |
| HTTP 408, 429, 500, 502, 503, 504 | yes | |
| Connection failed (exit 7) | no | `--retry-connrefused` (CurlDotNet retries any connect failure with it; curl only a refused connection) |
| Any other error (DNS failure, other 4xx/5xx with `-f`, TLS, ...) | no | `--retry-all-errors` |

Transient HTTP statuses are retried with or without `-f`. When the last attempt still gets, say, a 503, the result depends on `-f`: without it you get a normal `CurlResult` with `StatusCode` 503; with it `CurlHttpReturnedErrorException` (exit 22) is thrown.

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync(
        "curl -f --retry 4 --retry-connrefused --retry-max-time 60 https://httpbin.org/status/503");
    Console.WriteLine(result.Body);
}
catch (CurlHttpReturnedErrorException ex)
{
    Console.WriteLine($"Still failing after retries: HTTP {ex.StatusCode}");
}
catch (CurlCouldntConnectException ex)
{
    Console.WriteLine($"Server unreachable: {ex.Message}");
}
catch (CurlTimeoutException)
{
    Console.WriteLine("Gave up: --retry-max-time ran out");
}
```

## The retry options

| Option | Meaning |
|--------|---------|
| `--retry N` | Retry up to N times after the first attempt. |
| `--retry-delay SECONDS` | Wait this long between attempts instead of the doubling back-off. Decimals work: `--retry-delay 0.5` (a CurlDotNet extension; the curl tool takes whole seconds). |
| `--retry-max-time SECONDS` | Stop retrying when the next wait would go past this many seconds since the first attempt. |
| `--retry-all-errors` | Retry every failure, not only the transient ones. |
| `--retry-connrefused` | Also retry when the connection cannot be made. |

The waits without `--retry-delay` are 1, 2, 4, 8 ... seconds, up to 600. A `Retry-After: <seconds>` header on a retried response replaces the wait, capped at 600 seconds. Two differences from the curl tool: CurlDotNet honours `Retry-After` even when `--retry-delay` is given (curl then ignores the header), and only the seconds form is read, not an HTTP date.

When `--retry-max-time` runs out after a failed attempt, CurlDotNet throws `CurlOperationTimeoutException` (exit 28) rather than the last attempt's own error as curl does, so catch `CurlTimeoutException` too when you use it.

## Retries without a command string

`CurlOptions` has the same settings as properties, so you can build the request without any quoting:

```csharp
using CurlDotNet;
using CurlDotNet.Core;

var options = new CurlOptions
{
    Url = "https://httpbin.org/get",
    Retry = 3,
    RetryDelay = 2,          // seconds; 0 means curl's doubling back-off
    RetryMaxTime = 30,
    RetryConnRefused = true,
    MaxTime = 10
};

var result = await Curl.ExecuteAsync(options);
Console.WriteLine(result.StatusCode);
```

`CurlSettings.WithRetries(count, delayMs)` is the .NET-style shortcut. It retries any failure (it turns on `--retry-all-errors`) with a fixed delay:

```csharp
using CurlDotNet;
using CurlDotNet.Core;

var settings = new CurlSettings().WithRetries(3, 500);   // 3 retries, 500 ms apart
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get", settings);
Console.WriteLine(result.StatusCode);
```

## Downloads and output files

With `-o`, a failed attempt that already wrote body bytes is truncated before the next attempt, as in curl, so the file ends up holding one complete copy. An attempt that wrote only headers (for example `-i -f` on an error status) is left in the file.

To continue a large download that stopped in an earlier run instead of starting over, run the command again with `-C -`, which resumes from the size of the existing file:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl --retry 5 -C - -o big.iso https://example.com/big.iso");
Console.WriteLine(result.OutputFiles[0]);
```

See [curl download file in C#](beginner/download-file.html) for `-C -` and the other output options.

## Gotchas

- Cancellation is not retried. A cancelled `CancellationToken` stops the retry loop, including a wait between attempts, and throws `OperationCanceledException`.
- `-w` / `--write-out` text: when an attempt returns a transient status without `-f` (a 503, say) and is retried, its body and its `-w` text stay in `Body` ahead of the final attempt's output. The curl tool prints `-w` only once, so with `--retry` prefer reading `result.StatusCode` over parsing `-w` output. `%{num_retries}` is currently always 0.
- With several URLs in one command, each transfer gets its own retries.
- Retries apply to HTTP and HTTPS transfers.

## Related

- [curl timeout in C# (--max-time and --connect-timeout)](timeouts.html)
- [Handle curl errors and exceptions in C#](beginner/handle-errors.html)
- [curl exit codes in C#](../reference/curl-exit-codes.html)
- [curl -w write-out in C#](write-out.html)
- [Cookbook index](index.html)
