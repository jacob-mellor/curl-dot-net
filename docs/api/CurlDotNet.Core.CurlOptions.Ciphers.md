#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Ciphers Property

The TLS cipher suites to offer, from curl's `--ciphers` and `--tls13-ciphers`, as IANA names
separated by `:`\.

```csharp
public string? Ciphers { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
For example `TLS_AES_256_GCM_SHA384:TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256`\. Default: null \(the system's list\)\.

### Remarks

Like curl, the two versions are kept apart: a list that names only TLS 1.2 suites (`..._WITH_...`) keeps
            the usual TLS 1.3 suites, and a list of only TLS 1.3 suites keeps the usual ECDHE AES-GCM/ChaCha20 TLS 1.2 suites, so a
            server that speaks only the other version still connects.

.NET can only restrict cipher suites where `CipherSuitesPolicy` is supported (Linux with OpenSSL, .NET 5
            and later). Elsewhere, or when a name is not an IANA suite name (OpenSSL-style names such as
            `ECDHE-RSA-AES128-GCM-SHA256` are not translated), the parser leaves this null and records the option in
            [UnsupportedOptions](CurlDotNet.Core.CurlOptions.UnsupportedOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnsupportedOptions'). Setting it by hand on such a platform has no effect.

Example: `curl --tls13-ciphers TLS_AES_256_GCM_SHA384 https://example.com/`.

### See Also
- [TlsMinVersion](CurlDotNet.Core.CurlOptions.TlsMinVersion.md 'CurlDotNet\.Core\.CurlOptions\.TlsMinVersion')