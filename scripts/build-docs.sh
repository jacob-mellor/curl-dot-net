#!/bin/bash
# Build documentation using DocFX

set -e

echo "📚 Building documentation with DocFX..."

# Navigate to docfx directory
cd build/docfx

# Clean previous build
rm -rf _site obj

# Build documentation
echo "🔨 Running DocFX build..."
docfx build --warningsAsErrors false

# Verify output
if [ -d "_site" ]; then
    HTML_COUNT=$(find _site -name "*.html" | wc -l)
    echo "✅ Documentation built successfully: $HTML_COUNT HTML files generated"

    # Move _site to docs/_site for deployment
    rm -rf ../../docs/_site
    mv _site ../../docs/_site
    echo "📦 Documentation moved to docs/_site for deployment"
else
    echo "❌ Documentation build failed - no output generated"
    exit 1
fi

echo "✅ Documentation build complete!"