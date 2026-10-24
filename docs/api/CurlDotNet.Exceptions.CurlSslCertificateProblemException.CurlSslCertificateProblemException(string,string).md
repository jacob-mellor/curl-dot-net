#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlSslCertificateProblemException](CurlDotNet.Exceptions.CurlSslCertificateProblemException.md 'CurlDotNet\.Exceptions\.CurlSslCertificateProblemException')

## CurlSslCertificateProblemException\(string, string\) Constructor

Initializes a new instance of the [CurlSslCertificateProblemException](CurlDotNet.Exceptions.CurlSslCertificateProblemException.md 'CurlDotNet\.Exceptions\.CurlSslCertificateProblemException') class\.

```csharp
public CurlSslCertificateProblemException(string certError, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslCertificateProblemException.CurlSslCertificateProblemException(string,string).certError'></a>

`certError` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Details about the certificate error\.

<a name='CurlDotNet.Exceptions.CurlSslCertificateProblemException.CurlSslCertificateProblemException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.