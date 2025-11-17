# CurlDotNet Project Guidelines for Claude

## Core Development Principles

### Testing & Quality
- Never release if there's a red test. Always update the README with the testing status and code coverage. On every iteration or commit, try and improve the code coverage even by 1%.
- Fixing tests is always the priority on a to-do list. If there's any broken test, that is the first item that happens

### Error Handling & Documentation
- The purpose of an error message is not to tell the user that there's an exception but to help them fix the problem
- Every error message should link to an appropriate documentation page online
- Please ensure that every single exception that is thrown will link to an appropriate page within our documentation, either in the Git repo, or DocFX, or GitHub pages documentation

### Code Standards
- This project will only ever use pure C# code. We will never use PINVOKE for our deployable code, so if you can't find something in C# that is compatible, please transpile from C++ and use internet searching to do so

## Documentation Management Lessons

### Preventing Broken Links (Learned 2025-11-17)
- **Always verify link targets exist** before creating documentation links
- **Use consistent relative paths** - avoid mixing `~/` prefix with `../` navigation
- **Create placeholder files** for planned content rather than linking to non-existent files
- **Run DocFX build locally** before committing documentation changes to catch broken links early
- **Maintain an index README** in every documentation directory to prevent 404s

### DocFX Build Management
- **Clean obj/bin directories** when DocFX shows compilation errors - corrupted build artifacts are common
- **Build the project first** (`dotnet build -c Release`) before running DocFX to ensure XML docs are generated
- **Use the correct working directory** - DocFX must be run from where docfx.json is located
- **Monitor for warnings** - DocFX warnings about broken links should be treated as errors

### Branch Strategy for Documentation
- **Maintain strict separation** between source (master) and built docs (gh-pages)
- **Never mix source code and built HTML** in the same branch
- **Use GitHub Actions validation** to enforce branch separation rules
- **Document the separation clearly** in README files to prevent confusion

### Documentation Structure Best Practices
- **Every directory needs an index** - Either README.md or index.md to prevent navigation issues
- **Keep documentation close to code** - API documentation should be generated from XML comments
- **Provide multiple navigation paths** - Users should find content through search, TOC, and cross-links
- **Test documentation like code** - Broken links are bugs that need fixing

### Common Documentation Issues to Avoid
1. **Orphaned links** - Links to files that were planned but never created
2. **Incorrect anchor links** - Bookmarks (#section) that don't exist in target files
3. **Path confusion** - Mixing absolute and relative paths inconsistently
4. **Missing intermediate directories** - Creating deep links without the directory structure
5. **Assuming file existence** - Always verify files exist before linking to them

## Continuous Improvement
- Learn from what is happening right now with branch pollution and documentation 404s
- Documentation directories not having index READMEs causes user confusion - ensure it never happens again
- Treat documentation with the same rigor as code - test it, validate it, and maintain it