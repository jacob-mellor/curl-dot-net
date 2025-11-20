# CurlDotNet to libcurl API Mapping

This document maps CurlDotNet's .NET API to the underlying libcurl C library concepts, helping developers understand the relationship between the two APIs.

## Overview

CurlDotNet provides a .NET wrapper around libcurl's functionality, offering three main API styles:
1. **String-based API** - Direct curl command execution
2. **Fluent Builder API** - Type-safe request building
3. **Low-level LibCurl API** - Direct libcurl option setting

## Core Concepts Mapping

### libcurl Easy Interface → CurlDotNet Core Classes

| libcurl Concept | CurlDotNet Equivalent | Description |
|----------------|----------------------|-------------|
| `CURL*` handle | `CurlEasy` class | Single transfer handle |
| `curl_easy_init()` | `new CurlEasy()` | Initialize a session |
| `curl_easy_setopt()` | `CurlEasy.SetOpt()` | Set transfer options |
| `curl_easy_perform()` | `CurlEasy.Perform()` | Execute transfer |
| `curl_easy_cleanup()` | `CurlEasy.Dispose()` | Clean up resources |
| `curl_easy_getinfo()` | `CurlResult` properties | Get transfer information |

### Common CURLOPT Options Mapping

| libcurl Option | CurlDotNet Method/Property | Example |
|---------------|---------------------------|---------|
| `CURLOPT_URL` | `SetUrl()` | `.SetUrl("https://api.example.com")` |
| `CURLOPT_HTTPHEADER` | `AddHeader()` | `.AddHeader("Accept", "application/json")` |
| `CURLOPT_POSTFIELDS` | `SetBody()` | `.SetBody("{\"key\":\"value\"}")` |
| `CURLOPT_CUSTOMREQUEST` | `SetMethod()` | `.SetMethod("PUT")` |
| `CURLOPT_TIMEOUT` | `SetTimeout()` | `.SetTimeout(TimeSpan.FromSeconds(30))` |
| `CURLOPT_FOLLOWLOCATION` | `FollowRedirects()` | `.FollowRedirects(true)` |
| `CURLOPT_SSL_VERIFYPEER` | `SetSslVerification()` | `.SetSslVerification(false)` |
| `CURLOPT_USERAGENT` | `SetUserAgent()` | `.SetUserAgent("MyApp/1.0")` |
| `CURLOPT_COOKIE` | `AddCookie()` | `.AddCookie("session=abc123")` |
| `CURLOPT_HTTPAUTH` | `SetAuthentication()` | `.SetAuthentication("user", "pass")` |
| `CURLOPT_PROXY` | `SetProxy()` | `.SetProxy("http://proxy:8080")` |
| `CURLOPT_VERBOSE` | `SetVerbose()` | `.SetVerbose(true)` |

### HTTP Methods Mapping

| curl Command | libcurl | CurlDotNet |
|-------------|---------|------------|
| `curl -X GET` | `CURLOPT_HTTPGET` | `Curl.GetAsync()` |
| `curl -X POST` | `CURLOPT_POST` | `Curl.PostAsync()` |
| `curl -X PUT` | `CURLOPT_UPLOAD` | `Curl.PutAsync()` |
| `curl -X DELETE` | `CURLOPT_CUSTOMREQUEST` | `Curl.DeleteAsync()` |
| `curl -X PATCH` | `CURLOPT_CUSTOMREQUEST` | `Curl.PatchAsync()` |
| `curl -I` | `CURLOPT_NOBODY` | `Curl.HeadAsync()` |

### Response Information (CURLINFO) Mapping

