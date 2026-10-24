#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CommandParser](CurlDotNet.Core.CommandParser.md 'CurlDotNet\.Core\.CommandParser')

## CommandParser\.ExpandEnvironmentVariables\(string\) Method

Expands environment variables across different shell formats\.
\- Windows CMD: %VAR%
\- PowerShell: $env:VAR or $VAR
\- Bash/ZSH: $VAR or $\{VAR\}

```csharp
private string ExpandEnvironmentVariables(string command);
```
#### Parameters

<a name='CurlDotNet.Core.CommandParser.ExpandEnvironmentVariables(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')