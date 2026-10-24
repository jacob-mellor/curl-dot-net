---
title: "Debug curl Requests in C# with -v and --trace (CurlDotNet)"
description: "Debug curl requests in C# with -v, -i, -I, --trace, --trace-ascii and -w '%{http_code}' using CurlDotNet, plus result.Timings, Headers and Curl.Validate."
---

# Tutorial 14: debug curl requests in C# with -v and --trace

This tutorial shows how to debug curl requests in C# with -v and --trace using CurlDotNet. The same flags you would reach for on the command line work in the command string: `-v` for a verbose log, `-i` and `-I` for headers, `--trace` and `--trace-ascii` for a full wire dump, and `-w` for a one-line summary. You will also use `result.Timings`, `result.Headers`, `Curl.Validate` and `Curl.ToHttpClient`.

## Verbose Output (-v)

`-v` records the request line, the headers sent, and the status line and headers received. curl prints that log to standard error; CurlDotNet puts it at the start of `result.Body`, formatted the way curl prints it (`>` for outgoing, `<` for incoming). It shows the headers CurlDotNet adds by default, such as `User-Agent: curl/8.7.1` and `Accept: */*`:

```csharp
using CurlDotNet;

// Same as: curl -v https://httpbin.org/get
var result = await Curl.ExecuteAsync("curl -v https://httpbin.org/get");

Console.WriteLine(result.Body);
// *   Trying httpbin.org...
// > GET /get HTTP/1.1
// > Host: httpbin.org
// > User-Agent: curl/8.7.1
// > Accept: */*
// >
// < HTTP/1.1 200 OK
// < Content-Type: application/json
// <
// { "args": {}, ... }
```

The builder exposes the same switch as `Verbose()`:

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/get")
    .Verbose()
    .ExecuteAsync();
```

## Inspecting Headers (-i, -I, result.Headers)

Response headers are always parsed into `result.Headers`, a case-insensitive dictionary, whether or not you ask for them in the body:

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

foreach (var h in result.Headers)
    Console.WriteLine($"{h.Key}: {h.Value}");

Console.WriteLine(result.GetHeader("content-type"));   // application/json
Console.WriteLine(result.HasHeader("X-Request-Id"));   // false
```

`-i` prints the header block before the body, exactly as the server sent it. `-I` sends a HEAD request and, like curl, prints the header block as the output, so the header text is in `Body`. With `-L -i` you see every hop's headers, and `-D file` (or `-D -`) dumps them separately:

```csharp
// -i: status line and headers, blank line, then the body
var full = await Curl.ExecuteAsync("curl -i https://httpbin.org/get");
Console.WriteLine(full.Body);

// -I: HEAD request; the header block is the output
var head = await Curl.ExecuteAsync("curl -I https://httpbin.org/get");
Console.WriteLine(head.Body);        // HTTP/1.1 200 OK + header lines, no body
```

The builder's `IncludeHeaders()` corresponds to `-i`, and `CurlRequestBuilder.Head(url)` to `-I`.

## Full Wire Trace (--trace, --trace-ascii)

When `-v` is not enough, `--trace` writes everything, including request and response bodies, to a file:

```csharp
// Same as: curl --trace trace.txt https://httpbin.org/get
await Curl.ExecuteAsync("curl --trace trace.txt https://httpbin.org/get");
Console.WriteLine(File.ReadAllText("trace.txt"));

// --trace-ascii: the same content without the hex column (easier to read)
await Curl.ExecuteAsync(
    "curl --trace-ascii trace-ascii.txt -d 'a=1' https://httpbin.org/post");
```

The trace records connection and proxy selection, TLS verification, redirects and the body bytes. Keep trace files out of version control; they contain any credentials you sent.

## One-Line Summaries (-w)

`-w` (`--write-out`) appends its text to `Body` right after the response body, with no extra newline, just as curl writes it to stdout. Every curl variable is supported: `%{http_code}`, `%{http_version}`, `%{size_download}`, the `%{time_*}` timings (seconds with six decimals), `%{url_effective}`, `%{num_redirects}`, `%{remote_ip}`, `%header{name}`, `%{json}` and the rest:

