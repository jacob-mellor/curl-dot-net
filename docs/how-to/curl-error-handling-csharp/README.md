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

## The fluent builder

The builder throws the same exceptions:

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

## What is thrown when (9.7.0, observed)

| Situation | Exception | `CurlErrorCode` |
|---|---|---|
| Host not found, connection refused | `CurlCouldntConnectException` | 7 |
| TLS certificate rejected (no `-k`) | `CurlCouldntConnectException` | 7 |
| No response headers within `-m` (default 30 s) | `CurlTimeoutException` (`CurlOperationTimeoutException` in the current source) | 28 |
| `--max-redirs` exceeded | `CurlTooManyRedirectsException` | 47 |
| Unsupported scheme such as `gopher://` | `CurlUnsupportedProtocolException` | 1 |
| Command has no URL | `CurlInvalidCommandException` | (none) |
| Your `CancellationToken` fired | `CurlAbortedByCallbackException` | 42 |
| `-F 'f=@missing.txt'` | `System.IO.FileNotFoundException` | n/a |
| 4xx / 5xx response | nothing thrown; `IsSuccess == false` | n/a |

Because DNS and TLS failures currently share code 7, `IsRetryable()` returns `true` for a certificate problem too. Check the message before retrying in a loop.

## Useful members on CurlException

| Member | Use |
|---|---|
| `CurlErrorCode` | curl exit code (`int?`) |
| `GetCurlErrorName()` | Name for the code |
| `Command` | The command that failed. It may contain secrets, so redact before logging |
| `Suggestions` | Hints for fixing the problem, when available |
| `IsRetryable()` | `true` for codes 6, 7, 28, 52, 55 and 56 |
| `ToUserFriendlyMessage()` | Text you can show to a user |

## Retrying

`--retry` is not supported and `CurlSettings.WithRetries` is not applied by the engine in 9.7.0, so write the loop yourself (or use Polly):

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
- [Ignore SSL errors with -k](../curl-ignore-ssl-certificate-csharp/README.md)
- [Parse JSON safely](../parse-json-from-curl-in-csharp/README.md)
- [Exception reference](../../exceptions/index.md)
- [All how-to guides](../README.md)
