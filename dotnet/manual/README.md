# CurlDotNet Manual

Welcome to the CurlDotNet comprehensive manual! This folder contains detailed documentation, tutorials, and examples for using CurlDotNet in your .NET applications.

## 📚 Table of Contents

### Getting Started
- [Introduction](01-Introduction.md) - What is CurlDotNet and why use it?
- [Installation](02-Installation.md) - Installing and setting up CurlDotNet
- [Quick Start](03-Quick-Start.md) - Get up and running in 5 minutes

### Core Concepts
- [Three Ways to Use CurlDotNet](04-Three-Ways.md) - Curl strings, Builder API, and LibCurl
- [Memory vs Disk Output](05-Memory-vs-Disk.md) - How responses are handled
- [Error Handling](06-Error-Handling.md) - Exception hierarchy and best practices

### Tutorials - Curl String API
- [Basic HTTP Methods](tutorials/01-Basic-HTTP-Methods.md) - GET, POST, PUT, DELETE
- [Headers and Authentication](tutorials/02-Headers-and-Auth.md) - Setting headers, Bearer tokens, Basic auth
- [Request Bodies](tutorials/03-Request-Bodies.md) - JSON, form data, file uploads
- [Response Handling](tutorials/04-Response-Handling.md) - Working with CurlResult
- [Redirects and Follows](tutorials/05-Redirects.md) - Handling redirects
- [Timeouts and Retries](tutorials/06-Timeouts-Retries.md) - Managing timeouts and retries
- [SSL/TLS Configuration](tutorials/07-SSL-TLS.md) - Certificate handling, insecure mode
- [Proxies](tutorials/08-Proxies.md) - HTTP and SOCKS proxy configuration
- [Cookies](tutorials/09-Cookies.md) - Cookie handling and cookie jars
- [File Operations](tutorials/10-File-Operations.md) - Saving responses, uploading files
- [Verbose and Debugging](tutorials/11-Verbose-Debugging.md) - Verbose output, debugging tips

### Tutorials - Builder API
- [Fluent Builder Basics](tutorials/20-Builder-Basics.md) - Using CurlRequestBuilder
- [Builder Examples](tutorials/21-Builder-Examples.md) - Real-world builder patterns

### Tutorials - LibCurl API
- [LibCurl Overview](tutorials/30-LibCurl-Overview.md) - Object-oriented API
- [LibCurl Examples](tutorials/31-LibCurl-Examples.md) - Reusable client patterns

### Advanced Topics
- [Middleware](advanced/01-Middleware.md) - Custom middleware pipeline
- [Custom Protocols](advanced/02-Custom-Protocols.md) - FTP, File protocol handlers
- [Performance Optimization](advanced/03-Performance.md) - Best practices for performance
- [Testing](advanced/04-Testing.md) - Testing with CurlDotNet
- [Cross-Platform Considerations](advanced/05-Cross-Platform.md) - Windows, macOS, Linux differences

### Real-World Examples
- [GitHub API Integration](examples/01-GitHub-API.md) - Complete GitHub API client
- [REST API Client](examples/02-REST-API-Client.md) - Building a generic REST client
- [File Download Manager](examples/03-File-Download.md) - Downloading files with progress
- [API Testing](examples/04-API-Testing.md) - Using CurlDotNet for API testing
- [Web Scraping](examples/05-Web-Scraping.md) - Basic web scraping patterns

### Reference
- [API Reference](reference/API-Reference.md) - Full API documentation
- [Curl Option Mapping](reference/Curl-Options.md) - Mapping curl options to CurlDotNet
- [Exception Reference](reference/Exceptions.md) - All exception types
- [Migration Guide](reference/Migration.md) - Migrating from HttpClient/WebClient

### Contributing
- [Contributing Guidelines](../CONTRIBUTING.md)
- [Code of Conduct](../CODE_OF_CONDUCT.md)

---

**Last Updated:** 2025-01-15

**Version:** 1.0.0

