#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlSslEngineNotFoundException Class

The crypto engine named with `--engine` was not found \(exit code 53, CURLE\_SSL\_ENGINE\_NOTFOUND\); \.NET's TLS
stack has no OpenSSL engines\.

```csharp
public class CurlSslEngineNotFoundException : CurlDotNet.Exceptions.CurlSslException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException') &#129106; CurlSslEngineNotFoundException

### Remarks

Remove `--engine`: CurlDotNet uses the operating system's TLS (SChannel, OpenSSL via .NET, or Secure Transport).
            Example: `catch (CurlSslEngineNotFoundException) { /* drop --engine */ }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md

### See Also
- [CurlSslEngineSetFailedException](CurlDotNet.Exceptions.CurlSslEngineSetFailedException.md 'CurlDotNet\.Exceptions\.CurlSslEngineSetFailedException')
- [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')

| Constructors | |
| :--- | :--- |
| [CurlSslEngineNotFoundException\(string, string\)](CurlDotNet.Exceptions.CurlSslEngineNotFoundException.CurlSslEngineNotFoundException(string,string).md 'CurlDotNet\.Exceptions\.CurlSslEngineNotFoundException\.CurlSslEngineNotFoundException\(string, string\)') | Creates the error \(exit code 53\) for an `--engine` that does not exist, with advice to remove the option\. |

| Properties | |
| :--- | :--- |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlSslEngineNotFoundException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlSslEngineNotFoundException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
