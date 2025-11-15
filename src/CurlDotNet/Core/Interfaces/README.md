# Core Interfaces

This directory contains the core interfaces that define contracts for key components.

## Purpose

Interfaces provide abstraction and enable testability, extensibility, and clean separation of concerns.

## Interfaces

### IProtocolHandler

**`IProtocolHandler.cs`** - Contract for protocol-specific handlers. Defines:
- `Task<CurlResult> ExecuteAsync(CurlOptions options, CancellationToken cancellationToken)` - Execute a request for the protocol
- `bool CanHandle(string url)` - Determine if this handler can handle a URL scheme

**Implemented By**:
- `HttpHandler` - HTTP/HTTPS protocol
- `FtpHandler` - FTP/FTPS protocol
- `FileHandler` - file:// protocol

**Purpose**: Enables extensibility for new protocols and testability through mocking.

### ICommandParser

**`ICommandParser.cs`** - Contract for command parsers. Defines:
- `CurlOptions Parse(string curlCommand)` - Parse a curl command string into structured options
- `bool TryParse(string curlCommand, out CurlOptions options, out string? error)` - Try to parse with error information

**Implemented By**:
- `CommandParser` - Main command parser implementation

**Purpose**: Enables alternative parsing strategies and testability.

## Design Principles

1. **Minimal Surface Area** - Keep interfaces focused and simple
2. **Testability** - Enable easy mocking in unit tests
3. **Extensibility** - Allow new implementations without breaking existing code
4. **Single Responsibility** - Each interface has one clear purpose

