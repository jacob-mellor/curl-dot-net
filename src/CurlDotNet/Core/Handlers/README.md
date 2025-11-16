# Protocol Handlers

This directory contains protocol-specific handlers that implement the actual request execution for different URL schemes.

## Purpose

Protocol handlers implement the low-level details of making requests for specific protocols. Each handler implements the `IProtocolHandler` interface and is responsible for executing requests according to curl's behavior for that protocol.

## Handlers

### HTTP Handler

**`HttpHandler.cs`** - Handles HTTP and HTTPS requests. Implements:
- GET, POST, PUT, DELETE, PATCH, HEAD methods
- Request headers (including user-agent, content-type, authorization)
- Request body (raw data, JSON, form data)
- Response headers and status codes
- Redirect handling (following redirects based on options)
- SSL/TLS certificate validation
- Timeout handling
- Authentication (Basic, Bearer, Digest)

**Implementation Approach**: Transpiles curl's HTTP protocol logic from C source code. Uses `HttpClient` only as a transport layer, implementing curl's specific behavior (header handling, redirect logic, error mapping) in pure .NET.

### FTP Handler

**`FtpHandler.cs`** - Handles FTP and FTPS requests. Implements:
- FTP file transfers (upload/download)
- FTP directory listings
- FTPS (FTP over SSL/TLS)
- Authentication (username/password)
- Passive/active mode selection

**Implementation Approach**: Transpiles curl's FTP protocol logic from C source code, using .NET's FTP capabilities as the transport layer.

### File Handler

**`FileHandler.cs`** - Handles file:// protocol requests. Implements:
- Local file reading via file:// URLs
- File path resolution
- Error handling for missing files

**Implementation Approach**: Pure .NET file I/O, matching curl's file:// protocol behavior.

## Design Principles

1. **Protocol Fidelity** - Each handler matches curl's behavior for that protocol exactly
2. **Transpilation** - Logic is transpiled from curl's C source, not re-implemented from scratch
3. **Transport Layer Abstraction** - Use .NET's built-in capabilities (HttpClient, FtpWebRequest) as transport only
4. **Error Mapping** - Map .NET exceptions to curl error codes accurately

## Extending

To add a new protocol handler:

1. Implement `IProtocolHandler` interface
2. Register the handler in `CurlEngine` for the appropriate URL scheme
3. Ensure the handler matches curl's behavior for that protocol
4. Add comprehensive tests

## See Also

- [IProtocolHandler Interface](../Interfaces/README.md)
- [Core Engine](../README.md)

