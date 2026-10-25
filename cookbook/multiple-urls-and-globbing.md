---
title: "curl Multiple URLs and URL Globbing in C# ({a,b}, [1-10], --next)"
description: "Run curl commands with several URLs, URL globs such as {a,b} and [1-10], and --next groups in C# with CurlDotNet: output pairing with -o and #1, results, errors and -g."
---

# curl multiple URLs and URL globbing in C#

A curl command can name several URLs, expand URL globs such as `{one,two}` and `[1-10]`, and split options into groups with `--next`. CurlDotNet runs all of these transfers in C# exactly as curl does: one after another, in command-line order, with each `-o` paired to the next URL.

## Several URLs in one command

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/uuid https://httpbin.org/ip");
Console.WriteLine(result.Body);   // both bodies, one after the other, as curl prints them
```

The combined `CurlResult` works like curl's terminal output:

- `Body` is the concatenation of what every transfer printed (body, `-i` headers, `-w` text).
- `StatusCode`, `Headers` and `BinaryData` come from the last transfer.
- `OutputFiles` lists the files written by every transfer.

## Pair -o and -O with URLs

Each `-o` or `-O` belongs to the next URL that does not have one yet, left to right:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -o uuid.json https://httpbin.org/uuid -o ip.json https://httpbin.org/ip");

foreach (var file in result.OutputFiles)
    Console.WriteLine(file);   // uuid.json, ip.json
```

`--remote-name-all` gives every URL an implicit `-O`. Uploads pair the same way: `curl -T a.txt URL1 -T b.txt URL2`.

## URL globbing

curl's glob syntax expands one URL into many. CurlDotNet implements the same rules (a port of curl's `tool_urlglob.c`):

| Glob | Expands to |
|------|-----------|
| `{one,two,three}` | each listed value |
| `[1-5]` | 1, 2, 3, 4, 5 |
| `[01-10]` | 01, 02 ... 10 (zero padded to the width of the start value) |
| `[a-e]` | a, b, c, d, e |
| `[0-100:25]` | 0, 25, 50, 75, 100 (step after the colon) |

Several globs combine, the first one varying slowest. In the `-o` file name, `#1`, `#2` ... are replaced by the text each glob matched:

```csharp
using CurlDotNet;

// Fetches /anything/a/1 ... /anything/b/2 and saves a-1.json, a-2.json, b-1.json, b-2.json
var result = await Curl.ExecuteAsync(
    "curl 'https://httpbin.org/anything/{a,b}/[1-2]' -o '#1-#2.json'");
Console.WriteLine(result.OutputFiles.Count);   // 4
```

Quote URLs that contain globs with single quotes, as you would in a shell. To send `{`, `}`, `[` or `]` literally, either escape them with a backslash (`\[`) or switch globbing off with `-g` / `--globoff`. A bracketed IPv6 literal such as `http://[::1]:8080/` is recognised and left alone.

A malformed glob (`[1-]`, `{a,b`, a range whose end is before its start) throws a `CurlException` with `CurlErrorCode` 3, curl's "bad URL" exit code, before anything is sent.

## Groups with --next

`--next` (or `-:`) starts a new group of options. Options before it apply only to the URLs before it:

```csharp
using CurlDotNet;

// A GET with a custom header, then a POST without it
var result = await Curl.ExecuteAsync(
    "curl -H 'X-Trace: 1' https://httpbin.org/get --next -d name=Ada https://httpbin.org/post");
Console.WriteLine(result.StatusCode);   // status of the POST, the last transfer
```

Inside a group, options apply to every URL of the group, wherever they appear on the line, as in curl.

## When a transfer fails

As in curl, a failure does not stop the remaining transfers, and the outcome is that of the last transfer that ran:

- If the last transfer succeeds, you get the combined result, even when an earlier one failed.
- If the last transfer fails, its exception is thrown. `ex.StandardOutput` holds everything the earlier transfers printed.
- `--fail-early` stops at the first failure and throws it.
- Command-line errors (exit codes 2 and 43) always stop the command.

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl --fail-early -f https://httpbin.org/status/404 https://httpbin.org/get");
}
catch (CurlHttpReturnedErrorException ex)
{
    Console.WriteLine($"Stopped at HTTP {ex.StatusCode}");   // the second URL was not requested
}
```

## Inspect the transfers before running them

`Curl.Validate` parses without sending anything. The first transfer is `ParsedOptions`; the others are in `AdditionalTransfers`, each a complete `CurlOptions`:

```csharp
using CurlDotNet;

var check = Curl.Validate("curl 'https://httpbin.org/anything/[1-3]' -o 'page#1.json'");
var first = check.ParsedOptions!;

Console.WriteLine($"{first.Url} -> {first.OutputFile}");
foreach (var transfer in first.AdditionalTransfers)
    Console.WriteLine($"{transfer.Url} -> {transfer.OutputFile}");
// https://httpbin.org/anything/1 -> page1.json
// https://httpbin.org/anything/2 -> page2.json
// https://httpbin.org/anything/3 -> page3.json
```

`GlobMatches` on each transfer holds the text the globs matched (`["2"]` for the second one).

## One result per URL

The command string gives one combined result. When you want a separate `CurlResult` per URL (to check each status code, or run them in parallel), loop in C#:

```csharp
using CurlDotNet;

var urls = new[] { "https://httpbin.org/uuid", "https://httpbin.org/ip" };
var results = await Task.WhenAll(urls.Select(u => Curl.ExecuteAsync($"curl {u}")));

foreach (var r in results)
    Console.WriteLine($"{r.StatusCode} {r.Body.Length} bytes");
```

curl's `--parallel` (`-Z`) is accepted but ignored: transfers in one command always run one after another. Use `Task.WhenAll` as above, or `Curl.ExecuteManyAsync`, for concurrency.

## Related

- [curl download file in C# (curl -o)](beginner/download-file.html)
- [curl config files and variables in C#](config-files-and-variables.html)
- [curl -w write-out in C#](write-out.html)
- [curl CLI compatibility reference](../reference/curl-cli-compatibility.html)
- [Cookbook index](index.html)
