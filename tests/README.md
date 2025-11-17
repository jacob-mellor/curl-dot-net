# Tests

This folder contains the full test suite for CurlDotNet.

## Test Results

**✅ 244 Tests - 100% Pass Rate**
- Execution Time: ~33 seconds
- No external dependencies required
- Tests run anywhere .NET runs

## 🚀 Local HTTP Test Server

We use a **self-contained HTTP test server** that runs locally within each test, eliminating external dependencies and ensuring 100% reliability.

### Why We Built Our Own

- **No Docker Required** - Tests run without containers or external tools
- **No Network Dependencies** - No failures from rate limiting or service outages
- **100% Reliable** - Deterministic behavior every run
- **Fast** - Localhost speed, no network latency
- **CI/CD Friendly** - Works perfectly in all build pipelines

### How It Works

```csharp
// Each test starts its own isolated server
using var server = new LocalTestHttpServer();
server.Start();
await Task.Delay(50); // Give server time to start

// Execute curl commands against the local server
var command = $"curl {server.BaseUrl}/get?param=value";
var result = await Curl.ExecuteAsync(command);
```

### Supported Endpoints

Our local server implements httpbin-compatible endpoints:
- `/get` - Returns GET request data
- `/post` - Echoes POST data
- `/put` - Handles PUT requests
- `/headers` - Returns request headers
- `/cookies` - Parses cookie data
- `/anything` - Returns full request details

## Structure

- `CurlDotNet.Tests/` – xUnit test project covering all functionality
- `CurlDotNet.Tests/TestServers/` – Local HTTP test server implementation
- `test.ps1` / `test.sh` – Cross-platform test runners

## Running Tests

```bash
# Run all tests
dotnet test CurlDotNet.sln --configuration Release

# Run specific categories
dotnet test --filter Category=Integration
dotnet test --filter Category=Http

# Run offline tests only (no network)
dotnet test --filter OnlineRequired=false
```

For coverage:

```bash
dotnet test CurlDotNet.sln --configuration Release /p:CollectCoverage=true
```

## Test Categories

- **Command Parsing** (85+ tests) - Validates all curl options
- **HTTP Operations** (50+ tests) - All HTTP methods and features
- **Integration** (25+ tests) - End-to-end scenarios with local server
- **Synthetic** (45+ tests) - .NET-specific edge cases
- **Error Handling** (35+ tests) - Exception validation

## Framework Compatibility

Tests verified on:
- .NET 8.0, 9.0, 10.0
- .NET Framework 4.7.2, 4.8 (via .NET Standard 2.0)
- .NET Standard 2.0

No Mono required - we use .NET Standard 2.0 for cross-framework compatibility.

## Goals

- Maintain 100% test pass rate
- Include synthetic command parser tests and command-line comparison tests to ensure parity with the native curl binary
- Keep adding real-world use case tests as described in `REAL_WORLD_EXAMPLES.md`

