# CurlDotNet Project Guidelines for Claude

## 🚨 CRITICAL WORKFLOW RULES 🚨

### ALWAYS Generate API Documentation
- **Every build**: Run `./scripts/generate-docs.sh` after EVERY build
- **Before tests**: Generate docs BEFORE running unit tests
- **Before commits**: Ensure docs are regenerated before committing
- **Automated**: API docs must be current with code changes
- **Command**: Always run `./scripts/generate-docs.sh && ./scripts/test-all-locally.sh`

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
- **Command**: `./scripts/test-all-locally.sh`
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

### Essential Scripts
- `scripts/ship-it.sh` - Main deployment with auto-version bump
- `scripts/test-all-locally.sh` - Comprehensive testing (run before commits)
- `scripts/smoke-test.sh` - Quick validation (30 seconds)
- `scripts/test-local.sh` - Basic test runner
- `scripts/sync-upstream.sh` - Sync with upstream fork

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