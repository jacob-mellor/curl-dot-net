---
title: "How to Install CurlDotNet for PowerShell and C#"
description: "Install CurlDotNet for PowerShell 7, Windows PowerShell 5.1, C# projects and .csx scripts: Add-Type the right DLL, dotnet add package, or #r nuget."
---

# How to install CurlDotNet for PowerShell and C#

To install CurlDotNet for PowerShell, download the `CurlDotNet` package from nuget.org, extract it, and load the DLL that matches your PowerShell runtime with `Add-Type -Path`. For C# projects use `dotnet add package CurlDotNet`, and for scripts use `#r "nuget: CurlDotNet"`.

## C# projects

```bash
dotnet add package CurlDotNet
```

## C# scripts (.csx) and .NET 10 file-based apps

With [dotnet-script](https://github.com/dotnet-script/dotnet-script):

```csharp
#r "nuget: CurlDotNet, 9.7.0"
using CurlDotNet;

Console.WriteLine(Curl.Execute("curl https://httpbin.org/get").StatusCode);
```

With the .NET 10 SDK, a single `.cs` file can reference the package directly. Save this as `app.cs` and run `dotnet run app.cs`:

```csharp
#:package CurlDotNet@9.7.0
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/uuid");
Console.WriteLine(result.Body);
```

## PowerShell: pick the right DLL

A `.nupkg` is a zip file with one DLL per target framework under `lib/`. Version 9.7.0 on nuget.org contains `net472`, `net48`, `net8.0`, `net9.0` and `netstandard2.0`. Load the one that matches the runtime PowerShell runs on:

| PowerShell | Runs on | Load |
|---|---|---|
| PowerShell 7.4 | .NET 8 | `lib/net8.0/CurlDotNet.dll` |
| PowerShell 7.5 | .NET 9 | `lib/net9.0/CurlDotNet.dll` (or `net8.0`) |
| Windows PowerShell 5.1 | .NET Framework 4.x | `lib/net48/CurlDotNet.dll` (`net472` if only 4.7.2 is installed) |

To check what you are running, use `$PSVersionTable.PSVersion` and `[System.Runtime.InteropServices.RuntimeInformation]::FrameworkDescription`. On a runtime newer than the newest folder, use the newest folder that is not newer than the runtime.

## PowerShell 7: download and load

```powershell
$version = '9.7.0'
$root = Join-Path $HOME ".curldotnet/$version"
New-Item -ItemType Directory -Force -Path $root | Out-Null
$nupkg = Join-Path $root 'curldotnet.zip'
Invoke-WebRequest "https://api.nuget.org/v3-flatcontainer/curldotnet/$version/curldotnet.$version.nupkg" -OutFile $nupkg
Expand-Archive $nupkg -DestinationPath $root -Force

Add-Type -Path (Join-Path $root 'lib/net8.0/CurlDotNet.dll')
[CurlDotNet.Curl]::Execute('curl https://httpbin.org/get').StatusCode
```

Add the `Add-Type` line to your `$PROFILE` to load it in every session.

## Windows PowerShell 5.1

The same steps, with two Windows PowerShell details:

```powershell
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$root = "$env:LOCALAPPDATA\curldotnet\9.7.0"
New-Item -ItemType Directory -Force -Path $root | Out-Null
Invoke-WebRequest 'https://api.nuget.org/v3-flatcontainer/curldotnet/9.7.0/curldotnet.9.7.0.nupkg' -OutFile "$root\curldotnet.zip" -UseBasicParsing
Expand-Archive "$root\curldotnet.zip" -DestinationPath $root -Force
Get-ChildItem $root -Recurse -Filter *.dll | Unblock-File
Add-Type -Path "$root\lib\net48\CurlDotNet.dll"
```

`Expand-Archive` in 5.1 only accepts a `.zip` extension, which is why the file is saved as `curldotnet.zip`. `Unblock-File` removes the downloaded-from-internet mark that can stop .NET Framework from loading the DLL.

## Alternative: Install-Package (PackageManagement)

```powershell
Register-PackageSource -Name nuget.org -Location https://www.nuget.org/api/v2 -ProviderName NuGet -Trusted
Install-Package CurlDotNet -Source nuget.org -Scope CurrentUser -SkipDependencies
$pkgDir = Split-Path (Get-Package CurlDotNet).Source
Add-Type -Path (Join-Path $pkgDir 'lib/net8.0/CurlDotNet.dll')
```

`-SkipDependencies` avoids pulling the package's NuGet dependencies, which basic curl execution does not need on .NET 8+ (see below). Skip `Register-PackageSource` if `Get-PackageSource` already lists a NuGet source for nuget.org.

## Dependencies

On .NET 8 and later, loading `CurlDotNet.dll` on its own was enough to run requests, read headers and generate code in our test. The netstandard2.0 build uses Newtonsoft.Json for `ParseJson` and `AsJsonDynamic`. In PowerShell you can avoid those helpers entirely by piping `.Body` to `ConvertFrom-Json`.

## What was and was not verified

- Verified: `dotnet add package`, the `#r "nuget: ..."` script and the `#:package` file-based app, all against 9.7.0 from nuget.org; the package's `lib/` folders; the download URL; and loading the net8.0 DLL alone on .NET 8 to run a request.
- Not verified: the PowerShell commands above were not run in PowerShell 7 or Windows PowerShell 5.1 for this page, and neither was the `Install-Package` route (including where `Get-Package` reports the package). Please report problems on the [issue tracker](https://github.com/jacob-mellor/curl-dot-net/issues).

## Related

- [Run curl commands from PowerShell](../run-curl-commands-powershell/README.md)
- [curl vs Invoke-WebRequest](../curl-vs-invoke-webrequest/README.md)
- [Run curl synchronously in C#](../../how-to/curl-synchronously-in-csharp/README.md)
- [PowerShell guides](../README.md)
