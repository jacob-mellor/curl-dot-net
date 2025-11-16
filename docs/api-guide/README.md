# 📘 CurlDotNet API Guide

Complete reference guide with examples for every feature of CurlDotNet.

## 🎯 The Three APIs

CurlDotNet offers three different ways to make HTTP requests:

### 1. [Curl String API](curl-class/README.md) (Simplest)
```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");
```
**Best for**: Copy-pasting curl commands, quick scripts, prototyping

### 2. [CurlRequestBuilder API](curl-request-builder/README.md) (Type-Safe)
```csharp
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.example.com")
    .WithBearerToken("token123")
    .ExecuteAsync();
```
**Best for**: Building requests programmatically, compile-time safety

### 3. [LibCurl API](libcurl/README.md) (Reusable)
```csharp
var curl = new LibCurl()
    .WithBearerToken("token123");

var result = await curl.GetAsync("https://api.example.com");
```
**Best for**: Reusable clients, dependency injection, shared configuration

## 📚 Complete API Reference

### Core Classes

#### [Curl Class](curl-class/README.md)
The main entry point for executing curl commands
- [`ExecuteAsync()`](curl-class/execute-async.md) - Execute any curl command
- [`Get()`](curl-class/shortcuts.md#get) - Quick GET request
- [`Post()`](curl-class/shortcuts.md#post) - Quick POST request
- [`Download()`](curl-class/shortcuts.md#download) - Download files
- [`Upload()`](curl-class/shortcuts.md#upload) - Upload files
- [`ExecuteMany()`](curl-class/execute-many.md) - Parallel execution
- [`Validate()`](curl-class/validation.md) - Validate curl syntax
- [`ToHttpClient()`](curl-class/conversion.md#tohttpclient) - Convert to HttpClient

#### [CurlResult Class](curl-result/README.md)
The response object returned from all requests
- **Properties**: [`Body`](curl-result/properties.md#body), [`StatusCode`](curl-result/properties.md#statuscode), [`Headers`](curl-result/properties.md#headers), [`IsSuccess`](curl-result/properties.md#issuccess)
- **JSON Operations**: [`ParseJson<T>()`](curl-result/json-operations.md#parsejson), [`AsJson<T>()`](curl-result/json-operations.md#asjson), [`AsJsonDynamic()`](curl-result/json-operations.md#asjsondynamic)
- **Save Operations**: [`SaveToFile()`](curl-result/save-operations.md#savetofile), [`SaveAsJson()`](curl-result/save-operations.md#saveasjson), [`SaveAsCsv()`](curl-result/save-operations.md#saveascsv)
- **Validation**: [`EnsureSuccess()`](curl-result/validation.md#ensuresuccess), [`EnsureStatus()`](curl-result/validation.md#ensurestatus)
- **Retry**: [`Retry()`](curl-result/retry.md#retry), [`RetryWith()`](curl-result/retry.md#retrywith)

#### [CurlRequestBuilder Class](curl-request-builder/README.md)
Fluent API for building requests
- **HTTP Methods**: [`Get()`](curl-request-builder/http-methods.md), [`Post()`](curl-request-builder/http-methods.md), [`Put()`](curl-request-builder/http-methods.md), [`Delete()`](curl-request-builder/http-methods.md)
- **Headers**: [`WithHeader()`](curl-request-builder/headers.md), [`WithHeaders()`](curl-request-builder/headers.md)
- **Body**: [`WithData()`](curl-request-builder/body-data.md), [`WithJson()`](curl-request-builder/body-data.md), [`WithFormData()`](curl-request-builder/body-data.md)
- **Auth**: [`WithBasicAuth()`](curl-request-builder/authentication.md), [`WithBearerToken()`](curl-request-builder/authentication.md)
- **Options**: [`WithTimeout()`](curl-request-builder/options.md), [`WithFollowRedirects()`](curl-request-builder/options.md)

#### [LibCurl Class](libcurl/README.md)
Stateful client for reusable configuration
- **HTTP Verbs**: [`GetAsync()`](libcurl/http-methods.md), [`PostAsync()`](libcurl/http-methods.md), [`PutAsync()`](libcurl/http-methods.md)
- **Configuration**: [`WithHeader()`](libcurl/configuration.md), [`WithTimeout()`](libcurl/configuration.md), [`WithProxy()`](libcurl/configuration.md)
- **Reusability**: [Building client classes](libcurl/reusable-clients.md)

### Exception Classes

All CurlDotNet exceptions inherit from `CurlException`. See [Error Handling Guide](../error-handling/README.md) for details.

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

### Middleware System
```csharp
var pipeline = new CurlMiddlewarePipeline()
    .Use(new LoggingMiddleware())
    .Use(new RetryMiddleware(3))
    .Use(new CachingMiddleware());

var result = await pipeline.ExecuteAsync(options);
```
See [Middleware Guide](../advanced/middleware/README.md)

### Parallel Requests
```csharp
var results = await Curl.ExecuteMany(
    "curl https://api1.example.com",
    "curl https://api2.example.com",
    "curl https://api3.example.com"
);
```
See [Parallel Execution](curl-class/execute-many.md)

### Custom HTTP Clients
```csharp
var httpClient = new HttpClient();
var result = await Curl.ExecuteAsync(
    "curl https://api.example.com",
    httpClient
);
```
See [Custom Clients](../advanced/custom-http-client.md)

## 📚 Learning Path

1. **Beginners**: Start with [Curl String API](curl-class/README.md)
2. **Type Safety**: Move to [CurlRequestBuilder](curl-request-builder/README.md)
3. **Reusability**: Graduate to [LibCurl](libcurl/README.md)
4. **Advanced**: Explore [Middleware](../advanced/middleware/README.md)

## 🔗 Related Documentation

- [Tutorials](../tutorials/README.md) - Learn the basics
- [Cookbook](../cookbook/README.md) - Ready-to-use recipes
- [Examples](../examples/README.md) - Complete programs
- [Error Handling](../error-handling/README.md) - Exception reference
- [Troubleshooting](../troubleshooting/README.md) - Fix common issues

---

**Ready to dive in?** Start with → [Curl Class Guide](curl-class/README.md)

*All APIs support the same 300+ curl options and work on .NET Framework 4.7.2+, .NET Core 2.0+, and .NET 5-10*