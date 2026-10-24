#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlFormPart](CurlDotNet.Core.CurlFormPart.md 'CurlDotNet\.Core\.CurlFormPart')

## CurlFormPart\.IsFileUpload Property

True for `name=@path`: the file at [FilePath](CurlDotNet.Core.CurlFormPart.FilePath.md 'CurlDotNet\.Core\.CurlFormPart\.FilePath') is sent as an attachment with a
`filename="..."` and its own Content\-Type\.

```csharp
public bool IsFileUpload { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
Default: false\.

### Remarks

The Content-Type is guessed from the extension unless [ContentType](CurlDotNet.Core.CurlFormPart.ContentType.md 'CurlDotNet\.Core\.CurlFormPart\.ContentType') is set; the file name is the base
            name unless [FileName](CurlDotNet.Core.CurlFormPart.FileName.md 'CurlDotNet\.Core\.CurlFormPart\.FileName') is set. Example: `-F "doc=@a.pdf"` sets this to true.

### See Also
- [FilePath](CurlDotNet.Core.CurlFormPart.FilePath.md 'CurlDotNet\.Core\.CurlFormPart\.FilePath')
- [IsFileContent](CurlDotNet.Core.CurlFormPart.IsFileContent.md 'CurlDotNet\.Core\.CurlFormPart\.IsFileContent')