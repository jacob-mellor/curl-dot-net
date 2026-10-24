#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.WithFollowRedirects\(int\) Method

Follows `Location:` redirects \(301, 302, 303, 307, 308\) for every request, like curl's `-L`,
up to [maxRedirects](CurlDotNet.Lib.LibCurl.WithFollowRedirects(int).md#CurlDotNet.Lib.LibCurl.WithFollowRedirects(int).maxRedirects 'CurlDotNet\.Lib\.LibCurl\.WithFollowRedirects\(int\)\.maxRedirects') hops; without it you get the 3xx response itself\.

```csharp
public CurlDotNet.Lib.LibCurl WithFollowRedirects(int maxRedirects=50);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithFollowRedirects(int).maxRedirects'></a>

`maxRedirects` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

Maximum number of redirects to follow \(default 50, curl's default\); \-1 for unlimited\.

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance, for chaining\.

### Remarks

More redirects than the limit end the request with [CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException')
            (exit code 47) in [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception'); `-1` means unlimited (curl's
            `--max-redirs -1`). A 301/302/303 after a POST is followed with GET, and credentials are not sent to
            another host, as in curl.

Example: `using var curl = new LibCurl().WithFollowRedirects(5);` then
            `(await curl.GetAsync("http://github.com")).StatusCode` is 200.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-follow-redirects-csharp/README.md

### See Also
- [FollowRedirects\(bool\)](CurlDotNet.Core.CurlRequestBuilder.FollowRedirects(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.FollowRedirects\(bool\)')
- [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation')
- [MaxRedirects](CurlDotNet.Core.CurlOptions.MaxRedirects.md 'CurlDotNet\.Core\.CurlOptions\.MaxRedirects')