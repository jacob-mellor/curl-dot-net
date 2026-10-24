---
title: "RestSharp vs CurlDotNet - curl for C# Compared"
description: "RestSharp vs CurlDotNet (curl for C#) side by side: GET, POST JSON, headers, auth, typed responses, uploads, timeouts, errors and a migration checklist."
---

# RestSharp vs CurlDotNet (curl for C#)

RestSharp vs CurlDotNet (curl for C#): both are .NET HTTP client libraries, but they start from different places. RestSharp wraps `HttpClient` in a request/response object model. CurlDotNet runs curl commands, so anything documented as a `curl ...` line works unchanged, and also offers a fluent builder and a reusable client for code that is written by hand. This page compares the two side by side and ends with a migration checklist.

## Key differences

| | RestSharp | CurlDotNet |
|--|-----------|------------|
| Primary input | `RestRequest` object | curl command string, `CurlRequestBuilder`, or `LibCurl` |
| Client lifetime | `RestClient` instance per base URL | Static `Curl.*` for one-offs; `LibCurl` or `CurlApiClient` for shared config |
| Response | `RestResponse` / `RestResponse<T>` | `CurlResult`, `ParseJson<T>()` |
| HTTP errors | `IsSuccessful == false`, optional `ThrowOnAnyError` | `IsSuccess == false`, optional `EnsureSuccess()` |
| Serializer | Pluggable (System.Text.Json, Newtonsoft, XML) | System.Text.Json on .NET 8+, Newtonsoft.Json on .NET Standard 2.0 |
| Interceptors | Yes | No; compose in your own code or use `CurlSettings` |
| Copy from API docs | Translate by hand | Paste the curl command |
| Native dependencies | None | None (pure C#) |

## GET request

RestSharp:

```csharp
var client = new RestClient("https://httpbin.org");
var request = new RestRequest("get");
var response = await client.ExecuteAsync(request);
Console.WriteLine(response.Content);
```

CurlDotNet:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
Console.WriteLine(result.Body);
```

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Get("https://httpbin.org/get").ExecuteAsync();
```

## POST JSON

RestSharp:

```csharp
var request = new RestRequest("post", Method.Post)
    .AddJsonBody(new { name = "Ada", age = 36 });
var response = await client.ExecuteAsync(request);
```

CurlDotNet:

```csharp
var result = await Curl.ExecuteAsync(@"curl -X POST https://httpbin.org/post
    -H 'Content-Type: application/json'
    -d '{""name"":""Ada"",""age"":36}'");
```

```csharp
var result = await CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithJson(new { name = "Ada", age = 36 })
    .ExecuteAsync();
```

## Headers and query parameters

RestSharp:

```csharp
var request = new RestRequest("get")
    .AddHeader("Accept", "application/json")
    .AddQueryParameter("page", "2")
    .AddQueryParameter("per_page", "50");
```

CurlDotNet puts query parameters in the URL, as curl does:

```csharp
var result = await Curl.ExecuteAsync(
    "curl -H 'Accept: application/json' 'https://httpbin.org/get?page=2&per_page=50'");
```

```csharp
var result = await CurlRequestBuilder.Get("https://httpbin.org/get?page=2&per_page=50")
    .WithHeader("Accept", "application/json")
    .ExecuteAsync();
```

## Authentication

RestSharp:

```csharp
var client = new RestClient(new RestClientOptions("https://httpbin.org")
{
    Authenticator = new HttpBasicAuthenticator("user", "passwd")
});
// or: new JwtAuthenticator("token123")
```

CurlDotNet:

```csharp
var basic  = await Curl.ExecuteAsync("curl -u user:passwd https://httpbin.org/basic-auth/user/passwd");
var bearer = await Curl.ExecuteAsync("curl -H 'Authorization: Bearer token123' https://httpbin.org/bearer");
```

```csharp
var basic  = await CurlRequestBuilder.Get("https://httpbin.org/basic-auth/user/passwd")
    .WithBasicAuth("user", "passwd").ExecuteAsync();
var bearer = await CurlRequestBuilder.Get("https://httpbin.org/bearer")
    .WithBearerToken("token123").ExecuteAsync();
```

## Typed responses

RestSharp:

```csharp
var response = await client.ExecuteAsync<Slideshow>(new RestRequest("json"));
var slideshow = response.Data;
```

CurlDotNet:

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/json");
var slideshow = result.ParseJson<SlideshowResponse>();
```

`ParseJson<T>()` is case-sensitive on .NET 8+; annotate properties with `[JsonPropertyName("...")]` when the API uses snake_case or lower-case names. `result.AsJsonDynamic()` returns a `dynamic` when you do not want a class.

## File upload

RestSharp:

```csharp
var request = new RestRequest("post", Method.Post)
    .AddFile("file", "report.pdf")
    .AddParameter("title", "Report");
```

CurlDotNet:

```csharp
var result = await Curl.ExecuteAsync("curl -F 'title=Report' -F 'file=@report.pdf' https://httpbin.org/post");
```

```csharp
var result = await CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithMultipartForm(
        fields: new Dictionary<string, string> { ["title"] = "Report" },
        files:  new Dictionary<string, string> { ["file"] = "report.pdf" })
    .ExecuteAsync();
```

## Timeout and retries

RestSharp:

```csharp
var client = new RestClient(new RestClientOptions("https://httpbin.org")
{
    Timeout = TimeSpan.FromSeconds(30)
});
// retries: typically Polly around ExecuteAsync
```

CurlDotNet:

```csharp
var result = await Curl.ExecuteAsync("curl -m 30 https://httpbin.org/delay/2");
```

```csharp
var settings = new CurlSettings().WithTimeout(30).WithRetries(count: 3, delayMs: 1000);
var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/503", settings);
```

## Error handling

RestSharp:

```csharp
var response = await client.ExecuteAsync(request);
if (!response.IsSuccessful)
    Console.WriteLine($"{(int)response.StatusCode}: {response.ErrorMessage}");
```

CurlDotNet:

```csharp
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/status/500");
    if (!result.IsSuccess)
        Console.WriteLine($"HTTP {result.StatusCode}: {result.Body}");
}
catch (CurlException ex)   // DNS, connect, timeout, SSL failures
{
    Console.WriteLine($"curl error {ex.CurlErrorCode}: {ex.Message}");
}
```

## Shared client and dependency injection

RestSharp:

```csharp
services.AddSingleton(_ => new RestClient("https://api.github.com"));
```

CurlDotNet offers two options. `CurlApiClient` is the closest equivalent to a `RestClient` bound to a base URL:

```csharp
using CurlDotNet;

services.AddSingleton(_ => new CurlApiClient("https://httpbin.org"));

// usage
var created = await api.PostJsonAsync("/post", new { name = "Ada" });
var fetched = await api.GetAsync("/get");
```

`LibCurl` carries default headers, auth, proxy and timeouts across any URL:

```csharp
using CurlDotNet.Lib;

services.AddSingleton(_ => new LibCurl()
    .WithUserAgent("MyApp/1.0")
    .WithBearerToken(token)
    .WithTimeout(TimeSpan.FromSeconds(20)));
```

## What RestSharp has that CurlDotNet does not

- **Request and response interceptors.** Wrap calls in your own method or decorator instead.
- **Pluggable serializers.** CurlDotNet uses System.Text.Json (Newtonsoft.Json on .NET Standard 2.0). For a custom serializer, deserialize `result.Body` yourself.
- **Automatic deserialization of error bodies into `T`.** Read `result.Body` and parse it when `IsSuccess` is false.

## What CurlDotNet has that RestSharp does not

- Run curl commands from API docs, bug reports and shell scripts unchanged.
- `ToCurlCommand()` on the builder, so every request can be reproduced from a terminal.
- `Curl.ToHttpClient`, `ToFetch`, `ToPythonRequests`, `ToPowershellCode` to generate code for other stacks.
- curl semantics for redirects, cookies, proxies, binary detection and timings (`result.Timings`).

## Migration checklist

- Replace `RestClient` + `RestRequest` pairs with `Curl.ExecuteAsync(curlString)` or `CurlRequestBuilder`.
- Replace `response.Content` with `result.Body` and `response.Data` with `result.ParseJson<T>()`.
- Replace `response.IsSuccessful` with `result.IsSuccess`; use `EnsureSuccess()` where you relied on `ThrowOnAnyError`.
- Move `AddQueryParameter` values into the URL.
- Replace authenticators with `-u` / `WithBasicAuth` or an `Authorization` header / `WithBearerToken`.
- Register `CurlApiClient` or `LibCurl` as a singleton where you registered `RestClient`.
- Add `[JsonPropertyName]` attributes where RestSharp's serializer settings handled naming for you.

## Related

- [curl to HttpClient in C#](httpclient.html)
- [API guide](../api-guide/README.html)
- [Quick start](../getting-started/quickstart.html)
