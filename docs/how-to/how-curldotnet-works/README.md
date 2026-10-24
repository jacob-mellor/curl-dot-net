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
        |   3. expand $VAR, ${VAR}, %VAR%, $env:VAR
        |   4. split into arguments using shell quoting rules
        |   5. map each flag onto a property
        v
  CurlOptions               (URL, method, headers, data, auth, proxy, timeouts, output...)
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
- `'{\"key\":\"value\"}'`, a Windows quoting habit, is unescaped when the quoted text has no bare `"` in it. This is in the repository's current source but **not** in the 9.7.0 package, which sends the backslashes literally.
- Whitespace inside quotes is kept exactly, so JSON payloads and file paths with double spaces arrive unchanged.

You can see the result without sending anything:

```csharp
using CurlDotNet;

var check = Curl.Validate("curl -X PUT -H 'X-Id: 7' -d 'a=1' https://httpbin.org/put");
Console.WriteLine(check.IsValid);                  // True
Console.WriteLine(check.ParsedOptions.Method);     // PUT
Console.WriteLine(check.ParsedOptions.Headers["X-Id"]);  // 7
```

## Sending with HttpClient

- **One shared client.** The static `Curl` class owns one engine and one `HttpClient`, so connections are pooled across calls. Each `LibCurl` instance owns its own.
- **Redirects are curl's, not .NET's.** The handler is created with `AllowAutoRedirect = false`, and CurlDotNet follows `Location` itself only when `-L` is present, honouring `--max-redirs`.
- **Cookies are opt-in.** `UseCookies = false`, so nothing leaks between unrelated requests; `-b` sends exactly the cookies you give it.
- **Per-request transports.** `-x`, `--socks5`, `-k`, `--digest`, `--ntlm`, `--negotiate` and `--anyauth` get a dedicated `HttpClientHandler` configured for that one request.
- **Timeouts.** `-m` becomes a linked `CancellationTokenSource` around sending the request and receiving the response headers; the body is then read in full. When there is no `-m` and no `Curl.DefaultMaxTimeSeconds`, 30 seconds applies.

## Text or binary?

The body is always read as bytes first. CurlDotNet then decides whether to decode it as text from the `Content-Type` (with a registry of known binary types such as Office Open XML, PDF and ZIP) plus a byte-level sniff, and a missing `Content-Type` counts as binary. Text goes to `result.Body`; everything else goes to `result.BinaryData`, unchanged. `-o` writes whichever one applies.

## Errors as curl exit codes

.NET exceptions are mapped to curl's error codes: an `HttpRequestException` becomes `CurlCouldntConnectException` (7), a timeout becomes `CurlOperationTimeoutException` (28), and so on. HTTP status codes are data, not exceptions, as with the curl CLI without `-f`. See [error handling](../curl-error-handling-csharp/README.md).

## Code generation

`Curl.ToHttpClient`, `ToFetch`, `ToPythonRequests` and `ToPowershellCode` run the same parser and print code from the resulting `CurlOptions` instead of sending it. See [convert curl to C#](../convert-curl-to-csharp/README.md).

## Packaging

One assembly targeting netstandard2.0, net472, net48, net8.0, net9.0 and net10.0 (version 9.7.0 on nuget.org ships net472, net48, net8.0, net9.0 and netstandard2.0). JSON goes through System.Text.Json, or Newtonsoft.Json on netstandard2.0. MIT licensed, by Jacob Mellor, sponsored by Iron Software. The source for each stage is in `src/CurlDotNet/Core/` in the repository.

## Related

- [Run a curl command in C#](../run-curl-command-in-csharp/README.md)
- [CurlDotNet vs HttpClient](../curl-vs-httpclient-csharp/README.md)
- [curl vs Invoke-WebRequest (quoting rules)](../../powershell/curl-vs-invoke-webrequest/README.md)
- [All how-to guides](../README.md)
