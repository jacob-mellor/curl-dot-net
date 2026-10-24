---
title: "Using CurlDotNet from PowerShell, C# and .NET Scripts"
description: "Using CurlDotNet from PowerShell and .NET scripts: install it, run curl commands with [CurlDotNet.Curl]::Execute, avoid quoting traps."
---

# Using CurlDotNet from PowerShell and .NET scripts

CurlDotNet is a .NET assembly, so PowerShell can load it with `Add-Type` and run curl commands through `[CurlDotNet.Curl]::Execute(...)`, with no curl.exe and none of PowerShell's native-argument quoting rules. The same command strings work unchanged in C# projects and `.csx` scripts.

```powershell
Add-Type -Path "$HOME/.curldotnet/9.8.0/lib/net8.0/CurlDotNet.dll"
$r = [CurlDotNet.Curl]::Execute('curl -H "Accept: application/json" https://httpbin.org/json')
($r.Body | ConvertFrom-Json).slideshow.title
```

## Guides

| Guide | What it covers |
|---|---|
| [Install CurlDotNet for PowerShell](install-curldotnet-powershell/README.md) | Downloading the package, which DLL to load in PowerShell 7 and in Windows PowerShell 5.1, `dotnet add package`, `#r "nuget: CurlDotNet"` and `#:package` |
| [Run curl commands from PowerShell](run-curl-commands-powershell/README.md) | `[CurlDotNet.Curl]::Execute`, reading `.StatusCode`, `.Headers` and `.Body`, `ConvertFrom-Json`, here-strings, errors |
| [curl vs Invoke-WebRequest vs Invoke-RestMethod](curl-vs-invoke-webrequest/README.md) | The `curl` alias in 5.1, why pasted commands break, how CurlDotNet's parser handles backtick continuations and Windows-style `\"` escapes |
| [curl flavors: bash, CMD, PowerShell and Invoke-WebRequest](../how-to/curl-flavors-bash-cmd-powershell/README.md) | `CurlFlavor.PowerShell` for `curl.exe` commands, `CurlFlavor.InvokeWebRequest` for `-Uri`/`-Headers @{}` syntax, how `Auto` tells them apart, and the bytes each sends |
| [Paste a curl command from API docs](paste-curl-command-powershell-csharp/README.md) | GitHub, Stripe and OpenAI-style commands, run unchanged in C# or PowerShell |
| [Convert curl to PowerShell code](curl-to-powershell-code/README.md) | `Curl.ToPowershellCode` for native `Invoke-RestMethod` code, and what to add by hand |

## Quick start in three steps

1. **Get the DLL.** Download `CurlDotNet` from nuget.org and extract it (a `.nupkg` is a zip file). The [install guide](install-curldotnet-powershell/README.md) has copy-paste commands for PowerShell 7 and Windows PowerShell 5.1.
2. **Load it.** `Add-Type -Path <folder>/lib/net8.0/CurlDotNet.dll` for PowerShell 7.4+, or `lib/net48` for Windows PowerShell 5.1.
3. **Run a command.** Put the curl command in single quotes (or a `@' ... '@` here-string) and pass it to `[CurlDotNet.Curl]::Execute`.

```powershell
$cmd = @'
curl -X POST https://httpbin.org/post `
  -H "Content-Type: application/json" `
  -d '{"name":"Ada"}'
'@
$r = [CurlDotNet.Curl]::Execute($cmd)
"$($r.StatusCode) $(($r.Body | ConvertFrom-Json).json.name)"
```

## Which tool for which job

| You want to... | Use |
|---|---|
| Write idiomatic PowerShell from scratch | `Invoke-RestMethod` |
| Run a curl command from docs exactly as written | `[CurlDotNet.Curl]::Execute` |
| Turn a curl command into a dependency-free script | `[CurlDotNet.Curl]::ToPowershellCode` |
| Use the same request in PowerShell and in a C# app | CurlDotNet in both |
| Script in C# instead of PowerShell | `dotnet script` with `#r "nuget: CurlDotNet, 9.8.0"` |

## What to expect

- The result is a `CurlDotNet.Core.CurlResult`: `.StatusCode`, `.IsSuccess`, `.Body`, `.Headers`, `.IsBinary`, `.BinaryData`.
- HTTP errors (404, 500) do not throw unless you pass `-f`; network failures and timeouts do.
- As with curl.exe, there is no timeout unless you pass `-m` (whole transfer) or `--connect-timeout`.
- Like curl.exe, every request sends `User-Agent: curl/8.7.1` and `Accept: */*` unless you pass `-A` or `-H`.

## C# guides

The task-by-task C# guides (POST JSON, auth, uploads, downloads, timeouts, ASP.NET Core and more) are in [How to do common curl tasks in C#](../how-to/README.md).

## About CurlDotNet

A pure C# implementation of curl's command-line behavior on top of `HttpClient`, for netstandard2.0, .NET Framework 4.7.2 and 4.8, and .NET 8, 9 and 10. MIT licensed, written by Jacob Mellor, sponsored by Iron Software. Source and issues: [github.com/jacob-mellor/curl-dot-net](https://github.com/jacob-mellor/curl-dot-net).
