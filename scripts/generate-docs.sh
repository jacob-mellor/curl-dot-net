#!/bin/bash
# Generate documentation into gh-pages folder for deployment

set -e

echo "📚 Generating documentation into gh-pages folder..."
echo "================================================"

# Clean and create gh-pages folder
rm -rf gh-pages
mkdir -p gh-pages

# 1. Build the project to generate XML docs
echo "🔨 Building project to generate XML documentation..."
dotnet build src/CurlDotNet/CurlDotNet.csproj -c Release -p:GenerateDocumentationFile=true

# 2. Generate API documentation from XML comments
echo "📝 Generating API documentation from XML comments..."
mkdir -p gh-pages/api

# Install DefaultDocumentation if not installed
if ! command -v defaultdocumentation &> /dev/null; then
    echo "Installing DefaultDocumentation tool..."
    dotnet tool install -g DefaultDocumentation.Console
fi

# Generate comprehensive API docs with all members
defaultdocumentation \
    -a src/CurlDotNet/obj/Release/netstandard2.0/CurlDotNet.dll \
    -o gh-pages/api \
    --GeneratedPages "Namespaces, Types, Members" \
    --IncludeUndocumentedItems true \
    --GeneratedAccessModifiers "Public, Protected, Internal"

echo "✅ Generated $(find gh-pages/api -name "*.md" | wc -l) API documentation files"

# 3. Create Jekyll configuration
echo "⚙️  Creating Jekyll configuration..."
cat > gh-pages/_config.yml << 'EOF'
# Jekyll configuration for CurlDotNet
title: CurlDotNet
description: Pure .NET implementation of curl for C#
baseurl: "/curl-dot-net"
url: "https://jacob-mellor.github.io"

# Theme
theme: jekyll-theme-cayman

# Plugins supported by GitHub Pages
plugins:
  - jekyll-feed
  - jekyll-seo-tag
  - jekyll-sitemap

# Exclude files
exclude:
  - Gemfile
  - Gemfile.lock
  - node_modules
  - vendor
EOF

# 4. Create main index page
echo "📄 Creating main index page..."
cat > gh-pages/index.md << 'EOF'
---
layout: home
title: CurlDotNet - Pure .NET curl for C#
---

# CurlDotNet

A pure .NET implementation of curl for C#, supporting .NET Standard 2.0, .NET 8.0, and .NET 10.0.

## Quick Start

```csharp
using CurlDotNet;

var curl = new Curl();
var result = await curl.GetAsync("https://api.example.com/data");
Console.WriteLine(result.Body);
```

## Documentation

- [API Reference](api/) - Complete API documentation
- [Getting Started](getting-started/) - Installation and first steps
- [Tutorials](tutorials/) - Step-by-step guides
- [Examples](examples/) - Code samples

## Installation

```bash
dotnet add package CurlDotNet
```

Or via Package Manager:

```powershell
Install-Package CurlDotNet
```

## Links

- [GitHub Repository](https://github.com/jacob-mellor/curl-dot-net)
- [NuGet Package](https://www.nuget.org/packages/CurlDotNet/)
- [Report Issues](https://github.com/jacob-mellor/curl-dot-net/issues)
EOF

# 5. Copy existing documentation
echo "📂 Copying existing documentation..."
if [ -d "docs" ]; then
    # Copy specific documentation folders
    [ -d "docs/tutorials" ] && cp -r docs/tutorials gh-pages/
    [ -d "docs/cookbook" ] && cp -r docs/cookbook gh-pages/
    [ -d "docs/getting-started" ] && cp -r docs/getting-started gh-pages/
    [ -d "docs/samples" ] && cp -r docs/samples gh-pages/

    # Copy important docs
    [ -f "docs/ADVANCED.md" ] && cp docs/ADVANCED.md gh-pages/
    [ -f "docs/USAGE_GUIDE.md" ] && cp docs/USAGE_GUIDE.md gh-pages/
    [ -f "docs/DOCUMENTATION.md" ] && cp docs/DOCUMENTATION.md gh-pages/
fi

# 6. Create API index
echo "📋 Creating API index page..."
cat > gh-pages/api/index.md << 'EOF'
---
layout: page
title: API Reference
permalink: /api/
---

# CurlDotNet API Reference

Complete API documentation for all namespaces, classes, and methods.

## Main Namespaces

- [CurlDotNet](CurlDotNet.md) - Main namespace
- [CurlDotNet.Core](CurlDotNet.Core.md) - Core functionality
- [CurlDotNet.Exceptions](CurlDotNet.Exceptions.md) - Exception types
- [CurlDotNet.Extensions](CurlDotNet.Extensions.md) - Extension methods
- [CurlDotNet.Middleware](CurlDotNet.Middleware.md) - Middleware pipeline

## Quick Links

### Essential Classes
- [Curl](CurlDotNet.Curl.md) - Main entry point
- [CurlResult](CurlDotNet.Core.CurlResult.md) - Request results
- [CurlOptions](CurlDotNet.Core.CurlOptions.md) - Configuration

### Middleware
- [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md) - Middleware interface
- [RetryMiddleware](CurlDotNet.Middleware.RetryMiddleware.md) - Retry logic
- [RateLimitMiddleware](CurlDotNet.Middleware.RateLimitMiddleware.md) - Rate limiting
EOF

# 7. Add .gitignore for gh-pages folder
echo "📝 Creating .gitignore for gh-pages..."
cat > gh-pages/.gitignore << 'EOF'
_site/
.sass-cache/
.jekyll-cache/
.jekyll-metadata
vendor/
EOF

echo ""
echo "========================================="
echo "✅ Documentation generation complete!"
echo ""
echo "📊 Summary:"
echo "  - Location: ./gh-pages/"
echo "  - API docs: $(find gh-pages/api -name "*.md" 2>/dev/null | wc -l) files"
echo "  - Ready for deployment to GitHub Pages"
echo ""
echo "📝 The gh-pages folder is ready to be deployed"
echo "   by the GitHub workflow on push/merge"
echo "========================================="