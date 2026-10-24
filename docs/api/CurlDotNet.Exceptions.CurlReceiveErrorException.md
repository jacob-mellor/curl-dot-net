#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlReceiveErrorException Class

Receiving the response failed part\-way \(exit code 56, CURLE\_RECV\_ERROR\): the connection was reset, a proxy
dropped it, or the response framing was broken \(bad chunked encoding, oversized headers\)\.

```csharp
public class CurlReceiveErrorException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlReceiveErrorException

### Remarks

Thrown by the transport and the HTTP handler. Transient causes are worth a retry (`--retry 3`); a reply that
            is consistently broken needs a server fix. Inspect it with `-v` or `--trace-ascii -`.
            Example: `catch (CurlReceiveErrorException ex) { Console.WriteLine(ex.WhatHappened); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md

### See Also
- [CurlSendErrorException](CurlDotNet.Exceptions.CurlSendErrorException.md 'CurlDotNet\.Exceptions\.CurlSendErrorException')
- [CurlPartialFileException](CurlDotNet.Exceptions.CurlPartialFileException.md 'CurlDotNet\.Exceptions\.CurlPartialFileException')

| Constructors | |
| :--- | :--- |
| [CurlReceiveErrorException\(string, string\)](CurlDotNet.Exceptions.CurlReceiveErrorException.#ctor.md#CurlDotNet.Exceptions.CurlReceiveErrorException.CurlReceiveErrorException(string,string) 'CurlDotNet\.Exceptions\.CurlReceiveErrorException\.CurlReceiveErrorException\(string, string\)') | Creates the receive error \(exit code 56\), prefixing `Receive error:` to the explanation of what broke\. |
| [CurlReceiveErrorException\(string, string, Exception\)](CurlDotNet.Exceptions.CurlReceiveErrorException.#ctor.md#CurlDotNet.Exceptions.CurlReceiveErrorException.CurlReceiveErrorException(string,string,System.Exception) 'CurlDotNet\.Exceptions\.CurlReceiveErrorException\.CurlReceiveErrorException\(string, string, System\.Exception\)') | Creates the receive error \(exit code 56\) and keeps the socket, I/O or [System\.Net\.Http\.HttpRequestException](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httprequestexception 'System\.Net\.Http\.HttpRequestException') that broke the transfer as [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException')\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlReceiveErrorException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlReceiveErrorException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlReceiveErrorException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlReceiveErrorException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
