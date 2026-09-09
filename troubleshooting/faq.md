# Frequently Asked Questions

Answers to common questions about CurlDotNet.

## General Questions

### What is CurlDotNet?

CurlDotNet is a pure .NET implementation of curl that allows you to execute curl commands directly in your C# applications. Instead of translating curl commands to HttpClient code, you can paste curl commands as strings and execute them directly.

### Why use CurlDotNet instead of HttpClient?

| Feature | CurlDotNet | HttpClient |
|---------|------------|------------|
| Copy/paste curl commands | Yes | No |
| Learning curve | None (if you know curl) | Moderate |
| Migration from bash | Easy | Manual translation |
| All curl options | 300+ supported | Limited |
| Code generation | Built-in | External tools |

CurlDotNet is ideal when:
- You have curl commands from API documentation
- You're migrating from bash scripts
- You want quick prototyping
- You need curl-specific features

HttpClient is better when:
- You want maximum performance
- You need fine-grained control
- You're building a library

### Does CurlDotNet use the native curl library?

No. CurlDotNet is a **pure .NET implementation**. It doesn't use P/Invoke or native binaries. This means:
- No native dependencies to install
- Works everywhere .NET runs
- Single NuGet package deployment

### What platforms are supported?

CurlDotNet supports:
- .NET 10.0 (Preview)
- .NET 9.0
- .NET 8.0 (LTS - Recommended)
- .NET 6.0 (LTS)
- .NET Framework 4.7.2+
- .NET Standard 2.0
- Xamarin, Unity, Blazor, MAUI

---

## Technical Questions

### Do I need to include "curl" in the command string?

No, the "curl" prefix is optional:

```csharp
// Both work identically:
await Curl.ExecuteAsync("curl https://api.example.com");
await Curl.ExecuteAsync("https://api.example.com");
```

### How do I handle authentication?

CurlDotNet supports various authentication methods:

```csharp
// Basic auth
await Curl.ExecuteAsync("curl -u username:password https://api.example.com");

// Bearer token
await Curl.ExecuteAsync("curl -H 'Authorization: Bearer token123' https://api.example.com");

// API key
await Curl.ExecuteAsync("curl -H 'X-API-Key: mykey' https://api.example.com");
```

### How do I send JSON data?

```csharp
// Using PostJson (easiest)
var result = await Curl.PostJsonAsync("https://api.example.com", new { name = "John" });

// Using curl command
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com \
      -H 'Content-Type: application/json' \
      -d '{""name"": ""John""}'
");
```

### How do I download files?

```csharp
// Using Download method
await Curl.DownloadAsync("https://example.com/file.pdf", "output.pdf");

// Using curl command
await Curl.ExecuteAsync("curl -o output.pdf https://example.com/file.pdf");
```

### Is CurlDotNet thread-safe?

Yes, all methods are thread-safe. You can call them from multiple threads simultaneously.

### How do I handle errors?

Check `IsSuccess` or catch specific exceptions:

```csharp
// Method 1: Check IsSuccess
var result = await Curl.ExecuteAsync("curl https://api.example.com");
if (result.IsSuccess)
{
    Console.WriteLine(result.Body);
}
else
{
    Console.WriteLine($"Error: {result.StatusCode}");
}

// Method 2: Catch exceptions
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Timed out after {ex.Timeout}s");
}
catch (CurlHttpException ex)
{
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.ResponseBody}");
}
```

### How do I set a timeout?

```csharp
// Global timeout for all requests
Curl.DefaultMaxTimeSeconds = 30;

// Per-request timeout
await Curl.ExecuteAsync("curl --max-time 10 https://api.example.com");

// Using CancellationToken
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
await Curl.ExecuteAsync("curl https://api.example.com", cts.Token);
```

---

## Performance Questions

### Is CurlDotNet fast?

CurlDotNet is designed for developer productivity, not raw performance. Under the hood, it uses HttpClient, so performance is comparable. For high-throughput scenarios, consider:
- Reusing the same command pattern
- Using `ExecuteManyAsync` for parallel requests
- Using HttpClient directly for maximum performance

