#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlSslCertificateProblemException](CurlDotNet.Exceptions.CurlSslCertificateProblemException.md 'CurlDotNet\.Exceptions\.CurlSslCertificateProblemException')

## CurlSslCertificateProblemException\(string, string\) Constructor

Creates the error \(exit code 58\) for an unusable client certificate, with advice on path, type, password and
key\.

```csharp
public CurlSslCertificateProblemException(string certError, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslCertificateProblemException.CurlSslCertificateProblemException(string,string).certError'></a>

`certError` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What is wrong with the local certificate, such as "file not found" or "bad password"\.

<a name='CurlDotNet.Exceptions.CurlSslCertificateProblemException.CurlSslCertificateProblemException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Message: `Problem with local certificate: DETAIL`.

### See Also
- [CertificateError](CurlDotNet.Exceptions.CurlSslException.CertificateError.md 'CurlDotNet\.Exceptions\.CurlSslException\.CertificateError')