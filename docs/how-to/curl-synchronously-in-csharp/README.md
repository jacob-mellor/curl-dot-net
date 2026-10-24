---
title: "How to Run curl Synchronously in C# (Curl.Execute)"
description: "Run curl synchronously in C# with Curl.Execute: blocking calls for console apps, scripts, tests and .csx files, and when to use async instead."
---

# How to run curl synchronously in C#

To run curl synchronously in C#, call `Curl.Execute("curl ...")` instead of `Curl.ExecuteAsync`. It blocks the calling thread until the response arrives and returns the same `CurlResult`, which suits console tools, scripts, build steps and tests.

```bash
dotnet add package CurlDotNet
```

## The curl command

```csharp
using CurlDotNet;

var result = Curl.Execute("curl -H 'Accept: application/json' https://httpbin.org/get");

Console.WriteLine(result.StatusCode);
Console.WriteLine(result.Body);
```

## The fluent builder

```csharp
using CurlDotNet.Core;

var result = CurlRequestBuilder
    .Get("https://httpbin.org/basic-auth/user/passwd")
    .WithBasicAuth("user", "passwd")
    .Execute();

Console.WriteLine(result.IsSuccess);
```

## Synchronous helpers

```csharp
using CurlDotNet;

var get  = Curl.Get("https://httpbin.org/get");
var post = Curl.Post("https://httpbin.org/post", "a=1&b=2");
var json = Curl.PostJson("https://httpbin.org/post", new { name = "Ada" });
var file = Curl.Download("https://httpbin.org/image/png", "logo.png");
```

`Curl.Execute(command, cancellationToken)`, `Curl.Execute(command, settings)` and `Curl.Execute(curlOptions)` mirror the async overloads.

## In a .csx script

Synchronous calls keep scripts short. With [dotnet-script](https://github.com/dotnet-script/dotnet-script):

```csharp
#r "nuget: CurlDotNet, 9.8.0"
using CurlDotNet;

var result = Curl.Execute("curl https://httpbin.org/get");
Console.WriteLine(result.StatusCode);
```

Run it with `dotnet script check.csx`. On the .NET 10 SDK, a file-based app works too: put `#:package CurlDotNet@9.8.0` at the top of `check.cs` and run `dotnet run check.cs`.

## When blocking is fine

- Console apps and command-line tools that do one thing at a time
- Scripts (`.csx`, PowerShell) and build or deployment steps
- Unit and integration tests that do not have an async test method
- Windows services or workers that already run on their own dedicated thread

## When to use ExecuteAsync instead

- **ASP.NET Core, Azure Functions, Lambda:** blocking a thread-pool thread per request limits throughput.
- **WPF, WinForms, MAUI UI code:** blocking the UI thread freezes the window.
- **Many requests at once:** `Curl.ExecuteManyAsync(cmd1, cmd2, ...)` runs them concurrently.

`Curl.Execute` is implemented as `ExecuteAsync(...).GetAwaiter().GetResult()`. With a single-threaded `SynchronizationContext` (classic ASP.NET or a UI thread) that pattern can deadlock, which is one more reason to stay async there.

Since C# 7.1 `Main` can be `async`, and top-level statements can use `await` directly, so in new console apps the async version costs nothing extra.

## Errors are the same

`Curl.Execute` throws the same `CurlException` types as the async methods (they are not wrapped in `AggregateException`), and HTTP error statuses still come back as a normal result.

## Related

- [Run a curl command in C#](../run-curl-command-in-csharp/README.md)
- [Error handling](../curl-error-handling-csharp/README.md)
- [Install for PowerShell and scripts](../../powershell/install-curldotnet-powershell/README.md)
- [All how-to guides](../README.md)
