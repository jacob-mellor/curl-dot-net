#!/bin/bash
# Build documentation using DocFX
# This script builds docs locally exactly as CI will
# Output: docs/_site

set -euo pipefail  # Exit on error, undefined variables, and pipe failures

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "📚 CurlDotNet Documentation Build Script"
echo "=========================================="

# Ensure we're in the repository root
if [ ! -f "CurlDotNet.sln" ]; then
    echo -e "${RED}❌ Error: This script must be run from the repository root.${NC}"
    echo "   Current directory: $(pwd)"
    exit 1
fi

# Check if DocFX is installed
if ! command -v docfx &> /dev/null; then
    echo -e "${RED}❌ DocFX is not installed.${NC}"
    echo ""
    echo "To install DocFX, run:"
    echo "  dotnet tool install -g docfx"
    echo ""
    echo "For CI/CD environments:"
    echo "  dotnet tool restore"
    echo ""
    exit 1
fi

# Check if .NET SDK is available
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}❌ .NET SDK is not installed.${NC}"
    exit 1
fi

# Clean previous build artifacts
echo "🧹 Cleaning previous build artifacts..."
rm -rf docs/_site docs/obj
rm -rf build/docfx/obj build/docfx/_site
rm -rf src/CurlDotNet/obj src/CurlDotNet/bin

# Restore dependencies
echo "📦 Restoring dependencies..."
dotnet restore CurlDotNet.sln

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Restore failed${NC}"
    exit 1
fi

# Build the project to generate XML documentation
echo "🔨 Building project to generate XML documentation..."
dotnet build src/CurlDotNet/CurlDotNet.csproj \
    --configuration Release \
    --no-restore

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Build failed${NC}"
    exit 1
fi

# Check if DocFX config exists
if [ ! -f "build/docfx/docfx.json" ]; then
    echo -e "${RED}❌ DocFX configuration not found at build/docfx/docfx.json${NC}"
    exit 1
fi

# Change to DocFX directory
cd build/docfx

# Generate metadata (allow warnings for known .NET 8 issue)
echo "📝 Generating API metadata..."
echo -e "${YELLOW}Note: Metadata warnings about assembly corruption are a known .NET 8.0 issue${NC}"
docfx metadata || true

# Build documentation
echo "🎨 Building HTML documentation..."
docfx build

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Documentation build failed${NC}"
    exit 1
fi

# Return to repo root
cd ../..

# Move output to stable location (docs/_site)
echo "📂 Moving output to stable location..."
rm -rf docs/_site
mkdir -p docs
mv build/docfx/_site docs/_site

# Verify output exists
if [ ! -f "docs/_site/index.html" ]; then
    echo -e "${RED}❌ Documentation output not found${NC}"
    exit 1
fi

# Fix GitHub Pages directory indexes - rename README.html to index.html everywhere
echo "🔗 Converting README.html to index.html for GitHub Pages..."
find docs/_site -name "README.html" | while read readme; do
    dir=$(dirname "$readme")
    # If index.html doesn't already exist from an index.md file, rename README.html to index.html
    if [ ! -f "$dir/index.html" ]; then
        mv "$readme" "$dir/index.html"
        echo "   Renamed $(basename "$dir")/README.html to index.html"
    else
        # If index.html exists, keep README.html as backup
        echo "   Kept both index.html and README.html in $(basename "$dir")/"
    fi
done

# Count HTML files generated
HTML_COUNT=$(find docs/_site -name "*.html" | wc -l)

echo ""
echo -e "${GREEN}✅ Documentation built successfully!${NC}"
echo "   Generated $HTML_COUNT HTML files"
echo ""
echo "📂 Output location: docs/_site/"
echo ""
echo "To view the documentation locally, run:"
echo "  cd docs && python3 -m http.server 8080 --directory _site"
echo ""
echo "Then open: http://localhost:8080"
echo ""
echo "Alternative (if DocFX is installed):"
echo "  cd docs && docfx serve _site"