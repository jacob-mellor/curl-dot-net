#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.FtpPassive Property

Uses passive mode for FTP data connections \(the client connects to the server\), which works through most
firewalls and NAT; curl's `--ftp-pasv`, on by default\.

```csharp
public bool FtpPassive { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True for passive mode\. Default: true\. Set false for active mode\.

### Remarks

Only affects `ftp://` and `ftps://` URLs. Example: `curl --ftp-pasv ftp://example.com/file.txt`
            is `new CurlOptions { Url = "ftp://example.com/file.txt", FtpPassive = true }`.

### See Also
- [FtpSsl](CurlDotNet.Core.CurlOptions.FtpSsl.md 'CurlDotNet\.Core\.CurlOptions\.FtpSsl')