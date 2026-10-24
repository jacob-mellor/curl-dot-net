#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException')

## CurlReadErrorException\.FilePath Property

The local file that could not be read, as given in the command \(relative to the process's current directory\)\.

```csharp
public string FilePath { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A path such as `payload.json`\.

### Remarks

Use `Path.GetFullPath(ex.FilePath)` to see where the process looked.

### See Also
- [FilePath](CurlDotNet.Exceptions.CurlWriteErrorException.FilePath.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException\.FilePath')