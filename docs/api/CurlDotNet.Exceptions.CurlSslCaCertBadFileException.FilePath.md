#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlSslCaCertBadFileException](CurlDotNet.Exceptions.CurlSslCaCertBadFileException.md 'CurlDotNet\.Exceptions\.CurlSslCaCertBadFileException')

## CurlSslCaCertBadFileException\.FilePath Property

The `--cacert` file or `--capath` folder that could not be used, as given in the command\.

```csharp
public string FilePath { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A path such as `corp-ca.pem`\.

### Remarks

Use `Path.GetFullPath(ex.FilePath)` to see where the process looked.

### See Also
- [CaPath](CurlDotNet.Core.CurlOptions.CaPath.md 'CurlDotNet\.Core\.CurlOptions\.CaPath')