---
title: "How to Set a curl Timeout (-m) in C#"
description: "Set a curl timeout in C#: use -m / --max-time with CurlDotNet, WithTimeout on the builder, or a CancellationToken. Catch CurlTimeoutException."
---

# How to set a curl timeout in C#

To set a curl timeout in C#, add `-m <seconds>` (or `--max-time`) to the command you pass to `Curl.ExecuteAsync`, or call `.WithTimeout(TimeSpan)` on `CurlRequestBuilder`. When the limit is hit, CurlDotNet throws a `CurlOperationTimeoutException` (a `CurlTimeoutException`) with curl error code 28. Like the curl CLI, there is no timeout at all unless you set one.

```bash
dotnet add package CurlDotNet
```

## The curl command

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    // httpbin waits 5 seconds before answering; we allow 2
    await Curl.ExecuteAsync("curl -m 2 https://httpbin.org/delay/5");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"{ex.CurlErrorCode}: {ex.Message}");  // 28: Operation timed out after 2000 milliseconds ...
}
```

The exception thrown is the more specific `CurlOperationTimeoutException`, which derives from `CurlTimeoutException`, so catching the base type covers both. `-m` accepts decimals too: `-m 2.5`.

## The fluent builder

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/delay/1")
    .WithTimeout(TimeSpan.FromSeconds(10))
    .ExecuteAsync();

Console.WriteLine(result.StatusCode);
```

## Per-call settings and global defaults

```csharp
using CurlDotNet;
using CurlDotNet.Core;

Curl.DefaultMaxTimeSeconds = 30;  // applies when a command has no -m

var settings = new CurlSettings().WithTimeout(10);
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get", settings);
```

`DefaultMaxTimeSeconds` is `0` out of the box, and the internal `HttpClient` has an infinite timeout. When neither `-m` nor a positive default is set, there is no transfer timeout at all, exactly like the curl CLI: a server that never answers keeps the call waiting until you cancel it. Set `-m`, `DefaultMaxTimeSeconds` or a `CancellationToken` for anything that must finish. `Curl.DefaultConnectTimeoutSeconds` works the same way for `--connect-timeout`.

## Cancel from outside with a CancellationToken

Use a token when the caller decides, for example on a user's Cancel button or `HttpContext.RequestAborted`:

```csharp
using CurlDotNet;

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
try
{
    await Curl.ExecuteAsync("curl https://httpbin.org/delay/5", cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Cancelled by the caller.");
}
```

Cancellation surfaces as the standard `OperationCanceledException` (a `TaskCanceledException` is one too), so the usual .NET pattern works. `CurlRequestBuilder.ExecuteAsync(token)` and every `LibCurl` method also take a `CancellationToken`; `LibCurl` reports other failures through `result.Exception` but still throws `OperationCanceledException` when you cancel.

## What -m covers

`-m` caps the **whole transfer**, as in the curl CLI: connecting, sending, waiting for the headers and reading the body. A download that is still streaming when the limit is reached is stopped with `CurlOperationTimeoutException`, and the message says how many bytes had arrived. A `CancellationToken` likewise stops the request at any point, including while the body is downloading.

## About --connect-timeout

`--connect-timeout <seconds>` (or `WithConnectTimeout(...)`, or `Curl.DefaultConnectTimeoutSeconds`) limits only the connection phase, so a dead host fails fast while a slow but working download is left alone. It accepts decimals. On .NET 5 and later it is enforced per connection attempt; on the netstandard2.0 and .NET Framework builds it is best effort, so pair it with `-m` there.

## Choosing a value

- Interactive calls: 5-15 seconds.
- Server-to-server inside a request pipeline: shorter than your own request timeout.
- Large downloads: a generous `-m` (it includes the body), or only `--connect-timeout` plus a `CancellationToken` you control.
- Serverless functions: below the platform's function timeout; see [Azure Functions and AWS Lambda](../curl-in-azure-functions-aws-lambda/README.md).

To retry a timed-out request, add `--retry <n>` (timeouts and HTTP 408, 429, 500, 502, 503 and 504 are retried, with back-off) or use `CurlSettings.WithRetries`. See [curl --retry in C#](../../cookbook/retry.md).

## Related

- [curl --retry in C#](../../cookbook/retry.md)
- [Error handling](../curl-error-handling-csharp/README.md)
- [Run curl synchronously](../curl-synchronously-in-csharp/README.md)
- [Download a file with curl -o](../curl-download-file-csharp/README.md)
- [All how-to guides](../README.md)
