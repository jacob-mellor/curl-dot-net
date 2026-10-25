#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlFormPart](CurlDotNet.Core.CurlFormPart.md 'CurlDotNet\.Core\.CurlFormPart')

## CurlFormPart\.IsFileContent Property

True for `name=<path`: the content of the file at [FilePath](CurlDotNet.Core.CurlFormPart.FilePath.md 'CurlDotNet\.Core\.CurlFormPart\.FilePath') becomes the text value of the field,
with no `filename` parameter\.

```csharp
public bool IsFileContent { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
Default: false\.

### Remarks

Example: `-F "description=<notes.txt"` sends the text of `notes.txt` as the `description` field.

### See Also
- [FilePath](CurlDotNet.Core.CurlFormPart.FilePath.md 'CurlDotNet\.Core\.CurlFormPart\.FilePath')
- [IsFileUpload](CurlDotNet.Core.CurlFormPart.IsFileUpload.md 'CurlDotNet\.Core\.CurlFormPart\.IsFileUpload')