---
title: "CurlDotNet Error Reference - curl exit codes in C#"
description: "CurlDotNet error reference: curl exit codes and exceptions in C#. Which CurlException subclass is thrown for timeouts, DNS, TLS, redirects and bad URLs."
---

# CurlDotNet error reference: curl exit codes and exceptions in C#

CurlDotNet error reference: curl exit codes and exceptions in C# - this page is the complete list of exception types in `CurlDotNet.Exceptions`, the curl exit code each one carries in `CurlErrorCode`, when CurlDotNet actually throws it, and how to fix it. Failed transfers throw, with the same exit code the curl tool would print; HTTP 4xx and 5xx status codes do not throw unless you pass `-f`/`--fail` or call `result.EnsureSuccess()`. A cancelled `CancellationToken` throws the standard `OperationCanceledException`. For the curl tool's full numbered list, see [curl exit codes](../reference/curl-exit-codes.html).

## Exception Hierarchy

Every type below lives in `CurlDotNet.Exceptions` unless its namespace is written out. The number after each type is the `CurlErrorCode` it carries ("none" means `null`). Types marked *reserved* exist so that code written against curl's error list compiles, but nothing in the library constructs them today.

```
System.Exception
├── CurlDotNet.Core.CurlHttpException          result.EnsureSuccess() / EnsureStatus()    none
└── CurlException                              base of everything below                   (varies)
    ├── CurlInvalidCommandException            command has options but no URL             2
    │   ├── CurlUnknownOptionException         reserved                                   none
    │   └── CurlOptionSyntaxException          bad --resolve / --connect-to entry         49
    ├── CurlConnectionException                reserved                                   none
    │   ├── CurlDnsException                   reserved                                   6
    │   └── CurlProxyException                 reserved                                   5
    ├── CurlTimeoutException                   timeout base (also a rare fallback)        28
    │   ├── CurlOperationTimeoutException      -m, --connect-timeout, --speed-limit       28
    │   └── CurlFtpAcceptTimeoutException      reserved                                   12
    ├── CurlSslException                       TLS base, reserved as a concrete type      60
    │   ├── CurlSslConnectErrorException       TLS handshake failed                       35
    │   ├── CurlPeerFailedVerificationException server certificate rejected               60
    │   ├── CurlSslCertificateProblemException --cert/--key file unusable                 58
    │   ├── CurlSslCaCertBadFileException      --cacert/--capath missing or empty         77
    │   ├── CurlSslPinnedPublicKeyMismatchException --pinnedpubkey mismatch           90
    │   ├── CurlSslCipherException             reserved                                   59
    │   ├── CurlSslEngineNotFoundException     reserved                                   53
    │   ├── CurlSslEngineSetFailedException    reserved                                   54
    │   └── CurlUseSslFailedException          reserved                                   64
    ├── CurlAuthenticationException            reserved                                   67
    │   └── CurlLoginDeniedException           reserved                                   67
    ├── CurlHttpException                      result.EnsureSuccessStatusCode()           none
    │   ├── CurlHttpReturnedErrorException     -f/--fail, --fail-with-body                22
    │   └── CurlRateLimitException             reserved                                   none
    ├── CurlUnsupportedProtocolException       scheme with no handler, bad status line    1
    ├── CurlNotSupportedException              reserved                                   1
    ├── CurlFailedInitException                reserved                                   2
    ├── CurlMalformedUrlException              URL cannot be parsed                       3
    ├── CurlNotBuiltInException                --pinnedpubkey cannot be checked here      4
    ├── CurlCouldntResolveProxyException       proxy name did not resolve                 5
    ├── CurlCouldntResolveHostException        host name did not resolve                  6
    ├── CurlCouldntConnectException            connection refused / unreachable           7
    ├── CurlWeirdServerReplyException          malformed response head                    8
    ├── CurlRemoteAccessDeniedException        reserved                                   9
    ├── CurlFtpException                       reserved (FTP uses the Core type below)    9
    ├── CurlDotNet.Core.CurlFtpException       FTP server returned an error reply         9
    ├── CurlFtpAcceptFailedException           reserved                                   10
    ├── CurlFtpWeirdPassReplyException         reserved                                   11
    ├── CurlPartialFileException               body cut short                             18
    ├── CurlWriteErrorException                output, header or cookie-jar file          23
    ├── CurlUploadFailedException              reserved                                   25
    ├── CurlReadErrorException                 input file missing or unreadable           26
    ├── CurlOutOfMemoryException               reserved                                   27
    ├── CurlRangeErrorException                -C resume refused by the server            33
    ├── CurlHttpPostErrorException             reserved                                   34
    ├── CurlBadDownloadResumeException         reserved                                   36
    ├── CurlFileCouldntReadException           -d @file / --json @file missing            37
    ├── CurlFunctionNotFoundException          reserved                                   41
    ├── CurlAbortedByCallbackException         progress or redirect callback threw        42
    ├── CurlBadFunctionArgumentException       CR/LF in a header, resumed upload          43
    ├── CurlInterfaceFailedException           reserved                                   45
    ├── CurlTooManyRedirectsException          --max-redirs exceeded                      47
    ├── CurlRedirectException                  reserved                                   47
    ├── CurlGotNothingException                empty reply from server                    52
    ├── CurlSendErrorException                 reserved                                   55
    ├── CurlReceiveErrorException              connection failed while reading            56
    ├── CurlBadContentEncodingException        body or transfer coding cannot be decoded  61
    ├── CurlFileSizeExceededException          --max-filesize exceeded                    63
    ├── CurlAuthErrorException                 Digest/NTLM/Negotiate negotiation failed   94
    ├── CurlTooLargeException                  too many / too long response headers       100
    ├── CurlRetryException                     RetryMiddleware ran out of attempts        none
    ├── CurlFileException                      reserved                                   none
    ├── CurlParsingException                   reserved                                   none
    ├── CurlExecutionException                 reserved                                   none
    └── CurlCookieException                    reserved                                   none
```

