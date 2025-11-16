# LibCurl API

`CurlDotNet.Lib` packages the “libcurl style” API: you instantiate `LibCurl`, set defaults once, and reuse it across multiple requests. Think of it as a typed, reusable client that still speaks curl.

## Why This Namespace Exists

- **Stateful configuration** – Headers, auth, proxy settings, and timeouts live on the instance.
- **Thread-safe execution** – Under the hood, `CurlEngine` + `HttpClient` are shared, so you can fire concurrent calls.
- **Parity with curl options** – Every helper (`WithTimeout`, `WithInsecureSsl`, etc.) maps directly to a curl flag.

## LibCurl Class Highlights

### HTTP verbs
- `Task<CurlResult> GetAsync(string url, Action<CurlOptions>? configure = null, CancellationToken token = default)`
- `PostAsync`, `PutAsync`, `PatchAsync`, `DeleteAsync`, `HeadAsync`, and `PerformAsync` (fully custom `CurlOptions`)

### Fluent defaults
- `WithHeader`, `WithBearerToken`, `WithBasicAuth`
- `WithTimeout`, `WithConnectTimeout`, `WithFollowRedirects`, `WithInsecureSsl`
- `WithProxy`, `WithUserAgent`, `WithOutputFile`, `WithVerbose`
- `Configure(Action<CurlOptions>)` for anything not covered by helpers

Defaults are stored in `_defaultHeaders` and `_defaultOptions`. Each request clones those defaults via `MergeDefaults`, so per-call overrides never mutate the global state.

## Usage Patterns

```csharp
using CurlDotNet.Lib;

await using var curl = new LibCurl();

curl.WithHeader("Accept", "application/json")
    .WithBearerToken("token123")
    .WithTimeout(TimeSpan.FromSeconds(20))
    .WithFollowRedirects();

var users = await curl.GetAsync("https://api.example.com/users");

var report = await curl.PostAsync(
    "https://api.example.com/reports",
    new { range = "last_30_days" },
    opts => opts.Headers["X-Trace"] = Activity.Current?.Id ?? Guid.NewGuid().ToString());
```

### Downloading to Disk

```csharp
curl.WithOutputFile("reports/daily.json");
var export = await curl.GetAsync("https://reports.example.com/daily");
Console.WriteLine($"Saved to {export.Options.OutputFile}");
```

### Custom `CurlOptions`

```csharp
var options = new CurlOptions
{
    Url = "https://idp.example.com/token",
    Method = "POST",
    Data = "grant_type=client_credentials",
    Headers = { ["Content-Type"] = "application/x-www-form-urlencoded" }
};

var result = await curl.PerformAsync(options);
```

## When to Choose LibCurl

- You need a reusable service client or DI-friendly abstraction.
- Multiple endpoints share the same auth/headers and you don’t want to repeat yourself.
- You are porting code from libcurl (C) or HttpClient factories and want equivalent lifecycle semantics.

Use `Curl.ExecuteAsync` when you literally have a curl command string. Use `CurlRequestBuilder` when you need compile-time guidance but not necessarily a shared client.

## Further Reading

- [manual/05-LibCurl-Playbook.md](../../../manual/05-LibCurl-Playbook.md) – step-by-step tutorials, migration tips, and troubleshooting.
- [CurlOptions](../Core/CurlOptions.cs) – the object passed to `PerformAsync`.
- [docs/ADVANCED.md](../../../docs/ADVANCED.md) – integrate LibCurl with middleware, retries, and protocol handlers.

