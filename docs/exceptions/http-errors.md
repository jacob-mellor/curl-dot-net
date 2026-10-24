---
title: "curl HTTP Errors in C#: 404, 500 and EnsureSuccess"
description: "Handle curl HTTP errors in C#: CurlDotNet returns 4xx/5xx as a CurlResult with StatusCode; -f throws CurlHttpReturnedErrorException (22) and EnsureSuccess() throws. --retry."
---

# curl HTTP Errors in C# (4xx and 5xx Status Codes)

curl HTTP errors in C# (404 Not Found, 401 Unauthorized, 500 Internal Server Error and friends) do not throw from `Curl.ExecuteAsync` unless you ask for it. Like command-line curl, CurlDotNet treats a 4xx or 5xx reply as a completed request: you get a `CurlResult` with `IsSuccess == false`, the `StatusCode`, the headers and the body. Add `-f`/`--fail` to the command (curl's exit code 22, `CurlHttpReturnedErrorException`), or call `result.EnsureSuccess()`, when you want an exception instead.

## Status Code Categories

### Client Errors (4xx)

- **400 Bad Request**: invalid request syntax or body
- **401 Unauthorized**: authentication required (see [authentication errors](auth-errors.html))
- **403 Forbidden**: authenticated but not allowed
- **404 Not Found**: the resource does not exist
- **429 Too Many Requests**: rate limited; check the `Retry-After` header

### Server Errors (5xx)

- **500 Internal Server Error**: server-side failure
- **502 Bad Gateway**: invalid upstream response
- **503 Service Unavailable**: overloaded or in maintenance
- **504 Gateway Timeout**: upstream timed out

## Checking the Status Code

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/404");

if (!result.IsSuccess)
{
    Console.WriteLine($"HTTP {result.StatusCode}");
    Console.WriteLine(result.Body);   // the server's error page or JSON, if any
}
```

`IsSuccess` is `true` for 200-299 only. A 3xx without `-L` is also reported as not successful; see [redirect errors](redirect-errors.html).

## Throwing with -f / --fail

`-f` makes the command fail on any status of 400 or above, exactly like the curl tool exiting with code 22. CurlDotNet throws `CurlHttpReturnedErrorException` (a `CurlDotNet.Exceptions.CurlHttpException`) with `StatusCode`, `StatusText`, `ResponseHeaders` and `CurlErrorCode` 22:

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -f https://httpbin.org/status/404");
}
catch (CurlHttpReturnedErrorException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode} (exit code {ex.CurlErrorCode})");   // HTTP 404 (exit code 22)
}
```

With `-f` the body is discarded, as in curl. Use `--fail-with-body` to keep it: it is then in `ex.ResponseBody`, and in `ex.StandardOutput` together with anything else the command printed (for example the `-i` headers or `-w` text). Without `-f`, nothing is thrown and you get the `CurlResult` described above.

## Throwing with EnsureSuccess

`EnsureSuccess()` returns the same result on 2xx and throws `CurlDotNet.Core.CurlHttpException` otherwise:

```csharp
using CurlDotNet;

try
{
    var result = (await Curl.ExecuteAsync("curl https://httpbin.org/status/503")).EnsureSuccess();
}
catch (CurlDotNet.Core.CurlHttpException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}");
    Console.WriteLine(ex.ResponseBody);
    Console.WriteLine(ex.ResponseHeaders.TryGetValue("Retry-After", out var ra) ? ra : "no Retry-After");
}
```

`EnsureStatus(200)` does the same for one exact status code. The type name is written out in full because a different `CurlHttpException` also exists in `CurlDotNet.Exceptions`; do not import both namespaces and catch the bare name.

The extension method `result.EnsureSuccessStatusCode()` (namespace `CurlDotNet`, modelled on `HttpResponseMessage`) throws that other type, `CurlDotNet.Exceptions.CurlHttpException`, which is a `CurlException` and has `StatusCode`, `ResponseBody`, `IsClientError`, `IsServerError`, `IsNotFound` and similar helpers. Its `CurlErrorCode` is `null`.

## HTTP Exception Types

| Exception | `CurlErrorCode` | Thrown by |
|-----------|----:|-----------|
| `CurlHttpReturnedErrorException` | 22 | A command with `-f`/`--fail` or `--fail-with-body` and a status >= 400 |
| `CurlDotNet.Core.CurlHttpException` | (not a `CurlException`) | `result.EnsureSuccess()`, `result.EnsureStatus(code)` |
| `CurlDotNet.Exceptions.CurlHttpException` | none | `result.EnsureSuccessStatusCode()`; base of `CurlHttpReturnedErrorException` |
| `CurlRetryException` | none | `CurlDotNet.Middleware.RetryMiddleware` after every attempt failed with a retryable exception; `LastAttemptException` and `InnerException` hold the last failure |
| `CurlRateLimitException` | none | Reserved: nothing throws it today; a 429 is a normal result (or a 22 with `-f`) |
| `CurlHttpPostErrorException` | 34 | Reserved: nothing throws it today |

Broken replies (empty reply, cut-short body, malformed headers, undecodable encoding) are transfer errors with their own exit codes; see [transfer and file errors](transfer-errors.html).

## Handling by Status

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/429");

switch (result.StatusCode)
{
    case 401: Console.WriteLine("Refresh credentials"); break;
    case 404: Console.WriteLine("Check the URL"); break;
    case 429:
        var wait = int.TryParse(result.GetHeader("Retry-After"), out var s) ? s : 60;
        await Task.Delay(TimeSpan.FromSeconds(wait));
        break;
    case >= 500: Console.WriteLine("Server error - retry later"); break;
}
```

## Retrying Server Errors

The simplest fix is curl's own `--retry N`: it retries HTTP 408, 429, 500, 502, 503 and 504 (and timeouts), waits 1 second and doubles the pause each time unless `--retry-delay` is given, and honours a `Retry-After` header:

```csharp
var result = await Curl.ExecuteAsync("curl --retry 3 -m 10 https://httpbin.org/status/503");
Console.WriteLine(result.StatusCode);   // 503 after four attempts, or 200 if the server recovered
```

`new CurlSettings().WithRetries(3, 1000)` does the same from code (and retries any failure). See [curl --retry in C#](../cookbook/retry.html).

Because 5xx responses are returned rather than thrown, a hand-written retry loop just inspects `StatusCode`. Keep `-m` on the command so a hung server cannot stall the loop:

```csharp
async Task<CurlResult> RetryOnServerError(string command, int attempts = 3)
{
    var delay = TimeSpan.FromSeconds(1);
    for (var i = 1; ; i++)
    {
        var result = await Curl.ExecuteAsync(command);
        if (result.StatusCode < 500 || i == attempts) return result;
        await Task.Delay(delay);
        delay *= 2;   // 1s, 2s, 4s
    }
}
```

(`CurlResult` is in `CurlDotNet.Core`.)

## Getting Just the Status Code

`-w "%{http_code}"` appends the status code to the output, the same as curl. Status endpoints return an empty body, so the body is just the code:

```csharp
var result = await Curl.ExecuteAsync("curl -w '%{http_code}' https://httpbin.org/status/418");
Console.WriteLine(result.Body);   // 418
```

Or use `-I` for a HEAD request and read `result.StatusCode`.

## Related Documentation

- [Authentication errors](auth-errors.html)
- [Redirect errors](redirect-errors.html)
- [Tutorial 6: Handling Errors](../tutorials/06-handling-errors.html)
- [Cookbook: handle errors](../cookbook/beginner/handle-errors.html)
- [HTTP status codes (MDN)](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status)
- [Exception reference](index.html)
