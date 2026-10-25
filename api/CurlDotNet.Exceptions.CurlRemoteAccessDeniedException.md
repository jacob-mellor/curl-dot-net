#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlRemoteAccessDeniedException Class

The server refused access to the remote resource \(exit code 9, CURLE\_REMOTE\_ACCESS\_DENIED\), typically an FTP
login or directory the account may not use; [Resource](CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.Resource.md 'CurlDotNet\.Exceptions\.CurlRemoteAccessDeniedException\.Resource') names it\.

```csharp
public class CurlRemoteAccessDeniedException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlRemoteAccessDeniedException

### Remarks

Fixes: check the credentials (`-u user:password`), the remote path (case-sensitive) and the account's
            permissions. Example: `catch (CurlRemoteAccessDeniedException ex) { Console.WriteLine($"No access to {ex.Resource}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/auth-errors.md

### See Also
- [CurlFtpException](CurlDotNet.Exceptions.CurlFtpException.md 'CurlDotNet\.Exceptions\.CurlFtpException')
- [CurlLoginDeniedException](CurlDotNet.Exceptions.CurlLoginDeniedException.md 'CurlDotNet\.Exceptions\.CurlLoginDeniedException')

| Constructors | |
| :--- | :--- |
| [CurlRemoteAccessDeniedException\(string, string\)](CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.CurlRemoteAccessDeniedException(string,string).md 'CurlDotNet\.Exceptions\.CurlRemoteAccessDeniedException\.CurlRemoteAccessDeniedException\(string, string\)') | Creates the access\-denied error \(exit code 9\) for [resource](CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.CurlRemoteAccessDeniedException(string,string).md#CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.CurlRemoteAccessDeniedException(string,string).resource 'CurlDotNet\.Exceptions\.CurlRemoteAccessDeniedException\.CurlRemoteAccessDeniedException\(string, string\)\.resource'), with advice on credentials, path and permissions\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlRemoteAccessDeniedException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlRemoteAccessDeniedException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [Resource](CurlDotNet.Exceptions.CurlRemoteAccessDeniedException.Resource.md 'CurlDotNet\.Exceptions\.CurlRemoteAccessDeniedException\.Resource') | The remote resource \(path, directory or URL\) the server refused access to, as requested\. |
