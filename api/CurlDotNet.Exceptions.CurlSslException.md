#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlSslException Class

Base type for TLS/SSL failures; this constructor reports exit code 60 \(CURLE\_PEER\_FAILED\_VERIFICATION\):
the server's certificate is not trusted, has expired, or does not match the host name\.

```csharp
public class CurlSslException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlSslException

Derived  
&#8627; [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException')  
&#8627; [CurlSslCaCertBadFileException](CurlDotNet.Exceptions.CurlSslCaCertBadFileException.md 'CurlDotNet\.Exceptions\.CurlSslCaCertBadFileException')  
&#8627; [CurlSslCertificateProblemException](CurlDotNet.Exceptions.CurlSslCertificateProblemException.md 'CurlDotNet\.Exceptions\.CurlSslCertificateProblemException')  
&#8627; [CurlSslCipherException](CurlDotNet.Exceptions.CurlSslCipherException.md 'CurlDotNet\.Exceptions\.CurlSslCipherException')  
&#8627; [CurlSslConnectErrorException](CurlDotNet.Exceptions.CurlSslConnectErrorException.md 'CurlDotNet\.Exceptions\.CurlSslConnectErrorException')  
&#8627; [CurlSslEngineNotFoundException](CurlDotNet.Exceptions.CurlSslEngineNotFoundException.md 'CurlDotNet\.Exceptions\.CurlSslEngineNotFoundException')  
&#8627; [CurlSslEngineSetFailedException](CurlDotNet.Exceptions.CurlSslEngineSetFailedException.md 'CurlDotNet\.Exceptions\.CurlSslEngineSetFailedException')  
&#8627; [CurlSslPinnedPublicKeyMismatchException](CurlDotNet.Exceptions.CurlSslPinnedPublicKeyMismatchException.md 'CurlDotNet\.Exceptions\.CurlSslPinnedPublicKeyMismatchException')  
&#8627; [CurlUseSslFailedException](CurlDotNet.Exceptions.CurlUseSslFailedException.md 'CurlDotNet\.Exceptions\.CurlUseSslFailedException')

### Remarks

Derived types carry curl's specific codes: [CurlSslConnectErrorException](CurlDotNet.Exceptions.CurlSslConnectErrorException.md 'CurlDotNet\.Exceptions\.CurlSslConnectErrorException') (35, handshake failed),
            [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException') (60, certificate rejected), [CurlSslCertificateProblemException](CurlDotNet.Exceptions.CurlSslCertificateProblemException.md 'CurlDotNet\.Exceptions\.CurlSslCertificateProblemException')
            (58, your client certificate), [CurlSslCipherException](CurlDotNet.Exceptions.CurlSslCipherException.md 'CurlDotNet\.Exceptions\.CurlSslCipherException') (59), [CurlUseSslFailedException](CurlDotNet.Exceptions.CurlUseSslFailedException.md 'CurlDotNet\.Exceptions\.CurlUseSslFailedException') (64),
            [CurlSslEngineNotFoundException](CurlDotNet.Exceptions.CurlSslEngineNotFoundException.md 'CurlDotNet\.Exceptions\.CurlSslEngineNotFoundException') (53) and [CurlSslEngineSetFailedException](CurlDotNet.Exceptions.CurlSslEngineSetFailedException.md 'CurlDotNet\.Exceptions\.CurlSslEngineSetFailedException') (54).
            Catch this type to handle them all.

Fixes for an untrusted certificate: install the issuing CA on the machine or pass it with
            `--cacert ca.pem`, fix an expired or mismatched certificate on the server, or (for local testing only) use
            `-k/--insecure`. Example: `catch (CurlSslException ex) { Console.WriteLine(ex.CertificateError ?? ex.WhatHappened); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md
            how-to: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md

### See Also
- [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException')
- [CurlSslConnectErrorException](CurlDotNet.Exceptions.CurlSslConnectErrorException.md 'CurlDotNet\.Exceptions\.CurlSslConnectErrorException')

| Constructors | |
| :--- | :--- |
| [CurlSslException\(string, string, string\)](CurlDotNet.Exceptions.CurlSslException.#ctor.md#CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string) 'CurlDotNet\.Exceptions\.CurlSslException\.CurlSslException\(string, string, string\)') | Creates a certificate\-verification failure \(exit code 60\) for a server certificate this machine does not trust, with the TLS layer's detail\. |
| [CurlSslException\(string, string, string, int\)](CurlDotNet.Exceptions.CurlSslException.#ctor.md#CurlDotNet.Exceptions.CurlSslException.CurlSslException(string,string,string,int) 'CurlDotNet\.Exceptions\.CurlSslException\.CurlSslException\(string, string, string, int\)') | Creates a TLS failure with a specific curl exit code, such as 35 \(CURLE\_SSL\_CONNECT\_ERROR\) for a handshake that failed for a reason other than certificate verification\. |

| Properties | |
| :--- | :--- |
| [CertificateError](CurlDotNet.Exceptions.CurlSslException.CertificateError.md 'CurlDotNet\.Exceptions\.CurlSslException\.CertificateError') | The TLS layer's own description of the certificate problem, such as `RemoteCertificateNameMismatch` or `UntrustedRoot`, when it was reported\. |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlSslException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlSslException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlSslException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlSslException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
