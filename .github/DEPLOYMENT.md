# CurlDotNet Deployment Guide

## Overview

This repository uses GitHub Actions to build and deploy NuGet packages with **100% stability** and **complete framework support** including .NET Framework 4.7.2 and 4.8.

## Workflows

### 1. Smoke Tests (`ci-smoke.yml`)

**Triggers**: On every PR to `main`, `master`, or `dev`

**Purpose**: Validates code works on all platforms

**What it does**:
- ✅ Runs smoke tests on Windows (with .NET Framework 4.7.2 & 4.8)
- ✅ Runs smoke tests on Ubuntu
- ✅ Runs smoke tests on macOS
- ✅ Must pass before any PR can merge

### 2. Production Deployment (`nuget-deploy.yml`)

**Triggers**:
- Manual trigger with options
- Automatic on version tags (e.g., `v1.2.37`)

**Purpose**: Build and deploy production-ready NuGet package

**Features**:
- **Dry Run Mode**: Test everything without actually deploying
- **Manual Approval**: Requires explicit confirmation to deploy
- **Complete Package**: Includes all frameworks (.NET Framework 4.7.2, 4.8, etc.)
- **Automatic Verification**: Tests package before deployment
- **GitHub Release**: Creates release with artifacts

## Setup Requirements

### 1. NuGet API Key

1. Go to https://www.nuget.org/account/apikeys
2. Create a new API Key with:
   - **Key Name**: `CurlDotNet-GitHub-Actions`
   - **Package Owner**: Your NuGet account
   - **Scopes**: `Push` and `Push new packages and package versions`
   - **Packages**: `CurlDotNet` (or use glob pattern `CurlDotNet*`)

3. Add to GitHub Secrets:
   - Go to Settings → Secrets and variables → Actions
   - Click "New repository secret"
   - Name: `NUGET_API_KEY`
   - Value: Your NuGet API key

### 2. Environment Protection (Optional but Recommended)

1. Go to Settings → Environments
2. Create environment: `nuget-production`
3. Add protection rules:
   - Required reviewers (yourself)
   - Restrict deployment to protected branches

## Deployment Process

### For Regular Releases (Recommended)

1. **Create PR**: Make changes in `dev` branch
2. **Automatic Validation**: PR triggers smoke tests on all platforms
3. **Review Results**: Ensure all smoke tests pass (Windows, Ubuntu, Mac)
4. **Merge PR**: Once smoke tests pass
5. **Manual Deploy**:
   - Go to Actions → "Deploy to NuGet"
   - Click "Run workflow"
   - Set options:
     - `Deploy to NuGet.org?`: `false` (first time)
     - `Dry run`: `true`
   - Review dry run results
   - Run again with:
     - `Deploy to NuGet.org?`: `true`
     - `Dry run`: `false`

### For Tagged Releases

1. Create and push a version tag:
   ```bash
   git tag v1.2.37
   git push origin v1.2.37
   ```
2. Workflow automatically:
   - Builds complete package on Windows
   - Deploys to NuGet.org
   - Creates GitHub Release

## Version Management

The version is controlled in `src/CurlDotNet/CurlDotNet.csproj`:

```xml
<Version>1.2.37</Version>
<AssemblyVersion>1.2.37.0</AssemblyVersion>
<FileVersion>1.2.37.0</FileVersion>
```

**IMPORTANT**: Always increment version before deployment!

## Validation Checklist

Before deploying, the workflow automatically verifies:

- [ ] Package builds on Windows (for .NET Framework support)
- [ ] Contains .NET Standard 2.0 assembly
- [ ] Contains .NET Framework 4.7.2 assembly
- [ ] Contains .NET Framework 4.8 assembly
- [ ] Contains .NET 8.0 assembly
- [ ] Contains .NET 9.0 assembly
- [ ] Package installs on Windows
- [ ] Package installs on Ubuntu
- [ ] Package installs on macOS
- [ ] Basic functionality works

## Troubleshooting

### Missing .NET Framework assemblies

**Problem**: Package doesn't contain net472/net48 assemblies

**Solution**: Ensure build runs on Windows. The workflow uses `windows-latest`.

### Version conflicts

**Problem**: NuGet rejects package due to existing version

**Solution**:
1. Increment version in .csproj
2. The workflow uses `--skip-duplicate` to handle re-runs

### Deployment fails

**Problem**: Deployment to NuGet fails

**Check**:
1. NUGET_API_KEY secret is set correctly
2. API key has push permissions
3. Package ID matches key permissions
4. Version doesn't already exist on NuGet.org

## Why This Setup?

1. **100% Stable**: Every deployment is validated before pushing
2. **Complete Package**: Windows build ensures all frameworks included
3. **Manual Control**: You decide when to deploy
4. **Automatic Validation**: Can't accidentally push broken packages
5. **Cross-Platform Testing**: Verified on all major OS
6. **Dry Run Option**: Test everything without risk

## Important Notes

- **NEVER** build NuGet packages on Mac/Linux for production (missing .NET Framework)
- **ALWAYS** use the GitHub Actions workflow for deployment
- **ALWAYS** run dry run first
- **ALWAYS** check PR validation results

## Support

For issues with deployment:
1. Check workflow logs in Actions tab
2. Verify all secrets are set
3. Ensure version is incremented
4. Create an issue if problems persist