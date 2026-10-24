---
title: "curl with Cookies in C# (curl -b) using CurlDotNet"
description: "Send cookies with curl in C# using curl -b 'name=value' and CurlRequestBuilder.WithCookie, read Set-Cookie headers and keep a session with CurlDotNet."
---

# curl with cookies in C# (curl -b)

To send cookies with curl in C#, pass `-b 'name=value'` in the command string you give to `Curl.ExecuteAsync`. The value is sent verbatim as the `Cookie` request header, so several cookies are separated with `; ` just like on the command line.

## The curl command

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -b 'session=abc123; theme=dark' https://httpbin.org/cookies");

Console.WriteLine(result.Body);   // {"cookies": {"session": "abc123", "theme": "dark"}}
```

`-H 'Cookie: session=abc123'` does exactly the same thing if you prefer a plain header.

## Builder style

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Get("https://httpbin.org/cookies")
    .WithCookie("session=abc123; theme=dark")
    .ExecuteAsync();

Console.WriteLine(result.Body);
```

## Read cookies the server sets

Response cookies arrive in the `Set-Cookie` header. `result.Headers` is case-insensitive, so `GetHeader` finds it whatever the server's casing:

```csharp
var result = await Curl.ExecuteAsync(
    "curl https://httpbin.org/cookies/set?session=abc123");

Console.WriteLine(result.StatusCode);              // 302 (httpbin redirects after setting)
Console.WriteLine(result.GetHeader("Set-Cookie")); // session=abc123; Path=/
```

Add `-L` if you want to follow that redirect and see the cookie echoed back; see [follow redirects](follow-redirects.html).

## Keep a simple session

Log in, take the cookie from `Set-Cookie`, and pass it back with `-b` on later requests:

```csharp
var login = await Curl.ExecuteAsync(
    "curl -d 'user=demo&pass=secret' https://httpbin.org/cookies/set/session/abc123");

var cookie = login.GetHeader("Set-Cookie")?.Split(';')[0];   // "session=abc123"

var page = await Curl.ExecuteAsync($"curl -b '{cookie}' https://httpbin.org/cookies");
Console.WriteLine(page.Body);
```

For a reusable client, keep the cookie string in a field and build each command from it.

## What about cookie jars (-c and -b file)?

curl's cookie jar flags, `-c cookies.txt` to save and `-b cookies.txt` to load a Netscape-format file, are not implemented yet; the value you give `-b` is always sent as a literal cookie string. Cookie jar support is coming. Until then, store the `Set-Cookie` value yourself as shown above.

## Gotchas

- Cookie values with spaces or semicolons need quoting inside the command string, exactly as in a shell: `-b 'a=1; b=two words'`.
- `-b` replaces the whole `Cookie` header; to add a cookie to an existing set, concatenate the strings yourself.
- Cookies are not stored between calls. Each `Curl.ExecuteAsync` is independent.

## Related

- [Cookie errors](../exceptions/cookie-errors.html)
- [curl -H HTTP headers in C#](../tutorials/08-headers-explained.html)
- [curl digest, NTLM and basic auth in C#](auth-schemes.html) - when the server wants credentials instead of a cookie
- [Cookbook index](index.html)
