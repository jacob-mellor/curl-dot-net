# New to curl? Start Here!

Welcome! If you're new to curl or HTTP clients in general, you're in the right place. This guide will help you understand what curl is, why CurlDotNet exists, and how to get started with making HTTP requests in .NET.

## What is curl?

curl (pronounced "curl" or "see-URL") is one of the most widely used command-line tools and libraries for transferring data with URLs. Created in 1996, it's battle-tested, incredibly reliable, and installed on billions of devices worldwide.

### Why curl matters:
- **Universal**: Pre-installed on macOS, Linux, and Windows 10+
- **Reliable**: 25+ years of development and testing
- **Powerful**: Supports 20+ protocols (HTTP, HTTPS, FTP, SFTP, etc.)
- **Trusted**: Used by NASA, Fortune 500 companies, and millions of developers

## What is CurlDotNet?

CurlDotNet brings the power and reliability of curl to .NET developers through a pure C# implementation. Unlike other .NET HTTP clients that have their own implementations, CurlDotNet provides curl's exact behavior and features.

### Key Benefits:
- **100% Pure C#**: No native dependencies or P/Invoke
- **Cross-platform**: Works everywhere .NET runs
- **Familiar**: If you know curl commands, you know CurlDotNet
- **Modern**: Full async/await support with .NET best practices

## Your First Request

Making an HTTP request with CurlDotNet is simple:

```csharp
using CurlDotNet;

// Create a GET request
var curl = new Curl();
var result = await curl.GetAsync("https://api.github.com/users/github");

// Check if successful
if (result.IsSuccess)
{
    Console.WriteLine(result.Data);  // JSON response
}
else
{
    Console.WriteLine($"Error: {result.Error}");
}
```

## Common Use Cases

### 1. Fetching Data from APIs
```csharp
var curl = new Curl();
var weather = await curl.GetAsync("https://api.weather.gov/gridpoints/TOP/31,80");
```

### 2. Submitting Forms
```csharp
var formData = new FormUrlEncodedContent(new Dictionary<string, string>
{
    ["username"] = "user@example.com",
    ["password"] = "secure123"
});

var result = await curl.PostAsync("https://example.com/login", formData);
```

### 3. Downloading Files
```csharp
await curl.DownloadFileAsync(
    "https://example.com/document.pdf",
    "/path/to/save/document.pdf"
);
```

## Key Concepts

### The Curl Object
The `Curl` class is your main interface to the library. Create one instance and reuse it for multiple requests:

```csharp
var curl = new Curl();  // Create once, use many times
```

### Results and Error Handling
Every operation returns a `CurlResult` with:
- `IsSuccess`: Did the request complete successfully?
- `Data`: The response body (if successful)
- `StatusCode`: HTTP status code
- `Error`: Error message (if failed)
- `Headers`: Response headers

### Async by Default
CurlDotNet is built for modern .NET with async/await:
```csharp
// All operations are async
await curl.GetAsync(url);
await curl.PostAsync(url, data);
await curl.PutAsync(url, data);
await curl.DeleteAsync(url);
```

## Next Steps

Now that you understand the basics:

1. **[Install CurlDotNet](getting-started/installation.md)** - Add it to your project
2. **[Follow the Tutorials](tutorials/01-what-is-dotnet.md)** - Step-by-step learning path
3. **[Try the Cookbook](cookbook/index.md)** - Copy-paste solutions for common tasks
4. **[Explore Examples](https://github.com/jacob-mellor/curl-dot-net/tree/master/examples)** - Complete working applications

## Getting Help

- **Documentation**: You're already here!
- **Examples**: Check the [examples directory](https://github.com/jacob-mellor/curl-dot-net/tree/master/examples)
- **Issues**: [Report bugs or ask questions](https://github.com/jacob-mellor/curl-dot-net/issues)
- **Cookbook**: [Common recipes and patterns](cookbook/index.md)

## Why Choose CurlDotNet?

If you're wondering why to use CurlDotNet over HttpClient or RestSharp:

- **Consistency**: Same behavior as curl command-line tool
- **Simplicity**: Minimal API surface, easy to learn
- **Reliability**: Built on curl's proven foundation
- **Performance**: Optimized for real-world usage
- **Support**: Active development and community

Ready to start? [Install CurlDotNet](getting-started/installation.md) and make your first request in minutes!

---

*CurlDotNet is the .NET binding for the world's most trusted HTTP client. Join millions of developers who rely on curl every day.*