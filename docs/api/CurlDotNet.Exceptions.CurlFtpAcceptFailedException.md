#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlFtpAcceptFailedException Class

In active FTP mode the server never connected back to accept the data connection \(exit code 10,
CURLE\_FTP\_ACCEPT\_FAILED\), usually because a firewall or NAT blocks incoming connections\.

```csharp
public class CurlFtpAcceptFailedException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFtpAcceptFailedException

### Remarks

Use passive mode (the default; do not pass `-P`/`--ftp-port`) or open the port range on the firewall.
            Example: `catch (CurlFtpAcceptFailedException) { /* retry without -P */ }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md

### See Also
- [CurlFtpAcceptTimeoutException](CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.md 'CurlDotNet\.Exceptions\.CurlFtpAcceptTimeoutException')

| Constructors | |
| :--- | :--- |
| [CurlFtpAcceptFailedException\(string, string\)](CurlDotNet.Exceptions.CurlFtpAcceptFailedException.CurlFtpAcceptFailedException(string,string).md 'CurlDotNet\.Exceptions\.CurlFtpAcceptFailedException\.CurlFtpAcceptFailedException\(string, string\)') | Creates the error for an active\-mode FTP data connection that the server could not open back to this machine\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlFtpAcceptFailedException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlFtpAcceptFailedException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlFtpAcceptFailedException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlFtpAcceptFailedException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
