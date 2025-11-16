#!/bin/bash

# Branch Validation Script
# Ensures master and gh-pages branches remain properly separated

set -e

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo "🔍 Branch Validation Tool"
echo "========================="
echo ""

ERRORS_FOUND=false
CURRENT_BRANCH=$(git branch --show-current)

# Function to check master branch
check_master_branch() {
    echo -e "${BLUE}Checking master branch...${NC}"

    git checkout master --quiet

    # Check for built documentation files
    BUILT_FILES=$(find . -name "*.html" -not -path "./.git/*" -not -path "./node_modules/*" 2>/dev/null | head -20 || true)

    if [ -n "$BUILT_FILES" ]; then
        echo -e "${RED}❌ Found HTML files in master branch:${NC}"
        echo "$BUILT_FILES" | head -10
        ERRORS_FOUND=true
    fi

    # Check for _site directories
    if [ -d "_site" ] || [ -d "docs/_site" ] || [ -d "build/docfx/_site" ]; then
        echo -e "${RED}❌ Found _site directory in master branch${NC}"
        ERRORS_FOUND=true
    fi

    # Check for minified JavaScript
    MINIFIED_JS=$(find . -name "*.min.js" -not -path "./.git/*" -not -path "./node_modules/*" 2>/dev/null | head -10 || true)
    if [ -n "$MINIFIED_JS" ]; then
        echo -e "${RED}❌ Found minified JS files in master branch:${NC}"
        echo "$MINIFIED_JS" | head -5
        ERRORS_FOUND=true
    fi

    # Check for documentation build artifacts
    if [ -f "docs/manifest.json" ] || [ -f "docs/xrefmap.yml" ] || [ -f "docs/index.json" ]; then
        echo -e "${RED}❌ Found DocFX build artifacts in docs/ directory${NC}"
        ERRORS_FOUND=true
    fi

    if [ "$ERRORS_FOUND" = false ]; then
        echo -e "${GREEN}✅ Master branch is clean${NC}"
    fi
}

# Function to check gh-pages branch
check_gh_pages_branch() {
    echo -e "${BLUE}Checking gh-pages branch...${NC}"

    if ! git show-ref --verify --quiet refs/heads/gh-pages; then
        echo -e "${YELLOW}⚠️  gh-pages branch doesn't exist${NC}"
        return
    fi

    git checkout gh-pages --quiet

    # Check for source code files
    SOURCE_FILES=$(find . -name "*.cs" -o -name "*.csproj" -o -name "*.sln" 2>/dev/null | head -10 || true)

    if [ -n "$SOURCE_FILES" ]; then
        echo -e "${RED}❌ Found source code files in gh-pages branch:${NC}"
        echo "$SOURCE_FILES" | head -10
        ERRORS_FOUND=true
    fi

    # Check for test files
    if [ -d "tests" ]; then
        echo -e "${RED}❌ Found tests directory in gh-pages branch${NC}"
        ERRORS_FOUND=true
    fi

    # Check for source directory
    if [ -d "src" ]; then
        echo -e "${RED}❌ Found src directory in gh-pages branch${NC}"
        ERRORS_FOUND=true
    fi

    # Check for build scripts
    SCRIPTS=$(find . -name "*.sh" -not -path "./.git/*" 2>/dev/null | head -10 || true)
    if [ -n "$SCRIPTS" ]; then
        echo -e "${RED}❌ Found shell scripts in gh-pages branch:${NC}"
        echo "$SCRIPTS" | head -5
        ERRORS_FOUND=true
    fi

    # Check for required documentation files
    if [ ! -f "index.html" ]; then
        echo -e "${YELLOW}⚠️  Missing index.html in gh-pages${NC}"
    fi

    if [ ! -f ".nojekyll" ]; then
        echo -e "${YELLOW}⚠️  Missing .nojekyll file in gh-pages${NC}"
    fi

    if [ "$ERRORS_FOUND" = false ]; then
        echo -e "${GREEN}✅ gh-pages branch is clean${NC}"
    fi
}

# Function to check file counts
check_file_counts() {
    echo -e "${BLUE}Analyzing file counts...${NC}"

    git checkout master --quiet
    MASTER_CS_COUNT=$(find . -name "*.cs" -not -path "./.git/*" 2>/dev/null | wc -l || echo 0)
    MASTER_HTML_COUNT=$(find . -name "*.html" -not -path "./.git/*" -not -path "./node_modules/*" 2>/dev/null | wc -l || echo 0)

    git checkout gh-pages --quiet 2>/dev/null || true
    if [ $? -eq 0 ]; then
        GHPAGES_CS_COUNT=$(find . -name "*.cs" -not -path "./.git/*" 2>/dev/null | wc -l || echo 0)
        GHPAGES_HTML_COUNT=$(find . -name "*.html" -not -path "./.git/*" 2>/dev/null | wc -l || echo 0)

        echo ""
        echo "File Statistics:"
        echo "  Master branch:   ${MASTER_CS_COUNT} .cs files, ${MASTER_HTML_COUNT} .html files"
        echo "  gh-pages branch: ${GHPAGES_CS_COUNT} .cs files, ${GHPAGES_HTML_COUNT} .html files"

        if [ "$MASTER_HTML_COUNT" -gt 5 ]; then
            echo -e "${YELLOW}⚠️  Master has ${MASTER_HTML_COUNT} HTML files (should be minimal)${NC}"
        fi

        if [ "$GHPAGES_CS_COUNT" -gt 0 ]; then
            echo -e "${RED}❌ gh-pages has ${GHPAGES_CS_COUNT} C# files (should be 0)${NC}"
            ERRORS_FOUND=true
        fi
    fi
}

# Main execution
echo "Starting validation..."
echo ""

check_master_branch
echo ""
check_gh_pages_branch
echo ""
check_file_counts

# Return to original branch
git checkout "$CURRENT_BRANCH" --quiet

echo ""
echo "================================="
if [ "$ERRORS_FOUND" = true ]; then
    echo -e "${RED}❌ Validation FAILED - Branches are contaminated${NC}"
    echo ""
    echo "To fix these issues, run:"
    echo "  ./scripts/fix-branches.sh"
    exit 1
else
    echo -e "${GREEN}✅ Validation PASSED - Branches are properly separated${NC}"
fi