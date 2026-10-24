---
title: "CurlDotNet Documentation - curl for C# and .NET"
description: "CurlDotNet documentation. Run curl commands in C# and .NET with a pure-C# curl implementation. Install guide, quick start, API guide and migration guides."
---

# CurlDotNet documentation — curl for C# and .NET

CurlDotNet is curl for C# and .NET: a pure-C# reimplementation of the curl command line that lets you run a curl command string directly in your .NET application and get a typed result back. No native libcurl binding, no `Process.Start`, no external curl binary.

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -A 'MyApp/1.0' -H 'Accept: application/json' https://api.github.com/users/octocat");

Console.WriteLine(result.StatusCode);   // 200
Console.WriteLine(result.Body);         // JSON text
```

## Install CurlDotNet

```bash
dotnet add package CurlDotNet
```

CurlDotNet targets `netstandard2.0`, `net472`, `net48`, `net8.0`, `net9.0` and `net10.0`. See the [installation guide](getting-started/installation.html) for IDE, Docker and .NET Framework notes.

## Where to start

| You are... | Start here |
|------------|------------|
| New to curl | [New to curl? How to run curl commands in C#](new-to-curl.html) |
| New to .NET | [What is .NET?](tutorials/01-what-is-dotnet.html), then [Your first request](tutorials/04-your-first-request.html) |
| An experienced .NET developer | [Quick start](getting-started/quickstart.html), then the [API guide](api-guide/README.html) |
| Coming from HttpClient | [curl to HttpClient in C#](migration/httpclient.html) |
| Coming from RestSharp | [RestSharp vs CurlDotNet](migration/restsharp.html) |

## Three ways to use CurlDotNet

### Run a curl command string

Paste the command from API docs, Stack Overflow or a shell script. The leading `curl` is optional.

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(@"curl -X POST https://httpbin.org/post
    -H 'Content-Type: application/json'
    -d '{""name"":""Ada""}'");

if (result.IsSuccess)
    Console.WriteLine(result.Body);
```

### Build the request fluently

`CurlRequestBuilder` gives you IntelliSense and compile-time checking. It produces the same request as the equivalent curl command.

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithJson(new { name = "Ada" })
    .WithBearerToken("token123")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .ExecuteAsync();
```

### Reuse a configured client

`LibCurl` is an instance class you can register in dependency injection and share across requests.

```csharp
using CurlDotNet.Lib;

using var curl = new LibCurl()
    .WithUserAgent("MyApp/1.0")
    .WithBearerToken("token123")
    .WithTimeout(TimeSpan.FromSeconds(30));

var result = await curl.GetAsync("https://api.github.com/user");
```

## Common tasks

- **curl -X POST JSON in C#** — [Quick start: POST JSON](getting-started/quickstart.html#curl--x-post-json-in-c)
- **curl bearer token in C#** — [Quick start: bearer token](getting-started/quickstart.html#curl-bearer-token-in-c)
- **curl download file in C#** — [Advanced guide: downloads](guides/advanced.html#curl-download-file-in-c)
- **curl follow redirects in C#** — [Advanced guide: redirects](guides/advanced.html#curl-follow-redirects-in-c)
- **curl ignore SSL certificate in C#** — [Advanced guide: SSL](guides/advanced.html#curl-ignore-ssl-certificate-in-c)
- **curl with cookies in C#** — [Advanced guide: cookies](guides/advanced.html#curl-with-cookies-in-c)
- **curl proxy in C#** — [Advanced guide: proxies](guides/advanced.html#curl-proxy-in-c)
- **curl timeout in C#** — [Advanced guide: timeouts](guides/advanced.html#curl-timeout-in-c)
- **Convert a curl command to HttpClient code** — [`Curl.ToHttpClient`](migration/httpclient.html#convert-a-curl-command-to-c-httpclient-code)

## Documentation sections

| Section | What it covers |
|---------|----------------|
| [Getting started](getting-started/README.html) | Installation, quick start, first request, configuration |
| [Tutorials](tutorials/index.html) | Step-by-step lessons for developers new to .NET or HTTP |
| [Cookbook](cookbook/index.html) | Copy-paste recipes for common tasks |
| [API guide](api-guide/README.html) | The three APIs, `CurlResult`, settings and exceptions |
| [Guides](guides/index.html) | Advanced usage, logging and observability |
| [Migration](migration/index.html) | Moving from HttpClient or RestSharp |
| [Reference](reference/curl-cli-compatibility.html) | Which curl flags are supported |
| [Exceptions](exceptions/index.html) | Every exception type and how to fix it |
| [Troubleshooting](troubleshooting/README.html) | Common issues and FAQ |
| [API reference](api/index.html) | Generated from the XML documentation |

## Supported .NET versions

- .NET 10.0
- .NET 9.0
- .NET 8.0 (LTS)
- .NET Framework 4.7.2 and 4.8
- .NET Standard 2.0 (any runtime that implements it)

Details: [Framework compatibility](framework-compatibility.html).

## Getting help

- [GitHub issues](https://github.com/jacob-mellor/curl-dot-net/issues)
- [Troubleshooting](troubleshooting/README.html) and [FAQ](troubleshooting/faq.html)
- [Example projects](https://github.com/jacob-mellor/curl-dot-net/tree/master/examples)

CurlDotNet is sponsored by [Iron Software](https://ironsoftware.com).
