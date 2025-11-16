# 05 - LibCurl Deep Dive & Tutorials

The `CurlDotNet.Lib.LibCurl` class (`src/CurlDotNet/Lib/LibCurl.cs`) mirrors the ergonomics of libcurl’s easy interface while staying 100% managed. This chapter turns the XML docs in `LibCurl.cs` into a narrative guide that explains **when** to reach for LibCurl, **how** it composes with the rest of CurlDotNet, and **what** production patterns it unlocks.

---

## 1. When to Choose LibCurl

| Scenario | Why LibCurl Helps | Key API |
| --- | --- | --- |
| Microservice SDKs with shared defaults | Reuse auth headers, timeouts, proxies across dozens of calls | `WithHeader`, `WithBearerToken`, `WithTimeout` |
| Background workers and cron jobs | Maintain one `HttpClient` + connection pool per worker | Constructor + `Dispose()` |
| UI apps needing cancelation | Forward `CancellationToken`s directly to `GetAsync`/`PostAsync` etc. | `cancellationToken` parameter |
| Integration tests that tweak options per case | Establish baseline config, override with `opts => …` lambdas | `configure` parameter on every verb |
| Multi-tenant gateways | Clone clients per tenant, swap default headers, proxies, credentials | Fluent `With*` methods |

If you would normally new-up `HttpClient`, add default headers, and wrap it in your own class, LibCurl already does that—but with full curl option parity and the CurlEngine under the hood.

---

## 2. Lifecycle & Thread Safety

```csharp
using CurlDotNet.Lib;

public sealed class BillingClient : IDisposable
{
    private readonly LibCurl _curl = new LibCurl()
        .WithUserAgent("BillingService/2.3")
        .WithTimeout(TimeSpan.FromSeconds(20))
        .WithFollowRedirects()
        .WithHeader("Accept", "application/json");

    public Task<CurlResult> GetInvoiceAsync(string id, CancellationToken ct) =>
        _curl.GetAsync($"https://api.example.com/invoices/{id}", cancellationToken: ct);

    public void Dispose() => _curl.Dispose();
}
```

- `LibCurl` encapsulates a shared `HttpClient` and `CurlEngine`; dispose exactly once per logical lifetime.
- All public methods are thread-safe—use a singleton per service or register it as a scoped/transient dependency depending on your app design.
- `MergeDefaults` (see `LibCurl.cs`) copies fluent defaults into each `CurlOptions` instance immediately before execution, ensuring per-request overrides stay isolated.

---

## 3. Fluent Defaults vs. Per-Request Overrides

1. **Configure defaults once** via fluent methods such as `WithHeader`, `WithBasicAuth`, `WithProxy`, `WithInsecureSsl`, etc.
2. Call any HTTP verb method.
3. Supply an optional `Action<CurlOptions>` to tweak that individual request.

```csharp
var client = new LibCurl()
    .WithBearerToken(tokenProvider.Current)
    .WithConnectTimeout(TimeSpan.FromSeconds(5))
    .WithProxy("http://proxy.internal:8080", "svc", "secret");

var catalog = await client.GetAsync("https://api.internal/catalog",
    opts => opts.Headers["X-Tenant"] = tenantId);

var import = await client.PostAsync("https://api.internal/import",
    data: new { tenantId, files },
    configure: opts =>
    {
        opts.MaxTime = 120;
        opts.Verbose = true; // temporary logging
    });
```

Behind the scenes:
- Default headers merge via `Dictionary` or `TryAdd` depending on target framework.
- Options only overwrite when the per-request object leaves a field unset (see the guard clauses in `MergeDefaults`).

---

## 4. Feature Matrix

| Fluent Call | Maps To `CurlOptions` | Notes |
| --- | --- | --- |
| `WithHeader(key, value)` | `Headers[key] = value` | Honors multi-target differences (`TryAdd` on modern runtimes). |
| `WithBearerToken(token)` | `Authorization: Bearer {token}` | Convenience wrapper over `WithHeader`. |
| `WithBasicAuth(user, pass)` | `Credentials = NetworkCredential` | Applies to HTTP & proxy auth if no explicit proxy creds. |
| `WithTimeout(TimeSpan)` | `MaxTime` (seconds) | Equivalent to `--max-time` in curl. |
| `WithConnectTimeout(TimeSpan)` | `ConnectTimeout` | Equivalent to `--connect-timeout`. |
| `WithFollowRedirects(int)` | `FollowLocation`, `MaxRedirects` | Mirrors `-L` and `--max-redirs`. |
| `WithInsecureSsl()` | `Insecure = true` | Only for diagnostics; log when enabling. |
| `WithProxy(url, user, pass)` | `Proxy`, `ProxyCredentials` | Accepts HTTP, HTTPS, SOCKS proxies. |
| `WithUserAgent(string)` | `UserAgent` | Default mimics curl but override for app identity. |
| `WithOutputFile(path)` | `OutputFile` | Server responses stream to disk automatically. |
| `WithVerbose()` | `Verbose = true` | Equivalent to `-v`; outputs wire trace to logger/console. |
| `Configure(Action<CurlOptions>)` | Arbitrary mutation | Use for rarely toggled options (HTTP/2, rate limits, etc.). |

Refer back to `LibCurl.cs` when adding new fluent methods; the XML `<example>` tags there must stay aligned with this manual.

---

## 5. Tutorials

### 5.1 Multi-Request Workflow

```csharp
var crm = new LibCurl()
    .WithHeader("Accept", "application/json")
    .WithUserAgent("SalesPortal/5.4")
    .WithTimeout(TimeSpan.FromSeconds(15));

var leads = await crm.GetAsync("https://crm.local/api/leads?status=new");
var enrichment = await crm.PostAsync("https://crm.local/api/enrich",
    data: leads.AsJsonDynamic().records,
    configure: opts => opts.Headers["X-Priority"] = "bulk");
await crm.DeleteAsync("https://crm.local/api/cache",
    opts => opts.Headers["X-Cache-Scope"] = "leads");
```

### 5.2 Streaming Downloads with Safe Disposal

```csharp
await using var file = File.Create("export.ndjson");
var result = await client.GetAsync("https://api.internal/export");
await using var stream = result.ToStream();
await stream.CopyToAsync(file, cancellationToken);
```

Because `CurlResult` exposes stream helpers, you can keep LibCurl focused on transport while downstream services manage persistence.

### 5.3 Parallel Fan-Out with Cancellation

```csharp
var reports = new[] { "alpha", "beta", "gamma" };
await Parallel.ForEachAsync(reports, cancellationToken, async (report, ct) =>
{
    await client.GetAsync($"https://api/report/{report}", cancellationToken: ct);
});
```

`LibCurl`’s thread-safe design means you do not need a separate instance per task. The underlying `HttpClient` handles connection pooling.

---

## 6. Troubleshooting & Observability

- **Verbose tracing**: Call `.WithVerbose()` during development to mirror `curl -v`. On request-level debugging use `configure: opts => opts.Verbose = true`.
- **Timeouts**: Distinguish between `WithTimeout` (entire transfer) and `WithConnectTimeout` (socket handshake). Combine both for deterministic retries.
- **Proxy issues**: Confirm `WithProxy` credentials vs. `WithBasicAuth`. Each has its own `NetworkCredential`.
- **Disposal**: Register `LibCurl` with dependency injection as `Singleton` or `Scoped` and allow the container to dispose it automatically.
- **Unit testing**: Inject a mock `CurlEngine` or wrap LibCurl behind your own interface; capture `CurlOptions` in the `configure` callback to assert behavior.

---

## 7. Next Steps

- Pair this chapter with `docs/ADVANCED.md` for middleware and handler extensibility.
- For fluent builder or raw string usage, see the sibling tutorials that follow in this manual (`06 - Curl String Playbook`, `07 - Fluent Builder Recipes`, etc.).

> **Keep Docs in Sync**  
> Whenever you add a new fluent method to `LibCurl`, update:
> 1. XML docs inside `LibCurl.cs`
> 2. This manual chapter’s feature matrix
> 3. Any related samples in `examples/`