```csharp
var result = await Curl.ExecuteAsync(
    "curl -w '\\n%{http_code} %{content_type} %{time_total}' https://httpbin.org/get");

// The body comes first; the leading \n in the format puts the summary on its own last line
Console.WriteLine(result.Body.Split('\n').Last());   // 200 application/json 0.412345
```

`%{url_effective}` is handy with `-L` to see where a redirect chain ended. When the transfer fails, the `-w` output is on the exception: `CurlException.StandardOutput` has everything curl would have printed. For plain status checks you do not need `-w` at all; `result.StatusCode` is already an `int`. The [write-out cookbook page](../cookbook/write-out.html) lists every variable.

## Timing a Request

`result.Timings` reports milliseconds for the main phases. The time to first byte (`StartTransfer`) and the total are always measured; `NameLookup`, `Connect` and `AppConnect` (TLS) are filled in on .NET 5 and later when the request opened a new connection, and stay zero when an existing connection was reused (and on .NET Standard 2.0 / .NET Framework). For curl's exact numbers, use `-w '%{time_namelookup} %{time_connect} %{time_appconnect} %{time_starttransfer} %{time_total}'`:

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/delay/1");
var t = result.Timings;

Console.WriteLine($"Time to first byte: {t.StartTransfer:F0} ms");
Console.WriteLine($"Total:              {t.Total:F0} ms");   // a little over 1000
```

If a request is slow, compare `StartTransfer` with `Total`: a large gap means a slow download, a large `StartTransfer` means the server took long to answer.

## Checking a Command Before Running It

`Curl.Validate` parses the command without sending anything. Use it on user-supplied commands or in unit tests:

```csharp
var check = Curl.Validate("curl -X POST -H 'Content-Type: application/json' https://httpbin.org/post");

if (!check.IsValid)
{
    Console.WriteLine($"Bad command: {check.Error}");   // e.g. "No URL specified"
    return;
}
```

A misspelled option (`--hedaer`) makes the command invalid and is listed in `check.ParsedOptions.UnknownOptions`. Real curl options that CurlDotNet has no implementation for (such as `--parallel` or `--ipv4`) are listed in `UnsupportedOptions` instead, so you can warn about them.

## Comparing with Hand-Written HttpClient Code

When you want to see what the command does in plain .NET terms, generate the equivalent `HttpClient` code:

```csharp
string code = Curl.ToHttpClient("curl -H 'Accept: application/json' https://httpbin.org/get");
Console.WriteLine(code);
```

`Curl.ToFetch`, `Curl.ToPythonRequests` and `Curl.ToPowershellCode` do the same for other languages, which is useful when you are reproducing a problem for someone else's team.

## Common Issues and Solutions

### Timeouts

A timeout throws `CurlOperationTimeoutException`. Every CurlDotNet exception carries the command that ran and a list of suggestions:

```csharp
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/delay/20");
}
catch (CurlOperationTimeoutException ex)
{
    Console.WriteLine($"Timed out ({ex.Timeout}). Command: {ex.Command}");
    foreach (var tip in ex.Suggestions) Console.WriteLine($"  - {tip}");
}
```

An unreachable server fails with `CurlCouldntConnectException` instead, so the exception type already tells you whether the server is unreachable or just slow. With `--retry N`, timeouts are retried automatically before the exception reaches you (see [retries](../cookbook/retry.html)).

### Connection failures

Each kind of connection failure has its own exception and curl exit code: `CurlCouldntResolveHostException` (6) for DNS, `CurlCouldntResolveProxyException` (5) for a proxy name, `CurlCouldntConnectException` (7) for a refused or unreachable host, and `CurlSslConnectErrorException` (35) or `CurlPeerFailedVerificationException` (60) for TLS. When a proxy is in use, the message names the proxy. A refused connection carries the host and port:

```csharp
try
{
    await Curl.ExecuteAsync("curl https://localhost:9/nothing-listens-here");
}
catch (CurlCouldntConnectException ex)
{
    Console.WriteLine($"Could not connect to {ex.Host}:{ex.Port}");
    Console.WriteLine(ex.DiagnosticInfo);
}
```

### Authentication issues

Compare the anonymous and authenticated responses. A `401` means credentials are missing or wrong; a `403` means they were accepted but do not grant access. The `WWW-Authenticate` header tells you which scheme the server wants:

```csharp
var anon = await Curl.ExecuteAsync("curl https://httpbin.org/basic-auth/user/passwd");
Console.WriteLine(anon.StatusCode);                        // 401
Console.WriteLine(anon.GetHeader("WWW-Authenticate"));     // Basic realm="Fake Realm"