Each TLS subclass carries its own curl number (53, 54, 58, 59, 64, 77, 90), and `CurlFtpAcceptTimeoutException` carries 12, even though they derive from `CurlSslException` (60) and `CurlTimeoutException` (28); catch the base type to cover a family. `CurlUnknownOptionException` carries none (not 48). `CurlOptionSyntaxException` carries 49 when thrown by the library; its single-argument constructor leaves the code `null`.

`CurlDotNet.Core.CurlHttpException` (from `EnsureSuccess()`) derives from `System.Exception`, not `CurlException`, and `CurlDotNet.Core.CurlFtpException` (with an `FtpStatusCode` property) is the FTP type the FTP handler actually throws. If a file imports both `CurlDotNet.Core` and `CurlDotNet.Exceptions`, write those names out in full to avoid compiler error CS0104.

## Errors Thrown as a Plain CurlException

Some command-line problems are reported with the base `CurlException` itself (`ex.GetType() == typeof(CurlException)`), carrying the exit code curl uses for them:

| `CurlErrorCode` | When | Fix |
|---:|------|-----|
| 2 | An option is the last word of the command and has no value (`curl -H`) | Put the value right after the option; quote it if it has spaces |
| 2 | Two request methods are asked for: `-I` with `-d`, `-F` with `-d`, `--json` with `-F`, and similar | Keep one; use `-X` to change only the method name |
| 2 | The command is only `curl`, or an `ExecuteAsync(CurlOptions)` call has no `Url` | Add the URL |
| 2 | A numeric option has a value that is not a number: `-m`, `--connect-timeout`, `--max-redirs`, `--retry`, `--retry-delay`, `--retry-max-time`, `--speed-limit`, `--speed-time`, `--limit-rate`, `--max-filesize` (the message names the option) | Give a number; seconds are read like curl (`2.5`, `1e1`, `+5`), sizes take a k, M, G, T or P suffix |
| 2 | `--aws-sigv4` is malformed (empty, white space, more than four `:` parts, a part over 64 characters); `AwsSigV4Config.Parse` throws the same | Use `provider1[:provider2[:region[:service]]]` |
| 2 | `-C`/`--continue-at` combined with `--no-clobber` or `--remove-on-error` | Remove one of the two options |
| 2 | `-K`/`--config` files include each other more than 10 levels deep; a `--variable` error (unset `%ENV` variable, bad byte range, NUL byte in an expansion) | Fix the config file or the variable as the message says |
| 3 | A URL glob is malformed (`[1-` without `]`) | Fix the `{a,b}` / `[1-10]` expression, escape `\{` `\[`, or pass `-g` |
| 43 | `-o` uses `#<name>` but the URL has no glob with that name | Name the glob in the URL or remove the placeholder |
| 1 | An `ftp://` URL on a runtime without `FtpWebRequest` | Use HTTP(S), or a .NET runtime that includes FTP |
| 100 | The NTLM user name (and domain) is too long for the type-3 message | Use a shorter user name |

