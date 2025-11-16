# CRITICAL: Documentation Structure Guidelines

## ⚠️ NEVER MIX SOURCE AND BUILD FILES

### The Problem We Keep Having:
1. Documentation build files (HTML, JS, CSS) keep getting mixed with source files
2. The master branch gets polluted with built documentation
3. The gh-pages branch gets source code and tests

### The Permanent Solution:

#### Directory Structure:
```
/build/docfx/          <- DocFX configuration and source files
  docfx.json           <- DocFX config
  index.md             <- Main documentation page
  toc.yml              <- Table of contents
  api/                 <- API documentation config
  _site/               <- BUILD OUTPUT (NEVER COMMIT)

/docs/                 <- Markdown documentation sources
  README.md            <- Documentation readme
  tutorials/           <- Tutorial markdown files
  cookbook/            <- Cookbook markdown files
  api-guide/           <- API guide markdown files
  etc...

gh-pages branch:       <- ONLY built documentation
  index.html           <- Built HTML files
  api/                 <- Built API docs
  public/              <- CSS, JS assets
  NO SOURCE FILES!
  NO TESTS!
  NO .cs FILES!
```

#### Rules to Follow:

1. **NEVER** run `docfx build` in the /docs directory
2. **ALWAYS** run `docfx build` in /build/docfx directory
3. **NEVER** commit _site/ directory to master branch
4. **NEVER** copy source files to gh-pages branch
5. **ALWAYS** use the deployment scripts

#### Correct Deployment Process:

```bash
# 1. Build documentation
cd build/docfx
docfx build

# 2. Switch to gh-pages branch (orphan)
git checkout --orphan gh-pages
git rm -rf .

# 3. Copy ONLY built files
cp -r build/docfx/_site/* .
touch .nojekyll

# 4. Commit and push
git add .
git commit -m "Deploy documentation"
git push origin gh-pages --force
```

#### GitHub Actions Workflow:
The `.github/workflows/docs.yml` file automates this process.
It builds in `build/docfx` and deploys to gh-pages.

## If Things Get Messed Up Again:

1. Clean the docs directory:
```bash
cd docs
rm -rf *.html *.json *.xml *.svg *.ico public/ api/*.html
```

2. Reset gh-pages branch:
```bash
git branch -D gh-pages
git push origin --delete gh-pages
```

3. Rebuild and redeploy:
```bash
./scripts/deploy-docs.sh
```

## Remember:
- Source files go in `/docs` (markdown) and `/build/docfx` (docfx config)
- Built files go ONLY in gh-pages branch
- Never mix the two!