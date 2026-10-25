#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlHttpPostErrorException Class

A multipart/form POST could not be built or sent \(exit code 34, CURLE\_HTTP\_POST\_ERROR\), for example a
`-F` field whose file or syntax is wrong\.

```csharp
public class CurlHttpPostErrorException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlHttpPostErrorException

### Remarks

Fixes: check every `-F name=value` / `-F name=@file;type=mime` field and the files it names.
            Example: `catch (CurlHttpPostErrorException ex) { Console.WriteLine(ex.WhatHappened); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md

### See Also
- [CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException')

| Constructors | |
| :--- | :--- |
| [CurlHttpPostErrorException\(string, string\)](CurlDotNet.Exceptions.CurlHttpPostErrorException.CurlHttpPostErrorException(string,string).md 'CurlDotNet\.Exceptions\.CurlHttpPostErrorException\.CurlHttpPostErrorException\(string, string\)') | Creates the form\-post error \(exit code 34\), prefixing `HTTP POST error:` to the caller's explanation of which field failed\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlHttpPostErrorException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlHttpPostErrorException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlHttpPostErrorException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlHttpPostErrorException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
