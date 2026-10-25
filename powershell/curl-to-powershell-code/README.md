---
title: "Convert curl to PowerShell Invoke-RestMethod in C#"
description: "Convert curl to PowerShell: Curl.ToPowershellCode(cmd) turns a curl command into native Invoke-RestMethod code. What it translates and what to add by hand."
---

# How to convert curl to PowerShell Invoke-RestMethod code

To convert a curl command to native PowerShell, call `Curl.ToPowershellCode(cmd)` from C#, or `[CurlDotNet.Curl]::ToPowershellCode('curl ...')` from PowerShell. It returns an `Invoke-RestMethod` line you can paste into a script that has no CurlDotNet dependency.

## From C#

```csharp
using CurlDotNet;

var cmd = "curl -X POST https://api.example.com/users " +
          "-H 'Authorization: Bearer abc' -H 'Content-Type: application/json' " +
          "-d '{\"name\":\"Ada\"}'";

Console.WriteLine(Curl.ToPowershellCode(cmd));
```

Output (one line, wrapped here):

```powershell
Invoke-RestMethod -Uri "https://api.example.com/users" -Method POST `
  -Headers @{"Authorization"="Bearer abc"; "Content-Type"="application/json"} `
  -Body "{`"name`":`"Ada`"}" -ContentType "application/json"
```

The JSON quotes are escaped with backticks, which is correct inside a double-quoted PowerShell string.

## From the fluent builder

If you build requests in C# with `CurlRequestBuilder`, `ToCurlCommand()` gives you the curl string to convert:

```csharp
using CurlDotNet;
using CurlDotNet.Core;

var curl = CurlRequestBuilder.Post("https://api.example.com/users")
    .WithBearerToken("abc").WithJson(new { name = "Ada" }).ToCurlCommand();

Console.WriteLine(Curl.ToPowershellCode(curl));
```

## From PowerShell

```powershell
$code = [CurlDotNet.Curl]::ToPowershellCode(@'
curl -X DELETE https://api.example.com/users/1 -H "Authorization: Bearer t"
'@)
$code
# Invoke-RestMethod -Uri "https://api.example.com/users/1" -Method DELETE -Headers @{"Authorization"="Bearer t"}
```

Review generated code before you run it. Do not pipe it straight into `Invoke-Expression`, especially when the curl command came from somewhere you do not control.

## What gets translated

In 9.8.0 the PowerShell generator carries over the **URL, method, headers and body**. `--json` becomes the body plus `Content-Type` and `Accept` headers. Other flags are dropped without a warning (unlike `ToHttpClient` and `ToPythonRequests`, which also translate `-u`, `-x`, `-k`, `-L`, `-m` and `-F`). Add them by hand:

| curl flag | Add to Invoke-RestMethod |
|---|---|
| `-u user:pass` | `-Authentication Basic -Credential (Get-Credential)` (PowerShell 7) or an `Authorization` header |
| `-k` | `-SkipCertificateCheck` (PowerShell 6+) |
| `-L` | Nothing; redirects are followed by default (`-MaximumRedirection 0` turns them off) |
| `-m 10` | `-TimeoutSec 10` |
| `-o file` | `-OutFile file` |
| `-F 'f=@a.pdf'` | `-Form @{ f = Get-Item a.pdf }` (PowerShell 6.1+) |
| `-x http://proxy:8080` | `-Proxy http://proxy:8080` (`-ProxyCredential` for auth) |
| `-A 'MyApp/1.0'` | `-UserAgent 'MyApp/1.0'` |
| `-I` | Already `-Method HEAD`; use `Invoke-WebRequest` to see the headers |

## Content-Type appears twice

When the curl command sets `Content-Type` with `-H`, the output has it both in `-Headers` and in `-ContentType`. If your PowerShell version complains about the duplicate, delete it from the `-Headers` hashtable; `-ContentType` is enough.

## Generate other languages from the same command

```csharp
using CurlDotNet;

var cmd = "curl -H 'Accept: application/json' https://api.example.com/items";
Console.WriteLine(Curl.ToHttpClient(cmd));      // C# HttpClient
Console.WriteLine(Curl.ToPythonRequests(cmd));  // Python requests
Console.WriteLine(Curl.ToFetch(cmd));           // JavaScript fetch
```

Or skip conversion and run the curl command itself with `[CurlDotNet.Curl]::Execute`; see [running curl commands from PowerShell](../run-curl-commands-powershell/README.md).

## Related

- [curl vs Invoke-WebRequest vs Invoke-RestMethod](../curl-vs-invoke-webrequest/README.md)
- [Convert curl to C#](../../how-to/convert-curl-to-csharp/README.md)
- [Install CurlDotNet for PowerShell](../install-curldotnet-powershell/README.md)
- [PowerShell guides](../README.md)
