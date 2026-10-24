---
title: libcurl Options Mapped to C# - CurlDotNet Reference
description: libcurl options mapped to C#: CURLOPT settings, CURLINFO values and CURLcode errors matched to CurlDotNet methods, CurlResult properties and exceptions.
---

# libcurl Options Mapped to C# (CURLOPT, CURLINFO and CURLcode in CurlDotNet)

This reference shows libcurl options mapped to C#: each common `CURLOPT_*` setting, `CURLINFO_*` value and `CURLE_*` return code next to the CurlDotNet method, `CurlResult` property or exception that plays the same role. CurlDotNet is pure C# (it does not wrap the libcurl binary), so the mapping is by behaviour, not by P/Invoke.

## Easy Interface Concepts

| libcurl | CurlDotNet | Notes |
|---------|------------|-------|
| `curl_easy_init()` | `using var curl = new LibCurl();` | `CurlDotNet.Lib.LibCurl` is the reusable instance API; the static `Curl` class needs no init |
| `curl_easy_setopt()` | Fluent methods (`WithHeader`, `WithTimeout`, ...) or flags inside the command string | |
| `curl_easy_perform()` | `Curl.ExecuteAsync(cmd)`, `builder.ExecuteAsync()`, `curl.GetAsync(url)` | Sync `Curl.Execute` also exists |
| `curl_easy_getinfo()` | `CurlResult` properties | See CURLINFO table |
| `curl_easy_cleanup()` | `LibCurl.Dispose()` | Static `Curl` has nothing to clean up |

## CURLOPT Options

| libcurl option | curl flag | `CurlRequestBuilder` | `LibCurl` |
|----------------|-----------|----------------------|-----------|
| `CURLOPT_URL` | (positional URL) | `CurlRequestBuilder.Get(url)` etc. | `GetAsync(url)` |
| `CURLOPT_CUSTOMREQUEST` | `-X` | `Request(method, url)` | - |
| `CURLOPT_HTTPHEADER` | `-H` | `WithHeader(name, value)` | `WithHeader(name, value)` |
| `CURLOPT_POSTFIELDS` | `-d` | `WithData(string)`, `WithJson(object)` | `PostAsync(url, data)` |
| `CURLOPT_MIMEPOST` | `-F` | `WithMultipartForm(fields, files)`, `WithFile(field, path)` | - |
| `CURLOPT_USERPWD` | `-u` | `WithBasicAuth(user, pass)` | `WithBasicAuth(user, pass)` |
| `CURLOPT_XOAUTH2_BEARER` | `-H 'Authorization: Bearer ...'` | `WithBearerToken(token)` | `WithBearerToken(token)` |
| `CURLOPT_TIMEOUT` | `-m` | `WithTimeout(TimeSpan)` | `WithTimeout(TimeSpan)` |
| `CURLOPT_CONNECTTIMEOUT` | `--connect-timeout` | `WithConnectTimeout(TimeSpan)` | `WithConnectTimeout(TimeSpan)` |
| `CURLOPT_FOLLOWLOCATION` | `-L` | `FollowRedirects()` | `WithFollowRedirects()` |
| `CURLOPT_MAXREDIRS` | `--max-redirs` | `WithMaxRedirects(n)` | `WithFollowRedirects(n)` |
| `CURLOPT_SSL_VERIFYPEER` | `-k` | `Insecure()` | `WithInsecureSsl()` |
| `CURLOPT_USERAGENT` | `-A` | `WithUserAgent(ua)` | `WithUserAgent(ua)` |
| `CURLOPT_REFERER` | `-e` | `WithReferer(url)` | - |
| `CURLOPT_COOKIE` | `-b 'a=1; b=2'` | `WithCookie(string)` | - |
| `CURLOPT_PROXY`, `CURLOPT_PROXYUSERPWD` | `-x`, `--proxy-user` | `WithProxy(url, user, pass)` | `WithProxy(url, user, pass)` |
| `CURLOPT_ACCEPT_ENCODING` | `--compressed` | `Compressed()` | - |
| `CURLOPT_RANGE` | `-r` | `WithRange("0-1023")` | - |
| `CURLOPT_NOBODY` | `-I` | `CurlRequestBuilder.Head(url)` | `HeadAsync(url)` |
| `CURLOPT_HEADER` | `-i` | `IncludeHeaders()` | - |
| `CURLOPT_WRITEDATA` (to a file) | `-o path` | `WithOutput(path)` | `WithOutputFile(path)` |
| `CURLOPT_VERBOSE` | `-v` | `Verbose()` | `WithVerbose()` |

