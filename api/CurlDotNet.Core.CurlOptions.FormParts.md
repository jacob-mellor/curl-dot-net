#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.FormParts Property

The parts of a `multipart/form-data` body in command\-line order, with curl's full `-F` syntax
\(file uploads, file contents, custom types, file names and part headers\)\.

```csharp
public System.Collections.Generic.List<CurlDotNet.Core.CurlFormPart> FormParts { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[CurlFormPart](CurlDotNet.Core.CurlFormPart.md 'CurlDotNet\.Core\.CurlFormPart')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
A list of [CurlFormPart](CurlDotNet.Core.CurlFormPart.md 'CurlDotNet\.Core\.CurlFormPart')\. Default: empty, in which case [FormData](CurlDotNet.Core.CurlOptions.FormData.md 'CurlDotNet\.Core\.CurlOptions\.FormData') and [Files](CurlDotNet.Core.CurlOptions.Files.md 'CurlDotNet\.Core\.CurlOptions\.Files') are used\.

### Remarks

Each `-F name=value`, `-F name=@file`, `-F name=<file` or `--form-string name=value` adds one part.
            When the list is not empty the request body is `multipart/form-data` formatted exactly like curl's (boundary, part headers
            and file content types) and [FormData](CurlDotNet.Core.CurlOptions.FormData.md 'CurlDotNet\.Core\.CurlOptions\.FormData')/[Files](CurlDotNet.Core.CurlOptions.Files.md 'CurlDotNet\.Core\.CurlOptions\.Files') are ignored for the body (the parser still fills them
            for code that reads them). A file that cannot be read fails with exit code 26.

Example: `curl -F "avatar=@me.png;type=image/png" URL` is
            `options.FormParts.Add(new CurlFormPart { Name = "avatar", FilePath = "me.png", IsFileUpload = true, ContentType = "image/png" });`
            with `Method = "POST"`. Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md

### See Also
- [CurlFormPart](CurlDotNet.Core.CurlFormPart.md 'CurlDotNet\.Core\.CurlFormPart')
- [FormData](CurlDotNet.Core.CurlOptions.FormData.md 'CurlDotNet\.Core\.CurlOptions\.FormData')
- [Files](CurlDotNet.Core.CurlOptions.Files.md 'CurlDotNet\.Core\.CurlOptions\.Files')
- [FormEscape](CurlDotNet.Core.CurlOptions.FormEscape.md 'CurlDotNet\.Core\.CurlOptions\.FormEscape')