#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.CaCertFile Property

A PEM bundle of CA certificates to trust instead of the operating system's store, from curl's `--cacert FILE`;
the way to call servers whose certificate comes from a private or corporate CA\.

```csharp
public string? CaCertFile { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A path to a PEM file with one or more `-----BEGIN CERTIFICATE-----` blocks \(a single DER certificate
            also works\)\. Default: null \(the system store decides\)\.

### Remarks

Applied on .NET 5 and later: like curl, only the CAs in the bundle (plus [CaPath](CurlDotNet.Core.CurlOptions.CaPath.md 'CurlDotNet\.Core\.CurlOptions\.CaPath')) are trusted,
            the server's intermediates are used to build the chain, the host name must still match, and revocation is not
            checked. An untrusted server fails with [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException')
            (exit 60); a missing or empty bundle fails before connecting with
            [CurlSslCaCertBadFileException](CurlDotNet.Exceptions.CurlSslCaCertBadFileException.md 'CurlDotNet\.Exceptions\.CurlSslCaCertBadFileException') (exit 77). On .NET Standard 2.0 and .NET
            Framework the option is recorded in [UnsupportedOptions](CurlDotNet.Core.CurlOptions.UnsupportedOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnsupportedOptions') and the system store is used: install the CA
            there instead. [Insecure](CurlDotNet.Core.CurlOptions.Insecure.md 'CurlDotNet\.Core\.CurlOptions\.Insecure') skips verification altogether (testing only).

Example: `curl --cacert corp-ca.pem https://intranet.example/` is
            `new CurlOptions { Url = url, CaCertFile = "corp-ca.pem" }`. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md

### See Also
- [CaPath](CurlDotNet.Core.CurlOptions.CaPath.md 'CurlDotNet\.Core\.CurlOptions\.CaPath')
- [PinnedPublicKey](CurlDotNet.Core.CurlOptions.PinnedPublicKey.md 'CurlDotNet\.Core\.CurlOptions\.PinnedPublicKey')
- [Insecure](CurlDotNet.Core.CurlOptions.Insecure.md 'CurlDotNet\.Core\.CurlOptions\.Insecure')
- [CaCert](CurlDotNet.Core.CurlOptions.CaCert.md 'CurlDotNet\.Core\.CurlOptions\.CaCert')