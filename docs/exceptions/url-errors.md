---
title: "curl Malformed URL Errors in C#: CurlMalformedUrlException"
description: "Fix curl URL errors in C#: CurlMalformedUrlException for bad URLs, CurlUnsupportedProtocolException for unknown schemes, CurlInvalidCommandException."
---

# curl Malformed URL Errors in C# (CurlMalformedUrlException)

curl URL errors in C# fall into three cases, each with its own exception in CurlDotNet: a URL that cannot be parsed throws `CurlMalformedUrlException` (curl error code 3), a scheme CurlDotNet does not handle throws `CurlUnsupportedProtocolException` (code 1), and a command with no URL at all throws `CurlInvalidCommandException`. This page shows what triggers each one and how to validate a command before running it.

## The Three Exceptions

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl htp://example.com/path");
}
catch (CurlMalformedUrlException ex)      { Console.WriteLine($"Bad URL: {ex.Message}"); }
catch (CurlUnsupportedProtocolException ex) { Console.WriteLine($"No handler for {ex.Protocol}://"); }
catch (CurlInvalidCommandException ex)    { Console.WriteLine($"Bad command near '{ex.InvalidPart}'"); }
```

| Input | Exception | Why |
|-------|-----------|-----|
| `curl example.com` | `CurlMalformedUrlException` | No scheme, so .NET cannot parse it as an absolute URI |
| `curl "https://exa mple.com"` | `CurlMalformedUrlException` | Space inside the host name |
| `curl https://example.com:abc` | `CurlMalformedUrlException` | Port is not a number |
| `curl gopher://example.com` | `CurlUnsupportedProtocolException` | Scheme has no handler |
| `curl -X GET` | `CurlInvalidCommandException` | No URL in the command (`InvalidPart` is `"URL"`) |

Unlike command-line curl, CurlDotNet does not guess `http://` when the scheme is missing; always include `https://` or `http://`.

## Validating Before Executing

`Curl.Validate` parses the command without sending anything:

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
// Wrong - throws CurlMalformedUrlException
var r1 = await Curl.ExecuteAsync("curl httpbin.org/get");

// Right
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

Encode each value with `Uri.EscapeDataString`, or let curl do it with `--data-urlencode`:

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

CurlDotNet handles `http://` and `https://` (and FTP via its own handler). Anything else raises `CurlUnsupportedProtocolException`. See [supported protocols](../supported-protocols.html).

## Related Documentation

- [Supported protocols](../supported-protocols.html)
- [DNS and connection errors](dns-errors.html)
- [curl CLI compatibility](../reference/curl-cli-compatibility.html)
- [RFC 3986: URI syntax](https://www.rfc-editor.org/rfc/rfc3986)
- [Exception reference](index.html)
