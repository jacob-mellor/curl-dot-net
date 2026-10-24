#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlBadFunctionArgumentException](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.md 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException')

## CurlBadFunctionArgumentException\(string, string\) Constructor

Initializes a new instance of the [CurlBadFunctionArgumentException](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.md 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException') class\.

```csharp
public CurlBadFunctionArgumentException(string argumentName, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string).argumentName'></a>

`argumentName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The name of the argument that was invalid\.

<a name='CurlDotNet.Exceptions.CurlBadFunctionArgumentException.CurlBadFunctionArgumentException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception\.