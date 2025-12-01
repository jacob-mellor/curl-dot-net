# 📘 CurlDotNet API Guide

Complete reference guide with examples for every feature of CurlDotNet.

## 🎯 The Three APIs

CurlDotNet offers three different ways to make HTTP requests:

### 1. Curl String API (Simplest)
```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");
```
**Best for**: Copy-pasting curl commands, quick scripts, prototyping

### 2. CurlRequestBuilder API (Type-Safe)
```csharp
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.example.com")
    .WithBearerToken("token123")
    .ExecuteAsync();
```
**Best for**: Building requests programmatically, compile-time safety

### 3. LibCurl API (Reusable)
```csharp
var curl = new LibCurl()
    .WithBearerToken("token123");

var result = await curl.GetAsync("https://api.example.com");
```
**Best for**: Reusable clients, dependency injection, shared configuration

## 📚 Complete API Reference

### Core Classes

#### Curl Class
The main entry point for executing curl commands
- `ExecuteAsync()` - Execute any curl command
- `Get()` - Quick GET request
- `Post()` - Quick POST request
- `Download()` - Download files
- `Upload()` - Upload files
- `ExecuteMany()` - Parallel execution

#### CurlResult Class
The response object returned from all requests
- **Properties**: `Body`, `StatusCode`, `Headers`, `IsSuccess`
- **JSON Operations**: `ParseJson<T>()`, `AsJson<T>()`, `AsJsonDynamic()`
- **Validation**: `EnsureSuccess()`, `EnsureStatus()`

#### CurlRequestBuilder Class
Fluent API for building requests
- **HTTP Methods**: `Get()`, `Post()`, `Put()`, `Delete()`
- **Headers**: `WithHeader()`, `WithHeaders()`
- **Body**: `WithData()`, `WithJson()`, `WithFormData()`
- **Auth**: `WithBasicAuth()`, `WithBearerToken()`
- **Options**: `WithTimeout()`, `WithFollowRedirects()`

#### LibCurl Class
Stateful client for reusable configuration
- **HTTP Verbs**: `GetAsync()`, `PostAsync()`, `PutAsync()`
- **Configuration**: `WithHeader()`, `WithTimeout()`, `WithProxy()`
- **Reusability**: Building client classes

### Exception Classes

All CurlDotNet exceptions inherit from `CurlException`. See [Troubleshooting](../troubleshooting/README.md) for details.

Common exceptions:
- `CurlHttpException` - HTTP errors (4xx, 5xx)
- `CurlTimeoutException` - Request timeout
- `CurlConnectionException` - Connection failures
- `CurlSslException` - SSL/TLS errors
- `CurlAuthenticationException` - Auth failures

## 🚀 Quick Examples

### Basic GET Request

```csharp
// String API
var result = await Curl.ExecuteAsync("curl https://api.github.com");

// Builder API
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.github.com")
    .ExecuteAsync();

// LibCurl API
using var curl = new LibCurl();
var result = await curl.GetAsync("https://api.github.com");
```

### POST with JSON

```csharp
// String API
var result = await Curl.ExecuteAsync(@"
    curl -X POST https://api.example.com/users \
    -H 'Content-Type: application/json' \
    -d '{""name"": ""John"", ""email"": ""john@example.com""}'
");

// Builder API
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.example.com/users")
    .WithMethod(HttpMethod.Post)
    .WithJson(new { name = "John", email = "john@example.com" })
    .ExecuteAsync();

// LibCurl API
using var curl = new LibCurl();
var result = await curl.PostAsync(
    "https://api.example.com/users",
    new { name = "John", email = "john@example.com" }
);
```

### With Authentication

```csharp
// String API
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com \
    -H 'Authorization: Bearer token123'
");

// Builder API
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.example.com")
    .WithBearerToken("token123")
    .ExecuteAsync();

// LibCurl API
using var curl = new LibCurl()
    .WithBearerToken("token123");
var result = await curl.GetAsync("https://api.example.com");
```

### Error Handling

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
    result.EnsureSuccess();  // Throws if not 2xx

    var data = result.ParseJson<MyData>();
}
catch (CurlHttpException ex)
{
    Console.WriteLine($"HTTP error {ex.StatusCode}: {ex.Message}");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Request timed out: {ex.Message}");
}
catch (CurlException ex)
{
    Console.WriteLine($"Curl error: {ex.Message}");
}
```

## 🎯 Which API Should You Use?

### Use String API When:
- ✅ You have existing curl commands
- ✅ You're prototyping or testing
- ✅ You're migrating from shell scripts
- ✅ You need maximum curl compatibility

### Use Builder API When:
- ✅ You're building requests programmatically
- ✅ You want compile-time safety
- ✅ You need IntelliSense support
- ✅ You're building dynamic requests

### Use LibCurl API When:
- ✅ You need a reusable client
- ✅ You're using dependency injection
- ✅ You have shared configuration
- ✅ You're building service classes

## 📊 Feature Comparison

| Feature | String API | Builder API | LibCurl API |
|---------|------------|-------------|-------------|
| Copy-paste curl | ✅ Perfect | ❌ Must convert | ❌ Must convert |
| Type safety | ❌ Runtime | ✅ Compile-time | ✅ Compile-time |
| IntelliSense | ❌ No | ✅ Full | ✅ Full |
| Reusable config | ❌ No | ❌ No | ✅ Yes |
| DI-friendly | ⚠️ Static | ⚠️ Per-request | ✅ Injectable |
| Learning curve | 🟢 Easy | 🟡 Medium | 🟡 Medium |

## 🔧 Advanced Features

### Parallel Requests
```csharp
var results = await Curl.ExecuteMany(
    "curl https://api1.example.com",
    "curl https://api2.example.com",
    "curl https://api3.example.com"
);
```

### Custom HTTP Clients
```csharp
var httpClient = new HttpClient();
var result = await Curl.ExecuteAsync(
    "curl https://api.example.com",
    httpClient
);
```

## 📚 Learning Path

1. **Beginners**: Start with the Curl String API examples above
2. **Type Safety**: Move to CurlRequestBuilder API
3. **Reusability**: Graduate to LibCurl API
4. **Advanced**: Explore parallel requests and custom HTTP clients

## 🔗 Related Documentation

- [Tutorials](../tutorials/README.md) - Learn the basics
- [Cookbook](../cookbook/README.md) - Ready-to-use recipes
- [Troubleshooting](../troubleshooting/README.md) - Fix common issues

---

**Ready to dive in?** Check out the examples above to get started!

*All APIs support the same 300+ curl options and work on .NET Framework 4.7.2+, .NET Core 2.0+, and .NET 5-10*