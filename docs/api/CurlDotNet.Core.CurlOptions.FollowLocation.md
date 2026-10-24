#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.FollowLocation Property

Follows `3xx` redirects \(the `Location` header\) until a final response, like curl's
`-L`/`--location`; without it a redirect is returned as the result\.

```csharp
public bool FollowLocation { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to follow redirects\. Default: false, unless [DefaultFollowRedirects](CurlDotNet.Curl.DefaultFollowRedirects.md 'CurlDotNet\.Curl\.DefaultFollowRedirects') is true\.

### Remarks

At most [MaxRedirects](CurlDotNet.Core.CurlOptions.MaxRedirects.md 'CurlDotNet\.Core\.CurlOptions\.MaxRedirects') hops are followed (50 by default), then the transfer fails with
            [CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException') (exit 47). A 301/302/303 after a POST continues
            as GET unless [Post301](CurlDotNet.Core.CurlOptions.Post301.md 'CurlDotNet\.Core\.CurlOptions\.Post301')/[Post302](CurlDotNet.Core.CurlOptions.Post302.md 'CurlDotNet\.Core\.CurlOptions\.Post302')/[Post303](CurlDotNet.Core.CurlOptions.Post303.md 'CurlDotNet\.Core\.CurlOptions\.Post303') are set. Credentials are only
            sent to the original host unless [LocationTrusted](CurlDotNet.Core.CurlOptions.LocationTrusted.md 'CurlDotNet\.Core\.CurlOptions\.LocationTrusted') is true. Only `http` and `https`
            targets are followed.

Example: `curl -L https://bit.ly/abc` is `new CurlOptions { Url = "https://bit.ly/abc", FollowLocation = true }`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-follow-redirects-csharp/README.md

### See Also
- [MaxRedirects](CurlDotNet.Core.CurlOptions.MaxRedirects.md 'CurlDotNet\.Core\.CurlOptions\.MaxRedirects')
- [LocationTrusted](CurlDotNet.Core.CurlOptions.LocationTrusted.md 'CurlDotNet\.Core\.CurlOptions\.LocationTrusted')
- [FollowObeysMethod](CurlDotNet.Core.CurlOptions.FollowObeysMethod.md 'CurlDotNet\.Core\.CurlOptions\.FollowObeysMethod')
- [DefaultFollowRedirects](CurlDotNet.Curl.DefaultFollowRedirects.md 'CurlDotNet\.Curl\.DefaultFollowRedirects')
- [FollowRedirects\(bool\)](CurlDotNet.Core.CurlRequestBuilder.FollowRedirects(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.FollowRedirects\(bool\)')
- [FollowRedirects](CurlDotNet.Core.CurlSettings.FollowRedirects.md 'CurlDotNet\.Core\.CurlSettings\.FollowRedirects')