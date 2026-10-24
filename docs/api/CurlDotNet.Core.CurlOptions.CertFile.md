#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.CertFile Property

Client certificate for mutual TLS, from curl's `-E`/`--cert FILE[:PASSWORD]`: the certificate \(and,
for PEM, usually its private key\) presented to servers that ask for one\.

```csharp
public string? CertFile { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A file path\. PEM \(default\), DER or PKCS\#12 as chosen by [CertType](CurlDotNet.Core.CurlOptions.CertType.md 'CurlDotNet\.Core\.CurlOptions\.CertType') \(a `.p12`/`.pfx`
            file is read as PKCS\#12 without it\)\. Default: null \(no client certificate\)\.

### Remarks

Applied on .NET 5 and later. PEM: the key is read from [KeyFile](CurlDotNet.Core.CurlOptions.KeyFile.md 'CurlDotNet\.Core\.CurlOptions\.KeyFile'), or from the same file when
            [KeyFile](CurlDotNet.Core.CurlOptions.KeyFile.md 'CurlDotNet\.Core\.CurlOptions\.KeyFile') is not set; an encrypted key needs [KeyPassword](CurlDotNet.Core.CurlOptions.KeyPassword.md 'CurlDotNet\.Core\.CurlOptions\.KeyPassword') (`--pass`) or
            [CertPassword](CurlDotNet.Core.CurlOptions.CertPassword.md 'CurlDotNet\.Core\.CurlOptions\.CertPassword'). DER: needs [KeyFile](CurlDotNet.Core.CurlOptions.KeyFile.md 'CurlDotNet\.Core\.CurlOptions\.KeyFile') (a PEM RSA or ECDSA key). P12: the password is
            [CertPassword](CurlDotNet.Core.CurlOptions.CertPassword.md 'CurlDotNet\.Core\.CurlOptions\.CertPassword') (the part after the colon in `--cert file.p12:secret`). A file that cannot be
            read, a wrong password or a key that does not match fails before connecting with
            [CurlSslCertificateProblemException](CurlDotNet.Exceptions.CurlSslCertificateProblemException.md 'CurlDotNet\.Exceptions\.CurlSslCertificateProblemException') (exit 58); a server that rejects the
            certificate fails the handshake with [CurlSslConnectErrorException](CurlDotNet.Exceptions.CurlSslConnectErrorException.md 'CurlDotNet\.Exceptions\.CurlSslConnectErrorException') (exit 35).
            On .NET Standard 2.0 and .NET Framework the option is recorded in [UnsupportedOptions](CurlDotNet.Core.CurlOptions.UnsupportedOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnsupportedOptions') and no
            certificate is sent. It is not applied when you pass your own `HttpClient` to the engine (its handler decides).

Example: `curl --cert client.pem --key client.key https://mtls.example.com/` is
            `new CurlOptions { Url = url, CertFile = "client.pem", KeyFile = "client.key" }`; with PKCS#12,
            `curl --cert client.p12:secret --cert-type P12 URL`.

### See Also
- [KeyFile](CurlDotNet.Core.CurlOptions.KeyFile.md 'CurlDotNet\.Core\.CurlOptions\.KeyFile')
- [CertType](CurlDotNet.Core.CurlOptions.CertType.md 'CurlDotNet\.Core\.CurlOptions\.CertType')
- [CertPassword](CurlDotNet.Core.CurlOptions.CertPassword.md 'CurlDotNet\.Core\.CurlOptions\.CertPassword')
- [CaCertFile](CurlDotNet.Core.CurlOptions.CaCertFile.md 'CurlDotNet\.Core\.CurlOptions\.CaCertFile')