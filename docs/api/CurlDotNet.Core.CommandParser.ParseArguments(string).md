#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CommandParser](CurlDotNet.Core.CommandParser.md 'CurlDotNet\.Core\.CommandParser')

## CommandParser\.ParseArguments\(string\) Method

Parses arguments from command string, handling quotes from all shells\.
Supports:
\- Single quotes: 'text'
\- Double quotes: "text"
\- Escaped quotes: \\" and \\' and "" \(Windows\-style\)

```csharp
private System.Collections.Generic.List<string> ParseArguments(string command);
```
#### Parameters

<a name='CurlDotNet.Core.CommandParser.ParseArguments(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')