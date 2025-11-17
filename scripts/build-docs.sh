#!/bin/bash
# Build documentation using DocFX
# Run this script from the repository root

set -e  # Exit on error

echo "📚 Building CurlDotNet Documentation..."
echo "=========================================="

# Check if DocFX is installed
if ! command -v docfx &> /dev/null
then
    echo "❌ DocFX is not installed."
    echo ""
    echo "To install DocFX, run one of these commands:"
    echo ""
    echo "  Using .NET tool:"
    echo "    dotnet tool install -g docfx"
    echo ""
    echo "  Using Homebrew (macOS):"
    echo "    brew install docfx"
    echo ""
    echo "  Using Chocolatey (Windows):"
    echo "    choco install docfx"
    echo ""
    exit 1
fi

# Ensure we're in the repository root
if [ ! -f "CurlDotNet.sln" ]; then
    echo "❌ Error: This script must be run from the repository root."
    echo "   Current directory: $(pwd)"
    exit 1
fi

# Clean previous build artifacts
echo "🧹 Cleaning previous build artifacts..."
rm -rf build/docfx/obj build/docfx/_site
rm -rf src/CurlDotNet/obj src/CurlDotNet/bin

# Build the project to generate XML documentation
echo "🔨 Building project to generate XML documentation..."
dotnet build src/CurlDotNet/CurlDotNet.csproj --configuration Release

if [ $? -ne 0 ]; then
    echo "❌ Build failed"
    exit 1
fi

# Change to DocFX directory
cd build/docfx

# Generate metadata and build documentation
echo "📝 Generating API metadata..."
docfx metadata

if [ $? -ne 0 ]; then
    echo "❌ Metadata generation failed"
    exit 1
fi

echo "🎨 Building HTML documentation..."
docfx build

if [ $? -ne 0 ]; then
    echo "❌ Documentation build failed"
    exit 1
fi

# Return to repo root
cd ../..

echo ""
echo "✅ Documentation built successfully!"
echo ""
echo "📂 Output location: build/docfx/_site/"
echo ""
echo "To view the documentation locally, run:"
echo "  cd build/docfx && docfx serve _site"
echo ""
echo "Or open: build/docfx/_site/index.html"