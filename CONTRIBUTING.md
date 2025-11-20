# Contributing to CurlDotNet

## Development Workflow

We follow a strict PR-based development workflow to ensure quality and reliability.

### 🌳 Branch Strategy

1. **`master`** - Production branch. Never commit directly here.
2. **`dev`** - Development branch. All work happens here.
3. **`gh-pages`** - Documentation deployment (automated, don't touch).

### 🔄 Development Process

1. **Always work in the `dev` branch**
   ```bash
   git checkout dev
   git pull origin dev
   ```

2. **Test all scripts locally before committing**
   ```bash
   ./test-all-scripts.sh
   ```

3. **Make your changes**
   ```bash
   # Edit files
   git add .
   git commit -m "Description of changes"
   ```

4. **Push to dev branch**
   ```bash
   git push origin dev
   ```

5. **Create Pull Request to master**
   - Go to GitHub
   - Create PR from `dev` → `master`
   - Wait for all checks to pass
   - Merge when approved

### ✅ What Happens on PR

When you create a PR to master, these checks run automatically:

1. **Script Validation** - All shell scripts are validated
2. **Build & Test** - Runs on Linux, Windows, and macOS
3. **Documentation Build** - Ensures docs build successfully

### 🚀 What Happens on Merge

When PR is merged to master:

1. **Documentation deploys** to https://jacob-mellor.github.io/curl-dot-net/
2. **NuGet package publishes** if version changed
3. **GitHub release created** if new version

### 📝 Local Testing

Always test locally first:

```bash
# Test all scripts
./test-all-scripts.sh

# Test documentation build
cd build/docfx && docfx build

# Test NuGet packaging
./publish-nuget.sh 5  # Option 5 = dry run
```

### 🔑 Important Scripts

- `test-all-scripts.sh` - Validate all scripts work
- `publish.sh` - Manual publish (if automation fails)
- `force-deploy.sh` - Force documentation deployment
- `publish-nuget.sh` - Manual NuGet publishing

### 📋 Checklist Before PR

- [ ] All scripts tested with `./test-all-scripts.sh`
- [ ] Code builds: `dotnet build`
- [ ] Tests pass: `dotnet test`
- [ ] Documentation builds: `cd build/docfx && docfx build`
- [ ] Commit messages are clear
- [ ] Version bumped if needed (in .csproj)

### 🚫 Never Do This

- ❌ Don't commit directly to master
- ❌ Don't force push to master
- ❌ Don't skip script testing
- ❌ Don't merge if checks are failing
- ❌ Don't edit gh-pages branch manually

### 💡 Tips

- Keep PRs focused on one feature/fix
- Write clear PR descriptions
- Link related issues
- Test on your Mac first
- Use meaningful commit messages

### 🆘 If Automation Fails

If GitHub Actions isn't working:

```bash
# Deploy docs manually
./force-deploy.sh

# Publish NuGet manually
./publish-nuget.sh

# Or use the combined script
./publish.sh
```

### 📊 Workflow Status

- **PR Validation**: Runs on every PR
- **Deploy on Merge**: Runs when PR merges to master
- **NuGet Release**: Automatic when version changes

## Questions?

Open an issue or discussion on GitHub!