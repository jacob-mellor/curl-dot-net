# 07 – Fluent Builder Cookbook

> `CurlRequestBuilder` gives you IntelliSense, compile-time safety, and reusable recipes for every HTTP scenario.

While curl strings are unbeatable for copy/paste workflows, the fluent builder shines when:
- You want strongly typed objects for URLs, headers, and payloads.
- You need to compose requests dynamically (per-tenant headers, derived tokens, etc.).
- You prefer chaining methods with tooltips explaining each option.

This cookbook curates patterns taken from real projects plus references to official curl usage so you can map CLI knowledge to builder calls.

---

## 1. Builder Fundamentals

```csharp
using CurlDotNet.Core;

var result = await CurlRequestBuilder
    .Get("https://api.example.com/users")
    .WithHeader("Accept", "application/json")
    .WithBearerToken("token123")
    .FollowRedirects()
    .ExecuteAsync();
```

Key concepts:
- Every static method (`Get`, `Post`, `Custom`, etc.) seeds a `CurlOptions`.
- Chain modifiers mutate the same options object until `ExecuteAsync()` (or `Build()` if you want to inspect before sending).
- All methods return the builder instance, enabling fluent chaining.

---

## 2. JSON APIs

```csharp
var createdUser = await CurlRequestBuilder
    .Post("https://api.example.com/users")
    .WithJson(new { name = "Aurora", email = "aurora@example.com" })
    .WithHeader("X-Tenant", tenantId)
    .WithTimeout(TimeSpan.FromSeconds(15))
    .ExecuteAsync();
```

Internally, `WithJson` serializes via `System.Text.Json`. For custom converters, call `.WithSerializerOptions(...)` once that property is exposed, or serialize yourself and pass `.WithData(jsonString)`.

---

## 3. Multipart & Files

```csharp
var upload = await CurlRequestBuilder
    .Post("https://api.example.com/uploads")
    .WithFormField("description", "Quarterly numbers")
    .WithFile("file", "/tmp/report.pdf")
    .WithVerbose()
    .ExecuteAsync();
```

Tips:
- Use absolute paths for files to avoid `DirectoryNotFoundException` when running inside containers or CI.
- Combine with `.WithProgressHandler(...)` (coming soon) to emit upload percentages.

---

## 4. Streaming Downloads

```csharp
var download = await CurlRequestBuilder
    .Get("https://cdn.example.com/assets/app.tar.gz")
    .WriteToFile("artifacts/app.tar.gz")
    .WithRetry(maxRetries: 3, delay: TimeSpan.FromSeconds(5))
    .ExecuteAsync();
```

`WriteToFile` mirrors curl’s `-o`. If you omit it, the response body is stored in memory and accessible via `CurlResult.Body` or `CurlResult.ToStream()`.

---

## 5. Authentication Recipes

### 5.1 Basic Auth

```csharp
var result = await CurlRequestBuilder
    .Get("https://api.example.com/private")
    .WithBasicAuth(user: "service", password: secret)
    .ExecuteAsync();
```

### 5.2 Bearer

```csharp
var result = await CurlRequestBuilder
    .Get("https://api.example.com/me")
    .WithBearerToken(tokenProvider.Get())
    .ExecuteAsync();
```

### 5.3 Custom

```csharp
.WithHeader("Authorization", $"Signature {signature}")
.WithHeader("Date", timestamp)
```

---

## 6. Timeouts, Retries, and Resilience

```csharp
var resilient = await CurlRequestBuilder
    .Get("https://edge.example.com/health")
    .WithConnectTimeout(TimeSpan.FromSeconds(5))
    .WithTimeout(TimeSpan.FromSeconds(20))
    .FollowRedirects(maxRedirects: 5)
    .ExecuteAsync();
```

Compose with Polly:

```csharp
var policy = Policy
    .Handle<CurlTimeoutException>()
    .OrResult<CurlResult>(r => r.StatusCode >= 500)
    .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1));

var response = await policy.ExecuteAsync(() =>
    CurlRequestBuilder.Get("https://api.example.com/status").ExecuteAsync());
```

---

## 7. Dynamic Query Parameters

```csharp
var builder = CurlRequestBuilder
    .Get("https://api.example.com/search")
    .WithQuery("q", term)
    .WithQuery("limit", limit.ToString())
    .WithQuery("page", page.ToString());

var result = await builder.ExecuteAsync();
```

Use `.WithQuery(Dictionary<string,string>)` for batch scenarios (e.g., multi-select filters).

---

## 8. Custom HTTP Methods & Raw Options

```csharp
var purge = await CurlRequestBuilder
    .Custom("PURGE", "https://cdn.example.com/cache")
    .WithHeader("X-Invalidate", resourceId)
    .ExecuteAsync();
```

Need an option that is not exposed yet? Fall back to `.Configure(opts => opts.SomeProperty = value);`

```csharp
.Configure(opts =>
{
    opts.SocksProxy = "socks5://proxy.example.com:1080";
    opts.Cert = "/etc/certs/client.pfx";
})
```

---

## 9. Working with Responses

```csharp
var result = await CurlRequestBuilder
    .Get("https://api.example.com/users/42")
    .ExecuteAsync();

result.EnsureSuccess();
var user = result.ParseJson<User>();

if (result.HasHeader("Etag"))
{
    cache.Store(result.GetHeader("Etag"), user);
}
```

Response helpers mirror those documented in the root `README.md`. Combine them with builder-specific metadata (e.g., `result.Options`).

---

## 10. Testing with the Builder

```csharp
[Fact]
public async Task Builder_Uses_Defaults()
{
    var result = await CurlRequestBuilder
        .Get("https://httpbin.org/headers")
        .WithHeader("X-Test", "true")
        .ExecuteAsync();

    result.EnsureSuccess();
    var json = result.AsJsonDynamic();
    Assert.Equal("true", (string)json.headers["X-Test"]);
}
```

Pair with `FakeHttpMessageHandler` or `CurlEngine` middleware when you want deterministic responses in unit tests.

---

## 11. Migration Tips

| From | To |
| --- | --- |
| Raw curl strings with environment substitution | Builder with `.WithHeader/WithQuery` so you can inject values programmatically |
| `HttpRequestMessage` pipelines | Wrap logic in builder extension methods (`public static CurlRequestBuilder WithTenant(this CurlRequestBuilder builder, string tenant)`), then reuse across services |
| RestSharp/Refit | Use builder for custom requests that RestSharp struggles with (e.g., streaming, multi-part plus proxies) |

Pro tip: author extension methods to encode business rules.

```csharp
public static class CurlBuilderExtensions
{
    public static CurlRequestBuilder WithTenantHeaders(this CurlRequestBuilder builder, TenantContext context)
        => builder.WithHeader("X-Tenant", context.Id)
                 .WithHeader("X-Tenant-Signature", context.Signature);
}
```

---

## 12. External Inspiration

- [curl Man Page](https://curl.se/docs/manpage.html) – every option has a builder equivalent; consult when adding new helpers.
- [.NET REST guidelines](https://learn.microsoft.com/dotnet/csharp/tutorials/console-webapiclient) – combine with builder for best practices on error handling and resiliency.
- [GitHub API examples](https://docs.github.com/en/rest) – convert their curl samples into builder chains for typed clients.

---

**Next:** Tie everything together with middleware, retries, and advanced handlers in [docs/ADVANCED.md](../docs/ADVANCED.md), or jump back to the [Manual README](README.md) for more guides.*** End Patch to=functions.apply_patch

