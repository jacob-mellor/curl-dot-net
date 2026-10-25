#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

## CurlException\.DiagnosticInfo Property

Free\-form diagnostic text attached with [WithDiagnostics\(string\)](CurlDotNet.Exceptions.CurlException.WithDiagnostics(string).md 'CurlDotNet\.Exceptions\.CurlException\.WithDiagnostics\(string\)'), such as the
output of `-v` or a trace file path, printed by [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)')\.

```csharp
public string DiagnosticInfo { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Null unless your code sets it; CurlDotNet does not fill it\.

### Remarks

Example: `throw ex.WithDiagnostics(File.ReadAllText("trace.txt"));` after running the command
            with `--trace-ascii trace.txt`, so the full wire log travels with the exception.

### See Also
- [WithDiagnostics\(string\)](CurlDotNet.Exceptions.CurlException.WithDiagnostics(string).md 'CurlDotNet\.Exceptions\.CurlException\.WithDiagnostics\(string\)')
- [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)')