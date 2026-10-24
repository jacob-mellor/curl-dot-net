#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CommandParser](CurlDotNet.Core.CommandParser.md 'CurlDotNet\.Core\.CommandParser')

## CommandParser\.NormalizeLineContinuations\(string\) Method

Normalizes line continuations across different shells\.
\- Windows CMD: ^ at end of line
\- PowerShell: backtick \` at end of line
\- Bash/ZSH: \\ at end of line

```csharp
private string NormalizeLineContinuations(string command);
```
#### Parameters

<a name='CurlDotNet.Core.CommandParser.NormalizeLineContinuations(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')