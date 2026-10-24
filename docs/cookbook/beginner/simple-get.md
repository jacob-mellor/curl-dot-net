---
title: "curl GET Request in C# - CurlDotNet Recipe"
description: "Make a curl GET request in C# with CurlDotNet: run the curl command string, check the status code, read headers, parse JSON and save the body to a file."
---

# curl GET request in C#

A curl GET request in C# is one line with CurlDotNet: pass the same `curl` command you would type in a terminal to `Curl.ExecuteAsync`, then read `result.StatusCode` and `result.Body`. This recipe covers the plain GET, adding headers and query parameters, parsing the JSON reply, saving the body to a file, and the errors you are most likely to hit.

## The problem

You want to fetch a URL (a JSON API, a web page, a file) from a .NET application without translating curl options into `HttpClient` calls.

## Solution 1: run the curl command string

```csharp
using System;
using System.Threading.Tasks;
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

Console.WriteLine(result.StatusCode);   // 200
Console.WriteLine(result.Body);         // the response body as a string
```

The leading `curl ` is optional. As with curl, the request carries `User-Agent: curl/8.7.1` and `Accept: */*` unless you change them. HTTP error statuses (4xx, 5xx) do not throw unless you add `-f`; check `result.IsSuccess` (true for 2xx) or `result.StatusCode`.

```csharp
var result = await Curl.GetAsync("https://httpbin.org/get");

if (!result.IsSuccess)
    Console.WriteLine($"Request failed with HTTP {result.StatusCode}");
```

`Curl.GetAsync(url)` is a shortcut for the same thing. Add `-i` to the command if you also want the response headers printed into `result.Body`, or use `result.Headers` as shown below.

## Solution 2: the fluent builder

If you would rather not write a command string, `CurlRequestBuilder` builds the same request and produces the same `CurlResult`.

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://api.github.com/repos/curl/curl")
    .WithHeader("Accept", "application/vnd.github+json")
    .WithUserAgent("MyApp/1.0")   // optional: replaces the default curl/8.7.1
    .ExecuteAsync();

Console.WriteLine(result.StatusCode);
Console.WriteLine(result.GetHeader("Content-Type"));
```

`ToCurlCommand()` on the builder returns the equivalent curl string, which is handy for logging or for pasting into a terminal.

## Variations

### Send request headers (-H, -A)

```csharp
var result = await Curl.ExecuteAsync(@"
    curl https://api.github.com/repos/curl/curl \
      -H 'Accept: application/vnd.github+json' \
      -A 'MyApp/1.0'");

Console.WriteLine(result.StatusCode);
```

Multi-line verbatim strings with backslash continuations are parsed exactly like a shell would parse them. `-H` replaces a default header of the same name (`-H 'Accept: application/json'` replaces `*/*`), `-A` sets your own User-Agent, and `-H 'User-Agent:'` removes the header altogether.

### Read response headers

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

string contentType = result.GetHeader("Content-Type");  // "application/json"
bool hasEtag = result.HasHeader("ETag");

foreach (var header in result.Headers)                 // case-insensitive dictionary
    Console.WriteLine($"{header.Key}: {header.Value}");
```

### Add query parameters

Build the URL yourself and escape each value. Quote the URL if it contains `&`.

```csharp
string search = Uri.EscapeDataString("curl dot net");
string url = $"https://httpbin.org/get?q={search}&page=1";

var result = await Curl.ExecuteAsync($"curl '{url}'");
Console.WriteLine(result.Body);   // httpbin echoes the "args" it received
```

### Parse the JSON reply

```csharp
using System.Text.Json.Serialization;

public class HttpBinGet
{
    [JsonPropertyName("url")] public string Url { get; set; }
    [JsonPropertyName("origin")] public string Origin { get; set; }
}

var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
var data = result.ParseJson<HttpBinGet>();
Console.WriteLine($"{data.Url} from {data.Origin}");
```

`ParseJson<T>()` uses `System.Text.Json` (Newtonsoft.Json on .NET Standard 2.0) and matches property names case-insensitively, so `"url"` binds to `Url` without any attribute. The `[JsonPropertyName]` attributes above are only required for names that differ in more than case, such as snake_case. For a quick look without a class, `result.AsJsonDynamic()` returns a `dynamic`.

### Save the body to a file (-o)

```csharp
var result = await Curl.ExecuteAsync("curl -o repo.json https://api.github.com/repos/curl/curl");

Console.WriteLine(result.OutputFiles[0]);   // "repo.json"; result.Body is empty, as curl prints nothing

// or decide after the fact:
var page = await Curl.ExecuteAsync("curl https://httpbin.org/html");
await page.SaveToFileAsync("page.html");
```

Binary responses (images, zips) are detected from the Content-Type and the bytes, and land in `result.BinaryData` with `result.IsBinary == true`. See [Download a File](download-file.html).

### Set a timeout (-m)

```csharp
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/delay/2");
    Console.WriteLine(result.StatusCode);
}
catch (CurlOperationTimeoutException ex)
{
    Console.WriteLine($"Timed out after {ex.Timeout}");
}
```

There is no timeout unless you set one; `-m` covers the whole transfer and accepts decimals (`-m 2.5`). More in the [timeouts recipe](../timeouts.html).

## Troubleshooting

**403 Forbidden from api.github.com.** Usually rate limiting for unauthenticated calls; send a token. GitHub also rejects requests without a `User-Agent`, but CurlDotNet sends `curl/8.7.1` by default, so this only bites if you removed it with `-A ''` or `-H 'User-Agent:'`.

**404 Not Found.** Check the URL for typos and remember that `result.Body` usually contains the server's own error message; print it.

**Empty `result.Body`.** A 204 No Content reply, a binary response (`result.IsBinary`, body in `result.BinaryData`), or `-o`/`-O` (the body went to the file) all leave `Body` empty. Check `result.StatusCode` and `result.GetHeader("Content-Type")`. (`-I` is different: like curl, it prints the response headers into `Body`.)

**`ParseJson<T>()` throws `InvalidOperationException`.** The body is empty or not JSON (often an HTML error page); the JSON parser's error is the inner exception. Check `result.IsSuccess` first and inspect `result.Body`.

**`CurlCouldntResolveHostException` or `CurlCouldntConnectException`.** The host name could not be resolved (exit code 6), or the connection was refused (exit code 7). Check the host name, your network, and whether a proxy is required ([proxy recipe](../proxy.html)). TLS problems raise `CurlSslConnectErrorException` or `CurlPeerFailedVerificationException` instead.

**The server redirected and I got a 301/302.** curl does not follow redirects unless you pass `-L`. See [Follow Redirects](../follow-redirects.html).

## Related

- [Call a REST API](call-api.html) - GET/POST/PUT/DELETE with a bearer token
- [Send JSON](send-json.html) - `curl -X POST` with a JSON body
- [Download a File](download-file.html) - `-o`, `-O` and binary responses
- [Handle Errors](handle-errors.html) - status codes versus exceptions
- [HEAD requests](../head-request.html) - `-I` for headers only
- [Tutorial: Understanding Results](../../tutorials/05-understanding-results.html)
- [Cookbook index](../index.html)
