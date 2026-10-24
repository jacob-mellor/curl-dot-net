#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlUnknownOptionException](CurlDotNet.Exceptions.CurlUnknownOptionException.md 'CurlDotNet\.Exceptions\.CurlUnknownOptionException')

## CurlUnknownOptionException\.OptionName Property

The option that curl does not recognise, exactly as written, such as `--headers`\.

```csharp
public string OptionName { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The option text including dashes\.

### Remarks

Also available as [InvalidPart](CurlDotNet.Exceptions.CurlInvalidCommandException.InvalidPart.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.InvalidPart').

### See Also
- [InvalidPart](CurlDotNet.Exceptions.CurlInvalidCommandException.InvalidPart.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException\.InvalidPart')