---
title: "libcurl Options Mapped to C# - CurlDotNet Reference"
description: "libcurl options mapped to C#: CURLOPT settings, CURLINFO values and CURLcode errors matched to CurlDotNet methods, CurlResult properties and exceptions."
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
| `CURLOPT_XOAUTH2_BEARER` | `--oauth2-bearer`, `-H 'Authorization: Bearer ...'` | `WithBearerToken(token)` | `WithBearerToken(token)` |
| `CURLOPT_HTTPAUTH` | `--basic`, `--digest`, `--ntlm`, `--negotiate`, `--anyauth` | - | - |
| `CURLOPT_NETRC`, `CURLOPT_NETRC_FILE` | `-n`, `--netrc-optional`, `--netrc-file` | - | - |
| `CURLOPT_TIMEOUT` | `-m` | `WithTimeout(TimeSpan)` | `WithTimeout(TimeSpan)` |
| `CURLOPT_CONNECTTIMEOUT` | `--connect-timeout` | `WithConnectTimeout(TimeSpan)` | `WithConnectTimeout(TimeSpan)` |
| `CURLOPT_FOLLOWLOCATION` | `-L` | `FollowRedirects()` | `WithFollowRedirects()` |
| `CURLOPT_MAXREDIRS` | `--max-redirs` | `WithMaxRedirects(n)` | `WithFollowRedirects(n)` |
| `CURLOPT_POSTREDIR` | `--post301`, `--post302`, `--post303` | - | - |
| `CURLOPT_UNRESTRICTED_AUTH` | `--location-trusted` | - | - |
| `CURLOPT_SSL_VERIFYPEER` | `-k` | `Insecure()` | `WithInsecureSsl()` |
| `CURLOPT_USERAGENT` | `-A` | `WithUserAgent(ua)` | `WithUserAgent(ua)` |
| `CURLOPT_REFERER` | `-e` | `WithReferer(url)` | - |
| `CURLOPT_COOKIE` | `-b 'a=1; b=2'` | `WithCookie(string)` | - |
| `CURLOPT_COOKIEFILE` | `-b file` | - | - |
| `CURLOPT_COOKIEJAR` | `-c file` | `WithCookieJar(path)` | - |
| `CURLOPT_COOKIESESSION` | `-j` | - | - |
| `CURLOPT_PROXY`, `CURLOPT_PROXYUSERPWD` | `-x`, `--proxy-user` | `WithProxy(url, user, pass)` | `WithProxy(url, user, pass)` |
| `CURLOPT_NOPROXY` | `--noproxy` (and `NO_PROXY`) | - | - |
| `CURLOPT_PROXYAUTH` | `--proxy-digest`, `--proxy-ntlm`, `--proxy-anyauth` | - | - |
| `CURLOPT_HTTPPROXYTUNNEL` | `-p` (.NET 5+) | - | - |
| `CURLOPT_HTTP_VERSION` | `--http1.0`, `--http1.1`, `--http2`, `--http2-prior-knowledge`, `--http3` | `WithHttpVersion("2.0")` | - |
| `CURLOPT_RESOLVE`, `CURLOPT_CONNECT_TO` | `--resolve`, `--connect-to` | - | - |
| `CURLOPT_UPLOAD`, `CURLOPT_READDATA` | `-T file` | - | - |
| `CURLOPT_RESUME_FROM_LARGE` | `-C N`, `-C -` | - | - |
| `CURLOPT_MAXFILESIZE_LARGE` | `--max-filesize` | - | - |
| `CURLOPT_TIMECONDITION`, `CURLOPT_TIMEVALUE` | `-z` | - | - |
| `CURLOPT_REQUEST_TARGET` | `--request-target` (.NET 5+) | - | - |
| `CURLOPT_PATH_AS_IS` | `--path-as-is` | - | - |
| `CURLOPT_ACCEPT_ENCODING` | `--compressed` | `Compressed()` | - |
| `CURLOPT_RANGE` | `-r` | `WithRange("0-1023")` | - |
| `CURLOPT_NOBODY` | `-I` | `CurlRequestBuilder.Head(url)` | `HeadAsync(url)` |
| `CURLOPT_HEADER` | `-i` | `IncludeHeaders()` | - |
| `CURLOPT_WRITEDATA` (to a file) | `-o path` | `WithOutput(path)` | `WithOutputFile(path)` |
| `CURLOPT_VERBOSE` | `-v` | `Verbose()` | `WithVerbose()` |

Options marked `-` in the builder and `LibCurl` columns are available through the command string (or by setting the matching `CurlOptions` property and calling `Curl.ExecuteAsync(options)`).

