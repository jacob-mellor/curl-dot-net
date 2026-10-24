---
title: "CurlDotNet Cookbook: curl Recipes in C#"
description: "Copy-paste curl recipes in C# with CurlDotNet: GET, POST JSON, forms, file upload and download, redirects, cookies, proxies, timeouts, retries, write-out, globbing, config files, HEAD and auth."
---

# CurlDotNet Cookbook: curl recipes in C#

These curl recipes in C# show how to do one common HTTP task each with CurlDotNet, the pure .NET library that runs curl command strings. Every recipe gives the curl command first, the `Curl.ExecuteAsync` call that runs it, and the `CurlRequestBuilder` equivalent where one exists.

## Installation

```bash
dotnet add package CurlDotNet
```

Package page: [CurlDotNet on NuGet](https://www.nuget.org/packages/CurlDotNet/).

## Beginner recipes

Start here if you are new to CurlDotNet. See also the [beginner overview](beginner/README.html).

- [curl GET request in C#](beginner/simple-get.html) - fetch a URL, check the status, read headers
- [curl -X POST JSON in C#](beginner/send-json.html) - `-d`, `--json`, `WithJson`, `ParseJson<T>()`
- [curl -d form data POST in C#](beginner/post-form.html) - URL-encoded forms and `--data-urlencode`
- [curl -F file upload in C#](beginner/upload-file.html) - multipart/form-data, `-T` uploads and `--data-binary`
- [curl download file in C# (curl -o)](beginner/download-file.html) - `-o`, `-O`, `-J`, `-C -` resume, `Curl.DownloadAsync`
- [Handle curl errors and exceptions in C#](beginner/handle-errors.html) - status codes, `-f`, `EnsureSuccess()`, typed exceptions and exit codes
- [Call a REST API with curl in C#](beginner/call-api.html) - bearer tokens, verbs, a reusable client

## One flag at a time

- [curl follow redirects in C# (curl -L)](follow-redirects.html) - `-L`, `--max-redirs`, `CurlTooManyRedirectsException`
- [curl with cookies in C# (curl -b and -c)](cookies.html) - cookie strings, cookie files and `-c` cookie jars
- [curl proxy in C# (curl -x and SOCKS5)](proxy.html) - `-x`, `--proxy-user`, `--noproxy`, proxy environment variables, `--socks5`
- [curl timeout in C# (--max-time and --connect-timeout)](timeouts.html) - `-m`, `--connect-timeout`, no hidden default, `CurlOperationTimeoutException`
- [curl -I HEAD request in C#](head-request.html) - headers without the body
- [curl PUT, DELETE and PATCH requests in C#](http-methods.html) - `-X` with and without a body
- [curl digest, NTLM and basic auth in C#](auth-schemes.html) - `-u`, `--digest`, `--ntlm`, `--anyauth`, `.netrc`, bearer tokens
- [curl --retry in C#](retry.html) - `--retry`, `--retry-delay`, `--retry-max-time`, `WithRetries`
- [curl -w write-out in C#](write-out.html) - `%{http_code}`, `%{time_total}`, `%{json}`, output on failure
- [curl multiple URLs and URL globbing in C#](multiple-urls-and-globbing.html) - several URLs, `{a,b}`, `[1-10]`, `#1`, `--next`
- [curl config files, --variable and .netrc in C#](config-files-and-variables.html) - `-K`, `--variable`, `{{name:json}}`, `-n`

## Quick examples

### GET

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://api.github.com/zen");   // sends User-Agent: curl/8.7.1, like curl
Console.WriteLine(result.Body);
```

### POST JSON

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(@"
    curl -X POST https://httpbin.org/post \
         -H 'Content-Type: application/json' \
         -d '{""name"":""Ada""}'");

Console.WriteLine(result.IsSuccess ? "created" : $"HTTP {result.StatusCode}");
```

### Download a file

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -L -o image.png https://httpbin.org/image/png");
Console.WriteLine(result.OutputFiles[0]);
```

## Find a recipe by task

| I want to... | Recipe |
|--------------|--------|
| Fetch data from a URL | [curl GET request](beginner/simple-get.html) |
| Send JSON to an API | [curl -X POST JSON](beginner/send-json.html) |
| Submit an HTML-style form | [curl -d form data](beginner/post-form.html) |
| Upload a file | [curl -F file upload](beginner/upload-file.html) |
| Save a response to disk | [curl download file](beginner/download-file.html) |
| Update or delete a resource | [PUT, DELETE and PATCH](http-methods.html) |
| Log in with a username and password | [digest, NTLM and basic auth](auth-schemes.html) |
| Send a session cookie | [cookies](cookies.html) |
| Go through a corporate proxy | [proxy](proxy.html) |
| Stop a slow request | [timeout](timeouts.html) |
| Check headers without downloading | [HEAD request](head-request.html) |
| Follow a 301/302 | [follow redirects](follow-redirects.html) |
| Deal with failures | [handle errors](beginner/handle-errors.html) |
| Retry a flaky endpoint | [retry](retry.html) |
| Print the status code or timing | [write-out](write-out.html) |
| Fetch a numbered series of URLs | [multiple URLs and globbing](multiple-urls-and-globbing.html) |
| Keep options or credentials in a file | [config files, variables and .netrc](config-files-and-variables.html) |

## How the recipes are written

Each page states the problem, shows the curl command and the C# that runs it, then the builder version, a few variations, the things that commonly go wrong, and links onward. Every snippet is short enough to paste into a console app with `using CurlDotNet;` (and `using CurlDotNet.Core;` for the builder and `CurlResult`).

## More documentation

- [Tutorials](../tutorials/) - a step-by-step learning path
- [Getting started](../getting-started/first-request.html) - installation and your first request
- [API reference](../api/) - generated from the source
- [Troubleshooting](../troubleshooting/common-issues.html) - common issues and fixes
- [Exceptions](../exceptions/) - every exception type and when it is thrown
- [curl exit codes](../reference/curl-exit-codes.html) - every exit code and the exception it maps to

Questions and bug reports: [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues).
