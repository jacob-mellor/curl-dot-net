#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.AutoReferer Property

Sets the `Referer` header automatically to the previous URL when a redirect is followed, like the
`;auto` suffix of curl's `-e`/`--referer`\.

```csharp
public bool AutoReferer { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to send the previous URL as Referer on each redirect hop\. Default: false\.

### Remarks

`-e ";auto"` sends no Referer on the first request and the previous URL on every redirect;
            `-e "https://from.example/;auto"` sends `https://from.example/` first. The parser strips the suffix, so
            [Referer](CurlDotNet.Core.CurlOptions.Referer.md 'CurlDotNet\.Core\.CurlOptions\.Referer') only ever holds the literal value. Needs [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation').

Example: `curl -L -e ";auto" https://example.com/old-link` is
            `new CurlOptions { Url = url, FollowLocation = true, AutoReferer = true }`.

### See Also
- [Referer](CurlDotNet.Core.CurlOptions.Referer.md 'CurlDotNet\.Core\.CurlOptions\.Referer')
- [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation')