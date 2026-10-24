#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CommandParser](CurlDotNet.Core.CommandParser.md 'CurlDotNet\.Core\.CommandParser')

## CommandParser\.CollapseUnquotedWhitespace\(string\) Method

Collapses runs of whitespace to a single space outside quoted regions while
leaving the inside of single\- and double\-quoted strings untouched, so paths
like `"C:\Temp\file with  double  spaces.txt"` and JSON payloads keep
their exact spacing\.

```csharp
private static string CollapseUnquotedWhitespace(string command);
```
#### Parameters

<a name='CurlDotNet.Core.CommandParser.CollapseUnquotedWhitespace(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')