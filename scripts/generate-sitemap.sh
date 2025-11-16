#!/bin/bash

# Sitemap Generator for Documentation
# Creates comprehensive sitemap.xml for better SEO and navigation

set -e

YELLOW='\033[1;33m'
GREEN='\033[0;32m'
NC='\033[0m'

BASE_URL="https://jacob-mellor.github.io/curl-dot-net"
DOCS_DIR="docs"
OUTPUT_FILE="sitemap.xml"
DATE=$(date +%Y-%m-%d)

echo "🗺️  Generating sitemap.xml..."

# Start sitemap
cat > "$OUTPUT_FILE" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9"
        xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
        xsi:schemaLocation="http://www.sitemaps.org/schemas/sitemap/0.9
        http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd">
EOF

# Function to add URL entry
add_url() {
    local path="$1"
    local priority="$2"
    local changefreq="$3"

    # Convert markdown path to HTML URL
    local url_path=$(echo "$path" | sed 's/\.md$/\.html/g' | sed 's/README\.html/index.html/g')

    cat >> "$OUTPUT_FILE" << EOF
  <url>
    <loc>$BASE_URL/$url_path</loc>
    <lastmod>$DATE</lastmod>
    <changefreq>$changefreq</changefreq>
    <priority>$priority</priority>
  </url>
EOF
}

# Add homepage
add_url "" "1.0" "weekly"

# Add main sections with high priority
add_url "manual/" "0.9" "weekly"
add_url "manual/getting-started/" "0.9" "weekly"
add_url "manual/tutorials/" "0.8" "weekly"
add_url "manual/api-guide/" "0.8" "weekly"
add_url "manual/cookbook/" "0.8" "weekly"

# Add all documentation files
find "$DOCS_DIR" -name "*.md" -type f | while read -r file; do
    # Skip README files in subdirectories (they become index.html)
    relative_path=${file#$DOCS_DIR/}

    # Determine priority based on depth and location
    depth=$(echo "$relative_path" | tr '/' '\n' | wc -l)

    if [[ "$relative_path" == *"getting-started"* ]]; then
        priority="0.8"
    elif [[ "$relative_path" == *"tutorials"* ]]; then
        priority="0.7"
    elif [[ "$relative_path" == *"cookbook/beginner"* ]]; then
        priority="0.7"
    elif [[ "$relative_path" == *"api-guide"* ]]; then
        priority="0.6"
    elif [[ "$relative_path" == *"troubleshooting"* ]]; then
        priority="0.6"
    else
        priority=$(echo "scale=1; 0.5 - ($depth * 0.1)" | bc)
        if (( $(echo "$priority < 0.3" | bc -l) )); then
            priority="0.3"
        fi
    fi

    # Change frequency based on type
    if [[ "$relative_path" == *"README.md" ]]; then
        changefreq="weekly"
    elif [[ "$relative_path" == *"cookbook"* ]]; then
        changefreq="monthly"
    else
        changefreq="monthly"
    fi

    # Convert to manual/ path structure
    url_path="manual/$relative_path"
    add_url "$url_path" "$priority" "$changefreq"
done

# Add special pages
add_url "api/" "0.7" "monthly"

# Close sitemap
cat >> "$OUTPUT_FILE" << EOF
</urlset>
EOF

# Count URLs
URL_COUNT=$(grep -c "<url>" "$OUTPUT_FILE")

echo -e "${GREEN}✅ Sitemap generated successfully${NC}"
echo "   - URLs indexed: $URL_COUNT"
echo "   - Output file: $OUTPUT_FILE"
echo ""
echo -e "${YELLOW}Next steps:${NC}"
echo "1. Copy sitemap.xml to gh-pages branch root"
echo "2. Submit to Google Search Console"
echo "3. Add to robots.txt:"
echo "   Sitemap: $BASE_URL/sitemap.xml"