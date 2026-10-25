---
title: "curl Digest, NTLM and Basic Auth in C# with CurlDotNet"
description: "Use curl digest, NTLM and basic auth in C# with curl -u, --digest, --ntlm, --negotiate and --anyauth in CurlDotNet, plus .netrc, bearer tokens, proxy auth and the builder auth methods."
---

# curl digest, NTLM and basic auth in C#

To use curl digest, NTLM and basic auth in C#, pass `-u user:password` together with the scheme flag (`--basic`, `--digest`, `--ntlm`, `--negotiate` or `--anyauth`) in the command you give to `Curl.ExecuteAsync`. `-u` alone means basic auth, exactly as in curl. Digest and NTLM (v1 and v2) are negotiated inside the library in pure C#, transpiled from curl, so the request sequence and headers match what curl sends on every platform.

## Basic auth (-u)

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync(
    "curl -u user:passwd https://httpbin.org/basic-auth/user/passwd");

Console.WriteLine(result.StatusCode);   // 200
Console.WriteLine(result.Body);         // {"authenticated": true, "user": "user"}
```

The credentials are sent pre-emptively as an `Authorization: Basic ...` header. `-u` splits at the first colon, so `-u 'alice:pa:ss'` is user `alice` with password `pa:ss`.

Credentials in the URL work the same way and are also sent as Basic:

```csharp
var result = await Curl.ExecuteAsync(
    "curl https://user:passwd@httpbin.org/basic-auth/user/passwd");
```

## Digest auth (--digest)

```csharp
var result = await Curl.ExecuteAsync(
    "curl --digest -u user:passwd https://httpbin.org/digest-auth/auth/user/passwd");

Console.WriteLine(result.StatusCode);   // 200
```

Digest is challenge-based: the first request goes out without credentials, the server answers 401 with a nonce, and the second request carries the computed `Authorization: Digest ...` header. CurlDotNet handles the round trip for you and returns the final response. As in curl:

- No preemptive Basic header is sent when you pick `--digest`, `--ntlm`, `--negotiate` or `--anyauth`, so the password never travels in clear text.
- A POST or PUT is first sent as a probe with `Content-Length: 0`; the body goes out once the handshake has finished, so a large upload is not sent twice.

## NTLM and Negotiate (Windows auth)

```csharp
var ntlm = await Curl.ExecuteAsync(
    "curl --ntlm -u DOMAIN\\alice:secret https://intranet.example.com/api");

var nego = await Curl.ExecuteAsync(
    "curl --negotiate -u DOMAIN\\alice:secret https://intranet.example.com/api");
```

Both are challenge-based like digest; the credentials are only sent once the server asks. NTLM (v1 and v2) is computed in-library and is byte-identical to curl's messages, so it works the same on Windows, Linux and macOS. `--negotiate` (Kerberos/SPNEGO) is handed to the platform's `HttpClient` handler, so it needs a Kerberos setup on the machine.

`--anyauth` first asks without credentials and then picks the strongest scheme the server offers, preferring Digest, then NTLM, then Basic:

```csharp
var any = await Curl.ExecuteAsync(
    "curl --anyauth -u user:passwd https://httpbin.org/digest-auth/auth/user/passwd");
```

## Bearer tokens

Token-based APIs do not use `-u`. Use curl's `--oauth2-bearer`, or send the header yourself:

```csharp
var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

var viaOption = await Curl.ExecuteAsync(
    $"curl --oauth2-bearer {token} https://api.github.com/user");

var viaHeader = await Curl.ExecuteAsync(
    $"curl -H 'Authorization: Bearer {token}' https://api.github.com/user");

Console.WriteLine($"{viaOption.StatusCode} {viaHeader.StatusCode}");
```

`--oauth2-bearer` wins over `-u` and URL credentials, and an explicit `-H 'Authorization: ...'` wins over everything. No `-A` is needed for GitHub: CurlDotNet sends `User-Agent: curl/8.7.1` by default.

## Credentials from .netrc (-n)

`-n` / `--netrc` reads the login for the host from `~/.netrc` (`_netrc` on Windows when there is no `.netrc`), or from the file in the `NETRC` environment variable. `--netrc-file FILE` names the file, and `--netrc-optional` uses it only to fill in what the URL does not already give.

```csharp
// creds.netrc contains: machine httpbin.org login user password passwd
var result = await Curl.ExecuteAsync(
    "curl --netrc-file creds.netrc https://httpbin.org/basic-auth/user/passwd");

Console.WriteLine(result.StatusCode);   // 200
```

The parser follows curl's `lib/netrc.c`: `machine`, `default`, `login`, `password`, `macdef` and quoted values. Keywords are case-insensitive.

## Credentials and redirects

With `-L`, the `Authorization` header (from `-u`, `-H`, `--oauth2-bearer` or the URL) and custom `Cookie` headers are only sent to the host, port and scheme of the first request. A redirect to another host gets no credentials, so a token cannot leak to a third party. `--location-trusted` (which also turns on `-L`) sends them to every host, as in curl; use it only when you trust every server in the chain.

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

`WithAuth("Digest ...")` sets an arbitrary `Authorization` header value when you have already computed it. For a reusable client, `new LibCurl().WithBasicAuth(u, p)` and `.WithBearerToken(t)` apply to every request it makes. The builder has no Digest/NTLM methods; use a command string with `--digest` or `--ntlm` for those.

## Proxy authentication

Proxy credentials come from `-U` / `--proxy-user` or from `user:password@` in the proxy URL, and are sent pre-emptively as `Proxy-Authorization: Basic`. `--proxy-digest`, `--proxy-ntlm` and `--proxy-anyauth` negotiate with the proxy instead (for plain HTTP requests through the proxy); `--proxy-negotiate` and `--proxy-basic` are accepted too.

```csharp
var result = await Curl.ExecuteAsync(
    "curl -x http://proxy.example.com:8080 -U alice:s3cret --proxy-digest http://example.com/");
```

See [curl proxy in C#](proxy.html).

## Handle 401 and 403

A wrong password comes back as a result, not an exception, just as curl exits 0 with a 401 page:

```csharp
var result = await Curl.ExecuteAsync("curl -u user:wrong https://httpbin.org/basic-auth/user/passwd");

if (result.StatusCode == 401)
    Console.WriteLine($"Rejected; server offers: {result.GetHeader("WWW-Authenticate")}");
else if (result.StatusCode == 403)
    Console.WriteLine("Authenticated but not allowed");
```

If the authentication handshake itself cannot complete (for example the server's challenge cannot be answered), CurlDotNet throws `CurlAuthErrorException`, curl's exit code 94. Its `Scheme` and `Proxy` properties say which side failed:

```csharp
using CurlDotNet.Exceptions;

try
{
    await Curl.ExecuteAsync("curl --ntlm -u alice:secret https://intranet.example.com/api");
}
catch (CurlAuthErrorException ex)
{
    Console.WriteLine($"{ex.Scheme} negotiation failed (proxy: {ex.Proxy}), exit code {ex.CurlErrorCode}");
}
```

Add `-f` if you want a 401 or 403 to throw `CurlHttpReturnedErrorException` (exit code 22) instead of returning a result.

## AWS Signature V4

`--aws-sigv4` signs the request with the AWS algorithm; the credentials still travel through `-u`:

```csharp
var result = await Curl.ExecuteAsync(@"
    curl --aws-sigv4 'aws:amz:us-east-1:s3' -u ACCESS_KEY:SECRET_KEY \
         https://my-bucket.s3.amazonaws.com/object.txt");
```

## Gotchas

- Never hard-code secrets in the command string in source control; read them from environment variables or your secret store and interpolate them.
- A password containing `@`, `:` or spaces must be quoted: `-u 'user:p@ss word'`. Only the first colon separates user and password.
- Basic credentials are only safe over HTTPS; the header is just base64.
- Client certificates (mutual TLS) work on .NET 5 and later: `--cert client.pem --key client.key`, or `--cert client.p12:password --cert-type P12`, with `--pass` for an encrypted key and `--cacert` for a private CA. On .NET Standard 2.0 and .NET Framework these options are listed in `CurlOptions.UnsupportedOptions` and no certificate is sent. See [SSL certificate errors](../exceptions/ssl-errors.html).
- `~/.curlrc` is never read implicitly; put shared options in a file and load it with `-K`.

## Related

- [curl -u basic auth and bearer tokens tutorial](../tutorials/09-authentication-basics.html)
- [Authentication errors](../exceptions/auth-errors.html)
- [curl exit codes](../reference/curl-exit-codes.html)
- [curl follow redirects in C#](follow-redirects.html) - how credentials are handled across hops
- [Call a REST API with curl in C#](beginner/call-api.html)
- [Cookbook index](index.html)
