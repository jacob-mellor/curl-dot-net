# Error Reference

CurlDotNet provides specific exception types for each curl error code, making it easy to handle different error conditions precisely.

## Exception Hierarchy

All CurlDotNet exceptions inherit from `CurlException`:

```
CurlException (base class)
├── CurlHttpException (HTTP 4xx/5xx errors)
├── CurlFtpException (FTP errors)
├── CurlTimeoutException (CURLE_OPERATION_TIMEDOUT)
├── CurlSslException (SSL/TLS errors)
├── CurlCouldntResolveHostException (DNS failures)
├── CurlCouldntConnectException (Connection failures)
├── CurlParsingException (Invalid curl command)
└── ... (91 total exception types)
```

## Common Exceptions

### CurlTimeoutException

**Error Code:** CURLE_OPERATION_TIMEDOUT (28)

Thrown when the request takes longer than the configured timeout.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl --max-time 5 https://slow-api.com");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Request timed out after {ex.Timeout} seconds");
    Console.WriteLine($"Error code: {ex.CurlErrorCode}");
}
```

**Solutions:**
- Increase the timeout: `--max-time 60`
- Check if the server is responding
- Use `DefaultMaxTimeSeconds` for global timeout

---

### CurlCouldntResolveHostException

**Error Code:** CURLE_COULDNT_RESOLVE_HOST (6)

Thrown when DNS lookup fails for the hostname.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://nonexistent-domain.com");
}
catch (CurlCouldntResolveHostException ex)
{
    Console.WriteLine($"Could not find host: {ex.Hostname}");
}
```

**Solutions:**
- Verify the URL is spelled correctly
- Check your internet connection
- Check DNS settings
- Try using IP address directly

---

### CurlCouldntConnectException

**Error Code:** CURLE_COULDNT_CONNECT (7)

Thrown when the connection to the host cannot be established.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com:9999");
}
catch (CurlCouldntConnectException ex)
{
    Console.WriteLine($"Could not connect to {ex.Host}:{ex.Port}");
}
```

**Solutions:**
- Verify the server is running
- Check firewall settings
- Verify the port is correct
- Try with `--connect-timeout` to fail faster

---

### CurlSslException

**Error Codes:** CURLE_SSL_CONNECT_ERROR (35), CURLE_PEER_FAILED_VERIFICATION (60)

Thrown for SSL/TLS certificate problems.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://self-signed-cert.example.com");
}
catch (CurlSslException ex)
{
    Console.WriteLine($"SSL error: {ex.Message}");
    if (ex.CertificateError != null)
    {
        Console.WriteLine($"Certificate issue: {ex.CertificateError}");
    }
}
```

**Solutions:**
- **Development only:** Use `-k` or `--insecure`
- Install the CA certificate
- Update system certificates
- Use `--cacert` to specify custom CA bundle

---

### CurlHttpException

Thrown for HTTP error status codes (4xx, 5xx).

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl -f https://api.example.com/notfound");
}
catch (CurlHttpException ex)
{
    Console.WriteLine($"HTTP Error: {ex.StatusCode}");
    Console.WriteLine($"Response body: {ex.ResponseBody}");

    switch (ex.StatusCode)
    {
        case 400: Console.WriteLine("Bad request - check your parameters"); break;
        case 401: Console.WriteLine("Unauthorized - check your credentials"); break;
        case 403: Console.WriteLine("Forbidden - you don't have permission"); break;
        case 404: Console.WriteLine("Not found - resource doesn't exist"); break;
        case 429: Console.WriteLine("Rate limited - slow down requests"); break;
        case 500: Console.WriteLine("Server error - try again later"); break;
    }
}
```

---

### CurlMalformedUrlException

**Error Code:** CURLE_URL_MALFORMAT (3)

Thrown when the URL is invalid or malformed.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl not-a-valid-url");
}
catch (CurlMalformedUrlException ex)
{
    Console.WriteLine($"Invalid URL: {ex.MalformedUrl}");
}
```

**Solutions:**
- Include the protocol (http:// or https://)
- Check for typos in the URL
- URL-encode special characters

---

### CurlUnsupportedProtocolException

**Error Code:** CURLE_UNSUPPORTED_PROTOCOL (1)

Thrown when the URL uses an unsupported protocol.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl gopher://example.com");
}
catch (CurlUnsupportedProtocolException ex)
{
    Console.WriteLine($"Unsupported protocol: {ex.Protocol}");
}
```

**Supported protocols:** http, https, ftp, ftps, file

---

### CurlParsingException

Thrown when the curl command cannot be parsed.

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl --invalid-option https://api.example.com");
}
catch (CurlParsingException ex)
{
    Console.WriteLine($"Invalid command: {ex.Message}");
    Console.WriteLine($"Position: {ex.ErrorPosition}");
}
```

