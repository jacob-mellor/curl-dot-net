#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlRedirectException Class

A redirect chain was longer than allowed \(curl exit code 47, CURLE\_TOO\_MANY\_REDIRECTS\);
[RedirectCount](CurlDotNet.Exceptions.CurlRedirectException.RedirectCount.md 'CurlDotNet\.Exceptions\.CurlRedirectException\.RedirectCount') and [LastUrl](CurlDotNet.Exceptions.CurlRedirectException.LastUrl.md 'CurlDotNet\.Exceptions\.CurlRedirectException\.LastUrl') show how far it got\.

```csharp
public class CurlRedirectException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlRedirectException

### Example

```csharp
try
{
    var result = await Curl.ExecuteAsync("curl -L https://example.com/redirect-loop");
}
catch (CurlTooManyRedirectsException ex)        // what the engine throws (exit code 47)
{
    Console.WriteLine($"Too many redirects: {ex.RedirectCount}");
}
catch (CurlRedirectException ex)                // raised by your own redirect-following code
{
    Console.WriteLine($"Too many redirects: {ex.RedirectCount}, last URL {ex.LastUrl}");
}
```

### Remarks

The transport throws [CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException') for `-L` chains longer than
            `--max-redirs` (50 by default); this type is for application code that follows redirects itself.
            A long chain is usually a loop (A redirects to B, B back to A), often caused by cookies or a login page.

Fixes: inspect the chain with `curl -i -L URL`, keep cookies with `-b cookies.txt -c cookies.txt`,
            or raise `--max-redirs`. Example: `catch (CurlRedirectException ex) { Console.WriteLine($"Stopped after {ex.RedirectCount} hops at {ex.LastUrl}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/redirect-errors.md
            how-to: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-follow-redirects-csharp/README.md

### See Also
- [CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException')

| Constructors | |
| :--- | :--- |
| [CurlRedirectException\(string, int, string, string\)](CurlDotNet.Exceptions.CurlRedirectException.CurlRedirectException(string,int,string,string).md 'CurlDotNet\.Exceptions\.CurlRedirectException\.CurlRedirectException\(string, int, string, string\)') | Creates a too\-many\-redirects error \(exit code 47\) with the number of hops and the last URL, which the advice in the message uses\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlRedirectException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlRedirectException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlRedirectException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlRedirectException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [LastUrl](CurlDotNet.Exceptions.CurlRedirectException.LastUrl.md 'CurlDotNet\.Exceptions\.CurlRedirectException\.LastUrl') | The last URL in the chain before it was stopped, which shows where the loop or the long chain goes\. |
| [RedirectCount](CurlDotNet.Exceptions.CurlRedirectException.RedirectCount.md 'CurlDotNet\.Exceptions\.CurlRedirectException\.RedirectCount') | How many redirects were followed before the limit stopped the chain; a value equal to the limit usually means a redirect loop\. |
