#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithCookie\(string\) Method

Sends cookies with the request \(curl's `-b` / `--cookie`\): either a cookie string such as
`"session=abc123; theme=dark"` or the path of a cookie file to read\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithCookie(string cookie);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithCookie(string).cookie'></a>

`cookie` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

A cookie string like `"name=value; other=2"`, or the path of a cookie file\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

A value containing `=` is sent as the `Cookie:` header exactly as written (pairs separated
            by `"; "`). A value without `=` is a file name: a Netscape/Mozilla cookie file (the format
            written by [WithCookieJar\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithCookieJar(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithCookieJar\(string\)') and by browsers' export tools) or a file of raw `Set-Cookie`
            lines; matching cookies are sent and the cookie engine is turned on for redirects.

To keep a login session across requests, read and write the same file:
            `.WithCookie("cookies.txt").WithCookieJar("cookies.txt")` is `curl -b cookies.txt -c cookies.txt URL`.

Example: `CurlRequestBuilder.Get("https://example.com/account").WithCookie("session=abc123; lang=en")`
            is `curl -b 'session=abc123; lang=en' https://example.com/account`.

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/cookies.md

### See Also
- [WithCookieJar\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithCookieJar(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithCookieJar\(string\)')
- [Cookie](CurlDotNet.Core.CurlOptions.Cookie.md 'CurlDotNet\.Core\.CurlOptions\.Cookie')