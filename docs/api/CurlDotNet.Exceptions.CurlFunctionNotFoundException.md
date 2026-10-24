#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlFunctionNotFoundException Class

A required function \(for example an LDAP or crypto routine\) is not available on this platform \(exit code 41,
CURLE\_FUNCTION\_NOT\_FOUND\); [FunctionName](CurlDotNet.Exceptions.CurlFunctionNotFoundException.FunctionName.md 'CurlDotNet\.Exceptions\.CurlFunctionNotFoundException\.FunctionName') names it\.

```csharp
public class CurlFunctionNotFoundException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFunctionNotFoundException

### Remarks

Usually means a platform API is missing on the current OS or target framework. Remove the option that needs
            it or run on a platform that has it. Example: `catch (CurlFunctionNotFoundException ex) { Console.WriteLine(ex.FunctionName); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/reference/curl-cli-compatibility.md

### See Also
- [CurlNotBuiltInException](CurlDotNet.Exceptions.CurlNotBuiltInException.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException')

| Constructors | |
| :--- | :--- |
| [CurlFunctionNotFoundException\(string, string\)](CurlDotNet.Exceptions.CurlFunctionNotFoundException.CurlFunctionNotFoundException(string,string).md 'CurlDotNet\.Exceptions\.CurlFunctionNotFoundException\.CurlFunctionNotFoundException\(string, string\)') | Creates the error \(exit code 41\) for a missing function, with advice to remove the option or change platform\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlFunctionNotFoundException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlFunctionNotFoundException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlFunctionNotFoundException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlFunctionNotFoundException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [FunctionName](CurlDotNet.Exceptions.CurlFunctionNotFoundException.FunctionName.md 'CurlDotNet\.Exceptions\.CurlFunctionNotFoundException\.FunctionName') | The function or platform API that could not be found, so you can tell which option depends on it\. |
