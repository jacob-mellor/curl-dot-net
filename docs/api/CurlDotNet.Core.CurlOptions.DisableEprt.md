#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.DisableEprt Property

curl's `--disable-eprt` \(do not use the FTP EPRT command in active mode\); recorded for compatibility but
not applied by the \.NET FTP client\.

```csharp
public bool DisableEprt { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True when given\. Default: false\. Not acted on\.

### Remarks

Example: `curl --disable-eprt ftp://example.com/` parses to `DisableEprt = true`.

### See Also
- [FtpPassive](CurlDotNet.Core.CurlOptions.FtpPassive.md 'CurlDotNet\.Core\.CurlOptions\.FtpPassive')
- [DisableEpsv](CurlDotNet.Core.CurlOptions.DisableEpsv.md 'CurlDotNet\.Core\.CurlOptions\.DisableEpsv')