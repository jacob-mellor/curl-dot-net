#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlPeerFailedVerificationException Class

The server's certificate was rejected \(exit code 60, CURLE\_PEER\_FAILED\_VERIFICATION\): untrusted issuer \(for
example self\-signed\), expired, or issued for a different host name\.

```csharp
public class CurlPeerFailedVerificationException : CurlDotNet.Exceptions.CurlSslException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException') &#129106; CurlPeerFailedVerificationException

### Remarks

Thrown by the transport when certificate validation fails. Fixes: install the issuing CA or pass it with
            `--cacert ca.pem`, use the host name the certificate was issued for, renew an expired certificate, or for local
            testing only use `-k/--insecure`. Corporate TLS inspection proxies need their root CA installed.

Example: `catch (CurlPeerFailedVerificationException ex) { Console.WriteLine(ex.WhatHappened); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md how-to:
            https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md

### See Also
- [CurlSslConnectErrorException](CurlDotNet.Exceptions.CurlSslConnectErrorException.md 'CurlDotNet\.Exceptions\.CurlSslConnectErrorException')
- [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')

| Constructors | |
| :--- | :--- |
| [CurlPeerFailedVerificationException\(string, string\)](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.CurlPeerFailedVerificationException(string,string).md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException\.CurlPeerFailedVerificationException\(string, string\)') | Creates the certificate\-rejected error \(exit code 60\), prefixing `Peer certificate verification failed:` to the TLS layer's reason\. |

| Properties | |
| :--- | :--- |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
