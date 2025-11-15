# NuGet Publishing Instructions

This guide explains how to publish CurlDotNet to NuGet.org.

## Prerequisites

1. **NuGet API Key**
   - Go to https://www.nuget.org/account/apikeys
   - Sign in with your Microsoft account (or create one)
   - Click "Create" to create a new API key
   - Name it (e.g., "CurlDotNet Publishing")
   - Set expiration (recommended: 1 year, or custom)
   - Select scope: "Select scopes" → Check "Push new packages and package versions"
   - Click "Create"
   - **Copy and save the API key** (you won't see it again!)

2. **Verify Package**
   - Run tests: `dotnet test`
   - Build package: `dotnet pack -c Release`
   - Inspect the `.nupkg` file in `bin/Release/`

## Publishing Steps

### Option 1: Command Line (Recommended)

1. **Navigate to the project directory:**
   ```bash
   cd src/CurlDotNet
   ```

2. **Build and pack the release version:**
   ```bash
   dotnet pack -c Release
   ```
   This creates a `.nupkg` file in `bin/Release/`

3. **Publish to NuGet.org:**
   ```bash
   dotnet nuget push bin/Release/CurlDotNet.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
   ```
   Replace `YOUR_API_KEY` with your actual API key.

4. **Verify the publish:**
   - Visit https://www.nuget.org/packages/CurlDotNet
   - The package should appear within a few minutes

### Option 2: Using NuGet.exe

1. **Install NuGet CLI** (if not already installed):
   ```bash
   # macOS/Linux
   brew install nuget
   
   # Or download from https://www.nuget.org/downloads
   ```

2. **Configure API key:**
   ```bash
   nuget setApiKey YOUR_API_KEY -Source https://api.nuget.org/v3/index.json
   ```
   This saves the API key for future use.

3. **Pack and publish:**
   ```bash
   cd src/CurlDotNet
   dotnet pack -c Release
   nuget push bin/Release/CurlDotNet.1.0.0.nupkg -Source https://api.nuget.org/v3/index.json
   ```

### Option 3: Visual Studio

1. Right-click the `CurlDotNet` project in Solution Explorer
2. Select "Pack"
3. Right-click again → "Publish" → "NuGet"
4. Enter your API key
5. Click "Publish"

## Testing Before Publishing

### Test Package Locally

1. **Create a local NuGet feed:**
   ```bash
   mkdir ~/local-nuget
   ```

2. **Pack to local feed:**
   ```bash
   cd src/CurlDotNet
   dotnet pack -c Release --output ~/local-nuget
   ```

3. **Create a test project and add local feed:**
   ```bash
   # Create test project
   dotnet new console -n TestCurlDotNet
   cd TestCurlDotNet
   
   # Add local NuGet source
   dotnet nuget add source ~/local-nuget --name local
   
   # Install package
   dotnet add package CurlDotNet --source local
   ```

4. **Test the package works:**
   ```csharp
   using CurlDotNet;
   
   var result = await Curl.ExecuteAsync("curl https://httpbin.org/get");
   Console.WriteLine(result.Body);
   ```

### Verify Package Contents

After packing, inspect the `.nupkg` file:

```bash
# macOS/Linux - unzip and inspect
unzip -l bin/Release/CurlDotNet.1.0.0.nupkg

# Should contain:
# - CurlDotNet.dll
# - CurlDotNet.pdb
# - CurlDotNet.xml (documentation)
# - LICENSE
# - README.md
# - [Content_Types].xml
# - CurlDotNet.nuspec
```

## Versioning

### Semantic Versioning

Follow [Semantic Versioning](https://semver.org/): `MAJOR.MINOR.PATCH`

- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

### Update Version

Edit `src/CurlDotNet/CurlDotNet.csproj`:

```xml
<Version>1.0.1</Version>
<AssemblyVersion>1.0.1.0</AssemblyVersion>
<FileVersion>1.0.1.0</FileVersion>
```

Update `PackageReleaseNotes` to describe what changed.

## Publishing Checklist

- [ ] All tests pass: `dotnet test`
- [ ] Code coverage meets target (90%+)
- [ ] Version number updated
- [ ] Release notes updated
- [ ] README.md is up to date
- [ ] LICENSE file is included
- [ ] XML documentation is complete
- [ ] Package builds successfully: `dotnet pack -c Release`
- [ ] Package tested locally
- [ ] API key is valid
- [ ] Repository URL is correct in .csproj

## Troubleshooting

### "API key is invalid"
- Verify the API key is correct
- Check the API key hasn't expired
- Ensure you copied the entire key

### "Package already exists"
- Update the version number
- Cannot overwrite existing versions

### "Package validation failed"
- Check package size (should be < 250MB)
- Verify all required files are included
- Check for invalid characters in package metadata

### "Symbols package failed"
- Ensure `<IncludeSymbols>true</IncludeSymbols>` is set
- Symbol package is optional but recommended

## After Publishing

1. **Verify on NuGet.org:**
   - Visit https://www.nuget.org/packages/CurlDotNet
   - Check the package page loads correctly
   - Verify README renders properly

2. **Test installation:**
   ```bash
   dotnet new console -n TestInstall
   cd TestInstall
   dotnet add package CurlDotNet
   dotnet restore
   ```

3. **Update documentation:**
   - Update README with new version
   - Create release notes on GitHub
   - Tag the release: `git tag v1.0.0 && git push --tags`

## Automatic Publishing with GitHub Actions

You can set up automatic publishing when tags are pushed:

1. Create `.github/workflows/publish.yml`
2. Configure to trigger on `v*` tags
3. Use GitHub Secrets to store the NuGet API key

Example workflow (create this separately if desired):

```yaml
name: Publish to NuGet

on:
  push:
    tags:
      - 'v*'

jobs:
  publish:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - name: Pack
        run: dotnet pack -c Release
      - name: Publish
        run: dotnet nuget push bin/Release/*.nupkg --api-key ${{ secrets.NUGET_API_KEY }} --source https://api.nuget.org/v3/index.json --skip-duplicate
```

Set `NUGET_API_KEY` in GitHub repository secrets.

## Additional Resources

- [NuGet Package Creation Guide](https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package)
- [NuGet Publishing Guide](https://docs.microsoft.com/en-us/nuget/nuget-org/publish-a-package)
- [Semantic Versioning](https://semver.org/)
- [NuGet CLI Reference](https://docs.microsoft.com/en-us/nuget/reference/nuget-exe-cli-reference)

---

**Note:** After the first publish, the package will be available immediately. Subsequent updates may take a few minutes to appear on NuGet.org.

