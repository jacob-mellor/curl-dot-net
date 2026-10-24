#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core')

## CurlFormPart Class

One part of a `multipart/form-data` body, as given with curl's `-F` or `--form-string`: a text field,
a file upload, or a field whose value is read from a file\.

```csharp
public class CurlFormPart
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlFormPart

### Remarks

Accepted forms: `name=value` (text field), `name=@path` (file upload; sends `filename="basename"` and a
            Content-Type guessed from the extension), `name=<path` (the file's content becomes the field value) and the modifiers
            `;type=mime/type`, `;filename=name` and `;headers=Header: value`. Values may be double-quoted to protect
            `;` and `,`.

Example: `curl -F "avatar=@me.png;type=image/png" -F name=Alice https://example.com/upload` is two parts:
            `new CurlFormPart { Name = "avatar", FilePath = "me.png", IsFileUpload = true, ContentType = "image/png" }` and
            `new CurlFormPart { Name = "name", Value = "Alice" }`, added to [FormParts](CurlDotNet.Core.CurlOptions.FormParts.md 'CurlDotNet\.Core\.CurlOptions\.FormParts').
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md

### See Also
- [FormParts](CurlDotNet.Core.CurlOptions.FormParts.md 'CurlDotNet\.Core\.CurlOptions\.FormParts')
- [FormData](CurlDotNet.Core.CurlOptions.FormData.md 'CurlDotNet\.Core\.CurlOptions\.FormData')
- [Files](CurlDotNet.Core.CurlOptions.Files.md 'CurlDotNet\.Core\.CurlOptions\.Files')

| Properties | |
| :--- | :--- |
| [ContentType](CurlDotNet.Core.CurlFormPart.ContentType.md 'CurlDotNet\.Core\.CurlFormPart\.ContentType') | The part's Content\-Type, from curl's `;type=mime/type` modifier, such as `image/png` or `application/json`; overrides the type guessed from the file extension\. |
| [FileName](CurlDotNet.Core.CurlFormPart.FileName.md 'CurlDotNet\.Core\.CurlFormPart\.FileName') | The `filename` parameter of a file part, from curl's `;filename=name` modifier; lets you upload `tmp123.bin` while the server sees `report.pdf`\. |
| [FilePath](CurlDotNet.Core.CurlFormPart.FilePath.md 'CurlDotNet\.Core\.CurlFormPart\.FilePath') | Path of the local file to upload \(for `name=@path`, with [IsFileUpload](CurlDotNet.Core.CurlFormPart.IsFileUpload.md 'CurlDotNet\.Core\.CurlFormPart\.IsFileUpload')\) or to read as the field value \(for `name=<path`, with [IsFileContent](CurlDotNet.Core.CurlFormPart.IsFileContent.md 'CurlDotNet\.Core\.CurlFormPart\.IsFileContent')\)\. |
| [Headers](CurlDotNet.Core.CurlFormPart.Headers.md 'CurlDotNet\.Core\.CurlFormPart\.Headers') | Extra headers written inside this part, from curl's `;headers=Header: value` modifier \(for example `X-Checksum: abc`\), one `"Name: value"` string per entry\. |
| [IsFileContent](CurlDotNet.Core.CurlFormPart.IsFileContent.md 'CurlDotNet\.Core\.CurlFormPart\.IsFileContent') | True for `name=<path`: the content of the file at [FilePath](CurlDotNet.Core.CurlFormPart.FilePath.md 'CurlDotNet\.Core\.CurlFormPart\.FilePath') becomes the text value of the field, with no `filename` parameter\. |
| [IsFileUpload](CurlDotNet.Core.CurlFormPart.IsFileUpload.md 'CurlDotNet\.Core\.CurlFormPart\.IsFileUpload') | True for `name=@path`: the file at [FilePath](CurlDotNet.Core.CurlFormPart.FilePath.md 'CurlDotNet\.Core\.CurlFormPart\.FilePath') is sent as an attachment with a `filename="..."` and its own Content\-Type\. |
| [MixedFiles](CurlDotNet.Core.CurlFormPart.MixedFiles.md 'CurlDotNet\.Core\.CurlFormPart\.MixedFiles') | The files of `-F "name=@a.txt,b.txt"`: when more than one file is named, each one is an entry here and they are sent together as a nested `multipart/mixed` part, like curl\. |
| [Name](CurlDotNet.Core.CurlFormPart.Name.md 'CurlDotNet\.Core\.CurlFormPart\.Name') | The form field name sent in the part's `Content-Disposition: form-data; name="..."` header; the part before `=` in curl's `-F name=value`\. |
| [Value](CurlDotNet.Core.CurlFormPart.Value.md 'CurlDotNet\.Core\.CurlFormPart\.Value') | The literal text of a text field, the part after `=` in `-F name=value` or `--form-string name=value`; null for file parts\. |
