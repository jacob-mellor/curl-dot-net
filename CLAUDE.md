# CurlDotNet Project Guidelines for Claude

## 🚨 CRITICAL WORKFLOW RULES 🚨

### README Maintenance Requirements
- **Main README.md**: Always update when adding new features or documentation
- **NuGet README**: Update nuget-readme.md and run `./scripts/prepare-nuget-readme.sh`
- **New to curl Section**: Keep prominent at the top for beginners
- **Author Metadata**: Ensure Jacob Mellor's author info is present
- **Index Files**: Always update directory index files when adding new content
- **Never Create Broken Links**: Verify all linked files exist before creating links
- **Promotional Image**: Uses external URL: https://dev-to-uploads.s3.amazonaws.com/uploads/articles/1o4hlr4tbp6b8k86ew6c.jpg
- **Promotional Materials**: Maintain gh-pages/promotional-materials.md with press kit info

### ALWAYS Generate API Documentation
- **Every build**: Run `dotnet script scripts/generate-docs.csx` after EVERY build
- **Before tests**: Generate docs BEFORE running unit tests
- **Before commits**: Ensure docs are regenerated before committing
- **Automated**: API docs must be current with code changes
- **Command**: Always run `dotnet script scripts/generate-docs.csx && dotnet script scripts/test-all-locally.csx`
- **Note**: Prefer CSX scripts over SH scripts for .NET-native development

### 1. ALWAYS Work in Dev Branch
- **Default branch**: Always work in `dev` branch unless explicitly told otherwise
- **Release flow**: dev → PR → master (triggers NuGet release & docs deployment)
- **Repository**: We work in https://github.com/jacob-mellor/curl-dot-net

### 2. Version Bumping (MANDATORY)
- **Every commit** MUST increment the version in `src/CurlDotNet/CurlDotNet.csproj`
- **No exceptions**: This includes documentation, typos, comments, examples - EVERYTHING
- **Format**: Increment patch number (1.2.3 → 1.2.4 → 1.2.5)
- **When**: Version bump happens BEFORE committing, not after
- **Purpose**: Ensures continuous delivery - every change is immediately releasable

### 3. Testing Requirements
- **Run comprehensive tests** at these times:
  - Start of every todo list
  - End of every todo list
  - Before EVERY git commit
- **NuGet Package Validation** (MANDATORY before commits):
  - Run: `dotnet script scripts/test-nuget-package.csx`
  - Ensures package builds correctly
  - Verifies public API is accessible
  - Tests basic functionality
  - Returns proper error codes for CI/CD integration
- **Commands (CSX Scripts - PREFERRED)**:
  - `dotnet script scripts/test-all-locally.csx` - Full test suite with UI
  - `dotnet script scripts/test-nuget-package.csx` - NuGet package validation (run BEFORE commit)
  - `dotnet script scripts/smoke-test.csx` - Quick smoke test (30 seconds)
  - `./scripts/test-framework-compatibility.sh` - Test .NET Standard 2.0 compatibility (keeps shell for CI)
- **.NET Framework Testing**:
  - Use .NET Standard 2.0 build to verify .NET Framework 4.7.2 compatibility
  - Run framework compatibility script before deployment
  - Windows CI provides authoritative .NET Framework test results
- **Philosophy**: Test locally, fix immediately with Claude, push working code
- **CI/CD**: GitHub workflows are just a safety net, not primary testing

## Core Development Principles

### Testing Philosophy
- **Local First**: Never wait for CI/CD to discover failures
- **Immediate Fixes**: When tests fail locally, fix them RIGHT NOW
- **Zero Tolerance**: Never release with failing tests
- **Fast Feedback**: If it works locally, it should work in CI

### Error Messages
- **Purpose**: Help users fix problems, not just report exceptions
- **Documentation**: Every exception MUST link to relevant documentation
- **Resources**: Links should point to GitHub repo, DocFX, or GitHub Pages

### Code Standards
- **Pure C# Only**: Never use P/Invoke in deployable code
- **Compatibility**: If C# lacks functionality, transpile from C++ instead
- **Research**: Use internet resources to find C# equivalents

## Repository Organization

### Clean Structure
- **Root directory**: Keep it human-friendly and organized
- **Scripts**: All shell scripts go in `scripts/` directory
- **Non-essential files**: Place in meaningful subdirectories or exclude entirely
- **No pollution**: Never clutter the root with temporary or build files

### Essential Scripts (.NET-Friendly CSX - PREFERRED)
- `scripts/test-all-locally.csx` - Comprehensive testing with Spectre.Console UI (run before commits)
- `scripts/generate-docs.csx` - Documentation generator in C#
- `scripts/smoke-test.csx` - Quick validation with rich output (30 seconds)
- `scripts/test-nuget-package.csx` - NuGet package validator with progress indicators
- `scripts/prepare-nuget-readme.csx` - README converter for NuGet packages

### GitHub Workflow Scripts (Shell Scripts - KEEP FOR CI/CD)
- `scripts/ship-it.sh` - Main deployment with auto-version bump (GitHub Actions)
- `scripts/publish-nuget.sh` - NuGet publishing (GitHub Actions)
- `scripts/sync-upstream.sh` - Sync with upstream fork

### DEPRECATED Shell Scripts (Use CSX instead)
- ~~`scripts/test-all-locally.sh`~~ → Use `test-all-locally.csx`
- ~~`scripts/test-nuget-package.sh`~~ → Use `test-nuget-package.csx`
- ~~`scripts/smoke-test.sh`~~ → Use `smoke-test.csx`
- ~~`scripts/generate-docs.sh`~~ → Use `generate-docs.csx`
- ~~`scripts/prepare-nuget-readme.sh`~~ → Use `prepare-nuget-readme.csx`

## Documentation Best Practices

### Preventing Broken Links
- **Verify existence**: Always check link targets exist before creating links
- **Consistent paths**: Use relative paths consistently (avoid mixing styles)
- **Placeholder files**: Create stubs for planned content instead of dead links
- **Local validation**: Run DocFX build locally before committing
- **Index files**: Every directory needs README.md or index.md

### DocFX Management
- **Clean builds**: Delete obj/bin when DocFX shows compilation errors
- **Build order**: Run `dotnet build -c Release` before DocFX
- **Working directory**: Always run DocFX from where docfx.json is located
- **Treat warnings as errors**: Fix all DocFX warnings immediately

### Branch Strategy
- **Separation**: Keep source (master) and built docs (gh-pages) strictly separate
- **Never mix**: Don't combine source code and built HTML in same branch
- **Automation**: Use GitHub Actions to enforce branch rules
- **Clear documentation**: Explain branch strategy in README

### Quality Standards
- **Every directory needs an index**: Prevents navigation confusion
- **API docs from code**: Generate from XML comments in source
- **Multiple navigation paths**: Support search, TOC, and cross-links
- **Test like code**: Treat broken links as bugs requiring fixes

## Common Pitfalls to Avoid

1. **Orphaned links**: Don't link to files that don't exist yet
2. **Broken anchors**: Verify #section bookmarks exist in target files
3. **Path inconsistency**: Don't mix absolute and relative paths
4. **Missing directories**: Create full directory structure before deep linking
5. **Assumption errors**: Always verify file existence before linking

## Continuous Improvement

### Learning from Issues
- Branch pollution and 404s teach us to maintain cleaner structure
- Missing index files cause user confusion - prevent proactively
- Treat documentation with same rigor as code
- Validate built HTML in _site directory for broken links
- Ensure all markdown files are accessible and crawlable
- Maintain an up-to-date sitemap.xml

### Workflow Optimization
- Spend 80% time coding, 20% on deployment (not vice versa)
- Use simple scripts over complex CI/CD
- Fix issues locally with Claude for immediate resolution
- Keep feedback loops as short as possible
- never ignore CS1591: Missing XML comment for publicly visible type or member
- Always fix it to the best of your ability doing the best job of documentation humanly possible
- never  suppress  CS1591 warnings: And always fix on every single build cycle
- Run final test suite to verify all tests pass no errors or skips or 
  exceptions -> final todo
  Run all examples too Fixing any warning or exceptions or buuild errors 
  You will have to rebuild the documentation after too  because the XML 
  surface has changed 

As is the general instruction before finishing any to-do list
- Update the readme and NuGet readme making sure all of the links are up to date and all of the information is up to date. Rewriting as necessary before making a major commit
- Never bypass GitHub workflows by trying to push everything live yourself