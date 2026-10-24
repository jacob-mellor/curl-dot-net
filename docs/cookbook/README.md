---
title: "CurlDotNet Cookbook Overview: curl Recipes in C#"
description: "CurlDotNet cookbook overview: short curl recipes in C# for GET, POST, forms, uploads, downloads, redirects, cookies, proxies, timeouts and auth."
---

# CurlDotNet Cookbook overview: curl recipes in C#

The CurlDotNet cookbook is a set of short curl recipes in C#, one HTTP task per page, each showing the curl command, the `Curl.ExecuteAsync` call that runs it and the `CurlRequestBuilder` equivalent. The full list with descriptions is on the [cookbook index](index.html); this page is the quick finder.

## Beginner recipes

- [curl GET request in C#](beginner/simple-get.html)
- [curl -X POST JSON in C#](beginner/send-json.html)
- [curl -d form data POST in C#](beginner/post-form.html)
- [curl -F file upload in C#](beginner/upload-file.html)
- [curl download file in C# (curl -o)](beginner/download-file.html)
- [Handle curl errors and exceptions in C#](beginner/handle-errors.html)
- [Call a REST API with curl in C#](beginner/call-api.html)

## One flag at a time

- [curl follow redirects in C# (curl -L)](follow-redirects.html)
- [curl with cookies in C# (curl -b)](cookies.html)
- [curl proxy in C# (curl -x and SOCKS5)](proxy.html)
- [curl timeout in C# (--max-time and --connect-timeout)](timeouts.html)
- [curl -I HEAD request in C#](head-request.html)
- [curl PUT, DELETE and PATCH requests in C#](http-methods.html)
- [curl digest, NTLM and basic auth in C#](auth-schemes.html)

## Quick finder

**Download something:** [a web page or API response](beginner/simple-get.html), [a file](beginner/download-file.html), [only the headers](head-request.html)

**Send something:** [JSON](beginner/send-json.html), [a form](beginner/post-form.html), [a file](beginner/upload-file.html), [PUT, PATCH or DELETE](http-methods.html)

**Get through:** [a proxy](proxy.html), [a login](auth-schemes.html), [a redirect](follow-redirects.html), [a cookie-protected page](cookies.html)

**Stay in control:** [timeouts](timeouts.html), [errors and retries](beginner/handle-errors.html)

## Test before you build

httpbin.org echoes whatever you send, which makes it ideal for trying a recipe:

```csharp
using CurlDotNet;

var test = await Curl.ExecuteAsync("curl -X POST https://httpbin.org/post -d 'test=data'");
Console.WriteLine(test.IsSuccess ? test.Body : $"HTTP {test.StatusCode}");
```

Start with the simplest version of a command, confirm it works, then add headers, auth and options one at a time.

## Contributing a recipe

Keep the format: the problem, the curl command, the C# that runs it, the builder version, variations, troubleshooting, related links. Every snippet must compile against the current public API and stay short enough to paste into a console app. Open a pull request on [GitHub](https://github.com/jacob-mellor/curl-dot-net).

## Related

- [Tutorials](../tutorials/)
- [Getting started](../getting-started/first-request.html)
- [Troubleshooting](../troubleshooting/common-issues.html)
