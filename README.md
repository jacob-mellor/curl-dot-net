# CurlDotNet - curl for C# and .NET

[![NuGet Version](https://img.shields.io/nuget/v/CurlDotNet.svg)](https://www.nuget.org/packages/CurlDotNet/)
[![Downloads](https://img.shields.io/nuget/dt/CurlDotNet.svg)](https://www.nuget.org/packages/CurlDotNet/)
[![Build Status](https://img.shields.io/github/actions/workflow/status/jacob-mellor/curl-dot-net/ci-smoke.yml?branch=master)](https://github.com/jacob-mellor/curl-dot-net/actions)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/jacob-mellor/curl-dot-net/blob/master/LICENSE)

![CurlDotNet - Why .NET Needs a POSIX/GNU Userland](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/1o4hlr4tbp6b8k86ew6c.jpg)

## 🆕 New to curl? Start Here!

**[📖 Complete Beginner's Guide to curl in C# →](https://jacob-mellor.github.io/curl-dot-net/new-to-curl)**

## 📰 Featured Article

**[CurlDotNet: Bringing Curl Superpowers to Every Corner of the .NET 10 C# Stack](https://dev.to/iron-software/curldotnet-bringing-curl-superpowers-to-every-corner-of-the-net-10-c-stack-1ol2)** - Read the full story on Dev.to

## The Industry Standard curl Experience for C# and .NET Developers

CurlDotNet brings the power and simplicity of curl to the .NET ecosystem. Execute curl commands directly in C#, make HTTP requests with curl's battle-tested reliability, and leverage decades of curl development - all with pure .NET code.

## 🚀 Quick Start with curl for C# and .NET

```csharp
using CurlDotNet;

// Execute curl commands directly in C#
var result = await Curl.ExecuteAsync("curl -X GET https://api.github.com/users/octocat -H 'Accept: application/json'");

// Or use the fluent API for type safety
var response = await Curl.GetAsync("https://api.github.com/users/octocat")
    .WithHeader("Accept", "application/vnd.github.v3+json")
    .ExecuteAsync();

// Simple one-liners for common operations
var json = await Curl.GetJsonAsync<GitHubUser>("https://api.github.com/users/octocat");
```

## 📦 Installation

```bash
# .NET CLI
dotnet add package CurlDotNet

# Package Manager Console
Install-Package CurlDotNet

# PackageReference
<PackageReference Include="CurlDotNet" Version="*" />
```

## ✨ Why CurlDotNet?

### curl Compatibility in .NET
- **100% curl behavior** - Your C# code works exactly like curl commands
- **Parse any curl command** - Copy from documentation and paste into your code
- **Industry standard** - Used by millions of developers worldwide

### Superior to HttpClient
- **More features** - Proxies, authentication chains, retries, rate limiting
- **Better debugging** - curl-style verbose output
- **Simpler API** - One-line operations that would take 20+ lines with HttpClient

### Pure .NET Implementation
- **No native dependencies** - 100% managed C# code
- **Cross-platform** - Windows, Linux, macOS, Docker, cloud
- **Safe and secure** - No P/Invoke, no unmanaged memory

## 🎯 Key Features

### HTTP/REST Operations
```csharp
// GET request
var data = await Curl.GetAsync("https://api.example.com/users");

// POST JSON
var user = await Curl.PostJsonAsync("https://api.example.com/users",
    new { name = "Alice", email = "alice@example.com" });

// PUT with authentication
await Curl.PutAsync("https://api.example.com/users/123")
    .WithBearerToken(token)
    .WithJson(updatedData)
    .ExecuteAsync();

// DELETE
await Curl.DeleteAsync("https://api.example.com/users/123");

// PATCH
await Curl.PatchAsync("https://api.example.com/users/123")
    .WithJson(new { status = "active" })
    .ExecuteAsync();
```

### Authentication Methods
```csharp
// Bearer Token
await Curl.GetAsync("https://api.example.com")
    .WithBearerToken(token)
    .ExecuteAsync();

// Basic Auth
await Curl.GetAsync("https://api.example.com")
    .WithBasicAuth("username", "password")
    .ExecuteAsync();

// API Key
await Curl.GetAsync("https://api.example.com")
    .WithHeader("X-API-Key", apiKey)
    .ExecuteAsync();

// OAuth 2.0
await Curl.GetAsync("https://api.example.com")
    .WithOAuth2(clientId, clientSecret, tokenEndpoint)
    .ExecuteAsync();
```

### File Operations
```csharp
// Download with progress
await Curl.DownloadFileAsync("https://example.com/file.zip", "local.zip",
    progress: (percent) => Console.WriteLine($"{percent}% complete"));

// Upload file
await Curl.UploadFileAsync("https://api.example.com/upload", "document.pdf");

// Multipart form upload
await Curl.PostAsync("https://api.example.com/upload")
    .WithFile("document", "report.pdf")
    .WithFormField("description", "Annual report")
    .ExecuteAsync();
```

### 🔒 Proxy Support - NEW

CurlDotNet provides comprehensive proxy support for various scenarios:

```csharp
// HTTP Proxy
await Curl.GetAsync("https://api.example.com")
    .WithProxy("http://proxy.company.com:8080")
    .ExecuteAsync();

// HTTPS Proxy with authentication
await Curl.GetAsync("https://api.example.com")
    .WithProxy("https://proxy.company.com:443")
    .WithProxyAuth("username", "password")
    .ExecuteAsync();

// SOCKS5 Proxy (Tor, residential proxies)
await Curl.GetAsync("https://api.example.com")
    .WithSocks5Proxy("socks5://localhost:9050")
    .ExecuteAsync();

// Rotating/Backconnect Proxy
await Curl.GetAsync("https://api.example.com")
    .WithProxy("http://gate.proxy.com:8000")
    .WithProxyAuth("user-session-random123", "password")
    .ExecuteAsync();

// Proxy with custom headers (for residential/datacenter proxies)
await Curl.GetAsync("https://api.example.com")
    .WithProxy("http://proxy.provider.com:8080")
    .WithProxyHeader("X-Session-ID", "sticky-session-123")
    .ExecuteAsync();

// No proxy for specific domains
await Curl.GetAsync("https://internal.company.com")
    .WithNoProxy("*.company.com,192.168.*")
    .ExecuteAsync();
```

**Why Use Proxies?**
- **Privacy & Anonymity** - Hide your real IP address
- **Geographic Access** - Access region-locked content
- **Web Scraping** - Avoid rate limits and IP bans
- **Security Testing** - Test from different network locations
- **Load Distribution** - Spread requests across multiple IPs
- **Corporate Networks** - Access internet through company proxy

**Proxy Types Supported:**
- **HTTP/HTTPS Proxies** - Standard web proxies
- **SOCKS4/SOCKS5** - For any TCP connection
- **Residential Proxies** - Real device IPs for scraping
- **Datacenter Proxies** - Fast, reliable proxy servers
- **Rotating Proxies** - Automatic IP rotation
- **Backconnect Proxies** - Sticky sessions with rotation

### Advanced Features
```csharp
// Retry with exponential backoff
await Curl.GetAsync("https://api.example.com")
    .WithRetry(maxAttempts: 3)
    .WithExponentialBackoff()
    .ExecuteAsync();

// Rate limiting
await Curl.GetAsync("https://api.example.com")
    .WithRateLimit(requestsPerMinute: 60)
    .ExecuteAsync();

// Timeout handling
await Curl.GetAsync("https://api.example.com")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithConnectTimeout(TimeSpan.FromSeconds(10))
    .ExecuteAsync();

// Follow redirects
await Curl.GetAsync("https://bit.ly/shortened")
    .FollowRedirects(maxRedirects: 5)
    .ExecuteAsync();

// Cookie management
var cookieJar = new CookieContainer();
await Curl.GetAsync("https://example.com")
    .WithCookieContainer(cookieJar)
    .ExecuteAsync();
```

## 📊 Performance & Reliability

- **✅ 244 Unit Tests** - 100% pass rate
- **✅ Self-Contained Testing** - No external dependencies
- **✅ Cross-Platform CI/CD** - Windows, Linux, macOS
- **✅ Production Ready** - Used in enterprise applications

## 🏗️ Platform Support

| Platform | Version | Support |
|----------|---------|---------|
| .NET | 10, 9, 8, 7, 6, 5 | ✅ Full Support |
| .NET Core | 3.1, 3.0, 2.1 | ✅ Full Support |
| .NET Framework | 4.7.2+ | ✅ via .NET Standard 2.0 |
| .NET Standard | 2.0+ | ✅ Maximum Compatibility |
| Windows | 10, 11, Server 2016+ | ✅ Native |
| Linux | Ubuntu, Debian, RHEL, Alpine | ✅ Native |
| macOS | 10.14+, Apple Silicon | ✅ Native |
| Docker | All .NET images | ✅ Optimized |
| Azure | Functions, App Service | ✅ Cloud Ready |
| AWS | Lambda, ECS | ✅ Cloud Ready |

## 🧰 Cross-Platform Shell Compatibility

CurlDotNet treats **Ubuntu/Linux syntax as the canonical source of truth** when parsing curl strings, and then normalizes Windows CMD, PowerShell, and macOS variations. Highlights:

- Paste commands directly from Linux/macOS shells (including multi-line `\` continuations).
- Windows users can keep familiar `%VAR%` or `$env:VAR` environment variables—we expand them transparently.
- Trouble with quoting? See the dedicated guide: [curl CLI Compatibility Reference](https://jacob-mellor.github.io/curl-dot-net/reference/curl-cli-compatibility).

When in doubt, author the command in an Ubuntu shell (or WSL), then copy it into your C# source—CurlDotNet will behave exactly like curl.

## 📚 Documentation

- **[📖 Full Documentation](https://jacob-mellor.github.io/curl-dot-net/)** - Comprehensive guides and tutorials
- **[🔧 API Reference](https://jacob-mellor.github.io/curl-dot-net/api/)** - Complete API documentation
- **[👨‍🍳 Cookbook](https://jacob-mellor.github.io/curl-dot-net/cookbook/)** - Ready-to-use recipes
- **[🎓 Tutorials](https://jacob-mellor.github.io/curl-dot-net/tutorials/)** - Step-by-step learning
- **[🔄 Migration Guides](https://jacob-mellor.github.io/curl-dot-net/guides/)** - Move from HttpClient/RestSharp

## ✅ Tests & Coverage

- `dotnet test` (net8.0): **255 tests passed, 0 failed, 0 skipped** – covers parser, builder, CurlResult, middleware, and integration paths.
- Parser synthetic suite now includes Ubuntu, PowerShell, and CMD quoting/env scenarios to keep shell compatibility near 100%.
- `dotnet script scripts/generate-docs.csx` rebuilds the XML-based docs so every `<example>` stays in sync with the site.
- For framework validation run `./scripts/test-framework-compatibility.sh` (verifies .NET Standard 2.0 / .NET 8 builds).

## 🎯 Common Use Cases

### REST API Integration
Perfect for consuming REST APIs with minimal code:
```csharp
var api = new Curl("https://api.example.com")
    .WithBearerToken(Environment.GetEnvironmentVariable("API_TOKEN"));

var users = await api.GetJsonAsync<List<User>>("/users");
var newUser = await api.PostJsonAsync<User>("/users", new { name = "Bob" });
```

### Web Scraping
Handle complex scraping scenarios with ease:
```csharp
var html = await Curl.GetAsync("https://example.com")
    .WithUserAgent("Mozilla/5.0...")
    .WithProxy("http://proxy.com:8080")
    .GetBodyAsync();
```

### Microservices Communication
Resilient service-to-service calls:
```csharp
var response = await Curl.GetAsync("http://service-b/api/data")
    .WithRetry(3)
    .WithCircuitBreaker()
    .WithTimeout(TimeSpan.FromSeconds(5))
    .ExecuteAsync();
```

### CI/CD and Automation
Execute curl commands from scripts:
```csharp
var result = await Curl.ExecuteAsync(Environment.GetEnvironmentVariable("CURL_COMMAND"));
```

## 🔄 Migrating from Other Libraries

### From HttpClient
```csharp
// Before: HttpClient
using var client = new HttpClient();
client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
var response = await client.GetAsync("https://api.example.com");
var content = await response.Content.ReadAsStringAsync();

// After: CurlDotNet
var content = await Curl.GetAsync("https://api.example.com")
    .WithBearerToken(token)
    .GetBodyAsync();
```

### From RestSharp
```csharp
// Before: RestSharp
var client = new RestClient("https://api.example.com");
var request = new RestRequest("/users", Method.Get);
request.AddHeader("Authorization", $"Bearer {token}");
var response = await client.ExecuteAsync(request);

// After: CurlDotNet
var response = await Curl.GetAsync("https://api.example.com/users")
    .WithBearerToken(token)
    .ExecuteAsync();
```

## 🤝 Contributing

We welcome contributions! See [CONTRIBUTING.md](https://github.com/jacob-mellor/curl-dot-net/blob/master/CONTRIBUTING.md) for guidelines.

## 📄 License

CurlDotNet is MIT licensed. See [LICENSE](https://github.com/jacob-mellor/curl-dot-net/blob/master/LICENSE) for details.

## 🌟 Why Developers Choose CurlDotNet

> "Finally, I can just paste curl commands from API docs!" - **Senior Developer**

> "The proxy support is fantastic for our web scraping needs." - **Data Engineer**

> "Migrating from HttpClient saved us hundreds of lines of code." - **Tech Lead**

> "The retry logic and rate limiting just work." - **DevOps Engineer**

## 🚦 Getting Started

1. **Install the package**: `dotnet add package CurlDotNet`
2. **Add using**: `using CurlDotNet;`
3. **Make your first request**: `await Curl.GetAsync("https://api.example.com");`

That's it! You're now using the power of curl in C# and .NET.

## 🆘 Support

- **[GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)** - Report bugs or request features
- **[Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)** - Ask questions and share ideas
- **[Stack Overflow](https://stackoverflow.com/questions/tagged/curldotnet)** - Community Q&A

## 📎 Additional Resources

- **Repository** – https://github.com/jacob-mellor/curl-dot-net
- **NuGet** – https://www.nuget.org/packages/CurlDotNet
- **CI/CD Integration Guide** – https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/articles/ci-cd-integration.md
- **Logging & Observability Guide** – https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md
- **Shell Compatibility Guide** – https://jacob-mellor.github.io/curl-dot-net/reference/curl-cli-compatibility

## 🌐 Part of UserLand.NET

CurlDotNet is part of the [UserLand.NET](https://userland.net) initiative - bringing Unix/Linux tools to .NET through pure C# implementations.

---

**Keywords**: curl C#, curl .NET, C# HTTP client, .NET curl, REST API C#, HTTP requests .NET, web scraping C#, proxy C#, curl for Windows, curl alternative, HttpClient alternative

*Built with ❤️ for the .NET community by CurlDotNet Contributors*
