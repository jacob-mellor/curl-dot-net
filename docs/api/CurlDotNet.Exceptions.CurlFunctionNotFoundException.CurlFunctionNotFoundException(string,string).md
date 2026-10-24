#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFunctionNotFoundException](CurlDotNet.Exceptions.CurlFunctionNotFoundException.md 'CurlDotNet\.Exceptions\.CurlFunctionNotFoundException')

## CurlFunctionNotFoundException\(string, string\) Constructor

Creates the error \(exit code 41\) for a missing function, with advice to remove the option or change
platform\.

```csharp
public CurlFunctionNotFoundException(string functionName, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlFunctionNotFoundException.CurlFunctionNotFoundException(string,string).functionName'></a>

`functionName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The function name that was not found\.

<a name='CurlDotNet.Exceptions.CurlFunctionNotFoundException.CurlFunctionNotFoundException(string,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command that caused the exception, or null\.

### Remarks

Message: `Function not found: NAME`.

### See Also
- [CurlNotBuiltInException](CurlDotNet.Exceptions.CurlNotBuiltInException.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException')