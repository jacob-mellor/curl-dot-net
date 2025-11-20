# CurlDotNet Exception Documentation

CurlDotNet provides detailed exception handling with specific exception types for every curl error code.

## Exception Categories

### [Authentication Errors](auth-errors.md)
- Login denied, authentication failures
- Credential issues

### [Cookie Errors](cookie-errors.md)
- Cookie handling problems
- Session management issues

### [DNS Errors](dns-errors.md)
- Host resolution failures
- Proxy resolution issues

### [HTTP Errors](http-errors.md)
- HTTP protocol errors
- Status code issues

### [Initialization Errors](init-errors.md)
- Curl initialization failures
- Configuration problems

### [Redirect Errors](redirect-errors.md)
- Too many redirects
- Redirect loop detection

### [SSL/TLS Errors](ssl-errors.md)
- Certificate validation failures
- SSL handshake issues
- Cipher problems

### [Timeout Errors](timeout-errors.md)
- Connection timeouts
- Operation timeouts
- Transfer timeouts

### [URL Errors](url-errors.md)
- Malformed URLs
- Unsupported protocols

## Using Exception Types

Each exception provides:
- Specific error code
- Descriptive message
- Diagnostic information
- Link to relevant documentation

## Example

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://example.com");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Timeout: {ex.Message}");
    Console.WriteLine($"Diagnostic: {ex.DiagnosticInfo}");
}
catch (CurlSslException ex)
{
    Console.WriteLine($"SSL Error: {ex.Message}");
    Console.WriteLine($"Certificate Issue: {ex.CertificateError}");
}
```