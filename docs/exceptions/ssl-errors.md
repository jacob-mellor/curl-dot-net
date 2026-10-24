---
title: "curl SSL Certificate Errors in C#: -k and Insecure()"
description: "Fix curl SSL certificate errors in C#: CurlPeerFailedVerificationException (60) and CurlSslConnectErrorException (35) in CurlDotNet; trust a private CA with --cacert, pin keys with --pinnedpubkey, send client certificates, and when -k is OK."
---

# curl SSL Certificate Errors in C# (TLS Handshake Failures)

curl SSL certificate errors in C# happen when the TLS handshake fails: the server's certificate is self-signed, expired, issued for a different host name, or signed by a CA your machine does not trust, or the two sides cannot agree on a TLS version. In CurlDotNet a rejected server certificate throws `CurlPeerFailedVerificationException` (curl exit code 60) and any other handshake failure throws `CurlSslConnectErrorException` (35). Both derive from `CurlSslException`, so one `catch` covers every TLS failure. This page shows how to catch each one, how to fix the cause (`--cacert` for a private CA, the right host name, a client certificate), and when `-k` is acceptable.

## What Is Thrown

| Exception | `CurlErrorCode` | Thrown when |
|-----------|----:|-------------|
| `CurlPeerFailedVerificationException` | 60 | The server's certificate was rejected: self-signed, expired, issued for another host name, or signed by a CA that is not trusted (by the system store, or by your `--cacert`/`--capath` bundle when you give one) |
| `CurlSslConnectErrorException` | 35 | The handshake failed for another reason: no common TLS version (for example `--tlsv1.3` against a TLS 1.2-only server), no common cipher, a client certificate the server refused, or a port that does not speak TLS (`https://` to a plain-HTTP port). Also thrown before connecting when `--tls-max` is lower than the `--tlsv1.x` minimum |
| `CurlSslCertificateProblemException` | 58 | Your own client certificate (`--cert`/`--key`) could not be loaded: missing file, wrong format or password, key that does not match |
| `CurlSslCaCertBadFileException` | 77 | The `--cacert` file or `--capath` folder does not exist, cannot be read, or holds no certificate |
| `CurlSslPinnedPublicKeyMismatchException` | 90 | The server's public key matches none of the `--pinnedpubkey` pins (checked even with `-k`), or the pin value cannot be read |
| `CurlSslException` | (varies) | Base class of all of the above; catch it to handle every TLS failure |

