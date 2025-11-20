# CurlDotNet

![CurlDotNet - Why .NET Needs a POSIX/GNU Userland](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/1o4hlr4tbp6b8k86ew6c.jpg)

## Pure .NET implementation of curl for C#

CurlDotNet brings the power and simplicity of curl to .NET developers. Just paste any curl command as a string and it works!

### Quick Start

```csharp
// Just paste any curl command as a string:
var response = await Curl.ExecuteAsync("curl https://api.github.com");
Console.WriteLine(response.Body);
```

### Why CurlDotNet?

- **Zero Learning Curve** - If you know curl, you already know CurlDotNet
- **Copy & Paste from API Docs** - Use curl commands directly from documentation
- **Full curl Feature Set** - Supports all 300+ curl options
- **Pure .NET** - No native dependencies or P/Invoke
- **Cross-Platform** - Works on Windows, Linux, macOS, and mobile

### Installation

```bash
dotnet add package CurlDotNet
```

Or via Package Manager Console:

```powershell
Install-Package CurlDotNet
```

### Two Ways to Use CurlDotNet

#### 1. Paste curl Commands
```csharp
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/data
    -H 'Content-Type: application/json'
    -d '{""key"":""value""}'
");
```

#### 2. Fluent Builder API
```csharp
var result = await CurlRequestBuilder
    .Post("https://api.example.com/data")
    .WithHeader("Content-Type", "application/json")
    .WithJson(new { key = "value" })
    .ExecuteAsync();
```

### Features

- ✅ All HTTP methods (GET, POST, PUT, DELETE, PATCH, etc.)
- ✅ Headers and custom headers
- ✅ Authentication (Basic, Bearer, OAuth, etc.)
- ✅ File uploads and downloads
- ✅ Form data and JSON payloads
- ✅ Cookies and sessions
- ✅ Proxies and SOCKS
- ✅ Redirects and retries
- ✅ Timeouts and cancellation
- ✅ Progress callbacks
- ✅ Certificate validation
- ✅ HTTP/2 and HTTP/3
- ✅ WebSockets
- ✅ And much more!

### Supported Platforms

- .NET 8.0+
- .NET 6.0+
- .NET Framework 4.7.2+
- .NET Standard 2.0+

### Sponsors

This project is sponsored by [IronSoftware](https://ironsoftware.com), creators of IronPDF, IronOCR, IronXL, and IronBarcode.

### Links

- [GitHub Repository](https://github.com/jacob-mellor/curl-dot-net)
- [NuGet Package](https://www.nuget.org/packages/CurlDotNet)
- [API Documentation](api/index.md)
- [Getting Started Guide](docs/getting-started/quick-start.md)
- [Tutorials](docs/tutorials/index.md)

### License

CurlDotNet is licensed under the MIT License. See [LICENSE](https://github.com/jacob-mellor/curl-dot-net/blob/master/LICENSE) for details.

### Author

**Jacob Mellor**
- GitHub: [@jacob-mellor](https://github.com/jacob-mellor)
- LinkedIn: [Jacob Mellor](https://linkedin.com/in/jacob-mellor-iron-software)
- Twitter: [@jacobmellor](https://twitter.com/jacobmellor)