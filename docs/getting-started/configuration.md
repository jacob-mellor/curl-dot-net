---
title: "Configure CurlDotNet - Timeouts, Redirects, Proxies and TLS in C#"
description: "Configure CurlDotNet in C#: process-wide defaults, timeouts, redirects, proxies, TLS, User-Agent and CurlSettings, with the curl flag for each."
---

# Configuration

Learn how to configure CurlDotNet for timeouts, SSL, proxies, and more.

## Global Settings

CurlDotNet provides static properties to configure default behavior for all requests. Out of the box they match curl: no timeout, no redirect following, certificates verified, and `User-Agent: curl/8.7.1`. Each property below is applied to every command that does not set the matching option itself:

```csharp
using CurlDotNet;

// Opt in to a default overall timeout of 30 seconds (like adding --max-time 30 to every command)
Curl.DefaultMaxTimeSeconds = 30;

// Set connection timeout (10 seconds to establish connection)
Curl.DefaultConnectTimeoutSeconds = 10;

// Enable redirect following globally
Curl.DefaultFollowRedirects = true;

// Skip SSL verification (DEVELOPMENT ONLY!)
#if DEBUG
Curl.DefaultInsecure = true;
#endif

// Identify your application instead of curl/8.7.1
Curl.DefaultUserAgent = "MyApp/2.0 (+https://example.com)";
```

## Timeout Configuration

Like curl, CurlDotNet has no timeout unless you set one: with neither `--max-time` nor `Curl.DefaultMaxTimeSeconds`, a request waits as long as the server keeps the connection open (the internal `HttpClient` has an infinite `Timeout`, so there is no hidden 100-second limit either).

### Max Time (Overall Timeout)

Sets the maximum time a request can take, from connecting to the last byte of the body. Decimals work (`--max-time 2.5`). Expiry throws `CurlOperationTimeoutException` (curl exit code 28):

```csharp
// Global default for all requests
Curl.DefaultMaxTimeSeconds = 30;

// Per-request override using curl option
var result = await Curl.ExecuteAsync("curl --max-time 60 https://slow-api.example.com");
```

### Connect Timeout

Sets how long to wait for the connection to be established. It is enforced on .NET 5 and later; on .NET Standard 2.0 and .NET Framework it is best-effort, so also set `--max-time` there:

```csharp
// Global default
Curl.DefaultConnectTimeoutSeconds = 10;

// Per-request override
var result = await Curl.ExecuteAsync("curl --connect-timeout 5 https://api.example.com");
```

### Timeout with CancellationToken

For more control, use a CancellationToken. A cancelled token throws the standard `OperationCanceledException`, while an expired `--max-time` throws `CurlOperationTimeoutException`:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

try
{
    var result = await Curl.ExecuteAsync(
        "curl https://api.example.com",
        cts.Token
    );
}
catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
{
    Console.WriteLine("Request was cancelled (the token's 30 seconds ran out)");
}
```

## SSL/TLS Configuration

### Ignoring SSL Errors (Development Only)

Use `-k` or `--insecure` to skip SSL certificate verification:

```csharp
// WARNING: Only use in development!
var result = await Curl.ExecuteAsync("curl -k https://localhost:5001");

// Or set globally for development
#if DEBUG
Curl.DefaultInsecure = true;
#endif
```

### Custom Certificates

On .NET 5 and later the curl TLS options are applied:

- `--cacert FILE` / `--capath DIR` trust a private or corporate CA (only those CAs, as in curl). A missing or empty bundle throws `CurlSslCaCertBadFileException` (exit code 77).
- `--cert FILE[:password]`, `--key`, `--cert-type PEM|DER|P12`, `--key-type` and `--pass` send a client certificate for mutual TLS. A file that cannot be used throws `CurlSslCertificateProblemException` (58).
- `--pinnedpubkey sha256//BASE64` pins the server's public key; a mismatch throws `CurlSslPinnedPublicKeyMismatchException` (90).
- `--tlsv1.0` ... `--tlsv1.3` and `--tls-max` choose the TLS versions; `--ciphers` / `--tls13-ciphers` take IANA suite names where .NET can restrict cipher suites (Linux), and are listed in `UnsupportedOptions` elsewhere.

```csharp
// private CA plus a client certificate in a PKCS#12 file
var result = await Curl.ExecuteAsync(
    "curl --cacert corp-ca.pem --cert client.p12:secret --cert-type P12 https://mtls.example.com/");
```

On .NET Standard 2.0 and .NET Framework the trust and client-certificate options are listed in `CurlOptions.UnsupportedOptions` and the operating system trust store is used, and `--pinnedpubkey` fails with exit code 4 rather than skip the check. See [SSL certificate errors](../exceptions/ssl-errors.html).

## Proxy Configuration

### HTTP Proxy

```csharp
// Use an HTTP proxy
var result = await Curl.ExecuteAsync(
    "curl -x http://proxy.example.com:8080 https://api.example.com"
);

// Proxy with authentication
var result = await Curl.ExecuteAsync(
    "curl -x http://user:pass@proxy.example.com:8080 https://api.example.com"
);

// Same thing with -U, and skip the proxy for internal hosts
var result = await Curl.ExecuteAsync(
    "curl -x http://proxy.example.com:8080 -U user:pass --noproxy localhost,.internal https://api.example.com"
);
```

As in curl, the `http_proxy`, `https_proxy`, `all_proxy` and `NO_PROXY` environment variables are honoured, `--proxy-digest`/`--proxy-ntlm`/`--proxy-anyauth` pick the proxy authentication scheme, and `-p` tunnels through the proxy with `CONNECT` (.NET 5 and later).

### SOCKS Proxy

```csharp
// SOCKS5 proxy
var result = await Curl.ExecuteAsync(
    "curl --socks5 localhost:9050 https://api.example.com"
);

// SOCKS5 with hostname resolution through proxy
var result = await Curl.ExecuteAsync(
    "curl --socks5-hostname localhost:9050 https://api.example.com"
);
```

## Redirect Behavior

### Following Redirects

```csharp
// Enable globally
Curl.DefaultFollowRedirects = true;

// Or per-request with -L flag
var result = await Curl.ExecuteAsync("curl -L https://short.link/abc");

// Limit maximum redirects
var result = await Curl.ExecuteAsync(
    "curl -L --max-redirs 5 https://example.com"
);
```

Redirects are not followed unless you pass `-L` or set `Curl.DefaultFollowRedirects`. When they are, curl's rules apply: 301 and 302 turn a POST into a GET (unless `--post301`/`--post302`), 303 turns everything but GET/HEAD into GET, 307 and 308 keep the method and body. `--max-redirs N` allows exactly N hops (`-1` for unlimited); one more throws `CurlTooManyRedirectsException` (exit code 47).

### Preserving Authentication on Redirects

By default the `Authorization` and `Cookie` headers are dropped when a redirect goes to a different host. `--location-trusted` (which implies `-L`) keeps them:

```csharp
// Keep auth when redirected to different host
var result = await Curl.ExecuteAsync(
    "curl -L --location-trusted -u user:pass https://api.example.com"
);
```

## Advanced Settings with CurlSettings

For per-call settings that are not part of the command string, use the `CurlSettings` class. The engine applies every member (timeouts, redirects and a redirect callback, `-k`, extra headers, User-Agent, proxy, a `CookieContainer`, decompression, buffer size, retries, progress and the cancellation token) to every URL of the command:

```csharp
using CurlDotNet.Core;

var settings = new CurlSettings()
    .WithTimeout(60)                          // like --max-time 60
    .WithConnectTimeout(10)                   // like --connect-timeout 10
    .WithRetries(count: 3, delayMs: 2000)     // retry any failure 3 times, 2 s apart
    .WithHeader("X-Request-ID", Guid.NewGuid().ToString());

var result = await Curl.ExecuteAsync(
    "curl https://example.com/large-file.zip -o large-file.zip",
    settings
);
```

`WithRetries` is the .NET shortcut for curl's `--retry N --retry-delay S --retry-all-errors`. In the command string, `--retry 3` retries only transient failures (timeouts and HTTP 408/429/500/502/503/504) with doubling back-off; see [retries](../cookbook/retry.html).

## HTTP Version

Force a specific HTTP version. `--http1.0`, `--http1.1`, `--http2`, `--http2-prior-knowledge` and `--http3` set the request's HTTP version (on .NET 5 and later, `--http2`/`--http3` allow falling back to a lower version when the server does not offer it, and `--http2-prior-knowledge`/`--http3-only` require it):

```csharp
// Force HTTP/1.1
var result = await Curl.ExecuteAsync(
    "curl --http1.1 https://api.example.com"
);

// Force HTTP/2
var result = await Curl.ExecuteAsync(
    "curl --http2 https://api.example.com"
);
```

## User Agent Configuration

Without one, CurlDotNet sends `User-Agent: curl/8.7.1` (from `Curl.DefaultUserAgent`) and `Accept: */*`, like curl. Set a custom user agent:

```csharp
// Using -A flag
var result = await Curl.ExecuteAsync(
    "curl -A 'MyApp/1.0' https://api.example.com"
);

// Using --user-agent
var result = await Curl.ExecuteAsync(
    "curl --user-agent 'MyApp/1.0 (Windows 10)' https://api.example.com"
);

// Send no User-Agent at all
var result = await Curl.ExecuteAsync("curl -A '' https://api.example.com");
```

