# Cookie Handling Errors

## Overview

Cookie-related errors occur when CurlDotNet has problems managing HTTP cookies, including reading from or writing to cookie jar files, parsing cookie data, or handling cookie headers.

## Common Error Scenarios

### 1. Cookie Jar File Access Issues

**Problem:** Cannot read or write to the specified cookie jar file.

**Common Causes:**
- File permissions are too restrictive
- Directory doesn't exist
- Disk is full
- Invalid file path

**Solutions:**

```csharp
// Ensure directory exists
var cookieJarPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "myapp", "cookies.txt");
Directory.CreateDirectory(Path.GetDirectoryName(cookieJarPath));

// Use the cookie jar with proper error handling
try
{
    var result = await curl.ExecuteAsync($"curl --cookie-jar {cookieJarPath} --cookie {cookieJarPath} https://example.com");
}
catch (CurlCookieException ex)
{
    // Check if it's a file access issue
    if (!string.IsNullOrEmpty(ex.CookieJarPath))
    {
        Console.WriteLine($"Cookie jar issue with: {ex.CookieJarPath}");

        // Try alternative location
        var tempCookieJar = Path.GetTempFileName();
        result = await curl.ExecuteAsync($"curl --cookie-jar {tempCookieJar} https://example.com");
    }
}
```

### 2. Malformed Cookie Data

**Problem:** Cookie data in the jar file or from the server is malformed.

**Common Causes:**
- Corrupted cookie jar file
- Invalid cookie format from server
- Manual editing of cookie jar file

**Solutions:**

```csharp
// Clear and recreate cookie jar if corrupted
if (File.Exists(cookieJarPath))
{
    File.Delete(cookieJarPath);
}

// Start fresh with new cookies
var result = await curl.ExecuteAsync($"curl --cookie-jar {cookieJarPath} https://example.com");
```

### 3. Cookie Security Issues

**Problem:** Cookies are rejected due to security policies.

**Common Causes:**
- Secure cookie on non-HTTPS connection
- HttpOnly cookie access attempt from JavaScript
- SameSite policy violations

**Solutions:**

```csharp
// Ensure HTTPS for secure cookies
var url = "https://secure.example.com";  // Use HTTPS, not HTTP

// Handle cookie policies
var result = await curl.ExecuteAsync($"curl --cookie-jar {cookieJarPath} {url}");
```

## Best Practices

### 1. Cookie Jar Management

```csharp
public class CookieManager
{
    private readonly string _cookieJarPath;

    public CookieManager(string appName)
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var cookieDir = Path.Combine(appData, appName);
        Directory.CreateDirectory(cookieDir);
        _cookieJarPath = Path.Combine(cookieDir, "cookies.txt");
    }

    public async Task<CurlResult> ExecuteWithCookies(string url)
    {
        var curl = new Curl();
        return await curl.ExecuteAsync($"curl --cookie-jar {_cookieJarPath} --cookie {_cookieJarPath} {url}");
    }

    public void ClearCookies()
    {
        if (File.Exists(_cookieJarPath))
        {
            File.Delete(_cookieJarPath);
        }
    }
}
```

### 2. Session Management

```csharp
// Maintain session across requests
public class SessionManager
{
    private readonly string _sessionCookieJar;

    public SessionManager()
    {
        _sessionCookieJar = Path.GetTempFileName();
    }

    public async Task<CurlResult> Login(string username, string password)
    {
        var curl = new Curl();
        var loginData = $"username={Uri.EscapeDataString(username)}&password={Uri.EscapeDataString(password)}";

        return await curl.ExecuteAsync(
            $"curl --cookie-jar {_sessionCookieJar} " +
            $"--data \"{loginData}\" " +
            $"https://example.com/login"
        );
    }

    public async Task<CurlResult> AuthenticatedRequest(string url)
    {
        var curl = new Curl();
        return await curl.ExecuteAsync(
            $"curl --cookie {_sessionCookieJar} {url}"
        );
    }

    public void Logout()
    {
        if (File.Exists(_sessionCookieJar))
        {
            File.Delete(_sessionCookieJar);
        }
    }
}
```

## Debugging Tips

### 1. Inspect Cookie Jar Contents

```csharp
// Read and display cookie jar contents for debugging
if (File.Exists(cookieJarPath))
{
    var cookies = File.ReadAllLines(cookieJarPath);
    Console.WriteLine("Current cookies:");
    foreach (var cookie in cookies)
    {
        if (!cookie.StartsWith("#"))
        {
            Console.WriteLine(cookie);
        }
    }
}
```

### 2. Enable Verbose Mode

```csharp
// Use verbose mode to see cookie handling
var result = await curl.ExecuteAsync($"curl -v --cookie-jar {cookieJarPath} https://example.com");
Console.WriteLine(result.StdErr); // Contains verbose output including cookie operations
```

### 3. Test Cookie Handling

```csharp
// Test basic cookie functionality
public async Task<bool> TestCookieHandling()
{
    var testCookieJar = Path.GetTempFileName();
    try
    {
        var curl = new Curl();

        // Test writing cookies
        var writeResult = await curl.ExecuteAsync(
            $"curl --cookie-jar {testCookieJar} https://httpbin.org/cookies/set?test=value"
        );

        // Test reading cookies
        var readResult = await curl.ExecuteAsync(
            $"curl --cookie {testCookieJar} https://httpbin.org/cookies"
        );

        // Verify cookie was preserved
        return readResult.StdOut.Contains("test") && readResult.StdOut.Contains("value");
    }
    finally
    {
        if (File.Exists(testCookieJar))
        {
            File.Delete(testCookieJar);
        }
    }
}
```

## Common Cookie Jar Format

The cookie jar file follows the Netscape cookie format:

```
# Netscape HTTP Cookie File
# This is a generated file! Do not edit.

.example.com	TRUE	/	FALSE	1234567890	sessionid	abc123def456
.example.com	TRUE	/	TRUE	1234567890	secure_token	xyz789
```

Format: `domain flag path secure expiry name value`

## Related Documentation

- [HTTP Error Handling](http-errors.md)
- [Authentication Errors](auth-errors.md)
- [CurlDotNet Cookie API Reference](/api/CurlDotNet.Exceptions/CurlCookieException.html)

## Error Prevention Checklist

- [ ] Ensure cookie jar directory exists before use
- [ ] Check file permissions on cookie jar
- [ ] Use absolute paths for cookie jar files
- [ ] Clean up temporary cookie jars after use
- [ ] Handle cookie jar corruption gracefully
- [ ] Use HTTPS for secure cookies
- [ ] Implement proper session management
- [ ] Clear cookies on logout
- [ ] Validate cookie data format
- [ ] Test cookie persistence across requests