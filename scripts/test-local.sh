#!/bin/bash
# Test locally - FAST. Don't wait for CI.

echo "🧪 Local Testing (No BS, just results)"
echo "======================================"

# Quick build
echo "Building..."
dotnet build --verbosity quiet || exit 1

# Run tests with useful output
echo "Running tests..."
dotnet test --no-build --verbosity normal || {
    echo ""
    echo "❌ TESTS FAILED"
    echo "Fix them NOW, don't push broken code."
    exit 1
}

echo ""
echo "✅ All tests pass!"
echo "Safe to commit and push."