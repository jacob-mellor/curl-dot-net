---
title: "curl SSL Certificate Errors in C#: -k and Insecure()"
description: "Fix curl SSL certificate errors in C#: CurlPeerFailedVerificationException (60) and CurlSslConnectErrorException (35) in CurlDotNet; when -k / Insecure() is OK, and real fixes."
---

# curl SSL Certificate Errors in C# (TLS Handshake Failures)

curl SSL certificate errors in C# happen when the TLS handshake fails: the server's certificate is self-signed, expired, issued for a different host name, or signed by a CA your machine does not trust. In CurlDotNet a rejected certificate throws `CurlPeerFailedVerificationException` (curl exit code 60) and any other handshake failure `CurlSslConnectErrorException` (35); both derive from `CurlSslException`. `-k`/`--insecure` or the builder's `Insecure()` skips verification for development. This page explains how to recognise the error, when skipping verification is acceptable, and how to fix the root cause.

## What Is Thrown

.NET reports a TLS failure during the handshake; CurlDotNet maps it to curl's exit codes:

| Exception | `CurlErrorCode` | Thrown when |
|-----------|----:|-------------|
| `CurlPeerFailedVerificationException` | 60 | The server's certificate was rejected: self-signed, expired, issued for another host name, or signed by a CA the machine does not trust |
| `CurlSslConnectErrorException` | 35 | The handshake failed for another reason: no common TLS version or cipher, or the port does not speak TLS (for example `https://` to a plain-HTTP port) |
| `CurlSslException` | 60 | Base class of both; catch it to cover every TLS failure |

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl https://self-signed.badssl.com/");
}
catch (CurlPeerFailedVerificationException ex)
{
    Console.WriteLine($"Certificate rejected: {ex.Message}");   // exit code 60
}
catch (CurlSslException ex)
{
    Console.WriteLine($"TLS handshake failed ({ex.CurlErrorCode}): {ex.Message}");
}
```

### Reserved TLS Types

`CurlSslCertificateProblemException`, `CurlSslCipherException`, `CurlSslEngineNotFoundException`, `CurlSslEngineSetFailedException` and `CurlUseSslFailedException` exist in `CurlDotNet.Exceptions` for parity with curl's codes 58, 59, 53, 54 and 64, but nothing in the library throws them today. Because they derive from `CurlSslException`, they carry `CurlErrorCode` 60 rather than their own curl number.

## Telling It Apart from a Network Failure

A TLS failure has its own exception type, so `catch (CurlSslException)` never sees a refused connection (`CurlCouldntConnectException`, 7) or a DNS failure (`CurlCouldntResolveHostException`, 6). To confirm the cause:

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

## Client Certificates and Custom CAs

Command-line curl's `--cacert`, `--capath`, `--cert`, `--key` and `--pinnedpubkey` are parsed and stored (`CurlOptions.CaCertFile`, `CertFile`, `KeyFile`), but the transport does not apply them, so they neither help nor fail: client certificates and custom CA files are not supported. Trust a private CA through the operating system store as described above. For a mutual-TLS endpoint, call it with `HttpClient` directly; `Curl.ToHttpClient(command)` generates equivalent `HttpClient` code to start from, and you add the certificate to its `HttpClientHandler.ClientCertificates`.

## Related Documentation

- [DNS and connection errors](dns-errors.html)
- [Authentication errors](auth-errors.html)
- [Troubleshooting: common issues](../troubleshooting/common-issues.html)
- [Exception reference](index.html)
