# CurlDotNet Examples

Runnable C# samples for CurlDotNet. Every file in this folder is compiled by
[CurlDotNet.Examples.csproj](CurlDotNet.Examples.csproj) against the real library, so the
samples cannot drift from the API:

```bash
dotnet build examples/CurlDotNet.Examples.csproj -c Release
```

The project is a compile check (a library), not an app. To run a sample, copy the file into a
console project that references the `CurlDotNet` package, then call its `Main` or `RunAsync`.
The samples call public test services (httpbin.org, JSONPlaceholder, api.github.com,
quotes.toscrape.com), so they need network access.

## Example Categories

### Basic Examples

- **[00-CompleteSample.cs](BasicExamples/00-CompleteSample.cs)** - A tour of the main entry points:
  a pasted command, `Curl.GetAsync` with `Timings.Total`, `Curl.PostJsonAsync`, the fluent
  `CurlRequestBuilder`, `EnsureSuccess()` with `CurlHttpException`, and a multi-line command with an
  `Authorization` header.
- **[01-SimpleGetRequest.cs](BasicExamples/01-SimpleGetRequest.cs)** - One GET request three ways:
  command string, `CurlRequestBuilder.Get(...).WithHeader(...)`, and `Curl.GetAsync(url)`.
- **[02-PostJsonData.cs](BasicExamples/02-PostJsonData.cs)** - POSTing JSON as a raw `-d` command,
  with `WithJson(object)`, with `Curl.PostJsonAsync`, and with extra headers; reading the reply with
  `ParseJson<T>()`.
- **[03-ErrorHandling.cs](BasicExamples/03-ErrorHandling.cs)** - `IsSuccess`, switching on
  `StatusCode`, `EnsureSuccess()`, and the typed exceptions for a malformed URL
  (`CurlMalformedUrlException`), a timeout (`CurlOperationTimeoutException`), an unreachable host
  (`CurlCouldntConnectException`) and caller cancellation (`OperationCanceledException`), plus
  built-in retries with `CurlSettings.WithRetries`.
- **[04-SimpleGetFromOld.cs](BasicExamples/04-SimpleGetFromOld.cs)** - The `CurlResult` object:
  `IsSuccess`, `StatusCode`, `Body`, `Headers` and `GetHeader`, with query parameters in the URL.
- **[05-PostJsonFromOld.cs](BasicExamples/05-PostJsonFromOld.cs)** - Sending a JSON string with
  `WithData` and a `Content-Type` header, sending C# objects with `PostJsonAsync`, parsing the created
  resource with `ParseJson<T>()`, and JSON POSTs written as curl commands with custom headers.

### Authentication

- **[01-BearerToken.cs](Authentication/01-BearerToken.cs)** - Bearer tokens via a `-H` command and
  `WithBearerToken`, combining a token with API-key headers, a refresh-on-401 pattern that posts
  `WithFormData`, and a reusable `LibCurl` client that sends the token on every request.
- **[02-BearerTokenFromOld.cs](Authentication/02-BearerTokenFromOld.cs)** - Authentication in pasted
  commands: Bearer token, API-key header, an optional `GITHUB_TOKEN` against the GitHub rate-limit
  endpoint, several auth headers at once, and Basic auth with `-u user:pass`.

### File Operations

- **[01-DownloadFile.cs](FileOperations/01-DownloadFile.cs)** - Saving to disk with `curl -o`,
  `Curl.DownloadAsync` and `OutputFiles`, a binary download through the builder (`FollowRedirects`,
  `IsBinary`, `BinaryData`), a byte-range download with `-r`, parallel downloads with
  `Task.WhenAll`, and `SaveToFile` plus a metadata file.
- **[02-DownloadFileFromOld.cs](FileOperations/02-DownloadFileFromOld.cs)** - Saving a text body
  yourself, `-o` with a quoted path, reading binary payloads from `BinaryData`, checking headers
  first with `-I`, and saving and parsing a JSON download.

### Advanced Scenarios

- **[01-ProxyUsage.cs](AdvancedScenarios/01-ProxyUsage.cs)** - HTTP proxies with `-x` and
  `WithProxy(url, user, pass)`, SOCKS5 with `--socks5`, rotating proxies, session-style proxy
  usernames, `--proxy-user` in a command, and `ToCurlCommand()` to see the equivalent command. The
  proxy hosts are placeholders; each failure is caught and printed.

### Real World Examples

- **[01-GitHubApi.cs](RealWorld/01-GitHubApi.cs)** - A GitHub REST client built on
  `CurlRequestBuilder.Request(...)` with shared headers and an optional token: user profile,
  repository list, issue creation (token only), search with an escaped query, rate limit, and a
  zipball download. Responses are parsed into typed models with `ParseJson<T>()`.
