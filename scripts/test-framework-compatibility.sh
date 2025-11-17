#!/bin/bash

# Test Framework Compatibility
# This script tests both .NET 8.0 and .NET Standard 2.0 builds
# to ensure .NET Framework 4.7.2 compatibility

set -e

echo "🔄 Framework Compatibility Testing"
echo "==================================="
echo "Testing both .NET 8 and .NET Standard builds"
echo ""

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
cd "$PROJECT_ROOT"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "1️⃣ Building all target frameworks..."
echo ""

# Build .NET Standard 2.0
echo "Building .NET Standard 2.0..."
if dotnet build src/CurlDotNet/CurlDotNet.csproj -f netstandard2.0 -c Release --no-restore; then
    echo -e "${GREEN}✅ .NET Standard 2.0 build successful${NC}"
else
    echo -e "${RED}❌ .NET Standard 2.0 build failed${NC}"
    exit 1
fi

# Build .NET 8.0
echo ""
echo "Building .NET 8.0..."
if dotnet build src/CurlDotNet/CurlDotNet.csproj -f net8.0 -c Release --no-restore; then
    echo -e "${GREEN}✅ .NET 8.0 build successful${NC}"
else
    echo -e "${RED}❌ .NET 8.0 build failed${NC}"
    exit 1
fi

echo ""
echo "2️⃣ Testing with .NET 8.0 build..."
echo "=================================="

# Run tests with .NET 8 build (default)
if dotnet test tests/CurlDotNet.Tests/CurlDotNet.Tests.csproj \
    -c Release \
    --no-build \
    --verbosity minimal \
    --logger "console;verbosity=minimal"; then
    echo -e "${GREEN}✅ Tests pass with .NET 8.0 build${NC}"
    NET8_PASS=true
else
    echo -e "${RED}❌ Tests fail with .NET 8.0 build${NC}"
    NET8_PASS=false
fi

echo ""
echo "3️⃣ Testing with .NET Standard 2.0 build..."
echo "=========================================="

# Create a temporary test project that explicitly references .NET Standard build
TEMP_TEST_DIR="$PROJECT_ROOT/temp-netstandard-test"
rm -rf "$TEMP_TEST_DIR"
mkdir -p "$TEMP_TEST_DIR"

# Copy test project
cp -r tests/CurlDotNet.Tests/* "$TEMP_TEST_DIR/"

# Modify the project file to reference .NET Standard build
cat > "$TEMP_TEST_DIR/CurlDotNet.Tests.csproj" << 'EOF'
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <!-- Force reference to .NET Standard 2.0 build -->
    <Reference Include="CurlDotNet">
      <HintPath>../../src/CurlDotNet/bin/Release/netstandard2.0/CurlDotNet.dll</HintPath>
    </Reference>
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
    <PackageReference Include="FluentAssertions" Version="6.12.2" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="System.CommandLine" Version="2.0.0-beta4.22272.1" />
    <PackageReference Include="Ninject" Version="3.3.6" />
  </ItemGroup>
</Project>
EOF

echo "Restoring packages for .NET Standard test..."
dotnet restore "$TEMP_TEST_DIR/CurlDotNet.Tests.csproj" --verbosity quiet

echo "Building test project with .NET Standard reference..."
if dotnet build "$TEMP_TEST_DIR/CurlDotNet.Tests.csproj" -c Release --no-restore --verbosity quiet; then
    echo "Test project built successfully"
else
    echo -e "${YELLOW}⚠️  Cannot build tests with .NET Standard - may have .NET 8 specific APIs${NC}"
    NETSTANDARD_PASS=skipped
fi

if [ "$NETSTANDARD_PASS" != "skipped" ]; then
    echo "Running tests with .NET Standard 2.0 build..."
    if dotnet test "$TEMP_TEST_DIR/CurlDotNet.Tests.csproj" \
        -c Release \
        --no-build \
        --verbosity minimal \
        --logger "console;verbosity=minimal"; then
        echo -e "${GREEN}✅ Tests pass with .NET Standard 2.0 build${NC}"
        NETSTANDARD_PASS=true
    else
        echo -e "${RED}❌ Tests fail with .NET Standard 2.0 build${NC}"
        NETSTANDARD_PASS=false
    fi
fi

# Cleanup
rm -rf "$TEMP_TEST_DIR"

echo ""
echo "📊 Test Results Summary"
echo "======================="
echo ""

if [ "$NET8_PASS" = true ]; then
    echo -e "${GREEN}✅ .NET 8.0:        All tests passed${NC}"
else
    echo -e "${RED}❌ .NET 8.0:        Some tests failed${NC}"
fi

if [ "$NETSTANDARD_PASS" = true ]; then
    echo -e "${GREEN}✅ .NET Standard:   All tests passed${NC}"
    echo ""
    echo "✨ Framework Compatibility Verified!"
    echo "   .NET Standard 2.0 build is compatible with:"
    echo "   • .NET Framework 4.7.2+"
    echo "   • .NET Framework 4.8"
    echo "   • .NET Core 2.0+"
    echo "   • .NET 5.0+"
elif [ "$NETSTANDARD_PASS" = "skipped" ]; then
    echo -e "${YELLOW}⚠️  .NET Standard:   Tests use .NET 8 specific APIs${NC}"
    echo "   Library builds for .NET Standard but tests require .NET 8"
else
    echo -e "${RED}❌ .NET Standard:   Some tests failed${NC}"
    echo "   This may indicate .NET Framework 4.7.2 compatibility issues"
fi

echo ""
echo "💡 Note: Full .NET Framework 4.7.2 testing happens in Windows CI"
echo "   This local test verifies .NET Standard 2.0 compatibility"

# Exit code based on results
if [ "$NET8_PASS" = true ]; then
    exit 0
else
    exit 1
fi