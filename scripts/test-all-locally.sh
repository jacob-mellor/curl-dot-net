#!/bin/bash
# Comprehensive local testing - Run before releases
# This is what CI would do, but LOCALLY so you can fix issues immediately

echo "🧪 Comprehensive Local Testing"
echo "=============================="
echo "Running ALL tests locally - fix any issues before pushing!"
echo ""

# Track any failures
FAILED=0

# 0. ALWAYS Generate API Documentation FIRST
echo "Step 0: Generating API documentation..."
echo "---------------------------------------"
if [ -f "scripts/generate-docs.sh" ]; then
    chmod +x scripts/generate-docs.sh
    ./scripts/generate-docs.sh || {
        echo "⚠️  Documentation generation had issues (continuing anyway)"
    }
else
    echo "⚠️  No documentation script found (creating basic one)"
    mkdir -p docs/api
fi

# 1. Clean build
echo ""
echo "Step 1: Clean build from scratch..."
echo "-----------------------------------"
dotnet clean --verbosity quiet
dotnet build -c Release --verbosity normal || {
    echo "❌ Build failed!"
    FAILED=1
}

# 2. Run ALL tests (unit + integration)
echo ""
echo "Step 2: Running ALL tests..."
echo "----------------------------"
dotnet test -c Release --no-build --verbosity normal || {
    echo "❌ Tests failed!"
    FAILED=1
}

# 3. Check code coverage (if coverlet is installed)
echo ""
echo "Step 3: Checking test coverage..."
echo "---------------------------------"
if command -v coverlet &> /dev/null; then
    dotnet test --no-build --collect:"XPlat Code Coverage" --verbosity quiet || {
        echo "⚠️  Coverage collection failed (non-critical)"
    }
else
    echo "ℹ️  Coverage tool not installed (install with: dotnet tool install -g coverlet.console)"
fi

# 4. Build documentation
echo ""
echo "Step 4: Building documentation..."
echo "---------------------------------"
if [ -f "build/docfx/docfx.json" ]; then
    cd build/docfx
    docfx build --warningsAsErrors false || {
        echo "⚠️  Documentation build has warnings (non-critical)"
    }
    cd ../..
else
    echo "ℹ️  Documentation not configured"
fi

# 5. Create NuGet package
echo ""
echo "Step 5: Creating NuGet package..."
echo "---------------------------------"
dotnet pack -c Release --no-build --verbosity normal -o ./test-packages || {
    echo "❌ Package creation failed!"
    FAILED=1
}

# 6. Validate package contents
echo ""
echo "Step 6: Validating package..."
echo "-----------------------------"
if [ -d "./test-packages" ]; then
    PACKAGE=$(ls ./test-packages/*.nupkg 2>/dev/null | head -1)
    if [ -n "$PACKAGE" ]; then
        unzip -l "$PACKAGE" | grep -E "(dll|xml|md)" > /dev/null || {
            echo "⚠️  Package might be missing expected files"
        }
        echo "✓ Package created: $PACKAGE"
    fi
    rm -rf ./test-packages
fi

# 7. Check for common issues
echo ""
echo "Step 7: Checking for common issues..."
echo "-------------------------------------"

# Check for TODO comments
TODO_COUNT=$(grep -r "TODO" src/ --include="*.cs" 2>/dev/null | wc -l | tr -d ' ')
if [ "$TODO_COUNT" -gt "0" ]; then
    echo "ℹ️  Found $TODO_COUNT TODO comments in code"
fi

# Check for uncommitted changes
if [ -n "$(git status --porcelain)" ]; then
    echo "⚠️  You have uncommitted changes"
fi

# Final report
echo ""
echo "=============================="
if [ $FAILED -eq 0 ]; then
    echo "✅ ALL TESTS PASSED!"
    echo "Ready to ship with confidence!"
else
    echo "❌ SOME TESTS FAILED"
    echo "Fix issues above before pushing!"
    exit 1
fi