#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFtpException](CurlDotNet.Exceptions.CurlFtpException.md 'CurlDotNet\.Exceptions\.CurlFtpException')

## CurlFtpException\(string, int, string\) Constructor

Creates an FTP error \(exit code 9\) with the server's reply code, whose meaning chooses the advice in the
message\.

```csharp
public CurlFtpException(string message, int ftpCode, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFtpException.CurlFtpException(string,int,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What the server refused, naming the remote path; optionally followed by fix lines\.

<a name='CurlDotNet.Exceptions.CurlFtpException.CurlFtpException(string,int,string).ftpCode'></a>

`ftpCode` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The FTP reply code the server sent, such as 550\.

<a name='CurlDotNet.Exceptions.CurlFtpException.CurlFtpException(string,int,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The original curl command that was executed, or null\.

### Remarks

Example: `throw new CurlFtpException("RETR /pub/report.csv was refused", 550, command);` produces advice to
            check the remote path and permissions.

### See Also
- [CurlRemoteAccessDeniedException](CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.md 'CurlDotNet\.Exceptions\.CurlRemoteAccessDeniedException')