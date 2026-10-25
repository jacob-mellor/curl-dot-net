#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.MaxRedirects Property

The largest number of redirects followed before the transfer fails with exit code 47, like curl's
`--max-redirs N`; only used when [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation') is on\.

```csharp
public int MaxRedirects { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
A count of hops\. Default: 50 \(curl's default\)\. `-1` means no limit; `0` refuses every redirect\.

### Remarks

Exceeding it throws [CurlTooManyRedirectsException](CurlDotNet.Exceptions.CurlTooManyRedirectsException.md 'CurlDotNet\.Exceptions\.CurlTooManyRedirectsException') with the number of hops
            followed; a redirect loop usually shows up this way. See
            https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/redirect-errors.md

Example: `curl -L --max-redirs 5 URL` is `new CurlOptions { Url = url, FollowLocation = true, MaxRedirects = 5 }`.

### See Also
- [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation')
- [WithMaxRedirects\(int\)](CurlDotNet.Core.CurlRequestBuilder.WithMaxRedirects(int).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithMaxRedirects\(int\)')