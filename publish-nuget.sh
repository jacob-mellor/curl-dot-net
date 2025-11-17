#!/bin/bash
# Publish NuGet package manually
# Use this when GitHub Actions won't cooperate

echo "📦 CurlDotNet NuGet Publisher"
echo "============================="

# Check current version
CURRENT_VERSION=$(grep '<Version>' src/CurlDotNet/CurlDotNet.csproj | sed 's/.*<Version>\(.*\)<\/Version>.*/\1/')
echo "Current version in .csproj: $CURRENT_VERSION"
echo ""

echo "Options:"
echo "1) Publish current version ($CURRENT_VERSION)"
echo "2) Bump patch version and publish"
echo "3) Bump minor version and publish"
echo "4) Set specific version and publish"
echo "5) Dry run (build only, don't publish)"
read -p "Choose option (1-5): " choice

case $choice in
    1)
        VERSION=$CURRENT_VERSION
        ;;
    2)
        # Bump patch (1.0.0 -> 1.0.1)
        VERSION=$(echo $CURRENT_VERSION | awk -F. '{$NF = $NF + 1;} 1' | sed 's/ /./g')
        ;;
    3)
        # Bump minor (1.0.0 -> 1.1.0)
        VERSION=$(echo $CURRENT_VERSION | awk -F. '{$(NF-1) = $(NF-1) + 1; $NF = 0;} 1' | sed 's/ /./g')
        ;;
    4)
        read -p "Enter new version (e.g., 1.2.3): " VERSION
        ;;
    5)
        VERSION=$CURRENT_VERSION
        DRY_RUN=true
        ;;
    *)
        echo "Invalid choice"
        exit 1
        ;;
esac

# Update version if changed
if [ "$VERSION" != "$CURRENT_VERSION" ]; then
    echo "📝 Updating version to $VERSION..."
    sed -i.bak "s/<Version>.*<\/Version>/<Version>$VERSION<\/Version>/" src/CurlDotNet/CurlDotNet.csproj
    rm src/CurlDotNet/CurlDotNet.csproj.bak
fi

# Clean and build
echo "🧹 Cleaning previous builds..."
rm -rf nupkg/
dotnet clean src/CurlDotNet/CurlDotNet.csproj

echo "🔨 Building package..."
dotnet pack src/CurlDotNet/CurlDotNet.csproj -c Release -o ./nupkg

if [ ! -f "./nupkg/CurlDotNet.$VERSION.nupkg" ]; then
    echo "❌ Package build failed!"
    exit 1
fi

echo "✅ Package built: CurlDotNet.$VERSION.nupkg"

if [ "$DRY_RUN" = true ]; then
    echo "🔍 Dry run complete. Package built but not published."
    echo "Package location: ./nupkg/CurlDotNet.$VERSION.nupkg"
    exit 0
fi

# Get API key
if [ -z "$NUGET_API_KEY" ]; then
    echo ""
    echo "Enter your NuGet API key:"
    echo "(Get it from: https://www.nuget.org/account/apikeys)"
    read -s NUGET_API_KEY
fi

# Publish
echo "📤 Publishing to NuGet.org..."
dotnet nuget push ./nupkg/CurlDotNet.$VERSION.nupkg \
    --api-key $NUGET_API_KEY \
    --source https://api.nuget.org/v3/index.json \
    --skip-duplicate

if [ $? -eq 0 ]; then
    echo "✅ Successfully published CurlDotNet $VERSION to NuGet!"

    # Create git tag
    echo "🏷️  Creating git tag v$VERSION..."
    git add src/CurlDotNet/CurlDotNet.csproj
    git commit -m "Release v$VERSION to NuGet"
    git tag -a "v$VERSION" -m "Release version $VERSION"
    git push origin master
    git push origin "v$VERSION"

    echo "🎉 All done!"
    echo "📦 Package: https://www.nuget.org/packages/CurlDotNet/$VERSION"
else
    echo "❌ Publishing failed. Check your API key and network connection."
fi