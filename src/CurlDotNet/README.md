# CurlDotNet Source Code

This directory contains the core source code for CurlDotNet, a pure .NET implementation of curl.

## Purpose

CurlDotNet allows .NET developers to execute curl commands directly in their C# applications without needing the curl binary. This is a complete, native .NET implementation that transpiles curl's C source code logic, providing the same behavior with pure .NET performance and compatibility.

## Directory Structure

- **`Core/`** - Core components including command parsing, engine, options, request builder, and protocol handlers
- **`Exceptions/`** - Exception hierarchy matching curl error codes
- **`Lib/`** - Object-oriented LibCurl API for programmatic usage
- **`Middleware/`** - Middleware pipeline system for request/response interception

## Design Principles

1. **Simplicity** - Keep the codebase clean and focused. No feature bloat.
2. **Developer-Friendly** - Intuitive APIs that are discoverable through IntelliSense
3. **Well-Documented** - Every public API has comprehensive XML documentation with examples
4. **Native .NET** - Pure .NET implementation, transpiled from curl's C source code logic

## Key Files

- **`Curl.cs`** - Main entry point for executing curl commands
- **`DotNetCurl.cs`** - Alternative API names for familiarity
- **`Curl_SyncAsync.cs`** - Synchronous wrappers for async methods
- **`Core/CommandParser.cs`** - Parses curl command strings into structured options
- **`Core/CurlEngine.cs`** - Core engine that executes parsed commands
- **`Core/CurlResult.cs`** - Result object with utility methods for response handling
- **`Core/CurlRequestBuilder.cs`** - Fluent API for building requests programmatically

## Transpilation Approach

CurlDotNet implements curl's behavior by transpiling logic from the original curl C source code. We use `HttpClient` only as a transport layer, implementing curl's protocol handling, authentication, redirect logic, and error handling in pure .NET.

## See Also

- [Core Components](../Core/README.md)
- [Exception Hierarchy](../Exceptions/README.md)
- [LibCurl API](../Lib/README.md)
- [Middleware System](../Middleware/README.md)

