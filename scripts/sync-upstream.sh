#!/bin/bash
# Sync with upstream UserlandDotNet/curl-dot-net

echo "🔄 Syncing with upstream UserlandDotNet/curl-dot-net..."

# Add upstream if not exists
git remote get-url upstream &>/dev/null || git remote add upstream https://github.com/UserlandDotNet/curl-dot-net.git

# Fetch upstream
git fetch upstream

# Merge upstream/master into current branch
CURRENT_BRANCH=$(git branch --show-current)
echo "📥 Merging upstream/master into $CURRENT_BRANCH..."

git merge upstream/master --no-edit

if [ $? -eq 0 ]; then
    echo "✅ Successfully synced with upstream!"
    echo "📤 Push to origin with: git push origin $CURRENT_BRANCH"
else
    echo "⚠️  Merge conflicts detected. Resolve them and commit."
fi