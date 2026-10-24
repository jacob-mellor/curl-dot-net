---
title: "How to Use curl -k (Ignore SSL Errors) in C#"
description: "curl -k in C#: skip TLS certificate validation for one request with CurlDotNet's -k flag or .Insecure(). When it is safe, and the risks to know."
---

# How to use curl -k (ignore SSL certificate errors) in C#

To use `curl -k` in C#, add `-k` (or `--insecure`) to the command you pass to `Curl.ExecuteAsync`, or call `.Insecure()` on `CurlRequestBuilder`. Certificate validation is turned off for that request only.

```bash
dotnet add package CurlDotNet
```

## The curl command

```csharp
using CurlDotNet;

// self-signed.badssl.com serves a deliberately self-signed certificate
var result = await Curl.ExecuteAsync("curl -k https://self-signed.badssl.com/");

Console.WriteLine(result.StatusCode);  // 200
```

Without `-k` the same request throws. In 9.7.0 a TLS validation failure surfaces as `CurlCouldntConnectException` (curl error code 7), not as `CurlSslException`, so catch `CurlException` if you want to handle both.

## The fluent builder

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://self-signed.badssl.com/")
    .Insecure()
    .ExecuteAsync();

Console.WriteLine(result.StatusCode);
```

With `LibCurl`, `curl.WithInsecureSsl()` applies it to every request that instance sends.

## Make it conditional

Turn it on only for local development, never as a blanket setting:

```csharp
using CurlDotNet.Core;

var isDev = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "Development";

var result = await CurlRequestBuilder
    .Get("https://localhost:5001/health")
    .Insecure(isDev)
    .ExecuteAsync();
```

`Curl.DefaultInsecure` also exists, but in 9.7.0 it is only read by `CurlSettings.FromDefaults()`; plain `Curl.ExecuteAsync(command)` calls ignore it. Prefer the explicit per-request flag.

## How it works

`-k` makes CurlDotNet build a dedicated `HttpClientHandler` for that request whose `ServerCertificateCustomValidationCallback` accepts any certificate. Requests without `-k` keep using the shared, validating client. Because each `-k` request gets its own handler, it does not reuse pooled connections, so it is slightly slower under load.

## Is it safe?

`-k` removes the protection against man-in-the-middle attacks. Use it for:

- local development with a self-signed or dev certificate
- internal test environments you control

Do not use it against production APIs or anything carrying credentials over an untrusted network. A better fix for development is to trust the dev certificate (`dotnet dev-certs https --trust` for ASP.NET Core).

## Limitations

`--cacert`, `--cert` and `--key` (custom CA bundles and client certificates) are not supported in 9.7.0. If you need mutual TLS or a pinned CA, configure an `HttpClientHandler` and use `HttpClient` directly.

## Related

- [Error handling](../curl-error-handling-csharp/README.md)
- [Proxy settings](../curl-proxy-csharp/README.md)
- [CurlDotNet vs HttpClient](../curl-vs-httpclient-csharp/README.md)
- [How CurlDotNet works](../how-curldotnet-works/README.md)
- [All how-to guides](../README.md)
