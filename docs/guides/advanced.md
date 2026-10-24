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

Like curl, there is no timeout unless you set one. `-m` / `--max-time` caps the whole transfer, body included; `--connect-timeout` caps only the connection phase (enforced on .NET 5 and later, best-effort on .NET Standard 2.0 / .NET Framework). Both take decimals such as `-m 2.5`. Exceeding either throws `CurlOperationTimeoutException` (a `CurlTimeoutException`, curl exit code 28).

```csharp
var result = await Curl.ExecuteAsync("curl -m 10 --connect-timeout 3 https://httpbin.org/delay/2");
```

```csharp
var result = await CurlRequestBuilder.Get("https://httpbin.org/delay/2")
    .WithTimeout(TimeSpan.FromSeconds(10))
    .WithConnectTimeout(TimeSpan.FromSeconds(3))
    .ExecuteAsync();
```

Set process-wide defaults once instead of repeating them (both default to 0, meaning no limit):

```csharp
Curl.DefaultMaxTimeSeconds = 30;
Curl.DefaultConnectTimeoutSeconds = 5;
```

## Retry a failed request

curl's own `--retry` works: it retries timeouts and HTTP 408, 429, 500, 502, 503 and 504, waiting 1 s, 2 s, 4 s... (capped at 10 minutes) unless `--retry-delay` fixes the wait, and it honours `Retry-After`. `--retry-max-time` caps the total, `--retry-connrefused` adds refused connections and `--retry-all-errors` retries anything.

```csharp
var result = await Curl.ExecuteAsync("curl --retry 3 --retry-delay 0.5 https://httpbin.org/status/503");
```

`CurlSettings.WithRetries` is the .NET shortcut; it retries any failure with a fixed delay and applies to any command:

```csharp
var settings = new CurlSettings().WithRetries(count: 3, delayMs: 500);
var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/503", settings);
```

See [curl --retry in C#](../cookbook/retry.html) for the details.

## Cancel a request with a CancellationToken

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

try
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/delay/10", cts.Token);
}
catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
{
    Console.WriteLine("Cancelled");
}
```

Cancellation surfaces as the standard `OperationCanceledException` (not a `CurlException`), so it is easy to tell apart from an expired `-m`. `CurlRequestBuilder.ExecuteAsync(token)` and every `LibCurl` method accept a token too.

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

Redirects follow curl's rules: 301 and 302 turn a POST into a GET (unless `--post301`/`--post302`), 303 turns everything but GET and HEAD into GET (unless `--post303`), and 307/308 keep the method and body. `--max-redirs N` allows exactly N hops (`-1` is unlimited); one more throws `CurlTooManyRedirectsException` (exit 47). `Authorization` and `Cookie` headers are dropped when a redirect changes host, unless you add `--location-trusted`.

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

`-x` also reads credentials from the proxy URL (`http://alice:secret@proxy.internal:8080`), and like curl CurlDotNet honours the `http_proxy`, `https_proxy`, `all_proxy` and `NO_PROXY` environment variables and `--noproxy`. `--proxy-digest`, `--proxy-ntlm` and `--proxy-anyauth` choose the proxy authentication scheme, and `-p` tunnels with `CONNECT` on .NET 5 and later. A proxy name that does not resolve throws `CurlCouldntResolveProxyException` (exit 5).

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

Cookies the server sets come back in `result.Headers["Set-Cookie"]`. To carry cookies across several calls, use curl's cookie jar: `-c jar.txt` writes them in Netscape format and `-b jar.txt` sends them back, with curl's domain, path, expiry and `Secure` rules. (`CurlSettings.WithCookies(container)` is not applied by the engine; use the jar.)

```csharp
await Curl.ExecuteAsync("curl -c jar.txt https://httpbin.org/cookies/set?session=abc123");
var next = await Curl.ExecuteAsync("curl -b jar.txt -c jar.txt https://httpbin.org/cookies");
```

## curl download file in C#

`-o` saves to a named file; `-O` uses the remote file name. Saved paths are listed in `result.OutputFiles`, and `result.Body` stays empty, as curl prints nothing to stdout in that case. `-C -` resumes an interrupted download, `--create-dirs` creates missing directories and `--max-filesize` refuses oversized bodies (exit 63).

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

`--compressed` sends `Accept-Encoding` for gzip, deflate and br and decodes the body. Without it, no `Accept-Encoding` is sent, as with curl. The response headers stay exactly as the server sent them, so `Content-Encoding` and `Content-Length` (visible with `-i`) still describe the compressed bytes.

```csharp
var result = await Curl.ExecuteAsync("curl --compressed https://httpbin.org/gzip");
```

## Response headers and HEAD requests

```csharp
var head = await Curl.ExecuteAsync("curl -I https://httpbin.org/get");          // header block in Body
var both = await Curl.ExecuteAsync("curl -i https://httpbin.org/get");          // headers + body in Body
var dump = await Curl.ExecuteAsync("curl -D headers.txt https://httpbin.org/get"); // headers to a file
Console.WriteLine(head.GetHeader("content-type"));
```

## Timings and write-out variables

`result.Timings` exposes curl's timing phases in milliseconds. `-w` appends the formatted write-out string to `result.Body`, after the body and with no extra newline, and supports every curl variable: `%{http_code}`, `%{time_total}` and the other `time_*` values, `%{size_download}`, `%{url_effective}`, `%{num_redirects}`, `%header{name}`, `%{json}` and more. See [curl -w in C#](../cookbook/write-out.html).

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");
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

Digest and NTLM are negotiated inside CurlDotNet, in pure C# and byte-for-byte like curl; `--anyauth` picks Digest, then NTLM, then Basic from the server's challenge, and `--negotiate` uses .NET's own Negotiate support. Credentials can also come from `.netrc` (`-n`, `--netrc-file`); see [config files, variables and .netrc](../cookbook/config-files-and-variables.html). For AWS APIs, `--aws-sigv4` signs the request:

```csharp
var s3 = await Curl.ExecuteAsync(
    "curl --aws-sigv4 'aws:amz:us-east-1:s3' -u \"$AWS_ACCESS_KEY_ID:$AWS_SECRET_ACCESS_KEY\" https://my-bucket.s3.amazonaws.com/key");
```

Like a shell, CurlDotNet expands `$NAME`, `${NAME}` and `%NAME%` from the process environment outside single quotes (the double-quoted variables above are expanded), and leaves them alone inside single quotes.

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
    "curl https://api.github.com/repos/curl/curl",
    "curl https://api.github.com/repos/dotnet/runtime",
    "curl https://api.github.com/repos/jacob-mellor/curl-dot-net");
```

For finer control, start several `ExecuteAsync` tasks and `await Task.WhenAll(...)`. Several URLs in one command (`curl URL1 URL2`, globs such as `[1-3]`, or `--next`) all run, one after another; see [multiple URLs and globbing](../cookbook/multiple-urls-and-globbing.html).

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
