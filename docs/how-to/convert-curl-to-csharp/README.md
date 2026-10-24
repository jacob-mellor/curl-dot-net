---
title: "How to Convert curl to C# (HttpClient) Code"
description: "Convert curl to C#: run the curl string directly with CurlDotNet, or generate HttpClient, Python, fetch or PowerShell code with Curl.ToHttpClient."
---

# How to convert curl to C#

There are two ways to convert curl to C#: run the curl command directly with `Curl.ExecuteAsync` (no conversion at all), or generate starter code with `Curl.ToHttpClient(cmd)` and its siblings for Python, JavaScript and PowerShell.

```bash
dotnet add package CurlDotNet
```

## Option 1: do not convert, just run it

The most faithful conversion is none. `Curl.ExecuteAsync` accepts the command exactly as it appears in the API documentation:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -H 'Authorization: Bearer abc123' https://httpbin.org/bearer");

Console.WriteLine($"{result.StatusCode}: {result.Body}");
```

If you prefer typed code, the fluent builder expresses the same request and can print the curl command back out:

```csharp
using CurlDotNet.Core;

var request = CurlRequestBuilder
    .Get("https://httpbin.org/bearer")
    .WithBearerToken("abc123");

Console.WriteLine(request.ToCurlCommand());
var result = await request.ExecuteAsync();
```

## Option 2: generate HttpClient code

`Curl.ToHttpClient` returns C# source as a string:

```csharp
using CurlDotNet;

string code = Curl.ToHttpClient(
    "curl -H 'Authorization: Bearer t' https://api.example.com/me");
Console.WriteLine(code);
```

Output:

```text
using var client = new HttpClient();
client.DefaultRequestHeaders.Add("Authorization", "Bearer t");
var response = await client.GetAsync("https://api.example.com/me");
var body = await response.Content.ReadAsStringAsync();
```

## Other languages from the same curl string

```csharp
using CurlDotNet;

var cmd = "curl -X POST https://api.example.com/users -H 'Content-Type: application/json' -d '{\"name\":\"Ada\"}'";

Console.WriteLine(Curl.ToPythonRequests(cmd));  // Python requests
Console.WriteLine(Curl.ToFetch(cmd));           // JavaScript fetch()
Console.WriteLine(Curl.ToPowershellCode(cmd));  // Invoke-RestMethod
```

For that command, `ToPythonRequests` emits `requests.post(..., headers=headers, json={"name":"Ada"})` and `ToPowershellCode` emits a single `Invoke-RestMethod -Uri ... -Method POST -Headers @{...} -Body ... -ContentType "application/json"` line. See [curl to PowerShell code](../../powershell/curl-to-powershell-code/README.md) for details.

## Treat generated code as a starting point

The generators translate the URL, method, headers and body. In version 9.7.0 they do not carry over `-u`, `-k`, `-L`, `-m`, `-o`, `-F` or `-x`; add those by hand. Review generated C# before you commit it:

- JSON bodies are inserted without escaping the inner quotes, so fix the string literal (a raw string literal `"""..."""` is easiest).
- `Content-Type` is emitted as a `DefaultRequestHeaders` entry. HttpClient only accepts it on the content, so move it into the `StringContent` constructor.
- PATCH requests are emitted as `HttpMethod.PATCH`; use `HttpMethod.Patch` (.NET Core 2.1+) or `new HttpMethod("PATCH")`.

Running the curl string with `Curl.ExecuteAsync` has none of these issues, because no code is generated.

## Related

- [Run a curl command in C#](../run-curl-command-in-csharp/README.md)
- [CurlDotNet vs HttpClient](../curl-vs-httpclient-csharp/README.md)
- [curl to PowerShell code](../../powershell/curl-to-powershell-code/README.md)
- [How CurlDotNet works](../how-curldotnet-works/README.md)
- [All how-to guides](../README.md)