- **[02-WebScraping.cs](RealWorld/02-WebScraping.cs)** - Browser-like headers, reading a
  `Set-Cookie` header and sending it back with `WithCookie`, parallel requests limited by a
  `SemaphoreSlim`, a request through a (placeholder) proxy, regex data extraction, and following
  "next" links across pages.

## Quick Start

### Install CurlDotNet

```bash
dotnet add package CurlDotNet
```

### First Request

```csharp
using CurlDotNet;
using CurlDotNet.Core;

// Paste any curl command
var result = await Curl.ExecuteAsync("curl https://api.github.com");
Console.WriteLine(result.Body);

// Or use the fluent builder
var response = await CurlRequestBuilder.Get("https://api.github.com/users/octocat")
    .WithHeader("Accept", "application/json")
    .ExecuteAsync();
```

## Example Highlights

### Direct curl Command Execution

```csharp
// Paste curl commands directly from documentation; trailing backslashes work
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://httpbin.org/post \
      -H 'Content-Type: application/json' \
      -H 'Authorization: Bearer token123' \
      -d '{""name"":""John"",""email"":""john@example.com""}'
");
```

### Fluent Builder

```csharp
var result = await CurlRequestBuilder.Post("https://httpbin.org/post")
    .WithBearerToken("token123")
    .WithJson(new { name = "John", email = "john@example.com" })
    .WithTimeout(TimeSpan.FromSeconds(30))
    .ExecuteAsync();
```

### Proxy Support

```csharp
// HTTP proxy with authentication
var result = await CurlRequestBuilder.Get("https://httpbin.org/ip")
    .WithProxy("http://proxy.company.com:8080", "username", "password")
    .ExecuteAsync();

// SOCKS5 proxy (Tor) - available through the command string
var torResult = await Curl.ExecuteAsync("curl --socks5 127.0.0.1:9050 https://check.torproject.org");
```

### Downloads

```csharp
var download = await Curl.DownloadAsync("https://httpbin.org/bytes/1024", "local-file.bin");
Console.WriteLine($"Saved: {string.Join(", ", download.OutputFiles)}");
```

### Retry Logic

```csharp
// Retries any failure, waiting 1 second between attempts
var settings = new CurlSettings().WithRetries(3, delayMs: 1000);
var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/503", settings);
```

### Cookie Management

```csharp
// Send cookies
var result = await CurlRequestBuilder.Get("https://httpbin.org/cookies")
    .WithCookie("session=abc123; user=john")
    .ExecuteAsync();

// Read a cookie the server sets, to send on later requests
var login = await Curl.ExecuteAsync("curl https://httpbin.org/cookies/set?session=abc123");
var setCookie = login.GetHeader("Set-Cookie"); // "session=abc123; Path=/"
```

## Key Concepts

### The CurlResult Object

Every CurlDotNet call returns a `CurlResult`:

```csharp
var result = await Curl.GetAsync("https://httpbin.org/get");

if (result.IsSuccess)                    // true for 2xx status codes
{
    string body = result.Body;           // response body as text
    int status = result.StatusCode;      // HTTP status code
    var headers = result.Headers;        // case-insensitive header dictionary
    byte[] binary = result.BinaryData;   // set instead of Body for binary responses
    double ms = result.Timings.Total;    // total time in milliseconds
}
```

### Error Handling Patterns

An HTTP error status (404, 500, ...) is a normal result. Transport failures throw.

```csharp
using CurlDotNet.Exceptions;

// Pattern 1: Check IsSuccess
var result = await Curl.GetAsync("https://httpbin.org/status/404");
if (!result.IsSuccess)
{
    Console.WriteLine($"Failed: {result.StatusCode}");
}

// Pattern 2: Turn a bad status into an exception, and catch transport failures
try
{
    var checkedResult = (await Curl.GetAsync("https://httpbin.org/status/404")).EnsureSuccess();
}
catch (CurlDotNet.Core.CurlHttpException ex)   // thrown by EnsureSuccess()
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.Message}");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Timeout after {ex.Timeout?.TotalSeconds}s");
}
catch (CurlException ex)                       // base class for all curl failures
{
    Console.WriteLine($"curl error {ex.CurlErrorCode}: {ex.Message}");
}
```

`CurlDotNet.Core.CurlHttpException` (from `EnsureSuccess()`) and
`CurlDotNet.Exceptions.CurlHttpException` are different types; qualify the name when both
namespaces are imported.

## Useful Links

- **[Full Documentation](https://jacob-mellor.github.io/curl-dot-net/)**
- **[API Reference](https://jacob-mellor.github.io/curl-dot-net/api/)**
- **[NuGet Package](https://www.nuget.org/packages/CurlDotNet/)**
- **[GitHub Repository](https://github.com/jacob-mellor/curl-dot-net)**

## License

All examples are MIT licensed and free to use in your projects.
