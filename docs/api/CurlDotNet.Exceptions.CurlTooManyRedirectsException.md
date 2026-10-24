#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlTooManyRedirectsException Class

`-L` followed more redirects than `--max-redirs` allows \(exit code 47, CURLE\_TOO\_MANY\_REDIRECTS; the
            default limit is 50\), usually a redirect loop\.

```csharp
public class CurlTooManyRedirectsException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlTooManyRedirectsException

### Remarks

Thrown by the redirect handler with curl's wording `Maximum (50) redirects followed`.
            [RedirectCount](CurlDotNet.Exceptions.CurlTooManyRedirectsException.RedirectCount.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException\.RedirectCount') says how many hops were followed. Loops are often caused by a login page or a cookie the
            client does not keep; use `-b cookies.txt -c cookies.txt`.

Example: `catch (CurlTooManyRedirectsException ex) { Console.WriteLine($"Loop? {ex.RedirectCount} hops"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/redirect-errors.md

### See Also
- [CurlRedirectException](CurlDotNet.Exceptions.CurlRedirectException.md 'CurlDotNet\.Exceptions\.CurlRedirectException')
- [CurlUnsupportedProtocolException](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException')

| Constructors | |
| :--- | :--- |
| [CurlTooManyRedirectsException\(int, int, string\)](CurlDotNet.Exceptions.CurlTooManyRedirectsException.#ctor.md#CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,int,string) 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException\.CurlTooManyRedirectsException\(int, int, string\)') | Creates the error using curl's wording `Maximum (N) redirects followed`, naming both the limit and the number of hops\. |
| [CurlTooManyRedirectsException\(int, string\)](CurlDotNet.Exceptions.CurlTooManyRedirectsException.#ctor.md#CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,string) 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException\.CurlTooManyRedirectsException\(int, string\)') | Creates the error \(exit code 47\) for a redirect chain of [count](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md#CurlDotNet.Exceptions.CurlTooManyRedirectsException.CurlTooManyRedirectsException(int,string).count 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException\.CurlTooManyRedirectsException\(int, string\)\.count') hops, with advice to look for a loop or raise the limit\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlTooManyRedirectsException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlTooManyRedirectsException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [RedirectCount](CurlDotNet.Exceptions.CurlTooManyRedirectsException.RedirectCount.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException\.RedirectCount') | How many redirects were followed before the next one was refused; equal to the limit when the chain loops\. |
