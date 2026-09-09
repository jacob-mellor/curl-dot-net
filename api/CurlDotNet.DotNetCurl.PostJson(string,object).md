#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.PostJson\(string, object\) Method

Quick POST with JSON synchronously\.

```csharp
public static CurlDotNet.Core.CurlResult PostJson(string url, object data);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.PostJson(string,object).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

URL to POST to

<a name='CurlDotNet.DotNetCurl.PostJson(string,object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

Object to serialize as JSON

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
Result object