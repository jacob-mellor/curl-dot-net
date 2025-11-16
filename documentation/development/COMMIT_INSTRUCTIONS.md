# Git Commit Instructions for jacob-mellor

## Configuration

Your git is already configured with user.name "CTO". To commit as **jacob-mellor**, you can either:

### Option 1: Set Globally (Recommended)

```bash
git config --global user.name "jacob-mellor"
git config --global user.email "your-email@example.com"  # Replace with your email
```

### Option 2: Set Per-Repository Only

```bash
cd /Users/jacob/Documents/GitHub/curl-dot-net
git config user.name "jacob-mellor"
git config user.email "your-email@example.com"  # Replace with your email
```

### Verify Configuration

```bash
git config user.name
git config user.email
```

## Committing Changes

### Standard Workflow

```bash
# 1. Check status
git status

# 2. Stage changes
git add .

# Or stage specific files
git add src/CurlDotNet/Curl.cs
git add tests/

# 3. Commit with descriptive message
git commit -m "Short summary

Detailed explanation:
- What changed
- Why it changed
- Any important notes"
```

### Commit Message Format

Follow the format from `CONTINUING_WORK.md`:

```
Short summary (50 chars max)

Detailed explanation if needed:
- What changed
- Why it changed
- Any breaking changes

Fixes #123
Closes #456
```

### Examples

**Small fix:**
```bash
git commit -m "Fix command parser handling of Windows paths"
```

**Feature addition:**
```bash
git commit -m "Add PATCH and HEAD methods to LibCurl

- Added PatchAsync method
- Added HeadAsync method  
- Updated documentation
- Added unit tests"
```

**Major change:**
```bash
git commit -m "Major enhancement: Comprehensive test suite

- Added synthetic tests for CommandParser
- Added command-line comparison tests
- Enhanced error handling
- Updated documentation

Closes #42"
```

## Pushing to GitHub

```bash
# Push current branch
git push origin dotnetcurl

# Or push and set upstream
git push -u origin dotnetcurl
```

## Checking Status Before Commit

Always run tests before committing (TDD workflow):

```bash
# Build the solution
dotnet build src/CurlDotNet/CurlDotNet.csproj

# Run tests
dotnet test tests/CurlDotNet.Tests/CurlDotNet.Tests.csproj

# Check for warnings
dotnet build --verbosity normal
```

## Current Status

**Branch:** `dotnetcurl`  
**Ready to push:** Yes  
**Last commit:** "Major enhancement: Comprehensive features, tests, and documentation"

## Next Steps

1. **Set git user.name** (if not already set):
   ```bash
   git config --global user.name "jacob-mellor"
   ```

2. **Push to GitHub:**
   ```bash
   git push origin dotnetcurl
   ```

3. **Continue TDD workflow** (see `CONTINUING_WORK.md`):
   - Run tests
   - Fix bugs
   - Add edge case tests
   - Commit frequently
   - Iterate

---

**Note:** If you encounter authentication issues when pushing, you may need to set up GitHub credentials or SSH keys. See GitHub documentation for authentication setup.

