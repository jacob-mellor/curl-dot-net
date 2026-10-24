---
title: "Common CurlDotNet Issues and Fixes (curl errors in C#)"
description: "Common CurlDotNet issues and fixes (curl errors in C#): installation, DNS and connection failures, timeouts, TLS, HTTP 4xx/5xx, JSON and build errors."
---

# Common CurlDotNet Issues and Fixes (curl errors in C#)

Common CurlDotNet issues and fixes (curl errors in C#): this page lists the problems people hit most often when running curl commands through CurlDotNet, what causes each one, and the code or command that fixes it. Each section names the exception or status code you will see, then shows the fix.

## Contents

- [Installation Issues](#installation-issues)
- [DNS and Connection Errors](#dns-and-connection-errors)
- [Timeout Errors](#timeout-errors)
- [SSL Certificate Errors](#ssl-certificate-errors)
- [Authentication Errors](#authentication-errors)
- [HTTP Errors](#http-errors)
- [JSON Parsing Errors](#json-parsing-errors)
- [Compilation Errors](#compilation-errors)
- [Runtime Errors](#runtime-errors)
- [Error Handling Practices](#error-handling-practices)

Remember the one rule that explains most confusion: **transport failures throw, HTTP status codes do not** (unless the command has `-f`). A 404 or 500 comes back as a `CurlResult` with `IsSuccess == false`; a DNS failure, refused connection, TLS failure or timeout throws a `CurlException` subclass whose `CurlErrorCode` is the exit code the curl tool would return. A cancelled `CancellationToken` throws `OperationCanceledException`.

## Installation Issues

### The dotnet command is not found

**Problem:** After installing .NET, `dotnet` is not recognised.

**Cause:** The SDK directory is not on `PATH`, or the terminal was opened before the install finished.

**Fix on Windows:**

```powershell
# 1. Close and reopen the terminal

# 2. Still missing? Add the SDK directory for this session
$env:Path += ";C:\Program Files\dotnet"

# 3. Make it permanent through System Properties > Environment Variables
```

**Fix on macOS and Linux:**

```bash
# 1. Close and reopen the terminal

# 2. Still missing? Add the SDK directory
export PATH=$PATH:/usr/local/share/dotnet

# 3. Make it permanent
echo 'export PATH=$PATH:/usr/local/share/dotnet' >> ~/.zshrc
```

### Package CurlDotNet not found

**Problem:** `dotnet add package CurlDotNet` reports that the package cannot be found.

**Cause:** nuget.org is not configured as a source, or a corporate feed is intercepting the request.

**Fix:**

```bash
# 1. List sources
dotnet nuget list source

# 2. Add nuget.org if it is missing
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org

# 3. Clear the cache and retry
dotnet nuget locals all --clear
dotnet add package CurlDotNet
```

Or reference the package in the `.csproj` directly and run `dotnet restore`:

```xml
<ItemGroup>
  <PackageReference Include="CurlDotNet" Version="9.*" />
</ItemGroup>
```

### Target framework not supported

**Problem:** "The target framework 'netX.Y' is not supported" or the package will not restore.

**Cause:** The project targets a framework that is not installed, or one the package does not ship. CurlDotNet ships `net10.0`, `net9.0`, `net8.0`, `net48`, `net472` and `netstandard2.0`.

**Fix:**

```bash
dotnet --list-sdks
```

Then pick an installed target in the `.csproj`:

```xml
<!-- Current .NET -->
<TargetFramework>net10.0</TargetFramework>

<!-- Maximum reach (works from .NET Framework 4.7.2 upwards) -->
<TargetFramework>netstandard2.0</TargetFramework>

<!-- Several at once -->
<TargetFrameworks>net10.0;net8.0;netstandard2.0</TargetFrameworks>
```

See [Framework Compatibility](../framework-compatibility.html) and [Installation](../getting-started/installation.html).

## DNS and Connection Errors

### Could not resolve host or connection refused

**Problem:** `CurlCouldntResolveHostException: Could not resolve host: api.example.com` or `CurlCouldntConnectException: Failed to connect to api.example.com port 443: Connection refused`

**Cause:** The hostname does not resolve (typo, DNS outage) - `CurlCouldntResolveHostException`, exit code 6 (or `CurlCouldntResolveProxyException`, 5, when the proxy's name fails) - or it resolves but nothing is listening on the port or a firewall blocks the connection - `CurlCouldntConnectException`, exit code 7. TLS failures have their own types; see [SSL Certificate Errors](#ssl-certificate-errors).

**Fix:**

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -m 5 https://no-such-host.invalid/");
}
catch (CurlCouldntResolveHostException ex)
{
    Console.WriteLine($"Could not resolve {ex.Hostname}");
}
catch (CurlCouldntConnectException ex)
{
    Console.WriteLine($"Could not connect to {ex.Host}:{ex.Port}");
    Console.WriteLine(ex.Message);   // says what to check
}
```

**Troubleshooting steps:**

```bash
# Does the name resolve?
nslookup api.github.com        # Windows
dig api.github.com             # macOS / Linux

# Is the port reachable?
nc -vz api.github.com 443

# Does native curl reach it?
curl -v https://api.github.com
```

1. Check the hostname for typos (`api.examplee.com`).
2. Open the URL in a browser from the same machine; if that works but CurlDotNet does not, look for a proxy that the browser uses and the process does not (`-x http://proxy:8080`).
3. Confirm the port: `https://` defaults to 443, `http://` to 80.
4. If DNS is broken but the server is up, test with the IP address directly (testing only).

**Retrying transient failures:** add `--retry N --retry-connrefused` to the command (`--retry-all-errors` retries any failure); see [curl --retry in C#](../cookbook/retry.html). Or loop yourself:

```csharp
using CurlDotNet.Core;

async Task<CurlResult> ExecuteWithRetries(string command, int attempts = 3)
{
    for (int i = 1; ; i++)
    {
        try { return await Curl.ExecuteAsync(command); }
        catch (CurlException ex) when (ex.CurlErrorCode is 6 or 7 && i < attempts)
        {
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
        }
    }
}
```

**Prevention:** keep hostnames in configuration rather than scattered literals; use `--connect-timeout` so dead hosts fail quickly and `-m` to bound the whole transfer; add a health-check endpoint you can call before the real work.

## Timeout Errors

### Operation timed out

**Problem:** `CurlOperationTimeoutException: Operation timed out after 10 seconds`

**Cause:** The transfer (connection, request and the whole body) took longer than `-m`/`--max-time` allows, or the connection was not made within `--connect-timeout`. `CurlOperationTimeoutException` derives from `CurlTimeoutException`; catch the base type to cover every timeout path. By default there is no timeout (`Curl.DefaultMaxTimeSeconds` is 0), so a timeout only happens when you ask for one.

**Fix:**

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl --max-time 120 https://httpbin.org/delay/3");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Timed out after {ex.Timeout?.TotalSeconds} seconds");
    // Options: raise --max-time, retry, or check whether the server is overloaded
}
```

**The two timeout flags:** curl has `--connect-timeout` for the connection phase and `--max-time` for the whole transfer, and CurlDotNet enforces both (`--connect-timeout` on .NET 5 and later; best-effort on the netstandard2.0 / .NET Framework builds). Both accept decimals:

```csharp
// At most 5 s to connect, 60 s for the whole transfer, body included
await Curl.ExecuteAsync("curl --connect-timeout 5 --max-time 60 https://httpbin.org/get");

// Without -m there is no limit at all; set one process-wide
Curl.DefaultMaxTimeSeconds = 15;
```

**Same thing with `CurlSettings` or the builder:**

```csharp
using CurlDotNet.Core;

var settings = new CurlSettings().WithTimeout(60);
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get", settings);

var result2 = await CurlRequestBuilder.Get("https://httpbin.org/get")
    .WithTimeout(TimeSpan.FromSeconds(60))
    .ExecuteAsync();
```

**Caller-side cancellation:** a `CancellationToken` also stops a request, but it throws the standard `OperationCanceledException`, not a timeout exception:

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
try
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/delay/60", cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Cancelled");
}
```

**Retrying after a timeout:** `--retry 3` retries timeouts automatically (each attempt with its own `-m`). To decide yourself:

```csharp
using CurlDotNet.Core;

async Task<CurlResult> RetryOnTimeout(string command, int attempts = 3)
{
    for (int i = 1; ; i++)
    {
        try { return await Curl.ExecuteAsync(command); }
        catch (CurlTimeoutException) when (i < attempts)
        {
            Console.WriteLine($"Timeout, retry {i}/{attempts - 1}");
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
        }
    }
}
```

**Prevention:** set `--max-time` from the API's documented latency plus headroom; set `Curl.DefaultMaxTimeSeconds` once at startup so no request can hang forever; pass a `CancellationToken` for user-initiated cancellation. See [Timeout Errors reference](../exceptions/timeout-errors.html).

## SSL Certificate Errors

### SSL certificate problem

**Problem:** A request to an HTTPS endpoint throws `CurlPeerFailedVerificationException` (exit code 60) even though the host is up; native curl reports "SSL certificate problem: unable to get local issuer certificate" for the same URL. Other handshake failures throw `CurlSslConnectErrorException` (35); both derive from `CurlSslException`.

**Cause:** Certificate verification failed - a self-signed certificate, an expired certificate, a hostname mismatch, or a corporate root CA that is not trusted by the machine.

**Fix for development only:**

```csharp
// WARNING: -k disables certificate verification. Never ship this.
var result = await Curl.ExecuteAsync("curl -k https://localhost:5001/api/health");
```

`Curl.DefaultInsecure = true` applies the same setting process-wide, which is convenient for local integration tests and dangerous anywhere else.

**Fix for production:** trust the certificate authority. On .NET 5 and later pass the CA bundle with `--cacert ca.pem` (or a folder with `--capath`), exactly as with curl; on .NET Standard 2.0 and .NET Framework install the CA in the operating system trust store. Client certificates (`--cert`, `--key`) and `--pinnedpubkey` are applied on .NET 5 and later too; see [SSL errors](../exceptions/ssl-errors.html).

```bash
# macOS: trust a CA certificate
sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain ca.crt

# Debian / Ubuntu
sudo cp ca.crt /usr/local/share/ca-certificates/
sudo update-ca-certificates
```

```powershell
# Windows: import into the machine's Trusted Root store
Import-Certificate -FilePath ca.crt -CertStoreLocation Cert:\LocalMachine\Root
```

**Diagnose the certificate:**

```bash
# Full handshake and certificate chain
openssl s_client -connect api.github.com:443 -servername api.github.com

# Expiry dates only
echo | openssl s_client -connect api.github.com:443 2>/dev/null | openssl x509 -noout -dates
```

**Prevention:** never use `-k` in production; monitor certificate expiry; keep OS certificate stores updated; use a public CA (for example Let's Encrypt) for internet-facing services. See [SSL Errors reference](../exceptions/ssl-errors.html).

## Authentication Errors

### 401 Unauthorized

**Problem:** `result.StatusCode` is 401. No exception is thrown.

**Cause:** Credentials are missing, wrong, expired, or sent in a form the server does not accept.

**Fix - basic authentication:**

```csharp
using CurlDotNet;

// -u sends an Authorization: Basic header
var result = await Curl.ExecuteAsync("curl -u username:password https://httpbin.org/basic-auth/username/password");
Console.WriteLine(result.StatusCode); // 200

// Builder equivalent
var result2 = await CurlDotNet.Core.CurlRequestBuilder.Get("https://httpbin.org/basic-auth/username/password")
    .WithBasicAuth("username", "password")
    .ExecuteAsync();
```

**Fix - bearer token and API key:**

```csharp
string token = Environment.GetEnvironmentVariable("API_TOKEN");
var result = await Curl.ExecuteAsync($"curl -H 'Authorization: Bearer {token}' https://httpbin.org/bearer");

string apiKey = Environment.GetEnvironmentVariable("API_KEY");
var result2 = await Curl.ExecuteAsync($"curl -H 'X-API-Key: {apiKey}' https://httpbin.org/headers");
```

**Fix - OAuth 2.0 client credentials:** two requests, the first to get a token, the second to use it.

```csharp
string clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
string clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
var tokenResponse = await Curl.ExecuteAsync($@"curl -X POST https://oauth.example.com/token \
    -d 'grant_type=client_credentials' \
    -d 'client_id={clientId}' \
    -d 'client_secret={clientSecret}'");
tokenResponse.EnsureSuccess();

dynamic tokenData = tokenResponse.AsJsonDynamic();
string accessToken = tokenData.access_token;

var result = await Curl.ExecuteAsync($"curl -H 'Authorization: Bearer {accessToken}' https://api.example.com/protected");
```

**Diagnosing a 401:**

```csharp
string token = Environment.GetEnvironmentVariable("API_TOKEN");
var result = await Curl.ExecuteAsync($"curl -H 'Authorization: Bearer {token}' https://httpbin.org/bearer");
if (result.StatusCode == 401)
{
    // The server says which scheme it wants, e.g. Bearer realm="api"
    Console.WriteLine(result.GetHeader("WWW-Authenticate"));
    Console.WriteLine(result.Body);
}
```

Also check: the token has not expired; the header name is exactly what the API documents (`Authorization` vs `X-API-Key`); the token has no stray quotes or whitespace from the environment variable.

**Prevention:** keep credentials in environment variables or a secrets manager, never in source; refresh tokens before they expire; treat 401 as a normal branch, not an exception. See [Authentication Basics](../tutorials/09-authentication-basics.html).

### 403 Forbidden

**Problem:** `result.StatusCode` is 403. The server understood the request and refuses it.

**Cause:** The authenticated identity lacks permission, the token is missing a scope, the IP is blocked, or the resource requires a different kind of authentication.

**Fix:**

```csharp
string token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
var result = await Curl.ExecuteAsync($"curl -H 'Authorization: Bearer {token}' https://api.github.com/user/repos");
if (result.StatusCode == 403)
{
    Console.WriteLine("Forbidden - check scopes, IP allow-lists and the account's role");
    Console.WriteLine(result.Body);               // most APIs explain the reason here
    Console.WriteLine(result.GetHeader("X-RateLimit-Remaining")); // some APIs use 403 for rate limits
}
```

**Prevention:** request only the scopes you need and check the API's permission documentation; log the response body for 403s because it usually names the missing permission.

## HTTP Errors

### 404 Not Found

**Problem:** `result.StatusCode` is 404; `result.IsSuccess` is `false`.

**Cause:** Wrong path (singular vs plural, missing version segment), deleted resource, or wrong base URL.

**Fix:**

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://api.github.com/users/no-such-user-xyz-123");
if (result.StatusCode == 404)
{
    Console.WriteLine("Resource not found");
}
else
{
    result.EnsureSuccess(); // throws CurlDotNet.Core.CurlHttpException for any other non-2xx
    Console.WriteLine(result.Body);
}
```

**Troubleshooting:** compare the path with the API documentation character by character; list the parent collection first (`/users` before `/users/123`); try the URL in a browser or with native curl.

**Prevention:** build URLs from constants and validated IDs; handle 404 as an expected outcome in lookups.

### 429 Too Many Requests

**Problem:** `result.StatusCode` is 429.

**Cause:** You exceeded the API's rate limit.

**Fix - let curl retry:** `--retry 3` retries 429 (and 408, 500, 502, 503, 504) and honours the `Retry-After` header:

```csharp
var result = await Curl.ExecuteAsync("curl --retry 3 https://httpbin.org/status/429");
```

**Fix - honour `Retry-After` yourself:**

```csharp
using CurlDotNet.Core;

async Task<CurlResult> RequestWithBackoff(string command)
{
    var result = await Curl.ExecuteAsync(command);
    if (result.StatusCode != 429) return result;

    var retryAfter = result.GetHeader("Retry-After");
    int seconds = int.TryParse(retryAfter, out var s) ? s : 60;
    Console.WriteLine($"Rate limited. Waiting {seconds} s");
    await Task.Delay(TimeSpan.FromSeconds(seconds));
    return await Curl.ExecuteAsync(command);
}
```

**Fix - throttle concurrency with `SemaphoreSlim`:**

```csharp
using CurlDotNet.Core;

var gate = new SemaphoreSlim(4); // at most 4 requests in flight

async Task<CurlResult> ThrottledRequest(string command)
{
    await gate.WaitAsync();
    try { return await Curl.ExecuteAsync(command); }
    finally { gate.Release(); }
}

var urls = new[] { "https://httpbin.org/get", "https://httpbin.org/ip", "https://httpbin.org/uuid" };
var tasks = urls.Select(u => ThrottledRequest($"curl {u}"));
var results = await Task.WhenAll(tasks);
```

**Prevention:** read the API's rate-limit headers (`X-RateLimit-Remaining` and similar) and slow down before you hit zero; cache responses; use batch endpoints when the API offers them.

### 500 Internal Server Error

**Problem:** `result.StatusCode` is 500 (or another 5xx).

**Cause:** A bug or outage on the server side; often temporary.

**Fix - retry 5xx with `--retry`:** `curl --retry 3 URL` retries 500, 502, 503 and 504 with a doubling back-off; see [curl --retry in C#](../cookbook/retry.html).

**Fix - retry 5xx with your own exponential backoff:**

```csharp
using CurlDotNet.Core;

async Task<CurlResult> ExecuteWithRetries(string command, int attempts = 3)
{
    for (int i = 1; ; i++)
    {
        var result = await Curl.ExecuteAsync(command);
        if (result.StatusCode < 500 || i == attempts) return result;

        Console.WriteLine($"Server error {result.StatusCode}, retry {i}/{attempts - 1}");
        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
    }
}
```

**What to do:** retry a few times; check the provider's status page; log `result.Body` (it may contain a request ID the provider can look up); fall back to cached data where the product allows.

**Prevention:** retry only idempotent requests (GET, PUT, DELETE) automatically; add a circuit breaker so a failing dependency does not take your service down with it.

## JSON Parsing Errors

### Invalid JSON

**Problem:** `ParseJson<T>()` throws `InvalidOperationException`, or returns an object with some properties null.

**Cause:** The response is not JSON (an HTML error page, an empty body, plain text), or the property names do not match the type. Matching is case-insensitive (`userName` binds to `UserName`), but `snake_case` names such as `user_name` still need `[JsonPropertyName("user_name")]`.

**Fix - check the content type and body first:**

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/json");
var contentType = result.GetHeader("Content-Type") ?? "";

if (!result.IsSuccess)
    Console.WriteLine($"HTTP {result.StatusCode}: {result.Body}");
else if (!contentType.Contains("json"))
    Console.WriteLine($"Not JSON ({contentType}): {result.Body}");
else if (string.IsNullOrWhiteSpace(result.Body))
    Console.WriteLine("Empty body");
else
    Console.WriteLine(result.ParseJson<Slideshow>().slideshow.title);
```

`Slideshow` here is your own class matching the response shape; the same applies to `MyType` below.

**Fix - catch the parser exception:** an empty body or malformed JSON throws `InvalidOperationException`; for malformed JSON the underlying parser exception (`System.Text.Json.JsonException`, or Newtonsoft's on the `netstandard2.0` build) is its `InnerException`.

```csharp
try
{
    var data = result.ParseJson<MyType>();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Invalid JSON: {ex.Message}");
    if (result.Body.TrimStart().StartsWith("<"))
        Console.WriteLine("The server returned HTML, probably an error page");
}
```

**Fix - explore the shape with `AsJsonDynamic()`:** when you are not sure what the server returns, inspect it before writing a class.

```csharp
dynamic json = result.AsJsonDynamic();
Console.WriteLine(json.slideshow.title);
```

**Prevention:** check `IsSuccess` before parsing (error pages are rarely JSON); send `-H 'Accept: application/json'` so the server knows what you want; add `[JsonPropertyName]` attributes for `snake_case` or otherwise differently spelled names.

## Compilation Errors

### Cannot implicitly convert type 'Task<CurlResult>' to 'CurlResult'

**Cause:** Missing `await`, or the method is not `async`.

```csharp
// Wrong: missing await
var result = Curl.ExecuteAsync("curl https://httpbin.org/get");

// Right
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

// The enclosing method must be async
async Task MyMethod()
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
}
```

If you cannot use `async`, the synchronous methods exist: `Curl.Execute(command)`, `Curl.Get(url)`, `Curl.PostJson(url, body)`. They block the calling thread for the whole transfer, so do not call them on a UI thread or in classic ASP.NET request code.

### The name 'Curl' does not exist in the current context

**Cause:** Missing `using CurlDotNet;`.

```csharp
using CurlDotNet;              // Curl, static entry point
using CurlDotNet.Core;         // CurlResult, CurlSettings, CurlRequestBuilder
using CurlDotNet.Exceptions;   // CurlException and subclasses

var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
```

### 'CurlHttpException' is an ambiguous reference (CS0104)

**Cause:** A type named `CurlHttpException` exists in both `CurlDotNet.Core` and `CurlDotNet.Exceptions`, and the file imports both namespaces.

**Fix:** qualify the one you mean. `result.EnsureSuccess()` throws the `CurlDotNet.Core` type:

```csharp
catch (CurlDotNet.Core.CurlHttpException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.ResponseBody}");
}
```

### 'CurlResult' could not be found

**Cause:** `CurlResult` lives in `CurlDotNet.Core`. Add `using CurlDotNet.Core;` whenever you name the type explicitly (for example as a method return type).

## Runtime Errors

### NullReferenceException

**Problem:** `NullReferenceException` when reading a header or a parsed property.

**Cause:** `GetHeader(name)` returns `null` when the header is absent; `ParseJson<T>()` leaves properties `null` when names do not match; `Body` is empty for `-o`/`-O` downloads (the body went to the file; read `result.OutputFiles`) and for `HEAD` requests made without `-I` (with `-I` the headers are the body).

**Fix:**

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");

if (!result.IsSuccess)
{
    Console.WriteLine($"Request failed: {result.StatusCode}");
    return;
}

if (result.HasHeader("X-Custom-Header"))
    Console.WriteLine(result.GetHeader("X-Custom-Header"));

var contentType = result.GetHeader("Content-Type") ?? "unknown";
```

`result.Headers` is a case-insensitive dictionary, so `result.Headers.TryGetValue("content-type", out var ct)` also works.

**Prevention:** check `IsSuccess` first; use `?? ""` or `HasHeader` for optional headers; assert on parsed objects in tests so renamed JSON properties are caught early.

### Request runs forever

**Cause:** There is no default timeout (`Curl.DefaultMaxTimeSeconds` is 0).

**Fix:** set `Curl.DefaultMaxTimeSeconds = 30;` at startup, or pass `--max-time` on each command, or pass a `CancellationToken`. See [Timeout Errors](#timeout-errors).

## Error Handling Practices

### Catch specific exceptions, then the base

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -m 15 https://api.github.com");
    result.EnsureSuccess();
}
catch (CurlCouldntConnectException ex) { Console.WriteLine($"Unreachable: {ex.Host}:{ex.Port}"); }
catch (CurlTimeoutException ex) { Console.WriteLine($"Timeout: {ex.Timeout}"); }
catch (CurlTooManyRedirectsException ex) { Console.WriteLine($"Redirect loop after {ex.RedirectCount}"); }
catch (CurlDotNet.Core.CurlHttpException ex) { Console.WriteLine($"HTTP {ex.StatusCode}"); }
catch (CurlException ex) { Console.WriteLine($"curl error {ex.CurlErrorCode}: {ex.Message}"); }
catch (OperationCanceledException) { Console.WriteLine("Cancelled"); }
```

The [Error Reference](error-reference.html#exit-code-table) lists every exception type with its exit code.

### Check the status before using the body

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/json");

if (!result.IsSuccess)
{
    Console.WriteLine($"Request failed with status {result.StatusCode}");
    Console.WriteLine(result.Body);
    return;
}

var data = result.ParseJson<Slideshow>();
```

### Retry only what is safe to retry

Retry timeouts, connection failures and 5xx responses on idempotent requests (GET, PUT, DELETE). Do not blindly retry POSTs that create records. `--retry N` does this for you with curl's rules; the `ExecuteWithRetries` and `RetryOnTimeout` helpers above show the hand-written pattern. Combine either with `--max-time` so each attempt is bounded.

### Log the whole exception

```csharp
catch (CurlException ex)
{
    logger.LogError(ex, "curl request failed. Command: {Command}, Code: {Code}, Diagnostics: {Diag}",
        ex.Command, ex.CurlErrorCode, ex.DiagnosticInfo);
    throw;
}
```

See [Logging and Observability](../guides/logging-observability.html) for structured logging of requests and timings (`result.Timings`).

## Still Need Help?

1. Check [known issues](https://github.com/jacob-mellor/curl-dot-net/issues).
2. Ask in [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions).
3. [Open a new issue](https://github.com/jacob-mellor/curl-dot-net/issues/new) with: CurlDotNet version, .NET version, operating system, the smallest code that reproduces the problem, the full error message, and whether native curl behaves differently.

Back to the [Troubleshooting hub](README.html), the [Error Reference](error-reference.html) or the [FAQ](faq.html).
