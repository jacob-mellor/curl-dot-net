---
title: "curl vs Invoke-WebRequest vs Invoke-RestMethod (C#)"
description: "curl vs Invoke-WebRequest vs Invoke-RestMethod in PowerShell: the curl alias trap, why pasted curl commands break, and how CurlDotNet runs them."
---

# curl vs Invoke-WebRequest vs Invoke-RestMethod in PowerShell

In PowerShell, `Invoke-WebRequest` returns the raw HTTP response, `Invoke-RestMethod` returns the body already parsed (JSON becomes objects), and `curl.exe` is the native curl program. In Windows PowerShell 5.1, typing `curl` does not run curl at all: it is an alias for `Invoke-WebRequest`. CurlDotNet adds a fourth option: run the curl command text in-process with `[CurlDotNet.Curl]::Execute(...)`.

## The curl alias trap

```powershell
Get-Command curl      # Windows PowerShell 5.1: Alias  curl -> Invoke-WebRequest
                      # PowerShell 7+:          Application  curl(.exe)
curl.exe --version    # always the real curl (included with Windows 10 1803 and later)
```

PowerShell 6 removed the `curl` and `wget` aliases, but Windows PowerShell 5.1 still ships them. A pasted `curl -H "Accept: application/json" https://...` in 5.1 fails with parameter-binding errors, because `-H` binds to `Invoke-WebRequest -Headers`, which expects a hashtable, not a string.

## Why pasted curl commands break in PowerShell

Even when `curl.exe` really runs, commands written for bash often fail:

1. **Line continuation.** Bash uses `\` at the end of a line. PowerShell uses a backtick. With `\`, PowerShell runs the first line on its own, then tries to run `-H ...` as a command.
2. **Embedded double quotes.** Windows PowerShell 5.1, and PowerShell 7 before 7.3, drop embedded `"` when passing arguments to native programs. `-d '{"key":"value"}'` reaches curl.exe as `{key:value}`. That is why Windows-oriented docs write `-d '{\"key\":\"value\"}'`. PowerShell 7.3 and later pass the quotes through by default (see `$PSNativeCommandArgumentPassing`).
3. **Variable expansion.** In double-quoted strings PowerShell expands `$TOKEN` itself, often to an empty string.

## How CurlDotNet sidesteps all three

The command is a .NET string handed to a method, not arguments to a native process, so PowerShell's native-argument rules never apply. Inside a single-quoted here-string PowerShell changes nothing, and CurlDotNet's parser handles the shell syntax:

```powershell
$cmd = @'
curl -X POST https://httpbin.org/post `
  -H "Content-Type: application/json" `
  -d '{"key":"value"}'
'@
$r = [CurlDotNet.Curl]::Execute($cmd)
($r.Body | ConvertFrom-Json).json.key      # value
```

- Backtick, `\` and `^` line continuations are all joined.
- The JSON keeps its quotes, because no native command line is involved.
- `$env:NAME` and `$NAME` are expanded from environment variables by CurlDotNet. In a single-quoted PowerShell string PowerShell leaves them alone; CurlDotNet then expands them where a shell would, unquoted or inside the curl command's double quotes, but not inside its single quotes.
- The Windows habit `-d '{\"key\":\"value\"}'` is unescaped to `{"key":"value"}` (when the quoted text contains no bare `"`), so the server receives valid JSON whether or not you delete the backslashes.

The same PowerShell-style command from C#, verified with 9.8.0 against httpbin.org:

```csharp
using CurlDotNet;

var ps = "curl -X POST https://httpbin.org/post `\n  -H 'Content-Type: application/json' `\n  -d '{\"key\":\"value\"}'";
var result = await Curl.ExecuteAsync(ps);
Console.WriteLine(result.Body);  // "json": { "key": "value" }
```

## Side by side

| | `Invoke-WebRequest` | `Invoke-RestMethod` | `curl.exe` | CurlDotNet |
|---|---|---|---|---|
| Input | Cmdlet parameters | Cmdlet parameters | curl flags, subject to PowerShell quoting | curl command as one string |
| Output | Response object (`StatusCode`, `Content`, `Headers`) | Parsed body | Text on stdout | `CurlResult` (`StatusCode`, `Body`, `Headers`, `BinaryData`) |
| 4xx / 5xx | Throws (PowerShell 7 adds `-SkipHttpErrorCheck`) | Throws (same switch) | Exit code 0 unless `-f` | `IsSuccess = $false`, no throw unless `-f` |
| JSON | `ConvertFrom-Json` | Automatic | `ConvertFrom-Json` | `ConvertFrom-Json` (or `ParseJson` in C#) |
| Available | Built in | Built in | Windows 10 1803+, macOS, Linux | Load the DLL |
| Same code in C# | No | No | No (Process.Start) | Yes |

## Which to use

- Writing new PowerShell from scratch: `Invoke-RestMethod` is idiomatic, and `[CurlDotNet.Curl]::ToPowershellCode()` can [generate it from a curl command](../curl-to-powershell-code/README.md).
- Running a curl command from documentation exactly as written: CurlDotNet, or `curl.exe` in PowerShell 7.3+ with careful quoting.
- Sharing the request between a PowerShell script and a C# app: CurlDotNet, because the string is identical in both.

The PowerShell behaviors described here are standard PowerShell; the CurlDotNet snippets in PowerShell were not run in a PowerShell session for this page.

## Related

- [Run curl commands from PowerShell](../run-curl-commands-powershell/README.md)
- [Paste curl commands from API docs](../paste-curl-command-powershell-csharp/README.md)
- [Convert curl to Invoke-RestMethod](../curl-to-powershell-code/README.md)
- [How CurlDotNet's parser works](../../how-to/how-curldotnet-works/README.md)
- [PowerShell guides](../README.md)
