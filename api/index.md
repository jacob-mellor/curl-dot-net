---
layout: default
title: API Reference
---

# CurlDotNet API Reference

Complete API documentation for all classes and namespaces.

## Key Classes

### Main Entry Points
- [Curl](CurlDotNet.Curl.md) - Static methods for simple operations
- [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md) - Fluent API for complex requests

### Core Types
- [CurlResult](CurlDotNet.Core.CurlResult.md) - Response object with rich functionality
- [CurlOptions](CurlDotNet.Core.CurlOptions.md) - All available curl options
- [CurlSettings](CurlDotNet.Core.CurlSettings.md) - Configuration settings

### Exceptions
- [CurlException](CurlDotNet.Exceptions.CurlException.md) - Base exception class
- [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md) - HTTP-specific errors
- [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md) - Timeout errors

### Middleware
- [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md) - Middleware interface
- [RetryMiddleware](CurlDotNet.Middleware.RetryMiddleware.md) - Retry logic
- [RateLimitMiddleware](CurlDotNet.Middleware.RateLimitMiddleware.md) - Rate limiting

## Namespaces

- **[CurlDotNet](CurlDotNet.md)** - Main namespace with public API
- **[CurlDotNet.Core](CurlDotNet.Core.md)** - Core functionality
- **[CurlDotNet.Exceptions](CurlDotNet.Exceptions.md)** - Exception types
- **[CurlDotNet.Middleware](CurlDotNet.Middleware.md)** - Middleware components
- **[CurlDotNet.Extensions](CurlDotNet.Extensions.md)** - Extension methods
- **[CurlDotNet.Lib](CurlDotNet.Lib.md)** - Internal implementation

## Quick Examples

### Simple GET Request
```csharp
var response = await Curl.GetAsync("https://api.example.com/data");
Console.WriteLine(response.Body);
```

### POST with JSON
```csharp
var data = new { name = "John", age = 30 };
var response = await Curl.PostJsonAsync("https://api.example.com/users", data);
```

### Using the Fluent Builder
```csharp
var response = await new CurlRequestBuilder()
    .Post("https://api.example.com/data")
    .WithHeader("Authorization", "Bearer token")
    .WithTimeout(TimeSpan.FromSeconds(30))
    .WithRetry(3)
    .ExecuteAsync();
```

---

## Author

**Jacob Mellor** - Senior Software Engineer at IronSoftware
- [GitHub](https://github.com/jacob-mellor)
- [IronSoftware Profile](https://ironsoftware.com/about-us/authors/jacobmellor/)
- [LinkedIn](https://linkedin.com/in/jacob-mellor-iron-software)

Sponsored by [IronSoftware](https://ironsoftware.com)
