# SSL/TLS Certificate Errors

## Overview

SSL/TLS errors occur when there are issues with secure connections, including certificate validation, cipher negotiation, and protocol mismatches.

## Common SSL Error Scenarios

### 1. Invalid Certificate

```csharp
try
{
    var result = await curl.ExecuteAsync("curl https://self-signed.example.com");
}
catch (CurlSslException ex)
{
    // For development/testing only - skip certificate verification
    var result = await curl.ExecuteAsync("curl -k https://self-signed.example.com");
}
```

### 2. Certificate Chain Issues

```csharp
// Provide CA bundle for verification
var result = await curl.ExecuteAsync(
    "curl --cacert /path/to/ca-bundle.crt https://example.com"
);
```

### 3. Expired Certificates

```csharp
public async Task<bool> CheckCertificateExpiry(string url)
{
    var result = await curl.ExecuteAsync($"curl -v {url}");
    // Parse certificate info from verbose output
    return !result.StdErr.Contains("certificate has expired");
}
```

## Solutions

### Development Environment

```csharp
// WARNING: Only for development/testing
public class DevCurlClient
{
    public async Task<CurlResult> ExecuteInsecure(string url)
    {
        return await curl.ExecuteAsync($"curl --insecure {url}");
    }
}
```

### Production Environment

```csharp
public class SecureCurlClient
{
    private readonly string _caBundlePath;

    public SecureCurlClient(string caBundlePath)
    {
        _caBundlePath = caBundlePath;
    }

    public async Task<CurlResult> Execute(string url)
    {
        return await curl.ExecuteAsync($"curl --cacert {_caBundlePath} {url}");
    }
}
```

## Certificate Pinning

```csharp
// Pin specific certificate
var pinnedCert = "sha256//AbC123...";
var result = await curl.ExecuteAsync(
    $"curl --pinnedpubkey {pinnedCert} https://example.com"
);
```

## Related Documentation
- [Authentication Errors](auth-errors.md)
- [Security Best Practices](/guides/security.md)