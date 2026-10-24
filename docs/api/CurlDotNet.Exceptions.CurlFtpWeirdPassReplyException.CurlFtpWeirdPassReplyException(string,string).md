#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFtpWeirdPassReplyException](CurlDotNet.Exceptions.CurlFtpWeirdPassReplyException.md 'CurlDotNet\.Exceptions\.CurlFtpWeirdPassReplyException')

## CurlFtpWeirdPassReplyException\(string, string\) Constructor

Creates the error for an unexpected reply to the FTP password command, with advice on credentials and FTPS\.

```csharp
public CurlFtpWeirdPassReplyException(string message, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFtpWeirdPassReplyException.CurlFtpWeirdPassReplyException(string,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What the server answered, quoting its reply line; optionally followed by fix lines\.

<a name='CurlDotNet.Exceptions.CurlFtpWeirdPassReplyException.CurlFtpWeirdPassReplyException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

When [message](CurlDotNet.Exceptions.CurlFtpWeirdPassReplyException.CurlFtpWeirdPassReplyException(string,string).md#CurlDotNet.Exceptions.CurlFtpWeirdPassReplyException.CurlFtpWeirdPassReplyException(string,string).message 'CurlDotNet\.Exceptions\.CurlFtpWeirdPassReplyException\.CurlFtpWeirdPassReplyException\(string, string\)\.message') has no fix, the suggestion covers `-u` and `--ssl-reqd`.

### See Also
- [CurlLoginDeniedException](CurlDotNet.Exceptions.CurlLoginDeniedException.md 'CurlDotNet\.Exceptions\.CurlLoginDeniedException')