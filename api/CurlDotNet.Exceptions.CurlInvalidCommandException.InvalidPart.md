#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')

## CurlInvalidCommandException\.InvalidPart Property

Gets the part of the command that is invalid\.

```csharp
public string InvalidPart { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The specific option, argument, or syntax element that caused the parsing error\.

### Remarks

This helps identify exactly what part of the command is wrong.

AI-Usage: Use this to provide specific feedback about what needs to be corrected.