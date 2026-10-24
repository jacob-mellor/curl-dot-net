---
title: "Advanced CurlDotNet - Timeouts, Proxies, Cookies in C#"
description: "Advanced CurlDotNet guide for C#: curl timeout, retries, follow redirects, ignore SSL certificate, proxy, cookies, download file, timings and cancellation."
---

# Advanced CurlDotNet: timeouts, retries, proxies, cookies and downloads in C#

This advanced CurlDotNet guide shows how to express the curl options you reach for in production, in C#: timeouts, retries, redirects, SSL, proxies, cookies, binary downloads, timings, cancellation and request signing. Every section shows the curl-string form first and the `CurlRequestBuilder` form second where one exists.

```csharp
using CurlDotNet;         // Curl.ExecuteAsync
using CurlDotNet.Core;    // CurlRequestBuilder, CurlResult, CurlSettings
```

## curl timeout in C#

`-m` / `--max-time` caps the whole transfer; `--connect-timeout` caps only the connection phase. Exceeding either throws `CurlTimeoutException`.

```csharp
var result = await Curl.ExecuteAsync("curl -m 10 --connect-timeout 3 https://httpbin.org/delay/2");
```

```csharp
var result = await CurlRequestBuilder.Get("https://httpbin.org/delay/2")
    .WithTimeout(TimeSpan.FromSeconds(10))
    .WithConnectTimeout(TimeSpan.FromSeconds(3))
    .ExecuteAsync();
```

Set process-wide defaults once instead of repeating them:

```csharp
Curl.DefaultMaxTimeSeconds = 30;
Curl.DefaultConnectTimeoutSeconds = 5;
```

## Retry a failed request

Retries are configured through `CurlSettings` and apply to any command.

```csharp
var settings = new CurlSettings().WithRetries(count: 3, delayMs: 500);
var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/503", settings);
```

## Cancel a request with a CancellationToken

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

try
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/delay/10", cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Cancelled");
}
```

`CurlRequestBuilder.ExecuteAsync(token)` and every `LibCurl` method accept a token too.

## curl follow redirects in C#

curl does not follow redirects unless you pass `-L`. CurlDotNet behaves the same way.

```csharp
var result = await Curl.ExecuteAsync("curl -L --max-redirs 5 https://httpbin.org/redirect/3");
Console.WriteLine(result.StatusCode);   // 200
```

```csharp
var result = await CurlRequestBuilder.Get("https://httpbin.org/redirect/3")
    .FollowRedirects()
    .WithMaxRedirects(5)
    .ExecuteAsync();
```

Without `-L` you get the 3xx response itself and can read `result.GetHeader("Location")`. To follow by default everywhere, set `Curl.DefaultFollowRedirects = true`.

## curl ignore SSL certificate in C#

`-k` / `--insecure` skips certificate validation. Use it only against development servers with self-signed certificates.

```csharp
var result = await Curl.ExecuteAsync("curl -k https://self-signed.badssl.com/");
```

```csharp
var result = await CurlRequestBuilder.Get("https://self-signed.badssl.com/")
    .Insecure()
    .ExecuteAsync();
```

Certificate failures without `-k` throw `CurlSslException`; see [SSL errors](../exceptions/ssl-errors.html).

## curl proxy in C#

```csharp
var result = await Curl.ExecuteAsync(
    "curl -x http://proxy.internal:8080 --proxy-user alice:secret https://httpbin.org/ip");

var viaSocks = await Curl.ExecuteAsync("curl --socks5 127.0.0.1:1080 https://httpbin.org/ip");
```

```csharp
var result = await CurlRequestBuilder.Get("https://httpbin.org/ip")
    .WithProxy("http://proxy.internal:8080", "alice", "secret")
    .ExecuteAsync();
```

For a shared client, `new LibCurl().WithProxy(url, user, pass)` applies the proxy to every request.

## curl with cookies in C#

Send cookies with `-b "name=value; other=value"`.

```csharp
var result = await Curl.ExecuteAsync("curl -b 'session=abc123; theme=dark' https://httpbin.org/cookies");
```

```csharp
var result = await CurlRequestBuilder.Get("https://httpbin.org/cookies")
    .WithCookie("session=abc123; theme=dark")
    .ExecuteAsync();
```

Cookies the server sets come back in `result.Headers["Set-Cookie"]`. To carry a `CookieContainer` across several calls, use `new CurlSettings().WithCookies(container)`.

## curl download file in C#

`-o` saves to a named file; `-O` uses the remote file name. Saved paths are listed in `result.OutputFiles`.

```csharp
var result = await Curl.ExecuteAsync("curl -o archive.zip https://httpbin.org/bytes/1024");
Console.WriteLine(result.OutputFiles[0]);   // archive.zip
```

```csharp
var result = await Curl.DownloadAsync("https://httpbin.org/bytes/1024", "archive.zip");
```

To keep binary content in memory instead, read `result.BinaryData`. CurlDotNet detects binary content from the `Content-Type` header and by sniffing the first bytes, so images, PDFs, zips and Office files are never corrupted by text decoding.

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/image/png");
if (result.IsBinary)
    File.WriteAllBytes("image.png", result.BinaryData);   // or result.SaveToFile("image.png")
```

