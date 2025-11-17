#!/bin/bash
# Pack the CurlDotNet NuGet package
# This script creates a NuGet package ready for publishing

set -e  # Exit on error

echo "📦 CurlDotNet NuGet Packaging Script"
echo "====================================="

# Ensure we're in the repository root
if [ ! -f "CurlDotNet.sln" ]; then
    echo "❌ Error: This script must be run from the repository root."
    echo "   Current directory: $(pwd)"
    exit 1
fi

# Parse command line arguments
DRY_RUN=false
VERSION=""

while [[ $# -gt 0 ]]; do
    case $1 in
        --dry-run)
            DRY_RUN=true
            shift
            ;;
        --version)
            VERSION="$2"
            shift
            shift
            ;;
        *)
            echo "Unknown option: $1"
            echo "Usage: $0 [--dry-run] [--version <version>]"
            exit 1
            ;;
    esac
done

# Clean previous packages
echo "🧹 Cleaning previous packages..."
rm -rf src/CurlDotNet/bin/Release/*.nupkg

# Restore dependencies
echo "📦 Restoring dependencies..."
dotnet restore

# Build in Release mode
echo "🔨 Building in Release mode..."
dotnet build src/CurlDotNet/CurlDotNet.csproj --configuration Release

if [ $? -ne 0 ]; then
    echo "❌ Build failed"
    exit 1
fi

# Run tests before packing
echo "🧪 Running tests..."
dotnet test tests/CurlDotNet.Tests/CurlDotNet.Tests.csproj --configuration Release --no-build --verbosity quiet

if [ $? -ne 0 ]; then
    echo "❌ Tests failed - cannot create package with failing tests"
    exit 1
fi

# Pack the NuGet package
echo "📦 Creating NuGet package..."
if [ -n "$VERSION" ]; then
    echo "   Using version: $VERSION"
    dotnet pack src/CurlDotNet/CurlDotNet.csproj \
        --configuration Release \
        --no-build \
        --no-restore \
        -p:PackageVersion=$VERSION
else
    dotnet pack src/CurlDotNet/CurlDotNet.csproj \
        --configuration Release \
        --no-build \
        --no-restore
fi

if [ $? -ne 0 ]; then
    echo "❌ Package creation failed"
    exit 1
fi

# Find the created package
PACKAGE_FILE=$(ls src/CurlDotNet/bin/Release/*.nupkg 2>/dev/null | head -1)

if [ -z "$PACKAGE_FILE" ]; then
    echo "❌ No package file found"
    exit 1
fi

echo ""
echo "✅ Package created successfully!"
echo "   Package: $PACKAGE_FILE"

# Display package info
echo ""
echo "📋 Package Information:"
dotnet list src/CurlDotNet/CurlDotNet.csproj package

# Validate the package
echo ""
echo "🔍 Validating package..."

# Check package size
PACKAGE_SIZE=$(du -h "$PACKAGE_FILE" | cut -f1)
echo "   Size: $PACKAGE_SIZE"

# Extract and check package contents
TEMP_DIR=$(mktemp -d)
unzip -q "$PACKAGE_FILE" -d "$TEMP_DIR"

# Check for XML documentation
if ls "$TEMP_DIR"/lib/*/CurlDotNet.xml >/dev/null 2>&1; then
    echo "   ✅ XML documentation included"
else
    echo "   ⚠️  Warning: XML documentation not found in package"
fi

# Check for README
if [ -f "$TEMP_DIR/README.md" ]; then
    echo "   ✅ README.md included"
else
    echo "   ⚠️  Note: README.md not included in package"
fi

# Clean up temp directory
rm -rf "$TEMP_DIR"

# Dry run or publish instructions
echo ""
if [ "$DRY_RUN" = true ]; then
    echo "🏃 DRY RUN MODE - Package not published"
    echo ""
    echo "To publish this package to NuGet.org, run:"
    echo "  dotnet nuget push \"$PACKAGE_FILE\" --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json"
else
    echo "📤 Next Steps:"
    echo ""
    echo "1. Test the package locally:"
    echo "   dotnet add package CurlDotNet --source $(dirname "$PACKAGE_FILE")"
    echo ""
    echo "2. Publish to NuGet.org:"
    echo "   dotnet nuget push \"$PACKAGE_FILE\" --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json"
    echo ""
    echo "3. Or use GitHub Actions by creating a git tag:"
    echo "   git tag v$(grep '<Version>' src/CurlDotNet/CurlDotNet.csproj | sed 's/.*<Version>\(.*\)<\/Version>.*/\1/')"
    echo "   git push origin --tags"
fi

echo ""
echo "✨ Done!"