#!/bin/bash
# Ship it. Auto-bumps version. That's it.

# Auto-bump version
CSPROJ="src/CurlDotNet/CurlDotNet.csproj"
CURRENT_VERSION=$(grep '<Version>' $CSPROJ | sed 's/.*<Version>\(.*\)<\/Version>.*/\1/')

# Increment patch version (1.2.3 -> 1.2.4)
NEW_VERSION=$(echo $CURRENT_VERSION | awk -F. '{$NF = $NF + 1;} 1' | sed 's/ /./g')

echo "📦 Bumping version: $CURRENT_VERSION → $NEW_VERSION"

# Update all version references
sed -i.bak "s/<Version>$CURRENT_VERSION<\/Version>/<Version>$NEW_VERSION<\/Version>/" $CSPROJ
sed -i.bak "s/<AssemblyVersion>$CURRENT_VERSION.0<\/AssemblyVersion>/<AssemblyVersion>$NEW_VERSION.0<\/AssemblyVersion>/" $CSPROJ
sed -i.bak "s/<FileVersion>$CURRENT_VERSION.0<\/FileVersion>/<FileVersion>$NEW_VERSION.0<\/FileVersion>/" $CSPROJ
rm $CSPROJ.bak

# Generate documentation FIRST (part of every commit)
echo "📚 Generating API documentation..."
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
if [ -f "$DIR/generate-docs.sh" ]; then
    chmod +x $DIR/generate-docs.sh
    $DIR/generate-docs.sh || {
        echo "⚠️  Documentation generation had issues"
    }
fi

# Run comprehensive testing (required before every commit)
echo "🧪 Running comprehensive tests..."
$DIR/test-all-locally.sh || {
    echo "❌ Comprehensive tests failed - fix all issues before shipping!"
    exit 1
}

echo "📦 Creating final package with new version..."
dotnet pack -c Release -o nupkg --no-build --verbosity quiet

echo "
✅ Version bumped: $NEW_VERSION
✅ Built
✅ Tested
✅ Packed

NuGet package: ./nupkg/CurlDotNet.$NEW_VERSION.nupkg

Next: git add -A && git commit -m 'Version $NEW_VERSION'
"