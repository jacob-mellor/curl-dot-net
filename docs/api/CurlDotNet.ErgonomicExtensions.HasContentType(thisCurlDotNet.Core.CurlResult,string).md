#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[ErgonomicExtensions](CurlDotNet.ErgonomicExtensions.md 'CurlDotNet\.ErgonomicExtensions')

## ErgonomicExtensions\.HasContentType\(this CurlResult, string\) Method

True when the response's `Content-Type` header contains [contentType](CurlDotNet.ErgonomicExtensions.HasContentType(thisCurlDotNet.Core.CurlResult,string).md#CurlDotNet.ErgonomicExtensions.HasContentType(thisCurlDotNet.Core.CurlResult,string).contentType 'CurlDotNet\.ErgonomicExtensions\.HasContentType\(this CurlDotNet\.Core\.CurlResult, string\)\.contentType'), ignoring case \- so
`HasContentType("json")` matches `application/json; charset=utf-8`\.

```csharp
public static bool HasContentType(this CurlDotNet.Core.CurlResult result, string contentType);
```
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.HasContentType(thisCurlDotNet.Core.CurlResult,string).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The response to inspect; must not be null\.

<a name='CurlDotNet.ErgonomicExtensions.HasContentType(thisCurlDotNet.Core.CurlResult,string).contentType'></a>

`contentType` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The text to look for, such as `application/json`, `json` or `text/html`\.

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
true when the header exists and contains the text; otherwise false\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[result](CurlDotNet.ErgonomicExtensions.HasContentType(thisCurlDotNet.Core.CurlResult,string).md#CurlDotNet.ErgonomicExtensions.HasContentType(thisCurlDotNet.Core.CurlResult,string).result 'CurlDotNet\.ErgonomicExtensions\.HasContentType\(this CurlDotNet\.Core\.CurlResult, string\)\.result') is null\.

### Remarks

It is a substring test, not a media-type parse: `"json"` also matches `application/problem+json`.
            No `Content-Type` header returns false.

Example: `if (result.HasContentType("application/json")) data = result.ParseJson<Data>(); else Console.WriteLine(result.Body);`

### See Also
- [GetHeader\(this CurlResult, string\)](CurlDotNet.ErgonomicExtensions.GetHeader(thisCurlDotNet.Core.CurlResult,string).md 'CurlDotNet\.ErgonomicExtensions\.GetHeader\(this CurlDotNet\.Core\.CurlResult, string\)')
- [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers')