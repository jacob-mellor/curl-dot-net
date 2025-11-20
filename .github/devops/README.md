# DevOps Tools and Configuration

This directory contains tools and configurations for local DevOps testing and CI/CD validation.

## Contents

- `act-config.yaml` - Configuration for running GitHub Actions locally with `act`
- `test-actions-locally.sh` - Script to test GitHub Actions workflows locally

## Testing GitHub Actions Locally

To test GitHub Actions workflows without pushing to GitHub:

1. **Install Docker Desktop** (if not already installed):
   ```bash
   brew install --cask docker
   ```

2. **Install act** (if not already installed):
   ```bash
   brew install act
   ```

3. **Run workflows locally**:
   ```bash
   # Test CI smoke tests
   act pull_request --job smoke-integration-test

   # Test NuGet build validation
   act pull_request --job build-nuget-package

   # Use our configuration file
   act --config-file .github/devops/act-config.yaml
   ```

## Why This Exists

- Test GitHub Actions locally before pushing
- Verify workflows work correctly
- Debug CI/CD issues without waiting for GitHub
- Ensure .NET 10 is properly excluded from CI builds

## Important Notes

- These tools are for local testing only
- Production CI/CD runs on GitHub Actions
- Docker is required for `act` to work
- Configuration ensures .NET 10 is excluded in CI simulation