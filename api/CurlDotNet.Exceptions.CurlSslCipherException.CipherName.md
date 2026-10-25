#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlSslCipherException](CurlDotNet.Exceptions.CurlSslCipherException.md 'CurlDotNet\.Exceptions\.CurlSslCipherException')

## CurlSslCipherException\.CipherName Property

The cipher \(or cipher list\) from `--ciphers` that could not be used, such as `TLS_AES_256_GCM_SHA384`\.

```csharp
public string CipherName { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The value as given in the command\.

### Remarks

Check the platform's supported cipher suites before setting one.

### See Also
- [CertificateError](CurlDotNet.Exceptions.CurlSslException.CertificateError.md 'CurlDotNet\.Exceptions\.CurlSslException\.CertificateError')