---
layout: page
title: API Reference
permalink: /api/
---

# CurlDotNet API Reference

Complete API documentation for all namespaces, classes, and methods in CurlDotNet.

## Namespaces

- [CurlDotNet](CurlDotNet.md) - Main namespace
- [CurlDotNet.Core](CurlDotNet.Core.md) - Core functionality
- [CurlDotNet.Exceptions](CurlDotNet.Exceptions.md) - Exception types
- [CurlDotNet.Extensions](CurlDotNet.Extensions.md) - Extension methods
- [CurlDotNet.Middleware](CurlDotNet.Middleware.md) - Middleware pipeline
- [CurlDotNet.Lib](CurlDotNet.Lib.md) - Library internals

## Main Classes

### Core Components
- [Curl](CurlDotNet.Curl.md) - Main entry point
- [CurlEngine](CurlDotNet.Core.CurlEngine.md) - Core execution engine
- [CurlResult](CurlDotNet.Core.CurlResult.md) - Request results
- [CurlOptions](CurlDotNet.Core.CurlOptions.md) - Configuration options
- [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md) - Fluent request builder

### Middleware
- [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md) - Middleware interface
- [CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md) - Pipeline manager
- [RetryMiddleware](CurlDotNet.Middleware.RetryMiddleware.md) - Retry logic
- [RateLimitMiddleware](CurlDotNet.Middleware.RateLimitMiddleware.md) - Rate limiting
- [CachingMiddleware](CurlDotNet.Middleware.CachingMiddleware.md) - Response caching

### Exceptions
- [CurlException](CurlDotNet.Exceptions.CurlException.md) - Base exception
- [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md) - Timeout errors
- [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.md) - Connection errors
- [CurlAuthenticationException](CurlDotNet.Exceptions.CurlAuthenticationException.md) - Auth errors
