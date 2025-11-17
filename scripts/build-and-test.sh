#!/bin/bash
# Build and test the CurlDotNet project
# This script mirrors the CI pipeline for local development

set -e  # Exit on error

echo "🔨 CurlDotNet Build & Test Script"
echo "=================================="

# Ensure we're in the repository root
if [ ! -f "CurlDotNet.sln" ]; then
    echo "❌ Error: This script must be run from the repository root."
    echo "   Current directory: $(pwd)"
    exit 1
fi

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Clean previous build artifacts
echo "🧹 Cleaning previous build artifacts..."
dotnet clean --verbosity quiet

# Restore dependencies
echo "📦 Restoring dependencies..."
dotnet restore

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Restore failed${NC}"
    exit 1
fi

# Build the solution
echo "🔨 Building solution in Release mode..."
dotnet build --configuration Release --no-restore

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Build failed${NC}"
    exit 1
fi

echo -e "${GREEN}✅ Build completed successfully${NC}"

# Run tests
echo ""
echo "🧪 Running tests..."
dotnet test --no-build --configuration Release --verbosity normal --collect:"XPlat Code Coverage"

if [ $? -ne 0 ]; then
    echo -e "${RED}❌ Tests failed${NC}"
    exit 1
fi

echo -e "${GREEN}✅ All tests passed${NC}"

# Check for XML documentation warnings (optional)
echo ""
echo "📝 Checking XML documentation coverage..."
WARNING_COUNT=$(dotnet build src/CurlDotNet/CurlDotNet.csproj -c Release -p:TreatWarningsAsErrors=false 2>&1 | grep -c "CS1591" || true)

if [ "$WARNING_COUNT" -gt 0 ]; then
    echo -e "${YELLOW}⚠️  Found $WARNING_COUNT missing XML documentation comments${NC}"
else
    echo -e "${GREEN}✅ XML documentation complete${NC}"
fi

# Summary
echo ""
echo "====================================="
echo -e "${GREEN}✅ Build and test completed successfully!${NC}"
echo ""
echo "Next steps:"
echo "  • Run documentation build: ./scripts/build-docs.sh"
echo "  • Create NuGet package: dotnet pack -c Release"
echo "  • Run benchmarks: dotnet run -c Release --project tests/Benchmarks/CurlDotNet.Benchmarks.csproj"
echo ""