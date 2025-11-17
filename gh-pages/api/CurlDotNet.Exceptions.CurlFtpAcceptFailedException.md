#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlFtpAcceptFailedException Class

CURLE\_FTP\_ACCEPT\_FAILED \(10\) \- FTP accept failed

```csharp
public class CurlFtpAcceptFailedException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFtpAcceptFailedException
### Constructors

<a name='CurlDotNet.Exceptions.CurlFtpAcceptFailedException.CurlFtpAcceptFailedException(string,string)'></a>

## CurlFtpAcceptFailedException\(string, string\) Constructor

Initializes a new instance of the CurlFtpAcceptFailedException class\.

```csharp
public CurlFtpAcceptFailedException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFtpAcceptFailedException.CurlFtpAcceptFailedException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message that explains the reason for the exception\.

<a name='CurlDotNet.Exceptions.CurlFtpAcceptFailedException.CurlFtpAcceptFailedException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.