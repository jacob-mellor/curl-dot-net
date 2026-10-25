#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlNotBuiltInException Class

The command asks for a curl feature that this build does not include \(exit code 4, CURLE\_NOT\_BUILT\_IN\);
[Feature](CurlDotNet.Exceptions.CurlNotBuiltInException.Feature.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException\.Feature') names it\.

```csharp
public class CurlNotBuiltInException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlNotBuiltInException

### Remarks

curl compiled without a feature reports exit code 4; CurlDotNet uses the same code for curl features it does not
            implement on the current platform. Remove the option, or run the command with the native curl tool.
            Example: `catch (CurlNotBuiltInException ex) { Console.WriteLine($"{ex.Feature} needs native curl"); }`

Compatibility list: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/reference/curl-cli-compatibility.md

### See Also
- [CurlNotSupportedException](CurlDotNet.Exceptions.CurlNotSupportedException.md 'CurlDotNet\.Exceptions\.CurlNotSupportedException')

| Constructors | |
| :--- | :--- |
| [CurlNotBuiltInException\(string, string\)](CurlDotNet.Exceptions.CurlNotBuiltInException.CurlNotBuiltInException(string,string).md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException\.CurlNotBuiltInException\(string, string\)') | Creates the "feature not built in" error \(exit code 4\) for [feature](CurlDotNet.Exceptions.CurlNotBuiltInException.CurlNotBuiltInException(string,string).md#CurlDotNet.Exceptions.CurlNotBuiltInException.CurlNotBuiltInException(string,string).feature 'CurlDotNet\.Exceptions\.CurlNotBuiltInException\.CurlNotBuiltInException\(string, string\)\.feature'), with advice to remove it or use the native curl tool\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlNotBuiltInException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlNotBuiltInException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [Feature](CurlDotNet.Exceptions.CurlNotBuiltInException.Feature.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException\.Feature') | The feature or option that is not available in this build or platform, such as `--http3` on \.NET Standard 2\.0\. |
