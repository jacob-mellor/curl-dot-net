---
title: "curl Timeout in C# (--max-time, --connect-timeout)"
description: "Set a curl timeout in C# with --max-time and --connect-timeout in CurlDotNet, change the default, use the builder and catch CurlOperationTimeoutException."
---

# curl timeout in C# (--max-time and --connect-timeout)

To set a curl timeout in C#, add `-m` / `--max-time` (seconds) to the command you pass to `Curl.ExecuteAsync`. When the deadline passes, CurlDotNet throws `CurlOperationTimeoutException`, the C# equivalent of curl exit code 28.

## The curl command

```csharp
using CurlDotNet;

// httpbin waits 2 seconds; we allow 10, so this succeeds
var result = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/delay/2");
Console.WriteLine(result.StatusCode);   // 200
```

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

If the command has no `-m`, HTTP requests get a 30-second deadline. Change that process-wide:

```csharp
Curl.DefaultMaxTimeSeconds = 15;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");   // now 15 s
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

`CurlSettings` does the same for a command string, in whole seconds:

```csharp
var settings = new CurlSettings().WithTimeout(10);
var result = await Curl.ExecuteAsync("curl https://httpbin.org/delay/2", settings);
```

## --connect-timeout

`--connect-timeout 5` is accepted in the command string and by `WithConnectTimeout`, and FTP transfers honour it. For HTTP today, `--max-time` is the deadline that is enforced, and it covers connecting as well as transferring, so always set `-m` when you need a hard limit.

## Timeout versus cancellation

A `CancellationToken` you cancel yourself raises `CurlAbortedByCallbackException`, not a timeout, so you can tell the two apart:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

try
{
    await Curl.ExecuteAsync("curl -m 30 https://httpbin.org/delay/5", cts.Token);
}
catch (CurlAbortedByCallbackException) { Console.WriteLine("Cancelled by the caller"); }
catch (CurlOperationTimeoutException) { Console.WriteLine("Hit --max-time"); }
```

## Gotchas

- `-m` is the whole operation, including redirects followed with `-L` and the download itself. A 1 GB download needs a long `-m`.
- The timeout is in seconds; `-m 0.5` is not supported, use whole seconds.
- Retries are not automatic: `--retry` is not implemented yet, so wrap the call in a loop if a second attempt makes sense (see [handle errors](beginner/handle-errors.html)).

## Related

- [Timeout errors](../exceptions/timeout-errors.html) - exception reference
- [CancellationToken tutorial](../tutorials/12-cancellation-tokens.html)
- [curl download file in C#](beginner/download-file.html)
- [Cookbook index](index.html)
