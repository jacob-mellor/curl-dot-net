# Troubleshooting Guide

Welcome to the CurlDotNet troubleshooting guide. This section helps you resolve common issues and errors when using CurlDotNet.

## Quick Solutions

### Common Issues

- [Connection Problems](#connection-problems)
- [Authentication Errors](#authentication-errors)
- [SSL/TLS Issues](#ssltls-issues)
- [Timeout Problems](#timeout-problems)
- [Parsing Errors](#parsing-errors)

## Connection Problems

### Issue: "Could not connect to server"

**Symptoms:**
- `CurlCouldntConnectException` is thrown
- Connection timeouts
- Network unreachable errors

**Solutions:**

1. **Check the URL is correct**
   ```csharp
   // Make sure the URL is properly formatted
   var result = await Curl.ExecuteAsync("curl https://api.example.com");
   ```

2. **Verify network connectivity**
   ```csharp
   // Test with a known working endpoint
   var test = await Curl.ExecuteAsync("curl https://www.google.com");
   ```

3. **Check firewall/proxy settings**
   ```csharp
   // Use proxy if required
   var result = await Curl.ExecuteAsync("curl -x proxy.company.com:8080 https://api.example.com");
   ```

### Issue: "DNS resolution failed"

**Symptoms:**
- `CurlCouldntResolveHostException` is thrown
- Host not found errors

**Solutions:**

1. **Verify the hostname**
   ```csharp
   // Check for typos in the domain name
   var result = await Curl.ExecuteAsync("curl https://api.github.com"); // Correct
   // NOT: "curl https://api.githbu.com" // Typo
   ```

2. **Use IP address directly (temporary)**
   ```csharp
   // Use IP if DNS is temporarily down
   var result = await Curl.ExecuteAsync("curl https://140.82.114.5");
   ```

## Authentication Errors

### Issue: "401 Unauthorized"

**Symptoms:**
- `CurlAuthenticationException` is thrown
- 401 HTTP status code
- "Unauthorized" response

**Solutions:**

1. **Check API key/token**
   ```csharp
   // Bearer token
   var result = await Curl.ExecuteAsync(@"
       curl https://api.example.com \
       -H 'Authorization: Bearer YOUR_TOKEN_HERE'
   ");
   ```

2. **Verify credentials format**
   ```csharp
   // Basic auth
   var result = await Curl.ExecuteAsync("curl -u username:password https://api.example.com");
   ```

## SSL/TLS Issues

### Issue: "SSL certificate problem"

**Symptoms:**
- `CurlSslCertificateProblemException` is thrown
- Certificate verification failed
- Peer certificate cannot be authenticated

**Solutions:**

1. **For development only - skip verification**
   ```csharp
   // WARNING: Only use in development!
   var result = await Curl.ExecuteAsync("curl -k https://localhost:5001");
   ```

2. **Specify CA certificate**
   ```csharp
   // Use custom CA certificate
   var result = await Curl.ExecuteAsync("curl --cacert /path/to/cert.pem https://api.example.com");
   ```

## Timeout Problems

### Issue: "Operation timed out"

**Symptoms:**
- `CurlOperationTimeoutException` is thrown
- Request takes too long
- No response received

**Solutions:**

1. **Increase timeout**
   ```csharp
   // Set 30 second timeout
   var result = await Curl.ExecuteAsync("curl --max-time 30 https://slow-api.example.com");
   ```

2. **Use connection timeout**
   ```csharp
   // Set connection timeout separately
   var result = await Curl.ExecuteAsync("curl --connect-timeout 10 https://api.example.com");
   ```

## Parsing Errors

### Issue: "Invalid curl command"

**Symptoms:**
- `CurlParsingException` is thrown
- Command not recognized
- Syntax errors

**Solutions:**

1. **Check command syntax**
   ```csharp
   // Correct: quotes around URL with spaces
   var result = await Curl.ExecuteAsync("curl \"https://api.example.com/path with spaces\"");
   ```

2. **Escape special characters**
   ```csharp
   // Escape quotes in JSON
   var result = await Curl.ExecuteAsync(@"
       curl -X POST https://api.example.com \
       -d '{""name"": ""value""}'
   ");
   ```

## Debug Mode

Enable verbose output to see detailed information:

```csharp
// Enable verbose mode for debugging
var result = await Curl.ExecuteAsync("curl -v https://api.example.com");

// Check the verbose output
Console.WriteLine(result.VerboseOutput);
```

## Getting More Help

### Still Having Issues?

1. **Check the [Common Issues](common-issues.md) page**
2. **Search [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)**
3. **Review the [API Documentation](/api/)**
4. **Look at [Examples](/cookbook/)**

### Report a Bug

If you've found a bug:

1. Search existing issues first
2. Create a [new issue](https://github.com/jacob-mellor/curl-dot-net/issues/new)
3. Include:
   - CurlDotNet version
   - .NET version
   - Minimal code to reproduce
   - Error message/stack trace

## Related Pages

- [Common Issues](common-issues.md)
- [Connection Errors](connection-errors.md)
- [SSL/TLS Guide](/guides/security.md)
- [Network Troubleshooting](/guides/network-troubleshooting.md)
- [Exception Reference](/exceptions/)

---
*Last updated: November 2024*