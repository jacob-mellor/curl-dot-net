#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.KeyPassword Property

The passphrase of an encrypted private key, from curl's `--pass PHRASE`\.

```csharp
public string? KeyPassword { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The passphrase, or null\. Default: null\.

### Remarks

Used for [KeyFile](CurlDotNet.Core.CurlOptions.KeyFile.md 'CurlDotNet\.Core\.CurlOptions\.KeyFile') (or a PEM [CertFile](CurlDotNet.Core.CurlOptions.CertFile.md 'CurlDotNet\.Core\.CurlOptions\.CertFile') holding an encrypted key), and as the
            PKCS#12 password when [CertPassword](CurlDotNet.Core.CurlOptions.CertPassword.md 'CurlDotNet\.Core\.CurlOptions\.CertPassword') is not given. Never printed in messages or logs.
            Example: `curl --cert c.pem --key k.pem --pass phrase URL` parses to `KeyPassword = "phrase"`.

### See Also
- [KeyFile](CurlDotNet.Core.CurlOptions.KeyFile.md 'CurlDotNet\.Core\.CurlOptions\.KeyFile')
- [CertPassword](CurlDotNet.Core.CurlOptions.CertPassword.md 'CurlDotNet\.Core\.CurlOptions\.CertPassword')