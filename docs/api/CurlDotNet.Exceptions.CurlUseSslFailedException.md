#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlUseSslFailedException Class

TLS was required \(`--ssl-reqd`\) but the server does not offer it \(exit code 64, CURLE\_USE\_SSL\_FAILED\), for
example an FTP server without AUTH TLS\.

```csharp
public class CurlUseSslFailedException : CurlDotNet.Exceptions.CurlSslException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException') &#129106; CurlUseSslFailedException

### Remarks

Use a server or port that supports TLS (FTPS), or drop `--ssl-reqd` if a plain connection is acceptable.
            Example: `catch (CurlUseSslFailedException ex) { Console.WriteLine(ex.WhatHappened); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md

### See Also
- [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')

| Constructors | |
| :--- | :--- |
| [CurlUseSslFailedException\(string, string\)](CurlDotNet.Exceptions.CurlUseSslFailedException.CurlUseSslFailedException(string,string).md 'CurlDotNet\.Exceptions\.CurlUseSslFailedException\.CurlUseSslFailedException\(string, string\)') | Creates the error \(exit code 64\) for a server that could not provide the required TLS level, prefixing `Required SSL level failed:`\. |

| Properties | |
| :--- | :--- |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlUseSslFailedException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlUseSslFailedException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
