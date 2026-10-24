#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithUserAgent\(string\) Method

Sets the `User-Agent` header that identifies your application \(curl's `-A` /
`--user-agent`\); without it CurlDotNet sends `curl/8.x` like the curl tool\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithUserAgent(string userAgent);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithUserAgent(string).userAgent'></a>

`userAgent` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The full User\-Agent value, such as `"MyApp/1.0"`; null or empty keeps the default\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Many APIs (GitHub, Reddit, Wikipedia) reject or throttle requests without a descriptive
            agent; use a value such as `"MyCompany-Sync/2.1 (+https://mycompany.example/contact)"`.

Example: `CurlRequestBuilder.Get("https://api.github.com/repos/dotnet/runtime").WithUserAgent("MyApp/1.0")`
            is `curl -A 'MyApp/1.0' https://api.github.com/repos/dotnet/runtime`.

### See Also
- [WithHeader\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHeader\(string, string\)')
- [UserAgent](CurlDotNet.Core.CurlOptions.UserAgent.md 'CurlDotNet\.Core\.CurlOptions\.UserAgent')
- [WithUserAgent\(string\)](CurlDotNet.Lib.LibCurl.WithUserAgent(string).md 'CurlDotNet\.Lib\.LibCurl\.WithUserAgent\(string\)')