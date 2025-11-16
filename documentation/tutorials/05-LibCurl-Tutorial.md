# LibCurl Tutorial – Stateful HTTP Client for CurlDotNet

LibCurl (`CurlDotNet.Lib.LibCurl`) is the object-oriented facade that mirrors libcurl’s “easy” API. It wraps `CurlEngine` and `HttpClient` so you can keep reusable configuration, share headers, and execute multiple requests with the same defaults—no need to reassemble curl strings each time.

> **When to Reach for LibCurl**
>
> - You are building a service/client layer that must hit the same API many times.
> - You want per-request overrides without rebuilding every option from scratch.
> - You need to inject CurlDotNet into ASP.NET DI, background workers, or MAUI apps as a reusable singleton.

---

## 1. Quick Start

```csharp
using CurlDotNet.Lib;

await using var curl = new LibCurl()
    .WithHeader("Accept", "application/json")
    .WithUserAgent("MyProduct/2.3")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithFollowRedirects();

var user = await curl.GetAsync("https://api.example.com/users/42");
user.EnsureSuccess();
Console.WriteLine(user.Body);
```

- `LibCurl` implements `IDisposable`. Use `await using` (C# 8+) or a `using` statement to dispose of the underlying `HttpClient` and `CurlEngine`.
- All HTTP verbs are exposed (`GetAsync`, `PostAsync`, `PutAsync`, `PatchAsync`, `DeleteAsync`, `HeadAsync`), plus `PerformAsync` for full `CurlOptions` control.
- The class is thread-safe. You can share a single instance across multiple concurrent requests (see section 6).

---

## 2. Default Configuration Pipeline

LibCurl merges defaults into every outgoing `CurlOptions`. Set them once with the fluent helpers:

```csharp
await using var curl = new LibCurl()
    .WithHeader("Accept", "application/json")
    .WithBearerToken(tokenProvider.Current)
    .WithConnectTimeout(TimeSpan.FromSeconds(5))
    .WithTimeout(TimeSpan.FromSeconds(20))
    .WithProxy("http://proxy.internal:8080", "svc-user", "svc-pass")
    .WithUserAgent("Contoso.Inventory/5.1")
    .WithFollowRedirects(maxRedirects: 10)
    .WithOutputFile("responses/latest.json"); // optional file output
```

Behind the scenes `MergeDefaults` copies:
- Headers (`_defaultHeaders`)
- Credentials, tokens, proxy settings, max timeouts, redirect policy, output file, and verbosity flags stored in `_defaultOptions`.

> **Tip:** You can drop down to raw defaults with `curl.Configure(opts => { ... })` to set rarely-used properties such as `SpeedLimit`, `CookieJar`, or `Retry`.

---

## 3. Sending Data (JSON, Form, Raw, Binary)

`LibCurl` auto-serializes objects to JSON using `System.Text.Json` (or Newtonsoft.Json on .NET Standard 2.0). Strings pass through unchanged.

```csharp
var create = await curl.PostAsync(
    "https://api.example.com/users",
    new { name = "Ada Lovelace", email = "ada@example.com" },
    opts => opts.Headers["Content-Type"] = "application/json");

var form = await curl.PostAsync(
    "https://api.example.com/login",
    "username=ada&password=analytical",
    opts => opts.Headers["Content-Type"] = "application/x-www-form-urlencoded");

var patch = await curl.PatchAsync(
    "https://api.example.com/files/abc",
    File.ReadAllText("payload.json"));
```

For multipart uploads (file attachments) you can construct the payload manually or call `PerformAsync` with a pre-built `CurlOptions` that includes `FormFields` / `Files`.

---

## 4. Per-Request Overrides

Each verb accepts an optional `Action<CurlOptions> configure`. Use it to override headers, timeouts, or destinations without mutating the shared defaults.

```csharp
var invoice = await curl.GetAsync(
    "https://api.example.com/invoices/987",
    opts =>
    {
        opts.Headers["X-Tenant-Id"] = tenantId;
        opts.MaxTime = 10;
        opts.OutputFile = $"logs/invoice-{tenantId}.json";
    });
```

Key override scenarios:
- Temporarily enable verbose tracing (`opts.Verbose = true`)
- Switch to a different proxy or disable it
- Override request method (`opts.Method = "OPTIONS"`) when using `PerformAsync`
- Provide custom `CurlOptions.DataStream` for streaming uploads

---

## 5. Request Method Reference

| Method | Use Case | Notes |
|--------|----------|-------|
| `GetAsync` | Fetch resources, search endpoints | Sets `Method = "GET"` and `HeadOnly = false`. |
| `PostAsync` | Create resources, submit forms | Serializes `data` parameter via `SerializeData`. |
| `PutAsync` | Replace resources | Same serialization behavior as POST. |
| `PatchAsync` | Partial updates | Ideal for JSON merge patch payloads. |
| `DeleteAsync` | Remove resources | No body by default; override via `configure`. |
| `HeadAsync` | Metadata checks, health probes | Sets `HeadOnly = true` to avoid downloading bodies. |
| `PerformAsync` | Fully custom call | Supply a pre-populated `CurlOptions` (custom verbs, FTP, etc.). |

---

## 6. Dependency Injection & Concurrency

LibCurl is thread-safe because it:
- Uses a single `HttpClient` instance (`_httpClient`) that benefits from connection pooling.
- Synchronizes access to `_defaultHeaders` via normal dictionary semantics (call configuration during startup).
- Merges defaults into new `CurlOptions` objects per request, so there is no shared mutable options instance.

### ASP.NET Core registration

```csharp
builder.Services.AddSingleton(provider =>
{
    var curl = new LibCurl()
        .WithUserAgent("InventoryApi/1.0")
        .WithTimeout(TimeSpan.FromSeconds(30))
        .WithFollowRedirects();

    return curl;
});
```

When using dependency injection, register as singleton or scoped (never transient) to reuse connections. Remember to dispose it gracefully on application shutdown (`IHostApplicationLifetime.ApplicationStopping`).

---

## 7. Advanced Patterns

### 7.1 Batching Multiple Calls

```csharp
async Task<CurlResult[]> FetchDashboardAsync(LibCurl curl)
{
    var userTask = curl.GetAsync("https://api.example.com/me");
    var alertsTask = curl.GetAsync("https://api.example.com/alerts");
    var reportsTask = curl.GetAsync("https://api.example.com/reports?limit=5");

    return await Task.WhenAll(userTask, alertsTask, reportsTask);
}
```

### 7.2 Uploading Files with Streams

```csharp
await curl.PerformAsync(new CurlOptions
{
    Url = "https://upload.example.com/archive",
    Method = "PUT",
    DataStream = File.OpenRead("artifacts.zip"),
    Headers = { ["Content-Type"] = "application/zip" }
});
```

### 7.3 Mixing LibCurl with Curl Strings

Use LibCurl for shared configuration and fall back to raw commands for specialized cases:

```csharp
var defaults = await curl.PerformAsync(new CurlOptions
{
    Url = "https://api.example.com/profile",
    Method = "GET",
    Headers = { ["Authorization"] = curlToken }
});

var adhoc = await Curl.ExecuteAsync(
    "curl -X POST https://api.example.com/reports -d '{\"range\":\"30d\"}'",
    defaults.ToCurlSettings());
```

---

## 8. Real-World Recipes

### Multi-Tenant API Client

```csharp
public class TenantAwareClient
{
    private readonly LibCurl _curl;

    public TenantAwareClient(LibCurl curl) => _curl = curl;

    public Task<CurlResult> GetForTenantAsync(string tenantId, string resource) =>
        _curl.GetAsync(
            $"https://api.example.com/{resource}",
            opts => opts.Headers["X-Tenant-Id"] = tenantId);
}
```

### Background Job Runner (e.g., Hangfire)

```csharp
public class ReportJob
{
    private readonly LibCurl _curl;

    public ReportJob(LibCurl curl) => _curl = curl;

    public async Task RunAsync(Guid reportId, CancellationToken token)
    {
        var result = await _curl.PostAsync(
            "https://reports.example.com/run",
            new { reportId },
            opts => opts.MaxTime = 120,
            token);

        result.EnsureSuccess();
        await File.WriteAllTextAsync($"reports/{reportId}.json", result.Body, token);
    }
}
```

### Observability (Verbose + Output Files)

```csharp
await using var curl = new LibCurl()
    .WithVerbose()
    .WithOutputFile("logs/latest-response.txt");

var response = await curl.GetAsync("https://status.example.com/health");
Console.WriteLine(File.ReadAllText("logs/latest-response.txt"));
```

---

## 9. Troubleshooting & Best Practices

- **Disposal:** Always dispose LibCurl to close sockets. In ASP.NET Core, tie disposal to host lifetime.
- **Timeouts:** Set both `WithTimeout` (max time) and `WithConnectTimeout` to avoid hung sockets.
- **Authentication:** Prefer `WithBearerToken` or `WithBasicAuth` once during startup; override per request if tenants need unique tokens.
- **Insecure SSL:** `WithInsecureSsl()` mirrors `curl -k`. Limit usage to development or controlled environments.
- **Proxies:** Use `WithProxy` for corporate networks. For SOCKS proxies, configure `CurlOptions.ProxyType` in `Configure`.
- **Streaming:** When downloading large files, set `opts.OutputFile` or consume `CurlResult.ToStream()` to avoid memory pressure.
- **Thread safety:** Configure defaults before running parallel requests. After configuration, concurrent reads are safe.
- **Error handling:** Catch specific exceptions (e.g., `CurlTimeoutException`) or call `result.EnsureSuccess()` to throw automatically on HTTP >= 400.

---

## 10. Additional References

- `src/CurlDotNet/Lib/LibCurl.cs` contains the authoritative implementation with XML documentation.
- Official curl tutorials at [curl.se/docs](https://curl.se/docs/) explain every switch; LibCurl exposes the same capabilities via `CurlOptions`.
- See `manual/04-Compatibility-Matrix.md` for runtime support and `docs/ADVANCED.md` for middleware and handler customization.

Use this tutorial as the starting point for building reusable, enterprise-grade HTTP clients on top of CurlDotNet’s LibCurl surface area. Happy curling! 🚀

