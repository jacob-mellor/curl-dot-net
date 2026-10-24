---
title: "CurlDotNet Error Reference - curl exit codes in C#"
description: "CurlDotNet error reference: curl exit codes and exceptions in C#. Which CurlException subclass is thrown for timeouts, DNS, TLS, redirects and bad URLs."
---

# CurlDotNet error reference: curl exit codes and exceptions in C#

CurlDotNet error reference: curl exit codes and exceptions in C# - this page lists the exceptions CurlDotNet throws, the curl exit code each one corresponds to, and how to handle them. Transport failures throw an exception; HTTP 4xx and 5xx status codes do not throw unless you call `result.EnsureSuccess()`.

## Exception Hierarchy

All exceptions live in the `CurlDotNet.Exceptions` namespace and derive from `CurlException`, which exposes:

- `Command` - the curl command that failed
- `CurlErrorCode` - the curl exit code (`int?`) when one applies
- `Suggestions` - a `List<string>` of fix hints
- `DiagnosticInfo` - extra context for logging

The exceptions the library throws today:

```
CurlException
├── CurlTimeoutException                (Timeout: TimeSpan?)
│   └── CurlOperationTimeoutException   -m/--max-time expired            (exit code 28)
├── CurlAbortedByCallbackException      CancellationToken cancelled       (exit code 42)
├── CurlCouldntConnectException         DNS, refused, TLS handshake       (exit code 7)
├── CurlTooManyRedirectsException       --max-redirs exceeded             (exit code 47)
├── CurlMalformedUrlException           invalid URL                       (exit code 3)
├── CurlUnsupportedProtocolException    unknown scheme                    (exit code 1)
├── CurlInvalidCommandException         no URL / bad option syntax        (no exit code)
└── CurlFileCouldntReadException        -d @file or -F file=@path missing (exit code 37)
```

`CurlDotNet.Core.CurlHttpException` is separate: it is thrown only by `result.EnsureSuccess()` and `result.EnsureStatus(code)`, and derives from `System.Exception`, not `CurlException`.

Other exception types exist in `CurlDotNet.Exceptions` (for example `CurlSslException`, `CurlCouldntResolveHostException`, `CurlAuthenticationException`, `CurlCookieException`) so that code written against curl exit codes compiles, but the cases they describe currently surface as one of the exceptions listed above.

## HTTP Status Codes Are Not Exceptions

A 404 or 500 response is a successful transfer from curl's point of view, so `ExecuteAsync` returns normally:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/404");
Console.WriteLine(result.IsSuccess);   // false
Console.WriteLine(result.StatusCode);  // 404
```

To turn a non-2xx status into an exception, call `EnsureSuccess()`:

```csharp
using CurlDotNet;

try
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/500");
    result.EnsureSuccess();
}
catch (CurlDotNet.Core.CurlHttpException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.ResponseBody}");
}
```

`CurlHttpException` exists in both `CurlDotNet.Core` and `CurlDotNet.Exceptions`. If a file imports both namespaces, qualify the type name as shown above to avoid compiler error CS0104.

## Exceptions

### CurlOperationTimeoutException

**Exit code:** 28 (CURLE_OPERATION_TIMEDOUT). Derives from `CurlTimeoutException`; some code paths throw the base type, so catch `CurlTimeoutException` to cover both.

Thrown when the request exceeds `-m`/`--max-time` (or `Curl.DefaultMaxTimeSeconds`, when set). The default is 0, which means no timeout.

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl --max-time 2 https://httpbin.org/delay/5");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Timed out after {ex.Timeout?.TotalSeconds} s");
    Console.WriteLine($"curl exit code: {ex.CurlErrorCode}");
}
```

**Fixes:** raise `--max-time`; note that `--connect-timeout` is parsed but not enforced for HTTP yet, so `--max-time` is the limit that matters; check whether the server is overloaded. See [Timeout Errors](common-issues.html#timeout-errors).

### CurlAbortedByCallbackException

**Exit code:** 42 (CURLE_ABORTED_BY_CALLBACK).

Thrown when the `CancellationToken` you passed is cancelled. CurlDotNet does not throw `OperationCanceledException`.

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
try
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/delay/10", cts.Token);
}
catch (CurlAbortedByCallbackException)
{
    Console.WriteLine("Cancelled by the caller");
}
```

### CurlCouldntConnectException

**Exit code:** 7 (CURLE_COULDNT_CONNECT). Properties: `Host`, `Port`.

Thrown when no connection could be made: the hostname did not resolve, the port refused the connection, or the TLS handshake failed (self-signed, expired or mismatched certificate).

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://localhost:9999/");
}
catch (CurlCouldntConnectException ex)
{
    Console.WriteLine($"Could not connect to {ex.Host}:{ex.Port}");
    foreach (var tip in ex.Suggestions)
        Console.WriteLine($"  - {tip}");
}
```

