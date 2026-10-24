#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.FormData Property

Text fields of a `multipart/form-data` body, field name to value, as curl's `-F name=value` sends
them; filling it \(or [Files](CurlDotNet.Core.CurlOptions.Files.md 'CurlDotNet\.Core\.CurlOptions\.Files')\) turns the request into a multipart POST\.

```csharp
public System.Collections.Generic.Dictionary<string,string> FormData { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')  
Field name to text value\. Default: empty\.

### Remarks

When [FormParts](CurlDotNet.Core.CurlOptions.FormParts.md 'CurlDotNet\.Core\.CurlOptions\.FormParts') is empty, each entry becomes one text part and each [Files](CurlDotNet.Core.CurlOptions.Files.md 'CurlDotNet\.Core\.CurlOptions\.Files') entry one
            file part (fields first), with curl's boundary and part headers. When the parser filled [FormParts](CurlDotNet.Core.CurlOptions.FormParts.md 'CurlDotNet\.Core\.CurlOptions\.FormParts')
            (from `-F`), those parts are sent and this dictionary is only a summary; a field read from a file
            (`-F name=<file`) appears here as `"<file"`. Set [Method](CurlDotNet.Core.CurlOptions.Method.md 'CurlDotNet\.Core\.CurlOptions\.Method') to POST for hand-built options.

Example: `curl -F name=Ada -F avatar=@me.png URL` is
            `new CurlOptions { Url = url, Method = "POST", FormData = { ["name"] = "Ada" }, Files = { ["avatar"] = "me.png" } }`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md

### See Also
- [Files](CurlDotNet.Core.CurlOptions.Files.md 'CurlDotNet\.Core\.CurlOptions\.Files')
- [FormParts](CurlDotNet.Core.CurlOptions.FormParts.md 'CurlDotNet\.Core\.CurlOptions\.FormParts')
- [WithMultipartForm\(Dictionary&lt;string,string&gt;, Dictionary&lt;string,string&gt;\)](CurlDotNet.Core.CurlRequestBuilder.WithMultipartForm(System.Collections.Generic.Dictionary_string,string_,System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithMultipartForm\(System\.Collections\.Generic\.Dictionary\<string,string\>, System\.Collections\.Generic\.Dictionary\<string,string\>\)')