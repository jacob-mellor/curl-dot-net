---
title: "curl vs HttpClient in C#: When to Use Which"
description: "curl vs HttpClient in C#: an honest comparison of CurlDotNet's curl command strings and raw HttpClient, with clear cases for each."
---

# curl vs HttpClient in C#: when to use which

curl vs HttpClient in C# is not either-or: CurlDotNet runs curl commands on top of `HttpClient`. Use CurlDotNet when your starting point is a curl command or you want curl's behavior with less code. Use `HttpClient` directly when you need full control of the transport, streaming, or features CurlDotNet does not expose.

```bash
dotnet add package CurlDotNet
```

## The same request both ways

CurlDotNet, pasted from API docs:

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("""
    curl -X POST https://httpbin.org/post \
      -H "Authorization: Bearer abc123" \
      --json '{"name":"Ada"}'
    """);
Console.WriteLine(result.StatusCode);
```

Raw `HttpClient`:

```csharp
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using var client = new HttpClient();
using var request = new HttpRequestMessage(HttpMethod.Post, "https://httpbin.org/post");
request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "abc123");
request.Headers.Accept.ParseAdd("application/json");
request.Content = new StringContent("""{"name":"Ada"}""", Encoding.UTF8, "application/json");

using var response = await client.SendAsync(request);
Console.WriteLine((int)response.StatusCode);
```

CurlDotNet's typed alternative sits in between:

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithBearerToken("abc123").WithJson(new { name = "Ada" }).ExecuteAsync();
```

## Comparison

| | CurlDotNet | HttpClient |
|---|---|---|
| Starting from a curl command | Paste and run | Translate by hand |
| Redirects | Off unless `-L` (curl rules) | On by default |
| HTTP 4xx / 5xx | Result with `IsSuccess = false` (or an exception with `-f`) | Response with `IsSuccessStatusCode = false` |
| Network errors | `CurlException` with curl exit code | `HttpRequestException` / `TaskCanceledException` |
| Timeouts | None unless `-m` / `--connect-timeout`, as in curl | 100 seconds by default (`HttpClient.Timeout`) |
| Retries | `--retry` | Write your own, or use Polly |
| Binary vs text body | Detected for you | You choose the read method |
| Save to file | `-o` | Write the stream yourself |
| Streaming large bodies | No, the response is buffered in memory | Yes (`ResponseHeadersRead`) |
| HTTP/2 and HTTP/3 selection | `--http1.1`, `--http2`, `--http2-prior-knowledge`, `--http3` | `HttpRequestMessage.Version` |
| Client certificates, custom CA, key pinning | `--cert`, `--key`, `--cacert`, `--capath`, `--pinnedpubkey` (.NET 5+) | `HttpClientHandler` / `SocketsHttpHandler` |
| `IHttpClientFactory`, typed clients, Polly | Not integrated | First-class |
| Code generation (C#, Python, JS, PowerShell) | Built in | n/a |
| Dependencies | One NuGet package | In the box |

## Choose CurlDotNet when

- API docs, a vendor's support ticket or a browser's "Copy as cURL" gives you a curl command and you want it running now.
- You are writing scripts, test harnesses, health checks or internal tools where readability beats fine control.
- You want curl's semantics: opt-in redirects, exit-code-style errors, `--retry`, cookie jars, `-o`, `-u` with Digest or NTLM, `--aws-sigv4`.
- You keep commands in configuration or a database and run them as data. Only do that with trusted commands; see the injection note in [ASP.NET Core](../curl-in-aspnet-core/README.md).

## Choose HttpClient when

- You are building a high-throughput service and want `IHttpClientFactory`, handler pipelines and resilience policies.
- You stream large uploads or downloads, or need mutual TLS or a custom CA on .NET Framework (CurlDotNet applies `--cert` and `--cacert` on .NET 5 and later only).
- Every allocation matters. CurlDotNet parses a string per call; no benchmark is published, so measure in your own workload if this is a concern.

Mixing them is fine: prototype with the pasted curl command, then call `Curl.ToHttpClient(cmd)` for a starting point when you move a hot path to raw `HttpClient`. It carries over headers, body, `-u`, `-x`, `-k`, `-L`, `-m` and `-F`.

## Related

- [Convert curl to C#](../convert-curl-to-csharp/README.md)
- [How CurlDotNet works](../how-curldotnet-works/README.md)
- [curl vs Invoke-WebRequest](../../powershell/curl-vs-invoke-webrequest/README.md)
- [All how-to guides](../README.md)
