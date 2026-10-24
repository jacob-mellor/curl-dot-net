#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlSendErrorException Class

Sending the request failed part\-way \(exit code 55, CURLE\_SEND\_ERROR\): the connection was reset or closed while
the request or its body was being written\.

```csharp
public class CurlSendErrorException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlSendErrorException

### Remarks

Often transient (network glitch, server restart) or caused by a server that refuses large uploads by closing the
            connection. Retry with `--retry 3`; for uploads check the server's size limit.
            Example: `catch (CurlSendErrorException ex) when (ex.IsRetryable()) { /* retry */ }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md

### See Also
- [CurlReceiveErrorException](CurlDotNet.Exceptions.CurlReceiveErrorException.md 'CurlDotNet\.Exceptions\.CurlReceiveErrorException')

| Constructors | |
| :--- | :--- |
| [CurlSendErrorException\(string, string\)](CurlDotNet.Exceptions.CurlSendErrorException.CurlSendErrorException(string,string).md 'CurlDotNet\.Exceptions\.CurlSendErrorException\.CurlSendErrorException\(string, string\)') | Creates the send error \(exit code 55\), prefixing `Send error:` to the socket layer's explanation\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlSendErrorException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlSendErrorException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlSendErrorException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlSendErrorException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
