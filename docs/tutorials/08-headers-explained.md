---
title: "curl -H HTTP Headers in C# Explained"
description: "Set curl -H HTTP headers in C# and read response headers with CurlDotNet: Authorization, Content-Type, Accept, User-Agent (-A), ETags and rate limits."
---

# Tutorial 8: curl -H HTTP Headers in C#

Setting curl `-H` HTTP headers in C# works exactly like the command line: pass `-H 'Name: value'` inside the string you give to `Curl.ExecuteAsync`, or call `WithHeader` on `CurlRequestBuilder`. This tutorial explains what headers are, how to send them, and how to read the headers a server sends back.

## What Are Headers?

Headers are key-value metadata sent with every HTTP request and response:

```http
GET /users/octocat HTTP/1.1
Host: api.github.com
User-Agent: MyApp/1.0
Accept: application/json
Authorization: Bearer token123
```

## Setting Request Headers

### With -H in a curl String

Each `-H` adds one header. Quote the value because it contains a colon and a space.

```csharp
using CurlDotNet;
using CurlDotNet.Core;

var result = await Curl.ExecuteAsync(@"curl https://api.github.com/users/octocat \
    -H 'Accept: application/vnd.github+json' \
    -H 'X-GitHub-Api-Version: 2022-11-28' \
    -H 'User-Agent: MyApp/1.0'");

Console.WriteLine(result.StatusCode);
```

### With the Builder

`WithHeader` adds a single header; `WithHeaders` takes a dictionary.

```csharp
var headers = new Dictionary<string, string>
{
    ["Accept"] = "application/vnd.github+json",
    ["X-Request-ID"] = Guid.NewGuid().ToString()
};

var result = await CurlRequestBuilder.Get("https://api.github.com/users/octocat")
    .WithHeaders(headers)
    .WithHeader("X-GitHub-Api-Version", "2022-11-28")
    .ExecuteAsync();
```

### Default Headers for Many Requests

`CurlSettings.WithHeader` attaches headers to a command without editing the string, which is useful when the command comes from configuration or a user.

```csharp
var settings = new CurlSettings()
    .WithHeader("Accept", "application/json")
    .WithHeader("User-Agent", "MyApp/1.0");

var a = await Curl.ExecuteAsync("curl https://httpbin.org/get", settings);
var b = await Curl.ExecuteAsync("curl https://httpbin.org/headers", settings);
```

Use `WithHeader("User-Agent", ...)` here: the engine applies `CurlSettings` headers, timeouts, redirects, `-k`, retries, progress and cancellation, but `CurlSettings.WithUserAgent` is not applied to the request. To change the User-Agent for every request in the process, set `Curl.DefaultUserAgent`.

### Headers CurlDotNet Sends by Default