| libcurl Info | CurlDotNet Property | Description |
|-------------|-------------------|-------------|
| `CURLINFO_RESPONSE_CODE` | `CurlResult.StatusCode` | HTTP response code |
| `CURLINFO_TOTAL_TIME` | `CurlResult.ElapsedTime` | Total request time |
| `CURLINFO_SIZE_DOWNLOAD` | `CurlResult.DownloadSize` | Bytes downloaded |
| `CURLINFO_SIZE_UPLOAD` | `CurlResult.UploadSize` | Bytes uploaded |
| `CURLINFO_CONTENT_TYPE` | `CurlResult.ContentType` | Content-Type header |
| `CURLINFO_EFFECTIVE_URL` | `CurlResult.EffectiveUrl` | Final URL after redirects |
| `CURLINFO_PRIMARY_IP` | `CurlResult.ServerIp` | Server IP address |
| `CURLINFO_NAMELOOKUP_TIME` | `CurlTimings.NameLookup` | DNS resolution time |
| `CURLINFO_CONNECT_TIME` | `CurlTimings.Connect` | Connection time |
| `CURLINFO_PRETRANSFER_TIME` | `CurlTimings.PreTransfer` | Pre-transfer time |
| `CURLINFO_STARTTRANSFER_TIME` | `CurlTimings.StartTransfer` | Time to first byte |

### Error Codes (CURLcode) Mapping

| libcurl Error | CurlDotNet Exception | Description |
|--------------|---------------------|-------------|
| `CURLE_OK` | (no exception) | Success |
| `CURLE_UNSUPPORTED_PROTOCOL` | `CurlUnsupportedProtocolException` | Protocol not supported |
| `CURLE_FAILED_INIT` | `CurlFailedInitException` | Failed initialization |
| `CURLE_URL_MALFORMAT` | `CurlMalformedUrlException` | Malformed URL |
| `CURLE_COULDNT_RESOLVE_HOST` | `CurlCouldntResolveHostException` | DNS resolution failed |
| `CURLE_COULDNT_CONNECT` | `CurlCouldntConnectException` | Connection failed |
| `CURLE_HTTP_RETURNED_ERROR` | `CurlHttpException` | HTTP error response |
| `CURLE_OPERATION_TIMEDOUT` | `CurlTimeoutException` | Operation timeout |
| `CURLE_SSL_CONNECT_ERROR` | `CurlSslConnectErrorException` | SSL connection error |
| `CURLE_TOO_MANY_REDIRECTS` | `CurlTooManyRedirectsException` | Redirect limit exceeded |
| `CURLE_AUTH_ERROR` | `CurlAuthenticationException` | Authentication failed |

## Usage Examples

### Example 1: Basic GET Request

**libcurl (C)**:
```c
CURL *curl = curl_easy_init();
curl_easy_setopt(curl, CURLOPT_URL, "https://api.example.com/data");
curl_easy_perform(curl);
curl_easy_cleanup(curl);
```

**CurlDotNet**:
```csharp
var result = await Curl.GetAsync("https://api.example.com/data");
```

### Example 2: POST with Headers

**libcurl (C)**:
```c
CURL *curl = curl_easy_init();
struct curl_slist *headers = NULL;
headers = curl_slist_append(headers, "Content-Type: application/json");
curl_easy_setopt(curl, CURLOPT_URL, "https://api.example.com/data");
curl_easy_setopt(curl, CURLOPT_HTTPHEADER, headers);
curl_easy_setopt(curl, CURLOPT_POSTFIELDS, "{\"key\":\"value\"}");
curl_easy_perform(curl);
curl_slist_free_all(headers);
curl_easy_cleanup(curl);
```

**CurlDotNet**:
```csharp
var result = await Curl.PostAsync(
    "https://api.example.com/data",
    "{\"key\":\"value\"}",
    "application/json"
);
```

### Example 3: Custom Options

**libcurl (C)**:
```c
CURL *curl = curl_easy_init();
curl_easy_setopt(curl, CURLOPT_URL, "https://api.example.com/data");
curl_easy_setopt(curl, CURLOPT_TIMEOUT, 30L);
curl_easy_setopt(curl, CURLOPT_FOLLOWLOCATION, 1L);
curl_easy_setopt(curl, CURLOPT_MAXREDIRS, 5L);
curl_easy_perform(curl);
curl_easy_cleanup(curl);
```

