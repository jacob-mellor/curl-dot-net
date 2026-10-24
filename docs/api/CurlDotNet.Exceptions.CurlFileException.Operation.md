#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFileException](CurlDotNet.Exceptions.CurlFileException.md 'CurlDotNet\.Exceptions\.CurlFileException')

## CurlFileException\.Operation Property

What was being done with [FilePath](CurlDotNet.Exceptions.CurlFileException.FilePath.md 'CurlDotNet\.Exceptions\.CurlFileException\.FilePath') when it failed \(read, write, create, delete, upload or
download\), so you can word the fix\.

```csharp
public CurlDotNet.Exceptions.CurlFileException.FileOperation Operation { get; }
```

#### Property Value
[FileOperation](CurlDotNet.Exceptions.CurlFileException.FileOperation.md 'CurlDotNet\.Exceptions\.CurlFileException\.FileOperation')  
One of the [FileOperation](CurlDotNet.Exceptions.CurlFileException.FileOperation.md 'CurlDotNet\.Exceptions\.CurlFileException\.FileOperation') values\.

### Remarks

Example: `if (ex.Operation == CurlFileException.FileOperation.Write) Console.WriteLine("Is the disk full?");`

### See Also
- [FilePath](CurlDotNet.Exceptions.CurlFileException.FilePath.md 'CurlDotNet\.Exceptions\.CurlFileException\.FilePath')