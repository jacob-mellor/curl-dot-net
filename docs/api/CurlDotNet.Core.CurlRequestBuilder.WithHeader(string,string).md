#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithHeader\(string, string\) Method

Adds a request header \(curl's `-H 'Name: value'`\); calling it again with the same name
\(in any letter case\) replaces the earlier value\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithHeader(string name, string value);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).name'></a>

`name` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Header field name such as `"Accept"` \(no colon, no spaces\)\.

<a name='CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Header value such as `"application/json"`, sent exactly as given\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[name](CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).md#CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).name 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHeader\(string, string\)\.name') is null, empty or whitespace\. Fix: pass a header
            name such as `"Accept"`\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-custom\-headers\-user\-agent\-csharp/README\.md

### Example

```csharp
var result = await CurlRequestBuilder
    .Get("https://api.example.com/protected")
    .WithHeader("X-API-Key", "your-api-key-here")
    .WithHeader("X-Client-Version", "1.0.0")
    .ExecuteAsync();
```

### Remarks

[name](CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).md#CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).name 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHeader\(string, string\)\.name') is the header field name without the colon, for example
            `Accept`, `X-Api-Key` or `Content-Type`; [value](CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).md#CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).value 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHeader\(string, string\)\.value') is the text after
            `": "`, sent unchanged. An empty value sends the header with no value. Headers you set win over
            the defaults (`User-Agent: curl/8.x`, `Accept: */*`).

`Content-Type` is applied to text bodies from [WithData\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithData(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithData\(string\)'); [WithJson\(object\)](CurlDotNet.Core.CurlRequestBuilder.WithJson(object).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithJson\(object\)'),
            [WithFormData\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithFormData(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFormData\(System\.Collections\.Generic\.Dictionary\<string,string\>\)') and multipart bodies set their own.

Example: `CurlRequestBuilder.Get(url).WithHeader("Accept", "application/vnd.github.v3+json").WithHeader("X-Request-Id", Guid.NewGuid().ToString())`
            is `curl -H 'Accept: application/vnd.github.v3+json' -H 'X-Request-Id: ...' URL`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md

### See Also
- [WithHeaders\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithHeaders(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHeaders\(System\.Collections\.Generic\.Dictionary\<string,string\>\)')
- [WithUserAgent\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithUserAgent(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithUserAgent\(string\)')
- [Headers](CurlDotNet.Core.CurlOptions.Headers.md 'CurlDotNet\.Core\.CurlOptions\.Headers')
- [WithHeader\(string, string\)](CurlDotNet.Lib.LibCurl.WithHeader(string,string).md 'CurlDotNet\.Lib\.LibCurl\.WithHeader\(string, string\)')