var auth = await Curl.ExecuteAsync(
    "curl -u user:passwd https://httpbin.org/basic-auth/user/passwd");
Console.WriteLine(auth.StatusCode);                        // 200
```

[Tutorial 9](09-authentication-basics.html) covers the authentication flags.

### JSON parsing issues

When `ParseJson<T>` fails, look at the Content-Type and the first part of the body. Error pages and HTML login forms are the usual culprits:

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/html");

var contentType = result.GetHeader("Content-Type") ?? "(none)";
if (!contentType.Contains("json"))
    Console.WriteLine($"Not JSON: {contentType}");

try   { var data = result.ParseJson<Dictionary<string, object>>(); }
catch (InvalidOperationException ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine(result.Body.Substring(0, Math.Min(200, result.Body.Length)));
}
```

## Debugging Proxies (Fiddler, Charles, mitmproxy)

Route traffic through a local proxy with `-x`. The proxy re-signs HTTPS, so add `-k` to skip certificate checks while you debug. Never ship `-k` in production code:

```csharp
// Same as: curl -x http://localhost:8888 -k https://httpbin.org/get
var result = await Curl.ExecuteAsync(
    "curl -x http://localhost:8888 -k https://httpbin.org/get");
```

The builder equivalents are `WithProxy("http://localhost:8888")` and `Insecure()`.

## Best Practices

1. **Start with `-v`** - it answers most "what did I actually send?" questions.
2. **Read `result.Headers` instead of parsing text** when your code needs a header value.
3. **Use `--trace` for body-level problems** and delete the file afterwards; it contains your credentials.
4. **Catch the specific exception** - `CurlOperationTimeoutException`, `CurlCouldntResolveHostException` and `CurlCouldntConnectException` tell you where the request failed; `ex.CurlErrorCode` is curl's exit code ([full list](../reference/curl-exit-codes.html)).
5. **Print `ex.Suggestions`** - the library's hints often name the fix.
6. **Check Content-Type before parsing JSON.**
7. **Validate user-supplied commands** with `Curl.Validate` before running them.
8. **Keep `-k` and debugging proxies out of production.**

## Summary

- `-v` prepends a verbose log to `Body`; `-i` adds the header block; `-I` sends HEAD.
- `--trace` and `--trace-ascii` write a complete wire dump to a file.
- `-w '%{http_code}'` appends curl's write-out text to `Body`; `result.Timings` gives the phase timings in milliseconds.
- `Curl.Validate` checks a command without sending it; `Curl.ToHttpClient` shows the equivalent .NET code.

More help: [Common Issues](../troubleshooting/common-issues.html) and the [Error Reference](../troubleshooting/error-reference.html).

## Congratulations

You have completed all 14 tutorials. You can now make requests with curl syntax from C#, read results and headers, handle errors and timeouts, send forms, JSON and files, authenticate, cancel and parallelise requests, and debug what goes over the wire. Continue with the [Cookbook](../cookbook/index.html) for task-focused recipes or the [API Guide](../api-guide/README.html) for the full surface.

---

[Previous: Parallel Requests](13-parallel-requests.html) | [Back to Tutorials](README.html)
