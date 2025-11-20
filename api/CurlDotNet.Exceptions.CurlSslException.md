#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

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
### Constructors

<a name='CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string)'></a>

## CurlSslException\(string, string, string\) Constructor

Initializes a new instance of CurlSslException\.

```csharp
public CurlSslException(string message, string? certError=null, string? command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message\.

<a name='CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string).certError'></a>

`certError` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Details about the certificate error\.

<a name='CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that failed\.
### Properties

<a name='CurlDotNet.Exceptions.CurlSslException.CertificateError'></a>

## CurlSslException\.CertificateError Property

Details about the certificate error that occurred\.

```csharp
public string? CertificateError { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')