#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlUnknownOptionException](CurlDotNet.Exceptions.CurlUnknownOptionException.md 'CurlDotNet\.Exceptions\.CurlUnknownOptionException')

## CurlUnknownOptionException\(string, string\) Constructor

Creates the error for an unknown option, with curl's wording `option NAME: is unknown` and advice to check the
spelling\.

```csharp
public CurlUnknownOptionException(string optionName, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlUnknownOptionException.CurlUnknownOptionException(string,string).optionName'></a>

`optionName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the unknown option that was specified, with its dashes\.

<a name='CurlDotNet.Exceptions.CurlUnknownOptionException.CurlUnknownOptionException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Example: `throw new CurlUnknownOptionException("--headers", command);`

### See Also
- [CurlOptionSyntaxException](CurlDotNet.Exceptions.CurlOptionSyntaxException.md 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException')