# Releasing CurlDotNet

This document describes the process for releasing new versions of CurlDotNet to NuGet.

## Prerequisites

1. **NuGet API Key**: You need a valid NuGet.org API key with push permissions for the CurlDotNet package
2. **Git Tags**: Release versions are managed through git tags
3. **Clean Working Directory**: Ensure `git status` shows no uncommitted changes

## Release Process

### Option 1: Automated Release via GitHub Actions (Recommended)

The simplest way to release is using the automated GitHub Actions workflow:

1. **Update Version Number**
   ```bash
   # Edit src/CurlDotNet/CurlDotNet.csproj
   # Update <Version>, <AssemblyVersion>, and <FileVersion> tags
   ```

2. **Update Release Notes**
   - Add changes to the package description or create a CHANGELOG.md entry
   - Commit these changes

3. **Create and Push Tag**
   ```bash
   # Create a tag matching the version
   git tag v1.2.0
   git push origin v1.2.0
   ```

4. **Monitor Release**
   - The `publish.yml` workflow will automatically:
     - Run tests
     - Build the package
     - Publish to NuGet.org
   - Check Actions tab on GitHub for progress

### Option 2: Manual Release

For manual releases or testing:

1. **Test and Build Locally**
   ```bash
   # Run full build and test cycle
   ./scripts/build-and-test.sh
   ```

2. **Create Package**
   ```bash
   # Create package with current version
   ./scripts/pack-nuget.sh

   # Or specify a version
   ./scripts/pack-nuget.sh --version 1.2.0

   # For dry run (no publish)
   ./scripts/pack-nuget.sh --dry-run
   ```

3. **Test Package Locally**
   ```bash
   # Create a test project
   mkdir test-package && cd test-package
   dotnet new console

   # Add the local package
   dotnet add package CurlDotNet --source ../src/CurlDotNet/bin/Release/
   ```

4. **Publish to NuGet**
   ```bash
   # Set your API key (or use --api-key parameter)
   dotnet nuget setApiKey YOUR_API_KEY

   # Push the package
   dotnet nuget push src/CurlDotNet/bin/Release/*.nupkg \
     --source https://api.nuget.org/v3/index.json \
     --skip-duplicate
   ```

## Version Numbering

We follow [Semantic Versioning](https://semver.org/):

- **Major** (X.0.0): Breaking API changes
- **Minor** (1.X.0): New features, backward compatible
- **Patch** (1.1.X): Bug fixes, backward compatible

## Pre-Release Checklist

- [ ] All tests pass (`dotnet test`)
- [ ] Documentation is up to date
- [ ] XML documentation builds without errors
- [ ] CHANGELOG or release notes updated
- [ ] Version numbers updated in .csproj
- [ ] No security vulnerabilities in dependencies
- [ ] README examples work with new version

## Post-Release Checklist

- [ ] Verify package appears on NuGet.org
- [ ] Test installation in a clean project
- [ ] Create GitHub Release with notes
- [ ] Update documentation if needed
- [ ] Announce release (if applicable)

## Rollback Procedure

If a release has critical issues:

1. **Unlist Package** (doesn't delete, just hides from search)
   - Go to NuGet.org package page
   - Click "Manage Package"
   - Unlist the problematic version

2. **Fix Issues**
   - Create fixes on a hotfix branch
   - Thoroughly test changes

3. **Release Patch Version**
   - Increment patch version
   - Follow normal release process

## GitHub Secrets Setup

For automated releases, ensure these secrets are configured:

1. Go to Settings → Secrets → Actions
2. Add `NUGET_API_KEY` with your NuGet.org API key

## Troubleshooting

### Package Build Fails
- Check XML documentation is enabled
- Verify all target frameworks build
- Run `dotnet clean` and retry

### Tests Fail in CI
- Check platform-specific tests (Windows vs Linux/Mac)
- Verify test dependencies are restored

### NuGet Push Fails
- Verify API key is valid
- Check package ID isn't taken
- Ensure version doesn't already exist

## Contact

For release access or issues, contact the maintainers through GitHub Issues.