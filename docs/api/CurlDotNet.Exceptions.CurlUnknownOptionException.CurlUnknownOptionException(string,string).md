#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlUnknownOptionException](CurlDotNet.Exceptions.CurlUnknownOptionException.md 'CurlDotNet\.Exceptions\.CurlUnknownOptionException')

## CurlUnknownOptionException\(string, string\) Constructor

Initializes a new instance of the [CurlUnknownOptionException](CurlDotNet.Exceptions.CurlUnknownOptionException.md 'CurlDotNet\.Exceptions\.CurlUnknownOptionException') class\.

```csharp
public CurlUnknownOptionException(string optionName, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlUnknownOptionException.CurlUnknownOptionException(string,string).optionName'></a>

`optionName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the unknown option that was specified\.

<a name='CurlDotNet.Exceptions.CurlUnknownOptionException.CurlUnknownOptionException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.