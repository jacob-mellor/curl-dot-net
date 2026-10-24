#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlOptionSyntaxException](CurlDotNet.Exceptions.CurlOptionSyntaxException.md 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException')

## CurlOptionSyntaxException\(string, string\) Constructor

Initializes a new instance of the [CurlOptionSyntaxException](CurlDotNet.Exceptions.CurlOptionSyntaxException.md 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException') class\.

```csharp
public CurlOptionSyntaxException(string option, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string).option'></a>

`option` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The option that caused the syntax error\.

<a name='CurlDotNet.Exceptions.CurlOptionSyntaxException.CurlOptionSyntaxException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.