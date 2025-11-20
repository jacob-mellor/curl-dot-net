#!/bin/bash
# Test GitHub Actions workflows locally using act
# This script helps verify CI/CD works before pushing to GitHub

set -e

echo "======================================="
echo "GitHub Actions Local Testing Tool"
echo "======================================="
echo

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker is not running. Please start Docker Desktop first."
    echo "   Install with: brew install --cask docker"
    exit 1
fi

# Check if act is installed
if ! command -v act &> /dev/null; then
    echo "❌ act is not installed."
    echo "   Install with: brew install act"
    exit 1
fi

echo "✅ Docker is running"
echo "✅ act is installed"
echo

# Function to run a workflow
run_workflow() {
    local workflow=$1
    local job=$2
    local event=${3:-pull_request}

    echo "Testing: $workflow (job: $job)"
    echo "--------------------------------------"

    # Use our config file for consistent settings
    act $event \
        --job $job \
        --config-file .github/devops/act-config.yaml \
        --container-architecture linux/amd64 \
        --env GITHUB_ACTIONS=true \
        --verbose
}

# Menu for selecting what to test
echo "What would you like to test?"
echo "1) CI Smoke Tests"
echo "2) NuGet Package Build"
echo "3) Code Quality Check"
echo "4) All workflows (dry run)"
echo "5) Exit"
echo

read -p "Select option (1-5): " choice

case $choice in
    1)
        echo "Running CI Smoke Tests..."
        run_workflow "ci-smoke.yml" "smoke-integration-test" "pull_request"
        ;;
    2)
        echo "Running NuGet Package Build..."
        run_workflow "nuget-build-validation.yml" "build-nuget-package" "pull_request"
        ;;
    3)
        echo "Running Code Quality Check..."
        run_workflow "code-quality.yml" "quality-check" "pull_request"
        ;;
    4)
        echo "Running all workflows (dry run)..."
        act --dryrun --config-file .github/devops/act-config.yaml
        ;;
    5)
        echo "Exiting..."
        exit 0
        ;;
    *)
        echo "Invalid option"
        exit 1
        ;;
esac

echo
echo "======================================="
echo "✅ Local testing complete!"
echo "======================================="
echo
echo "Notes:"
echo "- .NET 10 is automatically excluded in CI environment"
echo "- Windows-specific tests may not run correctly in containers"
echo "- For full validation, push to a PR branch"