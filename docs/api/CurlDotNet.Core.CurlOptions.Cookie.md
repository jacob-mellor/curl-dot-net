#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Cookie Property

Cookies to send, like curl's `-b`/`--cookie`: a cookie string such as `name=value; other=2`,
or \(when there is no `=`\) the path of a Netscape cookie file to read\.

```csharp
public string? Cookie { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A `Cookie` header value, or a file path\. Default: null\.

### Remarks

A value containing `=` is sent as the `Cookie` header exactly as given. A value without `=`
            names a cookie file (Netscape format, as written by [CookieJar](CurlDotNet.Core.CurlOptions.CookieJar.md 'CurlDotNet\.Core\.CurlOptions\.CookieJar'), or a file of `Set-Cookie:` lines);
            its cookies are sent to matching hosts and paths, and a missing file is ignored like in curl. Several
            `-b name=value` options are joined with `"; "`. Combine with [CookieJar](CurlDotNet.Core.CurlOptions.CookieJar.md 'CurlDotNet\.Core\.CurlOptions\.CookieJar') to keep a session.

Example: `curl -b "session=abc123; theme=dark" URL` is `new CurlOptions { Url = url, Cookie = "session=abc123; theme=dark" }`,
            and `curl -b cookies.txt -c cookies.txt URL` is `Cookie = "cookies.txt", CookieJar = "cookies.txt"`.
            Cookbook: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/cookies.md

### See Also
- [CookieJar](CurlDotNet.Core.CurlOptions.CookieJar.md 'CurlDotNet\.Core\.CurlOptions\.CookieJar')
- [JunkSessionCookies](CurlDotNet.Core.CurlOptions.JunkSessionCookies.md 'CurlDotNet\.Core\.CurlOptions\.JunkSessionCookies')
- [WithCookie\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithCookie(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithCookie\(string\)')