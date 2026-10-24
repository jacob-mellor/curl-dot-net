---
title: "curl -u Basic Auth and Bearer Tokens in C#"
description: "Use curl -u basic auth and bearer tokens in C# with CurlDotNet: -u user:pass, --digest, -H Authorization: Bearer, WithBasicAuth and WithBearerToken."
---

# Tutorial 9: curl -u Basic Auth and Bearer Tokens in C#

Using curl `-u` basic auth and bearer tokens in C# is a one-line change to the command you pass to `Curl.ExecuteAsync`: add `-u user:pass` for HTTP Basic or `-H 'Authorization: Bearer TOKEN'` for token-based APIs. This tutorial covers the authentication schemes curl supports, their `CurlRequestBuilder` equivalents, and how to handle 401 and 403 responses.

## Types of Authentication

### 1. API Key Authentication

The simplest scheme: a key that identifies your application, sent as a header or query parameter. The API's documentation tells you which header name to use.

```csharp
using System.Text;
using System.Text.Json.Serialization;
using CurlDotNet;
using CurlDotNet.Core;

// API key in a header
var result = await Curl.ExecuteAsync("curl -H 'X-API-Key: your-api-key' https://httpbin.org/headers");

// API key as a query parameter (less secure: it ends up in server logs)
var result2 = await Curl.ExecuteAsync("curl 'https://httpbin.org/get?api_key=your-api-key'");

// Builder
var result3 = await CurlRequestBuilder.Get("https://httpbin.org/headers")
    .WithHeader("X-API-Key", "your-api-key")
    .ExecuteAsync();
```

### 2. Basic Authentication with -u

`-u user:password` makes curl build the `Authorization: Basic ...` header (base64 of `user:password`) for you. Only use it over HTTPS.

```csharp
// curl string
var result = await Curl.ExecuteAsync(
    "curl -u alice:secret https://httpbin.org/basic-auth/alice/secret");

Console.WriteLine(result.StatusCode);   // 200 when the credentials match

// Builder
var result2 = await CurlRequestBuilder.Get("https://httpbin.org/basic-auth/alice/secret")
    .WithBasicAuth("alice", "secret")
    .ExecuteAsync();
```

You can also write the header by hand. This is what `-u` does internally.

```csharp
var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes("alice:secret"));

var result = await Curl.ExecuteAsync(
    $"curl -H 'Authorization: Basic {credentials}' https://httpbin.org/basic-auth/alice/secret");
```

### 3. Bearer Tokens (OAuth 2.0 / JWT)

Modern APIs hand you an access token and expect it in the `Authorization` header with the `Bearer` prefix.

```csharp
string accessToken = Environment.GetEnvironmentVariable("API_TOKEN");

// curl string
var result = await Curl.ExecuteAsync(
    $"curl -H 'Authorization: Bearer {accessToken}' https://httpbin.org/bearer");

// Builder
var result2 = await CurlRequestBuilder.Get("https://httpbin.org/bearer")
    .WithBearerToken(accessToken)
    .ExecuteAsync();
```

GitHub, for example, accepts a personal access token this way:

```csharp
var result = await Curl.ExecuteAsync(
    $"curl -H 'Authorization: Bearer {accessToken}' https://api.github.com/user");

if (result.IsSuccess)
    Console.WriteLine(result.Body);
else if (result.StatusCode == 401)
    Console.WriteLine("Token is missing, expired or revoked");
```

## Choosing the Scheme with curl Flags

`-u` supplies the credentials; a second flag chooses how they are sent. The default is Basic.

| Flag | Scheme |
|------|--------|
| `--basic` | HTTP Basic (default for `-u`) |
| `--digest` | HTTP Digest: a challenge-response scheme that never sends the password itself |
| `--ntlm` | NTLM, common on Windows intranets |
| `--negotiate` | SPNEGO / Kerberos |
| `--anyauth` | Let the server pick the strongest scheme it offers |

```csharp
// Digest: curl makes an unauthenticated request, receives the challenge, then answers it
var digest = await Curl.ExecuteAsync(
    "curl --digest -u alice:secret https://httpbin.org/digest-auth/auth/alice/secret");

// Windows intranet service
var ntlm = await Curl.ExecuteAsync("curl --ntlm -u 'DOMAIN\\alice:secret' https://intranet.example/api");

// Let the server choose
var any = await Curl.ExecuteAsync("curl --anyauth -u alice:secret https://httpbin.org/basic-auth/alice/secret");
```

## Getting an OAuth 2.0 Access Token

Token endpoints are ordinary POST requests, so CurlDotNet handles them with the same `-d` flags you would use on the command line. This is the client-credentials flow.

