#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlFtpException Class

An FTP server refused a command \(curl exit code 9, CURLE\_REMOTE\_ACCESS\_DENIED\); [FtpCode](CurlDotNet.Exceptions.CurlFtpException.FtpCode.md 'CurlDotNet\.Exceptions\.CurlFtpException\.FtpCode') holds the
server's reply code, such as 530 \(not logged in\) or 550 \(file unavailable\)\.

```csharp
public class CurlFtpException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFtpException

### Remarks

The FTP handler raises its own `CurlDotNet.Core.CurlFtpException`; this type is for application code and
            callers that catch it. Fixes by reply code: 530 check `-u user:password`; 550 check the remote path and
            permissions; 553 check the file name; 421/425/426 retry or switch between passive and active mode.

Example: `catch (CurlFtpException ex) when (ex.FtpCode == 550) { Console.WriteLine("No such remote file"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md

### See Also
- [CurlRemoteAccessDeniedException](CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.md 'CurlDotNet\.Exceptions\.CurlRemoteAccessDeniedException')

| Constructors | |
| :--- | :--- |
| [CurlFtpException\(string, int, string\)](CurlDotNet.Exceptions.CurlFtpException.CurlFtpException(string,int,string).md 'CurlDotNet\.Exceptions\.CurlFtpException\.CurlFtpException\(string, int, string\)') | Creates an FTP error \(exit code 9\) with the server's reply code, whose meaning chooses the advice in the message\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlFtpException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlFtpException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlFtpException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlFtpException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [FtpCode](CurlDotNet.Exceptions.CurlFtpException.FtpCode.md 'CurlDotNet\.Exceptions\.CurlFtpException\.FtpCode') | The three\-digit FTP reply code the server answered with, for example 530 \(not logged in\), 550 \(file not found or no permission\) or 421 \(service not available\)\. |
