#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlBadFunctionArgumentException Class

An option value or combination is not valid for this request \(exit code 43, CURLE\_BAD\_FUNCTION\_ARGUMENT\);
[ArgumentName](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.ArgumentName.md 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException\.ArgumentName') names the option, for example `-C/--continue-at` with an upload\.

```csharp
public class CurlBadFunctionArgumentException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlBadFunctionArgumentException

### Remarks

Thrown by the HTTP handler before sending, for example when `-C` is combined with a request body (curl
            cannot resume uploads over HTTP). The message says which options clash and what to use instead.
            Example: `catch (CurlBadFunctionArgumentException ex) { Console.WriteLine($"Remove {ex.ArgumentName}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md

### See Also
- [CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')
- [CurlOptionSyntaxException](CurlDotNet.Exceptions.CurlOptionSyntaxException.md 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException')

| Constructors | |
| :--- | :--- |
| [CurlBadFunctionArgumentException\(string, string\)](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.#ctor.md#CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string) 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException\.CurlBadFunctionArgumentException\(string, string\)') | Creates the error \(exit code 43\) naming the invalid argument, with advice to correct or remove it\. |
| [CurlBadFunctionArgumentException\(string, string, string\)](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.#ctor.md#CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string,string) 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException\.CurlBadFunctionArgumentException\(string, string, string\)') | Creates the error for a combination of options curl refuses, explaining which options clash and what to use instead\. |

| Properties | |
| :--- | :--- |
| [ArgumentName](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.ArgumentName.md 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException\.ArgumentName') | The option or argument that cannot be used this way, such as `-C/--continue-at`\. |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
