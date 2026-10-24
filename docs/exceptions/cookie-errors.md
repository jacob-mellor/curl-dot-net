---
title: "curl Cookie Errors in C#: Cookies and Cookie Jars with CurlDotNet"
description: "Fix curl cookie errors in C#: send cookies with -b, read and write Netscape cookie jars with -b FILE and -c JAR, drop session cookies with -j, and handle exit code 23."
---

# curl Cookie Errors in C# (-b, -c and Cookie Jars)

curl cookie errors in C# are almost always about how the cookie was supplied or where the jar is saved. CurlDotNet handles cookies the way the curl tool does: `-b 'name=value'` sends a cookie string, `-b FILE` loads a Netscape cookie file, `-c JAR` writes every cookie the server set to a jar file, and `-j` drops session cookies when loading. The one cookie error that throws is a jar that cannot be written: `CurlWriteErrorException`, exit code 23. This page shows each option and the mistakes that cause trouble.

## Sending a Cookie String (-b name=value)

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -b 'session=abc123; theme=dark' https://httpbin.org/cookies");
Console.WriteLine(result.Body); // {"cookies": {"session": "abc123", "theme": "dark"}}
```

`--cookie 'name=value'` is the long form. Several `-b` strings are joined with `; `, so `-b 'a=1' -b 'b=2'` sends `Cookie: a=1; b=2`.

### Builder Equivalent

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/cookies")
    .WithCookie("session=abc123; theme=dark")
    .ExecuteAsync();
```

`WithCookie` follows the same rule as `-b`: a value without `=` is treated as a cookie file name. `WithCookieJar(path)` is the builder form of `-c`.

### Sending the Header Directly

A cookie is just a request header, so `-H` works too; it bypasses the cookie engine:

```csharp
var result = await Curl.ExecuteAsync("curl -H 'Cookie: session=abc123' https://httpbin.org/cookies");
```

## Cookie Jars (-b FILE and -c JAR)

A value without `=` is a file name. `-c JAR` (`--cookie-jar`) writes the cookies after the transfer in curl's Netscape format (`#HttpOnly_` prefixes, expiry capped at 400 days like curl), and `-b JAR` sends them on a later command, so cookies persist across separate `Curl.ExecuteAsync` calls:

```csharp
using CurlDotNet;

// 1. Log in and save the cookies the server sets
await Curl.ExecuteAsync("curl -c cookies.txt https://httpbin.org/cookies/set/session/abc123");

// 2. Send them back on the next call (and keep the jar up to date)
var result = await Curl.ExecuteAsync("curl -b cookies.txt -c cookies.txt https://httpbin.org/cookies");
Console.WriteLine(result.Body);   // {"cookies": {"session": "abc123"}}
```

Rules that match curl:

- The cookie engine runs only when the command has `-b FILE`, `-c JAR` or `-j`. With it on, cookies set during a transfer are sent on the following `-L` redirect hops and to later URLs of the same command; without it, a `Set-Cookie` on a redirect is not sent back. `-b ''` turns the engine on without loading a file.
- Domain, path, `Secure`, `__Secure-`/`__Host-` prefixes and `Max-Age`/`Expires` are applied as curl applies them, so a cookie is only sent where the server allowed it.
- `-c -` writes the jar to standard output, which in CurlDotNet means it is appended to `result.Body` after the response body.
- `-j` (`--junk-session-cookies`) discards session cookies (those without an expiry) when loading `-b FILE`, as if a new browser session started.
- `-b FILE` and `-b 'name=value'` can be combined; the file's cookies come first.

## Errors

### CurlWriteErrorException (23): the jar cannot be saved

Thrown after the transfer when the `-c` file cannot be written: its directory does not exist, the path is read-only, or permission is denied. `FilePath` is the jar path, and the message says what failed.

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl -c missing-dir/cookies.txt https://httpbin.org/cookies/set/session/abc123");
}
catch (CurlWriteErrorException ex)
{
    Console.WriteLine($"Could not save the cookie jar {ex.FilePath}: {ex.Message}");
}
```

**Fix:** point `-c` at a writable file in a directory that exists (create it first; `--create-dirs` applies to `-o` output files only).

### A missing -b file is not an error

`-b cookies.txt` with no such file silently starts with an empty jar, exactly like curl, so no `Cookie` header is sent. If cookies seem to vanish, check that the path is relative to the process working directory (`Directory.GetCurrentDirectory()`), not to your source file. No read error (exit code 26) is raised for cookie files.

### CurlCookieException is reserved

`CurlCookieException` (no exit code, property `CookieJarPath`) exists in `CurlDotNet.Exceptions`, but nothing in the library throws it today. Catch `CurlWriteErrorException` for jar problems.

## Common Mistakes

### Passing a File Name When You Meant a Cookie

A `-b` value without `=` is a file name. `-b session` loads a file called `session` (and finds nothing); write `-b 'session=abc123'` to send a cookie.

### Spaces Without Quotes

Multiple cookies are separated by `; `, which includes a space, so quote the whole value:

```csharp
var result = await Curl.ExecuteAsync("curl -b 'a=1; b=2' https://httpbin.org/cookies");
```

### Expecting Cookies to Persist Without a Jar

Each `Curl.ExecuteAsync` call starts with no cookies. To carry cookies from one call to the next, write them with `-c JAR` and read them with `-b JAR` as shown above, or pass the value explicitly with `-b 'name=value'`.

## Debugging Cookies

`-v` writes the outgoing request headers, including `Cookie:`, at the start of `result.Body`:

```csharp
var result = await Curl.ExecuteAsync("curl -v -b 'session=abc123' https://httpbin.org/cookies");
Console.WriteLine(result.Body);
```

`https://httpbin.org/cookies` echoes back the cookies it received, which makes it a convenient test endpoint.

## Related Documentation

- [Tutorial 8: Headers Explained](../tutorials/08-headers-explained.html) (includes sending cookies)
- [Cookbook: curl with cookies](../cookbook/cookies.html)
- [Transfer and file errors](transfer-errors.html) (exit code 23 for every unwritable file)
- [Authentication errors](auth-errors.html)
- [Exception reference](index.html)
