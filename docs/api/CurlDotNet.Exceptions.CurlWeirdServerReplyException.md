#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlWeirdServerReplyException Class

The server's reply broke the protocol's rules \(exit code 8, CURLE\_WEIRD\_SERVER\_REPLY\): an invalid status line,
malformed or smuggling\-prone headers, or bytes that are not HTTP at all\.

```csharp
public class CurlWeirdServerReplyException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlWeirdServerReplyException

### Remarks

Thrown while reading the response. Common causes: talking HTTP to a non-HTTP port (a database, SSH, or TLS on
            a plain-http URL), a buggy server or proxy, or header combinations curl rejects to prevent response smuggling
            (duplicate `Content-Length`, NUL bytes, bad chunk trailers). There is no client option that accepts such a
            reply; fix the server or the URL.

Example: `catch (CurlWeirdServerReplyException ex) { Console.WriteLine("Not an HTTP server? " + ex.WhatHappened); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md

### See Also
- [CurlUnsupportedProtocolException](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException')
- [CurlReceiveErrorException](CurlDotNet.Exceptions.CurlReceiveErrorException.md 'CurlDotNet\.Exceptions\.CurlReceiveErrorException')

| Constructors | |
| :--- | :--- |
| [CurlWeirdServerReplyException\(string, string\)](CurlDotNet.Exceptions.CurlWeirdServerReplyException.CurlWeirdServerReplyException(string,string).md 'CurlDotNet\.Exceptions\.CurlWeirdServerReplyException\.CurlWeirdServerReplyException\(string, string\)') | Creates the error with a description of what was wrong with the reply, such as "the status line is not HTTP/1\.x"\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlWeirdServerReplyException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlWeirdServerReplyException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlWeirdServerReplyException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlWeirdServerReplyException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
