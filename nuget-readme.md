# CurlDotNet: run curl commands in C# and .NET

Paste any curl command from API documentation into your .NET app and run it in-process. CurlDotNet is a pure-C# reimplementation of curl's command-line parser and transfer engine on top of `HttpClient`: no `curl.exe`, no `Process.Start`, no native libcurl. It targets .NET Standard 2.0, .NET Framework 4.7.2/4.8 and .NET 8, 9 and 10, so it runs in ASP.NET Core, Blazor Server, MAUI, Azure Functions, AWS Lambda and Linux containers.

## Run a curl command in C#

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -H 'Accept: application/json' https://api.github.com/users/octocat");

Console.WriteLine(result.StatusCode);      // 200
Console.WriteLine(result.Body);            // JSON body
var user = result.ParseJson<GitHubUser>(); // typed JSON
```

Commands may span lines with `\`, `^` or PowerShell backticks, use single or double quotes, read bodies from `@file`, and omit the leading `curl`. `Curl.Execute(...)` is the synchronous form. Commands pasted from bash, Windows CMD, PowerShell or PowerShell's `Invoke-WebRequest` alias are detected automatically, or forced with `Curl.ExecuteAsync(command, CurlFlavor.PowerShell)`. `CurlFlavor.Native` reads commands with the rules of the operating system's own shell.

## The curl commands people actually paste

```csharp
using CurlDotNet;
using CurlDotNet.Core;

// POST JSON (curl --json / -X POST -d)
var a = await Curl.ExecuteAsync("curl --json '{\"name\":\"Alice\"}' https://httpbin.org/post");
var b = await Curl.PostJsonAsync("https://httpbin.org/post", new { name = "Alice" });

// Bearer token and basic auth
var c = await Curl.ExecuteAsync("curl -H 'Authorization: Bearer TOKEN' https://api.example.com/me");
var d = await Curl.ExecuteAsync("curl -u alice:secret https://httpbin.org/basic-auth/alice/secret");

// Multipart file upload (curl -F)
var e = await Curl.ExecuteAsync("curl -F 'file=@report.pdf' -F 'name=Q3' https://httpbin.org/post");

// Download a file, binary-safe (curl -o), following redirects (-L)
await Curl.ExecuteAsync("curl -L -o report.xlsx https://example.com/report.xlsx");

// Ignore certificate errors (curl -k), proxies (-x), timeouts and retries
var f = await Curl.ExecuteAsync("curl -k --connect-timeout 5 --max-time 30 --retry 3 -x http://proxy:8080 https://api.example.com");

// Fluent builder when you have variables instead of a command string
var g = await CurlRequestBuilder.Post("https://httpbin.org/post").WithJson(new { name = "Alice" }).WithBearerToken("TOKEN").ExecuteAsync();
```

## Convert curl to C#

`Curl.ToHttpClient(command)` generates `HttpClient` code for a curl command; `Curl.ToFetch`, `Curl.ToPythonRequests` and `Curl.ToPowershellCode` do the same for JavaScript, Python and PowerShell.

## Why CurlDotNet instead of HttpClient

- Zero translation: the command in the docs is the code in your app, including `--digest`, `--ntlm`, `--aws-sigv4`, `--json`, `@file`, `-F`, `--retry`, `--compressed` and `--trace`.
- Behaves like curl: default `User-Agent` and `Accept` headers, curl's redirect and `-d`/`--data-binary`/`--data-urlencode` semantics, exit codes surfaced as typed exceptions that explain how to fix the problem.
- Verified against curl's own test suite: all 631 runnable HTTP tests from curl/curl pass (v9.8.11), replayed offline against a local stub server with the exact bytes, output and error text compared, and checked against real curl on Linux. Plus 2,073 unit tests.
- Cookie jars (-b/-c), .netrc, -K config files, --variable, URL globbing, multiple URLs and --next, every -w variable, pure-C# Digest and NTLM.
- Binary-safe downloads even when a server lies about `Content-Type`.

## Documentation

- Docs home: https://jacob-mellor.github.io/curl-dot-net/
- New to curl: https://jacob-mellor.github.io/curl-dot-net/new-to-curl.html
- How-to pages: https://github.com/jacob-mellor/curl-dot-net/tree/master/docs/how-to
- PowerShell guides: https://github.com/jacob-mellor/curl-dot-net/tree/master/docs/powershell
- curl to HttpClient migration: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/migration/httpclient.md
- Source, issues and discussions: https://github.com/jacob-mellor/curl-dot-net

MIT licensed. Written by Jacob Mellor (CTO, IronSoftware); sponsored by IronSoftware (https://ironsoftware.com), makers of IronPDF, IronOCR, IronXL and IronBarcode. Part of the UserLand.NET initiative. curl itself is the work of Daniel Stenberg and the curl contributors.
