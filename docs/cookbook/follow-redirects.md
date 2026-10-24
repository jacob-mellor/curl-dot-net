---
title: "curl Follow Redirects in C# (curl -L) with CurlDotNet"
description: "Make curl follow redirects in C# with curl -L and --max-redirs using CurlDotNet and CurlRequestBuilder, and catch CurlTooManyRedirectsException."
---

# curl follow redirects in C# (curl -L)

To make curl follow redirects in C#, add `-L` to the command string you pass to `Curl.ExecuteAsync`, exactly as you would on the command line. Without `-L` CurlDotNet returns the 301/302 response itself; with it, the final response after the redirect chain is what you get back.

## The curl command

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -L https://httpbin.org/redirect/3");

Console.WriteLine(result.StatusCode);   // 200 (after three hops)
Console.WriteLine(result.Body);
```

Without `-L` the same request stops at the first hop:

```csharp
var hop = await Curl.ExecuteAsync("curl https://httpbin.org/redirect/3");

Console.WriteLine(hop.StatusCode);                 // 302
Console.WriteLine(hop.GetHeader("Location"));      // /relative-redirect/2
```

## Limit the number of hops

`--max-redirs` caps the chain. Exceeding it throws `CurlTooManyRedirectsException`, which carries the hop count:

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl -L --max-redirs 2 https://httpbin.org/redirect/5");
}
catch (CurlTooManyRedirectsException ex)
{
    Console.WriteLine($"Gave up after {ex.RedirectCount} redirects");
}
```

`-L --max-redirs 0` throws on the very first redirect, which is a handy way to assert that an endpoint does not redirect.

## Builder style

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Get("https://httpbin.org/redirect/3")
    .FollowRedirects()
    .WithMaxRedirects(5)
    .ExecuteAsync();

Console.WriteLine(result.StatusCode);   // 200
```

## Follow redirects by default

Set the process-wide default once (for example at startup) and every command behaves as if `-L` were present:

```csharp
using CurlDotNet;

Curl.DefaultFollowRedirects = true;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/redirect/1");
Console.WriteLine(result.StatusCode);   // 200
```

Per call, `CurlSettings` does the same without touching the global default:

```csharp
using CurlDotNet.Core;

var settings = new CurlSettings().WithFollowRedirects();
var result = await Curl.ExecuteAsync("curl https://httpbin.org/redirect/1", settings);
```

## Find out where you ended up

`-w '%{url_effective}'` appends the final URL as the last line of `result.Body`, the same as curl prints it:

```csharp
var result = await Curl.ExecuteAsync(
    "curl -L -w '%{url_effective}' https://httpbin.org/redirect/2");

var lines = result.Body.TrimEnd().Split('\n');
Console.WriteLine(lines[lines.Length - 1]);   // https://httpbin.org/get
```

## Gotchas

- Redirect responses are returned as results, not exceptions: a 302 without `-L` has `IsSuccess == false` and `StatusCode == 302`.
- 301, 302, 303, 307 and 308 are all treated as redirects.
- Redirects to a different host are followed too; check `%{url_effective}` if you need to know where the data came from.

## Related

- [Redirect errors](../exceptions/redirect-errors.html) - `CurlTooManyRedirectsException` in depth
- [curl download file in C#](beginner/download-file.html) - download links that redirect to a CDN
- [curl timeout in C#](timeouts.html) - a long redirect chain still has to finish inside `--max-time`
- [Cookbook index](index.html)
