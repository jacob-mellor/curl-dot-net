# Core Components

This directory contains the core components of CurlDotNet that handle command parsing, execution, and protocol-specific logic.

## Purpose

The core components provide the foundation for CurlDotNet's functionality: parsing curl commands, executing requests, and handling different protocols (HTTP, FTP, File).

## Components

### Command Parser

**`CommandParser.cs`** - Parses curl command strings into structured `CurlOptions` objects. Handles:
- URL extraction and validation
- HTTP method detection (GET, POST, PUT, DELETE, PATCH, HEAD)
- Header parsing with proper quoting and escaping
- Authentication (Basic, Bearer, Digest)
- Data/body parsing (raw, JSON, form data)
- Output options (file, stdout, null)
- Behavior options (redirects, verbose, silent, include headers)
- SSL/TLS options
- Timeout configuration
- Complex command parsing across different shells (Windows, ZSH, Linux, Mac)

### Curl Engine

**`CurlEngine.cs`** - Core engine that executes parsed curl commands. Coordinates:
- Option validation
- Protocol handler selection (HTTP, FTP, File)
- Middleware pipeline execution
- Error handling and exception mapping
- Result construction

### Curl Options

**`CurlOptions.cs`** - Data structure representing all parsed options from a curl command. Provides:
- Immutable option storage
- Cloning for middleware modification
- Validation support

### Curl Result

**`CurlResult.cs`** - Result object returned from curl operations. Provides:
- Status code and response body access
- Header access and parsing
- JSON parsing utilities (`ParseJson<T>`, `AsJson<T>`, `AsJsonDynamic`)
- File saving (`SaveToFile`, `SaveToFileAsync`, `SaveAsJson`)
- Validation helpers (`EnsureSuccess`, `EnsureStatus`, `EnsureContains`)

### Curl Request Builder

**`CurlRequestBuilder.cs`** - Fluent API for programmatically building requests. Provides:
- Method chaining for intuitive API
- Type-safe header and data setting
- IntelliSense-friendly discoverability
- Conversion to curl command strings (`ToCurlCommand`)

### Curl Settings

**`CurlSettings.cs`** - .NET-specific configuration settings. Provides:
- Fluent API for .NET-specific options
- Integration with .NET patterns (async/await, cancellation tokens)

## Protocol Handlers

See [Handlers/README.md](./Handlers/README.md) for protocol-specific implementations:
- **`Handlers/HttpHandler.cs`** - HTTP/HTTPS protocol handler
- **`Handlers/FtpHandler.cs`** - FTP/FTPS protocol handler
- **`Handlers/FileHandler.cs`** - File protocol handler (file://)

## Interfaces

See [Interfaces/README.md](./Interfaces/README.md) for protocol handler interfaces:
- **`Interfaces/IProtocolHandler.cs`** - Protocol handler contract
- **`Interfaces/ICommandParser.cs`** - Command parser contract

## Design Principles

1. **Separation of Concerns** - Each component has a single, well-defined responsibility
2. **Testability** - All components are designed for easy unit testing
3. **Extensibility** - Protocol handlers and middleware can be extended
4. **Simplicity** - Keep complexity hidden from the developer-facing API

