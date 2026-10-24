---
title: "How to Use curl Basic Auth (-u) in C#"
description: "curl basic auth in C#: run curl -u user:password with CurlDotNet or call WithBasicAuth. Digest and NTLM via --digest and --ntlm are covered too."
---

# How to use curl basic auth in C#

To use curl basic auth in C#, pass `-u username:password` in the command you give `Curl.ExecuteAsync`, or call `.WithBasicAuth(username, password)` on `CurlRequestBuilder`. CurlDotNet base64-encodes the credentials into an `Authorization: Basic` header exactly as curl does.

```bash
dotnet add package CurlDotNet
```

## The curl command

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -u user:passwd https://httpbin.org/basic-auth/user/passwd");

Console.WriteLine(result.StatusCode);  // 200
```

`--basic` is accepted and is the default for `-u`.

## API keys as the user name

Some APIs, Stripe for example, take the API key as the user name with an empty password. The trailing colon works the same as in curl:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -u sk_test_123: https://httpbin.org/headers");
Console.WriteLine(result.Body);  // "Authorization": "Basic c2tfdGVzdF8xMjM6"
```

## The fluent builder

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://httpbin.org/basic-auth/user/passwd")
    .WithBasicAuth("user", "passwd")
    .ExecuteAsync();

Console.WriteLine(result.IsSuccess);
```

Or, with `LibCurl`, set it once for all requests:

```csharp
using CurlDotNet.Lib;

using var curl = new LibCurl();
curl.WithBasicAuth("user", "passwd");
var result = await curl.GetAsync("https://httpbin.org/basic-auth/user/passwd");
Console.WriteLine(result.StatusCode);
```

## Digest, NTLM and Negotiate

Challenge-response schemes use the same `-u` value plus a scheme flag:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl --digest -u user:passwd https://httpbin.org/digest-auth/auth/user/passwd");

Console.WriteLine(result.StatusCode);  // 200
```

`--ntlm`, `--negotiate` and `--anyauth` are also accepted. These schemes use .NET's built-in credential handling, so what works depends on the platform's NTLM and Kerberos support.

## When the credentials are wrong

A 401 does not throw. Check `result.StatusCode` or call `result.EnsureSuccess()`, which throws `CurlDotNet.Core.CurlHttpException` for any non-2xx status.

## Security notes

- Prefer HTTPS: Basic credentials are only base64-encoded, not encrypted.
- Read secrets from configuration or the environment. The parser expands `$NAME`, so `-u "$API_USER:$API_PASS"` works when both variables are set.
- `CurlResult.Command` contains the credentials; do not log it.

## Related

- [Bearer token authentication](../curl-bearer-token-csharp/README.md)
- [Error handling](../curl-error-handling-csharp/README.md)
- [Proxy with credentials](../curl-proxy-csharp/README.md)
- [All how-to guides](../README.md)