**CurlDotNet**:
```csharp
var result = await new CurlRequestBuilder()
    .SetUrl("https://api.example.com/data")
    .SetTimeout(TimeSpan.FromSeconds(30))
    .FollowRedirects(true)
    .SetMaxRedirects(5)
    .Build()
    .ExecuteAsync();
```

### Example 4: Authentication

**libcurl (C)**:
```c
CURL *curl = curl_easy_init();
curl_easy_setopt(curl, CURLOPT_URL, "https://api.example.com/secure");
curl_easy_setopt(curl, CURLOPT_HTTPAUTH, CURLAUTH_BEARER);
curl_easy_setopt(curl, CURLOPT_XOAUTH2_BEARER, "token123");
curl_easy_perform(curl);
curl_easy_cleanup(curl);
```

**CurlDotNet**:
```csharp
var result = await new CurlRequestBuilder()
    .SetUrl("https://api.example.com/secure")
    .AddHeader("Authorization", "Bearer token123")
    .Build()
    .ExecuteAsync();
```

## Advanced Features

### Multi Interface (Parallel Transfers)

| libcurl Multi | CurlDotNet | Description |
|--------------|------------|-------------|
| `curl_multi_init()` | `CurlMulti()` | Initialize multi handle |
| `curl_multi_add_handle()` | `AddHandle()` | Add easy handle to multi |
| `curl_multi_perform()` | `PerformAsync()` | Execute all transfers |
| `curl_multi_cleanup()` | `Dispose()` | Clean up multi handle |

**Example**:
```csharp
// CurlDotNet parallel execution
var tasks = urls.Select(url => Curl.GetAsync(url));
var results = await Task.WhenAll(tasks);
```

### Share Interface (Connection Sharing)

| libcurl Share | CurlDotNet | Description |
|--------------|------------|-------------|
| `curl_share_init()` | `CurlShare()` | Initialize share handle |
| `curl_share_setopt()` | `SetShareOpt()` | Configure sharing |
| `CURLSHOPT_SHARE` | Share cookies/DNS/SSL | Share data between handles |

## Performance Considerations

### Connection Reuse
- **libcurl**: Uses `curl_easy_reset()` to reuse handles
- **CurlDotNet**: Automatic connection pooling via `HttpClient` backend

### DNS Caching
- **libcurl**: `CURLOPT_DNS_CACHE_TIMEOUT`
- **CurlDotNet**: Automatic DNS caching with configurable TTL

### SSL Session Caching
- **libcurl**: `CURLOPT_SSL_SESSIONID_CACHE`
- **CurlDotNet**: Automatic SSL session reuse

## Migration Guide

### From libcurl C to CurlDotNet

1. **Replace curl_easy_* functions** with CurlDotNet class methods
2. **Convert CURLOPT_* options** to fluent method calls
3. **Replace manual memory management** with .NET garbage collection
4. **Use async/await** instead of blocking calls
5. **Handle exceptions** instead of checking return codes

### From curl Command Line to CurlDotNet

Simply use the string-based API:
```csharp
// Direct command execution
var result = await Curl.ExecuteAsync("curl -X GET https://api.example.com -H 'Accept: application/json'");
```

## Platform-Specific Notes

### Windows
- Uses Windows Certificate Store for SSL
- Supports Windows Authentication (NTLM/Kerberos)

### Linux/macOS
- Uses OpenSSL/LibreSSL for SSL
- Supports system proxy settings

### Mobile (Xamarin/MAUI)
- Reduced binary size via linking
- Platform-specific HTTP handlers available

## Further Reading

- [libcurl Documentation](https://curl.se/libcurl/c/)
- [CurlDotNet API Reference](https://jacob-mellor.github.io/curl-dot-net)
- [curl Command Line Tool](https://curl.se/docs/)