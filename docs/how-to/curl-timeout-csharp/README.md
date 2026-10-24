---
title: "How to Set a curl Timeout (-m) in C#"
description: "Set a curl timeout in C#: use -m / --max-time with CurlDotNet, WithTimeout on the builder, or a CancellationToken. Catch CurlTimeoutException."
---

# How to set a curl timeout in C#

To set a curl timeout in C#, add `-m <seconds>` (or `--max-time`) to the command you pass to `Curl.ExecuteAsync`, or call `.WithTimeout(TimeSpan)` on `CurlRequestBuilder`. When the limit is hit, CurlDotNet throws a `CurlTimeoutException` with curl error code 28.

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
    Console.WriteLine($"{ex.CurlErrorCode}: {ex.Message}");  // 28: Operation timed out after 2 seconds
}
```

The repository's current source throws the more specific `CurlOperationTimeoutException`, which derives from `CurlTimeoutException`, so catching the base type covers both.

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

`DefaultMaxTimeSeconds` is `0` out of the box. When neither `-m` nor a positive default is set, the HTTP handler applies a 30-second limit, so a slow endpoint needs an explicit `-m`. The curl CLI has no default timeout.

## Cancel from outside with a CancellationToken

Use a token when the caller decides, for example on a user's Cancel button or `HttpContext.RequestAborted`:

```csharp
using CurlDotNet;

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
try
{
    await Curl.ExecuteAsync("curl https://httpbin.org/delay/5", cts.Token);
}
catch (CurlDotNet.Exceptions.CurlAbortedByCallbackException)
{
    Console.WriteLine("Cancelled by the caller.");
}
```

`CurlRequestBuilder.ExecuteAsync(token)` and every `LibCurl` method also take a `CancellationToken`.

## What -m covers in 9.7.0

`-m` (and the 30-second default) limits the time until the **response headers** arrive. CurlDotNet sends with `HttpCompletionOption.ResponseHeadersRead` and then reads the body without a deadline. In our test, `curl -m 3` against an endpoint that streamed its body over 8 seconds completed normally after about 7 seconds. This differs from the curl CLI, where `-m` caps the whole transfer. A `CancellationToken` stops the request the same way: before the headers arrive, but not while the body is downloading.

## About --connect-timeout

`--connect-timeout` and `WithConnectTimeout(...)` are accepted, but in 9.7.0 only the FTP handler applies them. For HTTP and HTTPS, `-m` is the limit that is enforced, and it includes the time spent connecting.

## Choosing a value

- Interactive calls: 5-15 seconds.
- Server-to-server inside a request pipeline: shorter than your own request timeout.
- Large downloads: generous, or use a `CancellationToken` you control.
- Serverless functions: below the platform's function timeout; see [Azure Functions and AWS Lambda](../curl-in-azure-functions-aws-lambda/README.md).

`--retry` is not supported. Retry by catching `CurlTimeoutException` in your own loop or with a resilience library.

## Related

- [Error handling](../curl-error-handling-csharp/README.md)
- [Run curl synchronously](../curl-synchronously-in-csharp/README.md)
- [Download a file with curl -o](../curl-download-file-csharp/README.md)
- [All how-to guides](../README.md)
