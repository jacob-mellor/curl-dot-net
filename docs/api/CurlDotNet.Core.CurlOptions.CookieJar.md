#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.CookieJar Property

File where every cookie known at the end of the transfer is saved in Netscape format, like curl's
`-c`/`--cookie-jar FILE`; `"-"` appends the jar to [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\.

```csharp
public string? CookieJar { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A file path, or `"-"`\. Default: null \(cookies are not saved\)\.

### Remarks

Setting a jar also turns on the cookie engine, so cookies set by one redirect hop are sent to the next.
            The file is written even when the server set no cookies. A folder that does not exist or a file that cannot be
            written fails with [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') exit code 23. Read it back on the next
            request with [Cookie](CurlDotNet.Core.CurlOptions.Cookie.md 'CurlDotNet\.Core\.CurlOptions\.Cookie').

Example: `curl -c cookies.txt -d "user=ada&pass=x" https://example.com/login` is
            `new CurlOptions { Url = "https://example.com/login", Method = "POST", Data = "user=ada&pass=x", CookieJar = "cookies.txt" }`.
            Cookbook: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/cookies.md

### See Also
- [Cookie](CurlDotNet.Core.CurlOptions.Cookie.md 'CurlDotNet\.Core\.CurlOptions\.Cookie')
- [JunkSessionCookies](CurlDotNet.Core.CurlOptions.JunkSessionCookies.md 'CurlDotNet\.Core\.CurlOptions\.JunkSessionCookies')
- [WithCookieJar\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithCookieJar(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithCookieJar\(string\)')