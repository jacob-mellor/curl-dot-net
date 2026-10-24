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

An HTTP error status such as 404 does not throw; check `IsSuccess` or call `EnsureSuccess()`. Add `-f` / `--fail` to have it throw `CurlHttpReturnedErrorException` (exit code 22) instead. Network failures (DNS, refused connection, timeout) throw a `CurlException` subclass whose `CurlErrorCode` is curl's exit code. See [error handling](../curl-error-handling-csharp/README.md) and the [exit code reference](../../reference/curl-exit-codes.md).

## Check a command without sending it

```csharp
using CurlDotNet;

var check = Curl.Validate("curl -X POST");
if (!check.IsValid)
    Console.WriteLine(check.Error);  // No URL specified in command.
```

## Which curl flags work

The parser knows curl 8.7.1's complete option table, including combined short flags (`-sSL`), attached values (`-m10`) and `--long=value`. The flags you meet in API docs all work: `-X`, `-H`, `-d` and its variants (`--data-binary`, `--data-raw`, `--data-urlencode`, `--json`), `-G`, `-F`, `-T`, `-u` with `--basic`, `--digest`, `--ntlm`, `--negotiate` and `--anyauth`, `-A`, `-e`, `-b` and `-c` (cookie strings and cookie jars), `-L`, `--max-redirs`, `-k`, `-x`, `--proxy-user`, `--socks5`, `-m`, `--connect-timeout`, `--retry`, `-o`, `-O`, `-D`, `-I`, `-i`, `-v`, `--trace`, `--compressed`, `-r`, `-w`, `--http2` and `--aws-sigv4`. Several URLs in one command run one after another, URL globs such as `[1-3]` expand, and `-K` reads a config file; see [multiple URLs and globbing](../../cookbook/multiple-urls-and-globbing.md) and [config files and variables](../../cookbook/config-files-and-variables.md).

Options that are recognised but not implemented are listed in `ParsedOptions.UnsupportedOptions`, and typos land in `ParsedOptions.UnknownOptions` (which makes `Validate` report the command as invalid). Still not applied: client certificates and custom CAs (`--cert`, `--key`, `--cacert`, `--capath`, `--pinnedpubkey`), protocols other than HTTP, HTTPS, FTP and FILE, and `--parallel` (transfers run one after another). `~/.curlrc` is never read implicitly; pass it with `-K`.

Like the curl CLI, CurlDotNet sends `User-Agent: curl/8.7.1` and `Accept: */*` by default, so APIs that require a User-Agent (GitHub's, for example) work as pasted. Use `-A` to send your own.

## Related

- [Run curl synchronously in C#](../curl-synchronously-in-csharp/README.md)
- [Convert curl to C# code](../convert-curl-to-csharp/README.md)
- [POST JSON with curl in C#](../curl-post-json-csharp/README.md)
- [Parse JSON from a curl response](../parse-json-from-curl-in-csharp/README.md)
- [How CurlDotNet works](../how-curldotnet-works/README.md)
- [All how-to guides](../README.md)
