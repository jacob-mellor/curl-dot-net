# Tests

This folder contains the full test suite for CurlDotNet.

## Structure

- `CurlDotNet.Tests/` – xUnit test project covering the command parser, protocol handlers, fluent builder, middleware, and integration scenarios.
- `test.ps1` / `test.sh` – cross-platform helpers that run the full test suite, code coverage, and any shell-based parity checks.

## Running Tests

```bash
dotnet test CurlDotNet.sln --configuration Release
```

For coverage:

```bash
dotnet test CurlDotNet.sln --configuration Release /p:CollectCoverage=true
```

## Goals

- Maintain 90%+ line, branch, and method coverage.
- Include synthetic command parser tests and command-line comparison tests to ensure parity with the native curl binary.
- Keep adding real-world use case tests as described in `REAL_WORLD_EXAMPLES.md`.

