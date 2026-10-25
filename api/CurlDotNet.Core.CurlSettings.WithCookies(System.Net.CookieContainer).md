#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.WithCookies\(CookieContainer\) Method

Uses [container](CurlDotNet.Core.CurlSettings.WithCookies(System.Net.CookieContainer).md#CurlDotNet.Core.CurlSettings.WithCookies(System.Net.CookieContainer).container 'CurlDotNet\.Core\.CurlSettings\.WithCookies\(System\.Net\.CookieContainer\)\.container') \(see [Cookies](CurlDotNet.Core.CurlSettings.Cookies.md 'CurlDotNet\.Core\.CurlSettings\.Cookies')\) to send and collect cookies, and returns these
settings for chaining\.

```csharp
public CurlDotNet.Core.CurlSettings WithCookies(System.Net.CookieContainer container=null);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithCookies(System.Net.CookieContainer).container'></a>

`container` [System\.Net\.CookieContainer](https://learn.microsoft.com/en-us/dotnet/api/system.net.cookiecontainer 'System\.Net\.CookieContainer')

The container to use; null \(default\) creates a new empty [System\.Net\.CookieContainer](https://learn.microsoft.com/en-us/dotnet/api/system.net.cookiecontainer 'System\.Net\.CookieContainer')\.

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
This same [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') instance\.

### Remarks

Keep a reference to the container (or to these settings) and pass it to the next call to keep the session.
            Example: `var jar = new CookieContainer(); settings.WithCookies(jar);`.

Cookbook: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/cookies.md

### See Also
- [Cookies](CurlDotNet.Core.CurlSettings.Cookies.md 'CurlDotNet\.Core\.CurlSettings\.Cookies')
- [CookieJar](CurlDotNet.Core.CurlOptions.CookieJar.md 'CurlDotNet\.Core\.CurlOptions\.CookieJar')