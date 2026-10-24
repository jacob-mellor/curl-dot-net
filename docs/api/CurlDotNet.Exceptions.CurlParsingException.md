#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlParsingException Class

A response could not be converted to the type you asked for \(for example `ParseJson<T>()` or
`AsJsonDynamic()` on an empty body or an HTML error page\); the parser's exception is the inner exception\.

```csharp
public class CurlParsingException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlParsingException

### Remarks

Thrown by [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')'s JSON helpers. It has no curl exit code because the
            transfer itself worked. [ContentType](CurlDotNet.Exceptions.CurlParsingException.ContentType.md 'CurlDotNet\.Exceptions\.CurlParsingException\.ContentType') is the response's Content-Type and [ExpectedType](CurlDotNet.Exceptions.CurlParsingException.ExpectedType.md 'CurlDotNet\.Exceptions\.CurlParsingException\.ExpectedType')
            the type you asked for.

Fixes: check `result.StatusCode` before parsing (a 401/404/500 page is not your JSON), print
            `result.Body`, send `-H 'Accept: application/json'`, and make the class match the JSON
            (`JsonPropertyName` attributes for different names).
            Example: `catch (CurlParsingException ex) { Console.WriteLine($"Got {ex.ContentType}, wanted {ex.ExpectedType?.Name}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md

### See Also
- [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)')
- [AsJsonDynamic\(\)](CurlDotNet.Core.CurlResult.AsJsonDynamic().md 'CurlDotNet\.Core\.CurlResult\.AsJsonDynamic\(\)')

| Constructors | |
| :--- | :--- |
| [CurlParsingException\(string, string, Type, string, Exception\)](CurlDotNet.Exceptions.CurlParsingException.CurlParsingException(string,string,System.Type,string,System.Exception).md 'CurlDotNet\.Exceptions\.CurlParsingException\.CurlParsingException\(string, string, System\.Type, string, System\.Exception\)') | Creates a parsing error naming the response's Content\-Type and the type you asked for, with the parser's own exception kept as the inner exception\. |

| Properties | |
| :--- | :--- |
| [ContentType](CurlDotNet.Exceptions.CurlParsingException.ContentType.md 'CurlDotNet\.Exceptions\.CurlParsingException\.ContentType') | The Content\-Type of the response that could not be parsed, such as `text/html; charset=utf-8`, which usually explains the failure \(an HTML error page instead of JSON\)\. |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlParsingException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlParsingException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlParsingException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlParsingException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [ExpectedType](CurlDotNet.Exceptions.CurlParsingException.ExpectedType.md 'CurlDotNet\.Exceptions\.CurlParsingException\.ExpectedType') | The \.NET type the body was supposed to become, such as `List<User>`; `object` for `AsJsonDynamic()`\. |
