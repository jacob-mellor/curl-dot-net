---
title: "curl Cookie Errors in C#: Sending Cookies with CurlDotNet"
description: "Fix curl cookie errors in C#: send cookies with -b 'name=value' or WithCookie in CurlDotNet, read Set-Cookie, and see what cookie-jar support is coming."
---

# curl Cookie Errors in C# (-b and WithCookie)

curl cookie errors in C# are almost always about how the cookie was supplied. CurlDotNet currently supports sending cookies as a string with `-b 'name=value; other=value'` (or the builder's `WithCookie("a=1")`) and reading `Set-Cookie` from the response. Cookie jar files (`-c`, and `-b` with a file name) are not implemented yet, so a page that relies on them will fail; this page shows what works today.

## What Works Today

### Sending a Cookie String (-b)

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -b 'session=abc123; theme=dark' https://httpbin.org/cookies");
Console.WriteLine(result.Body); // {"cookies": {"session": "abc123", "theme": "dark"}}
```

`--cookie 'name=value'` is the long form of the same option.

### Builder Equivalent

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/cookies")
    .WithCookie("session=abc123; theme=dark")
    .ExecuteAsync();
```

### Sending the Header Directly

A cookie is just a request header, so `-H` works too:

```csharp
var result = await Curl.ExecuteAsync("curl -H 'Cookie: session=abc123' https://httpbin.org/cookies");
```

### Reading Set-Cookie from a Response

```csharp
var login = await Curl.ExecuteAsync("curl https://httpbin.org/cookies/set/session/abc123");
var setCookie = login.GetHeader("Set-Cookie");   // null if the server sent none
Console.WriteLine(setCookie);
```

To carry a cookie into the next request, take the `name=value` part of `Set-Cookie` and pass it with `-b`:

```csharp
var pair = setCookie?.Split(';')[0];   // "session=abc123"
var next = await Curl.ExecuteAsync($"curl -b '{pair}' https://httpbin.org/cookies");
```

## Not Yet Supported: Cookie Jars

The following curl features are not implemented in the current release, so do not rely on them:

- `-c <file>` / `--cookie-jar <file>` (write cookies to a file)
- `-b <file>` where the value is a file name instead of `name=value`
- Automatic cookie persistence across separate `Curl.ExecuteAsync` calls

Cookie jar support is planned. Until then, forward cookies manually as shown above, or keep them in your own `CookieContainer` and pass the string with `-b`.

## Common Mistakes

### Passing a File Name to -b

CurlDotNet sends the `-b` value verbatim as the `Cookie` request header. Command-line curl treats a value without `=` as a cookie file, but CurlDotNet does not read files here yet, so a file name would be sent as a meaningless cookie header. Always pass `name=value` pairs:

```csharp
// Wrong - "cookies.txt" is sent literally as the Cookie header
var r1 = await Curl.ExecuteAsync("curl -b cookies.txt https://httpbin.org/cookies");

// Right - name=value
var r2 = await Curl.ExecuteAsync("curl -b 'session=abc123' https://httpbin.org/cookies");
```

### Spaces Without Quotes

Multiple cookies are separated by `; `, which includes a space, so quote the whole value:

```csharp
var result = await Curl.ExecuteAsync("curl -b 'a=1; b=2' https://httpbin.org/cookies");
```

### Expecting Cookies to Persist

Each `Curl.ExecuteAsync` call is independent. If the second request needs the first request's cookie, pass it explicitly with `-b`.

## Debugging Cookies

`-v` writes the outgoing request headers, including `Cookie:`, at the start of `result.Body`:

```csharp
var result = await Curl.ExecuteAsync("curl -v -b 'session=abc123' https://httpbin.org/cookies");
Console.WriteLine(result.Body);
```

`https://httpbin.org/cookies` echoes back the cookies it received, which makes it a convenient test endpoint.

## Related Documentation

- [Tutorial 8: Headers Explained](../tutorials/08-headers-explained.html) (includes sending cookies)
- [Cookbook](../cookbook/index.html)
- [Authentication errors](auth-errors.html)
- [Exception reference](index.html)
