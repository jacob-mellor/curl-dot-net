#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.FormEscape Property

Escapes form field and file names with backslashes instead of percent\-encoding, for older servers; curl's
`--form-escape`\.

```csharp
public bool FormEscape { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True for backslash escaping\. Default: false\.

### Remarks

By default curl writes a double quote in a `-F` name or file name as `%22` (and CR/LF as `%0D`/`%0A`);
            with this option it writes `\"` and doubles backslashes instead.

Example: `curl --form-escape -F "file=@\"odd\".txt" URL`.

### See Also
- [FormParts](CurlDotNet.Core.CurlOptions.FormParts.md 'CurlDotNet\.Core\.CurlOptions\.FormParts')