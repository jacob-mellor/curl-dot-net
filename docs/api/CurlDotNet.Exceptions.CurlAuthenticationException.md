#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlAuthenticationException Class

The server rejected the login \(curl exit code 67, CURLE\_LOGIN\_DENIED\): the user name or password is wrong,
or the account may not use the requested authentication method\.

```csharp
public class CurlAuthenticationException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlAuthenticationException

Derived  
&#8627; [CurlLoginDeniedException](CurlDotNet.Exceptions.CurlLoginDeniedException.md 'CurlDotNet\.Exceptions\.CurlLoginDeniedException')

### Remarks

For HTTP a wrong password normally comes back as a 401 `CurlResult` (or
            [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException') with `-f`), not as this exception; this type is used for
            protocols with an explicit login step and by [CurlLoginDeniedException](CurlDotNet.Exceptions.CurlLoginDeniedException.md 'CurlDotNet\.Exceptions\.CurlLoginDeniedException'). Challenge negotiation
            failures (Digest, NTLM) are [CurlAuthErrorException](CurlDotNet.Exceptions.CurlAuthErrorException.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException') (94).

Fixes: check `-u user:password` (quote it if the password has special characters), the account's
            permissions, and the scheme (`--basic`, `--digest`, `--ntlm`).
            Example: `catch (CurlAuthenticationException ex) { Console.WriteLine($"Login failed ({ex.AuthMethod ?? "any method"})"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/auth-errors.md
            how-to: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-basic-auth-csharp/README.md

### See Also
- [CurlLoginDeniedException](CurlDotNet.Exceptions.CurlLoginDeniedException.md 'CurlDotNet\.Exceptions\.CurlLoginDeniedException')
- [CurlAuthErrorException](CurlDotNet.Exceptions.CurlAuthErrorException.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException')

| Constructors | |
| :--- | :--- |
| [CurlAuthenticationException\(string, string, string\)](CurlDotNet.Exceptions.CurlAuthenticationException.CurlAuthenticationException(string,string,string).md 'CurlDotNet\.Exceptions\.CurlAuthenticationException\.CurlAuthenticationException\(string, string, string\)') | Creates a login\-denied error \(exit code 67\) naming the authentication method that was refused\. |

| Properties | |
| :--- | :--- |
| [AuthMethod](CurlDotNet.Exceptions.CurlAuthenticationException.AuthMethod.md 'CurlDotNet\.Exceptions\.CurlAuthenticationException\.AuthMethod') | The authentication scheme that was rejected, such as `Basic`, `Digest`, `NTLM` or an FTP `USER/PASS` login, when known\. |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlAuthenticationException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlAuthenticationException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlAuthenticationException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlAuthenticationException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
