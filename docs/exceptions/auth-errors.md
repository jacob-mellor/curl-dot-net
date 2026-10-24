---
title: "curl 401 and 403 Authentication Errors in C# (CurlDotNet)"
description: "Handle curl authentication errors in C#: how CurlDotNet reports 401/403, how to send -u, --digest and bearer tokens, and how to retry with new credentials."
---

# curl Authentication Errors in C# (401 and 403)

curl authentication errors in C# show up as HTTP 401 Unauthorized or 403 Forbidden responses when credentials are missing, wrong, or sent with the wrong scheme. CurlDotNet does not throw for these: the request completes and you get a `CurlResult` with `IsSuccess == false` and the `StatusCode`, so you decide whether to retry, refresh a token, or fail.

## How Authentication Failures Surface

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://httpbin.org/basic-auth/user/passwd");

if (result.StatusCode == 401)
    Console.WriteLine($"Login required: {result.GetHeader("WWW-Authenticate")}");
else if (result.StatusCode == 403)
    Console.WriteLine("Authenticated, but not allowed to access this resource");
```

The `WWW-Authenticate` response header tells you which scheme the server expects (`Basic`, `Digest`, `Bearer`, ...).

If you prefer an exception, call `result.EnsureSuccess()`; it throws `CurlDotNet.Core.CurlHttpException` with `StatusCode`, `ResponseBody` and `ResponseHeaders`.

## Sending Credentials

### Basic Authentication (-u)

```csharp
var result = await Curl.ExecuteAsync("curl -u user:passwd https://httpbin.org/basic-auth/user/passwd");
Console.WriteLine(result.StatusCode); // 200
```

Builder equivalent:

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/basic-auth/user/passwd")
    .WithBasicAuth("user", "passwd")
    .ExecuteAsync();
```

### Digest, NTLM and Negotiate

Pick the scheme with the matching curl flag; `--anyauth` lets the server choose:

```csharp
var digest = await Curl.ExecuteAsync("curl --digest -u user:passwd https://httpbin.org/digest-auth/auth/user/passwd");
var any = await Curl.ExecuteAsync("curl --anyauth -u user:passwd https://httpbin.org/digest-auth/auth/user/passwd");
```

### Bearer Tokens

```csharp
var token = "eyJhbGc...";
var result = await Curl.ExecuteAsync($"curl -H 'Authorization: Bearer {token}' https://httpbin.org/bearer");
```

Builder: `CurlRequestBuilder.Get(url).WithBearerToken(token)`.

### API Keys in a Header

```csharp
var apiKey = Environment.GetEnvironmentVariable("MY_API_KEY");
var result = await Curl.ExecuteAsync($"curl -H 'X-API-Key: {apiKey}' https://httpbin.org/headers");
```

## Retrying After a 401

A common pattern when a token has expired:

```csharp
async Task<CurlResult> CallWithTokenRefresh(string url)
{
    var result = await Curl.ExecuteAsync($"curl -H 'Authorization: Bearer {_token}' {url}");
    if (result.StatusCode != 401)
        return result;

    _token = await RefreshTokenAsync();   // your own token endpoint call
    return await Curl.ExecuteAsync($"curl -H 'Authorization: Bearer {_token}' {url}");
}
```

(`CurlResult` is in `CurlDotNet.Core`.)

## Checklist for Common Issues

### Invalid Credentials

- Verify the username and password
- Wrap values containing spaces or special characters in single quotes inside the command string
- Check that the credentials have not expired or been revoked

### Wrong Authentication Scheme

- Read the `WWW-Authenticate` header on the 401 response
- Use `--digest`, `--ntlm` or `--negotiate` when the server does not accept Basic
- Some APIs want the key as a header (`-H 'X-API-Key: ...'`) rather than `-u`

### 403 After a Successful Login

- The account is authenticated but lacks permission for that resource
- Check scopes, roles, or IP allow-lists on the server side

## Seeing What Is Sent

Use `-v` to confirm the `Authorization` header is present in the outgoing request; the request headers are written at the start of `result.Body`:

```csharp
var result = await Curl.ExecuteAsync("curl -v -u user:passwd https://httpbin.org/basic-auth/user/passwd");
Console.WriteLine(result.Body);
```

## Related Documentation

- [HTTP errors](http-errors.html)
- [Tutorial 9: Authentication Basics](../tutorials/09-authentication-basics.html)
- [Cookbook](../cookbook/index.html)
- [Exception reference](index.html)
