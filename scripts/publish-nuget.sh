#!/bin/bash
# Publish CurlDotNet to NuGet.org
# This script builds, packs, and publishes the NuGet package

set -euo pipefail  # Exit on error, undefined variables, and pipe failures

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "📦 CurlDotNet NuGet Publishing Script"
echo "====================================="

# Ensure we're in the repository root
if [ ! -f "CurlDotNet.sln" ]; then
    echo -e "${RED}❌ Error: This script must be run from the repository root.${NC}"
    echo "   Current directory: $(pwd)"
    exit 1
fi

# Check for dry run mode
DRY_RUN=false
if [ "${1:-}" = "--dry-run" ]; then
    DRY_RUN=true
    echo -e "${YELLOW}🏃 Running in DRY RUN mode - no actual publishing${NC}"
fi

# Check for NuGet API key
if [ -z "${NUGET_API_KEY:-}" ] && [ "$DRY_RUN" = false ]; then
    echo -e "${RED}❌ Error: NUGET_API_KEY environment variable is not set${NC}"
    echo "   Set it with: export NUGET_API_KEY='your-api-key'"
    exit 1
fi

# Clean previous packages
echo "🧹 Cleaning previous packages..."
rm -rf src/CurlDotNet/bin/Release/*.nupkg
rm -rf src/CurlDotNet/bin/Release/*.snupkg

# Restore dependencies
echo "📦 Restoring dependencies..."
dotnet restore CurlDotNet.sln

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Restore failed${NC}"
    exit 1
fi

# Build in Release mode
echo "🔨 Building solution in Release mode..."
dotnet build CurlDotNet.sln --configuration Release --no-restore

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Build failed${NC}"
    exit 1
fi

# Run tests before packing
echo "🧪 Running tests..."
dotnet test tests/CurlDotNet.Tests/CurlDotNet.Tests.csproj --configuration Release --no-build --verbosity quiet

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Tests failed - cannot publish with failing tests${NC}"
    exit 1
fi

echo -e "${GREEN}✅ All tests passed${NC}"

# Pack the NuGet package
echo "📦 Creating NuGet package..."
dotnet pack src/CurlDotNet/CurlDotNet.csproj \
    --configuration Release \
    --no-build \
    --no-restore \
    --output ./nupkg

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Package creation failed${NC}"
    exit 1
fi

# Find the created package
PACKAGE_FILE=$(ls nupkg/*.nupkg 2>/dev/null | head -1)
SYMBOLS_FILE=$(ls nupkg/*.snupkg 2>/dev/null | head -1 || true)

if [ -z "$PACKAGE_FILE" ]; then
    echo -e "${RED}❌ No package file found${NC}"
    exit 1
fi

# Display package information
echo ""
echo -e "${GREEN}✅ Package created successfully!${NC}"
echo "   Package: $PACKAGE_FILE"
if [ -n "$SYMBOLS_FILE" ]; then
    echo "   Symbols: $SYMBOLS_FILE"
fi

# Get package size
PACKAGE_SIZE=$(du -h "$PACKAGE_FILE" | cut -f1)
echo "   Size: $PACKAGE_SIZE"

# Extract version from package name
PACKAGE_NAME=$(basename "$PACKAGE_FILE")
VERSION=$(echo "$PACKAGE_NAME" | grep -oE '[0-9]+\.[0-9]+\.[0-9]+(-[a-zA-Z0-9\.]+)?')
echo "   Version: $VERSION"

# Validate package contains required files
echo ""
echo "🔍 Validating package contents..."
TEMP_DIR=$(mktemp -d)
unzip -q "$PACKAGE_FILE" -d "$TEMP_DIR"

# Check for XML documentation
if ls "$TEMP_DIR"/lib/*/CurlDotNet.xml >/dev/null 2>&1; then
    echo -e "   ${GREEN}✅ XML documentation included${NC}"
else
    echo -e "   ${YELLOW}⚠️  Warning: XML documentation not found in package${NC}"
fi

# Check for README
if [ -f "$TEMP_DIR/README.md" ] || [ -f "$TEMP_DIR/readme.md" ]; then
    echo -e "   ${GREEN}✅ README included${NC}"
else
    echo -e "   ${YELLOW}⚠️  Note: README not included in package${NC}"
fi

# Check for license
if [ -f "$TEMP_DIR/LICENSE" ] || [ -f "$TEMP_DIR/LICENSE.txt" ] || [ -f "$TEMP_DIR/license.txt" ]; then
    echo -e "   ${GREEN}✅ License file included${NC}"
elif grep -q "PackageLicenseExpression" "$TEMP_DIR"/*.nuspec 2>/dev/null; then
    echo -e "   ${GREEN}✅ License expression specified${NC}"
else
    echo -e "   ${YELLOW}⚠️  Warning: No license information found${NC}"
fi

# Clean up temp directory
rm -rf "$TEMP_DIR"

# Publish to NuGet
if [ "$DRY_RUN" = true ]; then
    echo ""
    echo -e "${YELLOW}🏃 DRY RUN - Skipping actual publish${NC}"
    echo ""
    echo "To publish for real, run:"
    echo "  export NUGET_API_KEY='your-api-key'"
    echo "  $0"
else
    echo ""
    echo "📤 Publishing to NuGet.org..."

    # Publish main package
    dotnet nuget push "$PACKAGE_FILE" \
        --api-key "$NUGET_API_KEY" \
        --source https://api.nuget.org/v3/index.json \
        --skip-duplicate

    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Package published successfully!${NC}"
    else
        echo -e "${YELLOW}⚠️  Package push completed (may have been skipped if duplicate)${NC}"
    fi

    # Publish symbols package if it exists
    if [ -n "$SYMBOLS_FILE" ]; then
        echo "📤 Publishing symbols package..."
        dotnet nuget push "$SYMBOLS_FILE" \
            --api-key "$NUGET_API_KEY" \
            --source https://api.nuget.org/v3/index.json \
            --skip-duplicate

        if [ $? -eq 0 ]; then
            echo -e "${GREEN}✅ Symbols package published successfully!${NC}"
        else
            echo -e "${YELLOW}⚠️  Symbols push completed (may have been skipped)${NC}"
        fi
    fi
fi

# Final summary
echo ""
echo "====================================="
echo -e "${GREEN}✨ Publishing process complete!${NC}"
echo ""
echo "📦 Package: $PACKAGE_NAME"
echo "🏷️  Version: $VERSION"
echo ""

if [ "$DRY_RUN" = false ]; then
    echo "Next steps:"
    echo "  1. Visit: https://www.nuget.org/packages/CurlDotNet/$VERSION"
    echo "  2. Verify package appears correctly"
    echo "  3. Create GitHub release with tag: v$VERSION"
    echo "  4. Update documentation if needed"
else
    echo "This was a dry run. To publish for real:"
    echo "  export NUGET_API_KEY='your-api-key'"
    echo "  $0"
fi

# Clean up package output directory
rm -rf nupkg/

echo ""
echo "✅ Done!"