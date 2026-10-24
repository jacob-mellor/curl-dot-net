#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.WithFollowRedirects\(bool\) Method

Turns following of 3xx redirects on or off \(curl `-L`\) and returns these settings for chaining\.

```csharp
public CurlDotNet.Core.CurlSettings WithFollowRedirects(bool follow=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithFollowRedirects(bool).follow'></a>

`follow` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

true \(default\) to follow redirects; false to return the 3xx response itself\.

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
This same [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') instance\.

### Remarks

Sets [FollowRedirects](CurlDotNet.Core.CurlSettings.FollowRedirects.md 'CurlDotNet\.Core\.CurlSettings\.FollowRedirects'). Too many hops throw [CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException') (exit code 47).

Example: `new CurlSettings().WithFollowRedirects()` for short links such as `https://bit.ly/...`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-follow-redirects-csharp/README.md

### See Also
- [FollowRedirects](CurlDotNet.Core.CurlSettings.FollowRedirects.md 'CurlDotNet\.Core\.CurlSettings\.FollowRedirects')
- [FollowRedirects\(bool\)](CurlDotNet.Core.CurlRequestBuilder.FollowRedirects(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.FollowRedirects\(bool\)')