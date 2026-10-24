#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlFormPart](CurlDotNet.Core.CurlFormPart.md 'CurlDotNet\.Core\.CurlFormPart')

## CurlFormPart\.ContentType Property

The part's Content\-Type, from curl's `;type=mime/type` modifier, such as `image/png` or
`application/json`; overrides the type guessed from the file extension\.

```csharp
public string? ContentType { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A MIME type, or null \(default\) for curl's default: guessed from the extension for files, none for text fields\.

### Remarks

Example: `-F "meta={\"a\":1};type=application/json"` is
            `new CurlFormPart { Name = "meta", Value = "{\"a\":1}", ContentType = "application/json" }`.

### See Also
- [FileName](CurlDotNet.Core.CurlFormPart.FileName.md 'CurlDotNet\.Core\.CurlFormPart\.FileName')
- [Headers](CurlDotNet.Core.CurlFormPart.Headers.md 'CurlDotNet\.Core\.CurlFormPart\.Headers')