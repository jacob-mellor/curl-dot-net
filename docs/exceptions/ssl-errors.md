---
title: "curl SSL Certificate Errors in C#: -k and Insecure()"
description: "Fix curl SSL certificate errors in C#: CurlDotNet throws CurlCouldntConnectException on a failed TLS handshake; when -k / Insecure() is OK, and real fixes."
---

# curl SSL Certificate Errors in C# (TLS Handshake Failures)

curl SSL certificate errors in C# happen when the TLS handshake fails: the server's certificate is self-signed, expired, issued for a different host name, or signed by a CA your machine does not trust. In CurlDotNet the failure surfaces as `CurlCouldntConnectException` (the connection could not be established), and `-k`/`--insecure` or the builder's `Insecure()` skips verification for development. This page explains how to recognise the error, when skipping verification is acceptable, and how to fix the root cause.

## What Is Thrown

.NET reports a certificate failure as an `HttpRequestException` during the handshake, which CurlDotNet maps to `CurlCouldntConnectException`:

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl https://self-signed.badssl.com/");
}
catch (CurlCouldntConnectException ex)
{
    Console.WriteLine($"TLS connection to {ex.Host}:{ex.Port} failed");
}
```

`CurlSslException`, `CurlPeerFailedVerificationException` and `CurlSslCertificateProblemException` exist as types in `CurlDotNet.Exceptions`, but the current HTTP engine does not raise them for a failed handshake; catch `CurlCouldntConnectException`.

## Telling It Apart from a Network Failure

A plain connection failure and a certificate failure throw the same exception type. To tell them apart:

- Open the same URL in a browser: a certificate warning confirms a TLS problem
- Run `curl -v https://host/` in a terminal and read the `SSL certificate problem` line
- Retry with `-k`: if it now succeeds, the certificate was the issue

```csharp
var insecure = await Curl.ExecuteAsync("curl -k https://self-signed.badssl.com/");
Console.WriteLine(insecure.StatusCode);   // 200: so the certificate, not the network, was the problem
```

## Skipping Verification (Development Only)

`-k` disables certificate validation. Anyone on the network path can then impersonate the server, so never ship it in production code.

### -k / --insecure

```csharp
// WARNING: development and testing only
var result = await Curl.ExecuteAsync("curl -k https://localhost:5001/api/health");
```

### Builder: Insecure()

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://localhost:5001/api/health")
    .Insecure()
    .ExecuteAsync();
```

### Process-Wide Default

```csharp
#if DEBUG
Curl.DefaultInsecure = true;   // applies to every command in this process
#endif
```

`CurlSettings` also has `.WithInsecure()` for a single call.

## Fixing the Root Cause

### Self-Signed Certificate on Your Own Server

Trust the certificate on the machine running your code. For local ASP.NET Core development:

```bash
dotnet dev-certs https --trust
```

### Private or Corporate CA

Install the CA's root certificate in the operating system trust store (Windows certificate manager, macOS Keychain, or `/usr/local/share/ca-certificates` plus `update-ca-certificates` on Debian/Ubuntu). .NET's `HttpClient`, and therefore CurlDotNet, uses the system store.

### Expired or Wrong-Host Certificate

This is a server-side problem. Renew the certificate or request the host name that matches it. Browsers will show the same error.

### Missing Intermediate Certificates

Some servers send only the leaf certificate. Configure the server to send the full chain; test with an online TLS checker.

## Not Supported Yet

Command-line curl's `--cacert`, `--cert`, `--key` and `--pinnedpubkey` options are not implemented in CurlDotNet at this time. Use the operating system trust store as described above.

## Related Documentation

- [DNS and connection errors](dns-errors.html)
- [Authentication errors](auth-errors.html)
- [Troubleshooting: common issues](../troubleshooting/common-issues.html)
- [Exception reference](index.html)
