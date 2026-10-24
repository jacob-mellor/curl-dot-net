#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.BuildQuickOptions\(string, string, string, string, string\) Method

Builds the [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') for the quick helper methods directly, bypassing the
command parser so user data never has to be shell\-quoted\. [OriginalCommand](CurlDotNet.Core.CurlOptions.OriginalCommand.md 'CurlDotNet\.Core\.CurlOptions\.OriginalCommand')
is still populated with an equivalent, correctly quoted command for diagnostics\.

```csharp
private static CurlDotNet.Core.CurlOptions BuildQuickOptions(string url, string? method, string? data, string? contentType, string? outputFile);
```
#### Parameters

<a name='CurlDotNet.Curl.BuildQuickOptions(string,string,string,string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Curl.BuildQuickOptions(string,string,string,string,string).method'></a>

`method` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Curl.BuildQuickOptions(string,string,string,string,string).data'></a>

`data` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Curl.BuildQuickOptions(string,string,string,string,string).contentType'></a>

`contentType` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Curl.BuildQuickOptions(string,string,string,string,string).outputFile'></a>

`outputFile` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')