**Solutions:**
- Check curl option spelling
- Verify quote matching
- Use `Curl.Validate()` to check commands before executing

---

## Complete Error Code Reference

| Code | Exception | Description |
|------|-----------|-------------|
| 1 | `CurlUnsupportedProtocolException` | Unsupported protocol |
| 2 | `CurlFailedInitException` | Initialization failed |
| 3 | `CurlMalformedUrlException` | Invalid URL format |
| 5 | `CurlCouldntResolveProxyException` | Cannot resolve proxy |
| 6 | `CurlCouldntResolveHostException` | Cannot resolve host (DNS) |
| 7 | `CurlCouldntConnectException` | Cannot connect to host |
| 8 | `CurlWeirdServerReplyException` | Unexpected server response |
| 9 | `CurlRemoteAccessDeniedException` | Access denied |
| 22 | `CurlHttpException` | HTTP error (4xx/5xx) |
| 23 | `CurlWriteException` | Write error |
| 26 | `CurlReadException` | Read error |
| 27 | `CurlOutOfMemoryException` | Out of memory |
| 28 | `CurlTimeoutException` | Operation timed out |
| 33 | `CurlRangeException` | Range error |
| 35 | `CurlSslConnectException` | SSL connection error |
| 47 | `CurlTooManyRedirectsException` | Too many redirects |
| 51 | `CurlPeerFailedVerificationException` | SSL peer verification failed |
| 52 | `CurlGotNothingException` | Empty response |
| 55 | `CurlSendException` | Send error |
| 56 | `CurlReceiveException` | Receive error |
| 60 | `CurlSslCertificateException` | SSL certificate problem |
| 67 | `CurlLoginDeniedException` | Login denied |

## Best Practices for Error Handling

### 1. Catch Specific Exceptions First

```csharp
try
{
    var result = await Curl.ExecuteAsync(command);
}
catch (CurlTimeoutException ex)
{
    // Handle timeout specifically
    await RetryWithLongerTimeout();
}
catch (CurlCouldntResolveHostException ex)
{
    // Handle DNS failure
    Log.Error($"Unknown host: {ex.Hostname}");
}
catch (CurlHttpException ex) when (ex.StatusCode == 429)
{
    // Handle rate limiting
    await Task.Delay(TimeSpan.FromSeconds(60));
    await RetryRequest();
}
catch (CurlException ex)
{
    // Handle all other curl errors
    Log.Error($"Curl error {ex.CurlErrorCode}: {ex.Message}");
}
```

### 2. Use Pattern Matching

```csharp
try
{
    var result = await Curl.ExecuteAsync(command);
}
catch (CurlException ex)
{
    var message = ex switch
    {
        CurlTimeoutException => "The request timed out. Please try again.",
        CurlCouldntResolveHostException e => $"Could not find {e.Hostname}",
        CurlSslException => "SSL certificate error. Check your connection.",
        CurlHttpException { StatusCode: 401 } => "Please log in first.",
        CurlHttpException { StatusCode: 404 } => "Resource not found.",
        _ => $"Request failed: {ex.Message}"
    };

    Console.WriteLine(message);
}
```

### 3. Access Exception Properties

All CurlDotNet exceptions provide useful properties:

```csharp
catch (CurlException ex)
{
    Console.WriteLine($"Error code: {ex.CurlErrorCode}");
    Console.WriteLine($"Message: {ex.Message}");
    Console.WriteLine($"Command: {ex.Command}");

    // For HTTP errors
    if (ex is CurlHttpException httpEx)
    {
        Console.WriteLine($"Status: {httpEx.StatusCode}");
        Console.WriteLine($"Body: {httpEx.ResponseBody}");
        Console.WriteLine($"Headers: {string.Join(", ", httpEx.ResponseHeaders)}");
    }
}
```

## Related Topics

- [Common Issues](common-issues.html) - Solutions to frequent problems
- [FAQ](faq.html) - Frequently asked questions
- [Configuration](../getting-started/configuration.html) - Set timeouts and other options