## Cookie Configuration

### Sending Cookies

```csharp
// Send cookies with request
var result = await Curl.ExecuteAsync(
    "curl -b 'session=abc123; token=xyz' https://api.example.com"
);

// Load cookies from file
var result = await Curl.ExecuteAsync(
    "curl -b cookies.txt https://api.example.com"
);
```

A `-b` value containing `=` is a cookie string; without `=` it is a Netscape-format cookie file.

### Saving Cookies

```csharp
// Save cookies to file
var result = await Curl.ExecuteAsync(
    "curl -c cookies.txt https://api.example.com/login"
);

// Send them back on the next request
var next = await Curl.ExecuteAsync(
    "curl -b cookies.txt -c cookies.txt https://api.example.com/profile"
);
```

`-c` writes a Netscape cookie jar, as curl does. A cookie engine runs whenever `-b FILE`, `-c FILE` or `-j` is given, so cookies set during a redirect chain are sent on the following hops. See the [cookies recipe](../cookbook/cookies.html).

## DNS Configuration

### Custom DNS Servers

`--dns-servers` is accepted but listed in `CurlOptions.UnsupportedOptions`: CurlDotNet always uses the operating system's resolver. To steer one host name, use `--resolve` or `--connect-to` instead.

### DNS Override (Host Resolution)

```csharp
// Force hostname to resolve to specific IP
var result = await Curl.ExecuteAsync(
    "curl --resolve api.example.com:443:192.168.1.100 https://api.example.com"
);
```

## Speed Limits and Keep-Alive

`--limit-rate` caps the download speed (`CurlOptions.LimitRate`; `100k` is 102400 bytes per second), and `--speed-limit`/`--speed-time` abort a transfer that stays below a rate for a number of seconds with `CurlOperationTimeoutException` (exit code 28). Both apply to http and https response bodies.

```csharp
// at most 500 KB/s, and give up if it drops below 1 KB/s for 30 seconds
var result = await Curl.ExecuteAsync(
    "curl --limit-rate 500k --speed-limit 1024 --speed-time 30 -o big.zip https://example.com/big.zip");
```

`--keepalive-time` only switches TCP keep-alive on for FTP transfers; HTTP connections use HttpClient's pooling. `--interface`, `--dns-servers`, `--quote`, `--disable-epsv` and `--disable-eprt` are accepted and listed in `CurlOptions.UnsupportedOptions`.

## Environment-Specific Configuration

### Development

```csharp
public static class CurlConfiguration
{
    public static void ConfigureForDevelopment()
    {
        Curl.DefaultMaxTimeSeconds = 60;  // Longer timeout for debugging
        Curl.DefaultConnectTimeoutSeconds = 30;
        Curl.DefaultInsecure = true;  // Accept self-signed certs
        Curl.DefaultFollowRedirects = true;
    }
}
```

### Production

```csharp
public static class CurlConfiguration
{
    public static void ConfigureForProduction()
    {
        Curl.DefaultMaxTimeSeconds = 30;
        Curl.DefaultConnectTimeoutSeconds = 10;
        Curl.DefaultInsecure = false;  // Never skip SSL in production!
        Curl.DefaultFollowRedirects = true;
    }
}
```

### ASP.NET Core Integration

```csharp
// In Program.cs or Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // Configure CurlDotNet based on environment
    if (Environment.IsDevelopment())
    {
        Curl.DefaultInsecure = true;
        Curl.DefaultMaxTimeSeconds = 60;
    }
    else
    {
        Curl.DefaultInsecure = false;
        Curl.DefaultMaxTimeSeconds = 30;
    }
}
```

## Configuration Summary Table

| Setting | Global Property | Curl Option | Default |
|---------|-----------------|-------------|---------|
| Request timeout | `DefaultMaxTimeSeconds` | `--max-time` | 0 (none) |
| Connect timeout | `DefaultConnectTimeoutSeconds` | `--connect-timeout` | 0 (none) |
| Follow redirects | `DefaultFollowRedirects` | `-L` | false |
| Skip SSL verify | `DefaultInsecure` | `-k` | false |
| User agent | `DefaultUserAgent` | `-A` | `curl/8.7.1` |
| Proxy | (per-request, or `http_proxy`/`https_proxy` env vars) | `-x` | none |
| Max redirects | (per-request) | `--max-redirs` | 50 |
| Retries | (per-request, or `CurlSettings.WithRetries`) | `--retry` | 0 (none) |

## Next Steps

- [First Request](first-request.html) - Make your first request
- [Cookbook](../cookbook/) - Practical examples
- [Troubleshooting](../troubleshooting/) - Solve common issues
- [API Reference](../api/index.html) - Complete API documentation
