#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFileCouldntReadException](CurlDotNet.Exceptions.CurlFileCouldntReadException.md 'CurlDotNet\.Exceptions\.CurlFileCouldntReadException')

## CurlFileCouldntReadException\.FilePath Property

The local path from the `file://` URL that could not be opened or read by the current user\.

```csharp
public string FilePath { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
An absolute path such as `/home/me/data.json` or `C:\data\x.txt`\.

### Remarks

Check `File.Exists(ex.FilePath)` to tell a missing file from a permission problem.

### See Also
- [FilePath](CurlDotNet.Exceptions.CurlReadErrorException.FilePath.md 'CurlDotNet\.Exceptions\.CurlReadErrorException\.FilePath')