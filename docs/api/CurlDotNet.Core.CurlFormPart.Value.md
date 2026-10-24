#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlFormPart](CurlDotNet.Core.CurlFormPart.md 'CurlDotNet\.Core\.CurlFormPart')

## CurlFormPart\.Value Property

The literal text of a text field, the part after `=` in `-F name=value` or `--form-string name=value`;
null for file parts\.

```csharp
public string? Value { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The field text, sent as is\. Default: null\.

### Remarks

`--form-string` never treats a leading `@` or `<` as a file. Example: `-F "note=hello world"` is
            `new CurlFormPart { Name = "note", Value = "hello world" }`.

### See Also
- [Name](CurlDotNet.Core.CurlFormPart.Name.md 'CurlDotNet\.Core\.CurlFormPart\.Name')
- [FilePath](CurlDotNet.Core.CurlFormPart.FilePath.md 'CurlDotNet\.Core\.CurlFormPart\.FilePath')