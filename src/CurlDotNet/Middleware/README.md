# Middleware System

This directory contains the middleware pipeline system that enables request/response interception and modification.

## Purpose

The middleware system allows developers to intercept and modify requests and responses, enabling features like:
- Logging requests/responses
- Retrying failed requests
- Caching responses
- Authentication token injection
- Rate limiting
- Request/response transformation

## Components

### Middleware Interface

**`ICurlMiddleware.cs`** - Contract for middleware components. Defines:
- `Task<CurlResult> ExecuteAsync(CurlOptions options, Func<CurlOptions, CancellationToken, Task<CurlResult>> next, CancellationToken cancellationToken)` - Execute middleware logic

**Purpose**: Enables developers to create custom middleware for specific needs.

### Middleware Pipeline

**`CurlMiddlewarePipeline.cs`** - Manages the execution pipeline for middleware components. Handles:
- Middleware registration and ordering
- Pipeline execution
- Request/response flow through middleware chain

### Built-in Middleware

**`BuiltInMiddleware.cs`** - Contains implementations of common middleware:
- **LoggingMiddleware** - Logs requests and responses
- **RetryMiddleware** - Retries failed requests with exponential backoff
- **CachingMiddleware** - Caches responses based on URL and headers
- **AuthMiddleware** - Injects authentication tokens
- **RateLimitingMiddleware** - Enforces rate limits

## Design Principles

1. **Pipeline Pattern** - Middleware executes in order, can modify request/response
2. **Composability** - Combine multiple middleware components
3. **Simplicity** - Easy to create custom middleware
4. **Performance** - Minimal overhead when not used

## Usage Example

```csharp
// Register middleware
var pipeline = new CurlMiddlewarePipeline();
pipeline.AddMiddleware(new LoggingMiddleware());
pipeline.AddMiddleware(new RetryMiddleware(maxRetries: 3));
pipeline.AddMiddleware(new CachingMiddleware());

// Execute request through pipeline
var result = await pipeline.ExecuteAsync(options, cancellationToken);
```

## See Also

- [BuiltInMiddleware.cs](./BuiltInMiddleware.cs)
- [ICurlMiddleware.cs](./ICurlMiddleware.cs)
- [CurlMiddlewarePipeline.cs](./CurlMiddlewarePipeline.cs)

