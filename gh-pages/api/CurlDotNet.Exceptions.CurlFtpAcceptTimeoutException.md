#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlFtpAcceptTimeoutException Class

CURLE\_FTP\_ACCEPT\_TIMEOUT \(12\) \- FTP accept timeout

```csharp
public class CurlFtpAcceptTimeoutException : CurlDotNet.Exceptions.CurlTimeoutException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException') &#129106; CurlFtpAcceptTimeoutException
### Constructors

<a name='CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.CurlFtpAcceptTimeoutException(string,string)'></a>

## CurlFtpAcceptTimeoutException\(string, string\) Constructor

Initializes a new instance of the [CurlFtpAcceptTimeoutException](CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.md 'CurlDotNet\.Exceptions\.CurlFtpAcceptTimeoutException') class\.

```csharp
public CurlFtpAcceptTimeoutException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.CurlFtpAcceptTimeoutException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message that explains the reason for the exception\.

<a name='CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.CurlFtpAcceptTimeoutException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.