**Fixes:** check the hostname and port; confirm the server is running and reachable (`nc -vz host 443`); for certificate problems trust the CA on the machine, or use `-k` in development only. See [DNS and Connection Errors](common-issues.html#dns-and-connection-errors) and [SSL Certificate Errors](common-issues.html#ssl-certificate-errors).

### CurlTooManyRedirectsException

**Exit code:** 47 (CURLE_TOO_MANY_REDIRECTS). Property: `RedirectCount`.

Thrown when `-L` follows more redirects than `--max-redirs` allows.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl -L --max-redirs 2 https://httpbin.org/redirect/5");
}
catch (CurlTooManyRedirectsException ex)
{
    Console.WriteLine($"Stopped after {ex.RedirectCount} redirects");
}
```

**Fixes:** raise `--max-redirs`, or inspect the `Location` header without `-L` to find the redirect loop.

### CurlMalformedUrlException

**Exit code:** 3 (CURLE_URL_MALFORMAT). Property: `MalformedUrl`.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl 'https://exa mple.com/'");
}
catch (CurlMalformedUrlException ex)
{
    Console.WriteLine($"Invalid URL: {ex.MalformedUrl}");
}
```

**Fixes:** include the scheme (`https://`), remove spaces, percent-encode special characters.

### CurlUnsupportedProtocolException

**Exit code:** 1 (CURLE_UNSUPPORTED_PROTOCOL).

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl gopher://example.com/");
}
catch (CurlUnsupportedProtocolException ex)
{
    Console.WriteLine($"Unsupported protocol: {ex.Message}");
}
```

See [Supported Protocols](../supported-protocols.html) for the schemes CurlDotNet handles.

### CurlInvalidCommandException

**Exit code:** none (`CurlErrorCode` is `null`). Property: `InvalidPart`.

Thrown before any network activity when the command string cannot be turned into a request, most often because it contains no URL.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl -X POST -H 'Accept: application/json'");
}
catch (CurlInvalidCommandException ex)
{
    Console.WriteLine($"Bad command ({ex.InvalidPart}): {ex.Message}");
}
```

**Fix:** validate first with `Curl.Validate(command)`, which returns `IsValid` and `Error` without executing anything.

### CurlFileCouldntReadException

**Exit code:** 37 (CURLE_FILE_COULDNT_READ_FILE).

Thrown when `-d @body.json` or `-F file=@upload.bin` names a file that does not exist or cannot be read.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl -d @missing.json https://httpbin.org/post");
}
catch (CurlFileCouldntReadException ex)
{
    Console.WriteLine(ex.Message);
}
```

**Fix:** use an absolute path, or a path relative to the process working directory (not the source file).

## Exit Code Table

| curl exit code | Exception thrown by CurlDotNet | Meaning |
|---------------:|--------------------------------|---------|
| 1 | `CurlUnsupportedProtocolException` | URL scheme not supported |
| 3 | `CurlMalformedUrlException` | URL is not valid |
| 7 | `CurlCouldntConnectException` | DNS failure, connection refused or TLS handshake failed |
| 28 | `CurlOperationTimeoutException` | `--max-time` exceeded |
| 37 | `CurlFileCouldntReadException` | `@file` for `-d` or `-F` not readable |
| 42 | `CurlAbortedByCallbackException` | `CancellationToken` cancelled |
| 47 | `CurlTooManyRedirectsException` | `--max-redirs` exceeded |
| none | `CurlInvalidCommandException` | Command could not be parsed (no URL, bad option) |
| (HTTP status) | `CurlDotNet.Core.CurlHttpException` | Only from `EnsureSuccess()` / `EnsureStatus()` |

The property `CurlException.CurlErrorCode` carries the exit code so that existing scripts keyed on curl numbers keep working.

## Handling Patterns

### Catch specific exceptions first

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -m 15 https://api.github.com");
    result.EnsureSuccess();
}
catch (CurlTimeoutException) { Console.WriteLine("Slow server - try again later"); }
catch (CurlCouldntConnectException ex) { Console.WriteLine($"Unreachable: {ex.Host}"); }
catch (CurlDotNet.Core.CurlHttpException ex) when (ex.StatusCode == 429) { Console.WriteLine("Rate limited"); }
catch (CurlException ex) { Console.WriteLine($"curl error {ex.CurlErrorCode}: {ex.Message}"); }
```

### Pattern matching

```csharp
catch (CurlException ex)
{
    var message = ex switch
    {
        CurlTimeoutException => "The request timed out. Please try again.",
        CurlCouldntConnectException e => $"Could not reach {e.Host}:{e.Port}.",
        CurlTooManyRedirectsException e => $"Redirect loop ({e.RedirectCount} hops).",
        _ => $"Request failed: {ex.Message}"
    };
    Console.WriteLine(message);
}
```

### Status codes without exceptions

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/401");

var message = result.StatusCode switch
{
    200 => "OK",
    401 => "Please log in first.",
    403 => "You do not have permission.",
    404 => "Resource not found.",
    429 => $"Rate limited - retry after {result.GetHeader("Retry-After") ?? "a moment"}.",
    >= 500 => "Server error - try again later.",
    _ => $"HTTP {result.StatusCode}"
};
```

### Log everything on the exception

```csharp
catch (CurlException ex)
{
    Console.WriteLine($"Code: {ex.CurlErrorCode}");
    Console.WriteLine($"Message: {ex.Message}");
    Console.WriteLine($"Command: {ex.Command}");
    Console.WriteLine($"Diagnostics: {ex.DiagnosticInfo}");
}
```

## Related Pages

- [Common Issues](common-issues.html) - solutions to frequent problems
- [FAQ](faq.html) - frequently asked questions
- [Exception documentation](../exceptions/index.html) - one page per exception family
- [Configuration](../getting-started/configuration.html) - timeouts and other defaults
- [Handling Errors tutorial](../tutorials/06-handling-errors.html)
