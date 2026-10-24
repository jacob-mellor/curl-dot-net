#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlSslCipherException Class

The cipher list given with `--ciphers` could not be used \(exit code 59, CURLE\_SSL\_CIPHER\);
[CipherName](CurlDotNet.Exceptions.CurlSslCipherException.CipherName.md 'CurlDotNet\.Exceptions\.CurlSslCipherException\.CipherName') names it\.

```csharp
public class CurlSslCipherException : CurlDotNet.Exceptions.CurlSslException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException') &#129106; CurlSslCipherException

### Remarks

.NET only lets you choose cipher suites on some platforms (Linux with OpenSSL). Remove `--ciphers` or use
            names this platform knows. Example: `catch (CurlSslCipherException ex) { Console.WriteLine(ex.CipherName); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md

### See Also
- [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')

| Constructors | |
| :--- | :--- |
| [CurlSslCipherException\(string, string\)](CurlDotNet.Exceptions.CurlSslCipherException.CurlSslCipherException(string,string).md 'CurlDotNet\.Exceptions\.CurlSslCipherException\.CurlSslCipherException\(string, string\)') | Creates the error \(exit code 59\) for a cipher that cannot be used, with advice to remove or change `--ciphers`\. |

| Properties | |
| :--- | :--- |
| [CipherName](CurlDotNet.Exceptions.CurlSslCipherException.CipherName.md 'CurlDotNet\.Exceptions\.CurlSslCipherException\.CipherName') | The cipher \(or cipher list\) from `--ciphers` that could not be used, such as `TLS_AES_256_GCM_SHA384`\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlSslCipherException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlSslCipherException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
