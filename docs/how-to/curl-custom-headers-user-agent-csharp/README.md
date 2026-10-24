---
title: "How to Set curl Headers and User-Agent in C#"
description: "Set curl custom headers and User-Agent in C#: use -H, -A, -e and -b with CurlDotNet, or WithHeader and WithUserAgent; read response headers too."
---

# How to set curl custom headers and User-Agent in C#

To set custom headers in C# the curl way, repeat `-H "Name: value"` in the command you pass to `Curl.ExecuteAsync`; use `-A` for the User-Agent, `-e` for the Referer and `-b` for cookies. The fluent builder has `WithHeader`, `WithUserAgent`, `WithReferer` and `WithCookie`.

```bash
dotnet add package CurlDotNet
```

## The curl command

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("""
    curl https://httpbin.org/headers \
      -H "Accept: application/json" \
      -H "X-Api-Key: my-key" \
      -A "MyApp/1.0" \
      -e "https://example.com/start"
    """);

Console.WriteLine(result.Body);  // httpbin echoes the headers it received
```

## Set a User-Agent: CurlDotNet sends none by default

The curl CLI always sends `User-Agent: curl/<version>`. CurlDotNet sends no User-Agent unless you set one. Most servers do not care, but some do; the GitHub REST API answers 403 without one:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -A 'my-app/1.0' https://api.github.com/repos/jacob-mellor/curl-dot-net");

Console.WriteLine(result.StatusCode);  // 200 (403 without -A)
```

## The fluent builder

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/headers")
    .WithHeader("Accept", "application/json")
    .WithHeaders(new Dictionary<string, string> { ["X-Api-Key"] = "my-key" })
    .WithUserAgent("MyApp/1.0")
    .WithReferer("https://example.com/start")
    .ExecuteAsync();
```

## Send cookies

`-b` takes a cookie string (`name=value; name2=value2`):

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -b 'session=abc; theme=dark' https://httpbin.org/cookies");

Console.WriteLine(result.Body);  // {"cookies": {"session": "abc", "theme": "dark"}}
```

The builder equivalent is `.WithCookie("session=abc; theme=dark")`. Cookie jar files (`-c`) are not supported, so persist cookies yourself if you need them across runs.

## Read response headers

`result.Headers` is a case-insensitive dictionary that holds both response and content headers. Multi-value headers are joined with `", "`.

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/json");

Console.WriteLine(result.Headers["content-type"]);     // application/json
Console.WriteLine(result.GetHeader("Content-Length"));
Console.WriteLine(result.HasHeader("ETag"));
```

`-i` also puts the status line and headers at the start of `result.Body`, as the CLI prints them.

## Default headers for every request

`LibCurl` keeps defaults that it merges into each call:

```csharp
using CurlDotNet.Lib;

using var curl = new LibCurl();
curl.WithUserAgent("MyApp/1.0").WithHeader("X-Api-Key", "my-key");
var result = await curl.GetAsync("https://httpbin.org/headers");
```

## Related

- [Bearer token authentication](../curl-bearer-token-csharp/README.md)
- [HEAD requests and response headers](../curl-head-request-csharp/README.md)
- [Paste curl commands from API docs](../../powershell/paste-curl-command-powershell-csharp/README.md)
- [All how-to guides](../README.md)
