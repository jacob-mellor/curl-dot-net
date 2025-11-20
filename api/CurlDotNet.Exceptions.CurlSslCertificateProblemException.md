#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlSslCertificateProblemException Class

CURLE\_SSL\_CERTPROBLEM \(58\) \- Problem with local certificate

```csharp
public class CurlSslCertificateProblemException : CurlDotNet.Exceptions.CurlSslException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException') &#129106; CurlSslCertificateProblemException
### Constructors

<a name='CurlDotNet.Exceptions.CurlSslCertificateProblemException.CurlSslCertificateProblemException(string,string)'></a>

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