---
title: "curl Malformed URL Errors in C#: CurlMalformedUrlException"
description: "Fix curl URL and command errors in C#: CurlMalformedUrlException (3), CurlUnsupportedProtocolException (1), CurlInvalidCommandException and option errors (2, 43, 49)."
---

# curl Malformed URL Errors in C# (CurlMalformedUrlException)

curl URL errors in C# fall into three cases, each with its own exception in CurlDotNet: a URL that cannot be parsed throws `CurlMalformedUrlException` (curl error code 3), a scheme CurlDotNet does not handle throws `CurlUnsupportedProtocolException` (code 1), and a command with no URL at all throws `CurlInvalidCommandException`. Mistakes in the options themselves (a missing value, two request methods, a line break in a header) throw before anything is sent, with the exit code curl uses. This page shows what triggers each one and how to validate a command before running it.

## The Three Exceptions

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl htp://example.com/path");
}
catch (CurlMalformedUrlException ex)      { Console.WriteLine($"Bad URL: {ex.Message}"); }
catch (CurlUnsupportedProtocolException ex) { Console.WriteLine(ex.Message); }   // htp:// is not a scheme CurlDotNet handles
catch (CurlInvalidCommandException ex)    { Console.WriteLine($"Bad command near '{ex.InvalidPart}'"); }
```

| Input | Exception | Why |
|-------|-----------|-----|
| `curl "https://exa mple.com"` | `CurlMalformedUrlException` (3) | Space inside the host name |
| `curl https://example.com:abc` | `CurlMalformedUrlException` (3) | Port is not a number |
| `curl gopher://example.com` | `CurlUnsupportedProtocolException` (1) | Scheme has no handler |
| `curl htp://example.com` | `CurlUnsupportedProtocolException` (1) | Typo in the scheme |
| `curl -X GET` | `CurlInvalidCommandException` (no code) | Options but no URL (`InvalidPart` is `"URL"`) |
| `curl` | `CurlException` (3) | Nothing after the program name |
| `curl 'https://example.com/[1-'` | `CurlException` (3) | Malformed URL glob |

Like command-line curl, CurlDotNet prepends `http://` to a URL without a scheme (`ftp://` when the host starts with `ftp.`), so `curl example.com` works. Write `https://` explicitly when you want TLS. `CurlMalformedUrlException` is also thrown when a redirect's `Location` header is not a valid URL, and `CurlUnsupportedProtocolException` when a `-L` redirect points at a scheme other than `http` or `https`.

## Command and Option Errors

These are thrown while the command is parsed, before any network activity:

| Problem | Exception | `CurlErrorCode` |
|---------|-----------|----:|
| An option is the last word and has no value (`curl https://example.com -H`) | `CurlException` | 2 |
| Two request methods: `-I` with `-d`, `-F` with `-d`, `--json` with `-F`... | `CurlException` | 2 |
| `--max-filesize abc`, `-C` with `--no-clobber`, `-K` files nested too deep, `--variable` errors | `CurlException` | 2 |
| `-o` uses `#<name>` for a glob the URL does not name | `CurlException` | 43 |
| CR or LF inside a `-H` value or `--request-target`, space in `--request-target` | `CurlBadFunctionArgumentException` (property `ArgumentName`) | 43 |
| `-C`/`--continue-at` with a request body (`-d`, `-F`, `-T`) | `CurlBadFunctionArgumentException` | 43 |
| `--resolve` / `--connect-to` entry with a non-numeric port | `CurlOptionSyntaxException` (a `CurlInvalidCommandException`) | 49 |
| Options but no URL | `CurlInvalidCommandException` | none |

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl -I -d 'a=1' https://httpbin.org/post");
}
catch (CurlException ex) when (ex.CurlErrorCode == 2)
{
    Console.WriteLine(ex.Message);   // You can only select one HTTP request method, ...
}
```

A header with a line break is refused because the rest of the line would be sent as extra headers or a second request:

```csharp
try
{
    var userInput = "abc\r\nX-Admin: true";
    await Curl.ExecuteAsync($"curl -H 'X-Token: {userInput}' https://httpbin.org/headers");
}
catch (CurlBadFunctionArgumentException ex)
{
    Console.WriteLine($"{ex.ArgumentName}: {ex.Message}");   // -H/--header: The value given with -H/--header contains a line break ...
}
```

Unknown options (typos such as `--hedaer`) do not throw: the command runs and the typo is listed in `CurlOptions.UnknownOptions`. `Curl.Validate` reports it, so validate commands that come from users or configuration. `CurlUnknownOptionException`, `CurlNotSupportedException` (code 1) and `CurlNotBuiltInException` (code 4) exist in `CurlDotNet.Exceptions` but nothing throws them today.

## Validating Before Executing

`Curl.Validate` parses the command without sending anything, and reports a missing URL, unknown options and unsupported schemes:

```csharp
var check = Curl.Validate("curl -X POST https://httpbin.org/post -d 'a=1'");

