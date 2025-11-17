#!/bin/bash
# Force deploy documentation to GitHub Pages
# Run this when GitHub Actions is being stubborn

echo "🚀 Force Deploy Documentation to GitHub Pages"
echo "============================================="

# Build docs locally
echo "📚 Building documentation with DocFX..."
cd build/docfx
docfx build --force

if [ ! -d "_site" ]; then
    echo "❌ Build failed - no _site directory"
    exit 1
fi

echo "✅ Documentation built successfully"
cd ../..

# Deploy to gh-pages branch
echo "📤 Deploying to gh-pages branch..."

# Stash any local changes
git stash

# Checkout gh-pages
git fetch origin gh-pages
git checkout gh-pages || git checkout -b gh-pages

# Clear old content
git rm -rf . 2>/dev/null || true

# Copy new documentation
cp -r build/docfx/_site/* .

# Add nojekyll file to prevent GitHub from processing
touch .nojekyll

# Commit and push
git add -A
git commit -m "Force documentation update $(date +%Y-%m-%d_%H:%M:%S)" || echo "No changes"
git push origin gh-pages --force

# Return to master
git checkout master
git stash pop 2>/dev/null || true

echo "✅ Documentation force deployed!"
echo "🌐 Check: https://jacob-mellor.github.io/curl-dot-net/"
echo ""
echo "Note: It may take 5-10 minutes for GitHub to update the site"