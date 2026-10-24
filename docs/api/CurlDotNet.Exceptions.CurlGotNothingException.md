#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlGotNothingException Class

The server accepted the connection and the request, then closed it without sending a single byte \(exit code 52,
CURLE\_GOT\_NOTHING, "Empty reply from server"\)\.

```csharp
public class CurlGotNothingException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlGotNothingException

### Remarks

Thrown by the transport. Usual causes: the server crashed or rejected the request, plain HTTP sent to a TLS port
            (or the reverse), a request too large for the server, or an idle keep-alive connection closed by a proxy. Retrying
            may help ([IsRetryable\(\)](CurlDotNet.Exceptions.CurlException.IsRetryable().md 'CurlDotNet\.Exceptions\.CurlException\.IsRetryable\(\)') is true).

Example: `catch (CurlGotNothingException ex) { Console.WriteLine(ex.WhatHappened); /* check server logs */ }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md

### See Also
- [CurlReceiveErrorException](CurlDotNet.Exceptions.CurlReceiveErrorException.md 'CurlDotNet\.Exceptions\.CurlReceiveErrorException')
- [CurlWeirdServerReplyException](CurlDotNet.Exceptions.CurlWeirdServerReplyException.md 'CurlDotNet\.Exceptions\.CurlWeirdServerReplyException')

| Constructors | |
| :--- | :--- |
| [CurlGotNothingException\(string\)](CurlDotNet.Exceptions.CurlGotNothingException.#ctor.md#CurlDotNet.Exceptions.CurlGotNothingException.CurlGotNothingException(string) 'CurlDotNet\.Exceptions\.CurlGotNothingException\.CurlGotNothingException\(string\)') | Creates the empty\-reply error \(exit code 52\) with curl's wording `Empty reply from server` and advice on ports, TLS and server logs\. |
| [CurlGotNothingException\(string, string, string\)](CurlDotNet.Exceptions.CurlGotNothingException.#ctor.md#CurlDotNet.Exceptions.CurlGotNothingException.CurlGotNothingException(string,string,string) 'CurlDotNet\.Exceptions\.CurlGotNothingException\.CurlGotNothingException\(string, string, string\)') | Creates the empty\-reply error naming the server that closed the connection without replying, plus any extra context such as the request that was sent\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlGotNothingException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlGotNothingException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlGotNothingException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlGotNothingException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
