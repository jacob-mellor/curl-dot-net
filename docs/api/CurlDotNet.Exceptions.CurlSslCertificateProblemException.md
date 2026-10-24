#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlSslCertificateProblemException Class

Your client certificate or key could not be used \(exit code 58, CURLE\_SSL\_CERTPROBLEM\): wrong path, format or
password for `--cert`/`--key`\.

```csharp
public class CurlSslCertificateProblemException : CurlDotNet.Exceptions.CurlSslException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException') &#129106; CurlSslCertificateProblemException

### Remarks

Fixes: check the file exists, the format (`--cert-type PEM|DER|P12`), the password
            (`--cert file.p12:password`), and that the key matches the certificate.
            Example: `catch (CurlSslCertificateProblemException ex) { Console.WriteLine(ex.CertificateError); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md

### See Also
- [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')

| Constructors | |
| :--- | :--- |
| [CurlSslCertificateProblemException\(string, string\)](CurlDotNet.Exceptions.CurlSslCertificateProblemException.CurlSslCertificateProblemException(string,string).md 'CurlDotNet\.Exceptions\.CurlSslCertificateProblemException\.CurlSslCertificateProblemException\(string, string\)') | Creates the error \(exit code 58\) for an unusable client certificate, with advice on path, type, password and key\. |

| Properties | |
| :--- | :--- |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlSslCertificateProblemException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlSslCertificateProblemException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
