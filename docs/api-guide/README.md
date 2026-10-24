---
title: "CurlDotNet API Guide - curl Commands and Builder in C#"
description: "CurlDotNet API guide for C#: the static Curl string API, the CurlRequestBuilder fluent API, the LibCurl client, CurlResult, CurlSettings and exceptions."
---

# CurlDotNet API guide: curl commands, fluent builder and LibCurl in C#

This CurlDotNet API guide covers the three ways to make requests in C#: the static `Curl` string API that runs curl commands, the `CurlRequestBuilder` fluent API, and the reusable `LibCurl` client. All three return the same `CurlResult`.

## The three APIs

### Curl string API

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl -A 'MyApp/1.0' -H 'Accept: application/json' https://api.github.com/repos/curl/curl");
```

Best for copy-pasting commands from API docs, shell scripts and Stack Overflow. The leading `curl` is optional and multi-line commands with `\` continuations are accepted.

### CurlRequestBuilder fluent API

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder.Get("https://api.github.com/repos/curl/curl")
    .WithUserAgent("MyApp/1.0")
    .WithHeader("Accept", "application/json")
    .WithBearerToken("token123")
    .ExecuteAsync();
```

Best for building requests programmatically with IntelliSense and compile-time checking. `ToCurlCommand()` gives you back the equivalent curl string for logging or bug reports.

### LibCurl reusable client

```csharp
using CurlDotNet.Lib;

using var curl = new LibCurl()
    .WithUserAgent("MyApp/1.0")
    .WithBearerToken("token123")
    .WithTimeout(TimeSpan.FromSeconds(30));

var result = await curl.GetAsync("https://api.github.com/user");
```

Best for shared configuration and dependency injection. `LibCurl` is `IDisposable`; register one instance as a singleton.

## Which API should you use?

| Need | Use |
|------|-----|
| Run an existing curl command unchanged | String API |
| IntelliSense, refactoring, dynamic requests | `CurlRequestBuilder` |
| Shared headers, auth and timeouts across many calls, DI | `LibCurl` |
| Thin wrapper around one REST base URL | `CurlApiClient` (below) |

## Curl static class

| Member | Purpose |
|--------|---------|
| `ExecuteAsync(string)` / `Execute(string)` | Run a curl command string |
| `ExecuteAsync(string, CancellationToken)` | Same, cancellable |
| `ExecuteAsync(string, CurlSettings)` | Same, with per-call settings (timeout, retries, redirects) |
| `ExecuteAsync(CurlOptions)` / `Execute(CurlOptions)` | Run from a pre-built options object (no command string, no quoting); `ExecuteAsync(CurlOptions, CancellationToken)` is cancellable |
| `GetAsync(url)` / `Get(url)` | Shorthand for `curl <url>` |
| `PostAsync(url, data)` / `Post(url, data)` | Shorthand for `curl -d <data> <url>` |
| `PostJsonAsync(url, object)` / `PostJson(url, object)` | Serialize an object and POST it as JSON |
| `DownloadAsync(url, path)` / `Download(url, path)` | Shorthand for `curl -o <path> <url>` |
| `ExecuteManyAsync(params string[])` | Run several commands concurrently |
| `Validate(cmd)` | Parse a command without executing it |
| `ToHttpClient(cmd)`, `ToFetch(cmd)`, `ToPythonRequests(cmd)`, `ToPowershellCode(cmd)` | Generate equivalent code in other stacks |
| `DefaultMaxTimeSeconds`, `DefaultConnectTimeoutSeconds`, `DefaultFollowRedirects`, `DefaultInsecure` | Process-wide defaults applied when a command does not specify them; out of the box 0 (no timeout), 0, `false` (no redirects without `-L`) and `false`, matching curl |
| `DefaultUserAgent` | User-Agent sent when a command sets none; defaults to `curl/8.7.1` |
| `CurlCompatVersion` | The curl release whose behaviour is reproduced (`"8.7.1"`) |

```csharp
// Opt in to process-wide defaults (none of these apply unless you set them)
Curl.DefaultMaxTimeSeconds = 30;
Curl.DefaultFollowRedirects = true;

var result = await Curl.ExecuteAsync("https://httpbin.org/redirect/1");  // now follows the redirect, with a 30 s cap
```

Without those two lines the same call returns the 302 itself and waits as long as the server takes, exactly like `curl https://httpbin.org/redirect/1`.

## CurlResult

