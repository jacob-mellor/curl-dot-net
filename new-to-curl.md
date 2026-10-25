---
title: "New to curl? How to run curl commands in C#"
description: "New to curl? Learn what a curl command is, how to read its flags, and how to run curl commands in C# and .NET with CurlDotNet, a pure-C# curl."
---

# New to curl? How to run curl commands in C#

New to curl? This page explains what curl is, how to read a curl command, and how to run curl commands in C# with CurlDotNet. If you have ever copied a `curl ...` line from API documentation and wondered how to turn it into .NET code, this is the answer: you do not have to translate it. You run it.

## What is curl?

curl is a command-line tool for transferring data over URLs. It has been developed continuously since 1996, ships with Windows 10+, macOS and most Linux distributions, and is the format almost every REST API uses to document its endpoints. A curl command is a complete, precise description of an HTTP request: method, URL, headers, body and options.

## How to read a curl command

```bash
curl -X POST https://httpbin.org/post \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer token123" \
     -d '{"name":"Ada"}'
```

| Part | Meaning |
|------|---------|
| `curl` | The tool. Optional in CurlDotNet. |
| `-X POST` | HTTP method. GET is the default. |
| `https://httpbin.org/post` | The URL. |
| `-H "Name: value"` | A request header. Repeat for each header. |
| `-d '...'` | The request body. Implies POST if no `-X` is given. |
| `\` | Shell line continuation. CurlDotNet accepts multi-line strings with or without it. |

Other flags you will see constantly: `-u user:pass` (basic auth), `-L` (follow redirects), `-k` (ignore SSL certificate errors), `-o file` (save to file), `-F field=value` (multipart form), `-m 30` (timeout in seconds), `-x proxy` (use a proxy), `-v` (verbose output).

## What is CurlDotNet?

CurlDotNet is a pure-C# reimplementation of curl for .NET. It parses a curl command string and performs the request with .NET's own networking stack. There is no native libcurl dependency and no external process is started, so it runs anywhere .NET runs: Windows, macOS, Linux, containers and Azure Functions, on .NET 8, 9 and 10 and on .NET Framework 4.7.2+.

## Run your first curl command in C#

Install the package, then paste the command into `Curl.ExecuteAsync`.

```bash
dotnet add package CurlDotNet
```

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

Console.WriteLine(result.StatusCode);  // 200
Console.WriteLine(result.Body);        // the JSON response
```

Like curl, CurlDotNet sends `User-Agent: curl/8.7.1` and `Accept: */*` unless you set your own, so APIs that insist on a User-Agent, such as GitHub's, just work. Use `-A 'MyApp/1.0'` to name your app instead.

The same request with the fluent builder, if you prefer typed C# over a command string:

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Get("https://api.github.com/users/octocat")
    .WithUserAgent("MyApp/1.0")
    .ExecuteAsync();
```

## Reading the result

Every call returns a `CurlResult`:

| Member | What it holds |
|--------|---------------|
| `StatusCode` | HTTP status as an `int`, e.g. `200`, `404` |
| `IsSuccess` | `true` when the status is 2xx |
| `Body` | What curl would print: the response body as a string (empty with `-o`, which writes the file instead) |
| `Headers` | Response headers (case-insensitive dictionary) |
| `BinaryData` | Response bytes when the content is binary (images, PDFs, zips) |
| `ParseJson<T>()` | Deserialize the body into your own class |

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

if (!result.IsSuccess)
{
    Console.WriteLine($"Request failed with HTTP {result.StatusCode}");
    return;
}

Console.WriteLine(result.GetHeader("Content-Type"));
```

## Common curl commands in C#

### curl -X POST JSON in C#

```csharp
var result = await Curl.ExecuteAsync(@"curl -X POST https://httpbin.org/post
    -H 'Content-Type: application/json'
    -d '{""name"":""Ada"",""language"":""C#""}'");
```

Or let CurlDotNet serialize an object for you:

```csharp
var result = await Curl.PostJsonAsync("https://httpbin.org/post",
    new { name = "Ada", language = "C#" });
```

### curl bearer token in C#

```csharp
var result = await Curl.ExecuteAsync(
    "curl -H 'Authorization: Bearer token123' https://httpbin.org/bearer");
```

### curl download file in C#

```csharp
var result = await Curl.ExecuteAsync("curl -o logo.png https://httpbin.org/image/png");
Console.WriteLine(result.OutputFiles[0]);   // logo.png
```

### curl basic auth in C#

```csharp
var result = await Curl.ExecuteAsync("curl -u user:passwd https://httpbin.org/basic-auth/user/passwd");
```

## What happens when something goes wrong

An HTTP error such as 404 or 500 does not throw. The call returns normally with `IsSuccess == false` and the status code and body available, exactly as curl would print them. Add `-f`/`--fail` and an HTTP error of 400 or above throws `CurlHttpReturnedErrorException` instead, just as curl exits with code 22. Network-level failures (DNS lookup failed, connection refused, timeout) throw a `CurlException` from the `CurlDotNet.Exceptions` namespace, with a `CurlErrorCode` matching curl's exit codes (see [curl exit codes](reference/curl-exit-codes.html)) and a `Suggestions` list describing how to fix the problem. As with curl, there is no timeout unless you set one with `-m`.

```csharp
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -m 5 https://httpbin.org/delay/10");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Timed out: {ex.Message}");
}
```

## Next steps

1. [Install CurlDotNet](getting-started/installation.html)
2. [Quick start: your first curl command in C# in 2 minutes](getting-started/quickstart.html)
3. [Tutorial: what is curl?](tutorials/02-what-is-curl.html) for a slower, fuller introduction
4. [Cookbook](cookbook/index.html) for copy-paste recipes
5. [Which curl flags are supported](reference/curl-cli-compatibility.html)

Questions or bugs: [GitHub issues](https://github.com/jacob-mellor/curl-dot-net/issues).
