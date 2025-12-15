# API Documentation

Complete API reference for CurlDotNet.

## API Surfaces

### String API
Simple curl command execution using strings.

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");
```

### Builder API
Fluent interface for building requests.

```csharp
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.github.com")
    .ExecuteAsync();
```

### LibCurl API
Low-level curl implementation.

```csharp
using var curl = new LibCurl();
var result = await curl.GetAsync("https://api.github.com");
```

## Namespaces

- `CurlDotNet` - Main namespace
- `CurlDotNet.Core` - Core functionality
- `CurlDotNet.Exceptions` - Exception types
- `CurlDotNet.Middleware` - Middleware support

## Quick Links

- [Curl Class](CurlDotNet.Curl.html)
- [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.html)
- [LibCurl](CurlDotNet.Lib.LibCurl.html)
- [CurlResult](CurlDotNet.Core.CurlResult.html)
