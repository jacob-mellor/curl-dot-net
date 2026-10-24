---
title: "curl HTTP Errors in C#: 404, 500 and EnsureSuccess"
description: "Handle curl HTTP errors in C#: CurlDotNet returns 4xx/5xx as a CurlResult with StatusCode; result.EnsureSuccess() throws CurlHttpException. Retry patterns."
---

# curl HTTP Errors in C# (4xx and 5xx Status Codes)

curl HTTP errors in C# (404 Not Found, 401 Unauthorized, 500 Internal Server Error and friends) do not throw from `Curl.ExecuteAsync`. Like command-line curl, CurlDotNet treats a 4xx or 5xx reply as a completed request: you get a `CurlResult` with `IsSuccess == false`, the `StatusCode`, the headers and the body. Call `result.EnsureSuccess()` when you want an exception instead.

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

Because 5xx responses are returned rather than thrown, a retry loop just inspects `StatusCode`. Keep `-m` on the command so a hung server cannot stall the loop:

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
