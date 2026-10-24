---
title: "CurlDotNet Exceptions: Every curl Error in C# Explained"
description: "CurlDotNet exception reference: every curl error in C#, which CurlException is thrown for timeouts, DNS, SSL, redirects and bad URLs, and how to catch it."
---

# CurlDotNet Exception Reference

CurlDotNet exception reference: every curl error in C# maps to a subclass of `CurlDotNet.Exceptions.CurlException` carrying the curl exit code, and this page lists which exception is actually thrown for timeouts, DNS and connection failures, TLS problems, `-f` HTTP errors, redirect limits, malformed URLs, invalid commands and file errors, with a page per category.

## How Errors Surface

There are three kinds of outcome, and they surface differently:

- **The transfer failed** (timeout, host not found, connection refused, TLS failure, bad URL, too many redirects, unwritable output file). `Curl.ExecuteAsync` throws a `CurlException` subclass whose `CurlErrorCode` is the exit code the curl tool would return.
- **The server answered with 4xx or 5xx**. Without `-f`, nothing is thrown: you get a `CurlResult` with `IsSuccess == false` and the `StatusCode`. Call `result.EnsureSuccess()` if you want an exception (`CurlDotNet.Core.CurlHttpException`). With `-f`/`--fail` the command throws `CurlHttpReturnedErrorException` (exit code 22), like curl.
- **You cancelled.** A cancelled `CancellationToken` throws the standard `OperationCanceledException`, not a `CurlException`.

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

| Situation | Exception (exit code) | Useful properties |
|-----------|-----------|-------------------|
| `-m`/`--max-time` or `--connect-timeout` expired | `CurlOperationTimeoutException` (28), a `CurlTimeoutException` | `Timeout` |
| Your `CancellationToken` was cancelled | `System.OperationCanceledException` | `CancellationToken` |
| Host name does not resolve | `CurlCouldntResolveHostException` (6) | `Hostname` |
| Proxy name does not resolve | `CurlCouldntResolveProxyException` (5) | `ProxyHost` |
| Connection refused or unreachable | `CurlCouldntConnectException` (7) | `Host`, `Port`, `ViaProxy` |
| TLS handshake failed / certificate rejected | `CurlSslConnectErrorException` (35) / `CurlPeerFailedVerificationException` (60), both `CurlSslException` | `Message` |
| `-f`/`--fail` and status >= 400 | `CurlHttpReturnedErrorException` (22) | `StatusCode`, `ResponseBody` |
| `--max-redirs` exceeded | `CurlTooManyRedirectsException` (47) | `RedirectCount` |
| URL cannot be parsed | `CurlMalformedUrlException` (3) | `MalformedUrl` |
| URL scheme not supported | `CurlUnsupportedProtocolException` (1) | `Protocol` |
| Command has options but no URL | `CurlInvalidCommandException` (none) | `InvalidPart` |
| Option missing its value, conflicting methods | `CurlException` (2) | `Message` |
| CR/LF in a `-H` header | `CurlBadFunctionArgumentException` (43) | `ArgumentName` |
| `-o`, `-D` or `-c` file cannot be written | `CurlWriteErrorException` (23) | `FilePath` |
| `-T`, `-F name=@file`, `-K` file missing | `CurlReadErrorException` (26) | `FilePath` |
| Single `-d @file` / `--json @file` missing | `CurlFileCouldntReadException` (37) | `FilePath` |
| Digest/NTLM/Negotiate challenge cannot be processed | `CurlAuthErrorException` (94) | `Scheme`, `Proxy` |
| `result.EnsureSuccess()` on a 4xx/5xx | `CurlDotNet.Core.CurlHttpException` | `StatusCode`, `ResponseBody`, `ResponseHeaders` |

The [error reference](../troubleshooting/error-reference.html) lists every exception type with its exit code, including the reserved ones nothing throws today.

## Exception Categories

- [Authentication errors](auth-errors.html) - 401/403 results, `-u`, Digest/NTLM, `CurlAuthErrorException`
- [Cookie errors](cookie-errors.html) - `-b`, `-c` cookie jars, `-j`, jar write errors
- [DNS and connection errors](dns-errors.html) - `CurlCouldntResolveHostException`, `CurlCouldntConnectException`, proxies
- [HTTP errors](http-errors.html) - 4xx/5xx results, `-f`, `EnsureSuccess`, `--retry`
- [Initialization errors](init-errors.html) - package and setup problems
- [Redirect errors](redirect-errors.html) - `-L`, `--max-redirs`, `CurlTooManyRedirectsException`
- [SSL/TLS errors](ssl-errors.html) - certificate failures and `-k`
- [Timeout errors](timeout-errors.html) - `-m`, `--connect-timeout`, `CurlOperationTimeoutException`, cancellation
- [Transfer and file errors](transfer-errors.html) - partial file, write and read errors, empty reply, `--max-filesize`, FTP
- [URL and command errors](url-errors.html) - `CurlMalformedUrlException`, `CurlUnsupportedProtocolException`, option errors

## Properties on Every CurlException

Every exception derives from `CurlException`, which exposes:

- `Command` - the curl command string that was running
- `CurlErrorCode` - the matching curl exit code (`int?`), for example 28 for a timeout, 7 for connection failure
- `StandardOutput` - what the curl tool would already have printed before failing (`-i` headers, `--fail-with-body` body, `-w` text, earlier URLs of the same command); also in `ex.Data["CurlDotNet.Stdout"]`, with the `-w` text alone in `ex.Data["CurlDotNet.WriteOut"]`
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
- [curl exit codes](../reference/curl-exit-codes.html)
