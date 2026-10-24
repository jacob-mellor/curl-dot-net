#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlUnsupportedProtocolException Class

The URL uses a scheme CurlDotNet does not speak \(exit code 1, CURLE\_UNSUPPORTED\_PROTOCOL\), a redirect points
at such a scheme, or the server answered with an HTTP version curl does not accept\.

```csharp
public class CurlUnsupportedProtocolException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlUnsupportedProtocolException

### Remarks

CurlDotNet handles `http://`, `https://`, `ftp://`, `ftps://` and `file://`. It throws
            this type when the URL's scheme is something else (`gopher://`, `ldap://`, or a typo such as
            `htps://`), when `-L` meets a `Location:` header that points at a non-HTTP scheme, and when a
            server's status line announces an HTTP version curl refuses (HTTP/0.9 without `--http0.9`, `HTTP/1.2`).
            [Protocol](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.Protocol.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException\.Protocol') holds the scheme or version.

Example: `catch (CurlUnsupportedProtocolException ex) { Console.WriteLine($"Cannot fetch {ex.Protocol} URLs"); }`

Supported protocols: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/supported-protocols.md

### See Also
- [CurlMalformedUrlException](CurlDotNet.Exceptions.CurlMalformedUrlException.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException')
- [CurlNotSupportedException](CurlDotNet.Exceptions.CurlNotSupportedException.md 'CurlDotNet\.Exceptions\.CurlNotSupportedException')

| Constructors | |
| :--- | :--- |
| [CurlUnsupportedProtocolException\(string, string\)](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.#ctor.md#CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string) 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException\.CurlUnsupportedProtocolException\(string, string\)') | Creates the error for a URL whose scheme is not supported, with curl's wording `Protocol "gopher" not supported` and advice to fix the scheme\. |
| [CurlUnsupportedProtocolException\(string, string, bool\)](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.#ctor.md#CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string,bool) 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException\.CurlUnsupportedProtocolException\(string, string, bool\)') | Creates the error for a redirect \(`Location:` header\) that points at a scheme curl refuses to follow, such as `file://` or `gopher://`\. |
| [CurlUnsupportedProtocolException\(string, string, string\)](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.#ctor.md#CurlDotNet.Exceptions.CurlUnsupportedProtocolException.CurlUnsupportedProtocolException(string,string,string) 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException\.CurlUnsupportedProtocolException\(string, string, string\)') | Creates the error for a server reply whose status line uses a protocol version curl does not speak \(HTTP/0\.9 without `--http0.9`, `HTTP/1.2`, or a malformed status code\)\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [Protocol](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.Protocol.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException\.Protocol') | The scheme or protocol version that is not supported, such as `gopher`, `htps` or `HTTP/1.2`\. |
