#!/bin/bash
# Manual publish script - run this locally when you want to release

echo "🚀 CurlDotNet Manual Release Script"
echo "===================================="
echo "This script handles both documentation and NuGet publishing locally"
echo ""

# Check if we're in the right directory
if [ ! -f "CurlDotNet.sln" ]; then
    echo "❌ Error: Run this from the repository root"
    exit 1
fi

# Menu
echo "What would you like to do?"
echo "1) Build and publish documentation to GitHub Pages"
echo "2) Build and publish NuGet package"
echo "3) Both (documentation + NuGet)"
echo "4) Build documentation locally (test only)"
echo "5) Build NuGet package locally (test only)"
echo ""
read -p "Enter choice (1-5): " choice

case $choice in
    1)
        echo "📚 Building documentation..."
        cd build/docfx
        docfx build
        cd ../..

        echo "📤 Publishing to GitHub Pages..."
        git checkout gh-pages
        cp -r build/docfx/_site/* .
        git add -A
        git commit -m "Update documentation $(date +%Y-%m-%d)"
        git push origin gh-pages
        git checkout master
        echo "✅ Documentation published!"
        ;;

    2)
        echo "📦 Building NuGet package..."
        dotnet pack src/CurlDotNet/CurlDotNet.csproj -c Release -o ./nupkg

        echo "🔑 Enter your NuGet API key:"
        read -s NUGET_KEY

        echo "📤 Publishing to NuGet.org..."
        dotnet nuget push ./nupkg/*.nupkg -k $NUGET_KEY -s https://api.nuget.org/v3/index.json
        echo "✅ NuGet package published!"
        ;;

    3)
        # Do both
        $0 1
        $0 2
        ;;

    4)
        echo "🔨 Building documentation locally..."
        cd build/docfx
        docfx build
        echo "✅ Documentation built at: build/docfx/_site/index.html"
        echo "Open in browser: file://$(pwd)/_site/index.html"
        ;;

    5)
        echo "📦 Building NuGet package locally..."
        dotnet pack src/CurlDotNet/CurlDotNet.csproj -c Release -o ./nupkg
        echo "✅ Package built at: ./nupkg/"
        ls -la ./nupkg/*.nupkg
        ;;

    *)
        echo "❌ Invalid choice"
        exit 1
        ;;
esac

echo ""
echo "🎉 Done!"