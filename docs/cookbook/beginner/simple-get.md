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

The leading `curl ` is optional. HTTP error statuses (4xx, 5xx) do not throw; check `result.IsSuccess` (true for 2xx) or `result.StatusCode`.

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
    .WithUserAgent("CurlDotNet-Recipe")
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
      -A 'CurlDotNet-Recipe'");

Console.WriteLine(result.StatusCode);
```

Multi-line verbatim strings with backslash continuations are parsed exactly like a shell would parse them.

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

`ParseJson<T>()` uses `System.Text.Json` (Newtonsoft.Json on .NET Standard 2.0), so property names must match the JSON unless you add `[JsonPropertyName]`. For a quick look without a class, `result.AsJsonDynamic()` returns a `dynamic`.

### Save the body to a file (-o)

```csharp
var result = await Curl.ExecuteAsync("curl -o repo.json https://api.github.com/repos/curl/curl -A CurlDotNet");

Console.WriteLine(result.OutputFiles[0]);   // "repo.json"

// or decide after the fact:
var page = await Curl.ExecuteAsync("curl https://httpbin.org/html");
await page.SaveToFileAsync("page.html");
```

Binary responses (images, zips) are detected from the Content-Type and land in `result.BinaryData` with `result.IsBinary == true`. See [Download a File](download-file.html).

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

More in the [timeouts recipe](../timeouts.html).

## Troubleshooting

**403 Forbidden from api.github.com.** GitHub rejects requests without a `User-Agent`. CurlDotNet does not add one automatically, so pass `-A 'MyApp'` or `.WithUserAgent("MyApp")`.

**404 Not Found.** Check the URL for typos and remember that `result.Body` usually contains the server's own error message; print it.

**Empty `result.Body`.** A 204 No Content reply, a `HEAD` request (`-I`), or a binary response (`result.IsBinary`, body in `result.BinaryData`) all leave `Body` empty. Check `result.StatusCode` and `result.GetHeader("Content-Type")`.

**`ParseJson<T>()` throws `InvalidOperationException`.** The body is empty or not JSON (often an HTML error page). Check `result.IsSuccess` first and inspect `result.Body`.

**`CurlCouldntConnectException`.** DNS failure, connection refused, or a TLS handshake failure. Check the host name, your network, and whether a proxy is required ([proxy recipe](../proxy.html)).

**The server redirected and I got a 301/302.** curl does not follow redirects unless you pass `-L`. See [Follow Redirects](../follow-redirects.html).

## Related

- [Call a REST API](call-api.html) - GET/POST/PUT/DELETE with a bearer token
- [Send JSON](send-json.html) - `curl -X POST` with a JSON body
- [Download a File](download-file.html) - `-o`, `-O` and binary responses
- [Handle Errors](handle-errors.html) - status codes versus exceptions
- [HEAD requests](../head-request.html) - `-I` for headers only
- [Tutorial: Understanding Results](../../tutorials/05-understanding-results.html)
- [Cookbook index](../index.html)
