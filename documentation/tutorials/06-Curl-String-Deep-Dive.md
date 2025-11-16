# 06 – Curl String Deep Dive

> Copy/paste curl commands into .NET with confidence, parity, and production-ready guardrails.

The string API (`Curl.ExecuteAsync(string command, CurlSettings? settings = null)`) exists for one reason: *every* API reference on the internet documents curl first. This guide shows how to use those samples verbatim, harden them for production, and debug differences between CurlDotNet and the native curl CLI.

---

## 1. Anatomy of a Curl Command

```bash
curl -X POST \
     -H "Accept: application/json" \
     -H "Authorization: Bearer $TOKEN" \
     -d '{"name":"Ada","email":"ada@example.com"}' \
     https://api.example.com/users
```

When passed to `Curl.ExecuteAsync`, the parser (`CommandParser.cs`) tokenizes the string exactly like curl’s own parsing logic:

1. **Verb** (`-X POST`) → `CurlOptions.Method`
2. **Headers** (`-H`) → `CurlOptions.Headers`
3. **Body** (`-d/--data`) → `CurlOptions.Data`
4. **URL** → `CurlOptions.Url`
5. **Switches** (e.g., `-L`, `-k`, `--retry`) → corresponding boolean/int fields on `CurlOptions`

You can omit the literal word `curl` or keep it—both work:

```csharp
var result = await Curl.ExecuteAsync("curl https://ifconfig.me");
var same   = await Curl.ExecuteAsync("https://ifconfig.me");
```

---

## 2. Quick Start Template

```csharp
using CurlDotNet;

var command = @"
curl -X GET \
     -H 'Accept: application/json' \
     -H 'X-API-Key: {API_KEY}' \
     'https://api.example.com/resources?page=1&limit=50'
";

var result = await Curl.ExecuteAsync(
    command.Replace("{API_KEY}", Environment.GetEnvironmentVariable("API_KEY") ?? "")
);

result.EnsureSuccess();
Console.WriteLine(result.Body);
```

**Best practices**
- Use verbatim strings (`@"..."`) or indent-friendly raw strings (`"""`) in C# 11+.
- Store secrets in environment variables or secret vaults, not inside the literal command.
- Call `EnsureSuccess()` so HTTP ≥400 status codes throw `CurlHttpReturnedErrorException`.

---

## 3. Translating Docs to Production

| Source Command | .NET-safe Modification | Reason |
| --- | --- | --- |
| `curl -u user:pass https://api.example.com` | Replace with `-u user:$API_PASS` or `-H 'Authorization: Basic ...'` | Avoid storing passwords in source control. |
| `curl -k https://localhost` | Use only in dev, gate via feature flag | `-k/--insecure` skips SSL validation. |
| `curl -d @payload.json` | `@"curl -d @payload.json https://...` plus `settings.WorkingDirectory` | File paths are relative to working dir; set it explicitly. |
| `curl --compressed` | Works as-is; add `.EnsureContains("gzip")` for validation | Confirms server sent compressed payload. |

**Tip:** When commands reference shell substitution (`$TOKEN`, `$(cat file)`), replace them with .NET interpolations or load the file via `File.ReadAllText`.

---

## 4. Handling Complex Bodies

### 4.1 JSON

```csharp
var payload = JsonSerializer.Serialize(new { name = "Leona", email = "leona@example.com" });
var command = $@"curl -X POST -H 'Content-Type: application/json' -d '{payload}' https://api.example.com/users";
```

### 4.2 Multipart

```csharp
var upload = await Curl.ExecuteAsync(@"
curl -X POST \
     -F 'file=@/tmp/report.pdf' \
     -F 'description=Q4 forecast' \
     https://api.example.com/uploads
");
```

`Curl.ExecuteAsync` streams files just like the CLI. Ensure the process has access to the file path (absolute paths recommended inside containers).

### 4.3 Binary

```csharp
var result = await Curl.ExecuteAsync(@"
curl --data-binary '@/tmp/archive.tar.gz' \
     -H 'Content-Type: application/octet-stream' \
     https://api.example.com/import
");
```

---

## 5. Debugging Parity Issues

