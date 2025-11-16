# Curl Command Tutorials with CurlDotNet

This guide mirrors classic curl tutorials (see [curl.se/docs](https://curl.se/docs/)) and shows how each pattern translates into CurlDotNet. Whether you paste raw commands or map them into builders/LibCurl, the goal is to keep the mental model identical to the official curl CLI.

---

## 1. Copy/Paste Workflow Refresher

```csharp
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com/users \
         -H 'Accept: application/json' \
         -H 'Authorization: Bearer TOKEN'
");
result.EnsureSuccess();
```

- Commands may include multiline continuations (`\`) or single-line strings.
- You can omit the `curl` prefix—CurlDotNet will infer it.
- Combine with `Curl.ExecuteManyAsync` for batching multiple commands.

---

## 2. Request Anatomy Cheat Sheet

| curl flag | CurlDotNet notes |
|-----------|------------------|
| `-X, --request` | Always honored. If omitted, CurlDotNet infers `GET` or the most appropriate verb for data payloads. |
| `-H, --header` | Preserved exactly. `LibCurl.WithHeader` sets defaults; inline commands override per request. |
| `-d, --data`, `--data-raw`, `--data-binary` | Stored as request body. Combine with JSON, form-encoded, or raw binary. |
| `-F, --form` | Multipart form data; supports file uploads with `@path`. |
| `-o, -O` | Saves content to disk via `CurlResult.SaveToFile` or `CurlOptions.OutputFile`. |
| `-L, --location` | Sets `FollowRedirects`. Limit with `--max-redirs`. |
| `-u, --user` | Maps to `BasicAuth`. |
| `-k, --insecure` | Sets `Insecure = true`. |
| `--connect-timeout`, `--max-time` | Map to `ConnectTimeout` and `MaxTime`. |
| `-x, --proxy` | Sets `Proxy` and optional credentials. |
| `--compressed` | Requests gzip/deflate/brotli and automatically decompresses. |

---

## 3. Tutorials by Scenario

### 3.1 REST APIs with JSON

```csharp
var createOrder = await Curl.ExecuteAsync(@"
    curl -X POST https://api.shop.com/orders \
         -H 'Authorization: Bearer " + token + @"' \
         -H 'Content-Type: application/json' \
         -d '{""sku"":""ABC-123"",""qty"":2}'
");
```

**What to learn:** JSON payloads, authorization headers, and HTTP verbs follow the same syntax as the CLI tutorials on curl.se.

### 3.2 Form-Encoded Login

```csharp
await Curl.ExecuteAsync(@"
    curl -X POST https://login.example.com/token \
         -d 'grant_type=password' \
         -d 'username=demo' \
         -d 'password=P@ssw0rd!'
");
```

**Mapping:** Equivalent to `curl --data-urlencode`. CurlDotNet automatically URL-encodes when you write the literal string.

### 3.3 Multipart Upload

```csharp
await Curl.ExecuteAsync(@"
    curl -X POST https://api.cloud.com/upload \
         -F 'file=@/data/archive.zip' \
         -F 'environment=staging' \
         --progress-bar
");
```

See curl’s “Everything curl” multipart chapter—CurlDotNet honors the same `@` file semantics and progress flags.

### 3.4 Streaming Downloads

```csharp
var backup = await Curl.ExecuteAsync(
    "curl -o nightly.tar.gz https://storage.example.com/nightly.tar.gz --progress-bar");
backup.EnsureSuccess();
```

After execution, the file lands on disk just like `curl -o`. Use `result.ToStream()` if you want to process the stream yourself.

### 3.5 Authenticated APIs with Refresh Tokens

```csharp
string token = await AcquireTokenAsync();
var response = await Curl.ExecuteAsync($@"
    curl https://graph.microsoft.com/v1.0/me
         -H 'Authorization: Bearer {token}'
         -H 'ConsistencyLevel: eventual'
");
if (response.StatusCode == 401)
{
    token = await RefreshTokenAsync();
    // replay command
}
```

This pattern mirrors tutorials from Microsoft Graph docs. CurlDotNet preserves headers verbatim, so HMAC or signature headers behave the same as in shell scripts.

### 3.6 API Pagination

```csharp
var page1 = await Curl.ExecuteAsync("curl 'https://api.example.com/items?page=1&limit=50'");
var nextUrl = page1.AsJsonDynamic()?._links?.next?.href;
if (nextUrl != null)
{
    var page2 = await Curl.ExecuteAsync($"curl '{nextUrl}'");
}
```

### 3.7 Debugging TLS or Redirects

```csharp
var trace = await Curl.ExecuteAsync("curl -v -L https://redirect.example.com");
Console.WriteLine(trace.DebugOutput); // mirrors curl -v logs
```

### 3.8 FTP / SFTP Transfers

```csharp
await Curl.ExecuteAsync(
    "curl -u ftpuser:ftppass -T backup.zip ftp://ftp.example.com/incoming/backup.zip");
```

CurlDotNet uses the same URI parsing rules as libcurl, so ftp/ftps/sftp examples from the curl book apply directly.

---

## 4. Building Tutorials with LibCurl

Pair the tutorials with the `LibCurl` client when you need reusable defaults:

```csharp
await using var client = new LibCurl()
    .WithHeader("Accept", "application/json")
    .WithBearerToken(token)
    .WithTimeout(TimeSpan.FromSeconds(15));

var orders = await client.GetAsync("https://api.shop.com/orders",
    opts => opts.Headers["X-Region"] = "us-east-1");
```

Each section in this file maps 1:1 to the helpers in `manual/05-LibCurl-Tutorial.md`.

---

## 5. Reference Appendix

- **Official curl tutorial:** https://curl.se/docs/tutorial.html – every switch shown there works the same once pasted into CurlDotNet.
- **Man page:** https://curl.se/docs/manpage.html – keep it handy for quick flag lookups.
- **Real-world samples:** `REAL_WORLD_EXAMPLES.md`, `EXAMPLES.md`, and `examples/` projects in this repo.
- **Advanced middleware:** `docs/ADVANCED.md`.

Capture your own shell snippets, drop them into CurlDotNet, and document the outcome. The closer your guides stay to canonical curl usage, the faster other developers can migrate to .NET.

