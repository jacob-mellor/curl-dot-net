# .NET 10 Configuration Summary

## ✅ Configuration Complete

The project is now configured to:
- **Build with .NET 10 locally** on your Mac
- **Exclude .NET 10 from CI/CD** to maintain stability
- **Support .NET Framework 4.7.2 & 4.8** via Windows GitHub Actions

## How It Works

### 1. Automatic Environment Detection

The `CurlDotNet.csproj` file now detects if it's running in GitHub Actions:

```xml
<!-- Detect if running in GitHub Actions -->
<IsGitHubActions Condition="'$(GITHUB_ACTIONS)' == 'true'">true</IsGitHubActions>
```

### 2. Conditional Target Frameworks

Based on environment, different frameworks are built:

**Local Mac (your machine):**
- netstandard2.0
- net8.0
- net9.0
- **net10.0** ✅

**GitHub Actions (CI/CD):**
- netstandard2.0
- net8.0
- net9.0
- ~~net10.0~~ ❌ (excluded for stability)

**Windows (adds .NET Framework):**
- All of the above
- net472
- net48

## Testing the Configuration

### Quick Test
```bash
# Local build (includes .NET 10)
dotnet build --configuration Release

# Simulate CI (excludes .NET 10)
GITHUB_ACTIONS=true dotnet build --configuration Release
```

### Interactive Build Tool
```bash
# Choose which frameworks to build
dotnet script scripts/build-local.csx
```

### Test GitHub Actions Locally
```bash
# If you have Docker running
.github/devops/test-actions-locally.sh
```

## Why This Configuration?

1. **Future-ready**: You can use .NET 10 features locally
2. **CI Stability**: GitHub Actions remain 100% stable without preview SDKs
3. **Complete Packages**: NuGet packages built on Windows include all frameworks
4. **Developer Freedom**: Choose which frameworks to build during development

## Files Modified

1. **src/CurlDotNet/CurlDotNet.csproj** - Added environment detection
2. **scripts/build-local.csx** - Interactive framework selector
3. **.github/devops/** - Docker/DevOps testing tools (properly organized)

## Important Notes

- Never manually add .NET 10 to GitHub Actions workflows
- The configuration automatically handles framework selection
- Windows builds in GitHub Actions provide complete NuGet packages
- Local builds on Mac skip .NET Framework targets (expected)