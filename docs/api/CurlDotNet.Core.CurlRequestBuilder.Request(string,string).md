#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.Request\(string, string\) Method

Starts a request with any HTTP method, such as `OPTIONS`, `PROPFIND` or `PURGE`
\(curl's `-X METHOD`\); the method name is upper\-cased\.

```csharp
public static CurlDotNet.Core.CurlRequestBuilder Request(string method, string url);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Request(string,string).method'></a>

`method` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The HTTP method token, such as `"OPTIONS"` or `"purge"` \(case does not
            matter; it is sent upper\-case\)\. Must be a non\-empty token without spaces\.

<a name='CurlDotNet.Core.CurlRequestBuilder.Request(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Absolute URL of the request\. Must not be null\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
A new builder with that method\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[method](CurlDotNet.Core.CurlRequestBuilder.Request(string,string).md#CurlDotNet.Core.CurlRequestBuilder.Request(string,string).method 'CurlDotNet\.Core\.CurlRequestBuilder\.Request\(string, string\)\.method') is null, empty or contains whitespace\.
            Pass a method name such as `"OPTIONS"`; see
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-put\-delete\-patch\-csharp/README\.md

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[url](CurlDotNet.Core.CurlRequestBuilder.Request(string,string).md#CurlDotNet.Core.CurlRequestBuilder.Request(string,string).url 'CurlDotNet\.Core\.CurlRequestBuilder\.Request\(string, string\)\.url') is null\. Pass the URL string; see
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/url\-errors\.md

### Remarks

GET, POST, PUT, DELETE, HEAD, OPTIONS and PATCH behave exactly like their dedicated factories.
            Any other name is sent verbatim on the request line (like `curl -X PROPFIND`); a body added with
            [WithData\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithData(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithData\(string\)') or [WithJson\(object\)](CurlDotNet.Core.CurlRequestBuilder.WithJson(object).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithJson\(object\)') is sent with it.

Example: `var r = await CurlRequestBuilder.Request("PROPFIND", "https://dav.example.com/files/").WithHeader("Depth", "1").ExecuteAsync();`

### See Also
- [Get\(string\)](CurlDotNet.Core.CurlRequestBuilder.Get(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Get\(string\)')
- [CustomMethod](CurlDotNet.Core.CurlOptions.CustomMethod.md 'CurlDotNet\.Core\.CurlOptions\.CustomMethod')