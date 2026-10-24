---
title: "CurlDotNet Initialization Errors in C#: Setup Problems"
description: "CurlDotNet initialization errors in C#: why 'Curl does not exist', a missing package or wrong target framework happen, and the setup that avoids them."
---

# CurlDotNet Initialization Errors in C#

CurlDotNet initialization errors in C# are setup problems rather than runtime failures: there is no "init" step to fail, because `CurlDotNet.Curl` is a static class that is ready as soon as the package is referenced. When a project cannot call `Curl.ExecuteAsync`, the cause is almost always a missing package reference, a missing `using`, or an unsupported target framework. This page lists those causes and their fixes.

## No Initialization Step

```csharp
using CurlDotNet;

// Nothing to construct or configure first
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
Console.WriteLine(result.StatusCode);
```

`Curl` has no constructor to call; the class is static. If you want an instance to hold shared settings, use `CurlDotNet.Lib.LibCurl`:

```csharp
using CurlDotNet.Lib;

using var curl = new LibCurl();
curl.WithTimeout(TimeSpan.FromSeconds(10)).WithUserAgent("MyApp/1.0");
var result = await curl.GetAsync("https://httpbin.org/get");
if (result.Exception != null)
    Console.WriteLine(result.Exception.Message);   // LibCurl returns failures instead of throwing
```

`LibCurl` does not throw `CurlException`s: a failed transfer comes back as a `CurlResult` whose `Exception` property holds the exception. Only a cancelled token throws (`OperationCanceledException`).

## Reserved Initialization Types

These types exist in `CurlDotNet.Exceptions` for parity with libcurl's error list, but no code path raises them today:

| Type | `CurlErrorCode` |
|------|----:|
| `CurlFailedInitException` | 2 |
| `CurlNotBuiltInException` (property `Feature`) | 4 |
| `CurlOutOfMemoryException` | 27 |
| `CurlFunctionNotFoundException` (property `FunctionName`) | 41 |
| `CurlExecutionException` | none |

Exit code 2 does occur, but for command-line mistakes such as an option without its value or two request methods; those throw a plain `CurlException` with `CurlErrorCode` 2 and are covered on the [URL and command errors](url-errors.html) page.

## Common Setup Problems

### "The name 'Curl' does not exist in the current context"

The package is not referenced, or the `using` is missing.

```bash
dotnet add package CurlDotNet
dotnet restore
```

Then add `using CurlDotNet;` at the top of the file.

### "The type or namespace name 'CurlDotNet' could not be found"

The package restore did not run, or the project targets a framework the package does not ship. Check the target in your `.csproj`:

```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
</PropertyGroup>
```

CurlDotNet ships netstandard2.0, net472, net48, net8.0, net9.0 and net10.0 builds, so .NET Framework 4.7.2 and later, .NET Core 2.0 and later, and modern .NET all work. It has no Ninject or System.CommandLine dependencies. See [framework compatibility](../framework-compatibility.html).

### "'CurlResult' could not be found"

`CurlResult`, `CurlRequestBuilder` and `CurlSettings` live in `CurlDotNet.Core`:

```csharp
using CurlDotNet;
using CurlDotNet.Core;

CurlResult result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
```

### Two CurlHttpException Types

`CurlHttpException` exists in both `CurlDotNet.Core` (thrown by `result.EnsureSuccess()`) and `CurlDotNet.Exceptions`. If both namespaces are imported, qualify the one you catch:

```csharp
catch (CurlDotNet.Core.CurlHttpException ex)
{
    Console.WriteLine(ex.StatusCode);
}
```

### Static Defaults Applied Too Late

`Curl.DefaultMaxTimeSeconds`, `Curl.DefaultConnectTimeoutSeconds`, `Curl.DefaultFollowRedirects` and `Curl.DefaultInsecure` are read when each command runs. All of them default to off: in particular there is no implicit timeout until you set `Curl.DefaultMaxTimeSeconds` (or pass `-m`). Set them once at startup, before the first request:

```csharp
Curl.DefaultMaxTimeSeconds = 30;
Curl.DefaultFollowRedirects = true;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/redirect/1");
```

## Diagnostics

When reporting a setup problem, include the runtime details:

```csharp
using System.Runtime.InteropServices;

Console.WriteLine($"OS: {RuntimeInformation.OSDescription}");
Console.WriteLine($"Framework: {RuntimeInformation.FrameworkDescription}");
Console.WriteLine($"Architecture: {RuntimeInformation.ProcessArchitecture}");
Console.WriteLine($"CurlDotNet: {typeof(CurlDotNet.Curl).Assembly.GetName().Version}");
```

## Related Documentation

- [Installation guide](../getting-started/installation.html)
- [Framework compatibility](../framework-compatibility.html)
- [Troubleshooting: common issues](../troubleshooting/common-issues.html)
- [Exception reference](index.html)
