#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlFtpAcceptTimeoutException Class

In active FTP mode the server did not connect back within the accept timeout \(exit code 12,
CURLE\_FTP\_ACCEPT\_TIMEOUT\); a [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException') so timeout handlers catch it too\.

```csharp
public class CurlFtpAcceptTimeoutException : CurlDotNet.Exceptions.CurlTimeoutException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException') &#129106; CurlFtpAcceptTimeoutException

### Remarks

Use passive mode (no `-P`) or raise `--ftp-pasv`/`--connect-timeout`. Example:
            `catch (CurlFtpAcceptTimeoutException) { /* run again without -P */ }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/timeout-errors.md

### See Also
- [CurlFtpAcceptFailedException](CurlDotNet.Exceptions.CurlFtpAcceptFailedException.md 'CurlDotNet\.Exceptions\.CurlFtpAcceptFailedException')
- [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException')

| Constructors | |
| :--- | :--- |
| [CurlFtpAcceptTimeoutException\(string, string\)](CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.CurlFtpAcceptTimeoutException(string,string).md 'CurlDotNet\.Exceptions\.CurlFtpAcceptTimeoutException\.CurlFtpAcceptTimeoutException\(string, string\)') | Creates the error \(exit code 12\) for an FTP server that did not open the active\-mode data connection in time\. |

| Properties | |
| :--- | :--- |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlFtpAcceptTimeoutException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlFtpAcceptTimeoutException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
