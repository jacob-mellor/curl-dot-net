#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlExecutionException Class

A command failed for a reason that has no more specific type; the inner exception holds the underlying \.NET
error and the message says in words what it was\.

```csharp
public class CurlExecutionException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlExecutionException

### Remarks

Use it in wrappers and middleware that run curl commands and need to report an unexpected failure without
            losing the original exception. It carries no curl exit code. Prefer a specific type
            ([CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException'), [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')...) when one fits.

Example: `catch (Exception ex) when (!(ex is CurlException)) { throw new CurlExecutionException($"Running '{name}' failed.", command, ex); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/troubleshooting/error-reference.md

### See Also
- [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

| Constructors | |
| :--- | :--- |
| [CurlExecutionException\(string, string, Exception\)](CurlDotNet.Exceptions.CurlExecutionException.CurlExecutionException(string,string,System.Exception).md 'CurlDotNet\.Exceptions\.CurlExecutionException\.CurlExecutionException\(string, string, System\.Exception\)') | Creates a general execution failure that wraps the original exception and summarises it in the first line of the message\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlExecutionException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlExecutionException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlExecutionException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlExecutionException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
