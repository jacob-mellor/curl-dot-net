#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlFailedInitException Class

The transfer could not be set up at all \(exit code 2, CURLE\_FAILED\_INIT\), for example because a required
option value or local resource could not be prepared before connecting\.

```csharp
public class CurlFailedInitException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlFailedInitException

### Remarks

Nothing was sent. The message names what could not be initialised; fix that option or resource and run the
            command again. Example: `catch (CurlFailedInitException ex) { Console.WriteLine(ex.WhatHappened); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/init-errors.md

### See Also
- [CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')

| Constructors | |
| :--- | :--- |
| [CurlFailedInitException\(string, string\)](CurlDotNet.Exceptions.CurlFailedInitException.CurlFailedInitException(string,string).md 'CurlDotNet\.Exceptions\.CurlFailedInitException\.CurlFailedInitException\(string, string\)') | Creates an initialisation failure \(exit code 2\) with a message naming what could not be prepared before the transfer started\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlFailedInitException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlFailedInitException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlFailedInitException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlFailedInitException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
