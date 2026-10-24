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

`--max-redirs N` allows exactly N hops (`-1` means unlimited; the default is 50). One more redirect after that throws `CurlTooManyRedirectsException` (curl error code 47):

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl -L --max-redirs 1 https://httpbin.org/redirect/3");
}
catch (CurlTooManyRedirectsException ex)
{
    Console.WriteLine($"{ex.CurlErrorCode}: {ex.Message}");  // 47: Maximum (1) redirects followed: ...
}
```

## Turn it on for many calls

```csharp
using CurlDotNet;
using CurlDotNet.Core;

var settings = new CurlSettings().WithFollowRedirects(true);
var result = await Curl.ExecuteAsync("https://httpbin.org/redirect/1", settings);

Curl.DefaultFollowRedirects = true;  // every request in the process now follows redirects
var viaDefaults = await Curl.ExecuteAsync("https://httpbin.org/redirect/1");
```

`Curl.DefaultFollowRedirects` applies to every request, including plain `Curl.ExecuteAsync(command)` calls with no settings. `LibCurl` has `WithFollowRedirects(maxRedirects)` for an instance-wide default.

## How redirects are handled

The shared `HttpClient` is created with `AllowAutoRedirect = false`. CurlDotNet's own redirect handler follows each `Location` header, so curl's rules apply instead of .NET's defaults:

- 301 and 302 turn a POST into a GET (unless `--post301` / `--post302`); 303 turns anything except GET and HEAD into a GET (unless `--post303`); 307 and 308 keep the method and the body.
- A method set with `-X` is kept on every hop, as in curl.
- `Authorization` and `Cookie` headers are dropped when the redirect goes to a different host, unless `--location-trusted` (which also implies `-L`).
- Cookies the server sets along the way are sent on later hops when a cookie engine is active (`-b FILE`, `-c JAR` or `-j`).
- `-L -i` puts every hop's headers into `result.Body`, and `-w '%{num_redirects} %{url_effective}'` reports the chain; see [write-out variables](../../cookbook/write-out.md).

## Related

- [Download a file with curl -o](../curl-download-file-csharp/README.md)
- [Follow redirects cookbook](../../cookbook/follow-redirects.md)
- [HEAD requests](../curl-head-request-csharp/README.md)
- [Error handling](../curl-error-handling-csharp/README.md)
- [How CurlDotNet works](../how-curldotnet-works/README.md)
- [All how-to guides](../README.md)
