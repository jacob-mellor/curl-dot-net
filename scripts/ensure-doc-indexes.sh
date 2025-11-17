#!/bin/bash

# Documentation Index Enforcement Script
# Ensures every documentation directory has an index file to prevent 404s

set -e

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

echo "📚 Documentation Index Enforcement Tool"
echo "======================================="
echo ""

CREATED_COUNT=0
EXISTING_COUNT=0
DOCS_BASE="docs"

# Function to create index file if missing
ensure_index_file() {
    local dir="$1"
    local parent_name=$(basename "$(dirname "$dir")")
    local dir_name=$(basename "$dir")

    # Check for existing index files
    if [ -f "$dir/README.md" ] || [ -f "$dir/index.md" ]; then
        EXISTING_COUNT=$((EXISTING_COUNT + 1))
        echo -e "${GREEN}✓${NC} $dir has index file"
        return
    fi

    # Create appropriate index file
    echo -e "${YELLOW}⚠${NC}  Missing index in $dir - creating..."

    # Determine the content based on directory
    case "$dir_name" in
        "api")
            create_api_index "$dir"
            ;;
        "tutorials")
            create_tutorials_index "$dir"
            ;;
        "cookbook")
            create_cookbook_index "$dir"
            ;;
        "troubleshooting")
            create_troubleshooting_index "$dir"
            ;;
        "authentication")
            create_authentication_index "$dir"
            ;;
        "migration")
            create_migration_index "$dir"
            ;;
        "advanced")
            create_advanced_index "$dir"
            ;;
        "examples")
            create_examples_index "$dir"
            ;;
        "getting-started")
            create_getting_started_index "$dir"
            ;;
        "error-handling")
            create_error_handling_index "$dir"
            ;;
        "http-methods")
            create_http_methods_index "$dir"
            ;;
        *)
            create_generic_index "$dir" "$dir_name"
            ;;
    esac

    CREATED_COUNT=$((CREATED_COUNT + 1))
}

# Generic index creator
create_generic_index() {
    local dir="$1"
    local name="$2"
    local title=$(echo "$name" | sed 's/-/ /g' | sed 's/\b\(.\)/\u\1/g')

    cat > "$dir/README.md" << EOF
# $title

Welcome to the $title section of CurlDotNet documentation.

## Contents

EOF

    # List markdown files in directory
    for file in "$dir"/*.md; do
        if [ -f "$file" ] && [ "$(basename "$file")" != "README.md" ] && [ "$(basename "$file")" != "index.md" ]; then
            local filename=$(basename "$file" .md)
            local title=$(echo "$filename" | sed 's/-/ /g' | sed 's/\b\(.\)/\u\1/g')
            echo "- [$title]($filename.md)" >> "$dir/README.md"
        fi
    done

    # List subdirectories
    for subdir in "$dir"/*/; do
        if [ -d "$subdir" ]; then
            local dirname=$(basename "$subdir")
            local title=$(echo "$dirname" | sed 's/-/ /g' | sed 's/\b\(.\)/\u\1/g')
            echo "- [$title]($dirname/)" >> "$dir/README.md"
        fi
    done
}

create_api_index() {
    cat > "$1/README.md" << 'EOF'
# API Documentation

Complete API reference for CurlDotNet.

## API Surfaces

### String API
Simple curl command execution using strings.

```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");
```

### Builder API
Fluent interface for building requests.

```csharp
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.github.com")
    .ExecuteAsync();
```

### LibCurl API
Low-level curl implementation.

```csharp
using var curl = new LibCurl();
var result = await curl.GetAsync("https://api.github.com");
```

## Namespaces

- `CurlDotNet` - Main namespace
- `CurlDotNet.Core` - Core functionality
- `CurlDotNet.Exceptions` - Exception types
- `CurlDotNet.Middleware` - Middleware support

## Quick Links

- [Curl Class](CurlDotNet.Curl.html)
- [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.html)
- [LibCurl](CurlDotNet.Lib.LibCurl.html)
- [CurlResult](CurlDotNet.Core.CurlResult.html)
EOF
}

create_tutorials_index() {
    cat > "$1/README.md" << 'EOF'
# Tutorials

Step-by-step guides to learn CurlDotNet.

## Getting Started
1. [What is .NET?](01-what-is-dotnet.md)
2. [What is curl?](02-what-is-curl.md)
3. [Understanding Async/Await](03-what-is-async.md)
4. [Your First Request](04-your-first-request.md)

## Core Concepts
5. Understanding Results
6. Handling Errors
7. Working with JSON
8. Headers Explained
9. Authentication Basics

## Advanced Topics
10. Files and Downloads
11. Forms and Data
12. Cancellation Tokens
13. Parallel Requests
14. Debugging Requests

## Navigation
- [Back to Documentation](../README.md)
- [API Reference](../api/README.md)
- [Cookbook](../cookbook/README.md)
EOF
}