### Can I make parallel requests?

Yes, use `ExecuteManyAsync`:

```csharp
var results = await Curl.ExecuteManyAsync(
    "curl https://api.example.com/users/1",
    "curl https://api.example.com/users/2",
    "curl https://api.example.com/users/3"
);
// All three requests run in parallel!
```

### Does it support streaming?

Yes, responses are streamed internally. For large file downloads:

```csharp
// Downloads are streamed, not loaded fully into memory
await Curl.DownloadAsync("https://example.com/large-file.zip", "output.zip");
```

---

## Compatibility Questions

### Does CurlDotNet support all curl options?

CurlDotNet supports 300+ curl options. Most common options work exactly as expected. Some rarely-used options may not be implemented. If you find an unsupported option, please [open an issue](https://github.com/jacob-mellor/curl-dot-net/issues).

### Can I use CurlDotNet with ASP.NET Core?

Yes! Configure it in your startup:

```csharp
// In Program.cs
Curl.DefaultMaxTimeSeconds = 30;
Curl.DefaultFollowRedirects = true;

// In a controller
[HttpGet]
public async Task<IActionResult> GetData()
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
    return Ok(result.Body);
}
```

### Does it work with .NET Framework?

Yes, CurlDotNet supports .NET Framework 4.7.2 and later via .NET Standard 2.0.

### Can I use CurlDotNet in a Blazor app?

Yes, CurlDotNet works in Blazor Server. For Blazor WebAssembly, note that browser security restrictions apply to HTTP requests.

---

## Troubleshooting Questions

### Why am I getting SSL errors?

SSL errors typically occur with:
- Self-signed certificates
- Expired certificates
- Mismatched domains

**Development solution** (NOT for production):
```csharp
await Curl.ExecuteAsync("curl -k https://localhost:5001");
```

**Production solution:**
- Install proper SSL certificates
- Use `--cacert` to specify custom CA bundle

### Why does my curl command work in terminal but not in CurlDotNet?

Common reasons:
1. **Environment variables** - CurlDotNet doesn't read shell variables like `$TOKEN`
2. **Shell expansion** - Backticks and `$(...)` aren't supported
3. **File paths** - Ensure paths are absolute or correct relative paths

### Why am I getting a 401 Unauthorized error?

Common causes:
1. Missing or incorrect authentication
2. Token expired
3. Wrong authentication method

Check your headers:
```csharp
// Ensure correct authorization header
await Curl.ExecuteAsync("curl -H 'Authorization: Bearer YOUR_ACTUAL_TOKEN' https://api.example.com");
```

### How do I debug requests?

Use verbose mode to see what's happening:

```csharp
var result = await Curl.ExecuteAsync("curl -v https://api.example.com");
// Verbose output shows headers sent and received
```

Or validate your command first:

```csharp
var validation = Curl.Validate("curl -X POST https://api.example.com");
if (!validation.IsValid)
{
    Console.WriteLine($"Invalid: {validation.ErrorMessage}");
}
```

---

## Migration Questions

### How do I migrate from HttpClient to CurlDotNet?

Use the code conversion tools:

```csharp
// If you have curl commands, just paste them
var result = await Curl.ExecuteAsync("curl https://api.example.com");

// If you want to see equivalent HttpClient code
string httpClientCode = Curl.ToHttpClient("curl -X POST -d 'data' https://api.example.com");
Console.WriteLine(httpClientCode);
```

### Can I convert CurlDotNet code to other languages?

Yes! CurlDotNet can generate equivalent code:

```csharp
// To JavaScript fetch()
string jsCode = Curl.ToFetch("curl https://api.example.com");

// To Python requests
string pythonCode = Curl.ToPythonRequests("curl https://api.example.com");

// To PowerShell
string psCode = Curl.ToPowershellCode("curl https://api.example.com");
```

---

## Still Have Questions?

- [Common Issues](common-issues.html) - Solutions to frequent problems
- [Error Reference](error-reference.html) - All exception types explained
- [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues) - Ask questions or report bugs
- [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions) - Community help
