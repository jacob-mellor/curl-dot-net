#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.UserAgent Property

The `User-Agent` header sent with the request, like curl's `-A`/`--user-agent`; an empty
string sends no User\-Agent at all\.

```csharp
public string? UserAgent { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The header value, such as `"MyApp/2.1 (+https://example.com)"`\. Default: null, which sends
            [DefaultUserAgent](CurlDotNet.Curl.DefaultUserAgent.md 'CurlDotNet\.Curl\.DefaultUserAgent') \(`curl/8.x`\)\.

### Remarks

Takes precedence over a `User-Agent` entry in [Headers](CurlDotNet.Core.CurlOptions.Headers.md 'CurlDotNet\.Core\.CurlOptions\.Headers'); the parser also copies a
            `-H "User-Agent: ..."` value here. Some APIs (GitHub, for example) reject requests without one.

Example: `curl -A "MyApp/2.1" URL` is `new CurlOptions { Url = url, UserAgent = "MyApp/2.1" }`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md

### See Also
- [DefaultUserAgent](CurlDotNet.Curl.DefaultUserAgent.md 'CurlDotNet\.Curl\.DefaultUserAgent')
- [Headers](CurlDotNet.Core.CurlOptions.Headers.md 'CurlDotNet\.Core\.CurlOptions\.Headers')
- [WithUserAgent\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithUserAgent(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithUserAgent\(string\)')