Retries are a feature of the curl tool rather than libcurl. CurlDotNet implements them: `--retry`, `--retry-delay`, `--retry-max-time`, `--retry-all-errors` and `--retry-connrefused` (`CurlOptions.Retry`, `RetryDelay`, `RetryMaxTime`, `RetryAllErrors`, `RetryConnRefused`), plus the .NET shortcut `new CurlSettings().WithRetries(3, 500)`. See [curl --retry in C#](../cookbook/retry.html). The same goes for `-w`/`--write-out` ([curl -w in C#](../cookbook/write-out.html)) and for `-K` config files, `--variable` and URL globbing ([config files and variables](../cookbook/config-files-and-variables.html), [multiple URLs and globbing](../cookbook/multiple-urls-and-globbing.html)).

Not mapped: `CURLOPT_SSLCERT`/`CURLOPT_SSLKEY`/`CURLOPT_CAINFO`/`CURLOPT_CAPATH`/`CURLOPT_PINNEDPUBLICKEY` (`--cert`, `--key`, `--cacert`, `--capath`, `--pinnedpubkey`) are parsed into `CurlOptions.CertFile`, `KeyFile` and `CaCertFile` but not applied; certificates are verified against the operating system's trust store. `CURLOPT_MAX_RECV_SPEED_LARGE` (`--limit-rate`) and `CURLOPT_LOW_SPEED_LIMIT` (`--speed-limit`) are parsed but not applied.

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
| `CURLINFO_REDIRECT_COUNT`, `CURLINFO_HTTP_VERSION`, `CURLINFO_PRIMARY_IP`, ... | `-w '%{num_redirects}'`, `'%{http_version}'`, `'%{remote_ip}'`: every `-w` variable is supported, see [curl -w in C#](../cookbook/write-out.html) |
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

libcurl returns a `CURLcode`; CurlDotNet throws a typed exception from `CurlDotNet.Exceptions` (all derive from `CurlException`, which exposes `CurlErrorCode` (the same number), `Command`, `Suggestions` and `StandardOutput`). The full list of exit codes is in [curl exit codes](curl-exit-codes.html).

| libcurl code | CurlDotNet exception | When |
|--------------|---------------------|------|
| `CURLE_OK` (0) | (none) | Success; HTTP 4xx/5xx still return a result with `IsSuccess == false` unless `-f` is given |
| `CURLE_UNSUPPORTED_PROTOCOL` (1) | `CurlUnsupportedProtocolException` | Scheme other than http, https, ftp, ftps, file (also on a redirect), or an unusable status line / HTTP/0.9 reply |
| `CURLE_FAILED_INIT` (2) | `CurlException` with `CurlErrorCode == 2` | Conflicting request methods (`-I` with `-d`, `-F` with `-d`, ...) or an option missing its value |
| `CURLE_URL_MALFORMAT` (3) | `CurlMalformedUrlException` | URL cannot be parsed (a bad URL glob is a `CurlException` with `CurlErrorCode == 3`) |
| `CURLE_COULDNT_RESOLVE_PROXY` (5) | `CurlCouldntResolveProxyException` | Proxy host name cannot be resolved |
| `CURLE_COULDNT_RESOLVE_HOST` (6) | `CurlCouldntResolveHostException` | Host name cannot be resolved |
| `CURLE_COULDNT_CONNECT` (7) | `CurlCouldntConnectException` | Connection refused or unreachable (the message names the proxy when one is used) |
| `CURLE_WEIRD_SERVER_REPLY` (8) | `CurlWeirdServerReplyException` | Malformed response header |
| `CURLE_PARTIAL_FILE` (18) | `CurlPartialFileException` | The connection closed before the announced body length arrived |
| `CURLE_HTTP_RETURNED_ERROR` (22) | `CurlHttpReturnedErrorException` | `-f`/`--fail` (or `--fail-with-body`) and HTTP status 400 or above |
| `CURLE_WRITE_ERROR` (23) | `CurlWriteErrorException` | An output file (`-o`, `-D`, `-c`) cannot be written, for example a missing directory |
| `CURLE_READ_ERROR` (26) | `CurlReadErrorException` | An input file (`-d @file`, `-T`, `-F file=@path`, `-K`) is missing or unreadable |
| `CURLE_OPERATION_TIMEDOUT` (28) | `CurlOperationTimeoutException` | `--max-time` or `--connect-timeout` exceeded |
| `CURLE_RANGE_ERROR` (33) | `CurlRangeErrorException` | `-C` resume asked for, but the server ignored the range |
| `CURLE_SSL_CONNECT_ERROR` (35) | `CurlSslConnectErrorException` | TLS handshake failed |
| `CURLE_BAD_FUNCTION_ARGUMENT` (43) | `CurlBadFunctionArgumentException` | CR or LF inside a header value, or a resumed upload |
| `CURLE_TOO_MANY_REDIRECTS` (47) | `CurlTooManyRedirectsException` | More than `--max-redirs` redirects |
| `CURLE_SETOPT_OPTION_SYNTAX` (49) | `CurlOptionSyntaxException` | Bad `--resolve` or `--connect-to` value |
| `CURLE_GOT_NOTHING` (52) | `CurlGotNothingException` | The server closed the connection without replying |
| `CURLE_RECV_ERROR` (56) | `CurlReceiveErrorException` | Receiving data failed |
| `CURLE_PEER_FAILED_VERIFICATION` (60) | `CurlPeerFailedVerificationException` | Server certificate rejected (`-k` skips the check) |
| `CURLE_BAD_CONTENT_ENCODING` (61) | `CurlBadContentEncodingException` | Broken chunked or compressed (`--compressed`) body |
| `CURLE_FILESIZE_EXCEEDED` (63) | `CurlFileSizeExceededException` | Body larger than `--max-filesize` |
| `CURLE_AUTH_ERROR` (94) | `CurlAuthErrorException` | A Digest/NTLM challenge could not be processed (a wrong password is a normal 401 result) |
| `CURLE_TOO_LARGE` (100) | `CurlTooLargeException` | The response header block is larger than curl accepts |
| `CURLE_ABORTED_BY_CALLBACK` (42) | standard `OperationCanceledException` | Your `CancellationToken` was cancelled; CurlDotNet surfaces cancellation the .NET way, not as a `CurlException` |

`result.EnsureSuccess()` and `EnsureStatus()` throw `CurlDotNet.Core.CurlHttpException` after the fact, when you did not pass `-f`.

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

Connection reuse is handled by the underlying `HttpClient`; there is no equivalent of `curl_share_*` to configure. Several URLs in one command (`curl URL1 URL2`, globs, `--next`) run sequentially, like the curl tool without `-Z`; `--parallel` is parsed but not applied.

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
