---
title: "How CurlDotNet Works: curl Commands in Pure C#"
description: "How CurlDotNet works: a C# command parser turns curl strings into CurlOptions, then an HttpClient-based engine sends them. No libcurl, no curl.exe."
---

# How CurlDotNet works

CurlDotNet works by parsing a curl command string into a `CurlOptions` object in C#, then sending that request with .NET's own `HttpClient` (or `FtpWebRequest` and the file system for `ftp://` and `file://`). There is no native libcurl, no curl.exe, no `Process.Start` and no P/Invoke: it is managed code on every target framework.

```bash
dotnet add package CurlDotNet
```

## The pipeline

```text
"curl -X POST -H ... -d ... https://..."
        |
        v
  CommandParser.Parse       (CurlDotNet.Core.CommandParser)
        |   1. join line continuations: \  `  ^
        |   2. drop the leading "curl"
        |   3. expand $VAR, ${VAR}, %VAR%, $env:VAR (not inside single quotes)
        |   4. split into arguments using shell quoting rules
        |   5. map each flag onto a property (curl 8.7.1's full option table,
        |      -K config files, --variable, .netrc)
        |   6. expand URL globs and split URLs / --next groups into transfers
        v
  CurlOptions               (URL, method, headers, data, auth, proxy, timeouts, output...;
        |                    further transfers in AdditionalTransfers)
        |
        v
  CurlEngine                picks a handler from the URL scheme
        |-- http / https -> HttpHandler  (HttpClient)
        |-- ftp / ftps   -> FtpHandler   (FtpWebRequest)
        |-- file         -> FileHandler  (System.IO)
        v
  CurlResult                (StatusCode, Headers, Body or BinaryData, OutputFiles, Timings)
```

`CurlRequestBuilder` and `LibCurl` build a `CurlOptions` directly and skip the parser. `Curl.ExecuteAsync(CurlOptions)` takes one too.

## Parsing like a shell

The parser is modelled on curl's own argument handling (`src/tool_getparam.c`) and on how shells split words:

- Single quotes, double quotes and backslash escapes behave as in bash, so commands from Linux and macOS docs work.
- PowerShell backtick and cmd.exe caret continuations are joined, so Windows docs work too.
- `'{\"key\":\"value\"}'`, a Windows quoting habit, is unescaped when the quoted text has no bare `"` in it.
- Whitespace inside quotes is kept exactly, so JSON payloads and file paths with double spaces arrive unchanged.
- Environment variables expand only where a shell would expand them: unquoted or in double quotes, never inside single quotes. An unset variable is left as typed.
- Combined short flags (`-sSL`), attached values (`-m10`, `-ofile`), `--long=value` and `--` work as in curl. Several URLs, `--next` groups and globs such as `[1-3]` or `{a,b}` become one transfer each, run in order; see [multiple URLs and globbing](../../cookbook/multiple-urls-and-globbing.md). `-K` config files and `--variable` are covered in [config files and variables](../../cookbook/config-files-and-variables.md).
- An option curl knows but CurlDotNet does not implement is recorded in `CurlOptions.UnsupportedOptions`; a typo goes to `CurlOptions.UnknownOptions` and makes `Curl.Validate` fail.

You can see the result without sending anything:

```csharp
using CurlDotNet;

var check = Curl.Validate("curl -X PUT -H 'X-Id: 7' -d 'a=1' https://httpbin.org/put");
Console.WriteLine(check.IsValid);                  // True
Console.WriteLine(check.ParsedOptions.Method);     // PUT
Console.WriteLine(check.ParsedOptions.Headers["X-Id"]);  // 7
```

## Sending with HttpClient

