#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFtpAcceptFailedException](CurlDotNet.Exceptions.CurlFtpAcceptFailedException.md 'CurlDotNet\.Exceptions\.CurlFtpAcceptFailedException')

## CurlFtpAcceptFailedException\(string, string\) Constructor

Creates the error for an active\-mode FTP data connection that the server could not open back to this
machine\.

```csharp
public CurlFtpAcceptFailedException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFtpAcceptFailedException.CurlFtpAcceptFailedException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What failed, naming the FTP server; optionally followed by fix lines\.

<a name='CurlDotNet.Exceptions.CurlFtpAcceptFailedException.CurlFtpAcceptFailedException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlFtpAcceptFailedException.CurlFtpAcceptFailedException(string,string).md#CurlDotNet.Exceptions.CurlFtpAcceptFailedException.CurlFtpAcceptFailedException(string,string).message 'CurlDotNet\.Exceptions\.CurlFtpAcceptFailedException\.CurlFtpAcceptFailedException\(string, string\)\.message') has no fix, the suggestion recommends passive mode.

### See Also
- [CurlFtpAcceptTimeoutException](CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.md 'CurlDotNet\.Exceptions\.CurlFtpAcceptTimeoutException')