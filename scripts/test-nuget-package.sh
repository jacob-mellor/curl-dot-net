#!/bin/bash

# Test NuGet Package - Validates the NuGet package before deployment
# This script:
# 1. Unpacks the .nupkg file
# 2. Verifies all required DLLs are present
# 3. Creates a test project that consumes the package
# 4. Runs basic smoke tests

set -e

echo "🧪 NuGet Package Validation Test"
echo "================================"

# Find the latest .nupkg file
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
PACKAGE_PATH=$(find "$PROJECT_ROOT/src/CurlDotNet/bin/Release" -name "*.nupkg" | sort -V | tail -1)

if [ -z "$PACKAGE_PATH" ]; then
    echo "❌ No .nupkg file found. Run 'dotnet pack' first."
    exit 1
fi

echo "📦 Testing package: $PACKAGE_PATH"

# Create temp directory for testing
TEMP_DIR=$(mktemp -d)
echo "📁 Working directory: $TEMP_DIR"

# Unpack the NuGet package
echo ""
echo "1️⃣ Unpacking NuGet package..."
unzip -q "$PACKAGE_PATH" -d "$TEMP_DIR/unpacked"

# Verify required files exist
echo "2️⃣ Verifying package contents..."

# Check if this is a CI build (reduced target frameworks)
if ls "$TEMP_DIR/unpacked/lib" | grep -q "net10.0"; then
    # Full build with all frameworks
    REQUIRED_FILES=(
        "lib/netstandard2.0/CurlDotNet.dll"
        "lib/net8.0/CurlDotNet.dll"
        "lib/net9.0/CurlDotNet.dll"
        "lib/net10.0/CurlDotNet.dll"
        "CurlDotNet.nuspec"
    )
else
    # CI build with limited frameworks
    REQUIRED_FILES=(
        "lib/netstandard2.0/CurlDotNet.dll"
        "lib/net8.0/CurlDotNet.dll"
        "CurlDotNet.nuspec"
    )
    # Check for Windows-specific framework
    if [ -d "$TEMP_DIR/unpacked/lib/net472" ]; then
        REQUIRED_FILES+=("lib/net472/CurlDotNet.dll")
    fi
fi

for file in "${REQUIRED_FILES[@]}"; do
    if [ -f "$TEMP_DIR/unpacked/$file" ]; then
        echo "  ✅ Found: $file"
    else
        echo "  ❌ Missing: $file"
        exit 1
    fi
done

# Extract version from package name
PACKAGE_NAME=$(basename "$PACKAGE_PATH")
VERSION=$(echo "$PACKAGE_NAME" | sed -E 's/CurlDotNet\.([0-9]+\.[0-9]+\.[0-9]+)\.nupkg/\1/')
echo ""
echo "📌 Package Version: $VERSION"

# Create a test project that consumes the package
echo ""
echo "3️⃣ Creating test consumer project..."
cd "$TEMP_DIR"
dotnet new console -n NuGetTest -f net8.0 --force > /dev/null 2>&1
cd NuGetTest

# Create a local NuGet source
mkdir -p local-packages
cp "$PACKAGE_PATH" local-packages/

# Create a nuget.config to use local source
cat > nuget.config << EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="./local-packages" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
EOF

# Add the package reference
echo "4️⃣ Adding package reference..."
dotnet add package CurlDotNet --version "$VERSION" > /dev/null 2>&1

# Create a simple test program
echo "5️⃣ Creating test program..."
cat > Program.cs << 'EOF'
using System;
using System.Threading.Tasks;
using CurlDotNet;

class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            Console.WriteLine("🧪 Testing CurlDotNet package...");

            // Test 1: Verify main Curl class exists
            var curlType = typeof(CurlDotNet.Curl);
            if (curlType == null)
            {
                Console.WriteLine("❌ Test 1 Failed: Curl class not found");
                return 1;
            }
            Console.WriteLine("✅ Test 1 Passed: Curl class exists");

            // Test 2: Verify LibCurl class exists
            var libcurl = new CurlDotNet.Lib.LibCurl();
            if (libcurl == null)
            {
                Console.WriteLine("❌ Test 2 Failed: LibCurl instantiation");
                return 1;
            }
            Console.WriteLine("✅ Test 2 Passed: LibCurl instantiation");

            // Test 3: Check that we can set default properties
            CurlDotNet.Curl.DefaultMaxTimeSeconds = 30;
            if (CurlDotNet.Curl.DefaultMaxTimeSeconds != 30)
            {
                Console.WriteLine("❌ Test 3 Failed: Setting default timeout");
                return 1;
            }
            Console.WriteLine("✅ Test 3 Passed: Setting default timeout");

            // Test 4: Exception types exist
            var exceptionType = typeof(CurlDotNet.Exceptions.CurlException);
            if (exceptionType == null)
            {
                Console.WriteLine("❌ Test 4 Failed: Exception types");
                return 1;
            }
            Console.WriteLine("✅ Test 4 Passed: Exception types");

            Console.WriteLine("");
            Console.WriteLine("✨ All NuGet package tests passed!");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Test failed with exception: {ex.Message}");
            return 1;
        }
    }
}
EOF

# Build and run the test
echo "6️⃣ Building test project..."
if dotnet build --configuration Release; then
    echo "  ✅ Build successful"
else
    echo "  ❌ Build failed - See error above"
    exit 1
fi

echo "7️⃣ Running smoke tests..."
if dotnet run --configuration Release --no-build; then
    echo ""
    echo "✅ NuGet package validation PASSED!"
    echo ""

    # Show package info
    echo "📊 Package Details:"
    echo "  - Name: CurlDotNet"
    echo "  - Version: $VERSION"
    echo "  - Size: $(du -h "$PACKAGE_PATH" | cut -f1)"

    # List actual frameworks in the package
    echo -n "  - Frameworks: "
    ls "$TEMP_DIR/unpacked/lib" | tr '\n' ' ' | sed 's/netstandard2.0/.NET Standard 2.0/g; s/net472/.NET Framework 4.7.2/g; s/net8.0/.NET 8.0/g; s/net9.0/.NET 9.0/g; s/net10.0/.NET 10.0/g'
    echo
else
    echo ""
    echo "❌ NuGet package validation FAILED!"
    exit 1
fi

# Cleanup
cd /
rm -rf "$TEMP_DIR"

echo ""
echo "🎉 NuGet package is ready for deployment!"
echo "   Use: dotnet nuget push $PACKAGE_PATH"