---
title: "CurlDotNet Documentation: Run curl Commands in C#"
description: "CurlDotNet documentation hub: run curl commands in C# and .NET. Tutorials, cookbook recipes, API guide, exception reference, troubleshooting and FAQ."
---

# CurlDotNet documentation: run curl commands in C#

CurlDotNet lets you run curl commands in C#: paste the command from an API's documentation into `Curl.ExecuteAsync` and read the result as a `CurlResult`. It is pure .NET with no native curl binary, and targets .NET Framework 4.8, .NET Standard 2.0 and .NET 8, 9 and 10. This page is the map of the documentation.

## Installation

```bash
dotnet add package CurlDotNet
```

Package page: [CurlDotNet on NuGet](https://www.nuget.org/packages/CurlDotNet/).

## Start here

- **New to .NET or to curl?** [What is .NET and C#?](tutorials/01-what-is-dotnet.html), then [Your first request](tutorials/04-your-first-request.html).
- **Know .NET, new to CurlDotNet?** [Installation](getting-started/installation.html), then [Your first request](getting-started/first-request.html), then the [cookbook](cookbook/).
- **Coming from HttpClient or RestSharp?** [Migration guides](migration/).

## The three ways to call it

Run a curl command string:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -H 'Accept: application/json' https://httpbin.org/get");
Console.WriteLine(result.StatusCode);
Console.WriteLine(result.Body);
```

Build the request in code:

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Get("https://httpbin.org/get")
    .WithHeader("Accept", "application/json")
    .ExecuteAsync();
```

Keep a reusable client with defaults:

```csharp
using CurlDotNet.Lib;

using var client = new LibCurl()
    .WithBearerToken("token123")
    .WithTimeout(TimeSpan.FromSeconds(30));

var result = await client.GetAsync("https://httpbin.org/bearer");
```

## Documentation sections

### [Tutorials](tutorials/)

Fourteen short lessons from "what is .NET" to debugging requests, written for readers with no prior HTTP experience.

### [Cookbook](cookbook/)

One task per page: [GET](cookbook/beginner/simple-get.html), [POST JSON](cookbook/beginner/send-json.html), [forms](cookbook/beginner/post-form.html), [upload](cookbook/beginner/upload-file.html), [download](cookbook/beginner/download-file.html), [errors](cookbook/beginner/handle-errors.html), [REST APIs](cookbook/beginner/call-api.html), [redirects](cookbook/follow-redirects.html), [cookies](cookbook/cookies.html), [proxies](cookbook/proxy.html), [timeouts](cookbook/timeouts.html), [HEAD](cookbook/head-request.html), [PUT/DELETE/PATCH](cookbook/http-methods.html), [auth schemes](cookbook/auth-schemes.html).

### [API guide](api-guide/README.html) and [API reference](api/)

The guide explains `Curl`, `CurlRequestBuilder`, `CurlResult`, `CurlSettings` and `LibCurl` with examples; the reference is generated from the source XML comments.

### [Guides](guides/)

[Usage guide](guides/usage-guide.html), [advanced topics](guides/advanced.html) and [logging and observability](guides/logging-observability.html).

### [Exceptions](exceptions/)

What each exception type means and when the library throws it: timeouts, connection failures, redirects, URLs, SSL, auth, cookies.

### Troubleshooting

[Common issues](troubleshooting/common-issues.html), the [error reference](troubleshooting/error-reference.html) and the [FAQ](troubleshooting/faq.html).

### Reference

[curl CLI compatibility](reference/curl-cli-compatibility.html) (which flags work today), [libcurl mapping](reference/libcurl-mapping.html), [framework compatibility](framework-compatibility.html), [supported protocols](supported-protocols.html) and [articles](articles/) on CI/CD and implementation patterns.

## Common tasks

| Task | Page |
|------|------|
| Make a GET request | [curl GET request in C#](cookbook/beginner/simple-get.html) |
| POST JSON | [curl -X POST JSON in C#](cookbook/beginner/send-json.html) |
| Download or upload a file | [download](cookbook/beginner/download-file.html), [upload](cookbook/beginner/upload-file.html) |
| Use a bearer token or basic auth | [auth schemes](cookbook/auth-schemes.html) |
| Set a timeout | [curl timeout in C#](cookbook/timeouts.html) |
| Handle a 404 or a timeout | [handle errors](cookbook/beginner/handle-errors.html) |
| Check which curl flags work | [curl CLI compatibility](reference/curl-cli-compatibility.html) |

## Getting help

- [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues) for bugs and feature requests
- [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions) for questions
- [Troubleshooting](troubleshooting/common-issues.html) for the problems people hit most often

---

*Documentation for CurlDotNet 9.7.x, updated October 2026.*
