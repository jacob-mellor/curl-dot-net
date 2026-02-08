---
layout: page
title: API Reference
permalink: /api/
---

# CurlDotNet API Reference

Complete API documentation for all namespaces, classes, and methods in CurlDotNet.

## Namespaces

- [CurlDotNet](CurlDotNet.html) - Main namespace
- [CurlDotNet.Core](CurlDotNet.Core.html) - Core functionality
- [CurlDotNet.Exceptions](CurlDotNet.Exceptions.html) - Exception types
- [CurlDotNet.Extensions](CurlDotNet.Extensions.html) - Extension methods
- [CurlDotNet.Middleware](CurlDotNet.Middleware.html) - Middleware pipeline
- [CurlDotNet.Lib](CurlDotNet.Lib.html) - Library internals

## Main Classes

### Core Components
- [Curl](CurlDotNet.Curl.html) - Main entry point
- [CurlEngine](CurlDotNet.Core.CurlEngine.html) - Core execution engine
- [CurlResult](CurlDotNet.Core.CurlResult.html) - Request results
- [CurlOptions](CurlDotNet.Core.CurlOptions.html) - Configuration options
- [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.html) - Fluent request builder

### Middleware
- [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.html) - Middleware interface
- [CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.html) - Pipeline manager
- [RetryMiddleware](CurlDotNet.Middleware.RetryMiddleware.html) - Retry logic
- [RateLimitMiddleware](CurlDotNet.Middleware.RateLimitMiddleware.html) - Rate limiting
- [CachingMiddleware](CurlDotNet.Middleware.CachingMiddleware.html) - Response caching

### Exceptions
- [CurlException](CurlDotNet.Exceptions.CurlException.html) - Base exception
- [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.html) - Timeout errors
- [CurlConnectionException](CurlDotNet.Exceptions.CurlConnectionException.html) - Connection errors
- [CurlAuthenticationException](CurlDotNet.Exceptions.CurlAuthenticationException.html) - Auth errors
