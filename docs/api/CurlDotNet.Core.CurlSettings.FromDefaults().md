#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.FromDefaults\(\) Method

Creates settings pre\-filled from the global `Curl.Default*` properties \(timeouts, redirects,
certificate checking, flavor\), so you can start from the process\-wide defaults and tweak one value\.

```csharp
public static CurlDotNet.Core.CurlSettings FromDefaults();
```

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
A new [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') whose [MaxTimeSeconds](CurlDotNet.Core.CurlSettings.MaxTimeSeconds.md 'CurlDotNet\.Core\.CurlSettings\.MaxTimeSeconds'), [ConnectTimeoutSeconds](CurlDotNet.Core.CurlSettings.ConnectTimeoutSeconds.md 'CurlDotNet\.Core\.CurlSettings\.ConnectTimeoutSeconds'),
            [FollowRedirects](CurlDotNet.Core.CurlSettings.FollowRedirects.md 'CurlDotNet\.Core\.CurlSettings\.FollowRedirects'), [Insecure](CurlDotNet.Core.CurlSettings.Insecure.md 'CurlDotNet\.Core\.CurlSettings\.Insecure') and [Flavor](CurlDotNet.Core.CurlSettings.Flavor.md 'CurlDotNet\.Core\.CurlSettings\.Flavor') mirror the globals; everything else
            has its default value\.

### Remarks

Copies [DefaultMaxTimeSeconds](CurlDotNet.Curl.DefaultMaxTimeSeconds.md 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds') and [DefaultConnectTimeoutSeconds](CurlDotNet.Curl.DefaultConnectTimeoutSeconds.md 'CurlDotNet\.Curl\.DefaultConnectTimeoutSeconds')
            (0 becomes null, meaning no limit), [DefaultFollowRedirects](CurlDotNet.Curl.DefaultFollowRedirects.md 'CurlDotNet\.Curl\.DefaultFollowRedirects'), [DefaultInsecure](CurlDotNet.Curl.DefaultInsecure.md 'CurlDotNet\.Curl\.DefaultInsecure')
            and [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor'). It is a snapshot: later changes to the globals do not affect it.

Example: `var settings = CurlSettings.FromDefaults().WithTimeout(5);` keeps every global default
            except the time limit.

### See Also
- [DefaultMaxTimeSeconds](CurlDotNet.Curl.DefaultMaxTimeSeconds.md 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds')
- [DefaultFollowRedirects](CurlDotNet.Curl.DefaultFollowRedirects.md 'CurlDotNet\.Curl\.DefaultFollowRedirects')