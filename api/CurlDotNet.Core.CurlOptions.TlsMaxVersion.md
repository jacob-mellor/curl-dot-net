#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.TlsMaxVersion Property

The highest TLS version to use, from curl's `--tls-max VERSION`\.

```csharp
public string? TlsMaxVersion { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
`"1.0"`, `"1.1"`, `"1.2"` or `"1.3"`; `--tls-max default` leaves it null\.
            Default: null \(no maximum\)\.

### Remarks

Without [TlsMinVersion](CurlDotNet.Core.CurlOptions.TlsMinVersion.md 'CurlDotNet\.Core\.CurlOptions\.TlsMinVersion') the minimum is TLS 1.2, like curl. A maximum below the minimum fails with
            [CurlSslConnectErrorException](CurlDotNet.Exceptions.CurlSslConnectErrorException.md 'CurlDotNet\.Exceptions\.CurlSslConnectErrorException') (exit 35); another value fails parsing with exit
            code 2. Example: `curl --tls-max 1.2 https://example.com/` makes the handshake use TLS 1.2.

### See Also
- [TlsMinVersion](CurlDotNet.Core.CurlOptions.TlsMinVersion.md 'CurlDotNet\.Core\.CurlOptions\.TlsMinVersion')