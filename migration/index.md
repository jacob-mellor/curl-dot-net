# Migration Guides

Moving to CurlDotNet from another HTTP client? These guides will help you migrate your existing code with minimal effort.

## Available Migration Guides

### [Migrating from HttpClient](httpclient.md)
Learn how to replace `System.Net.Http.HttpClient` with CurlDotNet, including:
- Basic request patterns
- Header management
- Content types
- Error handling
- Performance considerations

### [Migrating from RestSharp](restsharp.md)
Transition from RestSharp to CurlDotNet with:
- Request building patterns
- Serialization/deserialization
- Authentication methods
- Interceptors and middleware
- Testing strategies

## Why Migrate to CurlDotNet?

### From HttpClient
- **Better error messages**: CurlDotNet provides detailed error information
- **Simpler API**: Less ceremony, more productivity
- **curl compatibility**: Leverage curl's proven reliability
- **Consistent behavior**: Same results across all platforms

### From RestSharp
- **Pure C#**: No native dependencies
- **Modern async**: Built for async/await from the ground up
- **Lighter weight**: Smaller package, fewer dependencies
- **Active maintenance**: Regular updates and improvements

## Quick Comparison

### HttpClient
```csharp
// HttpClient
using var client = new HttpClient();
var response = await client.GetAsync("https://api.example.com/data");
var content = await response.Content.ReadAsStringAsync();

// CurlDotNet
var curl = new Curl();
var result = await curl.GetAsync("https://api.example.com/data");
var content = result.Data;
```

### RestSharp
```csharp
// RestSharp
var client = new RestClient("https://api.example.com");
var request = new RestRequest("data", Method.GET);
var response = await client.ExecuteAsync(request);

// CurlDotNet
var curl = new Curl();
var result = await curl.GetAsync("https://api.example.com/data");
```

## Migration Tools

We provide helpers to make migration easier:

- **Extension methods**: HttpClient-like APIs for easier transition
- **Compatibility layer**: Drop-in replacements for common patterns
- **Migration analyzer**: Identify code that needs updating

## Getting Started

1. Choose your migration guide based on your current client
2. Review the key differences and benefits
3. Follow the step-by-step migration examples
4. Test your migrated code thoroughly
5. Enjoy the simplicity and reliability of CurlDotNet!

## Need Help?

- [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues) - Ask questions or report problems
- [Examples](https://github.com/jacob-mellor/curl-dot-net/tree/master/examples) - See working code
- [API Documentation](../api/index.md) - Detailed API reference