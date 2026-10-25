#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlSslConnectErrorException Class

The TLS handshake failed for a reason other than an untrusted certificate \(exit code 35,
CURLE\_SSL\_CONNECT\_ERROR\): protocol or cipher mismatch, a non\-TLS port, or a rejected client certificate\.

```csharp
public class CurlSslConnectErrorException : CurlDotNet.Exceptions.CurlSslException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException') &#129106; CurlSslConnectErrorException

### Remarks

Thrown by the transport when `SslStream` authentication fails. Fixes: check the port speaks TLS (https on 443,
            not http on 80), that the server supports TLS 1.2 or later, any `--cert`/`--key` client certificate, and
            corporate TLS inspection. Example: `catch (CurlSslConnectErrorException ex) { Console.WriteLine(ex.WhatHappened); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md

### See Also
- [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException')
- [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')

| Constructors | |
| :--- | :--- |
| [CurlSslConnectErrorException\(string, string\)](CurlDotNet.Exceptions.CurlSslConnectErrorException.CurlSslConnectErrorException(string,string).md 'CurlDotNet\.Exceptions\.CurlSslConnectErrorException\.CurlSslConnectErrorException\(string, string\)') | Creates the TLS handshake error \(exit code 35\), prefixing `SSL connect error:` to the TLS layer's explanation\. |

| Properties | |
| :--- | :--- |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlSslConnectErrorException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlSslConnectErrorException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
