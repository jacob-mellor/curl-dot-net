#!/bin/bash
# Deploy documentation to GitHub Pages (gh-pages branch)
# This runs LOCALLY for immediate feedback and control

set -e

echo "📚 Deploying Documentation to GitHub Pages"
echo "=========================================="

# 1. Generate fresh documentation
echo "🔨 Step 1: Generating documentation..."
./scripts/generate-docs.sh || {
    echo "❌ Documentation generation failed!"
    exit 1
}

# 2. Check if we're in a git repo
if ! git rev-parse --git-dir > /dev/null 2>&1; then
    echo "❌ Not in a git repository!"
    exit 1
fi

# 3. Save current branch
CURRENT_BRANCH=$(git branch --show-current)
echo "📍 Current branch: $CURRENT_BRANCH"

# 4. Stash any uncommitted changes
echo "💾 Stashing uncommitted changes..."
git stash push -m "docs-deploy-stash" --include-untracked || true

# 5. Prepare documentation files
echo "📦 Preparing documentation for deployment..."
TEMP_DOCS=$(mktemp -d)
echo "Using temp directory: $TEMP_DOCS"

# Copy Jekyll site files
cp -r _config.yml "$TEMP_DOCS/" 2>/dev/null || true
cp -r index.md "$TEMP_DOCS/" 2>/dev/null || true
cp -r docs "$TEMP_DOCS/" 2>/dev/null || true
cp -r _tutorials "$TEMP_DOCS/" 2>/dev/null || true
cp -r _cookbook "$TEMP_DOCS/" 2>/dev/null || true
cp -r _guides "$TEMP_DOCS/" 2>/dev/null || true
cp -r _data "$TEMP_DOCS/" 2>/dev/null || true
cp -r *.md "$TEMP_DOCS/" 2>/dev/null || true

# 6. Switch to gh-pages branch
echo "🔀 Switching to gh-pages branch..."
if git show-ref --verify --quiet refs/heads/gh-pages; then
    git checkout gh-pages
    git pull origin gh-pages --rebase || true
else
    echo "Creating new gh-pages branch..."
    git checkout --orphan gh-pages
    git rm -rf . || true
    git clean -fdx
fi

# 7. Clear old docs and copy new ones
echo "🗑️  Clearing old documentation..."
rm -rf * .* 2>/dev/null || true
git checkout HEAD -- .gitignore 2>/dev/null || true

echo "📝 Copying new documentation..."
cp -r "$TEMP_DOCS"/* . 2>/dev/null || true
cp -r "$TEMP_DOCS"/.* . 2>/dev/null || true

# 8. Add Jekyll config if missing
if [ ! -f _config.yml ]; then
    echo "Adding Jekyll config..."
    cat > _config.yml << 'EOF'
title: CurlDotNet Documentation
description: Pure .NET implementation of curl for C#
theme: jekyll-theme-cayman
plugins:
  - jekyll-sitemap
EOF
fi

# 9. Commit and push
echo "💾 Committing documentation..."
git add -A
if git diff --staged --quiet; then
    echo "📝 No documentation changes to deploy"
else
    git commit -m "📚 Deploy documentation from $CURRENT_BRANCH

Generated at $(date)
Source commit: $(git rev-parse --short HEAD)"

    echo "🚀 Pushing to GitHub Pages..."
    git push origin gh-pages
    echo "✅ Documentation deployed to gh-pages!"
fi

# 10. Return to original branch
echo "🔙 Returning to $CURRENT_BRANCH..."
git checkout "$CURRENT_BRANCH"

# 11. Restore stashed changes
echo "♻️  Restoring stashed changes..."
if git stash list | grep -q "docs-deploy-stash"; then
    git stash pop || true
fi

# Clean up temp directory
rm -rf "$TEMP_DOCS"

echo ""
echo "=========================================="
echo "✅ Documentation deployment complete!"
echo ""
echo "📖 View your documentation at:"
echo "   https://jacob-mellor.github.io/curl-dot-net/"
echo ""
echo "🎯 GitHub Pages will update in ~2 minutes"
echo "=========================================="