#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.DefaultInsecure Property

When true, TLS certificates are not verified for any request, as if `-k`/`--insecure` were in every
command\. Development and testing only: it accepts self\-signed, expired and wrong\-host certificates\.

```csharp
public static bool DefaultInsecure { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
true to skip certificate verification \(dangerous\); false \(default\) to verify\.

### Remarks

Turning certificate checks off makes every https:// request open to man-in-the-middle attacks. Prefer
            trusting the specific certificate with `--cacert ca.pem`, or add `-k` only to the commands that talk
            to a local development server. While this is true it cannot be switched off for a single command.

Without it, a certificate problem throws [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException') (exit code 60)
            or another [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException').

Process-wide and thread-safe; read at the start of every transfer.

Example: `#if DEBUG Curl.DefaultInsecure = true; #endif` lets
            `await Curl.ExecuteAsync("curl https://localhost:5001/api")` reach a dev server with a self-signed
            certificate, while release builds keep checking.

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md

### See Also
- [Insecure](CurlDotNet.Core.CurlOptions.Insecure.md 'CurlDotNet\.Core\.CurlOptions\.Insecure')
- [WithInsecure\(bool\)](CurlDotNet.Core.CurlSettings.WithInsecure(bool).md 'CurlDotNet\.Core\.CurlSettings\.WithInsecure\(bool\)')
- [Insecure\(bool\)](CurlDotNet.Core.CurlRequestBuilder.Insecure(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Insecure\(bool\)')
- [DefaultInsecure](CurlDotNet.DotNetCurl.DefaultInsecure.md 'CurlDotNet\.DotNetCurl\.DefaultInsecure')