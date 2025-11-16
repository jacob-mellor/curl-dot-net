# CurlDotNet Documentation

Welcome to the comprehensive documentation for CurlDotNet - a .NET library that allows you to use curl commands directly in C# without translation.

## 📚 Documentation Structure

### Quick Links
- [Getting Started](#getting-started)
- [Architecture & Design](#architecture--design)
- [Development Guide](#development-guide)
- [API Reference](#api-reference)
- [Examples & Tutorials](#examples--tutorials)

---

## Getting Started

### Installation & Setup
- [Installation Guide](tutorials/getting-started.md) - How to install and configure CurlDotNet
- [Quick Start Examples](examples/README.md) - Jump right in with code examples
- [Basic Usage](tutorials/basic-usage.md) - Common patterns and use cases

### Core Concepts
- [Curl Command Translation](tutorials/curl-translation.md) - How curl commands are processed
- [Response Handling](tutorials/response-handling.md) - Working with HTTP responses
- [Error Handling](tutorials/error-handling.md) - Understanding exceptions and error codes

---

## Architecture & Design

### Core Architecture
- [Architecture Decisions](architecture/ARCHITECTURE_DECISIONS.md) - Key design choices and rationale
- [Comprehensive Analysis](architecture/COMPREHENSIVE_ANALYSIS.md) - Deep dive into the codebase
- [Component Overview](architecture/components.md) - Understanding the system components
- [Middleware Pipeline](architecture/middleware.md) - Extensibility through middleware

### Design Patterns
- [Parser Design](architecture/parser-design.md) - Command parsing architecture
- [Handler Pattern](architecture/handler-pattern.md) - Protocol and option handlers
- [Builder Pattern](architecture/builder-pattern.md) - Fluent API design

---

## Development Guide

### Contributing
- [Contribution Guidelines](development/contributing.md) - How to contribute to CurlDotNet
- [Commit Instructions](development/COMMIT_INSTRUCTIONS.md) - Git commit standards
- [Development Setup](development/setup.md) - Setting up your development environment

### Building & Testing
- [Build Instructions](development/building.md) - How to build the project
- [Testing Guide](development/testing.md) - Running and writing tests
- [Benchmarks](development/benchmarks.md) - Performance testing

### Release Process
- [NuGet Publishing](development/NUGET_PUBLISHING.md) - Publishing to NuGet
- [Release Notes](development/release-notes.md) - Version history
- [Continuing Work](development/CONTINUING_WORK.md) - Ongoing development tasks

### Analysis & Planning
- [Exploration Summary](development/EXPLORATION_SUMMARY.txt) - Initial project analysis
- [Improvement Plan](../IMPROVEMENT_PLAN.md) - Roadmap for enhancements

---

## API Reference

### Core APIs
- [CurlClient](api/curl-client.md) - Main client interface
- [CurlOptions](api/curl-options.md) - Configuration options
- [CurlResponse](api/curl-response.md) - Response handling

### Extension Points
- [Middleware API](api/middleware.md) - Creating custom middleware
- [Handler API](api/handlers.md) - Protocol handler interface
- [Parser Extensions](api/parser-extensions.md) - Extending the command parser

### Generated Documentation
- [Full API Documentation](api/index.md) - Complete API reference (DocFX generated)

---

## Examples & Tutorials

### Basic Examples
- [Simple GET Request](examples/simple-get.md) - Basic HTTP GET
- [POST with JSON](examples/post-json.md) - Sending JSON data
- [File Upload](examples/file-upload.md) - Uploading files
- [Authentication](examples/authentication.md) - Various auth methods

### Advanced Tutorials
- [Custom Middleware](tutorials/custom-middleware.md) - Building middleware components
- [Error Recovery](tutorials/error-recovery.md) - Handling failures gracefully
- [Performance Optimization](tutorials/performance.md) - Optimizing requests
- [Cross-Platform Usage](tutorials/cross-platform.md) - Platform-specific considerations

### Real-World Scenarios
- [API Integration](tutorials/api-integration.md) - Working with REST APIs
- [File Transfer](tutorials/file-transfer.md) - FTP and file operations
- [Batch Operations](tutorials/batch-operations.md) - Multiple requests
- [Testing Strategies](tutorials/testing-strategies.md) - Testing with CurlDotNet

---

## 🔍 Index

### By Feature
- **HTTP Operations**: [GET](examples/simple-get.md), [POST](examples/post-json.md), [PUT](examples/put-update.md), [DELETE](examples/delete.md)
- **Authentication**: [Basic](examples/auth-basic.md), [Bearer](examples/auth-bearer.md), [OAuth](examples/auth-oauth.md)
- **Protocols**: [HTTP/HTTPS](tutorials/http-protocol.md), [FTP/FTPS](tutorials/ftp-protocol.md), [FILE](tutorials/file-protocol.md)
- **Data Formats**: [JSON](examples/json-handling.md), [XML](examples/xml-handling.md), [Form Data](examples/form-data.md)

### By Use Case
- **REST APIs**: [Guide](tutorials/rest-apis.md) | [Examples](examples/rest-examples.md)
- **File Operations**: [Guide](tutorials/file-operations.md) | [Examples](examples/file-examples.md)
- **Web Scraping**: [Guide](tutorials/web-scraping.md) | [Examples](examples/scraping-examples.md)
- **Testing**: [Guide](tutorials/testing-guide.md) | [Examples](examples/test-examples.md)

### By Skill Level
- **Beginners**: Start with [Getting Started](tutorials/getting-started.md) → [Basic Usage](tutorials/basic-usage.md) → [Simple Examples](examples/README.md)
- **Intermediate**: Explore [Architecture](architecture/components.md) → [Advanced Features](tutorials/advanced-features.md) → [Custom Middleware](tutorials/custom-middleware.md)
- **Advanced**: Dive into [Contributing](development/contributing.md) → [Parser Design](architecture/parser-design.md) → [Performance](tutorials/performance.md)

---

## 📖 Additional Resources

### External Links
- [GitHub Repository](https://github.com/your-org/curl-dot-net)
- [NuGet Package](https://www.nuget.org/packages/CurlDotNet)
- [Issue Tracker](https://github.com/your-org/curl-dot-net/issues)
- [Discussions](https://github.com/your-org/curl-dot-net/discussions)

### Related Projects
- [curl Official Documentation](https://curl.se/docs/)
- [.NET HttpClient Documentation](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient)

### Support
- [FAQ](tutorials/faq.md) - Frequently asked questions
- [Troubleshooting](tutorials/troubleshooting.md) - Common issues and solutions
- [Community Support](tutorials/community.md) - Getting help from the community

---

## 🔄 Navigation

[← Back to Root README](../README.md) | [Architecture →](architecture/README.md) | [Development →](development/README.md) | [Examples →](examples/README.md)

---

*Last Updated: November 2024 | Version: 1.0.0*