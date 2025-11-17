#!/bin/bash
# Smoke test - Quick validation that everything works
# Run this before pushing to catch obvious issues FAST

echo "🚬 Smoke Test - Quick Validation"
echo "================================="

# 1. Quick build check
echo "→ Quick build..."
dotnet build --verbosity quiet || {
    echo "❌ BUILD BROKEN - Fix immediately!"
    exit 1
}

# 2. Run just the fast unit tests (not integration)
echo "→ Running fast tests only..."
dotnet test --no-build --verbosity quiet --filter "Category!=Integration" || {
    echo "❌ UNIT TESTS BROKEN - Fix before pushing!"
    exit 1
}

# 3. Check if NuGet package can be created
echo "→ Checking package creation..."
dotnet pack --no-build --verbosity quiet -o /tmp/smoke-test-pkg || {
    echo "❌ PACKAGE BROKEN - Can't create NuGet package!"
    rm -rf /tmp/smoke-test-pkg
    exit 1
}
rm -rf /tmp/smoke-test-pkg

echo ""
echo "✅ Smoke test PASSED!"
echo "Safe for quick push. Run test-all-locally.sh for full validation."