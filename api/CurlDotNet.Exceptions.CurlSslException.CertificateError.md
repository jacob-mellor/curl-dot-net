#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')

## CurlSslException\.CertificateError Property

The TLS layer's own description of the certificate problem, such as `RemoteCertificateNameMismatch`
or `UntrustedRoot`, when it was reported\.

```csharp
public string? CertificateError { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The platform's error text, or null when the failure was not about a certificate\.

### Remarks

Example: `if (ex.CertificateError?.Contains("NameMismatch") == true) Console.WriteLine("Use the host name the certificate was issued for.");`

### See Also
- [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException')