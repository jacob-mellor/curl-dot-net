---
title: "How to Send a curl Bearer Token in C#"
description: "Send a curl bearer token in C#: run curl -H 'Authorization: Bearer ...' with CurlDotNet, or use WithBearerToken on the fluent builder or LibCurl."
---

# How to send a curl bearer token in C#

To send a curl bearer token in C#, include `-H "Authorization: Bearer <token>"` in the command you pass to `Curl.ExecuteAsync`, or call `.WithBearerToken(token)` on `CurlRequestBuilder` or `LibCurl`.

```bash
dotnet add package CurlDotNet
```

## The curl command

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -H 'Authorization: Bearer abc123' https://httpbin.org/bearer");

Console.WriteLine(result.StatusCode);  // 200 (401 without the header)
```

## Keep the token out of your source code

The parser expands environment variables written as `$NAME`, `${NAME}`, `%NAME%` or `$env:NAME`, so a command copied from API docs works as soon as the variable is set:

```csharp
using CurlDotNet;

// API_TOKEN is set in the environment, for example by your CI system
var result = await Curl.ExecuteAsync(
    "curl -H 'Authorization: Bearer $API_TOKEN' https://httpbin.org/bearer");

Console.WriteLine(result.StatusCode);
```

Unlike bash, the expansion also happens inside single quotes. If the variable is not set, the text `$API_TOKEN` is sent literally and the API answers 401.

## The fluent builder

```csharp
using CurlDotNet.Core;

var token = Environment.GetEnvironmentVariable("API_TOKEN") ?? "";

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/bearer")
    .WithBearerToken(token)
    .ExecuteAsync();

Console.WriteLine(result.StatusCode);
```

`WithAuth("Token xyz")` sets an arbitrary `Authorization` header value for APIs that use another scheme.

## Reuse a token across many calls

`CurlDotNet.Lib.LibCurl` keeps default headers for every request it sends:

```csharp
using CurlDotNet.Lib;

using var curl = new LibCurl();
curl.WithBearerToken(Environment.GetEnvironmentVariable("API_TOKEN") ?? "");

var me = await curl.GetAsync("https://httpbin.org/bearer");
var other = await curl.GetAsync("https://httpbin.org/headers");
Console.WriteLine($"{me.StatusCode} {other.StatusCode}");
```

## Handle an expired or wrong token

A 401 or 403 does not throw. Check the status and refresh the token:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/bearer");
if (result.StatusCode == 401)
    Console.WriteLine("Token missing or expired - refresh it and retry.");
```

## Security notes

- `CurlResult.Command` holds the original command string, token included. Do not log it.
- `-v` output, which appears in `result.Body`, includes the request headers.

## Related

- [Basic authentication with curl -u](../curl-basic-auth-csharp/README.md)
- [Custom headers and User-Agent](../curl-custom-headers-user-agent-csharp/README.md)
- [Use CurlDotNet in ASP.NET Core](../curl-in-aspnet-core/README.md)
- [Paste curl commands from API docs](../../powershell/paste-curl-command-powershell-csharp/README.md)
- [All how-to guides](../README.md)
