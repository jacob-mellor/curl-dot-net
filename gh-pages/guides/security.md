# Security Best Practices

A comprehensive guide to secure HTTP communication with CurlDotNet.

## 🔒 SSL/TLS Configuration

### Certificate Validation

**Always validate certificates in production:**

```csharp
// ✅ GOOD: Default behavior validates certificates
var result = await Curl.ExecuteAsync("curl https://api.example.com");

// ❌ BAD: Never do this in production
var result = await Curl.ExecuteAsync("curl -k https://api.example.com");
```

### Custom Certificate Authority

For internal services with custom CAs:

```csharp
// Specify custom CA certificate
var result = await Curl.ExecuteAsync(@"
    curl --cacert /path/to/company-ca.pem \
         https://internal-api.company.com
");

// Multiple CA certificates
var result = await Curl.ExecuteAsync(@"
    curl --capath /etc/ssl/certs \
         https://api.example.com
");
```

### Client Certificates

For mutual TLS authentication:

```csharp
var result = await Curl.ExecuteAsync(@"
    curl --cert /path/to/client-cert.pem \
         --key /path/to/client-key.pem \
         https://secure-api.example.com
");

// With password-protected key
var result = await Curl.ExecuteAsync(@"
    curl --cert /path/to/client-cert.pem \
         --key /path/to/client-key.pem \
         --pass 'keypassword' \
         https://secure-api.example.com
");
```

### TLS Version Control

Force specific TLS versions for compliance:

```csharp
// Require TLS 1.2 minimum
var result = await Curl.ExecuteAsync("curl --tlsv1.2 https://api.example.com");

// Require TLS 1.3 only
var result = await Curl.ExecuteAsync("curl --tlsv1.3 https://api.example.com");

// Specify allowed versions
var result = await Curl.ExecuteAsync(@"
    curl --tls-max 1.3 \
         --tlsv1.2 \
         https://api.example.com
");
```

## 🔑 Authentication Security

### Secure Credential Storage

**Never hardcode credentials:**

```csharp
// ❌ BAD: Hardcoded credentials
var result = await Curl.ExecuteAsync("curl -u admin:password123 https://api.example.com");

// ✅ GOOD: Use environment variables
var username = Environment.GetEnvironmentVariable("API_USERNAME");
var password = Environment.GetEnvironmentVariable("API_PASSWORD");
var result = await Curl.ExecuteAsync($"curl -u {username}:{password} https://api.example.com");

// ✅ BETTER: Use secure credential storage
var token = await SecureCredentialStore.GetTokenAsync("api-token");
var result = await Curl.ExecuteAsync($@"
    curl -H 'Authorization: Bearer {token}' https://api.example.com
");
```

### Token Management

Implement secure token handling:

```csharp
public class SecureApiClient
{
    private string _token;
    private DateTime _tokenExpiry;

    private async Task<string> GetValidTokenAsync()
    {
        if (_token == null || DateTime.UtcNow >= _tokenExpiry)
        {
            // Refresh token
            var result = await Curl.ExecuteAsync(@"
                curl -X POST https://auth.example.com/token \
                     -d 'grant_type=client_credentials' \
                     -d 'client_id=' + Environment.GetEnvironmentVariable("CLIENT_ID") + \
                     -d 'client_secret=' + Environment.GetEnvironmentVariable("CLIENT_SECRET")
            ");

            var tokenResponse = result.ParseJson<TokenResponse>();
            _token = tokenResponse.AccessToken;
            _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60);
        }

        return _token;
    }

    public async Task<CurlResult> MakeSecureRequestAsync(string endpoint)
    {
        var token = await GetValidTokenAsync();
        return await Curl.ExecuteAsync($@"
            curl -H 'Authorization: Bearer {token}' https://api.example.com/{endpoint}
        ");
    }
}
```

## 🛡️ Input Validation

### Prevent Command Injection

Always validate and sanitize user input:

```csharp
// ❌ DANGEROUS: Direct user input
public async Task<CurlResult> DangerousMethod(string userInput)
{
    return await Curl.ExecuteAsync($"curl https://api.example.com/{userInput}");
}

// ✅ SAFE: Validated input
public async Task<CurlResult> SafeMethod(string userInput)
{
    // Validate input
    if (string.IsNullOrWhiteSpace(userInput))
        throw new ArgumentException("Input cannot be empty");

    // Remove dangerous characters
    var sanitized = Regex.Replace(userInput, @"[^\w\-\.]", "");

    // Use parameterized approach
    var builder = new CurlRequestBuilder()
        .WithUrl($"https://api.example.com/{Uri.EscapeDataString(sanitized)}")
        .WithMethod("GET");

    return await builder.ExecuteAsync();
}
```

### URL Validation

```csharp
public static bool IsValidUrl(string url)
{
    if (string.IsNullOrWhiteSpace(url))
        return false;

    // Check URL format
    if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
        return false;

    // Only allow HTTPS in production
    if (uri.Scheme != Uri.UriSchemeHttps)
        return false;

    // Prevent SSRF attacks
    var blacklistedHosts = new[] { "localhost", "127.0.0.1", "0.0.0.0", "169.254.169.254" };
    if (blacklistedHosts.Contains(uri.Host))
        return false;

    return true;
}
```

## 🔐 Secure Headers

### Security Headers to Include

```csharp
var result = await new CurlRequestBuilder()
    .WithUrl("https://api.example.com")
    .WithHeader("X-Content-Type-Options", "nosniff")
    .WithHeader("X-Frame-Options", "DENY")
    .WithHeader("X-XSS-Protection", "1; mode=block")
    .WithHeader("Strict-Transport-Security", "max-age=31536000; includeSubDomains")
    .WithHeader("Content-Security-Policy", "default-src 'self'")
    .ExecuteAsync();
```

### Sensitive Data in Headers

```csharp
// Never log sensitive headers
public static void LogRequest(CurlResult result, bool includeSensitive = false)
{
    var sensitiveHeaders = new[] { "Authorization", "X-API-Key", "Cookie" };

    foreach (var header in result.Headers)
    {
        if (!includeSensitive && sensitiveHeaders.Contains(header.Key))
        {
            Console.WriteLine($"{header.Key}: [REDACTED]");
        }
        else
        {
            Console.WriteLine($"{header.Key}: {header.Value}");
        }
    }
}
```

## 🚨 Error Handling

### Don't Expose Internal Details

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com");
}
catch (CurlException ex)
{
    // ❌ BAD: Exposing internal details
    return new { error = ex.ToString() };

    // ✅ GOOD: Generic error message
    Logger.LogError(ex, "API call failed");
    return new { error = "An error occurred processing your request" };
}
```

## 🔍 Security Auditing

### Request Logging

```csharp
public class AuditingMiddleware : ICurlMiddleware
{
    private readonly ILogger _logger;

    public async Task<CurlResult> ExecuteAsync(CurlContext context, Func<CurlContext, Task<CurlResult>> next)
    {
        // Log request (without sensitive data)
        _logger.LogInformation("Request to {Url} by {User}",
            context.Request.Url,
            context.User?.Identity?.Name ?? "Anonymous");

        var result = await next(context);

        // Log response
        _logger.LogInformation("Response {StatusCode} from {Url}",
            result.StatusCode,
            context.Request.Url);

        return result;
    }
}
```

## 🏗️ Infrastructure Security

### Network Isolation

```csharp
// Use specific network interface
var result = await Curl.ExecuteAsync(@"
    curl --interface eth0 \
         https://api.example.com
");

// Bind to specific IP
var result = await Curl.ExecuteAsync(@"
    curl --interface 192.168.1.100 \
         https://api.example.com
");
```

### Rate Limiting

Implement client-side rate limiting:

```csharp
public class RateLimitedClient
{
    private readonly SemaphoreSlim _semaphore;
    private readonly Timer _timer;
    private int _requestCount;

    public RateLimitedClient(int maxRequests, TimeSpan period)
    {
        _semaphore = new SemaphoreSlim(maxRequests);
        _timer = new Timer(_ => ResetLimit(), null, period, period);
    }

    public async Task<CurlResult> ExecuteAsync(string command)
    {
        await _semaphore.WaitAsync();
        try
        {
            Interlocked.Increment(ref _requestCount);
            return await Curl.ExecuteAsync(command);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private void ResetLimit()
    {
        var requests = Interlocked.Exchange(ref _requestCount, 0);
        Console.WriteLine($"Reset limit. Processed {requests} requests.");
    }
}
```

## 📋 Security Checklist

Before deploying to production:

- [ ] Certificate validation is enabled
- [ ] Credentials are stored securely (not hardcoded)
- [ ] All user input is validated and sanitized
- [ ] TLS 1.2 or higher is enforced
- [ ] Sensitive data is not logged
- [ ] Error messages don't expose internal details
- [ ] Rate limiting is implemented
- [ ] Security headers are included
- [ ] Audit logging is configured
- [ ] SSRF protections are in place

## 🔗 Related Resources

- [SSL/TLS Errors](/exceptions/ssl-errors.md)
- [Authentication Errors](/exceptions/auth-errors.md)
- [API Guide](/api-guide/)
- [Troubleshooting](/troubleshooting/)

## 📚 External Resources

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [SSL Labs Best Practices](https://github.com/ssllabs/research/wiki/SSL-and-TLS-Deployment-Best-Practices)
- [RFC 5246 - TLS 1.2](https://tools.ietf.org/html/rfc5246)

---
*Security is not optional. Always follow best practices in production.*