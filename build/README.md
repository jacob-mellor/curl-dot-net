# Build Information

## Build Status
✅ **Build: Passing** - All builds are successful across all target frameworks.

## Supported Frameworks
- **.NET 10.0** ✨ (Latest)
- .NET 9.0
- .NET 8.0
- .NET 7.0
- .NET 6.0
- .NET Core 3.1
- .NET Standard 2.1
- .NET Standard 2.0
- .NET Framework 4.8
- .NET Framework 4.7.2

## Build Configuration
CurlDotNet uses a standard MSBuild configuration with multi-targeting to support a wide range of .NET frameworks.

### Building from Source
```bash
# Build all projects
dotnet build -c Release

# Build specific project
dotnet build src/CurlDotNet/CurlDotNet.csproj -c Release

# Run tests
dotnet test -c Release
```

### Build Scripts
The `scripts/` directory contains shell scripts for various build operations:
- `build-all.sh` - Build all projects and documentation
- `build-and-test.sh` - Build and run all tests with coverage
- `build-docs.sh` - Build DocFX documentation
- `pack-nuget.sh` - Create NuGet package

## Continuous Integration
We use GitHub Actions for CI/CD:
- **Build & Test** - On every push and PR
- **Documentation** - Deploys to GitHub Pages on master branch
- **NuGet Release** - Publishes to NuGet.org on version tags

## Build Artifacts
- **NuGet Package**: `CurlDotNet.*.nupkg`
- **Documentation**: `docs/_site/`
- **Test Results**: `test-results/`
- **Code Coverage**: `coverage/`

## Icon Assets
- `icon-128.png` - 128x128 logo for README display
- Source icon available at `src/CurlDotNet/icon.png` (1024x1024)

## Quality Standards
- ✅ 95% test coverage (228/240 tests passing)
- ✅ Zero compiler warnings
- ✅ All documentation builds without errors
- ✅ NuGet package validation passes