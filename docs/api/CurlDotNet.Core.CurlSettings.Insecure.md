#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.Insecure Property

Whether to skip TLS certificate verification, like curl's `-k`/`--insecure`; only for testing
against self\-signed or local certificates\.

```csharp
public System.Nullable<bool> Insecure { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
true to accept any certificate, false to verify, null \(default\) to keep the command's choice or
            [DefaultInsecure](CurlDotNet.Curl.DefaultInsecure.md 'CurlDotNet\.Curl\.DefaultInsecure')\.

### Remarks

Copied to [Insecure](CurlDotNet.Core.CurlOptions.Insecure.md 'CurlDotNet\.Core\.CurlOptions\.Insecure'). With verification on, an untrusted certificate throws
            [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException') (exit code 60); install the CA or use `--cacert` rather
            than turning verification off in production.

Example: `new CurlSettings { Insecure = true }` for `https://localhost:5001` with the ASP.NET dev certificate.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md

### See Also
- [WithInsecure\(bool\)](CurlDotNet.Core.CurlSettings.WithInsecure(bool).md 'CurlDotNet\.Core\.CurlSettings\.WithInsecure\(bool\)')
- [Insecure](CurlDotNet.Core.CurlOptions.Insecure.md 'CurlDotNet\.Core\.CurlOptions\.Insecure')
- [DefaultInsecure](CurlDotNet.Curl.DefaultInsecure.md 'CurlDotNet\.Curl\.DefaultInsecure')