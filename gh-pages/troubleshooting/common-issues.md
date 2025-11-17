# Common Issues and Solutions

This page covers the most frequently encountered issues when using CurlDotNet and their solutions.

## Top 10 Most Common Issues

### 1. ❌ "Could not find CurlDotNet package"

**Problem:** NuGet can't find the CurlDotNet package.

**Solution:**
```bash
# Make sure you're using the correct package name
dotnet add package CurlDotNet

# If still not working, clear NuGet cache
dotnet nuget locals all --clear
```

### 2. ❌ "Invalid curl command syntax"

**Problem:** `CurlParsingException: Invalid command format`

**Solution:**
```csharp
// BAD: Missing quotes around URL with query parameters
var result = await Curl.ExecuteAsync("curl https://api.example.com?key=value&foo=bar");

// GOOD: Use quotes or escape properly
var result = await Curl.ExecuteAsync("curl \"https://api.example.com?key=value&foo=bar\"");

// BETTER: Use verbatim string
var result = await Curl.ExecuteAsync(@"curl 'https://api.example.com?key=value&foo=bar'");
```

### 3. ❌ "SSL certificate problem"

**Problem:** `CurlSslCertificateProblemException: unable to verify the first certificate`

**Solution:**
```csharp
// For development/testing only:
var result = await Curl.ExecuteAsync("curl -k https://localhost:5001");

// For production, install the certificate properly or specify CA:
var result = await Curl.ExecuteAsync("curl --cacert /path/to/ca-cert.pem https://api.example.com");
```

### 4. ❌ "Connection timeout"

**Problem:** `CurlOperationTimeoutException: Operation timed out`

**Solution:**
```csharp
// Increase timeout to 60 seconds
var result = await Curl.ExecuteAsync("curl --max-time 60 https://slow-api.example.com");

// Or set different timeouts for connection and total
var result = await Curl.ExecuteAsync(@"
    curl --connect-timeout 10 \
         --max-time 60 \
         https://api.example.com
");
```

### 5. ❌ "401 Unauthorized"

**Problem:** API returns 401 even though credentials seem correct.

**Solution:**
```csharp
// Check token format - Bearer needs capital B
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com \
    -H 'Authorization: Bearer YOUR_TOKEN'
");

// For Basic auth, check encoding
var result = await Curl.ExecuteAsync("curl -u username:password https://api.example.com");

// For API keys, check header name
var result = await Curl.ExecuteAsync(@"
    curl https://api.example.com \
    -H 'X-API-Key: YOUR_KEY'
");
```

### 6. ❌ "JSON parsing error"

**Problem:** Can't parse response as JSON.

**Solution:**
```csharp
// Check if response is actually JSON
var result = await Curl.ExecuteAsync("curl https://api.example.com");
if (result.IsSuccess)
{
    try
    {
        var data = result.ParseJson<MyModel>();
    }
    catch (JsonException)
    {
        // Response might be HTML or plain text
        Console.WriteLine($"Response was: {result.Content}");
    }
}
```

### 7. ❌ "Proxy authentication required"

**Problem:** Corporate proxy blocking requests.

**Solution:**
```csharp
// Use proxy with authentication
var result = await Curl.ExecuteAsync(@"
    curl -x http://proxy.company.com:8080 \
         -U proxyuser:proxypass \
         https://external-api.com
");

// Or use environment variables
Environment.SetEnvironmentVariable("http_proxy", "http://proxy:8080");
var result = await Curl.ExecuteAsync("curl https://api.example.com");
```

### 8. ❌ "Request entity too large"

**Problem:** Server rejects large uploads with 413 error.

**Solution:**
```csharp
// Check file size before upload
var fileInfo = new FileInfo("large-file.zip");
if (fileInfo.Length > 100_000_000) // 100MB
{
    // Use chunked upload or compression
    var result = await Curl.ExecuteAsync(@"
        curl -X POST https://api.example.com/upload \
             -H 'Transfer-Encoding: chunked' \
             -T large-file.zip
    ");
}
```

### 9. ❌ "Too many redirects"

**Problem:** `CurlTooManyRedirectsException`

**Solution:**
```csharp
// Increase redirect limit
var result = await Curl.ExecuteAsync("curl -L --max-redirs 10 https://bit.ly/short-url");

// Or disable redirects to see what's happening
var result = await Curl.ExecuteAsync("curl https://bit.ly/short-url");
Console.WriteLine($"Redirect to: {result.Headers["Location"]}");
```

### 10. ❌ "Method not allowed"

**Problem:** 405 error when calling API.

**Solution:**
```csharp
// Make sure you're using the correct HTTP method
// GET (default)
var result = await Curl.ExecuteAsync("curl https://api.example.com/resource");

// POST
var result = await Curl.ExecuteAsync("curl -X POST https://api.example.com/resource");

// PUT
var result = await Curl.ExecuteAsync("curl -X PUT https://api.example.com/resource");

// DELETE
var result = await Curl.ExecuteAsync("curl -X DELETE https://api.example.com/resource");
```

## Platform-Specific Issues

### Windows

**Issue:** "curl command not recognized"

```csharp
// CurlDotNet doesn't require curl.exe to be installed
// It's a pure .NET implementation
var result = await Curl.ExecuteAsync("curl https://api.example.com");
```

### Linux/Mac

**Issue:** "Permission denied" when writing files

```csharp
// Specify writable directory
var result = await Curl.ExecuteAsync(@"
    curl https://example.com/file.zip \
         -o /tmp/file.zip
");
```

### Docker

**Issue:** DNS resolution fails in container

```dockerfile
# Add DNS to docker run
docker run --dns 8.8.8.8 myapp
```

## Performance Issues

### Slow Requests

```csharp
// Enable HTTP/2 for better performance
var result = await Curl.ExecuteAsync("curl --http2 https://api.example.com");

// Reuse connections
var curl = new LibCurl();
for (int i = 0; i < 100; i++)
{
    var result = await curl.GetAsync($"https://api.example.com/item/{i}");
}

// Use compression
var result = await Curl.ExecuteAsync("curl --compressed https://api.example.com");
```

### Memory Issues

```csharp
// Stream large files instead of loading to memory
var result = await Curl.ExecuteAsync(@"
    curl https://example.com/huge-file.zip \
         -o huge-file.zip
");

// Don't use ParseJson for large responses
// Process as stream instead
```

## Debugging Tips

### 1. Enable Verbose Output
```csharp
var result = await Curl.ExecuteAsync("curl -v https://api.example.com");
Console.WriteLine(result.VerboseOutput);
```

### 2. Check Exact Request Being Sent
```csharp
var result = await Curl.ExecuteAsync("curl --trace-ascii - https://api.example.com");
```

### 3. Test with curl CLI First
```bash
# Test the exact command in terminal first
curl -v https://api.example.com
```

### 4. Simplify to Find Issue
```csharp
// Start simple
var result = await Curl.ExecuteAsync("curl https://api.example.com");

// Then add options one by one
var result = await Curl.ExecuteAsync("curl -H 'Accept: application/json' https://api.example.com");
```

## Related Resources

- [Troubleshooting Guide](README.md)
- [Connection Errors](connection-errors.md)
- [Exception Reference](/exceptions/)
- [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)

---
*If your issue isn't listed here, please [search existing issues](https://github.com/jacob-mellor/curl-dot-net/issues) or [create a new one](https://github.com/jacob-mellor/curl-dot-net/issues/new).*