Unknown options do not throw: curl would exit with 2, but CurlDotNet runs the command and lists the typo in `CurlOptions.UnknownOptions`. Call `Curl.Validate(command)` before running a command you did not write yourself; it returns `IsValid == false` with `Error = "Unknown option: ..."`.

## Properties on Every CurlException

- `Command` - the curl command that failed
- `CurlErrorCode` - the curl exit code (`int?`); `GetCurlErrorName()` returns the `CURLE_*` name
- `StandardOutput` - what the curl tool would already have printed to standard output before failing: `-i` headers, the body kept by `--fail-with-body`, the `-w` text, and the output of earlier URLs in the same command (`null` when nothing was written). The same string is in `ex.Data["CurlDotNet.Stdout"]` (`CurlException.StandardOutputDataKey`)
- `ex.Data["CurlDotNet.WriteOut"]` (`CurlException.WriteOutDataKey`) - only the `-w`/`--write-out` text of the failed transfer, so `%{exitcode}` and `%{errormsg}` are usable after a failure
- `Suggestions` - a `List<string>` of fix hints; `DiagnosticInfo` - extra context for logging
- `IsRetryable()`, `ToDetailedString()`, `ToJson()`, `GetDiagnosticInfo()` - helpers for retry decisions and logging. The last three mask credentials in the command (`-u`/`-U`, `--oauth2-bearer`, `--pass`, `-b` values, `Authorization`/`Proxy-Authorization`/`Cookie` headers, URL and proxy passwords); `ex.Command` itself is the command as written

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl -i -f -w '%{http_code} %{exitcode}' https://httpbin.org/status/500");
}
catch (CurlException ex)
{
    Console.WriteLine($"{ex.GetCurlErrorName()} ({ex.CurlErrorCode})");       // CURLE_HTTP_RETURNED_ERROR (22)
    Console.WriteLine(ex.StandardOutput);                                       // the 500 response headers, then "500 22"
    Console.WriteLine(ex.Data[CurlException.WriteOutDataKey]);                  // "500 22"
}
```

## HTTP Status Codes Are Not Exceptions

Without `-f`, a 404 or 500 response is a successful transfer from curl's point of view, so `ExecuteAsync` returns normally:

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

With `-f`/`--fail` the command itself throws, like the curl tool exiting with 22: see [CurlHttpReturnedErrorException](#curlhttpreturnederrorexception) below.

## Cancellation Is OperationCanceledException

A cancelled `CancellationToken` surfaces as the standard `OperationCanceledException` (`TaskCanceledException` derives from it) from `Curl.ExecuteAsync`, `Curl.Execute` and `LibCurl`. It is not a `CurlException`, so a `catch (CurlException)` block does not swallow it. Cancellation is converted before it reaches your code; `CurlAbortedByCallbackException` (exit code 42) is thrown only when your own progress callback or `CurlSettings.OnRedirect` handler throws.

```csharp
using CurlDotNet;

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
try
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/delay/10", cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Cancelled by the caller");
}
```

## Exceptions

### CurlOperationTimeoutException

**Exit code:** 28 (CURLE_OPERATION_TIMEDOUT). Derives from `CurlTimeoutException`; one rare fallback path throws the base type, so catch `CurlTimeoutException` to cover both.

Thrown when the transfer exceeds `-m`/`--max-time` (or `Curl.DefaultMaxTimeSeconds`, when set) - the limit covers the whole transfer, body included - when the connection is not established within `--connect-timeout`, or when the download stays below `--speed-limit` bytes per second for `--speed-time` seconds ("Operation too slow"). When `--retry-max-time` runs out, the last attempt's own exception is thrown, as in curl. There is no implicit timeout: without `-m` or `Curl.DefaultMaxTimeSeconds` a transfer may run as long as the server keeps sending. Both options accept decimals (`--max-time 2.5`).

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

**Fixes:** raise `--max-time` for slow servers or large downloads; raise `--connect-timeout` or check reachability when the message says "Connection timed out"; add `--retry N` to retry timeouts automatically. See [Timeout Errors](../exceptions/timeout-errors.html).

### CurlCouldntResolveHostException and CurlCouldntResolveProxyException

**Exit codes:** 6 (CURLE_COULDNT_RESOLVE_HOST, property `Hostname`) and 5 (CURLE_COULDNT_RESOLVE_PROXY, property `ProxyHost`).

Thrown when the URL's host name, or the proxy's name (from `-x`/`--proxy` or the `http_proxy`/`https_proxy`/`all_proxy` environment variables), does not resolve.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://this-host-does-not-exist.invalid/");
}
catch (CurlCouldntResolveHostException ex)
{
    Console.WriteLine($"Could not resolve {ex.Hostname} (exit code {ex.CurlErrorCode})");   // 6
}
```

