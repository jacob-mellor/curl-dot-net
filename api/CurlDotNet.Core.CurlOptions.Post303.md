#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Post303 Property

Keeps the POST method and its body when a `303 See Other` redirect is followed; without it a 303 turns any
method except GET and HEAD into GET\. curl's `--post303`\.

```csharp
public bool Post303 { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to keep POST on 303\. Default: false\.

### Remarks

Only matters with [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation'). Example: `curl -L --post303 -d x=1 URL` is
            `new CurlOptions { Url = url, Method = "POST", Data = "x=1", FollowLocation = true, Post303 = true }`.

### See Also
- [Post301](CurlDotNet.Core.CurlOptions.Post301.md 'CurlDotNet\.Core\.CurlOptions\.Post301')
- [Post302](CurlDotNet.Core.CurlOptions.Post302.md 'CurlDotNet\.Core\.CurlOptions\.Post302')