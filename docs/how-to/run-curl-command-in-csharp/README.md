---
title: "How to Run a curl Command in C# with CurlDotNet"
description: "How to run a curl command in C#: pass the curl string to Curl.ExecuteAsync and read status, headers and body. Pure .NET, no curl.exe or Process.Start."
---

# How to run a curl command in C#

To run a curl command in C#, install the `CurlDotNet` NuGet package and pass the command string to `Curl.ExecuteAsync`; it parses the curl flags and sends the request through .NET's `HttpClient`, so there is no curl.exe, no native libcurl and no `Process.Start`.

```bash
dotnet add package CurlDotNet
```

## Run the curl string as-is

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -H 'Accept: application/json' https://httpbin.org/get");

Console.WriteLine(result.StatusCode);  // 200
Console.WriteLine(result.Body);        // the JSON response text
```

The leading `curl ` is optional: `Curl.ExecuteAsync("https://httpbin.org/get")` sends the same request.

## Paste multi-line commands from API docs

Commands in API documentation usually span several lines. Put them in a C# 11 raw string literal and they run unchanged. The parser joins lines ending in `\` (bash), `` ` `` (PowerShell) or `^` (cmd.exe).

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("""
    curl -X POST https://httpbin.org/post \
      -H "Content-Type: application/json" \
      -d '{"name":"Ada","role":"admin"}'
    """);

Console.WriteLine(result.StatusCode);
```

On older C# versions, use a verbatim string (`@"..."`) and double every embedded `"`.

## The same request with the fluent builder

If you are writing the request yourself rather than pasting one, `CurlRequestBuilder` gives you IntelliSense and compile-time checking:

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Post("https://httpbin.org/post")
    .WithJson(new { name = "Ada", role = "admin" })
    .ExecuteAsync();

Console.WriteLine(result.StatusCode);
```

Call `.ToCurlCommand()` on a builder to see the equivalent curl string.

## What comes back

Every call returns a `CurlDotNet.Core.CurlResult`:

| Member | What it holds |
|---|---|
| `StatusCode` | HTTP status as an `int` |
| `IsSuccess` | `true` for 200-299 |
| `Body` | Response text (for text content types) |
| `Headers` | Response headers, case-insensitive dictionary |
| `IsBinary` / `BinaryData` | Raw bytes for images, PDFs, archives and other binary responses |
| `OutputFiles` | Paths written by `-o` / `-O` |
| `Timings.Total` | Total time in milliseconds |

An HTTP error status such as 404 does not throw; check `IsSuccess` or call `EnsureSuccess()`. Network failures (DNS, refused connection, timeout) throw a `CurlException` subclass. See [error handling](../curl-error-handling-csharp/README.md).

## Check a command without sending it

```csharp
using CurlDotNet;

var check = Curl.Validate("curl -X POST");
if (!check.IsValid)
    Console.WriteLine(check.Error);  // No URL specified in command.
```

## Which curl flags work

`-X`, `-H`, `-d`, `--data-binary`, `--data-raw`, `--data-urlencode`, `--json`, `-F`, `-u`, `--basic`, `--digest`, `--ntlm`, `--negotiate`, `--anyauth`, `-A`, `-e`, `-b` (cookie string), `-L`, `--max-redirs`, `-k`, `-x`, `--proxy-user`, `--socks5`, `-m` / `--max-time`, `-o`, `-O`, `-I`, `-i`, `-v`, `--trace`, `--compressed`, `-r`, `-w` and `--aws-sigv4`.

Not supported today: `--retry`, `-c` cookie jars, client certificates (`--cert`, `--key`, `--cacert`), `--http2`, `-G`, `-T` and `-D`.

One difference from the curl CLI: CurlDotNet does not send a `User-Agent` header unless you pass `-A`. Some APIs (GitHub's, for example) reject requests without one.

## Related

- [Run curl synchronously in C#](../curl-synchronously-in-csharp/README.md)
- [Convert curl to C# code](../convert-curl-to-csharp/README.md)
- [POST JSON with curl in C#](../curl-post-json-csharp/README.md)
- [Parse JSON from a curl response](../parse-json-from-curl-in-csharp/README.md)
- [How CurlDotNet works](../how-curldotnet-works/README.md)
- [All how-to guides](../README.md)
