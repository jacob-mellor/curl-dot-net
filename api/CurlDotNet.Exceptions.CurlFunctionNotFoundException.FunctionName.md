#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFunctionNotFoundException](CurlDotNet.Exceptions.CurlFunctionNotFoundException.md 'CurlDotNet\.Exceptions\.CurlFunctionNotFoundException')

## CurlFunctionNotFoundException\.FunctionName Property

The function or platform API that could not be found, so you can tell which option depends on it\.

```csharp
public string FunctionName { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The function name as passed to the constructor\.

### Remarks

Search the compatibility list for the option that needs it.

### See Also
- [Feature](CurlDotNet.Exceptions.CurlNotBuiltInException.Feature.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException\.Feature')