#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CommandParser](CurlDotNet.Core.CommandParser.md 'CurlDotNet\.Core\.CommandParser')

## CommandParser\.PercentEncode\(string\) Method

Percent\-encodes a string the way curl's `curl_easy_escape` does: UTF\-8 bytes, with only
ASCII letters, digits and `- . _ ~` left as\-is\. Spaces become `%20`, never `+`\.

```csharp
public static string PercentEncode(string value);
```
#### Parameters

<a name='CurlDotNet.Core.CommandParser.PercentEncode(string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The text to encode\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The percent\-encoded text\.