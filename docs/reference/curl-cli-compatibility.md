---
title: curl CLI Flags Supported in C# - CurlDotNet Compatibility
description: Reference of the curl CLI flags supported in C# by CurlDotNet, the flags not yet implemented, and how shell quoting on Linux, macOS and Windows is parsed.
---

# curl CLI Flags Supported in C# (CurlDotNet Compatibility Reference)

This page lists the curl CLI flags supported in C# by CurlDotNet's command parser, the flags that are not implemented yet, and how shell syntax from Linux, macOS and Windows terminals is normalized before a command runs. The leading `curl` word is optional in every command string.

## Supported Flags

| Area | Flags | Notes |
|------|-------|-------|
| Method and body | `-X/--request`, `-d/--data`, `--data-raw`, `--data-binary`, `--data-urlencode`, `--json` | `-d @file` reads the file; a missing file throws `CurlFileCouldntReadException` |
| Forms | `-F/--form` | `field=value` and `file=@path` parts; builds `multipart/form-data` |
| Headers and identity | `-H/--header`, `-A/--user-agent`, `-e/--referer`, `-b/--cookie 'a=1; b=2'` | `-b` accepts a cookie string only (cookie jar files are coming) |
| Authentication | `-u/--user`, `--basic`, `--digest`, `--ntlm`, `--negotiate`, `--anyauth`, `--aws-sigv4` | Bearer tokens go in `-H 'Authorization: Bearer ...'` |
| Redirects | `-L/--location`, `--max-redirs` | Exceeding `--max-redirs` throws `CurlTooManyRedirectsException` |
| TLS | `-k/--insecure` | Disables certificate validation |
| Proxy | `-x/--proxy`, `--proxy-user`, `--socks5` | |
| Timeouts | `-m/--max-time` | Expiry throws `CurlOperationTimeoutException`; `--connect-timeout` is parsed but only enforced for FTP |
| Output | `-o/--output`, `-O/--remote-name`, `-i/--include`, `-I/--head`, `-w/--write-out` | `-w` supports `%{http_code}`, `%{size_download}`, `%{url_effective}`, `%{content_type}`; the formatted text is appended to `result.Body` |
| Diagnostics | `-v/--verbose`, `--trace <file>`, `--trace-ascii <file>`, `-s/--silent` | |
| Transfer | `--compressed`, `-r/--range` | |

### Example

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(@"curl -X POST https://httpbin.org/post \
    -H 'Content-Type: application/json' \
    -d '{""name"":""Ada"",""role"":""admin""}' \
    -u user:pass -L --max-time 10 --compressed");
Console.WriteLine(result.StatusCode);
```

## Not Yet Supported

These flags are parsed or ignored but do not change behaviour today. Use the C# alternative where one exists.

| Flag | Alternative today |
|------|-------------------|
| `--retry`, `--retry-delay` | Write a small retry loop around `Curl.ExecuteAsync` (see [Handle Errors](../cookbook/beginner/handle-errors.html)); `CurlSettings.WithRetries` is accepted but not acted on yet |
| `-c/--cookie-jar`, `-b cookiefile` | Pass cookies inline: `-b 'session=abc'` |
| `--cert`, `--key`, `--cacert` | Not available; `-k` skips validation in development |
| `--http2`, `--http3` | The runtime negotiates HTTP/1.1 or HTTP/2 automatically |
| `-G/--get` | Put the query string in the URL |
| `-T/--upload-file` | `--data-binary @file` with `-X PUT` |
| `-D/--dump-header` | `-i` to include headers in the body, or read `result.Headers` |

Options that the command string does not carry can be passed as `CurlSettings`:

```csharp
using CurlDotNet;
using CurlDotNet.Core;

var settings = new CurlSettings().WithTimeout(10).WithFollowRedirects();
var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/200", settings);
Console.WriteLine(result.StatusCode);
```

## Shell Syntax: Linux, macOS and Windows

Commands copied from any terminal are normalized before parsing. Bash/zsh (Linux and macOS) is the canonical syntax, matching the curl manual; CMD and PowerShell conveniences are translated to it.

| Concern | Bash / zsh | CMD | PowerShell |
|---------|------------|-----|------------|
| Quotes | `'...'` and `"..."` | `"..."` | `'...'` and `"..."` |
| Line continuation | trailing `\` | trailing `^` | trailing `` ` `` |
| Environment variables | `$TOKEN`, `${TOKEN}` | `%TOKEN%` | `$env:TOKEN` |

All three continuation characters are joined into a single line, and all variable syntaxes are expanded from the process environment (an unset variable is left as typed).

```bash
curl -X POST https://httpbin.org/post \
  -H "Authorization: Bearer $API_TOKEN" \
  -d '{"name":"Ada","role":"admin"}'
```

```powershell
curl -H "Accept: application/json" `
     -H "Authorization: Bearer $env:API_TOKEN" `
     https://httpbin.org/get
```

```cmd
curl -X POST ^
     -H "Content-Type: application/json" ^
     -d "{\"foo\":\"bar\"}" ^
     https://httpbin.org/post
```

Guidance:

- Prefer single quotes around JSON bodies; inside a C# verbatim string, double quotes are written as `""`.
- Avoid smart quotes copied from rich-text editors; replace them with `'` or `"`.
- Shell features such as process substitution (`<(...)`), command substitution (`$(...)`) and aliases are not evaluated.
- `~` is not expanded; pass full paths to `-o`, `-d @file` and `-F file=@path`.

## Debugging a Command

Validate a command before running it:

```csharp
using CurlDotNet;

var validation = Curl.Validate("curl -X POST -H 'Accept: application/json'");
if (!validation.IsValid)
    Console.WriteLine(validation.Error); // e.g. no URL in command
```

Or build the request in C# and print the curl string it produces:

```csharp
using CurlDotNet.Core;

var command = CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithJson(new { name = "Ada" })
    .WithBearerToken("token")
    .ToCurlCommand();
Console.WriteLine(command);
```

## Related Pages

- [Supported Protocols](../supported-protocols.html)
- [libcurl Mapping](libcurl-mapping.html)
- [Common Issues](../troubleshooting/common-issues.html)
