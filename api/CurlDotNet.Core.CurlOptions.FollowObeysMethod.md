#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.FollowObeysMethod Property

Follows redirects and lets 301/302/303 change a custom `-X` method to GET, as the HTTP specification
describes; curl's `--follow`, a stricter alternative to `-L`\.

```csharp
public bool FollowObeysMethod { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to apply the specification's method rules to [CustomMethod](CurlDotNet.Core.CurlOptions.CustomMethod.md 'CurlDotNet\.Core\.CurlOptions\.CustomMethod')\. Default: false\.

### Remarks

The parser also sets [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation'). With plain `-L` a method given with `-X` is used for every hop,
            which can repeat a DELETE on the redirect target.

Example: `curl --follow -X POST -d x=1 URL` is
            `new CurlOptions { Url = url, CustomMethod = "POST", Data = "x=1", FollowLocation = true, FollowObeysMethod = true }`.

### See Also
- [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation')
- [CustomMethod](CurlDotNet.Core.CurlOptions.CustomMethod.md 'CurlDotNet\.Core\.CurlOptions\.CustomMethod')