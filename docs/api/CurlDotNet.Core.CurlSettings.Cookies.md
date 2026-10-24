#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.Cookies Property

A \.NET cookie container that supplies the `Cookie` header of every request and receives every
`Set-Cookie` of the responses, so a session carries over from one call to the next\.

```csharp
public System.Net.CookieContainer Cookies { get; set; }
```

#### Property Value
[System\.Net\.CookieContainer](https://learn.microsoft.com/en-us/dotnet/api/system.net.cookiecontainer 'System\.Net\.CookieContainer')  
Null by default \(only the command's `-b`/`-c` cookies are used\); [WithCookies\(CookieContainer\)](CurlDotNet.Core.CurlSettings.WithCookies(System.Net.CookieContainer).md 'CurlDotNet\.Core\.CurlSettings\.WithCookies\(System\.Net\.CookieContainer\)')
            creates one\.

### Remarks

For each request (redirect hops and auth rounds included) the container's cookies for that URL
            ([System\.Net\.CookieContainer\.GetCookieHeader\(System\.Uri\)](https://learn.microsoft.com/en-us/dotnet/api/system.net.cookiecontainer.getcookieheader#system-net-cookiecontainer-getcookieheader(system-uri) 'System\.Net\.CookieContainer\.GetCookieHeader\(System\.Uri\)')) are added to any `Cookie` header the command sends
            (`-b name=value`, `-b FILE`, `-H "Cookie: ..."`), joined with `"; "`. Each `Set-Cookie`
            header of each response is stored with [System\.Net\.CookieContainer\.SetCookies\(System\.Uri,System\.String\)](https://learn.microsoft.com/en-us/dotnet/api/system.net.cookiecontainer.setcookies#system-net-cookiecontainer-setcookies(system-uri-system-string) 'System\.Net\.CookieContainer\.SetCookies\(System\.Uri,System\.String\)'); one that .NET cannot
            parse is skipped, as curl skips cookies it rejects. Pass the same container to the next call to stay logged in.
            [System\.Net\.CookieContainer](https://learn.microsoft.com/en-us/dotnet/api/system.net.cookiecontainer 'System\.Net\.CookieContainer') is thread-safe, so concurrent calls may share it.

Example: `var jar = new CookieContainer(); var s = new CurlSettings().WithCookies(jar);` then
            `await Curl.ExecuteAsync("curl -d user=me https://example.com/login", s);` and
            `await Curl.ExecuteAsync("curl https://example.com/account", s);` sends the session cookie.

Cookbook: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/cookies.md

### See Also
- [WithCookies\(CookieContainer\)](CurlDotNet.Core.CurlSettings.WithCookies(System.Net.CookieContainer).md 'CurlDotNet\.Core\.CurlSettings\.WithCookies\(System\.Net\.CookieContainer\)')
- [CookieJar](CurlDotNet.Core.CurlOptions.CookieJar.md 'CurlDotNet\.Core\.CurlOptions\.CookieJar')