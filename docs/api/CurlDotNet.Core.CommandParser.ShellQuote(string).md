#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CommandParser](CurlDotNet.Core.CommandParser.md 'CurlDotNet\.Core\.CommandParser')

## CommandParser\.ShellQuote\(string\) Method

Wraps a value in single quotes so it survives a round\-trip through [CurlDotNet\.Core\.CommandParser\.Parse\(System\.String\)](https://learn.microsoft.com/en-us/dotnet/api/curldotnet.core.commandparser.parse#curldotnet-core-commandparser-parse(system-string) 'CurlDotNet\.Core\.CommandParser\.Parse\(System\.String\)')
\(and through a real POSIX shell\) unchanged, even when it contains spaces, double quotes,
backslashes or single quotes\. Embedded single quotes use the standard `'\''` idiom\.

```csharp
public static string ShellQuote(string? value);
```
#### Parameters

<a name='CurlDotNet.Core.CommandParser.ShellQuote(string).value'></a>

`value` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Raw value such as a JSON body, header or URL\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The single\-quoted, shell\-safe representation; `''` for null or empty input\.

### Example

```csharp
var body = "{\"name\":\"O'Brien\"}";
var cmd = $"curl -d {CommandParser.ShellQuote(body)} https://api.example.com/users";
// curl -d '{"name":"O'\''Brien"}' https://api.example.com/users
```