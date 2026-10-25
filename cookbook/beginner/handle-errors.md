---
title: "Handle curl Errors and Exceptions in C# with CurlDotNet"
description: "How to handle curl errors and exceptions in C#: HTTP 4xx/5xx results, -f and EnsureSuccess, curl exit codes, timeouts, connection failures, cancellation and --retry."
---

# Handle curl errors and exceptions in C#

This recipe shows how to handle curl errors and exceptions in C# with CurlDotNet. There are two kinds of failure: HTTP errors (the server answered, but with 4xx or 5xx) come back as a normal result you inspect, unless you ask for an exception with `-f` or `EnsureSuccess()`; transport errors (timeout, DNS, refused connection, TLS, too many redirects) are thrown as typed exceptions that carry curl's exit code. Cancelling your own `CancellationToken` throws the standard `OperationCanceledException`.

## The problem

A request can fail in many different ways and you want code that tells them apart: retry a 503, give up on a 404, report a timeout, and never swallow a bad URL silently.

## Solution 1: check the result for HTTP errors

Like curl without `-f`, `Curl.ExecuteAsync` does not throw for HTTP 4xx/5xx. You get a `CurlResult` with `IsSuccess == false` and the real `StatusCode`, body and headers.

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/404");

if (!result.IsSuccess)
{
    Console.WriteLine($"HTTP {result.StatusCode}");   // HTTP 404
    Console.WriteLine(result.Body);                   // error body, if any
}
```

`IsSuccess` is true for any 2xx status. For a specific code use `result.StatusCode == 201` or `result.EnsureStatus(201)`.

### Turn HTTP errors into exceptions with EnsureSuccess

When a non-2xx answer should abort, call `EnsureSuccess()`. It throws `CurlDotNet.Core.CurlHttpException` carrying `StatusCode`, `ResponseBody` and `ResponseHeaders`, and returns the result otherwise so you can chain.

```csharp
using CurlDotNet;
using CurlDotNet.Core;

try
{
    var user = (await Curl.ExecuteAsync("curl https://api.github.com/users/octocat"))
        .EnsureSuccess()
        .AsJsonDynamic();
    Console.WriteLine(user.login);
}
catch (CurlHttpException ex)
{
    Console.WriteLine($"Server answered {ex.StatusCode}: {ex.ResponseBody}");
}
```

A `CurlHttpException` type also exists in `CurlDotNet.Exceptions`. If a file has both `using CurlDotNet.Core;` and `using CurlDotNet.Exceptions;`, write `catch (CurlDotNet.Core.CurlHttpException ex)` to avoid an ambiguous-reference compile error.

### Fail on HTTP errors the curl way (-f)

`-f` / `--fail` makes an HTTP status of 400 or above an error, as in curl (exit code 22). CurlDotNet throws `CurlHttpReturnedErrorException` (from `CurlDotNet.Exceptions`), which carries `StatusCode`. With `-f` the error body is discarded; `--fail-with-body` keeps it in `ResponseBody`.

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl --fail-with-body https://httpbin.org/status/404");
}
catch (CurlHttpReturnedErrorException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}, exit code {ex.CurlErrorCode}");   // HTTP 404, exit code 22
    Console.WriteLine(ex.ResponseBody);                                       // kept by --fail-with-body
}
```

## Solution 2: catch transport exceptions

Every failure the library reports derives from `CurlDotNet.Exceptions.CurlException`, and `CurlErrorCode` holds the curl exit code the real curl would have returned. Catch the specific types first and the base type last. (Cancellation is the one exception: it is the standard `OperationCanceledException`, see below.)

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -m 5 -L https://httpbin.org/delay/10");
}
catch (CurlOperationTimeoutException ex)   { Console.WriteLine($"Timed out after {ex.Timeout}"); }
catch (CurlCouldntResolveHostException ex) { Console.WriteLine($"Unknown host {ex.Hostname}"); }
catch (CurlCouldntConnectException ex)     { Console.WriteLine($"Cannot reach {ex.Host}:{ex.Port}"); }
catch (CurlTooManyRedirectsException ex)   { Console.WriteLine($"{ex.RedirectCount} redirects"); }
catch (CurlMalformedUrlException ex)       { Console.WriteLine($"Bad URL: {ex.MalformedUrl}"); }
catch (CurlException ex)                   { Console.WriteLine($"curl error {ex.CurlErrorCode}: {ex.Message}"); }
```

The ones you meet most often:

| Exception | Exit code | When it is thrown |
|-----------|-----------|-------------------|
| `CurlException` with code 2 | 2 | Conflicting options such as `-I` with `-d`, or an option missing its value. |
| `CurlMalformedUrlException` | 3 | The URL (or a URL glob) could not be parsed. Has `MalformedUrl`. |
| `CurlCouldntResolveProxyException` | 5 | The proxy host name could not be resolved. |
| `CurlCouldntResolveHostException` | 6 | The host name could not be resolved. Has `Hostname`. |
| `CurlCouldntConnectException` | 7 | The connection was refused or failed (directly or through the proxy). Has `Host` and `Port`. |
| `CurlHttpReturnedErrorException` | 22 | `-f` / `--fail-with-body` and the server answered 400 or above. |
| `CurlWriteErrorException` | 23 | An `-o` file or directory could not be written. |
| `CurlReadErrorException` | 26 | A file named by `-d @file`, `--data-binary @file`, `-F name=@file` or `-T file` could not be read. |
| `CurlOperationTimeoutException` | 28 | `-m` / `--max-time` or `--connect-timeout` expired. Derives from `CurlTimeoutException` (`Timeout` property). |
| `CurlSslConnectErrorException`, `CurlPeerFailedVerificationException` | 35, 60 | The TLS handshake failed, or the certificate was rejected. |
| `CurlTooManyRedirectsException` | 47 | `-L` followed more redirects than `--max-redirs` allows. Has `RedirectCount`. |
| `CurlAuthErrorException` | 94 | Digest/NTLM negotiation failed. A wrong password is a normal 401 result instead. |

The full list, with every curl exit code, is in the [curl exit code reference](../../reference/curl-exit-codes.html).

### Use the Suggestions on the exception

Every `CurlException` carries a `Suggestions` list and a `DiagnosticInfo` string meant for logs and error pages, plus the `Command` that failed and the curl exit code in `CurlErrorCode`.

```csharp
catch (CurlException ex)
{
    Console.WriteLine(ex.Message);
    foreach (var tip in ex.Suggestions)
        Console.WriteLine($"  - {tip}");
    Console.WriteLine(ex.DiagnosticInfo);
}
```

### Cancellation

Pass a `CancellationToken`. When it fires you get the standard .NET `OperationCanceledException` (a `TaskCanceledException` is one), the same as any other async API, so it is easy to tell apart from a `-m` timeout.

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

try
{
    await Curl.ExecuteAsync("curl https://httpbin.org/delay/10", cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Cancelled by the caller");
}
```

