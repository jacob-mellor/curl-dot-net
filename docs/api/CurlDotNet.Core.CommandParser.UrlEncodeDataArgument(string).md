#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CommandParser](CurlDotNet.Core.CommandParser.md 'CurlDotNet\.Core\.CommandParser')

## CommandParser\.UrlEncodeDataArgument\(string\) Method

Applies curl's `--data-urlencode` rules to one argument:
`content`, `=content`, `name=content`, `@file` and `name@file`\.
Only the content part is percent\-encoded; the name and the `=` separator are kept\.

```csharp
private static string UrlEncodeDataArgument(string value);
```
#### Parameters

<a name='CurlDotNet.Core.CommandParser.UrlEncodeDataArgument(string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The raw argument following `--data-urlencode`\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The encoded `name=value` pair \(or bare encoded content\)\.

#### Exceptions

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
Thrown with exit code 26 when a referenced file cannot be read\.