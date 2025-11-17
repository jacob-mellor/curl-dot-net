#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[ICommandParser](CurlDotNet.Core.ICommandParser.md 'CurlDotNet\.Core\.ICommandParser')

## ICommandParser\.IsValid\(string\) Method

Validate a curl command without fully parsing\.

```csharp
bool IsValid(string command);
```
#### Parameters

<a name='CurlDotNet.Core.ICommandParser.IsValid(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command string

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True if command syntax is valid