Partial downloads use `-r` / `--range`:

```csharp
var firstKb = await Curl.ExecuteAsync("curl -r 0-1023 https://httpbin.org/range/4096");
```

## Compressed responses

`--compressed` asks for gzip, deflate or br and transparently decompresses the body.

```csharp
var result = await Curl.ExecuteAsync("curl --compressed https://httpbin.org/gzip");
```

## Response headers and HEAD requests

```csharp
var head = await Curl.ExecuteAsync("curl -I https://httpbin.org/get");          // headers only
var both = await Curl.ExecuteAsync("curl -i https://httpbin.org/get");          // headers + body in Body
Console.WriteLine(head.GetHeader("content-type"));
```

## Timings and write-out variables

`result.Timings` exposes curl's timing phases in milliseconds. `-w` appends the formatted write-out string to `result.Body`, supporting `%{http_code}`, `%{size_download}`, `%{url_effective}` and `%{content_type}`.

```csharp
var result = await Curl.ExecuteAsync("curl -A 'MyApp/1.0' https://api.github.com");
Console.WriteLine($"DNS {result.Timings.NameLookup:F0} ms, TLS {result.Timings.AppConnect:F0} ms, total {result.Timings.Total:F0} ms");

var withCode = await Curl.ExecuteAsync("curl -w '%{http_code}' https://httpbin.org/status/204");
Console.WriteLine(withCode.Body);   // 204 (no body, so only the write-out)
```

## Verbose output and tracing

`-v` writes the request and response lines into the result; `--trace file` and `--trace-ascii file` write full wire dumps to disk for support tickets.

```csharp
var result = await Curl.ExecuteAsync("curl -v https://httpbin.org/get");
var traced = await Curl.ExecuteAsync("curl --trace-ascii trace.txt https://httpbin.org/get");
```

See [Logging and observability](logging-observability.html) for wiring this into `ILogger`.

## Authentication schemes

```csharp
var basic  = await Curl.ExecuteAsync("curl -u user:passwd https://httpbin.org/basic-auth/user/passwd");
var digest = await Curl.ExecuteAsync("curl --digest -u user:passwd https://httpbin.org/digest-auth/auth/user/passwd");
var bearer = await Curl.ExecuteAsync("curl -H 'Authorization: Bearer token123' https://httpbin.org/bearer");
```

`--ntlm`, `--negotiate` and `--anyauth` are also recognised. For AWS APIs, `--aws-sigv4` signs the request:

```csharp
var s3 = await Curl.ExecuteAsync(
    "curl --aws-sigv4 'aws:amz:us-east-1:s3' -u \"$AWS_ACCESS_KEY_ID:$AWS_SECRET_ACCESS_KEY\" https://my-bucket.s3.amazonaws.com/key");
```

Substitute real credentials into the string in C#; CurlDotNet does not expand shell variables.

## Form posts and file uploads

```csharp
var form = await Curl.ExecuteAsync("curl -d 'name=Ada' -d 'lang=C%23' https://httpbin.org/post");
var urlEncoded = await Curl.ExecuteAsync("curl --data-urlencode 'q=C# curl' https://httpbin.org/post");
var multipart = await Curl.ExecuteAsync("curl -F 'title=Report' -F 'file=@report.pdf' https://httpbin.org/post");
```

```csharp
var result = await CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithFormData(new Dictionary<string, string> { ["name"] = "Ada", ["lang"] = "C#" })
    .ExecuteAsync();
```

## Run requests in parallel

```csharp
var results = await Curl.ExecuteManyAsync(
    "curl -A 'MyApp/1.0' https://api.github.com/repos/curl/curl",
    "curl -A 'MyApp/1.0' https://api.github.com/repos/dotnet/runtime",
    "curl -A 'MyApp/1.0' https://api.github.com/repos/jacob-mellor/curl-dot-net");
```

For finer control, start several `ExecuteAsync` tasks and `await Task.WhenAll(...)`.

## Reusable, injectable client

When many calls share headers, auth and timeouts, use `LibCurl` from `CurlDotNet.Lib` rather than repeating flags:

```csharp
using CurlDotNet.Lib;

using var curl = new LibCurl()
    .WithBearerToken(token)
    .WithUserAgent("MyApp/1.0")
    .WithFollowRedirects(5)
    .WithTimeout(TimeSpan.FromSeconds(20));

var me = await curl.GetAsync("https://api.github.com/user");
var repo = await curl.PostAsync("https://api.github.com/user/repos", new { name = "demo" });
```

## Convert a curl command to other languages

```csharp
var cmd = "curl -A 'MyApp/1.0' -H 'Accept: application/json' https://api.github.com/users/octocat";
Console.WriteLine(Curl.ToHttpClient(cmd));
Console.WriteLine(Curl.ToFetch(cmd));
Console.WriteLine(Curl.ToPythonRequests(cmd));
Console.WriteLine(Curl.ToPowershellCode(cmd));
```

## Related

- [API guide](../api-guide/README.html)
- [curl CLI compatibility: which flags are supported](../reference/curl-cli-compatibility.html)
- [Exceptions reference](../exceptions/index.html)
- [Cookbook](../cookbook/index.html)
