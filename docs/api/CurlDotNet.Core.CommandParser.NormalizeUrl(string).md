#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CommandParser](CurlDotNet.Core.CommandParser.md 'CurlDotNet\.Core\.CommandParser')

## CommandParser\.NormalizeUrl\(string\) Method

Adds the scheme curl would guess for a URL given without one: `ftp.example.com`
becomes `ftp://ftp.example.com`, anything else becomes `http://...`\.
URLs that already carry a scheme are returned unchanged\.

```csharp
public static string NormalizeUrl(string url);
```
#### Parameters

<a name='CurlDotNet.Core.CommandParser.NormalizeUrl(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL as typed on the command line\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A URL with a scheme\.