Not yet mapped: `CURLOPT_SSLCERT`/`CURLOPT_SSLKEY`/`CURLOPT_CAINFO` (`--cert`, `--key`, `--cacert`), `CURLOPT_COOKIEJAR`/`CURLOPT_COOKIEFILE` (`-c`, `-b file`), `CURLOPT_HTTP_VERSION` (`--http2`) and `CURLOPT_UPLOAD` (`-T`). Retries (the curl tool's `--retry`) are a `CurlSettings` concern: `new CurlSettings().WithRetries(3, 500)`.

## HTTP Methods

| curl | libcurl | CurlDotNet |
|------|---------|------------|
| `curl URL` | `CURLOPT_HTTPGET` | `Curl.GetAsync(url)`, `CurlRequestBuilder.Get(url)` |
| `curl -X POST -d ...` | `CURLOPT_POST` | `Curl.PostAsync(url, data)`, `Curl.PostJsonAsync(url, obj)`, `CurlRequestBuilder.Post(url)` |
| `curl -X PUT` | `CURLOPT_CUSTOMREQUEST` | `CurlRequestBuilder.Put(url)`, `LibCurl.PutAsync(url, data)` |
| `curl -X DELETE` | `CURLOPT_CUSTOMREQUEST` | `CurlRequestBuilder.Delete(url)`, `LibCurl.DeleteAsync(url)` |
| `curl -X PATCH` | `CURLOPT_CUSTOMREQUEST` | `CurlRequestBuilder.Patch(url)`, `LibCurl.PatchAsync(url, data)` |
| `curl -I` | `CURLOPT_NOBODY` | `CurlRequestBuilder.Head(url)`, `LibCurl.HeadAsync(url)` |

## CURLINFO Response Information

| libcurl info | `CurlResult` member |
|--------------|---------------------|
| `CURLINFO_RESPONSE_CODE` | `StatusCode` (and `IsSuccess` for 2xx) |
| `CURLINFO_CONTENT_TYPE` | `GetHeader("Content-Type")` |
| `CURLINFO_SIZE_DOWNLOAD` | `Body.Length` or `BinaryData.Length` |
| `CURLINFO_EFFECTIVE_URL` | `-w '%{url_effective}'` in the command (appended to `Body`) |
| `CURLINFO_NAMELOOKUP_TIME` | `Timings.NameLookup` (ms) |
| `CURLINFO_CONNECT_TIME` | `Timings.Connect` |
| `CURLINFO_APPCONNECT_TIME` | `Timings.AppConnect` |
| `CURLINFO_PRETRANSFER_TIME` | `Timings.PreTransfer` |
| `CURLINFO_REDIRECT_TIME` | `Timings.Redirect` |
| `CURLINFO_STARTTRANSFER_TIME` | `Timings.StartTransfer` |
| `CURLINFO_TOTAL_TIME` | `Timings.Total` |

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -L https://httpbin.org/redirect/1");
Console.WriteLine(result.StatusCode);
Console.WriteLine(result.GetHeader("Content-Type"));
Console.WriteLine($"{result.Timings.Total} ms total, {result.Timings.Connect} ms connect");
```

## CURLcode Errors

libcurl returns a `CURLcode`; CurlDotNet throws a typed exception from `CurlDotNet.Exceptions` (all derive from `CurlException`, which exposes `CurlErrorCode`, `Command` and `Suggestions`).

| libcurl code | CurlDotNet exception | When |
|--------------|---------------------|------|
| `CURLE_OK` | (none) | Success; HTTP 4xx/5xx still return a result with `IsSuccess == false` |
| `CURLE_UNSUPPORTED_PROTOCOL` | `CurlUnsupportedProtocolException` | Scheme other than http, https, ftp, ftps, file |
| `CURLE_URL_MALFORMAT` | `CurlMalformedUrlException` | URL cannot be parsed |
| `CURLE_COULDNT_CONNECT` | `CurlCouldntConnectException` | DNS failure, refused connection or TLS handshake failure |
| `CURLE_OPERATION_TIMEDOUT` | `CurlOperationTimeoutException` | `--max-time` exceeded |
| `CURLE_TOO_MANY_REDIRECTS` | `CurlTooManyRedirectsException` | `--max-redirs` exceeded |
| `CURLE_ABORTED_BY_CALLBACK` | `CurlAbortedByCallbackException` | Your `CancellationToken` was cancelled |
| `CURLE_READ_ERROR` | `CurlFileCouldntReadException` | `-d @file` or `-F file=@path` not found |
| `CURLE_HTTP_RETURNED_ERROR` | `CurlDotNet.Core.CurlHttpException` | Only when you call `result.EnsureSuccess()` or `EnsureStatus()` |

```csharp
using CurlDotNet;
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl --max-time 2 https://httpbin.org/delay/5");
}
catch (CurlOperationTimeoutException ex)
{
    Console.WriteLine($"Timed out after {ex.Timeout}: {ex.Message}");
}
```

## Side-by-Side Examples

### Basic GET

```c
CURL *curl = curl_easy_init();
curl_easy_setopt(curl, CURLOPT_URL, "https://httpbin.org/get");
curl_easy_perform(curl);
curl_easy_cleanup(curl);
```

```csharp
using CurlDotNet;

