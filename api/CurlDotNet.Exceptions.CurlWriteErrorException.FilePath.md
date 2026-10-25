#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')

## CurlWriteErrorException\.FilePath Property

The file that could not be written, as given in the command \(relative to the process's current directory\)\.

```csharp
public string FilePath { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A path such as `downloads/report.pdf`\.

### Remarks

Use `Path.GetFullPath(ex.FilePath)` to see the absolute location the process tried.

### See Also
- [FilePath](CurlDotNet.Exceptions.CurlReadErrorException.FilePath.md 'CurlDotNet\.Exceptions\.CurlReadErrorException\.FilePath')