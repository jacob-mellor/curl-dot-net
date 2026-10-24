---
title: "How to Follow Redirects with curl -L in C#"
description: "Follow redirects in C# like curl -L: CurlDotNet does not follow 3xx by default; add -L or FollowRedirects(), and cap hops with --max-redirs."
---

# How to follow redirects with curl -L in C#

To follow redirects in C# the way `curl -L` does, add `-L` to the command you pass to `Curl.ExecuteAsync`, or call `.FollowRedirects()` on `CurlRequestBuilder`. Like the curl CLI, CurlDotNet does not follow redirects unless you ask.

```bash
dotnet add package CurlDotNet
```

## Without -L you get the redirect itself

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/redirect/2");

Console.WriteLine(result.StatusCode);             // 302
Console.WriteLine(result.GetHeader("Location"));  // /relative-redirect/1
```

That is useful when you want to inspect where a link points without fetching the target.

## With -L

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -L https://httpbin.org/redirect/2");
Console.WriteLine(result.StatusCode);  // 200, after two hops
```

## The fluent builder

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/redirect/3")
    .FollowRedirects()
    .WithMaxRedirects(5)
    .ExecuteAsync();

Console.WriteLine(result.StatusCode);
```

## Limit the number of hops

`--max-redirs` caps the chain. Exceeding it throws `CurlTooManyRedirectsException` (curl error code 47):

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl -L --max-redirs 1 https://httpbin.org/redirect/3");
}
catch (CurlTooManyRedirectsException ex)
{
    Console.WriteLine($"{ex.CurlErrorCode}: {ex.Message}");  // 47: Too many redirects: 1
}
```

## Turn it on for many calls

```csharp
using CurlDotNet;
using CurlDotNet.Core;

var settings = new CurlSettings().WithFollowRedirects(true);
var result = await Curl.ExecuteAsync("https://httpbin.org/redirect/1", settings);

Curl.DefaultFollowRedirects = true;  // read by CurlSettings.FromDefaults()
var viaDefaults = await Curl.ExecuteAsync(
    "https://httpbin.org/redirect/1", CurlSettings.FromDefaults());
```

In 9.7.0 `Curl.DefaultFollowRedirects` is only read by `CurlSettings.FromDefaults()`. Setting it does not change `Curl.ExecuteAsync(command)` calls that pass no settings. `LibCurl` has `WithFollowRedirects(maxRedirects)` for an instance-wide default.

## How redirects are handled

The shared `HttpClient` is created with `AllowAutoRedirect = false`. CurlDotNet's own redirect handler follows each `Location` header, so the curl semantics (opt-in `-L`, `--max-redirs`) apply instead of .NET's defaults.

## Related

- [Download a file with curl -o](../curl-download-file-csharp/README.md)
- [HEAD requests](../curl-head-request-csharp/README.md)
- [Error handling](../curl-error-handling-csharp/README.md)
- [How CurlDotNet works](../how-curldotnet-works/README.md)
- [All how-to guides](../README.md)
