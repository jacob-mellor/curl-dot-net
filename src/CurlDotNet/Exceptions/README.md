# Exception Hierarchy

This directory contains the exception types that map to curl error codes, providing detailed error information for .NET developers.

## Purpose

CurlDotNet provides a comprehensive exception hierarchy that maps to curl's error codes, giving developers precise error information and enabling proper error handling in their applications.

## Exception Classes

### Base Exception

**`CurlException`** (`CurlExceptions.cs`) - Base exception for all curl-related errors. Provides:
- Error message
- Original curl command (if available)
- Curl error code

**Usage**: Catch this to handle any curl-related error in your application.

### HTTP Exception

**`CurlHttpException`** (`CurlExceptions.cs`) - Exception for HTTP-specific errors. Extends `CurlException` with:
- HTTP status code
- Response headers
- Response body (for debugging)

**Usage**: Catch this to handle HTTP errors (4xx, 5xx) specifically.

### SSL Exception

**`CurlSslException`** (`CurlExceptions.cs`) - Exception for SSL/TLS errors. Extends `CurlException` with:
- Certificate error details
- SSL verification failure information

**Usage**: Catch this to handle SSL certificate or TLS connection errors.

### Specific Error Exceptions

**`CurlExceptionTypes.cs`** - Contains specific exception types for each curl error code:
- `CurlHttpReturnedErrorException` - HTTP error responses
- `CurlWriteErrorException` - Write/disk I/O errors
- `CurlReadErrorException` - Read/network errors
- `CurlHttpPostErrorException` - HTTP POST-specific errors
- `CurlSslConnectErrorException` - SSL connection errors
- `CurlSendErrorException` - Network send errors
- `CurlReceiveErrorException` - Network receive errors
- And more...

**Usage**: Catch specific exception types for granular error handling.

## Design Principles

1. **Comprehensive Mapping** - Every curl error code has a corresponding exception type
2. **Rich Context** - Exceptions include relevant context (command, response, headers)
3. **Intuitive Hierarchy** - Exception hierarchy matches curl's error categorization
4. **Developer-Friendly** - Clear, actionable error messages

## Error Handling Example

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl https://api.example.com/data");
    // Process result
}
catch (CurlHttpException ex)
{
    // Handle HTTP errors (404, 500, etc.)
    Console.WriteLine($"HTTP {ex.StatusCode}: {ex.Message}");
}
catch (CurlSslException ex)
{
    // Handle SSL errors
    Console.WriteLine($"SSL Error: {ex.CertificateError}");
}
catch (CurlException ex)
{
    // Handle any other curl error
    Console.WriteLine($"Error: {ex.Message}");
}
```

## See Also

- [CurlExceptions.cs](./CurlExceptions.cs)
- [CurlExceptionTypes.cs](./CurlExceptionTypes.cs)

