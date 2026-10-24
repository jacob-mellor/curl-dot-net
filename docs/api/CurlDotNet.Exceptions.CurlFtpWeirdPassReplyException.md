#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlFtpWeirdPassReplyException Class

The FTP server answered the password \(PASS\) command with something unexpected \(exit code 11,
CURLE\_FTP\_WEIRD\_PASS\_REPLY\), usually a rejected or malformed login\.

```csharp
public class CurlFtpWeirdPassReplyException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFtpWeirdPassReplyException

### Remarks

Check `-u user:password` (quote it when the password has special characters) and whether the server
            requires FTPS (`--ssl-reqd`). Example: `catch (CurlFtpWeirdPassReplyException ex) { Console.WriteLine(ex.WhatHappened); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/auth-errors.md

### See Also
- [CurlLoginDeniedException](CurlDotNet.Exceptions.CurlLoginDeniedException.md 'CurlDotNet\.Exceptions\.CurlLoginDeniedException')

| Constructors | |
| :--- | :--- |
| [CurlFtpWeirdPassReplyException\(string, string\)](CurlDotNet.Exceptions.CurlFtpWeirdPassReplyException.CurlFtpWeirdPassReplyException(string,string).md 'CurlDotNet\.Exceptions\.CurlFtpWeirdPassReplyException\.CurlFtpWeirdPassReplyException\(string, string\)') | Creates the error for an unexpected reply to the FTP password command, with advice on credentials and FTPS\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlFtpWeirdPassReplyException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlFtpWeirdPassReplyException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlFtpWeirdPassReplyException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlFtpWeirdPassReplyException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
