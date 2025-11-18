#!/bin/bash
set -e

# Script to prepare README for NuGet package with absolute links
# This converts all relative links to absolute GitHub links

REPO_BASE="https://github.com/jacob-mellor/curl-dot-net"
DOCS_BASE="https://jacob-mellor.github.io/curl-dot-net"
SOURCE_README="README.md"
NUGET_README="nuget-readme.md"

echo "📄 Preparing README for NuGet package..."

# Copy the main README
cp "$SOURCE_README" "$NUGET_README"

echo "🔗 Converting relative links to absolute..."

# Convert relative GitHub links to absolute
# [text](relative/path) -> [text](https://github.com/jacob-mellor/curl-dot-net/blob/master/relative/path)
sed -i.bak -E "s|\[([^\]]+)\]\(([^)]+\.md)\)|\[\1\]($REPO_BASE/blob/master/\2)|g" "$NUGET_README"

# Convert documentation links to absolute
# [text](tutorials/...) -> [text](https://jacob-mellor.github.io/curl-dot-net/tutorials/...)
sed -i.bak -E "s|\[([^\]]+)\]\(tutorials/([^)]+)\)|\[\1\]($DOCS_BASE/tutorials/\2)|g" "$NUGET_README"
sed -i.bak -E "s|\[([^\]]+)\]\(cookbook/([^)]+)\)|\[\1\]($DOCS_BASE/cookbook/\2)|g" "$NUGET_README"
sed -i.bak -E "s|\[([^\]]+)\]\(api-guide/([^)]+)\)|\[\1\]($DOCS_BASE/api-guide/\2)|g" "$NUGET_README"
sed -i.bak -E "s|\[([^\]]+)\]\(guides/([^)]+)\)|\[\1\]($DOCS_BASE/guides/\2)|g" "$NUGET_README"
sed -i.bak -E "s|\[([^\]]+)\]\(troubleshooting/([^)]+)\)|\[\1\]($DOCS_BASE/troubleshooting/\2)|g" "$NUGET_README"
sed -i.bak -E "s|\[([^\]]+)\]\(getting-started/([^)]+)\)|\[\1\]($DOCS_BASE/getting-started/\2)|g" "$NUGET_README"
sed -i.bak -E "s|\[([^\]]+)\]\(api/([^)]+)\)|\[\1\]($DOCS_BASE/api/\2)|g" "$NUGET_README"

# Remove backup files
rm -f "$NUGET_README.bak"

echo "✅ NuGet README prepared with absolute links"

# Copy to the src/CurlDotNet directory for packaging
cp "$NUGET_README" "src/CurlDotNet/README.md"

echo "📦 README copied to src/CurlDotNet for NuGet packaging"