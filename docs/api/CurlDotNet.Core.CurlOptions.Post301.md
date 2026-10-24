#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Post301 Property

Keeps the POST method and its body when a `301 Moved Permanently` redirect is followed, instead of switching
to GET as curl and browsers do; curl's `--post301`\.

```csharp
public bool Post301 { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to keep POST on 301\. Default: false\.

### Remarks

Only matters with [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation'). Example: `curl -L --post301 -d x=1 https://example.com/old` is
            `new CurlOptions { Url = url, Method = "POST", Data = "x=1", FollowLocation = true, Post301 = true }`.

### See Also
- [Post302](CurlDotNet.Core.CurlOptions.Post302.md 'CurlDotNet\.Core\.CurlOptions\.Post302')
- [Post303](CurlDotNet.Core.CurlOptions.Post303.md 'CurlDotNet\.Core\.CurlOptions\.Post303')
- [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation')