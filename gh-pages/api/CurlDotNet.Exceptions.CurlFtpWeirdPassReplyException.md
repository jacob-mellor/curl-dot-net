#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](index.md#CurlDotNet.Exceptions 'CurlDotNet\.Exceptions')

## CurlFtpWeirdPassReplyException Class

CURLE\_FTP\_WEIRD\_PASS\_REPLY \(11\) \- FTP weird PASS reply

```csharp
public class CurlFtpWeirdPassReplyException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFtpWeirdPassReplyException
### Constructors

<a name='CurlDotNet.Exceptions.CurlFtpWeirdPassReplyException.CurlFtpWeirdPassReplyException(string,string)'></a>

## CurlFtpWeirdPassReplyException\(string, string\) Constructor

Initializes a new instance of the [CurlFtpWeirdPassReplyException](CurlDotNet.Exceptions.CurlFtpWeirdPassReplyException.md 'CurlDotNet\.Exceptions\.CurlFtpWeirdPassReplyException') class\.

```csharp
public CurlFtpWeirdPassReplyException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFtpWeirdPassReplyException.CurlFtpWeirdPassReplyException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The error message that explains the reason for the exception\.

<a name='CurlDotNet.Exceptions.CurlFtpWeirdPassReplyException.CurlFtpWeirdPassReplyException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.