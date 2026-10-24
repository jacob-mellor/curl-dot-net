---
title: "curl to HttpClient in C# - Side-by-Side Migration Guide"
description: "curl to HttpClient in C#, side by side. Convert a curl command to C# HttpClient code with Curl.ToHttpClient, or run it directly with CurlDotNet."
---

# curl to HttpClient in C#: a side-by-side migration guide

Going from curl to HttpClient in C# usually means reading a `curl ...` line from API docs and rebuilding it by hand with `HttpRequestMessage`, `StringContent` and `DefaultRequestHeaders`. CurlDotNet gives you two shortcuts: generate the `HttpClient` code from the curl command, or skip the translation and run the curl command directly. This guide shows both, side by side with the hand-written `HttpClient` equivalent, and then covers moving existing `HttpClient` code to CurlDotNet.

## Convert a curl command to C# HttpClient code

`Curl.ToHttpClient` parses a curl command and emits the equivalent `HttpClient` C#. Use it when you want plain `HttpClient` in your codebase but do not want to translate by hand.

```csharp
using CurlDotNet;

var cmd = "curl -H 'Accept: application/json' -H 'User-Agent: MyApp/1.0' https://api.github.com/users/octocat";
Console.WriteLine(Curl.ToHttpClient(cmd));
```

Output:

```csharp
using var client = new HttpClient();
client.DefaultRequestHeaders.Add("Accept", "application/json");
client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
var response = await client.GetAsync("https://api.github.com/users/octocat");
var body = await response.Content.ReadAsStringAsync();
```

The generator covers method, URL, headers and request body. Review the output before committing it: data containing quotes is emitted verbatim and may need escaping, and options such as proxies, cookies and timeouts are not translated. The same parser also drives `Curl.ToFetch`, `Curl.ToPythonRequests` and `Curl.ToPowershellCode`.

## Or run the curl command directly

If you do not need `HttpClient` specifically, the shortest path is to run the command as-is:

```csharp
var result = await Curl.ExecuteAsync(cmd);
Console.WriteLine(result.StatusCode);
Console.WriteLine(result.Body);
```

## Side by side: curl, HttpClient, CurlDotNet

### GET with headers

```bash
curl -H "Accept: application/json" https://httpbin.org/headers
```

HttpClient:

```csharp
using var client = new HttpClient();
client.DefaultRequestHeaders.Add("Accept", "application/json");
var response = await client.GetAsync("https://httpbin.org/headers");
response.EnsureSuccessStatusCode();
var body = await response.Content.ReadAsStringAsync();
```

CurlDotNet:

```csharp
var result = await Curl.ExecuteAsync("curl -H 'Accept: application/json' https://httpbin.org/headers");
```

```csharp
var result = await CurlRequestBuilder.Get("https://httpbin.org/headers")
    .WithHeader("Accept", "application/json")
    .ExecuteAsync();
```

### curl -X POST JSON in C#

```bash
curl -X POST https://httpbin.org/post -H "Content-Type: application/json" -d '{"name":"Ada"}'
```

HttpClient:

```csharp
using var client = new HttpClient();
var json = JsonSerializer.Serialize(new { name = "Ada" });
using var content = new StringContent(json, Encoding.UTF8, "application/json");
var response = await client.PostAsync("https://httpbin.org/post", content);
var body = await response.Content.ReadAsStringAsync();
```

CurlDotNet:

```csharp
var result = await Curl.ExecuteAsync(@"curl -X POST https://httpbin.org/post
    -H 'Content-Type: application/json' -d '{""name"":""Ada""}'");
```

```csharp
var result = await CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithJson(new { name = "Ada" })
    .ExecuteAsync();
```

### curl bearer token in C#

```bash
curl -H "Authorization: Bearer token123" https://httpbin.org/bearer
```

HttpClient:

```csharp
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "token123");
var response = await client.GetAsync("https://httpbin.org/bearer");
```

CurlDotNet:

```csharp
var result = await Curl.ExecuteAsync("curl -H 'Authorization: Bearer token123' https://httpbin.org/bearer");
```

```csharp
var result = await CurlRequestBuilder.Get("https://httpbin.org/bearer").WithBearerToken("token123").ExecuteAsync();
```

### curl basic auth in C#

```bash
curl -u user:passwd https://httpbin.org/basic-auth/user/passwd
```

HttpClient:

```csharp
var bytes = Encoding.ASCII.GetBytes("user:passwd");
client.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));
```

CurlDotNet:

```csharp
var result = await Curl.ExecuteAsync("curl -u user:passwd https://httpbin.org/basic-auth/user/passwd");
```

### curl timeout in C#

```bash
curl -m 10 https://httpbin.org/delay/2
```

HttpClient:

```csharp
using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
try { var response = await client.GetAsync("https://httpbin.org/delay/2"); }
catch (TaskCanceledException) { Console.WriteLine("Timed out"); }
```

CurlDotNet:

```csharp
using CurlDotNet.Exceptions;

try { var result = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/delay/2"); }
catch (CurlTimeoutException) { Console.WriteLine("Timed out"); }
```

### curl follow redirects in C#

`HttpClient` follows redirects by default; curl does not unless `-L` is given. CurlDotNet follows curl.

