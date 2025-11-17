# Authentication Errors

## Overview

Authentication errors occur when credentials are missing, invalid, or the authentication method is not supported.

## Common Authentication Methods

### 1. Basic Authentication

```csharp
// Correct usage
var result = await curl.ExecuteAsync(
    "curl -u username:password https://api.example.com"
);

// Handle auth errors
try
{
    var result = await curl.ExecuteAsync("curl https://api.example.com/protected");
}
catch (CurlAuthenticationException ex)
{
    Console.WriteLine($"Auth failed: {ex.Message}");
    // Retry with credentials
    var result = await curl.ExecuteAsync("curl -u user:pass https://api.example.com/protected");
}
```

### 2. Bearer Token Authentication

```csharp
var token = "eyJhbGc...";
var result = await curl.ExecuteAsync(
    $"curl -H \"Authorization: Bearer {token}\" https://api.example.com"
);
```

### 3. API Key Authentication

```csharp
var apiKey = "your-api-key";
var result = await curl.ExecuteAsync(
    $"curl -H \"X-API-Key: {apiKey}\" https://api.example.com"
);
```

## Solutions for Common Issues

### Invalid Credentials
- Verify username/password
- Check for special characters that need escaping
- Ensure credentials haven't expired

### Wrong Authentication Method
- Check API documentation for required auth type
- Try different authentication headers

### Token Expiration
```csharp
public async Task<CurlResult> ExecuteWithTokenRefresh(string url)
{
    try
    {
        return await curl.ExecuteAsync($"curl -H \"Authorization: Bearer {_token}\" {url}");
    }
    catch (CurlAuthenticationException)
    {
        _token = await RefreshToken();
        return await curl.ExecuteAsync($"curl -H \"Authorization: Bearer {_token}\" {url}");
    }
}
```

## Related Documentation
- [HTTP Errors](http-errors.md)
- [Security Best Practices](/guides/security.md)