| Member | Type | Description |
|--------|------|-------------|
| `StatusCode` | `int` | HTTP status code |
| `IsSuccess` | `bool` | `true` for 2xx |
| `Body` | `string` | What curl would print to stdout: the response body as text, plus `-v`/`-i`/`-I` header text and `-w` output; empty with `-o`/`-O` |
| `Headers` | `Dictionary<string,string>` | Response headers, case-insensitive keys |
| `BinaryData` | `byte[]` | Response bytes for binary content |
| `IsBinary` | `bool` | `true` when `BinaryData` is populated |
| `Timings` | `CurlTimings` | `NameLookup`, `Connect`, `AppConnect`, `PreTransfer`, `Redirect`, `StartTransfer`, `Total` in milliseconds |
| `OutputFiles` | `List<string>` | Files written by `-o` / `-O` |
| `Exception` | `Exception` | Set when a request failed without throwing (the `LibCurl` instance API returns such a result instead of throwing) |
| `ParseJson<T>()` / `AsJson<T>()` | `T` | Deserialize the body; property names match case-insensitively |
| `AsJsonDynamic()` | `dynamic` | Parse without a type: a `JsonDocument` on .NET 8+ and .NET Framework, a Newtonsoft `JObject` on .NET Standard 2.0 |
| `GetHeader(name)` / `HasHeader(name)` | | Case-insensitive header lookup |
| `EnsureSuccess()` | `CurlResult` | Throw `CurlDotNet.Core.CurlHttpException` when not 2xx |
| `SaveToFile(path)` | `CurlResult` | Write body or bytes to disk |
| `ToStream()` | `Stream` | Body as a stream |

```csharp
var result = await Curl.ExecuteAsync("curl https://httpbin.org/json");

result.EnsureSuccess();
using var slideshow = System.Text.Json.JsonDocument.Parse(result.Body);
Console.WriteLine(slideshow.RootElement.GetProperty("slideshow").GetProperty("title"));
Console.WriteLine(result.GetHeader("content-type"));
Console.WriteLine($"{result.Timings.Total:F0} ms total");
```

## CurlSettings

Per-call settings that apply on top of the command string.

```csharp
using CurlDotNet.Core;

var settings = new CurlSettings()
    .WithTimeout(30)
    .WithFollowRedirects(true)
    .WithRetries(count: 3, delayMs: 1000);   // retry any failure 3 times, 1 s apart

var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/503", settings);
```