```csharp
var result = await Curl.ExecuteAsync("curl -L https://httpbin.org/redirect/2");   // 200
var raw    = await Curl.ExecuteAsync("curl https://httpbin.org/redirect/2");      // 302
```

To get `HttpClient`-like behaviour everywhere, set `Curl.DefaultFollowRedirects = true` once at startup.

### curl ignore SSL certificate in C#

HttpClient:

```csharp
var handler = new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
};
using var client = new HttpClient(handler);
```

CurlDotNet:

```csharp
var result = await Curl.ExecuteAsync("curl -k https://self-signed.badssl.com/");
```

### curl download file in C#

HttpClient:

```csharp
using var stream = await client.GetStreamAsync("https://httpbin.org/image/png");
using var file = File.Create("image.png");
await stream.CopyToAsync(file);
```

CurlDotNet:

```csharp
var result = await Curl.ExecuteAsync("curl -o image.png https://httpbin.org/image/png");
// or: await Curl.DownloadAsync("https://httpbin.org/image/png", "image.png");
```

### Form post and file upload

HttpClient:

```csharp
using var form = new MultipartFormDataContent();
form.Add(new StringContent("Report"), "title");
form.Add(new StreamContent(File.OpenRead("report.pdf")), "file", "report.pdf");
var response = await client.PostAsync("https://httpbin.org/post", form);
```

CurlDotNet:

```csharp
var result = await Curl.ExecuteAsync("curl -F 'title=Report' -F 'file=@report.pdf' https://httpbin.org/post");
```

### curl proxy in C#

HttpClient:

```csharp
var handler = new HttpClientHandler
{
    Proxy = new WebProxy("http://proxy.internal:8080") { Credentials = new NetworkCredential("alice", "secret") }
};
using var client = new HttpClient(handler);
```

CurlDotNet:

```csharp
var result = await Curl.ExecuteAsync(
    "curl -x http://proxy.internal:8080 --proxy-user alice:secret https://httpbin.org/ip");
```

## Error handling compared

`HttpClient` throws `HttpRequestException` for network failures and, after `EnsureSuccessStatusCode()`, for HTTP errors; timeouts surface as `TaskCanceledException`. CurlDotNet returns HTTP errors as a normal `CurlResult` with `IsSuccess == false`, and throws typed `CurlException` subclasses for network failures.

```csharp
using CurlDotNet.Exceptions;

try
{
    var result = await Curl.ExecuteAsync("curl -m 10 https://httpbin.org/status/404");
    if (!result.IsSuccess)
        Console.WriteLine($"HTTP {result.StatusCode}");
}
catch (CurlTimeoutException)       { /* -m exceeded */ }
catch (CurlConnectionException ex) { /* DNS, refused, reset */ }
catch (CurlSslException ex)        { /* certificate problems */ }
```

Call `result.EnsureSuccess()` if you prefer an exception (`CurlDotNet.Core.CurlHttpException`) for non-2xx responses, as with `EnsureSuccessStatusCode()`.

## Dependency injection

HttpClient:

```csharp
services.AddHttpClient<GitHubService>(client =>
{
    client.BaseAddress = new Uri("https://api.github.com");
    client.DefaultRequestHeaders.Add("User-Agent", "MyApp/1.0");
});
```

CurlDotNet with `LibCurl` (reusable, `IDisposable`, register as a singleton):

```csharp
using CurlDotNet.Lib;

services.AddSingleton(_ => new LibCurl()
    .WithUserAgent("MyApp/1.0")
    .WithTimeout(TimeSpan.FromSeconds(20)));

public class GitHubService(LibCurl curl)
{
    public Task<CurlResult> GetUserAsync(string login) =>
        curl.GetAsync($"https://api.github.com/users/{login}");
}
```

Or `CurlApiClient` for a single base URL: `new CurlApiClient("https://httpbin.org")` with `GetAsync("/get")`, `PostJsonAsync`, `PutJsonAsync`, `PatchJsonAsync` and `DeleteAsync`.

## Migration checklist: HttpClient to CurlDotNet

- Replace `client.GetAsync` + `ReadAsStringAsync` pairs with `Curl.ExecuteAsync(...)` and `result.Body`.
- Replace `StringContent` JSON bodies with `-d` / `WithJson` / `Curl.PostJsonAsync`.
- Replace `response.IsSuccessStatusCode` with `result.IsSuccess`; `(int)response.StatusCode` with `result.StatusCode`.
- Replace `EnsureSuccessStatusCode()` with `result.EnsureSuccess()`.
- Replace `JsonSerializer.Deserialize<T>(body)` with `result.ParseJson<T>()`.
- Add `-L` (or set `Curl.DefaultFollowRedirects = true`) where you relied on `HttpClient` following redirects automatically.
- Add a `User-Agent` header where the API requires one (GitHub does); `HttpClient` and CurlDotNet both send none by default.
- Register `LibCurl` or `CurlApiClient` as a singleton where you used `AddHttpClient`.

## Related

- [RestSharp vs CurlDotNet](restsharp.html)
- [Quick start](../getting-started/quickstart.html)
- [API guide](../api-guide/README.html)
- [Advanced guide](../guides/advanced.html)
