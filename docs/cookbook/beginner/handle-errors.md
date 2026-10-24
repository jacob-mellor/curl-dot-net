---
title: "Handle curl Errors and Exceptions in C# with CurlDotNet"
description: "How to handle curl errors and exceptions in C#: HTTP 4xx/5xx results, EnsureSuccess, timeouts, connection failures, redirects, cancellation and retries."
---

# Handle curl errors and exceptions in C#

This recipe shows how to handle curl errors and exceptions in C# with CurlDotNet. There are two kinds of failure: HTTP errors (the server answered, but with 4xx or 5xx) come back as a normal result you inspect, while transport errors (timeout, DNS, refused connection, TLS, too many redirects, cancellation) are thrown as typed exceptions you catch.

## The problem

A request can fail in many different ways and you want code that tells them apart: retry a 503, give up on a 404, report a timeout, and never swallow a bad URL silently.

## Solution 1: check the result for HTTP errors

`Curl.ExecuteAsync` does not throw for HTTP 4xx/5xx. You get a `CurlResult` with `IsSuccess == false` and the real `StatusCode`, body and headers.

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

## Solution 2: catch transport exceptions

Everything the library throws derives from `CurlDotNet.Exceptions.CurlException`. Catch the specific types first and the base type last.

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -m 5 -L https://httpbin.org/delay/10");
}
catch (CurlOperationTimeoutException ex) { Console.WriteLine($"Timed out after {ex.Timeout}"); }
catch (CurlCouldntConnectException ex)   { Console.WriteLine($"Cannot reach {ex.Host}:{ex.Port}"); }
catch (CurlTooManyRedirectsException ex) { Console.WriteLine($"{ex.RedirectCount} redirects"); }
catch (CurlMalformedUrlException ex)     { Console.WriteLine($"Bad URL: {ex.MalformedUrl}"); }
catch (CurlException ex)                 { Console.WriteLine($"curl error {ex.CurlErrorCode}: {ex.Message}"); }
```

What each one means:

| Exception | When it is thrown |
|-----------|-------------------|
| `CurlOperationTimeoutException` | `-m` / `--max-time` expired. Derives from `CurlTimeoutException` (`Timeout` property). |
| `CurlCouldntConnectException` | DNS lookup failed, connection refused, or the TLS handshake failed. Has `Host` and `Port`. |
| `CurlTooManyRedirectsException` | `-L` followed more redirects than `--max-redirs` allows. Has `RedirectCount`. |
| `CurlMalformedUrlException` | The URL could not be parsed. Has `MalformedUrl`. |
| `CurlInvalidCommandException` | The command string is not valid curl, for example no URL. Has `InvalidPart`. |
| `CurlFileCouldntReadException` | `-d @file` or `--data-binary @file` points at a file that does not exist. |
| `CurlAbortedByCallbackException` | Your `CancellationToken` was cancelled. |

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

Pass a `CancellationToken`. When it fires you get `CurlAbortedByCallbackException`, not `OperationCanceledException`.

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

try
{
    await Curl.ExecuteAsync("curl https://httpbin.org/delay/10", cts.Token);
}
catch (CurlAbortedByCallbackException)
{
    Console.WriteLine("Cancelled by the caller");
}
```

See the [cancellation tokens tutorial](../../tutorials/12-cancellation-tokens.html).

## Retrying transient failures

curl's `--retry` flag is not implemented yet, and `CurlSettings.WithRetries` is accepted but not acted on in the current release, so write the retry loop yourself. That also gives you control over which failures retry (for example only 5xx and timeouts):

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

**My `catch (CurlHttpException)` never runs on a 404.** HTTP errors are results, not exceptions, unless you call `EnsureSuccess()` or `EnsureStatus()`.

**`catch (OperationCanceledException)` never runs.** Cancellation surfaces as `CurlAbortedByCallbackException`. Catch that, or the base `CurlException`.

**Ambiguous reference `CurlHttpException`.** Both `CurlDotNet.Core` and `CurlDotNet.Exceptions` define one. Qualify the type: `catch (CurlDotNet.Core.CurlHttpException ex)`.

**A self-signed certificate fails with `CurlCouldntConnectException`.** TLS handshake failures map to this type. For development only, add `-k` (or `.Insecure()` on the builder).

**I want to validate the command before running it.** `Curl.Validate("curl ...")` returns a `ValidationResult` with `IsValid` and `Error` without sending anything.

## Related recipes

- [Simple GET request](simple-get.html)
- [Call a REST API](call-api.html)
- [Timeouts](../timeouts.html)
- [Follow redirects](../follow-redirects.html)
- [Cookbook index](../index.html)
- Reference: [Exceptions overview](../../exceptions/index.html), [HTTP errors](../../exceptions/http-errors.html), [Timeout errors](../../exceptions/timeout-errors.html), [Redirect errors](../../exceptions/redirect-errors.html), [URL errors](../../exceptions/url-errors.html)
- [Troubleshooting: common issues](../../troubleshooting/common-issues.html) and [error reference](../../troubleshooting/error-reference.html)
- Tutorial: [Handling errors](../../tutorials/06-handling-errors.html)
