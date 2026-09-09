#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlSslException Class

Thrown when SSL/TLS certificate validation fails

```csharp
public class CurlSslException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlSslException

Derived  
&#8627; [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException')  
&#8627; [CurlSslCertificateProblemException](CurlDotNet.Exceptions.CurlSslCertificateProblemException.md 'CurlDotNet\.Exceptions\.CurlSslCertificateProblemException')  
&#8627; [CurlSslCipherException](CurlDotNet.Exceptions.CurlSslCipherException.md 'CurlDotNet\.Exceptions\.CurlSslCipherException')  
&#8627; [CurlSslConnectErrorException](CurlDotNet.Exceptions.CurlSslConnectErrorException.md 'CurlDotNet\.Exceptions\.CurlSslConnectErrorException')  
&#8627; [CurlSslEngineNotFoundException](CurlDotNet.Exceptions.CurlSslEngineNotFoundException.md 'CurlDotNet\.Exceptions\.CurlSslEngineNotFoundException')  
&#8627; [CurlSslEngineSetFailedException](CurlDotNet.Exceptions.CurlSslEngineSetFailedException.md 'CurlDotNet\.Exceptions\.CurlSslEngineSetFailedException')  
&#8627; [CurlUseSslFailedException](CurlDotNet.Exceptions.CurlUseSslFailedException.md 'CurlDotNet\.Exceptions\.CurlUseSslFailedException')

| Constructors | |
| :--- | :--- |
| [CurlSslException\(string, string, string\)](CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string).md 'CurlDotNet\.Exceptions\.CurlSslException\.CurlSslException\(string, string, string\)') | Initializes a new instance of CurlSslException\. |

| Properties | |
| :--- | :--- |
| [CertificateError](CurlDotNet.Exceptions.CurlSslException.CertificateError.md 'CurlDotNet\.Exceptions\.CurlSslException\.CertificateError') | Details about the certificate error that occurred\. |
