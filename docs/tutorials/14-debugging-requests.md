---
title: "Debug curl Requests in C# with -v and --trace (CurlDotNet)"
description: "Debug curl requests in C# with -v, -i, -I, --trace, --trace-ascii and -w '%{http_code}' using CurlDotNet, plus result.Timings, Headers and Curl.Validate."
---

# Tutorial 14: debug curl requests in C# with -v and --trace

This tutorial shows how to debug curl requests in C# with -v and --trace using CurlDotNet. The same flags you would reach for on the command line work in the command string: `-v` for a verbose log, `-i` and `-I` for headers, `--trace` and `--trace-ascii` for a full wire dump, and `-w` for a one-line summary. You will also use `result.Timings`, `result.Headers`, `Curl.Validate` and `Curl.ToHttpClient`.

## Verbose Output (-v)

`-v` records the request line, the headers sent, and the status line and headers received. CurlDotNet puts that log at the start of `result.Body`, formatted the way curl prints it (`>` for outgoing, `<` for incoming):

```csharp
using CurlDotNet;

// Same as: curl -v https://httpbin.org/get
var result = await Curl.ExecuteAsync("curl -v https://httpbin.org/get");

Console.WriteLine(result.Body);
// *   Trying httpbin.org...
// > GET /get HTTP/1.1
// > Host: httpbin.org
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

`-i` prints the header block before the body, as curl does. `-I` sends a HEAD request, so there is no body at all; combine it with `-i` if you want the header text in `Body`:

```csharp
// -i: status line and headers, blank line, then the body
var full = await Curl.ExecuteAsync("curl -i https://httpbin.org/get");
Console.WriteLine(full.Body);

// -I: HEAD request. Headers are in full.Headers; add -i to see them as text
var head = await Curl.ExecuteAsync("curl -I -i https://httpbin.org/get");
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

`-w` appends a formatted line after the body. The supported variables are `%{http_code}`, `%{size_download}`, `%{url_effective}` and `%{content_type}`:

```csharp
var result = await Curl.ExecuteAsync(
    "curl -w '%{http_code} %{content_type} %{size_download}' https://httpbin.org/get");

// The formatted line is the last line of Body
Console.WriteLine(result.Body.Split('\n').Last());   // 200 application/json 2xx
```

`%{url_effective}` is handy with `-L` to see where a redirect chain ended. For plain status checks you do not need `-w` at all; `result.StatusCode` is already an `int`.

## Timing a Request

`result.Timings` reports milliseconds for the main phases. Because the transport is `HttpClient`, the time to first byte (`StartTransfer`) and the total are measured directly; the DNS, TCP and TLS phases are not broken out separately and read as zero:

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

An unreachable server fails with `CurlCouldntConnectException` instead, so the exception type already tells you whether the server is unreachable or just slow.

### Connection failures

DNS failures, refused connections and TLS handshake errors all surface as `CurlCouldntConnectException` with the host and port attached:

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
4. **Catch the specific exception** - `CurlOperationTimeoutException` and `CurlCouldntConnectException` tell you where the request failed.
5. **Print `ex.Suggestions`** - the library's hints often name the fix.
6. **Check Content-Type before parsing JSON.**
7. **Validate user-supplied commands** with `Curl.Validate` before running them.
8. **Keep `-k` and debugging proxies out of production.**

## Summary

- `-v` prepends a verbose log to `Body`; `-i` adds the header block; `-I` sends HEAD.
- `--trace` and `--trace-ascii` write a complete wire dump to a file.
- `-w '%{http_code}'` appends a summary line; `result.Timings` gives time to first byte and total.
- `Curl.Validate` checks a command without sending it; `Curl.ToHttpClient` shows the equivalent .NET code.

More help: [Common Issues](../troubleshooting/common-issues.html) and the [Error Reference](../troubleshooting/error-reference.html).

## Congratulations

You have completed all 14 tutorials. You can now make requests with curl syntax from C#, read results and headers, handle errors and timeouts, send forms, JSON and files, authenticate, cancel and parallelise requests, and debug what goes over the wire. Continue with the [Cookbook](../cookbook/index.html) for task-focused recipes or the [API Guide](../api-guide/README.html) for the full surface.

---

[Previous: Parallel Requests](13-parallel-requests.html) | [Back to Tutorials](README.html)
