---
title: "curl Follow Redirects in C# (curl -L) with CurlDotNet"
description: "Make curl follow redirects in C# with curl -L, --max-redirs, --post301/302/303 and --location-trusted using CurlDotNet, and catch CurlTooManyRedirectsException."
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

`--max-redirs N` allows exactly N redirects (the default is 50, and `-1` means unlimited). The next one throws `CurlTooManyRedirectsException`, curl's exit code 47, which carries the hop count:

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

`WithRedirectHandler` is called with the absolute URL of every redirect that is followed, before the next request is sent. Throw from it to refuse a target: the transfer stops with `CurlAbortedByCallbackException` (exit code 42), carrying your exception as `InnerException`.

```csharp
using CurlDotNet.Core;

var hops = new List<string>();
var tracked = new CurlSettings()
    .WithFollowRedirects()
    .WithRedirectHandler(url => hops.Add(url));
await Curl.ExecuteAsync("curl https://httpbin.org/redirect/2", tracked);
Console.WriteLine(string.Join(" -> ", hops));
```

## What happens to the method and body

CurlDotNet follows curl's rules, not a browser's:

| Status | What the next request is |
|--------|--------------------------|
| 301, 302 | A POST becomes a GET without a body, unless you pass `--post301` / `--post302`. Other methods are kept. |
| 303 | Every method except GET and HEAD becomes GET, unless it is a POST and you pass `--post303`. |
| 307, 308 | Method and body are sent again unchanged. |

```csharp
// The 307 keeps the POST and its body
var result = await Curl.ExecuteAsync(
    "curl -L -d 'a=1' 'https://httpbin.org/redirect-to?url=/post&status_code=307'");

Console.WriteLine(result.StatusCode);   // 200 from /post, which echoes form.a = 1
```

A method set with `-X` is used for every hop, which is curl's documented `-L` caveat (`-X POST -L` keeps sending POST even after a 302). Use `--follow` instead of `-L` when you want a 301, 302 or 303 to change a custom `-X` method to GET, as the HTTP specification describes.

## Credentials and cookies on redirects

The `Authorization` header (from `-u`, `-H`, `--oauth2-bearer` or `user:password@` in the URL) and custom `Cookie` headers are only sent to the host, port and scheme of the original request. When a redirect goes to a different host they are dropped, so a token cannot leak to a third party. `--location-trusted` sends them everywhere, as in curl, and implies `-L`:

```csharp
var result = await Curl.ExecuteAsync(
    "curl --location-trusted -u user:passwd https://example.com/login-redirect");
```

With a cookie engine (`-b FILE` or `-c JAR`, see [cookies](cookies.html)), cookies the server sets on one hop are sent on the next one that matches, as curl does.

`-L -e ';auto'` sets the `Referer` of every hop to the URL that redirected to it (no `Referer` on the first request); `-e 'https://start.example/;auto'` also sends that URL on the first request.

## See every hop's headers

`-L -i` prints the status line and headers of every response in the chain into `result.Body`, then the final body, the same as curl:

```csharp
var result = await Curl.ExecuteAsync("curl -L -i https://httpbin.org/redirect/2");
Console.WriteLine(result.Body);
// 302 status line and headers (Location: /relative-redirect/1)
// 302 status line and headers (Location: /get)
// 200 status line and headers, then the JSON body
```

`result.Headers` and `result.StatusCode` describe the final response.

## Find out where you ended up

`-w '%{url_effective}'` appends the final URL after the body in `result.Body`, the same as curl prints it (`%{num_redirects}` and `%{redirect_url}` work too; see [curl -w in C#](write-out.html)):

```csharp
var result = await Curl.ExecuteAsync(
    "curl -L -w '%{url_effective}' https://httpbin.org/redirect/2");

var lines = result.Body.TrimEnd().Split('\n');
Console.WriteLine(lines[lines.Length - 1]);   // https://httpbin.org/get
```

## Gotchas

- Redirect responses are returned as results, not exceptions: a 302 without `-L` has `IsSuccess == false` and `StatusCode == 302`.
- 301, 302, 303, 307 and 308 are all treated as redirects.
- Redirects to a different host are followed too, without your credentials; check `%{url_effective}` if you need to know where the data came from.

## Related

- [Redirect errors](../exceptions/redirect-errors.html) - `CurlTooManyRedirectsException` in depth
- [curl digest, NTLM and basic auth in C#](auth-schemes.html) - credentials and `--location-trusted`
- [curl exit codes](../reference/curl-exit-codes.html)
- [curl download file in C#](beginner/download-file.html) - download links that redirect to a CDN
- [curl timeout in C#](timeouts.html) - a long redirect chain still has to finish inside `--max-time`
- [Cookbook index](index.html)
