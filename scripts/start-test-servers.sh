#!/bin/bash

# Start test servers for CurlDotNet testing
# Provides reliable local alternatives to flaky public services

set -e

echo "🚀 Starting CurlDotNet Test Servers"
echo "===================================="

# Check if Docker is available
if command -v docker &> /dev/null; then
    echo "✅ Docker found. Starting containerized test servers..."

    # Check if docker-compose exists
    if [ -f "docker-compose.test.yml" ]; then
        echo "📦 Starting services with docker-compose..."
        docker-compose -f docker-compose.test.yml up -d

        echo "⏳ Waiting for services to be healthy..."
        sleep 5

        # Health check
        echo "🔍 Checking service health..."

        if curl -f http://localhost:8080/get > /dev/null 2>&1; then
            echo "✅ httpbin is running at http://localhost:8080"
        else
            echo "⚠️  httpbin not responding"
        fi

        if curl -f http://localhost:8081 > /dev/null 2>&1; then
            echo "✅ echo-server is running at http://localhost:8081"
        else
            echo "⚠️  echo-server not responding"
        fi

        if curl -f http://localhost:8082/__admin/health > /dev/null 2>&1; then
            echo "✅ WireMock is running at http://localhost:8082"
        else
            echo "⚠️  WireMock not responding"
        fi

        echo ""
        echo "📝 To stop servers: docker-compose -f docker-compose.test.yml down"
        echo "📊 To view logs: docker-compose -f docker-compose.test.yml logs -f"

    else
        echo "⚠️  docker-compose.test.yml not found"
        echo "    Using individual Docker containers..."

        # Start httpbin container
        if docker ps -a | grep -q curldotnet-httpbin; then
            echo "♻️  Restarting existing httpbin container..."
            docker start curldotnet-httpbin
        else
            echo "🆕 Creating new httpbin container..."
            docker run -d \
                --name curldotnet-httpbin \
                -p 8080:80 \
                kennethreitz/httpbin:latest
        fi

        echo ""
        echo "📝 To stop: docker stop curldotnet-httpbin"
        echo "📝 To remove: docker rm curldotnet-httpbin"
    fi

    echo ""
    echo "✅ Docker test servers started!"

else
    echo "⚠️  Docker not found. Falling back to public test services."
    echo ""
    echo "Available public alternatives:"
    echo "  1. httpbin.org       - https://httpbin.org"
    echo "  2. httpbingo.org     - https://httpbingo.org (Go implementation)"
    echo "  3. eu.httpbin.org    - https://eu.httpbin.org (European mirror)"
    echo "  4. postman-echo.com  - https://postman-echo.com"
    echo "  5. echo.hoppscotch.io- https://echo.hoppscotch.io"
    echo ""
    echo "To use local servers, install Docker from: https://www.docker.com/get-started"
fi

# Export environment variables for tests
echo ""
echo "🔧 Setting test environment variables..."
if [ -f "$HOME/.bashrc" ]; then
    echo "export HTTPBIN_URL=http://localhost:8080" >> "$HOME/.bashrc"
    echo "export USE_LOCAL_SERVICES=true" >> "$HOME/.bashrc"
fi

if [ -f "$HOME/.zshrc" ]; then
    echo "export HTTPBIN_URL=http://localhost:8080" >> "$HOME/.zshrc"
    echo "export USE_LOCAL_SERVICES=true" >> "$HOME/.zshrc"
fi

# For current session
export HTTPBIN_URL=http://localhost:8080
export USE_LOCAL_SERVICES=true

echo "✅ Environment variables set"
echo ""
echo "🎯 Ready to run tests with reliable local services!"
echo "   Run: dotnet test"