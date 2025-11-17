#!/bin/bash
# Publish to NuGet.org

set -e

# Check for dry run mode
DRY_RUN=false
if [[ "$1" == "--dry-run" ]]; then
    DRY_RUN=true
    echo "🔍 Running in DRY RUN mode - no actual publishing"
fi

# Check for API key
if [[ -z "$NUGET_API_KEY" ]] && [[ "$DRY_RUN" == "false" ]]; then
    echo "❌ Error: NUGET_API_KEY environment variable is not set"
    echo "Set it with: export NUGET_API_KEY='your-api-key'"
    exit 1
fi

# Build and pack
echo "📦 Building and packing CurlDotNet..."
dotnet build src/CurlDotNet/CurlDotNet.csproj -c Release
dotnet pack src/CurlDotNet/CurlDotNet.csproj -c Release -o nupkg --no-build

# Find the package
PACKAGE=$(ls nupkg/CurlDotNet.*.nupkg | head -1)
if [[ -z "$PACKAGE" ]]; then
    echo "❌ Error: No package found in nupkg/"
    exit 1
fi

echo "📦 Found package: $PACKAGE"

if [[ "$DRY_RUN" == "true" ]]; then
    echo "✅ DRY RUN: Would publish $PACKAGE to NuGet.org"
    echo "Package details:"
    unzip -l "$PACKAGE" | head -20
else
    echo "🚀 Publishing to NuGet.org..."
    dotnet nuget push "$PACKAGE" \
        --api-key "$NUGET_API_KEY" \
        --source https://api.nuget.org/v3/index.json \
        --skip-duplicate

    echo "✅ Successfully published to NuGet.org!"
    echo "View at: https://www.nuget.org/packages/CurlDotNet/"
fi