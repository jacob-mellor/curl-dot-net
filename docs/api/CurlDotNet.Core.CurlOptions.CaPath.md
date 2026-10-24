#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.CaPath Property

A folder of CA certificates to trust instead of the operating system's store, from curl's `--capath DIR`\.

```csharp
public string? CaPath { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A folder path\. Every file in it that holds PEM or DER certificates is loaded \(OpenSSL's hashed names such as
            `5ed36f99.0` included\); other files are skipped\. Default: null\.

### Remarks

Combined with [CaCertFile](CurlDotNet.Core.CurlOptions.CaCertFile.md 'CurlDotNet\.Core\.CurlOptions\.CaCertFile') into one trust store, applied on .NET 5 and later exactly like
            [CaCertFile](CurlDotNet.Core.CurlOptions.CaCertFile.md 'CurlDotNet\.Core\.CurlOptions\.CaCertFile'); a folder that does not exist or holds no certificate fails with
            [CurlSslCaCertBadFileException](CurlDotNet.Exceptions.CurlSslCaCertBadFileException.md 'CurlDotNet\.Exceptions\.CurlSslCaCertBadFileException') (exit 77). Recorded in
            [UnsupportedOptions](CurlDotNet.Core.CurlOptions.UnsupportedOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnsupportedOptions') on .NET Standard 2.0 and .NET Framework.

Example: `curl --capath /etc/ssl/private-cas https://intranet.example/`.

### See Also
- [CaCertFile](CurlDotNet.Core.CurlOptions.CaCertFile.md 'CurlDotNet\.Core\.CurlOptions\.CaCertFile')