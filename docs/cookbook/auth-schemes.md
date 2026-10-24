---
title: "curl Digest, NTLM and Basic Auth in C# with CurlDotNet"
description: "Use curl digest, NTLM and basic auth in C# with curl -u, --digest, --ntlm and --negotiate in CurlDotNet, plus bearer tokens and the builder auth methods."
---

# curl digest, NTLM and basic auth in C#

To use curl digest, NTLM and basic auth in C#, pass `-u user:password` together with the scheme flag (`--basic`, `--digest`, `--ntlm`, `--negotiate` or `--anyauth`) in the command you give to `Curl.ExecuteAsync`. `-u` alone means basic auth, exactly as in curl.

## Basic auth (-u)

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -u user:passwd https://httpbin.org/basic-auth/user/passwd");

Console.WriteLine(result.StatusCode);   // 200
Console.WriteLine(result.Body);         // {"authenticated": true, "user": "user"}
```

The credentials are sent pre-emptively as an `Authorization: Basic ...` header.

## Digest auth (--digest)

```csharp
var result = await Curl.ExecuteAsync(
    "curl --digest -u user:passwd https://httpbin.org/digest-auth/auth/user/passwd");

Console.WriteLine(result.StatusCode);   // 200
```

Digest is challenge-based: the first request gets a 401 with a nonce and the reply is computed from it. CurlDotNet handles the round trip for you and returns the final response.

## NTLM and Negotiate (Windows auth)

```csharp
var ntlm = await Curl.ExecuteAsync(
    "curl --ntlm -u DOMAIN\\alice:secret https://intranet.example.com/api");

var nego = await Curl.ExecuteAsync(
    "curl --negotiate -u DOMAIN\\alice:secret https://intranet.example.com/api");
```

Both are challenge-based like digest; the credentials are only sent once the server asks. `--anyauth` lets the server pick the strongest scheme it offers.

## Bearer tokens

Token-based APIs do not use `-u`; send the header:

```csharp
var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

var result = await Curl.ExecuteAsync(
    $"curl -H 'Authorization: Bearer {token}' -A 'my-app' https://api.github.com/user");

Console.WriteLine(result.StatusCode);
```

## Builder style

```csharp
using CurlDotNet.Core;

var basic = await CurlRequestBuilder.Get("https://httpbin.org/basic-auth/user/passwd")
    .WithBasicAuth("user", "passwd")
    .ExecuteAsync();

var bearer = await CurlRequestBuilder.Get("https://api.github.com/user")
    .WithBearerToken(Environment.GetEnvironmentVariable("GITHUB_TOKEN"))
    .WithUserAgent("my-app")
    .ExecuteAsync();

Console.WriteLine($"{basic.StatusCode} {bearer.StatusCode}");
```

`WithAuth("Digest ...")` sets an arbitrary `Authorization` header value when you have already computed it. For a reusable client, `new LibCurl().WithBasicAuth(u, p)` and `.WithBearerToken(t)` apply to every request it makes.

## Handle 401 and 403

Authentication failures come back as results, not exceptions:

```csharp
var result = await Curl.ExecuteAsync("curl -u user:wrong https://httpbin.org/basic-auth/user/passwd");

if (result.StatusCode == 401)
    Console.WriteLine($"Rejected; server offers: {result.GetHeader("WWW-Authenticate")}");
else if (result.StatusCode == 403)
    Console.WriteLine("Authenticated but not allowed");
```

## AWS Signature V4

`--aws-sigv4` signs the request with the AWS algorithm; the credentials still travel through `-u`:

```csharp
var result = await Curl.ExecuteAsync(@"
    curl --aws-sigv4 'aws:amz:us-east-1:s3' -u ACCESS_KEY:SECRET_KEY \
         https://my-bucket.s3.amazonaws.com/object.txt");
```

## Gotchas

- Never hard-code secrets in the command string in source control; read them from environment variables or your secret store and interpolate them.
- A password containing `@`, `:` or spaces must be quoted: `-u 'user:p@ss word'`.
- Basic credentials are only safe over HTTPS; the header is just base64.
- Client certificates (`--cert`, `--key`) are not supported yet.

## Related

- [curl -u basic auth and bearer tokens tutorial](../tutorials/09-authentication-basics.html)
- [Authentication errors](../exceptions/auth-errors.html)
- [Call a REST API with curl in C#](beginner/call-api.html)
- [Cookbook index](index.html)
