# CurlDotNet: run curl commands in C# and .NET

[![NuGet Version](https://img.shields.io/nuget/v/CurlDotNet.svg?cacheSeconds=0)](https://www.nuget.org/packages/CurlDotNet/)
[![Downloads](https://img.shields.io/nuget/dt/CurlDotNet.svg)](https://www.nuget.org/packages/CurlDotNet/)
[![Build Status](https://img.shields.io/github/actions/workflow/status/jacob-mellor/curl-dot-net/ci-smoke.yml?branch=master)](https://github.com/jacob-mellor/curl-dot-net/actions)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/jacob-mellor/curl-dot-net/blob/master/LICENSE)

![CurlDotNet - Why .NET Needs a POSIX/GNU Userland](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/1o4hlr4tbp6b8k86ew6c.jpg)

CurlDotNet is curl for C#: paste any curl command from API documentation, Stack Overflow or your terminal into a .NET application and run it in-process. It is a pure-C# reimplementation of curl's command-line parser and transfer engine on top of `HttpClient`. There is no `curl.exe`, no `Process.Start`, no native libcurl, and it runs anywhere .NET Standard 2.0 runs: .NET 8, 9 and 10, .NET Framework 4.7.2 and 4.8, ASP.NET Core, Blazor Server, MAUI, Azure Functions, AWS Lambda and Linux containers.

## New to curl? Start here

**[The beginner's guide to curl in C#](https://jacob-mellor.github.io/curl-dot-net/new-to-curl.html)** explains what curl is, why half the API documentation on the internet is written in it, and how to turn those examples into working C# in two minutes. The [how-to library](docs/how-to/README.md) answers the common tasks one page each, and the [PowerShell guides](docs/powershell/README.md) cover installing and using CurlDotNet from PowerShell.

## Install

```bash
dotnet add package CurlDotNet
```

```xml
<PackageReference Include="CurlDotNet" Version="9.*" />
```

## Run a curl command in C#

The whole library in one line: hand `Curl.ExecuteAsync` the command you would type in a terminal.

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -H 'Accept: application/json' https://api.github.com/users/octocat");

Console.WriteLine(result.StatusCode);                 // 200
Console.WriteLine(result.Body);                       // the JSON body
Console.WriteLine(result.GetHeader("Content-Type"));  // application/json; charset=utf-8

var user = result.ParseJson<GitHubUser>();            // typed JSON, case-insensitive property matching
```

Commands can span lines with `\`, `^` or PowerShell backticks, use single or double quotes, read bodies from `@file`, and omit the leading `curl`. A synchronous `Curl.Execute(...)` exists for console apps and scripts, and `Curl.ExecuteAsync(command, cancellationToken)` cancels like any other .NET call.

## Convert curl to C#

Two ways to go from a curl command to C#: run it as-is, or generate idiomatic `HttpClient` code to paste into your project.

```csharp
using CurlDotNet;

// 1. Run the command
var result = await Curl.ExecuteAsync("curl -X POST https://api.example.com/items -d '{\"id\":1}' -H 'Content-Type: application/json'");

// 2. Generate HttpClient, fetch, Python requests or PowerShell code
string csharp     = Curl.ToHttpClient("curl -u user:pass -x http://proxy:8080 https://api.example.com");
string javascript = Curl.ToFetch("curl -H 'Authorization: Bearer TOKEN' https://api.example.com/me");
string python     = Curl.ToPythonRequests("curl -k -L https://self-signed.example.com");
string powershell = Curl.ToPowershellCode("curl -X DELETE https://api.example.com/items/1");
```

## Common curl commands in C#

Every snippet below shows the curl-string form first and the fluent `CurlRequestBuilder` form second. Both compile against the current package.

### curl -X POST JSON in C#

```csharp
using CurlDotNet;
using CurlDotNet.Core;

var a = await Curl.ExecuteAsync("curl --json '{\"name\":\"Alice\"}' https://httpbin.org/post");
var b = await Curl.PostJsonAsync("https://httpbin.org/post", new { name = "Alice" });
var c = await CurlRequestBuilder.Post("https://httpbin.org/post").WithJson(new { name = "Alice" }).ExecuteAsync();
```

`--json` sets `Content-Type` and `Accept` to `application/json` exactly like curl 7.82+. `Curl.PostJsonAsync` serialises any object and keeps apostrophes and quotes readable in the payload.

### curl bearer token in C#

```csharp
var a = await Curl.ExecuteAsync("curl -H 'Authorization: Bearer TOKEN' https://api.example.com/me");
var b = await CurlRequestBuilder.Get("https://api.example.com/me").WithBearerToken("TOKEN").ExecuteAsync();
```

### curl basic, digest and NTLM auth in C# (curl -u)

```csharp
var basic  = await Curl.ExecuteAsync("curl -u alice:secret https://httpbin.org/basic-auth/alice/secret");
var digest = await Curl.ExecuteAsync("curl --digest -u alice:secret https://httpbin.org/digest-auth/auth/alice/secret");
var ntlm   = await Curl.ExecuteAsync("curl --ntlm -u 'DOMAIN\\alice:secret' https://intranet.example.com/report");
var fluent = await CurlRequestBuilder.Get("https://httpbin.org/basic-auth/alice/secret").WithBasicAuth("alice", "secret").ExecuteAsync();
```

`-u` sends preemptive Basic like curl; `--digest`, `--ntlm`, `--negotiate` and `--anyauth` perform the proper 401 challenge flow. `--aws-sigv4` signs requests for AWS, GCP and compatible providers:

```csharp
var s3 = await Curl.ExecuteAsync(@"curl --aws-sigv4 ""aws:amz:us-east-1:s3"" -u ""AKID:SECRET"" https://s3.us-east-1.amazonaws.com/my-bucket");
```

### curl -F file upload in C# (multipart form data)

```csharp
var a = await Curl.ExecuteAsync("curl -F 'file=@report.pdf' -F 'name=Q3' https://httpbin.org/post");
var b = await CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithMultipartForm(
        fields: new Dictionary<string, string> { ["name"] = "Q3" },
        files:  new Dictionary<string, string> { ["file"] = "report.pdf" })
    .ExecuteAsync();
```

### curl download file in C# (curl -o / -O)

```csharp
await Curl.ExecuteAsync("curl -L -o report.xlsx https://example.com/report.xlsx");
await Curl.DownloadAsync("https://example.com/report.xlsx", "report.xlsx");
var r = await CurlRequestBuilder.Get("https://example.com/report.xlsx").FollowRedirects().WithOutput("report.xlsx").ExecuteAsync();
Console.WriteLine(r.OutputFiles[0]);
```

Downloads are binary-safe: payloads are sniffed for ZIP, Office, PDF, image and archive signatures, so an `.xlsx` served as `text/plain` is still written byte-for-byte. `-i -o file` writes the response headers into the file, and with `-o` the `Body` property is empty, exactly as curl leaves stdout.

### curl ignore SSL certificate in C# (curl -k)

```csharp
var a = await Curl.ExecuteAsync("curl -k https://self-signed.badssl.com/");
var b = await CurlRequestBuilder.Get("https://self-signed.badssl.com/").Insecure().ExecuteAsync();
Curl.DefaultInsecure = true;   // every request, development only
```

### curl follow redirects in C# (curl -L)

```csharp
var a = await Curl.ExecuteAsync("curl -L --max-redirs 5 https://httpbin.org/redirect/3");
var b = await CurlRequestBuilder.Get("https://httpbin.org/redirect/3").FollowRedirects().WithMaxRedirects(5).ExecuteAsync();
Curl.DefaultFollowRedirects = true;   // process-wide default
```

With `-L -i` the header block of every hop is printed, as curl does.

### curl with cookies in C# (curl -b)

```csharp
var a = await Curl.ExecuteAsync("curl -b 'session=abc; theme=dark' https://httpbin.org/cookies");
var b = await CurlRequestBuilder.Get("https://httpbin.org/cookies").WithCookie("session=abc").ExecuteAsync();
```

Like curl, no cookie engine runs unless you ask for one, so cookies never leak between unrelated requests.

### curl proxy in C# (curl -x, SOCKS5)

```csharp
var a = await Curl.ExecuteAsync("curl -x http://proxy.corp.local:8080 --proxy-user user:pass https://api.example.com");
var b = await Curl.ExecuteAsync("curl --socks5 127.0.0.1:9050 https://check.torproject.org");
var c = await CurlRequestBuilder.Get("https://api.example.com").WithProxy("http://proxy.corp.local:8080", "user", "pass").ExecuteAsync();
```

### curl timeout and retry in C# (--max-time, --connect-timeout, --retry)

```csharp
var a = await Curl.ExecuteAsync("curl --connect-timeout 5 --max-time 30 https://httpbin.org/delay/2");
var b = await Curl.ExecuteAsync("curl --retry 3 --retry-delay 2 https://httpbin.org/status/503");
var c = await CurlRequestBuilder.Get("https://httpbin.org/delay/2")
    .WithConnectTimeout(TimeSpan.FromSeconds(5)).WithTimeout(TimeSpan.FromSeconds(30)).ExecuteAsync();
Curl.DefaultMaxTimeSeconds = 60;   // process-wide default; 0 means no timeout, like curl
```

A timeout throws `CurlOperationTimeoutException` (curl exit code 28). `--retry` retries timeouts and HTTP 408, 429, 500, 502, 503 and 504 with curl's doubling back-off; `--retry-all-errors` and `--retry-connrefused` widen it.

### curl -I HEAD request and custom headers in C#

```csharp
var head  = await Curl.ExecuteAsync("curl -I https://example.com");
var agent = await Curl.ExecuteAsync("curl -A 'MyApp/2.0' -H 'X-Request-Id: 42' -e https://referrer.example https://httpbin.org/headers");
Console.WriteLine(head.GetHeader("Last-Modified"));
```

CurlDotNet sends `User-Agent: curl/8.7.1` and `Accept: */*` by default, exactly like curl, so APIs that reject anonymous clients (GitHub's included) just work. Override with `-A`, `-H "User-Agent: ..."`, or `Curl.DefaultUserAgent`; `-H "Accept:"` removes a header.

### curl form data and URL-encoded data in C# (-d, --data-urlencode)

```csharp
var a = await Curl.ExecuteAsync("curl -d 'name=Ada' -d 'lang=C#' https://httpbin.org/post");
var b = await Curl.ExecuteAsync("curl --data-urlencode 'q=hello world & more' https://httpbin.org/post");
var c = await Curl.ExecuteAsync("curl -d @payload.json -H 'Content-Type: application/json' https://httpbin.org/post");
var d = await Curl.PostAsync("https://httpbin.org/post", "{\"name\":\"O'Brien\"}");   // no quoting to worry about
```

## No command string? Use CurlOptions, the builder, or LibCurl

```csharp
using CurlDotNet;
using CurlDotNet.Core;
using CurlDotNet.Lib;

// Values straight from variables, nothing is parsed or quoted
var options = new CurlOptions { Url = url, Method = "POST", Data = json, FollowLocation = true };
options.Headers["Content-Type"] = "application/json";
var r1 = await Curl.ExecuteAsync(options);

// Fluent builder
var r2 = await CurlRequestBuilder.Put("https://api.example.com/items/1").WithJson(item).WithBearerToken(token).ExecuteAsync();

// Reusable client for dependency injection (ASP.NET Core, Azure Functions)
using var curl = new LibCurl().WithBearerToken(token).WithTimeout(TimeSpan.FromSeconds(30));
var r3 = await curl.GetAsync("https://api.example.com/items");
```

`CurlResult` gives you `StatusCode`, `Body`, `Headers`, `IsSuccess`, `BinaryData`, `Timings` (name lookup, connect, TLS, first byte, total), `OutputFiles`, `ParseJson<T>()`, `AsJsonDynamic()`, `GetHeader()`, `EnsureSuccess()` and `SaveToFile()`.

## Debugging: -v, --trace and -w

```csharp
var verbose = await Curl.ExecuteAsync("curl -v https://api.example.com");                 // request/response headers in Body
await Curl.ExecuteAsync("curl --trace-ascii curl-trace.txt --trace-time https://api.example.com");   // full wire trace to a file
var timing  = await Curl.ExecuteAsync("curl -s -o /dev/null -w '%{http_code} %{time_total}' https://api.example.com");
```

A trace file records proxy selection, TLS verification, every header and body, transfer timings and the exact failure, so you can see which stage blocked a request behind a corporate proxy.

## How close to curl is it?

CurlDotNet is tested against curl's own test suite. The `tests/CurlDotNet.CurlSuite` project replays the HTTP tests from [curl/curl tests/data](https://github.com/curl/curl/tree/master/tests/data) against a local stub server and compares the exact bytes CurlDotNet puts on the wire with what curl sends. The current pass rate is published in each release's notes, and the remaining gaps are tracked in `tests/CurlDotNet.CurlSuite/known-failures.txt`. The unit suite (about 1,000 tests) runs offline against an in-process server in a few seconds.

Behaviour matches curl where a C# developer would notice: default headers, `-d` versus `--data-binary` versus `--data-urlencode`, `-L` semantics, `-i`/`-o` output, exit codes surfaced as exception types (`CurlCouldntConnectException` is 7, `CurlOperationTimeoutException` is 28, `CurlHttpReturnedErrorException` is 22), and the stripping of `Authorization` across hosts. Where .NET's `HttpClient` cannot do what curl does (HTTP/0.9, custom header ordering) the difference is documented rather than hidden.

## Supported platforms

Targets `netstandard2.0`, `net472`, `net48`, `net8.0`, `net9.0` and `net10.0`, so it runs on any runtime that supports .NET Standard 2.0: Windows, Linux and macOS; ASP.NET Core, Blazor Server, MAUI, WPF and WinForms; Azure Functions, AWS Lambda and Docker containers. HTTP, HTTPS, FTP, FTPS and `file://` URLs are supported.

## Documentation

- [Documentation home](https://jacob-mellor.github.io/curl-dot-net/)
- [How-to pages, one task each](docs/how-to/README.md)
- [Cookbook](docs/cookbook/index.md), [tutorials](docs/tutorials/index.md) and [troubleshooting](docs/troubleshooting/README.md)
- [curl to HttpClient migration guide](docs/migration/httpclient.md)
- [PowerShell guides](docs/powershell/README.md)
- [Exception reference](docs/exceptions/index.md): every exception says what happened, how to fix it, and links back here
- [Featured article: CurlDotNet - bringing curl superpowers to every corner of the .NET 10 and C# stack](https://dev.to/iron-software/curldotnet-bringing-curl-superpowers-to-every-corner-of-the-net-10-c-stack-1ol2)

## Contributing

Issues and pull requests are welcome at [github.com/jacob-mellor/curl-dot-net](https://github.com/jacob-mellor/curl-dot-net). See [CONTRIBUTING.md](CONTRIBUTING.md). Work happens on the `dev` branch; merging to `master` publishes to NuGet and redeploys the docs site.

```bash
dotnet build src/CurlDotNet/CurlDotNet.csproj -c Release --framework net10.0
dotnet test tests/CurlDotNet.Tests/CurlDotNet.Tests.csproj --framework net10.0 -c Release --filter "Category=Synthetic|Category=Unit"
dotnet test tests/CurlDotNet.CurlSuite/CurlDotNet.CurlSuite.csproj --framework net10.0 -c Release
```

## License and credits

MIT licensed. See [LICENSE](LICENSE).

Written by [Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/), CTO of [IronSoftware](https://ironsoftware.com), makers of IronPDF, IronOCR, IronXL and IronBarcode, who sponsor this project. CurlDotNet is part of the [UserLand.NET](https://userland.net) initiative that brings Unix userland tools to .NET as pure C#. The behaviour it reproduces, and the test suite it is measured against, are the work of Daniel Stenberg and the curl contributors.