create_cookbook_index() {
    cat > "$1/README.md" << 'EOF'
# Cookbook

Practical recipes and examples for common tasks.

## Beginner Recipes
- [Simple GET Request](beginner/simple-get.md)
- [POST JSON Data](beginner/send-json.md)
- [Handle Errors](beginner/handle-errors.md)
- [Upload Files](beginner/upload-file.md)
- [Download Files](beginner/download-file.md)
- [POST Forms](beginner/post-form.md)
- [Call APIs](beginner/call-api.md)

## Intermediate Recipes
- [API Client Class](intermediate/api-client-class.md)
- [Retry Logic](intermediate/retry-logic.md)
- [Progress Tracking](intermediate/progress-tracking.md)
- [Session Cookies](intermediate/session-cookies.md)
- [Rate Limiting](intermediate/rate-limiting.md)
- [File Management](intermediate/file-management.md)

## Advanced Patterns
- [Pagination](patterns/pagination.md)
- [Polling](patterns/polling.md)
- [Webhooks](patterns/webhooks.md)
- [Batch Processing](patterns/batch-processing.md)

## Real World Examples
- [GitHub Integration](real-world/github-integration.md)
- [Slack Notifications](real-world/slack-notifications.md)
- [Weather API](real-world/weather-api.md)
- [OAuth Flow](real-world/oauth-flow.md)
- [Stripe Payments](real-world/stripe-payments.md)
EOF
}

