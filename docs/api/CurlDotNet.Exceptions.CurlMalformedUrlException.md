#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlMalformedUrlException Class

The URL could not be parsed \(exit code 3, CURLE\_URL\_MALFORMAT\): missing scheme or host, a bad port, spaces
or illegal characters, or a redirect `Location` that is not a valid URL\.

```csharp
public class CurlMalformedUrlException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlMalformedUrlException

### Remarks

Thrown before connecting for the command's URL, and while following `-L` redirects for a broken
            `Location` header. [MalformedUrl](CurlDotNet.Exceptions.CurlMalformedUrlException.MalformedUrl.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException\.MalformedUrl') holds the URL as given.

Fixes: include the scheme (`https://`), percent-encode spaces (`%20`), quote the URL when it contains
            `&`, `?` or `[ ]` (or use `-g` to turn off globbing), and keep ports between 1 and 65535.
            Example: `catch (CurlMalformedUrlException ex) { Console.WriteLine($"Bad URL: {ex.MalformedUrl}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md

### See Also
- [CurlUnsupportedProtocolException](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException')
- [CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')

| Constructors | |
| :--- | :--- |
| [CurlMalformedUrlException\(string, string\)](CurlDotNet.Exceptions.CurlMalformedUrlException.#ctor.md#CurlDotNet.Exceptions.CurlMalformedUrlException.CurlMalformedUrlException(string,string) 'CurlDotNet\.Exceptions\.CurlMalformedUrlException\.CurlMalformedUrlException\(string, string\)') | Creates the error for a URL from the command that cannot be parsed, with advice on scheme, encoding and quoting\. |
| [CurlMalformedUrlException\(string, string, string\)](CurlDotNet.Exceptions.CurlMalformedUrlException.#ctor.md#CurlDotNet.Exceptions.CurlMalformedUrlException.CurlMalformedUrlException(string,string,string) 'CurlDotNet\.Exceptions\.CurlMalformedUrlException\.CurlMalformedUrlException\(string, string, string\)') | Creates the error with an explanation of what is wrong with the URL, for URLs that did not come from the command line \(such as a redirect's `Location` header\)\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlMalformedUrlException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlMalformedUrlException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [MalformedUrl](CurlDotNet.Exceptions.CurlMalformedUrlException.MalformedUrl.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException\.MalformedUrl') | The URL that could not be parsed, exactly as it appeared in the command or in the redirect's `Location` header\. |