var result = await Curl.GetAsync("https://httpbin.org/get");
Console.WriteLine(result.Body);
```

### POST with headers

```c
struct curl_slist *headers = NULL;
headers = curl_slist_append(headers, "Content-Type: application/json");
curl_easy_setopt(curl, CURLOPT_URL, "https://httpbin.org/post");
curl_easy_setopt(curl, CURLOPT_HTTPHEADER, headers);
curl_easy_setopt(curl, CURLOPT_POSTFIELDS, "{\"key\":\"value\"}");
curl_easy_perform(curl);
```

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithHeader("Content-Type", "application/json")
    .WithData("{\"key\":\"value\"}")
    .ExecuteAsync();
Console.WriteLine(result.StatusCode);
```

### Timeouts and redirects

```c
curl_easy_setopt(curl, CURLOPT_URL, "https://httpbin.org/redirect/3");
curl_easy_setopt(curl, CURLOPT_TIMEOUT, 30L);
curl_easy_setopt(curl, CURLOPT_FOLLOWLOCATION, 1L);
curl_easy_setopt(curl, CURLOPT_MAXREDIRS, 5L);
curl_easy_perform(curl);
```

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Get("https://httpbin.org/redirect/3")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .FollowRedirects()
    .WithMaxRedirects(5)
    .ExecuteAsync();
Console.WriteLine(result.StatusCode);
```

### Bearer token with a reusable handle

```c
curl_easy_setopt(curl, CURLOPT_URL, "https://httpbin.org/bearer");
curl_easy_setopt(curl, CURLOPT_HTTPAUTH, CURLAUTH_BEARER);
curl_easy_setopt(curl, CURLOPT_XOAUTH2_BEARER, "token123");
curl_easy_perform(curl);
```

```csharp
using CurlDotNet.Lib;

using var curl = new LibCurl();
curl.WithBearerToken("token123").WithTimeout(TimeSpan.FromSeconds(10));
var result = await curl.GetAsync("https://httpbin.org/bearer");
Console.WriteLine(result.StatusCode);
```

## Multi Interface (Parallel Transfers)

libcurl's `curl_multi_*` API runs several transfers at once. In CurlDotNet each call is already a `Task`, so use `Curl.ExecuteManyAsync` or `Task.WhenAll`:

```csharp
using CurlDotNet;

var results = await Curl.ExecuteManyAsync(
    "curl https://httpbin.org/get",
    "curl https://api.github.com",
    "curl https://httpbin.org/uuid");

foreach (var r in results)
    Console.WriteLine($"{r.StatusCode} {r.Body.Length} bytes");
```

Connection reuse is handled by the underlying `HttpClient`; there is no equivalent of `curl_share_*` to configure.

## Migrating from libcurl C

1. Replace `curl_easy_setopt` chains with a curl command string, a `CurlRequestBuilder` chain, or `LibCurl` fluent calls.
2. Replace `CURLcode` checks with `try`/`catch` on the exceptions above and `result.IsSuccess` for HTTP status.
3. Drop manual memory management (`curl_slist_free_all`, `curl_easy_cleanup`); only `LibCurl` is `IDisposable`.
4. Use `await` instead of blocking `curl_easy_perform` calls.

## Further Reading

- [libcurl documentation](https://curl.se/libcurl/c/)
- [curl CLI Compatibility](curl-cli-compatibility.html)
- [API Guide](../api-guide/README.html)
- [Error Reference](../troubleshooting/error-reference.html)
