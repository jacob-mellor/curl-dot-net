#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlBadContentEncodingException Class

The response body's encoding could not be decoded \(exit code 61, CURLE\_BAD\_CONTENT\_ENCODING\): unknown or
corrupt gzip/deflate/br data, or a forbidden Transfer\-Encoding\.

```csharp
public class CurlBadContentEncodingException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlBadContentEncodingException

### Remarks

Thrown by the HTTP handler with `--compressed` or `--tr-encoding`, and by the transport for illegal
            framing. Fixes: drop `--compressed` to receive the raw bytes, or have the server fix its Content-Encoding.
            Example: `catch (CurlBadContentEncodingException ex) { Console.WriteLine($"Bad encoding {ex.Encoding}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md

### See Also
- [CurlWeirdServerReplyException](CurlDotNet.Exceptions.CurlWeirdServerReplyException.md 'CurlDotNet\.Exceptions\.CurlWeirdServerReplyException')

| Constructors | |
| :--- | :--- |
| [CurlBadContentEncodingException\(string, string\)](CurlDotNet.Exceptions.CurlBadContentEncodingException.#ctor.md#CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string) 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException\.CurlBadContentEncodingException\(string, string\)') | Creates the error \(exit code 61\) for a body whose Content\-Encoding could not be decoded, with advice to drop `--compressed`\. |
| [CurlBadContentEncodingException\(string, string, string\)](CurlDotNet.Exceptions.CurlBadContentEncodingException.#ctor.md#CurlDotNet.Exceptions.CurlBadContentEncodingException.CurlBadContentEncodingException(string,string,string) 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException\.CurlBadContentEncodingException\(string, string, string\)') | Creates the error explaining why the server's Transfer\-Encoding or Content\-Encoding was refused, for framing that no client option can accept\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlBadContentEncodingException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlBadContentEncodingException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [Encoding](CurlDotNet.Exceptions.CurlBadContentEncodingException.Encoding.md 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException\.Encoding') | The encoding header value that could not be decoded, such as `gzip`, `br` or `chunked, gzip`\. |
