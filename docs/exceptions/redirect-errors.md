---
title: "curl Too Many Redirects in C#: -L and --max-redirs"
description: "Fix curl redirect errors in C#: follow redirects with -L or FollowRedirects(), cap them with --max-redirs, and catch CurlTooManyRedirectsException."
---

# curl Too Many Redirects in C# (-L and --max-redirs)

curl redirect errors in C# come in two forms: a 301/302 response that is returned unfollowed because `-L` was not given, and `CurlTooManyRedirectsException` when the chain exceeds `--max-redirs`. CurlDotNet follows the same rules as command-line curl, and this page shows how to enable redirects, cap them, and handle the exception.

## Redirects Are Not Followed by Default

Without `-L`, a redirect is just a response with a 3xx status and a `Location` header:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/redirect/1");

Console.WriteLine(result.StatusCode);              // 302
Console.WriteLine(result.IsSuccess);               // False (only 2xx counts)
Console.WriteLine(result.GetHeader("Location"));   // /get
```

## Following Redirects

### -L / --location

```csharp
var result = await Curl.ExecuteAsync("curl -L https://httpbin.org/redirect/3");
Console.WriteLine(result.StatusCode);   // 200, after three hops
```

### Builder: FollowRedirects()

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/redirect/3")
    .FollowRedirects()
    .ExecuteAsync();
```

### Process-Wide Default

```csharp
Curl.DefaultFollowRedirects = true;   // every command now behaves as if -L were given

var result = await Curl.ExecuteAsync("curl https://httpbin.org/redirect/1");
```

### CurlSettings

```csharp
var settings = new CurlSettings().WithFollowRedirects();
var result = await Curl.ExecuteAsync("curl https://httpbin.org/redirect/1", settings);
```

## Limiting Redirects

The default limit is 50 hops, matching curl. `--max-redirs N` allows exactly N redirects (`-1` means unlimited); when the server sends one more, `CurlTooManyRedirectsException` (exit code 47) is thrown with `RedirectCount` set and curl's message "Maximum (N) redirects followed":

```csharp
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -L --max-redirs 2 https://httpbin.org/redirect/5");
}
catch (CurlTooManyRedirectsException ex)
{
    Console.WriteLine($"Gave up after {ex.RedirectCount} redirects");   // 2
    Console.WriteLine(ex.CurlErrorCode);                                // 47
}
```

Builder equivalent: `.FollowRedirects().WithMaxRedirects(2)`.

## Watching the Chain

`-w '%{url_effective}'` appends the final URL to the body, and `-v` writes each hop's request and response header block to the start of `result.Body`:

```csharp
var result = await Curl.ExecuteAsync("curl -L -w '%{url_effective}' https://httpbin.org/redirect/3");
Console.WriteLine(result.Body);   // ... body of the final page, then https://httpbin.org/get
```

`-iL` includes every hop's response headers in the body as well, as curl does.

## Other Redirect Exceptions

| Exception | `CurlErrorCode` | Thrown when |
|-----------|----:|-------------|
| `CurlUnsupportedProtocolException` | 1 | With `-L`, the `Location` header points at a scheme other than `http`/`https` (for example `ftp://` or `file:///etc/passwd`); curl refuses to switch protocols. Fetch that URL explicitly if you trust it, or drop `-L` to see the redirect itself |
| `CurlMalformedUrlException` | 3 | The `Location` header is not a valid URL |
| `CurlRedirectException` | 47 | Reserved: exists in `CurlDotNet.Exceptions` but nothing throws it; catch `CurlTooManyRedirectsException` |

## Method and Credentials on Redirect

CurlDotNet follows curl's rules, which explain many "works in the browser, fails with -L" reports:

- 301 and 302 turn a POST into a GET (keep the POST with `--post301` / `--post302`); 303 turns everything except GET and HEAD into a GET (`--post303`); 307 and 308 keep the method and body.
- A method set with `-X` is kept on every hop (with `--follow` instead of `-L`, the 301/302/303 rules above apply to it too).
- `Authorization` (from `-u`, `--oauth2-bearer` or URL credentials) and `Cookie` headers are not sent to a different host unless you add `--location-trusted`, so an API that redirects to another host may answer 401 on the last hop.

## Common Causes of Redirect Loops

- **Login pages**: an unauthenticated request is redirected to `/login`, which redirects back. Fix the credentials (see [authentication errors](auth-errors.html)) rather than raising the limit.
- **HTTP to HTTPS and back**: a misconfigured server upgrades to HTTPS, then a proxy downgrades to HTTP. Request the HTTPS URL directly.
- **Trailing-slash loops**: `/path` redirects to `/path/` and back. Use the exact URL the API documents.
- **Cookies required**: some sites redirect until a cookie is set. Turn on the cookie engine with `-b ''` or `-c cookies.txt` so cookies set on one hop are sent on the next (see [cookie errors](cookie-errors.html)).

## Choosing a Limit

- Keep the default (50) for general use
- Use `--max-redirs 5` or lower when talking to an API you control, so loops fail fast
- Omit `-L` when you want to see the 3xx response itself; `-L --max-redirs 0` throws `CurlTooManyRedirectsException` on the first redirect instead

## Related Documentation

- [HTTP errors](http-errors.html)
- [Cookbook](../cookbook/index.html)
- [Timeout errors](timeout-errors.html)
- [Exception reference](index.html)
