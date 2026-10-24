#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlBadFunctionArgumentException](CurlDotNet.Exceptions.CurlBadFunctionArgumentException.md 'CurlDotNet\.Exceptions\.CurlBadFunctionArgumentException')

## CurlBadFunctionArgumentException\.ArgumentName Property

The option or argument that cannot be used this way, such as `-C/--continue-at`\.

```csharp
public string ArgumentName { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The option name as shown to the user\.

### Remarks

Remove or change that option in the command.

### See Also
- [InvalidPart](CurlDotNet.Exceptions.CurlInvalidCommandException.InvalidPart.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.InvalidPart')