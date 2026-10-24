#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.JunkSessionCookies Property

Ignores session cookies \(those without an expiry date\) when reading cookie files given with `-b`, as if
starting a new browser session; curl's `-j`/`--junk-session-cookies`\.

```csharp
public bool JunkSessionCookies { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to discard session cookies on load\. Default: false\.

### Remarks

Applies to cookie files named in [Cookie](CurlDotNet.Core.CurlOptions.Cookie.md 'CurlDotNet\.Core\.CurlOptions\.Cookie'), not to a literal `name=value` cookie string.
            Example: `curl -j -b cookies.txt URL` is `new CurlOptions { Url = url, Cookie = "cookies.txt", JunkSessionCookies = true }`.
            Cookbook: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/cookies.md

### See Also
- [Cookie](CurlDotNet.Core.CurlOptions.Cookie.md 'CurlDotNet\.Core\.CurlOptions\.Cookie')
- [CookieJar](CurlDotNet.Core.CurlOptions.CookieJar.md 'CurlDotNet\.Core\.CurlOptions\.CookieJar')