#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFtpAcceptTimeoutException](CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.md 'CurlDotNet\.Exceptions\.CurlFtpAcceptTimeoutException')

## CurlFtpAcceptTimeoutException\(string, string\) Constructor

Creates the error \(exit code 12\) for an FTP server that did not open the active\-mode data connection in time\.

```csharp
public CurlFtpAcceptTimeoutException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.CurlFtpAcceptTimeoutException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What timed out, naming the FTP server; optionally followed by fix lines\.

<a name='CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.CurlFtpAcceptTimeoutException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.CurlFtpAcceptTimeoutException(string,string).md#CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.CurlFtpAcceptTimeoutException(string,string).message 'CurlDotNet\.Exceptions\.CurlFtpAcceptTimeoutException\.CurlFtpAcceptTimeoutException\(string, string\)\.message') has no fix, the suggestion recommends passive mode.

### See Also
- [CurlFtpAcceptFailedException](CurlDotNet.Exceptions.CurlFtpAcceptFailedException.md 'CurlDotNet\.Exceptions\.CurlFtpAcceptFailedException')