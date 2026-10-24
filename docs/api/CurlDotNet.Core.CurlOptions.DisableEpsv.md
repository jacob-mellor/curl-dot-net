#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.DisableEpsv Property

curl's `--disable-epsv` \(do not use the FTP EPSV command\); recorded for compatibility but not applied:
the \.NET FTP client decides the passive command itself\.

```csharp
public bool DisableEpsv { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True when given\. Default: false\. Not acted on\.

### Remarks

Example: `curl --disable-epsv ftp://example.com/` parses to `DisableEpsv = true`.

### See Also
- [FtpPassive](CurlDotNet.Core.CurlOptions.FtpPassive.md 'CurlDotNet\.Core\.CurlOptions\.FtpPassive')
- [DisableEprt](CurlDotNet.Core.CurlOptions.DisableEprt.md 'CurlDotNet\.Core\.CurlOptions\.DisableEprt')