- **One shared client.** The static `Curl` class owns one engine and one `HttpClient`, so connections are pooled across calls. Each `LibCurl` instance owns its own. On .NET 5 and later the handler is a `SocketsHttpHandler` with a connect callback (for `--connect-timeout`, `--resolve`, `--connect-to` and `-p` tunnels); on netstandard2.0 and .NET Framework it is an `HttpClientHandler`.
- **Default headers are curl's.** Every request carries `User-Agent: curl/8.7.1` and `Accept: */*` unless you override or remove them.
- **Redirects are curl's, not .NET's.** The handler is created with `AllowAutoRedirect = false`, and CurlDotNet follows `Location` itself only when `-L` is present, with curl's rules: `--max-redirs`, POST becoming GET on 301/302/303 (unless `--post301`/`--post302`/`--post303`), method and body kept on 307/308, and `Authorization` and cookies dropped when the host changes unless `--location-trusted`.
- **Cookies are opt-in.** `UseCookies = false`, so nothing leaks between unrelated requests. `-b 'name=value'` sends exactly the cookies you give it; a cookie engine with curl's domain, path, secure and expiry rules runs only for `-b FILE`, `-c JAR` or `-j`.
- **Authentication is negotiated in-library.** Basic, Digest, NTLM (v1 and v2, in pure C#) and `--anyauth` follow curl's own negotiation, including the `Content-Length: 0` probe for POST and PUT. Only `--negotiate` (Kerberos/SPNEGO) is left to the platform.
- **Per-request transports.** A proxy (from `-x`, `--socks5` or the `http_proxy`/`https_proxy`/`all_proxy` environment variables), `-k`, challenge auth, `--resolve`, `--connect-to` and `-p` get a dedicated handler configured for that one request.
- **Timeouts.** The `HttpClient` timeout is infinite. `-m` becomes a linked `CancellationTokenSource` around the whole transfer, body included; without `-m` or `Curl.DefaultMaxTimeSeconds` there is no limit, as in curl.
- **Compression is decoded by CurlDotNet.** `--compressed` sends `Accept-Encoding` and decodes gzip, deflate and (on .NET Core and later) Brotli itself; the `Content-Encoding` and `Content-Length` headers stay as received.
- **Responses are read off the wire.** On .NET 5 and later the raw response bytes are recorded, so `-i`, `-D`, `-I` and `result.Headers` show headers exactly as the server sent them, and empty replies, truncated bodies and malformed headers map to curl's exit codes.
- **Retries and output.** `--retry` re-runs a transfer on timeouts and transient statuses; `-o`, `-O`, `-D` and the other output flags write files the way curl's tool does.

## Text or binary?

The body is always read as bytes first. CurlDotNet then decides whether to decode it as text from the `Content-Type` (with a registry of known binary types such as Office Open XML, PDF and ZIP) plus a byte-level sniff, and a missing `Content-Type` counts as binary. Text goes to `result.Body`; everything else goes to `result.BinaryData`, unchanged. `-o` writes whichever one applies.

## Errors as curl exit codes

.NET exceptions are mapped to curl's error codes by what actually failed: a name that does not resolve becomes `CurlCouldntResolveHostException` (6), a refused connection `CurlCouldntConnectException` (7), a rejected certificate `CurlPeerFailedVerificationException` (60), a timeout `CurlOperationTimeoutException` (28), an empty reply `CurlGotNothingException` (52), and so on. HTTP status codes are data, not exceptions, as with the curl CLI; `-f` turns 400 and above into `CurlHttpReturnedErrorException` (22). A cancelled `CancellationToken` surfaces as `OperationCanceledException`. See [error handling](../curl-error-handling-csharp/README.md) and the [exit code reference](../../reference/curl-exit-codes.md).

## Code generation

`Curl.ToHttpClient`, `ToFetch`, `ToPythonRequests` and `ToPowershellCode` run the same parser and print code from the resulting `CurlOptions` instead of sending it. See [convert curl to C#](../convert-curl-to-csharp/README.md).

## Packaging

One assembly targeting netstandard2.0, net472, net48, net8.0, net9.0 and net10.0. JSON goes through System.Text.Json, or Newtonsoft.Json on netstandard2.0. On netstandard2.0 and .NET Framework, CurlDotNet works from `HttpClient`'s parsed headers instead of the raw bytes, so a few wire-level features (`--request-target`, removing `Host`, blank headers, `--raw`, `-p` tunnels and a separate `--connect-timeout`) are not available there. MIT licensed, by Jacob Mellor, sponsored by Iron Software. The source for each stage is in `src/CurlDotNet/Core/` in the repository.

## Related

- [Run a curl command in C#](../run-curl-command-in-csharp/README.md)
- [CurlDotNet vs HttpClient](../curl-vs-httpclient-csharp/README.md)
- [curl vs Invoke-WebRequest (quoting rules)](../../powershell/curl-vs-invoke-webrequest/README.md)
- [All how-to guides](../README.md)
