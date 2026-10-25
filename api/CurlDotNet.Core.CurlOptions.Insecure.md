#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Insecure Property

Skips TLS certificate verification so self\-signed, expired or wrong\-host certificates are accepted,
like curl's `-k`/`--insecure`; use for local testing only\.

```csharp
public bool Insecure { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to accept any server certificate\. Default: false, unless [DefaultInsecure](CurlDotNet.Curl.DefaultInsecure.md 'CurlDotNet\.Curl\.DefaultInsecure') is true\.

### Remarks

Without it an untrusted certificate fails with [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')
            (exit 60). The traffic is still encrypted, but anyone in the middle can read it, so never enable it in
            production; trust the issuing CA in the operating system instead. The `--trace` output notes when
            verification is disabled.

Example: `curl -k https://localhost:5001/health` is
            `new CurlOptions { Url = "https://localhost:5001/health", Insecure = true }`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md

### See Also
- [DefaultInsecure](CurlDotNet.Curl.DefaultInsecure.md 'CurlDotNet\.Curl\.DefaultInsecure')
- [CaCertFile](CurlDotNet.Core.CurlOptions.CaCertFile.md 'CurlDotNet\.Core\.CurlOptions\.CaCertFile')
- [Insecure\(bool\)](CurlDotNet.Core.CurlRequestBuilder.Insecure(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Insecure\(bool\)')
- [Insecure](CurlDotNet.Core.CurlSettings.Insecure.md 'CurlDotNet\.Core\.CurlSettings\.Insecure')