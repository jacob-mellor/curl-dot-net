#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.WithUserAgent\(string\) Method

Sets the `User-Agent` header for every request \(curl's `-A`\); without it CurlDotNet sends
`curl/8.x` like the curl tool\.

```csharp
public CurlDotNet.Lib.LibCurl WithUserAgent(string userAgent);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithUserAgent(string).userAgent'></a>

`userAgent` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The full User\-Agent value, such as `"MyApp/1.0"`\.

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance, for chaining\.

### Remarks

Many APIs (GitHub among them) reject requests without a descriptive agent; use something like
            `"MyCompany-Sync/2.1"`. A request that sets [UserAgent](CurlDotNet.Core.CurlOptions.UserAgent.md 'CurlDotNet\.Core\.CurlOptions\.UserAgent') keeps its own.

Example: `using var github = new LibCurl().WithUserAgent("MyApp/1.0");`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md

### See Also
- [WithUserAgent\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithUserAgent(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithUserAgent\(string\)')
- [UserAgent](CurlDotNet.Core.CurlOptions.UserAgent.md 'CurlDotNet\.Core\.CurlOptions\.UserAgent')