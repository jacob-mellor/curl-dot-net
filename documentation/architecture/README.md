# Architecture Documentation

This section contains detailed information about the architecture and design of CurlDotNet.

## 📑 Table of Contents

### Core Documents
- [Architecture Decisions](ARCHITECTURE_DECISIONS.md) - Key architectural choices and their rationale
- [Comprehensive Analysis](COMPREHENSIVE_ANALYSIS.md) - In-depth analysis of the codebase structure

### Design Patterns & Components
- [Component Overview](components.md) - System components and their interactions
- [Parser Design](parser-design.md) - How curl commands are parsed and processed
- [Handler Pattern](handler-pattern.md) - Protocol and option handler architecture
- [Middleware Pipeline](middleware.md) - Request/response middleware system
- [Builder Pattern](builder-pattern.md) - Fluent API implementation

## 🏗️ System Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                      CurlDotNet API Layer                    │
│  ┌─────────────┐  ┌──────────────┐  ┌──────────────────┐  │
│  │ String API  │  │ Fluent API   │  │ Object-Oriented  │  │
│  └─────────────┘  └──────────────┘  └──────────────────┘  │
└─────────────────────────────┬───────────────────────────────┘
                              │
┌─────────────────────────────▼───────────────────────────────┐
│                       Command Engine                         │
│  ┌──────────────┐  ┌──────────────┐  ┌────────────────┐   │
│  │    Parser    │  │   Validator  │  │    Builder     │   │
│  └──────────────┘  └──────────────┘  └────────────────┘   │
└─────────────────────────────┬───────────────────────────────┘
                              │
┌─────────────────────────────▼───────────────────────────────┐
│                      Handler System                          │
│  ┌──────────────┐  ┌──────────────┐  ┌────────────────┐   │
│  │ HTTP Handler │  │ FTP Handler  │  │ File Handler   │   │
│  └──────────────┘  └──────────────┘  └────────────────┘   │
└─────────────────────────────┬───────────────────────────────┘
                              │
┌─────────────────────────────▼───────────────────────────────┐
│                    Middleware Pipeline                       │
│  ┌──────────────┐  ┌──────────────┐  ┌────────────────┐   │
│  │   Logging    │→ │    Auth      │→ │     Retry      │   │
│  └──────────────┘  └──────────────┘  └────────────────┘   │
└─────────────────────────────┬───────────────────────────────┘
                              │
┌─────────────────────────────▼───────────────────────────────┐
│                      .NET HttpClient                         │
└───────────────────────────────────────────────────────────────┘
```

## 🎯 Key Design Principles

### 1. Separation of Concerns
Each component has a single, well-defined responsibility:
- **Parser**: Converts curl commands to structured data
- **Handlers**: Execute protocol-specific operations
- **Middleware**: Cross-cutting concerns (logging, auth, retry)

### 2. Extensibility
The system is designed to be extended without modification:
- New protocols via handler interface
- Custom middleware components
- Parser extensions for new curl options

### 3. Testability
All components are independently testable:
- Dependency injection throughout
- Interface-based design
- Mock-friendly architecture

### 4. Performance
Optimized for minimal overhead:
- Stream-based processing
- Lazy evaluation where possible
- Connection pooling

## 🔗 Quick Links

### Related Documentation
- [Development Guide](../development/README.md) - Contributing and building
- [API Reference](../api/README.md) - Detailed API documentation
- [Examples](../examples/README.md) - Code examples and tutorials

### Key Files to Review
- [`src/CurlDotNet/Core/Engine.cs`](../../src/CurlDotNet/Core/Engine.cs) - Main processing engine
- [`src/CurlDotNet/Parser/CommandParser.cs`](../../src/CurlDotNet/Parser/CommandParser.cs) - Command parsing logic
- [`src/CurlDotNet/Core/Handlers/`](../../src/CurlDotNet/Core/Handlers/) - Protocol handlers
- [`src/CurlDotNet/Middleware/`](../../src/CurlDotNet/Middleware/) - Middleware components

## 📊 Architecture Metrics

| Metric | Value | Target |
|--------|-------|--------|
| Code Coverage | TBD | >80% |
| Cyclomatic Complexity | <10 | <15 |
| Coupling | Low | Low |
| Cohesion | High | High |
| Lines of Code | ~10,000 | - |
| Number of Classes | ~100 | - |

## 🔄 Navigation

[← Back to Documentation](../README.md) | [Development →](../development/README.md) | [API Reference →](../api/README.md) | [Examples →](../examples/README.md)

---

*Last Updated: November 2024*