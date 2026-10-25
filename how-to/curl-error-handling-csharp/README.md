---
title: "How to Handle curl Errors in C# (CurlException)"
description: "curl error handling in C#: check IsSuccess for HTTP errors, catch CurlException for network failures, read CurlErrorCode, and use EnsureSuccess()."
---

# How to handle curl errors in C#

curl error handling in C# with CurlDotNet has two layers. HTTP error statuses (404, 500) come back as a normal `CurlResult`, so you check `result.IsSuccess` or call `result.EnsureSuccess()`. Transport failures (DNS, refused connection, timeout, too many redirects) throw a `CurlDotNet.Exceptions.CurlException` subclass, and its `CurlErrorCode` holds the curl CLI exit code.

```bash
dotnet add package CurlDotNet
```

## The pattern

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/status/503");
    if (!result.IsSuccess)
        Console.WriteLine($"HTTP {result.StatusCode}: {result.Body}");
}
catch (CurlTimeoutException ex) { Console.WriteLine($"Timed out: {ex.Message}"); }
catch (CurlException ex)        { Console.WriteLine($"curl error {ex.CurlErrorCode}: {ex.Message}"); }
```

## Turn HTTP errors into exceptions

`EnsureSuccess()` returns the result for chaining and throws for any status outside 200-299. It throws `CurlDotNet.Core.CurlHttpException`, which is **not** a `CurlException`. A different `CurlDotNet.Exceptions.CurlHttpException` also exists, so write the namespace out if you import both:

```csharp
using CurlDotNet;

try
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/404");
    result.EnsureSuccess();
}
catch (CurlDotNet.Core.CurlHttpException ex)
{
    Console.WriteLine($"{ex.StatusCode}: {ex.ResponseBody}");
}
```

`EnsureStatus(201)` and `EnsureContains("text")` work the same way.

Or do it the curl way: with `-f` / `--fail`, a status of 400 or above throws `CurlHttpReturnedErrorException` (exit code 22, a `CurlDotNet.Exceptions.CurlHttpException`) and the body is discarded. `--fail-with-body` throws the same exception but keeps the body:

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl -f https://httpbin.org/status/404");
}
catch (CurlHttpReturnedErrorException ex)
{
    Console.WriteLine($"{ex.CurlErrorCode}: {ex.StatusCode}");  // 22: 404
}
```

## The fluent builder

The builder throws the same exceptions (`LibCurl` is different: it returns a failed `CurlResult` with the exception in `result.Exception`, and throws only `OperationCanceledException`):

```csharp
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

try
{
    var result = await CurlRequestBuilder.Get("https://no-such-host.invalid/")
        .WithTimeout(TimeSpan.FromSeconds(5)).ExecuteAsync();
}
catch (CurlException ex) when (ex.IsRetryable())
{
    Console.WriteLine($"Transient ({ex.GetCurlErrorName()}), try again later.");
}
```

## What is thrown when (observed)

| Situation | Exception | `CurlErrorCode` |
|---|---|---|
| Host name does not resolve | `CurlCouldntResolveHostException` | 6 |
| Proxy host does not resolve | `CurlCouldntResolveProxyException` | 5 |
| Connection refused or unreachable | `CurlCouldntConnectException` | 7 |
| TLS certificate rejected (no `-k`) | `CurlPeerFailedVerificationException` (a `CurlSslException`) | 60 |
| TLS handshake failed | `CurlSslConnectErrorException` (a `CurlSslException`) | 35 |
| Transfer not finished within `-m` (body included) | `CurlOperationTimeoutException` (a `CurlTimeoutException`) | 28 |
| `-f` and a status of 400 or above | `CurlHttpReturnedErrorException` | 22 |
| `--max-redirs` exceeded | `CurlTooManyRedirectsException` | 47 |
| Unsupported scheme such as `gopher://` | `CurlUnsupportedProtocolException` | 1 |
| Command has no URL | `CurlInvalidCommandException` | 2 |
| `-F 'f=@missing.txt'`, `-T missing.txt` | `CurlReadErrorException` | 26 |
| `-o` into a directory that does not exist | `CurlWriteErrorException` | 23 |
| Server closed the connection without a reply | `CurlGotNothingException` | 52 |
| Digest/NTLM negotiation failed | `CurlAuthErrorException` | 94 |
| Your `CancellationToken` fired | `OperationCanceledException` (standard .NET) | n/a |
| 4xx / 5xx response without `-f` | nothing thrown; `IsSuccess == false` | n/a |

DNS and TLS failures have their own codes, so `IsRetryable()` is `true` for 6 and 7 but `false` for a certificate problem. The full list is in the [curl exit code reference](../../reference/curl-exit-codes.md).

## Useful members on CurlException

| Member | Use |
|---|---|
| `CurlErrorCode` | curl exit code (`int?`) |
| `GetCurlErrorName()` | Name for the code |
| `Command` | The command that failed, exactly as written. It may contain secrets; log `ToDetailedString()` or `ToJson()` instead, which mask credentials |
| `Suggestions` | Hints for fixing the problem, when available |
| `IsRetryable()` | `true` for codes 6, 7, 28, 52, 55 and 56 |
| `ToUserFriendlyMessage()` | Text you can show to a user |

## Retrying

Add `--retry <n>` to the command. Like curl, it retries timeouts and HTTP 408, 429, 500, 502, 503 and 504, waits 1 second and doubles the wait each time (`--retry-delay` fixes it), and honours `Retry-After`. `--retry-connrefused` adds refused connections and `--retry-all-errors` retries any failure. When the retries run out, you get the last result or exception as usual:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl --retry 3 --retry-max-time 30 https://httpbin.org/get");
Console.WriteLine(result.StatusCode);
```

`new CurlSettings().WithRetries(3, 1000)` does the same from code, retrying any failure with a fixed delay in milliseconds. See [curl --retry in C#](../../cookbook/retry.md) for the details.

For your own policy (for example with Polly), retry on `IsRetryable()`:

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

for (var attempt = 1; ; attempt++)
{
    try
    {
        var r = await Curl.ExecuteAsync("curl -m 5 https://httpbin.org/get");
        Console.WriteLine(r.StatusCode);
        break;
    }
    catch (CurlException ex) when (ex.IsRetryable() && attempt < 3)
    { await Task.Delay(TimeSpan.FromSeconds(attempt)); }
}
```

## Related

- [Timeouts](../curl-timeout-csharp/README.md)
- [curl --retry in C#](../../cookbook/retry.md)
- [curl exit codes](../../reference/curl-exit-codes.md)
- [Ignore SSL errors with -k](../curl-ignore-ssl-certificate-csharp/README.md)
- [Parse JSON safely](../parse-json-from-curl-in-csharp/README.md)
- [Exception reference](../../exceptions/index.md)
- [All how-to guides](../README.md)
