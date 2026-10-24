#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.FollowRedirects Property

Whether to follow HTTP 3xx redirects, like curl's `-L`/`--location`; null leaves the
command \(or [DefaultFollowRedirects](CurlDotNet.Curl.DefaultFollowRedirects.md 'CurlDotNet\.Curl\.DefaultFollowRedirects')\) in charge\.

```csharp
public System.Nullable<bool> FollowRedirects { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
true to follow redirects, false to return the 3xx response itself, null \(default\) to keep the command's choice\.

### Remarks

Copied to [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation'). Redirects are limited by `--max-redirs`
            (50 by default); exceeding it throws [CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException') (exit code 47).

Example: `new CurlSettings { FollowRedirects = true }` makes `curl http://github.com` end on the https page.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-follow-redirects-csharp/README.md

### See Also
- [WithFollowRedirects\(bool\)](CurlDotNet.Core.CurlSettings.WithFollowRedirects(bool).md 'CurlDotNet\.Core\.CurlSettings\.WithFollowRedirects\(bool\)')
- [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation')
- [DefaultFollowRedirects](CurlDotNet.Curl.DefaultFollowRedirects.md 'CurlDotNet\.Curl\.DefaultFollowRedirects')