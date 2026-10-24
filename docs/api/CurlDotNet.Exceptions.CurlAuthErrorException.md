#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlAuthErrorException Class

An authentication scheme failed while answering the server's \(or proxy's\) challenge \(exit code 94,
CURLE\_AUTH\_ERROR\), for example a malformed Digest or NTLM challenge\.

```csharp
public class CurlAuthErrorException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlAuthErrorException

### Remarks

Raised when Digest, NTLM or Negotiate negotiation (`--digest`, `--ntlm`,
            `--negotiate`, `--anyauth`, or the `--proxy-*` variants) cannot process the
            `WWW-Authenticate`/`Proxy-Authenticate` challenge, for example because the server sent
            a parameter twice. A plain wrong password is not this error: it ends with a normal 401 result.
            [Scheme](CurlDotNet.Exceptions.CurlAuthErrorException.Scheme.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException\.Scheme') and [Proxy](CurlDotNet.Exceptions.CurlAuthErrorException.Proxy.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException\.Proxy') say which negotiation failed.

Example: `catch (CurlAuthErrorException ex) when (ex.Proxy) { Console.WriteLine($"Proxy {ex.Scheme} auth failed"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/auth-errors.md schemes:
            https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/auth-schemes.md

### See Also
- [CurlAuthenticationException](CurlDotNet.Exceptions.CurlAuthenticationException.md 'CurlDotNet\.Exceptions\.CurlAuthenticationException')

| Constructors | |
| :--- | :--- |
| [CurlAuthErrorException\(string, bool, string, string\)](CurlDotNet.Exceptions.CurlAuthErrorException.CurlAuthErrorException(string,bool,string,string).md 'CurlDotNet\.Exceptions\.CurlAuthErrorException\.CurlAuthErrorException\(string, bool, string, string\)') | Creates the authentication\-negotiation error \(exit code 94\), naming the scheme, whether it was the proxy, and what the authentication code reported\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlAuthErrorException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlAuthErrorException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [Proxy](CurlDotNet.Exceptions.CurlAuthErrorException.Proxy.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException\.Proxy') | True when the failing negotiation was with the proxy \(`--proxy-digest`, `--proxy-ntlm`\.\.\.\) rather than with the server\. |
| [Scheme](CurlDotNet.Exceptions.CurlAuthErrorException.Scheme.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException\.Scheme') | The scheme whose negotiation failed: `digest`, `ntlm`, `negotiate` or `anyauth`\. |
