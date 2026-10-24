#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.SerializeJson\(object\) Method

Serializes an object to JSON the way a curl user would type it: apostrophes, quotes and
non\-ASCII characters are kept literal instead of being \\uXXXX\-escaped\.

```csharp
internal static string SerializeJson(object data);
```
#### Parameters

<a name='CurlDotNet.Curl.SerializeJson(object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')