1. **Capture verbose output**
   ```csharp
   var result = await Curl.ExecuteAsync("curl -v https://api.example.com");
   Console.WriteLine(result.DebugLog);
   ```
   `DebugLog` mirrors curl’s `-v` stream.

2. **Run “real” curl as control**
   ```bash
   curl -v https://api.example.com > cli.log
   ```
   Compare headers, TLS negotiation, and payloads.

3. **Use CommandLineComparison tests**
   ```bash
   dotnet test --filter "Category=CommandLineComparison"
   ```

4. **Inspect `CurlOptions`**
   ```csharp
   var options = result.Options; // Only available if you call PerformAsync; for strings, set `settings.CaptureOptions = true`.
   ```

Common gotchas:
- **Shell quoting vs C# quoting** – escape sequences like `\"` in JSON need doubling.
- **Line continuations** – Use backslash (`\`) in verbatim strings, or prefer triple-quoted raw strings.
- **Environment variables** – CLI curl expands `$VAR`; C# doesn’t. Replace manually.

---

## 6. Security Checklist

- Strip tokens before logging: `command.Replace(Environment.GetEnvironmentVariable("TOKEN"), "***")`
- Gate `-k/--insecure` usage via `if (!Environment.IsDevelopment())`
- Prefer `--header "Authorization: Bearer $(vault read ...)"` pattern replaced with `WithBearerToken` when migrating to LibCurl.
- Validate downloaded files with checksums:

```csharp
var result = await Curl.ExecuteAsync("curl -o backup.zip https://cdn.example.com/backup.zip");
result.EnsureSuccess();
var checksum = SHA256.HashData(await File.ReadAllBytesAsync("backup.zip"));
```

---

## 7. Advanced Settings

`Curl.ExecuteAsync` accepts optional `CurlSettings` for global overrides:

```csharp
var settings = new CurlSettings
{
    FollowRedirects = true,
    MaxRedirects = 10,
    Timeout = TimeSpan.FromSeconds(60),
    WorkingDirectory = "/app/data",
    Verbose = true
};

var result = await Curl.ExecuteAsync("curl -L https://short.url/resource", settings);
```

Use `Curl.ConfigureDefaults(...)` to apply organization-wide policies (user agent, proxy, logging hooks).

---

## 8. Field Recipes

- **CLI to integration test**
  ```csharp
  [Fact]
  public async Task Stripe_CreatePaymentIntent()
  {
      var cmd = @"
      curl -X POST https://api.stripe.com/v1/payment_intents \
           -u sk_test_123: \
           -d amount=1099 \
           -d currency=usd \
           -d 'payment_method_types[]'='card'";

      var result = await Curl.ExecuteAsync(cmd.Replace("sk_test_123", _config.StripeKey));
      result.EnsureSuccess();
  }
  ```

- **Scheduled downloads with curl flags**
  ```csharp
  var backup = await Curl.ExecuteAsync(
      "curl -L --retry 5 --retry-delay 2 -o nightly.tar.gz https://bucket.example.com/backups/nightly.tar.gz");
  backup.EnsureSuccess();
  ```

- **HTTP debugging terminal**
  ```csharp
  var cliInput = Console.ReadLine();
  if (!string.IsNullOrWhiteSpace(cliInput))
  {
      var response = await Curl.ExecuteAsync("curl " + cliInput);
      Console.WriteLine(response.Body);
  }
  ```

---

## 9. External References

- [curl Tutorial](https://curl.se/docs/tutorial.html) – official guide to curl options (every flag has an equivalent in CurlDotNet).
- [curl HTTPS scripting guide](https://curl.se/docs/httpscripting.html) – best practices for auth, cookies, and security; mirror them in your .NET commands.
- [Stripe, GitHub, AWS docs](https://github.com/public-apis/public-apis) – copy their curl samples directly; this guide ensures safe translation.

---

**Next:** For reusable clients or DI-friendly services, move to the [LibCurl Playbook](05-LibCurl-Playbook.md). For type-safe construction with IntelliSense, continue with the [Fluent Builder Cookbook](07-Fluent-Builder-Cookbook.md).*** End Patch*** End Patch to=functions.apply_patch

