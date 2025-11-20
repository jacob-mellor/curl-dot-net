# GitHub Workflows Policy

## CRITICAL: WITHOUT THESE WORKFLOWS, ALL DEVELOPMENT IS POINTLESS

**If the smoke tests don't pass, if the docs don't go live, if the NuGet doesn't deploy - there is NO POINT to any of this work.**

## ONLY 5 WORKFLOWS ARE ALLOWED - NO EXCEPTIONS

This repository maintains exactly **5 GitHub workflows**. These are ESSENTIAL for the software to exist. No additional workflows are permitted.

### The 5 Allowed Workflows:

1. **Smoke Test Windows** (`ci-smoke.yml`)
   - Runs comprehensive smoke tests on Windows
   - Tests .NET Framework 4.7.2, 4.8, and all .NET versions
   - Must pass before PR merge

2. **Smoke Test Ubuntu** (`ci-smoke.yml`)
   - Runs comprehensive smoke tests on Ubuntu
   - Tests .NET Standard and .NET Core/5+ versions
   - Must pass before PR merge

3. **Smoke Test Mac** (`ci-smoke.yml`)
   - Runs comprehensive smoke tests on macOS
   - Tests .NET Standard and .NET Core/5+ versions
   - Must pass before PR merge

4. **Deploy GitHub Pages** (`deploy-docs.yml`)
   - Deploys documentation to GitHub Pages
   - Triggers after PR merge from dev to main
   - Must be stable and reliable

5. **Deploy NuGet** (`nuget-deploy.yml`)
   - Builds on **Windows** to support all target frameworks
   - Targets: .NET Framework 4.7.2, 4.8, .NET Standard 2.0, .NET 6/7/8/9/10
   - Triggers after PR merge from dev to main
   - Uses repository secret for NuGet API key

## Workflow Execution Policy

### Pre-Merge (PR Validation)
- Smoke tests (Windows, Ubuntu, Mac) MUST pass
- Currently runs 7+ tests, can be expanded as needed
- All tests must be stable and reliable

### Post-Merge (dev → main)
- Deploy GitHub Pages
- Deploy NuGet Package

## Stability Requirements

These workflows MUST be stable and reliable. Any workflow causing deployment issues or blocking progress will be immediately removed.

## Forbidden Workflows

The following types of workflows are explicitly **NOT ALLOWED**:
- Code formatting/quality checks
- Additional build validations
- Redundant deployment workflows
- Any workflow that blocks deployment

## Enforcement

Any workflow files not in the allowed list will be deleted immediately. This is non-negotiable.

---

**Last Updated**: November 20, 2024
**Policy Owner**: Jacob Mellor