---
title: "curl flavors: bash, Windows CMD, PowerShell and Invoke-WebRequest in C#"
description: "Run curl commands copied from bash, Windows CMD, PowerShell or Invoke-WebRequest in C# with CurlDotNet's CurlFlavor: the quoting, escaping and variable rules of each shell, and how Auto detects them."
---

# curl flavors: bash, Windows CMD, PowerShell and Invoke-WebRequest in C#

A curl command copied from a terminal is shell text, not curl syntax. bash, the Windows Command Prompt and PowerShell each quote, escape, continue lines and expand variables differently, so the same characters can mean different bytes on the wire. `-d '{\"a\":1}'` sends `{\"a\":1}` (backslashes included) from bash, and `{"a":1}` from PowerShell running `curl.exe`. PowerShell's own `curl` alias is not curl at all: it is `Invoke-WebRequest`, with `-Uri`, `-Headers @{...}` and `-Body` parameters.

CurlDotNet names these as a **flavor** (`CurlDotNet.CurlFlavor`). By default (`CurlFlavor.Auto`) it recognises where a command came from and reads it accordingly. When you know where your commands come from, say so and CurlDotNet applies that shell's exact rules.

```bash
dotnet add package CurlDotNet
```

```csharp
using CurlDotNet;

// Copied from a Windows PowerShell answer on Stack Overflow
var result = await Curl.ExecuteAsync(
    @"curl.exe -X POST https://api.example.com/items -H 'Content-Type: application/json' -d '{\""name\"":\""Ada\""}'",
    CurlFlavor.PowerShell);
// sends the body {"name":"Ada"}
```

## The flavors

| | `Posix` | `WindowsCmd` | `PowerShell` | `InvokeWebRequest` |
|---|---|---|---|---|
| Where the command came from | bash, zsh, sh on macOS and Linux, Git Bash, WSL, CI scripts, browser "Copy as cURL (bash)" | `curl.exe` in the Windows Command Prompt, batch files, browser "Copy as cURL (cmd)" | `curl.exe` in PowerShell 7 or Windows PowerShell 5.1 | `Invoke-WebRequest`, `Invoke-RestMethod` and their `curl`, `wget`, `iwr`, `irm` aliases |
| Single quotes `'...'` | Literal, nothing is special inside | Ordinary characters, they do not quote anything | Literal; `''` inside is one `'` | Literal; `''` inside is one `'` |
| Double quotes `"..."` | Group; `\"` `\\` `\$` `` \` `` are escapes, other backslashes are kept; `$VAR` expands | Group; `\"` and `""` are a literal quote (C runtime rule) | Group; `` `" `` and `""` are a quote, `` `n `` `` `t `` etc. are control characters; `$env:VAR` expands | Same as PowerShell |
| Escape character | `\` (outside quotes it escapes any character) | `^` outside double quotes | `` ` `` (backtick) | `` ` `` (backtick) |
| Line continuation | `\` at the end of a line | `^` at the end of a line | `` ` `` at the end of a line | `` ` `` at the end of a line |
| Variables | `$VAR`, `${VAR}` outside single quotes; unquoted values split into words | `%VAR%` everywhere, even inside quotes; a caret cannot stop it | `$env:VAR`, `${env:VAR}`, `$VAR` (as an environment variable); `%VAR%` does not expand | Same as PowerShell |
| Backslash | Escape | Path separator; only special right before a `"` | Not a PowerShell escape, but `\"` in an argument without bare quotes becomes `"` (see below) | Ordinary character |
| Other | `$'...'` ANSI-C strings, `#` comments; `\|` `;` `&` `>` `<` end the command | `&` `\|` `<` `>` end the command | `--%` stop-parsing, here-strings `@'...'@`, `#` comments, smart quotes; `\|` `;` `>` `&&` end the command; `&` call operator and `$r =` are removed | Cmdlet parameters are translated to curl options |
| `-d '{\"a\":1}'` sends | `{\"a\":1}` | `'{"a":1}'` (single quotes sent, they are not quotes) | `{"a":1}` | n/a |

Every flavor leaves an environment variable that is not set exactly as typed (`$TOKEN`, `%TOKEN%`, `$env:TOKEN`), where a real shell would substitute an empty string. That way a missing variable is visible in the request and in `Curl.Validate`, instead of silently disappearing.

### Why PowerShell commands escape quotes with a backslash

Windows PowerShell 5.1 (and PowerShell 7.2 and older) builds a command line for `curl.exe`, and the Microsoft C runtime inside `curl.exe` splits it again. Bare double quotes are eaten in that second step, so Windows documentation writes `-d '{\"key\":\"value\"}'`. PowerShell 7.3+ passes arguments through intact, so commands written for it use bare quotes: `-d '{"key":"value"}'`.

The `PowerShell` flavor reads each argument the way it was evidently written:

- an argument with **no bare double quote** has every `\"` turned into `"` (and `\\\"` into `\"`), the 5.1 reading;
- an argument that **contains a bare double quote** is sent as typed, `\"` included, the 7.3+ reading. That keeps JSON escapes such as `{"say":"\"hi\""}` intact.

Either way the bytes the author intended are sent.

### What InvokeWebRequest translates

`-Uri` (or the first positional argument), `-Method` (`Head` becomes `-I`), `-CustomMethod`, `-Headers @{ 'Name' = 'Value'; ... }`, `-Body` (a string as is; a hashtable URL-encoded as `name=value&...`, or appended to the query string for GET), `-ContentType`, `-InFile`, `-Form @{ name = 'value'; file = Get-Item ./a.png }` (multipart), `-OutFile`, `-UserAgent`, `-Proxy`, `-NoProxy`, `-TimeoutSec`, `-ConnectionTimeoutSeconds`, `-MaximumRedirection`, `-MaximumRetryCount`, `-RetryIntervalSec`, `-SkipCertificateCheck`, `-Authentication Bearer -Token ...` (also `OAuth`, and `(ConvertTo-SecureString 'x' -AsPlainText -Force)`), `-HttpVersion`, `-SslProtocol`, `-PreserveAuthorizationOnRedirect`, `-PreserveHttpMethodOnRedirect`, `-Resume` and `-TransferEncoding`. Parameter names are case-insensitive and may be shortened while the prefix is unique (`-Meth Post`).

The cmdlet's own defaults are kept: redirects are followed up to 5 times (`-MaximumRedirection 0` turns that off), URLs are never globbed, the method stays GET when only `-Body` is given, and header order is the order you wrote them. CurlDotNet sends curl's default `User-Agent` (`curl/8.7.1`), not PowerShell's, unless you give `-UserAgent`.

Ignored, because they do not change the request: `-UseBasicParsing`, `-DisableKeepAlive`, `-PassThru`, `-SkipHeaderValidation`, `-SkipHttpErrorCheck`, `-AllowUnencryptedAuthentication`, `-StatusCodeVariable`, `-ResponseHeadersVariable`, and common parameters such as `-Verbose`.

Refused with a `CurlInvalidCommandException` that says what to write instead:

| Parameter | Why | Write instead |
|---|---|---|
| `-Credential`, `-Authentication Basic` | A `PSCredential` object only exists inside PowerShell | `curl -u user:pass https://...` |
| `-ProxyCredential` | Same | `curl -x http://proxy:8080 -U user:pass ...` |
| `-UseDefaultCredentials` | Signs in as the current Windows user | `curl --negotiate -u : ...`, or a `CurlSettings` with your own handler |
| `-SessionVariable`, `-WebSession` | A `WebRequestSession` variable does not exist outside PowerShell | `-c cookies.txt` then `-b cookies.txt` |
| `-Certificate`, `-CertificateThumbprint` | Certificate objects and the Windows store | `--cert client.pem --key client.key` |
| `-FollowRelLink` | A loop over several requests | Read `Link` and request the next page yourself |
| Values held in variables (`-Headers $h`), `$( ... )`, `(... \| ConvertTo-Json)` | PowerShell code cannot be evaluated from a string | Paste the literal value |

## How Auto picks a flavor

`CurlFlavor.Auto` is the default. It checks for telltales in this order and the first match wins:

1. **InvokeWebRequest** when the first word is `Invoke-WebRequest`, `Invoke-RestMethod`, `iwr`, `irm` or `wget`, or when the first word is `curl` (not `curl.exe` or a path) or missing and an unquoted word is a cmdlet parameter such as `-Uri`, `-Method`, `-Headers`, `-Body`, `-ContentType`, `-OutFile` or `-UseBasicParsing`.
2. **PowerShell** when a line ends with a backtick, or the text contains `$env:`, `` `" `` or the `--%` token.
3. **WindowsCmd** when a line ends with `^`, the text contains `^"`, or it contains `%NAME%` where `NAME` is a set environment variable (so `%20` in a URL never counts).
4. **Posix** otherwise.

`Auto` then reads the command permissively, accepting all three shells at once, so slightly mixed commands still work: single and double quotes both group; `\"`, `\'`, `\\`, `\$` and a backslash before a space are escapes and any other backslash is kept (Windows paths survive); `""` inside double quotes is a quote; `\`, `^` and backtick line continuations all work; `$VAR`, `${VAR}` and `$env:VAR` expand outside single quotes and `%VAR%` expands when set; and a single-quoted value containing `\"` but no bare `"` has its `\"` unescaped, because that is only ever written for Windows. When the text contains a construct only one shell's exact rules can read (`^"` for cmd, `` `" `` or `--%` for PowerShell), Auto uses that flavor's exact rules. Invoke-WebRequest syntax is always translated.

Use `Curl.DetectFlavor(command)` to see the choice without running anything, and `CurlOptions.Flavor` (for example on `Curl.Validate(command).ParsedOptions`) to see the flavor that was used.

## How to force a flavor

```csharp
using CurlDotNet;
using CurlDotNet.Core;

// one call
var r1 = await Curl.ExecuteAsync(pasted, CurlFlavor.WindowsCmd);
var r2 = Curl.Execute(pasted, CurlFlavor.WindowsCmd);                  // blocking
var r3 = await Curl.ExecuteAsync(pasted, CurlFlavor.WindowsCmd, cancellationToken);

// together with other settings
var r4 = await Curl.ExecuteAsync(pasted, new CurlSettings().WithFlavor(CurlFlavor.Posix).WithTimeout(30));

// for the whole process (for example, every command comes from Windows users)
Curl.DefaultFlavor = CurlFlavor.WindowsCmd;

// check first, without sending
var check = Curl.Validate(pasted, CurlFlavor.PowerShell);
Console.WriteLine($"{check.IsValid} {check.ParsedOptions?.Flavor} {check.ParsedOptions?.Data}");

// what Auto would choose
Console.WriteLine(Curl.DetectFlavor(pasted));
```

The precedence is: the flavor passed to the call, then `CurlSettings.Flavor`, then `Curl.DefaultFlavor`. Choose `Posix` when you want exact bash behaviour and none of Auto's Windows conveniences.

## Match the operating system: `CurlFlavor.Native`

The default stays `Auto`, which reads a command from any shell. When the commands come from people typing at a terminal on the same machine, for example a command-line tool or an admin page that runs a pasted curl command, you may want exactly the rules that machine's shell applies instead. `CurlFlavor.Native` does that:

