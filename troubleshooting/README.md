# Troubleshooting Guide

Welcome to the CurlDotNet troubleshooting guide. This page will help you diagnose and fix common issues.

## 🎯 Quick Links

- **[Common Issues](common-issues.html)** - Most frequent problems and solutions
- **[Error Code Reference](#error-code-reference)** - Understanding error codes
- **[Diagnostic Tools](#diagnostic-tools)** - Tools for debugging
- **[Getting Help](#getting-help)** - Where to ask for help

## 🔍 How to Use This Guide

### 1. Identify Your Problem

First, determine what type of issue you're experiencing:

| Symptom | Category | Guide Link |
|---------|----------|------------|
| Can't install or setup | Installation | [Installation Issues](common-issues.md#installation-issues) |
| DNS/hostname errors | Network | [DNS Errors](common-issues.md#dns-errors) |
| Timeout errors | Network | [Timeout Errors](common-issues.md#timeout-errors) |
| SSL/certificate errors | Security | [SSL Errors](common-issues.md#ssl-errors) |
| Authentication failures | Security | [Authentication Errors](common-issues.md#authentication-errors) |
| HTTP 4xx errors | Client | [HTTP Client Errors](common-issues.md#http-errors) |
| HTTP 5xx errors | Server | [HTTP Server Errors](common-issues.md#http-errors) |
| JSON parsing issues | Data | [JSON Errors](common-issues.md#json-parsing-errors) |
| Code won't compile | Development | [Compilation Errors](common-issues.md#compilation-errors) |

### 2. Follow the Solution Steps

Each issue page provides:
- Clear explanation of the problem
- Step-by-step solutions
- Code examples
- Prevention tips
- Links to additional resources

### 3. Still Stuck?

If the guides don't solve your problem:
1. Check [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
2. Search [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)
3. Create a new issue with details

## 🚨 Common Issues at a Glance

### Installation and Setup

**Problem:** `dotnet: command not found`
**Solution:** [Install .NET SDK](common-issues.md#dotnet-not-found) and restart terminal

**Problem:** `Package 'CurlDotNet' not found`
**Solution:** [Check NuGet configuration](common-issues.md#package-not-found)

**Problem:** Target framework not supported
**Solution:** [Update .csproj](common-issues.md#target-framework-issues)

### Network and Connectivity

**Problem:** `Could not resolve host`
**Solution:** [Fix DNS issues](common-issues.md#dns-errors)

**Problem:** `Operation timeout`
**Solution:** [Increase timeout or check network](common-issues.md#timeout-errors)

**Problem:** `Connection refused`
**Solution:** [Check URL and firewall](common-issues.md#connection-refused)

### Security and Authentication

**Problem:** SSL certificate error
**Solution:** [Update certificates or use -k for testing](common-issues.md#ssl-errors)

**Problem:** `401 Unauthorized`
**Solution:** [Fix authentication](common-issues.md#authentication-errors)

**Problem:** `403 Forbidden`
**Solution:** [Check permissions](common-issues.md#permission-errors)

### HTTP and API Issues

**Problem:** `404 Not Found`
**Solution:** [Verify URL](common-issues.md#http-errors)

**Problem:** `429 Too Many Requests`
**Solution:** [Implement rate limiting](common-issues.md#rate-limiting)

**Problem:** `500 Internal Server Error`
**Solution:** [Check server logs or try later](common-issues.md#server-errors)

### Data Handling

**Problem:** Invalid JSON
**Solution:** [Validate and parse JSON correctly](common-issues.md#json-parsing-errors)

**Problem:** Empty response
**Solution:** [Check status code and headers](common-issues.md#empty-response)

**Problem:** Encoding issues
**Solution:** [Specify correct encoding](common-issues.md#encoding-issues)

## 🔧 Error Code Reference

CurlDotNet provides specific exception types for different error scenarios. Each exception links to detailed documentation.

### Network Errors (CurlDnsException)

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://invalid-domain.example");
}
catch (CurlDnsException ex)
{
    // Error code 6: Could not resolve host
    Console.WriteLine($"DNS Error: {ex.Message}");
    Console.WriteLine($"Hostname: {ex.Hostname}");
    // Documentation: https://github.com/jacob-mellor/curl-dot-net/docs/troubleshooting/common-issues.md#dns-errors
}
```

### Timeout Errors (CurlTimeoutException)

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://slow-server.example");
}
catch (CurlTimeoutException ex)
{
    // Error code 28: Operation timeout
    Console.WriteLine($"Timeout: {ex.Message}");
    Console.WriteLine($"Timeout Duration: {ex.Timeout}");
    // Documentation: https://github.com/jacob-mellor/curl-dot-net/docs/troubleshooting/common-issues.md#timeout-errors
}
```

### SSL/TLS Errors (CurlSslException)

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://self-signed.example");
}
catch (CurlSslException ex)
{
    // Error codes 35, 51, 53, 54, 58, 59, 60, 64, 66, 77, 80, 82, 83, 90, 91, 98
    Console.WriteLine($"SSL Error: {ex.Message}");
    Console.WriteLine($"Certificate Issue: {ex.CertificateError}");
    // Documentation: https://github.com/jacob-mellor/curl-dot-net/docs/troubleshooting/common-issues.md#ssl-errors
}
```

### HTTP Errors (CurlHttpReturnedErrorException)

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com/not-found");
}
catch (CurlHttpReturnedErrorException ex)
{
    // Error code 22: HTTP returned error
    Console.WriteLine($"HTTP Error: {ex.StatusCode}");
    Console.WriteLine($"Response: {ex.ResponseBody}");
    // Documentation: https://github.com/jacob-mellor/curl-dot-net/docs/troubleshooting/common-issues.md#http-errors
}
```

### General Curl Errors (CurlException)

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl ftp://example.com");
}
catch (CurlException ex)
{
    // Catches all curl errors
    Console.WriteLine($"Curl Error {ex.ErrorCode}: {ex.Message}");
    // Documentation: https://github.com/jacob-mellor/curl-dot-net/docs/troubleshooting/
}
```

### Complete Error Code List

| Code | Exception Type | Description | Guide |
|------|----------------|-------------|-------|
| 6 | CurlDnsException | Could not resolve host | [DNS Errors](common-issues.md#dns-errors) |
| 7 | CurlException | Failed to connect to host | [Connection Issues](common-issues.md#connection-errors) |
| 22 | CurlHttpReturnedErrorException | HTTP error (4xx/5xx) | [HTTP Errors](common-issues.md#http-errors) |
| 28 | CurlTimeoutException | Operation timeout | [Timeout Errors](common-issues.md#timeout-errors) |
| 35 | CurlSslException | SSL connect error | [SSL Errors](common-issues.md#ssl-errors) |
| 51 | CurlSslException | Peer certificate invalid | [SSL Errors](common-issues.md#ssl-errors) |
| 52 | CurlException | Server returned nothing | [Server Errors](common-issues.md#server-errors) |
| 53 | CurlSslException | Crypto engine not found | [SSL Errors](common-issues.md#ssl-errors) |
| 60 | CurlSslException | Peer certificate cannot be authenticated | [SSL Errors](common-issues.md#ssl-errors) |

For a complete list, see [common-issues.md](common-issues.html).

## 🛠 Diagnostic Tools

### Enable Verbose Mode

See exactly what's happening with your request:

```csharp
var result = await Curl.ExecuteAsync("curl -v https://api.example.com");
```

This shows:
- Connection details
- DNS resolution
- TLS handshake
- Headers sent and received
- Timing information

### Check Request Details

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");

Console.WriteLine($"Status: {result.StatusCode}");
Console.WriteLine($"Success: {result.IsSuccess}");
Console.WriteLine($"Content-Type: {result.ContentType}");
Console.WriteLine($"Body Length: {result.Body.Length}");
Console.WriteLine($"Time: {result.TotalTime}");

// Check all headers
foreach (var header in result.Headers)
{
    Console.WriteLine($"{header.Key}: {header.Value}");
}
```

### Test with curl Command Line

Compare CurlDotNet behavior with native curl:

```bash
# Test the same URL with native curl
curl -v https://api.example.com

# Save response to file
curl -v https://api.example.com -o response.txt

# Check just headers
curl -I https://api.example.com
```

If native curl works but CurlDotNet doesn't, please [file an issue](https://github.com/jacob-mellor/curl-dot-net/issues).

### Network Diagnostics

```bash
# Check DNS resolution
nslookup api.example.com

# Check connectivity
ping api.example.com

# Check route
traceroute api.example.com  # macOS/Linux
tracert api.example.com     # Windows

# Check open ports
telnet api.example.com 443

# Check SSL certificate
openssl s_client -connect api.example.com:443
```

### .NET Diagnostics

```bash
# Check .NET version
dotnet --version

# Check installed SDKs
dotnet --list-sdks

# Check installed runtimes
dotnet --list-runtimes

# Restore packages
dotnet restore

# Clean and rebuild
dotnet clean
dotnet build
```

## 📊 Debugging Checklist

When encountering an issue, go through this checklist:

### Basic Checks

- [ ] Is your internet connection working?
- [ ] Can you access the URL in a web browser?
- [ ] Is the URL spelled correctly?
- [ ] Are you using the correct HTTP method?
- [ ] Is the server actually running?

### Authentication

- [ ] Do you need authentication?
- [ ] Is your token/credentials correct?
- [ ] Is your token expired?
- [ ] Are you sending the right authentication headers?

### Request Configuration

- [ ] Are headers formatted correctly?
- [ ] Is your data properly formatted (JSON, form data, etc.)?
- [ ] Are you escaping special characters?
- [ ] Is the timeout sufficient?

### Response Handling

- [ ] Are you checking result.IsSuccess?
- [ ] Are you catching exceptions?
- [ ] Are you parsing the response correctly?
- [ ] Is the response type what you expected?

### Environment

- [ ] Is .NET installed correctly?
- [ ] Is CurlDotNet up to date?
- [ ] Are all dependencies restored?
- [ ] Is your firewall blocking requests?

## 🎓 Best Practices for Troubleshooting

### 1. Read the Error Message

CurlDotNet exceptions include detailed messages and link to documentation:

```csharp
catch (CurlDnsException ex)
{
    // Exception message includes:
    // - What went wrong
    // - Why it happened
    // - How to fix it
    // - Link to detailed documentation
    Console.WriteLine(ex.Message);
}
```

### 2. Use Try-Catch

Always wrap requests in try-catch for better error handling:

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
    // Handle success
}
catch (CurlDnsException ex)
{
    Console.WriteLine($"DNS error: {ex.Message}");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Timeout: {ex.Message}");
}
catch (CurlException ex)
{
    Console.WriteLine($"Curl error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

### 3. Test in Isolation

Simplify your request to identify the problem:

```csharp
// Start simple
var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/200");

// Add complexity gradually
var result = await Curl.ExecuteAsync(@"
    curl https://httpbin.org/get \
      -H 'Accept: application/json'
");

// Then add authentication
var result = await Curl.ExecuteAsync(@"
    curl https://httpbin.org/get \
      -H 'Accept: application/json' \
      -H 'Authorization: Bearer token'
");
```

### 4. Compare with Working Example

Test with a known working endpoint:

```csharp
// These should always work
await Curl.ExecuteAsync("curl https://httpbin.org/status/200");
await Curl.ExecuteAsync("curl https://api.github.com");
await Curl.ExecuteAsync("curl https://jsonplaceholder.typicode.com/posts/1");
```

If these work but your endpoint doesn't, the issue is likely with your specific endpoint or configuration.

### 5. Check the Documentation

Every exception message in CurlDotNet includes a link to detailed documentation. Follow these links for:
- Detailed explanation of the error
- Common causes
- Step-by-step solutions
- Code examples
- Prevention tips

## 🆘 Getting Help

### Before Asking for Help

1. **Search existing issues** - Your problem might be already solved
2. **Check the documentation** - Most issues have dedicated guides
3. **Test with native curl** - Does the command work in terminal?
4. **Simplify your code** - Reduce to minimal reproducible example

### Where to Get Help

#### GitHub Discussions (Recommended for Questions)
- [https://github.com/jacob-mellor/curl-dot-net/discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
- Best for: General questions, how-to questions, discussions

#### GitHub Issues (For Bugs)
- [https://github.com/jacob-mellor/curl-dot-net/issues](https://github.com/jacob-mellor/curl-dot-net/issues)
- Best for: Bug reports, feature requests, actual problems with the library

#### Stack Overflow
- Tag: `curldotnet`
- Best for: Code-specific questions, getting community input

### Creating a Good Issue Report

Include:

1. **Clear title** - "SSL error when connecting to example.com"
2. **CurlDotNet version** - Check with `dotnet list package`
3. **.NET version** - Run `dotnet --version`
4. **Operating system** - Windows, macOS, Linux version
5. **Minimal code example** - Simplest code that reproduces the issue
6. **Error message** - Full exception message and stack trace
7. **Expected behavior** - What should happen
8. **Actual behavior** - What actually happens
9. **Steps to reproduce** - How to recreate the issue

**Example:**

```markdown
## SSL Error Connecting to api.example.com

**Environment:**
- CurlDotNet: 1.0.0
- .NET: 8.0.100
- OS: macOS 14.0

**Code:**
```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");
```

**Error:**
```
CurlSslException: SSL certificate problem: unable to get local issuer certificate
```

**Expected:** Request should succeed
**Actual:** SSL error thrown

**Additional Info:**
- Works in browser
- Works with `curl -k`
- Started happening after OS update
```

## 📚 Additional Resources

- **[Common Issues Guide](common-issues.html)** - Detailed solutions
- **[Installation Guide](../getting-started/installation.html)** - Setup help
- **[API Guide](../api-guide/README.html)** - Complete API reference
- **[Tutorials](../tutorials/README.html)** - Step-by-step learning
- **[Cookbook](../cookbook/README.html)** - Ready-to-use recipes

## 🎯 Quick Reference

### Most Common Solutions

```bash
# Restart terminal after .NET installation
# Close and reopen your terminal/IDE

# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore

# Update CurlDotNet
dotnet add package CurlDotNet

# Build with diagnostics
dotnet build /v:diag

# Run with verbose output
dotnet run --verbosity detailed
```

### Test Connection

```csharp
// Quick connection test
try
{
    var result = await Curl.ExecuteAsync("curl https://httpbin.org/status/200");
    Console.WriteLine(result.IsSuccess ? "✓ Connection OK" : "✗ Connection Failed");
}
catch (Exception ex)
{
    Console.WriteLine($"✗ Error: {ex.Message}");
}
```

---

**Need immediate help?** Start with [Common Issues](common-issues.html) for the most frequent problems and solutions.

**Still stuck?** Ask in [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions) - the community is here to help!