create_troubleshooting_index() {
    cat > "$1/README.md" << 'EOF'
# Troubleshooting Guide

Solutions to common issues and frequently asked questions.

## Quick Solutions

### Connection Issues
- Check network connectivity
- Verify URL is correct
- Check firewall settings

### Authentication Problems
- Verify credentials
- Check token expiration
- Review API key permissions

### Performance Issues
- Enable connection pooling
- Use async/await properly
- Consider parallel requests

## Common Issues
- [Detailed Troubleshooting](common-issues.md)

## Getting Help
- [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)
- [Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
- [Documentation](https://jacob-mellor.github.io/curl-dot-net/)
EOF
}

create_authentication_index() {
    cat > "$1/README.md" << 'EOF'
# Authentication Guide

Complete guide to authentication methods in CurlDotNet.

## Supported Methods

### Basic Authentication
```csharp
var result = await Curl.ExecuteAsync("curl -u username:password https://api.example.com");
```
[Learn more](basic-auth.md)

### Bearer Tokens
```csharp
var result = await Curl.ExecuteAsync("curl -H 'Authorization: Bearer token' https://api.example.com");
```
[Learn more](bearer-tokens.md)

### API Keys
```csharp
var result = await Curl.ExecuteAsync("curl -H 'X-API-Key: key' https://api.example.com");
```
[Learn more](api-keys.md)

## Quick Links
- [OAuth Implementation](../cookbook/real-world/oauth-flow.md)
- [Security Best Practices](../advanced/security.md)
EOF
}

create_migration_index() {
    cat > "$1/README.md" << 'EOF'
# Migration Guides

Guides for migrating to CurlDotNet from other HTTP clients.

## Available Guides

### From HttpClient
- [Migration from HttpClient](from-httpclient.md)
- Minimal code changes
- Performance improvements
- Feature comparison

### From RestSharp
- [Migration from RestSharp](from-restsharp.md)
- API mapping guide
- Common patterns
- Best practices

## Why Migrate?

- **Simpler API** - Use familiar curl syntax
- **Better Performance** - Optimized for .NET
- **No Dependencies** - Pure .NET implementation
- **Modern Features** - Full async/await support
EOF
}

create_advanced_index() {
    cat > "$1/README.md" << 'EOF'
# Advanced Topics

Advanced usage patterns and optimization techniques.

## Topics

### Performance
- [Performance Optimization](performance.md)
- Connection pooling
- Request pipelining
- Memory management

### Custom Implementations
- [Custom HTTP Client](custom-http-client.md)
- Handler pipeline
- Protocol extensions

### Middleware
- [Middleware System](middleware/README.md)
- Custom middleware
- Built-in middleware

### Testing
- [Testing Strategies](testing.md)
- Mock responses
- Integration testing

### Architecture
- [Circuit Breaker Pattern](circuit-breaker.md)
- Retry policies
- Resilience patterns
EOF
}

create_examples_index() {
    cat > "$1/README.md" << 'EOF'
# Examples

Complete working examples for CurlDotNet.

## Quick Examples

### GET Request
```csharp
var result = await Curl.ExecuteAsync("curl https://api.github.com");
Console.WriteLine(result.Body);
```

### POST JSON
```csharp
var json = "{\"name\":\"John\"}";
var result = await Curl.ExecuteAsync($"curl -X POST -d '{json}' -H 'Content-Type: application/json' https://api.example.com");
```

## Complete Examples

- Console Application
- ASP.NET Core Integration
- Blazor WebAssembly
- WPF Application
- Background Service

## Running Examples

1. Clone the repository
2. Navigate to examples directory
3. Run with `dotnet run`

## More Resources

- [Cookbook](../cookbook/README.md)
- [Tutorials](../tutorials/README.md)
- [API Documentation](../api/README.md)
EOF
}

create_getting_started_index() {
    cat > "$1/README.md" << 'EOF'
# Getting Started

Quick start guide for CurlDotNet.

## Installation

### NuGet Package Manager
```bash
Install-Package CurlDotNet
```

### .NET CLI
```bash
dotnet add package CurlDotNet
```

### PackageReference
```xml
<PackageReference Include="CurlDotNet" Version="*" />
```

## First Request

```csharp
using CurlDotNet;

var result = await Curl.ExecuteAsync("curl https://api.github.com");
Console.WriteLine($"Status: {result.StatusCode}");
Console.WriteLine($"Body: {result.Body}");
```

## Next Steps

- [Installation Guide](installation.md)
- [Tutorials](../tutorials/README.md)
- [API Reference](../api/README.md)
- [Examples](../examples/README.md)
EOF
}

create_error_handling_index() {
    cat > "$1/README.md" << 'EOF'
# Error Handling

Comprehensive error handling guide for CurlDotNet.

## Exception Hierarchy

```
CurlException
├── CurlConnectionException
├── CurlTimeoutException
├── CurlAuthenticationException
├── CurlHttpException
└── CurlParsingException
```

## Basic Error Handling

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
    result.EnsureSuccess();
}
catch (CurlException ex)
{
    Console.WriteLine($"Curl error: {ex.Message}");
    Console.WriteLine($"Error code: {ex.ErrorCode}");
}
```

## Topics

- [Error Handling Patterns](error-handling-patterns.md)
- Exception types
- Retry strategies
- Circuit breaker pattern

## Best Practices

1. Always handle CurlException
2. Use specific exception types when needed
3. Implement retry logic for transient failures
4. Log errors appropriately
EOF
}

create_http_methods_index() {
    cat > "$1/README.md" << 'EOF'
# HTTP Methods

Complete guide to HTTP methods in CurlDotNet.

## Supported Methods

### GET
```csharp
await Curl.GetAsync("https://api.example.com");
```

### POST
```csharp
await Curl.PostAsync("https://api.example.com", "{\"data\":\"value\"}");
```

### PUT
```csharp
await Curl.ExecuteAsync("curl -X PUT -d 'data' https://api.example.com");
```

### DELETE
```csharp
await Curl.ExecuteAsync("curl -X DELETE https://api.example.com");
```

### PATCH
```csharp
await Curl.ExecuteAsync("curl -X PATCH -d 'data' https://api.example.com");
```

### HEAD
```csharp
await Curl.ExecuteAsync("curl -I https://api.example.com");
```

### OPTIONS
```csharp
await Curl.ExecuteAsync("curl -X OPTIONS https://api.example.com");
```

## Custom Methods

```csharp
await Curl.ExecuteAsync("curl -X CUSTOM https://api.example.com");
```

## Related Topics

- [API Reference](../api/README.md)
- [Cookbook](../cookbook/README.md)
EOF
}

# Main execution
echo -e "${BLUE}Scanning documentation directories...${NC}"
echo ""

# Process main docs directory
if [ -d "$DOCS_BASE" ]; then
    ensure_index_file "$DOCS_BASE"

    # Process all subdirectories
    for dir in "$DOCS_BASE"/*/ ; do
        if [ -d "$dir" ]; then
            ensure_index_file "$dir"

            # Process nested subdirectories (like cookbook/beginner)
            for subdir in "$dir"/*/ ; do
                if [ -d "$subdir" ]; then
                    ensure_index_file "$subdir"
                fi
            done
        fi
    done
fi

# Also check build/docfx if it exists
if [ -d "build/docfx" ]; then
    ensure_index_file "build/docfx"
fi

echo ""
echo "======================================="
echo -e "${GREEN}Documentation Index Enforcement Complete${NC}"
echo ""
echo "Summary:"
echo "  - Existing indexes: $EXISTING_COUNT"
echo "  - Created indexes: $CREATED_COUNT"
echo ""

if [ $CREATED_COUNT -gt 0 ]; then
    echo -e "${YELLOW}Action Required:${NC}"
    echo "  New index files were created. Please review and commit them."
    echo ""
    echo "  git add docs/**/README.md"
    echo "  git commit -m \"Add missing documentation index files\""
fi

# Create hook to ensure this is maintained
if [ ! -f ".git/hooks/pre-push" ]; then
    echo ""
    echo -e "${BLUE}Installing pre-push hook...${NC}"

    cat > .git/hooks/pre-push << 'EOF'
#!/bin/bash
# Pre-push hook to ensure documentation indexes exist

echo "🔍 Checking documentation indexes..."

MISSING=0
for dir in docs/*/ docs/*/*/; do
    if [ -d "$dir" ]; then
        if [ ! -f "$dir/README.md" ] && [ ! -f "$dir/index.md" ]; then
            echo "❌ Missing index in $dir"
            MISSING=$((MISSING + 1))
        fi
    fi
done

if [ $MISSING -gt 0 ]; then
    echo ""
    echo "❌ Found $MISSING directories without index files"
    echo "Run: ./scripts/ensure-doc-indexes.sh"
    exit 1
fi

echo "✅ All documentation directories have indexes"
EOF

    chmod +x .git/hooks/pre-push
    echo -e "${GREEN}✓ Pre-push hook installed${NC}"
fi

exit 0