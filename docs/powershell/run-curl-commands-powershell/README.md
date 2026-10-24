---
title: "How to Run curl Commands from PowerShell (C# .NET)"
description: "Run curl commands from PowerShell with [CurlDotNet.Curl]::Execute: read .StatusCode, .Headers and .Body, and convert JSON with ConvertFrom-Json."
---

# How to run curl commands from PowerShell with CurlDotNet

To run a curl command from PowerShell with CurlDotNet, load the DLL once with `Add-Type`, then call `[CurlDotNet.Curl]::Execute('curl ...')`. You get a .NET object back with `.StatusCode`, `.Headers` and `.Body`, and you can pipe `.Body` to `ConvertFrom-Json`. The command runs in-process, so PowerShell's native-command quoting rules never touch it.

Setup is covered in [installing CurlDotNet for PowerShell](../install-curldotnet-powershell/README.md).

## Run a command and read the result

```powershell
Add-Type -Path "$HOME/.curldotnet/9.8.0/lib/net8.0/CurlDotNet.dll"

$r = [CurlDotNet.Curl]::Execute('curl -H "Accept: application/json" https://httpbin.org/json')

$r.StatusCode                  # 200
$r.IsSuccess                   # True
$r.Headers['Content-Type']     # application/json (keys are case-insensitive)
$data = $r.Body | ConvertFrom-Json
$data.slideshow.title          # Sample Slide Show
```

List every response header with `$r.Headers.GetEnumerator() | Sort-Object Key | Format-Table Key, Value`.

## Use single quotes or a single-quoted here-string

PowerShell expands `$variables` and backtick escapes inside double-quoted strings before CurlDotNet sees them. Put the command in single quotes, or in a single-quoted here-string for multi-line commands copied from docs:

```powershell
$cmd = @'
curl -X POST https://httpbin.org/post `
  -H "Content-Type: application/json" `
  -d '{"name":"Ada","role":"admin"}'
'@
$r = [CurlDotNet.Curl]::Execute($cmd)
($r.Body | ConvertFrom-Json).json.name    # Ada
```

The backtick continuations, and bash-style `\` continuations, are joined by CurlDotNet's parser. The closing `'@` must start at the beginning of its line.

## Secrets from environment variables

CurlDotNet expands `$env:NAME` (and `$NAME`) itself, so the token stays out of the string and out of your history. Like a shell, it expands them unquoted or inside the command's double quotes, not inside its single quotes:

```powershell
$env:API_TOKEN = 'abc123'
$r = [CurlDotNet.Curl]::Execute('curl -H "Authorization: Bearer $env:API_TOKEN" https://httpbin.org/bearer')
$r.StatusCode   # 200
```

## Save files with absolute paths

.NET resolves relative paths against the process directory, which `Set-Location` does not change. Build the path from `$PWD`:

```powershell
$out = Join-Path $PWD 'logo.png'
$r = [CurlDotNet.Curl]::Execute("curl -L -o `"$out`" https://httpbin.org/image/png")
$r.OutputFiles       # the path; $r.Body is empty, as curl prints nothing with -o
(Get-Item $out).Length
```

This one uses a double-quoted string on purpose, so `$out` is expanded, with backtick-escaped quotes around the path.

## Async calls and the fluent builder

```powershell
$r = [CurlDotNet.Curl]::ExecuteAsync('curl https://httpbin.org/get').GetAwaiter().GetResult()

$builder = [CurlDotNet.Core.CurlRequestBuilder]::Get('https://httpbin.org/headers')
$b = $builder.WithHeader('X-Demo', '1').WithUserAgent('ps-script/1.0').Execute()
$b.StatusCode
```

## Handle errors

HTTP errors such as 404 come back as a result with `IsSuccess` set to `False`. Network failures throw. PowerShell wraps .NET exceptions, so unwrap them:

```powershell
try {
    $r = [CurlDotNet.Curl]::Execute('curl -m 5 https://no-such-host.invalid/')
} catch {
    $e = $_.Exception.GetBaseException()
    "$($e.GetType().Name) (curl code $($e.CurlErrorCode)): $($e.Message)"
}
```

## Notes

- As with curl.exe, there is no timeout unless you pass `-m` (which covers the whole transfer) or `--connect-timeout`. Add `--retry 3` to retry timeouts and transient 5xx answers.
- CurlDotNet sends `User-Agent: curl/8.7.1` like curl.exe, so APIs that require a User-Agent, such as GitHub's, work without `-A`.
- With `-f`, a status of 400 or above throws `CurlHttpReturnedErrorException` (curl code 22) instead of returning a result.
- These PowerShell snippets were written against the 9.8.0 API and checked in C#, but were not run in a PowerShell session for this page.

## Related

- [Install CurlDotNet for PowerShell](../install-curldotnet-powershell/README.md)
- [curl vs Invoke-WebRequest](../curl-vs-invoke-webrequest/README.md)
- [Paste curl commands from API docs](../paste-curl-command-powershell-csharp/README.md)
- [Parse JSON in C#](../../how-to/parse-json-from-curl-in-csharp/README.md)
- [PowerShell guides](../README.md)
