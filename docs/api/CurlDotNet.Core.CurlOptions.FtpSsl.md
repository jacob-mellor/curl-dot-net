#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.FtpSsl Property

Encrypts an `ftp://` transfer with TLS \(explicit FTPS\), like curl's `--ftp-ssl`/`--ssl`;
`ftps://` URLs are always encrypted\.

```csharp
public bool FtpSsl { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to require TLS\. Default: false\.

### Remarks

Example: `curl --ftp-ssl -u user:pass ftp://example.com/report.csv` is
            `new CurlOptions { Url = "ftp://example.com/report.csv", FtpSsl = true, Credentials = new NetworkCredential("user", "pass") }`.

### See Also
- [FtpPassive](CurlDotNet.Core.CurlOptions.FtpPassive.md 'CurlDotNet\.Core\.CurlOptions\.FtpPassive')