Also applied: `WithConnectTimeout(seconds)`, `WithInsecure()`, `WithHeader(key, value)`, `WithHeaders(dict)`, `WithCancellation(token)`. `WithUserAgent`, `WithProxy`, `WithCookies`, `WithAutoDecompression`, `WithBufferSize`, `WithRedirectHandler` and `WithProgress` exist on the class but the engine does not apply them; use `-A`, `-x`, `-b`/`-c` and `--compressed` in the command instead. `WithRetries` is the .NET shortcut for `--retry N --retry-delay S --retry-all-errors`; see [curl --retry in C#](../cookbook/retry.html).

## CurlRequestBuilder reference

| Group | Methods |
|-------|---------|
| Create | `Get`, `Post`, `Put`, `Delete`, `Patch`, `Head`, `Request(method, url)` |
| Headers | `WithHeader`, `WithHeaders`, `WithUserAgent`, `WithReferer` |
| Body | `WithData(string)`, `WithJson(object)`, `WithFormData(dict)`, `WithMultipartForm(fields, files)`, `WithFile(field, path)`, `WithBinaryData(byte[])` |
| Auth | `WithBasicAuth(user, pass)`, `WithBearerToken(token)`, `WithAuth(headerValue)` |
| Timing | `WithTimeout(TimeSpan)`, `WithConnectTimeout(TimeSpan)` |
| Redirects | `FollowRedirects(bool = true)`, `WithMaxRedirects(int)` |
| SSL and proxy | `Insecure(bool = true)`, `WithProxy(url)`, `WithProxy(url, user, pass)` |
| Cookies | `WithCookie(string)` |
| Output | `WithOutput(path)`, `IncludeHeaders()`, `Verbose()`, `Compressed()`, `WithRange(range)`, `AsBinary()` |
| Run | `Execute()`, `ExecuteAsync(CancellationToken = default)`, `ToCurlCommand()` |

```csharp
var builder = CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithMultipartForm(
        fields: new Dictionary<string, string> { ["title"] = "Report" },
        files:  new Dictionary<string, string> { ["file"] = "report.pdf" });

Console.WriteLine(builder.ToCurlCommand());   // curl -X POST -F 'title=Report' -F 'file=@report.pdf' ...
var result = await builder.ExecuteAsync();
```

## LibCurl reference

Instance methods: `GetAsync`, `PostAsync`, `PutAsync`, `PatchAsync`, `DeleteAsync`, `HeadAsync`, each taking `(url, Action<CurlOptions>? configure = null, CancellationToken ct = default)`; `PostAsync`, `PutAsync` and `PatchAsync` also take a `data` object.

Unlike the static `Curl` API, `LibCurl` does not throw for curl failures: it returns a `CurlResult` with `StatusCode == 0` and the `CurlException` in `result.Exception`. A cancelled token still throws `OperationCanceledException`.

Configuration (chainable, applies to every request): `WithHeader`, `WithBearerToken`, `WithBasicAuth`, `WithTimeout`, `WithConnectTimeout`, `WithFollowRedirects(maxRedirects)`, `WithInsecureSsl()`, `WithProxy(url, user?, pass?)`, `WithUserAgent`, `WithOutputFile`, `WithVerbose()`.

```csharp
// Program.cs
builder.Services.AddSingleton(_ => new LibCurl()
    .WithHeader("User-Agent", "MyApp/1.0")
    .WithTimeout(TimeSpan.FromSeconds(15)));

// a service
public class GitHubService(LibCurl curl)
{
    public Task<CurlResult> GetUserAsync(string login) =>
        curl.GetAsync($"https://api.github.com/users/{login}");
}
```

## CurlApiClient

A small wrapper for one REST base URL in the `CurlDotNet` namespace.

```csharp
using CurlDotNet;

var api = new CurlApiClient("https://httpbin.org");
var created = await api.PostJsonAsync("/post", new { name = "Ada" });
var fetched = await api.GetAsync("/get");
var deleted = await api.DeleteAsync("/delete");
```

Also: `PutJsonAsync(path, object)`, `PatchJsonAsync(path, object)`.

## String extension methods

In `CurlDotNet.Extensions`:

```csharp
using CurlDotNet.Extensions;

var r1 = await "curl https://httpbin.org/get".CurlAsync();
var r2 = await "https://httpbin.org/get".CurlGetAsync();
var r3 = await "https://httpbin.org/image/png".CurlDownloadAsync("image.png");
```

## Code generation and validation

```csharp
var cmd = "curl -A 'MyApp/1.0' -H 'Accept: application/json' https://api.github.com/users/octocat";

Console.WriteLine(Curl.ToHttpClient(cmd));      // C# HttpClient code
Console.WriteLine(Curl.ToFetch(cmd));           // JavaScript fetch
Console.WriteLine(Curl.ToPythonRequests(cmd));  // Python requests
Console.WriteLine(Curl.ToPowershellCode(cmd));  // PowerShell Invoke-RestMethod

var validation = Curl.Validate(cmd);            // parse only, no network
```

## Exceptions

All network-level failures throw a subclass of `CurlDotNet.Exceptions.CurlException`, which carries `CurlErrorCode` (curl's exit code), `Command`, a `Suggestions` list and `StandardOutput` (everything curl would have printed, including `-w` output). HTTP 4xx/5xx responses do not throw unless you call `EnsureSuccess()` or pass `-f`. A cancelled `CancellationToken` throws the standard `OperationCanceledException`, not a `CurlException`.

| Exception | When |
|-----------|------|
| `CurlTimeoutException` (`CurlOperationTimeoutException`) | `-m` / `--max-time` or connect timeout exceeded (exit 28) |
| `CurlHttpReturnedErrorException` | `-f` / `--fail` and HTTP status 400 or above (exit 22) |
| `CurlConnectionException`, `CurlCouldntConnectException` | Connection refused, reset or unreachable |
| `CurlDnsException`, `CurlCouldntResolveHostException` | Host name could not be resolved |
| `CurlSslException` | Certificate or TLS handshake failure |
| `CurlAuthenticationException` | Authentication could not be performed |
| `CurlProxyException` | Proxy could not be reached or rejected the request |
| `CurlMalformedUrlException`, `CurlUnsupportedProtocolException` | Bad URL or unsupported scheme |
| `CurlInvalidCommandException` | The command string could not be parsed |
| `CurlReadErrorException`, `CurlWriteErrorException` | An input file (`-d @file`, `-T`, `-F`) is missing (exit 26) or an output file cannot be written (exit 23) |

Full list with fixes: [Exceptions](../exceptions/index.html). Every exit code: [curl exit codes](../reference/curl-exit-codes.html).

## Related

- [Quick start](../getting-started/quickstart.html)
- [Advanced guide](../guides/advanced.html)
- [curl CLI compatibility](../reference/curl-cli-compatibility.html)
- [Generated API reference](../api/index.html)
