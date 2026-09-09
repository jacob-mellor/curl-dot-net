# Common Issues and Solutions

This guide covers the most common issues you'll encounter when using CurlDotNet and how to solve them.

## 📑 Table of Contents

- [Installation Issues](#installation-issues)
- [DNS Errors](#dns-errors)
- [Timeout Errors](#timeout-errors)
- [SSL/TLS Errors](#ssl-errors)
- [Authentication Errors](#authentication-errors)
- [HTTP Errors](#http-errors)
- [Connection Errors](#connection-errors)
- [JSON Parsing Errors](#json-parsing-errors)
- [Compilation Errors](#compilation-errors)
- [Runtime Errors](#runtime-errors)

---

## Installation Issues

### dotnet: command not found

**Problem:** After installing .NET, the `dotnet` command isn't recognized.

**Cause:** The .NET SDK path isn't in your system's PATH environment variable, or your terminal needs to be restarted.

**Solution:**

**On Windows:**
```powershell
# 1. Restart your command prompt/PowerShell

# 2. If still not working, add to PATH manually:
# Open System Properties > Environment Variables
# Add to Path: C:\Program Files\dotnet

# 3. Or use PowerShell:
$env:Path += ";C:\Program Files\dotnet"
```

**On macOS/Linux:**
```bash
# 1. Restart your terminal

# 2. If still not working, add to PATH:
export PATH=$PATH:/usr/local/share/dotnet

# 3. Make permanent (add to ~/.bashrc or ~/.zshrc):
echo 'export PATH=$PATH:/usr/local/share/dotnet' >> ~/.bashrc
source ~/.bashrc
```

**Prevention:**
- Always restart your terminal after installing .NET
- Use the official installer from microsoft.com/dotnet

---

### Package 'CurlDotNet' not found

**Problem:** When running `dotnet add package CurlDotNet`, you get "Package 'CurlDotNet' not found".

**Cause:** NuGet sources not configured correctly or network issues.

**Solution:**

```bash
# 1. Check your NuGet sources
dotnet nuget list source

# 2. Add nuget.org if missing
dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org

# 3. Clear cache and retry
dotnet nuget locals all --clear

# 4. Restore packages
dotnet restore

# 5. Try again
dotnet add package CurlDotNet
```

**Alternative Solution:**

Edit your `.csproj` file directly:

```xml
<ItemGroup>
  <PackageReference Include="CurlDotNet" Version="1.0.0" />
</ItemGroup>
```

Then run:
```bash
dotnet restore
```

**Prevention:**
- Ensure stable internet connection
- Keep NuGet sources configured correctly
- Use corporate NuGet servers if behind firewall

---

### Target Framework Issues

**Problem:** "The target framework 'netX.X' is not supported"

**Cause:** Your project targets a .NET version that's not installed or not compatible.

**Solution:**

```bash
# 1. Check installed .NET versions
dotnet --list-sdks

# 2. Edit your .csproj file
# Change TargetFramework to an installed version:

<!-- Option 1: Use .NET 8 (recommended) -->
<TargetFramework>net8.0</TargetFramework>

<!-- Option 2: Use .NET Standard 2.0 (maximum compatibility) -->
<TargetFramework>netstandard2.0</TargetFramework>

<!-- Option 3: Multi-target -->
<TargetFrameworks>net8.0;net6.0;netstandard2.0</TargetFrameworks>
```

**Prevention:**
- Use .NET 8.0 or .NET Standard 2.0 for new projects
- Keep .NET SDK updated

---

## DNS Errors

### Could not resolve host

**Problem:** `CurlDnsException: Could not resolve host: example.com`

**Cause:** DNS lookup failed - the hostname doesn't exist, there's a typo, or DNS server issues.

**Solution:**

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
}
catch (CurlDnsException ex)
{
    Console.WriteLine($"DNS Error: {ex.Message}");
    Console.WriteLine($"Hostname: {ex.Hostname}");

    // Troubleshooting steps:
    // 1. Check spelling of hostname
    // 2. Try in web browser
    // 3. Check DNS with: nslookup api.example.com
    // 4. Try Google DNS: 8.8.8.8
}
```

**Troubleshooting Steps:**

1. **Check the URL spelling:**
   ```csharp
   // Wrong: api.examplee.com (extra 'e')
   // Right: api.example.com
   ```

2. **Test DNS resolution:**
   ```bash
   # Windows
   nslookup api.example.com

   # macOS/Linux
   dig api.example.com
   host api.example.com
   ```

3. **Try ping:**
   ```bash
   ping api.example.com
   ```

4. **Check in browser:**
   - Open the URL in your web browser
   - If it works there but not in CurlDotNet, there might be a proxy or network configuration issue

5. **Try with IP address:**
   ```csharp
   // If DNS is broken, try direct IP (for testing only)
   var result = await Curl.ExecuteAsync("curl http://93.184.216.34");
   ```

**Prevention:**
- Double-check hostnames before making requests
- Use constants or configuration for frequently used URLs
- Implement retry logic for transient DNS failures

---

## Timeout Errors

### Operation timeout

**Problem:** `CurlTimeoutException: Operation timeout. The specified time has elapsed.`

**Cause:** The server took too long to respond, or network is slow/unstable.

**Solution:**

```csharp
try
{
    // Increase timeout (default is 30 seconds)
    var result = await Curl.ExecuteAsync(@"
        curl --connect-timeout 30 \
             --max-time 120 \
             https://slow-api.example.com
    ");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Timeout after {ex.Timeout} seconds");

    // Options:
    // 1. Increase timeout
    // 2. Retry the request
    // 3. Check if server is slow/overloaded
    // 4. Try different endpoint
}
```

**Different Timeout Types:**

```csharp
// Connection timeout - how long to wait for connection
var result = await Curl.ExecuteAsync(@"
    curl --connect-timeout 10 https://api.example.com
");

// Max time - total operation timeout
var result = await Curl.ExecuteAsync(@"
    curl --max-time 60 https://api.example.com
");

// Both together
var result = await Curl.ExecuteAsync(@"
    curl --connect-timeout 10 \
         --max-time 60 \
         https://api.example.com
");
```

**Implementing Retry Logic:**

```csharp
async Task<CurlResult> RequestWithRetry(string url, int maxRetries = 3)
{
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            return await Curl.ExecuteAsync($"curl --max-time 30 {url}");
        }
        catch (CurlTimeoutException)
        {
            if (attempt == maxRetries)
                throw;

            // Exponential backoff
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
            Console.WriteLine($"Retry {attempt}/{maxRetries}...");
        }
    }
    throw new Exception("Should not reach here");
}
```

**Prevention:**
- Set reasonable timeouts based on API SLA
- Implement retry logic for production code
- Monitor API response times
- Use cancellation tokens for user-initiated cancellations

---

## SSL Errors

### SSL certificate problem

**Problem:** `CurlSslException: SSL certificate problem: unable to get local issuer certificate`

**Cause:** SSL/TLS certificate verification failed - self-signed cert, expired cert, or system certificate store issues.

**Solution for Development (NOT for production):**

```csharp
// WARNING: Only use -k flag in development/testing!
var result = await Curl.ExecuteAsync("curl -k https://self-signed.example.com");
```

**Proper Solutions for Production:**

**1. Update System Certificates:**

**Windows:**
```powershell
certutil -generateSSTFromWU roots.sst
```

**macOS:**
```bash
# Update certificates
security find-certificate -a -p /System/Library/Keychains/SystemRootCertificates.keychain > /dev/null

# Or install specific certificate
sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain mycert.crt
```

**Linux (Ubuntu/Debian):**
```bash
# Update CA certificates
sudo apt-get update
sudo apt-get install --reinstall ca-certificates

# Or add specific certificate
sudo cp mycert.crt /usr/local/share/ca-certificates/
sudo update-ca-certificates
```

**2. Use Correct Certificate Bundle:**

```csharp
var result = await Curl.ExecuteAsync(@"
    curl --cacert /path/to/certificate.pem \
         https://api.example.com
");
```

**3. Specify Client Certificate:**

```csharp
var result = await Curl.ExecuteAsync(@"
    curl --cert client-cert.pem \
         --key client-key.pem \
         https://api.example.com
");
```

**Understanding SSL Error Codes:**

| Error | Meaning | Solution |
|-------|---------|----------|
| 35 | SSL connect error | Check SSL/TLS version compatibility |
| 51 | Peer certificate invalid | Update certificates or check hostname |
| 53 | Crypto engine not found | Install required crypto libraries |
| 54 | Set TLS/SSL version failed | Server doesn't support TLS version |
| 58 | Problem with local certificate | Check client certificate path/permissions |
| 59 | Could not load certificates | Update system certificate store |
| 60 | Peer certificate cannot be authenticated | Most common - see solutions above |
| 77 | Problem with CA cert bundle | Specify correct CA bundle path |

**Debugging SSL Issues:**

```bash
# Test SSL connection
openssl s_client -connect api.example.com:443

# Check certificate expiration
echo | openssl s_client -connect api.example.com:443 2>/dev/null | openssl x509 -noout -dates

# View certificate details
echo | openssl s_client -connect api.example.com:443 2>/dev/null | openssl x509 -noout -text
```

**Prevention:**
- Never use `-k` in production
- Keep system certificates updated
- Monitor certificate expiration
- Use Let's Encrypt for free valid certificates
- Test SSL configuration regularly

---

## Authentication Errors

### 401 Unauthorized

**Problem:** HTTP 401 - Authentication required or credentials invalid.

**Cause:** Missing authentication, invalid credentials, or expired tokens.

**Solution:**

**Basic Authentication:**

```csharp
// Method 1: Using -u flag
var result = await Curl.ExecuteAsync(@"
    curl -u username:password https://api.example.com
");

// Method 2: Using Authorization header
var credentials = Convert.ToBase64String(
    System.Text.Encoding.ASCII.GetBytes("username:password")
);
var result = await Curl.ExecuteAsync($@"
    curl -H 'Authorization: Basic {credentials}' \
         https://api.example.com
");
```

**Bearer Token Authentication:**

```csharp
// Get token from environment variable (recommended)
string token = Environment.GetEnvironmentVariable("API_TOKEN");

var result = await Curl.ExecuteAsync($@"
    curl -H 'Authorization: Bearer {token}' \
         https://api.example.com
");
```

**API Key Authentication:**

```csharp
string apiKey = Environment.GetEnvironmentVariable("API_KEY");

var result = await Curl.ExecuteAsync($@"
    curl -H 'X-API-Key: {apiKey}' \
         https://api.example.com
");
```

**OAuth 2.0 Flow:**

```csharp
// 1. Get access token
var tokenResponse = await Curl.ExecuteAsync($@"
    curl -X POST https://oauth.example.com/token \
      -d 'grant_type=client_credentials' \
      -d 'client_id={clientId}' \
      -d 'client_secret={clientSecret}'
");

dynamic tokenData = tokenResponse.AsJsonDynamic();
string accessToken = tokenData.access_token;

// 2. Use access token
var result = await Curl.ExecuteAsync($@"
    curl -H 'Authorization: Bearer {accessToken}' \
         https://api.example.com/protected
");
```

**Troubleshooting Authentication:**

```csharp
try
{
    var result = await Curl.ExecuteAsync($@"
        curl -H 'Authorization: Bearer {token}' \
             https://api.example.com
    ");
}
catch (CurlHttpReturnedErrorException ex) when (ex.StatusCode == 401)
{
    Console.WriteLine("Authentication failed!");
    Console.WriteLine("Possible causes:");
    Console.WriteLine("1. Token is invalid");
    Console.WriteLine("2. Token is expired");
    Console.WriteLine("3. Wrong authentication method");
    Console.WriteLine("4. Missing required headers");

    // Check response for hints
    Console.WriteLine($"Response: {ex.ResponseBody}");
}
```

**Prevention:**
- Store credentials in environment variables, not code
- Implement token refresh logic
- Handle 401 errors gracefully
- Monitor token expiration
- Use secrets management (Azure Key Vault, AWS Secrets Manager, etc.)

---

### 403 Forbidden

**Problem:** HTTP 403 - Server understood request but refuses to authorize it.

**Cause:** Insufficient permissions, IP blocking, or rate limiting.

**Solution:**

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com/admin");
}
catch (CurlHttpReturnedErrorException ex) when (ex.StatusCode == 403)
{
    Console.WriteLine("Access forbidden!");

    // Common causes:
    // 1. Insufficient permissions
    // 2. IP address blocked
    // 3. Rate limiting
    // 4. Geographic restrictions
    // 5. Resource requires different authentication

    Console.WriteLine($"Response: {ex.ResponseBody}");
}
```

**Checking Permissions:**

```csharp
// Check if you have the right scope/permissions
var result = await Curl.ExecuteAsync($@"
    curl -H 'Authorization: Bearer {token}' \
         -H 'X-Requested-Scope: admin' \
         https://api.example.com/admin
");
```

**Prevention:**
- Verify user permissions before making requests
- Implement proper rate limiting
- Handle 403 errors with user-friendly messages
- Check API documentation for required permissions

---

## HTTP Errors

### 404 Not Found

**Problem:** HTTP 404 - The requested resource doesn't exist.

**Cause:** Wrong URL, resource deleted, or incorrect endpoint.

**Solution:**

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com/users/99999");

    if (result.StatusCode == 404)
    {
        Console.WriteLine("Resource not found");
        // Handle gracefully - maybe show user-friendly message
    }
}
catch (CurlHttpReturnedErrorException ex) when (ex.StatusCode == 404)
{
    Console.WriteLine($"Not found: {ex.Message}");
}
```

**Troubleshooting:**

1. **Check the URL:**
   ```csharp
   // Wrong: /api/v1/user/123 (singular)
   // Right: /api/v1/users/123 (plural)
   ```

2. **Verify endpoint exists:**
   ```bash
   # Check API documentation
   # Try in browser or Postman first
   ```

3. **Check if resource was deleted:**
   ```csharp
   // Try listing all resources first
   var list = await Curl.ExecuteAsync("curl https://api.example.com/users");
   ```

**Prevention:**
- Use constants for API endpoints
- Validate resource IDs before making requests
- Implement proper error handling for 404s
- Show user-friendly "not found" messages

---

### 429 Too Many Requests

**Problem:** HTTP 429 - Rate limit exceeded.

**Cause:** Making too many requests too quickly.

**Solution:**

```csharp
async Task<CurlResult> RequestWithRateLimiting(string url)
{
    try
    {
        var result = await Curl.ExecuteAsync($"curl {url}");
        return result;
    }
    catch (CurlHttpReturnedErrorException ex) when (ex.StatusCode == 429)
    {
        // Check Retry-After header
        if (result.Headers.TryGetValue("Retry-After", out string retryAfter))
        {
            int seconds = int.Parse(retryAfter);
            Console.WriteLine($"Rate limited. Waiting {seconds} seconds...");
            await Task.Delay(TimeSpan.FromSeconds(seconds));

            // Retry
            return await Curl.ExecuteAsync($"curl {url}");
        }
        throw;
    }
}
```

**Implementing Rate Limiting:**

```csharp
using System.Threading;

class RateLimitedClient
{
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(10); // 10 concurrent requests
    private readonly TimeSpan _minInterval = TimeSpan.FromMilliseconds(100);
    private DateTime _lastRequest = DateTime.MinValue;

    public async Task<CurlResult> ExecuteAsync(string url)
    {
        await _semaphore.WaitAsync();
        try
        {
            // Ensure minimum interval between requests
            var timeSinceLastRequest = DateTime.UtcNow - _lastRequest;
            if (timeSinceLastRequest < _minInterval)
            {
                await Task.Delay(_minInterval - timeSinceLastRequest);
            }

            _lastRequest = DateTime.UtcNow;
            return await Curl.ExecuteAsync($"curl {url}");
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
```

**Prevention:**
- Implement rate limiting in your application
- Cache responses when possible
- Use batch endpoints when available
- Monitor rate limit headers
- Implement exponential backoff

---

### 500 Internal Server Error

**Problem:** HTTP 500 - Server encountered an error.

**Cause:** Server-side bug, database issue, or temporary problem.

**Solution:**

```csharp
async Task<CurlResult> RequestWithRetry(string url, int maxRetries = 3)
{
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            var result = await Curl.ExecuteAsync($"curl {url}");

            if (result.StatusCode >= 500 && result.StatusCode < 600)
            {
                if (attempt < maxRetries)
                {
                    Console.WriteLine($"Server error {result.StatusCode}. Retry {attempt}/{maxRetries}...");
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
                    continue;
                }
            }

            return result;
        }
        catch (CurlHttpReturnedErrorException ex) when (ex.StatusCode >= 500)
        {
            if (attempt == maxRetries)
                throw;

            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
        }
    }
    throw new Exception("Max retries exceeded");
}
```

**What to do:**
1. Retry the request (server might be temporarily down)
2. Check if it's a known outage (check status page)
3. Contact API support if persistent
4. Implement fallback logic
5. Log the error for debugging

**Prevention:**
- Implement retry logic for 5xx errors
- Use circuit breaker pattern
- Have fallback/cached data
- Monitor API status pages
- Set up alerting for persistent failures

---

## Connection Errors

### Connection refused

**Problem:** `CurlException: Failed to connect to host: Connection refused`

**Cause:** Server not running, firewall blocking, or wrong port.

**Solution:**

```bash
# 1. Check if server is running
curl https://api.example.com

# 2. Check if port is open
telnet api.example.com 443

# 3. Try with different port
curl https://api.example.com:8080

# 4. Check firewall rules
# Windows: Check Windows Firewall
# macOS: System Preferences > Security > Firewall
# Linux: sudo iptables -L
```

**In Code:**

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
}
catch (CurlException ex) when (ex.Message.Contains("Connection refused"))
{
    Console.WriteLine("Cannot connect to server");
    Console.WriteLine("Possible causes:");
    Console.WriteLine("1. Server is down");
    Console.WriteLine("2. Firewall is blocking");
    Console.WriteLine("3. Wrong URL or port");
    Console.WriteLine("4. Network connectivity issues");
}
```

**Prevention:**
- Verify server is running before deploying
- Test connectivity from deployment environment
- Document required ports
- Set up health check endpoints

---

## JSON Parsing Errors

### Invalid JSON

**Problem:** Exception when parsing JSON response.

**Cause:** Response isn't valid JSON, empty response, or wrong Content-Type.

**Solution:**

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");

    // Check Content-Type first
    if (!result.ContentType.Contains("application/json"))
    {
        Console.WriteLine($"Warning: Response is {result.ContentType}, not JSON");
        Console.WriteLine($"Body: {result.Body}");
        return;
    }

    // Check if response is empty
    if (string.IsNullOrWhiteSpace(result.Body))
    {
        Console.WriteLine("Empty response");
        return;
    }

    // Try parsing
    var data = result.ParseJson<MyType>();
}
catch (System.Text.Json.JsonException ex)
{
    Console.WriteLine($"Invalid JSON: {ex.Message}");
    Console.WriteLine($"Response was: {result.Body}");

    // Maybe it's HTML error page?
    if (result.Body.StartsWith("<"))
    {
        Console.WriteLine("Server returned HTML instead of JSON");
    }
}
```

**Validating JSON Before Parsing:**

```csharp
bool IsValidJson(string json)
{
    try
    {
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        return true;
    }
    catch
    {
        return false;
    }
}

var result = await Curl.ExecuteAsync("curl https://api.example.com");
if (IsValidJson(result.Body))
{
    var data = result.ParseJson<MyType>();
}
else
{
    Console.WriteLine("Invalid JSON response");
}
```

**Handling Different Response Types:**

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");

if (result.ContentType.Contains("application/json"))
{
    var data = result.ParseJson<MyType>();
}
else if (result.ContentType.Contains("text/html"))
{
    Console.WriteLine("Received HTML:");
    Console.WriteLine(result.Body);
}
else if (result.ContentType.Contains("text/plain"))
{
    Console.WriteLine("Plain text response:");
    Console.WriteLine(result.Body);
}
else
{
    Console.WriteLine($"Unknown content type: {result.ContentType}");
}
```

**Prevention:**
- Always check Content-Type header
- Validate JSON before parsing
- Handle empty responses
- Use try-catch around JSON parsing
- Have fallback for non-JSON responses

---

## Compilation Errors

### Cannot implicitly convert type 'Task<CurlResult>' to 'CurlResult'

**Problem:** Compiler error about Task conversion.

**Cause:** Forgot to use `await` keyword.

**Solution:**

```csharp
// Wrong - missing await
var result = Curl.ExecuteAsync("curl https://api.example.com");

// Right - with await
var result = await Curl.ExecuteAsync("curl https://api.example.com");
```

**Make sure your method is async:**

```csharp
// Wrong - method not async
void MyMethod()
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
}

// Right - method is async
async Task MyMethod()
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
}
```

---

### 'Curl' does not exist in the current context

**Problem:** Compiler can't find Curl class.

**Cause:** Missing `using CurlDotNet;` statement.

**Solution:**

```csharp
// Add this at the top of your file
using CurlDotNet;

class Program
{
    static async Task Main()
    {
        var result = await Curl.ExecuteAsync("curl https://api.example.com");
    }
}
```

---

## Runtime Errors

### NullReferenceException

**Problem:** Null reference exception when accessing result properties.

**Cause:** Not checking if result is null or if properties exist.

**Solution:**

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");

// Always check IsSuccess first
if (result.IsSuccess)
{
    // Safe to use result.Body
    Console.WriteLine(result.Body);
}
else
{
    Console.WriteLine($"Request failed: {result.StatusCode}");
}

// Check headers before accessing
if (result.Headers.ContainsKey("Content-Type"))
{
    var contentType = result.Headers["Content-Type"];
}

// Or use TryGetValue
if (result.Headers.TryGetValue("X-Custom-Header", out string customHeader))
{
    Console.WriteLine($"Custom header: {customHeader}");
}
```

**Prevention:**
- Always check `result.IsSuccess`
- Use null-conditional operators: `result?.Body`
- Check dictionary keys before accessing
- Use TryGetValue for dictionaries

---

## 🎓 Best Practices for Error Handling

### 1. Use Specific Exception Types

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
}
catch (CurlDnsException ex)
{
    // Handle DNS errors specifically
}
catch (CurlTimeoutException ex)
{
    // Handle timeout errors specifically
}
catch (CurlSslException ex)
{
    // Handle SSL errors specifically
}
catch (CurlHttpReturnedErrorException ex)
{
    // Handle HTTP errors specifically
}
catch (CurlException ex)
{
    // Handle all other curl errors
}
catch (Exception ex)
{
    // Handle unexpected errors
}
```

### 2. Check Result Status

```csharp
var result = await Curl.ExecuteAsync("curl https://api.example.com");

if (!result.IsSuccess)
{
    Console.WriteLine($"Request failed with status {result.StatusCode}");
    Console.WriteLine($"Response: {result.Body}");
    return;
}

// Proceed with successful result
var data = result.ParseJson<MyType>();
```

### 3. Implement Retry Logic

```csharp
async Task<CurlResult> ExecuteWithRetry(string url, int maxRetries = 3)
{
    Exception lastException = null;

    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return await Curl.ExecuteAsync($"curl {url}");
        }
        catch (CurlTimeoutException ex)
        {
            lastException = ex;
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
        }
        catch (CurlException ex) when (ex.ErrorCode == 7) // Connection failed
        {
            lastException = ex;
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
        }
    }

    throw lastException;
}
```

### 4. Log All Errors

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
}
catch (CurlException ex)
{
    // Log with context
    logger.LogError(ex, "Curl request failed. URL: {Url}, ErrorCode: {ErrorCode}",
        "https://api.example.com", ex.ErrorCode);
    throw;
}
```

---

## 🆘 Still Need Help?

If this guide doesn't solve your issue:

1. **Check if it's a known issue:** [GitHub Issues](https://github.com/jacob-mellor/curl-dot-net/issues)
2. **Ask the community:** [GitHub Discussions](https://github.com/jacob-mellor/curl-dot-net/discussions)
3. **Report a bug:** [Create new issue](https://github.com/jacob-mellor/curl-dot-net/issues/new)

When reporting issues, include:
- CurlDotNet version
- .NET version
- Operating system
- Minimal code to reproduce
- Full error message
- What you expected vs what happened

---

**Back to:** [Troubleshooting Guide](README.html) | [Documentation Home](../)
