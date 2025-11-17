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

# Build, test, pack
echo "🔨 Building..."
dotnet build -c Release --verbosity quiet || { echo "❌ Build failed"; exit 1; }

echo "🧪 Running tests locally (faster than waiting for CI)..."
dotnet test --no-build -c Release --verbosity minimal || { echo "❌ Tests failed - fix locally before pushing"; exit 1; }

echo "📦 Packing..."
dotnet pack -c Release -o nupkg --no-build --verbosity quiet

echo "
✅ Version bumped: $NEW_VERSION
✅ Built
✅ Tested
✅ Packed

NuGet package: ./nupkg/CurlDotNet.$NEW_VERSION.nupkg

Next: git add -A && git commit -m 'Version $NEW_VERSION'
"