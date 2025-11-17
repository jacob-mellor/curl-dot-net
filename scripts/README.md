# Scripts Documentation

This folder contains all scripts for building, testing, and deploying CurlDotNet.

## Active Scripts

### Core Workflow Scripts
- **`ship-it.sh`** - Main deployment script with auto-version bump
- **`test-all-locally.sh`** - Comprehensive local testing before push
- **`test-local.sh`** - Quick local test runner
- **`smoke-test.sh`** - 30-second validation test

### Documentation Scripts
- **`generate-docs.sh`** - Generate comprehensive documentation with DefaultDocumentation

### Utility Scripts
- **`sync-upstream.sh`** - Sync with upstream fork if needed

## Deployment Process

### Local Development
1. Make changes
2. Run `./scripts/test-local.sh` for quick validation
3. Run `./scripts/test-all-locally.sh` for comprehensive testing
4. Documentation is auto-generated during tests

### Releasing
1. Run `./scripts/ship-it.sh` - This will:
   - Bump version automatically
   - Run all tests
   - Generate documentation
   - Create commit
   - Push to dev branch

2. Create PR from dev → master
3. GitHub workflow automatically:
   - Builds documentation in gh-pages folder
   - Deploys to GitHub Pages from gh-pages branch
   - Publishes to NuGet

## Documentation Workflow

Documentation is generated at multiple points:
- **Locally**: During `test-all-locally.sh` and `ship-it.sh`
- **CI/CD**: GitHub workflow builds from gh-pages folder and deploys to gh-pages branch
- **GitHub Pages**: Serves from gh-pages branch with Jekyll

The documentation pipeline:
1. XML comments in C# code
2. DefaultDocumentation generates markdown in gh-pages folder
3. GitHub workflow deploys from gh-pages folder to gh-pages branch
4. GitHub Pages serves the gh-pages branch with Jekyll

## Archived Scripts

Old or experimental scripts are in the `old/` subfolder for reference.