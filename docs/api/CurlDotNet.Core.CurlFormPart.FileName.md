#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlFormPart](CurlDotNet.Core.CurlFormPart.md 'CurlDotNet\.Core\.CurlFormPart')

## CurlFormPart\.FileName Property

The `filename` parameter of a file part, from curl's `;filename=name` modifier; lets you upload
`tmp123.bin` while the server sees `report.pdf`\.

```csharp
public string? FileName { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A file name, or null \(default\) for the base name of [FilePath](CurlDotNet.Core.CurlFormPart.FilePath.md 'CurlDotNet\.Core\.CurlFormPart\.FilePath')\.

### Remarks

Example: `-F "file=@tmp123.bin;filename=report.pdf"` is
            `new CurlFormPart { Name = "file", FilePath = "tmp123.bin", IsFileUpload = true, FileName = "report.pdf" }`.

### See Also
- [FilePath](CurlDotNet.Core.CurlFormPart.FilePath.md 'CurlDotNet\.Core\.CurlFormPart\.FilePath')
- [ContentType](CurlDotNet.Core.CurlFormPart.ContentType.md 'CurlDotNet\.Core\.CurlFormPart\.ContentType')