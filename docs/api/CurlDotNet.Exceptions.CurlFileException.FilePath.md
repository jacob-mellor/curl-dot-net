#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFileException](CurlDotNet.Exceptions.CurlFileException.md 'CurlDotNet\.Exceptions\.CurlFileException')

## CurlFileException\.FilePath Property

The path of the file that failed, as it was given \(relative paths are relative to the process's current
directory\)\.

```csharp
public string FilePath { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A file path such as `downloads/report.pdf`; null when not known\.

### Remarks

Use `Path.GetFullPath(ex.FilePath)` to see where the process actually looked.

### See Also
- [Operation](CurlDotNet.Exceptions.CurlFileException.Operation.md 'CurlDotNet\.Exceptions\.CurlFileException\.Operation')