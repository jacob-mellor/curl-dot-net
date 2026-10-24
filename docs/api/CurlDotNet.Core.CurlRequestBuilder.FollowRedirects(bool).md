#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.FollowRedirects\(bool\) Method

Follows `Location:` redirects \(301, 302, 303, 307, 308\) to the final page, like curl's
`-L` / `--location`; without it you get the redirect response itself\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder FollowRedirects(bool follow=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.FollowRedirects(bool).follow'></a>

`follow` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

True \(the default\) to follow redirects; false to return the 3xx response\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Up to 50 redirects are followed (change with [WithMaxRedirects\(int\)](CurlDotNet.Core.CurlRequestBuilder.WithMaxRedirects(int).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithMaxRedirects\(int\)')); more fail with
            [CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException') (exit code 47). As in curl, a 301/302/303 after a POST is
            followed with GET, and [WithBasicAuth\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithBasicAuth(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBasicAuth\(string, string\)') credentials are not sent to a different host.

Example: `var r = await CurlRequestBuilder.Get("http://github.com").FollowRedirects().ExecuteAsync();`
            ends at `https://github.com/` with status 200, like `curl -L http://github.com`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-follow-redirects-csharp/README.md

### See Also
- [WithMaxRedirects\(int\)](CurlDotNet.Core.CurlRequestBuilder.WithMaxRedirects(int).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithMaxRedirects\(int\)')
- [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation')
- [WithFollowRedirects\(int\)](CurlDotNet.Lib.LibCurl.WithFollowRedirects(int).md 'CurlDotNet\.Lib\.LibCurl\.WithFollowRedirects\(int\)')