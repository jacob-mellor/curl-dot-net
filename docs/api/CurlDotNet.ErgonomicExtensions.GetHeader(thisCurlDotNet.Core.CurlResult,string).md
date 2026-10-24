#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[ErgonomicExtensions](CurlDotNet.ErgonomicExtensions.md 'CurlDotNet\.ErgonomicExtensions')

## ErgonomicExtensions\.GetHeader\(this CurlResult, string\) Method

Returns a response header value by name, ignoring case, or null \- the same as [GetHeader\(string\)](CurlDotNet.Core.CurlResult.GetHeader(string).md 'CurlDotNet\.Core\.CurlResult\.GetHeader\(string\)')\.

```csharp
public static string? GetHeader(this CurlDotNet.Core.CurlResult result, string headerName);
```
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.GetHeader(thisCurlDotNet.Core.CurlResult,string).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The response whose headers to search; must not be null\.

<a name='CurlDotNet.ErgonomicExtensions.GetHeader(thisCurlDotNet.Core.CurlResult,string).headerName'></a>

`headerName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The header name in any case, such as `content-type`\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The header value, or null when absent\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[result](CurlDotNet.ErgonomicExtensions.GetHeader(thisCurlDotNet.Core.CurlResult,string).md#CurlDotNet.ErgonomicExtensions.GetHeader(thisCurlDotNet.Core.CurlResult,string).result 'CurlDotNet\.ErgonomicExtensions\.GetHeader\(this CurlDotNet\.Core\.CurlResult, string\)\.result') is null\.

### Remarks

`result.GetHeader(name)` binds to the instance method; this static form exists for
            `ErgonomicExtensions.GetHeader(result, name)` and method-group use. A null [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers')
            returns null.

Example: `var etag = ErgonomicExtensions.GetHeader(result, "ETag");`

### See Also
- [GetHeader\(string\)](CurlDotNet.Core.CurlResult.GetHeader(string).md 'CurlDotNet\.Core\.CurlResult\.GetHeader\(string\)')
- [HasContentType\(this CurlResult, string\)](CurlDotNet.ErgonomicExtensions.HasContentType(thisCurlDotNet.Core.CurlResult,string).md 'CurlDotNet\.ErgonomicExtensions\.HasContentType\(this CurlDotNet\.Core\.CurlResult, string\)')