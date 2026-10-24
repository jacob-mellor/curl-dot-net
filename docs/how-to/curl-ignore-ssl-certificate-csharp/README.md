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

Without `-k` the same request throws `CurlPeerFailedVerificationException` (curl error code 60), as the curl CLI exits with 60. A failed TLS handshake (for example a port that does not speak TLS) throws `CurlSslConnectErrorException` (code 35). Both derive from `CurlSslException`, so catch that to handle any TLS problem.

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

`Curl.DefaultInsecure = true` turns it on for every request in the process, including plain `Curl.ExecuteAsync(command)` calls. Prefer the explicit per-request flag, so production traffic is never affected by accident.

## How it works

`-k` makes CurlDotNet build a dedicated handler for that request whose certificate validation callback accepts any certificate (a `SocketsHttpHandler` on .NET 5 and later, an `HttpClientHandler` on netstandard2.0 and .NET Framework). Requests without `-k` keep using the shared, validating client. Because each `-k` request gets its own handler, it does not reuse pooled connections, so it is slightly slower under load.

## Is it safe?

`-k` removes the protection against man-in-the-middle attacks. Use it for:

- local development with a self-signed or dev certificate
- internal test environments you control

Do not use it against production APIs or anything carrying credentials over an untrusted network. A better fix for development is to trust the dev certificate (`dotnet dev-certs https --trust` for ASP.NET Core).

## Trust the certificate instead of skipping the check

When the server's certificate comes from a private or corporate CA, trust that CA rather than switching validation off. On .NET 5 and later `--cacert` (a PEM bundle) and `--capath` (a folder of certificates) work as in curl: only those CAs are trusted, and the host name is still checked.

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl --cacert corp-ca.pem https://intranet.example/");
Console.WriteLine(result.StatusCode);
```

A missing or empty bundle throws `CurlSslCaCertBadFileException` (exit code 77). The other TLS options are applied too: `--cert`/`--key` for mutual TLS and `--pinnedpubkey sha256//...` to pin the server's key (checked even with `-k`). On .NET Standard 2.0 and .NET Framework these options are listed in `CurlOptions.UnsupportedOptions` instead and the operating system trust store is used, so install a private CA there. See [SSL certificate errors](../../exceptions/ssl-errors.md).

## Related

- [Error handling](../curl-error-handling-csharp/README.md)
- [Proxy settings](../curl-proxy-csharp/README.md)
- [CurlDotNet vs HttpClient](../curl-vs-httpclient-csharp/README.md)
- [How CurlDotNet works](../how-curldotnet-works/README.md)
- [All how-to guides](../README.md)
