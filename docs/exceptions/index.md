---
title: "CurlDotNet Exceptions: Every curl Error in C# Explained"
description: "CurlDotNet exception reference: every curl error in C#, which CurlException is thrown for timeouts, DNS, SSL, redirects and bad URLs, and how to catch it."
---

# CurlDotNet Exception Reference

CurlDotNet exception reference: every curl error in C# maps to a subclass of `CurlDotNet.Exceptions.CurlException`, and this page lists which exception is actually thrown for timeouts, connection and DNS failures, SSL problems, redirect limits, malformed URLs and invalid commands, with a page per category.

## How Errors Surface

There are two kinds of failure, and they surface differently:

- **The request never completed** (timeout, could not connect, bad URL, too many redirects). `Curl.ExecuteAsync` throws a `CurlException` subclass.
- **The server answered with 4xx or 5xx**. Nothing is thrown. You get a `CurlResult` with `IsSuccess == false` and the `StatusCode`. Call `result.EnsureSuccess()` if you want an exception (`CurlDotNet.Core.CurlHttpException`).

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/status/404");
    Console.WriteLine(result.IsSuccess ? result.Body : $"HTTP {result.StatusCode}");
}
catch (CurlException ex)
{
    Console.WriteLine($"{ex.GetType().Name}: {ex.Message} (curl code {ex.CurlErrorCode})");
}
```

## What Is Thrown When

| Situation | Exception | Useful properties |
|-----------|-----------|-------------------|
| `-m`/`--max-time` expired | `CurlOperationTimeoutException` (a `CurlTimeoutException`) | `Timeout` |
| Your `CancellationToken` was cancelled | `CurlAbortedByCallbackException` | `Command` |
| DNS failure, connection refused, TLS handshake failed | `CurlCouldntConnectException` | `Host`, `Port` |
| `--max-redirs` exceeded | `CurlTooManyRedirectsException` | `RedirectCount` |
| URL cannot be parsed | `CurlMalformedUrlException` | `MalformedUrl` |
| URL scheme not supported | `CurlUnsupportedProtocolException` | `Protocol` |
| Command has no URL or bad syntax | `CurlInvalidCommandException` | `InvalidPart` |
| `-d @file` or `-F name=@file` missing | `CurlFileCouldntReadException` | `FilePath` |
| `result.EnsureSuccess()` on a 4xx/5xx | `CurlDotNet.Core.CurlHttpException` | `StatusCode`, `ResponseBody`, `ResponseHeaders` |

## Exception Categories

- [Authentication errors](auth-errors.html) - 401/403 responses, `-u`, bearer tokens
- [Cookie errors](cookie-errors.html) - `-b` cookie strings and what is not yet supported
- [DNS and connection errors](dns-errors.html) - `CurlCouldntConnectException`
- [HTTP errors](http-errors.html) - 4xx/5xx results and `EnsureSuccess`
- [Initialization errors](init-errors.html) - package and setup problems
- [Redirect errors](redirect-errors.html) - `-L`, `--max-redirs`, `CurlTooManyRedirectsException`
- [SSL/TLS errors](ssl-errors.html) - certificate failures and `-k`
- [Timeout errors](timeout-errors.html) - `-m`, `--connect-timeout`, `CurlOperationTimeoutException`
- [URL errors](url-errors.html) - `CurlMalformedUrlException`, `CurlUnsupportedProtocolException`

## Properties on Every CurlException

Every exception derives from `CurlException`, which exposes:

- `Command` - the curl command string that was running
- `CurlErrorCode` - the matching curl exit code (`int?`), for example 28 for a timeout, 7 for connection failure
- `Suggestions` - a `List<string>` of fixes to try
- `DiagnosticInfo` - extra context for logs

```csharp
catch (CurlException ex)
{
    Console.WriteLine(ex.Message);
    foreach (var tip in ex.Suggestions)
        Console.WriteLine($"  - {tip}");
}
```

## Catch Specific Types First

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -L --max-redirs 5 -m 10 https://httpbin.org/redirect/3");
}
catch (CurlTimeoutException ex)       { Console.WriteLine($"Timed out after {ex.Timeout}"); }
catch (CurlCouldntConnectException ex) { Console.WriteLine($"Cannot reach {ex.Host}:{ex.Port}"); }
catch (CurlTooManyRedirectsException ex) { Console.WriteLine($"{ex.RedirectCount} redirects"); }
catch (CurlException ex)              { Console.WriteLine($"Other curl error: {ex.Message}"); }
```

## Related Documentation

- [Tutorial 6: Handling Errors](../tutorials/06-handling-errors.html)
- [Cookbook: handle errors](../cookbook/beginner/handle-errors.html)
- [Troubleshooting: error reference](../troubleshooting/error-reference.html)
