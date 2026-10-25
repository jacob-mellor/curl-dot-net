#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlNotSupportedException Class

The command uses a curl feature CurlDotNet does not implement \(exit code 1\); [Feature](CurlDotNet.Exceptions.CurlNotSupportedException.Feature.md 'CurlDotNet\.Exceptions\.CurlNotSupportedException\.Feature') names it so
you can remove it or fall back to the native curl tool\.

```csharp
public class CurlNotSupportedException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlNotSupportedException

### Remarks

For unsupported URL schemes (gopher://, ldap://...) CurlDotNet throws [CurlUnsupportedProtocolException](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException');
            for compiled-out options [CurlNotBuiltInException](CurlDotNet.Exceptions.CurlNotBuiltInException.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException') (4). This type is for other unsupported features.

Example: `catch (CurlNotSupportedException ex) { Console.WriteLine($"{ex.Feature} is not available in CurlDotNet"); }`

What is supported: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/reference/curl-cli-compatibility.md

### See Also
- [CurlNotBuiltInException](CurlDotNet.Exceptions.CurlNotBuiltInException.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException')
- [CurlUnsupportedProtocolException](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException')

| Constructors | |
| :--- | :--- |
| [CurlNotSupportedException\(string, string\)](CurlDotNet.Exceptions.CurlNotSupportedException.CurlNotSupportedException(string,string).md 'CurlDotNet\.Exceptions\.CurlNotSupportedException\.CurlNotSupportedException\(string, string\)') | Creates the "not supported" error \(exit code 1\) for [feature](CurlDotNet.Exceptions.CurlNotSupportedException.CurlNotSupportedException(string,string).md#CurlDotNet.Exceptions.CurlNotSupportedException.CurlNotSupportedException(string,string).feature 'CurlDotNet\.Exceptions\.CurlNotSupportedException\.CurlNotSupportedException\(string, string\)\.feature'), with advice to remove it or use the native curl tool\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlNotSupportedException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlNotSupportedException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlNotSupportedException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlNotSupportedException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [Feature](CurlDotNet.Exceptions.CurlNotSupportedException.Feature.md 'CurlDotNet\.Exceptions\.CurlNotSupportedException\.Feature') | The curl feature or option that is not available, such as `--libcurl` or a specific protocol, so you can remove it from the command\. |