**Fixes:** check the spelling and the machine's DNS; `--resolve host:443:203.0.113.10` pins a name to an address without DNS; for a proxy, check `-x` and the proxy environment variables, or add the host to `--noproxy`.

### CurlCouldntConnectException

**Exit code:** 7 (CURLE_COULDNT_CONNECT). Properties: `Host`, `Port`, `ViaProxy`.

Thrown when the name resolved but no connection could be made: connection refused, host unreachable, or a firewall drop. With a proxy, `Host`/`Port` name the proxy and `ViaProxy` is `true`; a refused `-p`/`--proxytunnel` CONNECT also lands here. TLS problems have their own types (35 and 60, see below).

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://localhost:9999/");
}
catch (CurlCouldntConnectException ex)
{
    Console.WriteLine($"Could not connect to {(ex.ViaProxy ? "proxy " : "")}{ex.Host}:{ex.Port}");
}
```

**Fixes:** check that the server is running and the host and port in the URL are right (`nc -vz host 443`); check firewalls; `--retry-connrefused` together with `--retry N` retries refused connections. See [DNS and Connection Errors](../exceptions/dns-errors.html).

### CurlSslConnectErrorException and CurlPeerFailedVerificationException

**Exit codes:** 35 (CURLE_SSL_CONNECT_ERROR) and 60 (CURLE_PEER_FAILED_VERIFICATION). Both derive from `CurlSslException`, so catch that to cover both.

`CurlPeerFailedVerificationException` means the server's certificate was rejected (self-signed, expired, wrong host name, untrusted CA). `CurlSslConnectErrorException` means the handshake failed for another reason, such as no common TLS version or a plain-HTTP port. **Fixes:** trust the CA in the operating system store, or for development only use `-k`. On .NET 5 and later, `--cacert`/`--capath` trust a private CA, `--cert`/`--key` send a client certificate and `--pinnedpubkey` pins the server key; problems with those files throw `CurlSslCertificateProblemException` (58), `CurlSslCaCertBadFileException` (77) or `CurlSslPinnedPublicKeyMismatchException` (90), all derived from `CurlSslException`. See [SSL/TLS errors](../exceptions/ssl-errors.html).

### CurlHttpReturnedErrorException

**Exit code:** 22 (CURLE_HTTP_RETURNED_ERROR). Derives from `CurlDotNet.Exceptions.CurlHttpException`. Properties: `StatusCode`, `StatusText`, `ResponseBody`, `ResponseHeaders`.

Thrown when the command has `-f`/`--fail` (or `--fail-with-body`) and the server answers 400 or above. With `-f` the body is discarded, as in curl; with `--fail-with-body` it is kept in `ResponseBody` and in `StandardOutput`.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl --fail-with-body https://httpbin.org/status/404");
}
catch (CurlHttpReturnedErrorException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.ResponseBody}");
}
```

**Fix:** drop `-f` to get a normal `CurlResult` with the status code, or handle the exception.

### CurlTooManyRedirectsException

**Exit code:** 47 (CURLE_TOO_MANY_REDIRECTS). Property: `RedirectCount`.

Thrown when `-L` would follow more redirects than `--max-redirs` allows (default 50; `--max-redirs N` allows exactly N, `-1` is unlimited).

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
    Console.WriteLine(ex.Message);   // Malformed URL: Invalid URL: https://exa mple.com/
}
```

Also thrown when a redirect's `Location` header is not a valid URL. **Fixes:** remove spaces from the host, use a numeric port, percent-encode special characters. A URL without a scheme is fine: like curl, CurlDotNet prepends `http://` (`ftp://` for hosts starting with `ftp.`).

### CurlUnsupportedProtocolException

**Exit code:** 1 (CURLE_UNSUPPORTED_PROTOCOL). Property: `Protocol`.

Thrown for a URL scheme CurlDotNet has no handler for (`gopher://`, `sftp://`, `scp://`, `smtp://`, `imap://`, `ldap://`, `telnet://`, or a typo such as `htp://`), for a `-L` redirect whose `Location` points at a scheme other than `http`/`https`, and for a response status line in an HTTP version curl does not speak (`HTTP/1.2`, or HTTP/0.9 without `--http0.9`).

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl gopher://example.com/");
}
catch (CurlUnsupportedProtocolException ex)
{
    Console.WriteLine(ex.Message);
}
```

**Fixes:** use `http://`, `https://`, `ftp://` or `file://`; for a redirect to another scheme, fetch the target explicitly if you trust it. See [Supported Protocols](../supported-protocols.html).

### CurlInvalidCommandException

**Exit code:** 2, like the curl tool. Property: `InvalidPart`.

Thrown before any network activity when the command has options but no URL (`InvalidPart` is `"URL"`). A command that is only `curl` throws a plain `CurlException` with code 2, and option problems throw the plain `CurlException` codes listed [above](#errors-thrown-as-a-plain-curlexception). Its subclass `CurlOptionSyntaxException` (code 49) is thrown for a `--resolve` or `--connect-to` entry whose port is not a number.

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

**Fix:** validate first with `Curl.Validate(command)`, which returns `IsValid` and `Error` (including unknown options) without executing anything.

### CurlReadErrorException and CurlFileCouldntReadException

**Exit codes:** 26 (CURLE_READ_ERROR) and 37 (CURLE_FILE_COULDNT_READ_FILE). Property: `FilePath`.

An input file named on the command line is missing or unreadable. `CurlReadErrorException` (26) covers `-T`/`--upload-file`, `-F name=@file` and `-F name=<file`, `--data-urlencode @file`, `--url-query @file`, `--variable name@file`, `-K`/`--config`, a `-d @file` that is combined with other `-d` items, `-` (standard input, which CurlDotNet does not read) for these options, a `.netrc` file with an unclosed quote, and an I/O error on a `file://` URL. A single `-d @file`, `--data-binary @file` or `--json @file` whose file does not exist throws `CurlFileCouldntReadException` (37), as does a `file://` URL that cannot be opened for lack of permission. A `file://` URL that does not exist returns a result with `StatusCode` 404.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl -T missing.bin https://httpbin.org/put");
}
catch (CurlReadErrorException ex)
{
    Console.WriteLine($"{ex.FilePath}: {ex.Message}");
}
```

**Fix:** use an absolute path, or a path relative to the process working directory (not the source file). Missing output directories and unwritable files are the mirror image: `CurlWriteErrorException` (23), see [transfer and file errors](../exceptions/transfer-errors.html).

## Exit Code Table

Every exception type, sorted by the `CurlErrorCode` it carries. "Reserved" means nothing in the library constructs the type today. Each message the library writes explains the fix in its own words; the last column summarises it.

| Code | Exception | Thrown when | How to fix |
|-----:|-----------|-------------|------------|
| 1 | `CurlUnsupportedProtocolException` | Scheme with no handler (`gopher://`, `sftp://`, `htp://`...); `-L` redirect to a non-HTTP scheme; unsupported HTTP version in the status line | Use http, https, ftp or file; fetch the redirect target yourself; `--http0.9` for HTTP/0.9 |
| 1 | `CurlNotSupportedException` | Reserved | - |
| 1 | `CurlException` (plain) | `ftp://` on a runtime without `FtpWebRequest` | Use HTTP(S) or a runtime with FTP |
| 2 | `CurlException` (plain) | Command is only `curl`; no `Url` in `CurlOptions`; option missing its value; non-numeric value for a numeric option; two request methods; `-C` with `--no-clobber`/`--remove-on-error`; `-K` nesting; `--variable` errors | Fix the option as the message says |
| 2 | `CurlInvalidCommandException` | Options but no URL; malformed `--aws-sigv4` | Add the URL; fix the value |
| 2 | `CurlFailedInitException` | Reserved | - |
| 3 | `CurlMalformedUrlException` | URL (or redirect `Location`) cannot be parsed | Fix spaces, port, encoding |
| 3 | `CurlException` (plain) | Malformed URL glob | Fix the glob or pass `-g` |
| 4 | `CurlNotBuiltInException` | `--pinnedpubkey` on an https transfer where the pin cannot be checked (.NET Standard 2.0 / .NET Framework, or your own `HttpClient`) | Run on .NET 5+ or drop the pin |
| 5 | `CurlCouldntResolveProxyException` | Proxy name does not resolve | Check `-x` and proxy environment variables; `--noproxy` |
| 5 | `CurlProxyException` | Reserved | - |
| 6 | `CurlCouldntResolveHostException` | Host name does not resolve | Check spelling and DNS; `--resolve` |
| 6 | `CurlDnsException` | Reserved | - |
| 7 | `CurlCouldntConnectException` | Connection refused or unreachable (host or proxy, `ViaProxy`); `-p` tunnel refused; FTP connection failed | Start the server, check host/port/firewall; `--retry-connrefused` |
| 8 | `CurlWeirdServerReplyException` | Malformed response head (invalid header line, bad status code, NUL in a chunked trailer) | Server bug; inspect with `-v` |
| 9 | `CurlDotNet.Core.CurlFtpException` | FTP server answered with an error (`FtpStatusCode`) | Check path, `-u` credentials and server permissions |
| 9 | `CurlFtpException`, `CurlRemoteAccessDeniedException` | Reserved | - |
| 10 | `CurlFtpAcceptFailedException` | Reserved | - |
| 11 | `CurlFtpWeirdPassReplyException` | Reserved | - |
| 18 | `CurlPartialFileException` | Body shorter than `Content-Length`, or chunked body without its last chunk | Retry; resume with `-C -`; `--ignore-content-length` accepts what arrived |
| 22 | `CurlHttpReturnedErrorException` | `-f`/`--fail` or `--fail-with-body` and status >= 400 | Drop `-f` to get a `CurlResult`, or handle it |
| 23 | `CurlWriteErrorException` | `-o`/`-O`/`-D`/`-c` file cannot be written: missing directory, invalid name, permissions, disk full; `-O -J` would overwrite; `--no-clobber` ran out of names | Create the directory or add `--create-dirs`; pick a writable path |
| 25 | `CurlUploadFailedException` | Reserved | - |
| 26 | `CurlReadErrorException` | Input file missing or unreadable (`-H @file`, `-T`, `-F @`/`<`, `--data-urlencode @`, `--url-query @`, `--variable @`, `-K`, mixed `-d @file`, `-` stdin); bad `.netrc` quoting | Fix the path (relative to the working directory) |
| 27 | `CurlOutOfMemoryException` | Reserved | - |
| 28 | `CurlOperationTimeoutException` | `-m`/`--max-time` or `--connect-timeout` expired; the transfer stayed below `--speed-limit` for `--speed-time` seconds | Raise the limit; `--retry` |
| 28 | `CurlTimeoutException` | Base type; thrown directly only on a rare fallback path | Same as above |
| 33 | `CurlRangeErrorException` | `-C` resume, but the server sent the whole file (no `Content-Range`) | Download again without `-C` |
| 34 | `CurlHttpPostErrorException` | Reserved | - |
| 12 | `CurlFtpAcceptTimeoutException` | Reserved | - |
| 35 | `CurlSslConnectErrorException` | TLS handshake failed (not a certificate problem): no common TLS version or cipher (`--tlsv1.x`, `--tls-max`, `--ciphers`), the server rejected the client certificate, or the port does not speak TLS | Check the port and the TLS options |
| 36 | `CurlBadDownloadResumeException` | Reserved | - |
| 37 | `CurlFileCouldntReadException` | Single `-d @file`, `--data-binary @file` or `--json @file` missing; `file://` permission denied | Fix the path |
| 41 | `CurlFunctionNotFoundException` | Reserved | - |
| 42 | `CurlAbortedByCallbackException` | A progress callback (`WithProgress` / `ProgressHandler`) or a `CurlSettings.OnRedirect` handler threw; your exception is the `InnerException`. A cancelled token still surfaces as `OperationCanceledException` | Fix or catch inside the callback; throw from `OnRedirect` on purpose to refuse a redirect target |
| 43 | `CurlBadFunctionArgumentException` | CR or LF in a `-H` value or `--request-target`; space in `--request-target`; `-C` with a request body (`-d`/`-F`/`-T`) | One header per `-H`; percent-encode; drop `-C` for uploads |
| 43 | `CurlException` (plain) | `-o` uses `#<name>` with no glob of that name | Name the glob or remove the placeholder |
| 45 | `CurlInterfaceFailedException` | Reserved | - |
| 47 | `CurlTooManyRedirectsException` | More redirects than `--max-redirs` (default 50) | Raise `--max-redirs` or fix the loop |
| 47 | `CurlRedirectException` | Reserved | - |
| 49 | `CurlOptionSyntaxException` | `--resolve` / `--connect-to` entry with a non-numeric port | Use `HOST:PORT:ADDRESS` / `HOST1:PORT1:HOST2:PORT2` |
| 52 | `CurlGotNothingException` | Server (or proxy) closed the connection without sending a byte | Check server logs, port and TLS; `--retry` |
| 55 | `CurlSendErrorException` | Reserved | - |
| 56 | `CurlReceiveErrorException` | Connection failed while reading the response; response headers over 300 KB in total | Retry; server-side problem |
| 53, 54, 59, 64 | `CurlSslEngineNotFoundException`, `CurlSslEngineSetFailedException`, `CurlSslCipherException`, `CurlUseSslFailedException` | Reserved | - |
| 58 | `CurlSslCertificateProblemException` | `--cert`/`--key` file missing, wrong format or password, key does not match (.NET 5+) | Check the path, `--cert-type` and `--pass` |
| 60 | `CurlPeerFailedVerificationException` | Server certificate rejected | Trust the CA with `--cacert` (.NET 5+) or in the OS store; `-k` for development only |
| 60 | `CurlSslException` | Base type, reserved as a concrete type (catch it to cover every TLS failure) | - |
| 61 | `CurlBadContentEncodingException` | `--compressed` body cannot be decoded; invalid `Transfer-Encoding` (chunked not last, unknown coding, more than five, gzip without `--tr-encoding`) | Server bug; drop `--compressed` to keep the raw bytes |
| 63 | `CurlFileSizeExceededException` | `--max-filesize` exceeded (announced or while downloading) | Raise the limit |
| 77 | `CurlSslCaCertBadFileException` | `--cacert` file or `--capath` folder missing, unreadable or without a certificate (.NET 5+) | Fix the path; PEM or DER certificates |
| 90 | `CurlSslPinnedPublicKeyMismatchException` | The server key matches no `--pinnedpubkey` pin, or the pin cannot be read | Update the pin (the message shows the presented key's pin) |
| 67 | `CurlAuthenticationException`, `CurlLoginDeniedException` | Reserved | - |
| 94 | `CurlAuthErrorException` | `--digest`/`--ntlm`/`--negotiate`/`--anyauth` (or `--proxy-*`) could not process the challenge | Try `--basic`; inspect the challenge with `-v` |
| 100 | `CurlTooLargeException` | More than 5000 response header fields, or a header line over 100 KB | Server bug or attack |
| 100 | `CurlException` (plain) | NTLM user name too long | Shorten it |
| none | `CurlUnknownOptionException` | Reserved (unknown options go to `CurlOptions.UnknownOptions`) | `Curl.Validate()` |
| none | `CurlDotNet.Exceptions.CurlHttpException` | `result.EnsureSuccessStatusCode()` on a non-2xx result | Handle the status |
| none | `CurlDotNet.Core.CurlHttpException` | `result.EnsureSuccess()` / `EnsureStatus(code)` | Handle the status |
| none | `CurlRateLimitException` | Reserved | - |
| none | `CurlRetryException` | `CurlDotNet.Middleware.RetryMiddleware` exhausted its attempts on retryable exceptions (`LastAttemptException`) | Fix the underlying failure |
| none | `CurlConnectionException`, `CurlFileException`, `CurlParsingException`, `CurlExecutionException`, `CurlCookieException` | Reserved | - |

The property `CurlException.CurlErrorCode` carries the exit code so that existing scripts keyed on curl numbers keep working. `LibCurl` (the instance API in `CurlDotNet.Lib`) does not throw these: it returns a `CurlResult` whose `Exception` property holds the `CurlException`, and throws only `OperationCanceledException`.

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
catch (CurlCouldntResolveHostException ex) { Console.WriteLine($"Unknown host: {ex.Hostname}"); }
catch (CurlCouldntConnectException ex) { Console.WriteLine($"Unreachable: {ex.Host}"); }
catch (CurlSslException ex) { Console.WriteLine($"TLS problem: {ex.Message}"); }
catch (CurlDotNet.Core.CurlHttpException ex) when (ex.StatusCode == 429) { Console.WriteLine("Rate limited"); }
catch (CurlException ex) { Console.WriteLine($"curl error {ex.CurlErrorCode}: {ex.Message}"); }
catch (OperationCanceledException) { Console.WriteLine("Cancelled"); }
```

### Switch on the exit code

Code that already knows curl's exit codes can switch on `CurlErrorCode` instead of the type:

```csharp
catch (CurlException ex)
{
    var retry = ex.CurlErrorCode switch
    {
        6 or 7 or 28 or 52 or 56 => true,   // DNS, connect, timeout, empty reply, receive error
        _ => false
    };
    Console.WriteLine(retry ? "transient - try again" : $"curl exit {ex.CurlErrorCode}: {ex.Message}");
}
```

For automatic retries, add `--retry N` to the command instead of writing a loop; see [curl --retry in C#](../cookbook/retry.html).

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
- [Transfer and file errors](../exceptions/transfer-errors.html) - exit codes 8, 18, 23, 26, 33, 52, 56, 61, 63, 100 and FTP
- [curl exit codes](../reference/curl-exit-codes.html) - the curl tool's full numbered list
- [Configuration](../getting-started/configuration.html) - timeouts and other defaults
- [Handling Errors tutorial](../tutorials/06-handling-errors.html)
