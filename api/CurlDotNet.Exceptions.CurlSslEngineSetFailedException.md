#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlSslEngineSetFailedException Class

The crypto engine could not be set as the default \(exit code 54, CURLE\_SSL\_ENGINE\_SETFAILED\)\.

```csharp
public class CurlSslEngineSetFailedException : CurlDotNet.Exceptions.CurlSslException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException') &#129106; CurlSslEngineSetFailedException

### Remarks

Remove `--engine`; .NET's TLS stack does not use engines. Example:
            `catch (CurlSslEngineSetFailedException ex) { Console.WriteLine(ex.WhatHappened); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md

### See Also
- [CurlSslEngineNotFoundException](CurlDotNet.Exceptions.CurlSslEngineNotFoundException.md 'CurlDotNet\.Exceptions\.CurlSslEngineNotFoundException')

| Constructors | |
| :--- | :--- |
| [CurlSslEngineSetFailedException\(string, string\)](CurlDotNet.Exceptions.CurlSslEngineSetFailedException.CurlSslEngineSetFailedException(string,string).md 'CurlDotNet\.Exceptions\.CurlSslEngineSetFailedException\.CurlSslEngineSetFailedException\(string, string\)') | Creates the error \(exit code 54\) for an engine that could not be made the default, prefixing `Failed to set SSL engine:` to the reason\. |

| Properties | |
| :--- | :--- |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlSslEngineSetFailedException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlSslEngineSetFailedException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
