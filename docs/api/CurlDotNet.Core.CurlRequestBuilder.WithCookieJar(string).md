#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithCookieJar\(string\) Method

Saves every cookie the server sets to a Netscape\-format file after the transfer \(curl's `-c` /
`--cookie-jar`\), so a later request can send them back with [WithCookie\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithCookie(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithCookie\(string\)')\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithCookieJar(string cookieJarPath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithCookieJar(string).cookieJarPath'></a>

`cookieJarPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path of the cookie file to write, such as `"cookies.txt"`, or `"-"` for the response body\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

The file is written when the request finishes (also after redirects) and overwritten each time;
            `"-"` writes the jar to the end of [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') instead of a file. The folder must
            exist; otherwise the request fails with a [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') naming the path. Session cookies
            are included.

Example, log in then call an API with the session: `await CurlRequestBuilder.Post(loginUrl).WithFormData(creds).WithCookieJar("session.txt").ExecuteAsync();`
            then `await CurlRequestBuilder.Get(apiUrl).WithCookie("session.txt").ExecuteAsync();`.

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/cookies.md

### See Also
- [WithCookie\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithCookie(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithCookie\(string\)')
- [CookieJar](CurlDotNet.Core.CurlOptions.CookieJar.md 'CurlDotNet\.Core\.CurlOptions\.CookieJar')