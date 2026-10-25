#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlFormPart](CurlDotNet.Core.CurlFormPart.md 'CurlDotNet\.Core\.CurlFormPart')

## CurlFormPart\.Name Property

The form field name sent in the part's `Content-Disposition: form-data; name="..."` header; the part
before `=` in curl's `-F name=value`\.

```csharp
public string Name { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The field name the server reads, such as `"avatar"`\. Default: empty string\.

### Remarks

Quotes and line breaks are percent-encoded (or backslash-escaped with [FormEscape](CurlDotNet.Core.CurlOptions.FormEscape.md 'CurlDotNet\.Core\.CurlOptions\.FormEscape')).
            Example: `-F user=ada` is `new CurlFormPart { Name = "user", Value = "ada" }`.

### See Also
- [Value](CurlDotNet.Core.CurlFormPart.Value.md 'CurlDotNet\.Core\.CurlFormPart\.Value')
- [FileName](CurlDotNet.Core.CurlFormPart.FileName.md 'CurlDotNet\.Core\.CurlFormPart\.FileName')