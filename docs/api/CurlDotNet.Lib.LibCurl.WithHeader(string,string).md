#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.WithHeader\(string, string\) Method

Adds a header that every request from this client sends \(curl's `-H 'Name: value'`\), unless the
request sets a header with the same name itself\.

```csharp
public CurlDotNet.Lib.LibCurl WithHeader(string key, string value);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithHeader(string,string).key'></a>

`key` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Header field name such as `"Accept"` \(no colon\); must not be null\.

<a name='CurlDotNet.Lib.LibCurl.WithHeader(string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Header value such as `"application/json"`, sent exactly as given\.

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance, for chaining\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[key](CurlDotNet.Lib.LibCurl.WithHeader(string,string).md#CurlDotNet.Lib.LibCurl.WithHeader(string,string).key 'CurlDotNet\.Lib\.LibCurl\.WithHeader\(string, string\)\.key') is null\. Pass the header name; see
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-custom\-headers\-user\-agent\-csharp/README\.md

### Example

```csharp
curl.WithHeader("Accept", "application/json")
    .WithHeader("X-API-Key", "your-key");
var result = await curl.GetAsync("https://api.example.com/data");
```

### Remarks

Names are matched case-insensitively; calling it again with the same name replaces the default value.
            A request overrides a default with `opts => opts.Headers["Accept"] = "text/csv"` in its
            `configure` callback.

Example: `curl.WithHeader("Accept", "application/json").WithHeader("X-Api-Key", key);` then every
            [GetAsync\(string, Action&lt;CurlOptions&gt;, CancellationToken\)](CurlDotNet.Lib.LibCurl.GetAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.GetAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)') sends both headers.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md

### See Also
- [WithHeader\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHeader\(string, string\)')
- [WithBearerToken\(string\)](CurlDotNet.Lib.LibCurl.WithBearerToken(string).md 'CurlDotNet\.Lib\.LibCurl\.WithBearerToken\(string\)')