```csharp
public class TokenResponse
{
    [JsonPropertyName("access_token")] public string AccessToken { get; set; }
    [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
}

var tokenResult = await Curl.ExecuteAsync(@"curl -X POST https://auth.example.com/oauth/token \
    -d grant_type=client_credentials \
    -d client_id=my-client \
    -d client_secret=my-secret \
    -d 'scope=read:data write:data'");

var token = tokenResult.EnsureSuccess().ParseJson<TokenResponse>();
```

Then use the token until it expires and request a new one.

```csharp
var expiresAt = DateTime.UtcNow.AddSeconds(token.ExpiresIn - 60);   // refresh a minute early

if (DateTime.UtcNow < expiresAt)
{
    var data = await CurlRequestBuilder.Get("https://api.example.com/data")
        .WithBearerToken(token.AccessToken)
        .ExecuteAsync();
}
```

## A Reusable Authenticated Client

`LibCurl` keeps the credentials between calls, so you set them once.

```csharp
using CurlDotNet.Lib;

using var client = new LibCurl()
    .WithBearerToken(accessToken)
    .WithHeader("Accept", "application/vnd.github+json");

var me = await client.GetAsync("https://api.github.com/user");
var repos = await client.GetAsync("https://api.github.com/user/repos");

// Or with Basic credentials
using var basic = new LibCurl().WithBasicAuth("alice", "secret");
```

## Handling 401 and 403

A `401 Unauthorized` means the server did not accept your credentials; a `403 Forbidden` means it knows who you are but you are not allowed. Neither throws: check `StatusCode`.

```csharp
var result = await Curl.ExecuteAsync(
    $"curl -H 'Authorization: Bearer {accessToken}' https://api.github.com/user");

switch (result.StatusCode)
{
    case 401: Console.WriteLine("Credentials rejected: get a new token"); break;
    case 403: Console.WriteLine("Authenticated, but this token lacks permission"); break;
    default:  Console.WriteLine(result.IsSuccess ? result.Body : $"HTTP {result.StatusCode}"); break;
}
```

### Retry Once with a Fresh Token

```csharp
static async Task<CurlResult> GetWithToken(string url, string token) =>
    await CurlRequestBuilder.Get(url).WithBearerToken(token).ExecuteAsync();

var result = await GetWithToken("https://api.example.com/data", currentToken);

if (result.StatusCode == 401)
{
    currentToken = await RequestNewToken();        // your token-endpoint call from above
    result = await GetWithToken("https://api.example.com/data", currentToken);
}
```

## Checking What Will Be Sent

The builder's `ToCurlCommand()` shows the exact command, which is useful for debugging. Be careful not to log it: it contains the credentials.

```csharp
var command = CurlRequestBuilder.Get("https://httpbin.org/basic-auth/alice/secret")
    .WithBasicAuth("alice", "secret")
    .ToCurlCommand();

Console.WriteLine(command);   // curl -u alice:secret https://httpbin.org/basic-auth/alice/secret
```

httpbin's `/headers` endpoint echoes the request headers, so you can confirm that the `Authorization` header arrived as expected.

## Secure Storage of Credentials

Never hard-code secrets in source. Read them from the environment or from .NET configuration (`appsettings.json`, user secrets, a key vault).

```csharp
// Bad: a secret in source control
var apiKey = "sk_live_abcd1234efgh5678";

// Good: environment variable
var apiKey2 = Environment.GetEnvironmentVariable("API_KEY");

// Good: Microsoft.Extensions.Configuration (appsettings.json, user secrets, key vault)
var apiKey3 = configuration["ApiKey"];

var result = await Curl.ExecuteAsync($"curl -H 'X-API-Key: {apiKey2}' https://httpbin.org/headers");
```

When a command string contains credentials, also make sure you do not write `result.Command` or `ex.Command` to your logs.

## Best Practices

1. Never hard-code credentials; use environment variables or configuration
2. Always use HTTPS; `-u` and bearer tokens are sent in clear text otherwise
3. Prefer `--digest` or a token over Basic where the API offers it
4. Request a new token before the old one expires, and retry once on 401
5. Treat 401 (bad credentials) and 403 (insufficient permission) differently
6. Keep `Authorization` headers and `-u` values out of logs
7. Use the least-privileged scope or token the task needs

## Summary

Authentication with CurlDotNet uses the same flags as curl:

- `-u user:pass` for Basic, plus `--digest`, `--ntlm`, `--negotiate` or `--anyauth` to pick the scheme
- `-H 'Authorization: Bearer TOKEN'` for OAuth 2.0 and JWT APIs
- `WithBasicAuth` and `WithBearerToken` on `CurlRequestBuilder` and `LibCurl`
- 401 and 403 come back as status codes on `CurlResult`, not exceptions

## What's Next?

Learn about [working with files and downloads](10-files-and-downloads.html) in the next tutorial.

See also: [authentication schemes recipe](../cookbook/auth-schemes.html) and [authentication errors](../exceptions/auth-errors.html).

---

[Previous: Headers Explained](08-headers-explained.html) | [Next: Files and Downloads](10-files-and-downloads.html)
