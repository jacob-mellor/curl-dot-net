#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.KeyFile Property

Private key file for [CertFile](CurlDotNet.Core.CurlOptions.CertFile.md 'CurlDotNet\.Core\.CurlOptions\.CertFile'), from curl's `--key FILE`; needed when the certificate file does
not contain the key\.

```csharp
public string? KeyFile { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A PEM key file path \(`-----BEGIN PRIVATE KEY-----`, `RSA PRIVATE KEY`, `EC PRIVATE KEY` or
            `ENCRYPTED PRIVATE KEY`\)\. Default: null \(the key is in [CertFile](CurlDotNet.Core.CurlOptions.CertFile.md 'CurlDotNet\.Core\.CurlOptions\.CertFile')\)\.

### Remarks

Applied on .NET 5 and later, together with [CertFile](CurlDotNet.Core.CurlOptions.CertFile.md 'CurlDotNet\.Core\.CurlOptions\.CertFile'). An encrypted key needs
            [KeyPassword](CurlDotNet.Core.CurlOptions.KeyPassword.md 'CurlDotNet\.Core\.CurlOptions\.KeyPassword') (`--pass`).

Example: `curl --cert client.pem --key client.key URL` parses to `KeyFile = "client.key"`.

### See Also
- [CertFile](CurlDotNet.Core.CurlOptions.CertFile.md 'CurlDotNet\.Core\.CurlOptions\.CertFile')
- [KeyPassword](CurlDotNet.Core.CurlOptions.KeyPassword.md 'CurlDotNet\.Core\.CurlOptions\.KeyPassword')