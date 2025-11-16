# 05 – LibCurl Playbook

> Stateful, reusable HTTP clients with curl fidelity and .NET ergonomics

The `CurlDotNet.Lib` namespace mirrors libcurl’s “easy handle” but reshaped for C#. You create a `LibCurl` instance, configure defaults once, and issue multiple requests that inherit those defaults. This guide expands on the XML docs inside `LibCurl.cs`, adds field-tested recipes, and highlights when `LibCurl` should be your default choice versus `Curl.ExecuteAsync` or `CurlRequestBuilder`.

---

## 1. Getting Started

```csharp
using CurlDotNet.Lib;

await using var curl = new LibCurl();

curl.WithHeader("Accept", "application/json")
    .WithBearerToken(Environment.GetEnvironmentVariable("API_TOKEN")!)
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithFollowRedirects();

var profile = await curl.GetAsync("https://api.example.com/me");
var posts   = await curl.GetAsync("https://api.example.com/posts");
```

**Why LibCurl?**
- Persistent configuration (headers, auth, timeouts) without rebuilding command strings.
- Thread-safe methods (`HttpClient` + `CurlEngine` are shared internally) so you can reuse one instance across typed clients or dependency injection scopes.
- Fluent helper methods that translate directly to properties on `CurlOptions`.

---

## 2. Configuration Matrix

| Concern | Method | Maps To | Notes |
| --- | --- | --- | --- |
| Default headers | `WithHeader(key, value)` | `CurlOptions.Headers` | Uses `Dictionary`/`TryAdd` depending on target framework. |
| Auth (Bearer) | `WithBearerToken(token)` | `Authorization` header | Automatically prefixes with `Bearer`. |
| Auth (Basic) | `WithBasicAuth(user, pass)` | `CurlOptions.Credentials` | Uses `NetworkCredential`. |
| Timeouts | `WithTimeout`, `WithConnectTimeout` | `MaxTime`, `ConnectTimeout` | Stored as seconds, matches curl’s `--max-time` / `--connect-timeout`. |
| Redirects | `WithFollowRedirects(max)` | `FollowLocation`, `MaxRedirects` | Equivalent to `-L` and `--max-redirs`. |
| SSL relax | `WithInsecureSsl()` | `Insecure` | Mirror of `-k/--insecure`. Use only in test envs. |
| Proxy | `WithProxy(url, user?, pass?)` | `Proxy`, `ProxyCredentials` | Supports HTTP/SOCKS endpoints and credentials. |
| User-Agent | `WithUserAgent(value)` | `UserAgent` | Fallbacks to curl UA if omitted. |
| Output file | `WithOutputFile(path)` | `OutputFile` | Replays `-o` semantics; respects relative/absolute paths. |
| Verbose | `WithVerbose()` | `Verbose` | Equivalent of `-v`. Excellent during diagnostics. |
| Catch-all | `Configure(opts => { ... })` | Whole `CurlOptions` instance | For advanced settings like `CookieJar`, `Retry`, etc. |

All defaults live in a private `_defaultOptions` and `_defaultHeaders`. Before each request, `MergeDefaults` combines them into the per-request `CurlOptions`. Read the source (`src/CurlDotNet/Lib/LibCurl.cs`) to understand precedence rules:

```45:63:src/CurlDotNet/Lib/LibCurl.cs
public LibCurl WithHeader(string key, string value)
{
    _defaultHeaders[key] = value;
    return this;
}
```

---

## 3. Request Walkthroughs

### 3.1 GET with Per-request Overrides

```csharp
var analytics = await curl.GetAsync(
    "https://api.example.com/analytics",
    opts =>
    {
        opts.Headers["X-Trace"] = Activity.Current?.Id ?? Guid.NewGuid().ToString();
        opts.Query ??= new Dictionary<string, string>();
        opts.Query["range"] = "last_7_days";
    });
```

Per-request delegates receive a clone of `CurlOptions` with defaults already merged. You can override headers, supply different bodies, or toggle flags without mutating global defaults.

### 3.2 POST JSON vs Raw Data

`LibCurl.PostAsync` accepts an `object data` parameter. The private `SerializeData` helper uses `System.Text.Json` (or `Newtonsoft.Json` on `NETSTANDARD2_0`). Pass a string to send raw payloads, or an anonymous type to serialize automatically.

```csharp
var created = await curl.PostAsync(
    "https://api.example.com/users",
    new { name = "Cassie", email = "cassie@example.com" });

var form = await curl.PostAsync(
    "https://api.example.com/login",
    "username=jay&password=swordfish",
    opts => opts.Headers["Content-Type"] = "application/x-www-form-urlencoded");
```

### 3.3 File Downloads

```csharp
curl.WithOutputFile("/tmp/daily-report.csv");
var export = await curl.GetAsync("https://reports.example.com/export");
Console.WriteLine($"Report saved to {export.Options.OutputFile}");
```

Setting a default output file is perfect for scheduled jobs. For per-request paths, change `opts.OutputFile` inside the `configure` delegate.

---

## 4. DI & Lifetime Guidance

| Scenario | Recommendation |
| --- | --- |
| ASP.NET Core | Register `LibCurl` as a scoped service if you need request-level defaults, or singleton if you manage headers per call. Disposal cascades to the internal `HttpClient`. |
| Worker services / Azure Functions | Instantiate once at startup and reuse. Disposal occurs during graceful shutdown. |
| Desktop / MAUI apps | Wrap in a `using` block per view model, or register via `IHostedService`. |

Since `LibCurl` implements `IDisposable`, prefer `await using` or DI lifetimes to avoid socket exhaustion.

---

## 5. Concurrency Patterns

- **Thread safety**: `LibCurl` delegates everything to `CurlEngine`, which was built with concurrent usage in mind. You can fire multiple `GetAsync` calls simultaneously from the same instance.
- **Rate limiting**: Combine with middleware (see `docs/ADVANCED.md`) or use `SemaphoreSlim` in your service layer.
- **Retries**: Wrap calls in Polly or custom logic. Example:

```csharp
var policy = Policy
    .Handle<CurlTimeoutException>()
    .OrResult<CurlResult>(r => r.StatusCode >= 500)
    .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(attempt * 2));

var reliable = await policy.ExecuteAsync(
    () => curl.GetAsync("https://api.example.com/health"));
```

---

## 6. Migrating from HttpClient

1. Replace `HttpClientFactory` consumers with `LibCurl` injected services.
2. Move default headers and configuration into `WithHeader`, `WithTimeout`, etc.
3. Convert HttpRequestMessage creation logic into per-call `configure` delegates. Anything in `HttpRequestMessage.Properties` typically maps to `CurlOptions.Metadata`.
4. Remove manual JSON serialization (unless you need custom converters) because `LibCurl` handles objects automatically.

| HttpClient Concept | LibCurl Equivalent |
| --- | --- |
| `HttpClient.DefaultRequestHeaders` | `WithHeader` / `_defaultHeaders` |
| `HttpClient.Timeout` | `WithTimeout` |
| `HttpClientHandler.ServerCertificateCustomValidationCallback` | `WithInsecureSsl()` or custom handler in `CurlEngine` (advanced) |
| `SendAsync(HttpRequestMessage)` | `PerformAsync(CurlOptions)` |

---

## 7. Troubleshooting Checklist

1. **Unexpected headers** – Inspect `CurlResult.Options.Headers` to confirm defaults merged as expected.
2. **Timeouts** – Ensure `WithTimeout` is realistic. Remember `MaxTime` covers the entire transfer, just like curl’s `--max-time`.
3. **Proxy auth** – Use `WithProxy(url, user, pass)` so credentials are stored in `NetworkCredential`; avoid embedding secrets in the URL.
4. **File permissions** – When using `WithOutputFile`, verify the process has write access to the directory—mirrors CLI curl behavior.
5. **Serialization issues** – Pass a string if you need handcrafted payloads, or supply custom options via `Configure(opts => opts.SerializerOptions = ...)` once that property is exposed.

---

## 8. Further Reading

- [LibCurl.cs source](../src/CurlDotNet/Lib/LibCurl.cs) – Read the XML comments and `MergeDefaults` logic.
- [src/CurlDotNet/Core/CurlOptions.cs](../src/CurlDotNet/Core/CurlOptions.cs) – Discover every flag you can tweak inside configuration delegates.
- [docs/ADVANCED.md](../docs/ADVANCED.md) – Compose LibCurl with middleware for logging, retries, caching.
- [curl.se/libcurl](https://curl.se/libcurl/c/libcurl.html) – Original C libcurl patterns; useful when porting legacy code.
- [Stripe, GitHub, and AWS curl tutorials](https://curl.se/docs/httpscripting.html) – Use their CLI samples verbatim inside LibCurl by setting defaults and calling `PerformAsync`.

---

**Next:** Revisit the [manual README](README.md) for other guides, or jump to `docs/ADVANCED.md` to integrate LibCurl with the middleware pipeline.


