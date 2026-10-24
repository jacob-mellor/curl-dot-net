#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlTooLargeException Class

The server sent more response header data than curl accepts \(exit code 100, CURLE\_TOO\_LARGE\): over 300 KB of
headers, a single header line over 100 KB, or too many header fields\.

```csharp
public class CurlTooLargeException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlTooLargeException

### Remarks

Thrown by the transport to protect against hostile or broken servers. There is no client option that raises the
            limit; the server (often a cookie that keeps growing, or a redirect loop adding headers) must be fixed.
            Example: `catch (CurlTooLargeException ex) { Console.WriteLine(ex.WhatHappened); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md

### See Also
- [CurlWeirdServerReplyException](CurlDotNet.Exceptions.CurlWeirdServerReplyException.md 'CurlDotNet\.Exceptions\.CurlWeirdServerReplyException')

| Constructors | |
| :--- | :--- |
| [CurlTooLargeException\(string, string\)](CurlDotNet.Exceptions.CurlTooLargeException.CurlTooLargeException(string,string).md 'CurlDotNet\.Exceptions\.CurlTooLargeException\.CurlTooLargeException\(string, string\)') | Creates the error \(exit code 100\) describing which header limit was exceeded, with advice that the server must be fixed\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlTooLargeException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlTooLargeException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlTooLargeException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlTooLargeException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
