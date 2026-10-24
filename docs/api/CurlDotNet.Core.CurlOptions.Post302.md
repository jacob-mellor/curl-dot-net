#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Post302 Property

Keeps the POST method and its body when a `302 Found` redirect is followed, instead of switching to GET;
curl's `--post302`\.

```csharp
public bool Post302 { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to keep POST on 302\. Default: false\.

### Remarks

Only matters with [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation'). Example: `curl -L --post302 -d x=1 URL` is
            `new CurlOptions { Url = url, Method = "POST", Data = "x=1", FollowLocation = true, Post302 = true }`.

### See Also
- [Post301](CurlDotNet.Core.CurlOptions.Post301.md 'CurlDotNet\.Core\.CurlOptions\.Post301')
- [Post303](CurlDotNet.Core.CurlOptions.Post303.md 'CurlDotNet\.Core\.CurlOptions\.Post303')