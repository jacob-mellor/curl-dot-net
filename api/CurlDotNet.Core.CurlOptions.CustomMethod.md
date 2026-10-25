#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.CustomMethod Property

The method sent on the request line exactly as written \(any token, such as `PROPFIND` or lower\-case
`purge`\), like curl's `-X`/`--request`; overrides [Method](CurlDotNet.Core.CurlOptions.Method.md 'CurlDotNet\.Core\.CurlOptions\.Method')\.

```csharp
public string? CustomMethod { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A method token\. Default: null \(use [Method](CurlDotNet.Core.CurlOptions.Method.md 'CurlDotNet\.Core\.CurlOptions\.Method')\)\.

### Remarks

Like curl, `-X` only changes the word on the request line: it does not add a body or remove one.
            With [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation'), the custom method is kept on every redirect hop unless
            [FollowObeysMethod](CurlDotNet.Core.CurlOptions.FollowObeysMethod.md 'CurlDotNet\.Core\.CurlOptions\.FollowObeysMethod') is true. Prefer [Method](CurlDotNet.Core.CurlOptions.Method.md 'CurlDotNet\.Core\.CurlOptions\.Method') for the standard verbs; `-X HEAD` is
            better written as [HeadOnly](CurlDotNet.Core.CurlOptions.HeadOnly.md 'CurlDotNet\.Core\.CurlOptions\.HeadOnly').

Example: `curl -X PROPFIND https://dav.example.com/files/` is
            `new CurlOptions { Url = url, CustomMethod = "PROPFIND" }`.

### See Also
- [Method](CurlDotNet.Core.CurlOptions.Method.md 'CurlDotNet\.Core\.CurlOptions\.Method')
- [FollowObeysMethod](CurlDotNet.Core.CurlOptions.FollowObeysMethod.md 'CurlDotNet\.Core\.CurlOptions\.FollowObeysMethod')