---
title: "curl Exit Codes in C# - CurlDotNet Exception Reference"
description: "Every curl exit code CurlDotNet produces in C#, the exception type it throws, when it happens and how to fix it: 2, 3, 6, 7, 22, 23, 26, 28, 35, 47, 52, 56, 60, 94 and more."
---

# curl exit codes in C#

The curl tool reports failures with a numeric exit code. CurlDotNet maps every failure to the same number in C#: the exception it throws carries the code in `CurlException.CurlErrorCode`, and the exception type names the error. This page lists every code CurlDotNet produces, which exception you get, and the usual fix.

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -f -m 10 https://httpbin.org/status/404");
}
catch (CurlException ex)
{
    Console.WriteLine($"curl: ({ex.CurlErrorCode}) {ex.Message}");   // curl: (22) ...
}
```

## Success is not an exception

Exit code 0 is a returned `CurlResult`. That includes HTTP error statuses: like curl, a 404 or 500 is a successful transfer unless you add `-f` / `--fail`. Check `result.StatusCode`, or call `result.EnsureSuccess()`, which throws `CurlDotNet.Core.CurlHttpException` for a non-2xx status (a plain `Exception` with a `StatusCode` property, not a `CurlException`).

Cancellation is not an exit code either: a cancelled `CancellationToken` throws the standard `OperationCanceledException`, so `catch (OperationCanceledException)` works as with any other .NET API.

## Exit code table

| Code | curl name | CurlDotNet throws | When | How to fix |
|------|-----------|-------------------|------|-----------|
| 1 | `CURLE_UNSUPPORTED_PROTOCOL` | `CurlUnsupportedProtocolException` | URL scheme with no handler (`sftp://`, `htp://` ...), a `-L` redirect to a non-HTTP scheme, a response that is not HTTP/1.x or HTTP/2 (HTTP/0.9 without `--http0.9`) | Use http, https, ftp or file; `--http0.9` for ancient servers |
| 2 | `CURLE_FAILED_INIT` | `CurlException` | An option is missing its value (`curl -H`), two request methods conflict (`-I` with `-d`, `-F` with `-d`, `--json` with `-F`), a bad `--max-filesize`, `-K` files nested too deeply, a `--variable` error | Fix the command as the message says |
| 3 | `CURLE_URL_MALFORMAT` | `CurlMalformedUrlException`, or `CurlException` | The URL (or a redirect `Location`) cannot be parsed, the command is only `curl` with no URL, a malformed glob such as `[1-` | Fix the URL; escape `{` `[` or use `-g` |
| 5 | `CURLE_COULDNT_RESOLVE_PROXY` | `CurlCouldntResolveProxyException` | The proxy host name does not resolve | Check `-x` and the `http_proxy` / `https_proxy` / `all_proxy` variables |
| 6 | `CURLE_COULDNT_RESOLVE_HOST` | `CurlCouldntResolveHostException` | The host name does not resolve | Check the spelling and DNS; `--resolve host:port:address` |
| 7 | `CURLE_COULDNT_CONNECT` | `CurlCouldntConnectException` | Connection refused or unreachable, to the host or the proxy (`ViaProxy` says which); a refused `-p` tunnel | Start the server, check port and firewall; `--retry-connrefused` |
| 8 | `CURLE_WEIRD_SERVER_REPLY` | `CurlWeirdServerReplyException` | The response head is malformed (bad status line, invalid header line) | Server bug; inspect with `-v` |
| 9 | `CURLE_REMOTE_ACCESS_DENIED` | `CurlDotNet.Core.CurlFtpException` | The FTP server refused the request | Check the path, `-u` and permissions |
| 18 | `CURLE_PARTIAL_FILE` | `CurlPartialFileException` | The connection closed before `Content-Length` bytes (or the last chunk) arrived | Retry, or resume with `-C -` |
| 22 | `CURLE_HTTP_RETURNED_ERROR` | `CurlHttpReturnedErrorException` | `-f` / `--fail` or `--fail-with-body` and a status of 400 or more | Handle the status, or drop `-f` to get a normal result |
| 23 | `CURLE_WRITE_ERROR` | `CurlWriteErrorException` | An `-o`, `-O`, `-D` or `-c` file cannot be written: missing directory, bad name, no permission, `-J` would overwrite | Create the directory or add `--create-dirs`; pick a writable path |
| 26 | `CURLE_READ_ERROR` | `CurlReadErrorException` | An input file is missing or unreadable: `-T`, `-F name=@file`, `--data-urlencode @file`, `-K`, `--variable name@file`, `.netrc` | Fix the path (relative paths are relative to the working directory) |
| 28 | `CURLE_OPERATION_TIMEDOUT` | `CurlOperationTimeoutException` | `-m` / `--max-time` or `--connect-timeout` expired; also `--retry-max-time` running out after a failed attempt (curl reports that attempt's own code instead) | Raise the limit; add `--retry` |
| 33 | `CURLE_RANGE_ERROR` | `CurlRangeErrorException` | `-C` asked to resume but the server sent the whole document | Download again without `-C` |
| 35 | `CURLE_SSL_CONNECT_ERROR` | `CurlSslConnectErrorException` | The TLS handshake failed (not a certificate problem) | Check the port speaks TLS and supports a current TLS version |
| 37 | `CURLE_FILE_COULDNT_READ_FILE` | `CurlFileCouldntReadException` | A single `-d @file`, `--data-binary @file` or `--json @file` is missing (the curl tool reports 26 here); `file://` permission denied | Fix the path |
| 43 | `CURLE_BAD_FUNCTION_ARGUMENT` | `CurlBadFunctionArgumentException`, or `CurlException` | CR or LF inside a `-H` value or `--request-target`, `-C` with an upload (curl ignores `-C` there), `#name` in `-o` with no such glob | One header per `-H`; drop `-C` for uploads |
| 47 | `CURLE_TOO_MANY_REDIRECTS` | `CurlTooManyRedirectsException` | More redirects than `--max-redirs` (default 50; `-1` is unlimited) | Raise `--max-redirs` or fix the redirect loop |
| 49 | `CURLE_SETOPT_OPTION_SYNTAX` | `CurlOptionSyntaxException` | A `--resolve` or `--connect-to` entry is malformed | Use `HOST:PORT:ADDRESS` / `HOST1:PORT1:HOST2:PORT2` |
| 52 | `CURLE_GOT_NOTHING` | `CurlGotNothingException` | The server accepted the request and closed without a byte | Check the server, the port and http vs https |
| 56 | `CURLE_RECV_ERROR` | `CurlReceiveErrorException` | The connection failed while reading the response, or the headers exceed 300 KB | Retry; server-side problem |
| 60 | `CURLE_PEER_FAILED_VERIFICATION` | `CurlPeerFailedVerificationException` | The server certificate was rejected (expired, wrong name, untrusted CA) | Trust the CA in the operating system store; `-k` only for development |
| 61 | `CURLE_BAD_CONTENT_ENCODING` | `CurlBadContentEncodingException` | A `--compressed` body cannot be decoded, or the `Transfer-Encoding` is invalid | Server bug; drop `--compressed` to get the raw bytes |
| 63 | `CURLE_FILESIZE_EXCEEDED` | `CurlFileSizeExceededException` | The response is larger than `--max-filesize` | Raise the limit |
| 94 | `CURLE_AUTH_ERROR` | `CurlAuthErrorException` | `--digest`, `--ntlm`, `--negotiate`, `--anyauth` (or a `--proxy-` variant) could not process the challenge | Try `--basic`; look at the challenge with `-v`. A wrong password is a normal 401 result, not this error |
| 100 | `CURLE_TOO_LARGE` | `CurlTooLargeException` | More than 5000 response header fields, or a header line over 100 KB | Server bug or hostile server |

Exceptions of the same family share base types, so you can catch broadly: `CurlTimeoutException` for 28, `CurlSslException` for 35 and 60, `CurlDotNet.Exceptions.CurlHttpException` for 22, and `CurlException` for everything.

One case has no code: a command with options but no URL (`curl -s`) throws `CurlInvalidCommandException` with `CurlErrorCode` null, where the curl tool exits with 2.

## When does a code stop the command?

With several URLs in one command, a failed transfer does not stop the others, and the outcome is that of the last transfer, as in curl. The command-line errors 2 and 43 always stop it, and `--fail-early` stops at the first failure. See [multiple URLs and URL globbing](../cookbook/multiple-urls-and-globbing.html).

## What curl would have printed

curl writes as it goes, so before it exits with an error code it may already have printed headers (`-i`), an error page (`--fail-with-body`), earlier URLs' bodies and `-w` text. CurlDotNet attaches that text to the exception: `ex.StandardOutput` is the complete output, and `ex.Data[CurlException.WriteOutDataKey]` the `-w` part alone.

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl -i -f -w '\\nexit=%{exitcode}' https://httpbin.org/status/418");
}
catch (CurlHttpReturnedErrorException ex)
{
    Console.WriteLine(ex.StandardOutput);   // HTTP/1.1 418 ... headers, then "exit=22"
}
```

## Switch on the code

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl -f --connect-timeout 5 https://httpbin.org/get");
}
catch (CurlException ex)
{
    var advice = ex.CurlErrorCode switch
    {
        6 or 5 => "Check the host or proxy name.",
        7 => "The server is not accepting connections.",
        28 => "Too slow: raise -m or add --retry.",
        35 or 60 => "TLS problem: check the certificate and the port.",
        22 => "The server answered with an error status.",
        _ => ex.Message
    };
    Console.WriteLine(advice);
}
```

## Codes CurlDotNet does not produce

curl defines more than 90 codes. The ones not in the table belong to protocols or features CurlDotNet does not implement (SSH, SMTP, TFTP, TELNET, LDAP, client certificates) and are never returned. Some exception types exist for them so that code written against curl's numbering compiles; the [error reference](../troubleshooting/error-reference.html) lists every type, including those reserved ones.

## Related

- [Error reference: every CurlDotNet exception](../troubleshooting/error-reference.html)
- [Handle curl errors and exceptions in C#](../cookbook/beginner/handle-errors.html)
- [curl --retry in C#](../cookbook/retry.html)
- [libcurl mapping](libcurl-mapping.html)
- [Reference index](index.html)