See the [cancellation tokens tutorial](../../tutorials/12-cancellation-tokens.html).

## Retrying transient failures

curl's `--retry N` works as it does in curl: it retries timeouts and HTTP 408, 429, 500, 502, 503 and 504, waits 1 second and doubles the wait each time (or uses `--retry-delay`), and honours a `Retry-After` header. `--retry-connrefused` also retries refused connections, and `--retry-all-errors` retries any failure.

```csharp
var result = await Curl.ExecuteAsync(
    "curl --retry 3 --retry-max-time 60 https://httpbin.org/status/503");

Console.WriteLine(result.StatusCode);   // 503 after the last attempt (no -f, so no exception)
```

From C#, `CurlSettings.WithRetries(count, delayMs)` turns retries on for any command; it retries every kind of failure (like `--retry-all-errors`) with a fixed delay:

```csharp
using CurlDotNet.Core;

var settings = new CurlSettings().WithRetries(3, delayMs: 500);
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get", settings);
```

See [curl --retry in C#](../retry.html) for the details. If you need your own policy (for example only 5xx and timeouts, with jitter), a hand-written loop still works:

```csharp
static async Task<CurlResult> ExecuteWithRetries(string command, int attempts = 3)
{
    for (var i = 1; ; i++)
    {
        try
        {
            var result = await Curl.ExecuteAsync(command);
            if (result.IsSuccess || result.StatusCode < 500 || i == attempts) return result;
        }
        catch (CurlOperationTimeoutException) when (i < attempts) { }
        catch (CurlCouldntConnectException) when (i < attempts) { }
        await Task.Delay(TimeSpan.FromMilliseconds(500 * Math.Pow(2, i - 1)));
    }
}
```

Do not retry 4xx responses: the request itself is wrong and will fail the same way again.

## Builder variant

The request builder returns the same `CurlResult` and throws the same exceptions.

```csharp
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

try
{
    var result = await CurlRequestBuilder
        .Get("https://httpbin.org/status/500")
        .WithTimeout(TimeSpan.FromSeconds(10))
        .ExecuteAsync();
    result.EnsureSuccess();
}
catch (CurlDotNet.Core.CurlHttpException ex) { Console.WriteLine($"HTTP {ex.StatusCode}"); }
catch (CurlException ex)                      { Console.WriteLine(ex.Message); }
```

## Troubleshooting

**My `catch (CurlHttpException)` never runs on a 404.** HTTP errors are results, not exceptions, unless you call `EnsureSuccess()` or `EnsureStatus()`, or add `-f` / `--fail-with-body` to the command (then catch `CurlHttpReturnedErrorException`).

**Cancellation versus timeout.** Your own `CancellationToken` throws `OperationCanceledException`; `-m` / `--max-time` throws `CurlOperationTimeoutException` (exit code 28). Catch them separately to tell "the caller gave up" from "the server was too slow".

**Ambiguous reference `CurlHttpException`.** Both `CurlDotNet.Core` and `CurlDotNet.Exceptions` define one. Qualify the type: `catch (CurlDotNet.Core.CurlHttpException ex)`.

**A self-signed certificate fails with `CurlPeerFailedVerificationException`.** Certificate problems are TLS exceptions (exit code 60; a failed handshake is `CurlSslConnectErrorException`, exit code 35), not connection failures. For development only, add `-k` (or `.Insecure()` on the builder). For production, trust the CA instead: `--cacert ca.pem` (and client certificates with `--cert`) work as in curl on .NET 5 and later; see [SSL errors](../../exceptions/ssl-errors.html).

**I want to validate the command before running it.** `Curl.Validate("curl ...")` returns a `ValidationResult` with `IsValid` and `Error` without sending anything. A misspelled option makes it invalid.

## Related recipes

- [Simple GET request](simple-get.html)
- [Call a REST API](call-api.html)
- [Timeouts](../timeouts.html)
- [Follow redirects](../follow-redirects.html)
- [curl --retry in C#](../retry.html)
- [curl exit codes](../../reference/curl-exit-codes.html)
- [Cookbook index](../index.html)
- Reference: [Exceptions overview](../../exceptions/index.html), [HTTP errors](../../exceptions/http-errors.html), [Timeout errors](../../exceptions/timeout-errors.html), [Redirect errors](../../exceptions/redirect-errors.html), [URL errors](../../exceptions/url-errors.html)
- [Troubleshooting: common issues](../../troubleshooting/common-issues.html) and [error reference](../../troubleshooting/error-reference.html)
- Tutorial: [Handling errors](../../tutorials/06-handling-errors.html)
