#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlLoginDeniedException Class

The server rejected the login \(exit code 67, CURLE\_LOGIN\_DENIED\), for example an FTP or IMAP user/password that
is wrong\.

```csharp
public class CurlLoginDeniedException : CurlDotNet.Exceptions.CurlAuthenticationException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlAuthenticationException](CurlDotNet.Exceptions.CurlAuthenticationException.md 'CurlDotNet\.Exceptions\.CurlAuthenticationException') &#129106; CurlLoginDeniedException

### Remarks

A [CurlAuthenticationException](CurlDotNet.Exceptions.CurlAuthenticationException.md 'CurlDotNet\.Exceptions\.CurlAuthenticationException'). Check `-u user:password` (quote it when the password has special
            characters) and the account's status. Example: `catch (CurlLoginDeniedException ex) { Console.WriteLine(ex.WhatHappened); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/auth-errors.md

### See Also
- [CurlAuthenticationException](CurlDotNet.Exceptions.CurlAuthenticationException.md 'CurlDotNet\.Exceptions\.CurlAuthenticationException')
- [CurlAuthErrorException](CurlDotNet.Exceptions.CurlAuthErrorException.md 'CurlDotNet\.Exceptions\.CurlAuthErrorException')

| Constructors | |
| :--- | :--- |
| [CurlLoginDeniedException\(string, string\)](CurlDotNet.Exceptions.CurlLoginDeniedException.CurlLoginDeniedException(string,string).md 'CurlDotNet\.Exceptions\.CurlLoginDeniedException\.CurlLoginDeniedException\(string, string\)') | Creates the login\-denied error \(exit code 67\), prefixing `Login denied:` to the server's answer\. |

| Properties | |
| :--- | :--- |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlLoginDeniedException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlLoginDeniedException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