if (!check.IsValid)
    Console.WriteLine($"Invalid command: {check.Error}");
```

For a URL on its own, .NET's `Uri.TryCreate` tells you whether it is absolute and which scheme it uses:

```csharp
bool IsHttpUrl(string url) =>
    Uri.TryCreate(url, UriKind.Absolute, out var uri)
    && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
```

## Common Mistakes

### Missing Scheme

```csharp
// Works, but over plain HTTP: curl (and CurlDotNet) prepend http://
var r1 = await Curl.ExecuteAsync("curl httpbin.org/get");

// Say https:// when you want TLS
var r2 = await Curl.ExecuteAsync("curl https://httpbin.org/get");
```

### Unencoded Characters

Spaces and most punctuation must be percent-encoded in the path and query:

```csharp
var fileName = "my report.pdf";
var url = $"https://httpbin.org/anything/{Uri.EscapeDataString(fileName)}";   // my%20report.pdf
var result = await Curl.ExecuteAsync($"curl {url}");
```

### Query Strings Built by Hand

Encode each value with `Uri.EscapeDataString`, or let curl do it with `--data-urlencode` (which, like curl, encodes spaces as `+`) or `-G --data-urlencode` for a query string:

```csharp
var q = Uri.EscapeDataString("c# & .net");
var r1 = await Curl.ExecuteAsync($"curl 'https://httpbin.org/get?q={q}'");

// POST form field, encoded by curl
var r2 = await Curl.ExecuteAsync("curl --data-urlencode 'q=c# & .net' https://httpbin.org/post");
```

### Quoting URLs with Special Characters

`&`, `?` and spaces are safe inside the C# string, but wrap the URL in single quotes when it contains spaces or characters the command parser could split on:

```csharp
var result = await Curl.ExecuteAsync("curl 'https://httpbin.org/get?a=1&b=2'");
```

## Building URLs Safely

```csharp
var builder = new UriBuilder("https", "httpbin.org")
{
    Path = "/anything/users",
    Query = $"name={Uri.EscapeDataString("Alice Smith")}&page=2"
};

var result = await Curl.ExecuteAsync($"curl '{builder.Uri}'");
```

## Supported Schemes

CurlDotNet handles `http://`, `https://`, `ftp://` and `file://`. Anything else (`sftp://`, `scp://`, `smtp://`, `imap://`, `ldap://`, `telnet://` and so on) raises `CurlUnsupportedProtocolException`. See [supported protocols](../supported-protocols.html).

## Related Documentation

- [Supported protocols](../supported-protocols.html)
- [DNS and connection errors](dns-errors.html)
- [curl CLI compatibility](../reference/curl-cli-compatibility.html)
- [RFC 3986: URI syntax](https://www.rfc-editor.org/rfc/rfc3986)
- [Exception reference](index.html)