The 58, 77 and 90 cases, and the `--cacert`/`--capath`/`--pinnedpubkey`/`--cert` options that cause them, need .NET 5 or later (see [Platform support](#platform-support)).

```csharp
using System;
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl https://self-signed.badssl.com/");
    Console.WriteLine(result.StatusCode);
}
catch (CurlPeerFailedVerificationException ex)
{
    // exit code 60: the certificate itself was rejected
    Console.WriteLine($"Certificate rejected: {ex.WhatHappened}");
    foreach (var fix in ex.Suggestions)
    {
        Console.WriteLine($"- {fix}");
    }
}
catch (CurlSslConnectErrorException ex)
{
    // exit code 35: TLS version, cipher, client certificate or a non-TLS port
    Console.WriteLine($"TLS handshake failed: {ex.WhatHappened}");
}
catch (CurlSslException ex)
{
    // 58, 77, 90: client certificate, CA bundle or pinned key
    Console.WriteLine($"TLS problem ({ex.CurlErrorCode}): {ex.WhatHappened}");
}
```

Every message says what happened, how to fix it, and ends with a link to this page. `WhatHappened` holds the first line (for a self-signed server, `Peer certificate verification failed: the TLS certificate of self-signed.badssl.com port 443 was rejected (...)`, with the platform's reason in the parentheses), and `Suggestions` the fixes.

### Types That Are Not Thrown

`CurlSslCipherException` (59), `CurlSslEngineNotFoundException` (53), `CurlSslEngineSetFailedException` (54) and `CurlUseSslFailedException` (64) exist in `CurlDotNet.Exceptions` for parity with curl's exit codes, but nothing in the library throws them: a cipher list the server does not share fails the handshake with `CurlSslConnectErrorException` (35). They carry their own curl numbers and derive from `CurlSslException`.

## Telling It Apart from a Network Failure

A TLS failure has its own exception types, so `catch (CurlSslException)` never sees a refused connection (`CurlCouldntConnectException`, 7) or a DNS failure (`CurlCouldntResolveHostException`, 6; see [DNS and connection errors](dns-errors.md)). To confirm a certificate problem:

- Open the same URL in a browser: a certificate warning confirms it
- Run `curl -v https://host/` in a terminal and read the `SSL certificate problem` line
- Retry with `-k` once, as a diagnosis only: if it now succeeds, the certificate was the issue

```csharp
var probe = await Curl.ExecuteAsync("curl -k -s -I https://self-signed.badssl.com/");
Console.WriteLine(probe.StatusCode);   // 200: the network is fine, the certificate is not trusted
```

## Fixing the Root Cause

### Private or Corporate CA: --cacert

When the server's certificate comes from your organisation's CA (an intranet service, a TLS-inspecting proxy, a test environment), give CurlDotNet that CA as a PEM bundle. Like curl, `--cacert` replaces the system store for that request: only the CAs in the file are trusted, the server's intermediate certificates are used to build the chain, and the host name must still match.

```csharp
var result = await Curl.ExecuteAsync("curl --cacert corp-root-ca.pem https://intranet.example.com/api/status");
```

```csharp
using CurlDotNet.Core;

var options = new CurlOptions
{
    Url = "https://intranet.example.com/api/status",
    CaCertFile = "corp-root-ca.pem"   // one or more -----BEGIN CERTIFICATE----- blocks
};
var result = await Curl.ExecuteAsync(options);
```

`--capath DIR` loads every certificate file in a folder (OpenSSL's hashed `5ed36f99.0` names included) and can be combined with `--cacert`. A Windows `.cer` export works when saved as "Base-64 encoded X.509". A bundle that does not exist or holds no certificate throws `CurlSslCaCertBadFileException` (77) before anything is sent:

```csharp
try
{
    await Curl.ExecuteAsync("curl --cacert missing.pem https://intranet.example.com/");
}
catch (CurlSslCaCertBadFileException ex)
{
    Console.WriteLine($"Fix the CA bundle path: {ex.FilePath}");
}
```

To trust the CA for every program on the machine instead, install it in the operating system store (Windows certificate manager, macOS Keychain, or `/usr/local/share/ca-certificates` plus `update-ca-certificates` on Debian/Ubuntu); without `--cacert`, CurlDotNet uses that store.

### Self-Signed Certificate on Your Own Development Server

For local ASP.NET Core development, trust the development certificate once:

```bash
dotnet dev-certs https --trust
```

Or export the certificate as PEM and pass it with `--cacert`, which keeps verification on.

### Expired or Wrong-Host Certificate

This is a server-side problem. Renew the certificate, or call the host name it was issued for (`https://api.example.com`, not `https://10.0.0.5`). With `--resolve api.example.com:443:10.0.0.5` you can keep the right name while connecting to a specific address. Also check that the machine's clock is correct: a wrong date makes valid certificates look expired.

### Missing Intermediate Certificates

Some servers send only their own certificate. Configure the server to send the full chain, or include the intermediate in your `--cacert` bundle.

### TLS Version Mismatch

`--tlsv1.2` and `--tlsv1.3` set the minimum version and `--tls-max` the maximum. If the server cannot meet that range the handshake fails with `CurlSslConnectErrorException` (35). Whether TLS 1.3 is available depends on the operating system.

```csharp
var result = await Curl.ExecuteAsync("curl --tlsv1.2 --tls-max 1.3 https://api.example.com/");
```

## Pinning the Server's Public Key: --pinnedpubkey

`--pinnedpubkey` makes the request fail unless the server's public key is one you expect, which protects against a mis-issued certificate or an unexpected TLS-inspecting proxy. Give `sha256//` followed by the base64 SHA-256 hash of the server's public key (several pins separated by `;` during a key rotation), or the path of a PEM/DER public key file. Pins are checked after normal verification, and also with `-k`, like curl.

```csharp
try
{
    var result = await Curl.ExecuteAsync(
        "curl --pinnedpubkey \"sha256//YhKJKSzoTt2b5FP18fvpHo7fJYqQCjAa3HWY3tvRMwE=\" https://api.example.com/");
}
catch (CurlSslPinnedPublicKeyMismatchException ex)
{
    // exit code 90: the message shows the sha256// pin of the key the server presented
    Console.WriteLine(ex.WhatHappened);
}
```

Compute a pin with OpenSSL: `openssl x509 -in server.pem -pubkey -noout | openssl pkey -pubin -outform der | openssl dgst -sha256 -binary | base64`.

## Client Certificates (Mutual TLS): --cert and --key

For servers that require a client certificate, pass it with `--cert`. PEM is the default type; the private key can be in the same file or in `--key`, and an encrypted key needs `--pass`. A PKCS#12 file takes its password after a colon. A file that cannot be loaded throws `CurlSslCertificateProblemException` (58) before anything is sent; a certificate the server refuses usually fails the handshake with `CurlSslConnectErrorException` (35) (with TLS 1.3 some servers reject it only after the handshake, which then surfaces as a broken connection).

```csharp
// PEM certificate and key
var pem = await Curl.ExecuteAsync("curl --cert client.pem --key client.key https://mtls.example.com/orders");

// PKCS#12 (.p12/.pfx) with its password
var p12 = await Curl.ExecuteAsync("curl --cert client.p12:p12-password --cert-type P12 https://mtls.example.com/orders");
```

```csharp
using System;
using CurlDotNet.Core;

var options = new CurlOptions
{
    Url = "https://mtls.example.com/orders",
    CertFile = "client.p12",
    CertType = "P12",
    CertPassword = Environment.GetEnvironmentVariable("CLIENT_CERT_PASSWORD")
};
var result = await Curl.ExecuteAsync(options);
```

Passwords are never printed in exception messages or in `LoggingMiddleware` output.

## Skipping Verification (Development Only)

`-k`/`--insecure` disables certificate validation. Anyone on the network path can then impersonate the server, so use it only against your own local or test servers, and never ship it in production code. Prefer `--cacert` with the test CA, which keeps the connection verified.

```csharp
// WARNING: development and testing only
var result = await Curl.ExecuteAsync("curl -k https://localhost:5001/api/health");
```

```csharp
using CurlDotNet.Core;

var viaBuilder = await CurlRequestBuilder
    .Get("https://localhost:5001/api/health")
    .Insecure()
    .ExecuteAsync();

var viaSettings = await Curl.ExecuteAsync("curl https://localhost:5001/api/health", new CurlSettings().WithInsecure());
```

```csharp
#if DEBUG
Curl.DefaultInsecure = true;   // applies to every request in this process
#endif
```

See also [how to ignore SSL certificate errors in C#](../how-to/curl-ignore-ssl-certificate-csharp/README.md) for when this is and is not acceptable.

## Platform Support

| Option | .NET 8, 9, 10 | .NET Standard 2.0 / .NET Framework |
|--------|---------------|------------------------------------|
| `-k` / `--insecure` | Applied | Applied |
| `--tlsv1.x`, `--tls-max` | Applied | Applied |
| `--cacert`, `--capath` | Applied | Listed in `CurlOptions.UnsupportedOptions`; install the CA in the system store |
| `--cert`, `--key`, `--cert-type`, `--pass` | Applied | Listed in `UnsupportedOptions`; no client certificate is sent |
| `--pinnedpubkey` | Applied | Listed in `UnsupportedOptions`, and an https transfer fails with `CurlNotBuiltInException` (4) rather than skip the check |
| `--ciphers`, `--tls13-ciphers` | Applied where .NET can choose cipher suites (Linux), with IANA names such as `TLS_AES_256_GCM_SHA384`; otherwise listed in `UnsupportedOptions` | Listed in `UnsupportedOptions` |

Check before sending when the platform matters:

```csharp
var check = Curl.Validate("curl --cacert corp-root-ca.pem https://intranet.example.com/");
if (check.ParsedOptions != null && check.ParsedOptions.UnsupportedOptions.Count > 0)
{
    Console.WriteLine("Not applied here: " + string.Join(", ", check.ParsedOptions.UnsupportedOptions));
}
```

Like curl, the TLS files (`--cacert`, `--capath`, `--cert`, `--key`, pins) are only loaded for transfers that can make a TLS connection (an https URL or proxy, or `-L`), so a plain `http://` request is not failed by a missing CA file. The TLS options apply to the `HttpClient` CurlDotNet creates. When you pass your own `HttpClient` to `CurlEngine`, its handler decides certificate validation and client certificates, and `--pinnedpubkey` fails with `CurlNotBuiltInException` (4) instead of being skipped.

## Related Documentation

- [DNS and connection errors](dns-errors.md)
- [Authentication errors](auth-errors.md)
- [Timeout errors](timeout-errors.md)
- [Troubleshooting: common issues](../troubleshooting/common-issues.md)
- [Exception reference](index.md)