Like curl, every request carries `User-Agent: curl/8.7.1` and `Accept: */*` unless you say otherwise, so APIs that reject anonymous clients (GitHub's, for example) work without `-A`. The value comes from `Curl.DefaultUserAgent` (built from the constant `Curl.CurlCompatVersion`, `"8.7.1"`).

| You write | What is sent |
|-----------|--------------|
| nothing | `User-Agent: curl/8.7.1` and `Accept: */*` |
| `-A 'MyApp/1.0'` | `User-Agent: MyApp/1.0` |
| `-A ''` | no `User-Agent` header |
| `-H 'User-Agent:'` / `-H 'Accept:'` | that default header is removed |
| `-H 'X-Empty;'` | `X-Empty:` with an empty value |
| `-H @headers.txt` | one header per line, read from the file |
| `-H 'X-A: 1' -H 'X-A: 2'` | `X-A: 1, 2` (duplicate names are joined with ", ") |

```csharp
// Change the default for the whole process
Curl.DefaultUserAgent = "MyApp/2.0 (+https://example.com)";
```

## Common Request Headers

### Authorization

Authentication is covered fully in [Tutorial 9](09-authentication-basics.html); the header forms are shown here for completeness.

```csharp
// Bearer token (OAuth 2.0, JWT)
await Curl.ExecuteAsync("curl -H 'Authorization: Bearer eyJhbGciOi...' https://httpbin.org/bearer");

// Basic auth: let curl build the header
await Curl.ExecuteAsync("curl -u alice:secret https://httpbin.org/basic-auth/alice/secret");

// API key in a custom header
await Curl.ExecuteAsync("curl -H 'X-API-Key: your-api-key' https://httpbin.org/headers");
```

### Content-Type

`Content-Type` describes the body you are sending. curl sets `application/x-www-form-urlencoded` automatically for `-d`, so you only need `-H` for other formats.

```csharp
// JSON body
await Curl.ExecuteAsync(@"curl -X POST https://httpbin.org/post \
    -H 'Content-Type: application/json' -d '{""a"":1}'");

// XML body
await Curl.ExecuteAsync(@"curl -X POST https://httpbin.org/post \
    -H 'Content-Type: application/xml' -d '<a>1</a>'");

// Form body: no header needed, curl adds it
await Curl.ExecuteAsync("curl -d 'name=Jane&age=25' https://httpbin.org/post");
```

### Accept

`Accept` tells the server which response formats you can handle.

```csharp
await Curl.ExecuteAsync("curl -H 'Accept: application/json' https://httpbin.org/get");

await Curl.ExecuteAsync("curl -H 'Accept: application/json, text/plain, */*' https://httpbin.org/get");

// JSON:API media type
await Curl.ExecuteAsync("curl -H 'Accept: application/vnd.api+json' https://httpbin.org/get");
```

### User-Agent and Referer

curl has short flags for these two: `-A` (or `--user-agent`) and `-e` (or `--referer`).

```csharp
await Curl.ExecuteAsync("curl -A 'MyApp/1.0 (Windows)' https://httpbin.org/user-agent");

await Curl.ExecuteAsync("curl -e 'https://example.com/page' https://httpbin.org/headers");

// Builder equivalents
await CurlRequestBuilder.Get("https://httpbin.org/headers")
    .WithUserAgent("MyApp/1.0")
    .WithReferer("https://example.com/page")
    .ExecuteAsync();
```

### Cache-Control

```csharp
// Ask intermediaries not to serve a cached copy
await Curl.ExecuteAsync("curl -H 'Cache-Control: no-cache' https://httpbin.org/cache");

// Accept a copy up to an hour old
await Curl.ExecuteAsync("curl -H 'Cache-Control: max-age=3600' https://httpbin.org/cache");
```

## Reading Response Headers

### Access Response Headers

`result.Headers` is a case-insensitive dictionary. Prefer `GetHeader` (returns `null` when missing) and `HasHeader` over indexing.

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

Console.WriteLine($"Content type: {result.GetHeader("Content-Type")}");

if (result.HasHeader("X-RateLimit-Remaining"))
    Console.WriteLine($"API calls remaining: {result.GetHeader("X-RateLimit-Remaining")}");

foreach (var header in result.Headers)
    Console.WriteLine($"{header.Key}: {header.Value}");
```

### Seeing Headers the curl Way

`-i` includes the response headers at the top of `Body`, and `-I` sends a HEAD request so you get headers only. Use `result.Headers` when you want to read them in code.

```csharp
// Headers plus body in Body, like curl -i on the terminal
var withHeaders = await Curl.ExecuteAsync("curl -i https://httpbin.org/get");
Console.WriteLine(withHeaders.Body);

// HEAD request: headers only, no body
var headOnly = await Curl.ExecuteAsync("curl -I https://httpbin.org/get");
Console.WriteLine(headOnly.GetHeader("Content-Length"));
```

### Common Response Headers

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");

var contentType  = result.GetHeader("Content-Type");   // application/json; charset=utf-8
var length       = result.GetHeader("Content-Length"); // 1234
var etag         = result.GetHeader("ETag");           // W/"123456789"
var lastModified = result.GetHeader("Last-Modified");  // Wed, 21 Oct 2026 07:28:00 GMT
var location     = result.GetHeader("Location");       // on redirects or 201 Created
var setCookie    = result.GetHeader("Set-Cookie");     // session=abc123; HttpOnly
```

## Sending Cookies

A cookie is just another header. curl's `-b` flag accepts a cookie string and builds the `Cookie` header; the builder has `WithCookie`.

```csharp
await Curl.ExecuteAsync("curl -b 'session=abc123' https://httpbin.org/cookies");

await Curl.ExecuteAsync("curl -b 'session=abc123; theme=dark' https://httpbin.org/cookies");

await CurlRequestBuilder.Get("https://httpbin.org/cookies")
    .WithCookie("session=abc123; theme=dark")
    .ExecuteAsync();
```

To use a cookie the server set, read `Set-Cookie` from the first response and pass the `name=value` part with `-b` on the next request. See the [cookies recipe](../cookbook/cookies.html) for more.

## Custom Headers

### API-Specific Headers

```csharp
// Version pinning used by several public APIs
await Curl.ExecuteAsync("curl -H 'X-GitHub-Api-Version: 2022-11-28' https://api.github.com/zen");
await Curl.ExecuteAsync("curl -H 'Stripe-Version: 2023-10-16' https://api.stripe.com/v1/charges");

// Correlation ID for tracing a request through your logs
var correlationId = Guid.NewGuid().ToString();
await Curl.ExecuteAsync($"curl -H 'X-Correlation-ID: {correlationId}' https://httpbin.org/headers");
```

### Request Tracking Headers

Wrap the call in a small helper when every request needs the same generated headers.

```csharp
static Task<CurlResult> TrackedGet(string url) =>
    CurlRequestBuilder.Get(url)
        .WithHeader("X-Request-ID", Guid.NewGuid().ToString())
        .WithHeader("X-Timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        .ExecuteAsync();

var result = await TrackedGet("https://httpbin.org/headers");
```

## Conditional Requests

### If-None-Match (ETag)

Send the `ETag` from a previous response back in `If-None-Match`; a `304 Not Modified` means your cached copy is still current.

```csharp
var first = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");
string etag = first.GetHeader("ETag");

var second = await Curl.ExecuteAsync(
    $"curl -H 'If-None-Match: {etag}' https://api.github.com/users/octocat");

if (second.StatusCode == 304)
    Console.WriteLine("Resource has not changed, use the cached version");
```

### If-Modified-Since

```csharp
var since = DateTime.UtcNow.AddHours(-1).ToString("R");   // RFC 1123 format

var result = await Curl.ExecuteAsync(
    $"curl -H 'If-Modified-Since: {since}' https://api.github.com/users/octocat");

if (result.StatusCode == 304)
    Console.WriteLine("No changes since last check");
```

## Rate Limiting Headers

Most APIs report quota in headers. Read them to back off before you hit a 429.

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com/rate_limit");

int.TryParse(result.GetHeader("X-RateLimit-Limit"), out int limit);
int.TryParse(result.GetHeader("X-RateLimit-Remaining"), out int remaining);
long.TryParse(result.GetHeader("X-RateLimit-Reset"), out long resetUnix);

var resetAt = DateTimeOffset.FromUnixTimeSeconds(resetUnix);
Console.WriteLine($"Calls remaining: {remaining}/{limit}, resets at {resetAt:u}");

if (remaining == 0)
    Console.WriteLine($"Rate limited. Wait {(resetAt - DateTimeOffset.UtcNow).TotalSeconds:F0} s");
```

## CORS Headers

CORS is enforced by browsers, not by CurlDotNet, but you can inspect what a server allows by sending an `Origin` header.

```csharp
var result = await Curl.ExecuteAsync(
    "curl -H 'Origin: https://myapp.example' https://httpbin.org/get");

Console.WriteLine(result.GetHeader("Access-Control-Allow-Origin") ?? "(no CORS header)");
```

## Header Management Patterns

### A Reusable Client with LibCurl

`LibCurl` is an `IDisposable` instance that keeps headers between calls, which suits a small API client class.

```csharp
using CurlDotNet.Lib;

using var client = new LibCurl()
    .WithHeader("Accept", "application/vnd.github+json")
    .WithHeader("X-GitHub-Api-Version", "2022-11-28")
    .WithUserAgent("MyApp/1.0");

var user = await client.GetAsync("https://api.github.com/users/octocat");
var repos = await client.GetAsync("https://api.github.com/users/octocat/repos");
```

### Checking What Was Sent

httpbin's `/headers` endpoint echoes the request headers, which is the quickest way to confirm your `-H` flags were parsed as you expected.

```csharp
var result = await Curl.ExecuteAsync(@"curl https://httpbin.org/headers \
    -H 'X-Custom: hello' -A 'MyApp/1.0'");

Console.WriteLine(result.Body);   // shows X-Custom and User-Agent as received
```

## Best Practices

1. Set a descriptive `User-Agent` (`-A`) for your own app; without one CurlDotNet identifies itself as `curl/8.7.1`, as curl does
2. Match `Content-Type` to the body you send
3. Read rate-limit headers and back off before a 429
4. Use `ETag` / `If-None-Match` to avoid re-downloading unchanged data
5. Add a correlation ID header so requests can be traced through logs
6. Never log `Authorization` or `Cookie` header values
7. Check `Content-Type` before calling `ParseJson<T>()`
8. Use `GetHeader` rather than indexing `Headers`, so a missing header returns `null`

## Summary

Headers control how HTTP requests and responses behave:

- Send request headers with `-H 'Name: value'`, `-A`, `-e`, `-b`, or the builder's `WithHeader`
- Read response headers with `result.GetHeader(name)` and `result.HasHeader(name)`
- Use `-i` to see headers in `Body` and `-I` for a headers-only HEAD request
- Respect caching (`ETag`) and rate-limit headers

## What's Next?

Learn about [authentication basics](09-authentication-basics.html) and how to access protected APIs securely.

See also: [HEAD requests](../cookbook/head-request.html) and the [curl CLI compatibility reference](../reference/curl-cli-compatibility.html).

---

[Previous: JSON for Beginners](07-json-for-beginners.html) | [Next: Authentication Basics](09-authentication-basics.html)
