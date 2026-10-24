#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.CertType Property

The format of [CertFile](CurlDotNet.Core.CurlOptions.CertFile.md 'CurlDotNet\.Core\.CurlOptions\.CertFile'), from curl's `--cert-type`: `PEM`, `DER` or `P12`\.

```csharp
public string? CertType { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
`PEM`, `DER`, `P12` \(also `PKCS12`, `PFX`\), case\-insensitive\. Default: null, meaning
            PEM, or P12 for a file ending in `.p12` or `.pfx`\.

### Remarks

Another value fails with [CurlSslCertificateProblemException](CurlDotNet.Exceptions.CurlSslCertificateProblemException.md 'CurlDotNet\.Exceptions\.CurlSslCertificateProblemException') (exit 58)
            when the transfer starts. Example: `curl --cert id.p12:pw --cert-type P12 URL` parses to `CertType = "P12"`.

### See Also
- [CertFile](CurlDotNet.Core.CurlOptions.CertFile.md 'CurlDotNet\.Core\.CurlOptions\.CertFile')