| Operating system | `Native` reads commands as |
|---|---|
| Windows | `WindowsCmd` (cmd.exe and the C runtime, the shell `curl.exe`'s documentation and "Copy as cURL (cmd)" target) |
| Linux, macOS, containers, everything else | `Posix` (bash rules) |

```csharp
Curl.DefaultFlavor = CurlFlavor.Native;   // or per call: Curl.ExecuteAsync(pasted, CurlFlavor.Native)

var result = await Curl.ExecuteAsync("curl -d '{\"a\":1}' https://api.example.com");
Console.WriteLine(result.Command);       // the command as typed
// result came from bash rules on Linux/macOS, cmd.exe rules on Windows
```

The operating system is checked each time a command is parsed, and `CurlOptions.Flavor` reports the flavor actually used (`WindowsCmd` or `Posix`, never `Native`). Nothing in the text changes the choice, unlike `Auto`. On Windows, commands typed into PowerShell follow PowerShell's rules, so pass `CurlFlavor.PowerShell` for those.

## Worked examples: one intent, four ways, the same bytes

Each example below is the same request written for bash, CMD, PowerShell (`curl.exe`) and Invoke-WebRequest. CurlDotNet sends byte-for-byte the same request for all four, with the matching flavor and also with `Auto`; the test `SameIntent_FromBashCmdPowerShellAndInvokeWebRequest_SendsIdenticalBytes` in `tests/CurlDotNet.Tests/ParityFlavorTests.cs` checks exactly these commands against an in-process server.

### 1. POST JSON

```bash
# bash / zsh
curl -X POST 'https://api.example.com/api' -H 'Content-Type: application/json' -d '{"name":"Ada","n":1}'
```

```bat
:: Windows CMD
curl -X POST "https://api.example.com/api" -H "Content-Type: application/json" -d "{\"name\":\"Ada\",\"n\":1}"
```

```powershell
# PowerShell, curl.exe
curl.exe -X POST 'https://api.example.com/api' -H 'Content-Type: application/json' -d '{\"name\":\"Ada\",\"n\":1}'
```

```powershell
# Invoke-WebRequest syntax
Invoke-RestMethod -Uri 'https://api.example.com/api' -Method Post -ContentType 'application/json' -Body '{"name":"Ada","n":1}'
```

All four send:

```http
POST /api HTTP/1.1
Host: api.example.com
User-Agent: curl/8.7.1
Accept: */*
Content-Type: application/json
Content-Length: 20

{"name":"Ada","n":1}
```

### 2. Bearer token from an environment variable

```bash
curl -H "Authorization: Bearer $API_TOKEN" 'https://api.example.com/api'
```

```bat
curl -H "Authorization: Bearer %API_TOKEN%" "https://api.example.com/api"
```

```powershell
curl.exe -H "Authorization: Bearer $env:API_TOKEN" 'https://api.example.com/api'
```

```powershell
iwr 'https://api.example.com/api' -Headers @{ Authorization = "Bearer $env:API_TOKEN" }
```

With `API_TOKEN=t0k3n`, all four send:

```http
GET /api HTTP/1.1
Host: api.example.com
Authorization: Bearer t0k3n
User-Agent: curl/8.7.1
Accept: */*
```

### 3. A multi-line command

```bash
curl -X PUT \
  -H 'Accept: application/json' \
  -d 'v=1' \
  'https://api.example.com/api'
```

```bat
curl -X PUT ^
  -H "Accept: application/json" ^
  -d "v=1" ^
  "https://api.example.com/api"
```

```powershell
curl.exe -X PUT `
  -H 'Accept: application/json' `
  -d 'v=1' `
  'https://api.example.com/api'
```

```powershell
Invoke-WebRequest -Uri 'https://api.example.com/api' `
  -Method Put `
  -Headers @{ Accept = 'application/json' } `
  -Body 'v=1'
```

All four send:

```http
PUT /api HTTP/1.1
Host: api.example.com
Accept: application/json
User-Agent: curl/8.7.1
Content-Type: application/x-www-form-urlencoded
Content-Length: 3

v=1
```

### 4. A single quote inside the data

```bash
curl -X POST -d 'name=O'\''Brien' 'https://api.example.com/api'
```

```bat
curl -X POST -d "name=O'Brien" "https://api.example.com/api"
```

```powershell
curl.exe -X POST -d "name=O'Brien" 'https://api.example.com/api'
```

```powershell
irm 'https://api.example.com/api' -Method Post -Body 'name=O''Brien'
```

All four send:

```http
POST /api HTTP/1.1
Host: api.example.com
User-Agent: curl/8.7.1
Accept: */*
Content-Type: application/x-www-form-urlencoded
Content-Length: 12

name=O'Brien
```

(`curl.exe -d 'name=O''Brien'` is also correct PowerShell; use `CurlFlavor.PowerShell` for it, because to `Auto` it looks like the bash text `'name=O''Brien'`, which bash reads as `name=OBrien`.)

### 5. A query string with `&` and a custom User-Agent

```bash
curl -A 'Probe/1.0' 'https://api.example.com/api?a=1&b=two'
```

```bat
curl -A "Probe/1.0" "https://api.example.com/api?a=1&b=two"
```

```powershell
curl.exe -A 'Probe/1.0' 'https://api.example.com/api?a=1&b=two'
```

```powershell
iwr 'https://api.example.com/api?a=1&b=two' -UserAgent 'Probe/1.0'
```

All four send:

```http
GET /api?a=1&b=two HTTP/1.1
Host: api.example.com
User-Agent: Probe/1.0
Accept: */*
```

Unquoted, the `&` ends the command in every shell (bash runs it in the background, cmd and PowerShell start a new command), and CurlDotNet's exact flavors do the same. In CMD you can also escape it as `^&` outside quotes.

### 6. The same text, different bytes

Sometimes the identical text is correct in two shells but means different things. These are the cases to know:

| Text | `Posix` | `WindowsCmd` | `PowerShell` | `Auto` |
|---|---|---|---|---|
| `-d '{\"a\":1}'` | `{\"a\":1}` | `'{"a":1}'` | `{"a":1}` | `{"a":1}` |
| `-d "{\"a\":1}"` | `{"a":1}` | `{"a":1}` | `{\a\:1}` (a backslash is not a PowerShell escape, so the string ends at the second `"`; write `` "{`"a`":1}" ``) | `{"a":1}` |
| `-H "X: %TOKEN%"` (TOKEN set) | `X: %TOKEN%` | `X: <value>` | `X: %TOKEN%` | `X: <value>` |
| `-H "X: $TOKEN"` (TOKEN set) | `X: <value>` | `X: $TOKEN` | `X: <value>` | `X: <value>` |
| `-o C:\temp\a.txt` | `C:tempa.txt` | `C:\temp\a.txt` | `C:\temp\a.txt` | `C:\temp\a.txt` |

These values are what `Curl.Validate(text, flavor).ParsedOptions` reports; try your own command the same way.

## Related

- [Run a curl command in C#](../run-curl-command-in-csharp/README.md)
- [POST JSON](../curl-post-json-csharp/README.md)
- [curl vs Invoke-WebRequest vs Invoke-RestMethod](../../powershell/curl-vs-invoke-webrequest/README.md)
- [Using CurlDotNet from PowerShell](../../powershell/README.md)
- [All how-to guides](../README.md)

## About CurlDotNet

CurlDotNet is a pure C# library (no curl.exe, no native libcurl, no P/Invoke) for netstandard2.0, .NET Framework 4.7.2 and 4.8, and .NET 8, 9 and 10. It is MIT licensed, written by Jacob Mellor and sponsored by Iron Software. Source: [github.com/jacob-mellor/curl-dot-net](https://github.com/jacob-mellor/curl-dot-net).
