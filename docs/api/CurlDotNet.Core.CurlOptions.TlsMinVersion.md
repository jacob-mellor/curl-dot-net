#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.TlsMinVersion Property

The lowest TLS version to use, from curl's `-1`/`--tlsv1`, `--tlsv1.0`, `--tlsv1.1`,
`--tlsv1.2` and `--tlsv1.3`\.

```csharp
public string? TlsMinVersion { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
`"1.0"`, `"1.1"`, `"1.2"` or `"1.3"`\. Default: null \(the operating system's default,
            TLS 1\.2 or later on current systems\)\.

### Remarks

With `"1.2"` or `"1.3"` only that version and newer ones are offered (up to [TlsMaxVersion](CurlDotNet.Core.CurlOptions.TlsMaxVersion.md 'CurlDotNet\.Core\.CurlOptions\.TlsMaxVersion'));
            `"1.0"` and `"1.1"` leave the system default in place, which already excludes the old versions on most
            systems. A server that cannot meet the range fails the handshake with
            [CurlSslConnectErrorException](CurlDotNet.Exceptions.CurlSslConnectErrorException.md 'CurlDotNet\.Exceptions\.CurlSslConnectErrorException') (exit 35). Whether TLS 1.3 is available depends on
            the operating system.

Example: `curl --tlsv1.3 https://example.com/` is `new CurlOptions { Url = url, TlsMinVersion = "1.3" }`.

### See Also
- [TlsMaxVersion](CurlDotNet.Core.CurlOptions.TlsMaxVersion.md 'CurlDotNet\.Core\.CurlOptions\.TlsMaxVersion')